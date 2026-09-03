using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LIMS.DAL;

namespace LIMS
{
    public partial class ResistivityOfSemiConductingScreen : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Initialize DAL object
                if (_cm == null) _cm = new DAL.CM();

                // Get sample information from query string
                if (Request.QueryString["s"] != null)
                {
                    _cm.sampleId = Convert.ToInt32(Request.QueryString["s"].ToLower());
                }

                if (Request.QueryString["r"] != null)
                {
                    _cm.ReportID = Request.QueryString["r"];
                }
                DataTable dt1 = _cm.Getdataforseriesandreportid_mechanical(_cm);
                if (dt1 != null && dt1.Rows.Count > 0)
                {
                    // Set dynamic values from database
                    cableType.Text = dt1.Rows[0]["SampleType"].ToString();
                    voltage.Text = dt1.Rows[0]["Voltage"].ToString();
                    cableSize.Text = dt1.Rows[0]["SizeOfSample"].ToString();
                    testMethodNo.Text = dt1.Rows[0]["TestMethod"].ToString();
                    refSpec.Text = dt1.Rows[0]["TestMethod"].ToString();


                    // hardcoded fields
                    roomno.Text = "Mechanical & Oven";
                    jobOrderNo.Text = "WO/2024120905";
                    worksheetno.Text = "-";
                    equipmentno.Text = "QE/17/PH/175,QE/12/TH/09";
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showError", "alert('Could not retrieve sample data. Please check your sample ID.');", true);
                }

