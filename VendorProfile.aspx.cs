using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class VendorProfile : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                if (!Page.IsPostBack)
                {
                    if (Session["Usertype"] != null)
                    {

                        if (Request.QueryString["ID"] != null)
                        {
                            divDocuments.Visible = true;
                            divEditDocuments.Visible = false;
                            lnkUpdateDocuments.Visible = false;
                            lnkEditDocuments.Visible = true;
                            hdnTAB.Value = "#home";

                            string vendorCode = Request.QueryString["ID"].ToString();
                            DataTable dt = _cm.GetVendorCode(vendorCode);
                            int vendorID = Convert.ToInt32(dt.Rows[0]["vendorID"].ToString());
                            lblVendorID.Text = vendorID.ToString();
                            Session["vendorID"] = vendorID;
                            BindVendorDetails(vendorCode);
                            BindVendorAddress(vendorCode);
                            BindVendorContact(vendorCode);
                            BindVendorDocuments(vendorID);
                            PopulateTreeview();
                            BindTreeView(vendorID);
                            BindDocumentTypes();
                        }
                        else
                        {
                            Response.Redirect("VendorList.aspx");
                        }
                    }
                    else
                    {
                        Response.Redirect("Login.aspx");
                    }
                }
            }
        }

        protected void BindVendorDetails(string vendorCode)
        {
            if (lblVendorID.Text != "")
            {
                int vendorID = Convert.ToInt32(lblVendorID.Text);
                DataTable dtVendor = _cm.getVendorDetails(vendorCode);
                if (dtVendor.Rows.Count > 0)
                {
                    lblVendorName.Text = dtVendor.Rows[0]["VendorName"].ToString();
                    lblBusinessType.Text = dtVendor.Rows[0]["TypeOfBusiness"].ToString();
                    lblPAN.Text = dtVendor.Rows[0]["PANNumber"].ToString();

                    int enquiryCount = _cm.GetVendorEnquiryCount(vendorID);

                    btnEditVendorName.Visible = enquiryCount == 0;
                }
            }           
        }
        protected void btnSaveVendorName_Click(object sender, EventArgs e)
        {
            // Important: check again before UPDATE
            if (lblVendorID.Text != "")
            {
                int vendorID = Convert.ToInt32(lblVendorID.Text);
                // Recheck on the server
                if (_cm.GetVendorEnquiryCount(vendorID) > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Vendor name cannot be changed because enquiries already exist for this vendor.');", true);
                    return;
                }


                string vendorName = txtVendorName.Text.Trim();

                if (string.IsNullOrWhiteSpace(vendorName))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Vendor name cannot be blank.');", true);
                    return;
                }

                _cm.UpdateVendorName(vendorID, vendorName);

                lblVendorName.Text = vendorName;

                pnlVendorNameEdit.Visible = false;
                lblVendorName.Visible = true;

                btnEditVendorName.Visible = true;
            }
        }
        protected void btnCancelVendorName_Click(object sender, EventArgs e)
        {
            pnlVendorNameEdit.Visible = false;
            lblVendorName.Visible = true;
            btnEditVendorName.Visible = true;
        }
        protected void btnEditVendorName_Click(object sender, EventArgs e)
        {
            if (lblVendorID.Text != "")
            {
                int vendorID = Convert.ToInt32(lblVendorID.Text);
                // Recheck on the server
                if (_cm.GetVendorEnquiryCount(vendorID) > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Vendor name cannot be changed because enquiries already exist for this vendor.');", true);
                    return;
                }

                txtVendorName.Text = lblVendorName.Text;

                pnlVendorNameEdit.Visible = true;
                btnEditVendorName.Visible = false;
                lblVendorName.Visible = false;
            }
        }
        protected void BindVendorAddress(string vendorCode)
        {
            DataTable dtVendorAddress = _cm.getVendorAddress(vendorCode);
            if (dtVendorAddress.Rows.Count > 0)
            {
                liAddress.DataSource = null;
                liAddress.DataBind();
                liAddress.DataSource = dtVendorAddress;
                liAddress.DataBind();
            }
        }

        protected void BindVendorContact(string vendorCode)
        {
            DataTable dtVendorContact = _cm.getVendorContact(vendorCode);
            if (dtVendorContact.Rows.Count > 0)
            {
                liContact.DataSource = null;
                liContact.DataBind();
                liContact.DataSource = dtVendorContact;
                liContact.DataBind();
            }
        }

        private void BindCountry(DropDownList ddlCountry)
        {
            DataTable dt = _cm.GetCountryDetails();
            if (dt.Rows.Count > 0)
            {
                ddlCountry.DataSource = dt;
                ddlCountry.DataTextField = "CountryName";
                ddlCountry.DataValueField = "CountryID";
                ddlCountry.DataBind();
                ddlCountry.Items.Insert(0, new ListItem("Select Country"));
            }
        }

        private void BindDocumentTypes()
        {
            DataTable dt = _cm.GetDocumentDetails();
            if (dt.Rows.Count > 0)
            {
                ddlDocumentType.DataSource = dt;
                ddlDocumentType.DataTextField = "DocumentType";
                ddlDocumentType.DataValueField = "DocumentID";
                ddlDocumentType.DataBind();
                ddlDocumentType.Items.Insert(0, new ListItem("Select Document Type"));
            }
        }

        protected void BindVendorDocuments(int vendorID)
        {
            DataTable dtVendorDocuments = _cm.GetDocumentsOfVendor(vendorID);
            if (dtVendorDocuments.Rows.Count > 0)
            {
                liVendorDocuments.DataSource = dtVendorDocuments;
                liVendorDocuments.DataBind();
            }
        }
        protected void liAddress_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            hdnTAB.Value = "#address";
            if (Request.QueryString["ID"] != null)
            {
                string vendorCode = Request.QueryString["ID"].ToString();
                if (e.Item.ItemType == ListViewItemType.DataItem)
                {
                    BindVendorAddress(vendorCode);
                }
            }
        }

        protected void liAddress_ItemEditing(object sender, ListViewEditEventArgs e)
        {
            hdnTAB.Value = "#address";

            string VendorCode = Request.QueryString["ID"].ToString();

            liAddress.EditIndex = e.NewEditIndex;
            this.BindVendorAddress(VendorCode);
            liAddress.DataBind();

            DropDownList ddlCountry = (DropDownList)liAddress.Items[e.NewEditIndex].FindControl("ddlCountry");
            BindCountry(ddlCountry);

            DropDownList ddlContactPerson = (DropDownList)liAddress.Items[e.NewEditIndex].FindControl("ddlContactPerson");
            BindContactPersonVendor(ddlContactPerson);

            Label lblAddressID = (Label)liAddress.Items[e.NewEditIndex].FindControl("lblAddressID");

            if (lblAddressID != null && !string.IsNullOrEmpty(lblAddressID.Text))
            {
                DataTable dt = _cm.GetVendorLocation(lblAddressID.Text);

                if (dt != null && dt.Rows.Count > 0 && ddlCountry != null)
                {
                    string countryName = dt.Rows[0]["Country"]?.ToString()?.Trim();

                    if (!string.IsNullOrEmpty(countryName))
                    {
                        // Try to find country (case-insensitive)
                        ListItem countryItem = null;

                        foreach (ListItem item in ddlCountry.Items)
                        {
                            if (item.Text.Equals(countryName, StringComparison.OrdinalIgnoreCase))
                            {
                                countryItem = item;
                                break;
                            }
                        }
                        if (countryItem != null)
                        {
                            countryItem.Selected = true;
                        }
                    }
                }
            }
        }
        protected void BindContactPersonVendor(DropDownList ddlContactPerson)
        {
            if (Session["VendorID"] != null)
            {
                DataTable dt = _cm.GetVendorContactPersonDetails(Convert.ToInt32(Session["VendorID"].ToString()));
                if (dt.Rows.Count > 0)
                {
                    ddlContactPerson.DataSource = dt;
                    ddlContactPerson.DataTextField = "ContactPersonVendor";
                    ddlContactPerson.DataValueField = "VendorContactPersonID";
                    ddlContactPerson.DataBind();
                }
            }

        }
        protected void liAddress_ItemUpdating(object sender, ListViewUpdateEventArgs e)
        {
            hdnTAB.Value = "#address";

            TextBox txtAddress1 = (TextBox)liAddress.Items[e.ItemIndex].FindControl("txtAddress1");
            TextBox txtAddress2 = (TextBox)liAddress.Items[e.ItemIndex].FindControl("txtAddress2");
            TextBox txtAddress3 = (TextBox)liAddress.Items[e.ItemIndex].FindControl("txtAddress3");
            TextBox txtCity = (TextBox)liAddress.Items[e.ItemIndex].FindControl("txtCity");
            TextBox txtState = (TextBox)liAddress.Items[e.ItemIndex].FindControl("txtState");
            TextBox txtPincode = (TextBox)liAddress.Items[e.ItemIndex].FindControl("txtPincode");
            DropDownList ddlCountry = (DropDownList)liAddress.Items[e.ItemIndex].FindControl("ddlCountry");
            Label lblAddressID = (Label)liAddress.Items[e.ItemIndex].FindControl("lblAddressID");
            DropDownList ddlContactPerson = (DropDownList)liAddress.Items[e.ItemIndex].FindControl("ddlContactPerson");
            DropDownList ddlAddressType = (DropDownList)liAddress.Items[e.ItemIndex].FindControl("ddlAddressType");
            BindContactPersonVendor(ddlContactPerson);


            int flag = 0;

            if (txtAddress1.Text == "")
            {
                txtAddress1.BorderColor = Color.Red;
                flag = 1;
            }
            else
            {
                txtAddress1.BorderColor = Color.Empty;
            }
            if (txtAddress2.Text == "")
            {
                txtAddress2.BorderColor = Color.Red;
                flag = 1;
            }
            else
            {
                txtAddress2.BorderColor = Color.Empty;
            }
            if (txtCity.Text == "")
            {
                txtCity.BorderColor = Color.Red;
                flag = 1;
            }
            else
            {
                txtCity.BorderColor = Color.Empty;
            }
            if (txtState.Text == "")
            {
                txtState.BorderColor = Color.Red;
                flag = 1;
            }
            else
            {
                txtState.BorderColor = Color.Empty;
            }
            if (txtPincode.Text == "")
            {
                txtPincode.BorderColor = Color.Red;
                flag = 1;
            }
            else
            {
                txtPincode.BorderColor = Color.Empty;
            }
            if (ddlCountry.SelectedIndex == 0)
            {
                ddlCountry.BorderColor = Color.Red;
                flag = 1;
            }
            else
            {
                ddlCountry.BorderColor = Color.Empty;
            }

            if (flag == 0)
            {
                if (Session["vendorID"] != null)
                {
                    _cm.VendorID = Convert.ToInt32(Session["vendorID"].ToString());
                    _cm.AddressLine1 = txtAddress1.Text.ToString();
                    _cm.AddressLine2 = txtAddress2.Text.ToString();
                    _cm.AddressLine3 = txtAddress3.Text.ToString();
                    _cm.City = txtCity.Text.ToString();
                    _cm.State = txtState.Text.ToString();
                    _cm.Country = ddlCountry.SelectedItem.ToString();
                    _cm.Pincode = txtPincode.Text;
                    if (lblAddressID.Text.ToString() == "")
                    {
                        _cm.ContactPersonID = Convert.ToInt32(ddlContactPerson.SelectedValue);
                        _cm.LocationID = _cm.AddVendorLocationDetails(_cm);
                    }
                    else
                    {
                        _cm.LocationID = Convert.ToInt32(lblAddressID.Text.ToString());
                        _cm.UpdateVendorLocation(_cm);
                    }


                    string VendorCode = Request.QueryString["ID"].ToString();

                    liAddress.EditIndex = -1;
                    this.BindVendorAddress(VendorCode);
                    liAddress.DataBind();
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Fill Highlighted Fields.!');", true);
            }
        }

        protected void liAddress_ItemCanceling(object sender, ListViewCancelEventArgs e)
        {
            hdnTAB.Value = "#address";
            string VendorCode = Request.QueryString["ID"].ToString();

            liAddress.EditIndex = -1;
            this.BindVendorAddress(VendorCode);
            liAddress.DataBind();
        }

        protected void liContact_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            hdnTAB.Value = "#contact";
            if (Request.QueryString["ID"] != null)
            {
                string vendorCode = Request.QueryString["ID"].ToString();
                if (e.Item.ItemType == ListViewItemType.DataItem)
                {
                    BindVendorContact(vendorCode);
                }
            }
        }

        protected void liContact_ItemEditing(object sender, ListViewEditEventArgs e)
        {
            hdnTAB.Value = "#contact";

            DataTable dt = new DataTable();
            string VendorCode = Request.QueryString["ID"].ToString();
            liContact.EditIndex = e.NewEditIndex;
            this.BindVendorContact(VendorCode);
            liContact.DataBind();

            DropDownList ddlTitle = (DropDownList)liContact.Items[e.NewEditIndex].FindControl("ddlTitle");
            Label lblVendorContactPersonID = (Label)liContact.Items[e.NewEditIndex].FindControl("lblVendorContactPersonID");

            if (lblVendorContactPersonID != null && !string.IsNullOrEmpty(lblVendorContactPersonID.Text))
            {
                dt = _cm.GetVendorContactPersonDetails(lblVendorContactPersonID.Text);

                if (dt != null && dt.Rows.Count > 0 && ddlTitle != null)
                {
                    string titleValue = dt.Rows[0]["Title"]?.ToString()?.Trim();

                    if (!string.IsNullOrEmpty(titleValue))
                    {
                        // Normalize/clean the title value
                        string normalizedTitle = NormalizeTitle(titleValue);

                        ListItem item = ddlTitle.Items.FindByValue(normalizedTitle);

                        if (item != null)
                        {
                            item.Selected = true;
                        }
                        else
                        {
                            // Default to first option if no match found
                            ddlTitle.SelectedIndex = 0;
                        }
                    }
                }
            }
        }

        // Helper method to normalize titles
        private string NormalizeTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return "";

            title = title.Trim().ToLower();

            // Handle variations
            if (title.StartsWith("mr"))
                return "Mr.";
            else if (title.StartsWith("mrs"))
                return "Mrs.";
            else if (title.StartsWith("miss"))
                return "Miss.";
            else if (title.StartsWith("ms"))
                return "Ms.";

            return ""; // Default
        }
        protected void liContact_ItemUpdating(object sender, ListViewUpdateEventArgs e)
        {
            hdnTAB.Value = "#contact"; int count = 0;

            int flag = 0;
            TextBox txtFirstName = (TextBox)liContact.Items[e.ItemIndex].FindControl("txtFirstName");
            TextBox txtMiddleName = (TextBox)liContact.Items[e.ItemIndex].FindControl("txtMiddleName");
            TextBox txtLastName = (TextBox)liContact.Items[e.ItemIndex].FindControl("txtLastName");
            TextBox txtMobile = (TextBox)liContact.Items[e.ItemIndex].FindControl("txtMobile");
            TextBox txtEmail = (TextBox)liContact.Items[e.ItemIndex].FindControl("txtEmail");
            TextBox txtDesignation = (TextBox)liContact.Items[e.ItemIndex].FindControl("txtDesignation");
            TextBox txtDepartment = (TextBox)liContact.Items[e.ItemIndex].FindControl("txtDepartment");
            DropDownList ddlTitle = (DropDownList)liContact.Items[e.ItemIndex].FindControl("ddlTitle");
            DropDownList ddlContactType = (DropDownList)liContact.Items[e.ItemIndex].FindControl("ddlContactType");

            Label lblVendorContactPersonID = (Label)liContact.Items[e.ItemIndex].FindControl("lblVendorContactPersonID");
            Label lblVendorContactID = (Label)liContact.Items[e.ItemIndex].FindControl("lblVendorContactID");

            if (txtFirstName.Text == "") { txtFirstName.BorderColor = Color.Red; flag = 1; } else { txtFirstName.BorderColor = Color.Empty; }
            if (txtLastName.Text == "") { txtLastName.BorderColor = Color.Red; flag = 1; } else { txtLastName.BorderColor = Color.Empty; }
            if (txtMobile.Text == "") { txtMobile.BorderColor = Color.Red; flag = 1; } else { txtMobile.BorderColor = Color.Empty; }
            if (txtEmail.Text == "")
            {
                txtEmail.BorderColor = Color.Red; flag = 1;
            }
            else
            {
                string emailID = txtEmail.Text.ToString();
                string[] emailList = emailID.Split(',');
                foreach (string email in emailList)
                {
                    Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                    Match match = regex.Match(email);
                    if (match.Success)
                    {
                        count++;
                        txtEmail.BorderColor = System.Drawing.Color.Empty;
                    }
                    else
                    {
                        txtEmail.BorderColor = System.Drawing.Color.Red;
                        flag = 1;
                        txtEmail.Focus();
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Enter valid email Address(s)!');", true);
                    }
                }
                //Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                //Match match = regex.Match(txtEmail.Text);
                //if (match.Success)
                //{
                //    txtEmail.BorderColor = Color.Empty;
                //}
                //else
                //{
                //    flag = 1;
                //    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email Address is not Valid..');", true);
                //}
            }
            if (txtDesignation.Text == "") { txtDesignation.BorderColor = Color.Red; flag = 1; } else { txtDesignation.BorderColor = Color.Empty; }
            if (txtDepartment.Text == "") { txtDepartment.BorderColor = Color.Red; flag = 1; } else { txtDepartment.BorderColor = Color.Empty; }
            if (ddlTitle.SelectedIndex == 0) { ddlTitle.BorderColor = Color.Red; flag = 1; } else { ddlTitle.BorderColor = Color.Empty; }


            if (flag == 0)
            {
                if (Session["vendorID"] != null)
                {
                    _cm.VendorID = Convert.ToInt32(Session["vendorID"].ToString());
                    _cm.FirstName = txtFirstName.Text.ToString();
                    _cm.MiddleName = txtMiddleName.Text.ToString();
                    _cm.LastName = txtLastName.Text.ToString();
                    _cm.ContactMobile = txtMobile.Text.ToString();
                    _cm.ContactEmail = txtEmail.Text.ToString();
                    _cm.Designation = txtDesignation.Text.ToString();
                    _cm.Department = txtDepartment.Text.ToString();
                    _cm.IsPrimaryContact = ddlContactType.SelectedValue == "Yes" ? "Yes" : "No";

                    _cm.Title = ddlTitle.SelectedItem.ToString();
                    if (lblVendorContactPersonID.Text == "")
                    {
                        _cm.ContactPersonID = _cm.AddContactPersonDetails(_cm);
                        _cm.ContactID = _cm.AddVendorContactDetails(_cm);
                    }
                    else
                    {
                        _cm.ContactID = Convert.ToInt32(lblVendorContactID.Text.ToString());
                        _cm.ContactPersonID = Convert.ToInt32(lblVendorContactPersonID.Text.ToString());
                        _cm.UpdateVendorContactPerson(_cm);
                        _cm.UpdateVendorContact(_cm);
                    }
                }
                else
                {
                    Response.Redirect("VendorList.aspx");
                }


                string VendorCode = Request.QueryString["ID"].ToString();
                liContact.EditIndex = -1;
                this.BindVendorContact(VendorCode);
                liContact.DataBind();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Fill Highlighted Fields.!');", true);
            }

        }

        protected void liContact_ItemCanceling(object sender, ListViewCancelEventArgs e)
        {
            hdnTAB.Value = "#contact";

            string VendorCode = Request.QueryString["ID"].ToString();
            liContact.EditIndex = -1;
            this.BindVendorContact(VendorCode);
            liContact.DataBind();
        }
        private void PopulateTreeview()
        {
            try
            {
                DataTable dtparent = _cm.FillParentTable();
                DataTable dtchild = _cm.FillChildTable();

                if (dtparent.Rows.Count > 0)
                {
                    tvTables.Nodes.Clear();
                    bool hasValidNodes = false;

                    foreach (DataRow masterRow in dtparent.Rows)
                    {
                        string purchaseGroupID = masterRow["purchaseGroupID"].ToString();

                        // Manually find child rows instead of using DataSet relations
                        DataRow[] childRows = dtchild.Select("purchaseGroupID = '" + purchaseGroupID + "'");

                        if (childRows.Length > 0)  // Only add parent if it has children
                        {
                            TreeNode masterNode = new TreeNode(
                                (string)masterRow["purchaseGroupName"],
                                purchaseGroupID);

                            tvTables.Nodes.Add(masterNode);
                            hasValidNodes = true;

                            foreach (DataRow childRow in childRows)
                            {
                                TreeNode childNode = new TreeNode(
                                    (string)childRow["purchaseSubGroupName"],
                                    childRow["purchaseGroupID"].ToString());

                                masterNode.ChildNodes.Add(childNode);
                                childNode.Value = childRow["purchaseSubGroupID"].ToString();
                            }
                        }
                    }

                    tvTables.CollapseAll();

                    // Show alert if no valid groups with subgroups found
                    if (!hasValidNodes)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                            "alert('No purchase groups with subgroups available.');", true);
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                        "alert('No purchase groups available.');", true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    "alert('Error loading treeview: " + ex.Message.Replace("'", "\\'") + "');", true);
                // Log the error but don't throw - allows page to continue
                
            }
        }

        //protected void BindTreeView(int vendorID)
        //{
        //    try
        //    {
        //        tvTables.ExpandAll();
        //        DataTable dt = _cm.GetVendor(vendorID.ToString());

        //        if (dt.Rows.Count > 0)
        //        {
        //            string str = dt.Rows[0]["PurchaseSubGroupID"].ToString();

        //            if (!string.IsNullOrEmpty(str))
        //            {
        //                string subgroup = str.Replace("[", "").Replace("]", "").TrimEnd(',');
        //                string[] subgroupID = subgroup.Split(',');

        //                bool anyChecked = false;

        //                foreach (TreeNode node in tvTables.Nodes)
        //                {
        //                    if (node.ChildNodes.Count > 0)
        //                    {
        //                        foreach (TreeNode child in node.ChildNodes)
        //                        {
        //                            // Trim whitespace from both values for comparison
        //                            if (Array.Exists(subgroupID, s => s.Trim() == child.Value.Trim()))
        //                            {
        //                                child.ShowCheckBox = true;
        //                                child.Selected = true;
        //                                child.Checked = true;
        //                                anyChecked = true;
        //                            }
        //                        }
        //                    }
        //                }

        //                if (!anyChecked)
        //                {
        //                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
        //                        "alert('No matching purchase subgroups found for this vendor.');", true);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
        //            "alert('Error binding treeview: " + ex.Message.Replace("'", "\\'") + "');", true);
        //    }
        //}
        protected void BindTreeView(int vendorID)
        {
            try
            {
                tvTables.ExpandAll();
                DataTable dt = _cm.GetVendor(vendorID.ToString());

                if (dt.Rows.Count > 0)
                {
                    string str = dt.Rows[0]["PurchaseSubGroupID"].ToString();

                    if (!string.IsNullOrEmpty(str))
                    {
                        //  string subgroup = str.Replace("[", "").Replace("]", "").TrimEnd(',');string[] subgroupID = str.Split(',');
                        string[] subgroupID = str.Split(',');
                        //string[] subgroupID = subgroup.Split(',');

                        bool anyChecked = false;

                        foreach (TreeNode node in tvTables.Nodes)
                        {
                            if (node.ChildNodes.Count > 0)
                            {
                                foreach (TreeNode child in node.ChildNodes)
                                {
                                    // Trim whitespace from both values for comparison
                                    if (Array.Exists(subgroupID, s => s.Trim() == child.Value.Trim()))
                                    {
                                        child.ShowCheckBox = true;
                                        child.Selected = true;
                                        child.Checked = true;
                                        anyChecked = true;
                                    }
                                }
                            }
                        }

                        if (!anyChecked)
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                                "alert('No matching purchase subgroups found for this vendor.');", true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    "alert('Error binding treeview: " + ex.Message.Replace("'", "\\'") + "');", true);
            }
        }
        //protected void btnSaveCategory_Click(object sender, EventArgs e)
        //{
        //    hdnTAB.Value = "#categories";

        //    string selectedGroups = "";
        //    string selectedSubGroups = "";

        //    foreach (TreeNode node in tvTables.Nodes)
        //    {
        //        bool hasCheckedChild = false;

        //        // First, collect all checked subgroups
        //        foreach (TreeNode child in node.ChildNodes)
        //        {
        //            if (child.Checked)
        //            {
        //                selectedSubGroups += "[" + child.Value + "],";
        //                hasCheckedChild = true;
        //            }
        //        }

        //        // If any child is checked, add the parent group
        //        if (hasCheckedChild)
        //        {
        //            selectedGroups += "[" + node.Value + "],";
        //        }
        //    }

        //    // Remove trailing comma
        //    selectedGroups = selectedGroups.TrimEnd(',');
        //    selectedSubGroups = selectedSubGroups.TrimEnd(',');

        //    _cm.SelectedGroups = selectedGroups;
        //    _cm.SelectedSubGroups = selectedSubGroups;

        //    if (Session["vendorID"] != null)
        //    {
        //        _cm.VendorID = Convert.ToInt32(Session["vendorID"].ToString());
        //        _cm.updateVendorCategory(_cm);
        //    }
        //}
        protected void btnSaveCategory_Click(object sender, EventArgs e)
        {
            hdnTAB.Value = "#categories";

            string selectedGroups = "";
            string selectedSubGroups = "";

            foreach (TreeNode node in tvTables.Nodes)
            {
                bool hasCheckedChild = false;

                // First, collect all checked subgroups
                foreach (TreeNode child in node.ChildNodes)
                {
                    if (child.Checked)
                    {
                        selectedSubGroups += child.Value + ",";  // removed []
                        hasCheckedChild = true;
                    }
                }

                // If any child is checked, add the parent group

                if (hasCheckedChild)
                {
                    selectedGroups += node.Value + ",";  // removed []
                }
            }

            // Remove trailing comma
            selectedGroups = selectedGroups.TrimEnd(',');
            selectedSubGroups = selectedSubGroups.TrimEnd(',');

            _cm.SelectedGroups = selectedGroups;
            _cm.SelectedSubGroups = selectedSubGroups;

            if (Session["vendorID"] != null)
            {
                _cm.VendorID = Convert.ToInt32(Session["vendorID"].ToString());
                _cm.updateVendorCategory(_cm);
            }
        }

        protected void lnkEditDocuments_Click(object sender, EventArgs e)
        {
            hdnTAB.Value = "#documents";
            divDocuments.Visible = false;
            divEditDocuments.Visible = true;
            lnkUpdateDocuments.Visible = true;
            lnkEditDocuments.Visible = false;
        }

        protected void lnkUpdateDocuments_Click(object sender, EventArgs e)
        {
            hdnTAB.Value = "#documents";
            int flag = 0;
            if (!fpUpload.HasFile)
            {
                fpUpload.BorderColor = Color.Red;
                fpUpload.BorderStyle = BorderStyle.Solid;
                fpUpload.BorderWidth = 2;
                flag = 1;
            }
            else
            {
                fpUpload.BorderColor = Color.Empty;
                fpUpload.BorderStyle = BorderStyle.Solid;
                fpUpload.BorderWidth = 2;
            }
            if (ddlDocumentType.SelectedIndex == 0)
            {
                ddlDocumentType.BorderColor = Color.Red;
                flag = 1;
            }
            else
            {
                ddlDocumentType.BorderColor = Color.Empty;
            }
            if (flag == 0)
            {
                _cm.DocumentType = "";

                string strpath = System.IO.Path.GetExtension(fpUpload.FileName);

                string VendorCode = Request.QueryString["ID"].ToString();
                DataTable dt = _cm.GetVendorCode(VendorCode.ToUpper());
                if (dt.Rows.Count > 0)
                {
                    _cm.VendorID = Convert.ToInt32(dt.Rows[0]["VendorID"].ToString());
                    DataTable dtVendor = _cm.GetVendorReference(dt.Rows[0]["VendorID"].ToString());
                    if (dtVendor.Rows.Count > 0)
                    {
                        _cm.Reference = dtVendor.Rows[0]["Reference"].ToString();
                    }
                }

                _cm.DocumentTypeID = Convert.ToInt32(ddlDocumentType.SelectedValue.ToString());
                DataTable dtType = _cm.GetDocumentVendorDetails(_cm);
                DataTable dtFile = _cm.GetFilename(_cm);
                if (dtFile.Rows.Count > 0)
                {
                    _cm.Type = dtFile.Rows[0]["Filename"].ToString();
                    if (dtType.Rows.Count > 0)
                    {
                        if (dtType.Rows[0]["VersionNumber"].ToString() != "")
                        {
                            int VersionNumber = Convert.ToInt32(dtType.Rows[0]["VersionNumber"].ToString());
                            _cm.Version = VersionNumber + 1;
                        }
                        else
                        {
                            _cm.Version = 0;
                        }
                    }
                    else
                    {
                        _cm.Version = 0;
                    }
                }
                _cm.DocumentName = _cm.Reference + "-" + _cm.Version + "-" + _cm.Type + strpath;
                _cm.CreatedDate = DateTime.Now.AddHours(12.5);
                string filename = Server.MapPath("//VendorDocument/" + _cm.DocumentName);
                fpUpload.SaveAs(filename);

                _cm.AddVendorDocument(_cm);


                string VendorID = "";
                DataTable dtCust = _cm.GetVendorCode(VendorCode.ToUpper());
                if (dtCust.Rows.Count > 0)
                {
                    VendorID = dtCust.Rows[0]["VendorID"].ToString();
                }
                BindVendorDocuments(Convert.ToInt32(VendorID));

                divDocuments.Visible = true;
                divEditDocuments.Visible = false;
                lnkUpdateDocuments.Visible = false;
                lnkEditDocuments.Visible = true;
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Fill Highlighted Fields.!');", true);
            }
        }

        protected void lnkCancelUpdate_Click(object sender, EventArgs e)
        {
            hdnTAB.Value = "#documents";
            divDocuments.Visible = true;
            divEditDocuments.Visible = false;
            lnkUpdateDocuments.Visible = false;
            lnkEditDocuments.Visible = true;

            int VendorID = 0;
            string VendorCode = Request.QueryString["ID"].ToString();

            DataTable dt = _cm.GetVendorCode(VendorCode.ToUpper());
            if (dt.Rows.Count > 0)
            {
                VendorID = Convert.ToInt32(dt.Rows[0]["VendorID"].ToString());
            }
            BindVendorDocuments(VendorID);
        }

        protected void btnEditTIN_Click(object sender, EventArgs e)
        {
            txtTIN.Text = lblPAN.Text;

            lblPAN.Visible = false;
            btnEditTIN.Visible = false;

            pnlTINEdit.Visible = true;
        }

        protected void btnSaveTIN_Click(object sender, EventArgs e)
        {
            int VendorID = 0;
            string tin = txtTIN.Text.Trim();

            if (string.IsNullOrEmpty(tin))
            {
                return;
            }

            // TODO: Update TIN in your database
            // using a parameterized SQL UPDATE query.
            if (Session["vendorID"] != null)
            {
                VendorID = Convert.ToInt32(Session["vendorID"]);
                _cm.VendorID = VendorID;
                _cm.PanNumber = tin;
                _cm.UpdateVendorTIN(_cm);
            }
            lblPAN.Text = tin;

            pnlTINEdit.Visible = false;
            lblPAN.Visible = true;
            btnEditTIN.Visible = true;
        }

        protected void btnCancelTIN_Click(object sender, EventArgs e)
        {
            pnlTINEdit.Visible = false;

            lblPAN.Visible = true;
            btnEditTIN.Visible = true;
        }
    }
}