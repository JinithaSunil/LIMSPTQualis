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
    public partial class PartialUBendingTest : System.Web.UI.Page
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
            // Method to prefill upper section details based on parameter IDs
            SetLabelText(cableType, "17144");
            SetLabelText(cableSize, "17145");
            SetLabelText(voltage, "17146");
            SetTextBoxText(Date, "17147");
            SetTextBoxText(roomno, "17148");
            SetTextBoxText(roomtemp, "17149");
            SetLabelText(refSpec, "17150");
            SetLabelText(testMethodNo, "17151");
            SetLabelText(jobOrderNo, "17152");
            SetLabelText(worksheetno, "17153");
            SetTextBoxText(roomhumidity, "17154");
            SetLabelText(equipmentno, "17155");

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
            // Create the same structure as TestTable
            DataTable gridDt = new DataTable();
            gridDt.Columns.Add("ItemTest");
            gridDt.Columns.Add("Unit");
            gridDt.Columns.Add("SpecifiedValue");
            gridDt.Columns.Add("MeasuredValue");

            // Get CSV strings from the saved data
            string itemTestsCsv = GetResultValue(dt, "17158");
            string unitsCsv = GetResultValue(dt, "17159");
            string specifiedValuesCsv = GetResultValue(dt, "17160");
            string measuredValuesCsv = GetResultValue(dt, "17161");

            if (!string.IsNullOrEmpty(itemTestsCsv))
            {
                string[] itemTests = itemTestsCsv.Split(',');
                string[] units = unitsCsv.Split(',');
                string[] specifiedValues = specifiedValuesCsv.Split(',');
                string[] measuredValues = measuredValuesCsv.Split(',');

                // Build rows from split arrays
                for (int i = 0; i < itemTests.Length; i++)
                {
                    DataRow dr = gridDt.NewRow();
                    dr["ItemTest"] = itemTests[i].Trim();
                    dr["Unit"] = (i < units.Length) ? units[i].Trim() : "";
                    dr["SpecifiedValue"] = (i < specifiedValues.Length) ? specifiedValues[i].Trim() : "";
                    dr["MeasuredValue"] = (i < measuredValues.Length) ? measuredValues[i].Trim() : "";
                    gridDt.Rows.Add(dr);
                }
            }

            // Save to ViewState so Add/Delete works later
            TestTable = gridDt;
            // Bind to GridView
            gvTests.DataSource = gridDt;
            gvTests.DataBind();
        }

        // Helper to get a single value from dt by ParameterID
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

                reportParamMapList.Add("17144");resultList.Add(cableType.Text);
                reportParamMapList.Add("17145"); resultList.Add(cableSize.Text);
                reportParamMapList.Add("17146"); resultList.Add(voltage.Text);
                reportParamMapList.Add("17147"); resultList.Add(Date.Text);
                reportParamMapList.Add("17148"); resultList.Add(roomno.Text);
                reportParamMapList.Add("17149"); resultList.Add(roomtemp.Text);
                reportParamMapList.Add("17150"); resultList.Add(refSpec.Text);
                reportParamMapList.Add("17151"); resultList.Add(testMethodNo.Text);
                reportParamMapList.Add("17152"); resultList.Add(jobOrderNo.Text);
                reportParamMapList.Add("17153"); resultList.Add(worksheetno.Text);
                reportParamMapList.Add("17154"); resultList.Add(roomhumidity.Text);
                reportParamMapList.Add("17155"); resultList.Add(equipmentno.Text);

                DataTable dt = TestTable;
                List<string> itemTests = new List<string>();
                List<string> units = new List<string>();
                List<string> specifiedValues = new List<string>();
                List<string> measuredValues = new List<string>();

                foreach (DataRow row in dt.Rows)
                {
                    itemTests.Add(row["ItemTest"].ToString());
                    units.Add(row["Unit"].ToString());
                    specifiedValues.Add(row["SpecifiedValue"].ToString());
                    measuredValues.Add(row["MeasuredValue"].ToString());
                }

                string itemTestsCsv = string.Join(",", itemTests);
                string unitsCsv = string.Join(",", units);
                string specifiedValuesCsv = string.Join(",", specifiedValues);
                string measuredValuesCsv = string.Join(",", measuredValues);

                reportParamMapList.Add("17158"); resultList.Add(itemTestsCsv);
                reportParamMapList.Add("17159"); resultList.Add(unitsCsv);
                reportParamMapList.Add("17160"); resultList.Add(specifiedValuesCsv);
                reportParamMapList.Add("17161"); resultList.Add(measuredValuesCsv);

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
                    Response.Redirect($"GenerateTestRecord_PartialUBendingTest.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
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

        //New Code//
        private DataTable TestTable
        {
            get
            {
                if (ViewState["TestTable"] == null)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("ItemTest");
                    dt.Columns.Add("Unit");
                    dt.Columns.Add("SpecifiedValue");
                    dt.Columns.Add("MeasuredValue");
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
            dr["ItemTest"] = txtItemTest.Text.Trim();
            dr["Unit"] = ddlUnit1.SelectedValue;
            dr["SpecifiedValue"] = txtSpecifiedValue1.Text.Trim();
            dr["MeasuredValue"] = txtMeasured1.Text.Trim();
            dt.Rows.Add(dr);

            TestTable = dt; // Save back to ViewState

            gvTests.DataSource = dt;
            gvTests.DataBind();

            // Optional: clear fields
            txtItemTest.Text = "";
            txtSpecifiedValue1.Text = "";
            txtMeasured1.Text = "";
        }
        
        protected void gvTests_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            DataTable dt = TestTable;
            dt.Rows.RemoveAt(e.RowIndex);
            TestTable = dt;

            gvTests.DataSource = dt;
            gvTests.DataBind();
        }




        //protected void gvTests_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        int serialNumber = e.Row.RowIndex + 1 + (gvTests.PageIndex * gvTests.PageSize);
        //        e.Row.Cells[0].Text = serialNumber.ToString();
        //    }
        //}

        //protected void btnSave_Click(object sender, EventArgs e)
        //{
        //    DataTable dt = TestTable;
        //    // Collect values in lists
        //    List<string> itemTests = new List<string>();
        //    List<string> units = new List<string>();
        //    List<string> specifiedValues = new List<string>();
        //    List<string> measuredValues = new List<string>();

        //    foreach (DataRow row in dt.Rows)
        //    {
        //        itemTests.Add(row["ItemTest"].ToString());
        //        units.Add(row["Unit"].ToString());
        //        specifiedValues.Add(row["SpecifiedValue"].ToString());
        //        measuredValues.Add(row["MeasuredValue"].ToString());
        //    }

        //    // Join with commas
        //    string itemTestsCsv = string.Join(",", itemTests);
        //    string unitsCsv = string.Join(",", units);
        //    string specifiedValuesCsv = string.Join(",", specifiedValues);
        //    string measuredValuesCsv = string.Join(",", measuredValues);

        //    //using (SqlConnection con = new SqlConnection("YourConnectionStringHere"))
        //    //{
        //    //    con.Open();
        //    //    using (SqlCommand cmd = new SqlCommand(@"INSERT INTO YourTable 
        //    //                                            (ItemTest, Unit, SpecifiedValue, MeasuredValue) 
        //    //                                            VALUES (@ItemTest, @Unit, @SpecifiedValue, @MeasuredValue)", con))
        //    //    {
        //    //        cmd.Parameters.AddWithValue("@ItemTest", itemTestsCsv);
        //    //        cmd.Parameters.AddWithValue("@Unit", unitsCsv);
        //    //        cmd.Parameters.AddWithValue("@SpecifiedValue", specifiedValuesCsv);
        //    //        cmd.Parameters.AddWithValue("@MeasuredValue", measuredValuesCsv);

        //    //        cmd.ExecuteNonQuery();
        //    //    }
        //    //}

        //    // Clear after save
        //    TestTable.Clear();
        //    gvTests.DataSource = TestTable;
        //    gvTests.DataBind();
        //}

        //protected void btnSave_Click(object sender, EventArgs e)
        //{
        //    DataTable dt = TestTable;

        //    using (SqlConnection con = new SqlConnection("YourConnectionStringHere"))
        //    {
        //        con.Open();
        //        foreach (DataRow row in dt.Rows)
        //        {
        //            using (SqlCommand cmd = new SqlCommand(@"INSERT INTO YourTable (ItemTest, Unit, SpecifiedValue, MeasuredValue) 
        //                                             VALUES (@ItemTest, @Unit, @SpecifiedValue, @MeasuredValue)", con))
        //            {
        //                cmd.Parameters.AddWithValue("@ItemTest", row["ItemTest"]);
        //                cmd.Parameters.AddWithValue("@Unit", row["Unit"]);
        //                cmd.Parameters.AddWithValue("@SpecifiedValue", row["SpecifiedValue"]);
        //                cmd.Parameters.AddWithValue("@MeasuredValue", row["MeasuredValue"]);
        //                cmd.ExecuteNonQuery();
        //            }
        //        }
        //    }

        //    // Clear after save
        //    TestTable.Clear();
        //    gvTests.DataSource = TestTable;
        //    gvTests.DataBind();
        //}


    }
}