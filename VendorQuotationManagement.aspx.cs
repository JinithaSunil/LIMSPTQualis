using System;
using System.Collections;
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
    public partial class VendorQuotationManagement : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        DAL.mailer objEmail = new DAL.mailer();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    BindQuotation();
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }

        private void BindQuotation()
        {
            DataTable dt = _cm.GetQuotationDetails();
            if (dt.Rows.Count > 0)
            {
                gvQuotation.DataSource = dt;
                gvQuotation.DataBind();
            }
        }

        protected void gvQuotation_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Send")
            {
                int rowIndex = int.Parse(e.CommandArgument.ToString());
                int VendorQuotationID = (int)this.gvQuotation.DataKeys[rowIndex]["VendorQuotationID"];
                int VendorID = (int)this.gvQuotation.DataKeys[rowIndex]["VendorID2"];
                int EnquiryID = (int)this.gvQuotation.DataKeys[rowIndex]["EnquiryID"];

                _cm.QuotationStatusID = 6;
                _cm.VendorQuotationID = VendorQuotationID;
                _cm.VendorID = VendorID;

                // Send the email
                SendMail(VendorID.ToString(), VendorQuotationID.ToString(), EnquiryID.ToString());

                // Check if email was sent successfully
                bool emailSentSuccessfully = false;
                if (Session["Failed"] != null)
                {
                    if (Session["Failed"].ToString() == "0")
                    {
                        _cm.UpdateVendorQuotationStatus(_cm);
                        emailSentSuccessfully = true;
                    }
                }

                // Update the specific row in GridView
                if (emailSentSuccessfully)
                {
                    GridViewRow row = gvQuotation.Rows[rowIndex];

                    // Find and hide the Resend button
                    Button btnResend = (Button)row.FindControl("btnResend");
                    if (btnResend != null)
                    {
                        btnResend.Visible = false;
                    }

                    // Find and show the success label
                    Label lblMailSent = (Label)row.FindControl("lblMailSent");
                    if (lblMailSent != null)
                    {
                        lblMailSent.Visible = true;
                        lblMailSent.Text = "Mail sent for Revision";
                    }

                    // Optional: Show success message
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                        "alert('Email sent successfully for revision!');", true);
                }
                else
                {
                    // Optional: Show error message
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                        "alert('Failed to send email. Please try again.');", true);
                }
            }
        }
        private void SendMail(string VendorID, string VendorQuotationID, string EnquiryID)
        {
            StringBuilder sb = new StringBuilder();
            StringReader sr = new StringReader(sb.ToString());

            StringBuilder sbEnq = new StringBuilder();
            StringReader srEnq = new StringReader(sbEnq.ToString());

            _cm.VendorID = Convert.ToInt32(VendorID);
            DataTable dtQuotation = _cm.GetQuotationRevision(VendorQuotationID, VendorID);
            if (dtQuotation.Rows.Count > 0)
            {
                _cm.Revision = Convert.ToInt32(dtQuotation.Rows[0]["Revision"].ToString());
            }

            _cm.EnquiryID = Convert.ToInt32(EnquiryID);
            DataTable dtVendor = _cm.GetVendorReference(VendorID);
            DataTable dtVendorEmail = _cm.GetVendorEmail(VendorID);
            if (dtVendor.Rows.Count > 0 && dtVendorEmail.Rows.Count > 0)
            {
                _cm.VendorName = dtVendor.Rows[0]["VendorName"].ToString();
                _cm.Reference = dtVendor.Rows[0]["Reference"].ToString();

                string Subject = "";
                string FromAddress = "";
                string ToAddress = dtVendorEmail.Rows[0]["ContactEmail"].ToString();
                string CCAddress = "";
                string BCCAddress = "";
                string Body = "";
                string Note = "";

                string EnqID = EnquiryID;
                DataTable dte = _cm.GetEnquiryNote(EnqID);
                if (dte.Rows.Count > 0)
                {
                    Note = dte.Rows[0]["Remarks"].ToString();
                }

                DataTable dtEmailDetails = _cm.getEmaildetails("Float Enquiry");
                if (dtEmailDetails.Rows.Count > 0)
                {
                    if (dtEmailDetails.Rows[0]["EmailSubject"].ToString() != "")
                    {
                        Subject = "Enquiry#" + _cm.EnquiryID + dtEmailDetails.Rows[0]["EmailSubject"].ToString();
                    }
                    if (dtEmailDetails.Rows[0]["FromAddress"].ToString() != "")
                    {
                        FromAddress = dtEmailDetails.Rows[0]["FromAddress"].ToString();
                    }
                    if (dtEmailDetails.Rows[0]["CCAddress"].ToString() != "")
                    {
                        CCAddress = dtEmailDetails.Rows[0]["CCAddress"].ToString();
                    }
                    if (dtEmailDetails.Rows[0]["BCCAddress"].ToString() != "")
                    {
                        BCCAddress = dtEmailDetails.Rows[0]["BCCAddress"].ToString();

                    }
					string baseUrl = "http://localhost:41414/"; //  default
					DataTable dtOrg = _cm.GetOrganizationDetails();
					if (dtOrg.Rows.Count > 0 && dtOrg.Columns.Contains("url"))
					{
						baseUrl = dtOrg.Rows[0]["url"].ToString();
						if (!baseUrl.EndsWith("/"))
							baseUrl += "/";
					}
                    sb.AppendFormat(string.Format("<a href='"+baseUrl+"EnquiryForm.aspx?VendorQuotationID={0}&Ven={1}' target='_blank'>", _cm.VendorQuotationID, _cm.VendorID));
                    sb.AppendFormat("Quotation");
                    sb.Append("</a>");

                    //var mailMessage = new MailMessage
                    //{
                    //    From = new MailAddress(FromAddress),
                    //    Subject = Subject,
                    //    Body = objEmail.PrepareMailBodyWith("Enquiry.html", "{VendorName}", _cm.VendorName, "{Link}", sb.ToString(), "{Note}", Note),
                    //    IsBodyHtml = true,
                    //};

                    string body = objEmail.PrepareMailBodyWith("Enquiry.html", "{VendorName}", _cm.VendorName, "{Link}", sb.ToString(), "{Note}", Note);
                    string str = body.ToString();

                    string filename = Server.MapPath("//Enquiry/Enquiry-" + _cm.EnquiryID + _cm.Reference + "-" + _cm.Revision + ".html");
                    System.IO.File.WriteAllText(@filename, str);

                    DataTable dtEnq = _cm.GetEnquiryDocuments(EnquiryID);
                    if (dtEnq.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtEnq.Rows)
                        {
                            string html = HttpContext.Current.Server.MapPath("//EnquiryDocuments/" + dr["FileName"].ToString());
                            sbEnq.Append(html + ";");
                        }
                    }
                    _cm.FromAddress = FromAddress;
                    _cm.ToAddress = ToAddress;
                    _cm.CCAddress = CCAddress;
                    _cm.BCCAddress = BCCAddress;
                    _cm.Subject = Subject;
                    _cm.Purpose = "Enquiry Resend";
                    _cm.FileName = "EnquiryRevisition.html";
                    _cm.SentTo = _cm.VendorName;
                    _cm.Body = body;


                    try
                    {
                        if (sbEnq.Length > 0)
                        {
                            objEmail.SendMail(FromAddress, ToAddress, Subject, body, sbEnq.ToString().TrimEnd(';'));
                        }
                        else
                        {
                            objEmail.SendMail(FromAddress, ToAddress, Subject, body);
                        }

                        if (Session["Failed"].ToString() == "0")
                        {
                            _cm.SentStatus1 = true;
                            _cm.AddMailBoxDetails(_cm);
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email Sent!');", true);
                        }
                        else
                        {
                            _cm.SentStatus1 = false;
                            _cm.AddMailBoxDetails(_cm);
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email Send Failed!');", true);
                        }
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email Send Failed. Try again Later!');", true);
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('No Vendor Email Found! Kindly update Vendor Details!');", true);
                }
            }
        }
        public DataTable RemoveDuplicateRows(DataTable table, string DistinctColumn)
        {
            try
            {
                ArrayList UniqueRecords = new ArrayList();
                ArrayList DuplicateRecords = new ArrayList();
                foreach (DataRow dRow in table.Rows)
                {
                    if (UniqueRecords.Contains(dRow[DistinctColumn]))
                        DuplicateRecords.Add(dRow);
                    else
                        UniqueRecords.Add(dRow[DistinctColumn]);
                }

                foreach (DataRow dRow in DuplicateRecords)
                {
                    table.Rows.Remove(dRow);
                }

                return table;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        protected void gvQuotation_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                HyperLink hypVendorQuotationID = (e.Row.FindControl("hypVendorQuotationID") as HyperLink);
                HiddenField hdVendorQuotationID = (e.Row.FindControl("hdVendorQuotationID") as HiddenField);
                HiddenField hdVendorID = (e.Row.FindControl("hdVendorID") as HiddenField);
                hypVendorQuotationID.NavigateUrl = "ViewQuotation.aspx?ID1=" + hdVendorQuotationID.Value + "&ID2=" + hdVendorID.Value;
            }
        }
    }
}