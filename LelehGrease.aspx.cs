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
    public partial class LelehGrease : System.Web.UI.Page
    {

        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (_cm == null) _cm = new DAL.CM();

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
                    cableType.Text = dt1.Rows[0]["SampleType"].ToString();
                    length.Text = dt1.Rows[0]["Voltage"].ToString();
                    cableSize.Text = dt1.Rows[0]["SizeOfSample"].ToString();
                    drumno.Text = dt1.Rows[0]["TestMethod"].ToString();
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
            SetTextBoxText(cableType, "12300");
            SetTextBoxText(cableSize, "12826");
            SetTextBoxText(length, "12302");
            SetTextBoxText(refSpec, "12306");
            SetTextBoxText(drumno, "12301");
            SetTextBoxText(jobOrderNo, "12307");
            SetTextBoxText(worksheetno, "12308");
            SetTextBoxText(roomno, "12303");
            SetTextBoxText(roomhumidity, "12309");
            SetTextBoxText(roomtemp, "12304");
            SetTextBoxText(Date, "12827");
            SetTextBoxText(equipmentno, "12310");
            SetTextBoxText(markingofcable, "12305");

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
            // Map parameter IDs to their corresponding TextBox arrays for the 7 rows
            Dictionary<string, TextBox[]> fieldMap = new Dictionary<string, TextBox[]>
            {
                {
                    "12311", // Berat Grease
                    new TextBox[] { txtBeratGrease1, txtBeratGrease2, txtBeratGrease3, txtBeratGrease4, txtBeratGrease5, txtBeratGrease6, txtBeratGrease7 }
                },
                {
                    "12823", // Temperatur
                    new TextBox[] { txtTemperatur1, txtTemperatur2, txtTemperatur3, txtTemperatur4, txtTemperatur5, txtTemperatur6, txtTemperatur7 }
                },
                {
                    "12824", // Kondisi Grease
                    new TextBox[] { txtKondisiGrease1, txtKondisiGrease2, txtKondisiGrease3, txtKondisiGrease4, txtKondisiGrease5, txtKondisiGrease6, txtKondisiGrease7 }
                },
                {
                    "12825", // Keterangan
                    new TextBox[] { txtKeterangan1, txtKeterangan2, txtKeterangan3, txtKeterangan4, txtKeterangan5, txtKeterangan6, txtKeterangan7 }
                }
            };

            foreach (DataRow row in dt.Rows)
            {
                // Use correct DataTable column names
                string paramId = row["ParameterID"].ToString();
                string rawValue = row["Result"].ToString();

                if (fieldMap.ContainsKey(paramId))
                {
                    // Split the pipe-separated values
                    string[] values = rawValue.Split('|');
                    TextBox[] textBoxes = fieldMap[paramId];

                    // Fill each textbox with corresponding value
                    for (int i = 0; i < Math.Min(values.Length, textBoxes.Length); i++)
                    {
                        if (!string.IsNullOrWhiteSpace(values[i]))
                        {
                            textBoxes[i].Text = values[i].Trim();
                        }
                    }
                }
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            bool isSaved = Submitform();
            if (isSaved)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "SaveSuccess", "alert('Test results and Report Generated successfully!');", true);
                ClearForm();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "SaveFailed", "alert('Test results were not saved due to an error.');", true);
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

                // Section 1 - Berat Grease
                reportParamMapList.Add("12311"); //Berat Grease 
                resultList.Add($"{txtBeratGrease1.Text.Trim()}|{txtBeratGrease2.Text.Trim()}|{txtBeratGrease3.Text.Trim()}|{txtBeratGrease4.Text.Trim()}|{txtBeratGrease5.Text.Trim()}|{txtBeratGrease6.Text.Trim()}|{txtBeratGrease7.Text.Trim()}");

                reportParamMapList.Add("12823"); //Temperatur
                resultList.Add($"{txtTemperatur1.Text.Trim()}|{txtTemperatur2.Text.Trim()}|{txtTemperatur3.Text.Trim()}|{txtTemperatur4.Text.Trim()}|{txtTemperatur5.Text.Trim()}|{txtTemperatur6.Text.Trim()}|{txtTemperatur7.Text.Trim()}");

                reportParamMapList.Add("12824"); //Kondisi Grease(Sample condition)
                resultList.Add($"{txtKondisiGrease1.Text.Trim()}|{txtKondisiGrease2.Text.Trim()}|{txtKondisiGrease3.Text.Trim()}|{txtKondisiGrease4.Text.Trim()}|{txtKondisiGrease5.Text.Trim()}|{txtKondisiGrease6.Text.Trim()}|{txtKondisiGrease7.Text.Trim()}");

                reportParamMapList.Add("12825"); //Keterangan
                resultList.Add($"{txtKeterangan1.Text.Trim()}|{txtKeterangan2.Text.Trim()}|{txtKeterangan3.Text.Trim()}|{txtKeterangan4.Text.Trim()}|{txtKeterangan5.Text.Trim()}|{txtKeterangan6.Text.Trim()}|{txtKeterangan7.Text.Trim()}");

                reportParamMapList.Add("12300"); resultList.Add(cableType.Text);
                reportParamMapList.Add("12826"); resultList.Add(cableSize.Text);
                reportParamMapList.Add("12302"); resultList.Add(length.Text);
                reportParamMapList.Add("12306"); resultList.Add(refSpec.Text);
                reportParamMapList.Add("12301"); resultList.Add(drumno.Text);
                reportParamMapList.Add("12307"); resultList.Add(jobOrderNo.Text);
                reportParamMapList.Add("12308"); resultList.Add(worksheetno.Text);
                reportParamMapList.Add("12303"); resultList.Add(roomno.Text);
                reportParamMapList.Add("12309"); resultList.Add(roomhumidity.Text);
                reportParamMapList.Add("12304"); resultList.Add(roomtemp.Text);
                reportParamMapList.Add("12827"); resultList.Add(Date.Text);
                reportParamMapList.Add("12310"); resultList.Add(equipmentno.Text);
                reportParamMapList.Add("12305"); resultList.Add(markingofcable.Text);

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

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showSuccess", "alert('Data saved successfully!');", true);

                    Response.Redirect($"GenerateTestRecord_Meltingofgrease.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
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

        protected void btnClearData_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
        protected void ClearForm()
        {
            // Clear Specified values
          
            roomhumidity.Text = "";
            roomtemp.Text = "";

        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
        private void DeleteExistingRecords(int sampleId, int reportid)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    conn.Open();

                    // Delete all existing test results for this sample
                    string deleteQuery = "DELETE FROM TestResultsPTQualis WHERE SampleID = @SampleID AND ReportID = @ReportID ";

                    using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@SampleID", sampleId);
                        cmd.Parameters.AddWithValue("@ReportID", reportid);
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
                if (Request.QueryString["s"] != null && Request.QueryString["r"] != null)
                {
                    int sampleId = Convert.ToInt32(Request.QueryString["s"]);
                    int reportid = Convert.ToInt32(Request.QueryString["r"]);
                    if (_cm == null) _cm = new DAL.CM();
                    _cm.sampleId = sampleId;

                    DeleteExistingRecords(sampleId, reportid);

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


    }
}