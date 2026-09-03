using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocumentFormat.OpenXml.Spreadsheet;

namespace LIMS
{
	public partial class Activities : System.Web.UI.Page
	{
        private const int PageSize = 10;
        DAL.GLModule _gl = new DAL.GLModule();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataTable dt = _gl.BindActivities(0, PageSize);
                if (dt.Rows.Count > 0)
                {
                    gvActivities.DataSource = dt;
                    gvActivities.PageIndex = 0;
                    gvActivities.VirtualItemCount = dt.Rows.Count;
                    gvActivities.DataBind();
                }
            }
               
        }

        protected void gvActivities_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
             DataTable dt = _gl.BindActivities(e.NewPageIndex, PageSize);
            if(dt.Rows.Count > 0)
            {
                gvActivities.DataSource = dt;
                gvActivities.PageIndex = e.NewPageIndex;
                gvActivities.VirtualItemCount = dt.Rows.Count;
                gvActivities.DataBind();
            }
        }

    }
}