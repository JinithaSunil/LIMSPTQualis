using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace LIMS
{
    public partial class MechanicalandElectricalWireTest : System.Web.UI.Page
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
                
                txtNo.Text = "1";
                CheckAndPrefillForm();
            }
        }
        private void InitializeGrid()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("No");
            dt.Columns.Add("Diameter1");
            dt.Columns.Add("Diameter2");
            dt.Columns.Add("Diameter3");
            dt.Columns.Add("Diameter4");
            dt.Columns.Add("Diameter5");
            dt.Columns.Add("Diameter6");
            dt.Columns.Add("Nom");
            dt.Columns.Add("Area");
            dt.Columns.Add("BF");
            dt.Columns.Add("TS");
            dt.Columns.Add("L0");
            dt.Columns.Add("Elmm");
            dt.Columns.Add("StressAt1Percent");
            dt.Columns.Add("Resistance");
            dt.Columns.Add("Resistivity");
            dt.Columns.Add("Wrapping");
            dt.Columns.Add("Torsion");

            ViewState["TestResults"] = dt;
            gvcopper.DataSource = dt;
            gvcopper.DataBind();
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
                        PrefillcopperGrid(sampleId);
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
        private void PrefillUpperSection(DataTable existingData)
        {
            // Upper section
            SetTextBoxText(cableType, "12918");
            SetTextBoxText(refSpec, "12924");
            SetTextBoxText(cableSize, "12919");
            SetTextBoxText(testMethodNo, "12925");
            SetTextBoxText(voltage, "12920");
            SetTextBoxText(jobOrderNo, "12926");
            SetTextBoxText(worksheetno, "12927");
            SetTextBoxText(roomno, "12922");
            SetTextBoxText(roomhumidity, "12928");
            SetTextBoxText(roomtemp, "12923");
            SetTextBoxText(Date, "12921");
            SetTextBoxText(equipmentno, "12929");

            // Standard values
            SetTextBoxText(txtStandardDiameterMin, "12930"); // Min from "min|max"
            SetTextBoxText(txtStandardDiameterMax, "12930"); // Max from "min|max"

            SetTextBoxText(txtStandardDiameterNom, "12931");
            SetTextBoxText(txtStandardcrosssectionarea, "12932");
            SetTextBoxText(txtStandardbreakingforce, "12933");
            SetTextBoxText(txtStandardTensilestrength, "12934");
            SetTextBoxText(txtStandardElongationL0, "12935");
            SetTextBoxText(txtStandardElongationmm, "12936");
            SetTextBoxText(txtStandardstressat1, "12937");
            SetTextBoxText(txtStandardresistance, "12938");
            SetTextBoxText(txtStandardresistivity, "12939");
            SetTextBoxText(txtStandardwrapping, "12940");
            SetTextBoxText(txtStandardtorsion, "12941");

            SetTextBoxText(txtcorrectionTemp, "12942");
            SetTextBoxText(txtminimumbrekaingload, "12943");
            SetTextBoxText(txtstandardfor, "12944");
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

        private void UpdateNumber()
        {
            var dt = ViewState["TestResults"] as DataTable;
            int nextNo = (dt != null) ? dt.Rows.Count + 1 : 1;

            txtNo.Text = nextNo.ToString();
        }
     
        protected void btnAddToGrid_Click(object sender, EventArgs e)
        {
            DataTable dt = ViewState["TestResults"] as DataTable;
            if (dt == null)
            {
                InitializeGrid();
                dt = ViewState["TestResults"] as DataTable;
            }

            string newNo = txtNo.Text.Trim();
            //bool duplicateExists = dt.AsEnumerable().Any(row => row.Field<string>("No") == newNo);
            //if (duplicateExists)
            //{
            //    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('This No. already exists in the grid.');", true);
            //    return;
            //}

            DataRow dr = dt.NewRow();

            dr["No"] = newNo;
            dr["Diameter1"] = txtInput1.Text;
            dr["Diameter2"] = txtInput2.Text;
            dr["Diameter3"] = txtInput3.Text;
            dr["Diameter4"] = txtInput4.Text;
            dr["Diameter5"] = txtInput5.Text;
            dr["Diameter6"] = txtInput6.Text;
            dr["Nom"] = txtNom.Text;  // Keep Nom as user input
            dr["Area"] = txtCrossSectionArea.Text;
            dr["BF"] = txtBreakingForce.Text;
            dr["TS"] = txtTensileStrength.Text;
            dr["L0"] = txtEL0.Text;
            dr["Elmm"] = txtElmm.Text;
            dr["StressAt1Percent"] = txtStressAt1Percent.Text;
            dr["Resistance"] = txtResistance.Text;
            dr["Resistivity"] = txtResistivity.Text;
            dr["Wrapping"] = txtWrapping.Text;
            dr["Torsion"] = txtTorsion.Text;

            dt.Rows.Add(dr);
            ViewState["TestResults"] = dt;

            gvcopper.DataSource = dt;
            gvcopper.DataBind();

            SetNextAvailableNo(dt);


            // Update diameters and other standard fields (without average)
            UpdateStandardDiameterFields(dt);
            UpdateStandardFields(dt);
        }
        private void SetNextAvailableNo(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                txtNo.Text = "1";
                return;
            }

            var existingNumbers = dt.AsEnumerable()
                .Select(r => int.TryParse(r["No"].ToString(), out int val) ? val : 0)
                .Where(v => v > 0)
                .OrderBy(v => v)
                .ToList();

            int nextNo = 1;
            foreach (int num in existingNumbers)
            {
                if (num == nextNo)
                    nextNo++;
                else
                    break;
            }

            txtNo.Text = nextNo.ToString();
        }
        private void UpdateStandardDiameterFields(DataTable dt)
        {
            List<double> allDiameters = new List<double>();

            foreach (DataRow row in dt.Rows)
            {
                for (int i = 1; i <= 6; i++)
                {
                    string columnName = "Diameter" + i;
                    if (double.TryParse(row[columnName]?.ToString(), out double val))
                    {
                        allDiameters.Add(val);
                    }
                }
            }

            if (allDiameters.Count > 0)
            {
                double min = allDiameters.Min();
                double max = allDiameters.Max();

                txtStandardDiameterMin.Text = min.ToString("0.###");
                txtStandardDiameterMax.Text = max.ToString("0.###");

                // Clear Nom textbox as average is NOT calculated
                txtStandardDiameterNom.Text = string.Empty;
            }
            else
            {
                txtStandardDiameterMin.Text = string.Empty;
                txtStandardDiameterMax.Text = string.Empty;
                txtStandardDiameterNom.Text = string.Empty;
            }
        }
        private void UpdateStandardFields(DataTable dt)
        {
            List<double> breakingForces = new List<double>();
            List<double> tensileStrengths = new List<double>();
            List<double> elongations = new List<double>();
            List<double> stressAt1Percents = new List<double>();
            List<double> resistivities = new List<double>();
            List<double> torsions = new List<double>();

            foreach (DataRow row in dt.Rows)
            {
                if (double.TryParse(row["BF"]?.ToString(), out double bf))
                    breakingForces.Add(bf);

                if (double.TryParse(row["TS"]?.ToString(), out double ts))
                    tensileStrengths.Add(ts);

                if (double.TryParse(row["Elmm"]?.ToString(), out double el))
                    elongations.Add(el);

                if (double.TryParse(row["StressAt1Percent"]?.ToString(), out double stress))
                    stressAt1Percents.Add(stress);

                if (double.TryParse(row["Resistivity"]?.ToString(), out double resistivity))
                    resistivities.Add(resistivity);

                if (double.TryParse(row["Torsion"]?.ToString(), out double torsion))
                    torsions.Add(torsion);
            }

            txtStandardbreakingforce.Text = breakingForces.Any() ? breakingForces.Min().ToString("0.###") : "";
            txtStandardTensilestrength.Text = tensileStrengths.Any() ? tensileStrengths.Min().ToString("0.###") : "";
            txtStandardElongationmm.Text = elongations.Any() ? elongations.Min().ToString("0.###") : "";
            txtStandardstressat1.Text = stressAt1Percents.Any() ? stressAt1Percents.Min().ToString("0.###") : "";
            txtStandardresistivity.Text = resistivities.Any() ? resistivities.Max().ToString("0.###") : "";
            txtStandardtorsion.Text = torsions.Any() ? torsions.Min().ToString("0.###") : "";
        }
        protected void gvcopper_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                if (ViewState["StandardMin"] != null &&
                    ViewState["StandardMax"] != null)
                {
                    e.Row.Cells[0].Text = "Standard";
                    e.Row.Cells[1].Text = ViewState["StandardMin"].ToString();
                    // Skip Nom (average) cell or clear it if needed:
                    e.Row.Cells[2].Text = ""; // No average (Nom)
                    e.Row.Cells[6].Text = ViewState["StandardMax"].ToString();

                    e.Row.Font.Bold = true;
                    e.Row.BackColor = System.Drawing.Color.LightGray;
                }
            }
        }
        private void PrefillcopperGrid(int sampleId)
        {
            try
            {
                // Updated paramMap with your IDs and field names:
                var paramMap = new Dictionary<int, string>
        {
            { 12906, "Diameter" },
            { 12907, "Nom" },               
            { 12908, "Area" },              
            { 12909, "BF" },               
            { 12910, "TS" },              
            { 12911, "L0" },               
            { 12912, "Elmm" },             // elmm is saved in el1 parameter
            { 12913, "StressAt1Percent" }, // Stressat1% mapped
            { 12914, "Resistance" },
            { 12915, "Resistivity" },
            { 12916, "Wrapping" },
            { 12917, "Torsion" }
       
        };

                Dictionary<int, string> allResults = new Dictionary<int, string>();

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT ReportParamMapID, Result FROM TestResultsPTQualis WHERE SampleID = @SampleID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SampleID", sampleId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int paramId = Convert.ToInt32(reader["ReportParamMapID"]);
                                string result = reader["Result"].ToString();
                                allResults[paramId] = result;
                            }
                        }
                    }
                }

                DataTable dt = new DataTable();

                // Setup columns to match your GridView
                dt.Columns.Add("No", typeof(int));
                dt.Columns.Add("Diameter1");
                dt.Columns.Add("Diameter2");
                dt.Columns.Add("Diameter3");
                dt.Columns.Add("Diameter4");
                dt.Columns.Add("Diameter5");
                dt.Columns.Add("Diameter6");
                dt.Columns.Add("Nom");  // DiameterNom
                dt.Columns.Add("Area"); // CrossSectionArea
                dt.Columns.Add("BF");   // BreakingLoad
                dt.Columns.Add("TS");   // Tensilestrength
                dt.Columns.Add("L0");   // ElongationL0
                dt.Columns.Add("Elmm"); // ElongationL1
                dt.Columns.Add("StressAt1Percent");
                dt.Columns.Add("Resistance");
                dt.Columns.Add("Resistivity");
                dt.Columns.Add("Wrapping");
                dt.Columns.Add("Torsion");

                // Get specimen count from one of the params 
                if (!allResults.ContainsKey(12906) || string.IsNullOrWhiteSpace(allResults[12906]))
                {
                    // No diameter data, bind empty table
                    gvcopper.DataSource = dt;
                    gvcopper.DataBind();
                    return;
                }

                string[] specimens = allResults[12906].Split(';');

                int rowIndex = 1;

                foreach (var specimenData in specimens)
                {
                    DataRow row = dt.NewRow();
                    row["No"] = rowIndex++;

                    // Parse diameters (comma separated)
                    string[] diameters = specimenData.Split(',');

                    for (int i = 0; i < diameters.Length && i < 6; i++)
                    {
                        row[$"Diameter{i + 1}"] = diameters[i].Trim();
                    }

                    // For other params, split by '|' and match index of specimen
                    foreach (var param in paramMap)
                    {
                        if (param.Key == 12906) continue; // Skip Diameter since handled

                        if (!allResults.ContainsKey(param.Key)) continue;

                        string[] paramValues = allResults[param.Key].Split(';');

                        if (paramValues.Length > (rowIndex - 2)) // zero-based index
                        {
                            row[param.Value] = paramValues[rowIndex - 2].Trim();
                        }
                    }

                    dt.Rows.Add(row);
                }

                gvcopper.DataSource = dt;
                gvcopper.DataBind();

                ViewState["TestResults"] = dt;
                UpdateStandardDiameterFields(dt);
                UpdateStandardFields(dt);

            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "GridPrefillError",
                    $"alert('Error loading test results: {ex.Message}');", true);
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
        protected void btnSubmit_Click(object sender, EventArgs e)
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
        private bool Submitform()
        {
            try
            {
                DataTable dt = ViewState["TestResults"] as DataTable;
                if (dt == null || dt.Rows.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "err", "alert('No data found.');", true);
                    return false;
                }

                if (_cm == null) _cm = new DAL.CM();
                if (Request.QueryString["s"] != null) _cm.sampleId = Convert.ToInt32(Request.QueryString["s"]);
                if (Request.QueryString["r"] != null) _cm.ReportID = Request.QueryString["r"];

                DataTable dt1 = _cm.Getdataforseriesandreportid_mechanical(_cm);
                if (dt1.Rows.Count > 0)
                {
                    _cm.sampleIDSeries = dt1.Rows[0]["sampleIDSeries"].ToString();
                    _cm.CRFSampleId = Convert.ToInt32(dt1.Rows[0]["CRFSampleId"]);
                }

                List<string> reportParamMapList = new List<string>();
                List<string> resultList = new List<string>();

                // Diameter (multi-input, comma-separated per specimen, ; separated between specimens)
                reportParamMapList.Add("12906");
                resultList.Add(string.Join(";", dt.AsEnumerable().Select(row =>
                    string.Join(",", new string[] {
                row["Diameter1"].ToString(),
                row["Diameter2"].ToString(),
                row["Diameter3"].ToString(),
                row["Diameter4"].ToString(),
                row["Diameter5"].ToString(),
                row["Diameter6"].ToString()
                    })
                )));

                // Other single values per specimen, separated by ;
                reportParamMapList.Add("12907"); resultList.Add(string.Join(";", dt.AsEnumerable().Select(r => r["Nom"].ToString().Trim())));             // DiameterNom
                reportParamMapList.Add("12908"); resultList.Add(string.Join(";", dt.AsEnumerable().Select(r => r["Area"].ToString().Trim())));            // CrossSectionArea
                reportParamMapList.Add("12909"); resultList.Add(string.Join(";", dt.AsEnumerable().Select(r => r["BF"].ToString().Trim())));              // BreakingLoad
                reportParamMapList.Add("12910"); resultList.Add(string.Join(";", dt.AsEnumerable().Select(r => r["TS"].ToString().Trim())));              // TensileStrength
                reportParamMapList.Add("12911"); resultList.Add(string.Join(";", dt.AsEnumerable().Select(r => r["L0"].ToString().Trim())));              // ElongationL0
                reportParamMapList.Add("12912"); resultList.Add(string.Join(";", dt.AsEnumerable().Select(r => r["Elmm"].ToString().Trim())));            // ElongationL1 (mapped to Elmm)
                reportParamMapList.Add("12913"); resultList.Add(string.Join(";", dt.AsEnumerable().Select(r => r["StressAt1Percent"].ToString().Trim()))); // Stressat1%
                reportParamMapList.Add("12914"); resultList.Add(string.Join(";", dt.AsEnumerable().Select(r => r["Resistance"].ToString().Trim())));       // Resistance
                reportParamMapList.Add("12915"); resultList.Add(string.Join(";", dt.AsEnumerable().Select(r => r["Resistivity"].ToString().Trim())));      // Resistivity
                reportParamMapList.Add("12916"); resultList.Add(string.Join(";", dt.AsEnumerable().Select(r => r["Wrapping"].ToString().Trim())));         // Wrapping
                reportParamMapList.Add("12917"); resultList.Add(string.Join(";", dt.AsEnumerable().Select(r => r["Torsion"].ToString().Trim())));          // Torsion

            
                reportParamMapList.Add("12918"); resultList.Add(cableType.Text);        // Type
                reportParamMapList.Add("12919"); resultList.Add(cableSize.Text);        // Size
                reportParamMapList.Add("12920"); resultList.Add(voltage.Text);          // Voltage
                reportParamMapList.Add("12921"); resultList.Add(Date.Text);             // Date
                reportParamMapList.Add("12922"); resultList.Add(roomno.Text);           // RoomNo
                reportParamMapList.Add("12923"); resultList.Add(roomtemp.Text);         // RoomTemp
                reportParamMapList.Add("12924"); resultList.Add(refSpec.Text);          // RefSpec
                reportParamMapList.Add("12925"); resultList.Add(testMethodNo.Text);     // TestMethodNo
                reportParamMapList.Add("12926"); resultList.Add(jobOrderNo.Text);       // JobOrderNo
                reportParamMapList.Add("12927"); resultList.Add(worksheetno.Text);      // WorkSheetNo
                reportParamMapList.Add("12928"); resultList.Add(roomhumidity.Text);     // RoomHumidity
                reportParamMapList.Add("12929"); resultList.Add(equipmentno.Text);      // EquipmentNo
                                                                                        // SpecifiedDiameter: Min|Max format
                reportParamMapList.Add("12930");
                resultList.Add(txtStandardDiameterMin.Text.Trim() + "|" + txtStandardDiameterMax.Text.Trim());

                // SpecifiedDiameterNom
                reportParamMapList.Add("12931");
                resultList.Add(txtStandardDiameterNom.Text.Trim());

                // SpecifiedArea
                reportParamMapList.Add("12932");
                resultList.Add(txtStandardcrosssectionarea.Text.Trim());   // cross section are is save under are parameter name

                // SpecifiedBF
                reportParamMapList.Add("12933");
                resultList.Add(txtStandardbreakingforce.Text.Trim());

                // SpecifiedTS
                reportParamMapList.Add("12934");
                resultList.Add(txtStandardTensilestrength.Text.Trim());

                // SpecifiedL0
                reportParamMapList.Add("12935");
                resultList.Add(txtStandardElongationL0.Text.Trim());

                // SpecifiedL1
                reportParamMapList.Add("12936");
                resultList.Add(txtStandardElongationmm.Text.Trim());

                // SpecifiedStressat1%
                reportParamMapList.Add("12937");
                resultList.Add(txtStandardstressat1.Text.Trim());

                // SpecifiedResistance
                reportParamMapList.Add("12938");
                resultList.Add(txtStandardresistance.Text.Trim());

                // SpecifiedResistivity
                reportParamMapList.Add("12939");
                resultList.Add(txtStandardresistivity.Text.Trim());

                // SpecifiedWrapping
                reportParamMapList.Add("12940");
                resultList.Add(txtStandardwrapping.Text.Trim());

           
                reportParamMapList.Add("12941");
                resultList.Add(txtStandardtorsion.Text.Trim());

               
                reportParamMapList.Add("12942");
                resultList.Add(txtcorrectionTemp.Text.Trim());
         
                reportParamMapList.Add("12943");
                resultList.Add(txtminimumbrekaingload.Text.Trim());

                reportParamMapList.Add("12944");
                resultList.Add(txtstandardfor.Text.Trim());


                // Convert to arrays
                string[] reportParamMap = reportParamMapList.ToArray();
                string[] result = resultList.ToArray();

                // Save results via DAL
                int modified = _cm.AddTestResultPTQualis(_cm, reportParamMap, result);
                if (modified > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "success", "alert('Test results saved and report generated successfully.');", true);

                    _cm.sampleStatusID = 27;
                    _cm.UpdateCRFSampleDetailsStatus(_cm);

                    _cm.StatusID = 27;
                    _cm.UpdateAllocationStatusid_mechanical(_cm);

                    Response.Redirect("GenerateTestRecord_MechanicalandElectricalWireTest.aspx?s=" + _cm.sampleId + "&r=" + _cm.ReportID);
                    return true;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "fail", "alert('Failed to save test results.');", true);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "err", $"alert('Error: {ex.Message}');", true);
                return false;
            }
        }
        protected void btnUpdate_Click(object sender, EventArgs e)
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
                    ScriptManager.RegisterStartupScript(this, GetType(), "UpdateError", "alert('Sample ID not found.');", true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "UpdateError", $"alert('Error: {ex.Message}');", true);
            }
        }
        private void ProcessColumn(DataTable dt, int currentRowIndex, GridViewRow currentRow, int columnIndex)
        {

        }
        protected void gvcopper_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);

                DataTable dt = ViewState["TestResults"] as DataTable;

                if (dt != null && dt.Rows.Count > 0 && rowIndex >= 0 && rowIndex < dt.Rows.Count)
                {
                    dt.Rows.RemoveAt(rowIndex);  // Remove the row at the specified index
                    dt.AcceptChanges();

                    ViewState["TestResults"] = dt;

                    gvcopper.DataSource = dt;
                    gvcopper.DataBind();
                    SetNextAvailableNo(dt);
                    UpdateNumber();

                   
                    UpdateStandardDiameterFields(dt);
                    UpdateStandardFields(dt);
                }
            }
        }
        private void ShowMessage(string message)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('{message}');", true);
        }
        protected void btnClearData_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
        protected void gvcopper_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                GridView grid = sender as GridView;

                // Hide the original header row (the default single header row)
                e.Row.Visible = false;

                // First header row
                GridViewRow firstHeaderRow = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal);
                firstHeaderRow.Cells.Add(CreateHeaderCell("No.", 3, 1));
                firstHeaderRow.Cells.Add(CreateHeaderCell("Diameter (mm)", 1, 7));
                firstHeaderRow.Cells.Add(CreateHeaderCell("Cross-sectional area<br/>mm²", 3, 1));
                firstHeaderRow.Cells.Add(CreateHeaderCell("Breaking force<br/>Kg / N", 3, 1));
                firstHeaderRow.Cells.Add(CreateHeaderCell("Tensile strength<br/>Kg/mm²<br/>N/mm²", 3, 1));
                firstHeaderRow.Cells.Add(CreateHeaderCell("Elongation", 1, 2));
                firstHeaderRow.Cells.Add(CreateHeaderCell("Stress at 1%<br/>Kg/mm²", 3, 1));
                firstHeaderRow.Cells.Add(CreateHeaderCell("Resistance<br/>Ω.m", 3, 1));
                firstHeaderRow.Cells.Add(CreateHeaderCell("Resistivity<br/>Ω·mm²", 3, 1));
                firstHeaderRow.Cells.Add(CreateHeaderCell("Wrapping<br/>…./…./….", 3, 1));
                firstHeaderRow.Cells.Add(CreateHeaderCell("Torsion", 3, 1));
                firstHeaderRow.Cells.Add(CreateHeaderCell("Add", 3, 1));

                grid.Controls[0].Controls.AddAt(0, firstHeaderRow);

                // Second header row (diameter numbers + elongation sub-headers)
                GridViewRow secondHeaderRow = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Normal);
                for (int i = 1; i <= 6; i++)
                {
                    secondHeaderRow.Cells.Add(CreateHeaderCell(i.ToString(), 1, 1));
                }
                secondHeaderRow.Cells.Add(CreateHeaderCell("Nom", 1, 1)); // 7th diameter col

                // Elongation sub headers span two rows, so here only placeholders
                secondHeaderRow.Cells.Add(CreateHeaderCell("L₀<br/>mm", 2, 1));
                secondHeaderRow.Cells.Add(CreateHeaderCell("mm<br/>%", 2, 1));

                grid.Controls[0].Controls.AddAt(1, secondHeaderRow);

                // Third header row (units for diameter)
                GridViewRow thirdHeaderRow = new GridViewRow(2, 0, DataControlRowType.Header, DataControlRowState.Normal);
                for (int i = 0; i < 7; i++) // 7 diameter columns
                {
                    thirdHeaderRow.Cells.Add(CreateHeaderCell("mm", 1, 1));
                }

                // The remaining header cells here have rowspan=2 in second header row, so no more cells here.

                grid.Controls[0].Controls.AddAt(2, thirdHeaderRow);
            }
        }

        private TableCell CreateHeaderCell(string text, int rowspan, int colspan)
        {
            TableCell cell = new TableCell();
            cell.Text = text;
            if (rowspan > 1) cell.RowSpan = rowspan;
            if (colspan > 1) cell.ColumnSpan = colspan;

            cell.HorizontalAlign = HorizontalAlign.Center;
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.Font.Bold = true;

            // Add Bootstrap classes for light gray header look
            cell.CssClass = "table-light"; // Bootstrap class for light table header background
            cell.Attributes["style"] = "border: 1px solid #dee2e6;"; // Bootstrap-like border

            return cell;
        }

        private void ClearForm()
        {
            // Upper section
            Date.Text = "";
            roomno.Text = "";
            roomhumidity.Text = "";
            roomtemp.Text = "";
            worksheetno.Text = "";
            equipmentno.Text = "";
            cableType.Text = "";
            cableSize.Text = "";
            voltage.Text = "";
            refSpec.Text = "";
            testMethodNo.Text = "";
            jobOrderNo.Text = "";
            equipmentno.Text = "";

            // Standard (specified) values
            txtStandardDiameterMin.Text = "";
            txtStandardDiameterNom.Text = "";
            txtStandardDiameterMax.Text = "";
            txtStandardcrosssectionarea.Text = "";
            txtStandardbreakingforce.Text = "";
            txtStandardTensilestrength.Text = "";
            txtStandardElongationL0.Text = "";
            txtStandardElongationmm.Text = "";
            txtStandardstressat1.Text = "";
            txtStandardresistance.Text = "";
            txtStandardresistivity.Text = "";
            txtStandardwrapping.Text = "";
            txtStandardtorsion.Text = "";

            txtcorrectionTemp.Text = "";
            txtminimumbrekaingload.Text = "";
            txtstandardfor.Text = "";

            // Specimen entry row (if used separately before adding to grid)
            txtNo.Text = "";
            txtInput1.Text = "";
            txtInput2.Text = "";
            txtInput3.Text = "";
            txtInput4.Text = "";
            txtInput5.Text = "";
            txtInput6.Text = "";
            txtNom.Text = "";
            txtCrossSectionArea.Text = "";
            txtBreakingForce.Text = "";
            txtTensileStrength.Text = "";
            txtEL0.Text = "";
            txtElmm.Text = "";
            txtStressAt1Percent.Text = "";
            txtResistance.Text = "";
            txtResistivity.Text = "";
            txtWrapping.Text = "";
            txtTorsion.Text = "";
            ViewState["TestResults"] = null;
            // Clear GridView
            gvcopper.DataSource = null;
            gvcopper.DataBind();
        }



    }
}