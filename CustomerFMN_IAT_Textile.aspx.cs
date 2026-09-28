using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class CustomerFMN_IAT_Textile : System.Web.UI.Page
    {
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["ID"] != null)
                {
                    hdInquiryId.Value = Request.QueryString["ID"];

                    int inquiryId;
                    if (int.TryParse(hdInquiryId.Value, out inquiryId))
                    {
                        using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
                        {
                            con.Open();

                            bool officerMode = CustomerCrfSampleExists(con, inquiryId);
                            hdIsOfficerMode.Value = officerMode ? "1" : "0";

                            if (officerMode)
                            {
                                LoadExistingBookingForm(con, inquiryId);
                            }
                            else
                            {
                                // Fresh customer submission - same behavior as before.
                                txtRequestDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

                                var company = IATReportHelper.FetchCompanyContactDetails(con, inquiryId);
                                BindContactPerson(con, inquiryId);
                                txtApplicantName.Text = company.CompanyName;
                                txtAddress.Text = company.Address;
                                //txtContactPerson.Text = company.Contactperson;
                                txtEmail.Text = company.Email;
                                txtPhone.Text = company.Phone;
                                txtFax.Text = company.Fax;

                                hdCustomerMasterID.Value = company.CustomerMasterId;
                                hdPersonID.Value = company.PersonId;
                            }
                        }
                    }
                }
                else
                {
                    Response.Redirect("Login.aspx");
                    return;
                }
            }

            bool isOfficer = hdIsOfficerMode.Value == "1";
            SetControlsEnabled(pnlOfficerSection, isOfficer);
            lblOfficerLocked.Visible = !isOfficer;
            btnSubmit.Text = isOfficer ? "Save Officer Amendments" : "Submit";
        }

        private void BindContactPerson(SqlConnection con, int inquiryId)
        {
            DataTable dt = _cm.BindContactPerson(inquiryId);

            ddlContactPerson.Items.Clear();
            ddlContactPerson.Items.Add(new ListItem("Select Person", ""));

            if (dt != null && dt.Rows.Count > 0)
            {
                ddlContactPerson.DataSource = dt;
                ddlContactPerson.DataTextField = "CustomerName";
                ddlContactPerson.DataValueField = "PersonId";
                ddlContactPerson.DataBind();

                ddlContactPerson.Items.Insert(0, new ListItem("Select Person", ""));
            }
        }
        protected void ddlContactPerson_SelectedIndexChanged(object sender, EventArgs e)
        {
            int personId = 0;

            if (ddlContactPerson.SelectedIndex != 0)
            {
                personId = Convert.ToInt32(ddlContactPerson.SelectedItem.Value);

                DataTable dt = _cm.BindCustomerContactPerson(personId);
                if (dt.Rows.Count > 0)
                {
                    txtPhone.Text = dt.Rows[0]["Mobile"].ToString();
                    txtEmail.Text = dt.Rows[0]["Email"].ToString();
                    txtFax.Text = dt.Rows[0]["Fax"].ToString();
                }

                hdPersonID.Value = personId.ToString();
            }
            else
            {
                txtPhone.Text = "";
                txtEmail.Text = "";
                txtFax.Text = "";
                hdPersonID.Value = "";
            }
        }
        protected void radReferenceProvided_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlReference.Visible = (radReferenceProvided.SelectedValue == "Yes");
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            lblMessage.Text = string.Empty;

            if (!Page.IsValid) return;

            if (string.IsNullOrWhiteSpace(txtApplicantName.Text) ||
               ddlContactPerson.SelectedIndex <= 0 ||
               string.IsNullOrWhiteSpace(txtRequestDate.Text))
            {
                lblMessage.Text = "Please fill in Applicant (Company Name), Contact Person and Date.";
                return;
            }

            int inquiryId = 0;
            int.TryParse(hdInquiryId.Value, out inquiryId);

            bool isOfficerMode = hdIsOfficerMode.Value == "1";

            if (isOfficerMode)
            {
                if (string.IsNullOrWhiteSpace(txtInspectionLocation.Text) ||
                    string.IsNullOrWhiteSpace(txtFactorySupplierName.Text))
                {
                    lblMessage.Text = "Please fill in Inspection Location and Factory / Supplier Name.";
                    return;
                }

                SaveOfficerAmendments(inquiryId);
                Response.Redirect("CreateRequestCustomerFM_Textile.aspx?ID=" + inquiryId);
                return;
            }

            int organizationId = Session["OrganizationID"] != null ? Convert.ToInt32(Session["OrganizationID"]) : 1;
            int userId = Session["UserID"] != null ? Convert.ToInt32(Session["UserID"]) : 0;
            int customerId = Session["CustomerId"] != null ? Convert.ToInt32(Session["CustomerId"]) : 0;
            int customerMasterId = 0;
            int.TryParse(hdCustomerMasterID.Value, out customerMasterId);
            int personId = 0;
            int.TryParse(hdPersonID.Value, out personId);

            int requestId = inquiryId;
            int additionalDetailsId;

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
            {
                con.Open();

                using (SqlTransaction tran = con.BeginTransaction())
                {
                    try
                    {
                        var common = new DAL.IATCommonFields
                        {
                            BookingFormNo = string.IsNullOrWhiteSpace(txtBookingNo.Text) ? null : txtBookingNo.Text.Trim(),
                            RequestDate = string.IsNullOrWhiteSpace(txtRequestDate.Text) ? (DateTime?)null : Convert.ToDateTime(txtRequestDate.Text),
                            Attn = string.IsNullOrWhiteSpace(txtAttn.Text) ? null : txtAttn.Text.Trim(),
                            ApplicantEmail = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                            ApplicantFax = string.IsNullOrWhiteSpace(txtFax.Text) ? null : txtFax.Text.Trim(),
                            CC = string.IsNullOrWhiteSpace(txtCC.Text) ? null : txtCC.Text.Trim(),
                            OtherInstructions = string.IsNullOrWhiteSpace(txtOtherInstructions.Text) ? null : txtOtherInstructions.Text.Trim(),
                            ServiceRequired = string.IsNullOrWhiteSpace(txtCommodity.Text) ? null : txtCommodity.Text.Trim(),

                            ReferenceProvided = radReferenceProvided.SelectedValue == "Yes" ? "Yes" : "No",
                            RefOther = (radReferenceProvided.SelectedValue == "Yes" && chkRefOther.Checked) ? "Yes" : "No",
                            OtherReferenceText = (radReferenceProvided.SelectedValue == "Yes" && chkRefOther.Checked && !string.IsNullOrWhiteSpace(txtOtherReference.Text))
                    ? txtOtherReference.Text.Trim() : null,

                            // Officer section is locked for the customer at this point, so these stay null here
                            // and get filled in by SaveOfficerAmendments() on the delete+reinsert path.
                            InspectionLocation = string.IsNullOrWhiteSpace(txtInspectionLocation.Text) ? null : txtInspectionLocation.Text.Trim(),
                            RequestedInspectionDate = string.IsNullOrWhiteSpace(txtRequestedInspectionDate.Text) ? (DateTime?)null : Convert.ToDateTime(txtRequestedInspectionDate.Text),
                            PreviousReportNo = string.IsNullOrWhiteSpace(txtPreviousReportNo.Text) ? null : txtPreviousReportNo.Text.Trim()
                        };

                        var deptFields = new DAL.TextileDeptFields
                        {
                            BuyerName = string.IsNullOrWhiteSpace(txtBuyerName.Text) ? null : txtBuyerName.Text.Trim(),
                            ProductDescription = string.IsNullOrWhiteSpace(txtProductDescription.Text) ? null : txtProductDescription.Text.Trim(),
                            ShipmentQty = string.IsNullOrWhiteSpace(txtShipmentQty.Text) ? null : txtShipmentQty.Text.Trim(),
                            PercentReady = string.IsNullOrWhiteSpace(txtPercentReady.Text) ? null : txtPercentReady.Text.Trim(),
                            ShippingDate = string.IsNullOrWhiteSpace(txtShippingDate.Text) ? (DateTime?)null : Convert.ToDateTime(txtShippingDate.Text),

                            RefApprovedSamples = (radReferenceProvided.SelectedValue == "Yes" && chkRefApprovedSamples.Checked) ? "Yes" : "No",
                            RefSwatchCard = (radReferenceProvided.SelectedValue == "Yes" && chkRefSwatchCard.Checked) ? "Yes" : "No",
                            RefLCCopy = (radReferenceProvided.SelectedValue == "Yes" && chkRefLCCopy.Checked) ? "Yes" : "No",
                            RefCertificate = (radReferenceProvided.SelectedValue == "Yes" && chkRefCertificate.Checked) ? "Yes" : "No",
                            RefProductSpec = (radReferenceProvided.SelectedValue == "Yes" && chkRefProductSpec.Checked) ? "Yes" : "No",
                            RefPOCopy = (radReferenceProvided.SelectedValue == "Yes" && chkRefPOCopy.Checked) ? "Yes" : "No",
                            RefClientInstructionSheet = (radReferenceProvided.SelectedValue == "Yes" && chkRefClientInstructionSheet.Checked) ? "Yes" : "No",
                            RefConstructionDataForm = (radReferenceProvided.SelectedValue == "Yes" && chkRefConstructionDataForm.Checked) ? "Yes" : "No",
                            RefDigitalImages = (radReferenceProvided.SelectedValue == "Yes" && chkRefDigitalImages.Checked) ? "Yes" : "No",
                            RefPackingListInvoice = (radReferenceProvided.SelectedValue == "Yes" && chkRefPackingListInvoice.Checked) ? "Yes" : "No",
                            RefRoadMap = (radReferenceProvided.SelectedValue == "Yes" && chkRefRoadMap.Checked) ? "Yes" : "No",

                            FactorySupplierName = string.IsNullOrWhiteSpace(txtFactorySupplierName.Text) ? null : txtFactorySupplierName.Text.Trim(),
                            FactoryAddress = string.IsNullOrWhiteSpace(txtFactoryAddress.Text) ? null : txtFactoryAddress.Text.Trim(),
                            FactoryContactPerson = string.IsNullOrWhiteSpace(txtFactoryContactPerson.Text) ? null : txtFactoryContactPerson.Text.Trim(),
                            FactoryPhone = string.IsNullOrWhiteSpace(txtFactoryPhone.Text) ? null : txtFactoryPhone.Text.Trim(),
                            FactoryFax = string.IsNullOrWhiteSpace(txtFactoryFax.Text) ? null : txtFactoryFax.Text.Trim()
                        };

                        const string insertAdditionalDetails = @"
                        INSERT INTO [dbo].[AdditionalDetails_Certification]
                            ([InquiryID],[RequestId],[CustomerMasterID],[PersonID],[DepartmentCode],
                             [BuyerCustomerName],[BuyerAddressLine1],[BuyerCountry],
                             [ReportRecipient],[InvoiceRecipient],
                             [PayerName],[PayerContactPerson],[PayerContactNo],[PayerContactEmail],
                             [ItemStyleNo],[PONo],[CreatedDate],
                             [IATFieldsJson],[IATTXTFieldsJson])
                        VALUES
                            (@InquiryID,@RequestId,@CustomerMasterID,@PersonID,@DepartmentCode,
                             @BuyerCustomerName,@BuyerAddressLine1,@BuyerCountry,
                             @ReportRecipient,@InvoiceRecipient,
                             @PayerName,@PayerContactPerson,@PayerContactNo,@PayerContactEmail,
                             @ItemStyleNo,@PONo,@CreatedDate,
                             @IATFieldsJson,@IATTXTFieldsJson);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                        using (SqlCommand cmd = new SqlCommand(insertAdditionalDetails, con, tran))
                        {
                            cmd.Parameters.AddWithValue("@InquiryID", inquiryId);
                            cmd.Parameters.AddWithValue("@RequestId", requestId);
                            cmd.Parameters.AddWithValue("@CustomerMasterID", customerMasterId);
                            cmd.Parameters.AddWithValue("@PersonID", personId == 0 ? (object)DBNull.Value : personId);
                            cmd.Parameters.AddWithValue("@DepartmentCode", "IAT_TEXTILE");

                            cmd.Parameters.AddWithValue("@BuyerCustomerName", txtApplicantName.Text);
                            cmd.Parameters.AddWithValue("@BuyerAddressLine1", txtAddress.Text);
                            cmd.Parameters.AddWithValue("@BuyerCountry", txtBuyerCountry.Text);

                            cmd.Parameters.AddWithValue("@ReportRecipient", radReportRecipient.SelectedValue);
                            cmd.Parameters.AddWithValue("@InvoiceRecipient", radInvoiceRecipient.SelectedValue);
                            cmd.Parameters.AddWithValue("@PayerName", txtApplicantName.Text);
                            cmd.Parameters.AddWithValue("@PayerContactPerson", ddlContactPerson.SelectedIndex > 0 ? ddlContactPerson.SelectedItem.Text : "");
                            cmd.Parameters.AddWithValue("@PayerContactNo", txtPhone.Text);
                            cmd.Parameters.AddWithValue("@PayerContactEmail",
                                string.IsNullOrWhiteSpace(txtInvoiceEmail.Text) ? txtEmail.Text : txtInvoiceEmail.Text);

                            cmd.Parameters.AddWithValue("@ItemStyleNo", txtItemStyleNo.Text);
                            cmd.Parameters.AddWithValue("@PONo", txtPONumber.Text);
                            cmd.Parameters.AddWithValue("@CreatedDate", Convert.ToDateTime(txtRequestDate.Text));

                            cmd.Parameters.Add("@IATFieldsJson", SqlDbType.NVarChar, -1).Value = DAL.JsonFieldHelper.ToJson(common);
                            cmd.Parameters.Add("@IATTXTFieldsJson", SqlDbType.NVarChar, -1).Value = DAL.JsonFieldHelper.ToJson(deptFields);

                            additionalDetailsId = (int)cmd.ExecuteScalar();
                        }

                        var selectedServices = GetSelectedServiceTypes();

                        const string insertCrfSample = @"
                    INSERT INTO [dbo].[CustomerCRFSampleDetails]
                        ([InquiryId],[CustomerRequestId],[CustomerId],[SampleName],[BatchNo],[TestMethod],
                         [TestParameterID],[TestParameter],[TestSubParameterID],[TestSubParameter],
                         [TestSubSubParameterID],[TestSubSubParameter],[unitofmeasurement],[Specification],
                         [SizeOfSample],[Remarks],[Created],[DescriptionOfTest],[ExpectedValue],[Dimensions],
                         [TestsRequested],[CRFID],[Temp],[RH],[SampleType],[OrganizationID],[LOD],[LOQ],
                         [Requirement],[RequirementID],[CASNo],[MDL],[CodeCombination],[ReportID],[ReportName],
                         [Voltage],[Part],[TestStandard],[TestStandardID],[Identifier],[TypeNumber])
                    VALUES
                        (@InquiryId,@CustomerRequestId,@CustomerId,@SampleName,@BatchNo,@TestMethod,
                         @TestParameterID,@TestParameter,@TestSubParameterID,@TestSubParameter,
                         @TestSubSubParameterID,@TestSubSubParameter,@unitofmeasurement,@Specification,
                         @SizeOfSample,@Remarks,@Created,@DescriptionOfTest,@ExpectedValue,@Dimensions,
                         @TestsRequested,@CRFID,@Temp,@RH,@SampleType,@OrganizationID,@LOD,@LOQ,
                         @Requirement,@RequirementID,@CASNo,@MDL,@CodeCombination,@ReportID,@ReportName,
                         @Voltage,@Part,@TestStandard,@TestStandardID,@Identifier,@TypeNumber);";

                        foreach (var service in selectedServices)
                        {
                            using (SqlCommand cmd = new SqlCommand(insertCrfSample, con, tran))
                            {
                                cmd.Parameters.AddWithValue("@InquiryId", inquiryId);
                                cmd.Parameters.AddWithValue("@CustomerRequestId", requestId);
                                cmd.Parameters.AddWithValue("@CustomerId", customerId);
                                cmd.Parameters.AddWithValue("@SampleName", txtProductDescription.Text);
                                cmd.Parameters.AddWithValue("@BatchNo", txtItemStyleNo.Text);
                                cmd.Parameters.AddWithValue("@TestMethod", "Visual / On-site Inspection");

                                cmd.Parameters.AddWithValue("@TestParameterID", 0);
                                cmd.Parameters.AddWithValue("@TestParameter", service.Name);
                                cmd.Parameters.AddWithValue("@TestSubParameterID", 0);
                                cmd.Parameters.AddWithValue("@TestSubParameter", "");
                                cmd.Parameters.AddWithValue("@TestSubSubParameterID", 0);
                                cmd.Parameters.AddWithValue("@TestSubSubParameter", "");
                                cmd.Parameters.AddWithValue("@unitofmeasurement", "-");
                                cmd.Parameters.AddWithValue("@Specification", "");

                                cmd.Parameters.AddWithValue("@SizeOfSample", txtShipmentQty.Text);
                                cmd.Parameters.AddWithValue("@Remarks", service.Remarks);
                                cmd.Parameters.AddWithValue("@Created", DateTime.Now);
                                cmd.Parameters.AddWithValue("@DescriptionOfTest", service.Name);
                                cmd.Parameters.AddWithValue("@ExpectedValue", "");
                                cmd.Parameters.AddWithValue("@Dimensions", "");

                                cmd.Parameters.AddWithValue("@TestsRequested", "Yes");
                                cmd.Parameters.AddWithValue("@CRFID", 0);
                                cmd.Parameters.AddWithValue("@Temp", (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@RH", (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@SampleType", txtCommodity.Text);
                                cmd.Parameters.AddWithValue("@OrganizationID", organizationId);
                                cmd.Parameters.AddWithValue("@LOD", "");
                                cmd.Parameters.AddWithValue("@LOQ", "");

                                cmd.Parameters.AddWithValue("@Requirement", "");
                                cmd.Parameters.AddWithValue("@RequirementID", 0);
                                cmd.Parameters.AddWithValue("@CASNo", "");
                                cmd.Parameters.AddWithValue("@MDL", "");
                                cmd.Parameters.AddWithValue("@CodeCombination", "");
                                cmd.Parameters.AddWithValue("@ReportID", 0);
                                cmd.Parameters.AddWithValue("@ReportName", service.Name);

                                cmd.Parameters.AddWithValue("@Voltage", "");
                                cmd.Parameters.AddWithValue("@Part", "");
                                cmd.Parameters.AddWithValue("@TestStandard", "");
                                cmd.Parameters.AddWithValue("@TestStandardID", 0);
                                cmd.Parameters.AddWithValue("@Identifier", "");
                                cmd.Parameters.AddWithValue("@TypeNumber", "");

                                cmd.ExecuteNonQuery();
                            }
                        }

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }

            Response.Redirect("CreateRequestCustomerFM_Textile.aspx?ID=" + inquiryId);
        }

        private static bool CustomerCrfSampleExists(SqlConnection con, int inquiryId)
        {
            const string sql = "SELECT COUNT(1) FROM [dbo].[CustomerCRFSampleDetails] WHERE [InquiryId] = @InquiryId";
            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@InquiryId", inquiryId);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        private void LoadExistingBookingForm(SqlConnection con, int inquiryId)
        {
            const string sql = @"
        SELECT TOP 1
            [AdditionalDetailsID],[CustomerMasterID],[PersonID],
            [BuyerCustomerName],[BuyerAddressLine1],[BuyerCountry],
            [ReportRecipient],[InvoiceRecipient],
            [PayerContactPerson],[PayerContactNo],[PayerContactEmail],
            [ItemStyleNo],[PONo],
            [IATFieldsJson],[IATTXTFieldsJson]
        FROM [dbo].[AdditionalDetails_Certification]
        WHERE [InquiryID] = @InquiryID AND [DepartmentCode] = @DepartmentCode
        ORDER BY [AdditionalDetailsID] DESC";

            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@InquiryID", inquiryId);
                cmd.Parameters.AddWithValue("@DepartmentCode", "IAT_TEXTILE");

                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        hdAdditionalDetailsID.Value = rdr["AdditionalDetailsID"].ToString();
                        hdCustomerMasterID.Value = AsString(rdr, "CustomerMasterID");
                        hdPersonID.Value = AsString(rdr, "PersonID");

                        BindContactPerson(con, inquiryId);
                        if (!string.IsNullOrEmpty(hdPersonID.Value) && ddlContactPerson.Items.FindByValue(hdPersonID.Value) != null)
                        {
                            ddlContactPerson.SelectedValue = hdPersonID.Value;
                        }
                        txtApplicantName.Text = AsString(rdr, "BuyerCustomerName");
                        txtAddress.Text = AsString(rdr, "BuyerAddressLine1");
                        txtBuyerCountry.Text = AsString(rdr, "BuyerCountry");
                        //txtContactPerson.Text = AsString(rdr, "PayerContactPerson");
                        txtPhone.Text = AsString(rdr, "PayerContactNo");
                        txtInvoiceEmail.Text = AsString(rdr, "PayerContactEmail");
                        txtItemStyleNo.Text = AsString(rdr, "ItemStyleNo");
                        txtPONumber.Text = AsString(rdr, "PONo");

                        SetRadioValue(radReportRecipient, AsString(rdr, "ReportRecipient"));
                        SetRadioValue(radInvoiceRecipient, AsString(rdr, "InvoiceRecipient"));

                        var common = DAL.JsonFieldHelper.FromJson<DAL.IATCommonFields>(rdr["IATFieldsJson"]);
                        var dept = DAL.JsonFieldHelper.FromJson<DAL.TextileDeptFields>(rdr["IATTXTFieldsJson"]);

                        // --- common IAT fields ---
                        txtCommodity.Text = common.ServiceRequired ?? "";
                        txtBookingNo.Text = common.BookingFormNo ?? "";
                        txtRequestDate.Text = common.RequestDate.HasValue
                            ? common.RequestDate.Value.ToString("yyyy-MM-dd") : "";
                        txtAttn.Text = common.Attn ?? "";
                        txtEmail.Text = common.ApplicantEmail ?? "";
                        txtFax.Text = common.ApplicantFax ?? "";
                        txtCC.Text = common.CC ?? "";
                        txtOtherInstructions.Text = common.OtherInstructions ?? "";

                        bool refProvided = common.ReferenceProvided == "Yes";
                        SetRadioValue(radReferenceProvided, refProvided ? "Yes" : "No");
                        pnlReference.Visible = refProvided;

                        chkRefOther.Checked = common.RefOther == "Yes";
                        txtOtherReference.Text = common.OtherReferenceText ?? "";

                        // Officer section
                        txtInspectionLocation.Text = common.InspectionLocation ?? "";
                        txtRequestedInspectionDate.Text = common.RequestedInspectionDate.HasValue
                            ? common.RequestedInspectionDate.Value.ToString("yyyy-MM-dd") : "";
                        txtPreviousReportNo.Text = common.PreviousReportNo ?? "";

                        // --- textile department fields ---
                        txtBuyerName.Text = dept.BuyerName ?? "";
                        txtProductDescription.Text = dept.ProductDescription ?? "";
                        txtShipmentQty.Text = dept.ShipmentQty ?? "";
                        txtPercentReady.Text = dept.PercentReady ?? "";
                        txtShippingDate.Text = dept.ShippingDate.HasValue
                            ? dept.ShippingDate.Value.ToString("yyyy-MM-dd") : "";

                        chkRefApprovedSamples.Checked = dept.RefApprovedSamples == "Yes";
                        chkRefProductSpec.Checked = dept.RefProductSpec == "Yes";
                        chkRefDigitalImages.Checked = dept.RefDigitalImages == "Yes";
                        chkRefSwatchCard.Checked = dept.RefSwatchCard == "Yes";
                        chkRefPOCopy.Checked = dept.RefPOCopy == "Yes";
                        chkRefPackingListInvoice.Checked = dept.RefPackingListInvoice == "Yes";
                        chkRefLCCopy.Checked = dept.RefLCCopy == "Yes";
                        chkRefClientInstructionSheet.Checked = dept.RefClientInstructionSheet == "Yes";
                        chkRefRoadMap.Checked = dept.RefRoadMap == "Yes";
                        chkRefCertificate.Checked = dept.RefCertificate == "Yes";
                        chkRefConstructionDataForm.Checked = dept.RefConstructionDataForm == "Yes";

                        txtFactorySupplierName.Text = dept.FactorySupplierName ?? "";
                        txtFactoryAddress.Text = dept.FactoryAddress ?? "";
                        txtFactoryContactPerson.Text = dept.FactoryContactPerson ?? "";
                        txtFactoryPhone.Text = dept.FactoryPhone ?? "";
                        txtFactoryFax.Text = dept.FactoryFax ?? "";
                    }
                }
            }

            LoadSelectedServices(con, inquiryId);
        }
        private void LoadSelectedServices(SqlConnection con, int inquiryId)
        {
            const string sql = "SELECT [TestParameter] FROM [dbo].[CustomerCRFSampleDetails] WHERE [InquiryId] = @InquiryId";
            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@InquiryId", inquiryId);
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        string name = rdr["TestParameter"] == DBNull.Value ? "" : rdr["TestParameter"].ToString();

                        if (string.Equals(name, "Initial Production Inspection (IPI)", StringComparison.OrdinalIgnoreCase)) chkIPI.Checked = true;
                        else if (string.Equals(name, "During Production Inspection (DPI)", StringComparison.OrdinalIgnoreCase)) chkDPI.Checked = true;
                        else if (string.Equals(name, "Final Random Inspection (FRI)", StringComparison.OrdinalIgnoreCase)) chkFRI.Checked = true;
                        else if (string.Equals(name, "Loading Supervision (LS)", StringComparison.OrdinalIgnoreCase)) chkLoadingSupervision.Checked = true;
                        else if (string.Equals(name, "Sample Sealing", StringComparison.OrdinalIgnoreCase)) chkSampleSealing.Checked = true;
                        else if (string.Equals(name, "Fabric Inspection", StringComparison.OrdinalIgnoreCase)) chkFabricInspection.Checked = true;
                        else if (string.Equals(name, "Re-Inspection", StringComparison.OrdinalIgnoreCase)) chkReInspection.Checked = true;
                        else if (!string.IsNullOrWhiteSpace(name))
                        {
                            chkOtherService.Checked = true;
                            txtOtherService.Text = name;
                        }
                    }
                }
            }
        }
        private void SaveOfficerAmendments(int inquiryId)
        {
            int requestId = inquiryId;

            int organizationId = Session["OrganizationID"] != null ? Convert.ToInt32(Session["OrganizationID"]) : 1;
            int employeeId = Session["UserID"] != null ? Convert.ToInt32(Session["UserID"]) : 0;
            int customerId = Session["CustomerId"] != null ? Convert.ToInt32(Session["CustomerId"]) : 0;

            int customerMasterId = 0;
            int.TryParse(hdCustomerMasterID.Value, out customerMasterId);
            int personId = 0;
            int.TryParse(hdPersonID.Value, out personId);

            var common = new DAL.IATCommonFields
            {
                BookingFormNo = string.IsNullOrWhiteSpace(txtBookingNo.Text) ? null : txtBookingNo.Text.Trim(),
                RequestDate = string.IsNullOrWhiteSpace(txtRequestDate.Text) ? (DateTime?)null : Convert.ToDateTime(txtRequestDate.Text),
                Attn = string.IsNullOrWhiteSpace(txtAttn.Text) ? null : txtAttn.Text.Trim(),
                ApplicantEmail = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                ApplicantFax = string.IsNullOrWhiteSpace(txtFax.Text) ? null : txtFax.Text.Trim(),
                CC = string.IsNullOrWhiteSpace(txtCC.Text) ? null : txtCC.Text.Trim(),
                OtherInstructions = string.IsNullOrWhiteSpace(txtOtherInstructions.Text) ? null : txtOtherInstructions.Text.Trim(),
                ServiceRequired = string.IsNullOrWhiteSpace(txtCommodity.Text) ? null : txtCommodity.Text.Trim(),

                ReferenceProvided = radReferenceProvided.SelectedValue == "Yes" ? "Yes" : "No",
                RefOther = (radReferenceProvided.SelectedValue == "Yes" && chkRefOther.Checked) ? "Yes" : "No",
                OtherReferenceText = (radReferenceProvided.SelectedValue == "Yes" && chkRefOther.Checked && !string.IsNullOrWhiteSpace(txtOtherReference.Text))
                    ? txtOtherReference.Text.Trim() : null,

                InspectionLocation = string.IsNullOrWhiteSpace(txtInspectionLocation.Text) ? null : txtInspectionLocation.Text.Trim(),
                RequestedInspectionDate = string.IsNullOrWhiteSpace(txtRequestedInspectionDate.Text) ? (DateTime?)null : Convert.ToDateTime(txtRequestedInspectionDate.Text),
                PreviousReportNo = string.IsNullOrWhiteSpace(txtPreviousReportNo.Text) ? null : txtPreviousReportNo.Text.Trim()
            };

            var deptFields = new DAL.TextileDeptFields
            {
                BuyerName = string.IsNullOrWhiteSpace(txtBuyerName.Text) ? null : txtBuyerName.Text.Trim(),
                ProductDescription = string.IsNullOrWhiteSpace(txtProductDescription.Text) ? null : txtProductDescription.Text.Trim(),
                ShipmentQty = string.IsNullOrWhiteSpace(txtShipmentQty.Text) ? null : txtShipmentQty.Text.Trim(),
                PercentReady = string.IsNullOrWhiteSpace(txtPercentReady.Text) ? null : txtPercentReady.Text.Trim(),
                ShippingDate = string.IsNullOrWhiteSpace(txtShippingDate.Text) ? (DateTime?)null : Convert.ToDateTime(txtShippingDate.Text),

                RefApprovedSamples = (radReferenceProvided.SelectedValue == "Yes" && chkRefApprovedSamples.Checked) ? "Yes" : "No",
                RefSwatchCard = (radReferenceProvided.SelectedValue == "Yes" && chkRefSwatchCard.Checked) ? "Yes" : "No",
                RefLCCopy = (radReferenceProvided.SelectedValue == "Yes" && chkRefLCCopy.Checked) ? "Yes" : "No",
                RefCertificate = (radReferenceProvided.SelectedValue == "Yes" && chkRefCertificate.Checked) ? "Yes" : "No",
                RefProductSpec = (radReferenceProvided.SelectedValue == "Yes" && chkRefProductSpec.Checked) ? "Yes" : "No",
                RefPOCopy = (radReferenceProvided.SelectedValue == "Yes" && chkRefPOCopy.Checked) ? "Yes" : "No",
                RefClientInstructionSheet = (radReferenceProvided.SelectedValue == "Yes" && chkRefClientInstructionSheet.Checked) ? "Yes" : "No",
                RefConstructionDataForm = (radReferenceProvided.SelectedValue == "Yes" && chkRefConstructionDataForm.Checked) ? "Yes" : "No",
                RefDigitalImages = (radReferenceProvided.SelectedValue == "Yes" && chkRefDigitalImages.Checked) ? "Yes" : "No",
                RefPackingListInvoice = (radReferenceProvided.SelectedValue == "Yes" && chkRefPackingListInvoice.Checked) ? "Yes" : "No",
                RefRoadMap = (radReferenceProvided.SelectedValue == "Yes" && chkRefRoadMap.Checked) ? "Yes" : "No",

                FactorySupplierName = string.IsNullOrWhiteSpace(txtFactorySupplierName.Text) ? null : txtFactorySupplierName.Text.Trim(),
                FactoryAddress = string.IsNullOrWhiteSpace(txtFactoryAddress.Text) ? null : txtFactoryAddress.Text.Trim(),
                FactoryContactPerson = string.IsNullOrWhiteSpace(txtFactoryContactPerson.Text) ? null : txtFactoryContactPerson.Text.Trim(),
                FactoryPhone = string.IsNullOrWhiteSpace(txtFactoryPhone.Text) ? null : txtFactoryPhone.Text.Trim(),
                FactoryFax = string.IsNullOrWhiteSpace(txtFactoryFax.Text) ? null : txtFactoryFax.Text.Trim()
            };

            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
            {
                con.Open();

                using (var tran = con.BeginTransaction())
                {
                    try
                    {
                        const string deleteExisting = @"DELETE FROM [dbo].[AdditionalDetails_Certification]
                                                 WHERE [InquiryID]=@InquiryID AND [DepartmentCode]=@DepartmentCode";
                        using (var cmd = new SqlCommand(deleteExisting, con, tran))
                        {
                            cmd.Parameters.AddWithValue("@InquiryID", inquiryId);
                            cmd.Parameters.AddWithValue("@DepartmentCode", "IAT_TEXTILE");
                            cmd.ExecuteNonQuery();
                        }

                        const string insertAdditionalDetails = @"
                    INSERT INTO [dbo].[AdditionalDetails_Certification]
                        ([InquiryID],[RequestId],[CustomerMasterID],[PersonID],[DepartmentCode],
                         [BuyerCustomerName],[BuyerAddressLine1],[BuyerCountry],
                         [ReportRecipient],[InvoiceRecipient],
                         [PayerName],[PayerContactPerson],[PayerContactNo],[PayerContactEmail],
                         [ItemStyleNo],[PONo],[CreatedDate],
                         [IATFieldsJson],[IATTXTFieldsJson])
                    VALUES
                        (@InquiryID,@RequestId,@CustomerMasterID,@PersonID,@DepartmentCode,
                         @BuyerCustomerName,@BuyerAddressLine1,@BuyerCountry,
                         @ReportRecipient,@InvoiceRecipient,
                         @PayerName,@PayerContactPerson,@PayerContactNo,@PayerContactEmail,
                         @ItemStyleNo,@PONo,@CreatedDate,
                         @IATFieldsJson,@IATTXTFieldsJson);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                        int newAdditionalDetailsId;
                        using (var cmd = new SqlCommand(insertAdditionalDetails, con, tran))
                        {
                            cmd.Parameters.AddWithValue("@InquiryID", inquiryId);
                            cmd.Parameters.AddWithValue("@RequestId", requestId);
                            cmd.Parameters.AddWithValue("@CustomerMasterID", customerMasterId);
                            cmd.Parameters.AddWithValue("@PersonID", personId == 0 ? (object)DBNull.Value : personId);
                            cmd.Parameters.AddWithValue("@DepartmentCode", "IAT_TEXTILE");

                            cmd.Parameters.AddWithValue("@BuyerCustomerName", txtApplicantName.Text);
                            cmd.Parameters.AddWithValue("@BuyerAddressLine1", txtAddress.Text);
                            cmd.Parameters.AddWithValue("@BuyerCountry", txtBuyerCountry.Text);

                            cmd.Parameters.AddWithValue("@ReportRecipient", radReportRecipient.SelectedValue);
                            cmd.Parameters.AddWithValue("@InvoiceRecipient", radInvoiceRecipient.SelectedValue);
                            cmd.Parameters.AddWithValue("@PayerName", txtApplicantName.Text);
                            cmd.Parameters.AddWithValue("@PayerContactPerson", ddlContactPerson.SelectedIndex > 0 ? ddlContactPerson.SelectedItem.Text : "");
                            cmd.Parameters.AddWithValue("@PayerContactNo", txtPhone.Text);
                            cmd.Parameters.AddWithValue("@PayerContactEmail",
                                string.IsNullOrWhiteSpace(txtInvoiceEmail.Text) ? txtEmail.Text : txtInvoiceEmail.Text);

                            cmd.Parameters.AddWithValue("@ItemStyleNo", txtItemStyleNo.Text);
                            cmd.Parameters.AddWithValue("@PONo", txtPONumber.Text);
                            cmd.Parameters.AddWithValue("@CreatedDate",
                                string.IsNullOrWhiteSpace(txtRequestDate.Text) ? (object)DateTime.Now : (object)Convert.ToDateTime(txtRequestDate.Text));

                            cmd.Parameters.Add("@IATFieldsJson", SqlDbType.NVarChar, -1).Value = DAL.JsonFieldHelper.ToJson(common);
                            cmd.Parameters.Add("@IATTXTFieldsJson", SqlDbType.NVarChar, -1).Value = DAL.JsonFieldHelper.ToJson(deptFields);

                            newAdditionalDetailsId = (int)cmd.ExecuteScalar();
                        }

                        hdAdditionalDetailsID.Value = newAdditionalDetailsId.ToString();

                        // Existing CRF replace-and-reinsert logic (unchanged) continues here:
                        const string deleteExistingCrf = @"DELETE FROM [dbo].[CRFSampleDetails] WHERE [InquiryId]=@InquiryId AND [CustomerRequestId]=@RequestId";
                        using (var cmd = new SqlCommand(deleteExistingCrf, con, tran))
                        {
                            cmd.Parameters.AddWithValue("@InquiryId", inquiryId);
                            cmd.Parameters.AddWithValue("@RequestId", requestId);
                            cmd.ExecuteNonQuery();
                        }

                        var selectedServices = GetSelectedServiceTypes();

                        const string insertCrf = @"
                    INSERT INTO [dbo].[CRFSampleDetails]
                        ([InquiryId],[CustomerRequestId],[CustomerId],[SampleName],[BatchNo],[TestMethod],
                         [TestParameter],[DescriptionOfTest],[SizeOfSample],[Specification],[ExpectedValue],
                         [Dimensions],[Remarks],[Created],[unitofmeasurement],[TestsRequested],[CRFID],
                         [Temp],[RH],[SampleType],[EmployeeId],[samplestatus],[OrganizationID],
                         [TestSubParameter],[TestParameterID],[TestSubParameterID],[TestSubSubParameterID],
                         [TestSubSubParameter],[LOD],[LOQ],[Requirement],[RequirementID],[CASNo],[MDL],
                         [CodeCombination],[ReportID],[ReportName],[Voltage],[Part],[TestStandardID],[TestStandard])
                    VALUES
                        (@InquiryId,@CustomerRequestId,@CustomerId,@SampleName,@BatchNo,@TestMethod,
                         @TestParameter,@DescriptionOfTest,@SizeOfSample,@Specification,@ExpectedValue,
                         @Dimensions,@Remarks,@Created,@unitofmeasurement,@TestsRequested,@CRFID,
                         @Temp,@RH,@SampleType,@EmployeeId,@samplestatus,@OrganizationID,
                         @TestSubParameter,@TestParameterID,@TestSubParameterID,@TestSubSubParameterID,
                         @TestSubSubParameter,@LOD,@LOQ,@Requirement,@RequirementID,@CASNo,@MDL,
                         @CodeCombination,@ReportID,@ReportName,@Voltage,@Part,@TestStandardID,@TestStandard);";

                        foreach (var service in selectedServices)
                        {
                            using (var cmd = new SqlCommand(insertCrf, con, tran))
                            {
                                cmd.Parameters.AddWithValue("@InquiryId", inquiryId);
                                cmd.Parameters.AddWithValue("@CustomerRequestId", requestId);
                                cmd.Parameters.AddWithValue("@CustomerId", customerId);
                                cmd.Parameters.AddWithValue("@SampleName", txtProductDescription.Text);
                                cmd.Parameters.AddWithValue("@BatchNo", txtItemStyleNo.Text);
                                cmd.Parameters.AddWithValue("@TestMethod", "Visual / On-site Inspection");
                                cmd.Parameters.AddWithValue("@TestParameter", service.Name);
                                cmd.Parameters.AddWithValue("@DescriptionOfTest", service.Name);
                                cmd.Parameters.AddWithValue("@SizeOfSample", txtShipmentQty.Text);
                                cmd.Parameters.AddWithValue("@Specification", "");
                                cmd.Parameters.AddWithValue("@ExpectedValue", "");
                                cmd.Parameters.AddWithValue("@Dimensions", "");
                                cmd.Parameters.AddWithValue("@Remarks", service.Remarks);
                                cmd.Parameters.AddWithValue("@Created", DateTime.Now);
                                cmd.Parameters.AddWithValue("@unitofmeasurement", "-");
                                cmd.Parameters.AddWithValue("@TestsRequested", "Yes");
                                cmd.Parameters.AddWithValue("@CRFID", 0);
                                cmd.Parameters.AddWithValue("@Temp", (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@RH", (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@SampleType", txtCommodity.Text);
                                cmd.Parameters.AddWithValue("@EmployeeId", employeeId == 0 ? (object)DBNull.Value : employeeId);
                                cmd.Parameters.AddWithValue("@samplestatus", (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@OrganizationID", organizationId);
                                cmd.Parameters.AddWithValue("@TestSubParameter", "");
                                cmd.Parameters.AddWithValue("@TestParameterID", 0);
                                cmd.Parameters.AddWithValue("@TestSubParameterID", 0);
                                cmd.Parameters.AddWithValue("@TestSubSubParameterID", 0);
                                cmd.Parameters.AddWithValue("@TestSubSubParameter", "");
                                cmd.Parameters.AddWithValue("@LOD", "");
                                cmd.Parameters.AddWithValue("@LOQ", "");
                                cmd.Parameters.AddWithValue("@Requirement", "");
                                cmd.Parameters.AddWithValue("@RequirementID", 0);
                                cmd.Parameters.AddWithValue("@CASNo", "");
                                cmd.Parameters.AddWithValue("@MDL", "");
                                cmd.Parameters.AddWithValue("@CodeCombination", "");
                                cmd.Parameters.AddWithValue("@ReportID", 0);
                                cmd.Parameters.AddWithValue("@ReportName", service.Name);
                                cmd.Parameters.AddWithValue("@Voltage", "");
                                cmd.Parameters.AddWithValue("@Part", "");
                                cmd.Parameters.AddWithValue("@TestStandardID", 0);
                                cmd.Parameters.AddWithValue("@TestStandard", "");

                                cmd.ExecuteNonQuery();
                            }
                        }

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }
        private static void SetControlsEnabled(Control container, bool enabled)
        {
            foreach (Control c in container.Controls)
            {
                var webControl = c as WebControl;
                if (webControl != null)
                {
                    webControl.Enabled = enabled;
                }

                if (c.Controls != null && c.Controls.Count > 0)
                {
                    SetControlsEnabled(c, enabled);
                }
            }
        }

        private static void SetRadioValue(RadioButtonList list, string value)
        {
            if (!string.IsNullOrEmpty(value) && list.Items.FindByValue(value) != null)
                list.SelectedValue = value;
        }

        private static string AsString(SqlDataReader rdr, string column)
        {
            object v = rdr[column];
            return v == null || v == DBNull.Value ? "" : v.ToString();
        }

        private class ServiceTypeItem
        {
            public string Name;
            public string Remarks;
        }

        private List<ServiceTypeItem> GetSelectedServiceTypes()
        {
            var list = new List<ServiceTypeItem>();

            if (chkIPI.Checked)
                list.Add(new ServiceTypeItem { Name = "Initial Production Inspection (IPI)", Remarks = "" });

            if (chkDPI.Checked)
                list.Add(new ServiceTypeItem { Name = "During Production Inspection (DPI)", Remarks = "" });

            if (chkFRI.Checked)
                list.Add(new ServiceTypeItem { Name = "Final Random Inspection (FRI)", Remarks = "" });

            if (chkLoadingSupervision.Checked)
                list.Add(new ServiceTypeItem { Name = "Loading Supervision (LS)", Remarks = "" });

            if (chkSampleSealing.Checked)
                list.Add(new ServiceTypeItem { Name = "Sample Sealing", Remarks = "" });

            if (chkFabricInspection.Checked)
                list.Add(new ServiceTypeItem { Name = "Fabric Inspection", Remarks = "" });

            if (chkReInspection.Checked)
                list.Add(new ServiceTypeItem { Name = "Re-Inspection", Remarks = "Previous Report No.: " + txtPreviousReportNo.Text });

            if (chkOtherService.Checked && !string.IsNullOrWhiteSpace(txtOtherService.Text))
                list.Add(new ServiceTypeItem { Name = txtOtherService.Text, Remarks = "" });

            return list;
        }
    }
}
