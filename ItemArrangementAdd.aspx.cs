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
using AjaxControlToolkit;
using LIMS.Models;

namespace LIMS
{
    public partial class ItemArrangementAdd : System.Web.UI.Page
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
                        txtReceivedQty.Enabled = false;
                        txtBalanceQty.Enabled = false;
                        txtRejectQty.Enabled = false;
                        txtCurrentStock.Enabled = false;
                        txtMinStock.Enabled = false;
                        txtShortFall.Enabled = false;
                        txtTotalShortFallPrevious.Enabled = false;
                        txtReceivedQtyAsInvoice.Enabled = false;
                        txtPreReceivedQtyAsInvoice.Enabled = false;
                        txtAcceptQty.Enabled = false;

                        int companyUnitId = Convert.ToInt32(Session["CompanyUnitId"]);

                        List<GRNMasterData> response = dal.GRNDetailsShowNew();
                        List<GRNMasterData> filteredData = response.Where(item => item.GRN_CompanyUnitId == companyUnitId && item.GDStatusId == 13).ToList();
                        List<GRNMasterData> allFilteredGRNNumbers = new List<GRNMasterData>();

                        if (filteredData != null && filteredData.Count > 0)
                        {
                            foreach (var item in filteredData)
                            {
                                string GRN_Num = item.GRN_NUMBER.ToString();

                                List<GRNMasterData> getGRNMD = dal.GRNMasterDataShow();
                                List<GRNMasterData> filteredGRNNumbers = getGRNMD.Where(grnItem => grnItem.GRN_NUMBER == GRN_Num).ToList();
                                allFilteredGRNNumbers.AddRange(filteredGRNNumbers);
                            }

                            /* Get distinct GRN_NUMBER values */
                            var distinctGRNNumbers = allFilteredGRNNumbers
                                .GroupBy(grn => grn.GRN_NUMBER)
                                .Select(group => group.First())
                                .ToList();

                            /* Bind distinct GRN_NUMBER values to the dropdown */
                            ddlGRNNumber.DataSource = distinctGRNNumbers;
                            ddlGRNNumber.DataTextField = "GRN_NUMBER";
                            ddlGRNNumber.DataValueField = "GRN_ID";
                            ddlGRNNumber.DataBind();
                            ddlGRNNumber.Items.Insert(0, new ListItem("Select", ""));
                        }
                        else
                        {

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
                    else
                    {

                    }
                }

            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                var s = new StackTrace(ex);
                var methodName = s.GetFrames().Select(f => f.GetMethod()).First(m => m.Module.Assembly == Assembly.GetExecutingAssembly()).Name;
                string PageName = System.IO.Path.GetFileName(Request.Url.AbsolutePath);
                /* ErrorLogData(string methodName, string errorMessage, string errorDescription, string errorLineNumber) */
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

        protected void ddlGRNNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string GRN_No = ddlGRNNumber.SelectedItem.Text;

                List<GRNMasterData> response = dal.GRNDetailsShowNew();
                List<GRNMasterData> filteredData = response.Where(item => item.GRN_NUMBER == GRN_No && (item.GDStatusId == 13)).ToList();
                if (filteredData != null && filteredData.Count > 0)
                {
                    GRNMasterData selectedItem = filteredData[0];
                    string PONumber = selectedItem.GRN_PoNumber.ToString();
                    ViewState["PO_Number"] = PONumber;
                    ddlPoItemCode.DataSource = filteredData;
                    ddlPoItemCode.DataTextField = "GDPoItemCode";
                    ddlPoItemCode.DataValueField = "GDITId";
                    ddlPoItemCode.DataBind();
                    ddlPoItemCode.Items.Insert(0, new ListItem("Select", ""));
                    GridView1.DataSource = filteredData;
                    GridView1.DataBind();
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No Data Found!'); setTimeout(function() { window.location = 'ItemArrangementAdd.aspx'; }, 100);</script>");
                }
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                var s = new StackTrace(ex);
                var methodName = s.GetFrames().Select(f => f.GetMethod()).First(m => m.Module.Assembly == Assembly.GetExecutingAssembly()).Name;
                string PageName = System.IO.Path.GetFileName(Request.Url.AbsolutePath);
                /* ErrorLogData(string methodName, string errorMessage, string errorDescription, string errorLineNumber) */
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

