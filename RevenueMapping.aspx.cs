using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
	public partial class RevenueMapping : System.Web.UI.Page
	{
		DAL.CM _cm = new DAL.CM();
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				BindAccounts();
				BindDivisions();
			}
		}
		protected void BindDivisions()
		{
			DataTable dt = _cm.GetAllDepartments();
			if (dt.Rows.Count > 0)
			{
				ddlDivision.DataSource = dt;
				ddlDivision.DataTextField = "DepartmentName";
				ddlDivision.DataValueField = "DepartmentID";
				ddlDivision.DataBind();
				ddlDivision.Items.Insert(0, "Select Division");
			}
			else
			{
				ddlDivision.DataSource = null;
				ddlDivision.DataBind();
			}
		}

        protected void BindAccounts()
		{
			DataTable dt = new DataTable();
			if ((ddlDivision.SelectedIndex != -1) && (ddlDivision.SelectedIndex != 0))

            {
				dt= _cm.GetAccountMapping(ddlDivision.SelectedValue);
            }
			else
			{
                dt = _cm.GetAccountMapping();
            }
				
			if (dt.Rows.Count > 0)
			{
				gvRevenue.DataSource = dt;
				gvRevenue.DataBind();
			}
			else
			{
				gvRevenue.DataSource = null;
				gvRevenue.DataBind();
			}
           
        }

        protected void ddldivision_SelectedIndexChanged(object sender, EventArgs e)
        {
			if (ddlDivision.SelectedIndex != 0)
			{
				BindAccounts();
			}
        }
    }
}