using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using System.Text;
using LIMS.DAL;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace LIMS
{
    public partial class Impulsetest2 : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                List<string> phases = new List<string> { "Red", "Blue", "Yellow" };
                Session["Phases"] = phases;
                Session["CurrentPhaseIndex"] = 0; // Start with the first phase (Red)
                Session["CurrentTestType"] = "Positive"; // Start with Positive test

                // Initialize test data dictionary
                Dictionary<string, Dictionary<string, Dictionary<string, string>>> allTestData =
                    new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
                Session["TestData"] = allTestData;

                // Always hide Previous button
                btnPrevious.Visible = false;

                // Initialize the form
                InitializeForm();
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
                    lblvoltage.Text = dt1.Rows[0]["Voltage"].ToString();
                    cableSize.Text = dt1.Rows[0]["SizeOfSample"].ToString();
                    testMethodNo.Text = dt1.Rows[0]["TestMethod"].ToString();
                    refSpec.Text = dt1.Rows[0]["TestMethod"].ToString();
                    // Upper section
                    roomno.Text = "SHIELDING";
                    jobOrderNo.Text = "WO/2024120905";
                    worksheetno.Text = "-";
                    equipmentno.Text = "QE/15/EL/33";
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showError", "alert('Could not retrieve sample data. Please check your sample ID.');", true);
                }

                // Check and prefill form if sample ID exists
                CheckAndPrefillForm();

                // Update button visibility based on current state
                UpdateNavigationButtons();
            }
        }


        //Buttons
        protected void btnNext_Click(object sender, EventArgs e)
        {
            if (!ValidateMeasuredValues())
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(),
                    "ValidationError",
                    "alert('Please enter valid numeric values for all measured fields');",
                    true);
                return;
            }
            // Save current form data
            SaveCurrentData();
            List<string> phases = Session["Phases"] as List<string>;
            int currentPhaseIndex = (int)Session["CurrentPhaseIndex"];
            string currentTestType = Session["CurrentTestType"].ToString();

            // Determine next action based on current test type
            if (currentTestType == "Positive")
            {
                // Move to negative test for the same phase
                Session["CurrentTestType"] = "Negative";
                InitializeForm();
            }
            else // currentTestType == "Negative"
            {
                // Move to the next phase
                if (currentPhaseIndex < phases.Count - 1)
                {
                    Session["CurrentPhaseIndex"] = currentPhaseIndex + 1;
                    Session["CurrentTestType"] = "Positive"; // Reset to positive for new phase
                    InitializeForm();
                    // Show message to indicate transition to new phase
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "PhaseChangeScript",
                        "alert('Now proceeding to " + phases[currentPhaseIndex + 1] + " Phase - Positive Impulse Test');",
                        true);
                }
                else
                {
                    // We're done with all phases
                    // Show completion message
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "CompletionScript",
                        "alert('All phases and tests are complete. Please review and update your data.');",
                        true);
                }
            }

            // Update navigation buttons to show only Next and Update as appropriate
            UpdateNavigationButtons();
        }
        protected void btnPrevious_Click(object sender, EventArgs e)
        {
            // Save current form data
            SaveCurrentData();

            List<string> phases = Session["Phases"] as List<string>;
            int currentPhaseIndex = (int)Session["CurrentPhaseIndex"];
            string currentTestType = Session["CurrentTestType"].ToString();

            // Configure test fields based on test type
            if (currentTestType == "Positive")
            {
                SetupPositiveImpulseTest();
            }
            else
            {
                SetupNegativeImpulseTest();
            }
            // Determine previous action based on current test type
            if (currentTestType == "Negative")
            {
                // Move back to positive test for the same phase
                Session["CurrentTestType"] = "Positive";
                InitializeForm();
            }
            else // currentTestType == "Positive"
            {
                // Move to the previous phase's negative test
                if (currentPhaseIndex > 0)
                {
                    Session["CurrentPhaseIndex"] = currentPhaseIndex - 1;
                    Session["CurrentTestType"] = "Negative"; // Move to negative of previous phase
                    InitializeForm();
                }
            }

            // Adjust navigation buttons
            UpdateNavigationButtons();
        }
        protected void btnClearData_Click(object sender, EventArgs e)
        {
            // Clear form fields
            ClearFormFields();

            // Clear session data for current phase and test type
            List<string> phases = Session["Phases"] as List<string>;
            int currentPhaseIndex = (int)Session["CurrentPhaseIndex"];
            string currentTestType = Session["CurrentTestType"].ToString();

            if (currentPhaseIndex >= 0 && currentPhaseIndex < phases.Count)
            {
                string currentPhase = phases[currentPhaseIndex];
                Dictionary<string, Dictionary<string, Dictionary<string, string>>> allTestData =
                    Session["TestData"] as Dictionary<string, Dictionary<string, Dictionary<string, string>>>;

                if (allTestData.ContainsKey(currentPhase) &&
                    allTestData[currentPhase].ContainsKey(currentTestType))
                {
                    // Remove just this phase and test type data
                    allTestData[currentPhase].Remove(currentTestType);

                    // Update session
                    Session["TestData"] = allTestData;
                }
            }

            // Show confirmation message
            ScriptManager.RegisterStartupScript(this, this.GetType(),
                "ClearDataScript", "alert('Test data has been cleared.');", true);
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateMeasuredValues())
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "ValidationError",
                        "alert('Please enter valid numeric values for all measured fields');",
                        true);
                    return;
                }
                // Save the current phase and test type data first
                SaveCurrentData();

                // Process all collected data
                Dictionary<string, Dictionary<string, Dictionary<string, string>>> allTestData =
                    Session["TestData"] as Dictionary<string, Dictionary<string, Dictionary<string, string>>>;

                // Initialize DAL object if needed
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

                // Get data for series and report ID
                DataTable dt1 = _cm.Getdataforseriesandreportid_mechanical(_cm);
                if (dt1 != null && dt1.Rows.Count > 0)
                {
                    _cm.sampleIDSeries = dt1.Rows[0]["sampleIDSeries"].ToString();
                    _cm.CRFSampleId = Convert.ToInt32(dt1.Rows[0]["CRFSampleId"]);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showError", "alert('Could not retrieve sample data. Please check your sample ID.');", true);
                    return;
                }

                // Prepare arrays for the AddTestResultGlobal method
                List<string> reportParamMapList = new List<string>();
                List<string> resultList = new List<string>();

                // Use values from the form or if available, from stored values
                string roomNoValue = roomno.Text?.Trim() ?? string.Empty;
                string roomHumidityValue = roomhumidity.Text?.Trim() ?? string.Empty;
                string roomTempValue = roomtemp.Text?.Trim() ?? string.Empty;
                string dateValue = Date.Text?.Trim() ?? string.Empty;
                string cableTypeValue = cableType.Text?.Trim() ?? string.Empty;
                string refSpecValue = refSpec.Text?.Trim() ?? string.Empty;
                string cableSizeValue = cableSize.Text?.Trim() ?? string.Empty;
                string testMethodNoValue = testMethodNo.Text?.Trim() ?? string.Empty;
                string voltageValue = lblvoltage.Text?.Trim() ?? string.Empty;
                string jobOrderNoValue = jobOrderNo.Text?.Trim() ?? string.Empty;
                string worksheetNoValue = worksheetno.Text?.Trim() ?? string.Empty;
                string equipmentNoValue = equipmentno.Text?.Trim() ?? string.Empty;

                // Upper Section - common for all phases
                reportParamMapList.Add("7212"); resultList.Add(cableTypeValue);
                reportParamMapList.Add("7214"); resultList.Add(refSpecValue);
                reportParamMapList.Add("7215"); resultList.Add(cableSizeValue);
                reportParamMapList.Add("7217"); resultList.Add(testMethodNoValue);
                reportParamMapList.Add("7216"); resultList.Add(voltageValue);
                reportParamMapList.Add("7218"); resultList.Add(jobOrderNoValue);
                reportParamMapList.Add("7220"); resultList.Add(worksheetNoValue);
                reportParamMapList.Add("7224"); resultList.Add(equipmentNoValue);
                reportParamMapList.Add("7221"); resultList.Add(roomNoValue);
                reportParamMapList.Add("7222"); resultList.Add(roomHumidityValue);
                reportParamMapList.Add("7223"); resultList.Add(roomTempValue);
                reportParamMapList.Add("7219"); resultList.Add(dateValue);

                // Process phase data
                if (allTestData != null && allTestData.Count > 0)
                {
                    // Dictionary to hold combined data across all phases
                    Dictionary<string, Dictionary<string, StringBuilder>> combinedData = new Dictionary<string, Dictionary<string, StringBuilder>>
            {
                { "Positive", new Dictionary<string, StringBuilder>() },
                { "Negative", new Dictionary<string, StringBuilder>() }
            };

                    // Initialize the dictionaries
                    foreach (var testType in combinedData.Keys)
                    {
                        combinedData[testType]["ImpulseVoltageMeasured"] = new StringBuilder();
                        combinedData[testType]["PeakVoltageMeasured"] = new StringBuilder();
                        combinedData[testType]["FrontTimeMeasured"] = new StringBuilder();
                        combinedData[testType]["TailTimeMeasured"] = new StringBuilder();
                        combinedData[testType]["ConductorTempMeasured"] = new StringBuilder();
                        combinedData[testType]["TestResultMeasured"] = new StringBuilder();
                        combinedData[testType]["Remarks"] = new StringBuilder();
                    }

                    // Phase counter for pipe separator
                    int phaseCount = 0;

                    // Debug logging
                    System.Diagnostics.Debug.WriteLine("Session Test Data Structure:");
                    foreach (var phase in allTestData.Keys)
                    {
                        System.Diagnostics.Debug.WriteLine($"Phase: {phase}");
                        foreach (var testType in allTestData[phase].Keys)
                        {
                            System.Diagnostics.Debug.WriteLine($"  TestType: {testType}");
                            foreach (var key in allTestData[phase][testType].Keys)
                            {
                                System.Diagnostics.Debug.WriteLine($"    {key}: {allTestData[phase][testType][key]}");
                            }
                        }
                    }

                    // Combine data from all phases
                    foreach (var phase in allTestData.Keys)
                    {
                        // Add pipe separator if not the first phase
                        string phaseSeparator = phaseCount > 0 ? "|" : "";

                        foreach (var testType in allTestData[phase].Keys)
                        {
                            // Critical fix - Map test type correctly
                            string dbTestType;
                            if (testType.ToLower().Contains("positive"))
                            {
                                dbTestType = "Positive";
                            }
                            else if (testType.ToLower().Contains("negative"))
                            {
                                dbTestType = "Negative";
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"Warning: Unknown test type: {testType}");
                                continue;
                            }

                            Dictionary<string, string> testData = allTestData[phase][testType];

                            System.Diagnostics.Debug.WriteLine($"Processing Phase: {phase}, TestType: {testType} -> DbTestType: {dbTestType}");

                            // Add phase data with pipe separator
                            if (testData.ContainsKey("ImpulseVoltageMeasured"))
                                combinedData[dbTestType]["ImpulseVoltageMeasured"].Append(phaseSeparator + testData["ImpulseVoltageMeasured"]);

                            if (testData.ContainsKey("PeakVoltageMeasured"))
                                combinedData[dbTestType]["PeakVoltageMeasured"].Append(phaseSeparator + testData["PeakVoltageMeasured"]);

                            if (testData.ContainsKey("FrontTimeMeasured"))
                                combinedData[dbTestType]["FrontTimeMeasured"].Append(phaseSeparator + testData["FrontTimeMeasured"]);

                            if (testData.ContainsKey("TailTimeMeasured"))
                                combinedData[dbTestType]["TailTimeMeasured"].Append(phaseSeparator + testData["TailTimeMeasured"]);

                            if (testData.ContainsKey("ConductorTempMeasured"))
                                combinedData[dbTestType]["ConductorTempMeasured"].Append(phaseSeparator + testData["ConductorTempMeasured"]);

                            if (testData.ContainsKey("TestResultMeasured"))
                                combinedData[dbTestType]["TestResultMeasured"].Append(phaseSeparator + testData["TestResultMeasured"]);

                            // Combine all remarks fields with proper separators
                            StringBuilder phaseRemarks = new StringBuilder();
                            for (int i = 1; i <= 6; i++)
                            {
                                string remarkKey = $"Remarks{i}";
                                if (testData.ContainsKey(remarkKey))
                                {
                                    if (phaseRemarks.Length > 0)
                                        phaseRemarks.Append(",");
                                    phaseRemarks.Append(testData[remarkKey]);
                                }
                            }

                            combinedData[dbTestType]["Remarks"].Append(phaseSeparator + phaseRemarks.ToString());
                        }

                        phaseCount++;
                    }

                    // Add the core/phase value
                    List<string> phases = Session["Phases"] as List<string>;
                    string corePhaseValue = string.Join("|", phases);
                    reportParamMapList.Add("7213"); resultList.Add(corePhaseValue);

                    // Debug - Output combined data for verification
                    System.Diagnostics.Debug.WriteLine("Combined Data Before Database Entry:");
                    foreach (var testType in combinedData.Keys)
                    {
                        System.Diagnostics.Debug.WriteLine($"TestType: {testType}");
                        foreach (var measurement in combinedData[testType].Keys)
                        {
                            System.Diagnostics.Debug.WriteLine($"  {measurement}: {combinedData[testType][measurement]}");
                        }
                    }

                    // Positive impulse test data
                    string positiveImpulseVoltage = combinedData["Positive"]["ImpulseVoltageMeasured"].Length > 0 ?
                        combinedData["Positive"]["ImpulseVoltageMeasured"].ToString() : "";
                    reportParamMapList.Add("7225"); // WithstandVoltage_Positive
                    resultList.Add(positiveImpulseVoltage);

                    string positivePeakVoltage = combinedData["Positive"]["PeakVoltageMeasured"].Length > 0 ?
                        combinedData["Positive"]["PeakVoltageMeasured"].ToString() : "";
                    reportParamMapList.Add("7226"); // PeakVoltage_Positive
                    resultList.Add(positivePeakVoltage);

                    string positiveFrontTime = combinedData["Positive"]["FrontTimeMeasured"].Length > 0 ?
                        combinedData["Positive"]["FrontTimeMeasured"].ToString() : "";
                    reportParamMapList.Add("7227"); // FrontWaveTime_Positive
                    resultList.Add(positiveFrontTime);

                    string positiveTailTime = combinedData["Positive"]["TailTimeMeasured"].Length > 0 ?
                        combinedData["Positive"]["TailTimeMeasured"].ToString() : "";
                    reportParamMapList.Add("7228"); // TailWaveTime_Positive
                    resultList.Add(positiveTailTime);

                    string positiveConductorTemp = combinedData["Positive"]["ConductorTempMeasured"].Length > 0 ?
                        combinedData["Positive"]["ConductorTempMeasured"].ToString() : "";
                    reportParamMapList.Add("7229"); // ConductorTemp_Positive
                    resultList.Add(positiveConductorTemp);

                    string positiveTestResult = combinedData["Positive"]["TestResultMeasured"].Length > 0 ?
                        combinedData["Positive"]["TestResultMeasured"].ToString() : "";
                    reportParamMapList.Add("7230"); // TestResult_Positive
                    resultList.Add(positiveTestResult);

                    // Add remarks for positive impulse test
                    string positiveRemarks = combinedData["Positive"]["Remarks"].Length > 0 ?
                        combinedData["Positive"]["Remarks"].ToString() : "";
                    reportParamMapList.Add("7231"); // Remarks_Positive
                    resultList.Add(positiveRemarks);

                    // FIXED SECTION FOR NEGATIVE IMPULSE TEST DATA
                    // Important: Ensure we have negative test data before adding to database
                    bool hasNegativeData = false;

                    // Check if there are any non-empty values in the negative test data
                    if (combinedData["Negative"]["ImpulseVoltageMeasured"].Length > 0 ||
                        combinedData["Negative"]["PeakVoltageMeasured"].Length > 0 ||
                        combinedData["Negative"]["FrontTimeMeasured"].Length > 0 ||
                        combinedData["Negative"]["TailTimeMeasured"].Length > 0 ||
                        combinedData["Negative"]["ConductorTempMeasured"].Length > 0 ||
                        combinedData["Negative"]["TestResultMeasured"].Length > 0)
                    {
                        hasNegativeData = true;
                    }

                    // Debug log negative data
                    System.Diagnostics.Debug.WriteLine($"Has Negative Data: {hasNegativeData}");
                    System.Diagnostics.Debug.WriteLine($"Negative Impulse Voltage: {combinedData["Negative"]["ImpulseVoltageMeasured"]}");
                    System.Diagnostics.Debug.WriteLine($"Negative Peak Voltage: {combinedData["Negative"]["PeakVoltageMeasured"]}");
                    System.Diagnostics.Debug.WriteLine($"Negative Front Time: {combinedData["Negative"]["FrontTimeMeasured"]}");
                    System.Diagnostics.Debug.WriteLine($"Negative Tail Time: {combinedData["Negative"]["TailTimeMeasured"]}");
                    System.Diagnostics.Debug.WriteLine($"Negative Conductor Temp: {combinedData["Negative"]["ConductorTempMeasured"]}");
                    System.Diagnostics.Debug.WriteLine($"Negative Test Result: {combinedData["Negative"]["TestResultMeasured"]}");

                    // Add negative data FOLLOWING THE EXACT ORDER shown in the database
                    // Important: Follow the parameter order as displayed in the image

                    string negativeWithstandVoltage = combinedData["Negative"]["ImpulseVoltageMeasured"].Length > 0 ?
                        combinedData["Negative"]["ImpulseVoltageMeasured"].ToString() : "";
                    reportParamMapList.Add("7232"); // WithstandVoltage_Negative
                    resultList.Add(negativeWithstandVoltage);

                    string negativePeakVoltage = combinedData["Negative"]["PeakVoltageMeasured"].Length > 0 ?
                        combinedData["Negative"]["PeakVoltageMeasured"].ToString() : "";
                    reportParamMapList.Add("7233"); // PeakVoltage_Negative
                    resultList.Add(negativePeakVoltage);

                    string negativeFrontTime = combinedData["Negative"]["FrontTimeMeasured"].Length > 0 ?
                        combinedData["Negative"]["FrontTimeMeasured"].ToString() : "";
                    reportParamMapList.Add("7234"); // FrontWaveTime_Negative
                    resultList.Add(negativeFrontTime);

                    string negativeTailTime = combinedData["Negative"]["TailTimeMeasured"].Length > 0 ?
                        combinedData["Negative"]["TailTimeMeasured"].ToString() : "";
                    reportParamMapList.Add("7235"); // TailWaveTime_Negative
                    resultList.Add(negativeTailTime);

                    string negativeConductorTemp = combinedData["Negative"]["ConductorTempMeasured"].Length > 0 ?
                        combinedData["Negative"]["ConductorTempMeasured"].ToString() : "";
                    reportParamMapList.Add("7236"); // ConductorTemp_Negative
                    resultList.Add(negativeConductorTemp);

                    string negativeTestResult = combinedData["Negative"]["TestResultMeasured"].Length > 0 ?
                        combinedData["Negative"]["TestResultMeasured"].ToString() : "";
                    reportParamMapList.Add("7237"); // TestResult_Negative
                    resultList.Add(negativeTestResult);

                    // Add remarks for negative impulse test
                    string negativeRemarks = combinedData["Negative"]["Remarks"].Length > 0 ?
                        combinedData["Negative"]["Remarks"].ToString() : "";
                    reportParamMapList.Add("7238"); // Remarks_Negative
                    resultList.Add(negativeRemarks);

                    // Debug - Output final arrays before database insertion
                    System.Diagnostics.Debug.WriteLine("Final Parameters and Values for Database:");
                    for (int i = 0; i < reportParamMapList.Count; i++)
                    {
                        System.Diagnostics.Debug.WriteLine($"Parameter {reportParamMapList[i]}: {resultList[i]}");
                    }

                    // Convert lists to arrays
                    string[] reportParamMap = reportParamMapList.ToArray();
                    string[] result = resultList.ToArray();

                    // Call the database save method
                    int modified = _cm.AddTestResultPTQualis(_cm, reportParamMap, result);

                    if (modified > 0)
                    {
                        // Update sample status
                        _cm.sampleStatusID = 27; // Test Result Submitted
                        _cm.UpdateCRFSampleDetailsStatus(_cm);

                        _cm.StatusID = 27;
                        _cm.UpdateAllocationStatusid_mechanical(_cm);

                        // Generate reports
                        // Generate reports
                        string script = @"
                            alert('Test results saved successfully!');
                            setTimeout(function() {
                                window.open('GenerateTestRecord_positiveimpulsetest.aspx?s=" + _cm.sampleId + "&r=" + _cm.ReportID + @"', '_blank');
                            }, 100);
                            setTimeout(function() {
                                window.open('GenerateTestRecord_negativeimpulsetest.aspx?s=" + _cm.sampleId + "&r=" + _cm.ReportID + @"', '_blank');
                            }, 200);";

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "openWindows", script, true);

                        // Reset the form
                        ResetForm();
                    }
                    else
                    {
                        // Show error message
                        ScriptManager.RegisterStartupScript(this, this.GetType(),
                            "showError", "alert('Failed to save test results. No records were saved to the database.');", true);
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showError", "alert('No test data found. Please complete the test for at least one phase.');", true);
                }
            }
            catch (Exception ex)
            {
                // Log error details
                System.Diagnostics.Debug.WriteLine($"Error in btnSubmit_Click: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }

                // Show user-friendly error message
                ScriptManager.RegisterStartupScript(this, this.GetType(),
                    "showError", $"alert('An error occurred while saving data: {ex.Message.Replace("'", "\\'")}');", true);
            }
        }
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            List<string> errors = new List<string>();
            List<Control> invalidControls = new List<Control>();

            // Validate required fields
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
            if (string.IsNullOrEmpty(cableType.Text))
            {
                errors.Add("Cable Type is required.");
                invalidControls.Add(cableType);
            }
            if (string.IsNullOrEmpty(refSpec.Text))
            {
                errors.Add("Reference Specification is required.");
                invalidControls.Add(refSpec);
            }
            if (string.IsNullOrEmpty(cableSize.Text))
            {
                errors.Add("Cable Size is required.");
                invalidControls.Add(cableSize);
            }
            if (string.IsNullOrEmpty(testMethodNo.Text))
            {
                errors.Add("Test Method No. is required.");
                invalidControls.Add(testMethodNo);
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

                string errorMessage = "Validation Errors:\n• " + string.Join("\n• ", errors);
                ScriptManager.RegisterStartupScript(this, GetType(), "ValidationError",
                    $"alert('{errorMessage}');", true);
            }
            else
            {
                try
                {
                    if (Request.QueryString["s"] != null)
                    {
                        // First delete existing records for this sample
                        DeleteExistingRecords(Convert.ToInt32(Request.QueryString["s"]));

                        // Then save the updated data (reusing your btnSubmit_Click logic)
                        btnSubmit_Click(sender, e);

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

        private void DeleteExistingRecords(int sampleId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    conn.Open();

                    // Delete all existing test results for this sample
                    string deleteQuery = "DELETE FROM TestResultsPTQualis WHERE SampleID = @SampleID";

                    using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@SampleID", sampleId);
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
        private void ResetControlBorders()
        {
            // Reset all textbox borders to default
            foreach (Control control in this.Controls)
            {
                if (control is TextBox)
                {
                    ((TextBox)control).BorderColor = System.Drawing.Color.Empty;
                    ((TextBox)control).BorderStyle = BorderStyle.NotSet;
                    ((TextBox)control).BorderWidth = Unit.Empty;
                }
            }
        }

        // Helper Methods
        private void SaveCurrentData()
        {
            List<string> phases = Session["Phases"] as List<string>;
            int currentPhaseIndex = (int)Session["CurrentPhaseIndex"];
            string currentTestType = Session["CurrentTestType"]?.ToString();

            // Debug logging
            System.Diagnostics.Debug.WriteLine($"SaveCurrentData - Phase Index: {currentPhaseIndex}, Test Type: {currentTestType}");

            if (currentPhaseIndex >= 0 && currentPhaseIndex < phases.Count && !string.IsNullOrEmpty(currentTestType))
            {
                string currentPhase = phases[currentPhaseIndex];
                Dictionary<string, Dictionary<string, Dictionary<string, string>>> allTestData =
                    Session["TestData"] as Dictionary<string, Dictionary<string, Dictionary<string, string>>>;

                // Initialize data structure if needed
                if (allTestData == null)
                {
                    System.Diagnostics.Debug.WriteLine("Creating new TestData dictionary");
                    allTestData = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
                    Session["TestData"] = allTestData;
                }

                if (!allTestData.ContainsKey(currentPhase))
                {
                    System.Diagnostics.Debug.WriteLine($"Adding phase {currentPhase} to TestData");
                    allTestData[currentPhase] = new Dictionary<string, Dictionary<string, string>>();
                }

                if (!allTestData[currentPhase].ContainsKey(currentTestType))
                {
                    System.Diagnostics.Debug.WriteLine($"Adding test type {currentTestType} to phase {currentPhase}");
                    allTestData[currentPhase][currentTestType] = new Dictionary<string, string>();
                }

                Dictionary<string, string> testData = allTestData[currentPhase][currentTestType];

                // Save all form fields from pnlTestData panel
                if (pnlTestData.Visible)
                {
                    System.Diagnostics.Debug.WriteLine("Panel is visible, collecting test data");

                    // Initialize StringBuilders to collect test data
                    StringBuilder impulseVoltageMeasured = new StringBuilder();
                    StringBuilder peakVoltageMeasured = new StringBuilder();
                    StringBuilder frontTimeMeasured = new StringBuilder();
                    StringBuilder tailTimeMeasured = new StringBuilder();
                    StringBuilder conductorTempMeasured = new StringBuilder();
                    StringBuilder testResultMeasured = new StringBuilder();
                    StringBuilder remarks1 = new StringBuilder();
                    StringBuilder remarks2 = new StringBuilder();
                    StringBuilder remarks3 = new StringBuilder();
                    StringBuilder remarks4 = new StringBuilder();
                    StringBuilder remarks5 = new StringBuilder();
                    StringBuilder remarks6 = new StringBuilder();

                    // Test 1
                    impulseVoltageMeasured.Append(txtMeasuredImpulseVoltage1.Text);
                    peakVoltageMeasured.Append(txtMeasuredPeakVoltage1.Text);
                    frontTimeMeasured.Append(txtMeasuredFrontTime1.Text);
                    tailTimeMeasured.Append(txtMeasuredTailTime1.Text);
                    conductorTempMeasured.Append(txtMeasuredconductortemp1.Text);
                    testResultMeasured.Append(ddlTestResult1.SelectedValue);
                    remarks1.Append(txtRemark1_1.Text);
                    remarks2.Append(txtRemark1_2.Text);
                    remarks3.Append(txtRemark1_3.Text);
                    remarks4.Append(txtRemark1_4.Text);
                    remarks5.Append(txtRemark1_5.Text);
                    remarks6.Append(txtRemark1_6.Text);

                    // Test 2
                    impulseVoltageMeasured.Append("," + txtMeasuredImpulseVoltage2.Text);
                    peakVoltageMeasured.Append("," + txtMeasuredPeakVoltage2.Text);
                    frontTimeMeasured.Append("," + txtMeasuredFrontTime2.Text);
                    tailTimeMeasured.Append("," + txtMeasuredTailTime2.Text);
                    conductorTempMeasured.Append("," + txtMeasuredconductortemp2.Text);
                    testResultMeasured.Append("," + ddlTestResult2.SelectedValue);
                    remarks1.Append("," + txtRemark2_1.Text);
                    remarks2.Append("," + txtRemark2_2.Text);
                    remarks3.Append("," + txtRemark2_3.Text);
                    remarks4.Append("," + txtRemark2_4.Text);
                    remarks5.Append("," + txtRemark2_5.Text);
                    remarks6.Append("," + txtRemark2_6.Text);

                    // Test 3
                    impulseVoltageMeasured.Append("," + txtMeasuredImpulseVoltage3.Text);
                    peakVoltageMeasured.Append("," + txtMeasuredPeakVoltage3.Text);
                    frontTimeMeasured.Append("," + txtMeasuredFrontTime3.Text);
                    tailTimeMeasured.Append("," + txtMeasuredTailTime3.Text);
                    conductorTempMeasured.Append("," + txtMeasuredconductortemp3.Text);
                    testResultMeasured.Append("," + ddlTestResult3.SelectedValue);
                    remarks1.Append("," + txtRemark3_1.Text);
                    remarks2.Append("," + txtRemark3_2.Text);
                    remarks3.Append("," + txtRemark3_3.Text);
                    remarks4.Append("," + txtRemark3_4.Text);
                    remarks5.Append("," + txtRemark3_5.Text);
                    remarks6.Append("," + txtRemark3_6.Text);

                    // Test 4
                    impulseVoltageMeasured.Append("," + txtMeasuredImpulseVoltage4.Text);
                    peakVoltageMeasured.Append("," + txtMeasuredPeakVoltage4.Text);
                    frontTimeMeasured.Append("," + txtMeasuredFrontTime4.Text);
                    tailTimeMeasured.Append("," + txtMeasuredTailTime4.Text);
                    conductorTempMeasured.Append("," + txtMeasuredconductortemp4.Text);
                    testResultMeasured.Append("," + ddlTestResult4.SelectedValue);
                    remarks1.Append("," + txtRemark4_1.Text);
                    remarks2.Append("," + txtRemark4_2.Text);
                    remarks3.Append("," + txtRemark4_3.Text);
                    remarks4.Append("," + txtRemark4_4.Text);
                    remarks5.Append("," + txtRemark4_5.Text);
                    remarks6.Append("," + txtRemark4_6.Text);

                    // Test 5
                    impulseVoltageMeasured.Append("," + txtMeasuredImpulseVoltage5.Text);
                    peakVoltageMeasured.Append("," + txtMeasuredPeakVoltage5.Text);
                    frontTimeMeasured.Append("," + txtMeasuredFrontTime5.Text);
                    tailTimeMeasured.Append("," + txtMeasuredTailTime5.Text);
                    conductorTempMeasured.Append("," + txtMeasuredconductortemp5.Text);
                    testResultMeasured.Append("," + ddlTestResult5.SelectedValue);
                    remarks1.Append("," + txtRemark5_1.Text);
                    remarks2.Append("," + txtRemark5_2.Text);
                    remarks3.Append("," + txtRemark5_3.Text);
                    remarks4.Append("," + txtRemark5_4.Text);
                    remarks5.Append("," + txtRemark5_5.Text);
                    remarks6.Append("," + txtRemark5_6.Text);

                    // Test 6
                    impulseVoltageMeasured.Append("," + txtMeasuredImpulseVoltage6.Text);
                    peakVoltageMeasured.Append("," + txtMeasuredPeakVoltage6.Text);
                    frontTimeMeasured.Append("," + txtMeasuredFrontTime6.Text);
                    tailTimeMeasured.Append("," + txtMeasuredTailTime6.Text);
                    conductorTempMeasured.Append("," + txtMeasuredconductortemp6.Text);
                    testResultMeasured.Append("," + ddlTestResult6.SelectedValue);
                    remarks1.Append("," + txtRemark6_1.Text);
                    remarks2.Append("," + txtRemark6_2.Text);
                    remarks3.Append("," + txtRemark6_3.Text);
                    remarks4.Append("," + txtRemark6_4.Text);
                    remarks5.Append("," + txtRemark6_5.Text);
                    remarks6.Append("," + txtRemark6_6.Text);

                    // Test 7
                    impulseVoltageMeasured.Append("," + txtMeasuredImpulseVoltage7.Text);
                    peakVoltageMeasured.Append("," + txtMeasuredPeakVoltage7.Text);
                    frontTimeMeasured.Append("," + txtMeasuredFrontTime7.Text);
                    tailTimeMeasured.Append("," + txtMeasuredTailTime7.Text);
                    conductorTempMeasured.Append("," + txtMeasuredconductortemp7.Text);
                    testResultMeasured.Append("," + ddlTestResult7.SelectedValue);
                    remarks1.Append("," + txtRemark7_1.Text);
                    remarks2.Append("," + txtRemark7_2.Text);
                    remarks3.Append("," + txtRemark7_3.Text);
                    remarks4.Append("," + txtRemark7_4.Text);
                    remarks5.Append("," + txtRemark7_5.Text);
                    remarks6.Append("," + txtRemark7_6.Text);

                    // Test 8
                    impulseVoltageMeasured.Append("," + txtMeasuredImpulseVoltage8.Text);
                    peakVoltageMeasured.Append("," + txtMeasuredPeakVoltage8.Text);
                    frontTimeMeasured.Append("," + txtMeasuredFrontTime8.Text);
                    tailTimeMeasured.Append("," + txtMeasuredTailTime8.Text);
                    conductorTempMeasured.Append("," + txtMeasuredconductortemp8.Text);
                    testResultMeasured.Append("," + ddlTestResult8.SelectedValue);
                    remarks1.Append("," + txtRemark8_1.Text);
                    remarks2.Append("," + txtRemark8_2.Text);
                    remarks3.Append("," + txtRemark8_3.Text);
                    remarks4.Append("," + txtRemark8_4.Text);
                    remarks5.Append("," + txtRemark8_5.Text);
                    remarks6.Append("," + txtRemark8_6.Text);

                    // Test 9
                    impulseVoltageMeasured.Append("," + txtMeasuredImpulseVoltage9.Text);
                    peakVoltageMeasured.Append("," + txtMeasuredPeakVoltage9.Text);
                    frontTimeMeasured.Append("," + txtMeasuredFrontTime9.Text);
                    tailTimeMeasured.Append("," + txtMeasuredTailTime9.Text);
                    conductorTempMeasured.Append("," + txtMeasuredconductortemp9.Text);
                    testResultMeasured.Append("," + ddlTestResult9.SelectedValue);
                    remarks1.Append("," + txtRemark9_1.Text);
                    remarks2.Append("," + txtRemark9_2.Text);
                    remarks3.Append("," + txtRemark9_3.Text);
                    remarks4.Append("," + txtRemark9_4.Text);
                    remarks5.Append("," + txtRemark9_5.Text);
                    remarks6.Append("," + txtRemark9_6.Text);

                    // Test 10
                    impulseVoltageMeasured.Append("," + txtMeasuredImpulseVoltage10.Text);
                    peakVoltageMeasured.Append("," + txtMeasuredPeakVoltage10.Text);
                    frontTimeMeasured.Append("," + txtMeasuredFrontTime10.Text);
                    tailTimeMeasured.Append("," + txtMeasuredTailTime10.Text);
                    conductorTempMeasured.Append("," + txtMeasuredconductortemp10.Text);
                    testResultMeasured.Append("," + ddlTestResult10.SelectedValue);
                    remarks1.Append("," + txtRemark10_1.Text);
                    remarks2.Append("," + txtRemark10_2.Text);
                    remarks3.Append("," + txtRemark10_3.Text);
                    remarks4.Append("," + txtRemark10_4.Text);
                    remarks5.Append("," + txtRemark10_5.Text);
                    remarks6.Append("," + txtRemark10_6.Text);


                    // Store comma-separated values in the dictionary
                    testData["ImpulseVoltageMeasured"] = impulseVoltageMeasured.ToString();
                    testData["PeakVoltageMeasured"] = peakVoltageMeasured.ToString();
                    testData["FrontTimeMeasured"] = frontTimeMeasured.ToString();
                    testData["TailTimeMeasured"] = tailTimeMeasured.ToString();
                    testData["ConductorTempMeasured"] = conductorTempMeasured.ToString();
                    testData["TestResultMeasured"] = testResultMeasured.ToString();
                    testData["Remarks1"] = remarks1.ToString();
                    testData["Remarks2"] = remarks2.ToString();
                    testData["Remarks3"] = remarks3.ToString();
                    testData["Remarks4"] = remarks4.ToString();
                    testData["Remarks5"] = remarks5.ToString();
                    testData["Remarks6"] = remarks6.ToString();

                    // Debug logging - print what we've saved
                    System.Diagnostics.Debug.WriteLine($"Saved {currentPhase}/{currentTestType} data:");
                    foreach (var key in testData.Keys)
                    {
                        System.Diagnostics.Debug.WriteLine($"  {key}: {testData[key]}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Panel is not visible, no data collected");
                }

                // Update session
                Session["TestData"] = allTestData;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Invalid phase index or test type - nothing saved");
            }
        }   
        private void InitializeForm()
        {
            // Get current phase and test type
            List<string> phases = Session["Phases"] as List<string>;
            int currentPhaseIndex = (int)Session["CurrentPhaseIndex"];
            string currentTestType = Session["CurrentTestType"].ToString();
            string currentPhase = phases[currentPhaseIndex];

            // Update radio button selection to match current phase
            rblCorePhase.SelectedValue = currentPhase;

            // Update label showing current phase and test type
            lblSelectedPhase.Text = currentPhase + " Phase";
            lblfooterSelectedPhase.Text = currentPhase + " Phase";

            // Set the test mode label based on test type
            if (currentTestType == "Positive")
            {
                lblTestMode.Text = " - 10 Positive Impulse Tests followed by a Voltage test";
                lblfooterTestMode.Text = " - 10 Positive Impulse Tests followed by a Voltage test";
            }
            else // Negative
            {
                lblTestMode.Text = " - 10 Negative Impulse Tests followed by a Voltage test";
                lblfooterTestMode.Text = " - 10 Negative Impulse Tests followed by a Voltage test";
            }
         
            // Show the appropriate panels
            pnlTestData.Visible = true;
            pnlNavigation.Visible = true;
         


            // Set navigation button visibility
            btnPrevious.Visible = (currentPhaseIndex > 0) || (currentTestType == "Negative");
            btnNext.Visible = (currentPhaseIndex < phases.Count - 1) || (currentTestType == "Positive");
            btnSaveData.Visible = (currentPhaseIndex == phases.Count - 1) && (currentTestType == "Negative");

            // Load any saved data from session
            LoadDataFromSession();

            // Configure test fields based on test type
            if (currentTestType == "Positive")
            {
                SetupPositiveImpulseTest();
            }
            else
            {
                SetupNegativeImpulseTest();
            }
        }
        private void LoadDataFromSession()
        {
            List<string> phases = Session["Phases"] as List<string>;
            int currentPhaseIndex = (int)Session["CurrentPhaseIndex"];
            string currentTestType = Session["CurrentTestType"].ToString();

            if (currentPhaseIndex >= 0 && currentPhaseIndex < phases.Count)
            {
                string currentPhase = phases[currentPhaseIndex];
                Dictionary<string, Dictionary<string, Dictionary<string, string>>> allTestData =
                    Session["TestData"] as Dictionary<string, Dictionary<string, Dictionary<string, string>>>;

                // Check if we have data for this phase and test type
                if (allTestData != null &&
                    allTestData.ContainsKey(currentPhase) &&
                    allTestData[currentPhase].ContainsKey(currentTestType))
                {
                    Dictionary<string, string> testData = allTestData[currentPhase][currentTestType];

                    // Get the comma-separated values
                    string impulseVoltageMeasured = testData.ContainsKey("ImpulseVoltageMeasured") ? testData["ImpulseVoltageMeasured"] : "";
                    string peakVoltageMeasured = testData.ContainsKey("PeakVoltageMeasured") ? testData["PeakVoltageMeasured"] : "";
                    string frontTimeMeasured = testData.ContainsKey("FrontTimeMeasured") ? testData["FrontTimeMeasured"] : "";
                    string tailTimeMeasured = testData.ContainsKey("TailTimeMeasured") ? testData["TailTimeMeasured"] : "";
                    string conductorTempMeasured = testData.ContainsKey("ConductorTempMeasured") ? testData["ConductorTempMeasured"] : "";
                    string testResultMeasured = testData.ContainsKey("TestResultMeasured") ? testData["TestResultMeasured"] : "";
                    string remarks1 = testData.ContainsKey("Remarks1") ? testData["Remarks1"] : "";
                    string remarks2 = testData.ContainsKey("Remarks2") ? testData["Remarks2"] : "";
                    string remarks3 = testData.ContainsKey("Remarks3") ? testData["Remarks3"] : "";
                    string remarks4 = testData.ContainsKey("Remarks4") ? testData["Remarks4"] : "";
                    string remarks5 = testData.ContainsKey("Remarks5") ? testData["Remarks5"] : "";
                    string remarks6 = testData.ContainsKey("Remarks6") ? testData["Remarks6"] : "";

                    // Split the comma-separated values
                    string[] impulseVoltageMeasuredValues = impulseVoltageMeasured.Split(',');
                    string[] peakVoltageMeasuredValues = peakVoltageMeasured.Split(',');
                    string[] frontTimeMeasuredValues = frontTimeMeasured.Split(',');
                    string[] tailTimeMeasuredValues = tailTimeMeasured.Split(',');
                    string[] conductorTempMeasuredValues = conductorTempMeasured.Split(',');
                    string[] testResultMeasuredValues = testResultMeasured.Split(',');
                    string[] remarks1Values = remarks1.Split(',');
                    string[] remarks2Values = remarks2.Split(',');
                    string[] remarks3Values = remarks3.Split(',');
                    string[] remarks4Values = remarks4.Split(',');
                    string[] remarks5Values = remarks5.Split(',');
                    string[] remarks6Values = remarks6.Split(',');

                    // Populate the form fields for the first test
                    if (impulseVoltageMeasuredValues.Length > 0)
                        txtMeasuredImpulseVoltage1.Text = impulseVoltageMeasuredValues[0];
                    if (peakVoltageMeasuredValues.Length > 0)
                        txtMeasuredPeakVoltage1.Text = peakVoltageMeasuredValues[0];
                    if (frontTimeMeasuredValues.Length > 0)
                        txtMeasuredFrontTime1.Text = frontTimeMeasuredValues[0];
                    if (tailTimeMeasuredValues.Length > 0)
                        txtMeasuredTailTime1.Text = tailTimeMeasuredValues[0];
                    if (conductorTempMeasuredValues.Length > 0)
                        txtMeasuredconductortemp1.Text = conductorTempMeasuredValues[0];
                    if (testResultMeasuredValues.Length > 0 && ddlTestResult1 != null)
                        ddlTestResult1.SelectedValue = testResultMeasuredValues[0];
                    if (remarks1Values.Length > 0)
                        txtRemark1_1.Text = remarks1Values[0];
                    if (remarks2Values.Length > 0)
                        txtRemark1_2.Text = remarks2Values[0];
                    if (remarks3Values.Length > 0)
                        txtRemark1_3.Text = remarks3Values[0];
                    if (remarks4Values.Length > 0)
                        txtRemark1_4.Text = remarks4Values[0];
                    if (remarks5Values.Length > 0)
                        txtRemark1_5.Text = remarks5Values[0];
                    if (remarks6Values.Length > 0)
                        txtRemark1_6.Text = remarks6Values[0];

                    // Populate the form fields for the second test
                    if (impulseVoltageMeasuredValues.Length > 1)
                        txtMeasuredImpulseVoltage2.Text = impulseVoltageMeasuredValues[1];
                    if (peakVoltageMeasuredValues.Length > 1)
                        txtMeasuredPeakVoltage2.Text = peakVoltageMeasuredValues[1];
                    if (frontTimeMeasuredValues.Length > 1)
                        txtMeasuredFrontTime2.Text = frontTimeMeasuredValues[1];
                    if (tailTimeMeasuredValues.Length > 1)
                        txtMeasuredTailTime2.Text = tailTimeMeasuredValues[1];
                    if (conductorTempMeasuredValues.Length > 1)
                        txtMeasuredconductortemp2.Text = conductorTempMeasuredValues[1];
                    if (testResultMeasuredValues.Length > 1 && ddlTestResult2 != null)
                        ddlTestResult2.SelectedValue = testResultMeasuredValues[1];
                    if (remarks1Values.Length > 1)
                        txtRemark2_1.Text = remarks1Values[1];
                    if (remarks2Values.Length > 1)
                        txtRemark2_2.Text = remarks2Values[1];
                    if (remarks3Values.Length > 1)
                        txtRemark2_3.Text = remarks3Values[1];
                    if (remarks4Values.Length > 1)
                        txtRemark2_4.Text = remarks4Values[1];
                    if (remarks5Values.Length > 1)
                        txtRemark2_5.Text = remarks5Values[1];
                    if (remarks6Values.Length > 1)
                        txtRemark2_6.Text = remarks6Values[1];

                    // Populate the form fields for the third test
                    if (impulseVoltageMeasuredValues.Length > 2)
                        txtMeasuredImpulseVoltage3.Text = impulseVoltageMeasuredValues[2];
                    if (peakVoltageMeasuredValues.Length > 2)
                        txtMeasuredPeakVoltage3.Text = peakVoltageMeasuredValues[2];
                    if (frontTimeMeasuredValues.Length > 2)
                        txtMeasuredFrontTime3.Text = frontTimeMeasuredValues[2];
                    if (tailTimeMeasuredValues.Length > 2)
                        txtMeasuredTailTime3.Text = tailTimeMeasuredValues[2];
                    if (conductorTempMeasuredValues.Length > 2)
                        txtMeasuredconductortemp3.Text = conductorTempMeasuredValues[2];
                    if (testResultMeasuredValues.Length > 2 && ddlTestResult3 != null)
                        ddlTestResult3.SelectedValue = testResultMeasuredValues[2];
                    if (remarks1Values.Length > 2)
                        txtRemark3_1.Text = remarks1Values[2];
                    if (remarks2Values.Length > 2)
                        txtRemark3_2.Text = remarks2Values[2];
                    if (remarks3Values.Length > 2)
                        txtRemark3_3.Text = remarks3Values[2];
                    if (remarks4Values.Length > 2)
                        txtRemark3_4.Text = remarks4Values[2];
                    if (remarks5Values.Length > 2)
                        txtRemark3_5.Text = remarks5Values[2];
                    if (remarks6Values.Length > 2)
                        txtRemark3_6.Text = remarks6Values[2];

                    // Populate the form fields for the fourth test
                    if (impulseVoltageMeasuredValues.Length > 3)
                        txtMeasuredImpulseVoltage4.Text = impulseVoltageMeasuredValues[3];
                    if (peakVoltageMeasuredValues.Length > 3)
                        txtMeasuredPeakVoltage4.Text = peakVoltageMeasuredValues[3];
                    if (frontTimeMeasuredValues.Length > 3)
                        txtMeasuredFrontTime4.Text = frontTimeMeasuredValues[3];
                    if (tailTimeMeasuredValues.Length > 3)
                        txtMeasuredTailTime4.Text = tailTimeMeasuredValues[3];
                    if (conductorTempMeasuredValues.Length > 3)
                        txtMeasuredconductortemp4.Text = conductorTempMeasuredValues[3];
                    if (testResultMeasuredValues.Length > 3 && ddlTestResult4 != null)
                        ddlTestResult4.SelectedValue = testResultMeasuredValues[3];
                    if (remarks1Values.Length > 3)
                        txtRemark4_1.Text = remarks1Values[3];
                    if (remarks2Values.Length > 3)
                        txtRemark4_2.Text = remarks2Values[3];
                    if (remarks3Values.Length > 3)
                        txtRemark4_3.Text = remarks3Values[3];
                    if (remarks4Values.Length > 3)
                        txtRemark4_4.Text = remarks4Values[3];
                    if (remarks5Values.Length > 3)
                        txtRemark4_5.Text = remarks4Values[3];
                    if (remarks6Values.Length > 3)
                        txtRemark4_6.Text = remarks6Values[3];

                    // Populate the form fields for the fifth test
                    if (impulseVoltageMeasuredValues.Length > 4)
                        txtMeasuredImpulseVoltage5.Text = impulseVoltageMeasuredValues[4];
                    if (peakVoltageMeasuredValues.Length > 4)
                        txtMeasuredPeakVoltage5.Text = peakVoltageMeasuredValues[4];
                    if (frontTimeMeasuredValues.Length > 4)
                        txtMeasuredFrontTime5.Text = frontTimeMeasuredValues[4];
                    if (tailTimeMeasuredValues.Length > 4)
                        txtMeasuredTailTime5.Text = tailTimeMeasuredValues[4];
                    if (conductorTempMeasuredValues.Length > 4)
                        txtMeasuredconductortemp5.Text = conductorTempMeasuredValues[4];
                    if (testResultMeasuredValues.Length > 4 && ddlTestResult5 != null)
                        ddlTestResult5.SelectedValue = testResultMeasuredValues[4];
                    if (remarks1Values.Length > 4)
                        txtRemark5_1.Text = remarks1Values[4];
                    if (remarks2Values.Length > 4)
                        txtRemark5_2.Text = remarks2Values[4];
                    if (remarks3Values.Length > 4)
                        txtRemark5_3.Text = remarks4Values[4];
                    if (remarks4Values.Length > 4)
                        txtRemark5_4.Text = remarks4Values[4];
                    if (remarks5Values.Length > 4)
                        txtRemark5_5.Text = remarks5Values[4];
                    if (remarks6Values.Length > 4)
                        txtRemark5_6.Text = remarks6Values[4];

                    // Populate the form fields for the sixth test
                    if (impulseVoltageMeasuredValues.Length > 5)
                        txtMeasuredImpulseVoltage6.Text = impulseVoltageMeasuredValues[5];
                    if (peakVoltageMeasuredValues.Length > 5)
                        txtMeasuredPeakVoltage6.Text = peakVoltageMeasuredValues[5];
                    if (frontTimeMeasuredValues.Length > 5)
                        txtMeasuredFrontTime6.Text = frontTimeMeasuredValues[5];
                    if (tailTimeMeasuredValues.Length > 5)
                        txtMeasuredTailTime6.Text = tailTimeMeasuredValues[5];
                    if (conductorTempMeasuredValues.Length > 5)
                        txtMeasuredconductortemp6.Text = conductorTempMeasuredValues[5];
                    if (testResultMeasuredValues.Length > 5 && ddlTestResult6 != null)
                        ddlTestResult6.SelectedValue = testResultMeasuredValues[5];
                    if (remarks1Values.Length > 5)
                        txtRemark6_1.Text = remarks1Values[5];
                    if (remarks2Values.Length > 5)
                        txtRemark6_2.Text = remarks2Values[5];
                    if (remarks3Values.Length > 5)
                        txtRemark6_3.Text = remarks3Values[5];
                    if (remarks4Values.Length > 5)
                        txtRemark6_4.Text = remarks4Values[5];
                    if (remarks5Values.Length > 5)
                        txtRemark6_5.Text = remarks5Values[5];
                    if (remarks6Values.Length > 5)
                        txtRemark6_6.Text = remarks6Values[5];

                    // Populate the form fields for the seventh test
                    if (impulseVoltageMeasuredValues.Length > 6)
                        txtMeasuredImpulseVoltage7.Text = impulseVoltageMeasuredValues[6];
                    if (peakVoltageMeasuredValues.Length > 6)
                        txtMeasuredPeakVoltage7.Text = peakVoltageMeasuredValues[6];
                    if (frontTimeMeasuredValues.Length > 6)
                        txtMeasuredFrontTime7.Text = frontTimeMeasuredValues[6];
                    if (tailTimeMeasuredValues.Length > 6)
                        txtMeasuredTailTime7.Text = tailTimeMeasuredValues[6];
                    if (conductorTempMeasuredValues.Length > 6)
                        txtMeasuredconductortemp7.Text = conductorTempMeasuredValues[6];
                    if (testResultMeasuredValues.Length > 6 && ddlTestResult7 != null)
                        ddlTestResult7.SelectedValue = testResultMeasuredValues[6];
                    if (remarks1Values.Length > 6)
                        txtRemark7_1.Text = remarks1Values[6];
                    if (remarks2Values.Length > 6)
                        txtRemark7_2.Text = remarks2Values[6];
                    if (remarks3Values.Length > 6)
                        txtRemark7_3.Text = remarks3Values[6];
                    if (remarks4Values.Length > 6)
                        txtRemark7_4.Text = remarks4Values[6];
                    if (remarks5Values.Length > 6)
                        txtRemark7_5.Text = remarks5Values[6];
                    if (remarks6Values.Length > 6)
                        txtRemark7_6.Text = remarks6Values[6];

                    // Populate the form fields for the eighth test
                    if (impulseVoltageMeasuredValues.Length > 7)
                        txtMeasuredImpulseVoltage8.Text = impulseVoltageMeasuredValues[7];
                    if (peakVoltageMeasuredValues.Length > 7)
                        txtMeasuredPeakVoltage8.Text = peakVoltageMeasuredValues[7];
                    if (frontTimeMeasuredValues.Length > 7)
                        txtMeasuredFrontTime8.Text = frontTimeMeasuredValues[7];
                    if (tailTimeMeasuredValues.Length > 7)
                        txtMeasuredTailTime8.Text = tailTimeMeasuredValues[7];
                    if (conductorTempMeasuredValues.Length > 7)
                        txtMeasuredconductortemp8.Text = conductorTempMeasuredValues[7];
                    if (testResultMeasuredValues.Length > 7 && ddlTestResult8 != null)
                        ddlTestResult8.SelectedValue = testResultMeasuredValues[7];
                    if (remarks1Values.Length > 7)
                        txtRemark8_1.Text = remarks1Values[7];
                    if (remarks2Values.Length > 7)
                        txtRemark8_2.Text = remarks2Values[7];
                    if (remarks3Values.Length > 7)
                        txtRemark8_3.Text = remarks3Values[7];
                    if (remarks4Values.Length > 7)
                        txtRemark8_4.Text = remarks4Values[7];
                    if (remarks5Values.Length > 7)
                        txtRemark8_5.Text = remarks5Values[7];
                    if (remarks6Values.Length > 7)
                        txtRemark8_6.Text = remarks6Values[7];

                    // Populate the form fields for the ninth test
                    if (impulseVoltageMeasuredValues.Length > 8)
                        txtMeasuredImpulseVoltage9.Text = impulseVoltageMeasuredValues[8];
                    if (peakVoltageMeasuredValues.Length > 8)
                        txtMeasuredPeakVoltage9.Text = peakVoltageMeasuredValues[8];
                    if (frontTimeMeasuredValues.Length > 8)
                        txtMeasuredFrontTime9.Text = frontTimeMeasuredValues[8];
                    if (tailTimeMeasuredValues.Length > 8)
                        txtMeasuredTailTime9.Text = tailTimeMeasuredValues[8];
                    if (conductorTempMeasuredValues.Length > 8)
                        txtMeasuredconductortemp9.Text = conductorTempMeasuredValues[8];
                    if (testResultMeasuredValues.Length > 8 && ddlTestResult9 != null)
                        ddlTestResult9.SelectedValue = testResultMeasuredValues[8];
                    if (remarks1Values.Length > 8)
                        txtRemark9_1.Text = remarks1Values[8];
                    if (remarks2Values.Length > 8)
                        txtRemark9_2.Text = remarks2Values[8];
                    if (remarks3Values.Length > 8)
                        txtRemark9_3.Text = remarks3Values[8];
                    if (remarks4Values.Length > 8)
                        txtRemark9_4.Text = remarks4Values[8];
                    if (remarks5Values.Length > 8)
                        txtRemark9_5.Text = remarks5Values[8];
                    if (remarks6Values.Length > 8)
                        txtRemark9_6.Text = remarks6Values[8];

                    // Populate the form fields for the tenth test
                    if (impulseVoltageMeasuredValues.Length > 9)
                        txtMeasuredImpulseVoltage10.Text = impulseVoltageMeasuredValues[9];
                    if (peakVoltageMeasuredValues.Length > 9)
                        txtMeasuredPeakVoltage10.Text = peakVoltageMeasuredValues[9];
                    if (frontTimeMeasuredValues.Length > 9)
                        txtMeasuredFrontTime10.Text = frontTimeMeasuredValues[9];
                    if (tailTimeMeasuredValues.Length > 9)
                        txtMeasuredTailTime10.Text = tailTimeMeasuredValues[9];
                    if (conductorTempMeasuredValues.Length > 9)
                        txtMeasuredconductortemp10.Text = conductorTempMeasuredValues[9];
                    if (testResultMeasuredValues.Length > 9 && ddlTestResult10 != null)
                        ddlTestResult10.SelectedValue = testResultMeasuredValues[9];
                    if (remarks1Values.Length > 9)
                        txtRemark10_1.Text = remarks1Values[9];
                    if (remarks2Values.Length > 9)
                        txtRemark10_2.Text = remarks2Values[9];
                    if (remarks3Values.Length > 9)
                        txtRemark10_3.Text = remarks3Values[9];
                    if (remarks4Values.Length > 9)
                        txtRemark10_4.Text = remarks4Values[9];
                    if (remarks5Values.Length > 9)
                        txtRemark10_5.Text = remarks5Values[9];
                    if (remarks6Values.Length > 9)
                        txtRemark10_6.Text = remarks6Values[9];
                }
                else
                {
                    // No data exists, clear the form fields
                    ClearFormFields();
                }
            }
        }
        private TextBox FindTextBoxRecursive(Control parent, string id)
        {
            foreach (Control c in parent.Controls)
            {
                if (c.ID == id)
                    return c as TextBox;

                TextBox found = FindTextBoxRecursive(c, id);
                if (found != null)
                    return found;
            }
            return null;
        }
        private bool ValidateMeasuredValues()
        {
            bool isValid = true;

            for (int i = 1; i <= 10; i++)
            {
                TextBox txtImpulse = FindTextBoxRecursive(Page, "txtMeasuredImpulseVoltage" + i);
                TextBox txtPeak = FindTextBoxRecursive(Page, "txtMeasuredPeakVoltage" + i);
                TextBox txtFront = FindTextBoxRecursive(Page, "txtMeasuredFrontTime" + i);
                TextBox txtTail = FindTextBoxRecursive(Page, "txtMeasuredTailTime" + i);
                TextBox txtTemp = FindTextBoxRecursive(Page, "txtMeasuredconductortemp" + i);

                ValidateTextBox(txtImpulse, ref isValid);
                ValidateTextBox(txtPeak, ref isValid);
                ValidateTextBox(txtFront, ref isValid);
                ValidateTextBox(txtTail, ref isValid);
                ValidateTextBox(txtTemp, ref isValid);
            }
            return isValid;
        }
        private void ValidateTextBox(TextBox txt, ref bool isValid)
        {
            if (txt == null) return;

            if (string.IsNullOrWhiteSpace(txt.Text) || !double.TryParse(txt.Text, out _))
            {
                txt.Style["border"] = "1px solid red";
                if (isValid) txt.Focus();
                isValid = false;
            }
            else
            {
                txt.Style["border"] = "";
            }
        }

        //Others
        protected void rblCorePhase_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get selected phase
            string selectedPhase = rblCorePhase.SelectedValue;
            List<string> phases = Session["Phases"] as List<string>;

            // Find the index of the selected phase
            int newIndex = phases.IndexOf(selectedPhase);

            if (newIndex >= 0)
            {
                // Update session
                Session["CurrentPhaseIndex"] = newIndex;

                // Reset test type to positive when manually changing phase
                Session["CurrentTestType"] = "Positive";

                // Initialize the form for this phase
                InitializeForm();
            }
        }
        private void ClearFormFields()
        {
            // Clear Test 1 fields
            txtSpecImpulseVoltage1.Text = "";
            txtMeasuredImpulseVoltage1.Text = "";
            txtSpecPeakVoltage1.Text = "";
            txtMeasuredPeakVoltage1.Text = "";
            txtSpecFrontTime1.Text = "";
            txtMeasuredFrontTime1.Text = "";
            txtSpecTailTime1.Text = "";
            txtMeasuredTailTime1.Text = "";
            txtSpecconductortemp1.Text = "";
            txtMeasuredconductortemp1.Text = "";
            txtSpecTestResult1.Text = "";
            ddlTestResult1.SelectedValue = "PASS";
            txtRemark1_1.Text = "";
            txtRemark1_2.Text = "";
            txtRemark1_3.Text = "";
            txtRemark1_4.Text = "";
            txtRemark1_5.Text = "";
            txtRemark1_6.Text = "";

            // Clear Test 2 fields
            txtSpecImpulseVoltage2.Text = "";
            txtMeasuredImpulseVoltage2.Text = "";
            txtSpecPeakVoltage2.Text = "";
            txtMeasuredPeakVoltage2.Text = "";
            txtSpecFrontTime2.Text = "";
            txtMeasuredFrontTime2.Text = "";
            txtSpecTailTime2.Text = "";
            txtMeasuredTailTime2.Text = "";
            txtSpecconductortemp2.Text = "";
            txtMeasuredconductortemp2.Text = "";
            txtSpecTestResult2.Text = "";
            ddlTestResult2.SelectedValue = "PASS";
            txtRemark2_1.Text = "";
            txtRemark2_2.Text = "";
            txtRemark2_3.Text = "";
            txtRemark2_4.Text = "";
            txtRemark2_5.Text = "";
            txtRemark2_6.Text = "";

            // Clear Test 3 fields
            txtSpecImpulseVoltage3.Text = "";
            txtMeasuredImpulseVoltage3.Text = "";
            txtSpecPeakVoltage3.Text = "";
            txtMeasuredPeakVoltage3.Text = "";
            txtSpecFrontTime3.Text = "";
            txtMeasuredFrontTime3.Text = "";
            txtSpecTailTime3.Text = "";
            txtMeasuredTailTime3.Text = "";
            txtSpecconductortemp3.Text = "";
            txtMeasuredconductortemp3.Text = "";
            txtSpecTestResult3.Text = "";
            ddlTestResult3.SelectedValue = "PASS";
            txtRemark3_1.Text = "";
            txtRemark3_2.Text = "";
            txtRemark3_3.Text = "";
            txtRemark3_4.Text = "";
            txtRemark3_5.Text = "";
            txtRemark3_6.Text = "";

            // Clear Test 4 fields
            txtSpecImpulseVoltage4.Text = "";
            txtMeasuredImpulseVoltage4.Text = "";
            txtSpecPeakVoltage4.Text = "";
            txtMeasuredPeakVoltage4.Text = "";
            txtSpecFrontTime4.Text = "";
            txtMeasuredFrontTime4.Text = "";
            txtSpecTailTime4.Text = "";
            txtMeasuredTailTime4.Text = "";
            txtSpecconductortemp4.Text = "";
            txtMeasuredconductortemp4.Text = "";
            txtSpecTestResult4.Text = "";
            ddlTestResult4.SelectedValue = "PASS";
            txtRemark4_1.Text = "";
            txtRemark4_2.Text = "";
            txtRemark4_3.Text = "";
            txtRemark4_4.Text = "";
            txtRemark4_5.Text = "";
            txtRemark4_6.Text = "";

            // Clear Test 5 fields
            txtSpecImpulseVoltage5.Text = "";
            txtMeasuredImpulseVoltage5.Text = "";
            txtSpecPeakVoltage5.Text = "";
            txtMeasuredPeakVoltage5.Text = "";
            txtSpecFrontTime5.Text = "";
            txtMeasuredFrontTime5.Text = "";
            txtSpecTailTime5.Text = "";
            txtMeasuredTailTime5.Text = "";
            txtSpecconductortemp5.Text = "";
            txtMeasuredconductortemp5.Text = "";
            txtSpecTestResult5.Text = "";
            ddlTestResult5.SelectedValue = "PASS";
            txtRemark5_1.Text = "";
            txtRemark5_2.Text = "";
            txtRemark5_3.Text = "";
            txtRemark5_4.Text = "";
            txtRemark5_5.Text = "";
            txtRemark5_6.Text = "";

            // Clear Test 6 fields
            txtSpecImpulseVoltage6.Text = "";
            txtMeasuredImpulseVoltage6.Text = "";
            txtSpecPeakVoltage6.Text = "";
            txtMeasuredPeakVoltage6.Text = "";
            txtSpecFrontTime6.Text = "";
            txtMeasuredFrontTime6.Text = "";
            txtSpecTailTime6.Text = "";
            txtMeasuredTailTime6.Text = "";
            txtSpecconductortemp6.Text = "";
            txtMeasuredconductortemp6.Text = "";
            txtSpecTestResult6.Text = "";
            ddlTestResult6.SelectedValue = "PASS";
            txtRemark6_1.Text = "";
            txtRemark6_2.Text = "";
            txtRemark6_3.Text = "";
            txtRemark6_4.Text = "";
            txtRemark6_5.Text = "";
            txtRemark6_6.Text = "";

            // Clear Test 7 fields
            txtSpecImpulseVoltage7.Text = "";
            txtMeasuredImpulseVoltage7.Text = "";
            txtSpecPeakVoltage7.Text = "";
            txtMeasuredPeakVoltage7.Text = "";
            txtSpecFrontTime7.Text = "";
            txtMeasuredFrontTime7.Text = "";
            txtSpecTailTime7.Text = "";
            txtMeasuredTailTime7.Text = "";
            txtSpecconductortemp7.Text = "";
            txtMeasuredconductortemp7.Text = "";
            txtSpecTestResult7.Text = "";
            ddlTestResult7.SelectedValue = "PASS";
            txtRemark7_1.Text = "";
            txtRemark7_2.Text = "";
            txtRemark7_3.Text = "";
            txtRemark7_4.Text = "";
            txtRemark7_5.Text = "";
            txtRemark7_6.Text = "";

            // Clear Test 8 fields
            txtSpecImpulseVoltage8.Text = "";
            txtMeasuredImpulseVoltage8.Text = "";
            txtSpecPeakVoltage8.Text = "";
            txtMeasuredPeakVoltage8.Text = "";
            txtSpecFrontTime8.Text = "";
            txtMeasuredFrontTime8.Text = "";
            txtSpecTailTime8.Text = "";
            txtMeasuredTailTime8.Text = "";
            txtSpecconductortemp8.Text = "";
            txtMeasuredconductortemp8.Text = "";
            txtSpecTestResult8.Text = "";
            ddlTestResult8.SelectedValue = "PASS";
            txtRemark8_1.Text = "";
            txtRemark8_2.Text = "";
            txtRemark8_3.Text = "";
            txtRemark8_4.Text = "";
            txtRemark8_5.Text = "";
            txtRemark8_6.Text = "";

            // Clear Test 9 fields
            txtSpecImpulseVoltage9.Text = "";
            txtMeasuredImpulseVoltage9.Text = "";
            txtSpecPeakVoltage9.Text = "";
            txtMeasuredPeakVoltage9.Text = "";
            txtSpecFrontTime9.Text = "";
            txtMeasuredFrontTime9.Text = "";
            txtSpecTailTime9.Text = "";
            txtMeasuredTailTime9.Text = "";
            txtSpecconductortemp9.Text = "";
            txtMeasuredconductortemp9.Text = "";
            txtSpecTestResult9.Text = "";
            ddlTestResult9.SelectedValue = "PASS";
            txtRemark9_1.Text = "";
            txtRemark9_2.Text = "";
            txtRemark9_3.Text = "";
            txtRemark9_4.Text = "";
            txtRemark9_5.Text = "";
            txtRemark9_6.Text = "";

            // Clear Test 10 fields
            txtSpecImpulseVoltage10.Text = "";
            txtMeasuredImpulseVoltage10.Text = "";
            txtSpecPeakVoltage10.Text = "";
            txtMeasuredPeakVoltage10.Text = "";
            txtSpecFrontTime10.Text = "";
            txtMeasuredFrontTime10.Text = "";
            txtSpecTailTime10.Text = "";
            txtMeasuredTailTime10.Text = "";
            txtSpecconductortemp10.Text = "";
            txtMeasuredconductortemp10.Text = "";
            txtSpecTestResult10.Text = "";
            ddlTestResult10.SelectedValue = "PASS";
            txtRemark10_1.Text = "";
            txtRemark10_2.Text = "";
            txtRemark10_3.Text = "";
            txtRemark10_4.Text = "";
            txtRemark10_5.Text = "";
            txtRemark10_6.Text = "";
        }
        private void UpdateNavigationButtons()
        {
            List<string> phases = Session["Phases"] as List<string>;
            int currentPhaseIndex = (int)Session["CurrentPhaseIndex"];
            string currentTestType = Session["CurrentTestType"].ToString();

            // Always hide Previous button as per requirements
          //  btnPrevious.Visible = false;

            // Next button is visible except at the last phase with negative test
            bool isLastPhase = currentPhaseIndex == phases.Count - 1;
            bool isLastTest = currentTestType == "Negative";
            btnNext.Visible = !(isLastPhase && isLastTest);

            // Check if we're editing existing data
            bool isEditingExistingData = Request.QueryString["s"] != null && IsExistingData();

            // For last phase with negative test:
            if (isLastPhase && isLastTest)
            {
                if (isEditingExistingData)
                {
                    // Show Update button for existing data
                    btnUpdateData.Visible = true;
                    btnSaveData.Visible = false;
                }
                else
                {
                    // Show Save button for new data
                    btnUpdateData.Visible = false;
                    btnSaveData.Visible = true;
                }
            }
            else
            {
                // Not at the last phase/test yet
                btnUpdateData.Visible = false;
                btnSaveData.Visible = false;
            }
        }
        private void SetupPositiveImpulseTest()
        {
            // Set default values for positive impulse test
            SetImpulseValues("60");
        }
        private void SetupNegativeImpulseTest()
        {
            // Set default values for negative impulse test
            SetImpulseValues("-60");
        }
        private void SetImpulseValues(string impulseVoltage)
        {
            for (int i = 1; i <= 10; i++)
            {
                SetTextBoxValue($"txtSpecImpulseVoltage{i}", impulseVoltage);
                SetTextBoxValue($"txtSpecPeakVoltage{i}", "");
                SetTextBoxValue($"txtSpecFrontTime{i}", "1-5");
                SetTextBoxValue($"txtSpecTailTime{i}", "40-60");
                SetTextBoxValue($"txtSpecconductortemp{i}", "95-100");
                SetTextBoxValue($"txtSpecTestResult{i}", "No break down");
            }
        }
        private void SetTextBoxValue(string controlId, string value)
        {
            Control foundControl = FindControlRecursive(Page, controlId);
            if (foundControl is TextBox txt)
            {
                txt.Text = value;
            }
        }
        private Control FindControlRecursive(Control root, string id)
        {
            if (root.ID == id)
            {
                return root;
            }

            foreach (Control c in root.Controls)
            {
                Control foundControl = FindControlRecursive(c, id);
                if (foundControl != null)
                {
                    return foundControl;
                }
            }
            return null;
        }
        private void ResetForm()
        {
            // Reset session data
            Session["Phases"] = new List<string> { "Red", "Blue", "Yellow" };
            Session["CurrentPhaseIndex"] = 0;
            Session["CurrentTestType"] = "Positive";
            Session["TestData"] = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

            // Select first radio button
            rblCorePhase.SelectedIndex = 0;

            // Clear all form fields
            ClearFormFields();

            // Reset labels
            lblSelectedPhase.Text = "Red Phase";
            lblTestMode.Text = " - 10 Positive Impulse Test followed by a Voltage test";

            lblfooterSelectedPhase.Text = "Red Phase";
            lblfooterTestMode.Text = " - 10 Positive Impulse Test followed by a Voltage test";

            // Reset navigation buttons
            btnPrevious.Visible = false;
            btnNext.Visible = true;
            btnSaveData.Visible = false;

            // Initialize form
            InitializeForm();
        }
        private bool IsExistingData()
        {
            bool result = false;

            try
            {
                if (Request.QueryString["s"] != null && Request.QueryString["r"] != null)
                {
                    int sampleId = Convert.ToInt32(Request.QueryString["s"]);
                    int reportId = Convert.ToInt32(Request.QueryString["r"]);

                    // Check if there's data for this sample and report
                    DataTable existingData = GetExistingTestResultsForSample(sampleId, reportId);
                    result = (existingData != null && existingData.Rows.Count > 0);
                }
            }
            catch (Exception)
            {
                // Handle exception - return false by default
                result = false;
            }

            return result;
        }

        //PRE-FILLING DATA IF EXISTS FOR SAMPLEID
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
                       // btnPrevious.Visible = false; // Always hide Previous

                        // Prefill upper section details
                        PrefillUpperSection(existingData);
                        // Prefill values
                        PrefillValues(existingData);
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
            SetLabelText(cableType, "7212");
            SetLabelText(refSpec, "7214");
            SetLabelText(cableSize, "7215");
            SetLabelText(testMethodNo, "7217");
            SetLabelText(lblvoltage, "7216");
            SetLabelText(jobOrderNo, "7218");
            SetLabelText(worksheetno, "7220");
            SetLabelText(equipmentno, "7224");

            // Textbox fields
            SetTextBoxText(roomno, "7221");
            SetTextBoxText(roomhumidity, "7222");
            SetTextBoxText(roomtemp, "7223");
            SetTextBoxText(Date, "7219");

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
        private void PrefillValues(DataTable existingData)
        {
            try
            {
                // Get all the phase names from session
                List<string> phases = Session["Phases"] as List<string>;
                if (phases == null || phases.Count == 0)
                {
                    return;
                }

                // Helper function to get value from DataTable
                string GetParameterValue(string paramId)
                {
                    DataRow[] rows = existingData.Select($"ParameterID = '{paramId}'");
                    return rows.Length > 0 ? rows[0]["Result"].ToString() : string.Empty;
                }

                // Initialize the session data structure if not already present
                if (Session["TestData"] == null)
                {
                    Session["TestData"] = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
                }

                Dictionary<string, Dictionary<string, Dictionary<string, string>>> testData =
                    Session["TestData"] as Dictionary<string, Dictionary<string, Dictionary<string, string>>>;

                // Create dictionary for each phase if it doesn't exist
                foreach (string phase in phases)
                {
                    if (!testData.ContainsKey(phase))
                    {
                        testData[phase] = new Dictionary<string, Dictionary<string, string>>
                {
                    { "Positive", new Dictionary<string, string>() },
                    { "Negative", new Dictionary<string, string>() }
                };
                    }
                }

                // Get data for positive impulse tests
                string positiveImpulseVoltage = GetParameterValue("7225");
                string positivePeakVoltage = GetParameterValue("7226");
                string positiveFrontTime = GetParameterValue("7227");
                string positiveTailTime = GetParameterValue("7228");
                string positiveConductorTemp = GetParameterValue("7229");
                string positiveTestResult = GetParameterValue("7230");
                string positiveRemarks = GetParameterValue("7231");

                // Get data for negative impulse tests
                string negativeImpulseVoltage = GetParameterValue("7232");
                string negativePeakVoltage = GetParameterValue("7233");
                string negativeFrontTime = GetParameterValue("7234");
                string negativeTailTime = GetParameterValue("7235");
                string negativeConductorTemp = GetParameterValue("7236");
                string negativeTestResult = GetParameterValue("7237");
                string negativeRemarks = GetParameterValue("7238");

                // Process Positive data for each phase
                if (!string.IsNullOrEmpty(positiveImpulseVoltage))
                {
                    string[] positiveImpulseValues = positiveImpulseVoltage.Split('|');
                    string[] positivePeakValues = positivePeakVoltage.Split('|');
                    string[] positiveFrontValues = positiveFrontTime.Split('|');
                    string[] positiveTailValues = positiveTailTime.Split('|');
                    string[] positiveConductorValues = positiveConductorTemp.Split('|');
                    string[] positiveResultValues = positiveTestResult.Split('|');
                    string[] positiveRemarkValues = positiveRemarks.Split('|');

                    for (int i = 0; i < phases.Count && i < positiveImpulseValues.Length; i++)
                    {
                        testData[phases[i]]["Positive"]["ImpulseVoltageMeasured"] = i < positiveImpulseValues.Length ? positiveImpulseValues[i] : "";
                        testData[phases[i]]["Positive"]["PeakVoltageMeasured"] = i < positivePeakValues.Length ? positivePeakValues[i] : "";
                        testData[phases[i]]["Positive"]["FrontTimeMeasured"] = i < positiveFrontValues.Length ? positiveFrontValues[i] : "";
                        testData[phases[i]]["Positive"]["TailTimeMeasured"] = i < positiveTailValues.Length ? positiveTailValues[i] : "";
                        testData[phases[i]]["Positive"]["ConductorTempMeasured"] = i < positiveConductorValues.Length ? positiveConductorValues[i] : "";
                        testData[phases[i]]["Positive"]["TestResultMeasured"] = i < positiveResultValues.Length ? positiveResultValues[i] : "";

                        // Handle remarks - may need to split further if stored as a CSV within the pipe-delimited string
                        if (i < positiveRemarkValues.Length)
                        {
                            string[] remarks = positiveRemarkValues[i].Split(',');
                            for (int j = 0; j < remarks.Length && j < 6; j++)
                            {
                                testData[phases[i]]["Positive"][$"Remarks{j + 1}"] = j < remarks.Length ? remarks[j] : "";
                            }
                        }
                    }
                }

                // Process Negative data for each phase
                if (!string.IsNullOrEmpty(negativeImpulseVoltage))
                {
                    string[] negativeImpulseValues = negativeImpulseVoltage.Split('|');
                    string[] negativePeakValues = negativePeakVoltage.Split('|');
                    string[] negativeFrontValues = negativeFrontTime.Split('|');
                    string[] negativeTailValues = negativeTailTime.Split('|');
                    string[] negativeConductorValues = negativeConductorTemp.Split('|');
                    string[] negativeResultValues = negativeTestResult.Split('|');
                    string[] negativeRemarkValues = negativeRemarks.Split('|');

                    for (int i = 0; i < phases.Count && i < negativeImpulseValues.Length; i++)
                    {
                        testData[phases[i]]["Negative"]["ImpulseVoltageMeasured"] = i < negativeImpulseValues.Length ? negativeImpulseValues[i] : "";
                        testData[phases[i]]["Negative"]["PeakVoltageMeasured"] = i < negativePeakValues.Length ? negativePeakValues[i] : "";
                        testData[phases[i]]["Negative"]["FrontTimeMeasured"] = i < negativeFrontValues.Length ? negativeFrontValues[i] : "";
                        testData[phases[i]]["Negative"]["TailTimeMeasured"] = i < negativeTailValues.Length ? negativeTailValues[i] : "";
                        testData[phases[i]]["Negative"]["ConductorTempMeasured"] = i < negativeConductorValues.Length ? negativeConductorValues[i] : "";
                        testData[phases[i]]["Negative"]["TestResultMeasured"] = i < negativeResultValues.Length ? negativeResultValues[i] : "";

                        // Handle remarks - may need to split further if stored as a CSV within the pipe-delimited string
                        if (i < negativeRemarkValues.Length)
                        {
                            string[] remarks = negativeRemarkValues[i].Split(',');
                            for (int j = 0; j < remarks.Length && j < 6; j++)
                            {
                                testData[phases[i]]["Negative"][$"Remarks{j + 1}"] = j < remarks.Length ? remarks[j] : "";
                            }
                        }
                    }
                }

                // Update session
                Session["TestData"] = testData;

                // Update UI fields based on the session data
                LoadDataFromSession();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "PrefillValuesError",
                    $"alert('Error prefilling test values: {ex.Message}');", true);
            }
        }


    }
}