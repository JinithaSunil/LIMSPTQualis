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
    public partial class SingleFlameRetardant_GR : System.Web.UI.Page
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
                        Dictionary<string, List<string>> d1 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> d2 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> d3 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> d4 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> d5 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> d6 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> d7 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> d8 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> d9 = new Dictionary<string, List<string>>();

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

                                if (result.Contains("|"))
                                {
                                    string[] parts = result.Split('|');

                                    if (parts.Length >= 3)
                                    {
                                        string unit = parts[0].Trim();
                                        string specified = parts[1].Trim();
                                        string valueTest = parts[2].Trim();
                                        string formattedData = $"{unit}|{specified}|{valueTest}";

                                        if (reportParamMapID == "13023")
                                        {
                                            if (!d1.ContainsKey(paramName))
                                                d1[paramName] = new List<string>();
                                            d1[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "13024")
                                        {
                                            if (!d2.ContainsKey(paramName))
                                                d2[paramName] = new List<string>();
                                            d2[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "13025")
                                        {
                                            if (!d3.ContainsKey(paramName))
                                                d3[paramName] = new List<string>();
                                            d3[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "13026")
                                        {
                                            if (!d4.ContainsKey(paramName))
                                                d4[paramName] = new List<string>();
                                            d4[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "13027")
                                        {
                                            if (!d5.ContainsKey(paramName))
                                                d5[paramName] = new List<string>();
                                            d5[paramName].Add(formattedData);
                                        }                                        
                                        else if (reportParamMapID == "13028")
                                        {
                                            if (!d7.ContainsKey(paramName))
                                                d7[paramName] = new List<string>();
                                            d7[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "13029")
                                        {
                                            if (!d8.ContainsKey(paramName))
                                                d8[paramName] = new List<string>();
                                            d8[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "13030")
                                        {
                                            if (!d9.ContainsKey(paramName))
                                                d9[paramName] = new List<string>();
                                            d9[paramName].Add(formattedData);
                                        }
                                        else
                                        {
                                            if (!d6.ContainsKey(paramName))
                                                d6[paramName] = new List<string>();
                                            d6[paramName].Add(formattedData);
                                        }

                                    }
                                }
                                else // Non-measurement data (simple key-value pairs)
                                {
                                    if (reportParamMapID == "13009") { Type = result; }
                                    else if (reportParamMapID == "13010") { Size = result; }
                                    else if (reportParamMapID == "13011") { Voltage = result; }
                                    else if (reportParamMapID == "13012") { Date = result; }
                                    else if (reportParamMapID == "13013") { RoomNo = result; }
                                    else if (reportParamMapID == "13014") { RoomTemp = result; }
                                    else if (reportParamMapID == "13015") { RefSpec = result; }
                                    else if (reportParamMapID == "13016") { TestMethodNo = result; }
                                    else if (reportParamMapID == "13017") { JobOrderNo = result; }
                                    else if (reportParamMapID == "13018") { WorkSheetNo = result; }
                                    else if (reportParamMapID == "13019") { RoomHumidity = result; }
                                    else if (reportParamMapID == "13020") { EquipmentNo = result; }
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
                        sbDetails.Append(@"<table class='tab1' border='1' cellpadding='3' cellspacing='0' style='width:100%; border-collapse: collapse;'>");

                        // Header row
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>No</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe;'><strong>Item test</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>Unit</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe;'><strong>Specified Value</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>Measured</strong></td>");
                        sbDetails.Append("</tr>");

                        int RowNumber = 1;

                        // Length of cable
                        if (d1.Count > 0)
                        {
                            foreach (var entry in d1)
                            {
                                var dataValues = entry.Value;
                                if (dataValues.Count > 0)
                                {
                                    string[] parts = dataValues[0].Split('|');
                                    string unit = parts.Length > 0 ? parts[0] : "";
                                    string specified = parts.Length > 1 ? parts[1] : "-";
                                    string valueTest = parts.Length > 2 ? parts[2] : "";

                                    sbDetails.Append("<tr>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{RowNumber}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Length of cable</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{unit}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:left; padding:5px;'>{specified}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{valueTest}</td>");
                                    sbDetails.Append("</tr>");
                                    RowNumber++;
                                }
                                else
                                {// Empty data row
                                    sbDetails.Append("<tr>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{RowNumber}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Length of cable</td>");
                                    sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                                    sbDetails.Append("</tr>");
                                    RowNumber++;
                                }
                            }
                        }
                        else
                        {
                            // No data - still add row with test name and empty values
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{RowNumber}</td>");
                            sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Length of cable</td>");
                            sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // Diameter of cable
                        if (d2.Count > 0)
                        {
                            foreach (var entry in d2)
                            {
                                var dataValues = entry.Value;
                                if (dataValues.Count > 0)
                                {
                                    string[] parts = dataValues[0].Split('|');
                                    string unit = parts.Length > 0 ? parts[0] : "";
                                    string specified = parts.Length > 1 ? parts[1] : "-";
                                    string valueTest = parts.Length > 2 ? parts[2] : "";

                                    sbDetails.Append("<tr>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{RowNumber}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Diameter of cable</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{unit}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:left; padding:5px;'>{specified}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{valueTest}</td>");
                                    sbDetails.Append("</tr>");
                                    RowNumber++;
                                }
                                else
                                {
                                    sbDetails.Append("<tr>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{RowNumber}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Diameter of cable</td>");
                                    sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                                    sbDetails.Append("</tr>");
                                    RowNumber++;
                                }
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{RowNumber}</td>");
                            sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Diameter of cable</td>");
                            sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // Duration
                        if (d3.Count > 0)
                        {
                            foreach (var entry in d3)
                            {
                                var dataValues = entry.Value;
                                if (dataValues.Count > 0)
                                {
                                    string[] parts = dataValues[0].Split('|');
                                    string unit = parts.Length > 0 ? parts[0] : "";
                                    string specified = parts.Length > 1 ? parts[1] : "-";
                                    string valueTest = parts.Length > 2 ? parts[2] : "";

                                    sbDetails.Append("<tr>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{RowNumber}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Duration</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{unit}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:left; padding:5px;'>{specified}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{valueTest}</td>");
                                    sbDetails.Append("</tr>");
                                    RowNumber++;
                                }
                                else
                                {
                                    sbDetails.Append("<tr>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{RowNumber}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Duration</td>");
                                    sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                                    sbDetails.Append("</tr>");
                                    RowNumber++;
                                }
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{RowNumber}</td>");
                            sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Duration</td>");
                            sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // Temperature // 
                        if (d4.Count > 0)
                        {
                            foreach (var entry in d4)
                            {
                                var dataValues = entry.Value;
                                if (dataValues.Count > 0)
                                {
                                    string[] parts = dataValues[0].Split('|');
                                    string unit = parts.Length > 0 ? parts[0] : "";
                                    string specified = parts.Length > 1 ? parts[1] : "-";
                                    string valueTest = parts.Length > 2 ? parts[2] : "";

                                    sbDetails.Append("<tr>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{RowNumber}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Temperature</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{unit}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:left; padding:5px;'>{specified}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{valueTest}</td>");
                                    sbDetails.Append("</tr>");
                                    RowNumber++;
                                }
                                else
                                {
                                    sbDetails.Append("<tr>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{RowNumber}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Temperature</td>");
                                    sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                                    sbDetails.Append("</tr>");
                                    RowNumber++;
                                }
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{RowNumber}</td>");
                            sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Temperature</td>");
                            sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // Time for Reaching Burn Temperature
                        if (d5.Count > 0)
                        {
                            foreach (var entry in d5)
                            {
                                var dataValues = entry.Value;
                                if (dataValues.Count > 0)
                                {
                                    string[] parts = dataValues[0].Split('|');
                                    string unit = parts.Length > 0 ? parts[0] : "";
                                    string specified = parts.Length > 1 ? parts[1] : "-";
                                    string valueTest = parts.Length > 2 ? parts[2] : "";

                                    sbDetails.Append("<tr>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{RowNumber}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Time for Reaching Burn Temperature</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{unit}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:left; padding:5px;'>{specified}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{valueTest}</td>");
                                    sbDetails.Append("</tr>");
                                    RowNumber++;
                                }
                                else
                                {
                                    sbDetails.Append("<tr>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{RowNumber}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Time for Reaching Burn Temperature</td>");
                                    sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                                    sbDetails.Append("</tr>");
                                    RowNumber++;
                                }
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{RowNumber}</td>");
                            sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Time for Reaching Burn Temperature</td>");
                            sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }


                        // Flame test result //
                        if (d6.Count > 0)
                        {
                            foreach (var entry in d6)
                            {
                                var dataValues = entry.Value;
                                if (dataValues.Count > 0)
                                {
                                    string[] parts = dataValues[0].Split('|');
                                    string unit = parts.Length > 0 ? parts[0] : "";
                                    string specified = parts.Length > 1 ? parts[1] : "-";
                                    string valueTest = parts.Length > 2 ? parts[2] : "";

                                    sbDetails.Append("<tr>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{RowNumber}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Flame test result</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{unit}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:left; padding:5px;'>{specified}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{valueTest}</td>");
                                    sbDetails.Append("</tr>");
                                    RowNumber++;
                                }
                                else
                                {
                                    sbDetails.Append("<tr>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>6</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Flame test result</td>");
                                    sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                                    sbDetails.Append("</tr>");
                                }
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>6</td>");
                            sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Flame test result</td>");
                            sbDetails.Append($"<td colspan='3' style='border:1px solid black;'> </td>");
                            sbDetails.Append("</tr>");
                        }


                        // Process d7
                        if (d7.Count > 0)
                        {
                            foreach (var entry in d7)
                            {
                                var dataValues = entry.Value;
                                if (dataValues.Count > 0)
                                {
                                    string[] parts = dataValues[0].Split('|');
                                    string unit = parts.Length > 0 ? parts[0] : "";
                                    string specified = parts.Length > 1 ? parts[1] : "-";
                                    string valueTest = parts.Length > 2 ? parts[2] : "";

                                    sbDetails.Append("<tr>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>6.1</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>The distance from lower edge of the top support to the upper onset of charring</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{unit}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:left; padding:5px;'>{specified}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{valueTest}</td>");
                                    sbDetails.Append("</tr>");
                                }
                                else
                                {
                                    sbDetails.Append("<tr>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>6.1</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>The distance from lower edge of the top support to the upper onset of charring</td>");
                                    sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                                    sbDetails.Append("</tr>");
                                }
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>6.1</td>");
                            sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>The distance from lower edge of the top support to the upper onset of charring</td>");
                            sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                            sbDetails.Append("</tr>");
                        }


                        // Process d8
                        if (d8.Count > 0)
                        {
                            foreach (var entry in d8)
                            {
                                var dataValues = entry.Value;
                                if (dataValues.Count > 0)
                                {
                                    string[] parts = dataValues[0].Split('|');
                                    string unit = parts.Length > 0 ? parts[0] : "";
                                    string specified = parts.Length > 1 ? parts[1] : "-";
                                    string valueTest = parts.Length > 2 ? parts[2] : "";

                                    sbDetails.Append("<tr>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>6.2</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>The distance from the lower edge of the top support to the lower onset of charring</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{unit}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:left; padding:5px;'>{specified}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{valueTest}</td>");
                                    sbDetails.Append("</tr>");
                                }
                                else
                                {
                                    sbDetails.Append("<tr>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>6.2</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>The distance from the lower edge of the top support to the lower onset of charring</td>");
                                    sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                                    sbDetails.Append("</tr>");
                                }
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>6.2</td>");
                            sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>The distance from the lower edge of the top support to the lower onset of charring</td>");
                            sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }


                        if (d9.Count > 0)
                        {
                            foreach (var entry in d9)
                            {
                                var dataValues = entry.Value;
                                if (dataValues.Count > 0)
                                {
                                    string[] parts = dataValues[0].Split('|');
                                    string unit = parts.Length > 0 ? parts[0] : "";
                                    string specified = parts.Length > 1 ? parts[1] : "-";
                                    string valueTest = parts.Length > 2 ? parts[2] : "";

                                    sbDetails.Append("<tr>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>6.3</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>After burning.................</td>");  // Update label as needed
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{unit}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:left; padding:5px;'>{specified}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{valueTest}</td>");
                                    sbDetails.Append("</tr>");                                    
                                }
                                else
                                {
                                    sbDetails.Append("<tr>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>6.3</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>After burning.................</td>");
                                    sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                                    sbDetails.Append("</tr>");
                                }
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>6.3</td>");
                            sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>After burning.................</td>");
                            sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                            sbDetails.Append("</tr>");
                        }




                        sbDetails.Append("</tbody></table>");

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

                        sbDetails.Append("</table>");

                        // Generate the HTML file
                        body = objEmail.PrepareMailBodyWith("SingleFlameRetardant_GR.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/SingleFlameRetardant_GR-" + SampleID + ".htm");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/SingleFlameRetardant_GR-" + SampleID + ".htm"));
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