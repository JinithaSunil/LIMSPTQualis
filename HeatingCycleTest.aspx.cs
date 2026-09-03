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
    public partial class HeatingCycleTest : System.Web.UI.Page
    {

        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                gvTests.DataSource = TestTable;
                gvTests.DataBind();

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
                    roomno.Text = "9";
                    jobOrderNo.Text = "WO/2024020848";
                    worksheetno.Text = "-";
                    equipmentno.Text = "QE/12/TH/19";
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", "alert('Could not retrieve sample data. Please check your sample ID.');", true);
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
                ScriptManager.RegisterStartupScript(this, GetType(), "PrefillError", $"alert('Error prefilling form: {ex.Message}');", true);
            }
        }

        private void PrefillUpperSection(DataTable existingData)
        {
            SetLabelText(cableType, "17567");
            SetLabelText(cableSize, "17568");
            SetLabelText(voltage, "17569");
            SetTextBoxText(Date, "17570");
            SetTextBoxText(roomno, "17571");
            SetTextBoxText(roomtemp, "17572");
            SetLabelText(refSpec, "17573");
            SetLabelText(testMethodNo, "17574");
            SetLabelText(jobOrderNo, "17575");
            SetLabelText(worksheetno, "17576");
            SetTextBoxText(roomhumidity, "17577");
            SetLabelText(equipmentno, "17578");

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
            DataTable gridDt = new DataTable();
            gridDt.Columns.Add("StartDate");
            gridDt.Columns.Add("StartTime");
            gridDt.Columns.Add("ReachedTime");
            gridDt.Columns.Add("ActualTemp");
            gridDt.Columns.Add("CoolingTime");

            string startDatesCsv = GetResultValue(dt, "17581");
            string startTimesCsv = GetResultValue(dt, "17582");
            string reachedTimesCsv = GetResultValue(dt, "17583");
            string actualTempsCsv = GetResultValue(dt, "17584");
            string coolingTimesCsv = GetResultValue(dt, "17585");

            if (!string.IsNullOrEmpty(startDatesCsv))
            {
                string[] startDates = startDatesCsv.Split(',');
                string[] startTimes = startTimesCsv.Split(',');
                string[] reachedTimes = reachedTimesCsv.Split(',');
                string[] actualTemps = actualTempsCsv.Split(',');
                string[] coolingTimes = coolingTimesCsv.Split(',');

                for (int i = 0; i < startDates.Length; i++)
                {
                    DataRow dr = gridDt.NewRow();
                    dr["StartDate"] = startDates[i].Trim();
                    dr["StartTime"] = (i < startTimes.Length) ? startTimes[i].Trim() : "";
                    dr["ReachedTime"] = (i < reachedTimes.Length) ? reachedTimes[i].Trim() : "";
                    dr["ActualTemp"] = (i < actualTemps.Length) ? actualTemps[i].Trim() : "";
                    dr["CoolingTime"] = (i < coolingTimes.Length) ? coolingTimes[i].Trim() : "";
                    gridDt.Rows.Add(dr);
                }
            }
            TestTable = gridDt;
            gvTests.DataSource = gridDt;
            gvTests.DataBind();
        }

        private string GetResultValue(DataTable dt, string parameterId)
        {
            DataRow[] rows = dt.Select($"ParameterID = '{parameterId}'");
            if (rows.Length > 0)
                return rows[0]["Result"].ToString();
            return "";
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

                reportParamMapList.Add("17567"); resultList.Add(cableType.Text);
                reportParamMapList.Add("17568"); resultList.Add(cableSize.Text);
                reportParamMapList.Add("17569"); resultList.Add(voltage.Text);
                reportParamMapList.Add("17570"); resultList.Add(Date.Text);
                reportParamMapList.Add("17571"); resultList.Add(roomno.Text);
                reportParamMapList.Add("17572"); resultList.Add(roomtemp.Text);
                reportParamMapList.Add("17573"); resultList.Add(refSpec.Text);
                reportParamMapList.Add("17574"); resultList.Add(testMethodNo.Text);
                reportParamMapList.Add("17575"); resultList.Add(jobOrderNo.Text);
                reportParamMapList.Add("17576"); resultList.Add(worksheetno.Text);
                reportParamMapList.Add("17577"); resultList.Add(roomhumidity.Text);
                reportParamMapList.Add("17578"); resultList.Add(equipmentno.Text);

                DataTable dt = TestTable;
                List<string> startDates = new List<string>();
                List<string> startTimes = new List<string>();
                List<string> reachedTimes = new List<string>();
                List<string> actualTemps = new List<string>();
                List<string> coolingTimes = new List<string>();

                foreach (DataRow row in dt.Rows)
                {
                    startDates.Add(row["StartDate"].ToString());
                    startTimes.Add(row["StartTime"].ToString());
                    reachedTimes.Add(row["ReachedTime"].ToString());
                    actualTemps.Add(row["ActualTemp"].ToString());
                    coolingTimes.Add(row["CoolingTime"].ToString());
                }

                string startDatesCsv = string.Join(",", startDates);
                string startTimesCsv = string.Join(",", startTimes);
                string reachedTimesCsv = string.Join(",", reachedTimes);
                string actualTempsCsv = string.Join(",", actualTemps);
                string coolingTimesCsv = string.Join(",", coolingTimes);

                reportParamMapList.Add("17581"); resultList.Add(startDatesCsv);
                reportParamMapList.Add("17582"); resultList.Add(startTimesCsv);
                reportParamMapList.Add("17583"); resultList.Add(reachedTimesCsv);
                reportParamMapList.Add("17584"); resultList.Add(actualTempsCsv);
                reportParamMapList.Add("17585"); resultList.Add(coolingTimesCsv);

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
                    Response.Redirect($"GenerateTestRecord_HeatingCycleTest.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
                    return true;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", "alert('Failed to save data.');", true);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", $"alert('Error: {ex.Message}');", true);
                return false;
            }
        }


        protected void btnClearData_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        protected void ClearForm()
        {
            txtStartDate.Text = "";
            txtStartTime.Text = "";
            txtReachedTime.Text = "";
            txtActualTemp.Text = "";
            txtCooling.Text = "";
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void DeleteExistingRecords(int sampleId, int reportId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    conn.Open();
                    string deleteQuery = "DELETE FROM TestResultsPTQualis WHERE SampleID = @SampleID and ReportID = @ReportID";
                    using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@SampleID", sampleId);
                        cmd.Parameters.AddWithValue("@ReportID", reportId);
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
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
                    int reportID = Convert.ToInt32(Request.QueryString["r"]);
                    if (_cm == null) _cm = new DAL.CM();
                    _cm.sampleId = sampleId;

                    DeleteExistingRecords(sampleId, reportID);

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

        //New Code for data show in gridview //
        private DataTable TestTable
        {
            get
            {
                if (ViewState["TestTable"] == null)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("StartDate");
                    dt.Columns.Add("StartTime");
                    dt.Columns.Add("ReachedTime");
                    dt.Columns.Add("ActualTemp");
                    dt.Columns.Add("CoolingTime");
                    ViewState["TestTable"] = dt;
                }
                return (DataTable)ViewState["TestTable"];
            }
            set
            {
                ViewState["TestTable"] = value;
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            DataTable dt = TestTable;

            DataRow dr = dt.NewRow();
            dr["StartDate"] = txtStartDate.Text.Trim();
            dr["StartTime"] = txtStartTime.Text.Trim();
            dr["ReachedTime"] = txtReachedTime.Text.Trim();
            dr["ActualTemp"] = txtActualTemp.Text.Trim();
            dr["CoolingTime"] = txtCooling.Text.Trim();
            dt.Rows.Add(dr);

            TestTable = dt;

            gvTests.DataSource = dt;
            gvTests.DataBind();

            ClearForm();
        }

        protected void gvTests_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            DataTable dt = TestTable;
            dt.Rows.RemoveAt(e.RowIndex);
            TestTable = dt;

            gvTests.DataSource = dt;
            gvTests.DataBind();
        }



    }
}