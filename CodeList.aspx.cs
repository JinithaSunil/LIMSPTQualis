using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LIMS.DAL;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace LIMS
{   
    public partial class CodeList : System.Web.UI.Page
    {
    
        LIMS.DAL.CM _cm = new LIMS.DAL.CM();
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ToString());

        string date = DateTime.Today.ToString("dd-MMM-yyyy");
        string time = DateTime.Now.ToString("h:mm:ss tt");

        string CreateDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string UpdateDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        string constr = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    //BindUserDetails(Session["UserID"].ToString());
                    BindItemType();

                    if (Session["ItemType"] != null)
                    {
                        BindItemsByCode(Session["ItemType"].ToString());
                        rblStoreType.Items.FindByValue(Session["ItemType"].ToString()).Selected = true;
                        //  rblStoreType.Enabled = false;
                        string ItemTypeID = rblStoreType.SelectedValue.ToString();
                        BindPurchaseGroup(ItemTypeID);
                    }
                }
                else
                {
                    Response.Redirect("Login.aspx");
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
       
        private void BindItemType()
        {
            DataTable dt = _cm.GetItemType();
            if (dt.Rows.Count > 0)
            {
                rblStoreType.DataSource = dt;
                rblStoreType.DataTextField = "ItemType";
                rblStoreType.DataValueField = "ItemTypeID";
                rblStoreType.DataBind();
                rblStoreType.Enabled = true;
            }
        }
        protected void BindPurchaseGroup(string Type)
        {
            DataTable dt = _cm.getPurchaseGroups(Type);
            ddlPurchaseGroup.DataSource = dt;
            ddlPurchaseGroup.DataTextField = "PurchaseGroupName";
            ddlPurchaseGroup.DataValueField = "PurchaseGroupID";
            ddlPurchaseGroup.DataBind();
            ddlPurchaseGroup.Items.Insert(0, "Select Purchase Group");
        }
        protected void BindPurchaseSubGroup(string PurchaseGroupID)
        {
            DataTable dt = _cm.getPurchaseSubGroups(PurchaseGroupID);
            ddlPurchaseSubGroup.DataSource = dt;
            ddlPurchaseSubGroup.DataTextField = "PurchaseSubGroupName";
            ddlPurchaseSubGroup.DataValueField = "PurchaseSubGroupID";
            ddlPurchaseSubGroup.DataBind();
            ddlPurchaseSubGroup.Items.Insert(0, "Select Purchase Sub Group");
        }
        private void BindItemsByCode(string search)
        {
            DataTable dt = _cm.GetPurchaseItemDetailswithcode(search);
            if (dt.Rows.Count > 0)
            {
                gvItemList.DataSource = dt;
                gvItemList.DataBind();
            }
            else
            {
                gvItemList.DataSource = null;
                gvItemList.DataBind();
            }
        }
        private void BindItemsByDescription(string search)
        {
            DataTable dt = _cm.GetItemsByDescriptionwithcode(search);
            if (dt.Rows.Count > 0)
            {
                gvItemList.DataSource = dt;
                gvItemList.DataBind();
            }
            else
            {
                gvItemList.DataSource = null;
                gvItemList.DataBind();
            }
        }

        protected void imgSearch_Click(object sender, ImageClickEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                txtSearch.BorderColor = System.Drawing.Color.Empty;
                BindItemsByCode(txtSearch.Text.Trim());
            }
            else if (!string.IsNullOrEmpty(txtSearchText.Text))
            {
                txtSearchText.BorderColor = System.Drawing.Color.Empty;
                BindItemsByDescription(txtSearchText.Text.Trim());
            }
            else
            {
                txtSearch.BorderColor = System.Drawing.Color.Red;
                txtSearchText.BorderColor = System.Drawing.Color.Red;
            }
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                txtSearch.BorderColor = System.Drawing.Color.Empty;
                BindItemsByCode(txtSearch.Text.Trim());
            }
            else
            {
                txtSearch.BorderColor = System.Drawing.Color.Red;
            }
        }

        protected void txtSearchText_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSearchText.Text))
            {
                txtSearchText.BorderColor = System.Drawing.Color.Empty;
                BindItemsByDescription(txtSearchText.Text.Trim());
            }
            else
            {
                txtSearchText.BorderColor = System.Drawing.Color.Red;
            }
        }

        protected void gvItemList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                int rowIndex = int.Parse(e.CommandArgument.ToString());
                int ItemID = (int)this.gvItemList.DataKeys[rowIndex]["ItemID"];

                Label txtItemID = (Label)gvItemList.Rows[rowIndex].FindControl("txtItemID");
                Label lblAvailableStock = (Label)gvItemList.Rows[rowIndex].FindControl("lblAvailableStock");
                //Label lblItemType = (Label)gvStockList.Rows[rowIndex].FindControl("lblItemType");

                DataTable dt = _cm.GetItemDetails(txtItemID.Text.ToString());
                if (dt.Rows.Count > 0)
                {
                    _cm.ItemCode = dt.Rows[0]["ItemCode"].ToString();
                    _cm.ItemType = dt.Rows[0]["ItemTypeID"].ToString();
                     
                }
                Session["ItemID"] = _cm.ItemCode;
                Session["ItemType"] = _cm.ItemType;

                if (Request.QueryString["Type"] != null)
                {
                    string RequestType = Request.QueryString["Type"].ToString();
                    switch (RequestType)
                    {
                        case "PR":
                            Response.Redirect("PurchaseRequisitionForm.aspx");
                            break;
                        case "MR":
                            if (Convert.ToDecimal(lblAvailableStock.Text) <= 0 || lblAvailableStock.Text == "")
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Stock not Available!..');", true);
                            }
                            else
                            {
                                Session["AvailableStock"] = lblAvailableStock.Text;
                                Response.Redirect("MaterialRequisition.aspx");
                            }
                            break;
                        case "AMC":
                            if (Session["AMCID"] != null)
                            {
                                Response.Redirect("AddAMC.aspx?ID=" + Session["AMCID"].ToString());
                            }
                            else
                            {
                                Response.Redirect("AddAMC.aspx");
                            }
                            break;
                         
                    }
                }

            }
        }

        protected void rblStoreType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ItemTypeID = rblStoreType.SelectedValue.ToString();
            BindPurchaseGroup(ItemTypeID);
            DataTable dt = null;
            gvItemList.DataSource = dt;
            gvItemList.DataBind();
        }

        protected void ddlPurchaseGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            string PurchaseGroupID = ddlPurchaseGroup.SelectedValue;
            BindPurchaseSubGroup(PurchaseGroupID);
        }

        protected void ddlPurchaseSubGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindItemsByPurchaseSubGroup(ddlPurchaseSubGroup.SelectedValue);
        }



        private void BindItemsByPurchaseSubGroup(string purchaseSubGroupId)
        {
            DataTable dt = _cm.GetItemsByPurchaseSubGroup(purchaseSubGroupId);  // New method to create
            if (dt.Rows.Count > 0)
            {
                gvItemList.DataSource = dt;
                gvItemList.DataBind();
            }
            else
            {
                gvItemList.DataSource = null;
                gvItemList.DataBind();
            }
        }
    }





}

