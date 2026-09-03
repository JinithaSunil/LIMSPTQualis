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
    public partial class GenerateTestRecord_FireResistantTest : System.Web.UI.Page
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
                        //Dictionary<string, List<string>> d0 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> d1 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> d2 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> d3 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> d4 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> d5 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> d6 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> d7 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> d8 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> d9 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> d10 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> d11 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> d12 = new Dictionary<string, List<string>>();

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

                                        if (reportParamMapID == "16174")
                                        {
                                            if (!d1.ContainsKey(paramName))
                                                d1[paramName] = new List<string>();
                                            d1[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "16175")
                                        {
                                            if (!d2.ContainsKey(paramName))
                                                d2[paramName] = new List<string>();
                                            d2[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "16176")
                                        {
                                            if (!d3.ContainsKey(paramName))
                                                d3[paramName] = new List<string>();
                                            d3[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "16177")
                                        {
                                            if (!d4.ContainsKey(paramName))
                                                d4[paramName] = new List<string>();
                                            d4[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "16178")
                                        {
                                            if (!d5.ContainsKey(paramName))
                                                d5[paramName] = new List<string>();
                                            d5[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "16179")
                                        {
                                            if (!d6.ContainsKey(paramName))
                                                d6[paramName] = new List<string>();
                                            d6[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "16180")
                                        {
                                            if (!d7.ContainsKey(paramName))
                                                d7[paramName] = new List<string>();
                                            d7[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "16181")
                                        {
                                            if (!d8.ContainsKey(paramName))
                                                d8[paramName] = new List<string>();
                                            d8[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "16182")
                                        {
                                            if (!d9.ContainsKey(paramName))
                                                d9[paramName] = new List<string>();
                                            d9[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "16183")
                                        {
                                            if (!d10.ContainsKey(paramName))
                                                d10[paramName] = new List<string>();
                                            d10[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "16184")
                                        {
                                            if (!d11.ContainsKey(paramName))
                                                d11[paramName] = new List<string>();
                                            d11[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "16185")
                                        {
                                            if (!d12.ContainsKey(paramName))
                                                d12[paramName] = new List<string>();
                                            d12[paramName].Add(formattedData);
                                        }

                                    }
                                }
                                else
                                {
                                    if (reportParamMapID == "16160") { Type = result; }
                                    else if (reportParamMapID == "16161") { Size = result; }
                                    else if (reportParamMapID == "16162") { Voltage = result; }
                                    else if (reportParamMapID == "16163") { Date = result; }
                                    else if (reportParamMapID == "16164") { RoomNo = result; }
                                    else if (reportParamMapID == "16165") { RoomTemp = result; }
                                    else if (reportParamMapID == "16166") { RefSpec = result; }
                                    else if (reportParamMapID == "16167") { TestMethodNo = result; }
                                    else if (reportParamMapID == "16168") { JobOrderNo = result; }
                                    else if (reportParamMapID == "16169") { WorkSheetNo = result; }
                                    else if (reportParamMapID == "16170") { RoomHumidity = result; }
                                    else if (reportParamMapID == "16171") { EquipmentNo = result; }
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

                        //if (d0.Count > 0)
                        //{
                        //    foreach (var entry in d0)
                        //    {
                        //        var dataValues = entry.Value;
                        //        if (dataValues.Count > 0)
                        //        {
                        //            string[] parts = dataValues[0].Split('|');
                        //            string unit = parts.Length > 0 ? parts[0] : "";
                        //            string specified = parts.Length > 1 ? parts[1] : "-";
                        //            string valueTest = parts.Length > 2 ? parts[2] : "";
                        //            sbDetails.Append("<tr>");
                        //            sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Category</td>");
                        //            sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{unit}</td>");
                        //            sbDetails.Append($"<td colspan='3' style='border:1px solid black; text-align:center; padding:5px;'></td>");
                        //            sbDetails.Append("</tr>");
                        //        }
                        //    }
                        //}

                        // Header row
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>No</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>Item test</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>Unit</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>Specified Value</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>Measured</strong></td>");
                        sbDetails.Append("</tr>");

                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td colspan='5' style='background-color:#bebebe; text-align:left;'><strong>Fire resistant characteristics tests: FIRE RESISITANT ALONE</strong></td>");
                        sbDetails.Append("</tr>");

                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td colspan='5' style='background-color:#bebebe; text-align:left;'><strong>Category test:-</strong></td>");
                        sbDetails.Append("</tr>");

                        int RowNumber = 1;

                        // Length of Specimen
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
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Length of Specimen</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{unit}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:left; padding:5px;'>{specified}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{valueTest}</td>");
                                    sbDetails.Append("</tr>");
                                    RowNumber++;
                                }
                                else
                                {
                                    // Empty data row
                                    sbDetails.Append("<tr>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{RowNumber}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Length of Specimen</td>");
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
                            sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Length of Specimen</td>");
                            sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // Diameter of Cable
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
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Diameter of Cable</td>");
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
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Diameter of Cable</td>");
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
                            sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Diameter of Cable</td>");
                            sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // Size of fuse connector 
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
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Size of fuse connector </td>");
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
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Size of fuse connector</td>");
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
                            sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Size of fuse connector</td>");
                            sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // Type of ignition source
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
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Type of ignition source</td>");
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
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Type of ignition source</td>");
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
                            sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Type of ignition source</td>");
                            sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // Flame temperature
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
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Flame temperature</td>");
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
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Flame temperature</td>");
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
                            sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Flame temperature</td>");
                            sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }


                        // Test voltage 
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
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Test voltage </td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{unit}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:left; padding:5px;'>{specified}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{valueTest}</td>");
                                    sbDetails.Append("</tr>");
                                    RowNumber++;
                                }
                                else
                                {
                                    AppendEmptyRow("Test voltage ", ref sbDetails, ref RowNumber);
                                }
                            }
                        }
                        else
                        {
                            AppendEmptyRow("Test voltage ", ref sbDetails, ref RowNumber);
                        }

                        // Current
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
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{RowNumber}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Current</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{unit}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:left; padding:5px;'>{specified}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{valueTest}</td>");
                                    sbDetails.Append("</tr>");
                                    RowNumber++;
                                }
                                else
                                {
                                    AppendEmptyRow("Current", ref sbDetails, ref RowNumber);
                                }
                            }
                        }
                        else
                        {
                            AppendEmptyRow("Current", ref sbDetails, ref RowNumber);
                        }

                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td style='background-color:#bebebe;'>8</td>");
                        sbDetails.Append("<td colspan='4' style='background-color:#bebebe;'>Flame application</td>");
                        sbDetails.Append("</tr>");

                        // Flame application time
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
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>-</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Flame application time</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{unit}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:left; padding:5px;'>{specified}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{valueTest}</td>");
                                    sbDetails.Append("</tr>");
                                    RowNumber++;
                                }
                                else
                                {
                                    AppendEmptyRow("Flame application time", ref sbDetails, ref RowNumber);
                                }
                            }
                        }
                        else
                        {
                            AppendEmptyRow("Flame application time", ref sbDetails, ref RowNumber);
                        }

                        // Cooling Period
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
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>-</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Cooling Period</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{unit}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:left; padding:5px;'>{specified}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{valueTest}</td>");
                                    sbDetails.Append("</tr>");
                                    RowNumber++;
                                }
                                else
                                {
                                    AppendEmptyRow("Cooling Period", ref sbDetails, ref RowNumber);
                                }
                            }
                        }
                        else
                        {
                            AppendEmptyRow("Cooling Period", ref sbDetails, ref RowNumber);
                        }

                        // Spray period with water
                        if (d10.Count > 0)
                        {
                            foreach (var entry in d10)
                            {
                                var dataValues = entry.Value;
                                if (dataValues.Count > 0)
                                {
                                    string[] parts = dataValues[0].Split('|');
                                    string unit = parts.Length > 0 ? parts[0] : "";
                                    string specified = parts.Length > 1 ? parts[1] : "-";
                                    string valueTest = parts.Length > 2 ? parts[2] : "";

                                    sbDetails.Append("<tr>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>-</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Spray period with water</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{unit}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:left; padding:5px;'>{specified}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{valueTest}</td>");
                                    sbDetails.Append("</tr>");
                                    RowNumber++;
                                }
                                else
                                {
                                    AppendEmptyRow("Spray period with water", ref sbDetails, ref RowNumber);
                                }
                            }
                        }
                        else
                        {
                            AppendEmptyRow("Spray period with water", ref sbDetails, ref RowNumber);
                        }

                        // Shock period at ignite the burner
                        if (d11.Count > 0)
                        {
                            foreach (var entry in d11)
                            {
                                var dataValues = entry.Value;
                                if (dataValues.Count > 0)
                                {
                                    string[] parts = dataValues[0].Split('|');
                                    string unit = parts.Length > 0 ? parts[0] : "";
                                    string specified = parts.Length > 1 ? parts[1] : "-";
                                    string valueTest = parts.Length > 2 ? parts[2] : "";

                                    sbDetails.Append("<tr>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>-</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Shock period at ignite the burner</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{unit}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:left; padding:5px;'>{specified}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{valueTest}</td>");
                                    sbDetails.Append("</tr>");
                                    RowNumber++;
                                }
                                else
                                {
                                    AppendEmptyRow("Shock period at ignite the burner", ref sbDetails, ref RowNumber);
                                }
                            }
                        }
                        else
                        {
                            AppendEmptyRow("Shock period at ignite the burner", ref sbDetails, ref RowNumber);
                        }

                        //Test result
                        if (d12.Count > 0)
                        {
                            foreach (var entry in d12)
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
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Test result</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{unit}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:left; padding:5px;'>{specified}</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{valueTest}</td>");
                                    sbDetails.Append("</tr>");
                                    RowNumber++;
                                }
                                else
                                {
                                    AppendEmptyRow("Test result", ref sbDetails, ref RowNumber);
                                }
                            }
                        }
                        else
                        {
                            AppendEmptyRow("Test result", ref sbDetails, ref RowNumber);
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
                        body = objEmail.PrepareMailBodyWith("TestRecord_FireResistantTest.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord_FireResistantTest-" + SampleID + ".htm");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_FireResistantTest-" + SampleID + ".htm"));
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

        // Helper method to avoid code duplication for empty rows
        private void AppendEmptyRow(string parameterName, ref StringBuilder sbDetails, ref int RowNumber)
        {
            sbDetails.Append("<tr>");
            sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{RowNumber}</td>");
            sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>{parameterName}</td>");
            sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
            sbDetails.Append("</tr>");
            RowNumber++;
        }


    }
}