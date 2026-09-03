using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
	public partial class DimensionTypeTestMediumHighVoltage_mv : System.Web.UI.Page
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
                PopulateCoreDropdown();

            }
        }
        private void InitializeGrid()
        {
            // Create a DataTable to store the test results
            DataTable dt = new DataTable();
            dt.Columns.Add("typeofsample", typeof(int));
            dt.Columns.Add("ItemType", typeof(string));
            dt.Columns.Add("ItemSubType", typeof(string));
            dt.Columns.Add("Unit", typeof(string));
            dt.Columns.Add("CorePhase", typeof(string));
            dt.Columns.Add("Specification", typeof(string));
            //   dt.Columns.Add("SpecimenNo", typeof(int));

            // Add the 6 additional input columns
            dt.Columns.Add("Inputs", typeof(string));

            dt.Columns.Add("InputMinMax", typeof(string));


            // Store the DataTable in ViewState for persistence across postbacks
            ViewState["TestResults"] = dt;

            // Bind the empty DataTable to the GridView
            gvDimension.DataSource = dt;
            gvDimension.DataBind();

        }
        private void PopulateCoreDropdown()
        {
            try
            {
                DataTable coreData = _cm.Get_Core();
                ddlCorePhase.Items.Clear();
                if (coreData != null && coreData.Rows.Count > 0)
                {
                    // Bind data to dropdown
                    foreach (DataRow row in coreData.Rows)
                    {
                        string coreName = row["CoreName"].ToString();
                        string coreId = row["CoreID"].ToString();
                        ddlCorePhase.Items.Add(new ListItem(coreName, coreId));
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error if needed
                ScriptManager.RegisterStartupScript(this, GetType(), "DropdownError",
                    $"alert('Error loading core data: {ex.Message}');", true);
            }
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

                       
                        // Prefill test results grid
                        PrefillDimensionGrid(sampleId);
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
        private void PrefillDimensionGrid(int sampleId)
        {
            try
            {
                InitializeGrid();
                DataTable dt = ViewState["TestResults"] as DataTable;
                if (dt == null) return;

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT ReportParamMapID, Result FROM TestResultsPTQualis WHERE SampleID = @SampleID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SampleID", sampleId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            string itemTypesRaw = "", phasesRaw = "", unitsRaw = "", inputsRaw = "", subTypesRaw = "", minMaxAvgRaw = "", specificationRaw = "";

                            while (reader.Read())
                            {
                                string id = reader["ReportParamMapID"].ToString();
                                string result = reader["Result"].ToString();

                                if (id == "17320") itemTypesRaw = result;
                                else if (id == "17333") phasesRaw = result;
                                else if (id == "17334") unitsRaw = result;
                                else if (id == "17337") inputsRaw = result;
                                else if (id == "17336") subTypesRaw = result;
                                else if (id == "17338") minMaxAvgRaw = result;
                                else if (id == "17339") specificationRaw = result;
                            }

                            // Extract all Inputs and MinMaxAvg values across all samples (flattened) - keep existing logic
                            var allInputMatches = Regex.Matches(inputsRaw, @"\(([^)]*)\)")
                                .Cast<Match>()
                                .Select(m => m.Groups[1].Value)
                                .ToArray();

                            var allMinMaxAvgMatches = Regex.Matches(minMaxAvgRaw, @"\(([^)]*)\)")
                                .Cast<Match>()
                                .Select(m => m.Groups[1].Value)
                                .ToArray();

                            // Extract all specification values - keep existing logic
                            var allSpecificationValues = new List<string>();
                            var specificationSamples = specificationRaw.Split('|');
                            for (int sampleIndex = 0; sampleIndex < specificationSamples.Length; sampleIndex++)
                            {
                                var specGroups = Safe(specificationSamples, sampleIndex).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (var group in specGroups)
                                {
                                    var specs = group.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    allSpecificationValues.AddRange(specs.Select(s => s.Trim()));
                                }
                            }

                            // Split samples by '|'
                            var itemSamples = itemTypesRaw.Split('|');
                            var subTypeSamples = subTypesRaw.Split('|');
                            var phaseSamples = phasesRaw.Split('|');
                            var unitSamples = unitsRaw.Split('|');

                            int inputIndex = 0;

                            // Process each sample
                            for (int sampleIndex = 0; sampleIndex < itemSamples.Length; sampleIndex++)
                            {
                                // Split groups within sample by ';'
                                var itemGroups = Safe(itemSamples, sampleIndex).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                                var subTypeGroups = Safe(subTypeSamples, sampleIndex).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                                var phaseGroups = Safe(phaseSamples, sampleIndex).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                                var unitGroups = Safe(unitSamples, sampleIndex).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);


                                // Process each group within the sample
                                for (int groupIndex = 0; groupIndex < itemGroups.Length; groupIndex++)
                                {
                                    string itemType = itemGroups[groupIndex].Trim();

                                    // Parse subtypes (comma-separated)
                                    var subtypes = Safe(subTypeGroups, groupIndex).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                    // Extract phase groups - each parentheses is a separate group for each subtype
                                    var rawPhaseString = Safe(phaseGroups, groupIndex);
                                    var phaseGroups_extracted = Regex.Matches(rawPhaseString, @"\(([^)]*)\)")
                                        .Cast<Match>()
                                        .Select(m => m.Groups[1].Value.Split(',').Select(s => s.Trim()).ToArray())
                                        .ToArray();


                                    var rawUnitString = Safe(unitGroups, groupIndex);
                                    var unitGroups_extracted = Regex.Matches(rawUnitString, @"\(([^)]*)\)")
                                        .Cast<Match>()
                                        .Select(m => m.Groups[1].Value.Split(',').Select(s => s.Trim()).ToArray())
                                        .ToArray();


                                    // Create rows for each subtype paired with its corresponding phase/unit group
                                    for (int subtypeIndex = 0; subtypeIndex < subtypes.Length; subtypeIndex++)
                                    {
                                        var subtype = subtypes[subtypeIndex].Trim();

                                        // Each subtype gets its corresponding phase and unit group
                                        if (subtypeIndex < phaseGroups_extracted.Length && subtypeIndex < unitGroups_extracted.Length)
                                        {
                                            var phasesForThisSubtype = phaseGroups_extracted[subtypeIndex];
                                            var unitsForThisSubtype = unitGroups_extracted[subtypeIndex];


                                            if (unitsForThisSubtype.Length < phasesForThisSubtype.Length && unitsForThisSubtype.Length == 1)
                                            {
                                                unitsForThisSubtype = Enumerable.Repeat(unitsForThisSubtype[0], phasesForThisSubtype.Length).ToArray();
                                            }
                                            // Create a row for each phase/unit pair in this group
                                            for (int p = 0; p < Math.Max(phasesForThisSubtype.Length, unitsForThisSubtype.Length); p++)
                                            {
                                                var row = dt.NewRow();
                                                row["typeofsample"] = sampleIndex + 1;
                                                row["ItemType"] = itemType;
                                                row["ItemSubType"] = subtype;
                                                row["CorePhase"] = p < phasesForThisSubtype.Length ? phasesForThisSubtype[p] : "";
                                                row["Unit"] = p < unitsForThisSubtype.Length ? unitsForThisSubtype[p] : "";

                                                // Keep existing logic for inputs, MinMaxAvg, and specifications
                                                row["Inputs"] = inputIndex < allInputMatches.Length ? allInputMatches[inputIndex] : "";
                                                row["InputMinMax"] = inputIndex < allMinMaxAvgMatches.Length ? allMinMaxAvgMatches[inputIndex] : "";
                                                row["Specification"] = inputIndex < allSpecificationValues.Count ? allSpecificationValues[inputIndex] : "";

                                                dt.Rows.Add(row);
                                                inputIndex++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                gvDimension.DataSource = dt;
                gvDimension.DataBind();
            }
            catch
            {
                throw;
            }
        }
        private string Safe(string[] arr, int index) => (index >= 0 && index < arr.Length) ? arr[index] : "";
        private void PrefillUpperSection(DataTable existingData)
        {
            SetTextBoxText(cableType, "17332");        // Type
            SetTextBoxText(refSpec, "17326");          // RefSpec
            SetTextBoxText(cableSize, "17321");        // Size
            SetTextBoxText(testMethodNo, "17327");     // TestMethodNo
            SetTextBoxText(voltage, "17322");          // Voltage
            SetTextBoxText(jobOrderNo, "17328");       // JobOrderNo
            SetTextBoxText(worksheetno, "17329");      // WorkSheetNo
            SetTextBoxText(equipmentno, "17331");      // EquipmentNo

            SetTextBoxText(roomno, "17324");           // RoomNo
            SetTextBoxText(roomhumidity, "17330");     // RoomHumidity
            SetTextBoxText(roomtemp, "17325");         // RoomTemp
            SetTextBoxText(Date, "17323");

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
        protected void txtInput_TextChanged(object sender, EventArgs e)
        {
            List<double> values = new List<double>();

            // Split comma-separated input values
            string[] inputs = txtInputs.Text.Split(',');

            foreach (string input in inputs)
            {
                string trimmed = input.Trim();

                if (string.IsNullOrWhiteSpace(trimmed) || trimmed == "-")
                    continue; // Skip empty or '-' entirely
                else if (double.TryParse(trimmed, out double number))
                    values.Add(number);
            }

            if (values.Count > 0)
            {
                double min = values.Min();
                double max = values.Max();
                double avg = values.Average();

                txtMinMax.Text = $"Min: {min:F2}   Max: {max:F2}   Avg: {avg:F2}";
            }
            else
            {
                txtMinMax.Text = "";
            }
        }
        protected void btnAddToGrid_Click(object sender, EventArgs e)
        {
            ResetControlBorders();

            bool hasError = false;
            List<Control> invalidControls = new List<Control>();

            // Validate ItemType
            if (string.IsNullOrWhiteSpace(txtItemType.Text))
            {
                invalidControls.Add(txtItemType);
                hasError = true;
            }

            // Validate ItemSubType
            if (string.IsNullOrWhiteSpace(txtsubitem.Text))
            {
                invalidControls.Add(txtsubitem);
                hasError = true;
            }
            string rawInputs = txtInputs.Text.Trim();

            // Remove trailing commas
            rawInputs = rawInputs.TrimEnd(',');

            // Split and check if any empty entries (like consecutive commas or trailing commas)
            string[] inputParts = rawInputs.Split(',');

            bool inputsValid = inputParts.All(part => !string.IsNullOrWhiteSpace(part));

            if (!inputsValid)
            {
                invalidControls.Add(txtInputs);
                hasError = true;
            }
            else if (string.IsNullOrWhiteSpace(rawInputs))
            {
                invalidControls.Add(txtInputs);
                hasError = true;
            }

            // Validate Min/Max/Avg (from textbox which is auto-filled)
            string minMaxText = txtMinMax.Text.Trim();
            if (string.IsNullOrWhiteSpace(minMaxText) ||
                !Regex.IsMatch(minMaxText, @"Min:\s*(\d+(\.\d+)?)\s*Max:\s*(\d+(\.\d+)?)\s*Avg:\s*(\d+(\.\d+)?)"))
            {
                invalidControls.Add(txtMinMax);
                hasError = true;
            }

            // Highlight errors
            foreach (var control in invalidControls)
            {
                if (control is TextBox txt)
                {
                    txt.BorderColor = System.Drawing.Color.FromArgb(255, 150, 150);
                    txt.BorderStyle = BorderStyle.Solid;
                    txt.BorderWidth = 1;
                }
            }

            if (hasError)
            {
                ShowMessage("Please fill in all required fields correctly.");
                return;
            }

            // Initialize DataTable if not exists
            DataTable dt = ViewState["TestResults"] as DataTable;
            if (dt == null)
            {
                dt = new DataTable();
                dt.Columns.Add("typeofsample", typeof(int));
                dt.Columns.Add("ItemType", typeof(string));
                dt.Columns.Add("ItemSubType", typeof(string));
                dt.Columns.Add("CorePhase", typeof(string));
                dt.Columns.Add("Unit", typeof(string));
                dt.Columns.Add("Inputs", typeof(string));
                dt.Columns.Add("InputMinMax", typeof(string));
                dt.Columns.Add("Specification", typeof(string));
                ViewState["TestResults"] = dt;
            }

            // Add new row
            DataRow dr = dt.NewRow();
            dr["typeofsample"] = txttypeofsample.Text.Trim();
            dr["ItemType"] = txtItemType.Text.Trim();
            dr["ItemSubType"] = txtsubitem.Text.Trim();
            dr["CorePhase"] = ddlCorePhase.SelectedItem?.Text ?? "";
            dr["Unit"] = ddlUnit.SelectedItem?.Text ?? "";
            dr["Inputs"] = rawInputs;
            dr["InputMinMax"] = txtMinMax.Text.Trim(); // Already calculated
            dr["Specification"] = txtSpecifications.Text.Trim();

            dt.Rows.Add(dr);
            ViewState["TestResults"] = dt;

            // Bind to grid
            gvDimension.DataSource = dt;
            gvDimension.DataBind();


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
        private void ResetControlBorders()
        {
            // Reset all TextBox controls
            List<TextBox> allTextBoxes = new List<TextBox>();

            // Add all TextBox controls from the form
            allTextBoxes.Add(roomno);
            allTextBoxes.Add(roomhumidity);
            allTextBoxes.Add(roomtemp);
            allTextBoxes.Add(txtMinMax);

            // Add validated input fields as well
            allTextBoxes.Add(txtItemType);
            allTextBoxes.Add(txtsubitem);
            allTextBoxes.Add(txtInputs);

            // Reset border for each TextBox
            foreach (var textBox in allTextBoxes)
            {
                textBox.BorderColor = System.Drawing.Color.Empty; // Default color
                textBox.BorderStyle = BorderStyle.NotSet;         // Default style
                textBox.BorderWidth = 0;                          // Default width
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            List<string> errors = new List<string>();
            List<Control> invalidControls = new List<Control>();

            bool gridHasData = (gvDimension.Rows.Count > 0);


            if (string.IsNullOrWhiteSpace(Date.Text))
            {
                errors.Add("Date is required.");
                invalidControls.Add(Date);
            }

            if (string.IsNullOrWhiteSpace(roomno.Text))
            {
                errors.Add("Room No. is required.");
                invalidControls.Add(roomno);
            }

            if (string.IsNullOrWhiteSpace(roomhumidity.Text) || (!double.TryParse(roomhumidity.Text, out _) && roomhumidity.Text != "-"))
            {
                errors.Add("Room Humidity must be a valid number or '-'.");
                invalidControls.Add(roomhumidity);
            }

            if (string.IsNullOrWhiteSpace(roomtemp.Text) || (!double.TryParse(roomtemp.Text, out _) && roomtemp.Text != "-"))
            {
                errors.Add("Room Temp. must be a valid number or '-'.");
                invalidControls.Add(roomtemp);
            }


            if (!gridHasData)
            {
                if (string.IsNullOrWhiteSpace(txtSpecifications.Text) || (txtSpecifications.Text != "-" && !double.TryParse(txtSpecifications.Text, out _)))
                {
                    errors.Add("Specifications must be a valid number or '-'.");
                    invalidControls.Add(txtSpecifications);
                }

                // Grid should not be empty
                errors.Add("Add at least one test result to the grid.");
            }


            ResetControlBorders();

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

                string errorMessage = "Validation Errors:\\n• " + string.Join("\\n• ", errors.Select(err => err.Replace("'", "\\'").Replace("\n", "").Replace("\r", "")));

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
        private bool Submitform()
        {
            try
            {
                DataTable dt = ViewState["TestResults"] as DataTable;

                if (dt == null || dt.Rows.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showError", "alert('No data to save. Please add test results first.');", true);
                    return false;
                }
                var sampleGroups = dt.AsEnumerable().GroupBy(r => r.Field<int>("typeofsample")).OrderBy(g => g.Key).ToList();

                if (sampleGroups.Count > 1)
                {
                    var firstSample = sampleGroups[0];
                    var firstSampleStructure = firstSample
                        .GroupBy(r => r.Field<string>("ItemType")?.Trim())
                        .OrderBy(g => g.Key)
                        .Select(itemGroup => new
                        {
                            ItemType = itemGroup.Key,
                            SubTypes = itemGroup.GroupBy(r => r.Field<string>("ItemSubType")?.Trim())
                                .OrderBy(g => g.Key)
                                .Select(subGroup => new
                                {
                                    SubType = subGroup.Key,
                                    CorePhases = subGroup.Select(r => r.Field<string>("CorePhase")?.Trim())
                                        .Where(core => !string.IsNullOrEmpty(core) && core != "-")
                                        .Distinct()
                                        .OrderBy(c => c)
                                        .ToList(),
                                    Units = subGroup.Select(r => r.Field<string>("Unit")?.Trim())
                                        .Where(unit => !string.IsNullOrEmpty(unit) && unit != "-")
                                        .Distinct()
                                        .OrderBy(u => u)
                                        .ToList(),
                                    CoreCount = subGroup.Count(r => !string.IsNullOrEmpty(r.Field<string>("CorePhase")?.Trim()) && r.Field<string>("CorePhase").Trim() != "-"),
                                    UnitCount = subGroup.Count(r => !string.IsNullOrEmpty(r.Field<string>("Unit")?.Trim()) && r.Field<string>("Unit").Trim() != "-")
                                }).ToList()
                        }).ToList();

                    // Validate each subsequent sample against the first sample
                    for (int i = 1; i < sampleGroups.Count; i++)
                    {
                        var currentSample = sampleGroups[i];
                        var currentSampleStructure = currentSample
                            .GroupBy(r => r.Field<string>("ItemType")?.Trim())
                            .OrderBy(g => g.Key)
                            .Select(itemGroup => new
                            {
                                ItemType = itemGroup.Key,
                                SubTypes = itemGroup.GroupBy(r => r.Field<string>("ItemSubType")?.Trim())
                                    .OrderBy(g => g.Key)
                                    .Select(subGroup => new
                                    {
                                        SubType = subGroup.Key,
                                        CorePhases = subGroup.Select(r => r.Field<string>("CorePhase")?.Trim())
                                            .Where(core => !string.IsNullOrEmpty(core) && core != "-")
                                            .Distinct()
                                            .OrderBy(c => c)
                                            .ToList(),
                                        Units = subGroup.Select(r => r.Field<string>("Unit")?.Trim())
                                            .Where(unit => !string.IsNullOrEmpty(unit) && unit != "-")
                                            .Distinct()
                                            .OrderBy(u => u)
                                            .ToList(),
                                        CoreCount = subGroup.Count(r => !string.IsNullOrEmpty(r.Field<string>("CorePhase")?.Trim()) && r.Field<string>("CorePhase").Trim() != "-"),
                                        UnitCount = subGroup.Count(r => !string.IsNullOrEmpty(r.Field<string>("Unit")?.Trim()) && r.Field<string>("Unit").Trim() != "-")
                                    }).ToList()
                            }).ToList();

                        // Check if structures match
                        if (firstSampleStructure.Count != currentSampleStructure.Count)
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "showError",
                                $"alert('Sample {i + 1} has {currentSampleStructure.Count} item type(s) but Sample 1 has {firstSampleStructure.Count} item type(s). All samples must have the same structure.\\n\\nSample 1 item types: {string.Join(", ", firstSampleStructure.Select(x => x.ItemType))}\\nSample {i + 1} item types: {string.Join(", ", currentSampleStructure.Select(x => x.ItemType))}');", true);
                            return false;
                        }

                        // Check each item type
                        for (int j = 0; j < firstSampleStructure.Count; j++)
                        {
                            var firstItem = firstSampleStructure[j];
                            var currentItem = currentSampleStructure.FirstOrDefault(x => x.ItemType == firstItem.ItemType);

                            if (currentItem == null)
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "showError",
                                    $"alert('Sample {i + 1} is missing item type \\\"{firstItem.ItemType}\\\" that exists in Sample 1. All samples must have the same structure.\\n\\nExpected item types: {string.Join(", ", firstSampleStructure.Select(x => x.ItemType))}');", true);
                                return false;
                            }

                            // Check number of subtypes
                            if (firstItem.SubTypes.Count != currentItem.SubTypes.Count)
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "showError",
                                    $"alert('Sample {i + 1} has {currentItem.SubTypes.Count} subtype(s) for \\\"{firstItem.ItemType}\\\" but Sample 1 has {firstItem.SubTypes.Count} subtype(s). All samples must have the same structure.\\n\\nSample 1 subtypes: {string.Join(", ", firstItem.SubTypes.Select(x => x.SubType))}\\nSample {i + 1} subtypes: {string.Join(", ", currentItem.SubTypes.Select(x => x.SubType))}');", true);
                                return false;
                            }

                            // Check each subtype
                            foreach (var firstSub in firstItem.SubTypes)
                            {
                                var currentSub = currentItem.SubTypes.FirstOrDefault(x => x.SubType == firstSub.SubType);
                                if (currentSub == null)
                                {
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showError",
                                        $"alert('Sample {i + 1} is missing subtype \\\"{firstSub.SubType}\\\" under \\\"{firstItem.ItemType}\\\" that exists in Sample 1. All samples must have the same structure.\\n\\nExpected subtypes for {firstItem.ItemType}: {string.Join(", ", firstItem.SubTypes.Select(x => x.SubType))}');", true);
                                    return false;
                                }

                                // Check core count
                                if (firstSub.CoreCount != currentSub.CoreCount)
                                {
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showError",
                                        $"alert('Sample {i + 1} has {currentSub.CoreCount} core(s) for \\\"{firstSub.SubType}\\\" under \\\"{firstItem.ItemType}\\\" but Sample 1 has {firstSub.CoreCount} core(s). All samples must have the same structure.\\n\\nSample 1 cores: {string.Join(", ", firstSub.CorePhases)}\\nSample {i + 1} cores: {string.Join(", ", currentSub.CorePhases)}');", true);
                                    return false;
                                }

                                // Check unit count
                                if (firstSub.UnitCount != currentSub.UnitCount)
                                {
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showError",
                                        $"alert('Sample {i + 1} has {currentSub.UnitCount} unit(s) for \\\"{firstSub.SubType}\\\" under \\\"{firstItem.ItemType}\\\" but Sample 1 has {firstSub.UnitCount} unit(s). All samples must have the same structure.\\n\\nSample 1 units: {string.Join(", ", firstSub.Units)}\\nSample {i + 1} units: {string.Join(", ", currentSub.Units)}');", true);
                                    return false;
                                }

                                // Check if actual core values match (not just count)
                                if (!firstSub.CorePhases.SequenceEqual(currentSub.CorePhases))
                                {
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showError",
                                        $"alert('Sample {i + 1} has different core values for \\\"{firstSub.SubType}\\\" under \\\"{firstItem.ItemType}\\\". All samples must have the same core values.\\n\\nSample 1 cores: {string.Join(", ", firstSub.CorePhases)}\\nSample {i + 1} cores: {string.Join(", ", currentSub.CorePhases)}');", true);
                                    return false;
                                }

                                // Check if actual unit values match (not just count)
                                if (!firstSub.Units.SequenceEqual(currentSub.Units))
                                {
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showError",
                                        $"alert('Sample {i + 1} has different unit values for \\\"{firstSub.SubType}\\\" under \\\"{firstItem.ItemType}\\\". All samples must have the same unit values.\\n\\nSample 1 units: {string.Join(", ", firstSub.Units)}\\nSample {i + 1} units: {string.Join(", ", currentSub.Units)}');", true);
                                    return false;
                                }
                            }
                        }
                    }

                    // If we reach here, all samples have the same structure
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showInfo",
                        $"console.log('Structure validation passed: All {sampleGroups.Count} samples have matching structure.');", true);
                }

                if (_cm == null) _cm = new DAL.CM();
                if (Request.QueryString["s"] != null)
                    _cm.sampleId = Convert.ToInt32(Request.QueryString["s"].ToLower());
                if (Request.QueryString["r"] != null)
                    _cm.ReportID = Request.QueryString["r"];

                DataTable dt1 = _cm.Getdataforseriesandreportid_mechanical(_cm);
                if (dt1.Rows.Count > 0)
                {
                    _cm.sampleIDSeries = dt1.Rows[0]["sampleIDSeries"].ToString();
                    _cm.CRFSampleId = Convert.ToInt32(dt1.Rows[0]["CRFSampleId"]);
                }

                // Group data by typeofsample (sample number)
                var samples = dt.AsEnumerable()
                    .GroupBy(r => r.Field<int>("typeofsample"))
                    .OrderBy(g => g.Key)
                    .ToList();

                // Lists to store strings for each sample group (for other fields)
                List<string> sampleStrs = new List<string>();
                List<string> itemTypeStrs = new List<string>();
                List<string> itemSubTypeStrs = new List<string>();

                // For Inputs, MinMaxAvg, Unit, Core and Specification we'll use new simplified serialization:
                List<string> inputsSampleGroups = new List<string>();
                List<string> minMaxAvgSampleGroups = new List<string>();
                List<string> unitSampleGroups = new List<string>();
                List<string> coreSampleGroups = new List<string>();
                List<string> specificationSampleGroups = new List<string>();

                foreach (var sampleGroup in samples)
                {
                    int sampleNumber = sampleGroup.Key;
                    sampleStrs.Add(sampleNumber.ToString());

                    // Group by ItemType inside sample for other fields
                    var itemTypeGroups = sampleGroup
                        .GroupBy(r => r.Field<string>("ItemType")?.Trim())
                        .ToList();

                    List<string> itemTypeBlocks = new List<string>();
                    List<string> itemSubTypeBlocks = new List<string>();
                    List<string> coreBlocks = new List<string>();
                    List<string> unitBlocks = new List<string>();

                    foreach (var itemTypeGroup in itemTypeGroups)
                    {
                        string itemType = itemTypeGroup.Key;

                        // Group by ItemSubType inside ItemType
                        var subItemGroups = itemTypeGroup
                            .GroupBy(r => r.Field<string>("ItemSubType")?.Trim())
                            .ToList();

                        List<string> subItemTypeParts = new List<string>();
                        List<string> coreParts = new List<string>();
                        List<string> unitParts = new List<string>();

                        foreach (var subItemGroup in subItemGroups)
                        {
                            string subItem = subItemGroup.Key;
                            subItemTypeParts.Add(subItem);

                            // Core values for this ItemSubType
                            var coreValues = subItemGroup
                                .Select(r => r.Field<string>("CorePhase")?.Trim())
                                .Where(core => !string.IsNullOrEmpty(core))
                                .Distinct()
                                .ToList();

                            coreParts.Add($"({string.Join(",", coreValues)})");

                            // Unit values for this ItemSubType
                            var unitValues = subItemGroup
                                .Select(r => r.Field<string>("Unit")?.Trim())
                                .Where(unit => !string.IsNullOrEmpty(unit))
                                .Distinct()
                                .ToList();

                            unitParts.Add($"({string.Join(",", unitValues)})");
                        }

                        itemTypeBlocks.Add(itemType);
                        itemSubTypeBlocks.Add(string.Join(",", subItemTypeParts));
                        coreBlocks.Add(string.Join(",", coreParts));
                        unitBlocks.Add(string.Join(",", unitParts));
                    }

                    itemTypeStrs.Add(string.Join(";", itemTypeBlocks));
                    itemSubTypeStrs.Add(string.Join(";", itemSubTypeBlocks));
                    coreSampleGroups.Add(string.Join(";", coreBlocks));
                    unitSampleGroups.Add(string.Join(";", unitBlocks));

                    // Group rows by ItemSubType inside each sample for Inputs, MinMaxAvg, Specification
                    var subtypeGroups = sampleGroup
                        .GroupBy(r => r.Field<string>("ItemSubType")?.Trim())
                        .ToList();

                    List<string> inputsSubtypes = new List<string>();
                    List<string> minMaxAvgSubtypes = new List<string>();
                    List<string> specificationSubtypes = new List<string>();

                    foreach (var subtypeGroup in subtypeGroups)
                    {
                        // Inputs for this subtype group
                        var inputsValues = subtypeGroup
                            .Select(r => r.Field<string>("Inputs")?.Trim())
                            .Where(val => !string.IsNullOrEmpty(val))
                            .Select(val => $"({val})")
                            .ToList();
                        inputsSubtypes.Add(string.Join(",", inputsValues));

                        // MinMaxAvg for this subtype group
                        var minMaxAvgValues = subtypeGroup
                            .Select(r => r.Field<string>("InputMinMax")?.Trim())
                            .Where(val => !string.IsNullOrEmpty(val))
                            .Select(val => $"({val})")
                            .ToList();
                        minMaxAvgSubtypes.Add(string.Join(",", minMaxAvgValues));

                        // Specification for this subtype group
                        var specificationValues = subtypeGroup
                            .Select(r => r.Field<string>("Specification")?.Trim())
                            .Where(val => !string.IsNullOrEmpty(val))
                            .ToList();
                        specificationSubtypes.Add(string.Join(",", specificationValues));
                    }

                    inputsSampleGroups.Add(string.Join(";", inputsSubtypes));
                    minMaxAvgSampleGroups.Add(string.Join(";", minMaxAvgSubtypes));
                    specificationSampleGroups.Add(string.Join(";", specificationSubtypes));
                }

                // Join all samples by pipe '|'
                string finalSampleString = string.Join("|", sampleStrs);
                string finalItemTypeString = string.Join("|", itemTypeStrs);
                string finalItemSubTypeString = string.Join("|", itemSubTypeStrs);
                string finalCoreString = string.Join("|", coreSampleGroups);
                string finalUnitString = string.Join("|", unitSampleGroups);
                string finalInputsString = string.Join("|", inputsSampleGroups);
                string finalMinMaxAvgString = string.Join("|", minMaxAvgSampleGroups);
                string finalSpecificationString = string.Join("|", specificationSampleGroups);

                // Prepare parameters for saving
                List<string> reportParamMapList = new List<string>();
                List<string> resultList = new List<string>();

                reportParamMapList.Add("17335"); resultList.Add(finalSampleString);
                reportParamMapList.Add("17320"); resultList.Add(finalItemTypeString);
                reportParamMapList.Add("17336"); resultList.Add(finalItemSubTypeString);
                reportParamMapList.Add("17333"); resultList.Add(finalCoreString);
                reportParamMapList.Add("17334"); resultList.Add(finalUnitString);
                reportParamMapList.Add("17337"); resultList.Add(finalInputsString);
                reportParamMapList.Add("17338"); resultList.Add(finalMinMaxAvgString);
                reportParamMapList.Add("17339"); resultList.Add(finalSpecificationString);

                // Add upper section values
            
                reportParamMapList.Add("17332"); resultList.Add(cableType.Text);       // Type
                reportParamMapList.Add("17326"); resultList.Add(refSpec.Text);         // RefSpec
                reportParamMapList.Add("17321"); resultList.Add(cableSize.Text);       // Size
                reportParamMapList.Add("17327"); resultList.Add(testMethodNo.Text);    // TestMethodNo
                reportParamMapList.Add("17322"); resultList.Add(voltage.Text);         // Voltage
                reportParamMapList.Add("17328"); resultList.Add(jobOrderNo.Text);      // JobOrderNo
                reportParamMapList.Add("17329"); resultList.Add(worksheetno.Text);     // WorkSheetNo
                reportParamMapList.Add("17324"); resultList.Add(roomno.Text);          // RoomNo
                reportParamMapList.Add("17330"); resultList.Add(roomhumidity.Text);    // RoomHumidity
                reportParamMapList.Add("17325"); resultList.Add(roomtemp.Text);        // RoomTemp
                reportParamMapList.Add("17323"); resultList.Add(Date.Text);            // Date
                reportParamMapList.Add("17331"); resultList.Add(equipmentno.Text);     // EquipmentNo


                string[] reportParamMap = reportParamMapList.ToArray();
                string[] result = resultList.ToArray();

                int modified = _cm.AddTestResultPTQualis(_cm, reportParamMap, result);

                if (modified > 0)
                {
                    _cm.sampleStatusID = 27;
                    _cm.UpdateCRFSampleDetailsStatus(_cm);

                    _cm.StatusID = 27;
                    _cm.UpdateAllocationStatusid_mechanical(_cm);

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showSuccess",
                        "alert('Data saved successfully!');", true);

                    Response.Redirect($"GenerateTestRecord_DimensionTypeatHightVoltage_mv.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
                    return true;
                }
                else
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
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            List<string> errors = new List<string>();
            List<Control> invalidControls = new List<Control>();

            bool gridHasData = (gvDimension.Rows.Count > 0);

            // Validate always-required fields
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

            // Only validate test input fields if grid is empty
            if (!gridHasData)
            {
                bool hasError = false;
                int specimenNo = 0;

                // Test Specification
                if (string.IsNullOrEmpty(txtSpecifications.Text) || (txtSpecifications.Text != "-" && !double.TryParse(txtSpecifications.Text, out _)))
                {
                    errors.Add("Test Specification must be a valid number or '-'.");
                    invalidControls.Add(txtSpecifications);
                }

                // Core Phase (not required, defaults to "-")
                string corePhase = "-";
                if (ddlCorePhase.SelectedIndex > 0 && !string.IsNullOrWhiteSpace(ddlCorePhase.SelectedValue))
                {
                    corePhase = ddlCorePhase.SelectedValue;
                }
                if (string.IsNullOrWhiteSpace(txtItemType.Text) || !int.TryParse(txtItemType.Text.Trim(), out specimenNo))
                {
                    errors.Add("Item Type No must be valid");
                    invalidControls.Add(txtItemType);
                }
                if (string.IsNullOrWhiteSpace(txtsubitem.Text) || !int.TryParse(txtsubitem.Text.Trim(), out specimenNo))
                {
                    errors.Add("Sub Item must be valid.");
                    invalidControls.Add(txtsubitem);
                }
                // SpecimenNo
                //if (string.IsNullOrWhiteSpace(txtSpecimenNo.Text) || !int.TryParse(txtSpecimenNo.Text.Trim(), out specimenNo))
                //{
                //    errors.Add("Specimen No must be a valid number.");
                //    invalidControls.Add(txtSpecimenNo);
                //}


                string[] inputs = txtInputs.Text.Split(',');
                List<decimal> parsedValues = new List<decimal>();

                foreach (string input in inputs)
                {
                    string trimmed = input.Trim();

                    if (string.IsNullOrEmpty(trimmed))
                        continue;

                    if (trimmed == "-")
                    {
                        parsedValues.Add(0);
                    }
                    else if (decimal.TryParse(trimmed, out decimal val))
                    {
                        parsedValues.Add(val);
                    }
                    else
                    {
                        errors.Add("Inputs must be numbers or '-' separated by commas.");
                        invalidControls.Add(txtInputs);
                        hasError = true;
                        break;
                    }
                }
            }

            // If grid is still empty, require at least one test result
            if (!gridHasData)
            {
                errors.Add("Add at least one test result to the grid.");
            }

            // Reset borders
            ResetControlBorders();

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

                string errorMessage = "Validation Errors:\\n• " + string.Join("\\n• ", errors.Select(err => err.Replace("'", "\\'")));

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
        protected void gvDimension_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            GridView grid = (GridView)sender;
            int rowIndex = e.Row.RowIndex;

            if (rowIndex == 0)
                return;

            GridViewRow currentRow = e.Row;

            // Get current cell values, use correct indices
            string currentTypeOfSample = currentRow.Cells[0].Text.Trim().ToLower();
            string currentItemType = currentRow.Cells[1].Text.Trim().ToLower();
            string currentItemSubType = currentRow.Cells[2].Text.Trim().ToLower();
            string currentCore = currentRow.Cells[3].Text.Trim().ToLower();

            // Helper function to find the previous visible row's cell text in a given column index
            string GetPreviousVisibleCellText(int startIndex, int colIndex)
            {
                for (int i = startIndex - 1; i >= 0; i--)
                {
                    if (grid.Rows[i].Cells[colIndex].Visible)
                        return grid.Rows[i].Cells[colIndex].Text.Trim().ToLower();
                }
                return null;
            }

            // Merge TypeOfSample (column 0)
            {
                GridViewRow targetRow = null;
                for (int i = rowIndex - 1; i >= 0; i--)
                {
                    if (grid.Rows[i].Cells[0].Visible)
                    {
                        targetRow = grid.Rows[i];
                        break;
                    }
                }

                if (targetRow != null && currentTypeOfSample == targetRow.Cells[0].Text.Trim().ToLower() && !string.IsNullOrEmpty(currentTypeOfSample))
                {
                    if (targetRow.Cells[0].RowSpan == 0)
                        targetRow.Cells[0].RowSpan = 1;

                    targetRow.Cells[0].RowSpan += 1;
                    currentRow.Cells[0].Visible = false;
                }
            }

            // Merge ItemType (column 1) only if TypeOfSample also matches
            {
                GridViewRow targetRow = null;
                for (int i = rowIndex - 1; i >= 0; i--)
                {
                    if (grid.Rows[i].Cells[1].Visible)
                    {
                        targetRow = grid.Rows[i];
                        break;
                    }
                }

                if (targetRow != null &&
                    currentItemType == targetRow.Cells[1].Text.Trim().ToLower() &&
                    currentTypeOfSample == targetRow.Cells[0].Text.Trim().ToLower() &&
                    !string.IsNullOrEmpty(currentItemType))
                {
                    if (targetRow.Cells[1].RowSpan == 0)
                        targetRow.Cells[1].RowSpan = 1;

                    targetRow.Cells[1].RowSpan += 1;
                    currentRow.Cells[1].Visible = false;
                }
            }

            // Merge ItemSubType (column 2) if TypeOfSample and ItemType match
            {
                GridViewRow targetRow = null;
                for (int i = rowIndex - 1; i >= 0; i--)
                {
                    if (grid.Rows[i].Cells[2].Visible)
                    {
                        targetRow = grid.Rows[i];
                        break;
                    }
                }

                if (targetRow != null &&
                    currentItemSubType == targetRow.Cells[2].Text.Trim().ToLower() &&
                    currentItemType == targetRow.Cells[1].Text.Trim().ToLower() &&
                    currentTypeOfSample == targetRow.Cells[0].Text.Trim().ToLower() &&
                    !string.IsNullOrEmpty(currentItemSubType))
                {
                    if (targetRow.Cells[2].RowSpan == 0)
                        targetRow.Cells[2].RowSpan = 1;

                    targetRow.Cells[2].RowSpan += 1;
                    currentRow.Cells[2].Visible = false;
                }
            }

            // Merge Core (column 3) if TypeOfSample, ItemType, and ItemSubType match
            {
                GridViewRow targetRow = null;
                for (int i = rowIndex - 1; i >= 0; i--)
                {
                    if (grid.Rows[i].Cells[3].Visible)
                    {
                        targetRow = grid.Rows[i];
                        break;
                    }
                }

                if (targetRow != null &&
                    currentCore == targetRow.Cells[3].Text.Trim().ToLower() &&
                    currentItemSubType == targetRow.Cells[2].Text.Trim().ToLower() &&
                    currentItemType == targetRow.Cells[1].Text.Trim().ToLower() &&
                    currentTypeOfSample == targetRow.Cells[0].Text.Trim().ToLower() &&
                    !string.IsNullOrEmpty(currentCore))
                {
                    if (targetRow.Cells[3].RowSpan == 0)
                        targetRow.Cells[3].RowSpan = 1;

                    targetRow.Cells[3].RowSpan += 1;
                    currentRow.Cells[3].Visible = false;
                }
            }
        }
        protected void gvDimension_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);

                // Check if the "TestResults" ViewState exists
                DataTable dt = (DataTable)ViewState["TestResults"];
                //if (dt == null)
                //{
                //    // If "TestResults" doesn't exist, try "DimensionResults"
                //    dt = (DataTable)ViewState["DimensionResults"];
                //}

                // Ensure the DataTable exists and the row index is valid before attempting to remove the row
                if (dt != null && dt.Rows.Count > 0 && rowIndex >= 0 && rowIndex < dt.Rows.Count)
                {
                    dt.Rows.RemoveAt(rowIndex);  // Remove the row at the specified index
                    ViewState["TestResults"] = dt;  // Update ViewState with the modified DataTable for "TestResults"
                                                    // ViewState["DimensionResults"] = dt;  // Update ViewState with the modified DataTable for "DimensionResults"

                    gvDimension.DataSource = dt;  // Rebind the updated DataTable to the GridView
                    gvDimension.DataBind();  // Refresh the grid to reflect the deletion

                    // Update Specimen Number (if necessary)
                    //UpdateSpecimenNumber();
                }
            }
        }
        private void ShowMessage(string message)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('{message}');", true);
        }
        protected void btnClear_Click(object sender, EventArgs e)
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
            // testdate.Text = "";


            //  txtSpecimenNo.Text = "";
            txtInputs.Text = "";
            txtSpecifications.Text = "";
            txtItemType.Text = ""; ;
            txtsubitem.Text = "";

            txtMinMax.Text = "";

            // Clear GridView
            gvDimension.DataSource = null;
            gvDimension.DataBind();

            //// Standard Values
            //txtStandardWidth.Text = "";
            //txtStandardSpeed.Text = "";
            //txtStandardmeasured.Text = "";
            //txtStandardMin.Text = "";
            //txtStandardMax.Text = "";
            ViewState["TestResults"] = null;


        }
    }
}