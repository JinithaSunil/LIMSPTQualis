using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LIMS.Models;



namespace LIMS
{
    public partial class PurchaseItemAdd : System.Web.UI.Page
    {

        LIMS.DAL.CM _cm = new LIMS.DAL.CM();
        DAL.GLModule _gl = new DAL.GLModule();
        // bool ItemCodeExists = false;
        //string toleranceUOM = "4,8,6,15,18,20";

        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ToString());

        string date = DateTime.Today.ToString("dd-MMM-yyyy");
        string time = DateTime.Now.ToString("h:mm:ss tt");

        string CreateDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string UpdateDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        string constr = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    BindUnits();
                    BindPurchaseGroup();
                    BindVAT1();
                    BindCOA();

                    BindStoreType();

                    if (Request.QueryString["ID"] != null)
                    {
                        string ItemID = Request.QueryString["ID"].ToString();
                        btnAdd.Visible = false;
                        btnCancel.Visible = false;
                        btnUpdate.Visible = true;
                        btnPrevious.Visible = true;
                        btnNext.Visible = true;

                        // First get the item's purchase group
                        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ToString()))
                        {
                            conn.Open();
                            string query = "SELECT PurchaseGroupID FROM Items WHERE ItemID = @ItemID";
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@ItemID", ItemID);
                                string purchaseGroupID = cmd.ExecuteScalar()?.ToString();

                                if (!string.IsNullOrEmpty(purchaseGroupID))
                                {
                                    ddlPurchaseGroup.SelectedValue = purchaseGroupID;

                                    // Now that we have set the purchase group, we can get the item details
                                    if (ddlPurchaseGroup.SelectedIndex > 0)
                                    {
                                        GetItemDetails(ItemID, purchaseGroupID);
                                    }
                                    else
                                    {
                                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                                            "alert('Purchase Group not found for this item!');", true);
                                    }
                                }
                                else
                                {
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                                        "alert('Item not found or no Purchase Group assigned!');", true);
                                }
                            }
                        }



                    }
                    else
                    {
                        Session["Type"] = "Add";
                        btnAdd.Visible = true;
                        btnCancel.Visible = true;
                        btnUpdate.Visible = false;
                    }
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }

        protected void BindCOA()
        {
            DataTable dt = _gl.GetCOAPurchaseItems();
            ddlCOA.DataSource = dt;
            ddlCOA.DataTextField = "AccountName";
            ddlCOA.DataValueField = "AccountCode";
            ddlCOA.DataBind();
            ddlCOA.Items.Insert(0, new ListItem("--Select COA--", ""));

            foreach (ListItem item in ddlCOA.Items)
            {
                DataRow row = dt.AsEnumerable()
                                .FirstOrDefault(r => r.Field<string>("AccountCode") == item.Value);

                if (row != null)
                {
                    item.Attributes["AccountID"] = row["AccountID"].ToString();
                }
            }
        }
        public int GetReseedIdentity(string tableName, string identityField)
        {
            try
            {
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();

                con.Open();
                string sQuery = "select max(" + identityField + ") from " + tableName;

                SqlCommand cmd = new SqlCommand(sQuery, con);
                cmd.CommandType = CommandType.Text;
                int ReseedIdentity = (int)cmd.ExecuteScalar();

                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();

                return ReseedIdentity;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return 0;
            }
        }
        public void ReseedIdentity(string tableName, int reseedIdentity)
        {
            if (con.State == System.Data.ConnectionState.Open)
                con.Close();
            string sQuery = "DBCC CHECKIDENT('" + tableName + "', RESEED, " + reseedIdentity + ")";
            con.Open();
            SqlCommand cmd = new SqlCommand(sQuery, con);
            int modified = cmd.ExecuteNonQuery();
            if (con.State == System.Data.ConnectionState.Open)
                con.Close();
        }

        private void GetItemDetails(string ItemID, string PurchaseGroupID)
        {
            Session["Type"] = "Edit";
            DataTable dtCheckPR = _cm.CheckPRExistsForItem(ItemID);
            if (dtCheckPR.Rows.Count == 0)
            {
                DataTable dt = _cm.GetPurchaseItemForEdit(ItemID, PurchaseGroupID);
                if (dt.Rows.Count > 0)
                {
                    BindPurchaseSubGroup(dt.Rows[0]["PurchaseGroupID"].ToString());

                    BindPurchaseGroup();

                    ddlPurchaseGroup.ClearSelection();
                    ddlSubGroup.ClearSelection();
                    ddlUnit.ClearSelection();
                    ddlStoreType.ClearSelection();

                    txtItemCode.Text = dt.Rows[0]["ItemCode"].ToString();
                    txtItemCode.ReadOnly = true;  // Make ItemCode read-only during edit
                    txtItemCode.Enabled = false;
                    ddlPurchaseGroup.Items.FindByValue(dt.Rows[0]["PurchaseGroupID"].ToString()).Selected = true;
                    ddlSubGroup.Items.FindByValue(dt.Rows[0]["PurchaseSubGroupID"].ToString()).Selected = true;
                    ddlStoreType.Items.FindByValue(dt.Rows[0]["ItemTypeID"].ToString()).Selected = true;
                    if (dt.Rows[0]["VATId"] != DBNull.Value && !string.IsNullOrEmpty(dt.Rows[0]["VATId"].ToString()))
                    {
                        ddlVAT.Items.FindByValue(dt.Rows[0]["VATId"].ToString()).Selected = true;
                    }
                    else
                    {
                        // Set default value to "1" if VATId is not found or null
                        if (ddlVAT.Items.FindByValue("1") != null)
                        {
                            ddlVAT.Items.FindByValue("1").Selected = true;
                        }
                    }
                    txtItemDescription.Text = dt.Rows[0]["ItemName"].ToString();
                    txtDescription.Text = dt.Rows[0]["ItemDescription"].ToString();
                    txtModel.Text = dt.Rows[0]["ModelNo"].ToString();

                    // Handle nullable fields
                    txtStock.Text = dt.Rows[0]["StockQuantity"] != DBNull.Value ? dt.Rows[0]["StockQuantity"].ToString() : string.Empty;
                    txtMinStock.Text = dt.Rows[0]["Minstock"] != DBNull.Value ? dt.Rows[0]["Minstock"].ToString() : string.Empty;
                    txtMaxStock.Text = dt.Rows[0]["MaxStock"] != DBNull.Value ? dt.Rows[0]["MaxStock"].ToString() : string.Empty;

                    ddlUnit.Items.FindByValue(dt.Rows[0]["UOMID"].ToString()).Selected = true;
                    ddlCOA.Items.FindByValue(dt.Rows[0]["AccountCode"].ToString()).Selected = true;

                    txtHSN.Text = dt.Rows[0]["HSNCode"] != DBNull.Value ? dt.Rows[0]["HSNCode"].ToString() : string.Empty;
                    txtLeadTime.Text = dt.Rows[0]["LeadTime"] != DBNull.Value ? dt.Rows[0]["LeadTime"].ToString() : string.Empty;
                    txtRate.Text = dt.Rows[0]["Rate"] != DBNull.Value ? dt.Rows[0]["Rate"].ToString() : string.Empty;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('No Details Found!')", true);
                }
            }
            else
            {

                btnUpdate.Visible = false;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Item Blocked For Edit!')", true);

                // Get the item details again in this scope
                DataTable dtView = _cm.GetPurchaseItemForEdit(ItemID, PurchaseGroupID);
                if (dtView.Rows.Count > 0)
                {
                    BindPurchaseSubGroup(dtView.Rows[0]["PurchaseGroupID"].ToString());
                    BindPurchaseGroup();

                    ddlPurchaseGroup.ClearSelection();
                    ddlSubGroup.ClearSelection();
                    ddlUnit.ClearSelection();
                    ddlStoreType.ClearSelection();

                    txtItemCode.Text = dtView.Rows[0]["ItemCode"].ToString();

                    ddlPurchaseGroup.Items.FindByValue(dtView.Rows[0]["PurchaseGroupID"].ToString()).Selected = true;
                    ddlSubGroup.Items.FindByValue(dtView.Rows[0]["PurchaseSubGroupID"].ToString()).Selected = true;
                    ddlStoreType.Items.FindByValue(dtView.Rows[0]["ItemTypeID"].ToString()).Selected = true;

                    // Handle nullable VATId
                    if (dtView.Rows[0]["VATId"] != DBNull.Value && !string.IsNullOrEmpty(dtView.Rows[0]["VATId"].ToString()))
                    {
                        ddlVAT.Items.FindByValue(dtView.Rows[0]["VATId"].ToString()).Selected = true;
                    }

                    txtItemDescription.Text = dtView.Rows[0]["ItemName"].ToString();
                    txtDescription.Text = dtView.Rows[0]["ItemDescription"].ToString();
                    txtModel.Text = dtView.Rows[0]["ModelNo"].ToString();

                    // Handle nullable fields
                    txtStock.Text = dtView.Rows[0]["StockQuantity"] != DBNull.Value ? dtView.Rows[0]["StockQuantity"].ToString() : string.Empty;
                    txtMinStock.Text = dtView.Rows[0]["Minstock"] != DBNull.Value ? dtView.Rows[0]["Minstock"].ToString() : string.Empty;
                    txtMaxStock.Text = dtView.Rows[0]["MaxStock"] != DBNull.Value ? dtView.Rows[0]["MaxStock"].ToString() : string.Empty;

                    ddlUnit.Items.FindByValue(dtView.Rows[0]["UOMID"].ToString()).Selected = true;
                    ddlCOA.Items.FindByValue(dtView.Rows[0]["AccountCode"].ToString()).Selected = true;

                    txtHSN.Text = dtView.Rows[0]["HSNCode"] != DBNull.Value ? dtView.Rows[0]["HSNCode"].ToString() : string.Empty;
                    txtLeadTime.Text = dtView.Rows[0]["LeadTime"] != DBNull.Value ? dtView.Rows[0]["LeadTime"].ToString() : string.Empty;
                    txtRate.Text = dtView.Rows[0]["Rate"] != DBNull.Value ? dtView.Rows[0]["Rate"].ToString() : string.Empty;

                    // Make all controls read-only
                    txtItemCode.ReadOnly = true;
                    txtItemDescription.ReadOnly = true;
                    txtDescription.ReadOnly = true;
                    txtModel.ReadOnly = true;
                    txtStock.ReadOnly = true;
                    txtMinStock.ReadOnly = true;
                    txtMaxStock.ReadOnly = true;
                    txtHSN.ReadOnly = true;
                    txtLeadTime.ReadOnly = true;
                    txtRate.ReadOnly = true;

                    // Disable dropdowns
                    ddlPurchaseGroup.Enabled = false;
                    ddlSubGroup.Enabled = false;
                    ddlUnit.Enabled = false;
                    ddlStoreType.Enabled = false;



                }
            }
        }
        protected void BindData(string purchaseGroupID, string ItemCode)
        {
            Session["Type"] = "Add";
            //DataTable dt2 = _cm.GetMinMaxQty(purchaseGroupID, ItemCode);
            DataTable dt = _cm.CheckItemCodeExists(ItemCode);
            //if (dt2.Rows.Count > 0)
            //{
            //    txtMinStock.Text = dt2.Rows[0]["MinQty"].ToString();
            //    txtMaxStock.Text = dt2.Rows[0]["MaxQty"].ToString();
            //}
            if (dt.Rows.Count > 0)
            {
                txtItemDescription.Text = dt.Rows[0]["ItemDescription"].ToString();
                // txtDescription.Text = dt.Rows[0]["Specification"].ToString();
                ddlStoreType.SelectedValue = dt.Rows[0]["ItemTypeID"].ToString();
                ddlVAT.SelectedValue = dt.Rows[0]["VATId"].ToString();

                txtStock.Text = dt.Rows[0]["StockQuantity"].ToString();
                //txtMake.Text = dt.Rows[0]["Make"].ToString();
                // txtActualValue.Text = dt.Rows[0]["UOMValue"].ToString();
                // txtConversionValue.Text = dt.Rows[0]["ConversionUOMValue"].ToString();
                txtHSN.Text = dt.Rows[0]["HSNCode"].ToString();
                //txtCGST.Text = Convert.ToInt32(dt.Rows[0]["CGST"]).ToString();
                //txtSGST.Text = Convert.ToInt32(dt.Rows[0]["SGST"]).ToString();
                // txtIGST.Text = Convert.ToInt32(dt.Rows[0]["IGST"]).ToString();
                txtLeadTime.Text = dt.Rows[0]["LeadTime"].ToString();
                txtModel.Text = dt.Rows[0]["ModelNo"].ToString();



                BindPurchaseGroup();
                string PurchaseGroupID = dt.Rows[0]["PurchaseGroupID"].ToString();
                BindPurchaseSubGroup(PurchaseGroupID);

                ddlUnit.ClearSelection();
                //ddlOrderUOM.ClearSelection();
                ddlSubGroup.ClearSelection();
                ddlPurchaseGroup.ClearSelection();

                //if (dt.Rows[0]["GSTID"].ToString() != "")
                //{
                //    ddlGST.Items.FindByValue(dt.Rows[0]["GSTID"].ToString()).Selected = true;
                //}
                if (dt.Rows[0]["UOMID"].ToString() != "")
                    ddlUnit.Items.FindByValue(dt.Rows[0]["UOMID"].ToString()).Selected = true;
                //if (dt.Rows[0]["ConversionUOMID"].ToString() != "")
                //    ddlOrderUOM.Items.FindByValue(dt.Rows[0]["ConversionUOMID"].ToString()).Selected = true;
                string psid = dt.Rows[0]["PurchaseSubGroupID"].ToString();
                if (dt.Rows[0]["PurchaseSubGroupID"].ToString() != "")
                    ddlSubGroup.Items.FindByValue(dt.Rows[0]["PurchaseSubGroupID"].ToString()).Selected = true;
                ddlPurchaseGroup.Items.FindByValue(PurchaseGroupID).Selected = true;
            }



        }
        private void BindPurchaseGroup()
        {
            DataTable dt = _cm.FillParentTable();
            if (dt.Rows.Count > 0)
            {
                ddlPurchaseGroup.DataSource = dt;
                ddlPurchaseGroup.DataTextField = "PurchaseGroupName";
                ddlPurchaseGroup.DataValueField = "PurchaseGroupID";
                ddlPurchaseGroup.DataBind();
                ddlPurchaseGroup.Items.Insert(0, new ListItem("Select Purchase Group", ""));

            }
        }
        protected void ddlPurchaseGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            string PurchaseGroupID = ddlPurchaseGroup.SelectedValue.ToString();
            Session["PurchaseGroupID"] = PurchaseGroupID;
            BindPurchaseSubGroup(PurchaseGroupID);
            BindItemCode();
        }
        protected void BindItemCode()
        {
            DataTable dtNext = _cm.GetNextItemCode();
            if (dtNext.Rows.Count > 0)
            {
                txtItemCode.Text = dtNext.Rows[0]["NextItemCode"].ToString();
                txtItemCode.Enabled = false;
            }

        }
        protected void BindPurchaseSubGroup(string PurchaseGroupID)
        {
            DataTable dt = new DataTable();
            dt = _cm.GetAllPurchaseSubGroup(PurchaseGroupID);
            ddlSubGroup.DataSource = dt;
            ddlSubGroup.DataTextField = "PurchaseSubGroupName";
            ddlSubGroup.DataValueField = "PurchaseSubGroupID";
            ddlSubGroup.DataBind();
            ddlSubGroup.Items.Insert(0, new ListItem("Select Sub Group", ""));
        }
        private void BindStoreType()
        {
            DataTable dt = _cm.GetItemType();
            if (dt.Rows.Count > 0)
            {
                ddlStoreType.DataSource = dt;
                ddlStoreType.DataTextField = "ItemType";
                ddlStoreType.DataValueField = "ItemTypeID";
                ddlStoreType.DataBind();
                ddlStoreType.Items.Insert(0, new ListItem("Select Store Type", ""));
            }
        }

        private void BindVAT1()
        {
            DataTable dt = _cm.getVAT();
            ddlVAT.DataSource = dt;
            ddlVAT.DataTextField = "VATCode";
            ddlVAT.DataValueField = "VATID";
            ddlVAT.DataBind();
            ddlVAT.Items.Insert(0, new ListItem("Select VAT", ""));
        }
        protected void ddlVat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlVAT.SelectedIndex > 0)
            {
                string VAT = ddlVAT.SelectedValue.ToString();

                if (Request.QueryString["ID"] != null)
                {
                    string ItemID = Request.QueryString["ID"].ToString();

                    if (ddlPurchaseGroup.SelectedIndex > 0)
                    {
                        GetItemDetails(ItemID, ddlPurchaseGroup.SelectedValue.ToString());
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Select PurchaseGroup!');", true);
                    }
                }
                else
                {
                    // No need to generate ItemCode automatically since it's manually entered
                    txtItemCode.ReadOnly = false;
                }
            }
        }

        protected void ddlStoreType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlStoreType.SelectedIndex > 0)
            {
                string StoreType = ddlStoreType.SelectedValue.ToString();

                if (Request.QueryString["ID"] != null)
                {
                    string ItemID = Request.QueryString["ID"].ToString();

                    if (ddlPurchaseGroup.SelectedIndex > 0)
                    {
                        GetItemDetails(ItemID, ddlPurchaseGroup.SelectedValue.ToString());
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Select PurchaseGroup!');", true);
                    }
                }
                else
                {
                    // No need to generate ItemCode automatically since it's manually entered
                    txtItemCode.ReadOnly = false;
                }
            }
        }


        protected void txtItemCode_TextChanged(object sender, EventArgs e)
        {
            string ItemCode = txtItemCode.Text.Trim();

            // Pad the ItemCode with leading zeros if necessary
            if (ItemCode.Length > 0 && ItemCode.Length < 6)
            {
                ItemCode = ItemCode.PadLeft(6, '0');
            }

            // Check if the ItemCode already exists
            DataTable dt = _cm.CheckItemCodeExists(ItemCode);
            if (dt.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    "alert('Item Code already exists! Please enter a unique Item Code.');", true);
                txtItemCode.Text = string.Empty;
                txtItemCode.Focus();
                return;
            }

            // If the ItemCode is unique, update the textbox with the padded value
            txtItemCode.Text = ItemCode;
        }
        protected void BindUnits()
        {
            DataTable dt = new DataTable();
            dt = _cm.GetAllUOM();
            if (dt.Rows.Count > 0)
            {
                ddlUnit.DataSource = dt;
                ddlUnit.DataTextField = "UOMName";
                ddlUnit.DataValueField = "UOMId";
                ddlUnit.DataBind();
                ddlUnit.Items.Insert(0, new ListItem("Select UOM", ""));
            }


        }
        protected void Clear()
        {
            txtItemCode.Text = "";
            txtItemDescription.Text = "";
            txtStock.Text = "";
            txtMinStock.Text = "";
            txtMaxStock.Text = "";
            txtRate.Text = "";
            txtLeadTime.Text = "";
            txtHSN.Text = "";
            txtModel.Text = "";

            ddlPurchaseGroup.SelectedIndex = 0;
            ddlSubGroup.SelectedIndex = 0;
            ddlStoreType.SelectedIndex = 0;
            ddlUnit.SelectedIndex = 0;
            ddlVAT.SelectedIndex = 0;
            ddlCOA.SelectedIndex = 0;

        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string ItemCode = txtItemCode.Text.Trim();
                DataTable dt = _cm.CheckItemCodeExists(ItemCode);
                if (dt.Rows.Count > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                        "alert('This Item Code already exists! Please enter a unique Item Code.');", true);
                    txtItemCode.Focus();
                    return;
                }
                if (txtStock.Text == "") { txtStock.Text = "0"; }
                if (txtMaxStock.Text == "") { txtMaxStock.Text = "0"; }
                if (txtMinStock.Text == "") { txtMinStock.Text = "0"; }

                if(!_cm.ItemExists(Convert.ToInt32(ddlPurchaseGroup.SelectedValue), 
                    (Convert.ToInt32(ddlSubGroup.SelectedValue)), txtItemDescription.Text)){
                    con.Open();
                    int prevIdentity = _cm.GetReseedIdentity("Items", "ItemId");
                    _cm.ReseedIdentity("Items", prevIdentity);
                    con.Close();

                    string query = @"insert into Items (ItemCode,ItemName,ItemDescription,HSNCode,CreatedDate,
                                StockQuantity,Minstock, MaxStock,ModelNo,Rate,PurchaseGroupID,
                                PurchaseSubGroupID,ItemTypeID,UOMID,LeadTime,DisabledItem,
                                VATId,isActive,AccountCode)
                                output INSERTED.ItemID 
                                values(@ItemCode,@ItemName,@ItemDescription, @HSNCode, @CreatedDate,
                                @StockQuantity,@Minstock, @MaxStock,@ModelNo,@Rate,
                                @PurchaseGroupID,@PurchaseSubGroupID,@ItemTypeID,@UOMID,@LeadTime,
                                @DisabledItem,@VATId,@isActive,@AccountCode)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@PurchaseGroupID", ddlPurchaseGroup.SelectedItem.Value);
                        cmd.Parameters.AddWithValue("@PurchaseSubGroupID", ddlSubGroup.SelectedItem.Value);
                        cmd.Parameters.AddWithValue("@ItemTypeID", ddlStoreType.SelectedItem.Value);
                        cmd.Parameters.AddWithValue("@UOMID", ddlUnit.SelectedItem.Value);
                        cmd.Parameters.AddWithValue("@DisabledItem", 0);
                        cmd.Parameters.AddWithValue("@isActive", 1);
                        //cmd.Parameters.AddWithValue("@ItemCode", ddlStoreType.SelectedItem.Text);

                        cmd.Parameters.AddWithValue("@ItemCode", txtItemCode.Text);
                        cmd.Parameters.AddWithValue("@ItemName", txtItemDescription.Text);
                        cmd.Parameters.AddWithValue("@ItemDescription", txtItemDescription.Text);

                        cmd.Parameters.AddWithValue("@CreatedDate", CreateDateTime); // Assuming current date and time

                        cmd.Parameters.AddWithValue("@VATId", ddlVAT.SelectedItem.Value);

                        cmd.Parameters.AddWithValue("@HSNCode",
                            string.IsNullOrWhiteSpace(txtHSN.Text) ? (object)DBNull.Value : txtHSN.Text);

                        // STOCK
                        decimal stock;
                        if (decimal.TryParse(txtStock.Text.Replace(",", "").Trim(), out stock))
                            cmd.Parameters.Add("@StockQuantity", SqlDbType.Decimal).Value = stock;
                        else
                            cmd.Parameters.Add("@StockQuantity", SqlDbType.Decimal).Value = DBNull.Value;


                        // MIN STOCK
                        int minStock;
                        if (int.TryParse(txtMinStock.Text.Replace(",", "").Trim(), out minStock))
                            cmd.Parameters.Add("@Minstock", SqlDbType.Int).Value = minStock;
                        else
                            cmd.Parameters.Add("@Minstock", SqlDbType.Int).Value = DBNull.Value;


                        // MAX STOCK
                        int maxStock;
                        if (int.TryParse(txtMaxStock.Text.Replace(",", "").Trim(), out maxStock))
                            cmd.Parameters.Add("@MaxStock", SqlDbType.Int).Value = maxStock;
                        else
                            cmd.Parameters.Add("@MaxStock", SqlDbType.Int).Value = DBNull.Value;


                        // RATE
                        decimal rate;
                        if (decimal.TryParse(txtRate.Text.Replace(",", "").Trim(), out rate))
                            cmd.Parameters.Add("@Rate", SqlDbType.Decimal).Value = rate;
                        else
                            cmd.Parameters.Add("@Rate", SqlDbType.Decimal).Value = DBNull.Value;


                        // LEAD TIME
                        int leadTime;
                        if (int.TryParse(txtLeadTime.Text.Replace(",", "").Trim(), out leadTime))
                            cmd.Parameters.Add("@LeadTime", SqlDbType.Int).Value = leadTime;
                        else
                            cmd.Parameters.Add("@LeadTime", SqlDbType.Int).Value = DBNull.Value; cmd.Parameters.AddWithValue("@ModelNo",
                            string.IsNullOrWhiteSpace(txtModel.Text) ? (object)DBNull.Value : txtModel.Text);

                        cmd.Parameters.AddWithValue("@AccountCode", ddlCOA.SelectedItem.Value);
                        con.Open();
                        int InsertGetId = (int)cmd.ExecuteScalar();
                        con.Close();

                        if (InsertGetId > 0)
                        {
                            Clear();
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(),
                                "alert",
                                "alert('Added successfully');",
                                true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(Page, Page.GetType(),
                                "alert",
                                "alert('Something went wrong, try later!');",
                                true);

                        }

                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(),
                               "alert",
                               "alert('Item with same Group, Sub Group and Item Name exists!');",
                               true);
                }
            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error Log Data Save: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
            }

        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateFields())
                {
                    string query = @"UPDATE Items 
                SET 
                    ItemName = @ItemName,
                    ItemDescription = @ItemDescription,
                    HSNCode = @HSNCode,
                    StockQuantity = @StockQuantity,
                    Minstock = @Minstock,
                    MaxStock = @MaxStock,
                    ModelNo = @ModelNo,
                    Rate = @Rate,
                    PurchaseGroupID = @PurchaseGroupID,
                    PurchaseSubGroupID = @PurchaseSubGroupID,
                    ItemTypeID = @ItemTypeID,
                    UOMID = @UOMID,
                    VATId = @VATId,
                    LeadTime = @LeadTime,
                    AccountCode = @AccountCode                     
                WHERE ItemID = @ItemID";

                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ToString()))
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ItemID", Request.QueryString["ID"]);

                        cmd.Parameters.AddWithValue("@ItemName", txtItemDescription.Text.Replace("'", "''"));
                        cmd.Parameters.AddWithValue("@ItemDescription", txtDescription.Text);

                        cmd.Parameters.AddWithValue("@PurchaseGroupID", Convert.ToInt32(ddlPurchaseGroup.SelectedValue));
                        cmd.Parameters.AddWithValue("@VATId", Convert.ToInt32(ddlVAT.SelectedValue));
                        cmd.Parameters.AddWithValue("@PurchaseSubGroupID", Convert.ToInt32(ddlSubGroup.SelectedValue));
                        cmd.Parameters.AddWithValue("@ItemTypeID", Convert.ToInt32(ddlStoreType.SelectedValue));
                        cmd.Parameters.AddWithValue("@UOMID", Convert.ToInt32(ddlUnit.SelectedValue));

                        cmd.Parameters.AddWithValue("@HSNCode",
                            string.IsNullOrWhiteSpace(txtHSN.Text) ? (object)DBNull.Value : txtHSN.Text);

                        cmd.Parameters.AddWithValue("@StockQuantity",
                            string.IsNullOrWhiteSpace(txtStock.Text) ? (object)DBNull.Value : Convert.ToInt32(txtStock.Text));

                        cmd.Parameters.AddWithValue("@Minstock",
                            string.IsNullOrWhiteSpace(txtMinStock.Text) ? (object)DBNull.Value : Convert.ToInt32(txtMinStock.Text));

                        cmd.Parameters.AddWithValue("@MaxStock",
                            string.IsNullOrWhiteSpace(txtMaxStock.Text) ? (object)DBNull.Value : Convert.ToInt32(txtMaxStock.Text));
                        cmd.Parameters.AddWithValue("@Rate",
                              string.IsNullOrWhiteSpace(txtRate.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtRate.Text));

                        cmd.Parameters.AddWithValue("@LeadTime",
                            string.IsNullOrWhiteSpace(txtLeadTime.Text) ? (object)DBNull.Value : Convert.ToInt32(txtLeadTime.Text));

                        cmd.Parameters.AddWithValue("@ModelNo",
                            string.IsNullOrWhiteSpace(txtModel.Text) ? (object)DBNull.Value : txtModel.Text);
                        cmd.Parameters.AddWithValue("@AccountCode", ddlCOA.SelectedItem.Value);

                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alert", "alert('Item Updated Successfully');", true);
                            Clear();
                        }
                        else
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alert", "alert('Update failed. Please try again.');", true);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alert", $"alert('Error updating item: {ex.Message.Replace("'", "\\'")}');", true);
            }
        }

        private bool ValidateFields()
        {
            var requiredFields = new Dictionary<Control, string>
        {
            { txtItemCode, "Item Code" },
            { txtItemDescription, "Item Description" },
            { txtMinStock, "Minimum Stock" },
            { txtMaxStock, "Maximum Stock" },
            { txtStock, "Stock" }
        };


            var requiredDropDowns = new Dictionary<DropDownList, string>
        {
            { ddlSubGroup, "Sub Group" },
            { ddlUnit, "Unit" },
            { ddlCOA, "COA" },
            { ddlVAT, "VAT" }
            //{ ddlUnitType, "Unit Type" },
            //{ ddlStoreType, "Store Type" },
            //{ ddlGST, "GST" }
        };

            bool isValid = true;

            // Validate TextBoxes


            // Validate DropDownLists
            foreach (var ddl in requiredDropDowns)
            {
                if (ddl.Key.SelectedIndex == 0)
                {
                    ddl.Key.BorderColor = System.Drawing.Color.Red;
                    isValid = false;
                }
                else
                {
                    ddl.Key.BorderColor = System.Drawing.Color.Empty;
                }
            }


            if (!isValid)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alert", "alert('Please fill in all required fields');", true);
            }

            return isValid;
        }

        //private void UpdateShelfLife(int itemId)
        //{
        //    using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["YourConnectionString"].ConnectionString))
        //    using (SqlCommand cmd = new SqlCommand("UPDATE Items SET ShelfLifeDays = @ShelfLifeDays WHERE ItemID = @ItemID", connection))
        //    {
        //        cmd.Parameters.AddWithValue("@ItemID", itemId);
        //        cmd.Parameters.AddWithValue("@ShelfLifeDays", Convert.ToInt32(txtSheftLife.Text));

        //        connection.Open();
        //        cmd.ExecuteNonQuery();
        //    }
        //}
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Clear();
        }
        protected void btnPrevious_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["ID"] != null)
            {
                int ItemID = Convert.ToInt32(Request.QueryString["ID"].ToString());
                Response.Redirect("PurchaseItemAdd.aspx?ID=" + (ItemID - 1));
            }
        }
        protected void btnNext_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["ID"] != null)
            {
                int ItemID = Convert.ToInt32(Request.QueryString["ID"].ToString());
                Response.Redirect("PurchaseItemAdd.aspx?ID=" + (ItemID + 1));
            }
        }
        protected void btnShowAddUOM_Click(object sender, EventArgs e)
        {
            // Show the Add UOM panel
            pnlAddUOM.Visible = true;

            // Clear any previous messages
            lblUOMMessage.Visible = false;
            lblUOMMessage.Text = "";

            // Clear the input fields
            txtNewUOMName.Text = "";
            txtNewUOMCode.Text = "";

            // Focus on the first input field
            txtNewUOMName.Focus();
        }
        protected void btnCancelUOM_Click(object sender, EventArgs e)
        {
            // Hide the Add UOM panel
            pnlAddUOM.Visible = false;

            // Clear the input fields
            txtNewUOMName.Text = "";
            txtNewUOMCode.Text = "";

            // Clear any messages
            lblUOMMessage.Visible = false;
            lblUOMMessage.Text = "";
        }
        protected void btnSaveUOM_Click(object sender, EventArgs e)
        {
            // Clear any previous messages
            lblUOMMessage.Visible = false;
            lblUOMMessage.Text = "";

            try
            {
                // Validate required fields
                if (string.IsNullOrEmpty(txtNewUOMName.Text.Trim()))
                {
                    ShowUOMMessage("UOM Name is required.", false);
                    return;
                }

                string uomName = txtNewUOMName.Text.Trim();
                string uomCode = txtNewUOMCode.Text.Trim();

                // Check if UOM Name already exists
                if (IsUOMNameExists(uomName))
                {
                    ShowUOMMessage("UOM Name already exists. Please enter a different name.", false);
                    return;
                }

                // Check if UOM Code already exists (if provided)
                if (!string.IsNullOrEmpty(uomCode) && IsUOMCodeExists(uomCode))
                {
                    ShowUOMMessage("UOM Code already exists. Please enter a different code.", false);
                    return;
                }

                // Insert new UOM into database
                string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
                string query = @"INSERT INTO UOM (UOMName, UOMCode, createdDate, OrganizationID) 
                        VALUES (@UOMName, @UOMCode, @createdDate, @OrganizationID)";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UOMName", uomName);
                        cmd.Parameters.AddWithValue("@UOMCode", string.IsNullOrEmpty(uomCode) ? DBNull.Value : (object)uomCode);
                        cmd.Parameters.AddWithValue("@createdDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@OrganizationID", 1);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                // Show success message
                ShowUOMMessage("UOM added successfully!", true);

                // Clear the form fields
                txtNewUOMName.Text = "";
                txtNewUOMCode.Text = "";

                // Reload the UOM dropdown to include the new UOM
                LoadUOMs();

                // Hide the panel after successful save
                pnlAddUOM.Visible = false;

                // Show success alert
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                    "alert('UOM added successfully!');", true);
            }
            catch (Exception ex)
            {
                ShowUOMMessage("Error adding UOM: " + ex.Message, false);
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                    "alert('Error adding UOM: " + ex.Message + "');", true);
            }
        }
        private bool IsUOMNameExists(string uomName)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
            string query = "SELECT COUNT(*) FROM UOM WHERE LOWER(UOMName) = LOWER(@UOMName)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UOMName", uomName);

                    try
                    {
                        conn.Open();
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or handle it as needed
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                            "alert('Error checking UOM name: " + ex.Message + "');", true);
                        return false;
                    }
                }
            }
        }
        private bool IsUOMCodeExists(string uomCode)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
            string query = "SELECT COUNT(*) FROM UOM WHERE LOWER(UOMCode) = LOWER(@UOMCode)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UOMCode", uomCode);

                    try
                    {
                        conn.Open();
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or handle it as needed
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                            "alert('Error checking UOM code: " + ex.Message + "');", true);
                        return false;
                    }
                }
            }
        }
        private void ShowUOMMessage(string message, bool isSuccess)
        {
            lblUOMMessage.Text = message;
            lblUOMMessage.CssClass = isSuccess ? "success-message" : "error-message";
            lblUOMMessage.Visible = true;
        }
        private void LoadUOMs()
        {
            try
            {
                DataTable dt = _cm.GetUOMs();
                ddlUnit.DataSource = dt;
                ddlUnit.DataTextField = "UOMName";
                ddlUnit.DataValueField = "UOMID";
                ddlUnit.DataBind();
                ddlUnit.Items.Insert(0, new ListItem("--Select UOM--", ""));
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Error loading UOMs: " + ex.Message + "');", true);
            }
        }

        protected void ddlCOA_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCOA.SelectedIndex != 0)
            {
                lblCOA.Text=ddlCOA.SelectedValue;
            }
        }
    }
}













