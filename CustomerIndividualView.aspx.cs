using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class CustomerIndividualView : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
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
            HtmlControl liCustomers = (HtmlGenericControl)this.Master.FindControl("liCustomers1");
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
                if (!Page.IsPostBack)
                {
                    liCustomers.Attributes.Add("class", "active");
                    BindCustomers();
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
                      
        }
        private void BindCustomers()
        {
            DataTable dt = _cm.getIndividualCustomerDetails();
            if (dt.Rows.Count > 0)
            {
                gvCustomers.DataSource = dt;
                gvCustomers.DataBind();
                lblNoRecords.Visible = false;
            }
            else
            {
                lblNoRecords.Visible = true;
            }
        }
        private void BindCustomers(string search)
        {
            DataTable dt = _cm.getCustomerDetails(search,"Individual");
            if (dt.Rows.Count > 0)
            {
                gvCustomers.DataSource = dt;
                gvCustomers.DataBind();
                lblNoRecords.Visible = false;
            }
            else
            {
                lblNoRecords.Visible = true;
            }
        }
        

        protected void imgSearch_Click(object sender, ImageClickEventArgs e)
        {
            string search = txtSearch.Text;
            if (search == "")
            {
                txtSearch.BorderColor = System.Drawing.Color.Red;
            }
            else
            {
                txtSearch.BorderColor = System.Drawing.Color.Empty;
                BindCustomers(search);
            }
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string search = txtSearch.Text;
            BindCustomers(search);
        }

        protected void gvCustomers_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCustomers.PageIndex = e.NewPageIndex;
            this.BindCustomers();
        }

        protected void gvCustomers_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                HyperLink hl = (HyperLink)e.Row.FindControl("hypCustomer");
                if (hl != null)
                {
                    //DataRowView drv = (DataRowView)e.Row.DataItem;
                    //string customerID = drv["CustomerMasterID"].ToString();
                    //hl.NavigateUrl = "~/CustomerProfile.aspx?ID=" + customerID;
                }
            }
        }

        protected void gvCustomers_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable dt = new DataTable();
            dt = _cm.getIndividualCustomerDetails();
            dt.DefaultView.Sort = e.SortExpression;
            gvCustomers.DataSource = dt;
            gvCustomers.DataBind();    
        }
    }
}