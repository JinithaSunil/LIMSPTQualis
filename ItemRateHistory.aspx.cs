using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace LIMS
{
    public partial class ItemRateHistory : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["ID"] != null)
                {
                    BindAllPO(Request.QueryString["ID"]);
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }
        private void BindAllPO(string ID)
        {
            if (Request.QueryString["ID"] != null)
            {
                _cm.ItemID = Convert.ToInt32(ID);
                DataTable dt = _cm.GetItemRateComparison(_cm);
                if (dt.Rows.Count > 0)
                {
                    gvPO.DataSource = dt;
                    gvPO.DataBind();
                    ViewState["dt"] = dt;
                    ViewState["sort"] = "Asc";
                }
                else
                {
                    gvPO.DataSource = null;
                    gvPO.DataBind();
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }

        protected void gvPO_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable dt = (DataTable)ViewState["dt"];
            if (dt.Rows.Count > 0)
            {
                if (Convert.ToString(ViewState["sort"]) == "Asc")
                {
                    dt.DefaultView.Sort = e.SortExpression + " Desc";
                    ViewState["sort"] = "Desc";
                }
                else
                {
                    dt.DefaultView.Sort = e.SortExpression + " Asc";
                    ViewState["sort"] = "Asc";
                }
                gvPO.DataSource = dt;
                gvPO.DataBind();
            }
        }
    }
}