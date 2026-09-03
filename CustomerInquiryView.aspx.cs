using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DocumentFormat.OpenXml.Bibliography;

namespace LIMS
{
    public partial class CustomerInquiryView : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int CustomerUID = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
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
                    }

                    if (Request.QueryString["s"] != null)
                    {
                        int InquiryStatus = Convert.ToInt32(Request.QueryString["s"].ToString());
                        BindInquiries(InquiryStatus);
                    }
                    else
                    {
                        BindInquiries();
                    }

                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }

        private void BindInquiries()
        {
            string Dept = "", Option = "";
            Dept = radOption.SelectedItem.ToString();
            if (Dept == "Lab")
            {
                Option = " not in(4011,4012,4013,4014)";
            }
            else
            {
                Option = " in(4011,4012,4013,4014)";
            }
            Option = Session["DepartmentID"].ToString();
            DataTable dt = _cm.getInquiryDetails(Dept, Option);
            if (dt.Rows.Count > 0)
            {
                gvInquiries.DataSource = dt;
                gvInquiries.DataBind();
                Session["MyGrid"] = dt;
                lblNoRecords.Visible = false;
            }
            else
            {
                gvInquiries.DataSource = null;
                gvInquiries.DataBind();
                lblNoRecords.Visible = true;
            }
        }

        private void BindInquiries(int status)
        {
            DataTable dt = _cm.getInquiryDetails(status);
            if (dt.Rows.Count > 0)
            {
                gvInquiries.DataSource = dt;
                gvInquiries.DataBind();
                Session["MyGrid"] = dt;
                lblNoRecords.Visible = false;
            }
            else
            {
                lblNoRecords.Visible = true;
            }
        }

        private void BindInquiries(string search)
        {
            DataTable dt = _cm.getInquiryDetails(search);
            if (dt.Rows.Count > 0)
            {
                gvInquiries.DataSource = dt;
                gvInquiries.DataBind();
                Session["MyGrid"] = dt;
                lblNoRecords.Visible = false;
            }
            else
            {
                lblNoRecords.Visible = true;
            }
        }

        protected void imgSearch_Click(object sender, ImageClickEventArgs e)
        {
            string search = txtSearch.Text;
            if (search == "")
            {
                txtSearch.BorderColor = System.Drawing.Color.Red;
            }
            else
            {
                txtSearch.BorderColor = System.Drawing.Color.Empty;
                BindInquiries(search);
            }
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string search = txtSearch.Text;
            BindInquiries(search);
        }

        protected void gvInquiries_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvInquiries.PageIndex = e.NewPageIndex;
            DataTable dt = Session["MyGrid"] as DataTable;

            if (dt != null)
            {
                gvInquiries.DataSource = dt;
                gvInquiries.DataBind();
            }
        }

        protected void gvInquiries_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int RoleID = 0;
                DropDownList ddlCRFList = (e.Row.FindControl("ddlCRFList") as DropDownList);
                Label lblStatusName = (e.Row.FindControl("lblStatusName") as Label);
                Button btnSendCRF = (e.Row.FindControl("btnSendCRF") as Button);
                Button btnCloneCRF = (e.Row.FindControl("btnCloneCRF") as Button);
                Button btnAcceptCRA = (e.Row.FindControl("btnAcceptCRA") as Button);
                //HyperLink hyp = (e.Row.FindControl("hyp") as HyperLink);
                HyperLink hypCRF = (e.Row.FindControl("hypCRF") as HyperLink);
                HyperLink hypCRA = (e.Row.FindControl("hypCRA") as HyperLink);
                HyperLink hypCRFClone = (e.Row.FindControl("hypCRFClone") as HyperLink);
                PlaceHolder PlaceHolderCRFCloneList = (e.Row.FindControl("PlaceHolderCRFCloneList") as PlaceHolder);
                HiddenField hdEnquiryDept = (e.Row.FindControl("hdEnquiryDept") as HiddenField);
                Button btnQuotationGenerate = (e.Row.FindControl("btnQuotationGenerate") as Button);
                HiddenField hdQuotationStatus = (e.Row.FindControl("hdQuotationStatus") as HiddenField);
                Button btnTAF = (e.Row.FindControl("btnTAF") as Button);
                btnTAF.Visible = false;

                DataTable dt = new DataTable();
                if (Session["RoleID"] != null)
                {
                    RoleID = Convert.ToInt32(Session["RoleID"].ToString());
                }
                dt = new DataTable();
                if (radOption.SelectedItem.ToString() == "Lab")
                {
                    dt = _cm.getCRFList();
                    if (dt.Rows.Count > 0)
                    {
                        ddlCRFList.DataSource = dt;
                        ddlCRFList.DataTextField = "ReportType";
                        ddlCRFList.DataValueField = "ReportFormatID";
                        ddlCRFList.DataBind();
                        ddlCRFList.Items.Insert(0, new ListItem("Select TAF"));
                    }
                }
                else
                {
                    dt = new DataTable();
                    dt.Columns.Add("ReportType");
                    dt.Columns.Add("ReportFormatID");
                    DataRow dr = null;

                    dr = dt.NewRow(); // have new row on each iteration
                    dr["ReportType"] = "Calibration";
                    dr["ReportFormatID"] = "2008";
                    dt.Rows.Add(dr);

                    ddlCRFList.DataSource = dt;
                    ddlCRFList.DataTextField = "ReportType";
                    ddlCRFList.DataValueField = "ReportFormatID";
                    ddlCRFList.DataBind();
                    ddlCRFList.Items.Insert(0, new ListItem("Select CRA"));
                }

                int InquiryId = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "InquiryID").ToString());
                DataTable dt2 = _cm.getInquiryDetails(InquiryId, 1);
                if (dt2.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt2.Rows)
                    {
                        CustomerUID = Convert.ToInt32(dr["CustomerUID"].ToString());
                    }
                }
                DataTable dtCRFCloneList = new DataTable();
                StringBuilder sbCRFCloneLinks = new StringBuilder();

                string[] substrings = { "4011", "4012", "4013", "4014" };//If Enquiry belongs to Calibration Division
                bool containsAny = false;

                containsAny = substrings.Any(s => hdEnquiryDept.Value.Contains(s));

                if (containsAny)
                {
                    hypCRA.Text = "- Customer CRA";
                    hypCRA.NavigateUrl = string.Format("~/CalibrationRequest/CalibrationRequest-" + InquiryId + ".pdf");
                    hypCRA.Target = "_blank";
                    hypCRA.Visible = true;
                    hypCRF.Visible = false;
                }
                else
                {
                    hypCRF.Text = "- Customer TAF";
                    hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryId + CustomerUID + ".htm");
                    hypCRF.Target = "_blank";
                    hypCRF.Visible = true;
                    hypCRA.Visible = false;
                }

                HyperLink hyp2 = (e.Row.FindControl("hyp") as HyperLink);
                //Button btnQuotationGenerate = (e.Row.FindControl("btnQuotationGenerate") as Button);

                RoleID = Convert.ToInt32(Session["RoleID"].ToString());


                switch (lblStatusName.Text)
                {
                    case "New":
                        if (RoleID == 6 || RoleID == 7)//Sales Employee or Sales Manager
                        {
                            ddlCRFList.Visible = true;
                            btnSendCRF.Visible = true;
                        }
                        btnCloneCRF.Visible = false;
                        hypCRF.Visible = false;
                        hypCRA.Visible = false;
                        break;
                    case "CRF Sent":
                        lblStatusName.Text = "TAF Sent";
                        containsAny = substrings.Any(s => hdEnquiryDept.Value.Contains(s));

                        if (containsAny)
                        {
                            btnTAF.Text = "CRA";
                            //hyp.Text = "CRA";
                            //hyp.NavigateUrl = string.Format("~/CustomerCal.aspx?ID=" + InquiryId);
                        }
                        else
                        {
                            btnTAF.Text = "TAF";
                            btnTAF.Visible = true;
                            //hyp.Text = "TAF";
                            //hyp.NavigateUrl = string.Format("~/CustomerFMN.aspx?ID=" + InquiryId);
                        }
                        hypCRA.Visible = false;
                        hypCRF.Visible = false;
                        ddlCRFList.Visible = false;
                        btnSendCRF.Visible = false;
                        btnCloneCRF.Visible = false;
                        break;
                    case "Allocated For Testing":
                        ddlCRFList.Visible = false;
                        btnSendCRF.Visible = false;
                        btnCloneCRF.Visible = false;
                        hypCRA.Visible = true;
                        break;
                    case "CRF Received":
                        lblStatusName.Text = "TAF Received";
                        ddlCRFList.Visible = false;
                        btnSendCRF.Visible = false;
                        btnCloneCRF.Visible = false;

                        dtCRFCloneList = _cm.getCRFcloneList(InquiryId.ToString());
                        if (dtCRFCloneList.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtCRFCloneList.Rows)
                            {
                                HyperLink hyp1 = new HyperLink();
                                hyp1.ID = "hyp" + dr["CloneCRFSampleID"].ToString();
                                // hyp1.NavigateUrl += string.Format("~/CustomerRequest/Request-" + dr["CloneCRFSampleID"].ToString() + InquiryId + CustomerUID + ".htm");
                                hyp1.NavigateUrl += String.Format("~/CreateRequestCloneFM.aspx?id1=" + InquiryId + "&id2=" + dr["CloneCRFSampleID"].ToString());
                                hyp1.Text = "TAF - " + dr["CloneCRFSampleID"].ToString();
                                hyp1.Target = "_new";
                                PlaceHolderCRFCloneList.Controls.Add(new LiteralControl("<br />"));
                                PlaceHolderCRFCloneList.Controls.Add(hyp1);
                            }
                        }
                        break;
                    case "CRF Finalized":
                        lblStatusName.Text = "TAF Finalized";
                        ddlCRFList.Visible = false;
                        btnSendCRF.Visible = false;
                        if (hdQuotationStatus.Value == "1")
                        {
                            btnCloneCRF.Visible = true;
                        }

                        hypCRF.Text = "- Customer TAF";
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryId + CustomerUID + ".htm");


                        // hypCRFClone.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryId + CustomerUID + ".htm");
                        break;
                    case "CRF Approved":
                        lblStatusName.Text = "TAF Approved";
                        ddlCRFList.Visible = false;
                        btnSendCRF.Visible = false;
                        if (hdQuotationStatus.Value == "1")
                        {
                            btnCloneCRF.Visible = true;
                        }
                        hypCRF.Text = "- Customer TAF";
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryId + CustomerUID + ".htm");

                        dtCRFCloneList = _cm.getCRFcloneList(InquiryId.ToString());
                        if (dtCRFCloneList.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtCRFCloneList.Rows)
                            {
                                HyperLink hyp1 = new HyperLink();
                                hyp1.ID = "hyp" + dr["CloneCRFSampleID"].ToString();
                                //hyp1.NavigateUrl += string.Format("~/CustomerRequest/Request-" + dr["CloneCRFSampleID"].ToString() + InquiryId + CustomerUID + ".htm");
                                hyp1.NavigateUrl += String.Format("~/CreateRequestCloneFM.aspx?id1=" + InquiryId + "&id2=" + dr["CloneCRFSampleID"].ToString());
                                hyp1.Text = "TAF - " + dr["CloneCRFSampleID"].ToString();
                                hyp1.Target = "_new";
                                PlaceHolderCRFCloneList.Controls.Add(new LiteralControl("<br />"));
                                PlaceHolderCRFCloneList.Controls.Add(hyp1);
                            }
                        }
                        if (RoleID == 6 || RoleID == 7)//Sales Employee or Sales Manager
                        {
                            btnTAF.Visible = false;
                            //hyp.Visible = false;
                            btnCloneCRF.Visible = false;
                            if (hdQuotationStatus.Value != "1")
                            {
                                btnQuotationGenerate.Visible = true;
                            }

                            hypCRF.Visible = true;
                            hypCRA.Visible = true;
                            PlaceHolderCRFCloneList.Visible = false;
                        }
                        else
                        {
                            btnCloneCRF.Visible = true;
                            btnQuotationGenerate.Visible = false;
                        }
                        break;
                    case "Sample Received":
                        ddlCRFList.Visible = false;
                        btnSendCRF.Visible = false;
                        btnCloneCRF.Visible = false;
                        hypCRF.Text = "- Customer TAF";
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryId + CustomerUID + ".htm");

                        break;
                    case "Sample Rejected By Sample Management":
                        ddlCRFList.Visible = false;
                        btnSendCRF.Visible = false;
                        btnCloneCRF.Visible = false;
                        hypCRF.Text = "- Customer TAF";
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryId + CustomerUID + ".htm");

                        break;
                    case "Sample Accepted By Sample Management":
                        ddlCRFList.Visible = false;
                        btnSendCRF.Visible = false;
                        btnCloneCRF.Visible = false;
                        hypCRF.Text = "- Customer TAF";
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryId + CustomerUID + ".htm");

                        break;
                    case "Sample Rejected By Lab":
                        ddlCRFList.Visible = false;
                        btnSendCRF.Visible = false;
                        btnCloneCRF.Visible = false;
                        hypCRF.Text = "- Customer TAF";
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryId + CustomerUID + ".htm");

                        break;
                    case "Sample Accepted By Lab":
                        ddlCRFList.Visible = false;
                        btnSendCRF.Visible = false;
                        btnCloneCRF.Visible = false;
                        hypCRF.Text = "- Customer TAF";
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryId + CustomerUID + ".htm");

                        break;
                    case "Test Result Submitted":
                        ddlCRFList.Visible = false;
                        btnSendCRF.Visible = false;
                        btnCloneCRF.Visible = false;
                        hypCRF.Text = "- Customer TAF";
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryId + CustomerUID + ".htm");

                        break;
                    case "Test Report Submitted for Review":
                        ddlCRFList.Visible = false;
                        btnSendCRF.Visible = false;
                        btnCloneCRF.Visible = false;
                        hypCRF.Text = "- Customer TAF";
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryId + CustomerUID + ".htm");

                        break;
                    case "Test Report Generated":
                        ddlCRFList.Visible = false;
                        btnSendCRF.Visible = false;
                        btnCloneCRF.Visible = false;
                        hypCRF.Text = "- Customer TAF";
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryId + CustomerUID + ".htm");

                        break;
                    case "Reviewed Accepted":
                        ddlCRFList.Visible = false;
                        btnSendCRF.Visible = false;
                        btnCloneCRF.Visible = false;
                        hypCRF.Text = "- Customer TAF";
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryId + CustomerUID + ".htm");

                        break;
                    case "Feedback Received":
                        ddlCRFList.Visible = false;
                        btnSendCRF.Visible = false;
                        btnCloneCRF.Visible = false;
                        hypCRF.Text = "- Customer TAF";
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryId + CustomerUID + ".htm");

                        break;
                    case "Completed":
                        ddlCRFList.Visible = false;
                        btnSendCRF.Visible = false;
                        btnCloneCRF.Visible = false;
                        hypCRF.Text = "- Customer TAF";
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryId + CustomerUID + ".htm");
                        break;

                    // Calibration Status
                    //*************************
                    case "New Inquiry":
                        ddlCRFList.Visible = true;
                        btnSendCRF.Visible = true;
                        btnSendCRF.Text = "Send CRA";
                        btnCloneCRF.Visible = false;
                        hypCRF.Visible = false;
                        hypCRA.Visible = false;
                        break;

                    case "CRA Sent":
                        containsAny = substrings.Any(s => hdEnquiryDept.Value.Contains(s));

                        if (containsAny)
                        {
                            btnTAF.Text = "CRA";
                            hypCRA.Text = "CRA";
                            hypCRA.NavigateUrl = string.Format("~/CustomerCal.aspx?ID=" + InquiryId);
                            hypCRA.Visible = true;
                        }
                        else
                        {
                            btnTAF.Text = "TAF";
                            hypCRA.Text = "TAF";
                            hypCRA.NavigateUrl = string.Format("~/CustomerFMN.aspx?ID=" + InquiryId);
                        }
                        ddlCRFList.Visible = false;
                        btnSendCRF.Visible = false;
                        hypCRF.Visible = false;
                        btnCloneCRF.Visible = false;
                        break;
                    case "CRA Received":
                        ddlCRFList.Visible = false;
                        btnSendCRF.Visible = false;
                        btnCloneCRF.Visible = false;
                        btnAcceptCRA.Visible = true;

                        dtCRFCloneList = _cm.getCRFcloneList(InquiryId.ToString());
                        if (dtCRFCloneList.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtCRFCloneList.Rows)
                            {
                                HyperLink hyp1 = new HyperLink();
                                hyp1.ID = "hyp" + dr["CloneCRFSampleID"].ToString();
                                hyp1.NavigateUrl += string.Format("~/CalibrationRequest/CalibrationRequestRequest-" + InquiryId + ".pdf");
                                hyp1.Text = "CRA";
                                hyp1.Target = "_new";
                                PlaceHolderCRFCloneList.Controls.Add(new LiteralControl("<br />"));
                                PlaceHolderCRFCloneList.Controls.Add(hyp1);
                            }
                        }
                        break;
                    case "CRA Accepted by Sales":
                        ddlCRFList.Visible = false;
                        btnSendCRF.Visible = false;
                        btnCloneCRF.Visible = false;
                        if (RoleID == 6 || RoleID == 7)
                        {
                            btnTAF.Visible = false;
                            //hyp.Visible = false;
                            btnCloneCRF.Visible = false;
                            if (hdQuotationStatus.Value != "1")
                            {
                                btnQuotationGenerate.Visible = true;
                            }

                            hypCRF.Visible = false;
                            hypCRA.Visible = true;
                            PlaceHolderCRFCloneList.Visible = false;
                        }
                        else
                        {
                            btnQuotationGenerate.Visible = false;
                        }
                        break;
                    default:
                        ddlCRFList.Visible = true;
                        btnSendCRF.Visible = true;
                        btnCloneCRF.Visible = false;
                        hypCRF.Text = "- Customer TAF";
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryId + CustomerUID + ".htm");

                        break;
                }

            }
        }

        protected void gvInquiries_Sorting(object sender, GridViewSortEventArgs e)
        {
            string Dept = "", Option = "";
            Dept = radOption.SelectedItem.ToString();
            if (Dept == "Lab")
            {
                Option = " not in(4011,4012,4013,4014)";
            }
            else
            {
                Option = " in(4011,4012,4013,4014)";
            }
            DataTable dt = new DataTable();
            dt = _cm.getInquiryDetails(Dept, Option);

            string sortExpression = e.SortExpression;   // ValueA or ValueB
            string sortDirection = GetSortDirection(sortExpression);

            dt.DefaultView.Sort = $"{sortExpression} {sortDirection}";

            gvInquiries.DataSource = dt;
            gvInquiries.DataBind();
            Session["MyGrid"] = dt;
        }
        private string GetSortDirection(string column)
        {
            string direction = "ASC";

            if (ViewState["SortExpression"] != null &&
                ViewState["SortExpression"].ToString() == column &&
                ViewState["SortDirection"] != null)
            {
                direction = ViewState["SortDirection"].ToString() == "ASC"
                    ? "DESC"
                    : "ASC";
            }

            ViewState["SortExpression"] = column;
            ViewState["SortDirection"] = direction;

            return direction;
        }


        protected void gvInquiries_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            if (!e.CommandName.Equals("SendCRF") &&
                !e.CommandName.Equals("CloneCRF") &&
                !e.CommandName.Equals("AcceptCRA") &&
                !e.CommandName.Equals("QuotationGenerate") &&
                !e.CommandName.Equals("TAF"))
            {
                return;
            }
            else
            {
                DAL.emailCommunication eml = new DAL.emailCommunication();
                string CRFLink = "";
                int rowIndex = int.Parse(e.CommandArgument.ToString());
                int inquiryID = (int)this.gvInquiries.DataKeys[rowIndex]["InquiryID"];

                Label lblReportFormat = (Label)gvInquiries.Rows[rowIndex].Cells[0].FindControl("lblReportFormat");
                HiddenField hdEnquiryDept = (HiddenField)gvInquiries.Rows[rowIndex].Cells[0].FindControl("hdEnquiryDept");


                if (e.CommandName == "SendCRF")
                {
                    DropDownList ddlCRFList = (DropDownList)gvInquiries.Rows[rowIndex].Cells[0].FindControl("ddlCRFList");
                    if (ddlCRFList.SelectedIndex != 0)
                    {
                        int CRFID = Convert.ToInt32(ddlCRFList.SelectedValue);
                        if (radOption.SelectedItem.ToString() == "Lab")
                        {
                            switch (CRFID)
                            {
                                case 1:
                                    CRFLink = "CustomerFMN.aspx?ID=" + inquiryID + "&r=" + lblReportFormat.Text;
                                    //CRFLink = "CustomerFM.aspx?ID=" + inquiryID;
                                    break;
                                case 2:
                                    CRFLink = "CustomerMocon.aspx?ID=" + inquiryID;
                                    // CRFLink = "CustomerMocon.aspx?ID=" + inquiryID;
                                    break;
                                case 3:
                                    CRFLink = "CustomerPTI.aspx?ID=" + inquiryID + "&r=" + lblReportFormat.Text;
                                    //CRFLink = "CustomerPTI.aspx?ID=" + inquiryID;
                                    break;
                                default:
                                    ddlCRFList.BackColor = System.Drawing.Color.LightPink;
                                    break;
                            }
                            //eml.CRFSent(inquiryID, CRFLink);

                            //change inquiry status to 'CRF Sent'
                            _cm.inquiryId = inquiryID;
                            _cm.inquiryStatusId = 5;
                            _cm.UpdateInquiryStatus(_cm);
                            _cm.ReportFormat = Convert.ToInt32(ddlCRFList.SelectedValue);
                            _cm.UpdateReportFormat(_cm);
                        }
                        if (radOption.SelectedItem.ToString() == "Certification")
                        {
                            CRFLink = "CustomerCal.aspx?ID=" + inquiryID + "&r=" + lblReportFormat.Text;
                            //change inquiry status to 'CRF Sent'
                            _cm.inquiryId = inquiryID;
                            _cm.inquiryStatusId = 2;//CRA Sent
                            _cm.UpdateInquiryStatus(_cm);
                            _cm.ReportFormat = Convert.ToInt32(ddlCRFList.SelectedValue);
                            _cm.UpdateReportFormat(_cm);
                        }

                        BindInquiries();
                    }

                }

                if (e.CommandName == "CloneCRF")
                {
                    int CRFID = 0;
                    DataTable dtInquiryStatus = _cm.GetEnquiryStatus(inquiryID);

                    if (dtInquiryStatus.Rows.Count > 0)
                    {
                        int DepartmentID = Convert.ToInt32(dtInquiryStatus.Rows[0]["DepartmentID"].ToString());

                        if (DepartmentID == 1)
                        {
                            string url = "CloneCustomerFM_Transportation.aspx?ID=" + inquiryID + "&r=" + lblReportFormat.Text;
                            Response.Redirect(url);
                        }
                        if (DepartmentID == 4)
                        {
                            string url = "CloneCustomerFM_Chemical.aspx?ID=" + inquiryID + "&r=" + lblReportFormat.Text;
                            Response.Redirect(url);
                        }
                        else if (DepartmentID == 5)
                        {
                            string url = "CloneCustomerFM.aspx?ID=" + inquiryID + "&r=" + lblReportFormat.Text;
                            Response.Redirect(url);
                        }
                        else if (DepartmentID == 3006)
                        {
                            string url = "CloneCustomerFM_ElectricalAccessories.aspx?ID=" + inquiryID + "&r=" + lblReportFormat.Text;
                            Response.Redirect(url);
                        }
                        else if (DepartmentID == 4006)
                        {
                            string url = "CloneCustomerFM_PV.aspx?ID=" + inquiryID + "&r=" + lblReportFormat.Text;
                            Response.Redirect(url);
                        }
                        else if (DepartmentID == 4008)
                        {
                            string url = "CloneCustomerFM_HomeAppliances.aspx?ID=" + inquiryID + "&r=" + lblReportFormat.Text;
                            Response.Redirect(url);
                        }
                        else if (DepartmentID == 4009)
                        {
                            string url = "CloneCustomerFM_LED.aspx?ID=" + inquiryID + "&r=" + lblReportFormat.Text;
                            Response.Redirect(url);
                        }
                        else if (DepartmentID == 1)
                        {
                            string url = "CloneCustomerFM_Transportation.aspx?ID=" + inquiryID + "&r=" + lblReportFormat.Text;
                            Response.Redirect(url);
                        }
                        else if (DepartmentID == 3)
                        {
                            string url = "CloneCustomerFM_ToyAndBabyWear.aspx?ID=" + inquiryID + "&r=" + lblReportFormat.Text;
                            Response.Redirect(url);
                        }
                        else if (DepartmentID == 5020)
                        {
                            string url = "CloneCustomerFM_PhysicalSoftline.aspx?ID=" + inquiryID + "&r=" + lblReportFormat.Text;
                            Response.Redirect(url);
                        }
                        else if (DepartmentID == 2)
                        {
                            string url = "CloneCustomerFM_Battery.aspx?ID=" + inquiryID + "&r=" + lblReportFormat.Text;
                            Response.Redirect(url);
                        }
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Record Not Found for DepartmentId : , \\'" + DepartmentID + "Error! Hyperlink reference not valid.");
                        }
                    }
                    //change CloneCRF status to 'CRF Finalized'
                    if (Session["CloneCRFSampleID"] != null)
                    {
                        int CloneCRFSampleID = Convert.ToInt32(Session["CloneCRFSampleID"].ToString());
                        _cm.CRFStatusID = 28;
                        _cm.CloneCRFSampleID = CloneCRFSampleID;
                        _cm.UpdateCRFStatus(_cm);
                    }

                    BindInquiries();
                }

                if (e.CommandName == "AcceptCRA")
                {
                    //change inquiry status to 'CRA Accepted by Sales'
                    _cm.inquiryId = inquiryID;
                    _cm.inquiryStatusId = 32;//CRA Accepted by Sales
                    _cm.UpdateInquiryStatus(_cm);
                }
                if (e.CommandName == "QuotationGenerate")
                {
                    int rowIndex1 = Convert.ToInt32(e.CommandArgument);
                    GridViewRow row = gvInquiries.Rows[rowIndex1];
                    DataTable dtCustomerInquiry = _cm.GetEnquiryStatus(inquiryID);

                    if (dtCustomerInquiry.Rows.Count > 0)
                    {
                        int inquiryId = Convert.ToInt32(dtCustomerInquiry.Rows[0]["InquiryId"].ToString());
                        int inquiryStatusId = Convert.ToInt32(dtCustomerInquiry.Rows[0]["InquiryStatusId"].ToString());
                        if (inquiryStatusId == 5 || inquiryStatusId == 6 || inquiryStatusId == 21 || inquiryStatusId == 14)
                        {
                            //Response.Redirect("Quotation.aspx?CustomerMasterID=" + customerMasterID+";");//
                            var customerMasterID = gvInquiries.DataKeys[rowIndex]["CustomerMasterID"];
                            string url = $"Quotation.aspx?CustomerMasterID={customerMasterID}&id={inquiryId}";
                            Response.Redirect(url, true);
                        }
                        else
                            if (inquiryStatusId == 32)//CRA Accepted by Sales
                            {
                                //Response.Redirect("Quotation.aspx?CustomerMasterID=" + customerMasterID+";");//
                                var customerMasterID = gvInquiries.DataKeys[rowIndex]["CustomerMasterID"];
                                string url = $"QuotationCalib.aspx?CustomerMasterID={customerMasterID}&id={inquiryId}";
                                Response.Redirect(url, true);
                            }
                            else
                            {
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No Records Found.'); setTimeout(function() { window.location = 'CustomerInquiryView.aspx'; }, 20);</script>");
                                //Response.Redirect(Request.Url.ToString(), true);
                            }
                    }
                }
                if (e.CommandName == "TAF")
                {
                    string[] substrings = { "4011", "4012", "4013", "4014" };//If Enquiry belongs to Calibration Division
                    bool containsAny = false;
                    string url = "";
                    containsAny = substrings.Any(s => hdEnquiryDept.Value.Contains(s));

                    if (containsAny)
                    {
                        url = "CustomerCal.aspx?ID=" + inquiryID;
                    }
                    else
                    {
                        url = "CustomerFMN.aspx?ID=" + inquiryID;
                    }

                    Response.Write("<script> window.open( '" + url + "','_blank' ); </script>");
                }
                BindInquiries();
            }
        }

        protected string Limit(object desc, int maxLength)
        {
            var description = (string)desc;
            if (string.IsNullOrEmpty(description)) { return description; }
            return description.Length <= maxLength ?
                description : description.Substring(0, maxLength) + "...";
        }

        protected bool SetVisibility(object desc, int maxLength)
        {
            var description = (string)desc;
            if (string.IsNullOrEmpty(description)) { return false; }
            return description.Length > maxLength;
        }

        protected void ReadMoreLinkButton_Click(object sender, EventArgs e)
        {
            LinkButton button = (LinkButton)sender;
            GridViewRow row = button.NamingContainer as GridViewRow;
            Label descLabel = row.FindControl("lblInquiryDetail") as Label;
            button.Text = (button.Text == "Read More") ? "Hide" : "Read More";
            string temp = descLabel.Text;
            descLabel.Text = descLabel.ToolTip;
            descLabel.ToolTip = temp;
        }

        protected void radOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindInquiries();
        }
    }
}