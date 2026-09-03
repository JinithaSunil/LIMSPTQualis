using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LIMS;

namespace LIMS
{
    public partial class Copper_AluminiumWire : System.Web.UI.Page
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
                ViewState["LastCoil"] = txtCoil1.Text;
                txtSpecimenNo.Text = "1";
                CheckAndPrefillForm();
            }
        }
        private void InitializeGrid()
        {
            // Create a DataTable to store the test results based on new structure
            DataTable dt = new DataTable();

            dt.Columns.Add("Date", typeof(string));
            dt.Columns.Add("CoilNo", typeof(string));
            dt.Columns.Add("SpecimenNo", typeof(string));

            dt.Columns.Add("Diameter1", typeof(string));
            dt.Columns.Add("Diameter2", typeof(string));
            dt.Columns.Add("Diameter3", typeof(string));
            dt.Columns.Add("Diameter4", typeof(string));
            dt.Columns.Add("Diameter5", typeof(string));
            dt.Columns.Add("Diameter6", typeof(string));
            dt.Columns.Add("Nom", typeof(string));

            dt.Columns.Add("Area", typeof(string));
            dt.Columns.Add("BF", typeof(string));
            dt.Columns.Add("TS", typeof(string));

            dt.Columns.Add("L0", typeof(string));
            dt.Columns.Add("L1", typeof(string));
            dt.Columns.Add("EL", typeof(string));

            dt.Columns.Add("MTS", typeof(string));
            dt.Columns.Add("MEL", typeof(string));

            dt.Columns.Add("Resistance", typeof(string));
            dt.Columns.Add("Temp", typeof(string));
            dt.Columns.Add("Kt", typeof(string));
            dt.Columns.Add("Resistivity", typeof(string));
            dt.Columns.Add("Conductivity", typeof(string));
            dt.Columns.Add("Density", typeof(string));

            // Store the DataTable in ViewState
            ViewState["TestResults"] = dt;

            // Bind the empty DataTable to the GridView
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
        { //Upper section


            SetTextBoxText(cableType, "12887");
            SetTextBoxText(refSpec, "12883");
            SetTextBoxText(cableSize, "12878");
            SetTextBoxText(testMethodNo, "12884");
            SetTextBoxText(voltage, "12879");
            SetTextBoxText(jobOrderNo, "12885");
            SetTextBoxText(worksheetno, "12886");
            SetTextBoxText(roomno, "12881");
            SetTextBoxText(roomhumidity, "12887");
            SetTextBoxText(roomtemp, "12882");
            SetTextBoxText(Date, "12880");
            SetTextBoxText(equipmentno, "12888");

            SetTextBoxText(txtSpecifiedDiameter, "12890");
            SetTextBoxText(txtSpecifiedDiameterNom, "12891");
            SetTextBoxText(txtSpecifiedArea, "12892");
            SetTextBoxText(txtSpecifiedBf, "12893");
            SetTextBoxText(txtSpecifiedTS, "12894");
            SetTextBoxText(txtSpecifiedL0, "12895");
            SetTextBoxText(txtSpecifiedL1, "12896");
            SetTextBoxText(txtSpecifiedEL, "12897");
            SetTextBoxText(txtSpecifiedMedianTS, "12898");
            SetTextBoxText(txtSpecifiedMedianEL, "12899");
            SetTextBoxText(txtSpecifiedResistance, "12900");
            SetTextBoxText(txtSpecifiedTemp, "12901");
            SetTextBoxText(txtSpecifiedKt, "12902");
            SetTextBoxText(txtSpecifiedResistivityat20C, "12903");
            SetTextBoxText(txtSpecifiedConductivity, "12904");
            SetTextBoxText(txtSpecifiedDensity, "12905");


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
        protected void txtCoil1_TextChanged(object sender, EventArgs e)
        {
            string currentCoil = txtCoil1.Text;
            string lastCoil = ViewState["LastCoil"] as string;

            // If coil has changed, reset specimen number to 1
            if (lastCoil == null || lastCoil != currentCoil)
            {
                txtSpecimenNo.Text = "1";
                ViewState["LastCoil"] = currentCoil;
            }
        }
        protected void btnAddToGrid_Click(object sender, EventArgs e)
        {
            DataTable dt = ViewState["TestResults"] as DataTable;
            if (dt == null)
            {
                dt = new DataTable();
                dt.Columns.Add("Date");
                dt.Columns.Add("CoilNo");
                dt.Columns.Add("SpecimenNo");
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
                dt.Columns.Add("L1");
                dt.Columns.Add("EL");
                dt.Columns.Add("MTS");
                dt.Columns.Add("MEL");
                dt.Columns.Add("Resistance");
                dt.Columns.Add("Temp");
                dt.Columns.Add("Kt");
                dt.Columns.Add("Resistivity");
                dt.Columns.Add("Conductivity");
                dt.Columns.Add("Density");
            }

            // Parse and format the date just once
            string currentDate;
            DateTime parsedDate;
            if (DateTime.TryParse(txtDate1.Text.Trim(), out parsedDate))
                currentDate = parsedDate.ToString("dd-MM-yyyy");
            else
                currentDate = txtDate1.Text.Trim(); // fallback

            string currentCoil = txtCoil1.Text.Trim();

            // Get all rows with same date and coil
            int specimenNo = 1;
            if (dt.Rows.Count > 0)
            {
                var sameCoilRows = dt.Select($"Date = '{currentDate}' AND CoilNo = '{currentCoil}'");
                specimenNo = sameCoilRows.Length + 1;
            }

            // Add new row
            DataRow dr = dt.NewRow();

            dr["Date"] = currentDate;
            dr["CoilNo"] = currentCoil;
            dr["SpecimenNo"] = specimenNo.ToString();
            dr["Diameter1"] = txtInput1.Text;
            dr["Diameter2"] = txtInput2.Text;
            dr["Diameter3"] = txtInput3.Text;
            dr["Diameter4"] = txtInput4.Text;
            dr["Diameter5"] = txtInput5.Text;
            dr["Diameter6"] = txtInput6.Text;
            dr["Nom"] = txtNom1.Text;
            dr["Area"] = txtArea.Text;
            dr["BF"] = txtBF.Text;
            dr["TS"] = txtTS.Text;
            dr["L0"] = txtL0.Text;
            dr["L1"] = txtL1.Text;
            dr["EL"] = txtEL.Text;
            dr["MTS"] = txtMTS.Text;
            dr["MEL"] = txtMEL.Text;
            dr["Resistance"] = txtRes.Text;
            dr["Temp"] = txtTemp.Text;
            dr["Kt"] = txtKt.Text;
            dr["Resistivity"] = txtResist.Text;
            dr["Conductivity"] = txtCond.Text;
            dr["Density"] = txtDens.Text;

            dt.Rows.Add(dr);
            ViewState["TestResults"] = dt;

            gvcopper.DataSource = dt;
            gvcopper.DataBind();

            UpdateStandardFields(dt);

            // Update last date/coil in ViewState
            ViewState["LastDate"] = currentDate;
            ViewState["LastCoil"] = currentCoil;

            // Set the next specimen number in the textbox
            txtSpecimenNo.Text = (specimenNo + 1).ToString();
        }

        private void UpdateStandardFields(DataTable dt)
        {
            List<double> breakingForces = new List<double>();
            List<double> tensileStrengths = new List<double>();
            List<double> elongations = new List<double>();
            List<double> MedianTS = new List<double>();
            List<double> MedianEl = new List<double>();
            

            foreach (DataRow row in dt.Rows)
            {
                

                if (double.TryParse(row["TS"]?.ToString(), out double ts))
                    tensileStrengths.Add(ts);

                if (double.TryParse(row["El"]?.ToString(), out double el))
                    elongations.Add(el);

                if (double.TryParse(row["MTS"]?.ToString(), out double mts))
                    MedianTS.Add(mts); 

                if (double.TryParse(row["MEL"]?.ToString(), out double mel))
                    MedianEl.Add(mel);  



            }

            txtSpecifiedTS.Text = tensileStrengths.Any() ? tensileStrengths.Min().ToString("0.###") : "";
            txtSpecifiedEL.Text = elongations.Any() ? elongations.Min().ToString("0.###") : "";
            txtSpecifiedMedianTS.Text = MedianTS.Any() ? MedianTS.Min().ToString("0.###") : "";
            txtSpecifiedMedianEL.Text = MedianEl.Any() ? MedianEl.Min().ToString("0.###") : "";
           
        }
        protected void gvcopper_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && e.Row.RowIndex > 0)
            {
       
                DataTable dt = gvcopper.DataSource as DataTable;

                if (dt == null)
                    return;

            
                ProcessColumn(dt, e.Row.RowIndex, e.Row, 0); // Column 0: Date
                ProcessColumn(dt, e.Row.RowIndex, e.Row, 1); // Column 1: CoilNo
            }
        }
        private void PrefillcopperGrid(int sampleId)
        {
            try
            {
                // Parameter mapping for copper test results
                var paramMap = new Dictionary<int, string>
        {
            { 12889, "Date" },
            { 12841, "CoilNo" },
            { 12842, "SpecimenNo" },
            { 12843, "Diameter" },
            { 12844, "Nom" },
            { 12845, "Area" },
            { 12846, "BF" },
            { 12847, "TS" },
            { 12848, "Temp" },
            { 12849, "Density" },
            { 12850, "L0" },
            { 12851, "L1" },
            { 12852, "EL" },
            { 12853, "MTS" },
            { 12854, "MEL" },
            { 12855, "Resistance" },
            { 12856, "Kt" },
            { 12857, "Conductivity" },
            { 12858, "Resistivity" }
        };

                // Load results from database
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

                // Create DataTable with grid structure
                DataTable dt = new DataTable();
                dt.Columns.Add("Date");
                dt.Columns.Add("CoilNo");
                dt.Columns.Add("SpecimenNo");
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
                dt.Columns.Add("L1");
                dt.Columns.Add("EL");
                dt.Columns.Add("MTS");
                dt.Columns.Add("MEL");
                dt.Columns.Add("Resistance");
                dt.Columns.Add("Temp");
                dt.Columns.Add("Kt");
                dt.Columns.Add("Resistivity");
                dt.Columns.Add("Conductivity");
                dt.Columns.Add("Density");

                // Parse stored data back to grid format
                if (allResults.ContainsKey(12841) && !string.IsNullOrWhiteSpace(allResults[12841]))
                {
                    // Get coil numbers (format: "1;2")
                    string[] coilNos = allResults[12841].Split(';');

                    // Get specimen data for each parameter
                    Dictionary<string, List<List<string>>> paramData = new Dictionary<string, List<List<string>>>();

                    foreach (var param in paramMap)
                    {
                        if (allResults.ContainsKey(param.Key) && !string.IsNullOrWhiteSpace(allResults[param.Key]))
                        {
                            string[] coilGroups = allResults[param.Key].Split(new[] { " ; " }, StringSplitOptions.None);
                            List<List<string>> coilData = new List<List<string>>();

                            foreach (string coilGroup in coilGroups)
                            {
                                if (param.Key == 12843) // Special handling for Diameter (multiple values per specimen)
                                {
                                    string[] specimens = coilGroup.Split('|');
                                    List<string> specimenData = new List<string>();

                                    foreach (string specimen in specimens)
                                    {
                                        specimenData.Add(specimen); // Contains comma-separated diameter values
                                    }
                                    coilData.Add(specimenData);
                                }
                                else
                                {
                                    string[] specimens = coilGroup.Split('|');
                                    coilData.Add(specimens.ToList());
                                }
                            }
                            paramData[param.Value] = coilData;
                        }
                    }

                    // Build rows - FIXED: Show individual specimen data, not grouped by coil
                    for (int coilIndex = 0; coilIndex < coilNos.Length; coilIndex++)
                    {
                        string currentCoil = coilNos[coilIndex].Trim();

                        // Get specimen count for this coil (use SpecimenNo as reference)
                        int specimenCount = paramData.ContainsKey("SpecimenNo") && paramData["SpecimenNo"].Count > coilIndex
                            ? paramData["SpecimenNo"][coilIndex].Count : 0;

                        for (int specimenIndex = 0; specimenIndex < specimenCount; specimenIndex++)
                        {
                            DataRow row = dt.NewRow();

                            // FIXED: Always show CoilNo and Date for each specimen row
                            row["CoilNo"] = currentCoil;

                            // Get the date for this specific specimen
                            if (paramData.ContainsKey("Date") && paramData["Date"].Count > coilIndex && paramData["Date"][coilIndex].Count > specimenIndex)
                            {
                                row["Date"] = paramData["Date"][coilIndex][specimenIndex].Trim();
                            }

                            // Set other parameters for each specimen
                            foreach (var param in paramData)
                            {
                                if (param.Key == "CoilNo" || param.Key == "Date") continue; // Already handled

                                if (param.Key == "Diameter")
                                {
                                    // Special handling for diameter - split comma-separated values
                                    if (param.Value.Count > coilIndex && param.Value[coilIndex].Count > specimenIndex)
                                    {
                                        string diameterValues = param.Value[coilIndex][specimenIndex];
                                        string[] diameters = diameterValues.Split(',');

                                        for (int i = 0; i < Math.Min(6, diameters.Length); i++)
                                        {
                                            row[$"Diameter{i + 1}"] = diameters[i].Trim();
                                        }
                                    }
                                }
                                else
                                {
                                    // Regular parameter handling
                                    if (param.Value.Count > coilIndex && param.Value[coilIndex].Count > specimenIndex)
                                    {
                                        row[param.Key] = param.Value[coilIndex][specimenIndex].Trim();
                                    }
                                }
                            }

                            dt.Rows.Add(row);
                        }
                    }
                }

                // Bind to GridView
                gvcopper.DataSource = dt;
                gvcopper.DataBind();

                // FIXED: Remove the rowspan styling since we want to show all data normally
                UpdateStandardFields(dt);
                ViewState["TestResults"] = dt;

                if (dt.Rows.Count > 0)
                {
                    //ScriptManager.RegisterStartupScript(this, GetType(), "DataLoaded",
                    //    $"alert('test result records.');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "NoData",
                        "alert('No saved test results found for this sample.');", true);
                }
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

                // FIXED: Group by both CoilNo AND Date to handle multiple coils with same number on different dates
                var grouped = dt.AsEnumerable()
                    .GroupBy(r => new {
                        CoilNo = r["CoilNo"].ToString(),
                        Date = r["Date"].ToString()
                    })
                    .OrderBy(g => g.Key.CoilNo)
                    .ThenBy(g => g.Key.Date);

                // Function to collect formatted result per column
                string Collect(string column, bool isMultiInput = false)
                {
                    List<string> perCoil = new List<string>();

                    foreach (var coilGroup in grouped)
                    {
                        List<string> perSpecimen = new List<string>();

                        foreach (var row in coilGroup)
                        {
                            if (isMultiInput)
                            {
                                string diameter = string.Join(",", new string[]
                                {
                            row["Diameter1"].ToString(), row["Diameter2"].ToString(),
                            row["Diameter3"].ToString(), row["Diameter4"].ToString(),
                            row["Diameter5"].ToString(), row["Diameter6"].ToString()
                                });
                                perSpecimen.Add(diameter);
                            }
                            else
                            {
                                perSpecimen.Add(row[column]?.ToString()?.Trim() ?? "");
                            }
                        }

                        perCoil.Add(string.Join("|", perSpecimen));
                    }

                    return string.Join(" ; ", perCoil);
                }

                // FIXED: For CoilNo - now includes all coil instances even if same coil number appears multiple times
                string coilNos = string.Join(";", grouped.Select(g => g.Key.CoilNo));
                reportParamMapList.Add("12841");
                resultList.Add(coilNos);

                reportParamMapList.Add("12842"); resultList.Add(Collect("SpecimenNo"));

                // FIXED: Handle Diameter with the new grouping
                string diameterResult = string.Join(" ; ", grouped.Select(coilGroup =>
                    string.Join("|", coilGroup.Select(row => string.Join(",", new string[] {
                row["Diameter1"].ToString(),
                row["Diameter2"].ToString(),
                row["Diameter3"].ToString(),
                row["Diameter4"].ToString(),
                row["Diameter5"].ToString(),
                row["Diameter6"].ToString()
                    })))
                ));

                reportParamMapList.Add("12843");
                resultList.Add(diameterResult);

                reportParamMapList.Add("12844"); resultList.Add(Collect("Nom"));
                reportParamMapList.Add("12845"); resultList.Add(Collect("Area"));
                reportParamMapList.Add("12889"); resultList.Add(Collect("Date"));
                reportParamMapList.Add("12846"); resultList.Add(Collect("BF"));
                reportParamMapList.Add("12847"); resultList.Add(Collect("TS"));
                reportParamMapList.Add("12848"); resultList.Add(Collect("Temp"));
                reportParamMapList.Add("12849"); resultList.Add(Collect("Density"));
                reportParamMapList.Add("12850"); resultList.Add(Collect("L0"));
                reportParamMapList.Add("12851"); resultList.Add(Collect("L1"));
                reportParamMapList.Add("12852"); resultList.Add(Collect("EL"));
                reportParamMapList.Add("12853"); resultList.Add(Collect("MTS"));
                reportParamMapList.Add("12854"); resultList.Add(Collect("MEL"));
                reportParamMapList.Add("12855"); resultList.Add(Collect("Resistance"));
                reportParamMapList.Add("12856"); resultList.Add(Collect("Kt"));
                reportParamMapList.Add("12857"); resultList.Add(Collect("Conductivity"));
                reportParamMapList.Add("12858"); resultList.Add(Collect("Resistivity"));

                // Add upper-section values
                reportParamMapList.Add("12877"); resultList.Add(cableType.Text);
                reportParamMapList.Add("12883"); resultList.Add(refSpec.Text);
                reportParamMapList.Add("12878"); resultList.Add(cableSize.Text);
                reportParamMapList.Add("12884"); resultList.Add(testMethodNo.Text);
                reportParamMapList.Add("12879"); resultList.Add(voltage.Text);
                reportParamMapList.Add("12885"); resultList.Add(jobOrderNo.Text);
                reportParamMapList.Add("12886"); resultList.Add(worksheetno.Text);
                reportParamMapList.Add("12881"); resultList.Add(roomno.Text);
                reportParamMapList.Add("12887"); resultList.Add(roomhumidity.Text);
                reportParamMapList.Add("12882"); resultList.Add(roomtemp.Text);
                reportParamMapList.Add("12880"); resultList.Add(Date.Text);
                reportParamMapList.Add("12888"); resultList.Add(equipmentno.Text);

                // Specified values
                reportParamMapList.Add("12890"); resultList.Add(txtSpecifiedDiameter.Text);
                reportParamMapList.Add("12891"); resultList.Add(txtSpecifiedDiameterNom.Text);
                reportParamMapList.Add("12892"); resultList.Add(txtSpecifiedArea.Text);
                reportParamMapList.Add("12893"); resultList.Add(txtSpecifiedBf.Text);
                reportParamMapList.Add("12894"); resultList.Add(txtSpecifiedTS.Text);
                reportParamMapList.Add("12895"); resultList.Add(txtSpecifiedL0.Text);
                reportParamMapList.Add("12896"); resultList.Add(txtSpecifiedL1.Text);
                reportParamMapList.Add("12897"); resultList.Add(txtSpecifiedEL.Text);
                reportParamMapList.Add("12898"); resultList.Add(txtSpecifiedMedianTS.Text);
                reportParamMapList.Add("12899"); resultList.Add(txtSpecifiedMedianEL.Text);
                reportParamMapList.Add("12900"); resultList.Add(txtSpecifiedResistance.Text);
                reportParamMapList.Add("12901"); resultList.Add(txtSpecifiedTemp.Text);
                reportParamMapList.Add("12902"); resultList.Add(txtSpecifiedKt.Text);
                reportParamMapList.Add("12903"); resultList.Add(txtSpecifiedResistivityat20C.Text);
                reportParamMapList.Add("12904"); resultList.Add(txtSpecifiedConductivity.Text);
                reportParamMapList.Add("12905"); resultList.Add(txtSpecifiedDensity.Text);

                // Save data
                string[] reportParamMap = reportParamMapList.ToArray();
                string[] result = resultList.ToArray();

                int modified = _cm.AddTestResultPTQualis(_cm, reportParamMap, result);
                if (modified > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "success", "alert('Test results saved and report generated successfully.');", true);

                    _cm.sampleStatusID = 27;
                    _cm.UpdateCRFSampleDetailsStatus(_cm);

                    _cm.StatusID = 27;
                    _cm.UpdateAllocationStatusid_mechanical(_cm);

                    Response.Redirect("GenerateTestRecord_CopperandAluminiumWire.aspx?s=" + _cm.sampleId + "&r=" + _cm.ReportID);
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
            try
            {
                string columnName = dt.Columns[columnIndex].ColumnName;

                // FIXED: Only apply merging logic to Date and CoilNo columns
                if (columnName != "Date" && columnName != "CoilNo")
                {
                    return; // Don't merge other columns
                }

                string currentValue = dt.Rows[currentRowIndex][columnName].ToString();
                string previousValue = dt.Rows[currentRowIndex - 1][columnName].ToString();

                // FIXED: For CoilNo, also check if Date is the same
                bool shouldMerge = false;

                if (columnName == "Date")
                {
                    // Merge Date if same date
                    shouldMerge = (currentValue == previousValue);
                }
                else if (columnName == "CoilNo")
                {
                    // Merge CoilNo only if both CoilNo AND Date are the same
                    string currentDate = dt.Rows[currentRowIndex]["Date"].ToString();
                    string previousDate = dt.Rows[currentRowIndex - 1]["Date"].ToString();
                    shouldMerge = (currentValue == previousValue && currentDate == previousDate);
                }

                if (shouldMerge)
                {
                    currentRow.Cells[columnIndex].Visible = false;
                    int spanRowIndex = currentRowIndex - 1;
                    while (spanRowIndex > 0 && !gvcopper.Rows[spanRowIndex].Cells[columnIndex].Visible)
                    {
                        spanRowIndex--;
                    }
                    if (gvcopper.Rows[spanRowIndex].Cells[columnIndex].RowSpan == 0)
                    {
                        gvcopper.Rows[spanRowIndex].Cells[columnIndex].RowSpan = 2;
                    }
                    else
                    {
                        gvcopper.Rows[spanRowIndex].Cells[columnIndex].RowSpan += 1;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in ProcessColumn: " + ex.Message);
            }
        }
        protected void gvcopper_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);

                // Check if the "TestResults" ViewState exists
                DataTable dt = (DataTable)ViewState["TestResults"];



                if (dt != null && dt.Rows.Count > 0 && rowIndex >= 0 && rowIndex < dt.Rows.Count)
                {
                    dt.Rows.RemoveAt(rowIndex);  // Remove the row at the specified index
                    ViewState["TestResults"] = dt;

                    gvcopper.DataSource = dt;
                    gvcopper.DataBind();


                    UpdateSpecimenNumber();
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

                // Clear the default header
                e.Row.Visible = false;

                // First header row
                GridViewRow firstHeaderRow = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal);

                // Row 1: Main headers with appropriate rowspan and colspan
                firstHeaderRow.Cells.Add(CreateHeaderCell("Date", 3, 1));
                firstHeaderRow.Cells.Add(CreateHeaderCell("Coil<br/>No.", 3, 1));
                firstHeaderRow.Cells.Add(CreateHeaderCell("Specimen<br/>No.", 3, 1));
                firstHeaderRow.Cells.Add(CreateHeaderCell("Diameter (mm)", 1, 7));
                firstHeaderRow.Cells.Add(CreateHeaderCell("Area<br/>mm²", 3, 1));
                firstHeaderRow.Cells.Add(CreateHeaderCell("BF<br/>N", 3, 1));
                firstHeaderRow.Cells.Add(CreateHeaderCell("TS<br/>N/mm²", 3, 1));
                firstHeaderRow.Cells.Add(CreateHeaderCell("Elongation", 1, 3));
                firstHeaderRow.Cells.Add(CreateHeaderCell("Median", 1, 2));
                firstHeaderRow.Cells.Add(CreateHeaderCell("Resistance<br/>Ω/km", 3, 1));
                firstHeaderRow.Cells.Add(CreateHeaderCell("Temp.<br/>°C", 3, 1));
                firstHeaderRow.Cells.Add(CreateHeaderCell("Kt<br/>%", 3, 1));
                firstHeaderRow.Cells.Add(CreateHeaderCell("Resistivity<br/>at 20°C<br/>Ω·mm²/m", 3, 1));
                firstHeaderRow.Cells.Add(CreateHeaderCell("Conductivity<br/>%", 3, 1));
                firstHeaderRow.Cells.Add(CreateHeaderCell("Density<br/>g/cm³", 3, 1));
                firstHeaderRow.Cells.Add(CreateHeaderCell("Actions", 3, 1));

                grid.Controls[0].Controls.AddAt(0, firstHeaderRow);

                // Second header row
                GridViewRow secondHeaderRow = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Normal);

                // Row 2: Sub-headers for Diameter, Elongation, and Median
                secondHeaderRow.Cells.Add(CreateHeaderCell("1", 1, 1));
                secondHeaderRow.Cells.Add(CreateHeaderCell("2", 1, 1));
                secondHeaderRow.Cells.Add(CreateHeaderCell("3", 1, 1));
                secondHeaderRow.Cells.Add(CreateHeaderCell("4", 1, 1));
                secondHeaderRow.Cells.Add(CreateHeaderCell("5", 1, 1));
                secondHeaderRow.Cells.Add(CreateHeaderCell("6", 1, 1));
                secondHeaderRow.Cells.Add(CreateHeaderCell("Nom", 1, 1));
                secondHeaderRow.Cells.Add(CreateHeaderCell("L0<br/>mm", 2, 1));
                secondHeaderRow.Cells.Add(CreateHeaderCell("L1<br/>mm", 2, 1));
                secondHeaderRow.Cells.Add(CreateHeaderCell("EL<br/>%", 2, 1));
                secondHeaderRow.Cells.Add(CreateHeaderCell("TS<br/>N/mm²", 2, 1));
                secondHeaderRow.Cells.Add(CreateHeaderCell("EL<br/>%", 2, 1));

                grid.Controls[0].Controls.AddAt(1, secondHeaderRow);

                // Third header row (units row)
                GridViewRow thirdHeaderRow = new GridViewRow(2, 0, DataControlRowType.Header, DataControlRowState.Normal);

                // Row 3: Units for diameter columns only (L0, L1, EL, TS, EL already have units in row 2)
                thirdHeaderRow.Cells.Add(CreateHeaderCell("mm", 1, 1)); // Diameter 1
                thirdHeaderRow.Cells.Add(CreateHeaderCell("mm", 1, 1)); // Diameter 2
                thirdHeaderRow.Cells.Add(CreateHeaderCell("mm", 1, 1)); // Diameter 3
                thirdHeaderRow.Cells.Add(CreateHeaderCell("mm", 1, 1)); // Diameter 4
                thirdHeaderRow.Cells.Add(CreateHeaderCell("mm", 1, 1)); // Diameter 5
                thirdHeaderRow.Cells.Add(CreateHeaderCell("mm", 1, 1)); // Diameter 6
                thirdHeaderRow.Cells.Add(CreateHeaderCell("mm", 1, 1)); // Nom

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
            //cell.BackColor = System.Drawing.Color.LightGray;
            cell.Font.Bold = true;
            //cell.BorderStyle = BorderStyle.Solid;
            cell.BorderWidth = 1;
            //cell.BorderColor = System.Drawing.Color.Black;
            return cell;
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

            txtSpecimenNo.Text = "";
            txtInput1.Text = "";
            txtInput2.Text = "";
            txtInput3.Text = "";
            txtInput4.Text = "";
            txtInput5.Text = "";
            txtInput6.Text = "";

            // Clear the new inputs added in your table
            txtDate1.Text = "";
            txtCoil1.Text = "";
            txtNom1.Text = "";
            txtArea.Text = "";
            txtBF.Text = "";
            txtTS.Text = "";
            txtL0.Text = "";
            txtL1.Text = "";
            txtEL.Text = "";
            txtMTS.Text = "";
            txtMEL.Text = "";
            txtRes.Text = "";
            txtTemp.Text = "";
            txtKt.Text = "";
            txtResist.Text = "";
            txtCond.Text = "";
            txtDens.Text = "";

            ViewState["TestResults"] = null; 
            // Clear GridView
            gvcopper.DataSource = null;
            gvcopper.DataBind();


        }


    }
}