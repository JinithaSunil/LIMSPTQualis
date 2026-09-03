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
    public partial class ManageGroupSubGroup : System.Web.UI.Page
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
                    BindDepartments();
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
        protected void BindDepartments()
        {
            DataTable dt = _cm.GetAllDepartments("LIMS",1);
            gvDepartments.DataSource = dt;
            gvDepartments.DataBind();
            ddlDepartments.DataSource = dt;
            ddlDepartments.DataTextField = "DepartmentName";
            ddlDepartments.DataValueField = "DepartmentID";
            ddlDepartments.DataBind();
            ddlDepartments.Items.Insert(0, new ListItem("Select Department", ""));
            ddlDepartments.SelectedIndex = 0;

            ddlDepartments2.DataSource = dt;
            ddlDepartments2.DataTextField = "DepartmentName";
            ddlDepartments2.DataValueField = "DepartmentID";
            ddlDepartments2.DataBind();
            ddlDepartments2.Items.Insert(0, new ListItem("Select Department", ""));
            ddlDepartments2.SelectedIndex = 0;
        }

        protected void ddlDepartments_SelectedIndexChanged(object sender, EventArgs e)
        {
            int departmentID = Convert.ToInt32(ddlDepartments.SelectedValue);
            BindProductGroups(departmentID);
        }
        protected void BindProductGroups(int departmentID)
        {
            DataTable dtProdGroup = _cm.getProductGroup(departmentID);
            gvProductGroup.DataSource = dtProdGroup;
            gvProductGroup.DataBind();
            ddlProductGroup.DataSource = dtProdGroup;
            ddlProductGroup.DataTextField = "ProductGroupName";
            ddlProductGroup.DataValueField = "ProductGroupID";
            ddlProductGroup.DataBind();
            ddlProductGroup.Items.Insert(0, new ListItem("Select Product Group", ""));
            ddlProductGroup.SelectedIndex = 0;
        }
        protected void ddlProductGroup_SelectedIndexChanged(object sender, EventArgs e)
        {            
            int ProductGroupID = Convert.ToInt32(ddlProductGroup.SelectedValue);
            BindProductSubGroups(ProductGroupID);
        }
        protected void BindProductSubGroups(int ProductGroupID)
        {
            DataTable dtProductSubGroup = _cm.getProductSubGroup(ProductGroupID);
            gvProductSubGroup.DataSource = dtProductSubGroup;
            gvProductSubGroup.DataBind();
        }
        protected void ddlDepartments2_SelectedIndexChanged(object sender, EventArgs e)
        {
            int departmentID = Convert.ToInt32(ddlDepartments2.SelectedValue);
            BindProductGroups(departmentID);
        }

        protected void btnAddProductGroup_Click(object sender, EventArgs e)
        {
            int departmentID = 0;int status = 0;int flag = 1;
            if(ddlDepartments.SelectedIndex == 0)
            {
                flag = 0;
                ddlDepartments.BackColor = System.Drawing.Color.LightPink;
            }
            else
            {
                ddlDepartments.BackColor = System.Drawing.Color.Empty;
                departmentID = Convert.ToInt32(ddlDepartments.SelectedValue);
            }
            if (txtProductGroup.Text == "")
            {
                flag = 0;
                txtProductGroup.BackColor = System.Drawing.Color.LightPink;
            }
            else
            {
                txtProductGroup.BackColor = System.Drawing.Color.Empty;
            }
            if(flag == 1)
            {
                if (Session["OrganizationID"] != null)
                {
                    _cm.OrganizationID = Convert.ToInt32(Session["OrganizationID"].ToString());

                    _cm.GroupName = txtProductGroup.Text;
                    _cm.departmentID = Convert.ToInt32(ddlDepartments.SelectedValue);
                    status = _cm.AddProductGroup(_cm);
                    if (status == 0)
                    {
                        txtProductGroup.BackColor = System.Drawing.Color.LightPink;
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "notify", "alert(Group Not added. Check for duplicates.)", true);
                    }
                    else
                    {
                        BindProductGroups(departmentID);
                    }
                }
            }
        }

        protected void btnShowProductGroup_Click(object sender, EventArgs e)
        {
            divProductGroup.Visible = true;
        }

        protected void btnAddProductSubGroup_Click(object sender, EventArgs e)
        {
            int departmentID = 0; int productGroupID = 0; int status = 0; int flag = 1;
            if (ddlDepartments2.SelectedIndex == 0)
            {
                flag = 0;
                ddlDepartments2.BackColor = System.Drawing.Color.LightPink;
            }
            else
            {
                ddlDepartments2.BackColor = System.Drawing.Color.Empty;
                departmentID = Convert.ToInt32(ddlDepartments2.SelectedValue);
            }
            if (ddlProductGroup.SelectedIndex == 0)
            {
                flag = 0;
                ddlProductGroup.BackColor = System.Drawing.Color.LightPink;
            }
            else
            {
                if(ddlProductGroup.Items.Count == 0)
                {
                    ddlProductGroup.BackColor = System.Drawing.Color.LightPink;
                }
                else
                {
                    ddlProductGroup.BackColor = System.Drawing.Color.Empty;
                    productGroupID = Convert.ToInt32(ddlProductGroup.SelectedValue);
                }
                
            }
            if (txtProductSubGroup.Text == "")
            {
                flag = 0;
                txtProductSubGroup.ForeColor = System.Drawing.Color.LightPink;
            }
            else
            {
                txtProductSubGroup.ForeColor = System.Drawing.Color.Empty;
            }
            if (flag == 1)
            {
                if (Session["OrganizationID"] != null)
                {
                    _cm.OrganizationID = Convert.ToInt32(Session["OrganizationID"].ToString());
                    _cm.subGroup = txtProductSubGroup.Text;
                    _cm.groupId = Convert.ToInt32(ddlProductGroup.SelectedValue);
                    _cm.departmentID = Convert.ToInt32(ddlDepartments2.SelectedValue);
                    _cm.sampleCategoryID = 1;
                    status = _cm.AddProductSubGroup(_cm);
                    if (status == 0)
                    {
                        txtProductSubGroup.BackColor = System.Drawing.Color.LightPink;
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "notify", "alert(Sub Group Not added. Check for duplicates.)", true);
                    }
                    else
                    {
                        BindProductSubGroups(departmentID);
                    }
                }
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "notify", "alert(Fill Highlighted values.)", true);
            }
        }

        protected void btnShowProductSubGroup_Click(object sender, EventArgs e)
        {
            divProductSubGroup.Visible = true;
        }
    }
}