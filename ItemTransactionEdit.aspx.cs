using LIMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class ItemTransactionEdit : Page
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

                    txtBalanceQty.Text = hdnBalanceQty.Value;

                    ViewState["GetBQty"] = txtBalanceQty.Text;


                    //ddlBranchUnit.Enabled = false;
                    ddlRackType.Enabled = false;
                    ddlUOM.Enabled = false;
                    ddlVAT.Enabled = false;
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
                    /*txtShortFallPrevious.Enabled = false;*/
                    txtShortFall.Enabled = false;
                    txtTotalShortFallPrevious.Enabled = false;
                    txtTotalPreReceivedQtyGetting.Enabled = false;

                    try
                    {
                        if (Request.QueryString["PageID"] != null)
                        {
                            byte[] decodedBytes = Convert.FromBase64String(Request.QueryString["PageID"]);
                            string decodedEventListId = Encoding.UTF8.GetString(decodedBytes);
                            lblPageID.Text = decodedEventListId;
                            lblPageID.Visible = false;

                            ddlPoItemCode.Enabled = false;
                            ddlPoNo.Enabled = false;


                            if (roleId != "1")
                            {
                                /* PO Get From the Excel */
                                List<PurchaseOrder> PoNumbersGet = dal.GetPurchaseOrdersData();
                                ddlPoNo.DataSource = PoNumbersGet;
                                ddlPoNo.DataTextField = "PONumber";
                                ddlPoNo.DataBind();
                                ddlPoNo.Items.Insert(0, new ListItem("Select PO-Number", ""));

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

                                List<PurchaseOrder> response = dal.GetPurchaseOrderDetailsData();
                                ddlPoItemCode.DataSource = response;
                                ddlPoItemCode.DataTextField = "ItemCode";
                                ddlPoItemCode.DataValueField = "ItemId";
                                ddlPoItemCode.DataBind();
                                ddlPoItemCode.Items.Insert(0, new ListItem("--Item Code--", ""));

                                //int companyUnitId = Convert.ToInt32(Session["CompanyUnitId"]);
                                int itInventoryId = 0;
                                int poID = 0;

                                List<ItemTransactionData> responseData = dal.ItemTransactionDataGet();
                                List<ItemTransactionData> filteredData2 = responseData.Where(item => item.IT_Id == Convert.ToInt32(lblPageID.Text)).ToList();

                                if (filteredData2 != null && filteredData2.Count > 0)
                                {
                                    ItemTransactionData selectedItem = filteredData2[0];
                                    itInventoryId = selectedItem.IT_InventoryId;
                                    poID = selectedItem.IT_PoId;
                                    ddlPoNo.SelectedValue = selectedItem.IT_PoNumber.ToString();
                                    ddlVendor.SelectedValue = selectedItem.IT_VendorId.ToString();
                                    txtBalanceQty.Text = selectedItem.IT_BalanceQty.ToString();
                                    txtPreBalanceQty.Text = selectedItem.IT_PreviousBalanceQty.ToString();
                                    txtPreAcceptQty.Text = selectedItem.IT_PreviousAcceptedQty.ToString();
                                    txtPreRejectedQty.Text = selectedItem.IT_PreviousRejectedQty.ToString();
                                    txtDeliveryDateTime.Text = selectedItem.IT_DeliveryDateTime.ToString();
                                    txtExpiryDate.Text = selectedItem.IT_ExpiryDateTime.ToString();
                                    txtBatchNo.Text = selectedItem.IT_BatchNumber.ToString();
                                    txtBasicPrice.Text = selectedItem.IT_BasicPrice.ToString();
                                    txtReceivedQty.Text = selectedItem.IT_ReceivedQtyInvoice.ToString();
                                    txtPreReceivedQty.Text = selectedItem.IT_PreviousReceivedQtyInvoice.ToString();
                                    txtAcceptQty.Text = selectedItem.IT_AcceptedQty.ToString();
                                    txtRejectQty.Text = selectedItem.IT_RejectedQty.ToString();
                                    txtReceivedQtyGetting.Text = selectedItem.IT_ReceivedQty.ToString();
                                    txtShortFall.Text = selectedItem.IT_ShortFall.ToString();
                                    txtTotalShortFallPrevious.Text = selectedItem.IT_ShortFall.ToString();
                                    txtTotalPreReceivedQtyGetting.Text = selectedItem.IT_PreviousReceivedQty.ToString();

                                    int ITstatusId = selectedItem.IT_Status;

                                    if (ITstatusId != 11)
                                    {
                                        btnUpdate.Enabled = false;
                                        lblMessage.Text = "GRN was Generated,so you can not Edit these Data!";
                                    }
                                    else
                                    {
                                        lblMessage.Text = "";
                                    }

                                    List<PurchaseOrder> resfe1 = dal.GetPurchaseOrderDetailsData();
                                    List<PurchaseOrder> filteredData = resfe1.Where(item => item.PODetailsID == poID).ToList();
                                    if (filteredData != null && filteredData.Count > 0)
                                    {
                                        PurchaseOrder selectedItemsGet = filteredData[0];

                                        GridView1.DataSource = filteredData;
                                        GridView1.DataBind();

                                        ddlPoItemCode.SelectedValue = selectedItemsGet.ItemID.ToString();
                                        double orderQty = Convert.ToDouble(selectedItemsGet.PO_OrderedQty.ToString());
                                        double receiveQty = Convert.ToDouble(selectedItemsGet.PO_ReceivedQty.ToString());
                                        double balanceQty = Convert.ToDouble(selectedItemsGet.PO_BalanceQty.ToString());

                                        lblorderQty.Text = orderQty.ToString();
                                        lblreceiveQty.Text = receiveQty.ToString();
                                        lblbalanceQty.Text = balanceQty.ToString();
                                        lblPoNo.Text = selectedItemsGet.PODetailsID.ToString();

                                        ViewState["PO_OrderedQty"] = orderQty;
                                        ViewState["PO_ReceivedQty"] = receiveQty;
                                        ViewState["PO_BalanceQty"] = balanceQty;


                                        ddlRackType.SelectedValue = selectedItemsGet.RackTypeId.ToString();
                                        ddlUOM.SelectedValue = selectedItemsGet.UOMId.ToString();
                                        ddlVAT.SelectedValue = selectedItemsGet.VATID.ToString();
                                        txtItemName.Text = selectedItemsGet.ItemName.ToString();
                                        txtHSNCode.Text = selectedItemsGet.HSNCode.ToString();
                                        txtCurrentStock.Text = selectedItemsGet.InventoryCurrentStock.ToString();
                                        txtMinStock.Text = selectedItemsGet.InventoryMinStock.ToString();
                                        ViewState["InventoryID"] = selectedItemsGet.InventoryId;
                                        ddlRackType_SelectedIndexChanged(sender, e);

                                        foreach (ListItem item in chkRowSeries.Items)
                                        {
                                            string[] getData = selectedItemsGet.InventoryItemLocationStore.Split(',');
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
                                    else
                                    {
                                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Data Not Found.'); </script>");
                                    }
                                }
                                else
                                {
                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('ALL Records Not Found.'); </script>");
                                }
                            }
                            else
                            {
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Company Admin can not access this'); setTimeout(function() { window.location = 'Dashboard.aspx'; }, 100); </script>");
                            }
                        }
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Edit ID Not Getting'); setTimeout(function() { window.location = 'Dashboard.aspx'; }, 100); </script>");
                        }

                    }
                    catch (Exception ex)
                    {
                        int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                        var s = new StackTrace(ex);
                        var methodName = s.GetFrames().Select(f => f.GetMethod()).First(m => m.Module.Assembly == Assembly.GetExecutingAssembly()).Name;
                        /* ErrorLogData(string methodName, string errorMessage, string errorDescription, string errorLineNumber) */
                        bool success = dal.ErrorLogData(methodName, ex.Message, "ItemTransactionEdit.aspx.cs", linenum.ToString());
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
            }
        }

        protected void ddlRackType_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedUnitId;
            int selectedRackTypeId;

            if (int.TryParse(ddlRackType.SelectedValue, out selectedRackTypeId))
            {
                BindRackRowSeriesNumbers(selectedRackTypeId);
            }
        }

        private void BindRackRowSeriesNumbers(int selectedRackTypeId)
        {
            List<RackNumberData> allRackNumbers = dal.AllRackRowSerialNumberDisplay();
            List<RackNumberData> filteredRackNumbers = allRackNumbers
                .Where(item => item.RackTypeId == selectedRackTypeId).ToList();

            chkRowSeries.DataSource = filteredRackNumbers;
            chkRowSeries.DataTextField = "RackSerialNumber";
            chkRowSeries.DataValueField = "RackSerialId";
            chkRowSeries.DataBind();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                int inventoryID = Convert.ToInt32(ViewState["InventoryID"].ToString());
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

                        List<PurchaseOrder> responseData = dal.GetPurchaseOrderDetailsData();
                        List<PurchaseOrder> filteredData1 = responseData.Where(item => item.PONumber == getPoNo && item.ItemCode == poItemCode).ToList();

                        if (filteredData1 != null && filteredData1.Count > 0)
                        {
                            PurchaseOrder selectedItems = filteredData1[0];
                            decimal PoOrderedStock = Convert.ToDecimal(selectedItems.PO_OrderedQty.ToString());
                            decimal PoReceiveStock = Convert.ToDecimal(selectedItems.PO_ReceivedQty);
                            decimal PoShortFall = Convert.ToDecimal(selectedItems.PO_ShortFall.ToString());
                            decimal PoGettingStock = Convert.ToDecimal(selectedItems.PO_GettingStock);
                            decimal PoBalanceStock = Convert.ToDecimal(selectedItems.PO_BalanceQty);
                            decimal TotalItemreceived = PoReceiveStock + ReceivedQtyAsInvoice;
                            decimal balanceQty = PoOrderedStock - (PoReceiveStock + ReceivedQtyAsInvoice);                            
                            string GstPercentage = "";
                            string BasicAmount = txtBasicPrice.Text;
                            decimal totalAmount = 0;
                            decimal totalPerItem = 0;

                            if (ReceivedQtyAsInvoice > 0 && balanceQty >= 0)
                            { /* TotalAmount = TAX + Basic Price */
                                List<ItemTransactionData> responseData2 = dal.ItemTransactionDataGet();
                                List<ItemTransactionData> filteredData2 = responseData2.Where(item => item.IT_Id == Convert.ToInt32(lblPageID.Text)).ToList();

                                if (filteredData2 != null && filteredData2.Count > 0)
                                {
                                    ItemTransactionData selectedItem1 = filteredData2[0];
                                    decimal ReceiveStock = selectedItem1.IT_ReceivedQtyInvoice;
                                    decimal GettingStock = selectedItem1.IT_ReceivedQty;
                                    decimal shortFallStock = selectedItem1.IT_ShortFall;
                                    decimal acceptStock = selectedItem1.IT_AcceptedQty;
                                    decimal rejectStock = selectedItem1.IT_RejectedQty;
                                    decimal acceptQty = Convert.ToDecimal(txtAcceptQty.Text);
                                    decimal AcceptQtydiff = 0;
                                    decimal AcceptQtyfinal = 0;
                                    decimal rejectQty = 0;

                                    if (acceptQty == acceptStock)
                                    {
                                        AcceptQtyfinal = acceptStock;
                                    }
                                    else if (acceptQty > acceptStock)
                                    {
                                        AcceptQtydiff = acceptQty - acceptStock;
                                        AcceptQtyfinal = acceptStock + AcceptQtydiff;
                                    }
                                    else if (acceptStock > acceptQty)
                                    {
                                        AcceptQtydiff = acceptStock - acceptQty;
                                        AcceptQtyfinal = acceptStock - AcceptQtydiff;
                                    }
                                    rejectQty = ReceivedQtyAsGetting - AcceptQtyfinal;
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
                                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Something Wrong TAX Not Get!!!'); setTimeout(function() { window.location = 'ItemTransactionAdd.aspx'; }, 100);</script>");
                                    }

                                    if (decimal.TryParse(BasicAmount, out decimal basicPrice))
                                    {
                                        if (decimal.TryParse(GstPercentage, out decimal gstPercentage))
                                        {
                                            decimal gstAmount = (basicPrice * gstPercentage) / 100;
                                            totalAmount = basicPrice + gstAmount;
                                            totalPerItem = totalAmount * ReceivedQtyAsGetting;
                                        }
                                    }
                                    decimal roundedTotalAmount = Math.Round(totalPerItem, 2);

                                    /* Received Stock as invoice Calculate*/
                                    decimal DiffCount = 0;
                                    decimal TotalItemReceived = 0;
                                    decimal BalanceStock = 0;

                                    if (ReceiveStock > ReceivedQtyAsInvoice)
                                    {
                                        DiffCount = ReceiveStock - ReceivedQtyAsInvoice;
                                        TotalItemReceived = PoReceiveStock - DiffCount;
                                        BalanceStock = PoOrderedStock - (TotalItemReceived);
                                    }
                                    else if (ReceivedQtyAsInvoice > ReceiveStock)
                                    {
                                        DiffCount = ReceivedQtyAsInvoice - ReceiveStock;
                                        TotalItemReceived = PoReceiveStock + DiffCount;
                                        BalanceStock = PoOrderedStock - (TotalItemReceived);
                                    }
                                    else if (ReceivedQtyAsInvoice == ReceiveStock)
                                    {
                                        TotalItemReceived = PoReceiveStock;
                                        BalanceStock = PoBalanceStock;
                                    }

                                    /* Getting Stock Calculation */
                                    decimal DiffCountForGetting = 0;
                                    decimal TotalItemReceivedForGetting = 0;

                                    if (GettingStock > ReceivedQtyAsGetting)
                                    {
                                        DiffCountForGetting = GettingStock - ReceivedQtyAsGetting;
                                        TotalItemReceivedForGetting = PoGettingStock - DiffCountForGetting;
                                    }
                                    else if (ReceivedQtyAsGetting > GettingStock)
                                    {
                                        DiffCountForGetting = ReceivedQtyAsGetting - GettingStock;
                                        TotalItemReceivedForGetting = PoGettingStock + DiffCountForGetting;
                                    }
                                    else if (ReceivedQtyAsGetting == GettingStock)
                                    {
                                        TotalItemReceivedForGetting = PoGettingStock;
                                    }

                                    /* ShortFallQty Calculate */
                                    decimal DiffCountForShortFall = 0;
                                    decimal TotalShortFall = 0;

                                    if (shortFallStock > ShortFallQty)
                                    {
                                        DiffCountForShortFall = shortFallStock - ShortFallQty;
                                        TotalShortFall = PoShortFall - DiffCountForShortFall;
                                    }
                                    else if (ShortFallQty > shortFallStock)
                                    {
                                        DiffCountForShortFall = ShortFallQty - shortFallStock;
                                        TotalShortFall = PoShortFall + DiffCountForShortFall;
                                    }
                                    else if (ShortFallQty == shortFallStock)
                                    {
                                        TotalShortFall = PoShortFall;
                                    }

                                    string query = @"update ItemTransactionMaster set 
                                                     IT_ReceivedQtyInvoice = @IT_ReceivedQtyInvoice, IT_ReceivedQty = @IT_ReceivedQty,
                                                     IT_AcceptedQty = @IT_AcceptedQty,IT_RejectedQty=@IT_RejectedQty, IT_ShortFall = @IT_ShortFall,
                                                     IT_BalanceQty = @IT_BalanceQty, 
                                                     IT_DeliveryDateTime = @IT_DeliveryDateTime, IT_ExpiryDateTime = @IT_ExpiryDateTime, 
                                                     IT_TotalAmount = @IT_TotalAmount, IT_VendorId = @IT_VendorId,
                                                     IT_MasterCompanyId = @IT_MasterCompanyId, IT_CreatedEmployeeId = @IT_CreatedEmployeeId,
                                                     IT_UpdatedDateTime = @IT_UpdatedDateTime                                     
                                                     WHERE IT_Id = @IT_Id";

                                    using (SqlCommand cmd = new SqlCommand(query, con))
                                    {
                                        cmd.Parameters.AddWithValue("@IT_ReceivedQtyInvoice", ReceivedQtyAsInvoice);
                                        cmd.Parameters.AddWithValue("@IT_ReceivedQty", ReceivedQtyAsGetting);
                                        cmd.Parameters.AddWithValue("@IT_AcceptedQty", AcceptQtyfinal);
                                        cmd.Parameters.AddWithValue("@IT_RejectedQty", rejectQty);
                                        cmd.Parameters.AddWithValue("@IT_ShortFall", ShortFallQty);
                                        cmd.Parameters.AddWithValue("@IT_BalanceQty", BalanceStock);
                                        cmd.Parameters.AddWithValue("@IT_DeliveryDateTime", txtDeliveryDateTime.Text);
                                        cmd.Parameters.AddWithValue("@IT_ExpiryDateTime", txtExpiryDate.Text);
                                        cmd.Parameters.AddWithValue("@IT_TotalAmount", roundedTotalAmount);
                                        cmd.Parameters.AddWithValue("@IT_VendorId", ddlVendor.SelectedItem.Value);
                                        cmd.Parameters.AddWithValue("@IT_MasterCompanyId", Session["MasterCompanyId"].ToString());
                                        cmd.Parameters.AddWithValue("@IT_CreatedEmployeeId", Session["EmployeeID"].ToString());                                        
                                        cmd.Parameters.AddWithValue("@IT_UpdatedDateTime", GetDateTime);
                                        cmd.Parameters.AddWithValue("@IT_Id", lblPageID.Text);

                                        con.Open();
                                        int a = cmd.ExecuteNonQuery();
                                        con.Close();

                                        if (a > 0)
                                        {
                                            var TableRowId = lblPageID.Text;
                                            bool success = dal.AuditLogData("Item Transaction Edit", "ItemTransactionMaster", TableRowId, "");
                                            if (success)
                                            { }
                                            else
                                            {
                                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed for ItemTransactionMaster!.'); </script>");
                                            }

                                            using (SqlConnection con2 = new SqlConnection(constr))
                                            {
                                                string str1 = @"UPDATE PurchaseOrderDetails set
                                                                PO_ReceivedQty = @PO_ReceivedQty,PO_BalanceQty = @PO_BalanceQty,
                                                                PO_GettingStock = @PO_GettingStock, PO_ShortFall = @PO_ShortFall                                                                
                                                                WHERE PODetailsID = @PODetailsID";

                                                SqlCommand cmd2 = new SqlCommand(str1, con2);
                                                cmd2.Parameters.AddWithValue("@PODetailsID", lblPoNo.Text);                                                
                                                cmd2.Parameters.AddWithValue("@PO_ReceivedQty", TotalItemReceived);
                                                cmd2.Parameters.AddWithValue("@PO_BalanceQty", BalanceStock);
                                                cmd2.Parameters.AddWithValue("@PO_GettingStock", TotalItemReceivedForGetting);
                                                cmd2.Parameters.AddWithValue("@PO_ShortFall", TotalShortFall);
                                                
                                                con2.Open();
                                                int j = cmd2.ExecuteNonQuery();
                                                con2.Close();

                                                if (j > 0)
                                                {
                                                    var TableRowId1 = lblPoNo.Text;
                                                    bool success1 = dal.AuditLogData("Po Details Update when Item Transaction Edit", "PurchaseOrderDetails", TableRowId1, "");
                                                    if (success1)
                                                    { }
                                                    else
                                                    {
                                                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed for ItemTransactionMaster!.'); </script>");
                                                    }
                                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Update Successfully'); setTimeout(function() { window.location = 'ItemTransactionShow.aspx'; }, 100);</script>");
                                                }
                                                else
                                                {
                                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Item-Transaction Updated but PO Not Updated!'); setTimeout(function() { window.location = 'ItemTransactionShow.aspx'; }, 100);</script>");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('something went wrong try later!');</script>");
                                        }
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
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Data is Not get of PO');</script>");
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
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                var s = new StackTrace(ex);
                var methodName = s.GetFrames().Select(f => f.GetMethod()).First(m => m.Module.Assembly == Assembly.GetExecutingAssembly()).Name;
                /* ErrorLogData(string methodName, string errorMessage, string errorDescription, string errorLineNumber) */
                bool success = dal.ErrorLogData(methodName, ex.Message, "ItemTransactionEdit.aspx.cs", linenum.ToString());
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