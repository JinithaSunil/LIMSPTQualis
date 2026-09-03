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
    public partial class GenerateBreakingload : System.Web.UI.Page
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
                        Dictionary<string, List<string>> Breakingload = new Dictionary<string, List<string>>();

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
                        string Grade = "";
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

                        string body = "";
                        string str = "";
                        string filename = "";
                        string html = "";
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
                                        string Measured = parts[1].Trim();
                                        string specified = parts[2].Trim();
                                        string formattedData = $"{unit}|{Measured}|{specified}";

                                        if (reportParamMapID == "12649") // Dimension
                                        {
                                            if (!Breakingload.ContainsKey(paramName))
                                                Breakingload[paramName] = new List<string>();
                                            Breakingload[paramName].Add(formattedData);
                                        }
                                    }
                                }
                                else // Non-measurement data (simple key-value pairs)
                                {
                                    if (reportParamMapID == "12650") { Type = result; }
                                    else if (reportParamMapID == "12651") { Size = result; }
                                    else if (reportParamMapID == "12652") { Grade = result; } // voltage value in grade
                                    else if (reportParamMapID == "12656") { RefSpec = result; }
                                    else if (reportParamMapID == "12657") { TestMethodNo = result; }
                                    else if (reportParamMapID == "12658") { JobOrderNo = result; }
                                    else if (reportParamMapID == "12659") { WorkSheetNo = result; }
                                    else if (reportParamMapID == "12654") { RoomNo = result; }
                                    else if (reportParamMapID == "12660") { RoomHumidity = result; }
                                    else if (reportParamMapID == "12655") { RoomTemp = result; }
                                    else if (reportParamMapID == "12653") { Date = result; }
                                    else if (reportParamMapID == "12661") { EquipmentNo = result; }
                                }
                            }
                        }

                        // Build header table OUTSIDE if, so it always appears
                        sbHeader.Append($@"<table style='width: 100%; border: 1px solid; margin-top: 10px;' class='tab1'>
        <tr>
            <td>Type</td><td>{Type}</td>
            <td>Ref Spec.</td><td>{RefSpec}</td>
        </tr>
        <tr>
            <td>Size</td><td>{Size}</td>
            <td>Test Method No.</td><td>{TestMethodNo}</td>
        </tr>
        <tr>
            <td>Grade</td><td>{Grade}</td>
            <td>Job Order No.</td><td>{JobOrderNo}</td>
        </tr>
        <tr>
            <td>Date</td><td>{Date}</td>
            <td>Work Sheet No.</td><td>{WorkSheetNo}</td>
        </tr>
        <tr>
            <td>Room No.</td><td>{RoomNo}</td>
            <td>Room Humidity</td><td>{RoomHumidity}%</td>
        </tr>
        <tr>
            <td>Room Temp.</td><td>{RoomTemp}°C</td>
            <td>Equipment No.</td><td>{EquipmentNo}</td>
        </tr>
    </table>");

                        sbDetails.Append(@"<table class='tab1' border='1' cellpadding='3' cellspacing='0' style='width:100%; border-collapse: collapse;'>");

                        // Header row
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>No</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>Item Test</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>Unit</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>Measured</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>Specified Test</strong></td>");
                        sbDetails.Append("</tr>");

                        int RowNumber = 1;

                        if (dtRecordDetails.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtRecordDetails.Rows)
                            {
                                string reportParamMapID = dr["ReportParamMapID"].ToString();
                                string result = dr["Result"].ToString();
                                string paramName = dr["ReportParameter"]?.ToString() ?? "";

                                if (reportParamMapID == "12649")
                                {
                                    string unit = "-";
                                    string measured = "-";
                                    string specified = "-";

                                    if (!string.IsNullOrEmpty(result) && result.Contains("|"))
                                    {
                                        string[] parts = result.Split('|');
                                        unit = parts.Length > 0 ? parts[0].Trim() : "-";
                                        measured = parts.Length > 1 ? parts[1].Trim() : "-";
                                        specified = parts.Length > 2 ? parts[2].Trim() : "-";
                                    }

                                    sbDetails.Append("<tr>");
                                    sbDetails.Append($"<td style='text-align:center; vertical-align:middle;'>{RowNumber}</td>");
                                    sbDetails.Append($"<td>{paramName}</td>");
                                    sbDetails.Append($"<td style='text-align:center;'>{unit}</td>");
                                    sbDetails.Append($"<td style='text-align:center;'>{measured}</td>");
                                    sbDetails.Append($"<td style='text-align:center;'>Min:-{specified}</td>");
                                    sbDetails.Append("</tr>");

                                    RowNumber++;
                                }
                            }
                        }

                        sbDetails.Append("</table>");


                        sbFooter.Append(@"<table style='width:100%; font-family:Times New Roman; font-size:11px' class='tab1'>
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

                        // Generate the HTML file and display
                        body = objEmail.PrepareMailBodyWith(
                            "TestRecord_Breakingload.html",
                            "{Header}", sbHeader.ToString(),
                            "{TestDetails}", sbDetails.ToString(),
                            "{Dates}", sbDates.ToString(),
                            "{Footer}", sbFooter.ToString());

                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord_Breakingload -" + SampleID + ".htm");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_Breakingload -" + SampleID + ".htm"));
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