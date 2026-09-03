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
	public partial class GenerateTestRecord_DimensionAndConstruction : System.Web.UI.Page
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
                        string Size = "";
                        string Voltage = "";
                        string Date = "";
                        string RoomNo = "";
                        string RoomTemp = "";
                        string RefSpec = "";
                        string TestMethodNo = "";
                        string JobOrderNo = "";
                        string WorkSheetNo = "";
                        string RoomHumidity = "";
                        string EquipmentNo = "";

                        // Insulation specific data
                        List<string> InsulationCore = new List<string>();

                        Dictionary<string, List<string>> Conductorcondition = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> InsulationCondition = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Insulationcolorscheme = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Insulationcolorcombination = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Fillercondition = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> Innersheathcondition = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> armouringcondition = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> sheathcondition = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> markingcondition = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> markingidentification = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> markingdistance = new Dictionary<string, List<string>>();


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
                                string[] values = dr["Result"].ToString().Split(',').Select(v => v.Trim()).ToArray();
                                string key = ""; // No core/specimen — using a fixed key

                                if (dr["ReportParamMapID"].ToString() == "10221") // Conductor condition
                                {
                                    if (!Conductorcondition.ContainsKey(key))
                                        Conductorcondition[key] = new List<string>();
                                    Conductorcondition[key].AddRange(values);
                                }
                                else if (dr["ReportParamMapID"].ToString() == "10222") // Insulation condition
                                {
                                    if (!InsulationCondition.ContainsKey(key))
                                        InsulationCondition[key] = new List<string>();
                                    InsulationCondition[key].AddRange(values);
                                }
                                else if (dr["ReportParamMapID"].ToString() == "10223") // Insulation color scheme
                                {
                                    if (!Insulationcolorscheme.ContainsKey(key))
                                        Insulationcolorscheme[key] = new List<string>();
                                    Insulationcolorscheme[key].AddRange(values);
                                }
                                else if (dr["ReportParamMapID"].ToString() == "10224") // Insulation color combination
                                {
                                    if (!Insulationcolorcombination.ContainsKey(key))
                                        Insulationcolorcombination[key] = new List<string>();
                                    Insulationcolorcombination[key].AddRange(values);
                                }
                                else if (dr["ReportParamMapID"].ToString() == "10225") // Filler condition
                                {
                                    if (!Fillercondition.ContainsKey(key))
                                        Fillercondition[key] = new List<string>();
                                    Fillercondition[key].AddRange(values);
                                }
                                else if (dr["ReportParamMapID"].ToString() == "10226") // Inner sheath condition
                                {
                                    if (!Innersheathcondition.ContainsKey(key))
                                        Innersheathcondition[key] = new List<string>();
                                    Innersheathcondition[key].AddRange(values);
                                }
                                else if (dr["ReportParamMapID"].ToString() == "10227") // Armouring condition
                                {
                                    if (!armouringcondition.ContainsKey(key))
                                        armouringcondition[key] = new List<string>();
                                    armouringcondition[key].AddRange(values);
                                }
                                else if (dr["ReportParamMapID"].ToString() == "10228") // Sheath condition
                                {
                                    if (!sheathcondition.ContainsKey(key))
                                        sheathcondition[key] = new List<string>();
                                    sheathcondition[key].AddRange(values);
                                }
                                else if (dr["ReportParamMapID"].ToString() == "10229") // Marking condition
                                {
                                    if (!markingcondition.ContainsKey(key))
                                        markingcondition[key] = new List<string>();
                                    markingcondition[key].AddRange(values);
                                }
                                else if (dr["ReportParamMapID"].ToString() == "10230") // Marking identification
                                {
                                    if (!markingidentification.ContainsKey(key))
                                        markingidentification[key] = new List<string>();
                                    markingidentification[key].AddRange(values);
                                }
                                else if (dr["ReportParamMapID"].ToString() == "10231") // Marking distance
                                {
                                    if (!markingdistance.ContainsKey(key))
                                        markingdistance[key] = new List<string>();
                                    markingdistance[key].AddRange(values);
                                }



                                // Process specified values

                                // Process other general parameters
                                if (dr["ReportParamMapID"].ToString() == "10232") { Type = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "10233") { Size = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "10234") { Voltage = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "10235") { Date = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "10236") { RoomNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "10237") { RoomTemp = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "10238") { RefSpec = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "10239") { TestMethodNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "10240") { JobOrderNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "10241") { WorkSheetNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "10240") { RoomHumidity = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "10243") { EquipmentNo = dr["Result"].ToString(); }

                            }
                            // Build header table
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
                        }

                        // Build the Insulation test details table
                        sbDetails.Append(@"
<table class='tab1' border='1' style='border-collapse: collapse; width: 100%; font-family: Arial, sans-serif; font-size: 14px;'>
<tr>
    <td style='width:25px;'>1</td>
    <td  style='text-align : left'><b>Conductor Condition</b></td>
    <td style='width:10px;'>:</td>
    <td>" + string.Join(", ", GetValueOrDefault(Conductorcondition, "")) + @"</td>
</tr>

<tr>
    <td rowspan='4'>2</td>
    <td style='text-align : left' colspan='3'><b>Insulation</b></td>
</tr>
<tr>
    <td style='text-align : left'>- Condition</td>
    <td>:</td>
    <td>" + string.Join(", ", GetValueOrDefault(InsulationCondition, "")) + @"</td>
</tr>
<tr>
    <td style='text-align : left'>- Color Scheme</td>
    <td>:</td>
    <td>" + string.Join(", ", GetValueOrDefault(Insulationcolorscheme, "")) + @"</td>
</tr>
<tr>
    <td style='text-align : left'>- Color Combination</td>
    <td>:</td>
    <td>" + string.Join(", ", GetValueOrDefault(Insulationcolorcombination, "")) + @"</td>
</tr>

<tr>
    <td>3</td>
    <td style='text-align : left'><b>Filler Condition</b></td>
    <td>:</td>
    <td>" + string.Join(", ", GetValueOrDefault(Fillercondition, "")) + @"</td>
</tr>

<tr>
    <td>4</td>
    <td style='text-align : left'><b>Inner Sheath Condition</b></td>
    <td>:</td>
    <td>" + string.Join(", ", GetValueOrDefault(Innersheathcondition, "")) + @"</td>
</tr>

<tr>
    <td>5</td>
    <td style='text-align : left'><b>Armouring Condition</b></td>
    <td>:</td>
    <td>" + string.Join(", ", GetValueOrDefault(armouringcondition, "")) + @"</td>
</tr>

<tr>
    <td>6</td>
    <td style='text-align : left'><b>Sheath Condition</b></td>
    <td>:</td>
    <td>" + string.Join(", ", GetValueOrDefault(sheathcondition, "")) + @"</td>
</tr>

<tr>
    <td rowspan='4'>7</td>
    <td style='text-align : left' colspan='3'><b>Marking</b></td>
</tr>
<tr>
    <td style='text-align : left'>- Condition</td>
    <td>:</td>
    <td>" + string.Join(", ", GetValueOrDefault(markingcondition, "")) + @"</td>
</tr>
<tr>
    <td style='text-align : left'>- Identification</td>
    <td>:</td>
    <td>" + string.Join(", ", GetValueOrDefault(markingidentification, "")) + @"</td>
</tr>
<tr>
    <td style='text-align : left'>- Distance</td>
    <td>:</td>
    <td>" + string.Join(", ", GetValueOrDefault(markingdistance, "")) + @"</td>
</tr>
</table>
");



                        sbDetails.Append("</table>");

                        // Add signature section
                        sbDetails.Append(@"<table style='width:100%; font-family:Times New Roman; font-size:11px' class='tab1'>
                        <tr>
                            <td style='font-weight: bold;vertical-align: middle;' rowspan='2'>PT Qualis Indonesia</td>
                            <td colspan='3' style='text-align:center'><b>Witnessed By</b></td>
                        </tr>
                        <tr>
                            <td>Name</td><td>Company</td><td>Signature</td>
                        </tr>
                        <tr>
                            <td style='text-align:left'>Tested By:<br /><br /><br />Checked By:<br /><br /><br /></td>
                            <td></td><td></td><td></td>
                        </tr>
                    </table>");

                        // Generate the HTML file
                        body = objEmail.PrepareMailBodyWith("TestRecord_DimensionAndConstruction.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord_DimensionAndConstruction-" + SampleID + ".htm");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_DimensionAndConstruction-" + SampleID + ".htm"));
                        divCRFReview.InnerHtml = html;
                        GC.Collect();
                    }
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }

        // Helper method to get values safely
        // Helper method (place this in your class)
        private List<string> GetValueOrDefault(Dictionary<string, List<string>> dict, string key)
        {
            return dict.TryGetValue(key, out var value) ? value : new List<string>();
        }

        // Helper method to count occurrences of a value in a list up to a given index
        int GetOccurrenceIndex(List<string> list, int currentIndex)
        {
            string value = list[currentIndex];
            int count = 0;
            for (int i = 0; i < currentIndex; i++)
            {
                if (list[i].Trim() == value.Trim()) count++;
            }
            return count;
        }
    }
}
