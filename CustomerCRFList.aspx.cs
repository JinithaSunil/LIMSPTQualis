using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DocumentFormat.OpenXml.Bibliography;
using OpenXmlPowerTools;

namespace LIMS
{
    public partial class CustomerCRFList : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int CustomerUID = 0;
        string InquiryStatus = "";
        protected void Page_Load(object sender, EventArgs e)
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
                if (!Page.IsPostBack)
                {
                    //liCRFList1.Attributes.Add("class", "active");
                    BindCRFList();
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }

        }
        private void BindCRFList()
        {
            int RoleID = 0; string type = "";
            //InquiryStatus = "'2','3','5','6','13','16','21','28'";
            InquiryStatus = " (c.InquiryStatusID in('6','13') or cr.CRFStatusID in('13','14')) ";
            if (Request.QueryString["t"] != null)
            {
                type = Request.QueryString["t"].ToString();
                switch (type)
                {
                    case "r"://CRF Received
                        InquiryStatus = " c.InquiryStatusID in('6')";
                        break;
                    case "c"://CRF Finalized
                        InquiryStatus = " (c.InquiryStatusID in('21') and CRFStatusID in('28')) ";
                        break;
                    case "s"://Sample Received
                        InquiryStatus = " cr.CRFStatusID in('13')";
                        break;
                    case "a"://Sample Accepted By Sample Management
                        InquiryStatus = " cr.CRFStatusID in('14')";
                        break;
                    default:
                        break;

                }
            }
            string DepartmentID = Session["DepartmentID"].ToString();
            
            DataTable dt = _cm.getCRFByStatus(InquiryStatus, DepartmentID);//get CRF Received  List
            //int departmentIdGet = Convert.ToInt32(DepartmentID);
           
            dt = _cm.getCRFByStatus(InquiryStatus, DepartmentID);
            if (dt.Rows.Count > 0)
            {
                gvCRFList.DataSource = dt;
                gvCRFList.DataBind();
                lblNoRecords.Visible = false;
            }
            else
            {
                lblNoRecords.Visible = true;
            }
        }
        private void BindCRFList(string search)
        {
            int RoleID = 0; string type = "";
            //InquiryStatus = "'2','3','5','6','13','16','21','28'";
            InquiryStatus = " (c.InquiryStatusID in('6','13') or cr.CRFStatusID in('13','14')) ";
            if (Request.QueryString["t"] != null)
            {
                type = Request.QueryString["t"].ToString();
                switch (type)
                {
                    case "r"://CRF Received
                        InquiryStatus = " c.InquiryStatusID in('6')";
                        break;
                    case "c"://CRF Finalized
                        InquiryStatus = " c.InquiryStatusID in('21')";
                        break;
                    case "s"://Sample Received
                        InquiryStatus = " cr.CRFStatusID in('13')";
                        break;
                    case "a"://Sample Accepted By Sample Management
                        InquiryStatus = " cr.CRFStatusID in('14')";
                        break;
                    default:
                        break;

                }
            }
            string DepartmentID = Session["DepartmentID"].ToString();
            int departmentIdGet = Convert.ToInt32(DepartmentID);
            DataTable dt;
            if (departmentIdGet == 3)
            {
                dt = _cm.getCRFByStatusWithoutDepartment(InquiryStatus);
            }
            else
            {
                dt = _cm.getCRFByStatus(InquiryStatus, DepartmentID);
            }

            if (dt.Rows.Count > 0)
            {                
                gvCRFList.DataSource = dt;
                gvCRFList.DataBind();
                lblNoRecords.Visible = false;
            }
            else
            {
                lblNoRecords.Visible = true;
            }
            spanCount.InnerHtml = dt.Rows.Count.ToString();
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
                BindCRFList(search);
            }
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string search = txtSearch.Text;
            BindCRFList(search);
        }

        protected void gvCRFList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCRFList.PageIndex = e.NewPageIndex;
            this.BindCRFList();
        }

        protected void gvCRFList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int RoleID = 0;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                HyperLink hypCRF = (e.Row.FindControl("hypCRF") as HyperLink);
                HyperLink hypCloneCRF = (e.Row.FindControl("hypCloneCRF") as HyperLink);
                HyperLink hypWO = (e.Row.FindControl("hypWO") as HyperLink);
                Label lblWO = (e.Row.FindControl("lblWO") as Label);
                Label lblSampleID = (e.Row.FindControl("lblSampleID") as Label);
                int InquiryID = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "InquiryID").ToString());
                int CloneCRFSampleID = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "CloneCRFSampleID").ToString());
                DataTable dt2 = _cm.getInquiryDetails(InquiryID, 1);
                if (dt2.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt2.Rows)
                    {
                        CustomerUID = Convert.ToInt32(dr["CustomerUID"].ToString());
                    }
                }
                int CRFID = 0;

                Label lblStatusName = (e.Row.FindControl("lblStatusName") as Label);
                Label lblCRFStatus = (e.Row.FindControl("lblCRFStatus") as Label);
                Button btnApproveCRF = (e.Row.FindControl("btnApproveCRF") as Button);
                if (lblSampleID.Text!="")
                {
                    hypWO.Visible = true;
                    string url = "GenerateWorkOrder.aspx?i=" + InquiryID.ToString() +"&s=" + lblSampleID.Text;
                    hypWO.NavigateUrl = string.Format(url);
                }
                //Button btnMoreInfoCRF = (e.Row.FindControl("btnMoreInfoCRF") as Button);
                if (Session["RoleID"] != null)
                {
                    RoleID = Convert.ToInt32(Session["RoleID"].ToString());
                }
                Button btnSampleReceivedOn = (e.Row.FindControl("btnSampleReceivedOn") as Button);
                Button btnSampleReceivedOff = (e.Row.FindControl("btnSampleReceivedOff") as Button);
                Button btnSampleManagement = (e.Row.FindControl("btnSampleManagement") as Button);

                DataTable dtCRFCloneList = new DataTable();
                StringBuilder sbCRFCloneLinks = new StringBuilder();
                PlaceHolder PlaceHolderCRFCloneList = (e.Row.FindControl("PlaceHolderCRFCloneList") as PlaceHolder);

                if (CloneCRFSampleID == 0)
                {
                    lblStatusName.Visible = true;
                    lblCRFStatus.Visible = false;
                    hypCloneCRF.Visible = false;
                }
                else
                {
                    lblStatusName.Visible = false;
                    lblCRFStatus.Visible = true;
                }
               
                switch (lblCRFStatus.Text)
                {
                    case "CRF Sent":
                        lblCRFStatus.Text = "TAF Sent";
                        break;
                    case "CRF Received":
                        lblCRFStatus.Text = "TAF Received";
                        btnApproveCRF.Visible = true;
                        if (RoleID == 5) { btnApproveCRF.Visible = false; }
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        hypCloneCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryID + CustomerUID + ".htm");
                        hypCloneCRF.Visible = false;
                        break;
                    case "CRF Finalized":
                        lblCRFStatus.Text = "TAF Finalized";
                        btnApproveCRF.Visible = false;
                        //btnMoreInfoCRF.Visible = true;
                        if (RoleID == 2)
                        {
                            btnSampleReceivedOn.Visible = false;
                            btnSampleReceivedOff.Visible = false;
                        }
                        //btnMoreInfoCRF.Visible = false;
                        sampleReceivedStatus(InquiryID, e);
                        if (RoleID == 5)
                        {
                            btnSampleReceivedOn.Visible = true;
                            btnSampleReceivedOff.Visible = true;
                        }
                        DataTable dt1 = _cm.getCRFDetails(InquiryID);

                        if (dt1.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt1.Rows)
                            {
                                CRFID = Convert.ToInt32(dr["CustomerRequestId"].ToString());
                            }
                        }
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        hypCloneCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryID + CustomerUID + ".htm");
                        hypCloneCRF.Visible = true;
                        break;
                    case "CRF Approved":
                    case "TAF Approved":
                        lblCRFStatus.Text = "TAF Approved";
                        btnApproveCRF.Visible = false;
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        hypCloneCRF.Visible = false;
                        break;
                    case "Sample Received":
                        btnApproveCRF.Visible = false;
                        //btnMoreInfoCRF.Visible = false;
                        btnSampleReceivedOn.Visible = false;
                        btnSampleReceivedOff.Visible = false;
                        btnSampleManagement.Visible = true;
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        hypCloneCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryID + CustomerUID + ".htm");
                        hypCloneCRF.Visible = true;
                        break;
                    case "Sample Accepted By Sample Management":
                        btnApproveCRF.Visible = false;
                        //btnMoreInfoCRF.Visible = false;
                        btnSampleReceivedOn.Visible = false;
                        btnSampleReceivedOff.Visible = false;
                        btnSampleManagement.Visible = false;
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        hypCloneCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryID + CustomerUID + ".htm");
                        hypCloneCRF.Visible = true;
                        break;
                    case "Sample Rejected By Sample Management":
                        btnApproveCRF.Visible = false;
                        //btnMoreInfoCRF.Visible = false;
                        btnSampleReceivedOn.Visible = true;
                        btnSampleReceivedOff.Visible = true;
                        btnSampleManagement.Visible = false;
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        hypCloneCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryID + CustomerUID + ".htm");
                        hypCloneCRF.Visible = true;
                        break;
                    case "Sample Accepted By Lab":
                        btnApproveCRF.Visible = false;
                        //btnMoreInfoCRF.Visible = false;
                        btnSampleReceivedOn.Visible = true;
                        btnSampleReceivedOff.Visible = true;
                        btnSampleManagement.Visible = false;
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        hypCloneCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryID + CustomerUID + ".htm");
                        hypCloneCRF.Visible = true;
                        break;
                    case "Sample Rejected By Lab":
                        btnApproveCRF.Visible = false;
                        //btnMoreInfoCRF.Visible = false;
                        btnSampleReceivedOn.Visible = true;
                        btnSampleReceivedOff.Visible = true;
                        btnSampleManagement.Visible = false;
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        hypCloneCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryID + CustomerUID + ".htm");
                        hypCloneCRF.Visible = true;
                        break;
                    default:
                        btnApproveCRF.Visible = false;
                        //btnMoreInfoCRF.Visible = false;
                        break;
                }
                switch (lblStatusName.Text)
                {
                    case "CRF Sent":
                        lblStatusName.Text = "TAF Sent";
                        break;
                    case "CRF Received":
                        lblStatusName.Text = "TAF Received";
                        btnApproveCRF.Visible = true;
                        if (RoleID == 5) { btnApproveCRF.Visible = false; }
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        
                        break;
                    case "CRF Finalized":
                        lblStatusName.Text = "TAF Finalized";
                        btnApproveCRF.Visible = false;
                        //btnMoreInfoCRF.Visible = true;
                        if (RoleID == 2)
                        {
                            btnSampleReceivedOn.Visible = false;
                            btnSampleReceivedOff.Visible = false;
                        }
                        
                        sampleReceivedStatus(InquiryID, e);
                        if (RoleID == 5)
                        {
                            btnSampleReceivedOn.Visible = true;
                            btnSampleReceivedOff.Visible = true;
                        }
                        DataTable dt1 = _cm.getCRFDetails(InquiryID);

                        if (dt1.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt1.Rows)
                            {
                                CRFID = Convert.ToInt32(dr["CustomerRequestId"].ToString());
                            }
                        }
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        hypCloneCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryID + CustomerUID + ".htm");
                        hypCloneCRF.Visible = true;
                        break;
                    case "CRF Approved":
                    case "TAF Approved":
                        lblStatusName.Text = "TAF Approved";
                        btnApproveCRF.Visible = false;
                        hypCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                        hypCRF.Visible = true;
                        hypCloneCRF.Visible = true;
                        break;
                    default:
                        btnApproveCRF.Visible = false;
                        //btnMoreInfoCRF.Visible = false;
                        break;
                }

            }
        }

        protected void gvCRFList_Sorting(object sender, GridViewSortEventArgs e)
        {
            InquiryStatus = "'2','3','5','6','13','14','16','21','28'";
            DataTable dt = new DataTable();
            dt = _cm.getCRFByInquiryStatus(InquiryStatus);
            dt.DefaultView.Sort = e.SortExpression;
            gvCRFList.DataSource = dt;
            gvCRFList.DataBind();
        }
        protected void sampleReceivedStatus(int InquiryID, GridViewRowEventArgs e)
        {
            Button btnOn = (e.Row.FindControl("btnSampleReceivedOn") as Button);
            Button btnOff = (e.Row.FindControl("btnSampleReceivedOff") as Button);

            //DataTable dt7 = _cm.GetCustomerInquiryDetails();

            //if (dt7.Rows.Count > 0)
            //{
            //    for (int i = 0; i < dt7.Rows.Count; i++)
            //    {
            //        _cm.inquiryStatusId = Convert.ToInt32(dt7.Rows[i]["InquiryStatusId"].ToString());
            //        _cm.inquiryId = Convert.ToInt32(dt7.Rows[i]["InquiryId"].ToString());

            //        if (_cm.inquiryStatusId == 21)
            //        {
            //            if (_cm.inquiryId != 0)
            //            {
            //                InquiryID = Convert.ToInt32(_cm.inquiryId);

            //                btnOn.Visible = true;
            //                btnOff.Visible = true;
            //                btnOn.CssClass = "btn btn-default";
            //                btnOff.CssClass = "btn btn-danger";
            //            }
            //        }
            //        if (_cm.inquiryStatusId == 13)
            //        {
            //            if (_cm.inquiryId != 0)
            //            {
            //                InquiryID = Convert.ToInt32(_cm.sampleId);

            //                btnOn.CssClass = "btn btn-success";
            //                btnOff.CssClass = "btn btn-default";
            //                btnOn.Enabled = false;
            //                btnOff.Enabled = false;

            //            }
            //        }
            //        if (_cm.inquiryStatusId == 16 || _cm.inquiryStatusId == 17)
            //        {
            //            if (_cm.inquiryId != 0)
            //            {
            //                InquiryID = Convert.ToInt32(_cm.inquiryId);

            //                btnOn.CssClass = "btn btn-default";
            //                btnOff.CssClass = "btn btn-danger";
            //                btnOn.Enabled = true;
            //                btnOff.Enabled = true;

            //            }
            //        }
            //    }
            //}
        }
        protected void gvCRFList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int rowIndex = 0;
            int inquiryID = 0;
            if (e.CommandName == "CRFApprove")
            {
                rowIndex = int.Parse(e.CommandArgument.ToString());
                inquiryID = (int)this.gvCRFList.DataKeys[rowIndex]["InquiryID"];

                Session["InquiryID"] = inquiryID;

                Response.Redirect("TestRequestReviewChecklist.aspx");


                //            BindCRFList();
            }
            if (e.CommandName == "CRFMoreInfo")
            {
                rowIndex = int.Parse(e.CommandArgument.ToString());
                inquiryID = (int)this.gvCRFList.DataKeys[rowIndex]["InquiryID"];
                int CloneCRFSampleID = (int)this.gvCRFList.DataKeys[rowIndex]["CloneCRFSampleID"];

                //change CloneCRF Status to CRF More Info
                _cm.CRFStatusID = 3;
                _cm.CloneCRFSampleID = CloneCRFSampleID;
                _cm.UpdateCRFStatus(_cm);


                BindCRFList();
            }
            if (e.CommandName == "On")
            {
                int index = int.Parse(e.CommandArgument.ToString());
                int Id = int.Parse(gvCRFList.DataKeys[index]["InquiryID"].ToString());
                int CloneCRFSampleID = int.Parse(gvCRFList.DataKeys[index]["CloneCRFSampleID"].ToString());

                Button btnOn = (Button)gvCRFList.Rows[rowIndex].Cells[0].FindControl("btnSampleReceivedOn");
                Button btnOff = (Button)gvCRFList.Rows[rowIndex].Cells[0].FindControl("btnSampleReceivedOff");

                DataTable dt = _cm.GetCustomerInquiryDetails(Id, 1);
                if (dt.Rows.Count > 0)
                {
                    _cm.CRFStatusID = 13;
                    _cm.CloneCRFSampleID = CloneCRFSampleID;
                    _cm.UpdateCRFStatus(_cm);
                }

                btnOn.CssClass = "btn btn-success";
                btnOff.CssClass = "btn btn-default";
                BindCRFList();
            }
            if (e.CommandName == "Off")
            {
                Button btnOn = (Button)gvCRFList.Rows[rowIndex].Cells[0].FindControl("btnSampleReceivedOn");
                Button btnOff = (Button)gvCRFList.Rows[rowIndex].Cells[0].FindControl("btnSampleReceivedOff");

                btnOn.CssClass = "btn btn-default";
                btnOff.CssClass = "btn btn-danger";
            }
            if (e.CommandName == "SampleManagement")
            {
                int index = int.Parse(e.CommandArgument.ToString());
                inquiryID = int.Parse(gvCRFList.DataKeys[index]["InquiryID"].ToString());
                int CloneCRFSampleID = int.Parse(gvCRFList.DataKeys[index]["CloneCRFSampleID"].ToString());
                //inquiryID = (int)this.gvCRFList.DataKeys[rowIndex]["InquiryID"];
                Session["InquiryID"] = inquiryID;
                Session["CloneCRFSampleID"] = CloneCRFSampleID;
                DataTable dt = _cm.GetReportFormat(inquiryID);
                if (dt.Rows.Count > 0)
                {
                    string sampleVerification = dt.Rows[0]["sampleVerification"].ToString();
                    Response.Redirect(sampleVerification);
                }

            }
        }
    }
}