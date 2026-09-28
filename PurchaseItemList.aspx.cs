using DocumentFormat.OpenXml.Spreadsheet;
using LIMS.DAL;
using Syncfusion.Pdf;
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

namespace LIMS
{
    public partial class PurchaseItemList : System.Web.UI.Page
    {

        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ToString());

        string date = DateTime.Today.ToString("dd-MMM-yyyy");
        string time = DateTime.Now.ToString("h:mm:ss tt");

        string CreateDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string UpdateDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        LIMS.DAL.CM _cm = new LIMS.DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindPurchaseSubGroup();
                BindTopItems(0);

            }
        }
        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvPurchaseItem.PageIndex = 0; 
            BindTopItems(0);
        }
        private void BindTopItems(int index)
        {
            int PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
            gvPurchaseItem.PageSize = PageSize;

            int StartIndex = index * PageSize;   // important for OFFSET

            DataTable dt = _cm.GetTopItems(StartIndex, PageSize);

            if (dt.Rows.Count > 0)
            {
                gvPurchaseItem.VirtualItemCount = Convert.ToInt32(dt.Rows[0]["TotalRecords"]);

                gvPurchaseItem.DataSource = dt;
                gvPurchaseItem.DataBind();
                spanCount.InnerHtml = dt.Rows[0]["TotalRecords"].ToString();
            }
            else
            {
                gvPurchaseItem.DataSource = null;
                gvPurchaseItem.DataBind();
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
        private void BindPurchaseSubGroup()
        {
            DataTable dt = _cm.FillParentTable();
            if (dt.Rows.Count > 0)
            {
                ddlPurchaseGroup.DataSource = dt;
                ddlPurchaseGroup.DataTextField = "PurchaseGroupName";
                ddlPurchaseGroup.DataValueField = "PurchaseGroupID";
                ddlPurchaseGroup.DataBind();
                ddlPurchaseGroup.Items.Insert(0, new ListItem("Select PurchaseGroup"));
            }
        }
        private void BindItemsByCode(string search)
        {
            DataTable dt = _cm.GetPurchaseItemDetailsByCode(search);
            if (dt.Rows.Count > 0)
            {
                gvPurchaseItem.DataSource = dt;
                gvPurchaseItem.DataBind();
            }
            else
            {
                gvPurchaseItem.DataSource = null;
                gvPurchaseItem.DataBind();
            }
        }
        protected void gvPurchaseItem_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "update")
            {
                int rowIndex = int.Parse(e.CommandArgument.ToString());
                int ItemID = (int)this.gvPurchaseItem.DataKeys[rowIndex]["ItemID"];

                TextBox txtItemDescription = (TextBox)gvPurchaseItem.Rows[rowIndex].Cells[2].FindControl("txtDescription");
                TextBox txtHSNCode = (TextBox)gvPurchaseItem.Rows[rowIndex].Cells[5].FindControl("txtHSNCode");
                // TextBox txtMake = (TextBox)gvPurchaseItem.Rows[rowIndex].Cells[6].FindControl("txtMake");
                TextBox txtLeadTime = (TextBox)gvPurchaseItem.Rows[rowIndex].Cells[7].FindControl("txtLeadTime");
                TextBox txtModel = (TextBox)gvPurchaseItem.Rows[rowIndex].Cells[8].FindControl("txtModel");

                _cm.ItemID = ItemID;
                _cm.ItemDescription = txtItemDescription.Text.ToString();
                _cm.HSNCode = txtHSNCode.Text.ToString();
                //  _cm.Make = txtMake.Text.ToString();
                _cm.LeadTime = Convert.ToInt32(txtLeadTime.Text.ToString());
                _cm.ModelNo = txtModel.Text.ToString();
                _cm.UpdateItems(_cm);
                //BindItems();


            }
            if (e.CommandName == "disable")
            {
                int rowIndex = int.Parse(e.CommandArgument.ToString());
                int ItemID = (int)this.gvPurchaseItem.DataKeys[rowIndex]["ItemID"];

                _cm.ItemID = ItemID;
                _cm.DisableItem = true;
                _cm.UpdateItemDisabled(_cm);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "refresh", "window.location.reload();", true);

            }
            if (e.CommandName == "enable")
            {
                int rowIndex = int.Parse(e.CommandArgument.ToString());
                int ItemID = (int)this.gvPurchaseItem.DataKeys[rowIndex]["ItemID"];

                _cm.ItemID = ItemID;
                _cm.DisableItem = false;
                _cm.UpdateItemDisabled(_cm);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "refresh", "window.location.reload();", true);

            }
        }
        protected void gvPurchaseItem_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblDisabled = (Label)e.Row.FindControl("lblDisabled");
                Button btnEnable = (Button)e.Row.FindControl("btnEnable");
                Button btnDisable = (Button)e.Row.FindControl("btnDisable");
                if (lblDisabled.Text == "True")
                {
                    btnEnable.Visible = true;
                    btnDisable.Visible = false;
                }
                else
                {
                    btnEnable.Visible = false;
                    btnDisable.Visible = true;
                }
                //23Sep26 - blocking disable option
                btnDisable.Visible = false;
            }
        }
        protected void gvPurchaseItem_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

        }
        private void BindItemsByDescription(string Search)
        {
            DataTable dt = _cm.GetItemsByDescription(Search);
            if (dt.Rows.Count > 0)
            {
                gvPurchaseItem.DataSource = dt;
                gvPurchaseItem.DataBind();
            }
            else
            {
                {
                    gvPurchaseItem.DataSource = null;
                    gvPurchaseItem.DataBind();
                }
            }
        }
       
        private void BindItems(string PurchaseGroup, string search)
        {
            DataTable dt = _cm.GetPurchaseItems(PurchaseGroup, search);
            if (dt.Rows.Count > 0)
            {
                gvPurchaseItem.DataSource = dt;
                gvPurchaseItem.DataBind();
            }
            else
            {
                gvPurchaseItem.DataSource = null;
                gvPurchaseItem.DataBind();
            }
        }
       
        protected void imgSearch_Click(object sender, ImageClickEventArgs e)
        {
            int flag = 0;
            string search = txtSearch.Text;
            if (search == "")
            {
                txtSearch.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else
            {
                txtSearch.BorderColor = System.Drawing.Color.Empty;
            }
            if (ddlPurchaseGroup.SelectedIndex == 0)
            {
                ddlPurchaseGroup.BorderColor = System.Drawing.Color.Red;
                BindItemsByCode(search);
            }
            else
            {
                ddlPurchaseGroup.BorderColor = System.Drawing.Color.Empty;
                string PurchaseGroup = ddlPurchaseGroup.SelectedValue.ToString();
                BindItems(PurchaseGroup, search);
            }
        }
        protected void ddlPurchaseGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlPurchaseGroup.SelectedIndex != 0)
            {
                //txtSearch.Visible = true;
                //  imgSearch.Visible = true;
                //   txtSearch.Text = "";
                string PurchaseGroup = ddlPurchaseGroup.SelectedValue.ToString();
                BindItems(PurchaseGroup);
            }
            else
            {
                //txtSearch.Visible = false;
                //imgSearch.Visible = false;
            }
        }
        protected void txtSearchText_TextChanged(object sender, EventArgs e)
        {
            int flag = 0;
            string search = txtSearchText.Text;
            if (search == "")
            {
                txtSearchText.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else
            {
                txtSearchText.BorderColor = System.Drawing.Color.Empty;
            }

            if (flag == 0)
            {
                BindItemsByDescription(txtSearchText.Text);
            }
        }
        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            int flag = 1;
            string search = txtSearch.Text;
            if (search == "")
            {
                txtSearch.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else
            {
                txtSearch.BorderColor = System.Drawing.Color.Empty;
            }
            if (ddlPurchaseGroup.SelectedIndex == 0)
            {
                ddlPurchaseGroup.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else
            {
                ddlPurchaseGroup.BorderColor = System.Drawing.Color.Empty;
            }
            if (flag == 0)
            {
                string PurchaseSubGroup = ddlPurchaseGroup.SelectedValue.ToString();
                BindItems(PurchaseSubGroup, search);
            }
        }
        private void BindItems(string PurchaseGroup)
        {
            DataTable dt = _cm.GetPurchaseItemDetails(PurchaseGroup);
            if (dt.Rows.Count > 0)
            {
                gvPurchaseItem.DataSource = dt;
                gvPurchaseItem.DataBind();
            }
            else
            {
                gvPurchaseItem.DataSource = null;
                gvPurchaseItem.DataBind();
            }
        }

        protected void gvPurchaseItem_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvPurchaseItem.PageIndex = e.NewPageIndex;
            BindTopItems(e.NewPageIndex);
        }
    }
}