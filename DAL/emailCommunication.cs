using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;
using System.Net.Mail;

namespace LIMS.DAL
{
    public class emailCommunication
    {
        SqlConnection _sCon = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString);

        string sQuery = "";
        DAL.CM _cm = new DAL.CM();
        DataTable dt = new DataTable();

        string _title;
        string _firstName;
        string _lastName;
        string _email;
        string _To;
        string _CC;
        string _BCC;
        string _Subject;
        string _Message;
        string _attachmentPath;
        public string title
        {
            get { return _title; }
            set { _title = value; }
        }
        public string firstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }
        public string lastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }
        public string email
        {
            get { return _email; }
            set { _email = value; }
        }
        public string To { get { return _To; } set { _To = value; } }
        public string CC { get { return _CC; } set { _CC = value; } }
        public string BCC { get { return _BCC; } set { _BCC = value; } }
        public string Subject { get { return _Subject; } set { _Subject = value; } }
        public string Message { get { return _Message; } set { _Message = value; } }
        public string attachmentPath { get { return _attachmentPath; } set { _attachmentPath = value; } }
        public void EnquiryReceived(int enquiryID)
        {
            LIMS.DAL.mailer _mailer = new LIMS.DAL.mailer();
            //get Inquiry Details
            string Subject = "";
            sQuery = "Select * from customerInquiry where InquiryId=" + enquiryID;
            dt = _cm.GetDataUsingQuery(sQuery);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {

                }
                email = "jinitha.s@rnisolutions.in";
                Subject = "Enquiry Received - Enquiry# " + enquiryID.ToString();

                string body = string.Empty;
                using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/EmailEnquiryReceived.html")))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{EnquiryID}", enquiryID.ToString());
                body = body.Replace("EnquiryLink", "~/EnquiryStatus.aspx?id=" + enquiryID.ToString());
                _mailer.SendMail("support@rnisolutions.in", "support@rnisolutions.in", Subject, body);
                //_mailer.SendMail("support@hemshellservices.com", "support@hemshellservices.com", Subject, body);
            }
        }
        public void CRFSent(int enquiryID, string CRFLink)
        {
            LIMS.DAL.mailer _mailer = new LIMS.DAL.mailer();
            //get Inquiry Details
            string Subject = ""; string customerEmail = ""; string ErrorMessage = "";
            sQuery = @"select co.email from customerInquiry ci 
                            inner join CustomerDetails cd on ci.CustomerId = cd.CustomerId
                            inner join ContactPersonDetails cp on cd.CustomerMasterId = cp.CustomerMasterId
                            inner join ContactDetails co on cp.customermasterid = co.customermasterid where ci.InquiryId = " + enquiryID +
                      @" union
                            select co.email from customerInquiry ci 
                            inner join CompanyDetails cd on ci.CustomerId = cd.companyid
                            inner join ContactPersonDetails cp on cd.CustomerMasterId = cp.CustomerMasterId
                            inner join ContactDetails co on cp.customermasterid = co.customermasterid where ci.InquiryId = " + enquiryID;
            dt = _cm.GetDataUsingQuery(sQuery);
            if (dt.Rows.Count > 0)
            {
                    email = dt.Rows[0]["Email"].ToString() + "; jinitha.s@rni-solutions.com";
                    Subject = "CRF Sent - Enquiry# " + enquiryID.ToString();

                    string body = string.Empty;
                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/EmailCRFSent.html")))
                    {
                        body = reader.ReadToEnd();
                    }
                    body = body.Replace("{EnquiryID}", enquiryID.ToString());
                    body = body.Replace("{CustomerEmail}", customerEmail);
                    body = body.Replace("{CRFLink}", CRFLink);
                    _mailer.SendMail("support@rni-solutions.com", email, Subject, body);
                
            }
            else
            {
                ErrorMessage = "No Customer Email found. Kindly check customer details.";
                _cm.ErrorHandler(1, ErrorMessage);
            }
        }
        public void FeedbackFormSent(int enquiryID, string Link)
        {
            LIMS.DAL.mailer _mailer = new LIMS.DAL.mailer();
            //get Inquiry Details
            string Subject = ""; string customerEmail = ""; string ErrorMessage = "";
            sQuery = @"select co.email from customerInquiry ci 
                            inner join CustomerDetails cd on ci.CustomerId = cd.CustomerId
                            inner join ContactPersonDetails cp on cd.CustomerMasterId = cp.CustomerMasterId
                            inner join ContactDetails co on cp.customermasterid = co.customermasterid where ci.InquiryId = " + enquiryID +
                    @"  union
                            select co.email from customerInquiry ci 
                            inner join CompanyDetails cd on ci.CustomerId = cd.companyid
                            inner join ContactPersonDetails cp on cd.CustomerMasterId = cp.CustomerMasterId
                            inner join ContactDetails co on cp.customermasterid = co.customermasterid where ci.InquiryId = " + enquiryID;
            dt = _cm.GetDataUsingQuery(sQuery);
            if (dt.Rows.Count > 0)
            {
                email = "jinitha.s@rnisolutions.in";
                Subject = "Customer Feedback Form Sent - Enquiry# " + enquiryID.ToString();

                string body = string.Empty;
                using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/EmailCustomerFeedbackSent.html")))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{EnquiryID}", enquiryID.ToString());
                body = body.Replace("{CustomerEmail}", customerEmail);
                body = body.Replace("{FeedbackLink}", Link);
                _mailer.SendMail("support@rnisolutions.in", "support@rnisolutions.in", Subject, body);               
            }
            else
            {
                ErrorMessage = "No Customer Email found. Kindly check customer details.";
                _cm.ErrorHandler(1, ErrorMessage);
            }
        }
        public void ReportSent(emailCommunication eml)
        {
            string ErrorMessage = "";
            LIMS.DAL.mailer _mailer = new LIMS.DAL.mailer();
            _mailer.SendMail("support@rni-solutions.com", eml.To, eml.Subject, eml.Message, eml.attachmentPath);            
        }
    }
}