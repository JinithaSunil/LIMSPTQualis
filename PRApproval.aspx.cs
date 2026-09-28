using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Data;
using System.Web.Caching;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Spreadsheet;
using iTextSharp.text;

namespace LIMS
{
    public partial class PRApproval : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        DAL.PurchaseRequisition _pr = new DAL.PurchaseRequisition();
        private string currentDepartment;
        private string currentUserID;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Assuming UserID is stored in session after login
            currentUserID = Session["UserID"]?.ToString();

            if (!IsPostBack && !string.IsNullOrEmpty(currentUserID))
            {
                DataTable userDetails = _cm.GetEmpDetails(currentUserID);
                if (userDetails != null && userDetails.Rows.Count > 0)
                {
                    // Store department in ViewState
                    ViewState["CurrentDepartment"] = userDetails.Rows[0]["DepartmentID"]?.ToString() ?? string.Empty;
                    currentDepartment = ViewState["CurrentDepartment"].ToString();

                    // Department specific grids
                    if (Request.QueryString["p"] != null)
                    {
                        string PRID = Request.QueryString["p"].ToString();
                        BindGrid(PRID);
                    }
                }
                else
                {
                    // Handle case where user details cannot be retrieved
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('User Detail Not Found');", true);
                }
            }
            else
            {
                // Retrieve department from ViewState on postback
                currentDepartment = ViewState["CurrentDepartment"]?.ToString() ?? string.Empty;
            }
        }
        private void BindGrid(string PRID)
        {
            if (Session["RoleID"] != null)
            {
                if (Session["RoleID"].ToString() == "28") { btnApproveAll.Visible = true; btnRejectAll.Visible = true; }

                else { btnApproveAll.Visible = false; btnRejectAll.Visible = false; }
            }


            DataTable dtPR = _cm.GetPR(PRID);
            if (dtPR.Rows.Count > 0)
            {
                Session["PRDepartment"] = dtPR.Rows[0]["DepartmentID"].ToString();
                DataTable dtApprovals = _pr.getApprovalList(PRID);
                if (dtApprovals.Rows.Count > 0)
                {
                    hdCurrentTier.Value = dtApprovals.Rows[0]["CurrentTier"].ToString();
                    hdCurrentApprover.Value = dtApprovals.Rows[0]["CurrentApproverRole"].ToString();
                    if (dtApprovals.Rows[0]["CurrentTier"].ToString() != "4")
                    {
                        divDir.Visible = false;
                        ddl.DataSource = dtApprovals;
                        ddl.DataTextField = "Approver";
                        ddl.DataValueField = "ID";
                        ddl.DataBind();
                        ddl.Items.Insert(0, "Select Next Approver");
                        if (hdCurrentTier.Value == "2")
                        {
                            if ((dtPR.Rows[0]["DepartmentID"].ToString().Contains("1008") || (dtPR.Rows[0]["DepartmentID"].ToString().Contains("8022"))))
                            {
                                ddl.ClearSelection();
                                ddl.Items.FindByValue("7").Selected = true;
                                hdNextApprovalLevel.Value = "3";
                            }
                            else
                            {
                                ddl.ClearSelection();
                                ddl.Items.FindByValue("8").Selected = true;
                                hdNextApprovalLevel.Value = "3";
                            }
                            ddl.Enabled = false;
                        }
                        if (hdCurrentTier.Value == "3")
                        {
                            ddl.ClearSelection();
                            ddl.Items.FindByValue("9").Selected = true;
                            hdNextApprovalLevel.Value = "4";
                            ddl.Enabled = false;
                        }
                        if (hdCurrentTier.Value == "4")
                        {
                            hdNextApprovalLevel.Value = "";
                        }
                    }
                    else
                    {
                        divNextApprover.Visible = false;
                        divDir.Visible = true;
                    }
                }
                else
                {
                    hdCurrentTier.Value = "3";
                    hdCurrentApprover.Value = "Director";
                    divNextApprover.Visible = false;
                    divDir.Visible = true;
                }
                // Hide GM GridView and show Requests GridView
                gvRequests.Visible = true;


                try
                {
                    DataTable dtDetails = _cm.GetPRDetails(PRID);
                    if (dtDetails.Rows.Count > 0)
                    {
                        gvRequests.DataSource = dtDetails;
                        gvRequests.VirtualItemCount = dtDetails.Rows.Count;
                        gvRequests.DataBind();
                        hfTotalRows.Value = dtDetails.Rows.Count.ToString();
                        ViewState["PRData"] = dtDetails;
                    }
                    else
                    {
                        gvRequests.DataSource = null;
                        gvRequests.DataBind();
                    }
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('Error: {ex.Message}');", true);
                }
            }


        }
        private void BindGrid(string PRID, int pageIndex, int pageSize)
        {
            DataTable dt = (DataTable)ViewState["PRData"];

            var pagedRows = dt.AsEnumerable()
                              .Skip(pageIndex * pageSize)
                              .Take(pageSize);

            if (pagedRows.Any())
            {
                gvRequests.DataSource = pagedRows.CopyToDataTable();
                gvRequests.VirtualItemCount = dt.Rows.Count;
            }
            else
            {
                gvRequests.DataSource = null;
            }
            gvRequests.DataBind();


        }
        private void BindGridGM()
        {
            // Show GM GridView and hide Requests GridView
            gvRequests.Visible = false;

            string query = @"
                   SELECT 
                    RIGHT('0000' + CAST(PRNumber AS VARCHAR(4)), 4) + '/' + RIGHT('0' + CAST(MONTH(RequestDate) AS VARCHAR(2)), 2) + '/' + cast(year(RequestDate) as varchar) + '/' + deptcode AS PR,
                    pr.PRID AS PRID,pd.PurchaseRequisitionID,
                    e.EmployeeName,
                    i.ItemDescription AS Description,
                    d.EmployeeDepartmentName,
                    pr.RequestDate,
                    pd.StatusID as Status,
                    pt.PRType,
                    pr.StoreType
                    FROM PurchaseRequisition pr
                    LEFT JOIN PurchaseRequisitionDetails pd ON pr.PRID = pd.PRID
                    LEFT JOIN Items i ON i.itemcode = pd.ItemCode
                    LEFT JOIN Employees e ON e.ID = pr.RequestBy
                    LEFT JOIN EmployeeDepartment d ON d.EmployeeDepartmentID = pr.DepartmentID
                    LEFT JOIN PurchaseRequisitionStatus ps ON ps.StatusID = pd.StatusID
                    LEFT JOIN PRTypes pt ON pt.PRTypesID = pr.PRTypeID
                    WHERE pd.StatusID = 1005 and pd.StatusID not in(1010,1012,1014,1016)";



            query += " ORDER BY pr.PRID DESC";
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {

                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            sda.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('Error: {ex.Message}');", true);
            }
        }

        protected void imgSearch_Click(object sender, ImageClickEventArgs e)
        {
            // Get the current department from ViewState
            currentDepartment = ViewState["CurrentDepartment"]?.ToString() ?? string.Empty;

            if (string.IsNullOrEmpty(currentDepartment))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Department not found. Please refresh the page.');", true);
                return;
            }

            // Bind the appropriate grid based on department
            switch (currentDepartment.ToUpper())
            {
                case "2009": // Stores
                case "2008": // Purchase
                    BindGrid(currentDepartment);
                    break;

                case "3008": // GM
                    BindGridGM();
                    break;

                default:
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Error: Invalid Department');", true);
                    break;
            }
        }

        //Upper section
        protected void ddlPR_SelectedIndexChanged(object sender, EventArgs e)
        {
            string currentDepartment = ViewState["CurrentDepartment"]?.ToString() ?? string.Empty;
            // Call the appropriate grid binding method based on the department
            switch (currentDepartment.ToUpper())
            {
                case "2009": // Stores
                    BindGrid(currentDepartment);
                    break;
                case "2008": // Purchase
                    BindGrid(currentDepartment);
                    break;

                case "3008": // GM
                    BindGridGM();
                    break;

                default:
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Invalid Department');", true);
                    break;
            }
        }

        //Buttons
        protected void btnDownload_Click(object sender, EventArgs e)
        {
            // Export the GridView to Excel
            ExportGridViewToExcel(gvRequests, "GridViewData.xlsx");
        }
        private void ExportGridViewToExcel(GridView gridView, string fileName)
        {
            // Create an instance of Excel application
            Excel.Application excelApp = new Excel.Application();
            excelApp.Visible = false; // Do not show Excel application
            excelApp.DisplayAlerts = false; // Suppress alerts

            // Add a workbook
            Excel.Workbook workbook = excelApp.Workbooks.Add(Type.Missing);
            Excel.Worksheet worksheet = workbook.ActiveSheet;

            try
            {
                // Add headers
                for (int i = 0; i < gridView.HeaderRow.Cells.Count; i++)
                {
                    worksheet.Cells[1, i + 1] = gridView.HeaderRow.Cells[i].Text;
                }

                // Add rows
                for (int i = 0; i < gridView.Rows.Count; i++)
                {
                    for (int j = 0; j < gridView.Rows[i].Cells.Count; j++)
                    {
                        worksheet.Cells[i + 2, j + 1] = gridView.Rows[i].Cells[j].Text;
                    }
                }

                // Set the path to the Downloads folder on the C drive
                string downloadFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                string filePath = Path.Combine(downloadFolder, fileName);

                // Save the workbook to the Downloads folder
                workbook.SaveAs(filePath);

                // Close the workbook and quit Excel
                workbook.Close(false);
                excelApp.Quit();

                // Send the file to the client
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
                Response.TransmitFile(filePath);
                Response.End();

                // Optionally, delete the file after sending to the client (if required)
                // File.Delete(filePath);
            }
            catch (Exception ex)
            {
                // Handle errors
                Response.Write("An error occurred: " + ex.Message);
            }
            finally
            {
                // Release COM objects to avoid memory leaks
                if (workbook != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                if (excelApp != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
            }
        }
        protected void btnChangeStatus_Click(object sender, EventArgs e)
        {
            Button btnChangeStatus = (Button)sender;
            GridViewRow row = (GridViewRow)btnChangeStatus.NamingContainer;
            GridView gv = (GridView)row.NamingContainer;
            int rowIndex = row.RowIndex;
            DropDownList ddlStatus = (DropDownList)row.FindControl("ddlStatus");
            _cm.PurchaseRequisitionID = Convert.ToInt32(gv.DataKeys[rowIndex].Values[0]);
            _cm.PRID = Convert.ToInt32(gv.DataKeys[rowIndex].Values[1]);
            string newStatusID = ddlStatus.SelectedValue;
            string StatusName = ddlStatus.SelectedItem.ToString();
            bool isRejected = newStatusID == "1010" || newStatusID == "1012" || newStatusID == "1014" || newStatusID == "1016"; // ADDED- 23Feb26

            if (ddlStatus.SelectedIndex == 0 && !isRejected) // MODIFIED - 23Feb26
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please select Next Approver.');", true);
                return;
            }
            if (string.IsNullOrEmpty(newStatusID))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please select a status.');", true);
                return;
            }
            if (ddlStatus.SelectedIndex == 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please select Next Approver.');", true);
                return;
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    string updateQuery = @"
                UPDATE PurchaseRequisitionDetails 
                SET StatusID = @StatusID 
                WHERE PurchaseRequisitionID = @PurchaseRequisitionID";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@StatusID", newStatusID);
                        cmd.Parameters.AddWithValue("@PurchaseRequisitionID", _cm.PurchaseRequisitionID);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                var User = new DAL.User();
                User.EmployeeID = Session["EmployeeID"]?.ToString();
                User.DepartmentID = Session["DepartmentID"]?.ToString();
                User.UserID = Session["UserID"]?.ToString();
                User.RoleID = Session["RoleID"]?.ToString();
                User.Remarks = hdCurrentApprover.Value + "-" + StatusName;
                User.PRStatus = newStatusID.ToString();
                User.PurchaseRequisitionDetailsID = _cm.PurchaseRequisitionID.ToString();
                _cm.AddPRHistory(User);
                if (isRejected) // ADDED BLOCK - 23Feb26
                {
                    //_cm.CurrentTier = 1;
                    //_cm.CurrentApproverRole = hdCurrentApprover.Value;
                    //_cm.PurchaserequisitionSetApprover(_cm);
                    // Response.Redirect("PRList.aspx");
                }
                else
                {
                    if (hdNextApprovalLevel.Value == "")
                    {
                        if (((ddl.SelectedIndex != -1) && (ddl.SelectedIndex != 0)) && (ddl.SelectedItem.ToString() == "Skip Level-2"))
                        {
                            _cm.CurrentTier = Convert.ToInt32(hdCurrentTier.Value) + 2;
                            string ApproverID = "";
                            DataTable dtApprovers = _cm.GetApprover(_cm.CurrentTier);
                            if (dtApprovers.Rows.Count > 0)
                            {
                                string DepartmentID = Session["PRDepartment"].ToString();
                                if ((DepartmentID.Contains("1008") || (DepartmentID.Contains("8022"))))
                                {
                                    ApproverID = "7";
                                }
                                else
                                {
                                    ApproverID = "8";
                                }
                                DataRow[] rows = dtApprovers.Select($"ID = '{ApproverID}'");

                                if (rows.Length > 0)
                                {
                                    _cm.CurrentApproverRole = rows[0]["Approver"].ToString();
                                }
                            }
                            else
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Not Assigned. Select a different approver!');", true);
                            }
                        }
                        else if (((ddl.SelectedIndex != -1) && (ddl.SelectedIndex != 0)) && (ddl.SelectedItem.ToString() == "Skip Level-3"))
                        {
                            _cm.CurrentTier = Convert.ToInt32(hdCurrentTier.Value) + 3;
                            string ApproverID = "";
                            DataTable dtApprovers = _cm.GetApprover(_cm.CurrentTier);
                            if (dtApprovers.Rows.Count > 0)
                            {
                                ApproverID = "9";

                                DataRow[] rows = dtApprovers.Select($"ID = '{ApproverID}'");

                                if (rows.Length > 0)
                                {
                                    _cm.CurrentApproverRole = rows[0]["Approver"].ToString();
                                }
                            }
                            else
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Not Assigned. Select a different approver!');", true);
                            }
                        }
                        
                    }
                    else
                    {
                        _cm.CurrentTier = Convert.ToInt32(hdNextApprovalLevel.Value);
                        _cm.CurrentApproverRole = ddl.SelectedItem.ToString();
                    }

                    _cm.PurchaserequisitionSetApprover(_cm);
                }
                int approved = Convert.ToInt32(hfApprovedRows.Value);
                hfApprovedRows.Value = (approved + 1).ToString();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Status updated successfully.');", true);
                if (hfApprovedRows.Value == hfTotalRows.Value)
                {
                    Response.Redirect("PRList.aspx");
                }


            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('Error updating status: {ex.Message}');", true);
            }
        }

        //Grid(HOD's)
        protected void gvRequests_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlStatus = (DropDownList)e.Row.FindControl("ddlStatus");
                DataTable dt = _cm.getNextPRStatus(Convert.ToInt32(hdCurrentTier.Value));
                if (dt.Rows.Count > 0)
                {
                    ddlStatus.DataSource = dt;
                    ddlStatus.DataTextField = "Status";
                    ddlStatus.DataValueField = "StatusID";
                    ddlStatus.DataBind();
                    ddlStatus.Items.Insert(0, "Select Status");
                }
            }
        }
        protected void gvRequests_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                int pageIndex = e.NewPageIndex;
                int pageSize = gvRequests.PageSize;
                // Set the new page index
                gvRequests.PageIndex = e.NewPageIndex;

                // Get the department ID from ViewState (already stored in Page_Load)
                string departmentID = ViewState["CurrentDepartment"]?.ToString() ?? string.Empty;

                // Rebind the grid with the same data
                BindGrid(departmentID, pageIndex, pageSize);

            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    $"alert('Paging Error: {ex.Message}');", true);
            }
        }

        protected void btnApproveAll_Click(object sender, EventArgs e)
        {
            All("Approve");
        }

        protected void btnRejectAll_Click(object sender, EventArgs e)
        {
            All("Reject");
        }

        protected void All(string status1)
        {
            int prId = Convert.ToInt32(Request.QueryString["p"].ToString());
            string status = status1 == "Approve" ? "1015" : "1016";
            string StatusName = status1 == "Approve" ? "Approved" : "Rejected";

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
            {
                string updateQuery = @"
                UPDATE PurchaseRequisitionDetails 
                SET StatusID = @StatusID 
                WHERE PRID = @PRID";

                using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                {
                    cmd.Parameters.AddWithValue("@PRID", prId);
                    cmd.Parameters.AddWithValue("@StatusID", status);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            var User = new DAL.User();
            User.EmployeeID = Session["EmployeeID"]?.ToString();
            User.DepartmentID = Session["DepartmentID"]?.ToString();
            User.UserID = Session["UserID"]?.ToString();
            User.RoleID = Session["RoleID"]?.ToString();
            User.Remarks = hdCurrentApprover.Value + "-" + StatusName;
            User.PRStatus = status;
            DataTable dtPRDetails = _cm.GetPRDetailsID(prId.ToString());
            if (dtPRDetails.Rows.Count > 0)
            {
                foreach (DataRow dr in dtPRDetails.Rows)
                {
                    User.PurchaseRequisitionDetailsID = dr["PurchaseRequisitionID"].ToString();
                    _cm.AddPRHistory(User);
                }
            }
            if (StatusName == "Approve")
            {
                _cm.CurrentTier = 5;
                _cm.CurrentApproverRole = "Purchase";
                _cm.PurchaserequisitionSetApprover(_cm);
            }
            Response.Redirect("PRList.aspx");
        }

        protected void btnSubmitStatus_Click(object sender, EventArgs e)
        {
            bool allSelected = true;
            if (ddl.SelectedIndex==0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                    "alert('Please select next approver');", true);
                return;
            }

            foreach (GridViewRow row in gvRequests.Rows)
            {
                DropDownList ddlStatus = (DropDownList)row.FindControl("ddlStatus");

                if (ddlStatus.SelectedValue == "" || ddlStatus.SelectedValue == "Select Status")
                {
                    allSelected = false;
                    break;
                }
            }

            if (!allSelected)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                    "alert('Please select status for all items before submitting.');", true);
                return;
            }
           
            foreach (GridViewRow row in gvRequests.Rows)
            {
                HiddenField hfPRDetailID = (HiddenField)row.FindControl("hfPRDetailID");
                DropDownList ddlStatus = (DropDownList)row.FindControl("ddlStatus");

                int prDetailID = Convert.ToInt32(hfPRDetailID.Value);
                string status = ddlStatus.SelectedValue;
                string statusName = ddlStatus.SelectedItem.ToString();

                if (!string.IsNullOrEmpty(status))
                {
                    _cm.UpdatePRDetailStatus(prDetailID, status, statusName);
                    var User = new DAL.User();
                    User.EmployeeID = Session["EmployeeID"]?.ToString();
                    User.DepartmentID = Session["DepartmentID"]?.ToString();
                    User.UserID = Session["UserID"]?.ToString();
                    User.RoleID = Session["RoleID"]?.ToString();
                    User.Remarks = hdCurrentApprover.Value + "-" + statusName;
                    User.PRStatus = status.ToString();
                    User.PurchaseRequisitionDetailsID = prDetailID.ToString();
                    _cm.AddPRHistory(User);
                }
            }
            SetNextApprover();
            Response.Redirect("PRList.aspx");
        }
        protected void SetNextApprover()
        {
            if (Request.QueryString["p"] != null)
            {
                _cm.PRID = Convert.ToInt32(Request.QueryString["p"].ToString());
                if (hdNextApprovalLevel.Value == "")
                {
                    if (((ddl.SelectedIndex != -1) && (ddl.SelectedIndex != 0)) && (ddl.SelectedItem.ToString() == "Skip Level-2"))
                    {
                        hdNextApprovalLevel.Value = "3";
                        _cm.CurrentTier = Convert.ToInt32(hdCurrentTier.Value) + 2;
                        string ApproverID = "";
                        DataTable dtApprovers = _cm.GetApprover(_cm.CurrentTier);
                        if (dtApprovers.Rows.Count > 0)
                        {
                            string DepartmentID = Session["PRDepartment"].ToString();
                            if ((DepartmentID.Contains("1008") || (DepartmentID.Contains("8022"))))
                            {
                                ApproverID = "7";
                            }
                            else
                            {
                                ApproverID = "8";
                            }
                            DataRow[] rows = dtApprovers.Select($"ID = '{ApproverID}'");

                            if (rows.Length > 0)
                            {
                                _cm.CurrentApproverRole = rows[0]["Approver"].ToString();
                            }
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Not Assigned. Select a different approver!');", true);
                        }
                    }
                    else
                    if (((ddl.SelectedIndex != -1) && (ddl.SelectedIndex != 0)) && (ddl.SelectedItem.ToString() == "Skip Level-3"))
                    {
                        hdNextApprovalLevel.Value = "4";
                        _cm.CurrentTier = Convert.ToInt32(hdCurrentTier.Value) + 3;
                        string ApproverID = "";
                        DataTable dtApprovers = _cm.GetApprover(_cm.CurrentTier);
                        if (dtApprovers.Rows.Count > 0)
                        {
                            ApproverID = "9";

                            DataRow[] rows = dtApprovers.Select($"ID = '{ApproverID}'");

                            if (rows.Length > 0)
                            {
                                _cm.CurrentApproverRole = rows[0]["Approver"].ToString();
                            }
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Not Assigned. Select a different approver!');", true);
                        }
                    }
                    else
                    {
                            if (hdCurrentTier.Value != "4")
                            {
                                hdNextApprovalLevel.Value = "2";
                                _cm.CurrentTier = Convert.ToInt32(hdNextApprovalLevel.Value);
                                _cm.CurrentApproverRole = ddl.SelectedItem.ToString();
                            }
                            else
                            {
                                _cm.CurrentTier = Convert.ToInt32(hdCurrentTier.Value);
                                _cm.CurrentApproverRole = hdCurrentApprover.Value;
                            }  
                    }
                }
                else
                {
                    _cm.CurrentTier = Convert.ToInt32(hdNextApprovalLevel.Value);
                    _cm.CurrentApproverRole = ddl.SelectedItem.ToString();
                }

                _cm.PurchaserequisitionSetApprover(_cm);
            
            }
        }
    }
    //Grid(GM's)

}
