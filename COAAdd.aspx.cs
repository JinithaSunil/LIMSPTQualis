using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
	public partial class ManageCOA : System.Web.UI.Page
	{
        DAL.GLModule _gl = new DAL.GLModule();
        string cs = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadParentAccounts();
            }
        }
        protected void LoadParentAccounts()
        {
            DataTable dt = _gl.GetChartOfAccounts();
            ddlParent.DataSource = dt;
            ddlParent.DataTextField = "AccountName";
            ddlParent.DataValueField = "AccountID";
            ddlParent.DataBind();
        }
        protected void rbAccountType_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlHeader.Visible = rbAccountType.SelectedValue == "H";
            pnlDetail.Visible = rbAccountType.SelectedValue == "D";
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd;

                if (rbAccountType.SelectedValue == "H")
                {
                    cmd = new SqlCommand(
                        @"INSERT INTO ChartOfAccountsHeader
                          (AccountName, Balance, AccountCode, AccountCategoryID,
                           AccountNameEng, HeaderLevel, DebitOrCredit, ParentID)
                          VALUES
                          (@Name, 0, @Code, 1, @Eng, @Level, @DrCr, @ParentID)", con);

                    cmd.Parameters.AddWithValue("@Level", ddlHeaderLevel.SelectedValue);
                }
                else
                {
                    cmd = new SqlCommand(
                        @"INSERT INTO ChartOfAccountsDetail
                          (AccountName, Balance, AccountCode, AccountCategoryID,
                           AccountNameEng, DebitOrCredit, ParentID, TaxCode)
                          VALUES
                          (@Name, 0, @Code, 1, @Eng, @DrCr, @ParentID, @Tax)", con);

                    cmd.Parameters.AddWithValue("@Tax", ddlTaxCode.SelectedValue);
                }

                cmd.Parameters.AddWithValue("@Name", txtAccountName.Text);
                cmd.Parameters.AddWithValue("@Code", txtAccountCode.Text);
                cmd.Parameters.AddWithValue("@Eng", txtAccountName.Text);
                cmd.Parameters.AddWithValue("@DrCr", ddlDrCr.SelectedValue);
                cmd.Parameters.AddWithValue("@ParentID", ddlParent.SelectedValue);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            lblMsg.Text = "Account saved successfully ✔";
            ClearForm();
            LoadParentAccounts();
        }
        void ClearForm()
        {
            txtAccountName.Text = "";
            txtAccountCode.Text = "";
            ddlParent.SelectedIndex = 0;
        }
        public void AddCOAAccount()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd;

                if (rbAccountType.SelectedValue == "H")
                {
                    cmd = new SqlCommand(
                        @"INSERT INTO ChartOfAccountsHeader
               (AccountName, Balance, AccountCode, AccountCategoryID,
                AccountNameEng, HeaderLevel, DebitOrCredit, ParentID)
               VALUES
               (@Name, 0, @Code, 1, @Eng, @Level, @DrCr, @ParentID)", con);

                    cmd.Parameters.AddWithValue("@Level", ddlHeaderLevel.SelectedValue);
                }
                else
                {
                    cmd = new SqlCommand(
                        @"INSERT INTO ChartOfAccountsDetail
               (AccountName, Balance, AccountCode, AccountCategoryID,
                AccountNameEng, DebitOrCredit, ParentID, TaxCode)
               VALUES
               (@Name, 0, @Code, 1, @Eng, @DrCr, @ParentID, @Tax)", con);

                    cmd.Parameters.AddWithValue("@Tax", ddlTaxCode.SelectedValue);
                }

                cmd.Parameters.AddWithValue("@Name", txtAccountName.Text);
                cmd.Parameters.AddWithValue("@Code", txtAccountCode.Text);
                cmd.Parameters.AddWithValue("@Eng", txtAccountName.Text);
                cmd.Parameters.AddWithValue("@DrCr", ddlDrCr.SelectedValue);
                cmd.Parameters.AddWithValue("@ParentID", ddlParent.SelectedValue);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}