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
    public partial class ColdBendingTest : System.Web.UI.Page
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
                txtSpecimenNo.Text = "1";
                CheckAndPrefillForm();
                PopulateCoreDropdown();
            }
        }

        private void PopulateCoreDropdown()
        {
            try
            {
                DataTable coreData = _cm.Get_Core();
                ddlCorePhase.Items.Clear();
                if (coreData != null && coreData.Rows.Count > 0)
                {// Bind data to dropdown
                    foreach (DataRow row in coreData.Rows)
                    {
                        string coreName = row["CoreName"].ToString();
                        string coreId = row["CoreID"].ToString();
                        ddlCorePhase.Items.Add(new ListItem(coreName, coreId));
                    }
                }
            }
            catch (Exception ex)
            {// Log error if needed
                ScriptManager.RegisterStartupScript(this, GetType(), "DropdownError", $"alert('Error loading core data: {ex.Message}');", true);
            }
        }

        private void InitializeGrid()
        {
            // Create a DataTable to store the test results
            DataTable dt = new DataTable();
            dt.Columns.Add("CorePhase", typeof(string));
            dt.Columns.Add("SpecimenNo", typeof(int));
            dt.Columns.Add("Diameter", typeof(string));
            dt.Columns.Add("Thickness", typeof(string));
            dt.Columns.Add("Beforeheating", typeof(string));
            dt.Columns.Add("afterheating", typeof(string));
            dt.Columns.Add("Temperature", typeof(string));
            dt.Columns.Add("Duration", typeof(string));
            dt.Columns.Add("result", typeof(string));

            // Store the DataTable in ViewState for persistence across postbacks
            ViewState["TestResults"] = dt;

            // Bind the empty DataTable to the GridView
            gvShrinkageTest.DataSource = dt;
            gvShrinkageTest.DataBind();
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
                    DataTable existingData = _cm.GetExistingTestResultsForSample(sampleId, reportId);

                    if (existingData != null && existingData.Rows.Count > 0)
                    {
                        // Show update button since data exists
                        btnSaveData.Visible = false;
                        btnClearData.Visible = false;
                        btnUpdateData.Visible = true;

                        // Prefill upper section details
                        PrefillUpperSection(existingData);
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

        private void PrefillUpperSection(DataTable existingData)
        {
            SetTextBoxText(cableType, "14196");
            SetTextBoxText(cableSize, "14197");
            SetTextBoxText(voltage, "14198");
            SetTextBoxText(Date, "14199");
            SetTextBoxText(roomno, "14200");
            SetTextBoxText(roomtemp, "14201");
            SetTextBoxText(refSpec, "14202");
            SetTextBoxText(testMethodNo, "14203");
            SetTextBoxText(jobOrderNo, "14204");
            SetTextBoxText(worksheetno, "14205");
            SetTextBoxText(roomhumidity, "14206");
            SetTextBoxText(equipmentno, "14207");

            SetTextBoxText(txtSpecificBeforeHeating, "14218");
            SetTextBoxText(txtSpecificAfterHeating, "14219");
            SetTextBoxText(txtSpecificTemperature, "14220");
            SetTextBoxText(txtSpecificDuration, "14221");
            SetTextBoxText(txtSpecificResult, "14222");
            SetTextBoxText(txtitemtype, "14223");

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

        private void PrefillTestResultsGrid(int sampleId)
        {
            try
            {
                InitializeGrid();

                // Create DAL object if not exists
                if (_cm == null)
                    _cm = new DAL.CM();

                // Fetch existing test results for this sample
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    conn.Open();
                    string query = @"WITH ResultPivot AS (
                                            SELECT 
                                                SampleID,
                                                MAX(CASE WHEN ReportParamMapID = '14210' THEN Result END) AS Core,
                                                MAX(CASE WHEN ReportParamMapID = '14211' THEN Result END) AS Diameter,
                                                MAX(CASE WHEN ReportParamMapID = '14212' THEN Result END) AS Thickness,
                                                MAX(CASE WHEN ReportParamMapID = '14213' THEN Result END) AS DiameterOfMandrel,
                                                MAX(CASE WHEN ReportParamMapID = '14214' THEN Result END) AS TotalSpiral,
                                                MAX(CASE WHEN ReportParamMapID = '14215' THEN Result END) AS Temperature,
                                                MAX(CASE WHEN ReportParamMapID = '14216' THEN Result END) AS Duration,
                                                MAX(CASE WHEN ReportParamMapID = '14217' THEN Result END) AS Result,
                                                MAX(CASE WHEN ReportParamMapID = '14218' THEN Result END) AS SpecifiedDiameter,
                                                MAX(CASE WHEN ReportParamMapID = '14219' THEN Result END) AS SpecifiedTotalSpiral,
                                                MAX(CASE WHEN ReportParamMapID = '14220' THEN Result END) AS SpecifiedTemp,
                                                MAX(CASE WHEN ReportParamMapID = '14221' THEN Result END) AS SpecifiedTime,
                                                MAX(CASE WHEN ReportParamMapID = '14222' THEN Result END) AS SpecifiedResult
                                            FROM TestResultsPTQualis
                                            WHERE SampleID = @SampleID
                                            GROUP BY SampleID
                                        )
                                        SELECT 
                                            Core,
                                            '' AS SpecimenNo,Diameter,Thickness,DiameterOfMandrel,TotalSpiral,
                                            Temperature AS Temperature,
                                            Duration AS Duration,    
                                            Result AS Result,
	                                        SpecifiedDiameter,SpecifiedTotalSpiral,
                                            SpecifiedTemp AS InsulationSpecifiedTemperature,
                                            SpecifiedTime AS InsulationSpecifiedDuration,
                                            SpecifiedResult AS InsulationSpecifiedResult
                                        FROM ResultPivot;";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SampleID", sampleId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            DataTable dt = ViewState["TestResults"] as DataTable;
                            dt.Clear();

                            while (reader.Read())
                            {
                                string[] corePhases = SplitAndTrim(reader["Core"].ToString());
                                string[] temperature = SplitAndTrim(reader["Temperature"].ToString());
                                string[] duration = SplitAndTrim(reader["Duration"].ToString());
                                string[] beforeHeating = SplitAndTrim(reader["DiameterOfMandrel"].ToString());
                                string[] afterHeating = SplitAndTrim(reader["TotalSpiral"].ToString());
                                string[] results = SplitAndTrim(reader["Result"].ToString());
                                string[] diameter = SplitAndTrim(reader["Diameter"].ToString());
                                string[] thickness = SplitAndTrim(reader["Thickness"].ToString());

                                string currentCorePhase = null;
                                int corePhaseCount = 0;

                                for (int i = 0; i < corePhases.Length; i++)
                                {
                                    DataRow newRow = dt.NewRow();
                                    newRow["CorePhase"] = corePhases[i];
                                    // If CorePhase changed, reset the counter
                                    if (corePhases[i] != currentCorePhase)
                                    {
                                        currentCorePhase = corePhases[i];
                                        corePhaseCount = 1;
                                    }
                                    else
                                    {
                                        corePhaseCount++;
                                    }

                                    newRow["SpecimenNo"] = corePhaseCount;
                                    newRow["Diameter"] = GetArrayValue(diameter, i);
                                    newRow["Thickness"] = GetArrayValue(thickness, i);
                                    newRow["Temperature"] = GetArrayValue(temperature, i);
                                    newRow["Duration"] = GetArrayValue(duration, i);
                                    newRow["Beforeheating"] = GetArrayValue(beforeHeating, i);
                                    newRow["afterheating"] = GetArrayValue(afterHeating, i);
                                    newRow["result"] = GetArrayValue(results, i);
                                    dt.Rows.Add(newRow);
                                }

                                
                            }
                            // Rebind grid
                            gvShrinkageTest.DataSource = dt;
                            gvShrinkageTest.DataBind();
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

        protected void ddlTestType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSpecimenNo.Text = "1";
        }

        protected void ddlCorePhase_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSpecimenNo.Text = "1";
        }

        private void UpdateSpecimenNumber()
        {
            // Get the DataTable from ViewState
            DataTable dt = (DataTable)ViewState["TestResults"];
            // Calculate next specimen number
            int nextSpecimenNo = 1; // Default to 1 if no rows
            if (dt != null && dt.Rows.Count > 0)
            {
                nextSpecimenNo = dt.Rows.Count + 1;
            }
            TextBox txtSpecimenNo = FindControl("txtSpecimenNo") as TextBox;
            // Set the value if found
            if (txtSpecimenNo != null)
            {
                txtSpecimenNo.Text = nextSpecimenNo.ToString();
            }

        }

        protected void btnAddToGrid_Click(object sender, EventArgs e)
        {
            ResetControlBorders();

            bool hasError = false;
            List<Control> invalidControls = new List<Control>();

            int specimenNo = 0;
            decimal Diameter = 0, Thickness = 0, Temperature = 0, Duration = 0, Beforeheating = 0, afterheating = 0, result = 0;

            if (ddlCorePhase.SelectedIndex == -1 || string.IsNullOrWhiteSpace(ddlCorePhase.SelectedItem.Text))
            {
                ddlCorePhase.CssClass += " input-error";
                hasError = true;
            }
            if (string.IsNullOrWhiteSpace(txtSpecimenNo.Text) || !int.TryParse(txtSpecimenNo.Text, out specimenNo))
            {
                invalidControls.Add(txtSpecimenNo);
                hasError = true;
            }

            bool ParseOrDash(string input, out decimal value)
            {
                if (input == "-")
                {
                    value = 0;
                    return true;
                }
                return decimal.TryParse(input, out value);
            }

            if (!ParseOrDash(txtTemperature.Text, out Temperature)) { invalidControls.Add(txtTemperature); hasError = true; }
            if (!ParseOrDash(txtDuration.Text, out Duration)) { invalidControls.Add(txtDuration); hasError = true; }
            if (!ParseOrDash(txtDiameter.Text, out Diameter)) { invalidControls.Add(txtDiameter); hasError = true; }
            if (!ParseOrDash(txtThickness.Text, out Thickness)) { invalidControls.Add(txtThickness); hasError = true; }
            if (!ParseOrDash(txtBeforeheating.Text, out Beforeheating)) { invalidControls.Add(txtBeforeheating); hasError = true; }
            if (!ParseOrDash(txtafterheating.Text, out afterheating)) { invalidControls.Add(txtafterheating); hasError = true; }
            if (!ParseOrDash(txtresult.Text, out result)) { invalidControls.Add(txtresult); hasError = true; }

            foreach (var control in invalidControls)
            {
                if (control is TextBox tb)
                {
                    tb.BorderColor = System.Drawing.Color.FromArgb(255, 150, 150);
                    tb.BorderStyle = BorderStyle.Solid;
                    tb.BorderWidth = 1;
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
                dt.Columns.Add("CorePhase", typeof(string));
                dt.Columns.Add("SpecimenNo", typeof(int));
                dt.Columns.Add("Diameter", typeof(string));
                dt.Columns.Add("Thickness", typeof(string));
                dt.Columns.Add("Beforeheating", typeof(string));
                dt.Columns.Add("afterheating", typeof(string));
                dt.Columns.Add("Temperature", typeof(string));
                dt.Columns.Add("Duration", typeof(string));
                dt.Columns.Add("result", typeof(string));
            }

            // Check duplicates by CorePhase + SpecimenNo
            bool isDuplicate = dt.AsEnumerable().Any(row =>
                row.Field<string>("CorePhase") == ddlCorePhase.SelectedItem.Text &&
                row.Field<int>("SpecimenNo") == specimenNo);

            if (isDuplicate)
            {
                string core = ddlCorePhase.SelectedItem.Text;
                ShowMessage($"Duplicate entry for Core Phase '{core}' and Specimen No. '{specimenNo}' is not allowed.");
                txtSpecimenNo.Text = (specimenNo + 1).ToString();
                return;
            }
            string ToDisplay(string input, decimal val) => input == "-" ? "-" : val.ToString();

            DataRow dr = dt.NewRow();
            dr["CorePhase"] = ddlCorePhase.SelectedItem.Text;
            dr["SpecimenNo"] = specimenNo;
            dr["Diameter"] = ToDisplay(txtDiameter.Text, Diameter);
            dr["Thickness"] = ToDisplay(txtThickness.Text, Thickness);
            dr["Beforeheating"] = ToDisplay(txtBeforeheating.Text, Beforeheating);
            dr["afterheating"] = ToDisplay(txtafterheating.Text, afterheating);
            dr["Temperature"] = ToDisplay(txtTemperature.Text, Temperature);
            dr["Duration"] = ToDisplay(txtDuration.Text, Duration);
            dr["result"] = ToDisplay(txtresult.Text, result);

            dt.Rows.Add(dr);
            ViewState["TestResults"] = dt;
            gvShrinkageTest.DataSource = dt;
            gvShrinkageTest.DataBind();
            txtSpecimenNo.Text = (specimenNo + 1).ToString();
            ClearInputFields();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            List<string> errors = new List<string>();
            List<Control> invalidControls = new List<Control>();

            // Check if Grid has data
            bool gridHasData = (gvShrinkageTest.Rows.Count > 0);
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
            if (string.IsNullOrEmpty(roomhumidity.Text) || !IsValidNumberOrDash(roomhumidity.Text))
            {
                errors.Add("Room Humidity must be a valid number or '-'.");
                invalidControls.Add(roomhumidity);
            }
            if (string.IsNullOrEmpty(roomtemp.Text) || !IsValidNumberOrDash(roomtemp.Text))
            {
                errors.Add("Room Temp. must be a valid number or '-'.");
                invalidControls.Add(roomtemp);
            }

            // Only validate the upper fields if grid has no data
            if (!gridHasData)
            {
                if (string.IsNullOrEmpty(txtTemperature.Text) || !IsValidNumberOrDash(txtTemperature.Text))
                {
                    errors.Add("Width Diameter must be a valid number or '-'.");
                    invalidControls.Add(txtTemperature);
                }
                if (string.IsNullOrEmpty(txtDuration.Text) || !IsValidNumberOrDash(txtDuration.Text))
                {
                    errors.Add("Thickness must be a valid number or '-'.");
                    invalidControls.Add(txtDuration);
                }
                if (string.IsNullOrEmpty(txtBeforeheating.Text) || !IsValidNumberOrDash(txtBeforeheating.Text))
                {
                    errors.Add("Volume Resistivity value must be a valid number or '-'.");
                    invalidControls.Add(txtBeforeheating);
                }
                if (string.IsNullOrEmpty(txtafterheating.Text) || !IsValidNumberOrDash(txtafterheating.Text))
                {
                    errors.Add("After heating value must be a valid number or '-'.");
                    invalidControls.Add(txtafterheating);
                }
                if (string.IsNullOrEmpty(txtresult.Text) || !IsValidNumberOrDash(txtresult.Text))
                {
                    errors.Add("Result value must be a valid number or '-'.");
                    invalidControls.Add(txtresult);
                }
            }

            // Validate Standard Values section
            if (string.IsNullOrEmpty(txtSpecificTemperature.Text) || !IsValidNumberOrDash(txtSpecificTemperature.Text))
            {
                errors.Add("Specific Temperature must be a valid number or '-'.");
                invalidControls.Add(txtSpecificTemperature);
            }
            if (string.IsNullOrEmpty(txtSpecificDuration.Text) || !IsValidNumberOrDash(txtSpecificDuration.Text))
            {
                errors.Add("Specific Duration must be a valid number or '-'.");
                invalidControls.Add(txtSpecificDuration);
            }
            if (string.IsNullOrEmpty(txtSpecificResult.Text) || !IsValidNumberOrDash(txtSpecificResult.Text))
            {
                errors.Add("Specific Result must be a valid number or '-'.");
                invalidControls.Add(txtSpecificResult);
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
                    // Show success message
                    ScriptManager.RegisterStartupScript(this, GetType(), "SaveSuccess",
                        "alert('Test results and Report Generated successfully!');", true);
                    // Clear form
                    ClearForm();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "SaveFailed",
                        "alert('Test results were not saved due to an error.');", true);
                }
            }
        }

        protected void btnClearData_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private bool Submitform()
        {
            try
            {
                DataTable dt = ViewState["TestResults"] as DataTable;

                if (dt == null || dt.Rows.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", "alert('No data to save. Please add test results first.');", true);
                    return false;
                }

                if (_cm == null) _cm = new DAL.CM();
                if (Request.QueryString["s"] != null) { _cm.sampleId = Convert.ToInt32(Request.QueryString["s"].ToLower()); }
                if (Request.QueryString["r"] != null) { _cm.ReportID = Request.QueryString["r"]; }

                DataTable dt1 = _cm.Getdataforseriesandreportid_mechanical(_cm);
                if (dt1.Rows.Count > 0)
                {
                    _cm.sampleIDSeries = dt1.Rows[0]["sampleIDSeries"].ToString();
                    _cm.CRFSampleId = Convert.ToInt32(dt1.Rows[0]["CRFSampleId"]);
                }

                List<string> reportParamMapList = new List<string>();
                List<string> resultList = new List<string>();

                // Collect all core phases in order
                string corePhases = string.Join(",", dt.AsEnumerable().Select(r => r["CorePhase"].ToString()));

                string diameter = string.Join(",", dt.AsEnumerable().Select(r => r["Diameter"].ToString()));

                string thickness = string.Join(",", dt.AsEnumerable().Select(r => r["Thickness"].ToString()));

                // Collect all temperature values in order
                string temperature = string.Join(",", dt.AsEnumerable().Select(r => r["Temperature"].ToString()));

                // Collect all duration values in order
                string duration = string.Join(",", dt.AsEnumerable().Select(r => r["Duration"].ToString()));

                // Collect all before heating values in order
                string beforeHeating = string.Join(",", dt.AsEnumerable().Select(r => r["Beforeheating"].ToString()));

                // Collect all after heating values in order
                string afterHeating = string.Join(",", dt.AsEnumerable().Select(r => r["afterheating"].ToString()));

                // Collect all result values in order
                string results = string.Join(",", dt.AsEnumerable().Select(r => r["result"].ToString()));

                //Upper section
                reportParamMapList.Add("14196"); resultList.Add(cableType.Text);
                reportParamMapList.Add("14197"); resultList.Add(cableSize.Text);
                reportParamMapList.Add("14198"); resultList.Add(voltage.Text);
                reportParamMapList.Add("14199"); resultList.Add(Date.Text);
                reportParamMapList.Add("14200"); resultList.Add(roomno.Text);
                reportParamMapList.Add("14201"); resultList.Add(roomtemp.Text);
                reportParamMapList.Add("14202"); resultList.Add(refSpec.Text);
                reportParamMapList.Add("14203"); resultList.Add(testMethodNo.Text);
                reportParamMapList.Add("14204"); resultList.Add(jobOrderNo.Text);
                reportParamMapList.Add("14205"); resultList.Add(worksheetno.Text);
                reportParamMapList.Add("14206"); resultList.Add(roomhumidity.Text);
                reportParamMapList.Add("14207"); resultList.Add(equipmentno.Text);
                // Add core phase data ID
                reportParamMapList.Add("14210"); resultList.Add(corePhases);
                // Add other measurements
                reportParamMapList.Add("14211"); resultList.Add(diameter);
                reportParamMapList.Add("14212"); resultList.Add(thickness);                
                reportParamMapList.Add("14213"); resultList.Add(beforeHeating);
                reportParamMapList.Add("14214"); resultList.Add(afterHeating);
                reportParamMapList.Add("14215"); resultList.Add(temperature);
                reportParamMapList.Add("14216"); resultList.Add(duration);
                reportParamMapList.Add("14217"); resultList.Add(results);
                // Add Standard values
                reportParamMapList.Add("14218"); resultList.Add(txtSpecificBeforeHeating.Text);
                reportParamMapList.Add("14219"); resultList.Add(txtSpecificAfterHeating.Text);
                reportParamMapList.Add("14220"); resultList.Add(txtSpecificTemperature.Text);
                reportParamMapList.Add("14221"); resultList.Add(txtSpecificDuration.Text);
                reportParamMapList.Add("14222"); resultList.Add(txtSpecificResult.Text);
                reportParamMapList.Add("14223"); resultList.Add(txtitemtype.Text);
                

                // Convert lists to arrays
                string[] reportParamMap = reportParamMapList.ToArray();
                string[] result = resultList.ToArray();

                // Call the database save method
                int modified = _cm.AddTestResultPTQualis(_cm, reportParamMap, result);

                if (modified > 0)
                {
                    // Show success message
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showSuccess", "alert('Test results and Report Generated successfully!');", true);
                    _cm.sampleStatusID = 27;//Test Result Submitted
                    _cm.UpdateCRFSampleDetailsStatus(_cm);

                    _cm.StatusID = 27;
                    _cm.UpdateAllocationStatusid_mechanical(_cm);

                    Response.Redirect($"ColdBendingTest_GR.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
                    return true;
                }
                else
                {
                    // Show error message
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", "alert('Failed to save test results.');", true);
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Log error and show message
                ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", $"alert('Error: {ex.Message}');", true);
                return false;
            }
        }

        private bool IsValidNumberOrDash(string input)
        {
            return input == "-" || double.TryParse(input, out _);
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            List<string> errors = new List<string>();
            List<Control> invalidControls = new List<Control>();

            // Check if Grid has data
            bool gridHasData = (gvShrinkageTest.Rows.Count > 0);

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
            if (string.IsNullOrEmpty(roomhumidity.Text) || !IsValidNumberOrDash(roomhumidity.Text))
            {
                errors.Add("Room Humidity must be a valid number or '-'.");
                invalidControls.Add(roomhumidity);
            }
            if (string.IsNullOrEmpty(roomtemp.Text) || !IsValidNumberOrDash(roomtemp.Text))
            {
                errors.Add("Room Temp. must be a valid number or '-'.");
                invalidControls.Add(roomtemp);
            }

            // Only validate the upper fields if grid has no data
            if (!gridHasData)
            {
                if (string.IsNullOrEmpty(txtSpecimenNo.Text))
                {
                    errors.Add("Specimen No. is required.");
                    invalidControls.Add(txtSpecimenNo);
                }
                if (string.IsNullOrEmpty(txtTemperature.Text) || !IsValidNumberOrDash(txtTemperature.Text))
                {
                    errors.Add("Temperature must be a valid number or '-'.");
                    invalidControls.Add(txtTemperature);
                }
                if (string.IsNullOrEmpty(txtDuration.Text) || !IsValidNumberOrDash(txtDuration.Text))
                {
                    errors.Add("Duration must be a valid number or '-'.");
                    invalidControls.Add(txtDuration);
                }
                if (string.IsNullOrEmpty(txtBeforeheating.Text) || !IsValidNumberOrDash(txtBeforeheating.Text))
                {
                    errors.Add("Before heating value must be a valid number or '-'.");
                    invalidControls.Add(txtBeforeheating);
                }
                if (string.IsNullOrEmpty(txtafterheating.Text) || !IsValidNumberOrDash(txtafterheating.Text))
                {
                    errors.Add("After heating value must be a valid number or '-'.");
                    invalidControls.Add(txtafterheating);
                }
                if (string.IsNullOrEmpty(txtresult.Text) || !IsValidNumberOrDash(txtresult.Text))
                {
                    errors.Add("Result value must be a valid number or '-'.");
                    invalidControls.Add(txtresult);
                }
            }

            // Validate Standard Values section
            if (string.IsNullOrEmpty(txtSpecificTemperature.Text) || !IsValidNumberOrDash(txtSpecificTemperature.Text))
            {
                errors.Add("Specific Temperature must be a valid number or '-'.");
                invalidControls.Add(txtSpecificTemperature);
            }
            if (string.IsNullOrEmpty(txtSpecificDuration.Text) || !IsValidNumberOrDash(txtSpecificDuration.Text))
            {
                errors.Add("Specific Duration must be a valid number or '-'.");
                invalidControls.Add(txtSpecificDuration);
            }
            if (string.IsNullOrEmpty(txtSpecificResult.Text) || !IsValidNumberOrDash(txtSpecificResult.Text))
            {
                errors.Add("Specific Result must be a valid number or '-'.");
                invalidControls.Add(txtSpecificResult);
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

        //Grids
        protected void gvShrinkageTest_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataTable dt = ViewState["TestResults"] as DataTable;
                if (dt == null) return;

                int rowIndex = e.Row.RowIndex;

                // Merge ONLY the first column (CorePhase) if values match the previous row
                if (rowIndex > 0 && dt.Rows[rowIndex]["CorePhase"].ToString() == dt.Rows[rowIndex - 1]["CorePhase"].ToString())
                {
                    ProcessColumn(dt, rowIndex, e.Row, 0); // Column 0 = CorePhase
                }
            }
        }

        private void ProcessColumn(DataTable dt, int currentRowIndex, GridViewRow currentRow, int columnIndex)
        {
            try
            {
                string columnName = dt.Columns[columnIndex].ColumnName;
                string currentValue = dt.Rows[currentRowIndex][columnName].ToString();
                string previousValue = dt.Rows[currentRowIndex - 1][columnName].ToString();

                if (currentValue == previousValue)
                {
                    // Hide the current cell
                    currentRow.Cells[columnIndex].Visible = false;

                    // Find the first visible row above with the same value
                    int spanRowIndex = currentRowIndex - 1;
                    while (spanRowIndex >= 0 && !gvShrinkageTest.Rows[spanRowIndex].Cells[columnIndex].Visible)
                    {
                        spanRowIndex--;
                    }

                    if (spanRowIndex >= 0)
                    {
                        // Increase rowspan of the visible cell
                        if (gvShrinkageTest.Rows[spanRowIndex].Cells[columnIndex].RowSpan < 2)
                        {
                            gvShrinkageTest.Rows[spanRowIndex].Cells[columnIndex].RowSpan = 2;
                        }
                        else
                        {
                            gvShrinkageTest.Rows[spanRowIndex].Cells[columnIndex].RowSpan++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in ProcessColumn: " + ex.Message);
            }
        }

        protected void gvShrinkageTest_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                DataTable dt = (DataTable)ViewState["TestResults"];
                dt.Rows.RemoveAt(rowIndex);
                ViewState["TestResults"] = dt;
                gvShrinkageTest.DataSource = dt;
                gvShrinkageTest.DataBind();

                // Update the Specimen No. in the upper section
                UpdateSpecimenNumber();
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

        private void ClearInputFields()
        {
            txtTemperature.Text = "";
            txtDuration.Text = "";
            txtBeforeheating.Text = "";
            txtafterheating.Text = "";
            txtresult.Text = "";
            txtDiameter.Text = "";
            txtThickness.Text = "";
        }

        private void ShowMessage(string message)
        {
            string safeMessage = HttpUtility.JavaScriptStringEncode(message);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('{safeMessage}');", true);
        }

        private void ResetControlBorders()
        {
            // Reset all TextBox controls
            List<TextBox> allTextBoxes = new List<TextBox>();

            // Add all TextBox controls from the form
            allTextBoxes.Add(Date);
            allTextBoxes.Add(roomno);
            allTextBoxes.Add(roomhumidity);
            allTextBoxes.Add(roomtemp);
            // allTextBoxes.Add(testdate);
            allTextBoxes.Add(txtSpecimenNo);
            allTextBoxes.Add(txtTemperature);
            allTextBoxes.Add(txtDuration);
            allTextBoxes.Add(txtBeforeheating);
            allTextBoxes.Add(txtafterheating);
            allTextBoxes.Add(txtresult);

            // Reset border for each TextBox
            foreach (var textBox in allTextBoxes)
            {
                textBox.BorderColor = System.Drawing.Color.Empty; // Default color
                textBox.BorderStyle = BorderStyle.NotSet; // Default style
                textBox.BorderWidth = 0; // Default width
            }
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
            // ddlTestType.SelectedIndex = 0;
            ddlCorePhase.SelectedIndex = 0;
            txtTemperature.Text = "";
            txtDuration.Text = "";
            txtBeforeheating.Text = "";
            txtafterheating.Text = "";
            txtresult.Text = "";
            // txtMeasured.Text = "";

            txtDiameter.Text = "";
            txtThickness.Text = "";


            // Clear GridView
            gvShrinkageTest.DataSource = null;
            gvShrinkageTest.DataBind();

            // Standard Values
            txtSpecificTemperature.Text = "";

            txtSpecificDuration.Text = "";
            txtSpecificResult.Text = "";
            // Ageing Specimen & Cable

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