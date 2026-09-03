using iTextSharp.tool.xml.html;
using LIMS.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Image = System.Web.UI.WebControls.Image;

namespace LIMS
{
    public partial class SampleImages : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM(); int EmployeeID = 0; int CloneCRFSampleID = 0;
        DAL.mailer objEmail = new DAL.mailer();
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
                if (Session["EmployeeID"] != null)
                {
                    EmployeeID = Convert.ToInt32(Session["EmployeeID"].ToString());
                }
                if (!Page.IsPostBack)
                {
                    BindInstruments();
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
        private void BindInstruments()
        {
            Session["EnquiryID"] = 585;
            if (Session["EnquiryID"] != null)
            {
                int EnquiryID = Convert.ToInt32(Session["EnquiryID"]);

                DataTable dtCalibRequest = _cm.GetCalibrationRequest(EnquiryID, 1);
                if (dtCalibRequest.Rows.Count > 0)
                {
                    lblInquiryID.Text = "ENQ" + dtCalibRequest.Rows[0]["EnquiryID"];
                    lblSubContractor.Text = dtCalibRequest.Rows[0]["SubContractorName"].ToString();
                    gvInstruments.DataSource = dtCalibRequest;
                    gvInstruments.DataBind();
                }
                else
                {
                    gvInstruments.DataSource = null;
                    gvInstruments.DataBind();
                }
            }

        }
        protected void gvInstruments_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvInstruments.EditIndex = e.NewEditIndex;
            BindInstruments();
        }

        protected void gvInstruments_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = gvInstruments.Rows[e.RowIndex];
            int EnquiryID = int.Parse(gvInstruments.DataKeys[e.RowIndex]["EnquiryID"].ToString());
            int QuotationID = int.Parse(gvInstruments.DataKeys[e.RowIndex]["QuotationID"].ToString());
            int RequestID = int.Parse(gvInstruments.DataKeys[e.RowIndex]["RequestID"].ToString());
            int SubContractorID = int.Parse(gvInstruments.DataKeys[e.RowIndex]["SubContractorID"].ToString());
            RadioButtonList radStatus = gvInstruments.Rows[e.RowIndex].FindControl("radStatus") as RadioButtonList;
            Label lblStatus = gvInstruments.Rows[e.RowIndex].FindControl("lblStatus") as Label;
            FileUpload fpSampleImage = (FileUpload)row.FindControl("fpSampleImage");
            Label lblInstrumentName = (Label)row.FindControl("lblInstrumentName");
            _cm.RequestID = RequestID;
            string photoPath = "";
            if (fpSampleImage.HasFile)
            {
                int count = 1;
                fpSampleImage.BorderColor = System.Drawing.Color.Empty;
                string folderPath = Server.MapPath("SampleImages/");               
                FileInfo fi = new FileInfo(fpSampleImage.FileName);
                string ext = fi.Extension;

                fpSampleImage.SaveAs(folderPath + Path.GetFileName(lblInquiryID.Text + "-" + RequestID + ext));
                _cm.SampleFileName = lblInquiryID.Text + "-" + RequestID + ext;
                _cm.SampleText = lblInstrumentName.Text;
                _cm.created = DateTime.Now.AddHours(12.5);

                int ID = _cm.SampleImageUpload(_cm);
                _cm.UpdateImageCalibrationRequest(_cm);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('File Upload Successful')", true);
               
            }
            else
            {
                // Keep existing image if not uploading a new one
                System.Web.UI.WebControls.Image imgEditPhoto = (System.Web.UI.WebControls.Image)row.FindControl("imgEditPhoto");
                photoPath = imgEditPhoto.ImageUrl;
            }

