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

namespace LIMS
{
    public partial class PurchaseRequisitionListHOD : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
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

                    BindPendingPR();

                    // Department specific grids
                    switch (currentDepartment.ToUpper())
                    {
                        case "3008": // GM
                            BindGridGM();
                            break;

                        default:
                            BindGrid(currentDepartment);
                            break;
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

        private void BindGrid(string departmentID)
        {
            // Hide GM GridView and show Requests GridView
            gvGM.Visible = false;
            gvRequests.Visible = true;

            if (string.IsNullOrEmpty(departmentID))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Department not specified.');", true);
                return;
            }

            string selectedPRType = ddlPRType.SelectedItem.Text;
            string searchText = TextBox1.Text.Trim();

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
                    WHERE pd.StatusID = 1 
                    AND pr.DepartmentID = @DepartmentID";

            // Add search condition if search text is provided
            if (!string.IsNullOrEmpty(searchText))
            {
                query += " AND pr.PRID LIKE @SearchText";
            }

            // Add PRType filter if not "All"
            if (selectedPRType != "All")
            {
                query += " AND pt.PRType = @PRType";
            }
            query += " ORDER BY pr.PRID DESC";
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@DepartmentID", departmentID);

                        // Add search parameter if search text is provided
                        if (!string.IsNullOrEmpty(searchText))
                        {
                            cmd.Parameters.AddWithValue("@SearchText", "%" + searchText + "%");
                        }

                        // Add PRType parameter if not "All"
                        if (selectedPRType != "All")
                        {
                            cmd.Parameters.AddWithValue("@PRType", selectedPRType);
                        }

                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            sda.Fill(dt);
                            gvRequests.DataSource = dt;
                            gvRequests.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('Error: {ex.Message}');", true);
            }
        }

        private void BindGridGM()
        {
            // Show GM GridView and hide Requests GridView
            gvGM.Visible = true;
            gvRequests.Visible = false;

            string selectedPRType = ddlPRType.SelectedItem.Text;
            string searchText = TextBox1.Text.Trim();

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
                    WHERE pd.StatusID = 1005";

            // Add search condition if search text is provided
            if (!string.IsNullOrEmpty(searchText))
            {
                query += " AND pr.PRID LIKE @SearchText";
            }

            // Add PRType filter if not "All"
            if (selectedPRType != "All")
            {
                query += " AND pt.PRType = @PRType";
            }
            query += " ORDER BY pr.PRID DESC";
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // Add search parameter if search text is provided
                        if (!string.IsNullOrEmpty(searchText))
                        {
                            cmd.Parameters.AddWithValue("@SearchText", "%" + searchText + "%");
                        }

                        // Add PRType parameter if not "All"
                        if (selectedPRType != "All")
                        {
                            cmd.Parameters.AddWithValue("@PRType", selectedPRType);
                        }

                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            sda.Fill(dt);
                            gvGM.DataSource = dt;
                            gvGM.DataBind();
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
        private void BindPendingPR()
        {
            System.Data.DataTable dt = _cm.GetPRTypes();
            if (dt.Rows.Count > 0)
            {
                ddlPRType.DataSource = dt;
                ddlPRType.DataTextField = "PRType";
                ddlPRType.DataValueField = "PRTypesID";
                ddlPRType.DataBind();
                ddlPRType.Items.Insert(0, new ListItem("All"));
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
        protected void btnChangeStatusss_Click(object sender, EventArgs e)
        {
            // Get the clicked button
            Button btnChangeStatus = (Button)sender;

            // Find the GridView row that the button belongs to
            GridViewRow row = (GridViewRow)btnChangeStatus.NamingContainer;

            // Find the DropDownList in the same row as the button
            DropDownList ddlStatus = (DropDownList)row.FindControl("ddlStatus");

            // Get the PRID from the row (assuming it's in the first column, adjust if needed)
            string prID = row.Cells[1].Text; // Ensure this is the correct index for your PRID column

            // Get the selected StatusID from the DropDownList (this should be 6 for "Approve", 7 for "Reject")
            string newStatusID = ddlStatus.SelectedValue;

            // Check if a valid status is selected (if no selection is made, the value will be "")
            if (string.IsNullOrEmpty(newStatusID))
            {
                // No status selected, show an alert to the user
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please select a status.');", true);
                return; // Stop further processing
            }

            // Update the status in the database
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
                        cmd.Parameters.AddWithValue("@StatusID", newStatusID); // Use the StatusID from the DropDownList
                        cmd.Parameters.AddWithValue("@PRID", prID); // PRID to identify the row to update

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                // Show a success alert if the update was successful
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Status updated successfully.');", true);

                // Refresh the entire page
                ScriptManager.RegisterStartupScript(this, this.GetType(), "refresh", "location.reload();", true);

            }
            catch (Exception ex)
            {
                // If there is an error, you can log it and show a failure message
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Error updating status. Please try again.');", true);
            }
        }
        protected void btnChangeStatus_Click(object sender, EventArgs e)
        {
            Button btnChangeStatus = (Button)sender;
            GridViewRow row = (GridViewRow)btnChangeStatus.NamingContainer;
            GridView gv = (GridView)row.NamingContainer;
            int rowIndex = row.RowIndex;
            DropDownList ddlStatus = (DropDownList)row.FindControl("ddlStatus");
            int PurchaseRequisitionID = Convert.ToInt32(gv.DataKeys[rowIndex].Value);
            string newStatusID = ddlStatus.SelectedValue;
            string StatusName = ddlStatus.SelectedItem.ToString();

            if (string.IsNullOrEmpty(newStatusID))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please select a status.');", true);
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
                        cmd.Parameters.AddWithValue("@PurchaseRequisitionID", PurchaseRequisitionID);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                var User = new DAL.User();
                User.EmployeeID = Session["EmployeeID"]?.ToString();
                User.DepartmentID = Session["DepartmentID"]?.ToString();
                User.UserID = Session["UserID"]?.ToString();
                User.RoleID = Session["RoleID"]?.ToString();
                if (User.RoleID == "2")
                {
                    User.Remarks = "Manager - " + StatusName;
                }
                if (User.RoleID == "10")
                {
                    User.Remarks = "GM - " + StatusName;
                }
                User.PRStatus = newStatusID.ToString();
                User.PurchaseRequisitionDetailsID = PurchaseRequisitionID.ToString();
                _cm.AddPRHistory(User);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Status updated successfully.');", true);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "refresh", "location.reload();", true);
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
                string currentStatus = DataBinder.Eval(e.Row.DataItem, "Status").ToString();
                ddlStatus.SelectedValue = currentStatus;
            }
        }
        protected void gvRequests_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                // Set the new page index
                gvRequests.PageIndex = e.NewPageIndex;

                // Get the department ID from ViewState (already stored in Page_Load)
                string departmentID = ViewState["CurrentDepartment"]?.ToString() ?? string.Empty;

                // Rebind the grid with the same data
                BindGrid(departmentID);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    $"alert('Paging Error: {ex.Message}');", true);
            }
        }

        //Grid(GM's)
        protected void gvGM_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvGM.PageIndex = e.NewPageIndex;
                BindGridGM();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    $"alert('GM Grid Paging Error: {ex.Message}');", true);
            }
        }
        protected void gvGM_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlStatus = (DropDownList)e.Row.FindControl("ddlStatus");
                string currentStatus = DataBinder.Eval(e.Row.DataItem, "Status").ToString();
                ddlStatus.SelectedValue = currentStatus;
            }
        }
    }
}