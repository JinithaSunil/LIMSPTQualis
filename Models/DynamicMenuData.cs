using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LIMS.Models
{
    public class DynamicMenuData
    {

    }

    public class MenuItem
    {
        public int MenuId { get; set; }
        public int ParentMenuId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string RoleId { get; set; }
        public string MenuIcon { get; set; }
        public int Position { get; set; }

        public int IconId { get; set; }
        public string IconName { get; set; }
        public string IconClassName { get; set; }
        public string CreatedDateTime { get; set; }
        public string UpdatedDateTime { get; set; }
        public bool OpenInNewTab { get; set; }
        public List<MenuItem> SubItems { get; set; }
    }

    public class RoleNameData
    {        
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }

    public class AdminUserLimitSetData
    {
        public int UserLimitSetId { get; set; }
        public int LimitMaxNumber { get; set; }
        public string UserLimitSetData { get; set; }
    }

    public class LicenseValidityMasterData
    {
        public int LicenseValidityId { get; set; }
        public string LicensePeriodData { get; set; }
        public int TotalMonths { get; set; }
    }

    public class ParentMenuName
    {
        public int MenuId { get; set; }
        public int ParentMenuId { get; set; }
        public string Title { get; set; }        
        public string Description { get; set; }
        public string Url { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string MenuIcon { get; set; }
        public int Position { get; set; }
        public int IsSubMenu { get; set; }
    }

    public class AllParentMenuName
    {
        public int MenuId { get; set; }
        public int ParentMenuId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string MenuIcon { get; set; }
        public int Position { get; set; }
        public int IsSubMenu { get; set; }

        public int IconId { get; set; }
        public string IconName { get; set; }
        public string IconClassName { get; set; }
        public string CreatedDateTime { get; set; }
        public string UpdatedDateTime { get; set; }

        public string AspxPageID { get; set;}
    }

    public class PageName
    {
        public int AspxPageID { get; set; }
        public string AspxPageTitle { get; set; }
        public string AspxPageName { get; set; }
        public string AccessRoleId { get; set; }
        public string CreatedDateTime { get; set; }
        public string UpdatedDateTime { get; set; }
    }

    public class IconNameData
    {
        public int IconId { get; set; }
        public string IconName { get; set; }
        public string IconClassName { get; set; }
        public string CreatedDateTime { get; set; }
        public string UpdatedDateTime { get; set; }
    }

    public class SMTPDataModel
    {
        public int SMTP_ID { get; set; }
        public int SMTP_PORT { get; set; }
        public string SMTP_NAME { get; set; }
        public string SMTP_TYPE { get; set; }
        public string SMTP_EMAIL { get; set; }
        public string SMTP_USERNAME { get; set; }
        public string SMTP_PASSWORD { get; set; }
        public string SMTP_URL { get; set; }
        public string SMTP_REMARKS { get; set; }
        public int SMTP_Active { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }

        public string WebsiteURL { get; set; }
        public IEnumerable<SMTPDataModel> SMTPDataList { get; set; }

    }

    public class EmailMasterModel
    {
        public int EmailTemplateId { get; set; }
        public string EmailType { get; set; }
        public string EmailSubject { get; set; }
        public string FromEmailAddress { get; set; }
        public string ToEmailAddress { get; set; }
        public string CCEmailAddress { get; set; }
        public string BCCEmailAddress { get; set; }
        public string Body { get; set; }
        public string FileName { get; set; }
        public string CompanyName { get; set; }
        public string FileType { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public int RemindersDays { get; set; }

        public IEnumerable<EmailMasterModel> EmailTemplateMasterModelList { get; set; }

    }

    public class UserLicensePeriodModel
    {
        public int LicensePeriodId { get; set; }
        public string LicensePeriodData { get; set; }
        public string CompanyName { get; set; }
        public string LicenseNumber { get; set; }
        public string LicenseNumberData { get; set; }
        public string LimitMaxNumber { get; set; }
        public string LicenseStartDate { get; set; }
        public string LicenseExpiredDate { get; set; }
        public string LicenseStatus { get; set; }
        public int MasterCompanyId { get; set; }

        public int TotalMonths { get; set; }
        public int MaxUsers { get; set; }
        public string LicenseName { get; set; }

        public string LicensePeriodInMonths { get; set; }
        public IEnumerable<UserLicensePeriodModel> UserLicensePeriodList { get; set; }
    }

    //public class CompanyBranchData
    //{
    //    public int BranchId { get; set; }
    //    public int MasterCompanyId { get; set; }
    //    public int CreatedEmployeeId { get; set; }
    //    public int IsActive { get; set; }
    //    public string BranchName { get; set; }
    //    public string GSTNo { get; set; }
    //    public string BranchAddress { get; set; }
    //    public string State { get; set; }
    //    public string City { get; set; }
    //    public string PINCode { get; set; }
    //    public string CreatedDateTime { get; set; }
    //    public string UpdatedDateTime { get; set; }
    //}

    public class CompanyUnitData
    {
        public int CompanyUnitId { get; set; }
        public string GSTNumberOfUnit { get; set; }
        public int MasterCompanyId { get; set; }
        public int CreatedEmployeeId { get; set; }
        public int IsActive { get; set; }
        public string UnitName { get; set; }
        public string UnitAddress { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string PinCode { get; set; }
        public string CreatedDateTime { get; set; }
        public string UpdatedDateTime { get; set; }
    }

}