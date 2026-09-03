using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using LIMS.DAL;
using static iTextSharp.text.pdf.PRTokeniser;

namespace LIMS
{
    public partial class ToolsVerification : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindInstruments();
            }
        }
        protected void BindInstruments()
        {
            if (Request.QueryString["q"] != null)
            {
                hdQuotationID.Value = Request.QueryString["q"].ToString();
                DataTable dtCalibRequest = _cm.GetCalibrationRequest(hdQuotationID.Value);
                if (dtCalibRequest.Rows.Count > 0)
                {
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

        protected void gvInstruments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string RequestID = e.CommandArgument.ToString();
            int QuotationID = Convert.ToInt32(hdQuotationID.Value);
            int index = Convert.ToInt32(((GridViewRow)((Control)e.CommandSource).NamingContainer).RowIndex);
            _cm.RequestID = Convert.ToInt32(RequestID);
            // Find FileUpload in that row
            GridViewRow row = gvInstruments.Rows[index];
            Button btnVerify = (Button)row.FindControl("btnVerify");
            if (e.CommandName == "Verify")
            {
                string url = "ToolReceiptRecord-TSQ-CAL-FM-003.aspx?r=" + RequestID + "&q=" + QuotationID;
                Response.Redirect(url);
            }
            if (e.CommandName == "SPK")
            {
                string url = "Work-Order-TSQ-CAL-FM-002-SPK.aspx?r=" + RequestID + "&q=" + QuotationID;
                Response.Redirect(url);
            }
            if (e.CommandName == "CalibResult")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);

                Panel pnlReport = (Panel)row.FindControl("pnlReport");
                Repeater rptReports = (Repeater)row.FindControl("rptReports");

                // Example: fetch report list using row key
                DataTable dtReports = GetReports(RequestID);

                rptReports.DataSource = dtReports;
                rptReports.DataBind();

                pnlReport.Visible = true;
            }
            if (e.CommandName == "Allocate")
            {
                _cm.calibrationStatusID = 40;
                _cm.QuotationID = Convert.ToInt32(QuotationID);
                _cm.updateToolStatus(_cm);
                _cm.UpdateRequestStatus(_cm);

                DataTable dtComplete = _cm.getToolStatus(40);
                if (dtComplete.Rows.Count == 0)
                {
                    _cm.QuotationStatusID = 40;// Allocated for Calibration  
                    _cm.UpdateQuotationCalibStatus(_cm);
                }
                BindInstruments();
            }
        }
        private DataTable GetReports(string orderNo)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ReportName");

            dt.Rows.Add("Invoice.pdf");
            dt.Rows.Add("TestReport.pdf");
            dt.Rows.Add("DispatchNote.pdf");

            return dt;
        }
        protected void gvInstruments_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                HiddenField hdRequestID = (HiddenField)e.Row.FindControl("hdRequestID");
                Label lblStatus = (Label)e.Row.FindControl("lblStatus");
                Button btnVerify = (Button)e.Row.FindControl("btnVerify");
                Button btnSPK = (Button)e.Row.FindControl("btnSPK");
                Button btnAllocate = (Button)e.Row.FindControl("btnAllocate");
                HiddenField hdToolStatus = (HiddenField)e.Row.FindControl("hdToolStatus");
                HiddenField hdInstrumentID = (HiddenField)e.Row.FindControl("hdInstrumentID");
                Panel pnlReport = (Panel)e.Row.FindControl("pnlReport");
                
                // 2️⃣ Find the Repeater inside the Panel



                if (Session["RoleID"] != null)
                {
                    int RoleID = Convert.ToInt32(Session["RoleID"]);
                    if (hdToolStatus != null)
                    {
                        switch (hdToolStatus.Value)
                        {
                            case "22":// Tools Received by CS
                                break;
                            case "51":// Tools Accepted by CS
                                btnSPK.Visible = true;
                                break;
                            case "36"://SPK to Calibration Staff (WO)
                                if (RoleID == 19)//Calib Manager
                                {
                                    btnAllocate.Visible = true;
                                }
                                break;
                            case "40"://Allocated for Calibration
                                if (RoleID == 20)//Calib Employee
                                {
                                    if (pnlReport != null)
                                    {
                                        if (hdInstrumentID != null)
                                        {
                                            DataTable dtReports = _cm.getReportList(hdInstrumentID.Value);
                                            if (dtReports.Rows.Count > 0)
                                            {
                                                if (dtReports.Rows[0]["ReportList"].ToString() != "")
                                                {
                                                    DataTable dtReportNames = _cm.getStandardTests(dtReports.Rows[0]["ReportList"].ToString());
                                                    if (dtReportNames.Rows.Count > 0)
                                                    {
                                                        Repeater rptReports = pnlReport.FindControl("rptReports") as Repeater;
                                                        if (rptReports != null)
                                                        {
                                                            rptReports.DataSource = dtReportNames;
                                                            rptReports.DataBind();
                                                            pnlReport.Visible = true;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
                DataTable dt = _cm.GetCalibrationRequest(hdQuotationID.Value, hdRequestID.Value);
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["SampleID"].ToString() == "")
                    {
                        if (Session["RoleID"] != null)
                        {
                            int RoleID = Convert.ToInt32(Session["RoleID"]);
                            switch (RoleID)
                            {
                                case 17://CS Employee
                                    if (dt.Rows[0]["VerificationStatus"].ToString() == "1")
                                    {
                                        lblStatus.Text = "Verified";
                                        btnVerify.Visible = false;
                                        btnSPK.Visible = true;
                                    }
                                    else if (dt.Rows[0]["VerificationStatus"].ToString() == "0")
                                    {
                                        lblStatus.Text = "Rejected";
                                        btnVerify.Visible = false;
                                        btnSPK.Visible = false;
                                    }
                                    else
                                    {
                                        lblStatus.Text = "";
                                        btnVerify.Visible = true;
                                        btnSPK.Visible = false;
                                    }
                                    break;
                                default:
                                    if (dt.Rows[0]["ID"].ToString() == "")
                                    {
                                        lblStatus.Text = "";
                                        btnVerify.Visible = true;
                                        btnSPK.Visible = false;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }

        protected void rptReports_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "CalibResult")
            { 
                // Repeater
                Repeater rpt = (Repeater)source;

                // GridViewRow that contains the Repeater
                GridViewRow gvRow = (GridViewRow)rpt.NamingContainer;

                // GridView
                GridView gv = (GridView)gvRow.NamingContainer;

                // DataKey value
                int requestId = Convert.ToInt32(
                    gv.DataKeys[gvRow.RowIndex].Value
                );

                int SampleID = 0, ReportID = 0;
                ReportID = Convert.ToInt32(e.CommandArgument);
                DataTable dt = _cm.GetCalibrationSample(requestId);
                if (dt.Rows.Count > 0)
                {
                    SampleID = Convert.ToInt32(dt.Rows[0]["ID"].ToString());
                    string url = "CalibrationRecord.aspx?r=" + ReportID + "&s=" + SampleID;

                    Response.Redirect(url);
                }
            }
        }
    }
}