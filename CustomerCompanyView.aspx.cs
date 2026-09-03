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
    public partial class CustomerCompanyView : System.Web.UI.Page
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
                if (Session["RoleID"] != null)
                {
                    int RoleID = Convert.ToInt32(Session["RoleID"].ToString());
                   
                }
                if (!Page.IsPostBack)
                {
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
            DataTable dt = _cm.getCustomerList();
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
            DataTable dt = _cm.getCustomerList(search,"Company");
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
            dt = _cm.getCompanyCustomerDetails();
            dt.DefaultView.Sort = e.SortExpression;
            gvCustomers.DataSource = dt;
            gvCustomers.DataBind();    
        }

        protected void gvCustomers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteCustomer")
            {
                int customerMasterID = Convert.ToInt32(e.CommandArgument);
                _cm.DeleteCustomer(customerMasterID);
                BindCustomers();
            }
        }
    }
}