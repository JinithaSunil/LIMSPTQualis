using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class PurchaseGroupManagement : System.Web.UI.Page
    {
        LIMS.DAL.CM _cm = new LIMS.DAL.CM();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserID"] == null)
                    Response.Redirect("Login.aspx");

                BindGrid();
                BindItemType();
                BindPurchaseGroup();
            }
        }

        private void BindGrid()
        {
            DataTable dt = _cm.GetPurchaseGroupDetails();
            gvPurchase.DataSource = dt;
            gvPurchase.DataBind();
        }

        private void BindItemType()
        {
            DataTable dt = _cm.GetItemType();
            ddlItemType.DataSource = dt;
            ddlItemType.DataTextField = "ItemType";
            ddlItemType.DataValueField = "ItemTypeID";
            ddlItemType.DataBind();
            ddlItemType.Items.Insert(0, new ListItem("Select Item Type", "0"));
        }

        private void BindPurchaseGroup()
        {
            DataTable dt = _cm.GetPurchaseGroup();
            ddlPurchaseGroup.DataSource = dt;
            ddlPurchaseGroup.DataTextField = "PurchaseGroupName";
            ddlPurchaseGroup.DataValueField = "PurchaseGroupID";
            ddlPurchaseGroup.DataBind();
            ddlPurchaseGroup.Items.Insert(0, new ListItem("Select Purchase Group", "0"));
        }

        protected void btnSaveGroup_Click(object sender, EventArgs e)
        {
            if (txtPurchaseGroup.Text.Trim() == "" || ddlItemType.SelectedIndex == 0)
            {
                ShowMessage("Please fill all fields.");
                return;
            }

            _cm.PurchaseGroupName = txtPurchaseGroup.Text.Trim();
            _cm.ItemTypeID = Convert.ToInt32(ddlItemType.SelectedValue);

            _cm.AddPurchaseGroup(_cm);

            ShowMessage("Purchase Group saved successfully.");
            ClearFields();
            BindGrid();
            BindPurchaseGroup();
        }

        protected void btnSaveSubGroup_Click(object sender, EventArgs e)
        {
            if (ddlPurchaseGroup.SelectedIndex == 0 || txtSubGroup.Text.Trim() == "")
            {
                ShowMessage("Please fill all fields.");
                return;
            }

            _cm.PurchaseGroupID = Convert.ToInt32(ddlPurchaseGroup.SelectedValue);
            _cm.PurchaseSubGroup = txtSubGroup.Text.Trim();

            _cm.AddPurchaseSubGroup(_cm);

            ShowMessage("Purchase Sub Group saved successfully.");
            ClearFields();
            BindGrid();
        }

        protected void gvPurchase_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Toggle")
            {
                int index = Convert.ToInt32(e.CommandArgument);

                int groupID = Convert.ToInt32(gvPurchase.DataKeys[index]["PurchaseGroupID"]);
                int subGroupID = Convert.ToInt32(gvPurchase.DataKeys[index]["PurchaseSubGroupID"]);

                if (subGroupID > 0)
                    _cm.TogglePurchaseSubGroupStatus(subGroupID);
                else
                    _cm.TogglePurchaseGroupStatus(groupID);

                BindGrid();
                ShowMessage("Status updated successfully.");
            }
        }

        protected void ddlPurchaseGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Optional future enhancement
        }

        private void ClearFields()
        {
            txtPurchaseGroup.Text = "";
            txtSubGroup.Text = "";
            ddlItemType.SelectedIndex = 0;
            ddlPurchaseGroup.SelectedIndex = 0;
        }

        private void ShowMessage(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(),
                "alert", $"alert('{message}');", true);
        }
    }
}