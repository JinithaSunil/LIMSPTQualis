using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LIMS.Models;
using System.Linq;

namespace LIMS
{
    public partial class ItemTransactionShow : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ToString());
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        string date = DateTime.Today.ToString("dd-MMM-yyyy");
        string time = DateTime.Now.ToString("h:mm:ss tt");
        string GetDateTime = DateTime.Today.ToString("dd-MMM-yyyy") + " " + DateTime.Now.ToString("h:mm:ss tt");
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
                string urlToCheck = pageName;

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
                int companyUnitId = Convert.ToInt32(Session["CompanyUnitId"]);
                string websiteURL = Session["SMTP_URL"].ToString();

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

                int totalRecords;
                List<ItemTransactionData> res = dal.ItemTransactionDataGetPaged(pageIndex, PageSize, searchTerm,startDate, endDate, out totalRecords);
                
                if (res != null && res.Count > 0)
                {
                    string FileName = res[0].IT_QRFileName;
                    string FileFullPath = websiteURL + "/QRCodeSave/" + FileName;
                    string resolvedUrl = ResolveUrl(FileFullPath);
                    RepDataShow.DataSource = res;
                    RepDataShow.DataBind();
                    GeneratePager(totalRecords, pageIndex);
                }
                else
                {
                    RepDataShow.DataSource = null;
                    RepDataShow.DataBind();
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts","<script> alert('No data found for this filter.'); </script>");
                }
            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts","<script> alert('Error: " + ex.Message.Replace("'", "\\'") + "'); </script>");
            }
        }
       
        protected void RepDataShow_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ItemEdit")
            {
                string GetID = e.CommandArgument.ToString();
                string encodedEventID = Convert.ToBase64String(Encoding.UTF8.GetBytes(GetID));
                string url = $"ItemTransactionEdit.aspx?PageID={encodedEventID}";
                Response.Redirect(url);
            }
            else if (e.CommandName == "ItemQRCodeShow")
            {
                string GetID = e.CommandArgument.ToString();
                string encodedEventID = Convert.ToBase64String(Encoding.UTF8.GetBytes(GetID));
                string url = $"QRCodeDisplay.aspx?PageID={encodedEventID}";
                Response.Redirect(url);
            }
            //else if (e.CommandName == "ItemDelete")
            //{
            //    string GetID = e.CommandArgument.ToString();
            //    List<ItemTransactionData> responseData = dal.ItemTransactionDataGet();
            //    List<ItemTransactionData> filteredData2 = responseData.Where(item => item.IT_Id == Convert.ToInt32(GetID)).ToList();
            //    if (filteredData2 != null && filteredData2.Count > 0)
            //    {
            //        ItemTransactionData selectedItem = filteredData2[0];
            //        int ITstatusId = selectedItem.IT_Status;
            //        int PoId = selectedItem.IT_PoId;
            //        if (ITstatusId == 2)
            //        {
            //            double OrderedQty = Convert.ToDouble(selectedItem.IT_OrderedQty);
            //            double ReceivedQty = Convert.ToDouble(selectedItem.IT_ReceivedQty);
            //            double BalanceQty = Convert.ToDouble(selectedItem.IT_BalanceQty);
            //            double PreReceivedQty = Convert.ToDouble(selectedItem.IT_PreviousReceivedQty);
            //            double PreBalanceQty = Convert.ToDouble(selectedItem.IT_PreviousBalanceQty);
            //            string query = @"update ItemTransactionMaster set IT_IsDelete = @IT_IsDelete, IT_MasterCompanyId = @IT_MasterCompanyId, 
            //                             IT_CreatedEmployeeId = @IT_CreatedEmployeeId, IT_CompanyUnitId = @IT_CompanyUnitId, 
            //                             IT_UpdatedDateTime = @IT_UpdatedDateTime WHERE IT_Id = @IT_Id";
            //            using (SqlCommand cmd = new SqlCommand(query, con))
            //            {
            //                cmd.Parameters.AddWithValue("@IT_IsDelete", 1);
            //                cmd.Parameters.AddWithValue("@IT_MasterCompanyId", Session["MasterCompanyId"].ToString());
            //                cmd.Parameters.AddWithValue("@IT_CreatedEmployeeId", Session["EmployeeID"].ToString());
            //                cmd.Parameters.AddWithValue("@IT_CompanyUnitId", Session["CompanyUnitId"].ToString());
            //                cmd.Parameters.AddWithValue("@IT_UpdatedDateTime", GetDateTime);
            //                cmd.Parameters.AddWithValue("@IT_Id", GetID);
            //                con.Open();
            //                int a = cmd.ExecuteNonQuery();
            //                con.Close();
            //                if (a > 0)
            //                {
            //                    using (SqlConnection con2 = new SqlConnection(constr))
            //                    {
            //                        string str1 = @"UPDATE PoExcelUploadMaster set PO_ReceivedQty = @PO_ReceivedQty,PO_BalanceQty = @PO_BalanceQty,
            //                                        PO_MasterCompanyId = @PO_MasterCompanyId, PO_CreatedEmployeeId = @PO_CreatedEmployeeId, 
            //                                        PO_UpdatedDateTime = @PO_UpdatedDateTime WHERE PO_Id = @PO_Id";
            //                        SqlCommand cmd2 = new SqlCommand(str1, con2);
            //                        cmd2.Parameters.AddWithValue("@PO_Id", PoId);
            //                        cmd2.Parameters.AddWithValue("@PO_ReceivedQty", PreReceivedQty);
            //                        cmd2.Parameters.AddWithValue("@PO_BalanceQty", PreBalanceQty);
            //                        cmd2.Parameters.AddWithValue("@PO_MasterCompanyId", Session["MasterCompanyId"].ToString());
            //                        cmd2.Parameters.AddWithValue("@PO_CreatedEmployeeId", Session["EmployeeID"].ToString());
            //                        cmd2.Parameters.AddWithValue("@PO_UpdatedDateTime", GetDateTime);
            //                        con2.Open();
            //                        int j = cmd2.ExecuteNonQuery();
            //                        con2.Close();
            //                        if (j > 0)
            //                        {
            //                            var TableRowId = j.ToString();
            //                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Delete Records successfully'); setTimeout(function() { window.location = 'ItemTransactionShow.aspx'; }, 100);</script>");
            //                        }
            //                        else
            //                        {
            //                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('PO Details not Updated!!'); setTimeout(function() { window.location = 'ItemTransactionShow.aspx'; }, 100);</script>");
            //                        }
            //                    }
            //                }
            //                else
            //                {
            //                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('something went wrong try later!');</script>");
            //                }
            //            }
            //        }
            //        else
            //        {
            //            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('GRN was Generated,so you can not Delete these Data!'); </script>");
            //        }
            //    }
            //    else
            //    {
            //        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('ALL Records Not Found.'); </script>");
            //    }
            //}

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
                startDate = DateTime.Parse(txtStartDate.Text);
            }

            if (!string.IsNullOrEmpty(txtEndDate.Text))
            {
                endDate = DateTime.Parse(txtEndDate.Text).AddDays(1).AddSeconds(-1);
            }

            dal.ItemTransactionDataGetPaged(1, PageSize, ViewState["SearchTerm"]?.ToString() ?? "",
                startDate, endDate, out int totalRecords);

            int lastPageIndex = (int)Math.Ceiling((double)totalRecords / PageSize);
            ViewState["PageIndex"] = lastPageIndex;
            BindPagedData();
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

        protected void btnDownloadExcelFile_Click(object sender, EventArgs e)
        {
            try
            {
                // Get the date range from your controls
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

                // Get other filter parameters
                string searchTerm = ViewState["SearchTerm"]?.ToString() ?? "";
                int companyUnitId = Convert.ToInt32(Session["CompanyUnitId"]);

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

        private List<ItemTransactionData> GetExportData(string searchTerm, DateTime? startDate, DateTime? endDate)
        {
            List<ItemTransactionData> items = new List<ItemTransactionData>();

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM ItemTransactionMaster itr
                                  INNER JOIN InventoryMaster im ON im.InventoryId = itr.IT_InventoryId
                                  INNER JOIN Items itm ON itm.ItemId = itr.IT_ItemId
                                  INNER JOIN VendorDetails vds ON vds.VendorId = itr.IT_VendorId
                                  INNER JOIN UOM um ON um.UOMId = itr.IT_UOMId
                                  INNER JOIN VAT vt ON vt.VATId = itr.IT_GSTId
                                  INNER JOIN RackTypeDetails rtd ON rtd.RackTypeId = itr.IT_RackTypeId
                                  WHERE itr.IT_MasterCompanyId = @CompanyId 
                                  AND (itm.ItemName LIKE @Search OR itm.ItemCode LIKE @Search OR 
                                       vds.VendorName LIKE @Search OR itr.IT_PoNumber LIKE @Search)
                                  AND (@StartDate IS NULL OR CONVERT(datetime, itr.IT_CreatedDateTime, 120) >= @StartDate)
                                  AND (@EndDate IS NULL OR CONVERT(datetime, itr.IT_CreatedDateTime, 120) <= @EndDate)
                                  ORDER BY CONVERT(datetime, itr.IT_CreatedDateTime, 120) DESC";

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
                            // Your existing mapping code here
                            ItemTransactionData data = new ItemTransactionData
                            {
                                IT_Id = Convert.IsDBNull(sdr["IT_Id"]) ? 0 : Convert.ToInt32(sdr["IT_Id"]),
                                IT_InventoryId = Convert.IsDBNull(sdr["IT_InventoryId"]) ? 0 : Convert.ToInt32(sdr["IT_InventoryId"]),
                                IT_ItemId = Convert.IsDBNull(sdr["IT_ItemId"]) ? 0 : Convert.ToInt32(sdr["IT_ItemId"]),
                                IT_VendorId = Convert.IsDBNull(sdr["IT_VendorId"]) ? 0 : Convert.ToInt32(sdr["IT_VendorId"]),
                                IT_PoNumber = Convert.IsDBNull(sdr["IT_PoNumber"]) ? string.Empty : sdr["IT_PoNumber"].ToString(),
                                IT_UOMId = Convert.IsDBNull(sdr["IT_UOMId"]) ? 0 : Convert.ToInt32(sdr["IT_UOMId"]),
                                IT_GSTId = Convert.IsDBNull(sdr["IT_GSTId"]) ? 0 : Convert.ToInt32(sdr["IT_GSTId"]),
                                IT_RackTypeId = Convert.IsDBNull(sdr["IT_RackTypeId"]) ? 0 : Convert.ToInt32(sdr["IT_RackTypeId"]),
                                IT_BasicPrice = Convert.IsDBNull(sdr["IT_BasicPrice"]) ? 0 : Convert.ToDecimal(sdr["IT_BasicPrice"]),
                                IT_ReceivedQty = Convert.IsDBNull(sdr["IT_ReceivedQty"]) ? 0 : Convert.ToDecimal(sdr["IT_ReceivedQty"]),
                                IT_ShortFall = Convert.IsDBNull(sdr["IT_ReceivedQty"]) ? 0 : Convert.ToDecimal(sdr["IT_ShortFall"]),
                                IT_PreviousShortFall = Convert.IsDBNull(sdr["IT_PreviousShortFall"]) ? 0 : Convert.ToDecimal(sdr["IT_PreviousShortFall"]),
                                IT_ReceivedQtyInvoice = Convert.IsDBNull(sdr["IT_ReceivedQtyInvoice"]) ? 0 : Convert.ToDecimal(sdr["IT_ReceivedQtyInvoice"]),
                                IT_PreviousReceivedQtyInvoice = Convert.IsDBNull(sdr["IT_PreviousReceivedQtyInvoice"]) ? 0 : Convert.ToDecimal(sdr["IT_PreviousReceivedQtyInvoice"]),
                                IT_AcceptedQty = Convert.IsDBNull(sdr["IT_AcceptedQty"]) ? 0 : Convert.ToDecimal(sdr["IT_AcceptedQty"]),
                                IT_RejectedQty = Convert.IsDBNull(sdr["IT_RejectedQty"]) ? 0 : Convert.ToDecimal(sdr["IT_RejectedQty"]),
                                IT_PreviousAcceptedQty = Convert.IsDBNull(sdr["IT_PreviousAcceptedQty"]) ? 0 : Convert.ToDecimal(sdr["IT_PreviousAcceptedQty"]),
                                IT_PreviousRejectedQty = Convert.IsDBNull(sdr["IT_PreviousRejectedQty"]) ? 0 : Convert.ToDecimal(sdr["IT_PreviousRejectedQty"]),
                                IT_PoId = Convert.IsDBNull(sdr["IT_PoId"]) ? 0 : Convert.ToInt32(sdr["IT_PoId"]),
                                IT_OrderedQty = Convert.IsDBNull(sdr["IT_OrderedQty"]) ? 0 : Convert.ToDecimal(sdr["IT_OrderedQty"]),
                                IT_QRCodeNumber = Convert.IsDBNull(sdr["IT_QRCodeNumber"]) ? string.Empty : sdr["IT_QRCodeNumber"].ToString(),
                                IT_QRFileName = Convert.IsDBNull(sdr["IT_QRFileName"]) ? string.Empty : sdr["IT_QRFileName"].ToString(),
                                IT_BalanceQty = Convert.IsDBNull(sdr["IT_BalanceQty"]) ? 0 : Convert.ToDecimal(sdr["IT_BalanceQty"]),
                                IT_TotalAmount = Convert.IsDBNull(sdr["IT_TotalAmount"]) ? 0 : Convert.ToDecimal(sdr["IT_TotalAmount"]),
                                IT_BatchNumber = Convert.IsDBNull(sdr["IT_BatchNumber"]) ? string.Empty : sdr["IT_BatchNumber"].ToString(),
                                IT_ExpiryDateTime = Convert.IsDBNull(sdr["IT_ExpiryDateTime"]) ? string.Empty : sdr["IT_ExpiryDateTime"].ToString(),
                                IT_DeliveryDateTime = Convert.IsDBNull(sdr["IT_DeliveryDateTime"]) ? string.Empty : sdr["IT_DeliveryDateTime"].ToString(),
                                IT_CreatedDateTime = Convert.IsDBNull(sdr["IT_CreatedDateTime"]) ? string.Empty : sdr["IT_CreatedDateTime"].ToString(),
                                IT_UpdatedDateTime = Convert.IsDBNull(sdr["IT_UpdatedDateTime"]) ? string.Empty : sdr["IT_UpdatedDateTime"].ToString(),
                                IT_RackRowSeriesNo = Convert.IsDBNull(sdr["IT_RackRowSeriesNo"]) ? string.Empty : sdr["IT_RackRowSeriesNo"].ToString(),
                                IT_CreatedEmployeeId = Convert.IsDBNull(sdr["IT_CreatedEmployeeId"]) ? 0 : Convert.ToInt32(sdr["IT_CreatedEmployeeId"]),
                                IT_MasterCompanyId = Convert.IsDBNull(sdr["IT_MasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["IT_MasterCompanyId"]),
                                IT_Status = Convert.IsDBNull(sdr["IT_Status"]) ? 0 : Convert.ToInt32(sdr["IT_Status"]),
                                IT_PreviousReceivedQty = Convert.IsDBNull(sdr["IT_PreviousReceivedQty"]) ? 0 : Convert.ToDecimal(sdr["IT_PreviousReceivedQty"]),
                                IT_PreviousBalanceQty = Convert.IsDBNull(sdr["IT_PreviousBalanceQty"]) ? 0 : Convert.ToDecimal(sdr["IT_PreviousBalanceQty"]),
                                IT_IsDelete = Convert.IsDBNull(sdr["IT_IsDelete"]) ? 0 : Convert.ToInt32(sdr["IT_IsDelete"]),

                                ////IT_ReturnQty = Convert.IsDBNull(sdr["IT_ReturnQty"]) ? 0 : Convert.ToDecimal(sdr["IT_ReturnQty"]),
                                ////IT_ReplaceQty = Convert.IsDBNull(sdr["IT_ReplaceQty"]) ? 0 : Convert.ToDecimal(sdr["IT_ReplaceQty"]),
                                ////IT_PreReturnQty = Convert.IsDBNull(sdr["IT_PreReturnQty"]) ? 0 : Convert.ToDecimal(sdr["IT_PreReturnQty"]),
                                ////IT_PreReplaceQty = Convert.IsDBNull(sdr["IT_PreReplaceQty"]) ? 0 : Convert.ToDecimal(sdr["IT_PreReplaceQty"]),
                                ////IT_isValid = Convert.IsDBNull(sdr["IT_isValid"]) ? 0 : Convert.ToInt32(sdr["IT_isValid"]),
                                ////IT_ReturnDateTime = Convert.IsDBNull(sdr["IT_ReturnDateTime"]) ? string.Empty : sdr["IT_ReturnDateTime"].ToString(),
                                ////IT_ReplaceDateTime = Convert.IsDBNull(sdr["IT_ReplaceDateTime"]) ? string.Empty : sdr["IT_ReplaceDateTime"].ToString(),

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
                                VAT = sdr["VAT"] != DBNull.Value ? Convert.ToDecimal(sdr["VAT"]) : 0,
                                UOMValue = sdr["UOMValue"] != DBNull.Value ? Convert.ToDecimal(sdr["UOMValue"]) : 0,
                                ConversionUOMID = sdr["ConversionUOMID"] != DBNull.Value ? Convert.ToInt32(sdr["ConversionUOMID"]) : 0,
                                ConversionUOMValue = sdr["ConversionUOMValue"] != DBNull.Value ? Convert.ToDecimal(sdr["ConversionUOMValue"]) : 0,
                                ModelNo = sdr["ModelNo"] != DBNull.Value ? sdr["ModelNo"].ToString() : string.Empty,
                                ItemTypeID = sdr["ItemTypeID"] != DBNull.Value ? Convert.ToInt32(sdr["ItemTypeID"]) : 0,
                                Description = sdr["Description"] != DBNull.Value ? sdr["Description"].ToString() : string.Empty,
                                Specification = sdr["Specification"] != DBNull.Value ? sdr["Specification"].ToString() : string.Empty,
                                PurchaseGroupID = sdr["PurchaseGroupID"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseGroupID"]) : 0,
                                CreatedBy = sdr["CreatedBy"] != DBNull.Value ? Convert.ToInt32(sdr["CreatedBy"]) : 0,
                                ConvertUOM = sdr["ConvertUOM"] != DBNull.Value ? Convert.ToInt32(sdr["ConvertUOM"]) : 0,
                                //ItemTypeName = sdr["ItemType"] != DBNull.Value ? sdr["ItemType"].ToString() : string.Empty,

                                InventoryId = sdr["InventoryId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryId"]) : 0,
                                InventoryRackTypeId = sdr["InventoryRackTypeId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryRackTypeId"]) : 0,
                                InventoryItemCategoryId = sdr["InventoryItemCategoryId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryItemCategoryId"]) : 0,
                                InventoryItemId = sdr["InventoryItemId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryItemId"]) : 0,
                                InventoryUOMId = sdr["InventoryUOMId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryUOMId"]) : 0,
                                InventoryGSTId = sdr["InventoryGSTId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryGSTId"]) : 0,
                                InventoryCurrentStock = sdr["InventoryCurrentStock"] != DBNull.Value ? Convert.ToDecimal(sdr["InventoryCurrentStock"]) : 0,
                                InventoryMinStock = sdr["InventoryMinStock"] != DBNull.Value ? Convert.ToDecimal(sdr["InventoryMinStock"]) : 0,
                                InventoryCreatedEmployeeId = sdr["InventoryCreatedEmployeeId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryCreatedEmployeeId"]) : 0,
                                InventoryMasterCompanyId = sdr["InventoryMasterCompanyId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryMasterCompanyId"]) : 0,
                                InventoryItemLocationStore = sdr["InventoryItemLocationStore"] != DBNull.Value ? sdr["InventoryItemLocationStore"].ToString() : string.Empty,
                                InventoryCreatedDateTime = sdr["InventoryCreatedDateTime"] != DBNull.Value ? sdr["InventoryCreatedDateTime"].ToString() : string.Empty,
                                InventoryUpdatedDateTime = sdr["InventoryUpdatedDateTime"] != DBNull.Value ? sdr["InventoryUpdatedDateTime"].ToString() : string.Empty,

                                UOMId = sdr["UOMId"] != DBNull.Value ? Convert.ToInt32(sdr["UOMId"]) : 0,
                                UOMCode = sdr["UOMName"] != DBNull.Value ? sdr["UOMName"].ToString() : string.Empty,
                                UOMName = sdr["UOMName"] != DBNull.Value ? sdr["UOMName"].ToString() : string.Empty,

                                VATID = sdr["VATID"] != DBNull.Value ? Convert.ToInt32(sdr["VATID"]) : 0,
                                VATCode = sdr["VATCode"] != DBNull.Value ? sdr["VATCode"].ToString() : string.Empty,
                                Percentage = sdr["Percentage"] != DBNull.Value ? Convert.ToDecimal(sdr["Percentage"]) : 0,
                              
                                RackTypeId = sdr["RackTypeId"] != DBNull.Value ? Convert.ToInt32(sdr["RackTypeId"]) : 0,
                                RackType = sdr["RackType"] != DBNull.Value ? sdr["RackType"].ToString() : string.Empty,
                                RackMasterCompanyId = sdr["RackMasterCompanyId"] != DBNull.Value ? Convert.ToInt32(sdr["RackMasterCompanyId"]) : 0,
                                RackCreatedEmployeeId = sdr["RackCreatedEmployeeId"] != DBNull.Value ? Convert.ToInt32(sdr["RackCreatedEmployeeId"]) : 0,
                                RackCreatedDateTime = sdr["RackCreatedDateTime"] != DBNull.Value ? sdr["RackCreatedDateTime"].ToString() : string.Empty,
                                RackUpdatedDateTime = sdr["RackUpdatedDateTime"] != DBNull.Value ? sdr["RackUpdatedDateTime"].ToString() : string.Empty,


                                VendorCode = sdr["VendorCode"]?.ToString(),
                                VendorName = sdr["VendorName"]?.ToString(),
                                DealerID = sdr["DealerID"] != DBNull.Value ? Convert.ToInt32(sdr["DealerID"]) : (int?)null,
                                Website = sdr["Website"]?.ToString(),
                                YearOfEstablishment = sdr["YearOfEstablishment"]?.ToString(),
                                TypeOfBusiness = sdr["TypeOfBusiness"]?.ToString(),
                                CompanyTradeLicence = sdr["CompanyTradeLicence"]?.ToString(),
                                YearlyTurnover1 = sdr["YearlyTurnover1"] != DBNull.Value ? Convert.ToDecimal(sdr["YearlyTurnover1"]) : (decimal?)null,
                                RequiredCredit = sdr["RequiredCredit"] != DBNull.Value ? Convert.ToDecimal(sdr["RequiredCredit"]) : (decimal?)null,
                                ApprovedCredit = sdr["ApprovedCredit"] != DBNull.Value ? Convert.ToDecimal(sdr["ApprovedCredit"]) : (decimal?)null,
                                PanNumber = sdr["PanNumber"]?.ToString(),
                                AdharNumber = sdr["AdharNumber"]?.ToString(),
                                EstablishmentID = sdr["EstablishmentID"] != DBNull.Value ? Convert.ToInt32(sdr["EstablishmentID"]) : (int?)null,
                                RegistrationNumber = sdr["RegistrationNumber"]?.ToString(),
                                YearlyTurnover2 = sdr["YearlyTurnover2"] != DBNull.Value ? Convert.ToDecimal(sdr["YearlyTurnover2"]) : (decimal?)null,
                                YearlyTurnover3 = sdr["YearlyTurnover3"] != DBNull.Value ? Convert.ToDecimal(sdr["YearlyTurnover3"]) : (decimal?)null,
                                Year1 = sdr["Year1"]?.ToString(),
                                Year2 = sdr["Year2"]?.ToString(),
                                Year3 = sdr["Year3"]?.ToString(),
                                UANNumber = sdr["UANNumber"]?.ToString(),
                                PFNumber = sdr["PFNumber"]?.ToString(),
                                NumberofEmployee = sdr["NumberofEmployee"].ToString(),
                                IEC = sdr["IEC"]?.ToString(),
                                ESIC = sdr["ESIC"]?.ToString(),
                                VendorStatus = sdr["VendorStatus"].ToString(),
                                VendorCurrency = sdr["VendorCurrency"]?.ToString(),
                                CompanySizeID = sdr["CompanySizeID"].ToString(),
                            };

                            items.Add(data);
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }

        private void ExportToExcel(List<ItemTransactionData> data)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Item Transactions");
                ws.Cell(1, 1).Value = "PO Number";
                ws.Cell(1, 2).Value = "Item Code";
                ws.Cell(1, 3).Value = "Item Name";
                ws.Cell(1, 4).Value = "Vendor";
                ws.Cell(1, 5).Value = "Received Qty";
                ws.Cell(1, 6).Value = "Shortfall Qty";
                ws.Cell(1, 7).Value = "Balance Qty";
                ws.Cell(1, 8).Value = "Batch Number";
                ws.Cell(1, 9).Value = "Created Date";
                int row = 2;
                foreach (var item in data)
                {
                    ws.Cell(row, 1).Value = item.IT_PoNumber;
                    ws.Cell(row, 2).Value = item.ItemCode;
                    ws.Cell(row, 3).Value = item.ItemName;
                    ws.Cell(row, 4).Value = item.VendorName;
                    ws.Cell(row, 5).Value = item.IT_ReceivedQty;
                    ws.Cell(row, 6).Value = item.IT_ShortFall;
                    ws.Cell(row, 7).Value = item.IT_BalanceQty;
                    ws.Cell(row, 8).Value = item.IT_BatchNumber;
                    ws.Cell(row, 9).Value = item.IT_CreatedDateTime;
                    row++;
                }
                ws.Columns().AdjustToContents();
                ws.SheetView.FreezeRows(1);
                using (MemoryStream ms = new MemoryStream())
                {
                    wb.SaveAs(ms);
                    ms.Position = 0;

                    string fileName = $"ItemTransactions_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
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