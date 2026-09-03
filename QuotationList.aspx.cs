using ClosedXML.Excel;
using LIMS.DAL;
using LIMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class QuotationList : System.Web.UI.Page
    {
        //SqlConnection _sCon = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString);
        //string CM = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
        string date = DateTime.Today.ToString("dd-MMM-yyyy");
        string time = DateTime.Now.ToString("h:mm:ss tt");
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindQuotation();
            }
        }
        protected void BindQuotation()
        {
            DataTable dt = _cm.getQuotationNEW();
            if (dt.Rows.Count > 0)
            {
                ViewState["GridData"] = dt;
                lblRecords.Text = dt.Rows.Count.ToString();
                gvQuotation.DataSource = dt;
                gvQuotation.DataBind();
            }
            else
            {
                ViewState["GridData"] = null;
                gvQuotation.DataSource = null;
                gvQuotation.DataBind();
            }
        }
        protected void BindQuotationFilter(DAL.CM _cm)
        {
            DataTable dt = _cm.getQuotationFilter(_cm);
            if (dt.Rows.Count > 0)
            {
                ViewState["GridData"] = dt;
                lblRecords.Text = dt.Rows.Count.ToString();
                gvQuotation.DataSource = dt;
                gvQuotation.DataBind();
            }
            else
            {
                ViewState["GridData"] = null;
                gvQuotation.DataSource = null;
                gvQuotation.DataBind();
            }
            if (gvQuotation.HeaderRow != null)
            {
                (gvQuotation.HeaderRow.FindControl("txtCustomerName") as TextBox).Text = ViewState["customerName"] as string ?? "";
                (gvQuotation.HeaderRow.FindControl("txtQuotationNumber") as TextBox).Text = ViewState["QuotationNumber"] as string ?? "";
                (gvQuotation.HeaderRow.FindControl("txtDate") as TextBox).Text = ViewState["Date"] as string ?? "";
                (gvQuotation.HeaderRow.FindControl("txtQuotationStatus") as TextBox).Text = ViewState["QuotationStatus"] as string ?? "";
            }

        }
        protected void gvQuotation_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            // Set the new page index
            gvQuotation.PageIndex = e.NewPageIndex;
            // Re-bind the GridView with the same data source
            BindQuotation();
        }

        protected void gvQuotation_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (Session["DepartmentID"] != null)
            {
                int DepartmentID = Convert.ToInt32(Session["DepartmentID"].ToString());
                // Check if the row is a data row (not header or footer)
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Get the value of QuotationStatus for this row
                    string quotationStatus = DataBinder.Eval(e.Row.DataItem, "QuotationStatus").ToString();

                    // Find the InvoiceGenerate button (lnkInvoice) in the current row
                    LinkButton lnkEdit = (LinkButton)e.Row.FindControl("lnkEdit");
                    LinkButton lnkSendQuote = (LinkButton)e.Row.FindControl("lnkSendQuote");
                    LinkButton lnkAcceptQuote = (LinkButton)e.Row.FindControl("lnkAcceptQuote");
                    LinkButton lnkRejectQuote = (LinkButton)e.Row.FindControl("lnkRejectQuote");
                    LinkButton lnkCoverLetter = (LinkButton)e.Row.FindControl("lnkCoverLetter");

                    //PO controls start
                    FileUpload fpPO = (FileUpload)e.Row.FindControl("fpPO");
                    TextBox txtPONo = (TextBox)e.Row.FindControl("txtPONo");
                    TextBox txtPODate = (TextBox)e.Row.FindControl("txtPODate");
                    Button btnUploadRow = (Button)e.Row.FindControl("btnUploadRow");
                    //PO controls end
                    LinkButton lnkSOgenerate = (LinkButton)e.Row.FindControl("lnkSOgenerate");
                    LinkButton lnkInvoice = (LinkButton)e.Row.FindControl("lnkInvoice");

                    LinkButton lnkShowSO = (LinkButton)e.Row.FindControl("lnkShowSO");
                    LinkButton lnkProformaInvoice = (LinkButton)e.Row.FindControl("lnkProformaInvoice");
                    LinkButton lnkShowInvoice = (LinkButton)e.Row.FindControl("lnkShowInvoice");
                    HyperLink hypInquiry = (HyperLink)e.Row.FindControl("hypInquiry");

                    HiddenField hdQuotationMasterID = (HiddenField)e.Row.FindControl("hdQuotationMasterID");
                    HiddenField hdRevisionCount = (HiddenField)e.Row.FindControl("hdRevisionCount");
                    HiddenField hdDepartmentID = (HiddenField)e.Row.FindControl("hdDepartmentID");

                    // If the QuotationStatus is "Approved", show the button, else hide it
                    string[] substrings = { "4011", "4012", "4013", "4014" };//If Enquiry belongs to Calibration Division
                    bool containsAny = false;

                    containsAny = substrings.Any(s => hdDepartmentID.Value.Contains(s));

                    if (containsAny)
                    {
                        hypInquiry.NavigateUrl = "GenerateQuotationCalib.aspx?ID=" + hdQuotationMasterID.Value + "&r=" + hdRevisionCount.Value;
                    }
                    else
                    {
                        hypInquiry.NavigateUrl = "GenerateQuotation.aspx?ID=" + hdQuotationMasterID.Value + "&r=" + hdRevisionCount.Value;
                    }
                    lnkEdit.Visible = false;
                    lnkSendQuote.Visible = false;
                    lnkAcceptQuote.Visible = false;
                    lnkRejectQuote.Visible = false;
                    fpPO.Visible = false;
                    txtPONo.Visible = false;
                    txtPODate.Visible = false;
                    btnUploadRow.Visible = false;
                    lnkSOgenerate.Visible = false;
                    lnkInvoice.Visible = false;
                    lnkShowSO.Visible = false;
                    lnkShowInvoice.Visible = false;
                    if (quotationStatus == "Revised Quote" || quotationStatus == "Quotation New")
                    {
                        lnkEdit.Visible = true;
                        lnkSendQuote.Visible = true;
                    }
                    else if (quotationStatus == "Quotation Sent")
                    {
                        lnkAcceptQuote.Visible = true;
                        lnkRejectQuote.Visible = true;
                    }
                    else if (quotationStatus == "Quotation Accepted")
                    {
                        fpPO.Visible = true;
                        txtPONo.Visible = true;
                        txtPODate.Visible = true;
                        btnUploadRow.Visible = true;
                    }
                    if (quotationStatus == "Accepted")
                    {
                        lnkEdit.Visible = false;
                        lnkSendQuote.Visible = false;
                        lnkAcceptQuote.Visible = false;
                        lnkRejectQuote.Visible = false;
                        fpPO.Visible = false;
                        txtPONo.Visible = false;
                        txtPODate.Visible = false;
                        btnUploadRow.Visible = false;
                        lnkSOgenerate.Visible = false;
                        lnkInvoice.Visible = false;
                        lnkShowSO.Visible = false;
                        lnkShowInvoice.Visible = false;
                        if (DepartmentID == 4015)
                        {
                            lnkProformaInvoice.Visible = true;
                            lnkSOgenerate.Visible = true;
                        }
                    }
                    else if (quotationStatus == "Revised Quote" || quotationStatus == "New")
                    {
                        lnkEdit.Visible = true;
                        lnkInvoice.Visible = false;
                        lnkShowSO.Visible = false;
                        lnkSendQuote.Visible = true;
                        lnkShowInvoice.Visible = false;
                        if (DepartmentID == 4015)
                        {
                            lnkSOgenerate.Visible = true;
                        }
                    }
                    else if (quotationStatus == "Quotation Accepted")
                    {
                        fpPO.Visible = true;
                        txtPONo.Visible = true;
                        txtPODate.Visible = true;
                        btnUploadRow.Visible = true;
                        lnkSOgenerate.Visible = true;
                        lnkInvoice.Visible = true;
                    }
                    else if (quotationStatus == "PO Received")
                    {
                        lnkSOgenerate.Visible = true;
                        lnkInvoice.Visible = true;
                    }
                    else if (quotationStatus == "Sales Order Generated")
                    {
                        lnkShowSO.Visible = true;
                        lnkProformaInvoice.Visible = true;
                        lnkInvoice.Visible = true;
                        lnkEdit.Visible = false;
                        lnkSOgenerate.Visible = false;
                        lnkShowInvoice.Visible = false;                        
                    }
                    else if (quotationStatus == "Invoice Generated")
                    {
                        lnkEdit.Visible = false;
                        lnkSOgenerate.Visible = false;
                        lnkInvoice.Visible = false;
                        lnkShowSO.Visible = false;
                        lnkShowInvoice.Visible = true;
                        if (DepartmentID == 4015)
                        {
                            lnkProformaInvoice.Visible = true;
                            lnkCoverLetter.Visible = true;
                        }
                    }
                    else
                    {
                        lnkEdit.Visible = true;
                        lnkSOgenerate.Visible = false;
                        lnkInvoice.Visible = false;
                        lnkShowSO.Visible = false;
                        lnkShowInvoice.Visible = false;
                    }

                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }

        }


        protected void gvQuotation_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Page")
                return;
            string quotationMasterID = e.CommandArgument.ToString();
            int index = Convert.ToInt32(((GridViewRow)((Control)e.CommandSource).NamingContainer).RowIndex);

            // Find FileUpload in that row
            GridViewRow row = gvQuotation.Rows[index];
            HiddenField hdDepartmentID = (HiddenField)row.FindControl("hdDepartmentID");

            string[] substrings = { "4011", "4012", "4013", "4014" };//If Enquiry belongs to Calibration Division
            bool containsAny = false;

            containsAny = substrings.Any(s => hdDepartmentID.Value.Contains(s));
            if (e.CommandName == "Filter")
            {
                TextBox txtCustomerName = (TextBox)gvQuotation.HeaderRow.FindControl("txtCustomerName");
                TextBox txtQuotationNumber = (TextBox)gvQuotation.HeaderRow.FindControl("txtQuotationNumber");
                TextBox txtDate = (TextBox)gvQuotation.HeaderRow.FindControl("txtDate");
                TextBox txtQuotationStatus = (TextBox)gvQuotation.HeaderRow.FindControl("txtQuotationStatus");

                _cm.customerName = txtCustomerName.Text.Trim();
                _cm.QuotationNumber = txtQuotationNumber.Text.Trim();
                if (txtDate.Text.Trim() != "")
                {
                    _cm.Date = Convert.ToDateTime(txtDate.Text.Trim());
                }
                _cm.QuotationStatus = txtQuotationStatus.Text.Trim();

                BindQuotationFilter(_cm);
            }

            if (e.CommandName == "ClearFilter")
            {
                GridViewRow currentRow = (GridViewRow)(((Control)e.CommandSource).NamingContainer);

                // Find a TextBox named "txtData" within the current row
                TextBox txtCustomerName = (TextBox)currentRow.FindControl("txtCustomerName");
                TextBox txtQuotationNumber = (TextBox)currentRow.FindControl("txtQuotationNumber");
                TextBox txtDate = (TextBox)currentRow.FindControl("txtDate");
                TextBox txtQuotationStatus = (TextBox)currentRow.FindControl("txtQuotationStatus");
                txtCustomerName.Text = "";
                txtQuotationNumber.Text = "";
                txtDate.Text = "";
                txtQuotationStatus.Text = "";
                BindQuotation();
            }

            if (e.CommandName == "Edit")
            {
                Response.Redirect("QuotationEditPage.aspx?QuotationMasterID=" + quotationMasterID);
            }
            if (e.CommandName == "SendQuote")
            {
                _cm.QuotationStatusID = 5;//Quotation Sent
                _cm.QuotationID = Convert.ToInt32(quotationMasterID);
                _cm.UpdateQuotationStatus(_cm);
                if (containsAny)
                {
                    _cm.QuotationStatusID = 7;//Quotation Sent
                    _cm.QuotationID = Convert.ToInt32(quotationMasterID);
                    _cm.UpdateQuotationCalibStatus(_cm);
                }

            }

            if (e.CommandName == "QuoteAccepted")
            {
                _cm.QuotationStatusID = 3;//Quotation Accepted
                _cm.QuotationID = Convert.ToInt32(quotationMasterID);
                _cm.UpdateQuotationStatus(_cm);
                if (containsAny)
                {
                    _cm.QuotationStatusID = 8;//Quotation Accepted
                    _cm.QuotationID = Convert.ToInt32(quotationMasterID);
                    _cm.UpdateQuotationCalibStatus(_cm);
                }

            }
            if (e.CommandName == "QuoteRejected")
            {
                _cm.QuotationStatusID = 4;//Quotation Rejected
                _cm.QuotationID = Convert.ToInt32(quotationMasterID);
                _cm.UpdateQuotationStatus(_cm);
                if (containsAny)
                {
                    _cm.QuotationStatusID = 9;//Quotation Rejected
                    _cm.QuotationID = Convert.ToInt32(quotationMasterID);
                    _cm.UpdateQuotationCalibStatus(_cm);
                }

            }
            if (e.CommandName == "POReceived")
            {
                FileUpload fpPO = (FileUpload)row.FindControl("fpPO");
                TextBox txtPONo = (TextBox)row.FindControl("txtPONo");
                TextBox txtPODate = (TextBox)row.FindControl("txtPODate");

                if (fpPO != null && fpPO.HasFile)
                {
                    if ((txtPONo.Text == "") || (txtPODate.Text == ""))
                    {
                        txtPONo.BorderColor = Color.Red; txtPODate.BorderColor = Color.Red;
                        Response.Write($"<script>alert('Enter PO details');</script>");
                    }
                    else
                    {
                        txtPONo.BorderColor = Color.Empty; txtPODate.BorderColor = Color.Empty;

                        string fileName = fpPO.FileName; // original file name
                        string extension = System.IO.Path.GetExtension(fileName); // includes the dot, e.g. ".pdf"

                        fileName = "PO-" + quotationMasterID + extension;  //Path.GetFileName(fpPO.PostedFile.FileName);
                        string savePath = Server.MapPath("~/CustomerPO/" + fileName);

                        fpPO.SaveAs(savePath);
                        _cm.QuotationStatusID = 7;//PO Received
                        _cm.QuotationID = Convert.ToInt32(quotationMasterID);
                        _cm.UpdateQuotationStatus(_cm);
                        if (containsAny)
                        {
                            _cm.QuotationStatusID = 11;//PO Received
                            _cm.QuotationID = Convert.ToInt32(quotationMasterID);
                            _cm.UpdateQuotationCalibStatus(_cm);
                        }

                        _cm.PONumber = txtPONo.Text;
                        _cm.PODate = Convert.ToDateTime(txtPODate.Text);
                        _cm.POFile = fileName;
                        _cm.UpdateQuotationPO(_cm);

                        Response.Write($"<script>alert('File {fileName} uploaded successfully');</script>");
                    }


                }
                else
                {
                    Response.Write("<script>alert('Please select a file to upload.');</script>");
                }
            }


            if (e.CommandName == "SalesOrderGenerate")
            {
                string query = "SELECT SalesOrderNumber FROM QuotationMaster WHERE QuotationMasterID = @QuotationMasterID";
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@QuotationMasterID", quotationMasterID);

                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    conn.Close();

                    if (result == null || result.ToString().Trim() == "")
                    {

                        string newSalesOrderNumber = GenerateSalesOrderNumber(); // Generate new Sales Order Number
                        int QuotationStatusID = 27;//Sales Order Generated
                        UpdateSalesOrder(quotationMasterID, newSalesOrderNumber, QuotationStatusID); // Update the QuotationMaster with new order number and status
                        if (containsAny)
                        {
                            _cm.QuotationStatusID = 49;//Sales Order Generated
                            _cm.QuotationID = Convert.ToInt32(quotationMasterID);
                            _cm.UpdateQuotationCalibStatus(_cm);
                        }
                        ClientScript.RegisterStartupScript(this.GetType(), "OpenNewTab", "window.open('GenerateSalesOrder.aspx?ID=" + quotationMasterID + "', '_blank');", true);


                        // Response.Redirect("GenerateSalesOrder.aspx?ID=" + quotationMasterID); //
                    }
                    else
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Something went wrong!');</script>");
                    }

                }

            }

            if (e.CommandName == "SOShow")
            {

                //string fileOpen = Server.MapPath("/SalesOrder/SO-" + quotationMasterID + ".htm");
                // Generate the file path
                string fileOpen = "/SalesOrder/SO-" + quotationMasterID + ".htm";

                // Register the JavaScript to open the file in a new tab
                string script = $"window.open('{fileOpen}', '_blank');";
                ClientScript.RegisterStartupScript(this.GetType(), "OpenFile", script, true);

            }

            if (e.CommandName == "InvoiceGenerate")
            {
                Control ctl = e.CommandSource as Control;
                GridViewRow currentRow = ctl.NamingContainer as GridViewRow;
                int rowIndex = currentRow.RowIndex;
                int RevisionCount = Convert.ToInt32(gvQuotation.DataKeys[rowIndex].Values["RevisionCount"]);
                string url = "InvoiceGenerate.aspx?ID=" + quotationMasterID + "&c=" + RevisionCount.ToString();
                Response.Redirect(url);

            }

            if (e.CommandName == "InvoiceShow")
            {

                // Generate the file path
                string fileOpen = "/Invoice/INV-" + quotationMasterID + ".htm";

                // Register the JavaScript to open the file in a new tab
                string script = $"window.open('{fileOpen}', '_blank');";
                ClientScript.RegisterStartupScript(this.GetType(), "OpenFile", script, true);
            }
            if (e.CommandName == "ProformaInvoice")
            {
                Response.Redirect("GenerateProformaInvoice.aspx?ID=" + quotationMasterID);
            }
            if (e.CommandName == "CoverLetter")
            {
                Response.Redirect("GenerateCoverLetter.aspx?ID=" + quotationMasterID);
            }
            BindQuotation();
        }



        private string GenerateSalesOrderNumber()
        {
            string newSalesOrderNumber = "SO-00001"; // Default order number in case no orders exist

            // Get the last SalesOrderNumber from the database
            string query = @"SELECT COALESCE(MAX(CAST(SUBSTRING(SalesOrderNumber, PATINDEX('%[0-9]%', SalesOrderNumber), LEN(SalesOrderNumber)) AS INT)), 0) AS LastOrderNumber 
                             FROM QuotationMaster WHERE SalesOrderNumber LIKE 'SO%'";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                object result = cmd.ExecuteScalar();
                // Check if result is not DBNull.Value and handle the result
                if (result != DBNull.Value)
                {
                    int lastOrderNumber = Convert.ToInt32(result);
                    newSalesOrderNumber = "SO-" + (lastOrderNumber + 1).ToString("D5"); // Increment last order number
                }
            }

            return newSalesOrderNumber;
        }

        private void UpdateSalesOrder(object quotationMasterID, string newSalesOrderNumber, int QuotationStatusId)
        {
            string updateQuery = @"UPDATE QuotationMaster SET SalesOrderNumber = @SalesOrderNumber, SalesOrderDate = @SalesOrderDate, 
                                                              QuotationStatusId = @QuotationStatusId, UpdatedDate = @UpdatedDate
                                    WHERE QuotationMasterID = @QuotationMasterID";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(updateQuery, conn);
                cmd.Parameters.AddWithValue("@SalesOrderNumber", newSalesOrderNumber);
                cmd.Parameters.AddWithValue("@SalesOrderDate", date + " " + time);
                cmd.Parameters.AddWithValue("@QuotationStatusId", QuotationStatusId);
                cmd.Parameters.AddWithValue("@UpdatedDate", date + " " + time);
                cmd.Parameters.AddWithValue("@QuotationMasterID", quotationMasterID);

                cmd.ExecuteNonQuery();
            }
        }




       

        protected void radInv_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            if (radInv.SelectedItem.ToString() == "UnInvoiced")
            {
                dt = _cm.getQuotationUninvoiced();
            }
            else if (radInv.SelectedItem.ToString() == "Invoiced")
            {
                dt = _cm.getQuotationInvoiced();
            }
            else
            {
                dt = _cm.getQuotationNEW();
            }
            if (dt.Rows.Count > 0)
            {
                lblRecords.Text = dt.Rows.Count.ToString();
                gvQuotation.DataSource = dt;
                gvQuotation.DataBind();
            }
            else
            {
                gvQuotation.DataSource = null;
                gvQuotation.DataBind();
            }
        }

        protected void txtQuotationNumber_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;

            // Get the header row (the NamingContainer of the textbox)
            GridViewRow headerRow = txt.NamingContainer as GridViewRow;

            TextBox txtCustomerName = headerRow.FindControl("txtCustomerName") as TextBox;
            TextBox txtQuotationNumber = headerRow.FindControl("txtQuotationNumber") as TextBox;
            TextBox txtDate = headerRow.FindControl("txtDate") as TextBox;
            TextBox txtQuotationStatus = headerRow.FindControl("txtQuotationStatus") as TextBox;

            _cm.customerName = txtCustomerName.Text.Trim();
            _cm.QuotationNumber = txtQuotationNumber.Text.Trim();
            if (txtDate.Text.Trim() != "")
            {
                _cm.Date = Convert.ToDateTime(txtDate.Text.Trim());
            }
            _cm.QuotationStatus = txtQuotationStatus.Text.Trim();

            ViewState["customerName"] = txtCustomerName.Text;
            ViewState["QuotationNumber"] = txtQuotationNumber.Text;
            ViewState["Date"] = txtDate.Text;
            ViewState["QuotationStatus"] = txtQuotationStatus.Text;

            BindQuotationFilter(_cm);

        }

        protected void txtCustomerName_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;

            // Get the header row (the NamingContainer of the textbox)
            GridViewRow headerRow = txt.NamingContainer as GridViewRow;

            TextBox txtCustomerName = headerRow.FindControl("txtCustomerName") as TextBox;
            TextBox txtQuotationNumber = headerRow.FindControl("txtQuotationNumber") as TextBox;
            TextBox txtDate = headerRow.FindControl("txtDate") as TextBox;
            TextBox txtQuotationStatus = headerRow.FindControl("txtQuotationStatus") as TextBox;

            _cm.customerName = txtCustomerName.Text.Trim();
            _cm.QuotationNumber = txtQuotationNumber.Text.Trim();
            if (txtDate.Text.Trim() != "")
            {
                _cm.Date = Convert.ToDateTime(txtDate.Text.Trim());
            }
            _cm.QuotationStatus = txtQuotationStatus.Text.Trim();

            ViewState["customerName"] = txtCustomerName.Text;
            ViewState["QuotationNumber"] = txtQuotationNumber.Text;
            ViewState["Date"] = txtDate.Text;
            ViewState["QuotationStatus"] = txtQuotationStatus.Text;

            BindQuotationFilter(_cm);
        }

        protected void txtDate_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;

            // Get the header row (the NamingContainer of the textbox)
            GridViewRow headerRow = txt.NamingContainer as GridViewRow;

            TextBox txtCustomerName = headerRow.FindControl("txtCustomerName") as TextBox;
            TextBox txtQuotationNumber = headerRow.FindControl("txtQuotationNumber") as TextBox;
            TextBox txtDate = headerRow.FindControl("txtDate") as TextBox;
            TextBox txtQuotationStatus = headerRow.FindControl("txtQuotationStatus") as TextBox;

            _cm.customerName = txtCustomerName.Text.Trim();
            _cm.QuotationNumber = txtQuotationNumber.Text.Trim();
            if (txtDate.Text.Trim() != "")
            {
                _cm.Date = Convert.ToDateTime(txtDate.Text.Trim());
            }
            _cm.QuotationStatus = txtQuotationStatus.Text.Trim();

            ViewState["customerName"] = txtCustomerName.Text;
            ViewState["QuotationNumber"] = txtQuotationNumber.Text;
            ViewState["Date"] = txtDate.Text;
            ViewState["QuotationStatus"] = txtQuotationStatus.Text;

            BindQuotationFilter(_cm);
        }

        protected void txtQuotationStatus_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;

            // Get the header row (the NamingContainer of the textbox)
            GridViewRow headerRow = txt.NamingContainer as GridViewRow;

            TextBox txtCustomerName = headerRow.FindControl("txtCustomerName") as TextBox;
            TextBox txtQuotationNumber = headerRow.FindControl("txtQuotationNumber") as TextBox;
            TextBox txtDate = headerRow.FindControl("txtDate") as TextBox;
            TextBox txtQuotationStatus = headerRow.FindControl("txtQuotationStatus") as TextBox;

            _cm.customerName = txtCustomerName.Text.Trim();
            _cm.QuotationNumber = txtQuotationNumber.Text.Trim();
            if (txtDate.Text.Trim() != "")
            {
                _cm.Date = Convert.ToDateTime(txtDate.Text.Trim());
            }
            _cm.QuotationStatus = txtQuotationStatus.Text.Trim();

            ViewState["customerName"] = txtCustomerName.Text;
            ViewState["QuotationNumber"] = txtQuotationNumber.Text;
            ViewState["Date"] = txtDate.Text;
            ViewState["QuotationStatus"] = txtQuotationStatus.Text;

            BindQuotationFilter(_cm);
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = ViewState["GridData"] as DataTable;

                if (dt == null || dt.Rows.Count == 0)
                    return;

                ExportToExcel(dt, "QuotationList");
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "exportError",
                    $"alert('Error exporting to Excel: {ex.Message.Replace("'", "\\'")}');", true);
            }
        }
        private void ExportToExcel(DataTable dt, string fileName)
        {
            Response.Clear();
            Response.AddHeader("content-disposition", $"attachment;filename={fileName}.xls");
            Response.ContentType = "application/vnd.ms-excel";

            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);

            hw.Write("<table border='1'>");

            hw.Write("<tr>");
            foreach (DataColumn col in dt.Columns)
                hw.Write("<th>" + col.ColumnName + "</th>");
            hw.Write("</tr>");

            foreach (DataRow row in dt.Rows)
            {
                hw.Write("<tr>");
                foreach (var cell in row.ItemArray)
                    hw.Write("<td>" + cell.ToString() + "</td>");
                hw.Write("</tr>");
            }

            hw.Write("</table>");

            Response.Write(sw.ToString());
            Response.End();
        }
        private void BindQuotation(string search)
        {
            DataTable dt = _cm.searchQuotationNEW(search);
            if (dt.Rows.Count > 0)
            {
                gvQuotation.DataSource = dt;
                gvQuotation.DataBind();
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
                BindQuotation(search);
            }
        }
        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string search = txtSearch.Text;
            BindQuotation(search);
        }
    }
}

