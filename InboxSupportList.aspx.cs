using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

namespace LIMS
{
    public partial class InboxSupportList : System.Web.UI.Page
    {
        DAL.Pop3 _cm = new DAL.Pop3();
        //public const string Host = "pop.secureserver.net";
        public const string Host = "pop.hemshell.net";
        public const int Port = 995;
        public string Email;
        public string Password;
        //public const int NoOfEmailsPerPage = 100;
        int NoOfEmailsPerPage = 100;
        public const string SelfLink = "<a href=\"InboxSupportList.aspx?page={0}\">{1}</a>";
        public const string DisplayEmailLink = "<a href=\"InboxSupportDisplay.aspx?emailId={0}\">{1}</a>";
        protected void Page_Load(object sender, EventArgs e)
        {
   //         HtmlControl liHome1 = (HtmlGenericControl)this.Master.FindControl("liHome1");
   //         HtmlControl liCustomers1 = (HtmlGenericControl)this.Master.FindControl("liCustomers1");
   //         HtmlControl liInquiries1 = (HtmlGenericControl)this.Master.FindControl("liInquiries1");
   //         HtmlControl liCRFList1 = (HtmlGenericControl)this.Master.FindControl("liCRFList1");
   //         HtmlControl liSampleManagement1 = (HtmlGenericControl)this.Master.FindControl("liSampleManagement1");
   //         HtmlControl liTestManagement1 = (HtmlGenericControl)this.Master.FindControl("liTestManagement1");
   //         HtmlControl liQualityManagement1 = (HtmlGenericControl)this.Master.FindControl("liQualityManagement1");
   //         HtmlControl liCustomerFeedback1 = (HtmlGenericControl)this.Master.FindControl("liCustomerFeedback1");
   //         HtmlControl liUserManagement1 = (HtmlGenericControl)this.Master.FindControl("liUserManagement1");
   //         HtmlControl liSupport1 = (HtmlGenericControl)this.Master.FindControl("liSupport1");
   //         HtmlControl liViewInquiries2 = (HtmlGenericControl)this.Master.FindControl("liViewInquiries2");
			//HtmlControl liInquiryStatus2 = (HtmlGenericControl)this.Master.FindControl("liInquiryStatus2");

            if (Session["Usertype"] != null)
            {
                if (Session["EmployeeName"] != null)
                {
                    Label lblUserName = (Label)Master.FindControl("lblUserName");
                    lblUserName.Text = Session["EmployeeName"].ToString();
                }
                if (Session["Email"] != null)
                {
                    Label lblUserEmail = (Label)Master.FindControl("lblUserEmail");
                    lblUserEmail.Text = Session["Email"].ToString();
                }
                if (Session["RoleID"] != null)
                {
                    int RoleID = Convert.ToInt32(Session["RoleID"].ToString());
                    //switch (RoleID)
                    //{
                    //    case 1://QC
                    //        liHome1.Visible = true;
                    //        liCustomers1.Visible = true;
                    //        liInquiries1.Visible = true;
                    //        liCRFList1.Visible = true;
                    //        liSampleManagement1.Visible = true;
                    //        liTestManagement1.Visible = false;
                    //        liQualityManagement1.Visible = true;
                    //        liCustomerFeedback1.Visible = false;
                    //        liUserManagement1.Visible = false;
                    //        liSupport1.Visible = false;
                    //        break;
                    //    case 2://Manager
                    //        liHome1.Visible = true;
                    //        liCustomers1.Visible = false;
                    //        liInquiries1.Visible = false;
                    //        liCRFList1.Visible = false;
                    //        liSampleManagement1.Visible = true;
                    //        liTestManagement1.Visible = true;
                    //        liQualityManagement1.Visible = true;
                    //        liCustomerFeedback1.Visible = false;
                    //        liUserManagement1.Visible = false;
                    //        liSupport1.Visible = false;
                    //        break;
                    //    case 3://Lab Technician
                    //        liHome1.Visible = true;
                    //        liCustomers1.Visible = false;
                    //        liInquiries1.Visible = false;
                    //        liCRFList1.Visible = false;
                    //        liSampleManagement1.Visible = false;
                    //        liTestManagement1.Visible = true;
                    //        liQualityManagement1.Visible = false;
                    //        liCustomerFeedback1.Visible = false;
                    //        liUserManagement1.Visible = false;
                    //        liSupport1.Visible = false;
                    //        break;
                    //    case 4://Admin
                    //        liHome1.Visible = true;
                    //        liCustomers1.Visible = true;
                    //        liInquiries1.Visible = true;
                    //        liCRFList1.Visible = true;
                    //        liSampleManagement1.Visible = true;
                    //        liTestManagement1.Visible = true;
                    //        liQualityManagement1.Visible = true;
                    //        liCustomerFeedback1.Visible = false;
                    //        liUserManagement1.Visible = false;
                    //        liSupport1.Visible = false;
                    //        break;
                    //    case 5://Sample Management
                    //        liHome1.Visible = true;
                    //        liCustomers1.Visible = true;
                    //        liInquiries1.Visible = true;
                    //        liCRFList1.Visible = true;
                    //        liSampleManagement1.Visible = true;
                    //        liTestManagement1.Visible = false;
                    //        liQualityManagement1.Visible = false;
                    //        liCustomerFeedback1.Visible = false;
                    //        liUserManagement1.Visible = false;
                    //        liSupport1.Visible = false;
                    //        break;
                    //    default:
                    //        break;
                    //}
                }
                if (!Page.IsPostBack)
                {
                    //liSupport1.Attributes.Add("class", "active");
                    int page = 1;
                    if (Request.QueryString["page"] == null)
                    {
                        Response.Redirect("InboxSupportList.aspx?page=1");
                        Response.Flush();
                        Response.End();
                    }
                    else
                        page = Convert.ToInt32(Request.QueryString["page"]);
                    try
                    {
                        // Session["email"] = "support@rnisolutions.in";
                        // Session["pwd"] = "jinitha123";
                        Session["email"] = "support@hemshell.net";
                        Session["pwd"] = "qN#Cwqu3";
                        Email = Session["email"].ToString();
                        Password = Session["pwd"].ToString();
                    }
                    catch (Exception ex) { Response.Redirect("Default.aspx"); }
                    int totalEmails;
                    List<DAL.Email> emails;
                    string emailAddress;
                    using (DAL.Pop3Client client = new DAL.Pop3Client(Host, Port, Email, Password, true))
                    {
                        emailAddress = client.Email;
                        client.Connect();
                        totalEmails = client.GetEmailCount();
                        emails = client.FetchEmailList(((page - 1) * NoOfEmailsPerPage) + 1, NoOfEmailsPerPage);
                    }
                    int totalPages;
                    int mod = totalEmails % NoOfEmailsPerPage;
                    if (mod == 0)
                        totalPages = totalEmails / NoOfEmailsPerPage;
                    else
                        totalPages = ((totalEmails - mod) / NoOfEmailsPerPage) + 1;
                    for (int i = 0; i < emails.Count; i++)
                    {
                        DAL.Email email = emails[i];
                        int emailId = ((page - 1) * NoOfEmailsPerPage) + i + 1;
                        TableCell noCell = new TableCell();
                        noCell.CssClass = "emails-table-cell";
                        noCell.Text = Convert.ToString(emailId);
                        TableCell fromCell = new TableCell();
                        fromCell.CssClass = "emails-table-cell";
                        fromCell.Text = email.From;
                        TableCell subjectCell = new TableCell();
                        subjectCell.CssClass = "emails-table-cell";
                        subjectCell.Style["width"] = "300px";
                        subjectCell.Text = String.Format(DisplayEmailLink, emailId, email.Subject);
                        TableCell dateCell = new TableCell();
                        dateCell.CssClass = "emails-table-cell";
                        if (email.UtcDateTime != DateTime.MinValue)
                            dateCell.Text = email.UtcDateTime.ToString();
                        TableRow emailRow = new TableRow();
                        emailRow.Cells.Add(noCell);
                        emailRow.Cells.Add(fromCell);
                        emailRow.Cells.Add(subjectCell);
                        emailRow.Cells.Add(dateCell);
                        EmailsTable.Rows.AddAt(2 + i, emailRow);
                    }
                    if (totalPages > 1)
                    {
                        if (page > 1)
                            PreviousPageLiteral.Text = String.Format(SelfLink, page - 1, "Previous Page");
                        if (page > 0 && page < totalPages)
                            NextPageLiteral.Text = String.Format(SelfLink, page + 1, "Next Page");
                    }
                    EmailFromLiteral.Text = Convert.ToString(((page - 1) * NoOfEmailsPerPage) + 1);
                    EmailToLiteral.Text = Convert.ToString(page * NoOfEmailsPerPage);
                    EmailTotalLiteral.Text = Convert.ToString(totalEmails);
                    EmailLiteral.Text = emailAddress;
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }

    }
}