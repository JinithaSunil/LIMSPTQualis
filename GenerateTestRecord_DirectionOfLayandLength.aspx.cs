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
    public partial class GenerateTestRecord_DirectionOfLay_Length : System.Web.UI.Page
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
                        Dictionary<string, List<string>> ConductordiameterTestresults = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> ConductordiameterTestresultsNom = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> ConductordiameterStandardrequirement = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> ConductordiameterStandardrequirementNom = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> LayratiosofstrandedconductorsTestresult = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> LayratiosofstrandedconductorsTestresultNom = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> DirectionoflayTestresult = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> DirectionoflayStandardrequirment = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> LayratiosofstrandedconductorsStandard = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> LayratiosofstrandedconductorsStandardNom = new Dictionary<string, List<string>>();

                        //-----------------------------------------------------

                        Dictionary<string, List<string>> ConductordiameterTestresultsAdditional = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> ConductordiameterTestresultsNomAdditional = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> ConductordiameterStandardrequirementAdditional = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> ConductordiameterStandardrequirementNomAdditional = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> LayratiosofstrandedconductorsTestresultAdditional = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> LayratiosofstrandedconductorsTestresultNomAdditional = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> LayratiosofstrandedconductorsStandardAdditional = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> LayratiosofstrandedconductorsStandardNomAdditional = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> DirectionoflayTestresultAdditional = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> DirectionoflayStandardrequirementAdditional = new Dictionary<string, List<string>>();

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
                                if (reportParamMapID == "12802") // Equipment Number
                                {
                                    EquipmentNo = result;
                                    Console.WriteLine($"Equipment Number set to: {EquipmentNo}");
                                }
                                // Handle different data formats based on content and parameter ID
                                else if (result.Contains(";")) // Standard requirement format: min;max
                                {
                                    string[] standardParts = result.Split(';');
                                    if (standardParts.Length >= 2)
                                    {
                                        string minValue = standardParts[0].Trim();
                                        string maxValue = standardParts[1].Trim();
                                        string formattedStandard = $"{minValue};{maxValue}";

                                        if (reportParamMapID == "12777") // ConductordiameterStandardrequirementupper
                                        {
                                            if (!ConductordiameterStandardrequirement.ContainsKey(reportParamMapID))
                                                ConductordiameterStandardrequirement[reportParamMapID] = new List<string>();
                                            ConductordiameterStandardrequirement[reportParamMapID].Add(formattedStandard);
                                        }
                                        else if (reportParamMapID == "12779") // LayratiosofstrandedconductorsTestresultupper
                                        {
                                            if (!LayratiosofstrandedconductorsTestresult.ContainsKey(reportParamMapID))
                                                LayratiosofstrandedconductorsTestresult[reportParamMapID] = new List<string>();
                                            LayratiosofstrandedconductorsTestresult[reportParamMapID].Add(formattedStandard);
                                        }
                                        else if (reportParamMapID == "12783") // 
                                        {
                                            if (!LayratiosofstrandedconductorsStandard.ContainsKey(reportParamMapID))
                                                LayratiosofstrandedconductorsStandard[reportParamMapID] = new List<string>();
                                            LayratiosofstrandedconductorsStandard[reportParamMapID].Add(formattedStandard);
                                        }
                                        else if (reportParamMapID == "12787") // ConductordiameterStandardrequirementlower
                                        {
                                            if (!ConductordiameterStandardrequirementAdditional.ContainsKey(reportParamMapID))
                                                ConductordiameterStandardrequirementAdditional[reportParamMapID] = new List<string>();
                                            ConductordiameterStandardrequirementAdditional[reportParamMapID].Add(formattedStandard);
                                        }
                                        else if (reportParamMapID == "12789") // LayratiosofstrandedconductorsTestresultlower
                                        {
                                            if (!LayratiosofstrandedconductorsTestresultAdditional.ContainsKey(reportParamMapID))
                                                LayratiosofstrandedconductorsTestresultAdditional[reportParamMapID] = new List<string>();
                                            LayratiosofstrandedconductorsTestresultAdditional[reportParamMapID].Add(formattedStandard);
                                        }
                                        else if (reportParamMapID == "12821") // LayratiosofstrandedconductorsStandardlower
                                        {
                                            if (!LayratiosofstrandedconductorsStandardAdditional.ContainsKey(reportParamMapID))
                                                LayratiosofstrandedconductorsStandardAdditional[reportParamMapID] = new List<string>();
                                            LayratiosofstrandedconductorsStandardAdditional[reportParamMapID].Add(formattedStandard);
                                        }
                                    }
                                }
                                else if (result.Contains(",")) // Test result format: inp1,inp2,inp3,inp4,inp5,inp6
                                {
                                    string[] inputParts = result.Split(',');
                                    List<string> inputValues = new List<string>();

                                    foreach (string input in inputParts)
                                    {
                                        inputValues.Add(input.Trim());
                                    }

                                    string formattedInputs = string.Join(",", inputValues);


                                    if (reportParamMapID == "12775")
                                    {
                                        if (!ConductordiameterTestresults.ContainsKey(reportParamMapID))
                                            ConductordiameterTestresults[reportParamMapID] = new List<string>();
                                        ConductordiameterTestresults[reportParamMapID].Add(formattedInputs);
                                    }
                                    else if (reportParamMapID == "12785")
                                    {
                                        if (!ConductordiameterTestresultsAdditional.ContainsKey(reportParamMapID))
                                            ConductordiameterTestresultsAdditional[reportParamMapID] = new List<string>();
                                        ConductordiameterTestresultsAdditional[reportParamMapID].Add(formattedInputs);
                                    }

                                }
                                else
                                {

                                    if (reportParamMapID == "12776")
                                    {
                                        if (!ConductordiameterTestresultsNom.ContainsKey(reportParamMapID))
                                            ConductordiameterTestresultsNom[reportParamMapID] = new List<string>();
                                        ConductordiameterTestresultsNom[reportParamMapID].Add(result);
                                    }
                                    else if (reportParamMapID == "12778")
                                    {
                                        if (!ConductordiameterStandardrequirementNom.ContainsKey(reportParamMapID))
                                            ConductordiameterStandardrequirementNom[reportParamMapID] = new List<string>();
                                        ConductordiameterStandardrequirementNom[reportParamMapID].Add(result);
                                    }
                                    else if (reportParamMapID == "12780") // Lay ratios Nom test
                                    {
                                        if (!LayratiosofstrandedconductorsTestresultNom.ContainsKey(reportParamMapID))
                                            LayratiosofstrandedconductorsTestresultNom[reportParamMapID] = new List<string>();
                                        LayratiosofstrandedconductorsTestresultNom[reportParamMapID].Add(result);
                                    }

                                    else if (reportParamMapID == "12781") // Direction of lay test
                                    {
                                        if (!DirectionoflayTestresult.ContainsKey(reportParamMapID))
                                            DirectionoflayTestresult[reportParamMapID] = new List<string>();
                                        DirectionoflayTestresult[reportParamMapID].Add(result);
                                    }
                                    else if (reportParamMapID == "12782") // Direction of lay standard requirement
                                    {
                                        if (!DirectionoflayStandardrequirment.ContainsKey(reportParamMapID))
                                            DirectionoflayStandardrequirment[reportParamMapID] = new List<string>();
                                        DirectionoflayStandardrequirment[reportParamMapID].Add(result);
                                    }
                                    else if (reportParamMapID == "12784") // standard requirement
                                    {
                                        if (!LayratiosofstrandedconductorsStandardNom.ContainsKey(reportParamMapID))
                                            LayratiosofstrandedconductorsStandardNom[reportParamMapID] = new List<string>();
                                        LayratiosofstrandedconductorsStandardNom[reportParamMapID].Add(result);
                                    }
                                    else if (reportParamMapID == "12786") // ConductordiameterTestresultsNomlower
                                    {
                                        if (!ConductordiameterTestresultsNomAdditional.ContainsKey(reportParamMapID))
                                            ConductordiameterTestresultsNomAdditional[reportParamMapID] = new List<string>();
                                        ConductordiameterTestresultsNomAdditional[reportParamMapID].Add(result);
                                    }
                                    else if (reportParamMapID == "12788") // ConductordiameterStandardrequirementNomlower
                                    {
                                        if (!ConductordiameterStandardrequirementNomAdditional.ContainsKey(reportParamMapID))
                                            ConductordiameterStandardrequirementNomAdditional[reportParamMapID] = new List<string>();
                                        ConductordiameterStandardrequirementNomAdditional[reportParamMapID].Add(result);
                                    }
                                    else if (reportParamMapID == "12790") // LayratiosofstrandedconductorsTestresultNomlower
                                    {
                                        if (!LayratiosofstrandedconductorsTestresultNomAdditional.ContainsKey(reportParamMapID))
                                            LayratiosofstrandedconductorsTestresultNomAdditional[reportParamMapID] = new List<string>();
                                        LayratiosofstrandedconductorsTestresultNomAdditional[reportParamMapID].Add(result);
                                    }
                                    else if (reportParamMapID == "12819") // DirectionoflayTestresultlower
                                    {
                                        if (!DirectionoflayTestresultAdditional.ContainsKey(reportParamMapID))
                                            DirectionoflayTestresultAdditional[reportParamMapID] = new List<string>();
                                        DirectionoflayTestresultAdditional[reportParamMapID].Add(result);
                                    }
                                    else if (reportParamMapID == "12820") // DirectionoflayStandardrequirmentlower
                                    {
                                        if (!DirectionoflayStandardrequirementAdditional.ContainsKey(reportParamMapID))
                                            DirectionoflayStandardrequirementAdditional[reportParamMapID] = new List<string>();
                                        DirectionoflayStandardrequirementAdditional[reportParamMapID].Add(result);
                                    }
                                    else if (reportParamMapID == "12822") // LayratiosofstrandedconductorsStandardNomlower
                                    {
                                        if (!LayratiosofstrandedconductorsStandardNomAdditional.ContainsKey(reportParamMapID))
                                            LayratiosofstrandedconductorsStandardNomAdditional[reportParamMapID] = new List<string>();
                                        LayratiosofstrandedconductorsStandardNomAdditional[reportParamMapID].Add(result);
                                    }

                                    // Handle single values
                                    else if (reportParamMapID == "12791") { Type = result; }
                                    else if (reportParamMapID == "12792") { Size = result; }
                                    else if (reportParamMapID == "12793") { Voltage = result; }
                                    else if (reportParamMapID == "12794") { Date = result; }
                                    else if (reportParamMapID == "12795") { RoomNo = result; }
                                    else if (reportParamMapID == "12796") { RoomTemp = result; }
                                    else if (reportParamMapID == "12797") { RefSpec = result; }
                                    else if (reportParamMapID == "12798") { TestMethodNo = result; }
                                    else if (reportParamMapID == "12799") { JobOrderNo = result; }
                                    else if (reportParamMapID == "12800") { WorkSheetNo = result; }
                                    else if (reportParamMapID == "12801") { RoomHumidity = result; }

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
                           );// Remove the outer table with border and create a single container
                        sbDetails.Append(@"<div style='border:2 px solid black; width:100%;'>");

                        // First Table - Remove individual border
                        sbDetails.Append(@"<table class='tab1' cellpadding='3' cellspacing='0' style='width:100%; border-collapse: collapse;'>");

                        // Headers Row 1
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td rowspan='1' style='background-color:#bebebe; text-align:center; vertical-align:middle;'><strong>Layer:</strong></td>");
                        sbDetails.Append("<td colspan='7' style='background-color:#bebebe; text-align:center;'><strong>Conductor diameter (mm)</strong></td>");
                        sbDetails.Append("<td colspan='3' style='background-color:#bebebe; text-align:center;'><strong>Lay ratios of stranded conductors (x D)</strong></td>");
                        sbDetails.Append("<td rowspan='1' style='background-color:#bebebe; text-align:center; vertical-align:middle;'><strong>Direction of lay</strong></td>");
                        sbDetails.Append("</tr>");

                        // Row: Test Result
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td style='text-align:left;'><strong>Test Result</strong></td>");

                        // Conductor diameter test results (12775)
                        if (ConductordiameterTestresults.ContainsKey("12775"))
                        {
                            var values = ConductordiameterTestresults["12775"].FirstOrDefault()?.Split(',') ?? new string[6];
                            for (int i = 0; i < 6; i++)
                            {
                                string val = i < values.Length ? values[i] : "";
                                sbDetails.Append($"<td style='text-align:center;'>{val}</td>");
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 6; i++) sbDetails.Append("<td></td>");
                        }

                        // Nominal conductor diameter (12776)
                        if (ConductordiameterTestresultsNom.ContainsKey("12776"))
                        {
                            string nomVal = ConductordiameterTestresultsNom["12776"].FirstOrDefault() ?? "";
                            sbDetails.Append($"<td style='text-align:center;'>Nom:{nomVal}</td>");
                        }
                        else sbDetails.Append("<td></td>");

                        // Lay ratios min/max (12779)
                        if (LayratiosofstrandedconductorsTestresult.ContainsKey("12779"))
                        {
                            var minMax = LayratiosofstrandedconductorsTestresult["12779"].FirstOrDefault()?.Split(';') ?? new string[2];
                            sbDetails.Append($"<td style='text-align:center;'>Min:{(minMax.Length > 0 ? minMax[0] : "")}</td>");
                            sbDetails.Append($"<td style='text-align:center;'>Max:{(minMax.Length > 1 ? minMax[1] : "")}</td>");
                        }
                        else
                        {
                            sbDetails.Append("<td></td><td></td>");
                        }

                        // Lay ratio nominal (12780)
                        if (LayratiosofstrandedconductorsTestresultNom.ContainsKey("12780"))
                        {
                            string nomVal = LayratiosofstrandedconductorsTestresultNom["12780"].FirstOrDefault() ?? "";
                            sbDetails.Append($"<td style='text-align:center;'>Nom:{nomVal}</td>");
                        }
                        else sbDetails.Append("<td></td>");

                        // Direction of lay test result (12781)
                        if (DirectionoflayTestresult.ContainsKey("12781"))
                        {
                            string val = DirectionoflayTestresult["12781"].FirstOrDefault() ?? "";
                            sbDetails.Append($"<td style='text-align:center;'>{val}</td>");
                        }
                        else sbDetails.Append("<td></td>");

                        sbDetails.Append("</tr>");

                        // Row: Standard Requirement
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td style='text-align:left;'><strong>Standard Requirement</strong></td>");

                        // ConductordiameterStandardrequirement (min;max) 12777 - 2 values
                        if (ConductordiameterStandardrequirement.ContainsKey("12777"))
                        {
                            var minMax = ConductordiameterStandardrequirement["12777"].FirstOrDefault()?.Split(';') ?? new string[2];
                            sbDetails.Append($"<td  colspan='3' style='text-align:center;'>Min:{(minMax.Length > 0 ? minMax[0] : "")}</td>");
                            sbDetails.Append($"<td colspan='3' style='text-align:center;'>Max:{(minMax.Length > 1 ? minMax[1] : "")}</td>");
                        }
                        else
                        {
                            sbDetails.Append("<td colspan='6'></td>");
                        }

                        // Nominal conductor diameter (12778)
                        if (ConductordiameterStandardrequirementNom.ContainsKey("12778"))
                        {
                            string nomVal = ConductordiameterStandardrequirementNom["12778"].FirstOrDefault() ?? "";
                            sbDetails.Append($"<td style='text-align:center;'>Nom:{nomVal}</td>");
                        }
                        else sbDetails.Append("<td></td>");

                        if (LayratiosofstrandedconductorsStandard.ContainsKey("12783"))
                        {
                            var minMax = LayratiosofstrandedconductorsStandard["12783"].FirstOrDefault()?.Split(';') ?? new string[2];
                            sbDetails.Append($"<td style='text-align:center;'>Min:{(minMax.Length > 0 ? minMax[0] : "")}</td>");
                            sbDetails.Append($"<td style='text-align:center;'>Max:{(minMax.Length > 1 ? minMax[1] : "")}</td>");
                        }
                        else
                        {
                            sbDetails.Append("<td></td><td></td>");
                        }

                        // Lay ratio nominal (12780)
                        if (LayratiosofstrandedconductorsStandardNom.ContainsKey("12784"))
                        {
                            string nomVal = LayratiosofstrandedconductorsStandardNom["12784"].FirstOrDefault() ?? "";
                            sbDetails.Append($"<td style='text-align:center;'>Nom:{nomVal}</td>");
                        }
                        else sbDetails.Append("<td></td>");

                        // Direction of lay standard requirement (12782)
                        if (DirectionoflayStandardrequirment.ContainsKey("12782"))
                        {
                            string val = DirectionoflayStandardrequirment["12782"].FirstOrDefault() ?? "";
                            sbDetails.Append($"<td style='text-align:center;'>{val}</td>");
                        }
                        else sbDetails.Append("<td></td>");

                        sbDetails.Append("</tr>");
                        sbDetails.Append("</table>");

                        // Add some spacing between tables
                        sbDetails.Append("<br/>");

                        // Second Table - Remove individual border
                        sbDetails.Append(@"<table class='tab1' cellpadding='3' cellspacing='0' style='width:100%; border-collapse: collapse;'>");

                        // Headers (same as first table)
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td rowspan='1' style='background-color:#bebebe; text-align:center; vertical-align:middle;'><strong>Layer:</strong></td>");
                        sbDetails.Append("<td colspan='7' style='background-color:#bebebe; text-align:center;'><strong>Conductor diameter (mm)</strong></td>");
                        sbDetails.Append("<td colspan='3' style='background-color:#bebebe; text-align:center;'><strong>Lay ratios of stranded conductors (x D)</strong></td>");
                        sbDetails.Append("<td rowspan='1' style='background-color:#bebebe; text-align:center; vertical-align:middle;'><strong>Direction of lay</strong></td>");
                        sbDetails.Append("</tr>");

                        // Row: Test Result
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td style='text-align:left;'><strong>Test Result</strong></td>");

                        // Conductor diameter test results (Additional)
                        if (ConductordiameterTestresultsAdditional.ContainsKey("12785"))
                        {
                            var values = ConductordiameterTestresultsAdditional["12785"].FirstOrDefault()?.Split(',') ?? new string[6];
                            for (int i = 0; i < 6; i++)
                            {
                                string val = i < values.Length ? values[i] : "";
                                sbDetails.Append($"<td style='text-align:center;'>{val}</td>");
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 6; i++) sbDetails.Append("<td></td>");
                        }

                        // Nominal conductor diameter (Additional)
                        if (ConductordiameterTestresultsNomAdditional.ContainsKey("12786"))
                        {
                            string nomVal = ConductordiameterTestresultsNomAdditional["12786"].FirstOrDefault() ?? "";
                            sbDetails.Append($"<td style='text-align:center;'>Nom:{nomVal}</td>");
                        }
                        else sbDetails.Append("<td></td>");

                        // Lay ratios min/max (Additional)
                        if (LayratiosofstrandedconductorsTestresultAdditional.ContainsKey("12789"))
                        {
                            var minMax = LayratiosofstrandedconductorsTestresultAdditional["12789"].FirstOrDefault()?.Split(';') ?? new string[2];
                            sbDetails.Append($"<td style='text-align:center;'>Min:{(minMax.Length > 0 ? minMax[0] : "")}</td>");
                            sbDetails.Append($"<td style='text-align:center;'>Max:{(minMax.Length > 1 ? minMax[1] : "")}</td>");
                        }
                        else
                        {
                            sbDetails.Append("<td></td><td></td>");
                        }

                        // Lay ratio nominal (Additional)
                        if (LayratiosofstrandedconductorsTestresultNomAdditional.ContainsKey("12790"))
                        {
                            string nomVal = LayratiosofstrandedconductorsTestresultNomAdditional["12790"].FirstOrDefault() ?? "";
                            sbDetails.Append($"<td style='text-align:center;'>Nom:{nomVal}</td>");
                        }
                        else sbDetails.Append("<td></td>");

                        // Direction of lay test result (Additional)
                        if (DirectionoflayTestresultAdditional.ContainsKey("12819"))
                        {
                            string val = DirectionoflayTestresultAdditional["12819"].FirstOrDefault() ?? "";
                            sbDetails.Append($"<td style='text-align:center;'>{val}</td>");
                        }
                        else sbDetails.Append("<td></td>");

                        sbDetails.Append("</tr>");

                        // Row: Standard Requirement
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td style='text-align:left;'><strong>Standard Requirement</strong></td>");

                        if (ConductordiameterStandardrequirementAdditional.ContainsKey("12787"))
                        {
                            var minMax = ConductordiameterStandardrequirementAdditional["12787"].FirstOrDefault()?.Split(';') ?? new string[2];
                            sbDetails.Append($"<td colspan='3' style='text-align:center;'>Min:{(minMax.Length > 0 ? minMax[0] : "")}</td>");
                            sbDetails.Append($"<td colspan='3' style='text-align:center;'>Max:{(minMax.Length > 1 ? minMax[1] : "")}</td>");
                        }
                        else
                        {
                            sbDetails.Append("<td colspan='6'></td>");
                        }

                        // Nominal conductor diameter (Additional)
                        if (ConductordiameterStandardrequirementNomAdditional.ContainsKey("12788"))
                        {
                            string nomVal = ConductordiameterStandardrequirementNomAdditional["12788"].FirstOrDefault() ?? "";
                            sbDetails.Append($"<td style='text-align:center;'>Nom:{nomVal}</td>");
                        }
                        else sbDetails.Append("<td></td>");

                        // Lay ratios standard requirement (Additional)
                        if (LayratiosofstrandedconductorsStandardAdditional.ContainsKey("12821"))
                        {
                            var minMax = LayratiosofstrandedconductorsStandardAdditional["12821"].FirstOrDefault()?.Split(';') ?? new string[2];
                            sbDetails.Append($"<td style='text-align:center;'>Min:{(minMax.Length > 0 ? minMax[0] : "")}</td>");
                            sbDetails.Append($"<td style='text-align:center;'>Max:{(minMax.Length > 1 ? minMax[1] : "")}</td>");
                        }
                        else
                        {
                            sbDetails.Append("<td></td><td></td>");
                        }

                        // Lay ratio nominal standard requirement (Additional)
                        if (LayratiosofstrandedconductorsStandardNomAdditional.ContainsKey("12822"))
                        {
                            string nomVal = LayratiosofstrandedconductorsStandardNomAdditional["12822"].FirstOrDefault() ?? "";
                            sbDetails.Append($"<td style='text-align:center;'>Nom:{nomVal}</td>");
                        }
                        else sbDetails.Append("<td></td>");

                        // Direction of lay standard requirement (Additional)
                        if (DirectionoflayStandardrequirementAdditional.ContainsKey("12820"))
                        {
                            string val = DirectionoflayStandardrequirementAdditional["12820"].FirstOrDefault() ?? "";
                            sbDetails.Append($"<td style='text-align:center;'>{val}</td>");
                        }
                        else sbDetails.Append("<td></td>");

                        sbDetails.Append("</tr>");
                        sbDetails.Append("</table>");

                        // Close the container div
                        sbDetails.Append("</div>");

                        sbFooter.Append(@"
<table style='width:100%; font-family: Times New Roman; font-size: 11px;' class='tab1' border='1' cellspacing='0' cellpadding='4'>
    <tr>
        <td colspan='4' style='text-align: center; font-weight: bold;'>Witnessed By</td>
    </tr>
    <tr>
        <td style='text-align: left; width: 25%;'>
            Tested by:<br /><br /><br />
        </td>
        <td style='text-align: left; width: 25%;'>
            Checked by:<br /><br /><br />
        </td>
        <td style='text-align: left; width: 25%;'>
            Witnessed by:<br /><br /><br />
        </td>
        <td style='text-align: left; width: 25%;'>
            <br /><br /><br />
        </td>
    </tr>
</table>");


                        // Generate the HTML file
                        body = objEmail.PrepareMailBodyWith("TestRecord_DirectionOflayandlength.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord_DirectionOflayandlength -" + SampleID + ".htm");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_DirectionOflayandlength -" + SampleID + ".htm"));
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