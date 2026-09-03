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
    public partial class TanDeltaTest : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataTable dt0 = _cm.Get_Core();
                ddlGetCore.DataSource = dt0;
                ddlGetCore.DataTextField = "CoreName";
                ddlGetCore.DataValueField = "CoreId";
                ddlGetCore.DataBind();
                ddlGetCore.Items.Insert(0, new ListItem("-- Select Core --", "0"));

                // Initialize GridView
                gvTests.DataSource = TestTable;
                gvTests.DataBind();

                DataTable dt01 = _cm.Get_Core();
                ddlGetCore2.DataSource = dt01;
                ddlGetCore2.DataTextField = "CoreName";
                ddlGetCore2.DataValueField = "CoreId";
                ddlGetCore2.DataBind();
                ddlGetCore2.Items.Insert(0, new ListItem("-- Select Core --", "0"));

                // Initialize second GridView
                gvTests2.DataSource = TestTable2;
                gvTests2.DataBind();

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

                    // Hardcoded fields
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

        // DataTables for Test 1 and Test 2
        private DataTable TestTable
        {
            get
            {
                if (ViewState["TestTable"] == null)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Core");
                    dt.Columns.Add("VoltageTest");
                    dt.Columns.Add("CapacitanceMeasured");
                    dt.Columns.Add("CapacitanceResult");
                    dt.Columns.Add("TanDeltaMeasured");
                    dt.Columns.Add("TanDeltaResult");
                    dt.Columns.Add("TanDeltaStandard");
                    ViewState["TestTable"] = dt;
                }
                return (DataTable)ViewState["TestTable"];
            }
            set
            {
                ViewState["TestTable"] = value;
            }
        }

        private DataTable TestTable2
        {
            get
            {
                if (ViewState["TestTable2"] == null)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("TemperatureType");
                    dt.Columns.Add("Core");
                    dt.Columns.Add("VoltageTest");
                    dt.Columns.Add("CapacitanceMeasured");
                    dt.Columns.Add("CapacitanceResult");
                    dt.Columns.Add("TanDeltaMeasured");
                    dt.Columns.Add("TanDeltaResult");
                    dt.Columns.Add("TanDeltaStandard");
                    ViewState["TestTable2"] = dt;
                }
                return (DataTable)ViewState["TestTable2"];
            }
            set
            {
                ViewState["TestTable2"] = value;
            }
        }

        // Add rows to Test 1
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                DataTable dt = TestTable;
                DataRow dr = dt.NewRow();

                // Get selected core text
                string coreName = ddlGetCore.SelectedItem.Text;
                if (coreName == "-- Select Core --")
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", "alert('Please select a core.');", true);
                    return;
                }

                dr["Core"] = coreName;
                dr["VoltageTest"] = txtVoltageTest50HzkV.Text.Trim();
                dr["CapacitanceMeasured"] = txtCapacitanceMeasurednF.Text.Trim();
                dr["CapacitanceResult"] = txtCapacitanceResultµF.Text.Trim();
                dr["TanDeltaMeasured"] = txtTanδMeasured.Text.Trim();
                dr["TanDeltaResult"] = txtTanδResult.Text.Trim();
                dr["TanDeltaStandard"] = txtTanδStandard.Text.Trim();

                dt.Rows.Add(dr);
                TestTable = dt;

                gvTests.DataSource = dt;
                gvTests.DataBind();

                // Clear input fields
                txtVoltageTest50HzkV.Text = "";
                txtCapacitanceMeasurednF.Text = "";
                txtCapacitanceResultµF.Text = "";
                txtTanδMeasured.Text = "";
                txtTanδResult.Text = "";
                txtTanδStandard.Text = "";
            }
        }

        // Add rows to Test 2
        protected void btnAdd2_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                DataTable dt = TestTable2;
                DataRow dr = dt.NewRow();

                string temperatureType = ddlTempType.SelectedItem.Text;
                if (temperatureType == "-- Select Temperature Type --")
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", "alert('Please select a temperature type.');", true);
                    return;
                }

                string coreName = ddlGetCore2.SelectedItem.Text;
                if (coreName == "-- Select Core --")
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", "alert('Please select a core.');", true);
                    return;
                }

                dr["TemperatureType"] = temperatureType;
                dr["Core"] = coreName;
                dr["VoltageTest"] = txtVoltageTest50HzkV2.Text.Trim();
                dr["CapacitanceMeasured"] = txtCapacitanceMeasurednF2.Text.Trim();
                dr["CapacitanceResult"] = txtCapacitanceResultµF2.Text.Trim();
                dr["TanDeltaMeasured"] = txtTanδMeasured2.Text.Trim();
                dr["TanDeltaResult"] = txtTanδResult2.Text.Trim();
                dr["TanDeltaStandard"] = txtTanδStandard2.Text.Trim();

                dt.Rows.Add(dr);
                TestTable2 = dt;

                gvTests2.DataSource = dt;
                gvTests2.DataBind();

                // Clear input fields
                txtVoltageTest50HzkV2.Text = "";
                txtCapacitanceMeasurednF2.Text = "";
                txtCapacitanceResultµF2.Text = "";
                txtTanδMeasured2.Text = "";
                txtTanδResult2.Text = "";
                txtTanδStandard2.Text = "";
            }
        }

        // Row deleting for GridViews
        protected void gvTests_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            DataTable dt = TestTable;
            dt.Rows.RemoveAt(e.RowIndex);
            TestTable = dt;

            gvTests.DataSource = dt;
            gvTests.DataBind();
        }

        protected void gvTests2_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            DataTable dt = TestTable2;
            dt.Rows.RemoveAt(e.RowIndex);
            TestTable2 = dt;

            gvTests2.DataSource = dt;
            gvTests2.DataBind();
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
                        PrefillTestResultsGrid2(existingData); // New method for second test
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "PrefillError", $"alert('Error prefilling form: {ex.Message}');", true);
            }
        }

        private void PrefillTestResultsGrid(DataTable dt)
        {
            // Get characteristic data for Test 1
            txtTemperatureTestObject1.Text = GetResultValue(dt, "17298");
            txtLengthOfTestObject1.Text = GetResultValue(dt, "17299");
            txtStandardCapacitor1.Text = GetResultValue(dt, "17300");

            // Get CSV strings from the saved data for Test 1
            string coresCsv = GetResultValue(dt, "17301");
            string voltageTestsCsv = GetResultValue(dt, "17302");
            string capacitanceMeasuredCsv = GetResultValue(dt, "17303");
            string capacitanceResultCsv = GetResultValue(dt, "17304");
            string tanDeltaMeasuredCsv = GetResultValue(dt, "17305");
            string tanDeltaResultCsv = GetResultValue(dt, "17306");
            string tanDeltaStandardCsv = GetResultValue(dt, "17307");
            
            if (!string.IsNullOrEmpty(coresCsv))
            {
                DataTable gridDt = new DataTable();
                gridDt.Columns.Add("Core");
                gridDt.Columns.Add("VoltageTest");
                gridDt.Columns.Add("CapacitanceMeasured");
                gridDt.Columns.Add("CapacitanceResult");
                gridDt.Columns.Add("TanDeltaMeasured");
                gridDt.Columns.Add("TanDeltaResult");
                gridDt.Columns.Add("TanDeltaStandard");

                string[] cores = coresCsv.Split(',');
                string[] voltageTests = voltageTestsCsv.Split(',');
                string[] capacitanceMeasured = capacitanceMeasuredCsv.Split(',');
                string[] capacitanceResult = capacitanceResultCsv.Split(',');
                string[] tanDeltaMeasured = tanDeltaMeasuredCsv.Split(',');
                string[] tanDeltaResult = tanDeltaResultCsv.Split(',');
                string[] tanDeltaStandard = tanDeltaStandardCsv.Split(',');

                // Build rows from split arrays
                for (int i = 0; i < cores.Length; i++)
                {
                    DataRow dr = gridDt.NewRow();
                    dr["Core"] = cores[i].Trim();
                    dr["VoltageTest"] = (i < voltageTests.Length) ? voltageTests[i].Trim() : "";
                    dr["CapacitanceMeasured"] = (i < capacitanceMeasured.Length) ? capacitanceMeasured[i].Trim() : "";
                    dr["CapacitanceResult"] = (i < capacitanceResult.Length) ? capacitanceResult[i].Trim() : "";
                    dr["TanDeltaMeasured"] = (i < tanDeltaMeasured.Length) ? tanDeltaMeasured[i].Trim() : "";
                    dr["TanDeltaResult"] = (i < tanDeltaResult.Length) ? tanDeltaResult[i].Trim() : "";
                    dr["TanDeltaStandard"] = (i < tanDeltaStandard.Length) ? tanDeltaStandard[i].Trim() : "";
                    gridDt.Rows.Add(dr);
                }

                // Save to ViewState so Add/Delete works later
                TestTable = gridDt;
                // Bind to GridView
                gvTests.DataSource = gridDt;
                gvTests.DataBind();
            }
        }

        private void PrefillTestResultsGrid2(DataTable dt)
        {
            // Get characteristic data for Test 2
            txtTemperatureTestObject2.Text = GetResultValue(dt, "17308");
            txtLengthOfTestObject2.Text = GetResultValue(dt, "17309");
            txtStandardCapacitor2.Text = GetResultValue(dt, "17310");

            // Get CSV strings from the saved data for Test 2
            string temperatureTypesCsv = GetResultValue(dt, "17318");
            string coresCsv = GetResultValue(dt, "17311");
            string voltageTestsCsv = GetResultValue(dt, "17312");
            string capacitanceMeasuredCsv = GetResultValue(dt, "17313");
            string capacitanceResultCsv = GetResultValue(dt, "17314");
            string tanDeltaMeasuredCsv = GetResultValue(dt, "17315");
            string tanDeltaResultCsv = GetResultValue(dt, "17316");
            string tanDeltaStandardCsv = GetResultValue(dt, "17317");

            if (!string.IsNullOrEmpty(coresCsv))
            {
                DataTable gridDt = new DataTable();
                gridDt.Columns.Add("TemperatureType");
                gridDt.Columns.Add("Core");
                gridDt.Columns.Add("VoltageTest");
                gridDt.Columns.Add("CapacitanceMeasured");
                gridDt.Columns.Add("CapacitanceResult");
                gridDt.Columns.Add("TanDeltaMeasured");
                gridDt.Columns.Add("TanDeltaResult");
                gridDt.Columns.Add("TanDeltaStandard");

                string[] temperatureTypes = temperatureTypesCsv.Split(',');
                string[] cores = coresCsv.Split(',');
                string[] voltageTests = voltageTestsCsv.Split(',');
                string[] capacitanceMeasured = capacitanceMeasuredCsv.Split(',');
                string[] capacitanceResult = capacitanceResultCsv.Split(',');
                string[] tanDeltaMeasured = tanDeltaMeasuredCsv.Split(',');
                string[] tanDeltaResult = tanDeltaResultCsv.Split(',');
                string[] tanDeltaStandard = tanDeltaStandardCsv.Split(',');

                // Build rows from split arrays
                for (int i = 0; i < cores.Length; i++)
                {
                    DataRow dr = gridDt.NewRow();
                    dr["TemperatureType"] = (i < temperatureTypes.Length) ? temperatureTypes[i].Trim() : "";
                    dr["Core"] = cores[i].Trim();
                    dr["VoltageTest"] = (i < voltageTests.Length) ? voltageTests[i].Trim() : "";
                    dr["CapacitanceMeasured"] = (i < capacitanceMeasured.Length) ? capacitanceMeasured[i].Trim() : "";
                    dr["CapacitanceResult"] = (i < capacitanceResult.Length) ? capacitanceResult[i].Trim() : "";
                    dr["TanDeltaMeasured"] = (i < tanDeltaMeasured.Length) ? tanDeltaMeasured[i].Trim() : "";
                    dr["TanDeltaResult"] = (i < tanDeltaResult.Length) ? tanDeltaResult[i].Trim() : "";
                    dr["TanDeltaStandard"] = (i < tanDeltaStandard.Length) ? tanDeltaStandard[i].Trim() : "";
                    gridDt.Rows.Add(dr);
                }
                // Save to ViewState so Add/Delete works later
                TestTable2 = gridDt;
                // Bind to GridView
                gvTests2.DataSource = gridDt;
                gvTests2.DataBind();
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

                // Upper section data
                reportParamMapList.Add("17284"); resultList.Add(cableType.Text);
                reportParamMapList.Add("17285"); resultList.Add(cableSize.Text);
                reportParamMapList.Add("17286"); resultList.Add(voltage.Text);
                reportParamMapList.Add("17287"); resultList.Add(Date.Text);
                reportParamMapList.Add("17288"); resultList.Add(roomno.Text);
                reportParamMapList.Add("17289"); resultList.Add(roomtemp.Text);
                reportParamMapList.Add("17290"); resultList.Add(refSpec.Text);
                reportParamMapList.Add("17291"); resultList.Add(testMethodNo.Text);
                reportParamMapList.Add("17292"); resultList.Add(jobOrderNo.Text);
                reportParamMapList.Add("17293"); resultList.Add(worksheetno.Text);
                reportParamMapList.Add("17294"); resultList.Add(roomhumidity.Text);
                reportParamMapList.Add("17295"); resultList.Add(equipmentno.Text);

                // Test 1 characteristic data
                reportParamMapList.Add("17298"); resultList.Add(txtTemperatureTestObject1.Text);
                reportParamMapList.Add("17299"); resultList.Add(txtLengthOfTestObject1.Text);
                reportParamMapList.Add("17300"); resultList.Add(txtStandardCapacitor1.Text);

                // Test 1 results
                DataTable dt = TestTable;
                if (dt == null || dt.Rows.Count == 0)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Record Not Found!');</script>");
                }
                else
                {
                    List<string> cores1 = new List<string>();
                    List<string> voltageTests1 = new List<string>();
                    List<string> capacitanceMeasured1 = new List<string>();
                    List<string> capacitanceResult1 = new List<string>();
                    List<string> tanDeltaMeasured1 = new List<string>();
                    List<string> tanDeltaResult1 = new List<string>();
                    List<string> tanDeltaStandard1 = new List<string>();

                    foreach (DataRow row in dt.Rows)
                    {
                        cores1.Add(row["Core"].ToString());
                        voltageTests1.Add(row["VoltageTest"].ToString());
                        capacitanceMeasured1.Add(row["CapacitanceMeasured"].ToString());
                        capacitanceResult1.Add(row["CapacitanceResult"].ToString());
                        tanDeltaMeasured1.Add(row["TanDeltaMeasured"].ToString());
                        tanDeltaResult1.Add(row["TanDeltaResult"].ToString());
                        tanDeltaStandard1.Add(row["TanDeltaStandard"].ToString());
                    }

                    string coresCsv1 = string.Join(",", cores1);
                    string voltageTestsCsv1 = string.Join(",", voltageTests1);
                    string capacitanceMeasuredCsv1 = string.Join(",", capacitanceMeasured1);
                    string capacitanceResultCsv1 = string.Join(",", capacitanceResult1);
                    string tanDeltaMeasuredCsv1 = string.Join(",", tanDeltaMeasured1);
                    string tanDeltaResultCsv1 = string.Join(",", tanDeltaResult1);
                    string tanDeltaStandardCsv1 = string.Join(",", tanDeltaStandard1);

                    reportParamMapList.Add("17301"); resultList.Add(coresCsv1);
                    reportParamMapList.Add("17302"); resultList.Add(voltageTestsCsv1);
                    reportParamMapList.Add("17303"); resultList.Add(capacitanceMeasuredCsv1);
                    reportParamMapList.Add("17304"); resultList.Add(capacitanceResultCsv1);
                    reportParamMapList.Add("17305"); resultList.Add(tanDeltaMeasuredCsv1);
                    reportParamMapList.Add("17306"); resultList.Add(tanDeltaResultCsv1);
                    reportParamMapList.Add("17307"); resultList.Add(tanDeltaStandardCsv1);

                    // Test 2 characteristic data
                    reportParamMapList.Add("17308"); resultList.Add(txtTemperatureTestObject2.Text);
                    reportParamMapList.Add("17309"); resultList.Add(txtLengthOfTestObject2.Text);
                    reportParamMapList.Add("17310"); resultList.Add(txtStandardCapacitor2.Text);


                    // Test 2 results
                    DataTable dt2 = TestTable2;
                    if (dt2 == null || dt2.Rows.Count == 0)
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Record Not Found!');</script>");
                    }
                    else
                    {
                        List<string> temperatureTypes2 = new List<string>();
                        List<string> cores2 = new List<string>();
                        List<string> voltageTests2 = new List<string>();
                        List<string> capacitanceMeasured2 = new List<string>();
                        List<string> capacitanceResult2 = new List<string>();
                        List<string> tanDeltaMeasured2 = new List<string>();
                        List<string> tanDeltaResult2 = new List<string>();
                        List<string> tanDeltaStandard2 = new List<string>();

                        foreach (DataRow row in dt2.Rows)
                        {
                            temperatureTypes2.Add(row["TemperatureType"].ToString());
                            cores2.Add(row["Core"].ToString());
                            voltageTests2.Add(row["VoltageTest"].ToString());
                            capacitanceMeasured2.Add(row["CapacitanceMeasured"].ToString());
                            capacitanceResult2.Add(row["CapacitanceResult"].ToString());
                            tanDeltaMeasured2.Add(row["TanDeltaMeasured"].ToString());
                            tanDeltaResult2.Add(row["TanDeltaResult"].ToString());
                            tanDeltaStandard2.Add(row["TanDeltaStandard"].ToString());
                        }

                        string temperatureTypesCsv2 = string.Join(",", temperatureTypes2);
                        string coresCsv2 = string.Join(",", cores2);
                        string voltageTestsCsv2 = string.Join(",", voltageTests2);
                        string capacitanceMeasuredCsv2 = string.Join(",", capacitanceMeasured2);
                        string capacitanceResultCsv2 = string.Join(",", capacitanceResult2);
                        string tanDeltaMeasuredCsv2 = string.Join(",", tanDeltaMeasured2);
                        string tanDeltaResultCsv2 = string.Join(",", tanDeltaResult2);
                        string tanDeltaStandardCsv2 = string.Join(",", tanDeltaStandard2);

                        reportParamMapList.Add("17318"); resultList.Add(temperatureTypesCsv2);
                        reportParamMapList.Add("17311"); resultList.Add(coresCsv2);
                        reportParamMapList.Add("17312"); resultList.Add(voltageTestsCsv2);
                        reportParamMapList.Add("17313"); resultList.Add(capacitanceMeasuredCsv2);
                        reportParamMapList.Add("17314"); resultList.Add(capacitanceResultCsv2);
                        reportParamMapList.Add("17315"); resultList.Add(tanDeltaMeasuredCsv2);
                        reportParamMapList.Add("17316"); resultList.Add(tanDeltaResultCsv2);
                        reportParamMapList.Add("17317"); resultList.Add(tanDeltaStandardCsv2);

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
                            Response.Redirect($"GenerateTestRecord_TanDeltaTest.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
                            return true;
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", "alert('Failed to save data.');", true);
                            return false;
                        }

                    }

                }


                return false;
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", $"alert('Error: {ex.Message}');", true);
                return false;
            }
        }

        private void PrefillUpperSection(DataTable existingData)
        {
            // Method to prefill upper section details based on parameter IDs
            SetLabelText(cableType, "17284");
            SetLabelText(cableSize, "17285");
            SetLabelText(voltage, "17286");
            SetTextBoxText(Date, "17287");
            SetTextBoxText(roomno, "17288");
            SetTextBoxText(roomtemp, "17289");
            SetLabelText(refSpec, "17290");
            SetLabelText(testMethodNo, "17291");
            SetLabelText(jobOrderNo, "17292");
            SetLabelText(worksheetno, "17293");
            SetTextBoxText(roomhumidity, "17294");
            SetLabelText(equipmentno, "17295");

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

        protected void btnClearData_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        protected void ClearForm()
        {
            //// Clear Specified values
            //txtSpecifiedValue1.Text = "";
            //txtSpecifiedValue2.Text = "";
            //txtSpecifiedValue3.Text = "";
            //txtSpecifiedValue4.Text = "";
            //txtSpecifiedValue5.Text = "";
            //txtSpecifiedValue6.Text = "";
            //txtSpecifiedValue7.Text = "";
            //txtSpecifiedValue8.Text = "";
            //txtSpecifiedValue9.Text = "";
            //txtSpecifiedValue10.Text = "";
            //txtSpecifiedValue11.Text = "";

            //// Clear Test values
            //txtMeasured1.Text = "";
            //txtMeasured2.Text = "";
            //txtMeasured3.Text = "";
            //txtMeasured4.Text = "";
            //txtMeasured5.Text = "";
            //txtMeasured6.Text = "";
            //txtMeasured7.Text = "";
            //txtMeasured8.Text = "";
            //txtMeasured9.Text = "";
            //txtMeasured10.Text = "";
            //txtMeasured11.Text = "";

            //// Reset DropDowns (set to default or first item)
            //ddlUnit1.SelectedIndex = 0;
            //ddlUnit2.SelectedIndex = 0;
            //ddlUnit3.SelectedIndex = 0;
            //ddlUnit4.SelectedIndex = 0;
            //ddlUnit5.SelectedIndex = 0;
            //ddlUnit6.SelectedIndex = 0;
            //ddlUnit7.SelectedIndex = 0;
            //ddlUnit8.SelectedIndex = 0;
            //ddlUnit9.SelectedIndex = 0;
            //ddlUnit10.SelectedIndex = 0;
            //ddlUnit11.SelectedIndex = 0;
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


    }
}