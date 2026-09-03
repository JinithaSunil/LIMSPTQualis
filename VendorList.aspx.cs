using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LIMS.DAL;

namespace LIMS
{
    public partial class VendorList : System.Web.UI.Page
    {
        LIMS.DAL.CM _cm = new LIMS.DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Session["Usertype"] != null)
                {
                    BindVendors("Purchase");
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }

            }
        }

        protected void BindVendors(string Department)
        {
            DataTable dt = string.IsNullOrWhiteSpace(txtSearch.Text)
                ? _cm.GetAllVendors(Department)
                : _cm.GetVendorSearch(txtSearch.Text, Department);

            if (dt.Rows.Count > 0)
            {
                dt.DefaultView.Sort = "VendorName ASC"; // or use CreatedDate DESC if you have it
                gvVendor.PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
                gvVendor.DataSource = dt;
                gvVendor.DataBind();
                spanCount.InnerHtml = dt.Rows.Count.ToString();
            }
            else
            {
                gvVendor.DataSource = null;
                gvVendor.DataBind();
            }
        }
        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvVendor.PageIndex = 0;
            BindVendors("Purchase");
        }

        protected void BindVendors(string search, string department)
        {
            DataTable dt = _cm.GetVendorSearch(search, department);
            if (dt.Rows.Count > 0)
            {
                dt.DefaultView.Sort = "VendorID DESC"; // Apply sorting here too
                gvVendor.DataSource = dt;
                gvVendor.DataBind();
                spanCount.InnerHtml = dt.Rows.Count.ToString();
            }
            else
            {
                gvVendor.DataSource = null;
                gvVendor.DataBind();
            }
        }
        protected void gvVendor_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
        
            }
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string Vendor = txtSearch.Text;
            if (Session["DeptName"] != null)
            {
                string department = Session["DeptName"].ToString();
                BindVendors(Vendor, department);
            }
        }

        protected void gvVendor_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvVendor.EditIndex = e.NewEditIndex;
            if (Session["DeptName"] != null)
            {
                string department = Session["DeptName"].ToString();
                BindVendors(department);
            }
        }

        protected void gvVendor_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            if (Session["DeptName"] != null)
            {
                RadioButtonList radVendorStatus = (RadioButtonList)gvVendor.Rows[e.RowIndex].FindControl("radVendorStatus");
                Label lblVendorID = (Label)gvVendor.Rows[e.RowIndex].FindControl("lblVendorID");
               _cm.VendorID = Convert.ToInt32(lblVendorID.Text);
                _cm.VendorStatus = Convert.ToInt32(radVendorStatus.SelectedValue);
                _cm.UpdateVendorStatus(_cm);
                gvVendor.EditIndex = -1;
                BindVendors(Session["DeptName"].ToString());
            }
        }

        protected void gvVendor_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            if (Session["DeptName"] != null)
            {
                gvVendor.EditIndex = -1;
                BindVendors(Session["DeptName"].ToString());
            }
        }
        protected void gvVendor_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvVendor.PageIndex = e.NewPageIndex;

            if (Session["DeptName"] != null)
            {
                string department = Session["DeptName"].ToString();
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    BindVendors(txtSearch.Text, department);
                }
                else
                {
                    BindVendors(department);
                }
            }
        }




    }
}