using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class CustomerProfile : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
           if (Session["Usertype"] != null)
            {
                if (Session["EmployeeName"] != null)
                {
                    Label lblUserName = (Label)Master.FindControl("lblUserName");
                    lblUserName.Text = Session["EmployeeName"].ToString();
                }
                if (Session["Email"] != null)
                {
                    Label lblUserEmail = (Label)Master.FindControl("lblUserEmail");
                    lblUserEmail.Text = Session["Email"].ToString();
                }
                if (Request.QueryString["ID"] != null)
                {
                    if (!Page.IsPostBack)
                    {
                        int customerMasterID = Convert.ToInt32(Request.QueryString["ID"].ToString());
                        //liCustomers1.Attributes.Add("class", "active");
                        BindCustomer(customerMasterID);
                        BindCustomerStats(customerMasterID);
                        Session["CustomerMasterID"] = customerMasterID;
                    }
                    if (Session["RoleID"] != null)
                    {
                        int RoleID = Convert.ToInt32(Session["RoleID"].ToString());
                        
                    }
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }

        }
        private void BindCustomer(int customerMasterID)
        {
            DataTable dtCustomerType = _cm.getCustomerType(customerMasterID);
            string customerType = dtCustomerType.Rows[0]["CustomerType"].ToString();
            hdCustomerType.Value = customerType;
            DataTable dt = _cm.getCustomerDetails(customerMasterID, customerType);
            if (dt.Rows.Count > 0)
            {
                hdCustomerID.Value = dt.Rows[0]["CustomerID1"].ToString();
                if (customerType == "Individual")
                {
                    ulCompany.Visible = false;
                    ulIndividual.Visible = true;
                    tabName.InnerText = "Individual Details";
                    lblCompany.Text = dt.Rows[0]["FirstName"].ToString().ToUpper() + " " + dt.Rows[0]["MiddleName"].ToString().ToUpper() + " " + dt.Rows[0]["LastName"].ToString().ToUpper();
                    lblIndividualName.Text = dt.Rows[0]["Title"].ToString() + " " + dt.Rows[0]["FirstName"].ToString() + " " + dt.Rows[0]["MiddleName"].ToString() + " " + dt.Rows[0]["LastName"].ToString();
                    lblIAddress1.Text = dt.Rows[0]["AddressLine1"].ToString();
                    lblIAddress2.Text = dt.Rows[0]["AddressLine2"].ToString();
                    lblIAddress3.Text = dt.Rows[0]["AddressLine3"].ToString();
                    lblICity.Text = "City: " + dt.Rows[0]["City"].ToString();
                    lblIDistrict.Text = "District: " + dt.Rows[0]["District"].ToString();
                    lblITaluka.Text = "Taluka: " + dt.Rows[0]["Taluka"].ToString();
                    lblIState.Text = "State: " + dt.Rows[0]["State"].ToString();
                    lblIPinCode.Text = "PinCode: " + dt.Rows[0]["PinCode"].ToString();
                    lblICountry.Text = "Country: " + dt.Rows[0]["Country"].ToString();
                    divViewIndividualAddress.Visible = true;
                    divEditIndividualAddress.Visible = false;
                }
                else
                {
                    ulIndividual.Visible = false;
                    ulCompany.Visible = true;
                    DataTable dtGroupDetails = _cm.getGroupDetails(customerMasterID);
                    if (dtGroupDetails.Rows.Count > 0)
                    {
                        liGroup.Visible = true;
                        lblGroupName.Text = dtGroupDetails.Rows[0]["GroupName"].ToString();
                    }
                    else
                    {
                        liGroup.Visible = false;
                    }
                    tabName.InnerText = "Company Details";
                    lblCompany.Text = dt.Rows[0]["CompanyName"].ToString().ToUpper();
                    lblCompanyName.Text = dt.Rows[0]["CompanyName"].ToString();
                    lblCAddress1.Text = dt.Rows[0]["AddressLine1"].ToString();
                    lblCAddress2.Text = dt.Rows[0]["AddressLine2"].ToString();
                    //lblCAddress3.Text = dt.Rows[0]["AddressLine3"].ToString();
                    lblCCity.Text = "City: " + dt.Rows[0]["City"].ToString();
                    //lblCDistrict.Text = "District: " + dt.Rows[0]["District"].ToString();
                    //lblCTaluka.Text = "Taluka: " + dt.Rows[0]["Taluka"].ToString();
                    //lblCState.Text = "State: " + dt.Rows[0]["State"].ToString();
                    lblCPinCode.Text = "PinCode: " + dt.Rows[0]["PinCode"].ToString();
                    lblCCountry.Text = "Country: " + dt.Rows[0]["Country"].ToString();
                    divViewCompanyAddress.Visible = true;
                    divEditCompanyAddress.Visible = false;
                }
                BindContacts(customerMasterID);
            }

        }
        private void BindContacts(int customerMasterID)
        {
            DataTable dtContacts = _cm.getContactDetails(customerMasterID);
            liContacts.DataSource = null;
            liContacts.DataBind();
            liContacts.DataSource = dtContacts;
            liContacts.DataBind();
        }
        private void BindCustomerStats(int customerMasterID)
        {
            int customerID = 0;
            DataTable dt = _cm.GetCustomerID(customerMasterID);
            if (dt.Rows.Count > 0)
            {
                customerID = Convert.ToInt32(dt.Rows[0]["customerID"].ToString());
            }
            DataTable[] dt1 = new DataTable[4];
            dt1 = _cm.GetCountForCustomerProfile(customerID);
            if (dt1[0].Rows.Count > 0)
            {
                lblEnquiries.Text = dt1[0].Rows.Count.ToString();
                lblSamples.Text = dt1[1].Rows.Count.ToString();
                lblCompleted.Text = dt1[2].Rows.Count.ToString();
            }
        }

        protected void btnEditIndividualAddress_Click(object sender, EventArgs e)
        {
            int customerMasterID = Convert.ToInt32(Request.QueryString["ID"].ToString());
            divViewIndividualAddress.Visible = true;
            divEditIndividualAddress.Visible = true;
            DataTable dt = _cm.getCustomerDetails(customerMasterID, "Individual");
            if (dt.Rows.Count > 0)
            {
                txtIAddress1.Text = dt.Rows[0]["AddressLine1"].ToString();
                txtIAddress2.Text = dt.Rows[0]["AddressLine2"].ToString();
                txtIAddress3.Text = dt.Rows[0]["AddressLine3"].ToString();
                txtICity.Text = dt.Rows[0]["City"].ToString();
                txtIDistrict.Text = dt.Rows[0]["District"].ToString();
                txtITaluka.Text = dt.Rows[0]["Taluka"].ToString();
                txtIState.Text = dt.Rows[0]["State"].ToString();
                txtIPinCode.Text = dt.Rows[0]["PinCode"].ToString();
                txtICountry.Text = dt.Rows[0]["Country"].ToString();
            }
        }
        protected void btnEditCompanyAddress_Click(object sender, EventArgs e)
        {
            int customerMasterID = Convert.ToInt32(Request.QueryString["ID"].ToString());
            divViewCompanyAddress.Visible = true;
            divEditCompanyAddress.Visible = true;
            DataTable dt = _cm.getCustomerDetails(customerMasterID, "Company");
            if (dt.Rows.Count > 0)
            {
                txtCAddress1.Text = dt.Rows[0]["AddressLine1"].ToString();
                txtCAddress2.Text = dt.Rows[0]["AddressLine2"].ToString();
                //txtCAddress3.Text = dt.Rows[0]["AddressLine3"].ToString();
                txtCCity.Text = dt.Rows[0]["City"].ToString();
                //txtCDistrict.Text = dt.Rows[0]["District"].ToString();
                //txtCTaluka.Text = dt.Rows[0]["Taluka"].ToString();
                //txtCState.Text = dt.Rows[0]["State"].ToString();
                txtCPinCode.Text = dt.Rows[0]["PinCode"].ToString();
                txtCCountry.Text = dt.Rows[0]["Country"].ToString();
            }
        }
        protected void btnUpdateCAddress_Click(object sender, EventArgs e)
        {
            int customerMasterID = Convert.ToInt32(Request.QueryString["ID"].ToString());
            int flag = 0;
            if (txtCAddress1.Text == "")
            {
                txtCAddress1.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtCAddress1.BackColor = System.Drawing.Color.Empty;
            }
            //if (txtCDistrict.Text == "")
            //{
            //    txtCDistrict.BackColor = System.Drawing.Color.LightPink;
            //    flag = 1;
            //}
            //else
            //{
            //    txtCDistrict.BackColor = System.Drawing.Color.Empty;
            //}
            //if (txtCState.Text == "")
            //{
            //    txtCState.BackColor = System.Drawing.Color.LightPink;
            //    flag = 1;
            //}
            //else
            //{
            //    txtCState.BackColor = System.Drawing.Color.Empty;
            //}
            if (txtCCountry.Text == "")
            {
                txtCCountry.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtCCountry.BackColor = System.Drawing.Color.Empty;
            }
            if (flag == 0)
            {
                _cm.CustomerMasterId = customerMasterID;
                _cm.customerId = Convert.ToInt32(hdCustomerID.Value);
                _cm.customerType = "Company";
                _cm.location = "";
                _cm.locationName = "";
                _cm.locationAddressType = "";
                _cm.addressLine1 = txtCAddress1.Text;
                _cm.addressLine2 = txtCAddress2.Text;
                _cm.addressLine3 = "";// txtCAddress3.Text;
                _cm.city = txtCCity.Text;
                _cm.suburb = "";
                _cm.taluka = "";// txtCTaluka.Text;
                _cm.district = "";// txtCDistrict.Text;
                _cm.state = "";// txtCState.Text;
                _cm.pincode = txtCPinCode.Text;
                _cm.country = txtCCountry.Text;
                _cm.status = true;
                DataTable dtExists = _cm.checkLocationExists(customerMasterID);
                if (dtExists.Rows.Count > 0)
                {
                    _cm.UpdateProfileAddress(_cm);
                }
                else
                {
                    _cm.AddLocationDetails(_cm);
                }
                divEditCompanyAddress.Visible = false;
                BindCustomer(customerMasterID);
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "notify", "alert(Fill Highlighted Fields.)", true);
            }
        }

        protected void btnUpdateIAddress_Click(object sender, EventArgs e)
        {
            int customerMasterID = Convert.ToInt32(Request.QueryString["ID"].ToString());
            int flag = 0;
            if (txtIAddress1.Text == "")
            {
                txtIAddress1.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtIAddress1.BackColor = System.Drawing.Color.Empty;
            }
            if (txtIDistrict.Text == "")
            {
                txtIDistrict.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtIDistrict.BackColor = System.Drawing.Color.Empty;
            }
            if (txtIState.Text == "")
            {
                txtIState.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtIState.BackColor = System.Drawing.Color.Empty;
            }
            if (txtICountry.Text == "")
            {
                txtICountry.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtICountry.BackColor = System.Drawing.Color.Empty;
            }
            if (flag == 0)
            {
                _cm.CustomerMasterId = customerMasterID;
                _cm.customerId = Convert.ToInt32(hdCustomerID.Value);
                _cm.addressLine1 = txtIAddress1.Text;
                _cm.addressLine2 = txtIAddress2.Text;
                _cm.addressLine3 = txtIAddress3.Text;
                _cm.taluka = txtITaluka.Text;
                _cm.district = txtIDistrict.Text;
                _cm.state = txtIState.Text;
                _cm.pincode = txtIPinCode.Text;
                _cm.country = txtICountry.Text;
                _cm.UpdateProfileAddress(_cm);
                divEditIndividualAddress.Visible = false;
                BindCustomer(customerMasterID);
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "notify", "alert(Fill Highlighted Fields.)", true);
            }
        }



        protected void liContacts_ItemUpdating(object sender, ListViewUpdateEventArgs e)
        {
            Button btnUpdate = (Button)liContacts.Items[e.ItemIndex].FindControl("btnUpdate");

            int personId = Convert.ToInt32(btnUpdate.CommandArgument);
            _cm.personId = personId;

            int customerMasterID = Convert.ToInt32(Session["customerMasterID"].ToString());
            TextBox txtMobile = (TextBox)liContacts.EditItem.FindControl("txtMobile") as TextBox;
            TextBox txtEmail = (TextBox)liContacts.EditItem.FindControl("txtEmail") as TextBox;
            TextBox txtPhone = (TextBox)liContacts.EditItem.FindControl("txtPhone") as TextBox;
            _cm.CustomerMasterId = customerMasterID;
            _cm.mobile = txtMobile.Text;
            _cm.email = txtEmail.Text;
            _cm.phone = txtPhone.Text;
            _cm.UpdateProfileContact(_cm);
            liContacts.EditIndex = -1;
            BindContacts(customerMasterID);
        }

        protected void liContacts_ItemEditing(object sender, ListViewEditEventArgs e)
        {
            int customerMasterID = Convert.ToInt32(Session["customerMasterID"].ToString());
            liContacts.EditIndex = e.NewEditIndex;
            TextBox txtMobile = (TextBox)liContacts.EditItem.FindControl("txtMobile") as TextBox;
            TextBox txtEmail = (TextBox)liContacts.EditItem.FindControl("txtEmail") as TextBox;
            TextBox txtPhone = (TextBox)liContacts.EditItem.FindControl("txtPhone") as TextBox;
            liContacts.EditIndex = e.NewEditIndex;
            BindContacts(customerMasterID);
        }

        protected void liContacts_ItemCanceling(object sender, ListViewCancelEventArgs e)
        {
            int customerMasterID = Convert.ToInt32(Session["customerMasterID"].ToString());
            liContacts.EditIndex = -1;
            BindContacts(customerMasterID);
        }

        protected void liContacts_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (Request.QueryString["ID"] != null)
            {
                int customerMasterID = Convert.ToInt32(Request.QueryString["ID"].ToString());
                if (e.Item.ItemType == ListViewItemType.DataItem)
                {
                    BindContacts(customerMasterID);
                }
            }

        }

        protected void btnAddContact_Click(object sender, EventArgs e)
        {
            divBtnAdd.Visible = false;
            divAddContact.Visible = true;
            divContactList.Visible = false;
        }
        protected void btnAddPerson_Click(object sender, EventArgs e)
        {
            AddPersonDetails();
            Clear();
        }

        private void AddPersonDetails()
        {
            int flag = 0;
            if (txtPersonFirstName.Text == "")
            {
                txtPersonFirstName.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtPersonFirstName.BackColor = System.Drawing.Color.Empty;
            }
            if (txtPersonLastName.Text == "")
            {
                txtPersonLastName.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtPersonLastName.BackColor = System.Drawing.Color.Empty;
            }
            if (ddlPersonTitle.SelectedIndex == 0)
            {
                ddlPersonTitle.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                ddlPersonTitle.BackColor = System.Drawing.Color.Empty;
            }
            if (txtDesignation.Text == "")
            {
                txtDesignation.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtDesignation.BackColor = System.Drawing.Color.Empty;
            }
            if (txtDepartment.Text == "")
            {
                txtDepartment.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtDepartment.BackColor = System.Drawing.Color.Empty;
            }
            int code = 0;
            if (txtMobile.Text == "")
            {
                txtMobile.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtMobile.BackColor = System.Drawing.Color.Empty;
            }
            if (txtEmail.Text == "")
            {
                txtEmail.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                if ((Regex.IsMatch(txtEmail.Text, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase)))
                {
                    txtEmail.BackColor = System.Drawing.Color.Empty;
                }
                else
                {
                    code = 1;
                    txtEmail.BackColor = System.Drawing.Color.LightPink;
                }
            }

            if (flag == 0 && code == 0)
            {
                DataTable dt = new DataTable();
                if (Session["DTPerson"] != null)
                {
                    dt = (DataTable)Session["DTPerson"];
                }
                else
                {
                    dt.Columns.Add("Title");
                    dt.Columns.Add("FirstName");
                    dt.Columns.Add("MiddleName");
                    dt.Columns.Add("LastName");
                    dt.Columns.Add("Designation");
                    dt.Columns.Add("Department");
                }
                DataRow dr = dt.NewRow();
                dr["Title"] = ddlPersonTitle.SelectedItem.Text.ToString();
                dr["FirstName"] = txtPersonFirstName.Text.ToString();
                dr["MiddleName"] = txtPersonMiddleName.Text.ToString();
                dr["LastName"] = txtPersonLastName.Text.ToString();
                dr["Designation"] = txtDesignation.Text.ToString();
                dr["Department"] = txtDepartment.Text.ToString();

                dt.Rows.Add(dr);
                Session["DTPerson"] = dt;
                Clear();
            }
        }
        private void Clear()
        {
            ddlPersonTitle.SelectedIndex = 0;
            txtPersonFirstName.Text = "";
            txtPersonMiddleName.Text = "";
            txtPersonLastName.Text = "";
            txtDesignation.Text = "";
            txtDepartment.Text = "";
        }
        protected void btnContactSave_Click(object sender, EventArgs e)
        {
            int PersonMst = 0;
            int flag = 0;
            if (txtPersonFirstName.Text == "")
            {
                txtPersonFirstName.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtPersonFirstName.BackColor = System.Drawing.Color.Empty;
            }
            if (txtPersonLastName.Text == "")
            {
                txtPersonLastName.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtPersonLastName.BackColor = System.Drawing.Color.Empty;
            }
            if (ddlPersonTitle.SelectedIndex == 0)
            {
                ddlPersonTitle.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                ddlPersonTitle.BackColor = System.Drawing.Color.Empty;
            }
            if (txtDesignation.Text == "")
            {
                txtDesignation.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtDesignation.BackColor = System.Drawing.Color.Empty;
            }
            if (txtDepartment.Text == "")
            {
                txtDepartment.BackColor = System.Drawing.Color.LightPink;
                flag = 1;
            }
            else
            {
                txtDepartment.BackColor = System.Drawing.Color.Empty;
            }
            if (flag == 0)
            {
                if (hdCustomerType.Value == "Individual")
                {
                    _cm.CustomerMasterId = Convert.ToInt32(Session["CustomerMasterId"]);
                    _cm.customerId = Convert.ToInt32(hdCustomerID.Value);
                    _cm.customerType = Convert.ToString(hdCustomerType.Value);
                    _cm.title = ddlPersonTitle.SelectedItem.ToString();
                    _cm.firstName = txtPersonFirstName.Text;
                    _cm.middleName = txtPersonMiddleName.Text;
                    _cm.lastName = txtPersonLastName.Text;
                    _cm.designation = txtDesignation.Text;
                    _cm.department = txtDepartment.Text;
                    PersonMst = _cm.AddPersonDetails(_cm);

                    _cm.personId = PersonMst;
                    _cm.mobile = txtMobile.Text;
                    _cm.email = txtEmail.Text;
                    _cm.phone = txtPhone.Text;
                    _cm.fax = txtFax.Text;

                    int ContactDetail = _cm.AddContactDetails(_cm);
                    btnContactSave.Visible = false;
                    ClearContact();
                }
                if (hdCustomerType.Value == "Company")
                {
                    _cm.CustomerMasterId = Convert.ToInt32(Session["CustomerMasterId"]);
                    _cm.customerId = Convert.ToInt32(hdCustomerID.Value);
                    _cm.customerType = Convert.ToString(hdCustomerType.Value);
                    _cm.title = ddlPersonTitle.SelectedItem.ToString();
                    _cm.firstName = txtPersonFirstName.Text;
                    _cm.middleName = txtPersonMiddleName.Text;
                    _cm.lastName = txtPersonLastName.Text;
                    _cm.designation = txtDesignation.Text;
                    _cm.department = txtDepartment.Text;
                    PersonMst = _cm.AddPersonDetails(_cm);

                    _cm.personId = PersonMst;
                    _cm.mobile = txtMobile.Text;
                    _cm.email = txtEmail.Text;
                    _cm.phone = txtPhone.Text;
                    _cm.fax = txtFax.Text;

                    int ContactDetail = _cm.AddContactDetails(_cm);
                    btnContactSave.Visible = false;
                    ClearContact();
                }
               

                int customerMasterID = Convert.ToInt32(Request.QueryString["ID"].ToString());
                BindCustomer(customerMasterID);
                divAddContact.Visible = false;
                divBtnAdd.Visible = true;
                divContactList.Visible = true;
                Response.Redirect(Request.RawUrl);
            }
        }
        private void ClearContact()
        {
            txtMobile.Text = "";
            txtEmail.Text = "";
            txtPhone.Text = "";
            txtFax.Text = "";
        }

        protected void lnkEditCompanyName_Click(object sender, EventArgs e)
        {
            int customerMasterID = Convert.ToInt32(Request.QueryString["ID"].ToString());
            divViewCustomerName.Visible = true;
            divEditCompanyName.Visible = true;
            DataTable dt = _cm.getCustomerDetails(customerMasterID, "Company");
            if (dt.Rows.Count > 0)
            {
                txtCompanyName.Text = dt.Rows[0]["CompanyName"].ToString();
            }
        }

        protected void btnSaveCustomerName_Click(object sender, EventArgs e)
        {
            int flag = 0;
            if (txtCompanyName.Text == "")
            {
                flag = 1;
                txtCompanyName.BorderColor = Color.Red;
            }
            else
            {
                txtCompanyName.BorderColor = Color.Empty;
            }
            if (flag == 0)
            {
                if (hdCustomerType.Value == "Individual")
                {
                    _cm.CustomerMasterId = Convert.ToInt32(Session["CustomerMasterId"]);
                    _cm.customerId = Convert.ToInt32(hdCustomerID.Value);
                    _cm.customerType = Convert.ToString(hdCustomerType.Value);
                    _cm.firstName = txtCompanyName.Text;
                    _cm.UpdateCustomername(_cm);
                }
                if (hdCustomerType.Value == "Company")
                {
                    _cm.CustomerMasterId = Convert.ToInt32(Session["CustomerMasterId"]);
                    _cm.customerId = Convert.ToInt32(hdCustomerID.Value);
                    _cm.customerType = Convert.ToString(hdCustomerType.Value);
                    _cm.CompanyName = txtCompanyName.Text;
                    _cm.UpdateCompanyname(_cm);
                }
                divEditCompanyName.Visible = false;
                BindCustomer(_cm.CustomerMasterId);
            }
        }
        protected void btnToggle_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            int contactPersonId =
                Convert.ToInt32(btn.CommandArgument);

            _cm.ToggleStatus(contactPersonId);

            int customerMasterID = Convert.ToInt32(Request.QueryString["ID"].ToString());
            BindCustomer(customerMasterID);
        }
        protected void chkActive_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;

            ListViewItem item = (ListViewItem)chk.NamingContainer;

            HiddenField hfPersonID =
                (HiddenField)item.FindControl("hfPersonID");

            int personId = Convert.ToInt32(hfPersonID.Value);

            bool isActive = chk.Checked;

            _cm.ToggleStatus(personId);
            int customerMasterID = Convert.ToInt32(Request.QueryString["ID"].ToString());
            BindCustomer(customerMasterID);
        }
    }
}