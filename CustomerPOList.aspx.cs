using DocumentFormat.OpenXml.Bibliography;
using LIMS.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class CustomerPOList : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        decimal sumFooterValue = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["Usertype"] != null)
                {
                    BindCustomerPO();
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }

        private void BindCustomerPO()
        {
            string DepartmentID = Session["DepartmentID"].ToString();
            DataTable dt = _cm.getCustomerPO(DepartmentID);
            if (dt.Rows.Count > 0)
            {
                gvCustomerPO.DataSource = dt;
                gvCustomerPO.DataBind();
            }
            else
            {
                gvCustomerPO.DataSource = null;
                gvCustomerPO.DataBind();
            }
        }

        protected void ddlQuotation_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void gvCustomerPO_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string quotationMasterID = e.CommandArgument.ToString();
            int index = Convert.ToInt32(((GridViewRow)((Control)e.CommandSource).NamingContainer).RowIndex);

            // Find FileUpload in that row
            GridViewRow row = gvCustomerPO.Rows[index];

            HiddenField hdDepartmentID = (HiddenField)row.FindControl("hdDepartmentID");

            string[] substrings = { "4011", "4012", "4013", "4014" };//If Enquiry belongs to Calibration Division
            bool containsAny = false;

            containsAny = substrings.Any(s => hdDepartmentID.Value.Contains(s));

            Button btnVerifyPO = (Button)row.FindControl("btnVerifyPO");
            Button btnPOVerified = (Button)row.FindControl("btnPOVerified");

            Button btnCalibAccept = (Button)row.FindControl("btnCalibAccept");
            Button btnCalibOnTime = (Button)row.FindControl("btnCalibOnTime");
            Button btnCalibDelay = (Button)row.FindControl("btnCalibDelay");
            Button btnCalibReject = (Button)row.FindControl("btnCalibReject");
            Button btnAllocate = (Button)row.FindControl("btnAllocate");
            Button btnGenerateMR = (Button)row.FindControl("btnGenerateMR");
            Button btnAddSubContractor = (Button)row.FindControl("btnAddSubContractor");

            TextBox txtDueDate = (TextBox)row.FindControl("txtDueDate");
            Label lblServiceDue = (Label)row.FindControl("lblServiceDue");

            DropDownList ddlSubContractors = (DropDownList)row.FindControl("ddlSubContractors");

            


            _cm.QuotationID = Convert.ToInt32(quotationMasterID);

            if (e.CommandName == "VerifyPO")
            {
                btnVerifyPO.Visible = false;
                txtDueDate.Visible = true;
                btnPOVerified.Visible = true;
                lblServiceDue.Visible = true;
            }
            if (e.CommandName == "VerifiedPO")
            {
                if (txtDueDate.Text != "")
                {
                    txtDueDate.BorderColor = Color.Empty;
                    _cm.DueDate = Convert.ToDateTime(txtDueDate.Text);
                    _cm.UpdateServiceDueDate(_cm);

                    _cm.QuotationStatusID = 25;//PO Verified - InquiryStatus                  
                    _cm.UpdateQuotationStatus(_cm);

                    if (containsAny)
                    {
                        _cm.QuotationStatusID = 34;//  PO Verified/ Calibration Scheduled
                        _cm.QuotationID = Convert.ToInt32(quotationMasterID);
                        _cm.UpdateQuotationCalibStatus(_cm);
                    }
                    BindCustomerPO();
                }
                else
                {
                    txtDueDate.BorderColor = Color.Red;
                }
            }
            if (e.CommandName == "AcceptForCalibration")
            {
                _cm.QuotationStatusID = 12;//  CRA Accepted by Lab
                _cm.QuotationID = Convert.ToInt32(quotationMasterID);
                _cm.UpdateQuotationCalibStatus(_cm);
                BindCustomerPO();
            }
            if (e.CommandName == "RejectForCalibration")
            {
                _cm.QuotationStatusID = 14;//  CRA Rejected by Lab
                _cm.QuotationID = Convert.ToInt32(quotationMasterID);
                _cm.UpdateQuotationCalibStatus(_cm);
                BindCustomerPO();
            }
            if (e.CommandName == "ToolsReceivedByCS")
            {             
                //BindCustomerPO();
                string url = "ToolsVerification.aspx?q=" + quotationMasterID;
                Response.Redirect(url);
            }
            if (e.CommandName == "SPK")
            {                
                string url = "ToolsVerification.aspx?q=" + quotationMasterID;
                Response.Redirect(url);
            }
            if (e.CommandName == "AcceptCalib")
            {
                btnCalibAccept.Visible = false;
                btnCalibOnTime.Visible = true;
                btnCalibDelay.Visible = true;
                btnCalibReject.Visible = false;
            }
            if (e.CommandName == "CalibOnTime")
            {
                _cm.QuotationStatusID = 37;//  Accepted by Calibration on Time
                _cm.QuotationID = Convert.ToInt32(quotationMasterID);
                _cm.UpdateQuotationCalibStatus(_cm);
                BindCustomerPO();
            }
            if (e.CommandName == "CalibDelay")
            {
                _cm.QuotationStatusID = 38;//  Accepted by Calibration Delay
                _cm.QuotationID = Convert.ToInt32(quotationMasterID);
                _cm.UpdateQuotationCalibStatus(_cm);
                BindCustomerPO();
            }
            if (e.CommandName == "RejectCalib")
            {
                _cm.QuotationStatusID = 39;// Rejected by Calibration
                _cm.QuotationID = Convert.ToInt32(quotationMasterID);
                _cm.UpdateQuotationCalibStatus(_cm);
                BindCustomerPO();
            }
            if (e.CommandName == "Allocate")
            {
                Response.Redirect("ToolsVerification.aspx?q=" + quotationMasterID);
                BindCustomerPO();
            }
            if (e.CommandName == "CalibCustomerDelayAccepted")
            {
                _cm.QuotationStatusID = 53;// Allocated for Calibration
                _cm.QuotationID = Convert.ToInt32(quotationMasterID);
                _cm.UpdateQuotationCalibStatus(_cm);
                BindCustomerPO();
            }
            if (e.CommandName == "CalibCustomerDelayRejected")
            {
                _cm.QuotationStatusID = 43;// Allocated for Calibration
                _cm.QuotationID = Convert.ToInt32(quotationMasterID);
                _cm.UpdateQuotationCalibStatus(_cm);
                BindCustomerPO();
            }
            if (e.CommandName == "CustomerRejectedSubContracting")
            {
                _cm.QuotationStatusID = 45;// Customer Rejection Due to Subcontracting
                _cm.QuotationID = Convert.ToInt32(quotationMasterID);
                _cm.UpdateQuotationCalibStatus(_cm);
                BindCustomerPO();
            }
            if (e.CommandName == "CustomerAcceptedSubContracting")
            {
                _cm.QuotationStatusID = 54;// Customer Acceptance of Sub Contracting
                _cm.QuotationID = Convert.ToInt32(quotationMasterID);
                _cm.UpdateQuotationCalibStatus(_cm);
                BindCustomerPO();
            }
            if (e.CommandName == "CalibResult")
            {
                string url = "ToolsVerification.aspx?q=" + quotationMasterID;
                Response.Redirect(url);
            }
            if (e.CommandName == "GenerateMR")
            {
                _cm.QuotationStatusID = 23;// MR Published
                _cm.QuotationID = Convert.ToInt32(quotationMasterID);
                _cm.UpdateQuotationCalibStatus(_cm);
                BindCustomerPO();
            }
            if (e.CommandName == "ReturnTool")
            {
                string url = "SampleReturn_TSQ_CAL_FM_028.aspx?q=" + quotationMasterID;
                Response.Redirect(url);

            }
            if (e.CommandName == "ChooseSubContractor")
            {
                ddlSubContractors.Visible = true;
                btnAddSubContractor.Visible = true;
            }
            if (e.CommandName == "AddSubContractor")
            {
                _cm.SubContractorID = Convert.ToInt32(ddlSubContractors.SelectedValue);
                _cm.QuotationID = Convert.ToInt32(quotationMasterID);
                _cm.UpdateSubContractor(_cm);
                _cm.QuotationStatusID = 55;// Assigned to Sub Contractor
                _cm.QuotationID = Convert.ToInt32(quotationMasterID);
                _cm.UpdateQuotationCalibStatus(_cm);
                BindCustomerPO();
            }
            if(e.CommandName == "SendImages")
            {
                DataTable dtQuotation = _cm.getQuotationCustomer(quotationMasterID);
                if (dtQuotation.Rows.Count > 0)
                {
                    Session["EnquiryID"] = dtQuotation.Rows[0]["EnquiryID"].ToString();
                    Response.Redirect("SampleImages.aspx");
                }
            }
        }
        protected void gvCustomerPO_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string quotationStatus = DataBinder.Eval(e.Row.DataItem, "QuotationStatus").ToString();

                PlaceHolder PlaceHolderDocuments = (e.Row.FindControl("PlaceHolderDocuments") as PlaceHolder);

                HiddenField hdDepartmentID = (HiddenField)e.Row.FindControl("hdDepartmentID");
                Button btnVerifyPO = (Button)e.Row.FindControl("btnVerifyPO");
                Button btnPOVerified = (Button)e.Row.FindControl("btnPOVerified");
                Button btnAcceptForCalibration = (Button)e.Row.FindControl("btnAcceptForCalibration");
                Button btnRejectForCalibration = (Button)e.Row.FindControl("btnRejectForCalibration");
                Button btnToolsReceivedByCS = (Button)e.Row.FindControl("btnToolsReceivedByCS");
                Button btnSPK = (Button)e.Row.FindControl("btnSPK");

                Button btnCalibAccept = (Button)e.Row.FindControl("btnCalibAccept");
                Button btnCalibOnTime = (Button)e.Row.FindControl("btnCalibOnTime");
                
                Button btnCalibDelay = (Button)e.Row.FindControl("btnCalibDelay");
                Button btnCalibDelayCustomerAccept = (Button)e.Row.FindControl("btnCalibDelayCustomerAccept");
                Button btnCalibDelayCustomerReject = (Button)e.Row.FindControl("btnCalibDelayCustomerReject");
                
                Button btnSubContractCustomerAccept = (Button)e.Row.FindControl("btnSubContractCustomerAccept");
                Button btnSubContractCustomerReject = (Button)e.Row.FindControl("btnSubContractCustomerReject");

                Button btnChooseSubContractor = (Button)e.Row.FindControl("btnChooseSubContractor");
                Button btnAddSubContractor = (Button)e.Row.FindControl("btnAddSubContractor");

                DropDownList ddlSubContractors = (DropDownList)e.Row.FindControl("ddlSubContractors");

                Button btnCalibReject = (Button)e.Row.FindControl("btnCalibReject");

                Button btnAllocate = (Button)e.Row.FindControl("btnAllocate");
                Button btnCalibResult = (Button)e.Row.FindControl("btnCalibResult");
                Button btnGenerateMR = (Button)e.Row.FindControl("btnGenerateMR");
                Button btnReturnTool = (Button)e.Row.FindControl("btnReturnTool");
                Button btnSendImages = (Button)e.Row.FindControl("btnSendImages");

                TextBox txtDueDate = (TextBox)e.Row.FindControl("txtDueDate");
                Label lblServiceDue = (Label)e.Row.FindControl("lblServiceDue");
                Label lblCalibrationStatusID = (Label)e.Row.FindControl("lblCalibrationStatusID");

                string[] substrings = { "4011", "4012", "4013", "4014" };//If Enquiry belongs to Calibration Division
                bool containsAny = false;

                containsAny = substrings.Any(s => hdDepartmentID.Value.Contains(s));

                string CalibrationStatus = lblCalibrationStatusID.Text;

                //Reset all control status
                btnVerifyPO.Visible = false;
                btnPOVerified.Visible = false;
                btnAcceptForCalibration.Visible = false;
                btnRejectForCalibration.Visible = false;
                btnToolsReceivedByCS.Visible = false;
                btnSPK.Visible = false;
                btnCalibAccept.Visible = false;
                btnCalibOnTime.Visible = false;
                btnCalibDelay.Visible = false;
                btnCalibDelayCustomerAccept.Visible = false;
                btnCalibDelayCustomerReject.Visible = false;
                btnSubContractCustomerAccept.Visible = false;
                btnSubContractCustomerReject.Visible = false;
                btnCalibReject.Visible = false;
                btnAllocate.Visible = false;
                btnCalibResult.Visible = false;
                btnGenerateMR.Visible = false;
                btnReturnTool.Visible = false;
                btnChooseSubContractor.Visible=false;
                btnAddSubContractor.Visible = false;
                ddlSubContractors.Visible = false;
                btnSendImages.Visible = false;

                txtDueDate.Visible = false;
                lblServiceDue.Visible = false;

                if (Session["RoleID"] != null)
                {
                    int RoleID = Convert.ToInt32(Session["RoleID"]);
                    switch (RoleID)
                    {
                        case 17://CS Employee
                            switch (CalibrationStatus)
                            {
                                case "11"://PO Received
                                    btnVerifyPO.Visible = true;
                                    txtDueDate.Visible = true;
                                    lblServiceDue.Visible = true;
                                    break;
                                case "12"://CRA Accepted by Lab
                                    btnToolsReceivedByCS.Visible = true;
                                    break;
                                case "22"://Tools Received by CS
                                    btnToolsReceivedByCS.Visible = true;
                                    break;
                                case "51"://Tools Received by CS
                                    btnSPK.Visible = true;
                                    break;
                                case "55"://Assigned to Sub Contractor
                                    btnSendImages.Visible = true;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case 19://Calibration Manager
                            switch (CalibrationStatus)
                            {
                                case "34"://PO Verified/Calibration Scheduled
                                    btnAcceptForCalibration.Visible = true;
                                    btnRejectForCalibration.Visible = true;
                                    break;
                                case "36"://SPK to Calibration Staff"
                                    btnCalibAccept.Visible = true;
                                    btnCalibReject.Visible=true;
                                    break;
                                case "37"://Accepted by Calibration on Time
                                case "53"://Customer Acceptance of Delay
                                    btnAllocate.Visible = true;
                                    break;
                                case "38"://Accepted by Calibration Delay
                                    btnCalibDelayCustomerAccept.Visible = true;
                                    btnCalibDelayCustomerReject.Visible = true;
                                    break;
                                case "41"://Calibration Result Submitted
                                    btnGenerateMR.Visible = true;
                                    break;
                                case "44"://Calibtration to be Subcontracted
                                    btnSubContractCustomerAccept.Visible = true;
                                    btnSubContractCustomerReject.Visible = true;
                                    break;
                                case "54"://Customer Acceptance of Sub Contracting
                                    ddlSubContractors.Visible = true;
                                    DataTable dtSubContractors = _cm.GetSubContractorList();
                                    if (dtSubContractors.Rows.Count > 0)
                                    {
                                        ddlSubContractors.DataSource = dtSubContractors;
                                        ddlSubContractors.DataValueField = "SubContractorID";
                                        ddlSubContractors.DataTextField = "SubContractorName";
                                        ddlSubContractors.DataBind();
                                        ddlSubContractors.Items.Insert(0, "Select Sub Contractor");
                                    }
                                    btnAddSubContractor.Visible = true;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case 20://Calib Employee
                            switch (CalibrationStatus)
                            {
                                case "40"://Allocated for Calibration
                                   btnCalibResult.Visible = true;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }

                }

            }
        }

        protected void btnAddSubContractor_Click(object sender, EventArgs e)
        {

        }
    }
}