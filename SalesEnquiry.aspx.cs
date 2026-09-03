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
    public partial class SalesEnquiry : System.Web.UI.Page
    {
        LIMS.DAL.CM _cm = new LIMS.DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {      
               
                txtDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                Populate();
            }
        }
        private void Populate()
        {
            DataTable dt = _cm.getUOM();
            
            dt = _cm.getCustomers();
            if (dt.Rows.Count > 0)
            {
                ddlCustomer.DataSource = dt;
                ddlCustomer.DataTextField = "CompanyName";
                ddlCustomer.DataValueField = "CustomerMasterId";
                ddlCustomer.DataBind();
                ddlCustomer.Items.Insert(0, "--Select Customer--");
            }
            
        }

        [System.Web.Script.Services.ScriptMethod()]
        [System.Web.Services.WebMethod]
        public static List<string> GetProducts(string prefixText)
        {
            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;

                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandText = @"SELECT ps.SubGroupName, ps.ProductGroupID, pg.ProductGroupName 
                                       FROM [LIMSPTQualis].[dbo].[ProductSubGroup] ps
                                       INNER JOIN [LIMSPTQualis].[dbo].[ProductGroup] pg 
                                       ON ps.ProductGroupID = pg.ProductGroupID
                                       WHERE ps.SubGroupName LIKE @Prefix + '%'
                                       ORDER BY ps.SubGroupName";
                    com.Parameters.AddWithValue("@Prefix", prefixText);
                    com.Connection = con;
                    con.Open();
                    List<string> products = new List<string>();
                    using (SqlDataReader dr = com.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            products.Add(dr["SubGroupName"].ToString());

                        }
                    }
                    con.Close();
                    return products;
                }

            }

        }


        // ProductInfo class
        public class ProductInfo
        {
            public string SubGroupName { get; set; }
            public string ProductGroupID { get; set; }
            public string GroupName { get; set; }
        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCustomer.SelectedValue != "")
            {
                string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    //cmd.CommandText = @"SELECT AddressLine1, AddressLine2,AddressLine3, City, State, Country, Pincode 
                    //                    FROM LocationDetails 
                    //                    WHERE CustomerMasterId = @CustomerMasterId";

                    cmd.CommandText = @"SELECT * FROM LocationDetails ld
                                        INNER JOIN ContactPersonDetails cpd ON cpd.CustomerMasterId = ld.CustomerMasterId
                                        INNER JOIN ContactDetails cd ON cd.PersonId = cpd.PersonId
                                        WHERE cpd.CustomerMasterId = @CustomerMasterId
                                            AND (cpd.isPrimary = 1  OR ((cpd.isPrimary = 0 OR cpd.isPrimary IS NULL)
                                               AND NOT EXISTS (SELECT 1 FROM ContactPersonDetails cpd2
                                               WHERE cpd2.CustomerMasterId = cpd.CustomerMasterId
                                               AND cpd2.isPrimary = 1
		                                        )))";

                    cmd.Parameters.AddWithValue("@CustomerMasterId", ddlCustomer.SelectedItem.Value);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Concatenate all address parts
                            string address = reader["AddressLine1"].ToString();
                            address += !string.IsNullOrEmpty(reader["AddressLine2"].ToString()) ? ", " + reader["AddressLine2"].ToString() : "";
                            address += !string.IsNullOrEmpty(reader["AddressLine3"].ToString()) ? ", " + reader["AddressLine3"].ToString() : "";
                            address += !string.IsNullOrEmpty(reader["City"].ToString()) ? ", " + reader["City"].ToString() : "";
                            address += !string.IsNullOrEmpty(reader["State"].ToString()) ? ", " + reader["State"].ToString() : "";
                            address += !string.IsNullOrEmpty(reader["Country"].ToString()) ? ", " + reader["Country"].ToString() : "";
                            address += !string.IsNullOrEmpty(reader["Pincode"].ToString()) ? "-" + reader["Pincode"].ToString() : "";

                            txtBillingAddress.Text = address;
                            txtShippingAddress.Text = address;

                            txtCustomerName.Text = reader["FirstName"].ToString() +" "+ reader["LastName"].ToString();
                            txtMobileNumber.Text = reader["Mobile"].ToString();
                            txtEmail.Text = reader["Email"].ToString();
                        }
                    }
                    conn.Close();
                }
            }
            else
            {
                // Clear billing address if no company is selected
                txtBillingAddress.Text = string.Empty;
                txtShippingAddress.Text = string.Empty;
            }
        }


        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            
        }

       

    }
}