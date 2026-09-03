using LIMS.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class ColdElongation_GR : System.Web.UI.Page
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
                        Dictionary<string, List<string>> a01 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> a02 = new Dictionary<string, List<string>>();

                        Dictionary<string, List<string>> d01 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> d02 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> d03 = new Dictionary<string, List<string>>();

                        Dictionary<string, List<string>> D1 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> D2 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> D3 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> D4 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> D5 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> D6 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> D7 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> D8 = new Dictionary<string, List<string>>();

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

                                        if (reportParamMapID == "14074")
                                        {
                                            if (!d01.ContainsKey(paramName))
                                                d01[paramName] = new List<string>();
                                            d01[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "14075")
                                        {
                                            if (!d02.ContainsKey(paramName))
                                                d02[paramName] = new List<string>();
                                            d02[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "14076")
                                        {
                                            if (!d03.ContainsKey(paramName))
                                                d03[paramName] = new List<string>();
                                            d03[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "14077")
                                        {
                                            if (!D1.ContainsKey(reportParamMapID))
                                                D1[reportParamMapID] = new List<string>();
                                            D1[reportParamMapID].Add(result);
                                        }
                                        else if (reportParamMapID == "14078")
                                        {
                                            if (!D3.ContainsKey(reportParamMapID))
                                                D3[reportParamMapID] = new List<string>();
                                            D3[reportParamMapID].Add(result);
                                        }
                                        else if (reportParamMapID == "14079")
                                        {
                                            if (!D5.ContainsKey(reportParamMapID))
                                                D5[reportParamMapID] = new List<string>();
                                            D5[reportParamMapID].Add(result);
                                        }
                                        else if (reportParamMapID == "14080")
                                        {
                                            if (!D7.ContainsKey(reportParamMapID))
                                                D7[reportParamMapID] = new List<string>();
                                            D7[reportParamMapID].Add(result);
                                        }
                                    }
                                }
                                else
                                {                                    
                                    string formattedData = result;

                                    if (reportParamMapID == "14072")
                                    {
                                        if (!a01.ContainsKey(paramName))
                                            a01[paramName] = new List<string>();
                                        a01[paramName].Add(formattedData);
                                    }
                                    else if (reportParamMapID == "14073")
                                    {
                                        if (!a02.ContainsKey(paramName))
                                            a02[paramName] = new List<string>();
                                        a02[paramName].Add(formattedData);
                                    }

                                if (reportParamMapID == "14081")
                                    {
                                        if (!D2.ContainsKey(reportParamMapID))
                                            D2[reportParamMapID] = new List<string>();
                                        D2[reportParamMapID].Add(result);
                                    }
                                    else if (reportParamMapID == "14082")
                                    {
                                        if (!D4.ContainsKey(reportParamMapID))
                                            D4[reportParamMapID] = new List<string>();
                                        D4[reportParamMapID].Add(result);
                                    }
                                    else if (reportParamMapID == "14083")
                                    {
                                        if (!D6.ContainsKey(reportParamMapID))
                                            D6[reportParamMapID] = new List<string>();
                                        D6[reportParamMapID].Add(result);
                                    }
                                    else if (reportParamMapID == "14084")
                                    {
                                        if (!D8.ContainsKey(reportParamMapID))
                                            D8[reportParamMapID] = new List<string>();
                                        D8[reportParamMapID].Add(result);
                                    }


                                    if (reportParamMapID == "14058") { Type = result; }
                                    else if (reportParamMapID == "14059") { Size = result; }
                                    else if (reportParamMapID == "14060") { Voltage = result; }
                                    else if (reportParamMapID == "14061") { Date = result; }
                                    else if (reportParamMapID == "14062") { RoomNo = result; }
                                    else if (reportParamMapID == "14063") { RoomTemp = result; }
                                    else if (reportParamMapID == "14064") { RefSpec = result; }
                                    else if (reportParamMapID == "14065") { TestMethodNo = result; }
                                    else if (reportParamMapID == "14066") { JobOrderNo = result; }
                                    else if (reportParamMapID == "14067") { WorkSheetNo = result; }
                                    else if (reportParamMapID == "14068") { RoomHumidity = result; }
                                    else if (reportParamMapID == "14069") { EquipmentNo = result; }

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

                        if (a01.Count > 0)
                        {
                            foreach (var entry in a01)
                            {
                                var dataValues = entry.Value;
                                if (dataValues.Count > 0)
                                {
                                    string[] parts = dataValues[0].Split('|');
                                    string unit = parts.Length > 0 ? parts[0] : "";
                                    string specified = parts.Length > 1 ? parts[1] : "-";
                                    string valueTest = parts.Length > 2 ? parts[2] : "";

                                    sbDetails.Append("<tr>");
                                    sbDetails.Append($"<td colspan='8' style='border:1px solid black; padding:5px;'> <b>Insulation / Sheath :</b> {unit}</td>");
                                    sbDetails.Append("</tr>");
                                    
                                }
                                else
                                {
                                    // Empty data row
                                    sbDetails.Append("<tr>");
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Insulation / Sheath</td>");
                                    sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                                    sbDetails.Append("</tr>");
                                    
                                }
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Insulation / Sheath</td>");
                            sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                            sbDetails.Append("</tr>");
                            
                        }

                        
                        if (a02.Count > 0)
                        {
                            foreach (var entry in a02)
                            {
                                var dataValues = entry.Value;
                                if (dataValues.Count > 0)
                                {
                                    string[] parts = dataValues[0].Split('|');
                                    string unit = parts.Length > 0 ? parts[0] : "";
                                    string specified = parts.Length > 1 ? parts[1] : "-";
                                    string valueTest = parts.Length > 2 ? parts[2] : "";

                                    sbDetails.Append("<tr>");
                                    sbDetails.Append($"<td colspan='8' style='border:1px solid black; padding:5px;'> <b> Core: </b> {unit}</td>");
                                    sbDetails.Append("</tr>");                                    
                                }
                                else
                                {
                                    sbDetails.Append("<tr>");
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Core:</td>");
                                    sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                                    sbDetails.Append("</tr>");                                    
                                }
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Core:</td>");
                            sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                            sbDetails.Append("</tr>");                            
                        }

                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td colspan='8' style='background-color:#bebebe;'><b>1. Cooling:</b></td>");
                        sbDetails.Append("</tr>");

                        // Header row
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>Item test</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>Unit</strong></td>");
                        sbDetails.Append("<td colspan='3' style='background-color:#bebebe; text-align:center;'><strong>Specified Value</strong></td>");
                        sbDetails.Append("<td colspan='3' style='background-color:#bebebe; text-align:center;'><strong>Measured</strong></td>");
                        sbDetails.Append("</tr>");


                       

                        // Temperature
                        if (d01.Count > 0)
                        {
                            foreach (var entry in d01)
                            {
                                var dataValues = entry.Value;
                                if (dataValues.Count > 0)
                                {
                                    string[] parts = dataValues[0].Split('|');
                                    string unit = parts.Length > 0 ? parts[0] : "";
                                    string specified = parts.Length > 1 ? parts[1] : "-";
                                    string valueTest = parts.Length > 2 ? parts[2] : "";

                                    sbDetails.Append("<tr>");                                    
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Temperature</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{unit}</td>");
                                    sbDetails.Append($"<td colspan='3' style='border:1px solid black; text-align:center; padding:5px;'>{specified}</td>");
                                    sbDetails.Append($"<td colspan='3' style='border:1px solid black; text-align:center; padding:5px;'>{valueTest}</td>");
                                    sbDetails.Append("</tr>");
                                    
                                }
                                else
                                {
                                    // Empty data row
                                    sbDetails.Append("<tr>");                                    
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Temperature</td>");
                                    sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                                    sbDetails.Append("</tr>");
                                    
                                }
                            }
                        }
                        else
                        {
                            // No data - still add row with test name and empty values
                            sbDetails.Append("<tr>");                            
                            sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Temperature</td>");
                            sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                            sbDetails.Append("</tr>");
                            
                        }

                        // Cooling time of the sample/piece test inside the apparatus
                        if (d02.Count > 0)
                        {
                            foreach (var entry in d02)
                            {
                                var dataValues = entry.Value;
                                if (dataValues.Count > 0)
                                {
                                    string[] parts = dataValues[0].Split('|');
                                    string unit = parts.Length > 0 ? parts[0] : "";
                                    string specified = parts.Length > 1 ? parts[1] : "-";
                                    string valueTest = parts.Length > 2 ? parts[2] : "";

                                    sbDetails.Append("<tr>");                                    
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Cooling time of the sample/piece test inside the apparatus</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{unit}</td>");
                                    sbDetails.Append($"<td colspan='3' style='border:1px solid black; text-align:center; padding:5px;'>{specified}</td>");
                                    sbDetails.Append($"<td colspan='3' style='border:1px solid black; text-align:center; padding:5px;'>{valueTest}</td>");
                                    sbDetails.Append("</tr>");
                                    
                                }
                                else
                                {
                                    sbDetails.Append("<tr>");                                    
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Cooling time of the sample/piece test inside the apparatus</td>");
                                    sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                                    sbDetails.Append("</tr>");
                                    
                                }
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");                            
                            sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Cooling time of the sample/piece test inside the apparatus</td>");
                            sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                            sbDetails.Append("</tr>");
                            
                        }

                        // Conditioning time of the sample/piece test inside apparatus
                        if (d03.Count > 0)
                        {
                            foreach (var entry in d03)
                            {
                                var dataValues = entry.Value;
                                if (dataValues.Count > 0)
                                {
                                    string[] parts = dataValues[0].Split('|');
                                    string unit = parts.Length > 0 ? parts[0] : "";
                                    string specified = parts.Length > 1 ? parts[1] : "-";
                                    string valueTest = parts.Length > 2 ? parts[2] : "";

                                    sbDetails.Append("<tr>");
                                    
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Conditioning time of the sample/piece test inside apparatus</td>");
                                    sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{unit}</td>");
                                    sbDetails.Append($"<td colspan='3' style='border:1px solid black; text-align:center; padding:5px;'>{specified}</td>");
                                    sbDetails.Append($"<td colspan='3' style='border:1px solid black; text-align:center; padding:5px;'>{valueTest}</td>");
                                    sbDetails.Append("</tr>");
                                    
                                }
                                else
                                {
                                    sbDetails.Append("<tr>");
                                    
                                    sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Conditioning time of the sample/piece test inside apparatus</td>");
                                    sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                                    sbDetails.Append("</tr>");
                                    
                                }
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            
                            sbDetails.Append($"<td style='border:1px solid black; padding:5px;'>Conditioning time of the sample/piece test inside apparatus</td>");
                            sbDetails.Append($"<td style='border:1px solid black;'></td><td style='border:1px solid black;'></td><td style='border:1px solid black;'></td>");
                            sbDetails.Append("</tr>");
                            
                        }

                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td colspan='8' style='background-color:#bebebe;'><b>2 Elongation under cooling:</b></td>");
                        sbDetails.Append("</tr>");
                        // First header row
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center;'>Test Item</td>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center;'>Unit</td>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center;'>Specified</td>");
                        sbDetails.Append("<td colspan='5' style='background-color:#bebebe; text-align:center;'>Measurement</td>");
                        sbDetails.Append("</tr>");

                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'>1</td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'>2</td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'>3</td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'>4</td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'>Median</td>");
                        sbDetails.Append("</tr>");

                        

                        //int RowNumber = 1;

                        // Speed
                        if (D1.ContainsKey("14077"))
                        {
                            var dataValues = D1["14077"];
                            string avg = D2.ContainsKey("14081") ? D2["14081"].FirstOrDefault() : "";

                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "", "" };

                                while (inputParts.Length < 3)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");                                
                                sbDetails.Append("<td>Speed</td>");
                                sbDetails.Append($"<td>{unit}</td>");
                                sbDetails.Append($"<td>{specified}</td>");
                                sbDetails.Append($"<td>{inputParts[0]}</td>");
                                sbDetails.Append($"<td>{inputParts[1]}</td>");
                                sbDetails.Append($"<td>{inputParts[2]}</td>");
                                sbDetails.Append($"<td>{inputParts[3]}</td>");
                                sbDetails.Append($"<td>{avg}</td>");
                                sbDetails.Append("</tr>");
                                
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");                            
                            sbDetails.Append("<td>Speed</td>");
                            sbDetails.Append("<td></td><td></td><td></td><td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            
                        }

                        // L0
                        if (D3.ContainsKey("14078"))
                        {
                            var dataValues = D3["14078"];
                            string avg = D4.ContainsKey("14082") ? D4["14082"].FirstOrDefault() : "";

                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "", "" };

                                while (inputParts.Length < 3)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");                                
                                sbDetails.Append("<td>L0</td>");
                                sbDetails.Append($"<td>{unit}</td>");
                                sbDetails.Append($"<td>{specified}</td>");
                                sbDetails.Append($"<td>{inputParts[0]}</td>");
                                sbDetails.Append($"<td>{inputParts[1]}</td>");
                                sbDetails.Append($"<td>{inputParts[2]}</td>");
                                sbDetails.Append($"<td>{inputParts[3]}</td>");
                                sbDetails.Append($"<td>{avg}</td>");
                                sbDetails.Append("</tr>");
                                
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");                            
                            sbDetails.Append("<td>L0</td>");
                            sbDetails.Append("<td></td><td></td><td></td><td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            
                        }

                        // L1
                        if (D5.ContainsKey("14079"))
                        {
                            var dataValues = D5["14079"];
                            string avg = D6.ContainsKey("14083") ? D6["14083"].FirstOrDefault() : "";

                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "", "" };

                                while (inputParts.Length < 3)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");                                
                                sbDetails.Append("<td>L1</td>");
                                sbDetails.Append($"<td>{unit}</td>");
                                sbDetails.Append($"<td>{specified}</td>");
                                sbDetails.Append($"<td>{inputParts[0]}</td>");
                                sbDetails.Append($"<td>{inputParts[1]}</td>");
                                sbDetails.Append($"<td>{inputParts[2]}</td>");
                                sbDetails.Append($"<td>{inputParts[3]}</td>");
                                sbDetails.Append($"<td>{avg}</td>");
                                sbDetails.Append("</tr>");
                                
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");                            
                            sbDetails.Append("<td>L1</td>");
                            sbDetails.Append("<td></td><td></td><td></td><td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            
                        }

                        // EL 
                        if (D7.ContainsKey("14080"))
                        {
                            var dataValues = D7["14080"];
                            string avg = D8.ContainsKey("14084") ? D8["14084"].FirstOrDefault() : "";

                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "", "" };

                                while (inputParts.Length < 3)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");                                
                                sbDetails.Append("<td>EL</td>");
                                sbDetails.Append($"<td>{unit}</td>");
                                sbDetails.Append($"<td>{specified}</td>");
                                sbDetails.Append($"<td>{inputParts[0]}</td>");
                                sbDetails.Append($"<td>{inputParts[1]}</td>");
                                sbDetails.Append($"<td>{inputParts[2]}</td>");
                                sbDetails.Append($"<td>{inputParts[3]}</td>");
                                sbDetails.Append($"<td>{avg}</td>");
                                sbDetails.Append("</tr>");
                                
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");                            
                            sbDetails.Append("<td>EL</td>");
                            sbDetails.Append("<td></td><td></td><td></td><td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            
                        }


                        sbDetails.Append("</table>");

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
                        body = objEmail.PrepareMailBodyWith("ColdElongation_GR.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/ColdElongation_GR-" + SampleID + ".htm");
                        File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/ColdElongation_GR-" + SampleID + ".htm"));
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