using LIMS.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace LIMS
{
    public partial class GenerateTestRecord_VoltageTestandInsulationTest : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usertype"] != null)
            {
                if (Request.QueryString["s"] != null)
                {
                    Session["SampleID"] = Request.QueryString["s"].ToString();
                }
                if (Request.QueryString["r"] != null)
                {
                    Session["ReportId"] = Request.QueryString["r"].ToString();
                }
                if (Session["SampleID"] != null && Session["ReportId"] != null)
                {
                    int SampleID = Convert.ToInt32(Session["SampleID"].ToString());
                    int ReportId = Convert.ToInt32(Session["ReportId"].ToString());

                    if (Session["Usertype"] != null)
                    {

                        // Define data structures
                        List<string> acTestCores = new List<string>();
                        List<string> acTestLengths = new List<string>();
                        List<string> acTestImmerseTimes = new List<string>();
                        List<string> acTestTemperatures = new List<string>();
                        List<string> acTestVoltages = new List<string>();
                        List<string> acTestTimes = new List<string>();
                        List<string> acTestResults = new List<string>();
                        List<string> acTestSpecified = new List<string>();

                        List<string> dcTestCores = new List<string>();
                        List<string> dcTestLengths = new List<string>();
                        List<string> dcTestImmerseTimes = new List<string>();
                        List<string> dcTestTemperatures = new List<string>();
                        List<string> dcTestVoltages = new List<string>();
                        List<string> dcTestTimes = new List<string>();
                        List<string> dcTestResults = new List<string>();
                        List<string> dcTestSpecified = new List<string>();

                        List<string> irAmbientDate = new List<string>();
                        List<string> irAmbientCores = new List<string>();
                        List<string> irAmbientLengths = new List<string>();
                        List<string> irAmbientImmerseTimes = new List<string>();
                        List<string> irAmbientTemperatures = new List<string>();
                        List<string> irAmbientMeasurements = new List<string>();
                        List<string> irAmbientInsulationResistance = new List<string>();
                        List<string> irAmbientSpecified = new List<string>();

                        // Define data structures
                        List<string> irMaxTempDate = new List<string>();
                        List<string> irMaxTempCores = new List<string>();
                        List<string> irMaxTempLengths = new List<string>();
                        List<string> irMaxTempImmerseTimes = new List<string>();
                        List<string> irMaxTempTemperatures = new List<string>();
                        List<string> irMaxTempFactors = new List<string>();
                        List<string> irMaxTempMeasurements = new List<string>();
                        List<string> irMaxTempInsulationResistance = new List<string>();
                        List<string> irMaxTempSpecified = new List<string>();

                        // Define data structures
                        List<string> diameterDate = new List<string>();
                        List<string> diameterCores = new List<string>();
                        List<string> diameterLengths = new List<string>();
                        List<string> diameterInner1 = new List<string>();
                        List<string> diameterInner2 = new List<string>();
                        List<string> diameterInnerAvg = new List<string>();
                        List<string> diameterOuter1 = new List<string>();
                        List<string> diameterOuter2 = new List<string>();
                        List<string> diameterOuterAvg = new List<string>();
                        List<string> diameterSpecified = new List<string>();

                        // Define data structures
                        List<string> resistivityDate = new List<string>();
                        List<string> resistivityCores = new List<string>();
                        List<string> resistivityTemperatures = new List<string>();
                        List<string> resistivityInsulationResistance = new List<string>();
                        List<string> resistivityVolumeResistivity = new List<string>();
                        List<string> resistivityInsulationConstant = new List<string>();
                        List<string> resistivitySpecified = new List<string>();

                        // Define data structures
                        List<string> resistivityMaxDate = new List<string>();
                        List<string> resistivityMaxCores = new List<string>();
                        List<string> resistivityMaxTemperatures = new List<string>();
                        List<string> resistivityMaxInsulationResistance = new List<string>();
                        List<string> resistivityMaxVolumeResistivity = new List<string>();
                        List<string> resistivityMaxInsulationConstant = new List<string>();
                        List<string> resistivityMaxSpecified = new List<string>();


                        string acTestDate = "";
                        string dcTestDate = "";

                        string Type = "";
                        string Size = "";
                        string Voltage = "";
                        string Date = "";
                        string RoomNo = "";
                        string RoomTemp = "";
                        string RefSpec = "";
                        string TestMethodNo = "";
                        string JobOrderNo = "";
                        string WorkSheetNo = "";
                        string RoomHumidity = "";
                        string EquipmentNo = "";

                        int InquiryID = 0;

                        DataTable dt6 = new DataTable();
                        DataTable dtRecordDetails = new DataTable();
                        StringBuilder sbHeader = new StringBuilder();
                        StringBuilder sbDetails = new StringBuilder();
                        StringBuilder sbDates = new StringBuilder();
                        StringBuilder sbFooter = new StringBuilder();
                        StringBuilder sbCustomer = new StringBuilder();
                        StringBuilder sb = new StringBuilder();
                        mailer objEmail = new mailer();
                        StringReader sr = new StringReader(sb.ToString());

                        string body = ""; string str = ""; string filename = ""; string html = "";
                        int TestRequestReviewID = 0;

                        dtRecordDetails = _cm.getTestRecord(ReportId, SampleID);
                        if (dtRecordDetails.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtRecordDetails.Rows)
                            {
                                // 1st Grid
                                if (dr["ReportParamMapID"].ToString() == "17219") { acTestDate = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17220") { acTestCores = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17221") { acTestLengths = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17222") { acTestImmerseTimes = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17223") { acTestTemperatures = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17224") { acTestVoltages = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17225") { acTestTimes = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17226") { acTestResults = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17227") { acTestSpecified = new List<string>(dr["Result"].ToString().Split(',')); }

                                // 2nd Grid
                                if (dr["ReportParamMapID"].ToString() == "17228") { dcTestDate = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17229") { dcTestCores = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17230") { dcTestLengths = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17231") { dcTestImmerseTimes = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17232") { dcTestTemperatures = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17233") { dcTestVoltages = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17234") { dcTestTimes = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17235") { dcTestResults = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17236") { dcTestSpecified = new List<string>(dr["Result"].ToString().Split(',')); }

                                // 3rd Grid - Populate from database
                                if (dr["ReportParamMapID"].ToString() == "17237") { irAmbientDate.Add(dr["Result"].ToString()); }
                                if (dr["ReportParamMapID"].ToString() == "17238") { irAmbientCores = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17239") { irAmbientLengths = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17240") { irAmbientImmerseTimes = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17241") { irAmbientTemperatures = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17242") { irAmbientMeasurements = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17243") { irAmbientInsulationResistance = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17244") { irAmbientSpecified = new List<string>(dr["Result"].ToString().Split(',')); }

                                // 4th Grid - Populate from database
                                if (dr["ReportParamMapID"].ToString() == "17245") { irMaxTempDate.Add(dr["Result"].ToString()); }
                                if (dr["ReportParamMapID"].ToString() == "17246") { irMaxTempCores = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17247") { irMaxTempLengths = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17248") { irMaxTempImmerseTimes = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17249") { irMaxTempTemperatures = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17283") { irMaxTempFactors = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17250") { irMaxTempMeasurements = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17251") { irMaxTempInsulationResistance = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17252") { irMaxTempSpecified = new List<string>(dr["Result"].ToString().Split(',')); }

                                // 5th Grid - Populate from database
                                if (dr["ReportParamMapID"].ToString() == "17253") { diameterDate.Add(dr["Result"].ToString()); }
                                if (dr["ReportParamMapID"].ToString() == "17254") { diameterCores = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17255") { diameterLengths = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17256") { diameterInner1 = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17257") { diameterInner2 = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17258") { diameterInnerAvg = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17259") { diameterOuter1 = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17260") { diameterOuter2 = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17261") { diameterOuterAvg = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17262")
                                {
                                    string[] specifiedValues = dr["Result"].ToString().Split(',');
                                    if (specifiedValues.Length >= 3)
                                    {
                                        diameterSpecified = new List<string>
                                        {
                                            specifiedValues[0], // Length
                                            specifiedValues[1], // Inner diameter
                                            specifiedValues[2]  // Outer diameter
                                        };
                                    }
                                }

                                // 6th Grid - Populate from database
                                if (dr["ReportParamMapID"].ToString() == "17263") { resistivityDate.Add(dr["Result"].ToString()); }
                                if (dr["ReportParamMapID"].ToString() == "17264") { resistivityCores = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17265") { resistivityTemperatures = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17266") { resistivityInsulationResistance = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17267") { resistivityVolumeResistivity = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17270") { resistivityInsulationConstant = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17272")
                                {
                                    string[] specifiedValues = dr["Result"].ToString().Split(',');
                                    if (specifiedValues.Length >= 4)
                                    {
                                        resistivitySpecified = new List<string>
                                        {
                                            specifiedValues[0], // Temperature
                                            specifiedValues[1], // Insulation Resistance
                                            specifiedValues[2], // Volume Resistivity
                                            specifiedValues[3]  // Insulation Constant
                                        };
                                    }
                                }

                                // 7th Grid - Populate from database
                                if (dr["ReportParamMapID"].ToString() == "17273") { resistivityMaxDate.Add(dr["Result"].ToString()); }
                                if (dr["ReportParamMapID"].ToString() == "17274") { resistivityMaxCores = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17275") { resistivityMaxTemperatures = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17276") { resistivityMaxInsulationResistance = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17277") { resistivityMaxVolumeResistivity = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17280") { resistivityMaxInsulationConstant = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "17283")
                                {
                                    string[] specifiedValues = dr["Result"].ToString().Split(',');
                                    if (specifiedValues.Length >= 4)
                                    {
                                        resistivityMaxSpecified = new List<string>
                                        {
                                            specifiedValues[0], // Temperature
                                            specifiedValues[1], // Insulation Resistance
                                            specifiedValues[2], // Volume Resistivity
                                            specifiedValues[3]  // Insulation Constant
                                        };
                                    }
                                }



                                // Header Information
                                if (dr["ReportParamMapID"].ToString() == "17204") { Type = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17205") { Size = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17206") { Voltage = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17207") { Date = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17208") { RoomNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17209") { RoomTemp = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17210") { RefSpec = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17211") { TestMethodNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17212") { JobOrderNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17213") { WorkSheetNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17214") { RoomHumidity = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "17215") { EquipmentNo = dr["Result"].ToString(); }
                            }
                        }
                        // Fix the HTML syntax errors in sbDetails
                        sbHeader.Append(@"<table style='width: 100%; border: 1px solid; margin-top: 10px;' class='tab1'>
                        <tr>
                            <td style='text-align:left;width:20%;'>Type</td>
                            <td style='width:30%;'>" + Type + @"</td>
                            <td style='text-align:left;width:20%;'>Ref Spec.</td>
                            <td style='width:30%;'>" + RefSpec + @"</td>
                        </tr>
                        <tr>
                            <td style='text-align:left;width:20%;'>Size</td>
                            <td>" + Size + @"</td>
                            <td style='text-align:left;width:20%;'>Test Method No.</td>
                            <td>" + TestMethodNo + @"</td>
                        </tr>
                        <tr>
                            <td style='text-align:left;width:20%;'>Voltage</td>
                            <td>" + Voltage + @"</td>
                            <td style='text-align:left;width:20%;'>Job Order No.</td>
                            <td>" + JobOrderNo + @"</td>
                        </tr>
                        <tr>
                            <td style='text-align:left;width:20%;'>Date</td>
                            <td>" + Date + @"</td>
                            <td style='text-align:left;width:20%;'>Work Sheet No.</td>
                            <td>" + WorkSheetNo + @"</td>
                        </tr>
                        <tr>
                            <td style='text-align:left;width:20%;'>Room No.</td>
                            <td>" + RoomNo + @"</td>
                            <td style='text-align:left;width:20%;'>Room Humidity</td>
                            <td>" + RoomHumidity + "%" + @"</td>
                        </tr>
                        <tr>
                            <td style='text-align:left;width:20%;'>Room Temp.</td>
                            <td>" + RoomTemp + "°C" + @"</td>
                            <td style='text-align:left;width:20%;'>Equipment No.</td>
                            <td>" + EquipmentNo + @"</td>
                        </tr>
                    </table>");


                        // 1st grid Test Results 
                        if (acTestCores.Count > 0)
                        {
                            sbDetails.Append(@"
                            <table class='tab1' style='width:100%;border-collapse:collapse;'>
                                <thead>
                                <tr style='background-color:#bebebe;font-weight:bold;'>
                                        <th colspan='9' style='border:1px solid;padding:5px;'>1. Voltage Test on complete cable</th>
                                </tr>
                                    <tr style='background-color:#bebebe;font-weight:bold;'>
                                        <th rowspan='2' style='border:1px solid;padding:5px;text-align: center;'>Date</th>
                                        <th rowspan='2' style='border:1px solid;padding:5px;text-align: center;'>Core</th>
                                        <th style='border:1px solid;padding:5px;text-align: center;'>Length</th>
                                        <th style='border:1px solid;padding:5px;text-align: center;'>Immerse</th>
                                        <th style='border:1px solid;padding:5px;text-align: center;'>Temp.</th>
                                        <th style='border:1px solid;padding:5px;text-align: center;'>Voltage</th>
                                        <th style='border:1px solid;padding:5px;text-align: center;'>Time</th>
                                        <th rowspan='2' style='border:1px solid;padding:5px;text-align: center;'>Result</th>
                                        <th rowspan='2' style='border:1px solid;padding:5px;text-align: center;'>Specified</th>
                                    </tr>
                                  </tr>
                                    <tr style='background-color:#bebebe;font-weight:bold;'>
                                        <th style='border:1px solid;padding:5px;text-align: center;'>(m)</th>
                                        <th style='border:1px solid;padding:5px;text-align: center;'>(hour)</th>
                                        <th style='border:1px solid;padding:5px;text-align: center;'>(°C)</th>
                                        <th style='border:1px solid;padding:5px;text-align: center;'>(kV)</th>
                                        <th style='border:1px solid;padding:5px;text-align: center;'>(minute)</th>
                                    </tr>
                                </thead>
                                <tbody>");

                            // Split dates and process each group
                            string[] acDates = acTestDate.Split(',');

                            // Create a mapping of date to row indices
                            Dictionary<string, List<int>> dateRowMapping = new Dictionary<string, List<int>>();

                            for (int i = 0; i < acDates.Length && i < acTestCores.Count; i++)
                            {
                                string trimmedDate = acDates[i % acDates.Length].Trim();

                                if (!dateRowMapping.ContainsKey(trimmedDate))
                                {
                                    dateRowMapping[trimmedDate] = new List<int>();
                                }
                                dateRowMapping[trimmedDate].Add(i);
                            }

                            // Track which rows have been processed
                            HashSet<int> processedRows = new HashSet<int>();

                            // Process each unique date group
                            foreach (var dateGroup in dateRowMapping.OrderBy(x => x.Value.Min()))
                            {
                                string currentDate = dateGroup.Key;
                                List<int> rowIndices = dateGroup.Value;

                                for (int i = 0; i < rowIndices.Count; i++)
                                {
                                    int rowIndex = rowIndices[i];

                                    if (processedRows.Contains(rowIndex)) continue;

                                    sbDetails.Append("<tr>");

                                    // Add Date column with rowspan for first occurrence of this date
                                    if (i == 0)
                                    {
                                        sbDetails.Append("<td rowspan='" + rowIndices.Count + "' style='border:1px solid;padding:5px;text-align:center;'>" + currentDate + "</td>");
                                    }

                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + acTestCores[rowIndex] + "</td>");
                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (rowIndex < acTestLengths.Count ? acTestLengths[rowIndex] : "") + "</td>");
                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (rowIndex < acTestImmerseTimes.Count ? acTestImmerseTimes[rowIndex] : "") + "</td>");
                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (rowIndex < acTestTemperatures.Count ? acTestTemperatures[rowIndex] : "") + "</td>");
                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (rowIndex < acTestVoltages.Count ? acTestVoltages[rowIndex] : "") + "</td>");
                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (rowIndex < acTestTimes.Count ? acTestTimes[rowIndex] : "") + "</td>");
                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (rowIndex < acTestResults.Count ? acTestResults[rowIndex] : "") + "</td>");

                                    // Add Specified column with rowspan for the first row only
                                    if (rowIndex == 0)
                                    {
                                        sbDetails.Append("<td rowspan='" + acTestCores.Count + "' style='border:1px solid;padding:5px;text-align:center;'>" + (acTestSpecified.Count > 0 ? acTestSpecified[0] : "") + "</td>");
                                    }

                                    sbDetails.Append("</tr>");
                                    processedRows.Add(rowIndex);
                                }
                            }

                            sbDetails.Append(@"</tbody></table>");
                        }

                        // 2nd grid Test Results 
                        if (dcTestCores.Count > 0)
                        {
                            sbDetails.Append(@"
                            <table class='tab1' style='width:100%;border-collapse:collapse;'>
                                <thead>
                                <tr style='background-color:#bebebe;font-weight:bold;'>
                                        <th colspan='9' style='border:1px solid;padding:5px;'>2. Voltage Test on insulation</th>
                                </tr>
                                    <tr style='background-color:#bebebe;font-weight:bold;'>
                                        <th rowspan='2' style='border:1px solid;padding:5px;text-align: center;'>Date</th>
                                        <th rowspan='2' style='border:1px solid;padding:5px;text-align: center;'>Core</th>
                                        <th style='border:1px solid;padding:5px;text-align: center;'>Length</th>
                                        <th style='border:1px solid;padding:5px;text-align: center;'>Immerse</th>
                                        <th style='border:1px solid;padding:5px;text-align: center;'>Temp.</th>
                                        <th style='border:1px solid;padding:5px;text-align: center;'>Voltage</th>
                                        <th style='border:1px solid;padding:5px;text-align: center;'>Time</th>
                                        <th rowspan='2' style='border:1px solid;padding:5px;text-align: center;'>Result</th>
                                        <th rowspan='2' style='border:1px solid;padding:5px;text-align: center;'>Specified</th>
                                    </tr>
                                  </tr>
                                    <tr style='background-color:#bebebe;font-weight:bold;'>
                                        <th style='border:1px solid;padding:5px;text-align: center;'>(m)</th>
                                        <th style='border:1px solid;padding:5px;text-align: center;'>(hour)</th>
                                        <th style='border:1px solid;padding:5px;text-align: center;'>(°C)</th>
                                        <th style='border:1px solid;padding:5px;text-align: center;'>(kV)</th>
                                        <th style='border:1px solid;padding:5px;text-align: center;'>(minute)</th>
                                    </tr>
                                </thead>
                                <tbody>");

                            string[] dcDates = dcTestDate.Split(',');
                            string previousDate = "";
                            int dateSpanCount = 0;

                            // First pass: calculate rowspan for each unique date
                            List<int> dateRowSpans = new List<int>();
                            for (int i = 0; i < dcTestCores.Count; i++)
                            {
                                string currentDate = dcDates[i % dcDates.Length].Trim();

                                if (i == 0 || currentDate != previousDate)
                                {
                                    // Count consecutive rows with same date
                                    int spanCount = 1;
                                    for (int j = i + 1; j < dcTestCores.Count; j++)
                                    {
                                        string nextDate = dcDates[j % dcDates.Length].Trim();
                                        if (nextDate == currentDate)
                                            spanCount++;
                                        else
                                            break;
                                    }
                                    dateRowSpans.Add(spanCount);
                                    dateSpanCount = spanCount;
                                }
                                else
                                {
                                    dateRowSpans.Add(0);
                                }
                                previousDate = currentDate;
                            }

                            // Second pass: generate table rows
                            for (int i = 0; i < dcTestCores.Count; i++)
                            {
                                string currentDate = dcDates[i % dcDates.Length].Trim();

                                sbDetails.Append("<tr>");

                                // Add Date column only for first occurrence of each date
                                if (dateRowSpans[i] > 0)
                                {
                                    sbDetails.Append("<td rowspan='" + dateRowSpans[i] + "' style='border:1px solid;padding:5px;text-align:center;'>" + currentDate + "</td>");
                                }

                                sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + dcTestCores[i] + "</td>");
                                sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (i < dcTestLengths.Count ? dcTestLengths[i] : "") + "</td>");
                                sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (i < dcTestImmerseTimes.Count ? dcTestImmerseTimes[i] : "") + "</td>");
                                sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (i < dcTestTemperatures.Count ? dcTestTemperatures[i] : "") + "</td>");
                                sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (i < dcTestVoltages.Count ? dcTestVoltages[i] : "") + "</td>");
                                sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (i < dcTestTimes.Count ? dcTestTimes[i] : "") + "</td>");
                                sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (i < dcTestResults.Count ? dcTestResults[i] : "") + "</td>");

                                // Add Specified column with rowspan for first row only
                                if (i == 0)
                                {
                                    sbDetails.Append("<td rowspan='" + dcTestCores.Count + "' style='border:1px solid;padding:5px;text-align:center;'>" + (dcTestSpecified.Count > 0 ? dcTestSpecified[0] : "") + "</td>");
                                }

                                sbDetails.Append("</tr>");
                            }

                            sbDetails.Append(@"</tbody></table>");
                        }

                        // 3rd grid Test Results
                        if (irAmbientCores.Count > 0)
                        {
                            sbDetails.Append(@"
                            <table class='tab1' style='width:100%;border-collapse:collapse;margin-top:20px;'>
                                <thead>
                                <tr style='background-color:#bebebe;font-weight:bold;'>
                                    <th colspan='8' style='border:1px solid;padding:5px;'>3. Insulation Resistance at ambient temperature</th>
                                </tr>
                                <tr style='background-color:#bebebe;font-weight:bold;'>
                                    <th rowspan='2' style='border:1px solid;padding:5px;text-align:center;'>Date</th>
                                    <th rowspan='2' style='border:1px solid;padding:5px;text-align:center;'>Core</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>Length</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>Immerse</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>Temp.</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>Measurement</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>Insulation Resistance</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>Specified</th>
                                </tr>
                                <tr style='background-color:#bebebe;font-weight:bold;'>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>(m)</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>(hour)</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>(°C)</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>(MΩ)</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>(MΩ.km)</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>(MΩ.km)</th>
                                </tr>
                                </thead>
                                <tbody>");

                            // Split dates and process each group
                            string[] irAmbientDates = irAmbientDate.Count > 0 ? irAmbientDate[0].Split(',') : new string[0];

                            // Create a mapping of date to row indices
                            Dictionary<string, List<int>> dateRowMapping = new Dictionary<string, List<int>>();

                            for (int i = 0; i < irAmbientDates.Length && i < irAmbientCores.Count; i++)
                            {
                                string trimmedDate = irAmbientDates[i % irAmbientDates.Length].Trim();

                                if (!dateRowMapping.ContainsKey(trimmedDate))
                                {
                                    dateRowMapping[trimmedDate] = new List<int>();
                                }
                                dateRowMapping[trimmedDate].Add(i);
                            }

                            // Track which rows have been processed
                            HashSet<int> processedRows = new HashSet<int>();

                            // Process each unique date group
                            foreach (var dateGroup in dateRowMapping.OrderBy(x => x.Value.Min()))
                            {
                                string currentDate = dateGroup.Key;
                                List<int> rowIndices = dateGroup.Value;

                                for (int i = 0; i < rowIndices.Count; i++)
                                {
                                    int rowIndex = rowIndices[i];

                                    if (processedRows.Contains(rowIndex)) continue;

                                    sbDetails.Append("<tr>");

                                    // Add Date column with rowspan for first occurrence of this date
                                    if (i == 0)
                                    {
                                        sbDetails.Append("<td rowspan='" + rowIndices.Count + "' style='border:1px solid;padding:5px;text-align:center;'>" + currentDate + "</td>");
                                    }

                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + irAmbientCores[rowIndex] + "</td>");
                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (rowIndex < irAmbientLengths.Count ? irAmbientLengths[rowIndex] : "") + "</td>");
                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (rowIndex < irAmbientImmerseTimes.Count ? irAmbientImmerseTimes[rowIndex] : "") + "</td>");
                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (rowIndex < irAmbientTemperatures.Count ? irAmbientTemperatures[rowIndex] : "") + "</td>");
                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (rowIndex < irAmbientMeasurements.Count ? irAmbientMeasurements[rowIndex] : "") + "</td>");
                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (rowIndex < irAmbientInsulationResistance.Count ? irAmbientInsulationResistance[rowIndex] : "") + "</td>");

                                    // Add Specified column with rowspan for the first row only
                                    if (rowIndex == 0)
                                    {
                                        sbDetails.Append("<td rowspan='" + irAmbientCores.Count + "' style='border:1px solid;padding:5px;text-align:center;'>" + (irAmbientSpecified.Count > 0 ? irAmbientSpecified[0] : "") + "</td>");
                                    }

                                    sbDetails.Append("</tr>");
                                    processedRows.Add(rowIndex);
                                }
                            }

                            sbDetails.Append(@"</tbody></table>");
                        }

                        // 4th grid Test Results 
                        if (irMaxTempCores.Count > 0)
                        {
                            sbDetails.Append(@"
                            <table class='tab1' style='width:100%;border-collapse:collapse;margin-top:20px;'>
                                <thead>
                                <tr style='background-color:#bebebe;font-weight:bold;'>
                                    <th colspan='9' style='border:1px solid;padding:5px;'>4. Insulation Resistance at max. conductor temperature</th>
                                </tr>
                                <tr style='background-color:#bebebe;font-weight:bold;'>
                                    <th rowspan='2' style='border:1px solid;padding:5px;text-align:center;'>Date</th>
                                    <th rowspan='2' style='border:1px solid;padding:5px;text-align:center;'>Core</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>Length</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>Immerse</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>Temp.</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>Factor correction<br/>k<sub>i</sub></th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>Measurement</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>Insulation Resistance</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>Specified</th>
                                </tr>
                                <tr style='background-color:#bebebe;font-weight:bold;'>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>(m)</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>(hour)</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>(°C)</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>(-)</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>(MΩ)</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>(MΩ.km)</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>(MΩ.km)</th>
                                </tr>
                                </thead>
                                <tbody>");

                            // Split dates and process each group
                            string irMaxTempDateString = string.Join(",", irMaxTempDate);
                            string[] irMaxDates = irMaxTempDateString.Split(',');

                            // Create a mapping of date to row indices
                            Dictionary<string, List<int>> dateRowMapping4 = new Dictionary<string, List<int>>();

                            for (int i = 0; i < irMaxDates.Length && i < irMaxTempCores.Count; i++)
                            {
                                string trimmedDate = irMaxDates[i % irMaxDates.Length].Trim();

                                if (!dateRowMapping4.ContainsKey(trimmedDate))
                                {
                                    dateRowMapping4[trimmedDate] = new List<int>();
                                }
                                dateRowMapping4[trimmedDate].Add(i);
                            }

                            // Track which rows have been processed
                            HashSet<int> processedRows4 = new HashSet<int>();

                            // Process each unique date group
                            foreach (var dateGroup in dateRowMapping4.OrderBy(x => x.Value.Min()))
                            {
                                string currentDate = dateGroup.Key;
                                List<int> rowIndices = dateGroup.Value;

                                for (int i = 0; i < rowIndices.Count; i++)
                                {
                                    int rowIndex = rowIndices[i];

                                    if (processedRows4.Contains(rowIndex)) continue;

                                    sbDetails.Append("<tr>");

                                    // Add Date column with rowspan for first occurrence of this date
                                    if (i == 0)
                                    {
                                        sbDetails.Append("<td rowspan='" + rowIndices.Count + "' style='border:1px solid;padding:5px;text-align:center;'>" + currentDate + "</td>");
                                    }

                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + irMaxTempCores[rowIndex] + "</td>");
                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (rowIndex < irMaxTempLengths.Count ? irMaxTempLengths[rowIndex] : "") + "</td>");
                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (rowIndex < irMaxTempImmerseTimes.Count ? irMaxTempImmerseTimes[rowIndex] : "") + "</td>");
                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (rowIndex < irMaxTempTemperatures.Count ? irMaxTempTemperatures[rowIndex] : "") + "</td>");
                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (rowIndex < irMaxTempFactors.Count ? irMaxTempFactors[rowIndex] : "") + "</td>");
                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (rowIndex < irMaxTempMeasurements.Count ? irMaxTempMeasurements[rowIndex] : "") + "</td>");
                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (rowIndex < irMaxTempInsulationResistance.Count ? irMaxTempInsulationResistance[rowIndex] : "") + "</td>");

                                    // Add Specified column with rowspan for the first row only
                                    if (rowIndex == 0)
                                    {
                                        sbDetails.Append("<td rowspan='" + irMaxTempCores.Count + "' style='border:1px solid;padding:5px;text-align:center;'>" + (irMaxTempSpecified.Count > 0 ? irMaxTempSpecified[0] : "") + "</td>");
                                    }

                                    sbDetails.Append("</tr>");
                                    processedRows4.Add(rowIndex);
                                }
                            }

                            sbDetails.Append(@"</tbody></table>");
                        }

                        // 5th grid Test Results 
                        if (diameterCores.Count > 0)
                        {
                            sbDetails.Append(@"
                            <table class='tab1' style='width:100%;border-collapse:collapse;margin-top:20px;'>
                                <thead>
                                <tr style='background-color:#bebebe;font-weight:bold;'>
                                    <th colspan='9' style='border:1px solid;padding:5px;'>5. Inner & Outer diameter of insulation</th>
                                </tr>
                                <tr style='background-color:#bebebe;font-weight:bold;'>
                                    <th rowspan='3' style='border:1px solid;padding:5px;text-align:center;'>Date</th>
                                    <th rowspan='3' style='border:1px solid;padding:5px;text-align:center;'>Core</th>
                                    <th rowspan='3' style='border:1px solid;padding:5px;text-align:center;'>Length</th>
                                    <th colspan='3' style='border:1px solid;padding:5px;text-align:center;'>Inner diameter of insulation</th>
                                    <th colspan='3' style='border:1px solid;padding:5px;text-align:center;'>Outer diameter of insulation</th>
                                </tr>
                                <tr style='background-color:#bebebe;font-weight:bold;'>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>1</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>2</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>Avg</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>1</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>2</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>Avg</th>
                                </tr>
                                <tr style='background-color:#bebebe;font-weight:bold;'>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>(mm)</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>(mm)</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>(mm)</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>(mm)</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>(mm)</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>(mm)</th>
                                </tr>
                                </thead>
                                <tbody>");

                            // Split dates and process each group
                            string diameterDateString = string.Join(",", diameterDate);
                            string[] diamDates = diameterDateString.Split(',');

                            // Create a mapping of date to row indices
                            Dictionary<string, List<int>> dateRowMapping5 = new Dictionary<string, List<int>>();

                            for (int i = 0; i < diamDates.Length && i < diameterCores.Count; i++)
                            {
                                string trimmedDate = diamDates[i % diamDates.Length].Trim();

                                if (!dateRowMapping5.ContainsKey(trimmedDate))
                                {
                                    dateRowMapping5[trimmedDate] = new List<int>();
                                }
                                dateRowMapping5[trimmedDate].Add(i);
                            }

                            // Track which rows have been processed
                            HashSet<int> processedRows5 = new HashSet<int>();

                            // Process each unique date group
                            foreach (var dateGroup in dateRowMapping5.OrderBy(x => x.Value.Min()))
                            {
                                string currentDate = dateGroup.Key;
                                List<int> rowIndices = dateGroup.Value;

                                for (int i = 0; i < rowIndices.Count; i++)
                                {
                                    int rowIndex = rowIndices[i];

                                    if (processedRows5.Contains(rowIndex)) continue;

                                    sbDetails.Append("<tr>");

                                    // Add Date column with rowspan for first occurrence of this date
                                    if (i == 0)
                                    {
                                        sbDetails.Append("<td rowspan='" + rowIndices.Count + "' style='border:1px solid;padding:5px;text-align:center;'>" + currentDate + "</td>");
                                    }

                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + diameterCores[rowIndex] + "</td>");
                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (rowIndex < diameterLengths.Count ? diameterLengths[rowIndex] : "") + "</td>");
                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (rowIndex < diameterInner1.Count ? diameterInner1[rowIndex] : "") + "</td>");
                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (rowIndex < diameterInner2.Count ? diameterInner2[rowIndex] : "") + "</td>");
                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (rowIndex < diameterInnerAvg.Count ? diameterInnerAvg[rowIndex] : "") + "</td>");
                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (rowIndex < diameterOuter1.Count ? diameterOuter1[rowIndex] : "") + "</td>");
                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (rowIndex < diameterOuter2.Count ? diameterOuter2[rowIndex] : "") + "</td>");
                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (rowIndex < diameterOuterAvg.Count ? diameterOuterAvg[rowIndex] : "") + "</td>");

                                    sbDetails.Append("</tr>");
                                    processedRows5.Add(rowIndex);
                                }
                            }

                            // Add Specified row
                            sbDetails.Append(@"<tr style='font-weight:bold;'>
                            <td colspan='2' style='border:1px solid;padding:5px;text-align:center;background-color:#bebebe;'>Specified</td>");

                            if (diameterSpecified.Count >= 3)
                            {
                                sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + diameterSpecified[0] + "</td>");
                                sbDetails.Append("<td colspan='3' style='border:1px solid;padding:5px;'>" + diameterSpecified[1] + "</td>");
                                sbDetails.Append("<td colspan='3' style='border:1px solid;padding:5px;'>" + diameterSpecified[2] + "</td>");
                            }
                            else
                            {
                                sbDetails.Append("<td style='border:1px solid;padding:5px;'></td>");
                                sbDetails.Append("<td colspan='3' style='border:1px solid;padding:5px;'></td>");
                                sbDetails.Append("<td colspan='3' style='border:1px solid;padding:5px;'></td>");
                            }

                            sbDetails.Append("</tr>");
                            sbDetails.Append(@"</tbody></table>");
                        }

                        // 6th grid 
                        if (resistivityCores.Count > 0)
                        {
                            sbDetails.Append(@"
                            <table class='tab1' style='width:100%;border-collapse:collapse;margin-top:20px;'>
                                <thead>
                                <tr style='background-color:#bebebe;font-weight:bold;'>
                                    <th colspan='6' style='border:1px solid;padding:5px;'>6. Volume Resistivity, KI at ambient temperature</th>
                                </tr>
                                <tr style='background-color:#bebebe;font-weight:bold;'>
                                    <th rowspan='2' style='border:1px solid;padding:5px;text-align:center;'>Date</th>
                                    <th rowspan='2' style='border:1px solid;padding:5px;text-align:center;'>Core</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>Temp.</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>Insulation resistance</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>Volume resistivity at 20°C</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>Insulation resistant constant KI at 20°C</th>
                                </tr>
                                <tr style='background-color:#bebebe;font-weight:bold;'>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>(°C)</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>(MΩ.L)</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>(Ω.cm)</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>(MΩ.km)</th>
                                </tr>
                                </thead>
                                <tbody>");

                            // Split dates and process each group
                            string resistivityDateString = string.Join(",", resistivityDate);
                            string[] resDates = resistivityDateString.Split(',');

                            // Create a mapping of date to row indices
                            Dictionary<string, List<int>> dateRowMapping6 = new Dictionary<string, List<int>>();

                            for (int i = 0; i < resDates.Length && i < resistivityCores.Count; i++)
                            {
                                string trimmedDate = resDates[i % resDates.Length].Trim();

                                if (!dateRowMapping6.ContainsKey(trimmedDate))
                                {
                                    dateRowMapping6[trimmedDate] = new List<int>();
                                }
                                dateRowMapping6[trimmedDate].Add(i);
                            }

                            // Track which rows have been processed
                            HashSet<int> processedRows6 = new HashSet<int>();

                            // Process each unique date group
                            foreach (var dateGroup in dateRowMapping6.OrderBy(x => x.Value.Min()))
                            {
                                string currentDate = dateGroup.Key;
                                List<int> rowIndices = dateGroup.Value;

                                for (int i = 0; i < rowIndices.Count; i++)
                                {
                                    int rowIndex = rowIndices[i];

                                    if (processedRows6.Contains(rowIndex)) continue;

                                    sbDetails.Append("<tr>");

                                    // Add Date column with rowspan for first occurrence of this date
                                    if (i == 0)
                                    {
                                        sbDetails.Append("<td rowspan='" + rowIndices.Count + "' style='border:1px solid;padding:5px;text-align:center;'>" + currentDate + "</td>");
                                    }

                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + resistivityCores[rowIndex] + "</td>");
                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (rowIndex < resistivityTemperatures.Count ? resistivityTemperatures[rowIndex] : "") + "</td>");
                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (rowIndex < resistivityInsulationResistance.Count ? resistivityInsulationResistance[rowIndex] : "") + "</td>");
                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (rowIndex < resistivityVolumeResistivity.Count ? resistivityVolumeResistivity[rowIndex] : "") + "</td>");
                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (rowIndex < resistivityInsulationConstant.Count ? resistivityInsulationConstant[rowIndex] : "") + "</td>");

                                    sbDetails.Append("</tr>");
                                    processedRows6.Add(rowIndex);
                                }
                            }

                            // Add Specified row
                            sbDetails.Append(@"<tr style='font-weight:bold;'>
                            <td colspan='3' style='border:1px solid;padding:5px;text-align:center;background-color:#bebebe;'>Specified</td>");

                            if (resistivitySpecified.Count >= 4)
                            {
                                sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + resistivitySpecified[1] + "</td>");
                                sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + resistivitySpecified[2] + "</td>");
                                sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + resistivitySpecified[3] + "</td>");
                            }
                            else
                            {
                                sbDetails.Append("<td style='border:1px solid;padding:5px;'></td>");
                                sbDetails.Append("<td style='border:1px solid;padding:5px;'></td>");
                                sbDetails.Append("<td style='border:1px solid;padding:5px;'></td>");
                            }

                            sbDetails.Append("</tr>");
                            sbDetails.Append(@"</tbody></table>");
                        }

                        // 7th grid Test Results
                        if (resistivityMaxCores.Count > 0)
                        {
                            sbDetails.Append(@"
                            <table class='tab1' style='width:100%;border-collapse:collapse;margin-top:20px;'>
                                <thead>
                                <tr style='background-color:#bebebe;font-weight:bold;'>
                                    <th colspan='6' style='border:1px solid;padding:5px;'>7. Volume Resistivity, KI at max. conductor temperature</th>
                                </tr>
                                <tr style='background-color:#bebebe;font-weight:bold;'>
                                    <th rowspan='2' style='border:1px solid;padding:5px;text-align:center;'>Date</th>
                                    <th rowspan='2' style='border:1px solid;padding:5px;text-align:center;'>Core</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>Temp.</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>Insulation resistance</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>Volume resistivity at °C</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>Insulation resistant constant KI at °C</th>
                                </tr>
                                <tr style='background-color:#bebebe;font-weight:bold;'>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>(°C)</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>(MΩ.L)</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>(Ω.cm)</th>
                                    <th style='border:1px solid;padding:5px;text-align:center;'>(MΩ.km)</th>
                                </tr>
                                </thead>
                                <tbody>");

                            // Split dates and process each group
                            string resistivityMaxDateString = string.Join(",", resistivityMaxDate);
                            string[] resMaxDates = resistivityMaxDateString.Split(',');

                            // Create a mapping of date to row indices
                            Dictionary<string, List<int>> dateRowMapping7 = new Dictionary<string, List<int>>();

                            for (int i = 0; i < resMaxDates.Length && i < resistivityMaxCores.Count; i++)
                            {
                                string trimmedDate = resMaxDates[i % resMaxDates.Length].Trim();

                                if (!dateRowMapping7.ContainsKey(trimmedDate))
                                {
                                    dateRowMapping7[trimmedDate] = new List<int>();
                                }
                                dateRowMapping7[trimmedDate].Add(i);
                            }

                            // Track which rows have been processed
                            HashSet<int> processedRows7 = new HashSet<int>();

                            // Process each unique date group
                            foreach (var dateGroup in dateRowMapping7.OrderBy(x => x.Value.Min()))
                            {
                                string currentDate = dateGroup.Key;
                                List<int> rowIndices = dateGroup.Value;

                                for (int i = 0; i < rowIndices.Count; i++)
                                {
                                    int rowIndex = rowIndices[i];

                                    if (processedRows7.Contains(rowIndex)) continue;

                                    sbDetails.Append("<tr>");

                                    // Add Date column with rowspan for first occurrence of this date
                                    if (i == 0)
                                    {
                                        sbDetails.Append("<td rowspan='" + rowIndices.Count + "' style='border:1px solid;padding:5px;text-align:center;'>" + currentDate + "</td>");
                                    }

                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + resistivityMaxCores[rowIndex] + "</td>");
                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (rowIndex < resistivityMaxTemperatures.Count ? resistivityMaxTemperatures[rowIndex] : "") + "</td>");
                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (rowIndex < resistivityMaxInsulationResistance.Count ? resistivityMaxInsulationResistance[rowIndex] : "") + "</td>");
                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (rowIndex < resistivityMaxVolumeResistivity.Count ? resistivityMaxVolumeResistivity[rowIndex] : "") + "</td>");
                                    sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (rowIndex < resistivityMaxInsulationConstant.Count ? resistivityMaxInsulationConstant[rowIndex] : "") + "</td>");

                                    sbDetails.Append("</tr>");
                                    processedRows7.Add(rowIndex);
                                }
                            }

                            // Add Specified row
                            sbDetails.Append(@"<tr style='font-weight:bold;'>
                             <td colspan='3' style='border:1px solid;padding:5px;text-align:center;background-color:#bebebe;'>Specified</td>");

                            if (resistivityMaxSpecified.Count >= 4)
                            {
                                sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + resistivityMaxSpecified[1] + "</td>");
                                sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + resistivityMaxSpecified[2] + "</td>");
                                sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + resistivityMaxSpecified[3] + "</td>");
                            }
                            else
                            {
                                sbDetails.Append("<td style='border:1px solid;padding:5px;'></td>");
                                sbDetails.Append("<td style='border:1px solid;padding:5px;'></td>");
                                sbDetails.Append("<td style='border:1px solid;padding:5px;'></td>");
                            }

                            sbDetails.Append("</tr>");
                            sbDetails.Append(@"</tbody></table>");
                        }

                        sbFooter.Append(@"<table style='width:100%; font-family:Times New Roman; font-size:11px' class='tab1'>
                        <tr>
                            <td rowspan='2' style='vertical-align: middle;'><b>PT Qualis Indonesia</b></td>
                            <td colspan='3' style='text-align:center'><b>Witnessed By</b></td>
                        </tr>
                        <tr>
                           <td>Name</td><td>Company</td><td>Signature</td>
                        </tr>
                        <tr>
                            <td style='text-align:left'>Tested By:<br /><br /><br />Checked By:<br /><br /><br /></td>
                            <td></td><td></td><td></td>
                        </tr>
                    </table>");

                        dt6 = _cm.getTestRequestReview(InquiryID);
                        //Table start.
                        sb.Append("<table border='1' style='border-spacing:0px; width:100%'>");

                        //Adding HeaderRow.
                        sb.Append("<tr>");
                        sb.Append("<td rowspan='2' style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:10%'><b>Date of Review</b></td>");
                        sb.Append("<td colspan='2' style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:20%'><b>Review against</b></td>");
                        sb.Append("<td colspan='9' style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:70%'><b>Contract /Test Request Review Checking Points</b></td>");
                        sb.Append("</tr>");
                        sb.Append("<tr>");
                        sb.Append("<td style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:10%'><b>Sample <br/>Commodity<br/> & No. of<br/> Sample</b></td>");
                        sb.Append("<td style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:10%'><b>Customer<br/> Details</b></td>");
                        sb.Append("<td style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:10%'><b>Equipment<br/> Condition<br/> (OK or not)</b></td>");
                        sb.Append("<td style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:10%'><b>Manpower<br/> Availability<br/>(OK or not)</b></td>");
                        sb.Append("<td style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:10%'><b>Reagent/Std Availability(OK or not)</b></td>");
                        sb.Append("<td style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:10%'><b>Test<br/> Method<br/>/Specification <br/>(OK or not)</b></td>");
                        sb.Append("<td style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:5%'><b>*Sample Quantity <br/>Accepted<br/> (Minimum <br/>Quantity)</b></td>");
                        sb.Append("<td style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:10%'><b>*Physical<br/> Condition of <br/>Sample to be<br/> accepted</b></td>");
                        sb.Append("<td style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:5%'><b>*Temp <br/>Checking of<br/> sample (if<br/> Required)</b></td>");
                        sb.Append("<td style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:10%'><b>Delivery of<br/> Test report <br/>(approx.)</b></td>");
                        sb.Append("<td style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:10%'><b>Remarks /<br/> Signature</b></td>");
                        sb.Append("</tr>");

                        if (dt6 != null && dt6.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt6.Rows)
                            {
                                sb.Append("<tr>");
                                TestRequestReviewID = Convert.ToInt32(dr["TestRequestReviewID"].ToString());
                                sb.Append("<td style='border: 1px solid #2F4F4F;'>" + Convert.ToDateTime(dr["reviewDate"]).ToString("dd/MM/yyyy") + "</td>");
                                sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["sampleName"].ToString() + "," + dr["sampleQty"].ToString() + "</td>");
                                sb.Append("<td style='border: 1px solid #2F4F4F;'>" + sbCustomer.ToString() + "</td>");
                                sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["equipmentCondition"].ToString() + "</td>");
                                sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["manpower"].ToString() + "</td>");
                                sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["reagent"].ToString() + "</td>");
                                sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["testMethod"].ToString() + "</td>");
                                sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["sampleQtyAccepted"].ToString() + "</td>");
                                sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["samplePhyCondn"].ToString() + "</td>");
                                sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["sampleTemp"].ToString() + "</td>");
                                sb.Append("<td style='border: 1px solid #2F4F4F;'>" + Convert.ToDateTime(dr["deliveryDate"]).ToString("dd/MM/yyyy") + "</td>");
                                sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["remarks"].ToString() + "</td>");
                                sb.Append("</tr>");
                            }
                        }

                        //Table end.
                        sb.Append("</table>");
                        string DocumentNo = ""; string IssueNo = ""; string Revision = ""; string title = "";
                        DataTable dtDocument = _cm.getDocumentDetails("Test Request Review Checklist");
                        if (dtDocument.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtDocument.Rows)
                            {
                                DocumentNo = dr["DocumentNo"].ToString();
                                IssueNo = dr["IssueNo"].ToString();
                                Revision = dr["Revision"].ToString();
                                title = dr["Title"].ToString();
                            }
                        }

                        body = objEmail.PrepareMailBodyWith("TestRecord_VoltageTestandInsulationTest.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString(), "{IssueNo}", IssueNo, "{Revision}", Revision, "{Title}", title, "{Details}", sb.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord_VoltageTestandInsulationTest-" + SampleID + ".html");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_VoltageTestandInsulationTest-" + SampleID + ".html"));
                        divCRFReview.InnerHtml = html;
                        GC.Collect();
                    }
                }
            }

            else
            {
                Response.Redirect("Login.aspx");
            }
        }

    }
}