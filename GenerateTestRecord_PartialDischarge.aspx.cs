using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using LIMS.DAL;
using static iTextSharp.text.pdf.AcroFields;
using static System.Net.Mime.MediaTypeNames;
using DataTable = System.Data.DataTable;


namespace LIMS
{
    public partial class GenerateTestRecord_PartialDischarge : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int InquiryID = 0; int CloneCRFSampleID = 0; int SampleID = 0; int ReportId = 0;
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

                        // Define the data structures
                        List<string> AfterBendingTestCorePhases = new List<string>();
                        List<string> AfterHeatingCycleCorePhases = new List<string>();

                        Dictionary<string, List<string>> AfterBendingTestVoltage1 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> AfterBendingTestVoltage2 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> AfterBendingTestDuration = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> AfterBendingTestMaxPD = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> AfterHeatingCycleVoltage1 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> AfterHeatingCycleVoltage2 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> AfterHeatingCycleDuration = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> AfterHeatingCycleMaxPD = new Dictionary<string, List<string>>();

                        // Store dates for the tests
                        string AfterBendingTestDate = "";
                        string AfterHeatingCycleDate = "";

                        string StandardMaxPD = "";

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

                        string Circuit = "";
                        string Bandwidth = "";
                        string Calibration = "";
                        string Couplingcapactor = "";
                        string Noise = "";
                        string Testfrequency = "";

                        int InquiryID = 0; // Added missing variable definition

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

