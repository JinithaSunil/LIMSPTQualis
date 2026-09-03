using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace LIMS
{
    public partial class EditClauses : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        string constr = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDepartments();
                LoadStandard();

                if (Request.QueryString["id"] != null)
                {
                    string pageID = Request.QueryString["id"];
                    string encodedString = pageID;
                    byte[] decodedBytes = Convert.FromBase64String(encodedString);
                    string decodedEventListId = Encoding.UTF8.GetString(decodedBytes);
                    lblClausesID.Text = decodedEventListId;
                    lblClausesID.Visible = false;

                    LoadData();
                }

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
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Error loading departments: " + ex.Message + "');", true);
                    }
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("ShowClauses.aspx");
        }

        protected void ddlDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            int DepartmentID = Convert.ToInt32(ddlDepartment.SelectedItem.Value);
            LoadStandard(DepartmentID);
        }

        private void LoadStandard(int departmentID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
            string query = "SELECT * FROM Standards where DepartmentId = '" + departmentID + "' ";

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

                        ddlStandards.DataSource = dt;
                        ddlStandards.DataTextField = "StandardName";
                        ddlStandards.DataValueField = "StandardID";
                        ddlStandards.DataBind();
                        ddlStandards.Items.Insert(0, new ListItem("--Select Standard--", ""));
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Error loading Standard Name: " + ex.Message + "');", true);
                    }
                }
            }
        }

        private void LoadStandard()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
            string query = "SELECT * FROM Standards ";

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

                        ddlStandards.DataSource = dt;
                        ddlStandards.DataTextField = "StandardName";
                        ddlStandards.DataValueField = "StandardID";
                        ddlStandards.DataBind();
                        ddlStandards.Items.Insert(0, new ListItem("--Select Standard--", ""));
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Error loading Standard Name: " + ex.Message + "');", true);
                    }
                }
            }
        }

        private void LoadData()
        {
            string clauseID = lblClausesID.Text;
            DataTable dtClauses = _cm.GetClauses(clauseID);

            if (dtClauses.Rows.Count > 0)
            {
                DataRow row = dtClauses.Rows[0];

                txtClauseNumber.Text = GetSafeString(row["ClauseNumber"]);
                txtClauseTitle.Text = GetSafeString(row["ClauseTitle"]);
                txtClauseTitleBH.Text = GetSafeString(row["ClauseTitle_BH"]);
                txtRequirement.Text = GetSafeString(row["Requirement"]);
                txtRequirementBH.Text = GetSafeString(row["Requirement_BH"]);
                txtResultOptions.Text = GetSafeString(row["ResultOptions"]);
                txtResultOptionsBH.Text = GetSafeString(row["ResultOptions_BH"]);

                string departmentId = GetSafeString(row["Department_ID"]);
                if (!string.IsNullOrEmpty(departmentId))
                {
                    ddlDepartment.SelectedValue = departmentId;
                    LoadStandard(Convert.ToInt32(departmentId));
                }

                string standardId = GetSafeString(row["Standard_ID"]);
                if (!string.IsNullOrEmpty(standardId))
                {
                    ddlStandards.SelectedValue = standardId;
                }

                string isHeaderValue = GetSafeString(row["isHeader"]);
                if (!string.IsNullOrEmpty(isHeaderValue))
                {
                    ddlIsHeader.SelectedValue = isHeaderValue;
                }
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script>alert('Clauses Not Found!');</script>");
            }
        }


        private string GetSafeString(object value)
        {
            return value == null || value == DBNull.Value ? string.Empty : value.ToString();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            // Get values from UI controls
            string clauseId = lblClausesID.Text.Trim();
            string clauseNumber = txtClauseNumber.Text.Trim();
            string clauseTitle = txtClauseTitle.Text.Trim();
            string clauseTitleBH = txtClauseTitleBH.Text.Trim();
            string requirement = txtRequirement.Text.Trim();
            string requirementBH = txtRequirementBH.Text.Trim();
            string resultOptions = txtResultOptions.Text.Trim();
            string resultOptionsBH = txtResultOptionsBH.Text.Trim();
            string isHeader = ddlIsHeader.SelectedValue;
            string departmentId = ddlDepartment.SelectedValue;
            string standardId = ddlStandards.SelectedValue;

            // Safety check
            if (string.IsNullOrEmpty(clauseId))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                    "alert('Invalid Clause ID.');", true);
                return;
            }

            // SQL update query
            string query = @"UPDATE Clauses
                                SET ClauseNumber = @ClauseNumber,
                                    ClauseTitle = @ClauseTitle,
                                    ClauseTitle_BH = @ClauseTitleBH,
                                    Requirement = @Requirement,
                                    Requirement_BH = @RequirementBH,
                                    ResultOptions = @ResultOptions,
                                    ResultOptions_BH = @ResultOptionsBH,
                                    isHeader = @IsHeader,
                                    Department_ID = @DepartmentID,
                                    Standard_ID = @StandardID
                                WHERE ClauseID = @ClauseID";

            using (SqlConnection conn = new SqlConnection(constr))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                // Parameters
                cmd.Parameters.AddWithValue("@ClauseNumber", clauseNumber);
                cmd.Parameters.AddWithValue("@ClauseTitle", clauseTitle);
                cmd.Parameters.AddWithValue("@ClauseTitleBH", clauseTitleBH);
                cmd.Parameters.AddWithValue("@Requirement", requirement);
                cmd.Parameters.AddWithValue("@RequirementBH", requirementBH);
                cmd.Parameters.AddWithValue("@ResultOptions", resultOptions);
                cmd.Parameters.AddWithValue("@ResultOptionsBH", resultOptionsBH);
                cmd.Parameters.AddWithValue("@IsHeader", isHeader);
                cmd.Parameters.AddWithValue("@DepartmentID", departmentId);
                cmd.Parameters.AddWithValue("@StandardID", standardId);
                cmd.Parameters.AddWithValue("@ClauseID", clauseId);

                try
                {
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    conn.Close();
                    if (rows > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                            "alert('Clause updated successfully!'); window.location='ShowClauses.aspx';", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                            "alert('No records were updated. Please check Clause ID.');", true);
                    }
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                        "alert('Error updating clause: " + ex.Message.Replace("'", "") + "');", true);
                }
            }
        }


    }
}