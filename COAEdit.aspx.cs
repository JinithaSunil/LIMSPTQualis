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
	public partial class COAEdit : System.Web.UI.Page
    {
        DAL.GLModule _gl = new DAL.GLModule();
        string cs = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
		{
            if (!IsPostBack)
            {
                BindGrid();
                LoadParentAccounts();
            }
        }
        void BindGrid()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlDataAdapter da = new SqlDataAdapter(@"
            SELECT AccountID, AccountCode, AccountName, DebitOrCredit, 'H' AS AccountType
            FROM ChartOfAccountsHeader
            UNION ALL
            SELECT AccountID, AccountCode, AccountName, DebitOrCredit, 'D'
            FROM ChartOfAccountsDetail
            ORDER BY AccountCode", con);

                DataTable dt = new DataTable();
                da.Fill(dt);
                gvCOA.DataSource = dt;
                gvCOA.DataBind();
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
            BindGrid();
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
        protected void gvCOA_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvCOA.EditIndex = e.NewEditIndex;
            BindGrid();
        }
        protected void gvCOA_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvCOA.EditIndex = -1;
            BindGrid();
        }
        protected void gvCOA_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int id = Convert.ToInt32(gvCOA.DataKeys[e.RowIndex].Values["AccountID"]);
            string type = gvCOA.DataKeys[e.RowIndex].Values["AccountType"].ToString();

            GridViewRow row = gvCOA.Rows[e.RowIndex];
            TextBox txtName = (TextBox)row.FindControl("txtAccountName");
            DropDownList ddlDrCr = (DropDownList)row.FindControl("ddlDrCr");

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd;

                if (type == "H")
                {
                    cmd = new SqlCommand(
                        "UPDATE ChartOfAccountsHeader SET AccountName=@Name, DebitOrCredit=@DrCr WHERE AccountID=@ID", con);
                }
                else
                {
                    cmd = new SqlCommand(
                        "UPDATE ChartOfAccountsDetail SET AccountName=@Name, DebitOrCredit=@DrCr WHERE AccountID=@ID", con);
                }

                cmd.Parameters.AddWithValue("@ID", id);
                cmd.Parameters.AddWithValue("@Name", txtName.Text);
                cmd.Parameters.AddWithValue("@DrCr", ddlDrCr.SelectedValue);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            gvCOA.EditIndex = -1;
            BindGrid();
        }

    }
}