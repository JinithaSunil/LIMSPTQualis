using ClosedXML.Excel;
using LIMS.Models;
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

namespace LIMS
{
    public partial class MaterialReleasedShowEmployee : System.Web.UI.Page
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

                List<MaterialRequestData> res = dal.MaterialReleasedShowEmployeePaged(pageIndex, PageSize, searchTerm, startDate, endDate, out totalRecords, EmpID, roleId);

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

        protected void rptPager_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Page")
            {
                ViewState["PageIndex"] = Convert.ToInt32(e.CommandArgument);
                BindPagedData();
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
                    ScriptManager.RegisterStartupScript(this, GetType(), "noData",
                        "alert('No data found to export.');", true);
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
                string roleId = Session["RoleId"]?.ToString() ?? "NULL";
                int empId = Convert.ToInt32(Session["UserID"] ?? "0");

                Debug.WriteLine($"Export Params - Search: '{searchTerm}', Start: {startDate}, End: {endDate}, Role: {roleId}, EmpID: {empId}");

                var result = dal.MaterialReleasedShowEmployeePaged(1, int.MaxValue, searchTerm, startDate, endDate, out totalRecords, empId, roleId);

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
                ws.Cell(1, 2).Value = "Item Code";
                ws.Cell(1, 3).Value = "Requested By";
                ws.Cell(1, 4).Value = "Required Qty";
                ws.Cell(1, 5).Value = "Received Qty";
                ws.Cell(1, 6).Value = "Balance Qty";
                ws.Cell(1, 7).Value = "Unit";
                ws.Cell(1, 8).Value = "Project";
                ws.Cell(1, 9).Value = "Status";
                ws.Cell(1, 10).Value = "Created Date";

                // Data
                int row = 2;
                foreach (var item in data)
                {
                    ws.Cell(row, 1).Value = item.MRM_NUMBER;
                    ws.Cell(row, 2).Value = item.MT_ItemCode;
                    ws.Cell(row, 3).Value = $"{item.FirstName} {item.LastName}";
                    ws.Cell(row, 4).Value = item.MT_RequiredQty;
                    ws.Cell(row, 5).Value = item.MT_ReceivedQty;
                    ws.Cell(row, 6).Value = item.MT_BalanceQty;
                    ws.Cell(row, 7).Value = item.CompanyUnitName;
                    ws.Cell(row, 8).Value = item.MRM_ProjectCode;
                    ws.Cell(row, 9).Value = GetStatusPlainText(item.MT_BalanceQty, item.MT_ReceivedQty); // Changed here
                    ws.Cell(row, 10).Value = item.MRM_CreatedDateTime;
                    row++;
                }

                // Formatting
                ws.Columns().AdjustToContents();
                var headerRange = ws.Range(1, 1, 1, 10);
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

        // For HTML display (with styling)
        public string GetStatusWithStyle(object balanceQtyObj, object receivedQtyObj)
        {
            string status = GetStatusPlainText(balanceQtyObj, receivedQtyObj);

            switch (status)
            {
                case "Completed":
                    return "<span style='color:green;'>Completed</span>";
                case "Pending":
                    return "<span style='color:red;'>Pending</span>";
                case "Partially Released":
                    return "<span style='color:blue;'>Partially Released</span>";
                default:
                    return "<span style='color:gray;'>UNKNOWN</span>";
            }
        }

        // For Excel export (plain text)
        public string GetStatusPlainText(object balanceQtyObj, object receivedQtyObj)
        {
            decimal balanceQty = 0;
            decimal receivedQty = 0;

            if (balanceQtyObj != null && receivedQtyObj != null &&
                balanceQtyObj != DBNull.Value && receivedQtyObj != DBNull.Value)
            {
                balanceQty = SafeConvertToDecimal(balanceQtyObj);
                receivedQty = SafeConvertToDecimal(receivedQtyObj);

                if (balanceQty == 0)
                {
                    return "Completed";
                }
                else if (receivedQty == 0)
                {
                    return "Pending";
                }
                else if (receivedQty > 0 && balanceQty > 0)
                {
                    return "Partially Released";
                }
            }

            return "UNKNOWN";
        }

        public string GetStatus(object balanceQtyObj, object receivedQtyObj)
        {
            decimal balanceQty = 0;
            decimal receivedQty = 0;

            // Log the raw data types and values before processing
            Debug.WriteLine("balanceQtyObj Type: " + balanceQtyObj.GetType().ToString());
            Debug.WriteLine("receivedQtyObj Type: " + receivedQtyObj.GetType().ToString());

            // Check if the values are valid and not DBNull
            if (balanceQtyObj != null && receivedQtyObj != null && balanceQtyObj != DBNull.Value && receivedQtyObj != DBNull.Value)
            {
                // Check and log the actual raw values to see what's being passed in
                Debug.WriteLine("Raw balanceQtyObj: " + balanceQtyObj.ToString());
                Debug.WriteLine("Raw receivedQtyObj: " + receivedQtyObj.ToString());

                // Attempt to convert values using SafeConvertToDecimal() with logging
                balanceQty = SafeConvertToDecimal(balanceQtyObj);
                receivedQty = SafeConvertToDecimal(receivedQtyObj);

                // Log the results of conversion to verify
                Debug.WriteLine($"Converted balanceQty: {balanceQty}, Converted receivedQty: {receivedQty}");

                // Now apply conditions based on the parsed values
                if (balanceQty == 0)
                {
                    return "<span style='color:green;'>Completed</span>";
                }
                else if (receivedQty == 0)
                {
                    return "<span style='color:red;'>Pending</span>";
                }
                else if (receivedQty > 0 && balanceQty > 0)
                {
                    return "<span style='color:blue;'>Partially Released</span>";
                }
            }
            else
            {
                // Log if one of the values is null or DBNull
                Debug.WriteLine("One or both values are null or DBNull.");
            }

            // Default return value for any issues encountered
            return "<span style='color:gray;'>UNKNOWN</span>";
        }

        protected void RepDataShow_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "MTshow")
            {
                string[] arguments = e.CommandArgument.ToString().Split(';');
                string MRM_ID = arguments[0];
                string MRM_NUMBER = arguments[1];
                string MRM_StatusId = arguments[2];
                string MT_ItemCode = arguments[3];

                string encodedEventID = Convert.ToBase64String(Encoding.UTF8.GetBytes(MRM_ID));
                string encodedEventID1 = Convert.ToBase64String(Encoding.UTF8.GetBytes(MRM_NUMBER));
                string encodedEventID2 = Convert.ToBase64String(Encoding.UTF8.GetBytes(MT_ItemCode));

                int statusID = Convert.ToInt32(MRM_StatusId);
                if (statusID == 20 || statusID == 35)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No transactions found'); setTimeout(function() { window.location = 'MaterialReleasedShowEmployee.aspx'; }, 100); </script>");
                }
                else if (statusID == 21)
                {
                    string ApprovalURL = $"MaterialReleasedListItemWise.aspx?PageID={encodedEventID}&PageData={encodedEventID1}&PageCode={encodedEventID2}";
                    Response.Redirect(ApprovalURL);
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Something went wrong!');</script>");
                }
            }

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                // Get the status from the data item
                int statusId = Convert.ToInt32(DataBinder.Eval(e.Item.DataItem, "MT_StatusId"));

                // Find the PlaceHolder
                PlaceHolder phDropdown = (PlaceHolder)e.Item.FindControl("phDropdown");

                // Check if the control was found
                if (phDropdown != null)
                {
                    // Show dropdown ONLY if status is 21, 20, or 35
                    phDropdown.Visible = (statusId == 21 || statusId == 20 || statusId == 35);
                }
                else
                {
                    // Optional: Log or handle the case where control isn't found
                    // You might want to check your control ID or template structure
                }
            }

        }

        protected System.Drawing.Color GetStatusColor(object statusTitle)
        {
            string status = statusTitle.ToString();

            switch (status)
            {
                case "Item Sent Partially":
                    return System.Drawing.Color.Blue;
                case "Item Fully Sent":
                    return System.Drawing.Color.Green;
                default:
                    return System.Drawing.Color.Red;
            }
        }

        private decimal SafeConvertToDecimal(object value)
        {
            try
            {
                // Ensure that the value is not null or empty before attempting to convert
                if (value != null && value != DBNull.Value && !string.IsNullOrWhiteSpace(value.ToString()))
                {
                    // Attempt to convert using Convert.ToDecimal
                    decimal result = Convert.ToDecimal(value);
                    return result;
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
            }

            // Return 0 if conversion fails or if value is invalid
            return 0;
        }

        protected void btnLast_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime? startDate = null;
                DateTime? endDate = null;

                if (!string.IsNullOrEmpty(txtStartDate.Text))
                {
                    if (DateTime.TryParse(txtStartDate.Text, out DateTime parsedStartDate))
                    {
                        startDate = parsedStartDate;
                    }
                }

                if (!string.IsNullOrEmpty(txtEndDate.Text))
                {
                    if (DateTime.TryParse(txtEndDate.Text, out DateTime parsedEndDate))
                    {
                        endDate = parsedEndDate.AddDays(1).AddSeconds(-1);
                    }
                }

                string searchTerm = ViewState["SearchTerm"]?.ToString() ?? "";
                string roleId = Session["RoleId"]?.ToString() ?? "";
                int empId = Convert.ToInt32(Session["UserID"]);

                // Get total records count
                dal.MaterialReleasedShowEmployeePaged(
                    1,
                    PageSize,
                    searchTerm,
                    startDate,
                    endDate,
                    out int totalRecords,
                    empId,
                    roleId);

                // Calculate last page index correctly
                int lastPageIndex = (int)Math.Ceiling((double)totalRecords / PageSize);
                ViewState["PageIndex"] = lastPageIndex;
                BindPagedData();
            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                    "<script>alert('Error: " + ex.Message.Replace("'", "\\'") + "');</script>");
            }
        }




    }
}