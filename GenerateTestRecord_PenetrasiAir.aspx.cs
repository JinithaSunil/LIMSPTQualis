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
    public partial class GenerateTestRecord_PenetrasiAir : System.Web.UI.Page
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
                   roomHumidity = "", equipmentNo = "", size = "", voltage = "", date = "",
                   Specification = "", SpecificationMin = "", SpecificationMin2 = "", 
                   onconductor1 = "", onconductor2 = "", onconductiveWBTtype1 = "", onconductiveWBTtype2 = "",
                   onnonconductiveWBTtype1 = "", onnonconductiveWBTtype2 = "";

            ;

            DataTable dtRecordDetails = _cm.getTestRecord(reportId, sampleID);

            if (dtRecordDetails.Rows.Count > 0)
            {
                foreach (DataRow dr in dtRecordDetails.Rows)
                {
                    string reportParamMapID = dr["ReportParamMapID"].ToString();
                    string result = dr["Result"].ToString();

                    switch (reportParamMapID)
                    {
                        case "17589": type = result; break;
                        case "17590": size = result; break;
                        case "17591": voltage = result; break;

                        //  Format main header date (17570)
                        case "17592":
                            if (DateTime.TryParse(result, out DateTime parsedHeaderDate))
                                date = parsedHeaderDate.ToString("dd-MM-yyyy");
                            else
                                date = result;
                            break;

                        case "17593": roomNo = result; break;
                        case "17594": roomTemp = result; break;
                        case "17595": refSpec = result; break;
                        case "17596": testMethodNo = result; break;
                        case "17597": jobOrderNo = result; break;
                        case "17598": workSheetNo = result; break;
                        case "17599": roomHumidity = result; break;
                        case "17600": equipmentNo = result; break;
                        case "17609": Specification = result; break;
                        case "17610": SpecificationMin = result; break;
                        case "17611": SpecificationMin2 = result; break;




                        case "17620": onconductor1 = result; break;
                        case "17623": onconductor2 = result; break;
                        case "17621": onconductiveWBTtype1 = result; break;
                        case "17629": onconductiveWBTtype2 = result; break;
                        case "17622": onnonconductiveWBTtype1 = result; break;
                        case "17630": onnonconductiveWBTtype2 = result; break;

                  
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
                                       
                                        <td colspan='5' style='background-color:#bebebe; text-align:center;'>
                                            <strong>Uji tekuk</strong>
                                        </td>

                                    <td colspan='4' style='background-color:#bebebe; text-align:center;'>
                                            <strong>Penetrasi air </strong>
                                        </td>
                                    </tr>
                                     <tr>
 <td rowspan='2' style='background-color:#bebebe; text-align:center;'>Tanggal</td>
 <td rowspan='2' style='background-color:#bebebe; text-align:center;'>Diameter kabel</td>
 <td rowspan='2' style='background-color:#bebebe; text-align:center;'>Diameter penghantar d</td>
 <td rowspan='2' style='background-color:#bebebe; text-align:center;'>Maks.diameter bending</td>
 <td rowspan='2' style='background-color:#bebebe; text-align:center;'>Diameter silinder</td>
 <td colspan='4' style='background-color:#bebebe; text-align:center;'>Pada temp. ruang 24 jam </td>
                                    </tr>


                            <tr>
                        <td colspan='2' style='background-color:#bebebe; text-align:center;'><strong>Mulai</strong></td>
                          <td colspan='2' style='background-color:#bebebe; vertical-align: middle; text-align:center;'>
                        <strong>Selesai</strong></td>
                                    </tr>
                                    <tr>
                                    <td  style='background-color:#bebebe; text-align:center;'>mm</td>
                                    <td  style='background-color:#bebebe; text-align:center;'>mm</td>
                                    <td  style='background-color:#bebebe; text-align:center;'>mm</td>
                                    <td  style='background-color:#bebebe; text-align:center;'>mm</td>
                                    <td  style='background-color:#bebebe; text-align:center;'>mm</td>
                                    <td  style='background-color:#bebebe; text-align:center;'>Jam</td>
                                    <td  style='background-color:#bebebe; text-align:center;'>Tanggal</td>
                                    <td  style='background-color:#bebebe; text-align:center;'>Jam</td>
                                    <td  style='background-color:#bebebe; text-align:center;'>Jam</td>
                                    </tr>
                                      
                            ");
            string dateForBendingList = GetResultValue(dtRecordDetails, "17612");
            string diameterCableList = GetResultValue(dtRecordDetails, "17624");
            string conductorDiameterList = GetResultValue(dtRecordDetails, "17627");
            string maxDiameterBendingList = GetResultValue(dtRecordDetails, "17625");
            string diameterCylinderList = GetResultValue(dtRecordDetails, "17626");
            string startDateWaterList = GetResultValue(dtRecordDetails, "17613");

            string startTimeWaterList = GetResultValue(dtRecordDetails, "17614");
            string finishDateWaterList = GetResultValue(dtRecordDetails, "17615");
            string finishTimeWaterList = GetResultValue(dtRecordDetails, "17616");


            if (!string.IsNullOrEmpty(dateForBendingList))
            {
                string[] dates = dateForBendingList.Split(',');
                string[] diameters = diameterCableList.Split(',');
                string[] conductorDiameters = conductorDiameterList.Split(',');
                string[] maxBendingDiameters = maxDiameterBendingList.Split(',');
                string[] cylinderDiameters = diameterCylinderList.Split(',');
                string[] waterStartDates = startDateWaterList.Split(',');
                string[] waterStartTimes = startTimeWaterList.Split(',');
                string[] waterFinishDates = finishDateWaterList.Split(',');
                string[] waterFinishTimes = finishTimeWaterList.Split(',');

                for (int i = 0; i < dates.Length; i++)
                {
                    string formattedDate = dates[i].Trim();
                    if (DateTime.TryParse(formattedDate, out DateTime parsedDate))
                        formattedDate = parsedDate.ToString("dd-MM-yyyy");


                    string formattedStartWaterDate = (i < waterStartDates.Length) ? waterStartDates[i].Trim() : "";
                    if (DateTime.TryParse(formattedStartWaterDate, out DateTime parsedStartWaterDate))
                        formattedStartWaterDate = parsedStartWaterDate.ToString("dd-MM-yyyy");

                    // Format Finish Water Date
                    string formattedFinishWaterDate = (i < waterFinishDates.Length) ? waterFinishDates[i].Trim() : "";
                    if (DateTime.TryParse(formattedFinishWaterDate, out DateTime parsedFinishWaterDate))
                        formattedFinishWaterDate = parsedFinishWaterDate.ToString("dd-MM-yyyy");


                    sbDetails.Append("<tr>");
                    sbDetails.Append($"<td style='text-align:center;'>{formattedDate}</td>");
                    sbDetails.Append($"<td style='text-align:center;'>{(i < diameters.Length ? diameters[i].Trim() : "")}</td>");
                    sbDetails.Append($"<td style='text-align:center;'>{(i < conductorDiameters.Length ? conductorDiameters[i].Trim() : "")}</td>");
                    sbDetails.Append($"<td style='text-align:center;'>{(i < maxBendingDiameters.Length ? maxBendingDiameters[i].Trim() : "")}</td>");
                    sbDetails.Append($"<td style='text-align:center;'>{(i < cylinderDiameters.Length ? cylinderDiameters[i].Trim() : "")}</td>");
                    sbDetails.Append($"<td style='text-align:center;'>{formattedStartWaterDate}</td>");
                    sbDetails.Append($"<td style='text-align:center;'>{(i < waterStartTimes.Length ? waterStartTimes[i].Trim() : "")}</td>");
                    sbDetails.Append($"<td style='text-align:center;'>{formattedFinishWaterDate}</td>");
                    sbDetails.Append($"<td style='text-align:center;'>{(i < waterFinishTimes.Length ? waterFinishTimes[i].Trim() : "")}</td>");
                    sbDetails.Append("</tr>");
                }
            }

            sbDetails.Append($@"
                        <tr>
                            <td colspan='9'  style='text-align:center'>            
                                <strong>Hasil pengujian penetrasi air</strong></td>
                    </tr>
                    <tr>
                            <td colspan='3'  style='text-align:center;'>
                                Panjang perembesan air
                            </td>
                            <td colspan='3' style='text-align:center;'>
                                <strong>Ujung  1 </strong>
                            </td>
                     <td colspan='3' style='text-align:center;'>
                                <strong>Ujung  2 </strong>
                            </td>
                        </tr>
                    <tr>
                    <td colspan='3' style='text-align:center;'>
                                <strong>Pada penghantar </strong></td>

                    <td  colspan='3' style='text-align:center;'>
                                {onconductor1}</td>
                    <td  colspan='3' style='text-align:center;'>
                                {onconductor2}</td>
                    </tr>
                    <tr>
                    <td colspan='3' style='text-align:center;'>Pada WBT  konduktif tipe</td>

                    <td colspan='3' style='text-align:center;'>{onconductiveWBTtype1}</td>

                    <td colspan='3' style='text-align:center;'>{onconductiveWBTtype2}</td>
                    </tr>

                    <tr>
                    <td colspan='3' style='text-align:center;'>Pada WBT non konduktif tipe</td>

                    <td colspan='3' style='text-align:center;'>{onnonconductiveWBTtype1}</td>

                    <td colspan='3' style='text-align:center;'>{onnonconductiveWBTtype2}</td>
                    </tr>

                        <tr><td colspan='3' style='text-align:center;'>Persyaratan</td>
                            <td colspan='6' style='text-align:center;'>
                                <strong>Selama pengujian tidak boleh ada air keluar dari ujung potongan sampel uji</strong>
                            </td>
                        </tr>
                    ");

         

            sbDetails.Append("</table>");


            sbDetails.Append(@"<table class='tab1' border='1' cellpadding='3' cellspacing='0' 
                                       style='width:100%; border-collapse: collapse; text-align:center;'>
                                    <tr>
                                        <td rowspan='3' style='background-color:#bebebe; vertical-align: middle; text-align:center;'>
                                            <strong>Siklus</strong>
                                        </td>
                                        <td colspan='5' style='background-color:#bebebe; text-align:center;'>
                                            <strong>Siklus pemanasan, dikenakan pada penghantar</strong>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan='2' style='background-color:#bebebe; text-align:center;'><strong>Mulai</strong></td>
                                        <td rowspan='2' style='background-color:#bebebe; vertical-align: middle; text-align:center;'>
                                            <strong>Tercapai
95-100⁰
<br/>Time</strong>
                                        </td>
                                        <td rowspan='2' style='background-color:#bebebe; vertical-align: middle; text-align:center;'>
                                            <strong>Aktual Temp.<br/>°C</strong>
                                        </td>
                                        <td rowspan='2' style='background-color:#bebebe; vertical-align: middle; text-align:center;'>
                                            <strong>Selesai<br/>Time</strong>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='background-color:#bebebe; text-align:center;'><strong>Tanggal</strong></td>
                                        <td style='background-color:#bebebe; text-align:center;'><strong>Jam</strong></td>
                                    </tr>
                            ");

            string startDatesCsv = GetResultValue(dtRecordDetails, "17603");
            string startTimesCsv = GetResultValue(dtRecordDetails, "17604");
            string reachedTimesCsv = GetResultValue(dtRecordDetails, "17605");
            string actualTempsCsv = GetResultValue(dtRecordDetails, "17606");
            string coolingTimesCsv = GetResultValue(dtRecordDetails, "17607");


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
            sbDetails.Append($@"
    <tr>
        <td colspan='3' rowspan='2' style='text-align:center; vertical-align: middle;'>
            
            <strong>Persyaratan:<br/></strong><br/>
            {Specification}
        </td>
        <td colspan='2' style='text-align:center;'>
            <strong>Min.: {SpecificationMin}</strong>
        </td>
        <td colspan='1' style='text-align:center;'>
            <strong>Min.: {SpecificationMin2}</strong>
        </td>
    </tr>
    <tr>
        <td colspan='3' style='text-align:center;'>
            <strong>Setiap satu siklus pemanasan 8 jam</strong>
        </td>
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
                "TestRecord_PenetrasiAir.html",
                "{Header}", sbHeader.ToString(),
                "{TestDetails}", sbDetails.ToString(),
                "{Dates}", "",
                "{Footer}", sbFooter.ToString()
            );

            string filename = Server.MapPath($"//TestRecord/TestRecord_PenetrasiAir-{sampleID}.htm");
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