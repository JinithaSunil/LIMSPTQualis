using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Math;

namespace LIMS
{
    public partial class PurchaseRequisitionList : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        public class PRStatusVM
        {
            public string DeptStatus { get; set; }
            public string ITHCGAStatus { get; set; }
            public string DirSalesStatus { get; set; }
            public string DirectorStatus { get; set; }

            public string QuoteStatus { get; set; } // Create / No
            public string POStatus { get; set; }
            public string GRNStatus { get; set; }
        }
        private void SetApprovalStatus(Label lbl,string str)
        {
            lbl.Text = string.IsNullOrEmpty(str) ? "-" : str;
            lbl.CssClass = "status-circle";

            switch (lbl.Text)
            {
                case "Approved by Tier-1":
                case "Approved by Tier-2":
                case "Approved by Tier-3":
                case "Approved by Tier-4":
                    lbl.Text = "A";
                    lbl.CssClass += " approved";
                    break;
                case "Rejected by Tier-1":
                case "Rejected by Tier-2":
                case "Rejected by Tier-3":
                case "Rejected by Tier-4":
                    lbl.Text = "R";
                    lbl.CssClass += " reject";
                    break;
                case "New":
                    lbl.Text = "N";
                    lbl.CssClass += " new";
                    break;
                default:
                    lbl.CssClass += " disabled";
                    break;
            }
        }
        private void SetFlowStatus(Label lbl, string status)
        {
            lbl.Text = status;

            if (status == "Create")
                lbl.CssClass += " green";
            else
                lbl.CssClass += " red";
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    DataTable dt1 = _cm.GetEmployeeDetailsNew(Session["UserID"].ToString());
                    if (dt1.Rows.Count > 0)
                    {
                        Session["DepartmentID"] = dt1.Rows[0]["DepartmentID"].ToString();
                    }
                    BindGrid();
                    BindPendingPR();
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }
        private void BindPendingPR()
        {
            DataTable dt = _cm.GetPRTypes();
            if (dt.Rows.Count > 0)
            {
                ddlPRType.DataSource = dt;
                ddlPRType.DataTextField = "PRType";
                ddlPRType.DataValueField = "PRTypesID";
                ddlPRType.DataBind();
                ddlPRType.Items.Insert(0, new ListItem("All"));
            }
        }
        private void BindGrid(string sortExpression = "", string sortDirection = "ASC")
        {
            try
            {
                // Always fetch fresh data from database - don't rely on ViewState for search operations
                // Get filter values
                string selectedPRType = ddlPRType.SelectedItem.Text;
                string searchText = TextBox1.Text.Trim();

                int userDepartmentID = 0;
                if (Session["DepartmentID"] != null && int.TryParse(Session["DepartmentID"].ToString(), out userDepartmentID))
                {
                    // Department ID is available
                }

                // Base query
                string query = @"
                SELECT 
                    RIGHT('0000' + CAST(PRNumber AS VARCHAR(4)), 4) + '/' + RIGHT('0' + CAST(MONTH(RequestDate) AS VARCHAR(2)), 2) + '/' + cast(year(RequestDate) as varchar) + '/' + deptcode AS PR,
                    pr.PRID,pd.PurchaseRequisitionID,
                    e.EmployeeName,
                    i.ItemDescription AS Description,
                    d.EmployeeDepartmentName,
                    pr.RequestDate,
                    ps.Status,
                    pt.PRType,
                    pr.StoreType
                FROM PurchaseRequisition pr
                INNER JOIN PurchaseRequisitionDetails pd ON pr.PRID = pd.PRID
                LEFT JOIN Items i ON i.itemcode = pd.ItemCode
                LEFT JOIN Employees e ON e.ID = pr.RequestBy
                LEFT JOIN EmployeeDepartment d ON d.EmployeeDepartmentID = pr.DepartmentID
                LEFT JOIN PurchaseRequisitionStatus ps ON ps.StatusID = pd.StatusID
                LEFT JOIN PRTypes pt ON pt.PRTypesID = pr.PRTypeID
                WHERE 1=1"; // Base condition for easier WHERE clause building

                // Add filters
                List<SqlParameter> parameters = new List<SqlParameter>();

                // Add department filter - show only records from user's department
                if (userDepartmentID > 0)
                {
                    query += " AND d.EmployeeDepartmentID = @DepartmentID";
                    parameters.Add(new SqlParameter("@DepartmentID", userDepartmentID));
                }

                // Add other filters
                if (selectedPRType != "All")
                {
                    query += " AND pt.PRType = @PRType";
                    parameters.Add(new SqlParameter("@PRType", selectedPRType));
                }

                if (!string.IsNullOrEmpty(searchText))
                {
                    query += " AND (CAST(pr.PRID AS VARCHAR) LIKE @SearchText OR e.EmployeeName LIKE @SearchText OR i.ItemDescription LIKE @SearchText)";
                    parameters.Add(new SqlParameter("@SearchText", "%" + searchText + "%"));
                }

                // Add sorting
                if (!string.IsNullOrEmpty(sortExpression))
                {
                    if (sortExpression == "Status")
                    {
                        query += " ORDER BY CASE ps.Status " +
                                "WHEN 'New' THEN 1 " +
                                "WHEN 'Approved' THEN 2 " +
                                "WHEN 'Rejected' THEN 3 " +
                                "WHEN 'Approved by HOD' THEN 4 " +
                                "WHEN 'Approved by GM' THEN 5 " +
                                "ELSE 6 END " + sortDirection;
                    }
                    else
                    {
                        // Validate sortExpression to prevent SQL injection
                        string[] validColumns = { "PR", "EmployeeName", "Description", "EmployeeDepartmentName", "RequestDate", "PRType", "StoreType" };
                        if (validColumns.Contains(sortExpression))
                        {
                            query += " ORDER BY " + sortExpression + " " + sortDirection;
                        }
                    }
                }
                else
                {
                    // Default sorting
                    query += " ORDER BY pr.RequestDate DESC";
                }

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // Add parameters
                        if (parameters.Count > 0)
                        {
                            cmd.Parameters.AddRange(parameters.ToArray());
                        }

                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            sda.Fill(dt);

                            // Only store the data in ViewState after successful database query
                            ViewState["GridData"] = dt;
                            ViewState["SortDirection"] = sortDirection;
                            ViewState["SortExpression"] = sortExpression;

                            // Bind the DataTable to the GridView
                            gvRequests.DataSource = dt;
                            gvRequests.DataBind();

                            // Show "No records found" message if empty
                            if (dt.Rows.Count == 0)
                            {
                                DataRow emptyRow = dt.NewRow();
                                dt.Rows.Add(emptyRow);
                                gvRequests.DataSource = dt;
                                gvRequests.DataBind();
                                gvRequests.Rows[0].Cells.Clear();
                                gvRequests.Rows[0].Cells.Add(new TableCell());
                                gvRequests.Rows[0].Cells[0].ColumnSpan = gvRequests.Columns.Count;
                                gvRequests.Rows[0].Cells[0].Text = "No records found";
                                gvRequests.Rows[0].Cells[0].HorizontalAlign = HorizontalAlign.Center;
                                gvRequests.Rows[0].Cells[0].CssClass = "no-records";
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                ShowMessage("Database error occurred. Please try again later.", MessageType.Error);
            }
            catch (Exception ex)
            {
                ShowMessage("An unexpected error occurred. Please contact support.", MessageType.Error);
            }
        }
        private void ShowMessage(string message, MessageType type)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "notification",
                $"showNotification('{message}', '{type.ToString().ToLower()}');", true);
        }
        public enum MessageType
        {
            Success,
            Error,
            Warning,
            Info
        }

        //ddl
        protected void ddlProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            //BindRoleBasedPRList();
        }
        protected void ddlPR_SelectedIndexChanged(object sender, EventArgs e)
        {

            BindGrid();
        }

        //Buttons
        protected void imgSearch_Click(object sender, ImageClickEventArgs e)
        {
            // Clear all cached data and reset pagination
            ViewState["GridData"] = null;
            ViewState["SortExpression"] = null;
            ViewState["SortDirection"] = "ASC";
            gvRequests.PageIndex = 0;

            // Bind grid with search filter
            BindGrid();
        }
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

        //Grid
        protected void gvRequests_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Repeater rptStatus = (Repeater)e.Row.FindControl("rptStatus");
                HiddenField hdPR = (HiddenField)e.Row.FindControl("hdPR");
                HiddenField hdPRD = (HiddenField)e.Row.FindControl("hdPRD");
                if (hdPR != null)
                {
                    string PurchaseRequisitionID = hdPRD.Value;
                    DataTable dtStatus = _cm.GetPRStatus(PurchaseRequisitionID);
                    if ((dtStatus.Rows.Count > 0))
                    {
                        if (rptStatus != null)
                        {
                            try
                            {
                                rptStatus.DataSource = dtStatus;
                                rptStatus.DataBind();
                            }
                            catch (Exception ex) {
                                string s = ex.Message;
                            }
                           
                        }
                    }
                }
                // Assuming status is in the 7th column (index 6)
                if (e.Row.Cells.Count > 6)
                {
                    string originalStatus = e.Row.Cells[6].Text;

                    if (!string.IsNullOrEmpty(originalStatus) && originalStatus != "&nbsp;")
                    {
                        string status = originalStatus.Trim();
                        string displayStatus = status;
                        string pillClass = "pill";

                        // Normalize status value for logic
                        if (status.Equals("Mail Sent to Vendor", StringComparison.OrdinalIgnoreCase))
                        {
                            status = "Approved";         // For styling
                            displayStatus = "Approved";  // For display
                        }


                        switch (status)
                        {
                            case "New":
                                pillClass += " New";
                                break;
                            case "Approved":
                                pillClass += " Approved";
                                break;
                            case "Rejected":
                                pillClass += " Rejected";
                                break;
                            case "Approved by Tier-1":
                                pillClass += " ApprovedByHOD";
                                break;
                            case "Approved by Tier-2":
                                pillClass += " ApprovedByStores";
                                break;
                            case "Approved by Tier-3":
                                pillClass += " ApprovedByGM";
                                break;
                            case "Approved by Tier-4":
                                pillClass += " ApprovedByStores";
                                break;
                            case "Rejected by Tier-1":
                                pillClass += " Rejected";
                                break;
                            case "Rejected by Tier-2":
                                pillClass += " Rejected";
                                break;
                            case "Rejected by Tier-3":
                                pillClass += " Rejected";
                                break;
                            case "Rejected by Tier-4":
                                pillClass += " Rejected";
                                break;
                        }

                        // Apply the pill styling
                        e.Row.Cells[6].Text = $"<span class='{pillClass}'>{displayStatus}</span>";
                    }
                }
            }
        }

        protected void gvRequests_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvRequests.PageIndex = e.NewPageIndex;

            // Get stored sort values
            string sortExpression = ViewState["SortExpression"] as string ?? "";
            string sortDirection = ViewState["SortDirection"] as string ?? "ASC";

            // For pagination, we need to re-execute the query with current search filters
            // Don't rely on ViewState["GridData"] as it might contain stale data
            ViewState["GridData"] = null; // Clear cached data to force fresh query

            // Rebind with current filters and sort
            BindGrid(sortExpression, sortDirection);
        }
        protected void gvRequests_Sorting(object sender, GridViewSortEventArgs e)
        {
            string sortExpression = e.SortExpression;
            string sortDirection = "ASC";

            // Toggle sort direction if same column
            if (ViewState["SortExpression"] != null && ViewState["SortExpression"].ToString() == sortExpression)
            {
                sortDirection = ViewState["SortDirection"].ToString() == "ASC" ? "DESC" : "ASC";
            }

            BindGrid(sortExpression, sortDirection);
        }
        protected void rptStatus_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item ||
                e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;

                SetApprovalStatus(
                    (Label)e.Item.FindControl("lblDeptStatus"),
                    drv["DeptStatus"] == DBNull.Value ? "-" : drv["DeptStatus"].ToString());

                SetApprovalStatus(
                    (Label)e.Item.FindControl("lblITHCGAStatus"),
                    drv["ITHCGAStatus"] == DBNull.Value ? "-" : drv["ITHCGAStatus"].ToString());

                SetApprovalStatus(
                    (Label)e.Item.FindControl("lblDirSalesStatus"),
                    drv["DirSalesStatus"] == DBNull.Value ? "-" : drv["DirSalesStatus"].ToString());

                SetApprovalStatus(
                    (Label)e.Item.FindControl("lblDirectorStatus"),
                    drv["DirectorStatus"] == DBNull.Value ? "-" : drv["DirectorStatus"].ToString());

                SetFlowStatus(
                    (Label)e.Item.FindControl("lblQuote"),
                    drv["QuoteStatus"].ToString());

                SetFlowStatus(
                    (Label)e.Item.FindControl("lblPO"),
                    drv["POStatus"].ToString());

                SetFlowStatus(
                    (Label)e.Item.FindControl("lblGRN"),
                    drv["GRNStatus"].ToString());
            }
        }
        //protected void rptStatus_ItemDataBound(object sender, RepeaterItemEventArgs e)
        //{
        //    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        //    {
        //        DataRowView drv = (DataRowView)e.Item.DataItem;
        //        HiddenField hdStatusID = (HiddenField)e.Item.FindControl("hdStatusID");
        //        string StatusID = hdStatusID.Value;
        //        switch (StatusID)
        //        {
        //            case "1"://New
        //                SetApprovalStatus(
        //           (Label)e.Item.FindControl("lblDeptStatus"), "New");
        //                break;
        //            case "1009"://Approved by Tier-1
        //                SetApprovalStatus(
        //           (Label)e.Item.FindControl("lblDeptStatus"), "Approved by Tier-1");
        //                break;
        //            case "1010"://Rejected by Tier-1
        //                SetApprovalStatus(
        //           (Label)e.Item.FindControl("lblDeptStatus"), "Rejected by Tier-1");
        //                break;
        //            case "1011"://Approved by Tier-2
        //                SetApprovalStatus(
        //            (Label)e.Item.FindControl("lblDeptStatus"), "Approved by Tier-1");
        //                SetApprovalStatus(
        //            (Label)e.Item.FindControl("lblITHCGAStatus"), "Approved by Tier-2");
        //                break;
        //            case "1012"://Rejected by Tier-2
        //                SetApprovalStatus(
        //            (Label)e.Item.FindControl("lblDeptStatus"), "Approved by Tier-1");
        //                SetApprovalStatus(
        //            (Label)e.Item.FindControl("lblITHCGAStatus"), "Rejected by Tier-2");
        //                break;
        //            case "1013"://Approved by Tier-3
        //                SetApprovalStatus(
        //            (Label)e.Item.FindControl("lblDeptStatus"), "Approved by Tier-1");
        //                SetApprovalStatus(
        //            (Label)e.Item.FindControl("lblITHCGAStatus"), "Approved by Tier-2");
        //                SetApprovalStatus(
        //            (Label)e.Item.FindControl("lblDirSalesStatus"), "Approved by Tier-3");
        //                break;
        //            case "1014"://Rejected by Tier-3
        //                SetApprovalStatus(
        //            (Label)e.Item.FindControl("lblDeptStatus"), "Approved by Tier-1");
        //                SetApprovalStatus(
        //            (Label)e.Item.FindControl("lblITHCGAStatus"), "Approved by Tier-2");
        //                SetApprovalStatus(
        //            (Label)e.Item.FindControl("lblDirSalesStatus"), "Rejected by Tier-3");
        //                break;
        //            case "1015"://Approved By Tier-4
        //                SetApprovalStatus(
        //            (Label)e.Item.FindControl("lblDeptStatus"), "Approved by Tier-1");
        //                SetApprovalStatus(
        //            (Label)e.Item.FindControl("lblITHCGAStatus"), "Approved by Tier-2");
        //                SetApprovalStatus(
        //            (Label)e.Item.FindControl("lblDirSalesStatus"), "Approved by Tier-3");
        //                SetApprovalStatus(
        //             (Label)e.Item.FindControl("lblDirectorStatus"), "Approved by Tier-4");
        //                break;
        //            case "1016"://Rejected By Tier-4
        //                SetApprovalStatus(
        //            (Label)e.Item.FindControl("lblDeptStatus"), "Approved by Tier-1");
        //                SetApprovalStatus(
        //            (Label)e.Item.FindControl("lblITHCGAStatus"), "Approved by Tier-2");
        //                SetApprovalStatus(
        //            (Label)e.Item.FindControl("lblDirSalesStatus"), "Approved by Tier-3");
        //                SetApprovalStatus(
        //             (Label)e.Item.FindControl("lblDirectorStatus"), "Rejected by Tier-4");
        //                break;
        //        }



        //        SetFlowStatus(
        //            (Label)e.Item.FindControl("lblQuote"),
        //            drv["QuoteStatus"].ToString()
        //        );

        //        SetFlowStatus(
        //            (Label)e.Item.FindControl("lblPO"),
        //            drv["POStatus"].ToString()
        //        );

        //        SetFlowStatus(
        //            (Label)e.Item.FindControl("lblGRN"),
        //            drv["GRNStatus"].ToString()
        //        );
        //    }
        //}
    }
}