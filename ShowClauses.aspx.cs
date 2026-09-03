using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class ShowClauses : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ToString());
        string constr = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;

        // Add ViewState properties to maintain search state across postbacks
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
            if (!IsPostBack)
            {
                string roleId = Session["RoleId"].ToString();
                string currentPageUrl = HttpContext.Current.Request.Url.AbsolutePath;
                string pageName = System.IO.Path.GetFileName(currentPageUrl);

                LoadDepartments();
                BindGridView();
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
            CurrentDepartment = ddlDepartment.SelectedItem.Value;
            BindGridView(CurrentSearchTerm, CurrentDepartment);
        }

        private void BindGridView(string searchTerm = "", string departmentId = "")
        {
            using (SqlConnection connection = new SqlConnection(constr))
            {
                string query = @"SELECT 
                                    c.ClauseID, 
                                    c.ClauseNumber, 
                                    c.ClauseTitle, 
                                    c.ClauseTitle_BH, 
                                    c.Requirement, 
                                    c.Requirement_BH, 
                                    c.isHeader, 
                                    c.ResultOptions, 
                                    c.ResultOptions_BH, 
                                    c.Standard_ID, 
                                    c.Department_ID, 
                                    c.Product_ID, 
                                    d.DepartmentID, 
                                    d.DepartmentName, 
                                    STRING_AGG(s.StandardName, ', ') AS StandardNames
                                 FROM Clauses c
                                 CROSS APPLY STRING_SPLIT(c.Standard_ID, ',') AS split
                                 INNER JOIN Standards s ON s.StandardID = TRY_CAST(split.value AS INT)
                                 INNER JOIN Department d ON d.DepartmentID = s.DepartmentId";

                List<string> conditions = new List<string>();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    conditions.Add("(c.ClauseTitle LIKE @SearchTerm OR c.ClauseNumber LIKE @SearchTerm)");
                }

                // Fix: Check if departmentId is a valid integer before adding to conditions
                if (!string.IsNullOrEmpty(departmentId) && departmentId != "--Select Department--")
                {
                    // Try to parse departmentId to integer
                    if (int.TryParse(departmentId, out int deptId))
                    {
                        conditions.Add("d.DepartmentID = @DepartmentID");
                    }
                    else
                    {
                        // If it's not a valid integer, don't apply department filter
                        // Optionally log this or handle it differently
                        Console.WriteLine($"Invalid department ID format: {departmentId}");
                    }
                }

                if (conditions.Count > 0)
                {
                    query += " WHERE " + string.Join(" AND ", conditions);
                }

                query += @" GROUP BY 
                                c.ClauseID, 
                                c.ClauseNumber, 
                                c.ClauseTitle, 
                                c.ClauseTitle_BH, 
                                c.Requirement, 
                                c.Requirement_BH, 
                                c.isHeader, 
                                c.ResultOptions, 
                                c.ResultOptions_BH, 
                                c.Standard_ID, 
                                c.Department_ID, 
                                c.Product_ID, 
                                d.DepartmentID, 
                                d.DepartmentName
                            ORDER BY c.ClauseID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        command.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");
                    }

                    // Fix: Only add the parameter if departmentId is a valid integer
                    if (!string.IsNullOrEmpty(departmentId) && departmentId != "--Select Department--")
                    {
                        if (int.TryParse(departmentId, out int deptId))
                        {
                            command.Parameters.AddWithValue("@DepartmentID", deptId);
                        }
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
            CurrentDepartment = ddlDepartment.SelectedItem.Text; // or SelectedValue
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
                //int clausesID = Convert.ToInt32(e.CommandArgument);
                string[] arguments = e.CommandArgument.ToString().Split(';');
                string clausesID = arguments[0];
                string encodedEventID = Convert.ToBase64String(Encoding.UTF8.GetBytes(clausesID));
                Response.Redirect($"EditClauses.aspx?id={encodedEventID}");
            }
        }


    }
}