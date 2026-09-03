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
    public partial class GenerateVendorDetailsReport : System.Web.UI.Page
    {
        DAL.mailer objEmail = new DAL.mailer();
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["VendorID"] != null)
                {
                    Session["VendorID"] = Request.QueryString["VendorID"].ToString();
                }
                if (Session["VendorID"] != null)
                {
                    string VendorID = Session["VendorID"].ToString();

                    string SLNO = "";
                    string VendorName = "";
                    string CompanyName = "";
                    string DealerName = "";
                    string Website = "";
                    string VAT = "";
                    string YearofEstablishment = "";
                    string TypeofBusiness = "";
                    string TradeLicence = "";
                    string YearlyTurnover1 = "";
                    string YearlyTurnover2 = "";
                    string YearlyTurnover3 = "";
                    string RequiredCredit = "";
                    string ApprovedCredit = "";
                    string ContactMobile = "";
                    string ContactEmail = "";
                    string ContactPerson = "";
                    string AddressLine1 = "";
                    string AddressLine2 = "";
                    string AddressLine3 = "";
                    string City = "";
                    string State = "";
                    string Country = "";
                    string CreatedDate = "";
                    string PanNumber = "";
                    string AdharNumber = "";
                    string Reference = "";
                    string LocationType = "";
                    string AccountName = "";
                    string AccountNumber = "";
                    string BankName = "";
                    string BankBranch = "";
                    string IFSCode = "";
                    string Type = "";
                    string SwiftCode = "";
                    string NumberofEmployees = "";
                    string Year1 = "";
                    string Year2 = "";
                    string Year3 = "";
                    string RegistrationNumber = "";
                    string DocumentType = "";
                    string DocumentID = "";
                    string TypeOfEstablishment = "";
                    string PFNumber = "";
                    string UANNumber = "";
                    string ESICNumber = "";

                    // Get vendor basic details (single row from registered office)
                    DataTable dt = _cm.GetVendorDetailsforreport(VendorID);

                    // Get all contacts and locations
                    DataTable dtContacts = _cm.GetVendorContactsAndLocations(VendorID);

                    DataTable dt3 = new DataTable();
                    DataTable dtDocument = new DataTable();

                    StringBuilder sb = new StringBuilder();
                    StringBuilder sbBank = new StringBuilder();
                    StringBuilder sbDocument = new StringBuilder();

                    // Process basic vendor details
                    if (dt.Rows.Count > 0)
                    {
                        VendorName = dt.Rows[0]["VendorName"].ToString();
                        //CompanyName = dt.Rows[0]["CompanyName"].ToString();
                        DealerName = dt.Rows[0]["DealerName"].ToString();
                        Website = dt.Rows[0]["Website"].ToString();
                        VAT = dt.Rows[0]["VAT"].ToString();
                        YearofEstablishment = dt.Rows[0]["YearOfEstablishment"].ToString();
                        TypeofBusiness = dt.Rows[0]["TypeOfBusiness"].ToString();
                        TypeOfEstablishment = dt.Rows[0]["TypeOfEstablishment"].ToString();
                        YearlyTurnover1 = dt.Rows[0]["YearlyTurnover1"].ToString();
                        YearlyTurnover2 = dt.Rows[0]["YearlyTurnover2"].ToString();
                        YearlyTurnover3 = dt.Rows[0]["YearlyTurnover3"].ToString();
                        ApprovedCredit = dt.Rows[0]["ApprovedCredit"].ToString();

                        // For main vendor info, get from registered office only
                        ContactMobile = dt.Rows[0]["ContactMobile"].ToString();
                        ContactEmail = dt.Rows[0]["ContactEmail"].ToString();
                        ContactPerson = dt.Rows[0]["Title"].ToString() + " " + dt.Rows[0]["FirstName"].ToString() + " " + dt.Rows[0]["LastName"].ToString();

                        // Build main address from first row
                        string addr1 = dt.Rows[0]["AddressLine1"].ToString();
                        string addr2 = dt.Rows[0]["AddressLine2"].ToString();
                        string addr3 = dt.Rows[0]["AddressLine3"].ToString();
                        string city = dt.Rows[0]["City"].ToString();
                        string state = dt.Rows[0]["State"].ToString();
                        string country = dt.Rows[0]["Country"].ToString();

                        AddressLine1 = addr1 + (!string.IsNullOrEmpty(addr2) ? addr2 : "") +
                                       (!string.IsNullOrEmpty(addr3) ? ", <br/>" + addr3 : "") +
                                       (!string.IsNullOrEmpty(city) ? city : "") + ", <br/>" +
                                       (!string.IsNullOrEmpty(state) ? state : "") +
                                       (!string.IsNullOrEmpty(country) ? ", " + country : "");

                        City = city;
                        State = state;
                        Country = country;

                        DateTime Date = Convert.ToDateTime(dt.Rows[0]["CreatedDate"].ToString());
                        CreatedDate = Date.ToString("dd/MMM/yyyy");

                        PanNumber = dt.Rows[0]["PanNumber"].ToString();
                        AdharNumber = dt.Rows[0]["AdharNumber"].ToString();
                        Reference = dt.Rows[0]["VendorCode"].ToString();
                        Year1 = dt.Rows[0]["Year1"].ToString();
                        Year2 = dt.Rows[0]["Year2"].ToString();
                        Year3 = dt.Rows[0]["Year3"].ToString();
                        NumberofEmployees = dt.Rows[0]["NumberofEmployee"].ToString();

                        if (!string.IsNullOrEmpty(dt.Rows[0]["PFNumber"].ToString()))
                            PFNumber = dt.Rows[0]["PFNumber"].ToString();
                        else
                            PFNumber = "-";

                        UANNumber = dt.Rows[0]["UANNumber"].ToString();
                        ESICNumber = dt.Rows[0]["ESIC"].ToString();
                        RegistrationNumber = dt.Rows[0]["RegistrationNumber"].ToString();
                    }

                    // Build contacts table with improved layout (3 columns per row)
                    if (dtContacts.Rows.Count > 0)
                    {
                        sb.Append("<table cellpadding='5' cellspacing='0' width='100%' style='font-size: 14px;font-family:Times New Roman; border-collapse: collapse;'>");

                        // Process contacts in groups of 3 per row
                        for (int i = 0; i < dtContacts.Rows.Count; i += 3)
                        {
                            sb.Append("<tr>");

                            // Add up to 3 contacts per row
                            for (int col = 0; col < 3 && (i + col) < dtContacts.Rows.Count; col++)
                            {
                                int currentIndex = i + col;

                                sb.Append("<td style='width:33.33%; vertical-align:top; padding:15px; border: 1px solid #ddd;'>");

                                // Get all data from current row only - using correct column names
                                string contactPerson = dtContacts.Rows[currentIndex]["Title"].ToString() + " " +
                                                      dtContacts.Rows[currentIndex]["FirstName"].ToString() + " " +
                                                      dtContacts.Rows[currentIndex]["LastName"].ToString();
                                string contactMobile = dtContacts.Rows[currentIndex]["ContactMobile"].ToString();
                                string contactEmail = dtContacts.Rows[currentIndex]["ContactEmail"].ToString();
                                string locationType = dtContacts.Rows[currentIndex]["LocationType"].ToString();

                                // Build complete address from current row
                                string fullAddress = dtContacts.Rows[currentIndex]["AddressLine1"].ToString();
                                if (!string.IsNullOrEmpty(dtContacts.Rows[currentIndex]["AddressLine2"].ToString()))
                                    fullAddress += ", " + dtContacts.Rows[currentIndex]["AddressLine2"].ToString();
                                if (!string.IsNullOrEmpty(dtContacts.Rows[currentIndex]["AddressLine3"].ToString()))
                                    fullAddress += ", " + dtContacts.Rows[currentIndex]["AddressLine3"].ToString();
                                if (!string.IsNullOrEmpty(dtContacts.Rows[currentIndex]["City"].ToString()))
                                    fullAddress += ", " + dtContacts.Rows[currentIndex]["City"].ToString();
                                if (!string.IsNullOrEmpty(dtContacts.Rows[currentIndex]["State"].ToString()))
                                    fullAddress += ", " + dtContacts.Rows[currentIndex]["State"].ToString();
                                if (!string.IsNullOrEmpty(dtContacts.Rows[currentIndex]["Country"].ToString()))
                                    fullAddress += ", " + dtContacts.Rows[currentIndex]["Country"].ToString();

                                sb.Append("<div style='background-color: #f8f9fa; padding: 8px; margin-bottom: 10px; border-radius: 4px;'>");
                                sb.Append("<h6 style='margin: 0; color: #495057; font-weight: bold;'>" + locationType + "</h6>");
                                sb.Append("</div>");
                                sb.Append("<div style='margin-bottom: 5px; font-weight: bold; color: #333;'>" + contactPerson.Trim() + "</div>");
                                sb.Append("<div style='margin-bottom: 3px; color: #666;'>Mobile: " + contactMobile + "</div>");
                                sb.Append("<div style='margin-bottom: 8px; color: #666;'>Email: " + contactEmail + "</div>");
                                sb.Append("<div style='margin-bottom: 3px; font-weight: bold; color: #333;'>Address:</div>");
                                sb.Append("<div style='color: #555; line-height: 1.4;'>" + fullAddress + "</div>");

                                sb.Append("</td>");
                            }

                            // Fill empty cells if less than 3 contacts in the row
                            int remainingCells = 3 - Math.Min(3, dtContacts.Rows.Count - i);
                            for (int empty = 0; empty < remainingCells; empty++)
                            {
                                sb.Append("<td style='width:33.33%; border: 1px solid #ddd;'>&nbsp;</td>");
                            }

                            sb.Append("</tr>");
                        }

                        sb.Append("</table>");
                    }
                    else
                    {
                        sb.Append("<div style='padding: 20px; text-align: center; color: #666;'>No contact details available</div>");
                    }

                    // Bank Details Section
                    dt3.Columns.AddRange(new DataColumn[6] {
        new DataColumn("AccountName", typeof(string)),
        new DataColumn("BankName", typeof(string)),
        new DataColumn("BankBranch", typeof(string)),
        new DataColumn("AccountNumber", typeof(string)),
        new DataColumn("IFSCode", typeof(string)),
        new DataColumn("SwiftCode", typeof(string)),
    });

                    dt3 = _cm.GetBankDetails(VendorID);

                    sbBank.Append("<br/><br/>");
                    sbBank.Append("<table cellpadding='8' cellspacing='0' width='100%' style='font-size: 14px;font-family:Times New Roman; border-collapse: collapse; margin-top: 20px;'>");

                    // Bank Details Header
                    sbBank.Append("<tr>");
                    sbBank.Append("<td colspan='6' style='text-align: center; background-color: #f8f9fa; padding: 15px; border: 2px solid #2F4F4F;'>");
                    sbBank.Append("<h3 style='margin: 0; color: #b34788;'>Bank Details</h3>");
                    sbBank.Append("</td>");
                    sbBank.Append("</tr>");

                    // Column Headers
                    sbBank.Append("<tr style='background-color: #b34788; color: white;'>");
                    sbBank.Append("<th style='text-align:center; border: 1px solid #ddd; padding: 12px;'>Account Name</th>");
                    sbBank.Append("<th style='text-align:center; border: 1px solid #ddd; padding: 12px;'>Bank Name</th>");
                    sbBank.Append("<th style='text-align:center; border: 1px solid #ddd; padding: 12px;'>Bank Branch</th>");
                    sbBank.Append("<th style='text-align:center; border: 1px solid #ddd; padding: 12px;'>Account Number</th>");
                    sbBank.Append("<th style='text-align:center; border: 1px solid #ddd; padding: 12px;'>IFSC Code</th>");
                    sbBank.Append("<th style='text-align:center; border: 1px solid #ddd; padding: 12px;'>Swift Code</th>");
                    sbBank.Append("</tr>");

                    // Bank Data Rows
                    if (dt3.Rows.Count > 0)
                    {
                        for (int bankIndex = 0; bankIndex < dt3.Rows.Count; bankIndex++)
                        {
                            sbBank.Append("<tr style='background-color: " + (bankIndex % 2 == 0 ? "#ffffff" : "#f9f9f9") + ";'>");

                            string accountName = dt3.Rows[bankIndex]["AccountName"].ToString();
                            string bankName = dt3.Rows[bankIndex]["BankName"].ToString();
                            string bankBranch = dt3.Rows[bankIndex]["BankBranch"].ToString();
                            string accountNumber = dt3.Rows[bankIndex]["AccountNumber"].ToString();
                            string ifsCode = dt3.Rows[bankIndex]["IFSCode"].ToString();
                            string swiftCode = dt3.Rows[bankIndex]["SwiftCode"].ToString();

                            sbBank.Append("<td style='border: 1px solid #ddd; text-align:center; padding: 10px;'>" + accountName + "</td>");
                            sbBank.Append("<td style='border: 1px solid #ddd; text-align:center; padding: 10px;'>" + bankName + "</td>");
                            sbBank.Append("<td style='border: 1px solid #ddd; text-align:center; padding: 10px;'>" + bankBranch + "</td>");
                            sbBank.Append("<td style='border: 1px solid #ddd; text-align:center; padding: 10px;'>" + accountNumber + "</td>");
                            sbBank.Append("<td style='border: 1px solid #ddd; text-align:center; padding: 10px;'>" + ifsCode + "</td>");
                            sbBank.Append("<td style='border: 1px solid #ddd; text-align:center; padding: 10px;'>" + swiftCode + "</td>");
                            sbBank.Append("</tr>");
                        }
                    }
                    else
                    {
                        sbBank.Append("<tr>");
                        sbBank.Append("<td colspan='6' style='border: 1px solid #ddd; text-align:center; padding: 20px; color: #666;'>No bank details available</td>");
                        sbBank.Append("</tr>");
                    }

                    sbBank.Append("</table>");

                    dtDocument.Columns.AddRange(new DataColumn[1] {
                    new DataColumn("DocumentType", typeof(string))
                });

                    dtDocument = _cm.GetDocumentDetails(VendorID);

                    for (int i = 0; i < dtDocument.Rows.Count; i++)
                    {
                        DocumentType = dtDocument.Rows[i]["DocumentType"].ToString();
                        DocumentID = dtDocument.Rows[i]["VendorDocumentID"].ToString();
                        string baseUrl = "http://localhost:41414/"; //  default
                        DataTable dtOrg = _cm.GetOrganizationDetails();
                        if (dtOrg.Rows.Count > 0 && dtOrg.Columns.Contains("url"))
                        {
                            baseUrl = dtOrg.Rows[0]["url"].ToString();
                            if (!baseUrl.EndsWith("/"))
                                baseUrl += "/";
                        }
                        sbDocument.AppendFormat(string.Format("<a href='" + baseUrl + "ViewDocuments.aspx?ID={0}' target='_blank'>", DocumentID));
                        sbDocument.AppendFormat(DocumentType);
                        sbDocument.Append("</a>");
                        sbDocument.Append("<br/>");
                        sbDocument.AppendLine();
                    }

                    //for (int i = 0; i < dtDocument.Rows.Count; i++)
                    //{
                    //    DocumentType = dtDocument.Rows[i]["DocumentType"].ToString();
                    //    DocumentID = dtDocument.Rows[i]["VendorDocumentID"].ToString();

                    //    // Get the actual filename if available
                    //    string fileName = "";
                    //    if (dtDocument.Rows[i].Table.Columns.Contains("FileName") && dtDocument.Rows[i]["FileName"] != DBNull.Value)
                    //    {
                    //        fileName = dtDocument.Rows[i]["FileName"].ToString();
                    //    }

                    //    if (!string.IsNullOrEmpty(fileName))
                    //    {
                    //        // If filename is available, create link to actual file
                    //        sbDocument.AppendFormat("<a href='/VendorDocument/{0}' target='_blank'>{1}</a>", fileName, DocumentType);
                    //    }
                    //    else
                    //    {
                    //        // If no filename, just show DocumentType as plain text (no link)
                    //        sbDocument.Append(DocumentType);
                    //    }

                    //    sbDocument.Append("<br/>");
                    //    sbDocument.AppendLine();
                    //}

                    string attach = objEmail.PrepareMailBodyWith("VendorProfile.html", "{VendorName}", VendorName, "{Address}", AddressLine1, "{ContactPerson}", ContactPerson, "{Mobile}", ContactMobile, "{Website}", Website, "{Reference}", Reference, "{DealerName}", DealerName, "{TypeofBusiness}", TypeofBusiness, "{YearofEstablishment}", YearofEstablishment, "{GST}", VAT, "{Date}", CreatedDate, "{ApprovedCredit}", ApprovedCredit, "{PANNumber}", PanNumber, "{AdharNumber}", AdharNumber, "{YearlyTurnover1}", YearlyTurnover1, "{YearlyTurnover2}", YearlyTurnover2, "{YearlyTurnover3}", YearlyTurnover3, "{ContactDetails}", sb.ToString(), "{BankDetails}", sbBank.ToString(), "{Year1}", Year1, "{Year2}", Year2, "{Year3}", Year3, "{NumberofEmployees}", NumberofEmployees, "{RegistrationNumber}", RegistrationNumber, "{Documents}", sbDocument.ToString(), "{TypeOfEstablishment}", TypeOfEstablishment, "{PFNumber}", PFNumber, "{UANNumber}", UANNumber, "{ESICNumber}", ESICNumber);
                    string str = attach.ToString();
                    string Body = "Dear " + VendorName + ", Please find attached profile details submitted to PTQualis ." + str; ;


                    string filename = Server.MapPath("//VendorDetails/Vendor-" + Reference + ".htm");
                    System.IO.File.WriteAllText(@filename, str);


                    string html = File.ReadAllText(HttpContext.Current.Server.MapPath("//VendorDetails/Vendor-" + Reference + ".htm"));
                    divVendor.InnerHtml = html;
                    string attachment = filename;

                    if (Request.QueryString["s"] == null)
                    {
                        string Subject = "";
                        string FromAddress = "";
                        string ToAddress = "";
                        string BCCAddress = "";
                        string CCAddress = "";
                        string VendorType = "";
                        DataTable dtEmailDetails = new DataTable();
                        if (Reference.Contains("VEN"))
                        {
                            VendorType = "Vendor Profile";
                        }
                        else
                        {
                            VendorType = "Vendor Profile";
                        }
                        dtEmailDetails = _cm.getEmaildetails(VendorType);
                        if (dtEmailDetails.Rows.Count > 0)
                        {
                            if (dtEmailDetails.Rows[0]["EmailSubject"].ToString() != "")
                            {
                                Subject = "Vendor Profile for: " + _cm.CustomerCode + "  " + dtEmailDetails.Rows[0]["EmailSubject"].ToString();
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
                        }
                        ToAddress = ContactEmail;

                        _cm.FromAddress = FromAddress;
                        _cm.ToAddress = ToAddress;
                        _cm.CCAddress = "";
                        _cm.BCCAddress = "";
                        _cm.Subject = Subject;
                        _cm.Body = Body;
                        _cm.Purpose = "Vendor Details";
                        _cm.FileName = "VendorDetails.html";
                        _cm.SentTo = VendorName;

                        //try
                        //{
                        //    //objEmail.SendMail(FromAddress, ToAddress, Subject, Body, attachment);

                        //    if (Session["Failed"].ToString() == "0")
                        //    {
                        //        _cm.SentStatus1 = true;
                        //        _cm.AddMailBoxDetails(_cm);
                        //        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email Sent!');", true);
                        //    }
                        //    else
                        //    {
                        //        _cm.SentStatus1 = false;
                        //        _cm.AddMailBoxDetails(_cm);
                        //        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email Send Failed!');", true);
                        //    }
                        //}
                        //catch (Exception ex)
                        //{
                        //    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email Failed : " + ex + "');", true);
                        //}
                    }

                }
                else
                {
                    Session["VendorID"] = 0;
                }
            }

        }
    }
}