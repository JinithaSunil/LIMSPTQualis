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
    public partial class AuthorizedSignatory : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int flag = 1;
        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlControl liUserManagement1 = (HtmlGenericControl)this.Master.FindControl("liUserManagement1");
            HtmlControl liAuthorizedSignatory2 = (HtmlGenericControl)this.Master.FindControl("liAuthorizedSignatory2");            

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
                    liAuthorizedSignatory2.Attributes.Add("class", "active");
                    BindEmployees();
                    BindSignatories();
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
        private void BindSignatories()
        {
            DataTable dt = _cm.GetSignatories();
            if (dt.Rows.Count > 0)
            {
                gvSignatories.DataSource = dt;
                gvSignatories.DataBind();
            }

        }
        private void BindEmployees()
        {
            DataTable dt = _cm.GetEmployeeDetailsForSignatory();
            if (dt.Rows.Count > 0)
            {
                ddlEmployees.DataSource = dt;
                ddlEmployees.DataTextField = "EmployeeName";
                ddlEmployees.DataValueField = "ID";
                ddlEmployees.DataBind();
                ddlEmployees.Items.Insert(0, new ListItem("Select Employee", ""));
            }

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int exists = 0;
                    
            if (ddlEmployees.SelectedIndex == 0)
            {
                flag = 0;
                ddlEmployees.BackColor = System.Drawing.Color.LightPink;
            }
            else
            {
                ddlEmployees.BackColor = System.Drawing.Color.Empty;
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
                DataTable employeeDetails = new DataTable();
                exists = _cm.checkSignatoryExists(ddlEmployees.SelectedValue);
                if(exists == 0)
                {
                    if (Session["OrganizationID"] != null)
                    {
                        _cm.OrganizationID = Convert.ToInt32(Session["OrganizationID"].ToString());
                        employeeDetails = _cm.GetEmployeeDetails(ddlEmployees.SelectedValue);
                        if (employeeDetails.Rows.Count > 0)
                        {
                            _cm.AuthorizedSignatoryName = employeeDetails.Rows[0]["EmployeeName"].ToString();
                            _cm.designation = employeeDetails.Rows[0]["DesignationName"].ToString();
                            _cm.EmployeeID = Convert.ToInt32(ddlEmployees.SelectedValue);
                            _cm.AuthorizedSignatoryStatus = Convert.ToInt32(ddlStatus.SelectedValue);
                            int sigantoryID = _cm.AddSignatory(_cm);
                            divAddSingatory.Visible = false;
                            BindSignatories();
                            Clear();
                        }
                    }                    
                }
                else
                {
                    divError.Visible = true;
                    lblMsg.Text = "Signatory Exists";
                }
            }
        }
        private void Clear()
        {
           ddlEmployees.SelectedIndex = -1;
        }

        protected void btnAddSignatory_Click(object sender, EventArgs e)
        {
            divAddSingatory.Visible = true;
        }
    }
}