        protected void ddlPoItemCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlPoItemCode.SelectedIndex != 0)
                {
                    string poItemCode = ddlPoItemCode.SelectedItem.Text;
                    //int companyUnitId = Convert.ToInt32(Session["CompanyUnitId"]);
                    int GRNID = Convert.ToInt32(ddlGRNNumber.SelectedItem.Value);
                    int ITID = Convert.ToInt32(ddlPoItemCode.SelectedItem.Value);

                    List<ItemTransactionData> responseData = dal.ItemTransactionDataGet();
                    List<ItemTransactionData> filteredData1 = responseData.Where(item => item.IT_Id == ITID).ToList();

                    if (filteredData1 != null && filteredData1.Count > 0)
                    {
                        ItemTransactionData selectedItem = filteredData1[0];
                        txtReceivedQty.Text = selectedItem.IT_ReceivedQty.ToString();
                        txtBalanceQty.Text = selectedItem.IT_BalanceQty.ToString();
                        txtPreReceivedQty.Text = selectedItem.IT_PreviousReceivedQty.ToString();
                        txtPreBalanceQty.Text = selectedItem.IT_PreviousBalanceQty.ToString();
                        txtAcceptQty.Text = selectedItem.IT_AcceptedQty.ToString();
                        txtRejectQty.Text = selectedItem.IT_RejectedQty.ToString();
                        txtPreAcceptQty.Text = selectedItem.IT_PreviousAcceptedQty.ToString();
                        txtPreRejectedQty.Text = selectedItem.IT_PreviousRejectedQty.ToString();
                        txtDeliveryDateTime.Text = selectedItem.IT_DeliveryDateTime.ToString();
                        txtExpiryDate.Text = selectedItem.IT_ExpiryDateTime.ToString();
                        txtBatchNo.Text = selectedItem.IT_BatchNumber.ToString();
                        lblorderQty.Text = selectedItem.IT_OrderedQty.ToString();
                        lblreceiveQty.Text = selectedItem.IT_ReceivedQty.ToString();
                        lblbalanceQty.Text = selectedItem.IT_BalanceQty.ToString();
                        txtBasicPrice.Text = selectedItem.IT_BasicPrice.ToString();
                        ddlVendor.SelectedValue = selectedItem.IT_VendorId.ToString();
                        lblPoID.Text = selectedItem.IT_PoId.ToString();
                        lblPoNo.Text = selectedItem.IT_PoNumber.ToString();
                        lblbalanceQty.Visible = false;
                        lblPoID.Visible = false;
                        lblPoNo.Visible = false;

                        txtShortFall.Text = selectedItem.IT_ShortFall.ToString();
                        txtTotalShortFallPrevious.Text = selectedItem.IT_PreviousShortFall.ToString();
                        txtPreReceivedQtyAsInvoice.Text = selectedItem.IT_PreviousReceivedQtyInvoice.ToString();
                        txtReceivedQtyAsInvoice.Text = selectedItem.IT_ReceivedQtyInvoice.ToString();
                    }

                    List<InventoryItemData> response = dal.ActiveInventoryDetailsGet();
                    List<InventoryItemData> filteredData = response.Where(item => item.ItemCode == poItemCode).ToList();

                    List<string> selectedCheckBoxNames = new List<string>();

                    if (filteredData != null && filteredData.Count > 0)
                    {
                        InventoryItemData selectedItem = filteredData[0];
                        //ddlCompanyUnit.SelectedValue = selectedItem.InventoryCompanyUnitId.ToString();
                        //ddlVendor.SelectedValue = selectedItem.VendorId.ToString();
                        ddlRackType.SelectedValue = selectedItem.RackTypeId.ToString();
                        ddlUOM.SelectedValue = selectedItem.UOMId.ToString();
                        ddlVAT.SelectedValue = selectedItem.VATID.ToString();

                        txtItemName.Text = selectedItem.ItemName.ToString();
                        txtHSNCode.Text = selectedItem.HSNCode.ToString();
                        txtCurrentStock.Text = selectedItem.InventoryCurrentStock.ToString();
                        txtMinStock.Text = selectedItem.InventoryMinStock.ToString();
                        //txtBasicPrice.Text = selectedItem.InventoryBasicPrice.ToString();

                        int inventoryId = selectedItem.InventoryId;
                        ViewState["InventoryID"] = inventoryId;

                        ddlRackType_SelectedIndexChanged(sender, e);

                        foreach (ListItem item in chkRowSeries.Items)
                        {
                            string[] getData = selectedItem.InventoryItemLocationStore.Split(',');
                            bool found = false;
                            foreach (string DataShow in getData)
                            {
                                if (DataShow.Trim() == item.Value)
                                {
                                    string RackSerialNo = item.Value;
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

                        List<RackNumberData> response2 = dal.AllRackRowSerialNumberDisplay(poItemCode);
                        Repeater1.DataSource = response2;
                        Repeater1.DataBind();

                        // Move the data retrieval outside the loop
                        List<ItemStoreData> response11 = dal.DataStoreinRack();
                        List<ItemStoreData> filteredData11 = response11.Where(item11 => item11.IS_ITId == ITID).ToList();

                        foreach (RepeaterItem item in Repeater1.Items)
                        {
                            if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                            {
                                TextBox txtBox = (TextBox)item.FindControl("txtBox");
                                Label lblRowTotal = (Label)item.FindControl("lblRowTotal");

                                if (filteredData11 != null && filteredData11.Count > 0)
                                {
                                    int currentIndex = item.ItemIndex % filteredData11.Count;
                                    ItemStoreData selectedItem11 = filteredData11[currentIndex];

                                    txtBox.Text = selectedItem11.IS_StoreQty.ToString();
                                }
                            }
                        }

                    }
                    else
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No Records Found.'); setTimeout(function() { window.location = 'ItemTransactionAdd.aspx'; }, 10);</script>");
                    }


                }
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                var s = new StackTrace(ex);
                var methodName = s.GetFrames().Select(f => f.GetMethod()).First(m => m.Module.Assembly == Assembly.GetExecutingAssembly()).Name;
                string PageName = System.IO.Path.GetFileName(Request.Url.AbsolutePath);
                /* ErrorLogData(string methodName, string errorMessage, string errorDescription, string errorLineNumber) */
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

        protected void ddlRackType_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedRackTypeId;

            if (int.TryParse(ddlRackType.SelectedValue, out selectedRackTypeId))
            {
                BindRackRowSeriesNumbers(selectedRackTypeId);
            }
        }

        private void BindRackRowSeriesNumbers(int selectedRackTypeId)
        {
            try
            {
                List<RackNumberData> allRackNumbers = dal.AllRackRowSerialNumberDisplay();
                List<RackNumberData> filteredRackNumbers = allRackNumbers
                    .Where(item => item.RackTypeId == selectedRackTypeId).ToList();

                chkRowSeries.DataSource = filteredRackNumbers;
                chkRowSeries.DataTextField = "RackSerialNumber";
                chkRowSeries.DataValueField = "RackSerialId";
                chkRowSeries.DataBind();
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                var s = new StackTrace(ex);
                var methodName = s.GetFrames().Select(f => f.GetMethod()).First(m => m.Module.Assembly == Assembly.GetExecutingAssembly()).Name;
                string PageName = System.IO.Path.GetFileName(Request.Url.AbsolutePath);
                /* ErrorLogData(string methodName, string errorMessage, string errorDescription, string errorLineNumber) */
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

        [System.Web.Services.WebMethod]
        public static string UpdateBalanceQty(string acceptQty, string poOrderedQty, string poReceivedQty)
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

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            decimal totalSum = 0;
            decimal rowTotal;

            for (int i = 0; i < Repeater1.Items.Count; i++)
            {
                CheckBox chkRowTotal = (CheckBox)Repeater1.Items[i].FindControl("chkRowTotal");
                Label lblRowTotal = (Label)Repeater1.Items[i].FindControl("lblRowTotal");
                TextBox txtBox = (TextBox)Repeater1.Items[i].FindControl("txtBox");

                if (chkRowTotal.Checked)
                {
                    if (!string.IsNullOrEmpty(txtBox.Text))
                    {

                        if (decimal.TryParse(txtBox.Text, out rowTotal))
                        {
                            totalSum += rowTotal;
                        }
                        else
                        {
                            totalSum += 0;
                        }
                    }
                    else
                    {
                        totalSum += 0;
                    }
                }
            }

            decimal acceptQty = Convert.ToDecimal(txtAcceptQty.Text);

            if (totalSum == acceptQty)
            {
                try
                {
                    string getPoNo = lblPoNo.Text;
                    string poItemCode = ddlPoItemCode.SelectedItem.Text;

                    List<PurchaseOrder> responseData = dal.GetPurchaseOrderDetailsDataForStore();
                    List<PurchaseOrder> filteredData1 = responseData.Where(item => item.PONumber == getPoNo && item.ItemCode == poItemCode).ToList();

                    if (filteredData1 != null && filteredData1.Count > 0)
                    {
                        PurchaseOrder selectedItems = filteredData1[0];
                        double PoOrderedQty = Convert.ToDouble(selectedItems.PO_OrderedQty.ToString());
                        double PoReceivedQty = Convert.ToDouble(selectedItems.PO_ReceivedQty.ToString());
                        double PoAcceptQty = Convert.ToDouble(selectedItems.PO_AcceptQty.ToString());
                        double PoRejectQty = Convert.ToDouble(selectedItems.PO_RejectQty.ToString());
                        double PoBalanceQty = Convert.ToDouble(selectedItems.PO_BalanceQty.ToString());
                        double BalanceQty = Convert.ToDouble(txtBalanceQty.Text);
                        double ItemReceivedQty = Convert.ToDouble(txtReceivedQty.Text);
                        double AcceptQty = Convert.ToDouble(txtAcceptQty.Text);
                        double ItemRejectQty = ItemReceivedQty - AcceptQty;
                        int inventoryID = Convert.ToInt32(ViewState["InventoryID"].ToString());
                        string GstPercentage = "";
                        string BasicAmount = txtBasicPrice.Text;
                        decimal itemQty = Convert.ToDecimal(txtAcceptQty.Text);
                        decimal totalAmount = 0;
                        decimal totalPerItem = 0;
                        int PurchaseOrderID = selectedItems.PurchaseOrderID;

                        if (ItemRejectQty >= 0)
                        {   /* TotalAmount = TAX + Basic Price */
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
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Something Wrong TAX Not Get!!!'); </script>");
                            }

                            int GRNDetailsID = 0;
                            List<GRNMasterData> grnDetails = dal.GRNDetailsShow();
                            List<GRNMasterData> filteredgrnDetails = grnDetails.Where(item => item.GRN_NUMBER == ddlGRNNumber.SelectedItem.Text && item.GDPoItemCode  == ddlPoItemCode.SelectedItem.Text).ToList();

                            if (filteredgrnDetails != null && filteredgrnDetails.Count > 0)
                            {
                                GRNMasterData selectedItem = filteredgrnDetails[0];
                                GRNDetailsID = selectedItem.GRNDetailsID;
                            }
                            else
                            {
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Something Wrong GRN Details Not Get!!!');");
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

                            bool saveData = false;
                            int successCount = 0;


                            for (int i = 0; i < Repeater1.Items.Count; i++)
                            {
                                CheckBox chkRowTotal = (CheckBox)Repeater1.Items[i].FindControl("chkRowTotal");
                                Label lblRowTotal = (Label)Repeater1.Items[i].FindControl("lblRowTotal");
                                TextBox txtBox = (TextBox)Repeater1.Items[i].FindControl("txtBox");

                                if (chkRowTotal.Checked)
                                {
                                    if (string.IsNullOrEmpty(txtBox.Text))
                                    {
                                        txtBox.Text = "0";
                                    }

                                    msg.Text += lblRowTotal.Text + "," + chkRowTotal.Text + "," + txtBox.Text + "|";

                                    bool IsRackAllocated = dal.IsStoreDataInRack(ddlGRNNumber.SelectedItem.Text, lblRowTotal.Text);

                                    if (IsRackAllocated)
                                    {
                                        /* update ItemStore table */
                                        string query1 = @"UPDATE ItemStore SET IS_RackSerialNo = @IS_RackSerialNo, IS_StoreQty = @IS_StoreQty 
                                                           WHERE IS_RackSerialNo = @IS_RackSerialNo AND IS_GRNNUMBER = @IS_GRNNUMBER";

                                        using (SqlCommand cmd = new SqlCommand(query1, con))
                                        {
                                            cmd.Parameters.AddWithValue("@IS_StoreQty", txtBox.Text);
                                            cmd.Parameters.AddWithValue("@IS_RackSerialNo", lblRowTotal.Text);
                                            cmd.Parameters.AddWithValue("@IS_GRNNUMBER", ddlGRNNumber.SelectedItem.Text);

                                            con.Open();
                                            int I = cmd.ExecuteNonQuery();
                                            con.Close();

                                            if (I > 0)
                                            {
                                                List<ItemStoreData> response11 = dal.DataStoreinRack();
                                                List<ItemStoreData> filteredData11 = response11.Where(item => item.IS_RackSerialNo == lblRowTotal.Text
                                                && item.IS_GRNNUMBER == ddlGRNNumber.SelectedItem.Text).ToList();

                                                if (filteredData11 != null && filteredData11.Count > 0)
                                                {
                                                    ItemStoreData selectedItem = filteredData11[0];
                                                    int ItemStoreId = selectedItem.IS_ItemStoreId;

                                                    var TableRowId = ItemStoreId.ToString();
                                                    bool success = dal.AuditLogData("Update Item Store for Rack", "ItemStore", TableRowId, "");
                                                    if (success) { }
                                                    else
                                                    {
                                                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                                                    }
                                                }
                                                else
                                                {
                                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                                                }
                                                saveData = true;
                                            }
                                            else
                                            {
                                                throw new Exception("Failed to update Item Store.");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        /* insert code*/
                                        con.Open();
                                        int prevIdentity = dal.GetReseedIdentity("ItemStore", "IS_ItemStoreId");
                                        dal.ReseedIdentity("ItemStore", prevIdentity);
                                        con.Close();

                                        string query1 = @"INSERT INTO ItemStore (IS_RackSerialNo, IS_StoreQty, IS_ITId, IS_GRNNUMBER)
                                                          OUTPUT INSERTED.IS_ItemStoreId
                                                          VALUES (@IS_RackSerialNo, @IS_StoreQty, @IS_ITId, @IS_GRNNUMBER)";

                                        using (SqlCommand cmd = new SqlCommand(query1, con))
                                        {
                                            cmd.Parameters.AddWithValue("@IS_StoreQty", txtBox.Text);
                                            cmd.Parameters.AddWithValue("@IS_RackSerialNo", lblRowTotal.Text);
                                            cmd.Parameters.AddWithValue("@IS_ITId", ddlPoItemCode.SelectedItem.Value);
                                            cmd.Parameters.AddWithValue("@IS_GRNNUMBER", ddlGRNNumber.SelectedItem.Text);

                                            con.Open();
                                            int InsertGetId = (int)cmd.ExecuteScalar();
                                            con.Close();

                                            if (InsertGetId > 0)
                                            {
                                                var TableRowId = InsertGetId.ToString();
                                                bool success = dal.AuditLogData("Add Item Store for Rack", "ItemStore", TableRowId, "");
                                                if (success) { }
                                                else
                                                {
                                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                                                }
                                                saveData = true;
                                            }
                                            else
                                            {
                                                throw new Exception("Failed to SAVE Item Store.");
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Select Checkbox.');</script>");
                                }
                            }

                            if (saveData == true)
                            {
                                string QRNo = ddlPoItemCode.SelectedItem.Value;

                                string query = @"update ItemTransactionMaster set
                                                 IT_AcceptedQty = @IT_AcceptedQty, IT_RejectedQty = @IT_RejectedQty, IT_TotalAmount = @IT_TotalAmount,IT_isValid = @IT_isValid,
                                                 IT_MasterCompanyId = @IT_MasterCompanyId, IT_CreatedEmployeeId = @IT_CreatedEmployeeId, IT_Status = @IT_Status,
                                                 IT_CompanyUnitId = @IT_CompanyUnitId, IT_RackRowSeriesNo = @IT_RackRowSeriesNo, IT_UpdatedDateTime = @IT_UpdatedDateTime                                   
                                                 WHERE IT_Id = @IT_Id";

                                using (SqlCommand cmd = new SqlCommand(query, con))
                                {
                                    cmd.Parameters.AddWithValue("@IT_AcceptedQty", txtAcceptQty.Text);
                                    cmd.Parameters.AddWithValue("@IT_RejectedQty", ItemRejectQty);
                                    cmd.Parameters.AddWithValue("@IT_TotalAmount", roundedTotalAmount);
                                    cmd.Parameters.AddWithValue("@IT_MasterCompanyId", Session["MasterCompanyId"].ToString());
                                    cmd.Parameters.AddWithValue("@IT_CreatedEmployeeId", Session["EmployeeID"].ToString());
                                    cmd.Parameters.AddWithValue("@IT_CompanyUnitId", Session["CompanyUnitId"].ToString());
                                    cmd.Parameters.AddWithValue("@IT_RackRowSeriesNo", chkboxSelect);
                                    cmd.Parameters.AddWithValue("@IT_UpdatedDateTime", GetDateTime);
                                    cmd.Parameters.AddWithValue("@IT_Status", 15);
                                    cmd.Parameters.AddWithValue("@IT_isValid", 1);
                                    cmd.Parameters.AddWithValue("@IT_Id", ddlPoItemCode.SelectedItem.Value);

                                    con.Open();
                                    int a = cmd.ExecuteNonQuery();
                                    con.Close();

                                    if (a > 0)
                                    {
                                        successCount++;
                                    }
                                    else
                                    {
                                        throw new Exception("Failed to update ItemTransactionMaster.");
                                    }

                                    using (SqlConnection con1 = new SqlConnection(constr))
                                    {
                                        string str = @"UPDATE InventoryMaster set
                                                       InventoryCurrentStock = @InventoryCurrentStock,
                                                       InventoryCreatedEmployeeId = @InventoryCreatedEmployeeId,
                                                       InventoryMasterCompanyId = @InventoryMasterCompanyId,                                                
                                                       InventoryUpdatedDateTime = @InventoryUpdatedDateTime
                                                       WHERE InventoryId = @InventoryId";

                                        SqlCommand cmd1 = new SqlCommand(str, con1);
                                        cmd1.Parameters.AddWithValue("@InventoryId", inventoryID);
                                        double totalStock = Convert.ToDouble(txtCurrentStock.Text) + Convert.ToDouble(txtAcceptQty.Text);
                                        cmd1.Parameters.AddWithValue("@InventoryCurrentStock", totalStock);
                                        cmd1.Parameters.AddWithValue("@InventoryCompanyUnitId", Session["CompanyUnitId"].ToString());
                                        cmd1.Parameters.AddWithValue("@InventoryMasterCompanyId", Session["MasterCompanyId"].ToString());
                                        cmd1.Parameters.AddWithValue("@InventoryCreatedEmployeeId", Session["EmployeeID"].ToString());
                                        cmd1.Parameters.AddWithValue("@InventoryUpdatedDateTime", GetDateTime);

                                        con1.Open();
                                        int b = cmd1.ExecuteNonQuery();
                                        con1.Close();

                                        if (b > 0)
                                        {
                                            var TableRowId2 = inventoryID.ToString();
                                            bool successes2 = dal.AuditLogData("Update Inventory Stock", "InventoryMaster", TableRowId2, "");
                                            if (successes2) { }
                                            else
                                            {
                                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                                            }
                                            successCount++;
                                        }
                                        else
                                        {
                                            throw new Exception("Failed to update InventoryMaster.");
                                        }
                                    }


                                    using (SqlConnection con2 = new SqlConnection(constr))
                                    {
                                        string str2 = @"UPDATE PurchaseOrderDetails set POStatusID = @PO_StatusId, PO_StatusId = @PO_StatusId, PO_StatusForIT = @PO_StatusForIT, PO_AcceptQty = @PO_AcceptQty, PO_RejectQty = @PO_RejectQty WHERE PODetailsID = @PO_Id";
                                        SqlCommand cmd2 = new SqlCommand(str2, con2);
                                        cmd2.Parameters.AddWithValue("@PO_Id", lblPoID.Text);
                                        cmd2.Parameters.AddWithValue("@PO_Number", lblPoNo.Text);
                                        cmd2.Parameters.AddWithValue("@PO_StatusId", 15);
                                        double TotalAcceptQty = PoAcceptQty + Convert.ToDouble(txtAcceptQty.Text);
                                        cmd2.Parameters.AddWithValue("@PO_AcceptQty", TotalAcceptQty);
                                        double TotalRejectQty = PoRejectQty + Convert.ToDouble(ItemRejectQty);
                                        cmd2.Parameters.AddWithValue("@PO_RejectQty", TotalRejectQty);
                                        cmd2.Parameters.AddWithValue("@PO_StatusForIT", 0);

                                        con2.Open();
                                        int c = cmd2.ExecuteNonQuery();
                                        con2.Close();

                                        if (c > 0)
                                        {
                                            using (SqlConnection con12 = new SqlConnection(constr))
                                            {
                                                string str12 = @"UPDATE PurchaseOrder set PurchaseOrderStatus = @PurchaseOrderStatus WHERE PONumber = @PO_Number";
                                                SqlCommand cmd12 = new SqlCommand(str12, con12);                                                
                                                cmd12.Parameters.AddWithValue("@PO_Number", lblPoNo.Text);
                                                cmd12.Parameters.AddWithValue("@PurchaseOrderStatus", 15);
                                                con12.Open();
                                                int c12 = cmd12.ExecuteNonQuery();
                                                con12.Close();

                                                if (c12 > 0)
                                                {
                                                    successCount++;
                                                }
                                                else
                                                {
                                                    throw new Exception("Failed to update PoUploadMaster.");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception("Failed to update PoUploadMaster.");
                                        }
                                    }

                                    using (SqlConnection con3 = new SqlConnection(constr))
                                    {
                                        string str3 = @"UPDATE GRNMaster set GRN_EmployeeID = @GRN_EmployeeID, 
                                                        GRN_UpdatedDateTime = @GRN_UpdatedDateTime, GRN_Status = @GRN_Status
                                                        WHERE GRN_NUMBER = @GRN_NUMBER";

                                        SqlCommand cmd3 = new SqlCommand(str3, con3);

                                        cmd3.Parameters.AddWithValue("@GRN_NUMBER", ddlGRNNumber.SelectedItem.Text);
                                        //cmd3.Parameters.AddWithValue("@GRN_CompanyUnitId", Session["CompanyUnitId"].ToString());
                                        cmd3.Parameters.AddWithValue("@GRN_MasterCompanyId", Session["MasterCompanyId"].ToString());
                                        cmd3.Parameters.AddWithValue("@GRN_EmployeeID", Session["EmployeeID"].ToString());
                                        cmd3.Parameters.AddWithValue("@GRN_UpdatedDateTime", GetDateTime);
                                        cmd3.Parameters.AddWithValue("@GRN_Status", 15);

                                        con3.Open();
                                        int d = cmd3.ExecuteNonQuery();
                                        con3.Close();

                                        if (d > 0)
                                        {
                                            var TableRowId = ddlGRNNumber.SelectedItem.Value;
                                            bool successes = dal.AuditLogData("Update GRN for Item Arrangement", "GRNMaster", TableRowId, "");
                                            if (successes) { }
                                            else
                                            {
                                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                                            }
                                            successCount++;
                                        }
                                        else
                                        {
                                            throw new Exception("Failed to update GRNMaster.");
                                        }
                                    }

                                    using (SqlConnection con4 = new SqlConnection(constr))
                                    {
                                        string str4 = @"UPDATE GRNDetails set GDStatusId = @GDStatusId WHERE GRNDetailsID = @GRNDetailsID";

                                        SqlCommand cmd4 = new SqlCommand(str4, con4);
                                        cmd4.Parameters.AddWithValue("@GDStatusId", 15);
                                        cmd4.Parameters.AddWithValue("@GRNDetailsID", GRNDetailsID);

                                        con4.Open();
                                        int e1 = cmd4.ExecuteNonQuery();
                                        con4.Close();

                                        if (e1 > 0)
                                        {
                                            var TableRowId1 = GRNDetailsID.ToString();
                                            bool successes1 = dal.AuditLogData("Update GRN Details for Item Arrangement", "GRNDetails", TableRowId1, "");
                                            if (successes1) { }
                                            else
                                            {
                                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                                            }
                                            successCount++;
                                        }
                                        else
                                        {
                                            throw new Exception("Failed to update GRNDetails.");
                                        }
                                    }


                                    bool IsExisted = dal.IsDateExistForDataStoreOrRelease(date, ddlPoItemCode.SelectedItem.Text);

                                    if (IsExisted)
                                    {
                                        List<WarehouseStockDetailsData> responseData11 = dal.WarehouseStockDetailsGet();
                                        List<WarehouseStockDetailsData> filteredData11 = responseData11.Where(item => item.SC_ItemCode == ddlPoItemCode.SelectedItem.Text && item.SC_EntryDate == date).ToList();

                                        if (filteredData11 != null && filteredData11.Count > 0)
                                        {
                                            WarehouseStockDetailsData selectedItems11 = filteredData11[0];
                                            int ScId = selectedItems11.SC_ID;
                                            decimal SC_TotalReceivedQty = selectedItems11.SC_TotalReceivedQty;
                                            decimal TotalReceivedQty = SC_TotalReceivedQty + Convert.ToDecimal(txtAcceptQty.Text);

                                            using (SqlConnection con5 = new SqlConnection(constr))
                                            {
                                                string str5 = @"UPDATE WarehouseStockDetails set SC_TotalReceivedQty = @SC_TotalReceivedQty, SC_UpdatedDateTime = @SC_UpdatedDateTime
                                                                WHERE SC_EntryDate = @SC_EntryDate AND SC_ItemCode = @SC_ItemCode AND SC_MasterCompanyId = @SC_MasterCompanyId";

                                                SqlCommand cmd5 = new SqlCommand(str5, con5);
                                                cmd5.Parameters.AddWithValue("@SC_TotalReceivedQty", TotalReceivedQty);
                                                cmd5.Parameters.AddWithValue("@SC_ItemCode", ddlPoItemCode.SelectedItem.Text);
                                                cmd5.Parameters.AddWithValue("@SC_MasterCompanyId", Session["MasterCompanyId"]);
                                                cmd5.Parameters.AddWithValue("@SC_CompanyUnitId", Session["CompanyUnitId"]);
                                                cmd5.Parameters.AddWithValue("@SC_EntryDate", date);
                                                cmd5.Parameters.AddWithValue("@SC_UpdatedDateTime", GetDateTime);

                                                con5.Open();
                                                int f1 = cmd5.ExecuteNonQuery();
                                                con5.Close();

                                                if (f1 > 0)
                                                {
                                                    var TableRowId1 = ScId.ToString();
                                                    bool successes1 = dal.AuditLogData("Update Stock in Warehouse", "WarehouseStockDetails", TableRowId1, "");
                                                    if (successes1) { }
                                                    else
                                                    {
                                                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                                                    }
                                                    successCount++;
                                                }
                                                else
                                                {
                                                    throw new Exception("Failed to update WarehouseStockDetails.");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception("Warehouse Table Data not Getting.");
                                        }
                                    }
                                    else
                                    {
                                        /* insert the data to w.stock*/
                                        con.Open();
                                        int prevIdentity1 = dal.GetReseedIdentity("WarehouseStockDetails", "SC_ID");
                                        dal.ReseedIdentity("WarehouseStockDetails", prevIdentity1);
                                        con.Close();

                                        string query1 = @"INSERT INTO WarehouseStockDetails 
                                                          (SC_ItemCode, SC_TotalReceivedQty, SC_TotalReleasedQty, SC_BalanceQty,
                                                           SC_MasterCompanyId, SC_EmployeeId, SC_EntryDate, SC_UOM, SC_CreatedDateTime)
                                                          OUTPUT INSERTED.SC_ID VALUES 
                                                          (@SC_ItemCode, @SC_TotalReceivedQty, @SC_TotalReleasedQty, @SC_BalanceQty,
                                                           @SC_MasterCompanyId, @SC_EmployeeId, @SC_EntryDate, @SC_UOM, @SC_CreatedDateTime)";

                                        using (SqlCommand cmds = new SqlCommand(query1, con))
                                        {
                                            cmds.Parameters.AddWithValue("@SC_ItemCode", ddlPoItemCode.SelectedItem.Text);
                                            cmds.Parameters.AddWithValue("@SC_TotalReceivedQty", txtAcceptQty.Text);
                                            cmds.Parameters.AddWithValue("@SC_TotalReleasedQty", 0);
                                            cmds.Parameters.AddWithValue("@SC_BalanceQty", 0);
                                            cmds.Parameters.AddWithValue("@SC_MasterCompanyId", Session["MasterCompanyId"]);
                                            cmds.Parameters.AddWithValue("@SC_EmployeeId",Session["UserID"]);
                                            cmds.Parameters.AddWithValue("@SC_EntryDate", date);
                                            cmds.Parameters.AddWithValue("@SC_UOM", ddlUOM.SelectedItem.Text);
                                            cmds.Parameters.AddWithValue("@SC_CreatedDateTime", GetDateTime);

                                            con.Open();
                                            int InsertGetId1 = (int)cmds.ExecuteScalar();
                                            con.Close();

                                            if (InsertGetId1 > 0)
                                            {
                                                var TableRowId1 = InsertGetId1.ToString();
                                                bool successes1 = dal.AuditLogData("Add Stock in Warehouse", "WarehouseStockDetails", TableRowId1, "");
                                                if (successes1) { }
                                                else
                                                {
                                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Audit Log failed!.'); </script>");
                                                }
                                                successCount++;
                                            }
                                            else
                                            {
                                                throw new Exception("Failed to SAVE Item Store in warehouse details.");
                                            }
                                        }

                                    }

                                    if (successCount == 6)
                                    {
                                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('All updates successful!'); setTimeout(function() { window.location = 'ItemArrangementAdd.aspx'; }, 100);</script>");
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
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Filtered Data is Not getting!');</script>");
                    }
                }
                catch (Exception ex)
                {
                    int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                    var s = new StackTrace(ex);
                    var methodName = s.GetFrames().Select(f => f.GetMethod()).First(m => m.Module.Assembly == Assembly.GetExecutingAssembly()).Name;
                    string PageName = System.IO.Path.GetFileName(Request.Url.AbsolutePath);
                    /* ErrorLogData(string methodName, string errorMessage, string errorDescription, string errorLineNumber) */
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
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "invalidAlert", "alert('Invalid Store Quantity.');", true);
            }
        }


    }
}