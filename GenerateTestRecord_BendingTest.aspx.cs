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
	public partial class GenerateTestRecord_BendingTest : System.Web.UI.Page
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
                        List<string> ForSingleCoreCables = new List<string>();
                        List<string> ForThreeCoreCables = new List<string>();
                      

                        string OuterDiameter = "";
                        string ConductorDiameter = "";
                        string BendingDiameter = "";
                        string TestCylinder = "";
                        string result = "";
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
                      

                        dtRecordDetails = _cm.getTestRecord(ReportId,SampleID);
                        if (dtRecordDetails.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtRecordDetails.Rows)
                            {
                                if (dr["ReportParamMapID"].ToString() == "4146")
                                {
                                    ForSingleCoreCables = new List<string>(dr["Result"].ToString().Split(','));
                                }

                                if (dr["ReportParamMapID"].ToString() == "4147")
                                {
                                    ForThreeCoreCables = new List<string>(dr["Result"].ToString().Split(','));
                                }

                                if (dr["ReportParamMapID"].ToString() == "4148") { OuterDiameter = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4149") { ConductorDiameter = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4152") { BendingDiameter = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4151") { TestCylinder = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4153") { Type = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4154") { Size = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4155") { Voltage = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4156") { Date = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4157") { RoomNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4158") { RoomTemp = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4159") { RefSpec = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4160") { TestMethodNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4161") { JobOrderNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4162") { WorkSheetNo = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4163") { RoomHumidity = dr["Result"].ToString(); }
                                if (dr["ReportParamMapID"].ToString() == "4164") { EquipmentNo = dr["Result"].ToString(); }

                                if (dr["ReportParamMapID"].ToString() == "4150") { result = dr["Result"].ToString(); }
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

                            // Replace sbDetails with this new section to match the characteristic data table
                            sbDetails.Append(@"<table style='width: 100%; border: 1px solid black; border-collapse: collapse; margin-top: 10px;'>
                            <tr>
                                <td colspan='5' style='border: 1px solid black; padding: 5px; background-color: #bebebe;'><b>Characteristic of data</b> <br/>  <br/> Bending diameter </td>
                            </tr>
                            <tr>
                                <td colspan='2' style='border: 1px solid black; padding: 5px;'>a) for cable with a lead sheath, corrugated metallic,<br /> sheath or metal foil</td>
                                <td colspan='3' style='border: 1px solid black; padding: 5px;'>b) for other cables</td>
                            </tr>
                            <tr>
                                <td colspan='2' style='border: 1px solid black; padding: 5px;'>-for single-core cables    : " +
                            (ForSingleCoreCables.Count > 0 ? ForSingleCoreCables[0] : "x") + @"</td>
                                <td colspan='3' style='border: 1px solid black; padding: 5px;'>-for single-core cables    : " +
                            (ForSingleCoreCables.Count > 1 ? ForSingleCoreCables[1] : "x") + @"</td>
                            </tr>
                            <tr>
                                <td colspan='2' style='border: 1px solid black; padding: 5px;'>-for three-core cables    : " +
                            (ForThreeCoreCables.Count > 0 ? ForThreeCoreCables[0] : "--") + @"</td>
                                <td colspan='3' style='border: 1px solid black; padding: 5px;'>-for three-core cables    : " +
                            (ForThreeCoreCables.Count > 1 ? ForThreeCoreCables[1] : "--") + @"</td>
                            </tr>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px; text-align: center; background-color:#bebebe;font-weight: bold;'>Outer diameter of<br/> cable<br/>D</td>
                                <td style='border: 1px solid black; padding: 5px; text-align: center; background-color:#bebebe;font-weight: bold;'>Diameter cable<br/>conductor<br/>d</td>
                                <td style='border: 1px solid black; padding: 5px; text-align: center; background-color:#bebebe;font-weight: bold;'>Maximum diameter<br/>bending diameter<br/>Dr</td>
                                <td style='border: 1px solid black; padding: 5px; text-align: center; background-color:#bebebe;font-weight: bold;'>Diameter of test<br/>Cylinder<br/>Dt</td>
                                <td rowspan='2' style='border: 1px solid black; padding: 5px; text-align: center; background-color:#bebebe;font-weight: bold;'>Results</td>
                            </tr>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px; text-align: center;background-color:#bebebe;font-weight: bold;'>(mm)</td>
                                <td style='border: 1px solid black; padding: 5px; text-align: center;background-color:#bebebe;font-weight: bold;'>(mm)</td>
                                <td style='border: 1px solid black; padding: 5px; text-align: center;background-color:#bebebe;font-weight: bold;'>(mm)</td>
                                <td style='border: 1px solid black; padding: 5px; text-align: center;background-color:#bebebe;font-weight: bold;'>(mm)</td>
      
                            </tr>
                            <tr>
                                <td style='border: 1px solid black; padding: 5px; text-align: center;'>" + OuterDiameter + @"</td>
                                <td style='border: 1px solid black; padding: 5px; text-align: center;'>" + ConductorDiameter + @"</td>
                                <td style='border: 1px solid black; padding: 5px; text-align: center;'>Dr Max.:" + BendingDiameter + @"</td>
                                <td style='border: 1px solid black; padding: 5px; text-align: center;'>" + TestCylinder + @"</td>
                                <td style='border: 1px solid black; padding: 5px; text-align: center;'>" + result + @"</td>
                            </tr>
                        </table>");

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

                        body = objEmail.PrepareMailBodyWith("TestRecord_BendingTest.html", "{Header}", sbHeader.ToString(), "{TestDetails}", sbDetails.ToString(), "{Dates}", sbDates.ToString(), "{Footer}", sbFooter.ToString(), "{IssueNo}", IssueNo, "{Revision}", Revision, "{Title}", title, "{Details}", sb.ToString());
                        str = body.ToString();
                        filename = Server.MapPath("//TestRecord/TestRecord_BendingTest-" + SampleID + ".html");
                        System.IO.File.WriteAllText(@filename, str);
                        html = File.ReadAllText(HttpContext.Current.Server.MapPath("//TestRecord/TestRecord_BendingTest-" + SampleID + ".html"));
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