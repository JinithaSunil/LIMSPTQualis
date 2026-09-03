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
    public partial class GenerateTestReport_Gravimetricwaterabsorption : System.Web.UI.Page
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
                        Dictionary<string, List<string>> Width = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> WidthAvg = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Length = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> LengthAvg = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> TotalSurfaceArea = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> WeightM1 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> WeightM2 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> WeightM3 = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> MgPerCm = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Mean = new Dictionary<string, List<string>>();



                        // Specified values
                        string SpecifiedThickness = "";
                        string SpecifiedWidth = "";
                        string SpecifiedLength = "";
                        string SpecifiedTotalsurfacearea = "";
                        string Specifiedmgpercm2 = "";
                        string SpecifiedMean = "";




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

                        if (dtRecordDetails.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtRecordDetails.Rows)
                            {
                                string reportParamMapID = dr["ReportParamMapID"].ToString();
                                string result = dr["Result"].ToString();

                                // Handle metadata parameters
                                if (dr["ReportParamMapID"].ToString() == "8262") { Type = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8264") { Size = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8266") { Voltage = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8272") { Date = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8269") { RoomNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8271") { RoomTemp = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8263") { RefSpec = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8265") { TestMethodNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8267") { JobOrderNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8268") { WorkSheetNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8270") { RoomHumidity = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8273") { EquipmentNo = dr["Result"].ToString(); }

                                // Handle specification parameters
                                if (dr["ReportParamMapID"].ToString() == "8296") { SpecifiedThickness = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8283") { SpecifiedWidth = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8284") { SpecifiedLength = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8285") { SpecifiedTotalsurfacearea = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8286") { Specifiedmgpercm2 = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8287") { SpecifiedMean = dr["Result"].ToString(); }

                                // Handle Thickness data (8274)
                                if (reportParamMapID == "8274")
                                {
                                    // Parse the format: "Red;1:20,6,67,-,7,7|Red;2:3,3,6,68,-,-"
                                    string[] specimens = result.Split('|');
                                    foreach (var spec in specimens)
                                    {
                                        string[] parts = spec.Split(':');
                                        if (parts.Length == 2)
                                        {
                                            string phaseSpecimen = parts[0]; // e.g., "Red;1"
                                            string[] values = parts[1].Split(',');

                                            // Extract the phase (e.g., "Red")
                                            string phase = phaseSpecimen.Split(';')[0];

                                            // Add the phase to the Thickness dictionary if not already added
                                            if (!Thickness.ContainsKey(phase))
                                                Thickness[phase] = new List<string>();

                                            Thickness[phase].AddRange(values);
                                        }
                                    }
                                }

                                // Handle Width/Diameter data (8275)
                                if (reportParamMapID == "8275")
                                {
                                    // If there is no width data stored, we'll need to create default values
                                    // Based on the samples, width data should be set to "-" for each measurement point
                                    // We'll use the phases from Thickness as a reference
                                    foreach (var phase in Thickness.Keys)
                                    {
                                        if (!Width.ContainsKey(phase))
                                            Width[phase] = new List<string>();

                                        // Add default "-" values for each measurement point
                                        for (int i = 0; i < Thickness[phase].Count; i++)
                                        {
                                            Width[phase].Add("-");
                                        }
                                    }

                                    // If there is actual data, parse it (similar to Thickness)
                                    if (!string.IsNullOrEmpty(result))
                                    {
                                        string[] specimens = result.Split('|');
                                        foreach (var spec in specimens)
                                        {
                                            string[] parts = spec.Split(':');
                                            if (parts.Length == 2)
                                            {
                                                string phaseSpecimen = parts[0];
                                                string[] values = parts[1].Split(',');

                                                string phase = phaseSpecimen.Split(';')[0];

                                                if (!Width.ContainsKey(phase))
                                                    Width[phase] = new List<string>();

                                                // Clear any default values and add the actual ones
                                                Width[phase].Clear();
                                                Width[phase].AddRange(values);
                                            }
                                        }
                                    }
                                }

                                // Handle Length data (8276)
                                if (reportParamMapID == "8276")
                                {
                                    // Similar approach as Width
                                    foreach (var phase in Thickness.Keys)
                                    {
                                        if (!Length.ContainsKey(phase))
                                            Length[phase] = new List<string>();

                                        // Add default "-" values
                                        for (int i = 0; i < Thickness[phase].Count; i++)
                                        {
                                            Length[phase].Add("-");
                                        }
                                    }

                                    // Parse actual data if available
                                    if (!string.IsNullOrEmpty(result))
                                    {
                                        string[] specimens = result.Split('|');
                                        foreach (var spec in specimens)
                                        {
                                            string[] parts = spec.Split(':');
                                            if (parts.Length == 2)
                                            {
                                                string phaseSpecimen = parts[0];
                                                string[] values = parts[1].Split(',');

                                                string phase = phaseSpecimen.Split(';')[0];

                                                if (!Length.ContainsKey(phase))
                                                    Length[phase] = new List<string>();

                                                Length[phase].Clear();
                                                Length[phase].AddRange(values);
                                            }
                                        }
                                    }
                                }

                                // Handle Averages and other measurements

                                // ThicknessAvg (8312)
                                if (reportParamMapID == "8312")
                                {
                                    // Make sure we clear any old data
                                    ThicknessAvg.Clear();

                                    // Parse in format like "12.25|14.25" where each value corresponds to a specimen
                                    string[] averages = result.Split('|');
                                    foreach (var phase in Thickness.Keys)
                                    {
                                        if (!ThicknessAvg.ContainsKey(phase))
                                            ThicknessAvg[phase] = new List<string>();

                                        // Add all average values to the list
                                        foreach (string avg in averages)
                                        {
                                            if (!string.IsNullOrEmpty(avg))
                                                ThicknessAvg[phase].Add(avg);
                                            else
                                                ThicknessAvg[phase].Add("-");
                                        }
                                    }
                                }

                                // Similarly for WidthAvg (8313)
                                if (reportParamMapID == "8313")
                                {
                                    WidthAvg.Clear();
                                    string[] averages = result.Split('|');
                                    foreach (var phase in Thickness.Keys)
                                    {
                                        if (!WidthAvg.ContainsKey(phase))
                                            WidthAvg[phase] = new List<string>();

                                        foreach (string avg in averages)
                                        {
                                            if (!string.IsNullOrEmpty(avg))
                                                WidthAvg[phase].Add(avg);
                                            else
                                                WidthAvg[phase].Add("-");
                                        }
                                    }
                                }

                                // And for LengthAvg (8314)
                                if (reportParamMapID == "8314")
                                {
                                    LengthAvg.Clear();
                                    string[] averages = result.Split('|');
                                    foreach (var phase in Thickness.Keys)
                                    {
                                        if (!LengthAvg.ContainsKey(phase))
                                            LengthAvg[phase] = new List<string>();

                                        foreach (string avg in averages)
                                        {
                                            if (!string.IsNullOrEmpty(avg))
                                                LengthAvg[phase].Add(avg);
                                            else
                                                LengthAvg[phase].Add("-");
                                        }
                                    }
                                }

                                // Total Surface Area (8277)
                                if (reportParamMapID == "8277")
                                {
                                    TotalSurfaceArea.Clear();
                                    string[] values = result.Split('|');
                                    foreach (var phase in Thickness.Keys)
                                    {
                                        if (!TotalSurfaceArea.ContainsKey(phase))
                                            TotalSurfaceArea[phase] = new List<string>();

                                        foreach (string val in values)
                                        {
                                            if (!string.IsNullOrEmpty(val))
                                                TotalSurfaceArea[phase].Add(val);
                                            else
                                                TotalSurfaceArea[phase].Add("-");
                                        }
                                    }
                                }

                                // WeightM1 (8278)
                                if (reportParamMapID == "8278")
                                {
                                    WeightM1.Clear();
                                    string[] values = result.Split('|');
                                    foreach (var phase in Thickness.Keys)
                                    {
                                        if (!WeightM1.ContainsKey(phase))
                                            WeightM1[phase] = new List<string>();

                                        foreach (string val in values)
                                        {
                                            if (!string.IsNullOrEmpty(val))
                                                WeightM1[phase].Add(val);
                                            else
                                                WeightM1[phase].Add("-");
                                        }
                                    }
                                }

                                // WeightM2 (8279)
                                if (reportParamMapID == "8279")
                                {
                                    WeightM2.Clear();
                                    string[] values = result.Split('|');
                                    foreach (var phase in Thickness.Keys)
                                    {
                                        if (!WeightM2.ContainsKey(phase))
                                            WeightM2[phase] = new List<string>();

                                        foreach (string val in values)
                                        {
                                            if (!string.IsNullOrEmpty(val))
                                                WeightM2[phase].Add(val);
                                            else
                                                WeightM2[phase].Add("-");
                                        }
                                    }
                                }

                                // WeightM3 (8280)
                                if (reportParamMapID == "8280")
                                {
                                    WeightM3.Clear();
                                    string[] values = result.Split('|');
                                    foreach (var phase in Thickness.Keys)
                                    {
                                        if (!WeightM3.ContainsKey(phase))
                                            WeightM3[phase] = new List<string>();

                                        foreach (string val in values)
                                        {
                                            if (!string.IsNullOrEmpty(val))
                                                WeightM3[phase].Add(val);
                                            else
                                                WeightM3[phase].Add("-");
                                        }
                                    }
                                }

                                // MgPerCm (8281)
                                if (reportParamMapID == "8281")
                                {
                                    MgPerCm.Clear();
                                    string[] values = result.Split('|');
                                    foreach (var phase in Thickness.Keys)
                                    {
                                        if (!MgPerCm.ContainsKey(phase))
                                            MgPerCm[phase] = new List<string>();

                                        foreach (string val in values)
                                        {
                                            if (!string.IsNullOrEmpty(val))
                                                MgPerCm[phase].Add(val);
                                            else
                                                MgPerCm[phase].Add("-");
                                        }
                                    }
                                }

                                // Mean (8282)
                                if (reportParamMapID == "8282")
                                {
                                    Mean.Clear();
                                    string[] values = result.Split('|');
                                    foreach (var phase in Thickness.Keys)
                                    {
                                        if (!Mean.ContainsKey(phase))
                                            Mean[phase] = new List<string>();

                                        foreach (string val in values)
                                        {
                                            if (!string.IsNullOrEmpty(val))
                                                Mean[phase].Add(val);
                                            else
                                                Mean[phase].Add("-");
                                        }
                                    }
                                }



                                if (dr["ReportParamMapID"].ToString() == "8262") { Type = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8264") { Size = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8266") { Voltage = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8272") { Date = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8269") { RoomNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8271") { RoomTemp = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8263") { RefSpec = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8265") { TestMethodNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8267") { JobOrderNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8268") { WorkSheetNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8270") { RoomHumidity = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8273") { EquipmentNo = dr["Result"].ToString(); }


                                if (dr["ReportParamMapID"].ToString() == "8296") { SpecifiedThickness = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8283") { SpecifiedWidth = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8284") { SpecifiedLength = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8285") { SpecifiedTotalsurfacearea = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8286") { Specifiedmgpercm2 = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "8287") { SpecifiedMean = dr["Result"].ToString(); }

                            }
                            sbHeader.Append(@"<table style='width: 100%; border: 1px solid; margin-top: 10px; ' class='tab1'>
    <tr>
        <td>Type</td>
        <td>" + Type + @"</td>
        <td>Ref Spec.</td>
        <td>" + RefSpec + @"</td>
    </tr>
    <tr>
        <td>Size</td>
        <td>" + Size + @"</td>
        <td>Test Method No.</td>
        <td>" + TestMethodNo + @"</td>
    </tr>
    <tr>
        <td>Voltage</td>
        <td>" + Voltage + @"</td>
        <td>Job Order No.</td>
        <td>" + JobOrderNo + @"</td>
    </tr>
    <tr>
        <td>Date</td>
        <td>" + Date + @"</td>
        <td>Work Sheet No.</td>
        <td>" + WorkSheetNo + @"</td>
    </tr>
    <tr>
        <td>Room No.</td>
        <td>" + RoomNo + @"</td>
        <td>Room Humidity</td>
        <td>" + RoomHumidity + @"%</td>
    </tr>
    <tr>
        <td>Room Temp.</td>
        <td>" + RoomTemp + @"°C</td>
        <td>Equipment No.</td>
        <td>" + EquipmentNo + @"</td>
    </tr>
</table>");

                            // Build the details table
                            sbDetails.Append(@"<table class='tab1' border='1' cellpadding='3' cellspacing='0' style='width:100%; border-collapse: collapse;'>
    <tr>
        <td rowspan='3' style='background-color:#bebebe;font-weight: bold; width: 10%;'>Core Phase</td>
        <td rowspan='3' style='text-align: center; font-weight: bold; background-color: #bebebe; width: 5%;'>Specimen No.</td>
        <td colspan='7' style='background-color:#bebebe;font-weight: bold;'>Thickness</td>
        <td colspan='7' style='background-color:#bebebe;font-weight: bold;'>Width</td>
        <td colspan='7' style='background-color:#bebebe;font-weight: bold;'>Length</td>
        <td rowspan='3' style='background-color:#bebebe;font-weight: bold; width: 8%;'>Total Surface Area<br>cm<sup>2</sup></td>
        <td colspan='3' style='background-color:#bebebe;font-weight: bold;'>Weight</td>
        <td colspan='2' style='background-color:#bebebe;font-weight: bold;'>Variation of Mass</td>
    </tr>
    <tr>
        <td colspan='6' style='background-color:#bebebe;font-weight: bold;'>mm</td>
        <td rowspan='2' style='background-color:#bebebe;font-weight: bold;'>Avg.</td>
        <td colspan='6' style='background-color:#bebebe;font-weight: bold;'>mm</td>
        <td rowspan='2' style='background-color:#bebebe;font-weight: bold;'>Avg.</td>
        <td colspan='6' style='background-color:#bebebe;font-weight: bold;'>mm</td>
        <td rowspan='2' style='background-color:#bebebe;font-weight: bold;'>Avg.</td>
        <td rowspan='2' style='background-color:#bebebe;font-weight: bold;'>M1<br>mg</td>
        <td rowspan='2' style='background-color:#bebebe;font-weight: bold;'>M2<br>mg</td>
        <td rowspan='2' style='background-color:#bebebe;font-weight: bold;'>M3<br>mg</td>
        <td rowspan='2' style='background-color:#bebebe;font-weight: bold;'>mg/cm<sup>2</sup></td>
        <td rowspan='2' style='background-color:#bebebe;font-weight: bold;'>Mean</td>
    </tr>
    <tr>
        <td style='background-color:#bebebe;font-weight: bold;'>1</td>
        <td style='background-color:#bebebe;font-weight: bold;'>2</td>
        <td style='background-color:#bebebe;font-weight: bold;'>3</td>
        <td style='background-color:#bebebe;font-weight: bold;'>4</td>
        <td style='background-color:#bebebe;font-weight: bold;'>5</td>
        <td style='background-color:#bebebe;font-weight: bold;'>6</td>

        <td style='background-color:#bebebe;font-weight: bold;'>1</td>
        <td style='background-color:#bebebe;font-weight: bold;'>2</td>
        <td style='background-color:#bebebe;font-weight: bold;'>3</td>
        <td style='background-color:#bebebe;font-weight: bold;'>4</td>
        <td style='background-color:#bebebe;font-weight: bold;'>5</td>
        <td style='background-color:#bebebe;font-weight: bold;'>6</td>
 
        <td style='background-color:#bebebe;font-weight: bold;'>1</td>
        <td style='background-color:#bebebe;font-weight: bold;'>2</td>
        <td style='background-color:#bebebe;font-weight: bold;'>3</td>
        <td style='background-color:#bebebe;font-weight: bold;'>4</td>
        <td style='background-color:#bebebe;font-weight: bold;'>5</td>
        <td style='background-color:#bebebe;font-weight: bold;'>6</td>
    </tr>");



                            // Get distinct phases
                            var distinctPhases = Thickness.Keys.ToList();

                            // Process each phase
                            foreach (string phase in distinctPhases)
                            {
                                List<string> thicknessValues = Thickness.ContainsKey(phase) ? Thickness[phase] : new List<string>();
                                List<string> widthValues = Width.ContainsKey(phase) ? Width[phase] : new List<string>();
                                List<string> lengthValues = Length.ContainsKey(phase) ? Length[phase] : new List<string>();

                                // Get average values (as lists now)
                                List<string> thicknessAvgs = ThicknessAvg.ContainsKey(phase) ? ThicknessAvg[phase] : new List<string>();
                                List<string> widthAvgs = WidthAvg.ContainsKey(phase) ? WidthAvg[phase] : new List<string>();
                                List<string> lengthAvgs = LengthAvg.ContainsKey(phase) ? LengthAvg[phase] : new List<string>();

                                // Get other measurements
                                List<string> totalAreas = TotalSurfaceArea.ContainsKey(phase) ? TotalSurfaceArea[phase] : new List<string>();
                                List<string> weightM1s = WeightM1.ContainsKey(phase) ? WeightM1[phase] : new List<string>();
                                List<string> weightM2s = WeightM2.ContainsKey(phase) ? WeightM2[phase] : new List<string>();
                                List<string> weightM3s = WeightM3.ContainsKey(phase) ? WeightM3[phase] : new List<string>();
                                List<string> mgPerCms = MgPerCm.ContainsKey(phase) ? MgPerCm[phase] : new List<string>();
                                List<string> means = Mean.ContainsKey(phase) ? Mean[phase] : new List<string>();

                                // Calculate the number of specimen rows needed
                                int measurementsPerSpecimen = 6; // 6 thickness measurements per specimen
                                int specimenCount = (thicknessValues.Count + measurementsPerSpecimen - 1) / measurementsPerSpecimen;
                                specimenCount = Math.Max(1, specimenCount); // Ensure at least 1 specimen

                                for (int specimenIndex = 0; specimenIndex < specimenCount; specimenIndex++)
                                {
                                    sbDetails.Append("<tr>");

                                    // Only show phase name in the first row of each phase
                                    if (specimenIndex == 0)
                                        sbDetails.Append("<td rowspan='" + specimenCount + "' style='text-align:center;'>" + phase + "</td>");

                                    // Specimen number
                                    sbDetails.Append("<td style='text-align:center;'>" + (specimenIndex + 1) + "</td>");

                                    // Thickness values (6 columns)
                                    for (int i = 0; i < 6; i++)
                                    {
                                        int valueIndex = specimenIndex * 6 + i;
                                        string value = valueIndex < thicknessValues.Count ? thicknessValues[valueIndex] : "-";
                                        sbDetails.Append("<td style='text-align:center;'>" + value + "</td>");
                                    }

                                    // Thickness Average - show individual value for each specimen
                                    string thicknessAvg = specimenIndex < thicknessAvgs.Count ? thicknessAvgs[specimenIndex] : "-";
                                    sbDetails.Append("<td style='text-align:center;'>" + thicknessAvg + "</td>");

                                    // Width values (extended to 6 columns)
                                    for (int i = 0; i < 6; i++)
                                    {
                                        int valueIndex = specimenIndex * 3 + (i % 3); // Use modulo to repeat values if needed
                                        string value = "-"; // Default value

                                        // Only assign real values for the first 3 columns (if available)
                                        if (i < 3 && valueIndex < widthValues.Count)
                                        {
                                            value = widthValues[valueIndex];
                                        }

                                        sbDetails.Append("<td style='text-align:center;'>" + value + "</td>");
                                    }

                                    // Width Average - show individual value for each specimen
                                    string widthAvg = specimenIndex < widthAvgs.Count ? widthAvgs[specimenIndex] : "-";
                                    sbDetails.Append("<td style='text-align:center;'>" + widthAvg + "</td>");

                                    // Length values (extended to 6 columns)
                                    for (int i = 0; i < 6; i++)
                                    {
                                        int valueIndex = specimenIndex * 3 + (i % 3); // Use modulo to repeat values if needed
                                        string value = "-"; // Default value

                                        // Only assign real values for the first 3 columns (if available)
                                        if (i < 3 && valueIndex < lengthValues.Count)
                                        {
                                            value = lengthValues[valueIndex];
                                        }

                                        sbDetails.Append("<td style='text-align:center;'>" + value + "</td>");
                                    }

                                    // Length Average - show individual value for each specimen
                                    string lengthAvg = specimenIndex < lengthAvgs.Count ? lengthAvgs[specimenIndex] : "-";
                                    sbDetails.Append("<td style='text-align:center;'>" + lengthAvg + "</td>");

                                    // Total Surface Area - show individual value for each specimen
                                    string totalArea = specimenIndex < totalAreas.Count ? totalAreas[specimenIndex] : "-";
                                    sbDetails.Append("<td style='text-align:center;'>" + totalArea + "</td>");

                                    // Weight values - show individual values for each specimen
                                    string weightM1 = specimenIndex < weightM1s.Count ? weightM1s[specimenIndex] : "-";
                                    string weightM2 = specimenIndex < weightM2s.Count ? weightM2s[specimenIndex] : "-";
                                    string weightM3 = specimenIndex < weightM3s.Count ? weightM3s[specimenIndex] : "-";
                                    sbDetails.Append("<td style='text-align:center;'>" + weightM1 + "</td>");
                                    sbDetails.Append("<td style='text-align:center;'>" + weightM2 + "</td>");
                                    sbDetails.Append("<td style='text-align:center;'>" + weightM3 + "</td>");

                                    // Variation of mass - show individual values for each specimen
                                    string mgPerCm = specimenIndex < mgPerCms.Count ? mgPerCms[specimenIndex] : "-";
                                    string mean = specimenIndex < means.Count ? means[specimenIndex] : "-";
                                    sbDetails.Append("<td style='text-align:center;'>" + mgPerCm + "</td>");
                                    sbDetails.Append("<td style='text-align:center;'>" + mean + "</td>");

                                    sbDetails.Append("</tr>");
                                }
                            }

                            // Add the "Specified" row
                            sbDetails.Append("<tr>");
                            sbDetails.Append("<td colspan='2' style='text-align:center;font-weight: bold;'>Specified</td>");

                   
                            sbDetails.Append("<td colspan='7' style='text-align:center;font-weight: bold;'>" + SpecifiedThickness + "</td>");
                        
                            sbDetails.Append("<td colspan='7' style='text-align:center;font-weight: bold;'>" + SpecifiedWidth + "</td>");
       
                            sbDetails.Append("<td colspan='7' style='text-align:center;font-weight: bold;'>" + SpecifiedLength + "</td>");

                            sbDetails.Append("<td style='text-align:center;font-weight: bold;'>" + SpecifiedTotalsurfacearea + "</td>");

                            // Weight columns
                            sbDetails.Append("<td colspan='3' style='text-align:center;font-weight: bold;'>-</td>");

                            // Variation of mass
                            sbDetails.Append("<td style='text-align:center;font-weight: bold;'>" + Specifiedmgpercm2 + "</td>");
                            sbDetails.Append("<td style='text-align:center;font-weight: bold;'>" + SpecifiedMean + "</td>");

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

                            body = objEmail.PrepareMailBodyWith("TestRecord_Gravimetricwaterabsorption.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString(), "{IssueNo}", IssueNo, "{Revision}", Revision, "{Title}", title, "{Details}", sb.ToString());
                            str = body.ToString();
                            filename = Server.MapPath("//TestRecord/TestRecord_Gravimetricwaterabsorption-" + SampleID + ".htm");
                            System.IO.File.WriteAllText(@filename, str);
                            html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_Gravimetricwaterabsorption-" + SampleID + ".htm"));
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
        }
      

    }
}