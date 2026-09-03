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
    public partial class GenerateTestRecord_ResistivityOfSemiConductingScreen_Conductor : System.Web.UI.Page
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
                        List<string> ConductorscreenBeforeAgeingCore = new List<string>();
                        List<string> ConductorscreenAfterAgeingCore = new List<string>();
                        Dictionary<string, List<string>> ConductorscreenBeforeAgeingDiameter = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> ConductorscreenBeforeAgeingThickness = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> ConductorscreenBeforeAgeingVolumeResistivity = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> ConductorscreenAfterAgeingDiameter = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> ConductorscreenAfterAgeingThickness = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> ConductorscreenAfterAgeingVolumeResistivity = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> StandardbeforeafterageingVolumeResistivity__Conductor = new Dictionary<string, List<string>>();
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
                            // First pass: Get the core lists first before processing other parameters
                            foreach (DataRow dr in dtRecordDetails.Rows)
                            {
                                if (dr["ReportParamMapID"].ToString() == "5209")
                                {
                                    ConductorscreenBeforeAgeingCore = new List<string>(dr["Result"].ToString().Split(','));
                                }
                                else if (dr["ReportParamMapID"].ToString() == "5210")
                                {
                                    ConductorscreenAfterAgeingCore = new List<string>(dr["Result"].ToString().Split(','));
                                }
                            }

                            // Second pass: Process the remaining parameters after core lists are established
                            foreach (DataRow dr in dtRecordDetails.Rows)
                            {
                                // Process diameter, thickness, and volume resistivity data
                                if (dr["ReportParamMapID"].ToString() == "5171") // Before Ageing Diameter
                                {
                                    string[] values = dr["Result"].ToString().Split(',');
                                    for (int i = 0; i < ConductorscreenBeforeAgeingCore.Count && i < values.Length; i++)
                                    {
                                        string core = ConductorscreenBeforeAgeingCore[i].Trim();
                                        if (!ConductorscreenBeforeAgeingDiameter.ContainsKey(core))
                                            ConductorscreenBeforeAgeingDiameter[core] = new List<string>();

                                        ConductorscreenBeforeAgeingDiameter[core].Add(values[i].Trim());
                                    }
                                }
                                else if (dr["ReportParamMapID"].ToString() == "5172") // Before Ageing Thickness
                                {
                                    string[] values = dr["Result"].ToString().Split(',');
                                    for (int i = 0; i < ConductorscreenBeforeAgeingCore.Count && i < values.Length; i++)
                                    {
                                        string core = ConductorscreenBeforeAgeingCore[i].Trim();
                                        if (!ConductorscreenBeforeAgeingThickness.ContainsKey(core))
                                            ConductorscreenBeforeAgeingThickness[core] = new List<string>();

                                        ConductorscreenBeforeAgeingThickness[core].Add(values[i].Trim());
                                    }
                                }
                                else if (dr["ReportParamMapID"].ToString() == "5173") // Before Ageing Volume Resistivity
                                {
                                    string[] values = dr["Result"].ToString().Split(',');
                                    for (int i = 0; i < ConductorscreenBeforeAgeingCore.Count && i < values.Length; i++)
                                    {
                                        string core = ConductorscreenBeforeAgeingCore[i].Trim();
                                        if (!ConductorscreenBeforeAgeingVolumeResistivity.ContainsKey(core))
                                            ConductorscreenBeforeAgeingVolumeResistivity[core] = new List<string>();

                                        ConductorscreenBeforeAgeingVolumeResistivity[core].Add(values[i].Trim());
                                    }
                                }
                                else if (dr["ReportParamMapID"].ToString() == "5174") // After Ageing Diameter
                                {
                                    string[] values = dr["Result"].ToString().Split(',');
                                    for (int i = 0; i < ConductorscreenAfterAgeingCore.Count && i < values.Length; i++)
                                    {
                                        string core = ConductorscreenAfterAgeingCore[i].Trim();
                                        if (!ConductorscreenAfterAgeingDiameter.ContainsKey(core))
                                            ConductorscreenAfterAgeingDiameter[core] = new List<string>();

                                        ConductorscreenAfterAgeingDiameter[core].Add(values[i].Trim());
                                    }
                                }
                                else if (dr["ReportParamMapID"].ToString() == "5175") // After Ageing Thickness
                                {
                                    string[] values = dr["Result"].ToString().Split(',');
                                    for (int i = 0; i < ConductorscreenAfterAgeingCore.Count && i < values.Length; i++)
                                    {
                                        string core = ConductorscreenAfterAgeingCore[i].Trim();
                                        if (!ConductorscreenAfterAgeingThickness.ContainsKey(core))
                                            ConductorscreenAfterAgeingThickness[core] = new List<string>();

                                        ConductorscreenAfterAgeingThickness[core].Add(values[i].Trim());
                                    }
                                }
                                else if (dr["ReportParamMapID"].ToString() == "5176") // After Ageing Volume Resistivity
                                {
                                    string[] values = dr["Result"].ToString().Split(',');
                                    for (int i = 0; i < ConductorscreenAfterAgeingCore.Count && i < values.Length; i++)
                                    {
                                        string core = ConductorscreenAfterAgeingCore[i].Trim();
                                        if (!ConductorscreenAfterAgeingVolumeResistivity.ContainsKey(core))
                                            ConductorscreenAfterAgeingVolumeResistivity[core] = new List<string>();

                                        ConductorscreenAfterAgeingVolumeResistivity[core].Add(values[i].Trim());
                                    }
                                }
                                else if (dr["ReportParamMapID"].ToString() == "5211") // Standard before & after ageing Volume Resistivity
                                {
                                    // Store the standard value
                                    string standardValue = dr["Result"].ToString().Trim();
                                    if (!StandardbeforeafterageingVolumeResistivity__Conductor.ContainsKey("Standard"))
                                        StandardbeforeafterageingVolumeResistivity__Conductor["Standard"] = new List<string>();

                                    StandardbeforeafterageingVolumeResistivity__Conductor["Standard"].Add(standardValue);
                                }

                                // Process other general parameters
                                if (dr["ReportParamMapID"].ToString() == "6217") { Type = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "6218") { Size = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "6219") { Voltage = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "6220") { Date = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "6221") { RoomNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "6222") { RoomTemp = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "6223") { RefSpec = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "6224") { TestMethodNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "6225") { JobOrderNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "6226") { WorkSheetNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "6227") { RoomHumidity = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "6228") { EquipmentNo = dr["Result"].ToString(); }
                            }

                            //// Debugging - Log the content of dictionaries to verify data is mapped correctly
                            //System.Diagnostics.Debug.WriteLine("--- Before Ageing Cores ---");
                            //foreach (var core in ConductorscreenBeforeAgeingCore)
                            //{
                            //    System.Diagnostics.Debug.WriteLine($"Core: {core}");
                            //    System.Diagnostics.Debug.WriteLine($"  Diameter: {(ConductorscreenBeforeAgeingDiameter.ContainsKey(core) ? string.Join(",", ConductorscreenBeforeAgeingDiameter[core]) : "Not found")}");
                            //    System.Diagnostics.Debug.WriteLine($"  Thickness: {(ConductorscreenBeforeAgeingThickness.ContainsKey(core) ? string.Join(",", ConductorscreenBeforeAgeingThickness[core]) : "Not found")}");
                            //    System.Diagnostics.Debug.WriteLine($"  Volume Resistivity: {(ConductorscreenBeforeAgeingVolumeResistivity.ContainsKey(core) ? string.Join(",", ConductorscreenBeforeAgeingVolumeResistivity[core]) : "Not found")}");
                            //}

                            //System.Diagnostics.Debug.WriteLine("--- After Ageing Cores ---");
                            //foreach (var core in ConductorscreenAfterAgeingCore)
                            //{
                            //    System.Diagnostics.Debug.WriteLine($"Core: {core}");
                            //    System.Diagnostics.Debug.WriteLine($"  Diameter: {(ConductorscreenAfterAgeingDiameter.ContainsKey(core) ? string.Join(",", ConductorscreenAfterAgeingDiameter[core]) : "Not found")}");
                            //    System.Diagnostics.Debug.WriteLine($"  Thickness: {(ConductorscreenAfterAgeingThickness.ContainsKey(core) ? string.Join(",", ConductorscreenAfterAgeingThickness[core]) : "Not found")}");
                            //    System.Diagnostics.Debug.WriteLine($"  Volume Resistivity: {(ConductorscreenAfterAgeingVolumeResistivity.ContainsKey(core) ? string.Join(",", ConductorscreenAfterAgeingVolumeResistivity[core]) : "Not found")}");
                            //}

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
                        }

                        int totalRows = ConductorscreenBeforeAgeingCore.Count + ConductorscreenAfterAgeingCore.Count;

                        sbDetails.Append($@"<table class='tab1' border='1' style='border-collapse: collapse; width: 100%; text-align: center;'>
<tr>
<td colspan='2' style='background-color:#bebebe; font-weight: bold; vertical-align: middle;'>Test Item</td>
<td style='background-color:#bebebe; font-weight: bold; vertical-align: middle;'>Core</td>
<td style='background-color:#bebebe; font-weight: bold; vertical-align: middle;'>Diameter (Avg) mm</td>
<td style='background-color:#bebebe; font-weight: bold; vertical-align: middle;'>Thickness (Avg) mm</td>
<td style='background-color:#bebebe; font-weight: bold; vertical-align: middle;'>Volume Resistivity (Ω·m)</td>
</tr>");

                        bool testItemAdded = false;

                        // Add Before Ageing Section
                        if (ConductorscreenBeforeAgeingCore.Count > 0)
                        {
                            // Group cores for rowspan calculation
                            Dictionary<string, int> coreCountBefore = new Dictionary<string, int>();
                            foreach (string core in ConductorscreenBeforeAgeingCore)
                            {
                                string coreKey = core.Trim();
                                if (!coreCountBefore.ContainsKey(coreKey))
                                    coreCountBefore[coreKey] = 0;
                                coreCountBefore[coreKey]++;
                            }

                            string lastCore = "";

                            for (int i = 0; i < ConductorscreenBeforeAgeingCore.Count; i++)
                            {
                                string core = ConductorscreenBeforeAgeingCore[i].Trim();
                                sbDetails.Append("<tr>");

                                if (!testItemAdded)
                                {
                                    sbDetails.Append($"<td rowspan='{totalRows}' style='vertical-align: middle; text-align: center;'>Conductor screen</td>");
                                    testItemAdded = true;
                                }

                                if (i == 0)
                                    sbDetails.Append("<td rowspan='" + ConductorscreenBeforeAgeingCore.Count + "'>Before Ageing</td>");

                                int occurrenceIndex = GetOccurrenceIndex(ConductorscreenBeforeAgeingCore, i);

                                // Only add the core cell if it's a new core or the first item
                                if (core != lastCore || i == 0)
                                {
                                    sbDetails.Append("<td rowspan='" + coreCountBefore[core] + "'>" + core + "</td>");
                                    lastCore = core;
                                }

                                sbDetails.Append("<td>" + GetValueSafely(ConductorscreenBeforeAgeingDiameter, core, occurrenceIndex) + "</td>");
                                sbDetails.Append("<td>" + GetValueSafely(ConductorscreenBeforeAgeingThickness, core, occurrenceIndex) + "</td>");
                                sbDetails.Append("<td>" + GetValueSafely(ConductorscreenBeforeAgeingVolumeResistivity, core, occurrenceIndex) + "</td>");

                                sbDetails.Append("</tr>");
                            }
                        }

                        // Add After Ageing Section
                        if (ConductorscreenAfterAgeingCore.Count > 0)
                        {
                            // Group cores for rowspan calculation
                            Dictionary<string, int> coreCountAfter = new Dictionary<string, int>();
                            foreach (string core in ConductorscreenAfterAgeingCore)
                            {
                                string coreKey = core.Trim();
                                if (!coreCountAfter.ContainsKey(coreKey))
                                    coreCountAfter[coreKey] = 0;
                                coreCountAfter[coreKey]++;
                            }

                            string lastCore = "";

                            for (int i = 0; i < ConductorscreenAfterAgeingCore.Count; i++)
                            {
                                string core = ConductorscreenAfterAgeingCore[i].Trim();
                                sbDetails.Append("<tr>");

                                if (!testItemAdded)
                                {
                                    sbDetails.Append($"<td rowspan='{totalRows}' style='vertical-align: middle; text-align: center;'>Conductor screen</td>");
                                    testItemAdded = true;
                                }

                                if (i == 0)
                                    sbDetails.Append("<td rowspan='" + ConductorscreenAfterAgeingCore.Count + "'>After Ageing</td>");

                                int occurrenceIndex = GetOccurrenceIndex(ConductorscreenAfterAgeingCore, i);

                                // Only add the core cell if it's a new core or the first item
                                if (core != lastCore || i == 0)
                                {
                                    sbDetails.Append("<td rowspan='" + coreCountAfter[core] + "'>" + core + "</td>");
                                    lastCore = core;
                                }

                                sbDetails.Append("<td>" + GetValueSafely(ConductorscreenAfterAgeingDiameter, core, occurrenceIndex) + "</td>");
                                sbDetails.Append("<td>" + GetValueSafely(ConductorscreenAfterAgeingThickness, core, occurrenceIndex) + "</td>");
                                sbDetails.Append("<td>" + GetValueSafely(ConductorscreenAfterAgeingVolumeResistivity, core, occurrenceIndex) + "</td>");

                                sbDetails.Append("</tr>");
                            }
                        }

                        // Get standard value - if exists
                        string stdValue = "0"; // Default value
                        if (StandardbeforeafterageingVolumeResistivity__Conductor.ContainsKey("Standard") &&
                            StandardbeforeafterageingVolumeResistivity__Conductor["Standard"].Count > 0)
                        {
                            stdValue = StandardbeforeafterageingVolumeResistivity__Conductor["Standard"][0];
                        }

                        // Add standard row
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td colspan='5' style='text-align: center; font-weight:bold;'>Standard before & after ageing</td>");
                        sbDetails.Append("<td style='text-align: center;'><span style='font-weight:bold;'>Max:</span> " + stdValue + "</td>");
                        sbDetails.Append("</tr>");

                        sbDetails.Append("</table>");

                        sbDetails.Append(@" <table style='width:100%; font-family:Times New Roman; font-size:11px' class='tab1'>
<tr>
<td style='font-weight: bold;vertical-align: middle;' rowspan='2' >PT Qualis Indonesia</td>
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

                        body = objEmail.PrepareMailBodyWith("TestRecord_ResistivityOfSemiConductingScreen.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString(), "{IssueNo}", IssueNo, "{Revision}", Revision, "{Title}", title, "{Details}", sb.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord_ResistivityOfSemiConductingScreen-" + SampleID + ".htm");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_ResistivityOfSemiConductingScreen-" + SampleID + ".htm"));
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

        // Add this helper method to get values safely
        private string GetValueSafely(Dictionary<string, List<string>> data, string key, int index)
        {
            if (data != null && data.ContainsKey(key) && data[key] != null && index < data[key].Count)
                return data[key][index];
            return "";
        }

        int GetOccurrenceIndex(List<string> list, int currentIndex)
        {
            string value = list[currentIndex];
            int count = 0;
            for (int i = 0; i < currentIndex; i++)
            {
                if (list[i].Trim() == value.Trim()) count++;
            }
            return count;
        }

    }
}