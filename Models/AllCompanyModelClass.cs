using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LIMS.Models
{
    public class AllCompanyModelClass
    {

    }

    public class MasterCompanyData
    {
        public int MasterCompanyId { get; set; }
        public string EmailId { get; set; }
        public string Password { get; set; }
        public string SecurityId { get; set; }
        public string CompanyName { get; set; }
        public int RoleId { get; set; }
        public int AccountStatus { get; set; }
        public int NumberofUsersId { get; set; }
        public int LicenseCounter { get; set; }
        public int LicenseYear { get; set; }
        public string LicenseNumber { get; set; }
        public int LicenseValidityId { get; set; }
        public int LicenseStatusId { get; set; }
        public int LicenseRenewRequest { get; set; }
        public string LicenseStartDate { get; set; }
        public string LicenseEndDate { get; set; }
        public int IsLicenseExpired { get; set; }
        public string LastLoginDateTime { get; set; }
        public string CreatedDateTime { get; set; }
        public string UpdatedDateTime { get; set; }
    }

    public class MasterCompanyDetails
    {
        public int MasterCompanyDetailsId { get; set; }
        public int MasterCompanyId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string Address { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string CountryName { get; set; }
        public string PINCode { get; set; }
        public string PANCardNo { get; set; }
        public string GSTNo { get; set; }
        public string StateGSTCode { get; set; }
        public string CompanyLogo { get; set; }
        public string CreatedDateTime { get; set; }
        public string UpdatedDateTime { get; set; }
    }

    public class AllMasterCompanyData
    {
        public int MasterCompanyId { get; set; }
        public string EmailId { get; set; }
        public string Password { get; set; }
        public string SecurityId { get; set; }
        public string CompanyName { get; set; }
        public string RoleName { get; set; }
        public int RoleId { get; set; }
        public int AccountStatus { get; set; }
        public int NumberofUsersId { get; set; }
        public int LicenseCounter { get; set; }
        public int LicenseYear { get; set; }
        public string LicenseNumber { get; set; }
        public int LicenseValidityId { get; set; }
        public int LicenseStatusId { get; set; }
        public int LicenseRenewRequest { get; set; }
        public string LicenseStartDate { get; set; }
        public string LicenseEndDate { get; set; }
        public int IsLicenseExpired { get; set; }
        public string LastLoginDateTime { get; set; }
        public string CreatedDateTime { get; set; }
        public string UpdatedDateTime { get; set; }


        public int MasterCompanyDetailsId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PINCode { get; set; }
        public string PANCardNo { get; set; }
        public string GSTNo { get; set; }
        public string StateGSTCode { get; set; }
        public string CompanyLogo { get; set; }

        public int TotalMonths { get; set; }
        public string LicensePeriodData { get; set; }
        public string MaxUserLimit { get; set; }
        public string UserLimitSetData { get; set; }




    }

    public class MasterEmployeeData
    {
        public int EmployeeId { get; set; }
        public string EmailId { get; set; }
        public string Password { get; set; }
        public string SecurityID { get; set; }
        public int RoleId { get; set; }
        public int MasterCompanyId { get; set; }
        public int CreatedEmployeeId { get; set; }
        public int HODEmployeeRoleId { get; set; }
        public int HODEmployeeId { get; set; }
        public int IsLicenseExpired { get; set; }
        public int IsUserDeleted { get; set; }

        public int UnitId { get; set; }
        public string AccountStatus { get; set; }
        public string CurrentActiveMode { get; set; }
        public string LastLoginDateTime { get; set; }
        public string CreatedDateTime { get; set; }
        public string UpdatedDateTime { get; set; }
    }

    public class MasterEmployeeDetail
    {
        public int EDId { get; set; }
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ProfilePicture { get; set; }
        public string CreatedDateTime { get; set; }
        public string UpdatedDateTime { get; set; }
        public int MasterCompanyId { get; set; }
    }

    public class AllEmployeeDetailsData
    {
        public int EmployeeId { get; set; }
        public string EmailId { get; set; }
        public string Password { get; set; }
        public string SecurityID { get; set; }
        public string RoleName { get; set; }
        public int RoleId { get; set; }
        public int MasterCompanyId { get; set; }
        public int CreatedEmployeeId { get; set; }
        public int HODEmployeeRoleId { get; set; }
        public int HODEmployeeId { get; set; }
        public int IsLicenseExpired { get; set; }
        public int IsUserDeleted { get; set; }

        public int UnitId { get; set; }
        public int AccountStatus { get; set; }
        public string CurrentActiveMode { get; set; }
        public string LastLoginDateTime { get; set; }
        public string CreatedDateTime { get; set; }
        public string UpdatedDateTime { get; set; }

        public int EDId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ProfilePicture { get; set; }
        public string CompanyName { get; set; }

        public string PINCode { get; set; }
        public string Country { get; set; }
        public string State { get; set; }

        public string UnitName { get; set; }
        public string BranchName { get; set; }
    }

    public class LicenseRenewalRequestData
    {
        public int MasterCompanyID { get; set; }
        public int LicenseRenewRequestID { get; set; }
        public string Remarks { get; set; }
        public string CompanyName { get; set; }
        public string LicenseNumber { get; set; }
        public string LicenseExpireDate { get; set; }
        public int RequestStatusId { get; set; }
        public string RequestStatus { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
    }

    public class LicenseRenewalData
    {
        public int RenewalId { get; set; }
        public int MasterCompanyID { get; set; }
        public string LicenseNumber { get; set; }
        public int LicenseValidityId { get; set; }
        public int NumberofUsersId { get; set; }
        public string LicenseStartDate { get; set; }
        public string LicenseEndDate { get; set; }
        public int LicenseStatusId { get; set; }
        public int LicenseCounter { get; set; }
        public int LicenseYear { get; set; }
        public string CreatedDateTime { get; set; }
        public string UpdatedDateTime { get; set; }
    }

    public class LicenseValidityData
    {
        public int LicenseValidityId { get; set; }
        public string LicensePeriodData { get; set; }
        public int TotalMonths { get; set; }
    }

    public class AdminUserLimitData
    {
        public int UserLimitSetId { get; set; }
        public string UserLimitSetData { get; set; }
        public int LimitMaxNumber { get; set; }
    }

    public class LicenseRenewalAllData
    {
        public int RenewalId { get; set; }
        public int MasterCompanyID { get; set; }
        public string LicenseNumber { get; set; }
        public int LicenseValidityId { get; set; }
        public int NumberofUsersId { get; set; }
        public string LicenseStartDate { get; set; }
        public string LicenseEndDate { get; set; }
        public int LicenseStatusId { get; set; }
        public int LicenseCounter { get; set; }
        public int LicenseYear { get; set; }
        public string CreatedDateTime { get; set; }
        public string UpdatedDateTime { get; set; }
        public string LicensePeriodData { get; set; }
        public int TotalMonths { get; set; }
        public int UserLimitSetId { get; set; }
        public string UserLimitSetData { get; set; }
        public int LimitMaxNumber { get; set; }

        public string RenewDate { get; set; }
        public string CompanyName { get; set; }
    }

    public class CategoryNameData
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int CategoryMasterCompanyId { get; set; }
        public int CategoryCreatedEmployeeId { get; set; }
        public string CategoryCreatedDateTime { get; set; }
        public string CategoryUpdatedDateTime { get; set; }
    }

    public class RackTypeDetailsData
    {
        public int RackTypeId { get; set; }
        public string RackType { get; set; }
        public int RackCompanyUnitId { get; set; }
        public int RackMasterCompanyId { get; set; }
        public int RackCreatedEmployeeId { get; set; }
        public string RackCreatedDateTime { get; set; }
        public string RackUpdatedDateTime { get; set; }

        public string UnitName { get; set; }

    }


    public class RackNumberData
    {
        public int RackNumberId { get; set; }
        public int RackTypeId { get; set; }
        public int RackNumber { get; set; }
        public int TotalRackNumber { get; set; }

        public int CompanyUnitId { get; set; }
        public int CreatedEmployeeId { get; set; }
        public int MasterCompanyId { get; set; }

        public string ItemCode { get; set; }

        public string RackType { get; set; }
        public int RackCompanyUnitId { get; set; }
        public int RackMasterCompanyId { get; set; }
        public int RackCreatedEmployeeId { get; set; }
        public string RackCreatedDateTime { get; set; }
        public string RackUpdatedDateTime { get; set; }

        public string BranchName { get; set; }
        public string UnitName { get; set; }

        public int TotalRackRowNumber { get; set; }


        public int RackRowId { get; set; }
        public int RackRowNumber { get; set; }


        public int RackSerialId { get; set; }
        public int TotalSerialNumber { get; set; }
        public string RackAlphSeries { get; set; }
        public string RackSerialNumber { get; set; }
        public int SerialNumberId { get; set; }



        public int IS_ItemStoreId { get; set; }
        public int IS_ITId { get; set; }
        public string IS_RackSerialNo { get; set; }
        public decimal IS_StoreQty { get; set; }
        public string IS_GRNNUMBER { get; set; }
    }


    public class RackSerialDetail
    {
        public int RackSerialId { get; set; }
        public int TotalSerialNumber { get; set; }
        public string RackAlphSeries { get; set; }
        public string RackSerialNumber { get; set; }
        public int SerialNumberId { get; set; }
        public int RackRowId { get; set; }
        public int RackTypeId { get; set; }
        public int RackNumberId { get; set; }
        public int CompanyUnitId { get; set; }
        public int CreatedEmployeeId { get; set; }
        public int MasterCompanyId { get; set; }
    }


    public class SerialNumberMasterData
    {
        public int SerialNumberId { get; set; }
        public string SerialNumber { get; set; }
    }

    public class VendorDetailsData
    {
        public int VendorId { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string VendorEmail { get; set; }
        public string VendorMobileNumber { get; set; }
        public string VendorCompanyName { get; set; }
        public string VendorCompanyAddress { get; set; }
        public string VendorCompanyGSTNumber { get; set; }
        public string VendorCompanyPANNumber { get; set; }
        public string VendorCountry { get; set; }
        public string VendorState { get; set; }
        public string VendorCity { get; set; }
        public string VendorPinCode { get; set; }
        public string VendorBusinessType { get; set; }
        public string VendorCreatedDateTime { get; set; }
        public string VendorUpdatedDateTime { get; set; }
        public int VendorIsActive { get; set; }
        public int VendorMasterCompanyId { get; set; }
        public int VendorCreatedEmployeeId { get; set; }

        public string VendorNameAndCompanyName { get; set; }
    }

    public class UOMData
    {
        public int UOMId { get; set; }
        public string UOMCode { get; set; }
        public string UOMName { get; set; }
    }

    public class VATData
    {
        public int VATId { get; set; }
        public string VATCode { get; set; }
        public decimal Percentage { get; set; }
    }

    public class ItemMasterData
    {
        public int ItemId { get; set; }
        public int ItemCategoryId { get; set; }
        public int ItemUOMId { get; set; }
        public int ItemGSTId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public string ItemHSNCode { get; set; }
        public int ItemCreatedEmployeeId { get; set; }
        public int ItemMasterCompanyId { get; set; }
        public string ItemCreatedDateTime { get; set; }
        public string ItemUpdatedDateTime { get; set; }
        public string ItemImageName { get; set; }

        public string ItemTypeName { get; set; }

        public string CategoryName { get; set; }

        public int UOMId { get; set; }
        public string UOMCode { get; set; }
        public string UOMName { get; set; }

        public int VATID { get; set; }
        public string VATCode { get; set; }
        public decimal Percentage { get; set; }
    }

    public class Item
    {
        public int ItemID { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public int PurchaseSubGroupID { get; set; }
        public int LeadTime { get; set; }
        public int UOMID { get; set; }
        public decimal StockQuantity { get; set; }
        public decimal MinStock { get; set; }
        public decimal MaxStock { get; set; }
        public bool DisabledItem { get; set; }
        public string HSNCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal VAT { get; set; }
        public string Remarks { get; set; }
        public decimal UOMValue { get; set; }
        public int ConversionUOMID { get; set; }
        public decimal ConversionUOMValue { get; set; }
        public string ModelNo { get; set; }
        public decimal Rate { get; set; }
        public int ItemTypeID { get; set; }
        public string Description { get; set; }
        public int VATID { get; set; }
        public string Specification { get; set; }
        public int PurchaseGroupID { get; set; }
        public int CreatedBy { get; set; }
        public int ConvertUOM { get; set; }

        public string ItemTypeName { get; set; }
    }


    public class InventoryItemData
    {
        public int InventoryId { get; set; }

        public int InventoryCompanyUnitId { get; set; }
        public int InventoryRackTypeId { get; set; }
        public int InventoryItemCategoryId { get; set; }
        public int InventoryItemId { get; set; }
        public int InventoryUOMId { get; set; }
        public int InventoryGSTId { get; set; }
        public decimal InventoryCurrentStock { get; set; }
        public decimal InventoryMinStock { get; set; }
        public int InventoryCreatedEmployeeId { get; set; }
        public int InventoryMasterCompanyId { get; set; }
        public string InventoryItemLocationStore { get; set; }
        public string InventoryCreatedDateTime { get; set; }
        public string InventoryUpdatedDateTime { get; set; }

        public int ItemID { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public int PurchaseSubGroupID { get; set; }
        public int LeadTime { get; set; }
        public int UOMID { get; set; }
        public decimal StockQuantity { get; set; }
        public decimal MinStock { get; set; }
        public decimal MaxStock { get; set; }
        public bool DisabledItem { get; set; }
        public string HSNCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal VAT { get; set; }
        public string Remarks { get; set; }
        public decimal UOMValue { get; set; }
        public int ConversionUOMID { get; set; }
        public decimal ConversionUOMValue { get; set; }
        public string ModelNo { get; set; }
        public decimal Rate { get; set; }
        public int ItemTypeID { get; set; }
        public string Description { get; set; }
        public string Specification { get; set; }
        public int PurchaseGroupID { get; set; }
        public int CreatedBy { get; set; }
        public int ConvertUOM { get; set; }

        public string ItemTypeName { get; set; }

        public int VendorId { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string VendorEmail { get; set; }
        public string VendorMobileNumber { get; set; }
        public string VendorCompanyName { get; set; }
        public string VendorCompanyAddress { get; set; }
        public string VendorCompanyGSTNumber { get; set; }
        public string VendorCompanyPANNumber { get; set; }
        public string VendorCountry { get; set; }
        public string VendorState { get; set; }
        public string VendorCity { get; set; }
        public string VendorPinCode { get; set; }
        public string VendorBusinessType { get; set; }
        public string VendorCreatedDateTime { get; set; }
        public string VendorUpdatedDateTime { get; set; }
        public int VendorIsActive { get; set; }
        public int VendorMasterCompanyId { get; set; }
        public int VendorCreatedEmployeeId { get; set; }


        public int RackTypeId { get; set; }
        public string RackType { get; set; }
        public int RackMasterCompanyId { get; set; }
        public int RackCreatedEmployeeId { get; set; }
        public string RackCreatedDateTime { get; set; }
        public string RackUpdatedDateTime { get; set; }

        public int RackSerialId { get; set; }
        public int TotalSerialNumber { get; set; }
        public string RackAlphSeries { get; set; }
        public string RackSerialNumber { get; set; }
        public int SerialNumberId { get; set; }


        public int ItemId { get; set; }
        public int ItemCategoryId { get; set; }
        public int ItemUOMId { get; set; }
        public int ItemGSTId { get; set; }
        //public string ItemCode { get; set; }
        //public string ItemName { get; set; }
        //public string ItemDescription { get; set; }
        public string ItemHSNCode { get; set; }

        public int ItemCreatedEmployeeId { get; set; }
        public int ItemMasterCompanyId { get; set; }
        public string ItemCreatedDateTime { get; set; }
        public string ItemUpdatedDateTime { get; set; }
        public string ItemImageName { get; set; }



        public int UOMId { get; set; }
        public string UOMCode { get; set; }
        public string UOMName { get; set; }
        public int VATID { get; set; }
        public string VATCode { get; set; }
        public decimal Percentage { get; set; }

        public string UnitName { get; set; }

    }


    public class ItemTransactionData
    {
        public int IT_Id { get; set; }
        public int IT_InventoryId { get; set; }
        public int IT_ItemId { get; set; }
        public int IT_VendorId { get; set; }
        public int IT_PoId { get; set; }
        public int IT_UOMId { get; set; }
        public int IT_GSTId { get; set; }
        public int IT_CompanyUnitId { get; set; }
        public int IT_RackTypeId { get; set; }
        public decimal IT_BasicPrice { get; set; }
        public decimal IT_ReceivedQty { get; set; }

        public decimal IT_ShortFall { get; set; }
        public decimal IT_PreviousShortFall { get; set; }
        public decimal IT_ReceivedQtyInvoice { get; set; }
        public decimal IT_PreviousReceivedQtyInvoice { get; set; }

        public decimal IT_BalanceQty { get; set; }
        public decimal IT_TotalAmount { get; set; }
        public string IT_BatchNumber { get; set; }
        public string IT_ExpiryDateTime { get; set; }
        public string IT_DeliveryDateTime { get; set; }
        public string IT_CreatedDateTime { get; set; }
        public string IT_UpdatedDateTime { get; set; }
        public string IT_RackRowSeriesNo { get; set; }
        public int IT_CreatedEmployeeId { get; set; }
        public int IT_MasterCompanyId { get; set; }
        public string IT_PoNumber { get; set; }
        public decimal IT_PreviousReceivedQty { get; set; }
        public decimal IT_PreviousBalanceQty { get; set; }
        public decimal IT_AcceptedQty { get; set; }
        public decimal IT_RejectedQty { get; set; }
        public decimal IT_PreviousRejectedQty { get; set; }
        public decimal IT_PreviousAcceptedQty { get; set; }
        public decimal IT_OrderedQty { get; set; }
        public int IT_Status { get; set; }
        public string IT_QRCodeNumber { get; set; }
        public string IT_QRFileName { get; set; }
        //public int IT_CompanyUnitId { get; set; }
        public int IT_IsDelete { get; set; }
        public string IT_ProjectID { get; set; }
        public string IT_IgirNumber { get; set; }



        public decimal IT_ReturnQty { get; set; }
        public decimal IT_ReplaceQty { get; set; }
        public decimal IT_PreReturnQty { get; set; }
        public decimal IT_PreReplaceQty { get; set; }
        public int IT_isValid { get; set; }
        public string IT_ReturnDateTime { get; set; }
        public string IT_ReplaceDateTime { get; set; }



        public decimal TotalItemInRack { get; set; }



        public int InventoryId { get; set; }

        public int InventoryCompanyUnitId { get; set; }
        public int InventoryRackTypeId { get; set; }

        public int InventoryItemCategoryId { get; set; }
        public int InventoryItemId { get; set; }
        public int InventoryUOMId { get; set; }
        public int InventoryGSTId { get; set; }
        public decimal InventoryCurrentStock { get; set; }
        public decimal InventoryMinStock { get; set; }
        public int InventoryCreatedEmployeeId { get; set; }
        public int InventoryMasterCompanyId { get; set; }
        public string InventoryItemLocationStore { get; set; }
        public string InventoryCreatedDateTime { get; set; }
        public string InventoryUpdatedDateTime { get; set; }





        public int VendorId { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string VendorEmail { get; set; }
        public string VendorMobileNumber { get; set; }
        public string VendorCompanyName { get; set; }
        public string VendorCompanyAddress { get; set; }
        public string VendorCompanyGSTNumber { get; set; }
        public string VendorCompanyPANNumber { get; set; }
        public string VendorCountry { get; set; }
        public string VendorState { get; set; }
        public string VendorCity { get; set; }
        public string VendorPinCode { get; set; }
        public string VendorBusinessType { get; set; }
        public string VendorCreatedDateTime { get; set; }
        public string VendorUpdatedDateTime { get; set; }
        public int VendorIsActive { get; set; }
        public int VendorMasterCompanyId { get; set; }
        public int VendorCreatedEmployeeId { get; set; }


        public int RackTypeId { get; set; }
        public string RackType { get; set; }
        public int RackMasterCompanyId { get; set; }
        public int RackCreatedEmployeeId { get; set; }
        public string RackCreatedDateTime { get; set; }
        public string RackUpdatedDateTime { get; set; }

        public int RackSerialId { get; set; }
        public int TotalSerialNumber { get; set; }
        public string RackAlphSeries { get; set; }
        public string RackSerialNumber { get; set; }
        public int SerialNumberId { get; set; }
        public int RackRowId { get; set; }
        public int RackNumberId { get; set; }
        public int CompanyUnitId { get; set; }
        public int CreatedEmployeeId { get; set; }
        public int MasterCompanyId { get; set; }


        public int ItemId { get; set; }
        public int ItemCategoryId { get; set; }

        public int ItemUOMId { get; set; }
        public int ItemGSTId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public string ItemHSNCode { get; set; }

        public int ItemCreatedEmployeeId { get; set; }
        public int ItemMasterCompanyId { get; set; }
        public string ItemCreatedDateTime { get; set; }
        public string ItemUpdatedDateTime { get; set; }
        public string ItemImageName { get; set; }



        public int UOMId { get; set; }
        public string UOMCode { get; set; }
        public string UOMName { get; set; }



        public int VATID { get; set; }
        public string VATCode { get; set; }
        public decimal Percentage { get; set; }

        public string UnitName { get; set; }

        public int IS_ItemStoreId { get; set; }


        public int ExpiryRank { get; set; }
        public decimal CumulativeTotalItemInRack { get; set; }

        public int IsQtyGo { get; set; }

        public int BothMatch { get; set; }



        /* lims */


        public int PurchaseSubGroupID { get; set; }
        public int LeadTime { get; set; }
        public int UOMID { get; set; }
        public decimal StockQuantity { get; set; }
        public decimal MinStock { get; set; }
        public decimal MaxStock { get; set; }
        public bool DisabledItem { get; set; }
        public string HSNCode { get; set; }

        public decimal VAT { get; set; }

        public decimal UOMValue { get; set; }
        public int ConversionUOMID { get; set; }
        public decimal ConversionUOMValue { get; set; }
        public string ModelNo { get; set; }

        public int ItemTypeID { get; set; }
        public string Description { get; set; }

        public string Specification { get; set; }
        public int PurchaseGroupID { get; set; }
        public int CreatedBy { get; set; }
        public int ConvertUOM { get; set; }

        public string ItemTypeName { get; set; }


        public int? DealerID { get; set; }
        public string Website { get; set; }
        public string YearOfEstablishment { get; set; }
        public string TypeOfBusiness { get; set; }
        public string CompanyTradeLicence { get; set; }
        public decimal? YearlyTurnover1 { get; set; }
        public decimal? RequiredCredit { get; set; }
        public decimal? ApprovedCredit { get; set; }
        public string PanNumber { get; set; }
        public string AdharNumber { get; set; }
        public int? EstablishmentID { get; set; }
        public string RegistrationNumber { get; set; }
        public decimal? YearlyTurnover2 { get; set; }
        public decimal? YearlyTurnover3 { get; set; }
        public string Year1 { get; set; }
        public string Year2 { get; set; }
        public string Year3 { get; set; }
        public string UANNumber { get; set; }
        public string PFNumber { get; set; }
        public string NumberofEmployee { get; set; }
        public string IEC { get; set; }
        public string ESIC { get; set; }
        public string VendorStatus { get; set; }
        public string VendorCurrency { get; set; }

        public string CompanySizeID { get; set; }

        public string VendorNameAndCompanyName { get; set; }


        public decimal PO_OrderedQty { get; set; }
        public decimal PO_ReceivedQty { get; set; }
        public decimal PO_GettingStock { get; set; }
        public decimal PO_AcceptQty { get; set; }
        public decimal PO_RejectQty { get; set; }
        public decimal PO_BalanceQty { get; set; }
        public decimal PO_BasicPrice { get; set; }
        public decimal PO_GSTPercentage { get; set; }
        public decimal PO_ShortFall { get; set; }
        public string PO_VendorCode { get; set; }
        public string PO_OrderedDateTime { get; set; }
        public string PO_DeliveryDateTime { get; set; }
        public string PO_GeneratedDateTime { get; set; }
        public string PO_CreatedDateTime { get; set; }
        public string PO_UpdatedDateTime { get; set; }
        public int PO_StatusId { get; set; }
        public int PO_CompanyUnitId { get; set; }
        public int PO_CreatedEmployeeId { get; set; }
        public int PO_MasterCompanyId { get; set; }
        public int PO_IsActive { get; set; }
        public int PO_IsNotificationSend { get; set; }

        public decimal PO_ReturnQty { get; set; }
        public string PO_Status_Title { get; set; }

    }

    public class POMasterData
    {
        public int PoId { get; set; }
        public string PoNumber { get; set; }
        public int PoItemId { get; set; }
        public int PoUOMId { get; set; }
        public int PoGSTId { get; set; }
        public int PoVendorId { get; set; }
        public decimal PoOrderQty { get; set; }
        public decimal PoReceivedQty { get; set; }
        public decimal PoBalanceQty { get; set; }
        public decimal PoBasicPrice { get; set; }
        public decimal PoTotalAmount { get; set; }
        public string PoDeliveryDate { get; set; }
        public string PoOrderDate { get; set; }
        public string PoCreatedDateTime { get; set; }
        public string PoUpdatedDateTime { get; set; }
        public int PoCreatedEmployeeId { get; set; }
        public int PoMasterCompanyId { get; set; }
        public int PoStatusId { get; set; }

        public int PoUnitId { get; set; }


        public int VendorId { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string VendorEmail { get; set; }
        public string VendorMobileNumber { get; set; }
        public string VendorCompanyName { get; set; }
        public string VendorCompanyAddress { get; set; }
        public string VendorCompanyGSTNumber { get; set; }
        public string VendorCompanyPANNumber { get; set; }
        public string VendorCountry { get; set; }
        public string VendorState { get; set; }
        public string VendorCity { get; set; }
        public string VendorPinCode { get; set; }
        public string VendorBusinessType { get; set; }
        public string VendorCreatedDateTime { get; set; }
        public string VendorUpdatedDateTime { get; set; }
        public int VendorIsActive { get; set; }
        public int VendorMasterCompanyId { get; set; }
        public int VendorCreatedEmployeeId { get; set; }


        public int ItemId { get; set; }
        public int ItemCategoryId { get; set; }
        public int ItemUOMId { get; set; }
        public int ItemGSTId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public string ItemHSNCode { get; set; }
        public int ItemCreatedEmployeeId { get; set; }
        public int ItemMasterCompanyId { get; set; }
        public string ItemCreatedDateTime { get; set; }
        public string ItemUpdatedDateTime { get; set; }
        public string ItemImageName { get; set; }


        public int UOMId { get; set; }
        public string UOMCode { get; set; }
        public string UOMName { get; set; }


        public int VATID { get; set; }
        public string VATCode { get; set; }
        public decimal Percentage { get; set; }
    }
    [Serializable]
    public class PaymentRequisitionData
    {        
        public int PaymentRequisitionID { get; set; }
        public DateTime RequisitionDate { get; set; }
        public string RequisitionBy { get; set; }
        public string RequisitionNumber { get; set; }
        public string RequisitionType { get; set; }
        public string RequisitionSubType { get; set; }
        public string RequisitionStatus { get; set; }
        public string StatusID { get; set; }
        public decimal TotalAmount { get; set; }
    }
    public class GRNData
    {
        public int GRN_ID { get; set; }
        public string GRN_PoNumber { get; set; }
        public string GRN_Employee { get; set; }
        public string GRN_Status { get; set; }
        public string GRN_NUMBER { get; set; }
        public string GRN_CreatedDateTime { get; set; }
        public string VendorName { get; set; }  
    }
    public class GRNMasterData
    {
        public int GRN_ID { get; set; }
        public string GRN_PoNumber { get; set; }
        public int GRN_MasterCompanyId { get; set; }
        public int GRN_EmployeeID { get; set; }
        public int GRN_CompanyUnitId { get; set; }
        public int GRN_Status { get; set; }
        public int GRN_CounterYear { get; set; }
        public int GRN_CounterNumber { get; set; }
        public string GRN_NUMBER { get; set; }
        public string GRN_CreatedDateTime { get; set; }
        public string GRN_UpdatedDateTime { get; set; }
        public int GRN_ApprovedEmployeeID { get; set; }
        public string GRN_ApprovedDateTime { get; set; }
        public int GRN_ApprovedAccountantEmployeeID { get; set; }
        public string GRN_ApprovedDateTimebyAccountant { get; set; }



        public int GRNFileId { get; set; }
        public string GRNO { get; set; }
        public string GRNFileName { get; set; }
        public string GRNDocumentName { get; set; }

        public int GDITId { get; set; }
        public int GDPoId { get; set; }
        public int GRNDetailsID { get; set; }
        public string GRN { get; set; }
        public string GDPoNumber { get; set; }
        public string GDPoItemCode { get; set; }
        public string GDItemName { get; set; }

        //public decimal GDPoOrderQty { get; set; }
        //public decimal GDPoReceivedQty { get; set; }
        //public decimal GDPoBalanceQty { get; set; }

        public int GDStatusId { get; set; }

        public int IT_Id { get; set; }
        public int IT_InventoryId { get; set; }
        public int IT_ItemId { get; set; }
        public int IT_VendorId { get; set; }
        public int IT_PoId { get; set; }
        public int IT_UOMId { get; set; }
        public int IT_GSTId { get; set; }
        public int IT_CompanyUnitId { get; set; }
        public int IT_RackTypeId { get; set; }
        public decimal IT_BasicPrice { get; set; }
        public decimal IT_ReceivedQty { get; set; }

        public decimal IT_ShortFall { get; set; }
        public decimal IT_PreviousShortFall { get; set; }
        public decimal IT_ReceivedQtyInvoice { get; set; }
        public decimal IT_PreviousReceivedQtyInvoice { get; set; }

        public decimal IT_BalanceQty { get; set; }
        public decimal IT_TotalAmount { get; set; }
        public string IT_BatchNumber { get; set; }
        public string IT_ExpiryDateTime { get; set; }
        public string IT_DeliveryDateTime { get; set; }
        public string IT_CreatedDateTime { get; set; }
        public string IT_UpdatedDateTime { get; set; }
        public string IT_RackRowSeriesNo { get; set; }
        public int IT_CreatedEmployeeId { get; set; }
        public int IT_MasterCompanyId { get; set; }
        public string IT_PoNumber { get; set; }
        public decimal IT_PreviousReceivedQty { get; set; }
        public decimal IT_PreviousBalanceQty { get; set; }
        public decimal IT_AcceptedQty { get; set; }
        public decimal IT_RejectedQty { get; set; }
        public decimal IT_PreviousRejectedQty { get; set; }
        public decimal IT_PreviousAcceptedQty { get; set; }
        public decimal IT_OrderedQty { get; set; }
        public int IT_Status { get; set; }
        public string IT_QRCodeNumber { get; set; }
        public string IT_QRFileName { get; set; }
        //public int IT_CompanyUnitId { get; set; }

        public decimal IT_ReturnQty { get; set; }
        public decimal IT_ReplaceQty { get; set; }
        public decimal IT_PreReturnQty { get; set; }
        public decimal IT_PreReplaceQty { get; set; }
        public int IT_isValid { get; set; }
        public string IT_ReturnDateTime { get; set; }
        public string IT_ReplaceDateTime { get; set; }


        public int PoId { get; set; }
        public string PoNumber { get; set; }
        public int PoItemId { get; set; }
        public int PoUOMId { get; set; }
        public int PoGSTId { get; set; }
        public int PoVendorId { get; set; }
        public decimal PoOrderQty { get; set; }
        public decimal PoReceivedQty { get; set; }
        public decimal PoBalanceQty { get; set; }
        public decimal PoBasicPrice { get; set; }
        public decimal PoTotalAmount { get; set; }
        public string PoDeliveryDate { get; set; }
        public string PoOrderDate { get; set; }
        public string PoCreatedDateTime { get; set; }
        public string PoUpdatedDateTime { get; set; }
        public int PoCreatedEmployeeId { get; set; }
        public int PoMasterCompanyId { get; set; }
        public int PoStatusId { get; set; }
        public int PoUnitId { get; set; }



        public int PO_Id { get; set; }
        public string PO_Number { get; set; }
        public string PO_ItemCode { get; set; }
        public string PO_ItemName { get; set; }
        public decimal PO_OrderedQty { get; set; }
        public decimal PO_ReceivedQty { get; set; }
        public decimal PO_GettingStock { get; set; }
        public decimal PO_BalanceQty { get; set; }
        public decimal PO_BasicPrice { get; set; }
        public decimal PO_GSTPercentage { get; set; }
        public string PO_VendorCode { get; set; }
        public string PO_OrderedDateTime { get; set; }
        public string PO_DeliveryDateTime { get; set; }
        public string PO_GeneratedDateTime { get; set; }
        public string PO_CreatedDateTime { get; set; }
        public string PO_UpdatedDateTime { get; set; }
        public int PO_StatusId { get; set; }
        public int PO_CompanyUnitId { get; set; }
        public int PO_CreatedEmployeeId { get; set; }
        public int PO_MasterCompanyId { get; set; }
        public int PO_IsActive { get; set; }
        public int PO_IsNotificationSend { get; set; }

        public decimal PO_ReturnQty { get; set; }

        public int VendorId { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string VendorEmail { get; set; }
        public string VendorMobileNumber { get; set; }
        public string VendorCompanyName { get; set; }
        public string VendorCompanyAddress { get; set; }
        public string VendorCompanyGSTNumber { get; set; }
        public string VendorCompanyPANNumber { get; set; }
        public string VendorCountry { get; set; }
        public string VendorState { get; set; }
        public string VendorCity { get; set; }
        public string VendorPinCode { get; set; }
        public string VendorBusinessType { get; set; }
        public string VendorCreatedDateTime { get; set; }
        public string VendorUpdatedDateTime { get; set; }
        public int VendorIsActive { get; set; }
        public int VendorMasterCompanyId { get; set; }
        public int VendorCreatedEmployeeId { get; set; }


        public int ItemId { get; set; }
        public int ItemCategoryId { get; set; }

        public int ItemUOMId { get; set; }
        public int ItemGSTId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public string ItemHSNCode { get; set; }
        public int ItemCreatedEmployeeId { get; set; }
        public int ItemMasterCompanyId { get; set; }
        public string ItemCreatedDateTime { get; set; }
        public string ItemUpdatedDateTime { get; set; }
        public string ItemImageName { get; set; }


        public int UOMId { get; set; }
        public string UOMCode { get; set; }
        public string UOMName { get; set; }

        public string VATCode { get; set; }
        public decimal Percentage { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UnitName { get; set; }

        public string EmployeeName { get; set; }

        public string StatusTitle { get; set; }



        // new added


        public int? PO_StatusForIT { get; set; }
        public int PurchaseOrderID { get; set; }
        public string PONumber { get; set; }
        public DateTime? PODate { get; set; }
        public int? VendorID { get; set; }
        public string Unit { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string SpecialInstructions { get; set; }
        public string ReportFileName { get; set; }
        public decimal? GrandTotal { get; set; }
        public int? UnitID { get; set; }
        public string Delivery { get; set; }
        public string Transportation { get; set; }
        public string Payment { get; set; }
        public string Guarantee { get; set; }
        public string Remarks { get; set; }
        public int? ContactPersonID { get; set; }
        public int? PurchaseOrderStatus { get; set; }
        public string Reason { get; set; }
        public string POSeries { get; set; }
        public int? POyear { get; set; }
        public string StoreType { get; set; }
        public int? projectid { get; set; }
        public int? penaltyID { get; set; }

        public int PODetailsID { get; set; }

        //public decimal? Discount { get; set; }
        public string Discount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? Tax { get; set; }
        public decimal? TotalAmount { get; set; }

        public DateTime? DeliveryDate { get; set; }
        public int? VendorQuotationID { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Amount { get; set; }
        public int? ItemID { get; set; }
        public int? POStatusID { get; set; }
        public decimal? ReceivedQty { get; set; }
        public int? TransactionID { get; set; }
        public decimal? AllocatedStock { get; set; }
        public int? ReceivedBy { get; set; }
        public decimal? SecurityReceived { get; set; }
        public decimal? TaxAmount { get; set; }
        public int? VATID { get; set; }
        public int? VendorQuotationDetailsID { get; set; }
        public int? EnquiryDetailsID { get; set; }
        public string PurchaseComments { get; set; }

        public int PurchaseSubGroupID { get; set; }
        public int LeadTime { get; set; }
        public int UOMID { get; set; }
        public decimal StockQuantity { get; set; }
        public decimal MinStock { get; set; }
        public decimal MaxStock { get; set; }
        public bool DisabledItem { get; set; }
        public string HSNCode { get; set; }

        public decimal VAT { get; set; }

        public decimal UOMValue { get; set; }
        public int ConversionUOMID { get; set; }
        public decimal ConversionUOMValue { get; set; }
        public string ModelNo { get; set; }

        public int ItemTypeID { get; set; }
        public string Description { get; set; }

        public string Specification { get; set; }
        public int PurchaseGroupID { get; set; }
        public int CreatedBy { get; set; }
        public int ConvertUOM { get; set; }

        public string ItemType { get; set; }

        public int InventoryId { get; set; }

        public int InventoryCompanyUnitId { get; set; }
        public int InventoryRackTypeId { get; set; }
        public int InventoryItemCategoryId { get; set; }
        public int InventoryItemId { get; set; }
        public int InventoryUOMId { get; set; }
        public int InventoryGSTId { get; set; }
        public decimal InventoryCurrentStock { get; set; }
        public decimal InventoryMinStock { get; set; }
        public int InventoryCreatedEmployeeId { get; set; }
        public int InventoryMasterCompanyId { get; set; }
        public string InventoryItemLocationStore { get; set; }
        public string InventoryCreatedDateTime { get; set; }
        public string InventoryUpdatedDateTime { get; set; }


        public int? DealerID { get; set; }
        public string Website { get; set; }
        public string YearOfEstablishment { get; set; }
        public string TypeOfBusiness { get; set; }
        public string CompanyTradeLicence { get; set; }
        public decimal? YearlyTurnover1 { get; set; }
        public decimal? RequiredCredit { get; set; }
        public decimal? ApprovedCredit { get; set; }
        public string PanNumber { get; set; }
        public string AdharNumber { get; set; }
        public int? EstablishmentID { get; set; }
        public string RegistrationNumber { get; set; }
        public decimal? YearlyTurnover2 { get; set; }
        public decimal? YearlyTurnover3 { get; set; }
        public string Year1 { get; set; }
        public string Year2 { get; set; }
        public string Year3 { get; set; }
        public string UANNumber { get; set; }
        public string PFNumber { get; set; }
        public string NumberofEmployee { get; set; }
        public string IEC { get; set; }
        public string ESIC { get; set; }
        public string VendorStatus { get; set; }
        public string VendorCurrency { get; set; }
        public string CompanySizeID { get; set; }
        public string VendorNameAndCompanyName { get; set; }

        public decimal PO_AcceptQty { get; set; }
        public decimal PO_RejectQty { get; set; }
        public decimal PO_ShortFall { get; set; }
        public string PO_Status_Title { get; set; }

    }

    public class ItemStoreData
    {
        public int IS_ItemStoreId { get; set; }
        public int IS_ITId { get; set; }
        public string IS_RackSerialNo { get; set; }
        public decimal IS_StoreQty { get; set; }
        public string IS_GRNNUMBER { get; set; }
    }

    public class PoExcelUploadMasterData
    {
        public int PO_Id { get; set; }
        public string PO_Number { get; set; }
        public string PO_ItemCode { get; set; }
        public string PO_ItemName { get; set; }
        public decimal PO_OrderedQty { get; set; }
        public decimal PO_ReceivedQty { get; set; }
        public decimal PO_GettingStock { get; set; }
        public decimal PO_AcceptQty { get; set; }
        public decimal PO_RejectQty { get; set; }
        public decimal PO_BalanceQty { get; set; }
        public decimal PO_BasicPrice { get; set; }
        public decimal PO_GSTPercentage { get; set; }
        public decimal PO_ShortFall { get; set; }
        public string PO_VendorCode { get; set; }
        public string PO_OrderedDateTime { get; set; }
        public string PO_DeliveryDateTime { get; set; }
        public string PO_GeneratedDateTime { get; set; }
        public string PO_CreatedDateTime { get; set; }
        public string PO_UpdatedDateTime { get; set; }
        public int PO_StatusId { get; set; }
        public int PO_CompanyUnitId { get; set; }
        public int PO_CreatedEmployeeId { get; set; }
        public int PO_MasterCompanyId { get; set; }
        public int PO_IsActive { get; set; }
        public int PO_IsNotificationSend { get; set; }
        public int PO_StatusForIT { get; set; }
        public decimal PO_ReturnQty { get; set; }

        public string PO_Status_Title { get; set; }

        public string VendorName { get; set; }
        public string VendorCompanyName { get; set; }


        public int ItemId { get; set; }


        // Newly added columns
        public string PO_ProjectID { get; set; }
        public string PO_ApprovalStatus { get; set; }
        public string PO_VendorName { get; set; }
        public string PO_Unit { get; set; }
        public string PO_ProductType { get; set; }
        public string PO_DeliverRemainder { get; set; }
        public decimal PO_UnitPrice { get; set; }
        public decimal PO_NetAmount { get; set; }
        public string PO_Site { get; set; }
        public string PO_Warehouse { get; set; }
        public string PO_Location { get; set; }
        public string PO_PurchaseRequisition { get; set; }
        public string PO_PurchaseLineCreatedDateTime { get; set; }
        public string PO_ProjectCategory { get; set; }
        public string PO_Currency { get; set; }
        public string PO_TermsOfPayment { get; set; }

    }

    public class NotificationModelData
    {
        public int NotificationId { get; set; }
        public int SenderMasterCompanyId { get; set; }
        public int SenderEmployeeId { get; set; }
        public int SenderRoleId { get; set; }
        public int SenderCompanyUnitId { get; set; }
        public string SenderEmailId { get; set; }
        public string SenderDepartment { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public int IsReadData { get; set; }
        public int ReceiverEmployeeId { get; set; }
        public string SenderCreatedDateTime { get; set; }
        public string SenderUpdatedDateTime { get; set; }
        public string ReceiverFirstName { get; set; }
        public string ReceiverLastName { get; set; }
        public string SenderFirstName { get; set; }
        public string SenderLastName { get; set; }
        public string SenderProfilePicture { get; set; }
    }

    public class EmailNotificationData
    {
        public int EN_ID { get; set; }
        public int EN_EmployeeId { get; set; }
        public int EN_CCEmployeeId { get; set; }
        public int EN_BCCEmployeeId { get; set; }
        public int EN_EmployeeRoleId { get; set; }
        public int EN_CCEmployeeRoleId { get; set; }
        public int EN_BCCEmployeeRoleId { get; set; }

        //public string EN_FromEmail { get; set; }
        public string EN_ToEmail { get; set; }
        public string EN_Subject { get; set; }
        //public string EN_Body { get; set; }
        public string EN_CcEmail { get; set; }
        public string EN_BccEmail { get; set; }
        public int EN_ENPID { get; set; }
        public int EN_IsEnableEmail { get; set; }
        public int EN_IsEnableNotification { get; set; }
        public int EN_MasterCompanyId { get; set; }
        public int EN_CompanyUnitId { get; set; }
        public int EN_CreatedEmployeeId { get; set; }
        //public int EN_CreatedEmployeeRoleId { get; set; }
        public string EN_CreatedDateTime { get; set; }
        public string EN_UpdatedDateTime { get; set; }

        public int EmployeeId { get; set; }
        public string EmailId { get; set; }
        public string Password { get; set; }
        public string SecurityID { get; set; }
        public string RoleName { get; set; }
        public int RoleId { get; set; }
        public int MasterCompanyId { get; set; }
        public int CreatedEmployeeId { get; set; }
        public int HODEmployeeRoleId { get; set; }
        public int HODEmployeeId { get; set; }
        public int IsLicenseExpired { get; set; }
        public int IsUserDeleted { get; set; }

        public int UnitId { get; set; }
        public int AccountStatus { get; set; }
        public string CurrentActiveMode { get; set; }
        public string LastLoginDateTime { get; set; }
        public string CreatedDateTime { get; set; }
        public string UpdatedDateTime { get; set; }

        public int EDId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ProfilePicture { get; set; }
        public string CompanyName { get; set; }

        public string PINCode { get; set; }
        public string Country { get; set; }
        public string State { get; set; }

        public string UnitName { get; set; }
        public string BranchName { get; set; }



        public int MailId { get; set; }
        public string M_FromAddress { get; set; }
        public string M_ToAddress { get; set; }
        public string M_CCAddress { get; set; }
        public string M_BCCAddress { get; set; }
        public string M_Subject { get; set; }
        public string M_Body { get; set; }
        public string M_Purpose { get; set; }
        public string M_FileName { get; set; }
        public string M_FileType { get; set; }
        public string M_EmailStatus { get; set; }
        public int M_MasterCompanyId { get; set; }
        public int M_CompanyUnitId { get; set; }
        public int M_CreatedEmployeeId { get; set; }
        public int M_CreatedEmployeeRoleId { get; set; }
        public int M_MailCounter { get; set; }
        public int M_TotalEmailSent { get; set; }
        public int M_IsSend { get; set; }
        public int M_IsRead { get; set; }
        public string M_CreatedDateTime { get; set; }
        public string M_UpdatedDateTime { get; set; }



        public int ENP_ID { get; set; }
        public string ENP_Name { get; set; }
        public int ENP_MasterCompanyId { get; set; }
        public int ENP_CreatedEmployeeId { get; set; }
        public string ENP_CreatedDateTime { get; set; }
        public string ENP_UpdatedDateTime { get; set; }

    }

    public class NotificationSettingModelData
    {
        public int NT_ID { get; set; }
        public string NT_Name { get; set; }
        public int NT_MasterCompanyId { get; set; }
        public int NT_CompanyUnitId { get; set; }
        public int NT_CreatedEmployeeId { get; set; }
        public int NT_IsEnabled { get; set; }
        public int NT_IsVisibled { get; set; }
        public string NT_CreatedDateTime { get; set; }
        public string NT_UpdatedDateTime { get; set; }


        public int N_ID { get; set; }
        public string N_NTID { get; set; }
        public string N_Subject { get; set; }
        public int N_MasterCompanyId { get; set; }
        public int N_CompanyUnitId { get; set; }
        public int N_CreatedEmployeeId { get; set; }
        public string N_CreatedDateTime { get; set; }
        public string N_UpdatedDateTime { get; set; }
    }

    public class NotificationSettingSaveModelData
    {
        public int NS_ID { get; set; }
        public int NS_EmployeeId { get; set; }
        public int NS_MasterCompanyId { get; set; }
        public int NS_CompanyUnitId { get; set; }
        public string NS_SubjectName { get; set; }
        public string NS_TypeName { get; set; }
        public string NS_IsChecke { get; set; }
        public string NS_IsEnable { get; set; }
        public string NS_IsVisible { get; set; }
        public string NS_CreatedDateTime { get; set; }
        public string NS_UpdatedDateTime { get; set; }

        public string EmailId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ProfilePicture { get; set; }
        public string PINCode { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
    }

    public class NotificationAssignMasterModelData
    {
        public int NA_ID { get; set; }
        public int NA_NID { get; set; }
        public string NA_SubjectName { get; set; }
        public int NA_ToEmployeeRoleId { get; set; }
        public int NA_ToEmployeeId { get; set; }
        public int NA_CCEmployeeRoleId { get; set; }
        public int NA_CCEmployeeId { get; set; }
        public int NA_BCCEmployeeRoleId { get; set; }
        public int NA_BCCEmployeeId { get; set; }
        public int NA_MasterCompanyId { get; set; }
        public int NA_CompanyUnitId { get; set; }
        public int NA_CreatedEmployeeId { get; set; }
        public string NA_CreatedDateTime { get; set; }
        public string NA_UpdatedDateTime { get; set; }



        public string ToEmployeeEmail { get; set; }
        public string ToEmployeeFirstName { get; set; }
        public string ToEmployeeLastName { get; set; }
        public string ToEmployeeMobile { get; set; }

        public string CCEmployeeEmail { get; set; }
        public string CCEmployeeFirstName { get; set; }
        public string CCEmployeeLastName { get; set; }
        public string CCEmployeeMobile { get; set; }

        public string BCCEmployeeEmail { get; set; }
        public string BCCEmployeeFirstName { get; set; }
        public string BCCEmployeeLastName { get; set; }
        public string BCCEmployeeMobile { get; set; }

        public string UnitName { get; set; }
    }



    public class NotificationDataForEmpAndHOD
    {
        public int EmployeeId { get; set; }
        public int RoleIdOfEmployee { get; set; }
        public string FirstNameOfEmployee { get; set; }
        public string LastNameOfEmployee { get; set; }
        public string RoleNameOfEmployee { get; set; }
        public string EmailIdOfEmployee { get; set; }
        public int EmployeeIdOfHOD { get; set; }
        public int RoleIdOfHOD { get; set; }
        public string FirstNameOfHOD { get; set; }
        public string LastNameOfHOD { get; set; }
        public string RoleNameOfHOD { get; set; }
        public string EmailIdOfHOD { get; set; }
    }



    public class MaterialRequestData
    {
        public int MRM_ID { get; set; }
        public int MRM_MasterCompanyId { get; set; }
        public int MRM_EmployeeID { get; set; }
        public int MRM_CompanyUnitId { get; set; }
        public int MRM_StatusId { get; set; }
        public int MRM_IsActive { get; set; }
        public int MRM_CounterYear { get; set; }
        public int MRM_CounterNumber { get; set; }
        public string MRM_NUMBER { get; set; }
        public int MRM_HODEmployeeID { get; set; }
        public int MRM_ApprovedEmployeeID { get; set; }
        public string MRM_ApprovedDateTime { get; set; }
        public string MRM_CreatedDateTime { get; set; }
        public string MRM_UpdatedDateTime { get; set; }
        public string MRM_ProjectCode { get; set; }

        public int MR_ID { get; set; }
        public int MR_StatusId { get; set; }
        public int MR_MRMID { get; set; }
        public string MR_NUMBER { get; set; }
        public string MR_ItemCode { get; set; }
        public string MR_ItemName { get; set; }
        public string MR_UOM { get; set; }
        public decimal MR_CurrentStock { get; set; }
        public decimal MR_MinStock { get; set; }
        public decimal MR_EmployeeRequireItemQty { get; set; }
        public decimal MR_ReceiveQty { get; set; }
        public decimal MR_BalanceQty { get; set; }

        public string MR_RequireDate { get; set; }


        public int MT_ID { get; set; }
        public int MT_MRID { get; set; }
        public int MT_MRMID { get; set; }
        public string MT_ItemCode { get; set; }
        public decimal MT_RequiredQty { get; set; }
        public decimal MT_ReceivedQty { get; set; }
        public decimal MT_BalanceQty { get; set; }
        public decimal MT_PreviousReceivedQty { get; set; }
        public decimal MT_PreviousBalanceQty { get; set; }
        public int MT_StatusId { get; set; }
        public string MT_CreatedDateTime { get; set; }
        public string MT_UpdateDateTime { get; set; }
        public int MT_MasterCompanyId { get; set; }
        public int MT_CompanyUnitId { get; set; }
        public int MT_EmployeeId { get; set; }


        public int MTS_ID { get; set; }
        public int MTS_MTID { get; set; }
        public decimal MTS_ItemStock { get; set; }
        public string MTS_BatchNo { get; set; }
        public string MTS_ExpiryDate { get; set; }
        public string MTS_CreatedDateTime { get; set; }
        public string MTS_UpdatedDateTime { get; set; }


        public string CompanyUnitName { get; set; }
        public string ApprovalFirstName { get; set; }
        public string ApprovalLastName { get; set; }
        public string StatusTitle { get; set; }



        public decimal TotalReleasedQty { get; set; }
        public string ItemName { get; set; }



        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class MasterTransactionData
    {
        public int MT_ID { get; set; }
        public int MT_MRID { get; set; }
        public int MT_MRMID { get; set; }
        public string MT_ItemCode { get; set; }
        public decimal MT_RequiredQty { get; set; }
        public decimal MT_ReceivedQty { get; set; }
        public decimal MT_BalanceQty { get; set; }
        public decimal MT_PreviousReceivedQty { get; set; }
        public decimal MT_PreviousBalanceQty { get; set; }
        public int MT_StatusId { get; set; }
        public string MT_CreatedDateTime { get; set; }
        public string MT_UpdateDateTime { get; set; }
        public int MT_MasterCompanyId { get; set; }
        public int MT_CompanyUnitId { get; set; }
        public int MT_EmployeeId { get; set; }


        public int MTS_ID { get; set; }
        public int MTS_MTID { get; set; }
        public decimal MTS_ItemStock { get; set; }
        public string MTS_BatchNo { get; set; }
        public string MTS_ExpiryDate { get; set; }
        public string MTS_CreatedDateTime { get; set; }
        public string MTS_UpdatedDateTime { get; set; }
    }

    public class MaterialTransationSubData
    {
        public int MTS_ID { get; set; }
        public int MTS_MTID { get; set; }
        public decimal MTS_ItemStock { get; set; }
        public string MTS_BatchNo { get; set; }
        public string MTS_ExpiryDate { get; set; }
        public string MTS_CreatedDateTime { get; set; }
        public string MTS_UpdatedDateTime { get; set; }

    }

    public class WarehouseStockDetailsData
    {
        public int SC_ID { get; set; }
        public string SC_ItemCode { get; set; }
        public decimal SC_TotalReceivedQty { get; set; }
        public decimal SC_TotalReleasedQty { get; set; }
        public decimal SC_BalanceQty { get; set; }
        public decimal SC_PreTotalReceivedQty { get; set; }
        public decimal SC_PreTotalReleasedQty { get; set; }
        public decimal SC_PreBalanceQty { get; set; }
        public int SC_ITID { get; set; }
        public int SC_MTID { get; set; }
        public int SC_MasterCompanyId { get; set; }
        public int SC_CompanyUnitId { get; set; }
        public int SC_EmployeeId { get; set; }
        public string SC_EntryDate { get; set; }
        public string SC_UOM { get; set; }
        public string SC_CreatedDateTime { get; set; }
        public string SC_UpdatedDateTime { get; set; }

        public decimal OpeningStock { get; set; }
        public decimal ClosingStock { get; set; }

        public int Years { get; set; }
        public int Months { get; set; }

    }

    public class ProjectMasterData
    {
        public int ProjectID { get; set; }
        public int MasterCompanyId { get; set; }
        public int CreatedEmployeeId { get; set; }
        public string ProjectName { get; set; }
        public string Status { get; set; }
        public decimal NDTPercent { get; set; }
        public DateTime StartDate { get; set; }
        public string FormattedStartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string FormattedEndDate { get; set; }
        public DateTime NewEndDate { get; set; }
        public string FormattedNewEndDate { get; set; }
        public string ProjectNumber { get; set; }
        public string CreatedDateTime { get; set; }
        public string UpdateDateTime { get; set; }
    }

    public class ProjectMaterialReturnDetail
    {
        public int PMR_ID { get; set; }
        public int PMR_ProjectId { get; set; }
        public int PMR_RackSerialId { get; set; }
        public string PMR_ProjectCode { get; set; }
        public string PMR_ItemCode { get; set; }
        public string PMR_UOM { get; set; }
        public string PMR_RackType { get; set; }
        public string PMR_RackSerialNumber { get; set; }
        public decimal PMR_ReturnStock { get; set; }
        public string PMR_BatchNumber { get; set; }
        public string PMR_ExpiryDate { get; set; }
        public string PMR_CreatedDateTime { get; set; }
        public string PMR_UpdatedDateTime { get; set; }
        public int PMR_MasterCompanyId { get; set; }
        public int PMR_CompanyUnitId { get; set; }
        public int PMR_CreatedEmployeeId { get; set; }
        public int PMR_IsDelete { get; set; }

        public int PMR_InventoryId { get; set; }
        public int PMR_ItemStockId { get; set; }
        public int PMR_wsdID { get; set; }

    }

    public class PoMasterData
    {
        public int POMasterID { get; set; }
        public string CreatedDate { get; set; }
        public string ReportStatus { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string ReportFileName { get; set; }
        public int Revision { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public int UserID { get; set; }
        public int StoreKeeperID { get; set; }
        public int QualityID { get; set; }
        public string ProjectID { get; set; }

        public int StatusID { get; set; }
        public string StatusTitle { get; set; }

        public string IGIR_Number { get; set; }
        public int IGIR_Series { get; set; }
    }

    public class IGIRData
    {
        public int IGIRID { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string ProjectID { get; set; }
        public string VendorCode { get; set; }
        public string ApprovalStatus { get; set; }
        public string ItemNumber { get; set; }
        public string DeliveryDate { get; set; }
        public string VendorName { get; set; }
        public string ProductDescription { get; set; }
        public string Unit { get; set; }
        public string ProductType { get; set; }
        public decimal? OrderedQuantity { get; set; }
        public decimal? ReceivedQuantity { get; set; }
        public string DeliverRemainder { get; set; }
        public decimal? UnitPrice { get; set; }
        public string Site { get; set; }
        public string Warehouse { get; set; }
        public string Location { get; set; }
        public string PurchaseRequisition { get; set; }
        public string PurchaseLineDateTime { get; set; }
        public string ProjectCategory { get; set; }
        public string Currency { get; set; }
        public string TermsOfPayment { get; set; }
        public string CMTR { get; set; }
        public string TDCNo { get; set; }
        public string MaterialReceived { get; set; }
        public string StockMaterial { get; set; }
        public string CertificateReceived { get; set; }
        public string ItemRemark { get; set; }
        public string IGIRRemark { get; set; }
        public string Marking { get; set; }
        public decimal? NetAmount { get; set; }
        public string Created { get; set; }
        public int? POMasterID { get; set; }
        public string Project { get; set; }
        public string Status { get; set; }
        public string IGIRSeries { get; set; }
        public string VendorReferenceDO { get; set; }
        public string MaterialOrigin { get; set; }
        public string GeneralRemark { get; set; }
        public string IGIRRemarkQuality { get; set; }
        public decimal? PastReceivedQuantity { get; set; }
        public string IGIR_Number { get; set; }
        public int IGIR_Series { get; set; }


        public string CreatedDate { get; set; }
        public string ReportStatus { get; set; }
        public string ReportFileName { get; set; }
        public int? Revision { get; set; }
        public string Type { get; set; }
        public int UserID { get; set; }
        public int? StoreKeeperID { get; set; }
        public int? QualityID { get; set; }


        public int InventoryId { get; set; }

        public int InventoryCompanyUnitId { get; set; }
        public int InventoryRackTypeId { get; set; }
        public int InventoryItemCategoryId { get; set; }
        public int InventoryItemId { get; set; }
        public int InventoryUOMId { get; set; }
        public int InventoryGSTId { get; set; }
        public decimal InventoryCurrentStock { get; set; }
        public decimal InventoryMinStock { get; set; }
        public int InventoryCreatedEmployeeId { get; set; }
        public int InventoryMasterCompanyId { get; set; }
        public string InventoryItemLocationStore { get; set; }
        public string InventoryCreatedDateTime { get; set; }
        public string InventoryUpdatedDateTime { get; set; }

        public int VendorId { get; set; }
        public string VendorEmail { get; set; }
        public string VendorMobileNumber { get; set; }
        public string VendorCompanyName { get; set; }
        public string VendorCompanyAddress { get; set; }
        public string VendorCompanyGSTNumber { get; set; }
        public string VendorCompanyPANNumber { get; set; }
        public string VendorCountry { get; set; }
        public string VendorState { get; set; }
        public string VendorCity { get; set; }
        public string VendorPinCode { get; set; }
        public string VendorBusinessType { get; set; }
        public string VendorCreatedDateTime { get; set; }
        public string VendorUpdatedDateTime { get; set; }
        public int VendorIsActive { get; set; }
        public int VendorMasterCompanyId { get; set; }
        public int VendorCreatedEmployeeId { get; set; }

        public string VendorNameAndCompanyName { get; set; }

        public int RackTypeId { get; set; }
        public string RackType { get; set; }
        public int RackMasterCompanyId { get; set; }
        public int RackCreatedEmployeeId { get; set; }
        public string RackCreatedDateTime { get; set; }
        public string RackUpdatedDateTime { get; set; }

        public int ItemId { get; set; }
        public int ItemCategoryId { get; set; }
        public int ItemUOMId { get; set; }
        public int ItemGSTId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public string ItemHSNCode { get; set; }

        public int ItemCreatedEmployeeId { get; set; }
        public int ItemMasterCompanyId { get; set; }
        public string ItemCreatedDateTime { get; set; }
        public string ItemUpdatedDateTime { get; set; }
        public string ItemImageName { get; set; }

        public int UOMId { get; set; }
        public string UOMCode { get; set; }
        public string UOMName { get; set; }

        public int VATID { get; set; }
        public string VATCode { get; set; }
        public decimal Percentage { get; set; }

        public string UnitName { get; set; }
    }

    public class ReturnMaterialMaster
    {
        public int RM_ID { get; set; }
        public int RM_ItemId { get; set; }
        public string RM_ProjectCode { get; set; }
        public string RM_ItemCode { get; set; }
        public string RM_PoNumber { get; set; }
        public decimal RM_PoOrderedQty { get; set; }
        public decimal RM_PoReceivedQty { get; set; }
        public decimal RM_PoGettingStock { get; set; }
        public decimal RM_PoAcceptQty { get; set; }
        public decimal RM_PoRejectQty { get; set; }
        public decimal RM_PoShortFall { get; set; }
        public decimal RM_PoBalanceQty { get; set; }
        public decimal RM_PoReturnQty { get; set; }
        public decimal RM_PreviousReturnQty { get; set; }

        public decimal RM_TotalReturnQty { get; set; }
        public decimal RM_PoBasicPrice { get; set; }
        public decimal RM_PoGSTPercentage { get; set; }
        public string RM_PoVendorCode { get; set; }
        public string RM_CreatedDateTime { get; set; }
        public string RM_UpdatedDateTime { get; set; }
        public int RM_CreatedEmployeeId { get; set; }
        public int RM_MasterCompanyId { get; set; }
        public int RM_CompanyUnitId { get; set; }
        public int RM_StatusId { get; set; }
        public int RM_IsActive { get; set; }
    }

    public class MaterialTransactionData
    {
        public string MT_ProjectCode { get; set; }
        public string MT_ItemCode { get; set; }
        public decimal TotalReceivedQty { get; set; }
        public int MT_EmployeeId { get; set; }
        public int MT_CompanyUnitId { get; set; }
        public int MT_MasterCompanyId { get; set; }
    }

    public class ProjectMaterialReturnData
    {
        public int PMR_ProjectId { get; set; }
        public string PMR_ProjectCode { get; set; }
        public string PMR_ItemCode { get; set; }
        public string PMR_UOM { get; set; }
        public string PMR_RackType { get; set; }
        public decimal TotalReturnStock { get; set; }
        public int PMR_MasterCompanyId { get; set; }
        public int PMR_CompanyUnitId { get; set; }
        public int PMR_CreatedEmployeeId { get; set; }
    }

    public class ProjectItemSummaryData
    {
        public string ProjectCode { get; set; }
        public string ItemCode { get; set; }
        public decimal ReceivedQty { get; set; }
        public decimal ReturnQty { get; set; }
        public decimal UsedQty { get; set; }
        public int CompanyUnitId { get; set; }
        public int MasterCompanyId { get; set; }
    }


    public class ItemQuarantineDetail
    {
        public int QuarantineId { get; set; }
        public int QuarantineItemID { get; set; }
        public string QuarantineItemCode { get; set; }
        public string QuarantineItem { get; set; }
        public string QuarantineItemBatchNo { get; set; }
        public int UOM { get; set; }
        public string PoNumber { get; set; }
        public int VendoreId { get; set; }
        public string Vendor { get; set; }
        public decimal ReceivedQty { get; set; }
        public decimal AcceptQty { get; set; }
        public decimal RejectQty { get; set; }
        public int MasterCompanyId { get; set; }
        public int CompanyUnitId { get; set; }
        public int CreatedEmployeeId { get; set; }
        public string CreatedDateTime { get; set; }
        public string UpdatedDateTime { get; set; }


        public int InventoryId { get; set; }
        public int ItemTransactionId { get; set; }
        public int PoMasterID { get; set; }
        public string IGIRNumber { get; set; }

        public string ProjectCode { get; set; }
        public string UOMName { get; set; }


    }


    public class PurchaseGroupData
    {
        public int PurchaseGroupID { get; set; }
        public string PurchaseGroupName { get; set; }
        public int ItemTypeID { get; set; }
    }

    public class PurchaseSubGroupData
    {
        public int PurchaseSubGroupID { get; set; }
        public string PurchaseSubGroupName { get; set; }
        public int PurchaseGroupID { get; set; }
        public int ItemTypeID { get; set; }


    }


    public class VendorDetails
    {
        public int VendorID { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public int? DealerID { get; set; }
        public string Website { get; set; }
        public string VAT { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string YearOfEstablishment { get; set; }
        public string TypeOfBusiness { get; set; }
        public string CompanyTradeLicence { get; set; }
        public decimal? YearlyTurnover1 { get; set; }
        public decimal? RequiredCredit { get; set; }
        public decimal? ApprovedCredit { get; set; }
        public string PanNumber { get; set; }
        public string AdharNumber { get; set; }
        public int? EstablishmentID { get; set; }
        public string RegistrationNumber { get; set; }
        public decimal? YearlyTurnover2 { get; set; }
        public decimal? YearlyTurnover3 { get; set; }
        public string Year1 { get; set; }
        public string Year2 { get; set; }
        public string Year3 { get; set; }
        public string UANNumber { get; set; }
        public string PFNumber { get; set; }
        public string NumberofEmployee { get; set; }
        public string IEC { get; set; }
        public string ESIC { get; set; }
        public string VendorStatus { get; set; }
        public string VendorCurrency { get; set; }
        public string PurchaseSubGroupID { get; set; }
        public string CompanySizeID { get; set; }
        public string PurchaseGroupID { get; set; }

        public string VendorNameAndCompanyName { get; set; }
    }

    public class PurchaseOrder
    {
        public int? PO_StatusForIT { get; set; }
        public int PurchaseOrderID { get; set; }
        public string PONumber { get; set; }
        public DateTime? PODate { get; set; }
        public int? VendorID { get; set; }
        public string Unit { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string SpecialInstructions { get; set; }
        public string ReportFileName { get; set; }
        public decimal? GrandTotal { get; set; }
        public int? UnitID { get; set; }
        public string Delivery { get; set; }
        public string Transportation { get; set; }
        public string Payment { get; set; }
        public string Guarantee { get; set; }
        public string Remarks { get; set; }
        public int? ContactPersonID { get; set; }
        public int? PurchaseOrderStatus { get; set; }
        public string Reason { get; set; }
        public string POSeries { get; set; }
        public int? POyear { get; set; }
        public string StoreType { get; set; }
        public int? projectid { get; set; }
        public int? penaltyID { get; set; }

        public int PODetailsID { get; set; }

        public decimal? Discount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? Tax { get; set; }
        public decimal? TotalAmount { get; set; }

        public DateTime? DeliveryDate { get; set; }
        public int? VendorQuotationID { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Amount { get; set; }
        public int? ItemID { get; set; }
        public int? POStatusID { get; set; }
        public decimal? ReceivedQty { get; set; }
        public int? TransactionID { get; set; }
        public decimal? AllocatedStock { get; set; }
        public int? ReceivedBy { get; set; }
        public decimal? SecurityReceived { get; set; }
        public decimal? TaxAmount { get; set; }
        public int? VATID { get; set; }
        public int? VendorQuotationDetailsID { get; set; }
        public int? EnquiryDetailsID { get; set; }
        public string PurchaseComments { get; set; }



        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public int PurchaseSubGroupID { get; set; }
        public int LeadTime { get; set; }
        public int UOMID { get; set; }
        public decimal StockQuantity { get; set; }
        public decimal MinStock { get; set; }
        public decimal MaxStock { get; set; }
        public bool DisabledItem { get; set; }
        public string HSNCode { get; set; }

        public decimal VAT { get; set; }

        public decimal UOMValue { get; set; }
        public int ConversionUOMID { get; set; }
        public decimal ConversionUOMValue { get; set; }
        public string ModelNo { get; set; }

        public int ItemTypeID { get; set; }
        public string Description { get; set; }

        public string Specification { get; set; }
        public int PurchaseGroupID { get; set; }
        public int CreatedBy { get; set; }
        public int ConvertUOM { get; set; }

        public string ItemTypeName { get; set; }

        public int InventoryId { get; set; }

        public int InventoryCompanyUnitId { get; set; }
        public int InventoryRackTypeId { get; set; }
        public int InventoryItemCategoryId { get; set; }
        public int InventoryItemId { get; set; }
        public int InventoryUOMId { get; set; }
        public int InventoryGSTId { get; set; }
        public decimal InventoryCurrentStock { get; set; }
        public decimal InventoryMinStock { get; set; }
        public int InventoryCreatedEmployeeId { get; set; }
        public int InventoryMasterCompanyId { get; set; }
        public string InventoryItemLocationStore { get; set; }
        public string InventoryCreatedDateTime { get; set; }
        public string InventoryUpdatedDateTime { get; set; }

        public int VendorId { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string VendorEmail { get; set; }
        public string VendorMobileNumber { get; set; }
        public string VendorCompanyName { get; set; }
        public string VendorCompanyAddress { get; set; }
        public string VendorCompanyGSTNumber { get; set; }
        public string VendorCompanyPANNumber { get; set; }
        public string VendorCountry { get; set; }
        public string VendorState { get; set; }
        public string VendorCity { get; set; }
        public string VendorPinCode { get; set; }
        public string VendorBusinessType { get; set; }
        public string VendorCreatedDateTime { get; set; }
        public string VendorUpdatedDateTime { get; set; }
        public int VendorIsActive { get; set; }
        public int VendorMasterCompanyId { get; set; }
        public int VendorCreatedEmployeeId { get; set; }


        public int RackTypeId { get; set; }
        public string RackType { get; set; }
        public int RackMasterCompanyId { get; set; }
        public int RackCreatedEmployeeId { get; set; }
        public string RackCreatedDateTime { get; set; }
        public string RackUpdatedDateTime { get; set; }

        public int RackSerialId { get; set; }
        public int TotalSerialNumber { get; set; }
        public string RackAlphSeries { get; set; }
        public string RackSerialNumber { get; set; }
        public int SerialNumberId { get; set; }


        public int ItemId { get; set; }
        public int ItemCategoryId { get; set; }
        public int ItemUOMId { get; set; }
        public int ItemGSTId { get; set; }
        //public string ItemCode { get; set; }
        //public string ItemName { get; set; }
        //public string ItemDescription { get; set; }
        public string ItemHSNCode { get; set; }

        public int ItemCreatedEmployeeId { get; set; }
        public int ItemMasterCompanyId { get; set; }
        public string ItemCreatedDateTime { get; set; }
        public string ItemUpdatedDateTime { get; set; }
        public string ItemImageName { get; set; }



        public int UOMId { get; set; }
        public string UOMCode { get; set; }
        public string UOMName { get; set; }

        public string VATCode { get; set; }
        public decimal Percentage { get; set; }

        public string UnitName { get; set; }


        public int? DealerID { get; set; }
        public string Website { get; set; }
        public string YearOfEstablishment { get; set; }
        public string TypeOfBusiness { get; set; }
        public string CompanyTradeLicence { get; set; }
        public decimal? YearlyTurnover1 { get; set; }
        public decimal? RequiredCredit { get; set; }
        public decimal? ApprovedCredit { get; set; }
        public string PanNumber { get; set; }
        public string AdharNumber { get; set; }
        public int? EstablishmentID { get; set; }
        public string RegistrationNumber { get; set; }
        public decimal? YearlyTurnover2 { get; set; }
        public decimal? YearlyTurnover3 { get; set; }
        public string Year1 { get; set; }
        public string Year2 { get; set; }
        public string Year3 { get; set; }
        public string UANNumber { get; set; }
        public string PFNumber { get; set; }
        public string NumberofEmployee { get; set; }
        public string IEC { get; set; }
        public string ESIC { get; set; }
        public string VendorStatus { get; set; }
        public string VendorCurrency { get; set; }

        public string CompanySizeID { get; set; }

        public string VendorNameAndCompanyName { get; set; }


        public decimal PO_OrderedQty { get; set; }
        public decimal PO_ReceivedQty { get; set; }
        public decimal PO_GettingStock { get; set; }
        public decimal PO_AcceptQty { get; set; }
        public decimal PO_RejectQty { get; set; }
        public decimal PO_BalanceQty { get; set; }
        public decimal PO_BasicPrice { get; set; }
        public decimal PO_GSTPercentage { get; set; }
        public decimal PO_ShortFall { get; set; }
        public string PO_VendorCode { get; set; }
        public string PO_OrderedDateTime { get; set; }
        public string PO_DeliveryDateTime { get; set; }
        public string PO_GeneratedDateTime { get; set; }
        public string PO_CreatedDateTime { get; set; }
        public string PO_UpdatedDateTime { get; set; }
        public int PO_StatusId { get; set; }
        public int PO_CompanyUnitId { get; set; }
        public int PO_CreatedEmployeeId { get; set; }
        public int PO_MasterCompanyId { get; set; }
        public int PO_IsActive { get; set; }
        public int PO_IsNotificationSend { get; set; }

        public decimal PO_ReturnQty { get; set; }
        public string PO_Status_Title { get; set; }



        public int StatusID { get; set; }
        public string StatusTitle { get; set; }

    }


}