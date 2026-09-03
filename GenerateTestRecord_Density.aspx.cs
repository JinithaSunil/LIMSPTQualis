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
	public partial class GenerateTestRecord_Density : System.Web.UI.Page
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
                        Dictionary<string, List<string>> WeightOfSample = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Solution = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> WeightOfSampleInSolution = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Density = new Dictionary<string, List<string>>();


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

                                        if (reportParamMapID == "12552") // 
                                        {
                                            if (!WeightOfSample.ContainsKey(reportParamMapID))
                                                WeightOfSample[reportParamMapID] = new List<string>();
                                            WeightOfSample[reportParamMapID].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "12554") // 
                                        {
                                            if (!Solution.ContainsKey(reportParamMapID))
                                                Solution[reportParamMapID] = new List<string>();
                                            Solution[reportParamMapID].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "12556") // 
                                        {
                                            if (!WeightOfSampleInSolution.ContainsKey(reportParamMapID))
                                                WeightOfSampleInSolution[reportParamMapID] = new List<string>();
                                            WeightOfSampleInSolution[reportParamMapID].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "12558") // 
                                        {
                                            if (!Density.ContainsKey(reportParamMapID))
                                                Density[reportParamMapID] = new List<string>();
                                            Density[reportParamMapID].Add(formattedData);
                                        }
                                    }
                                }
                                else
                                {
                                    if (reportParamMapID == "12726") { Type = result; }
                                    else if (reportParamMapID == "12732") { RefSpec = result; }
                                    else if (reportParamMapID == "12727") { Size = result; }
                                    else if (reportParamMapID == "12733") { TestMethodNo = result; }
                                    else if (reportParamMapID == "12728") { Voltage = result; }
                                    else if (reportParamMapID == "12734") { JobOrderNo = result; }
                                    else if (reportParamMapID == "12735") { WorkSheetNo = result; }
                                    else if (reportParamMapID == "12730") { RoomNo = result; }
                                    else if (reportParamMapID == "12736") { RoomHumidity = result; }
                                    else if (reportParamMapID == "12731") { RoomTemp = result; }
                                    else if (reportParamMapID == "12729") { Date = result; }
                                    else if (reportParamMapID == "12737") { EquipmentNo = result; }

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

                        // First header row
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center;'><strong>No.</strong></td>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center;'><strong>Item test</strong></td>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center;'><strong>Unit</strong></td>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center;'><strong>Specified</strong></td>");
                        sbDetails.Append("<td colspan='5' style='background-color:#bebebe; text-align:center;'><strong>Measured</strong></td>");
                        sbDetails.Append("</tr>");

                        // Second header row under Measured
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>1</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>2</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>3</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>4</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>5</strong></td>");
                        sbDetails.Append("</tr>");


                        int RowNumber = 1;

                        
                        if (WeightOfSample.ContainsKey("12552"))
                        {
                            var dataValues = WeightOfSample["12552"];
                           
                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "", "" ,"",""};

                                while (inputParts.Length < 3)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");
                                sbDetails.Append($"<td>{RowNumber}</td>");
                                sbDetails.Append("<td>Weight of sample</td>");
                                sbDetails.Append($"<td>{unit}</td>");
                                sbDetails.Append($"<td>{specified}</td>");
                                sbDetails.Append($"<td>{inputParts[0]}</td>");
                                sbDetails.Append($"<td>{inputParts[1]}</td>");
                                sbDetails.Append($"<td>{inputParts[2]}</td>");
                                sbDetails.Append($"<td>{inputParts[3]}</td>");
                                sbDetails.Append($"<td>{inputParts[4]}</td>");
                          
                                sbDetails.Append("</tr>");
                                RowNumber++;
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append("<td>Weight of sample</td>");
                            sbDetails.Append("<td></td><td></td><td></td><td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }
                        // Solution (12554)
                        if (Solution.ContainsKey("12554"))
                        {
                            var dataValues = Solution["12554"];

                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "", "", "", "" };

                                while (inputParts.Length < 5)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");
                                sbDetails.Append($"<td>{RowNumber}</td>");
                                sbDetails.Append("<td>Solution</td>");
                                sbDetails.Append($"<td>{unit}</td>");
                                sbDetails.Append($"<td>{specified}</td>");
                                sbDetails.Append($"<td>{inputParts[0]}</td>");
                                sbDetails.Append($"<td>{inputParts[1]}</td>");
                                sbDetails.Append($"<td>{inputParts[2]}</td>");
                                sbDetails.Append($"<td>{inputParts[3]}</td>");
                                sbDetails.Append($"<td>{inputParts[4]}</td>");
                                sbDetails.Append("</tr>");
                                RowNumber++;
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append("<td>Solution</td>");
                            sbDetails.Append("<td></td><td></td><td></td><td></td><td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // Weight of sample in solution (12556)
                        if (WeightOfSampleInSolution.ContainsKey("12556"))
                        {
                            var dataValues = WeightOfSampleInSolution["12556"];

                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "", "", "", "" };

                                while (inputParts.Length < 5)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");
                                sbDetails.Append($"<td>{RowNumber}</td>");
                                sbDetails.Append("<td>Weight of sample in solution</td>");
                                sbDetails.Append($"<td>{unit}</td>");
                                sbDetails.Append($"<td>{specified}</td>");
                                sbDetails.Append($"<td>{inputParts[0]}</td>");
                                sbDetails.Append($"<td>{inputParts[1]}</td>");
                                sbDetails.Append($"<td>{inputParts[2]}</td>");
                                sbDetails.Append($"<td>{inputParts[3]}</td>");
                                sbDetails.Append($"<td>{inputParts[4]}</td>");
                                sbDetails.Append("</tr>");
                                RowNumber++;
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append("<td>Weight of sample in solution</td>");
                            sbDetails.Append("<td></td><td></td><td></td><td></td><td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // Density (12558)
                        if (Density.ContainsKey("12558"))
                        {
                            var dataValues = Density["12558"];

                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "", "", "", "" };

                                while (inputParts.Length < 5)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");
                                sbDetails.Append($"<td>{RowNumber}</td>");
                                sbDetails.Append("<td>Density</td>");
                                sbDetails.Append($"<td>{unit}</td>");
                                sbDetails.Append($"<td>{specified}</td>");
                                sbDetails.Append($"<td>{inputParts[0]}</td>");
                                sbDetails.Append($"<td>{inputParts[1]}</td>");
                                sbDetails.Append($"<td>{inputParts[2]}</td>");
                                sbDetails.Append($"<td>{inputParts[3]}</td>");
                                sbDetails.Append($"<td>{inputParts[4]}</td>");
                                sbDetails.Append("</tr>");
                                RowNumber++;
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append("<td>Density</td>");
                            sbDetails.Append("<td></td><td></td><td></td><td></td><td></td><td></td><td></td>");
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
                        body = objEmail.PrepareMailBodyWith("TestRecord_Density.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord_Density -" + SampleID + ".htm");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_Density -" + SampleID + ".htm"));
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