using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
	public partial class CustomerLedger : System.Web.UI.Page
	{
        DAL.CM _cm = new DAL.CM();
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
                ddlCustomer.DataValueField = "CustomerID";
                ddlCustomer.DataBind();
                ddlCustomer.Items.Insert(0, new ListItem("--Select Customer--", "0"));
            }
            else
            {

            }            
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetCustomerLedger", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@CustomerID", ddlCustomer.SelectedValue);
                cmd.Parameters.AddWithValue("@FromDate", txtFromDate.Text);
                cmd.Parameters.AddWithValue("@ToDate", txtToDate.Text);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvLedger.DataSource = dt;
                gvLedger.DataBind();
            }
        }
    }
}