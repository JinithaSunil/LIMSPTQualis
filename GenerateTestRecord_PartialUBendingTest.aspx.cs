using LIMS.DAL;
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Web;

namespace LIMS
{
    public partial class GenerateTestRecord_PartialUBendingTest : System.Web.UI.Page
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
                        //int TestRequestReviewID = 0;

                        dtRecordDetails = _cm.getTestRecord(ReportId, SampleID);
                        if (dtRecordDetails.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtRecordDetails.Rows)
                            {
                                string reportParamMapID = dr["ReportParamMapID"].ToString();
                                string result = dr["Result"].ToString();
                                string paramName = dr["ReportParameter"]?.ToString() ?? "";

                                if (result.Contains(","))
                                {
                                    string[] parts = result.Split(',');
                                }
                                else
                                {
                                    if (reportParamMapID == "17144") { Type = result; }
                                    else if (reportParamMapID == "17145") { Size = result; }
                                    else if (reportParamMapID == "17146") { Voltage = result; }
                                    else if (reportParamMapID == "17147") { Date = result; }
                                    else if (reportParamMapID == "17148") { RoomNo = result; }
                                    else if (reportParamMapID == "17149") { RoomTemp = result; }
                                    else if (reportParamMapID == "17150") { RefSpec = result; }
                                    else if (reportParamMapID == "17151") { TestMethodNo = result; }
                                    else if (reportParamMapID == "17152") { JobOrderNo = result; }
                                    else if (reportParamMapID == "17153") { WorkSheetNo = result; }
                                    else if (reportParamMapID == "17154") { RoomHumidity = result; }
                                    else if (reportParamMapID == "17155") { EquipmentNo = result; }
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

                        // Header row
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center; width:5%;'><strong>No</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center; width:35%;'><strong>Item test</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center; width:10%;'><strong>Unit</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center; width:35%;'><strong>Specified Value</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center; width:15%;'><strong>Measured</strong></td>");
                        sbDetails.Append("</tr>");

                        int RowNumber = 1;

                        // How to add data rows from the data table // Get CSV strings for the detail table from dtRecordDetails//
                        string testItemsCsv = GetResultValue(dtRecordDetails, "17158");  // Test Item
                        string unitsCsv = GetResultValue(dtRecordDetails, "17159");      // Unit
                        string specsCsv = GetResultValue(dtRecordDetails, "17160");      // Specified Value
                        string measuredCsv = GetResultValue(dtRecordDetails, "17161");   // Measured Value

                        if (!string.IsNullOrEmpty(testItemsCsv))
                        {
                            string[] testItems = testItemsCsv.Split(',');
                            string[] units = unitsCsv.Split(',');
                            string[] specs = specsCsv.Split(',');
                            string[] measured = measuredCsv.Split(',');

                            for (int i = 0; i < testItems.Length; i++)
                            {
                                sbDetails.Append("<tr>");
                                sbDetails.Append("<td style='text-align:center;'>" + RowNumber + "</td>");
                                sbDetails.Append("<td>" + testItems[i].Trim() + "</td>");
                                sbDetails.Append("<td style='text-align:center;'>" + (i < units.Length ? units[i].Trim() : "") + "</td>");
                                sbDetails.Append("<td>" + (i < specs.Length ? specs[i].Trim() : "") + "</td>");
                                sbDetails.Append("<td>" + (i < measured.Length ? measured[i].Trim() : "") + "</td>");
                                sbDetails.Append("</tr>");
                                RowNumber++;
                            }
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
                        body = objEmail.PrepareMailBodyWith("TestRecord_PartialUBendingTest.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord_PartialUBendingTest-" + SampleID + ".htm");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_PartialUBendingTest-" + SampleID + ".htm"));
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


        private string GetResultValue(DataTable dt, string reportParamMapID)
        {
            DataRow[] rows = dt.Select($"ReportParamMapID = '{reportParamMapID}'");
            if (rows.Length > 0)
                return rows[0]["Result"].ToString();
            return "";
        }



    }
}