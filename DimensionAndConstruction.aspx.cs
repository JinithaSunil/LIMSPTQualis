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
using ZXing;

namespace LIMS
{

    public partial class DimensionAndConstruction : System.Web.UI.Page
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
                    roomno.Text = "Mechanical";
                    jobOrderNo.Text = "WO/2024120905";
                    worksheetno.Text = "-";
                    equipmentno.Text = "QE/12/PH/16,QE/16/PH/03,QE/15/PH/43";
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
            dt.Columns.Add("ConductorCondition", typeof(string));
            dt.Columns.Add("InsulationCondition", typeof(string));
            dt.Columns.Add("ColorScheme", typeof(string));            
            dt.Columns.Add("ColorCombination", typeof(string));
            dt.Columns.Add("FillerCondition", typeof(string));
            dt.Columns.Add("InnerSheathCondition", typeof(string));
            dt.Columns.Add("ArmouringCondition", typeof(string));
            dt.Columns.Add("SheathCondition", typeof(string));
            dt.Columns.Add("MarkingCondition", typeof(string));
            dt.Columns.Add("Identification", typeof(string));
            dt.Columns.Add("Distance", typeof(string));

            ViewState["TestResults"] = dt;

            // Bind the empty DataTable to the GridView
            gvDimensionConstruction.DataSource = dt;
            gvDimensionConstruction.DataBind();
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
        private void PrefillTestResultsGrid(int sampleId)
        {
            try
            {
                InitializeGrid();

                if (_cm == null)
                    _cm = new DAL.CM();

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    conn.Open();
                    string query = @"
                WITH ResultPivot AS (
                    SELECT 
                        SampleID,
                        MAX(CASE WHEN ReportParamMapID = '10221' THEN Result END) AS ConductorCondition,
                        MAX(CASE WHEN ReportParamMapID = '10222' THEN Result END) AS InsulationCondition,
                        MAX(CASE WHEN ReportParamMapID = '10223' THEN Result END) AS InsulationColorScheme,
                        MAX(CASE WHEN ReportParamMapID = '10224' THEN Result END) AS InsulationColorCombination,
                        MAX(CASE WHEN ReportParamMapID = '10225' THEN Result END) AS FillerCondition,
                        MAX(CASE WHEN ReportParamMapID = '10226' THEN Result END) AS InnerSheathCondition,
                        MAX(CASE WHEN ReportParamMapID = '10227' THEN Result END) AS ArmouringCondition,
                        MAX(CASE WHEN ReportParamMapID = '10228' THEN Result END) AS SheathCondition,
                        MAX(CASE WHEN ReportParamMapID = '10229' THEN Result END) AS MarkingCondition,
                        MAX(CASE WHEN ReportParamMapID = '10230' THEN Result END) AS MarkingIdentification,
                        MAX(CASE WHEN ReportParamMapID = '10231 ' THEN Result END) AS MarkingDistance
                    FROM TestResultsPTQualis
                    WHERE SampleID = @SampleID
                    GROUP BY SampleID
                )
                SELECT * FROM ResultPivot";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SampleID", sampleId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            DataTable dt = ViewState["TestResults"] as DataTable;
                            dt.Clear();

                            if (reader.Read())
                            {
                                DataRow newRow = dt.NewRow();
                                newRow["ConductorCondition"] = reader["ConductorCondition"].ToString();
                                newRow["InsulationCondition"] = reader["InsulationCondition"].ToString();
                                newRow["ColorScheme"] = reader["InsulationColorScheme"].ToString();
                                newRow["ColorCombination"] = reader["InsulationColorCombination"].ToString();
                                newRow["FillerCondition"] = reader["FillerCondition"].ToString();
                                newRow["InnerSheathCondition"] = reader["InnerSheathCondition"].ToString();
                                newRow["ArmouringCondition"] = reader["ArmouringCondition"].ToString();
                                newRow["SheathCondition"] = reader["SheathCondition"].ToString();
                                newRow["MarkingCondition"] = reader["MarkingCondition"].ToString();
                                newRow["Identification"] = reader["MarkingIdentification"].ToString();
                                newRow["Distance"] = reader["MarkingDistance"].ToString();

                                dt.Rows.Add(newRow);
                            }

                            gvDimensionConstruction.DataSource = dt;
                            gvDimensionConstruction.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "GridPrefillError",
                    $"alert('Error prefilling test results grid: {ex.Message}');", true);
            }
        }
        private void PrefillUpperSection(DataTable existingData)
        { //Upper section

            // Method to prefill upper section details based on parameter IDs
            SetLabelText(cableType, "10232");
            SetLabelText(refSpec, "10238");
            SetLabelText(cableSize, "10233");
            SetLabelText(testMethodNo, "10228");
            SetLabelText(voltage, "10234");
            SetLabelText(jobOrderNo, "10240");
            SetLabelText(worksheetno, "10241");
            SetLabelText(equipmentno, "10243");

            // Textbox fields
            SetTextBoxText(roomno, "10236");
            SetTextBoxText(roomhumidity, "10242");
            SetTextBoxText(roomtemp, "10237");
            SetTextBoxText(Date, "10235");

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


            // allTextBoxes.Add(txtMedian);

            //allTextBoxes.Add(txtStandardmaxVolumeResitstivity);
            //allTextBoxes.Add(txtStandardmaxVolumeResitstivity_conductor);

            // Reset border for each TextBox
            foreach (var textBox in allTextBoxes)
            {
                textBox.BorderColor = System.Drawing.Color.Empty; // Default color
                textBox.BorderStyle = BorderStyle.NotSet; // Default style
                textBox.BorderWidth = 0; // Default width
            }
        }
        protected void btnClearData_Click(object sender, EventArgs e)
        {
            ClearForm();
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
            txtarmouring.Text = "";
            txtcolorcombination.Text = "";
            txtCondition.Text = "";
            txtconductorcondition.Text = "";
            txtdistance.Text = "";
            txtfillercondition.Text = "";
            txtidentification.Text = "";
            txtinnersheath.Text = "";
            txtmarkingcondition.Text = "";
            txtsheath.Text = "";
            // txtMeasured.Text = "";

            // Clear GridView
            gvDimensionConstruction.DataSource = null;
            gvDimensionConstruction.DataBind();

        

        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            List<string> errors = new List<string>();
            List<Control> invalidControls = new List<Control>();

            // Check if Grid has data
            bool gridHasData = (gvDimensionConstruction.Rows.Count > 0);

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
                // Validate Test Details Input 

                if (string.IsNullOrEmpty(txtconductorcondition.Text) || (txtconductorcondition.Text != "-" && !double.TryParse(txtconductorcondition.Text, out _)))
                {
                    errors.Add("conductor condition must be a valid number or '-'.");
                    invalidControls.Add(txtconductorcondition);
                }
                if (string.IsNullOrEmpty(txtCondition.Text) || (txtCondition.Text != "-" && !double.TryParse(txtCondition.Text, out _)))
                {
                    errors.Add("Condition must be a valid number or '-'.");
                    invalidControls.Add(txtCondition);
                }
                if (string.IsNullOrEmpty(txtcolorcombination.Text) || (txtcolorcombination.Text != "-" && !double.TryParse(txtcolorcombination.Text, out _)))
                {
                    errors.Add("color combination value must be a valid number or '-'.");
                    invalidControls.Add(txtcolorcombination);
                }
                if (string.IsNullOrEmpty(txtdistance.Text) || (txtdistance.Text != "-" && !double.TryParse(txtdistance.Text, out _)))
                {
                    errors.Add("distance value must be a valid number or '-'.");
                    invalidControls.Add(txtdistance);
                }
                if (string.IsNullOrEmpty(txtcolorscheme.Text) || (txtcolorscheme.Text != "-" && !double.TryParse(txtcolorscheme.Text, out _)))
                {
                    errors.Add("color scheme value must be a valid number or '-'.");
                    invalidControls.Add(txtcolorscheme);
                }
                if (string.IsNullOrEmpty(txtfillercondition.Text) || (txtfillercondition.Text != "-" && !double.TryParse(txtfillercondition.Text, out _)))
                {
                    errors.Add("filler condition value must be a valid number or '-'.");
                    invalidControls.Add(txtfillercondition);
                }
                if (string.IsNullOrEmpty(txtinnersheath.Text) || (txtinnersheath.Text != "-" && !double.TryParse(txtinnersheath.Text, out _)))
                {
                    errors.Add("inner sheath condition value must be a valid number or '-'.");
                    invalidControls.Add(txtinnersheath);
                }
                if (string.IsNullOrEmpty(txtarmouring.Text) || (txtarmouring.Text != "-" && !double.TryParse(txtarmouring.Text, out _)))
                {
                    errors.Add("armouring condition value must be a valid number or '-'.");
                    invalidControls.Add(txtarmouring);
                }
                if (string.IsNullOrEmpty(txtmarkingcondition.Text) || (txtmarkingcondition.Text != "-" && !double.TryParse(txtmarkingcondition.Text, out _)))
                {
                    errors.Add("marking condition value must be a valid number or '-'.");
                    invalidControls.Add(txtmarkingcondition);
                }
                if (string.IsNullOrEmpty(txtidentification.Text) || (txtidentification.Text != "-" && !double.TryParse(txtidentification.Text, out _)))
                {
                    errors.Add("identification value must be a valid number or '-'.");
                    invalidControls.Add(txtidentification);
                }
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
                foreach (var control in invalidControls)
                {
                    if (control is TextBox txt)
                    {
                        txt.BorderColor = System.Drawing.Color.FromArgb(255, 150, 150);
                        txt.BorderStyle = BorderStyle.Solid;
                        txt.BorderWidth = 1;
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
        protected void btnAddToGrid_Click(object sender, EventArgs e)
        {
            // Reset previous error styles
            ResetControlBorders();

            bool hasError = false;
            List<Control> invalidControls = new List<Control>();

            // Validate fields
            if (string.IsNullOrWhiteSpace(txtconductorcondition.Text))
            {
                invalidControls.Add(txtconductorcondition);
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(txtCondition.Text))
            {
                invalidControls.Add(txtCondition);
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(txtcolorcombination.Text))
            {
                invalidControls.Add(txtcolorcombination);
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(txtfillercondition.Text))
            {
                invalidControls.Add(txtfillercondition);
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(txtinnersheath.Text))
            {
                invalidControls.Add(txtinnersheath);
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(txtarmouring.Text))
            {
                invalidControls.Add(txtarmouring);
                hasError = true;
            }

            


            if (string.IsNullOrWhiteSpace(txtcolorscheme.Text))
            {
                invalidControls.Add(txtcolorscheme);
                hasError = true;
            }


            if (string.IsNullOrWhiteSpace(txtsheath.Text))
            {
                invalidControls.Add(txtsheath);
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(txtmarkingcondition.Text))
            {
                invalidControls.Add(txtmarkingcondition);
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(txtidentification.Text))
            {
                invalidControls.Add(txtidentification);
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(txtdistance.Text))
            {
                invalidControls.Add(txtdistance);
                hasError = true;
            }

            // Highlight invalid fields
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
                ShowMessage("Please fill in all required fields.");
                return;
            }

            // Retrieve or create the DataTable from ViewState
            DataTable dt = ViewState["TestResults"] as DataTable;
            if (dt == null)
            {
                dt = new DataTable();
                dt.Columns.Add("ConductorCondition", typeof(string));
                dt.Columns.Add("InsulationCondition", typeof(string));
                dt.Columns.Add("ColorScheme", typeof(string));
                dt.Columns.Add("ColorCombination", typeof(string));
                dt.Columns.Add("FillerCondition", typeof(string));
                dt.Columns.Add("InnerSheathCondition", typeof(string));
                dt.Columns.Add("ArmouringCondition", typeof(string));
                dt.Columns.Add("SheathCondition", typeof(string));
                dt.Columns.Add("MarkingCondition", typeof(string));
                dt.Columns.Add("Identification", typeof(string));
                dt.Columns.Add("Distance", typeof(string));
            }
         

            DataRow dr = dt.NewRow();
            dr["ConductorCondition"] = txtconductorcondition.Text.Trim();
            dr["InsulationCondition"] = txtCondition.Text.Trim();
            dr["ColorScheme"] = txtCondition.Text.Trim();
            dr["ColorCombination"] = txtcolorcombination.Text.Trim();
            dr["FillerCondition"] = txtfillercondition.Text.Trim();
            dr["InnerSheathCondition"] = txtinnersheath.Text.Trim();
            dr["ArmouringCondition"] = txtarmouring.Text.Trim();
            dr["SheathCondition"] = txtsheath.Text.Trim();
            dr["MarkingCondition"] = txtmarkingcondition.Text.Trim();
            dr["Identification"] = txtidentification.Text.Trim();
            dr["Distance"] = txtdistance.Text.Trim();


            dt.Rows.Add(dr);

            // Save back to ViewState and bind to GridView
            ViewState["TestResults"] = dt;
            gvDimensionConstruction.DataSource = dt;
            gvDimensionConstruction.DataBind();

            // Clear fields
            //  ClearInputFields();
        }
        private bool Submitform()
        {
            try
            {
                // Get the DataTable from ViewState
                DataTable dt = ViewState["TestResults"] as DataTable;

                if (dt == null || dt.Rows.Count == 0)
                {
                    // Show error message if no data to save
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

                // Prepare arrays for the AddTestResultGlobal method
                List<string> reportParamMapList = new List<string>();
                List<string> resultList = new List<string>();

                // Collect all core phases in order
                string Conductorcondition = string.Join(",", dt.AsEnumerable().Select(r => r["Conductorcondition"].ToString()));

                // Collect all temperature values in order
                string InsulationCondition = string.Join(",", dt.AsEnumerable().Select(r => r["InsulationCondition"].ToString()));

                // Collect all duration values in order
                string ColorScheme = string.Join(",", dt.AsEnumerable().Select(r => r["ColorScheme"].ToString()));

                // Collect all before heating values in order
                string ColorCombination = string.Join(",", dt.AsEnumerable().Select(r => r["ColorCombination"].ToString()));

                // Collect all after heating values in order
                string FillerCondition = string.Join(",", dt.AsEnumerable().Select(r => r["FillerCondition"].ToString()));
                string ArmouringCondition = string.Join(",", dt.AsEnumerable().Select(r => r["ArmouringCondition"].ToString()));



                string InnerSheathCondition = string.Join(",", dt.AsEnumerable().Select(r => r["InnerSheathCondition"].ToString()));

                string SheathCondition = string.Join(",", dt.AsEnumerable().Select(r => r["SheathCondition"].ToString()));

                string MarkingCondition = string.Join(",", dt.AsEnumerable().Select(r => r["MarkingCondition"].ToString()));

                string Identification = string.Join(",", dt.AsEnumerable().Select(r => r["Identification"].ToString()));

                string Distance = string.Join(",", dt.AsEnumerable().Select(r => r["Distance"].ToString()));

                // Collect all result values in order
               
                //// Add core phase data
                //reportParamMapList.Add(" "); // Core phase parameter ID
                //resultList.Add(corePhases);

                // Add other measurements
                reportParamMapList.Add("10221"); resultList.Add(Conductorcondition); // Temperature parameter ID
                reportParamMapList.Add("10222"); resultList.Add(InsulationCondition); // Duration parameter ID
                reportParamMapList.Add("10223"); resultList.Add(ColorScheme); // Before heating parameter ID
                reportParamMapList.Add("10224"); resultList.Add(ColorCombination); // After heating parameter ID
                reportParamMapList.Add("10225"); resultList.Add(FillerCondition); // After heating parameter ID
                reportParamMapList.Add("10226"); resultList.Add(InnerSheathCondition); // After heating parameter ID
                reportParamMapList.Add("10227"); resultList.Add(ArmouringCondition); // After heating parameter ID
                reportParamMapList.Add("10228"); resultList.Add(SheathCondition); // After heating parameter ID
                reportParamMapList.Add("10229"); resultList.Add(MarkingCondition); // Results parameter ID
                reportParamMapList.Add("10230"); resultList.Add(Identification); // Results parameter ID
                reportParamMapList.Add("10231"); resultList.Add(Distance); // Results parameter ID



                //Upper section
                reportParamMapList.Add("10232"); resultList.Add(cableType.Text);
                reportParamMapList.Add("10238"); resultList.Add(refSpec.Text);
                reportParamMapList.Add("10233"); resultList.Add(cableSize.Text);
                reportParamMapList.Add("10239"); resultList.Add(testMethodNo.Text);
                reportParamMapList.Add("10234"); resultList.Add(voltage.Text);
                reportParamMapList.Add("10240"); resultList.Add(jobOrderNo.Text);
                reportParamMapList.Add("10241"); resultList.Add(worksheetno.Text);
                reportParamMapList.Add("10236"); resultList.Add(roomno.Text);
                reportParamMapList.Add("10242"); resultList.Add(roomhumidity.Text);
                reportParamMapList.Add("10237"); resultList.Add(roomtemp.Text);
                reportParamMapList.Add("10235"); resultList.Add(Date.Text);
                reportParamMapList.Add("10243"); resultList.Add(equipmentno.Text);

                // Convert lists to arrays
                string[] reportParamMap = reportParamMapList.ToArray();
                string[] result = resultList.ToArray();

                // Call the database save method
                int modified = _cm.AddTestResultPTQualis(_cm, reportParamMap, result);

                if (modified > 0)
                {
                    _cm.sampleStatusID = 27; // Submitted
                    _cm.UpdateCRFSampleDetailsStatus(_cm);

                    _cm.StatusID = 27;
                    _cm.UpdateAllocationStatusid_mechanical(_cm);

                    // Redirect with success alert
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                          "showSuccess", "alert('Test results and Report Generated successfully!');", true);


                    Response.Redirect($"GenerateTestRecord_DimensionAndConstruction.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
                    return true;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "saveFailed", "alert('Failed to save test results.');", true);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(),
                    "exceptionError", $"alert('Error: {ex.Message}');", true);
                return false;
            }
        }
        private void ShowMessage(string message)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + message + "');", true);
        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtconductorcondition.Text = " ";
            txtcolorcombination.Text = "";
            txtarmouring.Text = "";
            txtCondition.Text = "";
            txtfillercondition.Text = "";
            txtidentification.Text = "";
            txtmarkingcondition.Text = "";
            txtinnersheath.Text = "";
            txtdistance.Text = "";
            txtsheath.Text = "";
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            List<string> errors = new List<string>();
            List<Control> invalidControls = new List<Control>();

            // Check if Grid has data
            bool gridHasData = (gvDimensionConstruction.Rows.Count > 0);

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

            // Only validate test details section if grid has no data
            if (!gridHasData)
            {
                if (string.IsNullOrWhiteSpace(txtarmouring.Text) ||
                    (txtarmouring.Text != "-" && !double.TryParse(txtarmouring.Text, out _)))
                {
                    errors.Add("Armouring must be a valid number or '-'");
                    invalidControls.Add(txtarmouring);
                }

                if (string.IsNullOrWhiteSpace(txtcolorcombination.Text) ||
                    (txtcolorcombination.Text != "-" && !double.TryParse(txtcolorcombination.Text, out _)))
                {
                    errors.Add("Color Combination must be a valid number or '-'");
                    invalidControls.Add(txtcolorcombination);
                }

                if (string.IsNullOrWhiteSpace(txtCondition.Text) ||
                    (txtCondition.Text != "-" && !double.TryParse(txtCondition.Text, out _)))
                {
                    errors.Add("Condition must be a valid number or '-'");
                    invalidControls.Add(txtCondition);
                }

                if (string.IsNullOrWhiteSpace(txtfillercondition.Text) ||
                    (txtfillercondition.Text != "-" && !double.TryParse(txtfillercondition.Text, out _)))
                {
                    errors.Add("Filler Condition must be a valid number or '-'");
                    invalidControls.Add(txtfillercondition);
                }

                if (string.IsNullOrWhiteSpace(txtidentification.Text) ||
                    (txtidentification.Text != "-" && !double.TryParse(txtidentification.Text, out _)))
                {
                    errors.Add("Identification must be a valid number or '-'");
                    invalidControls.Add(txtidentification);
                }

                if (string.IsNullOrWhiteSpace(txtinnersheath.Text) ||
                    (txtinnersheath.Text != "-" && !double.TryParse(txtinnersheath.Text, out _)))
                {
                    errors.Add("Inner sheath must be a valid number or '-'");
                    invalidControls.Add(txtinnersheath);
                }

                if (string.IsNullOrWhiteSpace(txtmarkingcondition.Text) ||
                    (txtmarkingcondition.Text != "-" && !double.TryParse(txtmarkingcondition.Text, out _)))
                {
                    errors.Add("Marking condition must be a valid number or '-'");
                    invalidControls.Add(txtmarkingcondition);
                }

                if (string.IsNullOrWhiteSpace(txtsheath.Text) ||
                    (txtsheath.Text != "-" && !double.TryParse(txtsheath.Text, out _)))
                {
                    errors.Add("Sheath must be a valid number or '-'");
                    invalidControls.Add(txtsheath);
                }

                if (string.IsNullOrWhiteSpace(txtdistance.Text) ||
                    (txtdistance.Text != "-" && !double.TryParse(txtdistance.Text, out _)))
                {
                    errors.Add("Distance must be a valid number or '-'");
                    invalidControls.Add(txtdistance);
                }

                if (string.IsNullOrWhiteSpace(txtcolorscheme.Text) ||
                    (txtcolorscheme.Text != "-" && !double.TryParse(txtcolorscheme.Text, out _)))
                {
                    errors.Add("Color Scheme must be a valid number or '-'");
                    invalidControls.Add(txtcolorscheme);
                }
                if (string.IsNullOrWhiteSpace(txtsheath.Text) ||
                   (txtsheath.Text != "-" && !double.TryParse(txtsheath.Text, out _)))
                {
                    errors.Add("Color Scheme must be a valid number or '-'");
                    invalidControls.Add(txtcolorscheme);
                }
                if (string.IsNullOrWhiteSpace(txtconductorcondition.Text) ||
                    (txtconductorcondition.Text != "-" && !double.TryParse(txtconductorcondition.Text, out _)))
                {
                    errors.Add("Conductor Condition must be a valid number or '-'");
                    invalidControls.Add(txtconductorcondition);
                }
            }

            // Require at least one row in the grid
            if (!gridHasData)
            {
                errors.Add("Add at least one test result to the grid.");
            }

            // Reset control borders
            ResetControlBorders();

            // Display errors or proceed
            if (errors.Count > 0)
            {
                foreach (var control in invalidControls)
                {
                    if (control is TextBox txt)
                    {
                        txt.BorderColor = System.Drawing.Color.FromArgb(255, 150, 150);
                        txt.BorderStyle = BorderStyle.Solid;
                        txt.BorderWidth = 1;
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

                        // Create DAL object
                        if (_cm == null) _cm = new DAL.CM();
                        _cm.sampleId = sampleId;

                        // First delete existing records for this sample
                        DeleteExistingRecords(sampleId);

                        bool result = Submitform();
                        if (!result)
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "UpdateError",
                                "alert('Update failed. Please check the input data.');", true);
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
    }
}