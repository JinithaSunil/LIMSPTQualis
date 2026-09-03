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
	public partial class GenerateTestRecord_DimenisonOfzinccoatingTest : System.Web.UI.Page
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
                                        if (reportParamMapID == "12593") // Diameter
                                        {
                                            if (!Diameterofsteelwire.ContainsKey(reportParamMapID))
                                                Diameterofsteelwire[reportParamMapID] = new List<string>();
                                            Diameterofsteelwire[reportParamMapID].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "12594") // Before immersed
                                        {
                                            if (!Weight_Thicknessbeforeimmersed.ContainsKey(reportParamMapID))
                                                Weight_Thicknessbeforeimmersed[reportParamMapID] = new List<string>();
                                            Weight_Thicknessbeforeimmersed[reportParamMapID].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "12595") // After immersed
                                        {
                                            if (!Weight_Thicknessafterimmersed.ContainsKey(reportParamMapID))
                                                Weight_Thicknessafterimmersed[reportParamMapID] = new List<string>();
                                            Weight_Thicknessafterimmersed[reportParamMapID].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "12596") // Zinc coating
                                        {
                                            if (!Weight_Thicknesszinccoatingimmersed.ContainsKey(reportParamMapID))
                                                Weight_Thicknesszinccoatingimmersed[reportParamMapID] = new List<string>();
                                            Weight_Thicknesszinccoatingimmersed[reportParamMapID].Add(formattedData);
                                        }
                                    }
                                }
                                else 
                                {
                                 
                                    if (reportParamMapID == "12700") // Diameter AVG
                                    {
                                        if (!DiameterofsteelwireAvg.ContainsKey(reportParamMapID))
                                            DiameterofsteelwireAvg[reportParamMapID] = new List<string>();
                                        DiameterofsteelwireAvg[reportParamMapID].Add(result);
                                    }
                                    else if (reportParamMapID == "12701") // Before immersed AVG
                                    {
                                        if (!Weight_ThicknessbeforeimmersedAvg.ContainsKey(reportParamMapID))
                                            Weight_ThicknessbeforeimmersedAvg[reportParamMapID] = new List<string>();
                                        Weight_ThicknessbeforeimmersedAvg[reportParamMapID].Add(result);
                                    }
                                    else if (reportParamMapID == "12702") // After immersed AVG
                                    {
                                        if (!Weight_ThicknessafterimmersedAvg.ContainsKey(reportParamMapID))
                                            Weight_ThicknessafterimmersedAvg[reportParamMapID] = new List<string>();
                                        Weight_ThicknessafterimmersedAvg[reportParamMapID].Add(result);
                                    }
                                    else if (reportParamMapID == "12703") // Zinc coating AVG
                                    {
                                        if (!Weight_ThicknesszinccoatingimmersedAvg.ContainsKey(reportParamMapID))
                                            Weight_ThicknesszinccoatingimmersedAvg[reportParamMapID] = new List<string>();
                                        Weight_ThicknesszinccoatingimmersedAvg[reportParamMapID].Add(result);
                                    }

                                    // Cable properties (simple values)
                                    else if (reportParamMapID == "12598") { Type = result; }
                                    else if (reportParamMapID == "12603") { RefSpec = result; }
                                    else if (reportParamMapID == "12597") { Size = result; }
                                    else if (reportParamMapID == "12604") { TestMethodNo = result; }
                                    else if (reportParamMapID == "12599") { Voltage = result; }
                                    else if (reportParamMapID == "12605") { JobOrderNo = result; }
                                    else if (reportParamMapID == "12606") { WorkSheetNo = result; }
                                    else if (reportParamMapID == "12601") { RoomNo = result; }
                                    else if (reportParamMapID == "12607") { RoomHumidity = result; }
                                    else if (reportParamMapID == "12602") { RoomTemp = result; }
                                    else if (reportParamMapID == "12600") { Date = result; }
                                    else if (reportParamMapID == "12608") { EquipmentNo = result; }
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
                        sbDetails.Append("<td colspan='4' style='background-color:#bebebe; text-align:center;'><strong>Measured</strong></td>");
                        sbDetails.Append("</tr>");

                        // Second header row under Measured
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>1</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>2</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>3</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>Avg.</strong></td>");
                        sbDetails.Append("</tr>");


                        int RowNumber = 1;

                        // Diameter of Steel Wire
                        if (Diameterofsteelwire.ContainsKey("12593"))
                        {
                            var dataValues = Diameterofsteelwire["12593"];
                            string avg = DiameterofsteelwireAvg.ContainsKey("12700") ? DiameterofsteelwireAvg["12700"].FirstOrDefault() : "";

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
                                sbDetails.Append("<td>Diameter of Steel Wire</td>");
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
                            sbDetails.Append("<td>Diameter of Steel Wire</td>");
                            sbDetails.Append("<td></td><td></td><td></td><td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // Weight / Thickness before immersed
                        if (Weight_Thicknessbeforeimmersed.ContainsKey("12594"))
                        {
                            var dataValues = Weight_Thicknessbeforeimmersed["12594"];
                            string avg = Weight_ThicknessbeforeimmersedAvg.ContainsKey("12701") ? Weight_ThicknessbeforeimmersedAvg["12701"].FirstOrDefault() : "";

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
                                sbDetails.Append("<td>Weight / Thickness before immersed</td>");
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
                            sbDetails.Append("<td>Weight / Thickness before immersed</td>");
                            sbDetails.Append("<td></td><td></td><td></td><td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // Weight / Thickness after immersed
                        if (Weight_Thicknessafterimmersed.ContainsKey("12595"))
                        {
                            var dataValues = Weight_Thicknessafterimmersed["12595"];
                            string avg = Weight_ThicknessafterimmersedAvg.ContainsKey("12702") ? Weight_ThicknessafterimmersedAvg["12702"].FirstOrDefault() : "";

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
                                sbDetails.Append("<td>Weight / Thickness after immersed</td>");
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
                            sbDetails.Append("<td>Weight / Thickness after immersed</td>");
                            sbDetails.Append("<td></td><td></td><td></td><td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // Weight / Thickness zinc coating immersed
                        if (Weight_Thicknesszinccoatingimmersed.ContainsKey("12596"))
                        {
                            var dataValues = Weight_Thicknesszinccoatingimmersed["12596"];
                            string avg = Weight_ThicknesszinccoatingimmersedAvg.ContainsKey("12703") ? Weight_ThicknesszinccoatingimmersedAvg["12703"].FirstOrDefault() : "";

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
                                sbDetails.Append("<td>Weight / Thickness zinc coating immersed</td>");
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
                            sbDetails.Append("<td>Weight / Thickness zinc coating immersed</td>");
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
                        body = objEmail.PrepareMailBodyWith("TestRecord_Dimensionofzinccoating.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord_Dimensionofzinccoating -" + SampleID + ".htm");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_Dimensionofzinccoating -" + SampleID + ".htm"));
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