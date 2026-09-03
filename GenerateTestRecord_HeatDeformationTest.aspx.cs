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
    public partial class GenerateTestRecord_HeatDeformationTest : System.Web.UI.Page
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
                        Dictionary<string, List<string>> D6 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> D7 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> D8 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> D9 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> D10 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> D11 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> D12 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> D13 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> D14 = new Dictionary<string, List<string>>();

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
                                        if (reportParamMapID == "16200")
                                        {
                                            if (!D1.ContainsKey(reportParamMapID))
                                                D1[reportParamMapID] = new List<string>();
                                            D1[reportParamMapID].Add(result);
                                        }
                                        else if (reportParamMapID == "16201")
                                        {
                                            if (!D3.ContainsKey(reportParamMapID))
                                                D3[reportParamMapID] = new List<string>();
                                            D3[reportParamMapID].Add(result);
                                        }
                                        else if (reportParamMapID == "16202")
                                        {
                                            if (!D5.ContainsKey(reportParamMapID))
                                                D5[reportParamMapID] = new List<string>();
                                            D5[reportParamMapID].Add(result);
                                        }
                                        else if (reportParamMapID == "16203")
                                        {
                                            if (!D7.ContainsKey(reportParamMapID))
                                                D7[reportParamMapID] = new List<string>();
                                            D7[reportParamMapID].Add(result);
                                        }
                                        else if (reportParamMapID == "16204")
                                        {
                                            if (!D9.ContainsKey(reportParamMapID))
                                                D9[reportParamMapID] = new List<string>();
                                            D9[reportParamMapID].Add(result);
                                        }
                                        else if (reportParamMapID == "16205")
                                        {
                                            if (!D11.ContainsKey(reportParamMapID))
                                                D11[reportParamMapID] = new List<string>();
                                            D11[reportParamMapID].Add(result);
                                        }
                                        else if (reportParamMapID == "16206")
                                        {
                                            if (!D13.ContainsKey(reportParamMapID))
                                                D13[reportParamMapID] = new List<string>();
                                            D13[reportParamMapID].Add(result);
                                        }
                                    }
                                }
                                else
                                {
                                    if (reportParamMapID == "16207")
                                    {
                                        if (!D2.ContainsKey(reportParamMapID))
                                            D2[reportParamMapID] = new List<string>();
                                        D2[reportParamMapID].Add(result);
                                    }
                                    else if (reportParamMapID == "16208")
                                    {
                                        if (!D4.ContainsKey(reportParamMapID))
                                            D4[reportParamMapID] = new List<string>();
                                        D4[reportParamMapID].Add(result);
                                    }
                                    else if (reportParamMapID == "16209")
                                    {
                                        if (!D6.ContainsKey(reportParamMapID))
                                            D6[reportParamMapID] = new List<string>();
                                        D6[reportParamMapID].Add(result);
                                    }
                                    else if (reportParamMapID == "16210")
                                    {
                                        if (!D8.ContainsKey(reportParamMapID))
                                            D8[reportParamMapID] = new List<string>();
                                        D8[reportParamMapID].Add(result);
                                    }
                                    else if (reportParamMapID == "16211")
                                    {
                                        if (!D10.ContainsKey(reportParamMapID))
                                            D10[reportParamMapID] = new List<string>();
                                        D10[reportParamMapID].Add(result);
                                    }
                                    else if (reportParamMapID == "16212")
                                    {
                                        if (!D12.ContainsKey(reportParamMapID))
                                            D12[reportParamMapID] = new List<string>();
                                        D12[reportParamMapID].Add(result);
                                    }
                                    else if (reportParamMapID == "16213")
                                    {
                                        if (!D14.ContainsKey(reportParamMapID))
                                            D14[reportParamMapID] = new List<string>();
                                        D14[reportParamMapID].Add(result);
                                    }

                                    if (reportParamMapID == "16186") { Type = result; }
                                    else if (reportParamMapID == "16187") { Size = result; }
                                    else if (reportParamMapID == "16188") { Voltage = result; }
                                    else if (reportParamMapID == "16189") { Date = result; }
                                    else if (reportParamMapID == "16190") { RoomNo = result; }
                                    else if (reportParamMapID == "16191") { RoomTemp = result; }
                                    else if (reportParamMapID == "16192") { RefSpec = result; }
                                    else if (reportParamMapID == "16193") { TestMethodNo = result; }
                                    else if (reportParamMapID == "16194") { JobOrderNo = result; }
                                    else if (reportParamMapID == "16195") { WorkSheetNo = result; }
                                    else if (reportParamMapID == "16196") { RoomHumidity = result; }
                                    else if (reportParamMapID == "16197") { EquipmentNo = result; }

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
                        sbDetails.Append("<td colspan='4' style='background-color:#bebebe; text-align:center;'>Measured</td>");
                        sbDetails.Append("</tr>");

                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'>1</td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'>2</td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'>3</td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'>Average</td>");
                        sbDetails.Append("</tr>");

                        //sbDetails.Append("<tr>");
                        //sbDetails.Append("<td><b>1</b></td>");
                        //sbDetails.Append("<td><b>Electrical test :</b></td>");
                        //sbDetails.Append("<td colspan='6'></td>");
                        //sbDetails.Append("</tr>");

                        int RowNumber = 1;

                        // Diameter
                        if (D1.ContainsKey("16200"))
                        {
                            var dataValues = D1["16200"];
                            string avg = D2.ContainsKey("16207") ? D2["16207"].FirstOrDefault() : "";

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
                                sbDetails.Append("<td>Diameter</td>");
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
                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append("<td>Diameter</td>");
                            sbDetails.Append("<td></td><td></td><td></td><td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // Load
                        if (D3.ContainsKey("16201"))
                        {
                            var dataValues = D3["16201"];
                            string avg = D4.ContainsKey("16208") ? D4["16208"].FirstOrDefault() : "";

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
                                sbDetails.Append("<td>Load</td>");
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
                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append("<td>Load</td>");
                            sbDetails.Append("<td></td><td></td><td></td><td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // Temperature
                        if (D5.ContainsKey("16202"))
                        {
                            var dataValues = D5["16202"];
                            string avg = D6.ContainsKey("16209") ? D6["16209"].FirstOrDefault() : "";

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
                                sbDetails.Append("<td>Temperature</td>");
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
                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append("<td>Temperature</td>");
                            sbDetails.Append("<td></td><td></td><td></td><td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // During test 
                        if (D7.ContainsKey("16203"))
                        {
                            var dataValues = D7["16203"];
                            string avg = D8.ContainsKey("16210") ? D8["16210"].FirstOrDefault() : "";

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
                                sbDetails.Append("<td>During test</td>");
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
                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append("<td>During test</td>");
                            sbDetails.Append("<td></td><td></td><td></td><td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        //Thickness before heating
                        if (D9.ContainsKey("16204"))
                        {
                            var dataValues = D9["16204"];
                            string avg = D10.ContainsKey("16211") ? D10["16211"].FirstOrDefault() : "";

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
                                sbDetails.Append("<td>Thickness before heating</td>");
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
                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append("<td>Thickness before heating</td>");
                            sbDetails.Append("<td></td><td></td><td></td><td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // Thickness after heating
                        if (D11.ContainsKey("16205"))
                        {
                            var dataValues = D11["16205"];
                            string avg = D12.ContainsKey("16212") ? D12["16212"].FirstOrDefault() : "";

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
                                sbDetails.Append("<td>Thickness after heating</td>");
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
                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append("<td>Thickness after heating</td>");
                            sbDetails.Append("<td></td><td></td><td></td><td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // Decrease of thickness
                        if (D13.ContainsKey("16206"))
                        {
                            var dataValues = D13["16206"];
                            string avg = D14.ContainsKey("16213") ? D14["16213"].FirstOrDefault() : "";

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
                                sbDetails.Append("<td>Decrease of thickness</td>");
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
                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append("<td>Decrease of thickness</td>");
                            sbDetails.Append("<td></td><td></td><td></td><td></td><td></td><td></td>");
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
                        body = objEmail.PrepareMailBodyWith("TestRecord_HeatDeformationTest.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord_HeatDeformationTest-" + SampleID + ".htm");
                        File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_HeatDeformationTest-" + SampleID + ".htm"));
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