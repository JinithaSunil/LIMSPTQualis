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
    public partial class CreateRequestCustomerPTI : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int InquiryID = 0;int CloneCRFSampleID = 0;
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Request.QueryString["ID"] != null)
            {
                InquiryID = Convert.ToInt32(Request.QueryString["ID"].ToString());
                Session["InquiryID"] = InquiryID;
            }
            if (Session["InquiryID"] != null)
                {
                    string CompanyName = "";
                    string Address = "";
                    string Phone = "";
                    string fax = "";
                    string Email = "";
                    string ContactPerson = "";
                    string Designation = "";
                    string Department = "";
                    string Mobile = "";
                    string ShippingAgency = "";
                    string AirwayBillNo = "";
                    DateTime DateOfRequest = new DateTime();
                    string SampleName = "";
                    string TestParameter = "";
                    string TestMethod = "";
                    string SampleType = "";
                    string SizeOfSample = "";
                    string Remarks = "";
                    string CustomerUID = "";

                    DataTable dt = new DataTable();
                    DataTable dt1 = new DataTable();
                    DataTable dt2 = new DataTable();
                    int InquiryID = Convert.ToInt32(Session["InquiryID"].ToString());
                    dt = _cm.getInquiryDetails(InquiryID, 3);

                    if (dt.Rows.Count > 0)
                    {
                        int ContactPersonId = Convert.ToInt32(dt.Rows[0]["ContactPerson"]);
                        foreach (DataRow dr in dt.Rows)
                        {
                            InquiryID = Convert.ToInt32(dr["InquiryID"].ToString());
                            CustomerUID = dr["CustomerUID"].ToString();
                            CompanyName = dr["CustomerName"].ToString();
                            Address = dr["Address"].ToString();
                            ShippingAgency = dr["ShippingAgency"].ToString();
                            AirwayBillNo = dr["AirwayBillNo"].ToString();
                            DateOfRequest = Convert.ToDateTime(dr["Created"].ToString());
                        }
                        dt2 = _cm.getContactPersonDetails(ContactPersonId);
                        foreach (DataRow dr in dt2.Rows)
                        {
                            if (dr["Mobile"].ToString() != "") { Mobile = dr["Mobile"].ToString(); }
                            if (dr["Email"].ToString() != "") { Email = dr["Email"].ToString(); }
                            if (dr["Phone"].ToString() != "") { Phone = dr["Phone"].ToString(); }
                            Designation = dr["Designation"].ToString();
                            Department = dr["Department"].ToString();
                        }

                        foreach (DataRow dr2 in dt2.Rows)
                        {
                            ContactPerson = dr2["CustomerName"].ToString();
                        }
                        StringBuilder sb = new StringBuilder();
                        mailer objEmail = new mailer();
                        StringReader sr = new StringReader(sb.ToString());

                        DataTable dt3 = new DataTable();
                        dt3.Columns.AddRange(new DataColumn[6] {
                    new DataColumn("SampleName", typeof(string)),
                    new DataColumn("TestParameter",typeof(string)),
                    new DataColumn("TestMethod",typeof(string)),
                    new DataColumn("SampleType",typeof(string)),
                    new DataColumn("SizeOfSample",typeof(string)),
                    new DataColumn("Remarks",typeof(string))});

                        dt3 = _cm.getCustomerCRFSampleDetails(InquiryID);
                        int freqCounter = dt3.Rows.Count;


                        sb.Append(@"<table border='0' cellspacing='0' cellpadding='0' class='table table-bordered table-striped table-hover js-basic-example dataTable'>
                                  <thead >
                                      <tr style='background-color:#DDDDDD'>
                                          <th class='no' width='5%'>#</th>
                                          <th class='desc' width='20%'>Sample ID</th>
                                          <th class='unit' width='15%'>TEST PARAMETER</th>
                                          <th class='qty' width='15%'>TEST METHOD</th>
                                          <th class='qty' width='15%'>TYPE OF SAMPLES </th>
                                          <th class='qty' width='15%'>No of Samples (If Resin then No of Pouches/ Wt of sample,If Preform and/or  Bottles then specify Nos) </th>
                                          <th class='qty' width='15%'>Remark (Or any other relevant information regarding the test / contract) </th>
                                      </tr>
                                  </thead>
                                  <tbody>");

                        for (int freqCount = 1; freqCount < dt3.Rows.Count + 1; freqCount++)
                        {
                            sb.Append(@"<tr style = 'background-color:#f7f7f7'>");
                            {
                                SampleName = dt3.Rows[freqCount - 1]["SampleName"].ToString();
                                TestParameter = dt3.Rows[freqCount - 1]["TestParameter"].ToString();
                                TestMethod = dt3.Rows[freqCount - 1]["TestMethod"].ToString();
                                SampleType = dt3.Rows[freqCount - 1]["SampleType"].ToString();
                                SizeOfSample = dt3.Rows[freqCount - 1]["SizeOfSample"].ToString();
                                Remarks = dt3.Rows[freqCount - 1]["Remarks"].ToString();

                                sb.Append("<td class='no'>" + freqCount.ToString("00") + "</td>");
                                sb.Append("<td>" + SampleName + "</td>");
                                sb.Append("<td>" + TestParameter + "</td>");
                                sb.Append("<td>" + TestMethod + "</td>");
                                sb.Append("<td>" + SampleType + "</td>");
                                sb.Append("<td>" + SizeOfSample + "</td>");
                                sb.Append("<td>" + Remarks + "</td>");
                            }
                            sb.Append("</tr>");
                        }
                        sb.Append(@"<tr>
                                    <td colspan = '8'><hr /></td>
                                </tr>
                                <tr>
                                    <td colspan = '8' style='text-align:left;'>
                                    <b>NOTE:</b><br /> Test samples (resin/preforms/bottles) should be packed in proper sealed plastic pouch which should be moisture free and should kept in proper box to courier.For Moisture content Test resin/granuals samples should be packed in Vaccum sealed Glass Jar,For Colour-Haze Test Samples(like preforms/bottles) should be free from damage,dent,fingerprint,scraches on it.The Prior intimation should be given for sending the samples back & for saperate test report for all samples. All the testing will be done as per global standard.
                                    </td>
                                </tr>                                
                                <tr>
                                    <td colspan = '8'><hr /></td>
                                </tr>
                                
                              </tbody>
                              </table>");

                        string body = objEmail.PrepareMailBodyWith("CRF-PTI.html", "Company Name", CompanyName, "Address", Address, "Phone", Phone, "Fax", fax, "{Email}", Email, "{Contact Person}", ContactPerson, "{Designation}", Designation, "{Department}", Department, "{Mobile}", Mobile, "{ShippingAgency}", ShippingAgency, "{AirwayBillNo}", AirwayBillNo, "{InquiryID}", InquiryID.ToString(), "{Date}", DateOfRequest.ToString("dd MMM yyyy"), " {SampleDetails}", sb.ToString());
                        string str = body.ToString();
                        string filename = Server.MapPath("//CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                        System.IO.File.WriteAllText(@filename, str);

                        //change customer inquiry Status to CRF Received
                        _cm.inquiryStatusId = 6;
						_cm.UpdateInquiryStatus(_cm);
						GC.Collect();
                        string html = File.ReadAllText(HttpContext.Current.Server.MapPath("//CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm"));
                        divRequest.InnerHtml = html;
                    }
                }
            
        }
    }
}