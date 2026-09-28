using LIMS.DAL;
using LIMS.Models;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.Xml;
using System.Web;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;
using ZXing;

namespace LIMS
{
    public partial class PaymentRequisition : System.Web.UI.Page
    {
        string date = DateTime.Today.ToString("dd-MMM-yyyy");
        string time = DateTime.Now.ToString("h:mm:ss tt");
        string GetDateTime = DateTime.Today.ToString("dd-MMM-yyyy") + " " + DateTime.Now.ToString("h:mm:ss tt");
        decimal grandTotal = 0;

        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddDays(-1));
            Response.Cache.SetNoStore();

            if (Session["EmployeeName"] == null)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Session Timeout'); setTimeout(function() { window.location = 'Login.aspx'; }, 100); </script>");
            }
            else
            {
                if (!IsPostBack)
                {
                    Page.Form.Attributes.Add("enctype", "multipart/form-data");
                    if (Session["UserID"] != null)
                    {
                        BindCurrency();
                        BindUserDetails(Session["UserID"].ToString());
                        txtRequisitionDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                        txtRequisitionFrom.Text = Session["EmployeeName"].ToString();
                        GetRequisitionNumber();
                    }
                }
            }
        }
        protected void BindCurrency()
        {
            DataTable dt = _cm.getCurrency();
            if (dt.Rows.Count > 0)
            {
                ddlCurrency.DataSource = dt;
                ddlCurrency.DataTextField = "CurrencyCode";
                ddlCurrency.DataValueField = "CurrencyID";
                ddlCurrency.DataBind();
                ddlCurrency.Items.Insert(0, "Select Currency");
            }
        }
        protected void BindUserDetails(String UserID)
        {
            DataTable dt = _cm.GetEmpDetails(UserID);
            if (dt.Rows.Count > 0)
            {
                // Session["Unit"] = dt.Rows[0]["JobLocation"].ToString();
                Session["EmployeeID"] = dt.Rows[0]["EmployeeID"].ToString();
                Session["DepartmentID"] = dt.Rows[0]["DepartmentID"].ToString();
            }
        }
        protected void BindGrid()
        {
            DataTable dt = new DataTable();
            if (ViewState["DT"] != null)
            {
                dt = (DataTable)ViewState["DT"];
                gvPaymentRequisition.DataSource = dt;
                gvPaymentRequisition.DataBind();
            }
            else
            {
                if (dt.Rows.Count > 0)
                {
                    ViewState["DT"] = dt;
                    CheckViewState();
                    gvPaymentRequisition.DataSource = dt;
                    gvPaymentRequisition.DataBind();

                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No Records Found.'); setTimeout(function() { window.location = 'ItemInward.aspx'; }, 10);</script>");
                }
            }

        }
        protected void CheckViewState()
        {
            if (ViewState["DT"] != null)
            {
              
            }
            else
            {
                DataTable dt = new DataTable();
                DataColumnCollection columns = dt.Columns;
                if (!columns.Contains("ItemDescription"))
                {
                    dt.Columns.Add("ItemDescription", typeof(string));
                }
                if (!columns.Contains("Quantity"))
                {
                    dt.Columns.Add("Quantity", typeof(int));
                }
                if (!columns.Contains("UnitPrice"))
                {
                    dt.Columns.Add("UnitPrice", typeof(decimal));
                }
                if (!columns.Contains("TotalPrice"))
                {
                    dt.Columns.Add("TotalPrice", typeof(decimal));
                }
                if (!columns.Contains("RequirementDate"))
                {
                    dt.Columns.Add("RequirementDate", typeof(DateTime));
                }
                ViewState["DT"] = dt;
            }
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvPaymentRequisition.EditIndex = -1;
            BindGrid();
        }

        public void GetRequisitionNumber()
        {
            int DepartmentID = Convert.ToInt32(Session["DepartmentID"]);
            DataTable dt=_cm.GetEmployeeDepartment(DepartmentID);
            if (dt.Rows.Count > 0)
            {
                string DeptCode = dt.Rows[0]["DeptCode"].ToString();
                if (txtRequisitionDate.Text != "")
                {
                    DateTime ReceivedDate = Convert.ToDateTime(txtRequisitionDate.Text);
                    string PaymentRequisition_Number; string PaymentRequisitionYear = ""; int PaymentRequisitionCounter = 1;

                    PaymentRequisitionYear = ReceivedDate.ToString("yyyy");
                    DataTable dtLastPaymentRequisition = _cm.GetLastPaymentRequisition();
                    if (dtLastPaymentRequisition.Rows.Count > 0)
                    {
                        PaymentRequisitionCounter = dtLastPaymentRequisition.Rows.Count + 1;
                    }
                    else
                    {
                        PaymentRequisitionCounter = 1;
                    }
                    PaymentRequisition_Number = Convert.ToInt32(PaymentRequisitionCounter).ToString("D4") + "-" + Convert.ToDateTime(ReceivedDate).ToString("MM") + "-" + Convert.ToDateTime(ReceivedDate).ToString("yyyy") + "-PQI-PR-" + DeptCode;
                    txtRequisitionNumber.Text = PaymentRequisition_Number;
                    ViewState["PaymentRequisitionNumber"] = PaymentRequisition_Number;
                    ViewState["PaymentRequisitionCounter"] = PaymentRequisitionCounter;
                }
            }
           
        }
        protected void FileUpload(string RequisitionNumber)
        {
            int flag = 0;
            if (!fpFile.HasFile)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Notify", "alert('Select File');", true);
                fpFile.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else
            {
                int count = 1;
                fpFile.BorderColor = System.Drawing.Color.Empty;
                string folderPath = Server.MapPath("UploadedFiles/PaymentRequisitionDocs/");

                FileInfo fi = new FileInfo(fpFile.FileName);
                string ext = fi.Extension;

                fpFile.SaveAs(folderPath + Path.GetFileName(hdPaymentRequisitionID.Value + ext));
                _cm.PaymentRequisitionID = Convert.ToInt32(hdPaymentRequisitionID.Value);
                _cm.RequisitionNumber = RequisitionNumber;
                _cm.FileName = hdPaymentRequisitionID.Value + ext;
                _cm.DocumentName = txtDocumentName.Text;

                int ID = _cm.PaymentRequisitionFileUpload(_cm);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('File Upload Successful')", true);
            }
        }
        protected void btnSavePaymentRequest_Click(object sender, EventArgs e)
        {
            if (Session["Saving"] != null)
                return;

            int flag = 0;
            if (radPRType.SelectedIndex == -1)
            {
                flag = 1;
                radPRType.BackColor = Color.LightPink;
            }
            else
            {
                radPRType.BackColor = Color.Empty;
            }
           
            if (txtRequisitionNumber.Text == "")
            {
                flag = 1;
                txtRequisitionNumber.BorderColor = Color.Red;
            }
            else
            {
                txtRequisitionNumber.BorderColor = Color.Empty;
            }
            if (txtRequisitionDate.Text == "")
            {
                flag = 1;
                txtRequisitionDate.BorderColor = Color.Red;
            }
            else
            {
                txtRequisitionDate.BorderColor = Color.Empty;
            }
            if (ddlCurrency.SelectedIndex == 0)
            {
                flag = 1;
                ddlCurrency.BackColor = Color.LightPink;
            }
            else
            {
                ddlCurrency.BackColor = Color.Empty;
            }
            if (txtDocumentName.Text != "")
            {
                if (!fpFile.HasFile)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Notify", "alert('Select File');", true);
                    fpFile.BorderColor = System.Drawing.Color.Red;
                    flag = 1;
                }
            }
            if (flag == 0)
            {
                if (ViewState["DT"] != null)
                {
                    try
                    {
                        Session["Saving"] = true;
                        GetRequisitionNumber();
                        DataTable dt = (DataTable)ViewState["DT"];
                        _cm.RequisitionDate = Convert.ToDateTime(txtRequisitionDate.Text);
                        _cm.RequisitionBy = Session["EmployeeID"].ToString();
                        _cm.departmentID = Convert.ToInt32(Session["DepartmentID"].ToString());
                        _cm.RequisitionNumber = txtRequisitionNumber.Text;
                        _cm.RequisitionYear = Convert.ToDateTime(txtRequisitionDate.Text).ToString("yyyy");
                        _cm.RequisitionCount = Convert.ToInt32(ViewState["PaymentRequisitionCounter"]);
                        _cm.RequisitionStatus = 1;
                        _cm.TotalAmount = Convert.ToDecimal(hdGrandTotal.Value);
                        _cm.RequisitionType = radPRType.SelectedItem.ToString();
                        _cm.RequisitionSubType = "";
                        _cm.Remarks = txtRemarks.Text;
                        _cm.CurrencyID = Convert.ToInt32(ddlCurrency.SelectedValue.ToString());

                        int PaymentRequisitionID = _cm.AddPaymentRequisition(_cm);
                        hdPaymentRequisitionID.Value = PaymentRequisitionID.ToString();
                        if (txtDocumentName.Text != "")
                        {
                            FileUpload(_cm.RequisitionNumber);
                        }

                        _cm.PaymentRequisitionID = PaymentRequisitionID;
                        string EmployeeID = "";
                        DataTable dtReporting = new DataTable();
                        if (Session["EmployeeID"] != null)
                        {
                            EmployeeID = Session["EmployeeID"].ToString();
                            dtReporting = _cm.GetReportingTo(EmployeeID);
                            if (dtReporting.Rows.Count > 0)
                            {
                                if (dtReporting.Rows[0]["PID"].ToString() == "1")
                                {
                                    _cm.CurrentTier = 4;
                                    _cm.CurrentApproverRole = "Director";
                                }
                                else
                                {
                                    _cm.CurrentTier = 1;
                                    _cm.CurrentApproverRole = "Manager";
                                }
                                _cm.PaymentrequisitionSetApprover(_cm);
                            }

                            foreach (DataRow dr in dt.Rows)
                            {
                                _cm.PaymentRequisitionID = PaymentRequisitionID;
                                _cm.ItemDescription = dr["ItemDescription"].ToString();
                                _cm.Quantity = Convert.ToDecimal(dr["Quantity"].ToString());
                                _cm.unitprice = Convert.ToDecimal(dr["UnitPrice"].ToString());
                                _cm.totalprice = Convert.ToDecimal(dr["TotalPrice"].ToString());
                                _cm.RequirementDate = Convert.ToDateTime(dr["RequirementDate"].ToString());
                                _cm.PaymentRequisitionDetailsID = _cm.AddPaymentRequisitionDetails(_cm);

                                var User = new DAL.User();
                                User.EmployeeID = Session["EmployeeID"]?.ToString();
                                User.DepartmentID = Session["DepartmentID"]?.ToString();
                                User.UserID = Session["UserID"]?.ToString();
                                User.RoleID = Session["RoleID"]?.ToString();
                                User.Remarks = "New Payment Requisition";
                                User.PRStatus = "1";
                                User.PaymentRequisitionDetailsID = _cm.PaymentRequisitionDetailsID.ToString();
                                _cm.AddPaymentRequisitionHistory(User);
                            }

                            string s = _cm.Encode(_cm.PaymentRequisitionID.ToString());
                            string url = "GeneratePaymentRequisition.aspx?ID=" + s;

                            string script = @"
                                            window.open('" + url + @"', '_blank');
                                            window.location.href = 'PaymentRequisitionList.aspx';
                                        ";

                            ScriptManager.RegisterStartupScript(
                                this,
                                this.GetType(),
                                "OpenAndRedirect",
                                script,
                                true
                            );
                        }
                    }
                    finally
                    {
                        Session["Saving"] = null;
                    }
                    
                }
            }
        }


        protected void btnAdd_Click(object sender, EventArgs e)
        {
            int flag = 0;
            CheckViewState();
            DataTable dt = ViewState["DT"] as DataTable;

            if (txtItemDescription.Text == "")
            {
                txtItemDescription.BorderColor = Color.Red;
                flag = 1;
            }
            else
            {
                txtItemDescription.BorderColor = Color.Empty;
            }

            if (txtQuantity.Text == "")
            {
                txtQuantity.BorderColor = Color.Red;
                flag = 1;
            }
            else
            {
                txtQuantity.BorderColor = Color.Empty;
            }
            if (txtUnitPrice.Text == "")
            {
                txtUnitPrice.BorderColor = Color.Red;
                flag = 1;
            }
            else
            {
                txtUnitPrice.BorderColor = Color.Empty;
            }
            if (txtTotalPrice.Text == "")
            {
                txtTotalPrice.BorderColor = Color.Red;
                flag = 1;
            }
            else
            {
                txtTotalPrice.BorderColor = Color.Empty;
            }
            if (txtRequirementDate.Text == "")
            {
                txtRequirementDate.BorderColor = Color.Red;
                flag = 1;
            }
            else
            {
                txtRequirementDate.BorderColor = Color.Empty;
            }
            if (flag == 0)
            {
                DataRow dr = dt.NewRow();
                dr["ItemDescription"] = txtItemDescription.Text;
                dr["Quantity"] = txtQuantity.Text;
                dr["UnitPrice"] = txtUnitPrice.Text;
                dr["TotalPrice"] = txtTotalPrice.Text;
                dr["RequirementDate"] = txtRequirementDate.Text;

                dt.Rows.Add(dr);   // <-- Always inserts at last row
                Clear();
                ViewState["DT"] = dt;
            }
          
            gvPaymentRequisition.DataSource = dt;
            gvPaymentRequisition.DataBind();
        }
        protected void Clear()
        {
            txtItemDescription.Text = "";
            txtQuantity.Text = "";
            txtUnitPrice.Text = "";
            txtTotalPrice.Text = "";
            txtRequirementDate.Text = "";
        }
        protected void gvPaymentRequisition_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Get column value
                decimal amount = Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "TotalPrice"));
                grandTotal += amount;
                hdGrandTotal.Value = grandTotal.ToString();
            }

            // Assign total to footer
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[4].Text = "TOTAL";  // footer label
                e.Row.Cells[4].Font.Bold = true;

                e.Row.Cells[5].Text = grandTotal.ToString("N2"); // total value
                e.Row.Cells[5].Font.Bold = true;
            }
        }

        protected void Calculate()
        {
            int qty = 0; decimal unitPrice = 0;decimal totalPrice = 0;
            if (txtQuantity.Text != "" && txtUnitPrice.Text != "")
            {
                totalPrice = Convert.ToDecimal(txtQuantity.Text)*Convert.ToDecimal(txtUnitPrice.Text);
                txtTotalPrice.Text=totalPrice.ToString();
            }

        }

        protected void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            Calculate();
        }

        protected void txtUnitPrice_TextChanged(object sender, EventArgs e)
        {
            Calculate();
        }
    }
}