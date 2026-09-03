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
    public partial class GenerateTestRecord_DimensionCFCC : System.Web.UI.Page
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
                        Dictionary<string, List<string>> ThicknessGalvanic = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> ThicknessGalvanicMinMax = new Dictionary<string, List<string>>();

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

                                        // Thickness of galvanic measurement values 
                                        if (reportParamMapID == "12580") // Thickness of galvanic
                                        {
                                            if (!ThicknessGalvanic.ContainsKey(reportParamMapID))
                                                ThicknessGalvanic[reportParamMapID] = new List<string>();
                                            ThicknessGalvanic[reportParamMapID].Add(formattedData);
                                        }
                                    }
                                }
                                else
                                {
                                    if (reportParamMapID == "12581") // Min.
                                    {
                                        if (!ThicknessGalvanicMinMax.ContainsKey(reportParamMapID))
                                            ThicknessGalvanicMinMax[reportParamMapID] = new List<string>();
                                        ThicknessGalvanicMinMax[reportParamMapID].Add(result);
                                    }
                                    if (reportParamMapID == "12582") // Max
                                    {
                                        if (!ThicknessGalvanicMinMax.ContainsKey(reportParamMapID))
                                            ThicknessGalvanicMinMax[reportParamMapID] = new List<string>();
                                        ThicknessGalvanicMinMax[reportParamMapID].Add(result);
                                    }

                                    // Cable properties (simple values) - Fixed parameter IDs
                                    else if (reportParamMapID == "12859") { Type = result; }             // Cable Type
                                    else if (reportParamMapID == "12865") { RefSpec = result; }          // Reference Specification
                                    else if (reportParamMapID == "12860") { Size = result; }             // Cable Size
                                    else if (reportParamMapID == "12866") { TestMethodNo = result; }     // Test Method No
                                    else if (reportParamMapID == "12861") { Voltage = result; }          // Voltage
                                    else if (reportParamMapID == "12867") { JobOrderNo = result; }       // Job Order No
                                    else if (reportParamMapID == "12868") { WorkSheetNo = result; }      // Worksheet No
                                    else if (reportParamMapID == "12863") { RoomNo = result; }           // Room No
                                    else if (reportParamMapID == "12869") { RoomHumidity = result; }     // Room Humidity
                                    else if (reportParamMapID == "12864") { RoomTemp = result; }         // Room Temperature
                                    else if (reportParamMapID == "12862") { Date = result; }             // Date
                                    else if (reportParamMapID == "12870") { EquipmentNo = result; }      // Equipment No
                                }
                            }
                        }

                        // Build header table
                        sbHeader.Append(@"<table style='width: 100%; border: 1px solid black; margin-top: 10px; border-collapse: collapse; font-family: Arial, sans-serif; font-size: 12px;' class='tab1'>
                        <tr>
                            <td style='border: 1px solid black; padding: 5px; width: 15%;'><strong>Type</strong></td>
                            <td style='border: 1px solid black; padding: 5px; width: 35%;'>" + Type + @"</td>
                            <td style='border: 1px solid black; padding: 5px; width: 15%;'><strong>Ref. Spec.</strong></td>
                            <td style='border: 1px solid black; padding: 5px; width: 35%;'>" + RefSpec + @"</td>
                        </tr>
                        <tr>
                            <td style='border: 1px solid black; padding: 5px;'><strong>Size</strong></td>
                            <td style='border: 1px solid black; padding: 5px;'>" + Size + @"</td>
                            <td style='border: 1px solid black; padding: 5px;'><strong>Test Method No.</strong></td>
                            <td style='border: 1px solid black; padding: 5px;'>" + TestMethodNo + @"</td>
                        </tr>
                        <tr>
                            <td style='border: 1px solid black; padding: 5px;'><strong>Voltage</strong></td>
                            <td style='border: 1px solid black; padding: 5px;'>" + Voltage + @"</td>
                            <td style='border: 1px solid black; padding: 5px;'><strong>Job Order No.</strong></td>
                            <td style='border: 1px solid black; padding: 5px;'>" + JobOrderNo + @"</td>
                        </tr>
                        <tr>
                            <td style='border: 1px solid black; padding: 5px;'><strong>Date</strong></td>
                            <td style='border: 1px solid black; padding: 5px;'>" + Date + @"</td>
                            <td style='border: 1px solid black; padding: 5px;'><strong>Work Sheet No.</strong></td>
                            <td style='border: 1px solid black; padding: 5px;'>" + WorkSheetNo + @"</td>
                        </tr>
                        <tr>
                            <td style='border: 1px solid black; padding: 5px;'><strong>Room No.</strong></td>
                            <td style='border: 1px solid black; padding: 5px;'>" + RoomNo + @"</td>
                            <td style='border: 1px solid black; padding: 5px;'><strong>Room Humidity</strong></td>
                            <td style='border: 1px solid black; padding: 5px;'>" + RoomHumidity + "%" + @"</td>
                        </tr>
                        <tr>
                            <td style='border: 1px solid black; padding: 5px;'><strong>Room Temp.</strong></td>
                            <td style='border: 1px solid black; padding: 5px;'>" + RoomTemp + "°C" + @"</td>
                            <td style='border: 1px solid black; padding: 5px;'><strong>Equipment No.</strong></td>
                            <td style='border: 1px solid black; padding: 5px;'>" + EquipmentNo + @"</td>
                        </tr>
                    </table>");

                        // Build test details table
                        sbDetails.Append(@"<table style='width:100%; border: 1px solid black; border-collapse: collapse; font-family: Arial, sans-serif; font-size: 12px; margin-top: 15px;' class='tab1'>");

                        // Header row 1
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#d3d3d3; text-align:center; border: 1px solid black; padding: 5px; width: 40px;'><strong>No</strong></td>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#d3d3d3; text-align:center; border: 1px solid black; padding: 5px; width: 120px;'><strong>Item test</strong></td>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#d3d3d3; text-align:center; border: 1px solid black; padding: 5px; width: 50px;'><strong>unit</strong></td>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#d3d3d3; text-align:center; border: 1px solid black; padding: 5px; width: 80px;'><strong>Specified</strong></td>");
                        sbDetails.Append("<td colspan='8' style='background-color:#d3d3d3; text-align:center; border: 1px solid black; padding: 5px;'><strong>Value test</strong></td>");
                        sbDetails.Append("</tr>");

                        // Header row 2
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td style='background-color:#d3d3d3; text-align:center; border: 1px solid black; padding: 5px; width: 50px;'><strong>1</strong></td>");
                        sbDetails.Append("<td style='background-color:#d3d3d3; text-align:center; border: 1px solid black; padding: 5px; width: 50px;'><strong>2</strong></td>");
                        sbDetails.Append("<td style='background-color:#d3d3d3; text-align:center; border: 1px solid black; padding: 5px; width: 50px;'><strong>3</strong></td>");
                        sbDetails.Append("<td style='background-color:#d3d3d3; text-align:center; border: 1px solid black; padding: 5px; width: 50px;'><strong>4</strong></td>");
                        sbDetails.Append("<td style='background-color:#d3d3d3; text-align:center; border: 1px solid black; padding: 5px; width: 50px;'><strong>5</strong></td>");
                        sbDetails.Append("<td style='background-color:#d3d3d3; text-align:center; border: 1px solid black; padding: 5px; width: 50px;'><strong>6</strong></td>");
                        sbDetails.Append("<td style='background-color:#d3d3d3; text-align:center; border: 1px solid black; padding: 5px; width: 50px;'><strong>Min:</strong></td>");
                        sbDetails.Append("<td style='background-color:#d3d3d3; text-align:center; border: 1px solid black; padding: 5px; width: 50px;'><strong>Max:</strong></td>");
                        sbDetails.Append("</tr>");

                        int RowNumber = 1;

                        // Thickness of galvanic data row
                        if (ThicknessGalvanic.ContainsKey("12580"))
                        {
                            var dataValues = ThicknessGalvanic["12580"];
                            string minValue = ThicknessGalvanicMinMax.ContainsKey("12581") ? ThicknessGalvanicMinMax["12581"].FirstOrDefault() : "";
                            string maxValue = ThicknessGalvanicMinMax.ContainsKey("12582") ? ThicknessGalvanicMinMax["12582"].FirstOrDefault() : "";

                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "", "", "", "", "" };

                                // Ensure we have 6 values
                                while (inputParts.Length < 6)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");
                                sbDetails.Append($"<td style='text-align:center; border: 1px solid black; padding: 5px;'>{RowNumber}</td>");
                                sbDetails.Append("<td style='border: 1px solid black; padding: 5px;'>Thickness of galvanic</td>");
                                sbDetails.Append($"<td style='text-align:center; border: 1px solid black; padding: 5px;'>{unit}</td>");
                                sbDetails.Append($"<td style='text-align:center; border: 1px solid black; padding: 5px;'>{specified}</td>");
                                sbDetails.Append($"<td style='text-align:center; border: 1px solid black; padding: 5px;'>{inputParts[0]}</td>");
                                sbDetails.Append($"<td style='text-align:center; border: 1px solid black; padding: 5px;'>{inputParts[1]}</td>");
                                sbDetails.Append($"<td style='text-align:center; border: 1px solid black; padding: 5px;'>{inputParts[2]}</td>");
                                sbDetails.Append($"<td style='text-align:center; border: 1px solid black; padding: 5px;'>{inputParts[3]}</td>");
                                sbDetails.Append($"<td style='text-align:center; border: 1px solid black; padding: 5px;'>{inputParts[4]}</td>");
                                sbDetails.Append($"<td style='text-align:center; border: 1px solid black; padding: 5px;'>{inputParts[5]}</td>");
                                sbDetails.Append($"<td style='text-align:center; border: 1px solid black; padding: 5px;'>{minValue}</td>");
                                sbDetails.Append($"<td style='text-align:center; border: 1px solid black; padding: 5px;'>{maxValue}</td>");
                                sbDetails.Append("</tr>");
                                RowNumber++;
                            }
                        }
                        else
                        {
                            // Empty row if no data
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td style='text-align:center; border: 1px solid black; padding: 5px;'>{RowNumber}</td>");
                            sbDetails.Append("<td style='border: 1px solid black; padding: 5px;'>Thickness of galvanic</td>");
                            sbDetails.Append("<td style='border: 1px solid black; padding: 5px;'></td>");
                            sbDetails.Append("<td style='border: 1px solid black; padding: 5px;'></td>");
                            sbDetails.Append("<td style='border: 1px solid black; padding: 5px;'></td>");
                            sbDetails.Append("<td style='border: 1px solid black; padding: 5px;'></td>");
                            sbDetails.Append("<td style='border: 1px solid black; padding: 5px;'></td>");
                            sbDetails.Append("<td style='border: 1px solid black; padding: 5px;'></td>");
                            sbDetails.Append("<td style='border: 1px solid black; padding: 5px;'></td>");
                            sbDetails.Append("<td style='border: 1px solid black; padding: 5px;'></td>");
                            sbDetails.Append("<td style='border: 1px solid black; padding: 5px;'></td>");
                            sbDetails.Append("<td style='border: 1px solid black; padding: 5px;'></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        sbDetails.Append("</table>");

                        // Footer table
                        sbFooter.Append(@"<table style='width:100%; font-family: Arial, sans-serif; font-size: 12px; margin-top: 20px; border: 1px solid black; border-collapse: collapse;' class='tab1'>
                       <tr>
                           <td style='font-weight: bold; vertical-align: middle; border: 1px solid black; padding: 8px;' rowspan='3'><strong>PT Qualis Indonesia</strong></td>
                           <td colspan='3' style='text-align:center; border: 1px solid black; padding: 5px; background-color:#d3d3d3;'><strong>Witnessed by</strong></td>
                       </tr>
                       <tr>
                          <td style='border: 1px solid black; padding: 5px; font-weight: bold;'>Name</td>
                          <td style='border: 1px solid black; padding: 5px; font-weight: bold;'>Company</td>
                          <td style='border: 1px solid black; padding: 5px; font-weight: bold;'>Signature</td>
                       </tr>
                       <tr>
                           <td style='border: 1px solid black; padding: 15px;'></td>
                           <td style='border: 1px solid black; padding: 15px;'></td>
                           <td style='border: 1px solid black; padding: 15px;'></td>
                       </tr>
                       <tr>
                           <td style='text-align:left; border: 1px solid black; padding: 8px;'>
                               <strong>Tested by:</strong><br /><br /><br />
                               <strong>Checked by:</strong><br /><br /><br />
                           </td>
                           <td colspan='3' style='border: 1px solid black; padding: 15px;'></td>
                       </tr>
                   </table>");

                        // Generate the HTML file
                        body = objEmail.PrepareMailBodyWith("TestRecord_DimensionCFCC.html",
                            "{Header}", sbHeader.ToString(),
                            "{TestDetails}", sbDetails.ToString(),
                            "{Dates}", sbDates.ToString(),
                            "{Footer}", sbFooter.ToString());

                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord_DimensionCFCC -" + SampleID + ".htm");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_DimensionCFCC -" + SampleID + ".htm"));
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