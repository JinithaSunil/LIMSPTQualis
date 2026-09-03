using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ZXing;

namespace LIMS
{
    public partial class SampleListMocon : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int CustomerUID = 0; string SampleIDSeriesAll = ""; string[] SampleIDSeries;int CloneCRFSampleID = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlControl liHome1 = (HtmlGenericControl)this.Master.FindControl("liHome1");
            HtmlControl liCustomers1 = (HtmlGenericControl)this.Master.FindControl("liCustomers1");
            HtmlControl liInquiries1 = (HtmlGenericControl)this.Master.FindControl("liInquiries1");
            HtmlControl liCRFList1 = (HtmlGenericControl)this.Master.FindControl("liCRFList1");
            HtmlControl liSampleManagement1 = (HtmlGenericControl)this.Master.FindControl("liSampleManagement1");
            HtmlControl liTestManagement1 = (HtmlGenericControl)this.Master.FindControl("liTestManagement1");
            HtmlControl liQualityManagement1 = (HtmlGenericControl)this.Master.FindControl("liQualityManagement1");
            HtmlControl liCustomerFeedback1 = (HtmlGenericControl)this.Master.FindControl("liCustomerFeedback1");
            HtmlControl liUserManagement1 = (HtmlGenericControl)this.Master.FindControl("liUserManagement1");
            HtmlControl liSupport1 = (HtmlGenericControl)this.Master.FindControl("liSupport1");
            HtmlControl liViewInquiries2 = (HtmlGenericControl)this.Master.FindControl("liViewInquiries2");
			HtmlControl liInquiryStatus2 = (HtmlGenericControl)this.Master.FindControl("liInquiryStatus2");
            if (Session["Usertype"] != null)
            {
                HtmlControl liSampleManagement = (HtmlGenericControl)this.Master.FindControl("liSampleManagement1");
                HtmlControl liViewSamples2 = (HtmlGenericControl)this.Master.FindControl("liViewSamples2");
				HtmlControl liAllSamples2 = (HtmlGenericControl)this.Master.FindControl("liAllSamples2");
                
                HtmlControl liWorkList2 = (HtmlGenericControl)this.Master.FindControl("liWorkList2");
                
                HtmlControl liGenerateReport2 = (HtmlGenericControl)this.Master.FindControl("liGenerateReport2");
                
                

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
                    switch (RoleID)
                    {
                        case 1://QC
                            liHome1.Visible = true;
                            liCustomers1.Visible = true;
                            liInquiries1.Visible = true;
                            liCRFList1.Visible = true;
                            liSampleManagement1.Visible = true;
                            liTestManagement1.Visible = false;
                            liQualityManagement1.Visible = true;
                            liCustomerFeedback1.Visible = false;
                            liUserManagement1.Visible = false;
                            liSupport1.Visible = false;
                            break;
                        case 2://Manager
                            liHome1.Visible = true;
                            liCustomers1.Visible = false;
                            liInquiries1.Visible = false;
                            liCRFList1.Visible = false;
                            liSampleManagement1.Visible = true;
							liAllSamples2.Visible = true;
                            if (Session["DepartmentID"].ToString() == "1")
                            {
                                liViewSamples2.Visible = false;
                                liWorkList2.Visible = false;
                                liGenerateReport2.Visible = false;

                                
                                
                                
                            }
                            else
                            {
                                liViewSamples2.Visible = true;
                                liWorkList2.Visible = true;
                                liGenerateReport2.Visible = true;

                                
                                
                               
                            }
                            liTestManagement1.Visible = true;
                            liQualityManagement1.Visible = true;
                            liCustomerFeedback1.Visible = false;
                            liUserManagement1.Visible = false;
                            liSupport1.Visible = false;
                            break;
                        case 3://Lab Technician
                            liHome1.Visible = true;
                            liCustomers1.Visible = false;
                            liInquiries1.Visible = false;
                            liCRFList1.Visible = false;
                            liSampleManagement1.Visible = false;
                            liTestManagement1.Visible = true;
                            liQualityManagement1.Visible = false;
                            liCustomerFeedback1.Visible = false;
                            liUserManagement1.Visible = false;
                            liSupport1.Visible = false;
                            break;
                        case 4://Admin
                            liHome1.Visible = true;
                            liCustomers1.Visible = true;
                            liInquiries1.Visible = true;
                            liCRFList1.Visible = true;
                            liSampleManagement1.Visible = true;
                            liTestManagement1.Visible = true;
                            liQualityManagement1.Visible = true;
                            liCustomerFeedback1.Visible = false;
                            liUserManagement1.Visible = false;
                            liSupport1.Visible = false;
                            break;
                        case 5://Sample Management
                            liHome1.Visible = true;
                            liCustomers1.Visible = true;
                            liInquiries1.Visible = true;
                            liCRFList1.Visible = true;
                            liSampleManagement1.Visible = true;
                            if (Session["DepartmentID"].ToString() == "1")
                            {
                                liViewSamples2.Visible = false;
                            }
                            else
                            {
                                
                            }
                            liTestManagement1.Visible = false;
                            liQualityManagement1.Visible = false;
                            liCustomerFeedback1.Visible = false;
                            liUserManagement1.Visible = false;
                            liSupport1.Visible = false;
                            break;
                        default:
                            break;
                    }
                }
                if (!Page.IsPostBack)
                {
                    BindYear();
                    liSampleManagement1.Attributes.Add("class", "active");
                    
					liViewSamples2.Visible = false;
                    Session["Id"] = 1;
                    if (Request.QueryString["s"] != null)
                    {
                        int InquiryStatus = Convert.ToInt32(Request.QueryString["s"].ToString());
                        BindSampleList(InquiryStatus);
                    }
                    else
                    {
                        BindSampleList();
                        BindAllocatedTo();
                    }
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
        protected void BindYear()
        {
            DataTable dt = new DataTable();
            List<int> years = new List<int>();
            int currentYear = DateTime.Now.Year;
            for (int year = currentYear; year >= currentYear - 5; year--)
            {
                years.Add(year);
            }

            // Bind the dropdownlist
            ddlYear.DataSource = years;
            ddlYear.DataBind();
            ddlYear.Items.Insert(0, "Select Year");
        }
        private void BindSampleList()
        {
            string DepartmentID = "";
            string Id = Session["Id"].ToString();
            if (Session["DepartmentID"] != null)
            {
                if (ddlYear.SelectedIndex != 0)
                {
                    int year = Convert.ToInt32(ddlYear.SelectedValue);
                    int month = Convert.ToInt32(ddlMonth.SelectedValue);
                    DepartmentID = Session["DepartmentID"].ToString();
                    DataTable dt = _cm.GetDepartmentSampleDetails(DepartmentID,year,month);
                    gvSampleList.DataSource = dt;
                    gvSampleList.DataBind();
                }            
            }
        }
        private void BindSampleList(int InquiryStatus)
        {
            string Id = Session["Id"].ToString();
            DataTable dt = _cm.getAssignedSampleDetails(19);
            gvSampleList.DataSource = dt;
            gvSampleList.DataBind();
        }
        protected void BindAllocatedTo()
        {
            int SupervisorID = Convert.ToInt32(Session["EmployeeID"].ToString());
            DataTable dt3 = _cm.GetEmployeeDetails(SupervisorID, true);
            if (dt3.Rows.Count > 0)
            {
                ddlAllocatedTo.DataSource = dt3;
                ddlAllocatedTo.DataTextField = "EmployeeName";
                ddlAllocatedTo.DataValueField = "ID1";
                ddlAllocatedTo.DataBind();
                ddlAllocatedTo.Items.Insert(0, new ListItem("Select Employee"));
            }
        }
        protected void gvSampleList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            string InquiryId = "";
            string RequestId = "";
            int AllocationID = 0; int sampleUID = 0;
            int SupervisorID = Convert.ToInt32(Session["EmployeeID"].ToString());
            
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int CloneCRFSampleID = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "CloneCRFSampleID").ToString());
                Button btnSampleManagement = (e.Row.FindControl("btnSampleManagement") as Button);
               // Button btnAllocate = (e.Row.FindControl("btnAllocate") as Button);
                Button btnAccept = (e.Row.FindControl("btnAccept") as Button);
                Button btnReject = (e.Row.FindControl("btnReject") as Button);

                HyperLink hypCustCRF = (e.Row.FindControl("hypCustCRF") as HyperLink);
                HyperLink hypCRF = (e.Row.FindControl("hypCRF") as HyperLink);
                HyperLink hypResult = (e.Row.FindControl("hypResult") as HyperLink);
                HyperLink hypCustomerFeedback = (e.Row.FindControl("hypCustomerFeedback") as HyperLink);
                HyperLink hypAllocation = (e.Row.FindControl("hypAllocation") as HyperLink);

                Label lblAllocatedTo = (e.Row.FindControl("lblAllocatedTo") as Label);
                Label lblSampleIDSeries = (e.Row.FindControl("lblSampleIDSeries") as Label);

                CheckBox chkSample = (e.Row.FindControl("chkSample") as CheckBox);

                int index = e.Row.RowIndex;

                int SampleId = int.Parse(gvSampleList.DataKeys[index]["ID"].ToString());
                sampleUID = int.Parse(gvSampleList.DataKeys[index]["SampleID"].ToString());
                Session["SampleId"] = sampleUID;

                int InquiryID = int.Parse(gvSampleList.DataKeys[index]["InquiryID"].ToString());
                DataTable dt7 = _cm.GetAllocationFileDetails(SampleId);
                DataTable dt4 = _cm.GetCustomerInquiryDetails(InquiryID, 1);
                DataTable dt6 = _cm.GetAllocationDetails(SampleId);
                DataTable dt2 = _cm.getInquiryDetails(InquiryID, 1);
                if (dt2.Rows.Count > 0)
                {
                    foreach (DataRow dr2 in dt2.Rows)
                    {
                        CustomerUID = Convert.ToInt32(dr2["CustomerUID"].ToString());
                    }
                }
                if (dt7.Rows.Count > 0)
                {
                    foreach (DataRow dr7 in dt7.Rows)
                    {
                        AllocationID = Convert.ToInt32(dr7["AllocationID"].ToString());
                        sampleUID = Convert.ToInt32(dr7["sampleUID"].ToString());
                    }
                }
                if (dt4.Rows.Count > 0)
                {
                    InquiryId = dt4.Rows[0]["InquiryId"].ToString();
                    DataTable dt5 = _cm.GetCustomerRequestDetails(InquiryId);
                    if (dt5.Rows.Count > 0)
                    {
                        RequestId = dt5.Rows[0]["CustomerRequestId"].ToString();
                        _cm.requestTypeId = Convert.ToInt32(dt5.Rows[0]["RequestTypeId"].ToString());
                        Session["RequestId"] = RequestId;
                        if (_cm.requestTypeId == 1)
                        {
                            hypCRF.NavigateUrl = "~/CreateRequestFM.aspx";
                        }
                        if (_cm.requestTypeId == 2)
                        {
                            hypCRF.NavigateUrl = "~/CreateRequestMocon.aspx";
                        }
                        if (_cm.requestTypeId == 3)
                        {
                            hypCRF.NavigateUrl = "~/CreateRequestPTI.aspx";
                        }
                    }

                    Label lblStatusName = (e.Row.FindControl("lblStatusName") as Label);
                    switch (lblStatusName.Text)
                    {
                        case "CRF Received":
                            hypCustCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                            hypCustCRF.Visible = true;
                            hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryId + CustomerUID + ".htm");
                            hypCRF.Visible = true;
                            btnSampleManagement.Visible = false;
                            hypResult.Visible = false;
                            hypCustomerFeedback.Visible = false;
                            chkSample.Visible = false;
                            lblSampleIDSeries.Visible = true;
                            break;
                        case "CRF Finalized":
                            hypCustCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                            hypCustCRF.Visible = true;
                            hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryId + CustomerUID + ".htm");
                            hypCRF.Visible = true;
                            btnSampleManagement.Visible = false;
                            hypResult.Visible = false;
                            hypCustomerFeedback.Visible = false;
                            chkSample.Visible = false;
                            lblSampleIDSeries.Visible = true;
                            break;
                        case "Sample Accepted By Sample Management":
                            hypCustCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                            hypCustCRF.Visible = true;
                            hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryId + CustomerUID + ".htm");
                            hypCRF.Visible = true;
                            btnSampleManagement.Visible = false;
                            hypResult.Visible = false;
                            btnAccept.Visible = true;
                            btnReject.Visible = true;
                            hypCustomerFeedback.Visible = false;
                            chkSample.Visible = false;
                            lblSampleIDSeries.Visible = true;
                            break;
                        case "Allocated For Testing":
                            if (dt6.Rows.Count > 0)
                            {
                                Session["AllocationId"] = dt6.Rows[0]["AllocationId"].ToString();
                                lblAllocatedTo.Text = dt6.Rows[0]["EmployeeName"].ToString();
                            }
                            hypCustCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                            hypCustCRF.Visible = true;
                            hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryId + CustomerUID + ".htm");
                            hypCRF.Visible = true;
                            hypAllocation.NavigateUrl = string.Format("~/TestAllocation/Allocation-" + sampleUID + ".htm");
                            hypAllocation.Visible = true;
                            hypResult.Visible = false;
                            hypCustomerFeedback.Visible = false;
                            chkSample.Visible = false;
                            lblSampleIDSeries.Visible = true;
                            break;
                        case "Sample Rejected By Sample Management":
                            hypCustCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                            hypCustCRF.Visible = true;
                            hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryId + CustomerUID + ".htm");
                            hypCRF.Visible = true;
                            hypResult.Visible = false;
                            btnSampleManagement.Visible = true;
                            hypCustomerFeedback.Visible = false;
                            chkSample.Visible = false;
                            lblSampleIDSeries.Visible = true;
                            break;
                        case "Sample Accepted By Lab":
                            hypCustCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                            hypCustCRF.Visible = true;
                            hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryId + CustomerUID + ".htm");
                            hypCRF.Visible = true;
                            hypResult.Visible = false;
                            btnSampleManagement.Visible = false;
                            btnAccept.Visible = false;
                            btnReject.Visible = false;
                            hypCustomerFeedback.Visible = false;
                            if (Session["DepartmentID"].ToString() == "1")
                            {
                                chkSample.Visible = true;
                                lblSampleIDSeries.Visible = true;
                               // ddlAllocatedTo.Visible = false;
                               //  btnAllocate.Visible = false;
                            }
                            else
                            {
                             //   ddlAllocatedTo.Visible = false;
                            //    btnAllocate.Visible = false;
                            }
                            
                            lblSampleIDSeries.Visible = false;
                            break;
                        case "Sample Rejected By Lab":
                            hypCustCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                            hypCustCRF.Visible = true;
                            hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryId + CustomerUID + ".htm");
                            hypCRF.Visible = true;
                            hypResult.Visible = false;
                            btnSampleManagement.Visible = false;
                            btnAccept.Visible = false;
                            btnReject.Visible = false;
                            hypCustomerFeedback.Visible = false;
                            chkSample.Visible = false;
                            lblSampleIDSeries.Visible = true;
                            break;
                        case "Test Result Submitted":
                            hypCustCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                            hypCustCRF.Visible = true;
                            hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryId + CustomerUID + ".htm");
                            hypCRF.Visible = true;
                            hypResult.Visible = false;
                            btnSampleManagement.Visible = false;
                            btnAccept.Visible = false;
                            btnReject.Visible = false;
                            hypCustomerFeedback.Visible = false;
                            chkSample.Visible = false;
                            lblSampleIDSeries.Visible = true;
                            break;
                        case "Test Report Submitted for Review":
                            hypCustCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                            hypCustCRF.Visible = true;
                            hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryId + CustomerUID + ".htm");
                            hypCRF.Visible = true;
                            hypResult.Visible = false;
                            btnSampleManagement.Visible = false;
                            btnAccept.Visible = false;
                            btnReject.Visible = false;
                            hypCustomerFeedback.Visible = false;
                            chkSample.Visible = false;
                            lblSampleIDSeries.Visible = true;
                            break;
                        case "Test Report Generated":
                            hypCustCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                            hypCustCRF.Visible = true;
                            hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryId + CustomerUID + ".htm");
                            hypCRF.Visible = true;
                            hypResult.Visible = false;
                            btnSampleManagement.Visible = false;
                            btnAccept.Visible = false;
                            btnReject.Visible = false;
                            hypAllocation.NavigateUrl = string.Format("~/TestAllocation/Allocation-" + sampleUID + ".htm");
                            hypAllocation.Visible = true;
                            DataTable dt8 = _cm.getTestReportID(sampleUID);
                            if (dt8.Rows.Count > 0)
                            {
                                int TestResultId = Convert.ToInt32(dt8.Rows[0]["TestResultId"].ToString());
                                hypResult.NavigateUrl = string.Format("~/TestReport/TestReport-" + TestResultId + sampleUID + ".htm");
                                hypResult.Visible = true;
                            }
                            hypCustomerFeedback.Visible = false;
                            chkSample.Visible = false;
                            lblSampleIDSeries.Visible = true;
                            break;
                        case "Feedback Received":
                            hypCustCRF.NavigateUrl = string.Format("~/CustomerRequest/CustomerRequest-" + InquiryID + CustomerUID + ".htm");
                            hypCustCRF.Visible = true;
                            hypCRF.NavigateUrl = string.Format("~/CustomerRequest/Request-" + CloneCRFSampleID + InquiryId + CustomerUID + ".htm");
                            hypCRF.Visible = true;
                            hypResult.Visible = false;
                            btnSampleManagement.Visible = false;
                            btnAccept.Visible = false;
                            btnReject.Visible = false;
                            hypCustomerFeedback.NavigateUrl = string.Format("CustomerFeedbackStatus.aspx?ID=" + InquiryID);
                            hypCustomerFeedback.Visible = true;
                            chkSample.Visible = false;
                            lblSampleIDSeries.Visible = true;   
                            break;
                        default:
                            break;
                    }
                }

            }
        }
        protected void gvSampleList_RowCommand(object sender, GridViewCommandEventArgs e)
        {

                int index = int.Parse(e.CommandArgument.ToString());
                int InquiryID = int.Parse(gvSampleList.DataKeys[index]["InquiryID"].ToString());
                int CloneCRFSampleID = int.Parse(gvSampleList.DataKeys[index]["CloneCRFSampleID"].ToString());
                GridView grid = (GridView)sender;
                GridViewRow row1 = grid.Rows[index];
                System.Web.UI.WebControls.Image imgageQRCode = (System.Web.UI.WebControls.Image)row1.FindControl("imgageQRCode");

                Session["CloneCRFSampleID"] = CloneCRFSampleID;

                if (e.CommandName == "Select")
                {                
                    string SampleId = ((Label)row1.FindControl("lblSampleIDSeries")).Text.ToString(); //this store the  value in varName1

                    var QCwriter = new BarcodeWriter();
                    QCwriter.Format = BarcodeFormat.QR_CODE;
                    var result = QCwriter.Write(SampleId);
                    string path = Server.MapPath("~/images/MyQRImage.jpg");
                    var barcodeBitmap = new Bitmap(result);

                    using (MemoryStream memory = new MemoryStream())
                    {
                        using (FileStream fs = new FileStream(path,
                           FileMode.Create, FileAccess.ReadWrite))
                        {
                            barcodeBitmap.Save(memory, ImageFormat.Jpeg);
                            byte[] bytes = memory.ToArray();
                            fs.Write(bytes, 0, bytes.Length);
                        }
                    }
                    imgageQRCode.Visible = true;
                //resize image 
                float width = 200;
                float height = 200;
                var brush = new SolidBrush(Color.Black);
                var image = new Bitmap(result);
                float scale = Math.Min(width / image.Width, height / image.Height);
                var bmp = new Bitmap((int)width, (int)height);
                var graph = Graphics.FromImage(bmp);

                // uncomment for higher quality output
                //graph.InterpolationMode = InterpolationMode.High;
                //graph.CompositingQuality = CompositingQuality.HighQuality;
                //graph.SmoothingMode = SmoothingMode.AntiAlias;

                var scaleWidth = (int)(image.Width * scale);
                var scaleHeight = (int)(image.Height * scale);

                graph.FillRectangle(brush, new RectangleF(0, 0, width, height));
                graph.DrawImage(image, ((int)width - scaleWidth) / 2, ((int)height - scaleHeight) / 2, scaleWidth, scaleHeight);
                using (MemoryStream memory = new MemoryStream())
                {
                    using (FileStream fs = new FileStream(path,
                       FileMode.Create, FileAccess.ReadWrite))
                    {
                        bmp.Save(memory, ImageFormat.Jpeg);
                        byte[] bytes = memory.ToArray();
                        fs.Write(bytes, 0, bytes.Length);
                    }
                }
                imgageQRCode.ImageUrl = "~/images/MyQRImage.jpg";
                    //int index = Convert.ToInt32(e.CommandArgument);
                    //GridViewRow row = gvSampleList.Rows[index];
                    //int SampleId = int.Parse(gvSampleList.DataKeys[index]["ID"].ToString());
                    //string SampleIdSeries = "";
                    //DataTable dt5 = _cm.getSampleIDSeries(SampleId);
                    //if (dt5.Rows.Count > 0)
                    //{
                    //    SampleIdSeries = dt5.Rows[0]["sampleIDseries"].ToString();
                    //}

                    //var QCwriter = new BarcodeWriter();
                    //QCwriter.Format = BarcodeFormat.QR_CODE;
                    //var result = QCwriter.Write(SampleIdSeries);
                    //string path = Server.MapPath("~/images/MyQRImage.jpg");
                    //var barcodeBitmap = new Bitmap(result);

                    //using (MemoryStream memory = new MemoryStream())
                    //{
                    //    using (FileStream fs = new FileStream(path,
                    //       FileMode.Create, FileAccess.ReadWrite))
                    //    {
                    //        barcodeBitmap.Save(memory, ImageFormat.Jpeg);
                    //        byte[] bytes = memory.ToArray();
                    //        fs.Write(bytes, 0, bytes.Length);
                    //    }
                    //}
                    //string url = ResolveUrl("~/images/MyQRImage.jpg");
                    //string fullURL = "window.open('" + url + "', '_blank', 'height=600,width=1000,status=yes,toolbar=no,menubar=no,location=no,scrollbars=yes,resizable=yes,titlebar=no' );";
                    //// ScriptManager.RegisterStartupScript(this, typeof(string), "OPEN_WINDOW", fullURL, true);
                    //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "tmp", "<script type='text/javascript'>showMyImage();</script>", false);
                }            
            if (e.CommandName == "Reject")
            {
                DataTable dt = new DataTable();
                DataTable dt2 = new DataTable();
                
                //change CloneCRF status to 'Sample Rejected by Lab'
                _cm.CRFStatusID = 17;
				_cm.CloneCRFSampleID = CloneCRFSampleID;
				_cm.UpdateCRFStatus(_cm);
				
                BindSampleList();
            }
            if (e.CommandName == "Accept")
            {
                DataTable dt = new DataTable();
                DataTable dt2 = new DataTable();

                //change CloneCRF status to 'Sample Accepted by Lab'
                _cm.CRFStatusID = 15;
				_cm.CloneCRFSampleID = CloneCRFSampleID;
				_cm.UpdateCRFStatus(_cm);
				
                BindSampleList();
            }
            if (e.CommandName == "SampleManagement")
            {
                int rowIndex = int.Parse(e.CommandArgument.ToString());
                int inquiryID = (int)this.gvSampleList.DataKeys[rowIndex]["InquiryID"];
                Session["InquiryID"] = inquiryID;
                Response.Redirect("SampleVerification.aspx");
            }

        }
        protected void btnAllocate_Click(object sender, EventArgs e)
        {
            int flag = 0;
            if (ddlAllocatedTo.SelectedIndex == 0)
            {
                ddlAllocatedTo.BackColor = Color.LightPink;
                flag = 1;
            }
            else
            {
                ddlAllocatedTo.BackColor = Color.Empty;
            }
            if (flag == 0)
            {
                int InquiryId = 0; int SampleId = 0;int count = 0;int sampleUID = 0;
                foreach (GridViewRow gvrow in gvSampleList.Rows)
                {
                    var chkSample = gvrow.FindControl("chkSample") as CheckBox;
                    if (chkSample.Checked)
                    {
                        count += 1;
                    }
                }
                if(count == 0)
                {
                    lblMsg.Text="Select Sample For Allocation";
                }
                else
                {
                    foreach (GridViewRow gvrow in gvSampleList.Rows)
                    {
                        HyperLink hypAllocation = (gvrow.FindControl("hypAllocation") as HyperLink);
                        var chkSample = gvrow.FindControl("chkSample") as CheckBox;
                        if (chkSample.Checked)
                        {
                            string SampleIDSeries = chkSample.Text;
                            SampleIDSeriesAll += SampleIDSeries + ",";
                            DataTable dtInquiry = _cm.getSampleInquiryID(SampleIDSeries);
                            if (dtInquiry.Rows.Count > 0)
                            {
                                CloneCRFSampleID = Convert.ToInt32(dtInquiry.Rows[0]["CloneCRFSampleID"].ToString());
                                InquiryId = Convert.ToInt32(dtInquiry.Rows[0]["InquiryID"].ToString());
                                SampleId = Convert.ToInt32(dtInquiry.Rows[0]["ID"].ToString());
                                sampleUID = Convert.ToInt32(dtInquiry.Rows[0]["sampleID"].ToString());
                            }
                            DataTable dt2 = _cm.GetProductCategory(InquiryId);
                            int AllocationID = 0;
                            foreach (DataRow dr in dt2.Rows)
                            {
                                _cm.department = dr["DepartmentID"].ToString();
                                _cm.productCategory = dr["subgroupid"].ToString();
                            }
                            _cm.employeID = ddlAllocatedTo.SelectedItem.Value;
                            _cm.sampleId = SampleId;

                            AllocationID = _cm.AddAllocationDetails(_cm);
                            Session["AllocationId"] = AllocationID;

                            DataTable dt = _cm.GetCustomerInquiryDetails(SampleId);
                            if (dt.Rows.Count > 0)
                            {
                                //change CloneCRF status to 'Allocated for Testing'
								_cm.CRFStatusID = 19;
								_cm.CloneCRFSampleID = CloneCRFSampleID;
								_cm.UpdateCRFStatus(_cm);

                                _cm.sampleId = SampleId;
                                _cm.sampleStatusID = 19;
                                _cm.updateSampleStatus(_cm);
                            }
                           // hypAllocation.NavigateUrl = string.Format("~/TestAllocation/Allocation-" + sampleUID + ".htm");
                           // hypAllocation.Visible = true;
                        }
                    }
                    DataTable dtCRFType = _cm.getCRFTypeDetails(InquiryId);
                    if (dtCRFType.Rows.Count > 0)
                    {
                        int CRFID = Convert.ToInt32(dtCRFType.Rows[0]["CRFID"].ToString());
                        switch (CRFID)
                        {
                            case 1:
                                Response.Redirect("TestAllocationFM.aspx");
                                break;
                            case 2:
                                Session["SampleIDSeriesAll"] = SampleIDSeriesAll;
                                Response.Redirect("TestAllocationMocon.aspx");
                                break;
                            case 3:
                                Response.Redirect("TestAllocationPTI.aspx");
                                break;
                            default: break;
                        }
                    }
                    Response.Redirect("~/GenerateTestAllocation.aspx");                    
                    BindSampleList();
                }
            }
            else
            {
                lblMsg.Text = "Fill Highlighted Fields";
            }
        }

        protected void chkSample_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = sender as CheckBox;

            if (chk.Checked)
            {
                GridViewRow row = (GridViewRow)chk.NamingContainer;
                string sampleIDSeries = row.Cells[1].Text;
            }
        }
        protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlMonth.SelectedIndex == 0)
            {
                ddlMonth.BorderColor = Color.Red;
            }
            else
            {
                BindSampleList();
            }

        }

        protected void ddlMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlMonth.SelectedIndex == 0)
            {
                ddlMonth.BorderColor = Color.Red;
            }
            else
            {
                BindSampleList();
            }
        }
    }
}