                CheckAndPrefillForm();
            }
        }

        private void InitializeGrid()
        {
            // Create a DataTable to store the test results
            DataTable dt = new DataTable();
            dt.Columns.Add("ItemCategory", typeof(string));
            dt.Columns.Add("ItemTest", typeof(string));
            dt.Columns.Add("CorePhase", typeof(string));
            dt.Columns.Add("Diameter", typeof(string));
            dt.Columns.Add("Thickness", typeof(string));
            dt.Columns.Add("VolumeResistivity", typeof(string));

            // Store the DataTable in ViewState for persistence across postbacks
            ViewState["TestResults"] = dt;

            // Sort the DataView by the columns you want to merge
            DataView dv = dt.DefaultView;
            dv.Sort = "ItemCategory, ItemTest, CorePhase";

            // Bind the empty DataTable to the GridView
            gvSemiConductingTest.DataSource = dt;
            gvSemiConductingTest.DataBind();
        }
        protected void CheckAndPrefillForm()
        {
            try
            {
                // Check if there's a sample ID in the query string
                if (Request.QueryString["s"] != null)
                {
                    int sampleId = Convert.ToInt32(Request.QueryString["s"]);
                    int reportId = Convert.ToInt32(Request.QueryString["r"]);

                    // Create DAL object
                    if (_cm == null) _cm = new DAL.CM();
                    _cm.sampleId = sampleId;
                    _cm.ReportID = reportId.ToString();

                    // Pass both sampleId and reportId
                    DataTable existingData = GetExistingTestResultsForSample(sampleId, reportId);

                    if (existingData != null && existingData.Rows.Count > 0)
                    {
                        // Show update button since data exists
                        btnSaveData.Visible = false;
                        btnClearData.Visible = false;
                        btnUpdateData.Visible = true;

                        // Prefill upper section details
                        PrefillUpperSection(existingData);

                        // Prefill standard values
                        // PrefillStandardValues(existingData);                     
                        // Prefill test results grid
                        PrefillTestResultsGrid(sampleId);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error and show message
                ScriptManager.RegisterStartupScript(this, GetType(), "PrefillError",
                    $"alert('Error prefilling form: {ex.Message}');", true);
            }


        }
        private DataTable GetExistingTestResultsForSample(int sampleId, int reportId)
        {
            Dictionary<string, string> parameterResults = new Dictionary<string, string>();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    conn.Open();
                    string query = @"
                SELECT ReportParamMapID, Result 
                FROM TestResultsPTQualis 
                WHERE SampleID = @SampleID AND ReportID = @ReportID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SampleID", sampleId);
                        cmd.Parameters.AddWithValue("@ReportID", reportId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string paramId = reader["ReportParamMapID"].ToString();
                                string result = reader["Result"].ToString();
                                parameterResults[paramId] = result;
                            }
                        }
                    }
                }

                DataTable dt = new DataTable();
                dt.Columns.Add("ParameterID", typeof(string));
                dt.Columns.Add("Result", typeof(string));

                foreach (var param in parameterResults)
                {
                    dt.Rows.Add(param.Key, param.Value);
                }

                return dt;
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "DataFetchError",
                    $"alert('Error fetching existing data: {ex.Message}');", true);
                return null;
            }
        }
        private void PrefillUpperSection(DataTable existingData)
        {
            // Method to prefill upper section details based on parameter IDs
            SetTextBoxText(cableType, "6217");
            SetTextBoxText(refSpec, "6223");
            SetTextBoxText(cableSize, "6218");
            SetTextBoxText(testMethodNo, "6224");
            SetTextBoxText(voltage, "6219");
            SetTextBoxText(jobOrderNo, "6225");
            SetTextBoxText(worksheetno, "6226");
            SetTextBoxText(equipmentno, "6228");
            // Textbox fields
            SetTextBoxText(roomno, "6221");
            SetTextBoxText(roomhumidity, "6227");
            SetTextBoxText(roomtemp, "6222");
            SetTextBoxText(Date, "6220");

            SetTextBoxText(txtStandardmaxVolumeResitstivity, "5170");
            SetTextBoxText(txtStandardmaxVolumeResitstivity_conductor, "5211");

            // Helper method to set text for labels
            void SetLabelText(Label label, string paramId)
            {
                var row = existingData.Select($"ParameterID = '{paramId}'").FirstOrDefault();
                if (row != null)
                {
                    label.Text = row["Result"].ToString();
                }
            }

            // Helper method to set text for textboxes
            void SetTextBoxText(TextBox textBox, string paramId)
            {
                var row = existingData.Select($"ParameterID = '{paramId}'").FirstOrDefault();
                if (row != null)
                {
                    textBox.Text = row["Result"].ToString();
                }
            }
        }
        protected void btnClearData_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
        protected void btnAddToGrid_Click(object sender, EventArgs e)
        {
            ResetControlBorders();

            bool hasError = false;
            List<Control> invalidControls = new List<Control>();

            int specimenNo = 0; // You can handle specimenNo similarly if needed

            string Diameter = null, Thickness = null, VolumeResistivity = null;

            // Validate dropdown lists
            if (ddlTestCategory.SelectedIndex == -1 || string.IsNullOrWhiteSpace(ddlTestCategory.SelectedValue))
            {
                ddlTestCategory.CssClass += " input-error";
                hasError = true;
            }
            if (ddlTestType.SelectedIndex == -1 || string.IsNullOrWhiteSpace(ddlTestType.SelectedValue))
            {
                ddlTestType.CssClass += " input-error";
                hasError = true;
            }
            if (ddlCorePhase.SelectedIndex == -1 || string.IsNullOrWhiteSpace(ddlCorePhase.SelectedValue))
            {
                ddlCorePhase.CssClass += " input-error";
                hasError = true;
            }

            // Validate Diameter (allow "-")
            if (string.IsNullOrWhiteSpace(txtDiameter.Text))
            {
                Diameter = "0";  // Treat empty as 0 internally, but you can decide what to display
            }
            else if (txtDiameter.Text == "-")
            {
                Diameter = "-";  // Keep "-" as string for display
            }
            else if (decimal.TryParse(txtDiameter.Text, out decimal dValue))
            {
                Diameter = dValue.ToString();
            }
            else
            {
                invalidControls.Add(txtDiameter);
                hasError = true;
            }

            // Validate Thickness (allow "-")
            if (string.IsNullOrWhiteSpace(txtThickness.Text))
            {
                Thickness = "0";
            }
            else if (txtThickness.Text == "-")
            {
                Thickness = "-";
            }
            else if (decimal.TryParse(txtThickness.Text, out decimal tValue))
            {
                Thickness = tValue.ToString();
            }
            else
            {
                invalidControls.Add(txtThickness);
                hasError = true;
            }

            // Validate VolumeResistivity (allow "-")
            if (string.IsNullOrWhiteSpace(txtVolumeResistivity.Text))
            {
                VolumeResistivity = "0";
            }
            else if (txtVolumeResistivity.Text == "-")
            {
                VolumeResistivity = "-";
            }
            else if (decimal.TryParse(txtVolumeResistivity.Text, out decimal vValue))
            {
                VolumeResistivity = vValue.ToString();
            }
            else
            {
                invalidControls.Add(txtVolumeResistivity);
                hasError = true;
            }

            foreach (var control in invalidControls)
            {
                if (control is TextBox)
                {
                    ((TextBox)control).BorderColor = System.Drawing.Color.FromArgb(255, 150, 150);
                    ((TextBox)control).BorderStyle = BorderStyle.Solid;
                    ((TextBox)control).BorderWidth = 1;
                }
            }

            if (hasError)
            {
                ShowMessage("Please fill in all required fields correctly.");
                return;
            }

            DataTable dt = ViewState["TestResults"] as DataTable;
            if (dt == null)
            {
                dt = new DataTable();
                dt.Columns.Add("ItemCategory", typeof(string));
                dt.Columns.Add("ItemTest", typeof(string));
                dt.Columns.Add("CorePhase", typeof(string));
                dt.Columns.Add("Diameter", typeof(string));  // changed to string
                dt.Columns.Add("Thickness", typeof(string)); // changed to string
                dt.Columns.Add("VolumeResistivity", typeof(string)); // changed to string
            }

            DataRow dr = dt.NewRow();
            dr["ItemCategory"] = ddlTestCategory.SelectedValue;
            dr["ItemTest"] = ddlTestType.SelectedValue;
            dr["CorePhase"] = ddlCorePhase.SelectedValue;
            dr["Diameter"] = Diameter;
            dr["Thickness"] = Thickness;
            dr["VolumeResistivity"] = VolumeResistivity;

            dt.Rows.Add(dr);
            ViewState["TestResults"] = dt;

            gvSemiConductingTest.DataSource = dt;
            gvSemiConductingTest.DataBind();

            ClearInputFields();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            List<string> errors = new List<string>();
            List<Control> invalidControls = new List<Control>();

            // Check if Grid has data
            bool gridHasData = (gvSemiConductingTest.Rows.Count > 0);

            // Validate Main Section (required regardless of grid data)
            if (string.IsNullOrEmpty(Date.Text))
            {
                errors.Add("Date is required.");
                invalidControls.Add(Date);
            }
            if (string.IsNullOrEmpty(roomno.Text))
            {
                errors.Add("Room No. is required.");
                invalidControls.Add(roomno);
            }
            if (string.IsNullOrEmpty(roomhumidity.Text) || !double.TryParse(roomhumidity.Text, out _))
            {
                errors.Add("Room Humidity must be a valid number.");
                invalidControls.Add(roomhumidity);
            }
            if (string.IsNullOrEmpty(roomtemp.Text) || !double.TryParse(roomtemp.Text, out _))
            {
                errors.Add("Room Temp. must be a valid number.");
                invalidControls.Add(roomtemp);
            }

            // Only validate the upper fields if grid has no data
            if (!gridHasData)
            {
                // Diameter - allow "-" or valid number
                if (string.IsNullOrEmpty(txtDiameter.Text) || (txtDiameter.Text != "-" && !double.TryParse(txtDiameter.Text, out _)))
                {
                    errors.Add("Diameter must be a valid number or '-'.");
                    invalidControls.Add(txtDiameter);
                }

                // Thickness - allow "-" or valid number
                if (string.IsNullOrEmpty(txtThickness.Text) || (txtThickness.Text != "-" && !double.TryParse(txtThickness.Text, out _)))
                {
                    errors.Add("Thickness must be a valid number or '-'.");
                    invalidControls.Add(txtThickness);
                }

                // VolumeResistivity - allow "-" or valid number
                if (string.IsNullOrEmpty(txtVolumeResistivity.Text) || (txtVolumeResistivity.Text != "-" && !double.TryParse(txtVolumeResistivity.Text, out _)))
                {
                    errors.Add("VolumeResistivity must be a valid number or '-'.");
                    invalidControls.Add(txtVolumeResistivity);
                }
            }

            // Validate Standard Values section - allow "-" or valid number
            if (string.IsNullOrEmpty(txtStandardmaxVolumeResitstivity.Text)
                || (txtStandardmaxVolumeResitstivity.Text != "-"
                    && !double.TryParse(txtStandardmaxVolumeResitstivity.Text, out _)))
            {
                errors.Add("Standard Width must be a valid number or '-'.");
                invalidControls.Add(txtStandardmaxVolumeResitstivity);
            }

            // Check if Grid has data (only required if grid is empty)
            if (!gridHasData)
            {
                errors.Add("Add at least one test result to the grid.");
            }

            // Reset all control borders first
            ResetControlBorders();

            // Display errors or proceed
            if (errors.Count > 0)
            {
                // Highlight invalid controls with light red border
                foreach (var control in invalidControls)
                {
                    if (control is TextBox)
                    {
                        ((TextBox)control).BorderColor = System.Drawing.Color.FromArgb(255, 150, 150);
                        ((TextBox)control).BorderStyle = BorderStyle.Solid;
                        ((TextBox)control).BorderWidth = 1;
                    }
                }
                string errorMessage = "Validation Errors:\\n" + string.Join("\\n• ", errors.Select(err => err.Replace("'", "\\'").Replace("\n", "").Replace("\r", "")));

                ScriptManager.RegisterStartupScript(this, GetType(), "ValidationError",
                    $"alert('{errorMessage}');", true);

            }
            else
            {
                bool isSaved = Submitform();
                if (isSaved)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "SaveSuccess",
                        "alert('Test results and Report Generated successfully!');", true);
                    ClearForm();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "SaveFailed",
                        "alert('Test results were not saved due to an error.');", true);
                }
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            List<string> errors = new List<string>();
            List<Control> invalidControls = new List<Control>();

            // Check if Grid has data
            bool gridHasData = (gvSemiConductingTest.Rows.Count > 0);

            // Validate Main Section (required regardless of grid data)
            if (string.IsNullOrEmpty(Date.Text))
            {
                errors.Add("Date is required.");
                invalidControls.Add(Date);
            }
            if (string.IsNullOrEmpty(roomno.Text))
            {
                errors.Add("Room No. is required.");
                invalidControls.Add(roomno);
            }
            if (string.IsNullOrEmpty(roomhumidity.Text) || !double.TryParse(roomhumidity.Text, out _))
            {
                errors.Add("Room Humidity must be a valid number.");
                invalidControls.Add(roomhumidity);
            }
            if (string.IsNullOrEmpty(roomtemp.Text) || !double.TryParse(roomtemp.Text, out _))
            {
                errors.Add("Room Temp. must be a valid number.");
                invalidControls.Add(roomtemp);
            }

            // Only validate the upper fields if grid has no data
            if (!gridHasData)
            {
                // Diameter - allow "-" or valid number
                if (string.IsNullOrEmpty(txtDiameter.Text) || (txtDiameter.Text != "-" && !double.TryParse(txtDiameter.Text, out _)))
                {
                    errors.Add("Width Diameter must be a valid number or '-'.");
                    invalidControls.Add(txtDiameter);
                }

                // Thickness - allow "-" or valid number
                if (string.IsNullOrEmpty(txtThickness.Text) || (txtThickness.Text != "-" && !double.TryParse(txtThickness.Text, out _)))
                {
                    errors.Add("Thickness must be a valid number or '-'.");
                    invalidControls.Add(txtThickness);
                }

                // VolumeResistivity - allow "-" or valid number
                if (string.IsNullOrEmpty(txtVolumeResistivity.Text) || (txtVolumeResistivity.Text != "-" && !double.TryParse(txtVolumeResistivity.Text, out _)))
                {
                    errors.Add("Volume value must be a valid number or '-'.");
                    invalidControls.Add(txtVolumeResistivity);
                }
            }

            // Validate Standard Values section - allow "-" or valid number
            if (string.IsNullOrEmpty(txtStandardmaxVolumeResitstivity.Text)
                || (txtStandardmaxVolumeResitstivity.Text != "-"
                    && !double.TryParse(txtStandardmaxVolumeResitstivity.Text, out _)))
            {
                errors.Add("Standard Width must be a valid number or '-'.");
                invalidControls.Add(txtStandardmaxVolumeResitstivity);
            }

            // Check if Grid has data (only required if grid is empty)
            if (!gridHasData)
            {
                errors.Add("Add at least one test result to the grid.");
            }

            // Reset all control borders first
            ResetControlBorders();

            // Display errors or proceed
            if (errors.Count > 0)
            {
                // Highlight invalid controls with light red border
                foreach (var control in invalidControls)
                {
                    if (control is TextBox)
                    {
                        ((TextBox)control).BorderColor = System.Drawing.Color.FromArgb(255, 150, 150);
                        ((TextBox)control).BorderStyle = BorderStyle.Solid;
                        ((TextBox)control).BorderWidth = 1;
                    }
                }
                string errorMessage = "Validation Errors:\\n" + string.Join("\\n• ", errors.Select(err => err.Replace("'", "\\'").Replace("\n", "").Replace("\r", "")));

                ScriptManager.RegisterStartupScript(this, GetType(), "ValidationError",
                    $"alert('{errorMessage}');", true);

            }
            else
            {
                try
                {
                    if (Request.QueryString["s"] != null)
                    {
                        int sampleId = Convert.ToInt32(Request.QueryString["s"]);
                        int reportId = Convert.ToInt32(Request.QueryString["r"]);

                        if (_cm == null) _cm = new DAL.CM();
                        _cm.sampleId = sampleId;
                        _cm.ReportID = reportId.ToString();
                        DeleteExistingRecords(sampleId, reportId);

                        bool success = Submitform();
                        if (!success)
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "UpdateError", "alert('Update failed.');", true);
                        }
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "UpdateError",
                            "alert('Sample ID not found. Cannot update.');", true);
                    }
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "UpdateError",
                        $"alert('Error updating test results: {ex.Message}');", true);
                }
            }
        }

        private bool Submitform()
        {
            try
            {
                // Get the DataTable from ViewState
                DataTable dt = ViewState["TestResults"] as DataTable;

                if (dt == null || dt.Rows.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showError", "alert('No data to save. Please add test results first.');", true);
                    return false;
                }

                // Create DAL object
                if (_cm == null) _cm = new DAL.CM();
                if (Request.QueryString["s"] != null) { _cm.sampleId = Convert.ToInt32(Request.QueryString["s"].ToLower()); }
                if (Request.QueryString["r"] != null) { _cm.ReportID = Request.QueryString["r"]; }

                DataTable dt1 = _cm.Getdataforseriesandreportid_mechanical(_cm);
                if (dt1.Rows.Count > 0)
                {
                    _cm.sampleIDSeries = dt1.Rows[0]["sampleIDSeries"].ToString();
                    _cm.CRFSampleId = Convert.ToInt32(dt1.Rows[0]["CRFSampleId"]);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "noData", "alert('No data found for the given sample ID and report ID');", true);
                    return false;
                }

                // Organize data by test types (Before aging and After aging)
                //Dictionary<string, List<DataRow>> testTypeData = new Dictionary<string, List<DataRow>>();
                Dictionary<string, List<DataRow>> testTypeData = new Dictionary<string, List<DataRow>>();


                //foreach (DataRow row in dt.Rows)
                //{
                //    string testType = row["ItemTest"].ToString();

                //    if (!testTypeData.ContainsKey(testType))
                //    {
                //        testTypeData[testType] = new List<DataRow>();
                //    }

                //    testTypeData[testType].Add(row);
                //}
                foreach (DataRow row in dt.Rows)
                {
                    string testType = row["ItemTest"].ToString();
                    string category = row["ItemCategory"].ToString().Trim().ToLower();
                    string combinedKey = testType + "_" + category;  // Create a combined key

                    if (!testTypeData.ContainsKey(combinedKey))
                    {
                        testTypeData[combinedKey] = new List<DataRow>();
                    }

                    testTypeData[combinedKey].Add(row);
                }

                List<string> reportParamMapList = new List<string>();
                List<string> resultList = new List<string>();

                //foreach (var testTypeKey in testTypeData.Keys)
                //{
                //    var rows = testTypeData[testTypeKey];
                //    string testType = testTypeKey.Trim().ToLower();
                //    string testCategory = rows[0]["ItemCategory"].ToString().Trim().ToLower();
                // Then when processing:
                foreach (var combinedKey in testTypeData.Keys)
                {
                    var rows = testTypeData[combinedKey];
                    // Split the combined key to get test type and category
                    string[] keyParts = combinedKey.Split('_');
                    string testType = keyParts[0].Trim().ToLower();
                    string testCategory = keyParts[1];  // Already lowercase from earlier


                    // List<string> coreList = new List<string>();
                    List<string> coreList = new List<string>();
                    List<string> diameterList = new List<string>();
                    List<string> thicknessList = new List<string>();
                    List<string> volumeResList = new List<string>();

                    foreach (var row in rows)
                    {
                        coreList.Add(row["CorePhase"].ToString());
                        diameterList.Add(row["Diameter"].ToString());
                        thicknessList.Add(row["Thickness"].ToString());
                        volumeResList.Add(row["VolumeResistivity"].ToString());
                    }

                    string corePhases = string.Join(",", coreList);
                    string diameter = string.Join(",", diameterList);
                    string thickness = string.Join(",", thicknessList);
                    string volumeResistivity = string.Join(",", volumeResList);

                    if (testCategory == "insulation screen")
                    {
                        if (testType.Contains("before"))
                        {
                            reportParamMapList.Add("5177"); resultList.Add(corePhases);
                            reportParamMapList.Add("5164"); resultList.Add(diameter);
                            reportParamMapList.Add("5165"); resultList.Add(thickness);
                            reportParamMapList.Add("5166"); resultList.Add(volumeResistivity);
                        }
                        else if (testType.Contains("after"))
                        {
                            reportParamMapList.Add("5208"); resultList.Add(corePhases);
                            reportParamMapList.Add("5167"); resultList.Add(diameter);
                            reportParamMapList.Add("5168"); resultList.Add(thickness);
                            reportParamMapList.Add("5169"); resultList.Add(volumeResistivity);
                        }
                    }
                    else if (testCategory == "conductor screen")
                    {
                        if (testType.Contains("before"))
                        {
                            reportParamMapList.Add("5209"); resultList.Add(corePhases);
                            reportParamMapList.Add("5171"); resultList.Add(diameter);
                            reportParamMapList.Add("5172"); resultList.Add(thickness);
                            reportParamMapList.Add("5173"); resultList.Add(volumeResistivity);
                        }
                        else if (testType.Contains("after"))
                        {
                            reportParamMapList.Add("5210"); resultList.Add(corePhases);
                            reportParamMapList.Add("5174"); resultList.Add(diameter);
                            reportParamMapList.Add("5175"); resultList.Add(thickness);
                            reportParamMapList.Add("5176"); resultList.Add(volumeResistivity);
                        }
                    }
                }


                // Add Standard values
                reportParamMapList.Add("5170"); resultList.Add(txtStandardmaxVolumeResitstivity.Text);
                reportParamMapList.Add("5211"); resultList.Add(txtStandardmaxVolumeResitstivity_conductor.Text);

                reportParamMapList.Add("6217"); resultList.Add(cableType.Text);
                reportParamMapList.Add("6223"); resultList.Add(refSpec.Text);
                reportParamMapList.Add("6218"); resultList.Add(cableSize.Text);
                reportParamMapList.Add("6224"); resultList.Add(testMethodNo.Text);
                reportParamMapList.Add("6219"); resultList.Add(voltage.Text);
                reportParamMapList.Add("6225"); resultList.Add(jobOrderNo.Text);
                reportParamMapList.Add("6226"); resultList.Add(worksheetno.Text);
                reportParamMapList.Add("6221"); resultList.Add(roomno.Text);
                reportParamMapList.Add("6227"); resultList.Add(roomhumidity.Text);
                reportParamMapList.Add("6222"); resultList.Add(roomtemp.Text);
                reportParamMapList.Add("6220"); resultList.Add(Date.Text);
                reportParamMapList.Add("6228"); resultList.Add(equipmentno.Text);

                // Convert to arrays
                string[] reportParamMap = reportParamMapList.ToArray();
                string[] result = resultList.ToArray();

                int modified = _cm.AddTestResultPTQualis(_cm, reportParamMap, result);

                if (modified > 0)
                {
                    _cm.sampleStatusID = 27;
                    _cm.UpdateCRFSampleDetailsStatus(_cm);

                    _cm.StatusID = 27;
                    _cm.UpdateAllocationStatusid_mechanical(_cm);

                    // Show success and open both reports in new tabs
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
     "showSuccessAndOpenReports",
     $@"
        alert('Test results and Reports Generated successfully!');
        window.open('GenerateTestRecord_ResistivityOfSemiConductingScreen.aspx?s={_cm.sampleId}&r={_cm.ReportID}', '_blank');
        window.open('GenerateTestRecord_ResistivityOfSemiConductingScreen_Conductor.aspx?s={_cm.sampleId}&r={_cm.ReportID}', '_blank');
        setTimeout(function() {{
            window.location.href = 'Default.aspx';
        }}, 1000);
    ",
     true);


                }
                else
                {

                }
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showError", "alert('Failed to save test results.');", true);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(),
                    "showError", $"alert('Error: {ex.Message}');", true);
                return false;
            }
        }
        private void DeleteExistingRecords(int sampleId, int reportid)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    conn.Open();


                    // Delete all existing test results for this sample
                    string deleteQuery = "DELETE FROM TestResultsPTQualis WHERE SampleID = @SampleID and ReportId = @ReportId";

                    using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@SampleID", sampleId);
                        cmd.Parameters.AddWithValue("@ReportId", reportid);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error and rethrow
                throw new Exception("Error deleting existing records: " + ex.Message);
            }
        }
        private void PrefillTestResultsGrid(int sampleId)
        {
            try
            {
                // Initialize grid if needed
                InitializeGrid();

                // Create DAL object if not exists
                if (_cm == null)
                    _cm = new DAL.CM();

                // Fetch existing test results for this sample
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    conn.Open();
                    string query = @"
WITH ResultPivot AS (
    SELECT 
        SampleID,
        -- Insulation Screen
        MAX(CASE WHEN ReportParamMapID = '5177' THEN Result END) AS InsulationScreenBeforeAgeingCore,
        MAX(CASE WHEN ReportParamMapID = '5208' THEN Result END) AS InsulationScreenAfterAgeingCore,
        MAX(CASE WHEN ReportParamMapID = '5164' THEN Result END) AS InsulationScreenBeforeAgeingDiameter,
        MAX(CASE WHEN ReportParamMapID = '5167' THEN Result END) AS InsulationScreenAfterAgeingDiameter,
        MAX(CASE WHEN ReportParamMapID = '5165' THEN Result END) AS InsulationScreenBeforeAgeingThickness,
        MAX(CASE WHEN ReportParamMapID = '5168' THEN Result END) AS InsulationScreenAfterAgeingThickness,
        MAX(CASE WHEN ReportParamMapID = '5166' THEN Result END) AS InsulationScreenBeforeAgeingVolumeResistivity,
        MAX(CASE WHEN ReportParamMapID = '5169' THEN Result END) AS InsulationScreenAfterAgeingVolumeResistivity,

        -- Conductor Screen
        MAX(CASE WHEN ReportParamMapID = '5209' THEN Result END) AS ConductorScreenBeforeAgeingCore,
        MAX(CASE WHEN ReportParamMapID = '5210' THEN Result END) AS ConductorScreenAfterAgeingCore,
        MAX(CASE WHEN ReportParamMapID = '5171' THEN Result END) AS ConductorScreenBeforeAgeingDiameter,
        MAX(CASE WHEN ReportParamMapID = '5174' THEN Result END) AS ConductorScreenAfterAgeingDiameter,
        MAX(CASE WHEN ReportParamMapID = '5172' THEN Result END) AS ConductorScreenBeforeAgeingThickness,
        MAX(CASE WHEN ReportParamMapID = '5175' THEN Result END) AS ConductorScreenAfterAgeingThickness,
        MAX(CASE WHEN ReportParamMapID = '5173' THEN Result END) AS ConductorScreenBeforeAgeingVolumeResistivity,
        MAX(CASE WHEN ReportParamMapID = '5176' THEN Result END) AS ConductorScreenAfterAgeingVolumeResistivity
    FROM TestResultsPTQualis
    WHERE SampleID = @SampleID
    GROUP BY SampleID
)

SELECT *
FROM (
    -- Insulation screen - Before
    SELECT 
        'Insulation screen' AS ItemCategory,
        'Before Ageing' AS ItemTest,
        InsulationScreenBeforeAgeingCore AS CorePhase,
        InsulationScreenBeforeAgeingDiameter AS Diameter,
        InsulationScreenBeforeAgeingThickness AS Thickness,
        InsulationScreenBeforeAgeingVolumeResistivity AS VolumeResistivity
    FROM ResultPivot
    WHERE InsulationScreenBeforeAgeingCore IS NOT NULL
    UNION ALL

    -- Insulation screen - After
    SELECT 
        'Insulation screen' AS ItemCategory,
        'After Ageing' AS ItemTest,
        InsulationScreenAfterAgeingCore AS CorePhase,
        InsulationScreenAfterAgeingDiameter AS Diameter,
        InsulationScreenAfterAgeingThickness AS Thickness,
        InsulationScreenAfterAgeingVolumeResistivity AS VolumeResistivity
    FROM ResultPivot
    WHERE InsulationScreenAfterAgeingCore IS NOT NULL
    UNION ALL

    -- Conductor screen - Before
    SELECT 
        'Conductor screen' AS ItemCategory,
        'Before Ageing' AS ItemTest,
        ConductorScreenBeforeAgeingCore AS CorePhase,
        ConductorScreenBeforeAgeingDiameter AS Diameter,
        ConductorScreenBeforeAgeingThickness AS Thickness,
        ConductorScreenBeforeAgeingVolumeResistivity AS VolumeResistivity
    FROM ResultPivot
    WHERE ConductorScreenBeforeAgeingCore IS NOT NULL
    UNION ALL

    -- Conductor screen - After
    SELECT 
        'Conductor screen' AS ItemCategory,
        'After Ageing' AS ItemTest,
        ConductorScreenAfterAgeingCore AS CorePhase,
        ConductorScreenAfterAgeingDiameter AS Diameter,
        ConductorScreenAfterAgeingThickness AS Thickness,
        ConductorScreenAfterAgeingVolumeResistivity AS VolumeResistivity
    FROM ResultPivot
    WHERE ConductorScreenAfterAgeingCore IS NOT NULL
) AS OrderedResults
ORDER BY 
    CASE 
        WHEN ItemCategory = 'Insulation screen' AND ItemTest = 'Before Ageing' THEN 1
        WHEN ItemCategory = 'Insulation screen' AND ItemTest = 'After Ageing' THEN 2
        WHEN ItemCategory = 'Conductor screen' AND ItemTest = 'Before Ageing' THEN 3
        WHEN ItemCategory = 'Conductor screen' AND ItemTest = 'After Ageing' THEN 4
    END,
    CorePhase";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SampleID", sampleId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            DataTable dt = ViewState["TestResults"] as DataTable;
                            dt.Clear(); // Clear existing rows

                            while (reader.Read())
                            {
                                // Split all comma-separated values
                                string[] corePhases = SplitAndTrim(reader["CorePhase"].ToString());
                                string[] Diameter = SplitAndTrim(reader["Diameter"].ToString());
                                string[] Thickness = SplitAndTrim(reader["Thickness"].ToString());
                                string[] VolumeResistivity = SplitAndTrim(reader["VolumeResistivity"].ToString());

                                for (int i = 0; i < corePhases.Length; i++)
                                {
                                    DataRow newRow = dt.NewRow();
                                    newRow["ItemCategory"] = reader["ItemCategory"].ToString();
                                    newRow["ItemTest"] = reader["ItemTest"].ToString();
                                    newRow["CorePhase"] = corePhases[i];
                                    newRow["Diameter"] = GetArrayValue(Diameter, i);
                                    newRow["Thickness"] = GetArrayValue(Thickness, i);
                                    newRow["VolumeResistivity"] = GetArrayValue(VolumeResistivity, i);

                                    dt.Rows.Add(newRow);
                                }
                            }

                            // Rebind grid
                            gvSemiConductingTest.DataSource = dt;
                            gvSemiConductingTest.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error and show message
                ScriptManager.RegisterStartupScript(this, GetType(), "GridPrefillError",
                    $"alert('Error prefilling test results grid: {ex.Message}');", true);
            }
        }
        protected void gvSemiConductingTest_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("RowDataBound fired for row: " + e.Row.RowIndex);
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataTable dt = (DataTable)ViewState["TestResults"];
                int rowIndex = e.Row.RowIndex;

                // 1. Merge ItemCategory (TestCategory) ONLY if same as previous row
                if (rowIndex > 0 &&
                    dt.Rows[rowIndex]["ItemCategory"].ToString() == dt.Rows[rowIndex - 1]["ItemCategory"].ToString())
                {
                    ProcessColumn(dt, rowIndex, e.Row, 0); // Column 0 = ItemCategory
                }

                // 2. Handle ItemTest column - DO NOT merge if different from previous row
                // (This preserves the existing grouping logic)
                if (rowIndex > 0 &&
                    dt.Rows[rowIndex]["ItemTest"].ToString() == dt.Rows[rowIndex - 1]["ItemTest"].ToString())
                {
                    ProcessColumn(dt, rowIndex, e.Row, 1); // Column 1 = ItemTest
                }

                // 3. Handle CorePhase column - merge ONLY within same ItemTest group
                if (rowIndex > 0 &&
                    dt.Rows[rowIndex]["ItemTest"].ToString() == dt.Rows[rowIndex - 1]["ItemTest"].ToString() &&
                    dt.Rows[rowIndex]["CorePhase"].ToString() == dt.Rows[rowIndex - 1]["CorePhase"].ToString())
                {
                    ProcessColumn(dt, rowIndex, e.Row, 2); // Column 2 = CorePhase
                }

                // 4. Handle Resistivity Min - merge ONLY within same ItemTest & CorePhase
                if (rowIndex > 0 &&
                    dt.Rows[rowIndex]["ItemTest"].ToString() == dt.Rows[rowIndex - 1]["ItemTest"].ToString() &&
                    dt.Rows[rowIndex]["CorePhase"].ToString() == dt.Rows[rowIndex - 1]["CorePhase"].ToString())
                {
                    ProcessColumn(dt, rowIndex, e.Row, 7); // Column 7 = Strippability Min
                }

                // 5. Handle Resistivity  - merge ONLY within same ItemTest & CorePhase
                if (rowIndex > 0 &&
                    dt.Rows[rowIndex]["ItemTest"].ToString() == dt.Rows[rowIndex - 1]["ItemTest"].ToString() &&
                    dt.Rows[rowIndex]["CorePhase"].ToString() == dt.Rows[rowIndex - 1]["CorePhase"].ToString())
                {
                    ProcessColumn(dt, rowIndex, e.Row, 8); // Column 8 = Strippability Max
                }
            }
        }
        private void ProcessColumn(DataTable dt, int currentRowIndex, GridViewRow currentRow, int columnIndex)
        {
            try
            {
                // Get current cell value
                string currentValue = dt.Rows[currentRowIndex][columnIndex].ToString();
                // Get previous cell value
                string previousValue = dt.Rows[currentRowIndex - 1][columnIndex].ToString();

                // Debug
                System.Diagnostics.Debug.WriteLine($"Processing column {columnIndex}: Row {currentRowIndex} value: '{currentValue}', Previous value: '{previousValue}'");

                // If values are the same
                if (currentValue == previousValue)
                {
                    // Hide current cell
                    currentRow.Cells[columnIndex].Visible = false;

                    // Find the first visible cell above with the same value
                    int spanRowIndex = currentRowIndex - 1;
                    GridView gv = (GridView)currentRow.Parent.Parent; // Get reference to GridView from current row

                    // Make sure we don't go below 0 and we're looking for the first VISIBLE cell
                    while (spanRowIndex >= 0 &&
                           gv.Rows[spanRowIndex].RowType == DataControlRowType.DataRow &&
                           !gv.Rows[spanRowIndex].Cells[columnIndex].Visible)
                    {
                        spanRowIndex--;
                    }

                    // Make sure we have a valid row index
                    if (spanRowIndex >= 0 && gv.Rows[spanRowIndex].RowType == DataControlRowType.DataRow)
                    {
                        // Increase rowspan of that cell
                        if (gv.Rows[spanRowIndex].Cells[columnIndex].RowSpan == 0)
                        {
                            gv.Rows[spanRowIndex].Cells[columnIndex].RowSpan = 2;
                            System.Diagnostics.Debug.WriteLine($"Set RowSpan=2 for row {spanRowIndex}, column {columnIndex}");
                        }
                        else
                        {
                            gv.Rows[spanRowIndex].Cells[columnIndex].RowSpan += 1;
                            System.Diagnostics.Debug.WriteLine($"Increased RowSpan to {gv.Rows[spanRowIndex].Cells[columnIndex].RowSpan} for row {spanRowIndex}, column {columnIndex}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ProcessColumn: {ex.Message}");
                // You might want to handle or log this error
            }
        }
        protected void gvSemiConductingTest_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                DataTable dt = (DataTable)ViewState["TestResults"];
                dt.Rows.RemoveAt(rowIndex);
                ViewState["TestResults"] = dt;
                gvSemiConductingTest.DataSource = dt;
                gvSemiConductingTest.DataBind();

                // Update the Specimen No. in the upper section
                // UpdateSpecimenNumber();
            }
        }
        private void ClearInputFields()
        {
            txtDiameter.Text = "";
            txtThickness.Text = "";
            txtVolumeResistivity.Text = "";
        }
        private void ShowMessage(string message)
        {
            string safeMessage = HttpUtility.JavaScriptStringEncode(message);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('{safeMessage}');", true);
        }
        private void ClearForm()
        {
            // Main Section
            Date.Text = "";
            roomno.Text = "";
            roomhumidity.Text = "";
            roomtemp.Text = "";
            worksheetno.Text = "";
            equipmentno.Text = "";

            // Test Details
            //  testdate.Text = "";
            ddlTestType.SelectedIndex = 0;
            ddlCorePhase.SelectedIndex = 0;
            txtDiameter.Text = "";
            txtThickness.Text = "";
            txtVolumeResistivity.Text = "";
            // txtMeasured.Text = "";

            // Clear GridView
            gvSemiConductingTest.DataSource = null;
            gvSemiConductingTest.DataBind();

            // Standard Values
            txtStandardmaxVolumeResitstivity.Text = "";

            txtStandardmaxVolumeResitstivity_conductor.Text = "";
            // Ageing Specimen & Cable

        }
        private void ResetControlBorders()
        {
            // Reset all TextBox controls
            List<TextBox> allTextBoxes = new List<TextBox>();

            // Add all TextBox controls from the form
            //  allTextBoxes.Add(Date);
            allTextBoxes.Add(roomno);
            allTextBoxes.Add(roomhumidity);
            allTextBoxes.Add(roomtemp);
            //  allTextBoxes.Add(testdate);
            allTextBoxes.Add(txtDiameter);
            allTextBoxes.Add(txtThickness);
            allTextBoxes.Add(txtVolumeResistivity);

            allTextBoxes.Add(txtStandardmaxVolumeResitstivity);
            allTextBoxes.Add(txtStandardmaxVolumeResitstivity_conductor);

            // Reset border for each TextBox
            foreach (var textBox in allTextBoxes)
            {
                textBox.BorderColor = System.Drawing.Color.Empty; // Default color
                textBox.BorderStyle = BorderStyle.NotSet; // Default style
                textBox.BorderWidth = 0; // Default width
            }
        }
        private string[] SplitAndTrim(string input)
        {
            return string.IsNullOrWhiteSpace(input)
                ? new[] { string.Empty }
                : input.Split(',').Select(s => s.Trim()).ToArray();
        }
        private string GetArrayValue(string[] array, int index)
        {
            return index < array.Length ? array[index] : array[0];
        }
    }
}