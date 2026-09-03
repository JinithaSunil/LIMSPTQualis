using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Threading;
using System.Text.RegularExpressions;
using System.Drawing;
using Newtonsoft.Json;
using Color = System.Drawing.Color;

namespace LIMS
{
    public partial class CustomerAdd : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        int custID = 0; int customerUID = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlControl liHome1 = (HtmlGenericControl)this.Master.FindControl("liHome1");
            HtmlControl liCustomers1 = (HtmlGenericControl)this.Master.FindControl("liCustomers1");
            HtmlControl liInquiries1 = (HtmlGenericControl)this.Master.FindControl("liInquiries1");
            HtmlControl liCRFList1 = (HtmlGenericControl)this.Master.FindControl("liCRFList1");
            HtmlControl liSampleManagement1 = (HtmlGenericControl)this.Master.FindControl("liSampleManagement1");
            HtmlControl liTestManagement1 = (HtmlGenericControl)this.Master.FindControl("liTestManagement1");
            HtmlControl liQualityManagement1 = (HtmlGenericControl)this.Master.FindControl("liQualityManagement1");
            HtmlControl liCustomerFeedback1 = (HtmlGenericControl)this.Master.FindControl("liCustomerFeedback1");
            HtmlControl liUserManagement1 = (HtmlGenericControl)this.Master.FindControl("liUserManagement1");
            HtmlControl liSupport1 = (HtmlGenericControl)this.Master.FindControl("liSupport1");
            HtmlControl liViewInquiries2 = (HtmlGenericControl)this.Master.FindControl("liViewInquiries2");
            HtmlControl liInquiryStatus2 = (HtmlGenericControl)this.Master.FindControl("liInquiryStatus2");
            HtmlControl liNewCustomer = (HtmlGenericControl)this.Master.FindControl("liNewCustomer2");
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
                if (Session["RoleID"] != null)
                {
                    int RoleID = Convert.ToInt32(Session["RoleID"].ToString());
                }
                if (!Page.IsPostBack)
                {
                    BindGroup();
                    CheckgvPerson();
                    BindDepartment();
					BindLocationGrid();
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }

        }
        private void CheckgvPerson()
        {
            if (gvPerson.Rows.Count == 0) { btnPersonSave.Visible = false; }
            else { btnPersonSave.Visible = true; }
        }
        private void BindGroup()
        {
            DataTable dt = _cm.getGroupDetails();
            if (dt.Rows.Count > 0)
            {
                ddlGroupName.DataSource = dt;
                ddlGroupName.DataTextField = "GroupName";
                ddlGroupName.DataValueField = "GroupId";
                ddlGroupName.DataBind();
                ddlGroupName.Items.Insert(0, new ListItem("Select Group", ""));
            }
            else
            {
                ddlGroupName.DataSource = null;
                ddlGroupName.DataBind();
                ddlGroupName.Items.Insert(0, new ListItem("Select Group", "0"));
                ddlGroupName.Items.Insert(1, new ListItem("NA", "1"));
            }
        }
        private void BindDepartment()
        {
            DataTable dt = _cm.GetAllDepartments("LIMS",1);
            if (dt.Rows.Count > 0)
            {
                ddlDepartment.DataSource = dt;
                ddlDepartment.DataTextField = "DepartmentName";
                ddlDepartment.DataValueField = "DepartmentID";
                ddlDepartment.DataBind();
                ddlDepartment.Items.Insert(0, new ListItem("Select Department", ""));
            }
        }
        protected void btnGroupSave_Click(object sender, EventArgs e)
        {
            //System.Threading.Thread.Sleep(1000);
            int flag = 0;
            if (txtGroupName.Text == "")
            {
                txtGroupName.BackColor = Color.LightPink;
                flag = 1;
            }
            else
            {
                txtGroupName.BackColor = Color.Empty;
            }

        }
        protected void btnCustSave_Click(object sender, EventArgs e)
        {
            int flag = 0; int code = 0;

            if (ddlDepartment.SelectedIndex == 0)
            {
                ddlDepartment.BackColor = Color.LightPink;
                flag = 1;
            }
            else
            {
                ddlDepartment.BackColor = Color.Empty;
            }
            if (txtCompanyName.Text == "")
            {
                txtCompanyName.BackColor = Color.LightPink;
                flag = 1;
            }
            else
            {
                txtCompanyName.BackColor = Color.Empty;
                DataTable dt = _cm.checkCompanyNameExists(txtCompanyName.Text);
                DataTable dtCheckGroup = new DataTable();
                if (dt.Rows.Count > 0)
                {
                    if ((ddlGroupName.SelectedIndex != 0) && (ddlGroupName.SelectedIndex != 1))
                    {
                        if (ddlGroupName.Text == "")
                        {
                            ddlGroupName.BackColor = Color.LightPink;
                            flag = 1;
                        }
                        else
                        {
                            ddlGroupName.BackColor = Color.Empty;
                            int groupID = Convert.ToInt32(ddlGroupName.SelectedItem.Value);
                            dtCheckGroup = _cm.checkGroupCompanyNameExists(txtCompanyName.Text, groupID);
                            if (dtCheckGroup.Rows.Count > 0)
                            {
                                txtCompanyName.BackColor = Color.Empty;
                            }
                            else
                            {
                                txtCompanyName.BackColor = Color.LightPink;
                                code = 1;
                            }
                        }
                    }
                    if ((ddlGroupName.SelectedIndex == 1))
                    {
                        code = 1;
                    }

                }
            }
            if (ddlGroupName.Text == "")
            {
                ddlGroupName.BackColor = Color.LightPink;
                flag = 1;
            }
            else
            {
                ddlGroupName.BackColor = Color.Empty;
            }
            if (flag == 0)
            {
                if (Session["OrganizationID"] != null)
                {
                    int exists = 0;
                    _cm.OrganizationID = Convert.ToInt32(Session["OrganizationID"].ToString());
                    _cm.customerType = "Company";
                    if (code == 1)
                    {
                        divAlert.Visible = true;
                        lblMsg2.Visible = true;
                        lblMsg2.Text = "Company Name Exists!";
                        lblMsg2.ForeColor = Color.Red;
                    }
                    else
                    {
                        _cm.OrganizationID = Convert.ToInt32(Session["OrganizationID"].ToString());
                        customerUID = _cm.generateCustomerUID();
                        exists = _cm.checkCustomerUIDExists(customerUID);
                        while (exists == 1)
                        {
                            customerUID = _cm.generateCustomerUID();
                        }
                        _cm.CustomerUID = customerUID;
                        _cm.departmentID = Convert.ToInt32(ddlDepartment.SelectedValue);
                        custID = _cm.AddCustomer(_cm);
                        //AuditLog
                        DAL.Customer customer = new DAL.Customer
                        {
                            CustomerID = custID,
                            CustomerName = txtCompanyName.Text,
                            Address = txtCity.Text,
                            IsActive = true
                        };
                        if (Session["UserID"]!=null&& Session["UserName"] != null)
                        {
                            _cm.SaveAuditLog(
                               Convert.ToInt32(Session["UserID"].ToString()),
                               Session["UserName"].ToString(),
                               "INSERT",
                               "Masters",
                               "Customer",
                               custID.ToString(),
                               "",
                               JsonConvert.SerializeObject(customer),
                               "Customer created"
                           );
                        }
                       
                        Session["CustomerMasterId"] = custID;
                        lblMsg2.Visible = false;
                        _cm.customerType = "Company";
                        _cm.CustomerMasterId = Convert.ToInt32(Session["CustomerMasterId"]);
                        _cm.groupId = Convert.ToInt32(ddlGroupName.SelectedItem.Value);
                        _cm.CompanyName = txtCompanyName.Text.ToString();
                        _cm.TradingName = txtCompanyName.Text.ToString();
                        _cm.created = DateTime.Now.AddHours(12.5);
                        _cm.VAT = txtVAT.Text;

                        try
                        {
                            int company = _cm.AddCompanyDetails(_cm);
                            Session["CompanyId"] = company;
                            btnCustSave.Visible = false;
                            divAlert.Visible = true;
                            lblMsg.Text = "Click Next and then Save Button";
                            lblMsg.ForeColor = Color.Red;
                        }
                        catch(Exception ex)
                        {
                            string s = ex.Message;
                        }
                       
                    }
                }
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "notify", "alert(Fill Highlighted Fields.)", true);
            }

        }
        protected void btnContactSave_Click(object sender, EventArgs e)
        {

            if (Session["DT"] != null)
            {
                DataTable dt = (DataTable)Session["DT"];



                foreach (DataRow dr in dt.Rows)
                {
                    _cm.CustomerMasterId = Convert.ToInt32(Session["CustomerMasterId"]);
                    _cm.customerId = Convert.ToInt32(Session["CompanyId"]);
                    _cm.customerType = Convert.ToString(Session["CustomerType"]);
                    //_cm.personId = Convert.ToInt32(Session["PersonId"]);
                    _cm.personId = Convert.ToInt32(dr["ContactPersonID"].ToString());
                    _cm.mobile = dr["ContactMobile"].ToString();
                    _cm.email = dr["ContactEmail"].ToString();
                    _cm.phone = dr["ContactPhone"].ToString();
                    _cm.fax = dr["ContactFax"].ToString();

                    int ContactDetail = _cm.AddContactDetails(_cm);
                    btnContactSave.Visible = false;
                    divAlert.Visible = true;
                    lblMsg.Text = "Click Finish Button";
                    ClearContact();
                }

                Session["DT"] = null;
                Response.Redirect("CustomerProfile.aspx?ID=" + Convert.ToInt32(Session["CustomerMasterId"].ToString()));
            }


        }
        private void ClearContact()
        {
            txtMobile.Text = "";
            txtEmail.Text = "";
            txtPhone.Text = "";
            txtFax.Text = "";

        }
        protected void btnGroupNext_Click(object sender, EventArgs e)
        {
            //Group.Attributes["style"] = "display:none";
            //Customer.Attributes["style"] = "display:block";


        }
        protected void btnCustNext_Click(object sender, EventArgs e)
        {

        }
        protected void btnLocNext_Click(object sender, EventArgs e)
        {

        }
        protected void btnContactNext_Click(object sender, EventArgs e)
        {

        }
        protected void gvContact_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "del")
            {
                int index = int.Parse(e.CommandArgument.ToString());
                DataTable dt = (DataTable)Session["DT"];
                dt.Rows[index].Delete();
                gvContact.DataSource = dt;
                gvContact.DataBind();
                if (gvContact.Rows.Count == 0)
                {
                    btnContactSave.Visible = false;
                }
                else
                {
                    btnContactSave.Visible = true;
                }
            }
        }
        protected void btnAddGroup_Click(object sender, EventArgs e)
        {
            divGroupName.Visible = true;
            btnAdd.Visible = true;
            divGroupAddBtn.Visible = true;
        }
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            //System.Threading.Thread.Sleep(1000);
            int flag = 0;
            if (txtGroupName.Text == "")
            {
                txtGroupName.BackColor = Color.LightPink;
                flag = 1;
            }
            else
            {
                txtGroupName.BackColor = Color.Empty;
            }
            if (flag == 0)
            {
                int group = 0;
                _cm.CustomerMasterId = Convert.ToInt32(Session["CustomerMasterId"]);
                if (ddlGroupName.SelectedItem.Value == "NA")
                {
                    _cm.GroupName = "Not a Group";
                }
                else
                {
                    _cm.GroupName = txtGroupName.Text.ToString();
                }
                _cm.status = true;
                group = _cm.AddGroupDetails(_cm);
                Session["GroupId"] = group;
                divGroupName.Visible = false;
                divGroupAddBtn.Visible = false;
            }
            BindGroup();
        }

        protected void btnAddContact_Click(object sender, EventArgs e)
        {
            AddDetails();
            //ClearContact();
        }
        private void AddDetails()
        {
            int flag = 0; int code = 0;

            if (txtEmail.Text == "")
            {
                txtEmail.BackColor = Color.LightPink;
                flag = 1;
            }
            else
            {
                if ((Regex.IsMatch(txtEmail.Text, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase)))
                {
                    txtEmail.BackColor = Color.Empty;
                }
                else
                {
                    code = 1;
                    txtEmail.BackColor = Color.LightPink;
                }
            }
            if (ddlContactPerson.SelectedIndex == 0)
            {
                ddlContactPerson.BackColor = Color.LightPink;
                flag = 1;
            }
            else
            {
                ddlContactPerson.BackColor = Color.Empty;
            }
            if (flag == 0 && code == 0)
            {
                DataTable dt = new DataTable();
                if (Session["DT"] != null)
                {
                    dt = (DataTable)Session["DT"];
                }
                else
                {
                    dt.Columns.Add("ContactPerson");
                    dt.Columns.Add("ContactPersonID");
                    dt.Columns.Add("ContactMobile");
                    dt.Columns.Add("ContactEmail");
                    dt.Columns.Add("ContactPhone");
                    dt.Columns.Add("ContactFax");
                }
                DataRow dr = dt.NewRow();
                dr["ContactPerson"] = ddlContactPerson.SelectedItem.ToString();
                dr["ContactPersonID"] = ddlContactPerson.SelectedValue;
                dr["ContactMobile"] = txtMobile.Text;
                dr["ContactEmail"] = txtEmail.Text;
                dr["ContactPhone"] = txtPhone.Text;
                dr["ContactFax"] = txtFax.Text;

                dt.Rows.Add(dr);
                gvContact.DataSource = dt;
                gvContact.DataBind();
                ddlContactPerson.SelectedIndex = 0;
                if (gvContact.Rows.Count == 0)
                {
                    btnContactSave.Visible = false;
                }
                else
                {
                    btnContactSave.Visible = true;
                }
                Session["DT"] = dt;
                ClearContact();
            }
            else
            if (code == 0)
            {
                switch (code)
                {
                    case 1: divError.Visible = true; lblMsg.Text = "Invalid Email"; break;
                    case 2: divError.Visible = true; lblMsg.Text = "Invalid Mobile Number"; break;
                    case 3: divError.Visible = true; lblMsg.Text = "Invalid Phone Number"; break;
                    default: divError.Visible = false; break;
                }
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "notify", "alert(Fill Highlighted Fields.)", true);
            }
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
                txtPersonFirstName.BackColor = Color.LightPink;
                flag = 1;
            }
            else
            {
                txtPersonFirstName.BackColor = Color.Empty;
            }
            //if (txtPersonLastName.Text == "")
            //{
            //    txtPersonLastName.BackColor = Color.LightPink;
            //    flag = 1;
            //}
            //else
            //{
            //    txtPersonLastName.BackColor = Color.Empty;
            //}
            if (ddlPersonTitle.SelectedIndex == 0)
            {
                ddlPersonTitle.BackColor = Color.LightPink;
                flag = 1;
            }
            else
            {
                ddlPersonTitle.BackColor = Color.Empty;
            }
            if (txtDepartment.Text == "")
            {
                txtDepartment.BackColor = Color.LightPink;
                flag = 1;
            }
            else
            {
                txtDepartment.BackColor = Color.Empty;
            }

            if (ddlPersonAccountType.SelectedIndex == 0)
            {
                ddlPersonAccountType.BackColor = Color.LightPink;
                flag = 1;
            }
            else
            {
                ddlPersonAccountType.BackColor = Color.Empty;
            }

            if (flag == 0)
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
                    dt.Columns.Add("PersonAccountType");
                }

                bool isPrimaryExists = dt.AsEnumerable().Any(row => row.Field<string>("PersonAccountType") == "Primary");

                if (ddlPersonAccountType.SelectedItem.Text == "Primary" && isPrimaryExists)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Only one  Primary Account is allowed!');", true);
                    return;
                }
                else
                {
                    DataRow dr = dt.NewRow();
                    dr["Title"] = ddlPersonTitle.SelectedItem.Text.ToString();
                    dr["FirstName"] = txtPersonFirstName.Text.ToString();
                    dr["MiddleName"] = txtPersonMiddleName.Text.ToString();
                    dr["LastName"] = txtPersonLastName.Text.ToString();
                    dr["Designation"] = txtDesignation.Text.ToString();
                    dr["Department"] = txtDepartment.Text.ToString();
                    dr["PersonAccountType"] = ddlPersonAccountType.SelectedItem.Text.ToString();

                    dt.Rows.Add(dr);
                    gvPerson.DataSource = dt;
                    gvPerson.DataBind();
                    if (gvPerson.Rows.Count == 0)
                    {
                        btnPersonSave.Visible = false;
                    }
                    else
                    {
                        btnPersonSave.Visible = true;
                    }
                    Session["DTPerson"] = dt;
                    Clear();
                }
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
            ddlPersonAccountType.SelectedIndex = 0;
        }
        protected void btnPersonSave_Click(object sender, EventArgs e)
        {
            //System.Threading.Thread.Sleep(1000);
            int PersonMst = 0;
            if (Session["DTPerson"] != null)
            {
                DataTable dt = (DataTable)Session["DTPerson"];
                bool hasPrimary = dt.AsEnumerable()
           .Any(row => row["PersonAccountType"].ToString().Equals("Primary", StringComparison.OrdinalIgnoreCase));

                if (!hasPrimary)
                {

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please add at least one Primary Contact Person before saving.');", true);
                    return;
                }

                foreach (DataRow dr in dt.Rows)
                {
                    _cm.title = dr["Title"].ToString();
                    _cm.customerType = Convert.ToString(Session["CustomerType"]);
                    _cm.CustomerMasterId = Convert.ToInt32(Session["CustomerMasterId"]);
                    _cm.firstName = dr["FirstName"].ToString();
                    _cm.middleName = dr["MiddleName"].ToString();
                    _cm.lastName = dr["LastName"].ToString();
                    _cm.designation = dr["Designation"].ToString();
                    _cm.department = dr["Department"].ToString();
                    _cm.personAccountType = dr["PersonAccountType"].ToString();
                    
                    try
                    {
                        PersonMst = _cm.AddPersonDetails(_cm);
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.LogError(ex);
                    }
                    Session["PersonId"] = PersonMst;
                }
                btnPersonSave.Visible = false;
                //Session["DTPerson"] = null;
                BindContactPerson();
            }
        }
        private void BindContactPerson()
        {
            DataTable dt = _cm.getAllContactPersonDetails(Convert.ToInt32(Session["CustomerMasterId"]));
            if (dt.Rows.Count > 0)
            {
                ddlContactPerson.DataSource = dt;
                ddlContactPerson.DataTextField = "FirstName";
                ddlContactPerson.DataValueField = "PersonID";
                ddlContactPerson.DataBind();
                ddlContactPerson.Items.Insert(0, new ListItem("Select Contact Person", ""));
                //ddlGroupName.Items.Insert(1, new ListItem("Not a Group", "NA"));
            }
        }

        protected void testadd_Click(object sender, EventArgs e)
        {

        }

        protected void gvPerson_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "del")
            {
                if (Session["DTPerson"] != null)
                {
                    DataTable dt = (DataTable)Session["DTPerson"];
                    int index;
                    if (int.TryParse(e.CommandArgument.ToString(), out index))
                    {
                        if (index >= 0 && index < dt.Rows.Count)
                        {
                            dt.Rows[index].Delete();
                            dt.AcceptChanges();
                            Session["DTPerson"] = dt;

                            gvPerson.DataSource = dt;
                            gvPerson.DataBind();

                            btnPersonSave.Visible = (gvPerson.Rows.Count > 0);
                        }
                    }
                }
            }

        }


        protected void ddlContactPerson_SelectedIndexChanged(object sender, EventArgs e)
        {
            string contactperson = ddlContactPerson.SelectedItem.ToString();
            if (Session["DT"] != null)
            {
                DataTable dt = (DataTable)Session["DT"];
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["ContactPerson"].ToString() == contactperson)
                    {
                        ddlContactPerson.BackColor = Color.LightPink;
                        lblContactExists.ForeColor = Color.Red;
                        lblContactExists.Visible = true;
                    }
                    else
                    {
                        ddlContactPerson.BackColor = Color.Empty;
                        lblContactExists.Visible = false;
                    }
                }
            }
        }

        protected void ddlGroupName_SelectedIndexChanged(object sender, EventArgs e)
        {
            string group = ddlGroupName.SelectedValue;
            if (group != "")
            {
                divGroupName.Visible = false;
                divbtnAddGroup.Visible = false;
                divGroupAddBtn.Visible = false;
            }
            else
            if (group == "")
            {
                divGroupName.Visible = false;
                divbtnAddGroup.Visible = true;
                divGroupAddBtn.Visible = false;
            }
            else
            {
                divGroupName.Visible = true;
                divbtnAddGroup.Visible = true;
                divGroupAddBtn.Visible = true;
            }
        }
        // ViewState Properties
        public AddressModel InvoiceAddress
        {
            get { return ViewState["InvoiceAddress"] as AddressModel ?? new AddressModel(); }
            set { ViewState["InvoiceAddress"] = value; }
        }
        public AddressModel ShippingAddress
        {
            get { return ViewState["ShippingAddress"] as AddressModel ?? new AddressModel(); }
            set { ViewState["ShippingAddress"] = value; }
        }
        public bool InvoiceSaved
        {
            get { return ViewState["InvoiceSaved"] != null && (bool)ViewState["InvoiceSaved"]; }
            set { ViewState["InvoiceSaved"] = value; }
        }

        public class AddressModel
        {
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string City { get; set; }
            public string Postal { get; set; }
            public string Country { get; set; }
        }
        protected void chkAddressSame_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAddressSame.Checked)
            {
                radLocationType.Enabled = false;
                radLocationType.SelectedValue = "1";

                //  Check if any address already exists in grid
                if (Session["DTLocation"] != null)
                {
                    DataTable dt = (DataTable)Session["DTLocation"];
                    bool hasInvoice = false;
                    bool hasSend = false;
                    DataRow invoiceRow = null;
                    DataRow sendRow = null;

                    foreach (DataRow row in dt.Rows)
                    {
                        string locType = row["LocationType"].ToString();
                        if (locType == "Invoice Address")
                        {
                            hasInvoice = true;
                            invoiceRow = row;
                        }
                        if (locType == "Send Address")
                        {
                            hasSend = true;
                            sendRow = row;
                        }
                    }

                    // Scenario 1: Only Invoice exists - Copy to Send
                    if (hasInvoice && !hasSend && invoiceRow != null)
                    {
                        DataRow drSend = dt.NewRow();
                        drSend["LocationType"] = "Send Address";
                        drSend["AddressLine1"] = invoiceRow["AddressLine1"].ToString();
                        drSend["AddressLine2"] = invoiceRow["AddressLine2"].ToString();
                        drSend["City"] = invoiceRow["City"].ToString();
                        drSend["PostalCode"] = invoiceRow["PostalCode"].ToString();
                        drSend["Country"] = invoiceRow["Country"].ToString();
                        dt.Rows.Add(drSend);

                        Session["DTLocation"] = dt;
                        BindLocationGrid();
                        btnLocSave.Visible = true;

                        ShowAlertMessage("Invoice Address copied to Send Address successfully!");
                    }
                    // 2: Only Send exists - Copy to Invoice
                    else if (!hasInvoice && hasSend && sendRow != null)
                    {
                        DataRow drInvoice = dt.NewRow();
                        drInvoice["LocationType"] = "Invoice Address";
                        drInvoice["AddressLine1"] = sendRow["AddressLine1"].ToString();
                        drInvoice["AddressLine2"] = sendRow["AddressLine2"].ToString();
                        drInvoice["City"] = sendRow["City"].ToString();
                        drInvoice["PostalCode"] = sendRow["PostalCode"].ToString();
                        drInvoice["Country"] = sendRow["Country"].ToString();
                        dt.Rows.Add(drInvoice);

                        Session["DTLocation"] = dt;
                        BindLocationGrid();
                        btnLocSave.Visible = true;

                        ShowAlertMessage("Send Address copied to Invoice Address successfully!");
                    }
                    // Scenario 3: Both already exist
                    else if (hasInvoice && hasSend)
                    {
                        ShowAlertMessage("Both addresses already exist in grid!");
                    }
                    //  No address exists yet
                    else
                    {
                        ShowAlertMessage("Enter address details and click + button. Both Invoice and Send Address will be added.");
                    }
                }
                else
                {
                    ShowAlertMessage("Enter address details and click + button. Both Invoice and Send Address will be added.");
                }
            }
            else
            {
                radLocationType.Enabled = true;
            }
        }
        protected void radLocationType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //// Only save if fields have data (prevents overwriting with empty fields)
            //if (!string.IsNullOrWhiteSpace(txtLocLine1.Text) ||
            //    !string.IsNullOrWhiteSpace(txtCity.Text))
            //{
            //    SaveCurrentFieldsToModel();
            //}

            //// Load the selected address type
            //if (radLocationType.SelectedValue == "1")
            //    LoadAddressToFields(InvoiceAddress);
            //else
            //    LoadAddressToFields(ShippingAddress);
        }
        protected void btnlocation_Click(object sender, EventArgs e)
        {        

            AddlocationDetails();
        }

        private void AddlocationDetails()
        {
            try
            {
                // Validation
                int flag = 0;

                if (string.IsNullOrWhiteSpace(txtLocLine1.Text))
                {
                    txtLocLine1.BackColor = System.Drawing.Color.LightPink;
                    flag = 1;
                }
                else
                {
                    txtLocLine1.BackColor = System.Drawing.Color.Empty;
                }

                if (string.IsNullOrWhiteSpace(txtCity.Text))
                {
                    txtCity.BackColor = System.Drawing.Color.LightPink;
                    flag = 1;
                }
                else
                {
                    txtCity.BackColor = System.Drawing.Color.Empty;
                }

                if (string.IsNullOrWhiteSpace(txtPincode.Text))
                {
                    txtPincode.BackColor = System.Drawing.Color.LightPink;
                    flag = 1;
                }
                else
                {
                    txtPincode.BackColor = System.Drawing.Color.Empty;
                }

                if (ddlCountry.SelectedIndex == 0)
                {
                    ddlCountry.BackColor = System.Drawing.Color.LightPink;
                    flag = 1;
                }
                else
                {
                    ddlCountry.BackColor = System.Drawing.Color.Empty;
                }

                if (flag == 1)
                {
                    ShowAlertMessage("Please fill all highlighted fields.");
                    return;
                }

                // Get or Create DataTable
                DataTable dt;

                if (Session["DTLocation"] != null)
                {
                    dt = (DataTable)Session["DTLocation"];
                }
                else
                {
                    dt = new DataTable();
                    dt.Columns.Add("LocationType", typeof(string));
                    dt.Columns.Add("AddressLine1", typeof(string));
                    dt.Columns.Add("AddressLine2", typeof(string));
                    dt.Columns.Add("City", typeof(string));
                    dt.Columns.Add("PostalCode", typeof(string));
                    dt.Columns.Add("Country", typeof(string));
                }

                // Check if "Both Same" is checked
                if (chkAddressSame.Checked)
                {
                    // Check if both already exist
                    bool hasInvoice = false;
                    bool hasSend = false;

                    foreach (DataRow row in dt.Rows)
                    {
                        string locType = row["LocationType"].ToString();
                        if (locType == "Invoice Address")
                            hasInvoice = true;
                        if (locType == "Send Address")
                            hasSend = true;
                    }

                    if (hasInvoice && hasSend)
                    {
                        ShowAlertMessage("Both Invoice and Send Address already exist in grid! Please delete existing entries first.");
                        return;
                    }

                    // Add Invoice Address
                    DataRow drInvoice = dt.NewRow();
                    drInvoice["LocationType"] = "Invoice Address";
                    drInvoice["AddressLine1"] = txtLocLine1.Text.Trim();
                    drInvoice["AddressLine2"] = txtLocLine2.Text.Trim();
                    drInvoice["City"] = txtCity.Text.Trim();
                    drInvoice["PostalCode"] = txtPincode.Text.Trim();
                    drInvoice["Country"] = ddlCountry.SelectedItem.Text;
                    dt.Rows.Add(drInvoice);

                    // Add Send Address
                    DataRow drSend = dt.NewRow();
                    drSend["LocationType"] = "Send Address";
                    drSend["AddressLine1"] = txtLocLine1.Text.Trim();
                    drSend["AddressLine2"] = txtLocLine2.Text.Trim();
                    drSend["City"] = txtCity.Text.Trim();
                    drSend["PostalCode"] = txtPincode.Text.Trim();
                    drSend["Country"] = ddlCountry.SelectedItem.Text;
                    dt.Rows.Add(drSend);
                }
                else
                {
                    // Check if selected address type already exists
                    string selectedType = radLocationType.SelectedItem.Text;
                    bool addressExists = false;

                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["LocationType"].ToString() == selectedType)
                        {
                            addressExists = true;
                            break;
                        }
                    }

                    if (addressExists)
                    {
                        ShowAlertMessage(selectedType + " already exists in grid! Please delete the existing entry first.");
                        return;
                    }

                    // Add single selected address type
                    DataRow dr = dt.NewRow();
                    dr["LocationType"] = selectedType;
                    dr["AddressLine1"] = txtLocLine1.Text.Trim();
                    dr["AddressLine2"] = txtLocLine2.Text.Trim();
                    dr["City"] = txtCity.Text.Trim();
                    dr["PostalCode"] = txtPincode.Text.Trim();
                    dr["Country"] = ddlCountry.SelectedItem.Text;
                    dt.Rows.Add(dr);
                }

                // Save to session
                Session["DTLocation"] = dt;

                // Bind to grid
                BindLocationGrid();

                // Clear fields
                ClearFields();

                // Show Save button
                btnLocSave.Visible = true;

                ShowAlertMessage("Address added to grid successfully!");
            }
            catch (Exception ex)
            {
                ShowAlertMessage("Error: " + ex.Message);
            }
        }


        // Bind Location Grid
        private void BindLocationGrid()
        {
            if (Session["DTLocation"] != null)
            {
                DataTable dt = (DataTable)Session["DTLocation"];
                gvlocation.DataSource = dt;
                gvlocation.DataBind();
                btnLocSave.Visible = (dt.Rows.Count > 0);
            }
            else
            {
                gvlocation.DataSource = null;
                gvlocation.DataBind();
                btnLocSave.Visible = false;
            }
        }


      
        // GridView Row Command Event (for Delete functionality)
        protected void gvlocation_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "del")
            {
                try
                {
                    int index = Convert.ToInt32(e.CommandArgument);

                    if (Session["DTLocation"] != null)
                    {
                        DataTable dt = (DataTable)Session["DTLocation"];

                        if (index >= 0 && index < dt.Rows.Count)
                        {
                            dt.Rows[index].Delete();
                            dt.AcceptChanges();

                            Session["DTLocation"] = dt;
                            BindLocationGrid();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowAlertMessage("Error deleting: " + ex.Message);
                }
            }
        }
        protected void btnLocSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["DTLocation"] != null)
                {
                    DataTable dt = (DataTable)Session["DTLocation"];

                    if (dt.Rows.Count > 0)
                    {
                        // Check if both address types exist
                        bool hasInvoice = false;
                        bool hasSend = false;

                        foreach (DataRow row in dt.Rows)
                        {
                            string locType = row["LocationType"].ToString();
                            if (locType == "Invoice Address")
                                hasInvoice = true;
                            if (locType == "Send Address")
                                hasSend = true;
                        }

                        // Validation - Both types must exist
                        if (!hasInvoice && !hasSend)
                        {
                            ShowAlertMessage("Please add at least one address type!");
                            return;
                        }
                        else if (!hasInvoice)
                        {
                            ShowAlertMessage("Please add Invoice Address before saving!");
                            return;
                        }
                        else if (!hasSend)
                        {
                            ShowAlertMessage("Please add Send Address before saving!");
                            return;
                        }

                        // Save to database
                        _cm.CustomerMasterId = Convert.ToInt32(Session["CustomerMasterId"]);
                        _cm.customerId = Convert.ToInt32(Session["CompanyId"]);
                        _cm.customerType = Convert.ToString(Session["CustomerType"]);

                        foreach (DataRow row in dt.Rows)
                        {
                            _cm.addressLine1 = row["AddressLine1"].ToString();
                            _cm.addressLine2 = row["AddressLine2"].ToString();
                            _cm.city = row["City"].ToString();
                            _cm.pincode = row["PostalCode"].ToString();
                            _cm.country = row["Country"].ToString();

                            string locType = row["LocationType"].ToString();
                            if (locType == "Invoice Address")
                                _cm.locationAddressType = "Invoice";
                            else
                                _cm.locationAddressType = "Shipping";

                            _cm.location = "";
                            _cm.locationName = "";
                            _cm.addressLine3 = "";
                            _cm.state = "";
                            _cm.district = "";
                            _cm.taluka = "";
                            _cm.suburb = "";
                            _cm.status = true;

                            int result = _cm.AddLocationDetails(_cm);
                        }

                        // Clear session and grid
                        Session["DTLocation"] = null;
                        BindLocationGrid();

                        ShowAlertMessage("All addresses saved successfully!");
                    }
                    else
                    {
                        ShowAlertMessage("No data to save! Please add addresses first.");
                    }
                }
                else
                {
                    ShowAlertMessage("No data to save! Please add addresses first.");
                }
            }
            catch (Exception ex)
            {
                ShowAlertMessage("Error saving: " + ex.Message);
            }
        }


        private void ClearFields()
        {
            txtLocLine1.Text = "";
            txtLocLine2.Text = "";
            txtCity.Text = "";
            txtPincode.Text = "";
            ddlCountry.SelectedIndex = 0;
            chkAddressSame.Checked = false;

            // Reset background colors
            txtLocLine1.BackColor = System.Drawing.Color.Empty;
            txtCity.BackColor = System.Drawing.Color.Empty;
            txtPincode.BackColor = System.Drawing.Color.Empty;
            ddlCountry.BackColor = System.Drawing.Color.Empty;
        }

        // Show Alert Message
        private void ShowAlertMessage(string message)
        {
            string script = $"alert('{message.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", script, true);
        }

     

    }
}