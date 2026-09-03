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
    public partial class GenerateTestReport_LowVoltageCableLCR : System.Web.UI.Page
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
                    SampleID = Convert.ToInt32(Session["SampleID"].ToString());
                    ReportId = Convert.ToInt32(Session["ReportId"].ToString());

                    if (Session["Usertype"] != null)
                    {
                        string Type = "";
                        //List<string> Thickness = new List<string>();
                        //List<string> ThicknessAvg = new List<string>();

                        Dictionary<string, List<string>> Unit = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Specified = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> DateOfTest = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> DrumNo = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Length = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> CoreNo = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> TestResult = new Dictionary<string, List<string>>();



                        List<string> ExtractSpecimenValues(string resultString)
                        {
                            return resultString
                                .Split('|')
                                .Select(entry =>
                                {
                                    var parts = entry.Split(':');
                                    return parts.Length == 2 ? parts[1] : null;
                                })
                                .Where(val => val != null)
                                .ToList();
                        }

                        // Specified values
                        string SpecifiedThickness = "";
                        string SpecifiedDiameter = "";
                        string SpecifiedWeightload = "";
                        string Specified6hoursthickness = "";
                        string Specified6hoursDeformation = "";
                        string Specified6hoursMedian = "";
                        string AfterTemp = "";
                        string AfterTempLoad = "";



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
                                string reportParamMapID = dr["ReportParamMapID"].ToString();
                                string result = dr["Result"].ToString();

                                // 16068: Unit (comma-separated)
                                if (reportParamMapID == "16068")
                                {
                                    if (!Unit.ContainsKey("values"))
                                        Unit["values"] = new List<string>();

                                    var values = result.Split(',');
                                    foreach (var val in values)
                                        Unit["values"].Add(val.Trim());
                                }

                                // 16090: Specification (comma-separated)
                                else if (reportParamMapID == "16090")
                                {
                                    if (!Specified.ContainsKey("values"))
                                        Specified["values"] = new List<string>();

                                    var values = result.Split(',');
                                    foreach (var val in values)
                                        Specified["values"].Add(val.Trim());
                                }

                                // 16069: Date of Test (pipe-separated)
                                else if (reportParamMapID == "16069")
                                {
                                    if (!DateOfTest.ContainsKey("values"))
                                        DateOfTest["values"] = new List<string>();

                                    var dates = result.Split('|');
                                    foreach (var date in dates)
                                        DateOfTest["values"].Add(date.Trim());
                                }

                                // 16070: Drum No (pipe-separated)
                                else if (reportParamMapID == "16070")
                                {
                                    if (!DrumNo.ContainsKey("values"))
                                        DrumNo["values"] = new List<string>();

                                    var drums = result.Split('|');
                                    foreach (var drum in drums)
                                        DrumNo["values"].Add(drum.Trim());
                                }

                                // 16072: Length (pipe-separated)
                                else if (reportParamMapID == "16072")
                                {
                                    if (!Length.ContainsKey("values"))
                                        Length["values"] = new List<string>();

                                    var lengths = result.Split('|');
                                    foreach (var length in lengths)
                                        Length["values"].Add(length.Trim());
                                }

                                // 16071: Core (pipe-separated per specimen, comma-separated inside)
                                else if (reportParamMapID == "16071")
                                {
                                    if (!CoreNo.ContainsKey("values"))
                                        CoreNo["values"] = new List<string>();

                                    var specimens = result.Split('|');
                                    foreach (var specimen in specimens)
                                    {
                                        var cores = specimen.Split(',');
                                        CoreNo["values"].Add(string.Join(",", cores.Select(c => c.Trim())));
                                    }
                                }

                                // 16073: Test Result (pipe = specimen, comma = values inside)
                                else if (reportParamMapID == "16073")
                                {
                                    if (!TestResult.ContainsKey("values"))
                                        TestResult["values"] = new List<string>();

                                    var specimens = result.Split('|');
                                    foreach (var specimen in specimens)
                                    {
                                        var values = specimen.Split(',');
                                        TestResult["values"].Add(string.Join(",", values.Select(v => v.Trim())));
                                    }
                                }



                                if (dr["ReportParamMapID"].ToString() == "16074") { Type = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "16075") { Size = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "16076") { Voltage = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "16077") { Date = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "16078") { RoomNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "16079") { RoomTemp = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "16080") { RefSpec = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "16081") { TestMethodNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "16082") { JobOrderNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "16083") { WorkSheetNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "16084") { RoomHumidity = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "16085") { EquipmentNo = dr["Result"].ToString(); }


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
                         </table>");

                        // Get all the values from your dictionaries (following your existing pattern)
                        List<string> unitValues = Unit.ContainsKey("values") ? Unit["values"] : new List<string>();
                        List<string> specifiedValues = Specified.ContainsKey("values") ? Specified["values"] : new List<string>();
                        List<string> dateValues = DateOfTest.ContainsKey("values") ? DateOfTest["values"] : new List<string>();
                        List<string> drumValues = DrumNo.ContainsKey("values") ? DrumNo["values"] : new List<string>();
                        List<string> lengthValues = Length.ContainsKey("values") ? Length["values"] : new List<string>();
                        List<string> coreValues = CoreNo.ContainsKey("values") ? CoreNo["values"] : new List<string>();
                        List<string> testResultValues = TestResult.ContainsKey("values") ? TestResult["values"] : new List<string>();

                        sbDetails.Append(@"<table class='tab1' border='1' cellpadding='3' cellspacing='0' style='width:100%; border-collapse: collapse;'>
<tr>
    <td colspan='4' style='background-color:#bebebe; text-align: center; font-weight: bold;'>ITEM TEST</td>
    <td style='text-align: center; font-weight: bold; background-color: #bebebe;'>Inductance</td>
    <td style='text-align: center; font-weight: bold; background-color: #bebebe;'>Maximum inductance</td>
    <td style='text-align: center; font-weight: bold; background-color: #bebebe;'>Resistance</td>
    <td style='text-align: center; font-weight: bold; background-color: #bebebe;'>L/R Ratio</td>
    <td style='text-align: center; font-weight: bold; background-color: #bebebe;'>Mutual capacitance</td>
</tr>");

                        // Build the dynamic Unit row
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td colspan='4' style='text-align: center; background-color: #bebebe;'>Unit</td>");
                        for (int i = 0; i < 5; i++)
                        {
                            string unitText = i < unitValues.Count ? unitValues[i] : "";
                            sbDetails.Append($"<td style='text-align: center; background-color: #bebebe;'>{System.Net.WebUtility.HtmlEncode(unitText)}</td>");
                        }
                        sbDetails.Append("</tr>");

                        // Build the dynamic Specified row
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td colspan='4' style='text-align: center; background-color: #bebebe;'>Specified</td>");
                        for (int i = 0; i < 5; i++)
                        {
                            string specifiedText = i < specifiedValues.Count ? specifiedValues[i] : "";
                            sbDetails.Append($"<td style='text-align: center; background-color: #bebebe;'>{System.Net.WebUtility.HtmlEncode(specifiedText)}</td>");
                        }
                        sbDetails.Append("</tr>");

                        // Header row for data columns
                        sbDetails.Append(@"
<tr>
    <td style='text-align: center; background-color: #bebebe;'>Date</td>
    <td style='text-align: center; background-color: #bebebe;'>Drum No.</td>
    <td style='text-align: center; background-color: #bebebe;'>Length</td>
    <td style='text-align: center; background-color: #bebebe;'>Core No <br/>/Pair</td>
    <td colspan='5' style='text-align: center; background-color: #bebebe;'>Test Result</td>
</tr>");

                        // Build data rows with core splitting and rowspan
                        int maxSpecimens = Math.Max(Math.Max(Math.Max(dateValues.Count, drumValues.Count),
                                                             Math.Max(lengthValues.Count, testResultValues.Count)),
                                                    coreValues.Count);

                        for (int i = 0; i < maxSpecimens; i++)
                        {
                            // Get core values for this specimen (split by comma)
                            List<string> currentCores = new List<string>();
                            if (i < coreValues.Count && !string.IsNullOrEmpty(coreValues[i]))
                            {
                                currentCores = coreValues[i].Split(',').Select(c => c.Trim()).ToList();
                            }

                            // Get test result values for this specimen (split by comma)
                            List<string> currentTestResults = new List<string>();
                            if (i < testResultValues.Count && !string.IsNullOrEmpty(testResultValues[i]))
                            {
                                currentTestResults = testResultValues[i].Split(',').Select(t => t.Trim()).ToList();
                            }

                            int coreCount = Math.Max(currentCores.Count, 1); // At least 1 row even if no cores

                            // Build rows for each core
                            for (int coreIndex = 0; coreIndex < coreCount; coreIndex++)
                            {
                                sbDetails.Append("<tr>");

                                // Only add Date, Drum No, Length on first core row (with rowspan)
                                if (coreIndex == 0)
                                {
                                    // Date - format to dd-mm-yyyy
                                    string dateText = "";
                                    if (i < dateValues.Count && !string.IsNullOrEmpty(dateValues[i]))
                                    {
                                        if (DateTime.TryParse(dateValues[i], out DateTime parsedDate))
                                        {
                                            dateText = parsedDate.ToString("dd-MM-yyyy");
                                        }
                                        else
                                        {
                                            dateText = dateValues[i]; // Keep original if parsing fails
                                        }
                                    }
                                    if (coreCount > 1)
                                        sbDetails.Append($"<td rowspan='{coreCount}' style='text-align: center; vertical-align: middle;'>{System.Net.WebUtility.HtmlEncode(dateText)}</td>");
                                    else
                                        sbDetails.Append($"<td style='text-align: center;'>{System.Net.WebUtility.HtmlEncode(dateText)}</td>");

                                    // Drum No
                                    string drumText = i < drumValues.Count ? drumValues[i] : "";
                                    if (coreCount > 1)
                                        sbDetails.Append($"<td rowspan='{coreCount}' style='text-align: center; vertical-align: middle;'>{System.Net.WebUtility.HtmlEncode(drumText)}</td>");
                                    else
                                        sbDetails.Append($"<td style='text-align: center;'>{System.Net.WebUtility.HtmlEncode(drumText)}</td>");

                                    // Length
                                    string lengthText = i < lengthValues.Count ? lengthValues[i] : "";
                                    if (coreCount > 1)
                                        sbDetails.Append($"<td rowspan='{coreCount}' style='text-align: center; vertical-align: middle;'>{System.Net.WebUtility.HtmlEncode(lengthText)}</td>");
                                    else
                                        sbDetails.Append($"<td style='text-align: center;'>{System.Net.WebUtility.HtmlEncode(lengthText)}</td>");
                                }

                                // Core No (individual core for this row)
                                string coreText = coreIndex < currentCores.Count ? currentCores[coreIndex] : "";
                                sbDetails.Append($"<td style='text-align: center;'>{System.Net.WebUtility.HtmlEncode(coreText)}</td>");

                                // Test Result (5 columns - only add on first core row with rowspan)
                                if (coreIndex == 0)
                                {
                                    for (int j = 0; j < 5; j++)
                                    {
                                        string testResultText = j < currentTestResults.Count ? currentTestResults[j] : "";
                                        if (coreCount > 1)
                                            sbDetails.Append($"<td rowspan='{coreCount}' style='text-align: center; vertical-align: middle;'>{System.Net.WebUtility.HtmlEncode(testResultText)}</td>");
                                        else
                                            sbDetails.Append($"<td style='text-align: center;'>{System.Net.WebUtility.HtmlEncode(testResultText)}</td>");
                                    }
                                }

                                sbDetails.Append("</tr>");
                            }
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

                        for (int freqCount = 1; freqCount < dt6.Rows.Count + 1; freqCount++)
                        {
                            sb.Append("<tr>");
                            {
                                foreach (DataRow dr in dt6.Rows)
                                {
                                    TestRequestReviewID = Convert.ToInt32(dr["TestRequestReviewID"].ToString());
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + Convert.ToDateTime(dr["reviewDate"]).ToString("dd/MM/yyyy") + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["sampleName"].ToString() + "," + dr["sampleQty"].ToString() + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + sbCustomer.ToString() + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["equipmentCondition"].ToString() + "</ td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["manpower"].ToString() + "</ td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["reagent"].ToString() + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["testMethod"].ToString() + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["sampleQtyAccepted"].ToString() + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["samplePhyCondn"].ToString() + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["sampleTemp"].ToString() + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + Convert.ToDateTime(dr["deliveryDate"]).ToString("dd/MM/yyyy") + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["remarks"].ToString() + "</td>");
                                }

                            }
                            sb.Append("</tr>");
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

                        body = objEmail.PrepareMailBodyWith("TestRecord_LowVoltageCableLCR.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString(), "{IssueNo}", IssueNo, "{Revision}", Revision, "{Title}", title, "{Details}", sb.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord_LowVoltageCableLCR-" + SampleID + ".htm");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_LowVoltageCableLCR-" + SampleID + ".htm"));
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
        // Add this helper method to get values safely
        private string GetValueSafely(Dictionary<string, List<string>> data, string key, int index)
        {
            if (data.ContainsKey(key) && index < data[key].Count)
                return data[key][index];
            return "";
        }
    }
}