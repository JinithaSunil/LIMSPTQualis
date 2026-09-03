using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{ 
public partial class TestSubSubParameterEdit : System.Web.UI.Page
{
    string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
      
        if (!IsPostBack)
        {
            LoadDepartments();
            LoadUOMs();
            LoadTestMethods();

            // Check if we have an ID in the query string
            if (Request.QueryString["id"] != null)
            {
                int testSubSubParameterId;
                if (int.TryParse(Request.QueryString["id"], out testSubSubParameterId))
                {
                    // Load the test sub-sub-parameter for editing
                    LoadTestSubSubParameterData(testSubSubParameterId);
                        string departmentid = ddlDepartment.SelectedValue;
                        ShowDepartmentFields(departmentid);
                    }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Invalid Test Sub-Sub-Parameter ID');", true);
                    Response.Redirect("TestSubSubParameterShow.aspx");
                }
            }
            else
            {
                // No ID provided, redirect to list page
                Response.Redirect("TestSubSubParameterShow.aspx");
            }
        }
    }

    private void LoadDepartments()
    {
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
                    ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Error loading departments: " + ex.Message.Replace("'", "\\'") + "');", true);
                }
            }
        }
    }
    private void LoadUOMs()
    {
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
                    ddlUOM.Enabled = false;
                }
                catch (Exception ex)
                {
                    // Handle exception
                    ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Error loading UOMs: " + ex.Message.Replace("'", "\\'") + "');", true);
                }
            }
        }
    }
    private void LoadTestSubParameters(int departmentId)
    {
        try
        {
            string query = @"SELECT tsp.TestSubParameterID, tsp.TestSubParameter 
                            FROM TestSubParameter tsp 
                            INNER JOIN TestParameter tp ON tsp.TestParameterID = tp.TestParameterID
                            INNER JOIN Department d ON tp.departmentID = d.DepartmentID
                            WHERE d.DepartmentID = @DepartmentID 
                            ORDER BY tsp.TestSubParameter";  

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DepartmentID", departmentId);
                    conn.Open();

                    // Clear existing items first
                    ddltsp.Items.Clear();
                    ddltsp.Items.Add(new ListItem("--Select Test Sub-Parameter--", ""));

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ddltsp.DataSource = dt;
                    ddltsp.DataTextField = "TestSubParameter";
                    ddltsp.DataValueField = "TestSubParameterID";
                    ddltsp.DataBind();
                    ddltsp.Items.Insert(0, new ListItem("--Select Test Sub-Parameter--", ""));
                }
            }
        }
        catch (Exception ex)
        {
            // Handle error
            ScriptManager.RegisterStartupScript(this, GetType(), "showalert", $"alert('Error loading test sub-parameters: {ex.Message.Replace("'", "\\'")}');", true);
        }
    }
    private void LoadTestMethods()
    {
        string query = "SELECT TestMethodID, TestMethod FROM TestMethod";

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
                    ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Error loading test methods: " + ex.Message.Replace("'", "\\'") + "');", true);
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
          
            ShowDepartmentFields(selectedDepartment);
               
            // Load test sub parameters for the selected department
            int departmentId = Convert.ToInt32(ddlDepartment.SelectedValue);
            LoadTestSubParameters(departmentId);
        }
        else
        {
            // Clear the test sub parameter dropdown
            ddltsp.Items.Clear();
            ddltsp.Items.Add(new ListItem("--Select Test Sub-Parameter--", ""));
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
        private void LoadTestSubSubParameterData(int testSubSubParameterId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
            string query = @"SELECT tssp.*, tsp.TestSubParameterID,tsp.TestSubParameter, d.DepartmentID, d.DepartmentName, tp.departmentID
            FROM TestSubSubParameter tssp
            INNER JOIN TestSubParameter tsp ON tssp.TestSubParameterID = tsp.TestSubParameterID
            INNER JOIN TestParameter tp ON tsp.TestParameterID = tp.TestParameterID
            INNER JOIN Department d ON tp.departmentID = d.DepartmentID
            WHERE tssp.TestSubSubParameterID = @TestSubSubParameterID";

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                try
                {
                    cmd.Parameters.AddWithValue("@TestSubSubParameterID", testSubSubParameterId);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Populate basic fields
                            txtTestsubsubParameter.Text = reader["TestSubSubParameter"].ToString();

                                // Set Department dropdown
                                string departmentName = reader["DepartmentName"].ToString();
                                string departmentID = reader["DepartmentID"].ToString();
                                if (!string.IsNullOrEmpty(departmentID))
                                {
                                    ddlDepartment.SelectedValue = departmentID;
                                }
                                ddlDepartment_SelectedIndexChanged(ddlDepartment, EventArgs.Empty);

                                // Load test sub parameters for this department
                                LoadTestSubParameters(Convert.ToInt32(departmentID));

                                // Set Test Sub Parameter dropdown
                                if (reader["TestSubParameterID"] != DBNull.Value)
                                {
                                    string testSubParameterId = reader["TestSubParameterID"].ToString();
                                    SetDropDownByValue(ddltsp, testSubParameterId);
                                }

                                // UOM dropdown
                                if (reader["UOMID"] != DBNull.Value)
                                {
                                    ddlUOM.SelectedValue = reader["UOMID"].ToString();
                                    hdnUOMId.Value = reader["UOMID"].ToString();
                                }

                                // Test Method dropdown - match by text since we store TestMethod text in Protocol
                                if (reader["Protocol"] != DBNull.Value)
                                {
                                    string protocol = reader["Protocol"].ToString();
                                    bool foundByValue = false;

                                    foreach (ListItem item in ddlTestMethod.Items)
                                    {
                                        if (item.Value == protocol)
                                        {
                                            ddlTestMethod.SelectedValue = protocol;
                                            foundByValue = true;
                                            break;
                                        }
                                    }

                                    if (!foundByValue)
                                    {
                                        foreach (ListItem item in ddlTestMethod.Items)
                                        {
                                            if (item.Text.Equals(protocol, StringComparison.OrdinalIgnoreCase))
                                            {
                                                ddlTestMethod.SelectedValue = item.Value;
                                                break;
                                            }
                                        }
                                    }
                                }

                                // Other text fields
                                txtSpecification.Text = reader["Specification"] != DBNull.Value ? reader["Specification"].ToString() : string.Empty;
                                txtPosition.Text = reader["Position"] != DBNull.Value ? reader["Position"].ToString() : string.Empty;
                                txtSpecificationValue.Text = reader["SpecificationValue"] != DBNull.Value ? reader["SpecificationValue"].ToString() : string.Empty;

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

                                if (reader["CASNo"] != DBNull.Value)
                                {
                                    txtCASNo.Text = reader["CASNo"].ToString();
                                }

                                // Show the appropriate department sections based on department selection
                                ShowDepartmentFields(departmentID);

                                // Store the Test Sub-Sub-Parameter ID in ViewState for update operation
                                ViewState["TestSubSubParameterId"] = testSubSubParameterId;
                            }
                            else
                            {
                                // Record not found
                                ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Test Sub-Sub-Parameter not found');", true);
                                Response.Redirect("TestSubSubParameterShow.aspx");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle exception
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Error loading test sub-sub-parameter: " + ex.Message.Replace("'", "\\'") + "');", true);
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
        // Helper method to set dropdown by value
        private void SetDropDownByValue(DropDownList dropDown, string value)
    {
        if (string.IsNullOrEmpty(value))
            return;

        ListItem item = dropDown.Items.FindByValue(value);
        if (item != null)
        {
            item.Selected = true;
        }
    }
        private bool IsTestSubSubParameterExists(string testSubSubParameter, string testSubParameterId, int? excludeId = null)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;

            string query;

            if (excludeId.HasValue)
            {
                // For update operations - exclude the current record being updated
                query = @"SELECT COUNT(*) FROM TestSubSubParameter 
                  WHERE TestSubSubParameter = @TestSubSubParameter 
                  AND TestSubParameterID = @TestSubParameterID
                  AND TestSubSubParameterID != @ExcludeId";
            }
            else
            {
                // For insert operations - check all records
                query = @"SELECT COUNT(*) FROM TestSubSubParameter 
                  WHERE TestSubSubParameter = @TestSubSubParameter 
                  AND TestSubParameterID = @TestSubParameterID";
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        cmd.Parameters.AddWithValue("@TestSubSubParameter", testSubSubParameter.Trim());
                        cmd.Parameters.AddWithValue("@TestSubParameterID", testSubParameterId);

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
                            "alert('Error checking Test Sub-Sub-Parameter: " + ex.Message + "');", true);
                        return false;
                    }
                }
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
            if (string.IsNullOrEmpty(txtTestsubsubParameter.Text))
                validationErrors.Add("Test Parameter is required");

            if (string.IsNullOrEmpty(ddlDepartment.SelectedValue))
                validationErrors.Add("Department is required");

            if (string.IsNullOrEmpty(ddlUOM.SelectedValue))
                validationErrors.Add("UOM is required");

            // Get ID from ViewState or QueryString first (before duplicate check)
            int testSubSubParameterId;
            if (ViewState["TestSubSubParameterId"] != null)
            {
                testSubSubParameterId = Convert.ToInt32(ViewState["TestSubSubParameterId"]);
            }
            else if (Request.QueryString["id"] != null)
            {
                testSubSubParameterId = Convert.ToInt32(Request.QueryString["id"]);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Invalid operation mode');", true);
                return;
            }

            // Check for duplicates - EXCLUDE the current record being updated
            if (IsTestSubSubParameterExists(txtTestsubsubParameter.Text, ddltsp.SelectedValue, testSubSubParameterId))
            {
                validationErrors.Add("This Test Sub-Sub-Parameter already exists under the selected Test Sub-Parameter.");
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

            // If there are validation errors, show them and return
            if (validationErrors.Count > 0)
            {
                string errorMessage = string.Join("<br>", validationErrors);
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert",
                    $"alert('{errorMessage.Replace("<br>", "")}');", true);
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;

            // Create connection and command
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Use transaction to ensure the operation completes successfully
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Update the TestSubSubParameter record
                        string updateQuery = @"UPDATE TestSubSubParameter SET 
                    TestSubSubParameter = @TestSubSubParameter,
                    TestSubParameterID = @TestSubParameterID,
                    Specification = @Specification,
                    SpecificationValue = @SpecificationValue,
                    UOMID = @UOMID,
                    UOMName = @UOMName,
                    Position = @Position,
                    LOD = @LOD,
                    LOQ = @LOQ,
                    LOR = @LOR,
                    MDL = @MDL,
                    CASNo = @CASNo,
                    Protocol = @Protocol
                    WHERE TestSubSubParameterID = @TestSubSubParameterID";

                        using (SqlCommand cmd = new SqlCommand(updateQuery, conn, transaction))
                        {
                            // Add parameters
                            cmd.Parameters.AddWithValue("@TestSubSubParameter", txtTestsubsubParameter.Text.Trim());
                            cmd.Parameters.AddWithValue("@TestSubParameterID", ddltsp.SelectedValue);
                            cmd.Parameters.AddWithValue("@UOMID", string.IsNullOrEmpty(ddlUOM.SelectedValue) ? (object)DBNull.Value : (object)ddlUOM.SelectedValue);
                            cmd.Parameters.AddWithValue("@Specification", string.IsNullOrEmpty(txtSpecification.Text) ? DBNull.Value : (object)txtSpecification.Text.Trim());
                            cmd.Parameters.AddWithValue("@SpecificationValue", string.IsNullOrEmpty(txtSpecificationValue.Text) ? DBNull.Value : (object)txtSpecificationValue.Text.Trim());
                            cmd.Parameters.AddWithValue("@Position", string.IsNullOrEmpty(txtPosition.Text) ? DBNull.Value : (object)Convert.ToInt32(txtPosition.Text));

                            if (ddlUOM.SelectedIndex > 0)
                                cmd.Parameters.AddWithValue("@UOMName", ddlUOM.SelectedItem.Text);
                            else
                                cmd.Parameters.AddWithValue("@UOMName", DBNull.Value);

                            if (ddlTestMethod.SelectedIndex > 0)
                                cmd.Parameters.AddWithValue("@Protocol", ddlTestMethod.SelectedItem.Text);
                            else
                                cmd.Parameters.AddWithValue("@Protocol", DBNull.Value);

                            cmd.Parameters.AddWithValue("@TestSubSubParameterID", testSubSubParameterId);

                            // Department-specific parameters
                            string selecteddepartmentID = ddlDepartment.SelectedValue;
                            if(selecteddepartmentID == "5")
                            {
                                cmd.Parameters.AddWithValue("@LOD", string.IsNullOrEmpty(txtLOD.Text) ? DBNull.Value : (object)Convert.ToDecimal(txtLOD.Text));
                                cmd.Parameters.AddWithValue("@LOQ", string.IsNullOrEmpty(txtLOQ.Text) ? DBNull.Value : (object)Convert.ToDecimal(txtLOQ.Text));
                                cmd.Parameters.AddWithValue("@LOR", string.IsNullOrEmpty(txtLOR.Text) ? DBNull.Value : (object)Convert.ToDecimal(txtLOR.Text));
                                // Add null values for Chemical fields
                                cmd.Parameters.AddWithValue("@MDL", DBNull.Value);
                                cmd.Parameters.AddWithValue("@CASNo", DBNull.Value);
                            }
                            else if (selecteddepartmentID == "4")
                            {
                                cmd.Parameters.AddWithValue("@MDL", string.IsNullOrEmpty(txtMDL.Text) ? DBNull.Value : (object)Convert.ToDecimal(txtMDL.Text));
                                cmd.Parameters.AddWithValue("@CASNo", string.IsNullOrEmpty(txtCASNo.Text) ? DBNull.Value : (object)txtCASNo.Text.Trim());
                                // Add null values for Food fields
                                cmd.Parameters.AddWithValue("@LOD", DBNull.Value);
                                cmd.Parameters.AddWithValue("@LOQ", DBNull.Value);
                                cmd.Parameters.AddWithValue("@LOR", DBNull.Value);
                            }
                            else
                            {
                                // Handle case where department is not Food or Chemical
                                cmd.Parameters.AddWithValue("@LOD", DBNull.Value);
                                cmd.Parameters.AddWithValue("@LOQ", DBNull.Value);
                                cmd.Parameters.AddWithValue("@LOR", DBNull.Value);
                                cmd.Parameters.AddWithValue("@MDL", DBNull.Value);
                                cmd.Parameters.AddWithValue("@CASNo", DBNull.Value);
                            }

                            cmd.ExecuteNonQuery();
                        }

                        // Commit transaction if the operation succeeds
                        transaction.Commit();

                        // Show success message
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Test Sub-Sub-Parameter updated successfully!'); window.location='TestSubSubParameterShow.aspx';", true);
                        Response.Redirect("TestSubSubParameterShow.aspx", false);
                    }
                    catch (Exception ex)
                    {
                        // Rollback transaction if the operation fails
                        transaction.Rollback();

                        // Handle exception
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Error updating test sub-sub-parameter: " + ex.Message.Replace("'", "\\'") + "');", true);
                    }
                }
            }
        }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("TestSubSubParameterShow.aspx");
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
    }
}