using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class TemporaryPageforTesting : System.Web.UI.Page
    {

        SqlConnection _sCon = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString);
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        string date = DateTime.Today.ToString("dd-MMM-yyyy");
        string time = DateTime.Now.ToString("h:mm:ss tt");
        string GetDateTime = DateTime.Today.ToString("dd-MMM-yyyy") + " " + DateTime.Now.ToString("h:mm:ss tt");

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            foreach (string key in Request.Form)
            {
                if (key.StartsWith("__") || key == "btnSubmit")
                    continue;

                string value = Request.Form[key];
                SaveControlData(key, value);
            }
        }

        private void SaveControlData(string controlName, string controlValue)
        {

            using (SqlConnection conn = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(@"INSERT INTO FormData 
                (ControlName, ControlValue, FormName, CreatedDateTime) 
                VALUES (@ControlName, @ControlValue, @FormName, @CreatedDateTime)", conn);

                cmd.Parameters.AddWithValue("@ControlName", controlName);
                cmd.Parameters.AddWithValue("@ControlValue", controlValue);
                cmd.Parameters.AddWithValue("@FormName", "Form1");
                cmd.Parameters.AddWithValue("@CreatedDateTime", GetDateTime);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

            }
        }



    }
}