                        // Get test record data
                        dtRecordDetails = _cm.getTestRecord(ReportId, SampleID);
                        if (dtRecordDetails.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtRecordDetails.Rows)
                            {
                                if (dr["ReportParamMapID"].ToString() == "4136")
                                {
                                    AfterBendingTestCorePhases = new List<string>(dr["Result"].ToString().Split(','));
                                }

                                if (dr["ReportParamMapID"].ToString() == "4140")
                                {
                                    AfterHeatingCycleCorePhases = new List<string>(dr["Result"].ToString().Split(','));
                                }
                                if (dr["ReportParamMapID"].ToString() == "4137")
                                {
                                    string[] values = dr["Result"].ToString().Split(',');
                                    for (int i = 0; i < values.Length; i++)
                                    {
                                        string phase = "";
                                        if (i < AfterBendingTestCorePhases.Count)
                                        {
                                            phase = AfterBendingTestCorePhases[i];
                                            if (!AfterBendingTestVoltage1.ContainsKey(phase))
                                                AfterBendingTestVoltage1[phase] = new List<string>();
                                            AfterBendingTestVoltage1[phase].Add(values[i]);
                                        }
                                    }
                                }

                                if (dr["ReportParamMapID"].ToString() == "5146")
                                {
                                    string[] values = dr["Result"].ToString().Split(',');
                                    for (int i = 0; i < values.Length; i++)
                                    {
                                        string phase = "";
                                        if (i < AfterBendingTestCorePhases.Count)
                                        {
                                            phase = AfterBendingTestCorePhases[i];
                                            if (!AfterBendingTestVoltage2.ContainsKey(phase))
                                                AfterBendingTestVoltage2[phase] = new List<string>();
                                            AfterBendingTestVoltage2[phase].Add(values[i]);
                                        }
                                    }
                                }

                                if (dr["ReportParamMapID"].ToString() == "4138")
                                {
                                    string[] values = dr["Result"].ToString().Split(',');
                                    for (int i = 0; i < values.Length; i++)
                                    {
                                        string phase = "";
                                        if (i < AfterBendingTestCorePhases.Count)
                                        {
                                            phase = AfterBendingTestCorePhases[i];
                                            if (!AfterBendingTestDuration.ContainsKey(phase))
                                                AfterBendingTestDuration[phase] = new List<string>();
                                            AfterBendingTestDuration[phase].Add(values[i]);
                                        }
                                    }
                                }
                                if (dr["ReportParamMapID"].ToString() == "4139")
                                {
                                    string[] values = dr["Result"].ToString().Split(',');
                                    for (int i = 0; i < values.Length; i++)
                                    {
                                        string phase = "";
                                        if (i < AfterBendingTestCorePhases.Count)
                                        {
                                            phase = AfterBendingTestCorePhases[i];
                                            if (!AfterBendingTestMaxPD.ContainsKey(phase))
                                                AfterBendingTestMaxPD[phase] = new List<string>();
                                            AfterBendingTestMaxPD[phase].Add(values[i]);
                                        }
                                    }
                                }
                                if (dr["ReportParamMapID"].ToString() == "5156")
                                {
                                    string[] values = dr["Result"].ToString().Split(',');
                                    for (int i = 0; i < values.Length; i++)
                                    {
                                        string phase = "";
                                        if (i < AfterHeatingCycleCorePhases.Count)
                                        {
                                            phase = AfterHeatingCycleCorePhases[i];
                                            if (!AfterHeatingCycleVoltage1.ContainsKey(phase))
                                                AfterHeatingCycleVoltage1[phase] = new List<string>();
                                            AfterHeatingCycleVoltage1[phase].Add(values[i]);
                                        }
                                    }
                                }
                                if (dr["ReportParamMapID"].ToString() == "5147")
                                {
                                    string[] values = dr["Result"].ToString().Split(',');
                                    for (int i = 0; i < values.Length; i++)
                                    {
                                        string phase = "";
                                        if (i < AfterHeatingCycleCorePhases.Count)
                                        {
                                            phase = AfterHeatingCycleCorePhases[i];
                                            if (!AfterHeatingCycleVoltage2.ContainsKey(phase))
                                                AfterHeatingCycleVoltage2[phase] = new List<string>();
                                            AfterHeatingCycleVoltage2[phase].Add(values[i]);
                                        }
                                    }
                                }
                                if (dr["ReportParamMapID"].ToString() == "4141")
                                {
                                    string[] values = dr["Result"].ToString().Split(',');
                                    for (int i = 0; i < values.Length; i++)
                                    {
                                        string phase = "";
                                        if (i < AfterHeatingCycleCorePhases.Count)
                                        {
                                            phase = AfterHeatingCycleCorePhases[i];
                                            if (!AfterHeatingCycleDuration.ContainsKey(phase))
                                                AfterHeatingCycleDuration[phase] = new List<string>();
                                            AfterHeatingCycleDuration[phase].Add(values[i]);
                                        }
                                    }
                                }
                                if (dr["ReportParamMapID"].ToString() == "4142")
                                {
                                    string[] values = dr["Result"].ToString().Split(',');
                                    for (int i = 0; i < values.Length; i++)
                                    {
                                        string phase = "";
                                        if (i < AfterHeatingCycleCorePhases.Count)
                                        {
                                            phase = AfterHeatingCycleCorePhases[i];
                                            if (!AfterHeatingCycleMaxPD.ContainsKey(phase))
                                                AfterHeatingCycleMaxPD[phase] = new List<string>();
                                            AfterHeatingCycleMaxPD[phase].Add(values[i]);
                                        }
                                    }
                                }
                                if (dr["ReportParamMapID"].ToString() == "5148") { AfterBendingTestDate = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "5149") { AfterHeatingCycleDate = dr["Result"].ToString(); }

                                if (dr["ReportParamMapID"].ToString() == "4165") { Type = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4166") { Size = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4167") { Voltage = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4168") { Date = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4169") { RoomNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4170") { RoomTemp = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4171") { RefSpec = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4172") { TestMethodNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4173") { JobOrderNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4174") { WorkSheetNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4175") { RoomHumidity = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4176") { EquipmentNo = dr["Result"].ToString(); }

                                if (dr["ReportParamMapID"].ToString() == "5150") { Circuit = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "5151") { Bandwidth = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "5152") { Calibration = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "5154") { Noise = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "5153") { Couplingcapactor = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "5155") { Testfrequency = dr["Result"].ToString(); }

                                if (dr["ReportParamMapID"].ToString() == "4143") { StandardMaxPD = dr["Result"].ToString(); }

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

                        // Build characteristics table
                        sbDetails.Append(@"<table class='tab1'>
                        <tr>
                            <td colspan='6' style='background-color:#bebebe;font-weight: bold;'>Characteristic of data</td>
                        </tr>
                        <tr>
                            <td style='text-align:left;width:20%;'>Circuit</td>
                            <td style='width:30%;'>" + Circuit + @"</td>
                            <td style='text-align:left;'>Band width</td>
                            <td style='width:30%;'>" + Bandwidth + @" kHz</td>
                        </tr>
                        <tr>
                            <td style='text-align:left;width:20%;'>Calibration</td>
                            <td>" + Calibration + @" pc</td>
                            <td style='text-align:left;width:20%;'>Test frequency</td>
                            <td>" + Testfrequency + @" Hz</td>
                        </tr>
                        <tr>
                            <td style='text-align:left;width:20%;'>Noise</td>
                            <td>" + Noise + @" pc</td>
                            <td style='text-align:left;width:20%;'>Coupling capacitor</td>
                            <td>" + Couplingcapactor + @" pF</td>
                        </tr>
                    </table>");

                        // Main results table
                        sbDetails.Append(@"<table class='tab1'>
                        <tr>
                            <td rowspan='2' style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Date</td>
                            <td rowspan='2' style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Item Test</td>
                            <td rowspan='2' style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Core</td>
                            <td colspan='2' style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Voltage test 50 Hz</td>
                            <td rowspan='2' style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Duration</td>
                            <td rowspan='2' style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Maximum Partial discharge Level at U<sub>o</sub> / kV</td>
                        </tr>
                        <tr>
                            <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>...U<sub>o</sub></td>
                            <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>kV</td>
                        </tr>");

                        // Helper method to build the rows
                        void AppendTestData(
                            string testDate,
                            string testName,
                            List<string> corePhases,
                            Dictionary<string, List<string>> voltage1,
                            Dictionary<string, List<string>> voltage2,
                            Dictionary<string, List<string>> duration,
                            Dictionary<string, List<string>> maxPD)
                        {
                            if (corePhases.Count == 0) return;

                            var distinctPhases = corePhases.Distinct().ToList();
                            int totalRows = corePhases.Count;

                            bool firstRow = true;
                            int rowIndex = 0;

                            foreach (string phase in distinctPhases)
                            {
                                int phaseCount = corePhases.Count(p => p == phase);

                                for (int i = 0; i < phaseCount; i++)
                                {
                                    sbDetails.Append("<tr>");

                                    if (firstRow)
                                    {
                                        sbDetails.Append($"<td rowspan='{totalRows}'>" + testDate + "</td>");
                                        sbDetails.Append($"<td rowspan='{totalRows}'>" + testName + "</td>");
                                        firstRow = false;
                                    }

                                    if (i == 0)
                                    {
                                        sbDetails.Append($"<td rowspan='{phaseCount}'>" + phase + "</td>");
                                    }

                                    sbDetails.Append("<td>" + GetValueSafely(voltage1, phase, i) + "</td>");
                                    sbDetails.Append("<td>" + GetValueSafely(voltage2, phase, i) + "</td>");
                                    sbDetails.Append("<td>" + GetValueSafely(duration, phase, i) + "</td>");
                                    sbDetails.Append("<td>" + GetValueSafely(maxPD, phase, i) + "</td>");
                                    sbDetails.Append("</tr>");

                                    rowIndex++;
                                }
                            }
                        }

                        // Append bending test data
                        AppendTestData(
                            AfterBendingTestDate,
                            "Partial discharge after bending test 3 cycles diameter cylinder 120 cm",
                            AfterBendingTestCorePhases,
                            AfterBendingTestVoltage1,
                            AfterBendingTestVoltage2,
                            AfterBendingTestDuration,
                            AfterBendingTestMaxPD
                        );

                        // Append heating cycle data
                        AppendTestData(
                            AfterHeatingCycleDate,
                            "Partial discharge after heating cycles 20 times",
                            AfterHeatingCycleCorePhases,
                            AfterHeatingCycleVoltage1,
                            AfterHeatingCycleVoltage2,
                            AfterHeatingCycleDuration,
                            AfterHeatingCycleMaxPD
                        );



                        // Add Standard Row and close table
                        sbDetails.Append(@"
                        <tr>
                            <td colspan='6' style='text-align: Center;font-weight: Bold;'>Standard</td>
                            <td style='text-align: Center;font-weight: Bold;'>" + StandardMaxPD + @"</td>
                        </tr> 
                    </table>");

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

                        body = objEmail.PrepareMailBodyWith("TestRecord_Partialdischarge.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString(), "{IssueNo}", IssueNo, "{Revision}", Revision, "{Title}", title, "{Details}", sb.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord_Partialdischarge-" + SampleID + ".html");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_Partialdischarge-" + SampleID + ".html"));
                        divCRFReview.InnerHtml = html;
                        GC.Collect();
                        //Response.Redirect("~/SampleList.aspx");
                    }
                }
            }

            else
            {
                Response.Redirect("Login.aspx");
            }
        }


        // Helper method to safely get values
        private string GetValueSafely(Dictionary<string, List<string>> data, string key, int index)
        {
            if (data.ContainsKey(key) && index < data[key].Count)
                return data[key][index];
            return "";
        }
    }
}