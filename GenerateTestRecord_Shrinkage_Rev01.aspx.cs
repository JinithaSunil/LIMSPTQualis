using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LIMS;
using LIMS.DAL;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace LIMS
{
	public partial class GenerateTestRecord_Shrinkage_Rev01 : System.Web.UI.Page
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

                        // Insulation specific data
                        List<string> InsulationCore = new List<string>();
                        Dictionary<string, List<string>> InsulationSpecimenNo = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> InsulationTemperature = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> InsulationDuration = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> InsulationDistanceBeforeHeating = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> InsulationDistanceAfterHeating = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> InsulationResult = new Dictionary<string, List<string>>();

                        // Specified values
                        string InsulationSpecifiedTemperature = "";
                        string InsulationSpecifiedDuration = "";
                        string InsulationSpecifiedResult = "";
                        string InsulationSpecifiedBeforeHeating = "";
                        string InsulationSpecifiedAfterHeating = "";
                        string ItemType = "";

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
                            // First pass: Get the core list before processing other parameters
                            foreach (DataRow dr in dtRecordDetails.Rows)
                            {
                                if (dr["ReportParamMapID"].ToString() == "14041")
                                {
                                    InsulationCore = new List<string>(dr["Result"].ToString().Split(','));
                                }
                            }

                            // Second pass: Process the remaining parameters
                            foreach (DataRow dr in dtRecordDetails.Rows)
                            {
                                // Process specimen number data
                                if (dr["ReportParamMapID"].ToString() == "14042") // Temperature
                                {
                                    string[] values = dr["Result"].ToString().Split(',');
                                    for (int i = 0; i < InsulationCore.Count && i < values.Length; i++)
                                    {
                                        string core = InsulationCore[i].Trim();
                                        if (!InsulationTemperature.ContainsKey(core))
                                            InsulationTemperature[core] = new List<string>();

                                        InsulationTemperature[core].Add(values[i].Trim());
                                    }
                                }
                                else if (dr["ReportParamMapID"].ToString() == "14043") // Duration
                                {
                                    string[] values = dr["Result"].ToString().Split(',');
                                    for (int i = 0; i < InsulationCore.Count && i < values.Length; i++)
                                    {
                                        string core = InsulationCore[i].Trim();
                                        if (!InsulationDuration.ContainsKey(core))
                                            InsulationDuration[core] = new List<string>();

                                        InsulationDuration[core].Add(values[i].Trim());
                                    }
                                }
                                else if (dr["ReportParamMapID"].ToString() == "14044") // Distance before heating
                                {
                                    string[] values = dr["Result"].ToString().Split(',');
                                    for (int i = 0; i < InsulationCore.Count && i < values.Length; i++)
                                    {
                                        string core = InsulationCore[i].Trim();
                                        if (!InsulationDistanceBeforeHeating.ContainsKey(core))
                                            InsulationDistanceBeforeHeating[core] = new List<string>();

                                        InsulationDistanceBeforeHeating[core].Add(values[i].Trim());
                                    }
                                }
                                else if (dr["ReportParamMapID"].ToString() == "14045") // Distance after heating
                                {
                                    string[] values = dr["Result"].ToString().Split(',');
                                   
                                    for (int i = 0; i < InsulationCore.Count && i < values.Length; i++)
                                    {
                                        string core = InsulationCore[i].Trim();
                                        if (!InsulationDistanceAfterHeating.ContainsKey(core))
                                            InsulationDistanceAfterHeating[core] = new List<string>();

                                        InsulationDistanceAfterHeating[core].Add(values[i].Trim());
                                    }
                                }
                                else if (dr["ReportParamMapID"].ToString() == "14046") // Result
                                {
                                    string[] values = dr["Result"].ToString().Split(',');
                                    for (int i = 0; i < InsulationCore.Count && i < values.Length; i++)
                                    {
                                        string core = InsulationCore[i].Trim();
                                        if (!InsulationResult.ContainsKey(core))
                                            InsulationResult[core] = new List<string>();

                                        InsulationResult[core].Add(values[i].Trim());
                                    }
                                }


                                // Process specified values
                                if (dr["ReportParamMapID"].ToString() == "14047") { InsulationSpecifiedTemperature = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14048") { InsulationSpecifiedDuration = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14049") { InsulationSpecifiedBeforeHeating = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14050") { InsulationSpecifiedAfterHeating = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14051") { InsulationSpecifiedResult = dr["Result"].ToString(); }

                                // Process other general parameters
                                if (dr["ReportParamMapID"].ToString() == "13081") { Type = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "13082") { Size = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14031") { Voltage = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14032") { Date = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14033") { RoomNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14034") { RoomTemp = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14035") { RefSpec = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14036") { TestMethodNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14037") { JobOrderNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14038") { WorkSheetNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14039") { RoomHumidity = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14040") { EquipmentNo = dr["Result"].ToString(); }
                                //if (dr["ReportParamMapID"].ToString() == "14056") { ItemType = dr["Result"].ToString(); }

                            }
                            // Build header table
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
                        }     // Build the Insulation test details table

                        sbDetails.Append($@"
<table class='tab1' border='1' style='border-collapse: collapse; width: 100%;'>
<tr>
    <td colspan='7' style='text-align: left; font-weight: bold; background-color: #bebebe;'>Insulation / Sheath</td>
</tr>
<tr>
    <td rowspan='2' style='text-align: center; font-weight: bold; background-color: #bebebe;'>Core</td>
    <td rowspan='2' style='text-align: center; font-weight: bold; background-color: #bebebe;'>Specimen<br>No.</td>
    <td rowspan='2' style='text-align: center; font-weight: bold; background-color: #bebebe;'>Temperature<br>°C</td>
    <td rowspan='2' style='text-align: center; font-weight: bold; background-color: #bebebe;'>Duration<br>Hours</td>
    <td colspan='2' style='text-align: center; font-weight: bold; background-color: #bebebe;'>Distance L/mm between mark</td>
    <td rowspan='2' style='text-align: center; font-weight: bold; background-color: #bebebe;'>Result<br>%</td>
</tr>
<tr>
    <td style='text-align: center; font-weight: bold; background-color: #bebebe;'>Before heating</td>
    <td style='text-align: center; font-weight: bold; background-color: #bebebe;'>After heating</td>
</tr>");


                        // Group cores together for proper rowspan calculation
                        Dictionary<string, List<int>> coreSpecimenIndices = new Dictionary<string, List<int>>();
                        for (int i = 0; i < InsulationCore.Count; i++)
                        {
                            string core = InsulationCore[i].Trim();
                            if (!coreSpecimenIndices.ContainsKey(core))
                                coreSpecimenIndices[core] = new List<int>();
                            coreSpecimenIndices[core].Add(i);
                        }

                        // Process each core and its specimens
                        foreach (var coreEntry in coreSpecimenIndices)
                        {
                            string core = coreEntry.Key;
                            List<int> indices = coreEntry.Value;
                            int specimenCount = indices.Count;

                            bool firstSpecimen = true;
                            int specimenNumber = 1; // Start specimen numbering from 1

                            foreach (int index in indices)
                            {
                                sbDetails.Append("<tr>");

                                // Only add core cell for first specimen of this core with rowspan for all specimens
                                if (firstSpecimen)
                                {
                                    sbDetails.Append("<td rowspan='" + specimenCount + "' style='text-align: center; vertical-align: middle;'>" + core + "</td>");
                                    firstSpecimen = false;
                                }

                                // Add specimen number (incrementing from 1 for each core)
                                sbDetails.Append("<td style='text-align: center;'>" + specimenNumber + "</td>");

                                // Add the rest of the specimen data
                                sbDetails.Append("<td style='text-align: center;'>" + GetValueSafely(InsulationTemperature, core, specimenNumber - 1) + "</td>");
                                sbDetails.Append("<td style='text-align: center;'>" + GetValueSafely(InsulationDuration, core, specimenNumber - 1) + "</td>");
                                sbDetails.Append("<td style='text-align: center;'>" + GetValueSafely(InsulationDistanceBeforeHeating, core, specimenNumber - 1) + "</td>");
                                sbDetails.Append("<td style='text-align: center;'>" + GetValueSafely(InsulationDistanceAfterHeating, core, specimenNumber - 1) + "</td>");
                                sbDetails.Append("<td style='text-align: center;'>" + GetValueSafely(InsulationResult, core, specimenNumber - 1) + "</td>");

                                sbDetails.Append("</tr>");
                                specimenNumber++; // Increment specimen number for next specimen of this core


                            }
                        }
                        // Add the specified values row with dynamic data

                        sbDetails.Append("<tr>" +
                            "<td  colspan='2'  style='text-align: center; font-weight: bold;'>Specified</td>" +
                            "<td   style='text-align: center;'>" + InsulationSpecifiedTemperature + "</td>" +
                            "<td style='text-align: center;'>" + InsulationSpecifiedDuration + "</td>" +
                          "<td   style='text-align: center;'>" + InsulationSpecifiedBeforeHeating + "</td>" +
                            "<td style='text-align: center;'>" + InsulationSpecifiedAfterHeating + "</td>" +
                            "<td style='text-align: center;'>" + InsulationSpecifiedResult + "</td>" +
                        "</tr>");
                        sbDetails.Append("</table>");

                        // Add signature section
                        sbDetails.Append(@"<table style='width:100%; font-family:Times New Roman; font-size:11px' class='tab1'>
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
                    </table>");

                        // Generate the HTML file
                        body = objEmail.PrepareMailBodyWith("TestRecord_Shrinkage_Rev01.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord_Shrinkage_Rev01-" + SampleID + ".htm");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_Shrinkage_Rev01-" + SampleID + ".htm"));
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

        // Helper method to get values safely
        private string GetValueSafely(Dictionary<string, List<string>> data, string key, int index)
        {
            if (data != null && data.ContainsKey(key) && data[key] != null && index < data[key].Count)
                return data[key][index];
            return "";
        }

        // Helper method to count occurrences of a value in a list up to a given index
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