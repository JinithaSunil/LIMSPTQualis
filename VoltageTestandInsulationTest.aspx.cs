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
    public partial class VoltageTestandInsulationTest : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Initialize DAL object
                if (_cm == null) _cm = new DAL.CM();
                PopulateCoreDropdown();
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
                    lblvoltage.Text = dt1.Rows[0]["Voltage"].ToString();
                    cableSize.Text = dt1.Rows[0]["SizeOfSample"].ToString();
                    testMethodNo.Text = dt1.Rows[0]["TestMethod"].ToString();
                    refSpec.Text = dt1.Rows[0]["TestMethod"].ToString();

                    //upper section
                    roomno.Text = "SHIELDING";
                    jobOrderNo.Text = "WO/2024120905";
                    worksheetno.Text = "-";
                    equipmentno.Text = "QE/15/EL/34";
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "showError", "alert('Could not retrieve sample data. Please check your sample ID.');", true);
                }

                // Check and prefill form if sample ID exists
                CheckAndPrefillForm();
            }
        }

        //Buttons
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            List<string> errors = new List<string>();
            List<Control> invalidControls = new List<Control>();

            // Check GridView counts for all tests
            bool hasTest1Data = gv1.Rows.Count > 0;
            bool hasTest2Data = gv2.Rows.Count > 0;
            bool hasTest3Data = gv3.Rows.Count > 0;
            bool hasTest4Data = gv4.Rows.Count > 0;
            bool hasTest5Data = gv5.Rows.Count > 0;
            bool hasTest6Data = gv6.Rows.Count > 0;
            bool hasTest7Data = gv7.Rows.Count > 0;

            // Validate Main Info (from your existing code)
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
                errors.Add("Room Temperature must be a valid number.");
                invalidControls.Add(roomtemp);
            }

            // Test 1 Validation (Voltage Test on complete cable)
            if (!hasTest1Data)
            {
                if (string.IsNullOrEmpty(testdate1.Text))
                {
                    errors.Add("Test 1 Date is required.");
                    invalidControls.Add(testdate1);
                }
                if (ddlCore1.SelectedIndex == 0)
                {
                    errors.Add("Select a Core for Test 1.");
                    invalidControls.Add(ddlCore1);
                }
                if (string.IsNullOrEmpty(txtLength1.Text) || !double.TryParse(txtLength1.Text, out _))
                {
                    errors.Add("Length (Test 1) must be a valid number.");
                    invalidControls.Add(txtLength1);
                }
                if (string.IsNullOrEmpty(txtImmerse1.Text) || !double.TryParse(txtImmerse1.Text, out _))
                {
                    errors.Add("Immerse (Test 1) must be a valid number.");
                    invalidControls.Add(txtImmerse1);
                }
                if (string.IsNullOrEmpty(txtTemp1.Text) || !double.TryParse(txtTemp1.Text, out _))
                {
                    errors.Add("Temperature (Test 1) must be a valid number.");
                    invalidControls.Add(txtTemp1);
                }
                if (string.IsNullOrEmpty(txtVoltage1.Text) || !double.TryParse(txtVoltage1.Text, out _))
                {
                    errors.Add("Voltage (Test 1) must be a valid number.");
                    invalidControls.Add(txtVoltage1);
                }
                if (string.IsNullOrEmpty(txtTime1.Text) || !double.TryParse(txtTime1.Text, out _))
                {
                    errors.Add("Time (Test 1) must be a valid number.");
                    invalidControls.Add(txtTime1);
                }
                if (string.IsNullOrEmpty(txtresults1.Text))
                {
                    errors.Add("Result (Test 1) is required.");
                    invalidControls.Add(txtresults1);
                }
            }

            // Test 2 Validation (Voltage Test on insulation)
            if (!hasTest2Data)
            {
                if (string.IsNullOrEmpty(testdate2.Text))
                {
                    errors.Add("Test 2 Date is required.");
                    invalidControls.Add(testdate2);
                }
                if (ddlCore2.SelectedIndex == 0)
                {
                    errors.Add("Select a Core for Test 2.");
                    invalidControls.Add(ddlCore2);
                }
                if (string.IsNullOrEmpty(txtLength2.Text) || !double.TryParse(txtLength2.Text, out _))
                {
                    errors.Add("Length (Test 2) must be a valid number.");
                    invalidControls.Add(txtLength2);
                }
                if (string.IsNullOrEmpty(txtImmerse2.Text) || !double.TryParse(txtImmerse2.Text, out _))
                {
                    errors.Add("Immerse (Test 2) must be a valid number.");
                    invalidControls.Add(txtImmerse2);
                }
                if (string.IsNullOrEmpty(txtTemp2.Text) || !double.TryParse(txtTemp2.Text, out _))
                {
                    errors.Add("Temperature (Test 2) must be a valid number.");
                    invalidControls.Add(txtTemp2);
                }
                if (string.IsNullOrEmpty(txtVoltage2.Text) || !double.TryParse(txtVoltage2.Text, out _))
                {
                    errors.Add("Voltage (Test 2) must be a valid number.");
                    invalidControls.Add(txtVoltage2);
                }
                if (string.IsNullOrEmpty(txtTime2.Text) || !double.TryParse(txtTime2.Text, out _))
                {
                    errors.Add("Time (Test 2) must be a valid number.");
                    invalidControls.Add(txtTime2);
                }
                if (string.IsNullOrEmpty(txtresults2.Text))
                {
                    errors.Add("Result (Test 2) is required.");
                    invalidControls.Add(txtresults2);
                }
            }

            // Test 3 Validation (Insulation Resistance at ambient temperature)
            if (!hasTest3Data)
            {
                if (string.IsNullOrEmpty(testdate3.Text))
                {
                    errors.Add("Test 3 Date is required.");
                    invalidControls.Add(testdate3);
                }
                if (ddlCore3.SelectedIndex == 0)
                {
                    errors.Add("Select a Core for Test 3.");
                    invalidControls.Add(ddlCore3);
                }
                if (string.IsNullOrEmpty(txtLength3.Text) || !double.TryParse(txtLength3.Text, out _))
                {
                    errors.Add("Length (Test 3) must be a valid number.");
                    invalidControls.Add(txtLength3);
                }
                if (string.IsNullOrEmpty(txtImmerse3.Text) || !double.TryParse(txtImmerse3.Text, out _))
                {
                    errors.Add("Immerse (Test 3) must be a valid number.");
                    invalidControls.Add(txtImmerse3);
                }
                if (string.IsNullOrEmpty(txtTemp3.Text) || !double.TryParse(txtTemp3.Text, out _))
                {
                    errors.Add("Temperature (Test 3) must be a valid number.");
                    invalidControls.Add(txtTemp3);
                }
                if (string.IsNullOrEmpty(txtMeasurement3.Text) || !double.TryParse(txtMeasurement3.Text, out _))
                {
                    errors.Add("Measurement (Test 3) must be a valid number.");
                    invalidControls.Add(txtMeasurement3);
                }
                if (string.IsNullOrEmpty(txtInsulationResistance3.Text) || !double.TryParse(txtInsulationResistance3.Text, out _))
                {
                    errors.Add("Insulation Resistance (Test 3) must be a valid number.");
                    invalidControls.Add(txtInsulationResistance3);
                }
            }

            // Test 4 Validation (Insulation Resistance at max. conductor temperature)
            if (!hasTest4Data)
            {
                if (string.IsNullOrEmpty(testdate4.Text))
                {
                    errors.Add("Test 4 Date is required.");
                    invalidControls.Add(testdate4);
                }
                if (ddlCore4.SelectedIndex == 0)
                {
                    errors.Add("Select a Core for Test 4.");
                    invalidControls.Add(ddlCore4);
                }
                if (string.IsNullOrEmpty(txtLength4.Text) || !double.TryParse(txtLength4.Text, out _))
                {
                    errors.Add("Length (Test 4) must be a valid number.");
                    invalidControls.Add(txtLength4);
                }
                if (string.IsNullOrEmpty(txtImmerse4.Text) || !double.TryParse(txtImmerse4.Text, out _))
                {
                    errors.Add("Immerse (Test 4) must be a valid number.");
                    invalidControls.Add(txtImmerse4);
                }
                if (string.IsNullOrEmpty(txtTemp4.Text) || !double.TryParse(txtTemp4.Text, out _))
                {
                    errors.Add("Temperature (Test 4) must be a valid number.");
                    invalidControls.Add(txtTemp4);
                }
                if (string.IsNullOrEmpty(txtFactor4.Text) || !double.TryParse(txtFactor4.Text, out _))
                {
                    errors.Add("Factor correction (Test 4) must be a valid number.");
                    invalidControls.Add(txtFactor4);
                }
                if (string.IsNullOrEmpty(txtMeasurement4.Text) || !double.TryParse(txtMeasurement4.Text, out _))
                {
                    errors.Add("Measurement (Test 4) must be a valid number.");
                    invalidControls.Add(txtMeasurement4);
                }
                if (string.IsNullOrEmpty(txtInsulationResistance4.Text) || !double.TryParse(txtInsulationResistance4.Text, out _))
                {
                    errors.Add("Insulation Resistance (Test 4) must be a valid number.");
                    invalidControls.Add(txtInsulationResistance4);
                }
            }

            // Test 5 Validation (Inner & Outer diameter of insulation)
            if (!hasTest5Data)
            {
                if (string.IsNullOrEmpty(testdate5.Text))
                {
                    errors.Add("Test 5 Date is required.");
                    invalidControls.Add(testdate5);
                }
                if (ddlCore5.SelectedIndex == 0)
                {
                    errors.Add("Select a Core for Test 5.");
                    invalidControls.Add(ddlCore5);
                }
                if (string.IsNullOrEmpty(txtLength5.Text) || !double.TryParse(txtLength5.Text, out _))
                {
                    errors.Add("Length (Test 5) must be a valid number.");
                    invalidControls.Add(txtLength5);
                }
                if (string.IsNullOrEmpty(txtInner1.Text) || !double.TryParse(txtInner1.Text, out _))
                {
                    errors.Add("Inner diameter 1 (Test 5) must be a valid number.");
                    invalidControls.Add(txtInner1);
                }
                if (string.IsNullOrEmpty(txtInner2.Text) || !double.TryParse(txtInner2.Text, out _))
                {
                    errors.Add("Inner diameter 2 (Test 5) must be a valid number.");
                    invalidControls.Add(txtInner2);
                }
                if (string.IsNullOrEmpty(txtInnerAvg.Text) || !double.TryParse(txtInnerAvg.Text, out _))
                {
                    errors.Add("Inner average (Test 5) must be a valid number.");
                    invalidControls.Add(txtInnerAvg);
                }
                if (string.IsNullOrEmpty(txtOuter1.Text) || !double.TryParse(txtOuter1.Text, out _))
                {
                    errors.Add("Outer diameter 1 (Test 5) must be a valid number.");
                    invalidControls.Add(txtOuter1);
                }
                if (string.IsNullOrEmpty(txtOuter2.Text) || !double.TryParse(txtOuter2.Text, out _))
                {
                    errors.Add("Outer diameter 2 (Test 5) must be a valid number.");
                    invalidControls.Add(txtOuter2);
                }
                if (string.IsNullOrEmpty(txtOuterAvg.Text) || !double.TryParse(txtOuterAvg.Text, out _))
                {
                    errors.Add("Outer average (Test 5) must be a valid number.");
                    invalidControls.Add(txtOuterAvg);
                }
            }

            // Test 6 Validation (Volume Resistivity, KI at ambient temperature)
            if (!hasTest6Data)
            {
                if (string.IsNullOrEmpty(testdate6.Text))
                {
                    errors.Add("Test 6 Date is required.");
                    invalidControls.Add(testdate6);
                }
                if (ddlCore6.SelectedIndex == 0)
                {
                    errors.Add("Select a Core for Test 6.");
                    invalidControls.Add(ddlCore6);
                }
                if (string.IsNullOrEmpty(txtTemp6.Text) || !double.TryParse(txtTemp6.Text, out _))
                {
                    errors.Add("Temperature (Test 6) must be a valid number.");
                    invalidControls.Add(txtTemp6);
                }
                if (string.IsNullOrEmpty(txtInsulationResistance.Text) || !double.TryParse(txtInsulationResistance.Text, out _))
                {
                    errors.Add("Insulation resistance (Test 6) must be a valid number.");
                    invalidControls.Add(txtInsulationResistance);
                }
                if (string.IsNullOrEmpty(txtVolumeResistivity.Text) || !double.TryParse(txtVolumeResistivity.Text, out _))
                {
                    errors.Add("Volume resistivity (Test 6) must be a valid number.");
                    invalidControls.Add(txtVolumeResistivity);
                }
                if (string.IsNullOrEmpty(txtInsulationConstant.Text) || !double.TryParse(txtInsulationConstant.Text, out _))
                {
                    errors.Add("Insulation constant (Test 6) must be a valid number.");
                    invalidControls.Add(txtInsulationConstant);
                }
            }

            // Test 7 Validation (Volume Resistivity, KI at max. conductor temperature)
            if (!hasTest7Data)
            {
                if (string.IsNullOrEmpty(testdate7.Text))
                {
                    errors.Add("Test 7 Date is required.");
                    invalidControls.Add(testdate7);
                }
                if (ddlCore7.SelectedIndex == 0)
                {
                    errors.Add("Select a Core for Test 7.");
                    invalidControls.Add(ddlCore7);
                }
                if (string.IsNullOrEmpty(txtTemp7.Text) || !double.TryParse(txtTemp7.Text, out _))
                {
                    errors.Add("Temperature (Test 7) must be a valid number.");
                    invalidControls.Add(txtTemp7);
                }
                if (string.IsNullOrEmpty(txtInsulationResistance7.Text) || !double.TryParse(txtInsulationResistance7.Text, out _))
                {
                    errors.Add("Insulation resistance (Test 7) must be a valid number.");
                    invalidControls.Add(txtInsulationResistance7);
                }
                if (string.IsNullOrEmpty(txtVolumeResistivity7.Text) || !double.TryParse(txtVolumeResistivity7.Text, out _))
                {
                    errors.Add("Volume resistivity (Test 7) must be a valid number.");
                    invalidControls.Add(txtVolumeResistivity7);
                }
                if (string.IsNullOrEmpty(txtInsulationConstant7.Text) || !double.TryParse(txtInsulationConstant7.Text, out _))
                {
                    errors.Add("Insulation constant (Test 7) must be a valid number.");
                    invalidControls.Add(txtInsulationConstant7);
                }
            }

            // Ensure at least one test set is filled
            if (!hasTest1Data && !hasTest2Data && !hasTest3Data && !hasTest4Data &&
                !hasTest5Data && !hasTest6Data && !hasTest7Data)
            {
                errors.Add("Add at least one test to any of the grids.");
            }

            // Reset all previous error styles
            ResetControlBorders();

            if (errors.Count > 0)
            {
                foreach (var control in invalidControls)
                {
                    if (control is TextBox txt)
                    {
                        txt.BorderColor = System.Drawing.Color.Red;
                        txt.BorderStyle = BorderStyle.Solid;
                        txt.BorderWidth = 1;
                    }
                    else if (control is DropDownList ddl)
                    {
                        ddl.BorderColor = System.Drawing.Color.Red;
                        ddl.BorderStyle = BorderStyle.Solid;
                        ddl.BorderWidth = 1;
                    }
                }

                string msg = "Validation Errors:\\n• " + string.Join("\\n• ", errors);
                ScriptManager.RegisterStartupScript(this, GetType(), "ValidationError",
                    $"alert('{msg}');", true);
            }
            else
            {
                Submitform();
                ScriptManager.RegisterStartupScript(this, GetType(), "SaveSuccess",
                    "alert('Test results and report saved successfully!');", true);
                ClearForm();
            }
        }
        protected void btnClearData_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            List<string> errors = new List<string>();
            List<Control> invalidControls = new List<Control>();

            // Check GridView counts for all tests
            bool hasTest1Data = gv1.Rows.Count > 0;
            bool hasTest2Data = gv2.Rows.Count > 0;
            bool hasTest3Data = gv3.Rows.Count > 0;
            bool hasTest4Data = gv4.Rows.Count > 0;
            bool hasTest5Data = gv5.Rows.Count > 0;
            bool hasTest6Data = gv6.Rows.Count > 0;
            bool hasTest7Data = gv7.Rows.Count > 0;

            // Validate Main Info (from your existing code)
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
                errors.Add("Room Temperature must be a valid number.");
                invalidControls.Add(roomtemp);
            }

            // Test 1 Validation (Voltage Test on complete cable)
            if (!hasTest1Data)
            {
                if (string.IsNullOrEmpty(testdate1.Text))
                {
                    errors.Add("Test 1 Date is required.");
                    invalidControls.Add(testdate1);
                }
                if (ddlCore1.SelectedIndex == 0)
                {
                    errors.Add("Select a Core for Test 1.");
                    invalidControls.Add(ddlCore1);
                }
                if (string.IsNullOrEmpty(txtLength1.Text) || !double.TryParse(txtLength1.Text, out _))
                {
                    errors.Add("Length (Test 1) must be a valid number.");
                    invalidControls.Add(txtLength1);
                }
                if (string.IsNullOrEmpty(txtImmerse1.Text) || !double.TryParse(txtImmerse1.Text, out _))
                {
                    errors.Add("Immerse (Test 1) must be a valid number.");
                    invalidControls.Add(txtImmerse1);
                }
                if (string.IsNullOrEmpty(txtTemp1.Text) || !double.TryParse(txtTemp1.Text, out _))
                {
                    errors.Add("Temperature (Test 1) must be a valid number.");
                    invalidControls.Add(txtTemp1);
                }
                if (string.IsNullOrEmpty(txtVoltage1.Text) || !double.TryParse(txtVoltage1.Text, out _))
                {
                    errors.Add("Voltage (Test 1) must be a valid number.");
                    invalidControls.Add(txtVoltage1);
                }
                if (string.IsNullOrEmpty(txtTime1.Text) || !double.TryParse(txtTime1.Text, out _))
                {
                    errors.Add("Time (Test 1) must be a valid number.");
                    invalidControls.Add(txtTime1);
                }
                if (string.IsNullOrEmpty(txtresults1.Text))
                {
                    errors.Add("Result (Test 1) is required.");
                    invalidControls.Add(txtresults1);
                }
            }

            // Test 2 Validation (Voltage Test on insulation)
            if (!hasTest2Data)
            {
                if (string.IsNullOrEmpty(testdate2.Text))
                {
                    errors.Add("Test 2 Date is required.");
                    invalidControls.Add(testdate2);
                }
                if (ddlCore2.SelectedIndex == 0)
                {
                    errors.Add("Select a Core for Test 2.");
                    invalidControls.Add(ddlCore2);
                }
                if (string.IsNullOrEmpty(txtLength2.Text) || !double.TryParse(txtLength2.Text, out _))
                {
                    errors.Add("Length (Test 2) must be a valid number.");
                    invalidControls.Add(txtLength2);
                }
                if (string.IsNullOrEmpty(txtImmerse2.Text) || !double.TryParse(txtImmerse2.Text, out _))
                {
                    errors.Add("Immerse (Test 2) must be a valid number.");
                    invalidControls.Add(txtImmerse2);
                }
                if (string.IsNullOrEmpty(txtTemp2.Text) || !double.TryParse(txtTemp2.Text, out _))
                {
                    errors.Add("Temperature (Test 2) must be a valid number.");
                    invalidControls.Add(txtTemp2);
                }
                if (string.IsNullOrEmpty(txtVoltage2.Text) || !double.TryParse(txtVoltage2.Text, out _))
                {
                    errors.Add("Voltage (Test 2) must be a valid number.");
                    invalidControls.Add(txtVoltage2);
                }
                if (string.IsNullOrEmpty(txtTime2.Text) || !double.TryParse(txtTime2.Text, out _))
                {
                    errors.Add("Time (Test 2) must be a valid number.");
                    invalidControls.Add(txtTime2);
                }
                if (string.IsNullOrEmpty(txtresults2.Text))
                {
                    errors.Add("Result (Test 2) is required.");
                    invalidControls.Add(txtresults2);
                }
            }

            // Test 3 Validation (Insulation Resistance at ambient temperature)
            if (!hasTest3Data)
            {
                if (string.IsNullOrEmpty(testdate3.Text))
                {
                    errors.Add("Test 3 Date is required.");
                    invalidControls.Add(testdate3);
                }
                if (ddlCore3.SelectedIndex == 0)
                {
                    errors.Add("Select a Core for Test 3.");
                    invalidControls.Add(ddlCore3);
                }
                if (string.IsNullOrEmpty(txtLength3.Text) || !double.TryParse(txtLength3.Text, out _))
                {
                    errors.Add("Length (Test 3) must be a valid number.");
                    invalidControls.Add(txtLength3);
                }
                if (string.IsNullOrEmpty(txtImmerse3.Text) || !double.TryParse(txtImmerse3.Text, out _))
                {
                    errors.Add("Immerse (Test 3) must be a valid number.");
                    invalidControls.Add(txtImmerse3);
                }
                if (string.IsNullOrEmpty(txtTemp3.Text) || !double.TryParse(txtTemp3.Text, out _))
                {
                    errors.Add("Temperature (Test 3) must be a valid number.");
                    invalidControls.Add(txtTemp3);
                }
                if (string.IsNullOrEmpty(txtMeasurement3.Text) || !double.TryParse(txtMeasurement3.Text, out _))
                {
                    errors.Add("Measurement (Test 3) must be a valid number.");
                    invalidControls.Add(txtMeasurement3);
                }
                if (string.IsNullOrEmpty(txtInsulationResistance3.Text) || !double.TryParse(txtInsulationResistance3.Text, out _))
                {
                    errors.Add("Insulation Resistance (Test 3) must be a valid number.");
                    invalidControls.Add(txtInsulationResistance3);
                }
            }

            // Test 4 Validation (Insulation Resistance at max. conductor temperature)
            if (!hasTest4Data)
            {
                if (string.IsNullOrEmpty(testdate4.Text))
                {
                    errors.Add("Test 4 Date is required.");
                    invalidControls.Add(testdate4);
                }
                if (ddlCore4.SelectedIndex == 0)
                {
                    errors.Add("Select a Core for Test 4.");
                    invalidControls.Add(ddlCore4);
                }
                if (string.IsNullOrEmpty(txtLength4.Text) || !double.TryParse(txtLength4.Text, out _))
                {
                    errors.Add("Length (Test 4) must be a valid number.");
                    invalidControls.Add(txtLength4);
                }
                if (string.IsNullOrEmpty(txtImmerse4.Text) || !double.TryParse(txtImmerse4.Text, out _))
                {
                    errors.Add("Immerse (Test 4) must be a valid number.");
                    invalidControls.Add(txtImmerse4);
                }
                if (string.IsNullOrEmpty(txtTemp4.Text) || !double.TryParse(txtTemp4.Text, out _))
                {
                    errors.Add("Temperature (Test 4) must be a valid number.");
                    invalidControls.Add(txtTemp4);
                }
                if (string.IsNullOrEmpty(txtFactor4.Text) || !double.TryParse(txtFactor4.Text, out _))
                {
                    errors.Add("Factor correction (Test 4) must be a valid number.");
                    invalidControls.Add(txtFactor4);
                }
                if (string.IsNullOrEmpty(txtMeasurement4.Text) || !double.TryParse(txtMeasurement4.Text, out _))
                {
                    errors.Add("Measurement (Test 4) must be a valid number.");
                    invalidControls.Add(txtMeasurement4);
                }
                if (string.IsNullOrEmpty(txtInsulationResistance4.Text) || !double.TryParse(txtInsulationResistance4.Text, out _))
                {
                    errors.Add("Insulation Resistance (Test 4) must be a valid number.");
                    invalidControls.Add(txtInsulationResistance4);
                }
            }

            // Test 5 Validation (Inner & Outer diameter of insulation)
            if (!hasTest5Data)
            {
                if (string.IsNullOrEmpty(testdate5.Text))
                {
                    errors.Add("Test 5 Date is required.");
                    invalidControls.Add(testdate5);
                }
                if (ddlCore5.SelectedIndex == 0)
                {
                    errors.Add("Select a Core for Test 5.");
                    invalidControls.Add(ddlCore5);
                }
                if (string.IsNullOrEmpty(txtLength5.Text) || !double.TryParse(txtLength5.Text, out _))
                {
                    errors.Add("Length (Test 5) must be a valid number.");
                    invalidControls.Add(txtLength5);
                }
                if (string.IsNullOrEmpty(txtInner1.Text) || !double.TryParse(txtInner1.Text, out _))
                {
                    errors.Add("Inner diameter 1 (Test 5) must be a valid number.");
                    invalidControls.Add(txtInner1);
                }
                if (string.IsNullOrEmpty(txtInner2.Text) || !double.TryParse(txtInner2.Text, out _))
                {
                    errors.Add("Inner diameter 2 (Test 5) must be a valid number.");
                    invalidControls.Add(txtInner2);
                }
                if (string.IsNullOrEmpty(txtInnerAvg.Text) || !double.TryParse(txtInnerAvg.Text, out _))
                {
                    errors.Add("Inner average (Test 5) must be a valid number.");
                    invalidControls.Add(txtInnerAvg);
                }
                if (string.IsNullOrEmpty(txtOuter1.Text) || !double.TryParse(txtOuter1.Text, out _))
                {
                    errors.Add("Outer diameter 1 (Test 5) must be a valid number.");
                    invalidControls.Add(txtOuter1);
                }
                if (string.IsNullOrEmpty(txtOuter2.Text) || !double.TryParse(txtOuter2.Text, out _))
                {
                    errors.Add("Outer diameter 2 (Test 5) must be a valid number.");
                    invalidControls.Add(txtOuter2);
                }
                if (string.IsNullOrEmpty(txtOuterAvg.Text) || !double.TryParse(txtOuterAvg.Text, out _))
                {
                    errors.Add("Outer average (Test 5) must be a valid number.");
                    invalidControls.Add(txtOuterAvg);
                }
            }

            // Test 6 Validation (Volume Resistivity, KI at ambient temperature)
            if (!hasTest6Data)
            {
                if (string.IsNullOrEmpty(testdate6.Text))
                {
                    errors.Add("Test 6 Date is required.");
                    invalidControls.Add(testdate6);
                }
                if (ddlCore6.SelectedIndex == 0)
                {
                    errors.Add("Select a Core for Test 6.");
                    invalidControls.Add(ddlCore6);
                }
                if (string.IsNullOrEmpty(txtTemp6.Text) || !double.TryParse(txtTemp6.Text, out _))
                {
                    errors.Add("Temperature (Test 6) must be a valid number.");
                    invalidControls.Add(txtTemp6);
                }
                if (string.IsNullOrEmpty(txtInsulationResistance.Text) || !double.TryParse(txtInsulationResistance.Text, out _))
                {
                    errors.Add("Insulation resistance (Test 6) must be a valid number.");
                    invalidControls.Add(txtInsulationResistance);
                }
                if (string.IsNullOrEmpty(txtVolumeResistivity.Text) || !double.TryParse(txtVolumeResistivity.Text, out _))
                {
                    errors.Add("Volume resistivity (Test 6) must be a valid number.");
                    invalidControls.Add(txtVolumeResistivity);
                }
                if (string.IsNullOrEmpty(txtInsulationConstant.Text) || !double.TryParse(txtInsulationConstant.Text, out _))
                {
                    errors.Add("Insulation constant (Test 6) must be a valid number.");
                    invalidControls.Add(txtInsulationConstant);
                }
            }

            // Test 7 Validation (Volume Resistivity, KI at max. conductor temperature)
            if (!hasTest7Data)
            {
                if (string.IsNullOrEmpty(testdate7.Text))
                {
                    errors.Add("Test 7 Date is required.");
                    invalidControls.Add(testdate7);
                }
                if (ddlCore7.SelectedIndex == 0)
                {
                    errors.Add("Select a Core for Test 7.");
                    invalidControls.Add(ddlCore7);
                }
                if (string.IsNullOrEmpty(txtTemp7.Text) || !double.TryParse(txtTemp7.Text, out _))
                {
                    errors.Add("Temperature (Test 7) must be a valid number.");
                    invalidControls.Add(txtTemp7);
                }
                if (string.IsNullOrEmpty(txtInsulationResistance7.Text) || !double.TryParse(txtInsulationResistance7.Text, out _))
                {
                    errors.Add("Insulation resistance (Test 7) must be a valid number.");
                    invalidControls.Add(txtInsulationResistance7);
                }
                if (string.IsNullOrEmpty(txtVolumeResistivity7.Text) || !double.TryParse(txtVolumeResistivity7.Text, out _))
                {
                    errors.Add("Volume resistivity (Test 7) must be a valid number.");
                    invalidControls.Add(txtVolumeResistivity7);
                }
                if (string.IsNullOrEmpty(txtInsulationConstant7.Text) || !double.TryParse(txtInsulationConstant7.Text, out _))
                {
                    errors.Add("Insulation constant (Test 7) must be a valid number.");
                    invalidControls.Add(txtInsulationConstant7);
                }
            }

            // Ensure at least one test set is filled
            if (!hasTest1Data && !hasTest2Data && !hasTest3Data && !hasTest4Data &&
                !hasTest5Data && !hasTest6Data && !hasTest7Data)
            {
                errors.Add("Add at least one test to any of the grids.");
            }

            // Reset all previous error styles
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

                string errorMessage = "Validation Errors:\\n• " + string.Join("\\n• ", errors);
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

                        int reportid = Convert.ToInt32(Request.QueryString["r"]);
                        if (_cm == null) _cm = new DAL.CM();
                        _cm.sampleId = sampleId;

                        DeleteExistingRecords(sampleId, reportid);

                        // Then save the updated data (reusing your Submitform logic)
                        Submitform();

                        // Show success message
                        ScriptManager.RegisterStartupScript(this, GetType(), "UpdateSuccess",
                            "alert('Test results updated successfully!');", true);
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

        //Grid buttons
        protected void btnAddToGrid1_Click(object sender, EventArgs e)
        {
            // Reset previous error styles
            ResetControlBordersAC();
            bool hasError = false;
            List<Control> invalidControls = new List<Control>();
            decimal length = 0, immerse = 0, temperature = 0, voltage = 0, time = 0;
            string result = txtresults1.Text.Trim();

            // Validate Date
            if (string.IsNullOrWhiteSpace(testdate1.Text))
            {
                invalidControls.Add(testdate1);
                hasError = true;
            }

            // Validate Core DropDownList
            if (ddlCore1.SelectedIndex == -1 || string.IsNullOrWhiteSpace(ddlCore1.SelectedValue))
            {
                // Handle DropDownList separately since it's not a TextBox
                ddlCore1.CssClass += " input-error";
                hasError = true;
            }

            // Validate Length
            if (string.IsNullOrWhiteSpace(txtLength1.Text) || !decimal.TryParse(txtLength1.Text, out length))
            {
                invalidControls.Add(txtLength1);
                hasError = true;
            }

            // Validate Immerse
            if (string.IsNullOrWhiteSpace(txtImmerse1.Text) || !decimal.TryParse(txtImmerse1.Text, out immerse))
            {
                invalidControls.Add(txtImmerse1);
                hasError = true;
            }

            // Validate Temperature
            if (string.IsNullOrWhiteSpace(txtTemp1.Text) || !decimal.TryParse(txtTemp1.Text, out temperature))
            {
                invalidControls.Add(txtTemp1);
                hasError = true;
            }

            // Validate Voltage
            if (string.IsNullOrWhiteSpace(txtVoltage1.Text) || !decimal.TryParse(txtVoltage1.Text, out voltage))
            {
                invalidControls.Add(txtVoltage1);
                hasError = true;
            }

            // Validate Time
            if (string.IsNullOrWhiteSpace(txtTime1.Text) || !decimal.TryParse(txtTime1.Text, out time))
            {
                invalidControls.Add(txtTime1);
                hasError = true;
            }

            // Highlight invalid controls with darker red border
            foreach (var control in invalidControls)
            {
                if (control is TextBox)
                {
                    ((TextBox)control).BorderColor = System.Drawing.Color.FromArgb(255, 150, 150); // Darker red
                    ((TextBox)control).BorderStyle = BorderStyle.Solid;
                    ((TextBox)control).BorderWidth = 1;
                }
            }

            if (hasError)
            {
                ShowMessage("Please fill in all required fields correctly.");
                return;
            }

            // Get the DataTable from ViewState
            DataTable dt = ViewState["ACTestResults"] as DataTable;
            if (dt == null)
            {
                // Initialize the DataTable if it doesn't exist
                dt = new DataTable();
                dt.Columns.Add("Date", typeof(string));
                dt.Columns.Add("Core", typeof(string));
                dt.Columns.Add("Length", typeof(decimal));
                dt.Columns.Add("Immerse", typeof(decimal));
                dt.Columns.Add("Temperature", typeof(decimal));
                dt.Columns.Add("Voltage", typeof(decimal));
                dt.Columns.Add("Time", typeof(decimal));
                dt.Columns.Add("Result", typeof(string));
                dt.Columns.Add("Specified", typeof(string));
            }

            // Create a new row
            DataRow dr = dt.NewRow();
            dr["Date"] = testdate1.Text;
            dr["Core"] = ddlCore1.SelectedItem.Text;
            dr["Length"] = length;
            dr["Immerse"] = immerse;
            dr["Temperature"] = temperature;
            dr["Voltage"] = voltage;
            dr["Time"] = time;
            dr["Result"] = result;
            dr["Specified"] = txtspecified1.Text.Trim();

            // Add the row to the DataTable
            dt.Rows.Add(dr);

            // Update the ViewState
            ViewState["ACTestResults"] = dt;

            // Rebind the GridView
            gv1.DataSource = dt;
            gv1.DataBind();

            // Clear the input fields for next entry
            ClearInputFieldsAC();

        }
        protected void btnAddToGrid2_Click(object sender, EventArgs e)
        {
            // Reset previous error styles
            ResetControlBordersDC();
            bool hasError = false;
            List<Control> invalidControls = new List<Control>();
            decimal length = 0, immerse = 0, temperature = 0, voltage = 0, time = 0;
            string result = txtresults2.Text.Trim();
            // Validate Date
            if (string.IsNullOrWhiteSpace(testdate2.Text))
            {
                invalidControls.Add(testdate2);
                hasError = true;
            }

            // Validate Core DropDownList
            if (ddlCore2.SelectedIndex == -1 || string.IsNullOrWhiteSpace(ddlCore2.SelectedValue))
            {
                // Handle DropDownList separately since it's not a TextBox
                ddlCore2.CssClass += " input-error";
                hasError = true;
            }

            // Validate Length
            if (string.IsNullOrWhiteSpace(txtLength2.Text) || !decimal.TryParse(txtLength2.Text, out length))
            {
                invalidControls.Add(txtLength2);
                hasError = true;
            }

            // Validate Immerse
            if (string.IsNullOrWhiteSpace(txtImmerse2.Text) || !decimal.TryParse(txtImmerse2.Text, out immerse))
            {
                invalidControls.Add(txtImmerse2);
                hasError = true;
            }

            // Validate Temperature
            if (string.IsNullOrWhiteSpace(txtTemp2.Text) || !decimal.TryParse(txtTemp2.Text, out temperature))
            {
                invalidControls.Add(txtTemp2);
                hasError = true;
            }

            // Validate Voltage
            if (string.IsNullOrWhiteSpace(txtVoltage2.Text) || !decimal.TryParse(txtVoltage2.Text, out voltage))
            {
                invalidControls.Add(txtVoltage2);
                hasError = true;
            }

            // Validate Time
            if (string.IsNullOrWhiteSpace(txtTime2.Text) || !decimal.TryParse(txtTime2.Text, out time))
            {
                invalidControls.Add(txtTime2);
                hasError = true;
            }

            // Highlight invalid controls with darker red border
            foreach (var control in invalidControls)
            {
                if (control is TextBox)
                {
                    ((TextBox)control).BorderColor = System.Drawing.Color.FromArgb(255, 150, 150); // Darker red
                    ((TextBox)control).BorderStyle = BorderStyle.Solid;
                    ((TextBox)control).BorderWidth = 1;
                }
            }

            if (hasError)
            {
                ShowMessage("Please fill in all required fields correctly.");
                return;
            }

            // Get the DataTable from ViewState
            DataTable dt = ViewState["DCTestResults"] as DataTable;
            if (dt == null)
            {
                // Initialize the DataTable if it doesn't exist
                dt = new DataTable();
                dt.Columns.Add("Date", typeof(string));
                dt.Columns.Add("Core", typeof(string));
                dt.Columns.Add("Length", typeof(decimal));
                dt.Columns.Add("Immerse", typeof(decimal));
                dt.Columns.Add("Temperature", typeof(decimal));
                dt.Columns.Add("Voltage", typeof(decimal));
                dt.Columns.Add("Time", typeof(decimal));
                dt.Columns.Add("Result", typeof(string));
                dt.Columns.Add("Specified", typeof(string));
            }

            // Create a new row
            DataRow dr = dt.NewRow();
            dr["Date"] = testdate2.Text;
            dr["Core"] = ddlCore2.SelectedItem.Text;
            dr["Length"] = length;
            dr["Immerse"] = immerse;
            dr["Temperature"] = temperature;
            dr["Voltage"] = voltage;
            dr["Time"] = time;
            dr["Result"] = result;
            dr["Specified"] = txtspecified2.Text.Trim();

            // Add the row to the DataTable
            dt.Rows.Add(dr);

            // Update the ViewState
            ViewState["DCTestResults"] = dt;

            // Rebind the GridView
            gv2.DataSource = dt;
            gv2.DataBind();

            // Clear the input fields for next entry
            ClearInputFieldsDC();

        }
        protected void btnAddToGrid3_Click(object sender, EventArgs e)
        {
            // Reset previous error styles
            ResetControlBordersDC();
            bool hasError = false;
            List<Control> invalidControls = new List<Control>();
            decimal length = 0, immerse = 0, temperature = 0, measurement = 0, insulationResistance = 0;

            // Validate Date
            if (string.IsNullOrWhiteSpace(testdate3.Text))
            {
                invalidControls.Add(testdate3);
                hasError = true;
            }

            // Validate Core DropDownList
            if (ddlCore3.SelectedIndex == -1 || string.IsNullOrWhiteSpace(ddlCore3.SelectedValue))
            {
                ddlCore3.CssClass += " input-error";
                hasError = true;
            }

            // Validate Length
            if (string.IsNullOrWhiteSpace(txtLength3.Text) || !decimal.TryParse(txtLength3.Text, out length))
            {
                invalidControls.Add(txtLength3);
                hasError = true;
            }

            // Validate Immerse
            if (string.IsNullOrWhiteSpace(txtImmerse3.Text) || !decimal.TryParse(txtImmerse3.Text, out immerse))
            {
                invalidControls.Add(txtImmerse3);
                hasError = true;
            }

            // Validate Temperature
            if (string.IsNullOrWhiteSpace(txtTemp3.Text) || !decimal.TryParse(txtTemp3.Text, out temperature))
            {
                invalidControls.Add(txtTemp3);
                hasError = true;
            }

            // Validate Measurement
            if (string.IsNullOrWhiteSpace(txtMeasurement3.Text) || !decimal.TryParse(txtMeasurement3.Text, out measurement))
            {
                invalidControls.Add(txtMeasurement3);
                hasError = true;
            }

            // Validate Insulation Resistance
            if (string.IsNullOrWhiteSpace(txtInsulationResistance3.Text) || !decimal.TryParse(txtInsulationResistance3.Text, out insulationResistance))
            {
                invalidControls.Add(txtInsulationResistance3);
                hasError = true;
            }

            // Highlight invalid controls
            foreach (var control in invalidControls)
            {
                if (control is TextBox)
                {
                    ((TextBox)control).BorderColor = System.Drawing.Color.FromArgb(255, 150, 150);
                    ((TextBox)control).BorderStyle = BorderStyle.Solid;
                    ((TextBox)control).BorderWidth = 1;
                }
            }

            if (hasError)
            {
                ShowMessage("Please fill in all required fields correctly.");
                return;
            }

            // Get the DataTable from ViewState
            DataTable dt = ViewState["InsulationAmbientTestResults"] as DataTable;
            if (dt == null)
            {
                // Initialize the DataTable if it doesn't exist
                dt = new DataTable();
                dt.Columns.Add("Date", typeof(string));
                dt.Columns.Add("Core", typeof(string));
                dt.Columns.Add("Length", typeof(decimal));
                dt.Columns.Add("Immerse", typeof(decimal));
                dt.Columns.Add("Temperature", typeof(decimal));
                dt.Columns.Add("Measurement", typeof(decimal));
                dt.Columns.Add("InsulationResistance", typeof(decimal));
                dt.Columns.Add("Specified", typeof(string));
            }

            // Create a new row
            DataRow dr = dt.NewRow();
            dr["Date"] = testdate3.Text;
            dr["Core"] = ddlCore3.SelectedItem.Text;
            dr["Length"] = length;
            dr["Immerse"] = immerse;
            dr["Temperature"] = temperature;
            dr["Measurement"] = measurement;
            dr["InsulationResistance"] = insulationResistance;
            dr["Specified"] = txtspecified3.Text.Trim();

            // Add the row to the DataTable
            dt.Rows.Add(dr);

            // Update the ViewState
            ViewState["InsulationAmbientTestResults"] = dt;

            // Rebind the GridView
            gv3.DataSource = dt;
            gv3.DataBind();

            // Clear the input fields for next entry
            ClearInputFields3();
        }
        protected void btnAddToGrid4_Click(object sender, EventArgs e)
        {
            // Reset previous error styles
            ResetControlBordersDC();
            bool hasError = false;
            List<Control> invalidControls = new List<Control>();
            decimal length = 0, immerse = 0, temperature = 0, factor = 0, measurement = 0, insulationResistance = 0;

            // Validate Date
            if (string.IsNullOrWhiteSpace(testdate4.Text))
            {
                invalidControls.Add(testdate4);
                hasError = true;
            }

            // Validate Core DropDownList
            if (ddlCore4.SelectedIndex == -1 || string.IsNullOrWhiteSpace(ddlCore4.SelectedValue))
            {
                ddlCore4.CssClass += " input-error";
                hasError = true;
            }

            // Validate Length
            if (string.IsNullOrWhiteSpace(txtLength4.Text) || !decimal.TryParse(txtLength4.Text, out length))
            {
                invalidControls.Add(txtLength4);
                hasError = true;
            }

            // Validate Immerse
            if (string.IsNullOrWhiteSpace(txtImmerse4.Text) || !decimal.TryParse(txtImmerse4.Text, out immerse))
            {
                invalidControls.Add(txtImmerse4);
                hasError = true;
            }

            // Validate Temperature
            if (string.IsNullOrWhiteSpace(txtTemp4.Text) || !decimal.TryParse(txtTemp4.Text, out temperature))
            {
                invalidControls.Add(txtTemp4);
                hasError = true;
            }

            // Validate Factor
            if (string.IsNullOrWhiteSpace(txtFactor4.Text) || !decimal.TryParse(txtFactor4.Text, out factor))
            {
                invalidControls.Add(txtFactor4);
                hasError = true;
            }

            // Validate Measurement
            if (string.IsNullOrWhiteSpace(txtMeasurement4.Text) || !decimal.TryParse(txtMeasurement4.Text, out measurement))
            {
                invalidControls.Add(txtMeasurement4);
                hasError = true;
            }

            // Validate Insulation Resistance
            if (string.IsNullOrWhiteSpace(txtInsulationResistance4.Text) || !decimal.TryParse(txtInsulationResistance4.Text, out insulationResistance))
            {
                invalidControls.Add(txtInsulationResistance4);
                hasError = true;
            }

            // Highlight invalid controls
            foreach (var control in invalidControls)
            {
                if (control is TextBox)
                {
                    ((TextBox)control).BorderColor = System.Drawing.Color.FromArgb(255, 150, 150);
                    ((TextBox)control).BorderStyle = BorderStyle.Solid;
                    ((TextBox)control).BorderWidth = 1;
                }
            }

            if (hasError)
            {
                ShowMessage("Please fill in all required fields correctly.");
                return;
            }

            // Get the DataTable from ViewState
            DataTable dt = ViewState["InsulationMaxTempTestResults"] as DataTable;
            if (dt == null)
            {
                // Initialize the DataTable if it doesn't exist
                dt = new DataTable();
                dt.Columns.Add("Date", typeof(string));
                dt.Columns.Add("Core", typeof(string));
                dt.Columns.Add("Length", typeof(decimal));
                dt.Columns.Add("Immerse", typeof(decimal));
                dt.Columns.Add("Temperature", typeof(decimal));
                dt.Columns.Add("Factor", typeof(decimal));
                dt.Columns.Add("Measurement", typeof(decimal));
                dt.Columns.Add("InsulationResistance", typeof(decimal));
                dt.Columns.Add("Specified", typeof(string));
            }

            // Create a new row
            DataRow dr = dt.NewRow();
            dr["Date"] = testdate4.Text;
            dr["Core"] = ddlCore4.SelectedItem.Text;
            dr["Length"] = length;
            dr["Immerse"] = immerse;
            dr["Temperature"] = temperature;
            dr["Factor"] = factor;
            dr["Measurement"] = measurement;
            dr["InsulationResistance"] = insulationResistance;
            dr["Specified"] = txtspecified4.Text.Trim();

            // Add the row to the DataTable
            dt.Rows.Add(dr);

            // Update the ViewState
            ViewState["InsulationMaxTempTestResults"] = dt;

            // Rebind the GridView
            gv4.DataSource = dt;
            gv4.DataBind();

            // Clear the input fields for next entry
            ClearInputFields4();
        }
        protected void btnAddToGrid5_Click(object sender, EventArgs e)
        {
            // Reset previous error styles
            ResetControlBorders5();
            bool hasError = false;
            List<Control> invalidControls = new List<Control>();
            decimal length = 0, inner1 = 0, inner2 = 0, innerAvg = 0, outer1 = 0, outer2 = 0, outerAvg = 0;

            // Validate Date
            if (string.IsNullOrWhiteSpace(testdate5.Text))
            {
                invalidControls.Add(testdate5);
                hasError = true;
            }

            // Validate Core DropDownList
            if (ddlCore5.SelectedIndex == -1 || string.IsNullOrWhiteSpace(ddlCore5.SelectedValue))
            {
                ddlCore5.CssClass += " input-error";
                hasError = true;
            }

            // Validate Length
            if (string.IsNullOrWhiteSpace(txtLength5.Text) || !decimal.TryParse(txtLength5.Text, out length))
            {
                invalidControls.Add(txtLength5);
                hasError = true;
            }

            // Validate Inner1
            if (string.IsNullOrWhiteSpace(txtInner1.Text) || !decimal.TryParse(txtInner1.Text, out inner1))
            {
                invalidControls.Add(txtInner1);
                hasError = true;
            }

            // Validate Inner2
            if (string.IsNullOrWhiteSpace(txtInner2.Text) || !decimal.TryParse(txtInner2.Text, out inner2))
            {
                invalidControls.Add(txtInner2);
                hasError = true;
            }

            // Validate InnerAvg
            if (string.IsNullOrWhiteSpace(txtInnerAvg.Text) || !decimal.TryParse(txtInnerAvg.Text, out innerAvg))
            {
                invalidControls.Add(txtInnerAvg);
                hasError = true;
            }

            // Validate Outer1
            if (string.IsNullOrWhiteSpace(txtOuter1.Text) || !decimal.TryParse(txtOuter1.Text, out outer1))
            {
                invalidControls.Add(txtOuter1);
                hasError = true;
            }

            // Validate Outer2
            if (string.IsNullOrWhiteSpace(txtOuter2.Text) || !decimal.TryParse(txtOuter2.Text, out outer2))
            {
                invalidControls.Add(txtOuter2);
                hasError = true;
            }

            // Validate OuterAvg
            if (string.IsNullOrWhiteSpace(txtOuterAvg.Text) || !decimal.TryParse(txtOuterAvg.Text, out outerAvg))
            {
                invalidControls.Add(txtOuterAvg);
                hasError = true;
            }

            // Highlight invalid controls with darker red border
            foreach (var control in invalidControls)
            {
                if (control is TextBox)
                {
                    ((TextBox)control).BorderColor = System.Drawing.Color.FromArgb(255, 150, 150);
                    ((TextBox)control).BorderStyle = BorderStyle.Solid;
                    ((TextBox)control).BorderWidth = 1;
                }
            }

            if (hasError)
            {
                ShowMessage("Please fill in all required fields correctly.");
                return;
            }

            // Get the DataTable from ViewState
            DataTable dt = ViewState["DiameterTestResults"] as DataTable;
            if (dt == null)
            {
                // Initialize the DataTable if it doesn't exist
                dt = new DataTable();
                dt.Columns.Add("Date", typeof(string));
                dt.Columns.Add("Core", typeof(string));
                dt.Columns.Add("Length", typeof(decimal));
                dt.Columns.Add("Inner1", typeof(decimal));
                dt.Columns.Add("Inner2", typeof(decimal));
                dt.Columns.Add("InnerAvg", typeof(decimal));
                dt.Columns.Add("Outer1", typeof(decimal));
                dt.Columns.Add("Outer2", typeof(decimal));
                dt.Columns.Add("OuterAvg", typeof(decimal));
            }

            // Create a new row
            DataRow dr = dt.NewRow();
            dr["Date"] = testdate5.Text;
            dr["Core"] = ddlCore5.SelectedItem.Text;
            dr["Length"] = length;
            dr["Inner1"] = inner1;
            dr["Inner2"] = inner2;
            dr["InnerAvg"] = innerAvg;
            dr["Outer1"] = outer1;
            dr["Outer2"] = outer2;
            dr["OuterAvg"] = outerAvg;

            // Add the row to the DataTable
            dt.Rows.Add(dr);

            // Update the ViewState
            ViewState["DiameterTestResults"] = dt;

            // Rebind the GridView
            gv5.DataSource = dt;
            gv5.DataBind();

            // Clear the input fields for next entry
            ClearInputFields5();
        }
        protected void btnAddToGrid6_Click(object sender, EventArgs e)
        {
            // Reset previous error styles
            ResetControlBorders6();
            bool hasError = false;
            List<Control> invalidControls = new List<Control>();
            decimal temperature = 0, insulationResistance = 0, volumeResistivity = 0, insulationConstant = 0;

            // Validate Date
            if (string.IsNullOrWhiteSpace(testdate6.Text))
            {
                invalidControls.Add(testdate6);
                hasError = true;
            }

            // Validate Core DropDownList
            if (ddlCore6.SelectedIndex == -1 || string.IsNullOrWhiteSpace(ddlCore6.SelectedValue))
            {
                ddlCore6.CssClass += " input-error";
                hasError = true;
            }

            // Validate Temperature
            if (string.IsNullOrWhiteSpace(txtTemp6.Text) || !decimal.TryParse(txtTemp6.Text, out temperature))
            {
                invalidControls.Add(txtTemp6);
                hasError = true;
            }

            // Validate Insulation Resistance
            if (string.IsNullOrWhiteSpace(txtInsulationResistance.Text) || !decimal.TryParse(txtInsulationResistance.Text, out insulationResistance))
            {
                invalidControls.Add(txtInsulationResistance);
                hasError = true;
            }

            // Validate Volume Resistivity
            if (string.IsNullOrWhiteSpace(txtVolumeResistivity.Text) || !decimal.TryParse(txtVolumeResistivity.Text, out volumeResistivity))
            {
                invalidControls.Add(txtVolumeResistivity);
                hasError = true;
            }

            // Validate Insulation Constant
            if (string.IsNullOrWhiteSpace(txtInsulationConstant.Text) || !decimal.TryParse(txtInsulationConstant.Text, out insulationConstant))
            {
                invalidControls.Add(txtInsulationConstant);
                hasError = true;
            }

            // Highlight invalid controls with darker red border
            foreach (var control in invalidControls)
            {
                if (control is TextBox)
                {
                    ((TextBox)control).BorderColor = System.Drawing.Color.FromArgb(255, 150, 150);
                    ((TextBox)control).BorderStyle = BorderStyle.Solid;
                    ((TextBox)control).BorderWidth = 1;
                }
            }

            if (hasError)
            {
                ShowMessage("Please fill in all required fields correctly.");
                return;
            }

            // Get the DataTable from ViewState
            DataTable dt = ViewState["ResistivityTestResults"] as DataTable;
            if (dt == null)
            {
                // Initialize the DataTable if it doesn't exist
                dt = new DataTable();
                dt.Columns.Add("Date", typeof(string));
                dt.Columns.Add("Core", typeof(string));
                dt.Columns.Add("Temperature", typeof(decimal));
                dt.Columns.Add("InsulationResistance", typeof(decimal));
                dt.Columns.Add("VolumeResistivity", typeof(decimal));
                dt.Columns.Add("InsulationConstant", typeof(decimal));
       
            }

            // Create a new row
            DataRow dr = dt.NewRow();
            dr["Date"] = testdate6.Text;
            dr["Core"] = ddlCore6.SelectedItem.Text;
            dr["Temperature"] = temperature;
            dr["InsulationResistance"] = insulationResistance;
            dr["VolumeResistivity"] = volumeResistivity;
            dr["InsulationConstant"] = insulationConstant;
         

            // Add the row to the DataTable
            dt.Rows.Add(dr);

            // Update the ViewState
            ViewState["ResistivityTestResults"] = dt;

            // Rebind the GridView
            gv6.DataSource = dt;
            gv6.DataBind();

            // Clear the input fields for next entry
            ClearInputFields6();
        }
        protected void btnAddToGrid7_Click(object sender, EventArgs e)
        {
            // Reset previous error styles
            ResetControlBorders7();
            bool hasError = false;
            List<Control> invalidControls = new List<Control>();
            decimal temperature = 0, insulationResistance = 0, volumeResistivity = 0, insulationConstant = 0;

            // Validate Date
            if (string.IsNullOrWhiteSpace(testdate7.Text))
            {
                invalidControls.Add(testdate7);
                hasError = true;
            }

            // Validate Core DropDownList
            if (ddlCore7.SelectedIndex == -1 || string.IsNullOrWhiteSpace(ddlCore7.SelectedValue))
            {
                ddlCore7.CssClass += " input-error";
                hasError = true;
            }

            // Validate Temperature
            if (string.IsNullOrWhiteSpace(txtTemp7.Text) || !decimal.TryParse(txtTemp7.Text, out temperature))
            {
                invalidControls.Add(txtTemp7);
                hasError = true;
            }

            // Validate Insulation Resistance
            if (string.IsNullOrWhiteSpace(txtInsulationResistance7.Text) || !decimal.TryParse(txtInsulationResistance7.Text, out insulationResistance))
            {
                invalidControls.Add(txtInsulationResistance7);
                hasError = true;
            }

            // Validate Volume Resistivity
            if (string.IsNullOrWhiteSpace(txtVolumeResistivity7.Text) || !decimal.TryParse(txtVolumeResistivity7.Text, out volumeResistivity))
            {
                invalidControls.Add(txtVolumeResistivity7);
                hasError = true;
            }

            // Validate Insulation Constant
            if (string.IsNullOrWhiteSpace(txtInsulationConstant7.Text) || !decimal.TryParse(txtInsulationConstant7.Text, out insulationConstant))
            {
                invalidControls.Add(txtInsulationConstant7);
                hasError = true;
            }

            // Highlight invalid controls with darker red border
            foreach (var control in invalidControls)
            {
                if (control is TextBox)
                {
                    ((TextBox)control).BorderColor = System.Drawing.Color.FromArgb(255, 150, 150);
                    ((TextBox)control).BorderStyle = BorderStyle.Solid;
                    ((TextBox)control).BorderWidth = 1;
                }
            }

            if (hasError)
            {
                ShowMessage("Please fill in all required fields correctly.");
                return;
            }

            // Get the DataTable from ViewState
            DataTable dt = ViewState["ResistivityMaxTempTestResults"] as DataTable;
            if (dt == null)
            {
                // Initialize the DataTable if it doesn't exist
                dt = new DataTable();
                dt.Columns.Add("Date", typeof(string));
                dt.Columns.Add("Core", typeof(string));
                dt.Columns.Add("Temperature", typeof(decimal));
                dt.Columns.Add("InsulationResistance", typeof(decimal));
                dt.Columns.Add("VolumeResistivity", typeof(decimal));
                dt.Columns.Add("InsulationConstant", typeof(decimal));
              
            }

            // Create a new row
            DataRow dr = dt.NewRow();
            dr["Date"] = testdate7.Text;
            dr["Core"] = ddlCore7.SelectedItem.Text;
            dr["Temperature"] = temperature;
            dr["InsulationResistance"] = insulationResistance;
            dr["VolumeResistivity"] = volumeResistivity;
            dr["InsulationConstant"] = insulationConstant;
           

            // Add the row to the DataTable
            dt.Rows.Add(dr);

            // Update the ViewState
            ViewState["ResistivityMaxTempTestResults"] = dt;

            // Rebind the GridView
            gv7.DataSource = dt;
            gv7.DataBind();

            // Clear the input fields for next entry
            ClearInputFields7();
        }
       
        //Grids
        protected void gv1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Get reference to the GridView and data
                GridView gridView = (GridView)sender;
                DataTable dt = (DataTable)gridView.DataSource;

                // Current row data
                DataRowView rowView = (DataRowView)e.Row.DataItem;

                // Format date if needed
                if (rowView["Date"] != null && !string.IsNullOrEmpty(rowView["Date"].ToString()))
                {
                    DateTime date;
                    if (DateTime.TryParse(rowView["Date"].ToString(), out date))
                    {
                        e.Row.Cells[0].Text = date.ToString("dd/MM/yyyy");
                    }
                }

                // Check if this is not the first row
                if (e.Row.RowIndex > 0)
                {
                    // Get current and previous dates
                    string currentDate = rowView["Date"].ToString();
                    string previousDate = dt.Rows[e.Row.RowIndex - 1]["Date"].ToString();

                    // If Dates are the same, merge them
                    if (currentDate == previousDate)
                    {
                        // Find the first row with this Date
                        int dateStartRowIndex = e.Row.RowIndex - 1;
                        while (dateStartRowIndex > 0 && dt.Rows[dateStartRowIndex - 1]["Date"].ToString() == currentDate)
                        {
                            dateStartRowIndex--;
                        }

                        // Increase the row span for the Date cell
                        if (gridView.Rows[dateStartRowIndex].Cells[0].RowSpan == 0)
                            gridView.Rows[dateStartRowIndex].Cells[0].RowSpan = 2;
                        else
                            gridView.Rows[dateStartRowIndex].Cells[0].RowSpan++;

                        // Hide the cell in the current row
                        e.Row.Cells[0].Visible = false;
                    }
                }
                if (e.Row.RowIndex > 0)
                {
                    // Merge SPECIFIED column 
                    string currentSpecified = rowView["Specified"]?.ToString() ?? "";
                    string previousSpecified = dt.Rows[e.Row.RowIndex - 1]["Specified"]?.ToString() ?? "";

                    if (currentSpecified == previousSpecified)
                    {
                        int specifiedStartRowIndex = e.Row.RowIndex - 1;
                        while (specifiedStartRowIndex > 0 &&
                              (dt.Rows[specifiedStartRowIndex - 1]["Specified"]?.ToString() ?? "") == currentSpecified)
                        {
                            specifiedStartRowIndex--;
                        }

                        // Find the cell index for "Specified" column
                        int specifiedCellIndex = -1;
                        for (int i = 0; i < gridView.Columns.Count; i++)
                        {
                            if (((BoundField)gridView.Columns[i]).DataField == "Specified")
                            {
                                specifiedCellIndex = i;
                                break;
                            }
                        }
                        if (specifiedCellIndex >= 0)
                        {
                            if (gridView.Rows[specifiedStartRowIndex].Cells[specifiedCellIndex].RowSpan == 0)
                                gridView.Rows[specifiedStartRowIndex].Cells[specifiedCellIndex].RowSpan = 2;
                            else
                                gridView.Rows[specifiedStartRowIndex].Cells[specifiedCellIndex].RowSpan++;

                            e.Row.Cells[specifiedCellIndex].Visible = false;
                        }
                    }
                }
                // Format numeric values to show appropriate decimal places
                if (rowView["Length"] != null && decimal.TryParse(rowView["Length"].ToString(), out decimal length))
                {
                    e.Row.Cells[2].Text = length.ToString("0.##");
                }

                if (rowView["Immerse"] != null && decimal.TryParse(rowView["Immerse"].ToString(), out decimal immerse))
                {
                    e.Row.Cells[3].Text = immerse.ToString("0.##");
                }

                if (rowView["Temperature"] != null && decimal.TryParse(rowView["Temperature"].ToString(), out decimal temperature))
                {
                    e.Row.Cells[4].Text = temperature.ToString("0.##");
                }

                if (rowView["Voltage"] != null && decimal.TryParse(rowView["Voltage"].ToString(), out decimal voltage))
                {
                    e.Row.Cells[5].Text = voltage.ToString("0.##");
                }

                if (rowView["Time"] != null && decimal.TryParse(rowView["Time"].ToString(), out decimal time))
                {
                    e.Row.Cells[6].Text = time.ToString("0.##");
                }

                // Find and configure the delete button
                LinkButton lnkDelete = (LinkButton)e.Row.FindControl("lnkDelete");
                if (lnkDelete != null)
                {
                    lnkDelete.CommandArgument = e.Row.RowIndex.ToString();
                    lnkDelete.Attributes.Add("onclick", "return confirm('Are you sure you want to delete this AC voltage test record?');");
                }
            }

        }
        protected void gv2_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Get reference to the GridView and data
                GridView gridView = (GridView)sender;
                DataTable dt = (DataTable)gridView.DataSource;

                // Current row data
                DataRowView rowView = (DataRowView)e.Row.DataItem;

                // Format date if needed
                if (rowView["Date"] != null && !string.IsNullOrEmpty(rowView["Date"].ToString()))
                {
                    DateTime date;
                    if (DateTime.TryParse(rowView["Date"].ToString(), out date))
                    {
                        e.Row.Cells[0].Text = date.ToString("dd/MM/yyyy");
                    }
                }

                // Check if this is not the first row
                if (e.Row.RowIndex > 0)
                {
                    // Get current and previous dates
                    string currentDate = rowView["Date"].ToString();
                    string previousDate = dt.Rows[e.Row.RowIndex - 1]["Date"].ToString();

                    // If Dates are the same, merge them
                    if (currentDate == previousDate)
                    {
                        // Find the first row with this Date
                        int dateStartRowIndex = e.Row.RowIndex - 1;
                        while (dateStartRowIndex > 0 && dt.Rows[dateStartRowIndex - 1]["Date"].ToString() == currentDate)
                        {
                            dateStartRowIndex--;
                        }

                        // Increase the row span for the Date cell
                        if (gridView.Rows[dateStartRowIndex].Cells[0].RowSpan == 0)
                            gridView.Rows[dateStartRowIndex].Cells[0].RowSpan = 2;
                        else
                            gridView.Rows[dateStartRowIndex].Cells[0].RowSpan++;

                        // Hide the cell in the current row
                        e.Row.Cells[0].Visible = false;
                    }
                }
                if (e.Row.RowIndex > 0)
                {
                    // Merge SPECIFIED column 
                    string currentSpecified = rowView["Specified"]?.ToString() ?? "";
                    string previousSpecified = dt.Rows[e.Row.RowIndex - 1]["Specified"]?.ToString() ?? "";

                    if (currentSpecified == previousSpecified)
                    {
                        int specifiedStartRowIndex = e.Row.RowIndex - 1;
                        while (specifiedStartRowIndex > 0 &&
                              (dt.Rows[specifiedStartRowIndex - 1]["Specified"]?.ToString() ?? "") == currentSpecified)
                        {
                            specifiedStartRowIndex--;
                        }

                        // Find the cell index for "Specified" column
                        int specifiedCellIndex = -1;
                        for (int i = 0; i < gridView.Columns.Count; i++)
                        {
                            if (((BoundField)gridView.Columns[i]).DataField == "Specified")
                            {
                                specifiedCellIndex = i;
                                break;
                            }
                        }
                        if (specifiedCellIndex >= 0)
                        {
                            if (gridView.Rows[specifiedStartRowIndex].Cells[specifiedCellIndex].RowSpan == 0)
                                gridView.Rows[specifiedStartRowIndex].Cells[specifiedCellIndex].RowSpan = 2;
                            else
                                gridView.Rows[specifiedStartRowIndex].Cells[specifiedCellIndex].RowSpan++;

                            e.Row.Cells[specifiedCellIndex].Visible = false;
                        }
                    }
                }

                if (rowView["Length"] != null && decimal.TryParse(rowView["Length"].ToString(), out decimal length))
                {
                    e.Row.Cells[2].Text = length.ToString("0.##");
                }

                if (rowView["Immerse"] != null && decimal.TryParse(rowView["Immerse"].ToString(), out decimal immerse))
                {
                    e.Row.Cells[3].Text = immerse.ToString("0.##");
                }

                if (rowView["Temperature"] != null && decimal.TryParse(rowView["Temperature"].ToString(), out decimal temperature))
                {
                    e.Row.Cells[4].Text = temperature.ToString("0.##");
                }

                if (rowView["Voltage"] != null && decimal.TryParse(rowView["Voltage"].ToString(), out decimal voltage))
                {
                    e.Row.Cells[5].Text = voltage.ToString("0.##");
                }

                if (rowView["Time"] != null && decimal.TryParse(rowView["Time"].ToString(), out decimal time))
                {
                    e.Row.Cells[6].Text = time.ToString("0.##");
                }

                // Find and configure the delete button
                LinkButton lnkDelete = (LinkButton)e.Row.FindControl("lnkDelete");
                if (lnkDelete != null)
                {
                    lnkDelete.CommandArgument = e.Row.RowIndex.ToString();
                    lnkDelete.Attributes.Add("onclick", "return confirm('Are you sure you want to delete this DC voltage test record?');");
                }
            }

        }
        protected void gv3_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Get reference to the GridView and data
                GridView gridView = (GridView)sender;
                DataTable dt = (DataTable)gridView.DataSource;

                // Current row data
                DataRowView rowView = (DataRowView)e.Row.DataItem;

                // Format date if needed
                if (rowView["Date"] != null && !string.IsNullOrEmpty(rowView["Date"].ToString()))
                {
                    DateTime date;
                    if (DateTime.TryParse(rowView["Date"].ToString(), out date))
                    {
                        e.Row.Cells[0].Text = date.ToString("dd/MM/yyyy");
                    }
                }

                // Check if this is not the first row
                if (e.Row.RowIndex > 0)
                {
                    // Get current and previous dates
                    string currentDate = rowView["Date"].ToString();
                    string previousDate = dt.Rows[e.Row.RowIndex - 1]["Date"].ToString();

                    // If Dates are the same, merge them
                    if (currentDate == previousDate)
                    {
                        // Find the first row with this Date
                        int dateStartRowIndex = e.Row.RowIndex - 1;
                        while (dateStartRowIndex > 0 && dt.Rows[dateStartRowIndex - 1]["Date"].ToString() == currentDate)
                        {
                            dateStartRowIndex--;
                        }

                        // Increase the row span for the Date cell
                        if (gridView.Rows[dateStartRowIndex].Cells[0].RowSpan == 0)
                            gridView.Rows[dateStartRowIndex].Cells[0].RowSpan = 2;
                        else
                            gridView.Rows[dateStartRowIndex].Cells[0].RowSpan++;

                        // Hide the cell in the current row
                        e.Row.Cells[0].Visible = false;
                    }
                }
                if (e.Row.RowIndex > 0)
                {
                    // Merge SPECIFIED column 
                    string currentSpecified = rowView["Specified"]?.ToString() ?? "";
                    string previousSpecified = dt.Rows[e.Row.RowIndex - 1]["Specified"]?.ToString() ?? "";

                    if (currentSpecified == previousSpecified)
                    {
                        int specifiedStartRowIndex = e.Row.RowIndex - 1;
                        while (specifiedStartRowIndex > 0 &&
                              (dt.Rows[specifiedStartRowIndex - 1]["Specified"]?.ToString() ?? "") == currentSpecified)
                        {
                            specifiedStartRowIndex--;
                        }

                        // Find the cell index for "Specified" column
                        int specifiedCellIndex = -1;
                        for (int i = 0; i < gridView.Columns.Count; i++)
                        {
                            if (((BoundField)gridView.Columns[i]).DataField == "Specified")
                            {
                                specifiedCellIndex = i;
                                break;
                            }
                        }
                        if (specifiedCellIndex >= 0)
                        {
                            if (gridView.Rows[specifiedStartRowIndex].Cells[specifiedCellIndex].RowSpan == 0)
                                gridView.Rows[specifiedStartRowIndex].Cells[specifiedCellIndex].RowSpan = 2;
                            else
                                gridView.Rows[specifiedStartRowIndex].Cells[specifiedCellIndex].RowSpan++;

                            e.Row.Cells[specifiedCellIndex].Visible = false;
                        }
                    }
                }

                if (rowView["Length"] != null && decimal.TryParse(rowView["Length"].ToString(), out decimal length))
                {
                    e.Row.Cells[2].Text = length.ToString("0.##");
                }

                if (rowView["Immerse"] != null && decimal.TryParse(rowView["Immerse"].ToString(), out decimal immerse))
                {
                    e.Row.Cells[3].Text = immerse.ToString("0.##");
                }

                if (rowView["Temperature"] != null && decimal.TryParse(rowView["Temperature"].ToString(), out decimal temperature))
                {
                    e.Row.Cells[4].Text = temperature.ToString("0.##");
                }

                if (rowView["Measurement"] != null && decimal.TryParse(rowView["Measurement"].ToString(), out decimal measurement))
                {
                    e.Row.Cells[5].Text = measurement.ToString("0.##");
                }

                if (rowView["InsulationResistance"] != null && decimal.TryParse(rowView["InsulationResistance"].ToString(), out decimal insulationResistance))
                {
                    e.Row.Cells[6].Text = insulationResistance.ToString("0.##");
                }

                // Find and configure the delete button
                LinkButton lnkDelete = (LinkButton)e.Row.FindControl("lnkDelete");
                if (lnkDelete != null)
                {
                    lnkDelete.CommandArgument = e.Row.RowIndex.ToString();
                    lnkDelete.Attributes.Add("onclick", "return confirm('Are you sure you want to delete this insulation resistance test record?');");
                }
            }
        }
        protected void gv4_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Get reference to the GridView and data
                GridView gridView = (GridView)sender;
                DataTable dt = (DataTable)gridView.DataSource;

                // Current row data
                DataRowView rowView = (DataRowView)e.Row.DataItem;

                // Format date if needed
                if (rowView["Date"] != null && !string.IsNullOrEmpty(rowView["Date"].ToString()))
                {
                    DateTime date;
                    if (DateTime.TryParse(rowView["Date"].ToString(), out date))
                    {
                        e.Row.Cells[0].Text = date.ToString("dd/MM/yyyy");
                    }
                }

                // Check if this is not the first row
                if (e.Row.RowIndex > 0)
                {
                    // Get current and previous dates
                    string currentDate = rowView["Date"].ToString();
                    string previousDate = dt.Rows[e.Row.RowIndex - 1]["Date"].ToString();

                    // If Dates are the same, merge them
                    if (currentDate == previousDate)
                    {
                        // Find the first row with this Date
                        int dateStartRowIndex = e.Row.RowIndex - 1;
                        while (dateStartRowIndex > 0 && dt.Rows[dateStartRowIndex - 1]["Date"].ToString() == currentDate)
                        {
                            dateStartRowIndex--;
                        }

                        // Increase the row span for the Date cell
                        if (gridView.Rows[dateStartRowIndex].Cells[0].RowSpan == 0)
                            gridView.Rows[dateStartRowIndex].Cells[0].RowSpan = 2;
                        else
                            gridView.Rows[dateStartRowIndex].Cells[0].RowSpan++;

                        // Hide the cell in the current row
                        e.Row.Cells[0].Visible = false;
                    }
                }
                if (e.Row.RowIndex > 0)
                {
                    // Merge SPECIFIED column 
                    string currentSpecified = rowView["Specified"]?.ToString() ?? "";
                    string previousSpecified = dt.Rows[e.Row.RowIndex - 1]["Specified"]?.ToString() ?? "";

                    if (currentSpecified == previousSpecified)
                    {
                        int specifiedStartRowIndex = e.Row.RowIndex - 1;
                        while (specifiedStartRowIndex > 0 &&
                              (dt.Rows[specifiedStartRowIndex - 1]["Specified"]?.ToString() ?? "") == currentSpecified)
                        {
                            specifiedStartRowIndex--;
                        }

                        // Find the cell index for "Specified" column
                        int specifiedCellIndex = -1;
                        for (int i = 0; i < gridView.Columns.Count; i++)
                        {
                            if (((BoundField)gridView.Columns[i]).DataField == "Specified")
                            {
                                specifiedCellIndex = i;
                                break;
                            }
                        }
                        if (specifiedCellIndex >= 0)
                        {
                            if (gridView.Rows[specifiedStartRowIndex].Cells[specifiedCellIndex].RowSpan == 0)
                                gridView.Rows[specifiedStartRowIndex].Cells[specifiedCellIndex].RowSpan = 2;
                            else
                                gridView.Rows[specifiedStartRowIndex].Cells[specifiedCellIndex].RowSpan++;

                            e.Row.Cells[specifiedCellIndex].Visible = false;
                        }
                    }
                }

                if (rowView["Length"] != null && decimal.TryParse(rowView["Length"].ToString(), out decimal length))
                {
                    e.Row.Cells[2].Text = length.ToString("0.##");
                }

                if (rowView["Immerse"] != null && decimal.TryParse(rowView["Immerse"].ToString(), out decimal immerse))
                {
                    e.Row.Cells[3].Text = immerse.ToString("0.##");
                }

                if (rowView["Temperature"] != null && decimal.TryParse(rowView["Temperature"].ToString(), out decimal temperature))
                {
                    e.Row.Cells[4].Text = temperature.ToString("0.##");
                }

                if (rowView["Factor"] != null && decimal.TryParse(rowView["Factor"].ToString(), out decimal factor))
                {
                    e.Row.Cells[5].Text = factor.ToString("0.##");
                }

                if (rowView["Measurement"] != null && decimal.TryParse(rowView["Measurement"].ToString(), out decimal measurement))
                {
                    e.Row.Cells[6].Text = measurement.ToString("0.##");
                }

                if (rowView["InsulationResistance"] != null && decimal.TryParse(rowView["InsulationResistance"].ToString(), out decimal insulationResistance))
                {
                    e.Row.Cells[7].Text = insulationResistance.ToString("0.##");
                }

                // Find and configure the delete button
                LinkButton lnkDelete = (LinkButton)e.Row.FindControl("lnkDelete");
                if (lnkDelete != null)
                {
                    lnkDelete.CommandArgument = e.Row.RowIndex.ToString();
                    lnkDelete.Attributes.Add("onclick", "return confirm('Are you sure you want to delete this max temperature insulation resistance test record?');");
                }
            }
        }
        protected void gv5_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Get reference to the GridView and data
                GridView gridView = (GridView)sender;
                DataTable dt = (DataTable)gridView.DataSource;

                // Current row data
                DataRowView rowView = (DataRowView)e.Row.DataItem;

                // Format date if needed
                if (rowView["Date"] != null && !string.IsNullOrEmpty(rowView["Date"].ToString()))
                {
                    DateTime date;
                    if (DateTime.TryParse(rowView["Date"].ToString(), out date))
                    {
                        e.Row.Cells[0].Text = date.ToString("dd/MM/yyyy");
                    }
                }

                // Check if this is not the first row
                if (e.Row.RowIndex > 0)
                {
                    // Get current and previous dates
                    string currentDate = rowView["Date"].ToString();
                    string previousDate = dt.Rows[e.Row.RowIndex - 1]["Date"].ToString();

                    // If Dates are the same, merge them
                    if (currentDate == previousDate)
                    {
                        // Find the first row with this Date
                        int dateStartRowIndex = e.Row.RowIndex - 1;
                        while (dateStartRowIndex > 0 && dt.Rows[dateStartRowIndex - 1]["Date"].ToString() == currentDate)
                        {
                            dateStartRowIndex--;
                        }

                        // Increase the row span for the Date cell
                        if (gridView.Rows[dateStartRowIndex].Cells[0].RowSpan == 0)
                            gridView.Rows[dateStartRowIndex].Cells[0].RowSpan = 2;
                        else
                            gridView.Rows[dateStartRowIndex].Cells[0].RowSpan++;

                        // Hide the cell in the current row
                        e.Row.Cells[0].Visible = false;
                    }
                }

                // Format numeric values
                if (rowView["Length"] != null && decimal.TryParse(rowView["Length"].ToString(), out decimal length))
                {
                    e.Row.Cells[2].Text = length.ToString("0.##");
                }

                if (rowView["Inner1"] != null && decimal.TryParse(rowView["Inner1"].ToString(), out decimal inner1))
                {
                    e.Row.Cells[3].Text = inner1.ToString("0.##");
                }

                if (rowView["Inner2"] != null && decimal.TryParse(rowView["Inner2"].ToString(), out decimal inner2))
                {
                    e.Row.Cells[4].Text = inner2.ToString("0.##");
                }

                if (rowView["InnerAvg"] != null && decimal.TryParse(rowView["InnerAvg"].ToString(), out decimal innerAvg))
                {
                    e.Row.Cells[5].Text = innerAvg.ToString("0.##");
                }

                if (rowView["Outer1"] != null && decimal.TryParse(rowView["Outer1"].ToString(), out decimal outer1))
                {
                    e.Row.Cells[6].Text = outer1.ToString("0.##");
                }

                if (rowView["Outer2"] != null && decimal.TryParse(rowView["Outer2"].ToString(), out decimal outer2))
                {
                    e.Row.Cells[7].Text = outer2.ToString("0.##");
                }

                if (rowView["OuterAvg"] != null && decimal.TryParse(rowView["OuterAvg"].ToString(), out decimal outerAvg))
                {
                    e.Row.Cells[8].Text = outerAvg.ToString("0.##");
                }

                // Find and configure the delete button
                LinkButton lnkDelete = (LinkButton)e.Row.FindControl("lnkDelete");
                if (lnkDelete != null)
                {
                    lnkDelete.CommandArgument = e.Row.RowIndex.ToString();
                    lnkDelete.Attributes.Add("onclick", "return confirm('Are you sure you want to delete this diameter test record?');");
                }
            }
        }
        protected void gv6_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Get reference to the GridView and data
                GridView gridView = (GridView)sender;
                DataTable dt = (DataTable)gridView.DataSource;

                // Current row data
                DataRowView rowView = (DataRowView)e.Row.DataItem;

                // Format date if needed
                if (rowView["Date"] != null && !string.IsNullOrEmpty(rowView["Date"].ToString()))
                {
                    DateTime date;
                    if (DateTime.TryParse(rowView["Date"].ToString(), out date))
                    {
                        e.Row.Cells[0].Text = date.ToString("dd/MM/yyyy");
                    }
                }

                // Check if this is not the first row
                if (e.Row.RowIndex > 0)
                {
                    // Get current and previous dates
                    string currentDate = rowView["Date"].ToString();
                    string previousDate = dt.Rows[e.Row.RowIndex - 1]["Date"].ToString();

                    // If Dates are the same, merge them
                    if (currentDate == previousDate)
                    {
                        // Find the first row with this Date
                        int dateStartRowIndex = e.Row.RowIndex - 1;
                        while (dateStartRowIndex > 0 && dt.Rows[dateStartRowIndex - 1]["Date"].ToString() == currentDate)
                        {
                            dateStartRowIndex--;
                        }

                        // Increase the row span for the Date cell
                        if (gridView.Rows[dateStartRowIndex].Cells[0].RowSpan == 0)
                            gridView.Rows[dateStartRowIndex].Cells[0].RowSpan = 2;
                        else
                            gridView.Rows[dateStartRowIndex].Cells[0].RowSpan++;

                        // Hide the cell in the current row
                        e.Row.Cells[0].Visible = false;
                    }
                }

                // Format numeric values
                if (rowView["Temperature"] != null && decimal.TryParse(rowView["Temperature"].ToString(), out decimal temperature))
                {
                    e.Row.Cells[2].Text = temperature.ToString("0.##");
                }

                if (rowView["InsulationResistance"] != null && decimal.TryParse(rowView["InsulationResistance"].ToString(), out decimal insulationResistance))
                {
                    e.Row.Cells[3].Text = insulationResistance.ToString("0.##");
                }

                if (rowView["VolumeResistivity"] != null && decimal.TryParse(rowView["VolumeResistivity"].ToString(), out decimal volumeResistivity))
                {
                    e.Row.Cells[4].Text = volumeResistivity.ToString("0.##");
                }

                if (rowView["InsulationConstant"] != null && decimal.TryParse(rowView["InsulationConstant"].ToString(), out decimal insulationConstant))
                {
                    e.Row.Cells[5].Text = insulationConstant.ToString("0.##");
                }

                // Find and configure the delete button
                LinkButton lnkDelete = (LinkButton)e.Row.FindControl("lnkDelete");
                if (lnkDelete != null)
                {
                    lnkDelete.CommandArgument = e.Row.RowIndex.ToString();
                    lnkDelete.Attributes.Add("onclick", "return confirm('Are you sure you want to delete this resistivity test record?');");
                }
            }
        }
        protected void gv7_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Get reference to the GridView and data
                GridView gridView = (GridView)sender;
                DataTable dt = (DataTable)gridView.DataSource;

                // Current row data
                DataRowView rowView = (DataRowView)e.Row.DataItem;

                // Format date if needed
                if (rowView["Date"] != null && !string.IsNullOrEmpty(rowView["Date"].ToString()))
                {
                    DateTime date;
                    if (DateTime.TryParse(rowView["Date"].ToString(), out date))
                    {
                        e.Row.Cells[0].Text = date.ToString("dd/MM/yyyy");
                    }
                }

                // Check if this is not the first row
                if (e.Row.RowIndex > 0)
                {
                    // Get current and previous dates
                    string currentDate = rowView["Date"].ToString();
                    string previousDate = dt.Rows[e.Row.RowIndex - 1]["Date"].ToString();

                    // If Dates are the same, merge them
                    if (currentDate == previousDate)
                    {
                        // Find the first row with this Date
                        int dateStartRowIndex = e.Row.RowIndex - 1;
                        while (dateStartRowIndex > 0 && dt.Rows[dateStartRowIndex - 1]["Date"].ToString() == currentDate)
                        {
                            dateStartRowIndex--;
                        }

                        // Increase the row span for the Date cell
                        if (gridView.Rows[dateStartRowIndex].Cells[0].RowSpan == 0)
                            gridView.Rows[dateStartRowIndex].Cells[0].RowSpan = 2;
                        else
                            gridView.Rows[dateStartRowIndex].Cells[0].RowSpan++;

                        // Hide the cell in the current row
                        e.Row.Cells[0].Visible = false;
                    }
                }
                // Format numeric values
                if (rowView["Temperature"] != null && decimal.TryParse(rowView["Temperature"].ToString(), out decimal temperature))
                {
                    e.Row.Cells[2].Text = temperature.ToString("0.##");
                }

                if (rowView["InsulationResistance"] != null && decimal.TryParse(rowView["InsulationResistance"].ToString(), out decimal insulationResistance))
                {
                    e.Row.Cells[3].Text = insulationResistance.ToString("0.##");
                }

                if (rowView["VolumeResistivity"] != null && decimal.TryParse(rowView["VolumeResistivity"].ToString(), out decimal volumeResistivity))
                {
                    e.Row.Cells[4].Text = volumeResistivity.ToString("0.##");
                }

                if (rowView["InsulationConstant"] != null && decimal.TryParse(rowView["InsulationConstant"].ToString(), out decimal insulationConstant))
                {
                    e.Row.Cells[5].Text = insulationConstant.ToString("0.##");
                }

                // Find and configure the delete button
                LinkButton lnkDelete = (LinkButton)e.Row.FindControl("lnkDelete");
                if (lnkDelete != null)
                {
                    lnkDelete.CommandArgument = e.Row.RowIndex.ToString();
                    lnkDelete.Attributes.Add("onclick", "return confirm('Are you sure you want to delete this resistivity test record?');");
                }
            }
        }

        //gv RowCommand
        protected void gv1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                try
                {
                    int rowIndex = Convert.ToInt32(e.CommandArgument);

                    // Get the current DataTable from ViewState
                    DataTable dt = ViewState["ACTestResults"] as DataTable;

                    if (dt != null && rowIndex >= 0 && rowIndex < dt.Rows.Count)
                    {
                        // Remove the row from DataTable
                        dt.Rows.RemoveAt(rowIndex);

                        // Update ViewState
                        ViewState["ACTestResults"] = dt;

                        // Rebind the GridView
                        gv1.DataSource = dt;
                        gv1.DataBind();
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("Error deleting record: " + ex.Message);
                }
            }
        }
        protected void gv2_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                try
                {
                    int rowIndex = Convert.ToInt32(e.CommandArgument);

                    // Get the current DataTable from ViewState
                    DataTable dt = ViewState["DCTestResults"] as DataTable;

                    if (dt != null && rowIndex >= 0 && rowIndex < dt.Rows.Count)
                    {
                        // Remove the row from DataTable
                        dt.Rows.RemoveAt(rowIndex);

                        // Update ViewState
                        ViewState["DCTestResults"] = dt;

                        // Rebind the GridView
                        gv2.DataSource = dt;
                        gv2.DataBind();

                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("Error deleting record: " + ex.Message);
                }
            }
        }
        protected void gv3_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                try
                {
                    int rowIndex = Convert.ToInt32(e.CommandArgument);

                    // Get the current DataTable from ViewState
                    DataTable dt = ViewState["InsulationAmbientTestResults"] as DataTable;

                    if (dt != null && rowIndex >= 0 && rowIndex < dt.Rows.Count)
                    {
                        // Remove the row from DataTable
                        dt.Rows.RemoveAt(rowIndex);

                        // Update ViewState
                        ViewState["InsulationAmbientTestResults"] = dt;

                        // Rebind the GridView
                        gv3.DataSource = dt;
                        gv3.DataBind();

                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("Error deleting record: " + ex.Message);
                }
            }
        }
        protected void gv4_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                try
                {
                    int rowIndex = Convert.ToInt32(e.CommandArgument);

                    // Get the current DataTable from ViewState
                    DataTable dt = ViewState["InsulationMaxTempTestResults"] as DataTable;

                    if (dt != null && rowIndex >= 0 && rowIndex < dt.Rows.Count)
                    {
                        // Remove the row from DataTable
                        dt.Rows.RemoveAt(rowIndex);

                        // Update ViewState
                        ViewState["InsulationMaxTempTestResults"] = dt;

                        // Rebind the GridView
                        gv4.DataSource = dt;
                        gv4.DataBind();

                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("Error deleting record: " + ex.Message);
                }
            }
        }
        protected void gv5_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                try
                {
                    int rowIndex = Convert.ToInt32(e.CommandArgument);

                    // Get the current DataTable from ViewState
                    DataTable dt = ViewState["DiameterTestResults"] as DataTable;

                    if (dt != null && rowIndex >= 0 && rowIndex < dt.Rows.Count)
                    {
                        // Remove the row from DataTable
                        dt.Rows.RemoveAt(rowIndex);

                        // Update ViewState
                        ViewState["DiameterTestResults"] = dt;

                        // Rebind the GridView
                        gv5.DataSource = dt;
                        gv5.DataBind();

                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("Error deleting record: " + ex.Message);
                }
            }
        }
        protected void gv6_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                try
                {
                    int rowIndex = Convert.ToInt32(e.CommandArgument);

                    // Get the current DataTable from ViewState
                    DataTable dt = ViewState["ResistivityTestResults"] as DataTable;

                    if (dt != null && rowIndex >= 0 && rowIndex < dt.Rows.Count)
                    {
                        // Remove the row from DataTable
                        dt.Rows.RemoveAt(rowIndex);

                        // Update ViewState
                        ViewState["ResistivityTestResults"] = dt;

                        // Rebind the GridView
                        gv6.DataSource = dt;
                        gv6.DataBind();

                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("Error deleting record: " + ex.Message);
                }
            }
        }
        protected void gv7_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                try
                {
                    int rowIndex = Convert.ToInt32(e.CommandArgument);

                    // Get the current DataTable from ViewState
                    DataTable dt = ViewState["ResistivityMaxTempTestResults"] as DataTable;

                    if (dt != null && rowIndex >= 0 && rowIndex < dt.Rows.Count)
                    {
                        // Remove the row from DataTable
                        dt.Rows.RemoveAt(rowIndex);

                        // Update ViewState
                        ViewState["ResistivityMaxTempTestResults"] = dt;

                        // Rebind the GridView
                        gv7.DataSource = dt;
                        gv7.DataBind();

                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("Error deleting record: " + ex.Message);
                }
            }
        }

        //Saving Data
        //private void Submitform()
        //{
        //    try
        //    {
        //        // Get GridView data from ViewState
        //        DataTable dt = ViewState["TestResults"] as DataTable;

        //        if ((dt == null || dt.Rows.Count == 0) && gv1.Rows.Count == 0 && gv2.Rows.Count == 0)
        //        {
        //            ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", "alert('No test data to save. Please add results first.');", true);
        //            return;
        //        }

        //        if (_cm == null) _cm = new DAL.CM();
        //        if (Request.QueryString["s"] != null) _cm.sampleId = Convert.ToInt32(Request.QueryString["s"].ToLower());
        //        if (Request.QueryString["r"] != null) _cm.ReportID = Request.QueryString["r"];

        //        DataTable dt1 = _cm.Getdataforseriesandreportid_mechanical(_cm);
        //        if (dt1.Rows.Count > 0)
        //        {
        //            _cm.sampleIDSeries = dt1.Rows[0]["sampleIDSeries"].ToString();
        //            _cm.CRFSampleId = Convert.ToInt32(dt1.Rows[0]["CRFSampleId"]);
        //        }

        //        List<string> reportParamMapList = new List<string>();
        //        List<string> resultList = new List<string>();

        //        // Upper Section
        //        reportParamMapList.Add("17204"); resultList.Add(cableType.Text);        // Type
        //        reportParamMapList.Add("17205"); resultList.Add(cableSize.Text);        // Size
        //        reportParamMapList.Add("17206"); resultList.Add(lblvoltage.Text);       // Voltage
        //        reportParamMapList.Add("17207"); resultList.Add(Date.Text);             // Date
        //        reportParamMapList.Add("17208"); resultList.Add(roomno.Text);           // Room No
        //        reportParamMapList.Add("17209"); resultList.Add(roomtemp.Text);         // Room Temp
        //        reportParamMapList.Add("17210"); resultList.Add(refSpec.Text);          // Ref Spec
        //        reportParamMapList.Add("17211"); resultList.Add(testMethodNo.Text);     // Test Method No
        //        reportParamMapList.Add("17212"); resultList.Add(jobOrderNo.Text);       // Job Order No
        //        reportParamMapList.Add("17213"); resultList.Add(worksheetno.Text);      // Worksheet No
        //        reportParamMapList.Add("17214"); resultList.Add(roomhumidity.Text);     // Room Humidity
        //        reportParamMapList.Add("17215"); resultList.Add(equipmentno.Text);      // Equipment No

        //        // 1st grid
        //        if (gv1.Rows.Count > 0)
        //        {
        //            List<string> coreList = new List<string>();
        //            List<string> lengthList = new List<string>();
        //            List<string> immerseList = new List<string>();
        //            List<string> tempList = new List<string>();
        //            List<string> voltageList = new List<string>();
        //            List<string> timeList = new List<string>();
        //            List<string> resultACList = new List<string>();
        //            List<string> specifiedList = new List<string>();

        //            string dateAC = ""; // assuming same date for all rows

        //            foreach (GridViewRow row in gv1.Rows)
        //            {
        //                if (row.RowType == DataControlRowType.DataRow)
        //                {
        //                    if (string.IsNullOrEmpty(dateAC))
        //                        dateAC = row.Cells[0].Text; // Date column

        //                    coreList.Add(row.Cells[1].Text);        // Core
        //                    lengthList.Add(row.Cells[2].Text);      // Length
        //                    immerseList.Add(row.Cells[3].Text);     // Immerse
        //                    tempList.Add(row.Cells[4].Text);        // Temperature
        //                    voltageList.Add(row.Cells[5].Text);     // Voltage
        //                    timeList.Add(row.Cells[6].Text);        // Time
        //                    resultACList.Add(row.Cells[7].Text);    // Result
        //                    specifiedList.Add(row.Cells[8].Text);   // Specified
        //                }
        //            }
        //            reportParamMapList.Add("17219"); resultList.Add(dateAC);
        //            reportParamMapList.Add("17220"); resultList.Add(string.Join(",", coreList));
        //            reportParamMapList.Add("17221"); resultList.Add(string.Join(",", lengthList));
        //            reportParamMapList.Add("17222"); resultList.Add(string.Join(",", immerseList));
        //            reportParamMapList.Add("17223"); resultList.Add(string.Join(",", tempList));
        //            reportParamMapList.Add("17224"); resultList.Add(string.Join(",", voltageList));
        //            reportParamMapList.Add("17225"); resultList.Add(string.Join(",", timeList));
        //            reportParamMapList.Add("17226"); resultList.Add(string.Join(",", resultACList));
        //            reportParamMapList.Add("17227"); resultList.Add(string.Join(",", specifiedList));
        //        }

        //        // 2nd grid
        //        if (gv2.Rows.Count > 0)
        //        {
        //            List<string> coreList = new List<string>();
        //            List<string> lengthList = new List<string>();
        //            List<string> immerseList = new List<string>();
        //            List<string> tempList = new List<string>();
        //            List<string> voltageList = new List<string>();
        //            List<string> timeList = new List<string>();
        //            List<string> resultDCList = new List<string>();
        //            List<string> specifiedList = new List<string>();

        //            string dateDC = ""; // assuming same date for all rows

        //            foreach (GridViewRow row in gv2.Rows)
        //            {
        //                if (row.RowType == DataControlRowType.DataRow)
        //                {
        //                    if (string.IsNullOrEmpty(dateDC))
        //                        dateDC = row.Cells[0].Text; // Date column

        //                    coreList.Add(row.Cells[1].Text);        // Core
        //                    lengthList.Add(row.Cells[2].Text);      // Length
        //                    immerseList.Add(row.Cells[3].Text);     // Immerse
        //                    tempList.Add(row.Cells[4].Text);        // Temperature
        //                    voltageList.Add(row.Cells[5].Text);     // Voltage
        //                    timeList.Add(row.Cells[6].Text);        // Time
        //                    resultDCList.Add(row.Cells[7].Text);    // Result
        //                    specifiedList.Add(row.Cells[8].Text);   // Specified
        //                }
        //            }

        //            reportParamMapList.Add("17228"); resultList.Add(dateDC);
        //            reportParamMapList.Add("17229"); resultList.Add(string.Join(",", coreList));
        //            reportParamMapList.Add("17230"); resultList.Add(string.Join(",", lengthList));
        //            reportParamMapList.Add("17231"); resultList.Add(string.Join(",", immerseList));
        //            reportParamMapList.Add("17232"); resultList.Add(string.Join(",", tempList));
        //            reportParamMapList.Add("17233"); resultList.Add(string.Join(",", voltageList));
        //            reportParamMapList.Add("17234"); resultList.Add(string.Join(",", timeList));
        //            reportParamMapList.Add("17235"); resultList.Add(string.Join(",", resultDCList));
        //            reportParamMapList.Add("17236"); resultList.Add(string.Join(",", specifiedList));
        //        }

        //        // 3rd grid 
        //        if (gv3.Rows.Count > 0)
        //        {
        //            string dateIR = "";
        //            List<string> coreList = new List<string>();
        //            List<string> lengthList = new List<string>();
        //            List<string> immerseList = new List<string>();
        //            List<string> tempList = new List<string>();
        //            List<string> measurementList = new List<string>();
        //            List<string> insulationResistanceList = new List<string>();
        //            List<string> specifiedList = new List<string>();

        //            foreach (GridViewRow row in gv3.Rows)
        //            {
        //                if (row.RowType == DataControlRowType.DataRow)
        //                {
        //                    if (string.IsNullOrEmpty(dateIR))
        //                        dateIR = row.Cells[0].Text; // Date column

        //                    coreList.Add(row.Cells[1].Text);                 // Core
        //                    lengthList.Add(row.Cells[2].Text);               // Length
        //                    immerseList.Add(row.Cells[3].Text);              // Immerse
        //                    tempList.Add(row.Cells[4].Text);                 // Temperature
        //                    measurementList.Add(row.Cells[5].Text);          // Measurement
        //                    insulationResistanceList.Add(row.Cells[6].Text); // Insulation Resistance
        //                    specifiedList.Add(row.Cells[7].Text);            // Specified
        //                }
        //            }

        //            reportParamMapList.Add("17237"); resultList.Add(dateIR);
        //            reportParamMapList.Add("17238"); resultList.Add(string.Join(",", coreList));
        //            reportParamMapList.Add("17239"); resultList.Add(string.Join(",", lengthList));
        //            reportParamMapList.Add("17240"); resultList.Add(string.Join(",", immerseList));
        //            reportParamMapList.Add("17241"); resultList.Add(string.Join(",", tempList));
        //            reportParamMapList.Add("17242"); resultList.Add(string.Join(",", measurementList));
        //            reportParamMapList.Add("17243"); resultList.Add(string.Join(",", insulationResistanceList));
        //            reportParamMapList.Add("17244"); resultList.Add(string.Join(",", specifiedList));
        //        }

        //        // 4th grid 
        //        if (gv4.Rows.Count > 0)
        //        {
        //            string dateIRMax = "";
        //            List<string> coreList = new List<string>();
        //            List<string> lengthList = new List<string>();
        //            List<string> immerseList = new List<string>();
        //            List<string> tempList = new List<string>();
        //            List<string> factorList = new List<string>();
        //            List<string> measurementList = new List<string>();
        //            List<string> insulationResistanceList = new List<string>();
        //            List<string> specifiedList = new List<string>();

        //            foreach (GridViewRow row in gv4.Rows)
        //            {
        //                if (row.RowType == DataControlRowType.DataRow)
        //                {
        //                    if (string.IsNullOrEmpty(dateIRMax))
        //                        dateIRMax = row.Cells[0].Text; // Date column

        //                    coreList.Add(row.Cells[1].Text);                 // Core
        //                    lengthList.Add(row.Cells[2].Text);               // Length
        //                    immerseList.Add(row.Cells[3].Text);              // Immerse
        //                    tempList.Add(row.Cells[4].Text);                 // Temperature
        //                    factorList.Add(row.Cells[5].Text);               // Factor
        //                    measurementList.Add(row.Cells[6].Text);          // Measurement
        //                    insulationResistanceList.Add(row.Cells[7].Text); // Insulation Resistance
        //                    specifiedList.Add(row.Cells[8].Text);            // Specified
        //                }
        //            }

        //            reportParamMapList.Add("17245"); resultList.Add(dateIRMax);
        //            reportParamMapList.Add("17246"); resultList.Add(string.Join(",", coreList));
        //            reportParamMapList.Add("17247"); resultList.Add(string.Join(",", lengthList));
        //            reportParamMapList.Add("17248"); resultList.Add(string.Join(",", immerseList));
        //            reportParamMapList.Add("17249"); resultList.Add(string.Join(",", tempList));
        //            reportParamMapList.Add("17283"); resultList.Add(string.Join(",", factorList));
        //            reportParamMapList.Add("17250"); resultList.Add(string.Join(",", measurementList));
        //            reportParamMapList.Add("17251"); resultList.Add(string.Join(",", insulationResistanceList));
        //            reportParamMapList.Add("17252"); resultList.Add(string.Join(",", specifiedList));
        //        }

        //        // 5th grid 
        //        if (gv5.Rows.Count > 0)
        //        {
        //            string dateDiameter = "";
        //            List<string> coreList = new List<string>();
        //            List<string> lengthList = new List<string>();
        //            List<string> inner1List = new List<string>();
        //            List<string> inner2List = new List<string>();
        //            List<string> innerAvgList = new List<string>();
        //            List<string> outer1List = new List<string>();
        //            List<string> outer2List = new List<string>();
        //            List<string> outerAvgList = new List<string>();

        //            foreach (GridViewRow row in gv5.Rows)
        //            {
        //                if (row.RowType == DataControlRowType.DataRow)
        //                {
        //                    if (string.IsNullOrEmpty(dateDiameter))
        //                        dateDiameter = row.Cells[0].Text; // Date column

        //                    coreList.Add(row.Cells[1].Text);        // Core
        //                    lengthList.Add(row.Cells[2].Text);      // Length
        //                    inner1List.Add(row.Cells[3].Text);      // Inner1
        //                    inner2List.Add(row.Cells[4].Text);      // Inner2
        //                    innerAvgList.Add(row.Cells[5].Text);    // InnerAvg
        //                    outer1List.Add(row.Cells[6].Text);      // Outer1
        //                    outer2List.Add(row.Cells[7].Text);      // Outer2
        //                    outerAvgList.Add(row.Cells[8].Text);    // OuterAvg
        //                }
        //            }

        //            // Add specified values from textboxes
        //            string specifiedLength = txtspecifiedlength5.Text;
        //            string specifiedInnerDiameter = txtspecifiedInnerdiameter5.Text;
        //            string specifiedOuterDiameter = txtspecifiedOuterdiameter5.Text;
        //            string combinedSpecifiedValues = $"{specifiedLength},{specifiedInnerDiameter},{specifiedOuterDiameter}";

        //            reportParamMapList.Add("17253"); resultList.Add(dateDiameter);
        //            reportParamMapList.Add("17254"); resultList.Add(string.Join(",", coreList));
        //            reportParamMapList.Add("17255"); resultList.Add(string.Join(",", lengthList));
        //            reportParamMapList.Add("17256"); resultList.Add(string.Join(",", inner1List));
        //            reportParamMapList.Add("17257"); resultList.Add(string.Join(",", inner2List));
        //            reportParamMapList.Add("17258"); resultList.Add(string.Join(",", innerAvgList));
        //            reportParamMapList.Add("17259"); resultList.Add(string.Join(",", outer1List));
        //            reportParamMapList.Add("17260"); resultList.Add(string.Join(",", outer2List));
        //            reportParamMapList.Add("17261"); resultList.Add(string.Join(",", outerAvgList));
        //            reportParamMapList.Add("17262"); resultList.Add(combinedSpecifiedValues);
        //        }

        //        // 6th grid
        //        if (gv6.Rows.Count > 0)
        //        {
        //            string dateVolume = "";
        //            List<string> coreList = new List<string>();
        //            List<string> tempList = new List<string>();
        //            List<string> insulationResistanceList = new List<string>();
        //            List<string> volumeResistivityList = new List<string>();
        //            List<string> insulationConstantList = new List<string>();
        //            List<string> specifiedList = new List<string>();

        //            foreach (GridViewRow row in gv6.Rows)
        //            {
        //                if (row.RowType == DataControlRowType.DataRow)
        //                {
        //                    if (string.IsNullOrEmpty(dateVolume))
        //                        dateVolume = row.Cells[0].Text; // Date column

        //                    coreList.Add(row.Cells[1].Text);                 // Core
        //                    tempList.Add(row.Cells[2].Text);                 // Temperature
        //                    insulationResistanceList.Add(row.Cells[3].Text); // Insulation Resistance
        //                    volumeResistivityList.Add(row.Cells[4].Text);    // Volume Resistivity
        //                    insulationConstantList.Add(row.Cells[5].Text);   // Insulation Constant
        //                    specifiedList.Add(row.Cells[6].Text);            // Specified
        //                }
        //            }

        //            // Add specified values from textboxes
        //            string specifiedTemp = txtspecifiedtemp6.Text;
        //            string specifiedInsulationResistance = txtspecifiedInsulationresistance6.Text;
        //            string specifiedVolumeResistivity = txtspecifiedVolumeresistivity6.Text;
        //            string specifiedInsulationConstant = txtspecifiedInsulationresistantconstantKIat20C6.Text;
        //            string combinedSpecifiedValues = $"{specifiedTemp},{specifiedInsulationResistance},{specifiedVolumeResistivity},{specifiedInsulationConstant}";

        //            reportParamMapList.Add("17263"); resultList.Add(dateVolume);
        //            reportParamMapList.Add("17264"); resultList.Add(string.Join(",", coreList));
        //            reportParamMapList.Add("17265"); resultList.Add(string.Join(",", tempList));
        //            reportParamMapList.Add("17266"); resultList.Add(string.Join(",", insulationResistanceList));
        //            reportParamMapList.Add("17267"); resultList.Add(string.Join(",", volumeResistivityList));
        //            reportParamMapList.Add("17270"); resultList.Add(string.Join(",", insulationConstantList));
        //            reportParamMapList.Add("17272"); resultList.Add(string.Join(",", combinedSpecifiedValues));
        //        }

        //        // 7th grid
        //        if (gv7.Rows.Count > 0)
        //        {
        //            string dateVolumeMax = "";
        //            List<string> coreList = new List<string>();
        //            List<string> tempList = new List<string>();
        //            List<string> insulationResistanceList = new List<string>();
        //            List<string> volumeResistivityList = new List<string>();
        //            List<string> insulationConstantList = new List<string>();
        //            List<string> specifiedList = new List<string>();

        //            foreach (GridViewRow row in gv7.Rows)
        //            {
        //                if (row.RowType == DataControlRowType.DataRow)
        //                {
        //                    if (string.IsNullOrEmpty(dateVolumeMax))
        //                        dateVolumeMax = row.Cells[0].Text; // Date column

        //                    coreList.Add(row.Cells[1].Text);                 // Core
        //                    tempList.Add(row.Cells[2].Text);                 // Temperature
        //                    insulationResistanceList.Add(row.Cells[3].Text); // Insulation Resistance
        //                    volumeResistivityList.Add(row.Cells[4].Text);    // Volume Resistivity
        //                    insulationConstantList.Add(row.Cells[5].Text);   // Insulation Constant
        //                    specifiedList.Add(row.Cells[6].Text);            // Specified
        //                }
        //            }

        //            // Add specified values from textboxes
        //            string specifiedTemp = txtspecifiedtemp7.Text;
        //            string specifiedInsulationResistance = txtspecifiedInsulationresistance7.Text;
        //            string specifiedVolumeResistivity = TextBox3.Text;
        //            string specifiedInsulationConstant = TextBox4.Text;
        //            string combinedSpecifiedValues = $"{specifiedTemp},{specifiedInsulationResistance},{specifiedVolumeResistivity},{specifiedInsulationConstant}";

        //            reportParamMapList.Add("17273"); resultList.Add(dateVolumeMax);
        //            reportParamMapList.Add("17274"); resultList.Add(string.Join(",", coreList));
        //            reportParamMapList.Add("17275"); resultList.Add(string.Join(",", tempList));
        //            reportParamMapList.Add("17276"); resultList.Add(string.Join(",", insulationResistanceList));
        //            reportParamMapList.Add("17277"); resultList.Add(string.Join(",", volumeResistivityList));
        //            reportParamMapList.Add("17280"); resultList.Add(string.Join(",", insulationConstantList));
        //            reportParamMapList.Add("17283"); resultList.Add(string.Join(",", combinedSpecifiedValues));
        //        }

        //        // Convert lists to arrays and submit to DB
        //        string[] reportParamMap = reportParamMapList.ToArray();
        //        string[] result = resultList.ToArray();

        //        int modified = _cm.AddTestResultPTQualis(_cm, reportParamMap, result);

        //        if (modified > 0)
        //        {
        //            ScriptManager.RegisterStartupScript(this, this.GetType(), "showSuccess", "alert('Test results saved successfully!');", true);
        //            _cm.sampleStatusID = 27;
        //            _cm.UpdateCRFSampleDetailsStatus(_cm);
        //            _cm.StatusID = 27;
        //            _cm.UpdateAllocationStatusid_mechanical(_cm);

        //            Response.Redirect($"GenerateTestRecord_VoltageTestandInsulationTest.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
        //        }
        //        else
        //        {
        //            ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", "alert('Failed to save test results.');", true);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", $"alert('Error: {ex.Message}');", true);
        //    }
        //}

        //Clear Data
        private void Submitform()
        {
            try
            {
                // Get GridView data from ViewState
                DataTable dt = ViewState["TestResults"] as DataTable;

                if ((dt == null || dt.Rows.Count == 0) && gv1.Rows.Count == 0 && gv2.Rows.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", "alert('No test data to save. Please add results first.');", true);
                    return;
                }

                if (_cm == null) _cm = new DAL.CM();
                if (Request.QueryString["s"] != null) _cm.sampleId = Convert.ToInt32(Request.QueryString["s"].ToLower());
                if (Request.QueryString["r"] != null) _cm.ReportID = Request.QueryString["r"];

                DataTable dt1 = _cm.Getdataforseriesandreportid_mechanical(_cm);
                if (dt1.Rows.Count > 0)
                {
                    _cm.sampleIDSeries = dt1.Rows[0]["sampleIDSeries"].ToString();
                    _cm.CRFSampleId = Convert.ToInt32(dt1.Rows[0]["CRFSampleId"]);
                }

                List<string> reportParamMapList = new List<string>();
                List<string> resultList = new List<string>();

                // Upper Section
                reportParamMapList.Add("17204"); resultList.Add(cableType.Text);        // Type
                reportParamMapList.Add("17205"); resultList.Add(cableSize.Text);        // Size
                reportParamMapList.Add("17206"); resultList.Add(lblvoltage.Text);       // Voltage
                reportParamMapList.Add("17207"); resultList.Add(Date.Text);             // Date
                reportParamMapList.Add("17208"); resultList.Add(roomno.Text);           // Room No
                reportParamMapList.Add("17209"); resultList.Add(roomtemp.Text);         // Room Temp
                reportParamMapList.Add("17210"); resultList.Add(refSpec.Text);          // Ref Spec
                reportParamMapList.Add("17211"); resultList.Add(testMethodNo.Text);     // Test Method No
                reportParamMapList.Add("17212"); resultList.Add(jobOrderNo.Text);       // Job Order No
                reportParamMapList.Add("17213"); resultList.Add(worksheetno.Text);      // Worksheet No
                reportParamMapList.Add("17214"); resultList.Add(roomhumidity.Text);     // Room Humidity
                reportParamMapList.Add("17215"); resultList.Add(equipmentno.Text);      // Equipment No

                // 1st grid - AC Voltage Test
                if (gv1.Rows.Count > 0)
                {
                    List<string> dateList = new List<string>();
                    List<string> coreList = new List<string>();
                    List<string> lengthList = new List<string>();
                    List<string> immerseList = new List<string>();
                    List<string> tempList = new List<string>();
                    List<string> voltageList = new List<string>();
                    List<string> timeList = new List<string>();
                    List<string> resultACList = new List<string>();
                    List<string> specifiedList = new List<string>();

                    foreach (GridViewRow row in gv1.Rows)
                    {
                        if (row.RowType == DataControlRowType.DataRow)
                        {
                            dateList.Add(row.Cells[0].Text);            // Date column
                            coreList.Add(row.Cells[1].Text);            // Core
                            lengthList.Add(row.Cells[2].Text);          // Length
                            immerseList.Add(row.Cells[3].Text);         // Immerse
                            tempList.Add(row.Cells[4].Text);            // Temperature
                            voltageList.Add(row.Cells[5].Text);         // Voltage
                            timeList.Add(row.Cells[6].Text);            // Time
                            resultACList.Add(row.Cells[7].Text);        // Result
                            specifiedList.Add(row.Cells[8].Text);       // Specified
                        }
                    }

                    reportParamMapList.Add("17219"); resultList.Add(string.Join(",", dateList));
                    reportParamMapList.Add("17220"); resultList.Add(string.Join(",", coreList));
                    reportParamMapList.Add("17221"); resultList.Add(string.Join(",", lengthList));
                    reportParamMapList.Add("17222"); resultList.Add(string.Join(",", immerseList));
                    reportParamMapList.Add("17223"); resultList.Add(string.Join(",", tempList));
                    reportParamMapList.Add("17224"); resultList.Add(string.Join(",", voltageList));
                    reportParamMapList.Add("17225"); resultList.Add(string.Join(",", timeList));
                    reportParamMapList.Add("17226"); resultList.Add(string.Join(",", resultACList));
                    reportParamMapList.Add("17227"); resultList.Add(string.Join(",", specifiedList));
                }

                // 2nd grid - DC Voltage Test
                if (gv2.Rows.Count > 0)
                {
                    List<string> dateList = new List<string>();
                    List<string> coreList = new List<string>();
                    List<string> lengthList = new List<string>();
                    List<string> immerseList = new List<string>();
                    List<string> tempList = new List<string>();
                    List<string> voltageList = new List<string>();
                    List<string> timeList = new List<string>();
                    List<string> resultDCList = new List<string>();
                    List<string> specifiedList = new List<string>();

                    foreach (GridViewRow row in gv2.Rows)
                    {
                        if (row.RowType == DataControlRowType.DataRow)
                        {
                            dateList.Add(row.Cells[0].Text);            // Date column
                            coreList.Add(row.Cells[1].Text);            // Core
                            lengthList.Add(row.Cells[2].Text);          // Length
                            immerseList.Add(row.Cells[3].Text);         // Immerse
                            tempList.Add(row.Cells[4].Text);            // Temperature
                            voltageList.Add(row.Cells[5].Text);         // Voltage
                            timeList.Add(row.Cells[6].Text);            // Time
                            resultDCList.Add(row.Cells[7].Text);        // Result
                            specifiedList.Add(row.Cells[8].Text);       // Specified
                        }
                    }

                    reportParamMapList.Add("17228"); resultList.Add(string.Join(",", dateList));
                    reportParamMapList.Add("17229"); resultList.Add(string.Join(",", coreList));
                    reportParamMapList.Add("17230"); resultList.Add(string.Join(",", lengthList));
                    reportParamMapList.Add("17231"); resultList.Add(string.Join(",", immerseList));
                    reportParamMapList.Add("17232"); resultList.Add(string.Join(",", tempList));
                    reportParamMapList.Add("17233"); resultList.Add(string.Join(",", voltageList));
                    reportParamMapList.Add("17234"); resultList.Add(string.Join(",", timeList));
                    reportParamMapList.Add("17235"); resultList.Add(string.Join(",", resultDCList));
                    reportParamMapList.Add("17236"); resultList.Add(string.Join(",", specifiedList));
                }

                // 3rd grid - Insulation Resistance Test (Ambient)
                if (gv3.Rows.Count > 0)
                {
                    List<string> dateList = new List<string>();
                    List<string> coreList = new List<string>();
                    List<string> lengthList = new List<string>();
                    List<string> immerseList = new List<string>();
                    List<string> tempList = new List<string>();
                    List<string> measurementList = new List<string>();
                    List<string> insulationResistanceList = new List<string>();
                    List<string> specifiedList = new List<string>();

                    foreach (GridViewRow row in gv3.Rows)
                    {
                        if (row.RowType == DataControlRowType.DataRow)
                        {
                            dateList.Add(row.Cells[0].Text);                    // Date column
                            coreList.Add(row.Cells[1].Text);                    // Core
                            lengthList.Add(row.Cells[2].Text);                  // Length
                            immerseList.Add(row.Cells[3].Text);                 // Immerse
                            tempList.Add(row.Cells[4].Text);                    // Temperature
                            measurementList.Add(row.Cells[5].Text);             // Measurement
                            insulationResistanceList.Add(row.Cells[6].Text);    // Insulation Resistance
                            specifiedList.Add(row.Cells[7].Text);               // Specified
                        }
                    }

                    reportParamMapList.Add("17237"); resultList.Add(string.Join(",", dateList));
                    reportParamMapList.Add("17238"); resultList.Add(string.Join(",", coreList));
                    reportParamMapList.Add("17239"); resultList.Add(string.Join(",", lengthList));
                    reportParamMapList.Add("17240"); resultList.Add(string.Join(",", immerseList));
                    reportParamMapList.Add("17241"); resultList.Add(string.Join(",", tempList));
                    reportParamMapList.Add("17242"); resultList.Add(string.Join(",", measurementList));
                    reportParamMapList.Add("17243"); resultList.Add(string.Join(",", insulationResistanceList));
                    reportParamMapList.Add("17244"); resultList.Add(string.Join(",", specifiedList));
                }

                // 4th grid - Insulation Resistance Test (Max Temperature)
                if (gv4.Rows.Count > 0)
                {
                    List<string> dateList = new List<string>();
                    List<string> coreList = new List<string>();
                    List<string> lengthList = new List<string>();
                    List<string> immerseList = new List<string>();
                    List<string> tempList = new List<string>();
                    List<string> factorList = new List<string>();
                    List<string> measurementList = new List<string>();
                    List<string> insulationResistanceList = new List<string>();
                    List<string> specifiedList = new List<string>();

                    foreach (GridViewRow row in gv4.Rows)
                    {
                        if (row.RowType == DataControlRowType.DataRow)
                        {
                            dateList.Add(row.Cells[0].Text);                    // Date column
                            coreList.Add(row.Cells[1].Text);                    // Core
                            lengthList.Add(row.Cells[2].Text);                  // Length
                            immerseList.Add(row.Cells[3].Text);                 // Immerse
                            tempList.Add(row.Cells[4].Text);                    // Temperature
                            factorList.Add(row.Cells[5].Text);                  // Factor
                            measurementList.Add(row.Cells[6].Text);             // Measurement
                            insulationResistanceList.Add(row.Cells[7].Text);    // Insulation Resistance
                            specifiedList.Add(row.Cells[8].Text);               // Specified
                        }
                    }

                    reportParamMapList.Add("17245"); resultList.Add(string.Join(",", dateList));
                    reportParamMapList.Add("17246"); resultList.Add(string.Join(",", coreList));
                    reportParamMapList.Add("17247"); resultList.Add(string.Join(",", lengthList));
                    reportParamMapList.Add("17248"); resultList.Add(string.Join(",", immerseList));
                    reportParamMapList.Add("17249"); resultList.Add(string.Join(",", tempList));
                    reportParamMapList.Add("17283"); resultList.Add(string.Join(",", factorList));
                    reportParamMapList.Add("17250"); resultList.Add(string.Join(",", measurementList));
                    reportParamMapList.Add("17251"); resultList.Add(string.Join(",", insulationResistanceList));
                    reportParamMapList.Add("17252"); resultList.Add(string.Join(",", specifiedList));
                }

                // 5th grid - Diameter Test
                if (gv5.Rows.Count > 0)
                {
                    List<string> dateList = new List<string>();
                    List<string> coreList = new List<string>();
                    List<string> lengthList = new List<string>();
                    List<string> inner1List = new List<string>();
                    List<string> inner2List = new List<string>();
                    List<string> innerAvgList = new List<string>();
                    List<string> outer1List = new List<string>();
                    List<string> outer2List = new List<string>();
                    List<string> outerAvgList = new List<string>();

                    foreach (GridViewRow row in gv5.Rows)
                    {
                        if (row.RowType == DataControlRowType.DataRow)
                        {
                            dateList.Add(row.Cells[0].Text);            // Date column
                            coreList.Add(row.Cells[1].Text);            // Core
                            lengthList.Add(row.Cells[2].Text);          // Length
                            inner1List.Add(row.Cells[3].Text);          // Inner1
                            inner2List.Add(row.Cells[4].Text);          // Inner2
                            innerAvgList.Add(row.Cells[5].Text);        // InnerAvg
                            outer1List.Add(row.Cells[6].Text);          // Outer1
                            outer2List.Add(row.Cells[7].Text);          // Outer2
                            outerAvgList.Add(row.Cells[8].Text);        // OuterAvg
                        }
                    }

                    // Add specified values from textboxes
                    string specifiedLength = txtspecifiedlength5.Text;
                    string specifiedInnerDiameter = txtspecifiedInnerdiameter5.Text;
                    string specifiedOuterDiameter = txtspecifiedOuterdiameter5.Text;
                    string combinedSpecifiedValues = $"{specifiedLength},{specifiedInnerDiameter},{specifiedOuterDiameter}";

                    reportParamMapList.Add("17253"); resultList.Add(string.Join(",", dateList));
                    reportParamMapList.Add("17254"); resultList.Add(string.Join(",", coreList));
                    reportParamMapList.Add("17255"); resultList.Add(string.Join(",", lengthList));
                    reportParamMapList.Add("17256"); resultList.Add(string.Join(",", inner1List));
                    reportParamMapList.Add("17257"); resultList.Add(string.Join(",", inner2List));
                    reportParamMapList.Add("17258"); resultList.Add(string.Join(",", innerAvgList));
                    reportParamMapList.Add("17259"); resultList.Add(string.Join(",", outer1List));
                    reportParamMapList.Add("17260"); resultList.Add(string.Join(",", outer2List));
                    reportParamMapList.Add("17261"); resultList.Add(string.Join(",", outerAvgList));
                    reportParamMapList.Add("17262"); resultList.Add(combinedSpecifiedValues);
                }

                // 6th grid - Volume Resistivity Test (Ambient)
                if (gv6.Rows.Count > 0)
                {
                    List<string> dateList = new List<string>();
                    List<string> coreList = new List<string>();
                    List<string> tempList = new List<string>();
                    List<string> insulationResistanceList = new List<string>();
                    List<string> volumeResistivityList = new List<string>();
                    List<string> insulationConstantList = new List<string>();
                    List<string> specifiedList = new List<string>();

                    foreach (GridViewRow row in gv6.Rows)
                    {
                        if (row.RowType == DataControlRowType.DataRow)
                        {
                            dateList.Add(row.Cells[0].Text);                    // Date column
                            coreList.Add(row.Cells[1].Text);                    // Core
                            tempList.Add(row.Cells[2].Text);                    // Temperature
                            insulationResistanceList.Add(row.Cells[3].Text);    // Insulation Resistance
                            volumeResistivityList.Add(row.Cells[4].Text);       // Volume Resistivity
                            insulationConstantList.Add(row.Cells[5].Text);      // Insulation Constant
                            specifiedList.Add(row.Cells[6].Text);               // Specified
                        }
                    }

                    // Add specified values from textboxes
                    string specifiedTemp = txtspecifiedtemp6.Text;
                    string specifiedInsulationResistance = txtspecifiedInsulationresistance6.Text;
                    string specifiedVolumeResistivity = txtspecifiedVolumeresistivity6.Text;
                    string specifiedInsulationConstant = txtspecifiedInsulationresistantconstantKIat20C6.Text;
                    string combinedSpecifiedValues = $"{specifiedTemp},{specifiedInsulationResistance},{specifiedVolumeResistivity},{specifiedInsulationConstant}";

                    reportParamMapList.Add("17263"); resultList.Add(string.Join(",", dateList));
                    reportParamMapList.Add("17264"); resultList.Add(string.Join(",", coreList));
                    reportParamMapList.Add("17265"); resultList.Add(string.Join(",", tempList));
                    reportParamMapList.Add("17266"); resultList.Add(string.Join(",", insulationResistanceList));
                    reportParamMapList.Add("17267"); resultList.Add(string.Join(",", volumeResistivityList));
                    reportParamMapList.Add("17270"); resultList.Add(string.Join(",", insulationConstantList));
                    reportParamMapList.Add("17272"); resultList.Add(string.Join(",", combinedSpecifiedValues));
                }

                // 7th grid - Volume Resistivity Test (Max Temperature)
                if (gv7.Rows.Count > 0)
                {
                    List<string> dateList = new List<string>();
                    List<string> coreList = new List<string>();
                    List<string> tempList = new List<string>();
                    List<string> insulationResistanceList = new List<string>();
                    List<string> volumeResistivityList = new List<string>();
                    List<string> insulationConstantList = new List<string>();
                    List<string> specifiedList = new List<string>();

                    foreach (GridViewRow row in gv7.Rows)
                    {
                        if (row.RowType == DataControlRowType.DataRow)
                        {
                            dateList.Add(row.Cells[0].Text);                    // Date column
                            coreList.Add(row.Cells[1].Text);                    // Core
                            tempList.Add(row.Cells[2].Text);                    // Temperature
                            insulationResistanceList.Add(row.Cells[3].Text);    // Insulation Resistance
                            volumeResistivityList.Add(row.Cells[4].Text);       // Volume Resistivity
                            insulationConstantList.Add(row.Cells[5].Text);      // Insulation Constant
                            specifiedList.Add(row.Cells[6].Text);               // Specified
                        }
                    }

                    // Add specified values from textboxes
                    string specifiedTemp = txtspecifiedtemp7.Text;
                    string specifiedInsulationResistance = txtspecifiedInsulationresistance7.Text;
                    string specifiedVolumeResistivity = TextBox3.Text;
                    string specifiedInsulationConstant = TextBox4.Text;
                    string combinedSpecifiedValues = $"{specifiedTemp},{specifiedInsulationResistance},{specifiedVolumeResistivity},{specifiedInsulationConstant}";

                    reportParamMapList.Add("17273"); resultList.Add(string.Join(",", dateList));
                    reportParamMapList.Add("17274"); resultList.Add(string.Join(",", coreList));
                    reportParamMapList.Add("17275"); resultList.Add(string.Join(",", tempList));
                    reportParamMapList.Add("17276"); resultList.Add(string.Join(",", insulationResistanceList));
                    reportParamMapList.Add("17277"); resultList.Add(string.Join(",", volumeResistivityList));
                    reportParamMapList.Add("17280"); resultList.Add(string.Join(",", insulationConstantList));
                    reportParamMapList.Add("17283"); resultList.Add(string.Join(",", combinedSpecifiedValues));
                }

                // Convert lists to arrays and submit to DB
                string[] reportParamMap = reportParamMapList.ToArray();
                string[] result = resultList.ToArray();

                int modified = _cm.AddTestResultPTQualis(_cm, reportParamMap, result);

                if (modified > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showSuccess", "alert('Test results saved successfully!');", true);
                    _cm.sampleStatusID = 27;
                    _cm.UpdateCRFSampleDetailsStatus(_cm);
                    _cm.StatusID = 27;
                    _cm.UpdateAllocationStatusid_mechanical(_cm);

                    Response.Redirect($"GenerateTestRecord_VoltageTestandInsulationTest.aspx?s={_cm.sampleId}&r={_cm.ReportID}");
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", "alert('Failed to save test results.');", true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "showError", $"alert('Error: {ex.Message}');", true);
            }
        }
        private void ClearForm()
        {
            // AC Test Details
            testdate1.Text = "";
            ddlCore1.SelectedIndex = 0;
            txtLength1.Text = "";
            txtImmerse1.Text = "";
            txtTemp1.Text = "";
            txtVoltage1.Text = "";
            txtTime1.Text = "";

            // DC Test Details
            testdate2.Text = "";
            ddlCore2.SelectedIndex = 0;
            txtLength2.Text = "";
            txtImmerse2.Text = "";
            txtTemp2.Text = "";
            txtVoltage2.Text = "";
            txtTime2.Text = "";

            // Clear GridViews
            gv1.DataSource = null;
            gv1.DataBind();

            gv2.DataSource = null;
            gv2.DataBind();

        }
        private void ClearInputFieldsAC()
        {
            // Clear only AC Section input fields
            ddlCore1.SelectedIndex = 0;
            txtLength1.Text = "";
            txtImmerse1.Text = "";
            txtTemp1.Text = "";
            txtVoltage1.Text = "";
            txtresults1.Text = "";
            txtTime1.Text = "";
        }
        private void ClearInputFieldsDC()
        {
            // Clear only DC Section input fields
            txtresults2.Text = "";
            ddlCore2.SelectedIndex = 0;
            txtLength2.Text = "";
            txtImmerse2.Text = "";
            txtTemp2.Text = "";
            txtVoltage2.Text = "";
            txtTime2.Text = "";
        }
        private void ClearInputFields3()
        {
            txtLength3.Text = string.Empty;
            txtImmerse3.Text = string.Empty;
            txtTemp3.Text = string.Empty;
            txtInsulationResistance3.Text = string.Empty;
            txtMeasurement3.Text = string.Empty;
            ResetControlBorders3();
        }
        private void ClearInputFields4()
        {
            txtLength4.Text = string.Empty;
            txtImmerse4.Text = string.Empty;
            txtTemp4.Text = string.Empty;
            txtInsulationResistance4.Text = string.Empty;
            txtMeasurement4.Text = string.Empty;
            txtFactor4.Text = string.Empty;
            ResetControlBorders4();
        }
        private void ClearInputFields5()
        {
            txtLength5.Text = string.Empty;
            txtInner1.Text = string.Empty;
            txtInner2.Text = string.Empty;
            txtInnerAvg.Text = string.Empty;
            txtOuter1.Text = string.Empty;
            txtOuter2.Text = string.Empty;
            txtOuterAvg.Text = string.Empty;
            ResetControlBorders5();
        }
        private void ClearInputFields6()
        {
            txtTemp6.Text = string.Empty;
            txtInsulationResistance.Text = string.Empty;
            txtVolumeResistivity.Text = string.Empty;
            txtInsulationConstant.Text = string.Empty;
            ResetControlBorders6();
        }
        private void ClearInputFields7()
        {
            ddlCore7.SelectedIndex = -1;
            txtTemp7.Text = string.Empty;
            txtInsulationResistance7.Text = string.Empty;
            txtVolumeResistivity7.Text = string.Empty;
            txtInsulationConstant7.Text = string.Empty;
        }

        private void ResetControlBorders()
        {
            List<Control> allControls = new List<Control>
            {
                Date, roomno, roomhumidity, roomtemp,
                testdate1, ddlCore1, txtLength1, txtImmerse1, txtTemp1, txtVoltage1, txtTime1, txtresults1,
                testdate2, ddlCore2, txtLength2, txtImmerse2, txtTemp2, txtVoltage2, txtTime2, txtresults2,
                testdate3, ddlCore3, txtLength3, txtImmerse3, txtTemp3, txtMeasurement3, txtInsulationResistance3,
                testdate4, ddlCore4, txtLength4, txtImmerse4, txtTemp4, txtFactor4, txtMeasurement4, txtInsulationResistance4,
                testdate5, ddlCore5, txtLength5, txtInner1, txtInner2, txtInnerAvg, txtOuter1, txtOuter2, txtOuterAvg,
                testdate6, ddlCore6, txtTemp6, txtInsulationResistance, txtVolumeResistivity, txtInsulationConstant,
                testdate7, ddlCore7, txtTemp7, txtInsulationResistance7, txtVolumeResistivity7, txtInsulationConstant7
            };

            foreach (var control in allControls)
            {
                if (control is TextBox txt)
                {
                    txt.BorderColor = System.Drawing.Color.Empty;
                    txt.BorderStyle = BorderStyle.NotSet;
                    txt.BorderWidth = 0;
                }
                else if (control is DropDownList ddl)
                {
                    ddl.BorderColor = System.Drawing.Color.Empty;
                    ddl.BorderStyle = BorderStyle.NotSet;
                    ddl.BorderWidth = 0;
                }
            }
        }
        private void ResetControlBordersAC()
        {
            // Reset only 1st Section controls
            List<TextBox> acTextBoxes = new List<TextBox>
            {
                testdate1,
                txtLength1,
                txtImmerse1,
                txtTemp1,
                txtVoltage1,
                txtTime1
            };

            foreach (var textBox in acTextBoxes)
            {
                textBox.BorderColor = System.Drawing.Color.Empty;
                textBox.BorderStyle = BorderStyle.NotSet;
                textBox.BorderWidth = 0;
            }

            ddlCore1.BorderColor = System.Drawing.Color.Empty;
            ddlCore1.BorderStyle = BorderStyle.NotSet;
            ddlCore1.BorderWidth = 0;
        }
        private void ResetControlBordersDC()
        {
            // Reset only 2nd Section controls
            List<TextBox> dcTextBoxes = new List<TextBox>
            {
                testdate2,
                txtLength2,
                txtImmerse2,
                txtTemp2,
                txtVoltage2,
                txtTime2
            };

            foreach (var textBox in dcTextBoxes)
            {
                textBox.BorderColor = System.Drawing.Color.Empty;
                textBox.BorderStyle = BorderStyle.NotSet;
                textBox.BorderWidth = 0;
            }

            ddlCore2.BorderColor = System.Drawing.Color.Empty;
            ddlCore2.BorderStyle = BorderStyle.NotSet;
            ddlCore2.BorderWidth = 0;
        }
        private void ResetControlBorders3()
        {
            txtLength3.BorderColor = System.Drawing.Color.Empty;
            txtImmerse3.BorderColor = System.Drawing.Color.Empty;
            txtTemp3.BorderColor = System.Drawing.Color.Empty;
            txtInsulationResistance3.BorderColor = System.Drawing.Color.Empty;
            txtMeasurement3.BorderColor = System.Drawing.Color.Empty;
            txtspecified3.BorderColor = System.Drawing.Color.Empty;
        }
        private void ResetControlBorders4()
        {
            txtLength4.BorderColor = System.Drawing.Color.Empty;
            txtImmerse4.BorderColor = System.Drawing.Color.Empty;
            txtTemp4.BorderColor = System.Drawing.Color.Empty;
            txtInsulationResistance4.BorderColor = System.Drawing.Color.Empty;
            txtMeasurement4.BorderColor = System.Drawing.Color.Empty;
            txtspecified4.BorderColor = System.Drawing.Color.Empty;
            txtFactor4.BorderColor = System.Drawing.Color.Empty;
        }
        private void ResetControlBorders5()
        {
            testdate5.BorderColor = System.Drawing.Color.Empty;
            txtLength5.BorderColor = System.Drawing.Color.Empty;
            txtInner1.BorderColor = System.Drawing.Color.Empty;
            txtInner2.BorderColor = System.Drawing.Color.Empty;
            txtInnerAvg.BorderColor = System.Drawing.Color.Empty;
            txtOuter1.BorderColor = System.Drawing.Color.Empty;
            txtOuter2.BorderColor = System.Drawing.Color.Empty;
            txtOuterAvg.BorderColor = System.Drawing.Color.Empty;
            ddlCore5.CssClass = ddlCore5.CssClass.Replace(" input-error", "");
        }
        private void ResetControlBorders6()
        {
            testdate6.BorderColor = System.Drawing.Color.Empty;
            txtTemp6.BorderColor = System.Drawing.Color.Empty;
            txtInsulationResistance.BorderColor = System.Drawing.Color.Empty;
            txtVolumeResistivity.BorderColor = System.Drawing.Color.Empty;
            txtInsulationConstant.BorderColor = System.Drawing.Color.Empty;
            ddlCore6.CssClass = ddlCore6.CssClass.Replace(" input-error", "");
        }
        private void ResetControlBorders7()
        {
            testdate7.BorderColor = System.Drawing.Color.Empty;
            txtTemp7.BorderColor = System.Drawing.Color.Empty;
            txtInsulationResistance7.BorderColor = System.Drawing.Color.Empty;
            txtVolumeResistivity7.BorderColor = System.Drawing.Color.Empty;
            txtInsulationConstant7.BorderColor = System.Drawing.Color.Empty;
            ddlCore7.CssClass = ddlCore7.CssClass.Replace(" input-error", "");
        }

        //Others
        private void ShowMessage(string message)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + message + "');", true);
        }
        private void PopulateCoreDropdown()
        {
            try
            {
                // Get Core data from database
                DataTable coreData = _cm.Get_Core();

                // Clear existing items
                ddlCore1.Items.Clear();
                ddlCore2.Items.Clear();
                ddlCore3.Items.Clear();
                ddlCore4.Items.Clear();
                ddlCore5.Items.Clear();
                ddlCore6.Items.Clear();
                ddlCore7.Items.Clear();

                // Check if we got data
                if (coreData != null && coreData.Rows.Count > 0)
                {
                    // Bind data to dropdown
                    foreach (DataRow row in coreData.Rows)
                    {
                        string coreName = row["CoreName"].ToString();
                        string coreId = row["CoreID"].ToString();
                        ddlCore1.Items.Add(new ListItem(coreName, coreId));
                        ddlCore2.Items.Add(new ListItem(coreName, coreId));
                        ddlCore3.Items.Add(new ListItem(coreName, coreId));
                        ddlCore4.Items.Add(new ListItem(coreName, coreId));
                        ddlCore5.Items.Add(new ListItem(coreName, coreId));
                        ddlCore6.Items.Add(new ListItem(coreName, coreId));
                        ddlCore7.Items.Add(new ListItem(coreName, coreId));
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

        //PRE-FILLING DATA IF EXISTS
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

                    DataTable existingData = GetExistingTestResultsForSample(sampleId, reportId);

                    if (existingData != null && existingData.Rows.Count > 0)
                    {
                        btnSaveData.Visible = false;
                        btnClearData.Visible = false;
                        btnUpdateData.Visible = true;

                        PrefillUpperSection(existingData);
                        PrefillVoltageTestAC(existingData);
                        PrefillVoltageTestDC(existingData);
                        PrefillInsulationResistanceAmbient(existingData);
                        PrefillInsulationResistanceMaxTemp(existingData);
                        PrefillDiameterInsulation(existingData);
                        PrefillVolumeResistivityAmbient(existingData);
                        PrefillVolumeResistivityMaxTemp(existingData);
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "PrefillError",
                    $"alert('Error prefilling form: {ex.Message}');", true);
            }
        }
        private DataTable GetExistingTestResultsForSample(int sampleId, int reportId)
        {
            Dictionary<string, string> parameterResults = new Dictionary<string, string>();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    conn.Open();
                    string query = @"
                SELECT ReportParamMapID, Result 
                FROM TestResultsPTQualis 
                WHERE SampleID = @SampleID AND ReportID = @ReportID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SampleID", sampleId);
                        cmd.Parameters.AddWithValue("@ReportID", reportId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string paramId = reader["ReportParamMapID"].ToString();
                                string result = reader["Result"].ToString();
                                parameterResults[paramId] = result;
                            }
                        }
                    }
                }

                DataTable dt = new DataTable();
                dt.Columns.Add("ParameterID", typeof(string));
                dt.Columns.Add("Result", typeof(string));

                foreach (var param in parameterResults)
                {
                    dt.Rows.Add(param.Key, param.Value);
                }

                return dt;
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "DataFetchError",
                    $"alert('Error fetching existing data: {ex.Message}');", true);
                return null;
            }
        }
        private void PrefillUpperSection(DataTable existingData)
        {
            // Use ParameterID instead of ReportParamMapID
            SetLabelText(cableType, "17204");       // Type
            SetLabelText(refSpec, "17210");         // RefSpec
            SetLabelText(cableSize, "17205");       // Size
            SetLabelText(testMethodNo, "17211");    // Test Method No
            SetLabelText(lblvoltage, "17206");      // Voltage
            SetLabelText(jobOrderNo, "17212");      // Job Order No
            SetLabelText(worksheetno, "17213");     // Worksheet No
            SetLabelText(equipmentno, "17215");     // Equipment No

            // Textbox fields
            SetTextBoxText(roomno, "17208");        // Room No
            SetTextBoxText(roomhumidity, "17214");  // Room Humidity
            SetTextBoxText(roomtemp, "17209");      // Room Temperature
            SetTextBoxText(Date, "17207");          // Date

            // Helper to set Label controls
            void SetLabelText(Label label, string parameterId)
            {
                var row = existingData.Select($"ParameterID = '{parameterId}'").FirstOrDefault();
                if (row != null)
                {
                    label.Text = row["Result"].ToString();
                }
            }

            // Helper to set TextBox controls
            void SetTextBoxText(TextBox textBox, string parameterId)
            {
                var row = existingData.Select($"ParameterID = '{parameterId}'").FirstOrDefault();
                if (row != null)
                {
                    textBox.Text = row["Result"].ToString();
                }
            }
        }
        private void PrefillVoltageTestAC(DataTable data)
        {
            try
            {
                string date = GetParameterValue(data, "17219");
                string[] core = GetParameterValue(data, "17220").Split(',');
                string[] length = GetParameterValue(data, "17221").Split(',');
                string[] immerse = GetParameterValue(data, "17222").Split(',');
                string[] temp = GetParameterValue(data, "17223").Split(',');
                string[] voltage = GetParameterValue(data, "17224").Split(',');
                string[] time = GetParameterValue(data, "17225").Split(',');
                string[] result = GetParameterValue(data, "17226").Split(',');
                string[] specified = GetParameterValue(data, "17227").Split(',');

                DataTable dt = CreateVoltageTestDataTable();

                for (int i = 0; i < core.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(core[i])) continue;

                    DataRow row = dt.NewRow();
                    row["Date"] = date;
                    row["Core"] = core[i];
                    row["Length"] = i < length.Length ? length[i] : "";
                    row["Immerse"] = i < immerse.Length ? immerse[i] : "";
                    row["Temperature"] = i < temp.Length ? temp[i] : "";
                    row["Voltage"] = i < voltage.Length ? voltage[i] : "";
                    row["Time"] = i < time.Length ? time[i] : "";
                    row["Result"] = i < result.Length ? result[i] : "";
                    row["Specified"] = i < specified.Length ? specified[i] : "";

                    dt.Rows.Add(row);
                }

                ViewState["ACTestResults"] = dt;
                gv1.DataSource = dt;
                gv1.DataBind();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "PrefillACError",
                    $"alert('Error loading AC data: {ex.Message}');", true);
            }
        }
        private void PrefillVoltageTestDC(DataTable data)
        {
            try
            {
                string date = GetParameterValue(data, "17228");
                string[] core = GetParameterValue(data, "17229").Split(',');
                string[] length = GetParameterValue(data, "17230").Split(',');
                string[] immerse = GetParameterValue(data, "17231").Split(',');
                string[] temp = GetParameterValue(data, "17232").Split(',');
                string[] voltage = GetParameterValue(data, "17233").Split(',');
                string[] time = GetParameterValue(data, "17234").Split(',');
                string[] result = GetParameterValue(data, "17235").Split(',');
                string[] specified = GetParameterValue(data, "17236").Split(',');

                DataTable dt = CreateVoltageTestDataTable();

                for (int i = 0; i < core.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(core[i])) continue;

                    DataRow row = dt.NewRow();
                    row["Date"] = date;
                    row["Core"] = core[i];
                    row["Length"] = i < length.Length ? length[i] : "";
                    row["Immerse"] = i < immerse.Length ? immerse[i] : "";
                    row["Temperature"] = i < temp.Length ? temp[i] : "";
                    row["Voltage"] = i < voltage.Length ? voltage[i] : "";
                    row["Time"] = i < time.Length ? time[i] : "";
                    row["Result"] = i < result.Length ? result[i] : "";
                    row["Specified"] = i < specified.Length ? specified[i] : "";

                    dt.Rows.Add(row);
                }

                ViewState["DCTestResults"] = dt;
                gv2.DataSource = dt;
                gv2.DataBind();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "PrefillDCError",
                    $"alert('Error loading DC data: {ex.Message}');", true);
            }
        }
        private void PrefillInsulationResistanceAmbient(DataTable data)
        {
            try
            {
                string date = GetParameterValue(data, "17237");
                string[] core = GetParameterValue(data, "17238").Split(',');
                string[] length = GetParameterValue(data, "17239").Split(',');
                string[] immerse = GetParameterValue(data, "17240").Split(',');
                string[] temp = GetParameterValue(data, "17241").Split(',');
                string[] measurement = GetParameterValue(data, "17242").Split(',');
                string[] insulationResistance = GetParameterValue(data, "17243").Split(',');
                string[] specified = GetParameterValue(data, "17244").Split(',');

                DataTable dt = CreateInsulationResistanceAmbientDataTable();

                for (int i = 0; i < core.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(core[i])) continue;

                    DataRow row = dt.NewRow();
                    row["Date"] = date;
                    row["Core"] = core[i];
                    row["Length"] = i < length.Length ? length[i] : "";
                    row["Immerse"] = i < immerse.Length ? immerse[i] : "";
                    row["Temperature"] = i < temp.Length ? temp[i] : "";
                    row["Measurement"] = i < measurement.Length ? measurement[i] : "";
                    row["InsulationResistance"] = i < insulationResistance.Length ? insulationResistance[i] : "";
                    row["Specified"] = i < specified.Length ? specified[i] : "";

                    dt.Rows.Add(row);
                }

                ViewState["InsulationAmbientTestResults"] = dt;
                gv3.DataSource = dt;
                gv3.DataBind();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "PrefillAmbientError",
                    $"alert('Error loading ambient temperature data: {ex.Message}');", true);
            }
        }
        private void PrefillInsulationResistanceMaxTemp(DataTable data)
        {
            try
            {
                string date = GetParameterValue(data, "17245");
                string[] core = GetParameterValue(data, "17246").Split(',');
                string[] length = GetParameterValue(data, "17247").Split(',');
                string[] immerse = GetParameterValue(data, "17248").Split(',');
                string[] temp = GetParameterValue(data, "17249").Split(',');
                string[] factor = GetParameterValue(data, "17283").Split(',');
                string[] measurement = GetParameterValue(data, "17250").Split(',');
                string[] insulationResistance = GetParameterValue(data, "17251").Split(',');
                string[] specified = GetParameterValue(data, "17252").Split(',');

                DataTable dt = CreateInsulationResistanceMaxTempDataTable();

                for (int i = 0; i < core.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(core[i])) continue;

                    DataRow row = dt.NewRow();
                    row["Date"] = date;
                    row["Core"] = core[i];
                    row["Length"] = i < length.Length ? length[i] : "";
                    row["Immerse"] = i < immerse.Length ? immerse[i] : "";
                    row["Temperature"] = i < temp.Length ? temp[i] : "";
                    row["Factor"] = i < factor.Length ? factor[i] : "";
                    row["Measurement"] = i < measurement.Length ? measurement[i] : "";
                    row["InsulationResistance"] = i < insulationResistance.Length ? insulationResistance[i] : "";
                    row["Specified"] = i < specified.Length ? specified[i] : "";

                    dt.Rows.Add(row);
                }

                ViewState["InsulationMaxTempTestResults"] = dt;
                gv4.DataSource = dt;
                gv4.DataBind();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "PrefillMaxTempError",
                    $"alert('Error loading max temperature data: {ex.Message}');", true);
            }
        }
        private void PrefillDiameterInsulation(DataTable data)
        {
            try
            {
                string date = GetParameterValue(data, "17253");
                string[] core = GetParameterValue(data, "17254").Split(',');
                string[] length = GetParameterValue(data, "17255").Split(',');
                string[] inner1 = GetParameterValue(data, "17256").Split(',');
                string[] inner2 = GetParameterValue(data, "17257").Split(',');
                string[] innerAvg = GetParameterValue(data, "17258").Split(',');
                string[] outer1 = GetParameterValue(data, "17259").Split(',');
                string[] outer2 = GetParameterValue(data, "17260").Split(',');
                string[] outerAvg = GetParameterValue(data, "17261").Split(',');
                string[] specifiedValues = GetParameterValue(data, "17262").Split(',');

                DataTable dt = CreateDiameterInsulationDataTable();

                for (int i = 0; i < core.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(core[i])) continue;

                    DataRow row = dt.NewRow();
                    row["Date"] = date;
                    row["Core"] = core[i];
                    row["Length"] = i < length.Length ? length[i] : "";
                    row["Inner1"] = i < inner1.Length ? inner1[i] : "";
                    row["Inner2"] = i < inner2.Length ? inner2[i] : "";
                    row["InnerAvg"] = i < innerAvg.Length ? innerAvg[i] : "";
                    row["Outer1"] = i < outer1.Length ? outer1[i] : "";
                    row["Outer2"] = i < outer2.Length ? outer2[i] : "";
                    row["OuterAvg"] = i < outerAvg.Length ? outerAvg[i] : "";

                    dt.Rows.Add(row);
                }

                // Set specified values to textboxes
                if (specifiedValues.Length >= 3)
                {
                    txtspecifiedlength5.Text = specifiedValues[0];
                    txtspecifiedInnerdiameter5.Text = specifiedValues[1];
                    txtspecifiedOuterdiameter5.Text = specifiedValues[2];
                }

                ViewState["DiameterTestResults"] = dt;
                gv5.DataSource = dt;
                gv5.DataBind();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "PrefillDiameterError",
                    $"alert('Error loading diameter data: {ex.Message}');", true);
            }
        }
        private void PrefillVolumeResistivityAmbient(DataTable data)
        {
            try
            {
                string date = GetParameterValue(data, "17263");
                string[] core = GetParameterValue(data, "17264").Split(',');
                string[] temp = GetParameterValue(data, "17265").Split(',');
                string[] insulationResistance = GetParameterValue(data, "17266").Split(',');
                string[] volumeResistivity = GetParameterValue(data, "17267").Split(',');
                string[] insulationConstant = GetParameterValue(data, "17270").Split(',');
                string[] specifiedValues = GetParameterValue(data, "17272").Split(',');

                DataTable dt = CreateVolumeResistivityAmbientDataTable();

                for (int i = 0; i < core.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(core[i])) continue;

                    DataRow row = dt.NewRow();
                    row["Date"] = date;
                    row["Core"] = core[i];
                    row["Temperature"] = i < temp.Length ? temp[i] : "";
                    row["InsulationResistance"] = i < insulationResistance.Length ? insulationResistance[i] : "";
                    row["VolumeResistivity"] = i < volumeResistivity.Length ? volumeResistivity[i] : "";
                    row["InsulationConstant"] = i < insulationConstant.Length ? insulationConstant[i] : "";

                    dt.Rows.Add(row);
                }

                // Set specified values to textboxes
                if (specifiedValues.Length >= 4)
                {
                    txtspecifiedtemp6.Text = specifiedValues[0];
                    txtspecifiedInsulationresistance6.Text = specifiedValues[1];
                    txtspecifiedVolumeresistivity6.Text = specifiedValues[2];
                    txtspecifiedInsulationresistantconstantKIat20C6.Text = specifiedValues[3];
                }

                ViewState["ResistivityTestResults"] = dt;
                gv6.DataSource = dt;
                gv6.DataBind();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "PrefillVolumeAmbientError",
                    $"alert('Error loading volume resistivity ambient data: {ex.Message}');", true);
            }
        }
        private void PrefillVolumeResistivityMaxTemp(DataTable data)
        {
            try
            {
                string date = GetParameterValue(data, "17273");
                string[] core = GetParameterValue(data, "17274").Split(',');
                string[] temp = GetParameterValue(data, "17275").Split(',');
                string[] insulationResistance = GetParameterValue(data, "17276").Split(',');
                string[] volumeResistivity = GetParameterValue(data, "17277").Split(',');
                string[] insulationConstant = GetParameterValue(data, "17280").Split(',');
                string[] specifiedValues = GetParameterValue(data, "17283").Split(',');

                DataTable dt = CreateVolumeResistivityMaxTempDataTable();

                for (int i = 0; i < core.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(core[i])) continue;

                    DataRow row = dt.NewRow();
                    row["Date"] = date;
                    row["Core"] = core[i];
                    row["Temperature"] = i < temp.Length ? temp[i] : "";
                    row["InsulationResistance"] = i < insulationResistance.Length ? insulationResistance[i] : "";
                    row["VolumeResistivity"] = i < volumeResistivity.Length ? volumeResistivity[i] : "";
                    row["InsulationConstant"] = i < insulationConstant.Length ? insulationConstant[i] : "";

                    dt.Rows.Add(row);
                }

                // Set specified values to textboxes
                if (specifiedValues.Length >= 4)
                {
                    txtspecifiedtemp7.Text = specifiedValues[0];
                    txtspecifiedInsulationresistance7.Text = specifiedValues[1];
                    TextBox3.Text = specifiedValues[2]; // Volume resistivity
                    TextBox4.Text = specifiedValues[3]; // Insulation constant
                }

                ViewState["ResistivityMaxTempTestResults"] = dt;
                gv7.DataSource = dt;
                gv7.DataBind();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "PrefillVolumeMaxTempError",
                    $"alert('Error loading volume resistivity max temp data: {ex.Message}');", true);
            }
        }
        private string GetParameterValue(DataTable data, string paramId)
        {
            var row = data.Select($"ParameterID = '{paramId}'").FirstOrDefault();
            return row != null ? row["Result"].ToString() : "";
        }
        private DataTable CreateVoltageTestDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Date", typeof(string));
            dt.Columns.Add("Core", typeof(string));
            dt.Columns.Add("Length", typeof(string));
            dt.Columns.Add("Immerse", typeof(string));
            dt.Columns.Add("Temperature", typeof(string));
            dt.Columns.Add("Voltage", typeof(string));
            dt.Columns.Add("Time", typeof(string));
            dt.Columns.Add("Result", typeof(string));
            dt.Columns.Add("Specified", typeof(string));
            return dt;
        }
        private DataTable CreateInsulationResistanceAmbientDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Date");
            dt.Columns.Add("Core");
            dt.Columns.Add("Length");
            dt.Columns.Add("Immerse");
            dt.Columns.Add("Temperature");
            dt.Columns.Add("Measurement");
            dt.Columns.Add("InsulationResistance");
            dt.Columns.Add("Specified");
            return dt;
        }
        private DataTable CreateInsulationResistanceMaxTempDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Date");
            dt.Columns.Add("Core");
            dt.Columns.Add("Length");
            dt.Columns.Add("Immerse");
            dt.Columns.Add("Temperature");
            dt.Columns.Add("Factor");
            dt.Columns.Add("Measurement");
            dt.Columns.Add("InsulationResistance");
            dt.Columns.Add("Specified");
            return dt;
        }
        private DataTable CreateDiameterInsulationDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Date");
            dt.Columns.Add("Core");
            dt.Columns.Add("Length");
            dt.Columns.Add("Inner1");
            dt.Columns.Add("Inner2");
            dt.Columns.Add("InnerAvg");
            dt.Columns.Add("Outer1");
            dt.Columns.Add("Outer2");
            dt.Columns.Add("OuterAvg");
            return dt;
        }
        private DataTable CreateVolumeResistivityAmbientDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Date");
            dt.Columns.Add("Core");
            dt.Columns.Add("Temperature");
            dt.Columns.Add("InsulationResistance");
            dt.Columns.Add("VolumeResistivity");
            dt.Columns.Add("InsulationConstant");
            return dt;
        }
        private DataTable CreateVolumeResistivityMaxTempDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Date");
            dt.Columns.Add("Core");
            dt.Columns.Add("Temperature");
            dt.Columns.Add("InsulationResistance");
            dt.Columns.Add("VolumeResistivity");
            dt.Columns.Add("InsulationConstant");
            return dt;
        }
    }
}