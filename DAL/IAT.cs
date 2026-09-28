using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace LIMS.DAL
{
    public static class JsonFieldHelper
    {
        public static string ToJson<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T FromJson<T>(object dbValue) where T : new()
        {
            if (dbValue == null || dbValue == System.DBNull.Value)
                return new T();

            var json = dbValue.ToString();
            return string.IsNullOrWhiteSpace(json)
                ? new T()
                : JsonConvert.DeserializeObject<T>(json);
        }
    }
    public class IATCommonFields
    {
        public string BookingFormNo;
        public DateTime? RequestDate;
        public string Attn;
        public string ApplicantEmail;
        public string ApplicantFax;
        public string CC;
        public string OtherInstructions;
        public string ServiceRequired;

        public string ReferenceProvided;   // "Yes" / "No"
        public string RefOther;
        public string OtherReferenceText;

        public string InspectionLocation;
        public DateTime? RequestedInspectionDate;
        public string PreviousReportNo;
    }

    // Department-specific fields for Public Street Lighting (PJU) — CustomerFMN_IAT.aspx
    // -> stored in AdditionalDetails_Certification.IATTXTFieldsJson when DepartmentCode = "IAT_LIGHTING"
    public class LightingDeptFields
    {
        public string RoadLocation;
        public string LampPowerWatt;
        public int? NumberOfLampPoints;
        public string PoleHeightMeter;
        public string LampType;
        public string LampSpacingMeter;

        public string RefLampPoleData;
        public string RefLampPointList;
        public string RefLayout;
        public string RefCommissioning;

        public string FactorySupplierName;
        public string FactoryAddress;
        public string FactoryContactPerson;
        public string FactoryPhone;
        public string FactoryFax;
    }

    // Department-specific fields for Textile — CustomerFMN_IAT_Textile.aspx
    // -> stored in AdditionalDetails_Certification.IATTXTFieldsJson when DepartmentCode = "IAT_TEXTILE"
    public class TextileDeptFields
    {
        public string BuyerName;
        public string ProductDescription;
        public string ShipmentQty;
        public string PercentReady;
        public DateTime? ShippingDate;

        public string RefApprovedSamples;
        public string RefSwatchCard;
        public string RefLCCopy;
        public string RefCertificate;
        public string RefProductSpec;
        public string RefPOCopy;
        public string RefClientInstructionSheet;
        public string RefConstructionDataForm;
        public string RefDigitalImages;
        public string RefPackingListInvoice;
        public string RefRoadMap;

        public string FactorySupplierName;
        public string FactoryAddress;
        public string FactoryContactPerson;
        public string FactoryPhone;
        public string FactoryFax;
    }
    public class IAT
    {
        SqlConnection _sCon = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ConnectionString);

        //MAINS
        public int ExecuteNonQuery(string query, SqlParameter[] parameters)
        {
            using (SqlCommand cmd = new SqlCommand(query, _sCon))
            {
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                try
                {
                    if (_sCon.State != ConnectionState.Open) _sCon.Open();
                    return cmd.ExecuteNonQuery();
                }
                finally
                {
                    if (_sCon.State == ConnectionState.Open) _sCon.Close();
                }
            }
        }
        public object ExecuteScalar(string query, SqlParameter[] parameters)
        {
            using (SqlCommand cmd = new SqlCommand(query, _sCon))
            {
                if (parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                try
                {
                    if (_sCon.State != ConnectionState.Open) _sCon.Open();
                    return cmd.ExecuteScalar();
                }
                finally
                {
                    if (_sCon.State == ConnectionState.Open) _sCon.Close();
                }
            }
        }
        public DataTable GetDataUsingQuery(string sQuery)
        {
            DataTable dt = new DataTable();
            try
            {
                if (_sCon.State != ConnectionState.Open)
                {
                    _sCon.Open();
                }
                SqlCommand cmd = new SqlCommand(sQuery, _sCon);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);

            }
            catch
            {

            }
            return dt;
        }
        public DataTable GetDataUsingQueryWithParams(string sQuery, SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();
            try
            {
                if (_sCon.State != ConnectionState.Open)
                {
                    _sCon.Open();
                }
                SqlCommand cmd = new SqlCommand(sQuery, _sCon);
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
            }
            catch
            {

            }
            return dt;
        }

        // INSPECTION PLAN
        public int InsertInspectionPlan(string inspectionDate, string companyName, string companyAddress,
        string inspectionStandard, string inspectionType, string inspectionRequest,
        int organizationId, int createdBy)
        {
            string query = @"INSERT INTO InspectionPlan
                (InspectionDate, CompanyName, CompanyAddress, InspectionStandard, InspectionType, InspectionRequest, OrganizationID, IsActive, CreatedBy, CreatedDate)
                VALUES
                (@InspectionDate, @CompanyName, @CompanyAddress, @InspectionStandard, @InspectionType, @InspectionRequest, @OrganizationID, 1, @CreatedBy, GETDATE());
                SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@InspectionDate", string.IsNullOrWhiteSpace(inspectionDate) ? (object)DBNull.Value : inspectionDate),
                new SqlParameter("@CompanyName", companyName),
                new SqlParameter("@CompanyAddress", companyAddress),
                new SqlParameter("@InspectionStandard", string.IsNullOrWhiteSpace(inspectionStandard) ? (object)DBNull.Value : inspectionStandard),
                new SqlParameter("@InspectionType", string.IsNullOrWhiteSpace(inspectionType) ? (object)DBNull.Value : inspectionType),
                new SqlParameter("@InspectionRequest", string.IsNullOrWhiteSpace(inspectionRequest) ? (object)DBNull.Value : inspectionRequest),
                new SqlParameter("@OrganizationID", organizationId),
                new SqlParameter("@CreatedBy", createdBy)
            };

            return Convert.ToInt32(ExecuteScalar(query, parameters));
        }
        public void InsertInspectionPlanInspector(int inspectionPlanId, int sNo, string inspectorName)
        {
            string query = @"INSERT INTO InspectionPlanInspector (InspectionPlanID, SNo, InspectorName)
                VALUES (@InspectionPlanID, @SNo, @InspectorName)";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@InspectionPlanID", inspectionPlanId),
                new SqlParameter("@SNo", sNo),
                new SqlParameter("@InspectorName", inspectorName)
            };

            ExecuteNonQuery(query, parameters);
        }
        public void InsertInspectionPlanSchedule(int inspectionPlanId, int sNo, string dayLabel, object scheduleDate,
        string timeSlot, string personInCharge, string activity)
        {
            string query = @"INSERT INTO InspectionPlanSchedule
                (InspectionPlanID, SNo, DayLabel, ScheduleDate, TimeSlot, PersonInCharge, Activity)
                VALUES
                (@InspectionPlanID, @SNo, @DayLabel, @ScheduleDate, @TimeSlot, @PersonInCharge, @Activity)";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@InspectionPlanID", inspectionPlanId),
                new SqlParameter("@SNo", sNo),
                new SqlParameter("@DayLabel", string.IsNullOrWhiteSpace(dayLabel) ? (object)DBNull.Value : dayLabel),
                new SqlParameter("@ScheduleDate", scheduleDate ?? DBNull.Value),
                new SqlParameter("@TimeSlot", string.IsNullOrWhiteSpace(timeSlot) ? (object)DBNull.Value : timeSlot),
                new SqlParameter("@PersonInCharge", string.IsNullOrWhiteSpace(personInCharge) ? (object)DBNull.Value : personInCharge),
                new SqlParameter("@Activity", string.IsNullOrWhiteSpace(activity) ? (object)DBNull.Value : activity)
            };

            ExecuteNonQuery(query, parameters);
        }

        //GENERATE INSPECTION PLAN
        public DataTable GetInspectionPlanDetails(int inspectionPlanId)
        {
            string query = @"SELECT InspectionDate, CompanyName, CompanyAddress, InspectionStandard, InspectionType, InspectionRequest
        FROM InspectionPlan WHERE InspectionPlanID = @InspectionPlanID";

            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@InspectionPlanID", inspectionPlanId)
            };

            return GetDataUsingQueryWithParams(query, parameters);
        }
        public DataTable GetInspectionPlanInspectors(int inspectionPlanId)
        {
            string query = @"SELECT SNo, InspectorName FROM InspectionPlanInspector
        WHERE InspectionPlanID = @InspectionPlanID ORDER BY SNo";

            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@InspectionPlanID", inspectionPlanId)
            };

            return GetDataUsingQueryWithParams(query, parameters);
        }
        public DataTable GetInspectionPlanSchedules(int inspectionPlanId)
        {
            string query = @"SELECT SNo, DayLabel, ScheduleDate, TimeSlot, PersonInCharge, Activity
        FROM InspectionPlanSchedule WHERE InspectionPlanID = @InspectionPlanID ORDER BY SNo";

            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@InspectionPlanID", inspectionPlanId)
            };

            return GetDataUsingQueryWithParams(query, parameters);
        }

        //TOOL VERIFICATION
        public int InsertToolVerification(string verificationDate, string equipmentName, string equipmentCode,
        string city, string verifierName, int organizationId, int createdBy)
        {
            string query = @"INSERT INTO EquipmentVerification
                (VerificationDate, EquipmentName, EquipmentCode, City, VerifierName, OrganizationID, CreatedBy, CreatedDate)
                VALUES
                (@VerificationDate, @EquipmentName, @EquipmentCode, @City, @VerifierName, @OrganizationID, @CreatedBy, GETDATE());
                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@VerificationDate", string.IsNullOrWhiteSpace(verificationDate) ? (object)DBNull.Value : Convert.ToDateTime(verificationDate)),
                new SqlParameter("@EquipmentName", equipmentName),
                new SqlParameter("@EquipmentCode", equipmentCode),
                new SqlParameter("@City", string.IsNullOrWhiteSpace(city) ? (object)DBNull.Value : city),
                new SqlParameter("@VerifierName", string.IsNullOrWhiteSpace(verifierName) ? (object)DBNull.Value : verifierName),
                new SqlParameter("@OrganizationID", organizationId),
                new SqlParameter("@CreatedBy", createdBy)
            };
            object result = ExecuteScalar(query, parameters);
            return Convert.ToInt32(result);
        }
        public void InsertToolVerificationDetail(int verificationId, int sNo, string question,
        string actualCondition, string supportingDocument, string verificationResult)
        {
            string query = @"INSERT INTO EquipmentVerificationDetail
                (VerificationID, SNo, Question, ActualCondition, SupportingDocument, VerificationResult)
                VALUES
                (@VerificationID, @SNo, @Question, @ActualCondition, @SupportingDocument, @VerificationResult)";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@VerificationID", verificationId),
                new SqlParameter("@SNo", sNo),
                new SqlParameter("@Question", question),
                new SqlParameter("@ActualCondition", string.IsNullOrWhiteSpace(actualCondition) ? (object)DBNull.Value : actualCondition),
                new SqlParameter("@SupportingDocument", string.IsNullOrWhiteSpace(supportingDocument) ? (object)DBNull.Value : supportingDocument),
                new SqlParameter("@VerificationResult", string.IsNullOrWhiteSpace(verificationResult) ? (object)DBNull.Value : verificationResult)
            };
            ExecuteNonQuery(query, parameters);
        }

        //GENERATE TOOL VERIFICATION
        public DataTable GetToolVerificationById(int verificationId)
        {
            string query = @"SELECT * FROM EquipmentVerification WHERE VerificationID = @VerificationID";
            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@VerificationID", verificationId)
            };
            return GetDataUsingQueryWithParams(query, parameters);
        }
        public DataTable GetToolVerificationDetails(int verificationId)
        {
            string query = @"SELECT SNo, Question, ActualCondition, SupportingDocument, VerificationResult
        FROM EquipmentVerificationDetail
        WHERE VerificationID = @VerificationID
        ORDER BY SNo";
            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@VerificationID", verificationId)
            };
            return GetDataUsingQueryWithParams(query, parameters);
        }

        //TOOL HANDOVER
        public int InsertToolHandover(string handoverDate, string giverName, string department, string recipientName,
        string equipmentName, string inventoryCode, string specification, int quantity, string plannedReturnDate,
        string borrowDate, string borrowSignatoryName, string borrowCondition,
        string actualReturnDate, string returnSignatoryName, string returnCondition,
        int organizationId, int createdBy)
        {
            string query = @"INSERT INTO EquipmentHandover
                (HandoverDate, GiverName, Department, RecipientName, EquipmentName, InventoryCode, Specification, Quantity,
                 PlannedReturnDate, BorrowDate, BorrowSignatoryName, BorrowCondition, ActualReturnDate, ReturnSignatoryName,
                 ReturnCondition, OrganizationID, CreatedBy, CreatedDate)
                VALUES
                (@HandoverDate, @GiverName, @Department, @RecipientName, @EquipmentName, @InventoryCode, @Specification, @Quantity,
                 @PlannedReturnDate, @BorrowDate, @BorrowSignatoryName, @BorrowCondition, @ActualReturnDate, @ReturnSignatoryName,
                 @ReturnCondition, @OrganizationID, @CreatedBy, GETDATE());
                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@HandoverDate", string.IsNullOrWhiteSpace(handoverDate) ? (object)DBNull.Value : Convert.ToDateTime(handoverDate)),
                new SqlParameter("@GiverName", giverName),
                new SqlParameter("@Department", department),
                new SqlParameter("@RecipientName", recipientName),
                new SqlParameter("@EquipmentName", equipmentName),
                new SqlParameter("@InventoryCode", inventoryCode),
                new SqlParameter("@Specification", string.IsNullOrWhiteSpace(specification) ? (object)DBNull.Value : specification),
                new SqlParameter("@Quantity", quantity),
                new SqlParameter("@PlannedReturnDate", string.IsNullOrWhiteSpace(plannedReturnDate) ? (object)DBNull.Value : Convert.ToDateTime(plannedReturnDate)),
                new SqlParameter("@BorrowDate", string.IsNullOrWhiteSpace(borrowDate) ? (object)DBNull.Value : Convert.ToDateTime(borrowDate)),
                new SqlParameter("@BorrowSignatoryName", string.IsNullOrWhiteSpace(borrowSignatoryName) ? (object)DBNull.Value : borrowSignatoryName),
                new SqlParameter("@BorrowCondition", string.IsNullOrWhiteSpace(borrowCondition) ? (object)DBNull.Value : borrowCondition),
                new SqlParameter("@ActualReturnDate", string.IsNullOrWhiteSpace(actualReturnDate) ? (object)DBNull.Value : Convert.ToDateTime(actualReturnDate)),
                new SqlParameter("@ReturnSignatoryName", string.IsNullOrWhiteSpace(returnSignatoryName) ? (object)DBNull.Value : returnSignatoryName),
                new SqlParameter("@ReturnCondition", string.IsNullOrWhiteSpace(returnCondition) ? (object)DBNull.Value : returnCondition),
                new SqlParameter("@OrganizationID", organizationId),
                new SqlParameter("@CreatedBy", createdBy)
            };
            object result = ExecuteScalar(query, parameters);
            return Convert.ToInt32(result);
        }

        //GENERATE TOOL HANDOVER
        public DataTable GetToolHandoverById(int handoverId)
        {
            string query = @"SELECT * FROM EquipmentHandover WHERE HandoverID = @HandoverID";
            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@HandoverID", handoverId)
            };
            return GetDataUsingQueryWithParams(query, parameters);
        }

        //INSPECTION REPORT
        public int InsertInspectionReport(string workOrderNo, string reportNo, string companyName, string companyAddress,
        string letterNo, string productName, string productBrand, string productType, string productSpecification,
        string inspectedLocation, string roadClassification, string totalAreaInspected, string numberOfPointsInspected,
        string inspectionDate, string inspectedBy, string standardDocument, string notes,
        string conclusionResult, string measurementResultNotes, int organizationId, int createdBy)
        {
            string query = @"INSERT INTO InspectionReport
                (WorkOrderNo, ReportNo, CompanyName, CompanyAddress, LetterNo, ProductName, ProductBrand, ProductType, ProductSpecification,
                 InspectedLocation, RoadClassification, TotalAreaInspected, NumberOfPointsInspected, InspectionDate, InspectedBy,
                 StandardDocument, Notes, ConclusionResult, MeasurementResultNotes, OrganizationID, IsActive, CreatedBy, CreatedDate)
                VALUES
                (@WorkOrderNo, @ReportNo, @CompanyName, @CompanyAddress, @LetterNo, @ProductName, @ProductBrand, @ProductType, @ProductSpecification,
                 @InspectedLocation, @RoadClassification, @TotalAreaInspected, @NumberOfPointsInspected, @InspectionDate, @InspectedBy,
                 @StandardDocument, @Notes, @ConclusionResult, @MeasurementResultNotes, @OrganizationID, 1, @CreatedBy, GETDATE());
                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@WorkOrderNo", string.IsNullOrWhiteSpace(workOrderNo) ? (object)DBNull.Value : workOrderNo),
                new SqlParameter("@ReportNo", string.IsNullOrWhiteSpace(reportNo) ? (object)DBNull.Value : reportNo),
                new SqlParameter("@CompanyName", string.IsNullOrWhiteSpace(companyName) ? (object)DBNull.Value : companyName),
                new SqlParameter("@CompanyAddress", string.IsNullOrWhiteSpace(companyAddress) ? (object)DBNull.Value : companyAddress),
                new SqlParameter("@LetterNo", string.IsNullOrWhiteSpace(letterNo) ? (object)DBNull.Value : letterNo),
                new SqlParameter("@ProductName", string.IsNullOrWhiteSpace(productName) ? (object)DBNull.Value : productName),
                new SqlParameter("@ProductBrand", string.IsNullOrWhiteSpace(productBrand) ? (object)DBNull.Value : productBrand),
                new SqlParameter("@ProductType", string.IsNullOrWhiteSpace(productType) ? (object)DBNull.Value : productType),
                new SqlParameter("@ProductSpecification", string.IsNullOrWhiteSpace(productSpecification) ? (object)DBNull.Value : productSpecification),
                new SqlParameter("@InspectedLocation", string.IsNullOrWhiteSpace(inspectedLocation) ? (object)DBNull.Value : inspectedLocation),
                new SqlParameter("@RoadClassification", string.IsNullOrWhiteSpace(roadClassification) ? (object)DBNull.Value : roadClassification),
                new SqlParameter("@TotalAreaInspected", string.IsNullOrWhiteSpace(totalAreaInspected) ? (object)DBNull.Value : totalAreaInspected),
                new SqlParameter("@NumberOfPointsInspected", string.IsNullOrWhiteSpace(numberOfPointsInspected) ? (object)DBNull.Value : numberOfPointsInspected),
                new SqlParameter("@InspectionDate", string.IsNullOrWhiteSpace(inspectionDate) ? (object)DBNull.Value : Convert.ToDateTime(inspectionDate)),
                new SqlParameter("@InspectedBy", string.IsNullOrWhiteSpace(inspectedBy) ? (object)DBNull.Value : inspectedBy),
                new SqlParameter("@StandardDocument", string.IsNullOrWhiteSpace(standardDocument) ? (object)DBNull.Value : standardDocument),
                new SqlParameter("@Notes", string.IsNullOrWhiteSpace(notes) ? (object)DBNull.Value : notes),
                new SqlParameter("@ConclusionResult", string.IsNullOrWhiteSpace(conclusionResult) ? (object)DBNull.Value : conclusionResult),
                new SqlParameter("@MeasurementResultNotes", string.IsNullOrWhiteSpace(measurementResultNotes) ? (object)DBNull.Value : measurementResultNotes),
                new SqlParameter("@OrganizationID", organizationId),
                new SqlParameter("@CreatedBy", createdBy)
            };
            object result = ExecuteScalar(query, parameters);
            return Convert.ToInt32(result);
        }
        public void InsertInspectionReportTool(int inspectionReportId, int sNo, string toolName)
        {
            string query = @"INSERT INTO InspectionReportTool (InspectionReportID, SNo, ToolName)
                VALUES (@InspectionReportID, @SNo, @ToolName)";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@InspectionReportID", inspectionReportId),
                new SqlParameter("@SNo", sNo),
                new SqlParameter("@ToolName", toolName)
            };
            ExecuteNonQuery(query, parameters);
        }
        public void InsertInspectionReportPicture(int inspectionReportId, int sNo, string label, string caption, string filePath)
        {
            string query = @"INSERT INTO InspectionReportPicture (InspectionReportID, SNo, Label, Caption, FilePath)
                VALUES (@InspectionReportID, @SNo, @Label, @Caption, @FilePath)";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@InspectionReportID", inspectionReportId),
                new SqlParameter("@SNo", sNo),
                new SqlParameter("@Label", string.IsNullOrWhiteSpace(label) ? (object)DBNull.Value : label),
                new SqlParameter("@Caption", string.IsNullOrWhiteSpace(caption) ? (object)DBNull.Value : caption),
                new SqlParameter("@FilePath", filePath)
            };
            ExecuteNonQuery(query, parameters);
        }
        public void InsertInspectionReportConclusion(int inspectionReportId, int sNo, string clause,
        string inspectionDescription, string standardMethod, string result)
        {
            string query = @"INSERT INTO InspectionReportConclusion
                (InspectionReportID, SNo, Clause, InspectionDescription, StandardMethod, Result)
                VALUES (@InspectionReportID, @SNo, @Clause, @InspectionDescription, @StandardMethod, @Result)";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@InspectionReportID", inspectionReportId),
                new SqlParameter("@SNo", sNo),
                new SqlParameter("@Clause", string.IsNullOrWhiteSpace(clause) ? (object)DBNull.Value : clause),
                new SqlParameter("@InspectionDescription", inspectionDescription),
                new SqlParameter("@StandardMethod", standardMethod),
                new SqlParameter("@Result", string.IsNullOrWhiteSpace(result) ? (object)DBNull.Value : result)
            };
            ExecuteNonQuery(query, parameters);
        }
        public void InsertInspectionReportAreaDescription(int inspectionReportId, int sNo, string label, string value)
        {
            string query = @"INSERT INTO InspectionReportAreaDescription (InspectionReportID, SNo, Label, Value)
                VALUES (@InspectionReportID, @SNo, @Label, @Value)";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@InspectionReportID", inspectionReportId),
                new SqlParameter("@SNo", sNo),
                new SqlParameter("@Label", label),
                new SqlParameter("@Value", string.IsNullOrWhiteSpace(value) ? (object)DBNull.Value : value)
            };
            ExecuteNonQuery(query, parameters);
        }
        public void InsertInspectionReportLayoutImage(int inspectionReportId, int sNo, string caption, string filePath)
        {
            string query = @"INSERT INTO InspectionReportLayoutImage (InspectionReportID, SNo, Caption, FilePath)
                VALUES (@InspectionReportID, @SNo, @Caption, @FilePath)";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@InspectionReportID", inspectionReportId),
                new SqlParameter("@SNo", sNo),
                new SqlParameter("@Caption", string.IsNullOrWhiteSpace(caption) ? (object)DBNull.Value : caption),
                new SqlParameter("@FilePath", filePath)
            };
            ExecuteNonQuery(query, parameters);
        }
        public void InsertInspectionReportMeasurement(int inspectionReportId, int sNo, string pointNo,
        string illuminance, string luminance, string notes)
        {
            string query = @"INSERT INTO InspectionReportMeasurement
                (InspectionReportID, SNo, PointNo, Illuminance, Luminance, Notes)
                VALUES (@InspectionReportID, @SNo, @PointNo, @Illuminance, @Luminance, @Notes)";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@InspectionReportID", inspectionReportId),
                new SqlParameter("@SNo", sNo),
                new SqlParameter("@PointNo", string.IsNullOrWhiteSpace(pointNo) ? (object)DBNull.Value : pointNo),
                new SqlParameter("@Illuminance", string.IsNullOrWhiteSpace(illuminance) ? (object)DBNull.Value : illuminance),
                new SqlParameter("@Luminance", string.IsNullOrWhiteSpace(luminance) ? (object)DBNull.Value : luminance),
                new SqlParameter("@Notes", string.IsNullOrWhiteSpace(notes) ? (object)DBNull.Value : notes)
            };
            ExecuteNonQuery(query, parameters);
        }
        public void InsertInspectionReportSummary(int inspectionReportId, int sNo, string measurementPoint,
        string averageValue, string minimumValue, string maximumValue, string standardAverage, string conclusion)
        {
            string query = @"INSERT INTO InspectionReportSummary
                (InspectionReportID, SNo, MeasurementPoint, AverageValue, MinimumValue, MaximumValue, StandardAverage, Conclusion)
                VALUES (@InspectionReportID, @SNo, @MeasurementPoint, @AverageValue, @MinimumValue, @MaximumValue, @StandardAverage, @Conclusion)";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@InspectionReportID", inspectionReportId),
                new SqlParameter("@SNo", sNo),
                new SqlParameter("@MeasurementPoint", measurementPoint),
                new SqlParameter("@AverageValue", string.IsNullOrWhiteSpace(averageValue) ? (object)DBNull.Value : averageValue),
                new SqlParameter("@MinimumValue", string.IsNullOrWhiteSpace(minimumValue) ? (object)DBNull.Value : minimumValue),
                new SqlParameter("@MaximumValue", string.IsNullOrWhiteSpace(maximumValue) ? (object)DBNull.Value : maximumValue),
                new SqlParameter("@StandardAverage", string.IsNullOrWhiteSpace(standardAverage) ? (object)DBNull.Value : standardAverage),
                new SqlParameter("@Conclusion", string.IsNullOrWhiteSpace(conclusion) ? (object)DBNull.Value : conclusion)
            };
            ExecuteNonQuery(query, parameters);
        }
        public void InsertInspectionReportAppendix(int inspectionReportId, int sNo, string title, string filePath)
        {
            string query = @"INSERT INTO InspectionReportAppendix (InspectionReportID, SNo, Title, FilePath)
                VALUES (@InspectionReportID, @SNo, @Title, @FilePath)";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@InspectionReportID", inspectionReportId),
                new SqlParameter("@SNo", sNo),
                new SqlParameter("@Title", string.IsNullOrWhiteSpace(title) ? (object)DBNull.Value : title),
                new SqlParameter("@FilePath", filePath)
            };
            ExecuteNonQuery(query, parameters);
        }

        //GENERATE INSPECTION REPORT
        public DataTable GetInspectionReportById(int inspectionReportId)
        {
            DataTable dt = new DataTable();
            string query = @"
            SELECT ir.*,
            (SELECT SNo, ToolName
             FROM InspectionReportTool t
             WHERE t.InspectionReportID = ir.InspectionReportID
             ORDER BY SNo FOR JSON PATH) AS ToolsJSON,
            (SELECT SNo, Label, Caption, FilePath
             FROM InspectionReportPicture p
             WHERE p.InspectionReportID = ir.InspectionReportID
             ORDER BY SNo FOR JSON PATH) AS PicturesJSON,
            (SELECT SNo, Clause, InspectionDescription, StandardMethod, Result
             FROM InspectionReportConclusion c
             WHERE c.InspectionReportID = ir.InspectionReportID
             ORDER BY SNo FOR JSON PATH) AS ConclusionsJSON,
            (SELECT SNo, Label, Value
             FROM InspectionReportAreaDescription a
             WHERE a.InspectionReportID = ir.InspectionReportID
             ORDER BY SNo FOR JSON PATH) AS AreaDescriptionsJSON,
            (SELECT SNo, Caption, FilePath
             FROM InspectionReportLayoutImage l
             WHERE l.InspectionReportID = ir.InspectionReportID
             ORDER BY SNo FOR JSON PATH) AS LayoutImagesJSON,
            (SELECT SNo, PointNo, Illuminance, Luminance, Notes
             FROM InspectionReportMeasurement m
             WHERE m.InspectionReportID = ir.InspectionReportID
             ORDER BY SNo FOR JSON PATH) AS MeasurementsJSON,
            (SELECT SNo, MeasurementPoint, AverageValue, MinimumValue, MaximumValue, StandardAverage, Conclusion
             FROM InspectionReportSummary s
             WHERE s.InspectionReportID = ir.InspectionReportID
             ORDER BY SNo FOR JSON PATH) AS SummaryJSON,
            (SELECT SNo, Title, FilePath
             FROM InspectionReportAppendix ap
             WHERE ap.InspectionReportID = ir.InspectionReportID
             ORDER BY SNo FOR JSON PATH) AS AppendixJSON
            FROM InspectionReport ir
            WHERE ir.InspectionReportID = " + inspectionReportId;

            dt = GetDataUsingQuery(query);
            return dt;
        }
        public void UpdateInspectionReportQRLink(int inspectionReportId, string qrUrl)
        {
            string query = @"
          UPDATE [LIMSBISS].[dbo].[InspectionReport]
          SET    [QRLink]       = @QRLink,
                 [ModifiedDate] = @ModifiedDate
          WHERE  [InspectionReportID] = @InspectionReportID";
            SqlParameter[] parameters = new SqlParameter[]
            {
          new SqlParameter("@QRLink", qrUrl),
          new SqlParameter("@ModifiedDate", DateTime.Now),
          new SqlParameter("@InspectionReportID", inspectionReportId)
            };
            ExecuteNonQuery(query, parameters);
        }

        //INSPECTION REPORT LIST
        public DataTable GetInspectionReportList(int organizationId, string searchCode, string searchText, string conclusionResult)
        {
            string query = @"
            SELECT ir.InspectionReportID, ir.WorkOrderNo, ir.ReportNo, ir.CompanyName, ir.ProductName,
                   ir.InspectionDate, ir.ConclusionResult, ir.CreatedDate,
                   e.EmployeeName AS CreatedByName
            FROM InspectionReport ir
            LEFT JOIN Employees e ON e.ID = ir.CreatedBy
            WHERE ir.IsActive = 1
              AND ir.OrganizationID = @OrganizationID
              AND (@SearchCode = '' OR ir.WorkOrderNo LIKE '%' + @SearchCode + '%' OR ir.ReportNo LIKE '%' + @SearchCode + '%')
              AND (@SearchText = '' OR ir.CompanyName LIKE '%' + @SearchText + '%' OR ir.ProductName LIKE '%' + @SearchText + '%')
              AND (@ConclusionResult = '' OR ir.ConclusionResult = @ConclusionResult)
            ORDER BY ir.InspectionReportID DESC";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@OrganizationID", organizationId),
                new SqlParameter("@SearchCode", searchCode ?? ""),
                new SqlParameter("@SearchText", searchText ?? ""),
                new SqlParameter("@ConclusionResult", conclusionResult ?? "")
            };

            return GetDataUsingQueryWithParams(query, parameters);
        }

    }
}
