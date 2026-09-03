using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class AddEmployee : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int flag = 1;
        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlControl liUserManagement1 = (HtmlGenericControl)this.Master.FindControl("liUserManagement1");
            HtmlControl liAddEmployee2 = (HtmlGenericControl)this.Master.FindControl("liAddEmployee2");
            
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
                    liAddEmployee2.Attributes.Add("class", "active");
                    BindDesignation();
                    BindDepartment();
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
        private void BindDesignation()
        {
            DataTable dt = _cm.GetAllDesignations();
            if (dt.Rows.Count > 0)
            {
                ddlDesignation.DataSource = dt;
                ddlDesignation.DataTextField = "DesignationName";
                ddlDesignation.DataValueField = "DesignationID";
                ddlDesignation.DataBind();
                ddlDesignation.Items.Insert(0, new ListItem("Select Designation", ""));
            }

        }
        private void BindDepartment()
        {
            DataTable dt = _cm.GetAllDepartments();
            if (dt.Rows.Count > 0)
            {
                ddlDepartment.DataSource = dt;
                ddlDepartment.DataTextField = "DepartmentName";
                ddlDepartment.DataValueField = "DepartmentID";
                ddlDepartment.DataBind();
                ddlDepartment.Items.Insert(0, new ListItem("Select Department", ""));
            }

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int exists = 0;int code = 0;
            if(txtName.Text == "")
            {
                flag = 0;
                txtName.BackColor = System.Drawing.Color.LightPink;
            }
            else
            {
                txtName.BackColor = System.Drawing.Color.Empty;
            }
            if (txtEmployeeID.Text == "")
            {
                flag = 0;
                txtEmployeeID.BackColor = System.Drawing.Color.LightPink;
            }
            else
            {
                txtEmployeeID.BackColor = System.Drawing.Color.Empty;
            }
            if (txtEmail.Text == "")
            {
                flag = 0;
                txtEmail.BackColor = System.Drawing.Color.LightPink;
            }
            else
            {
                txtEmail.BackColor = System.Drawing.Color.Empty;
                //if (!(System.Text.RegularExpressions.Regex.IsMatch(txtEmail.Text, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")))
                if((Regex.IsMatch(txtEmail.Text, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase)))
                {
                    txtEmail.BackColor = System.Drawing.Color.Empty;
                }
                else
                {
                    code = 1;
                    txtEmail.BackColor = System.Drawing.Color.LightPink;
                }
            }
            if (txtMobile.Text == "")
            {
                flag = 0;
                txtMobile.BackColor = System.Drawing.Color.LightPink;
            }
            else
            {
                txtMobile.BackColor = System.Drawing.Color.Empty;
                if (!(System.Text.RegularExpressions.Regex.IsMatch(txtMobile.Text, @"[^0-9^/^-]")))
                {
                    txtMobile.BackColor = System.Drawing.Color.Empty;
                }
                else
                {
                    code = 2;
                    txtMobile.BackColor = System.Drawing.Color.LightPink;
                }
            }
            if (txtPhone.Text == "")
            {
                flag = 0;
                txtPhone.BackColor = System.Drawing.Color.LightPink;
            }
            else
            {
                txtPhone.BackColor = System.Drawing.Color.Empty;
                if (!(System.Text.RegularExpressions.Regex.IsMatch(txtPhone.Text, @"[^0-9^/^-]")))
                {
                    txtPhone.BackColor = System.Drawing.Color.Empty;
                }
                else
                {
                    code = 3;
                    txtPhone.BackColor = System.Drawing.Color.LightPink;
                }
            }
            if (ddlDesignation.SelectedIndex == 0)
            {
                flag = 0;
                ddlDesignation.BackColor = System.Drawing.Color.LightPink;
            }
            else
            {
                ddlDesignation.BackColor = System.Drawing.Color.Empty;
            }
            if (ddlDepartment.SelectedIndex == 0)
            {
                flag = 0;
                ddlDepartment.BackColor = System.Drawing.Color.LightPink;
            }
            else
            {
                ddlDepartment.BackColor = System.Drawing.Color.Empty;
            }
            if (flag == 1 && code == 0)
            {
                exists = _cm.checkEmployeeExists(txtName.Text);
                if(exists == 0)
                {
                    _cm.EmployeeID = Convert.ToInt32(txtEmployeeID.Text);
                    _cm.EmployeeName = txtName.Text;
                    _cm.phone = txtPhone.Text;
                    _cm.mobile = txtMobile.Text;
                    _cm.email = txtEmail.Text;
                    _cm.designationID = Convert.ToInt32(ddlDesignation.SelectedValue);
                    _cm.employeeDepartmentID = Convert.ToInt32(ddlDepartment.SelectedValue);
                    int employeeID = _cm.AddEmployee(_cm);
                    Clear();
                    Response.Redirect("Employees.aspx");
                }
                else
                {
                    divError.Visible = true;
                    lblMsg.Text = "Employee Name Exists";
                }
            }
            else
            {
                switch (code)
                {
                    case 1: divError.Visible = true; lblMsg.Text = "Invalid Email";break;
                    case 2: divError.Visible = true; lblMsg.Text = "Invalid Mobile Number"; break;
                    case 3: divError.Visible = true; lblMsg.Text = "Invalid Phone Number"; break;
                    default: divError.Visible = false; break;
                }
            }
        }
        private void Clear()
        {
            txtEmployeeID.Text = "";
            txtName.Text = "";
            txtPhone.Text = "";
            txtMobile.Text = "";
            txtEmail.Text = "";
            ddlDepartment.SelectedIndex = -1;
            ddlDesignation.SelectedIndex = -1;
        }
      
    }
}