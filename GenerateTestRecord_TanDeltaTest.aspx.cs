using LIMS.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class GenerateTestRecord_TanDeltaTest : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        int InquiryID = 0; int CloneCRFSampleID = 0; int SampleID = 0; int ReportId = 0;

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

                    GenerateTestRecord(SampleID, ReportId);
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }

        private void GenerateTestRecord(int SampleID, int ReportId)
        {
            string Type = "", Size = "", Voltage = "", Date = "", RoomNo = "", RoomTemp = "";
            string RefSpec = "", TestMethodNo = "", JobOrderNo = "", WorkSheetNo = "", RoomHumidity = "", EquipmentNo = "";

            // Test 1 characteristics
            string TempTestObject1 = "", LengthTestObject1 = "", StandardCapacitor1 = "";

            // Test 2 characteristics
            string TempTestObject2 = "", LengthTestObject2 = "", StandardCapacitor2 = "";
            List<string> temperatureTypes = new List<string>();

            DataTable dtRecordDetails = new DataTable();
            StringBuilder sbHeader = new StringBuilder();
            StringBuilder sbTestDetails = new StringBuilder();
            StringBuilder sbFooter = new StringBuilder();

            mailer objEmail = new mailer();

            string body = ""; string str = ""; string filename = ""; string html = "";

            dtRecordDetails = _cm.getTestRecord(ReportId, SampleID);
            if (dtRecordDetails.Rows.Count > 0)
            {
                foreach (DataRow dr in dtRecordDetails.Rows)
                {
                    string reportParamMapID = dr["ReportParamMapID"].ToString();
                    string result = dr["Result"].ToString();

                    // Basic information
                    if (reportParamMapID == "17284") Type = result;
                    else if (reportParamMapID == "17285") Size = result;
                    else if (reportParamMapID == "17286") Voltage = result;
                    else if (reportParamMapID == "17287") Date = result;
                    else if (reportParamMapID == "17288") RoomNo = result;
                    else if (reportParamMapID == "17289") RoomTemp = result;
                    else if (reportParamMapID == "17290") RefSpec = result;
                    else if (reportParamMapID == "17291") TestMethodNo = result;
                    else if (reportParamMapID == "17292") JobOrderNo = result;
                    else if (reportParamMapID == "17293") WorkSheetNo = result;
                    else if (reportParamMapID == "17294") RoomHumidity = result;
                    else if (reportParamMapID == "17295") EquipmentNo = result;

                    // Test 1 characteristics
                    else if (reportParamMapID == "17298") TempTestObject1 = result;
                    else if (reportParamMapID == "17299") LengthTestObject1 = result;
                    else if (reportParamMapID == "17300") StandardCapacitor1 = result;

                    // Test 2 characteristics
                    else if (reportParamMapID == "17308") TempTestObject2 = result;
                    else if (reportParamMapID == "17309") LengthTestObject2 = result;
                    else if (reportParamMapID == "17310") StandardCapacitor2 = result;
                    else if (reportParamMapID == "17318")
                    {
                        // Store temperature types as list
                        if (!string.IsNullOrEmpty(result))
                            temperatureTypes.AddRange(result.Split(','));
                    }
                }

                // Build complete HTML with separate headers for each page
                StringBuilder sbCompleteHTML = new StringBuilder();

                // Page 1
                sbCompleteHTML.Append(BuildHeaderTable(Type, Size, Voltage, Date, RoomNo, RoomTemp, RefSpec, TestMethodNo, JobOrderNo, WorkSheetNo, RoomHumidity, EquipmentNo, 1, 2));
                sbCompleteHTML.Append(@"<div style='margin-top: 20px;'><h2>1. Tan δ measurement as a function of the voltage</h2>");
                sbCompleteHTML.Append(@"<table style='width: 100%; border: 1px solid; margin-bottom: 20px;' class='tab1'>
                                    <tr>
                                        <td colspan='4'><b>Characteristic of data</b></td>
                                    </tr>
                                    <tr>
                                        <td style='width: 30%;'><b>Temperature test object</b></td>
                                        <td style='width: 28%;'>" + TempTestObject1 + @"</td>
                                        <td style='width: 15%;'><b>°C</b></td>
                                        <td colspan='2' style='width: 30%'></td>
                                    </tr>
                                    <tr>
                                        <td style='width: 30%;'><b>Length of test object</b></td>
                                        <td style='width: 28%;'>" + LengthTestObject1 + @"</td>
                                        <td style='width: 15%;'><b>m</b></td>
                                        <td colspan='2' style='width: 30%'></td>
                                    </tr>
                                    <tr>
                                        <td style='width: 30%;'><b>Standard capacitor</b></td>
                                        <td style='width: 28%;'>" + StandardCapacitor1 + @"</td>
                                        <td style='width: 15%;'><b>pF</b></td>    
                                        <td colspan='2' style='width: 30%'></td>
                                    </tr>
                                </table>");

                // Test 1 Results Table (without temperature types)
                sbCompleteHTML.Append(BuildTestResultsTable(dtRecordDetails, "17301", "17302", "17303", "17304", "17305", "17306", "17307", false, null));

                // Add footer for page 1
                sbCompleteHTML.Append(BuildFooter());

                // Page break
                sbCompleteHTML.Append(@"<div style='page-break-before: always;'></div>");

                // Page 2
                sbCompleteHTML.Append(BuildHeaderTable(Type, Size, Voltage, Date, RoomNo, RoomTemp, RefSpec, TestMethodNo, JobOrderNo, WorkSheetNo, RoomHumidity, EquipmentNo, 2, 2));
                sbCompleteHTML.Append(@"<div style='margin-top: 20px;'><h2>2. Tan δ measurement as a function of the temperature</h2>");
                sbCompleteHTML.Append(@"<table style='width: 100%; border: 1px solid; margin-bottom: 20px;' class='tab1'>
                                    <tr>
                                        <td colspan='4'><b>Characteristic of data</b></td>
                                    </tr>
                                    <tr>
                                        <td style='width: 30%;'><b>Temperature test object</b></td>
                                        <td style='width: 28%;'>" + TempTestObject2 + @"</td>
                                        <td style='width: 15%;'><b>°C</b></td>
                                        <td colspan='2' style='width: 30%'></td>
                                    </tr>
                                    <tr>
                                        <td style='width: 30%'><b>Length of test object</b></td>
                                        <td style='width: 28%'>" + LengthTestObject2 + @"</td>
                                        <td style='width: 15%'><b>m</b></td>
                                        <td colspan='2' style='width: 30%'></td>
                                    </tr>
                                    <tr>
                                        <td style='width: 30%'><b>Standard capacitor</b></td>
                                        <td style='width: 28%'>" + StandardCapacitor2 + @"</td>
                                        <td style='width: 15%'><b>pF</b></td>
                                        <td colspan='2' style='width: 30%'></td>
                                    </tr>
                                </table>");

                // Test 2 Results Table (with temperature types as group headers)
                sbCompleteHTML.Append(BuildTestResultsTable(dtRecordDetails, "17311", "17312", "17313", "17314", "17315", "17316", "17317", true, temperatureTypes));

                // Add footer for page 2
                sbCompleteHTML.Append(BuildFooter());

                // Generate the HTML file with proper page structure
                body = objEmail.PrepareMailBodyWith("TestRecord_TanDeltaTest.html",
                    "{Header}", "", // Empty because we're building complete HTML structure
                    "{TestDetails}", sbCompleteHTML.ToString(),
                    "{Footer}", ""); // Empty because we're adding footers in the content

                // Add CSS for proper page layout
                body = body.Replace("<head>", @"<head>
                    <style>
                        @media print {
                            @page {
                                size: A4;
                                margin: 2cm;
                            }
                            .page-break {
                                page-break-before: always;
                                padding-top: 0px;
                            }
                            .footer-container {
                                margin-top: 50px;
                                page-break-inside: avoid;
                            }
                        }
                        @media screen {
                            .footer-container {
                                margin-top: 50px;
                            }
                        }
                    </style>");

                str = body.ToString();
                filename = Server.MapPath("//TestRecord/TestRecord_TanDeltaTest-" + SampleID + ".htm");
                System.IO.File.WriteAllText(@filename, str);
                html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_TanDeltaTest-" + SampleID + ".htm"));
                divCRFReview.InnerHtml = html;

                GC.Collect();
            }
        }

        private string BuildHeaderTable(string Type, string Size, string Voltage, string Date, string RoomNo, string RoomTemp,
                                       string RefSpec, string TestMethodNo, string JobOrderNo, string WorkSheetNo,
                                       string RoomHumidity, string EquipmentNo, int currentPage, int totalPages)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(@"<table style='width: 100%; border: 1px solid; border-collapse: collapse; margin-top: 10px;' class='tab1'>
                        <tr>
                            <!-- Left block: Revision, Effective date, Doc No, Page -->
                            <td class='tab1' style='width: 25%; font-size: 15px; padding: 5px;'>
                                <div style='display: flex; justify-content: space-between;'>
                                    <span><b>Revision</b><br />00</span>
                                    <span><b>Effective as of</b><br />03/04/2014</span>
                                </div>
                                <div style='display: flex; justify-content: space-between; margin-top: 10px;'>
                                    <span><b>Doc. No.</b><br />TSQ-LAB-ERD-043</span>
                                    <span><b>Page</b><br />" + currentPage + @" of " + totalPages + @"</span>
                                </div>
                            </td>

                            <!-- Center block: TEST RECORD title -->
                            <td class='tab1' style='text-align: center; background-color: #bebebe; font-size: 16px; font-weight: bold;'>
                                TEST RECORD<br /><br />
                                TAN δ TEST
                            </td>

                            <!-- Right block: Logo -->
                            <td class='tab1' style='width: 10%;' rowspan='1'>
                                <img src='../assets/img/logo.png' alt='PT Qualis' height='80' />
                            </td>
                        </tr>
                    </table>");

            // Add the data table
            sb.Append(@"<table style='width: 100%; border: 1px solid; margin-top: 10px; ' class='tab1'>
                        <tr>
                            <td><b>Type</b></td>
                            <td>" + Type + @"</td>
                            <td><b>Ref Spec.</b></td>
                            <td>" + RefSpec + @"</td>
                        </tr>
                        <tr>
                            <td><b>Size</b></td>
                            <td>" + Size + @"</td>
                            <td><b>Test Method No.</b></td>
                            <td>" + TestMethodNo + @"</td>
                        </tr>
                        <tr>
                            <td><b>Voltage</b></td>
                            <td>" + Voltage + @"</td>
                            <td><b>Job Order No.</b></td>
                            <td>" + JobOrderNo + @"</td>
                        </tr>
                        <tr>
                            <td><b>Date</b></td>
                            <td>" + Date + @"</td>
                            <td><b>Work Sheet No.</b></td>
                            <td>" + WorkSheetNo + @"</td>
                        </tr>
                        <tr>
                            <td><b>Room No.</b></td>
                            <td>" + RoomNo + @"</td>
                            <td><b>Room Humidity</b></td>
                            <td>" + RoomHumidity + @"%</td>
                        </tr>
                        <tr>
                            <td><b>Room Temp.</b></td>
                            <td>" + RoomTemp + @"°C</td>
                            <td><b>Equipment No.</b></td>
                            <td>" + EquipmentNo + @"</td>
                        </tr>
                    </table>");

            return sb.ToString();
        }

        private string BuildFooter()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(@"<div class='footer-container' style='margin-top: 50px;'>
                        <table style='width:100%; font-family:Times New Roman; font-size:11px;' class='tab1'>
                            <tr>
                                <td style='font-weight: bold;vertical-align: middle;' rowspan='2'>PT Qualis Indonesia</td>
                                <td colspan='3' style='text-align:center'><b>Witnessed By</b></td>
                            </tr>
                            <tr>
                                <td>Name</td><td>Company</td><td>Signature</td>
                            </tr>
                            <tr>
                                <td style='text-align:left'>Tested By:<br /><br /><br />Checked By:<br /><br /><br /></td>
                                <td></td><td></td><td></td>
                            </tr>
                        </table>
                    </div>");

            return sb.ToString();
        }

        private string BuildTestResultsTable(DataTable dt, string coresParam, string voltageParam, string capMeasuredParam,
                                            string capResultParam, string tanMeasuredParam, string tanResultParam, string tanStandardParam,
                                            bool hasTemperatureGroups, List<string> temperatureTypes = null)
        {
            StringBuilder sb = new StringBuilder();

            string coresCsv = GetResultValue(dt, coresParam);
            string voltageCsv = GetResultValue(dt, voltageParam);
            string capMeasuredCsv = GetResultValue(dt, capMeasuredParam);
            string capResultCsv = GetResultValue(dt, capResultParam);
            string tanMeasuredCsv = GetResultValue(dt, tanMeasuredParam);
            string tanResultCsv = GetResultValue(dt, tanResultParam);
            string tanStandardCsv = GetResultValue(dt, tanStandardParam);

            if (!string.IsNullOrEmpty(coresCsv))
            {
                string[] cores = coresCsv.Split(',');
                string[] voltages = voltageCsv.Split(',');
                string[] capMeasured = capMeasuredCsv.Split(',');
                string[] capResult = capResultCsv.Split(',');
                string[] tanMeasured = tanMeasuredCsv.Split(',');
                string[] tanResult = tanResultCsv.Split(',');
                string[] tanStandard = tanStandardCsv.Split(',');

                // Start building the table
                sb.Append(@"<table style='width:100%; border-collapse: collapse; border: 1px solid black;' class='results-table'>");

                // Header row 1
                sb.Append(@"<tr style='background-color: #bebebe;'>");
                sb.Append(@"<th style='border: 1px solid black; padding: 5px; text-align: center;'>Core</th>
                            <th style='border: 1px solid black; padding: 5px; text-align: center;'>Voltage test<br/>50 Hz</th>
                            <th style='border: 1px solid black; padding: 5px; text-align: center;' colspan='2'>Capacitance</th>
                            <th style='border: 1px solid black; padding: 5px; text-align: center;' colspan='3'>Tan δ</th>
                        </tr>");

                // Header row 2
                sb.Append(@"<tr style='background-color: #bebebe;'>");
                sb.Append(@"<th style='border: 1px solid black; padding: 5px; text-align: center;'></th>
                            <th style='border: 1px solid black; padding: 5px; text-align: center;'>kV</th>
                            <th style='border: 1px solid black; padding: 5px; text-align: center;'>Measured<br/>nF</th>
                            <th style='border: 1px solid black; padding: 5px; text-align: center;'>Result<br/>μF</th>
                            <th style='border: 1px solid black; padding: 5px; text-align: center;'>Measured</th>
                            <th style='border: 1px solid black; padding: 5px; text-align: center;'>Result <br/>(.......X 10-4)</th>
                            <th style='border: 1px solid black; padding: 5px; text-align: center;'>Standard <br/>(.......X 10-4)</th>
                        </tr>");

                if (hasTemperatureGroups && temperatureTypes != null && temperatureTypes.Count > 0)
                {
                    // Group data by temperature type
                    var groupedData = new Dictionary<string, List<TestDataRow>>();

                    for (int i = 0; i < cores.Length; i++)
                    {
                        string tempType = i < temperatureTypes.Count ? temperatureTypes[i] : "Unknown";

                        if (!groupedData.ContainsKey(tempType))
                            groupedData[tempType] = new List<TestDataRow>();

                        groupedData[tempType].Add(new TestDataRow
                        {
                            Core = i < cores.Length ? cores[i] : "",
                            Voltage = i < voltages.Length ? voltages[i] : "",
                            CapMeasured = i < capMeasured.Length ? capMeasured[i] : "",
                            CapResult = i < capResult.Length ? capResult[i] : "",
                            TanMeasured = i < tanMeasured.Length ? tanMeasured[i] : "",
                            TanResult = i < tanResult.Length ? tanResult[i] : "",
                            TanStandard = i < tanStandard.Length ? tanStandard[i] : ""
                        });
                    }

                    // Add grouped data with temperature type headers
                    foreach (var group in groupedData)
                    {
                        // Temperature type header row
                        sb.Append(@"<tr style='background-color: #e0e0e0;'>");
                        sb.Append(@"<td colspan='7' style='border: 1px solid black; padding: 5px; text-align: left; font-weight: bold;'>" + group.Key + @"</td>");
                        sb.Append(@"</tr>");

                        // Data rows for this temperature type
                        foreach (var row in group.Value)
                        {
                            sb.Append(@"<tr>");
                            sb.Append(@"<td style='border: 1px solid black; padding: 5px; text-align: center;'>" + row.Core + @"</td>");
                            sb.Append(@"<td style='border: 1px solid black; padding: 5px; text-align: center;'>" + row.Voltage + @"</td>");
                            sb.Append(@"<td style='border: 1px solid black; padding: 5px; text-align: center;'>" + row.CapMeasured + @"</td>");
                            sb.Append(@"<td style='border: 1px solid black; padding: 5px; text-align: center;'>" + row.CapResult + @"</td>");
                            sb.Append(@"<td style='border: 1px solid black; padding: 5px; text-align: center;'>" + row.TanMeasured + @"</td>");
                            sb.Append(@"<td style='border: 1px solid black; padding: 5px; text-align: center;'>" + row.TanResult + @"</td>");
                            sb.Append(@"<td style='border: 1px solid black; padding: 5px; text-align: center;'>" + row.TanStandard + @"</td>");
                            sb.Append(@"</tr>");
                        }
                    }
                }
                else
                {
                    // Regular data rows without temperature grouping
                    for (int i = 0; i < cores.Length; i++)
                    {
                        sb.Append(@"<tr>");
                        sb.Append(@"<td style='border: 1px solid black; padding: 5px; text-align: center;'>" + cores[i] + @"</td>");
                        sb.Append(@"<td style='border: 1px solid black; padding: 5px; text-align: center;'>" + (i < voltages.Length ? voltages[i] : "") + @"</td>");
                        sb.Append(@"<td style='border: 1px solid black; padding: 5px; text-align: center;'>" + (i < capMeasured.Length ? capMeasured[i] : "") + @"</td>");
                        sb.Append(@"<td style='border: 1px solid black; padding: 5px; text-align: center;'>" + (i < capResult.Length ? capResult[i] : "") + @"</td>");
                        sb.Append(@"<td style='border: 1px solid black; padding: 5px; text-align: center;'>" + (i < tanMeasured.Length ? tanMeasured[i] : "") + @"</td>");
                        sb.Append(@"<td style='border: 1px solid black; padding: 5px; text-align: center;'>" + (i < tanResult.Length ? tanResult[i] : "") + @"</td>");
                        sb.Append(@"<td style='border: 1px solid black; padding: 5px; text-align: center;'>" + (i < tanStandard.Length ? tanStandard[i] : "") + @"</td>");
                        sb.Append(@"</tr>");
                    }
                }

                sb.Append("</table>");
                sb.Append("<div style='margin-top:20px;'> </div>");
            }

            return sb.ToString();
        }

        private string GetResultValue(DataTable dt, string reportParamMapID)
        {
            DataRow[] rows = dt.Select($"ReportParamMapID = '{reportParamMapID}'");
            if (rows.Length > 0)
                return rows[0]["Result"].ToString();
            return "";
        }

        // Helper class to store test data rows
        private class TestDataRow
        {
            public string Core { get; set; }
            public string Voltage { get; set; }
            public string CapMeasured { get; set; }
            public string CapResult { get; set; }
            public string TanMeasured { get; set; }
            public string TanResult { get; set; }
            public string TanStandard { get; set; }
        }
    }
}