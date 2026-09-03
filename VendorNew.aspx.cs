using LIMS.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Management.Automation;
using System.Security.Cryptography.Xml;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace LIMS
{
    public partial class VendorNew : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ToString());

        string date = DateTime.Today.ToString("dd-MMM-yyyy");
        string time = DateTime.Now.ToString("h:mm:ss tt");

        string CreateDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string UpdateDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        string constr = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;

        LIMS.DAL.CM _cm = new LIMS.DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                BindQuoteCurrency();
                Response.AppendHeader("Access-Control-Allow-Origin", "*");
                divContent.Visible = false;
                BindYear();
                BindDealer();
                BindVAT();
                BindCountry();
                divVerify.Visible = true;
                if (Request.QueryString["ID"] != null)
                {
                    string Reference = Request.QueryString["ID"].ToString();
                    txtReferenceNumber.Text = Reference;
                }
                ChangeTab("Guide");
                BindCompanySize();
                BindReference();
                PopulateTreeview();
                //BindTreeView();
                BindTypeofEstablishment();
                BindYears();


                btnSavePan.Enabled = true;
                btnSaveCancelledChq.Enabled = false;
                btnSaveRegistrationofCompany.Enabled = false;

                btnSaveGST.Enabled = false;
                btnSavePF.Enabled = false;
                btnSaveUANNumber.Enabled = false;
                btnSaveESIC.Enabled = false;
                btnSaveVendorLogo.Enabled = false;

            }
            else
            { // Restore tab state after postback
                string currentTab = Session["CurrentTab"] as string ?? "Guide";
                ChangeTab(currentTab);

                BindReference();
            }
        }
        protected void BindQuoteCurrency()
        {
            DataTable dt = _cm.QuoteCurrency();
            ddlQuoteCurrency.DataSource = dt;
            ddlQuoteCurrency.DataTextField = "CurrencyName";
            ddlQuoteCurrency.DataValueField = "CurrencyID";
            ddlQuoteCurrency.DataBind();
            ddlQuoteCurrency.Items.Insert(0, new ListItem("Select QuoteCurrency", ""));
        }
        private void BindVAT()
        {
            DataTable dt = _cm.getVAT();
            ddlVAT.DataSource = dt;
            ddlVAT.DataTextField = "VATCode";
            ddlVAT.DataValueField = "VATID";
            ddlVAT.DataBind();
            ddlVAT.Items.Insert(0, new ListItem("Select VAT", ""));
        }
        private void BindCountry()
        {
            DataTable dt = _cm.getActiveCountries(); //  update this function next
            ddlCountry.DataSource = dt;
            ddlCountry.DataTextField = "CountryName";
            ddlCountry.DataValueField = "CountryCode";
            ddlCountry.DataBind();
            ddlCountry.Items.Insert(0, new ListItem("Select Country", ""));
        }
        private void BindCompanySize()
        {
            DataTable dt = _cm.GetCompanySizeList();
            if (dt.Rows.Count > 0)
            {
                ddlCompanySize.DataSource = dt;
                ddlCompanySize.DataTextField = "CompanySize";
                ddlCompanySize.DataValueField = "CompanySizeCode";
                ddlCompanySize.DataBind();
                ddlCompanySize.Items.Insert(0, new ListItem("Select Company Size", ""));
            }
        }
        private void BindReference()
        {
            if (Session["Reference"] != null)
            {
                txtVendorCode.Text = Session["Code"].ToString();
            }
        }
        protected void btnGuideLines_Click(object sender, EventArgs e)
        {

            ChangeTab("Home"); // Activate the tab
        }
        protected void ChangeTab(string tabName)
        {
            // Reset all tabs first
            ResetAllTabs();

            switch (tabName)
            {
                case "Guide":
                    divguide.Attributes["class"] = "tab-pane show active";
                    aguide.Attributes["class"] = "nav-link active";
                    break;

                case "Home":
                    divhome.Attributes["class"] = "tab-pane show active";
                    ahome.Attributes["class"] = "nav-link active";
                    // Enable Bank tab if vendor is saved
                    if (Session["VendorSaved"] != null && (bool)Session["VendorSaved"])
                        abank.Attributes["class"] = "nav-link";
                    break;

                case "Bank":
                    divbank.Attributes["class"] = "tab-pane show active";
                    abank.Attributes["class"] = "nav-link active";
                    // Enable accessible tabs
                    aguide.Attributes["class"] = "nav-link";
                    ahome.Attributes["class"] = "nav-link";
                    if (Session["VendorSaved"] != null && (bool)Session["VendorSaved"])
                        aperson.Attributes["class"] = "nav-link";
                    break;

                case "Person":
                    divperson.Attributes["class"] = "tab-pane show active";
                    aperson.Attributes["class"] = "nav-link active";
                    // Enable all previous tabs
                    aguide.Attributes["class"] = "nav-link";
                    ahome.Attributes["class"] = "nav-link";
                    abank.Attributes["class"] = "nav-link";
                    alocation.Attributes["class"] = "nav-link";
                    break;

                case "Location":
                    divlocation.Attributes["class"] = "tab-pane show active";
                    alocation.Attributes["class"] = "nav-link active";
                    // Enable all previous tabs
                    aguide.Attributes["class"] = "nav-link";
                    ahome.Attributes["class"] = "nav-link";
                    abank.Attributes["class"] = "nav-link";
                    aperson.Attributes["class"] = "nav-link";
                    afiles.Attributes["class"] = "nav-link";
                    break;

                case "Files":
                    divfiles.Attributes["class"] = "tab-pane show active";
                    afiles.Attributes["class"] = "nav-link active";
                    // Enable all previous tabs
                    aguide.Attributes["class"] = "nav-link";
                    ahome.Attributes["class"] = "nav-link";
                    abank.Attributes["class"] = "nav-link";
                    aperson.Attributes["class"] = "nav-link";
                    alocation.Attributes["class"] = "nav-link";
                    BindReference();
                    break;

                default:
                    // Default to Guide tab
                    divguide.Attributes["class"] = "tab-pane show active";
                    aguide.Attributes["class"] = "nav-link active";
                    break;
            }

            // Store current tab in session for persistence
            Session["CurrentTab"] = tabName;
        }
        private void ResetAllTabs()
        {
            // Reset all tab panes
            divguide.Attributes["class"] = "tab-pane fade";
            divhome.Attributes["class"] = "tab-pane fade";
            divbank.Attributes["class"] = "tab-pane fade";
            divperson.Attributes["class"] = "tab-pane fade";
            divlocation.Attributes["class"] = "tab-pane fade";
            divfiles.Attributes["class"] = "tab-pane fade";

            // Reset all nav links to disabled state
            aguide.Attributes["class"] = "nav-link disabled";
            ahome.Attributes["class"] = "nav-link disabled";
            abank.Attributes["class"] = "nav-link disabled";
            aperson.Attributes["class"] = "nav-link disabled";
            alocation.Attributes["class"] = "nav-link disabled";
            afiles.Attributes["class"] = "nav-link disabled";
        }
        protected void btnAddMoreContactPerson_Click(object sender, EventArgs e)
        {
            // Make sure this executes AFTER Page_Load
            ChangeTab("Person");
        }

        protected void btnVendor_Click(object sender, EventArgs e)
        {
            int flag = 0;

            if (string.IsNullOrWhiteSpace(txtVendorName.Text))
            {
                txtVendorName.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            if (string.IsNullOrWhiteSpace(txtCompanyName.Text))
            {
                txtCompanyName.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            if (ddlCompanySize.SelectedIndex == 0)
            {
                ddlCompanySize.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            if (ddlYearOfEstablishment.SelectedIndex == 0)
            {
                ddlYearOfEstablishment.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            if (ddlTypeofEstablishment.SelectedIndex == 0)
            {
                ddlTypeofEstablishment.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            if (ddlTypeOfBussiness.SelectedIndex == 0)
            {
                ddlTypeOfBussiness.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            if (ddlDealer.SelectedIndex == 0)
            {
                ddlDealer.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            if (ddlVAT.SelectedIndex == 0)
            {
                ddlVAT.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            if (ddlQuoteCurrency.SelectedIndex == 0)
            {
                ddlQuoteCurrency.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            if (string.IsNullOrWhiteSpace(txtPan.Text) || txtPan.Text.Length < 10)
            {
                txtPan.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            /*Elan - make not mandatory - whatsapp - 6Apr26*/
            //if (string.IsNullOrWhiteSpace(txtYearlyTurnover1.Text))
            //{
            //    txtYearlyTurnover1.BorderColor = System.Drawing.Color.Red;
            //    flag = 1;
            //}
            //if (string.IsNullOrWhiteSpace(txtYearlyTurnover2.Text))
            //{
            //    txtYearlyTurnover2.BorderColor = System.Drawing.Color.Red;
            //    flag = 1;
            //}
            //if (string.IsNullOrWhiteSpace(txtYearlyTurnover3.Text))
            //{
            //    txtYearlyTurnover3.BorderColor = System.Drawing.Color.Red;
            //    flag = 1;
            //}
            // TreeView Category Selection Validation - At least one category must be selected
            bool hasSelectedCategory = false;
            foreach (TreeNode node in tvTables.Nodes)
            {
                node.ShowCheckBox = true;
                if (node.Checked)
                {
                    hasSelectedCategory = true;
                    break;
                }
                foreach (TreeNode child in node.ChildNodes)
                {
                    if (child.Checked)
                    {
                        hasSelectedCategory = true;
                        break;
                    }
                }
                if (hasSelectedCategory) break;
            }

            if (!hasSelectedCategory)
            {
                // Highlight the TreeView container or show error message
                btnSelectCategory.Style.Add("border", "2px solid red");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please select at least one category from Distributor Category.');", true);
                flag = 1;
            }
            else
            {
                btnSelectCategory.Style.Remove("border");
            }

            if (flag == 1)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    "alert('Please fill all highlighted required fields!');", true);
                return;
            }

            // Check for duplicate Vendor Code
            string VendorCode = txtVendorCode.Text.ToUpper();
            DataTable dt = _cm.GetVendorCode(VendorCode);
            if (dt.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    "alert('Vendor Code Already Exists!');", true);
                txtVendorCode.BorderColor = System.Drawing.Color.Red;
                return;
            }

            // Check for duplicate Vendor Name
            string VendorName = txtVendorName.Text.ToUpper();
            DataTable dt1 = _cm.GetVendorName(VendorName);
            if (dt1.Rows.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    "alert('Vendor Name Already Exists!');", true);
                txtVendorName.BorderColor = System.Drawing.Color.Red;
                return;
            }

            ChangeTab("Bank");
        }

        protected void btnSaveBank_Click(object sender, EventArgs e)
        {
            if (gvBank.Rows.Count == 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    "alert('Please add at least one Bank Account!');", true);
                return;
            }

            BindReference();
            ChangeTab("Person");
        }

        protected void btnSaveContactPerson_Click(object sender, EventArgs e)
        {
            if (gvPerson.Rows.Count == 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    "alert('Please add at least one Contact Person!');", true);
                return;
            }

            bool primaryExists = false;
            foreach (GridViewRow row in gvPerson.Rows)
            {
                string contactType = row.Cells[7].Text.Trim();
                if (contactType.Equals("Primary", StringComparison.OrdinalIgnoreCase))
                {
                    primaryExists = true;
                    break;
                }
            }

            if (!primaryExists)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    "alert('Please add at least one Primary Contact Person!');", true);
                return;
            }

            BindReference();
            ChangeTab("Location");
        }

        protected void btnSaveLocation_Click(object sender, EventArgs e)
        {
            if (gvLocation.Rows.Count == 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    "alert('Please add at least one Location!');", true);
                return;
            }

            bool registeredOfficeExists = false;
            foreach (GridViewRow row in gvLocation.Rows)
            {
                string locationType = row.Cells[1].Text.Trim();
                if (locationType.Equals("Registered Office", StringComparison.OrdinalIgnoreCase))
                {
                    registeredOfficeExists = true;
                    break;
                }
            }

            if (!registeredOfficeExists)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    "alert('Registered Office address is mandatory!');", true);
                return;
            }

            // Just move to Files tab - NO SAVING
            ChangeTab("Files");
        }

        // STEP 2: Update all document save buttons to store files in temporary session
        // Replace ALL your document button click methods with these:

        protected void btnSavePan_Click(object sender, EventArgs e)
        {
            if (fpPanCard.HasFile && IsValidExtension(fpPanCard.FileName))
            {
                fpPanCard.BorderColor = System.Drawing.Color.Empty;

                // Store file in session temporarily
                Session["PAN_File"] = fpPanCard.FileBytes;
                Session["PAN_FileName"] = fpPanCard.FileName;
                Session["PAN"] = 1;

                lblSavePan.Visible = true;
                lblSavePan.Text = "File Selected!";
                fpPanCard.Enabled = false;
                btnSavePan.Enabled = false;
                btnSaveCancelledChq.Enabled = true;
            }
            else
            {
                fpPanCard.BorderColor = System.Drawing.Color.Red;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    "alert('Please select a valid file! Allowed: PDF, JPG, PNG, DOC, DOCX');", true);
            }
        }

        protected void btnSaveCancelledChq_Click(object sender, EventArgs e)
        {
            if (fpCancelledChq.HasFile && IsValidExtension(fpCancelledChq.FileName))
            {
                fpCancelledChq.BorderColor = System.Drawing.Color.Empty;

                Session["CHQ_File"] = fpCancelledChq.FileBytes;
                Session["CHQ_FileName"] = fpCancelledChq.FileName;
                Session["CHQ"] = 1;

                lblSaveCancelledChq.Visible = true;
                lblSaveCancelledChq.Text = "File Selected!";
                fpCancelledChq.Enabled = false;
                btnSaveCancelledChq.Enabled = false;
                btnSaveRegistrationofCompany.Enabled = true;
            }
            else
            {
                fpCancelledChq.BorderColor = System.Drawing.Color.Red;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    "alert('Please select a valid file! Allowed: PDF, JPG, PNG, DOC, DOCX');", true);
            }
        }

        protected void btnSaveRegistrationofCompany_Click(object sender, EventArgs e)
        {
            if (fpRegistrationofCompany.HasFile && IsValidExtension(fpRegistrationofCompany.FileName))
            {
                fpRegistrationofCompany.BorderColor = System.Drawing.Color.Empty;

                Session["REG_File"] = fpRegistrationofCompany.FileBytes;
                Session["REG_FileName"] = fpRegistrationofCompany.FileName;

                lblSaveRegistrationofCompany.Visible = true;
                lblSaveRegistrationofCompany.Text = "File Selected!";
                fpRegistrationofCompany.Enabled = false;
                btnSaveRegistrationofCompany.Enabled = false;
                btnSaveGST.Enabled = true;
                btnSavePF.Enabled = true;
                btnSaveUANNumber.Enabled = true;
                btnSaveESIC.Enabled = true;
                btnSaveVendorLogo.Enabled = true;
            }
            else
            {
                fpRegistrationofCompany.BorderColor = System.Drawing.Color.Red;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    "alert('Please select a valid file! Allowed: PDF, JPG, PNG, DOC, DOCX');", true);
            }
        }

        protected void btnSaveGST_Click(object sender, EventArgs e)
        {
            if (fpGST.HasFile && IsValidExtension(fpGST.FileName))
            {
                Session["GST_File"] = fpGST.FileBytes;
                Session["GST_FileName"] = fpGST.FileName;
                lblSaveGST.Visible = true;
                lblSaveGST.Text = "File Selected!";
                fpGST.Enabled = false;
                btnSaveGST.Enabled = false;
            }
        }

        protected void btnSavePF_Click(object sender, EventArgs e)
        {
            if (fpPFCard.HasFile && IsValidExtension(fpPFCard.FileName))
            {
                Session["PF_File"] = fpPFCard.FileBytes;
                Session["PF_FileName"] = fpPFCard.FileName;
                lblSavePF.Visible = true;
                lblSavePF.Text = "File Selected!";
                fpPFCard.Enabled = false;
                btnSavePF.Enabled = false;
                btnFinish.Visible = true;
            }
        }

        protected void btnSaveUANNumber_Click(object sender, EventArgs e)
        {
            if (fpUANNumber.HasFile && IsValidExtension(fpUANNumber.FileName))
            {
                Session["UAN_File"] = fpUANNumber.FileBytes;
                Session["UAN_FileName"] = fpUANNumber.FileName;
                lblSaveUANNumber.Visible = true;
                lblSaveUANNumber.Text = "File Selected!";
                fpUANNumber.Enabled = false;
                btnSaveUANNumber.Enabled = false;
            }
        }

        protected void btnSaveESIC_Click(object sender, EventArgs e)
        {
            if (fpESIC.HasFile && IsValidExtension(fpESIC.FileName))
            {
                Session["ESIC_File"] = fpESIC.FileBytes;
                Session["ESIC_FileName"] = fpESIC.FileName;
                lblSaveESIC.Visible = true;
                lblSaveESIC.Text = "File Selected!";
                fpESIC.Enabled = false;
                btnSaveESIC.Enabled = false;
            }
        }

        protected void btnSaveVendorLogo_Click(object sender, EventArgs e)
        {
            if (fpVendorLogo.HasFile && IsValidExtension(fpVendorLogo.FileName))
            {
                Session["LOGO_File"] = fpVendorLogo.FileBytes;
                Session["LOGO_FileName"] = fpVendorLogo.FileName;
                lblSaveVendorLogo.Visible = true;
                lblSaveVendorLogo.Text = "File Selected!";
                fpVendorLogo.Enabled = false;
                btnSaveVendorLogo.Enabled = false;
            }
        }

        // STEP 3: Replace btnFinish_Click - Save EVERYTHING in one transaction
        protected void btnFinish_Click(object sender, EventArgs e)
        {
            if (!chkAccept.Checked)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    "alert('Please accept the terms and conditions!');", true);
                return;
            }
            //if (Session["PAN"] == null || Session["PAN"].ToString() != "1")
            ////if (Session["PAN"] == null || Session["CHQ"] == null ||
            ////    Session["PAN"].ToString() != "1" || Session["CHQ"].ToString() != "1")
            //{
            //    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
            //        "alert('PAN Card and Cancelled Cheque documents are mandatory!');", true);
            //    return;
            //}

            try
            {
                // Save vendor data
                SaveAllVendorData();

                // Save all documents
                SaveAllDocuments();

                // Update verification status
                _cm.Reference = Session["Reference"].ToString();
                _cm.StatusID = 3;
                _cm.UpdateVerificationDetails(_cm);

                Clear();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    "alert('Vendor Details Saved Successfully!');", true);
                Response.Redirect("GenerateVendorDetailsReport.aspx?VendorID=" + _cm.VendorID);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    "alert('Error saving data: " + ex.Message + "');", true);
            }
        }

        private void SaveAllVendorData()
        {
            string selectedGroups = "";
            string selectedSubGroups = "";

            foreach (TreeNode node in tvTables.Nodes)
            {
                if (node.Checked)
                {
                    selectedGroups += node.Value + ",";
                    foreach (TreeNode child in node.ChildNodes)
                    {
                        if (child.Checked)
                            selectedSubGroups += child.Value + ",";
                    }
                }
            }

            selectedGroups = selectedGroups.TrimEnd(',');
            selectedSubGroups = selectedSubGroups.TrimEnd(',');
            _cm.SelectedGroups = selectedGroups;
            _cm.SelectedSubGroups = selectedSubGroups;
            if (Session["Code"] != null)
            {
                _cm.VendorCode = Session["Code"].ToString();
            }
            else
            {
                if (Request.QueryString["ID"] != null)
                {
                    string reference = Request.QueryString["ID"].ToString();
                    txtReferenceNumber.Text = reference;
                    DataTable dtCheck = _cm.GetVerificationDetails(reference);
                    if (dtCheck.Rows.Count > 0)
                    {
                        Session["Reference"] = dtCheck.Rows[0]["Reference"].ToString();
                        Session["Code"] = dtCheck.Rows[0]["Code"].ToString();
                    }

                }
            }
            _cm.VendorName = txtVendorName.Text.ToString();
            _cm.DealerID = Convert.ToInt32(ddlDealer.SelectedValue.ToString());
            _cm.VAT = ddlVAT.SelectedIndex > 0 ? ddlVAT.SelectedValue : "0";
            _cm.PanNumber = string.IsNullOrWhiteSpace(txtPan.Text) ? "" : txtPan.Text.Trim();
            _cm.PFNumber = string.IsNullOrWhiteSpace(txtPF.Text) ? "" : txtPF.Text.Trim();
            _cm.IECode = string.IsNullOrWhiteSpace(txtIEC.Text) ? "" : txtIEC.Text.Trim();
            _cm.ESIC = string.IsNullOrWhiteSpace(txtESIC.Text) ? "" : txtESIC.Text.Trim();
            _cm.Website = string.IsNullOrWhiteSpace(txtWebsite.Text) ? "" : txtWebsite.Text.Trim();
            _cm.CompanyName = txtCompanyName.Text.ToString();
            _cm.YearofEstablishment = ddlYearOfEstablishment.SelectedItem.ToString();
            _cm.QuoteCurrencyid = Convert.ToInt32(ddlQuoteCurrency.SelectedValue);
            _cm.TypeofBusiness = ddlTypeOfBussiness.SelectedItem.ToString();
            _cm.TypeofEstablishment = ddlTypeofEstablishment.SelectedValue.ToString();
            _cm.Pincode = txtPincode.Text.ToString();
            _cm.RegistrationNumber = string.IsNullOrWhiteSpace(txtRegistrationNumber.Text) ? "" : txtRegistrationNumber.Text.Trim();
            if (txtYearlyTurnover1.Text.ToString() == "") { _cm.YearlyTurnover1 = 0; }
            else { _cm.YearlyTurnover1 = Convert.ToDecimal(txtYearlyTurnover1.Text.ToString()); }
            if (txtYearlyTurnover2.Text.ToString() == "") { _cm.YearlyTurnover2 = 0; }
            else { _cm.YearlyTurnover2 = Convert.ToDecimal(txtYearlyTurnover2.Text.ToString()); }
            if (txtYearlyTurnover3.Text.ToString() == "") { _cm.YearlyTurnover3 = 0; }
            else { _cm.YearlyTurnover3 = Convert.ToDecimal(txtYearlyTurnover3.Text.ToString()); }

            _cm.Year1 = lblYearlyTurnover1.Text.ToString();
            _cm.Year2 = lblYearlyTurnover2.Text.ToString();
            _cm.Year3 = lblYearlyTurnover3.Text.ToString();
            _cm.UANNumber = string.IsNullOrWhiteSpace(txtUANNumber.Text) ? "" : txtUANNumber.Text.Trim();
            _cm.NumberofEmployee = ddlCompanySize.SelectedValue.ToString();
            _cm.RequiredCredit = 0.0m;
            _cm.ApprovedCredit = string.IsNullOrWhiteSpace(txtApprovedCredit.Text) ? 0.0m : Convert.ToDecimal(txtApprovedCredit.Text);
            _cm.CreatedDate = DateTime.Now.AddHours(12.5);
            _cm.VendorStatus = 1;

            // Add vendor
            _cm.VendorID = _cm.AddVendorDetails(_cm);
            Session["VendorID"] = _cm.VendorID;

            // Save bank details
            if (Session["DTBank"] != null)
            {
                DataTable dtBank = (DataTable)Session["DTBank"];
                foreach (DataRow dr in dtBank.Rows)
                {
                    _cm.VendorID = Convert.ToInt32(Session["VendorID"].ToString());
                    _cm.AccountName = dr["AccountName"].ToString();
                    _cm.BankName = dr["BankName"].ToString();
                    _cm.BankBranch = dr["BankBranch"].ToString();
                    _cm.BankBranchCode = dr["BankBranchCode"].ToString();
                    _cm.AccountNumber = dr["AccountNumber"].ToString();
                    _cm.IFSCode = dr["IFSCode"].ToString();
                    _cm.SwiftCode = dr["SwiftCode"] != DBNull.Value ? dr["SwiftCode"].ToString() : string.Empty;
                    _cm.Type = "Vendor";
                    _cm.CreatedDate = DateTime.Now.AddHours(12.5);
                    _cm.AddBankDetails(_cm);
                }
            }

            // Save contact person and location
            if (Session["DTPerson"] != null && Session["DTLocation"] != null)
            {
                DataTable dtContactPerson = (DataTable)Session["DTPerson"];
                DataTable dtLocation = (DataTable)Session["DTLocation"];

                // STEP 1: Save all unique contact persons to ContactPerson table
                Dictionary<string, int> contactPersonMapping = new Dictionary<string, int>();

                for (int i = 0; i < dtContactPerson.Rows.Count; i++)
                {
                    _cm.VendorID = Convert.ToInt32(Session["VendorID"].ToString());
                    _cm.ContactPerson = dtContactPerson.Rows[i]["ID"].ToString();
                    _cm.Title = dtContactPerson.Rows[i]["Title"].ToString();
                    _cm.FirstName = dtContactPerson.Rows[i]["FirstName"].ToString();
                    _cm.MiddleName = dtContactPerson.Rows[i]["MiddleName"].ToString();
                    _cm.LastName = dtContactPerson.Rows[i]["LastName"].ToString();
                    _cm.Department = string.IsNullOrWhiteSpace(dtContactPerson.Rows[i]["Department"]?.ToString()) ? "N/A" : dtContactPerson.Rows[i]["Department"].ToString();
                    _cm.Designation = string.IsNullOrWhiteSpace(dtContactPerson.Rows[i]["Designation"]?.ToString()) ? "N/A" : dtContactPerson.Rows[i]["Designation"].ToString();
                    _cm.PersonAccountType = dtContactPerson.Rows[i]["PersonAccountType"].ToString();
                    _cm.CreatedDate = DateTime.Now.AddHours(12.5);

                    // Save contact person and get DB ID
                    int contactPersonID = _cm.AddContactPersonDetails(_cm);
                    contactPersonMapping[_cm.ContactPerson] = contactPersonID;
                }

                // STEP 2: Save UNIQUE contact details (one per contact person) to VendorContact table
                Dictionary<int, int> vendorContactMapping = new Dictionary<int, int>();
                HashSet<string> processedContacts = new HashSet<string>();

                for (int j = 0; j < dtLocation.Rows.Count; j++)
                {
                    string sessionContactPersonID = dtLocation.Rows[j]["ContactPerson"].ToString();

                    // Skip if we already processed this contact person
                    if (processedContacts.Contains(sessionContactPersonID))
                        continue;

                    processedContacts.Add(sessionContactPersonID);

                    int dbContactPersonID = contactPersonMapping[sessionContactPersonID];

                    _cm.VendorID = Convert.ToInt32(Session["VendorID"].ToString());
                    _cm.ContactPersonID = dbContactPersonID;
                    _cm.ContactMobile = dtLocation.Rows[j]["Mobile"].ToString();
                    _cm.ContactEmail = dtLocation.Rows[j]["Email"].ToString();
                    _cm.fax = dtLocation.Rows[j]["Fax"] == DBNull.Value ? null : dtLocation.Rows[j]["Fax"].ToString();

                    // Set Primary based on PersonAccountType from the location row
                    bool isPrimaryType = dtLocation.Rows[j]["PersonAccountType"].ToString().Equals("Primary", StringComparison.OrdinalIgnoreCase);
                    _cm.Primary = isPrimaryType ? "Yes" : "No";
                    _cm.PersonAccountType = dtLocation.Rows[j]["PersonAccountType"].ToString();
                    _cm.CreatedDate = DateTime.Now.AddHours(12.5);

                    // Save contact details - ONE entry per unique contact person
                    int vendorContactID = _cm.AddVendorContactDetails(_cm);
                    vendorContactMapping[dbContactPersonID] = vendorContactID;
                }

                // STEP 3: Save all locations
                for (int k = 0; k < dtLocation.Rows.Count; k++)
                {
                    string sessionContactPersonID = dtLocation.Rows[k]["ContactPerson"].ToString();
                    int dbContactPersonID = contactPersonMapping[sessionContactPersonID];

                    _cm.VendorID = Convert.ToInt32(Session["VendorID"].ToString());
                    _cm.VendorContactID = vendorContactMapping[dbContactPersonID]; // Use the saved VendorContactID
                    _cm.LocationType = dtLocation.Rows[k]["LocationType"].ToString();
                    _cm.AddressLine1 = dtLocation.Rows[k]["AddressLine1"].ToString();
                    _cm.AddressLine2 = dtLocation.Rows[k]["AddressLine2"].ToString();
                    _cm.AddressLine3 = dtLocation.Rows[k]["AddressLine3"].ToString();
                    _cm.City = dtLocation.Rows[k]["City"].ToString();
                    _cm.State = dtLocation.Rows[k]["State"].ToString();
                    _cm.Country = dtLocation.Rows[k]["Country"].ToString();
                    _cm.CreatedDate = DateTime.Now.AddHours(12.5);
                    _cm.AddVendorLocationDetails(_cm);
                }

                // Clear the mapping
                Session["ContactPersonMapping"] = null;
            }
        }

        private void SaveAllDocuments()
        {
            string reference = Session["Reference"].ToString();
            DataTable dt = _cm.GetVerificationReferenceDetail(reference);

            if (dt.Rows.Count > 0)
            {
                _cm.Reference = dt.Rows[0]["Reference"].ToString();
                _cm.VendorID = Convert.ToInt32(Session["VendorID"].ToString());

                // Save PAN document
                if (Session["PAN_File"] != null)
                {
                    SaveDocument(3, (byte[])Session["PAN_File"], Session["PAN_FileName"].ToString());
                }

                // Save Cancelled Cheque
                if (Session["CHQ_File"] != null)
                {
                    SaveDocument(10, (byte[])Session["CHQ_File"], Session["CHQ_FileName"].ToString());
                }

                // Save Registration document
                if (Session["REG_File"] != null)
                {
                    SaveDocument(5, (byte[])Session["REG_File"], Session["REG_FileName"].ToString());
                }

                // Save VAT document
                if (Session["GST_File"] != null)
                {
                    SaveDocument(2, (byte[])Session["GST_File"], Session["GST_FileName"].ToString());
                }

                // Save PF document
                if (Session["PF_File"] != null)
                {
                    SaveDocument(4, (byte[])Session["PF_File"], Session["PF_FileName"].ToString());
                }

                // Save UAN document
                if (Session["UAN_File"] != null)
                {
                    SaveDocument(8, (byte[])Session["UAN_File"], Session["UAN_FileName"].ToString());
                }

                // Save ESIC document
                if (Session["ESIC_File"] != null)
                {
                    SaveDocument(10, (byte[])Session["ESIC_File"], Session["ESIC_FileName"].ToString());
                }

                // Save Vendor Logo
                if (Session["LOGO_File"] != null)
                {
                    SaveDocument(9, (byte[])Session["LOGO_File"], Session["LOGO_FileName"].ToString());
                }
            }
        }

        private void SaveDocument(int documentTypeID, byte[] fileBytes, string fileName)
        {
            string reference = Session["Reference"].ToString();
            DataTable dt = _cm.GetVerificationReferenceDetail(reference);

            if (dt.Rows.Count > 0)
            {
                _cm.VendorID = Convert.ToInt32(Session["VendorID"].ToString());
                _cm.Reference = dt.Rows[0]["Reference"].ToString();
                string strpath = System.IO.Path.GetExtension(fileName);

                _cm.DocumentTypeID = documentTypeID;
                DataTable dtType = _cm.GetDocumentVendorDetails(_cm);
                DataTable dtFile = _cm.GetFilename(_cm);

                if (dtFile.Rows.Count > 0)
                {
                    _cm.Type = dtFile.Rows[0]["Filename"].ToString();
                    if (dtType.Rows.Count > 0)
                    {
                        if (dtType.Rows[0]["VersionNumber"].ToString() != "")
                        {
                            int VersionNumber = Convert.ToInt32(dtType.Rows[0]["VersionNumber"].ToString());
                            _cm.Version = VersionNumber + 1;
                        }
                        else
                        {
                            _cm.Version = 0;
                        }
                    }
                    else
                    {
                        _cm.Version = 0;
                    }
                }

                _cm.DocumentName = _cm.Reference + "-" + _cm.Version + "-" + _cm.Type + strpath;
                _cm.CreatedDate = DateTime.Now.AddHours(12.5);

                // Save file to server
                string filepath = Server.MapPath("//VendorDocument/" + _cm.DocumentName);
                System.IO.File.WriteAllBytes(filepath, fileBytes);

                // Add document record
                _cm.AddVendorDocument(_cm);
            }
        }
        protected void btnSubmitReference_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtReferenceNumber.Text))
            {
                txtReferenceNumber.BorderColor = System.Drawing.Color.Red;
            }
            else
            {
                string reference = txtReferenceNumber.Text.Trim();


                DataTable dtVendorCheck = _cm.CheckReferenceHasVendor(reference);

                if (dtVendorCheck.Rows.Count > 0)
                {

                    ScriptManager.RegisterStartupScript(this, GetType(), "showError",
                        "alert('Vendor already registered to this Reference. Please enter new Reference Number!');", true);
                    txtReferenceNumber.BorderColor = System.Drawing.Color.Red;
                    return;
                }


                DataTable dtCheck = _cm.GetVerificationDetails(reference);
                if (dtCheck.Rows.Count > 0)
                {
                    Session["Reference"] = dtCheck.Rows[0]["Reference"].ToString();
                    Session["Code"] = dtCheck.Rows[0]["Code"].ToString();
                    divVerify.Visible = false;
                    divContent.Visible = true;
                    divguide.Attributes["class"] = "tab-pane show active";
                    aguide.Attributes["class"] = "nav-link active";
                }
                else
                {
                    // Reference doesn't exist in verification table
                    ScriptManager.RegisterStartupScript(this, GetType(), "showError",
                        "alert('Invalid Reference Number!');", true);
                    txtReferenceNumber.BorderColor = System.Drawing.Color.Red;
                    return;
                }

                BindReference();
            }
        }
        protected void BindYears()
        {
            DateTime year1 = DateTime.Now.AddYears(-1);
            DateTime year2 = DateTime.Now.AddYears(-2);
            DateTime year3 = DateTime.Now.AddYears(-3);

            lblYearlyTurnover1.Text = year1.Year.ToString();
            lblYearlyTurnover2.Text = year2.Year.ToString();
            lblYearlyTurnover3.Text = year3.Year.ToString();
        }
        private void BindTypeofEstablishment()
        {
            DataTable dt = _cm.GetTypeofEstablishment();
            if (dt.Rows.Count > 0)
            {
                ddlTypeofEstablishment.DataSource = dt;
                ddlTypeofEstablishment.DataTextField = "TypeofEstablishment";
                ddlTypeofEstablishment.DataValueField = "EstablishmentID";
                ddlTypeofEstablishment.DataBind();
                ddlTypeofEstablishment.Items.Insert(0, new ListItem("Select Type of Establishment"));
            }

        }
        private void BindDealer()
        {
            DataTable dt = _cm.GetDealerDetails();
            if (dt.Rows.Count > 0)
            {
                ddlDealer.DataSource = dt;
                ddlDealer.DataTextField = "DealerName";
                ddlDealer.DataValueField = "DealerID";
                ddlDealer.DataBind();
                ddlDealer.Items.Insert(0, new ListItem("Select Dealer"));
            }
        }
        private void BindYear()
        {
            int year = DateTime.Now.Year;
            for (int i = 1960; i <= DateTime.Now.Year; i++)
            {
                ListItem li = new ListItem(i.ToString());
                ddlYearOfEstablishment.Items.Add(li);
            }
            ddlYearOfEstablishment.Items.Insert(0, new ListItem("Select Year of Establishment"));
        }
        protected void ddlTypeofEstablishment_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlTypeofEstablishment.SelectedIndex != 0)
            {
                if ((ddlTypeofEstablishment.SelectedValue == "1") || (ddlTypeofEstablishment.SelectedValue == "2"))
                {
                    divRegistrationNumber.Visible = true;
                }
                else
                {
                    divRegistrationNumber.Visible = false;
                }
            }
            BindReference();
        }
        protected void ddlYearOfEstablishment_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindReference();
            DateTime year1 = DateTime.Now.AddYears(-1);
            DateTime year2 = DateTime.Now.AddYears(-2);
            DateTime year3 = DateTime.Now.AddYears(-3);
            string y = (ddlYearOfEstablishment.SelectedItem.ToString() + "-01-01").ToString();
            DateTime Establishment = Convert.ToDateTime(ddlYearOfEstablishment.SelectedItem.ToString() + "-01-01");

            lblYearlyTurnover1.Text = year1.Year.ToString();
            lblYearlyTurnover2.Text = year2.Year.ToString();
            lblYearlyTurnover3.Text = year3.Year.ToString();

            string EstablishYear = Establishment.Year.ToString();
            if (EstablishYear == year1.Year.ToString())
            {
                txtYearlyTurnover1.ReadOnly = true;
                txtYearlyTurnover2.ReadOnly = true;
                txtYearlyTurnover3.ReadOnly = true;

                txtYearlyTurnover1.Text = "0";
                txtYearlyTurnover2.Text = "0";
                txtYearlyTurnover3.Text = "0";
            }
            else if (EstablishYear == year2.Year.ToString())
            {
                txtYearlyTurnover1.ReadOnly = false;
                txtYearlyTurnover2.ReadOnly = true;
                txtYearlyTurnover3.ReadOnly = true;

                txtYearlyTurnover1.Text = "";
                txtYearlyTurnover2.Text = "0";
                txtYearlyTurnover3.Text = "0";
            }
            else if (EstablishYear == year3.Year.ToString())
            {
                txtYearlyTurnover1.ReadOnly = false;
                txtYearlyTurnover2.ReadOnly = false;
                txtYearlyTurnover3.ReadOnly = true;

                txtYearlyTurnover1.Text = "";
                txtYearlyTurnover2.Text = "";
                txtYearlyTurnover3.Text = "0";
            }
            else if (EstablishYear == DateTime.Now.Year.ToString())
            {
                txtYearlyTurnover1.ReadOnly = true;
                txtYearlyTurnover2.ReadOnly = true;
                txtYearlyTurnover3.ReadOnly = true;

                txtYearlyTurnover1.Text = "0";
                txtYearlyTurnover2.Text = "0";
                txtYearlyTurnover3.Text = "0";
            }
            else
            {
                txtYearlyTurnover1.ReadOnly = false;
                txtYearlyTurnover2.ReadOnly = false;
                txtYearlyTurnover3.ReadOnly = false;

                txtYearlyTurnover1.Text = "";
                txtYearlyTurnover2.Text = "";
                txtYearlyTurnover3.Text = "";
            }
        }
        private void PopulateTreeview()
        {
            try
            {
                DataTable dtparent = _cm.FillParentTable();
                DataTable dtchild = _cm.FillChildTable();

                if (dtparent.Rows.Count > 0)
                {
                    tvTables.Nodes.Clear();
                    bool hasValidNodes = false;

                    foreach (DataRow masterRow in dtparent.Rows)
                    {
                        string purchaseGroupID = masterRow["purchaseGroupID"].ToString();

                        // Manually find child rows instead of using DataSet relations
                        DataRow[] childRows = dtchild.Select("purchaseGroupID = '" + purchaseGroupID + "'");

                        if (childRows.Length > 0)  // Only add parent if it has children
                        {
                            TreeNode masterNode = new TreeNode(
                                (string)masterRow["purchaseGroupName"],
                                purchaseGroupID);

                            tvTables.Nodes.Add(masterNode);
                            hasValidNodes = true;

                            foreach (DataRow childRow in childRows)
                            {
                                TreeNode childNode = new TreeNode(
                                    (string)childRow["purchaseSubGroupName"],
                                    childRow["purchaseGroupID"].ToString());

                                masterNode.ChildNodes.Add(childNode);
                                childNode.Value = childRow["purchaseSubGroupID"].ToString();
                            }
                        }
                    }

                    tvTables.CollapseAll();

                    // Show alert if no valid groups with subgroups found
                    if (!hasValidNodes)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                            "alert('No purchase groups with subgroups available.');", true);
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                        "alert('No purchase groups available.');", true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    "alert('Error loading treeview: " + ex.Message.Replace("'", "\\'") + "');", true);
                // Log the error but don't throw - allows page to continue

            }
        }
        protected void ddlVAT_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Ensure vendor code is maintained after postback
            BindReference();
        }

        private void Clear()
        {
            txtVendorCode.Text = "";
            txtVendorName.Text = "";
            ddlDealer.SelectedIndex = 0;
            // txtWebsite.Text = "";
            ddlVAT.SelectedIndex = 0;
            ddlYearOfEstablishment.SelectedIndex = 0;
            ddlTypeOfBussiness.SelectedIndex = 0;
            txtYearlyTurnover1.Text = "";
            txtRequiredCredit.Text = "";
            txtApprovedCredit.Text = "";
            ddlCompanySize.SelectedIndex = 0;
            ddlTypeOfBussiness.SelectedIndex = 0;
            txtReferenceNumber.Text = "";

            txtPF.Text = "";
            txtUANNumber.Text = "";
            txtESIC.Text = "";
            txtIEC.Text = "";
            txtYearlyTurnover2.Text = "";            //txtPincode.Text = "";
            txtYearlyTurnover3.Text = "";


            txtAccountName.Text = "";
            txtBankName.Text = "";
            txtBankBranch.Text = "";
            txtBankBranchCode.Text = "";
            txtAccountNumber.Text = "";
            txtIFSCode.Text = "";

            //txtFirstName.Text = "";
            //ddlTitle.SelectedIndex = 0;
            //txtMiddleName.Text = "";
            //txtLastName.Text = "";
            //txtDesignation.Text = "";
            //txtDepartment.Text = "";

            //gvPerson.DataSource = null;
            //gvPerson.DataBind();

            //gvLocation.DataSource = null;
            //gvLocation.DataBind();

            gvBank.DataSource = null;
            gvBank.DataBind();
            gvBank.DataBind();
        }
        protected void btnAddBank_Click(object sender, EventArgs e)
        {
            BindReference();
            int flag = 0;
            if (txtAccountName.Text == "")
            {
                txtAccountName.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else
            {
                txtAccountName.BorderColor = System.Drawing.Color.Empty;
            }
            if (txtBankName.Text == "")
            {
                txtBankName.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else
            {
                txtBankName.BorderColor = System.Drawing.Color.Empty;
            }
            if (txtBankBranch.Text == "")
            {
                txtBankBranch.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else
            {
                txtBankBranch.BorderColor = System.Drawing.Color.Empty;
            }
            if (txtBankBranchCode.Text == "")
            {
                txtBankBranchCode.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else
            {
                txtBankBranchCode.BorderColor = System.Drawing.Color.Empty;
            }
            //if (txtSwiftCode.Text == "")
            //{
            //    txtSwiftCode.BorderColor = System.Drawing.Color.Red;
            //    flag = 1;
            //}
            //else
            //{
            //    txtSwiftCode.BorderColor = System.Drawing.Color.Empty;
            //}
            if (txtAccountNumber.Text == "")
            {
                txtAccountNumber.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else
            {
                txtAccountNumber.BorderColor = System.Drawing.Color.Empty;
            }
            if (txtIFSCode.Text == "")
            {
                txtIFSCode.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else
            {
                txtIFSCode.BorderColor = System.Drawing.Color.Empty;
            }
            if (flag == 0)
            {
                DataTable dt = new DataTable();
                if (Session["DTBank"] != null)
                {
                    dt = (DataTable)Session["DTBank"];
                }
                else
                {
                    dt.Columns.Add("AccountName");
                    dt.Columns.Add("BankName");
                    dt.Columns.Add("BankBranch");
                    dt.Columns.Add("BankBranchCode");
                    dt.Columns.Add("AccountNumber");
                    dt.Columns.Add("IFSCode");
                    dt.Columns.Add("SwiftCode");
                    // dt.Columns.Add("CancelCheque");
                }
                bool isDuplicate = dt.AsEnumerable().Any(row =>
               row.Field<string>("AccountName").Equals(txtAccountName.Text.Trim(), StringComparison.OrdinalIgnoreCase) &&
               row.Field<string>("BankName").Equals(txtBankName.Text.Trim(), StringComparison.OrdinalIgnoreCase) &&
               row.Field<string>("BankBranch").Equals(txtBankBranch.Text.Trim(), StringComparison.OrdinalIgnoreCase) &&
               row.Field<string>("BankBranchCode").Equals(txtBankBranchCode.Text.Trim(), StringComparison.OrdinalIgnoreCase) &&
               row.Field<string>("AccountNumber").Equals(txtAccountNumber.Text.Trim(), StringComparison.OrdinalIgnoreCase) &&
               row.Field<string>("IFSCode").Equals(txtIFSCode.Text.Trim(), StringComparison.OrdinalIgnoreCase) &&
               row.Field<string>("SwiftCode").Equals(txtSwiftCode.Text.Trim(), StringComparison.OrdinalIgnoreCase)
           );

                if (isDuplicate)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertDuplicateBank", "alert('A bank account with the same details already exists.');", true);
                    return;
                }

                DataRow dr = dt.NewRow();

                dr["AccountName"] = txtAccountName.Text.ToString();
                dr["BankName"] = txtBankName.Text.ToString();
                dr["BankBranch"] = txtBankBranch.Text.ToString();
                dr["BankBranchCode"] = txtBankBranchCode.Text.ToString();
                dr["AccountNumber"] = txtAccountNumber.Text.ToString();
                dr["IFSCode"] = txtIFSCode.Text.ToString();
                dr["SwiftCode"] = txtSwiftCode.Text.ToString();

                dt.Rows.Add(dr);

                gvBank.DataSource = dt;
                gvBank.DataBind();

                if (gvBank.Rows.Count == 0)
                {
                    btnSaveBank.Visible = false;
                }
                else
                {
                    btnSaveBank.Visible = true;
                }
                Session["DTBank"] = dt;
                //ClearBank();
            }
        }
        private void ClearBank()
        {
            txtAccountName.Text = "";
            txtBankName.Text = "";
            txtBankBranch.Text = "";
            txtBankBranchCode.Text = "";
            txtAccountNumber.Text = "";
            txtIFSCode.Text = "";
            txtSwiftCode.Text = "";
        }
        protected void gvBank_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "del")
            {
                int index = int.Parse(e.CommandArgument.ToString());
                DataTable dt = (DataTable)Session["DTBank"];
                dt.Rows[index].Delete();
                dt.AcceptChanges();
                gvBank.DataSource = dt;
                gvBank.DataBind();
                if (gvBank.Rows.Count == 0)
                {
                    btnSaveBank.Visible = false;
                }
                else
                {
                    btnSaveBank.Visible = true;
                }
            }
        }
        protected void btnAddContactPerson_Click(object sender, EventArgs e)
        {
            BindReference();
            int flag = 1;


            if (ddlTitle.SelectedIndex == 0)
            {
                ddlTitle.BorderColor = System.Drawing.Color.Red;
                flag = 0;
            }
            else
            {
                ddlTitle.BorderColor = System.Drawing.Color.Empty;
            }

            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                txtFirstName.BorderColor = System.Drawing.Color.Red;
                flag = 0;
            }
            else
            {
                txtFirstName.BorderColor = System.Drawing.Color.Empty;
            }

            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                txtLastName.BorderColor = System.Drawing.Color.Red;
                flag = 0;
            }
            else
            {
                txtLastName.BorderColor = System.Drawing.Color.Empty;
            }

            //if (string.IsNullOrWhiteSpace(txtDepartment.Text))
            //{
            //    txtDepartment.BorderColor = System.Drawing.Color.Red;
            //    flag = 0;
            //}
            //else
            //{
            //    txtDepartment.BorderColor = System.Drawing.Color.Empty;
            //}

            //if (string.IsNullOrWhiteSpace(txtDesignation.Text))
            //{
            //    txtDesignation.BorderColor = System.Drawing.Color.Red;
            //    flag = 0;
            //}
            //else
            //{
            //    txtDesignation.BorderColor = System.Drawing.Color.Empty;
            //}

            if (ddlPersonAccountType.SelectedIndex == 0 || ddlPersonAccountType.SelectedValue == "")
            {
                ddlPersonAccountType.BorderColor = System.Drawing.Color.Red;
                flag = 0;
            }
            else
            {
                ddlPersonAccountType.BorderColor = System.Drawing.Color.Empty;
            }

            if (flag == 1)
            {
                lblDuplicateContactPerson.Visible = false;
                DataTable dt = new DataTable();

                if (Session["DTPerson"] != null)
                {
                    dt = (DataTable)Session["DTPerson"];
                }
                else
                {
                    dt.Columns.Add("ID");
                    dt.Columns.Add("Title");
                    dt.Columns.Add("FirstName");
                    dt.Columns.Add("MiddleName");
                    dt.Columns.Add("LastName");
                    dt.Columns.Add("Designation");
                    dt.Columns.Add("Department");
                    dt.Columns.Add("PersonAccountType");
                }


                bool isPrimaryExists = dt.AsEnumerable()
                    .Any(row => row.Field<string>("PersonAccountType") == "Primary");

                if (ddlPersonAccountType.SelectedItem.Text == "Primary" && isPrimaryExists)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertPrimary", "alert('Only one Primary Contact is allowed!');", true);
                    return;
                }
                bool isDuplicateCombination = dt.AsEnumerable().Any(row =>
    row.Field<string>("FirstName").Equals(txtFirstName.Text.Trim(), StringComparison.OrdinalIgnoreCase)
//row.Field<string>("LastName").Equals(txtLastName.Text.Trim(), StringComparison.OrdinalIgnoreCase) &&
//row.Field<string>("Department").Equals(txtDepartment.Text.Trim(), StringComparison.OrdinalIgnoreCase) &&
//row.Field<string>("Designation").Equals(txtDesignation.Text.Trim(), StringComparison.OrdinalIgnoreCase)
);

                if (isDuplicateCombination)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertDuplicate", "alert('A contact with the same  Name already exists.');", true);
                    return;
                }

                //bool duplicateDesignationDept = dt.AsEnumerable().Any(row =>
                //    row.Field<string>("Designation").Equals(txtDesignation.Text.Trim(), StringComparison.OrdinalIgnoreCase) &&
                //    row.Field<string>("Department").Equals(txtDepartment.Text.Trim(), StringComparison.OrdinalIgnoreCase)
                //);

                //if (duplicateDesignationDept)
                //{
                //    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertDuplicateDesDept", "alert('A contact with the same Designation and Department already exists.');", true);
                //    return;
                //}

                DataRow dr = dt.NewRow();
                int count = Convert.ToInt32(hdnContactPerson.Value);
                count++;
                hdnContactPerson.Value = count.ToString();

                dr["ID"] = count.ToString();
                dr["Title"] = ddlTitle.SelectedItem.Text;
                dr["FirstName"] = txtFirstName.Text.Trim();
                dr["MiddleName"] = txtMiddleName.Text.Trim();
                dr["LastName"] = txtLastName.Text.Trim();
                dr["Designation"] = txtDesignation.Text.Trim();
                dr["Department"] = txtDepartment.Text.Trim();
                dr["PersonAccountType"] = ddlPersonAccountType.SelectedItem.Text;

                dt.Rows.Add(dr);
                gvPerson.DataSource = dt;
                gvPerson.DataBind();

                btnSaveContactPerson.Visible = (gvPerson.Rows.Count > 0);
                Session["DTPerson"] = dt;
                BindContactPerson();
            }
        }
        protected void ddlLocationType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlLocationType.SelectedValue == "manufacture")
            {
                if (gvLocation.Rows.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Registered Office Address is Required to add Manufacturing Address')", true);
                    divLocationType.Visible = false;
                    ddlLocationType.SelectedIndex = 0;
                }
                else
                {
                    divLocationType.Visible = true;
                    //ddlLocationType.Items.FindByValue("manufacture").Selected = true;
                    if (ddlLocationType.Items.FindByValue("manufacture") != null)
                        ddlLocationType.Items.FindByValue("manufacture").Selected = true;

                }
            }
            if (ddlLocationType.SelectedValue == "registered")
            {
                if (gvLocation.Rows.Count == 0)
                    divLocationType.Visible = false;

                if (gvLocation.Rows.Count == 1)
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Registered Office Address Already added.')", true);
            }
        }
        //protected void rblLocationType_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    string Value = rblLocationType.SelectedValue.ToString();
        //    if (Value == "Yes")
        //    {
        //        if (Session["DTLocation"] != null)
        //        {
        //            DataTable dt = (DataTable)Session["DTLocation"];
        //            foreach (DataRow dr1 in dt.Rows)
        //            {
        //                if (dr1["LocationType"].ToString() == "Registered Office")
        //                {
        //                    // Copy ONLY address fields - NOT mobile, email, fax
        //                    txtAddressLine1.Text = dr1["AddressLine1"].ToString();
        //                    txtAddressLine2.Text = dr1["AddressLine2"].ToString();
        //                    txtAddressLine3.Text = dr1["AddressLine3"].ToString();
        //                    txtCity.Text = dr1["City"].ToString();
        //                    txtState.Text = dr1["State"].ToString();

        //                    // Set the country dropdown
        //                    string country = dr1["Country"].ToString();
        //                    if (ddlCountry.Items.FindByText(country) != null)
        //                    {
        //                        ddlCountry.SelectedIndex = ddlCountry.Items.IndexOf(ddlCountry.Items.FindByText(country));
        //                    }

        //                    // DON'T touch Mobile, Email, Fax, Contact Person
        //                    // User will select contact person manually from dropdown
        //                    // User will enter mobile, email, fax manually
        //                }
        //                break;
        //            }
        //        }
        //    }
        //    else if (Value == "No")
        //    {
        //        // Clear all fields when user selects "No"
        //        txtAddressLine1.Text = "";
        //        txtAddressLine2.Text = "";
        //        txtAddressLine3.Text = "";
        //        txtCity.Text = "";
        //        txtState.Text = "";
        //        ddlCountry.SelectedIndex = 0;
        //    }
        //}
        protected void rblLocationType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string Value = rblLocationType.SelectedValue.ToString();
            if (Value == "Yes")
            {
                if (Session["DTLocation"] != null)
                {
                    DataTable dt = (DataTable)Session["DTLocation"];
                    foreach (DataRow dr1 in dt.Rows)
                    {
                        if (dr1["LocationType"].ToString() == "Registered Office")
                        {
                            // Copy ONLY address fields - NOT mobile, email, fax
                            txtAddressLine1.Text = dr1["AddressLine1"].ToString();
                            txtAddressLine2.Text = dr1["AddressLine2"].ToString();
                            txtAddressLine3.Text = dr1["AddressLine3"].ToString();
                            txtCity.Text = dr1["City"].ToString();
                            txtState.Text = dr1["State"].ToString();

                            // Set the country dropdown
                            string country = dr1["Country"].ToString();
                            if (ddlCountry.Items.FindByText(country) != null)
                            {
                                ddlCountry.SelectedIndex = ddlCountry.Items.IndexOf(ddlCountry.Items.FindByText(country));
                            }

                            // DON'T touch Mobile, Email, Fax, Contact Person
                            // User will select contact person manually from dropdown
                            // User will enter mobile, email, fax manually
                        }
                        break;
                    }
                }
            }
            else if (Value == "No")
            {
                // Clear all fields when user selects "No"
                txtAddressLine1.Text = "";
                txtAddressLine2.Text = "";
                txtAddressLine3.Text = "";
                txtCity.Text = "";
                txtState.Text = "";
                ddlCountry.SelectedIndex = 0;
            }
        }
        protected void ddlCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCountry.SelectedIndex != 0)
            {
                _cm.CountryCode = ddlCountry.SelectedValue.ToString();
                txtCountryCode.Text = "+" + _cm.CountryCode;
            }
        }
        protected void gvPerson_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                RadioButton rbtn = (RadioButton)e.Row.FindControl("rbtnPrimary");
                DataRowView row = (DataRowView)e.Row.DataItem;
                rbtn.Checked = Convert.ToBoolean(row["PrimaryContact"]);
            }
        }
        protected void btnAddLocation_Click(object sender, EventArgs e)
        {
            BindReference();
            int flag = 0;
            int count = 0;
            if (txtAddressLine1.Text == "")
            {
                txtAddressLine1.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else
            {
                txtAddressLine1.BorderColor = System.Drawing.Color.Empty;
            }
            if (txtMobile.Text == "")
            {
                txtMobile.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else
            {
                txtMobile.BorderColor = System.Drawing.Color.Empty;
            }
            if (txtEmail.Text == "")
            {
                txtEmail.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else
            {
                txtEmail.BorderColor = System.Drawing.Color.Empty;
            }
            //if (txtfax.Text == "")
            //{
            //    txtfax.BorderColor = System.Drawing.Color.Red;
            //    flag = 1;
            //}
            //else
            //{
            //    txtfax.BorderColor = System.Drawing.Color.Empty;
            //}
            if (ddlLocationType.SelectedIndex == 0)
            {
                ddlLocationType.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else
            {
                ddlLocationType.BorderColor = System.Drawing.Color.Empty;
            }
            if (ddlContactPerson1.SelectedIndex == 0)
            {
                ddlContactPerson1.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else
            {
                ddlContactPerson1.BorderColor = System.Drawing.Color.Empty;
            }
            if (ddlCountry.SelectedIndex == 0)
            {
                ddlCountry.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else
            {
                ddlCountry.BorderColor = System.Drawing.Color.Empty;
            }
            if (txtState.Text == "")
            {
                txtState.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else
            {
                txtState.BorderColor = System.Drawing.Color.Empty;
            }
            if (txtCity.Text == "")
            {
                txtCity.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else
            {
                txtCity.BorderColor = System.Drawing.Color.Empty;
            }

            if (txtEmail.Text == "")
            {
                txtEmail.BorderColor = System.Drawing.Color.Red;
                flag = 1;
            }
            else
            {
                txtEmail.BorderColor = System.Drawing.Color.Empty;
            }
            //string emailID = txtEmail.Text.ToString();
            //string[] emailList = emailID.Split(',');

            //foreach (string email in emailList)
            //{
            //    int currentVendorId = Session["VendorID"] != null ? Convert.ToInt32(Session["VendorID"]) : 0;

            //    if (_cm.IsEmailExistsForOtherVendors(email.Trim(), currentVendorId))
            //    {
            //        txtEmail.BorderColor = System.Drawing.Color.Red;
            //        flag = 1;
            //        txtEmail.Focus();
            //        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Email " + email.Trim() + " already exists for another vendor!');", true);
            //        return;
            //    }

            //    Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            //    Match match = regex.Match(email);
            //    if (match.Success)
            //    {
            //        count++;
            //        txtEmail.BorderColor = System.Drawing.Color.Empty;
            //    }
            //    else
            //    {
            //        txtEmail.BorderColor = System.Drawing.Color.Red;
            //        flag = 1;
            //        txtEmail.Focus();
            //        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Enter valid email Address(s)!');", true);
            //    }
            //}



            if (flag == 0)
            {
                //if (count >= 2)
                //{
                DataTable dt = new DataTable();
                if (Session["DTLocation"] != null)
                {
                    dt = (DataTable)Session["DTLocation"];
                }
                else
                {
                    dt.Columns.Add("LocationType");
                    dt.Columns.Add("ContactPerson");
                    dt.Columns.Add("Mobile");
                    dt.Columns.Add("Email");
                    dt.Columns.Add("Fax");
                    dt.Columns.Add("AddressLine1");
                    dt.Columns.Add("AddressLine2");
                    dt.Columns.Add("AddressLine3");
                    dt.Columns.Add("City");
                    dt.Columns.Add("State");
                    dt.Columns.Add("Country");
                    dt.Columns.Add("ContactPersonName");
                    dt.Columns.Add("PersonAccountType");
                }
                // In btnAddLocation_Click method, replace this section:

                DataRow dr = dt.NewRow();
                dr["LocationType"] = ddlLocationType.SelectedItem.ToString();
                dr["ContactPerson"] = ddlContactPerson1.SelectedValue.ToString();
                dr["Mobile"] = txtCountryCode.Text.ToString() + " " + txtMobile.Text.ToString();
                dr["Email"] = txtEmail.Text.ToString();
                dr["Fax"] = txtfax.Text.ToString();
                dr["AddressLine1"] = txtAddressLine1.Text.ToString();
                dr["AddressLine2"] = txtAddressLine2.Text.ToString();
                dr["AddressLine3"] = txtAddressLine3.Text.ToString();
                dr["City"] = txtCity.Text.ToString();
                dr["State"] = txtState.Text.ToString();
                dr["Country"] = ddlCountry.SelectedItem.ToString();
                dr["ContactPersonName"] = ddlContactPerson1.SelectedItem.ToString();

                // FIX: Get PersonAccountType from the selected contact person's data
                string personAccountType = "Secondary"; // Default
                if (Session["DTPerson"] != null)
                {
                    DataTable dtPerson = (DataTable)Session["DTPerson"];
                    string selectedContactPersonID = ddlContactPerson1.SelectedValue.ToString();

                    foreach (DataRow personRow in dtPerson.Rows)
                    {
                        if (personRow["ID"].ToString() == selectedContactPersonID)
                        {
                            personAccountType = personRow["PersonAccountType"].ToString();
                            break;
                        }
                    }
                }

                dr["PersonAccountType"] = personAccountType;
                if (gvLocation.Rows.Count > 0)
                {
                    for (int rows = 0; rows < gvLocation.Rows.Count; rows++)
                    {
                        bool duplicateRow = true;
                        if (gvLocation.Rows[rows].Cells[1].Text == "Registered Office")
                        {
                            if (gvLocation.Rows[rows].Cells[1].Text == ddlLocationType.SelectedItem.ToString())
                            {
                                lblDuplicateData.Visible = true;
                            }
                            else
                            {
                                duplicateRow = false;
                                dt.Rows.Add(dr);
                                gvLocation.DataSource = dt;
                                gvLocation.DataBind();
                                lblDuplicateData.Visible = false;
                                break;
                            }
                        }
                        else
                        {

                        }
                    }
                }
                else
                {
                    dt.Rows.Add(dr);
                    gvLocation.DataSource = dt;
                    gvLocation.DataBind();
                }
                if (gvLocation.Rows.Count >= 2)
                {
                    GetEmailDetails(this.gvLocation);
                }
                else
                {
                    GetEmailDetails(this.gvLocation);
                }
                Session["DTLocation"] = dt;
            }
            else
            {
                // ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Two or more Email Addresses Required.');", true);

            }
            //}
            //else
            //{
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please fill highlighted fields');", true);
            //}
        }
        private void ClearLocationFields()
        {
            // If you have location input controls on the page (outside session-based logic), reset them here
            txtAddressLine1.Text = "";
            txtAddressLine2.Text = "";
            txtAddressLine3.Text = " ";
            txtCity.Text = "";
            txtState.Text = "";
            ddlCountry.SelectedIndex = 0;
            txtEmail.Text = "";
            txtPincode.Text = "";
            txtMobile.Text = "";
            ddlLocationType.SelectedIndex = 0;
            ddlContactPerson1.SelectedIndex = 0;

            Session["DTPerson"] = null;
            Session["DTLocation"] = null;

            // Clear GridView by resetting its DataSource and rebinding
            gvLocation.DataSource = null;
            gvLocation.DataBind();
            // Optional: Rebind grids or controls after session is cleared
            // BindLocationGrid();
        }
        private void ClearForm()
        {
            txtVendorName.Text = "";
            ddlDealer.SelectedIndex = 0;
            txtWebsite.Text = "";
            ddlVAT.SelectedIndex = 0;
            txtIEC.Text = "";
            txtESIC.Text = "";
            ddlYearOfEstablishment.SelectedIndex = 0;
            ddlTypeOfBussiness.SelectedIndex = 0;
            ddlTypeofEstablishment.SelectedIndex = 0;
            txtPincode.Text = "";
            txtRegistrationNumber.Text = "";
            txtYearlyTurnover1.Text = "";
            txtYearlyTurnover2.Text = "";
            txtYearlyTurnover3.Text = "";
            txtUANNumber.Text = "";
            ddlCompanySize.SelectedIndex = 0;
            txtApprovedCredit.Text = "";
            txtVendorCode.Text = "";

            // Clear any dropdown/treeview selection if needed
            foreach (TreeNode node in tvTables.Nodes)
            {
                node.Checked = false;
                foreach (TreeNode child in node.ChildNodes)
                {
                    child.Checked = false;
                }
            }
        }
        protected void gvPerson_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "del")
            {
                int index = int.Parse(e.CommandArgument.ToString());
                DataTable dt = (DataTable)Session["DTPerson"];
                dt.Rows[index].Delete();
                dt.AcceptChanges();
                gvPerson.DataSource = dt;
                gvPerson.DataBind();
                Session["DTPerson"] = dt;


                BindContactPerson();
                if (gvPerson.Rows.Count == 0)
                {
                    btnSaveContactPerson.Visible = false;
                    lblDuplicateContactPerson.Visible = false;
                }
                else
                {
                    btnSaveContactPerson.Visible = true;
                }
            }
        }
        protected void BindContactPerson()
        {
            if (Session["DTPerson"] != null)
            {
                DataTable dt = (DataTable)Session["DTPerson"];
                if (dt.Rows.Count > 0)
                {
                    ddlContactPerson1.DataSource = dt;
                    ddlContactPerson1.DataTextField = "FirstName";
                    ddlContactPerson1.DataValueField = "ID";
                    ddlContactPerson1.DataBind();
                    ddlContactPerson1.Items.Insert(0, new ListItem("Select Contact Person"));
                }
            }
        }
        protected void gvLocation_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "del")
            {
                int index = int.Parse(e.CommandArgument.ToString());
                DataTable dt = (DataTable)Session["DTLocation"];
                dt.Rows[index].Delete();
                dt.AcceptChanges();


                gvLocation.DataSource = dt;
                gvLocation.DataBind();

                Session["DTLocation"] = dt;
                if (gvLocation.Rows.Count >= 2)
                {
                    GetEmailDetails(this.gvLocation);
                }
                else
                {
                    GetEmailDetails(this.gvLocation);
                }
            }
        }
        private void GetEmailDetails(GridView gvLocation)
        {
            int count = 0;
            for (int i = 0; i < gvLocation.Rows.Count; i++)
            {
                GridViewRow row = gvLocation.Rows[i];
                if (row.Cells[4].Text != "")
                {
                    count++;
                }
                if (count >= 2)
                {
                    lblSaveData.Visible = false;
                    btnSaveLocation.Visible = true;
                }
                else
                {
                    lblSaveData.Visible = true;
                    btnSaveLocation.Visible = false;
                }
            }
        }
        protected void chkPrimary_CheckedChanged(object sender, EventArgs e)
        {
            GridViewRow row = (GridViewRow)(((CheckBox)sender).NamingContainer);
            CheckBox chkSelect = (CheckBox)sender;
            if (chkSelect.Checked)
            {

            }
        }
        private bool IsValidExtension(string filename)
        {
            string[] allowed = { ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx" };
            string ext = System.IO.Path.GetExtension(filename).ToLower();
            return allowed.Contains(ext);
        }

        protected void chkAccept_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAccept.Checked == true)
            {
                btnFinish.Enabled = true;
            }
            else
            {
                btnFinish.Enabled = false;
            }
        }
        protected void PreviousTab(string tabName)
        {
            // Reset all tab panes
            divguide.Attributes["class"] = "tab-pane fade";
            divhome.Attributes["class"] = "tab-pane fade";
            divbank.Attributes["class"] = "tab-pane fade";
            divperson.Attributes["class"] = "tab-pane fade";
            divlocation.Attributes["class"] = "tab-pane fade";
            divfiles.Attributes["class"] = "tab-pane fade";

            // Reset all nav links
            aguide.Attributes["class"] = "nav-link";
            ahome.Attributes["class"] = "nav-link";
            abank.Attributes["class"] = "nav-link";
            aperson.Attributes["class"] = "nav-link";
            alocation.Attributes["class"] = "nav-link";
            afiles.Attributes["class"] = "nav-link";
            afiles.Attributes["class"] = "nav-link";

            // Activate the selected tab
            switch (tabName)
            {
                case "Guide":
                    divguide.Attributes["class"] = "tab-pane show active";
                    aguide.Attributes["class"] = "nav-link active";
                    break;

                case "Home":
                    divhome.Attributes["class"] = "tab-pane show active";
                    ahome.Attributes["class"] = "nav-link active show";
                    break;

                case "Bank":
                    divbank.Attributes["class"] = "tab-pane show active";
                    abank.Attributes["class"] = "nav-link active show";
                    break;
                case "Person":
                    divperson.Attributes["class"] = "tab-pane show active";
                    aperson.Attributes["class"] = "nav-link active";
                    break;

                case "Location":
                    divlocation.Attributes["class"] = "tab-pane show active";
                    alocation.Attributes["class"] = "nav-link active";
                    break;
            }
        }
        protected void btnPreviousVendor_Click(object sender, EventArgs e)
        {
            PreviousTab("Guid");
        }
        protected void btnPreviousbank_Click(object sender, EventArgs e)
        {
            PreviousTab("Home");
        }
        protected void btnPreviousContactPerson_Click(object sender, EventArgs e)
        {
            PreviousTab("Bank");
        }
        protected void btnPreviousLocation_Click(object sender, EventArgs e)
        {
            PreviousTab("Person");
        }
        protected void btnPreviousFile_Click(object sender, EventArgs e)
        {
            PreviousTab("Location");
        }
    }
}

//protected void btnPreviousLocation_Click(object sender, EventArgs e)
//{
//    PreviousTab("Location");
//}



//protected void btnPreviousLocation_Click(object sender, EventArgs e)
//{
//    PreviousTab("Location");
//}