            _cm.RequestID = RequestID;
            //_cm.UpdateToolStatus(_cm);
            gvInstruments.EditIndex = -1;
            BindInstruments();
        }

        protected void gvInstruments_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvInstruments.EditIndex = -1;
            BindInstruments();
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            DataTable dtUploadCompleted = _cm.CheckCalibrationImageUploadCompleted(Convert.ToInt32(lblEnquiryID.Text));
            if (dtUploadCompleted.Rows.Count == 0)
            {
                _cm.QuotationStatusID = 56;// Calibration Images Sent to Sub Contractor
                _cm.QuotationID = Convert.ToInt32(lblQuotationID.Text);
                _cm.UpdateQuotationCalibStatus(_cm);

                string SubContractorName = "";
                string Subject = "";
                string FromAddress = "";
                string ToAddress = "";

                DataTable dtSubContractor = _cm.GetSubContractorDetails(Convert.ToInt32(lblSubContractorID.Text));
                if(dtSubContractor.Rows.Count > 0)
                {
                    ToAddress = dtSubContractor.Rows[0]["Email"].ToString();
                    SubContractorName = dtSubContractor.Rows[0]["SubContractorName"].ToString();
                }
               
                StringBuilder sb = new StringBuilder();
                StringBuilder sbImg = new StringBuilder();
                string CCAddress = "";
                string BCCAddress = "";
                string Body = "";
                string Note = "";
                               
                DataTable dtEmailDetails = _cm.getEmaildetails("Sub Contractor Images");
                if (dtEmailDetails.Rows.Count > 0)
                {

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
                    if (dtEmailDetails.Rows[0]["EmailSubject"].ToString() != "")
                    {
                        Subject = "Enquiry#" + lblEnquiryID.Text + " : " + dtEmailDetails.Rows[0]["EmailSubject"].ToString();
                    }
                    sb.AppendFormat(string.Format("<a href='http://localhost:41414/EnquiryForm.aspx?Reference={0}&Enquiry={1}' target='_blank'>", _cm.Reference, _cm.EnquiryID));
                    sb.AppendFormat("Quotation");
                    sb.Append("</a>");
                    
                    string body = objEmail.PrepareMailBodyWith("SubContractorImages.html", "{SubContractorName}", SubContractorName, "{EnquiryID}", lblEnquiryID.Text);
                    string str = body.ToString();

                    string EnquiryID = lblEnquiryID.Text;
                    DataTable dtEnq = _cm.GetCalibrationRequestContact(Convert.ToInt32(EnquiryID));
                    if (dtEnq.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtEnq.Rows)
                        {
                            string html = HttpContext.Current.Server.MapPath("//SampleImages/" + dr["SampleImage"].ToString());
                            sbImg.Append(html + ";");
                        }
                    }
                    _cm.FromAddress = FromAddress;
                    _cm.ToAddress = ToAddress;
                    _cm.CCAddress = CCAddress;
                    _cm.BCCAddress = BCCAddress;
                    _cm.Subject = Subject;
                    _cm.Purpose = "Images to Sub Contractor";
                    _cm.FileName = "SubContractorImages.html";
                    _cm.SentTo = _cm.VendorName;
                    _cm.Body = str.ToString();
                    try
                    {
                        objEmail.SendMail(FromAddress, ToAddress, Subject, body, sbImg.ToString());
                        Response.Redirect("CustomerPOList.aspx");
                    }
                    catch (Exception ex)
                    {
                        string s = ex.Message;
                    }
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Upload Images for all Instruments!')", true);
                }
            }
        }

        protected void gvInstruments_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }

        protected void gvInstruments_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == System.Web.UI.WebControls.DataControlRowType.DataRow)
            {
                Image img = e.Row.FindControl("imgPhoto") as Image;
                if (img != null && string.IsNullOrEmpty(img.ImageUrl))
                    img.ImageUrl = "~/SampleImages/no-image.png";

                DataKey dataKey = gvInstruments.DataKeys[e.Row.RowIndex];

                // Access individual data key values by name or index
                if (dataKey != null)
                {
                    // By Name:
                    lblEnquiryID.Text = dataKey.Values["EnquiryID"].ToString();
                    lblQuotationID.Text = dataKey.Values["QuotationID"].ToString();
                    lblSubContractorID.Text = dataKey.Values["SubContractorID"].ToString();
                }
            }
        }
    }
}