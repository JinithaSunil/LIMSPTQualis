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
    public partial class AddPurchaseGroup : System.Web.UI.Page
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
                if (Session["UserID"] != null)
                {
                    BindGrid();
                    BindItemType();
                    BindPurchaseGroup();
                    BindDisablePurchaseGroupDropdown();
                    BindDisableGroupDropdownForSubGroup();
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }
        public void BindGrid()
        {
            DataTable dt = _cm.GetPurchaseGroupDetails();
            if (dt.Rows.Count > 0)
            {
                gvPurchaseGroup.DataSource = dt;
                gvPurchaseGroup.DataBind();
                ViewState["dirState"] = dt;
                ViewState["sortdr"] = "Asc";
            }
        }
        public void BindItemType()
        {
            DataTable dt = _cm.GetItemType();
            if (dt.Rows.Count > 0)
            {
                ddlitemtype.DataSource = dt;
                ddlitemtype.DataTextField = "ItemType";
                ddlitemtype.DataValueField = "ItemTypeID";
                ddlitemtype.DataBind();
                ddlitemtype.Items.Insert(0, new ListItem("Select ItemType"));
            }
        }
        public void BindPurchaseGroup()
        {
            DataTable dt = _cm.GetPurchaseGroup();
            if (dt.Rows.Count > 0)
            {
                ddlpurchaseGroup.DataSource = dt;
                ddlpurchaseGroup.DataTextField = "PurchaseGroupName";
                ddlpurchaseGroup.DataValueField = "PurchaseGroupID";
                ddlpurchaseGroup.DataBind();
                ddlpurchaseGroup.Items.Insert(0, new ListItem("Select PurchaseGroup"));
            }
        }

        protected void btnadd_Click(object sender, EventArgs e)
        {
            int flag = 0;
            if (txtpurchasegroup.Text == "")
            {
                txtpurchasegroup.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else
            {
                txtpurchasegroup.BorderColor = System.Drawing.Color.Empty;
            }
            if (ddlitemtype.SelectedIndex <= 0)
            {
                ddlitemtype.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else
            {
                ddlitemtype.BorderColor = System.Drawing.Color.Empty;
            }
            if (flag == 0)
            {
                _cm.PurchaseGroupName = txtpurchasegroup.Text;
                _cm.ItemTypeID = Convert.ToInt32(ddlitemtype.SelectedItem.Value);
                DataTable dt = _cm.GetDuplicatePurchaseGroup(_cm);
                if (dt.Rows.Count > 0)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alert", "alert('This Purchase Group is already available');", true);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "refresh", "window.location.reload();", true);
                    txtpurchasegroup.Text = "";
                    ddlitemtype.ClearSelection();
                }
                else
                {
                    _cm.AddPurchaseGroup(_cm);
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alert", "alert('PurchaseGroup is sucessfully Added');", true);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "refresh", "window.location.reload();", true);
                    BindGrid();
                    txtpurchasegroup.Text = "";
                    ddlitemtype.ClearSelection();
                }

            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alert", "alert('Fill Highlighted Fields');", true);
            }
        }

        protected void ddlpurchaseGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            _cm.PurchaseGroupID = Convert.ToInt32(ddlpurchaseGroup.SelectedItem.Value);
            DataTable dt = _cm.GetItemTypeOnPurchaseGroup(_cm);
            if (dt.Rows.Count > 0)
            {
                divitemtype.Visible = true;

                ddlitemtypefrosubgroup.DataSource = dt;
                ddlitemtypefrosubgroup.DataTextField = "ItemType";
                ddlitemtypefrosubgroup.DataValueField = "ItemTypeID";
                ddlitemtypefrosubgroup.DataBind();
            }
        }

        protected void btnAddsubgroup_Click(object sender, EventArgs e)
        {
            int flag = 0;
            if (txtpurchasesubgroup.Text == "")
            {
                txtpurchasesubgroup.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else
            {
                txtpurchasesubgroup.BorderColor = System.Drawing.Color.Empty;
            }
            if (ddlpurchaseGroup.SelectedIndex <= 0)
            {
                ddlpurchaseGroup.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else
            {
                ddlpurchaseGroup.BorderColor = System.Drawing.Color.Empty;
            }
            if (flag == 0)
            {
                _cm.PurchaseGroupID = Convert.ToInt32(ddlpurchaseGroup.SelectedValue);
                _cm.ItemTypeID = Convert.ToInt32(ddlitemtypefrosubgroup.SelectedValue);
                _cm.PurchaseSubGroup = txtpurchasesubgroup.Text;
                DataTable dt = _cm.GetDuplicatePurchasesubGroup(_cm);
                if (dt.Rows.Count > 0)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alert", "alert('This Purchase SubGroup is already available');", true);
                    ddlitemtypefrosubgroup.ClearSelection();
                    divitemtype.Visible = false;
                    txtpurchasesubgroup.Text = "";
                    ddlpurchaseGroup.ClearSelection();
                }
                else
                {
                    _cm.AddPurchaseSubGroup(_cm);
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alert", "alert('PurchaseSubGroup is sucessfully Added');", true);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "refresh", "window.location.reload();", true);
                    //BindGrid();
                    ddlitemtypefrosubgroup.ClearSelection();
                    divitemtype.Visible = false;
                    txtpurchasesubgroup.Text = "";
                    ddlpurchaseGroup.ClearSelection();
                }
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alert", "alert('Fill Highlighted Fields');", true);
            }
        }

        protected void gvPurchaseGroup_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable dtrslt = (DataTable)ViewState["dirState"];
            if (dtrslt.Rows.Count > 0)
            {
                if (Convert.ToString(ViewState["sortdr"]) == "Asc")
                {
                    dtrslt.DefaultView.Sort = e.SortExpression + " Desc";
                    ViewState["sortdr"] = "Desc";
                }
                else
                {
                    dtrslt.DefaultView.Sort = e.SortExpression + " Asc";
                    ViewState["sortdr"] = "Asc";
                }
                gvPurchaseGroup.DataSource = dtrslt;
                gvPurchaseGroup.DataBind();

                // lblTotalCount.Text = "Total Count: " + dtrslt.Rows.Count.ToString();
            }

        }
        protected void gvPurchaseGroup_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvPurchaseGroup.PageIndex = e.NewPageIndex;
            BindGrid();
        }
        public void BindDisablePurchaseGroupDropdown()
        {
            DataTable dt = _cm.GetAllPurchaseGroups();
            if (dt.Rows.Count > 0)
            {
                ddlDisablePurchaseGroup.DataSource = dt;
                ddlDisablePurchaseGroup.DataTextField = "PurchaseGroupName";
                ddlDisablePurchaseGroup.DataValueField = "PurchaseGroupID";
                ddlDisablePurchaseGroup.DataBind();
                ddlDisablePurchaseGroup.Items.Insert(0, new ListItem("Select PurchaseGroup", "0"));
            }
        }

        public void BindDisableGroupDropdownForSubGroup()
        {
            DataTable dt = _cm.GetAllPurchaseGroups();
            if (dt.Rows.Count > 0)
            {
                ddlDisableSelectGroup.DataSource = dt;
                ddlDisableSelectGroup.DataTextField = "PurchaseGroupName";
                ddlDisableSelectGroup.DataValueField = "PurchaseGroupID";
                ddlDisableSelectGroup.DataBind();
                ddlDisableSelectGroup.Items.Insert(0, new ListItem("Select PurchaseGroup", "0"));
            }
        }

        protected void ddlDisableSelectGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlDisableSelectGroup.SelectedIndex > 0)
            {
                int purchaseGroupID = Convert.ToInt32(ddlDisableSelectGroup.SelectedValue);
                DataTable dt = _cm.GetPurchaseSubGroupsByGroup(purchaseGroupID);
                if (dt.Rows.Count > 0)
                {
                    ddlDisablePurchaseSubGroup.DataSource = dt;
                    ddlDisablePurchaseSubGroup.DataTextField = "PurchaseSubGroupName";
                    ddlDisablePurchaseSubGroup.DataValueField = "PurchaseSubGroupID";
                    ddlDisablePurchaseSubGroup.DataBind();
                    ddlDisablePurchaseSubGroup.Items.Insert(0, new ListItem("Select SubGroup", "0"));
                }
                else
                {
                    ddlDisablePurchaseSubGroup.Items.Clear();
                    ddlDisablePurchaseSubGroup.Items.Insert(0, new ListItem("No SubGroups Available", "0"));
                }
            }
        }

        protected void btnDisablePurchaseGroup_Click(object sender, EventArgs e)
        {
            if (ddlDisablePurchaseGroup.SelectedIndex > 0)
            {
                int purchaseGroupID = Convert.ToInt32(ddlDisablePurchaseGroup.SelectedValue);

                // Get current status before toggling
                DataTable dt = _cm.GetAllPurchaseGroups();
                DataRow[] rows = dt.Select("PurchaseGroupID = " + purchaseGroupID);
                int currentStatus = Convert.ToInt32(rows[0]["IsDisabled"]);

                // Toggle the status
                _cm.TogglePurchaseGroupStatus(purchaseGroupID);

                // Clear message based on what action was performed
                string message = "";
                if (currentStatus == 0) // Was Enabled, now Disabled
                {
                    //message = "Purchase Group has been DISABLED successfully (Status set to 1)";
                    message = "Purchase Group has been DISABLED successfully";
                }
                else // Was Disabled, now Enabled
                {
                    //message = "Purchase Group has been ENABLED successfully (Status set to 0)";
                    message = "Purchase Group has been ENABLED successfully";
                }

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alert", $"alert('{message}');", true);

                BindGrid();
                BindDisablePurchaseGroupDropdown();
                BindPurchaseGroup();
                BindDisableGroupDropdownForSubGroup();
                ddlDisablePurchaseGroup.SelectedIndex = 0;
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alert", "alert('Please select a Purchase Group');", true);
            }
        }

        protected void btnDisablePurchaseSubGroup_Click(object sender, EventArgs e)
        {
            if (ddlDisablePurchaseSubGroup.SelectedIndex > 0)
            {
                int purchaseSubGroupID = Convert.ToInt32(ddlDisablePurchaseSubGroup.SelectedValue);
                int purchaseGroupID = Convert.ToInt32(ddlDisableSelectGroup.SelectedValue);

                // Get current status before toggling
                DataTable dt = _cm.GetPurchaseSubGroupsByGroup(purchaseGroupID);
                DataRow[] rows = dt.Select("PurchaseSubGroupID = " + purchaseSubGroupID);
                int currentStatus = Convert.ToInt32(rows[0]["IsDisabled"]);

                // Toggle the status
                _cm.TogglePurchaseSubGroupStatus(purchaseSubGroupID);

                // Clear message based on what action was performed
                string message = "";
                if (currentStatus == 0) // Was Enabled, now Disabled
                {
                    //message = "Purchase SubGroup has been DISABLED successfully (Status set to 1)";

                    message = "Purchase SubGroup has been DISABLED successfully";
                }
                else // Was Disabled, now Enabled
                {
                    //message = "Purchase SubGroup has been ENABLED successfully (Status set to 0)";

                    message = "Purchase SubGroup has been ENABLED successfully";
                }

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alert", $"alert('{message}');", true);

                BindGrid();
                ddlDisablePurchaseSubGroup.SelectedIndex = 0;
                ddlDisableSelectGroup.SelectedIndex = 0;
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alert", "alert('Please select a Purchase SubGroup');", true);
            }
        }

    }
}