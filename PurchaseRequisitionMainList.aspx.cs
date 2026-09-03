using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Data;
using System.Text;

namespace LIMS
{
    public partial class PurchaseRequisitionMainList : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
                BindPendingPR(); BindCompletelistPRFinal();
            }
        }
        //Bind
        private void BindGrid()
        {
            string selectedPRType = ddlPRType.SelectedItem.Text;
            string searchText = TextBox1.Text.Trim();

            string query = @"
                            SELECT 
                            pd.PurchaseRequisitionID AS PR,
                            pr.PRID AS PRID,
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
                            WHERE pd.StatusID = 9";

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
                        gvRequests.DataSource = dt;
                        gvRequests.DataBind();
                    }
                }
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
        private void BindCompletelistPRFinal()
        {
            System.Data.DataTable dt = _cm.GetCompletelistPRFinal();
            if (dt.Rows.Count > 0)
            {
                ddlApproved.DataSource = dt;
                ddlApproved.DataTextField = "Status";
                ddlApproved.DataValueField = "StatusID";
                ddlApproved.DataBind();
                ddlApproved.Items.Insert(0, new ListItem("--Select Status--"));
            }
        }
        private void BindCompletedGrid()
        {
            string selectedStatus = ddlApproved.SelectedItem.Text;

            string query = @"
        SELECT 
            pr.PRID AS PR,
            e.EmployeeName,
            i.ItemDescription AS Description,
            d.EmployeeDepartmentName,
            pr.RequestDate,
            ps.Status,
            pt.PRType,
            pr.StoreType
        FROM PurchaseRequisition pr
        LEFT JOIN PurchaseRequisitionDetails pd ON pr.PRID = pd.PRID
        LEFT JOIN Items i ON i.itemcode = pd.ItemCode
        LEFT JOIN Employees e ON e.ID = pr.RequestBy
        LEFT JOIN EmployeeDepartment d ON d.EmployeeDepartmentID = pr.DepartmentID
        LEFT JOIN PurchaseRequisitionStatus ps ON ps.StatusID = pd.StatusID
        LEFT JOIN PRTypes pt ON pt.PRTypesID = pr.PRTypeID
        WHERE 1 = 1"; // Always true, so other ANDs can follow

            // Add status filter
            if (selectedStatus != "Select Approved By")
            {
                query += " AND ps.Status = @Status";
            }

            query += " ORDER BY pr.PRID DESC";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (selectedStatus != "Select Approved By")
                    {
                        cmd.Parameters.AddWithValue("@Status", selectedStatus);
                    }

                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        gvRequests2.DataSource = dt;
                        gvRequests2.DataBind();
                    }
                }
            }
        }


        //ddl's
        protected void ddlPR_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGrid();
        }
        protected void ddlApproved_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvRequests.Visible = false;
            BindCompletedGrid();
        }
        protected void imgSearch_Click(object sender, ImageClickEventArgs e)
        {
            BindGrid();
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
            DropDownList ddlStatus = (DropDownList)row.FindControl("ddlStatus");

            HyperLink lnkPRID = (HyperLink)row.FindControl("PRID");
            string prID = lnkPRID.Text;

            string newStatusID = ddlStatus.SelectedValue;
            string StatusName = ddlStatus.SelectedItem.ToString();
            string PurchaseRequisitionID = row.Cells[1].Text;

            if (string.IsNullOrEmpty(newStatusID))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(),
                    "alertNoStatus",
                    "alert('Please select a status.');",
                    true);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    // Update status
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
                    var User = new DAL.User();
                    User.EmployeeID = Session["EmployeeID"]?.ToString();
                    User.DepartmentID = Session["DepartmentID"]?.ToString();
                    User.UserID = Session["UserID"]?.ToString();
                    User.RoleID = Session["RoleID"]?.ToString();
                    User.Remarks = "Purchase - " + StatusName;
                    User.PRStatus = newStatusID.ToString();
                    User.PurchaseRequisitionDetailsID = PurchaseRequisitionID.ToString();
                    _cm.AddPRHistory(User);
                    // Generate and save report
                    GenerateAndSaveReport(prID, conn);
                }

                // Construct the report URL
                string reportUrl = ResolveUrl($"~/GeneratePurchaseRequisitionReport.aspx?ID={prID}");

                string script = @"
            alert('Status updated successfully and report saved!'); 
            window.location.href = '" + reportUrl + @"';";

                ScriptManager.RegisterStartupScript(this, this.GetType(),
                    "SaveSuccessAndRedirect",
                    script,
                    true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(),
                    "alertError",
                    $"alert('Error: {ex.Message}. Please try again.');",
                    true);
            }
        }
        private void GenerateAndSaveReport(string prId, SqlConnection conn)
        {
            StringBuilder html = new StringBuilder();

            // Get PR Header Info
            string reportFileName = string.Empty;  // Declare the reportFileName variable here

            using (SqlCommand cmd = new SqlCommand(@"
    SELECT PR.* , e.EmployeeName as RequestedBy, ed.EmployeeDepartmentName AS Department,
           ps.Status, PR.ProjectName, PR.ReportFileName, PR.RequestDate
    FROM PurchaseRequisition PR
    LEFT JOIN PurchaseRequisitionDetails pd ON PR.PRID = pd.PRID
    LEFT JOIN Employees e ON PR.RequestBy = e.EmployeeID 
    LEFT JOIN EmployeeDepartment ed ON PR.DepartmentID = ed.EmployeeDepartmentID 
    LEFT JOIN PurchaseRequisitionStatus ps ON pd.StatusID = ps.StatusID
    LEFT JOIN ProjectMaster pm ON pr.ProjectName = pm.ProjectName
    WHERE PR.PRID = @PRID", conn))
            {
                cmd.Parameters.AddWithValue("@PRID", prId);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Capture ReportFileName to use in the filename
                        reportFileName = reader["ReportFileName"].ToString();

                        html.Append("<div class='pr-header'>");
                        html.Append("<div class='header-left'>");
                        html.Append("<h2>Purchase Requisition</h2>");
                        html.Append($"<p>PR#: {reportFileName}</p>");  // Use reportFileName here
                        html.Append($"<p>PR Date: {Convert.ToDateTime(reader["RequestDate"]).ToString("dd/MMM/yyyy")}</p>");
                        html.Append($"<p>PR Status: {reader["Status"]}</p>");
                        html.Append($"<p>Project: {reader["ProjectName"] ?? "N/A"}</p>");
                        html.Append("</div>");
                        html.Append("<div class='header-right'>");
                        html.Append("<img src='assets/img/logo.png' alt='Company Logo' />");
                        html.Append("</div>");
                        html.Append("</div>");

                        html.Append("<div class='request-details'>");
                        html.Append("<h3>Request Details:</h3>");
                        html.Append($"<p>Requested By: {reader["RequestedBy"]}</p>");
                        html.Append($"<p>Department: {reader["Department"]}</p>");
                        html.Append("</div>");
                    }
                }
            }

            // Get PR Details
            html.Append("<h3>PR Summary</h3>");
            html.Append("<table class='pr-table'>");
            html.Append("<thead><tr>");
            html.Append("<th>Item Code</th>");
            html.Append("<th>Description</th>");
            html.Append("<th>UOM</th>");
            html.Append("<th>Quantity</th>");
            html.Append("<th>Request On</th>");
            html.Append("<th>Required On</th>");
            html.Append("<th>Purpose</th>");
            html.Append("<th>Status</th>");
            html.Append("</tr></thead>");
            html.Append("<tbody>");

            using (SqlCommand cmd = new SqlCommand(@"
    SELECT 
        pd.ItemCode AS [Item Code],
        i.ItemDescription AS [Description],
        u.UOMName AS UOM,
        pd.Quantity,
        pr.RequestDate AS [Requested On],
        pd.RequiredOn AS [Required On],
        pd.Purpose,
        ps.Status
    FROM purchaserequisition pr
    LEFT JOIN PurchaseRequisitionDetails pd ON pr.PRID = pd.PRID
    LEFT JOIN Items i ON pd.ItemCode = i.ItemCode
    LEFT JOIN UOM u ON pd.UOMID = u.UOMId
    LEFT JOIN PurchaseRequisitionStatus ps ON pd.StatusID = ps.StatusID
    WHERE pr.PRID = @PRID", conn))
            {
                cmd.Parameters.AddWithValue("@PRID", prId);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        html.Append("<tr>");
                        html.Append($"<td>{reader["Item Code"]}</td>");
                        html.Append($"<td>{reader["Description"]}</td>");
                        html.Append($"<td>{reader["UOM"]}</td>");
                        html.Append($"<td>{reader["Quantity"]}</td>");
                        html.Append($"<td>{Convert.ToDateTime(reader["Requested On"]).ToString("dd/MMM/yyyy")}</td>");
                        html.Append($"<td>{Convert.ToDateTime(reader["Required On"]).ToString("dd/MMM/yyyy")}</td>");
                        html.Append($"<td>{reader["Purpose"]}</td>");
                        html.Append($"<td>{reader["Status"]}</td>");
                        html.Append("</tr>");
                    }
                }
            }
            html.Append("</tbody></table>");

            // Save the report
            try
            {
                string directoryPath = Path.Combine(Server.MapPath("~/"), "PurchaseRequisition");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Save the file using the ReportFileName (or PR ID if ReportFileName is empty)
                string filename = string.IsNullOrEmpty(reportFileName) ? $"PR-{prId}.html" : $"{reportFileName}.html";
                string filePath = Path.Combine(directoryPath, filename);

                string completeHtml = $@"
        <!DOCTYPE html>
        <html>
        <head>
            <title>Purchase Requisition Report - {prId}</title>
            <meta charset='utf-8'>
            <style>
                .pr-header {{ display: flex; justify-content: space-between; margin-bottom: 20px; }}
                .header-left {{ flex: 1; }}
                .header-right {{ flex: 0 0 200px; }}
                .request-details {{ margin-bottom: 20px; }}
                .pr-table {{ width: 100%; border-collapse: collapse; margin-top: 20px; }}
                .pr-table th, .pr-table td {{ 
                    border: 1px solid #ddd; 
                    padding: 8px; 
                    text-align: left; 
                }}
                .pr-table th {{ 
                    background-color: #B94D8F; 
                    color: white; 
                }}
                .pr-table tr:nth-child(even) {{ background-color: #f2f2f2; }}
            </style>
        </head>
        <body>
            {html}
        </body>
        </html>";

                File.WriteAllText(filePath, completeHtml);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving report: {ex.Message}", ex);
            }
        }


        //Grid
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

                // Rebind the grid with the same data
                BindGrid();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    $"alert('Paging Error: {ex.Message}');", true);
            }
        }
        protected void gvRequests2_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                // Set the new page index
                gvRequests2.PageIndex = e.NewPageIndex;

                // Rebind the grid with the same data
                BindCompletedGrid();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    $"alert('Paging Error: {ex.Message}');", true);
            }
        }
    }
}