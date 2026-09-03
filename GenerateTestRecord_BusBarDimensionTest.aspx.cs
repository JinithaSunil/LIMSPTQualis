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
    public partial class GenerateTestRecord_BusBarDimensionTest : System.Web.UI.Page
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
                        Dictionary<string, List<string>> D1 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> D2 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> D3 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> D4 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> D5 = new Dictionary<string, List<string>>();
                        //Dictionary<string, List<string>> D6 = new Dictionary<string, List<string>>();
                        //Dictionary<string, List<string>> D7 = new Dictionary<string, List<string>>();

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

                                        // ONLY Measurement values (no AVG here!)
                                        if (reportParamMapID == "12439")
                                        {
                                            if (!D1.ContainsKey(reportParamMapID))
                                                D1[reportParamMapID] = new List<string>();
                                            D1[reportParamMapID].Add(result);
                                        }
                                        else if (reportParamMapID == "12441")
                                        {
                                            if (!D3.ContainsKey(reportParamMapID))
                                                D3[reportParamMapID] = new List<string>();
                                            D3[reportParamMapID].Add(result);
                                        }
                                        
                                    }
                                }
                                else
                                {
                                    if (reportParamMapID == "12440")
                                    {
                                        if (!D2.ContainsKey(reportParamMapID))
                                            D2[reportParamMapID] = new List<string>();
                                        D2[reportParamMapID].Add(result);
                                    }
                                    else if (reportParamMapID == "12442")
                                    {
                                        if (!D4.ContainsKey(reportParamMapID))
                                            D4[reportParamMapID] = new List<string>();
                                        D4[reportParamMapID].Add(result);
                                    }
                                    else if (reportParamMapID == "12443")
                                    {
                                        if (!D5.ContainsKey(reportParamMapID))
                                            D5[reportParamMapID] = new List<string>();
                                        D5[reportParamMapID].Add(result);
                                    }
                                    
                                    if (reportParamMapID == "12425") { Type = result; }
                                    else if (reportParamMapID == "12426") { Size = result; }
                                    else if (reportParamMapID == "12427") { Voltage = result; }
                                    else if (reportParamMapID == "12428") { Date = result; }
                                    else if (reportParamMapID == "12429") { RoomNo = result; }
                                    else if (reportParamMapID == "12430") { RoomTemp = result; }
                                    else if (reportParamMapID == "12431") { RefSpec = result; }
                                    else if (reportParamMapID == "12432") { TestMethodNo = result; }
                                    else if (reportParamMapID == "12433") { JobOrderNo = result; }
                                    else if (reportParamMapID == "12434") { WorkSheetNo = result; }
                                    else if (reportParamMapID == "12435") { RoomHumidity = result; }
                                    else if (reportParamMapID == "12436") { EquipmentNo = result; }

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
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center;'>No.</td>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center;'>Description</td>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center;'>Unit</td>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center;'>Specified</td>");
                        sbDetails.Append("<td colspan='4' style='background-color:#bebebe; text-align:center;'>Value test</td>");
                        sbDetails.Append("</tr>");
                        // Second header row under Measured
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'>1</td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'>2</td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'>3</td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'>Average</td>");
                        sbDetails.Append("</tr>");
                        // Add group row: "1 Dimension :"
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td><b>1</b></td>");
                        sbDetails.Append("<td><b>Dimension :</b></td>");
                        sbDetails.Append("<td colspan='6'></td>");
                        sbDetails.Append("</tr>");

                        int RowNumber = 1;

                        // Width (1.1)
                        if (D1.ContainsKey("12439"))
                        {
                            var dataValues = D1["12439"];
                            string avg = D2.ContainsKey("12440") ? D2["12440"].FirstOrDefault() : "";

                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "", "" };

                                while (inputParts.Length < 3)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");
                                sbDetails.Append($"<td>1.{RowNumber}</td>");
                                sbDetails.Append("<td>Width</td>");
                                sbDetails.Append($"<td>{unit}</td>");
                                sbDetails.Append($"<td>{specified}</td>");
                                sbDetails.Append($"<td>{inputParts[0]}</td>");
                                sbDetails.Append($"<td>{inputParts[1]}</td>");
                                sbDetails.Append($"<td>{inputParts[2]}</td>");
                                sbDetails.Append($"<td>{avg}</td>");
                                sbDetails.Append("</tr>");
                                RowNumber++;
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td>1.{RowNumber}</td>");
                            sbDetails.Append("<td>Width</td>");
                            sbDetails.Append("<td></td><td></td><td></td><td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // Thickness (1.2)
                        if (D3.ContainsKey("12441"))
                        {
                            var dataValues = D3["12441"];
                            string avg = D4.ContainsKey("12442") ? D4["12442"].FirstOrDefault() : "";

                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "", "" };

                                while (inputParts.Length < 3)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");
                                sbDetails.Append($"<td>1.{RowNumber}</td>");
                                sbDetails.Append("<td>Thickness</td>");
                                sbDetails.Append($"<td>{unit}</td>");
                                sbDetails.Append($"<td>{specified}</td>");
                                sbDetails.Append($"<td>{inputParts[0]}</td>");
                                sbDetails.Append($"<td>{inputParts[1]}</td>");
                                sbDetails.Append($"<td>{inputParts[2]}</td>");
                                sbDetails.Append($"<td>{avg}</td>");
                                sbDetails.Append("</tr>");
                                RowNumber++;
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td>1.{RowNumber}</td>");
                            sbDetails.Append("<td>Thickness</td>");
                            sbDetails.Append("<td></td><td></td><td></td><td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                       
                        // Final row: Result of test
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td style='text-align:center;'></td>");
                        sbDetails.Append("<td style='text-align:center;'><b>Result of test</b></td>");
                        sbDetails.Append("<td colspan='5' style='text-align:right;'></td>");
                        string resultOfTest = D5.ContainsKey("12443") ? D5["12443"].FirstOrDefault() : "";
                        sbDetails.Append($"<td><b>{resultOfTest}</b></td>");
                        sbDetails.Append("</tr>");

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
                        body = objEmail.PrepareMailBodyWith("TestRecord_BusBarDimensionTest.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord_BusBarDimensionTest-" + SampleID + ".htm");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_BusBarDimensionTest-" + SampleID + ".htm"));
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