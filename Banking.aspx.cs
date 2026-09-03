using LIMS.DAL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class Banking : System.Web.UI.Page
    {
        DAL.BankingModule.BankingVariables _bk = new DAL.BankingModule.BankingVariables();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) LoadBankAccounts();
        }

        private void LoadBankAccounts()
        {
            var accounts = BankingModule.GetBankAccounts();
            gvBankAccounts.DataSource = accounts;
            gvBankAccounts.DataBind();

            ddlBankAccounts.DataSource = accounts;
            ddlBankAccounts.DataTextField = "AccountNumber";
            ddlBankAccounts.DataValueField = "AccountId";
            ddlBankAccounts.DataBind();
        }

        protected void btnAddBank_Click(object sender, EventArgs e)
        {
            int flag = 0;
            if (txtBankName.Text == "")
            {
                txtBankName.BorderColor = Color.Red;
                flag = 1;
            }
            else
            {
                txtBankName.BorderColor = Color.Empty;
            }
            if (txtAccountNumber.Text == "")
            {
                txtAccountNumber.BorderColor = Color.Red;
                flag = 1;
            }
            else
            {
                txtAccountNumber.BorderColor = Color.Empty;
            }
            if (txtBalance.Text == "")
            {
                txtBalance.BorderColor = Color.Red;
                flag = 1;
            }
            else
            {
                txtBalance.BorderColor = Color.Empty;
            }
            if (flag == 0)
            {
                _bk.AccountNumber = txtAccountNumber.Text;
                _bk.BankName = txtBankName.Text;
                _bk.Balance = decimal.Parse(txtBalance.Text);
                BankingModule.AddBankAccount(_bk);
                LoadBankAccounts();
            }
            
        }

        protected void btnUpdateBalance_Click(object sender, EventArgs e)
        {
            BankingModule.UpdateBalance(
                int.Parse(ddlBankAccounts.SelectedValue),
                decimal.Parse(txtDepositAmount.Text)
            );
            LoadBankAccounts();
        }

        protected void gvBankAccounts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string BankAccountID = "";
            if (e.CommandName == "BankStatement")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                BankAccountID = gvBankAccounts.DataKeys[rowIndex].Value.ToString();
                Session["BankAccountID"] = BankAccountID;
                Response.Redirect("BankStatement.aspx");
            }
        }
    }
}