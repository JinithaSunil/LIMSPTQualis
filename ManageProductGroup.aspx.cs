using LIMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class ManageProductGroup : System.Web.UI.Page
    {
        Parameters param = new Parameters();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadProductSubGroups();
                LoadTestParameter();
                LoadAllParametersGrid();
            }
        }
        private void LoadAllParametersGrid()
        {
            try
            {
                DataTable dt = param.GetProductSubGroupParametersDetails(null); 
                gvParameters.DataSource = dt;
                gvParameters.DataBind();
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading parameters grid: " + ex.Message, false);
            }
        }
        private void LoadParametersGrid()
        {
            try
            {
                if (!string.IsNullOrEmpty(ddlProductSubGroup.SelectedValue))
                {
                    int subGroupId = Convert.ToInt32(ddlProductSubGroup.SelectedValue);
                    DataTable dt = param.GetProductSubGroupParametersDetails(subGroupId);
                    gvParameters.DataSource = dt;
                    gvParameters.DataBind();
                }
                else
                {
                    gvParameters.DataSource = null;
                    gvParameters.DataBind();
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading parameters grid: " + ex.Message, false);
            }
        }
        protected void gvParameters_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvParameters.PageIndex = e.NewPageIndex;
            LoadAllParametersGrid();
        }
        private void LoadProductSubGroups()
        {
            try
            {
                DataTable dt = param.GetProductSubGroups();
                ddlProductSubGroup.DataSource = dt;
                ddlProductSubGroup.DataTextField = "SubGroupName";
                ddlProductSubGroup.DataValueField = "SubGroupID";
                ddlProductSubGroup.DataBind();
                ddlProductSubGroup.Items.Insert(0, new ListItem("--Select Product Sub Group--", ""));
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading Product Sub Groups: " + ex.Message, false);
            }
        }
        private void LoadTestParameter()
        {
            try
            {
                DataTable dt = param.GetTestParametersSoftline();
                ddlTestParameter.DataSource = dt;
                ddlTestParameter.DataTextField = "TestParameter";
                ddlTestParameter.DataValueField = "TestParameterID";
                ddlTestParameter.DataBind();
                ddlTestParameter.Items.Insert(0, new ListItem("--Select Test Parameter--", ""));
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Error loading UOMs: " + ex.Message + "');", true);
            }
        }
        protected void ddlTestParameter_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ddlTestSubParameter.Items.Clear();
                ddlTestSubSubParameter.Items.Clear();

                if (!string.IsNullOrEmpty(ddlTestParameter.SelectedValue))
                {
                    int parameterId = Convert.ToInt32(ddlTestParameter.SelectedValue);
                    DataTable dtSub = param.GetTestSubParametersSoftline(parameterId);

                    ddlTestSubParameter.DataSource = dtSub;
                    ddlTestSubParameter.DataTextField = "TestSubParameter";
                    ddlTestSubParameter.DataValueField = "TestSubParameterID";
                    ddlTestSubParameter.DataBind();
                    ddlTestSubParameter.Items.Insert(0, new ListItem("--Select Sub Parameter--", ""));
                }

            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert", $"alert('Error loading Sub Parameters: {ex.Message}');", true);
            }
        }
        protected void ddlTestSubParameter_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ddlTestSubSubParameter.Items.Clear();

                if (!string.IsNullOrEmpty(ddlTestSubParameter.SelectedValue))
                {
                    int subParamId = Convert.ToInt32(ddlTestSubParameter.SelectedValue);
                    DataTable dtSubSub = param.GetTestSubSubParametersSoftline(subParamId);

                    ddlTestSubSubParameter.DataSource = dtSubSub;
                    ddlTestSubSubParameter.DataTextField = "TestSubSubParameter";
                    ddlTestSubSubParameter.DataValueField = "TestSubSubParameterID";
                    ddlTestSubSubParameter.DataBind();
                    ddlTestSubSubParameter.Items.Insert(0, new ListItem("--Select Sub Sub Parameter--", ""));
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert", $"alert('Error loading Sub Sub Parameters: {ex.Message}');", true);
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (Page.IsValid && !string.IsNullOrEmpty(ddlProductSubGroup.SelectedValue))
                {
                    int subGroupId = Convert.ToInt32(ddlProductSubGroup.SelectedValue);

                    // Get current parameter IDs from ProductSubGroup
                    DataTable currentData = param.GetProductSubGroupParameters(subGroupId);

                    string testParameterIds = "";
                    string testSubParameterIds = "";
                    string testSubSubParameterIds = "";

                    if (currentData.Rows.Count > 0)
                    {
                        testParameterIds = currentData.Rows[0]["TestParameterId"]?.ToString() ?? "";
                        testSubParameterIds = currentData.Rows[0]["TestSubParameterId"]?.ToString() ?? "";
                        testSubSubParameterIds = currentData.Rows[0]["TestSubSubParameterId"]?.ToString() ?? "";
                    }

                    // Add selected TestParameter if not already present
                    if (!string.IsNullOrEmpty(ddlTestParameter.SelectedValue))
                    {
                        string selectedParamId = ddlTestParameter.SelectedValue;
                        if (!IsIdInCommaSeparatedString(testParameterIds, selectedParamId))
                        {
                            testParameterIds = string.IsNullOrEmpty(testParameterIds)
                                ? selectedParamId
                                : testParameterIds + "," + selectedParamId;
                        }
                    }

                    // Add selected TestSubParameter if not already present
                    if (!string.IsNullOrEmpty(ddlTestSubParameter.SelectedValue))
                    {
                        string selectedSubParamId = ddlTestSubParameter.SelectedValue;
                        if (!IsIdInCommaSeparatedString(testSubParameterIds, selectedSubParamId))
                        {
                            testSubParameterIds = string.IsNullOrEmpty(testSubParameterIds)
                                ? selectedSubParamId
                                : testSubParameterIds + "," + selectedSubParamId;
                        }
                    }

                    // Add selected TestSubSubParameter if not already present
                    if (!string.IsNullOrEmpty(ddlTestSubSubParameter.SelectedValue))
                    {
                        string selectedSubSubParamId = ddlTestSubSubParameter.SelectedValue;
                        if (!IsIdInCommaSeparatedString(testSubSubParameterIds, selectedSubSubParamId))
                        {
                            testSubSubParameterIds = string.IsNullOrEmpty(testSubSubParameterIds)
                                ? selectedSubSubParamId
                                : testSubSubParameterIds + "," + selectedSubSubParamId;
                        }
                    }

                    int rowsAffected = param.UpdateProductSubGroupParameters(subGroupId,testParameterIds,testSubParameterIds,testSubSubParameterIds);

                    if (rowsAffected > 0)
                    {
                        ShowMessage("Test parameters updated successfully!", true);
                        ClearForm();
                        LoadParametersGrid();
                    }
                    else
                    {
                        ShowMessage("Failed to update test parameters. Please try again.", false);
                    }
                }
                else
                {
                    ShowMessage("Please select a Product Sub Group before submitting.", false);
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error updating parameters: " + ex.Message, false);
            }
        }

        private bool IsIdInCommaSeparatedString(string commaSeparatedIds, string idToCheck)
        {
            if (string.IsNullOrEmpty(commaSeparatedIds) || string.IsNullOrEmpty(idToCheck))
                return false;

            // Add commas at the beginning and end to ensure exact match
            string normalizedIds = "," + commaSeparatedIds + ",";
            string normalizedIdToCheck = "," + idToCheck + ",";

            return normalizedIds.Contains(normalizedIdToCheck);
        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearallForm();
        }
        private void ClearForm()
        {
            ddlTestSubParameter.SelectedIndex = 0;
            ddlTestSubSubParameter.SelectedIndex = 0;
            ddlTestParameter.SelectedIndex = 0;
            ddlTestSubSubParameter.Items.Clear();
            ddlTestSubParameter.Items.Clear();
        }
        private void ClearallForm()
        {
            
            if (ddlTestSubParameter.Items.Count > 0)
                ddlTestSubParameter.SelectedIndex = 0;

            if (ddlTestSubSubParameter.Items.Count > 0)
                ddlTestSubSubParameter.SelectedIndex = 0;

            if (ddlProductSubGroup.Items.Count > 0)
                ddlProductSubGroup.SelectedIndex = 0;

            if (ddlTestParameter.Items.Count > 0)
                ddlTestParameter.SelectedIndex = 0;

            LoadAllParametersGrid();
        }
        private void ShowMessage(string message, bool isSuccess)
        {
            // Also show browser alert
            string script = $"alert('{message.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "showalert", script, true);
        }

        //Grid
        protected void ddlProductSubGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadParametersGrid();
        }
        protected void gvParameters_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                // Set edit index
                gvParameters.EditIndex = e.NewEditIndex;

                // Keep the current grid state - don't reload, just rebind to maintain edit mode
                if (!string.IsNullOrEmpty(ddlProductSubGroup.SelectedValue))
                {
                    // If a specific subgroup is selected, load that
                    LoadParametersGrid();
                }
                else
                {
                    // If all data is shown, keep showing all data
                    LoadAllParametersGrid();
                }

                // Load dropdowns for edit mode
                GridViewRow editRow = gvParameters.Rows[e.NewEditIndex];
                DropDownList ddlEditTestParameter = editRow.FindControl("ddlEditTestParameter") as DropDownList;
                DropDownList ddlEditTestSubParameter = editRow.FindControl("ddlEditTestSubParameter") as DropDownList;
                DropDownList ddlEditTestSubSubParameter = editRow.FindControl("ddlEditTestSubSubParameter") as DropDownList;

                if (ddlEditTestParameter != null)
                {
                    LoadEditTestParameters(ddlEditTestParameter, editRow);
                }

                if (ddlEditTestSubParameter != null)
                {
                    LoadEditSubParameters(ddlEditTestSubParameter, editRow);
                }

                if (ddlEditTestSubSubParameter != null)
                {
                    LoadEditSubSubParameters(ddlEditTestSubSubParameter, editRow);
                }

                editRow.CssClass = "grid-edit-row";
            }
            catch (Exception ex)
            {
                ShowMessage("Error editing row: " + ex.Message, false);
            }
        }
        protected void gvParameters_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvParameters.EditIndex = -1;

            // Maintain the same grid state as before editing
            if (!string.IsNullOrEmpty(ddlProductSubGroup.SelectedValue))
            {
                LoadParametersGrid();
            }
            else
            {
                LoadAllParametersGrid();
            }
        }
        protected void ddlEditTestParameter_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DropDownList ddlTestParam = sender as DropDownList;
                if (ddlTestParam != null)
                {
                    GridViewRow row = ddlTestParam.NamingContainer as GridViewRow;
                    if (row != null)
                    {
                        DropDownList ddlSubParam = row.FindControl("ddlEditTestSubParameter") as DropDownList;
                        DropDownList ddlSubSubParam = row.FindControl("ddlEditTestSubSubParameter") as DropDownList;

                        if (ddlSubParam != null)
                        {
                            LoadEditSubParametersCascading(ddlSubParam, ddlTestParam, row);
                        }

                        if (ddlSubSubParam != null)
                        {
                            // Clear sub-sub parameters when test parameter changes
                            ddlSubSubParam.Items.Clear();
                            ddlSubSubParam.Items.Insert(0, new ListItem("--Select Sub-Sub Parameter--", ""));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error in test parameter dropdown change: " + ex.Message, false);
            }
        }
        protected void ddlEditTestSubParameter_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DropDownList ddlSub = sender as DropDownList;
                if (ddlSub != null)
                {
                    GridViewRow row = ddlSub.NamingContainer as GridViewRow;
                    if (row != null)
                    {
                        DropDownList ddlSubSub = row.FindControl("ddlEditTestSubSubParameter") as DropDownList;
                        if (ddlSubSub != null)
                        {
                            LoadEditSubSubParametersCascading(ddlSubSub, ddlSub, row);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error in dropdown change: " + ex.Message, false);
            }
        }
        protected void gvParameters_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                GridViewRow row = gvParameters.Rows[e.RowIndex];
                DropDownList ddlEditTestParameter = row.FindControl("ddlEditTestParameter") as DropDownList;
                DropDownList ddlEditTestSubParameter = row.FindControl("ddlEditTestSubParameter") as DropDownList;
                DropDownList ddlEditTestSubSubParameter = row.FindControl("ddlEditTestSubSubParameter") as DropDownList;

                // Get the original IDs from DataKeys
                string subGroupId = gvParameters.DataKeys[e.RowIndex]["SubGroupID"].ToString();
                int originalTestParameterId = GetOriginalParameterId("TestParameterID", e.RowIndex);
                int originalTestSubParameterId = GetOriginalParameterId("TestSubParameterID", e.RowIndex);
                int originalTestSubSubParameterId = GetOriginalParameterId("TestSubSubParameterID", e.RowIndex);

                // Get new IDs from selected dropdown values
                int newTestParameterId = ddlEditTestParameter?.SelectedValue != "" ? Convert.ToInt32(ddlEditTestParameter.SelectedValue) : 0;
                int newTestSubParameterId = ddlEditTestSubParameter?.SelectedValue != "" ? Convert.ToInt32(ddlEditTestSubParameter.SelectedValue) : 0;
                int newTestSubSubParameterId = ddlEditTestSubSubParameter?.SelectedValue != "" ? Convert.ToInt32(ddlEditTestSubSubParameter.SelectedValue) : 0;

                bool success = false;

                // Determine which update method to use based on what changed
                bool testParameterChanged = originalTestParameterId != newTestParameterId && newTestParameterId > 0;
                bool testSubParameterChanged = originalTestSubParameterId != newTestSubParameterId && newTestSubParameterId > 0;
                bool testSubSubParameterChanged = originalTestSubSubParameterId != newTestSubSubParameterId;

                if (testParameterChanged || testSubParameterChanged)
                {
                    // Full parameter update when TestParameter or TestSubParameter changes
                    success = param.UpdateSpecificParameter(
                        Convert.ToInt32(subGroupId),
                        originalTestParameterId,
                        originalTestSubParameterId,
                        originalTestSubSubParameterId,
                        newTestParameterId,
                        newTestSubParameterId,
                        newTestSubSubParameterId
                    );
                }
                else
                {
                    // Only TestSubSubParameter is being changed (most common scenario)
                    success = param.UpdateOnlyTestSubSubParameter(
                        Convert.ToInt32(subGroupId),
                        originalTestSubSubParameterId,
                        newTestSubSubParameterId
                    );
                }

                if (success)
                {
                    gvParameters.EditIndex = -1;

                    // Maintain the same grid state as before editing
                    if (!string.IsNullOrEmpty(ddlProductSubGroup.SelectedValue))
                    {
                        LoadParametersGrid();
                    }
                    else
                    {
                        LoadAllParametersGrid();
                    }

                    ShowMessage("Parameter updated successfully!", true);
                }
                else
                {
                    ShowMessage("Failed to update parameter.", false);
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error updating parameter: " + ex.Message, false);
            }
        }
        protected void gvParameters_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                string subGroupId = gvParameters.DataKeys[e.RowIndex]["SubGroupID"].ToString();
                int testParameterId = GetOriginalParameterId("TestParameterID", e.RowIndex);
                int testSubParameterId = GetOriginalParameterId("TestSubParameterID", e.RowIndex);
                int testSubSubParameterId = GetOriginalParameterId("TestSubSubParameterID", e.RowIndex);

                bool success = false;

                if (testParameterId > 0 && testSubParameterId > 0 && testSubSubParameterId > 0)
                {
                    string testParameterName = GetTestParameterNameById(testParameterId);
                    string testSubParameterName = GetTestSubParameterNameById(testSubParameterId);
                    string testSubSubParameterName = GetTestSubSubParameterNameById(testSubSubParameterId);

                    success = param.DeleteSpecificParameter(
                        Convert.ToInt32(subGroupId),
                        testParameterName,
                        testSubParameterName,
                        testSubSubParameterName
                    );
                }
                else if (testParameterId > 0 && testSubParameterId > 0 && testSubSubParameterId == 0)
                {
                    string testParameterName = GetTestParameterNameById(testParameterId);
                    string testSubParameterName = GetTestSubParameterNameById(testSubParameterId);

                    success = param.DeleteSpecificParameter(
                        Convert.ToInt32(subGroupId),
                        testParameterName,
                        testSubParameterName,
                        "" 
                    );
                }
                else if (testParameterId > 0 && testSubParameterId == 0 && testSubSubParameterId == 0)
                {
                    string testParameterName = GetTestParameterNameById(testParameterId);

                    success = param.DeleteSpecificParameter(
                        Convert.ToInt32(subGroupId),
                        testParameterName,
                        "", 
                        "" 
                    );
                }
                else if (testSubSubParameterId > 0)
                {
                    success = param.DeleteOnlySubSubParameter(Convert.ToInt32(subGroupId), testSubSubParameterId);
                }
                else
                {
                    ShowMessage("Invalid data for deletion.", false);
                    return;
                }

                if (success)
                {
                    // Maintain the same grid state as before deletion
                    if (!string.IsNullOrEmpty(ddlProductSubGroup.SelectedValue))
                    {
                        LoadParametersGrid();
                    }
                    else
                    {
                        LoadAllParametersGrid();
                    }

                    ShowMessage("Parameter deleted successfully!", true);
                }
                else
                {
                    ShowMessage("Failed to delete parameter.", false);
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error deleting parameter: " + ex.Message, false);
            }
        }

        private void LoadEditTestParameters(DropDownList ddl, GridViewRow row)
        {
            try
            {
                DataTable dtTestParams = param.GetTestParametersSoftline();
                ddl.DataSource = dtTestParams;
                ddl.DataTextField = "TestParameter";
                ddl.DataValueField = "TestParameterID";
                ddl.DataBind();
                ddl.Items.Insert(0, new ListItem("--Select Test Parameter--", ""));

                // Set selected value using ID if exists
                int currentTestParameterId = GetOriginalParameterId("TestParameterID", row.RowIndex);
                if (currentTestParameterId > 0)
                {
                    ListItem item = ddl.Items.FindByValue(currentTestParameterId.ToString());
                    if (item != null) item.Selected = true;
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading test parameters: " + ex.Message, false);
            }
        }
        private void LoadEditSubParametersCascading(DropDownList ddlSub, DropDownList ddlTestParam, GridViewRow row)
        {
            try
            {
                // Get the selected TestParameter ID from the dropdown
                int selectedTestParameterId = ddlTestParam.SelectedValue != "" ? Convert.ToInt32(ddlTestParam.SelectedValue) : 0;

                if (selectedTestParameterId > 0)
                {
                    DataTable dtSub = param.GetTestSubParametersSoftline(selectedTestParameterId);
                    ddlSub.DataSource = dtSub;
                    ddlSub.DataTextField = "TestSubParameter";
                    ddlSub.DataValueField = "TestSubParameterID";
                    ddlSub.DataBind();
                    ddlSub.Items.Insert(0, new ListItem("--Select Sub Parameter--", ""));

                    // Set selected value if editing existing record and TestParameter hasn't changed
                    int originalTestParameterId = GetOriginalParameterId("TestParameterID", row.RowIndex);
                    if (selectedTestParameterId == originalTestParameterId)
                    {
                        int currentSubParameterId = GetOriginalParameterId("TestSubParameterID", row.RowIndex);
                        if (currentSubParameterId > 0)
                        {
                            ListItem item = ddlSub.Items.FindByValue(currentSubParameterId.ToString());
                            if (item != null) item.Selected = true;
                        }
                    }
                }
                else
                {
                    // Clear the Sub dropdown if no TestParameter is selected
                    ddlSub.Items.Clear();
                    ddlSub.Items.Insert(0, new ListItem("--Select Sub Parameter--", ""));
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading sub parameters: " + ex.Message, false);
            }
        }
        private void LoadEditSubParameters(DropDownList ddl, GridViewRow row)
        {
            try
            {
                int testParameterId = GetOriginalParameterId("TestParameterID", row.RowIndex);

                if (testParameterId > 0)
                {
                    DataTable dtSub = param.GetTestSubParametersSoftline(testParameterId);
                    ddl.DataSource = dtSub;
                    ddl.DataTextField = "TestSubParameter";
                    ddl.DataValueField = "TestSubParameterID";
                    ddl.DataBind();
                    ddl.Items.Insert(0, new ListItem("--Select Sub Parameter--", ""));

                    // Set selected value using ID if exists
                    int currentSubParameterId = GetOriginalParameterId("TestSubParameterID", row.RowIndex);
                    if (currentSubParameterId > 0)
                    {
                        ListItem item = ddl.Items.FindByValue(currentSubParameterId.ToString());
                        if (item != null) item.Selected = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading sub parameters: " + ex.Message, false);
            }
        }
        private void LoadEditSubSubParameters(DropDownList ddl, GridViewRow row)
        {
            try
            {
                int testSubParameterId = GetOriginalParameterId("TestSubParameterID", row.RowIndex);

                if (testSubParameterId > 0)
                {
                    DataTable dtSub = param.GetTestSubSubParametersSoftline(testSubParameterId);
                    ddl.DataSource = dtSub;
                    ddl.DataTextField = "TestSubSubParameter";
                    ddl.DataValueField = "TestSubSubParameterID";
                    ddl.DataBind();
                    ddl.Items.Insert(0, new ListItem("--Select Sub-Sub Parameter--", ""));

                    // Set selected value using ID if exists
                    int currentSubSubParameterId = GetOriginalParameterId("TestSubSubParameterID", row.RowIndex);
                    if (currentSubSubParameterId > 0)
                    {
                        ListItem item = ddl.Items.FindByValue(currentSubSubParameterId.ToString());
                        if (item != null) item.Selected = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading sub-sub parameters: " + ex.Message, false);
            }
        }
        private void LoadEditSubSubParametersCascading(DropDownList ddlSubSub, DropDownList ddlSub, GridViewRow row)
        {
            try
            {
                // Get the selected TestSubParameter ID from the dropdown
                int selectedSubParameterId = ddlSub.SelectedValue != "" ? Convert.ToInt32(ddlSub.SelectedValue) : 0;

                if (selectedSubParameterId > 0)
                {
                    DataTable dtSubSub = param.GetTestSubSubParametersSoftline(selectedSubParameterId);
                    ddlSubSub.DataSource = dtSubSub;
                    ddlSubSub.DataTextField = "TestSubSubParameter";
                    ddlSubSub.DataValueField = "TestSubSubParameterID";
                    ddlSubSub.DataBind();
                    ddlSubSub.Items.Insert(0, new ListItem("--Select Sub-Sub Parameter--", ""));

                    // Set selected value if editing existing record and TestSubParameter hasn't changed
                    int originalSubParameterId = GetOriginalParameterId("TestSubParameterID", row.RowIndex);
                    if (selectedSubParameterId == originalSubParameterId)
                    {
                        int currentSubSubParameterId = GetOriginalParameterId("TestSubSubParameterID", row.RowIndex);
                        if (currentSubSubParameterId > 0)
                        {
                            ListItem item = ddlSubSub.Items.FindByValue(currentSubSubParameterId.ToString());
                            if (item != null) item.Selected = true;
                        }
                    }
                }
                else
                {
                    // Clear the SubSub dropdown if no SubParameter is selected
                    ddlSubSub.Items.Clear();
                    ddlSubSub.Items.Insert(0, new ListItem("--Select Sub-Sub Parameter--", ""));
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading sub-sub parameters: " + ex.Message, false);
            }
        }

        private int GetOriginalParameterId(string parameterIdField, int rowIndex)
        {
            try
            {
                if (gvParameters.DataKeys[rowIndex].Values[parameterIdField] != null)
                {
                    object value = gvParameters.DataKeys[rowIndex].Values[parameterIdField];
                    if (value != DBNull.Value && !string.IsNullOrEmpty(value.ToString()))
                    {
                        return Convert.ToInt32(value);
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                return 0; 
            }
        }
        private string GetTestParameterNameById(int testParameterId)
        {
            if (testParameterId <= 0) return "";

            string query = "SELECT TestParameter FROM TestParameter WHERE TestParameterID = @TestParameterID";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@TestParameterID", testParameterId);
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    return result?.ToString() ?? "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting test parameter name: " + ex.Message);
            }
        }
        private string GetTestSubParameterNameById(int testSubParameterId)
        {
            if (testSubParameterId <= 0) return "";

            string query = "SELECT TestSubParameter FROM TestSubParameter WHERE TestSubParameterID = @TestSubParameterID";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@TestSubParameterID", testSubParameterId);
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    return result?.ToString() ?? "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting test sub parameter name: " + ex.Message);
            }
        }
        private string GetTestSubSubParameterNameById(int testSubSubParameterId)
        {
            if (testSubSubParameterId <= 0) return "";

            string query = "SELECT TestSubSubParameter FROM TestSubSubParameter WHERE TestSubSubParameterID = @TestSubSubParameterID";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@TestSubSubParameterID", testSubSubParameterId);
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    return result?.ToString() ?? "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting test sub sub parameter name: " + ex.Message);
            }
        }
    }
}