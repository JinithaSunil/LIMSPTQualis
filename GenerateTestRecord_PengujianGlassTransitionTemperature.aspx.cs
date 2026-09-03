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
	public partial class GenerateTestRecord_PengujianGlassTransitionTemperature : System.Web.UI.Page
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
                        Dictionary<string, List<string>> Dimension = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Thickness = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Width = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Length = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> WeightOfSample = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> ConditionOfSpecimen = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Temperature = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> DurationTest = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> WeightOfSampleAfterCondition = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> ThermosetPolymer = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> WeightOfSampleAfterTest = new Dictionary<string, List<string>>();


                        // Initialize specified values



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

                                        if (reportParamMapID == "12620") // Dimension
                                        {
                                            if (!Dimension.ContainsKey(paramName))
                                                Dimension[paramName] = new List<string>();
                                            Dimension[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "12610") // Thickness
                                        {
                                            if (!Thickness.ContainsKey(paramName))
                                                Thickness[paramName] = new List<string>();
                                            Thickness[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "12611") // Width
                                        {
                                            if (!Width.ContainsKey(paramName))
                                                Width[paramName] = new List<string>();
                                            Width[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "12612") // length
                                        {
                                            if (!Length.ContainsKey(paramName))
                                                Length[paramName] = new List<string>();
                                            Length[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "12613") // WeightOfSample
                                        {
                                            if (!WeightOfSample.ContainsKey(paramName))
                                                WeightOfSample[paramName] = new List<string>();
                                            WeightOfSample[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "12614") // ConditionOfSpecimen
                                        {
                                            if (!ConditionOfSpecimen.ContainsKey(paramName))
                                                ConditionOfSpecimen[paramName] = new List<string>();
                                            ConditionOfSpecimen[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "12615") // Temperature
                                        {
                                            if (!Temperature.ContainsKey(paramName))
                                                Temperature[paramName] = new List<string>();
                                            Temperature[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "12616") // DurationTest
                                        {
                                            if (!DurationTest.ContainsKey(paramName))
                                                DurationTest[paramName] = new List<string>();
                                            DurationTest[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "12617") // WeightOfSampleAfterCondition
                                        {
                                            if (!WeightOfSampleAfterCondition.ContainsKey(paramName))
                                                WeightOfSampleAfterCondition[paramName] = new List<string>();
                                            WeightOfSampleAfterCondition[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "12618") // ThermosetPolymer
                                        {
                                            if (!ThermosetPolymer.ContainsKey(paramName))
                                                ThermosetPolymer[paramName] = new List<string>();
                                            ThermosetPolymer[paramName].Add(formattedData);
                                        }
                                        else if (reportParamMapID == "12619") // WeightOfSampleAfterTest
                                        {
                                            if (!WeightOfSampleAfterTest.ContainsKey(paramName))
                                                WeightOfSampleAfterTest[paramName] = new List<string>();
                                            WeightOfSampleAfterTest[paramName].Add(formattedData);
                                        }
                                    }
                                }
                                else // Non-measurement data (simple key-value pairs)
                                {

                                    if (reportParamMapID == "12621") { Type = result; }
                                    else if (reportParamMapID == "12622") { Size = result; }
                                    else if (reportParamMapID == "12623") { Grade = result; } // voltage value in grade
                                    else if (reportParamMapID == "12627") { RefSpec = result; }
                                    else if (reportParamMapID == "12628") { TestMethodNo = result; }
                                    else if (reportParamMapID == "12629") { JobOrderNo = result; }
                                    else if (reportParamMapID == "12630") { WorkSheetNo = result; }
                                    else if (reportParamMapID == "12625") { RoomNo = result; }
                                    else if (reportParamMapID == "12631") { RoomHumidity = result; }
                                    else if (reportParamMapID == "12626") { RoomTemp = result; }
                                    else if (reportParamMapID == "12624") { Date = result; }
                                    else if (reportParamMapID == "12632") { EquipmentNo = result; }
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
                                 <td>Garade</td>
                                <td>" + Grade + "</td>" +
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
                        }
                        sbDetails.Append(@"<table class='tab1' border='1' cellpadding='3' cellspacing='0' style='width:100%; border-collapse: collapse;'>");

                        // Header row
                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>No</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>Item test</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>Unit</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>Specified</strong></td>");
                        sbDetails.Append("<td style='background-color:#bebebe; text-align:center;'><strong>Value test</strong></td>");
                        sbDetails.Append("</tr>");


                        int mainRowNumber = 1;

                        // Group 1: Dimension measurements
                        if (Dimension.Count > 0 || Thickness.Count > 0 || Width.Count > 0 || Length.Count > 0 || WeightOfSample.Count > 0)
                        {
                            bool isFirstDimensionRow = true;

                            // Dimension
                            if (Dimension.Count > 0)
                            {
                                foreach (var entry in Dimension)
                                {
                                    var dataValues = entry.Value;
                                    if (dataValues.Count > 0)
                                    {
                                        string[] parts = dataValues[0].Split('|');
                                        string unit = parts.Length > 0 ? parts[0] : "";
                                        string specified = parts.Length > 1 ? parts[1] : "";
                                        string valueTest = parts.Length > 2 ? parts[2] : "";

                                        sbDetails.Append("<tr>");
                                        if (isFirstDimensionRow)
                                        {
                                            sbDetails.Append($"<td rowspan='5' style='text-align:center; vertical-align:middle;'>{mainRowNumber}</td>");
                                            isFirstDimensionRow = false;
                                        }
                                        sbDetails.Append($"<td>Dimension</td>");
                                        sbDetails.Append($"<td style='text-align:center;'>{unit}</td>");
                                        sbDetails.Append($"<td style='text-align:center;'>{specified}</td>");
                                        sbDetails.Append($"<td style='text-align:center;'>{valueTest}</td>");
                                        sbDetails.Append("</tr>");
                                    }
                                }
                            }

                            // Thickness
                            if (Thickness.Count > 0)
                            {
                                foreach (var entry in Thickness)
                                {
                                    var dataValues = entry.Value;
                                    if (dataValues.Count > 0)
                                    {
                                        string[] parts = dataValues[0].Split('|');
                                        string unit = parts.Length > 0 ? parts[0] : "mm";
                                        string specified = parts.Length > 1 ? parts[1] : "-";
                                        string valueTest = parts.Length > 2 ? parts[2] : "";

                                        sbDetails.Append("<tr>");
                                        sbDetails.Append($"<td>Thickness</td>");
                                        sbDetails.Append($"<td style='text-align:center;'>{unit}</td>");
                                        sbDetails.Append($"<td style='text-align:center;'>{specified}</td>");
                                        sbDetails.Append($"<td style='text-align:center;'>{valueTest}</td>");
                                        sbDetails.Append("</tr>");
                                    }
                                }
                            }

                            // Width
                            if (Width.Count > 0)
                            {
                                foreach (var entry in Width)
                                {
                                    var dataValues = entry.Value;
                                    if (dataValues.Count > 0)
                                    {
                                        string[] parts = dataValues[0].Split('|');
                                        string unit = parts.Length > 0 ? parts[0] : "mm";
                                        string specified = parts.Length > 1 ? parts[1] : "-";
                                        string valueTest = parts.Length > 2 ? parts[2] : "";

                                        sbDetails.Append("<tr>");
                                        sbDetails.Append($"<td>Width</td>");
                                        sbDetails.Append($"<td style='text-align:center;'>{unit}</td>");
                                        sbDetails.Append($"<td style='text-align:center;'>{specified}</td>");
                                        sbDetails.Append($"<td style='text-align:center;'>{valueTest}</td>");
                                        sbDetails.Append("</tr>");
                                    }
                                }
                            }

                            // Length
                            if (Length.Count > 0)
                            {
                                foreach (var entry in Length)
                                {
                                    var dataValues = entry.Value;
                                    if (dataValues.Count > 0)
                                    {
                                        string[] parts = dataValues[0].Split('|');
                                        string unit = parts.Length > 0 ? parts[0] : "mm";
                                        string specified = parts.Length > 1 ? parts[1] : "-";
                                        string valueTest = parts.Length > 2 ? parts[2] : "";

                                        sbDetails.Append("<tr>");
                                        sbDetails.Append($"<td>Length</td>");
                                        sbDetails.Append($"<td style='text-align:center;'>{unit}</td>");
                                        sbDetails.Append($"<td style='text-align:center;'>{specified}</td>");
                                        sbDetails.Append($"<td style='text-align:center;'>{valueTest}</td>");
                                        sbDetails.Append("</tr>");
                                    }
                                }
                            }

                            // Weight of sample
                            if (WeightOfSample.Count > 0)
                            {
                                foreach (var entry in WeightOfSample)
                                {
                                    var dataValues = entry.Value;
                                    if (dataValues.Count > 0)
                                    {
                                        string[] parts = dataValues[0].Split('|');
                                        string unit = parts.Length > 0 ? parts[0] : "gr";
                                        string specified = parts.Length > 1 ? parts[1] : "-";
                                        string valueTest = parts.Length > 2 ? parts[2] : "";

                                        sbDetails.Append("<tr>");
                                        sbDetails.Append($"<td>Weight of sample</td>");
                                        sbDetails.Append($"<td style='text-align:center;'>{unit}</td>");
                                        sbDetails.Append($"<td style='text-align:center;'>{specified}</td>");
                                        sbDetails.Append($"<td style='text-align:center;'>{valueTest}</td>");
                                        sbDetails.Append("</tr>");
                                    }
                                }
                            }

                            mainRowNumber++;
                        }

                        // Group 2: Condition of specimen and related tests
                        if (ConditionOfSpecimen.Count > 0 || Temperature.Count > 0 || DurationTest.Count > 0 || WeightOfSampleAfterCondition.Count > 0)
                        {
                            bool isFirstConditionRow = true;
                            int conditionRowCount = 0;

                            // Count total rows for this group
                            if (ConditionOfSpecimen.Count > 0) conditionRowCount++;
                            if (Temperature.Count > 0) conditionRowCount++;
                            if (DurationTest.Count > 0) conditionRowCount++;
                            if (WeightOfSampleAfterCondition.Count > 0) conditionRowCount++;

                            // Condition of specimen
                            if (ConditionOfSpecimen.Count > 0)
                            {
                                foreach (var entry in ConditionOfSpecimen)
                                {
                                    var dataValues = entry.Value;
                                    if (dataValues.Count > 0)
                                    {
                                        string[] parts = dataValues[0].Split('|');
                                        string unit = parts.Length > 0 ? parts[0] : "";
                                        string specified = parts.Length > 1 ? parts[1] : "-";
                                        string valueTest = parts.Length > 2 ? parts[2] : "";

                                        sbDetails.Append("<tr>");
                                        if (isFirstConditionRow)
                                        {
                                            sbDetails.Append($"<td rowspan='{conditionRowCount}' style='text-align:center; vertical-align:middle;'>{mainRowNumber}</td>");
                                            isFirstConditionRow = false;
                                        }
                                        sbDetails.Append($"<td>Condition of specimen :</td>");
                                        sbDetails.Append($"<td style='text-align:center;'>{unit}</td>");
                                        sbDetails.Append($"<td style='text-align:center;'>{specified}</td>");
                                        sbDetails.Append($"<td style='text-align:center;'>{valueTest}</td>");
                                        sbDetails.Append("</tr>");
                                    }
                                }
                            }

                            // Temperature
                            if (Temperature.Count > 0)
                            {
                                foreach (var entry in Temperature)
                                {
                                    var dataValues = entry.Value;
                                    if (dataValues.Count > 0)
                                    {
                                        string[] parts = dataValues[0].Split('|');
                                        string unit = parts.Length > 0 ? parts[0] : "°C";
                                        string specified = parts.Length > 1 ? parts[1] : "";
                                        string valueTest = parts.Length > 2 ? parts[2] : "";

                                        sbDetails.Append("<tr>");
                                        sbDetails.Append($"<td>Temperature</td>");
                                        sbDetails.Append($"<td style='text-align:center;'>{unit}</td>");
                                        sbDetails.Append($"<td style='text-align:center;'>{specified}</td>");
                                        sbDetails.Append($"<td style='text-align:center;'>{valueTest}</td>");
                                        sbDetails.Append("</tr>");
                                    }
                                }
                            }

                            // Duration test
                            if (DurationTest.Count > 0)
                            {
                                foreach (var entry in DurationTest)
                                {
                                    var dataValues = entry.Value;
                                    if (dataValues.Count > 0)
                                    {
                                        string[] parts = dataValues[0].Split('|');
                                        string unit = parts.Length > 0 ? parts[0] : "hours";
                                        string specified = parts.Length > 1 ? parts[1] : "";
                                        string valueTest = parts.Length > 2 ? parts[2] : "";

                                        sbDetails.Append("<tr>");
                                        sbDetails.Append($"<td>Duration test</td>");
                                        sbDetails.Append($"<td style='text-align:center;'>{unit}</td>");
                                        sbDetails.Append($"<td style='text-align:center;'>{specified}</td>");
                                        sbDetails.Append($"<td style='text-align:center;'>{valueTest}</td>");
                                        sbDetails.Append("</tr>");
                                    }
                                }
                            }

                            // Weight of sample after condition
                            if (WeightOfSampleAfterCondition.Count > 0)
                            {
                                foreach (var entry in WeightOfSampleAfterCondition)
                                {
                                    var dataValues = entry.Value;
                                    if (dataValues.Count > 0)
                                    {
                                        string[] parts = dataValues[0].Split('|');
                                        string unit = parts.Length > 0 ? parts[0] : "gr";
                                        string specified = parts.Length > 1 ? parts[1] : "-";
                                        string valueTest = parts.Length > 2 ? parts[2] : "";

                                        sbDetails.Append("<tr>");
                                        sbDetails.Append($"<td>Weight of sample after condition</td>");
                                        sbDetails.Append($"<td style='text-align:center;'>{unit}</td>");
                                        sbDetails.Append($"<td style='text-align:center;'>{specified}</td>");
                                        sbDetails.Append($"<td style='text-align:center;'>{valueTest}</td>");
                                        sbDetails.Append("</tr>");
                                    }
                                }
                            }

                            mainRowNumber++;
                        }

                        // Group 3: Thermoset Polymer and Weight of sample after test
                        if (ThermosetPolymer.Count > 0 || WeightOfSampleAfterTest.Count > 0)
                        {
                            bool isFirstThermosetRow = true;
                            int thermosetRowCount = 0;

                            // Count total rows for this group
                            if (ThermosetPolymer.Count > 0) thermosetRowCount++;
                            if (WeightOfSampleAfterTest.Count > 0) thermosetRowCount++;

                            // Thermoset Polymer
                            if (ThermosetPolymer.Count > 0)
                            {
                                foreach (var entry in ThermosetPolymer)
                                {
                                    var dataValues = entry.Value;
                                    if (dataValues.Count > 0)
                                    {
                                        string[] parts = dataValues[0].Split('|');
                                        string unit = parts.Length > 0 ? parts[0] : "°C";
                                        string specified = parts.Length > 1 ? parts[1] : "Min.\nMax.";
                                        string valueTest = parts.Length > 2 ? parts[2] : "";

                                        sbDetails.Append("<tr>");
                                        if (isFirstThermosetRow)
                                        {
                                            sbDetails.Append($"<td rowspan='{thermosetRowCount}' style='text-align:center; vertical-align:middle;'>{mainRowNumber}</td>");
                                            isFirstThermosetRow = false;
                                        }
                                        sbDetails.Append($"<td>Thermoset Polymer</td>");
                                        sbDetails.Append($"<td style='text-align:center;'>{unit}</td>");
                                        sbDetails.Append($"<td style='text-align:center;'>{specified}</td>");
                                        sbDetails.Append($"<td style='text-align:center;'>{valueTest}</td>");
                                        sbDetails.Append("</tr>");
                                    }
                                }
                            }

                            // Weight of sample after test
                            if (WeightOfSampleAfterTest.Count > 0)
                            {
                                foreach (var entry in WeightOfSampleAfterTest)
                                {
                                    var dataValues = entry.Value;
                                    if (dataValues.Count > 0)
                                    {
                                        string[] parts = dataValues[0].Split('|');
                                        string unit = parts.Length > 0 ? parts[0] : "gr";
                                        string specified = parts.Length > 1 ? parts[1] : "-";
                                        string valueTest = parts.Length > 2 ? parts[2] : "";

                                        sbDetails.Append("<tr>");
                                        sbDetails.Append($"<td>Weight of sample after test</td>");
                                        sbDetails.Append($"<td style='text-align:center;'>{unit}</td>");
                                        sbDetails.Append($"<td style='text-align:center;'>{specified}</td>");
                                        sbDetails.Append($"<td style='text-align:center;'>{valueTest}</td>");
                                        sbDetails.Append("</tr>");
                                    }
                                }
                            }
                        }
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
                        body = objEmail.PrepareMailBodyWith("TestRecord_PengujianGlassTransitionTemperature.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord_PengujianGlassTransitionTemperature -" + SampleID + ".htm");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_PengujianGlassTransitionTemperature -" + SampleID + ".htm"));
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