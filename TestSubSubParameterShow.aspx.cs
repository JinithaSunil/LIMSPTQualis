using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTable = System.Data.DataTable;

namespace LIMS
{
    public partial class TestSubSubParameterShow : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ToString());
        string constr = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;

        private string CurrentSearchTerm
        {
            get { return ViewState["SearchTerm"] as string ?? string.Empty; }
            set { ViewState["SearchTerm"] = value; }
        }

        private string CurrentDepartment
        {
            get { return ViewState["Department"] as string ?? string.Empty; }
            set { ViewState["Department"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddDays(-1));
            Response.Cache.SetNoStore();

            if (Session["EmailId"] == null)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Session Timeout'); setTimeout(function() { window.location = 'Login.aspx'; }, 100); </script>");
                return;
            }

            if (!IsPostBack)
            {
                string roleId = Session["RoleId"].ToString();
                string currentPageUrl = HttpContext.Current.Request.Url.AbsolutePath;
                string pageName = System.IO.Path.GetFileName(currentPageUrl);

                BindGridView();
                LoadDepartments();
            }
        }
        private void LoadDepartments()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
            string query = "SELECT DepartmentID, DepartmentName FROM Department WHERE IsActive = 1";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        ddlDepartment.DataSource = dt;
                        ddlDepartment.DataTextField = "DepartmentName";
                        ddlDepartment.DataValueField = "DepartmentID";
                        ddlDepartment.DataBind();
                        ddlDepartment.Items.Insert(0, new ListItem("--Select Department--", ""));
                    }
                    catch (Exception ex)
                    {
                        // Handle exception
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", $"alert('Error loading departments: {ex.Message.Replace("'", "\\'")}');", true);
                    }
                }
            }
        }

        protected void ddlDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentSearchTerm = txtSearch.Text.Trim();
            CurrentDepartment = ddlDepartment.SelectedValue;
            BindGridView(CurrentSearchTerm, CurrentDepartment);
        }


        private void BindGridView(string searchTerm = "", string departmentId = "")
        {
            using (SqlConnection connection = new SqlConnection(constr))
            {
                string query = @"
         SELECT tssp.*, 
          TestParameter, 
          TestSubParameter, 
          Department,
          CASE 
              WHEN tp.departmentID = 5  THEN 
                  CONCAT('LOR: ', ISNULL(tssp.LOR, 'N/A'), 
                         CHAR(13) + CHAR(10) + 'LOQ: ', ISNULL(tssp.LOQ, 'N/A'),
                         CHAR(13) + CHAR(10) + 'LOD: ', ISNULL(tssp.LOD, 'N/A'))
             WHEN tp.departmentID = 4  THEN 
                  CONCAT('CAS NO: ', ISNULL(tssp.CASNo, 'N/A'), 
                         CHAR(13) + CHAR(10) + 'MDL: ', ISNULL(tssp.MDL, 'N/A'))
          END AS FormattedOtherDetails
   FROM TestSubSubParameter tssp 
        LEFT JOIN TestSubParameter tsp ON tssp.TestSubParameterID = tsp.TestSubParameterID 
        LEFT JOIN TestParameter tp ON tsp.TestParameterID = tp.TestParameterID
        LEFT JOIN Reportlist rl ON tp.ReportID = rl.ReportID";

                List<string> conditions = new List<string>();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    conditions.Add("TestSubSubParameter LIKE @SearchTerm");
                }

                if (!string.IsNullOrEmpty(departmentId) && departmentId != "--Select Department--")
                {
                    conditions.Add(" tp.departmentID = @DepartmentID");
                }

                if (conditions.Count > 0)
                {
                    query += " WHERE " + string.Join(" AND ", conditions);
                }

                query += " ORDER BY TestSubSubParameter";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        command.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");
                    }

                    if (!string.IsNullOrEmpty(departmentId) && departmentId != "--Select Department--")
                    {
                        command.Parameters.AddWithValue("@DepartmentID", departmentId);
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    GridView1.DataSource = dt;
                    GridView1.DataBind();
                }
            }
        }


        protected void btnSearch_Click(object sender, EventArgs e)
        {
            CurrentSearchTerm = txtSearch.Text.Trim();
            CurrentDepartment = ddlDepartment.SelectedValue; 
            BindGridView(CurrentSearchTerm, CurrentDepartment);
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            CurrentSearchTerm = string.Empty;
            CurrentDepartment = string.Empty;
            ddlDepartment.SelectedIndex = 0;
            txtSearch.Text = "";
            BindGridView();
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            // Pass the current search parameters to maintain filter during pagination
            BindGridView(CurrentSearchTerm, CurrentDepartment);
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ItemEdit")
            {
                int testParameterId = Convert.ToInt32(e.CommandArgument);
                Response.Redirect($"TestSubSubParameterEdit.aspx?id={testParameterId}");
            }
        }
    }
}