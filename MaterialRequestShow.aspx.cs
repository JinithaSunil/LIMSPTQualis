using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LIMS.Models;
using ClosedXML.Excel;

namespace LIMS
{
    public partial class MaterialRequestShow : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ToString());
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        string date = DateTime.Today.ToString("dd-MMM-yyyy");
        string time = DateTime.Now.ToString("h:mm:ss tt");
        DataAccessLayer dal = new DataAccessLayer();
        public int PageSize = 10;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddDays(-1));
            Response.Cache.SetNoStore();
            try
            {
                if (Session["EmailId"] == null)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Session Timeout'); setTimeout(function() { window.location = 'Login.aspx'; }, 100); </script>");
                }
                else
                {
                    if (!IsPostBack)
                    {
                        string roleId = Session["RoleId"].ToString();
                        string currentPageUrl = HttpContext.Current.Request.Url.AbsolutePath;
                        string pageName = System.IO.Path.GetFileName(currentPageUrl);
                        string urlToCheck = pageName;

                        ViewState["PageIndex"] = 1;
                        ViewState["SearchTerm"] = "";
                        BindPagedData();
                    }
                }
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                var s = new StackTrace(ex);
                var methodName = s.GetFrames().Select(f => f.GetMethod()).First(m => m.Module.Assembly == Assembly.GetExecutingAssembly()).Name;
                string PageName = Path.GetFileName(Request.Url.AbsolutePath);
                bool success = dal.ErrorLogData(methodName, ex.Message, PageName + ".cs", linenum.ToString());
                if (success)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error Log Data Save: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
                }
            }
        }

        private void BindPagedData()
        {
            try
            {
                string roleId = Session["RoleId"]?.ToString() ?? "";
                int EmpID = Convert.ToInt32(Session["UserID"]);
                int pageIndex = Convert.ToInt32(ViewState["PageIndex"]);
                string searchTerm = ViewState["SearchTerm"]?.ToString() ?? "";
                int totalRecords;

                DateTime? startDate = null;
                DateTime? endDate = null;

                if (!string.IsNullOrEmpty(txtStartDate.Text))
                {
                    startDate = DateTime.Parse(txtStartDate.Text);
                }

                if (!string.IsNullOrEmpty(txtEndDate.Text))
                {
                    endDate = DateTime.Parse(txtEndDate.Text).AddDays(1).AddSeconds(-1);
                }

                List<MaterialRequestData> res = dal.MaterialRequestShowHODPaged(
                    pageIndex,
                    PageSize,
                    searchTerm,
                    startDate,
                    endDate,
                    out totalRecords,
                    EmpID,
                    roleId);

                if (res != null && res.Count > 0)
                {
                    RepDataShow.DataSource = res;
                    RepDataShow.DataBind();
                    GeneratePager(totalRecords, pageIndex);
                }
                else
                {
                    RepDataShow.DataSource = null;
                    RepDataShow.DataBind();
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Records Not Found.'); </script>");
                }
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                var s = new StackTrace(ex);
                var methodName = s.GetFrames().Select(f => f.GetMethod()).First(m => m.Module.Assembly == Assembly.GetExecutingAssembly()).Name;
                string PageName = Path.GetFileName(Request.Url.AbsolutePath);
                bool success = dal.ErrorLogData(methodName, ex.Message, PageName + ".cs", linenum.ToString());
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: " + ex.Message.Replace("'", "\\'") + "'); </script>");
            }
        }

        private void GeneratePager(int totalRecords, int currentPage)
        {
            int pageCount = (int)Math.Ceiling((double)totalRecords / PageSize);
            List<object> pages = new List<object>();
            int maxVisiblePages = 5;
            int startPage = Math.Max(1, currentPage - 2);
            int endPage = Math.Min(pageCount, startPage + maxVisiblePages - 1);

            if (endPage - startPage < maxVisiblePages && startPage > 1)
            {
                startPage = Math.Max(1, endPage - maxVisiblePages + 1);
            }

            for (int i = startPage; i <= endPage; i++)
            {
                pages.Add(new { PageIndex = i, PageNumber = i });
            }

            rptPager.DataSource = pages;
            rptPager.DataBind();

            btnFirst.Enabled = currentPage > 1;
            btnPrev.Enabled = currentPage > 1;
            btnNext.Enabled = currentPage < pageCount;
            btnLast.Enabled = currentPage < pageCount;
        }

        protected void rptPager_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Page")
            {
                ViewState["PageIndex"] = Convert.ToInt32(e.CommandArgument);
                BindPagedData();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            ViewState["SearchTerm"] = txtSearch.Text.Trim();
            ViewState["PageIndex"] = 1;
            BindPagedData();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            txtStartDate.Text = "";
            txtEndDate.Text = "";
            ViewState["SearchTerm"] = "";
            ViewState["PageIndex"] = 1;
            BindPagedData();
        }

        protected void btnPrev_Click(object sender, EventArgs e)
        {
            int pageIndex = Convert.ToInt32(ViewState["PageIndex"]);
            if (pageIndex > 1)
            {
                ViewState["PageIndex"] = pageIndex - 1;
                BindPagedData();
            }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            int pageIndex = Convert.ToInt32(ViewState["PageIndex"]);
            ViewState["PageIndex"] = pageIndex + 1;
            BindPagedData();
        }

        protected void btnFirst_Click(object sender, EventArgs e)
        {
            ViewState["PageIndex"] = 1;
            BindPagedData();
        }

        protected void btnLast_Click(object sender, EventArgs e)
        {
            try
            {
                // Get current parameters from ViewState
                string searchTerm = ViewState["SearchTerm"]?.ToString() ?? "";
                string roleId = Session["RoleId"]?.ToString() ?? "";
                int empId = Convert.ToInt32(Session["UserID"] ?? "0");

                // Parse date filters if provided
                DateTime? startDate = null;
                DateTime? endDate = null;

                if (!string.IsNullOrEmpty(txtStartDate.Text))
                {
                    startDate = DateTime.Parse(txtStartDate.Text);
                }

                if (!string.IsNullOrEmpty(txtEndDate.Text))
                {
                    endDate = DateTime.Parse(txtEndDate.Text).AddDays(1).AddSeconds(-1);
                }

                // Get total records count (without loading actual data)
                dal.MaterialRequestShowHODPaged(
                    pageIndex: 1,
                    pageSize: 1,  // Minimum page size to reduce query overhead
                    searchTerm: searchTerm,
                    startDate: startDate,
                    endDate: endDate,
                    totalRecords: out int totalRecords,
                    employeeId: empId,
                    roleId: roleId);

                // Calculate last page index
                int lastPageIndex = totalRecords > 0 ? (int)Math.Ceiling((double)totalRecords / PageSize) : 1;

                // Set the last page and rebind
                ViewState["PageIndex"] = lastPageIndex;
                BindPagedData();
            }
            catch (Exception ex)
            {
                // Log error
                Debug.WriteLine($"btnLast_Click error: {ex.Message}");

                // Show user-friendly message
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                    $"<script>alert('Error navigating to last page: {ex.Message.Replace("'", "\\'")}');</script>");
            }
        }


        protected void btnDownloadExcelFile_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime? startDate = null;
                DateTime? endDate = null;

                if (!string.IsNullOrEmpty(txtStartDate.Text))
                {
                    startDate = DateTime.Parse(txtStartDate.Text);
                }

                if (!string.IsNullOrEmpty(txtEndDate.Text))
                {
                    endDate = DateTime.Parse(txtEndDate.Text).AddDays(1).AddSeconds(-1);
                }

                string searchTerm = ViewState["SearchTerm"]?.ToString() ?? "";
                var data = GetExportData(searchTerm, startDate, endDate);

                if (data == null || data.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "noData", "alert('No data found to export.');", true);
                    return;
                }

                ExportToExcel(data);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
            }
        }

        private List<MaterialRequestData> GetExportData(string searchTerm, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                int totalRecords;
                string roleId = Session["RoleId"]?.ToString() ?? "";
                int empId = Convert.ToInt32(Session["UserID"] ?? "0");

                Debug.WriteLine($"Export Params - Search: '{searchTerm}', Start: {startDate}, End: {endDate}, Role: {roleId}, EmpID: {empId}");

                var result = dal.MaterialRequestShowHODPaged(
                    1,
                    int.MaxValue,
                    searchTerm,
                    startDate,
                    endDate,
                    out totalRecords,
                    empId,
                    roleId);

                Debug.WriteLine($"Found {totalRecords} total records, returning {result?.Count ?? 0} items");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetExportData error: {ex.Message}");
                throw;
            }
        }

        private void ExportToExcel(List<MaterialRequestData> data)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Material Requests");
                // Headers
                ws.Cell(1, 1).Value = "MR Number";
                ws.Cell(1, 2).Value = "Project";
                ws.Cell(1, 3).Value = "Unit";
                ws.Cell(1, 4).Value = "Requested By";
                ws.Cell(1, 5).Value = "Status";
                ws.Cell(1, 6).Value = "Created Date";

                // Data
                int row = 2;
                foreach (var item in data)
                {
                    ws.Cell(row, 1).Value = item.MRM_NUMBER;
                    ws.Cell(row, 2).Value = item.MRM_ProjectCode;
                    ws.Cell(row, 3).Value = item.CompanyUnitName;
                    ws.Cell(row, 4).Value = $"{item.FirstName} {item.LastName}";
                    ws.Cell(row, 5).Value = item.StatusTitle;
                    ws.Cell(row, 6).Value = item.MRM_CreatedDateTime;
                    row++;
                }

                // Formatting
                ws.Columns().AdjustToContents();
                var headerRange = ws.Range(1, 1, 1, 6);
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Font.Bold = true;

                // Export
                using (MemoryStream ms = new MemoryStream())
                {
                    wb.SaveAs(ms);
                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", $"attachment;filename=MaterialRequests_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
                    Response.BinaryWrite(ms.ToArray());
                    Response.End();
                }
            }
        }

        protected void RepDataShow_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ItemEdit")
            {
                string[] arguments = e.CommandArgument.ToString().Split(';');
                string MRM_ID = arguments[0];
                string MRM_StatusId = arguments[1];
                string encodedEventID = Convert.ToBase64String(Encoding.UTF8.GetBytes(MRM_ID));
                int statusID = Convert.ToInt32(MRM_StatusId);
                if (statusID == 20 || statusID == 22 || statusID == 35)
                {
                    string url = $"MaterialRequestFormEdit.aspx?PageID={encodedEventID}";
                    Response.Redirect(url);
                }
                else if (statusID == 21)
                {
                    string ApprovalURL = $"MaterialRequestApprovalShow.aspx?PageID={encodedEventID}";
                    Response.Redirect(ApprovalURL);
                }
            }

            if (e.CommandName == "MTshow")
            {
                string[] arguments = e.CommandArgument.ToString().Split(';');
                string MRM_ID = arguments[0];
                string MRM_NUMBER = arguments[1];
                string MRM_StatusId = arguments[2];

                string encodedEventID = Convert.ToBase64String(Encoding.UTF8.GetBytes(MRM_ID));
                string encodedEventID1 = Convert.ToBase64String(Encoding.UTF8.GetBytes(MRM_NUMBER));

                int statusID = Convert.ToInt32(MRM_StatusId);
                if (statusID == 3 || statusID == 4)
                {
                    string ApprovalURL = $"MaterialTransactionInUnit.aspx?PageID={encodedEventID}&PageData={encodedEventID1}";
                    Response.Redirect(ApprovalURL);                    
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No transactions found'); setTimeout(function() { window.location = 'MaterialRequestShow.aspx'; }, 100); </script>");
                }
            }

        }

        protected System.Drawing.Color GetStatusColor(object statusTitle)
        {
            string status = statusTitle.ToString();

            switch (status)
            {
                case "MR Generated":
                    return System.Drawing.Color.Blue;
                case "MR Approved":
                    return System.Drawing.Color.Green;
                case "Item Send":
                    return System.Drawing.Color.DarkGreen;
                default:
                    return System.Drawing.Color.Red;
            }
        }


    }
}