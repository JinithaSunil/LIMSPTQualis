using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class CustomerLabel : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["Inv"] != null)
                {
                    string Inv = Session["Inv"].ToString();
                    if(Inv.Substring(Inv.Length - 1, 1)[0] == ',')
                    {
                        Inv = Inv.Remove(Inv.Length - 1);
                    }
                    var dt = _cm.getCustomerDetails("Company", Inv, 1);
                    rptPage.DataSource = dt;
                    rptPage.DataBind();
                }
            }
        }
    }
}