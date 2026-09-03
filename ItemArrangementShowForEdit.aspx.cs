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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class ItemArrangementShowForEdit : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ToString());
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        DataAccessLayer dal = new DataAccessLayer();
        public int PageSize = 10;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddDays(-1));
            Response.Cache.SetNoStore();

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

        private void BindPagedData()
        {
            try
            {
                string roleId = Session["RoleId"]?.ToString() ?? "";
                int companyUnitId = Convert.ToInt32(Session["CompanyUnitId"]);
                int pageIndex = Convert.ToInt32(ViewState["PageIndex"]);
                string searchTerm = ViewState["SearchTerm"]?.ToString() ?? "";
                int totalRecords;

                List<ItemTransactionData> res = dal.ItemArrangementShowPaged(
                    pageIndex,
                    PageSize,
                    searchTerm,
                    out totalRecords,
                    companyUnitId,
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
                    if (pageIndex > 1)
                    {
                        ViewState["PageIndex"] = 1;
                        BindPagedData();
                    }
                    else
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Records Not Found.'); </script>");
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
            string roleId = Session["RoleId"]?.ToString() ?? "";
            int companyUnitId = Convert.ToInt32(Session["CompanyUnitId"]);
            string searchTerm = ViewState["SearchTerm"]?.ToString() ?? "";

            dal.ItemArrangementShowPaged(1, PageSize, searchTerm, out int totalRecords, companyUnitId, roleId);

            int lastPageIndex = (int)Math.Ceiling((double)totalRecords / PageSize);
            ViewState["PageIndex"] = lastPageIndex;
            BindPagedData();
        }

        protected void btnDownloadExcelFile_Click(object sender, EventArgs e)
        {
            try
            {
                string searchTerm = ViewState["SearchTerm"]?.ToString() ?? "";
                var data = GetExportData(searchTerm);

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

        private List<ItemTransactionData> GetExportData(string searchTerm)
        {
            try
            {
                int totalRecords;
                string roleId = Session["RoleId"]?.ToString() ?? "";
                int companyUnitId = Convert.ToInt32(Session["CompanyUnitId"]);

                var result = dal.ItemArrangementShowPaged(
                    1,
                    int.MaxValue,
                    searchTerm,
                    out totalRecords,
                    companyUnitId,
                    roleId);

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetExportData error: {ex.Message}");
                throw;
            }
        }

        private void ExportToExcel(List<ItemTransactionData> data)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Item Arrangement");
                // Headers
                ws.Cell(1, 1).Value = "Item Code";
                ws.Cell(1, 2).Value = "Item Name";
                ws.Cell(1, 3).Value = "Rack";
                ws.Cell(1, 4).Value = "Rack Series";
                ws.Cell(1, 5).Value = "Item Stored";
                ws.Cell(1, 6).Value = "Current Stock";

                // Data
                int row = 2;
                foreach (var item in data)
                {
                    ws.Cell(row, 1).Value = item.ItemCode;
                    ws.Cell(row, 2).Value = item.ItemName;
                    ws.Cell(row, 3).Value = item.RackType;
                    ws.Cell(row, 4).Value = item.RackSerialNumber;
                    ws.Cell(row, 5).Value = item.TotalItemInRack;
                    ws.Cell(row, 6).Value = item.InventoryCurrentStock;
                    row++;
                }
                // Formatting
                ws.Columns().AdjustToContents();
                var headerRange = ws.Range(1, 1, 1, 7);
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Font.Bold = true;

                // Export
                using (MemoryStream ms = new MemoryStream())
                {
                    wb.SaveAs(ms);
                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", $"attachment;filename=ItemArrangement_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
                    Response.BinaryWrite(ms.ToArray());
                    Response.End();
                }
            }
        }



    }
}