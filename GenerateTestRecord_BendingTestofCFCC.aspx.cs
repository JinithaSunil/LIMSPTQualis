using LIMS.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace LIMS
{
    public partial class GenerateTestRecord_BendingTestofCFCC : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
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
                        Dictionary<string, List<string>> Diametercore = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Crosssectionalarea = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Diameterofmandrel = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Bendingposition = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Loadoftension = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> DurationofLoad = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Resulttest = new Dictionary<string, List<string>>();



                        // Basic report data
                        string Type = "";
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

                                        if (reportParamMapID == "12583") // Diameter core
                                        {
                                            if (!Diametercore.ContainsKey(paramName))
                                                Diametercore[paramName] = new List<string>();
                                            Diametercore[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "12592") // Cross sectional area
                                        {
                                            if (!Crosssectionalarea.ContainsKey(paramName))
                                                Crosssectionalarea[paramName] = new List<string>();
                                            Crosssectionalarea[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "12584") // Diameter of mandrel
                                        {
                                            if (!Diameterofmandrel.ContainsKey(paramName))
                                                Diameterofmandrel[paramName] = new List<string>();
                                            Diameterofmandrel[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "12585") // Bending position
                                        {
                                            if (!Bendingposition.ContainsKey(paramName))
                                                Bendingposition[paramName] = new List<string>();
                                            Bendingposition[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "12586") // Load of tension 7.5 % RTS
                                        {
                                            if (!Loadoftension.ContainsKey(paramName))
                                                Loadoftension[paramName] = new List<string>();
                                            Loadoftension[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "12587") // Duration of Load
                                        {
                                            if (!DurationofLoad.ContainsKey(paramName))
                                                DurationofLoad[paramName] = new List<string>();
                                            DurationofLoad[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "12722") // Result test
                                        {
                                            if (!Resulttest.ContainsKey(paramName))
                                                Resulttest[paramName] = new List<string>();
                                            Resulttest[paramName].Add(formattedData);
                                        }
                                    }
                                }
                                else
                                {
                                    if (reportParamMapID == "12704") { Type = result; }
                                    else if (reportParamMapID == "12705") { Size = result; }
                                    else if (reportParamMapID == "12706") { Voltage = result; }
                                    else if (reportParamMapID == "12710") { RefSpec = result; }
                                    else if (reportParamMapID == "12711") { TestMethodNo = result; }
                                    else if (reportParamMapID == "12712") { JobOrderNo = result; }
                                    else if (reportParamMapID == "12713") { WorkSheetNo = result; }
                                    else if (reportParamMapID == "12708") { RoomNo = result; }
                                    else if (reportParamMapID == "12714") { RoomHumidity = result; }
                                    else if (reportParamMapID == "12709") { RoomTemp = result; }
                                    else if (reportParamMapID == "12707") { Date = result; }
                                    else if (reportParamMapID == "12715") { EquipmentNo = result; }

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
                                 <td>Grade</td>
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
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>Item test</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>Unit</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>Specified</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>Value test</strong></td>");
                        sbDetails.Append("</tr>");


                        int RowNumber = 1;

                        // Group 1: Diametercore 
                        if (Diametercore.Count > 0)
                        {
                            foreach (var entry in Diametercore)
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
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Diameter core</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{unit}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{specified}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{valueTest}</td>");
                                    sbDetails.Append("</tr>");
                                    RowNumber++;
                                }
                                else
                                {
                                    // Empty data row
                                    sbDetails.Append("<tr>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{RowNumber}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Diameter core</td>");
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
                            sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Diameter core</td>");
                            sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        //  Crosssectionalarea
                        if (Crosssectionalarea.Count > 0)
                        {
                            foreach (var entry in Crosssectionalarea)
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
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Cross sectional area</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{unit}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{specified}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{valueTest}</td>");
                                    sbDetails.Append("</tr>");
                                    RowNumber++;
                                }
                                else
                                {
                                    sbDetails.Append("<tr>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{RowNumber}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Cross sectional area</td>");
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
                            sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Cross sectional area</td>");
                            sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // Group: Diameter of mandrel
                        if (Diameterofmandrel.Count > 0)
                        {
                            foreach (var entry in Diameterofmandrel)
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
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Diameter of mandrel</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{unit}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{specified}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{valueTest}</td>");
                                    sbDetails.Append("</tr>");
                                    RowNumber++;
                                }
                                else
                                {
                                    sbDetails.Append("<tr>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{RowNumber}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Diameter of mandrel</td>");
                                    sbDetails.Append("<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                                    sbDetails.Append("</tr>");
                                    RowNumber++;
                                }
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{RowNumber}</td>");
                            sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Diameter of mandrel</td>");
                            sbDetails.Append("<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // Group: Bending position
                        if (Bendingposition.Count > 0)
                        {
                            foreach (var entry in Bendingposition)
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
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Bending position</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{unit}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{specified}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{valueTest}</td>");
                                    sbDetails.Append("</tr>");
                                    RowNumber++;
                                }
                                else
                                {
                                    sbDetails.Append("<tr>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{RowNumber}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Bending position</td>");
                                    sbDetails.Append("<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                                    sbDetails.Append("</tr>");
                                    RowNumber++;
                                }
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{RowNumber}</td>");
                            sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Bending position</td>");
                            sbDetails.Append("<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // Group: Load of tension 7.5 % RTS
                        if (Loadoftension.Count > 0)
                        {
                            foreach (var entry in Loadoftension)
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
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Load of tension 7.5 % RTS</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{unit}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{specified}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{valueTest}</td>");
                                    sbDetails.Append("</tr>");
                                    RowNumber++;
                                }
                                else
                                {
                                    sbDetails.Append("<tr>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{RowNumber}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Load of tension 7.5 % RTS</td>");
                                    sbDetails.Append("<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                                    sbDetails.Append("</tr>");
                                    RowNumber++;
                                }
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{RowNumber}</td>");
                            sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Load of tension 7.5 % RTS</td>");
                            sbDetails.Append("<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // Group: Duration of Load
                        if (DurationofLoad.Count > 0)
                        {
                            foreach (var entry in DurationofLoad)
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
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Duration of Load</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{unit}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{specified}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{valueTest}</td>");
                                    sbDetails.Append("</tr>");
                                    RowNumber++;
                                }
                                else
                                {
                                    sbDetails.Append("<tr>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{RowNumber}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Duration of Load</td>");
                                    sbDetails.Append("<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                                    sbDetails.Append("</tr>");
                                    RowNumber++;
                                }
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{RowNumber}</td>");
                            sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Duration of Load</td>");
                            sbDetails.Append("<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }


                        // Resulttest
                        if (Resulttest.Count > 0)
                        {
                            foreach (var entry in Resulttest)
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
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Result test</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{unit}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{specified}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{valueTest}</td>");
                                    sbDetails.Append("</tr>");
                                    RowNumber++;
                                }
                                else
                                {
                                    sbDetails.Append("<tr>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{RowNumber}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Result test</td>");
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
                            sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Result test</td>");
                            sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
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
                        body = objEmail.PrepareMailBodyWith("TestRecord_BendingTestofCFCC.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord_BendingTestofCFCC -" + SampleID + ".htm");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_BendingTestofCFCC -" + SampleID + ".htm"));
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