using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using LIMS.DAL;

namespace LIMS
{
	public partial class GenerateTestRecord_ConductorResistance : System.Web.UI.Page
	{

        DAL.CM _cm = new DAL.CM(); int SampleID = 0; int ReportId = 0;
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
                        List<string> Core = new List<string>();
                        List<string> length = new List<string>();
                        List<string> tempFactor = new List<string>();
                        List<string> rt = new List<string>();
                        List<string> rt20c = new List<string>();

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


                        dtRecordDetails = _cm.getTestRecord(ReportId, SampleID);
                        if (dtRecordDetails.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtRecordDetails.Rows)
                            {
                                if (dr["ReportParamMapID"].ToString() == "2138")
                                {
                                    Core = new List<string>(dr["Result"].ToString().Split(','));
                                }

                                if (dr["ReportParamMapID"].ToString() == "2139")
                                {
                                    length = new List<string>(dr["Result"].ToString().Split(','));
                                }
                                if (dr["ReportParamMapID"].ToString() == "2140")
                                {
                                    tempFactor = new List<string>(dr["Result"].ToString().Split(','));
                                }
                                if (dr["ReportParamMapID"].ToString() == "2141")
                                {
                                    rt = new List<string>(dr["Result"].ToString().Split(','));
                                }
                                if (dr["ReportParamMapID"].ToString() == "2142")
                                {
                                    rt20c = new List<string>(dr["Result"].ToString().Split(','));
                                }

                                if (dr["ReportParamMapID"].ToString() == "4177") { Type = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4178") { Size = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4179") { Voltage = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4180") { Date = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4181") { RoomNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4182") { RoomTemp = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4183") { RefSpec = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4184") { TestMethodNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4185") { JobOrderNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4186") { WorkSheetNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4187") { RoomHumidity = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4188") { EquipmentNo = dr["Result"].ToString(); }

                            }
                            sbHeader.Append(@"<table style='width: 100%; border: 1px solid black; border-collapse: collapse;'>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px;'>Type</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + Type + @"</td>
                                <td style='border: 1px solid black; padding: 5px;'>Ref Spec.</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + RefSpec + @"</td>
                            </tr>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px;'>Size</td>
                                <td style='border: 1px solid black; padding: 5px;width:41%'>" + Size + @"</td>
                                <td style='border: 1px solid black; padding: 5px;'>Test Method No.</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + TestMethodNo + @"</td>
                            </tr>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px;'>Voltage</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + Voltage + @"</td>
                                <td style='border: 1px solid black; padding: 5px;'>Job Order No.</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + JobOrderNo + @"</td>
                            </tr>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px;'>Date</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + Date + @"</td>
                                <td style='border: 1px solid black; padding: 5px;'>Work Sheet No.</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + WorkSheetNo + @"</td>
                            </tr>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px;'>Room No.</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + RoomNo + @"</td>
                                <td style='border: 1px solid black; padding: 5px;'>Room Humidity</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + RoomHumidity + @" %</td>
                            </tr>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px;'>Room Temp.</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + RoomTemp + @" °C</td>
                                <td style='border: 1px solid black; padding: 5px;'>Equipment No.</td>
                                <td style='border: 1px solid black; padding: 5px;'>" + EquipmentNo + @"</td>
                            </tr>
                        </table>");
                            // Second table - Core measurements table
                            sbDetails.Clear();
                            sbDetails.Append(@"<table style='width: 100%; border: 1px solid black; border-collapse: collapse; margin-top: 10px;'>
                            <tr style='background-color: #bebebe;'>
                                <th style='border: 1px solid black; padding: 5px; text-align: center;'>Core</th>
                                <th style='border: 1px solid black; padding: 5px; text-align: center;'>Length<br/>m</th>
                                <th style='border: 1px solid black; padding: 5px; text-align: center;'>Temp. Factor<br/>Correction<br/>At 20°C</th>
                                <th style='border: 1px solid black; padding: 5px; text-align: center;'>Rt<br/>Ω/L</th>
                                <th style='border: 1px solid black; padding: 5px; text-align: center;'>R20°C<br/>Ω/km</th>
                                <th style='border: 1px solid black; padding: 5px; text-align: center;'>Specified</th>
                            </tr>");

                            sbDetails.Append(@"<tr>
                                <td style='border: 1px solid black; padding: 5px; text-align: center;'>" + Core[0] + @"</td>
                                <td style='border: 1px solid black; padding: 5px; text-align: center;'>" + length[0] + @"</td>
                                <td style='border: 1px solid black; padding: 5px; text-align: center;'>" + tempFactor[0] + @"</td>
                                <td style='border: 1px solid black; padding: 5px; text-align: center;'>" + rt[0] + @"</td>
                                <td style='border: 1px solid black; padding: 5px; text-align: center;'>" + rt20c[0] + @"</td>
                                <td style='border: 1px solid black; padding: 5px; text-align: center;font-weight: Bold; vertical-align: middle;' rowspan='" + Core.Count + @"'>Max.: 0.727</td>
                            </tr>");

                            // Add remaining core rows without the Specified cell (since it's merged)
                            for (int i = 1; i < Core.Count; i++)
                            {
                                sbDetails.Append(@"<tr>
                                    <td style='border: 1px solid black; padding: 5px; text-align: center;'>" + Core[i] + @"</td>
                                    <td style='border: 1px solid black; padding: 5px; text-align: center;'>" + length[i] + @"</td>
                                    <td style='border: 1px solid black; padding: 5px; text-align: center;'>" + tempFactor[i] + @"</td>
                                    <td style='border: 1px solid black; padding: 5px; text-align: center;'>" + rt[i] + @"</td>
                                    <td style='border: 1px solid black; padding: 5px; text-align: center;'>" + rt20c[i] + @"</td>
                                </tr>");
                            }

                            sbDetails.Append(@"</table>");

                            // Replace sbFooter with a signature section like in the image
                            sbFooter.Append(@" <table style='width:100%; font-family:Times New Roman; font-size:11px' class='tab1'>
                                <tr>
                                    <td rowspan='2' ><b>PT Qualis Indonesia</b></td>
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
                        }
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

                        body = objEmail.PrepareMailBodyWith("TestRecord_ConductorResistance.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString(), "{IssueNo}", IssueNo, "{Revision}", Revision, "{Title}", title, "{Details}", sb.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord_ConductorResistance-" + SampleID + ".html");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_ConductorResistance-" + SampleID + ".html"));
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