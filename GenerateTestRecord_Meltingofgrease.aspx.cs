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
    public partial class GenerateTestRecord_Meltingofgrease : System.Web.UI.Page
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
                        // Initialize variables for grease test data
                        string BeratGreaseData = "";
                        string TemperaturData = "";
                        string KondisiGreaseData = "";
                        string KeteranganData = "";

                        // Basic report data
                        string Type = "";
                        string Size = "";
                        string Length = "";
                        string RefSpec = "";
                        string DrumNo = "";
                        string JobOrderNo = "";
                        string WorkSheetNo = "";
                        string Date = "";
                        string RoomNo = "";
                        string RoomHumidity = "";
                        string RoomTemp = "";
                        string EquipmentNo = "";
                        string MarkingOfCable = "";

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

                                // Map the report parameters based on your submit form mapping
                                if (reportParamMapID == "12300") { Type = result; }
                                else if (reportParamMapID == "12826") { Size = result; }
                                else if (reportParamMapID == "12302") { Length = result; }
                                else if (reportParamMapID == "12306") { RefSpec = result; }
                                else if (reportParamMapID == "12301") { DrumNo = result; }
                                else if (reportParamMapID == "12307") { JobOrderNo = result; }
                                else if (reportParamMapID == "12308") { WorkSheetNo = result; }
                                else if (reportParamMapID == "12827") { Date = result; }
                                else if (reportParamMapID == "12303") { RoomNo = result; }
                                else if (reportParamMapID == "12309") { RoomHumidity = result; }
                                else if (reportParamMapID == "12304") { RoomTemp = result; }
                                else if (reportParamMapID == "12310") { EquipmentNo = result; }
                                else if (reportParamMapID == "12305") { MarkingOfCable = result; }
                                else if (reportParamMapID == "12311") { BeratGreaseData = result; }
                                else if (reportParamMapID == "12823") { TemperaturData = result; }
                                else if (reportParamMapID == "12824") { KondisiGreaseData = result; }
                                else if (reportParamMapID == "12825") { KeteranganData = result; }
                            }
                        }

                        // Build the header table
                        sbHeader.Append(@"<table style='width: 100%; border: 1px solid; margin-top: 10px; ' class='tab1'>
                    <tr>
                        <td><strong>Type</strong></td>
                        <td>" + Type + @"</td>
                        <td><strong>Ref. Spec.</strong></td>
                        <td>" + RefSpec + @"</td>
                    </tr>
                    <tr>
                        <td><strong>Size</strong></td>
                        <td>" + Size + @"</td>
                        <td><strong>Drum No.</strong></td>
                        <td>" + DrumNo + @"</td>
                    </tr>
                    <tr>
                        <td><strong>Length</strong></td>
                        <td>" + Length + @"</td>
                        <td><strong>Job Order No.</strong></td>
                        <td>" + JobOrderNo + @"</td>
                    </tr>
                    <tr>
                        <td><strong>Date</strong></td>
                        <td>" + Date + @"</td>
                        <td><strong>Work Sheet No.</strong></td>
                        <td>" + WorkSheetNo + @"</td>
                    </tr>
                    <tr>
                        <td><strong>Room No.</strong></td>
                        <td>" + RoomNo + @"</td>
                        <td><strong>Room Humidity (%)</strong></td>
                        <td>" + RoomHumidity + @"</td>
                    </tr>
                    <tr>
                        <td><strong>Room Temp. (°C)</strong></td>
                        <td>" + RoomTemp + @"</td>
                        <td><strong>Equipment No.</strong></td>
                        <td>" + EquipmentNo + @"</td>
                    </tr>
                    <tr>
                        <td><strong>Marking of Cable</strong></td>
                        <td>" + MarkingOfCable + @"</td>
                        <td></td>
                        <td></td>
                    </tr>
                </table>");

                        // Build the details table for grease test data
                        sbDetails.Append(@"<table class='tab1' border='1' cellpadding='3' cellspacing='0' style='width:100%; border-collapse: collapse; margin-top: 10px;'>");

                        // Header row
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center; border:1px solid black; padding:5px;'><strong>No</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center; border:1px solid black; padding:5px;'><strong>Berat Grease (kg/km)</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center; border:1px solid black; padding:5px;'><strong>Temperatur (°C)</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center; border:1px solid black; padding:5px;'><strong>Kondisi Grease</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center; border:1px solid black; padding:5px;'><strong>Keterangan</strong></td>");
                        sbDetails.Append("</tr>");

                        // Parse the pipe-separated data and create rows
                        string[] beratGreaseValues = BeratGreaseData.Split('|');
                        string[] temperaturValues = TemperaturData.Split('|');
                        string[] kondisiGreaseValues = KondisiGreaseData.Split('|');
                        string[] keteranganValues = KeteranganData.Split('|');

                        // Create 7 rows for the test data
                        for (int i = 0; i < 7; i++)
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{i + 1}</td>");
                            sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{(i < beratGreaseValues.Length ? beratGreaseValues[i] : "")}</td>");
                            sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{(i < temperaturValues.Length ? temperaturValues[i] : "")}</td>");
                            sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{(i < kondisiGreaseValues.Length ? kondisiGreaseValues[i] : "")}</td>");
                            sbDetails.Append($"<td style='border:1px solid black; text-align:center; padding:5px;'>{(i < keteranganValues.Length ? keteranganValues[i] : "")}</td>");
                            sbDetails.Append("</tr>");
                        }

                        // Add footer row with requirement
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td colspan='5' style='border:1px solid black; text-align:left; padding:5px;'><strong>Persyaratan : suhu leleh min : 120°C</strong></td>");
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
                        sbDetails.Append("</table>");

                        // Generate the HTML file
                        body = objEmail.PrepareMailBodyWith("TestRecord_Meltingofgrease.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord_Meltingofgrease -" + SampleID + ".htm");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_Meltingofgrease -" + SampleID + ".htm"));
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