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
    public partial class Employees : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int flag = 1;
        protected void Page_Load(object sender, EventArgs e)
        {
   //         HtmlControl liHome1 = (HtmlGenericControl)this.Master.FindControl("liHome1");
   //         HtmlControl liCustomers1 = (HtmlGenericControl)this.Master.FindControl("liCustomers1");
   //         HtmlControl liInquiries1 = (HtmlGenericControl)this.Master.FindControl("liInquiries1");
   //         HtmlControl liCRFList1 = (HtmlGenericControl)this.Master.FindControl("liCRFList1");
   //         HtmlControl liSampleManagement1 = (HtmlGenericControl)this.Master.FindControl("liSampleManagement1");
   //         HtmlControl liTestManagement1 = (HtmlGenericControl)this.Master.FindControl("liTestManagement1");
   //         HtmlControl liQualityManagement1 = (HtmlGenericControl)this.Master.FindControl("liQualityManagement1");
   //         HtmlControl liCustomerFeedback1 = (HtmlGenericControl)this.Master.FindControl("liCustomerFeedback1");
   //         HtmlControl liUserManagement1 = (HtmlGenericControl)this.Master.FindControl("liUserManagement1");
   //         HtmlControl liSupport1 = (HtmlGenericControl)this.Master.FindControl("liSupport1");
   //         HtmlControl liViewInquiries2 = (HtmlGenericControl)this.Master.FindControl("liViewInquiries2");
			//HtmlControl liInquiryStatus2 = (HtmlGenericControl)this.Master.FindControl("liInquiryStatus2");
   //         HtmlControl liCustomers = (HtmlGenericControl)this.Master.FindControl("liCustomers1");
   //         HtmlControl liViewEmployee2 = (HtmlGenericControl)this.Master.FindControl("liViewEmployee2");
            
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
                    //liUserManagement1.Attributes.Add("class", "active");
                    //liViewEmployee2.Attributes.Add("class", "active");
                    BindEmployees();
                    BindDesignation();
                    BindDepartment();
                    BindRoles();
                }
                if (Session["RoleID"] != null)
                {
                    int RoleID = Convert.ToInt32(Session["RoleID"].ToString());
                    //switch (RoleID)
                    //{
                    //    case 1://QC
                    //        liHome1.Visible = true;
                    //        liCustomers1.Visible = true;
                    //        liInquiries1.Visible = true;
                    //        liCRFList1.Visible = true;
                    //        liSampleManagement1.Visible = true;
                    //        liTestManagement1.Visible = false;
                    //        liQualityManagement1.Visible = true;
                    //        liCustomerFeedback1.Visible = false;
                    //        liUserManagement1.Visible = false;
                    //        liSupport1.Visible = false;
                    //        break;
                    //    case 2://Manager
                    //        liHome1.Visible = true;
                    //        liCustomers1.Visible = false;
                    //        liInquiries1.Visible = false;
                    //        liCRFList1.Visible = false;
                    //        liSampleManagement1.Visible = true;
                    //        liTestManagement1.Visible = true;
                    //        liQualityManagement1.Visible = true;
                    //        liCustomerFeedback1.Visible = false;
                    //        liUserManagement1.Visible = false;
                    //        liSupport1.Visible = false;
                    //        break;
                    //    case 3://Lab Technician
                    //        liHome1.Visible = true;
                    //        liCustomers1.Visible = false;
                    //        liInquiries1.Visible = false;
                    //        liCRFList1.Visible = false;
                    //        liSampleManagement1.Visible = false;
                    //        liTestManagement1.Visible = true;
                    //        liQualityManagement1.Visible = false;
                    //        liCustomerFeedback1.Visible = false;
                    //        liUserManagement1.Visible = false;
                    //        liSupport1.Visible = false;
                    //        break;
                    //    case 4://Admin
                    //        liHome1.Visible = true;
                    //        liCustomers1.Visible = true;
                    //        liInquiries1.Visible = true;
                    //        liCRFList1.Visible = true;
                    //        liSampleManagement1.Visible = true;
                    //        liTestManagement1.Visible = true;
                    //        liQualityManagement1.Visible = true;
                    //        liCustomerFeedback1.Visible = false;
                    //        liUserManagement1.Visible = false;
                    //        liSupport1.Visible = false;
                    //        break;
                    //    case 5://Sample Management
                    //        liHome1.Visible = true;
                    //        liCustomers1.Visible = true;
                    //        liInquiries1.Visible = true;
                    //        liCRFList1.Visible = true;
                    //        liSampleManagement1.Visible = true;
                    //        liTestManagement1.Visible = false;
                    //        liQualityManagement1.Visible = false;
                    //        liCustomerFeedback1.Visible = false;
                    //        liUserManagement1.Visible = false;
                    //        liSupport1.Visible = false;
                    //        break;
                    //    default:
                    //        break;
                    //}
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
        private void BindRoles()
        {
            DataTable dt = _cm.getAllRoles();
            if (dt.Rows.Count > 0)
            {
                ddlDepartment.DataSource = dt;
                ddlDepartment.DataTextField = "RoleName";
                ddlDepartment.DataValueField = "RoleID";
                ddlDepartment.DataBind();
                ddlDepartment.Items.Insert(0, new ListItem("Select Role", ""));
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int exists = 0; int code = 0;
            if (txtName.Text == "")
            {
                flag = 0;
                txtName.BackColor = System.Drawing.Color.LightPink;
            }
            else
            {
                txtName.BackColor = System.Drawing.Color.Empty;
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
            if (ddlSupervisor.SelectedIndex == 0)
            {
                flag = 0;
                ddlSupervisor.BackColor = System.Drawing.Color.LightPink;
            }
            else
            {
                ddlSupervisor.BackColor = System.Drawing.Color.Empty;
            }
            if (flag == 1 && code == 0)
            {
                if (Session["OrganizationID"] != null)
                {
                    _cm.OrganizationID = Convert.ToInt32(Session["OrganizationID"].ToString());
                    exists = _cm.checkEmployeeExists(txtName.Text);
                    if (exists == 0)
                    {
                        _cm.EmployeeName = txtName.Text;
                        _cm.SupervisorID = Convert.ToInt32(ddlSupervisor.SelectedValue);
                        int employeeID1 = _cm.AddOrgEmployee(_cm);
                        _cm.EmployeeID = employeeID1;
                        _cm.phone = txtPhone.Text;
                        _cm.mobile = txtMobile.Text;
                        _cm.email = txtEmail.Text;
                        _cm.designationID = Convert.ToInt32(ddlDesignation.SelectedValue);
                        _cm.employeeDepartmentID = Convert.ToInt32(ddlDepartment.SelectedValue);
                        int employeeID2 = _cm.AddEmployee(_cm);
                        BindEmployees();
                        divAddEmployee.Visible = false;
                        Clear();

                    }
                    else
                    {
                        divError.Visible = true;
                        lblMsg.Text = "Employee Name Exists";
                    }
                }
            }
            else
            {
                switch (code)
                {
                    case 1: divError.Visible = true; lblMsg.Text = "Invalid Email"; break;
                    case 2: divError.Visible = true; lblMsg.Text = "Invalid Mobile Number"; break;
                    case 3: divError.Visible = true; lblMsg.Text = "Invalid Phone Number"; break;
                    default: divError.Visible = false; break;
                }
            }
        }
        private void Clear()
        {
            txtName.Text = "";
            txtPhone.Text = "";
            txtMobile.Text = "";
            txtEmail.Text = "";
            ddlDepartment.SelectedIndex = -1;
            ddlDesignation.SelectedIndex = -1;
        }
        private void BindEmployees()
        {
            DataTable dt = _cm.GetEmployees();
            gvEmployees.DataSource = dt;
            gvEmployees.DataBind();

            ddlSupervisor.DataSource = dt;
            ddlSupervisor.DataTextField = "EmployeeName";
            ddlSupervisor.DataValueField = "ID";
            ddlSupervisor.DataBind();
            ddlSupervisor.Items.Insert(0, new ListItem("Select Supervisor", ""));

        }

        protected void btnAddEmployee_Click(object sender, EventArgs e)
        {
            divAddEmployee.Visible = true;
        }

        protected void gvEmployees_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvEmployees.EditIndex = e.NewEditIndex;

            DropDownList ddlDesignation = (DropDownList)(gvEmployees.Rows[gvEmployees.EditIndex].FindControl("ddlDesignation"));
            DropDownList ddlEmpDepartment = (DropDownList)(gvEmployees.Rows[gvEmployees.EditIndex].FindControl("ddlEmpDepartment"));
            TextBox txtPhone = (TextBox)(gvEmployees.Rows[gvEmployees.EditIndex].FindControl("txtPhone"));
            TextBox txtMobile = (TextBox)(gvEmployees.Rows[gvEmployees.EditIndex].FindControl("txtMobile"));
            TextBox txtEmail = (TextBox)(gvEmployees.Rows[gvEmployees.EditIndex].FindControl("txtEmail"));
            gvEmployees.EditIndex = e.NewEditIndex;
            BindEmployees();
            DataTable dt1 = _cm.GetAllDepartments();
            DataTable dt2 = _cm.GetAllDesignations();
                if (dt1.Rows.Count > 0)
                {
                ddlEmpDepartment.DataSource = dt1;
                ddlEmpDepartment.DataTextField = "EmployeeDepartmentName";
                ddlEmpDepartment.DataValueField = "EmployeeDepartmentID";
                ddlEmpDepartment.DataBind();
                ddlEmpDepartment.Items.Insert(0, new ListItem("Select Department", ""));
                }
                if (dt2.Rows.Count > 0)
                {
                    ddlDesignation.DataSource = dt2;
                    ddlDesignation.DataTextField = "DesignationName";
                    ddlDesignation.DataValueField = "DesignationID";
                    ddlDesignation.DataBind();
                    ddlDesignation.Items.Insert(0, new ListItem("Select Designation", ""));
                }
        }

        protected void gvEmployees_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int code = 0;
            DropDownList ddlDesignation = gvEmployees.Rows[e.RowIndex].FindControl("ddlDesignation") as DropDownList;
            DropDownList ddlDepartment = gvEmployees.Rows[e.RowIndex].FindControl("ddlDepartment") as DropDownList;
            TextBox txtPhone = gvEmployees.Rows[e.RowIndex].FindControl("txtPhone") as TextBox;
            TextBox txtMobile = gvEmployees.Rows[e.RowIndex].FindControl("txtMobile") as TextBox;
            TextBox txtEmail = gvEmployees.Rows[e.RowIndex].FindControl("txtEmail") as TextBox;
            DataTable dt1 = _cm.GetAllDepartments();
            DataTable dt2 = _cm.GetAllDesignations();
            if (dt1.Rows.Count > 0)
            {
                ddlDepartment.DataSource = dt1;
                ddlDepartment.DataTextField = "DepartmentName";
                ddlDepartment.DataValueField = "DepartmentID";
                ddlDepartment.DataBind();
                ddlDepartment.Items.Insert(0, new ListItem("Select Department", ""));
            }
            if (dt2.Rows.Count > 0)
            {
                ddlDesignation.DataSource = dt2;
                ddlDesignation.DataTextField = "DesignationName";
                ddlDesignation.DataValueField = "DesignationID";
                ddlDesignation.DataBind();
                ddlDesignation.Items.Insert(0, new ListItem("Select Designation", ""));
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
            if (Session["OrganizationID"] != null)
            {
                _cm.OrganizationID = Convert.ToInt32(Session["OrganizationID"].ToString());
                if (flag == 1 && code == 0)
                {
                    _cm.phone = txtPhone.Text;
                    _cm.mobile = txtMobile.Text;
                    _cm.email = txtEmail.Text;
                    _cm.designationID = Convert.ToInt32(ddlDesignation.SelectedValue);
                    _cm.employeeDepartmentID = Convert.ToInt32(ddlDepartment.SelectedValue);
                    int employeeID = _cm.AddEmployee(_cm);
                    txtPhone.Text = "";
                    txtMobile.Text = "";
                    txtEmail.Text = "";
                    ddlDepartment.SelectedIndex = -1;
                    ddlDesignation.SelectedIndex = -1;
                }
                else
                {
                    switch (code)
                    {
                        case 1: divError.Visible = true; lblMsg.Text = "Invalid Email"; break;
                        case 2: divError.Visible = true; lblMsg.Text = "Invalid Mobile Number"; break;
                        case 3: divError.Visible = true; lblMsg.Text = "Invalid Phone Number"; break;
                        default: divError.Visible = false; break;
                    }
                }
                //Setting the EditIndex property to -1 to cancel the Edit mode in Gridview  
                gvEmployees.EditIndex = -1;
                //Call ShowData method for displaying updated data  
                BindEmployees();
            }
        }

        protected void gvEmployees_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            //Setting the EditIndex property to -1 to cancel the Edit mode in Gridview  
            gvEmployees.EditIndex = -1;
            BindEmployees();
        }

      
    }
}