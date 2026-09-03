using LIMS.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class ChartOfAccountsDetails : System.Web.UI.Page
    {
        DAL.GLModule _gl = new GLModule();
        decimal sumFooterValue = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindAccountCategory();
                LoadAccounts();
            }
        }

        private void BindAccountCategory()
        {
            ddlAccountCategory.DataSource = GLModule.GetAccountCategory();
            ddlAccountCategory.DataTextField = "AccountCategory";
            ddlAccountCategory.DataValueField = "AccountCategoryID";
            ddlAccountCategory.DataBind();
            ddlAccountCategory.Items.Insert(0, "Select Category");
        }
        private void LoadAccounts()
        {
            gvCOA.DataSource = _gl.GetChartOfAccounts();
            gvCOA.DataBind();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            _gl.AccountName = txtName.Text;
            _gl.AccountCode = txtAccountCode.Text;
            _gl.Amount = Convert.ToDecimal(txtBalance.Text);
            _gl.AccountCategoryID = Convert.ToInt32(ddlAccountCategory.SelectedValue);
            GLModule.AddAccount(_gl);
            LoadAccounts();
        }

        protected void gvCOA_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string Balance = ((Label)e.Row.FindControl("lblBalance")).Text;
                decimal TotalBalance = Convert.ToDecimal(Balance);
                sumFooterValue += TotalBalance;
            }
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                Label lbl = (Label)e.Row.FindControl("lblTotal");
                lbl.Text = sumFooterValue.ToString();
            }
        }

        protected void ddlAccountCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlAccountCategory.SelectedIndex != 0)
            {
                gvCOA.DataSource = GLModule.GetChartOfAccounts(ddlAccountCategory.SelectedValue);
                gvCOA.DataBind();
            }
            else
            {
                LoadAccounts();
            }
        }
    }
}