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
    public partial class GenerateTestRecord_SaltSpraytest : System.Web.UI.Page
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
                        Dictionary<string, List<string>> LengthOfSample = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> ActualCrossSectionalArea = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> TemperatureOfChamberOfSpray = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> DurationOfSaltSpray = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> QuantityOfSaltSpray = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> TotalWeightBeforeAfterSaltSpray = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> VisualAfterSaltSpray = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> TSWireAfterSaltSprayLayer1 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> TSWireAfterSaltSprayLayer2 = new Dictionary<string, List<string>>();


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

                                        if (reportParamMapID == "12346") // Length of sample
                                        {
                                            if (!LengthOfSample.ContainsKey(paramName))
                                                LengthOfSample[paramName] = new List<string>();
                                            LengthOfSample[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "12347") // Actual Cross sectional area
                                        {
                                            if (!ActualCrossSectionalArea.ContainsKey(paramName))
                                                ActualCrossSectionalArea[paramName] = new List<string>();
                                            ActualCrossSectionalArea[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "12348") // Temperature of chamber of spray
                                        {
                                            if (!TemperatureOfChamberOfSpray.ContainsKey(paramName))
                                                TemperatureOfChamberOfSpray[paramName] = new List<string>();
                                            TemperatureOfChamberOfSpray[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "12349") // Duration of salt spray
                                        {
                                            if (!DurationOfSaltSpray.ContainsKey(paramName))
                                                DurationOfSaltSpray[paramName] = new List<string>();
                                            DurationOfSaltSpray[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "12350") // Quantity of salt spray
                                        {
                                            if (!QuantityOfSaltSpray.ContainsKey(paramName))
                                                QuantityOfSaltSpray[paramName] = new List<string>();
                                            QuantityOfSaltSpray[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "12351") // Total weight before/after salt spray
                                        {
                                            if (!TotalWeightBeforeAfterSaltSpray.ContainsKey(paramName))
                                                TotalWeightBeforeAfterSaltSpray[paramName] = new List<string>();
                                            TotalWeightBeforeAfterSaltSpray[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "12352") // Visual after salt spray
                                        {
                                            if (!VisualAfterSaltSpray.ContainsKey(paramName))
                                                VisualAfterSaltSpray[paramName] = new List<string>();
                                            VisualAfterSaltSpray[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "12353") // TS wire after salt spray min.95% RTS Layer 1
                                        {
                                            if (!TSWireAfterSaltSprayLayer1.ContainsKey(paramName))
                                                TSWireAfterSaltSprayLayer1[paramName] = new List<string>();
                                            TSWireAfterSaltSprayLayer1[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "12354") // TS wire after salt spray min.95% RTS Layer 2
                                        {
                                            if (!TSWireAfterSaltSprayLayer2.ContainsKey(paramName))
                                                TSWireAfterSaltSprayLayer2[paramName] = new List<string>();
                                            TSWireAfterSaltSprayLayer2[paramName].Add(formattedData);
                                        }
                                    }
                                }
                                else
                                {
                                    if (reportParamMapID == "12332") { Type = result; }
                                    else if (reportParamMapID == "12333") { Size = result; }
                                    else if (reportParamMapID == "12334") { Voltage = result; }
                                    else if (reportParamMapID == "12338") { RefSpec = result; }
                                    else if (reportParamMapID == "12339") { TestMethodNo = result; }
                                    else if (reportParamMapID == "12340") { JobOrderNo = result; }
                                    else if (reportParamMapID == "12341") { WorkSheetNo = result; }
                                    else if (reportParamMapID == "12336") { RoomNo = result; }
                                    else if (reportParamMapID == "12342") { RoomHumidity = result; }
                                    else if (reportParamMapID == "12337") { RoomTemp = result; }
                                    else if (reportParamMapID == "12335") { Date = result; }
                                    else if (reportParamMapID == "12343") { EquipmentNo = result; }

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
                                 <td>Diameter</td>
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
                        sbDetails.Append("<td colspan='2' style='background-color:#bebebe; text-align:center;'><strong>Description</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>Unit</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>Specified</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>Value test</strong></td>");
                        sbDetails.Append("</tr>");


                        int RowNumber = 1;

                        // Group 1: Length of sample 
                        if (LengthOfSample.Count > 0)
                        {
                            foreach (var entry in LengthOfSample)
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
                                    sbDetails.Append($"<td colspan='2' style='border:1px solid black; padding:5px;'>Length of sample</td>");
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
                                    sbDetails.Append($"<td colspan='2' style='border:1px solid black; padding:5px;'>Length of sample</td>");
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
                            sbDetails.Append($"<td colspan='2' style='border:1px solid black; padding:5px;'>Length of sample</td>");
                            sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // Group 2: Actual Cross sectional area
                        if (ActualCrossSectionalArea.Count > 0)
                        {
                            foreach (var entry in ActualCrossSectionalArea)
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
                                    sbDetails.Append($"<td colspan='2' style='border:1px solid black; padding:5px;'>Actual Cross sectional area</td>");
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
                                    sbDetails.Append($"<td colspan='2' style='border:1px solid black; padding:5px;'>Actual Cross sectional area</td>");
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
                            sbDetails.Append($"<td colspan='2' style='border:1px solid black; padding:5px;'>Actual Cross sectional area</td>");
                            sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // Group 3: Temperature of chamber of spray
                        if (TemperatureOfChamberOfSpray.Count > 0)
                        {
                            foreach (var entry in TemperatureOfChamberOfSpray)
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
                                    sbDetails.Append($"<td colspan='2' style='border:1px solid black; padding:5px;'>Temperature of chamber of spray</td>");
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
                                    sbDetails.Append($"<td colspan='2' style='border:1px solid black; padding:5px;'>Temperature of chamber of spray</td>");
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
                            sbDetails.Append($"<td colspan='2' style='border:1px solid black; padding:5px;'>Temperature of chamber of spray</td>");
                            sbDetails.Append("<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // Group 4: Duration of salt spray
                        if (DurationOfSaltSpray.Count > 0)
                        {
                            foreach (var entry in DurationOfSaltSpray)
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
                                    sbDetails.Append($"<td colspan='2' style='border:1px solid black; padding:5px;'>Duration of salt spray</td>");
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
                                    sbDetails.Append($"<td colspan='2' style='border:1px solid black; padding:5px;'>Duration of salt spray</td>");
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
                            sbDetails.Append($"<td colspan='2' style='border:1px solid black; padding:5px;'>Duration of salt spray</td>");
                            sbDetails.Append("<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // Group 5: Quantity of salt spray
                        if (QuantityOfSaltSpray.Count > 0)
                        {
                            foreach (var entry in QuantityOfSaltSpray)
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
                                    sbDetails.Append($"<td colspan='2' style='border:1px solid black; padding:5px;'>Quantity of salt spray</td>");
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
                                    sbDetails.Append($"<td colspan='2' style='border:1px solid black; padding:5px;'>Quantity of salt spray</td>");
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
                            sbDetails.Append($"<td colspan='2' style='border:1px solid black; padding:5px;'>Quantity of salt spray</td>");
                            sbDetails.Append("<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // Group 6: Total weight before/after salt spray
                        if (TotalWeightBeforeAfterSaltSpray.Count > 0)
                        {
                            foreach (var entry in TotalWeightBeforeAfterSaltSpray)
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
                                    sbDetails.Append($"<td colspan='2' style='border:1px solid black; padding:5px;'>Total weight before/after salt spray</td>");
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
                                    sbDetails.Append($"<td colspan='2' style='border:1px solid black; padding:5px;'>Total weight before/after salt spray</td>");
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
                            sbDetails.Append($"<td colspan='2' style='border:1px solid black; padding:5px;'>Total weight before/after salt spray</td>");
                            sbDetails.Append("<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // Group 7: Visual after salt spray
                        if (VisualAfterSaltSpray.Count > 0)
                        {
                            foreach (var entry in VisualAfterSaltSpray)
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
                                    sbDetails.Append($"<td colspan='2' style='border:1px solid black; padding:5px;'>Visual after salt spray</td>");
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
                                    sbDetails.Append($"<td colspan='2' style='border:1px solid black; padding:5px;'>Total weight before/after salt spray</td>");
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
                            sbDetails.Append($"<td colspan='2' style='border:1px solid black; padding:5px;'>Total weight before/after salt spray</td>");
                            sbDetails.Append("<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // Group 8: TS wire after salt spray min.95% RTS
                        string layer1Unit = "-", layer1Spec = "-", layer1Value = "-";
                        string layer2Unit = "-", layer2Spec = "-", layer2Value = "-";

                        bool hasLayer1 = false, hasLayer2 = false;

                        // Get Layer 1 Data
                        if (TSWireAfterSaltSprayLayer1.Count > 0)
                        {
                            var entry = TSWireAfterSaltSprayLayer1.Values.FirstOrDefault();
                            if (entry != null && entry.Count > 0)
                            {
                                string[] parts = entry[0].Split('|');
                                layer1Unit = parts.Length > 0 ? parts[0] : "-";
                                layer1Spec = parts.Length > 1 ? parts[1] : "-";
                                layer1Value = parts.Length > 2 ? parts[2] : "-";
                                hasLayer1 = true;
                            }
                        }

                        // Get Layer 2 Data
                        if (TSWireAfterSaltSprayLayer2.Count > 0)
                        {
                            var entry = TSWireAfterSaltSprayLayer2.Values.FirstOrDefault();
                            if (entry != null && entry.Count > 0)
                            {
                                string[] parts = entry[0].Split('|');
                                layer2Unit = parts.Length > 0 ? parts[0] : "-";
                                layer2Spec = parts.Length > 1 ? parts[1] : "-";
                                layer2Value = parts.Length > 2 ? parts[2] : "-";
                                hasLayer2 = true;
                            }
                        }

                        // Check if both layers are empty
                        if (!hasLayer1 && !hasLayer2)
                        {
                            // No data at all, show blank rows
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td rowspan='2' style='border:1px solid black; text-align:center; padding:5px;'>{RowNumber}</td>");
                            sbDetails.Append("<td rowspan='2' style='border:1px solid black; padding:5px;'>TS wire after salt<br>spray min.95% RTS</td>");
                            sbDetails.Append("<td style='border:1px solid black; padding:5px;'>Layer 1</td>");
                            sbDetails.Append("<td style='border:1px solid black; text-align:center;'></td>");
                            sbDetails.Append("<td style='border:1px solid black; text-align:center;'></td>");
                            sbDetails.Append("<td style='border:1px solid black; text-align:center;'></td>");
                            sbDetails.Append("</tr>");

                            sbDetails.Append("<tr>");
                            sbDetails.Append("<td style='border:1px solid black; padding:5px;'>Layer 2</td>");
                            sbDetails.Append("<td style='border:1px solid black; text-align:center;'></td>");
                            sbDetails.Append("<td style='border:1px solid black; text-align:center;'></td>");
                            sbDetails.Append("<td style='border:1px solid black; text-align:center;'></td>");
                            sbDetails.Append("</tr>");
                        }
                        else
                        {
                            // Row for Layer 1
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td rowspan='2' style='border:1px solid black; text-align:center; padding:5px;'>{RowNumber}</td>");
                            sbDetails.Append("<td rowspan='2' style='border:1px solid black; padding:5px;'>TS wire after salt<br>spray min.95% RTS</td>");
                            sbDetails.Append("<td style='border:1px solid black; padding:5px;'>Layer 1</td>");
                            sbDetails.Append($"<td style='border:1px solid black; text-align:center;'>{layer1Unit}</td>");
                            sbDetails.Append($"<td style='border:1px solid black; text-align:center;'>{layer1Spec}</td>");
                            sbDetails.Append($"<td style='border:1px solid black; text-align:center;'>{layer1Value}</td>");
                            sbDetails.Append("</tr>");

                            // Row for Layer 2
                            sbDetails.Append("<tr>");
                            sbDetails.Append("<td style='border:1px solid black; padding:5px;'>Layer 2</td>");
                            sbDetails.Append($"<td style='border:1px solid black; text-align:center;'>{layer2Unit}</td>");
                            sbDetails.Append($"<td style='border:1px solid black; text-align:center;'>{layer2Spec}</td>");
                            sbDetails.Append($"<td style='border:1px solid black; text-align:center;'>{layer2Value}</td>");
                            sbDetails.Append("</tr>");
                        }

                        RowNumber++;

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
                        body = objEmail.PrepareMailBodyWith("TestRecord_SaltSpraytest.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord_SaltSpraytest -" + SampleID + ".htm");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_SaltSpraytest -" + SampleID + ".htm"));
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