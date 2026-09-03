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
    public partial class CreateRequestCustomerMocon : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int InquiryID = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
                if (Request.QueryString["ID"] != null)
                {
                    if (Request.QueryString["ID"] != null)
                    {
                        InquiryID = Convert.ToInt32(Request.QueryString["ID"].ToString());
                        Session["InquiryID"] = InquiryID;
                    }
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
                    string Description = "";
                    string TestMethod = "";
                    string TestCondition = "";
                    string ExpectedValue = "";
                    string Dimensions = "";
                    string SizeOfSample = "";
                    string Remarks = "";
                    string CustomerUID = "";

                    DataTable dt = new DataTable();
                    DataTable dt1 = new DataTable();
                    DataTable dt2 = new DataTable();
                    int InquiryID = Convert.ToInt32(Session["InquiryID"].ToString());
                    dt = _cm.getInquiryDetails(InquiryID, 2);
                    if (dt.Rows.Count > 0)
                    {
                        int ContactPersonId = Convert.ToInt32(dt.Rows[0]["Personid"]);
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
                        dt3.Columns.AddRange(new DataColumn[8] {
                    new DataColumn("SampleName", typeof(string)),
                    new DataColumn("Description",typeof(string)),
                    new DataColumn("TestMethod",typeof(string)),
                    new DataColumn("TestCondition",typeof(string)),
                    new DataColumn("ExpectedValue",typeof(string)),
                    new DataColumn("Dimensions",typeof(string)),
                    new DataColumn("SizeOfSample",typeof(string)),
                    new DataColumn("Remarks",typeof(string))});

                        //string sql = "Select * from SampleDetails where CustomerRequestId='" + InquiryID + "'";
                        dt3 = _cm.getCustomerCRFSampleDetails(InquiryID);

                        int freqCounter = dt3.Rows.Count;

                        sb.Append(@"<table border='0' style='word-break: break-word;table-layout: fixed; width: 100%' cellspacing='0' cellpadding='0' class='table table-bordered table-striped table-hover js-basic-example dataTable' style='table-layout:fixed;width:100%'>
                                  <thead>
                                      <tr style = 'background-color:#DDDDDD' >
                                          <th class='no'>#</th>
                                          <th class='desc'>Sample ID & Complete Sample Structure</th>
                                          <th class='unit'>Description of Test</th>
                                          <th class='qty'>Test Method/Standard to be followed</th>
                                          <th class='unit'>Test Conditions (Temperature & RH%)<br>* For OTR- Test gas RH% & Carrier gas RH%)</th>
                                          <th class='qty'>Expected Value/Range</th>
                                          <th class='unit'>Dimensions<br>Sample Size/Thickness</th>
                                          <th class='qty'>No. of Samples</th>
                                          <th class='qty'>Remarks</th>
                                      </tr>
                                  </thead>
                                  <tbody>");

                        for (int freqCount = 1; freqCount < dt3.Rows.Count + 1; freqCount++)
                        {
                            sb.Append(@"<tr style = 'background-color:#f7f7f7'>");
                            {
                                SampleName = dt3.Rows[freqCount - 1]["SampleName"].ToString();
                                Description = dt3.Rows[freqCount - 1]["DescriptionOfTest"].ToString();
                                TestMethod = dt3.Rows[freqCount - 1]["TestMethod"].ToString();
                                TestCondition = dt3.Rows[freqCount - 1]["TestParameter"].ToString();
                                ExpectedValue = dt3.Rows[freqCount - 1]["ExpectedValue"].ToString();
                                Dimensions = dt3.Rows[freqCount - 1]["Dimensions"].ToString();
                                SizeOfSample = dt3.Rows[freqCount - 1]["SizeOfSample"].ToString();
                                Remarks = dt3.Rows[freqCount - 1]["Remarks"].ToString();

                                sb.Append("<td class='no'>" + freqCount.ToString("00") + "</td>");
                                sb.Append("<td>" + SampleName + "</td>");
                                sb.Append("<td>" + Description + "</td>");
                                sb.Append("<td>" + TestMethod + "</td>");
                                sb.Append("<td>" + TestCondition + "</td>");
                                sb.Append("<td>" + ExpectedValue + "</td>");
                                sb.Append("<td>" + Dimensions + "</td>");
                                sb.Append("<td>" + SizeOfSample + "</td>");
                                sb.Append("<td>" + Remarks + "</td>");
                            }
                            sb.Append("</tr>");
                        }
                        sb.Append(@"<tr>
                                    <td colspan = '9'><hr /></td>
                                </tr>
                                <tr>
                                    <td colspan = '9' style='text-align:left;'>
                                    <b>NOTE:</b> <br /><br />
                                      1) For WVTR Test, As per the ASTM F-1249 standard, 3 specimens of every sample need to test to cover the WVTR test under NABL Scope. And the charges for the test will be accordingly increased. If Customer want single specimen testing, the parameter will be not cover under NABL Scope and seperate report will be provided without NABL logo.<br /><br />
                                      2) Please mention sample Structure/type like Monolayer Film, Multilayer Laminate, Foil Laminate, Metallised Film Sample, Coated Film Sample, PVDC/PVC Sample, Paper base Laminate Sample in Sample I.D as testing charges are depends on sample type.<br /><br />
                                      3) Please Mark Barrier/Coated side on the sample for WVTR/OTR tests.  Mark Machine Direction/Tranverse Direction on the sample for other mechanical /Physical testing. (e.g.-Bond/Seal/Tensile/Tear/Friction Test etc).<br /><br />
                                      4) WVTR/OTR Test samples should be free of Handling Marks, Scratches, creases, dust, pinholes, fingerprints and should be atleast in A4 size.<br /><br />
                                      5) Prior intimation should be given for sending the samples back. Samples will be tested as per the test method/standard and conditions specified by customer and data will be provided based on that.<br /><br />
                                      6) *** Please Submit this Customer Request Form along with every consignment of sample. It is Mandatory & on the basis of filled CRF only we are able to submit the Quotation.<br /><br />
                                    </td>
                                </tr>                                
                                <tr>
                                    <td colspan = '9'><hr /></td>
                                </tr>                                
                              </tbody>
                              </table>");

                        string body = objEmail.PrepareMailBodyWith("CRF-Mocon.html", "Company Name", CompanyName, "{Address}", Address, "Phone", Phone, "Fax", fax, "{Email}", Email, "{Contact Person}", ContactPerson, "{Designation}", Designation, "{Department}", Department, "{Mobile}", Mobile, "{ShippingAgency}", ShippingAgency, "{AirwayBillNo}", AirwayBillNo, "{InquiryID}", InquiryID.ToString(), "{Date}", DateOfRequest.ToString("dd MMM yyyy"), "{SampleName}", SampleName, "{Description}", Description, "{TestMethod}", TestMethod, "{TestCondition}", TestCondition, "{ExpectedValue}", ExpectedValue, "{Dimensions}", Dimensions, "{SizeOfSample}", SizeOfSample, "{Remarks}", Remarks, " {SampleDetails}", sb.ToString());
                        string str = body.ToString();
                        string filename = Server.MapPath("//CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                        System.IO.File.WriteAllText(@filename, str);

                        //change customer inquiry Status to CRF Received
                        _cm.inquiryStatusId = 6;
						_cm.UpdateInquiryStatus(_cm);
						GC.Collect();
                        string html = File.ReadAllText(HttpContext.Current.Server.MapPath("//CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm"));
                        divRequest.InnerHtml = html;
                        //divRequest.Visible = false;
                        //divSuccess.Visible = true;
                }
            }
        }
    }
}