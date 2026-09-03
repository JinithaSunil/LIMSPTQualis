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
    public partial class LossOfMass_Rev02 : System.Web.UI.Page
    {

        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateCoreDropdown();

                // Initialize DAL object
                if (_cm == null) _cm = new DAL.CM();

                // Get sample information from query string
                if (Request.QueryString["s"] != null)
                {
                    _cm.sampleId = Convert.ToInt32(Request.QueryString["s"].ToLower());
                }
                txtSpecimenNo.Text = "1";

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


                    roomno.Text = "-";
                    jobOrderNo.Text = "-";
                    worksheetno.Text = "-";
                    equipmentno.Text = "-";
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showError", "alert('Could not retrieve sample data. Please check your sample ID.');", true);
                }

                CheckAndPrefillForm();
            }
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

                        // Prefill upper section details
                        PrefillUpperSection(existingData);


                        // Prefill test results grid
                        PrefillTestResultsGrid(sampleId);
                        LoadDateTimeValues(sampleId);

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
        private void InitializeGrid()
        {
            // Create a DataTable to store the test results
            DataTable dt = new DataTable();

            dt.Columns.Add("CorePhase", typeof(string));
            dt.Columns.Add("SpecimenNo", typeof(int));
            dt.Columns.Add("CrossSectionarea", typeof(decimal));
            dt.Columns.Add("AfterAgeing", typeof(decimal));
            dt.Columns.Add("BeforeAgeing", typeof(decimal));
            dt.Columns.Add("LossOfMass", typeof(decimal));
            dt.Columns.Add("Median", typeof(decimal));

            // Add the 6 additional input columns
            dt.Columns.Add("InputthicknessInput1", typeof(decimal));
            dt.Columns.Add("InputthicknessInput2", typeof(decimal));
            dt.Columns.Add("InputthicknessInput3", typeof(decimal));
            dt.Columns.Add("InputthicknessInput4", typeof(decimal));
            dt.Columns.Add("InputthicknessInput5", typeof(decimal));
            dt.Columns.Add("InputthicknessInput6", typeof(decimal));
            dt.Columns.Add("InputthicknessInputAvg", typeof(decimal));
            dt.Columns.Add("InputthicknessInputMin", typeof(decimal));
            dt.Columns.Add("InputdiameterInput1", typeof(decimal));
            dt.Columns.Add("InputdiameterInput2", typeof(decimal));
            dt.Columns.Add("InputdiameterInput3", typeof(decimal));
            dt.Columns.Add("InputdiameterInput4", typeof(decimal));
            dt.Columns.Add("InputdiameterInput5", typeof(decimal));
            dt.Columns.Add("InputdiameterInput6", typeof(decimal));
            dt.Columns.Add("InputdiameterInputAvg", typeof(decimal));
            dt.Columns.Add("InputdiameterInputMin", typeof(decimal));

   


            // Bind the empty DataTable to the GridView
            gvPressureTest.DataSource = dt;
            gvPressureTest.DataBind();
            ViewState["TestResults"] = dt;
        }
        private void PrefillUpperSection(DataTable existingData)
        {
            // Uses ReportParamMapID
            SetTextBoxText(cableType, "14160");                      // Type
            SetTextBoxText(cableSize, "14161");                      // Size
            SetTextBoxText(voltage, "14162");                        // Voltage
            SetTextBoxText(Date, "14163");                           // Date
            SetTextBoxText(roomno, "14164");                         // RoomNo
            SetTextBoxText(roomtemp, "14165");                       // RoomTemp
            SetTextBoxText(refSpec, "14166");                        // RefSpec
            SetTextBoxText(testMethodNo, "14167");                   // TestMethodNo
            SetTextBoxText(jobOrderNo, "14168");                     // JobOrderNo
            SetTextBoxText(worksheetno, "14169");                    // WorkSheetNo
            SetTextBoxText(roomhumidity, "14170");                   // RoomHumidity
            SetTextBoxText(equipmentno, "14171");                    // EquipmentNo

            SetTextBoxText(txtSpecificThickness, "14181");           // SpecifiedThickness
            SetTextBoxText(txtSpecificDiameter, "14182");            // SpecifiedDiameter
            SetTextBoxText(txtSpecificcrosssecctionarea, "14194");   // Cross Section Area
            SetTextBoxText(txtSpecificbeforeageing, "14183");        // SpecifiedBeforeAgeing
            SetTextBoxText(txtSpecificafterageing, "14184");         // SpecifiedAfterAgeing
            SetTextBoxText(txtSpecificMedian, "14185");              // SpecifiedMedian
            SetTextBoxText(txtAirflow, "14190");                     // Airflow
            SetTextBoxText(txtOvenTemp, "14191");                    // Oven Temperature

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
        protected void ResetInputs()
        {
            txtCrossSectionarea.Text = "";
            txtbeforeageing.Text = "";
            txtafterkageing.Text = "";

            txtCrossSectionarea.Enabled = true;
            txtbeforeageing.Enabled = true;
            txtafterkageing.Enabled = true;
        }
        private void PrefillTestResultsGrid(int sampleId)
        {
            try
            {
                InitializeGrid();
                if (_cm == null) _cm = new DAL.CM();

                DataTable dt = ViewState["TestResults"] as DataTable;
                if (dt == null) return;
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    conn.Open();
                    string query = @"
SELECT ReportParamMapID, Result 
FROM TestResultsPTQualis 
WHERE SampleID = @SampleID 
AND ReportParamMapID IN ('14172','14173','14174','14175','14176','14177','14178','14179','14180')";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SampleID", sampleId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            dt.Clear();

                            while (reader.Read())
                            {
                                string paramId = reader["ReportParamMapID"].ToString();
                                string result = reader["Result"].ToString();

                                switch (paramId)
                                {
                                    case "14173": // Thickness data
                                        if (!string.IsNullOrEmpty(result))
                                        {
                                            string[] coreData = result.Split('|');
                                            foreach (string coreEntry in coreData)
                                            {
                                                string[] parts = coreEntry.Split(';');
                                                if (parts.Length < 2) continue;

                                                string corePhase = parts[0];
                                                string[] specimenParts = parts[1].Split(':');
                                                if (specimenParts.Length < 2) continue;

                                                if (!int.TryParse(specimenParts[0], out int specimenNo)) continue;
                                                string[] values = specimenParts[1].Split(',');

                                                // Find or create row
                                                DataRow row = dt.Rows.Cast<DataRow>().FirstOrDefault(r =>
                                                    r["CorePhase"].ToString() == corePhase && Convert.ToInt32(r["SpecimenNo"]) == specimenNo);

                                                if (row == null)
                                                {
                                                    row = dt.NewRow();
                                                    row["CorePhase"] = corePhase;
                                                    row["SpecimenNo"] = specimenNo;
                                                    dt.Rows.Add(row);
                                                }

                                                // Fill thickness inputs
                                                for (int i = 0; i < Math.Min(values.Length, 6); i++)
                                                {
                                                    if (decimal.TryParse(values[i], out decimal value))
                                                        row[$"InputthicknessInput{i + 1}"] = value;
                                                }
                                            }
                                        }
                                        break;

                                    case "14174": // ThicknessAvg and Min
                                        if (!string.IsNullOrEmpty(result))
                                        {
                                            string[] entries = result.Split('|');
                                            foreach (string entry in entries)
                                            {
                                                string[] parts = entry.Split(';');
                                                if (parts.Length < 2) continue;

                                                string corePhase = parts[0];
                                                string[] specimenParts = parts[1].Split(':');
                                                if (specimenParts.Length < 2) continue;

                                                if (!int.TryParse(specimenParts[0], out int specimenNo)) continue;
                                                string[] values = specimenParts[1].Split(',');

                                                // Find or create row
                                                DataRow row = dt.Rows.Cast<DataRow>().FirstOrDefault(r =>
                                                    r["CorePhase"].ToString() == corePhase && Convert.ToInt32(r["SpecimenNo"]) == specimenNo);

                                                if (row == null)
                                                {
                                                    row = dt.NewRow();
                                                    row["CorePhase"] = corePhase;
                                                    row["SpecimenNo"] = specimenNo;
                                                    dt.Rows.Add(row);
                                                }

                                                // Store avg and min in separate columns
                                                if (values.Length > 0 && decimal.TryParse(values[0], out decimal avg))
                                                    row["InputthicknessInputAvg"] = avg;

                                                if (values.Length > 1 && decimal.TryParse(values[1], out decimal min))
                                                    row["InputthicknessInputMin"] = min;
                                            }
                                        }
                                        break;

                                    case "14175": // Diameter data
                                        if (!string.IsNullOrEmpty(result))
                                        {
                                            string[] coreData = result.Split('|');
                                            foreach (string coreEntry in coreData)
                                            {
                                                string[] parts = coreEntry.Split(';');
                                                if (parts.Length < 2) continue;

                                                string corePhase = parts[0];
                                                string[] specimenParts = parts[1].Split(':');
                                                if (specimenParts.Length < 2) continue;

                                                if (!int.TryParse(specimenParts[0], out int specimenNo)) continue;
                                                string[] values = specimenParts[1].Split(',');

                                                // Find or create row
                                                DataRow row = dt.Rows.Cast<DataRow>().FirstOrDefault(r =>
                                                    r["CorePhase"].ToString() == corePhase && Convert.ToInt32(r["SpecimenNo"]) == specimenNo);

                                                if (row == null)
                                                {
                                                    row = dt.NewRow();
                                                    row["CorePhase"] = corePhase;
                                                    row["SpecimenNo"] = specimenNo;
                                                    dt.Rows.Add(row);
                                                }

                                                // Fill diameter inputs
                                                for (int i = 0; i < Math.Min(values.Length, 6); i++)
                                                {
                                                    if (decimal.TryParse(values[i], out decimal value))
                                                        row[$"InputdiameterInput{i + 1}"] = value;
                                                }
                                            }
                                        }
                                        break;

                                    case "14176": // DiameterAvg and Min
                                        if (!string.IsNullOrEmpty(result))
                                        {
                                            string[] entries = result.Split('|');
                                            foreach (string entry in entries)
                                            {
                                                string[] parts = entry.Split(';');
                                                if (parts.Length < 2) continue;

                                                string corePhase = parts[0];
                                                string[] specimenParts = parts[1].Split(':');
                                                if (specimenParts.Length < 2) continue;

                                                if (!int.TryParse(specimenParts[0], out int specimenNo)) continue;
                                                string[] values = specimenParts[1].Split(',');

                                                // Find or create row
                                                DataRow row = dt.Rows.Cast<DataRow>().FirstOrDefault(r =>
                                                    r["CorePhase"].ToString() == corePhase && Convert.ToInt32(r["SpecimenNo"]) == specimenNo);

                                                if (row == null)
                                                {
                                                    row = dt.NewRow();
                                                    row["CorePhase"] = corePhase;
                                                    row["SpecimenNo"] = specimenNo;
                                                    dt.Rows.Add(row);
                                                }

                                                // Store avg and min in separate columns
                                                if (values.Length > 0 && decimal.TryParse(values[0], out decimal avg))
                                                    row["InputdiameterInputAvg"] = avg;

                                                if (values.Length > 1 && decimal.TryParse(values[1], out decimal min))
                                                    row["InputdiameterInputMin"] = min;
                                            }
                                        }
                                        break;

                                    case "14172": // CrossSectionArea
                                        if (!string.IsNullOrEmpty(result))
                                        {
                                            string[] entries = result.Split('|');
                                            foreach (string entry in entries)
                                            {
                                                string[] parts = entry.Split(';');
                                                if (parts.Length < 2) continue;

                                                string corePhase = parts[0];
                                                string[] specimenParts = parts[1].Split(':');
                                                if (specimenParts.Length < 2) continue;

                                                if (!int.TryParse(specimenParts[0], out int specimenNo)) continue;
                                                string value = specimenParts[1];

                                                DataRow row = dt.Rows.Cast<DataRow>().FirstOrDefault(r =>
                                                    r["CorePhase"].ToString() == corePhase && Convert.ToInt32(r["SpecimenNo"]) == specimenNo);

                                                if (row == null)
                                                {
                                                    row = dt.NewRow();
                                                    row["CorePhase"] = corePhase;
                                                    row["SpecimenNo"] = specimenNo;
                                                    dt.Rows.Add(row);
                                                }

                                                if (decimal.TryParse(value, out decimal decValue))
                                                    row["CrossSectionarea"] = decValue;
                                            }
                                        }
                                        break;

                                    case "14177": // beforeageing
                                        if (!string.IsNullOrEmpty(result))
                                        {
                                            string[] entries = result.Split('|');
                                            foreach (string entry in entries)
                                            {
                                                string[] parts = entry.Split(';');
                                                if (parts.Length < 2) continue;

                                                string corePhase = parts[0];
                                                string[] specimenParts = parts[1].Split(':');
                                                if (specimenParts.Length < 2) continue;

                                                if (!int.TryParse(specimenParts[0], out int specimenNo)) continue;
                                                string value = specimenParts[1];

                                                DataRow row = dt.Rows.Cast<DataRow>().FirstOrDefault(r =>
                                                    r["CorePhase"].ToString() == corePhase && Convert.ToInt32(r["SpecimenNo"]) == specimenNo);

                                                if (row == null)
                                                {
                                                    row = dt.NewRow();
                                                    row["CorePhase"] = corePhase;
                                                    row["SpecimenNo"] = specimenNo;
                                                    dt.Rows.Add(row);
                                                }

                                                if (decimal.TryParse(value, out decimal decValue))
                                                    row["BeforeAgeing"] = decValue;
                                            }
                                        }
                                        break;

                                    case "14178": // afterageing
                                        if (!string.IsNullOrEmpty(result))
                                        {
                                            string[] entries = result.Split('|');
                                            foreach (string entry in entries)
                                            {
                                                string[] parts = entry.Split(';');
                                                if (parts.Length < 2) continue;

                                                string corePhase = parts[0];
                                                string[] specimenParts = parts[1].Split(':');
                                                if (specimenParts.Length < 2) continue;

                                                if (!int.TryParse(specimenParts[0], out int specimenNo)) continue;
                                                string value = specimenParts[1];

                                                DataRow row = dt.Rows.Cast<DataRow>().FirstOrDefault(r =>
                                                    r["CorePhase"].ToString() == corePhase && Convert.ToInt32(r["SpecimenNo"]) == specimenNo);

                                                if (row == null)
                                                {
                                                    row = dt.NewRow();
                                                    row["CorePhase"] = corePhase;
                                                    row["SpecimenNo"] = specimenNo;
                                                    dt.Rows.Add(row);
                                                }

                                                if (decimal.TryParse(value, out decimal decValue))
                                                    row["AfterAgeing"] = decValue;
                                            }
                                        }
                                        break;

                                    case "14179": // median
                                        if (!string.IsNullOrEmpty(result))
                                        {
                                            string[] entries = result.Split('|');
                                            foreach (string entry in entries)
                                            {
                                                string[] parts = entry.Split(';');
                                                if (parts.Length < 2) continue;

                                                string corePhase = parts[0];
                                                string[] specimenParts = parts[1].Split(':');
                                                if (specimenParts.Length < 2) continue;

                                                if (!int.TryParse(specimenParts[0], out int specimenNo)) continue;
                                                string value = specimenParts[1];

                                                DataRow row = dt.Rows.Cast<DataRow>().FirstOrDefault(r =>
                                                    r["CorePhase"].ToString() == corePhase && Convert.ToInt32(r["SpecimenNo"]) == specimenNo);

                                                if (row == null)
                                                {
                                                    row = dt.NewRow();
                                                    row["CorePhase"] = corePhase;
                                                    row["SpecimenNo"] = specimenNo;
                                                    dt.Rows.Add(row);
                                                }

                                                if (decimal.TryParse(value, out decimal decValue))
                                                    row["Median"] = decValue;
                                            }
                                        }
                                        break;

                                    case "14180": // lossofmass
                                        if (!string.IsNullOrEmpty(result))
                                        {
                                            string[] entries = result.Split('|');
                                            foreach (string entry in entries)
                                            {
                                                string[] parts = entry.Split(';');
                                                if (parts.Length < 2) continue;

                                                string corePhase = parts[0];
                                                string[] specimenParts = parts[1].Split(':');
                                                if (specimenParts.Length < 2) continue;

                                                if (!int.TryParse(specimenParts[0], out int specimenNo)) continue;
                                                string value = specimenParts[1];

                                                DataRow row = dt.Rows.Cast<DataRow>().FirstOrDefault(r =>
                                                    r["CorePhase"].ToString() == corePhase && Convert.ToInt32(r["SpecimenNo"]) == specimenNo);

                                                if (row == null)
                                                {
                                                    row = dt.NewRow();
                                                    row["CorePhase"] = corePhase;
                                                    row["SpecimenNo"] = specimenNo;
                                                    dt.Rows.Add(row);
                                                }

                                                if (decimal.TryParse(value, out decimal decValue))
                                                    row["LossOfMass"] = decValue;
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }

                gvPressureTest.DataSource = dt;
                gvPressureTest.DataBind();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "GridPrefillError", $"alert('Error: {ex.Message}');", true);
            }
        }
        private void LoadDateTimeValues(int sampleId)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
            {
                conn.Open();
                string query = @"
SELECT ReportParamMapID, Result 
FROM TestResultsPTQualis 
WHERE SampleID = @SampleID 
AND ReportParamMapID IN ('14186','14187','14188','14189','14192','14193')";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SampleID", sampleId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string paramId = reader["ReportParamMapID"].ToString();
                            string result = reader["Result"].ToString();

                            switch (paramId)
                            {
                                case "14186": // Conditioningbeforeageingdate
                                    if (!string.IsNullOrEmpty(result))
                                    {
                                        string[] dates = result.Split(';');
                                        if (dates.Length >= 1)
                                        {
                                            if (DateTime.TryParse(dates[0], out DateTime startDate))
                                                txtDesicBeforeDateStart.Text = startDate.ToString("yyyy-MM-dd");
                                        }
                                        if (dates.Length >= 2)
                                        {
                                            if (DateTime.TryParse(dates[1], out DateTime finishDate))
                                                txtDesicBeforeDateFinish.Text = finishDate.ToString("yyyy-MM-dd");
                                        }
                                    }
                                    break;

                                case "14187": // ConditioningbeforeageingTime
                                    if (!string.IsNullOrEmpty(result))
                                    {
                                        string[] times = result.Split(';');
                                        if (times.Length >= 1)
                                            txtDesicBeforeTimeStart.Text = times[0];
                                        if (times.Length >= 2)
                                            txtDesicBeforeTimeFinish.Text = times[1];
                                    }
                                    break;

                                case "14188": // Conditioningafterageingdate
                                    if (!string.IsNullOrEmpty(result))
                                    {
                                        string[] dates = result.Split(';');
                                        if (dates.Length >= 1)
                                        {
                                            if (DateTime.TryParse(dates[0], out DateTime startDate))
                                                txtDesicAfterDateStart.Text = startDate.ToString("yyyy-MM-dd");
                                        }
                                        if (dates.Length >= 2)
                                        {
                                            if (DateTime.TryParse(dates[1], out DateTime finishDate))
                                                txtDesicAfterDateFinish.Text = finishDate.ToString("yyyy-MM-dd");
                                        }
                                    }
                                    break;

                                case "14189": // ConditioningafterageingTime
                                    if (!string.IsNullOrEmpty(result))
                                    {
                                        string[] times = result.Split(';');
                                        if (times.Length >= 1)
                                            txtDesicAfterTimeStart.Text = times[0];
                                        if (times.Length >= 2)
                                            txtDesicAfterTimeFinish.Text = times[1];
                                    }
                                    break;

                                case "14192": // ageinginairovenadate
                                    if (!string.IsNullOrEmpty(result))
                                    {
                                        string[] dates = result.Split(';');
                                        if (dates.Length >= 1)
                                        {
                                            if (DateTime.TryParse(dates[0], out DateTime startDate))
                                                txtAirOvenDateStart.Text = startDate.ToString("yyyy-MM-dd");
                                        }
                                        if (dates.Length >= 2)
                                        {
                                            if (DateTime.TryParse(dates[1], out DateTime finishDate))
                                                txtAirOvenDateFinish.Text = finishDate.ToString("yyyy-MM-dd");
                                        }
                                    }
                                    break;

                                case "14193": // ageinginairovenatime
                                    if (!string.IsNullOrEmpty(result))
                                    {
                                        string[] times = result.Split(';');
                                        if (times.Length >= 1)
                                            txtAirOvenTimeStart.Text = times[0];
                                        if (times.Length >= 2)
                                            txtAirOvenTimeFinish.Text = times[1];
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }   
        protected void ddlCorePhase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlCorePhase.SelectedValue))
            {

                txtSpecimenNo.Text = "";
                ShowMessage("Please select a valid Core Phase.");
                return;
            }

            txtSpecimenNo.Text = "1";
        }
        protected void UpdateSpecimenNumber()
        {
            // Get the DataTable from ViewState
            DataTable dt = (DataTable)ViewState["TestResults"];

            // Calculate next specimen number
            int nextSpecimenNo = 1; // Default to 1 if no rows
            if (dt != null && dt.Rows.Count > 0)
            {
                nextSpecimenNo = dt.Rows.Count + 1;
            }

            // Find your actual control - you need to use the correct ID and search method
            // If it's in a form:
            TextBox txtSpecimenNo = FindControl("txtSpecimenNo") as TextBox;


            if (txtSpecimenNo != null)
            {
                txtSpecimenNo.Text = nextSpecimenNo.ToString();
            }

        }
        protected void ThicknessInputs_TextChanged(object sender, EventArgs e)
        {
            string[] inputs = new string[] {
        txtthicknessInput1.Text.Trim(),
        txtthicknessInput2.Text.Trim(),
        txtthicknessInput3.Text.Trim(),
        txtthicknessInput4.Text.Trim(),
        txtthicknessInput5.Text.Trim(),
        txtthicknessInput6.Text.Trim()
    };

            decimal sum = 0;
            int count = 0;
            decimal? min = null;

            foreach (string input in inputs)
            {
                if (input == "-")
                {
                    count++; // "-" counts for average but not in sum or min
                }
                else if (decimal.TryParse(input, out decimal value))
                {
                    sum += value;
                    count++;
                    if (min == null || value < min) min = value;
                }
            }

            decimal avgDecimal = (count > 0) ? Math.Round(sum / count, 2) : 0;
            decimal minDecimal = (min != null) ? Math.Round(min.Value, 2) : 0;

            txtthicknessInputAvg.Text = $"Avg: {avgDecimal}, Min: {minDecimal}";

            // Store numeric values in hidden fields for validation/storage
            hiddenThicknessAvg.Value = avgDecimal.ToString();
            hiddenThicknessMin.Value = minDecimal.ToString();
        }
        protected void DiameterInputs_TextChanged(object sender, EventArgs e)
        {
            string[] inputs = new string[] {
        txtdiameterInput1.Text.Trim(),
        txtdiameterInput2.Text.Trim(),
        txtdiameterInput3.Text.Trim(),
        txtdiameterInput4.Text.Trim(),
        txtdiameterInput5.Text.Trim(),
        txtdiameterInput6.Text.Trim()
    };

            decimal sum = 0;
            int count = 0;
            decimal? min = null;

            foreach (string input in inputs)
            {
                if (input == "-")
                {
                    count++; // "-" counts for average but not in sum or min
                }
                else if (decimal.TryParse(input, out decimal value))
                {
                    sum += value;
                    count++;
                    if (min == null || value < min) min = value;
                }
            }

            decimal avgDecimal = (count > 0) ? Math.Round(sum / count, 2) : 0;
            decimal minDecimal = (min != null) ? Math.Round(min.Value, 2) : 0;

            txtdiameterInputAvg.Text = $"Avg: {avgDecimal}, Min: {minDecimal}";

            // Store numeric values in hidden fields for validation/storage
            hiddenDiameterAvg.Value = avgDecimal.ToString();
            hiddenDiameterMin.Value = minDecimal.ToString();
        }
        protected void btnAddToGrid_Click(object sender, EventArgs e)
        {
            ResetControlBorders();

            bool hasError = false;
            List<Control> invalidControls = new List<Control>();

            int specimenNo = 0;

            // Validate CorePhase
            if (ddlCorePhase.SelectedIndex < 0 || string.IsNullOrWhiteSpace(ddlCorePhase.SelectedValue))
            {
                ddlCorePhase.CssClass += " input-error";
                invalidControls.Add(ddlCorePhase);
                hasError = true;
            }

            // Validate SpecimenNo
            if (string.IsNullOrWhiteSpace(txtSpecimenNo.Text) || !int.TryParse(txtSpecimenNo.Text.Trim(), out specimenNo))
            {
                invalidControls.Add(txtSpecimenNo);
                hasError = true;
            }

            TextBox[] thicknessInputs = {
    txtthicknessInput1, txtthicknessInput2, txtthicknessInput3,
    txtthicknessInput4, txtthicknessInput5, txtthicknessInput6
};

            TextBox[] diameterInputs = {
    txtdiameterInput1, txtdiameterInput2, txtdiameterInput3,
    txtdiameterInput4, txtdiameterInput5, txtdiameterInput6
};

            TextBox[] otherInputs = {
    txtCrossSectionarea, txtbeforeageing, txtafterkageing, txtloadofmass
};

            // Validate thickness inputs (no dashes allowed)
            foreach (TextBox txt in thicknessInputs)
            {
                string val = txt.Text.Trim();
                if (string.IsNullOrWhiteSpace(val) || !decimal.TryParse(val, out _))
                {
                    invalidControls.Add(txt);
                    hasError = true;
                }
            }

            // Validate diameter inputs (allow "-")
            foreach (TextBox txt in diameterInputs)
            {
                string val = txt.Text.Trim();
                if (string.IsNullOrWhiteSpace(val) || (val != "-" && !decimal.TryParse(val, out _)))
                {
                    invalidControls.Add(txt);
                    hasError = true;
                }
            }

            // Validate other decimal inputs
            foreach (TextBox txt in otherInputs)
            {
                string val = txt.Text.Trim();
                if (string.IsNullOrWhiteSpace(val) || !decimal.TryParse(val, out _))
                {
                    invalidControls.Add(txt);
                    hasError = true;
                }
            }

            // Highlight invalid controls
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

            // Get or initialize DataTable
            DataTable dt = ViewState["TestResults"] as DataTable;
            if (dt == null)
            {
                dt = new DataTable();
                dt.Columns.Add("CorePhase", typeof(string));
                dt.Columns.Add("SpecimenNo", typeof(int));
                dt.Columns.Add("CrossSectionarea", typeof(decimal));
                dt.Columns.Add("AfterAgeing", typeof(decimal));
                dt.Columns.Add("BeforeAgeing", typeof(decimal));
                dt.Columns.Add("LossOfMass", typeof(decimal));
                dt.Columns.Add("Median", typeof(decimal));

                dt.Columns.Add("InputthicknessInput1", typeof(decimal));
                dt.Columns.Add("InputthicknessInput2", typeof(decimal));
                dt.Columns.Add("InputthicknessInput3", typeof(decimal));
                dt.Columns.Add("InputthicknessInput4", typeof(decimal));
                dt.Columns.Add("InputthicknessInput5", typeof(decimal));
                dt.Columns.Add("InputthicknessInput6", typeof(decimal));
                dt.Columns.Add("InputthicknessInputAvg", typeof(decimal));
                dt.Columns.Add("InputthicknessInputMin", typeof(decimal));
                dt.Columns.Add("InputdiameterInput1", typeof(decimal));
                dt.Columns.Add("InputdiameterInput2", typeof(decimal));
                dt.Columns.Add("InputdiameterInput3", typeof(decimal));
                dt.Columns.Add("InputdiameterInput4", typeof(decimal));
                dt.Columns.Add("InputdiameterInput5", typeof(decimal));
                dt.Columns.Add("InputdiameterInput6", typeof(decimal));
                dt.Columns.Add("InputdiameterInputAvg", typeof(decimal));
                dt.Columns.Add("InputdiameterInputMin", typeof(decimal));
            }

            // Check for duplicate SpecimenNo + CorePhase
            bool duplicateExists = dt.AsEnumerable().Any(row =>
                row["CorePhase"].ToString() == ddlCorePhase.SelectedItem.Text &&
                Convert.ToInt32(row["SpecimenNo"]) == specimenNo
            );

            if (duplicateExists)
            {
                ShowMessage($"Data for Core Phase '{ddlCorePhase.SelectedItem.Text}' and Specimen No. '{specimenNo}' already exists. Please delete it before adding again.");
                txtSpecimenNo.Text = (specimenNo + 1).ToString();
                return;
            }

            // Create new row with proper decimal parsing
            DataRow dr = dt.NewRow();

            dr["CorePhase"] = ddlCorePhase.SelectedItem.Text;
            dr["SpecimenNo"] = specimenNo;
            dr["CrossSectionarea"] = decimal.Parse(txtCrossSectionarea.Text.Trim());
            dr["AfterAgeing"] = decimal.Parse(txtbeforeageing.Text.Trim());
            dr["BeforeAgeing"] = decimal.Parse(txtafterkageing.Text.Trim());
            dr["LossOfMass"] = decimal.Parse(txtloadofmass.Text.Trim());

            dr["InputthicknessInput1"] = decimal.Parse(txtthicknessInput1.Text.Trim());
            dr["InputthicknessInput2"] = decimal.Parse(txtthicknessInput2.Text.Trim());
            dr["InputthicknessInput3"] = decimal.Parse(txtthicknessInput3.Text.Trim());
            dr["InputthicknessInput4"] = decimal.Parse(txtthicknessInput4.Text.Trim());
            dr["InputthicknessInput5"] = decimal.Parse(txtthicknessInput5.Text.Trim());
            dr["InputthicknessInput6"] = decimal.Parse(txtthicknessInput6.Text.Trim());
            dr["InputthicknessInputAvg"] = decimal.Parse(hiddenThicknessAvg.Value);
            dr["InputthicknessInputMin"] = decimal.Parse(hiddenThicknessMin.Value);

            dr["InputdiameterInput1"] = Convert.ToDecimal(txtdiameterInput1.Text.Trim() == "-" ? "0" : txtdiameterInput1.Text.Trim());
            dr["InputdiameterInput2"] = Convert.ToDecimal(txtdiameterInput2.Text.Trim() == "-" ? "0" : txtdiameterInput2.Text.Trim());
            dr["InputdiameterInput3"] = Convert.ToDecimal(txtdiameterInput3.Text.Trim() == "-" ? "0" : txtdiameterInput3.Text.Trim());
            dr["InputdiameterInput4"] = Convert.ToDecimal(txtdiameterInput4.Text.Trim() == "-" ? "0" : txtdiameterInput4.Text.Trim());
            dr["InputdiameterInput5"] = Convert.ToDecimal(txtdiameterInput5.Text.Trim() == "-" ? "0" : txtdiameterInput5.Text.Trim());
            dr["InputdiameterInput6"] = Convert.ToDecimal(txtdiameterInput6.Text.Trim() == "-" ? "0" : txtdiameterInput6.Text.Trim());
            dr["InputdiameterInputAvg"] = decimal.Parse(hiddenDiameterAvg.Value);
            dr["InputdiameterInputMin"] = decimal.Parse(hiddenDiameterMin.Value);

            dt.Rows.Add(dr);

            // Calculate and update Median per CorePhase group based on LossOfMass
            foreach (var group in dt.AsEnumerable().GroupBy(r => r.Field<string>("CorePhase")))
            {
                var lossOfMassValues = group
                    .Where(r => !r.IsNull("LossOfMass"))
                    .Select(r => r.Field<decimal>("LossOfMass"))
                    .Where(d => d > 0)
                    .OrderBy(d => d)
                    .ToList();

                string medianValue = CalculateMedian(lossOfMassValues);
                decimal medianDecimal = decimal.TryParse(medianValue, out var medVal) ? medVal : 0m;

                foreach (var row in group)
                {
                    row["Median"] = medianDecimal;
                }
            }

            ViewState["TestResults"] = dt;

            gvPressureTest.DataSource = dt;
            gvPressureTest.DataBind();

            decimal maxMedian = dt.AsEnumerable()
                  .Select(row => row.Field<decimal>("Median"))
                  .DefaultIfEmpty(0)
                  .Max();
            txtSpecificMedian.Text = maxMedian.ToString("F2");

            txtSpecimenNo.Text = (specimenNo + 1).ToString();

            ClearInputFields();
        }
        private string CalculateMedian(List<decimal> values)
        {
            if (values == null || values.Count == 0)
                return "0";

            values = values.OrderBy(x => x).ToList();
            int count = values.Count;

            if (count % 2 == 0)
            {
                // Even number of values - average of middle two
                decimal median = (values[count / 2 - 1] + values[count / 2]) / 2;
                return median.ToString("F2");
            }
            else
            {
                // Odd number of values - middle value
                decimal median = values[count / 2];
                return median.ToString("F2");
            }
        }
        protected void btnClearData_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            List<string> errors = new List<string>();
            List<Control> invalidControls = new List<Control>();

            // Check if Grid has data
            bool gridHasData = (gvPressureTest.Rows.Count > 0);


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
            if (roomhumidity.Text.Trim() != "-" && (string.IsNullOrEmpty(roomhumidity.Text) || !double.TryParse(roomhumidity.Text, out _)))
            {
                errors.Add("Room Humidity must be a valid number.");
                invalidControls.Add(roomhumidity);
            }
            if (roomtemp.Text.Trim() != "-" && (string.IsNullOrEmpty(roomtemp.Text) || !double.TryParse(roomtemp.Text, out _)))
            {
                errors.Add("Room Temp. must be a valid number.");
                invalidControls.Add(roomtemp);
            }


            if (txtSpecificbeforeageing.Text.Trim() != "-" && (string.IsNullOrEmpty(txtSpecificbeforeageing.Text) || !double.TryParse(txtSpecificbeforeageing.Text, out _)))
            {
                errors.Add("Specific Before Ageing at  6 hour  must be a valid number or '-'.");
                invalidControls.Add(txtSpecificbeforeageing);
            }

            if (txtSpecificafterageing.Text.Trim() != "-" && (string.IsNullOrEmpty(txtSpecificafterageing.Text) || !double.TryParse(txtSpecificafterageing.Text, out _)))
            {
                errors.Add("Specific After Ageing at  6 hour  must be a valid number or '-'.");
                invalidControls.Add(txtSpecificafterageing);
            }


            if (txtSpecificcrosssecctionarea.Text.Trim() != "-" && (string.IsNullOrEmpty(txtSpecificcrosssecctionarea.Text) || !double.TryParse(txtSpecificcrosssecctionarea.Text, out _)))
            {
                errors.Add("Specific Cross Section Area at  6 hour  must be a valid number or '-'.");
                invalidControls.Add(txtSpecificcrosssecctionarea);
            }


            if (txtSpecificThickness.Text.Trim() != "-" && (string.IsNullOrEmpty(txtSpecificThickness.Text) || !double.TryParse(txtSpecificThickness.Text, out _)))
            {
                errors.Add("Specific Thickness  must be a valid number or '-'.");
                invalidControls.Add(txtSpecificThickness);
            }



            if (txtSpecificDiameter.Text.Trim() != "-" && (string.IsNullOrEmpty(txtSpecificDiameter.Text) || !double.TryParse(txtSpecificDiameter.Text, out _)))
            {
                errors.Add("Specific Diameter  must be a valid number or '-'.");
                invalidControls.Add(txtSpecificDiameter);
            }


            if (txtSpecificMedian.Text.Trim() != "-" && (string.IsNullOrEmpty(txtSpecificMedian.Text) || !double.TryParse(txtSpecificMedian.Text, out _)))
            {
                errors.Add("Specific Median  must be a valid number or '-'.");
                invalidControls.Add(txtSpecificMedian);
            }




            // Only validate the upper fields if grid has no data
            if (!gridHasData)
            {
                // Validate Test Details Input 
                if (string.IsNullOrEmpty(txtCrossSectionarea.Text) ||
                    (txtCrossSectionarea.Text.Trim() != "-" && !double.TryParse(txtCrossSectionarea.Text.Trim(), out _)))
                {
                    errors.Add("Cross Section area must be a valid number or '-'.");
                    invalidControls.Add(txtCrossSectionarea);
                }

                if (string.IsNullOrEmpty(txtbeforeageing.Text) ||
                    (txtbeforeageing.Text.Trim() != "-" && !double.TryParse(txtbeforeageing.Text.Trim(), out _)))
                {
                    errors.Add("before Ageing must be a valid number or '-'.");
                    invalidControls.Add(txtbeforeageing);
                }

                if (string.IsNullOrEmpty(txtafterkageing.Text) ||
                    (txtafterkageing.Text.Trim() != "-" && !double.TryParse(txtafterkageing.Text.Trim(), out _)))
                {
                    errors.Add("After Ageing must be a valid number or '-'.");
                    invalidControls.Add(txtafterkageing);
                }
                if (string.IsNullOrEmpty(txtafterkageing.Text) ||
                    (txtafterkageing.Text.Trim() != "-" && !double.TryParse(txtafterkageing.Text.Trim(), out _)))
                {
                    errors.Add("After Ageing must be a valid number or '-'.");
                    invalidControls.Add(txtafterkageing);
                }
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
        protected void gvPressureTest_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            DataTable dt = ViewState["TestResults"] as DataTable;
            if (dt == null) return;

            DataRowView drv = (DataRowView)e.Row.DataItem;
            string currentCore = drv["CorePhase"].ToString();
            int specimenNo = Convert.ToInt32(drv["SpecimenNo"]);


            var rowsForCore = dt.AsEnumerable()
                .Where(r => r.Field<string>("CorePhase") == currentCore)
                .OrderBy(r => r.Field<int>("SpecimenNo"))
                .ToList();

            int currentIndex = rowsForCore.FindIndex(r => r.Field<int>("SpecimenNo") == specimenNo);

            int corePhaseColIndex = GetColumnIndexByName("CorePhase");
            int medianColIndex = GetColumnIndexByName("Median");

            if (corePhaseColIndex == -1 || medianColIndex == -1)
                return; // Column not found

            TableCell coreCell = e.Row.Cells[corePhaseColIndex];
            TableCell medianCell = e.Row.Cells[medianColIndex];

            if (currentIndex == 0)
            {
                int rowSpanCount = rowsForCore.Count;
                coreCell.RowSpan = rowSpanCount;
                medianCell.RowSpan = rowSpanCount;

                decimal median = drv.Row.Field<decimal>("Median");
                medianCell.Text = median.ToString("F2");
            }
            else
            {
                coreCell.Visible = false;
                medianCell.Visible = false;
            }
        }
        private int GetColumnIndexByName(string columnName)
        {
            for (int i = 0; i < gvPressureTest.Columns.Count; i++)
            {
                if (gvPressureTest.Columns[i] is BoundField boundField)
                {
                    if (boundField.DataField == columnName)
                        return i;
                }
                else if (gvPressureTest.Columns[i] is TemplateField templateField)
                {
                    if (templateField.HeaderText == columnName)
                        return i;
                }
            }
            return -1; // Column not found
        }
        protected void gvPressureTest_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                int index = Convert.ToInt32(e.CommandArgument);

                // Get the DataTable from ViewState
                DataTable dt = ViewState["TestResults"] as DataTable;

                // Delete the row from the DataTable
                if (dt != null && index >= 0 && index < dt.Rows.Count)
                {
                    dt.Rows.RemoveAt(index);

                    // Update ViewState and rebind the grid
                    ViewState["TestResults"] = dt;
                    gvPressureTest.DataSource = dt;
                    gvPressureTest.DataBind();


                    // Show confirmation message
                    ShowMessage("Record deleted successfully.");
                }
            }
        }
        // Helper method
        private decimal TryParseDecimal(string input)
        {
            return decimal.TryParse(input, out decimal result) ? result : -1;
        }
        private void ProcessColumn(DataTable dt, int currentRowIndex, GridViewRow currentRow, int columnIndex)
        {
            try
            {
                // Get column name from the column index
                string columnName = dt.Columns[columnIndex].ColumnName;

                // Assuming Input1–Input6 are at indexes 3–8 exactly
                if (columnIndex >= 3 && columnIndex <= 8)
                {
                    return;
                }

                // Only get values if we're going to use them
                // Get current and previous cell values
                string currentValue = dt.Rows[currentRowIndex][columnName]?.ToString();
                string previousValue = dt.Rows[currentRowIndex - 1][columnName]?.ToString();

                // If values are the same, merge cells
                if (currentValue == previousValue)
                {
                    // Hide current cell
                    currentRow.Cells[columnIndex].Visible = false;
                    // Find the first visible cell above with the same value
                    int spanRowIndex = currentRowIndex - 1;
                    while (spanRowIndex > 0 && !gvPressureTest.Rows[spanRowIndex].Cells[columnIndex].Visible)
                    {
                        spanRowIndex--;
                    }
                    // Increase rowspan of that cell
                    if (gvPressureTest.Rows[spanRowIndex].Cells[columnIndex].RowSpan == 0)
                    {
                        gvPressureTest.Rows[spanRowIndex].Cells[columnIndex].RowSpan = 2;
                    }
                    else
                    {
                        gvPressureTest.Rows[spanRowIndex].Cells[columnIndex].RowSpan += 1;
                    }
                }
            }
            catch (Exception ex)
            {
                // Add error logging or handling as needed
                System.Diagnostics.Debug.WriteLine("Error in ProcessColumn: " + ex.Message);
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
        private void ClearForm()
        {
            // Main Section
            Date.Text = "";
            roomno.Text = "";
            roomhumidity.Text = "";
            roomtemp.Text = "";
            worksheetno.Text = "";
            equipmentno.Text = "";


            ddlCorePhase.SelectedIndex = 0;
            txtthicknessInput1.Text = "";
            txtthicknessInput2.Text = "";
            txtthicknessInput3.Text = "";
            txtthicknessInput4.Text = "";
            txtthicknessInput5.Text = "";
            txtthicknessInput6.Text = "";
            txtthicknessInputAvg.Text = "";

            txtdiameterInput1.Text = "";
            txtdiameterInput2.Text = "";
            txtdiameterInput3.Text = "";
            txtdiameterInput4.Text = "";
            txtdiameterInput5.Text = "";
            txtdiameterInput6.Text = "";
            txtdiameterInputAvg.Text = "";

            // Clear GridView
            gvPressureTest.DataSource = null;
            gvPressureTest.DataBind();


            // Standard Values
            txtSpecificThickness.Text = "";
            txtSpecificDiameter.Text = "";
            txtSpecificcrosssecctionarea.Text = "";
            txtSpecificafterageing.Text = " ";
            txtSpecificbeforeageing.Text = "";
            txtSpecificMedian.Text = "";
            txtCrossSectionarea.Text = "";
            txtloadofmass.Text = "";
            txtbeforeageing.Text = "";
            txtafterkageing.Text = "";

        }
        private void ClearInputFields()
        {

            txtCrossSectionarea.Text = "";
            txtafterkageing.Text = "";
            txtbeforeageing.Text = "";
            txtloadofmass.Text = "";
            txtthicknessInput1.Text = "";
            txtthicknessInput2.Text = "";
            txtthicknessInput3.Text = "";
            txtthicknessInput4.Text = "";
            txtthicknessInput5.Text = "";
            txtthicknessInput6.Text = "";
            txtthicknessInputAvg.Text = "";

            txtdiameterInput1.Text = "";
            txtdiameterInput2.Text = "";
            txtdiameterInput3.Text = "";
            txtdiameterInput4.Text = "";
            txtdiameterInput5.Text = "";
            txtdiameterInput6.Text = "";
            txtdiameterInputAvg.Text = "";

            // txtMedian.Text = "";
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


            //  allTextBoxes.Add(Date);
            allTextBoxes.Add(roomno);
            allTextBoxes.Add(roomhumidity);
            allTextBoxes.Add(roomtemp);

            allTextBoxes.Add(txtthicknessInputAvg);
            allTextBoxes.Add(txtdiameterInputAvg);
            allTextBoxes.Add(txtCrossSectionarea);
            allTextBoxes.Add(txtafterkageing);
            allTextBoxes.Add(txtbeforeageing);
            allTextBoxes.Add(txtloadofmass);


            // Reset border for each TextBox
            foreach (var textBox in allTextBoxes)
            {
                textBox.BorderColor = System.Drawing.Color.Empty; // Default color
                textBox.BorderStyle = BorderStyle.NotSet; // Default style
                textBox.BorderWidth = 0; // Default width
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

                // THICKNESS SPECIMENS
                List<string> thicknessSpecimens = new List<string>();
                foreach (DataRow row in dt.Rows)
                {
                    string core = row["CorePhase"].ToString();
                    int specimen = Convert.ToInt32(row["SpecimenNo"]);
                    string inputs = $"{row["InputthicknessInput1"]},{row["InputthicknessInput2"]}," +
                                    $"{row["InputthicknessInput3"]},{row["InputthicknessInput4"]}," +
                                    $"{row["InputthicknessInput5"]},{row["InputthicknessInput6"]}";
                    thicknessSpecimens.Add($"{core};{specimen}:{inputs}");
                }
                if (thicknessSpecimens.Any())
                {
                    reportParamMapList.Add("14173"); // Thickness
                    resultList.Add(string.Join("|", thicknessSpecimens));
                }

                // DIAMETER SPECIMENS
                List<string> diameterSpecimens = new List<string>();
                foreach (DataRow row in dt.Rows)
                {
                    string core = row["CorePhase"].ToString();
                    int specimen = Convert.ToInt32(row["SpecimenNo"]);
                    string inputs = $"{row["InputdiameterInput1"]},{row["InputdiameterInput2"]}," +
                                    $"{row["InputdiameterInput3"]},{row["InputdiameterInput4"]}," +
                                    $"{row["InputdiameterInput5"]},{row["InputdiameterInput6"]}";
                    diameterSpecimens.Add($"{core};{specimen}:{inputs}");
                }
                if (diameterSpecimens.Any())
                {
                    reportParamMapList.Add("14175"); // Diameter
                    resultList.Add(string.Join("|", diameterSpecimens));
                }
                var thicknessAvgMinCombined = dt.AsEnumerable()
                    .Select(row => $"{row["CorePhase"]};{row["SpecimenNo"]}:{row["InputthicknessInputAvg"]},{row["InputthicknessInputMin"]}")
                    .Where(value => !string.IsNullOrWhiteSpace(value))
                    .ToList();

                if (thicknessAvgMinCombined.Any())
                {
                    reportParamMapList.Add("14174"); // Thickness Avg+Min combined param ID
                    resultList.Add(string.Join("|", thicknessAvgMinCombined));
                }

                var diameterAvgMinCombined = dt.AsEnumerable()
    .Select(row => $"{row["CorePhase"]};{row["SpecimenNo"]}:{row["InputdiameterInputAvg"]},{row["InputdiameterInputMin"]}")
    .Where(value => !string.IsNullOrWhiteSpace(value))
    .ToList();

                if (diameterAvgMinCombined.Any())
                {
                    reportParamMapList.Add("14176"); // Diameter Avg+Min combined param ID
                    resultList.Add(string.Join("|", diameterAvgMinCombined));
                }

                // AFTER TEMP & LOAD
                List<string> beforeageing = new List<string>();
                List<string> afterageing = new List<string>();
                List<string> Median = new List<string>();
                List<string> crosssectionarea = new List<string>();
                List<string> lossOfMass = new List<string>();
                foreach (DataRow row in dt.Rows)
                {
                    string core = row["CorePhase"].ToString();
                    int specimen = Convert.ToInt32(row["SpecimenNo"]);

                    if (!string.IsNullOrWhiteSpace(row["BeforeAgeing"]?.ToString()))
                        beforeageing.Add($"{core};{specimen}:{row["BeforeAgeing"]}");
                    if (!string.IsNullOrWhiteSpace(row["AfterAgeing"]?.ToString()))
                        afterageing.Add($"{core};{specimen}:{row["AfterAgeing"]}");
                    if (!string.IsNullOrWhiteSpace(row["Median"]?.ToString()))
                        Median.Add($"{core};{specimen}:{row["Median"]}");
                    if (!string.IsNullOrWhiteSpace(row["CrossSectionarea"]?.ToString()))
                        crosssectionarea.Add($"{core};{specimen}:{row["CrossSectionarea"]}");
                    if (!string.IsNullOrWhiteSpace(row["LossOfMass"]?.ToString()))
                        lossOfMass.Add($"{core};{specimen}:{row["LossOfMass"]}");

                }
                if (beforeageing.Any())
                {
                    reportParamMapList.Add("14177"); // 
                    resultList.Add(string.Join("|", beforeageing));
                }
                if (afterageing.Any())
                {
                    reportParamMapList.Add("14178"); // AfterTempLoad
                    resultList.Add(string.Join("|", afterageing));
                }
                if (Median.Any())
                {
                    reportParamMapList.Add("14179");
                    resultList.Add(string.Join("|", Median));
                }
                if (crosssectionarea.Any())
                {
                    reportParamMapList.Add("14172");
                    resultList.Add(string.Join("|", crosssectionarea));
                }
                if (lossOfMass.Any())
                {
                    reportParamMapList.Add("14180");
                    resultList.Add(string.Join("|", lossOfMass));
                }
                // Conditioning in desiccators before ageing
                reportParamMapList.Add("14186"); // Conditioningbeforeageingdate
                resultList.Add($"{txtDesicBeforeDateStart.Text};{txtDesicBeforeDateFinish.Text}");

                reportParamMapList.Add("14187"); // ConditioningbeforeageingTime
                resultList.Add($"{txtDesicBeforeTimeStart.Text};{txtDesicBeforeTimeFinish.Text}");

                // Conditioning in desiccators after ageing
                reportParamMapList.Add("14188"); // Conditioningafterageingdate
                resultList.Add($"{txtDesicAfterDateStart.Text};{txtDesicAfterDateFinish.Text}");

                reportParamMapList.Add("14189"); // ConditioningafterageingTime
                resultList.Add($"{txtDesicAfterTimeStart.Text};{txtDesicAfterTimeFinish.Text}");

                // Ageing in air oven
                reportParamMapList.Add("14190"); // Airflow
                resultList.Add(txtAirflow.Text);

                reportParamMapList.Add("14191"); // OvenTemperature
                resultList.Add(txtOvenTemp.Text);

                reportParamMapList.Add("14192"); // Ageing in air oven date
                resultList.Add($"{txtAirOvenDateStart.Text};{txtAirOvenDateFinish.Text}");

                reportParamMapList.Add("14193"); // Ageing in air oven time
                resultList.Add($"{txtAirOvenTimeStart.Text};{txtAirOvenTimeFinish.Text}");


                reportParamMapList.Add("14181"); resultList.Add(txtSpecificThickness.Text);        // SpecifiedThickness
                reportParamMapList.Add("14182"); resultList.Add(txtSpecificDiameter.Text);         // SpecifiedDiameter
                reportParamMapList.Add("14194"); resultList.Add(txtSpecificcrosssecctionarea.Text); // SpecifiedWeightload (no ID provided)
                reportParamMapList.Add("14183"); resultList.Add(txtSpecificbeforeageing.Text);     // SpecifiedBeforeAgeing
                reportParamMapList.Add("14184"); resultList.Add(txtSpecificafterageing.Text);      // SpecifiedAfterAgeing
                reportParamMapList.Add("14185"); resultList.Add(txtSpecificMedian.Text);

                reportParamMapList.Add("14160"); resultList.Add(cableType.Text);         // Type
                reportParamMapList.Add("14161"); resultList.Add(cableSize.Text);         // Size
                reportParamMapList.Add("14162"); resultList.Add(voltage.Text);           // Voltage
                reportParamMapList.Add("14163"); resultList.Add(Date.Text);              // Date
                reportParamMapList.Add("14164"); resultList.Add(roomno.Text);            // RoomNo
                reportParamMapList.Add("14165"); resultList.Add(roomtemp.Text);          // RoomTemp
                reportParamMapList.Add("14166"); resultList.Add(refSpec.Text);           // RefSpec
                reportParamMapList.Add("14167"); resultList.Add(testMethodNo.Text);      // TestMethodNo
                reportParamMapList.Add("14168"); resultList.Add(jobOrderNo.Text);        // JobOrderNo
                reportParamMapList.Add("14169"); resultList.Add(worksheetno.Text);       // WorkSheetNo
                reportParamMapList.Add("14170"); resultList.Add(roomhumidity.Text);      // RoomHumidity
                reportParamMapList.Add("14171"); resultList.Add(equipmentno.Text);

                // SUBMIT
                int modified = _cm.AddTestResultPTQualis(_cm,
                                    reportParamMapList.ToArray(),
                                    resultList.ToArray());

                if (modified > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showSuccess", "alert('Test results and Report Generated successfully!');", true);
                    _cm.sampleStatusID = 27;
                    _cm.UpdateCRFSampleDetailsStatus(_cm);
                    _cm.StatusID = 27;
                    _cm.UpdateAllocationStatusid_mechanical(_cm);
                    Response.Redirect($"GenerateTestReport_LossOfMass_Rev02.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
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

            bool gridHasData = (gvPressureTest.Rows.Count > 0);


            // Helper function to validate inputs allowing "-" as zero
            bool IsValidNumberOrDash(string input)
            {
                if (input.Trim() == "-") return true;
                return double.TryParse(input, out _);
            }

            // General validations
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
                errors.Add("Room Humidity must be a valid number.");
                invalidControls.Add(roomhumidity);
            }

            if (string.IsNullOrEmpty(roomtemp.Text) || !IsValidNumberOrDash(roomtemp.Text))
            {
                errors.Add("Room Temp. must be a valid number.");
                invalidControls.Add(roomtemp);
            }


            if (!gridHasData)
            {
                if (ddlCorePhase.SelectedIndex == -1)
                {
                    errors.Add("Core Phase is required.");
                    invalidControls.Add(ddlCorePhase);
                }

                if (string.IsNullOrWhiteSpace(txtSpecimenNo.Text) || !int.TryParse(txtSpecimenNo.Text.Trim(), out _))
                {
                    errors.Add("Specimen No. must be a valid integer.");
                    invalidControls.Add(txtSpecimenNo);
                }

                // Validate your new input textboxes here (thickness and diameter)
                TextBox[] newTestInputs = {
            txtthicknessInput1, txtthicknessInput2, txtthicknessInput3, txtthicknessInput4, txtthicknessInput5, txtthicknessInput6,
            txtdiameterInput1, txtdiameterInput2, txtdiameterInput3, txtdiameterInput4, txtdiameterInput5, txtdiameterInput6,
            txtthicknessInputAvg, txtdiameterInputAvg,
            txtCrossSectionarea, txtbeforeageing, txtafterkageing,txtloadofmass
        };

                foreach (var txt in newTestInputs)
                {
                    if (string.IsNullOrWhiteSpace(txt.Text) || !IsValidNumberOrDash(txt.Text))
                    {
                        errors.Add($"{txt.ID.Replace("txt", "")} must be a valid number or '-'.");
                        invalidControls.Add(txt);
                    }
                }
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
                    else if (control is DropDownList ddl)
                    {
                        ddl.CssClass += " input-error";
                    }
                }

                string errorMessage = "Validation Errors:\\n" + string.Join("\\n• ", errors.Select(err => err.Replace("'", "\\'").Replace("\n", "").Replace("\r", "")));

                ScriptManager.RegisterStartupScript(this, GetType(), "ValidationError",
                    $"alert('{errorMessage}');", true);

                return; // stop further processing
            }

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
        protected void gvPressureTest_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                GridView gv = (GridView)sender;
                e.Row.Controls.Clear();

              
                GridViewRow row1 = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal);
                row1.Cells.Add(CreateHeaderCell("Core Phase", 3, 1));
                row1.Cells.Add(CreateHeaderCell("Specimen No.", 3, 1));
                row1.Cells.Add(CreateHeaderCell("Thickness/Weight", 1, 7));
                row1.Cells.Add(CreateHeaderCell("Width/Diameter/Length/Density", 1, 7));
                row1.Cells.Add(CreateHeaderCell("Cross sectional area", 2, 1)); 
                row1.Cells.Add(CreateHeaderCell("Weight", 0, 2)); 
                row1.Cells.Add(CreateHeaderCell("Loss of mass", 2, 1)); 
                row1.Cells.Add(CreateHeaderCell("Median",2, 1)); 
                row1.Cells.Add(CreateHeaderCell("Action", 3, 1));
                gv.Controls[0].Controls.AddAt(0, row1);

              
                GridViewRow row2 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Normal);
                for (int i = 1; i <= 6; i++)
                    row2.Cells.Add(CreateHeaderCell(i.ToString(), 1, 1));
                row2.Cells.Add(CreateHeaderCell("Avg.<br>min.", 1, 1));
                for (int i = 1; i <= 6; i++)
                    row2.Cells.Add(CreateHeaderCell(i.ToString(), 1, 1));
                row2.Cells.Add(CreateHeaderCell("Avg.<br>min.", 1, 1));
          
                row2.Cells.Add(CreateHeaderCell("Before ageing", 1, 1));
                row2.Cells.Add(CreateHeaderCell("After ageing", 1, 1));
                gv.Controls[0].Controls.AddAt(1, row2);

                // Third header row - units
                GridViewRow row3 = new GridViewRow(2, 0, DataControlRowType.Header, DataControlRowState.Normal);
                row3.Cells.Add(CreateHeaderCell("mm/gr", 1, 7));
                row3.Cells.Add(CreateHeaderCell("mm/[gr/cm²]", 1, 7));
                row3.Cells.Add(CreateHeaderCell("cm²", 1, 1)); 
                row3.Cells.Add(CreateHeaderCell("mg", 1, 2));
                row3.Cells.Add(CreateHeaderCell("mg/cm²", 1, 2)); 
                gv.Controls[0].Controls.AddAt(2, row3);
            }
        }

        private TableHeaderCell CreateHeaderCell(string text, int rowSpan, int colSpan)
        {
            TableHeaderCell cell = new TableHeaderCell
            {
                Text = text,
                RowSpan = rowSpan,
                ColumnSpan = colSpan,
                HorizontalAlign = HorizontalAlign.Center,
                VerticalAlign = VerticalAlign.Middle
            };
            return cell;
        }
    }
}