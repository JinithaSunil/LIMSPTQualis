using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LIMS.Models;
namespace LIMS
{
    public partial class TestParameterEdit : System.Web.UI.Page
    {
        Parameters param = new Parameters();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Load all dropdown data
                LoadDepartments();
                LoadUOMs();
                LoadTestMethods();
                LoadRequirements();

                // Check if we're in edit mode (ID parameter exists in the URL)
                if (Request.QueryString["id"] != null)
                {
                    int testParameterId;
                    if (int.TryParse(Request.QueryString["id"], out testParameterId))
                    {
                        // Load test parameter data
                        LoadTestParameterData(testParameterId);
                        string departmentid = ddlDepartment.SelectedValue;
                        ShowDepartmentFields(departmentid);
                        // Change button text to "Update" since we're editing
                        btnUpdate.Text = "Update";
                    }
                }
                else
                {
                    // New parameter mode
                    btnUpdate.Text = "Submit";
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
                        // Handle exception
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Error loading departments: " + ex.Message + "');", true);
                    }
                }
            }
        }
        private void LoadUOMs()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
            string query = "SELECT UOMID, UOMName FROM UOM";

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

                        ddlUOM.DataSource = dt;
                        ddlUOM.DataTextField = "UOMName";
                        ddlUOM.DataValueField = "UOMID";
                        ddlUOM.DataBind();
                        ddlUOM.Items.Insert(0, new ListItem("--Select UOM--", ""));
                    }
                    catch (Exception ex)
                    {
                        // Handle exception
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Error loading UOMs: " + ex.Message + "');", true);
                    }
                }
            }
        }
        private void LoadTestMethods()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
            string query = "Select * from TestMethod";

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

                        ddlTestMethod.DataSource = dt;
                        ddlTestMethod.DataTextField = "TestMethod";
                        ddlTestMethod.DataValueField = "TestMethodID";
                        ddlTestMethod.DataBind();
                        ddlTestMethod.Items.Insert(0, new ListItem("--Select Test Method--", ""));
                    }
                    catch (Exception ex)
                    {
                        // Handle exception
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Error loading test methods: " + ex.Message + "');", true);
                    }
                }
            }
        }
        private void LoadRequirements()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
            string query = "select * from Requirement";

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

                        ddlrequirement.DataSource = dt;
                        ddlrequirement.DataTextField = "Requirement";
                        ddlrequirement.DataValueField = "RequirementID";
                        ddlrequirement.DataBind();
                        ddlrequirement.Items.Insert(0, new ListItem("--Select Requirement--", ""));
                    }
                    catch (Exception ex)
                    {
                        // Handle exception
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Error loading Requirement: " + ex.Message + "');", true);
                    }
                }
            }
        }

        protected void ddlDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Hide both divs initially
            divFoodFields.Style["display"] = "none";
            divChemicalFields.Style["display"] = "none";

            ddlrequirement.Items.Clear();

            if (ddlDepartment.SelectedItem != null && !string.IsNullOrEmpty(ddlDepartment.SelectedValue))
            {
                string departmentId = ddlDepartment.SelectedValue;

                if (departmentId == "5") // Food
                {
                    divFoodFields.Style["display"] = "block";
                }
                else if (departmentId == "4") //softline
                {
                    divChemicalFields.Style["display"] = "block";
                }

                try
                {
                    DataTable dt = param.GetRequirementsByDepartment(departmentId);

                    ddlrequirement.DataSource = dt;
                    ddlrequirement.DataTextField = "Requirement";
                    ddlrequirement.DataValueField = "RequirementID";
                    ddlrequirement.DataBind();
                    ddlrequirement.Items.Insert(0, new ListItem("--Select Requirement--", ""));
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Error loading requirements: " + ex.Message.Replace("'", "") + "');", true);
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

        private void LoadTestParameterData(int testParameterId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
            string query = "SELECT * FROM TestParameter WHERE TestParameterId = @TestParameterId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        cmd.Parameters.AddWithValue("@TestParameterId", testParameterId);
                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Populate basic fields
                                txtTestParameter.Text = reader["TestParameter"].ToString();

                                // Set Department dropdown
                                string departmentName = reader["Department"].ToString();
                                string departmentID = reader["departmentID"].ToString();
                                if (!string.IsNullOrEmpty(departmentID))
                                {
                                    ddlDepartment.SelectedValue = departmentID;
                                }
                                ddlDepartment_SelectedIndexChanged(ddlDepartment, EventArgs.Empty);

                                // UOM dropdown
                                if (reader["uomid"] != DBNull.Value)
                                {
                                    ddlUOM.SelectedValue = reader["uomid"].ToString();
                                }

                                // Test Method dropdown - match by text since we store TestMethod text in Protocol
                                string protocol = reader["Protocol"].ToString();
                                SetDropDownByText(ddlTestMethod, protocol);

                                // Requirement dropdown
                                if (reader["RequirementID"] != DBNull.Value)
                                {
                                    ddlrequirement.SelectedValue = reader["RequirementID"].ToString();
                                }

                                // All other text fields
                                txtSpecification.Text = reader["specification"].ToString();
                                txtSpecificationValue.Text = reader["SpecificationValue"].ToString();

                                if (reader["Position"] != DBNull.Value)
                                {
                                    txtPosition.Text = reader["Position"].ToString();
                                }

                                txtClause.Text = reader["Clause"].ToString();

                                // Department-specific fields
                                // Food department fields
                                if (reader["LOD"] != DBNull.Value)
                                {
                                    txtLOD.Text = reader["LOD"].ToString();
                                }

                                if (reader["LOQ"] != DBNull.Value)
                                {
                                    txtLOQ.Text = reader["LOQ"].ToString();
                                }

                                if (reader["LOR"] != DBNull.Value)
                                {
                                    txtLOR.Text = reader["LOR"].ToString();
                                }

                                // Chemical department fields
                                if (reader["MDL"] != DBNull.Value)
                                {
                                    txtMDL.Text = reader["MDL"].ToString();
                                }

                                txtCASNo.Text = reader["CASNo"].ToString();

                                // Show the appropriate department sections based on department selection
                                ShowDepartmentFields(departmentID);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle exception
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Error loading test parameter data: " + ex.Message + "');", true);
                    }
                }
            }
        }
        private void SetDropDownByText(DropDownList dropDown, string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            ListItem item = dropDown.Items.FindByText(text);
            if (item != null)
            {
                item.Selected = true;
            }
        }

        // Helper method to show department-specific fields
        private void ShowDepartmentFields(string departmentID)
        {
            // Hide both divs initially
            divFoodFields.Style["display"] = "none";
            divChemicalFields.Style["display"] = "none";

            if (departmentID == "5") // Food
            {
                divFoodFields.Style["display"] = "block";
            }
            else if (departmentID == "4") // Softline
            {
                divChemicalFields.Style["display"] = "block";
            }
        }
        private bool IsTestParameterExists(string testParameter, string department, string requirementId, int? excludeId = null)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
            string query;

            // Build the base query with RequirementID logic
            string baseCondition;
            if (string.IsNullOrEmpty(requirementId))
            {
                // Check for NULL or empty RequirementID
                baseCondition = @"TestParameter = @TestParameter 
                         AND Department = @Department 
                         AND (RequirementID IS NULL OR RequirementID = '')";
            }
            else
            {
                // Exact match for RequirementID
                baseCondition = @"TestParameter = @TestParameter 
                         AND Department = @Department 
                         AND RequirementID = @RequirementID";
            }

            if (excludeId.HasValue)
            {
                // For update operations - exclude the current record being updated
                query = $"SELECT COUNT(*) FROM TestParameter WHERE {baseCondition} AND TestParameterId != @ExcludeId";
            }
            else
            {
                // For insert operations - check all records
                query = $"SELECT COUNT(*) FROM TestParameter WHERE {baseCondition}";
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        cmd.Parameters.AddWithValue("@TestParameter", testParameter.Trim());
                        cmd.Parameters.AddWithValue("@Department", department);

                        if (!string.IsNullOrEmpty(requirementId))
                        {
                            cmd.Parameters.AddWithValue("@RequirementID", requirementId);
                        }

                        if (excludeId.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@ExcludeId", excludeId.Value);
                        }

                        conn.Open();
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0; // true = already exists
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                            "alert('Error checking Test Parameter uniqueness: " + ex.Message.Replace("'", "\\'") + "');", true);
                        return false;
                    }
                }
            }
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
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            List<string> validationErrors = new List<string>();

            // Validate required fields
            if (string.IsNullOrEmpty(txtTestParameter.Text))
                validationErrors.Add("Test Parameter is required");

            if (string.IsNullOrEmpty(ddlDepartment.SelectedValue))
                validationErrors.Add("Department is required");

            if (string.IsNullOrEmpty(ddlUOM.SelectedValue))
                validationErrors.Add("UOM is required");

            // Check for duplicate Test Parameter name
            if (!string.IsNullOrEmpty(txtTestParameter.Text) && !string.IsNullOrEmpty(ddlDepartment.SelectedValue))
            {
                string requirementId = string.IsNullOrEmpty(ddlrequirement.SelectedValue) ? null : ddlrequirement.SelectedValue;

                if (Request.QueryString["id"] != null)
                {
                    // Update mode - exclude current record
                    int currentId = Convert.ToInt32(Request.QueryString["id"]);
                    if (IsTestParameterExists(txtTestParameter.Text, ddlDepartment.SelectedItem.Text, requirementId, currentId))
                    {
                        validationErrors.Add("Test Parameter with this name already exists for the selected Department and Requirement combination");
                    }
                }
                else
                {
                    // Insert mode - check all records
                    if (IsTestParameterExists(txtTestParameter.Text, ddlDepartment.SelectedItem.Text, requirementId))
                    {
                        validationErrors.Add("Test Parameter with this name already exists for the selected Department and Requirement combination");
                    }
                }
            }
            if (!string.IsNullOrEmpty(txtSpecification.Text) && !string.IsNullOrEmpty(txtSpecificationValue.Text))
            {
                if (!ValidateSpecification(txtSpecificationValue.Text, txtSpecification.Text))
                {
                    validationErrors.Add("Specification Value does not meet the specification criteria. Please check your input.");
                }
            }
            // Validate numeric fields if they're not empty
            if (!string.IsNullOrEmpty(txtPosition.Text))
            {
                int position;
                if (!int.TryParse(txtPosition.Text, out position))
                    validationErrors.Add("Position must be a valid number");
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

            // If there are validation errors, show them and return
            if (validationErrors.Count > 0)
            {
                string errorMessage = string.Join("<br>", validationErrors);
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                    $"alert('Please correct the following errors:\\n{errorMessage.Replace("<br>", "\\n")}');", true);
                return;
            }

            // Check if we're in update mode or insert mode
            if (Request.QueryString["id"] != null)
            {
                // Update existing record
                UpdateTestParameter(Convert.ToInt32(Request.QueryString["id"]));
            }
            else
            {
                // Insert new record
                InsertTestParameter();
            }
        }
        private void UpdateTestParameter(int testParameterId)
        {
            // Create connection and command
            string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
            string query = @"UPDATE TestParameter SET 
          TestParameter = @TestParameter,
          uomid = @uomid,
          Department = @Department,
          Protocol = @Protocol,
          specification = @specification,
          Position = @Position,
          LOD = @LOD,
          LOQ = @LOQ,
          UOMName = @UOMName,
          LOR = @LOR,
          CASNo = @CASNo,
          MDL = @MDL,
          RequirementID = @RequirementID,
          Clause = @Clause,
          SpecificationValue = @SpecificationValue,
          departmentID =@departmentID,
          NABLNonNABL=@NABLNonNABL
        WHERE TestParameterId = @TestParameterId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        // Add parameters
                        cmd.Parameters.AddWithValue("@TestParameterId", testParameterId);
                        cmd.Parameters.AddWithValue("@TestParameter", txtTestParameter.Text.Trim());
                        cmd.Parameters.AddWithValue("@uomid", ddlUOM.SelectedValue);
                        cmd.Parameters.AddWithValue("@Department", ddlDepartment.SelectedItem.Text);
                        cmd.Parameters.AddWithValue("@Protocol", string.IsNullOrEmpty(ddlTestMethod.SelectedValue) ? DBNull.Value : (object)ddlTestMethod.SelectedItem.Text);
                        cmd.Parameters.AddWithValue("@specification", string.IsNullOrEmpty(txtSpecification.Text) ? DBNull.Value : (object)txtSpecification.Text.Trim());
                        cmd.Parameters.AddWithValue("@Position", string.IsNullOrEmpty(txtPosition.Text) ? DBNull.Value : (object)Convert.ToInt32(txtPosition.Text));
                        cmd.Parameters.AddWithValue("@LOD", string.IsNullOrEmpty(txtLOD.Text) ? DBNull.Value : (object)Convert.ToDecimal(txtLOD.Text));
                        cmd.Parameters.AddWithValue("@LOQ", string.IsNullOrEmpty(txtLOQ.Text) ? DBNull.Value : (object)Convert.ToDecimal(txtLOQ.Text));
                        cmd.Parameters.AddWithValue("@UOMName", string.IsNullOrEmpty(ddlUOM.SelectedItem.Text) ? DBNull.Value : (object)ddlUOM.SelectedItem.Text);
                        cmd.Parameters.AddWithValue("@LOR", string.IsNullOrEmpty(txtLOR.Text) ? DBNull.Value : (object)Convert.ToDecimal(txtLOR.Text));
                        cmd.Parameters.AddWithValue("@CASNo", string.IsNullOrEmpty(txtCASNo.Text) ? DBNull.Value : (object)txtCASNo.Text.Trim());
                        cmd.Parameters.AddWithValue("@MDL", string.IsNullOrEmpty(txtMDL.Text) ? DBNull.Value : (object)Convert.ToDecimal(txtMDL.Text));
                        cmd.Parameters.AddWithValue("@RequirementID", string.IsNullOrEmpty(ddlrequirement.SelectedValue) ? DBNull.Value : (object)ddlrequirement.SelectedValue);
                        cmd.Parameters.AddWithValue("@Clause", string.IsNullOrEmpty(txtClause.Text) ? DBNull.Value : (object)txtClause.Text.Trim());
                        cmd.Parameters.AddWithValue("@SpecificationValue", string.IsNullOrEmpty(txtSpecificationValue.Text) ? DBNull.Value : (object)txtSpecificationValue.Text.Trim());
                        cmd.Parameters.AddWithValue("@NABLNonNABL", string.IsNullOrEmpty(radNABL.SelectedValue) ? DBNull.Value : (object)radNABL.SelectedValue);
                        cmd.Parameters.AddWithValue("@departmentID", ddlDepartment.SelectedValue);
                        conn.Open();
                        cmd.ExecuteNonQuery();

                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Test Parameter updated successfully');", true);
                        Response.Redirect("TestParameterShow.aspx");

                    }
                    catch (Exception ex)
                    {
                        // Handle exception
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Error updating test parameter: " + ex.Message + "');", true);
                    }
                }
            }
        }
        private void InsertTestParameter()
        {
            // Create connection and command
            string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
            string query = @"INSERT INTO TestParameter (
          TestParameter, uomid, OrganizationID, 
          Department, Protocol, specification, Position, LOD, LOQ, isRequirement, UOMName, 
          LOR, CASNo, MDL, RequirementID, 
          Clause, isActive, SpecificationValue
          ) VALUES (
          @TestParameter, @uomid, @OrganizationID,
          @Department, @Protocol, @specification, @Position, @LOD, @LOQ, @isRequirement, @UOMName, 
          @LOR, @CASNo, @MDL, @RequirementID, 
          @Clause, @isActive, @SpecificationValue
          )";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        // Add parameters
                        cmd.Parameters.AddWithValue("@TestParameter", txtTestParameter.Text.Trim());
                        cmd.Parameters.AddWithValue("@uomid", ddlUOM.SelectedValue);
                        cmd.Parameters.AddWithValue("@OrganizationID", 1);
                        cmd.Parameters.AddWithValue("@Department", ddlDepartment.SelectedItem.Text);
                        cmd.Parameters.AddWithValue("@specification", string.IsNullOrEmpty(txtSpecification.Text) ? DBNull.Value : (object)txtSpecification.Text.Trim());
                        cmd.Parameters.AddWithValue("@Position", string.IsNullOrEmpty(txtPosition.Text) ? DBNull.Value : (object)Convert.ToInt32(txtPosition.Text));
                        cmd.Parameters.AddWithValue("@LOD", string.IsNullOrEmpty(txtLOD.Text) ? DBNull.Value : (object)Convert.ToDecimal(txtLOD.Text));
                        cmd.Parameters.AddWithValue("@LOQ", string.IsNullOrEmpty(txtLOQ.Text) ? DBNull.Value : (object)Convert.ToDecimal(txtLOQ.Text));
                        cmd.Parameters.AddWithValue("@isRequirement", 0);
                        cmd.Parameters.AddWithValue("@UOMName", string.IsNullOrEmpty(ddlUOM.SelectedItem.Text) ? DBNull.Value : (object)ddlUOM.SelectedItem.Text);
                        cmd.Parameters.AddWithValue("@LOR", string.IsNullOrEmpty(txtLOR.Text) ? DBNull.Value : (object)Convert.ToDecimal(txtLOR.Text));
                        cmd.Parameters.AddWithValue("@CASNo", string.IsNullOrEmpty(txtCASNo.Text) ? DBNull.Value : (object)txtCASNo.Text.Trim());
                        cmd.Parameters.AddWithValue("@MDL", string.IsNullOrEmpty(txtMDL.Text) ? DBNull.Value : (object)Convert.ToDecimal(txtMDL.Text));                  
                        cmd.Parameters.AddWithValue("@RequirementID", string.IsNullOrEmpty(ddlrequirement.SelectedValue) ? DBNull.Value : (object)ddlrequirement.SelectedValue);
                        cmd.Parameters.AddWithValue("@Clause", string.IsNullOrEmpty(txtClause.Text) ? DBNull.Value : (object)txtClause.Text.Trim());
                        cmd.Parameters.AddWithValue("@Protocol", string.IsNullOrEmpty(ddlTestMethod.SelectedValue) ? DBNull.Value : (object)ddlTestMethod.SelectedItem.Text);
                        cmd.Parameters.AddWithValue("@isActive", 1);
                        cmd.Parameters.AddWithValue("@SpecificationValue", string.IsNullOrEmpty(txtSpecificationValue.Text) ? DBNull.Value : (object)txtSpecificationValue.Text.Trim());

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        // Clear form after successful submission
                        ClearForm();

                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Test Parameter added successfully');", true);
                        Response.Redirect("TestParameterShow.aspx", false);
                    }
                    catch (Exception ex)
                    {
                        // Handle exception
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Error adding test parameter: " + ex.Message + "');", true);
                    }
                }
            }
        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
        private void ClearForm()
        {
            txtTestParameter.Text = string.Empty;
            ddlDepartment.SelectedIndex = 0;
            ddlUOM.SelectedIndex = 0;
            txtCASNo.Text = string.Empty;
            ddlTestMethod.SelectedIndex = 0;
            txtSpecification.Text = string.Empty;
            txtSpecificationValue.Text = string.Empty;
            txtLOD.Text = string.Empty;
            txtLOQ.Text = string.Empty;
            txtLOR.Text = string.Empty;
            txtMDL.Text = string.Empty;
            txtPosition.Text = string.Empty;
            txtClause.Text = string.Empty;
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("TestParameterShow.aspx");
        }
    }
}