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
	public partial class PenetrasiAir : System.Web.UI.Page
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
                        PrefillBendingWaterGrid(existingData);
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
            SetTextBoxText(cableType, "17589");
            SetTextBoxText(cableSize, "17590");
            SetTextBoxText(voltage, "17591");
            SetTextBoxText(Date, "17592");
            SetTextBoxText(roomno, "17593");
            SetTextBoxText(roomtemp, "17594");
            SetTextBoxText(refSpec, "17595");
            SetTextBoxText(testMethodNo, "17596");
            SetTextBoxText(jobOrderNo, "17597");
            SetTextBoxText(worksheetno, "17598");
            SetTextBoxText(roomhumidity, "17599");
            SetTextBoxText(equipmentno, "17600");

            SetTextBoxText(txtspecification, "17609");
            SetTextBoxText(txtspecMin, "17610");
            SetTextBoxText(txtspecMin2, "17611");



            SetTextBoxText(txtonconductor1, "17620");
            SetTextBoxText(txtonconductor2, "17623");
            SetTextBoxText(txtonconductiveWBTtype1, "17622");
            SetTextBoxText(txtonconductiveWBTtype2, "17629");
            SetTextBoxText(txtonnonconductiveWBTtype1, "17622");
            SetTextBoxText(txtonnonconductiveWBTtype2, "17630");



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

            string startDatesCsv = GetResultValue(dt, "17603");
            string startTimesCsv = GetResultValue(dt, "17604");
            string reachedTimesCsv = GetResultValue(dt, "17605");
            string actualTempsCsv = GetResultValue(dt, "17606");
            string coolingTimesCsv = GetResultValue(dt, "17607");

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

                    DateTime parsedDate;
                    if (DateTime.TryParse(startDates[i].Trim(), out parsedDate))
                    {
                        dr["StartDate"] = parsedDate.ToString("dd-MM-yyyy");
                    }
                    else
                    {
                        dr["StartDate"] = startDates[i].Trim(); // fallback to original if parsing fails
                    }

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
        private void PrefillBendingWaterGrid(DataTable dt)
        {
            DataTable gridDt = new DataTable();
            gridDt.Columns.Add("dateforbending");
            gridDt.Columns.Add("Diametercabelforbending");
            gridDt.Columns.Add("Conductordiameterforbending");
            gridDt.Columns.Add("MaxDiameterforBending");
            gridDt.Columns.Add("Diametercylinderforbending");
            gridDt.Columns.Add("StartDateWaterforwater");
            gridDt.Columns.Add("StartTimeforwaterWater");
            gridDt.Columns.Add("FinishDateforwaterWater");
            gridDt.Columns.Add("FinishTimeforwaterWater");

            string dateForBendingCsv = GetResultValue(dt, "17612");
            string diameterCableCsv = GetResultValue(dt, "17624");
            string conductorDiameterCsv = GetResultValue(dt, "17627");
            string maxDiameterBendingCsv = GetResultValue(dt, "17625");
            string diameterCylinderCsv = GetResultValue(dt, "17626");
            string startDateWaterCsv = GetResultValue(dt, "17613");
            string startTimeWaterCsv = GetResultValue(dt, "17614");
            string finishDateWaterCsv = GetResultValue(dt, "17615");
            string finishTimeWaterCsv = GetResultValue(dt, "17616");

            if (!string.IsNullOrEmpty(dateForBendingCsv))
            {
                string[] dateForBendingArr = dateForBendingCsv.Split(',');
                string[] diameterCableArr = diameterCableCsv.Split(',');
                string[] conductorDiameterArr = conductorDiameterCsv.Split(',');
                string[] maxDiameterBendingArr = maxDiameterBendingCsv.Split(',');
                string[] diameterCylinderArr = diameterCylinderCsv.Split(',');
                string[] startDateWaterArr = startDateWaterCsv.Split(',');
                string[] startTimeWaterArr = startTimeWaterCsv.Split(',');
                string[] finishDateWaterArr = finishDateWaterCsv.Split(',');
                string[] finishTimeWaterArr = finishTimeWaterCsv.Split(',');

                for (int i = 0; i < dateForBendingArr.Length; i++)
                {
                    DataRow dr = gridDt.NewRow();

                    // Format dateforbending
                    if (DateTime.TryParse(dateForBendingArr[i].Trim(), out DateTime dtDateForBending))
                        dr["dateforbending"] = dtDateForBending.ToString("dd-MM-yyyy");
                    else
                        dr["dateforbending"] = dateForBendingArr[i].Trim();

                    dr["Diametercabelforbending"] = (i < diameterCableArr.Length) ? diameterCableArr[i].Trim() : "";
                    dr["Conductordiameterforbending"] = (i < conductorDiameterArr.Length) ? conductorDiameterArr[i].Trim() : "";
                    dr["MaxDiameterforBending"] = (i < maxDiameterBendingArr.Length) ? maxDiameterBendingArr[i].Trim() : "";
                    dr["Diametercylinderforbending"] = (i < diameterCylinderArr.Length) ? diameterCylinderArr[i].Trim() : "";

                    // Format StartDateWaterforwater
                    if (i < startDateWaterArr.Length && DateTime.TryParse(startDateWaterArr[i].Trim(), out DateTime dtStartDateWater))
                        dr["StartDateWaterforwater"] = dtStartDateWater.ToString("dd-MM-yyyy");
                    else
                        dr["StartDateWaterforwater"] = (i < startDateWaterArr.Length) ? startDateWaterArr[i].Trim() : "";

                    dr["StartTimeforwaterWater"] = (i < startTimeWaterArr.Length) ? startTimeWaterArr[i].Trim() : "";

                    // Format FinishDateforwaterWater
                    if (i < finishDateWaterArr.Length && DateTime.TryParse(finishDateWaterArr[i].Trim(), out DateTime dtFinishDateWater))
                        dr["FinishDateforwaterWater"] = dtFinishDateWater.ToString("dd-MM-yyyy");
                    else
                        dr["FinishDateforwaterWater"] = (i < finishDateWaterArr.Length) ? finishDateWaterArr[i].Trim() : "";

                    dr["FinishTimeforwaterWater"] = (i < finishTimeWaterArr.Length) ? finishTimeWaterArr[i].Trim() : "";

                    gridDt.Rows.Add(dr);
                }
            }

            BendingWaterTable = gridDt;
            gvBendingWater.DataSource = gridDt;
            gvBendingWater.DataBind();
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

                // Existing mappings (unchanged)
                reportParamMapList.Add("17589"); resultList.Add(cableType.Text);
                reportParamMapList.Add("17590"); resultList.Add(cableSize.Text);
                reportParamMapList.Add("17591"); resultList.Add(voltage.Text);
                reportParamMapList.Add("17592"); resultList.Add(Date.Text);
                reportParamMapList.Add("17593"); resultList.Add(roomno.Text);
                reportParamMapList.Add("17594"); resultList.Add(roomtemp.Text);
                reportParamMapList.Add("17595"); resultList.Add(refSpec.Text);
                reportParamMapList.Add("17596"); resultList.Add(testMethodNo.Text);
                reportParamMapList.Add("17597"); resultList.Add(jobOrderNo.Text);
                reportParamMapList.Add("17598"); resultList.Add(worksheetno.Text);
                reportParamMapList.Add("17599"); resultList.Add(roomhumidity.Text);
                reportParamMapList.Add("17600"); resultList.Add(equipmentno.Text);

                // Specification fields
                reportParamMapList.Add("17609"); resultList.Add(txtspecification.Text);
                reportParamMapList.Add("17610"); resultList.Add(txtspecMin.Text);
                reportParamMapList.Add("17611"); resultList.Add(txtspecMin2.Text);


                reportParamMapList.Add("17620"); resultList.Add(txtonconductor1.Text);
                reportParamMapList.Add("17623"); resultList.Add(txtonconductor2.Text);
                reportParamMapList.Add("17621"); resultList.Add(txtonconductiveWBTtype1.Text);
                reportParamMapList.Add("17629"); resultList.Add(txtonconductiveWBTtype2.Text);

                reportParamMapList.Add("17622"); resultList.Add(txtonnonconductiveWBTtype1.Text);
                reportParamMapList.Add("17630"); resultList.Add(txtonnonconductiveWBTtype2.Text);
             


                // --- Add TestTable data ---
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

                reportParamMapList.Add("17603"); resultList.Add(startDatesCsv);
                reportParamMapList.Add("17604"); resultList.Add(startTimesCsv);
                reportParamMapList.Add("17605"); resultList.Add(reachedTimesCsv);
                reportParamMapList.Add("17606"); resultList.Add(actualTempsCsv);
                reportParamMapList.Add("17607"); resultList.Add(coolingTimesCsv);

                // --- Add BendingWaterTable data ---
                DataTable dtBending = BendingWaterTable; // Your ViewState DataTable for bending water data

                List<string> dateForBendingList = new List<string>();
                List<string> diameterCableList = new List<string>();
                List<string> conductorDiameterList = new List<string>();
                List<string> maxDiameterBendingList = new List<string>();
                List<string> diameterCylinderList = new List<string>();
                List<string> startDateWaterList = new List<string>();
                List<string> startTimeWaterList = new List<string>();
                List<string> finishDateWaterList = new List<string>();
                List<string> finishTimeWaterList = new List<string>();

                foreach (DataRow row in dtBending.Rows)
                {
                    dateForBendingList.Add(row["dateforbending"].ToString());
                    diameterCableList.Add(row["Diametercabelforbending"].ToString());
                    conductorDiameterList.Add(row["Conductordiameterforbending"].ToString());
                    maxDiameterBendingList.Add(row["MaxDiameterforBending"].ToString());
                    diameterCylinderList.Add(row["Diametercylinderforbending"].ToString());
                    startDateWaterList.Add(row["StartDateWaterforwater"].ToString());
                    startTimeWaterList.Add(row["StartTimeforwaterWater"].ToString());
                    finishDateWaterList.Add(row["FinishDateforwaterWater"].ToString());
                    finishTimeWaterList.Add(row["FinishTimeforwaterWater"].ToString());
                }

                reportParamMapList.Add("17612"); resultList.Add(string.Join(",", dateForBendingList));
                reportParamMapList.Add("17624"); resultList.Add(string.Join(",", diameterCableList));
                reportParamMapList.Add("17627"); resultList.Add(string.Join(",", conductorDiameterList));
                reportParamMapList.Add("17625"); resultList.Add(string.Join(",", maxDiameterBendingList));
                reportParamMapList.Add("17626"); resultList.Add(string.Join(",", diameterCylinderList));
                reportParamMapList.Add("17613"); resultList.Add(string.Join(",", startDateWaterList));
                reportParamMapList.Add("17614"); resultList.Add(string.Join(",", startTimeWaterList));
                reportParamMapList.Add("17615"); resultList.Add(string.Join(",", finishDateWaterList));
                reportParamMapList.Add("17616"); resultList.Add(string.Join(",", finishTimeWaterList));

                // NOTE: Replace the above "BendingDate", etc keys with the correct report param IDs from your DB schema

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
                    Response.Redirect($"GenerateTestRecord_PenetrasiAir.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
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
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            DataTable dt = TestTable;

            DataRow dr = dt.NewRow();
            if (DateTime.TryParse(txtStartDate.Text.Trim(), out DateTime parsedStartDate))
            {
                dr["StartDate"] = parsedStartDate.ToString("dd-MM-yyyy");
            }
            else
            {
                dr["StartDate"] = txtStartDate.Text.Trim(); // fallback if invalid
            }



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
        protected void btnAdd2_Click(object sender, EventArgs e)
        {
            DataTable dt = BendingWaterTable;

            DataRow dr = dt.NewRow();

            // Format dateforbending
            if (DateTime.TryParse(txtbendingtestdate.Text.Trim(), out DateTime parsedBendingDate))
            {
                dr["dateforbending"] = parsedBendingDate.ToString("dd-MM-yyyy");
            }
            else
            {
                dr["dateforbending"] = txtbendingtestdate.Text.Trim();
            }

            dr["Diametercabelforbending"] = txtDiametercable.Text.Trim();
            dr["Conductordiameterforbending"] = txtconductordiameter.Text.Trim();
            dr["MaxDiameterforBending"] = txtMaxbendingdiameter.Text.Trim();
            dr["Diametercylinderforbending"] = txtCylinderdiameter.Text.Trim();

            // Format StartDateWaterforwater
            if (DateTime.TryParse(txtstartdatewater.Text.Trim(), out DateTime parsedStartDateWater))
            {
                dr["StartDateWaterforwater"] = parsedStartDateWater.ToString("dd-MM-yyyy");
            }
            else
            {
                dr["StartDateWaterforwater"] = txtstartdatewater.Text.Trim();
            }

            dr["StartTimeforwaterWater"] = txtstarttimewater.Text.Trim();

            // Format FinishDateforwaterWater
            if (DateTime.TryParse(txtfinisdatewater.Text.Trim(), out DateTime parsedFinishDateWater))
            {
                dr["FinishDateforwaterWater"] = parsedFinishDateWater.ToString("dd-MM-yyyy");
            }
            else
            {
                dr["FinishDateforwaterWater"] = txtfinisdatewater.Text.Trim();
            }

            dr["FinishTimeforwaterWater"] = txtfinishtimewater.Text.Trim();

            dt.Rows.Add(dr);

            BendingWaterTable = dt;

            gvBendingWater.DataSource = dt;
            gvBendingWater.DataBind();

            ClearForm2();
        }
        private void ClearForm2()
        {
            txtbendingtestdate.Text = "";
            txtDiametercable.Text = "";
            txtconductordiameter.Text = "";
            txtMaxbendingdiameter.Text = "";
            txtCylinderdiameter.Text = "";
            txtstartdatewater.Text = "";
            txtstarttimewater.Text = "";
            txtfinisdatewater.Text = "";
            txtfinishtimewater.Text = "";
           
            ViewState["BendingWaterTable"] = null;
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
            ViewState["TestTable"] = null;
        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
            ClearForm2();

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
        private DataTable BendingWaterTable
        {
            get
            {
                if (ViewState["BendingWaterTable"] == null)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("dateforbending");
                    dt.Columns.Add("Diametercabelforbending");
                    dt.Columns.Add("Conductordiameterforbending");
                    dt.Columns.Add("MaxDiameterforBending");
                    dt.Columns.Add("Diametercylinderforbending");
                    dt.Columns.Add("StartDateWaterforwater");
                    dt.Columns.Add("StartTimeforwaterWater");
                    dt.Columns.Add("FinishDateforwaterWater");
                    dt.Columns.Add("FinishTimeforwaterWater");
                    ViewState["BendingWaterTable"] = dt;
                }
                return (DataTable)ViewState["BendingWaterTable"];
            }
            set
            {
                ViewState["BendingWaterTable"] = value;
            }
        }
        protected void gvTests_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            DataTable dt = TestTable;

            if (dt != null && e.RowIndex >= 0 && e.RowIndex < dt.Rows.Count)
            {
                dt.Rows.RemoveAt(e.RowIndex);
                TestTable = dt;

                gvTests.DataSource = dt;
                gvTests.DataBind();
            }
        }
        protected void gvBendingWater_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            DataTable dt = BendingWaterTable; //   DataTable
            if (dt != null && e.RowIndex >= 0 && e.RowIndex < dt.Rows.Count)
            {
                dt.Rows.RemoveAt(e.RowIndex);
                BendingWaterTable = dt; // Save updated data source (ViewState, Session, etc)

                gvBendingWater.DataSource = dt;
                gvBendingWater.DataBind();
            }
        }
        protected void gvBendingWater_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                GridViewRow headerRow1 = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);
                GridViewRow headerRow2 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);
                GridViewRow headerRow3 = new GridViewRow(2, 0, DataControlRowType.Header, DataControlRowState.Insert);
                GridViewRow headerRow4 = new GridViewRow(2, 0, DataControlRowType.Header, DataControlRowState.Insert);

                // Row 1: Bending Test & Water Penetration
                headerRow1.Cells.Add(CreateHeaderCell("Uji tekuk ", 5, 1));
                headerRow1.Cells.Add(CreateHeaderCell("Penetrasi air", 4, 1));
                headerRow1.Cells.Add(CreateHeaderCell("Action", 0, 4));
                gvBendingWater.Controls[0].Controls.AddAt(0, headerRow1);

                // Row 2: Column titles under groups
                headerRow2.Cells.Add(CreateHeaderCell("Tanggal",1, 3));
                headerRow2.Cells.Add(CreateHeaderCell("Diameter kabel", 1,3));
                headerRow2.Cells.Add(CreateHeaderCell("Diameter penghantar", 1, 3));
                headerRow2.Cells.Add(CreateHeaderCell("Maks. diameter bending", 1, 3));
                headerRow2.Cells.Add(CreateHeaderCell("Diameter silinder", 1, 3));
                headerRow2.Cells.Add(CreateHeaderCell("Pada temp. ruang 24 jam", 4, 1));
                gvBendingWater.Controls[0].Controls.AddAt(1, headerRow2);

                // Row 3: Start & Finish under Water Penetration
                headerRow3.Cells.Add(CreateHeaderCell("Mulai", 2, 1));
                headerRow3.Cells.Add(CreateHeaderCell("Selesai", 2, 1));
                gvBendingWater.Controls[0].Controls.AddAt(2, headerRow3);


                headerRow4.Cells.Add(CreateHeaderCell("Tanggal", 1, 1));
                headerRow4.Cells.Add(CreateHeaderCell("Jam", 1, 1));
                headerRow4.Cells.Add(CreateHeaderCell("Tanggal", 1, 1));
                headerRow4.Cells.Add(CreateHeaderCell("Jam", 1, 1));
                gvBendingWater.Controls[0].Controls.AddAt(3, headerRow4);
            }
        }
        private TableHeaderCell CreateHeaderCell(string text, int columnSpan, int rowSpan)
        {
            TableHeaderCell cell = new TableHeaderCell();
            cell.Text = text;
            cell.ColumnSpan = columnSpan;
            cell.RowSpan = rowSpan;
            cell.HorizontalAlign = HorizontalAlign.Center;
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.BackColor = System.Drawing.ColorTranslator.FromHtml("#f2f2f2");

            return cell;
        }

    }
}