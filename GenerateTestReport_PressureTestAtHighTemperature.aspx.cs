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
    public partial class GenerateTestReport_PressureTestAtHighTemperature : System.Web.UI.Page
    {

        DAL.CM _cm = new DAL.CM(); int InquiryID = 0; int CloneCRFSampleID = 0; int SampleID = 0; int ReportId = 0;
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
                    SampleID = Convert.ToInt32(Session["SampleID"].ToString());
                    ReportId = Convert.ToInt32(Session["ReportId"].ToString());

                    if (Session["Usertype"] != null)
                    {
                        string Type = "";
                        //List<string> Thickness = new List<string>();
                        //List<string> ThicknessAvg = new List<string>();

                        Dictionary<string, List<string>> Thickness = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> ThicknessAvg = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Diameter = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> DiameterAvg = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> DiameterWeightload = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> After6HoursunderloadattemperatureCThickness = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> After6HoursunderloadattemperatureCDeformation = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> After6HoursunderloadattemperatureCMedian = new Dictionary<string, List<string>>();

                        List<string> ExtractSpecimenValues(string resultString)
                        {
                            return resultString
                                .Split('|')
                                .Select(entry =>
                                {
                                    var parts = entry.Split(':');
                                    return parts.Length == 2 ? parts[1] : null;
                                })
                                .Where(val => val != null)
                                .ToList();
                        }

                        // Specified values
                        string SpecifiedThickness = "";
                        string SpecifiedDiameter = "";
                        string SpecifiedWeightload = "";
                        string Specified6hoursthickness = "";
                        string Specified6hoursDeformation = "";
                        string Specified6hoursMedian = "";
                        string AfterTemp = "";
                        string AfterTempLoad = "";



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


                        DataTable dt6 = new DataTable();
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

                        // Sample DataRow (replace this with actual data from dtRecordDetails)
                        if (dtRecordDetails.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtRecordDetails.Rows)
                            {
                                string reportParamMapID = dr["ReportParamMapID"].ToString();
                                string result = dr["Result"].ToString();
                                string[] specifications = result.Split('|');  // Split by "|" to separate specimens

                                if (reportParamMapID == "6247")
                                {
                                    foreach (var spec in specifications)
                                    {
                                        string[] parts = spec.Split(':');
                                        if (parts.Length == 2)
                                        {
                                            string phaseSpecimen = parts[0];
                                            string[] values = parts[1].Split(',');

                                            // Extract the phase part (before the semicolon)
                                            string phase = phaseSpecimen.Split(';')[0];

                                            if (!Thickness.ContainsKey(phase))
                                                Thickness[phase] = new List<string>();

                                            Thickness[phase].AddRange(values);
                                        }
                                    }
                                }
                                if (reportParamMapID == "6249")
                                {
                                    foreach (var spec in specifications)
                                    {
                                        string[] parts = spec.Split(':');
                                        if (parts.Length == 2)
                                        {
                                            string phaseSpecimen = parts[0];
                                            string[] values = parts[1].Split(',');
                                            string phase = phaseSpecimen.Split(';')[0];

                                            if (!Diameter.ContainsKey(phase))
                                                Diameter[phase] = new List<string>();

                                            Diameter[phase].AddRange(values);
                                        }
                                    }
                                }


                                // 
                                if (reportParamMapID == "6248")
                                {
                                    int specIndex = 0;

                                    // Get all phases dynamically from thickness data
                                    foreach (string phase in Thickness.Keys)
                                    {
                                        if (!ThicknessAvg.ContainsKey(phase))
                                            ThicknessAvg[phase] = new List<string>();

                                        // Calculate specimens for this phase
                                        int specimenCount = Thickness[phase].Count / 6;

                                        // Add avg values for each specimen in this phase
                                        for (int i = 0; i < specimenCount && specIndex < specifications.Length; i++)
                                        {
                                            ThicknessAvg[phase].Add(specifications[specIndex]);
                                            specIndex++;
                                        }
                                    }
                                }

                                // 
                                if (reportParamMapID == "6250")
                                {
                                    int specIndex = 0;

                                    // Get all phases dynamically from diameter data
                                    foreach (string phase in Diameter.Keys)
                                    {
                                        if (!DiameterAvg.ContainsKey(phase))
                                            DiameterAvg[phase] = new List<string>();

                                        // Calculate specimens for this phase
                                        int specimenCount = Diameter[phase].Count / 6;

                                        // Add avg values for each specimen in this phase
                                        for (int i = 0; i < specimenCount && specIndex < specifications.Length; i++)
                                        {
                                            DiameterAvg[phase].Add(specifications[specIndex]);
                                            specIndex++;
                                        }
                                    }
                                }
                                // 
                                if (reportParamMapID == "6251")
                                {
                                    foreach (var spec in specifications)
                                    {
                                        string[] parts = spec.Split(':');
                                        if (parts.Length == 2)
                                        {
                                            string phaseSpecimen = parts[0];
                                            string value = parts[1];
                                            string phase = phaseSpecimen.Split(';')[0];

                                            if (!DiameterWeightload.ContainsKey(phase))
                                                DiameterWeightload[phase] = new List<string>();

                                            DiameterWeightload[phase].Add(value);
                                        }
                                    }
                                }
                                else if (reportParamMapID == "6252")
                                {
                                    foreach (var spec in specifications)
                                    {
                                        string[] parts = spec.Split(':');
                                        if (parts.Length == 2)
                                        {
                                            string phaseSpecimen = parts[0];
                                            string value = parts[1];
                                            string phase = phaseSpecimen.Split(';')[0];

                                            if (!After6HoursunderloadattemperatureCThickness.ContainsKey(phase))
                                                After6HoursunderloadattemperatureCThickness[phase] = new List<string>();

                                            After6HoursunderloadattemperatureCThickness[phase].Add(value);
                                        }
                                    }
                                }
                                else if (reportParamMapID == "6253")
                                {
                                    foreach (var spec in specifications)
                                    {
                                        string[] parts = spec.Split(':');
                                        if (parts.Length == 2)
                                        {
                                            string phaseSpecimen = parts[0];
                                            string value = parts[1];
                                            string phase = phaseSpecimen.Split(';')[0];

                                            if (!After6HoursunderloadattemperatureCDeformation.ContainsKey(phase))
                                                After6HoursunderloadattemperatureCDeformation[phase] = new List<string>();

                                            After6HoursunderloadattemperatureCDeformation[phase].Add(value);
                                        }
                                    }
                                }
                                else if (reportParamMapID == "6254")
                                {
                                    foreach (var spec in specifications)
                                    {
                                        string[] parts = spec.Split(':');
                                        if (parts.Length == 2)
                                        {
                                            string phaseSpecimen = parts[0];
                                            string value = parts[1];
                                            string phase = phaseSpecimen.Split(';')[0];

                                            if (!After6HoursunderloadattemperatureCMedian.ContainsKey(phase))
                                                After6HoursunderloadattemperatureCMedian[phase] = new List<string>();

                                            After6HoursunderloadattemperatureCMedian[phase].Add(value);
                                        }
                                    }
                                }


                                if (dr["ReportParamMapID"].ToString() == "8231") { Type = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8220") { Size = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8221") { Voltage = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8222") { Date = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8234") { RoomNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8224") { RoomTemp = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8225") { RefSpec = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8226") { TestMethodNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8227") { JobOrderNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8228") { WorkSheetNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8229") { RoomHumidity = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8230") { EquipmentNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8214") { SpecifiedThickness = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8215") { SpecifiedDiameter = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8216") { SpecifiedWeightload = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8217") { Specified6hoursthickness = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8218") { Specified6hoursDeformation = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8219") { Specified6hoursMedian = dr["Result"].ToString(); }

                                if (dr["ReportParamMapID"].ToString() == "15031") { AfterTemp = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "15032") { AfterTempLoad = dr["Result"].ToString(); }
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
                         </table>");
                        sbDetails.Append(@"<table class='tab1' border='1' cellpadding='3' cellspacing='0' style='width:100%; border-collapse: collapse;'>
<tr>
    <td rowspan='3' style='background-color:#bebebe;'>Core Phase</td>
    <td rowspan='3' style='text-align: center; font-weight: bold; background-color: #bebebe;'>Specimen No.</td>
    <td colspan='7' style='background-color:#bebebe;'>Thickness</td>
    <td colspan='7' style='background-color:#bebebe;'>Diameter</td>
    <td rowspan='3' style='background-color:#bebebe;'>Weight Load<br>gf</td>");

                        sbDetails.Append("<td colspan='2' style='background-color:#bebebe;'>After " + AfterTemp + " Hours under load<br>at temperature " + AfterTempLoad + "°C</td>");
                        sbDetails.Append("<td rowspan='3' style='background-color:#bebebe;'>Median<br>%</td></tr>");

                        sbDetails.Append(@"<tr>
    <td colspan='6' style='background-color:#bebebe;'>mm</td>
    <td rowspan='2' style='background-color:#bebebe;'>Avg.</td>
    <td colspan='6' style='background-color:#bebebe;'>mm</td>
    <td rowspan='2' style='background-color:#bebebe;'>Avg.</td>
    <td rowspan='2' style='background-color:#bebebe;'>Thickness<br>mm</td>
    <td rowspan='2' style='background-color:#bebebe;'>Deformation<br>%</td>
</tr>
<tr>
    <td style='background-color:#bebebe;'>1</td>
    <td style='background-color:#bebebe;'>2</td>
    <td style='background-color:#bebebe;'>3</td>
    <td style='background-color:#bebebe;'>4</td>
    <td style='background-color:#bebebe;'>5</td>
    <td style='background-color:#bebebe;'>6</td>

    <td style='background-color:#bebebe;'>1</td>
    <td style='background-color:#bebebe;'>2</td>
    <td style='background-color:#bebebe;'>3</td>
    <td style='background-color:#bebebe;'>4</td>
    <td style='background-color:#bebebe;'>5</td>
    <td style='background-color:#bebebe;'>6</td>
</tr>");
                        // Get distinct phases
                        var distinctPhases = Thickness.Keys.ToList();

                        int maxMeasurementsPerRow = 6; // Maximum number of measurements to display per row

                        // Process each phase
                        foreach (string phase in distinctPhases)
                        {
                            List<string> allThicknessValues = Thickness.ContainsKey(phase) ? Thickness[phase] : new List<string>();
                            List<string> allDiameterValues = Diameter.ContainsKey(phase) ? Diameter[phase] : new List<string>();
                            List<string> allWeightLoadValues = DiameterWeightload.ContainsKey(phase) ? DiameterWeightload[phase] : new List<string>();
                            List<string> allThicknessAtTempValues = After6HoursunderloadattemperatureCThickness.ContainsKey(phase) ? After6HoursunderloadattemperatureCThickness[phase] : new List<string>();
                            List<string> allDeformationValues = After6HoursunderloadattemperatureCDeformation.ContainsKey(phase) ? After6HoursunderloadattemperatureCDeformation[phase] : new List<string>();
                            List<string> allMedianValues = After6HoursunderloadattemperatureCMedian.ContainsKey(phase) ? After6HoursunderloadattemperatureCMedian[phase] : new List<string>();
                            List<string> allDiameterAvgValues = DiameterAvg.ContainsKey(phase) ? DiameterAvg[phase] : new List<string>();
                            List<string> allThicknessAvgValues = ThicknessAvg.ContainsKey(phase) ? ThicknessAvg[phase] : new List<string>();

                            int specimenCount = Math.Max(allThicknessValues.Count, allDiameterValues.Count) / 6;

                            // Prepare a single median value to display (example: max median value for the phase)
                            string mergedMedianValue = "-";
                            if (allMedianValues.Count > 0)
                            {
                                mergedMedianValue = allMedianValues.Max();
                             
                            }

                            for (int i = 0; i < specimenCount; i++)
                            {
                                sbDetails.Append("<tr>");
                                if (i == 0)
                                    sbDetails.Append("<td rowspan='" + specimenCount + "' style='text-align:center;'>" + phase + "</td>");

                                sbDetails.Append("<td style='text-align:center;'>" + (i + 1) + "</td>");

                                // Thickness 1-6
                                for (int j = 0; j < 6; j++)
                                {
                                    int index = i * 6 + j;
                                    string value = index < allThicknessValues.Count ? allThicknessValues[index] : "-";
                                    sbDetails.Append("<td style='text-align:center;'>" + value + "</td>");
                                }

                                // : Thickness Avg - use correct index calculation
                                string thicknessAvg = "-";
                                int avgIndex = i; // Each specimen should have one avg value
                                if (avgIndex < allThicknessAvgValues.Count)
                                {
                                    thicknessAvg = allThicknessAvgValues[avgIndex];
                                }
                                sbDetails.Append("<td style='text-align:center;'>" + thicknessAvg + "</td>");

                                // Diameter 1-6
                                for (int j = 0; j < 6; j++)
                                {
                                    int index = i * 6 + j;
                                    string value = index < allDiameterValues.Count ? allDiameterValues[index] : "-";
                                    sbDetails.Append("<td style='text-align:center;'>" + value + "</td>");
                                }

                                // : Diameter Avg - use correct index calculation
                                string diameterAvg = "-";
                                if (i < allDiameterAvgValues.Count)
                                {
                                    diameterAvg = allDiameterAvgValues[i];
                                }
                                sbDetails.Append("<td style='text-align:center;'>" + diameterAvg + "</td>");

                                // : Weight Load - use correct index calculation
                                string weightLoad = "-";
                                if (i < allWeightLoadValues.Count)
                                {
                                    weightLoad = allWeightLoadValues[i];
                                }
                                sbDetails.Append("<td style='text-align:center;'>" + weightLoad + "</td>");


                                string thicknessAtTemp = "-";
                                if (i < allThicknessAtTempValues.Count)
                                {
                                    thicknessAtTemp = allThicknessAtTempValues[i];
                                }
                                sbDetails.Append("<td style='text-align:center;'>" + thicknessAtTemp + "</td>");

                                // : Deformation - use correct index calculation
                                string deformation = "-";
                                if (i < allDeformationValues.Count)
                                {
                                    deformation = allDeformationValues[i];
                                }
                                sbDetails.Append("<td style='text-align:center;'>" + deformation + "</td>");

                                // Median - merge vertically on first specimen only per phase
                                if (i == 0)
                                {
                                    sbDetails.Append("<td rowspan='" + specimenCount + "' style='text-align:center; vertical-align:middle;'>" + mergedMedianValue + "</td>");
                                }

                                sbDetails.Append("</tr>");
                            }
                        }

                        sbDetails.Append("<tr>");
                        sbDetails.Append("<td colspan='2' style='text-align:center;'>Specified</td>");

                        // Thickness - Use colspan to merge the 6 cells
                        sbDetails.Append("<td colspan='7' style='text-align:center;'>" + SpecifiedThickness + "</td>");
                     

                        // Diameter - Use colspan to merge the 6 cells
                        sbDetails.Append("<td colspan='7' style='text-align:center;'>" + SpecifiedDiameter + "</td>");
                        
                        // Weight Load
                        sbDetails.Append("<td style='text-align:center;'>" + SpecifiedWeightload + "</td>");

                        // Thickness at temperature
                        sbDetails.Append("<td style='text-align:center;'>Temperature at :" + Specified6hoursthickness + "</td>");

                        // Deformation
                        sbDetails.Append("<td style='text-align:center;'>Temperature at :" + Specified6hoursDeformation + "</td>");

                        // Median
                        sbDetails.Append("<td style='text-align:center;'>Max:" + Specified6hoursMedian + "</td>");

                        sbDetails.Append("</tr>");
                        // Close the table
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
                        dt6 = _cm.getTestRequestReview(InquiryID);
                        //Table start.
                        sb.Append("<table border='1' style='border-spacing:0px; width:100%'>");

                        //Adding HeaderRow.
                        sb.Append("<tr>");
                        sb.Append("<td rowspan='2' style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:10%'><b>Date of Review</b></td>");
                        sb.Append("<td colspan='2' style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:20%'><b>Review against</b></td>");
                        sb.Append("<td colspan='9' style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:70%'><b>Contract /Test Request Review Checking Points</b></td>");
                        sb.Append("</tr>");
                        sb.Append("<tr>");
                        sb.Append("<td style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:10%'><b>Sample <br/>Commodity<br/> & No. of<br/> Sample</b></td>");
                        sb.Append("<td style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:10%'><b>Customer<br/> Details</b></td>");
                        sb.Append("<td style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:10%'><b>Equipment<br/> Condition<br/> (OK or not)</b></td>");
                        sb.Append("<td style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:10%'><b>Manpower<br/> Availability<br/>(OK or not)</b></td>");
                        sb.Append("<td style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:10%'><b>Reagent/Std Availability(OK or not)</b></td>");
                        sb.Append("<td style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:10%'><b>Test<br/> Method<br/>/Specification <br/>(OK or not)</b></td>");
                        sb.Append("<td style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:5%'><b>*Sample Quantity <br/>Accepted<br/> (Minimum <br/>Quantity)</b></td>");
                        sb.Append("<td style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:10%'><b>*Physical<br/> Condition of <br/>Sample to be<br/> accepted</b></td>");
                        sb.Append("<td style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:5%'><b>*Temp <br/>Checking of<br/> sample (if<br/> Required)</b></td>");
                        sb.Append("<td style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:10%'><b>Delivery of<br/> Test report <br/>(approx.)</b></td>");
                        sb.Append("<td style='text-align:center;background-color:#bebebe; border: 1px solid #2F4F4F;width:10%'><b>Remarks /<br/> Signature</b></td>");
                        sb.Append("</tr>");

                        for (int freqCount = 1; freqCount < dt6.Rows.Count + 1; freqCount++)
                        {
                            sb.Append("<tr>");
                            {
                                foreach (DataRow dr in dt6.Rows)
                                {
                                    TestRequestReviewID = Convert.ToInt32(dr["TestRequestReviewID"].ToString());
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + Convert.ToDateTime(dr["reviewDate"]).ToString("dd/MM/yyyy") + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["sampleName"].ToString() + "," + dr["sampleQty"].ToString() + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + sbCustomer.ToString() + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["equipmentCondition"].ToString() + "</ td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["manpower"].ToString() + "</ td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["reagent"].ToString() + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["testMethod"].ToString() + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["sampleQtyAccepted"].ToString() + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["samplePhyCondn"].ToString() + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["sampleTemp"].ToString() + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + Convert.ToDateTime(dr["deliveryDate"]).ToString("dd/MM/yyyy") + "</td>");
                                    sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["remarks"].ToString() + "</td>");
                                }

                            }
                            sb.Append("</tr>");
                        }
                        //Table end.
                        sb.Append("</table>");
                        string DocumentNo = ""; string IssueNo = ""; string Revision = ""; string title = "";
                        DataTable dtDocument = _cm.getDocumentDetails("Test Request Review Checklist");
                        if (dtDocument.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtDocument.Rows)
                            {
                                DocumentNo = dr["DocumentNo"].ToString();
                                IssueNo = dr["IssueNo"].ToString();
                                Revision = dr["Revision"].ToString();
                                title = dr["Title"].ToString();
                            }
                        }

                        body = objEmail.PrepareMailBodyWith("TestRecord_PressureTestAtHighTemperature.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString(), "{IssueNo}", IssueNo, "{Revision}", Revision, "{Title}", title, "{Details}", sb.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord_PressureTestAtHighTemperature-" + SampleID + ".htm");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_PressureTestAtHighTemperature-" + SampleID + ".htm"));
                        divCRFReview.InnerHtml = html;
                        GC.Collect();
                        //Response.Redirect("~/SampleList.aspx");

                    }
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
        // Add this helper method to get values safely
        private string GetValueSafely(Dictionary<string, List<string>> data, string key, int index)
        {
            if (data.ContainsKey(key) && index < data[key].Count)
                return data[key][index];
            return "";
        }

    }
}