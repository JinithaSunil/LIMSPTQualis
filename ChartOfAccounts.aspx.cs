using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LIMS.DAL;
using System.Security.Principal;

namespace LIMS
{
    public partial class ChartOfAccounts : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DataTable balanceSheetAccounts = GLModule.GetAccountCategory("BalanceSheet");
            DataTable plAccounts = GLModule.GetAccountCategory("ProfitAndLoss");
           
            rptMain.DataSource = balanceSheetAccounts;
            rptMain.DataBind();

            rptPL.DataSource = plAccounts;
            rptPL.DataBind();
        }

        protected void rptMain_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            RepeaterItem item = e.Item;
            if ((item.ItemType == ListItemType.Item) || (item.ItemType == ListItemType.AlternatingItem))
            {
                HiddenField hdBS = (HiddenField)item.FindControl("hdBS");
                Repeater rptChild = (Repeater)item.FindControl("rptChild");
                string AccountCategoryID = hdBS.Value;
                DataTable dt = GLModule.GetChartOfAccounts(AccountCategoryID);
                rptChild.DataSource = dt;
                rptChild.DataBind();
            }
        }

        protected void rptPL_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            RepeaterItem item = e.Item;
            if ((item.ItemType == ListItemType.Item) || (item.ItemType == ListItemType.AlternatingItem))
            {
                HiddenField hdPL = (HiddenField)item.FindControl("hdPL");
                Repeater rptChild = (Repeater)item.FindControl("rptChild");
                string AccountCategoryID = hdPL.Value;
                DataTable dt = GLModule.GetChartOfAccounts(AccountCategoryID);
                rptChild.DataSource = dt;
                rptChild.DataBind();
            }

        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Response.Redirect("ChartofAccountsDetails.aspx");
        }
    }
}