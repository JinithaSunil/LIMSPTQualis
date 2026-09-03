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
    public partial class AddTestParameters : System.Web.UI.Page
    {
        Parameters param = new Parameters();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                pnlAddUOM.Visible = false;
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

        // Add this method to your code-behind class
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
        private bool IsTestParameterRequirementExists(string testParameter, string requirementId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;

            string query;

            // If no requirement is selected, check for records with same TestParameter and NULL RequirementID
            if (string.IsNullOrEmpty(requirementId))
            {
                query = @"SELECT COUNT(*) FROM TestParameter 
                  WHERE TestParameter = @TestParameter 
                  AND (RequirementID IS NULL OR RequirementID = '')";
            }
            else
            {
                query = @"SELECT COUNT(*) FROM TestParameter 
                  WHERE TestParameter = @TestParameter 
                  AND RequirementID = @RequirementID";
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        cmd.Parameters.AddWithValue("@TestParameter", testParameter.Trim());

                        if (!string.IsNullOrEmpty(requirementId))
                        {
                            cmd.Parameters.AddWithValue("@RequirementID", requirementId);
                        }

                        conn.Open();

                        int count = (int)cmd.ExecuteScalar();
                        return count > 0; // Returns true if combination exists, false otherwise
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or handle it as needed
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                            "alert('Error checking test parameter and requirement combination: " + ex.Message + "');", true);
                        return false;
                    }
                }
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Create a list to track validation errors
            List<string> validationErrors = new List<string>();

            // Validate required fields
            if (string.IsNullOrEmpty(txtTestParameter.Text))
                validationErrors.Add("Test Parameter is required");

            if (string.IsNullOrEmpty(ddlDepartment.SelectedValue))
                validationErrors.Add("Department is required");

            if (string.IsNullOrEmpty(ddlUOM.SelectedValue))
                validationErrors.Add("UOM is required");

            // NEW: Check if test parameter and requirement combination already exists
            if (!string.IsNullOrEmpty(txtTestParameter.Text))
            {
                if (IsTestParameterRequirementExists(txtTestParameter.Text, ddlrequirement.SelectedValue))
                {
                    validationErrors.Add("Test Parameter with the same Requirement already exists. Please enter a different test parameter or select a different requirement.");
                }
            }

            // Validate numeric fields if they're not empty
            if (!string.IsNullOrEmpty(txtPosition.Text))
            {
                int position;
                if (!int.TryParse(txtPosition.Text, out position))
                    validationErrors.Add("Position must be a valid number");
            }

            // Validate specification against specification value
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

            // If there are validation errors, show them and return
            if (validationErrors.Count > 0)
            {
                string errorMessage = string.Join("<br>", validationErrors);
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                    $"alert('{errorMessage.Replace("<br>", "\\n")}');", true);
                return;
            }

            // Rest of your existing database insertion code remains exactly the same...
            string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
            string query = @"INSERT INTO TestParameter (
            TestParameter, uomid, OrganizationID,  
            Department, Protocol, specification, Position, LOD, LOQ, isRequirement,UOMName, 
            LOR, CASNo, MDL, RequirementID, 
            Clause, isActive, SpecificationValue,departmentID
            ) VALUES (
            @TestParameter, @uomid, @OrganizationID,  
            @Department, @Protocol, @specification, @Position, @LOD, @LOQ, @isRequirement,@UOMName, 
            @LOR, @CASNo, @MDL, @RequirementID, 
            @Clause, @isActive, @SpecificationValue,@departmentID
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
                        cmd.Parameters.AddWithValue("@departmentID", ddlDepartment.SelectedValue);
                        conn.Open();
                        cmd.ExecuteNonQuery();

                        // Clear form after successful submission
                        ClearForm();

                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Test Parameter added successfully');", true);
                        Response.Redirect("TestParameterShow.aspx");
                    }
                    catch (Exception ex)
                    {
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

        //Add New UOM
        protected void btnShowAddUOM_Click(object sender, EventArgs e)
        {
            // Show the Add UOM panel
            pnlAddUOM.Visible = true;

            // Clear any previous messages
            lblUOMMessage.Visible = false;
            lblUOMMessage.Text = "";

            // Clear the input fields
            txtNewUOMName.Text = "";
            txtNewUOMCode.Text = "";

            // Focus on the first input field
            txtNewUOMName.Focus();
        }
        protected void btnCancelUOM_Click(object sender, EventArgs e)
        {
            // Hide the Add UOM panel
            pnlAddUOM.Visible = false;

            // Clear the input fields
            txtNewUOMName.Text = "";
            txtNewUOMCode.Text = "";

            // Clear any messages
            lblUOMMessage.Visible = false;
            lblUOMMessage.Text = "";
        }
        protected void btnSaveUOM_Click(object sender, EventArgs e)
        {
            // Clear any previous messages
            lblUOMMessage.Visible = false;
            lblUOMMessage.Text = "";

            try
            {
                // Validate required fields
                if (string.IsNullOrEmpty(txtNewUOMName.Text.Trim()))
                {
                    ShowUOMMessage("UOM Name is required.", false);
                    return;
                }

                string uomName = txtNewUOMName.Text.Trim();
                string uomCode = txtNewUOMCode.Text.Trim();

                // Check if UOM Name already exists
                if (IsUOMNameExists(uomName))
                {
                    ShowUOMMessage("UOM Name already exists. Please enter a different name.", false);
                    return;
                }

                // Check if UOM Code already exists (if provided)
                if (!string.IsNullOrEmpty(uomCode) && IsUOMCodeExists(uomCode))
                {
                    ShowUOMMessage("UOM Code already exists. Please enter a different code.", false);
                    return;
                }

                // Insert new UOM into database
                string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
                string query = @"INSERT INTO UOM (UOMName, UOMCode, createdDate, OrganizationID) 
                        VALUES (@UOMName, @UOMCode, @createdDate, @OrganizationID)";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UOMName", uomName);
                        cmd.Parameters.AddWithValue("@UOMCode", string.IsNullOrEmpty(uomCode) ? DBNull.Value : (object)uomCode);
                        cmd.Parameters.AddWithValue("@createdDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@OrganizationID", 1); 

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                // Show success message
                ShowUOMMessage("UOM added successfully!", true);

                // Clear the form fields
                txtNewUOMName.Text = "";
                txtNewUOMCode.Text = "";

                // Reload the UOM dropdown to include the new UOM
                LoadUOMs();

                // Hide the panel after successful save
                pnlAddUOM.Visible = false;

                // Show success alert
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                    "alert('UOM added successfully!');", true);
            }
            catch (Exception ex)
            {
                ShowUOMMessage("Error adding UOM: " + ex.Message, false);
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                    "alert('Error adding UOM: " + ex.Message + "');", true);
            }
        }
        private bool IsUOMNameExists(string uomName)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
            string query = "SELECT COUNT(*) FROM UOM WHERE LOWER(UOMName) = LOWER(@UOMName)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UOMName", uomName);

                    try
                    {
                        conn.Open();
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or handle it as needed
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                            "alert('Error checking UOM name: " + ex.Message + "');", true);
                        return false;
                    }
                }
            }
        }
        private bool IsUOMCodeExists(string uomCode)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
            string query = "SELECT COUNT(*) FROM UOM WHERE LOWER(UOMCode) = LOWER(@UOMCode)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UOMCode", uomCode);

                    try
                    {
                        conn.Open();
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or handle it as needed
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                            "alert('Error checking UOM code: " + ex.Message + "');", true);
                        return false;
                    }
                }
            }
        }
        private void ShowUOMMessage(string message, bool isSuccess)
        {
            lblUOMMessage.Text = message;
            lblUOMMessage.CssClass = isSuccess ? "success-message" : "error-message";
            lblUOMMessage.Visible = true;
        }

        //Add New Test Method
        protected void btnShowAddTestMethod_Click(object sender, EventArgs e)
        {
            // Show the Add Test Method panel
            pnlAddTestMethod.Visible = true;

            // Clear any previous messages
            lblTestMethodMessage.Visible = false;
            lblTestMethodMessage.Text = "";

            // Clear the input field
            txtNewTestMethod.Text = "";

            // Focus on the input field
            txtNewTestMethod.Focus();
        }
        protected void btnCancelTestMethod_Click(object sender, EventArgs e)
        {
            // Hide the Add Test Method panel
            pnlAddTestMethod.Visible = false;

            // Clear the input field
            txtNewTestMethod.Text = "";

            // Clear any messages
            lblTestMethodMessage.Visible = false;
            lblTestMethodMessage.Text = "";
        }
        protected void btnSaveTestMethod_Click(object sender, EventArgs e)
        {
            // Clear any previous messages
            lblTestMethodMessage.Visible = false;
            lblTestMethodMessage.Text = "";

            try
            {
                // Validate required fields
                if (string.IsNullOrEmpty(txtNewTestMethod.Text.Trim()))
                {
                    ShowTestMethodMessage("Test Method is required.", false);
                    return;
                }

                string testMethod = txtNewTestMethod.Text.Trim();

                // Check if Test Method already exists
                if (IsTestMethodExists(testMethod))
                {
                    ShowTestMethodMessage("Test Method already exists. Please enter a different method.", false);
                    return;
                }

                // Insert new Test Method into database
                string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
                string query = @"INSERT INTO TestMethod (TestMethod, OrganizationID) 
                        VALUES (@TestMethod, @OrganizationID)";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TestMethod", testMethod);
                        cmd.Parameters.AddWithValue("@OrganizationID", 1); // Using 1 as default, same as in your main form

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                // Show success message
                ShowTestMethodMessage("Test Method added successfully!", true);

                // Clear the form field
                txtNewTestMethod.Text = "";

                // Reload the Test Method dropdown to include the new Test Method
                LoadTestMethods();

                // Hide the panel after successful save
                pnlAddTestMethod.Visible = false;

                // Show success alert
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                    "alert('Test Method added successfully!');", true);
            }
            catch (Exception ex)
            {
                ShowTestMethodMessage("Error adding Test Method: " + ex.Message, false);
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                    "alert('Error adding Test Method: " + ex.Message + "');", true);
            }
        }
        private bool IsTestMethodExists(string testMethod)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
            string query = "SELECT COUNT(*) FROM TestMethod WHERE LOWER(TestMethod) = LOWER(@TestMethod)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TestMethod", testMethod);

                    try
                    {
                        conn.Open();
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or handle it as needed
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                            "alert('Error checking Test Method: " + ex.Message + "');", true);
                        return false;
                    }
                }
            }
        }
        private void ShowTestMethodMessage(string message, bool isSuccess)
        {
            lblTestMethodMessage.Text = message;
            lblTestMethodMessage.CssClass = isSuccess ? "success-message" : "error-message";
            lblTestMethodMessage.Visible = true;
        }

        //Add New Requirement
        protected void btnShowAddRequirement_Click(object sender, EventArgs e)
        {
            // Show the Add Requirement panel
            pnlAddRequirement.Visible = true;

            // Clear any previous messages
            lblRequirementMessage.Visible = false;
            lblRequirementMessage.Text = "";

            // Clear the input field
            txtNewRequirement.Text = "";

            // Load departments for the requirement department dropdown
            LoadRequirementDepartments();

            // Reset department selection
            ddlRequirementDepartment.SelectedIndex = 0;

            // Focus on the input field
            txtNewRequirement.Focus();
        }
        protected void btnCancelRequirement_Click(object sender, EventArgs e)
        {
            // Hide the Add Requirement panel
            pnlAddRequirement.Visible = false;

            // Clear the input field
            txtNewRequirement.Text = "";

            // Reset department selection
            if (ddlRequirementDepartment.Items.Count > 0)
                ddlRequirementDepartment.SelectedIndex = 0;

            // Clear any messages
            lblRequirementMessage.Visible = false;
            lblRequirementMessage.Text = "";
        }
        protected void btnSaveRequirement_Click(object sender, EventArgs e)
        {
            // Clear any previous messages
            lblRequirementMessage.Visible = false;
            lblRequirementMessage.Text = "";

            try
            {
                // Validate required fields
                if (string.IsNullOrEmpty(txtNewRequirement.Text.Trim()))
                {
                    ShowRequirementMessage("Requirement is required.", false);
                    return;
                }

                if (string.IsNullOrEmpty(ddlRequirementDepartment.SelectedValue))
                {
                    ShowRequirementMessage("Department is required.", false);
                    return;
                }

                string requirement = txtNewRequirement.Text.Trim();
                string departmentId = ddlRequirementDepartment.SelectedValue;

                // Check if Requirement already exists for the selected department
                if (IsRequirementExists(requirement, departmentId))
                {
                    ShowRequirementMessage("Requirement already exists for this department. Please enter a different requirement.", false);
                    return;
                }

                // Insert new Requirement into database
                string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
                string query = @"INSERT INTO Requirement (Requirement, DepartmentID, CreatedDate) 
                VALUES (@Requirement, @DepartmentID, @CreatedDate)";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Requirement", requirement);
                        cmd.Parameters.AddWithValue("@DepartmentID", Convert.ToInt32(departmentId));
                        cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                // Show success message
                ShowRequirementMessage("Requirement added successfully!", true);

                // Clear the form fields
                txtNewRequirement.Text = "";
                ddlRequirementDepartment.SelectedIndex = 0;

                // Reload the Requirements dropdown to include the new Requirement
                LoadRequirements();

                // Hide the panel after successful save
                pnlAddRequirement.Visible = false;

                // Show success alert
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                    "alert('Requirement added successfully!');", true);
            }
            catch (Exception ex)
            {
                ShowRequirementMessage("Error adding Requirement: " + ex.Message, false);
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                    "alert('Error adding Requirement: " + ex.Message + "');", true);
            }
        }
        private void LoadRequirementDepartments()
        {
            try
            {
                DataTable dt = param.GetActiveDepartments();
                ddlRequirementDepartment.DataSource = dt;
                ddlRequirementDepartment.DataTextField = "DepartmentName";
                ddlRequirementDepartment.DataValueField = "DepartmentID";
                ddlRequirementDepartment.DataBind();
                ddlRequirementDepartment.Items.Insert(0, new ListItem("--Select Department--", ""));
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                    "alert('Error loading departments for requirement: " + ex.Message + "');", true);
            }
        }
        private bool IsRequirementExists(string requirement, string departmentId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
            string query = "SELECT COUNT(*) FROM Requirement WHERE LOWER(Requirement) = LOWER(@Requirement) AND DepartmentID = @DepartmentID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Requirement", requirement);
                    cmd.Parameters.AddWithValue("@DepartmentID", Convert.ToInt32(departmentId));

                    try
                    {
                        conn.Open();
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or handle it as needed
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                            "alert('Error checking Requirement: " + ex.Message + "');", true);
                        return false;
                    }
                }
            }
        }
        private void ShowRequirementMessage(string message, bool isSuccess)
        {
            lblRequirementMessage.Text = message;
            lblRequirementMessage.CssClass = isSuccess ? "success-message" : "error-message";
            lblRequirementMessage.Visible = true;
        }
    }
}