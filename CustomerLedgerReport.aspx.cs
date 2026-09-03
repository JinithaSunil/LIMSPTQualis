using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LIMS.DAL;

namespace LIMS
{
    public partial class CustomerLedgerReport : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        decimal totalDebit = 0;
        decimal totalCredit = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCustomers();
            }
        }
        public void LoadCustomers()
        {
            DataTable dt = _cm.getAllCustomers();

            if (dt.Rows.Count > 0)
            {
                ddlCustomer.DataSource = dt;
                ddlCustomer.DataTextField = "CustomerName";
                ddlCustomer.DataValueField = "CustomerMasterID";
                ddlCustomer.DataBind();
                ddlCustomer.Items.Insert(0, new ListItem("--Select Customer--", "0"));
            }
            else
            {

            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            int flag = 0;
            if(ddlCustomer.SelectedIndex == 0)
            {
                ddlCustomer.BackColor = Color.LightPink;
                flag = 1;
            }
            else
            {
                ddlCustomer.BackColor = Color.Empty;               
            }
            if (txtFromDate.Text == "")
            {
                txtFromDate.BorderColor = Color.Red;
                flag = 1;
            }
            else
            {
                txtFromDate.BorderColor = Color.Empty;
            }
            if (txtToDate.Text == "")
            {
                txtToDate.BorderColor = Color.Red;
                flag = 1;
            }
            else
            {
                txtToDate.BorderColor = Color.Empty;
            }
            if (flag == 0)
            {
                lblCustomer.Text = ddlCustomer.SelectedItem.ToString();
                lblFrom.Text = txtFromDate.Text;
                lblTo.Text = txtToDate.Text;

                string cs = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;

                using (SqlConnection con = new SqlConnection(cs))
                {
                    SqlCommand cmd = new SqlCommand("sp_CustomerLedgerReport", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@CustomerID", SqlDbType.Int).Value = ddlCustomer.SelectedValue;
                    cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = txtFromDate.Text;
                    cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = txtToDate.Text;

                    SqlDataAdapter da = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvLedger.DataSource = dt;
                    gvLedger.DataBind();
                }
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "notify", "alert(Fill Highlighted Fields.)", true);
            }
           
        }
        protected void gvLedger_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                totalDebit += Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "Debit"));
                totalCredit += Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "Credit"));
            }

            if (e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[3].Text = "Transaction Total";

                e.Row.Cells[4].Text = totalDebit.ToString("N2");
                e.Row.Cells[5].Text = totalCredit.ToString("N2");

                e.Row.Font.Bold = true;
            }
        }
    }
}