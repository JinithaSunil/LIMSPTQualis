using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LIMS.DAL;

namespace LIMS
{
    public partial class GenerateTestRecord_CreepTest : System.Web.UI.Page
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

                    if (Session["Usertype"] != null)
                    {
                        // Initialize dictionaries for each parameter
                        Dictionary<string, List<string>> DateofTest = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Duration = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Time = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> MeasurementEx1 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> MeasurementEx2 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> average = new Dictionary<string, List<string>>();
                      

                        // Basic report data
                        string Type = "";
                        string TestDate = "";
                        string RoomNo = "";
                        string RoomTemp = "";
                        string RefSpec = "";
                        string TestMethodNo = "";
                        string JobOrderNo = "";
                        string WorkSheetNo = "";
                        string RoomHumidity = "";
                        string EquipmentNo = "";
                        string Size = "";
                        string Voltage = "";
                        string Date = "";
                        string appliedLoad = "";
                        string gaugeLength = "";
                        string startTime = "";


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
                                string reportParamMapID = dr["ReportParamMapID"].ToString();
                                string result = dr["Result"].ToString();
                                string paramName = dr["ReportParameter"]?.ToString() ?? "";

                                if (reportParamMapID == "12989") // Date of test
                                {
                                    if (!string.IsNullOrEmpty(result))
                                    {
                                        string[] dates = result.Split(',');
                                        if (!DateofTest.ContainsKey(paramName))
                                            DateofTest[paramName] = new List<string>();
                                        DateofTest[paramName].AddRange(dates);
                                    }
                                }
                                else if (reportParamMapID == "12988") // Duration
                                {
                                    if (!string.IsNullOrEmpty(result))
                                    {
                                        string[] durations = result.Split(',');
                                        if (!Duration.ContainsKey(paramName))
                                            Duration[paramName] = new List<string>();
                                        Duration[paramName].AddRange(durations);
                                    }
                                }
                                else if (reportParamMapID == "12990") // Time
                                {
                                    if (!string.IsNullOrEmpty(result))
                                    {
                                        string[] times = result.Split(',');
                                        if (!Time.ContainsKey(paramName))
                                            Time[paramName] = new List<string>();
                                        Time[paramName].AddRange(times);
                                    }
                                }
                                else if (reportParamMapID == "12991") // Measurements (ext1|ext2 format)
                                {
                                    if (!string.IsNullOrEmpty(result))
                                    {
                                        string[] measurements = result.Split(',');
                                        if (!MeasurementEx1.ContainsKey(paramName))
                                            MeasurementEx1[paramName] = new List<string>();
                                        if (!MeasurementEx2.ContainsKey(paramName))
                                            MeasurementEx2[paramName] = new List<string>();

                                        foreach (string measurement in measurements)
                                        {
                                            string[] extValues = measurement.Split('|');
                                            MeasurementEx1[paramName].Add(extValues.Length > 0 ? extValues[0] : "");
                                            MeasurementEx2[paramName].Add(extValues.Length > 1 ? extValues[1] : "");
                                        }
                                    }
                                }
                                else if (reportParamMapID == "12992") // Average
                                {
                                    if (!string.IsNullOrEmpty(result))
                                    {
                                        string[] averages = result.Split(',');
                                        if (!average.ContainsKey(paramName))
                                            average[paramName] = new List<string>();
                                        average[paramName].AddRange(averages);
                                    }
                                }
                                else // Non-measurement data (simple key-value pairs)
                                {

                                    if (reportParamMapID == "12997") { Type = result; }
                                    else if (reportParamMapID == "12998") { Size = result; }
                                    else if (reportParamMapID == "12999") { Voltage = result; }
                                    else if (reportParamMapID == "13000") { Date = result; }
                                    else if (reportParamMapID == "13001") { RoomNo = result; }
                                    else if (reportParamMapID == "13002") { RoomTemp = result; }
                                    else if (reportParamMapID == "13003") { RefSpec = result; }
                                    else if (reportParamMapID == "13004") { TestMethodNo = result; }
                                    else if (reportParamMapID == "13005") { JobOrderNo = result; }
                                    else if (reportParamMapID == "13006") { WorkSheetNo = result; }
                                    else if (reportParamMapID == "13007") { RoomHumidity = result; }
                                    else if (reportParamMapID == "13008") { EquipmentNo = result; }
                                    else if (reportParamMapID == "12993") { appliedLoad = result; }
                                    else if (reportParamMapID == "12996") { gaugeLength = result; }
                                    else if (reportParamMapID == "13031") { startTime = result; }

                                }

                            }

                        }
                        sbHeader.Append(@"<table style='width: 100%; border: 1px solid; margin-top: 10px; ' class='tab1'>
                             <tr>
                                 <td>Type</td>
                                 <td>" + Type + "</td>" +
                             @" <td>Ref Spec.</td>
                                 <td>" + RefSpec + "</td>" +
                             @"</tr>
                             <tr>
                                 <td> Size</td>
                                 <td>" + Size + "</td>" +
                             @"<td>Test Method No.</td>
                                <td>" + TestMethodNo + "</td>" +
                             @" </tr>
                             <tr>
                                 <td>Voltage</td>
                                <td>" + Voltage + "</td>" +
                             @"<td>Job Order No.</td>
                                <td>" + JobOrderNo + "</td>" +
                             @"</tr>
                             <tr>
                                 <td>Date</td>
                               <td>" + Date + "</td>" +
                             @"<td>Work Sheet No.</td>
                                <td>" + WorkSheetNo + "</td>" +
                             @"</tr>
                             <tr>
                                 <td>Room No.</td>
                                <td>" + RoomNo + "</td>" +
                             @"<td>Room Humidity</td>
                                 <td>" + RoomHumidity + "%" + "</td>" +
                             @"</tr>
                             <tr>
                                 <td>Room Temp.</td>
                                 <td> " + RoomTemp + "°C" + "</td>" +
                             @" <td>Equipment No.</td>
                                <td>" + EquipmentNo + "</td>" +
                             @"</tr>
                         </table>"
                        );
                        sbDetails.Append($@"<p><strong>Test Start Time</strong>({startTime})</p>");

                        sbDetails.Append($@"<p><strong>Applied load (kN):</strong> ({appliedLoad})</p>");
                        sbDetails.Append($@"<p><strong>Gauge length (mm):</strong> {gaugeLength}</p>");



                        sbDetails.Append(@"<table class='tab1' border='1' cellpadding='3' cellspacing='0' style='width:100%; border-collapse: collapse;'>");


                        // First header row
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center;'><strong>Date</strong></td>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center;'><strong>Duration</strong></td>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center;'><strong>Time</strong></td>");
                        sbDetails.Append("<td colspan='2' style='background-color:#bebebe; text-align:center;'><strong>Measurement</strong></td>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center;'><strong>average</strong></td>");

                        sbDetails.Append("</tr>");

                        // Second header row
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'>Extension I</td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'>Extension II</td>");
                        sbDetails.Append("</tr>");

                   
                        int RowNumber = 1;

                        // Find maximum number of rows from all data
                        int maxRows = 0;
                        if (DateofTest.Count > 0) maxRows = Math.Max(maxRows, DateofTest.Values.First().Count);
                        if (Duration.Count > 0) maxRows = Math.Max(maxRows, Duration.Values.First().Count);
                        if (Time.Count > 0) maxRows = Math.Max(maxRows, Time.Values.First().Count);
                        if (MeasurementEx1.Count > 0) maxRows = Math.Max(maxRows, MeasurementEx1.Values.First().Count);
                        if (average.Count > 0) maxRows = Math.Max(maxRows, average.Values.First().Count);

                        // Display data rows
                        if (maxRows > 0)
                        {
                            for (int i = 0; i < maxRows; i++)
                            {
                                sbDetails.Append("<tr>");

                                // Date of Test
                                string dateValue = "";
                                if (DateofTest.Count > 0 && DateofTest.Values.First().Count > i)
                                {
                                    var rawDate = DateofTest.Values.First()[i];
                                    if (DateTime.TryParse(rawDate, out DateTime parsedDate))
                                    {
                                        dateValue = parsedDate.ToString("dd-MM-yy");
                                    }
                                    else
                                    {
                                        dateValue = rawDate; // fallback to raw string if parsing fails
                                    }
                                }
                                sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{dateValue}</td>");

                                // Duration
                                string durationValue = "";
                                if (Duration.Count > 0 && Duration.Values.First().Count > i)
                                    durationValue = Duration.Values.First()[i];
                                sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{durationValue}</td>");

                                // Time
                                string timeValue = "";
                                if (Time.Count > 0 && Time.Values.First().Count > i)
                                    timeValue = Time.Values.First()[i];
                                sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{timeValue}</td>");

                                // Extension I
                                string ext1Value = "";
                                if (MeasurementEx1.Count > 0 && MeasurementEx1.Values.First().Count > i)
                                    ext1Value = MeasurementEx1.Values.First()[i];
                                sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{ext1Value}</td>");

                                // Extension II
                                string ext2Value = "";
                                if (MeasurementEx2.Count > 0 && MeasurementEx2.Values.First().Count > i)
                                    ext2Value = MeasurementEx2.Values.First()[i];
                                sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{ext2Value}</td>");

                                // Average
                                string avgValue = "";
                                if (average.Count > 0 && average.Values.First().Count > i)
                                    avgValue = average.Values.First()[i];
                                sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{avgValue}</td>");

                                sbDetails.Append("</tr>");
                                RowNumber++;
                            }
                        }
                        else
                        {
                            // No data - add empty row
                            sbDetails.Append("<tr>");
                            sbDetails.Append("<td style='border:1px solid black;'></td>");
                            sbDetails.Append("<td style='border:1px solid black;'></td>");
                            sbDetails.Append("<td style='border:1px solid black;'></td>");
                            sbDetails.Append("<td style='border:1px solid black;'></td>");
                            sbDetails.Append("<td style='border:1px solid black;'></td>");
                            sbDetails.Append("<td style='border:1px solid black;'></td>");
                            sbDetails.Append("</tr>");
                        }
                        sbDetails.Append("</table>");
                        sbFooter.Append(@" <table style='width:100%; font-family:Times New Roman; font-size:11px' class='tab1'>
    <tr>
        <td style='font-weight: bold;vertical-align: middle;' rowspan='2' >PT Qualis Indonesia</td>
        <td colspan='3' style='text-align:center'><b>Witnessed By</b></td>
    </tr>
    <tr>
       <td>Name</td><td>Company</td><td>Signature</td>
    </tr>
    <tr>
        <td style='text-align:left'>Tested By:<br /><br /><br />Checked By:<br /><br /><br /></td>
        <td></td><td></td>
    </tr>
</table>");
                        // Generate the HTML file
                        body = objEmail.PrepareMailBodyWith("TestRecord_CreepTest.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord_CreepTest-" + SampleID + ".htm");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_CreepTest-" + SampleID + ".htm"));
                        divCRFReview.InnerHtml = html;
                        GC.Collect();

                    }
                }

                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }
    }
}