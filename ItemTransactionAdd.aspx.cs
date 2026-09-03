using LIMS.Models;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ZXing;

namespace LIMS
{
    public partial class ItemTransactionAdd : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ToString());
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        string date = DateTime.Today.ToString("dd-MMM-yyyy");
        string time = DateTime.Now.ToString("h:mm:ss tt");
        string GetDateTime = DateTime.Today.ToString("dd-MMM-yyyy") + " " + DateTime.Now.ToString("h:mm:ss tt");

        DataAccessLayer dal = new DataAccessLayer();
        DAL.CM _cm = new DAL.CM();
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

                    txtDeliveryDateTime.Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
                    //ddlBranchUnit.Enabled = false;
                    ddlRackType.Enabled = false;
                    ddlUOM.Enabled = false;
                    ddlVAT.Enabled = false;
                    ddlVendor.Enabled = false;
                    txtHSNCode.Enabled = false;
                    txtItemName.Enabled = false;
                    txtBasicPrice.Enabled = false;
                    txtPreBalanceQty.Enabled = false;
                    txtPreReceivedQty.Enabled = false;
                    txtPreAcceptQty.Enabled = false;
                    txtPreRejectedQty.Enabled = false;
                    txtBalanceQty.Enabled = false;
                    txtCurrentStock.Enabled = false;
                    txtMinStock.Enabled = false;
                    /* txtShortFallPrevious.Enabled = false; */
                    txtShortFall.Enabled = false;
                    txtTotalShortFallPrevious.Enabled = false;
                    txtTotalPreReceivedQtyGetting.Enabled = false;
                    txtRejectQty.Enabled = false;
                    //txtIGIRReceivedQty.Enabled = false;

                    try
                    {
                        List<PurchaseOrder> filteredData111 = dal.GetPurchaseOrdersData();
                        List<PurchaseOrder> filteredData11 = filteredData111.Where(item => (item.PurchaseOrderStatus == Convert.ToInt32(6) || item.PurchaseOrderStatus == Convert.ToInt32(15))).ToList();
                        //List<PurchaseOrder> filteredData11 = filteredData111.Where(item => (item.PurchaseOrderStatus == Convert.ToInt32(6) || item.PurchaseOrderStatus == Convert.ToInt32(15)) && item.PO_StatusForIT == 1).ToList();
                        if (filteredData11 != null && filteredData11.Count > 0)
                        {
                            ddlPoNo.DataSource = filteredData11;
                            ddlPoNo.DataTextField = "PONumber";
                            ddlPoNo.DataValueField = "PurchaseOrderID";
                            ddlPoNo.DataBind();
                            ddlPoNo.Items.Insert(0, new ListItem("Select PO Number", ""));
                        }
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('PO Complete First/Records Not Found.'); </script>");
                        }

                        List<VendorDetails> VendorData = dal.GetVendorDetails();
                        ddlVendor.DataSource = VendorData;
                        ddlVendor.DataTextField = "VendorName";
                        ddlVendor.DataValueField = "VendorID";
                        ddlVendor.DataBind();
                        ddlVendor.Items.Insert(0, new ListItem("Select", ""));

                        List<UOMData> uomData = dal.UOMDetailsGet();
                        ddlUOM.DataSource = uomData;
                        ddlUOM.DataTextField = "UOMCode";
                        ddlUOM.DataValueField = "UOMId";
                        ddlUOM.DataBind();
                        ddlUOM.Items.Insert(0, new ListItem("Select", ""));

                        DataTable dt = _cm.getVAT();
						 if(dt.Rows.Count > 0){
							 ddlVAT.DataSource = dt;
							 ddlVAT.DataTextField = "VATCode";
							 ddlVAT.DataValueField = "VATId";
							 ddlVAT.DataBind();
							 ddlVAT.Items.Insert(0, new ListItem("Select", ""));
						 }
                        List<RackTypeDetailsData> response1 = dal.RackTypeDetailsUnitWiseDisplay();
                        ddlRackType.DataSource = response1;
                        ddlRackType.DataTextField = "RackType";
                        ddlRackType.DataValueField = "RackTypeId";
                        ddlRackType.DataBind();
                        ddlRackType.Items.Insert(0, new ListItem("Select", ""));
                    }
                    catch (Exception ex)
                    {
                        if (!(ex is System.Threading.ThreadAbortException))
                        {
                            int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                            var s = new StackTrace(ex);
                            var methodName = s.GetFrames().Select(f => f.GetMethod()).First(m => m.Module.Assembly == Assembly.GetExecutingAssembly()).Name;
                            string PageName = Path.GetFileName(Request.Url.AbsolutePath);

                            bool success = dal.ErrorLogData(methodName, ex.Message, PageName + ".cs", linenum.ToString());
                            if (success)
                            {
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error Log Data Saved: " + ex.Message.Replace("'", "\\'") + "'); </script>");
                            }
                            else
                            {
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: " + ex.Message.Replace("'", "\\'") + "'); </script>");
                            }
                        }
                    }

                }
            }
        }

        protected void ddlRackType_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedUnitId;
            int selectedRackTypeId;

            //if (!int.TryParse(ddlBranchUnit.SelectedItem.Value, out selectedUnitId))
            //{
            //    ScriptManager.RegisterStartupScript(this, this.GetType(), "UnitAlert", "alert('Please select a Unit');", true);
            //    return;
            //}

            if (int.TryParse(ddlRackType.SelectedValue, out selectedRackTypeId))
            {
                BindRackRowSeriesNumbers(selectedRackTypeId);
            }
        }

        private void BindRackRowSeriesNumbers(int selectedRackTypeId)
        {
            List<RackNumberData> allRackNumbers = dal.AllRackRowSerialNumberDisplay();
            List<RackNumberData> filteredRackNumbers = allRackNumbers.Where(item => item.RackTypeId == selectedRackTypeId).ToList();

            chkRowSeries.DataSource = filteredRackNumbers;
            chkRowSeries.DataTextField = "RackSerialNumber";
            chkRowSeries.DataValueField = "RackSerialId";
            chkRowSeries.DataBind();
        }

        protected void ddlPoNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string poNumber = ddlPoNo.SelectedItem.Text;
            List<PurchaseOrder> response = dal.GetPurchaseOrderDetailsData();
            List<PurchaseOrder> filteredData = response.Where(item => item.PONumber == poNumber && item.PO_StatusForIT == 0).ToList();

            if (filteredData != null && filteredData.Count > 0)
            {
                PurchaseOrder selectedItem = filteredData[0];
                GridView1.DataSource = filteredData;
                GridView1.DataBind();

                ddlPoItemCode.DataSource = filteredData;
                ddlPoItemCode.DataTextField = "ItemCode";
                ddlPoItemCode.DataValueField = "ItemId";
                ddlPoItemCode.DataBind();
                ddlPoItemCode.Items.Insert(0, new ListItem("--Item Code--", ""));
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No Records Found.'); setTimeout(function() { window.location = 'ItemTransactionAdd.aspx'; }, 10);</script>");
            }
        }

        /* PO Details from the Excel Upload Data */
        protected void ddlPoItemCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            string getPoNo = ddlPoNo.SelectedItem.Text;
            string getItemId = ddlPoItemCode.SelectedItem.Value;
            string poItemCode = ddlPoItemCode.SelectedItem.Text;

            List<PurchaseOrder> responseData = dal.GetPurchaseOrderDetailsData();
            List<PurchaseOrder> filteredData1 = responseData.Where(item => item.PONumber == getPoNo && item.ItemCode == poItemCode).ToList();

            if (filteredData1 != null && filteredData1.Count > 0)
            {
                PurchaseOrder selectedItem = filteredData1[0];
                double orderQty = Convert.ToDouble(selectedItem.PO_OrderedQty.ToString());
                double receiveQty = Convert.ToDouble(selectedItem.PO_ReceivedQty.ToString());
                double balanceQty = Convert.ToDouble(selectedItem.PO_BalanceQty.ToString());
                txtPreReceivedQty.Text = receiveQty.ToString();
                txtPreBalanceQty.Text = balanceQty.ToString();
                txtPreAcceptQty.Text = selectedItem.PO_AcceptQty.ToString();
                txtPreRejectedQty.Text = selectedItem.PO_RejectQty.ToString();
                txtBasicPrice.Text = selectedItem.Rate.ToString();
                lblorderQty.Text = orderQty.ToString();
                lblreceiveQty.Text = receiveQty.ToString();
                lblbalanceQty.Text = balanceQty.ToString();
                lblPoNo.Text = selectedItem.PurchaseOrderID.ToString();
                ViewState["PO_OrderedQty"] = orderQty;
                ViewState["PO_ReceivedQty"] = receiveQty;
                ViewState["PO_BalanceQty"] = balanceQty;

                txtTotalShortFallPrevious.Text = selectedItem.PO_ShortFall.ToString();
                txtTotalPreReceivedQtyGetting.Text = selectedItem.PO_GettingStock.ToString();

                string VendorCode = selectedItem.VendorCode;

                List<VendorDetails> VendorDataget = dal.GetVendorDetails();
                List<VendorDetails> filteredData2 = VendorDataget.Where(item => item.VendorCode == VendorCode).ToList();

                if (filteredData2 != null && filteredData2.Count > 0)
                {
                    VendorDetails selectedItem2 = filteredData2[0];
                    ddlVendor.SelectedValue = selectedItem2.VendorID.ToString();
                }
                ddlRackType.SelectedValue = selectedItem.RackTypeId.ToString();
                ddlUOM.SelectedValue = selectedItem.UOMId.ToString();
                ddlVAT.SelectedValue = selectedItem.VATID.ToString();
                txtItemName.Text = selectedItem.ItemName.ToString();
                txtHSNCode.Text = selectedItem.HSNCode.ToString();
                txtCurrentStock.Text = selectedItem.InventoryCurrentStock.ToString();
                txtMinStock.Text = selectedItem.InventoryMinStock.ToString();
                ViewState["InventoryID"] = selectedItem.InventoryId;
                ddlRackType_SelectedIndexChanged(sender, e);

                foreach (ListItem item in chkRowSeries.Items)
                {
                    string[] getData = selectedItem.InventoryItemLocationStore.Split(',');
                    bool found = false;

                    foreach (string DataShow in getData)
                    {
                        if (DataShow.Trim() == item.Value)
                        {
                            item.Selected = true;
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        item.Enabled = false;
                    }
                    else
                    {
                        item.Enabled = false;
                    }
                }
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                decimal ReceivedQtyAsInvoice = Convert.ToDecimal(txtReceivedQty.Text);
                decimal ReceivedQtyAsGetting = Convert.ToDecimal(txtReceivedQtyGetting.Text);
                decimal ShortFallQty = ReceivedQtyAsInvoice - ReceivedQtyAsGetting;
                decimal ReceivedQtyCount = ReceivedQtyAsGetting + ShortFallQty;

                if (ReceivedQtyAsInvoice == ReceivedQtyCount)
                {
                    if (ReceivedQtyAsInvoice >= ReceivedQtyAsGetting)
                    {
                        string getPoNo = ddlPoNo.SelectedItem.Text;
                        string getItemId = ddlPoItemCode.SelectedItem.Value;
                        string poItemCode = ddlPoItemCode.SelectedItem.Text;
                        int companyUnitId = Convert.ToInt32(Session["CompanyUnitId"]);

                        List<PurchaseOrder> filteredData111 = dal.GetPurchaseOrderDetailsData();
                        List<PurchaseOrder> filteredData1 = filteredData111.Where(item => item.PONumber == getPoNo && item.ItemCode == poItemCode).ToList();
                        /* List<PurchaseOrder> filteredData1 = filteredData111.Where(item => (item.POStatusID == Convert.ToInt32(6) || item.POStatusID == Convert.ToInt32(15)) && item.PONumber == getPoNo && item.ItemCode == poItemCode).ToList();*/

                        if (filteredData1 != null && filteredData1.Count > 0)
                        {
                            PurchaseOrder selectedItems = filteredData1[0];
                            double PoOrderedQty = Convert.ToDouble(selectedItems.PO_OrderedQty.ToString());
                            double PoReceivedQty = Convert.ToDouble(selectedItems.PO_ReceivedQty.ToString());
                            double PoBalanceQty = Convert.ToDouble(selectedItems.PO_BalanceQty.ToString());
                            double PoShortFall = Convert.ToDouble(selectedItems.PO_ShortFall.ToString());
                            double PoGettingStock = Convert.ToDouble(selectedItems.PO_GettingStock);
                            double ItemReceivedQty = Convert.ToDouble(txtReceivedQty.Text);
                            double TotalItemreceived = PoReceivedQty + ItemReceivedQty;
                            double balanceQty = PoOrderedQty - (PoReceivedQty + ItemReceivedQty);
                            int inventoryID = Convert.ToInt32(ViewState["InventoryID"].ToString());
                            string GstPercentage = "";
                            string BasicAmount = txtBasicPrice.Text;
                            decimal itemQty = Convert.ToDecimal(txtReceivedQtyGetting.Text);
                            decimal totalAmount = 0;
                            decimal totalPerItem = 0;
                            double ItemActualReceivedQty = Convert.ToDouble(txtReceivedQtyGetting.Text);
                            double AcceptQty = Convert.ToDouble(txtAcceptQty.Text);
                            double RejectQty = ItemActualReceivedQty - AcceptQty;
                            decimal ReceiveStockUs = Convert.ToDecimal(PoGettingStock) + Convert.ToDecimal(txtReceivedQtyGetting.Text);
                            decimal TotalShoretFall = Convert.ToDecimal(PoShortFall) + ShortFallQty;
                            int PurchaseOrderID = selectedItems.PurchaseOrderID;
                            int poDetailsID = selectedItems.PODetailsID;

                            //if (AcceptQty > 0 && balanceQty >= 0)
                            if (balanceQty >= 0)
                            {/* TotalAmount = VAT + Basic Price */
                                DataTable dt = _cm.getVAT();
                                List<VATData> response = (from DataRow dr in dt.AsEnumerable()
                                                          select new VATData()
                                                          {
                                                              VATId = Convert.ToInt32(dr["VATID"]),
                                                              VATCode = dr["VATCode"].ToString(),
                                                              // Map other properties
                                                          }).ToList();
                                List<VATData> filteredData = response.Where(item => item.VATId == Convert.ToInt32(ddlVAT.SelectedItem.Value)).ToList();

                                if (filteredData != null && filteredData.Count > 0)
                                {
                                    VATData selectedItem = filteredData[0];
                                    GstPercentage = selectedItem.Percentage.ToString();
                                }
                                else
                                {
                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('VAT Not Found!!!'); setTimeout(function() { window.location = 'ItemTransactionAdd.aspx'; }, 100);</script>");
                                }

                                if (decimal.TryParse(BasicAmount, out decimal basicPrice))
                                {
                                    if (decimal.TryParse(GstPercentage, out decimal gstPercentage))
                                    {
                                        decimal gstAmount = (basicPrice * gstPercentage) / 100;
                                        totalAmount = basicPrice + gstAmount;
                                        totalPerItem = totalAmount * itemQty;
                                    }
                                }
                                decimal roundedTotalAmount = Math.Round(totalPerItem, 2);

                                /* Rack Row Series Num... Checkbox....*/
                                string chkboxSelect = "";
                                for (int i = 0; i < chkRowSeries.Items.Count; i++)
                                {
                                    if (chkRowSeries.Items[i].Selected)
                                    {
                                        if (chkboxSelect == "")
                                        {
                                            chkboxSelect = chkRowSeries.Items[i].Value;
                                        }
                                        else
                                        {
                                            chkboxSelect += "," + chkRowSeries.Items[i].Value;
                                        }
                                    }
                                }
                                string selectedRackNo = chkboxSelect;

                                con.Open();
                                int prevIdentity = dal.GetReseedIdentity("ItemTransactionMaster", "IT_Id");
                                dal.ReseedIdentity("ItemTransactionMaster", prevIdentity);
                                con.Close();

                                string query = @"insert into ItemTransactionMaster 
                                                (IT_PoId, IT_InventoryId, IT_ItemId, IT_VendorId, IT_UOMId, IT_GSTId, IT_IsDelete, IT_isValid,
                                                 IT_RackTypeId, IT_BasicPrice, IT_ReceivedQty, IT_BalanceQty, IT_TotalAmount, IT_BatchNumber, 
                                                 IT_ExpiryDateTime, IT_DeliveryDateTime, IT_CreatedDateTime, IT_RackRowSeriesNo, 
                                                 IT_CreatedEmployeeId, IT_MasterCompanyId,IT_PoNumber,IT_AcceptedQty,IT_RejectedQty,
                                                 IT_PreviousReceivedQty,IT_PreviousAcceptedQty,IT_PreviousRejectedQty,IT_PreviousBalanceQty,IT_Status,IT_OrderedQty,
                                                 IT_ReceivedQtyInvoice, IT_PreviousReceivedQtyInvoice, IT_ShortFall, IT_PreviousShortFall)
                                                output INSERTED.IT_Id values
                                                (@IT_PoId, @IT_InventoryId, @IT_ItemId, @IT_VendorId, @IT_UOMId, @IT_GSTId, @IT_IsDelete,@IT_isValid,
                                                 @IT_RackTypeId, @IT_BasicPrice, @IT_ReceivedQty, @IT_BalanceQty, @IT_TotalAmount, @IT_BatchNumber, 
                                                 @IT_ExpiryDateTime, @IT_DeliveryDateTime, @IT_CreatedDateTime, @IT_RackRowSeriesNo, 
                                                 @IT_CreatedEmployeeId, @IT_MasterCompanyId, @IT_PoNumber,@IT_AcceptedQty,@IT_RejectedQty,
                                                 @IT_PreviousReceivedQty,@IT_PreviousAcceptedQty,@IT_PreviousRejectedQty,@IT_PreviousBalanceQty,@IT_Status,@IT_OrderedQty,
                                                 @IT_ReceivedQtyInvoice, @IT_PreviousReceivedQtyInvoice, @IT_ShortFall, @IT_PreviousShortFall)";

                                using (SqlCommand cmd = new SqlCommand(query, con))
                                {
                                    cmd.Parameters.AddWithValue("@IT_PoId", poDetailsID);
                                    cmd.Parameters.AddWithValue("@IT_InventoryId", inventoryID);
                                    cmd.Parameters.AddWithValue("@IT_ItemId", ddlPoItemCode.SelectedItem.Value);
                                    cmd.Parameters.AddWithValue("@IT_VendorId", ddlVendor.SelectedItem.Value);
                                    cmd.Parameters.AddWithValue("@IT_UOMId", ddlUOM.SelectedItem.Value);
                                    cmd.Parameters.AddWithValue("@IT_GSTId", ddlVAT.SelectedItem.Value);
                                    cmd.Parameters.AddWithValue("@IT_RackTypeId", ddlRackType.SelectedItem.Value);
                                    cmd.Parameters.AddWithValue("@IT_BasicPrice", Convert.ToDecimal(txtBasicPrice.Text));
                                    cmd.Parameters.AddWithValue("@IT_BalanceQty", balanceQty);
                                    cmd.Parameters.AddWithValue("@IT_TotalAmount", roundedTotalAmount);
                                    cmd.Parameters.AddWithValue("@IT_BatchNumber", txtBatchNo.Text);
                                    cmd.Parameters.AddWithValue("@IT_ExpiryDateTime", txtExpiryDate.Text);
                                    cmd.Parameters.AddWithValue("@IT_DeliveryDateTime", txtDeliveryDateTime.Text);
                                    cmd.Parameters.AddWithValue("@IT_MasterCompanyId", Session["MasterCompanyId"].ToString());
                                    cmd.Parameters.AddWithValue("@IT_CreatedEmployeeId", Session["EmployeeID"].ToString());
                                    cmd.Parameters.AddWithValue("@IT_RackRowSeriesNo", chkboxSelect);
                                    cmd.Parameters.AddWithValue("@IT_CreatedDateTime", GetDateTime);
                                    cmd.Parameters.AddWithValue("@IT_PoNumber", ddlPoNo.SelectedItem.Text);
                                    cmd.Parameters.AddWithValue("@IT_Status", 11);
                                    cmd.Parameters.AddWithValue("@IT_OrderedQty", Convert.ToDecimal(PoOrderedQty));
                                    cmd.Parameters.AddWithValue("@IT_IsDelete", 0);
                                    cmd.Parameters.AddWithValue("@IT_AcceptedQty", AcceptQty);
                                    cmd.Parameters.AddWithValue("@IT_RejectedQty", RejectQty);
                                    cmd.Parameters.AddWithValue("@IT_PreviousAcceptedQty", Convert.ToDecimal(txtPreAcceptQty.Text));
                                    cmd.Parameters.AddWithValue("@IT_PreviousRejectedQty", Convert.ToDecimal(txtPreRejectedQty.Text));
                                    cmd.Parameters.AddWithValue("@IT_PreviousBalanceQty", Convert.ToDecimal(txtPreBalanceQty.Text));
                                    cmd.Parameters.AddWithValue("@IT_ReceivedQty", Convert.ToDecimal(txtReceivedQtyGetting.Text));
                                    cmd.Parameters.AddWithValue("@IT_PreviousReceivedQty", Convert.ToDecimal(txtTotalPreReceivedQtyGetting.Text));
                                    cmd.Parameters.AddWithValue("@IT_ReceivedQtyInvoice", Convert.ToDecimal(txtReceivedQty.Text));
                                    cmd.Parameters.AddWithValue("@IT_PreviousReceivedQtyInvoice", Convert.ToDecimal(txtPreReceivedQty.Text));
                                    cmd.Parameters.AddWithValue("@IT_ShortFall", Convert.ToDecimal(ShortFallQty));
                                    cmd.Parameters.AddWithValue("@IT_PreviousShortFall", Convert.ToDecimal(txtTotalShortFallPrevious.Text));
                                    //cmd.Parameters.AddWithValue("@IT_ProjectID", txtProjectId.Text);
                                    //cmd.Parameters.AddWithValue("@IT_IgirNumber", ddlIGIRNo.SelectedItem.Text);
                                    cmd.Parameters.AddWithValue("@IT_isValid", 0);

                                    con.Open();
                                    int InsertGetId = (int)cmd.ExecuteScalar();
                                    con.Close();
                                    if (InsertGetId > 0)
                                    {
                                        string QRNo = InsertGetId.ToString();
                                        /* Barcode generation and saving */
                                        string barcodeFolderPath = Server.MapPath("~/QRCodeSave/");
                                        if (!Directory.Exists(barcodeFolderPath))
                                        {
                                            Directory.CreateDirectory(barcodeFolderPath);
                                        }
                                        /* Rest of your barcode generation code remains the same */
                                        BarcodeWriter barcodeWriter = new BarcodeWriter();
                                        /* Choose a barcode format */
                                        barcodeWriter.Format = BarcodeFormat.QR_CODE;
                                        Bitmap barcodeBitmap = barcodeWriter.Write(QRNo);
                                        /* Save the barcode image to the folder */
                                        string barcodeFileName = $"{QRNo}_QR.png";
                                        string barcodeFilePath = System.IO.Path.Combine(barcodeFolderPath, barcodeFileName);
                                        barcodeBitmap.Save(barcodeFilePath);

                                        /* QR Code Save to the DAtabase */
                                        string query1 = @"update ItemTransactionMaster set IT_QRCodeNumber = @IT_Id, IT_QRFileName = @IT_QRFileName WHERE IT_Id = @IT_Id";
                                        using (SqlCommand cmd1 = new SqlCommand(query1, con))
                                        {
                                            cmd1.Parameters.AddWithValue("@IT_Id", InsertGetId);
                                            cmd1.Parameters.AddWithValue("@IT_QRFileName", barcodeFileName);

                                            con.Open();
                                            int a = cmd1.ExecuteNonQuery();
                                            con.Close();

                                            var TableRowId = InsertGetId.ToString();
                                            bool success = dal.AuditLogData("Item Transaction Add with QR Generate", "ItemTransactionMaster", TableRowId, "");
                                            if (success)
                                            {
                                                using (SqlConnection con2 = new SqlConnection(constr))
                                                {
                                                    double poReceivedQty = Convert.ToDouble(txtReceivedQty.Text);

                                                    string str1 = @"UPDATE PurchaseOrderDetails set POStatusID=@POStatusID, PO_StatusId = @PO_StatusId, PO_ReceivedQty = @PO_ReceivedQty,
                                                                    PO_BalanceQty = @PO_BalanceQty, PO_StatusForIT = @PO_StatusForIT,
                                                                    PO_GettingStock = @PO_GettingStock, PO_ShortFall = @PO_ShortFall
                                                                    WHERE PODetailsID = @PODetailsID";

                                                    SqlCommand cmd2 = new SqlCommand(str1, con2);
                                                    cmd2.Parameters.AddWithValue("@PODetailsID", poDetailsID);
                                                    //cmd2.Parameters.AddWithValue("@PO_Number", ddlPoNo.SelectedItem.Text);
                                                    cmd2.Parameters.AddWithValue("@POStatusID", 11);
                                                    cmd2.Parameters.AddWithValue("@PO_StatusId", 11);
                                                    cmd2.Parameters.AddWithValue("@PO_StatusForIT", 1);
                                                    cmd2.Parameters.AddWithValue("@PO_ReceivedQty", Convert.ToDecimal(TotalItemreceived));
                                                    cmd2.Parameters.AddWithValue("@PO_BalanceQty", Convert.ToDecimal(balanceQty));
                                                    cmd2.Parameters.AddWithValue("@PO_GettingStock", ReceiveStockUs);
                                                    cmd2.Parameters.AddWithValue("@PO_ShortFall", TotalShoretFall);

                                                    con2.Open();
                                                    int j = cmd2.ExecuteNonQuery();
                                                    con2.Close();

                                                    if (j > 0)
                                                    {
                                                        var TableRowId1 = poDetailsID.ToString();
                                                        bool success1 = dal.AuditLogData("Po Details Update when Item Transaction Add", "PurchaseOrderDetails", TableRowId1, "");
                                                        if (success1)
                                                        {
                                                            using (SqlConnection con3 = new SqlConnection(constr))
                                                            {

                                                                string str3 = @"UPDATE PurchaseOrder set PurchaseOrderStatus = @PurchaseOrderStatus WHERE PurchaseOrderID = @PurchaseOrderID";

                                                                SqlCommand cmd3 = new SqlCommand(str3, con3);
                                                                cmd3.Parameters.AddWithValue("@PurchaseOrderID", PurchaseOrderID);
                                                                cmd3.Parameters.AddWithValue("@PurchaseOrderStatus", 11);
                                                                

                                                                con3.Open();
                                                                int k = cmd3.ExecuteNonQuery();
                                                                con3.Close();

                                                                if (k > 0)
                                                                {
                                                                    var TableRowId3 = PurchaseOrderID.ToString();
                                                                    bool success3 = dal.AuditLogData("Po Data Update when Item Transaction Add", "PurchaseOrder", TableRowId3, "");
                                                                    if (success3)
                                                                    {
                                                                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('ITEM ADD Successfully for GRN'); setTimeout(function() { window.location = 'ItemTransactionAdd.aspx'; }, 100);</script>");
                                                                    }
                                                                    else
                                                                    {
                                                                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed for ItemTransactionMaster!.'); </script>");
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert(' something went wrong try later! '); </script>");
                                                                }
                                                            }
                                                            //Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('ITEM ADD Successfully for GRN'); setTimeout(function() { window.location = 'ItemTransactionAdd.aspx'; }, 100);</script>");
                                                        }
                                                        else
                                                        {
                                                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed for ItemTransactionMaster!.'); </script>");
                                                        }
                                                        //Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('ADD Successfully'); setTimeout(function() { window.location = 'ItemTransactionAdd.aspx'; }, 100);</script>");
                                                    }
                                                    else
                                                    {
                                                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert(' something went wrong try later! '); </script>");
                                                    }
                                                }

                                            }
                                            else
                                            {
                                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed for ItemTransactionMaster!.'); </script>");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('something went wrong try later!');</script>");
                                    }
                                }
                            }
                            else
                            {
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Data is Not Valid');</script>");
                            }
                        }
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('previous PO complete first!');</script>");
                        }
                    }
                    else
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert(' Not Valid '); </script>");
                    }
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert(' Received Qty and Shorefall not match with ReceivedQty as Invoice '); </script>");
                }

            }
            catch (Exception ex)
            {
                if (!(ex is System.Threading.ThreadAbortException))
                {
                    int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                    var s = new StackTrace(ex);
                    var methodName = s.GetFrames().Select(f => f.GetMethod()).First(m => m.Module.Assembly == Assembly.GetExecutingAssembly()).Name;
                    string PageName = Path.GetFileName(Request.Url.AbsolutePath);

                    bool success = dal.ErrorLogData(methodName, ex.Message, PageName + ".cs", linenum.ToString());
                    if (success)
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error Log Data Saved: " + ex.Message.Replace("'", "\\'") + "'); </script>");
                    }
                    else
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: " + ex.Message.Replace("'", "\\'") + "'); </script>");
                    }
                }
            }
        }


        [System.Web.Services.WebMethod]
        public static string UpdateBalanceQty(string receivedQty, string poOrderedQty, string poReceivedQty)
        {
            double receivedQuantity;
            double orderedQuantity;
            double receivedQuantityInPO;

            if (!double.TryParse(receivedQty, out receivedQuantity) || !double.TryParse(poOrderedQty, out orderedQuantity) || !double.TryParse(poReceivedQty, out receivedQuantityInPO))
            {
                return "Enter Valid Number!";
            }
            else
            {
                double PoOrderedQty = Convert.ToDouble(poOrderedQty);
                double PoReceivedQty = Convert.ToDouble(poReceivedQty);
                double ItemReceivedQty = Convert.ToDouble(receivedQty);

                if (PoOrderedQty >= ItemReceivedQty)
                {
                    double balanceQty = PoOrderedQty - (PoReceivedQty + ItemReceivedQty);
                    return balanceQty.ToString();
                }
                else
                {
                    return "Invalid Data";
                }
            }
        }


        [System.Web.Services.WebMethod]
        public static string UpdateShortFall(string receivedQty, string receivedQtyWE)
        {
            double receivedQuantity;
            double receivedQuantityWE;

            if (!double.TryParse(receivedQty, out receivedQuantity) || !double.TryParse(receivedQtyWE, out receivedQuantityWE))
            {
                return "Enter Valid Number!";
            }
            else
            {
                double ReceivedQtyAsInvoice = Convert.ToDouble(receivedQty);
                double ReceivedQtyAsOur = Convert.ToDouble(receivedQtyWE);

                if (ReceivedQtyAsInvoice >= ReceivedQtyAsOur)
                {
                    double shortFall = ReceivedQtyAsInvoice - ReceivedQtyAsOur;
                    return shortFall.ToString();
                }
                else
                {
                    return "Invalid Data";
                }
            }
        }


        [System.Web.Services.WebMethod]
        public static string AcceptUpdateQty(string acceptQty, string poOrderedQty, string poReceivedQty)
        {
            double acceptQuantity;
            double orderedQuantity;
            double receivedQuantityInPO;

            if (!double.TryParse(acceptQty, out acceptQuantity) || !double.TryParse(poOrderedQty, out orderedQuantity) || !double.TryParse(poReceivedQty, out receivedQuantityInPO))
            {
                return "Enter Valid Number!";
            }
            else
            {
                double PoOrderedQty = Convert.ToDouble(poOrderedQty);
                double PoReceiveQty = Convert.ToDouble(poReceivedQty);
                double ItemAcceptQty = Convert.ToDouble(acceptQty);

                if (PoOrderedQty >= ItemAcceptQty)
                {
                    double rejectQty = PoReceiveQty - ItemAcceptQty;
                    return rejectQty.ToString();
                }
                else
                {
                    return "Invalid Data";
                }
            }
        }



    }
}