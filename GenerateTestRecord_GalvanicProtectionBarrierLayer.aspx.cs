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
    public partial class GenerateTestRecord_GalvanicProtectionBarrierLayer : System.Web.UI.Page
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
                        Dictionary<string, List<string>> Diameterofsteelwire = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> DiameterofsteelwireAvg = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Weight_Thicknessbeforeimmersed = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Weight_ThicknessbeforeimmersedAvg = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Weight_Thicknessafterimmersed = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Weight_ThicknessafterimmersedAvg = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Weight_Thicknesszinccoatingimmersed = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Weight_ThicknesszinccoatingimmersedAvg = new Dictionary<string, List<string>>();

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

                                        // ONLY Measurement values (no AVG here!)
                                        if (reportParamMapID == "12815") // Diameter
                                        {
                                            if (!Diameterofsteelwire.ContainsKey(reportParamMapID))
                                                Diameterofsteelwire[reportParamMapID] = new List<string>();
                                            Diameterofsteelwire[reportParamMapID].Add(formattedData);
                                        }
                                    
                                    }
                                }
                                else
                                {

                                    if (reportParamMapID == "12816") //  AVG
                                    {
                                        if (!DiameterofsteelwireAvg.ContainsKey(reportParamMapID))
                                            DiameterofsteelwireAvg[reportParamMapID] = new List<string>();
                                        DiameterofsteelwireAvg[reportParamMapID].Add(result);
                                    }


                                    // Cable properties (simple values)
                                    else if (reportParamMapID == "12803") { Type = result; }             // Cable Type
                                    else if (reportParamMapID == "12809") { RefSpec = result; }          // Reference Specification
                                    else if (reportParamMapID == "12804") { Size = result; }             // Cable Size
                                    else if (reportParamMapID == "12810") { TestMethodNo = result; }     // Test Method No
                                    else if (reportParamMapID == "12805") { Voltage = result; }          // Voltage
                                    else if (reportParamMapID == "12811") { JobOrderNo = result; }       // Job Order No
                                    else if (reportParamMapID == "12812") { WorkSheetNo = result; }      // Worksheet No
                                    else if (reportParamMapID == "12807") { RoomNo = result; }           // Room No
                                    else if (reportParamMapID == "12813") { RoomHumidity = result; }     // Room Humidity
                                    else if (reportParamMapID == "12808") { RoomTemp = result; }         // Room Temperature
                                    else if (reportParamMapID == "12806") { Date = result; }             // Date
                                    else if (reportParamMapID == "12814") { EquipmentNo = result; }      // Equipment No

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
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center; width: 30px;'><strong>No.</strong></td>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center; width: 130px;'><strong>Item test</strong></td>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center; width: 50px;'><strong>Unit</strong></td>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center; width: 100px;'><strong>Specified</strong></td>");
                        sbDetails.Append("<td colspan='7' style='background-color:#bebebe; text-align:center;'><strong>Value Test</strong></td>");
                        sbDetails.Append("</tr>");

                        // Second header row under Measured
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>1</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>2</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>3</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>4</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>5</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>6</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>Avg.</strong></td>");
                        sbDetails.Append("</tr>");


                        int RowNumber = 1;

                        // Diameter of Steel Wire
                        if (Diameterofsteelwire.ContainsKey("12815"))
                        {
                            var dataValues = Diameterofsteelwire["12815"];
                            string avg = DiameterofsteelwireAvg.ContainsKey("12816") ? DiameterofsteelwireAvg["12816"].FirstOrDefault() : "";

                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "", "" ,"","",""};

                                while (inputParts.Length < 6)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");
                                sbDetails.Append($"<td>{RowNumber}</td>");
                                sbDetails.Append("<td>Thickness of coated</td>");
                                sbDetails.Append($"<td>{unit}</td>");
                                sbDetails.Append($"<td>{specified}</td>");
                                sbDetails.Append($"<td>{inputParts[0]}</td>");
                                sbDetails.Append($"<td>{inputParts[1]}</td>");
                                sbDetails.Append($"<td>{inputParts[2]}</td>");
                                sbDetails.Append($"<td>{inputParts[3]}</td>");
                                sbDetails.Append($"<td>{inputParts[4]}</td>");
                                sbDetails.Append($"<td>{inputParts[5]}</td>");
                                sbDetails.Append($"<td>{avg}</td>");
                                sbDetails.Append("</tr>");
                                RowNumber++;
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append("<td>Thickness of coated</td>");
                            sbDetails.Append("<td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td>");
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
                        body = objEmail.PrepareMailBodyWith("TestRecord_GalvanicProtectionBarrierLayer.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord_GalvanicProtectionBarrierLayer -" + SampleID + ".htm");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_GalvanicProtectionBarrierLayer -" + SampleID + ".htm"));
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