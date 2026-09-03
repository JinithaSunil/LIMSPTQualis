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
    public partial class AddTestSubParameters : System.Web.UI.Page
    {
        Parameters param = new Parameters();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDepartments();
                LoadUOMs();
                LoadTestMethods();
                LoadRequirements();
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
        private void LoadRequirements()
        {
            try
            {
                DataTable dt = param.GetRequirements();
                ddlrequirement.DataSource = dt;
                ddlrequirement.DataTextField = "Requirement";
                ddlrequirement.DataValueField = "RequirementID";
                ddlrequirement.DataBind();
                ddlrequirement.Items.Insert(0, new ListItem("--Select Requirement--", ""));
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Error loading requirements: " + ex.Message + "');", true);
            }
        }
        private void BindTestParameters(string departmentid)
        {
            try
            {
                Parameters param = new Parameters(); // or reuse an existing instance
                DataTable dt = param.GetTestParametersByDepartment(departmentid);

                ddltp.Items.Clear(); // Clear previous items
                foreach (DataRow row in dt.Rows)
                {
                    ddltp.Items.Add(new ListItem(row["TestParameter"].ToString(), row["TestParameterID"].ToString()));
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Error loading test parameters: {ex.Message}');", true);
            }
        }
        protected void ddltp_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedTPId = ddltp.SelectedValue;

            if (!string.IsNullOrEmpty(selectedTPId))
            {
                // Fetch RequirementID, UOMID, and Protocol for selected Test Parameter
                string query = @"SELECT RequirementID, UOMID, Protocol 
                        FROM TestParameter 
                        WHERE TestParameterID = @TestParameterID";

                string requirementId = null;
                string uomId = null;
                string protocolText = null;

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TestParameterID", selectedTPId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            requirementId = reader["RequirementID"]?.ToString();
                            uomId = reader["UOMID"]?.ToString();
                            protocolText = reader["Protocol"]?.ToString();
                        }
                    }
                }

                // Load all dropdowns again
                LoadRequirements();
                LoadUOMs();
                LoadTestMethods();

                // Handle Requirement dropdown
                if (!string.IsNullOrEmpty(requirementId))
                {
                    ListItem reqItem = ddlrequirement.Items.FindByValue(requirementId);
                    if (reqItem != null)
                    {
                        ddlrequirement.ClearSelection();
                        reqItem.Selected = true;
                    }

                    ddlrequirement.Enabled = false; // make dropdown readonly
                }
                else
                {
                    ddlrequirement.SelectedIndex = 0;
                    ddlrequirement.Enabled = true; // fallback to default if not found
                }

                // Handle UOM dropdown
                if (!string.IsNullOrEmpty(uomId))
                {
                    ListItem uomItem = ddlUOM.Items.FindByValue(uomId);
                    if (uomItem != null)
                    {
                        ddlUOM.ClearSelection();
                        uomItem.Selected = true;
                    }
                   // ddlUOM.Enabled = false;
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
                // Reset all dropdowns if no Test Parameter is selected
                ddlrequirement.SelectedIndex = 0;
                ddlrequirement.Enabled = true;
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

            // Clear and reset the Test Parameter dropdown
            ddltp.Items.Clear();
            ddltp.Items.Add(new ListItem("--Select Test Parameter--", ""));

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

                string departmentId = ddlDepartment.SelectedValue;
                // Bind Test Parameters based on selected department
                BindTestParameters(departmentId);
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
        private bool IsTestSubParameterExists(string testSubParameter, string testParameterId, string requirementId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
            string query;

            if (string.IsNullOrEmpty(requirementId))
            {
                // Check for NULL or empty RequirementID
                query = @"SELECT COUNT(*) FROM TestSubParameter 
                  WHERE TestSubParameter = @TestSubParameter 
                  AND TestParameterID = @TestParameterID 
                  AND (RequirementID IS NULL OR RequirementID = '')";
            }
            else
            {
                // Exact match for RequirementID
                query = @"SELECT COUNT(*) FROM TestSubParameter 
                  WHERE TestSubParameter = @TestSubParameter 
                  AND TestParameterID = @TestParameterID 
                  AND RequirementID = @RequirementID";
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        cmd.Parameters.AddWithValue("@TestSubParameter", testSubParameter.Trim());
                        cmd.Parameters.AddWithValue("@TestParameterID", testParameterId);

                        if (!string.IsNullOrEmpty(requirementId))
                        {
                            cmd.Parameters.AddWithValue("@RequirementID", requirementId);
                        }

                        conn.Open();
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                            "alert('Error checking Test Sub-Parameter uniqueness: " + ex.Message + "');", true);
                        return false;
                    }
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Validate required fields
            // Create a list to track validation errors
            List<string> validationErrors = new List<string>();
            string requirementId = ddlrequirement.SelectedValue;
            // Validate required fields
            if (string.IsNullOrEmpty(txtTestsubParameter.Text))
                validationErrors.Add("Test Parameter is required");

            if (string.IsNullOrEmpty(ddlDepartment.SelectedValue))
                validationErrors.Add("Department is required");

            if (string.IsNullOrEmpty(ddlUOM.SelectedValue))
                validationErrors.Add("UOM is required");

            if (IsTestSubParameterExists(txtTestsubParameter.Text, ddltp.SelectedValue, requirementId))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                    "alert('This Test Sub-Parameter already exists for the selected Test Parameter and Requirement.');", true);
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
                        // 1. Insert into TestSubParameter table
                        string insertQuery = @"INSERT INTO TestSubParameter (
                                    TestSubParameter, TestParameterID, Specification, uomid, 
                                    protocol, Position, LOD, LOQ, UOMName, 
                                    LOR, CASNo, MDL, RequirementId,SpecificationValue
                                    ) VALUES (
                                    @TestSubParameter, @TestParameterID, @Specification, @uomid, 
                                    @protocol, @Position, @LOD, @LOQ, @UOMName, 
                                    @LOR, @CASNo, @MDL, @RequirementId,@SpecificationValue
                                    )";

                        using (SqlCommand cmd = new SqlCommand(insertQuery, conn, transaction))
                        {
                            // Add parameters
                            cmd.Parameters.AddWithValue("@TestSubParameter", txtTestsubParameter.Text.Trim());
                            cmd.Parameters.AddWithValue("@TestParameterID", ddltp.SelectedValue);
                            cmd.Parameters.AddWithValue("@uomid", ddlUOM.SelectedValue);
                            cmd.Parameters.AddWithValue("@Specification", string.IsNullOrEmpty(txtSpecification.Text) ? DBNull.Value : (object)txtSpecification.Text.Trim());
                            cmd.Parameters.AddWithValue("@Position", string.IsNullOrEmpty(txtPosition.Text) ? DBNull.Value : (object)Convert.ToInt32(txtPosition.Text));
                            cmd.Parameters.AddWithValue("@LOD", string.IsNullOrEmpty(txtLOD.Text) ? DBNull.Value : (object)Convert.ToDecimal(txtLOD.Text));
                            cmd.Parameters.AddWithValue("@LOQ", string.IsNullOrEmpty(txtLOQ.Text) ? DBNull.Value : (object)Convert.ToDecimal(txtLOQ.Text));
                            cmd.Parameters.AddWithValue("@UOMName", string.IsNullOrEmpty(ddlUOM.SelectedItem.Text) ? DBNull.Value : (object)ddlUOM.SelectedItem.Text);
                            cmd.Parameters.AddWithValue("@LOR", string.IsNullOrEmpty(txtLOR.Text) ? DBNull.Value : (object)Convert.ToDecimal(txtLOR.Text));
                            cmd.Parameters.AddWithValue("@CASNo", string.IsNullOrEmpty(txtCASNo.Text) ? DBNull.Value : (object)txtCASNo.Text.Trim());
                            cmd.Parameters.AddWithValue("@MDL", string.IsNullOrEmpty(txtMDL.Text) ? DBNull.Value : (object)Convert.ToDecimal(txtMDL.Text));
                            cmd.Parameters.AddWithValue("@RequirementId", string.IsNullOrEmpty(ddlrequirement.SelectedValue) ? DBNull.Value : (object)ddlrequirement.SelectedValue);
                            cmd.Parameters.AddWithValue("@Protocol", string.IsNullOrEmpty(ddlTestMethod.SelectedValue) ? DBNull.Value : (object)ddlTestMethod.SelectedItem.Text);
                            cmd.Parameters.AddWithValue("@SpecificationValue", string.IsNullOrEmpty(txtSpecificationValue.Text) ? DBNull.Value : (object)txtSpecificationValue.Text.Trim());
                            cmd.ExecuteNonQuery();
                        }

                        // 2. Update HasSubParameter column in TestParameter table
                        string updateQuery = @"UPDATE TestParameter 
                                     SET hassubparameter = 1 
                                     WHERE TestParameterID = @TestParameterID";

                        using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn, transaction))
                        {
                            updateCmd.Parameters.AddWithValue("@TestParameterID", ddltp.SelectedValue);
                            updateCmd.ExecuteNonQuery();
                        }

                        // Commit transaction if both operations succeed
                        transaction.Commit();

                        // Clear form after successful submission
                        ClearForm();

                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Test Sub-Parameter added successfully');", true);
                        Response.Redirect("TestSubParameterShow.aspx", false);
                    }
                    catch (Exception ex)
                    {
                        // Rollback transaction if any operation fails
                        transaction.Rollback();

                        // Handle exception
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Error adding test sub-parameter: " + ex.Message.Replace("'", "\\'") + "');", true);
                    }
                }
                conn.Close();
            }
        }
        protected void ClearForm()
        {
            txtTestsubParameter.Text = string.Empty;
            txtSpecification.Text = string.Empty;
            txtPosition.Text = string.Empty;
            txtLOD.Text = string.Empty;
            txtLOQ.Text = string.Empty;
            txtLOR.Text = string.Empty;
            txtCASNo.Text = string.Empty;
            txtMDL.Text = string.Empty;
            txtSpecificationValue.Text = string.Empty;

            // Reset dropdowns to first item (usually the "--Select--" option)
            ddlDepartment.SelectedIndex = 0;
            ddltp.Items.Clear();
            ddltp.Items.Add(new ListItem("--Select Test Parameter--", ""));
            ddlUOM.SelectedIndex = 0;
            ddlTestMethod.SelectedIndex = 0;
            ddlrequirement.SelectedIndex = 0;

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