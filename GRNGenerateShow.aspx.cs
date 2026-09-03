using ClosedXML.Excel;
using LIMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class GRNGenerateShow : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ToString());
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        string date = DateTime.Today.ToString("dd-MMM-yyyy");
        string time = DateTime.Now.ToString("h:mm:ss tt");
        DataAccessLayer dal = new DataAccessLayer();
        DAL.CM _cm = new DAL.CM();

        public int PageSize = 10;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddDays(-1));
            Response.Cache.SetNoStore();

            if (Session["EmailId"] == null)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                    "<script> alert('Session Timeout'); setTimeout(function() { window.location = 'Login.aspx'; }, 100); </script>");
                return;
            }

            if (!IsPostBack)
            {
                string roleId = Session["RoleId"].ToString();
                string pageName = Path.GetFileName(HttpContext.Current.Request.Url.AbsolutePath);

                ViewState["PageIndex"] = 1;
                ViewState["SearchTerm"] = "";
                BindPagedData();
            }
        }

        private void BindPagedData()
        {
            try
            {
                int pageIndex = Convert.ToInt32(ViewState["PageIndex"]);
                string searchTerm = ViewState["SearchTerm"]?.ToString() ?? "";
                int totalRecords;

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

                List<GRNData> filtered = dal.GRNMasterDataGetPaged(pageIndex, PageSize, searchTerm, startDate, endDate, out totalRecords);

                if (filtered != null && filtered.Count > 0)
                {
                    RepDataShow.DataSource = filtered;
                    RepDataShow.DataBind();

                    GeneratePager(totalRecords, pageIndex);
                }
                else
                {
                    RepDataShow.DataSource = null;
                    RepDataShow.DataBind();
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                        "<script> alert('No records found.'); </script>");
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
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

            dal.GRNMasterDataGetPaged(1, PageSize, ViewState["SearchTerm"]?.ToString() ?? "", startDate, endDate, out int totalRecords);
            int lastPageIndex = (int)Math.Ceiling((double)totalRecords / PageSize);
            ViewState["PageIndex"] = lastPageIndex;
            BindPagedData();
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

        protected System.Drawing.Color GetStatusColor(object statusTitle)
        {
            string status = statusTitle.ToString();

            switch (status)
            {
                case "GRN Generated":
                    return System.Drawing.Color.Blue;
                case "GRN Approved":
                    return System.Drawing.Color.Green;
                case "Item Stored":
                    return System.Drawing.Color.Orange;
                case "GRN Rejected":
                    return System.Drawing.Color.Red;
                default:
                    return System.Drawing.Color.OrangeRed;
            }
        }

        protected void RepDataShow_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ItemShow")
            {
                string[] arguments = e.CommandArgument.ToString().Split(';');
                string GRN_ID = arguments[0];
                string GRN_NUMBER = arguments[1];
                string GRN_PoNumber = arguments[2];

                string encodedEventID = _cm.Encode(GRN_ID);

                string url = $"GenerateGRN.aspx?ID={encodedEventID}";
                Response.Write("<script> window.open( '" + url + "','_blank' ); </script>");
                // Response.Redirect(url);
            }
            else if (e.CommandName == "ItemEdit")
            {
                string[] arguments = e.CommandArgument.ToString().Split(';');
                string GRN_ID = arguments[0];
                string GRN_NUMBER = arguments[1];
                string GRN_PoNumber = arguments[2];

                string encodedEventID = Convert.ToBase64String(Encoding.UTF8.GetBytes(GRN_ID));
                string encodedEventID1 = Convert.ToBase64String(Encoding.UTF8.GetBytes(GRN_NUMBER));
                string encodedEventID2 = Convert.ToBase64String(Encoding.UTF8.GetBytes(GRN_PoNumber));

                string url = $"GRNGeneratedEdit.aspx?PageID={encodedEventID}&PageData={encodedEventID1}&PageData1={encodedEventID2}";
                Response.Redirect(url);
            }
            else
            {

            }


        }


        protected void btnDownloadExcelFile_Click(object sender, EventArgs e)
        {
            try
            {
                // Get the date range from your controls
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
                ScriptManager.RegisterStartupScript(this, GetType(), "exportError",
                    $"alert('Error exporting to Excel: {ex.Message.Replace("'", "\\'")}');", true);
            }
        }

        private List<GRNMasterData> GetExportData(string searchTerm, DateTime? startDate, DateTime? endDate)
        {
            List<GRNMasterData> items = new List<GRNMasterData>();

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"sELECT tm.InwardID,GRNNumber,PONumber,EmployeeName as RequestedBy, tm.ReceivedDate,POStatus as GRNStatus, VendorName
                                    FROM TransactionMaster tm
                                    inner join Transactiondetails td on td.InwardID = tm.InwardID
                                    inner join PurchaseOrderDetails pod on pod.PODetailsID = td.PODetailsID
                                    inner join PurchaseOrder po on po.PurchaseOrderID = pod.PurchaseOrderID
                                    inner join VendorDetails v on v.VendorID=po.VendorID
                                    INNER JOIN PurchaseRequisitiondetails pd on pd.PurchaseOrderID=po.PurchaseOrderID
                                    inner join PurchaseRequisition pr on pr.PRID=pd.PRID	
                                    inner join Employees e on e.ID = pr.RequestBy
                                    inner join POStatus ps on ps.POStatusID=tm.GRNStatus
                                  WHERE (@SearchTerm IS NULL OR 
                                      (tm.GRNNUMBER LIKE '%' + @SearchTerm + '%' OR 
                                       po.PoNumber LIKE '%' + @SearchTerm + '%' OR
                                       EmployeeName LIKE '%' + @SearchTerm + '%'))
                                  AND (@StartDate IS NULL OR CONVERT(date, tm.ReceivedDate) >= @StartDate)
                                  AND (@EndDate IS NULL OR CONVERT(date, tm.ReceivedDate) <= @EndDate)
                                  group by tm.InwardID,GRNNumber,PONumber,EmployeeName, tm.ReceivedDate,POStatus, VendorName
                                  ORDER BY CONVERT(datetime, tm.ReceivedDate, 120) DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    string masterCompanyId = HttpContext.Current.Session["MasterCompanyId"].ToString();

                    cmd.Parameters.AddWithValue("@MasterCompanyId", masterCompanyId);
                    cmd.Parameters.AddWithValue("@SearchTerm", string.IsNullOrEmpty(searchTerm) ? (object)DBNull.Value : searchTerm);
                    cmd.Parameters.AddWithValue("@StartDate", startDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EndDate", endDate ?? (object)DBNull.Value);

                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new GRNMasterData
                            {
                                GRN_ID = Convert.ToInt32(sdr["InwardID"]),
                                GRN_NUMBER = sdr["GRNNUMBER"].ToString(),
                                GRN_PoNumber = sdr["PoNumber"].ToString(),
                                EmployeeName = sdr["RequestedBy"].ToString(),
                                GRN_CreatedDateTime = sdr["ReceivedDate"].ToString(),
                                StatusTitle = sdr["GRNStatus"].ToString(),
                                VendorName = sdr["VendorName"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }

        private void ExportToExcel(List<GRNMasterData> data)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("GRN Report");

                // Add headers
                ws.Cell(1, 1).Value = "GRN Number";
                ws.Cell(1, 2).Value = "PO Number";
                ws.Cell(1, 3).Value = "Vendor Name";
                ws.Cell(1, 4).Value = "Created By";
                ws.Cell(1, 5).Value = "Created Date";
                ws.Cell(1, 6).Value = "Status";

                // Add data
                int row = 2;
                foreach (var item in data)
                {
                    ws.Cell(row, 1).Value = item.GRN_NUMBER;
                    ws.Cell(row, 2).Value = item.GRN_PoNumber;
                    ws.Cell(row, 3).Value = item.VendorName;
                    ws.Cell(row, 4).Value = item.EmployeeName;
                    ws.Cell(row, 5).Value = item.GRN_CreatedDateTime;
                    ws.Cell(row, 6).Value = item.StatusTitle;
                    row++;
                }

                // Formatting
                ws.Columns().AdjustToContents();
                ws.SheetView.FreezeRows(1);

                // Prepare response
                using (MemoryStream ms = new MemoryStream())
                {
                    wb.SaveAs(ms);
                    ms.Position = 0;

                    string fileName = $"GRNReport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", $"attachment;filename={fileName}");
                    Response.BinaryWrite(ms.ToArray());
                    Response.End();
                }
            }
        }



    }
}