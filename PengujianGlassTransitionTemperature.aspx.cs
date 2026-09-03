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
using LIMS;
using ZXing;

namespace LIMS
{
    public partial class Tg_Test : System.Web.UI.Page
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
                    grade.Text = dt1.Rows[0]["Voltage"].ToString(); // voltage value in grade
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

            // Method to prefill upper section details based on parameter IDs
            SetTextBoxText(cableType, "12621");
            SetTextBoxText(cableSize, "12622");
            SetTextBoxText(grade, "12623");
            SetTextBoxText(refSpec, "12627");
            SetTextBoxText(testMethodNo, "12628");
            SetTextBoxText(jobOrderNo, "12629");
            SetTextBoxText(worksheetno, "12630");
            SetTextBoxText(roomno, "12625");
            SetTextBoxText(roomhumidity, "12631");
            SetTextBoxText(roomtemp, "12626");
            SetTextBoxText(Date, "12624");
            SetTextBoxText(equipmentno, "12632");

            // Textbox fields


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
            // Map reportParamId to your controls
            Dictionary<string, (DropDownList unit, TextBox spec, TextBox test)> fieldMap = new Dictionary<string, (DropDownList, TextBox, TextBox)>
    {
        { "12620", (ddlUnit1, txtSpecified1, txtValueTest1) },
        { "12610", (ddlUnit2, txtSpecified2, txtValueTest2) },
        { "12611", (ddlUnit3, txtSpecified3, txtValueTest3) },
        { "12612", (ddlUnit4, txtSpecified4, txtValueTest4) },
        { "12613", (ddlUnit5, txtSpecified5, txtValueTest5) },
        { "12614", (ddlUnit6, txtSpecified6, txtValueTest6) },
          { "12615", (ddlUnit7, txtSpecified7, txtValueTest7) },
    { "12616", (ddlUnit8, txtSpecified8, txtValueTest8) },
    { "12617", (ddlUnit9, txtSpecified9, txtValueTest9) },
    { "12618", (ddlUnit10, txtSpecified10, txtValueTest10) },
    { "12619", (ddlUnit11, txtSpecified11, txtValueTest11) }
    };

