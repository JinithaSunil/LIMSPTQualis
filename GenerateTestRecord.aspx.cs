using iTextSharp.tool.xml.parser.state;
using LIMS.DAL;
using System;
using System.Collections;
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
    public partial class GenerateTestRecord : System.Web.UI.Page
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
                        List<string> BeforeAgeingCorePhases = new List<string>();
                        List<string> AfterAgeingCorePhases = new List<string>();
                        Dictionary<string, List<string>> BeforeAgeingWidthStrip = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> BeforeAgeingPullingSpeed = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> BeforeAgeingMeasured = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> BeforeAgeingStrippabilityMin = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> BeforeAgeingStrippabilityMax = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> AfterAgeingWidthStrip = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> AfterAgeingPullingSpeed = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> AfterAgeingMeasured = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> AfterAgeingStrippabilityMin = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> AfterAgeingStrippabilityMax = new Dictionary<string, List<string>>();

                        string BeforeAgeingDate = "";
                        string AfterAgeingDate = "";
                        string AgeingCompleteCableStartDate = "";
                        string AgeingCompleteCableFinishDate = "";
                        string AgeingConditioningInDesciccatorsStartDate = "";
                        string AgeingConditioningStartDate = "";
                        string AgeingConditioningFinishDate = "";
                        string StandardWidthNom = "";
                        string StandardPullingSpeedNom = "";
                        string StandardStrippabilityMin = "";
                        string StandardStrippabilityMax = "";
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
                        string AgeingConditioningInDesciccatorsFinishDate = "";
                        string AgeingSpecimenStartDate = "";
                        string AgeingSpecimenFinishDate = "";
                        string AgeingSpecimenAirflow = "";
                        string AgeingCompleteCableAirflow = "";


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
                                if (dr["ReportParamMapID"].ToString() == "2113")
                                {
                                    BeforeAgeingCorePhases = new List<string>(dr["Result"].ToString().Split(','));
                                }

                                if (dr["ReportParamMapID"].ToString() == "2119")
                                {
                                    AfterAgeingCorePhases = new List<string>(dr["Result"].ToString().Split(','));
                                }
                                if (dr["ReportParamMapID"].ToString() == "2114")
                                {
                                    string[] values = dr["Result"].ToString().Split(',');
                                    for (int i = 0; i < values.Length; i++)
                                    {
                                        string phase = "";
                                        if (i < BeforeAgeingCorePhases.Count)
                                        {
                                            phase = BeforeAgeingCorePhases[i];
                                            if (!BeforeAgeingWidthStrip.ContainsKey(phase))
                                                BeforeAgeingWidthStrip[phase] = new List<string>();
                                            BeforeAgeingWidthStrip[phase].Add(values[i]);
                                        }
                                    }
                                }

                                if (dr["ReportParamMapID"].ToString() == "2115")
                                {
                                    string[] values = dr["Result"].ToString().Split(',');
                                    for (int i = 0; i < values.Length; i++)
                                    {
                                        string phase = "";
                                        if (i < BeforeAgeingCorePhases.Count)
                                        {
                                            phase = BeforeAgeingCorePhases[i];
                                            if (!BeforeAgeingPullingSpeed.ContainsKey(phase))
                                                BeforeAgeingPullingSpeed[phase] = new List<string>();
                                            BeforeAgeingPullingSpeed[phase].Add(values[i]);
                                        }
                                    }
                                }

                                if (dr["ReportParamMapID"].ToString() == "2116")
                                {
                                    string[] values = dr["Result"].ToString().Split(',');
                                    for (int i = 0; i < values.Length; i++)
                                    {
                                        string phase = "";
                                        if (i < BeforeAgeingCorePhases.Count)
                                        {
                                            phase = BeforeAgeingCorePhases[i];
                                            if (!BeforeAgeingMeasured.ContainsKey(phase))
                                                BeforeAgeingMeasured[phase] = new List<string>();
                                            BeforeAgeingMeasured[phase].Add(values[i]);
                                        }
                                    }
                                }
                                if (dr["ReportParamMapID"].ToString() == "2120")
                                {
                                    string[] values = dr["Result"].ToString().Split(',');
                                    for (int i = 0; i < values.Length; i++)
                                    {
                                        string phase = "";
                                        if (i < AfterAgeingCorePhases.Count)
                                        {
                                            phase = AfterAgeingCorePhases[i];
                                            if (!AfterAgeingWidthStrip.ContainsKey(phase))
                                                AfterAgeingWidthStrip[phase] = new List<string>();
                                            AfterAgeingWidthStrip[phase].Add(values[i]);
                                        }
                                    }
                                }
                                if (dr["ReportParamMapID"].ToString() == "2121")
                                {
                                    string[] values = dr["Result"].ToString().Split(',');
                                    for (int i = 0; i < values.Length; i++)
                                    {
                                        string phase = "";
                                        if (i < AfterAgeingCorePhases.Count)
                                        {
                                            phase = AfterAgeingCorePhases[i];
                                            if (!AfterAgeingPullingSpeed.ContainsKey(phase))
                                                AfterAgeingPullingSpeed[phase] = new List<string>();
                                            AfterAgeingPullingSpeed[phase].Add(values[i]);
                                        }
                                    }
                                }
                                if (dr["ReportParamMapID"].ToString() == "2122")
                                {
                                    string[] values = dr["Result"].ToString().Split(',');
                                    for (int i = 0; i < values.Length; i++)
                                    {
                                        string phase = "";
                                        if (i < AfterAgeingCorePhases.Count)
                                        {
                                            phase = AfterAgeingCorePhases[i];
                                            if (!AfterAgeingMeasured.ContainsKey(phase))
                                                AfterAgeingMeasured[phase] = new List<string>();
                                            AfterAgeingMeasured[phase].Add(values[i]);
                                        }
                                    }
                                }
                                if (dr["ReportParamMapID"].ToString() == "2123")
                                {
                                    string[] values = dr["Result"].ToString().Split(',');
                                    var distinctPhases = AfterAgeingCorePhases.Distinct().ToList();
                                    for (int i = 0; i < Math.Min(values.Length, distinctPhases.Count); i++)
                                    {
                                        string phase = distinctPhases[i];
                                        if (!AfterAgeingStrippabilityMin.ContainsKey(phase))
                                            AfterAgeingStrippabilityMin[phase] = new List<string>();
                                        AfterAgeingStrippabilityMin[phase].Add(values[i]);
                                    }
                                }
                                if (dr["ReportParamMapID"].ToString() == "2124")
                                {
                                    string[] values = dr["Result"].ToString().Split(',');
                                    var distinctPhases = AfterAgeingCorePhases.Distinct().ToList();
                                    for (int i = 0; i < Math.Min(values.Length, distinctPhases.Count); i++)
                                    {
                                        string phase = distinctPhases[i];
                                        if (!AfterAgeingStrippabilityMax.ContainsKey(phase))
                                            AfterAgeingStrippabilityMax[phase] = new List<string>();
                                        AfterAgeingStrippabilityMax[phase].Add(values[i]);
                                    }
                                }
                                if (dr["ReportParamMapID"].ToString() == "2125") { BeforeAgeingDate = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "2126") { AfterAgeingDate = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "2127") { AgeingCompleteCableStartDate = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "2128") { AgeingCompleteCableFinishDate = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "3118") { Type = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "3119") { Size = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "3120") { Voltage = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "3121") { Date = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "3122") { RoomNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "3123") { RoomTemp = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "3124") { RefSpec = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "3125") { TestMethodNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "3126") { JobOrderNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "3127") { WorkSheetNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "3128") { RoomHumidity = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "3129") { EquipmentNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "2130") { AgeingConditioningInDesciccatorsStartDate = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "2131") { AgeingConditioningStartDate = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "2132") { AgeingConditioningFinishDate = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "3113") { StandardWidthNom = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "3114") { StandardPullingSpeedNom = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "3116") { StandardStrippabilityMin = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "3117") { StandardStrippabilityMax = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "3130") { AgeingConditioningInDesciccatorsFinishDate = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "3131") { AgeingSpecimenStartDate = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "3132") { AgeingSpecimenFinishDate = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "3133") { AgeingSpecimenAirflow = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "3134") { AgeingCompleteCableAirflow = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "3135")
                                {
                                    string[] values = dr["Result"].ToString().Split(',');
                                    var distinctPhases = BeforeAgeingCorePhases.Distinct().ToList();
                                    for (int i = 0; i < Math.Min(values.Length, distinctPhases.Count); i++)
                                    {
                                        string phase = distinctPhases[i];
                                        if (!BeforeAgeingStrippabilityMin.ContainsKey(phase))
                                            BeforeAgeingStrippabilityMin[phase] = new List<string>();
                                        BeforeAgeingStrippabilityMin[phase].Add(values[i]);
                                    }
                                }
                                if (dr["ReportParamMapID"].ToString() == "3136")
                                {
                                    string[] values = dr["Result"].ToString().Split(',');
                                    var distinctPhases = BeforeAgeingCorePhases.Distinct().ToList();
                                    for (int i = 0; i < Math.Min(values.Length, distinctPhases.Count); i++)
                                    {
                                        string phase = distinctPhases[i];
                                        if (!BeforeAgeingStrippabilityMax.ContainsKey(phase))
                                            BeforeAgeingStrippabilityMax[phase] = new List<string>();
                                        BeforeAgeingStrippabilityMax[phase].Add(values[i]);
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
                            </table>");
                        }
                        sbDetails.Append(@"<table class='tab1'>
                                            <tr>
                                                <td rowspan='2' style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Date</td>
                                                <td rowspan='2' style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Item Test</td>
                                                <td rowspan='2' style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Core<br />Phase</td>
                                                <td rowspan='2' style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Specimen No.</td>
                                                <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Width Strip<br />Insulation screen</td>
                                                <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Pulling Speed</td>
                                                <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Measured</td>
                                                <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;' colspan='2'>Strippability</td>
                                            </tr>
                                            <tr>
                                                <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>mm</td>
                                                <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>mm/min</td>
                                                <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>N</td>
                                                <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Min.</td>
                                                <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Max.</td>
                                            </tr>");

                        // Process Before Ageing data
                        if (BeforeAgeingCorePhases.Count > 0)
                        {
                            // Get distinct phases and count for rowspan
                            var distinctPhases = BeforeAgeingCorePhases.Distinct().ToList();
                            int totalSpecimens = BeforeAgeingCorePhases.Count;

                            // Add date and test name once for all before ageing data
                            sbDetails.Append("<tr>");
                            sbDetails.Append("<td rowspan='" + totalSpecimens + "'>" + BeforeAgeingDate + "</td>");
                            sbDetails.Append("<td rowspan='" + totalSpecimens + "'>Before Ageing</td>");

                            // Track if we have closed the first row
                            bool firstRow = true;

                            // Process each distinct phase
                            foreach (string phase in distinctPhases)
                            {
                                // Count specimens for this phase
                                int phaseCount = BeforeAgeingCorePhases.Count(p => p == phase);

                                // Add phase name
                                if (!firstRow)
                                    sbDetails.Append("<tr>");

                                sbDetails.Append("<td rowspan='" + phaseCount + "'>" + phase + "</td>");

                                // Get min/max for this phase (showing only once per phase)
                                string minValue = BeforeAgeingStrippabilityMin.ContainsKey(phase) && BeforeAgeingStrippabilityMin[phase].Count > 0
                                    ? BeforeAgeingStrippabilityMin[phase][0] : "";
                                string maxValue = BeforeAgeingStrippabilityMax.ContainsKey(phase) && BeforeAgeingStrippabilityMax[phase].Count > 0
                                    ? BeforeAgeingStrippabilityMax[phase][0] : "";

                                // Add specimens for this phase
                                for (int i = 0; i < phaseCount; i++)
                                {
                                    // Only add new row tag after first specimen of each phase
                                    if (i > 0)
                                        sbDetails.Append("<tr>");

                                    // Add specimen number
                                    sbDetails.Append("<td>" + (i + 1) + "</td>");

                                    // Add measurements for this specimen if available
                                    sbDetails.Append("<td>" + GetValueSafely(BeforeAgeingWidthStrip, phase, i) + "</td>");
                                    sbDetails.Append("<td>" + GetValueSafely(BeforeAgeingPullingSpeed, phase, i) + "</td>");
                                    sbDetails.Append("<td>" + GetValueSafely(BeforeAgeingMeasured, phase, i) + "</td>");

                                    // Add strippability only for first specimen of each phase
                                    if (i == 0)
                                    {
                                        sbDetails.Append("<td rowspan='" + phaseCount + "'>" + minValue + "</td>");
                                        sbDetails.Append("<td rowspan='" + phaseCount + "'>" + maxValue + "</td>");
                                    }

                                    sbDetails.Append("</tr>");
                                }

                                firstRow = false;
                            }
                        }

                        // Process After Ageing data
                        if (AfterAgeingCorePhases.Count > 0)
                        {
                            // Get distinct phases and count for rowspan
                            var distinctPhases = AfterAgeingCorePhases.Distinct().ToList();
                            int totalSpecimens = AfterAgeingCorePhases.Count;

                            // Add date and test name once for all before ageing data
                            sbDetails.Append("<tr>");
                            sbDetails.Append("<td rowspan='" + totalSpecimens + "'>" + AfterAgeingDate + "</td>");
                            sbDetails.Append("<td rowspan='" + totalSpecimens + "'>After ageing complete cable 100°C/168 h</td>");

                            // Track if we have closed the first row
                            bool firstRow = true;

                            // Process each distinct phase
                            foreach (string phase in distinctPhases)
                            {
                                // Count specimens for this phase
                                int phaseCount = AfterAgeingCorePhases.Count(p => p == phase);

                                // Add phase name
                                if (!firstRow)
                                    sbDetails.Append("<tr>");

                                sbDetails.Append("<td rowspan='" + phaseCount + "'>" + phase + "</td>");

                                // Get min/max for this phase (showing only once per phase)
                                string minValue = AfterAgeingStrippabilityMin.ContainsKey(phase) && AfterAgeingStrippabilityMin[phase].Count > 0
                                    ? AfterAgeingStrippabilityMin[phase][0] : "";
                                string maxValue = AfterAgeingStrippabilityMax.ContainsKey(phase) && AfterAgeingStrippabilityMax[phase].Count > 0
                                    ? AfterAgeingStrippabilityMax[phase][0] : "";

                                // Add specimens for this phase
                                for (int i = 0; i < phaseCount; i++)
                                {
                                    // Only add new row tag after first specimen of each phase
                                    if (i > 0)
                                        sbDetails.Append("<tr>");

                                    // Add specimen number
                                    sbDetails.Append("<td>" + (i + 1) + "</td>");

                                    // Add measurements for this specimen if available
                                    sbDetails.Append("<td>" + GetValueSafely(AfterAgeingWidthStrip, phase, i) + "</td>");
                                    sbDetails.Append("<td>" + GetValueSafely(AfterAgeingPullingSpeed, phase, i) + "</td>");
                                    sbDetails.Append("<td>" + GetValueSafely(AfterAgeingMeasured, phase, i) + "</td>");

                                    // Add strippability only for first specimen of each phase
                                    if (i == 0)
                                    {
                                        sbDetails.Append("<td rowspan='" + phaseCount + "'>" + minValue + "</td>");
                                        sbDetails.Append("<td rowspan='" + phaseCount + "'>" + maxValue + "</td>");
                                    }

                                    sbDetails.Append("</tr>");
                                }

                                firstRow = false;
                            }
                        }

                        sbDetails.Append(@"</table>");
                        sbDates.Append(@"<table class='tab1'>
                                <tr>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;' colspan='4'>Ageing Specimen</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;' colspan='4'>Ageing Complete Cable</td>
                                </tr>
                                <tr>
                                    <td>Airflow: " + AgeingSpecimenAirflow + "</td>" +
                                   @"<td colspan='3'>Standard: 8-20 air change per hour at temperature</td>" +
                                   @"<td>Airflow: " + AgeingCompleteCableAirflow + "</td>" +
                                   @"<td colspan='3'>Standard: 8-20 air change per hour at temperature</td>
                                </tr>
                                <tr>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;' colspan='2'>Date</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;' colspan='2'>Time</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;' colspan='2'>Date</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;' colspan='2'>Time</td>
                                </tr>
                                <tr>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Start</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Finish</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Start</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Finish</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Start</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Finish</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Start</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Finish</td>
                                </tr>
                                <tr>
                                    <td>
                                        " + Convert.ToDateTime(AgeingSpecimenStartDate).ToString("yyyy-MM-dd") +
                                   @"</td>
                                    <td>
                                         " + Convert.ToDateTime(AgeingSpecimenFinishDate).ToString("yyyy-MM-dd") +
                                   @"</td>
                                    <td>
                                         " + Convert.ToDateTime(AgeingSpecimenStartDate).ToString("HH:mm") +
                                   @"</td>
                                    <td>
                                         " + Convert.ToDateTime(AgeingSpecimenFinishDate).ToString("HH:mm") +
                                   @"</td>
                                    <td>
                                        " + Convert.ToDateTime(AgeingCompleteCableStartDate).ToString("yyyy-MM-dd") +
                                   @"</td>
                                    <td> " + Convert.ToDateTime(AgeingCompleteCableFinishDate).ToString("yyyy-MM-dd") +
                                   @"<td>
                                         " + Convert.ToDateTime(AgeingCompleteCableStartDate).ToString("HH:mm") +
                                   @"</td>
                                    <td> " + Convert.ToDateTime(AgeingCompleteCableFinishDate).ToString("HH:mm") +
                                   @"</td>
                                </tr>
                                <tr>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;' colspan='4'>Conditioning in desiccators</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;' colspan='4'>Conditioning</td>
                                </tr>
                                <tr>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;' colspan='2'>Date</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;' colspan='2'>Time</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;' colspan='2'>Date</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;' colspan='2'>Time</td>
                                </tr>
                                <tr>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Start</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Finish</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Start</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Finish</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Start</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Finish</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Start</td>
                                    <td style='background-color:#bebebe;font-weight: bold;vertical-align: middle;'>Finish</td>
                                </tr>
                                <tr>
                                    <td>" + Convert.ToDateTime(AgeingConditioningInDesciccatorsStartDate).ToString("yyyy-MM-dd") + "</td>" +
                                  @"<td>" + Convert.ToDateTime(AgeingConditioningInDesciccatorsFinishDate).ToString("yyyy-MM-dd") + "</td>" +
                                  @"<td> " + Convert.ToDateTime(AgeingConditioningInDesciccatorsStartDate).ToString("HH:mm") + " </td>" +
                                  @"<td> " + Convert.ToDateTime(AgeingConditioningInDesciccatorsFinishDate).ToString("HH:mm") + " </td>" +
                                  @"<td > " + Convert.ToDateTime(AgeingConditioningStartDate).ToString("yyyy-MM-dd") + " </ td > " +
                                  @"<td>" + Convert.ToDateTime(AgeingConditioningFinishDate).ToString("yyyy-MM-dd") + "</td>" +
                                  @"<td> " + Convert.ToDateTime(AgeingConditioningStartDate).ToString("HH:mm") + " </td>" +
                                  @"<td> " + Convert.ToDateTime(AgeingConditioningFinishDate).ToString("HH:mm") + " </td></tr></table>");

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

                        body = objEmail.PrepareMailBodyWith("TestRecord.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString(), "{IssueNo}", IssueNo, "{Revision}", Revision, "{Title}", title, "{Details}", sb.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord-" + SampleID + ".htm");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord-" + SampleID + ".htm"));
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