using DocumentFormat.OpenXml.Presentation;
using iTextSharp.text.pdf;
using Microsoft.Office.Interop.Word;
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
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTable = System.Data.DataTable;

namespace LIMS
{
    public partial class CustomerCal : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["ID"] != null)
                {
                    Session["OrganizationID"] = 1;
                    int InquiryId = Convert.ToInt32(Request.QueryString["ID"].ToString());
                    System.Data.DataTable dtInquiryStatus = _cm.GetEnquiryStatus(InquiryId);
                    if (dtInquiryStatus.Rows.Count > 0)
                    {
                        int inquiryStatus = Convert.ToInt32(dtInquiryStatus.Rows[0]["inquiryStatusID"].ToString());
                        if (inquiryStatus == 5)
                        {
                            int DepartmentID = Convert.ToInt32(dtInquiryStatus.Rows[0]["DepartmentID"].ToString());

                            if (DepartmentID == 4013)
                            {
                                string url = "CustomerFMN_IAT.aspx?ID=" + InquiryId;
                                Response.Redirect(url);
                            }
                        }
                            if (dtInquiryStatus.Rows[0]["Instruments"].ToString() != "")
                        {
                            string[] s1 = dtInquiryStatus.Rows[0]["Instruments"].ToString().Split('|');
                            foreach (string s2 in s1)
                            {
                                string[] inst = s2.Split(':');
                                hdInstruments.Value += inst[2] + ",";
                            }
                        }
                        if (hdInstruments.Value.Length > 0)
                        {
                            hdInstruments.Value = hdInstruments.Value.Remove(hdInstruments.Value.Length - 1);
                            BindInstruments();
                        }
                        divCRF.Visible = true;
                        divFail.Visible = false;
                        Session["InquiryID"] = InquiryId;
                        lblInquiry.Text = "ENQ" + InquiryId;
                        hdInquiryId.Value = InquiryId.ToString();
                        System.Data.DataTable dt = _cm.BindInquiryCustomer(InquiryId);
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
                            BindContactPerson(InquiryId);
                        }
                    }
                    if (gvInstruments.Rows.Count == 0)
                    {
                        btnSubmit.Visible = false;
                    }
                }
                else
                if (Request.QueryString["CID"] != null)
                {
                    int EnquiryID = Convert.ToInt32(Request.QueryString["CID"]);
                    GenerateCalibrationRequest(EnquiryID);
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }
        private void BindContactPerson(int InquiryId)
        {
            System.Data.DataTable dt = _cm.BindContactPerson(InquiryId);
            if (dt.Rows.Count > 0)
            {
                ddlContactPerson.DataSource = dt;
                ddlContactPerson.DataTextField = "CustomerName";
                ddlContactPerson.DataValueField = "PersonId";
                ddlContactPerson.DataBind();
                ddlContactPerson.Items.Insert(0, new ListItem("Select Person"));
            }

        }
        protected void ddlContactPerson_SelectedIndexChanged(object sender, EventArgs e)
        {
            int PersonId = 0;
            if (ddlContactPerson.SelectedIndex != 0)
            {
                PersonId = Convert.ToInt32(ddlContactPerson.SelectedItem.Value);
                System.Data.DataTable dt = _cm.BindCustomerContactPerson(PersonId);
                if (dt.Rows.Count > 0)
                {
                    txtMobile.Text = dt.Rows[0]["Mobile"].ToString();
                    txtContactEmail.Text = dt.Rows[0]["Email"].ToString();
                    txtFax.Text = dt.Rows[0]["Fax"].ToString();
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int flag = 0;
            //flag = CheckCustomer();
            if (txtCustomer.Text == "") { txtCustomer.BorderColor = Color.Red; flag = 1; } else { txtCustomer.BorderColor = Color.Empty; }
            if (txtAddressLine1.Text == "") { txtAddressLine1.BorderColor = Color.Red; flag = 1; } else { txtAddressLine1.BorderColor = Color.Empty; }
            if (ddlContactPerson.SelectedIndex == -1) { txtContactPerson.BorderColor = Color.Red; flag = 1; } else { txtContactPerson.BorderColor = Color.Empty; }
            if (flag == 0)
            {
                int InquiryId = Convert.ToInt32(Request.QueryString["ID"].ToString());
                System.Data.DataTable dt = (System.Data.DataTable)Session["DT"];
                foreach (DataRow dr in dt.Rows)
                {
                    _cm.EnquiryID = InquiryId;
                    _cm.InstrumentID = Convert.ToInt32(dr["InstrumentID"].ToString());
                    _cm.InstrumentName = dr["InstrumentName"].ToString();
                    _cm.Brand = dr["Brand"].ToString();
                    _cm.InstrumentType = dr["InstrumentType"].ToString();
                    _cm.SerialNo = dr["SerialNo"].ToString();
                    _cm.CalibrationRange = dr["CalibrationRange"].ToString();
                    _cm.Quantity = Convert.ToDecimal(dr["Quantity"].ToString());
                    _cm.ComplianceCheck = dr["ComplianceCheck"].ToString();
                    _cm.personId = Convert.ToInt32(dr["PersonID"].ToString());
                    _cm.AddCalibrationRequest(_cm);
                }
                GenerateCalibrationRequest(InquiryId);
            }
        }
        protected void GenerateCalibrationRequest(int InquiryId)
        {
            Application wordApp = new Application();
            Microsoft.Office.Interop.Word.Document doc = null;

            string path = Server.MapPath("~/Inputfiles/");
            string templatePath = path + "TSQ-CAL-FM-001.doc";
            doc = wordApp.Documents.Add(templatePath);

            System.Data.DataTable dtCustomer = _cm.BindInquiryCustomer(InquiryId);
            if (dtCustomer.Rows.Count > 0)
            {
                // Replace placeholders with database values
                FindAndReplace(wordApp, "{CustomerName}", dtCustomer.Rows[0]["CustomerName"].ToString());
                FindAndReplace(wordApp, "{Address}", dtCustomer.Rows[0]["AddressLine1"].ToString() + " " + dtCustomer.Rows[0]["City"].ToString() + " " + dtCustomer.Rows[0]["Pincode"].ToString());
            }
            System.Data.DataTable dtCalibContact = _cm.GetCalibrationContact(InquiryId);
            System.Data.DataTable dtCalibRequest = _cm.GetCalibrationRequest(InquiryId);
            if (dtCalibContact.Rows.Count > 0)
            {
                FindAndReplace(wordApp, "{InstrumentCount}", dtCalibContact.Rows.Count.ToString());
                FindAndReplace(wordApp, "{ContactPerson}", dtCalibContact.Rows[0]["ContactPerson"].ToString());
                FindAndReplace(wordApp, "{Telephone}", dtCalibContact.Rows[0]["phone"].ToString());
                FindAndReplace(wordApp, "{Email}", dtCalibContact.Rows[0]["Email"].ToString());
                FindAndReplace(wordApp, "{Fax}", dtCalibContact.Rows[0]["Fax"].ToString());

                CreateInstrumentTable(wordApp, doc, "{InstrumentList}", dtCalibRequest);
                try
                {
                    string pathout = Server.MapPath("~/CalibrationRequest/");
                    string outputDocxPath = pathout + "CalibrationRequest-" + InquiryId + ".docx";
                    string outputPdfPath = pathout + "CalibrationRequest-" + InquiryId + ".pdf";
                    doc.SaveAs2(outputDocxPath);
                    doc.SaveAs2(outputPdfPath, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatPDF);
                    Response.AddHeader("content-disposition", "inline;filename=" + outputPdfPath);
                    string script = "<script>window.open('/CalibrationRequest/CalibrationRequest-" + InquiryId + ".pdf" + "', '_blank');</script>";
                    string relativePath = @"CalibrationRequest\CalibrationRequest-" + InquiryId + ".pdf";
                    string filename= "CalibrationRequest-" + InquiryId + ".pdf";
                    // Convert to absolute path
                    string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

                    if (File.Exists(fullPath))
                    {
                        //  Process.Start(new ProcessStartInfo(fullPath) { UseShellExecute = true });
                        ScriptManager.RegisterStartupScript(
                          this,
                          GetType(),
                          "openPdf",
                          "window.open('/CalibrationRequest/"+filename+ "', '_self');",
                          true
                      );
                    }
                    else
                    {
                        Console.WriteLine("PDF file not found: " + fullPath);
                    }
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "OpenPDF", script);
                    // To save changes before closing
                    doc.Close(Microsoft.Office.Interop.Word.WdSaveOptions.wdSaveChanges);
                    wordApp.Quit();

                }
                catch (Exception ex)
                {
                    string s1 = ex.Message;
                    Response.Write(s1);
                    LogExceptionToFile(ex);
                }
                finally
                {
                    if (doc != null)
                    {
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(doc);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);
                        doc = null;
                        wordApp = null;
                    }

                    //change customer inquiry Status to CRF Received
                    _cm.inquiryId = InquiryId;
                    _cm.inquiryStatusId = 6;
                    _cm.UpdateInquiryStatus(_cm);
                    GC.Collect();
                    //Response.Redirect("CustomerInquiryView.aspx");
                }
            }

        }
        private void LogExceptionToFile(Exception ex)
        {
            try
            {
                // Define the file path (use Server.MapPath to get a physical path)
                // Ensure the 'App_Data' folder exists in your project for storing logs
                string logFilePath = Server.MapPath("~/App_Data/ErrorLog.txt");

                // Use StreamWriter to append the exception details to the file
                using (StreamWriter sw = new StreamWriter(logFilePath, true))
                {
                    sw.WriteLine("--------------------------------------------------");
                    sw.WriteLine("Date: " + DateTime.Now.ToString());
                    sw.WriteLine("Message: " + ex.Message);
                    sw.WriteLine("Source: " + ex.Source);
                    // This is the full stack trace
                    sw.WriteLine("Stack Trace: " + ex.StackTrace);
                    sw.WriteLine("--------------------------------------------------");
                    sw.WriteLine();
                }
            }
            catch (Exception logEx)
            {
                // Handle potential errors during the logging process itself
                // For example, if the file is locked or the path is invalid
                // You might consider using System.Diagnostics.Debug.WriteLine() here
                // if logging fails.
                System.Diagnostics.Debug.WriteLine("Error during logging: " + logEx.Message);
            }
        }
        public void CreateInstrumentTable(Microsoft.Office.Interop.Word.Application wordApp, Document doc, string placeholder, System.Data.DataTable dt)
        {
            var range = doc.Content;
            range.Find.Text = placeholder;
            range.Find.Execute();

            if (range != null)
            {
                // 4. Create a table at placeholder
                int rows = dt.Rows.Count + 1; // +1 for header
                int cols = dt.Columns.Count;

                Microsoft.Office.Interop.Word.Table wordTable = doc.Tables.Add(range, rows, cols);
                wordTable.Borders.Enable = 1;
                // 5. Insert column headers
                for (int c = 0; c < cols; c++)
                {
                    wordTable.Cell(1, c + 1).Range.Text = dt.Columns[c].ColumnName;
                    wordTable.Cell(1, c + 1).Range.Bold = 1;
                }


                // 6. Insert data rows
                for (int r = 0; r < dt.Rows.Count; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        wordTable.Cell(r + 2, c + 1).Range.Text = dt.Rows[r][c].ToString();
                    }
                }
            }
        }

        static void FindAndReplace(Application wordApp, string findText, string replaceText)
        {
            wordApp.Selection.Find.ClearFormatting();
            wordApp.Selection.Find.Text = findText;
            wordApp.Selection.Find.Replacement.ClearFormatting();
            wordApp.Selection.Find.Replacement.Text = replaceText;
            wordApp.Selection.Find.Execute(Replace: WdReplace.wdReplaceAll);
        }
        protected void btnAddDetails_Click(object sender, EventArgs e)
        {
            int flag = 0;
            // flag = CheckFields();
            if (txtCustomer.Text == "") { txtCustomer.BorderColor = Color.Red; flag = 1; } else { txtCustomer.BorderColor = Color.Empty; }
            if (txtAddressLine1.Text == "") { txtAddressLine1.BorderColor = Color.Red; flag = 1; } else { txtAddressLine1.BorderColor = Color.Empty; }
            if (ddlContactPerson.SelectedIndex == -1) { txtContactPerson.BorderColor = Color.Red; flag = 1; } else { txtContactPerson.BorderColor = Color.Empty; }

            if (flag == 0)
            {
                DataTable dt = createTempTable();
                DataRow dr = dt.NewRow();
                dr["InstrumentID"] = ddlInstruments.SelectedValue;
                dr["InstrumentName"] = ddlInstruments.SelectedItem.ToString();
                dr["Brand"] = txtBrand.Text;
                dr["InstrumentType"] = txtType.Text;
                dr["SerialNo"] = txtSerialNo.Text;
                dr["CalibrationRange"] = txtCalbRange.Text;
                dr["Quantity"] = Convert.ToDecimal(txtQty.Text);
                dr["ComplianceCheck"] = radCompliance.SelectedItem.ToString();
                dr["PersonID"] = ddlContactPerson.SelectedValue;
                dt.Rows.Add(dr);
                try
                {
                    if (dt.Rows.Count > 0)
                    {
                        gvInstruments.DataSource = dt;
                        gvInstruments.DataBind();
                        Session["DT"] = dt;
                        btnSubmit.Visible = true;
                    }
                    else
                    {

                    }
                }
                catch (Exception ex) { string s = ex.Message; }
                Clear();
            }
        }
        protected int CheckCustomer()
        {
            int flag = 0;
            if (txtCustomer.Text == "") { txtCustomer.BorderColor = Color.Red; flag = 1; } else { txtCustomer.BorderColor = Color.Empty; }
            if (txtAddressLine1.Text == "") { txtAddressLine1.BorderColor = Color.Red; flag = 1; } else { txtAddressLine1.BorderColor = Color.Empty; }
            if (ddlContactPerson.SelectedIndex == -1) { txtContactPerson.BorderColor = Color.Red; flag = 1; } else { txtContactPerson.BorderColor = Color.Empty; }
            return flag;
        }
        protected int CheckFields()
        {
            int flag = 0;
            if (ddlInstruments.SelectedIndex==0) { ddlInstruments.BorderColor = Color.Red; flag = 1; } else { ddlInstruments.BorderColor = Color.Empty; }
            if (txtBrand.Text == "") { txtBrand.BorderColor = Color.Red; flag = 1; } else { txtBrand.BorderColor = Color.Empty; }
            if (txtType.Text == "") { txtType.BorderColor = Color.Red; flag = 1; } else { txtType.BorderColor = Color.Empty; }
            if (txtSerialNo.Text == "") { txtSerialNo.BorderColor = Color.Red; flag = 1; } else { txtSerialNo.BorderColor = Color.Empty; }
            if (txtCalbRange.Text == "") { txtCalbRange.BorderColor = Color.Red; flag = 1; } else { txtCalbRange.BorderColor = Color.Empty; }
            if (txtQty.Text == "") { txtQty.BorderColor = Color.Red; flag = 1; } else { txtQty.BorderColor = Color.Empty; }
            if (radCompliance.SelectedIndex == -1) { radCompliance.BackColor = Color.LightPink; flag = 1; } else { radCompliance.BackColor = Color.Empty; }
            return flag;
        }
        protected void Clear()
        {
            ddlInstruments.ClearSelection();
            txtBrand.Text = "";
            txtType.Text = "";
            txtSerialNo.Text = "";
            txtCalbRange.Text = "";
            txtQty.Text = "";
            radCompliance.ClearSelection();
        }
        protected System.Data.DataTable createTempTable()
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            if (Session["DT"] != null)
            {
                dt = (System.Data.DataTable)Session["DT"];
            }
            else
            {
                dt.Columns.Add("InstrumentID");
                dt.Columns.Add("InstrumentName");
                dt.Columns.Add("Brand");
                dt.Columns.Add("InstrumentType");
                dt.Columns.Add("SerialNo");
                dt.Columns.Add("CalibrationRange");
                dt.Columns.Add("Quantity");
                dt.Columns.Add("ComplianceCheck");
                dt.Columns.Add("PersonID");
            }
            return dt;
        }

        protected void BindInstruments()
        {
            DataTable dt = new DataTable();
            if (hdInstruments.Value.Length > 0)
            {
                dt = _cm.getCalibInstruments(hdInstruments.Value);
                if (dt.Rows.Count > 0)
                {
                    ddlInstruments.DataSource = dt;
                    ddlInstruments.DataTextField = "InstrumentName";
                    ddlInstruments.DataValueField = "InstrumentID";
                    ddlInstruments.DataBind();
                    ddlInstruments.Items.Insert(0, "Select Instrument");
                }
            }
        }
    }
}