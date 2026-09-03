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
    public partial class TestSubParameterEdit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDepartments();
                LoadUOMs();
                LoadTestMethods();
                LoadRequirements();

                // Check if we have an ID in the query string
                if (Request.QueryString["id"] != null)
                {
                    int testSubParameterId;
                    if (int.TryParse(Request.QueryString["id"], out testSubParameterId))
                    {
                        // Load the test sub-parameter for editing
                        LoadTestSubParameterData(testSubParameterId);
                        string departmentid = ddlDepartment.SelectedValue;
                        ShowDepartmentFields(departmentid);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Invalid Test Sub-Parameter ID');", true);
                        Response.Redirect("TestSubParameterList.aspx");
                    }
                }
                else
                {
                    // No ID provided, redirect to list page
                    Response.Redirect("TestSubParameterList.aspx");
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
                        //ddlUOM.Enabled = false;
                    }
                    catch (Exception ex)
                    {
                        // Handle exception
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Error loading UOMs: " + ex.Message + "');", true);
                    }
                }
            }
        }
        private void BindTestParameters(string departmentID)
        {
            try
            {
                string query = "SELECT TestParameterID, TestParameter FROM TestParameter WHERE departmentID = @DepartmentID ORDER BY TestParameter";

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@DepartmentID", departmentID);
                        con.Open();

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                ddltp.Items.Add(new ListItem(
                                    rdr["TestParameter"].ToString(),
                                    rdr["TestParameterID"].ToString()
                                ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle error (log it, show message, etc.)
                // Example:
                ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Error loading test parameters: {ex.Message}');", true);
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
                        ddlTestMethod.Enabled = false;
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
                        ddlrequirement.Enabled = false; // make dropdown readonly
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

            if (ddlDepartment.SelectedItem != null && !string.IsNullOrEmpty(ddlDepartment.SelectedValue))
            {
                string selectedDepartment = ddlDepartment.SelectedValue;
                string departmentID = ddlDepartment.SelectedValue; // Get the department ID from the dropdown

                // Bind test parameters for the selected department
                BindTestParameters(departmentID);

                ShowDepartmentFields(selectedDepartment);
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

                if (!string.IsNullOrEmpty(requirementId))
                {
                    ListItem reqItem = ddlrequirement.Items.FindByValue(requirementId);
                    if (reqItem != null)
                    {
                        ddlrequirement.ClearSelection();
                        reqItem.Selected = true;
                    }
                    ddlrequirement.Enabled = false;
                }
                else
                {
                    ddlrequirement.SelectedIndex = 0;
                    ddlrequirement.Enabled = true;
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
                ddlrequirement.SelectedIndex = 0;
                ddlrequirement.Enabled = true;
                ddlUOM.SelectedIndex = 0;
                ddlUOM.Enabled = true;
                ddlTestMethod.SelectedIndex = 0;
                ddlTestMethod.Enabled = true;
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
        private void LoadTestSubParameterData(int testSubParameterId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
            string query = @"SELECT tsp.*, tp.Department, tp.TestParameter ,tp.departmentID
                    FROM TestSubParameter tsp
                    INNER JOIN TestParameter tp ON tsp.TestParameterID = tp.TestParameterID
                    WHERE tsp.TestSubParameterId = @TestSubParameterId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        cmd.Parameters.AddWithValue("@TestSubParameterId", testSubParameterId);
                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Populate basic fields
                                txtTestsubParameter.Text = reader["TestSubParameter"].ToString();

                                // Set Department dropdown
                                string departmentName = reader["Department"].ToString();
                                string departmentID = reader["departmentID"].ToString();
                                if (!string.IsNullOrEmpty(departmentID))
                                {
                                    ddlDepartment.SelectedValue = departmentID;
                                }
                                ddlDepartment_SelectedIndexChanged(ddlDepartment, EventArgs.Empty);

                                // Load and set Test Parameters for selected department
                                BindTestParameters(departmentID);

                                // Set Test Parameter dropdown
                                if (reader["TestParameterID"] != DBNull.Value)
                                {
                                    string testParameterId = reader["TestParameterID"].ToString();
                                    // If the item doesn't exist in the dropdown, add it
                                    if (ddltp.Items.FindByValue(testParameterId) == null)
                                    {
                                        ddltp.Items.Add(new ListItem(reader["TestParameter"].ToString(), testParameterId));
                                    }
                                    ddltp.SelectedValue = testParameterId;
                                }

                                // UOM dropdown
                                if (reader["uomid"] != DBNull.Value)
                                {
                                    ddlUOM.SelectedValue = reader["uomid"].ToString();
                                }

                                // Test Method dropdown - match by text since we store TestMethod text in Protocol
                                if (reader["protocol"] != DBNull.Value)
                                {
                                    string protocol = reader["protocol"].ToString();
                                    SetDropDownByText(ddlTestMethod, protocol);
                                }

                                // Requirement dropdown
                                if (reader["RequirementId"] != DBNull.Value)
                                {
                                    ddlrequirement.SelectedValue = reader["RequirementId"].ToString();
                                }

                                // Other text fields
                                txtSpecification.Text = reader["Specification"] != DBNull.Value ? reader["Specification"].ToString() : string.Empty;
                                txtSpecificationValue.Text = reader["SpecificationValue"] != DBNull.Value ? reader["SpecificationValue"].ToString() : string.Empty;

                                if (reader["Position"] != DBNull.Value)
                                {
                                    txtPosition.Text = reader["Position"].ToString();
                                }

                                // Department-specific fields
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

                                if (reader["MDL"] != DBNull.Value)
                                {
                                    txtMDL.Text = reader["MDL"].ToString();
                                }

                                txtCASNo.Text = reader["CASNo"] != DBNull.Value ? reader["CASNo"].ToString() : string.Empty;

                                // Show the appropriate department sections based on department selection
                                ShowDepartmentFields(departmentID);

                                // Store the Test Sub-Parameter ID in ViewState for update operation
                                ViewState["TestSubParameterId"] = testSubParameterId;
                            }
                            else
                            {
                                // Record not found
                                ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Test Sub-Parameter not found');", true);
                                Response.Redirect("TestSubParameterList.aspx");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle exception
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Error loading test sub-parameter: " + ex.Message.Replace("'", "\\'") + "');", true);
                    }
                }
            }
        }

        // Helper method to set dropdown by text value
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
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            // Validate required fields
            // Create a list to track validation errors
            List<string> validationErrors = new List<string>();

            // Validate required fields
            if (string.IsNullOrEmpty(txtTestsubParameter.Text))
                validationErrors.Add("Test Parameter is required");

            if (string.IsNullOrEmpty(ddlDepartment.SelectedValue))
                validationErrors.Add("Department is required");

            if (string.IsNullOrEmpty(ddlUOM.SelectedValue))
                validationErrors.Add("UOM is required");

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
            // Check for duplicate Test Sub-Parameter name (exclude current record)
            if (!string.IsNullOrEmpty(txtTestsubParameter.Text) && !string.IsNullOrEmpty(ddltp.SelectedValue))
            {
                int currentId = Convert.ToInt32(Request.QueryString["id"]);
                string requirementId = string.IsNullOrEmpty(ddlrequirement.SelectedValue) ? null : ddlrequirement.SelectedValue;

                if (IsTestSubParameterExists(txtTestsubParameter.Text, ddltp.SelectedValue, requirementId, currentId))
                {
                    validationErrors.Add("Test Sub-Parameter with this name already exists for the selected Test Parameter and Requirement combination");
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
                    $"alert('Please correct the following errors:\\n{errorMessage.Replace("<br>", "\\n")}');", true);
                return;
            }

            // Check if we're in update mode
            if (Request.QueryString["id"] != null)
            {
                // Update existing record
                UpdateTestSubParameter(Convert.ToInt32(Request.QueryString["id"]));
            }
            else
            {
                // This shouldn't happen as we redirect in Page_Load if no ID
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Invalid operation mode');", true);
                Response.Redirect("TestSubParameterList.aspx");
            }
        }
        private bool IsTestSubParameterExists(string testSubParameter, string testParameterId, string requirementId, int? excludeId = null)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
            string query;

            // Build the base query with RequirementID logic
            string baseCondition;
            if (string.IsNullOrEmpty(requirementId))
            {
                // Check for NULL or empty RequirementID
                baseCondition = @"TestSubParameter = @TestSubParameter 
                         AND TestParameterID = @TestParameterID 
                         AND (RequirementID IS NULL OR RequirementID = '')";
            }
            else
            {
                // Exact match for RequirementID
                baseCondition = @"TestSubParameter = @TestSubParameter 
                         AND TestParameterID = @TestParameterID 
                         AND RequirementID = @RequirementID";
            }

            if (excludeId.HasValue)
            {
                // For update operations - exclude the current record being updated
                query = $"SELECT COUNT(*) FROM TestSubParameter WHERE {baseCondition} AND TestSubParameterId != @ExcludeId";
            }
            else
            {
                // For insert operations - check all records
                query = $"SELECT COUNT(*) FROM TestSubParameter WHERE {baseCondition}";
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
                            "alert('Error checking Test Sub-Parameter uniqueness: " + ex.Message.Replace("'", "\\'") + "');", true);
                        return false;
                    }
                }
            }
        }
        private void UpdateTestSubParameter(int testSubParameterId)
        {
            // Create connection and command
            string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
            string query = @"UPDATE TestSubParameter SET 
                TestSubParameter = @TestSubParameter,
                TestParameterID = @TestParameterID,
                Specification = @Specification,
                uomid = @uomid,
                protocol = @protocol,
                Position = @Position,
                LOD = @LOD,
                LOQ = @LOQ,
                UOMName = @UOMName,
                LOR = @LOR,
                CASNo = @CASNo,
                MDL = @MDL,
                SpecificationValue = @SpecificationValue,
                RequirementId = @RequirementId
                WHERE TestSubParameterId = @TestSubParameterId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        // Add parameters
                        cmd.Parameters.AddWithValue("@TestSubParameterId", testSubParameterId);
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

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        // Update the hassubparameter flag in the TestParameter table
                        string updateParamQuery = @"UPDATE TestParameter 
                                 SET hassubparameter = 1 
                                 WHERE TestParameterID = @TestParameterID";

                        using (SqlCommand updateCmd = new SqlCommand(updateParamQuery, conn))
                        {
                            updateCmd.Parameters.AddWithValue("@TestParameterID", ddltp.SelectedValue);
                            updateCmd.ExecuteNonQuery();
                        }

                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Test Sub-Parameter updated successfully');", true);
                        Response.Redirect("TestSubParameterShow.aspx");
                    }
                    catch (Exception ex)
                    {
                        // Handle exception
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Error updating test sub-parameter: " + ex.Message.Replace("'", "\\'") + "');", true);
                    }
                }
            }
        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
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
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("TestSubParameterShow.aspx");
        }
    }
}