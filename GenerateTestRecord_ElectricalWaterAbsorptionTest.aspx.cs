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
    public partial class GenerateTestRecord_ElectricalWaterAbsorptionTest : System.Web.UI.Page
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

                        // Define data structures
                        List<string> acTestCores = new List<string>();
                        List<string> acTestLengths = new List<string>();
                        List<string> acTestImmerseTimes = new List<string>();
                        List<string> acTestTemperatures = new List<string>();
                        List<string> acTestVoltages = new List<string>();
                        List<string> acTestTimes = new List<string>();
                        List<string> acTestResults = new List<string>();
                        List<string> acTestSpecified = new List<string>();

                        List<string> dcTestCores = new List<string>();
                        List<string> dcTestLengths = new List<string>();
                        List<string> dcTestImmerseTimes = new List<string>();
                        List<string> dcTestTemperatures = new List<string>();
                        List<string> dcTestVoltages = new List<string>();
                        List<string> dcTestTimes = new List<string>();
                        List<string> dcTestResults = new List<string>();
                        List<string> dcTestSpecified = new List<string>();

                        string acTestDate = "";
                        string dcTestDate = "";

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

                        int InquiryID = 0;

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
                                // AC Test Data
                                if (dr["ReportParamMapID"].ToString() == "14236") { acTestDate = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14237") { acTestCores = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "14238") { acTestLengths = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "14239") { acTestImmerseTimes = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "14240") { acTestTemperatures = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "14241") { acTestVoltages = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "14242") { acTestTimes = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "14243") { acTestResults = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "14244") { acTestSpecified = new List<string>(dr["Result"].ToString().Split(',')); }

                                // DC Test Data
                                if (dr["ReportParamMapID"].ToString() == "14245")
                                {
                                    DateTime tempDate;
                                    if (DateTime.TryParse(dr["Result"].ToString(), out tempDate))
                                        dcTestDate = tempDate.ToString("dd/MM/yyyy");
                                    else
                                        dcTestDate = dr["Result"].ToString(); 
                                }
                                if (dr["ReportParamMapID"].ToString() == "14246") { dcTestCores = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "14247") { dcTestLengths = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "14248") { dcTestImmerseTimes = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "14249") { dcTestTemperatures = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "14250") { dcTestVoltages = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "14251") { dcTestTimes = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "14252") { dcTestResults = new List<string>(dr["Result"].ToString().Split(',')); }
                                if (dr["ReportParamMapID"].ToString() == "14253") { dcTestSpecified = new List<string>(dr["Result"].ToString().Split(',')); }

                                // Header Information
                                if (dr["ReportParamMapID"].ToString() == "14224") { Type = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14225") { Size = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14226") { Voltage = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14227") { Date = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14228") { RoomNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14229") { RoomTemp = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14230") { RefSpec = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14231") { TestMethodNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14232") { JobOrderNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14233") { WorkSheetNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14234") { RoomHumidity = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "14235") { EquipmentNo = dr["Result"].ToString(); }
                            }
                        }
                        // Fix the HTML syntax errors in sbDetails
                        sbHeader.Append(@"<table style='width: 100%; border: 1px solid; margin-top: 10px;' class='tab1'>
                        <tr>
                            <td style='text-align:left;width:20%;'>Type</td>
                            <td style='width:30%;'>" + Type + @"</td>
                            <td style='text-align:left;width:20%;'>Ref Spec.</td>
                            <td style='width:30%;'>" + RefSpec + @"</td>
                        </tr>
                        <tr>
                            <td style='text-align:left;width:20%;'>Size</td>
                            <td>" + Size + @"</td>
                            <td style='text-align:left;width:20%;'>Test Method No.</td>
                            <td>" + TestMethodNo + @"</td>
                        </tr>
                        <tr>
                            <td style='text-align:left;width:20%;'>Voltage</td>
                            <td>" + Voltage + @"</td>
                            <td style='text-align:left;width:20%;'>Job Order No.</td>
                            <td>" + JobOrderNo + @"</td>
                        </tr>
                        <tr>
                            <td style='text-align:left;width:20%;'>Date</td>
                            <td>" + Date + @"</td>
                            <td style='text-align:left;width:20%;'>Work Sheet No.</td>
                            <td>" + WorkSheetNo + @"</td>
                        </tr>
                        <tr>
                            <td style='text-align:left;width:20%;'>Room No.</td>
                            <td>" + RoomNo + @"</td>
                            <td style='text-align:left;width:20%;'>Room Humidity</td>
                            <td>" + RoomHumidity + "%" + @"</td>
                        </tr>
                        <tr>
                            <td style='text-align:left;width:20%;'>Room Temp.</td>
                            <td>" + RoomTemp + "°C" + @"</td>
                            <td style='text-align:left;width:20%;'>Equipment No.</td>
                            <td>" + EquipmentNo + @"</td>
                        </tr>
                    </table>");

                        if (acTestCores.Count > 0)
                        {

                            sbDetails.Append(@"
                                <table class='tab1' style='width:100%;border-collapse:collapse;'>
                                    <thead>
                                    <tr style='background-color:#bebebe;font-weight:bold;'>
                                            <th colspan='9' style='border:1px solid;padding:5px;'>1. Voltage Test (AC) on insulation</th>
                                    </tr>
                                        <tr style='background-color:#bebebe;font-weight:bold;'>
                                            <th rowspan='2' style='border:1px solid;padding:5px;text-align: center;'>Date</th>
                                            <th rowspan='2' style='border:1px solid;padding:5px;text-align: center;'>Core</th>
                                            <th style='border:1px solid;padding:5px;text-align: center;'>Length</th>
                                            <th style='border:1px solid;padding:5px;text-align: center;'>Immerse</th>
                                            <th style='border:1px solid;padding:5px;text-align: center;'>Temp.</th>
                                            <th style='border:1px solid;padding:5px;text-align: center;'>Voltage</th>
                                            <th style='border:1px solid;padding:5px;text-align: center;'>Time</th>
                                            <th rowspan='2' style='border:1px solid;padding:5px;text-align: center;'>Result</th>
                                            <th rowspan='2' style='border:1px solid;padding:5px;text-align: center;'>Specified</th>
                                        </tr>
                                      </tr>
                                        <tr style='background-color:#bebebe;font-weight:bold;'>
                                            <th style='border:1px solid;padding:5px;text-align: center;'>(m)</th>
                                            <th style='border:1px solid;padding:5px;text-align: center;'>(hour)</th>
                                            <th style='border:1px solid;padding:5px;text-align: center;'>(°C)</th>
                                            <th style='border:1px solid;padding:5px;text-align: center;'>(kV)</th>
                                            <th style='border:1px solid;padding:5px;text-align: center;'>(minute)</th>
                                        </tr>
                                    </thead>
                                    <tbody>");

                            int acRowCount = acTestCores.Count;
                            for (int i = 0; i < acRowCount; i++)
                            {
                                sbDetails.Append("<tr>");

                                // Merge Date column
                                if (i == 0)
                                    sbDetails.Append("<td rowspan='" + acRowCount + "' style='border:1px solid;padding:5px;text-align:center;'>" + acTestDate + "</td>");

                                sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + acTestCores[i] + "</td>");
                                sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (i < acTestLengths.Count ? acTestLengths[i] : "") + "</td>");
                                sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (i < acTestImmerseTimes.Count ? acTestImmerseTimes[i] : "") + "</td>");
                                sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (i < acTestTemperatures.Count ? acTestTemperatures[i] : "") + "</td>");
                                sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (i < acTestVoltages.Count ? acTestVoltages[i] : "") + "</td>");
                                sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (i < acTestTimes.Count ? acTestTimes[i] : "") + "</td>");
                                sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (i < acTestResults.Count ? acTestResults[i] : "") + "</td>");

                                // Merge Specified column
                                if (i == 0)
                                    sbDetails.Append("<td rowspan='" + acRowCount + "' style='border:1px solid;padding:5px;text-align:center;'>" + (acTestSpecified.Count > 0 ? acTestSpecified[0] : "") + "</td>");

                                sbDetails.Append("</tr>");
                            }


                            sbDetails.Append(@"</tbody></table>");
                        }

                        // DC Test Results
                        if (dcTestCores.Count > 0)
                        {
                            sbDetails.Append(@"
                                <table class='tab1' style='width:100%;border-collapse:collapse;'>
                                    <thead>
                                    <tr style='background-color:#bebebe;font-weight:bold;'>
                                            <th colspan='9' style='border:1px solid;padding:5px;'>2. Voltage Test (DC) on insulation</th>
                                    </tr>
                                        <tr style='background-color:#bebebe;font-weight:bold;'>
                                            <th rowspan='2' style='border:1px solid;padding:5px;text-align: center;'>Date</th>
                                            <th rowspan='2' style='border:1px solid;padding:5px;text-align: center;'>Core</th>
                                            <th style='border:1px solid;padding:5px;text-align: center;'>Length</th>
                                            <th style='border:1px solid;padding:5px;text-align: center;'>Immerse</th>
                                            <th style='border:1px solid;padding:5px;text-align: center;'>Temp.</th>
                                            <th style='border:1px solid;padding:5px;text-align: center;'>Voltage</th>
                                            <th style='border:1px solid;padding:5px;text-align: center;'>Time</th>
                                            <th rowspan='2' style='border:1px solid;padding:5px;text-align: center;'>Result</th>
                                            <th rowspan='2' style='border:1px solid;padding:5px;text-align: center;'>Specified</th>
                                        </tr>
                                      </tr>
                                        <tr style='background-color:#bebebe;font-weight:bold;'>
                                            <th style='border:1px solid;padding:5px;text-align: center;'>(m)</th>
                                            <th style='border:1px solid;padding:5px;text-align: center;'>(hour)</th>
                                            <th style='border:1px solid;padding:5px;text-align: center;'>(°C)</th>
                                            <th style='border:1px solid;padding:5px;text-align: center;'>(kV)</th>
                                            <th style='border:1px solid;padding:5px;text-align: center;'>(hour)</th>
                                        </tr>
                                    </thead>
                                    <tbody>");

                            int dcRowCount = dcTestCores.Count;
                            for (int i = 0; i < dcRowCount; i++)
                            {
                                sbDetails.Append("<tr>");

                                // Merge Date column
                                if (i == 0)
                                    sbDetails.Append("<td rowspan='" + dcRowCount + "' style='border:1px solid;padding:5px;text-align:center;'>" + dcTestDate + "</td>");

                                sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + dcTestCores[i] + "</td>");
                                sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (i < dcTestLengths.Count ? dcTestLengths[i] : "") + "</td>");
                                sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (i < dcTestImmerseTimes.Count ? dcTestImmerseTimes[i] : "") + "</td>");
                                sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (i < dcTestTemperatures.Count ? dcTestTemperatures[i] : "") + "</td>");
                                sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (i < dcTestVoltages.Count ? dcTestVoltages[i] : "") + "</td>");
                                sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (i < dcTestTimes.Count ? dcTestTimes[i] : "") + "</td>");
                                sbDetails.Append("<td style='border:1px solid;padding:5px;'>" + (i < dcTestResults.Count ? dcTestResults[i] : "") + "</td>");

                                // Merge Specified column
                                if (i == 0)
                                    sbDetails.Append("<td rowspan='" + dcRowCount + "' style='border:1px solid;padding:5px;text-align:center;'>" + (dcTestSpecified.Count > 0 ? dcTestSpecified[0] : "") + "</td>");

                                sbDetails.Append("</tr>");
                            }


                            sbDetails.Append(@"</tbody></table>");
                        }
                 

                        sbFooter.Append(@"<table style='width:100%; font-family:Times New Roman; font-size:11px' class='tab1'>
                        <tr>
                            <td rowspan='2' style='vertical-align: middle;'><b>PT Qualis Indonesia</b></td>
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

                        if (dt6 != null && dt6.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt6.Rows)
                            {
                                sb.Append("<tr>");
                                TestRequestReviewID = Convert.ToInt32(dr["TestRequestReviewID"].ToString());
                                sb.Append("<td style='border: 1px solid #2F4F4F;'>" + Convert.ToDateTime(dr["reviewDate"]).ToString("dd/MM/yyyy") + "</td>");
                                sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["sampleName"].ToString() + "," + dr["sampleQty"].ToString() + "</td>");
                                sb.Append("<td style='border: 1px solid #2F4F4F;'>" + sbCustomer.ToString() + "</td>");
                                sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["equipmentCondition"].ToString() + "</td>");
                                sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["manpower"].ToString() + "</td>");
                                sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["reagent"].ToString() + "</td>");
                                sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["testMethod"].ToString() + "</td>");
                                sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["sampleQtyAccepted"].ToString() + "</td>");
                                sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["samplePhyCondn"].ToString() + "</td>");
                                sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["sampleTemp"].ToString() + "</td>");
                                sb.Append("<td style='border: 1px solid #2F4F4F;'>" + Convert.ToDateTime(dr["deliveryDate"]).ToString("dd/MM/yyyy") + "</td>");
                                sb.Append("<td style='border: 1px solid #2F4F4F;'>" + dr["remarks"].ToString() + "</td>");
                                sb.Append("</tr>");
                            }
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

                        body = objEmail.PrepareMailBodyWith("TestRecord_ElectricalWaterAbsorptionTest.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString(), "{IssueNo}", IssueNo, "{Revision}", Revision, "{Title}", title, "{Details}", sb.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord_ElectricalWaterAbsorptionTest-" + SampleID + ".html");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_ElectricalWaterAbsorptionTest-" + SampleID + ".html"));
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