using ClosedXML.Excel;
using LIMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class InventoryShow : System.Web.UI.Page
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
                return;
            }

            if (!IsPostBack)
            {
                string roleId = Session["RoleId"].ToString();
                string currentPageUrl = HttpContext.Current.Request.Url.AbsolutePath;
                string pageName = System.IO.Path.GetFileName(currentPageUrl);

                ViewState["PageIndex"] = 1;
                ViewState["SearchTerm"] = "";
                BindPagedData();
            }
        }

        private void BindPagedData()
        {
            int pageIndex = Convert.ToInt32(ViewState["PageIndex"]);
            string searchTerm = ViewState["SearchTerm"]?.ToString() ?? "";

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

            //int companyUnitId = Convert.ToInt32(Session["CompanyUnitId"]);
            var data = dal.ActiveInventoryDetailsRackSeriesDataGetPaged(pageIndex, PageSize, searchTerm, startDate, endDate, out int totalRecords);
            RepDataShow.DataSource = data;
            RepDataShow.DataBind();
            GeneratePager(totalRecords, pageIndex);
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

            dal.ActiveInventoryDetailsRackSeriesDataGetPaged(1, PageSize, ViewState["SearchTerm"]?.ToString() ?? "", startDate, endDate, out totalRecords);

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

                // Get other filter parameters
                //int companyUnitId = Convert.ToInt32(Session["CompanyUnitId"]);
                string searchTerm = ViewState["SearchTerm"]?.ToString() ?? "";

                // Get the data to export
                var data = GetExportData(searchTerm, startDate, endDate);

                if (data == null || data.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "noData",
                        "alert('No data found to export.');", true);
                    return;
                }

                // Call the export method
                ExportToExcel(data);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "exportError",
                    $"alert('Error exporting to Excel: {ex.Message.Replace("'", "\\'")}');", true);
            }
        }

        private List<InventoryItemData> GetExportData(string searchTerm, DateTime? startDate, DateTime? endDate)
        {
            List<InventoryItemData> items = new List<InventoryItemData>();

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * 
                                  FROM InventoryMaster im 
                                  INNER JOIN Items imm ON imm.ItemId = im.InventoryItemId
							      INNER JOIN ItemType IT ON imm.ItemTypeID = IT.ItemTypeID
                                  INNER JOIN RackTypeDetails rtd ON rtd.RackTypeId = im.InventoryRackTypeId
                                  INNER JOIN RackSerialDetails rsd ON rtd.RackTypeId = rsd.RackTypeId
                                  INNER JOIN UOM um ON um.UOMId = im.InventoryUOMId
                                  INNER JOIN VAT vt ON vt.VATId = im.InventoryGSTId 
                                  WHERE im.InventoryMasterCompanyId = @CompanyId 
                                  AND CHARINDEX(CAST(rsd.RackSerialId AS VARCHAR), CAST(im.InventoryItemLocationStore AS VARCHAR)) > 0 
                                  AND (imm.ItemName LIKE @Search OR imm.ItemCode LIKE @Search)
                                  AND (@StartDate IS NULL OR CONVERT(datetime, im.InventoryCreatedDateTime, 120) >= @StartDate)
                                  AND (@EndDate IS NULL OR CONVERT(datetime, im.InventoryCreatedDateTime, 120) <= @EndDate)
                                  ORDER BY CONVERT(datetime, im.InventoryCreatedDateTime, 120) DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    string likeSearch = "%" + searchTerm + "%";
                    string companyId = Session["MasterCompanyId"].ToString();

                    cmd.Parameters.AddWithValue("@Search", likeSearch);
                    cmd.Parameters.AddWithValue("@CompanyId", companyId);
                    cmd.Parameters.AddWithValue("@StartDate", startDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EndDate", endDate ?? (object)DBNull.Value);

                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new InventoryItemData
                            {
                                // Your existing mapping code here
                                InventoryId = Convert.ToInt32(sdr["InventoryId"]),
                                //InventoryCompanyUnitId = Convert.ToInt32(sdr["InventoryCompanyUnitId"]),
                                InventoryRackTypeId = Convert.ToInt32(sdr["InventoryRackTypeId"]),
                                InventoryItemCategoryId = Convert.ToInt32(sdr["InventoryItemCategoryId"]),
                                InventoryItemId = Convert.ToInt32(sdr["InventoryItemId"]),
                                InventoryUOMId = Convert.ToInt32(sdr["InventoryUOMId"]),
                                InventoryGSTId = Convert.ToInt32(sdr["InventoryGSTId"]),
                                InventoryCurrentStock = Convert.ToDecimal(sdr["InventoryCurrentStock"]),
                                InventoryMinStock = Convert.ToDecimal(sdr["InventoryMinStock"]),
                                InventoryCreatedEmployeeId = Convert.ToInt32(sdr["InventoryCreatedEmployeeId"]),
                                InventoryMasterCompanyId = Convert.ToInt32(sdr["InventoryMasterCompanyId"]),
                                InventoryItemLocationStore = sdr["InventoryItemLocationStore"].ToString(),
                                InventoryCreatedDateTime = sdr["InventoryCreatedDateTime"].ToString(),
                                InventoryUpdatedDateTime = sdr["InventoryUpdatedDateTime"].ToString(),

                                ItemID = sdr["ItemID"] != DBNull.Value ? Convert.ToInt32(sdr["ItemID"]) : 0,
                                ItemCode = sdr["ItemCode"] != DBNull.Value ? sdr["ItemCode"].ToString() : string.Empty,
                                ItemName = sdr["ItemName"] != DBNull.Value ? sdr["ItemName"].ToString() : string.Empty,
                                ItemDescription = sdr["ItemDescription"] != DBNull.Value ? sdr["ItemDescription"].ToString() : string.Empty,
                                PurchaseSubGroupID = sdr["PurchaseSubGroupID"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseSubGroupID"]) : 0,
                                LeadTime = sdr["LeadTime"] != DBNull.Value ? Convert.ToInt32(sdr["LeadTime"]) : 0,
                                UOMID = sdr["UOMID"] != DBNull.Value ? Convert.ToInt32(sdr["UOMID"]) : 0,
                                StockQuantity = sdr["StockQuantity"] != DBNull.Value ? Convert.ToDecimal(sdr["StockQuantity"]) : 0,
                                MinStock = sdr["MinStock"] != DBNull.Value ? Convert.ToDecimal(sdr["MinStock"]) : 0,
                                MaxStock = sdr["MaxStock"] != DBNull.Value ? Convert.ToDecimal(sdr["MaxStock"]) : 0,
                                DisabledItem = sdr["DisabledItem"] != DBNull.Value ? Convert.ToBoolean(sdr["DisabledItem"]) : false,
                                HSNCode = sdr["HSNCode"] != DBNull.Value ? sdr["HSNCode"].ToString() : string.Empty,
                                CreatedDate = sdr["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(sdr["CreatedDate"]) : DateTime.MinValue,
                                VAT = sdr["VAT"] != DBNull.Value ? Convert.ToDecimal(sdr["VAT"]) : 0,
                                Remarks = sdr["Remarks"] != DBNull.Value ? sdr["Remarks"].ToString() : string.Empty,
                                UOMValue = sdr["UOMValue"] != DBNull.Value ? Convert.ToDecimal(sdr["UOMValue"]) : 0,
                                ConversionUOMID = sdr["ConversionUOMID"] != DBNull.Value ? Convert.ToInt32(sdr["ConversionUOMID"]) : 0,
                                ConversionUOMValue = sdr["ConversionUOMValue"] != DBNull.Value ? Convert.ToDecimal(sdr["ConversionUOMValue"]) : 0,
                                ModelNo = sdr["ModelNo"] != DBNull.Value ? sdr["ModelNo"].ToString() : string.Empty,
                                Rate = sdr["Rate"] != DBNull.Value ? Convert.ToDecimal(sdr["Rate"]) : 0,
                                ItemTypeID = sdr["ItemTypeID"] != DBNull.Value ? Convert.ToInt32(sdr["ItemTypeID"]) : 0,
                                Description = sdr["Description"] != DBNull.Value ? sdr["Description"].ToString() : string.Empty,
                               Specification = sdr["Specification"] != DBNull.Value ? sdr["Specification"].ToString() : string.Empty,
                                PurchaseGroupID = sdr["PurchaseGroupID"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseGroupID"]) : 0,
                                CreatedBy = sdr["CreatedBy"] != DBNull.Value ? Convert.ToInt32(sdr["CreatedBy"]) : 0,
                                ConvertUOM = sdr["ConvertUOM"] != DBNull.Value ? Convert.ToInt32(sdr["ConvertUOM"]) : 0,
                                ItemTypeName = sdr["ItemType"] != DBNull.Value ? sdr["ItemType"].ToString() : string.Empty,

                                UOMId = Convert.ToInt32(sdr["UOMId"]),
                                UOMCode = sdr["UOMName"].ToString(),
                                UOMName = sdr["UOMName"].ToString(),

                                VATID = Convert.ToInt32(sdr["VATID"]),
                                VATCode = sdr["VATCode"].ToString(),
                                Percentage = Convert.ToDecimal(sdr["Percentage"]),

                                RackTypeId = Convert.IsDBNull(sdr["RackTypeId"]) ? 0 : Convert.ToInt32(sdr["RackTypeId"]),
                                RackType = Convert.IsDBNull(sdr["RackType"]) ? string.Empty : sdr["RackType"].ToString(),
                                RackMasterCompanyId = Convert.IsDBNull(sdr["RackMasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["RackMasterCompanyId"]),
                                RackCreatedEmployeeId = Convert.IsDBNull(sdr["RackCreatedEmployeeId"]) ? 0 : Convert.ToInt32(sdr["RackCreatedEmployeeId"]),
                                RackCreatedDateTime = Convert.IsDBNull(sdr["RackCreatedDateTime"]) ? string.Empty : sdr["RackCreatedDateTime"].ToString(),
                                RackUpdatedDateTime = Convert.IsDBNull(sdr["RackUpdatedDateTime"]) ? string.Empty : sdr["RackUpdatedDateTime"].ToString(),
                                RackAlphSeries = Convert.IsDBNull(sdr["RackAlphSeries"]) ? string.Empty : sdr["RackAlphSeries"].ToString(),
                                TotalSerialNumber = Convert.IsDBNull(sdr["TotalSerialNumber"]) ? 0 : Convert.ToInt32(sdr["TotalSerialNumber"]),
                                RackSerialNumber = Convert.IsDBNull(sdr["RackSerialNumber"]) ? string.Empty : sdr["RackSerialNumber"].ToString(),
                                RackSerialId = Convert.IsDBNull(sdr["RackSerialId"]) ? 0 : Convert.ToInt32(sdr["RackSerialId"]),

                                //UnitName = sdr["UnitName"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }

        private void ExportToExcel(List<InventoryItemData> data)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Inventory Report");

                // Add headers
                ws.Cell(1, 1).Value = "Item Code";
                ws.Cell(1, 2).Value = "Item Name";
                ws.Cell(1, 3).Value = "Rack Name";
                ws.Cell(1, 4).Value = "Rack";
                ws.Cell(1, 5).Value = "Stock";
                ws.Cell(1, 6).Value = "Created Date";

                // Add data
                int row = 2;
                foreach (var item in data)
                {
                    ws.Cell(row, 1).Value = item.ItemCode;
                    ws.Cell(row, 2).Value = item.ItemName;
                    ws.Cell(row, 3).Value = item.RackType;
                    ws.Cell(row, 4).Value = item.RackSerialNumber;
                    ws.Cell(row, 5).Value = item.InventoryCurrentStock;
                    ws.Cell(row, 6).Value = item.InventoryCreatedDateTime;
                    row++;
                }

                // Formatting (optional)
                ws.Columns().AdjustToContents();
                ws.SheetView.FreezeRows(1);

                // Prepare response
                using (MemoryStream ms = new MemoryStream())
                {
                    wb.SaveAs(ms);
                    ms.Position = 0;

                    string fileName = $"InventoryReport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", $"attachment;filename={fileName}");
                    Response.BinaryWrite(ms.ToArray());
                    Response.End();
                }
            }
        }


        protected void RepDataShow_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ItemEdit")
            {
                string GetID = e.CommandArgument.ToString();
                string encodedEventID = Convert.ToBase64String(Encoding.UTF8.GetBytes(GetID));
                string url = $"InventoryEdit.aspx?PageID={encodedEventID}";
                Response.Redirect(url);
            }
        }


    }
}