using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LIMS;

namespace LIMS
{
	public partial class TensileStrengthCFCC : System.Web.UI.Page
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

        protected void CheckAndPrefillForm()
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

                    

                    DataTable existingData = _cm.GetExistingTestResultsForSample(sampleId, reportId);

                    if (existingData != null && existingData.Rows.Count > 0)
                    {
                    

                        btnSaveData.Visible = false;
                        btnClearData.Visible = false;
                        btnUpdateData.Visible = true;

                 

                        PrefillUpperSection(existingData);



                        PrefillTestResultsGrid(existingData);

                    }
                }
            }
            catch (Exception ex)
            {
              
                ScriptManager.RegisterStartupScript(this, GetType(), "PrefillError",
                    $"alert('Error prefilling form: {ex.Message}');", true);
            }
        }
        private void PrefillUpperSection(DataTable existingData)
        {

       

            SetTextBoxText(cableType, "12637");
            SetTextBoxText(cableSize, "12638");
            SetTextBoxText(voltage, "12639");
            SetTextBoxText(refSpec, "12643");
            SetTextBoxText(testMethodNo, "12644");
            SetTextBoxText(jobOrderNo, "12645");
            SetTextBoxText(worksheetno, "12646");
            SetTextBoxText(roomno, "12641");
            SetTextBoxText(roomhumidity, "12647");
            SetTextBoxText(roomtemp, "12642");
            SetTextBoxText(Date, "12640");
            SetTextBoxText(equipmentno, "12648");

            


            void SetLabelText(Label label, string paramId)
            {
                var row = existingData.Select($"ParameterID = '{paramId}'").FirstOrDefault();
                if (row != null)
                {
                    label.Text = row["Result"].ToString();
                }
            }

    

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
           

            Dictionary<string, (DropDownList unit, TextBox spec, TextBox test)> fieldMap = new Dictionary<string, (DropDownList, TextBox, TextBox)>
    {
        { "12633", (ddlUnit1, txtSpecified1, txtValueTest1) },  
        { "12634", (ddlUnit2, txtSpecified2, txtValueTest2) },  
        { "12635", (ddlUnit3, txtSpecified3, txtValueTest3) },  
        { "12636", (ddlUnit4, txtSpecified4, txtValueTest4) }, 
     
    };

            foreach (DataRow row in dt.Rows)
            {
             
                string paramId = row["ParameterID"].ToString();
                string rawValue = row["Result"].ToString();

                if (fieldMap.ContainsKey(paramId))
                {
                    var controls = fieldMap[paramId];

                    string[] parts = rawValue.Split('|');
                    if (parts.Length == 3)
                    {
                      
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
          
            List<TextBox> allTextBoxes = new List<TextBox>();

           
            allTextBoxes.Add(roomno);
            allTextBoxes.Add(roomhumidity);
            allTextBoxes.Add(roomtemp);

              foreach (var textBox in allTextBoxes)
            {
                textBox.BorderColor = System.Drawing.Color.Empty; 
                textBox.BorderStyle = BorderStyle.NotSet; 
                textBox.BorderWidth = 0; 
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

              
                reportParamMapList.Add("12633"); //
                resultList.Add($"{ddlUnit1.SelectedValue}|{txtSpecified1.Text.Trim()}|{txtValueTest1.Text.Trim()}");

                reportParamMapList.Add("12634"); // Cross Section area
                resultList.Add($"{ddlUnit2.SelectedValue}|{txtSpecified2.Text.Trim()}|{txtValueTest2.Text.Trim()}");

                reportParamMapList.Add("12635"); //Breaking load
                resultList.Add($"{ddlUnit3.SelectedValue}|{txtSpecified3.Text.Trim()}|{txtValueTest3.Text.Trim()}");

                reportParamMapList.Add("12636"); //Tensile Strength
                resultList.Add($"{ddlUnit4.SelectedValue}|{txtSpecified4.Text.Trim()}|{txtValueTest4.Text.Trim()}");




                //Upper section
                reportParamMapList.Add("12637"); resultList.Add(cableType.Text);
                reportParamMapList.Add("12638"); resultList.Add(cableSize.Text);
                reportParamMapList.Add("12639"); resultList.Add(voltage.Text);
                reportParamMapList.Add("12643"); resultList.Add(refSpec.Text);
                reportParamMapList.Add("12644"); resultList.Add(testMethodNo.Text);
                reportParamMapList.Add("12645"); resultList.Add(jobOrderNo.Text);
                reportParamMapList.Add("12646"); resultList.Add(worksheetno.Text);
                reportParamMapList.Add("12641"); resultList.Add(roomno.Text);
                reportParamMapList.Add("12647"); resultList.Add(roomhumidity.Text);
                reportParamMapList.Add("12642"); resultList.Add(roomtemp.Text);
                reportParamMapList.Add("12640"); resultList.Add(Date.Text);
                reportParamMapList.Add("12648"); resultList.Add(equipmentno.Text);

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

                    Response.Redirect($"GenerateTestRecord_TensileStrength.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
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

            txtSpecified1.Text = "";
            txtSpecified2.Text = "";
            txtSpecified3.Text = "";
            txtSpecified4.Text = "";



            txtValueTest1.Text = "";
            txtValueTest2.Text = "";
            txtValueTest3.Text = "";
            txtValueTest4.Text = "";


            ddlUnit1.SelectedIndex = 0;
            ddlUnit2.SelectedIndex = 0;
            ddlUnit3.SelectedIndex = 0;
            ddlUnit4.SelectedIndex = 0;


        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
        private void DeleteExistingRecords(int sampleId , int reportid)
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


    }
}