            foreach (DataRow row in dt.Rows)
            {
                // Use correct DataTable column names
                string paramId = row["ParameterID"].ToString();
                string rawValue = row["Result"].ToString();

                if (fieldMap.ContainsKey(paramId))
                {
                    var controls = fieldMap[paramId];

                    string[] parts = rawValue.Split('|');
                    if (parts.Length == 3)
                    {
                        // Ensure dropdown list contains the value before selecting
                        if (controls.unit.Items.FindByValue(parts[0]) != null)
                        {
                            controls.unit.SelectedValue = parts[0];
                        }
                        controls.spec.Text = parts[1];
                        controls.test.Text = parts[2];
                    }
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

                // Section 1 - Dimensions
                reportParamMapList.Add("12620"); // Dimension
                resultList.Add($"{ddlUnit1.SelectedValue}|{txtSpecified1.Text.Trim()}|{txtValueTest1.Text.Trim()}");

                reportParamMapList.Add("12610"); // Thickness
                resultList.Add($"{ddlUnit2.SelectedValue}|{txtSpecified2.Text.Trim()}|{txtValueTest2.Text.Trim()}");

                reportParamMapList.Add("12611"); // Width
                resultList.Add($"{ddlUnit3.SelectedValue}|{txtSpecified3.Text.Trim()}|{txtValueTest3.Text.Trim()}");

                reportParamMapList.Add("12612"); // Length
                resultList.Add($"{ddlUnit4.SelectedValue}|{txtSpecified4.Text.Trim()}|{txtValueTest4.Text.Trim()}");

                reportParamMapList.Add("12613"); // Weight of sample
                resultList.Add($"{ddlUnit5.SelectedValue}|{txtSpecified5.Text.Trim()}|{txtValueTest5.Text.Trim()}");

                // Section 2 - Condition of specimen
                reportParamMapList.Add("12614"); // Condition of specimen
                resultList.Add($"{ddlUnit6.SelectedValue}|{txtSpecified6.Text.Trim()}|{txtValueTest6.Text.Trim()}");

                reportParamMapList.Add("12615"); // Temperature
                resultList.Add($"{ddlUnit7.SelectedValue}|{txtSpecified7.Text.Trim()}|{txtValueTest7.Text.Trim()}");

                reportParamMapList.Add("12616"); // Duration test
                resultList.Add($"{ddlUnit8.SelectedValue}|{txtSpecified8.Text.Trim()}|{txtValueTest8.Text.Trim()}");

                reportParamMapList.Add("12617"); // Weight of sample after condition
                resultList.Add($"{ddlUnit9.SelectedValue}|{txtSpecified9.Text.Trim()}|{txtValueTest9.Text.Trim()}");

                // Section 3 - Thermoset Polymer
                reportParamMapList.Add("12618"); // Thermoset Polymer
                resultList.Add($"{ddlUnit10.SelectedValue}|Min./Max.{txtSpecified10.Text.Trim()}|{txtValueTest10.Text.Trim()}");

                reportParamMapList.Add("12619"); // Weight of sample after test
                resultList.Add($"{ddlUnit11.SelectedValue}|{txtSpecified11.Text.Trim()}|{txtValueTest11.Text.Trim()}");


                //Upper section
                reportParamMapList.Add("12621"); resultList.Add(cableType.Text);
                reportParamMapList.Add("12622"); resultList.Add(cableSize.Text);
                reportParamMapList.Add("12623"); resultList.Add(grade.Text); // voltage Value in grade
                reportParamMapList.Add("12627"); resultList.Add(refSpec.Text);
                reportParamMapList.Add("12628"); resultList.Add(testMethodNo.Text);
                reportParamMapList.Add("12629"); resultList.Add(jobOrderNo.Text);
                reportParamMapList.Add("12630"); resultList.Add(worksheetno.Text);
                reportParamMapList.Add("12625"); resultList.Add(roomno.Text);
                reportParamMapList.Add("12631"); resultList.Add(roomhumidity.Text);
                reportParamMapList.Add("12626"); resultList.Add(roomtemp.Text);
                reportParamMapList.Add("12624"); resultList.Add(Date.Text);
                reportParamMapList.Add("12632"); resultList.Add(equipmentno.Text);

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

                    Response.Redirect($"GenerateTestRecord_PengujianGlassTransitionTemperature.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
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
            // Clear Specified values
            txtSpecified1.Text = "";
            txtSpecified2.Text = "";
            txtSpecified3.Text = "";
            txtSpecified4.Text = "";
            txtSpecified5.Text = "";
            txtSpecified6.Text = "";
            txtSpecified7.Text = "";
            txtSpecified8.Text = "";
            txtSpecified10.Text = "";

            // Clear Test values
            txtValueTest1.Text = "";
            txtValueTest2.Text = "";
            txtValueTest3.Text = "";
            txtValueTest4.Text = "";
            txtValueTest5.Text = "";
            txtValueTest6.Text = "";
            txtValueTest7.Text = "";
            txtValueTest8.Text = "";
            txtValueTest9.Text = "";
            txtValueTest10.Text = "";

            // Reset DropDowns (set to default or first item)
            ddlUnit1.SelectedIndex = 0;
            ddlUnit2.SelectedIndex = 0;
            ddlUnit3.SelectedIndex = 0;
            ddlUnit4.SelectedIndex = 0;
            ddlUnit5.SelectedIndex = 0;
            ddlUnit6.SelectedIndex = 0;
            ddlUnit7.SelectedIndex = 0;
            ddlUnit8.SelectedIndex = 0;
            ddlUnit9.SelectedIndex = 0;
            ddlUnit10.SelectedIndex = 0;
        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
        private void DeleteExistingRecords(int sampleId ,int reportid)
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
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (Request.QueryString["s"] != null)
                {
                    int sampleId = Convert.ToInt32(Request.QueryString["s"]);
                    int reportId =Convert.ToInt32(Request.QueryString["r"]);

                    if (_cm == null) _cm = new DAL.CM();
                    _cm.sampleId = sampleId;
                    _cm.ReportID = reportId.ToString();
                    DeleteExistingRecords(sampleId, reportId);

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
}