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
    public partial class GenerateTestRecord_ResistanceAgainstAcidAndAlkalineTest : System.Web.UI.Page
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
                        // Data storage organized according to your form structure
                        Dictionary<string, List<string>> measurementData = new Dictionary<string, List<string>>();

                        // Basic report header data
                        string Core = "", SpecimenType = "", BeforeDate = "", AfterDate = "";
                        string Type = "", Size = "", Voltage = "", Date = "", RoomNo = "";
                        string RoomTemp = "", RefSpec = "", TestMethodNo = "", JobOrderNo = "";
                        string WorkSheetNo = "", RoomHumidity = "", EquipmentNo = "";

                        DataTable dtRecordDetails = new DataTable();
                        StringBuilder sbHeader = new StringBuilder();
                        StringBuilder sbDetails = new StringBuilder();
                        StringBuilder sbFooter = new StringBuilder();
                        mailer objEmail = new mailer();

                        string body = ""; string str = ""; string filename = ""; string html = "";

                        dtRecordDetails = _cm.getTestRecord(ReportId, SampleID);

                        if (dtRecordDetails.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtRecordDetails.Rows)
                            {
                                string parameterID = dr["ReportParamMapID"].ToString();
                                string result = dr["Result"].ToString();
                                string paramName = dr["ReportParameter"]?.ToString() ?? "";

                                // Header Info - Based on your ASPX form structure
                                if (parameterID == "17503") { Type = result; }
                                else if (parameterID == "17504") { Size = result; }
                                else if (parameterID == "17505") { Voltage = result; }
                                else if (parameterID == "17506") { Date = result; }
                                else if (parameterID == "17507") { RoomNo = result; }
                                else if (parameterID == "17508") { RoomTemp = result; }
                                else if (parameterID == "17509") { RefSpec = result; }
                                else if (parameterID == "17510") { TestMethodNo = result; }
                                else if (parameterID == "17511") { JobOrderNo = result; }
                                else if (parameterID == "17512") { WorkSheetNo = result; }
                                else if (parameterID == "17513") { RoomHumidity = result; }
                                else if (parameterID == "17514") { EquipmentNo = result; }
                                else if (parameterID == "17517") { Core = result; }
                                else if (parameterID == "17518") { SpecimenType = result; }
                                else if (parameterID == "17547") { BeforeDate = result; }
                                else if (parameterID == "17548") { AfterDate = result; }
                                else
                                {
                                    // Measurement data - store all measurement parameters
                                    if (!measurementData.ContainsKey(parameterID))
                                        measurementData[parameterID] = new List<string>();
                                    measurementData[parameterID].Add(result);
                                }
                            }
                        }

                        // Build Header Table - Matching your ASPX form structure
                        sbHeader.Append(@"<table style='width: 100%; border-collapse: collapse; font-family: Arial, sans-serif; font-size: 12px; margin-bottom: 20px;'>
                                             <tr>
                                                 <td style='padding: 8px; border: 1px solid #000; font-weight: bold; width: 15%; background-color: #f2f2f2;'>Type</td>
                                                 <td style='padding: 8px; border: 1px solid #000; width: 35%;'>" + Type + @"</td>
                                                 <td style='padding: 8px; border: 1px solid #000; font-weight: bold; width: 15%; background-color: #f2f2f2;'>Ref.Spec.</td>
                                                 <td style='padding: 8px; border: 1px solid #000; width: 35%;'>" + RefSpec + @"</td>
                                             </tr>
                                             <tr>
                                                 <td style='padding: 8px; border: 1px solid #000; font-weight: bold; background-color: #f2f2f2;'>Size</td>
                                                 <td style='padding: 8px; border: 1px solid #000;'>" + Size + @"</td>
                                                 <td style='padding: 8px; border: 1px solid #000; font-weight: bold; background-color: #f2f2f2;'>Test Method No.</td>
                                                 <td style='padding: 8px; border: 1px solid #000;'>" + TestMethodNo + @"</td>
                                             </tr>
                                             <tr>
                                                 <td style='padding: 8px; border: 1px solid #000; font-weight: bold; background-color: #f2f2f2;'>Voltage</td>
                                                 <td style='padding: 8px; border: 1px solid #000;'>" + Voltage + @"</td>
                                                 <td style='padding: 8px; border: 1px solid #000; font-weight: bold; background-color: #f2f2f2;'>Job Order No.</td>
                                                 <td style='padding: 8px; border: 1px solid #000;'>" + JobOrderNo + @"</td>
                                             </tr>
                                             <tr>
                                                 <td style='padding: 8px; border: 1px solid #000; font-weight: bold; background-color: #f2f2f2;'>Date</td>
                                                 <td style='padding: 8px; border: 1px solid #000;'>" + Date + @"</td>
                                                 <td style='padding: 8px; border: 1px solid #000; font-weight: bold; background-color: #f2f2f2;'>Work Sheet No.</td>
                                                 <td style='padding: 8px; border: 1px solid #000;'>" + WorkSheetNo + @"</td>
                                             </tr>
                                             <tr>
                                                 <td style='padding: 8px; border: 1px solid #000; font-weight: bold; background-color: #f2f2f2;'>Room No.</td>
                                                 <td style='padding: 8px; border: 1px solid #000;'>" + RoomNo + @"</td>
                                                 <td style='padding: 8px; border: 1px solid #000; font-weight: bold; background-color: #f2f2f2;'>Room Humidity (%)</td>
                                                 <td style='padding: 8px; border: 1px solid #000;'>" + RoomHumidity + @"</td>
                                             </tr>
                                             <tr>
                                                 <td style='padding: 8px; border: 1px solid #000; font-weight: bold; background-color: #f2f2f2;'>Room Temp. (°C)</td>
                                                 <td style='padding: 8px; border: 1px solid #000;'>" + RoomTemp + @"</td>
                                                 <td style='padding: 8px; border: 1px solid #000; font-weight: bold; background-color: #f2f2f2;'>Equipment No.</td>
                                                 <td style='padding: 8px; border: 1px solid #000;'>" + EquipmentNo + @"</td>
                                             </tr>
                                         </table>");

                        // Build Main Test Details Table - Exactly matching the PDF and ASPX form structure
                        sbDetails.Append(@"<table style='width:100%; border-collapse: collapse; font-family: Arial, sans-serif; font-size: 11px;' border='1' cellpadding='8' cellspacing='0'>");

                        // Table Headers - Exactly as in ASPX form
                        sbDetails.Append(@"<thead>
                                            <tr style='background-color: #D3D3D3;'>
                                                <th rowspan='2' colspan='2' style='border: 1px solid #000; padding: 10px; text-align: center; vertical-align: middle; font-weight: bold;'>Item</th>
                                                <th rowspan='2' style='border: 1px solid #000; padding: 10px; text-align: center; vertical-align: middle; font-weight: bold;'>Unit</th>
                                                <th rowspan='2' style='border: 1px solid #000; padding: 10px; text-align: center; vertical-align: middle; font-weight: bold;'>Equipment No.</th>
                                                <th colspan='6' style='border: 1px solid #000; padding: 10px; text-align: center; font-weight: bold;'>Measured value</th>
                                                <th rowspan='2' style='border: 1px solid #000; padding: 10px; text-align: center; vertical-align: middle; font-weight: bold;'>Specified</th>
                                            </tr>
                                            <tr style='background-color: #D3D3D3;'>
                                                <th style='border: 1px solid #000; padding: 10px; text-align: center; font-weight: bold;'>1</th>
                                                <th style='border: 1px solid #000; padding: 10px; text-align: center; font-weight: bold;'>2</th>
                                                <th style='border: 1px solid #000; padding: 10px; text-align: center; font-weight: bold;'>3</th>
                                                <th style='border: 1px solid #000; padding: 10px; text-align: center; font-weight: bold;'>4</th>
                                                <th style='border: 1px solid #000; padding: 10px; text-align: center; font-weight: bold;'>5</th>
                                                <th style='border: 1px solid #000; padding: 10px; text-align: center; font-weight: bold;'>Median</th>
                                            </tr>
                                        </thead>
                                        <tbody>");

                        // Core and Specimen Type Section - Matching ASPX exactly
                        sbDetails.Append("<tr>");
                        sbDetails.Append($"<td colspan='2' style='border: 1px solid #000; padding: 8px; font-weight: bold; text-align: center;'><b>Core</b></td>");
                        sbDetails.Append($"<td colspan='9' style='border: 1px solid #000; padding: 8px; text-align: left;'>{Core}</td>");
                        sbDetails.Append("</tr>");

                        sbDetails.Append("<tr>");
                        sbDetails.Append($"<td colspan='2' style='border: 1px solid #000; padding: 8px; font-weight: bold; text-align: center;'><b>Specimen type</b></td>");
                        sbDetails.Append($"<td colspan='9' style='border: 1px solid #000; padding: 8px; text-align: left;'>{SpecimenType}</td>");
                        sbDetails.Append("</tr>");

                        // TS & EL Before Immersed In Oil Section - First row with section header
                        sbDetails.Append("<tr>");
                        sbDetails.Append($"<td rowspan='9' style='border: 1px solid #000; padding: 8px; text-align: center; vertical-align: middle; font-weight: bold; background-color: #e0e0e0;'>TS & EL<br />Before<br />Immersed In<br />Oil<br />Date: {BeforeDate}</td>");
                        // Room temp - first row of this section
                        AddSimpleRow(sbDetails, "Room temp.", "°C", measurementData, "17519", "", true);

                        // Continue with remaining rows (no rowspan cell)
                        AddMeasurementRow(sbDetails, "Thickness", "mm", measurementData, "17520", "17549");
                        AddMeasurementRow(sbDetails, "Width/Dia", "mm", measurementData, "17521", "17550");
                        AddMeasurementRow(sbDetails, "A", "mm²", measurementData, "17522", "17551");
                        AddMeasurementRow(sbDetails, "BF", "N", measurementData, "17523", "17552");
                        AddMeasurementRow(sbDetails, "TS", "N/mm²", measurementData, "17524", "17553", "Min.:");
                        AddMeasurementRow(sbDetails, "LO", "mm", measurementData, "17525", "17554");
                        AddMeasurementRow(sbDetails, "L1", "mm", measurementData, "17526", "17555");
                        AddMeasurementRow(sbDetails, "EL", "%", measurementData, "17527", "17556", "Min.:");

                        // Immersed In Oil Section
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td rowspan='5' style='border: 1px solid #000; padding: 8px; text-align: center; vertical-align: middle; font-weight: bold; background-color: #e0e0e0;'>Immersed In<br />Oil</td>");
                        // Temp - first row of this section
                        AddSimpleRow(sbDetails, "Temp.", "°C", measurementData, "17528", "", true);

                        // Time - H (without rowspan)
                        AddTimeRow(sbDetails, "Time", "H", measurementData, "17529", "");

                        // Time - Date
                        AddTimeDateRow(sbDetails, "Time", "Date", measurementData, "17531");

                        // Time - H (second H row)
                        AddTimeHRow(sbDetails, "H", measurementData, "17530");

                        // Conditioning Section
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td rowspan='5' style='border: 1px solid #000; padding: 8px; text-align: center; vertical-align: middle; font-weight: bold; background-color: #e0e0e0;'>Conditioning</td>");

                        // Room Temp - first row of this section
                        AddSimpleRow(sbDetails, "Room Temp.", "°C", measurementData, "17532", "", true);

                        // Time - H
                        AddTimeRow(sbDetails, "Time", "H", measurementData, "17533", "");

                        // Time - Date
                        AddTimeDateRow(sbDetails, "Time", "Date", measurementData, "17535");

                        // Time - H (second H row)
                        AddTimeHRow(sbDetails, "H", measurementData, "17534");

                        // TS & EL After Immersed In Oil Section
                        sbDetails.Append("<tr>");
                        sbDetails.Append($"<td rowspan='8' style='border: 1px solid #000; padding: 8px; text-align: center; vertical-align: middle; font-weight: bold; background-color: #e0e0e0;'>TS & EL<br />After<br />Immersed In<br />Oil<br />Date: {AfterDate}</td>");

                        // Thickness - first row of this section
                        AddMeasurementRow(sbDetails, "Thickness", "mm", measurementData, "17537", "17557", "", true);

                        // Continue with remaining rows
                        AddMeasurementRow(sbDetails, "Width/Dia", "mm", measurementData, "17538", "17558");
                        AddMeasurementRow(sbDetails, "A", "mm²", measurementData, "17539", "17559");
                        AddMeasurementRow(sbDetails, "BF", "N", measurementData, "17540", "17560");
                        AddMeasurementRow(sbDetails, "TS", "N/mm²", measurementData, "17541", "17561", "Min.:");
                        AddMeasurementRow(sbDetails, "LO", "mm", measurementData, "17542", "17562");
                        AddMeasurementRow(sbDetails, "L1", "mm", measurementData, "17543", "17563");
                        AddMeasurementRow(sbDetails, "EL", "%", measurementData, "17544", "17564", "Min.:");

                        // Variation Section
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td rowspan='2' style='border: 1px solid #000; padding: 8px; text-align: center; vertical-align: middle; font-weight: bold; background-color: #e0e0e0;'>Variation</td>");

                        // TS Variation
                        AddMeasurementRow(sbDetails, "TS", "%", measurementData, "17545", "17565", "Max.:", true);

                        // EL Variation
                        AddMeasurementRow(sbDetails, "EL", "%", measurementData, "17546", "17566", "Max.:");

                        sbDetails.Append("</tbody></table>");


                        // Build Footer - Matching your form structure
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
                        body = objEmail.PrepareMailBodyWith("TestRecord_ResistanceAgainstAcidAndAlkalineTest.html",
                            "{Header}", sbHeader.ToString(),
                            "{TestDetails}", sbDetails.ToString(),
                            "{Footer}", sbFooter.ToString());

                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord_ResistanceAgainstAcidAndAlkalineTest-" + SampleID + ".htm");
                        File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_ResistanceAgainstAcidAndAlkalineTest-" + SampleID + ".htm"));
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

        // Helper method for standard measurement rows (5 values + median + specified)
        private void AddMeasurementRow(StringBuilder sb, string item, string unit,
            Dictionary<string, List<string>> dataDict, string paramId, string medianId, string specifiedPrefix = "", bool isFirstInSection = false)
        {
            if (!isFirstInSection)
            {
                sb.Append("<tr>");
            }

            sb.Append($"<td style='border: 1px solid #000; padding: 8px; text-align: left;'>{item}</td>");
            sb.Append($"<td style='border: 1px solid #000; padding: 8px; text-align: center;'>{unit}</td>");

            string equipmentNo = "";
            string[] values = new string[5] { "", "", "", "", "" };
            string median = "";
            string specifiedValue = "";

            if (dataDict != null && dataDict.ContainsKey(paramId))
            {
                string data = dataDict[paramId].FirstOrDefault();
                if (!string.IsNullOrEmpty(data))
                {
                    if (data.Contains("|"))
                    {
                        string[] parts = data.Split('|');
                        if (parts.Length >= 1) equipmentNo = parts[0].Trim();
                        if (parts.Length >= 2) specifiedValue = parts[1].Trim();
                        if (parts.Length >= 3)
                        {
                            string[] valuesPart = parts[2].Split(',');
                            for (int i = 0; i < Math.Min(valuesPart.Length, 5); i++)
                            {
                                values[i] = valuesPart[i].Trim();
                            }
                        }
                    }
                    else
                    {
                        // Single value - treat as equipment number
                        equipmentNo = data.Trim();
                    }
                }
            }

            if (!string.IsNullOrEmpty(medianId) && dataDict != null && dataDict.ContainsKey(medianId))
            {
                median = dataDict[medianId].FirstOrDefault();
            }

            sb.Append($"<td style='border: 1px solid #000; padding: 8px; text-align: center;'>{equipmentNo}</td>");
            for (int i = 0; i < 5; i++)
            {
                sb.Append($"<td style='border: 1px solid #000; padding: 8px; text-align: center;'>{values[i]}</td>");
            }
            sb.Append($"<td style='border: 1px solid #000; padding: 8px; text-align: center;'>{median}</td>");
            sb.Append($"<td style='border: 1px solid #000; padding: 8px; text-align: center;'>{specifiedPrefix} {specifiedValue}</td>");
            sb.Append("</tr>");
        }

        // Helper method for simple single value rows (Room temp, Temp, etc.)
        private void AddSimpleRow(StringBuilder sb, string item, string unit,
            Dictionary<string, List<string>> dataDict, string paramId, string specified = "", bool isFirstInSection = false)
        {
            if (!isFirstInSection)
            {
                sb.Append("<tr>");
            }

            sb.Append($"<td style='border: 1px solid #000; padding: 8px; text-align: left;'>{item}</td>");
            sb.Append($"<td style='border: 1px solid #000; padding: 8px; text-align: center;'>{unit}</td>");

            string equipmentNo = "";
            string value = "";
            string specifiedValue = specified;

            if (dataDict != null && dataDict.ContainsKey(paramId))
            {
                string data = dataDict[paramId].FirstOrDefault();
                if (!string.IsNullOrEmpty(data))
                {
                    if (data.Contains("|"))
                    {
                        string[] parts = data.Split('|');
                        if (parts.Length >= 1) equipmentNo = parts[0].Trim();
                        if (parts.Length >= 2 && string.IsNullOrEmpty(specifiedValue)) specifiedValue = parts[1].Trim();
                        if (parts.Length >= 3) value = parts[2].Trim();
                    }
                    else
                    {
                        // Single value - treat as equipment number
                        equipmentNo = data.Trim();
                    }
                }
            }

            sb.Append($"<td style='border: 1px solid #000; padding: 8px; text-align: center;'>{equipmentNo}</td>");
            sb.Append($"<td colspan='6' style='border: 1px solid #000; padding: 8px; text-align: center;'>{value}</td>");
            sb.Append($"<td style='border: 1px solid #000; padding: 8px; text-align: center;'>{specifiedValue}</td>");
            sb.Append("</tr>");
        }

        // Helper method for Time H rows (2 values in columns)
        private void AddTimeRow(StringBuilder sb, string item, string unit,
            Dictionary<string, List<string>> dataDict, string paramId1, string paramId2)
        {
            sb.Append("<tr>");
            sb.Append($"<td rowspan='4' style='border: 1px solid #000; padding: 8px; text-align: left; vertical-align: middle;'>{item}</td>");
            sb.Append($"<td style='border: 1px solid #000; padding: 8px; text-align: center;'>{unit}</td>");

            string equipmentNo = "";
            string value1 = "", value2 = "";
            string specifiedValue = "";

            if (dataDict != null && dataDict.ContainsKey(paramId1))
            {
                string data = dataDict[paramId1].FirstOrDefault();
                if (!string.IsNullOrEmpty(data))
                {
                    if (data.Contains("|"))
                    {
                        string[] parts = data.Split('|');
                        if (parts.Length >= 1) equipmentNo = parts[0].Trim();
                        if (parts.Length >= 2) specifiedValue = parts[1].Trim();
                        if (parts.Length >= 3)
                        {
                            string[] values = parts[2].Split(',');
                            if (values.Length >= 1) value1 = values[0].Trim();
                            if (values.Length >= 2) value2 = values[1].Trim();
                        }
                    }
                    else
                    {
                        equipmentNo = data.Trim();
                    }
                }
            }

            sb.Append($"<td rowspan='3' style='border: 1px solid #000; padding: 8px; text-align: center; vertical-align: middle;'>{equipmentNo}</td>");
            sb.Append($"<td colspan='3' style='border: 1px solid #000; padding: 8px; text-align: center;'>{value1}</td>");
            sb.Append($"<td colspan='3' style='border: 1px solid #000; padding: 8px; text-align: center;'>{value2}</td>");
            sb.Append($"<td style='border: 1px solid #000; padding: 8px; text-align: center;'>{specifiedValue}</td>");
            sb.Append("</tr>");
        }

        // Helper method for Time Date rows
        private void AddTimeDateRow(StringBuilder sb, string item, string unit,
            Dictionary<string, List<string>> dataDict, string paramId)
        {
            sb.Append("<tr>");
            sb.Append($"<td rowspan='2' style='border: 1px solid #000; padding: 8px; text-align: center; vertical-align: middle;'>{unit}</td>");
            sb.Append($"<td colspan='3' style='border: 1px solid #000; padding: 8px; text-align: center;'>Start</td>");
            sb.Append($"<td colspan='3' style='border: 1px solid #000; padding: 8px; text-align: center;'>End</td>");

            string specifiedValue = "";
            if (dataDict != null && dataDict.ContainsKey(paramId))
            {
                string data = dataDict[paramId].FirstOrDefault();
                if (!string.IsNullOrEmpty(data) && data.Contains("|"))
                {
                    string[] parts = data.Split('|');
                    if (parts.Length >= 2) specifiedValue = parts[1].Trim();
                }
            }

            sb.Append($"<td style='border: 1px solid #000; padding: 8px; text-align: center;'>{specifiedValue}</td>");
            sb.Append("</tr>");

            sb.Append("<tr>");
            string startValue = "", endValue = "";
            if (dataDict != null && dataDict.ContainsKey(paramId))
            {
                string data = dataDict[paramId].FirstOrDefault();
                if (!string.IsNullOrEmpty(data) && data.Contains("|"))
                {
                    string[] parts = data.Split('|');
                    if (parts.Length >= 3)
                    {
                        string[] values = parts[2].Split(',');
                        if (values.Length >= 1) startValue = values[0].Trim();
                        if (values.Length >= 2) endValue = values[1].Trim();
                    }
                }
            }

            sb.Append($"<td colspan='3' style='border: 1px solid #000; padding: 8px; text-align: center;'>{startValue}</td>");
            sb.Append($"<td colspan='3' style='border: 1px solid #000; padding: 8px; text-align: center;'>{endValue}</td>");

            // Get specified value again for this row
            string specifiedValue2 = "";
            if (dataDict != null && dataDict.ContainsKey(paramId))
            {
                string data = dataDict[paramId].FirstOrDefault();
                if (!string.IsNullOrEmpty(data) && data.Contains("|"))
                {
                    string[] parts = data.Split('|');
                    if (parts.Length >= 2) specifiedValue2 = parts[1].Trim();
                }
            }

            sb.Append($"<td style='border: 1px solid #000; padding: 8px; text-align: center;'>{specifiedValue2}</td>");
            sb.Append("</tr>");
        }

        // Helper method for Time H (second H row)
        private void AddTimeHRow(StringBuilder sb, string unit,
            Dictionary<string, List<string>> dataDict, string paramId)
        {
            sb.Append("<tr>");
            sb.Append($"<td style='border: 1px solid #000; padding: 8px; text-align: center;'>{unit}</td>");

            string equipmentNo = "";
            string value1 = "", value2 = "";
            string specifiedValue = "";

            if (dataDict != null && dataDict.ContainsKey(paramId))
            {
                string data = dataDict[paramId].FirstOrDefault();
                if (!string.IsNullOrEmpty(data))
                {
                    if (data.Contains("|"))
                    {
                        string[] parts = data.Split('|');
                        if (parts.Length >= 1) equipmentNo = parts[0].Trim();
                        if (parts.Length >= 2) specifiedValue = parts[1].Trim();
                        if (parts.Length >= 3)
                        {
                            string[] values = parts[2].Split(',');
                            if (values.Length >= 1) value1 = values[0].Trim();
                            if (values.Length >= 2) value2 = values[1].Trim();
                        }
                    }
                    else
                    {
                        equipmentNo = data.Trim();
                    }
                }
            }

            sb.Append($"<td style='border: 1px solid #000; padding: 8px; text-align: center;'>{equipmentNo}</td>");
            sb.Append($"<td colspan='3' style='border: 1px solid #000; padding: 8px; text-align: center;'>{value1}</td>");
            sb.Append($"<td colspan='3' style='border: 1px solid #000; padding: 8px; text-align: center;'>{value2}</td>");
            sb.Append($"<td style='border: 1px solid #000; padding: 8px; text-align: center;'>{specifiedValue}</td>");
            sb.Append("</tr>");
        }


    }
}