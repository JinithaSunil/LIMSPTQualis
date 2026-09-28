using LIMSStaging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class CustomerFMN_IAT : System.Web.UI.Page
    {
        private const string SampleTypeIAT = "Public Street Lighting (PJU)";
        private const int IATDepartmentID = 4013;
        private const int PJUProductSubCategoryID = 9145;

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
                        DataTable dtInquiryStatus = _cm.GetEnquiryStatus(inquiryId);
                        if (dtInquiryStatus.Rows.Count > 0)
                        {
                            int departmentId = Convert.ToInt32(dtInquiryStatus.Rows[0]["DepartmentID"].ToString());
                            if (departmentId == IATDepartmentID)
                            {
                                int productSubCategoryID = Convert.ToInt32(dtInquiryStatus.Rows[0]["productCategoryID"]);
                                if (productSubCategoryID != PJUProductSubCategoryID)
                                {
                                    Response.Redirect("CustomerFMN_IAT_Textile.aspx?ID=" + inquiryId);
                                    return;
                                }
                            }
                        }

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
                                txtRequestDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

                                var company = IATReportHelper.FetchCompanyContactDetails(con, inquiryId);
                                BindContactPerson(con, inquiryId);
                                txtApplicantName.Text = company.CompanyName;
                                txtAddress.Text = company.Address;
                                txtEmail.Text = company.Email;
                                txtInvoiceEmail.Text = company.Email;
                                txtPhone.Text = company.Phone;
                                txtFax.Text = company.Fax;
                                //txtContactPerson.Text = company.Contactperson;
                                hdCustomerMasterID.Value = company.CustomerMasterId.ToString();
                                hdPersonId.Value = company.PersonId.ToString();
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

                hdPersonId.Value = personId.ToString();
            }
            else
            {
                txtPhone.Text = "";
                txtEmail.Text = "";
                txtFax.Text = "";
                hdPersonId.Value = "";
            }
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
        protected void radReferenceProvided_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlReference.Visible = (radReferenceProvided.SelectedValue == "Yes");
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            lblMessage.Text = string.Empty;

            if (!Page.IsValid)
                return;

            if (string.IsNullOrWhiteSpace(txtApplicantName.Text) ||
                  ddlContactPerson.SelectedIndex <= 0 ||
                  string.IsNullOrWhiteSpace(txtRequestDate.Text))
            {
                lblMessage.Text = "Please fill in Applicant Name, Contact Person and Request Date.";
                return;
            }

            object lampPowerWatt;
            object poleHeightMeter;
            object lampSpacingMeter;
            object numberOfLampPoints;
            string numericError;

            if (!TryBuildNumericValues(out lampPowerWatt, out poleHeightMeter, out lampSpacingMeter, out numberOfLampPoints, out numericError))
            {
                lblMessage.Text = numericError;
                return;
            }

            DateTime requestDate;
            if (!DateTime.TryParseExact(txtRequestDate.Text.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out requestDate))
            {
                lblMessage.Text = "Request Date is not a valid date (expected yyyy-MM-dd).";
                return;
            }

            object requestedInspectionDate = ParseDateOrDbNull(txtRequestedInspectionDate.Text);

            if (!string.IsNullOrWhiteSpace(txtRequestedInspectionDate.Text) && requestedInspectionDate == DBNull.Value)
            {
                lblMessage.Text = "Requested Inspection Date is not a valid date.";
                return;
            }

            int inquiryId = 0;
            int.TryParse(hdInquiryId.Value, out inquiryId);

            bool isOfficerMode = hdIsOfficerMode.Value == "1";

            if (isOfficerMode)
            {
                if (string.IsNullOrWhiteSpace(txtInspectionLocation.Text))
                {
                    lblMessage.Text = "Please fill in Inspection Location.";
                    return;
                }

                SaveOfficerAmendments(inquiryId, lampPowerWatt, poleHeightMeter, lampSpacingMeter, numberOfLampPoints, requestedInspectionDate);

                Response.Redirect("CreateRequestCustomerFM_IAT.aspx?ID=" + inquiryId);
                return;
            }

            int customerMasterId = 0;
            int.TryParse(hdCustomerMasterID.Value, out customerMasterId);

            int personId = 0;
            int.TryParse(hdPersonId.Value, out personId);

            int organizationId = Session["OrganizationID"] != null ? Convert.ToInt32(Session["OrganizationID"]) : 1;
            int userId = Session["UserID"] != null ? Convert.ToInt32(Session["UserID"]) : 0;
            int customerId = Session["CustomerId"] != null ? Convert.ToInt32(Session["CustomerId"]) : 0;

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
                            RequestDate = requestDate,
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
                            RequestedInspectionDate = requestedInspectionDate == DBNull.Value ? (DateTime?)null : (DateTime)requestedInspectionDate,
                            PreviousReportNo = string.IsNullOrWhiteSpace(txtPreviousReportNo.Text) ? null : txtPreviousReportNo.Text.Trim()
                        };

                        var deptFields = new DAL.LightingDeptFields
                        {
                            RoadLocation = string.IsNullOrWhiteSpace(txtRoadLocation.Text) ? null : txtRoadLocation.Text.Trim(),
                            LampPowerWatt = lampPowerWatt == DBNull.Value ? null : lampPowerWatt.ToString(),
                            NumberOfLampPoints = numberOfLampPoints == DBNull.Value ? (int?)null : (int)numberOfLampPoints,
                            PoleHeightMeter = poleHeightMeter == DBNull.Value ? null : poleHeightMeter.ToString(),
                            LampType = string.IsNullOrWhiteSpace(txtLampType.Text) ? null : txtLampType.Text.Trim(),
                            LampSpacingMeter = lampSpacingMeter == DBNull.Value ? null : lampSpacingMeter.ToString(),

                            RefLampPoleData = (radReferenceProvided.SelectedValue == "Yes" && chkRefLampPoleData.Checked) ? "Yes" : "No",
                            RefLampPointList = (radReferenceProvided.SelectedValue == "Yes" && chkRefLampPointList.Checked) ? "Yes" : "No",
                            RefLayout = (radReferenceProvided.SelectedValue == "Yes" && chkRefLayout.Checked) ? "Yes" : "No",
                            RefCommissioning = (radReferenceProvided.SelectedValue == "Yes" && chkRefCommissioning.Checked) ? "Yes" : "No",

                            FactorySupplierName = txtFactorySupplierName.Text,
                            FactoryAddress = txtFactoryAddress.Text,
                            FactoryContactPerson = txtFactoryContactPerson.Text,
                            FactoryPhone = txtFactoryPhone.Text,
                            FactoryFax = txtFactoryFax.Text
                        };

                        const string insertAdditionalDetails = @"
                        INSERT INTO [dbo].[AdditionalDetails_Certification]
                            ([InquiryID],[RequestId],[CustomerMasterID],[PersonID],[DepartmentCode],
                             [BuyerCustomerName],[BuyerAddressLine1],
                             [ReportRecipient],[InvoiceRecipient],
                             [PayerName],[PayerContactPerson],[PayerContactNo],[PayerContactEmail],
                             [CreatedDate],[IATFieldsJson],[IATTXTFieldsJson])
                        VALUES
                            (@InquiryID,@RequestId,@CustomerMasterID,@PersonID,@DepartmentCode,
                             @BuyerCustomerName,@BuyerAddressLine1,
                             @ReportRecipient,@InvoiceRecipient,
                             @PayerName,@PayerContactPerson,@PayerContactNo,@PayerContactEmail,
                             @CreatedDate,@IATFieldsJson,@IATTXTFieldsJson);

                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                        using (SqlCommand cmd = new SqlCommand(insertAdditionalDetails, con, tran))
                        {
                            cmd.Parameters.AddWithValue("@InquiryID", inquiryId);
                            cmd.Parameters.AddWithValue("@RequestId", requestId);
                            cmd.Parameters.AddWithValue("@CustomerMasterID", customerMasterId);
                            cmd.Parameters.Add("@PersonID", SqlDbType.Int).Value = personId;
                            cmd.Parameters.AddWithValue("@DepartmentCode", "IAT_LIGHTING");

                            cmd.Parameters.AddWithValue("@BuyerCustomerName", txtApplicantName.Text);
                            cmd.Parameters.AddWithValue("@BuyerAddressLine1", txtAddress.Text);

                            cmd.Parameters.AddWithValue("@ReportRecipient", radReportRecipient.SelectedValue);
                            cmd.Parameters.AddWithValue("@InvoiceRecipient", radInvoiceRecipient.SelectedValue);
                            cmd.Parameters.AddWithValue("@PayerName", txtApplicantName.Text);
                            cmd.Parameters.AddWithValue("@PayerContactPerson", ddlContactPerson.SelectedIndex > 0 ? ddlContactPerson.SelectedItem.Text : "");
                            cmd.Parameters.AddWithValue("@PayerContactNo", txtPhone.Text);
                            cmd.Parameters.AddWithValue("@PayerContactEmail",
                                string.IsNullOrWhiteSpace(txtInvoiceEmail.Text) ? txtEmail.Text : txtInvoiceEmail.Text);
                            cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                            cmd.Parameters.Add("@IATFieldsJson", SqlDbType.NVarChar, -1).Value = DAL.JsonFieldHelper.ToJson(common);
                            cmd.Parameters.Add("@IATTXTFieldsJson", SqlDbType.NVarChar, -1).Value = DAL.JsonFieldHelper.ToJson(deptFields);

                            additionalDetailsId = (int)cmd.ExecuteScalar();
                        }

                        var selectedTests = GetSelectedInspectionTypes();

                        const string insertCrfSample = @"
                    INSERT INTO [dbo].[CustomerCRFSampleDetails]
                        ([InquiryId],[CustomerRequestId],[CustomerId],
                         [SampleName],[BatchNo],[TestMethod],
                         [TestParameterID],[TestParameter],
                         [TestSubParameterID],[TestSubParameter],
                         [TestSubSubParameterID],[TestSubSubParameter],
                         [unitofmeasurement],[Specification],
                         [SizeOfSample],[Remarks],[Created],
                         [DescriptionOfTest],[ExpectedValue],[Dimensions],
                         [TestsRequested],[CRFID],[Temp],[RH],
                         [SampleType],[OrganizationID],[LOD],[LOQ],
                         [Requirement],[RequirementID],[CASNo],[MDL],
                         [CodeCombination],[ReportID],[ReportName],
                         [IAT_LampPowerWatt],
                         [Part],[TestStandard],[TestStandardID],
                         [Identifier],[TypeNumber])
                    VALUES
                        (@InquiryId,@CustomerRequestId,@CustomerId,
                         @SampleName,@BatchNo,@TestMethod,
                         @TestParameterID,@TestParameter,
                         @TestSubParameterID,@TestSubParameter,
                         @TestSubSubParameterID,@TestSubSubParameter,
                         @unitofmeasurement,@Specification,
                         @SizeOfSample,@Remarks,@Created,
                         @DescriptionOfTest,@ExpectedValue,@Dimensions,
                         @TestsRequested,@CRFID,@Temp,@RH,
                         @SampleType,@OrganizationID,@LOD,@LOQ,
                         @Requirement,@RequirementID,@CASNo,@MDL,
                         @CodeCombination,@ReportID,@ReportName,
                         @IAT_LampPowerWatt,
                         @Part,@TestStandard,@TestStandardID,
                         @Identifier,@TypeNumber);";

                        foreach (var test in selectedTests)
                        {
                            using (SqlCommand cmd = new SqlCommand(insertCrfSample, con, tran))
                            {
                                cmd.Parameters.AddWithValue("@InquiryId", inquiryId);
                                cmd.Parameters.AddWithValue("@CustomerRequestId", requestId);
                                cmd.Parameters.AddWithValue("@CustomerId", customerId);
                                cmd.Parameters.AddWithValue("@SampleName", txtRoadLocation.Text);
                                cmd.Parameters.AddWithValue("@BatchNo", txtLampType.Text);
                                cmd.Parameters.AddWithValue("@TestMethod", "Lux Meter & Luminance Meter");
                                cmd.Parameters.AddWithValue("@TestParameterID", 0);
                                cmd.Parameters.AddWithValue("@TestParameter", test.Name);
                                cmd.Parameters.AddWithValue("@TestSubParameterID", 0);
                                cmd.Parameters.AddWithValue("@TestSubParameter", "");
                                cmd.Parameters.AddWithValue("@TestSubSubParameterID", 0);
                                cmd.Parameters.AddWithValue("@TestSubSubParameter", "");
                                cmd.Parameters.AddWithValue("@unitofmeasurement", test.Unit);
                                cmd.Parameters.AddWithValue("@Specification", "");
                                cmd.Parameters.AddWithValue("@SizeOfSample", txtNumberOfLampPoints.Text);
                                cmd.Parameters.AddWithValue("@Remarks", test.Remarks);
                                cmd.Parameters.AddWithValue("@Created", DateTime.Now);
                                cmd.Parameters.AddWithValue("@DescriptionOfTest", test.Name);
                                cmd.Parameters.AddWithValue("@ExpectedValue", "");
                                cmd.Parameters.AddWithValue("@Dimensions",
                                    "Pole height: " + txtPoleHeight.Text + " m; Spacing: " + txtLampSpacing.Text + " m");
                                cmd.Parameters.AddWithValue("@TestsRequested", "Yes");
                                cmd.Parameters.AddWithValue("@CRFID", 0);
                                cmd.Parameters.AddWithValue("@Temp", DBNull.Value);
                                cmd.Parameters.AddWithValue("@RH", DBNull.Value);
                                cmd.Parameters.AddWithValue("@SampleType", SampleTypeIAT);
                                cmd.Parameters.AddWithValue("@OrganizationID", organizationId);
                                cmd.Parameters.AddWithValue("@LOD", "");
                                cmd.Parameters.AddWithValue("@LOQ", "");
                                cmd.Parameters.AddWithValue("@Requirement", "");
                                cmd.Parameters.AddWithValue("@RequirementID", 0);
                                cmd.Parameters.AddWithValue("@CASNo", "");
                                cmd.Parameters.AddWithValue("@MDL", "");
                                cmd.Parameters.AddWithValue("@CodeCombination", "");
                                cmd.Parameters.AddWithValue("@ReportID", 0);
                                cmd.Parameters.AddWithValue("@ReportName", test.Name);

                                cmd.Parameters.Add("@IAT_LampPowerWatt", SqlDbType.NVarChar, 100).Value = lampPowerWatt;

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

            Response.Redirect("CreateRequestCustomerFM_IAT.aspx?ID=" + inquiryId);
        }

        private static bool CustomerCrfSampleExists(SqlConnection con, int inquiryId)
        {
            const string sql = @"SELECT COUNT(1) FROM [dbo].[CustomerCRFSampleDetails]
                                  WHERE [InquiryId] = @InquiryId AND [SampleType] = @SampleType";
            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@InquiryId", inquiryId);
                cmd.Parameters.AddWithValue("@SampleType", SampleTypeIAT);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        private void LoadExistingBookingForm(SqlConnection con, int inquiryId)
        {
            const string sql = @"
        SELECT TOP 1
            [AdditionalDetailsID],[CustomerMasterID],[PersonID],
            [BuyerCustomerName],[BuyerAddressLine1],
            [ReportRecipient],[InvoiceRecipient],
            [PayerContactPerson],[PayerContactNo],[PayerContactEmail],
            [IATFieldsJson],[IATTXTFieldsJson]
        FROM [dbo].[AdditionalDetails_Certification]
        WHERE [InquiryID] = @InquiryID AND [DepartmentCode] = @DepartmentCode
        ORDER BY [AdditionalDetailsID] DESC";

            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@InquiryID", inquiryId);
                cmd.Parameters.AddWithValue("@DepartmentCode", "IAT_LIGHTING");

                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        hdAdditionalDetailsID.Value = rdr["AdditionalDetailsID"].ToString();
                        hdCustomerMasterID.Value = AsString(rdr, "CustomerMasterID");
                        hdPersonId.Value = AsString(rdr, "PersonID");

                        BindContactPerson(con, inquiryId);   
                        if (!string.IsNullOrEmpty(hdPersonId.Value) && ddlContactPerson.Items.FindByValue(hdPersonId.Value) != null)
                        {
                            ddlContactPerson.SelectedValue = hdPersonId.Value;  
                        }

                        txtApplicantName.Text = AsString(rdr, "BuyerCustomerName");
                        txtAddress.Text = AsString(rdr, "BuyerAddressLine1");
                        txtPhone.Text = AsString(rdr, "PayerContactNo");
                        txtInvoiceEmail.Text = AsString(rdr, "PayerContactEmail");

                        SetRadioValue(radReportRecipient, AsString(rdr, "ReportRecipient"));
                        SetRadioValue(radInvoiceRecipient, AsString(rdr, "InvoiceRecipient"));

                        var common = DAL.JsonFieldHelper.FromJson<DAL.IATCommonFields>(rdr["IATFieldsJson"]);
                        var dept = DAL.JsonFieldHelper.FromJson<DAL.LightingDeptFields>(rdr["IATTXTFieldsJson"]);

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

                        txtInspectionLocation.Text = common.InspectionLocation ?? "";
                        txtRequestedInspectionDate.Text = common.RequestedInspectionDate.HasValue
                            ? common.RequestedInspectionDate.Value.ToString("yyyy-MM-dd") : "";
                        txtPreviousReportNo.Text = common.PreviousReportNo ?? "";

                        // --- lighting department fields ---
                        txtRoadLocation.Text = dept.RoadLocation ?? "";
                        txtLampPower.Text = dept.LampPowerWatt ?? "";
                        txtNumberOfLampPoints.Text = dept.NumberOfLampPoints.HasValue
                            ? dept.NumberOfLampPoints.Value.ToString() : "";
                        txtPoleHeight.Text = dept.PoleHeightMeter ?? "";
                        txtLampType.Text = dept.LampType ?? "";
                        txtLampSpacing.Text = dept.LampSpacingMeter ?? "";

                        chkRefLampPoleData.Checked = dept.RefLampPoleData == "Yes";
                        chkRefLampPointList.Checked = dept.RefLampPointList == "Yes";
                        chkRefLayout.Checked = dept.RefLayout == "Yes";
                        chkRefCommissioning.Checked = dept.RefCommissioning == "Yes";

                        txtFactorySupplierName.Text = dept.FactorySupplierName ?? "";
                        txtFactoryAddress.Text = dept.FactoryAddress ?? "";
                        txtFactoryContactPerson.Text = dept.FactoryContactPerson ?? "";
                        txtFactoryPhone.Text = dept.FactoryPhone ?? "";
                        txtFactoryFax.Text = dept.FactoryFax ?? "";
                    }
                }
            }

            LoadSelectedInspectionTypes(con, inquiryId);
        }

        private void LoadSelectedInspectionTypes(SqlConnection con, int inquiryId)
        {
            const string sql = @"SELECT [TestParameter] FROM [dbo].[CustomerCRFSampleDetails]
                                  WHERE [InquiryId] = @InquiryId AND [SampleType] = @SampleType";
            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@InquiryId", inquiryId);
                cmd.Parameters.AddWithValue("@SampleType", SampleTypeIAT);
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        string name = rdr["TestParameter"] == DBNull.Value ? "" : rdr["TestParameter"].ToString();

                        if (string.Equals(name, "Average Illuminance Level Inspection", StringComparison.OrdinalIgnoreCase)) chkAvgIlluminance.Checked = true;
                        else if (string.Equals(name, "Uniformity of Illuminance Inspection", StringComparison.OrdinalIgnoreCase)) chkUniformityIlluminance.Checked = true;
                        else if (string.Equals(name, "Dark Spot & Bright Area Mapping (Lighting Spot Analysis)", StringComparison.OrdinalIgnoreCase)) chkSpotAnalysis.Checked = true;
                        else if (string.Equals(name, "Average Luminance Level Inspection", StringComparison.OrdinalIgnoreCase)) chkAvgLuminance.Checked = true;
                        else if (string.Equals(name, "Uniformity of Luminance Inspection", StringComparison.OrdinalIgnoreCase)) chkUniformityLuminance.Checked = true;
                        else if (string.Equals(name, "Glare Potential Evaluation (Threshold Increment, TI)", StringComparison.OrdinalIgnoreCase)) chkGlareTI.Checked = true;
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

        private void SaveOfficerAmendments(
     int inquiryId,
     object lampPowerWatt,
     object poleHeightMeter,
     object lampSpacingMeter,
     object numberOfLampPoints,
     object requestedInspectionDate)
        {
            int requestId = inquiryId;

            int organizationId = Session["OrganizationID"] != null ? Convert.ToInt32(Session["OrganizationID"]) : 1;
            int employeeId = Session["UserID"] != null ? Convert.ToInt32(Session["UserID"]) : 0;
            int customerId = Session["CustomerId"] != null ? Convert.ToInt32(Session["CustomerId"]) : 0;

            int customerMasterId = 0;
            int.TryParse(hdCustomerMasterID.Value, out customerMasterId);
            int personId = 0;
            int.TryParse(hdPersonId.Value, out personId);

            var common = new DAL.IATCommonFields
            {
                BookingFormNo = string.IsNullOrWhiteSpace(txtBookingNo.Text) ? null : txtBookingNo.Text.Trim(),
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
                RequestedInspectionDate = requestedInspectionDate == DBNull.Value ? (DateTime?)null : (DateTime)requestedInspectionDate,
                PreviousReportNo = string.IsNullOrWhiteSpace(txtPreviousReportNo.Text) ? null : txtPreviousReportNo.Text.Trim()
            };

            var deptFields = new DAL.LightingDeptFields
            {
                RoadLocation = string.IsNullOrWhiteSpace(txtRoadLocation.Text) ? null : txtRoadLocation.Text.Trim(),
                LampPowerWatt = lampPowerWatt == DBNull.Value ? null : lampPowerWatt.ToString(),
                NumberOfLampPoints = numberOfLampPoints == DBNull.Value ? (int?)null : (int)numberOfLampPoints,
                PoleHeightMeter = poleHeightMeter == DBNull.Value ? null : poleHeightMeter.ToString(),
                LampType = string.IsNullOrWhiteSpace(txtLampType.Text) ? null : txtLampType.Text.Trim(),
                LampSpacingMeter = lampSpacingMeter == DBNull.Value ? null : lampSpacingMeter.ToString(),

                RefLampPoleData = (radReferenceProvided.SelectedValue == "Yes" && chkRefLampPoleData.Checked) ? "Yes" : "No",
                RefLampPointList = (radReferenceProvided.SelectedValue == "Yes" && chkRefLampPointList.Checked) ? "Yes" : "No",
                RefLayout = (radReferenceProvided.SelectedValue == "Yes" && chkRefLayout.Checked) ? "Yes" : "No",
                RefCommissioning = (radReferenceProvided.SelectedValue == "Yes" && chkRefCommissioning.Checked) ? "Yes" : "No",

                FactorySupplierName = txtFactorySupplierName.Text,
                FactoryAddress = txtFactoryAddress.Text,
                FactoryContactPerson = txtFactoryContactPerson.Text,
                FactoryPhone = txtFactoryPhone.Text,
                FactoryFax = txtFactoryFax.Text
            };

            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString))
            {
                con.Open();

                using (var tran = con.BeginTransaction())
                {
                    try
                    {
                        // Wipe the previous booking-form row for this inquiry, then re-insert fresh.
                        const string deleteExisting = @"DELETE FROM [dbo].[AdditionalDetails_Certification]
                                                 WHERE [InquiryID]=@InquiryID AND [DepartmentCode]=@DepartmentCode";
                        using (var cmd = new SqlCommand(deleteExisting, con, tran))
                        {
                            cmd.Parameters.AddWithValue("@InquiryID", inquiryId);
                            cmd.Parameters.AddWithValue("@DepartmentCode", "IAT_LIGHTING");
                            cmd.ExecuteNonQuery();
                        }

                        const string insertAdditionalDetails = @"
                    INSERT INTO [dbo].[AdditionalDetails_Certification]
                        ([InquiryID],[RequestId],[CustomerMasterID],[PersonID],[DepartmentCode],
                         [BuyerCustomerName],[BuyerAddressLine1],
                         [ReportRecipient],[InvoiceRecipient],
                         [PayerName],[PayerContactPerson],[PayerContactNo],[PayerContactEmail],
                         [CreatedDate],[IATFieldsJson],[IATTXTFieldsJson])
                    VALUES
                        (@InquiryID,@RequestId,@CustomerMasterID,@PersonID,@DepartmentCode,
                         @BuyerCustomerName,@BuyerAddressLine1,
                         @ReportRecipient,@InvoiceRecipient,
                         @PayerName,@PayerContactPerson,@PayerContactNo,@PayerContactEmail,
                         @CreatedDate,@IATFieldsJson,@IATTXTFieldsJson);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                        int newAdditionalDetailsId;
                        using (var cmd = new SqlCommand(insertAdditionalDetails, con, tran))
                        {
                            cmd.Parameters.AddWithValue("@InquiryID", inquiryId);
                            cmd.Parameters.AddWithValue("@RequestId", requestId);
                            cmd.Parameters.AddWithValue("@CustomerMasterID", customerMasterId);
                            cmd.Parameters.Add("@PersonID", SqlDbType.Int).Value = personId;
                            cmd.Parameters.AddWithValue("@DepartmentCode", "IAT_LIGHTING");

                            cmd.Parameters.AddWithValue("@BuyerCustomerName", txtApplicantName.Text);
                            cmd.Parameters.AddWithValue("@BuyerAddressLine1", txtAddress.Text);

                            cmd.Parameters.AddWithValue("@ReportRecipient", radReportRecipient.SelectedValue);
                            cmd.Parameters.AddWithValue("@InvoiceRecipient", radInvoiceRecipient.SelectedValue);
                            cmd.Parameters.AddWithValue("@PayerName", txtApplicantName.Text);
                            cmd.Parameters.AddWithValue("@PayerContactPerson", ddlContactPerson.SelectedIndex > 0 ? ddlContactPerson.SelectedItem.Text : "");
                            cmd.Parameters.AddWithValue("@PayerContactNo", txtPhone.Text);
                            cmd.Parameters.AddWithValue("@PayerContactEmail",
                                string.IsNullOrWhiteSpace(txtInvoiceEmail.Text) ? txtEmail.Text : txtInvoiceEmail.Text);
                            cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                            cmd.Parameters.Add("@IATFieldsJson", SqlDbType.NVarChar, -1).Value = DAL.JsonFieldHelper.ToJson(common);
                            cmd.Parameters.Add("@IATTXTFieldsJson", SqlDbType.NVarChar, -1).Value = DAL.JsonFieldHelper.ToJson(deptFields);

                            newAdditionalDetailsId = (int)cmd.ExecuteScalar();
                        }

                        hdAdditionalDetailsID.Value = newAdditionalDetailsId.ToString();

                        // Existing CRF replace-and-reinsert logic (unchanged) continues here:
                        const string deleteExistingCrf = @"DELETE FROM [dbo].[CRFSampleDetails]
                                                    WHERE [InquiryId]=@InquiryId AND [CustomerRequestId]=@RequestId AND [SampleType]=@SampleType";
                        using (var cmd = new SqlCommand(deleteExistingCrf, con, tran))
                        {
                            cmd.Parameters.AddWithValue("@InquiryId", inquiryId);
                            cmd.Parameters.AddWithValue("@RequestId", requestId);
                            cmd.Parameters.AddWithValue("@SampleType", SampleTypeIAT);
                            cmd.ExecuteNonQuery();
                        }

                        var selectedTests = GetSelectedInspectionTypes();

                        const string insertCrf = @"
                    INSERT INTO [dbo].[CRFSampleDetails]
                        ([InquiryId],[CustomerRequestId],[CustomerId],[SampleName],[BatchNo],[TestMethod],
                         [TestParameter],[DescriptionOfTest],[SizeOfSample],[Specification],[ExpectedValue],
                         [Dimensions],[Remarks],[Created],[unitofmeasurement],[TestsRequested],[CRFID],
                         [Temp],[RH],[SampleType],[EmployeeId],[samplestatus],[OrganizationID],
                         [TestSubParameter],[TestParameterID],[TestSubParameterID],[TestSubSubParameterID],
                         [TestSubSubParameter],[LOD],[LOQ],[Requirement],[RequirementID],[CASNo],[MDL],
                         [CodeCombination],[ReportID],[ReportName],[Voltage],[Part],[TestStandardID],[TestStandard],
                         [IAT_LampPowerWatt])
                    VALUES
                        (@InquiryId,@CustomerRequestId,@CustomerId,@SampleName,@BatchNo,@TestMethod,
                         @TestParameter,@DescriptionOfTest,@SizeOfSample,@Specification,@ExpectedValue,
                         @Dimensions,@Remarks,@Created,@unitofmeasurement,@TestsRequested,@CRFID,
                         @Temp,@RH,@SampleType,@EmployeeId,@samplestatus,@OrganizationID,
                         @TestSubParameter,@TestParameterID,@TestSubParameterID,@TestSubSubParameterID,
                         @TestSubSubParameter,@LOD,@LOQ,@Requirement,@RequirementID,@CASNo,@MDL,
                         @CodeCombination,@ReportID,@ReportName,@Voltage,@Part,@TestStandardID,@TestStandard,
                         @IAT_LampPowerWatt);";

                        foreach (var test in selectedTests)
                        {
                            using (var cmd = new SqlCommand(insertCrf, con, tran))
                            {
                                cmd.Parameters.AddWithValue("@InquiryId", inquiryId);
                                cmd.Parameters.AddWithValue("@CustomerRequestId", requestId);
                                cmd.Parameters.AddWithValue("@CustomerId", customerId);
                                cmd.Parameters.AddWithValue("@SampleName", txtRoadLocation.Text);
                                cmd.Parameters.AddWithValue("@BatchNo", txtLampType.Text);
                                cmd.Parameters.AddWithValue("@TestMethod", "Lux Meter & Luminance Meter");
                                cmd.Parameters.AddWithValue("@TestParameter", test.Name);
                                cmd.Parameters.AddWithValue("@DescriptionOfTest", test.Name);
                                cmd.Parameters.AddWithValue("@SizeOfSample", txtNumberOfLampPoints.Text);
                                cmd.Parameters.AddWithValue("@Specification", "");
                                cmd.Parameters.AddWithValue("@ExpectedValue", "");
                                cmd.Parameters.AddWithValue("@Dimensions",
                                    "Pole height: " + txtPoleHeight.Text + " m; Spacing: " + txtLampSpacing.Text + " m");
                                cmd.Parameters.AddWithValue("@Remarks", test.Remarks);
                                cmd.Parameters.AddWithValue("@Created", DateTime.Now);
                                cmd.Parameters.AddWithValue("@unitofmeasurement", test.Unit);
                                cmd.Parameters.AddWithValue("@TestsRequested", "Yes");
                                cmd.Parameters.AddWithValue("@CRFID", 0);
                                cmd.Parameters.AddWithValue("@Temp", (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@RH", (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@SampleType", SampleTypeIAT);
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
                                cmd.Parameters.AddWithValue("@ReportName", test.Name);
                                cmd.Parameters.AddWithValue("@Voltage", "");
                                cmd.Parameters.AddWithValue("@Part", "");
                                cmd.Parameters.AddWithValue("@TestStandardID", 0);
                                cmd.Parameters.AddWithValue("@TestStandard", "");

                                cmd.Parameters.Add("@IAT_LampPowerWatt", SqlDbType.NVarChar, 100).Value = lampPowerWatt;

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

        private bool TryBuildNumericValues(
            out object lampPowerWatt,
            out object poleHeightMeter,
            out object lampSpacingMeter,
            out object numberOfLampPoints,
            out string error)
        {
            error = null;

            lampPowerWatt = ParseTextOrDbNull(txtLampPower.Text);
            poleHeightMeter = ParseTextOrDbNull(txtPoleHeight.Text);
            lampSpacingMeter = ParseTextOrDbNull(txtLampSpacing.Text);

            numberOfLampPoints = ParseIntOrDbNull(txtNumberOfLampPoints.Text);

            if (!string.IsNullOrWhiteSpace(txtNumberOfLampPoints.Text) && numberOfLampPoints == DBNull.Value)
            {
                error = "Number of Lamp Points must be a whole number.";
                return false;
            }

            return true;
        }

        private static object ParseTextOrDbNull(string text)
        {
            return string.IsNullOrWhiteSpace(text) ? (object)DBNull.Value : text.Trim();
        }

        private static object ParseIntOrDbNull(string text)
        {
            int value;

            if (!string.IsNullOrWhiteSpace(text) &&
                int.TryParse(text.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            {
                return value;
            }

            return DBNull.Value;
        }

        private static object ParseDateOrDbNull(string text)
        {
            DateTime value;

            if (!string.IsNullOrWhiteSpace(text) &&
                DateTime.TryParse(text.Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out value))
            {
                return value;
            }

            return DBNull.Value;
        }

        private class InspectionTestItem
        {
            public string Name;
            public string Unit;
            public string Remarks;
        }

        private List<InspectionTestItem> GetSelectedInspectionTypes()
        {
            var list = new List<InspectionTestItem>();

            if (chkAvgIlluminance.Checked)
                list.Add(new InspectionTestItem { Name = "Average Illuminance Level Inspection", Unit = "Lux", Remarks = "" });

            if (chkUniformityIlluminance.Checked)
                list.Add(new InspectionTestItem { Name = "Uniformity of Illuminance Inspection", Unit = "-", Remarks = "" });

            if (chkSpotAnalysis.Checked)
                list.Add(new InspectionTestItem { Name = "Dark Spot & Bright Area Mapping (Lighting Spot Analysis)", Unit = "-", Remarks = "" });

            if (chkAvgLuminance.Checked)
                list.Add(new InspectionTestItem { Name = "Average Luminance Level Inspection", Unit = "cd/m2", Remarks = "" });

            if (chkUniformityLuminance.Checked)
                list.Add(new InspectionTestItem { Name = "Uniformity of Luminance Inspection", Unit = "-", Remarks = "" });

            if (chkGlareTI.Checked)
                list.Add(new InspectionTestItem { Name = "Glare Potential Evaluation (Threshold Increment, TI)", Unit = "-", Remarks = "" });

            if (chkReInspection.Checked)
                list.Add(new InspectionTestItem { Name = "Re-Inspection", Unit = "-", Remarks = "Previous Report No.: " + txtPreviousReportNo.Text });

            if (chkOtherService.Checked && !string.IsNullOrWhiteSpace(txtOtherService.Text))
                list.Add(new InspectionTestItem { Name = txtOtherService.Text, Unit = "-", Remarks = "" });

            return list;
        }
    }
}