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
    public partial class GenerateTestRecord_ResistivityOfSemiConductingScreen : System.Web.UI.Page
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
                        List<string> InsulationscreenBeforeAgeingCore = new List<string>();
                        List<string> InsulationscreenAfterAgeingCore = new List<string>();
                        Dictionary<string, List<string>> InsulationscreenBeforeAgeingDiameter = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> InsulationscreenBeforeAgeingThickness = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> InsulationscreenBeforeAgeingVolumeResistivity = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> InsulationscreenAfterAgeingDiameter = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> InsulationscreenAfterAgeingThickness = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> InsulationscreenAfterAgeingVolumeResistivity = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> StandardbeforeafterageingVolumeResistivity = new Dictionary<string, List<string>>();
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
                                if (dr["ReportParamMapID"].ToString() == "5177")
                                {
                                    InsulationscreenBeforeAgeingCore = new List<string>(dr["Result"].ToString().Split(','));
                                }
                                else if (dr["ReportParamMapID"].ToString() == "5208")
                                {
                                    InsulationscreenAfterAgeingCore = new List<string>(dr["Result"].ToString().Split(','));
                                }
                            }

                            // Second pass: Process the remaining parameters after core lists are established
                            foreach (DataRow dr in dtRecordDetails.Rows)
                            {
                                // Process diameter, thickness, and volume resistivity data
                                if (dr["ReportParamMapID"].ToString() == "5164") // Before Ageing Diameter
                                {
                                    string[] values = dr["Result"].ToString().Split(',');
                                    for (int i = 0; i < InsulationscreenBeforeAgeingCore.Count && i < values.Length; i++)
                                    {
                                        string core = InsulationscreenBeforeAgeingCore[i].Trim();
                                        if (!InsulationscreenBeforeAgeingDiameter.ContainsKey(core))
                                            InsulationscreenBeforeAgeingDiameter[core] = new List<string>();

                                        InsulationscreenBeforeAgeingDiameter[core].Add(values[i].Trim());
                                    }
                                }
                                else if (dr["ReportParamMapID"].ToString() == "5165") // Before Ageing Thickness
                                {
                                    string[] values = dr["Result"].ToString().Split(',');
                                    for (int i = 0; i < InsulationscreenBeforeAgeingCore.Count && i < values.Length; i++)
                                    {
                                        string core = InsulationscreenBeforeAgeingCore[i].Trim();
                                        if (!InsulationscreenBeforeAgeingThickness.ContainsKey(core))
                                            InsulationscreenBeforeAgeingThickness[core] = new List<string>();

                                        InsulationscreenBeforeAgeingThickness[core].Add(values[i].Trim());
                                    }
                                }
                                else if (dr["ReportParamMapID"].ToString() == "5166") // Before Ageing Volume Resistivity
                                {
                                    string[] values = dr["Result"].ToString().Split(',');
                                    for (int i = 0; i < InsulationscreenBeforeAgeingCore.Count && i < values.Length; i++)
                                    {
                                        string core = InsulationscreenBeforeAgeingCore[i].Trim();
                                        if (!InsulationscreenBeforeAgeingVolumeResistivity.ContainsKey(core))
                                            InsulationscreenBeforeAgeingVolumeResistivity[core] = new List<string>();

                                        InsulationscreenBeforeAgeingVolumeResistivity[core].Add(values[i].Trim());
                                    }
                                }
                                else if (dr["ReportParamMapID"].ToString() == "5167") // After Ageing Diameter
                                {
                                    string[] values = dr["Result"].ToString().Split(',');
                                    for (int i = 0; i < InsulationscreenAfterAgeingCore.Count && i < values.Length; i++)
                                    {
                                        string core = InsulationscreenAfterAgeingCore[i].Trim();
                                        if (!InsulationscreenAfterAgeingDiameter.ContainsKey(core))
                                            InsulationscreenAfterAgeingDiameter[core] = new List<string>();

                                        InsulationscreenAfterAgeingDiameter[core].Add(values[i].Trim());
                                    }
                                }
                                else if (dr["ReportParamMapID"].ToString() == "5168") // After Ageing Thickness
                                {
                                    string[] values = dr["Result"].ToString().Split(',');
                                    for (int i = 0; i < InsulationscreenAfterAgeingCore.Count && i < values.Length; i++)
                                    {
                                        string core = InsulationscreenAfterAgeingCore[i].Trim();
                                        if (!InsulationscreenAfterAgeingThickness.ContainsKey(core))
                                            InsulationscreenAfterAgeingThickness[core] = new List<string>();

                                        InsulationscreenAfterAgeingThickness[core].Add(values[i].Trim());
                                    }
                                }
                                else if (dr["ReportParamMapID"].ToString() == "5169") // After Ageing Volume Resistivity
                                {
                                    string[] values = dr["Result"].ToString().Split(',');
                                    for (int i = 0; i < InsulationscreenAfterAgeingCore.Count && i < values.Length; i++)
                                    {
                                        string core = InsulationscreenAfterAgeingCore[i].Trim();
                                        if (!InsulationscreenAfterAgeingVolumeResistivity.ContainsKey(core))
                                            InsulationscreenAfterAgeingVolumeResistivity[core] = new List<string>();

                                        InsulationscreenAfterAgeingVolumeResistivity[core].Add(values[i].Trim());
                                    }
                                }
                                else if (dr["ReportParamMapID"].ToString() == "5170") // Standard before & after ageing Volume Resistivity
                                {
                                    // Store the standard value
                                    string standardValue = dr["Result"].ToString().Trim();
                                    StandardbeforeafterageingVolumeResistivity["Standard"] = new List<string> { standardValue };
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

                        int totalRows = InsulationscreenBeforeAgeingCore.Count + InsulationscreenAfterAgeingCore.Count;

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
                        // Add Before Ageing Section
                        if (InsulationscreenBeforeAgeingCore.Count > 0)
                        {
                            // Group cores for rowspan calculation
                            Dictionary<string, int> coreCountBefore = new Dictionary<string, int>();
                            foreach (string core in InsulationscreenBeforeAgeingCore)
                            {
                                string coreKey = core.Trim();
                                if (!coreCountBefore.ContainsKey(coreKey))
                                    coreCountBefore[coreKey] = 0;
                                coreCountBefore[coreKey]++;
                            }

                            string lastCore = "";
                            int rowsProcessed = 0;

                            for (int i = 0; i < InsulationscreenBeforeAgeingCore.Count; i++)
                            {
                                string core = InsulationscreenBeforeAgeingCore[i].Trim();
                                sbDetails.Append("<tr>");

                                if (!testItemAdded)
                                {
                                    sbDetails.Append($"<td rowspan='{totalRows}' style='vertical-align: middle; text-align: center;'>Insulation screen</td>");
                                    testItemAdded = true;
                                }

                                if (i == 0)
                                    sbDetails.Append("<td rowspan='" + InsulationscreenBeforeAgeingCore.Count + "'>Before Ageing</td>");

                                int occurrenceIndex = GetOccurrenceIndex(InsulationscreenBeforeAgeingCore, i);

                                // Only add the core cell if it's a new core or the first item
                                if (core != lastCore || i == 0)
                                {
                                    sbDetails.Append("<td rowspan='" + coreCountBefore[core] + "'>" + core + "</td>");
                                    lastCore = core;
                                }

                                sbDetails.Append("<td>" + GetValueSafely(InsulationscreenBeforeAgeingDiameter, core, occurrenceIndex) + "</td>");
                                sbDetails.Append("<td>" + GetValueSafely(InsulationscreenBeforeAgeingThickness, core, occurrenceIndex) + "</td>");
                                sbDetails.Append("<td>" + GetValueSafely(InsulationscreenBeforeAgeingVolumeResistivity, core, occurrenceIndex) + "</td>");

                                sbDetails.Append("</tr>");
                            }
                        }

                        // Add After Ageing Section
                        if (InsulationscreenAfterAgeingCore.Count > 0)
                        {
                            // Group cores for rowspan calculation
                            Dictionary<string, int> coreCountAfter = new Dictionary<string, int>();
                            foreach (string core in InsulationscreenAfterAgeingCore)
                            {
                                string coreKey = core.Trim();
                                if (!coreCountAfter.ContainsKey(coreKey))
                                    coreCountAfter[coreKey] = 0;
                                coreCountAfter[coreKey]++;
                            }

                            string lastCore = "";

                            for (int i = 0; i < InsulationscreenAfterAgeingCore.Count; i++)
                            {
                                string core = InsulationscreenAfterAgeingCore[i].Trim();
                                sbDetails.Append("<tr>");

                                if (i == 0)
                                    sbDetails.Append("<td rowspan='" + InsulationscreenAfterAgeingCore.Count + "'>After Ageing</td>");

                                int occurrenceIndex = GetOccurrenceIndex(InsulationscreenAfterAgeingCore, i);

                                // Only add the core cell if it's a new core or the first item
                                if (core != lastCore || i == 0)
                                {
                                    sbDetails.Append("<td rowspan='" + coreCountAfter[core] + "'>" + core + "</td>");
                                    lastCore = core;
                                }

                                sbDetails.Append("<td>" + GetValueSafely(InsulationscreenAfterAgeingDiameter, core, occurrenceIndex) + "</td>");
                                sbDetails.Append("<td>" + GetValueSafely(InsulationscreenAfterAgeingThickness, core, occurrenceIndex) + "</td>");
                                sbDetails.Append("<td>" + GetValueSafely(InsulationscreenAfterAgeingVolumeResistivity, core, occurrenceIndex) + "</td>");

                                sbDetails.Append("</tr>");
                            }
                        }

                        // Get standard value - if exists
                        string stdValue = "0"; // Default value
                        if (StandardbeforeafterageingVolumeResistivity.ContainsKey("Standard") &&
                            StandardbeforeafterageingVolumeResistivity["Standard"].Count > 0)
                        {
                            stdValue = StandardbeforeafterageingVolumeResistivity["Standard"][0];
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
        //                    if (dtRecordDetails.Rows.Count > 0)
        //                    {
        //                        foreach (DataRow dr in dtRecordDetails.Rows)
        //                        {
        //                            // Header information
        //                            if (dr["ReportParamMapID"].ToString() == "4177") { Type = dr["Result"].ToString(); }
        //                            if (dr["ReportParamMapID"].ToString() == "4178") { Size = dr["Result"].ToString(); }
        //                            if (dr["ReportParamMapID"].ToString() == "4179") { Voltage = dr["Result"].ToString(); }
        //                            if (dr["ReportParamMapID"].ToString() == "4180") { Date = dr["Result"].ToString(); }
        //                            if (dr["ReportParamMapID"].ToString() == "4181") { RoomNo = dr["Result"].ToString(); }
        //                            if (dr["ReportParamMapID"].ToString() == "4182") { RoomTemp = dr["Result"].ToString(); }
        //                            if (dr["ReportParamMapID"].ToString() == "4183") { RefSpec = dr["Result"].ToString(); }
        //                            if (dr["ReportParamMapID"].ToString() == "4184") { TestMethodNo = dr["Result"].ToString(); }
        //                            if (dr["ReportParamMapID"].ToString() == "4185") { JobOrderNo = dr["Result"].ToString(); }
        //                            if (dr["ReportParamMapID"].ToString() == "4186") { WorkSheetNo = dr["Result"].ToString(); }
        //                            if (dr["ReportParamMapID"].ToString() == "4187") { RoomHumidity = dr["Result"].ToString(); }
        //                            if (dr["ReportParamMapID"].ToString() == "4188") { EquipmentNo = dr["Result"].ToString(); }

        //                            // Data for Before Aging tests
        //                            if (dr["ReportParamMapID"].ToString() == "5177") { beforeAgingCore = new List<string>(dr["Result"].ToString().Split(',')); }
        //                            if (dr["ReportParamMapID"].ToString() == "5164") { beforeAgingDiameter = new List<string>(dr["Result"].ToString().Split(',')); }
        //                            if (dr["ReportParamMapID"].ToString() == "5165") { beforeAgingThickness = new List<string>(dr["Result"].ToString().Split(',')); }
        //                            if (dr["ReportParamMapID"].ToString() == "5166") { beforeAgingResistivity = new List<string>(dr["Result"].ToString().Split(',')); }

        //                            // Data for After Aging tests
        //                            if (dr["ReportParamMapID"].ToString() == "5208") { afterAgingCore = new List<string>(dr["Result"].ToString().Split(',')); }
        //                            if (dr["ReportParamMapID"].ToString() == "5167") { afterAgingDiameter = new List<string>(dr["Result"].ToString().Split(',')); }
        //                            if (dr["ReportParamMapID"].ToString() == "5168") { afterAgingThickness = new List<string>(dr["Result"].ToString().Split(',')); }
        //                            if (dr["ReportParamMapID"].ToString() == "5169") { afterAgingResistivity = new List<string>(dr["Result"].ToString().Split(',')); }

        //                            // Max standard value
        //                            if (dr["ReportParamMapID"].ToString() == "5170") { maxStandard = dr["Result"].ToString(); }
        //                        }

        //                        // Header table
        //                        sbHeader.Append(@"<table style='width: 100%; border: 1px solid; margin-top: 10px; ' class='tab1'>
        //                            <tr>
        //                                <td>Type</td>
        //                                <td>" + Type + "</td>" +
        //                        @" <td>Ref Spec.</td>
        //                                <td>" + RefSpec + "</td>" +
        //                        @"</tr>
        //                            <tr>
        //                                <td> Size</td>
        //                                <td>" + Size + "</td>" +
        //                        @"<td>Test Method No.</td>
        //                               <td>" + TestMethodNo + "</td>" +
        //                        @" </tr>
        //                            <tr>
        //                                <td>Voltage</td>
        //                               <td>" + Voltage + "</td>" +
        //                        @"<td>Job Order No.</td>
        //                               <td>" + JobOrderNo + "</td>" +
        //                        @"</tr>
        //                            <tr>
        //                                <td>Date</td>
        //                              <td>" + Date + "</td>" +
        //                        @"<td>Work Sheet No.</td>
        //                               <td>" + WorkSheetNo + "</td>" +
        //                        @"</tr>
        //                            <tr>
        //                                <td>Room No.</td>
        //                               <td>" + RoomNo + "</td>" +
        //                        @"<td>Room Humidity</td>
        //                                <td>" + RoomHumidity + "%" + "</td>" +
        //                        @"</tr>
        //                            <tr>
        //                                <td>Room Temp.</td>
        //                                <td> " + RoomTemp + "°C" + "</td>" +
        //                        @" <td>Equipment No.</td>
        //                               <td>" + EquipmentNo + "</td>" +
        //                        @"</tr>
        //                        </table>");


        //                        // Add testing condition note
        //                        sbDetails.Append(@"<p style='margin-top: 5px;'>Testing in 90°C air oven</p>");

        //                        // Create the Resistivity Test Table
        //                        sbDetails.Append(@"<table style='width: 100%; border: 1px solid black; border-collapse: collapse; margin-top: 5px;'>
        //<tr>
        //    <th style='border: 1px solid black; padding: 5px;'>Test Item</th>
        //    <th style='border: 1px solid black; padding: 5px;'>Core</th>
        //    <th style='border: 1px solid black; padding: 5px;'>Diameter (Avg)<br/>mm</th>
        //    <th style='border: 1px solid black; padding: 5px;'>Thickness (Avg)<br/>mm</th>
        //    <th style='border: 1px solid black; padding: 5px;'>Volume Resistivity<br/>Ω.m</th>
        //</tr>");

        //                        // Before Aging section
        //                        for (int i = 0; i < beforeAgingCore.Count; i++)
        //                        {
        //                            if (i == 0)
        //                            {
        //                                sbDetails.Append(@"<tr>
        //                            <td style='border: 1px solid black; padding: 5px; text-align: center;' rowspan='" + beforeAgingCore.Count + @"'>Before<br/>Ageing</td>
        //                            <td style='border: 1px solid black; padding: 5px; text-align: center;'>" + beforeAgingCore[i] + @"</td>
        //                            <td style='border: 1px solid black; padding: 5px; text-align: center;'>" + beforeAgingDiameter[i] + @"</td>
        //                            <td style='border: 1px solid black; padding: 5px; text-align: center;'>" + beforeAgingThickness[i] + @"</td>
        //                            <td style='border: 1px solid black; padding: 5px; text-align: center;'>" + beforeAgingResistivity[i] + @"</td>
        //                        </tr>");
        //                            }
        //                            else
        //                            {
        //                                sbDetails.Append(@"<tr>
        //                            <td style='border: 1px solid black; padding: 5px; text-align: center;'>" + beforeAgingCore[i] + @"</td>
        //                            <td style='border: 1px solid black; padding: 5px; text-align: center;'>" + beforeAgingDiameter[i] + @"</td>
        //                            <td style='border: 1px solid black; padding: 5px; text-align: center;'>" + beforeAgingThickness[i] + @"</td>
        //                            <td style='border: 1px solid black; padding: 5px; text-align: center;'>" + beforeAgingResistivity[i] + @"</td>
        //                        </tr>");
        //                            }
        //                        }

        //                        // Add Insulation screen row
        //                        sbDetails.Append(@"<tr>
        //                    <td style='border: 1px solid black; padding: 5px; text-align: center;' rowspan='" + (afterAgingCore.Count + 1) + @"'>Insulation<br/>screen</td>
        //                </tr>");

        //                        // After Aging section
        //                        for (int i = 0; i < afterAgingCore.Count; i++)
        //                        {
        //                            if (i == 0)
        //                            {
        //                                sbDetails.Append(@"<tr>
        //                            <td style='border: 1px solid black; padding: 5px; text-align: center;' rowspan='" + afterAgingCore.Count + @"'>After<br/>Ageing</td>
        //                            <td style='border: 1px solid black; padding: 5px; text-align: center;'>" + afterAgingCore[i] + @"</td>
        //                            <td style='border: 1px solid black; padding: 5px; text-align: center;'>" + afterAgingDiameter[i] + @"</td>
        //                            <td style='border: 1px solid black; padding: 5px; text-align: center;'>" + afterAgingThickness[i] + @"</td>
        //                            <td style='border: 1px solid black; padding: 5px; text-align: center;'>" + afterAgingResistivity[i] + @"</td>
        //                        </tr>");
        //                            }
        //                            else
        //                            {
        //                                sbDetails.Append(@"<tr>
        //                            <td style='border: 1px solid black; padding: 5px; text-align: center;'>" + afterAgingCore[i] + @"</td>
        //                            <td style='border: 1px solid black; padding: 5px; text-align: center;'>" + afterAgingDiameter[i] + @"</td>
        //                            <td style='border: 1px solid black; padding: 5px; text-align: center;'>" + afterAgingThickness[i] + @"</td>
        //                            <td style='border: 1px solid black; padding: 5px; text-align: center;'>" + afterAgingResistivity[i] + @"</td>
        //                        </tr>");
        //                            }
        //                        }

        //                        // Standard row
        //                        sbDetails.Append(@"<tr>
        //                    <td style='border: 1px solid black; padding: 5px; text-align: center;' colspan='4'>Standard before & after ageing</td>
        //                    <td style='border: 1px solid black; padding: 5px; text-align: center;'>Max: " + maxStandard + @"</td>
        //                </tr>");

        //                        sbDetails.Append(@"</table>");

        //                        // Footer with signature section
        //                        sbFooter.Append(@" <table style='width:100%; font-family:Times New Roman; font-size:11px' class='tab1'>
        //                    <tr>
        //                        <td rowspan='2' ><b>PT Qualis Indonesia</b></td>
        //                        <td colspan='3' style='text-align:center'><b>Witnessed By</b></td>
        //                    </tr>
        //                    <tr>
        //                       <td>Name</td><td>Company</td><td>Signature</td>
        //                    </tr>
        //                    <tr>
        //                        <td style='text-align:left'>Tested By:<br /><br /><br />Checked By:<br /><br /><br /></td>
        //                        <td></td><td></td>
        //                    </tr>
        //                </table>");
        //                    }
