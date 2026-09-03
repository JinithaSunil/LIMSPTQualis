using LIMS.DAL;
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;

namespace LIMS
{
    public partial class GenerateTestRecord_HeatingCycleTest : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usertype"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (Request.QueryString["s"] != null)
                Session["SampleID"] = Request.QueryString["s"].ToString();

            if (Request.QueryString["r"] != null)
                Session["ReportId"] = Request.QueryString["r"].ToString();

            if (Session["SampleID"] == null || Session["ReportId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            int sampleID = Convert.ToInt32(Session["SampleID"]);
            int reportId = Convert.ToInt32(Session["ReportId"]);

            string type = "", testDate = "", roomNo = "", roomTemp = "",
                   refSpec = "", testMethodNo = "", jobOrderNo = "", workSheetNo = "",
                   roomHumidity = "", equipmentNo = "", size = "", voltage = "", date = "";

            DataTable dtRecordDetails = _cm.getTestRecord(reportId, sampleID);

            if (dtRecordDetails.Rows.Count > 0)
            {
                foreach (DataRow dr in dtRecordDetails.Rows)
                {
                    string reportParamMapID = dr["ReportParamMapID"].ToString();
                    string result = dr["Result"].ToString();

                    switch (reportParamMapID)
                    {
                        case "17567": type = result; break;
                        case "17568": size = result; break;
                        case "17569": voltage = result; break;

                        // 🔹 Format main header date (17570)
                        case "17570":
                            if (DateTime.TryParse(result, out DateTime parsedHeaderDate))
                                date = parsedHeaderDate.ToString("dd-MM-yyyy");
                            else
                                date = result;
                            break;

                        case "17571": roomNo = result; break;
                        case "17572": roomTemp = result; break;
                        case "17573": refSpec = result; break;
                        case "17574": testMethodNo = result; break;
                        case "17575": jobOrderNo = result; break;
                        case "17576": workSheetNo = result; break;
                        case "17577": roomHumidity = result; break;
                        case "17578": equipmentNo = result; break;
                    }
                }
            }

            StringBuilder sbHeader = new StringBuilder();
            StringBuilder sbDetails = new StringBuilder();
            StringBuilder sbFooter = new StringBuilder();

            // ===== Header Table =====
            sbHeader.Append($@"
                <table style='width: 100%; border: 1px solid; margin-top: 10px;' class='tab1'>
                    <tr><td>Type</td><td>{type}</td><td>Ref Spec.</td><td>{refSpec}</td></tr>
                    <tr><td>Size</td><td>{size}</td><td>Test Method No.</td><td>{testMethodNo}</td></tr>
                    <tr><td>Voltage</td><td>{voltage}</td><td>Job Order No.</td><td>{jobOrderNo}</td></tr>
                    <tr><td>Date</td><td>{date}</td><td>Work Sheet No.</td><td>{workSheetNo}</td></tr>
                    <tr><td>Room No.</td><td>{roomNo}</td><td>Room Humidity</td><td>{roomHumidity}%</td></tr>
                    <tr><td>Room Temp.</td><td>{roomTemp}°C</td><td>Equipment No.</td><td>{equipmentNo}</td></tr>
                </table>
            ");

            // ===== Details Table =====
            sbDetails.Append(@"<table class='tab1' border='1' cellpadding='3' cellspacing='0' 
                                       style='width:100%; border-collapse: collapse; text-align:center;'>
                                    <tr>
                                        <td rowspan='3' style='background-color:#bebebe; vertical-align: middle; text-align:center;'>
                                            <strong>Cycle</strong>
                                        </td>
                                        <td colspan='5' style='background-color:#bebebe; text-align:center;'>
                                            <strong>Heating cycle applied to conductor</strong>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan='2' style='background-color:#bebebe; text-align:center;'><strong>Start</strong></td>
                                        <td rowspan='2' style='background-color:#bebebe; vertical-align: middle; text-align:center;'>
                                            <strong>Reached 95°–100°<br/>Time</strong>
                                        </td>
                                        <td rowspan='2' style='background-color:#bebebe; vertical-align: middle; text-align:center;'>
                                            <strong>Actual Temp.<br/>°C</strong>
                                        </td>
                                        <td rowspan='2' style='background-color:#bebebe; vertical-align: middle; text-align:center;'>
                                            <strong>Cooling<br/>Time</strong>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='background-color:#bebebe; text-align:center;'><strong>Date</strong></td>
                                        <td style='background-color:#bebebe; text-align:center;'><strong>Time</strong></td>
                                    </tr>
                            ");

            string startDatesCsv = GetResultValue(dtRecordDetails, "17581");
            string startTimesCsv = GetResultValue(dtRecordDetails, "17582");
            string reachedTimesCsv = GetResultValue(dtRecordDetails, "17583");
            string actualTempsCsv = GetResultValue(dtRecordDetails, "17584");
            string coolingTimesCsv = GetResultValue(dtRecordDetails, "17585");

            if (!string.IsNullOrEmpty(startDatesCsv))
            {
                string[] startDates = startDatesCsv.Split(',');
                string[] startTimes = startTimesCsv.Split(',');
                string[] reachedTimes = reachedTimesCsv.Split(',');
                string[] actualTemps = actualTempsCsv.Split(',');
                string[] coolingTimes = coolingTimesCsv.Split(',');

                for (int i = 0; i < startDates.Length; i++)
                {
                    string formattedDate = startDates[i].Trim();

                    // 🔹 Format detail dates
                    if (DateTime.TryParse(formattedDate, out DateTime parsedDetailDate))
                        formattedDate = parsedDetailDate.ToString("dd-MM-yyyy");

                    sbDetails.Append("<tr>");
                    sbDetails.Append($"<td style='text-align:center;'>{i + 1}</td>");
                    sbDetails.Append($"<td style='text-align:center;'>{formattedDate}</td>");
                    sbDetails.Append($"<td style='text-align:center;'>{(i < startTimes.Length ? startTimes[i].Trim() : "")}</td>");
                    sbDetails.Append($"<td style='text-align:center;'>{(i < reachedTimes.Length ? reachedTimes[i].Trim() : "")}</td>");
                    sbDetails.Append($"<td style='text-align:center;'>{(i < actualTemps.Length ? actualTemps[i].Trim() : "")}</td>");
                    sbDetails.Append($"<td style='text-align:center;'>{(i < coolingTimes.Length ? coolingTimes[i].Trim() : "")}</td>");
                    sbDetails.Append("</tr>");
                }
            }

            sbDetails.Append(@"<tr>
                                    <td rowspan='3' style='background-color:#bebebe; vertical-align: middle; text-align:center;'>
                                        <strong>Requirements</strong>
                                    </td>
                                    <td colspan='5' style='background-color:#bebebe; text-align:center;'>
                                      <strong>Minimum heating for 2 hours</strong>
                                    </td>
                                    </tr>
                                    <tr>
                                        <td colspan='5' style='background-color:#bebebe; text-align:center;'><strong>Natural cooling in air for at least 3 hours</strong></td>
                                    </tr>
                                    <tr>
                                        <td colspan='5' style='background-color:#bebebe; text-align:center;'><strong>Each cycle 8 hours</strong></td>                                        
                                    </tr>
                            ");



            sbDetails.Append("</table>");

            // ===== Footer Table =====
            sbFooter.Append(@"
                <table style='width:100%; font-family:Times New Roman; font-size:11px' class='tab1'>
                    <tr>
                        <td style='font-weight: bold;vertical-align: middle;' rowspan='2'>PT Qualis Indonesia</td>
                        <td colspan='3' style='text-align:center'><b>Witnessed By</b></td>
                    </tr>
                    <tr><td>Name</td><td>Company</td><td>Signature</td></tr>
                    <tr>
                        <td style='text-align:left'>Tested By:<br /><br /><br />Checked By:<br /><br /><br /></td>
                        <td></td><td></td>
                    </tr>
                </table>
            ");

            // ===== Generate HTML =====
            mailer objEmail = new mailer();
            string body = objEmail.PrepareMailBodyWith(
                "TestRecord_HeatingCycleTest.html",
                "{Header}", sbHeader.ToString(),
                "{TestDetails}", sbDetails.ToString(),
                "{Dates}", "",
                "{Footer}", sbFooter.ToString()
            );

            string filename = Server.MapPath($"//TestRecord/TestRecord_HeatingCycleTest-{sampleID}.htm");
            File.WriteAllText(filename, body);
            divCRFReview.InnerHtml = File.ReadAllText(filename);
        }

        private string GetResultValue(DataTable dt, string reportParamMapID)
        {
            DataRow[] rows = dt.Select($"ReportParamMapID = '{reportParamMapID}'");
            return rows.Length > 0 ? rows[0]["Result"].ToString() : "";
        }

    }
}
