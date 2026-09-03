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
    public partial class AddUser : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int flag = 1;
        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlControl liUserManagement1 = (HtmlGenericControl)this.Master.FindControl("liUserManagement1");
            HtmlControl liAddUser2 = (HtmlGenericControl)this.Master.FindControl("liAddUser2");            

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
                if (!Page.IsPostBack)
                {
                    liUserManagement1.Attributes.Add("class", "active");
                    liAddUser2.Attributes.Add("class", "active");
                    BindEmployees();
                    BindRoles();
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

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int exists = 0;
            if(txtUserName.Text == "")
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
                        Response.Redirect("Users.aspx");
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