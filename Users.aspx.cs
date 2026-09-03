using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class Users : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();int flag = 1;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usertype"] != null)
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
                HtmlControl liViewUsers2 = (HtmlGenericControl)this.Master.FindControl("liViewUsers2");
                
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
                if (!Page.IsPostBack)
                {
                    liUserManagement1.Attributes.Add("class", "active");
                    liViewUsers2.Attributes.Add("class", "active");
                    BindEmployees();
                    BindRoles();
                    BindUsers();
                }
                if (Session["RoleID"] != null)
                {
                    int RoleID = Convert.ToInt32(Session["RoleID"].ToString());
                    switch (RoleID)
                    {
                        case 1://QC
                            liHome1.Visible = true;
                            liCustomers1.Visible = true;
                            liInquiries1.Visible = true;
                            liCRFList1.Visible = true;
                            liSampleManagement1.Visible = true;
                            liTestManagement1.Visible = false;
                            liQualityManagement1.Visible = true;
                            liCustomerFeedback1.Visible = false;
                            liUserManagement1.Visible = false;
                            liSupport1.Visible = false;
                            break;
                        case 2://Manager
                            liHome1.Visible = true;
                            liCustomers1.Visible = false;
                            liInquiries1.Visible = false;
                            liCRFList1.Visible = false;
                            liSampleManagement1.Visible = true;
                            liTestManagement1.Visible = true;
                            liQualityManagement1.Visible = true;
                            liCustomerFeedback1.Visible = false;
                            liUserManagement1.Visible = false;
                            liSupport1.Visible = false;
                            break;
                        case 3://Lab Technician
                            liHome1.Visible = true;
                            liCustomers1.Visible = false;
                            liInquiries1.Visible = false;
                            liCRFList1.Visible = false;
                            liSampleManagement1.Visible = false;
                            liTestManagement1.Visible = true;
                            liQualityManagement1.Visible = false;
                            liCustomerFeedback1.Visible = false;
                            liUserManagement1.Visible = false;
                            liSupport1.Visible = false;
                            break;
                        case 4://Admin
                            liHome1.Visible = true;
                            liCustomers1.Visible = true;
                            liInquiries1.Visible = true;
                            liCRFList1.Visible = true;
                            liSampleManagement1.Visible = true;
                            liTestManagement1.Visible = true;
                            liQualityManagement1.Visible = true;
                            liCustomerFeedback1.Visible = false;
                            liUserManagement1.Visible = false;
                            liSupport1.Visible = false;
                            break;
                        case 5://Sample Management
                            liHome1.Visible = true;
                            liCustomers1.Visible = true;
                            liInquiries1.Visible = true;
                            liCRFList1.Visible = true;
                            liSampleManagement1.Visible = true;
                            liTestManagement1.Visible = false;
                            liQualityManagement1.Visible = false;
                            liCustomerFeedback1.Visible = false;
                            liUserManagement1.Visible = false;
                            liSupport1.Visible = false;
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
        private void BindEmployees()
        {
            DataTable dt = _cm.GetEmployeeDetails();
            if (dt.Rows.Count > 0)
            {
                ddlEmployees.DataSource = dt;
                ddlEmployees.DataTextField = "EmployeeName";
                ddlEmployees.DataValueField = "ID";
                ddlEmployees.DataBind();
                ddlEmployees.Items.Insert(0, new ListItem("Select Employee", ""));
            }

        }
        private void BindRoles()
        {
            DataTable dt = _cm.getAllRoles();
            if (dt.Rows.Count > 0)
            {
                ddlRoles.DataSource = dt;
                ddlRoles.DataTextField = "RoleName";
                ddlRoles.DataValueField = "RoleID";
                ddlRoles.DataBind();
                ddlRoles.Items.Insert(0, new ListItem("Select Role", ""));
            }

        }
        private void BindUsers()
        {
            DataTable dt = _cm.GetUserDetails();
            gvUsers.DataSource = dt;
            gvUsers.DataBind();
        }

        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            divAddUser.Visible = true;
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int exists = 0;
            if (txtUserName.Text == "")
            {
                flag = 0;
                txtUserName.BackColor = System.Drawing.Color.LightPink;
            }
            else
            {
                txtUserName.BackColor = System.Drawing.Color.Empty;
            }
            if (txtPassword.Text == "")
            {
                flag = 0;
                txtPassword.BackColor = System.Drawing.Color.LightPink;
            }
            else
            {
                txtPassword.BackColor = System.Drawing.Color.Empty;
            }
            if (ddlEmployees.SelectedIndex == 0)
            {
                flag = 0;
                ddlEmployees.BackColor = System.Drawing.Color.LightPink;
            }
            else
            {
                ddlEmployees.BackColor = System.Drawing.Color.Empty;
            }
            if (ddlRoles.SelectedIndex == 0)
            {
                flag = 0;
                ddlRoles.BackColor = System.Drawing.Color.LightPink;
            }
            else
            {
                ddlRoles.BackColor = System.Drawing.Color.Empty;
            }
            if (ddlStatus.SelectedIndex == 0)
            {
                flag = 0;
                ddlStatus.BackColor = System.Drawing.Color.LightPink;
            }
            else
            {
                ddlStatus.BackColor = System.Drawing.Color.Empty;
            }
            if (flag == 1)
            {
                if (Session["OrganizationID"] != null)
                {
                    _cm.OrganizationID = Convert.ToInt32(Session["OrganizationID"].ToString());

                    exists = _cm.checkEmployeeExists(txtUserName.Text);
                    if (exists == 0)
                    {
                        _cm.password = txtPassword.Text;
                        _cm.username = txtUserName.Text;
                        _cm.empID = Convert.ToInt32(ddlEmployees.SelectedValue);
                        _cm.roleID = Convert.ToInt32(ddlRoles.SelectedValue);
                        _cm.userStatus = Convert.ToInt32(ddlStatus.SelectedValue);
                        int userID = _cm.AddUser(_cm);
                        Clear();
                        BindUsers();
                        divAddUser.Visible = false;
                    }
                    else
                    {
                        divError.Visible = true;
                        lblMsg.Text = "Employee Name Exists";
                    }
                }
            }
        }
        private void Clear()
        {
            txtPassword.Text = "";
            txtUserName.Text = "";
            ddlRoles.SelectedIndex = -1;
            ddlEmployees.SelectedIndex = -1;
        }
    }
}