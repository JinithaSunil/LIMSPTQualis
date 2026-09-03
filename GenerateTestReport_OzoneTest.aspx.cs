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
	public partial class GenerateTestReport_OzoneTest : System.Web.UI.Page
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
                        Dictionary<string, List<string>> Thickness = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Width = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Diameter = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Lengthofsample = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Gaugelength = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Temperature = new Dictionary<string, List<string>>();

                        Dictionary<string, List<string>> Volumeoven= new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> OzoneConcentration = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Duration = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Elongate = new Dictionary<string, List<string>>(); 
                        Dictionary<string, List<string>> Result = new Dictionary<string, List<string>>(); 

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
                        string material = "";


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
                                        if (reportParamMapID == "16214") // Thickness
                                        {
                                            if (!Thickness.ContainsKey(reportParamMapID))
                                                Thickness[reportParamMapID] = new List<string>();
                                            Thickness[reportParamMapID].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "16215") // Width
                                        {
                                            if (!Width.ContainsKey(reportParamMapID))
                                                Width[reportParamMapID] = new List<string>();
                                            Width[reportParamMapID].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "16216") // Diameter
                                        {
                                            if (!Diameter.ContainsKey(reportParamMapID))
                                                Diameter[reportParamMapID] = new List<string>();
                                            Diameter[reportParamMapID].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "16217") // LengthOfSample
                                        {
                                            if (!Lengthofsample.ContainsKey(reportParamMapID))
                                                Lengthofsample[reportParamMapID] = new List<string>();
                                            Lengthofsample[reportParamMapID].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "16218") // Gaugelength
                                        {
                                            if (!Gaugelength.ContainsKey(reportParamMapID))
                                                Gaugelength[reportParamMapID] = new List<string>();
                                            Gaugelength[reportParamMapID].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "16219") // Temperature
                                        {
                                            if (!Temperature.ContainsKey(reportParamMapID))
                                                Temperature[reportParamMapID] = new List<string>();
                                            Temperature[reportParamMapID].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "16220") // VolumeOven
                                        {
                                            if (!Volumeoven.ContainsKey(reportParamMapID))
                                                Volumeoven[reportParamMapID] = new List<string>();
                                            Volumeoven[reportParamMapID].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "16221" || reportParamMapID == "16223") // OzoneConcentration1 and 2
                                        {
                                            if (!OzoneConcentration.ContainsKey(reportParamMapID))
                                                OzoneConcentration[reportParamMapID] = new List<string>();
                                            OzoneConcentration[reportParamMapID].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "16222") // Duration
                                        {
                                            if (!Duration.ContainsKey(reportParamMapID))
                                                Duration[reportParamMapID] = new List<string>();
                                            Duration[reportParamMapID].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "16224") // Elongate
                                        {
                                            if (!Elongate.ContainsKey(reportParamMapID))
                                                Elongate[reportParamMapID] = new List<string>();
                                            Elongate[reportParamMapID].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "16225") // Result
                                        {
                                            if (!Result.ContainsKey(reportParamMapID))
                                                Result[reportParamMapID] = new List<string>();
                                            Result[reportParamMapID].Add(formattedData);
                                        }

                                    }

                                }

                                else
                                {
                                    if (reportParamMapID == "16226") { Type = result; }
                                    else if (reportParamMapID == "16232") { RefSpec = result; }
                                    else if (reportParamMapID == "16227") { Size = result; }
                                    else if (reportParamMapID == "16233") { TestMethodNo = result; }
                                    else if (reportParamMapID == "16228") { Voltage = result; }
                                    else if (reportParamMapID == "16234") { JobOrderNo = result; }
                                    else if (reportParamMapID == "16235") { WorkSheetNo = result; }
                                    else if (reportParamMapID == "16230") { RoomNo = result; }
                                    else if (reportParamMapID == "16236") { RoomHumidity = result; }
                                    else if (reportParamMapID == "16231") { RoomTemp = result; }
                                    else if (reportParamMapID == "16229") { Date = result; }
                                    else if (reportParamMapID == "16237") { EquipmentNo = result; }
                                    else if (reportParamMapID == "16262") { material = result; }



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
                        
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td colspan='7' style='text-align:left; font-weight:bold; border-bottom:none;'>Material : " + material + "</td>");
                       
                        sbDetails.Append("</tr>");
                       
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center;'><strong>No.</strong></td>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center;'><strong>Item test</strong></td>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center;'><strong>Unit</strong></td>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center;'><strong>Specified</strong></td>");
                        sbDetails.Append("<td colspan='3' style='background-color:#bebebe; text-align:center;'><strong>Measured</strong></td>");
                        sbDetails.Append("</tr>");

                        // Second header row under Measured
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>Sample 1</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>Sample 2</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>Sample 3</strong></td>");

                        sbDetails.Append("</tr>");



                        int RowNumber = 1;

                        if (Thickness.ContainsKey("16214"))

                        {
                            var dataValues = Thickness["16214"];

                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "" };

                                while (inputParts.Length < 2)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");
                                sbDetails.Append($"<td>{RowNumber}</td>");
                                sbDetails.Append("<td>Thickness</td>");
                                sbDetails.Append($"<td>{unit}</td>");
                                sbDetails.Append($"<td>{specified}</td>");
                                sbDetails.Append($"<td>{inputParts[0]}</td>");
                                sbDetails.Append($"<td>{inputParts[1]}</td>");
                                sbDetails.Append($"<td>{inputParts[2]}</td>");
                               
                                sbDetails.Append("</tr>");
                                RowNumber++;

                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append("<td>Thickness</td>");
                            sbDetails.Append("<td></td><td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        if (Width.ContainsKey("16215"))

                        {
                            var dataValues = Width["16215"];

                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "" };

                                while (inputParts.Length < 2)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");
                                sbDetails.Append($"<td>{RowNumber}</td>");
                                sbDetails.Append("<td>Width</td>");
                                sbDetails.Append($"<td>{unit}</td>");
                                sbDetails.Append($"<td>{specified}</td>");
                                sbDetails.Append($"<td>{inputParts[0]}</td>");
                                sbDetails.Append($"<td>{inputParts[1]}</td>");
                                sbDetails.Append($"<td>{inputParts[2]}</td>");
                           
                                sbDetails.Append("</tr>");
                                RowNumber++;
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append("<td>Width</td>");
                            sbDetails.Append("<td></td><td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        if (Diameter.ContainsKey("16216"))

                        {
                            var dataValues = Diameter["16216"];

                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "" };

                                while (inputParts.Length < 2)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");
                                sbDetails.Append($"<td>{RowNumber}</td>");
                                sbDetails.Append("<td>Diameter</td>");
                                sbDetails.Append($"<td>{unit}</td>");
                                sbDetails.Append($"<td>{specified}</td>");
                                sbDetails.Append($"<td>{inputParts[0]}</td>");
                                sbDetails.Append($"<td>{inputParts[1]}</td>");
                                sbDetails.Append($"<td>{inputParts[2]}</td>");
                          
                                sbDetails.Append("</tr>");
                                RowNumber++;

                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append("<td>Diameter</td>");
                            sbDetails.Append("<td></td><td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }



                        if (Lengthofsample.ContainsKey("16217"))

                        {
                            var dataValues = Lengthofsample["16217"];

                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "" };

                                while (inputParts.Length < 2)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");
                                sbDetails.Append($"<td>{RowNumber}</td>");
                                sbDetails.Append("<td>Length of sample</td>");
                                sbDetails.Append($"<td>{unit}</td>");
                                sbDetails.Append($"<td>{specified}</td>");
                                sbDetails.Append($"<td>{inputParts[0]}</td>");
                                sbDetails.Append($"<td>{inputParts[1]}</td>");
                                sbDetails.Append($"<td>{inputParts[2]}</td>");
                
                                sbDetails.Append("</tr>");
                                RowNumber++;
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append("<td>Length of sample</td>");
                            sbDetails.Append("<td></td><td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }


                        if (Gaugelength.ContainsKey("16218"))

                        {
                            var dataValues = Gaugelength["16218"];

                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "" };

                                while (inputParts.Length < 2)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");

                                sbDetails.Append($"<td>{RowNumber}</td>");
                                sbDetails.Append("<td>Gauge length/diameter mandrell</td>");
                                sbDetails.Append($"<td>{unit}</td>");
                                sbDetails.Append($"<td>{specified}</td>");
                                sbDetails.Append($"<td>{inputParts[0]}</td>");
                                sbDetails.Append($"<td>{inputParts[1]}</td>");
                                sbDetails.Append($"<td>{inputParts[2]}</td>");
                             
                                sbDetails.Append("</tr>");
                                RowNumber++;
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append("<td>Gauge length/diameter mandrell</td>");
                            sbDetails.Append("<td></td><td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        if (Temperature.ContainsKey("16219"))

                        {
                            var dataValues = Temperature["16219"];

                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "" };

                                while (inputParts.Length < 2)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");

                                sbDetails.Append($"<td>{RowNumber}</td>");
                                sbDetails.Append("<td>Temperature</td>");
                                sbDetails.Append($"<td>{unit}</td>");
                                sbDetails.Append($"<td>{specified}</td>");
                                sbDetails.Append($"<td>{inputParts[0]}</td>");
                                sbDetails.Append($"<td>{inputParts[1]}</td>");
                                sbDetails.Append($"<td>{inputParts[2]}</td>");
                       
                                sbDetails.Append("</tr>");
                                RowNumber++;
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append("<td>Temperature</td>");
                            sbDetails.Append("<td></td><td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        if (Volumeoven.ContainsKey("16220"))

                        {
                            var dataValues = Volumeoven["16220"];

                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "" };

                                while (inputParts.Length < 2)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");

                                sbDetails.Append($"<td>{RowNumber}</td>");
                                sbDetails.Append("<td>Volume oven</td>");
                                sbDetails.Append($"<td>{unit}</td>");
                                sbDetails.Append($"<td>{specified}</td>");
                                sbDetails.Append($"<td>{inputParts[0]}</td>");
                                sbDetails.Append($"<td>{inputParts[1]}</td>");
                                sbDetails.Append($"<td>{inputParts[2]}</td>");
           
                                sbDetails.Append("</tr>");

                                RowNumber++;

                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");

                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append("<td>Volume oven</td>");
                            sbDetails.Append("<td></td><td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");

                            RowNumber++;
                        }

                        if (OzoneConcentration.ContainsKey("16221"))
                        {
                            var dataValues = OzoneConcentration["16221"];
                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "", "" };

                                while (inputParts.Length < 3)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");
                                sbDetails.Append($"<td>{RowNumber}</td>");
                                sbDetails.Append("<td>Ozone Concentration</td>");
                                sbDetails.Append($"<td>{unit}</td>");
                                sbDetails.Append($"<td>{specified}</td>");
                                sbDetails.Append($"<td>{inputParts[0]}</td>");
                                sbDetails.Append($"<td>{inputParts[1]}</td>");
                                sbDetails.Append($"<td>{inputParts[2]}</td>");
                                sbDetails.Append("</tr>");

                                RowNumber++;
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append("<td>Ozone Concentration</td>");
                            sbDetails.Append("<td></td><td></td><td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // Ozone Concentration - pphm
                        if (OzoneConcentration.ContainsKey("16223"))
                        {
                            var dataValues = OzoneConcentration["16223"];
                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "", "" };

                                while (inputParts.Length < 3)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");
                                sbDetails.Append($"<td>{RowNumber}</td>");
                                sbDetails.Append("<td>Ozone Concentration</td>");
                                sbDetails.Append($"<td>{unit}</td>");
                                sbDetails.Append($"<td>{specified}</td>");
                                sbDetails.Append($"<td>{inputParts[0]}</td>");
                                sbDetails.Append($"<td>{inputParts[1]}</td>");
                                sbDetails.Append($"<td>{inputParts[2]}</td>");
                                sbDetails.Append("</tr>");
                                RowNumber++;
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append("<td>Ozone Concentration</td>");
                            sbDetails.Append("<td></td><td></td><td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }
                        if (Duration.ContainsKey("16222"))

                        {
                            var dataValues = Duration["16222"];

                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "" };

                                while (inputParts.Length < 2)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");
                                sbDetails.Append($"<td>{RowNumber}</td>");
                                sbDetails.Append("<td>Duration</td>");
                                sbDetails.Append($"<td>{unit}</td>");
                                sbDetails.Append($"<td>{specified}</td>");
                                sbDetails.Append($"<td>{inputParts[0]}</td>");
                                sbDetails.Append($"<td>{inputParts[1]}</td>");
                                sbDetails.Append($"<td>{inputParts[2]}</td>");
         
                                sbDetails.Append("</tr>");
                                RowNumber++;

                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append("<td>Duration</td>");
                            sbDetails.Append("<td></td><td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;

                        }

                        if (Elongate.ContainsKey("16224"))

                        {
                            var dataValues = Elongate["16224"];

                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "" };

                                while (inputParts.Length < 2)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");

                                sbDetails.Append($"<td>{RowNumber}</td>");
                                sbDetails.Append("<td>Elongate</td>");
                                sbDetails.Append($"<td>{unit}</td>");
                                sbDetails.Append($"<td>{specified}</td>");
                                sbDetails.Append($"<td>{inputParts[0]}</td>");
                                sbDetails.Append($"<td>{inputParts[1]}</td>");
                                sbDetails.Append($"<td>{inputParts[2]}</td>");
                               
                                sbDetails.Append("</tr>");
                                RowNumber++;
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");

                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append("<td>Elongate</td>");
                            sbDetails.Append("<td></td><td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        if (Result.ContainsKey("16225"))

                        {
                            var dataValues = Result["16225"];

                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "" };

                                while (inputParts.Length < 2)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");

                                sbDetails.Append($"<td>{RowNumber}</td>");
                                sbDetails.Append("<td>Result</td>");
                                sbDetails.Append($"<td>{unit}</td>");
                                sbDetails.Append($"<td>{specified}</td>");
                                sbDetails.Append($"<td>{inputParts[0]}</td>");
                                sbDetails.Append($"<td>{inputParts[1]}</td>");
                                sbDetails.Append($"<td>{inputParts[2]}</td>");
                               
                                sbDetails.Append("</tr>");
                                RowNumber++;
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");

                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append("<td>Result</td>");
                            sbDetails.Append("<td></td><td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
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
                        body = objEmail.PrepareMailBodyWith("TestRecord_OzoneTest.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord_OzoneTest -" + SampleID + ".htm");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_OzoneTest -" + SampleID + ".htm"));
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