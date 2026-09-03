using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;

namespace LIMS
{
    public partial class POMasterList : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        DAL.mailer objEmail = new DAL.mailer();
        string StoreType = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    //BindUserDetails(Session["UserID"].ToString());
                    Session["EmpID"] = Session["EmployeeID"].ToString();
                    BindPO();
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }
        protected void BindUserDetails(String UserID)
        {
            DataTable dt = _cm.GetEmployeeDetails(UserID);
            if (dt.Rows.Count > 0)
            {
                Session["EmployeeName"] = dt.Rows[0]["EmployeeName"].ToString();
                //Session["Unit"] = dt.Rows[0]["JobLocation"].ToString();
                Session["EmpID"] = dt.Rows[0]["EmployeeID"].ToString();
                Session["DepartmentID"] = dt.Rows[0]["DepartmentID"].ToString();
                
                if (Session["DepartmentID"].ToString() == "2008")
                {
                    StoreType = "Engineering";
                }
                //if (Session["DeptName"].ToString() == "RM Store")
                //{
                //    StoreType = "RM";
                //}
                dt = _cm.GetPOMasterDetails(StoreType);
                if (dt.Rows.Count > 0)
                {
                    gvPO.DataSource = dt;
                    gvPO.DataBind();
                }
                else
                {
                    gvPO.DataSource = null;
                    gvPO.DataBind();
                }
            }
        }



        private void BindPO()
        {
            DataTable dt = new DataTable();

            if (Session["DeptName"].ToString() == "Purchase")
            {
                StoreType = "Engineering";
            }
            if (Session["DeptName"].ToString() == "RM Store")
            {
                StoreType = "RM";
            }
            dt = _cm.GetPOMasterDetails(StoreType);
            if (dt.Rows.Count > 0)
            {
                gvPO.DataSource = dt;
                gvPO.DataBind();
            }
            else
            {
                gvPO.DataSource = null;
                gvPO.DataBind();
            }
        }
        
        protected void gvPO_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int rowIndex = int.Parse(e.CommandArgument.ToString());
            int POID = (int)this.gvPO.DataKeys[rowIndex]["PurchaseOrderID"];

            Label lblReportFileName = (Label)gvPO.Rows[rowIndex].FindControl("lblReportFileName");
            Label lblVendorID = (Label)gvPO.Rows[rowIndex].FindControl("lblVendorID");
            Label lblPONumber = (Label)gvPO.Rows[rowIndex].FindControl("lblPONumber");
            TextBox txtRemarks = (TextBox)gvPO.Rows[rowIndex].FindControl("txtRemarks");
            Label lblVendorName = (Label)gvPO.Rows[rowIndex].FindControl("lblVendorName");
            DropDownList ddlRevertType = (DropDownList)gvPO.Rows[rowIndex].FindControl("ddlRevertType");

            if (e.CommandName == "Approve")
            {
                _cm.PurchaseOrderID = POID;
                _cm.ApproveReject = "Approved";
                _cm.ApproveRejectDate = DateTime.Now;
                _cm.ApproveRejectBy = Convert.ToInt32(Session["EmployeeID"].ToString());
                _cm.StatusID = 6;//Approved
                _cm.UpdatePurchaseOrderStatus(_cm);//PO Status to Approved
                _cm.UpdatePOApproval(_cm);//PO Details Status to Approved
                _cm.UpdatePOApprovalDetails(_cm);
                Response.Redirect("GeneratePO.aspx?POID=" + POID + "&s=1");
                
            }
            if (e.CommandName == "Reject")
            {
                if ((ddlRevertType.SelectedIndex != 0) && (txtRemarks.Text != ""))
                {
                    txtRemarks.BorderColor = Color.Empty;

                    //_cm.StatusID = 7;
                    _cm.PurchaseOrderID = POID;
                    _cm.Reason = txtRemarks.Text;

                    if (Session["Usertype"].ToString() == "Director")
                    {
                        if (ddlRevertType.SelectedValue == "0")
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Select Enquiry Revert Type!');", true);
                        }
                        _cm.PurchaseOrderID = POID;
                        _cm.ApproveReject = "Rejected";
                        _cm.ApproveRejectDate = DateTime.Now;
                        _cm.ApproveRejectBy = Convert.ToInt32(Session["EmployeeID"].ToString());
                        _cm.UpdatePOApprovalDetails(_cm);

                        if (ddlRevertType.SelectedValue == "1")
                        {
                            _cm.StatusID = 7;//Rejected - Back to Quote Received
                            _cm.UpdateVendorQuotationDetails(_cm.PurchaseOrderID);//Reset VendorQuotation to 'Quote Received'
                            _cm.UpdatePORejectPRToQuoteAnalysis(_cm);//Remove PO# from PR
                            _cm.UpdateVendorQuotationStatusForPOReject(_cm);//Reset VendorQuotation to 'Quote Received'
                            _cm.UpdatePurchaseOrderEnquiryDetails(_cm);//Remove EnquiryDetailsID from PO
                            _cm.UpdatePurchaseOrderStatus(_cm);//PO Status to Rejected
                            _cm.UpdatePOApproval(_cm);//PO Details Status to Rejected
                            _cm.UpdateVendorQuotationinPOForPOReject(_cm);//Remove vendorquotationDetails & EnquiryDetails reference from PO
                            if (Session["EmployeeID"] != null)
                            {
                                _cm.UserID = Convert.ToInt32(Session["UserID"].ToString());
                                _cm.EmployeeID = Convert.ToInt32(Session["EmployeeID"].ToString());
                                _cm.Remarks = "PO Rejected - Back to Quote Received";
                                _cm.AddPOHistory(_cm);
                            }
                        }
                        if (ddlRevertType.SelectedValue == "2")
                        {
                            _cm.StatusID = 4;//Rejected - Back to Float Enquiry
                            _cm.UpdatePORejectPRToEnquiry(_cm); //Remove PO#,EnquiryID from PR   
                                                                // _cm.UpdatePORejectPRToQuoteAnalysis(_cm);//Remove PO# from PR                            
                            _cm.UpdatePurchaseOrderStatus(_cm);//PO Status to Rejected
                            _cm.UpdatePOApproval(_cm);//PO Details Status to Rejected
                            if (Session["EmployeeID"] != null)
                            {
                                _cm.UserID = Convert.ToInt32(Session["UserID"].ToString());
                                _cm.EmployeeID = Convert.ToInt32(Session["EmployeeID"].ToString());
                                _cm.Remarks = "PO Rejected - Back to Float Enquiry";
                                _cm.AddPOHistory(_cm);
                            }
                        }
                        if (ddlRevertType.SelectedValue == "3")
                        {
                            _cm.StatusID = 10;//Amend PO
                            _cm.UpdatePurchaseOrderStatus(_cm);
                            _cm.UpdatePOApproval(_cm);
                            if (Session["EmployeeID"] != null)
                            {
                                _cm.UserID = Convert.ToInt32(Session["UserID"].ToString());
                                _cm.EmployeeID = Convert.ToInt32(Session["EmployeeID"].ToString());
                                _cm.Remarks = "PO Rejected - Amend PO";
                                _cm.AddPOHistory(_cm);
                            }
                        }
                    }
                    _cm.UpdatePOIDInPR(_cm);
                    if (Session["UserID"] != null)
                    {
                        if (Session["EmployeeID"] != null)
                        {
                            _cm.UserID = Convert.ToInt32(Session["UserID"].ToString());
                            _cm.EmployeeID = Convert.ToInt32(Session["EmployeeID"].ToString());
                            _cm.Remarks = "PO Rejected Email Not Sent";
                            _cm.AddPOHistory(_cm);
                        }
                        else
                        {
                            Response.Redirect("Login.aspx");
                        }
                    }
                    else
                    {
                        Response.Redirect("Login.aspx");
                    }
                    BindPO();
                }
                else
                {
                    txtRemarks.BorderColor = Color.Red;
                    ddlRevertType.BorderColor = Color.Red;
                }
            }
        }

        protected void gvPO_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if ((Session["Usertype"] != null) && (Session["DeptName"] != null))
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    DropDownList ddlRevertType = (DropDownList)e.Row.FindControl("ddlRevertType");
                    Button btnApprove = (Button)e.Row.FindControl("btnApprove");
                    Button btnReject = (Button)e.Row.FindControl("btnReject");
                    Label lblPONumber = (Label)e.Row.FindControl("lblPONumber");
                    Label lblAcceptReason = (Label)e.Row.FindControl("lblAcceptReason");
                    Label lblPOStatusID = (Label)e.Row.FindControl("lblPOStatusID");

                    DataTable dt = _cm.getQuoteAcceptReason(lblPONumber.Text);
                    if (dt.Rows.Count > 0)
                    {
                        lblAcceptReason.Text = dt.Rows[0]["AcceptReason"].ToString();
                    }
                    string usertype = Session["Usertype"].ToString();
                    string dept = Session["DeptName"].ToString();
                    if (Session["Usertype"].ToString() == "Director")
                    {
                        gvPO.Columns[6].Visible = true;
                        gvPO.Columns[7].Visible = true;
                        gvPO.Columns[8].Visible = true;
                    }
                    else
                    {
                        gvPO.Columns[6].Visible = false;
                        gvPO.Columns[7].Visible = false;
                        gvPO.Columns[8].Visible = false;
                    }
                }
            }
        }

        protected void ddlRevertType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Reference the DropDownList.
            DropDownList ddlRevertType = sender as DropDownList;
            GridViewRow row = (GridViewRow)ddlRevertType.Parent.Parent;
            int idx = row.RowIndex;
            Button btnReject = (Button)gvPO.Rows[idx].FindControl("btnReject");
            //Get the ID of the DropDownList.
            string id = ddlRevertType.ID;

            if (ddlRevertType.SelectedIndex == 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Select Enquiry Revert Type!');", true);
                btnReject.Enabled = false;
            }
            else
            {
                btnReject.Enabled = true;
            }
        }


    }
}