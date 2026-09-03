using LIMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class AddTestSubSubParameters : System.Web.UI.Page
    {
        Parameters param = new Parameters();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDepartments();
                LoadUOMs();
                LoadTestMethods();
                Loadtestsubparameters();
            }
        }

        private void LoadDepartments()
        {
            try
            {
                DataTable dt = param.GetActiveDepartments();
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
        private void LoadUOMs()
        {
            try
            {
                DataTable dt = param.GetUOMs();
                ddlUOM.DataSource = dt;
                ddlUOM.DataTextField = "UOMName";
                ddlUOM.DataValueField = "UOMID";
                ddlUOM.DataBind();
                ddlUOM.Items.Insert(0, new ListItem("--Select UOM--", ""));
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Error loading UOMs: " + ex.Message + "');", true);
            }
        }
        private void LoadTestMethods()
        {
            try
            {
                DataTable dt = param.GetTestMethods();
                ddlTestMethod.DataSource = dt;
                ddlTestMethod.DataTextField = "TestMethod";
                ddlTestMethod.DataValueField = "TestMethodID";
                ddlTestMethod.DataBind();
                ddlTestMethod.Items.Insert(0, new ListItem("--Select Test Method--", ""));
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Error loading test methods: " + ex.Message + "');", true);
            }
        }
        private void Loadtestsubparameters()
        {
            try
            {
                Parameters param = new Parameters(); // or use an existing instance
                DataTable dt = param.GetTestSubParameters();

                ddltsp.DataSource = dt;
                ddltsp.DataTextField = "TestSubParameter";
                ddltsp.DataValueField = "TestSubParameterID";
                ddltsp.DataBind();
                ddltsp.Items.Insert(0, new ListItem("--Select TestSubParameter--", ""));
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Error loading TestSubParameter: " + ex.Message + "');", true);
            }
        }

        protected void ddltsp_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedTSPId = ddltsp.SelectedValue;

            if (!string.IsNullOrEmpty(selectedTSPId))
            {
                // Fetch UOMID and Protocol for selected Test Sub-Parameter
                string query = @"SELECT UOMID, Protocol 
                        FROM TestSubParameter 
                        WHERE TestSubParameterID = @TestSubParameterID";

                string uomId = null;
                string protocolText = null;

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TestSubParameterID", selectedTSPId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            uomId = reader["UOMID"]?.ToString();
                            protocolText = reader["Protocol"]?.ToString();
                        }
                    }
                }

                // Load dropdowns
                LoadUOMs();
                LoadTestMethods();

                // Handle UOM dropdown
                if (!string.IsNullOrEmpty(uomId))
                {
                    ListItem uomItem = ddlUOM.Items.FindByValue(uomId);
                    if (uomItem != null)
                    {
                        ddlUOM.ClearSelection();
                        uomItem.Selected = true;
                    }
                    ddlUOM.Enabled = false;
                }
                else
                {
                    ddlUOM.SelectedIndex = 0;
                    ddlUOM.Enabled = true;
                }

                // Handle Test Method dropdown (using protocol text)
                if (!string.IsNullOrEmpty(protocolText))
                {
                    // Find by text since we're matching the protocol text directly
                    ListItem methodItem = ddlTestMethod.Items.FindByText(protocolText);
                    if (methodItem != null)
                    {
                        ddlTestMethod.ClearSelection();
                        methodItem.Selected = true;
                        ddlTestMethod.Enabled = false;
                    }
                    else
                    {
                        // If protocol text doesn't match any existing item, add it and select it
                        ddlTestMethod.Items.Add(new ListItem(protocolText, protocolText));
                        ddlTestMethod.SelectedValue = protocolText;
                        ddlTestMethod.Enabled = false;
                    }
                }
                else
                {
                    ddlTestMethod.SelectedIndex = 0;
                    ddlTestMethod.Enabled = true;
                }
            }
            else
            {
                // Reset dropdowns if no Test Sub-Parameter is selected
                ddlUOM.SelectedIndex = 0;
                ddlUOM.Enabled = true;
                ddlTestMethod.SelectedIndex = 0;
                ddlTestMethod.Enabled = true;
            }
        }
        protected void ddlDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Hide both divs initially
            divFoodFields.Style["display"] = "none";
            divChemicalFields.Style["display"] = "none";

            if (ddlDepartment.SelectedItem != null && !string.IsNullOrEmpty(ddlDepartment.SelectedValue))
            {
                string selecteddepartmentId = ddlDepartment.SelectedValue;

                if (selecteddepartmentId == "5") // Food
                {
                    divFoodFields.Style["display"] = "block";
                }
                else if (selecteddepartmentId == "4") //softline
                {
                    divChemicalFields.Style["display"] = "block";
                }

            }
        }
        protected void txtSpecification_TextChanged(object sender, EventArgs e)
        {
            string specText = txtSpecification.Text.Trim();

            if (string.IsNullOrEmpty(specText))
            {
                txtSpecificationValue.Text = "";
                return;
            }

            // Convert to lowercase for easier matching
            string lowerSpec = specText.ToLower();

            // Check for minimum patterns (min., minimum)
            if (lowerSpec.StartsWith("min") || lowerSpec.StartsWith("minimum"))
            {
                System.Text.RegularExpressions.Match numberMatch =
                    System.Text.RegularExpressions.Regex.Match(specText, @"[\d.]+");

                if (numberMatch.Success)
                {
                    txtSpecificationValue.Text = ">= " + numberMatch.Value;
                    return;
                }
            }

            // Check for maximum patterns (max., maximum)
            if (lowerSpec.StartsWith("max") || lowerSpec.StartsWith("maximum"))
            {
                System.Text.RegularExpressions.Match numberMatch =
                    System.Text.RegularExpressions.Regex.Match(specText, @"[\d.]+");

                if (numberMatch.Success)
                {
                    txtSpecificationValue.Text = "<= " + numberMatch.Value;
                    return;
                }
            }

            // Check if it's just a number (integer or decimal)
            if (System.Text.RegularExpressions.Regex.IsMatch(specText, @"^\d+(\.\d+)?$"))
            {
                txtSpecificationValue.Text = "= " + specText;
                return;
            }

            // For any other text, copy as-is
            txtSpecificationValue.Text = specText;
        }
        private bool ValidateSpecification(string specificationValue, string specification)
        {
            try
            {
                if (string.IsNullOrEmpty(specification) || string.IsNullOrEmpty(specificationValue))
                    return true; // Allow empty values

                string lowerSpec = specification.ToLower().Trim();
                string trimmedSpecValue = specificationValue.Trim();

                // Check for minimum patterns
                if (lowerSpec.StartsWith("min") || lowerSpec.StartsWith("minimum"))
                {
                    // Should start with ">="
                    if (!trimmedSpecValue.StartsWith(">="))
                        return false;

                    // Extract and validate the number
                    string numberPart = trimmedSpecValue.Substring(2).Trim();
                    decimal value;
                    return decimal.TryParse(numberPart, out value);
                }

                // Check for maximum patterns
                if (lowerSpec.StartsWith("max") || lowerSpec.StartsWith("maximum"))
                {
                    // Should start with "<="
                    if (!trimmedSpecValue.StartsWith("<="))
                        return false;

                    // Extract and validate the number
                    string numberPart = trimmedSpecValue.Substring(2).Trim();
                    decimal value;
                    return decimal.TryParse(numberPart, out value);
                }

                // Check if specification is just a number
                decimal specNumber;
                if (decimal.TryParse(specification, out specNumber))
                {
                    // Should start with "="
                    if (!trimmedSpecValue.StartsWith("="))
                        return false;

                    // Extract and validate the number
                    string numberPart = trimmedSpecValue.Substring(1).Trim();
                    decimal value;
                    if (!decimal.TryParse(numberPart, out value))
                        return false;

                    // Numbers should match
                    return value == specNumber;
                }

                // For any other text, specification value should match specification
                return trimmedSpecValue.Equals(specification.Trim(), StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Validate required fields
            // Create a list to track validation errors
            List<string> validationErrors = new List<string>();

            // Validate required fields
            if (string.IsNullOrEmpty(txtTestsubsubParameter.Text))
                validationErrors.Add("Test Parameter is required");

            if (string.IsNullOrEmpty(ddlDepartment.SelectedValue))
                validationErrors.Add("Department is required");

            if (string.IsNullOrEmpty(ddlUOM.SelectedValue))
                validationErrors.Add("UOM is required");

            if (IsTestSubSubParameterExists(txtTestsubsubParameter.Text, ddltsp.SelectedValue))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                    "alert('This Test Sub-Sub-Parameter already exists under the selected Test Sub-Parameter.');", true);
                return;
            }
            // Validate numeric fields if they're not empty
            if (!string.IsNullOrEmpty(txtPosition.Text))
            {
                int position;
                if (!int.TryParse(txtPosition.Text, out position))
                    validationErrors.Add("Position must be a valid number");
            }
            if (!string.IsNullOrEmpty(txtSpecification.Text) && !string.IsNullOrEmpty(txtSpecificationValue.Text))
            {
                if (!ValidateSpecification(txtSpecificationValue.Text, txtSpecification.Text))
                {
                    validationErrors.Add("Specification Value does not meet the specification criteria. Please check your input.");
                }
            }
            // Validate decimal fields based on department-specific panels
            if (divFoodFields.Visible)
            {
                if (!string.IsNullOrEmpty(txtLOD.Text))
                {
                    decimal lod;
                    if (!decimal.TryParse(txtLOD.Text, out lod))
                        validationErrors.Add("LOD must be a valid number");
                }

                if (!string.IsNullOrEmpty(txtLOQ.Text))
                {
                    decimal loq;
                    if (!decimal.TryParse(txtLOQ.Text, out loq))
                        validationErrors.Add("LOQ must be a valid number");
                }

                if (!string.IsNullOrEmpty(txtLOR.Text))
                {
                    decimal lor;
                    if (!decimal.TryParse(txtLOR.Text, out lor))
                        validationErrors.Add("LOR must be a valid number");
                }
            }

            if (divChemicalFields.Visible)
            {
                if (!string.IsNullOrEmpty(txtMDL.Text))
                {
                    decimal mdl;
                    if (!decimal.TryParse(txtMDL.Text, out mdl))
                        validationErrors.Add("MDL must be a valid number");
                }
            }

            // Create connection and command
            string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Use transaction to ensure both operations complete successfully
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Insert into TestSubSubParameter table
                        string insertQuery = @"INSERT INTO TestSubSubParameter (
                            TestSubSubParameter, testsubparameterid, Specification, uomid, 
                            protocol, LOD, position, UOMName, 
                            LOR, LOQ, MDL, CASNo,SpecificationValue
                            ) VALUES (
                            @TestSubSubParameter, @testsubparameterid, @Specification, @uomid, 
                            @protocol, @LOD, @position, @UOMName, 
                            @LOR, @LOQ, @MDL, @CASNo,@SpecificationValue
                            )";


                        using (SqlCommand cmd = new SqlCommand(insertQuery, conn, transaction))
                        {
                            // Add parameters
                            cmd.Parameters.AddWithValue("@TestSubSubParameter", txtTestsubsubParameter.Text.Trim());
                            cmd.Parameters.AddWithValue("@testsubparameterid", ddltsp.SelectedValue);
                            cmd.Parameters.AddWithValue("@uomid", ddlUOM.SelectedValue);
                            cmd.Parameters.AddWithValue("@Specification", string.IsNullOrEmpty(txtSpecification.Text) ? DBNull.Value : (object)txtSpecification.Text.Trim());
                            cmd.Parameters.AddWithValue("@position", string.IsNullOrEmpty(txtPosition.Text) ? DBNull.Value : (object)Convert.ToInt32(txtPosition.Text));
                            cmd.Parameters.AddWithValue("@LOD", string.IsNullOrEmpty(txtLOD.Text) ? DBNull.Value : (object)Convert.ToDecimal(txtLOD.Text));
                            cmd.Parameters.AddWithValue("@LOQ", string.IsNullOrEmpty(txtLOQ.Text) ? DBNull.Value : (object)Convert.ToDecimal(txtLOQ.Text));
                            cmd.Parameters.AddWithValue("@UOMName", string.IsNullOrEmpty(ddlUOM.SelectedItem.Text) ? DBNull.Value : (object)ddlUOM.SelectedItem.Text);
                            cmd.Parameters.AddWithValue("@LOR", string.IsNullOrEmpty(txtLOR.Text) ? DBNull.Value : (object)Convert.ToDecimal(txtLOR.Text));
                            cmd.Parameters.AddWithValue("@CASNo", string.IsNullOrEmpty(txtCASNo.Text) ? DBNull.Value : (object)txtCASNo.Text.Trim());
                            cmd.Parameters.AddWithValue("@MDL", string.IsNullOrEmpty(txtMDL.Text) ? DBNull.Value : (object)Convert.ToDecimal(txtMDL.Text));
                            cmd.Parameters.AddWithValue("@Protocol", string.IsNullOrEmpty(ddlTestMethod.SelectedValue) ? DBNull.Value : (object)ddlTestMethod.SelectedItem.Text);
                            cmd.Parameters.AddWithValue("@SpecificationValue", string.IsNullOrEmpty(txtSpecificationValue.Text) ? DBNull.Value : (object)txtSpecificationValue.Text.Trim());
                            cmd.ExecuteNonQuery();
                        }

                        // 2. Update hassubparameter column in TestSubParameter table
                        string updateQuery = @"UPDATE TestSubParameter 
                             SET hassubparameter = 1 
                             WHERE TestSubParameterID = @TestSubParameterID";

                        using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn, transaction))
                        {
                            updateCmd.Parameters.AddWithValue("@TestSubParameterID", ddltsp.SelectedValue);
                            updateCmd.ExecuteNonQuery();
                        }

                        // Commit transaction if both operations succeed
                        transaction.Commit();

                        // Clear form after successful submission
                        ClearForm();

                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Test Sub-Sub-Parameter added successfully');", true);
                        Response.Redirect("TestSubSubParameterShow.aspx", false);
                    }
                    catch (Exception ex)
                    {
                        // Rollback transaction if any operation fails
                        transaction.Rollback();

                        // Handle exception
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Error adding test sub-sub-parameter: " + ex.Message.Replace("'", "\\'") + "');", true);
                    }
                }
            }
        }
        private bool IsTestSubSubParameterExists(string testSubSubParameter, string testSubParameterId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;

            string query = @"SELECT COUNT(*) FROM TestSubSubParameter 
                     WHERE TestSubSubParameter = @TestSubSubParameter 
                     AND TestSubParameterID = @TestSubParameterID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        cmd.Parameters.AddWithValue("@TestSubSubParameter", testSubSubParameter.Trim());
                        cmd.Parameters.AddWithValue("@TestSubParameterID", testSubParameterId);

                        conn.Open();
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0; // true = already exists
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                            "alert('Error checking Test Sub-Sub-Parameter: " + ex.Message + "');", true);
                        return false;
                    }
                }
            }
        }

        protected void ClearForm()
        {
            txtTestsubsubParameter.Text = string.Empty;
            txtSpecification.Text = string.Empty;
            txtPosition.Text = string.Empty;
            txtLOD.Text = string.Empty;
            txtLOQ.Text = string.Empty;
            txtLOR.Text = string.Empty;
            txtCASNo.Text = string.Empty;
            txtMDL.Text = string.Empty;
            txtSpecificationValue.Text = string.Empty;

            if (ddlDepartment.Items.Count > 0)
                ddlDepartment.SelectedIndex = 0;

            if (ddltsp.Items.Count > 0)
            {
                ddltsp.SelectedIndex = 0;
                ddltsp.Items.Clear();
                ddltsp.Items.Add(new ListItem("--Select Test Sub-Parameter--", ""));
            }

            if (ddlUOM.Items.Count > 0)
                ddlUOM.SelectedIndex = 0;

            if (ddlTestMethod.Items.Count > 0)
                ddlTestMethod.SelectedIndex = 0;

            // Hide department-specific fields
            divFoodFields.Style["display"] = "none";
            divChemicalFields.Style["display"] = "none";
        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

    }
}