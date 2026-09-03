using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using LIMS.DAL;

namespace LIMS
{
    public partial class GenerateTestRecord_positiveimpulsetest : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int SampleID = 0; int ReportId = 0;
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
                if (Request.QueryString["phase"] != null)
                {
                    Session["Phase"] = Request.QueryString["phase"].ToString();
                }
                if (Request.QueryString["phaseIndex"] != null)
                {
                    Session["PhaseIndex"] = Request.QueryString["phaseIndex"].ToString();
                }

                if (Session["SampleID"] != null && Session["ReportId"] != null)
                {
                    SampleID = Convert.ToInt32(Session["SampleID"].ToString());
                    ReportId = Convert.ToInt32(Session["ReportId"].ToString());
                    string selectedPhase = Session["Phase"] != null ? Session["Phase"].ToString() : "Red"; // Default to Red phase if not specified
                    int phaseIndex = Session["PhaseIndex"] != null ? Convert.ToInt32(Session["PhaseIndex"].ToString()) : 0; // Default to first phase

                    if (Session["Usertype"] != null)
                    {
                        string Type = "";
                        List<string> WithstandVoltage_Positive = new List<string>();
                        List<string> PeakVoltage_Positive = new List<string>();
                        List<string> FrontWaveTime_Positive = new List<string>();
                        List<string> TailWaveTime_Positive = new List<string>();
                        List<string> ConductorTemp_Positive = new List<string>();
                        List<string> TestResult_Positive = new List<string>();
                        List<string> Remarks_Positive = new List<string>();

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
                        string Core = "";
                        string[] allPhases = null;

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

                        dtRecordDetails = _cm.getTestRecord(ReportId, SampleID);
                        if (dtRecordDetails.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtRecordDetails.Rows)
                            {
                                // Parse positive impulse test data with phase consideration
                                if (dr["ReportParamMapID"].ToString() == "7225")
                                {
                                    string[] phaseData = dr["Result"].ToString().Split('|');
                                    if (phaseData.Length > phaseIndex)
                                    {
                                        WithstandVoltage_Positive = new List<string>(phaseData[phaseIndex].Split(','));
                                    }
                                }
                                if (dr["ReportParamMapID"].ToString() == "7226")
                                {
                                    string[] phaseData = dr["Result"].ToString().Split('|');
                                    if (phaseData.Length > phaseIndex)
                                    {
                                        PeakVoltage_Positive = new List<string>(phaseData[phaseIndex].Split(','));
                                    }
                                }
                                if (dr["ReportParamMapID"].ToString() == "7227")
                                {
                                    string[] phaseData = dr["Result"].ToString().Split('|');
                                    if (phaseData.Length > phaseIndex)
                                    {
                                        FrontWaveTime_Positive = new List<string>(phaseData[phaseIndex].Split(','));
                                    }
                                }
                                if (dr["ReportParamMapID"].ToString() == "7228")
                                {
                                    string[] phaseData = dr["Result"].ToString().Split('|');
                                    if (phaseData.Length > phaseIndex)
                                    {
                                        TailWaveTime_Positive = new List<string>(phaseData[phaseIndex].Split(','));
                                    }
                                }
                                if (dr["ReportParamMapID"].ToString() == "7229")
                                {
                                    string[] phaseData = dr["Result"].ToString().Split('|');
                                    if (phaseData.Length > phaseIndex)
                                    {
                                        ConductorTemp_Positive = new List<string>(phaseData[phaseIndex].Split(','));
                                    }
                                }
                                if (dr["ReportParamMapID"].ToString() == "7230")
                                {
                                    string[] phaseData = dr["Result"].ToString().Split('|');
                                    if (phaseData.Length > phaseIndex)
                                    {
                                        TestResult_Positive = new List<string>(phaseData[phaseIndex].Split(','));
                                    }
                                }
                                if (dr["ReportParamMapID"].ToString() == "7231")
                                {
                                    string[] phaseData = dr["Result"].ToString().Split('|');
                                    if (phaseData.Length > phaseIndex)
                                    {
                                        string phaseRemarks = phaseData[phaseIndex];

                                        // Create a list to hold all remarks
                                        Remarks_Positive = new List<string>();

                                        // Split the string by commas
                                        string[] remarkEntries = phaseRemarks.Split(new[] { ',' }, StringSplitOptions.None);

                                        // Process each test (each test has 6 remarks with 9 commas between them)
                                        int testCount = remarkEntries.Length / 10; // Each test takes 10 entries (1 remark + 9 commas)

                                        for (int testNum = 0; testNum < testCount; testNum++)
                                        {
                                            int baseIndex = testNum * 10;

                                            // Add the 6 remarks for this test (each at position 0, 10, 20, etc. relative to test)
                                            for (int row = 0; row < 6; row++)
                                            {
                                                int remarkIndex = baseIndex + (row * 10);
                                                if (remarkIndex < remarkEntries.Length)
                                                {
                                                    Remarks_Positive.Add(remarkEntries[remarkIndex]);
                                                }
                                                else
                                                {
                                                    Remarks_Positive.Add("");
                                                }
                                            }
                                        }
                                    }
                                }


                                if (dr["ReportParamMapID"].ToString() == "7212") { Type = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "7215") { Size = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "7216") { Voltage = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "7217") { TestMethodNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "7218") { JobOrderNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "7220") { WorkSheetNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "7214") { RefSpec = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "7224") { EquipmentNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "7221") { RoomNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "7222") { RoomHumidity = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "7223") { RoomTemp = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "7219") { Date = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "7213")
                                {
                                    Core = dr["Result"].ToString();
                                    allPhases = Core.Split('|');
                                }
                            }

                            // Use selected phase if passed as parameter
                            Core = selectedPhase;

                            // HEADER TABLE
                            sbHeader.Append(@"<table style='width: 100%; border: 1px solid black; border-collapse: collapse;'>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px;'>Type</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + Type + @"</td>
                                <td style='border: 1px solid black; padding: 5px;'>Ref. Spec.</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + RefSpec + @"</td>
                            </tr>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px;'>Size</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + Size + @"</td>
                                <td style='border: 1px solid black; padding: 5px;'>Test Method No.</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + TestMethodNo + @"</td>
                            </tr>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px;'>Voltage</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + Voltage + @"</td>
                                <td style='border: 1px solid black; padding: 5px;'>Job Order No.</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + JobOrderNo + @"</td>
                            </tr>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px;'>Date</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + Date + @"</td>
                                <td style='border: 1px solid black; padding: 5px;'>Work Sheet No.</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + WorkSheetNo + @"</td>
                            </tr>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px;'>Room No.</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + RoomNo + @"</td>
                                <td style='border: 1px solid black; padding: 5px;'>Room Humidity</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + RoomHumidity + @" %</td>
                            </tr>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px;'>Room Temp.</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + RoomTemp + @" °C</td>
                                <td style='border: 1px solid black; padding: 5px;'>Equipment No.</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + EquipmentNo + @"</td>
                            </tr>
                                <tr>
                                <td style='border: 1px solid black; padding: 5px;'><b>Core Phase</b></td>
                                <td colspan='3' style='border: 1px solid black; padding: 5px;'><b>" + Core + @"</b></td>
                            </tr>
                        </table>");

                            // DETAILS TABLE
                            sbDetails.Append(@"<table style='width: 100%; border: 1px solid black; border-collapse: collapse; margin-top:10px;'>
                              <tr>
                                  <td colspan='6' style='border:1px solid black;background-color:#d3d3d3; padding:5px; text-align:center;'><b>10 Positive Impulse Test followed by a Voltage Test</b></td>
                                 
                              </tr>
                              <tr style='background-color:#d3d3d3;'>
                                  <td style='border:1px solid black; padding:5px; text-align:center;'>No</td>
                                  <td style='border:1px solid black; padding:5px; text-align:center;'>Item Test</td>
                                  <td style='border:1px solid black; padding:5px; text-align:center;'>Unit</td>
                                  <td style='border:1px solid black; padding:5px; text-align:center;'>Specified</td>
                                  <td style='border:1px solid black; padding:5px; text-align:center;'>Measured</td>
                                  <td style='border:1px solid black; padding:5px; text-align:center;'>Remark</td>
                              </tr>");

                            // Modify the loop that builds the details table to use the correct remark for each item
                            int maxItems = Math.Min(WithstandVoltage_Positive.Count, 10);
                            for (int i = 0; i < maxItems; i++)
                            {
                                string withstandValue = i < WithstandVoltage_Positive.Count ? WithstandVoltage_Positive[i] : "";
                                string peakValue = i < PeakVoltage_Positive.Count ? PeakVoltage_Positive[i] : "";
                                string frontWaveValue = i < FrontWaveTime_Positive.Count ? FrontWaveTime_Positive[i] : "";
                                string tailWaveValue = i < TailWaveTime_Positive.Count ? TailWaveTime_Positive[i] : "";
                                string conductorTempValue = i < ConductorTemp_Positive.Count ? ConductorTemp_Positive[i] : "";
                                string testResultValue = i < TestResult_Positive.Count ? TestResult_Positive[i] : "";

                                // Calculate the starting index for remarks for this test
                                int remarkStartIndex = i * 6;

                                // Get individual remarks for each row of this test
                                string remark1 = remarkStartIndex < Remarks_Positive.Count ? Remarks_Positive[remarkStartIndex] : "";
                                string remark2 = remarkStartIndex + 1 < Remarks_Positive.Count ? Remarks_Positive[remarkStartIndex + 1] : "";
                                string remark3 = remarkStartIndex + 2 < Remarks_Positive.Count ? Remarks_Positive[remarkStartIndex + 2] : "";
                                string remark4 = remarkStartIndex + 3 < Remarks_Positive.Count ? Remarks_Positive[remarkStartIndex + 3] : "";
                                string remark5 = remarkStartIndex + 4 < Remarks_Positive.Count ? Remarks_Positive[remarkStartIndex + 4] : "";
                                string remark6 = remarkStartIndex + 5 < Remarks_Positive.Count ? Remarks_Positive[remarkStartIndex + 5] : "";

                                sbDetails.Append(@"<tr>
                                  <td style='border:1px solid black; padding:5px; text-align:center;' rowspan='6'>" + (i + 1) + @"</td>
                                  <td style='border:1px solid black; padding:5px;'>Impulse test at Withstand Voltage</td>
                                  <td style='border:1px solid black; padding:5px; text-align:center;'>kV</td>
                                  <td style='border:1px solid black; padding:5px; text-align:center;'>60</td>
                                  <td style='border:1px solid black; padding:5px; text-align:center;'>" + withstandValue + @"</td>
                                  <td style='border:1px solid black; padding:5px; text-align:center;'>" + remark1 + @"</td>
                                </tr>
                                <tr>
                                  <td style='border:1px solid black; padding:5px;'>Peak Voltage</td>
                                  <td style='border:1px solid black; padding:5px; text-align:center;'>Up(kV)</td>
                                  <td style='border:1px solid black; padding:5px; text-align:center;'></td>
                                  <td style='border:1px solid black; padding:5px; text-align:center;'>" + peakValue + @"</td>
                                  <td style='border:1px solid black; padding:5px; text-align:center;'>" + remark2 + @"</td>
                                </tr>
                                <tr>
                                  <td style='border:1px solid black; padding:5px;'>Front wave time</td>
                                  <td style='border:1px solid black; padding:5px; text-align:center;'>µs</td>
                                  <td style='border:1px solid black; padding:5px; text-align:center;'>1 - 5</td>
                                  <td style='border:1px solid black; padding:5px; text-align:center;'>" + frontWaveValue + @"</td>
                                  <td style='border:1px solid black; padding:5px; text-align:center;'>" + remark3 + @"</td>
                                </tr>
                                <tr>
                                  <td style='border:1px solid black; padding:5px;'>Tail wave time</td>
                                  <td style='border:1px solid black; padding:5px; text-align:center;'>µs</td>
                                  <td style='border:1px solid black; padding:5px; text-align:center;'>40 - 60</td>
                                  <td style='border:1px solid black; padding:5px; text-align:center;'>" + tailWaveValue + @"</td>
                                  <td style='border:1px solid black; padding:5px; text-align:center;'>" + remark4 + @"</td>
                                </tr>
                                <tr>
                                  <td style='border:1px solid black; padding:5px;'>Conductor temperature</td>
                                  <td style='border:1px solid black; padding:5px; text-align:center;'>°C</td>
                                  <td style='border:1px solid black; padding:5px; text-align:center;'>95 - 100</td>
                                  <td style='border:1px solid black; padding:5px; text-align:center;'>" + conductorTempValue + @"</td>
                                  <td style='border:1px solid black; padding:5px; text-align:center;'>" + remark5 + @"</td>
                                </tr>
                                <tr>
                                  <td style='border:1px solid black; padding:5px;'>Test Result</td>
                                  <td style='border:1px solid black; padding:5px; text-align:center;'>-</td>
                                  <td style='border:1px solid black; padding:5px; text-align:center;'>No break down</td>
                                  <td style='border:1px solid black; padding:5px; text-align:center;'>" + testResultValue + @"</td>
                                  <td style='border:1px solid black; padding:5px; text-align:center;'>" + remark6 + @"</td>
                                </tr>");
                            }

                            sbDetails.Append("</table>");

                            // Replace sbFooter with a signature section like in the image
                            sbFooter.Append(@" <table style='width:100%; font-family:Times New Roman; font-size:11px' class='tab1'>
                                <tr>
                                    <td rowspan='2'  style=' vertical-align: middle' ><b>PT Qualis Indonesia</b></td>
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
                        }
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

                        body = objEmail.PrepareMailBodyWith("TestRecord_ImpulseTest.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString(), "{IssueNo}", IssueNo, "{Revision}", Revision, "{Title}", title, "{Details}", sb.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord_ImpulseTest-" + SampleID + "-" + Core + ".html");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_ImpulseTest-" + SampleID + "-" + Core + ".html"));
                        divCRFReview.InnerHtml = html;
                        GC.Collect();

                        // If this is the main page and we have more than one phase
                        if (Session["Phase"] == null && Session["PhaseIndex"] == null && allPhases != null && allPhases.Length > 1)
                        {
                            // Open windows for the other phases
                            StringBuilder scriptBuilder = new StringBuilder();
                            scriptBuilder.Append("<script type='text/javascript'>");

                            // Modify the scriptBuilder part to include delays
                            for (int i = 0; i < allPhases.Length; i++)
                            {
                                if (i > 0)
                                {
                                    string phaseUrl = "GenerateTestRecord_positiveimpulsetest.aspx?s=" + SampleID + "&r=" + ReportId + "&phase=" + allPhases[i] + "&phaseIndex=" + i;
                                    scriptBuilder.Append("setTimeout(function() { window.open('" + phaseUrl + "', '_blank'); }, " + (i * 100) + ");");
                                }
                            }

                            scriptBuilder.Append("</script>");
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "OpenPhaseWindows", scriptBuilder.ToString());
                        }
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