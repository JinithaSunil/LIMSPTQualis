using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class CustomerFMN_LSSM : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["ID"] != null)
                {
                    string InquiryID = Request.QueryString["ID"].ToString();
                    Session["OrganizationID"] = 1;
                    Session["InquiryID"] = InquiryID;
                    lblInquiry.Text = "ENQ" + InquiryID;
                    hdInquiryId.Value = InquiryId.ToString();
                    DataTable dt = _cm.BindInquiryCustomer(InquiryId);
                    if (dt.Rows.Count > 0)
                    {
                        txtDateOfRequest.Text = dt.Rows[0]["Created"].ToString();
                        txtCustomer.Text = dt.Rows[0]["CustomerName"].ToString();
                        txtVAT.Text = dt.Rows[0]["VAT"].ToString();
                        txtAddressLine1.Text = dt.Rows[0]["AddressLine1"].ToString();
                        txtAddressLine2.Text = dt.Rows[0]["AddressLine2"].ToString() + "," + dt.Rows[0]["State"].ToString();
                        txtPincode.Text = dt.Rows[0]["Pincode"].ToString();
                        txtCountry.Text = dt.Rows[0]["Country"].ToString();
                        Session["CustomerId"] = dt.Rows[0]["CustomerId"].ToString();
                        hdCustomerMasterID.Value = dt.Rows[0]["CustomerMasterId"].ToString();
                    }
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }
        protected void ddlContactPerson_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        protected void btnSaveDraft_Click(object sender, EventArgs e)
        {

        }

        protected void btnNext1_Click(object sender, EventArgs e)
        {

        }

        
    }
}