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
    public partial class CreateRequestCustomerFM_Transport : System.Web.UI.Page
    {
        CM _cm = new CM(); int InquiryID = 0; int ReportID = 0;
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Request.QueryString["ID"] != null)
            {
                InquiryID = Convert.ToInt32(Request.QueryString["ID"].ToString());
                ReportID = Convert.ToInt32(Request.QueryString["r"].ToString());
                Session["InquiryID"] = InquiryID;
                Session["ReportID"] = ReportID;
            }
            StringBuilder sb = new StringBuilder();
            StringBuilder sbSampleName = new StringBuilder();
            StringBuilder sbTestParameters = new StringBuilder();
            StringBuilder sbTestMethods = new StringBuilder();
            StringBuilder sbOtherDetails = new StringBuilder();
            StringBuilder sbServiceRequired = new StringBuilder();

            mailer objEmail = new mailer();
            StringReader sr = new StringReader(sb.ToString());


            string CustomerUID = "";
            string CustomerName = "";
            string Address = "";
            string ContactPerson = "";
            string Phone = "";
            string Fax = "";
            string Email = "";
            string Country = "";
            string BuyerName = "";
            string[] Samples = new string[100];
            string SampleDescription1 = "";
            string SampleDescription2 = "";
            string SampleDescription3 = "";
            string SampleQty = "";
            string ProductType = "";
            string PO = "";
            string ItemStyle = "";
            string CountryOfOrigin = "";
            string CountryOfDestination = "";
            string PayerName = "";
            string PayerContactPerson = "";
            string PayerContactEmail = "";
            string PayerContactNo = "";
            string BillingAddress = "";
            string SampleName = "";
            string BatchNo = "";
            string TestMethod = "";
            string TestParameter = "";
            string TP = "";
            string TestSubParameter = "";
            string TSP = "";
            string TestSubSubParameter = "";
            string Specification = "";
            string SizeOfSample = "";
            string Remarks = "";
            string InquiryNumber = "";

            if (Session["InquiryID"] != null)
            {
                Session["OrganizationID"] = 1;
                DataTable dtCustomer = _cm.GetCRFDetails(Convert.ToInt32(Session["InquiryID"].ToString()));
                if (dtCustomer.Rows.Count > 0)
                {
                    CustomerUID = dtCustomer.Rows[0]["CustomerUID"].ToString();
                    DataTable dt3 = _cm.getCRFCustomer(InquiryID.ToString());

                    if (dt3.Rows.Count > 0)
                    {
                        InquiryNumber = dt3.Rows[0]["InquiryID"].ToString();
                        CustomerName = dt3.Rows[0]["BuyerCustomerName"].ToString();
                        Address = dt3.Rows[0]["BuyerAddressLine1"].ToString() + " " + dt3.Rows[0]["BuyerAddressLine2"].ToString();
                        ContactPerson = dt3.Rows[0]["Title"].ToString() + " " + dt3.Rows[0]["FirstName"].ToString() + " " + dt3.Rows[0]["LastName"].ToString();
                        Phone = dt3.Rows[0]["Phone"].ToString();
                        Fax = dt3.Rows[0]["Fax"].ToString();
                        Email = dt3.Rows[0]["Email"].ToString();
                        Country = dt3.Rows[0]["BuyerCountry"].ToString();
                        BuyerName = dt3.Rows[0]["BuyerCustomerName"].ToString();
                        ProductType = dt3.Rows[0]["ProductType"].ToString();
                        PO = dt3.Rows[0]["PONo"].ToString();
                        ItemStyle = dt3.Rows[0]["ItemStyleNo"].ToString();
                        CountryOfOrigin = dt3.Rows[0]["CountryOrigin"].ToString();
                        CountryOfDestination = dt3.Rows[0]["CountryDestination"].ToString();
                        PayerName = dt3.Rows[0]["PayerName"].ToString();
                        PayerContactPerson = dt3.Rows[0]["PayerContactPerson"].ToString();
                        PayerContactEmail = dt3.Rows[0]["PayerContactEmail"].ToString();
                        PayerContactNo = dt3.Rows[0]["PayerContactNo"].ToString();
                        BillingAddress = Address;

                        if (dt3.Rows[0]["ServiceRequired"].ToString() == "Regular")
                        {
                            sbServiceRequired.Append("<input type='checkbox' id='Regular' name='Regular' value='Regular' checked='checked'>");
                            sbServiceRequired.Append("<label for='Regular'> Regular</label>");
                            sbServiceRequired.Append("<input type='checkbox' id='Express' name='Express' value='Express'>");
                            sbServiceRequired.Append("<label for='Express'> Express</label>");
                            sbServiceRequired.Append("<input type='checkbox' id='Urgent' name='Urgent' value='Urgent'>");
                            sbServiceRequired.Append("<label for='Urgent'> Urgent</label>");
                        }
                        else
                        if (dt3.Rows[0]["ServiceRequired"].ToString() == "Express")
                        {
                            sbServiceRequired.Append("<input type='checkbox' id='Regular' name='Regular' value='Regular'>");
                            sbServiceRequired.Append("<label for='Regular'> Regular</label>");
                            sbServiceRequired.Append("<input type='checkbox' id='Express' name='Express' value='Express' checked='checked'>");
                            sbServiceRequired.Append("<label for='Express'> Express</label>");
                            sbServiceRequired.Append("<input type='checkbox' id='Urgent' name='Urgent' value='Urgent'>");
                            sbServiceRequired.Append("<label for='Urgent'> Urgent</label>");
                        }
                        else
                        if (dt3.Rows[0]["ServiceRequired"].ToString() == "Urgent")
                        {
                            sbServiceRequired.Append("<input type='checkbox' id='Regular' name='Regular' value='Regular'>");
                            sbServiceRequired.Append("<label for='Regular'> Regular</label>");
                            sbServiceRequired.Append("<input type='checkbox' id='Express' name='Express' value='Express'>");
                            sbServiceRequired.Append("<label for='Express'> Express</label>");
                            sbServiceRequired.Append("<input type='checkbox' id='Urgent' name='Urgent' value='Urgent' checked='checked'>");
                            sbServiceRequired.Append("<label for='Urgent'> Urgent</label>");
                        }
                        sbOtherDetails.Append("<table class='tab'>");
                        if (dt3.Rows[0]["PayerContactNo"].ToString() == "Yes")
                        {
                            sbOtherDetails.Append("<tr><td>Sample Return:</td><td><input type='checkbox' id='yes' name='yes' value='yes' checked='checked'><label for='yes'> Yes</label></td>");
                            sbOtherDetails.Append("<td><input type='checkbox' id='no' name='no' value='no'><label for='no'> No</label></td><td>Report Recipient:</td>");
                        }
                        else
                        {
                            sbOtherDetails.Append("<tr><td>Sample Return:</td><td><input type='checkbox' id='yes' name='yes' value='yes'><label for='yes'> Yes</label></td>");
                            sbOtherDetails.Append("<td><input type='checkbox' id='no' name='no' value='no' checked='checked'><label for='no'> No</label></td><td>Report Recipient:</td>");
                        }
                        if (dt3.Rows[0]["ReportRecipient"].ToString() == "Applicant")
                        {
                            sbOtherDetails.Append("<td><input type='checkbox' id='applicant' name='applicant' value='applicant' checked='checked'><label for='applicant'> Applicant</label></td>");
                            sbOtherDetails.Append("<td><input type='checkbox' id='Payer' name='Payer' value='Payer'><label for='Payer'> Payer</label></td></tr>");
                        }
                        else
                        {
                            sbOtherDetails.Append("<td><input type='checkbox' id='applicant' name='applicant' value='applicant'><label for='applicant'> Applicant</label></td>");
                            sbOtherDetails.Append("<td><input type='checkbox' id='Payer' name='Payer' value='Payer' checked='checked'><label for='Payer'> Payer</label></td></tr>");
                        }
                        if (dt3.Rows[0]["InvoiceRecipient"].ToString() == "Applicant")
                        {
                            sbOtherDetails.Append("<tr><td>Invoice Recipient:</td><td colspan='2'><input type='checkbox' id='applicant' name='applicant' value='applicant' checked='checked'><label for='applicant'> Applicant</label></td>");
                            sbOtherDetails.Append("<td><input type='checkbox' id='other' name='other' value='other'><label for='other'> If other, provide the information:</label></td>");
                        }
                        else
                        {
                            sbOtherDetails.Append("<tr><td>Invoice Recipient:</td><td colspan='2'><input type='checkbox' id='applicant' name='applicant' value='applicant'><label for='applicant'> Applicant</label></td>");
                            sbOtherDetails.Append("<td><input type='checkbox' id='other' name='other' value='other' checked='checked'><label for='other'> If other, provide the information:</label></td>");
                        }

                        sbOtherDetails.Append("<tr><td>Payer's Name:</td><td colspan='2' style='border-bottom:1px solid !important'>" + PayerName + "</td>");
                        sbOtherDetails.Append("<td>Contact Person:</td><td colspan='2' style='border-bottom:1px solid !important'>" + PayerContactPerson + "</td></tr>");
                        sbOtherDetails.Append("<tr><td>E-mail Address:</td><td colspan='2' style='border-bottom:1px solid !important'>" + PayerContactEmail + "</td>");
                        sbOtherDetails.Append("<td>Contact No:</td><td colspan='2' style='border-bottom:1px solid !important'>" + PayerContactNo + "</td></tr>");
                        sbOtherDetails.Append("<tr><td>Billing Address:</td><td colspan='5' style='border-bottom:1px solid !important'>" + BillingAddress + "</td></tr>");
                        sbOtherDetails.Append("<tr><td colspan='6' style='text-align:center;'><br/><b>THE ABOVE REQUESTED TESTS ARE SUBJECT TO THE TERMS & CONDITIONS SET FORTH BY THE RESPECTIVE PT. QUALIS INDONESIA LABS.</b></td></tr>");
                        sbOtherDetails.Append("<tr><td colspan='3' style='text-align:center;'>Authorized Signature / Company Stamp / Date:<br /><br /><br /><br /><br /></td>");
                        sbOtherDetails.Append("<td colspan='3' style='text-align:center;'>Checked & Received by PT. QUALIS INDONESIA / Date & Time<br /><br /><br /><br /><br /></td></tr></table>");

                    }

                    //DataTable dtSample = _cm.GetCustomerCRFSample(InquiryID.ToString());
                    DataTable dtSample = _cm.GetCustomerCRFSample_Chemical(InquiryID.ToString());

                    int freqCounter = dtSample.Rows.Count;

                    if (dtSample.Rows.Count > 0)
                    {
                        for (int freqCount = 1; freqCount < dtSample.Rows.Count + 1; freqCount++)
                        {
                            SampleName = dtSample.Rows[freqCount - 1]["SampleName"].ToString();
                            Samples = SampleName.Split(',');

                            SampleQty = dtSample.Rows[freqCount - 1]["SizeOfSample"].ToString();
                            BatchNo = dtSample.Rows[freqCount - 1]["BatchNo"].ToString();
                            TestMethod = dtSample.Rows[freqCount - 1]["TestMethod"].ToString();
                            TestParameter = dtSample.Rows[freqCount - 1]["TestParameter"].ToString();

                            TestSubParameter = dtSample.Rows[freqCount - 1]["TestSubParameter"].ToString();
                            TestSubParameter = _cm.ReplaceSuperScript(TestSubParameter);
                            TestSubSubParameter = dtSample.Rows[freqCount - 1]["TestSubSubParameter"].ToString();
                            TestSubSubParameter = _cm.ReplaceSuperScript(TestSubSubParameter);


                            if (dtSample.Rows[freqCount - 1]["TestSubSubParameter"].ToString() != "")
                            {
                                if (TP != TestParameter)
                                {
                                    sbTestParameters.Append("<br/><b>" + TestParameter + "</b><br/>");
                                    sbTestMethods.Append("<br/><br/>");
                                    if (TSP != TestSubParameter)
                                    {
                                        sbTestParameters.Append(". " + TestSubParameter + "<br/>");
                                        sbTestParameters.Append("&nbsp; - " + TestSubSubParameter + "<br/>");
                                        sbTestMethods.Append("<br/>" + TestMethod + "<br/>");
                                        TSP = TestSubParameter;
                                    }
                                    else
                                    {
                                        sbTestParameters.Append("&nbsp; - " + TestSubSubParameter + "<br/>");
                                        sbTestMethods.Append("<br/>" + TestMethod + "<br/>");
                                    }
                                    TP = TestParameter;
                                }
                                else
                                {
                                    if (TSP != TestSubParameter)
                                    {
                                        sbTestParameters.Append(". " + TestSubParameter + "<br/>");
                                        sbTestParameters.Append("&nbsp; - " + TestSubSubParameter + "<br/>");
                                        sbTestMethods.Append("<br/>" + TestMethod + "<br/>");
                                        TSP = TestSubParameter;
                                    }
                                    else
                                    {
                                        sbTestParameters.Append("&nbsp; - " + TestSubSubParameter + "<br/>");
                                        sbTestMethods.Append(TestMethod + "<br/>");
                                    }
                                }
                            }
                            else
                            if (dtSample.Rows[freqCount - 1]["TestSubParameter"].ToString() != "")
                            {
                                if (TP != TestParameter)
                                {
                                    sbTestParameters.Append("<br/><b>" + TestParameter + "</b><br/>");
                                    sbTestMethods.Append("<br/><br/>");
                                    sbTestParameters.Append("- " + TestSubParameter + "<br/>");
                                    sbTestMethods.Append(TestMethod + "<br/>");
                                    TP = TestParameter;
                                }
                                else
                                {
                                    sbTestParameters.Append("- " + TestSubParameter + "<br/>");
                                    sbTestMethods.Append(TestMethod + "<br/>");
                                }
                            }
                            else
                            {
                                sbTestParameters.Append("- " + TestParameter + "<br/>");
                                sbTestMethods.Append(TestMethod + "<br/>");
                            }

                            TestSubParameter = dtSample.Rows[freqCount - 1]["TestSubParameter"].ToString();
                            Specification = dtSample.Rows[freqCount - 1]["Specification"].ToString();
                            SizeOfSample = dtSample.Rows[freqCount - 1]["SizeOfSample"].ToString();
                            Remarks = dtSample.Rows[freqCount - 1]["Remarks"].ToString();
                        }
                    }

                    string Location = ""; string DocumentNo = ""; string IssueNo = ""; string Revision = ""; string title = "";

                    DataTable dtDocument = _cm.getDocumentDetails("Test Request Form");

                    if (dtDocument.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtDocument.Rows)
                        {
                            Location = dr["Location"].ToString();
                            DocumentNo = dr["DocumentNo"].ToString();
                            IssueNo = dr["IssueNo"].ToString();
                            Revision = dr["Revision"].ToString();
                            title = dr["Title"].ToString();
                        }
                    }
                    int counter = 0;
                    foreach (string s in Samples)
                    {
                        if (s != "")
                        {
                            counter++;
                            sbSampleName.Append(s + ",<br/>");
                            if ((counter > 0) && (counter <= 5))
                            {
                                SampleDescription1 += s + ",";
                            }
                            if ((counter > 5) && (counter <= 10))
                            {
                                SampleDescription2 += s + ",";
                            }
                            if ((counter > 10) && (counter <= 15))
                            {
                                SampleDescription3 += s + ",";
                            }
                        }
                    }
                    if (SampleDescription1 == "") { SampleDescription1 = "&nbsp;"; }
                    if (SampleDescription2 == "") { SampleDescription2 = "&nbsp;"; }
                    if (SampleDescription3 == "") { SampleDescription3 = "&nbsp;"; }

                    //get Sample Name Test Parameter and testmethods for quotation
                    var CustomerInquiry = new DAL.CustomerInquiry();
                    //var CustomerInquiry = _cm.CustomerInquiry();
                    CustomerInquiry.SampleName = SampleDescription1;
                    CustomerInquiry.TestItems = sbTestParameters.ToString();
                    CustomerInquiry.TestMethods = sbTestMethods.ToString();
                    CustomerInquiry.InquiryID = InquiryID;
                    _cm.UpdateCustomerInquiryTestDetails(CustomerInquiry);


                    string body = objEmail.PrepareMailBodyWith("TAF_Transport.html",

                        "{InquiryNumber}", InquiryNumber,
                        "{CustomerName}", CustomerName,
                        "{Address}", Address,
                        "{ContactPerson}", ContactPerson,
                        "{Phone}", Phone,
                        "{Fax}", Fax,
                        "{Email}", Email,
                        "{BuyerName}", BuyerName,
                        "{Country}", Country,
                        "{SampleDescription1}", SampleDescription1,
                        "{SampleDescription2}", SampleDescription2,
                        "{SampleDescription3}", SampleDescription3,
                        "{SampleQty}", SampleQty,
                        "{ProductType}", ProductType,
                        "{PO}", PO,
                        "{CountryOfOrigin}", CountryOfOrigin,
                        "{ItemStyle}", ItemStyle,
                        "{CountryOfDestination}", CountryOfDestination,
                        "{SampleName}", sbSampleName.ToString(),
                        "{TestMethods}", sbTestMethods.ToString(),
                        "{TestParameters}", sbTestParameters.ToString(),
                        "{ServiceRequired}", sbServiceRequired.ToString(),
                        "{Details}", sb.ToString(),
                        "{OtherDetails}", sbOtherDetails.ToString());

                    string str = body.ToString();
                    string filename = Server.MapPath("//CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                    System.IO.File.WriteAllText(@filename, str);

                    //change customer inquiry Status to CRF Received
                    _cm.inquiryStatusId = 6;
                    _cm.UpdateInquiryStatus(_cm);
                    GC.Collect();
                    string html = File.ReadAllText(HttpContext.Current.Server.MapPath("//CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm"));
                    divRequest.InnerHtml = html;
                    // Response.Redirect("CustomerFM.aspx?ID=" + InquiryID + "&r=" + ReportID);
                }
            }

        }

        protected void rptParent_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            InquiryID = Convert.ToInt32(Request.QueryString["ID"].ToString());

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Label lblTestParameter = (Label)e.Item.FindControl("lblTestParameter");
                if (lblTestParameter != null)
                {
                    DataTable dt = _cm.getCustomerCRFSampleDetails(InquiryID, lblTestParameter.Text);
                    if (dt.Rows.Count > 0)
                    {
                        Repeater rptChild = (Repeater)e.Item.FindControl("rptChild");
                        rptChild.DataSource = dt;
                        rptChild.DataBind();
                    }

                }

            }
        }


    }
}