using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace LIMS
{
    public partial class EditPurchasegroupAndSubgroup : System.Web.UI.Page
    {

        LIMS.DAL.CM _cm = new LIMS.DAL.CM();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    if (Request.QueryString["PurchaseGroupID"] != null)
                    {
                        string purchaseGroupID = Request.QueryString["PurchaseGroupID"];
                        string purchaseSubGroupID = Request.QueryString["PurchaseSubGroupID"];

                        LoadPurchaseGroupDetails(purchaseGroupID);
                        LoadSubGroups(purchaseGroupID);
                        BindItemType();

                        // Optionally highlight the specific subgroup if needed
                        if (!string.IsNullOrEmpty(purchaseSubGroupID))
                        {
                            // You can use this to scroll to or highlight a specific subgroup
                            ViewState["SelectedSubGroupID"] = purchaseSubGroupID;
                        }
                    }
                    else
                    {
                        Response.Redirect("AddPurchaseGroup.aspx");
                    }
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }

        private void BindItemType()
        {
            DataTable dt = _cm.GetItemType();
            if (dt.Rows.Count > 0)
            {
                ddlItemType.DataSource = dt;
                ddlItemType.DataTextField = "ItemType";
                ddlItemType.DataValueField = "ItemTypeID";
                ddlItemType.DataBind();
                ddlItemType.Items.Insert(0, new ListItem("Select ItemType", "0"));
            }
        }

        private void LoadPurchaseGroupDetails(string purchaseGroupID)
        {
            DataTable dt = _cm.GetPurchaseGroupByID(purchaseGroupID);
            if (dt.Rows.Count > 0)
            {
                txtPurchaseGroupID.Text = dt.Rows[0]["PurchaseGroupID"].ToString();
                txtPurchaseGroupName.Text = dt.Rows[0]["PurchaseGroupName"].ToString();

                // Set Item Type dropdown
                if (dt.Rows[0]["ItemTypeID"] != DBNull.Value)
                {
                    ddlItemType.SelectedValue = dt.Rows[0]["ItemTypeID"].ToString();
                }
            }
        }

        private void LoadSubGroups(string purchaseGroupID)
        {
            DataTable dt = _cm.GetSubGroupsByPurchaseGroupID(purchaseGroupID);
            if (dt.Rows.Count > 0)
            {
                gvSubGroups.DataSource = dt;
                gvSubGroups.DataBind();
            }
            else
            {
                gvSubGroups.DataSource = null;
                gvSubGroups.DataBind();
            }
        }

        protected void btnUpdateGroup_Click(object sender, EventArgs e)
        {
            int flag = 0;

            // Validation
            if (string.IsNullOrWhiteSpace(txtPurchaseGroupName.Text))
            {
                txtPurchaseGroupName.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else
            {
                txtPurchaseGroupName.BorderColor = System.Drawing.Color.Empty;
            }

            if (ddlItemType.SelectedIndex <= 0)
            {
                ddlItemType.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else
            {
                ddlItemType.BorderColor = System.Drawing.Color.Empty;
            }

            if (flag == 0)
            {
                try
                {
                    _cm.PurchaseGroupID = Convert.ToInt32(txtPurchaseGroupID.Text);
                    _cm.PurchaseGroupName = txtPurchaseGroupName.Text.Trim();
                    _cm.ItemTypeID = Convert.ToInt32(ddlItemType.SelectedValue);

                    // Check for duplicate
                    DataTable dtDuplicate = _cm.CheckDuplicatePurchaseGroupForEdit(_cm);
                    if (dtDuplicate.Rows.Count > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                            "alert('Purchase Group name already exists!');", true);
                        return;
                    }

                    // Update Purchase Group
                    int result = _cm.UpdatePurchaseGroup(_cm);
                    if (result > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                            "alert('Purchase Group updated successfully!'); window.location='PurchaseGroup.aspx';", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                            "alert('Failed to update Purchase Group!');", true);
                    }
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                        "alert('Error: " + ex.Message + "');", true);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    "alert('Please fill all required fields!');", true);
            }
        }

        protected void btnCancelGroup_Click(object sender, EventArgs e)
        {
            Response.Redirect("PurchaseGroup.aspx");
        }

        protected void gvSubGroups_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvSubGroups.EditIndex = e.NewEditIndex;
            LoadSubGroups(txtPurchaseGroupID.Text);
        }

        protected void gvSubGroups_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvSubGroups.EditIndex = -1;
            LoadSubGroups(txtPurchaseGroupID.Text);
        }

        protected void gvSubGroups_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            {
                try
                {
                    GridViewRow row = gvSubGroups.Rows[e.RowIndex];
                    int subGroupID = Convert.ToInt32(gvSubGroups.DataKeys[e.RowIndex].Value);
                    TextBox txtSubGroupName = (TextBox)row.FindControl("txtSubGroupName");

                    if (string.IsNullOrWhiteSpace(txtSubGroupName.Text))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                            "alert('SubGroup name cannot be empty!');", true);
                        return;
                    }

                    _cm.PurchaseSubGroupID = subGroupID;
                    _cm.PurchaseSubGroup = txtSubGroupName.Text.Trim();

                    // Check for duplicate
                    DataTable dtDuplicate = _cm.CheckDuplicateSubGroupForEdit(_cm);
                    if (dtDuplicate.Rows.Count > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                            "alert('SubGroup name already exists!');", true);
                        return;
                    }

                    int result = _cm.UpdatePurchaseSubGroup(_cm);
                    if (result > 0)
                    {
                        gvSubGroups.EditIndex = -1;
                        LoadSubGroups(txtPurchaseGroupID.Text);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                            "alert('SubGroup updated successfully!');", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                            "alert('Failed to update SubGroup!');", true);
                    }
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                        "alert('Error: " + ex.Message + "');", true);
                }
            }
        }
        protected void gvSubGroups_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int subGroupID = Convert.ToInt32(gvSubGroups.DataKeys[e.RowIndex].Value);

                // Check if subgroup is being used
                DataTable dtCheck = _cm.CheckSubGroupInUse(subGroupID);
                if (dtCheck.Rows.Count > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                        "alert('Cannot delete! This SubGroup is being used in purchase items.');", true);
                    return;
                }

                int result = _cm.DeletePurchaseSubGroup(subGroupID);
                if (result > 0)
                {
                    LoadSubGroups(txtPurchaseGroupID.Text);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                        "alert('SubGroup deleted successfully!');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                        "alert('Failed to delete SubGroup!');", true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    "alert('Error: " + ex.Message + "');", true);
            }
        }

        //protected void btnAddSubGroup_Click(object sender, EventArgs e)
        //{
        //    if (string.IsNullOrWhiteSpace(txtNewSubGroupName.Text))
        //    {
        //        txtNewSubGroupName.BorderColor = System.Drawing.Color.Red;
        //        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
        //            "alert('Please enter SubGroup name!');", true);
        //        return;
        //    }

        //    try
        //    {
        //        _cm.PurchaseGroupID = Convert.ToInt32(txtPurchaseGroupID.Text);
        //        _cm.ItemTypeID = Convert.ToInt32(ddlItemType.SelectedValue);
        //        _cm.PurchaseSubGroup = txtNewSubGroupName.Text.Trim();

        //        // Check for duplicate
        //        DataTable dtDuplicate = _cm.GetDuplicatePurchasesubGroup(_cm);
        //        if (dtDuplicate.Rows.Count > 0)
        //        {
        //            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
        //                "alert('This SubGroup name already exists!');", true);
        //            txtNewSubGroupName.Text = "";
        //            return;
        //        }

        //        _cm.AddPurchaseSubGroup(_cm);
        //        LoadSubGroups(txtPurchaseGroupID.Text);
        //        txtNewSubGroupName.Text = "";
        //        txtNewSubGroupName.BorderColor = System.Drawing.Color.Empty;

        //        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
        //            "alert('SubGroup added successfully!');", true);
        //    }
        //    catch (Exception ex)
        //    {
        //        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
        //            "alert('Error: " + ex.Message + "');", true);
        //    }
        //}
    }
}