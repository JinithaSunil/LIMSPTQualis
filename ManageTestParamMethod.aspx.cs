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
    public partial class ManageTestParamMethod : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
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
                HtmlControl liGroupManagement = (HtmlGenericControl)this.Master.FindControl("liGroupManagement");
                HtmlControl liTestParam = (HtmlGenericControl)this.Master.FindControl("liTestParam");                

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
                    BindTestMethods();
                    BindTestParameters();
                }
                if (Session["RoleID"] != null)
                {
                    int RoleID = Convert.ToInt32(Session["RoleID"].ToString());
                    
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
            
        }
        protected void BindTestMethods()
        {
            DataTable dtTestMethod = _cm.getTestMethod();
            gvTestMethod.DataSource = dtTestMethod;
            gvTestMethod.DataBind();
        }
        protected void BindTestParameters()
        {
            DataTable dtTestParameter = _cm.getTestParameter();
            gvTestParameter.DataSource = dtTestParameter;
            gvTestParameter.DataBind();
        }

        protected void btnAddTestMethod_Click(object sender, EventArgs e)
        {
            int status = 0;int flag = 0;
            if (txtTestMethod.Text == "")
            {
                flag = 1;
                txtTestMethod.BackColor = System.Drawing.Color.LightPink;
            }
            else
            {
                txtTestMethod.BackColor = System.Drawing.Color.Empty;
            }
            if(flag == 0)
            {
                if (Session["OrganizationID"] != null)
                {
                    _cm.OrganizationID = Convert.ToInt32(Session["OrganizationID"].ToString());
                    _cm.testMethod = txtTestMethod.Text;
                    status = _cm.AddTestMethod(_cm);
                    if (status == 0)
                    {
                        txtTestMethod.BackColor = System.Drawing.Color.LightPink;
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "notify", "alert(Test Method Not added. Check for duplicates.)", true);
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "notify", "alert(Test Method added.)", true);
                        BindTestMethods();
                    }
                }
            }
        }

        protected void btnShowTestMethod_Click(object sender, EventArgs e)
        {
            divTestMethod.Visible = true;
        }

        protected void btnAddTestParameter_Click(object sender, EventArgs e)
        {
            int status = 0; int flag = 0;
            if (txtTestParameter.Text == "")
            {
                flag = 1;
                txtTestParameter.ForeColor = System.Drawing.Color.LightPink;
            }
            else
            {
                txtTestParameter.ForeColor = System.Drawing.Color.Empty;
            }
            if (flag == 0)
            {
                if (Session["OrganizationID"] != null)
                {
                    _cm.OrganizationID = Convert.ToInt32(Session["OrganizationID"].ToString());
                    _cm.testParameter = txtTestParameter.Text;
                    status = _cm.AddTestParameter(_cm);
                    if (status == 0)
                    {
                        txtTestParameter.BackColor = System.Drawing.Color.LightPink;
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "notify", "alert(Test Parameter Not added. Check for duplicates.)", true);
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "notify", "alert(Test Parameter added.)", true);
                        BindTestParameters();
                    }
                }
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "notify", "alert(Fill Highlighted values.)", true);
            }
        }

        protected void btnShowTestParameter_Click(object sender, EventArgs e)
        {
            divTestParameter.Visible = true;
        }
    }
}