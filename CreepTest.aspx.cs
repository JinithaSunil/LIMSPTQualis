using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LIMS
{
	public partial class CreepTest : System.Web.UI.Page
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
                        PrefillTestResultsGrid(existingData);

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

            //Upper section
           


            // Method to prefill upper section details based on parameter IDs
            SetTextBoxText(cableType, "12997");
            SetTextBoxText(cableSize, "12998");
            SetTextBoxText(voltage, "12999");
            SetTextBoxText(Date, "13000");
            SetTextBoxText(roomno, "13001");
            SetTextBoxText(roomtemp, "13002");
            SetTextBoxText(refSpec, "13003");
            SetTextBoxText(testMethodNo, "13004");
            SetTextBoxText(jobOrderNo, "13006");
            SetTextBoxText(worksheetno, "13007");           
            SetTextBoxText(roomhumidity, "13007");                     
            SetTextBoxText(equipmentno, "13008");

            SetTextBoxText(txtappliedload,"12993");
            SetTextBoxText(txtgaugelength,"12996");

            SetTextBoxText(txtStarttime, "13031");
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


        private void PrefillTestResultsGrid(DataTable dt)
        {
            try
            {
                string[] dates = null, times = null, duration = null, averages = null, measurements = null;

                // Debug: Check if DataTable has data
                if (dt == null || dt.Rows.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showError",
                        "alert('No data found in DataTable');", true);
                    return;
                }

                // Debug: Check what columns are actually available
                string columnNames = string.Join(", ", dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
                System.Diagnostics.Debug.WriteLine($"Available columns: {columnNames}");

                // Process each row to extract different parameter types
                foreach (DataRow row in dt.Rows)
                {
                    string paramId = row["ParameterID"]?.ToString() ?? "";  // Changed from "ReportParamMapID" to "ParameterID"
                    string value = row["Result"]?.ToString() ?? "";

                    // Debug: Log what we're processing
                    System.Diagnostics.Debug.WriteLine($"Processing ParamID: {paramId}, Value: {value}");

                    if (string.IsNullOrEmpty(paramId) || string.IsNullOrEmpty(value))
                        continue;

                    switch (paramId)
                    {
                        case "12989": // Dates
                            dates = value.Split(',');
                            System.Diagnostics.Debug.WriteLine($"Dates array length: {dates.Length}");
                            break;
                        case "12988": // Duration
                            duration = value.Split(',');
                            System.Diagnostics.Debug.WriteLine($"Duration array length: {duration.Length}");
                            break;
                        case "12990": // Times
                            times = value.Split(',');
                            System.Diagnostics.Debug.WriteLine($"Times array length: {times.Length}");
                            break;
                        case "12992": // Averages
                            averages = value.Split(',');
                            System.Diagnostics.Debug.WriteLine($"Averages array length: {averages.Length}");
                            break;
                        case "12991": // Measurements
                            measurements = value.Split(',');
                            System.Diagnostics.Debug.WriteLine($"Measurements array length: {measurements.Length}");
                            break;
                    }
                }

                // Check if we got all required arrays
                if (dates == null) System.Diagnostics.Debug.WriteLine("Dates array is null");
                if (times == null) System.Diagnostics.Debug.WriteLine("Times array is null");
                if (duration == null) System.Diagnostics.Debug.WriteLine("Duration array is null");
                if (averages == null) System.Diagnostics.Debug.WriteLine("Averages array is null");
                if (measurements == null) System.Diagnostics.Debug.WriteLine("Measurements array is null");

                // Populate controls using direct assignment (more reliable than FindControl)
                for (int i = 0; i < 22; i++)
                {
                    int rowNum = i + 1;

                    // Process date
                    string dateVal = (dates != null && i < dates.Length) ? dates[i].Trim() : "";
                    if (!string.IsNullOrEmpty(dateVal) && DateTime.TryParse(dateVal, out DateTime parsedDate))
                        dateVal = parsedDate.ToString("yyyy-MM-dd");

                    // Process other values
                    string durVal = (duration != null && i < duration.Length) ? duration[i].Trim() : "";
                    string timeVal = (times != null && i < times.Length) ? times[i].Trim() : "";
                    string avgVal = (averages != null && i < averages.Length) ? averages[i].Trim() : "";

                    // Process measurements (split by |)
                    string ext1 = "", ext2 = "";
                    if (measurements != null && i < measurements.Length && !string.IsNullOrEmpty(measurements[i]))
                    {
                        var parts = measurements[i].Split('|');
                        if (parts.Length > 0) ext1 = parts[0].Trim();
                        if (parts.Length > 1) ext2 = parts[1].Trim();
                    }

                    // Use recursive FindControl method to search nested controls
                    TextBox txtDate = FindControlRecursive(this, $"txtDateOfTest{rowNum}") as TextBox;
                    TextBox txtDur = FindControlRecursive(this, $"txtDuration{rowNum}") as TextBox;
                    TextBox txtTime = FindControlRecursive(this, $"txtTime{rowNum}") as TextBox;
                    TextBox txtAvg = FindControlRecursive(this, $"txtavg_{rowNum}") as TextBox;
                    TextBox txtExt1 = FindControlRecursive(this, $"txtextension1_{rowNum}") as TextBox;
                    TextBox txtExt2 = FindControlRecursive(this, $"txtextension2_{rowNum}") as TextBox;

                    // Debug: Check if controls were found
                    System.Diagnostics.Debug.WriteLine($"Row {rowNum} controls found - Date: {txtDate != null}, Dur: {txtDur != null}, Time: {txtTime != null}, Avg: {txtAvg != null}, Ext1: {txtExt1 != null}, Ext2: {txtExt2 != null}");

                    // Populate controls if they exist
                    if (txtDate != null) txtDate.Text = dateVal;
                    if (txtDur != null)
                    {
                        txtDur.ReadOnly = false;  // Temporarily disable ReadOnly
                        txtDur.Text = durVal;
                        txtDur.ReadOnly = true;   // Re-enable ReadOnly
                    }
                    if (txtTime != null) txtTime.Text = timeVal;
                    if (txtAvg != null) txtAvg.Text = avgVal;
                    if (txtExt1 != null) txtExt1.Text = ext1;
                    if (txtExt2 != null) txtExt2.Text = ext2;

                    // Debug: Log what we're setting
                    System.Diagnostics.Debug.WriteLine($"Row {rowNum}: Date={dateVal}, Duration={durVal}, Time={timeVal}, Avg={avgVal}, Ext1={ext1}, Ext2={ext2}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in PrefillTestResultsGrid: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "showError",
                    $"alert('Error loading data: {ex.Message}');", true);
            }
        }

        // Helper method to recursively search for controls
        private Control FindControlRecursive(Control rootControl, string controlID)
        {
            if (rootControl.ID == controlID)
                return rootControl;

            foreach (Control control in rootControl.Controls)
            {
                Control foundControl = FindControlRecursive(control, controlID);
                if (foundControl != null)
                    return foundControl;
            }

            return null;
        }
        protected void Extension_TextChanged(object sender, EventArgs e)
        {
            TextBox currentBox = (TextBox)sender;
            string rowNumber = currentBox.ID.Split('_')[1];

            // Get the other textboxes in same row
            TextBox ext1 = (TextBox)currentBox.Parent.Parent.FindControl("txtextension1_" + rowNumber);
            TextBox ext2 = (TextBox)currentBox.Parent.Parent.FindControl("txtextension2_" + rowNumber);
            TextBox avg = (TextBox)currentBox.Parent.Parent.FindControl("txtavg_" + rowNumber);

            // Handle "-" as 0
            string ext1Text = ext1.Text.Trim() == "-" ? "0" : ext1.Text.Trim();
            string ext2Text = ext2.Text.Trim() == "-" ? "0" : ext2.Text.Trim();

            // Calculate if both have values
            double val1, val2;
            if (double.TryParse(ext1Text, out val1) && double.TryParse(ext2Text, out val2))
            {
                avg.Text = ((val1 + val2) / 2).ToString("F2");
            }
            else
            {
                avg.Text = ""; // Clear average if invalid input
            }
        }

        protected void txtStarttime_TextChanged(object sender, EventArgs e)
        {
          
            DateTime startTime;
            string input = txtStarttime.Text.Trim();

          
            if (Regex.IsMatch(input, @"^\d{1,2}$"))
            {
                input += ":00";
            }

            if (!DateTime.TryParseExact(input, "H:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out startTime) &&
                !DateTime.TryParseExact(input, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out startTime))
            {
              
                return;
            }

            txtTime1.Text = startTime.AddMinutes(1).ToString("HH:mm");
            txtTime2.Text = startTime.AddMinutes(2).ToString("HH:mm");
            txtTime3.Text = startTime.AddMinutes(3).ToString("HH:mm");
            txtTime4.Text = startTime.AddMinutes(4).ToString("HH:mm");
            txtTime5.Text = startTime.AddMinutes(5).ToString("HH:mm");
            txtTime6.Text = startTime.AddMinutes(10).ToString("HH:mm");
            txtTime7.Text = startTime.AddMinutes(15).ToString("HH:mm");
            txtTime8.Text = startTime.AddMinutes(30).ToString("HH:mm");
            txtTime9.Text = startTime.AddHours(1).ToString("HH:mm");
            txtTime10.Text = startTime.AddHours(2).ToString("HH:mm");
            txtTime11.Text = startTime.AddHours(3).ToString("HH:mm");
            txtTime12.Text = startTime.AddHours(4).ToString("HH:mm");
            txtTime13.Text = startTime.AddHours(5).ToString("HH:mm");
            txtTime14.Text = startTime.AddHours(6).ToString("HH:mm");
            txtTime15.Text = startTime.AddHours(7).ToString("HH:mm");
            txtTime16.Text = startTime.AddHours(24).ToString("HH:mm");
            txtTime17.Text = startTime.AddHours(50).ToString("HH:mm");
            txtTime18.Text = startTime.AddHours(80).ToString("HH:mm");
            txtTime19.Text = startTime.AddHours(150).ToString("HH:mm");
            txtTime20.Text = startTime.AddHours(300).ToString("HH:mm");
            txtTime21.Text = startTime.AddHours(600).ToString("HH:mm");
            txtTime22.Text = startTime.AddHours(1000).ToString("HH:mm");
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

            // Reset border for each TextBox
            foreach (var textBox in allTextBoxes)
            {
                textBox.BorderColor = System.Drawing.Color.Empty; // Default color
                textBox.BorderStyle = BorderStyle.NotSet; // Default style
                textBox.BorderWidth = 0; // Default width
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
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
        private bool Submitform()
        {
            try
            {
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

                List<string> reportParamMapList = new List<string>();
                List<string> resultList = new List<string>();

                List<string> dates = new List<string>();
                List<string> duration = new List<string>();
                List<string> times = new List<string>();
                List<string> averages = new List<string>();
                List<string> measurements = new List<string>();

                // Row 1
                if (!string.IsNullOrWhiteSpace(txtDateOfTest1.Text + txtTime1.Text + txtavg_1.Text + txtextension1_1.Text + txtextension2_1.Text))
                {
                    dates.Add(txtDateOfTest1.Text.Trim());
                    duration.Add(txtDuration1.Text.Trim());
                    times.Add(txtTime1.Text.Trim());
                    averages.Add(txtavg_1.Text.Trim());
                    measurements.Add($"{txtextension1_1.Text.Trim()}|{txtextension2_1.Text.Trim()}");
                }

                // Row 2
                if (!string.IsNullOrWhiteSpace(txtDateOfTest2.Text + txtTime2.Text + txtavg_2.Text + txtextension1_2.Text + txtextension2_2.Text))
                {
                    dates.Add(txtDateOfTest2.Text.Trim());
                    duration.Add(txtDuration2.Text.Trim());
                    times.Add(txtTime2.Text.Trim());
                    averages.Add(txtavg_2.Text.Trim());
                    measurements.Add($"{txtextension1_2.Text.Trim()}|{txtextension2_2.Text.Trim()}");
                }

                // Row 3
                if (!string.IsNullOrWhiteSpace(txtDateOfTest3.Text + txtTime3.Text + txtavg_3.Text + txtextension1_3.Text + txtextension2_3.Text))
                {
                    dates.Add(txtDateOfTest3.Text.Trim());
                    duration.Add(txtDuration3.Text.Trim());
                    times.Add(txtTime3.Text.Trim());
                    averages.Add(txtavg_3.Text.Trim());
                    measurements.Add($"{txtextension1_3.Text.Trim()}|{txtextension2_3.Text.Trim()}");
                }

                // Row 4
                if (!string.IsNullOrWhiteSpace(txtDateOfTest4.Text + txtTime4.Text + txtavg_4.Text + txtextension1_4.Text + txtextension2_4.Text))
                {
                    dates.Add(txtDateOfTest4.Text.Trim());
                    duration.Add(txtDuration4.Text.Trim());
                    times.Add(txtTime4.Text.Trim());
                    averages.Add(txtavg_4.Text.Trim());
                    measurements.Add($"{txtextension1_4.Text.Trim()}|{txtextension2_4.Text.Trim()}");
                }

                // Row 5
                if (!string.IsNullOrWhiteSpace(txtDateOfTest5.Text + txtTime5.Text + txtavg_5.Text + txtextension1_5.Text + txtextension2_5.Text))
                {
                    dates.Add(txtDateOfTest5.Text.Trim());
                    duration.Add(txtDuration5.Text.Trim());
                    times.Add(txtTime5.Text.Trim());
                    averages.Add(txtavg_5.Text.Trim());
                    measurements.Add($"{txtextension1_5.Text.Trim()}|{txtextension2_5.Text.Trim()}");
                }

                // Row 6
                if (!string.IsNullOrWhiteSpace(txtDateOfTest6.Text + txtTime6.Text + txtavg_6.Text + txtextension1_6.Text + txtextension2_6.Text))
                {
                    dates.Add(txtDateOfTest6.Text.Trim());
                    duration.Add(txtDuration6.Text.Trim());
                    times.Add(txtTime6.Text.Trim());
                    averages.Add(txtavg_6.Text.Trim());
                    measurements.Add($"{txtextension1_6.Text.Trim()}|{txtextension2_6.Text.Trim()}");
                }

                // Row 7
                if (!string.IsNullOrWhiteSpace(txtDateOfTest7.Text + txtTime7.Text + txtavg_7.Text + txtextension1_7.Text + txtextension2_7.Text))
                {
                    dates.Add(txtDateOfTest7.Text.Trim());
                    duration.Add(txtDuration7.Text.Trim());
                    times.Add(txtTime7.Text.Trim());
                    averages.Add(txtavg_7.Text.Trim());
                    measurements.Add($"{txtextension1_7.Text.Trim()}|{txtextension2_7.Text.Trim()}");
                }

                // Row 8
                if (!string.IsNullOrWhiteSpace(txtDateOfTest8.Text + txtTime8.Text + txtavg_8.Text + txtextension1_8.Text + txtextension2_8.Text))
                {
                    dates.Add(txtDateOfTest8.Text.Trim());
                    duration.Add(txtDuration8.Text.Trim());
                    times.Add(txtTime8.Text.Trim());
                    averages.Add(txtavg_8.Text.Trim());
                    measurements.Add($"{txtextension1_8.Text.Trim()}|{txtextension2_8.Text.Trim()}");
                }

                // Row 9
                if (!string.IsNullOrWhiteSpace(txtDateOfTest9.Text + txtTime9.Text + txtavg_9.Text + txtextension1_9.Text + txtextension2_9.Text))
                {
                    dates.Add(txtDateOfTest9.Text.Trim());
                    duration.Add(txtDuration9.Text.Trim());
                    times.Add(txtTime9.Text.Trim());
                    averages.Add(txtavg_9.Text.Trim());
                    measurements.Add($"{txtextension1_9.Text.Trim()}|{txtextension2_9.Text.Trim()}");
                }

                // Row 10
                if (!string.IsNullOrWhiteSpace(txtDateOfTest10.Text + txtTime10.Text + txtavg_10.Text + txtextension1_10.Text + txtextension2_10.Text))
                {
                    dates.Add(txtDateOfTest10.Text.Trim());
                    duration.Add(txtDuration10.Text.Trim());
                    times.Add(txtTime10.Text.Trim());
                    averages.Add(txtavg_10.Text.Trim());
                    measurements.Add($"{txtextension1_10.Text.Trim()}|{txtextension2_10.Text.Trim()}");
                }

                // Row 11
                if (!string.IsNullOrWhiteSpace(txtDateOfTest11.Text + txtTime11.Text + txtavg_11.Text + txtextension1_11.Text + txtextension2_11.Text))
                {
                    dates.Add(txtDateOfTest11.Text.Trim());
                    duration.Add(txtDuration11.Text.Trim());
                    times.Add(txtTime11.Text.Trim());
                    averages.Add(txtavg_11.Text.Trim());
                    measurements.Add($"{txtextension1_11.Text.Trim()}|{txtextension2_11.Text.Trim()}");
                }

                // Row 12
                if (!string.IsNullOrWhiteSpace(txtDateOfTest12.Text + txtTime12.Text + txtavg_12.Text + txtextension1_12.Text + txtextension2_12.Text))
                {
                    dates.Add(txtDateOfTest12.Text.Trim());
                    duration.Add(txtDuration12.Text.Trim());
                    times.Add(txtTime12.Text.Trim());
                    averages.Add(txtavg_12.Text.Trim());
                    measurements.Add($"{txtextension1_12.Text.Trim()}|{txtextension2_12.Text.Trim()}");
                }

                // Row 13
                if (!string.IsNullOrWhiteSpace(txtDateOfTest13.Text + txtTime13.Text + txtavg_13.Text + txtextension1_13.Text + txtextension2_13.Text))
                {
                    dates.Add(txtDateOfTest13.Text.Trim());
                    duration.Add(txtDuration13.Text.Trim());
                    times.Add(txtTime13.Text.Trim());
                    averages.Add(txtavg_13.Text.Trim());
                    measurements.Add($"{txtextension1_13.Text.Trim()}|{txtextension2_13.Text.Trim()}");
                }

                // Row 14
                if (!string.IsNullOrWhiteSpace(txtDateOfTest14.Text + txtTime14.Text + txtavg_14.Text + txtextension1_14.Text + txtextension2_14.Text))
                {
                    dates.Add(txtDateOfTest14.Text.Trim());
                    duration.Add(txtDuration14.Text.Trim());
                    times.Add(txtTime14.Text.Trim());
                    averages.Add(txtavg_14.Text.Trim());
                    measurements.Add($"{txtextension1_14.Text.Trim()}|{txtextension2_14.Text.Trim()}");
                }

                // Row 15
                if (!string.IsNullOrWhiteSpace(txtDateOfTest15.Text + txtTime15.Text + txtavg_15.Text + txtextension1_15.Text + txtextension2_15.Text))
                {
                    dates.Add(txtDateOfTest15.Text.Trim());
                    duration.Add(txtDuration15.Text.Trim());
                    times.Add(txtTime15.Text.Trim());
                    averages.Add(txtavg_15.Text.Trim());
                    measurements.Add($"{txtextension1_15.Text.Trim()}|{txtextension2_15.Text.Trim()}");
                }

                // Row 16
                if (!string.IsNullOrWhiteSpace(txtDateOfTest16.Text + txtTime16.Text + txtavg_16.Text + txtextension1_16.Text + txtextension2_16.Text))
                {
                    dates.Add(txtDateOfTest16.Text.Trim());
                    duration.Add(txtDuration16.Text.Trim());
                    times.Add(txtTime16.Text.Trim());
                    averages.Add(txtavg_16.Text.Trim());
                    measurements.Add($"{txtextension1_16.Text.Trim()}|{txtextension2_16.Text.Trim()}");
                }

                // Row 17
                if (!string.IsNullOrWhiteSpace(txtDateOfTest17.Text + txtTime17.Text + txtavg_17.Text + txtextension1_17.Text + txtextension2_17.Text))
                {
                    dates.Add(txtDateOfTest17.Text.Trim());
                    duration.Add(txtDuration17.Text.Trim());
                    times.Add(txtTime17.Text.Trim());
                    averages.Add(txtavg_17.Text.Trim());
                    measurements.Add($"{txtextension1_17.Text.Trim()}|{txtextension2_17.Text.Trim()}");
                }

                // Row 18
                if (!string.IsNullOrWhiteSpace(txtDateOfTest18.Text + txtTime18.Text + txtavg_18.Text + txtextension1_18.Text + txtextension2_18.Text))
                {
                    dates.Add(txtDateOfTest18.Text.Trim());
                    duration.Add(txtDuration18.Text.Trim());
                    times.Add(txtTime18.Text.Trim());
                    averages.Add(txtavg_18.Text.Trim());
                    measurements.Add($"{txtextension1_18.Text.Trim()}|{txtextension2_18.Text.Trim()}");
                }

                // Row 19
                if (!string.IsNullOrWhiteSpace(txtDateOfTest19.Text + txtTime19.Text + txtavg_19.Text + txtextension1_19.Text + txtextension2_19.Text))
                {
                    dates.Add(txtDateOfTest19.Text.Trim());
                    duration.Add(txtDuration19.Text.Trim());
                    times.Add(txtTime19.Text.Trim());
                    averages.Add(txtavg_19.Text.Trim());
                    measurements.Add($"{txtextension1_19.Text.Trim()}|{txtextension2_19.Text.Trim()}");
                }

                // Row 20
                if (!string.IsNullOrWhiteSpace(txtDateOfTest20.Text + txtTime20.Text + txtavg_20.Text + txtextension1_20.Text + txtextension2_20.Text))
                {
                    dates.Add(txtDateOfTest20.Text.Trim());
                    duration.Add(txtDuration20.Text.Trim());
                    times.Add(txtTime20.Text.Trim());
                    averages.Add(txtavg_20.Text.Trim());
                    measurements.Add($"{txtextension1_20.Text.Trim()}|{txtextension2_20.Text.Trim()}");
                }

                // Row 21
                if (!string.IsNullOrWhiteSpace(txtDateOfTest21.Text + txtTime21.Text + txtavg_21.Text + txtextension1_21.Text + txtextension2_21.Text))
                {
                    dates.Add(txtDateOfTest21.Text.Trim());
                    duration.Add(txtDuration21.Text.Trim());
                    times.Add(txtTime21.Text.Trim());
                    averages.Add(txtavg_21.Text.Trim());
                    measurements.Add($"{txtextension1_21.Text.Trim()}|{txtextension2_21.Text.Trim()}");
                }

                // Row 22
                if (!string.IsNullOrWhiteSpace(txtDateOfTest22.Text + txtTime22.Text + txtavg_22.Text + txtextension1_22.Text + txtextension2_22.Text))
                {
                    dates.Add(txtDateOfTest22.Text.Trim());
                    duration.Add(txtDuration22.Text.Trim());
                    times.Add(txtTime22.Text.Trim());
                    averages.Add(txtavg_22.Text.Trim());
                    measurements.Add($"{txtextension1_22.Text.Trim()}|{txtextension2_22.Text.Trim()}");
                }

                // Convert to comma-separated strings for saving
                string allDates = string.Join(",", dates);
                string allDuration = string.Join(",", duration);
                string allTimes = string.Join(",", times);
                string allAverages = string.Join(",", averages);
                string allMeasurements = string.Join(",", measurements);

                reportParamMapList.Add("12989"); resultList.Add(allDates);
                reportParamMapList.Add("12988"); resultList.Add(allDuration);
                reportParamMapList.Add("12990"); resultList.Add(allTimes);
                reportParamMapList.Add("12992"); resultList.Add(allAverages);
                reportParamMapList.Add("12991"); resultList.Add(allMeasurements);

                //Upper section
                reportParamMapList.Add("12997"); resultList.Add(cableType.Text);
                reportParamMapList.Add("12998"); resultList.Add(cableSize.Text);
                reportParamMapList.Add("12999"); resultList.Add(voltage.Text);
                reportParamMapList.Add("13000"); resultList.Add(Date.Text);
                reportParamMapList.Add("13001"); resultList.Add(roomno.Text);
                reportParamMapList.Add("13002"); resultList.Add(roomtemp.Text);
                reportParamMapList.Add("13003"); resultList.Add(refSpec.Text);
                reportParamMapList.Add("13004"); resultList.Add(testMethodNo.Text);
                reportParamMapList.Add("13005"); resultList.Add(jobOrderNo.Text);
                reportParamMapList.Add("13006"); resultList.Add(worksheetno.Text);
                reportParamMapList.Add("13007"); resultList.Add(roomhumidity.Text);
                reportParamMapList.Add("13008"); resultList.Add(equipmentno.Text);


                reportParamMapList.Add("13031"); resultList.Add(txtStarttime.Text);
                reportParamMapList.Add("12993"); resultList.Add(txtappliedload.Text);
                reportParamMapList.Add("12996"); resultList.Add(txtgaugelength.Text);

                // Convert to arrays and submit
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

                    Response.Redirect($"GenerateTestRecord_CreepTest.aspx?s={_cm.sampleId}&r={_cm.ReportID}", false);
                    Context.ApplicationInstance.CompleteRequest();

                    return true;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showError",
                        "alert('Failed to save data.');", true);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "showError",
                    $"alert('Error: {ex.Message}');", true);
                return false;
            }
        }
        private void ShowMessage(string message)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + message + "');", true);
        }
        protected void btnClearData_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
        protected void ClearForm()
        {
            txtStarttime.Text = "";

            // Row 1
            txtDateOfTest1.Text = "";
            txtDuration1.Text = "";
            txtTime1.Text = "";
            txtextension1_1.Text = "";
            txtextension2_1.Text = "";
            txtavg_1.Text = "";

            // Row 2
            txtDateOfTest2.Text = "";
            txtDuration2.Text = "";
            txtTime2.Text = "";
            txtextension1_2.Text = "";
            txtextension2_2.Text = "";
            txtavg_2.Text = "";

            // Row 3
            txtDateOfTest3.Text = "";
            txtDuration3.Text = "";
            txtTime3.Text = "";
            txtextension1_3.Text = "";
            txtextension2_3.Text = "";
            txtavg_3.Text = "";

            // Row 4
            txtDateOfTest4.Text = "";
            txtDuration4.Text = "";
            txtTime4.Text = "";
            txtextension1_4.Text = "";
            txtextension2_4.Text = "";
            txtavg_4.Text = "";

            // Row 5
            txtDateOfTest5.Text = "";
            txtDuration5.Text = "";
            txtTime5.Text = "";
            txtextension1_5.Text = "";
            txtextension2_5.Text = "";
            txtavg_5.Text = "";

            // Row 6
            txtDateOfTest6.Text = "";
            txtDuration6.Text = "";
            txtTime6.Text = "";
            txtextension1_6.Text = "";
            txtextension2_6.Text = "";
            txtavg_6.Text = "";

            // Row 7
            txtDateOfTest7.Text = "";
            txtDuration7.Text = "";
            txtTime7.Text = "";
            txtextension1_7.Text = "";
            txtextension2_7.Text = "";
            txtavg_7.Text = "";

            // Row 8
            txtDateOfTest8.Text = "";
            txtDuration8.Text = "";
            txtTime8.Text = "";
            txtextension1_8.Text = "";
            txtextension2_8.Text = "";
            txtavg_8.Text = "";

            // Row 9
            txtDateOfTest9.Text = "";
            txtDuration9.Text = "";
            txtTime9.Text = "";
            txtextension1_9.Text = "";
            txtextension2_9.Text = "";
            txtavg_9.Text = "";

            // Row 10
            txtDateOfTest10.Text = "";
            txtDuration10.Text = "";
            txtTime10.Text = "";
            txtextension1_10.Text = "";
            txtextension2_10.Text = "";
            txtavg_10.Text = "";

            // Row 11
            txtDateOfTest11.Text = "";
            txtDuration11.Text = "";
            txtTime11.Text = "";
            txtextension1_11.Text = "";
            txtextension2_11.Text = "";
            txtavg_11.Text = "";

            // Row 12
            txtDateOfTest12.Text = "";
            txtDuration12.Text = "";
            txtTime12.Text = "";
            txtextension1_12.Text = "";
            txtextension2_12.Text = "";
            txtavg_12.Text = "";

            // Row 13
            txtDateOfTest13.Text = "";
            txtDuration13.Text = "";
            txtTime13.Text = "";
            txtextension1_13.Text = "";
            txtextension2_13.Text = "";
            txtavg_13.Text = "";

            // Row 14
            txtDateOfTest14.Text = "";
            txtDuration14.Text = "";
            txtTime14.Text = "";
            txtextension1_14.Text = "";
            txtextension2_14.Text = "";
            txtavg_14.Text = "";

            // Row 15
            txtDateOfTest15.Text = "";
            txtDuration15.Text = "";
            txtTime15.Text = "";
            txtextension1_15.Text = "";
            txtextension2_15.Text = "";
            txtavg_15.Text = "";

            // Row 16
            txtDateOfTest16.Text = "";
            txtDuration16.Text = "";
            txtTime16.Text = "";
            txtextension1_16.Text = "";
            txtextension2_16.Text = "";
            txtavg_16.Text = "";

            // Row 17
            txtDateOfTest17.Text = "";
            txtDuration17.Text = "";
            txtTime17.Text = "";
            txtextension1_17.Text = "";
            txtextension2_17.Text = "";
            txtavg_17.Text = "";

            // Row 18
            txtDateOfTest18.Text = "";
            txtDuration18.Text = "";
            txtTime18.Text = "";
            txtextension1_18.Text = "";
            txtextension2_18.Text = "";
            txtavg_18.Text = "";

            // Row 19
            txtDateOfTest19.Text = "";
            txtDuration19.Text = "";
            txtTime19.Text = "";
            txtextension1_19.Text = "";
            txtextension2_19.Text = "";
            txtavg_19.Text = "";

            // Row 20
            txtDateOfTest20.Text = "";
            txtDuration20.Text = "";
            txtTime20.Text = "";
            txtextension1_20.Text = "";
            txtextension2_20.Text = "";
            txtavg_20.Text = "";

            // Row 21
            txtDateOfTest21.Text = "";
            txtDuration21.Text = "";
            txtTime21.Text = "";
            txtextension1_21.Text = "";
            txtextension2_21.Text = "";
            txtavg_21.Text = "";

            // Row 22
            txtDateOfTest22.Text = "";
            txtDuration22.Text = "";
            txtTime22.Text = "";
            txtextension1_22.Text = "";
            txtextension2_22.Text = "";
            txtavg_22.Text = "";
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
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

    }
}