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
    public partial class GenerateTestRecord_AcidGasEmissionTest : System.Web.UI.Page
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
                        //Dictionary<string, List<string>> D13 = new Dictionary<string, List<string>>();
                        //Dictionary<string, List<string>> D14 = new Dictionary<string, List<string>>();

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
                                        if (reportParamMapID == "17176")
                                        {
                                            if (!D1.ContainsKey(reportParamMapID))
                                                D1[reportParamMapID] = new List<string>();
                                            D1[reportParamMapID].Add(result);
                                        }
                                        if (reportParamMapID == "17177")
                                        {
                                            if (!D2.ContainsKey(reportParamMapID))
                                                D2[reportParamMapID] = new List<string>();
                                            D2[reportParamMapID].Add(result);
                                        }
                                        else if (reportParamMapID == "17178")
                                        {
                                            if (!D3.ContainsKey(reportParamMapID))
                                                D3[reportParamMapID] = new List<string>();
                                            D3[reportParamMapID].Add(result);
                                        }
                                        else if (reportParamMapID == "17179")
                                        {
                                            if (!D4.ContainsKey(reportParamMapID))
                                                D4[reportParamMapID] = new List<string>();
                                            D4[reportParamMapID].Add(result);
                                        }
                                        else if (reportParamMapID == "17180")
                                        {
                                            if (!D5.ContainsKey(reportParamMapID))
                                                D5[reportParamMapID] = new List<string>();
                                            D5[reportParamMapID].Add(result);
                                        }
                                        else if (reportParamMapID == "17181")
                                        {
                                            if (!D6.ContainsKey(reportParamMapID))
                                                D6[reportParamMapID] = new List<string>();
                                            D6[reportParamMapID].Add(result);
                                        }

                                        else if (reportParamMapID == "17182")
                                        {
                                            if (!D7.ContainsKey(reportParamMapID))
                                                D7[reportParamMapID] = new List<string>();
                                            D7[reportParamMapID].Add(result);
                                        }
                                        else if (reportParamMapID == "17183")
                                        {
                                            if (!D8.ContainsKey(reportParamMapID))
                                                D8[reportParamMapID] = new List<string>();
                                            D8[reportParamMapID].Add(result);
                                        }
                                        else if (reportParamMapID == "17184")
                                        {
                                            if (!D9.ContainsKey(reportParamMapID))
                                                D9[reportParamMapID] = new List<string>();
                                            D9[reportParamMapID].Add(result);
                                        }
                                        else if (reportParamMapID == "17185")
                                        {
                                            if (!D10.ContainsKey(reportParamMapID))
                                                D10[reportParamMapID] = new List<string>();
                                            D10[reportParamMapID].Add(result);
                                        }
                                        else if (reportParamMapID == "17186")
                                        {
                                            if (!D11.ContainsKey(reportParamMapID))
                                                D11[reportParamMapID] = new List<string>();
                                            D11[reportParamMapID].Add(result);
                                        }
                                        else if (reportParamMapID == "17187")
                                        {
                                            if (!D12.ContainsKey(reportParamMapID))
                                                D12[reportParamMapID] = new List<string>();
                                            D12[reportParamMapID].Add(result);
                                        }

                                    }
                                }
                                else
                                {

                                    if (reportParamMapID == "17162") { Type = result; }
                                    else if (reportParamMapID == "17163") { Size = result; }
                                    else if (reportParamMapID == "17164") { Voltage = result; }
                                    else if (reportParamMapID == "17165") { Date = result; }
                                    else if (reportParamMapID == "17166") { RoomNo = result; }
                                    else if (reportParamMapID == "17167") { RoomTemp = result; }
                                    else if (reportParamMapID == "17168") { RefSpec = result; }
                                    else if (reportParamMapID == "17169") { TestMethodNo = result; }
                                    else if (reportParamMapID == "17170") { JobOrderNo = result; }
                                    else if (reportParamMapID == "17171") { WorkSheetNo = result; }
                                    else if (reportParamMapID == "17172") { RoomHumidity = result; }
                                    else if (reportParamMapID == "17173") { EquipmentNo = result; }

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
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center; width: 5%'>No.</td>");
                        sbDetails.Append("<td rowspan='2' colspan='2' style='background-color:#bebebe; text-align:center; width: 35%'>Items</td>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center; width: 10%'>Unit</td>");
                        sbDetails.Append("<td rowspan='2' style='background-color:#bebebe; text-align:center; width: 20%'>Specified</td>");
                        sbDetails.Append("<td colspan='6' style='background-color:#bebebe; text-align:center; width: 30%'>Measured</td>");
                        sbDetails.Append("</tr>");

                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'>1</td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'>2</td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'>3</td>");
                        sbDetails.Append("</tr>");

                        int RowNumber = 1;



                        // 1. Sample Weight
                        if (D1.ContainsKey("17176"))
                        {
                            var dataValues = D1["17176"];
                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "", "" };

                                while (inputParts.Length < 6)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");
                                sbDetails.Append($"<td>{RowNumber}</td>");
                                sbDetails.Append($"<td rowspan='12'>HCL Content</td>");
                                sbDetails.Append($"<td>Sample Weight</td>");
                                sbDetails.Append($"<td>{unit}</td>");
                                sbDetails.Append($"<td>{specified}</td>");
                                sbDetails.Append($"<td>{inputParts[0]}</td>");
                                sbDetails.Append($"<td>{inputParts[1]}</td>");
                                sbDetails.Append($"<td>{inputParts[2]}</td>");
                                //sbDetails.Append($"<td>{inputParts[3]}</td>");
                                //sbDetails.Append($"<td>{inputParts[4]}</td>");
                                //sbDetails.Append($"<td>{inputParts[5]}</td>");
                                sbDetails.Append("</tr>");
                                RowNumber++;
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append($"<td rowspan='12'>HCL Content</td>");
                            sbDetails.Append("<td>Sample Weight</td>");
                            sbDetails.Append("<td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // 2. Sodium Hydroxide
                        if (D2.ContainsKey("17177"))
                        {
                            var dataValues = D2["17177"];
                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "", "" };

                                while (inputParts.Length < 6)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");
                                sbDetails.Append($"<td>{RowNumber}</td>");
                                sbDetails.Append("<td>Sodium Hydroxide </td>");
                                sbDetails.Append($"<td>{unit}</td>");
                                sbDetails.Append($"<td>{specified}</td>");
                                sbDetails.Append($"<td>{inputParts[0]}</td>");
                                sbDetails.Append($"<td>{inputParts[1]}</td>");
                                sbDetails.Append($"<td>{inputParts[2]}</td>");
                                //sbDetails.Append($"<td>{inputParts[3]}</td>");
                                //sbDetails.Append($"<td>{inputParts[4]}</td>");
                                //sbDetails.Append($"<td>{inputParts[5]}</td>");
                                sbDetails.Append("</tr>");
                                RowNumber++;
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append("<td>Sodium Hydroxide</td>");
                            sbDetails.Append("<td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // 3. Nitric Acid
                        if (D3.ContainsKey("17178"))
                        {
                            var dataValues = D3["17178"];
                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "", "" };

                                while (inputParts.Length < 6)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");
                                sbDetails.Append($"<td>{RowNumber}</td>");
                                sbDetails.Append("<td>Nitric Acid</td>");
                                sbDetails.Append($"<td>{unit}</td>");
                                sbDetails.Append($"<td>{specified}</td>");
                                sbDetails.Append($"<td>{inputParts[0]}</td>");
                                sbDetails.Append($"<td>{inputParts[1]}</td>");
                                sbDetails.Append($"<td>{inputParts[2]}</td>");
                                //sbDetails.Append($"<td>{inputParts[3]}</td>");
                                //sbDetails.Append($"<td>{inputParts[4]}</td>");
                                //sbDetails.Append($"<td>{inputParts[5]}</td>");
                                sbDetails.Append("</tr>");
                                RowNumber++;
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append("<td>Nitric Acid</td>");
                            sbDetails.Append("<td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // 4. Silver nitrate
                        if (D4.ContainsKey("17179"))
                        {
                            var dataValues = D4["17179"];
                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "", "" };

                                while (inputParts.Length < 6)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");
                                sbDetails.Append($"<td>{RowNumber}</td>");
                                sbDetails.Append("<td>Silver nitrate</td>");
                                sbDetails.Append($"<td>{unit}</td>");
                                sbDetails.Append($"<td>{specified}</td>");
                                sbDetails.Append($"<td>{inputParts[0]}</td>");
                                sbDetails.Append($"<td>{inputParts[1]}</td>");
                                sbDetails.Append($"<td>{inputParts[2]}</td>");
                                //sbDetails.Append($"<td>{inputParts[3]}</td>");
                                //sbDetails.Append($"<td>{inputParts[4]}</td>");
                                //sbDetails.Append($"<td>{inputParts[5]}</td>");
                                sbDetails.Append("</tr>");
                                RowNumber++;
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append("<td>Silver nitrate</td>");
                            sbDetails.Append("<td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // 5. Nitrobenzene (toluene)
                        if (D5.ContainsKey("17180"))
                        {
                            var dataValues = D5["17180"];
                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "", "" };

                                while (inputParts.Length < 6)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");
                                sbDetails.Append($"<td>{RowNumber}</td>");
                                sbDetails.Append("<td>Nitrobenzene (toluene)</td>");
                                sbDetails.Append($"<td>{unit}</td>");
                                sbDetails.Append($"<td>{specified}</td>");
                                sbDetails.Append($"<td>{inputParts[0]}</td>");
                                sbDetails.Append($"<td>{inputParts[1]}</td>");
                                sbDetails.Append($"<td>{inputParts[2]}</td>");
                                //sbDetails.Append($"<td>{inputParts[3]}</td>");
                                //sbDetails.Append($"<td>{inputParts[4]}</td>");
                                //sbDetails.Append($"<td>{inputParts[5]}</td>");
                                sbDetails.Append("</tr>");
                                RowNumber++;
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append("<td>Nitrobenzene (toluene)</td>");
                            sbDetails.Append("<td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // 6. Ferric ammonium  sulphate
                        if (D6.ContainsKey("17181"))
                        {
                            var dataValues = D6["17181"];
                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "", "" };

                                while (inputParts.Length < 6)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");
                                sbDetails.Append($"<td>{RowNumber}</td>");
                                sbDetails.Append("<td>Ferric ammonium  sulphate</td>");
                                sbDetails.Append($"<td>{unit}</td>");
                                sbDetails.Append($"<td>{specified}</td>");
                                sbDetails.Append($"<td>{inputParts[0]}</td>");
                                sbDetails.Append($"<td>{inputParts[1]}</td>");
                                sbDetails.Append($"<td>{inputParts[2]}</td>");
                                //sbDetails.Append($"<td>{inputParts[3]}</td>");
                                //sbDetails.Append($"<td>{inputParts[4]}</td>");
                                //sbDetails.Append($"<td>{inputParts[5]}</td>");
                                sbDetails.Append("</tr>");
                                RowNumber++;
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append("<td>Ferric ammonium  sulphate</td>");
                            sbDetails.Append("<td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // 7. Burn Temperature
                        if (D7.ContainsKey("17182"))
                        {
                            var dataValues = D7["17182"];
                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "", "" };

                                while (inputParts.Length < 6)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");
                                sbDetails.Append($"<td>{RowNumber}</td>");
                                sbDetails.Append("<td>Burn Temperature</td>");
                                sbDetails.Append($"<td>{unit}</td>");
                                sbDetails.Append($"<td>{specified}</td>");
                                sbDetails.Append($"<td>{inputParts[0]}</td>");
                                sbDetails.Append($"<td>{inputParts[1]}</td>");
                                sbDetails.Append($"<td>{inputParts[2]}</td>");
                                //sbDetails.Append($"<td>{inputParts[3]}</td>");
                                //sbDetails.Append($"<td>{inputParts[4]}</td>");
                                //sbDetails.Append($"<td>{inputParts[5]}</td>");
                                sbDetails.Append("</tr>");
                                RowNumber++;
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append("<td>Burn Temperature</td>");
                            sbDetails.Append("<td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // 8. Time for Reaching Burn Temperature
                        if (D8.ContainsKey("17183"))
                        {
                            var dataValues = D8["17183"];
                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "", "" };

                                while (inputParts.Length < 6)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");
                                sbDetails.Append($"<td>{RowNumber}</td>");
                                sbDetails.Append("<td>Time for Reaching Burn Temperature</td>");
                                sbDetails.Append($"<td>{unit}</td>");
                                sbDetails.Append($"<td>{specified}</td>");
                                sbDetails.Append($"<td>{inputParts[0]}</td>");
                                sbDetails.Append($"<td>{inputParts[1]}</td>");
                                sbDetails.Append($"<td>{inputParts[2]}</td>");
                                //sbDetails.Append($"<td>{inputParts[3]}</td>");
                                //sbDetails.Append($"<td>{inputParts[4]}</td>");
                                //sbDetails.Append($"<td>{inputParts[5]}</td>");
                                sbDetails.Append("</tr>");
                                RowNumber++;
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append("<td>Time for Reaching Burn Temperature</td>");
                            sbDetails.Append("<td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // 9. Burn Time
                        if (D9.ContainsKey("17184"))
                        {
                            var dataValues = D9["17184"];
                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "", "" };

                                while (inputParts.Length < 6)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");
                                sbDetails.Append($"<td>{RowNumber}</td>");
                                sbDetails.Append("<td>Burn Time</td>");
                                sbDetails.Append($"<td>{unit}</td>");
                                sbDetails.Append($"<td>{specified}</td>");
                                sbDetails.Append($"<td>{inputParts[0]}</td>");
                                sbDetails.Append($"<td>{inputParts[1]}</td>");
                                sbDetails.Append($"<td>{inputParts[2]}</td>");
                                //sbDetails.Append($"<td>{inputParts[3]}</td>");
                                //sbDetails.Append($"<td>{inputParts[4]}</td>");
                                //sbDetails.Append($"<td>{inputParts[5]}</td>");
                                sbDetails.Append("</tr>");
                                RowNumber++;
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append("<td>Burn Time</td>");
                            sbDetails.Append("<td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        // 10. Ammonium Thiocyanate for the Tritation
                        if (D10.ContainsKey("17185"))
                        {
                            var dataValues = D10["17185"];
                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "", "" };

                                while (inputParts.Length < 6)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");
                                sbDetails.Append($"<td>{RowNumber}</td>");
                                sbDetails.Append("<td>Ammonium Thiocyanate for the Tritation</td>");
                                sbDetails.Append($"<td>{unit}</td>");
                                sbDetails.Append($"<td>{specified}</td>");
                                sbDetails.Append($"<td>{inputParts[0]}</td>");
                                sbDetails.Append($"<td>{inputParts[1]}</td>");
                                sbDetails.Append($"<td>{inputParts[2]}</td>");
                                //sbDetails.Append($"<td>{inputParts[3]}</td>");
                                //sbDetails.Append($"<td>{inputParts[4]}</td>");
                                //sbDetails.Append($"<td>{inputParts[5]}</td>");
                                sbDetails.Append("</tr>");
                                RowNumber++;
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append("<td>Ammonium Thiocyanate for the Tritation</td>");
                            sbDetails.Append("<td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }

                        if (D11.ContainsKey("17186"))
                        {
                            var dataValues = D11["17186"];
                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "", "" };

                                while (inputParts.Length < 6)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");
                                sbDetails.Append($"<td rowspan='2'>{RowNumber}</td>");
                                sbDetails.Append("<td rowspan='2'>(HCI) Content</td>");
                                sbDetails.Append($"<td>{unit}</td>");
                                sbDetails.Append($"<td>{specified}</td>");
                                sbDetails.Append($"<td>{inputParts[0]}</td>");
                                sbDetails.Append($"<td>{inputParts[1]}</td>");
                                sbDetails.Append($"<td>{inputParts[2]}</td>");
                                //sbDetails.Append($"<td>{inputParts[3]}</td>");
                                //sbDetails.Append($"<td>{inputParts[4]}</td>");
                                //sbDetails.Append($"<td>{inputParts[5]}</td>");
                                sbDetails.Append("</tr>");
                                RowNumber++;
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append("<td>(HCI) Content</td>");
                            sbDetails.Append("<td></td><td></td><td></td>");
                            sbDetails.Append("</tr>");
                            RowNumber++;
                        }


                        if (D12.ContainsKey("17187"))
                        {
                            var dataValues = D12["17187"];
                            if (dataValues.Count > 0)
                            {
                                string[] parts = dataValues[0].Split('|');
                                string unit = parts.Length > 0 ? parts[0] : "";
                                string specified = parts.Length > 1 ? parts[1] : "";
                                string[] inputParts = parts.Length > 2 ? parts[2].Split(',') : new string[] { "", "", "" };

                                while (inputParts.Length < 6)
                                    inputParts = inputParts.Append("").ToArray();

                                sbDetails.Append("<tr>");
                                //sbDetails.Append($"<td>{RowNumber}</td>");
                                //sbDetails.Append("<td>(HCI) Content</td>");
                                sbDetails.Append($"<td>{unit}</td>");
                                sbDetails.Append($"<td>{specified}</td>");
                                sbDetails.Append($"<td>{inputParts[0]}</td>");
                                sbDetails.Append($"<td>{inputParts[1]}</td>");
                                sbDetails.Append($"<td>{inputParts[2]}</td>");
                                //sbDetails.Append($"<td>{inputParts[3]}</td>");
                                //sbDetails.Append($"<td>{inputParts[4]}</td>");
                                //sbDetails.Append($"<td>{inputParts[5]}</td>");
                                sbDetails.Append("</tr>");
                                RowNumber++;
                            }
                        }
                        else
                        {
                            sbDetails.Append("<tr>");
                            sbDetails.Append($"<td>{RowNumber}</td>");
                            sbDetails.Append("<td>(HCI) Content</td>");
                            sbDetails.Append("<td></td><td></td><td></td>");
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
                        body = objEmail.PrepareMailBodyWith("TestRecord_AcidGasEmissionTest.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord_AcidGasEmissionTest-" + SampleID + ".htm");
                        File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_AcidGasEmissionTest-" + SampleID + ".htm"));
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