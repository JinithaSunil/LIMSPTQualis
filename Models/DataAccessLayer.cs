using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using LIMS.Models;
using System.Security.Policy;
using System.Web.UI;
using System.IO;
using System.Globalization;
//using DocumentFormat.OpenXml.Drawing;
//using DocumentFormat.OpenXml.Bibliography;

namespace LIMS.Models
{
    public class DataAccessLayer
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ToString());
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        string date = DateTime.Today.ToString("dd-MMM-yyyy");
        string time = DateTime.Now.ToString("h:mm:ss tt");

        public bool HasAccessToUrl(string url, string roleId)
        {
            string query = "SELECT COUNT(*) FROM AspxPageMasterTB WHERE AspxPageName = @AspxPageName AND CHARINDEX(@AccessRoleId, AccessRoleId) > 0";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@AspxPageName", url);
                cmd.Parameters.AddWithValue("@AccessRoleId", roleId);
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                con.Close();
                return count > 0;
            }
        }

        public int GetReseedIdentity(string tableName, string identityField)
        {
            try
            {
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();

                con.Open();
                string sQuery = "select max(" + identityField + ") from " + tableName;

                SqlCommand cmd = new SqlCommand(sQuery, con);
                cmd.CommandType = CommandType.Text;
                int ReseedIdentity = (int)cmd.ExecuteScalar();

                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();

                return ReseedIdentity;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return 0;
            }
        }

        public void ReseedIdentity(string tableName, int reseedIdentity)
        {
            if (con.State == System.Data.ConnectionState.Open)
                con.Close();
            string sQuery = "DBCC CHECKIDENT('" + tableName + "', RESEED, " + reseedIdentity + ")";
            con.Open();
            SqlCommand cmd = new SqlCommand(sQuery, con);
            int modified = cmd.ExecuteNonQuery();
            if (con.State == System.Data.ConnectionState.Open)
                con.Close();
        }

        public List<RoleNameData> AllRoleNameDisplay()
        {
            List<RoleNameData> items = new List<RoleNameData>();

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT * FROM RoleMasterTB";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    //cmd.Parameters.AddWithValue("@IsActive", 1);
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new RoleNameData
                            {
                                RoleId = Convert.ToInt32(sdr["RoleId"]),
                                RoleName = sdr["RoleName"].ToString(),
                            });
                        }
                    }
                }
            }
            return items;
        }

        public List<RoleNameData> DisplayUserRole()
        {
            List<RoleNameData> items = new List<RoleNameData>();

            using (SqlConnection con = new SqlConnection(constr))
            {
                /* string query = "SELECT * FROM RoleMasterTB WHERE RoleId <> 1"; */
                string query = "SELECT * FROM RoleMasterTB WHERE RoleId <> 1 ORDER BY CONVERT(datetime,CreatedDateTime, 120) DESC";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new RoleNameData
                            {
                                RoleId = Convert.ToInt32(sdr["RoleId"]),
                                RoleName = sdr["RoleName"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<ParentMenuName> ParentMenuNameDisplay()
        {
            List<ParentMenuName> items = new List<ParentMenuName>();

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT * FROM Menus where ParentMenuId = @ParentMenuId";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ParentMenuId", 0);
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new ParentMenuName
                            {
                                MenuId = Convert.ToInt32(sdr["MenuId"]),
                                ParentMenuId = Convert.ToInt32(sdr["ParentMenuId"]),
                                Title = sdr["Title"].ToString(),
                                Description = sdr["Description"].ToString(),
                                Url = sdr["Url"].ToString(),
                                RoleId = sdr["RoleId"].ToString(),
                                MenuIcon = sdr["MenuIcon"].ToString(),
                                Position = Convert.ToInt32(sdr["Position"]),
                                IsSubMenu = Convert.ToInt32(sdr["IsSubMenu"]),
                            });
                        }
                    }
                }
            }
            return items;
        }

        public List<AllParentMenuName> AllParentMenuNameDisplay()
        {
            List<AllParentMenuName> items = new List<AllParentMenuName>();

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM Menus ms
                                  INNER JOIN IconTB itb ON itb.IconId = ms.MenuIcon 
                                  INNER JOIN AspxPageMasterTB apt ON apt.AspxPageName = ms.Url
                                  where ms.ParentMenuId = @ParentMenuId";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ParentMenuId", 0);
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new AllParentMenuName
                            {
                                MenuId = Convert.ToInt32(sdr["MenuId"]),
                                ParentMenuId = Convert.ToInt32(sdr["ParentMenuId"]),
                                Title = sdr["Title"].ToString(),
                                Description = sdr["Description"].ToString(),
                                AspxPageID = sdr["AspxPageID"].ToString(),
                                Url = sdr["Url"].ToString(),
                                RoleId = sdr["RoleId"].ToString(),
                                MenuIcon = sdr["MenuIcon"].ToString(),
                                Position = Convert.ToInt32(sdr["Position"]),
                                IsSubMenu = Convert.ToInt32(sdr["IsSubMenu"]),

                                IconId = Convert.ToInt32(sdr["IconId"]),
                                IconName = sdr["IconName"].ToString(),
                                IconClassName = sdr["IconClassName"].ToString(),
                            });
                        }
                    }
                }
            }
            return items;
        }

        public List<ParentMenuName> SubMenuNameDisplay()
        {
            List<ParentMenuName> items = new List<ParentMenuName>();

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT * FROM Menus where IsSubMenu = @IsSubMenu";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IsSubMenu", 1);
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new ParentMenuName
                            {
                                MenuId = Convert.ToInt32(sdr["MenuId"]),
                                ParentMenuId = Convert.ToInt32(sdr["ParentMenuId"]),
                                Title = sdr["Title"].ToString(),
                                Description = sdr["Description"].ToString(),
                                Url = sdr["Url"].ToString(),
                                RoleId = sdr["RoleId"].ToString(),
                                MenuIcon = sdr["MenuIcon"].ToString(),
                                Position = Convert.ToInt32(sdr["Position"]),
                                IsSubMenu = Convert.ToInt32(sdr["IsSubMenu"]),
                            });
                        }
                    }
                }
            }
            return items;
        }

        public List<AllParentMenuName> AllSubMenuNameDisplay()
        {
            List<AllParentMenuName> items = new List<AllParentMenuName>();

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM Menus ms 
                                  INNER JOIN IconTB itb ON itb.IconId = ms.MenuIcon 
                                  INNER JOIN AspxPageMasterTB apt ON apt.AspxPageName = ms.Url
                                  where ms.IsSubMenu = @IsSubMenu";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IsSubMenu", 1);
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new AllParentMenuName
                            {
                                MenuId = Convert.ToInt32(sdr["MenuId"]),
                                ParentMenuId = Convert.ToInt32(sdr["ParentMenuId"]),
                                Title = sdr["Title"].ToString(),
                                Description = sdr["Description"].ToString(),
                                AspxPageID = sdr["AspxPageID"].ToString(),
                                Url = sdr["Url"].ToString(),
                                RoleId = sdr["RoleId"].ToString(),
                                MenuIcon = sdr["MenuIcon"].ToString(),
                                Position = Convert.ToInt32(sdr["Position"]),
                                IsSubMenu = Convert.ToInt32(sdr["IsSubMenu"]),

                                IconId = Convert.ToInt32(sdr["IconId"]),
                                IconName = sdr["IconName"].ToString(),
                                IconClassName = sdr["IconClassName"].ToString(),
                            });
                        }
                    }
                }
            }
            return items;
        }

        public List<PageName> PageNameDisplay()
        {
            List<PageName> items = new List<PageName>();

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT * FROM AspxPageMasterTB";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new PageName
                            {
                                AspxPageID = Convert.ToInt32(sdr["AspxPageID"]),
                                AspxPageTitle = sdr["AspxPageTitle"].ToString(),
                                AspxPageName = sdr["AspxPageName"].ToString(),
                                AccessRoleId = sdr["AccessRoleId"].ToString(),
                                CreatedDateTime = sdr["CreatedDateTime"].ToString(),
                                UpdatedDateTime = sdr["UpdatedDateTime"].ToString(),
                            });
                        }
                    }
                }
            }
            return items;
        }

        public List<IconNameData> IconDisplay()
        {
            List<IconNameData> items = new List<IconNameData>();

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT * FROM IconTB";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new IconNameData
                            {
                                IconId = Convert.ToInt32(sdr["IconId"]),
                                IconName = sdr["IconName"].ToString(),
                                IconClassName = sdr["IconClassName"].ToString(),
                                CreatedDateTime = sdr["CreatedDateTime"].ToString(),
                                UpdatedDateTime = sdr["UpdatedDateTime"].ToString(),
                            });
                        }
                    }
                }
            }
            return items;
        }

        public List<AdminUserLimitSetData> GetAdminUserLimitSetData()
        {
            List<AdminUserLimitSetData> items = new List<AdminUserLimitSetData>();

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT * FROM AdminUserLimitSet";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new AdminUserLimitSetData
                            {
                                UserLimitSetId = Convert.ToInt32(sdr["UserLimitSetId"]),
                                UserLimitSetData = sdr["UserLimitSetData"].ToString(),
                                LimitMaxNumber = Convert.ToInt32(sdr["LimitMaxNumber"]),
                            });
                        }
                    }
                }
            }
            return items;
        }

        public List<LicenseValidityMasterData> GetLicenseValidityMaster()
        {
            List<LicenseValidityMasterData> items = new List<LicenseValidityMasterData>();

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT * FROM LicenseValidityMaster";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new LicenseValidityMasterData
                            {
                                LicenseValidityId = Convert.IsDBNull(sdr["LicenseValidityId"]) ? 0 : Convert.ToInt32(sdr["LicenseValidityId"]),
                                LicensePeriodData = Convert.IsDBNull(sdr["LicensePeriodData"]) ? string.Empty : sdr["LicensePeriodData"].ToString(),
                                TotalMonths = Convert.IsDBNull(sdr["TotalMonths"]) ? 0 : Convert.ToInt32(sdr["TotalMonths"]),

                            });
                        }
                    }
                }
            }
            return items;
        }

        public List<SMTPDataModel> SMTPDataListDisplay()
        {
            List<SMTPDataModel> items = new List<SMTPDataModel>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT * from SMTP_Master WHERE Active = @Active";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@Active", 1);
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new SMTPDataModel
                            {
                                SMTP_ID = Convert.ToInt32(sdr["SMTP_ID"]),
                                SMTP_PORT = Convert.ToInt32(sdr["SMTP_PORT"]),
                                SMTP_NAME = sdr["SMTP_NAME"].ToString(),
                                SMTP_EMAIL = sdr["SMTP_EMAIL"].ToString(),
                                SMTP_PASSWORD = sdr["SMTP_PASSWORD"].ToString(),
                                SMTP_TYPE = sdr["SMTP_TYPE"].ToString(),
                                SMTP_USERNAME = sdr["SMTP_USERNAME"].ToString(),
                                SMTP_REMARKS = sdr["SMTP_REMARKS"].ToString(),
                                SMTP_URL = sdr["SMTP_URL"].ToString(),
                                WebsiteURL = sdr["WebsiteURL"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            if (items.Count == 0)
            {
                return null;
            }
            return items;
        }

        public List<SMTPDataModel> SMTPAllDataListDisplay()
        {
            List<SMTPDataModel> items = new List<SMTPDataModel>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT * from SMTP_Master";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@Active", 1);
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new SMTPDataModel
                            {
                                SMTP_ID = Convert.ToInt32(sdr["SMTP_ID"]),
                                SMTP_PORT = Convert.ToInt32(sdr["SMTP_PORT"]),
                                SMTP_NAME = sdr["SMTP_NAME"].ToString(),
                                SMTP_EMAIL = sdr["SMTP_EMAIL"].ToString(),
                                SMTP_PASSWORD = sdr["SMTP_PASSWORD"].ToString(),
                                SMTP_TYPE = sdr["SMTP_TYPE"].ToString(),
                                SMTP_USERNAME = sdr["SMTP_USERNAME"].ToString(),
                                SMTP_REMARKS = sdr["SMTP_REMARKS"].ToString(),
                                SMTP_URL = sdr["SMTP_URL"].ToString(),
                                SMTP_Active = Convert.ToInt32(sdr["Active"].ToString()),
                                WebsiteURL = sdr["WebsiteURL"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            if (items.Count == 0)
            {
                return null;
            }
            return items;
        }

        public List<EmailMasterModel> EmailTemplateDataDisplay()
        {
            List<EmailMasterModel> items = new List<EmailMasterModel>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT * from EmailTemplateMaster";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new EmailMasterModel
                            {
                                EmailTemplateId = Convert.IsDBNull(sdr["EmailTemplateId"]) ? 0 : Convert.ToInt32(sdr["EmailTemplateId"]),
                                EmailType = sdr["EmailType"].ToString(),
                                EmailSubject = sdr["EmailSubject"].ToString(),
                                FromEmailAddress = sdr["FromEmailAddress"].ToString(),
                                ToEmailAddress = sdr["ToEmailAddress"].ToString(),
                                CCEmailAddress = sdr["CCEmailAddress"].ToString(),
                                BCCEmailAddress = sdr["BCCEmailAddress"].ToString(),
                                Body = sdr["Body"].ToString(),
                                FileName = sdr["FileName"].ToString(),
                                FileType = sdr["FileType"].ToString(),
                                CompanyName = sdr["CompanyName"].ToString(),
                                CreatedDate = sdr["CreatedDate"].ToString(),
                                UpdatedDate = sdr["UpdatedDate"].ToString(),
                                RemindersDays = Convert.IsDBNull(sdr["RemindersDays"]) ? 0 : Convert.ToInt32(sdr["RemindersDays"]),
                            });
                        }
                    }
                    con.Close();
                }
            }
            if (items.Count == 0)
            {
                return null;
            }
            return items;
        }

        public List<EmailNotificationData> EmailNotificationPurposeData()
        {
            List<EmailNotificationData> items = new List<EmailNotificationData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT * from EmailNotificationPurpose WHERE ENP_MasterCompanyId = @ENP_MasterCompanyId";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Parameters.AddWithValue("@ENP_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new EmailNotificationData
                            {
                                ENP_ID = Convert.ToInt32(sdr["ENP_ID"]),
                                ENP_MasterCompanyId = Convert.ToInt32(sdr["ENP_MasterCompanyId"]),
                                ENP_CreatedEmployeeId = Convert.ToInt32(sdr["ENP_CreatedEmployeeId"]),
                                ENP_Name = sdr["ENP_Name"].ToString(),
                                ENP_CreatedDateTime = sdr["ENP_CreatedDateTime"].ToString(),
                                ENP_UpdatedDateTime = sdr["ENP_UpdatedDateTime"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            if (items.Count == 0)
            {
                return null;
            }
            return items;
        }

        public List<CompanyUnitData> AllCompanyUnitDataGet()
        {
            List<CompanyUnitData> items = new List<CompanyUnitData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT * FROM CompanyUnitMaster ORDER BY CONVERT(datetime, CreatedDateTime, 120) DESC";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@IsActive", 1);
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new CompanyUnitData
                            {
                                CompanyUnitId = Convert.ToInt32(sdr["CompanyUnitId"]),
                                GSTNumberOfUnit = sdr["GSTNumberOfUnit"].ToString(),
                                MasterCompanyId = Convert.ToInt32(sdr["MasterCompanyId"]),
                                CreatedEmployeeId = Convert.ToInt32(sdr["CreatedEmployeeId"]),
                                IsActive = Convert.ToInt32(sdr["IsActive"]),
                                UnitName = sdr["UnitName"].ToString(),
                                UnitAddress = sdr["UnitAddress"].ToString(),
                                State = sdr["State"].ToString(),
                                City = sdr["City"].ToString(),
                                PinCode = sdr["PinCode"].ToString(),
                                CreatedDateTime = sdr["CreatedDateTime"].ToString(),
                                UpdatedDateTime = sdr["UpdatedDateTime"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<CompanyUnitData> ActiveCompanyUnitDataGet()
        {
            List<CompanyUnitData> items = new List<CompanyUnitData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT * FROM CompanyUnitMaster WHERE IsActive = @IsActive ORDER BY CONVERT(datetime, CreatedDateTime, 120) DESC";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@IsActive", 1);
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new CompanyUnitData
                            {
                                CompanyUnitId = Convert.ToInt32(sdr["CompanyUnitId"]),
                                GSTNumberOfUnit = sdr["GSTNumberOfUnit"].ToString(),
                                MasterCompanyId = Convert.ToInt32(sdr["MasterCompanyId"]),
                                CreatedEmployeeId = Convert.ToInt32(sdr["CreatedEmployeeId"]),
                                IsActive = Convert.ToInt32(sdr["IsActive"]),
                                UnitName = sdr["UnitName"].ToString(),
                                UnitAddress = sdr["UnitAddress"].ToString(),
                                State = sdr["State"].ToString(),
                                City = sdr["City"].ToString(),
                                PinCode = sdr["PinCode"].ToString(),
                                CreatedDateTime = sdr["CreatedDateTime"].ToString(),
                                UpdatedDateTime = sdr["UpdatedDateTime"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<AllEmployeeDetailsData> ActiveCompanyEmployeeDetailsDataGet()
        {
            List<AllEmployeeDetailsData> items = new List<AllEmployeeDetailsData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * from MasterEmployeeData med 
                                INNER JOIN MasterEmployeeDetails md ON md.EmployeeId = med.EmployeeId 
                                INNER JOIN MasterCompanyData mcd ON mcd.MasterCompanyId = med.MasterCompanyId 
                                INNER JOIN MasterCompanyDetails mcdd ON mcdd.MasterCompanyId = med.MasterCompanyId 
                                INNER JOIN RoleMasterTB rm ON rm.RoleId = med.RoleId
                                WHERE med.AccountStatus = @AccountStatus
                                ORDER BY CONVERT(datetime, med.CreatedDateTime, 120) DESC";

                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@AccountStatus", 1);
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new AllEmployeeDetailsData
                            {
                                EmployeeId = Convert.ToInt32(sdr["EmployeeId"]),
                                EmailId = sdr["EmailId"].ToString(),
                                Password = sdr["Password"].ToString(),
                                SecurityID = sdr["SecurityID"].ToString(),
                                RoleId = Convert.ToInt32(sdr["RoleId"]),
                                RoleName = sdr["RoleName"].ToString(),
                                MasterCompanyId = Convert.ToInt32(sdr["MasterCompanyId"]),
                                CreatedEmployeeId = Convert.ToInt32(sdr["CreatedEmployeeId"]),
                                HODEmployeeId = Convert.ToInt32(sdr["HODEmployeeId"]),
                                IsLicenseExpired = Convert.ToInt32(sdr["IsLicenseExpired"]),
                                IsUserDeleted = Convert.ToInt32(sdr["IsUserDeleted"]),
                                //BranchId = Convert.ToInt32(sdr["BranchId"]),
                                UnitId = Convert.ToInt32(sdr["UnitId"]),
                                AccountStatus = Convert.ToInt32(sdr["AccountStatus"]),
                                CurrentActiveMode = sdr["CurrentActiveMode"].ToString(),
                                LastLoginDateTime = sdr["LastLoginDateTime"].ToString(),
                                CreatedDateTime = sdr["CreatedDateTime"].ToString(),
                                UpdatedDateTime = sdr["UpdatedDateTime"].ToString(),
                                EDId = Convert.ToInt32(sdr["EDId"]),
                                FirstName = sdr["FirstName"].ToString(),
                                LastName = sdr["LastName"].ToString(),
                                MobileNumber = sdr["MobileNumber"].ToString(),
                                Gender = sdr["Gender"].ToString(),
                                Address = sdr["Address"].ToString(),
                                City = sdr["City"].ToString(),
                                State = sdr["State"].ToString(),
                                Country = sdr["Country"].ToString(),
                                PINCode = sdr["PINCode"].ToString(),
                                ProfilePicture = sdr["ProfilePicture"].ToString(),
                                CompanyName = sdr["CompanyName"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<AllEmployeeDetailsData> AllCompanyEmployeeDetailsDataGet()
        {
            List<AllEmployeeDetailsData> items = new List<AllEmployeeDetailsData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT *,
                                     med.EmployeeId AS EmpEmployeeId,med.MasterCompanyId AS EmpMasterCompanyId,
                                     med.EmailId AS EmpEmailId, med.Password AS EmpPassword, med.RoleId AS EmpRoleId,
                                     med.AccountStatus AS EmpAccountStatus,med.SecurityID AS EmpSecurityID,
                                     med.IsLicenseExpired AS EmpIsLicenseExpired,med.LastLoginDateTime AS EmpLastLoginDateTime,
                                     md.FirstName As EmpFirstName, md.LastName AS EmpLastName,
                                     md.MobileNumber AS EmpMobileNumber, md.Address AS EmpAddress,
                                     md.Country AS EmpCountry, md.State As EmpState,
                                     md.City AS EmpCity, md.PINCode AS EmpPINCode,md.ProfilePicture AS EmpProfilePicture,
                                     md.CreatedDateTime AS EmpCreatedDateTime, md.UpdatedDateTime AS EmpUpdatedDateTime 
                                 from MasterEmployeeData med 
                                 INNER JOIN MasterEmployeeDetails md ON md.EmployeeId = med.EmployeeId 
                                 INNER JOIN MasterCompanyData mcd ON mcd.MasterCompanyId = med.MasterCompanyId 
                                 INNER JOIN MasterCompanyDetails mcdd ON mcdd.MasterCompanyId = med.MasterCompanyId
                                 INNER JOIN CompanyUnitMaster cum ON cum.CompanyUnitId = med.UnitId 
                                 INNER JOIN RoleMasterTB rm ON rm.RoleId = med.RoleId 
                                 ORDER BY CONVERT(datetime, med.CreatedDateTime, 120) DESC";

                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    //cmd.Parameters.AddWithValue("@AccountStatus", 1);
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new AllEmployeeDetailsData
                            {
                                EmployeeId = Convert.ToInt32(sdr["EmpEmployeeId"]),
                                EmailId = sdr["EmpEmailId"].ToString(),
                                Password = sdr["EmpPassword"].ToString(),
                                SecurityID = sdr["EmpSecurityID"].ToString(),
                                RoleId = Convert.ToInt32(sdr["EmpRoleId"]),
                                RoleName = sdr["RoleName"].ToString(),
                                MasterCompanyId = Convert.ToInt32(sdr["EmpMasterCompanyId"]),
                                CreatedEmployeeId = Convert.ToInt32(sdr["CreatedEmployeeId"]),
                                HODEmployeeId = Convert.ToInt32(sdr["HODEmployeeId"]),
                                HODEmployeeRoleId = Convert.ToInt32(sdr["HODEmployeeRoleId"]),
                                IsLicenseExpired = Convert.ToInt32(sdr["EmpIsLicenseExpired"]),
                                IsUserDeleted = Convert.ToInt32(sdr["IsUserDeleted"]),
                                //BranchId = Convert.ToInt32(sdr["BranchId"]),
                                UnitId = Convert.ToInt32(sdr["UnitId"]),
                                AccountStatus = Convert.ToInt32(sdr["EmpAccountStatus"]),
                                CurrentActiveMode = sdr["CurrentActiveMode"].ToString(),
                                LastLoginDateTime = sdr["EmpLastLoginDateTime"].ToString(),
                                CreatedDateTime = sdr["EmpCreatedDateTime"].ToString(),
                                UpdatedDateTime = sdr["EmpUpdatedDateTime"].ToString(),
                                EDId = Convert.ToInt32(sdr["EDId"]),
                                FirstName = sdr["EmpFirstName"].ToString(),
                                LastName = sdr["EmpLastName"].ToString(),
                                MobileNumber = sdr["EmpMobileNumber"].ToString(),
                                Gender = sdr["Gender"].ToString(),
                                Address = sdr["EmpAddress"].ToString(),
                                City = sdr["EmpCity"].ToString(),
                                State = sdr["EmpState"].ToString(),
                                Country = sdr["EmpCountry"].ToString(),
                                PINCode = sdr["EmpPINCode"].ToString(),
                                ProfilePicture = sdr["EmpProfilePicture"].ToString(),
                                CompanyName = sdr["CompanyName"].ToString(),
                                UnitName = sdr["UnitName"].ToString(),
                                //BranchName = sdr["BranchName"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<AllMasterCompanyData> ActiveMasterCompanyDataGet()
        {
            List<AllMasterCompanyData> items = new List<AllMasterCompanyData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * from MasterCompanyData mcda 
                                 INNER JOIN MasterCompanyDetails mcde ON mcde.MasterCompanyId = mcda.MasterCompanyId
                                 INNER JOIN RoleMasterTB rm ON rm.RoleId = mcda.RoleId
                                 INNER JOIN AdminUserLimitSet aus ON aus.UserLimitSetId = mcda.NumberofUsersId
                                 INNER JOIN LicenseValidityMaster avm ON avm.LicenseValidityId = mcda.LicenseValidityId
                                 WHERE mcda.AccountStatus = @AccountStatus
                                 ORDER BY CONVERT(datetime, mcda.CreatedDateTime, 120) DESC";

                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@AccountStatus", 1);
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new AllMasterCompanyData
                            {
                                MasterCompanyId = Convert.ToInt32(sdr["MasterCompanyId"]),
                                EmailId = sdr["EmailId"].ToString(),
                                Password = sdr["Password"].ToString(),
                                SecurityId = sdr["SecurityId"].ToString(),
                                CompanyName = sdr["CompanyName"].ToString(),
                                RoleName = sdr["RoleName"].ToString(),
                                RoleId = Convert.ToInt32(sdr["RoleId"]),
                                AccountStatus = Convert.ToInt32(sdr["AccountStatus"]),
                                NumberofUsersId = Convert.ToInt32(sdr["NumberofUsersId"]),
                                LicenseCounter = Convert.ToInt32(sdr["LicenseCounter"]),
                                LicenseYear = Convert.ToInt32(sdr["LicenseYear"]),
                                LicenseNumber = sdr["LicenseNumber"].ToString(),
                                LicenseValidityId = Convert.ToInt32(sdr["LicenseValidityId"]),
                                LicenseStatusId = Convert.ToInt32(sdr["LicenseStatusId"]),
                                LicenseRenewRequest = Convert.ToInt32(sdr["LicenseRenewRequest"]),
                                LicenseStartDate = sdr["LicenseStartDate"].ToString(),
                                LicenseEndDate = sdr["LicenseEndDate"].ToString(),
                                IsLicenseExpired = Convert.ToInt32(sdr["IsLicenseExpired"]),
                                LastLoginDateTime = sdr["LastLoginDateTime"].ToString(),
                                CreatedDateTime = sdr["CreatedDateTime"].ToString(),
                                UpdatedDateTime = sdr["UpdatedDateTime"].ToString(),

                                // Properties from MasterCompanyDetails table
                                MasterCompanyDetailsId = Convert.ToInt32(sdr["MasterCompanyDetailsId"]),
                                FirstName = sdr["FirstName"].ToString(),
                                LastName = sdr["LastName"].ToString(),
                                MobileNumber = sdr["MobileNumber"].ToString(),
                                Address = sdr["Address"].ToString(),
                                City = sdr["City"].ToString(),
                                State = sdr["State"].ToString(),
                                Country = sdr["Country"].ToString(),
                                PINCode = sdr["PINCode"].ToString(),
                                PANCardNo = sdr["PANCardNo"].ToString(),
                                GSTNo = sdr["GSTNo"].ToString(),
                                StateGSTCode = sdr["StateGSTCode"].ToString(),
                                CompanyLogo = sdr["CompanyLogo"].ToString(),
                                MaxUserLimit = sdr["LimitMaxNumber"].ToString(),
                                UserLimitSetData = sdr["UserLimitSetData"].ToString(),
                                LicensePeriodData = sdr["LicensePeriodData"].ToString(),
                                TotalMonths = Convert.ToInt32(sdr["TotalMonths"]),
                            });
                        }

                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<AllMasterCompanyData> AllMasterCompanyDataGet()
        {
            List<AllMasterCompanyData> items = new List<AllMasterCompanyData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * from MasterCompanyData mcda 
                                 INNER JOIN MasterCompanyDetails mcde ON mcde.MasterCompanyId = mcda.MasterCompanyId
                                 INNER JOIN RoleMasterTB rm ON rm.RoleId = mcda.RoleId
                                 INNER JOIN AdminUserLimitSet aus ON aus.UserLimitSetId = mcda.NumberofUsersId
                                 INNER JOIN LicenseValidityMaster avm ON avm.LicenseValidityId = mcda.LicenseValidityId                                 
                                 ORDER BY CONVERT(datetime, mcda.CreatedDateTime, 120) DESC";

                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@AccountStatus", 1);
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new AllMasterCompanyData
                            {
                                MasterCompanyId = Convert.ToInt32(sdr["MasterCompanyId"]),
                                EmailId = sdr["EmailId"].ToString(),
                                Password = sdr["Password"].ToString(),
                                SecurityId = sdr["SecurityId"].ToString(),
                                CompanyName = sdr["CompanyName"].ToString(),
                                RoleName = sdr["RoleName"].ToString(),
                                RoleId = Convert.ToInt32(sdr["RoleId"]),
                                AccountStatus = Convert.ToInt32(sdr["AccountStatus"]),
                                NumberofUsersId = Convert.ToInt32(sdr["NumberofUsersId"]),
                                LicenseCounter = Convert.ToInt32(sdr["LicenseCounter"]),
                                LicenseYear = Convert.ToInt32(sdr["LicenseYear"]),
                                LicenseNumber = sdr["LicenseNumber"].ToString(),
                                LicenseValidityId = Convert.ToInt32(sdr["LicenseValidityId"]),
                                LicenseStatusId = Convert.ToInt32(sdr["LicenseStatusId"]),
                                LicenseRenewRequest = Convert.ToInt32(sdr["LicenseRenewRequest"]),
                                LicenseStartDate = sdr["LicenseStartDate"].ToString(),
                                LicenseEndDate = sdr["LicenseEndDate"].ToString(),
                                IsLicenseExpired = Convert.ToInt32(sdr["IsLicenseExpired"]),
                                LastLoginDateTime = sdr["LastLoginDateTime"].ToString(),
                                CreatedDateTime = sdr["CreatedDateTime"].ToString(),
                                UpdatedDateTime = sdr["UpdatedDateTime"].ToString(),

                                // Properties from MasterCompanyDetails table
                                MasterCompanyDetailsId = Convert.ToInt32(sdr["MasterCompanyDetailsId"]),
                                FirstName = sdr["FirstName"].ToString(),
                                LastName = sdr["LastName"].ToString(),
                                MobileNumber = sdr["MobileNumber"].ToString(),
                                Address = sdr["Address"].ToString(),
                                City = sdr["City"].ToString(),
                                State = sdr["State"].ToString(),
                                Country = sdr["Country"].ToString(),
                                PINCode = sdr["PINCode"].ToString(),
                                PANCardNo = sdr["PANCardNo"].ToString(),
                                GSTNo = sdr["GSTNo"].ToString(),
                                StateGSTCode = sdr["StateGSTCode"].ToString(),
                                CompanyLogo = sdr["CompanyLogo"].ToString(),
                                MaxUserLimit = sdr["LimitMaxNumber"].ToString(),
                                UserLimitSetData = sdr["UserLimitSetData"].ToString(),
                                LicensePeriodData = sdr["LicensePeriodData"].ToString(),
                                TotalMonths = Convert.ToInt32(sdr["TotalMonths"]),
                            });
                        }

                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<LicenseRenewalRequestData> LicenseRenewalRequestShow()
        {
            List<LicenseRenewalRequestData> items = new List<LicenseRenewalRequestData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT
                                    lr.LicenseRenewRequestID, lr.MasterCompanyID, lr.Remarks,
                                    lr.CreatedDate, lr.UpdatedDate, mcd.CompanyName, mcd.LicenseNumber, 
	                                mcd.LicenseEndDate AS LicenseExpireDate, ls.LicenseName AS RequestStatus, lr.RequestStatusId
                                FROM
                                    LicenseRenewalRequest lr
                                INNER JOIN
                                    MasterCompanyData mcd ON mcd.MasterCompanyID = lr.MasterCompanyID
                                INNER JOIN
                                    LicenseStatus ls ON ls.LicenseId = lr.RequestStatusId
                                ORDER BY
                                    CONVERT(datetime, lr.CreatedDate, 106) DESC;";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new LicenseRenewalRequestData
                            {
                                LicenseRenewRequestID = Convert.ToInt32(sdr["LicenseRenewRequestID"].ToString()),
                                MasterCompanyID = Convert.ToInt32(sdr["MasterCompanyID"]),
                                CompanyName = sdr["CompanyName"].ToString(),
                                LicenseNumber = sdr["LicenseNumber"].ToString(),
                                Remarks = sdr["Remarks"].ToString(),
                                LicenseExpireDate = sdr["LicenseExpireDate"].ToString(),
                                RequestStatusId = Convert.ToInt32(sdr["RequestStatusId"]),
                                RequestStatus = sdr["RequestStatus"].ToString(),
                                CreatedDate = sdr["CreatedDate"].ToString(),
                                UpdatedDate = sdr["UpdatedDate"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<UserLicensePeriodModel> UserLicenseDataDisplay()
        {
            List<UserLicensePeriodModel> items = new List<UserLicensePeriodModel>();

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT
                                mcd.MasterCompanyID AS MasterCompanyID,
                                mcd.CompanyName AS CompanyName,
                                mcd.LicenseNumber AS LicenseNumber,
                                lvm.LicensePeriodData,
                                lvm.TotalMonths,
                                ul.LimitMaxNumber AS LimitMaxNumber,
                                mcd.LicenseStartDate AS LicenseStartDate,
                                ls.LicenseName,
                                FORMAT(DATEADD(MONTH, lvm.TotalMonths, mcd.LicenseStartDate), 'dd-MMM-yyyy') AS LicenseExpiredDate,
                                CASE
                                    WHEN DATEADD(MONTH, lvm.TotalMonths, mcd.LicenseStartDate) < GETDATE() THEN 'Expired'
                                    WHEN DATEADD(MONTH, lvm.TotalMonths, mcd.LicenseStartDate) = GETDATE() THEN
                                        CASE
                                            WHEN CONVERT(TIME, DATEADD(MONTH, lvm.TotalMonths, mcd.LicenseStartDate)) < CONVERT(TIME, GETDATE()) THEN 'Expired'
                                            ELSE 'Running'
                                        END
                                    ELSE 'Running'
                                END AS LicenseStatus
                            FROM
                                MasterCompanyData mcd
                                INNER JOIN AdminUserLimitSet ul ON ul.UserLimitSetId = mcd.NumberofUsersId
                                INNER JOIN LicenseValidityMaster lvm ON lvm.LicenseValidityId = mcd.LicenseValidityId
                                INNER JOIN LicenseStatus ls ON ls.LicenseId = mcd.LicenseStatusId
                            ORDER BY
                                CONVERT(datetime, mcd.CreatedDateTime, 106) DESC;";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new UserLicensePeriodModel
                            {
                                MasterCompanyId = Convert.ToInt32(sdr["MasterCompanyID"]),
                                CompanyName = sdr["CompanyName"].ToString(),
                                LicenseNumber = sdr["LicenseNumber"].ToString(),
                                LicensePeriodData = sdr["LicensePeriodData"].ToString(),
                                TotalMonths = Convert.ToInt32(sdr["TotalMonths"].ToString()),
                                MaxUsers = Convert.ToInt32(sdr["LimitMaxNumber"].ToString()),
                                LicenseStartDate = sdr["LicenseStartDate"].ToString(),
                                LicenseExpiredDate = sdr["LicenseExpiredDate"].ToString(),
                                LicenseName = sdr["LicenseName"].ToString(),
                                LicenseStatus = sdr["LicenseStatus"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<LicenseRenewalAllData> AllLicenseRenewalData()
        {
            List<LicenseRenewalAllData> items = new List<LicenseRenewalAllData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @" SELECT *, lrm.CreatedDateTime AS RenewDate from LicenseRenewalMaster lrm 
	                                INNER JOIN MasterCompanyData mcd ON mcd.MasterCompanyId = lrm.MasterCompanyID
	                                INNER JOIN LicenseValidityMaster lvm ON lvm.LicenseValidityId = lrm.LicenseValidityId
	                                INNER JOIN AdminUserLimitSet aus ON aus.UserLimitSetId = lrm.NumberofUsersId
	                                ORDER BY CONVERT(datetime, lrm.CreatedDateTime, 120) DESC";

                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@AccountStatus", 1);
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new LicenseRenewalAllData
                            {
                                CompanyName = sdr["CompanyName"].ToString(),
                                RenewalId = Convert.ToInt32(sdr["RenewalId"]),
                                MasterCompanyID = Convert.ToInt32(sdr["MasterCompanyID"]),
                                LicenseNumber = sdr["LicenseNumber"].ToString(),
                                LicenseValidityId = Convert.ToInt32(sdr["LicenseValidityId"]),
                                NumberofUsersId = Convert.ToInt32(sdr["NumberofUsersId"]),
                                LicenseStartDate = sdr["LicenseStartDate"].ToString(),
                                LicenseEndDate = sdr["LicenseEndDate"].ToString(),
                                LicenseStatusId = Convert.ToInt32(sdr["LicenseStatusId"]),
                                LicenseCounter = Convert.ToInt32(sdr["LicenseCounter"]),
                                LicenseYear = Convert.ToInt32(sdr["LicenseYear"]),
                                CreatedDateTime = sdr["CreatedDateTime"].ToString(),
                                UpdatedDateTime = sdr["UpdatedDateTime"].ToString(),
                                LicensePeriodData = sdr["LicensePeriodData"].ToString(),
                                TotalMonths = Convert.ToInt32(sdr["TotalMonths"]),
                                UserLimitSetId = Convert.ToInt32(sdr["UserLimitSetId"]),
                                UserLimitSetData = sdr["UserLimitSetData"].ToString(),
                                LimitMaxNumber = Convert.ToInt32(sdr["LimitMaxNumber"]),
                                RenewDate = sdr["RenewDate"].ToString(),
                            });
                        }


                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<CategoryNameData> AllCategoryNameDisplayForDropDown()
        {
            List<CategoryNameData> items = new List<CategoryNameData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                //string query = "SELECT * FROM CategoryMaster ORDER BY CategoryName ASC";
                string query = "SELECT * FROM ItemType ORDER BY ItemType ASC";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new CategoryNameData
                            {
                                CategoryId = Convert.ToInt32(sdr["ItemTypeID"]),
                                CategoryName = sdr["ItemType"].ToString(),
                                //CategoryId = Convert.ToInt32(sdr["CategoryId"]),
                                //CategoryMasterCompanyId = Convert.ToInt32(sdr["CategoryMasterCompanyId"]),
                                //CategoryCreatedEmployeeId = Convert.ToInt32(sdr["CategoryCreatedEmployeeId"]),
                                //CategoryName = sdr["CategoryName"].ToString(),
                                //CategoryCreatedDateTime = sdr["CategoryCreatedDateTime"].ToString(),
                                //CategoryUpdatedDateTime = sdr["CategoryUpdatedDateTime"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            if (items.Count == 0)
            {
                return null;
            }
            return items;
        }

        public List<CategoryNameData> AllCategoryNameDisplay()
        {
            List<CategoryNameData> items = new List<CategoryNameData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT * from CategoryMaster ORDER BY CONVERT(datetime, CategoryCreatedDateTime, 120) DESC";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new CategoryNameData
                            {
                                CategoryId = Convert.ToInt32(sdr["CategoryId"]),
                                CategoryMasterCompanyId = Convert.ToInt32(sdr["CategoryMasterCompanyId"]),
                                CategoryCreatedEmployeeId = Convert.ToInt32(sdr["CategoryCreatedEmployeeId"]),
                                CategoryName = sdr["CategoryName"].ToString(),
                                CategoryCreatedDateTime = sdr["CategoryCreatedDateTime"].ToString(),
                                CategoryUpdatedDateTime = sdr["CategoryUpdatedDateTime"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            if (items.Count == 0)
            {
                return null;
            }
            return items;
        }

        public List<PurchaseGroupData> GetAllPurchaseGroups()
        {
            List<PurchaseGroupData> items = new List<PurchaseGroupData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT * FROM PurchaseGroup ORDER BY PurchaseGroupName ASC";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new PurchaseGroupData
                            {
                                PurchaseGroupID = Convert.ToInt32(sdr["PurchaseGroupID"]),
                                PurchaseGroupName = sdr["PurchaseGroupName"].ToString(),
                                ItemTypeID = Convert.ToInt32(sdr["ItemTypeID"])
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items.Count == 0 ? null : items;
        }

        public List<PurchaseSubGroupData> GetAllPurchaseSubGroups()
        {
            List<PurchaseSubGroupData> items = new List<PurchaseSubGroupData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT * FROM PurchaseSubGroup ORDER BY PurchaseSubGroupName ASC";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new PurchaseSubGroupData
                            {
                                PurchaseSubGroupID = Convert.ToInt32(sdr["PurchaseSubGroupID"]),
                                PurchaseSubGroupName = sdr["PurchaseSubGroupName"].ToString(),
                                PurchaseGroupID = Convert.ToInt32(sdr["PurchaseGroupID"]),
                                ItemTypeID = Convert.ToInt32(sdr["ItemTypeID"])
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items.Count == 0 ? null : items;
        }

        public List<Item> GetItemList()
        {
            List<Item> items = new List<Item>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT 
                            I.ItemID, I.ItemCode, I.ItemName, I.ItemDescription, I.PurchaseSubGroupID,
                            I.LeadTime, I.UOMID, I.StockQuantity, I.Minstock, I.MaxStock, I.DisabledItem,
                            I.HSNCode, I.CreatedDate, I.VAT, I.Remarks, I.UOMValue, I.ConversionUOMID,
                            I.ConversionUOMValue, I.ModelNo, I.Rate, I.ItemTypeID, I.Description,
                            I.VATID, I.Specification, I.PurchaseGroupID, I.CreatedBy, I.ConvertUOM,
                            IT.ItemType AS ItemTypeName
                         FROM Items I
                         LEFT JOIN ItemType IT ON I.ItemTypeID = IT.ItemTypeID
                         ORDER BY I.ItemName ASC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new Item
                            {
                                ItemID = sdr["ItemID"] != DBNull.Value ? Convert.ToInt32(sdr["ItemID"]) : 0,
                                ItemCode = sdr["ItemCode"] != DBNull.Value ? sdr["ItemCode"].ToString() : string.Empty,
                                ItemName = sdr["ItemName"] != DBNull.Value ? sdr["ItemName"].ToString() : string.Empty,
                                ItemDescription = sdr["ItemDescription"] != DBNull.Value ? sdr["ItemDescription"].ToString() : string.Empty,
                                PurchaseSubGroupID = sdr["PurchaseSubGroupID"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseSubGroupID"]) : 0,
                                LeadTime = sdr["LeadTime"] != DBNull.Value ? Convert.ToInt32(sdr["LeadTime"]) : 0,
                                UOMID = sdr["UOMID"] != DBNull.Value ? Convert.ToInt32(sdr["UOMID"]) : 0,
                                StockQuantity = sdr["StockQuantity"] != DBNull.Value ? Convert.ToDecimal(sdr["StockQuantity"]) : 0,
                                MinStock = sdr["MinStock"] != DBNull.Value ? Convert.ToDecimal(sdr["MinStock"]) : 0,
                                MaxStock = sdr["MaxStock"] != DBNull.Value ? Convert.ToDecimal(sdr["MaxStock"]) : 0,
                                DisabledItem = sdr["DisabledItem"] != DBNull.Value ? Convert.ToBoolean(sdr["DisabledItem"]) : false,
                                HSNCode = sdr["HSNCode"] != DBNull.Value ? sdr["HSNCode"].ToString() : string.Empty,
                                CreatedDate = sdr["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(sdr["CreatedDate"]) : DateTime.MinValue,
                                VAT = sdr["VAT"] != DBNull.Value ? Convert.ToDecimal(sdr["VAT"]) : 0,
                                Remarks = sdr["Remarks"] != DBNull.Value ? sdr["Remarks"].ToString() : string.Empty,
                                UOMValue = sdr["UOMValue"] != DBNull.Value ? Convert.ToDecimal(sdr["UOMValue"]) : 0,
                                ConversionUOMID = sdr["ConversionUOMID"] != DBNull.Value ? Convert.ToInt32(sdr["ConversionUOMID"]) : 0,
                                ConversionUOMValue = sdr["ConversionUOMValue"] != DBNull.Value ? Convert.ToDecimal(sdr["ConversionUOMValue"]) : 0,
                                ModelNo = sdr["ModelNo"] != DBNull.Value ? sdr["ModelNo"].ToString() : string.Empty,
                                Rate = sdr["Rate"] != DBNull.Value ? Convert.ToDecimal(sdr["Rate"]) : 0,
                                ItemTypeID = sdr["ItemTypeID"] != DBNull.Value ? Convert.ToInt32(sdr["ItemTypeID"]) : 0,
                                Description = sdr["Description"] != DBNull.Value ? sdr["Description"].ToString() : string.Empty,
                                VATID = sdr["VATID"] != DBNull.Value ? Convert.ToInt32(sdr["VATID"]) : 0,
                                Specification = sdr["Specification"] != DBNull.Value ? sdr["Specification"].ToString() : string.Empty,
                                PurchaseGroupID = sdr["PurchaseGroupID"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseGroupID"]) : 0,
                                CreatedBy = sdr["CreatedBy"] != DBNull.Value ? Convert.ToInt32(sdr["CreatedBy"]) : 0,
                                ConvertUOM = sdr["ConvertUOM"] != DBNull.Value ? Convert.ToInt32(sdr["ConvertUOM"]) : 0,
                                ItemTypeName = sdr["ItemTypeName"] != DBNull.Value ? sdr["ItemTypeName"].ToString() : string.Empty
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }


        public List<RackTypeDetailsData> AllRackTypeDetailsDisplay()
        {
            List<RackTypeDetailsData> items = new List<RackTypeDetailsData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * from RackTypeDetails rtd WHERE rtd.RackMasterCompanyId = @RackMasterCompanyId";

                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Parameters.AddWithValue("@RackMasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new RackTypeDetailsData
                            {
                                RackTypeId = Convert.ToInt32(sdr["RackTypeId"]),
                                RackType = sdr["RackType"].ToString(),
                                //RackCompanyUnitId = Convert.ToInt32(sdr["RackCompanyUnitId"]),
                                RackMasterCompanyId = Convert.ToInt32(sdr["RackMasterCompanyId"]),
                                RackCreatedEmployeeId = Convert.ToInt32(sdr["RackCreatedEmployeeId"]),
                                RackCreatedDateTime = sdr["RackCreatedDateTime"].ToString(),
                                RackUpdatedDateTime = sdr["RackUpdatedDateTime"].ToString(),

                                //UnitName = sdr["UnitName"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            if (items.Count == 0)
            {
                return null;
            }
            return items;
        }

        public List<RackTypeDetailsData> RackTypeDetailsUnitWiseDisplay()
        {
            List<RackTypeDetailsData> items = new List<RackTypeDetailsData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT * from RackTypeDetails order by RackType ASC";

                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Parameters.AddWithValue("@RackCompanyUnitId", HttpContext.Current.Session["CompanyUnitId"].ToString());
                    cmd.Parameters.AddWithValue("@RackMasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());

                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new RackTypeDetailsData
                            {
                                RackTypeId = sdr["RackTypeId"] != DBNull.Value ? Convert.ToInt32(sdr["RackTypeId"]) : 0,
                                RackType = sdr["RackType"] != DBNull.Value ? sdr["RackType"].ToString() : string.Empty,
                                RackCompanyUnitId = sdr["RackCompanyUnitId"] != DBNull.Value ? Convert.ToInt32(sdr["RackCompanyUnitId"]) : 0,
                                RackMasterCompanyId = sdr["RackMasterCompanyId"] != DBNull.Value ? Convert.ToInt32(sdr["RackMasterCompanyId"]) : 0,
                                RackCreatedEmployeeId = sdr["RackCreatedEmployeeId"] != DBNull.Value ? Convert.ToInt32(sdr["RackCreatedEmployeeId"]) : 0,
                                RackCreatedDateTime = sdr["RackCreatedDateTime"] != DBNull.Value ? sdr["RackCreatedDateTime"].ToString() : string.Empty,
                                RackUpdatedDateTime = sdr["RackUpdatedDateTime"] != DBNull.Value ? sdr["RackUpdatedDateTime"].ToString() : string.Empty
                            });

                        }
                    }
                    con.Close();
                }
            }
            if (items.Count == 0)
            {
                return null;
            }
            return items;
        }

        public List<SerialNumberMasterData> AlphaSerialNumberDisplay()
        {
            List<SerialNumberMasterData> items = new List<SerialNumberMasterData>();

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT * FROM SerialNumberMaster";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    /*  cmd.Parameters.AddWithValue("@IsActive", 1);    */
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new SerialNumberMasterData
                            {
                                SerialNumberId = Convert.ToInt32(sdr["SerialNumberId"]),
                                SerialNumber = sdr["SerialNumber"].ToString(),
                            });
                        }
                    }
                }
            }
            return items;
        }

        public List<RackNumberData> AllRackRowSerialNumberDisplay()
        {
            List<RackNumberData> items = new List<RackNumberData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * from RackSerialDetails rsd
                                  INNER JOIN RackTypeDetails rtd ON rtd.RackTypeId = rsd.RackTypeId";

                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new RackNumberData
                            {
                                //BranchId = Convert.ToInt32(sdr["BranchId"]),
                                //BranchName = sdr["BranchName"].ToString(),
                                //CompanyUnitId = Convert.ToInt32(sdr["CompanyUnitId"]),
                                //UnitName = sdr["UnitName"].ToString(),
                                RackTypeId = Convert.ToInt32(sdr["RackTypeId"]),
                                RackType = sdr["RackType"].ToString(),
                                RackMasterCompanyId = Convert.ToInt32(sdr["RackMasterCompanyId"]),
                                RackCreatedEmployeeId = Convert.ToInt32(sdr["RackCreatedEmployeeId"]),
                                RackCreatedDateTime = sdr["RackCreatedDateTime"].ToString(),
                                RackUpdatedDateTime = sdr["RackUpdatedDateTime"].ToString(),
                                //RackNumberId = Convert.ToInt32(sdr["RackNumberId"]),
                                //RackNumber = Convert.ToInt32(sdr["RackNumber"]),
                                //TotalRackRowNumber = Convert.ToInt32(sdr["TotalRackRowNumber"]),
                                //RackRowId = Convert.ToInt32(sdr["RackRowId"]),
                                //RackRowNumber = Convert.ToInt32(sdr["RackRowNumber"]),
                                RackAlphSeries = sdr["RackAlphSeries"].ToString(),
                                TotalSerialNumber = Convert.ToInt32(sdr["TotalSerialNumber"]),
                                RackSerialNumber = sdr["RackSerialNumber"].ToString(),
                                RackSerialId = Convert.ToInt32(sdr["RackSerialId"]),
                                //TotalRackNumber = Convert.ToInt32(sdr["TotalRackNumber"]),
                                CreatedEmployeeId = Convert.ToInt32(sdr["CreatedEmployeeId"]),
                                MasterCompanyId = Convert.ToInt32(sdr["MasterCompanyId"]),


                            });
                        }
                    }
                    con.Close();
                }
            }
            if (items.Count == 0)
            {
                return null;
            }
            return items;
        }

        public List<RackNumberData> AllRackRowSerialNumberDisplay(string itemCode)
        {
            List<RackNumberData> items = new List<RackNumberData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * from InventoryMaster im 
                                 INNER JOIN Items imm ON imm.ItemId = im.InventoryItemId
                                 INNER JOIN RackTypeDetails rtd ON rtd.RackTypeId = im.InventoryRackTypeId
                                 INNER JOIN RackSerialDetails rsd ON  rtd.RackTypeId = rsd.RackTypeId
                                 WHERE imm.ItemCode = @ItemCode
						          AND EXISTS (SELECT 1 FROM STRING_SPLIT(im.InventoryItemLocationStore, ',') AS s 
                                 WHERE TRY_CAST(s.value AS INT) = rsd.RackSerialId)";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@ItemCode", itemCode);
                    con.Open();

                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new RackNumberData
                            {
                                RackTypeId = Convert.ToInt32(sdr["RackTypeId"]),
                                RackType = sdr["RackType"].ToString(),
                                RackMasterCompanyId = Convert.ToInt32(sdr["RackMasterCompanyId"]),
                                RackCreatedEmployeeId = Convert.ToInt32(sdr["RackCreatedEmployeeId"]),
                                RackCreatedDateTime = sdr["RackCreatedDateTime"].ToString(),
                                RackUpdatedDateTime = sdr["RackUpdatedDateTime"].ToString(),

                                RackAlphSeries = sdr["RackAlphSeries"].ToString(),
                                TotalSerialNumber = Convert.ToInt32(sdr["TotalSerialNumber"]),
                                RackSerialNumber = sdr["RackSerialNumber"].ToString(),
                                RackSerialId = Convert.ToInt32(sdr["RackSerialId"]),
                                CreatedEmployeeId = Convert.ToInt32(sdr["CreatedEmployeeId"]),
                                MasterCompanyId = Convert.ToInt32(sdr["MasterCompanyId"]),
                            });
                        }
                    }

                    con.Close();
                }
            }

            if (items.Count == 0)
            {
                return null;
            }

            return items;
        }

        /* Rack Serial number show in table */
        public List<RackNumberData> AllTotalRackRowSerialNumberDisplay()
        {
            List<RackNumberData> items = new List<RackNumberData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                //string query = @"SELECT cum.CompanyUnitId,rtd.RackTypeId,cum.UnitName, 
                //                rtd.RackType,rsd.RackAlphSeries,rsd.TotalSerialNumber,rsd.MasterCompanyId 
                //                FROM RackSerialDetails rsd                                 
                //                INNER JOIN CompanyUnitMaster cum ON cum.CompanyUnitId = rsd.CompanyUnitId
                //                INNER JOIN RackTypeDetails rtd ON rtd.RackTypeId = rsd.RackTypeId
                //                WHERE rsd.MasterCompanyId = @MasterCompanyId
                //                GROUP BY cum.CompanyUnitId, cum.UnitName, rtd.RackType, rtd.RackTypeId,
                //                         rsd.RackAlphSeries,rsd.TotalSerialNumber,rsd.MasterCompanyId";

                string query = @"SELECT rtd.RackTypeId, rtd.RackType,rsd.RackAlphSeries,rsd.TotalSerialNumber,rsd.MasterCompanyId 
                                FROM RackSerialDetails rsd                                
                                INNER JOIN RackTypeDetails rtd ON rtd.RackTypeId = rsd.RackTypeId
                                WHERE rsd.MasterCompanyId = 1
                                GROUP BY rtd.RackType, rtd.RackTypeId,rsd.RackAlphSeries,rsd.TotalSerialNumber,rsd.MasterCompanyId";


                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Parameters.AddWithValue("@MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new RackNumberData
                            {
                                RackTypeId = Convert.ToInt32(sdr["RackTypeId"]),
                                RackType = sdr["RackType"].ToString(),
                                RackAlphSeries = sdr["RackAlphSeries"].ToString(),
                                TotalSerialNumber = Convert.ToInt32(sdr["TotalSerialNumber"]),
                            });
                        }
                    }
                    con.Close();
                }
            }
            if (items.Count == 0)
            {
                return null;
            }
            return items;
        }

        /* Active Vendor Details Show from the Vendor table */
        public List<VendorDetailsData> ActiveVendorDetailsGet()
        {
            List<VendorDetailsData> items = new List<VendorDetailsData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * from VendorDetailsMaster
                                WHERE VendorIsActive = @VendorIsActive AND VendorMasterCompanyId = @VendorMasterCompanyId
                                ORDER BY CONVERT(datetime, VendorCreatedDateTime, 120) DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@VendorIsActive", 1);
                    cmd.Parameters.AddWithValue("@VendorMasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new VendorDetailsData
                            {
                                VendorId = Convert.ToInt32(sdr["VendorId"]),
                                VendorCode = sdr["VendorCode"].ToString(),
                                VendorName = sdr["VendorName"].ToString(),
                                VendorEmail = sdr["VendorEmail"].ToString(),
                                VendorMobileNumber = sdr["VendorMobileNumber"].ToString(),
                                VendorCompanyName = sdr["VendorCompanyName"].ToString(),
                                VendorCompanyAddress = sdr["VendorCompanyAddress"].ToString(),
                                VendorCompanyGSTNumber = sdr["VendorCompanyGSTNumber"].ToString(),
                                VendorCompanyPANNumber = sdr["VendorCompanyPANNumber"].ToString(),
                                VendorCountry = sdr["VendorCountry"].ToString(),
                                VendorState = sdr["VendorState"].ToString(),
                                VendorCity = sdr["VendorCity"].ToString(),
                                VendorPinCode = sdr["VendorPinCode"].ToString(),
                                VendorBusinessType = sdr["VendorBusinessType"].ToString(),
                                VendorCreatedDateTime = sdr["VendorCreatedDateTime"].ToString(),
                                VendorUpdatedDateTime = sdr["VendorUpdatedDateTime"].ToString(),
                                VendorIsActive = Convert.ToInt32(sdr["VendorIsActive"]),
                                VendorMasterCompanyId = Convert.ToInt32(sdr["VendorMasterCompanyId"]),
                                VendorCreatedEmployeeId = Convert.ToInt32(sdr["VendorCreatedEmployeeId"]),

                                VendorNameAndCompanyName = $"{sdr["VendorName"]} - {sdr["VendorCompanyName"]}"

                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        /* All Vendor Details Show from the Vendor table */
        public List<VendorDetailsData> AllVendorDetailsGet()
        {
            List<VendorDetailsData> items = new List<VendorDetailsData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * from VendorDetailsMaster
                                 WHERE VendorMasterCompanyId = @VendorMasterCompanyId
                                 ORDER BY CONVERT(datetime, VendorCreatedDateTime, 120) DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@VendorMasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new VendorDetailsData
                            {
                                VendorId = Convert.ToInt32(sdr["VendorId"]),
                                VendorCode = sdr["VendorCode"].ToString(),
                                VendorName = sdr["VendorName"].ToString(),
                                VendorEmail = sdr["VendorEmail"].ToString(),
                                VendorMobileNumber = sdr["VendorMobileNumber"].ToString(),
                                VendorCompanyName = sdr["VendorCompanyName"].ToString(),
                                VendorCompanyAddress = sdr["VendorCompanyAddress"].ToString(),
                                VendorCompanyGSTNumber = sdr["VendorCompanyGSTNumber"].ToString(),
                                VendorCompanyPANNumber = sdr["VendorCompanyPANNumber"].ToString(),
                                VendorCountry = sdr["VendorCountry"].ToString(),
                                VendorState = sdr["VendorState"].ToString(),
                                VendorCity = sdr["VendorCity"].ToString(),
                                VendorPinCode = sdr["VendorPinCode"].ToString(),
                                VendorBusinessType = sdr["VendorBusinessType"].ToString(),
                                VendorCreatedDateTime = sdr["VendorCreatedDateTime"].ToString(),
                                VendorUpdatedDateTime = sdr["VendorUpdatedDateTime"].ToString(),
                                VendorIsActive = Convert.ToInt32(sdr["VendorIsActive"]),
                                VendorMasterCompanyId = Convert.ToInt32(sdr["VendorMasterCompanyId"]),
                                VendorCreatedEmployeeId = Convert.ToInt32(sdr["VendorCreatedEmployeeId"])
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

      
        /* All UOM Details Show from the UOM table */
        public List<UOMData> UOMDetailsGet()
        {
            List<UOMData> items = new List<UOMData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * from UOM";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new UOMData
                            {
                                UOMId = Convert.ToInt32(sdr["UOMId"]),
                                UOMCode = sdr["UOMName"].ToString(),
                                UOMName = sdr["UOMName"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

       public List<InventoryItemData> ItemNameForDropDown()
        {
            List<InventoryItemData> items = new List<InventoryItemData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * from Items itm 
                                INNER JOIN ItemType it ON it.ItemTypeID = itm.ItemTypeID
                                INNER JOIN UOM um ON um.UOMId = itm.UOMID
                                INNER JOIN VAT vt ON vt.VATID = itm.VATID
                                ORDER BY itm.ItemName ASC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new InventoryItemData
                            {
                                ItemID = sdr["ItemID"] != DBNull.Value ? Convert.ToInt32(sdr["ItemID"]) : 0,
                                ItemCode = sdr["ItemCode"] != DBNull.Value ? sdr["ItemCode"].ToString() : string.Empty,
                                ItemName = sdr["ItemName"] != DBNull.Value ? sdr["ItemName"].ToString() : string.Empty,
                                ItemDescription = sdr["ItemDescription"] != DBNull.Value ? sdr["ItemDescription"].ToString() : string.Empty,
                                PurchaseSubGroupID = sdr["PurchaseSubGroupID"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseSubGroupID"]) : 0,
                                LeadTime = sdr["LeadTime"] != DBNull.Value ? Convert.ToInt32(sdr["LeadTime"]) : 0,
                                UOMID = sdr["UOMID"] != DBNull.Value ? Convert.ToInt32(sdr["UOMID"]) : 0,
                                StockQuantity = sdr["StockQuantity"] != DBNull.Value ? Convert.ToDecimal(sdr["StockQuantity"]) : 0,
                                MinStock = sdr["MinStock"] != DBNull.Value ? Convert.ToDecimal(sdr["MinStock"]) : 0,
                                MaxStock = sdr["MaxStock"] != DBNull.Value ? Convert.ToDecimal(sdr["MaxStock"]) : 0,
                                DisabledItem = sdr["DisabledItem"] != DBNull.Value ? Convert.ToBoolean(sdr["DisabledItem"]) : false,
                                HSNCode = sdr["HSNCode"] != DBNull.Value ? sdr["HSNCode"].ToString() : string.Empty,
                                CreatedDate = sdr["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(sdr["CreatedDate"]) : DateTime.MinValue,
                                VAT = sdr["VAT"] != DBNull.Value ? Convert.ToDecimal(sdr["VAT"]) : 0,
                                Remarks = sdr["Remarks"] != DBNull.Value ? sdr["Remarks"].ToString() : string.Empty,
                                UOMValue = sdr["UOMValue"] != DBNull.Value ? Convert.ToDecimal(sdr["UOMValue"]) : 0,
                                ConversionUOMID = sdr["ConversionUOMID"] != DBNull.Value ? Convert.ToInt32(sdr["ConversionUOMID"]) : 0,
                                ConversionUOMValue = sdr["ConversionUOMValue"] != DBNull.Value ? Convert.ToDecimal(sdr["ConversionUOMValue"]) : 0,
                                ModelNo = sdr["ModelNo"] != DBNull.Value ? sdr["ModelNo"].ToString() : string.Empty,
                                Rate = sdr["Rate"] != DBNull.Value ? Convert.ToDecimal(sdr["Rate"]) : 0,
                                ItemTypeID = sdr["ItemTypeID"] != DBNull.Value ? Convert.ToInt32(sdr["ItemTypeID"]) : 0,
                                Description = sdr["Description"] != DBNull.Value ? sdr["Description"].ToString() : string.Empty,
                                VATID = sdr["VATID"] != DBNull.Value ? Convert.ToInt32(sdr["VATID"]) : 0,
                                Specification = sdr["Specification"] != DBNull.Value ? sdr["Specification"].ToString() : string.Empty,
                                PurchaseGroupID = sdr["PurchaseGroupID"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseGroupID"]) : 0,
                                CreatedBy = sdr["CreatedBy"] != DBNull.Value ? Convert.ToInt32(sdr["CreatedBy"]) : 0,
                                ConvertUOM = sdr["ConvertUOM"] != DBNull.Value ? Convert.ToInt32(sdr["ConvertUOM"]) : 0,

                                ItemTypeName = sdr["ItemType"] != DBNull.Value ? sdr["ItemType"].ToString() : string.Empty,

                                UOMId = sdr["UOMId"] != DBNull.Value ? Convert.ToInt32(sdr["UOMId"]) : 0,
                                UOMCode = sdr["UOMName"] != DBNull.Value ? sdr["UOMName"].ToString() : string.Empty,
                                UOMName = sdr["UOMName"] != DBNull.Value ? sdr["UOMName"].ToString() : string.Empty,

                                VATCode = sdr["VATCode"] != DBNull.Value ? sdr["VATCode"].ToString() : string.Empty,
                                Percentage = sdr["Percentage"] != DBNull.Value ? Convert.ToDecimal(sdr["Percentage"]) : 0
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<ItemMasterData> ItemShowNotInventory()
        {
            List<ItemMasterData> items = new List<ItemMasterData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT IM.ItemId, IM.ItemCategoryId, IM.ItemUOMId, IM.ItemGSTId, IM.ItemCode, IM.ItemName,
                                    IM.ItemDescription, IM.ItemHSNCode, IM.ItemCreatedEmployeeId, IM.ItemMasterCompanyId,
                                    IM.ItemCreatedDateTime, IM.ItemUpdatedDateTime, IM.ItemImageName
                                FROM ItemMaster AS IM
                                WHERE 
                                    NOT EXISTS (SELECT 1 FROM InventoryMaster AS IV
                                        WHERE IM.ItemId = IV.InventoryItemId AND IV.InventoryCompanyUnitId = @InventoryCompanyUnitId AND
                                              IV.InventoryMasterCompanyId = @InventoryMasterCompanyId)
                                ORDER BY 
                                    IM.ItemName ASC
                                ";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@InventoryMasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    cmd.Parameters.AddWithValue("@InventoryCompanyUnitId", HttpContext.Current.Session["CompanyUnitId"].ToString());
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new ItemMasterData
                            {
                                ItemId = Convert.IsDBNull(sdr["ItemId"]) ? 0 : Convert.ToInt32(sdr["ItemId"]),
                                ItemCategoryId = Convert.IsDBNull(sdr["ItemCategoryId"]) ? 0 : Convert.ToInt32(sdr["ItemCategoryId"]),
                                ItemUOMId = Convert.IsDBNull(sdr["ItemUOMId"]) ? 0 : Convert.ToInt32(sdr["ItemUOMId"]),
                                ItemGSTId = Convert.IsDBNull(sdr["ItemGSTId"]) ? 0 : Convert.ToInt32(sdr["ItemGSTId"]),
                                ItemCode = Convert.IsDBNull(sdr["ItemCode"]) ? string.Empty : sdr["ItemCode"].ToString(),
                                ItemName = Convert.IsDBNull(sdr["ItemName"]) ? string.Empty : sdr["ItemName"].ToString(),
                                ItemDescription = Convert.IsDBNull(sdr["ItemDescription"]) ? string.Empty : sdr["ItemDescription"].ToString(),
                                ItemHSNCode = Convert.IsDBNull(sdr["ItemHSNCode"]) ? string.Empty : sdr["ItemHSNCode"].ToString(),
                                ItemCreatedEmployeeId = Convert.IsDBNull(sdr["ItemCreatedEmployeeId"]) ? 0 : Convert.ToInt32(sdr["ItemCreatedEmployeeId"]),
                                ItemMasterCompanyId = Convert.IsDBNull(sdr["ItemMasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["ItemMasterCompanyId"]),
                                ItemCreatedDateTime = Convert.IsDBNull(sdr["ItemCreatedDateTime"]) ? string.Empty : sdr["ItemCreatedDateTime"].ToString(),
                                ItemUpdatedDateTime = Convert.IsDBNull(sdr["ItemUpdatedDateTime"]) ? string.Empty : sdr["ItemUpdatedDateTime"].ToString(),
                                ItemImageName = Convert.IsDBNull(sdr["ItemImageName"]) ? string.Empty : sdr["ItemImageName"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }



        /* Item Details Show from the ItemMaster table */
        public List<InventoryItemData> ActiveItemMasterDetailsGet()
        {
            List<InventoryItemData> items = new List<InventoryItemData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * from  Items itm 
                                    INNER JOIN ItemType it ON it.ItemTypeID = itm.ItemTypeID
                                    INNER JOIN UOM um ON um.UOMId = itm.UOMID
                                    INNER JOIN VAT vt on vt.VATID = itm.VATID
                                    ORDER BY itm.ItemID DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new InventoryItemData
                            {

                                ItemID = sdr["ItemID"] != DBNull.Value ? Convert.ToInt32(sdr["ItemID"]) : 0,
                                ItemCode = sdr["ItemCode"] != DBNull.Value ? sdr["ItemCode"].ToString() : string.Empty,
                                ItemName = sdr["ItemName"] != DBNull.Value ? sdr["ItemName"].ToString() : string.Empty,
                                ItemDescription = sdr["ItemDescription"] != DBNull.Value ? sdr["ItemDescription"].ToString() : string.Empty,
                                PurchaseSubGroupID = sdr["PurchaseSubGroupID"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseSubGroupID"]) : 0,
                                LeadTime = sdr["LeadTime"] != DBNull.Value ? Convert.ToInt32(sdr["LeadTime"]) : 0,
                                UOMID = sdr["UOMID"] != DBNull.Value ? Convert.ToInt32(sdr["UOMID"]) : 0,
                                StockQuantity = sdr["StockQuantity"] != DBNull.Value ? Convert.ToDecimal(sdr["StockQuantity"]) : 0,
                                MinStock = sdr["MinStock"] != DBNull.Value ? Convert.ToDecimal(sdr["MinStock"]) : 0,
                                MaxStock = sdr["MaxStock"] != DBNull.Value ? Convert.ToDecimal(sdr["MaxStock"]) : 0,
                                DisabledItem = sdr["DisabledItem"] != DBNull.Value ? Convert.ToBoolean(sdr["DisabledItem"]) : false,
                                HSNCode = sdr["HSNCode"] != DBNull.Value ? sdr["HSNCode"].ToString() : string.Empty,
                                CreatedDate = sdr["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(sdr["CreatedDate"]) : DateTime.MinValue,
                                VAT = sdr["VAT"] != DBNull.Value ? Convert.ToDecimal(sdr["VAT"]) : 0,
                                Remarks = sdr["Remarks"] != DBNull.Value ? sdr["Remarks"].ToString() : string.Empty,
                                UOMValue = sdr["UOMValue"] != DBNull.Value ? Convert.ToDecimal(sdr["UOMValue"]) : 0,
                                ConversionUOMID = sdr["ConversionUOMID"] != DBNull.Value ? Convert.ToInt32(sdr["ConversionUOMID"]) : 0,
                                ConversionUOMValue = sdr["ConversionUOMValue"] != DBNull.Value ? Convert.ToDecimal(sdr["ConversionUOMValue"]) : 0,
                                ModelNo = sdr["ModelNo"] != DBNull.Value ? sdr["ModelNo"].ToString() : string.Empty,
                                Rate = sdr["Rate"] != DBNull.Value ? Convert.ToDecimal(sdr["Rate"]) : 0,
                                ItemTypeID = sdr["ItemTypeID"] != DBNull.Value ? Convert.ToInt32(sdr["ItemTypeID"]) : 0,
                                Description = sdr["Description"] != DBNull.Value ? sdr["Description"].ToString() : string.Empty,
                                VATID = sdr["VATID"] != DBNull.Value ? Convert.ToInt32(sdr["VATID"]) : 0,
                                Specification = sdr["Specification"] != DBNull.Value ? sdr["Specification"].ToString() : string.Empty,
                                PurchaseGroupID = sdr["PurchaseGroupID"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseGroupID"]) : 0,
                                CreatedBy = sdr["CreatedBy"] != DBNull.Value ? Convert.ToInt32(sdr["CreatedBy"]) : 0,
                                ConvertUOM = sdr["ConvertUOM"] != DBNull.Value ? Convert.ToInt32(sdr["ConvertUOM"]) : 0,
                                ItemTypeName = sdr["ItemType"] != DBNull.Value ? sdr["ItemType"].ToString() : string.Empty,

                                UOMId = sdr["UOMId"] != DBNull.Value ? Convert.ToInt32(sdr["UOMId"]) : 0,
                                UOMCode = sdr["UOMName"] != DBNull.Value ? sdr["UOMName"].ToString() : string.Empty,
                                UOMName = sdr["UOMName"] != DBNull.Value ? sdr["UOMName"].ToString() : string.Empty,

                                VATCode = sdr["VATCode"] != DBNull.Value ? sdr["VATCode"].ToString() : string.Empty,
                                Percentage = sdr["Percentage"] != DBNull.Value ? Convert.ToDecimal(sdr["Percentage"]) : 0,
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<ItemMasterData> ActiveItemMasterDetailsGetPaged(int pageIndex, int pageSize, string searchTerm, out int totalRecords)
        {
            List<ItemMasterData> items = new List<ItemMasterData>();
            totalRecords = 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string countQuery = @"SELECT COUNT(*) FROM ItemMaster im
                                    INNER JOIN CategoryMaster cm ON cm.CategoryId = im.ItemCategoryId
                                    INNER JOIN UOM um ON um.UOMId = im.ItemUOMId
                                    INNER JOIN VAT vt on vt.VATID = im.ItemGSTId
                                    WHERE im.ItemName LIKE @Search OR im.ItemCode LIKE @Search";

                string dataQuery = @"SELECT * FROM ItemMaster im
                                    INNER JOIN CategoryMaster cm ON cm.CategoryId = im.ItemCategoryId
                                    INNER JOIN UOM um ON um.UOMId = im.ItemUOMId
                                    INNER JOIN VAT vt on vt.VATID = im.ItemGSTId
                                    WHERE im.ItemName LIKE @Search OR im.ItemCode LIKE @Search
                                    ORDER BY CONVERT(datetime, im.ItemCreatedDateTime, 120) DESC
                                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                SqlCommand countCmd = new SqlCommand(countQuery, con);
                SqlCommand dataCmd = new SqlCommand(dataQuery, con);

                string likeSearch = "%" + searchTerm + "%";
                countCmd.Parameters.AddWithValue("@Search", likeSearch);
                dataCmd.Parameters.AddWithValue("@Search", likeSearch);
                dataCmd.Parameters.AddWithValue("@Offset", (pageIndex - 1) * pageSize);
                dataCmd.Parameters.AddWithValue("@PageSize", pageSize);

                con.Open();
                totalRecords = (int)countCmd.ExecuteScalar();

                using (SqlDataReader sdr = dataCmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        items.Add(new ItemMasterData
                        {
                            ItemId = Convert.IsDBNull(sdr["ItemId"]) ? 0 : Convert.ToInt32(sdr["ItemId"]),
                            ItemCategoryId = Convert.IsDBNull(sdr["ItemCategoryId"]) ? 0 : Convert.ToInt32(sdr["ItemCategoryId"]),
                            ItemUOMId = Convert.IsDBNull(sdr["ItemUOMId"]) ? 0 : Convert.ToInt32(sdr["ItemUOMId"]),
                            ItemGSTId = Convert.IsDBNull(sdr["ItemGSTId"]) ? 0 : Convert.ToInt32(sdr["ItemGSTId"]),
                            ItemCode = Convert.IsDBNull(sdr["ItemCode"]) ? string.Empty : sdr["ItemCode"].ToString(),
                            ItemName = Convert.IsDBNull(sdr["ItemName"]) ? string.Empty : sdr["ItemName"].ToString(),
                            ItemDescription = Convert.IsDBNull(sdr["ItemDescription"]) ? string.Empty : sdr["ItemDescription"].ToString(),
                            ItemHSNCode = Convert.IsDBNull(sdr["ItemHSNCode"]) ? string.Empty : sdr["ItemHSNCode"].ToString(),
                            ItemCreatedEmployeeId = Convert.IsDBNull(sdr["ItemCreatedEmployeeId"]) ? 0 : Convert.ToInt32(sdr["ItemCreatedEmployeeId"]),
                            ItemMasterCompanyId = Convert.IsDBNull(sdr["ItemMasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["ItemMasterCompanyId"]),
                            ItemCreatedDateTime = Convert.IsDBNull(sdr["ItemCreatedDateTime"]) ? string.Empty : sdr["ItemCreatedDateTime"].ToString(),
                            ItemUpdatedDateTime = Convert.IsDBNull(sdr["ItemUpdatedDateTime"]) ? string.Empty : sdr["ItemUpdatedDateTime"].ToString(),
                            ItemImageName = Convert.IsDBNull(sdr["ItemImageName"]) ? string.Empty : sdr["ItemImageName"].ToString(),
                            CategoryName = Convert.IsDBNull(sdr["CategoryName"]) ? string.Empty : sdr["CategoryName"].ToString(),
                            UOMId = Convert.IsDBNull(sdr["UOMId"]) ? 0 : Convert.ToInt32(sdr["UOMId"]),
                            UOMCode = Convert.IsDBNull(sdr["UOMCode"]) ? string.Empty : sdr["UOMCode"].ToString(),
                            UOMName = Convert.IsDBNull(sdr["UOMName"]) ? string.Empty : sdr["UOMName"].ToString(),
                            VATID = Convert.IsDBNull(sdr["VATID"]) ? 0 : Convert.ToInt32(sdr["VATID"]),
                            VATCode = Convert.IsDBNull(sdr["VATCode"]) ? string.Empty : sdr["VATCode"].ToString(),
                            Percentage = Convert.IsDBNull(sdr["Percentage"]) ? 0 : Convert.ToDecimal(sdr["Percentage"]),
                        });
                    }
                }

                con.Close();
            }

            return items;
        }

        public List<ItemMasterData> ActiveItemMasterDetailsGetPaged(int pageIndex, int pageSize, string searchTerm, DateTime? startDate, DateTime? endDate, out int totalRecords)
        {
            List<ItemMasterData> items = new List<ItemMasterData>();
            totalRecords = 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string baseWhere = @"WHERE (im.ItemName LIKE @Search OR im.ItemCode LIKE @Search)";

                // Add date filter if dates are provided
                if (startDate.HasValue)
                {
                    baseWhere += " AND CONVERT(date, im.ItemCreatedDateTime) >= @StartDate";
                }
                if (endDate.HasValue)
                {
                    baseWhere += " AND CONVERT(date, im.ItemCreatedDateTime) <= @EndDate";
                }

                string countQuery = $@"SELECT COUNT(*) FROM ItemMaster im
                            INNER JOIN CategoryMaster cm ON cm.CategoryId = im.ItemCategoryId
                            INNER JOIN UOM um ON um.UOMId = im.ItemUOMId
                            INNER JOIN VAT vt on vt.VATID = im.ItemGSTId
                            {baseWhere}";

                string dataQuery = $@"SELECT * FROM ItemMaster im
                            INNER JOIN CategoryMaster cm ON cm.CategoryId = im.ItemCategoryId
                            INNER JOIN UOM um ON um.UOMId = im.ItemUOMId
                            INNER JOIN VAT vt on vt.VATID = im.ItemGSTId
                            {baseWhere}
                            ORDER BY CONVERT(datetime, im.ItemCreatedDateTime, 120) DESC
                            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                using (SqlCommand countCmd = new SqlCommand(countQuery, con))
                using (SqlCommand dataCmd = new SqlCommand(dataQuery, con))
                {
                    string likeSearch = "%" + searchTerm + "%";

                    // Add parameters to both queries
                    foreach (var cmd in new[] { countCmd, dataCmd })
                    {
                        cmd.Parameters.AddWithValue("@Search", likeSearch);

                        if (startDate.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@StartDate", startDate.Value);
                        }
                        if (endDate.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@EndDate", endDate.Value);
                        }
                    }

                    dataCmd.Parameters.AddWithValue("@Offset", (pageIndex - 1) * pageSize);
                    dataCmd.Parameters.AddWithValue("@PageSize", pageSize);

                    con.Open();
                    totalRecords = (int)countCmd.ExecuteScalar();

                    using (SqlDataReader sdr = dataCmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new ItemMasterData
                            {
                                ItemId = Convert.IsDBNull(sdr["ItemId"]) ? 0 : Convert.ToInt32(sdr["ItemId"]),
                                ItemCategoryId = Convert.IsDBNull(sdr["ItemCategoryId"]) ? 0 : Convert.ToInt32(sdr["ItemCategoryId"]),
                                ItemUOMId = Convert.IsDBNull(sdr["ItemUOMId"]) ? 0 : Convert.ToInt32(sdr["ItemUOMId"]),
                                ItemGSTId = Convert.IsDBNull(sdr["ItemGSTId"]) ? 0 : Convert.ToInt32(sdr["ItemGSTId"]),
                                ItemCode = Convert.IsDBNull(sdr["ItemCode"]) ? string.Empty : sdr["ItemCode"].ToString(),
                                ItemName = Convert.IsDBNull(sdr["ItemName"]) ? string.Empty : sdr["ItemName"].ToString(),
                                ItemDescription = Convert.IsDBNull(sdr["ItemDescription"]) ? string.Empty : sdr["ItemDescription"].ToString(),
                                ItemHSNCode = Convert.IsDBNull(sdr["ItemHSNCode"]) ? string.Empty : sdr["ItemHSNCode"].ToString(),
                                ItemCreatedEmployeeId = Convert.IsDBNull(sdr["ItemCreatedEmployeeId"]) ? 0 : Convert.ToInt32(sdr["ItemCreatedEmployeeId"]),
                                ItemMasterCompanyId = Convert.IsDBNull(sdr["ItemMasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["ItemMasterCompanyId"]),
                                ItemCreatedDateTime = Convert.IsDBNull(sdr["ItemCreatedDateTime"]) ? string.Empty : sdr["ItemCreatedDateTime"].ToString(),
                                ItemUpdatedDateTime = Convert.IsDBNull(sdr["ItemUpdatedDateTime"]) ? string.Empty : sdr["ItemUpdatedDateTime"].ToString(),
                                ItemImageName = Convert.IsDBNull(sdr["ItemImageName"]) ? string.Empty : sdr["ItemImageName"].ToString(),
                                CategoryName = Convert.IsDBNull(sdr["CategoryName"]) ? string.Empty : sdr["CategoryName"].ToString(),
                                UOMId = Convert.IsDBNull(sdr["UOMId"]) ? 0 : Convert.ToInt32(sdr["UOMId"]),
                                UOMCode = Convert.IsDBNull(sdr["UOMCode"]) ? string.Empty : sdr["UOMCode"].ToString(),
                                UOMName = Convert.IsDBNull(sdr["UOMName"]) ? string.Empty : sdr["UOMName"].ToString(),
                                VATID = Convert.IsDBNull(sdr["VATID"]) ? 0 : Convert.ToInt32(sdr["VATID"]),
                                VATCode = Convert.IsDBNull(sdr["VATCode"]) ? string.Empty : sdr["VATCode"].ToString(),
                                Percentage = Convert.IsDBNull(sdr["Percentage"]) ? 0 : Convert.ToDecimal(sdr["Percentage"]),
                              
                            });
                        }
                    }
                }
            }

            return items;
        }

        public List<ItemMasterData> ActiveItemMasterDetailsGetPaged(int pageIndex, int pageSize, out int totalRecords)
        {
            List<ItemMasterData> items = new List<ItemMasterData>();
            totalRecords = 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                // Get total count
                string countQuery = "SELECT COUNT(*) FROM ItemMaster";
                SqlCommand countCmd = new SqlCommand(countQuery, con);
                con.Open();
                totalRecords = (int)countCmd.ExecuteScalar();
                con.Close();

                // Get paged data
                string query = @"SELECT * FROM ItemMaster im
                                INNER JOIN CategoryMaster cm ON cm.CategoryId = im.ItemCategoryId
                                INNER JOIN UOM um ON um.UOMId = im.ItemUOMId
                                INNER JOIN VAT vt on vt.VATID = im.ItemGSTId
                                ORDER BY CONVERT(datetime, im.ItemCreatedDateTime, 120) DESC
                                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Offset", (pageIndex - 1) * pageSize);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                con.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        items.Add(new ItemMasterData
                        {
                            ItemId = Convert.IsDBNull(sdr["ItemId"]) ? 0 : Convert.ToInt32(sdr["ItemId"]),
                            ItemCategoryId = Convert.IsDBNull(sdr["ItemCategoryId"]) ? 0 : Convert.ToInt32(sdr["ItemCategoryId"]),
                            ItemUOMId = Convert.IsDBNull(sdr["ItemUOMId"]) ? 0 : Convert.ToInt32(sdr["ItemUOMId"]),
                            ItemGSTId = Convert.IsDBNull(sdr["ItemGSTId"]) ? 0 : Convert.ToInt32(sdr["ItemGSTId"]),
                            ItemCode = Convert.IsDBNull(sdr["ItemCode"]) ? string.Empty : sdr["ItemCode"].ToString(),
                            ItemName = Convert.IsDBNull(sdr["ItemName"]) ? string.Empty : sdr["ItemName"].ToString(),
                            ItemDescription = Convert.IsDBNull(sdr["ItemDescription"]) ? string.Empty : sdr["ItemDescription"].ToString(),
                            ItemHSNCode = Convert.IsDBNull(sdr["ItemHSNCode"]) ? string.Empty : sdr["ItemHSNCode"].ToString(),
                            ItemCreatedEmployeeId = Convert.IsDBNull(sdr["ItemCreatedEmployeeId"]) ? 0 : Convert.ToInt32(sdr["ItemCreatedEmployeeId"]),
                            ItemMasterCompanyId = Convert.IsDBNull(sdr["ItemMasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["ItemMasterCompanyId"]),
                            ItemCreatedDateTime = Convert.IsDBNull(sdr["ItemCreatedDateTime"]) ? string.Empty : sdr["ItemCreatedDateTime"].ToString(),
                            ItemUpdatedDateTime = Convert.IsDBNull(sdr["ItemUpdatedDateTime"]) ? string.Empty : sdr["ItemUpdatedDateTime"].ToString(),
                            ItemImageName = Convert.IsDBNull(sdr["ItemImageName"]) ? string.Empty : sdr["ItemImageName"].ToString(),
                            CategoryName = Convert.IsDBNull(sdr["CategoryName"]) ? string.Empty : sdr["CategoryName"].ToString(),
                            UOMId = Convert.IsDBNull(sdr["UOMId"]) ? 0 : Convert.ToInt32(sdr["UOMId"]),
                            UOMCode = Convert.IsDBNull(sdr["UOMCode"]) ? string.Empty : sdr["UOMCode"].ToString(),
                            UOMName = Convert.IsDBNull(sdr["UOMName"]) ? string.Empty : sdr["UOMName"].ToString(),
                            VATID = Convert.IsDBNull(sdr["VATID"]) ? 0 : Convert.ToInt32(sdr["VATID"]),
                            VATCode = Convert.IsDBNull(sdr["VATCode"]) ? string.Empty : sdr["VATCode"].ToString(),
                            Percentage = Convert.IsDBNull(sdr["Percentage"]) ? 0 : Convert.ToDecimal(sdr["Percentage"]),
                        });
                    }
                }
                con.Close();
            }

            return items;
        }


        /* Inventory Details Show from the InventoryMaster table with join */
        public List<InventoryItemData> ActiveInventoryDetailsGet()
        {
            List<InventoryItemData> items = new List<InventoryItemData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * from InventoryMaster im
                                INNER JOIN Items itm ON itm.ItemId = im.InventoryItemId
                                INNER JOIN ItemType it ON it.ItemTypeID = itm.ItemTypeID
                                INNER JOIN UOM um ON um.UOMId = im.InventoryUOMId
                                INNER JOIN VAT vt ON vt.VATID = im.InventoryGSTId
                                INNER JOIN RackTypeDetails rtd ON rtd.RackTypeId = im.InventoryRackTypeId 
                                WHERE InventoryMasterCompanyId = @InventoryMasterCompanyId
                                ORDER BY CONVERT(datetime, im.InventoryCreatedDateTime, 120) DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@InventoryMasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new InventoryItemData
                            {
                                ItemID = sdr["ItemID"] != DBNull.Value ? Convert.ToInt32(sdr["ItemID"]) : 0,
                                ItemCode = sdr["ItemCode"] != DBNull.Value ? sdr["ItemCode"].ToString() : string.Empty,
                                ItemName = sdr["ItemName"] != DBNull.Value ? sdr["ItemName"].ToString() : string.Empty,
                                ItemDescription = sdr["ItemDescription"] != DBNull.Value ? sdr["ItemDescription"].ToString() : string.Empty,
                                PurchaseSubGroupID = sdr["PurchaseSubGroupID"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseSubGroupID"]) : 0,
                                LeadTime = sdr["LeadTime"] != DBNull.Value ? Convert.ToInt32(sdr["LeadTime"]) : 0,
                                UOMID = sdr["UOMID"] != DBNull.Value ? Convert.ToInt32(sdr["UOMID"]) : 0,
                                StockQuantity = sdr["StockQuantity"] != DBNull.Value ? Convert.ToDecimal(sdr["StockQuantity"]) : 0,
                                MinStock = sdr["MinStock"] != DBNull.Value ? Convert.ToDecimal(sdr["MinStock"]) : 0,
                                MaxStock = sdr["MaxStock"] != DBNull.Value ? Convert.ToDecimal(sdr["MaxStock"]) : 0,
                                DisabledItem = sdr["DisabledItem"] != DBNull.Value ? Convert.ToBoolean(sdr["DisabledItem"]) : false,
                                HSNCode = sdr["HSNCode"] != DBNull.Value ? sdr["HSNCode"].ToString() : string.Empty,
                                CreatedDate = sdr["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(sdr["CreatedDate"]) : DateTime.MinValue,
                                VAT = sdr["VAT"] != DBNull.Value ? Convert.ToDecimal(sdr["VAT"]) : 0,
                                Remarks = sdr["Remarks"] != DBNull.Value ? sdr["Remarks"].ToString() : string.Empty,
                                UOMValue = sdr["UOMValue"] != DBNull.Value ? Convert.ToDecimal(sdr["UOMValue"]) : 0,
                                ConversionUOMID = sdr["ConversionUOMID"] != DBNull.Value ? Convert.ToInt32(sdr["ConversionUOMID"]) : 0,
                                ConversionUOMValue = sdr["ConversionUOMValue"] != DBNull.Value ? Convert.ToDecimal(sdr["ConversionUOMValue"]) : 0,
                                ModelNo = sdr["ModelNo"] != DBNull.Value ? sdr["ModelNo"].ToString() : string.Empty,
                                Rate = sdr["Rate"] != DBNull.Value ? Convert.ToDecimal(sdr["Rate"]) : 0,
                                ItemTypeID = sdr["ItemTypeID"] != DBNull.Value ? Convert.ToInt32(sdr["ItemTypeID"]) : 0,
                                Description = sdr["Description"] != DBNull.Value ? sdr["Description"].ToString() : string.Empty,
                                VATID = sdr["VATID"] != DBNull.Value ? Convert.ToInt32(sdr["VATID"]) : 0,
                                Specification = sdr["Specification"] != DBNull.Value ? sdr["Specification"].ToString() : string.Empty,
                                PurchaseGroupID = sdr["PurchaseGroupID"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseGroupID"]) : 0,
                                CreatedBy = sdr["CreatedBy"] != DBNull.Value ? Convert.ToInt32(sdr["CreatedBy"]) : 0,
                                ConvertUOM = sdr["ConvertUOM"] != DBNull.Value ? Convert.ToInt32(sdr["ConvertUOM"]) : 0,
                                ItemTypeName = sdr["ItemType"] != DBNull.Value ? sdr["ItemType"].ToString() : string.Empty,

                                InventoryId = sdr["InventoryId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryId"]) : 0,
                                InventoryRackTypeId = sdr["InventoryRackTypeId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryRackTypeId"]) : 0,
                                InventoryItemCategoryId = sdr["InventoryItemCategoryId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryItemCategoryId"]) : 0,
                                InventoryItemId = sdr["InventoryItemId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryItemId"]) : 0,
                                InventoryUOMId = sdr["InventoryUOMId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryUOMId"]) : 0,
                                InventoryGSTId = sdr["InventoryGSTId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryGSTId"]) : 0,
                                InventoryCurrentStock = sdr["InventoryCurrentStock"] != DBNull.Value ? Convert.ToDecimal(sdr["InventoryCurrentStock"]) : 0,
                                InventoryMinStock = sdr["InventoryMinStock"] != DBNull.Value ? Convert.ToDecimal(sdr["InventoryMinStock"]) : 0,
                                InventoryCreatedEmployeeId = sdr["InventoryCreatedEmployeeId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryCreatedEmployeeId"]) : 0,
                                InventoryMasterCompanyId = sdr["InventoryMasterCompanyId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryMasterCompanyId"]) : 0,
                                InventoryItemLocationStore = sdr["InventoryItemLocationStore"] != DBNull.Value ? sdr["InventoryItemLocationStore"].ToString() : string.Empty,
                                InventoryCreatedDateTime = sdr["InventoryCreatedDateTime"] != DBNull.Value ? sdr["InventoryCreatedDateTime"].ToString() : string.Empty,
                                InventoryUpdatedDateTime = sdr["InventoryUpdatedDateTime"] != DBNull.Value ? sdr["InventoryUpdatedDateTime"].ToString() : string.Empty,

                                UOMId = sdr["UOMId"] != DBNull.Value ? Convert.ToInt32(sdr["UOMId"]) : 0,
                                UOMCode = sdr["UOMName"] != DBNull.Value ? sdr["UOMName"].ToString() : string.Empty,
                                UOMName = sdr["UOMName"] != DBNull.Value ? sdr["UOMName"].ToString() : string.Empty,

                                VATCode = sdr["VATCode"] != DBNull.Value ? sdr["VATCode"].ToString() : string.Empty,
                                Percentage = sdr["Percentage"] != DBNull.Value ? Convert.ToDecimal(sdr["Percentage"]) : 0,                               

                                RackTypeId = sdr["RackTypeId"] != DBNull.Value ? Convert.ToInt32(sdr["RackTypeId"]) : 0,
                                RackType = sdr["RackType"] != DBNull.Value ? sdr["RackType"].ToString() : string.Empty,
                                RackMasterCompanyId = sdr["RackMasterCompanyId"] != DBNull.Value ? Convert.ToInt32(sdr["RackMasterCompanyId"]) : 0,
                                RackCreatedEmployeeId = sdr["RackCreatedEmployeeId"] != DBNull.Value ? Convert.ToInt32(sdr["RackCreatedEmployeeId"]) : 0,
                                RackCreatedDateTime = sdr["RackCreatedDateTime"] != DBNull.Value ? sdr["RackCreatedDateTime"].ToString() : string.Empty,
                                RackUpdatedDateTime = sdr["RackUpdatedDateTime"] != DBNull.Value ? sdr["RackUpdatedDateTime"].ToString() : string.Empty
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<InventoryItemData> ActiveInventoryDetailsRackSeriesDataGet()
        {
            List<InventoryItemData> items = new List<InventoryItemData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * from InventoryMaster im 
                                 INNER JOIN ItemMaster imm ON imm.ItemId = im.InventoryItemId
                                 INNER JOIN RackTypeDetails rtd ON rtd.RackTypeId = im.InventoryRackTypeId
                                 INNER JOIN RackSerialDetails rsd ON  rtd.RackTypeId = rsd.RackTypeId 
                                 INNER JOIN CompanyUnitMaster cum ON cum.CompanyUnitId = rsd.CompanyUnitId
						         INNER JOIN UOM um ON um.UOMId = im.InventoryUOMId
                                 INNER JOIN VAT vt on vt.VATID = im.InventoryGSTId						 
                                 WHERE  IM.InventoryMasterCompanyId = 1 AND 
                                 CHARINDEX(CAST(RackSerialId as varchar), Cast(InventoryItemLocationStore as varchar))>0";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@InventoryMasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new InventoryItemData
                            {
                                InventoryId = Convert.ToInt32(sdr["InventoryId"]),
                                InventoryCompanyUnitId = Convert.ToInt32(sdr["InventoryCompanyUnitId"]),
                                InventoryRackTypeId = Convert.ToInt32(sdr["InventoryRackTypeId"]),
                                InventoryItemCategoryId = Convert.ToInt32(sdr["InventoryItemCategoryId"]),
                                InventoryItemId = Convert.ToInt32(sdr["InventoryItemId"]),
                                InventoryUOMId = Convert.ToInt32(sdr["InventoryUOMId"]),
                                InventoryGSTId = Convert.ToInt32(sdr["InventoryGSTId"]),
                                InventoryCurrentStock = Convert.ToDecimal(sdr["InventoryCurrentStock"]),
                                InventoryMinStock = Convert.ToDecimal(sdr["InventoryMinStock"]),
                                InventoryCreatedEmployeeId = Convert.ToInt32(sdr["InventoryCreatedEmployeeId"]),
                                InventoryMasterCompanyId = Convert.ToInt32(sdr["InventoryMasterCompanyId"]),
                                InventoryItemLocationStore = sdr["InventoryItemLocationStore"].ToString(),
                                InventoryCreatedDateTime = sdr["InventoryCreatedDateTime"].ToString(),
                                InventoryUpdatedDateTime = sdr["InventoryUpdatedDateTime"].ToString(),

                                ItemId = Convert.ToInt32(sdr["ItemId"]),
                                ItemCategoryId = Convert.ToInt32(sdr["ItemCategoryId"]),
                                ItemUOMId = Convert.ToInt32(sdr["ItemUOMId"]),
                                ItemGSTId = Convert.ToInt32(sdr["ItemGSTId"]),
                                ItemCode = sdr["ItemCode"].ToString(),
                                ItemName = sdr["ItemName"].ToString(),
                                ItemDescription = sdr["ItemDescription"].ToString(),
                                ItemHSNCode = sdr["ItemHSNCode"].ToString(),
                                ItemCreatedEmployeeId = Convert.ToInt32(sdr["ItemCreatedEmployeeId"]),
                                ItemMasterCompanyId = Convert.ToInt32(sdr["ItemMasterCompanyId"]),
                                ItemCreatedDateTime = sdr["ItemCreatedDateTime"].ToString(),
                                ItemUpdatedDateTime = sdr["ItemUpdatedDateTime"].ToString(),
                                ItemImageName = sdr["ItemImageName"].ToString(),

                                UOMId = Convert.ToInt32(sdr["UOMId"]),
                                UOMCode = sdr["UOMCode"].ToString(),
                                UOMName = sdr["UOMName"].ToString(),

                                VATID = Convert.ToInt32(sdr["VATID"].ToString()),
                                VATCode = sdr["VATCode"].ToString(),
                                Percentage = Convert.ToDecimal(sdr["Percentage"]),

                                RackTypeId = Convert.IsDBNull(sdr["RackTypeId"]) ? 0 : Convert.ToInt32(sdr["RackTypeId"]),
                                RackType = Convert.IsDBNull(sdr["RackType"]) ? string.Empty : sdr["RackType"].ToString(),
                                RackMasterCompanyId = Convert.IsDBNull(sdr["RackMasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["RackMasterCompanyId"]),
                                RackCreatedEmployeeId = Convert.IsDBNull(sdr["RackCreatedEmployeeId"]) ? 0 : Convert.ToInt32(sdr["RackCreatedEmployeeId"]),
                                RackCreatedDateTime = Convert.IsDBNull(sdr["RackCreatedDateTime"]) ? string.Empty : sdr["RackCreatedDateTime"].ToString(),
                                RackUpdatedDateTime = Convert.IsDBNull(sdr["RackUpdatedDateTime"]) ? string.Empty : sdr["RackUpdatedDateTime"].ToString(),
                                RackAlphSeries = Convert.IsDBNull(sdr["RackAlphSeries"]) ? string.Empty : sdr["RackAlphSeries"].ToString(),
                                TotalSerialNumber = Convert.IsDBNull(sdr["TotalSerialNumber"]) ? 0 : Convert.ToInt32(sdr["TotalSerialNumber"]),
                                RackSerialNumber = Convert.IsDBNull(sdr["RackSerialNumber"]) ? string.Empty : sdr["RackSerialNumber"].ToString(),
                                RackSerialId = Convert.IsDBNull(sdr["RackSerialId"]) ? 0 : Convert.ToInt32(sdr["RackSerialId"]),

                                UnitName = sdr["UnitName"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<InventoryItemData> ActiveInventoryDetailsRackSeriesDataGetPaged(int pageIndex, int pageSize, string searchTerm, DateTime? startDate, DateTime? endDate, out int totalRecords)
        {
            List<InventoryItemData> items = new List<InventoryItemData>();
            totalRecords = 0;

            using (SqlConnection con = new SqlConnection(constr))
            {

                string countQuery = @"SELECT COUNT(*) 
           FROM InventoryMaster im 
           INNER JOIN Items imm ON imm.ItemId = im.InventoryItemId
           INNER JOIN ItemType IT ON imm.ItemTypeID = IT.ItemTypeID
           INNER JOIN RackTypeDetails rtd ON rtd.RackTypeId = im.InventoryRackTypeId
           INNER JOIN RackSerialDetails rsd ON rtd.RackTypeId = rsd.RackTypeId 
           INNER JOIN UOM um ON um.UOMId = im.InventoryUOMId
           INNER JOIN VAT vt on vt.VATID = im.InventoryGSTId 
           WHERE im.InventoryMasterCompanyId = @CompanyId
                  AND EXISTS (SELECT 1 FROM STRING_SPLIT(im.InventoryItemLocationStore, ',') AS s 
                  WHERE TRY_CAST(s.value AS INT) = rsd.RackSerialId)
           AND (imm.ItemName LIKE @Search OR imm.ItemCode LIKE @Search)
           AND (@StartDate IS NULL OR CONVERT(datetime, im.InventoryCreatedDateTime, 120) >= @StartDate)
           AND (@EndDate IS NULL OR CONVERT(datetime, im.InventoryCreatedDateTime, 120) <= @EndDate)";



                string dataQuery = $@"SELECT * 
          FROM InventoryMaster im 
          INNER JOIN Items imm ON imm.ItemId = im.InventoryItemId
          INNER JOIN ItemType IT ON imm.ItemTypeID = IT.ItemTypeID
          INNER JOIN RackTypeDetails rtd ON rtd.RackTypeId = im.InventoryRackTypeId
          INNER JOIN RackSerialDetails rsd ON rtd.RackTypeId = rsd.RackTypeId
          INNER JOIN UOM um ON um.UOMId = im.InventoryUOMId
          INNER JOIN VAT vt on vt.VATID = im.InventoryGSTId 
          WHERE im.InventoryMasterCompanyId = @CompanyId 
           AND EXISTS (SELECT 1 FROM STRING_SPLIT(im.InventoryItemLocationStore, ',') AS s 
                  WHERE TRY_CAST(s.value AS INT) = rsd.RackSerialId)
          AND (imm.ItemName LIKE @Search OR imm.ItemCode LIKE @Search)
          AND (@StartDate IS NULL OR CONVERT(datetime, im.InventoryCreatedDateTime, 120) >= @StartDate)
          AND (@EndDate IS NULL OR CONVERT(datetime, im.InventoryCreatedDateTime, 120) <= @EndDate)
          ORDER BY CONVERT(datetime, im.InventoryCreatedDateTime, 120) DESC
          OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";



                using (SqlCommand countCmd = new SqlCommand(countQuery, con))
                using (SqlCommand dataCmd = new SqlCommand(dataQuery, con))
                {
                    string likeSearch = "%" + searchTerm + "%";
                    string companyId = HttpContext.Current.Session["MasterCompanyId"].ToString();

                    // Common parameters
                    countCmd.Parameters.AddWithValue("@Search", likeSearch);
                    countCmd.Parameters.AddWithValue("@CompanyId", companyId);
                    //countCmd.Parameters.AddWithValue("@InventoryCompanyUnitId", companyUnitId);

                    dataCmd.Parameters.AddWithValue("@Search", likeSearch);
                    dataCmd.Parameters.AddWithValue("@CompanyId", companyId);
                    //dataCmd.Parameters.AddWithValue("@InventoryCompanyUnitId", companyUnitId);
                    dataCmd.Parameters.AddWithValue("@Offset", (pageIndex - 1) * pageSize);
                    dataCmd.Parameters.AddWithValue("@PageSize", pageSize);

                    // Date parameters (handle nulls)
                    countCmd.Parameters.AddWithValue("@StartDate", startDate ?? (object)DBNull.Value);
                    countCmd.Parameters.AddWithValue("@EndDate", endDate ?? (object)DBNull.Value);

                    dataCmd.Parameters.AddWithValue("@StartDate", startDate ?? (object)DBNull.Value);
                    dataCmd.Parameters.AddWithValue("@EndDate", endDate ?? (object)DBNull.Value);

                    con.Open();
                    totalRecords = (int)countCmd.ExecuteScalar();

                    using (SqlDataReader sdr = dataCmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new InventoryItemData
                            {
                                InventoryId = Convert.ToInt32(sdr["InventoryId"]),
                                //InventoryCompanyUnitId = Convert.ToInt32(sdr["InventoryCompanyUnitId"]),
                                InventoryRackTypeId = Convert.ToInt32(sdr["InventoryRackTypeId"]),
                                InventoryItemCategoryId = Convert.ToInt32(sdr["InventoryItemCategoryId"]),
                                InventoryItemId = Convert.ToInt32(sdr["InventoryItemId"]),
                                InventoryUOMId = Convert.ToInt32(sdr["InventoryUOMId"]),
                                InventoryGSTId = Convert.ToInt32(sdr["InventoryGSTId"]),
                                InventoryCurrentStock = Convert.ToDecimal(sdr["InventoryCurrentStock"]),
                                InventoryMinStock = Convert.ToDecimal(sdr["InventoryMinStock"]),
                                InventoryCreatedEmployeeId = Convert.ToInt32(sdr["InventoryCreatedEmployeeId"]),
                                InventoryMasterCompanyId = Convert.ToInt32(sdr["InventoryMasterCompanyId"]),
                                InventoryItemLocationStore = sdr["InventoryItemLocationStore"].ToString(),
                                InventoryCreatedDateTime = sdr["InventoryCreatedDateTime"].ToString(),
                                InventoryUpdatedDateTime = sdr["InventoryUpdatedDateTime"].ToString(),

                                ItemID = sdr["ItemID"] != DBNull.Value ? Convert.ToInt32(sdr["ItemID"]) : 0,
                                ItemCode = sdr["ItemCode"] != DBNull.Value ? sdr["ItemCode"].ToString() : string.Empty,
                                ItemName = sdr["ItemName"] != DBNull.Value ? sdr["ItemName"].ToString() : string.Empty,
                                ItemDescription = sdr["ItemDescription"] != DBNull.Value ? sdr["ItemDescription"].ToString() : string.Empty,
                                PurchaseSubGroupID = sdr["PurchaseSubGroupID"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseSubGroupID"]) : 0,
                                LeadTime = sdr["LeadTime"] != DBNull.Value ? Convert.ToInt32(sdr["LeadTime"]) : 0,
                                UOMID = sdr["UOMID"] != DBNull.Value ? Convert.ToInt32(sdr["UOMID"]) : 0,
                                StockQuantity = sdr["StockQuantity"] != DBNull.Value ? Convert.ToDecimal(sdr["StockQuantity"]) : 0,
                                MinStock = sdr["MinStock"] != DBNull.Value ? Convert.ToDecimal(sdr["MinStock"]) : 0,
                                MaxStock = sdr["MaxStock"] != DBNull.Value ? Convert.ToDecimal(sdr["MaxStock"]) : 0,
                                DisabledItem = sdr["DisabledItem"] != DBNull.Value ? Convert.ToBoolean(sdr["DisabledItem"]) : false,
                                HSNCode = sdr["HSNCode"] != DBNull.Value ? sdr["HSNCode"].ToString() : string.Empty,
                                CreatedDate = sdr["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(sdr["CreatedDate"]) : DateTime.MinValue,
                                VAT = sdr["VAT"] != DBNull.Value ? Convert.ToDecimal(sdr["VAT"]) : 0,
                                Remarks = sdr["Remarks"] != DBNull.Value ? sdr["Remarks"].ToString() : string.Empty,
                                UOMValue = sdr["UOMValue"] != DBNull.Value ? Convert.ToDecimal(sdr["UOMValue"]) : 0,
                                ConversionUOMID = sdr["ConversionUOMID"] != DBNull.Value ? Convert.ToInt32(sdr["ConversionUOMID"]) : 0,
                                ConversionUOMValue = sdr["ConversionUOMValue"] != DBNull.Value ? Convert.ToDecimal(sdr["ConversionUOMValue"]) : 0,
                                ModelNo = sdr["ModelNo"] != DBNull.Value ? sdr["ModelNo"].ToString() : string.Empty,
                                Rate = sdr["Rate"] != DBNull.Value ? Convert.ToDecimal(sdr["Rate"]) : 0,
                                ItemTypeID = sdr["ItemTypeID"] != DBNull.Value ? Convert.ToInt32(sdr["ItemTypeID"]) : 0,
                                Description = sdr["Description"] != DBNull.Value ? sdr["Description"].ToString() : string.Empty,
                                VATID = sdr["VATID"] != DBNull.Value ? Convert.ToInt32(sdr["VATID"]) : 0,
                                Specification = sdr["Specification"] != DBNull.Value ? sdr["Specification"].ToString() : string.Empty,
                                PurchaseGroupID = sdr["PurchaseGroupID"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseGroupID"]) : 0,
                                CreatedBy = sdr["CreatedBy"] != DBNull.Value ? Convert.ToInt32(sdr["CreatedBy"]) : 0,
                                ConvertUOM = sdr["ConvertUOM"] != DBNull.Value ? Convert.ToInt32(sdr["ConvertUOM"]) : 0,
                                ItemTypeName = sdr["ItemType"] != DBNull.Value ? sdr["ItemType"].ToString() : string.Empty,

                                UOMId = Convert.ToInt32(sdr["UOMId"]),
                                UOMCode = sdr["UOMName"].ToString(),
                                UOMName = sdr["UOMName"].ToString(),

                                VATCode = sdr["VATCode"].ToString(),
                                Percentage = Convert.ToDecimal(sdr["Percentage"]),

                                RackTypeId = Convert.IsDBNull(sdr["RackTypeId"]) ? 0 : Convert.ToInt32(sdr["RackTypeId"]),
                                RackType = Convert.IsDBNull(sdr["RackType"]) ? string.Empty : sdr["RackType"].ToString(),
                                RackMasterCompanyId = Convert.IsDBNull(sdr["RackMasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["RackMasterCompanyId"]),
                                RackCreatedEmployeeId = Convert.IsDBNull(sdr["RackCreatedEmployeeId"]) ? 0 : Convert.ToInt32(sdr["RackCreatedEmployeeId"]),
                                RackCreatedDateTime = Convert.IsDBNull(sdr["RackCreatedDateTime"]) ? string.Empty : sdr["RackCreatedDateTime"].ToString(),
                                RackUpdatedDateTime = Convert.IsDBNull(sdr["RackUpdatedDateTime"]) ? string.Empty : sdr["RackUpdatedDateTime"].ToString(),
                                RackAlphSeries = Convert.IsDBNull(sdr["RackAlphSeries"]) ? string.Empty : sdr["RackAlphSeries"].ToString(),
                                TotalSerialNumber = Convert.IsDBNull(sdr["TotalSerialNumber"]) ? 0 : Convert.ToInt32(sdr["TotalSerialNumber"]),
                                RackSerialNumber = Convert.IsDBNull(sdr["RackSerialNumber"]) ? string.Empty : sdr["RackSerialNumber"].ToString(),
                                RackSerialId = Convert.IsDBNull(sdr["RackSerialId"]) ? 0 : Convert.ToInt32(sdr["RackSerialId"]),

                                //UnitName = sdr["UnitName"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }

        public List<InventoryItemData> InventoryStockRefill()
        {
            List<InventoryItemData> items = new List<InventoryItemData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * from InventoryMaster ivm
                                  INNER JOIN ItemMaster im ON im.ItemId = ivm.InventoryItemId
                                  INNER JOIN CompanyUnitMaster cum ON cum.CompanyUnitId = ivm.InventoryCompanyUnitId
                                  where InventoryMinStock >= InventoryCurrentStock AND 
                                  InventoryMasterCompanyId = @InventoryMasterCompanyId AND 
                                  InventoryCompanyUnitId = @InventoryCompanyUnitId
                                  ORDER BY ItemName ASC ";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@InventoryMasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    cmd.Parameters.AddWithValue("@InventoryCompanyUnitId", HttpContext.Current.Session["CompanyUnitId"].ToString());

                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new InventoryItemData
                            {
                                InventoryId = Convert.ToInt32(sdr["InventoryId"]),
                                InventoryCompanyUnitId = Convert.ToInt32(sdr["InventoryCompanyUnitId"]),
                                InventoryRackTypeId = Convert.ToInt32(sdr["InventoryRackTypeId"]),
                                InventoryItemCategoryId = Convert.ToInt32(sdr["InventoryItemCategoryId"]),
                                InventoryItemId = Convert.ToInt32(sdr["InventoryItemId"]),
                                InventoryUOMId = Convert.ToInt32(sdr["InventoryUOMId"]),
                                InventoryGSTId = Convert.ToInt32(sdr["InventoryGSTId"]),
                                InventoryCurrentStock = Convert.ToDecimal(sdr["InventoryCurrentStock"]),
                                InventoryMinStock = Convert.ToDecimal(sdr["InventoryMinStock"]),
                                InventoryCreatedEmployeeId = Convert.ToInt32(sdr["InventoryCreatedEmployeeId"]),
                                InventoryMasterCompanyId = Convert.ToInt32(sdr["InventoryMasterCompanyId"]),
                                InventoryItemLocationStore = sdr["InventoryItemLocationStore"].ToString(),
                                InventoryCreatedDateTime = sdr["InventoryCreatedDateTime"].ToString(),
                                InventoryUpdatedDateTime = sdr["InventoryUpdatedDateTime"].ToString(),


                                ItemId = Convert.ToInt32(sdr["ItemId"]),
                                ItemCategoryId = Convert.ToInt32(sdr["ItemCategoryId"]),
                                ItemUOMId = Convert.ToInt32(sdr["ItemUOMId"]),
                                ItemGSTId = Convert.ToInt32(sdr["ItemGSTId"]),
                                ItemCode = sdr["ItemCode"].ToString(),
                                ItemName = sdr["ItemName"].ToString(),
                                ItemDescription = sdr["ItemDescription"].ToString(),
                                ItemHSNCode = sdr["ItemHSNCode"].ToString(),
                                ItemCreatedEmployeeId = Convert.ToInt32(sdr["ItemCreatedEmployeeId"]),
                                ItemMasterCompanyId = Convert.ToInt32(sdr["ItemMasterCompanyId"]),
                                ItemCreatedDateTime = sdr["ItemCreatedDateTime"].ToString(),
                                ItemUpdatedDateTime = sdr["ItemUpdatedDateTime"].ToString(),
                                ItemImageName = sdr["ItemImageName"].ToString(),


                                UnitName = sdr["UnitName"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<ItemTransactionData> ItemTransactionDataGet()
        {
            List<ItemTransactionData> items = new List<ItemTransactionData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM ItemTransactionMaster itr
                                  INNER JOIN InventoryMaster im ON im.InventoryId = itr.IT_InventoryId
                                  INNER JOIN Items itm ON itm.ItemId = itr.IT_ItemId
                                  INNER JOIN VendorDetails vds ON vds.VendorId = itr.IT_VendorId
                                  INNER JOIN UOM um ON um.UOMId = itr.IT_UOMId
                                  INNER JOIN VAT vt on vt.VATID = itr.IT_GSTId
                                  INNER JOIN RackTypeDetails rtd ON rtd.RackTypeId = itr.IT_RackTypeId
                                  WHERE IT_MasterCompanyId = @IT_MasterCompanyId
                                  ORDER BY CONVERT(datetime, itr.IT_CreatedDateTime, 120) DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IT_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    cmd.Parameters.AddWithValue("@IT_CreatedEmployeeId", HttpContext.Current.Session["UserID"].ToString());
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            ItemTransactionData data = new ItemTransactionData
                            {
                                IT_Id = Convert.IsDBNull(sdr["IT_Id"]) ? 0 : Convert.ToInt32(sdr["IT_Id"]),
                                IT_InventoryId = Convert.IsDBNull(sdr["IT_InventoryId"]) ? 0 : Convert.ToInt32(sdr["IT_InventoryId"]),
                                IT_ItemId = Convert.IsDBNull(sdr["IT_ItemId"]) ? 0 : Convert.ToInt32(sdr["IT_ItemId"]),
                                IT_VendorId = Convert.IsDBNull(sdr["IT_VendorId"]) ? 0 : Convert.ToInt32(sdr["IT_VendorId"]),
                                IT_PoNumber = Convert.IsDBNull(sdr["IT_PoNumber"]) ? string.Empty : sdr["IT_PoNumber"].ToString(),
                                IT_UOMId = Convert.IsDBNull(sdr["IT_UOMId"]) ? 0 : Convert.ToInt32(sdr["IT_UOMId"]),
                                IT_GSTId = Convert.IsDBNull(sdr["IT_GSTId"]) ? 0 : Convert.ToInt32(sdr["IT_GSTId"]),
                                IT_RackTypeId = Convert.IsDBNull(sdr["IT_RackTypeId"]) ? 0 : Convert.ToInt32(sdr["IT_RackTypeId"]),
                                IT_BasicPrice = Convert.IsDBNull(sdr["IT_BasicPrice"]) ? 0 : Convert.ToDecimal(sdr["IT_BasicPrice"]),
                                IT_ReceivedQty = Convert.IsDBNull(sdr["IT_ReceivedQty"]) ? 0 : Convert.ToDecimal(sdr["IT_ReceivedQty"]),
                                IT_ShortFall = Convert.IsDBNull(sdr["IT_ReceivedQty"]) ? 0 : Convert.ToDecimal(sdr["IT_ShortFall"]),
                                IT_PreviousShortFall = Convert.IsDBNull(sdr["IT_PreviousShortFall"]) ? 0 : Convert.ToDecimal(sdr["IT_PreviousShortFall"]),
                                IT_ReceivedQtyInvoice = Convert.IsDBNull(sdr["IT_ReceivedQtyInvoice"]) ? 0 : Convert.ToDecimal(sdr["IT_ReceivedQtyInvoice"]),
                                IT_PreviousReceivedQtyInvoice = Convert.IsDBNull(sdr["IT_PreviousReceivedQtyInvoice"]) ? 0 : Convert.ToDecimal(sdr["IT_PreviousReceivedQtyInvoice"]),
                                IT_AcceptedQty = Convert.IsDBNull(sdr["IT_AcceptedQty"]) ? 0 : Convert.ToDecimal(sdr["IT_AcceptedQty"]),
                                IT_RejectedQty = Convert.IsDBNull(sdr["IT_RejectedQty"]) ? 0 : Convert.ToDecimal(sdr["IT_RejectedQty"]),
                                IT_PreviousAcceptedQty = Convert.IsDBNull(sdr["IT_PreviousAcceptedQty"]) ? 0 : Convert.ToDecimal(sdr["IT_PreviousAcceptedQty"]),
                                IT_PreviousRejectedQty = Convert.IsDBNull(sdr["IT_PreviousRejectedQty"]) ? 0 : Convert.ToDecimal(sdr["IT_PreviousRejectedQty"]),
                                IT_PoId = Convert.IsDBNull(sdr["IT_PoId"]) ? 0 : Convert.ToInt32(sdr["IT_PoId"]),
                                IT_OrderedQty = Convert.IsDBNull(sdr["IT_OrderedQty"]) ? 0 : Convert.ToDecimal(sdr["IT_OrderedQty"]),
                                IT_QRCodeNumber = Convert.IsDBNull(sdr["IT_QRCodeNumber"]) ? string.Empty : sdr["IT_QRCodeNumber"].ToString(),
                                IT_QRFileName = Convert.IsDBNull(sdr["IT_QRFileName"]) ? string.Empty : sdr["IT_QRFileName"].ToString(),
                                IT_BalanceQty = Convert.IsDBNull(sdr["IT_BalanceQty"]) ? 0 : Convert.ToDecimal(sdr["IT_BalanceQty"]),
                                IT_TotalAmount = Convert.IsDBNull(sdr["IT_TotalAmount"]) ? 0 : Convert.ToDecimal(sdr["IT_TotalAmount"]),
                                IT_BatchNumber = Convert.IsDBNull(sdr["IT_BatchNumber"]) ? string.Empty : sdr["IT_BatchNumber"].ToString(),
                                IT_ExpiryDateTime = Convert.IsDBNull(sdr["IT_ExpiryDateTime"]) ? string.Empty : sdr["IT_ExpiryDateTime"].ToString(),
                                IT_DeliveryDateTime = Convert.IsDBNull(sdr["IT_DeliveryDateTime"]) ? string.Empty : sdr["IT_DeliveryDateTime"].ToString(),
                                IT_CreatedDateTime = Convert.IsDBNull(sdr["IT_CreatedDateTime"]) ? string.Empty : sdr["IT_CreatedDateTime"].ToString(),
                                IT_UpdatedDateTime = Convert.IsDBNull(sdr["IT_UpdatedDateTime"]) ? string.Empty : sdr["IT_UpdatedDateTime"].ToString(),
                                IT_RackRowSeriesNo = Convert.IsDBNull(sdr["IT_RackRowSeriesNo"]) ? string.Empty : sdr["IT_RackRowSeriesNo"].ToString(),
                                IT_CreatedEmployeeId = Convert.IsDBNull(sdr["IT_CreatedEmployeeId"]) ? 0 : Convert.ToInt32(sdr["IT_CreatedEmployeeId"]),
                                IT_MasterCompanyId = Convert.IsDBNull(sdr["IT_MasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["IT_MasterCompanyId"]),
                                IT_Status = Convert.IsDBNull(sdr["IT_Status"]) ? 0 : Convert.ToInt32(sdr["IT_Status"]),
                                IT_PreviousReceivedQty = Convert.IsDBNull(sdr["IT_PreviousReceivedQty"]) ? 0 : Convert.ToDecimal(sdr["IT_PreviousReceivedQty"]),
                                IT_PreviousBalanceQty = Convert.IsDBNull(sdr["IT_PreviousBalanceQty"]) ? 0 : Convert.ToDecimal(sdr["IT_PreviousBalanceQty"]),
                                IT_IsDelete = Convert.IsDBNull(sdr["IT_IsDelete"]) ? 0 : Convert.ToInt32(sdr["IT_IsDelete"]),

                                ItemCode = sdr["ItemCode"] != DBNull.Value ? sdr["ItemCode"].ToString() : string.Empty,
                                ItemName = sdr["ItemName"] != DBNull.Value ? sdr["ItemName"].ToString() : string.Empty,
                                ItemDescription = sdr["ItemDescription"] != DBNull.Value ? sdr["ItemDescription"].ToString() : string.Empty,
                                PurchaseSubGroupID = sdr["PurchaseSubGroupID"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseSubGroupID"]) : 0,
                                LeadTime = sdr["LeadTime"] != DBNull.Value ? Convert.ToInt32(sdr["LeadTime"]) : 0,
                                UOMID = sdr["UOMID"] != DBNull.Value ? Convert.ToInt32(sdr["UOMID"]) : 0,
                                StockQuantity = sdr["StockQuantity"] != DBNull.Value ? Convert.ToDecimal(sdr["StockQuantity"]) : 0,
                                MinStock = sdr["MinStock"] != DBNull.Value ? Convert.ToDecimal(sdr["MinStock"]) : 0,
                                MaxStock = sdr["MaxStock"] != DBNull.Value ? Convert.ToDecimal(sdr["MaxStock"]) : 0,
                                DisabledItem = sdr["DisabledItem"] != DBNull.Value ? Convert.ToBoolean(sdr["DisabledItem"]) : false,
                                HSNCode = sdr["HSNCode"] != DBNull.Value ? sdr["HSNCode"].ToString() : string.Empty,
                                VAT = sdr["VAT"] != DBNull.Value ? Convert.ToDecimal(sdr["VAT"]) : 0,
                                UOMValue = sdr["UOMValue"] != DBNull.Value ? Convert.ToDecimal(sdr["UOMValue"]) : 0,
                                ConversionUOMID = sdr["ConversionUOMID"] != DBNull.Value ? Convert.ToInt32(sdr["ConversionUOMID"]) : 0,
                                ConversionUOMValue = sdr["ConversionUOMValue"] != DBNull.Value ? Convert.ToDecimal(sdr["ConversionUOMValue"]) : 0,
                                ModelNo = sdr["ModelNo"] != DBNull.Value ? sdr["ModelNo"].ToString() : string.Empty,
                                ItemTypeID = sdr["ItemTypeID"] != DBNull.Value ? Convert.ToInt32(sdr["ItemTypeID"]) : 0,
                                Description = sdr["Description"] != DBNull.Value ? sdr["Description"].ToString() : string.Empty,
                                Specification = sdr["Specification"] != DBNull.Value ? sdr["Specification"].ToString() : string.Empty,
                                PurchaseGroupID = sdr["PurchaseGroupID"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseGroupID"]) : 0,
                                CreatedBy = sdr["CreatedBy"] != DBNull.Value ? Convert.ToInt32(sdr["CreatedBy"]) : 0,
                                ConvertUOM = sdr["ConvertUOM"] != DBNull.Value ? Convert.ToInt32(sdr["ConvertUOM"]) : 0,

                                InventoryId = sdr["InventoryId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryId"]) : 0,
                                InventoryRackTypeId = sdr["InventoryRackTypeId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryRackTypeId"]) : 0,
                                InventoryItemCategoryId = sdr["InventoryItemCategoryId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryItemCategoryId"]) : 0,
                                InventoryItemId = sdr["InventoryItemId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryItemId"]) : 0,
                                InventoryUOMId = sdr["InventoryUOMId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryUOMId"]) : 0,
                                InventoryGSTId = sdr["InventoryGSTId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryGSTId"]) : 0,
                                InventoryCurrentStock = sdr["InventoryCurrentStock"] != DBNull.Value ? Convert.ToDecimal(sdr["InventoryCurrentStock"]) : 0,
                                InventoryMinStock = sdr["InventoryMinStock"] != DBNull.Value ? Convert.ToDecimal(sdr["InventoryMinStock"]) : 0,
                                InventoryCreatedEmployeeId = sdr["InventoryCreatedEmployeeId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryCreatedEmployeeId"]) : 0,
                                InventoryMasterCompanyId = sdr["InventoryMasterCompanyId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryMasterCompanyId"]) : 0,
                                InventoryItemLocationStore = sdr["InventoryItemLocationStore"] != DBNull.Value ? sdr["InventoryItemLocationStore"].ToString() : string.Empty,
                                InventoryCreatedDateTime = sdr["InventoryCreatedDateTime"] != DBNull.Value ? sdr["InventoryCreatedDateTime"].ToString() : string.Empty,
                                InventoryUpdatedDateTime = sdr["InventoryUpdatedDateTime"] != DBNull.Value ? sdr["InventoryUpdatedDateTime"].ToString() : string.Empty,

                                UOMId = sdr["UOMId"] != DBNull.Value ? Convert.ToInt32(sdr["UOMId"]) : 0,
                                UOMCode = sdr["UOMName"] != DBNull.Value ? sdr["UOMName"].ToString() : string.Empty,
                                UOMName = sdr["UOMName"] != DBNull.Value ? sdr["UOMName"].ToString() : string.Empty,

                                VATID = sdr["VATID"] != DBNull.Value ? Convert.ToInt32(sdr["VATID"]) : 0,
                                VATCode = sdr["VATCode"] != DBNull.Value ? sdr["VATCode"].ToString() : string.Empty,
                                Percentage = sdr["Percentage"] != DBNull.Value ? Convert.ToDecimal(sdr["Percentage"]) : 0,
                              
                                RackTypeId = sdr["RackTypeId"] != DBNull.Value ? Convert.ToInt32(sdr["RackTypeId"]) : 0,
                                RackType = sdr["RackType"] != DBNull.Value ? sdr["RackType"].ToString() : string.Empty,
                                RackMasterCompanyId = sdr["RackMasterCompanyId"] != DBNull.Value ? Convert.ToInt32(sdr["RackMasterCompanyId"]) : 0,
                                RackCreatedEmployeeId = sdr["RackCreatedEmployeeId"] != DBNull.Value ? Convert.ToInt32(sdr["RackCreatedEmployeeId"]) : 0,
                                RackCreatedDateTime = sdr["RackCreatedDateTime"] != DBNull.Value ? sdr["RackCreatedDateTime"].ToString() : string.Empty,
                                RackUpdatedDateTime = sdr["RackUpdatedDateTime"] != DBNull.Value ? sdr["RackUpdatedDateTime"].ToString() : string.Empty,

                                VendorCode = sdr["VendorCode"]?.ToString(),
                                VendorName = sdr["VendorName"]?.ToString(),
                                DealerID = sdr["DealerID"] != DBNull.Value ? Convert.ToInt32(sdr["DealerID"]) : (int?)null,
                                Website = sdr["Website"]?.ToString(),
                                YearOfEstablishment = sdr["YearOfEstablishment"]?.ToString(),
                                TypeOfBusiness = sdr["TypeOfBusiness"]?.ToString(),
                                CompanyTradeLicence = sdr["CompanyTradeLicence"]?.ToString(),
                                YearlyTurnover1 = sdr["YearlyTurnover1"] != DBNull.Value ? Convert.ToDecimal(sdr["YearlyTurnover1"]) : (decimal?)null,
                                RequiredCredit = sdr["RequiredCredit"] != DBNull.Value ? Convert.ToDecimal(sdr["RequiredCredit"]) : (decimal?)null,
                                ApprovedCredit = sdr["ApprovedCredit"] != DBNull.Value ? Convert.ToDecimal(sdr["ApprovedCredit"]) : (decimal?)null,
                                PanNumber = sdr["PanNumber"]?.ToString(),
                                AdharNumber = sdr["AdharNumber"]?.ToString(),
                                EstablishmentID = sdr["EstablishmentID"] != DBNull.Value ? Convert.ToInt32(sdr["EstablishmentID"]) : (int?)null,
                                RegistrationNumber = sdr["RegistrationNumber"]?.ToString(),
                                YearlyTurnover2 = sdr["YearlyTurnover2"] != DBNull.Value ? Convert.ToDecimal(sdr["YearlyTurnover2"]) : (decimal?)null,
                                YearlyTurnover3 = sdr["YearlyTurnover3"] != DBNull.Value ? Convert.ToDecimal(sdr["YearlyTurnover3"]) : (decimal?)null,
                                Year1 = sdr["Year1"]?.ToString(),
                                Year2 = sdr["Year2"]?.ToString(),
                                Year3 = sdr["Year3"]?.ToString(),
                                UANNumber = sdr["UANNumber"]?.ToString(),
                                PFNumber = sdr["PFNumber"]?.ToString(),
                                NumberofEmployee = sdr["NumberofEmployee"].ToString(),
                                IEC = sdr["IEC"]?.ToString(),
                                ESIC = sdr["ESIC"]?.ToString(),
                                VendorStatus = sdr["VendorStatus"].ToString(),
                                VendorCurrency = sdr["VendorCurrency"]?.ToString(),
                                CompanySizeID = sdr["CompanySizeID"].ToString(),
                            };
                            items.Add(data);
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<ItemTransactionData> ItemTransactionDataGetPaged(int pageIndex, int pageSize, string searchTerm, out int totalRecords)
        {
            List<ItemTransactionData> items = new List<ItemTransactionData>();
            totalRecords = 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string baseWhere = @"WHERE IT_MasterCompanyId = @IT_MasterCompanyId
                                    AND (
                                        itm.ItemName LIKE @SearchTerm OR 
                                        itm.ItemCode LIKE @SearchTerm OR 
                                        vds.VendorName LIKE @SearchTerm OR
                                        itr.IT_PoNumber LIKE @SearchTerm
                                    )";

                string countQuery = $@"SELECT COUNT(*) 
                                        FROM ItemTransactionMaster itr
                                        INNER JOIN InventoryMaster im ON im.InventoryId = itr.IT_InventoryId
                                        INNER JOIN ItemMaster itm ON itm.ItemId = itr.IT_ItemId
                                        INNER JOIN VendorDetailsMaster vds ON vds.VendorId = itr.IT_VendorId
                                        INNER JOIN UOM um ON um.UOMId = itr.IT_UOMId
                                        INNER JOIN VAT vt on vt.VATID = itr.IT_GSTId
                                        INNER JOIN RackTypeDetails rtd ON rtd.RackTypeId = itr.IT_RackTypeId
                                        INNER JOIN CompanyUnitMaster cum ON cum.CompanyUnitId = itr.IT_CompanyUnitId
                                        {baseWhere}";

                string dataQuery = $@"SELECT * 
                                        FROM ItemTransactionMaster itr
                                        INNER JOIN InventoryMaster im ON im.InventoryId = itr.IT_InventoryId
                                        INNER JOIN ItemMaster itm ON itm.ItemId = itr.IT_ItemId
                                        INNER JOIN VendorDetailsMaster vds ON vds.VendorId = itr.IT_VendorId
                                        INNER JOIN UOM um ON um.UOMId = itr.IT_UOMId
                                        INNER JOIN VAT vt on vt.VATID = itr.IT_GSTId
                                        INNER JOIN RackTypeDetails rtd ON rtd.RackTypeId = itr.IT_RackTypeId
                                        INNER JOIN CompanyUnitMaster cum ON cum.CompanyUnitId = itr.IT_CompanyUnitId
                                        {baseWhere}
                                        ORDER BY CONVERT(datetime, itr.IT_CreatedDateTime, 120) DESC
                                        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                using (SqlCommand countCmd = new SqlCommand(countQuery, con))
                using (SqlCommand dataCmd = new SqlCommand(dataQuery, con))
                {
                    string likeSearch = "%" + searchTerm + "%";
                    string masterCompanyId = HttpContext.Current.Session["MasterCompanyId"].ToString();

                    // Add parameters to both queries
                    foreach (var cmd in new[] { countCmd, dataCmd })
                    {
                        cmd.Parameters.AddWithValue("@IT_MasterCompanyId", masterCompanyId);
                        cmd.Parameters.AddWithValue("@SearchTerm", likeSearch);
                    }

                    dataCmd.Parameters.AddWithValue("@Offset", (pageIndex - 1) * pageSize);
                    dataCmd.Parameters.AddWithValue("@PageSize", pageSize);

                    con.Open();
                    totalRecords = (int)countCmd.ExecuteScalar();

                    using (SqlDataReader sdr = dataCmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            // You can reuse your existing data mapping code here
                            ItemTransactionData data = new ItemTransactionData
                            {
                                IT_Id = Convert.ToInt32(sdr["IT_Id"]),
                                IT_InventoryId = Convert.ToInt32(sdr["IT_InventoryId"]),
                                IT_ItemId = Convert.ToInt32(sdr["IT_ItemId"]),
                                IT_VendorId = Convert.ToInt32(sdr["IT_VendorId"]),
                                IT_PoNumber = sdr["IT_PoNumber"].ToString(),
                                IT_UOMId = Convert.ToInt32(sdr["IT_UOMId"]),
                                IT_GSTId = Convert.ToInt32(sdr["IT_GSTId"]),
                                IT_CompanyUnitId = Convert.ToInt32(sdr["IT_CompanyUnitId"]),
                                IT_RackTypeId = Convert.ToInt32(sdr["IT_RackTypeId"]),
                                IT_BasicPrice = Convert.ToDecimal(sdr["IT_BasicPrice"]),
                                IT_ReceivedQty = Convert.ToDecimal(sdr["IT_ReceivedQty"]),
                                IT_ShortFall = Convert.IsDBNull(sdr["IT_ShortFall"]) ? 0 : Convert.ToDecimal(sdr["IT_ShortFall"]),
                                IT_PreviousShortFall = Convert.IsDBNull(sdr["IT_PreviousShortFall"]) ? 0 : Convert.ToDecimal(sdr["IT_PreviousShortFall"]),
                                IT_ReceivedQtyInvoice = Convert.IsDBNull(sdr["IT_ReceivedQtyInvoice"]) ? 0 : Convert.ToDecimal(sdr["IT_ReceivedQtyInvoice"]),
                                IT_PreviousReceivedQtyInvoice = Convert.IsDBNull(sdr["IT_PreviousReceivedQtyInvoice"]) ? 0 : Convert.ToDecimal(sdr["IT_PreviousReceivedQtyInvoice"]),

                                IT_BalanceQty = Convert.ToDecimal(sdr["IT_BalanceQty"]),
                                IT_TotalAmount = Convert.ToDecimal(sdr["IT_TotalAmount"]),
                                IT_BatchNumber = sdr["IT_BatchNumber"].ToString(),
                                IT_ExpiryDateTime = sdr["IT_ExpiryDateTime"].ToString(),
                                IT_DeliveryDateTime = sdr["IT_DeliveryDateTime"].ToString(),
                                IT_CreatedDateTime = sdr["IT_CreatedDateTime"].ToString(),
                                IT_UpdatedDateTime = sdr["IT_UpdatedDateTime"].ToString(),
                                IT_RackRowSeriesNo = sdr["IT_RackRowSeriesNo"].ToString(),
                                IT_CreatedEmployeeId = Convert.ToInt32(sdr["IT_CreatedEmployeeId"]),
                                IT_MasterCompanyId = Convert.ToInt32(sdr["IT_MasterCompanyId"]),
                                IT_Status = Convert.ToInt32(sdr["IT_Status"]),
                                IT_PreviousReceivedQty = Convert.ToDecimal(sdr["IT_PreviousReceivedQty"]),
                                IT_PreviousBalanceQty = Convert.ToDecimal(sdr["IT_PreviousBalanceQty"]),
                                IT_AcceptedQty = Convert.ToDecimal(sdr["IT_AcceptedQty"]),
                                IT_RejectedQty = Convert.ToDecimal(sdr["IT_RejectedQty"]),
                                IT_PreviousAcceptedQty = Convert.ToDecimal(sdr["IT_PreviousAcceptedQty"]),
                                IT_PreviousRejectedQty = Convert.ToDecimal(sdr["IT_PreviousRejectedQty"]),
                                IT_PoId = Convert.ToInt32(sdr["IT_PoId"]),
                                IT_OrderedQty = Convert.ToDecimal(sdr["IT_OrderedQty"]),
                                IT_QRCodeNumber = sdr["IT_QRCodeNumber"].ToString(),
                                IT_QRFileName = sdr["IT_QRFileName"].ToString(),
                                IT_IsDelete = Convert.IsDBNull(sdr["IT_IsDelete"]) ? 0 : Convert.ToInt32(sdr["IT_IsDelete"]),
                                IT_ProjectID = sdr["IT_ProjectID"].ToString(),
                                IT_IgirNumber = sdr["IT_IgirNumber"].ToString(),

                                IT_ReturnQty = Convert.IsDBNull(sdr["IT_ReturnQty"]) ? 0 : Convert.ToDecimal(sdr["IT_ReturnQty"]),
                                IT_ReplaceQty = Convert.IsDBNull(sdr["IT_ReplaceQty"]) ? 0 : Convert.ToDecimal(sdr["IT_ReplaceQty"]),
                                IT_PreReturnQty = Convert.IsDBNull(sdr["IT_PreReturnQty"]) ? 0 : Convert.ToDecimal(sdr["IT_PreReturnQty"]),
                                IT_PreReplaceQty = Convert.IsDBNull(sdr["IT_PreReplaceQty"]) ? 0 : Convert.ToDecimal(sdr["IT_PreReplaceQty"]),
                                IT_isValid = Convert.IsDBNull(sdr["IT_isValid"]) ? 0 : Convert.ToInt32(sdr["IT_isValid"]),
                                IT_ReturnDateTime = Convert.IsDBNull(sdr["IT_ReturnDateTime"]) ? string.Empty : sdr["IT_ReturnDateTime"].ToString(),
                                IT_ReplaceDateTime = Convert.IsDBNull(sdr["IT_ReplaceDateTime"]) ? string.Empty : sdr["IT_ReplaceDateTime"].ToString(),

                                InventoryId = Convert.ToInt32(sdr["InventoryId"]),
                                InventoryCompanyUnitId = Convert.ToInt32(sdr["InventoryCompanyUnitId"]),
                                InventoryRackTypeId = Convert.ToInt32(sdr["InventoryRackTypeId"]),
                                InventoryItemId = Convert.ToInt32(sdr["InventoryItemId"]),
                                InventoryUOMId = Convert.ToInt32(sdr["InventoryUOMId"]),
                                InventoryGSTId = Convert.ToInt32(sdr["InventoryGSTId"]),
                                InventoryCurrentStock = Convert.ToDecimal(sdr["InventoryCurrentStock"]),
                                InventoryMinStock = Convert.ToDecimal(sdr["InventoryMinStock"]),
                                InventoryCreatedEmployeeId = Convert.ToInt32(sdr["InventoryCreatedEmployeeId"]),
                                InventoryMasterCompanyId = Convert.ToInt32(sdr["InventoryMasterCompanyId"]),
                                InventoryItemLocationStore = sdr["InventoryItemLocationStore"].ToString(),
                                InventoryCreatedDateTime = sdr["InventoryCreatedDateTime"].ToString(),
                                InventoryUpdatedDateTime = sdr["InventoryUpdatedDateTime"].ToString(),


                                VendorId = Convert.ToInt32(sdr["VendorId"]),
                                VendorCode = sdr["VendorCode"].ToString(),
                                VendorName = sdr["VendorName"].ToString(),
                                VendorEmail = sdr["VendorEmail"].ToString(),
                                VendorMobileNumber = sdr["VendorMobileNumber"].ToString(),
                                VendorCompanyName = sdr["VendorCompanyName"].ToString(),
                                VendorCompanyAddress = sdr["VendorCompanyAddress"].ToString(),
                                VendorCompanyGSTNumber = sdr["VendorCompanyGSTNumber"].ToString(),
                                VendorCompanyPANNumber = sdr["VendorCompanyPANNumber"].ToString(),
                                VendorCountry = sdr["VendorCountry"].ToString(),
                                VendorState = sdr["VendorState"].ToString(),
                                VendorCity = sdr["VendorCity"].ToString(),
                                VendorPinCode = sdr["VendorPinCode"].ToString(),
                                VendorBusinessType = sdr["VendorBusinessType"].ToString(),
                                VendorCreatedDateTime = sdr["VendorCreatedDateTime"].ToString(),
                                VendorUpdatedDateTime = sdr["VendorUpdatedDateTime"].ToString(),
                                VendorIsActive = Convert.ToInt32(sdr["VendorIsActive"]),
                                VendorMasterCompanyId = Convert.ToInt32(sdr["VendorMasterCompanyId"]),
                                VendorCreatedEmployeeId = Convert.ToInt32(sdr["VendorCreatedEmployeeId"]),


                                ItemId = Convert.ToInt32(sdr["ItemId"]),
                                ItemCategoryId = Convert.ToInt32(sdr["ItemCategoryId"]),
                                ItemUOMId = Convert.ToInt32(sdr["ItemUOMId"]),
                                ItemGSTId = Convert.ToInt32(sdr["ItemGSTId"]),
                                ItemCode = sdr["ItemCode"].ToString(),
                                ItemName = sdr["ItemName"].ToString(),
                                ItemDescription = sdr["ItemDescription"].ToString(),
                                ItemHSNCode = sdr["ItemHSNCode"].ToString(),
                                ItemCreatedEmployeeId = Convert.ToInt32(sdr["ItemCreatedEmployeeId"]),
                                ItemMasterCompanyId = Convert.ToInt32(sdr["ItemMasterCompanyId"]),
                                ItemCreatedDateTime = sdr["ItemCreatedDateTime"].ToString(),
                                ItemUpdatedDateTime = sdr["ItemUpdatedDateTime"].ToString(),
                                ItemImageName = sdr["ItemImageName"].ToString(),


                                UOMId = Convert.ToInt32(sdr["UOMId"]),
                                UOMCode = sdr["UOMCode"].ToString(),
                                UOMName = sdr["UOMName"].ToString(),

                                VATID = Convert.ToInt32(sdr["GSTId"]),
                                VATCode = sdr["VATCode"].ToString(),
                                Percentage = Convert.ToDecimal(sdr["Percentage"]),

                                RackTypeId = Convert.ToInt32(sdr["RackTypeId"]),
                                RackType = sdr["RackType"].ToString(),
                                RackMasterCompanyId = Convert.ToInt32(sdr["RackMasterCompanyId"]),
                                RackCreatedEmployeeId = Convert.ToInt32(sdr["RackCreatedEmployeeId"]),
                                RackCreatedDateTime = sdr["RackCreatedDateTime"].ToString(),
                                RackUpdatedDateTime = sdr["RackUpdatedDateTime"].ToString(),

                                UnitName = sdr["UnitName"].ToString(),
                            };

                            items.Add(data);
                        }
                    }
                }
            }

            return items;
        }

        public List<ItemTransactionData> ItemTransactionDataGetPaged(int pageIndex, int pageSize, string searchTerm, DateTime? startDate, DateTime? endDate, out int totalRecords)
        {
            List<ItemTransactionData> items = new List<ItemTransactionData>();
            totalRecords = 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string baseWhere = @"WHERE IT_MasterCompanyId = @IT_MasterCompanyId
                            AND (
                                itm.ItemName LIKE @SearchTerm OR 
                                itm.ItemCode LIKE @SearchTerm OR 
                                vds.VendorName LIKE @SearchTerm OR
                                itr.IT_PoNumber LIKE @SearchTerm
                            )";

                // Add date filter if dates are provided
                if (startDate.HasValue)
                {
                    baseWhere += " AND CONVERT(date, itr.IT_CreatedDateTime) >= @StartDate";
                }
                if (endDate.HasValue)
                {
                    baseWhere += " AND CONVERT(date, itr.IT_CreatedDateTime) <= @EndDate";
                }

                string countQuery = $@"SELECT COUNT(*) FROM ItemTransactionMaster itr
                                        INNER JOIN InventoryMaster im ON im.InventoryId = itr.IT_InventoryId
                                        INNER JOIN Items itm ON itm.ItemId = itr.IT_ItemId
                                        INNER JOIN VendorDetails vds ON vds.VendorId = itr.IT_VendorId
                                        INNER JOIN UOM um ON um.UOMId = itr.IT_UOMId
                                        INNER JOIN VAT vt on vt.VATID = itr.IT_GSTId
                                        INNER JOIN RackTypeDetails rtd ON rtd.RackTypeId = itr.IT_RackTypeId
                                        {baseWhere}";

                string dataQuery = $@"SELECT * FROM ItemTransactionMaster itr
                                      INNER JOIN InventoryMaster im ON im.InventoryId = itr.IT_InventoryId
                                      INNER JOIN Items itm ON itm.ItemId = itr.IT_ItemId
                                      INNER JOIN VendorDetails vds ON vds.VendorId = itr.IT_VendorId
                                      INNER JOIN UOM um ON um.UOMId = itr.IT_UOMId
                                      INNER JOIN VAT vt on vt.VATID = itr.IT_GSTId
                                      INNER JOIN RackTypeDetails rtd ON rtd.RackTypeId = itr.IT_RackTypeId
                                     {baseWhere}
                                     ORDER BY CONVERT(datetime, itr.IT_CreatedDateTime, 120) DESC
                                     OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                using (SqlCommand countCmd = new SqlCommand(countQuery, con))
                using (SqlCommand dataCmd = new SqlCommand(dataQuery, con))
                {
                    string likeSearch = "%" + searchTerm + "%";
                    string masterCompanyId = HttpContext.Current.Session["MasterCompanyId"].ToString();

                    // Add parameters to both queries
                    foreach (var cmd in new[] { countCmd, dataCmd })
                    {
                        cmd.Parameters.AddWithValue("@IT_MasterCompanyId", masterCompanyId);
                        cmd.Parameters.AddWithValue("@SearchTerm", likeSearch);

                        if (startDate.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@StartDate", startDate.Value);
                        }
                        if (endDate.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@EndDate", endDate.Value);
                        }
                    }

                    dataCmd.Parameters.AddWithValue("@Offset", (pageIndex - 1) * pageSize);
                    dataCmd.Parameters.AddWithValue("@PageSize", pageSize);

                    con.Open();
                    totalRecords = (int)countCmd.ExecuteScalar();

                    using (SqlDataReader sdr = dataCmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            ItemTransactionData data = new ItemTransactionData
                            {
                                IT_Id = Convert.IsDBNull(sdr["IT_Id"]) ? 0 : Convert.ToInt32(sdr["IT_Id"]),
                                IT_InventoryId = Convert.IsDBNull(sdr["IT_InventoryId"]) ? 0 : Convert.ToInt32(sdr["IT_InventoryId"]),
                                IT_ItemId = Convert.IsDBNull(sdr["IT_ItemId"]) ? 0 : Convert.ToInt32(sdr["IT_ItemId"]),
                                IT_VendorId = Convert.IsDBNull(sdr["IT_VendorId"]) ? 0 : Convert.ToInt32(sdr["IT_VendorId"]),
                                IT_PoNumber = Convert.IsDBNull(sdr["IT_PoNumber"]) ? string.Empty : sdr["IT_PoNumber"].ToString(),
                                IT_UOMId = Convert.IsDBNull(sdr["IT_UOMId"]) ? 0 : Convert.ToInt32(sdr["IT_UOMId"]),
                                IT_GSTId = Convert.IsDBNull(sdr["IT_GSTId"]) ? 0 : Convert.ToInt32(sdr["IT_GSTId"]),
                                IT_RackTypeId = Convert.IsDBNull(sdr["IT_RackTypeId"]) ? 0 : Convert.ToInt32(sdr["IT_RackTypeId"]),
                                IT_BasicPrice = Convert.IsDBNull(sdr["IT_BasicPrice"]) ? 0 : Convert.ToDecimal(sdr["IT_BasicPrice"]),
                                IT_ReceivedQty = Convert.IsDBNull(sdr["IT_ReceivedQty"]) ? 0 : Convert.ToDecimal(sdr["IT_ReceivedQty"]),
                                IT_ShortFall = Convert.IsDBNull(sdr["IT_ReceivedQty"]) ? 0 : Convert.ToDecimal(sdr["IT_ShortFall"]),
                                IT_PreviousShortFall = Convert.IsDBNull(sdr["IT_PreviousShortFall"]) ? 0 : Convert.ToDecimal(sdr["IT_PreviousShortFall"]),
                                IT_ReceivedQtyInvoice = Convert.IsDBNull(sdr["IT_ReceivedQtyInvoice"]) ? 0 : Convert.ToDecimal(sdr["IT_ReceivedQtyInvoice"]),
                                IT_PreviousReceivedQtyInvoice = Convert.IsDBNull(sdr["IT_PreviousReceivedQtyInvoice"]) ? 0 : Convert.ToDecimal(sdr["IT_PreviousReceivedQtyInvoice"]),
                                IT_AcceptedQty = Convert.IsDBNull(sdr["IT_AcceptedQty"]) ? 0 : Convert.ToDecimal(sdr["IT_AcceptedQty"]),
                                IT_RejectedQty = Convert.IsDBNull(sdr["IT_RejectedQty"]) ? 0 : Convert.ToDecimal(sdr["IT_RejectedQty"]),
                                IT_PreviousAcceptedQty = Convert.IsDBNull(sdr["IT_PreviousAcceptedQty"]) ? 0 : Convert.ToDecimal(sdr["IT_PreviousAcceptedQty"]),
                                IT_PreviousRejectedQty = Convert.IsDBNull(sdr["IT_PreviousRejectedQty"]) ? 0 : Convert.ToDecimal(sdr["IT_PreviousRejectedQty"]),
                                IT_PoId = Convert.IsDBNull(sdr["IT_PoId"]) ? 0 : Convert.ToInt32(sdr["IT_PoId"]),
                                IT_OrderedQty = Convert.IsDBNull(sdr["IT_OrderedQty"]) ? 0 : Convert.ToDecimal(sdr["IT_OrderedQty"]),
                                IT_QRCodeNumber = Convert.IsDBNull(sdr["IT_QRCodeNumber"]) ? string.Empty : sdr["IT_QRCodeNumber"].ToString(),
                                IT_QRFileName = Convert.IsDBNull(sdr["IT_QRFileName"]) ? string.Empty : sdr["IT_QRFileName"].ToString(),
                                IT_BalanceQty = Convert.IsDBNull(sdr["IT_BalanceQty"]) ? 0 : Convert.ToDecimal(sdr["IT_BalanceQty"]),
                                IT_TotalAmount = Convert.IsDBNull(sdr["IT_TotalAmount"]) ? 0 : Convert.ToDecimal(sdr["IT_TotalAmount"]),
                                IT_BatchNumber = Convert.IsDBNull(sdr["IT_BatchNumber"]) ? string.Empty : sdr["IT_BatchNumber"].ToString(),
                                IT_ExpiryDateTime = Convert.IsDBNull(sdr["IT_ExpiryDateTime"]) ? string.Empty : sdr["IT_ExpiryDateTime"].ToString(),
                                IT_DeliveryDateTime = Convert.IsDBNull(sdr["IT_DeliveryDateTime"]) ? string.Empty : sdr["IT_DeliveryDateTime"].ToString(),
                                IT_CreatedDateTime = Convert.IsDBNull(sdr["IT_CreatedDateTime"]) ? string.Empty : sdr["IT_CreatedDateTime"].ToString(),
                                IT_UpdatedDateTime = Convert.IsDBNull(sdr["IT_UpdatedDateTime"]) ? string.Empty : sdr["IT_UpdatedDateTime"].ToString(),
                                IT_RackRowSeriesNo = Convert.IsDBNull(sdr["IT_RackRowSeriesNo"]) ? string.Empty : sdr["IT_RackRowSeriesNo"].ToString(),
                                IT_CreatedEmployeeId = Convert.IsDBNull(sdr["IT_CreatedEmployeeId"]) ? 0 : Convert.ToInt32(sdr["IT_CreatedEmployeeId"]),
                                IT_MasterCompanyId = Convert.IsDBNull(sdr["IT_MasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["IT_MasterCompanyId"]),
                                IT_Status = Convert.IsDBNull(sdr["IT_Status"]) ? 0 : Convert.ToInt32(sdr["IT_Status"]),
                                IT_PreviousReceivedQty = Convert.IsDBNull(sdr["IT_PreviousReceivedQty"]) ? 0 : Convert.ToDecimal(sdr["IT_PreviousReceivedQty"]),
                                IT_PreviousBalanceQty = Convert.IsDBNull(sdr["IT_PreviousBalanceQty"]) ? 0 : Convert.ToDecimal(sdr["IT_PreviousBalanceQty"]),
                                IT_IsDelete = Convert.IsDBNull(sdr["IT_IsDelete"]) ? 0 : Convert.ToInt32(sdr["IT_IsDelete"]),

                                ////IT_ReturnQty = Convert.IsDBNull(sdr["IT_ReturnQty"]) ? 0 : Convert.ToDecimal(sdr["IT_ReturnQty"]),
                                ////IT_ReplaceQty = Convert.IsDBNull(sdr["IT_ReplaceQty"]) ? 0 : Convert.ToDecimal(sdr["IT_ReplaceQty"]),
                                ////IT_PreReturnQty = Convert.IsDBNull(sdr["IT_PreReturnQty"]) ? 0 : Convert.ToDecimal(sdr["IT_PreReturnQty"]),
                                ////IT_PreReplaceQty = Convert.IsDBNull(sdr["IT_PreReplaceQty"]) ? 0 : Convert.ToDecimal(sdr["IT_PreReplaceQty"]),
                                ////IT_isValid = Convert.IsDBNull(sdr["IT_isValid"]) ? 0 : Convert.ToInt32(sdr["IT_isValid"]),
                                ////IT_ReturnDateTime = Convert.IsDBNull(sdr["IT_ReturnDateTime"]) ? string.Empty : sdr["IT_ReturnDateTime"].ToString(),
                                ////IT_ReplaceDateTime = Convert.IsDBNull(sdr["IT_ReplaceDateTime"]) ? string.Empty : sdr["IT_ReplaceDateTime"].ToString(),

                                ItemCode = sdr["ItemCode"] != DBNull.Value ? sdr["ItemCode"].ToString() : string.Empty,
                                ItemName = sdr["ItemName"] != DBNull.Value ? sdr["ItemName"].ToString() : string.Empty,
                                ItemDescription = sdr["ItemDescription"] != DBNull.Value ? sdr["ItemDescription"].ToString() : string.Empty,
                                PurchaseSubGroupID = sdr["PurchaseSubGroupID"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseSubGroupID"]) : 0,
                                LeadTime = sdr["LeadTime"] != DBNull.Value ? Convert.ToInt32(sdr["LeadTime"]) : 0,
                                UOMID = sdr["UOMID"] != DBNull.Value ? Convert.ToInt32(sdr["UOMID"]) : 0,
                                StockQuantity = sdr["StockQuantity"] != DBNull.Value ? Convert.ToDecimal(sdr["StockQuantity"]) : 0,
                                MinStock = sdr["MinStock"] != DBNull.Value ? Convert.ToDecimal(sdr["MinStock"]) : 0,
                                MaxStock = sdr["MaxStock"] != DBNull.Value ? Convert.ToDecimal(sdr["MaxStock"]) : 0,
                                DisabledItem = sdr["DisabledItem"] != DBNull.Value ? Convert.ToBoolean(sdr["DisabledItem"]) : false,
                                HSNCode = sdr["HSNCode"] != DBNull.Value ? sdr["HSNCode"].ToString() : string.Empty,
                                VAT = sdr["VAT"] != DBNull.Value ? Convert.ToDecimal(sdr["VAT"]) : 0,
                                UOMValue = sdr["UOMValue"] != DBNull.Value ? Convert.ToDecimal(sdr["UOMValue"]) : 0,
                                ConversionUOMID = sdr["ConversionUOMID"] != DBNull.Value ? Convert.ToInt32(sdr["ConversionUOMID"]) : 0,
                                ConversionUOMValue = sdr["ConversionUOMValue"] != DBNull.Value ? Convert.ToDecimal(sdr["ConversionUOMValue"]) : 0,
                                ModelNo = sdr["ModelNo"] != DBNull.Value ? sdr["ModelNo"].ToString() : string.Empty,
                                ItemTypeID = sdr["ItemTypeID"] != DBNull.Value ? Convert.ToInt32(sdr["ItemTypeID"]) : 0,
                                Description = sdr["Description"] != DBNull.Value ? sdr["Description"].ToString() : string.Empty,
                                Specification = sdr["Specification"] != DBNull.Value ? sdr["Specification"].ToString() : string.Empty,
                                PurchaseGroupID = sdr["PurchaseGroupID"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseGroupID"]) : 0,
                                CreatedBy = sdr["CreatedBy"] != DBNull.Value ? Convert.ToInt32(sdr["CreatedBy"]) : 0,
                                ConvertUOM = sdr["ConvertUOM"] != DBNull.Value ? Convert.ToInt32(sdr["ConvertUOM"]) : 0,
                                //ItemTypeName = sdr["ItemType"] != DBNull.Value ? sdr["ItemType"].ToString() : string.Empty,

                                InventoryId = sdr["InventoryId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryId"]) : 0,
                                InventoryRackTypeId = sdr["InventoryRackTypeId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryRackTypeId"]) : 0,
                                InventoryItemCategoryId = sdr["InventoryItemCategoryId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryItemCategoryId"]) : 0,
                                InventoryItemId = sdr["InventoryItemId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryItemId"]) : 0,
                                InventoryUOMId = sdr["InventoryUOMId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryUOMId"]) : 0,
                                InventoryGSTId = sdr["InventoryGSTId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryGSTId"]) : 0,
                                InventoryCurrentStock = sdr["InventoryCurrentStock"] != DBNull.Value ? Convert.ToDecimal(sdr["InventoryCurrentStock"]) : 0,
                                InventoryMinStock = sdr["InventoryMinStock"] != DBNull.Value ? Convert.ToDecimal(sdr["InventoryMinStock"]) : 0,
                                InventoryCreatedEmployeeId = sdr["InventoryCreatedEmployeeId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryCreatedEmployeeId"]) : 0,
                                InventoryMasterCompanyId = sdr["InventoryMasterCompanyId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryMasterCompanyId"]) : 0,
                                InventoryItemLocationStore = sdr["InventoryItemLocationStore"] != DBNull.Value ? sdr["InventoryItemLocationStore"].ToString() : string.Empty,
                                InventoryCreatedDateTime = sdr["InventoryCreatedDateTime"] != DBNull.Value ? sdr["InventoryCreatedDateTime"].ToString() : string.Empty,
                                InventoryUpdatedDateTime = sdr["InventoryUpdatedDateTime"] != DBNull.Value ? sdr["InventoryUpdatedDateTime"].ToString() : string.Empty,

                                UOMId = sdr["UOMId"] != DBNull.Value ? Convert.ToInt32(sdr["UOMId"]) : 0,
                                UOMCode = sdr["UOMName"] != DBNull.Value ? sdr["UOMName"].ToString() : string.Empty,
                                UOMName = sdr["UOMName"] != DBNull.Value ? sdr["UOMName"].ToString() : string.Empty,

                                VATID = sdr["VATID"] != DBNull.Value ? Convert.ToInt32(sdr["VATID"]) : 0,
                                VATCode = sdr["VATCode"] != DBNull.Value ? sdr["VATCode"].ToString() : string.Empty,
                                Percentage = sdr["Percentage"] != DBNull.Value ? Convert.ToDecimal(sdr["Percentage"]) : 0,

                                RackTypeId = sdr["RackTypeId"] != DBNull.Value ? Convert.ToInt32(sdr["RackTypeId"]) : 0,
                                RackType = sdr["RackType"] != DBNull.Value ? sdr["RackType"].ToString() : string.Empty,
                                RackMasterCompanyId = sdr["RackMasterCompanyId"] != DBNull.Value ? Convert.ToInt32(sdr["RackMasterCompanyId"]) : 0,
                                RackCreatedEmployeeId = sdr["RackCreatedEmployeeId"] != DBNull.Value ? Convert.ToInt32(sdr["RackCreatedEmployeeId"]) : 0,
                                RackCreatedDateTime = sdr["RackCreatedDateTime"] != DBNull.Value ? sdr["RackCreatedDateTime"].ToString() : string.Empty,
                                RackUpdatedDateTime = sdr["RackUpdatedDateTime"] != DBNull.Value ? sdr["RackUpdatedDateTime"].ToString() : string.Empty,


                                VendorCode = sdr["VendorCode"]?.ToString(),
                                VendorName = sdr["VendorName"]?.ToString(),
                                DealerID = sdr["DealerID"] != DBNull.Value ? Convert.ToInt32(sdr["DealerID"]) : (int?)null,
                                Website = sdr["Website"]?.ToString(),
                                YearOfEstablishment = sdr["YearOfEstablishment"]?.ToString(),
                                TypeOfBusiness = sdr["TypeOfBusiness"]?.ToString(),
                                CompanyTradeLicence = sdr["CompanyTradeLicence"]?.ToString(),
                                YearlyTurnover1 = sdr["YearlyTurnover1"] != DBNull.Value ? Convert.ToDecimal(sdr["YearlyTurnover1"]) : (decimal?)null,
                                RequiredCredit = sdr["RequiredCredit"] != DBNull.Value ? Convert.ToDecimal(sdr["RequiredCredit"]) : (decimal?)null,
                                ApprovedCredit = sdr["ApprovedCredit"] != DBNull.Value ? Convert.ToDecimal(sdr["ApprovedCredit"]) : (decimal?)null,
                                PanNumber = sdr["PanNumber"]?.ToString(),
                                AdharNumber = sdr["AdharNumber"]?.ToString(),
                                EstablishmentID = sdr["EstablishmentID"] != DBNull.Value ? Convert.ToInt32(sdr["EstablishmentID"]) : (int?)null,
                                RegistrationNumber = sdr["RegistrationNumber"]?.ToString(),
                                YearlyTurnover2 = sdr["YearlyTurnover2"] != DBNull.Value ? Convert.ToDecimal(sdr["YearlyTurnover2"]) : (decimal?)null,
                                YearlyTurnover3 = sdr["YearlyTurnover3"] != DBNull.Value ? Convert.ToDecimal(sdr["YearlyTurnover3"]) : (decimal?)null,
                                Year1 = sdr["Year1"]?.ToString(),
                                Year2 = sdr["Year2"]?.ToString(),
                                Year3 = sdr["Year3"]?.ToString(),
                                UANNumber = sdr["UANNumber"]?.ToString(),
                                PFNumber = sdr["PFNumber"]?.ToString(),
                                NumberofEmployee = sdr["NumberofEmployee"].ToString(),
                                IEC = sdr["IEC"]?.ToString(),
                                ESIC = sdr["ESIC"]?.ToString(),
                                VendorStatus = sdr["VendorStatus"].ToString(),
                                VendorCurrency = sdr["VendorCurrency"]?.ToString(),
                                CompanySizeID = sdr["CompanySizeID"].ToString(),
                            };

                            items.Add(data);
                        }
                    }
                }
            }

            return items;
        }

        public List<ItemQuarantineDetail> GetQuarantineDetails(int pageIndex, int pageSize, string searchTerm, out int totalRecords, int companyUnitId)
        {
            List<ItemQuarantineDetail> items = new List<ItemQuarantineDetail>();
            totalRecords = 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string countQuery = @"SELECT COUNT(*) 
                                        FROM ItemQuarantineDetails 
                                        WHERE CompanyUnitId = @CompanyUnitId 
                                          AND (
                                               IGIRNumber LIKE @Search OR
                                               ProjectCode LIKE @Search OR
                                               QuarantineItemCode LIKE @Search OR
                                               QuarantineItem LIKE @Search OR
                                               PoNumber LIKE @Search OR
                                               Vendor LIKE @Search
                                          )";

                string dataQuery = @"SELECT * 
                                        FROM ItemQuarantineDetails 
                                        WHERE CompanyUnitId = @CompanyUnitId 
                                          AND (
                                               IGIRNumber LIKE @Search OR
                                               ProjectCode LIKE @Search OR
                                               QuarantineItemCode LIKE @Search OR
                                               QuarantineItem LIKE @Search OR
                                               PoNumber LIKE @Search OR
                                               Vendor LIKE @Search
                                          )
                                        ORDER BY CONVERT(datetime, CreatedDateTime, 120) DESC
                                        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";


                using (SqlCommand countCmd = new SqlCommand(countQuery, con))
                using (SqlCommand dataCmd = new SqlCommand(dataQuery, con))
                {
                    string likeSearch = "%" + searchTerm + "%";

                    foreach (var cmd in new[] { countCmd, dataCmd })
                    {
                        cmd.Parameters.AddWithValue("@Search", likeSearch);
                        cmd.Parameters.AddWithValue("@CompanyUnitId", companyUnitId);
                    }

                    dataCmd.Parameters.AddWithValue("@Offset", (pageIndex - 1) * pageSize);
                    dataCmd.Parameters.AddWithValue("@PageSize", pageSize);

                    con.Open();
                    totalRecords = (int)countCmd.ExecuteScalar();

                    using (SqlDataReader sdr = dataCmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new ItemQuarantineDetail
                            {
                                QuarantineId = Convert.ToInt32(sdr["QuarantineId"]),
                                QuarantineItemID = Convert.ToInt32(sdr["QuarantineItemID"]),
                                QuarantineItemCode = sdr["QuarantineItemCode"].ToString(),
                                QuarantineItem = sdr["QuarantineItem"].ToString(),
                                QuarantineItemBatchNo = sdr["QuarantineItemBatchNo"].ToString(),
                                UOM = Convert.ToInt32(sdr["UOM"]),
                                IGIRNumber = sdr["IGIRNumber"].ToString(),
                                ProjectCode = sdr["ProjectCode"].ToString(),
                                ItemTransactionId = Convert.ToInt32(sdr["ItemTransactionId"]),
                                InventoryId = Convert.ToInt32(sdr["InventoryId"]),
                                PoMasterID = Convert.ToInt32(sdr["PoMasterID"]),
                                UOMName = sdr["UOMName"].ToString(),
                                PoNumber = sdr["PoNumber"].ToString(),
                                VendoreId = Convert.ToInt32(sdr["VendoreId"]),
                                Vendor = sdr["Vendor"].ToString(),
                                ReceivedQty = Convert.ToDecimal(sdr["ReceivedQty"]),
                                AcceptQty = Convert.ToDecimal(sdr["AcceptQty"]),
                                RejectQty = Convert.ToDecimal(sdr["RejectQty"]),
                                MasterCompanyId = Convert.ToInt32(sdr["MasterCompanyId"]),
                                CreatedDateTime = sdr["CreatedDateTime"].ToString()
                            });
                        }
                    }
                }
            }

            return items;
        }

        public List<ItemQuarantineDetail> GetQuarantineDetailsPaged(int pageIndex, int pageSize, int companyUnitId, string searchTerm, DateTime? startDate, DateTime? endDate, out int totalRecords)
        {
            List<ItemQuarantineDetail> items = new List<ItemQuarantineDetail>();
            totalRecords = 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string countQuery = @"SELECT COUNT(*) 
                           FROM ItemQuarantineDetails 
                           WHERE CompanyUnitId = @CompanyUnitId 
                             AND (
                                  IGIRNumber LIKE @Search OR
                                  ProjectCode LIKE @Search OR
                                  QuarantineItemCode LIKE @Search OR
                                  QuarantineItem LIKE @Search OR
                                  PoNumber LIKE @Search OR
                                  Vendor LIKE @Search
                             )
                             AND (@StartDate IS NULL OR CONVERT(datetime, CreatedDateTime, 120) >= @StartDate)
                             AND (@EndDate IS NULL OR CONVERT(datetime, CreatedDateTime, 120) <= @EndDate)";

                string dataQuery = @"SELECT * 
                          FROM ItemQuarantineDetails 
                          WHERE CompanyUnitId = @CompanyUnitId 
                            AND (
                                 IGIRNumber LIKE @Search OR
                                 ProjectCode LIKE @Search OR
                                 QuarantineItemCode LIKE @Search OR
                                 QuarantineItem LIKE @Search OR
                                 PoNumber LIKE @Search OR
                                 Vendor LIKE @Search
                            )
                            AND (@StartDate IS NULL OR CONVERT(datetime, CreatedDateTime, 120) >= @StartDate)
                            AND (@EndDate IS NULL OR CONVERT(datetime, CreatedDateTime, 120) <= @EndDate)
                          ORDER BY CONVERT(datetime, CreatedDateTime, 120) DESC
                          OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                using (SqlCommand countCmd = new SqlCommand(countQuery, con))
                using (SqlCommand dataCmd = new SqlCommand(dataQuery, con))
                {
                    string likeSearch = "%" + searchTerm + "%";

                    foreach (var cmd in new[] { countCmd, dataCmd })
                    {
                        cmd.Parameters.AddWithValue("@Search", likeSearch);
                        cmd.Parameters.AddWithValue("@CompanyUnitId", companyUnitId);
                        cmd.Parameters.AddWithValue("@StartDate", startDate ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@EndDate", endDate ?? (object)DBNull.Value);
                    }

                    dataCmd.Parameters.AddWithValue("@Offset", (pageIndex - 1) * pageSize);
                    dataCmd.Parameters.AddWithValue("@PageSize", pageSize);

                    con.Open();
                    totalRecords = (int)countCmd.ExecuteScalar();

                    using (SqlDataReader sdr = dataCmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new ItemQuarantineDetail
                            {
                                QuarantineId = Convert.ToInt32(sdr["QuarantineId"]),
                                QuarantineItemID = Convert.ToInt32(sdr["QuarantineItemID"]),
                                QuarantineItemCode = sdr["QuarantineItemCode"].ToString(),
                                QuarantineItem = sdr["QuarantineItem"].ToString(),
                                QuarantineItemBatchNo = sdr["QuarantineItemBatchNo"].ToString(),
                                UOM = Convert.ToInt32(sdr["UOM"]),
                                IGIRNumber = sdr["IGIRNumber"].ToString(),
                                ProjectCode = sdr["ProjectCode"].ToString(),
                                ItemTransactionId = Convert.ToInt32(sdr["ItemTransactionId"]),
                                InventoryId = Convert.ToInt32(sdr["InventoryId"]),
                                PoMasterID = Convert.ToInt32(sdr["PoMasterID"]),
                                UOMName = sdr["UOMName"].ToString(),
                                PoNumber = sdr["PoNumber"].ToString(),
                                VendoreId = Convert.ToInt32(sdr["VendoreId"]),
                                Vendor = sdr["Vendor"].ToString(),
                                ReceivedQty = Convert.ToDecimal(sdr["ReceivedQty"]),
                                AcceptQty = Convert.ToDecimal(sdr["AcceptQty"]),
                                RejectQty = Convert.ToDecimal(sdr["RejectQty"]),
                                MasterCompanyId = Convert.ToInt32(sdr["MasterCompanyId"]),
                                CreatedDateTime = sdr["CreatedDateTime"].ToString()
                            });
                        }
                    }
                }
            }

            return items;
        }

        public List<ItemTransactionData> ItemTransactionDataGet(string poNumber, string itemID)
        {
            List<ItemTransactionData> items = new List<ItemTransactionData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"select * from ItemTransactionMaster 
                                 where IT_PoNumber = @IT_PoNumber and IT_ItemId = @IT_ItemId and IT_RejectedQty > 0 and IT_isValid = 1 and IT_Status = 7";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IT_PoNumber", poNumber);
                    cmd.Parameters.AddWithValue("@IT_ItemId", itemID);
                    cmd.Parameters.AddWithValue("@IT_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    cmd.Parameters.AddWithValue("@IT_CreatedEmployeeId", HttpContext.Current.Session["UserID"].ToString());
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new ItemTransactionData
                            {
                                IT_Id = Convert.ToInt32(sdr["IT_Id"]),
                                IT_InventoryId = Convert.ToInt32(sdr["IT_InventoryId"]),
                                IT_ItemId = Convert.ToInt32(sdr["IT_ItemId"]),
                                IT_VendorId = Convert.ToInt32(sdr["IT_VendorId"]),
                                IT_PoNumber = sdr["IT_PoNumber"].ToString(),
                                IT_UOMId = Convert.ToInt32(sdr["IT_UOMId"]),
                                IT_GSTId = Convert.ToInt32(sdr["IT_GSTId"]),
                                IT_CompanyUnitId = Convert.ToInt32(sdr["IT_CompanyUnitId"]),
                                IT_RackTypeId = Convert.ToInt32(sdr["IT_RackTypeId"]),
                                IT_BasicPrice = Convert.ToDecimal(sdr["IT_BasicPrice"]),
                                IT_ReceivedQty = Convert.ToDecimal(sdr["IT_ReceivedQty"]),
                                IT_ShortFall = Convert.IsDBNull(sdr["IT_ShortFall"]) ? 0 : Convert.ToDecimal(sdr["IT_ShortFall"]),
                                IT_PreviousShortFall = Convert.IsDBNull(sdr["IT_PreviousShortFall"]) ? 0 : Convert.ToDecimal(sdr["IT_PreviousShortFall"]),
                                IT_ReceivedQtyInvoice = Convert.IsDBNull(sdr["IT_ReceivedQtyInvoice"]) ? 0 : Convert.ToDecimal(sdr["IT_ReceivedQtyInvoice"]),
                                IT_PreviousReceivedQtyInvoice = Convert.IsDBNull(sdr["IT_PreviousReceivedQtyInvoice"]) ? 0 : Convert.ToDecimal(sdr["IT_PreviousReceivedQtyInvoice"]),

                                IT_BalanceQty = Convert.ToDecimal(sdr["IT_BalanceQty"]),
                                IT_TotalAmount = Convert.ToDecimal(sdr["IT_TotalAmount"]),
                                IT_BatchNumber = sdr["IT_BatchNumber"].ToString(),
                                IT_ExpiryDateTime = sdr["IT_ExpiryDateTime"].ToString(),
                                IT_DeliveryDateTime = sdr["IT_DeliveryDateTime"].ToString(),
                                //IT_CreatedDateTime = sdr["IT_CreatedDateTime"].ToString(),
                                IT_CreatedDateTime = DateTime.Parse(sdr["IT_CreatedDateTime"].ToString()).ToString("dd-MM-yyyy"),
                                IT_UpdatedDateTime = sdr["IT_UpdatedDateTime"].ToString(),
                                IT_RackRowSeriesNo = sdr["IT_RackRowSeriesNo"].ToString(),
                                IT_CreatedEmployeeId = Convert.ToInt32(sdr["IT_CreatedEmployeeId"]),
                                IT_MasterCompanyId = Convert.ToInt32(sdr["IT_MasterCompanyId"]),
                                IT_Status = Convert.ToInt32(sdr["IT_Status"]),
                                IT_PreviousReceivedQty = Convert.ToDecimal(sdr["IT_PreviousReceivedQty"]),
                                IT_PreviousBalanceQty = Convert.ToDecimal(sdr["IT_PreviousBalanceQty"]),
                                IT_AcceptedQty = Convert.ToDecimal(sdr["IT_AcceptedQty"]),
                                IT_RejectedQty = Convert.ToDecimal(sdr["IT_RejectedQty"]),
                                IT_PreviousAcceptedQty = Convert.ToDecimal(sdr["IT_PreviousAcceptedQty"]),
                                IT_PreviousRejectedQty = Convert.ToDecimal(sdr["IT_PreviousRejectedQty"]),
                                IT_PoId = Convert.ToInt32(sdr["IT_PoId"]),
                                IT_OrderedQty = Convert.ToDecimal(sdr["IT_OrderedQty"]),
                                IT_QRCodeNumber = sdr["IT_QRCodeNumber"].ToString(),
                                IT_QRFileName = sdr["IT_QRFileName"].ToString(),
                                IT_IsDelete = Convert.IsDBNull(sdr["IT_IsDelete"]) ? 0 : Convert.ToInt32(sdr["IT_IsDelete"]),
                                IT_ProjectID = sdr["IT_ProjectID"].ToString(),
                                IT_IgirNumber = sdr["IT_IgirNumber"].ToString(),

                                IT_ReturnQty = Convert.IsDBNull(sdr["IT_ReturnQty"]) ? 0 : Convert.ToDecimal(sdr["IT_ReturnQty"]),
                                IT_ReplaceQty = Convert.IsDBNull(sdr["IT_ReplaceQty"]) ? 0 : Convert.ToDecimal(sdr["IT_ReplaceQty"]),
                                IT_PreReturnQty = Convert.IsDBNull(sdr["IT_PreReturnQty"]) ? 0 : Convert.ToDecimal(sdr["IT_PreReturnQty"]),
                                IT_PreReplaceQty = Convert.IsDBNull(sdr["IT_PreReplaceQty"]) ? 0 : Convert.ToDecimal(sdr["IT_PreReplaceQty"]),
                                IT_isValid = Convert.IsDBNull(sdr["IT_isValid"]) ? 0 : Convert.ToInt32(sdr["IT_isValid"]),
                                IT_ReturnDateTime = Convert.IsDBNull(sdr["IT_ReturnDateTime"]) ? string.Empty : sdr["IT_ReturnDateTime"].ToString(),
                                IT_ReplaceDateTime = Convert.IsDBNull(sdr["IT_ReplaceDateTime"]) ? string.Empty : sdr["IT_ReplaceDateTime"].ToString(),

                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<ItemTransactionData> BatchNumberPoWise(string poNumber, string itemID)
        {
            List<ItemTransactionData> items = new List<ItemTransactionData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"select distinct IT_BatchNumber from ItemTransactionMaster where IT_PoNumber = '" + poNumber + "' and IT_ItemId = '" + itemID + "' ";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {

                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new ItemTransactionData
                            {
                                IT_BatchNumber = sdr["IT_BatchNumber"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<ItemTransactionData> ItemTransactionWithRackArrangementInfoGet()
        {
            List<ItemTransactionData> items = new List<ItemTransactionData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT itr.IT_Id,itr.IT_CompanyUnitId, itr.IT_PoNumber, itr.IT_BatchNumber,itr.IT_ExpiryDateTime,um.UOMName, 
                                  itr.IT_CreatedEmployeeId, itr.IT_MasterCompanyId,itr.IT_Status, itm.ItemCode, itm.ItemName,im.InventoryCurrentStock,
                                  im.InventoryMinStock, rtd.RackType, rsd.RackSerialNumber,itr.IT_PoId, itr.IT_InventoryId, itr.IT_ItemId, itr.IT_VendorId, 
                                  itr.IT_UOMId, itr.IT_GSTId, itr.IT_RackTypeId,
                                 SUM(itr.IT_AcceptedQty) TotalItemInRack
                                 FROM ItemTransactionMaster itr
                                 INNER JOIN InventoryMaster im ON im.InventoryId = itr.IT_InventoryId
                                 INNER JOIN ItemMaster itm ON itm.ItemId = itr.IT_ItemId
                                 INNER JOIN VendorDetailsMaster vds ON vds.VendorId = itr.IT_VendorId
                                 INNER JOIN UOM um ON um.UOMId = itr.IT_UOMId
                                 INNER JOIN VAT vt on vt.VATID = itr.IT_GSTId
                                 INNER JOIN RackTypeDetails rtd ON rtd.RackTypeId = itr.IT_RackTypeId
                                 INNER JOIN CompanyUnitMaster cum ON cum.CompanyUnitId = itr.IT_CompanyUnitId
                                 INNER JOIN RackSerialDetails rsd ON CHARINDEX(',' + CAST(rsd.RackSerialId AS VARCHAR) + ',', ',' + itr.IT_RackRowSeriesNo + ',') > 0
                                 WHERE itr.IT_MasterCompanyId = IT_MasterCompanyId 
                                 GROUP BY IT_ItemId,itr.IT_Id,itr.IT_CompanyUnitId,itr.IT_PoNumber, itr.IT_BatchNumber, itr.IT_ExpiryDateTime,um.UOMName,
                                  itr.IT_CreatedEmployeeId, itr.IT_MasterCompanyId, itr.IT_Status,
                                  itm.ItemCode, itm.ItemName,im.InventoryCurrentStock, im.InventoryMinStock, rtd.RackType, rsd.RackSerialNumber,
                                  itr.IT_PoId, itr.IT_InventoryId, itr.IT_ItemId, itr.IT_VendorId, itr.IT_UOMId, itr.IT_GSTId, itr.IT_RackTypeId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IT_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    cmd.Parameters.AddWithValue("@IT_CreatedEmployeeId", HttpContext.Current.Session["UserID"].ToString());
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new ItemTransactionData
                            {
                                IT_Id = Convert.ToInt32(sdr["IT_Id"]),
                                IT_InventoryId = Convert.ToInt32(sdr["IT_InventoryId"]),
                                IT_ItemId = Convert.ToInt32(sdr["IT_ItemId"]),
                                IT_VendorId = Convert.ToInt32(sdr["IT_VendorId"]),
                                IT_PoNumber = sdr["IT_PoNumber"].ToString(),
                                IT_UOMId = Convert.ToInt32(sdr["IT_UOMId"]),
                                IT_GSTId = Convert.ToInt32(sdr["IT_GSTId"]),
                                IT_CompanyUnitId = Convert.ToInt32(sdr["IT_CompanyUnitId"]),
                                IT_RackTypeId = Convert.ToInt32(sdr["IT_RackTypeId"]),

                                //IT_BasicPrice = Convert.ToDecimal(sdr["IT_BasicPrice"]),
                                //IT_ReceivedQty = Convert.ToDecimal(sdr["IT_ReceivedQty"]),
                                //IT_BalanceQty = Convert.ToDecimal(sdr["IT_BalanceQty"]),
                                //IT_TotalAmount = Convert.ToDecimal(sdr["IT_TotalAmount"]),
                                IT_BatchNumber = sdr["IT_BatchNumber"].ToString(),
                                IT_ExpiryDateTime = DateTime.TryParse(sdr["IT_ExpiryDateTime"].ToString(), out var expiryDate) ? expiryDate.ToString("dd-MM-yyyy") : string.Empty,

                                //IT_DeliveryDateTime = sdr["IT_DeliveryDateTime"].ToString(),
                                //IT_CreatedDateTime = sdr["IT_CreatedDateTime"].ToString(),
                                //IT_UpdatedDateTime = sdr["IT_UpdatedDateTime"].ToString(),
                                //IT_RackRowSeriesNo = sdr["IT_RackRowSeriesNo"].ToString(),
                                IT_CreatedEmployeeId = Convert.ToInt32(sdr["IT_CreatedEmployeeId"]),
                                IT_MasterCompanyId = Convert.ToInt32(sdr["IT_MasterCompanyId"]),
                                IT_Status = Convert.ToInt32(sdr["IT_Status"]),
                                //IT_PreviousReceivedQty = Convert.ToDecimal(sdr["IT_PreviousReceivedQty"]),
                                //IT_PreviousBalanceQty = Convert.ToDecimal(sdr["IT_PreviousBalanceQty"]),
                                //IT_AcceptedQty = Convert.ToDecimal(sdr["IT_AcceptedQty"]),
                                //IT_RejectedQty = Convert.ToDecimal(sdr["IT_RejectedQty"]),
                                //IT_PreviousAcceptedQty = Convert.ToDecimal(sdr["IT_PreviousAcceptedQty"]),
                                //IT_PreviousRejectedQty = Convert.ToDecimal(sdr["IT_PreviousRejectedQty"]),
                                //IT_PoId = Convert.ToInt32(sdr["IT_PoId"]),
                                //IT_OrderedQty = Convert.ToDecimal(sdr["IT_OrderedQty"]),                                
                                //IT_IsDelete = Convert.IsDBNull(sdr["IT_IsDelete"]) ? 0 : Convert.ToInt32(sdr["IT_IsDelete"]),


                                TotalItemInRack = Convert.IsDBNull(sdr["TotalItemInRack"]) ? 0 : Convert.ToDecimal(sdr["TotalItemInRack"]),

                                //InventoryId = Convert.ToInt32(sdr["InventoryId"]),
                                ////InventoryBranchId = Convert.ToInt32(sdr["InventoryBranchId"]),
                                //InventoryCompanyUnitId = Convert.ToInt32(sdr["InventoryCompanyUnitId"]),
                                //InventoryRackTypeId = Convert.ToInt32(sdr["InventoryRackTypeId"]),
                                //InventoryItemId = Convert.ToInt32(sdr["InventoryItemId"]),
                                //InventoryVendorId = Convert.ToInt32(sdr["InventoryVendorId"]),
                                //InventoryUOMId = Convert.ToInt32(sdr["InventoryUOMId"]),
                                //InventoryGSTId = Convert.ToInt32(sdr["InventoryGSTId"]),
                                InventoryCurrentStock = Convert.ToDecimal(sdr["InventoryCurrentStock"]),
                                InventoryMinStock = Convert.ToDecimal(sdr["InventoryMinStock"]),
                                //InventoryBasicPrice = Convert.ToDecimal(sdr["InventoryBasicPrice"]),
                                //InventoryIsActive = Convert.ToInt32(sdr["InventoryIsActive"]),
                                //InventoryCreatedEmployeeId = Convert.ToInt32(sdr["InventoryCreatedEmployeeId"]),
                                //InventoryMasterCompanyId = Convert.ToInt32(sdr["InventoryMasterCompanyId"]),
                                //InventoryItemLocationStore = sdr["InventoryItemLocationStore"].ToString(),
                                //InventoryCreatedDateTime = sdr["InventoryCreatedDateTime"].ToString(),
                                //InventoryUpdatedDateTime = sdr["InventoryUpdatedDateTime"].ToString(),

                                // VendorDetailsMaster properties
                                //VendorId = Convert.ToInt32(sdr["VendorId"]),
                                //VendorCode = sdr["VendorCode"].ToString(),
                                //VendorName = sdr["VendorName"].ToString(),
                                //VendorEmail = sdr["VendorEmail"].ToString(),
                                //VendorMobileNumber = sdr["VendorMobileNumber"].ToString(),
                                //VendorCompanyName = sdr["VendorCompanyName"].ToString(),
                                //VendorCompanyAddress = sdr["VendorCompanyAddress"].ToString(),
                                //VendorCompanyGSTNumber = sdr["VendorCompanyGSTNumber"].ToString(),
                                //VendorCompanyPANNumber = sdr["VendorCompanyPANNumber"].ToString(),
                                //VendorCountry = sdr["VendorCountry"].ToString(),
                                //VendorState = sdr["VendorState"].ToString(),
                                //VendorCity = sdr["VendorCity"].ToString(),
                                //VendorPinCode = sdr["VendorPinCode"].ToString(),
                                //VendorBusinessType = sdr["VendorBusinessType"].ToString(),
                                //VendorCreatedDateTime = sdr["VendorCreatedDateTime"].ToString(),
                                //VendorUpdatedDateTime = sdr["VendorUpdatedDateTime"].ToString(),
                                //VendorIsActive = Convert.ToInt32(sdr["VendorIsActive"]),
                                //VendorMasterCompanyId = Convert.ToInt32(sdr["VendorMasterCompanyId"]),
                                //VendorCreatedEmployeeId = Convert.ToInt32(sdr["VendorCreatedEmployeeId"]),


                                //ItemId = Convert.ToInt32(sdr["ItemId"]),
                                //ItemCategoryId = Convert.ToInt32(sdr["ItemCategoryId"]),
                                //ItemVendorId = Convert.ToInt32(sdr["ItemVendorId"]),
                                //ItemUOMId = Convert.ToInt32(sdr["ItemUOMId"]),
                                //ItemGSTId = Convert.ToInt32(sdr["ItemGSTId"]),
                                ItemCode = sdr["ItemCode"].ToString(),
                                ItemName = sdr["ItemName"].ToString(),
                                //ItemDescription = sdr["ItemDescription"].ToString(),
                                //ItemHSNCode = sdr["ItemHSNCode"].ToString(),
                                //ItemCurrentStock = Convert.IsDBNull(sdr["ItemCurrentStock"]) ? 0 : Convert.ToDecimal(sdr["ItemCurrentStock"]),
                                //ItemMinStock = Convert.IsDBNull(sdr["ItemMinStock"]) ? 0 : Convert.ToDecimal(sdr["ItemMinStock"]),
                                //ItemBasicPrice = Convert.IsDBNull(sdr["ItemBasicPrice"]) ? 0 : Convert.ToDecimal(sdr["ItemBasicPrice"]),
                                //ItemIsActive = Convert.ToInt32(sdr["ItemIsActive"]),
                                //ItemCreatedEmployeeId = Convert.ToInt32(sdr["ItemCreatedEmployeeId"]),
                                //ItemMasterCompanyId = Convert.ToInt32(sdr["ItemMasterCompanyId"]),
                                //ItemCreatedDateTime = sdr["ItemCreatedDateTime"].ToString(),
                                //ItemUpdatedDateTime = sdr["ItemUpdatedDateTime"].ToString(),
                                //ItemImageName = sdr["ItemImageName"].ToString(),


                                //UOMId = Convert.ToInt32(sdr["UOMId"]),
                                //UOMCode = sdr["UOMCode"].ToString(),
                                UOMName = sdr["UOMName"].ToString(),

                                //RackTypeId = Convert.ToInt32(sdr["RackTypeId"]),
                                RackType = sdr["RackType"].ToString(),
                                //RackMasterCompanyId = Convert.ToInt32(sdr["RackMasterCompanyId"]),
                                //RackCreatedEmployeeId = Convert.ToInt32(sdr["RackCreatedEmployeeId"]),
                                //RackCreatedDateTime = sdr["RackCreatedDateTime"].ToString(),
                                //RackUpdatedDateTime = sdr["RackUpdatedDateTime"].ToString(),

                                //RackSerialId = Convert.IsDBNull(sdr["RackSerialId"]) ? 0 : Convert.ToInt32(sdr["RackSerialId"]),
                                //TotalSerialNumber = Convert.IsDBNull(sdr["TotalSerialNumber"]) ? 0 : Convert.ToInt32(sdr["TotalSerialNumber"]),
                                //SerialNumberId = Convert.IsDBNull(sdr["SerialNumberId"]) ? 0 : Convert.ToInt32(sdr["SerialNumberId"]),
                                //RackRowId = Convert.IsDBNull(sdr["RackRowId"]) ? 0 : Convert.ToInt32(sdr["RackRowId"]),
                                //RackNumberId = Convert.IsDBNull(sdr["RackNumberId"]) ? 0 : Convert.ToInt32(sdr["RackNumberId"]),
                                //CompanyUnitId = Convert.IsDBNull(sdr["CompanyUnitId"]) ? 0 : Convert.ToInt32(sdr["CompanyUnitId"]),
                                //CreatedEmployeeId = Convert.IsDBNull(sdr["CreatedEmployeeId"]) ? 0 : Convert.ToInt32(sdr["CreatedEmployeeId"]),
                                //MasterCompanyId = Convert.IsDBNull(sdr["MasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["MasterCompanyId"]),
                                //RackAlphSeries = sdr["RackAlphSeries"].ToString(),
                                RackSerialNumber = sdr["RackSerialNumber"].ToString(),


                                //UnitName = sdr["UnitName"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<ItemTransactionData> ItemTransactionWithRackArrangementInfoGetWithoutID()
        {
            List<ItemTransactionData> items = new List<ItemTransactionData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"Select itm.IT_CompanyUnitId,um.UOMName,itm.IT_MasterCompanyId,itm.IT_Status,im.ItemCode,im.ItemName,
		                           ivm.InventoryCurrentStock,ivm.InventoryMinStock,rtd.RackType,rsd.RackSerialNumber,
                                  SUM(its.IS_StoreQty) AS TotalItemInRack
                                  from ItemStore its
                                  Inner Join ItemTransactionMaster itm ON itm.IT_Id = its.IS_ITId
                                  INNER JOIN InventoryMaster ivm ON ivm.InventoryId = itm.IT_InventoryId
                                  INNER JOIN Items im ON im.ItemId = itm.IT_ItemId
                                  INNER JOIN UOM um ON um.UOMId = itm.IT_UOMId
                                  INNER JOIN RackTypeDetails rtd ON rtd.RackTypeId = itm.IT_RackTypeId
                                  Inner Join RackSerialDetails rsd ON rsd.RackSerialId = its.IS_RackSerialNo
                                  WHERE itm.IT_MasterCompanyId = @IT_MasterCompanyId
                                  Group by itm.IT_CompanyUnitId,um.UOMName,itm.IT_MasterCompanyId,itm.IT_Status,im.ItemCode,im.ItemName,
		                           ivm.InventoryCurrentStock,ivm.InventoryMinStock,rtd.RackType,rsd.RackSerialNumber";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IT_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    cmd.Parameters.AddWithValue("@IT_CreatedEmployeeId", HttpContext.Current.Session["UserID"].ToString());
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new ItemTransactionData
                            {
                                IT_CompanyUnitId = Convert.ToInt32(sdr["IT_CompanyUnitId"]),
                                IT_MasterCompanyId = Convert.ToInt32(sdr["IT_MasterCompanyId"]),
                                IT_Status = Convert.ToInt32(sdr["IT_Status"]),
                                TotalItemInRack = Convert.IsDBNull(sdr["TotalItemInRack"]) ? 0 : Convert.ToDecimal(sdr["TotalItemInRack"]),
                                InventoryCurrentStock = Convert.ToDecimal(sdr["InventoryCurrentStock"]),
                                InventoryMinStock = Convert.ToDecimal(sdr["InventoryMinStock"]),
                                ItemCode = sdr["ItemCode"].ToString(),
                                ItemName = sdr["ItemName"].ToString(),
                                UOMName = sdr["UOMName"].ToString(),
                                RackType = sdr["RackType"].ToString(),
                                RackSerialNumber = sdr["RackSerialNumber"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<ItemTransactionData> ItemArrangementShowPaged(int pageIndex, int pageSize, string searchTerm, out int totalRecords, int companyUnitId = 0, string roleId = "")
        {
            List<ItemTransactionData> items = new List<ItemTransactionData>();
            totalRecords = 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string baseWhere = @"WHERE itm.IT_MasterCompanyId = @IT_MasterCompanyId 
                                    AND itm.IT_Status = 15
                                    AND (@SearchTerm = '' OR 
                                        im.ItemCode LIKE '%' + @SearchTerm + '%' OR 
                                        im.ItemName LIKE '%' + @SearchTerm + '%' OR
                                        rtd.RackType LIKE '%' + @SearchTerm + '%' OR
                                        rsd.RackSerialNumber LIKE '%' + @SearchTerm + '%')";

                // Add role-based filtering
                //if (roleId != "1" && roleId != "7") // Not admin or privileged role
                //{
                    baseWhere += " AND itm.IT_CompanyUnitId = @CompanyUnitId";
                //}

                string countQuery = $@"SELECT COUNT(*) FROM (
                                        SELECT im.ItemCode
                                        FROM ItemStore its
                                        INNER JOIN ItemTransactionMaster itm ON itm.IT_Id = its.IS_ITId
                                        INNER JOIN InventoryMaster ivm ON ivm.InventoryId = itm.IT_InventoryId
                                        INNER JOIN Items im ON im.ItemId = itm.IT_ItemId
                                        INNER JOIN UOM um ON um.UOMId = itm.IT_UOMId
                                        INNER JOIN RackTypeDetails rtd ON rtd.RackTypeId = itm.IT_RackTypeId
                                        INNER JOIN RackSerialDetails rsd ON rsd.RackSerialId = its.IS_RackSerialNo
                                        {baseWhere}
                                        GROUP BY itm.IT_CompanyUnitId, um.UOMName, itm.IT_MasterCompanyId, itm.IT_Status, 
                                                 im.ItemCode, im.ItemName, ivm.InventoryCurrentStock, ivm.InventoryMinStock, 
                                                 rtd.RackType, rsd.RackSerialNumber
                                    ) AS GroupedResult
                                    ";


                string dataQuery = $@"SELECT itm.IT_CompanyUnitId, um.UOMName, itm.IT_MasterCompanyId, itm.IT_Status, 
                                        im.ItemCode, im.ItemName, ivm.InventoryCurrentStock, ivm.InventoryMinStock, 
                                        rtd.RackType, rsd.RackSerialNumber, SUM(its.IS_StoreQty) AS TotalItemInRack
                                    FROM ItemStore its
                                    INNER JOIN ItemTransactionMaster itm ON itm.IT_Id = its.IS_ITId
                                    INNER JOIN InventoryMaster ivm ON ivm.InventoryId = itm.IT_InventoryId
                                    INNER JOIN Items im ON im.ItemId = itm.IT_ItemId
                                    INNER JOIN UOM um ON um.UOMId = itm.IT_UOMId
                                    INNER JOIN RackTypeDetails rtd ON rtd.RackTypeId = itm.IT_RackTypeId
                                    INNER JOIN RackSerialDetails rsd ON rsd.RackSerialId = its.IS_RackSerialNo
                                    {baseWhere}
                                    GROUP BY itm.IT_CompanyUnitId, um.UOMName, itm.IT_MasterCompanyId, itm.IT_Status, 
                                        im.ItemCode, im.ItemName, ivm.InventoryCurrentStock, ivm.InventoryMinStock, 
                                        rtd.RackType, rsd.RackSerialNumber
                                    ORDER BY im.ItemCode
                                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                using (SqlCommand countCmd = new SqlCommand(countQuery, con))
                using (SqlCommand dataCmd = new SqlCommand(dataQuery, con))
                {
                    // Common parameters
                    foreach (var cmd in new[] { countCmd, dataCmd })
                    {
                        cmd.Parameters.AddWithValue("@IT_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                        cmd.Parameters.AddWithValue("@SearchTerm", searchTerm ?? "");

                        //if (roleId != "1" && roleId != "7")
                        //{
                            cmd.Parameters.AddWithValue("@CompanyUnitId", companyUnitId);
                        //}
                    }

                    // Paging parameters
                    int offset = Math.Max((pageIndex - 1) * pageSize, 0);
                    dataCmd.Parameters.AddWithValue("@Offset", offset);
                    dataCmd.Parameters.AddWithValue("@PageSize", pageSize);

                    con.Open();
                    totalRecords = (int)countCmd.ExecuteScalar();

                    using (SqlDataReader sdr = dataCmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new ItemTransactionData
                            {
                                IT_CompanyUnitId = Convert.ToInt32(sdr["IT_CompanyUnitId"]),
                                IT_MasterCompanyId = Convert.ToInt32(sdr["IT_MasterCompanyId"]),
                                IT_Status = Convert.ToInt32(sdr["IT_Status"]),
                                TotalItemInRack = Convert.IsDBNull(sdr["TotalItemInRack"]) ? 0 : Convert.ToDecimal(sdr["TotalItemInRack"]),
                                InventoryCurrentStock = Convert.ToDecimal(sdr["InventoryCurrentStock"]),
                                InventoryMinStock = Convert.ToDecimal(sdr["InventoryMinStock"]),
                                ItemCode = sdr["ItemCode"].ToString(),
                                ItemName = sdr["ItemName"].ToString(),
                                UOMName = sdr["UOMName"].ToString(),
                                RackType = sdr["RackType"].ToString(),
                                RackSerialNumber = sdr["RackSerialNumber"].ToString(),
                            });
                        }
                    }
                }
            }
            return items;
        }

        public List<ItemTransactionData> ActiveItemTransactionDataGet()
        {
            List<ItemTransactionData> items = new List<ItemTransactionData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * from ItemTransactionMaster itr
                                 INNER JOIN InventoryMaster im ON im.InventoryId = itr.IT_InventoryId
                                 INNER JOIN ItemMaster itm ON itm.ItemId = itr.IT_ItemId
                                 INNER JOIN VendorDetailsMaster vds ON vds.VendorId = itr.IT_VendorId
                                 INNER JOIN UOM um ON um.UOMId = itr.IT_UOMId
                                 INNER JOIN VAT vt on vt.VATID = itr.IT_GSTId
                                 INNER JOIN RackTypeDetails rtd ON rtd.RackTypeId = itr.IT_RackTypeId
                                 INNER JOIN CompanyUnitMaster cum ON cum.CompanyUnitId = itr.IT_CompanyUnitId
                                 WHERE IT_MasterCompanyId = @IT_MasterCompanyId AND IT_IsDelete = @IT_IsDelete
                                 ORDER BY CONVERT(datetime, itr.IT_CreatedDateTime, 120) DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IT_IsDelete", 0);
                    cmd.Parameters.AddWithValue("@IT_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    cmd.Parameters.AddWithValue("@IT_CreatedEmployeeId", HttpContext.Current.Session["UserID"].ToString());
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new ItemTransactionData
                            {
                                IT_Id = Convert.ToInt32(sdr["IT_Id"]),
                                IT_InventoryId = Convert.ToInt32(sdr["IT_InventoryId"]),
                                IT_ItemId = Convert.ToInt32(sdr["IT_ItemId"]),
                                IT_VendorId = Convert.ToInt32(sdr["IT_VendorId"]),
                                IT_PoNumber = sdr["IT_PoNumber"].ToString(),
                                IT_UOMId = Convert.ToInt32(sdr["IT_UOMId"]),
                                IT_GSTId = Convert.ToInt32(sdr["IT_GSTId"]),
                                IT_CompanyUnitId = Convert.ToInt32(sdr["IT_CompanyUnitId"]),
                                IT_RackTypeId = Convert.ToInt32(sdr["IT_RackTypeId"]),
                                IT_BasicPrice = Convert.ToDecimal(sdr["IT_BasicPrice"]),
                                IT_ReceivedQty = Convert.ToDecimal(sdr["IT_ReceivedQty"]),

                                IT_ShortFall = Convert.IsDBNull(sdr["IT_ShortFall"]) ? 0 : Convert.ToDecimal(sdr["IT_ShortFall"]),
                                IT_PreviousShortFall = Convert.IsDBNull(sdr["IT_PreviousShortFall"]) ? 0 : Convert.ToDecimal(sdr["IT_PreviousShortFall"]),
                                IT_ReceivedQtyInvoice = Convert.IsDBNull(sdr["IT_ReceivedQtyInvoice"]) ? 0 : Convert.ToDecimal(sdr["IT_ReceivedQtyInvoice"]),
                                IT_PreviousReceivedQtyInvoice = Convert.IsDBNull(sdr["IT_PreviousReceivedQtyInvoice"]) ? 0 : Convert.ToDecimal(sdr["IT_PreviousReceivedQtyInvoice"]),

                                IT_BalanceQty = Convert.ToDecimal(sdr["IT_BalanceQty"]),
                                IT_TotalAmount = Convert.ToDecimal(sdr["IT_TotalAmount"]),
                                IT_BatchNumber = sdr["IT_BatchNumber"].ToString(),
                                IT_ExpiryDateTime = sdr["IT_ExpiryDateTime"].ToString(),
                                IT_DeliveryDateTime = sdr["IT_DeliveryDateTime"].ToString(),
                                IT_CreatedDateTime = sdr["IT_CreatedDateTime"].ToString(),
                                IT_UpdatedDateTime = sdr["IT_UpdatedDateTime"].ToString(),
                                IT_RackRowSeriesNo = sdr["IT_RackRowSeriesNo"].ToString(),
                                IT_CreatedEmployeeId = Convert.ToInt32(sdr["IT_CreatedEmployeeId"]),
                                IT_MasterCompanyId = Convert.ToInt32(sdr["IT_MasterCompanyId"]),
                                IT_Status = Convert.ToInt32(sdr["IT_Status"]),
                                IT_PreviousReceivedQty = Convert.ToDecimal(sdr["IT_PreviousReceivedQty"]),
                                IT_PreviousBalanceQty = Convert.ToDecimal(sdr["IT_PreviousBalanceQty"]),
                                IT_AcceptedQty = Convert.ToDecimal(sdr["IT_AcceptedQty"]),
                                IT_RejectedQty = Convert.ToDecimal(sdr["IT_RejectedQty"]),
                                IT_PreviousAcceptedQty = Convert.ToDecimal(sdr["IT_PreviousAcceptedQty"]),
                                IT_PreviousRejectedQty = Convert.ToDecimal(sdr["IT_PreviousRejectedQty"]),
                                IT_PoId = Convert.ToInt32(sdr["IT_PoId"]),
                                IT_OrderedQty = Convert.ToDecimal(sdr["IT_OrderedQty"]),
                                IT_QRCodeNumber = sdr["IT_QRCodeNumber"].ToString(),
                                IT_QRFileName = sdr["IT_QRFileName"].ToString(),
                                //IT_CompanyUnitId = Convert.ToInt32(sdr["IT_CompanyUnitId"]),
                                IT_IsDelete = Convert.IsDBNull(sdr["IT_IsDelete"]) ? 0 : Convert.ToInt32(sdr["IT_IsDelete"]),

                                IT_ReturnQty = Convert.IsDBNull(sdr["IT_ReturnQty"]) ? 0 : Convert.ToDecimal(sdr["IT_ReturnQty"]),
                                IT_ReplaceQty = Convert.IsDBNull(sdr["IT_ReplaceQty"]) ? 0 : Convert.ToDecimal(sdr["IT_ReplaceQty"]),
                                IT_PreReturnQty = Convert.IsDBNull(sdr["IT_PreReturnQty"]) ? 0 : Convert.ToDecimal(sdr["IT_PreReturnQty"]),
                                IT_PreReplaceQty = Convert.IsDBNull(sdr["IT_PreReplaceQty"]) ? 0 : Convert.ToDecimal(sdr["IT_PreReplaceQty"]),
                                IT_isValid = Convert.IsDBNull(sdr["IT_isValid"]) ? 0 : Convert.ToInt32(sdr["IT_isValid"]),
                                IT_ReturnDateTime = Convert.IsDBNull(sdr["IT_ReturnDateTime"]) ? string.Empty : sdr["IT_ReturnDateTime"].ToString(),
                                IT_ReplaceDateTime = Convert.IsDBNull(sdr["IT_ReplaceDateTime"]) ? string.Empty : sdr["IT_ReplaceDateTime"].ToString(),


                                InventoryId = Convert.ToInt32(sdr["InventoryId"]),
                                //InventoryBranchId = Convert.ToInt32(sdr["InventoryBranchId"]),
                                InventoryCompanyUnitId = Convert.ToInt32(sdr["InventoryCompanyUnitId"]),
                                InventoryRackTypeId = Convert.ToInt32(sdr["InventoryRackTypeId"]),
                                InventoryItemId = Convert.ToInt32(sdr["InventoryItemId"]),
                                InventoryUOMId = Convert.ToInt32(sdr["InventoryUOMId"]),
                                InventoryGSTId = Convert.ToInt32(sdr["InventoryGSTId"]),
                                InventoryCurrentStock = Convert.ToDecimal(sdr["InventoryCurrentStock"]),
                                InventoryMinStock = Convert.ToDecimal(sdr["InventoryMinStock"]),
                                InventoryCreatedEmployeeId = Convert.ToInt32(sdr["InventoryCreatedEmployeeId"]),
                                InventoryMasterCompanyId = Convert.ToInt32(sdr["InventoryMasterCompanyId"]),
                                InventoryItemLocationStore = sdr["InventoryItemLocationStore"].ToString(),
                                InventoryCreatedDateTime = sdr["InventoryCreatedDateTime"].ToString(),
                                InventoryUpdatedDateTime = sdr["InventoryUpdatedDateTime"].ToString(),

                                // VendorDetailsMaster properties
                                VendorId = Convert.ToInt32(sdr["VendorId"]),
                                VendorCode = sdr["VendorCode"].ToString(),
                                VendorName = sdr["VendorName"].ToString(),
                                VendorEmail = sdr["VendorEmail"].ToString(),
                                VendorMobileNumber = sdr["VendorMobileNumber"].ToString(),
                                VendorCompanyName = sdr["VendorCompanyName"].ToString(),
                                VendorCompanyAddress = sdr["VendorCompanyAddress"].ToString(),
                                VendorCompanyGSTNumber = sdr["VendorCompanyGSTNumber"].ToString(),
                                VendorCompanyPANNumber = sdr["VendorCompanyPANNumber"].ToString(),
                                VendorCountry = sdr["VendorCountry"].ToString(),
                                VendorState = sdr["VendorState"].ToString(),
                                VendorCity = sdr["VendorCity"].ToString(),
                                VendorPinCode = sdr["VendorPinCode"].ToString(),
                                VendorBusinessType = sdr["VendorBusinessType"].ToString(),
                                VendorCreatedDateTime = sdr["VendorCreatedDateTime"].ToString(),
                                VendorUpdatedDateTime = sdr["VendorUpdatedDateTime"].ToString(),
                                VendorIsActive = Convert.ToInt32(sdr["VendorIsActive"]),
                                VendorMasterCompanyId = Convert.ToInt32(sdr["VendorMasterCompanyId"]),
                                VendorCreatedEmployeeId = Convert.ToInt32(sdr["VendorCreatedEmployeeId"]),


                                ItemId = Convert.ToInt32(sdr["ItemId"]),
                                ItemCategoryId = Convert.ToInt32(sdr["ItemCategoryId"]),
                                ItemUOMId = Convert.ToInt32(sdr["ItemUOMId"]),
                                ItemGSTId = Convert.ToInt32(sdr["ItemGSTId"]),
                                ItemCode = sdr["ItemCode"].ToString(),
                                ItemName = sdr["ItemName"].ToString(),
                                ItemDescription = sdr["ItemDescription"].ToString(),
                                ItemHSNCode = sdr["ItemHSNCode"].ToString(),
                                ItemCreatedEmployeeId = Convert.ToInt32(sdr["ItemCreatedEmployeeId"]),
                                ItemMasterCompanyId = Convert.ToInt32(sdr["ItemMasterCompanyId"]),
                                ItemCreatedDateTime = sdr["ItemCreatedDateTime"].ToString(),
                                ItemUpdatedDateTime = sdr["ItemUpdatedDateTime"].ToString(),
                                ItemImageName = sdr["ItemImageName"].ToString(),


                                UOMId = Convert.ToInt32(sdr["UOMId"]),
                                UOMCode = sdr["UOMCode"].ToString(),
                                UOMName = sdr["UOMName"].ToString(),

                                VATID = Convert.ToInt32(sdr["GSTId"]),
                                VATCode = sdr["VATCode"].ToString(),
                                Percentage = Convert.ToDecimal(sdr["Percentage"]),

                                RackTypeId = Convert.ToInt32(sdr["RackTypeId"]),
                                RackType = sdr["RackType"].ToString(),

                                UnitName = sdr["UnitName"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<GRNMasterData> GRNMasterDataGetPaged(int pageIndex, int pageSize, string searchTerm, out int totalRecords)
        {
            List<GRNMasterData> items = new List<GRNMasterData>();
            totalRecords = 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string masterCompanyId = HttpContext.Current.Session["MasterCompanyId"].ToString();

                // Base query with optional search
                string baseQuery = @"FROM GRNMaster gm
                                    INNER JOIN StatusMasterTB smt ON smt.StatusID = gm.GRN_Status
                                    INNER JOIN MasterEmployeeDetails medt ON medt.EmployeeId = gm.GRN_EmployeeID
                                    INNER JOIN CompanyUnitMaster cum ON cum.CompanyUnitId = gm.GRN_CompanyUnitId 
                                    WHERE gm.GRN_MasterCompanyId = @MasterCompanyId";

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    baseQuery += @" AND (
                                    gm.GRN_NUMBER LIKE @SearchTerm OR 
                                    gm.GRN_PoNumber LIKE @SearchTerm OR
                                    medt.FirstName LIKE @SearchTerm OR 
                                    medt.LastName LIKE @SearchTerm
                                    )";
                }

                // Query for total record count
                string countQuery = "SELECT COUNT(*) " + baseQuery;

                // Query for paginated data
                string dataQuery = @"SELECT gm.*, smt.StatusTitle, medt.FirstName, medt.LastName, cum.UnitName 
                                    " + baseQuery + @"
                                    ORDER BY CONVERT(datetime, gm.GRN_CreatedDateTime, 120) DESC
                                    OFFSET @Offset ROWS
                                    FETCH NEXT @PageSize ROWS ONLY";

                using (SqlCommand countCmd = new SqlCommand(countQuery, con))
                {
                    countCmd.Parameters.AddWithValue("@MasterCompanyId", masterCompanyId);
                    if (!string.IsNullOrEmpty(searchTerm))
                        countCmd.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");

                    con.Open();
                    totalRecords = Convert.ToInt32(countCmd.ExecuteScalar());
                    con.Close();
                }

                using (SqlCommand dataCmd = new SqlCommand(dataQuery, con))
                {
                    dataCmd.Parameters.AddWithValue("@MasterCompanyId", masterCompanyId);
                    dataCmd.Parameters.AddWithValue("@Offset", (pageIndex - 1) * pageSize);
                    dataCmd.Parameters.AddWithValue("@PageSize", pageSize);
                    if (!string.IsNullOrEmpty(searchTerm))
                        dataCmd.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");

                    con.Open();
                    using (SqlDataReader sdr = dataCmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new GRNMasterData
                            {
                                GRN_ID = Convert.ToInt32(sdr["GRN_ID"]),
                                GRN_PoNumber = sdr["GRN_PoNumber"].ToString(),
                                GRN_MasterCompanyId = Convert.ToInt32(sdr["GRN_MasterCompanyId"]),
                                GRN_EmployeeID = Convert.ToInt32(sdr["GRN_EmployeeID"]),
                                GRN_CompanyUnitId = Convert.ToInt32(sdr["GRN_CompanyUnitId"]),
                                GRN_Status = Convert.ToInt32(sdr["GRN_Status"]),
                                GRN_CounterYear = Convert.ToInt32(sdr["GRN_CounterYear"]),
                                GRN_CounterNumber = Convert.ToInt32(sdr["GRN_CounterNumber"]),
                                GRN_NUMBER = sdr["GRN_NUMBER"].ToString(),
                                GRN_CreatedDateTime = sdr["GRN_CreatedDateTime"].ToString(),
                                GRN_UpdatedDateTime = sdr["GRN_UpdatedDateTime"].ToString(),
                                GRN_ApprovedEmployeeID = Convert.IsDBNull(sdr["GRN_ApprovedEmployeeID"]) ? 0 : Convert.ToInt32(sdr["GRN_ApprovedEmployeeID"]),
                                GRN_ApprovedDateTime = sdr["GRN_ApprovedDateTime"].ToString(),

                                GRN_ApprovedAccountantEmployeeID = Convert.IsDBNull(sdr["GRN_ApprovedAccountantEmployeeID"]) ? 0 : Convert.ToInt32(sdr["GRN_ApprovedAccountantEmployeeID"]),
                                GRN_ApprovedDateTimebyAccountant = sdr["GRN_ApprovedDateTimebyAccountant"].ToString(),

                                FirstName = sdr["FirstName"].ToString(),
                                LastName = sdr["LastName"].ToString(),
                                UnitName = sdr["UnitName"].ToString(),
                                StatusTitle = sdr["StatusTitle"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<GRNData> GRNMasterDataGetPaged(int pageIndex, int pageSize, string searchTerm, DateTime? startDate, DateTime? endDate, out int totalRecords)
        {
            List<GRNData> items = new List<GRNData>();
            totalRecords = 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string masterCompanyId = HttpContext.Current.Session["MasterCompanyId"].ToString();

                // Base query with optional search
                string baseQuery = @" FROM TransactionMaster tm
                                    inner join Transactiondetails td on td.InwardID = tm.InwardID
                                    inner join PurchaseOrderDetails pod on pod.PODetailsID = td.PODetailsID
                                    inner join PurchaseOrder po on po.PurchaseOrderID = pod.PurchaseOrderID
                                    inner join vendordetails v on v.vendorid=po.vendorId
                                    INNER JOIN PurchaseRequisitiondetails pd on pd.PurchaseOrderID=po.PurchaseOrderID
                                    inner join PurchaseRequisition pr on pr.PRID=pd.PRID	
                                    inner join Employees e on e.ID = pr.RequestBy
                                    inner join POStatus ps on ps.POStatusID=tm.GRNStatus";

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    baseQuery += @" AND (tm.GRNNUMBER LIKE @SearchTerm OR po.PoNumber LIKE @SearchTerm OR EmployeeName LIKE @SearchTerm)";
                }

                // Add date filters if provided
                if (startDate.HasValue)
                {
                    baseQuery += " AND CONVERT(date, tm.ReceivedDate) >= @StartDate";
                }
                if (endDate.HasValue)
                {
                    baseQuery += " AND CONVERT(date, tm.ReceivedDate) <= @EndDate";
                }

                // Query for total record count
                string countQuery = "SELECT COUNT(*) " + baseQuery;

                // Query for paginated data - NOTE THE ADDED SPACE BEFORE ORDER BY
                string dataQuery = @"SELECT tm.InwardID,GRNNumber,PONumber,EmployeeName as RequestedBy, tm.ReceivedDate,POStatus as GRNStatus,VendorName  " + baseQuery + @" group by tm.InwardID,GRNNumber,PONumber,EmployeeName, tm.ReceivedDate,POStatus,VendorName ORDER BY CONVERT(datetime, tm.receivedDate, 120) DESC
                          OFFSET @Offset ROWS
                          FETCH NEXT @PageSize ROWS ONLY";

                using (SqlCommand countCmd = new SqlCommand(countQuery, con))
                {
                    countCmd.Parameters.AddWithValue("@MasterCompanyId", masterCompanyId);
                    if (!string.IsNullOrEmpty(searchTerm))
                        countCmd.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");
                    if (startDate.HasValue)
                        countCmd.Parameters.AddWithValue("@StartDate", startDate.Value);
                    if (endDate.HasValue)
                        countCmd.Parameters.AddWithValue("@EndDate", endDate.Value);

                    con.Open();
                    totalRecords = Convert.ToInt32(countCmd.ExecuteScalar());
                    con.Close();
                }

                using (SqlCommand dataCmd = new SqlCommand(dataQuery, con))
                {
                    dataCmd.Parameters.AddWithValue("@Offset", (pageIndex - 1) * pageSize);
                    dataCmd.Parameters.AddWithValue("@PageSize", pageSize);
                    if (!string.IsNullOrEmpty(searchTerm))
                        dataCmd.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");
                    if (startDate.HasValue)
                        dataCmd.Parameters.AddWithValue("@StartDate", startDate.Value);
                    if (endDate.HasValue)
                        dataCmd.Parameters.AddWithValue("@EndDate", endDate.Value);

                    con.Open();
                    using (SqlDataReader sdr = dataCmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new GRNData
                            {
                                GRN_ID = Convert.ToInt32(sdr["InwardID"]),
                                GRN_PoNumber = sdr["PoNumber"].ToString(),
                                GRN_Employee = sdr["RequestedBy"].ToString(),
                                GRN_Status = sdr["GRNStatus"].ToString(),
                                GRN_NUMBER = sdr["GRNNUMBER"].ToString(),
                                GRN_CreatedDateTime = sdr["ReceivedDate"].ToString(),
                                VendorName = sdr["VendorName"].ToString()                                
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }
        public List<GRNMasterData> GRNMasterDataShow()
        {
            List<GRNMasterData> items = new List<GRNMasterData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * from TransactionMaster gm
                                 inner join PurchaseOrder po on po.PurchaseOrderID = gm.PurchaseOrderID
                                 INNER JOIN POStatus smt ON smt.POStatusID = gm.GRNStatus
                                 INNER JOIN Users u on u.employeeID = gm.ReceivedBy
                                 INNER JOIN Employees e ON e.ID = u.employeeID
                                 ORDER BY CONVERT(datetime, gm.createddate, 120) DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {                  
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new GRNMasterData
                            {
                                GRN_ID = Convert.ToInt32(sdr["InwardID"]),
                                GRN_PoNumber = sdr["PONumber"].ToString(),
                                GRN_MasterCompanyId = 1,
                                GRN_EmployeeID = Convert.ToInt32(sdr["EmployeeID"]),
                                GRN_CompanyUnitId = 1,
                                GRN_Status = Convert.ToInt32(sdr["GRNStatus"]),
                                GRN_CounterYear = Convert.ToInt32(sdr["GRNYear"]),
                                GRN_CounterNumber = Convert.ToInt32(sdr["GRNCount"]),
                                GRN_NUMBER = sdr["GRNNUMBER"].ToString(),
                                GRN_CreatedDateTime = sdr["createddate"].ToString(),
                                GRN_UpdatedDateTime = sdr["createddate"].ToString(),
                                GRN_ApprovedEmployeeID = Convert.IsDBNull(sdr["ReceivedBy"]) ? 0 : Convert.ToInt32(sdr["ReceivedBy"]),
                                GRN_ApprovedDateTime = sdr["createddate"].ToString(),

                                GRN_ApprovedAccountantEmployeeID = Convert.IsDBNull(sdr["ApprovedBy"]) ? 0 : Convert.ToInt32(sdr["ApprovedBy"]),
                                GRN_ApprovedDateTimebyAccountant = sdr["ApprovedAt"].ToString(),

                                FirstName = sdr["EmployeeName"].ToString(),
                                //LastName = sdr["LastName"].ToString(),
                                //UnitName = sdr["UnitName"].ToString(),
                                StatusTitle = sdr["POStatus"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<GRNMasterData> GRNMasterDataShowForApprovedHod(string GRNNo)
        {
            List<GRNMasterData> items = new List<GRNMasterData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * from TransactionMaster gm
                                    inner join PurchaseOrder po on po.PurchaseOrderID = gm.PurchaseOrderID
                                    INNER JOIN POStatus smt ON smt.POStatusID = gm.GRNStatus
                                    INNER JOIN Users u on u.employeeID = gm.ReceivedBy
                                    INNER JOIN Employees e ON e.ID = u.employeeID
                                    where GRNNumber='"+ GRNNo + "' ORDER BY CONVERT(datetime, gm.createddate, 120) DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    string MasterCompanyId = HttpContext.Current.Session["MasterCompanyId"].ToString();
                    cmd.Parameters.AddWithValue("@GRN_MasterCompanyId", MasterCompanyId);

                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new GRNMasterData
                            {
                                GRN_ID = Convert.ToInt32(sdr["InwardID"]),
                                GRN_PoNumber = sdr["PONumber"].ToString(),
                                GRN_MasterCompanyId = 1,
                                GRN_EmployeeID = Convert.ToInt32(sdr["EmployeeID"]),
                                GRN_CompanyUnitId = 1,
                                GRN_Status = Convert.ToInt32(sdr["GRNStatus"]),
                                GRN_CounterYear = Convert.ToInt32(sdr["GRNYear"]),
                                GRN_CounterNumber = Convert.ToInt32(sdr["GRNCount"]),
                                GRN_NUMBER = sdr["GRNNUMBER"].ToString(),
                                GRN_CreatedDateTime = sdr["createddate"].ToString(),
                                GRN_UpdatedDateTime = sdr["createddate"].ToString(),
                                GRN_ApprovedEmployeeID = Convert.IsDBNull(sdr["ReceivedBy"]) ? 0 : Convert.ToInt32(sdr["ReceivedBy"]),
                                GRN_ApprovedDateTime = sdr["createddate"].ToString(),

                                GRN_ApprovedAccountantEmployeeID = Convert.IsDBNull(sdr["ApprovedBy"]) ? 0 : Convert.ToInt32(sdr["ApprovedBy"]),
                                GRN_ApprovedDateTimebyAccountant = sdr["ApprovedAt"].ToString(),

                                FirstName = sdr["EmployeeName"].ToString(),
                                //LastName = sdr["LastName"].ToString(),
                                //UnitName = sdr["UnitName"].ToString(),
                                StatusTitle = sdr["POStatus"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<GRNMasterData> GRNDetailsShow()
        {
            List<GRNMasterData> items = new List<GRNMasterData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * from GRNMaster gm
                                 INNER JOIN GRNDetails gd ON gd.GRN_ID = gm.GRN_ID
                                 INNER JOIN ItemTransactionMaster itr ON itr.IT_Id = gd.GDITId
                                 INNER JOIN PurchaseOrder po ON po.PONumber = itr.IT_PoNumber 
                                 INNER JOIN PurchaseOrderDetails pod ON pod.PurchaseOrderID = po.PurchaseOrderID
                                 INNER JOIN InventoryMaster im ON im.InventoryId = itr.IT_InventoryId
                                 INNER JOIN Items itm ON itm.ItemId = itr.IT_ItemId
                                 INNER JOIN ItemType it ON it.ItemTypeID = itm.ItemTypeID
                                 INNER JOIN VendorDetails vds ON vds.VendorId = itr.IT_VendorId
                                 INNER JOIN UOM um ON um.UOMId = itr.IT_UOMId
                                 INNER JOIN VAT vt on vt.VATID = itr.IT_GSTId
                                 WHERE GRN_MasterCompanyId = @GRN_MasterCompanyId
                                 ORDER BY CONVERT(datetime, gm.GRN_CreatedDateTime, 120) DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@GRN_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    cmd.Parameters.AddWithValue("@GRN_EmployeeID", HttpContext.Current.Session["UserID"].ToString());
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new GRNMasterData
                            {
                                GRN_ID = sdr["GRN_ID"] != DBNull.Value ? Convert.ToInt32(sdr["GRN_ID"]) : 0,
                                GRN_PoNumber = sdr["GRN_PoNumber"] != DBNull.Value ? sdr["GRN_PoNumber"].ToString() : string.Empty,
                                GRN_MasterCompanyId = sdr["GRN_MasterCompanyId"] != DBNull.Value ? Convert.ToInt32(sdr["GRN_MasterCompanyId"]) : 0,
                                GRN_EmployeeID = sdr["GRN_EmployeeID"] != DBNull.Value ? Convert.ToInt32(sdr["GRN_EmployeeID"]) : 0,
                                GRN_CompanyUnitId = sdr["GRN_CompanyUnitId"] != DBNull.Value ? Convert.ToInt32(sdr["GRN_CompanyUnitId"]) : 0,
                                GRN_Status = sdr["GRN_Status"] != DBNull.Value ? Convert.ToInt32(sdr["GRN_Status"]) : 0,
                                GRN_CounterYear = sdr["GRN_CounterYear"] != DBNull.Value ? Convert.ToInt32(sdr["GRN_CounterYear"]) : 0,
                                GRN_CounterNumber = sdr["GRN_CounterNumber"] != DBNull.Value ? Convert.ToInt32(sdr["GRN_CounterNumber"]) : 0,
                                GRN_NUMBER = sdr["GRN_NUMBER"] != DBNull.Value ? sdr["GRN_NUMBER"].ToString() : string.Empty,
                                GRN_CreatedDateTime = sdr["GRN_CreatedDateTime"] != DBNull.Value ? sdr["GRN_CreatedDateTime"].ToString() : string.Empty,
                                GRN_UpdatedDateTime = sdr["GRN_UpdatedDateTime"] != DBNull.Value ? sdr["GRN_UpdatedDateTime"].ToString() : string.Empty,
                                GRN_ApprovedEmployeeID = sdr["GRN_ApprovedEmployeeID"] != DBNull.Value ? Convert.ToInt32(sdr["GRN_ApprovedEmployeeID"]) : 0,
                                GRN_ApprovedDateTime = sdr["GRN_ApprovedDateTime"] != DBNull.Value ? sdr["GRN_ApprovedDateTime"].ToString() : string.Empty,
                                GRN_ApprovedAccountantEmployeeID = sdr["GRN_ApprovedAccountantEmployeeID"] != DBNull.Value ? Convert.ToInt32(sdr["GRN_ApprovedAccountantEmployeeID"]) : 0,
                                GRN_ApprovedDateTimebyAccountant = sdr["GRN_ApprovedDateTimebyAccountant"] != DBNull.Value ? sdr["GRN_ApprovedDateTimebyAccountant"].ToString() : string.Empty,

                                GDPoId = sdr["GDPoId"] != DBNull.Value ? Convert.ToInt32(sdr["GDPoId"]) : 0,
                                GRNDetailsID = sdr["GRNDetailsID"] != DBNull.Value ? Convert.ToInt32(sdr["GRNDetailsID"]) : 0,
                                GRN = sdr["GRN"] != DBNull.Value ? sdr["GRN"].ToString() : string.Empty,
                                GDPoNumber = sdr["GDPoNumber"] != DBNull.Value ? sdr["GDPoNumber"].ToString() : string.Empty,
                                GDPoItemCode = sdr["GDPoItemCode"] != DBNull.Value ? sdr["GDPoItemCode"].ToString() : string.Empty,
                                GDItemName = sdr["GDItemName"] != DBNull.Value ? sdr["GDItemName"].ToString() : string.Empty,
                                GDITId = sdr["GDITId"] != DBNull.Value ? Convert.ToInt32(sdr["GDITId"]) : 0,
                                GDStatusId = sdr["GDStatusId"] != DBNull.Value ? Convert.ToInt32(sdr["GDStatusId"]) : 0,

                                IT_Id = sdr["IT_Id"] != DBNull.Value ? Convert.ToInt32(sdr["IT_Id"]) : 0,
                                IT_InventoryId = sdr["IT_InventoryId"] != DBNull.Value ? Convert.ToInt32(sdr["IT_InventoryId"]) : 0,
                                IT_ItemId = sdr["IT_ItemId"] != DBNull.Value ? Convert.ToInt32(sdr["IT_ItemId"]) : 0,
                                IT_VendorId = sdr["IT_VendorId"] != DBNull.Value ? Convert.ToInt32(sdr["IT_VendorId"]) : 0,
                                IT_PoNumber = sdr["IT_PoNumber"] != DBNull.Value ? sdr["IT_PoNumber"].ToString() : string.Empty,
                                IT_UOMId = sdr["IT_UOMId"] != DBNull.Value ? Convert.ToInt32(sdr["IT_UOMId"]) : 0,
                                IT_GSTId = sdr["IT_GSTId"] != DBNull.Value ? Convert.ToInt32(sdr["IT_GSTId"]) : 0,
                                IT_RackTypeId = sdr["IT_RackTypeId"] != DBNull.Value ? Convert.ToInt32(sdr["IT_RackTypeId"]) : 0,
                                IT_BasicPrice = sdr["IT_BasicPrice"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_BasicPrice"]) : 0m,
                                IT_ReceivedQty = sdr["IT_ReceivedQty"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_ReceivedQty"]) : 0m,
                                IT_ShortFall = sdr["IT_ShortFall"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_ShortFall"]) : 0m,
                                IT_PreviousShortFall = sdr["IT_PreviousShortFall"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_PreviousShortFall"]) : 0m,
                                IT_ReceivedQtyInvoice = sdr["IT_ReceivedQtyInvoice"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_ReceivedQtyInvoice"]) : 0m,
                                IT_PreviousReceivedQtyInvoice = sdr["IT_PreviousReceivedQtyInvoice"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_PreviousReceivedQtyInvoice"]) : 0m,
                                IT_BalanceQty = sdr["IT_BalanceQty"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_BalanceQty"]) : 0m,
                                IT_TotalAmount = sdr["IT_TotalAmount"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_TotalAmount"]) : 0m,
                                IT_BatchNumber = sdr["IT_BatchNumber"] != DBNull.Value ? sdr["IT_BatchNumber"].ToString() : string.Empty,
                                IT_ExpiryDateTime = sdr["IT_ExpiryDateTime"] != DBNull.Value ? sdr["IT_ExpiryDateTime"].ToString() : string.Empty,
                                IT_DeliveryDateTime = sdr["IT_DeliveryDateTime"] != DBNull.Value ? sdr["IT_DeliveryDateTime"].ToString() : string.Empty,
                                IT_CreatedDateTime = sdr["IT_CreatedDateTime"] != DBNull.Value ? sdr["IT_CreatedDateTime"].ToString() : string.Empty,
                                IT_UpdatedDateTime = sdr["IT_UpdatedDateTime"] != DBNull.Value ? sdr["IT_UpdatedDateTime"].ToString() : string.Empty,
                                IT_RackRowSeriesNo = sdr["IT_RackRowSeriesNo"] != DBNull.Value ? sdr["IT_RackRowSeriesNo"].ToString() : string.Empty,
                                IT_CreatedEmployeeId = sdr["IT_CreatedEmployeeId"] != DBNull.Value ? Convert.ToInt32(sdr["IT_CreatedEmployeeId"]) : 0,
                                IT_MasterCompanyId = sdr["IT_MasterCompanyId"] != DBNull.Value ? Convert.ToInt32(sdr["IT_MasterCompanyId"]) : 0,
                                IT_Status = sdr["IT_Status"] != DBNull.Value ? Convert.ToInt32(sdr["IT_Status"]) : 0,
                                IT_PreviousReceivedQty = sdr["IT_PreviousReceivedQty"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_PreviousReceivedQty"]) : 0m,
                                IT_PreviousBalanceQty = sdr["IT_PreviousBalanceQty"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_PreviousBalanceQty"]) : 0m,
                                IT_AcceptedQty = sdr["IT_AcceptedQty"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_AcceptedQty"]) : 0m,
                                IT_RejectedQty = sdr["IT_RejectedQty"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_RejectedQty"]) : 0m,
                                IT_PreviousAcceptedQty = sdr["IT_PreviousAcceptedQty"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_PreviousAcceptedQty"]) : 0m,
                                IT_PreviousRejectedQty = sdr["IT_PreviousRejectedQty"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_PreviousRejectedQty"]) : 0m,
                                IT_PoId = sdr["IT_PoId"] != DBNull.Value ? Convert.ToInt32(sdr["IT_PoId"]) : 0,
                                IT_OrderedQty = sdr["IT_OrderedQty"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_OrderedQty"]) : 0m,
                                IT_QRCodeNumber = sdr["IT_QRCodeNumber"] != DBNull.Value ? sdr["IT_QRCodeNumber"].ToString() : string.Empty,
                                IT_QRFileName = sdr["IT_QRFileName"] != DBNull.Value ? sdr["IT_QRFileName"].ToString() : string.Empty,
                                IT_ReturnQty = Convert.IsDBNull(sdr["IT_ReturnQty"]) ? 0 : Convert.ToDecimal(sdr["IT_ReturnQty"]),
                                IT_ReplaceQty = Convert.IsDBNull(sdr["IT_ReplaceQty"]) ? 0 : Convert.ToDecimal(sdr["IT_ReplaceQty"]),
                                IT_PreReturnQty = Convert.IsDBNull(sdr["IT_PreReturnQty"]) ? 0 : Convert.ToDecimal(sdr["IT_PreReturnQty"]),
                                IT_PreReplaceQty = Convert.IsDBNull(sdr["IT_PreReplaceQty"]) ? 0 : Convert.ToDecimal(sdr["IT_PreReplaceQty"]),
                                IT_isValid = Convert.IsDBNull(sdr["IT_isValid"]) ? 0 : Convert.ToInt32(sdr["IT_isValid"]),
                                IT_ReturnDateTime = Convert.IsDBNull(sdr["IT_ReturnDateTime"]) ? string.Empty : sdr["IT_ReturnDateTime"].ToString(),
                                IT_ReplaceDateTime = Convert.IsDBNull(sdr["IT_ReplaceDateTime"]) ? string.Empty : sdr["IT_ReplaceDateTime"].ToString(),


                                PurchaseOrderID = sdr["PurchaseOrderID"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseOrderID"]) : 0,
                                PONumber = sdr["PONumber"]?.ToString(),
                                PODate = sdr["PODate"] != DBNull.Value ? Convert.ToDateTime(sdr["PODate"]) : (DateTime?)null,
                                VendorID = sdr["VendorID"] != DBNull.Value ? Convert.ToInt32(sdr["VendorID"]) : (int?)null,
                                Unit = sdr["Unit"]?.ToString(),
                                CreatedDate = sdr["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(sdr["CreatedDate"]) : (DateTime?)null,
                                SpecialInstructions = sdr["SpecialInstructions"]?.ToString(),
                                ReportFileName = sdr["ReportFileName"]?.ToString(),
                                GrandTotal = sdr["GrandTotal"] != DBNull.Value ? Convert.ToDecimal(sdr["GrandTotal"]) : (decimal?)null,
                                UnitID = sdr["UnitID"] != DBNull.Value ? Convert.ToInt32(sdr["UnitID"]) : (int?)null,
                                Delivery = sdr["Delivery"]?.ToString(),
                                Transportation = sdr["Transportation"]?.ToString(),
                                Payment = sdr["Payment"]?.ToString(),
                                Guarantee = sdr["Guarantee"]?.ToString(),
                                Remarks = sdr["Remarks"]?.ToString(),
                                ContactPersonID = sdr["ContactPersonID"] != DBNull.Value ? Convert.ToInt32(sdr["ContactPersonID"]) : (int?)null,
                                PurchaseOrderStatus = sdr["PurchaseOrderStatus"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseOrderStatus"]) : (int?)null,
                                Reason = sdr["Reason"]?.ToString(),
                                POSeries = sdr["POSeries"]?.ToString(),
                                POyear = sdr["POyear"] != DBNull.Value ? Convert.ToInt32(sdr["POyear"]) : (int?)null,
                                StoreType = sdr["StoreType"]?.ToString(),
                                projectid = sdr["projectid"] != DBNull.Value ? Convert.ToInt32(sdr["projectid"]) : (int?)null,
                                penaltyID = sdr["penaltyID"] != DBNull.Value ? Convert.ToInt32(sdr["penaltyID"]) : (int?)null,
                                PODetailsID = sdr["PODetailsID"] != DBNull.Value ? Convert.ToInt32(sdr["PODetailsID"]) : 0,
                                //Discount = sdr["Discount"] != DBNull.Value ? Convert.ToDecimal(sdr["Discount"]) : (decimal?)null,
                                Discount = sdr["Discount"].ToString(),
                                DiscountAmount = sdr["DiscountAmount"] != DBNull.Value ? Convert.ToDecimal(sdr["DiscountAmount"]) : (decimal?)null,
                                Tax = sdr["Tax"] != DBNull.Value ? Convert.ToDecimal(sdr["Tax"]) : (decimal?)null,
                                TotalAmount = sdr["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(sdr["TotalAmount"]) : (decimal?)null,
                                DeliveryDate = sdr["DeliveryDate"] != DBNull.Value ? Convert.ToDateTime(sdr["DeliveryDate"]) : (DateTime?)null,
                                VendorQuotationID = sdr["VendorQuotationID"] != DBNull.Value ? Convert.ToInt32(sdr["VendorQuotationID"]) : (int?)null,
                                Quantity = sdr["Quantity"] != DBNull.Value ? Convert.ToDecimal(sdr["Quantity"]) : (decimal?)null,
                                Rate = sdr["Rate"] != DBNull.Value ? Convert.ToDecimal(sdr["Rate"]) : (decimal?)null,
                                Amount = sdr["Amount"] != DBNull.Value ? Convert.ToDecimal(sdr["Amount"]) : (decimal?)null,
                                ItemID = sdr["ItemID"] != DBNull.Value ? Convert.ToInt32(sdr["ItemID"]) : (int?)null,
                                POStatusID = sdr["POStatusID"] != DBNull.Value ? Convert.ToInt32(sdr["POStatusID"]) : (int?)null,
                                ReceivedQty = sdr["ReceivedQty"] != DBNull.Value ? Convert.ToDecimal(sdr["ReceivedQty"]) : (decimal?)null,
                                TransactionID = sdr["TransactionID"] != DBNull.Value ? Convert.ToInt32(sdr["TransactionID"]) : (int?)null,
                                AllocatedStock = sdr["AllocatedStock"] != DBNull.Value ? Convert.ToDecimal(sdr["AllocatedStock"]) : (decimal?)null,
                                ReceivedBy = sdr["ReceivedBy"] != DBNull.Value ? Convert.ToInt32(sdr["ReceivedBy"]) : (int?)null,
                                SecurityReceived = sdr["SecurityReceived"] != DBNull.Value ? Convert.ToDecimal(sdr["SecurityReceived"]) : (decimal?)null,
                                TaxAmount = sdr["TaxAmount"] != DBNull.Value ? Convert.ToDecimal(sdr["TaxAmount"]) : (decimal?)null,
                                VATID = sdr["VATID"] != DBNull.Value ? Convert.ToInt32(sdr["VATID"]) : (int?)null,
                                VendorQuotationDetailsID = sdr["VendorQuotationDetailsID"] != DBNull.Value ? Convert.ToInt32(sdr["VendorQuotationDetailsID"]) : (int?)null,
                                EnquiryDetailsID = sdr["EnquiryDetailsID"] != DBNull.Value ? Convert.ToInt32(sdr["EnquiryDetailsID"]) : (int?)null,
                                PurchaseComments = sdr["PurchaseComments"]?.ToString(),


                                PO_OrderedQty = sdr["PO_OrderedQty"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_OrderedQty"]) : 0,
                                PO_ReceivedQty = sdr["PO_ReceivedQty"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_ReceivedQty"]) : 0,
                                PO_GettingStock = sdr["PO_GettingStock"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_GettingStock"]) : 0,
                                PO_BalanceQty = sdr["PO_BalanceQty"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_BalanceQty"]) : 0,
                                PO_BasicPrice = sdr["PO_BasicPrice"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_BasicPrice"]) : 0,
                                PO_StatusId = sdr["PO_StatusId"] != DBNull.Value ? Convert.ToInt32(sdr["PO_StatusId"]) : 0,
                                PO_IsActive = sdr["PO_IsActive"] != DBNull.Value ? Convert.ToInt32(sdr["PO_IsActive"]) : 0,
                                PO_AcceptQty = sdr["PO_AcceptQty"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_AcceptQty"]) : 0,
                                PO_RejectQty = sdr["PO_RejectQty"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_RejectQty"]) : 0,
                                PO_StatusForIT = sdr["PO_StatusForIT"] != DBNull.Value ? Convert.ToInt32(sdr["PO_StatusForIT"]) : 0,
                                PO_ShortFall = sdr["PO_ShortFall"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_ShortFall"]) : 0,
                                PO_ReturnQty = sdr["PO_ReturnQty"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_ReturnQty"]) : 0,
                                PO_IsNotificationSend = Convert.IsDBNull(sdr["PO_IsNotificationSend"]) ? 0 : Convert.ToInt32(sdr["PO_IsNotificationSend"]),


                                ItemCode = sdr["ItemCode"] != DBNull.Value ? sdr["ItemCode"].ToString() : string.Empty,
                                ItemName = sdr["ItemName"] != DBNull.Value ? sdr["ItemName"].ToString() : string.Empty,
                                ItemDescription = sdr["ItemDescription"] != DBNull.Value ? sdr["ItemDescription"].ToString() : string.Empty,
                                PurchaseSubGroupID = sdr["PurchaseSubGroupID"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseSubGroupID"]) : 0,
                                LeadTime = sdr["LeadTime"] != DBNull.Value ? Convert.ToInt32(sdr["LeadTime"]) : 0,
                                UOMID = sdr["UOMID"] != DBNull.Value ? Convert.ToInt32(sdr["UOMID"]) : 0,
                                StockQuantity = sdr["StockQuantity"] != DBNull.Value ? Convert.ToDecimal(sdr["StockQuantity"]) : 0,
                                MinStock = sdr["MinStock"] != DBNull.Value ? Convert.ToDecimal(sdr["MinStock"]) : 0,
                                MaxStock = sdr["MaxStock"] != DBNull.Value ? Convert.ToDecimal(sdr["MaxStock"]) : 0,
                                DisabledItem = sdr["DisabledItem"] != DBNull.Value ? Convert.ToBoolean(sdr["DisabledItem"]) : false,
                                HSNCode = sdr["HSNCode"] != DBNull.Value ? sdr["HSNCode"].ToString() : string.Empty,
                                VAT = sdr["VAT"] != DBNull.Value ? Convert.ToDecimal(sdr["VAT"]) : 0,
                                UOMValue = sdr["UOMValue"] != DBNull.Value ? Convert.ToDecimal(sdr["UOMValue"]) : 0,
                                ConversionUOMID = sdr["ConversionUOMID"] != DBNull.Value ? Convert.ToInt32(sdr["ConversionUOMID"]) : 0,
                                ConversionUOMValue = sdr["ConversionUOMValue"] != DBNull.Value ? Convert.ToDecimal(sdr["ConversionUOMValue"]) : 0,
                                ModelNo = sdr["ModelNo"] != DBNull.Value ? sdr["ModelNo"].ToString() : string.Empty,
                                ItemTypeID = sdr["ItemTypeID"] != DBNull.Value ? Convert.ToInt32(sdr["ItemTypeID"]) : 0,
                                Description = sdr["Description"] != DBNull.Value ? sdr["Description"].ToString() : string.Empty,
                                Specification = sdr["Specification"] != DBNull.Value ? sdr["Specification"].ToString() : string.Empty,
                                PurchaseGroupID = sdr["PurchaseGroupID"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseGroupID"]) : 0,
                                CreatedBy = sdr["CreatedBy"] != DBNull.Value ? Convert.ToInt32(sdr["CreatedBy"]) : 0,
                                ConvertUOM = sdr["ConvertUOM"] != DBNull.Value ? Convert.ToInt32(sdr["ConvertUOM"]) : 0,
                                ItemType = sdr["ItemType"] != DBNull.Value ? sdr["ItemType"].ToString() : string.Empty,

                                InventoryId = sdr["InventoryId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryId"]) : 0,
                                InventoryRackTypeId = sdr["InventoryRackTypeId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryRackTypeId"]) : 0,
                                InventoryItemCategoryId = sdr["InventoryItemCategoryId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryItemCategoryId"]) : 0,
                                InventoryItemId = sdr["InventoryItemId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryItemId"]) : 0,
                                InventoryUOMId = sdr["InventoryUOMId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryUOMId"]) : 0,
                                InventoryGSTId = sdr["InventoryGSTId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryGSTId"]) : 0,
                                InventoryCurrentStock = sdr["InventoryCurrentStock"] != DBNull.Value ? Convert.ToDecimal(sdr["InventoryCurrentStock"]) : 0,
                                InventoryMinStock = sdr["InventoryMinStock"] != DBNull.Value ? Convert.ToDecimal(sdr["InventoryMinStock"]) : 0,
                                InventoryCreatedEmployeeId = sdr["InventoryCreatedEmployeeId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryCreatedEmployeeId"]) : 0,
                                InventoryMasterCompanyId = sdr["InventoryMasterCompanyId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryMasterCompanyId"]) : 0,
                                InventoryItemLocationStore = sdr["InventoryItemLocationStore"] != DBNull.Value ? sdr["InventoryItemLocationStore"].ToString() : string.Empty,
                                InventoryCreatedDateTime = sdr["InventoryCreatedDateTime"] != DBNull.Value ? sdr["InventoryCreatedDateTime"].ToString() : string.Empty,
                                InventoryUpdatedDateTime = sdr["InventoryUpdatedDateTime"] != DBNull.Value ? sdr["InventoryUpdatedDateTime"].ToString() : string.Empty,

                                UOMId = sdr["UOMId"] != DBNull.Value ? Convert.ToInt32(sdr["UOMId"]) : 0,
                                UOMCode = sdr["UOMName"] != DBNull.Value ? sdr["UOMName"].ToString() : string.Empty,
                                UOMName = sdr["UOMName"] != DBNull.Value ? sdr["UOMName"].ToString() : string.Empty,

                                VATCode = sdr["VATCode"] != DBNull.Value ? sdr["VATCode"].ToString() : string.Empty,
                                Percentage = sdr["Percentage"] != DBNull.Value ? Convert.ToDecimal(sdr["Percentage"]) : 0,

                                VendorCode = sdr["VendorCode"]?.ToString(),
                                VendorName = sdr["VendorName"]?.ToString(),
                                DealerID = sdr["DealerID"] != DBNull.Value ? Convert.ToInt32(sdr["DealerID"]) : (int?)null,
                                Website = sdr["Website"]?.ToString(),
                                YearOfEstablishment = sdr["YearOfEstablishment"]?.ToString(),
                                TypeOfBusiness = sdr["TypeOfBusiness"]?.ToString(),
                                CompanyTradeLicence = sdr["CompanyTradeLicence"]?.ToString(),
                                YearlyTurnover1 = sdr["YearlyTurnover1"] != DBNull.Value ? Convert.ToDecimal(sdr["YearlyTurnover1"]) : (decimal?)null,
                                RequiredCredit = sdr["RequiredCredit"] != DBNull.Value ? Convert.ToDecimal(sdr["RequiredCredit"]) : (decimal?)null,
                                ApprovedCredit = sdr["ApprovedCredit"] != DBNull.Value ? Convert.ToDecimal(sdr["ApprovedCredit"]) : (decimal?)null,
                                PanNumber = sdr["PanNumber"]?.ToString(),
                                AdharNumber = sdr["AdharNumber"]?.ToString(),
                                EstablishmentID = sdr["EstablishmentID"] != DBNull.Value ? Convert.ToInt32(sdr["EstablishmentID"]) : (int?)null,
                                RegistrationNumber = sdr["RegistrationNumber"]?.ToString(),
                                YearlyTurnover2 = sdr["YearlyTurnover2"] != DBNull.Value ? Convert.ToDecimal(sdr["YearlyTurnover2"]) : (decimal?)null,
                                YearlyTurnover3 = sdr["YearlyTurnover3"] != DBNull.Value ? Convert.ToDecimal(sdr["YearlyTurnover3"]) : (decimal?)null,
                                Year1 = sdr["Year1"]?.ToString(),
                                Year2 = sdr["Year2"]?.ToString(),
                                Year3 = sdr["Year3"]?.ToString(),
                                UANNumber = sdr["UANNumber"]?.ToString(),
                                PFNumber = sdr["PFNumber"]?.ToString(),
                                NumberofEmployee = sdr["NumberofEmployee"].ToString(),
                                IEC = sdr["IEC"]?.ToString(),
                                ESIC = sdr["ESIC"]?.ToString(),
                                VendorStatus = sdr["VendorStatus"].ToString(),
                                VendorCurrency = sdr["VendorCurrency"]?.ToString(),
                                CompanySizeID = sdr["CompanySizeID"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<GRNMasterData> GRNDetailsShow(string GRNNumber)
        {
            List<GRNMasterData> items = new List<GRNMasterData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * from TransactionMaster gm
                                 INNER JOIN Transactiondetails gd ON gd.InwardID = gm.InwardID
                                 INNER JOIN PurchaseOrder po ON po.PurchaseOrderID = gm.PurchaseOrderID
                                 INNER JOIN PurchaseOrderDetails pod ON pod.PurchaseOrderID = po.PurchaseOrderID
                                 INNER JOIN Items itm ON itm.ItemId = pod.ItemID
                                 INNER JOIN ItemType it ON it.ItemTypeID = itm.ItemTypeID
                                 INNER JOIN VendorDetails vds ON vds.VendorId = po.VendorID
                                 INNER JOIN UOM um ON um.UOMId = itm.UOMID
                                 LEFT JOIN VAT vt on vt.VATID = itm.VATID
                                 WHERE GRNNUMBER=@GRNNumber
                                 ORDER BY CONVERT(datetime, gm.createddate, 120) DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@GRN_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    cmd.Parameters.AddWithValue("@GRNNumber", GRNNumber);
                    cmd.Parameters.AddWithValue("@GRN_EmployeeID", HttpContext.Current.Session["UserID"].ToString());
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new GRNMasterData
                            {
                                GRN_ID = sdr["GRN_ID"] != DBNull.Value ? Convert.ToInt32(sdr["GRN_ID"]) : 0,
                                GRN_PoNumber = sdr["GRN_PoNumber"] != DBNull.Value ? sdr["GRN_PoNumber"].ToString() : string.Empty,
                                GRN_MasterCompanyId = sdr["GRN_MasterCompanyId"] != DBNull.Value ? Convert.ToInt32(sdr["GRN_MasterCompanyId"]) : 0,
                                GRN_EmployeeID = sdr["GRN_EmployeeID"] != DBNull.Value ? Convert.ToInt32(sdr["GRN_EmployeeID"]) : 0,
                                GRN_CompanyUnitId = sdr["GRN_CompanyUnitId"] != DBNull.Value ? Convert.ToInt32(sdr["GRN_CompanyUnitId"]) : 0,
                                GRN_Status = sdr["GRN_Status"] != DBNull.Value ? Convert.ToInt32(sdr["GRN_Status"]) : 0,
                                GRN_CounterYear = sdr["GRN_CounterYear"] != DBNull.Value ? Convert.ToInt32(sdr["GRN_CounterYear"]) : 0,
                                GRN_CounterNumber = sdr["GRN_CounterNumber"] != DBNull.Value ? Convert.ToInt32(sdr["GRN_CounterNumber"]) : 0,
                                GRN_NUMBER = sdr["GRN_NUMBER"] != DBNull.Value ? sdr["GRN_NUMBER"].ToString() : string.Empty,
                                GRN_CreatedDateTime = sdr["GRN_CreatedDateTime"] != DBNull.Value ? sdr["GRN_CreatedDateTime"].ToString() : string.Empty,
                                GRN_UpdatedDateTime = sdr["GRN_UpdatedDateTime"] != DBNull.Value ? sdr["GRN_UpdatedDateTime"].ToString() : string.Empty,
                                GRN_ApprovedEmployeeID = sdr["GRN_ApprovedEmployeeID"] != DBNull.Value ? Convert.ToInt32(sdr["GRN_ApprovedEmployeeID"]) : 0,
                                GRN_ApprovedDateTime = sdr["GRN_ApprovedDateTime"] != DBNull.Value ? sdr["GRN_ApprovedDateTime"].ToString() : string.Empty,
                                GRN_ApprovedAccountantEmployeeID = sdr["GRN_ApprovedAccountantEmployeeID"] != DBNull.Value ? Convert.ToInt32(sdr["GRN_ApprovedAccountantEmployeeID"]) : 0,
                                GRN_ApprovedDateTimebyAccountant = sdr["GRN_ApprovedDateTimebyAccountant"] != DBNull.Value ? sdr["GRN_ApprovedDateTimebyAccountant"].ToString() : string.Empty,

                                GDPoId = sdr["GDPoId"] != DBNull.Value ? Convert.ToInt32(sdr["GDPoId"]) : 0,
                                GRNDetailsID = sdr["GRNDetailsID"] != DBNull.Value ? Convert.ToInt32(sdr["GRNDetailsID"]) : 0,
                                GRN = sdr["GRN"] != DBNull.Value ? sdr["GRN"].ToString() : string.Empty,
                                GDPoNumber = sdr["GDPoNumber"] != DBNull.Value ? sdr["GDPoNumber"].ToString() : string.Empty,
                                GDPoItemCode = sdr["GDPoItemCode"] != DBNull.Value ? sdr["GDPoItemCode"].ToString() : string.Empty,
                                GDItemName = sdr["GDItemName"] != DBNull.Value ? sdr["GDItemName"].ToString() : string.Empty,
                                GDITId = sdr["GDITId"] != DBNull.Value ? Convert.ToInt32(sdr["GDITId"]) : 0,
                                GDStatusId = sdr["GDStatusId"] != DBNull.Value ? Convert.ToInt32(sdr["GDStatusId"]) : 0,

                                IT_Id = sdr["IT_Id"] != DBNull.Value ? Convert.ToInt32(sdr["IT_Id"]) : 0,
                                IT_InventoryId = sdr["IT_InventoryId"] != DBNull.Value ? Convert.ToInt32(sdr["IT_InventoryId"]) : 0,
                                IT_ItemId = sdr["IT_ItemId"] != DBNull.Value ? Convert.ToInt32(sdr["IT_ItemId"]) : 0,
                                IT_VendorId = sdr["IT_VendorId"] != DBNull.Value ? Convert.ToInt32(sdr["IT_VendorId"]) : 0,
                                IT_PoNumber = sdr["IT_PoNumber"] != DBNull.Value ? sdr["IT_PoNumber"].ToString() : string.Empty,
                                IT_UOMId = sdr["IT_UOMId"] != DBNull.Value ? Convert.ToInt32(sdr["IT_UOMId"]) : 0,
                                IT_GSTId = sdr["IT_GSTId"] != DBNull.Value ? Convert.ToInt32(sdr["IT_GSTId"]) : 0,
                                IT_RackTypeId = sdr["IT_RackTypeId"] != DBNull.Value ? Convert.ToInt32(sdr["IT_RackTypeId"]) : 0,
                                IT_BasicPrice = sdr["IT_BasicPrice"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_BasicPrice"]) : 0m,
                                IT_ReceivedQty = sdr["IT_ReceivedQty"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_ReceivedQty"]) : 0m,
                                IT_ShortFall = sdr["IT_ShortFall"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_ShortFall"]) : 0m,
                                IT_PreviousShortFall = sdr["IT_PreviousShortFall"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_PreviousShortFall"]) : 0m,
                                IT_ReceivedQtyInvoice = sdr["IT_ReceivedQtyInvoice"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_ReceivedQtyInvoice"]) : 0m,
                                IT_PreviousReceivedQtyInvoice = sdr["IT_PreviousReceivedQtyInvoice"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_PreviousReceivedQtyInvoice"]) : 0m,
                                IT_BalanceQty = sdr["IT_BalanceQty"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_BalanceQty"]) : 0m,
                                IT_TotalAmount = sdr["IT_TotalAmount"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_TotalAmount"]) : 0m,
                                IT_BatchNumber = sdr["IT_BatchNumber"] != DBNull.Value ? sdr["IT_BatchNumber"].ToString() : string.Empty,
                                IT_ExpiryDateTime = sdr["IT_ExpiryDateTime"] != DBNull.Value ? sdr["IT_ExpiryDateTime"].ToString() : string.Empty,
                                IT_DeliveryDateTime = sdr["IT_DeliveryDateTime"] != DBNull.Value ? sdr["IT_DeliveryDateTime"].ToString() : string.Empty,
                                IT_CreatedDateTime = sdr["IT_CreatedDateTime"] != DBNull.Value ? sdr["IT_CreatedDateTime"].ToString() : string.Empty,
                                IT_UpdatedDateTime = sdr["IT_UpdatedDateTime"] != DBNull.Value ? sdr["IT_UpdatedDateTime"].ToString() : string.Empty,
                                IT_RackRowSeriesNo = sdr["IT_RackRowSeriesNo"] != DBNull.Value ? sdr["IT_RackRowSeriesNo"].ToString() : string.Empty,
                                IT_CreatedEmployeeId = sdr["IT_CreatedEmployeeId"] != DBNull.Value ? Convert.ToInt32(sdr["IT_CreatedEmployeeId"]) : 0,
                                IT_MasterCompanyId = sdr["IT_MasterCompanyId"] != DBNull.Value ? Convert.ToInt32(sdr["IT_MasterCompanyId"]) : 0,
                                IT_Status = sdr["IT_Status"] != DBNull.Value ? Convert.ToInt32(sdr["IT_Status"]) : 0,
                                IT_PreviousReceivedQty = sdr["IT_PreviousReceivedQty"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_PreviousReceivedQty"]) : 0m,
                                IT_PreviousBalanceQty = sdr["IT_PreviousBalanceQty"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_PreviousBalanceQty"]) : 0m,
                                IT_AcceptedQty = sdr["IT_AcceptedQty"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_AcceptedQty"]) : 0m,
                                IT_RejectedQty = sdr["IT_RejectedQty"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_RejectedQty"]) : 0m,
                                IT_PreviousAcceptedQty = sdr["IT_PreviousAcceptedQty"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_PreviousAcceptedQty"]) : 0m,
                                IT_PreviousRejectedQty = sdr["IT_PreviousRejectedQty"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_PreviousRejectedQty"]) : 0m,
                                IT_PoId = sdr["IT_PoId"] != DBNull.Value ? Convert.ToInt32(sdr["IT_PoId"]) : 0,
                                IT_OrderedQty = sdr["IT_OrderedQty"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_OrderedQty"]) : 0m,
                                IT_QRCodeNumber = sdr["IT_QRCodeNumber"] != DBNull.Value ? sdr["IT_QRCodeNumber"].ToString() : string.Empty,
                                IT_QRFileName = sdr["IT_QRFileName"] != DBNull.Value ? sdr["IT_QRFileName"].ToString() : string.Empty,
                                IT_ReturnQty = Convert.IsDBNull(sdr["IT_ReturnQty"]) ? 0 : Convert.ToDecimal(sdr["IT_ReturnQty"]),
                                IT_ReplaceQty = Convert.IsDBNull(sdr["IT_ReplaceQty"]) ? 0 : Convert.ToDecimal(sdr["IT_ReplaceQty"]),
                                IT_PreReturnQty = Convert.IsDBNull(sdr["IT_PreReturnQty"]) ? 0 : Convert.ToDecimal(sdr["IT_PreReturnQty"]),
                                IT_PreReplaceQty = Convert.IsDBNull(sdr["IT_PreReplaceQty"]) ? 0 : Convert.ToDecimal(sdr["IT_PreReplaceQty"]),
                                IT_isValid = Convert.IsDBNull(sdr["IT_isValid"]) ? 0 : Convert.ToInt32(sdr["IT_isValid"]),
                                IT_ReturnDateTime = Convert.IsDBNull(sdr["IT_ReturnDateTime"]) ? string.Empty : sdr["IT_ReturnDateTime"].ToString(),
                                IT_ReplaceDateTime = Convert.IsDBNull(sdr["IT_ReplaceDateTime"]) ? string.Empty : sdr["IT_ReplaceDateTime"].ToString(),


                                PurchaseOrderID = sdr["PurchaseOrderID"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseOrderID"]) : 0,
                                PONumber = sdr["PONumber"]?.ToString(),
                                PODate = sdr["PODate"] != DBNull.Value ? Convert.ToDateTime(sdr["PODate"]) : (DateTime?)null,
                                VendorID = sdr["VendorID"] != DBNull.Value ? Convert.ToInt32(sdr["VendorID"]) : (int?)null,
                                Unit = sdr["Unit"]?.ToString(),
                                CreatedDate = sdr["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(sdr["CreatedDate"]) : (DateTime?)null,
                                SpecialInstructions = sdr["SpecialInstructions"]?.ToString(),
                                ReportFileName = sdr["ReportFileName"]?.ToString(),
                                GrandTotal = sdr["GrandTotal"] != DBNull.Value ? Convert.ToDecimal(sdr["GrandTotal"]) : (decimal?)null,
                                UnitID = sdr["UnitID"] != DBNull.Value ? Convert.ToInt32(sdr["UnitID"]) : (int?)null,
                                Delivery = sdr["Delivery"]?.ToString(),
                                Transportation = sdr["Transportation"]?.ToString(),
                                Payment = sdr["Payment"]?.ToString(),
                                Guarantee = sdr["Guarantee"]?.ToString(),
                                Remarks = sdr["Remarks"]?.ToString(),
                                ContactPersonID = sdr["ContactPersonID"] != DBNull.Value ? Convert.ToInt32(sdr["ContactPersonID"]) : (int?)null,
                                PurchaseOrderStatus = sdr["PurchaseOrderStatus"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseOrderStatus"]) : (int?)null,
                                Reason = sdr["Reason"]?.ToString(),
                                POSeries = sdr["POSeries"]?.ToString(),
                                POyear = sdr["POyear"] != DBNull.Value ? Convert.ToInt32(sdr["POyear"]) : (int?)null,
                                StoreType = sdr["StoreType"]?.ToString(),
                                projectid = sdr["projectid"] != DBNull.Value ? Convert.ToInt32(sdr["projectid"]) : (int?)null,
                                penaltyID = sdr["penaltyID"] != DBNull.Value ? Convert.ToInt32(sdr["penaltyID"]) : (int?)null,
                                PODetailsID = sdr["PODetailsID"] != DBNull.Value ? Convert.ToInt32(sdr["PODetailsID"]) : 0,
                                //Discount = sdr["Discount"] != DBNull.Value ? Convert.ToDecimal(sdr["Discount"]) : (decimal?)null,
                                Discount = sdr["Discount"].ToString(),
                                DiscountAmount = sdr["DiscountAmount"] != DBNull.Value ? Convert.ToDecimal(sdr["DiscountAmount"]) : (decimal?)null,
                                Tax = sdr["Tax"] != DBNull.Value ? Convert.ToDecimal(sdr["Tax"]) : (decimal?)null,
                                TotalAmount = sdr["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(sdr["TotalAmount"]) : (decimal?)null,
                                DeliveryDate = sdr["DeliveryDate"] != DBNull.Value ? Convert.ToDateTime(sdr["DeliveryDate"]) : (DateTime?)null,
                                VendorQuotationID = sdr["VendorQuotationID"] != DBNull.Value ? Convert.ToInt32(sdr["VendorQuotationID"]) : (int?)null,
                                Quantity = sdr["Quantity"] != DBNull.Value ? Convert.ToDecimal(sdr["Quantity"]) : (decimal?)null,
                                Rate = sdr["Rate"] != DBNull.Value ? Convert.ToDecimal(sdr["Rate"]) : (decimal?)null,
                                Amount = sdr["Amount"] != DBNull.Value ? Convert.ToDecimal(sdr["Amount"]) : (decimal?)null,
                                ItemID = sdr["ItemID"] != DBNull.Value ? Convert.ToInt32(sdr["ItemID"]) : (int?)null,
                                POStatusID = sdr["POStatusID"] != DBNull.Value ? Convert.ToInt32(sdr["POStatusID"]) : (int?)null,
                                ReceivedQty = sdr["ReceivedQty"] != DBNull.Value ? Convert.ToDecimal(sdr["ReceivedQty"]) : (decimal?)null,
                                TransactionID = sdr["TransactionID"] != DBNull.Value ? Convert.ToInt32(sdr["TransactionID"]) : (int?)null,
                                AllocatedStock = sdr["AllocatedStock"] != DBNull.Value ? Convert.ToDecimal(sdr["AllocatedStock"]) : (decimal?)null,
                                ReceivedBy = sdr["ReceivedBy"] != DBNull.Value ? Convert.ToInt32(sdr["ReceivedBy"]) : (int?)null,
                                SecurityReceived = sdr["SecurityReceived"] != DBNull.Value ? Convert.ToDecimal(sdr["SecurityReceived"]) : (decimal?)null,
                                TaxAmount = sdr["TaxAmount"] != DBNull.Value ? Convert.ToDecimal(sdr["TaxAmount"]) : (decimal?)null,
                                VATID = sdr["VATID"] != DBNull.Value ? Convert.ToInt32(sdr["VATID"]) : (int?)null,
                                VendorQuotationDetailsID = sdr["VendorQuotationDetailsID"] != DBNull.Value ? Convert.ToInt32(sdr["VendorQuotationDetailsID"]) : (int?)null,
                                EnquiryDetailsID = sdr["EnquiryDetailsID"] != DBNull.Value ? Convert.ToInt32(sdr["EnquiryDetailsID"]) : (int?)null,
                                PurchaseComments = sdr["PurchaseComments"]?.ToString(),


                                PO_OrderedQty = sdr["PO_OrderedQty"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_OrderedQty"]) : 0,
                                PO_ReceivedQty = sdr["PO_ReceivedQty"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_ReceivedQty"]) : 0,
                                PO_GettingStock = sdr["PO_GettingStock"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_GettingStock"]) : 0,
                                PO_BalanceQty = sdr["PO_BalanceQty"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_BalanceQty"]) : 0,
                                PO_BasicPrice = sdr["PO_BasicPrice"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_BasicPrice"]) : 0,
                                PO_StatusId = sdr["PO_StatusId"] != DBNull.Value ? Convert.ToInt32(sdr["PO_StatusId"]) : 0,
                                PO_IsActive = sdr["PO_IsActive"] != DBNull.Value ? Convert.ToInt32(sdr["PO_IsActive"]) : 0,
                                PO_AcceptQty = sdr["PO_AcceptQty"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_AcceptQty"]) : 0,
                                PO_RejectQty = sdr["PO_RejectQty"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_RejectQty"]) : 0,
                                PO_StatusForIT = sdr["PO_StatusForIT"] != DBNull.Value ? Convert.ToInt32(sdr["PO_StatusForIT"]) : 0,
                                PO_ShortFall = sdr["PO_ShortFall"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_ShortFall"]) : 0,
                                PO_ReturnQty = sdr["PO_ReturnQty"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_ReturnQty"]) : 0,
                                PO_IsNotificationSend = Convert.IsDBNull(sdr["PO_IsNotificationSend"]) ? 0 : Convert.ToInt32(sdr["PO_IsNotificationSend"]),


                                ItemCode = sdr["ItemCode"] != DBNull.Value ? sdr["ItemCode"].ToString() : string.Empty,
                                ItemName = sdr["ItemName"] != DBNull.Value ? sdr["ItemName"].ToString() : string.Empty,
                                ItemDescription = sdr["ItemDescription"] != DBNull.Value ? sdr["ItemDescription"].ToString() : string.Empty,
                                PurchaseSubGroupID = sdr["PurchaseSubGroupID"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseSubGroupID"]) : 0,
                                LeadTime = sdr["LeadTime"] != DBNull.Value ? Convert.ToInt32(sdr["LeadTime"]) : 0,
                                UOMID = sdr["UOMID"] != DBNull.Value ? Convert.ToInt32(sdr["UOMID"]) : 0,
                                StockQuantity = sdr["StockQuantity"] != DBNull.Value ? Convert.ToDecimal(sdr["StockQuantity"]) : 0,
                                MinStock = sdr["MinStock"] != DBNull.Value ? Convert.ToDecimal(sdr["MinStock"]) : 0,
                                MaxStock = sdr["MaxStock"] != DBNull.Value ? Convert.ToDecimal(sdr["MaxStock"]) : 0,
                                DisabledItem = sdr["DisabledItem"] != DBNull.Value ? Convert.ToBoolean(sdr["DisabledItem"]) : false,
                                HSNCode = sdr["HSNCode"] != DBNull.Value ? sdr["HSNCode"].ToString() : string.Empty,
                                VAT = sdr["VAT"] != DBNull.Value ? Convert.ToDecimal(sdr["VAT"]) : 0,
                                UOMValue = sdr["UOMValue"] != DBNull.Value ? Convert.ToDecimal(sdr["UOMValue"]) : 0,
                                ConversionUOMID = sdr["ConversionUOMID"] != DBNull.Value ? Convert.ToInt32(sdr["ConversionUOMID"]) : 0,
                                ConversionUOMValue = sdr["ConversionUOMValue"] != DBNull.Value ? Convert.ToDecimal(sdr["ConversionUOMValue"]) : 0,
                                ModelNo = sdr["ModelNo"] != DBNull.Value ? sdr["ModelNo"].ToString() : string.Empty,
                                ItemTypeID = sdr["ItemTypeID"] != DBNull.Value ? Convert.ToInt32(sdr["ItemTypeID"]) : 0,
                                Description = sdr["Description"] != DBNull.Value ? sdr["Description"].ToString() : string.Empty,
                                Specification = sdr["Specification"] != DBNull.Value ? sdr["Specification"].ToString() : string.Empty,
                                PurchaseGroupID = sdr["PurchaseGroupID"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseGroupID"]) : 0,
                                CreatedBy = sdr["CreatedBy"] != DBNull.Value ? Convert.ToInt32(sdr["CreatedBy"]) : 0,
                                ConvertUOM = sdr["ConvertUOM"] != DBNull.Value ? Convert.ToInt32(sdr["ConvertUOM"]) : 0,
                                ItemType = sdr["ItemType"] != DBNull.Value ? sdr["ItemType"].ToString() : string.Empty,

                                InventoryId = sdr["InventoryId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryId"]) : 0,
                                InventoryRackTypeId = sdr["InventoryRackTypeId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryRackTypeId"]) : 0,
                                InventoryItemCategoryId = sdr["InventoryItemCategoryId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryItemCategoryId"]) : 0,
                                InventoryItemId = sdr["InventoryItemId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryItemId"]) : 0,
                                InventoryUOMId = sdr["InventoryUOMId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryUOMId"]) : 0,
                                InventoryGSTId = sdr["InventoryGSTId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryGSTId"]) : 0,
                                InventoryCurrentStock = sdr["InventoryCurrentStock"] != DBNull.Value ? Convert.ToDecimal(sdr["InventoryCurrentStock"]) : 0,
                                InventoryMinStock = sdr["InventoryMinStock"] != DBNull.Value ? Convert.ToDecimal(sdr["InventoryMinStock"]) : 0,
                                InventoryCreatedEmployeeId = sdr["InventoryCreatedEmployeeId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryCreatedEmployeeId"]) : 0,
                                InventoryMasterCompanyId = sdr["InventoryMasterCompanyId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryMasterCompanyId"]) : 0,
                                InventoryItemLocationStore = sdr["InventoryItemLocationStore"] != DBNull.Value ? sdr["InventoryItemLocationStore"].ToString() : string.Empty,
                                InventoryCreatedDateTime = sdr["InventoryCreatedDateTime"] != DBNull.Value ? sdr["InventoryCreatedDateTime"].ToString() : string.Empty,
                                InventoryUpdatedDateTime = sdr["InventoryUpdatedDateTime"] != DBNull.Value ? sdr["InventoryUpdatedDateTime"].ToString() : string.Empty,

                                UOMId = sdr["UOMId"] != DBNull.Value ? Convert.ToInt32(sdr["UOMId"]) : 0,
                                UOMCode = sdr["UOMName"] != DBNull.Value ? sdr["UOMName"].ToString() : string.Empty,
                                UOMName = sdr["UOMName"] != DBNull.Value ? sdr["UOMName"].ToString() : string.Empty,

                                VATCode = sdr["VATCode"] != DBNull.Value ? sdr["VATCode"].ToString() : string.Empty,
                                Percentage = sdr["Percentage"] != DBNull.Value ? Convert.ToDecimal(sdr["Percentage"]) : 0,

                                VendorCode = sdr["VendorCode"]?.ToString(),
                                VendorName = sdr["VendorName"]?.ToString(),
                                DealerID = sdr["DealerID"] != DBNull.Value ? Convert.ToInt32(sdr["DealerID"]) : (int?)null,
                                Website = sdr["Website"]?.ToString(),
                                YearOfEstablishment = sdr["YearOfEstablishment"]?.ToString(),
                                TypeOfBusiness = sdr["TypeOfBusiness"]?.ToString(),
                                CompanyTradeLicence = sdr["CompanyTradeLicence"]?.ToString(),
                                YearlyTurnover1 = sdr["YearlyTurnover1"] != DBNull.Value ? Convert.ToDecimal(sdr["YearlyTurnover1"]) : (decimal?)null,
                                RequiredCredit = sdr["RequiredCredit"] != DBNull.Value ? Convert.ToDecimal(sdr["RequiredCredit"]) : (decimal?)null,
                                ApprovedCredit = sdr["ApprovedCredit"] != DBNull.Value ? Convert.ToDecimal(sdr["ApprovedCredit"]) : (decimal?)null,
                                PanNumber = sdr["PanNumber"]?.ToString(),
                                AdharNumber = sdr["AdharNumber"]?.ToString(),
                                EstablishmentID = sdr["EstablishmentID"] != DBNull.Value ? Convert.ToInt32(sdr["EstablishmentID"]) : (int?)null,
                                RegistrationNumber = sdr["RegistrationNumber"]?.ToString(),
                                YearlyTurnover2 = sdr["YearlyTurnover2"] != DBNull.Value ? Convert.ToDecimal(sdr["YearlyTurnover2"]) : (decimal?)null,
                                YearlyTurnover3 = sdr["YearlyTurnover3"] != DBNull.Value ? Convert.ToDecimal(sdr["YearlyTurnover3"]) : (decimal?)null,
                                Year1 = sdr["Year1"]?.ToString(),
                                Year2 = sdr["Year2"]?.ToString(),
                                Year3 = sdr["Year3"]?.ToString(),
                                UANNumber = sdr["UANNumber"]?.ToString(),
                                PFNumber = sdr["PFNumber"]?.ToString(),
                                NumberofEmployee = sdr["NumberofEmployee"].ToString(),
                                IEC = sdr["IEC"]?.ToString(),
                                ESIC = sdr["ESIC"]?.ToString(),
                                VendorStatus = sdr["VendorStatus"].ToString(),
                                VendorCurrency = sdr["VendorCurrency"]?.ToString(),
                                CompanySizeID = sdr["CompanySizeID"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<GRNMasterData> GRNDetailsShowNew()
        {
            List<GRNMasterData> items = new List<GRNMasterData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM (SELECT DISTINCT  GRNNumber as GRN_NUMBER, PONumber as GRN_PoNumber,VendorName, gm.createddate as GRN_CreatedDateTime,
												ItemCode, ItemName,
                                                gd.Quantity as IT_OrderedQty,gd.PreviousReceivedQty as IT_PreviousReceivedQty,
											   gd.ReceivedQty as IT_ReceivedQty, gd.ReceivedQty as IT_AcceptedQty,0 as IT_RejectedQty,
											   (gd.Quantity - gd.ReceivedQty) IT_BalanceQty,po.CreatedDate as PO_CreatedDateTime,
                                               pod.Rate as IT_BasicPrice,0 Percentage, 1 GRN_CompanyUnitId,itm.ItemID IT_Id, 
											   gm.GRNStatus GDStatusId,itm.ItemCode GDPoItemCode,gm.InwardID GDITId
                                                FROM TransactionMaster gm
                                                INNER JOIN Transactiondetails gd ON gd.InwardID = gm.InwardID
                                                INNER JOIN PurchaseOrder po ON po.PurchaseOrderID = gm.PurchaseOrderID
                                                INNER JOIN PurchaseOrderDetails pod ON pod.PurchaseOrderID = po.PurchaseOrderID and gd.ItemID = pod.ItemID
                                                INNER JOIN Items itm ON itm.ItemId = gd.ItemID
                                                INNER JOIN ItemType it ON it.ItemTypeID = itm.ItemTypeID
                                                INNER JOIN VendorDetails vds ON vds.VendorId = po.VendorID
                                                INNER JOIN UOM um ON um.UOMId = itm.UOMID
                                            ) AS DistinctResults
                                            ORDER BY CONVERT(datetime, GRN_CreatedDateTime, 120) DESC;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    //cmd.Parameters.AddWithValue("@GRN_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    //cmd.Parameters.AddWithValue("@GRN_EmployeeID", HttpContext.Current.Session["UserID"].ToString());
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new GRNMasterData
                            {
                                GRN_CompanyUnitId = sdr["GRN_CompanyUnitId"] != DBNull.Value ? Convert.ToInt32(sdr["GRN_CompanyUnitId"]) : 0,
                                IT_Id = sdr["IT_Id"] != DBNull.Value ? Convert.ToInt32(sdr["IT_Id"]) : 0,
                                GRN_NUMBER = sdr["GRN_NUMBER"] != DBNull.Value ? sdr["GRN_NUMBER"].ToString() : string.Empty,
                                GRN_PoNumber = sdr["GRN_PoNumber"] != DBNull.Value ? sdr["GRN_PoNumber"].ToString() : string.Empty,
                                VendorName = sdr["VendorName"] != DBNull.Value ? sdr["VendorName"].ToString() : string.Empty,
                                GRN_CreatedDateTime = sdr["GRN_CreatedDateTime"] != DBNull.Value ? sdr["GRN_CreatedDateTime"].ToString() : string.Empty,
                                ItemCode = sdr["ItemCode"] != DBNull.Value ? sdr["ItemCode"].ToString() : string.Empty,
                                ItemName = sdr["ItemName"] != DBNull.Value ? sdr["ItemName"].ToString() : string.Empty,
                                IT_OrderedQty = sdr["IT_OrderedQty"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_OrderedQty"]) : 0m,
                                IT_PreviousReceivedQty = sdr["IT_PreviousReceivedQty"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_PreviousReceivedQty"]) : 0m,
                                IT_ReceivedQty = sdr["IT_ReceivedQty"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_ReceivedQty"]) : 0m,
                                IT_AcceptedQty = sdr["IT_AcceptedQty"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_AcceptedQty"]) : 0m,
                                IT_RejectedQty = sdr["IT_RejectedQty"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_RejectedQty"]) : 0m,
                                IT_BalanceQty = sdr["IT_BalanceQty"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_BalanceQty"]) : 0m,
                                IT_BasicPrice = sdr["IT_BasicPrice"] != DBNull.Value ? Convert.ToDecimal(sdr["IT_BasicPrice"]) : 0m,
                                Percentage = sdr["Percentage"] != DBNull.Value ? Convert.ToDecimal(sdr["Percentage"]) : 0m,
                                GDStatusId = sdr["GDStatusId"] != DBNull.Value ? Convert.ToInt32(sdr["GDStatusId"]) : 0,                                
                                GDPoItemCode = sdr["GDPoItemCode"] != DBNull.Value ? sdr["GDPoItemCode"].ToString() : string.Empty,
                                GDITId = sdr["GDITId"] != DBNull.Value ? Convert.ToInt32(sdr["GDITId"]) : 0,
                                PO_CreatedDateTime = sdr["PO_CreatedDateTime"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<GRNMasterData> GRNBillImageShow()
        {
            List<GRNMasterData> items = new List<GRNMasterData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT gm.InwardID GRN_ID,po.PONumber GRN_PoNumber,1 GRN_MasterCompanyId,gm.ReceivedBy GRN_EmployeeID,
                                    1 GRN_CompanyUnitId,gm.GRNStatus GRN_Status, gm.GRNYear GRN_CounterYear,gm.GRNCount GRN_CounterNumber,
                                    gm.GRNNumber GRN_NUMBER,gm.createddate GRN_CreatedDateTime,gm.createddate GRN_UpdatedDateTime,gm.ReceivedBy GRN_ApprovedEmployeeID,
                                    gm.createddate GRN_ApprovedDateTime,gm.approvedby GRN_ApprovedAccountantEmployeeID,gm.approvedat GRN_ApprovedDateTimebyAccountant,
                                    gfu.GRNFileName,gfu.DocumentName
                                    from TransactionMaster gm
                                    inner join PurchaseOrder po on po.PurchaseOrderID = gm.PurchaseOrderID
                                 INNER JOIN GRNFileUpload gfu ON gfu.GRN_ID = gm.InwardID
                                 ORDER BY CONVERT(datetime, gm.createddate, 120) DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new GRNMasterData
                            {
                                GRN_ID = Convert.IsDBNull(sdr["GRN_ID"]) ? 0 : Convert.ToInt32(sdr["GRN_ID"]),
                                GRN_PoNumber = Convert.IsDBNull(sdr["GRN_PoNumber"]) ? string.Empty : sdr["GRN_PoNumber"].ToString(),
                                GRN_MasterCompanyId = Convert.IsDBNull(sdr["GRN_MasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["GRN_MasterCompanyId"]),
                                GRN_EmployeeID = Convert.IsDBNull(sdr["GRN_EmployeeID"]) ? 0 : Convert.ToInt32(sdr["GRN_EmployeeID"]),
                                GRN_CompanyUnitId = Convert.IsDBNull(sdr["GRN_CompanyUnitId"]) ? 0 : Convert.ToInt32(sdr["GRN_CompanyUnitId"]),
                                GRN_Status = Convert.IsDBNull(sdr["GRN_Status"]) ? 0 : Convert.ToInt32(sdr["GRN_Status"]),
                                GRN_CounterYear = Convert.IsDBNull(sdr["GRN_CounterYear"]) ? 0 : Convert.ToInt32(sdr["GRN_CounterYear"]),
                                GRN_CounterNumber = Convert.IsDBNull(sdr["GRN_CounterNumber"]) ? 0 : Convert.ToInt32(sdr["GRN_CounterNumber"]),
                                GRN_NUMBER = Convert.IsDBNull(sdr["GRN_NUMBER"]) ? string.Empty : sdr["GRN_NUMBER"].ToString(),
                                GRN_CreatedDateTime = Convert.IsDBNull(sdr["GRN_CreatedDateTime"]) ? string.Empty : sdr["GRN_CreatedDateTime"].ToString(),
                                GRN_UpdatedDateTime = Convert.IsDBNull(sdr["GRN_UpdatedDateTime"]) ? string.Empty : sdr["GRN_UpdatedDateTime"].ToString(),
                                GRN_ApprovedEmployeeID = Convert.IsDBNull(sdr["GRN_ApprovedEmployeeID"]) ? 0 : Convert.ToInt32(sdr["GRN_ApprovedEmployeeID"]),
                                GRN_ApprovedDateTime = Convert.IsDBNull(sdr["GRN_ApprovedDateTime"]) ? string.Empty : sdr["GRN_ApprovedDateTime"].ToString(),
                                GRN_ApprovedAccountantEmployeeID = Convert.IsDBNull(sdr["GRN_ApprovedAccountantEmployeeID"]) ? 0 : Convert.ToInt32(sdr["GRN_ApprovedAccountantEmployeeID"]),
                                GRN_ApprovedDateTimebyAccountant = Convert.IsDBNull(sdr["GRN_ApprovedDateTimebyAccountant"]) ? string.Empty : sdr["GRN_ApprovedDateTimebyAccountant"].ToString(),
                                GRNFileName = Convert.IsDBNull(sdr["GRNFileName"]) ? string.Empty : sdr["GRNFileName"].ToString(),
                                GRNDocumentName = Convert.IsDBNull(sdr["DocumentName"]) ? string.Empty : sdr["DocumentName"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<GRNMasterData> GRNBillImage()
        {
            List<GRNMasterData> items = new List<GRNMasterData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * from GRNFileUpload";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new GRNMasterData
                            {
                                GRNFileId = Convert.ToInt32(sdr["GRNFileId"]),
                                GRN_ID = Convert.ToInt32(sdr["GRN_ID"]),
                                GRNO = sdr["GRNO"].ToString(),
                                GRNFileName = sdr["GRNFileName"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<PoExcelUploadMasterData> ProjectWiseItemCodeShow(string projectID)
        {
            List<PoExcelUploadMasterData> items = new List<PoExcelUploadMasterData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"select distinct PO_ItemCode from PoExcelUploadMaster where PO_ProjectID = @PO_ProjectID ";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@PO_ProjectID", projectID);
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new PoExcelUploadMasterData
                            {
                                PO_ItemCode = sdr["PO_ItemCode"] != DBNull.Value ? sdr["PO_ItemCode"].ToString() : string.Empty
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<PoExcelUploadMasterData> PoNoShowInDropDown()
        {
            List<PoExcelUploadMasterData> items = new List<PoExcelUploadMasterData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM PoExcelUploadMaster 
                                  WHERE PO_MasterCompanyId = @PO_MasterCompanyId AND 
                                        PO_IsActive = @PO_IsActive AND 
                                        PO_CompanyUnitId = @PO_CompanyUnitId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@PO_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    cmd.Parameters.AddWithValue("@PO_CompanyUnitId", HttpContext.Current.Session["CompanyUnitId"].ToString());
                    cmd.Parameters.AddWithValue("@PO_IsActive", 1);
                    cmd.Parameters.AddWithValue("@PO_BalanceQty", 0);

                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new PoExcelUploadMasterData
                            {
                                PO_Id = sdr["PO_Id"] != DBNull.Value ? Convert.ToInt32(sdr["PO_Id"]) : 0,
                                PO_Number = sdr["PO_Number"] != DBNull.Value ? sdr["PO_Number"].ToString() : string.Empty,
                                PO_ItemCode = sdr["PO_ItemCode"] != DBNull.Value ? sdr["PO_ItemCode"].ToString() : string.Empty,
                                PO_ItemName = sdr["PO_ItemName"] != DBNull.Value ? sdr["PO_ItemName"].ToString() : string.Empty,
                                PO_OrderedQty = sdr["PO_OrderedQty"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_OrderedQty"]) : 0,
                                PO_ReceivedQty = sdr["PO_ReceivedQty"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_ReceivedQty"]) : 0,
                                PO_GettingStock = sdr["PO_GettingStock"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_GettingStock"]) : 0,
                                PO_BalanceQty = sdr["PO_BalanceQty"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_BalanceQty"]) : 0,
                                PO_BasicPrice = sdr["PO_BasicPrice"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_BasicPrice"]) : 0,
                                PO_GSTPercentage = sdr["PO_GSTPercentage"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_GSTPercentage"]) : 0,
                                PO_VendorCode = sdr["PO_VendorCode"] != DBNull.Value ? sdr["PO_VendorCode"].ToString() : string.Empty,
                                PO_OrderedDateTime = sdr["PO_OrderedDateTime"] != DBNull.Value ? sdr["PO_OrderedDateTime"].ToString() : string.Empty,
                                PO_DeliveryDateTime = sdr["PO_DeliveryDateTime"] != DBNull.Value ? sdr["PO_DeliveryDateTime"].ToString() : string.Empty,
                                PO_GeneratedDateTime = sdr["PO_GeneratedDateTime"] != DBNull.Value ? sdr["PO_GeneratedDateTime"].ToString() : string.Empty,
                                PO_CreatedDateTime = sdr["PO_CreatedDateTime"] != DBNull.Value ? sdr["PO_CreatedDateTime"].ToString() : string.Empty,
                                PO_UpdatedDateTime = sdr["PO_UpdatedDateTime"] != DBNull.Value ? sdr["PO_UpdatedDateTime"].ToString() : string.Empty,
                                PO_CompanyUnitId = sdr["PO_CompanyUnitId"] != DBNull.Value ? Convert.ToInt32(sdr["PO_CompanyUnitId"]) : 0,
                                PO_CreatedEmployeeId = sdr["PO_CreatedEmployeeId"] != DBNull.Value ? Convert.ToInt32(sdr["PO_CreatedEmployeeId"]) : 0,
                                PO_MasterCompanyId = sdr["PO_MasterCompanyId"] != DBNull.Value ? Convert.ToInt32(sdr["PO_MasterCompanyId"]) : 0,
                                PO_StatusId = sdr["PO_StatusId"] != DBNull.Value ? Convert.ToInt32(sdr["PO_StatusId"]) : 0,
                                PO_IsActive = sdr["PO_IsActive"] != DBNull.Value ? Convert.ToInt32(sdr["PO_IsActive"]) : 0,
                                PO_AcceptQty = sdr["PO_AcceptQty"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_AcceptQty"]) : 0,
                                PO_RejectQty = sdr["PO_RejectQty"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_RejectQty"]) : 0,
                                PO_StatusForIT = sdr["PO_StatusForIT"] != DBNull.Value ? Convert.ToInt32(sdr["PO_StatusForIT"]) : 0,
                                PO_ShortFall = sdr["PO_ShortFall"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_ShortFall"]) : 0,
                                PO_ReturnQty = sdr["PO_ReturnQty"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_ReturnQty"]) : 0,


                                PO_IsNotificationSend = Convert.IsDBNull(sdr["PO_IsNotificationSend"]) ? 0 : Convert.ToInt32(sdr["PO_IsNotificationSend"]),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<PoExcelUploadMasterData> ExcelUploadPODetails()
        {
            List<PoExcelUploadMasterData> items = new List<PoExcelUploadMasterData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM PoExcelUploadMaster pum
                                 INNER JOIN ItemMaster itm ON itm.ItemCode = pum.PO_ItemCode
                                 INNER JOIN VendorDetailsMaster vdm ON vdm.VendorCode = pum.PO_VendorCode
                                 INNER JOIN StatusMasterTB sm ON sm.StatusID = pum.PO_StatusId
                                 WHERE pum.PO_MasterCompanyId = @PO_MasterCompanyId AND PO_IsActive = @PO_IsActive
                                 ORDER BY CONVERT(datetime, pum.PO_CreatedDateTime, 120) DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@PO_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    cmd.Parameters.AddWithValue("@PO_IsActive", 1);
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new PoExcelUploadMasterData
                            {
                                PO_Id = Convert.IsDBNull(sdr["PO_Id"]) ? 0 : Convert.ToInt32(sdr["PO_Id"]),
                                PO_Number = sdr["PO_Number"]?.ToString() ?? "",
                                PO_ItemCode = sdr["PO_ItemCode"]?.ToString() ?? "",
                                PO_ItemName = sdr["PO_ItemName"]?.ToString() ?? "",
                                PO_OrderedQty = Convert.IsDBNull(sdr["PO_OrderedQty"]) ? 0 : Convert.ToDecimal(sdr["PO_OrderedQty"]),
                                PO_ReceivedQty = Convert.IsDBNull(sdr["PO_ReceivedQty"]) ? 0 : Convert.ToDecimal(sdr["PO_ReceivedQty"]),
                                PO_GettingStock = Convert.IsDBNull(sdr["PO_GettingStock"]) ? 0 : Convert.ToDecimal(sdr["PO_GettingStock"]),
                                PO_BalanceQty = Convert.IsDBNull(sdr["PO_BalanceQty"]) ? 0 : Convert.ToDecimal(sdr["PO_BalanceQty"]),
                                PO_BasicPrice = Convert.IsDBNull(sdr["PO_BasicPrice"]) ? 0 : Convert.ToDecimal(sdr["PO_BasicPrice"]),
                                PO_GSTPercentage = Convert.IsDBNull(sdr["PO_GSTPercentage"]) ? 0 : Convert.ToDecimal(sdr["PO_GSTPercentage"]),
                                PO_VendorCode = sdr["PO_VendorCode"]?.ToString() ?? "",
                                PO_OrderedDateTime = sdr["PO_OrderedDateTime"]?.ToString() ?? "",
                                PO_DeliveryDateTime = sdr["PO_DeliveryDateTime"]?.ToString() ?? "",
                                PO_GeneratedDateTime = sdr["PO_GeneratedDateTime"]?.ToString() ?? "",
                                PO_CreatedDateTime = sdr["PO_CreatedDateTime"]?.ToString() ?? "",
                                PO_UpdatedDateTime = sdr["PO_UpdatedDateTime"]?.ToString() ?? "",
                                PO_CompanyUnitId = Convert.IsDBNull(sdr["PO_CompanyUnitId"]) ? 0 : Convert.ToInt32(sdr["PO_CompanyUnitId"]),
                                PO_CreatedEmployeeId = Convert.IsDBNull(sdr["PO_CreatedEmployeeId"]) ? 0 : Convert.ToInt32(sdr["PO_CreatedEmployeeId"]),
                                PO_MasterCompanyId = Convert.IsDBNull(sdr["PO_MasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["PO_MasterCompanyId"]),
                                PO_StatusId = Convert.IsDBNull(sdr["PO_StatusId"]) ? 0 : Convert.ToInt32(sdr["PO_StatusId"]),
                                PO_IsActive = Convert.IsDBNull(sdr["PO_IsActive"]) ? 0 : Convert.ToInt32(sdr["PO_IsActive"]),
                                PO_IsNotificationSend = Convert.IsDBNull(sdr["PO_IsNotificationSend"]) ? 0 : Convert.ToInt32(sdr["PO_IsNotificationSend"]),
                                PO_AcceptQty = Convert.IsDBNull(sdr["PO_AcceptQty"]) ? 0 : Convert.ToDecimal(sdr["PO_AcceptQty"]),
                                PO_RejectQty = Convert.IsDBNull(sdr["PO_RejectQty"]) ? 0 : Convert.ToDecimal(sdr["PO_RejectQty"]),
                                PO_StatusForIT = Convert.IsDBNull(sdr["PO_StatusForIT"]) ? 0 : Convert.ToInt32(sdr["PO_StatusForIT"]),
                                PO_ShortFall = Convert.IsDBNull(sdr["PO_ShortFall"]) ? 0 : Convert.ToDecimal(sdr["PO_ShortFall"]),
                                PO_ReturnQty = Convert.IsDBNull(sdr["PO_ReturnQty"]) ? 0 : Convert.ToDecimal(sdr["PO_ReturnQty"]),

                                PO_Status_Title = sdr["StatusTitle"]?.ToString() ?? "",

                                VendorName = sdr["VendorName"]?.ToString() ?? "",
                                VendorCompanyName = sdr["VendorCompanyName"]?.ToString() ?? "",

                                ItemId = Convert.IsDBNull(sdr["ItemId"]) ? 0 : Convert.ToInt32(sdr["ItemId"]),


                                PO_ProjectID = sdr["PO_ProjectID"]?.ToString() ?? "",
                                PO_ApprovalStatus = sdr["PO_ApprovalStatus"]?.ToString() ?? "",
                                PO_VendorName = sdr["PO_VendorName"]?.ToString() ?? "",
                                PO_Unit = sdr["PO_Unit"]?.ToString() ?? "",
                                PO_ProductType = sdr["PO_ProductType"]?.ToString() ?? "",
                                PO_DeliverRemainder = sdr["PO_DeliverRemainder"]?.ToString() ?? "",
                                PO_UnitPrice = Convert.IsDBNull(sdr["PO_UnitPrice"]) ? 0 : Convert.ToDecimal(sdr["PO_UnitPrice"]),
                                PO_NetAmount = Convert.IsDBNull(sdr["PO_NetAmount"]) ? 0 : Convert.ToDecimal(sdr["PO_NetAmount"]),
                                PO_Site = sdr["PO_Site"]?.ToString() ?? "",
                                PO_Warehouse = sdr["PO_Warehouse"]?.ToString() ?? "",
                                PO_Location = sdr["PO_Location"]?.ToString() ?? "",
                                PO_PurchaseRequisition = sdr["PO_PurchaseRequisition"]?.ToString() ?? "",
                                PO_PurchaseLineCreatedDateTime = sdr["PO_PurchaseLineCreatedDateTime"]?.ToString() ?? "",
                                PO_ProjectCategory = sdr["PO_ProjectCategory"]?.ToString() ?? "",
                                PO_Currency = sdr["PO_Currency"]?.ToString() ?? "",
                                PO_TermsOfPayment = sdr["PO_TermsOfPayment"]?.ToString() ?? ""

                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<PoExcelUploadMasterData> ExcelUploadPODetailsPaged(int pageIndex, int pageSize, string searchTerm, out int totalRecords)
        {
            List<PoExcelUploadMasterData> items = new List<PoExcelUploadMasterData>();
            totalRecords = 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string countQuery = $@"SELECT COUNT(*) 
                                        FROM PoExcelUploadMaster pum
                                        INNER JOIN ItemMaster itm ON itm.ItemCode = pum.PO_ItemCode
                                        INNER JOIN VendorDetailsMaster vdm ON vdm.VendorCode = pum.PO_VendorCode
                                        INNER JOIN StatusMasterTB sm ON sm.StatusID = pum.PO_StatusId
                                        WHERE pum.PO_MasterCompanyId = @PO_MasterCompanyId 
                                          AND pum.PO_IsActive = @PO_IsActive
                                          AND (
                                                pum.PO_Number LIKE @SearchTerm OR 
                                                pum.PO_ItemCode LIKE @SearchTerm OR 
                                                pum.PO_ItemName LIKE @SearchTerm OR 
                                                pum.PO_VendorCode LIKE @SearchTerm OR
                                                vdm.VendorName LIKE @SearchTerm)";

                string dataQuery = $@"SELECT * 
                                    FROM PoExcelUploadMaster pum
                                    INNER JOIN ItemMaster itm ON itm.ItemCode = pum.PO_ItemCode
                                    INNER JOIN VendorDetailsMaster vdm ON vdm.VendorCode = pum.PO_VendorCode
                                    INNER JOIN StatusMasterTB sm ON sm.StatusID = pum.PO_StatusId
                                    WHERE pum.PO_MasterCompanyId = @PO_MasterCompanyId 
                                      AND pum.PO_IsActive = @PO_IsActive
                                      AND (
                                            pum.PO_Number LIKE @SearchTerm OR 
                                            pum.PO_ItemCode LIKE @SearchTerm OR 
                                            pum.PO_ItemName LIKE @SearchTerm OR 
                                            pum.PO_VendorCode LIKE @SearchTerm OR
                                            vdm.VendorName LIKE @SearchTerm)
                                    ORDER BY CONVERT(datetime, pum.PO_CreatedDateTime, 120) DESC
                                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                using (SqlCommand countCmd = new SqlCommand(countQuery, con))
                using (SqlCommand dataCmd = new SqlCommand(dataQuery, con))
                {
                    string likeSearch = "%" + searchTerm + "%";
                    string masterCompanyId = HttpContext.Current.Session["MasterCompanyId"].ToString();

                    // Common parameters
                    foreach (var cmd in new[] { countCmd, dataCmd })
                    {
                        cmd.Parameters.AddWithValue("@PO_MasterCompanyId", masterCompanyId);
                        cmd.Parameters.AddWithValue("@PO_IsActive", 1);
                        cmd.Parameters.AddWithValue("@SearchTerm", likeSearch);
                    }

                    dataCmd.Parameters.AddWithValue("@Offset", (pageIndex - 1) * pageSize);
                    dataCmd.Parameters.AddWithValue("@PageSize", pageSize);

                    con.Open();
                    totalRecords = (int)countCmd.ExecuteScalar();

                    using (SqlDataReader sdr = dataCmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            PoExcelUploadMasterData item = new PoExcelUploadMasterData
                            {
                                PO_Id = Convert.IsDBNull(sdr["PO_Id"]) ? 0 : Convert.ToInt32(sdr["PO_Id"]),
                                PO_Number = sdr["PO_Number"]?.ToString() ?? "",
                                PO_ItemCode = sdr["PO_ItemCode"]?.ToString() ?? "",
                                PO_ItemName = sdr["PO_ItemName"]?.ToString() ?? "",
                                PO_OrderedQty = Convert.IsDBNull(sdr["PO_OrderedQty"]) ? 0 : Convert.ToDecimal(sdr["PO_OrderedQty"]),
                                PO_ReceivedQty = Convert.IsDBNull(sdr["PO_ReceivedQty"]) ? 0 : Convert.ToDecimal(sdr["PO_ReceivedQty"]),
                                PO_GettingStock = Convert.IsDBNull(sdr["PO_GettingStock"]) ? 0 : Convert.ToDecimal(sdr["PO_GettingStock"]),
                                PO_BalanceQty = Convert.IsDBNull(sdr["PO_BalanceQty"]) ? 0 : Convert.ToDecimal(sdr["PO_BalanceQty"]),
                                PO_BasicPrice = Convert.IsDBNull(sdr["PO_BasicPrice"]) ? 0 : Convert.ToDecimal(sdr["PO_BasicPrice"]),
                                PO_GSTPercentage = Convert.IsDBNull(sdr["PO_GSTPercentage"]) ? 0 : Convert.ToDecimal(sdr["PO_GSTPercentage"]),
                                PO_VendorCode = sdr["PO_VendorCode"]?.ToString() ?? "",
                                PO_OrderedDateTime = sdr["PO_OrderedDateTime"]?.ToString() ?? "",
                                PO_DeliveryDateTime = sdr["PO_DeliveryDateTime"]?.ToString() ?? "",
                                PO_GeneratedDateTime = sdr["PO_GeneratedDateTime"]?.ToString() ?? "",
                                PO_CreatedDateTime = sdr["PO_CreatedDateTime"]?.ToString() ?? "",
                                PO_UpdatedDateTime = sdr["PO_UpdatedDateTime"]?.ToString() ?? "",
                                PO_CompanyUnitId = Convert.IsDBNull(sdr["PO_CompanyUnitId"]) ? 0 : Convert.ToInt32(sdr["PO_CompanyUnitId"]),
                                PO_CreatedEmployeeId = Convert.IsDBNull(sdr["PO_CreatedEmployeeId"]) ? 0 : Convert.ToInt32(sdr["PO_CreatedEmployeeId"]),
                                PO_MasterCompanyId = Convert.IsDBNull(sdr["PO_MasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["PO_MasterCompanyId"]),
                                PO_StatusId = Convert.IsDBNull(sdr["PO_StatusId"]) ? 0 : Convert.ToInt32(sdr["PO_StatusId"]),
                                PO_IsActive = Convert.IsDBNull(sdr["PO_IsActive"]) ? 0 : Convert.ToInt32(sdr["PO_IsActive"]),
                                PO_IsNotificationSend = Convert.IsDBNull(sdr["PO_IsNotificationSend"]) ? 0 : Convert.ToInt32(sdr["PO_IsNotificationSend"]),
                                PO_AcceptQty = Convert.IsDBNull(sdr["PO_AcceptQty"]) ? 0 : Convert.ToDecimal(sdr["PO_AcceptQty"]),
                                PO_RejectQty = Convert.IsDBNull(sdr["PO_RejectQty"]) ? 0 : Convert.ToDecimal(sdr["PO_RejectQty"]),
                                PO_StatusForIT = Convert.IsDBNull(sdr["PO_StatusForIT"]) ? 0 : Convert.ToInt32(sdr["PO_StatusForIT"]),
                                PO_ShortFall = Convert.IsDBNull(sdr["PO_ShortFall"]) ? 0 : Convert.ToDecimal(sdr["PO_ShortFall"]),
                                PO_ReturnQty = Convert.IsDBNull(sdr["PO_ReturnQty"]) ? 0 : Convert.ToDecimal(sdr["PO_ReturnQty"]),
                                PO_Status_Title = sdr["StatusTitle"]?.ToString() ?? "",
                                VendorName = sdr["VendorName"]?.ToString() ?? "",
                                VendorCompanyName = sdr["VendorCompanyName"]?.ToString() ?? "",
                                ItemId = Convert.IsDBNull(sdr["ItemId"]) ? 0 : Convert.ToInt32(sdr["ItemId"]),
                                PO_ProjectID = sdr["PO_ProjectID"]?.ToString() ?? "",
                                PO_ApprovalStatus = sdr["PO_ApprovalStatus"]?.ToString() ?? "",
                                PO_VendorName = sdr["PO_VendorName"]?.ToString() ?? "",
                                PO_Unit = sdr["PO_Unit"]?.ToString() ?? "",
                                PO_ProductType = sdr["PO_ProductType"]?.ToString() ?? "",
                                PO_DeliverRemainder = sdr["PO_DeliverRemainder"]?.ToString() ?? "",
                                PO_UnitPrice = Convert.IsDBNull(sdr["PO_UnitPrice"]) ? 0 : Convert.ToDecimal(sdr["PO_UnitPrice"]),
                                PO_NetAmount = Convert.IsDBNull(sdr["PO_NetAmount"]) ? 0 : Convert.ToDecimal(sdr["PO_NetAmount"]),
                                PO_Site = sdr["PO_Site"]?.ToString() ?? "",
                                PO_Warehouse = sdr["PO_Warehouse"]?.ToString() ?? "",
                                PO_Location = sdr["PO_Location"]?.ToString() ?? "",
                                PO_PurchaseRequisition = sdr["PO_PurchaseRequisition"]?.ToString() ?? "",
                                PO_PurchaseLineCreatedDateTime = sdr["PO_PurchaseLineCreatedDateTime"]?.ToString() ?? "",
                                PO_ProjectCategory = sdr["PO_ProjectCategory"]?.ToString() ?? "",
                                PO_Currency = sdr["PO_Currency"]?.ToString() ?? "",
                                PO_TermsOfPayment = sdr["PO_TermsOfPayment"]?.ToString() ?? ""
                            };

                            items.Add(item);
                        }
                    }
                }
            }

            return items;
        }

        public List<PoExcelUploadMasterData> ExcelUploadPODetailsPaged(int pageIndex, int pageSize, int companyUnitId, string searchTerm, DateTime? startDate, DateTime? endDate, out int totalRecords)
        {
            List<PoExcelUploadMasterData> items = new List<PoExcelUploadMasterData>();
            totalRecords = 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string countQuery = @"SELECT COUNT(*) 
                   FROM PoExcelUploadMaster pum
                   INNER JOIN ItemMaster itm ON itm.ItemCode = pum.PO_ItemCode
                   INNER JOIN VendorDetailsMaster vdm ON vdm.VendorCode = pum.PO_VendorCode
                   INNER JOIN StatusMasterTB sm ON sm.StatusID = pum.PO_StatusId
                   WHERE pum.PO_MasterCompanyId = @CompanyId 
                   AND pum.PO_CompanyUnitId = @PO_CompanyUnitId
                   AND pum.PO_IsActive = 1
                   AND (
                        pum.PO_Number LIKE @Search OR 
                        pum.PO_ItemCode LIKE @Search OR 
                        pum.PO_ItemName LIKE @Search OR 
                        pum.PO_VendorCode LIKE @Search OR
                        vdm.VendorName LIKE @Search)
                   AND (@StartDate IS NULL OR CONVERT(datetime, pum.PO_CreatedDateTime, 120) >= @StartDate)
                   AND (@EndDate IS NULL OR CONVERT(datetime, pum.PO_CreatedDateTime, 120) <= @EndDate)";

                string dataQuery = @"SELECT pum.*, sm.StatusTitle, vdm.VendorName, vdm.VendorCompanyName, itm.ItemId
                  FROM PoExcelUploadMaster pum
                  INNER JOIN ItemMaster itm ON itm.ItemCode = pum.PO_ItemCode
                  INNER JOIN VendorDetailsMaster vdm ON vdm.VendorCode = pum.PO_VendorCode
                  INNER JOIN StatusMasterTB sm ON sm.StatusID = pum.PO_StatusId
                  WHERE pum.PO_MasterCompanyId = @CompanyId 
                  AND pum.PO_CompanyUnitId = @PO_CompanyUnitId
                  AND pum.PO_IsActive = 1
                  AND (
                       pum.PO_Number LIKE @Search OR 
                       pum.PO_ItemCode LIKE @Search OR 
                       pum.PO_ItemName LIKE @Search OR 
                       pum.PO_VendorCode LIKE @Search OR
                       vdm.VendorName LIKE @Search)
                  AND (@StartDate IS NULL OR CONVERT(datetime, pum.PO_CreatedDateTime, 120) >= @StartDate)
                  AND (@EndDate IS NULL OR CONVERT(datetime, pum.PO_CreatedDateTime, 120) <= @EndDate)
                  ORDER BY CONVERT(datetime, pum.PO_CreatedDateTime, 120) DESC
                  OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                using (SqlCommand countCmd = new SqlCommand(countQuery, con))
                using (SqlCommand dataCmd = new SqlCommand(dataQuery, con))
                {
                    string likeSearch = "%" + searchTerm + "%";
                    string companyId = HttpContext.Current.Session["MasterCompanyId"].ToString();

                    // Common parameters
                    countCmd.Parameters.AddWithValue("@Search", likeSearch);
                    countCmd.Parameters.AddWithValue("@CompanyId", companyId);
                    countCmd.Parameters.AddWithValue("@PO_CompanyUnitId", companyUnitId);

                    dataCmd.Parameters.AddWithValue("@Search", likeSearch);
                    dataCmd.Parameters.AddWithValue("@CompanyId", companyId);
                    dataCmd.Parameters.AddWithValue("@PO_CompanyUnitId", companyUnitId);
                    dataCmd.Parameters.AddWithValue("@Offset", (pageIndex - 1) * pageSize);
                    dataCmd.Parameters.AddWithValue("@PageSize", pageSize);

                    // Date parameters (handle nulls)
                    countCmd.Parameters.AddWithValue("@StartDate", startDate ?? (object)DBNull.Value);
                    countCmd.Parameters.AddWithValue("@EndDate", endDate ?? (object)DBNull.Value);

                    dataCmd.Parameters.AddWithValue("@StartDate", startDate ?? (object)DBNull.Value);
                    dataCmd.Parameters.AddWithValue("@EndDate", endDate ?? (object)DBNull.Value);

                    con.Open();
                    totalRecords = (int)countCmd.ExecuteScalar();

                    using (SqlDataReader sdr = dataCmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new PoExcelUploadMasterData
                            {
                                PO_Id = GetInt32(sdr, "PO_Id"),
                                PO_Number = GetString(sdr, "PO_Number"),
                                PO_ItemCode = GetString(sdr, "PO_ItemCode"),
                                PO_ItemName = GetString(sdr, "PO_ItemName"),
                                PO_OrderedQty = GetDecimal(sdr, "PO_OrderedQty"),
                                PO_ReceivedQty = GetDecimal(sdr, "PO_ReceivedQty"),
                                PO_GettingStock = GetDecimal(sdr, "PO_GettingStock"),
                                PO_BalanceQty = GetDecimal(sdr, "PO_BalanceQty"),
                                PO_BasicPrice = GetDecimal(sdr, "PO_BasicPrice"),
                                PO_GSTPercentage = GetDecimal(sdr, "PO_GSTPercentage"),
                                PO_VendorCode = GetString(sdr, "PO_VendorCode"),
                                PO_OrderedDateTime = GetString(sdr, "PO_OrderedDateTime"),
                                PO_DeliveryDateTime = GetString(sdr, "PO_DeliveryDateTime"),
                                PO_GeneratedDateTime = GetString(sdr, "PO_GeneratedDateTime"),
                                PO_CreatedDateTime = GetString(sdr, "PO_CreatedDateTime"),
                                PO_UpdatedDateTime = GetString(sdr, "PO_UpdatedDateTime"),
                                PO_CompanyUnitId = GetInt32(sdr, "PO_CompanyUnitId"),
                                PO_CreatedEmployeeId = GetInt32(sdr, "PO_CreatedEmployeeId"),
                                PO_MasterCompanyId = GetInt32(sdr, "PO_MasterCompanyId"),
                                PO_StatusId = GetInt32(sdr, "PO_StatusId"),
                                PO_IsActive = GetInt32(sdr, "PO_IsActive"),
                                PO_IsNotificationSend = GetInt32(sdr, "PO_IsNotificationSend"),
                                PO_AcceptQty = GetDecimal(sdr, "PO_AcceptQty"),
                                PO_RejectQty = GetDecimal(sdr, "PO_RejectQty"),
                                PO_StatusForIT = GetInt32(sdr, "PO_StatusForIT"),
                                PO_ShortFall = GetDecimal(sdr, "PO_ShortFall"),
                                PO_ReturnQty = GetDecimal(sdr, "PO_ReturnQty"),
                                PO_Status_Title = GetString(sdr, "StatusTitle"),
                                VendorName = GetString(sdr, "VendorName"),
                                VendorCompanyName = GetString(sdr, "VendorCompanyName"),
                                ItemId = GetInt32(sdr, "ItemId"),
                                PO_ProjectID = GetString(sdr, "PO_ProjectID"),
                                PO_ApprovalStatus = GetString(sdr, "PO_ApprovalStatus"),
                                PO_VendorName = GetString(sdr, "PO_VendorName"),
                                PO_Unit = GetString(sdr, "PO_Unit"),
                                PO_ProductType = GetString(sdr, "PO_ProductType"),
                                PO_DeliverRemainder = GetString(sdr, "PO_DeliverRemainder"),
                                PO_UnitPrice = GetDecimal(sdr, "PO_UnitPrice"),
                                PO_NetAmount = GetDecimal(sdr, "PO_NetAmount"),
                                PO_Site = GetString(sdr, "PO_Site"),
                                PO_Warehouse = GetString(sdr, "PO_Warehouse"),
                                PO_Location = GetString(sdr, "PO_Location"),
                                PO_PurchaseRequisition = GetString(sdr, "PO_PurchaseRequisition"),
                                PO_PurchaseLineCreatedDateTime = GetString(sdr, "PO_PurchaseLineCreatedDateTime"),
                                PO_ProjectCategory = GetString(sdr, "PO_ProjectCategory"),
                                PO_Currency = GetString(sdr, "PO_Currency"),
                                PO_TermsOfPayment = GetString(sdr, "PO_TermsOfPayment")
                            });
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }

        public List<PurchaseOrder> GetPurchaseOrdersDetailsData()
        {
            List<PurchaseOrder> orders = new List<PurchaseOrder>();

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"select DISTINCT PO_StatusForIT, po.PurchaseOrderID, PONumber, VendorID, Unit, SpecialInstructions, ReportFileName, GrandTotal, UnitID, Delivery, 
                                    Transportation, Payment, Guarantee, Remarks, ContactPersonID, PurchaseOrderStatus, Reason, POSeries, POyear, 
                                    StoreType, projectid, penaltyID, TotalVAT, AmountPaid from PurchaseOrder po
                                    INNER JOIN PurchaseOrderDetails pod ON pod.PurchaseOrderID = po.PurchaseOrderID
                                    where PurchaseOrderStatus in(6,15)";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            orders.Add(new PurchaseOrder
                            {
                                PO_StatusForIT = sdr["PO_StatusForIT"] != DBNull.Value ? Convert.ToInt32(sdr["PO_StatusForIT"]) : 0,
                                PurchaseOrderID = sdr["PurchaseOrderID"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseOrderID"]) : 0,
                                PONumber = sdr["PONumber"]?.ToString(),
                                //PODate = sdr["PODate"] != DBNull.Value ? Convert.ToDateTime(sdr["PODate"]) : (DateTime?)null,
                                VendorID = sdr["VendorID"] != DBNull.Value ? Convert.ToInt32(sdr["VendorID"]) : (int?)null,
                                Unit = sdr["Unit"]?.ToString(),
                                //CreatedDate = sdr["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(sdr["CreatedDate"]) : (DateTime?)null,
                                SpecialInstructions = sdr["SpecialInstructions"]?.ToString(),
                                ReportFileName = sdr["ReportFileName"]?.ToString(),
                                GrandTotal = sdr["GrandTotal"] != DBNull.Value ? Convert.ToDecimal(sdr["GrandTotal"]) : (decimal?)null,
                                UnitID = sdr["UnitID"] != DBNull.Value ? Convert.ToInt32(sdr["UnitID"]) : (int?)null,
                                Delivery = sdr["Delivery"]?.ToString(),
                                Transportation = sdr["Transportation"]?.ToString(),
                                Payment = sdr["Payment"]?.ToString(),
                                Guarantee = sdr["Guarantee"]?.ToString(),
                                Remarks = sdr["Remarks"]?.ToString(),
                                ContactPersonID = sdr["ContactPersonID"] != DBNull.Value ? Convert.ToInt32(sdr["ContactPersonID"]) : (int?)null,
                                PurchaseOrderStatus = sdr["PurchaseOrderStatus"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseOrderStatus"]) : (int?)null,
                                Reason = sdr["Reason"]?.ToString(),
                                POSeries = sdr["POSeries"]?.ToString(),
                                POyear = sdr["POyear"] != DBNull.Value ? Convert.ToInt32(sdr["POyear"]) : (int?)null,
                                StoreType = sdr["StoreType"]?.ToString(),
                                projectid = sdr["projectid"] != DBNull.Value ? Convert.ToInt32(sdr["projectid"]) : (int?)null,
                                penaltyID = sdr["penaltyID"] != DBNull.Value ? Convert.ToInt32(sdr["penaltyID"]) : (int?)null,
                            });
                        }
                    }
                    con.Close();
                }
            }

            return orders;
        }

        public List<PurchaseOrder> GetPurchaseOrdersData()
        {
            List<PurchaseOrder> orders = new List<PurchaseOrder>();

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM PurchaseOrder";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            orders.Add(new PurchaseOrder
                            {
                                PurchaseOrderID = sdr["PurchaseOrderID"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseOrderID"]) : 0,
                                PONumber = sdr["PONumber"]?.ToString(),
                                PODate = sdr["PODate"] != DBNull.Value ? Convert.ToDateTime(sdr["PODate"]) : (DateTime?)null,
                                VendorID = sdr["VendorID"] != DBNull.Value ? Convert.ToInt32(sdr["VendorID"]) : (int?)null,
                                Unit = sdr["Unit"]?.ToString(),
                                //CreatedDate = sdr["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(sdr["CreatedDate"]) : (DateTime?)null,
                                SpecialInstructions = sdr["SpecialInstructions"]?.ToString(),
                                ReportFileName = sdr["ReportFileName"]?.ToString(),
                                GrandTotal = sdr["GrandTotal"] != DBNull.Value ? Convert.ToDecimal(sdr["GrandTotal"]) : (decimal?)null,
                                UnitID = sdr["UnitID"] != DBNull.Value ? Convert.ToInt32(sdr["UnitID"]) : (int?)null,
                                Delivery = sdr["Delivery"]?.ToString(),
                                Transportation = sdr["Transportation"]?.ToString(),
                                Payment = sdr["Payment"]?.ToString(),
                                Guarantee = sdr["Guarantee"]?.ToString(),
                                Remarks = sdr["Remarks"]?.ToString(),
                                ContactPersonID = sdr["ContactPersonID"] != DBNull.Value ? Convert.ToInt32(sdr["ContactPersonID"]) : (int?)null,
                                PurchaseOrderStatus = sdr["PurchaseOrderStatus"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseOrderStatus"]) : (int?)null,
                                Reason = sdr["Reason"]?.ToString(),
                                POSeries = sdr["POSeries"]?.ToString(),
                                POyear = sdr["POyear"] != DBNull.Value ? Convert.ToInt32(sdr["POyear"]) : (int?)null,
                                StoreType = sdr["StoreType"]?.ToString(),
                                projectid = sdr["projectid"] != DBNull.Value ? Convert.ToInt32(sdr["projectid"]) : (int?)null,
                                penaltyID = sdr["penaltyID"] != DBNull.Value ? Convert.ToInt32(sdr["penaltyID"]) : (int?)null,                                
                            });
                        }
                    }
                    con.Close();
                }
            }

            return orders;
        }

        public List<PurchaseOrder> GetPurchaseOrderDetails()
        {
            List<PurchaseOrder> orderDetails = new List<PurchaseOrder>();

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM PurchaseOrderDetails";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            orderDetails.Add(new PurchaseOrder
                            {
                                PODetailsID = sdr["PODetailsID"] != DBNull.Value ? Convert.ToInt32(sdr["PODetailsID"]) : 0,
                                PurchaseOrderID = sdr["PurchaseOrderID"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseOrderID"]) : 0,
                                Discount = sdr["Discount"] != DBNull.Value ? Convert.ToDecimal(sdr["Discount"]) : (decimal?)null,
                                DiscountAmount = sdr["DiscountAmount"] != DBNull.Value ? Convert.ToDecimal(sdr["DiscountAmount"]) : (decimal?)null,
                                Tax = sdr["Tax"] != DBNull.Value ? Convert.ToDecimal(sdr["Tax"]) : (decimal?)null,
                                TotalAmount = sdr["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(sdr["TotalAmount"]) : (decimal?)null,
                                CreatedDate = sdr["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(sdr["CreatedDate"]) : (DateTime?)null,
                                DeliveryDate = sdr["DeliveryDate"] != DBNull.Value ? Convert.ToDateTime(sdr["DeliveryDate"]) : (DateTime?)null,
                                VendorQuotationID = sdr["VendorQuotationID"] != DBNull.Value ? Convert.ToInt32(sdr["VendorQuotationID"]) : (int?)null,
                                Quantity = sdr["Quantity"] != DBNull.Value ? Convert.ToDecimal(sdr["Quantity"]) : (decimal?)null,
                                Rate = sdr["Rate"] != DBNull.Value ? Convert.ToDecimal(sdr["Rate"]) : (decimal?)null,
                                Amount = sdr["Amount"] != DBNull.Value ? Convert.ToDecimal(sdr["Amount"]) : (decimal?)null,
                                ItemID = sdr["ItemID"] != DBNull.Value ? Convert.ToInt32(sdr["ItemID"]) : (int?)null,
                                POStatusID = sdr["POStatusID"] != DBNull.Value ? Convert.ToInt32(sdr["POStatusID"]) : (int?)null,
                                ReceivedQty = sdr["ReceivedQty"] != DBNull.Value ? Convert.ToDecimal(sdr["ReceivedQty"]) : (decimal?)null,
                                TransactionID = sdr["TransactionID"] != DBNull.Value ? Convert.ToInt32(sdr["TransactionID"]) : (int?)null,
                                AllocatedStock = sdr["AllocatedStock"] != DBNull.Value ? Convert.ToDecimal(sdr["AllocatedStock"]) : (decimal?)null,
                                ReceivedBy = sdr["ReceivedBy"] != DBNull.Value ? Convert.ToInt32(sdr["ReceivedBy"]) : (int?)null,
                                SecurityReceived = sdr["SecurityReceived"] != DBNull.Value ? Convert.ToDecimal(sdr["SecurityReceived"]) : (decimal?)null,
                                TaxAmount = sdr["TaxAmount"] != DBNull.Value ? Convert.ToDecimal(sdr["TaxAmount"]) : (decimal?)null,
                                VATID = sdr["VATID"] != DBNull.Value ? Convert.ToInt32(sdr["VATID"]) : (int?)null,
                                VendorQuotationDetailsID = sdr["VendorQuotationDetailsID"] != DBNull.Value ? Convert.ToInt32(sdr["VendorQuotationDetailsID"]) : (int?)null,
                                EnquiryDetailsID = sdr["EnquiryDetailsID"] != DBNull.Value ? Convert.ToInt32(sdr["EnquiryDetailsID"]) : (int?)null,
                                PurchaseComments = sdr["PurchaseComments"]?.ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }

            return orderDetails;
        }

        public List<PurchaseOrder> GetPurchaseOrderDetailsData()
        {
            List<PurchaseOrder> orderDetails = new List<PurchaseOrder>();

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM PurchaseOrder po
                                 INNER JOIN PurchaseOrderDetails pod ON pod.PurchaseOrderID = po.PurchaseOrderID
                                 INNER JOIN Items i ON i.ItemID = pod.ItemID
                                 LEFT JOIN ItemType IT ON I.ItemTypeID = IT.ItemTypeID
                                 INNER JOIN UOM um ON um.UOMId = i.UOMID
                                 INNER JOIN VAT vt on vt.VATID = i.VATID
                                 INNER JOIN InventoryMaster ivm ON ivm.InventoryItemId = i.ItemID
                                 INNER JOIN VendorDetails vd ON vd.VendorID = po.VendorID
                                 INNER JOIN RackTypeDetails rtd ON rtd.RackTypeId = ivm.InventoryRackTypeId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            orderDetails.Add(new PurchaseOrder
                            {
                                PurchaseOrderID = sdr["PurchaseOrderID"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseOrderID"]) : 0,
                                PONumber = sdr["PONumber"]?.ToString(),
                                PODate = sdr["PODate"] != DBNull.Value ? Convert.ToDateTime(sdr["PODate"]) : (DateTime?)null,
                                VendorID = sdr["VendorID"] != DBNull.Value ? Convert.ToInt32(sdr["VendorID"]) : (int?)null,
                                Unit = sdr["Unit"]?.ToString(),
                                CreatedDate = sdr["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(sdr["CreatedDate"]) : (DateTime?)null,
                                SpecialInstructions = sdr["SpecialInstructions"]?.ToString(),
                                ReportFileName = sdr["ReportFileName"]?.ToString(),
                                GrandTotal = sdr["GrandTotal"] != DBNull.Value ? Convert.ToDecimal(sdr["GrandTotal"]) : (decimal?)null,
                                UnitID = sdr["UnitID"] != DBNull.Value ? Convert.ToInt32(sdr["UnitID"]) : (int?)null,
                                Delivery = sdr["Delivery"]?.ToString(),
                                Transportation = sdr["Transportation"]?.ToString(),
                                Payment = sdr["Payment"]?.ToString(),
                                Guarantee = sdr["Guarantee"]?.ToString(),
                                Remarks = sdr["Remarks"]?.ToString(),
                                ContactPersonID = sdr["ContactPersonID"] != DBNull.Value ? Convert.ToInt32(sdr["ContactPersonID"]) : (int?)null,
                                PurchaseOrderStatus = sdr["PurchaseOrderStatus"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseOrderStatus"]) : (int?)null,
                                Reason = sdr["Reason"]?.ToString(),
                                POSeries = sdr["POSeries"]?.ToString(),
                                POyear = sdr["POyear"] != DBNull.Value ? Convert.ToInt32(sdr["POyear"]) : (int?)null,
                                StoreType = sdr["StoreType"]?.ToString(),
                                projectid = sdr["projectid"] != DBNull.Value ? Convert.ToInt32(sdr["projectid"]) : (int?)null,
                                penaltyID = sdr["penaltyID"] != DBNull.Value ? Convert.ToInt32(sdr["penaltyID"]) : (int?)null,
                                PODetailsID = sdr["PODetailsID"] != DBNull.Value ? Convert.ToInt32(sdr["PODetailsID"]) : 0,
                                Discount = sdr["Discount"] != DBNull.Value ? Convert.ToDecimal(sdr["Discount"]) : (decimal?)null,
                                DiscountAmount = sdr["DiscountAmount"] != DBNull.Value ? Convert.ToDecimal(sdr["DiscountAmount"]) : (decimal?)null,
                                Tax = sdr["Tax"] != DBNull.Value ? Convert.ToDecimal(sdr["Tax"]) : (decimal?)null,
                                TotalAmount = sdr["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(sdr["TotalAmount"]) : (decimal?)null,
                                DeliveryDate = sdr["DeliveryDate"] != DBNull.Value ? Convert.ToDateTime(sdr["DeliveryDate"]) : (DateTime?)null,
                                VendorQuotationID = sdr["VendorQuotationID"] != DBNull.Value ? Convert.ToInt32(sdr["VendorQuotationID"]) : (int?)null,
                                Quantity = sdr["Quantity"] != DBNull.Value ? Convert.ToDecimal(sdr["Quantity"]) : (decimal?)null,
                                Rate = sdr["Rate"] != DBNull.Value ? Convert.ToDecimal(sdr["Rate"]) : (decimal?)null,
                                Amount = sdr["Amount"] != DBNull.Value ? Convert.ToDecimal(sdr["Amount"]) : (decimal?)null,
                                ItemID = sdr["ItemID"] != DBNull.Value ? Convert.ToInt32(sdr["ItemID"]) : (int?)null,
                                POStatusID = sdr["POStatusID"] != DBNull.Value ? Convert.ToInt32(sdr["POStatusID"]) : (int?)null,
                                ReceivedQty = sdr["ReceivedQty"] != DBNull.Value ? Convert.ToDecimal(sdr["ReceivedQty"]) : (decimal?)null,
                                TransactionID = sdr["TransactionID"] != DBNull.Value ? Convert.ToInt32(sdr["TransactionID"]) : (int?)null,
                                AllocatedStock = sdr["AllocatedStock"] != DBNull.Value ? Convert.ToDecimal(sdr["AllocatedStock"]) : (decimal?)null,
                                ReceivedBy = sdr["ReceivedBy"] != DBNull.Value ? Convert.ToInt32(sdr["ReceivedBy"]) : (int?)null,
                                SecurityReceived = sdr["SecurityReceived"] != DBNull.Value ? Convert.ToDecimal(sdr["SecurityReceived"]) : (decimal?)null,
                                TaxAmount = sdr["TaxAmount"] != DBNull.Value ? Convert.ToDecimal(sdr["TaxAmount"]) : (decimal?)null,
                                VATID = sdr["VATID"] != DBNull.Value ? Convert.ToInt32(sdr["VATID"]) : (int?)null,
                                VendorQuotationDetailsID = sdr["VendorQuotationDetailsID"] != DBNull.Value ? Convert.ToInt32(sdr["VendorQuotationDetailsID"]) : (int?)null,
                                EnquiryDetailsID = sdr["EnquiryDetailsID"] != DBNull.Value ? Convert.ToInt32(sdr["EnquiryDetailsID"]) : (int?)null,
                                PurchaseComments = sdr["PurchaseComments"]?.ToString(),


                                PO_OrderedQty = sdr["PO_OrderedQty"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_OrderedQty"]) : 0,
                                PO_ReceivedQty = sdr["PO_ReceivedQty"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_ReceivedQty"]) : 0,
                                PO_GettingStock = sdr["PO_GettingStock"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_GettingStock"]) : 0,
                                PO_BalanceQty = sdr["PO_BalanceQty"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_BalanceQty"]) : 0,
                                PO_BasicPrice = sdr["PO_BasicPrice"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_BasicPrice"]) : 0,
                                PO_StatusId = sdr["PO_StatusId"] != DBNull.Value ? Convert.ToInt32(sdr["PO_StatusId"]) : 0,
                                PO_IsActive = sdr["PO_IsActive"] != DBNull.Value ? Convert.ToInt32(sdr["PO_IsActive"]) : 0,
                                PO_AcceptQty = sdr["PO_AcceptQty"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_AcceptQty"]) : 0,
                                PO_RejectQty = sdr["PO_RejectQty"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_RejectQty"]) : 0,
                                PO_StatusForIT = sdr["PO_StatusForIT"] != DBNull.Value ? Convert.ToInt32(sdr["PO_StatusForIT"]) : 0,
                                PO_ShortFall = sdr["PO_ShortFall"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_ShortFall"]) : 0,
                                PO_ReturnQty = sdr["PO_ReturnQty"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_ReturnQty"]) : 0,
                                PO_IsNotificationSend = Convert.IsDBNull(sdr["PO_IsNotificationSend"]) ? 0 : Convert.ToInt32(sdr["PO_IsNotificationSend"]),


                                ItemCode = sdr["ItemCode"] != DBNull.Value ? sdr["ItemCode"].ToString() : string.Empty,
                                ItemName = sdr["ItemName"] != DBNull.Value ? sdr["ItemName"].ToString() : string.Empty,
                                ItemDescription = sdr["ItemDescription"] != DBNull.Value ? sdr["ItemDescription"].ToString() : string.Empty,
                                PurchaseSubGroupID = sdr["PurchaseSubGroupID"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseSubGroupID"]) : 0,
                                LeadTime = sdr["LeadTime"] != DBNull.Value ? Convert.ToInt32(sdr["LeadTime"]) : 0,
                                UOMID = sdr["UOMID"] != DBNull.Value ? Convert.ToInt32(sdr["UOMID"]) : 0,
                                StockQuantity = sdr["StockQuantity"] != DBNull.Value ? Convert.ToDecimal(sdr["StockQuantity"]) : 0,
                                MinStock = sdr["MinStock"] != DBNull.Value ? Convert.ToDecimal(sdr["MinStock"]) : 0,
                                MaxStock = sdr["MaxStock"] != DBNull.Value ? Convert.ToDecimal(sdr["MaxStock"]) : 0,
                                DisabledItem = sdr["DisabledItem"] != DBNull.Value ? Convert.ToBoolean(sdr["DisabledItem"]) : false,
                                HSNCode = sdr["HSNCode"] != DBNull.Value ? sdr["HSNCode"].ToString() : string.Empty,
                                VAT = sdr["VAT"] != DBNull.Value ? Convert.ToDecimal(sdr["VAT"]) : 0,
                                UOMValue = sdr["UOMValue"] != DBNull.Value ? Convert.ToDecimal(sdr["UOMValue"]) : 0,
                                ConversionUOMID = sdr["ConversionUOMID"] != DBNull.Value ? Convert.ToInt32(sdr["ConversionUOMID"]) : 0,
                                ConversionUOMValue = sdr["ConversionUOMValue"] != DBNull.Value ? Convert.ToDecimal(sdr["ConversionUOMValue"]) : 0,
                                ModelNo = sdr["ModelNo"] != DBNull.Value ? sdr["ModelNo"].ToString() : string.Empty,
                                ItemTypeID = sdr["ItemTypeID"] != DBNull.Value ? Convert.ToInt32(sdr["ItemTypeID"]) : 0,
                                Description = sdr["Description"] != DBNull.Value ? sdr["Description"].ToString() : string.Empty,
                                Specification = sdr["Specification"] != DBNull.Value ? sdr["Specification"].ToString() : string.Empty,
                                PurchaseGroupID = sdr["PurchaseGroupID"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseGroupID"]) : 0,
                                CreatedBy = sdr["CreatedBy"] != DBNull.Value ? Convert.ToInt32(sdr["CreatedBy"]) : 0,
                                ConvertUOM = sdr["ConvertUOM"] != DBNull.Value ? Convert.ToInt32(sdr["ConvertUOM"]) : 0,
                                ItemTypeName = sdr["ItemType"] != DBNull.Value ? sdr["ItemType"].ToString() : string.Empty,

                                InventoryId = sdr["InventoryId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryId"]) : 0,
                                InventoryRackTypeId = sdr["InventoryRackTypeId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryRackTypeId"]) : 0,
                                InventoryItemCategoryId = sdr["InventoryItemCategoryId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryItemCategoryId"]) : 0,
                                InventoryItemId = sdr["InventoryItemId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryItemId"]) : 0,
                                InventoryUOMId = sdr["InventoryUOMId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryUOMId"]) : 0,
                                InventoryGSTId = sdr["InventoryGSTId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryGSTId"]) : 0,
                                InventoryCurrentStock = sdr["InventoryCurrentStock"] != DBNull.Value ? Convert.ToDecimal(sdr["InventoryCurrentStock"]) : 0,
                                InventoryMinStock = sdr["InventoryMinStock"] != DBNull.Value ? Convert.ToDecimal(sdr["InventoryMinStock"]) : 0,
                                InventoryCreatedEmployeeId = sdr["InventoryCreatedEmployeeId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryCreatedEmployeeId"]) : 0,
                                InventoryMasterCompanyId = sdr["InventoryMasterCompanyId"] != DBNull.Value ? Convert.ToInt32(sdr["InventoryMasterCompanyId"]) : 0,
                                InventoryItemLocationStore = sdr["InventoryItemLocationStore"] != DBNull.Value ? sdr["InventoryItemLocationStore"].ToString() : string.Empty,
                                InventoryCreatedDateTime = sdr["InventoryCreatedDateTime"] != DBNull.Value ? sdr["InventoryCreatedDateTime"].ToString() : string.Empty,
                                InventoryUpdatedDateTime = sdr["InventoryUpdatedDateTime"] != DBNull.Value ? sdr["InventoryUpdatedDateTime"].ToString() : string.Empty,

                                UOMId = sdr["UOMId"] != DBNull.Value ? Convert.ToInt32(sdr["UOMId"]) : 0,
                                UOMCode = sdr["UOMName"] != DBNull.Value ? sdr["UOMName"].ToString() : string.Empty,
                                UOMName = sdr["UOMName"] != DBNull.Value ? sdr["UOMName"].ToString() : string.Empty,

                                VATCode = sdr["VATCode"] != DBNull.Value ? sdr["VATCode"].ToString() : string.Empty,
                                Percentage = sdr["Percentage"] != DBNull.Value ? Convert.ToDecimal(sdr["Percentage"]) : 0,

                                RackTypeId = sdr["RackTypeId"] != DBNull.Value ? Convert.ToInt32(sdr["RackTypeId"]) : 0,
                                RackType = sdr["RackType"] != DBNull.Value ? sdr["RackType"].ToString() : string.Empty,
                                RackMasterCompanyId = sdr["RackMasterCompanyId"] != DBNull.Value ? Convert.ToInt32(sdr["RackMasterCompanyId"]) : 0,
                                RackCreatedEmployeeId = sdr["RackCreatedEmployeeId"] != DBNull.Value ? Convert.ToInt32(sdr["RackCreatedEmployeeId"]) : 0,
                                RackCreatedDateTime = sdr["RackCreatedDateTime"] != DBNull.Value ? sdr["RackCreatedDateTime"].ToString() : string.Empty,
                                RackUpdatedDateTime = sdr["RackUpdatedDateTime"] != DBNull.Value ? sdr["RackUpdatedDateTime"].ToString() : string.Empty,


                                VendorCode = sdr["VendorCode"]?.ToString(),
                                VendorName = sdr["VendorName"]?.ToString(),
                                DealerID = sdr["DealerID"] != DBNull.Value ? Convert.ToInt32(sdr["DealerID"]) : (int?)null,
                                Website = sdr["Website"]?.ToString(),
                                //VAT = sdr["VAT"].ToString(),
                                //CreatedDate = sdr["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(sdr["CreatedDate"]) : (DateTime?)null,
                                YearOfEstablishment = sdr["YearOfEstablishment"]?.ToString(),
                                TypeOfBusiness = sdr["TypeOfBusiness"]?.ToString(),
                                CompanyTradeLicence = sdr["CompanyTradeLicence"]?.ToString(),
                                YearlyTurnover1 = sdr["YearlyTurnover1"] != DBNull.Value ? Convert.ToDecimal(sdr["YearlyTurnover1"]) : (decimal?)null,
                                RequiredCredit = sdr["RequiredCredit"] != DBNull.Value ? Convert.ToDecimal(sdr["RequiredCredit"]) : (decimal?)null,
                                ApprovedCredit = sdr["ApprovedCredit"] != DBNull.Value ? Convert.ToDecimal(sdr["ApprovedCredit"]) : (decimal?)null,
                                PanNumber = sdr["PanNumber"]?.ToString(),
                                AdharNumber = sdr["AdharNumber"]?.ToString(),
                                EstablishmentID = sdr["EstablishmentID"] != DBNull.Value ? Convert.ToInt32(sdr["EstablishmentID"]) : (int?)null,
                                RegistrationNumber = sdr["RegistrationNumber"]?.ToString(),
                                YearlyTurnover2 = sdr["YearlyTurnover2"] != DBNull.Value ? Convert.ToDecimal(sdr["YearlyTurnover2"]) : (decimal?)null,
                                YearlyTurnover3 = sdr["YearlyTurnover3"] != DBNull.Value ? Convert.ToDecimal(sdr["YearlyTurnover3"]) : (decimal?)null,
                                Year1 = sdr["Year1"]?.ToString(),
                                Year2 = sdr["Year2"]?.ToString(),
                                Year3 = sdr["Year3"]?.ToString(),
                                UANNumber = sdr["UANNumber"]?.ToString(),
                                PFNumber = sdr["PFNumber"]?.ToString(),
                                NumberofEmployee = sdr["NumberofEmployee"].ToString(),
                                IEC = sdr["IEC"]?.ToString(),
                                ESIC = sdr["ESIC"]?.ToString(),
                                VendorStatus = sdr["VendorStatus"].ToString(),
                                VendorCurrency = sdr["VendorCurrency"]?.ToString(),
                                //PurchaseSubGroupID = sdr["PurchaseSubGroupID"].ToString(),
                                CompanySizeID = sdr["CompanySizeID"].ToString(),
                                //PurchaseGroupID = sdr["PurchaseGroupID"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }

            return orderDetails;
        }

        public List<PurchaseOrder> GetPurchaseOrderDetailsDataForStore()
        {
            List<PurchaseOrder> orderDetails = new List<PurchaseOrder>();

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM PurchaseOrder po
                                 INNER JOIN PurchaseOrderDetails pod ON pod.PurchaseOrderID = po.PurchaseOrderID
                                 INNER JOIN Items i ON i.ItemID = pod.ItemID                                 
                                 INNER JOIN VendorDetails vd ON vd.VendorID = po.VendorID
								 INNER JOIN StatusMasterTB sm ON sm.StatusID = pod.PoStatusId
                                 ORDER BY CONVERT(datetime, pod.CreatedDate, 120) DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            orderDetails.Add(new PurchaseOrder
                            {
                                PurchaseOrderID = sdr["PurchaseOrderID"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseOrderID"]) : 0,
                                PONumber = sdr["PONumber"]?.ToString(),
                                PODate = sdr["PODate"] != DBNull.Value ? Convert.ToDateTime(sdr["PODate"]) : (DateTime?)null,
                                VendorID = sdr["VendorID"] != DBNull.Value ? Convert.ToInt32(sdr["VendorID"]) : (int?)null,
                                Unit = sdr["Unit"]?.ToString(),
                                CreatedDate = sdr["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(sdr["CreatedDate"]) : (DateTime?)null,
                                SpecialInstructions = sdr["SpecialInstructions"]?.ToString(),
                                ReportFileName = sdr["ReportFileName"]?.ToString(),
                                GrandTotal = sdr["GrandTotal"] != DBNull.Value ? Convert.ToDecimal(sdr["GrandTotal"]) : (decimal?)null,
                                UnitID = sdr["UnitID"] != DBNull.Value ? Convert.ToInt32(sdr["UnitID"]) : (int?)null,
                                Delivery = sdr["Delivery"]?.ToString(),
                                Transportation = sdr["Transportation"]?.ToString(),
                                Payment = sdr["Payment"]?.ToString(),
                                Guarantee = sdr["Guarantee"]?.ToString(),
                                Remarks = sdr["Remarks"]?.ToString(),
                                ContactPersonID = sdr["ContactPersonID"] != DBNull.Value ? Convert.ToInt32(sdr["ContactPersonID"]) : (int?)null,
                                PurchaseOrderStatus = sdr["PurchaseOrderStatus"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseOrderStatus"]) : (int?)null,
                                Reason = sdr["Reason"]?.ToString(),
                                POSeries = sdr["POSeries"]?.ToString(),
                                POyear = sdr["POyear"] != DBNull.Value ? Convert.ToInt32(sdr["POyear"]) : (int?)null,
                                StoreType = sdr["StoreType"]?.ToString(),
                                projectid = sdr["projectid"] != DBNull.Value ? Convert.ToInt32(sdr["projectid"]) : (int?)null,
                                penaltyID = sdr["penaltyID"] != DBNull.Value ? Convert.ToInt32(sdr["penaltyID"]) : (int?)null,
                                PODetailsID = sdr["PODetailsID"] != DBNull.Value ? Convert.ToInt32(sdr["PODetailsID"]) : 0,
                                Discount = sdr["Discount"] != DBNull.Value ? Convert.ToDecimal(sdr["Discount"]) : (decimal?)null,
                                DiscountAmount = sdr["DiscountAmount"] != DBNull.Value ? Convert.ToDecimal(sdr["DiscountAmount"]) : (decimal?)null,
                                Tax = sdr["Tax"] != DBNull.Value ? Convert.ToDecimal(sdr["Tax"]) : (decimal?)null,
                                TotalAmount = sdr["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(sdr["TotalAmount"]) : (decimal?)null,
                                DeliveryDate = sdr["DeliveryDate"] != DBNull.Value ? Convert.ToDateTime(sdr["DeliveryDate"]) : (DateTime?)null,
                                VendorQuotationID = sdr["VendorQuotationID"] != DBNull.Value ? Convert.ToInt32(sdr["VendorQuotationID"]) : (int?)null,
                                Quantity = sdr["Quantity"] != DBNull.Value ? Convert.ToDecimal(sdr["Quantity"]) : (decimal?)null,
                                Rate = sdr["Rate"] != DBNull.Value ? Convert.ToDecimal(sdr["Rate"]) : (decimal?)null,
                                Amount = sdr["Amount"] != DBNull.Value ? Convert.ToDecimal(sdr["Amount"]) : (decimal?)null,
                                ItemID = sdr["ItemID"] != DBNull.Value ? Convert.ToInt32(sdr["ItemID"]) : (int?)null,
                                POStatusID = sdr["POStatusID"] != DBNull.Value ? Convert.ToInt32(sdr["POStatusID"]) : (int?)null,
                                ReceivedQty = sdr["ReceivedQty"] != DBNull.Value ? Convert.ToDecimal(sdr["ReceivedQty"]) : (decimal?)null,
                                TransactionID = sdr["TransactionID"] != DBNull.Value ? Convert.ToInt32(sdr["TransactionID"]) : (int?)null,
                                AllocatedStock = sdr["AllocatedStock"] != DBNull.Value ? Convert.ToDecimal(sdr["AllocatedStock"]) : (decimal?)null,
                                ReceivedBy = sdr["ReceivedBy"] != DBNull.Value ? Convert.ToInt32(sdr["ReceivedBy"]) : (int?)null,
                                SecurityReceived = sdr["SecurityReceived"] != DBNull.Value ? Convert.ToDecimal(sdr["SecurityReceived"]) : (decimal?)null,
                                TaxAmount = sdr["TaxAmount"] != DBNull.Value ? Convert.ToDecimal(sdr["TaxAmount"]) : (decimal?)null,
                                VATID = sdr["VATID"] != DBNull.Value ? Convert.ToInt32(sdr["VATID"]) : (int?)null,
                                VendorQuotationDetailsID = sdr["VendorQuotationDetailsID"] != DBNull.Value ? Convert.ToInt32(sdr["VendorQuotationDetailsID"]) : (int?)null,
                                EnquiryDetailsID = sdr["EnquiryDetailsID"] != DBNull.Value ? Convert.ToInt32(sdr["EnquiryDetailsID"]) : (int?)null,
                                PurchaseComments = sdr["PurchaseComments"]?.ToString(),


                                PO_OrderedQty = sdr["PO_OrderedQty"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_OrderedQty"]) : 0,
                                PO_ReceivedQty = sdr["PO_ReceivedQty"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_ReceivedQty"]) : 0,
                                PO_GettingStock = sdr["PO_GettingStock"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_GettingStock"]) : 0,
                                PO_BalanceQty = sdr["PO_BalanceQty"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_BalanceQty"]) : 0,
                                PO_BasicPrice = sdr["PO_BasicPrice"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_BasicPrice"]) : 0,
                                PO_StatusId = sdr["PO_StatusId"] != DBNull.Value ? Convert.ToInt32(sdr["PO_StatusId"]) : 0,
                                PO_IsActive = sdr["PO_IsActive"] != DBNull.Value ? Convert.ToInt32(sdr["PO_IsActive"]) : 0,
                                PO_AcceptQty = sdr["PO_AcceptQty"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_AcceptQty"]) : 0,
                                PO_RejectQty = sdr["PO_RejectQty"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_RejectQty"]) : 0,
                                PO_StatusForIT = sdr["PO_StatusForIT"] != DBNull.Value ? Convert.ToInt32(sdr["PO_StatusForIT"]) : 0,
                                PO_ShortFall = sdr["PO_ShortFall"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_ShortFall"]) : 0,
                                PO_ReturnQty = sdr["PO_ReturnQty"] != DBNull.Value ? Convert.ToDecimal(sdr["PO_ReturnQty"]) : 0,
                                PO_IsNotificationSend = Convert.IsDBNull(sdr["PO_IsNotificationSend"]) ? 0 : Convert.ToInt32(sdr["PO_IsNotificationSend"]),


                                ItemCode = sdr["ItemCode"] != DBNull.Value ? sdr["ItemCode"].ToString() : string.Empty,
                                ItemName = sdr["ItemName"] != DBNull.Value ? sdr["ItemName"].ToString() : string.Empty,
                                ItemDescription = sdr["ItemDescription"] != DBNull.Value ? sdr["ItemDescription"].ToString() : string.Empty,
                                PurchaseSubGroupID = sdr["PurchaseSubGroupID"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseSubGroupID"]) : 0,
                                LeadTime = sdr["LeadTime"] != DBNull.Value ? Convert.ToInt32(sdr["LeadTime"]) : 0,
                                UOMID = sdr["UOMID"] != DBNull.Value ? Convert.ToInt32(sdr["UOMID"]) : 0,
                                StockQuantity = sdr["StockQuantity"] != DBNull.Value ? Convert.ToDecimal(sdr["StockQuantity"]) : 0,
                                MinStock = sdr["MinStock"] != DBNull.Value ? Convert.ToDecimal(sdr["MinStock"]) : 0,
                                MaxStock = sdr["MaxStock"] != DBNull.Value ? Convert.ToDecimal(sdr["MaxStock"]) : 0,
                                DisabledItem = sdr["DisabledItem"] != DBNull.Value ? Convert.ToBoolean(sdr["DisabledItem"]) : false,
                                HSNCode = sdr["HSNCode"] != DBNull.Value ? sdr["HSNCode"].ToString() : string.Empty,
                                VAT = sdr["VAT"] != DBNull.Value ? Convert.ToDecimal(sdr["VAT"]) : 0,
                                UOMValue = sdr["UOMValue"] != DBNull.Value ? Convert.ToDecimal(sdr["UOMValue"]) : 0,
                                ConversionUOMID = sdr["ConversionUOMID"] != DBNull.Value ? Convert.ToInt32(sdr["ConversionUOMID"]) : 0,
                                ConversionUOMValue = sdr["ConversionUOMValue"] != DBNull.Value ? Convert.ToDecimal(sdr["ConversionUOMValue"]) : 0,
                                ModelNo = sdr["ModelNo"] != DBNull.Value ? sdr["ModelNo"].ToString() : string.Empty,
                                ItemTypeID = sdr["ItemTypeID"] != DBNull.Value ? Convert.ToInt32(sdr["ItemTypeID"]) : 0,
                                Description = sdr["Description"] != DBNull.Value ? sdr["Description"].ToString() : string.Empty,
                                Specification = sdr["Specification"] != DBNull.Value ? sdr["Specification"].ToString() : string.Empty,
                                PurchaseGroupID = sdr["PurchaseGroupID"] != DBNull.Value ? Convert.ToInt32(sdr["PurchaseGroupID"]) : 0,
                                CreatedBy = sdr["CreatedBy"] != DBNull.Value ? Convert.ToInt32(sdr["CreatedBy"]) : 0,
                                ConvertUOM = sdr["ConvertUOM"] != DBNull.Value ? Convert.ToInt32(sdr["ConvertUOM"]) : 0,
                                //ItemTypeName = sdr["ItemType"] != DBNull.Value ? sdr["ItemType"].ToString() : string.Empty,


                                VendorCode = sdr["VendorCode"]?.ToString(),
                                VendorName = sdr["VendorName"]?.ToString(),
                                DealerID = sdr["DealerID"] != DBNull.Value ? Convert.ToInt32(sdr["DealerID"]) : (int?)null,
                                Website = sdr["Website"]?.ToString(),
                                //VAT = sdr["VAT"].ToString(),
                                //CreatedDate = sdr["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(sdr["CreatedDate"]) : (DateTime?)null,
                                YearOfEstablishment = sdr["YearOfEstablishment"]?.ToString(),
                                TypeOfBusiness = sdr["TypeOfBusiness"]?.ToString(),
                                CompanyTradeLicence = sdr["CompanyTradeLicence"]?.ToString(),
                                YearlyTurnover1 = sdr["YearlyTurnover1"] != DBNull.Value ? Convert.ToDecimal(sdr["YearlyTurnover1"]) : (decimal?)null,
                                RequiredCredit = sdr["RequiredCredit"] != DBNull.Value ? Convert.ToDecimal(sdr["RequiredCredit"]) : (decimal?)null,
                                ApprovedCredit = sdr["ApprovedCredit"] != DBNull.Value ? Convert.ToDecimal(sdr["ApprovedCredit"]) : (decimal?)null,
                                PanNumber = sdr["PanNumber"]?.ToString(),
                                AdharNumber = sdr["AdharNumber"]?.ToString(),
                                EstablishmentID = sdr["EstablishmentID"] != DBNull.Value ? Convert.ToInt32(sdr["EstablishmentID"]) : (int?)null,
                                RegistrationNumber = sdr["RegistrationNumber"]?.ToString(),
                                YearlyTurnover2 = sdr["YearlyTurnover2"] != DBNull.Value ? Convert.ToDecimal(sdr["YearlyTurnover2"]) : (decimal?)null,
                                YearlyTurnover3 = sdr["YearlyTurnover3"] != DBNull.Value ? Convert.ToDecimal(sdr["YearlyTurnover3"]) : (decimal?)null,
                                Year1 = sdr["Year1"]?.ToString(),
                                Year2 = sdr["Year2"]?.ToString(),
                                Year3 = sdr["Year3"]?.ToString(),
                                UANNumber = sdr["UANNumber"]?.ToString(),
                                PFNumber = sdr["PFNumber"]?.ToString(),
                                NumberofEmployee = sdr["NumberofEmployee"].ToString(),
                                IEC = sdr["IEC"]?.ToString(),
                                ESIC = sdr["ESIC"]?.ToString(),
                                VendorStatus = sdr["VendorStatus"].ToString(),
                                VendorCurrency = sdr["VendorCurrency"]?.ToString(),
                                //PurchaseSubGroupID = sdr["PurchaseSubGroupID"].ToString(),
                                CompanySizeID = sdr["CompanySizeID"].ToString(),
                                //PurchaseGroupID = sdr["PurchaseGroupID"].ToString(),


                                StatusID = sdr["StatusID"] != DBNull.Value ? Convert.ToInt32(sdr["StatusID"]) : 0,
                                StatusTitle = sdr["StatusTitle"] != DBNull.Value ? sdr["StatusTitle"].ToString() : string.Empty,

                            });
                        }
                    }
                    con.Close();
                }
            }

            return orderDetails;
        }

        public List<VendorDetails> GetVendorDetails()
        {
            List<VendorDetails> vendors = new List<VendorDetails>();

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT VendorID, VendorCode, VendorName, DealerID, Website, VAT, CreatedDate, YearOfEstablishment, 
                        TypeOfBusiness, CompanyTradeLicence, YearlyTurnover1, RequiredCredit, ApprovedCredit, PanNumber, 
                        AdharNumber, EstablishmentID, RegistrationNumber, YearlyTurnover2, YearlyTurnover3, Year1, Year2, 
                        Year3, UANNumber, PFNumber, NumberofEmployee, IEC, ESIC, VendorStatus, VendorCurrency, 
                        PurchaseSubGroupID, CompanySizeID, PurchaseGroupID 
                        FROM VendorDetails";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            var vendor = new VendorDetails
                            {
                                VendorID = Convert.ToInt32(sdr["VendorID"]),
                                VendorCode = sdr["VendorCode"]?.ToString(),
                                VendorName = sdr["VendorName"]?.ToString(),
                                DealerID = sdr["DealerID"] != DBNull.Value ? Convert.ToInt32(sdr["DealerID"]) : (int?)null,
                                Website = sdr["Website"]?.ToString(),
                                VAT = sdr["VAT"]?.ToString(),
                                CreatedDate = sdr["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(sdr["CreatedDate"]) : (DateTime?)null,
                                YearOfEstablishment = sdr["YearOfEstablishment"]?.ToString(),
                                TypeOfBusiness = sdr["TypeOfBusiness"]?.ToString(),
                                CompanyTradeLicence = sdr["CompanyTradeLicence"]?.ToString(),
                                YearlyTurnover1 = sdr["YearlyTurnover1"] != DBNull.Value ? Convert.ToDecimal(sdr["YearlyTurnover1"]) : (decimal?)null,
                                RequiredCredit = sdr["RequiredCredit"] != DBNull.Value ? Convert.ToDecimal(sdr["RequiredCredit"]) : (decimal?)null,
                                ApprovedCredit = sdr["ApprovedCredit"] != DBNull.Value ? Convert.ToDecimal(sdr["ApprovedCredit"]) : (decimal?)null,
                                PanNumber = sdr["PanNumber"]?.ToString(),
                                AdharNumber = sdr["AdharNumber"]?.ToString(),
                                EstablishmentID = sdr["EstablishmentID"] != DBNull.Value ? Convert.ToInt32(sdr["EstablishmentID"]) : (int?)null,
                                RegistrationNumber = sdr["RegistrationNumber"]?.ToString(),
                                YearlyTurnover2 = sdr["YearlyTurnover2"] != DBNull.Value ? Convert.ToDecimal(sdr["YearlyTurnover2"]) : (decimal?)null,
                                YearlyTurnover3 = sdr["YearlyTurnover3"] != DBNull.Value ? Convert.ToDecimal(sdr["YearlyTurnover3"]) : (decimal?)null,
                                Year1 = sdr["Year1"]?.ToString(),
                                Year2 = sdr["Year2"]?.ToString(),
                                Year3 = sdr["Year3"]?.ToString(),
                                UANNumber = sdr["UANNumber"]?.ToString(),
                                PFNumber = sdr["PFNumber"]?.ToString(),
                                NumberofEmployee = sdr["NumberofEmployee"].ToString(),
                                IEC = sdr["IEC"]?.ToString(),
                                ESIC = sdr["ESIC"]?.ToString(),
                                VendorStatus = sdr["VendorStatus"].ToString(),
                                VendorCurrency = sdr["VendorCurrency"]?.ToString(),
                                PurchaseSubGroupID = sdr["PurchaseSubGroupID"].ToString(),
                                CompanySizeID = sdr["CompanySizeID"].ToString(),
                                PurchaseGroupID = sdr["PurchaseGroupID"].ToString(),
                            };

                            vendor.VendorNameAndCompanyName = $"{vendor.VendorName} - {vendor.VendorCode}";
                            vendors.Add(vendor);
                        }
                    }
                    con.Close();
                }
            }

            return vendors;
        }


        // Helper methods for safe data reading
        private int GetInt32(SqlDataReader reader, string columnName)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? 0 : reader.GetInt32(ordinal);
            }
            catch
            {
                return 0;
            }
        }

        private decimal GetDecimal(SqlDataReader reader, string columnName)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? 0 : reader.GetDecimal(ordinal);
            }
            catch
            {
                return 0;
            }
        }

        private string GetString(SqlDataReader reader, string columnName)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);
            }
            catch
            {
                return string.Empty;
            }
        }

        public List<PoExcelUploadMasterData> DistinctPODetails()
        {
            List<PoExcelUploadMasterData> items = new List<PoExcelUploadMasterData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT 
                                    PO_Number,
                                    MAX(PO_MasterCompanyId) AS PO_MasterCompanyId,
                                    MAX(PO_CompanyUnitId) AS PO_CompanyUnitId,
                                    MAX(PO_IsNotificationSend) AS PO_IsNotificationSend,
                                    MAX(PO_DeliveryDateTime) AS PO_DeliveryDateTime
                                 FROM 
                                    PoExcelUploadMaster
                                 WHERE 
                                    PO_MasterCompanyId = @PO_MasterCompanyId AND 
                                    PO_CompanyUnitId = @PO_CompanyUnitId AND
                                    PO_IsNotificationSend = @PO_IsNotificationSend AND 
                                    PO_BalanceQty > @PO_BalanceQty
                                 GROUP BY 
                                    PO_Number";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@PO_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    cmd.Parameters.AddWithValue("@PO_CompanyUnitId", HttpContext.Current.Session["CompanyUnitId"].ToString());
                    cmd.Parameters.AddWithValue("@PO_IsNotificationSend", 0);
                    cmd.Parameters.AddWithValue("@PO_BalanceQty", 0);

                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new PoExcelUploadMasterData
                            {
                                PO_Number = sdr["PO_Number"].ToString(),
                                PO_DeliveryDateTime = sdr["PO_DeliveryDateTime"].ToString(),
                                PO_CompanyUnitId = Convert.ToInt32(sdr["PO_CompanyUnitId"]),
                                PO_MasterCompanyId = Convert.ToInt32(sdr["PO_MasterCompanyId"]),
                                PO_IsNotificationSend = Convert.IsDBNull(sdr["PO_IsNotificationSend"]) ? 0 : Convert.ToInt32(sdr["PO_IsNotificationSend"]),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<PoExcelUploadMasterData> PoNoGetfromExcelUpload()
        {
            List<PoExcelUploadMasterData> items = new List<PoExcelUploadMasterData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT DISTINCT PO_Number FROM PoExcelUploadMaster
                                 WHERE PO_MasterCompanyId = @PO_MasterCompanyId AND PO_CompanyUnitId = @PO_CompanyUnitId AND PO_IsActive = @PO_IsActive";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@PO_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    cmd.Parameters.AddWithValue("@PO_CompanyUnitId", HttpContext.Current.Session["CompanyUnitId"].ToString());
                    cmd.Parameters.AddWithValue("@PO_StatusId", 3);
                    cmd.Parameters.AddWithValue("@PO_IsActive", 1);

                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new PoExcelUploadMasterData
                            {
                                PO_Number = sdr["PO_Number"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<ItemStoreData> DataStoreinRack()
        {
            List<ItemStoreData> items = new List<ItemStoreData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * from ItemStore";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new ItemStoreData
                            {
                                IS_ItemStoreId = Convert.IsDBNull(sdr["IS_ItemStoreId"]) ? 0 : Convert.ToInt32(sdr["IS_ItemStoreId"]),
                                IS_ITId = Convert.IsDBNull(sdr["IS_ITId"]) ? 0 : Convert.ToInt32(sdr["IS_ITId"]),
                                IS_RackSerialNo = sdr["IS_RackSerialNo"].ToString(),
                                IS_StoreQty = Convert.IsDBNull(sdr["IS_StoreQty"]) ? 0 : Convert.ToDecimal(sdr["IS_StoreQty"]),
                                IS_GRNNUMBER = sdr["IS_GRNNUMBER"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<RackNumberData> DataStoreinRackGet(string rackSerialNo)
        {
            List<RackNumberData> items = new List<RackNumberData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"select * from ItemStore i
                                 INNER JOIN RackSerialDetails rsd ON  rsd.RackSerialId = i.IS_RackSerialNo
                                 INNER JOIN RackTypeDetails rtd ON rtd.RackTypeId = rsd.RackTypeId
                                 where IS_RackSerialNo = @IS_RackSerialNo and IS_ItemStoreId = (select max(IS_ItemStoreId) from ItemStore where IS_RackSerialNo = @IS_RackSerialNo)";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IS_RackSerialNo", rackSerialNo);
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new RackNumberData
                            {
                                IS_ItemStoreId = Convert.IsDBNull(sdr["IS_ItemStoreId"]) ? 0 : Convert.ToInt32(sdr["IS_ItemStoreId"]),
                                IS_ITId = Convert.IsDBNull(sdr["IS_ITId"]) ? 0 : Convert.ToInt32(sdr["IS_ITId"]),
                                IS_RackSerialNo = sdr["IS_RackSerialNo"].ToString(),
                                IS_StoreQty = Convert.IsDBNull(sdr["IS_StoreQty"]) ? 0 : Convert.ToDecimal(sdr["IS_StoreQty"]),
                                IS_GRNNUMBER = sdr["IS_GRNNUMBER"].ToString(),

                                RackTypeId = Convert.IsDBNull(sdr["RackTypeId"]) ? 0 : Convert.ToInt32(sdr["RackTypeId"]),
                                RackType = Convert.IsDBNull(sdr["RackType"]) ? string.Empty : sdr["RackType"].ToString(),
                                RackMasterCompanyId = Convert.IsDBNull(sdr["RackMasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["RackMasterCompanyId"]),
                                RackCreatedEmployeeId = Convert.IsDBNull(sdr["RackCreatedEmployeeId"]) ? 0 : Convert.ToInt32(sdr["RackCreatedEmployeeId"]),
                                RackCreatedDateTime = Convert.IsDBNull(sdr["RackCreatedDateTime"]) ? string.Empty : sdr["RackCreatedDateTime"].ToString(),
                                RackUpdatedDateTime = Convert.IsDBNull(sdr["RackUpdatedDateTime"]) ? string.Empty : sdr["RackUpdatedDateTime"].ToString(),
                                RackAlphSeries = Convert.IsDBNull(sdr["RackAlphSeries"]) ? string.Empty : sdr["RackAlphSeries"].ToString(),
                                TotalSerialNumber = Convert.IsDBNull(sdr["TotalSerialNumber"]) ? 0 : Convert.ToInt32(sdr["TotalSerialNumber"]),
                                RackSerialNumber = Convert.IsDBNull(sdr["RackSerialNumber"]) ? string.Empty : sdr["RackSerialNumber"].ToString(),
                                RackSerialId = Convert.IsDBNull(sdr["RackSerialId"]) ? 0 : Convert.ToInt32(sdr["RackSerialId"]),
                                CreatedEmployeeId = Convert.IsDBNull(sdr["CreatedEmployeeId"]) ? 0 : Convert.ToInt32(sdr["CreatedEmployeeId"]),
                                MasterCompanyId = Convert.IsDBNull(sdr["MasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["MasterCompanyId"]),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<ItemStoreData> DataStoreinRackGet1(string[] rackSerialNos)
        {
            List<ItemStoreData> items = new List<ItemStoreData>();

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                // Modify the query to handle multiple rack serial numbers
                string query = @"SELECT * FROM ItemStore 
                                    WHERE IS_RackSerialNo IN (
                                        SELECT value FROM STRING_SPLIT(@RackSerialNos, ',')
                                    ) 
                                    AND IS_ItemStoreId = (
                                        SELECT MAX(IS_ItemStoreId) 
                                        FROM ItemStore 
                                        WHERE IS_RackSerialNo = ItemStore.IS_RackSerialNo
                                    )";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    // Convert the string array to a comma-separated string
                    string rackSerialNoString = string.Join(",", rackSerialNos);

                    cmd.Parameters.AddWithValue("@RackSerialNos", rackSerialNoString);

                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new ItemStoreData
                            {
                                IS_ItemStoreId = Convert.IsDBNull(sdr["IS_ItemStoreId"]) ? 0 : Convert.ToInt32(sdr["IS_ItemStoreId"]),
                                IS_ITId = Convert.IsDBNull(sdr["IS_ITId"]) ? 0 : Convert.ToInt32(sdr["IS_ITId"]),
                                IS_RackSerialNo = sdr["IS_RackSerialNo"].ToString(),
                                IS_StoreQty = Convert.IsDBNull(sdr["IS_StoreQty"]) ? 0 : Convert.ToDecimal(sdr["IS_StoreQty"]),
                                IS_GRNNUMBER = sdr["IS_GRNNUMBER"].ToString(),
                            });
                        }
                    }
                }
            }
            return items;
        }

        public List<NotificationModelData> NotificationDataGet()
        {
            List<NotificationModelData> items = new List<NotificationModelData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT nm.NotificationId, nm.SenderMasterCompanyId, nm.SenderEmployeeId, nm.SenderRoleId, nm.SenderCompanyUnitId,
                                  nm.SenderEmailId, nm.SenderDepartment, nm.Subject, nm.Description, nm.IsReadData, nm.ReceiverEmployeeId,
                                  nm.SenderCreatedDateTime, nm.SenderUpdatedDateTime, 
                                  meds.FirstName AS SenderFirstName, meds.LastName AS SenderLastName, meds.ProfilePicture, 
                                  medr.FirstName AS ReceiverFirstName, medr.LastName AS ReceiverLastName
                                 FROM NotificationMaster nm
                                 INNER JOIN MasterEmployeeDetails meds ON meds.EmployeeId = nm.SenderEmployeeId
                                 INNER JOIN MasterEmployeeDetails medr ON medr.EmployeeId = nm.ReceiverEmployeeId
                                 Where SenderMasterCompanyId = @SenderMasterCompanyId 
                                 ORDER BY CONVERT(datetime, nm.SenderCreatedDateTime, 120) DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IsReadData", 0);
                    cmd.Parameters.AddWithValue("@SenderMasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    /*  cmd.Parameters.AddWithValue("@SenderCompanyUnitId", HttpContext.Current.Session["CompanyUnitId"].ToString()); */
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            string profilePicture = sdr["ProfilePicture"].ToString();
                            string defaultProfilePicturePath = HttpContext.Current.Session["WebsiteURL"].ToString() + "/assets/images/DefaultProfilePic.png";

                            items.Add(new NotificationModelData
                            {
                                NotificationId = Convert.IsDBNull(sdr["NotificationId"]) ? 0 : Convert.ToInt32(sdr["NotificationId"]),
                                SenderMasterCompanyId = Convert.IsDBNull(sdr["SenderMasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["SenderMasterCompanyId"]),
                                SenderEmployeeId = Convert.IsDBNull(sdr["SenderEmployeeId"]) ? 0 : Convert.ToInt32(sdr["SenderEmployeeId"]),
                                SenderRoleId = Convert.IsDBNull(sdr["SenderRoleId"]) ? 0 : Convert.ToInt32(sdr["SenderRoleId"]),
                                SenderCompanyUnitId = Convert.IsDBNull(sdr["SenderCompanyUnitId"]) ? 0 : Convert.ToInt32(sdr["SenderCompanyUnitId"]),
                                SenderEmailId = sdr["SenderEmailId"].ToString(),
                                SenderDepartment = sdr["SenderDepartment"].ToString(),
                                Subject = sdr["Subject"].ToString(),
                                Description = sdr["Description"].ToString(),
                                IsReadData = Convert.IsDBNull(sdr["IsReadData"]) ? 0 : Convert.ToInt32(sdr["IsReadData"]),
                                ReceiverEmployeeId = Convert.IsDBNull(sdr["ReceiverEmployeeId"]) ? 0 : Convert.ToInt32(sdr["ReceiverEmployeeId"]),
                                SenderCreatedDateTime = DateTime.TryParse(sdr["SenderCreatedDateTime"].ToString(), out var createdDate) ? createdDate.ToString("dd-MM-yyyy") : string.Empty,
                                SenderUpdatedDateTime = DateTime.TryParse(sdr["SenderUpdatedDateTime"].ToString(), out var updatedDate) ? updatedDate.ToString("dd-MM-yyyy") : string.Empty,
                                SenderFirstName = sdr["SenderFirstName"].ToString(),
                                SenderLastName = sdr["SenderLastName"].ToString(),
                                ReceiverFirstName = sdr["ReceiverFirstName"].ToString(),
                                ReceiverLastName = sdr["ReceiverLastName"].ToString(),
                                SenderProfilePicture = !string.IsNullOrEmpty(profilePicture)
                                                       ? HttpContext.Current.Session["WebsiteURL"].ToString() + "/UploadImages/" + profilePicture
                                                       : defaultProfilePicturePath
                                //SenderProfilePicture = HttpContext.Current.Session["WebsiteURL"].ToString() + "/UploadImages/" + sdr["ProfilePicture"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<EmailNotificationData> NotificationDataGetforEmailorSimple()
        {
            List<EmailNotificationData> items = new List<EmailNotificationData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM EmailNotification en
                                 INNER JOIN MasterEmployeeData med  ON med.EmployeeId = en.EN_EmployeeId
                                 INNER JOIN MasterEmployeeDetails med1 ON med1.EmployeeId = en.EN_EmployeeId
                                 WHERE EN_MasterCompanyId = @EN_MasterCompanyId AND EN_CompanyUnitId = @EN_CompanyUnitId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@EN_IsEnableEmail", 1);
                    cmd.Parameters.AddWithValue("@EN_MasterCompanyId", Convert.ToInt32(HttpContext.Current.Session["MasterCompanyId"]));
                    cmd.Parameters.AddWithValue("@EN_CompanyUnitId", Convert.ToInt32(HttpContext.Current.Session["CompanyUnitId"]));
                    con.Open();

                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            EmailNotificationData item = new EmailNotificationData
                            {
                                EN_ID = sdr["EN_ID"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_ID"]),
                                EN_EmployeeId = sdr["EN_EmployeeId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_EmployeeId"]),
                                EN_CCEmployeeId = sdr["EN_CCEmployeeId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_CCEmployeeId"]),
                                EN_BCCEmployeeId = sdr["EN_BCCEmployeeId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_BCCEmployeeId"]),
                                EN_EmployeeRoleId = sdr["EN_EmployeeRoleId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_EmployeeRoleId"]),
                                EN_CCEmployeeRoleId = sdr["EN_CCEmployeeRoleId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_CCEmployeeRoleId"]),
                                EN_BCCEmployeeRoleId = sdr["EN_BCCEmployeeRoleId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_BCCEmployeeRoleId"]),
                                EN_ToEmail = sdr["EN_ToEmail"] == DBNull.Value ? string.Empty : Convert.ToString(sdr["EN_ToEmail"]),
                                EN_Subject = sdr["EN_Subject"] == DBNull.Value ? string.Empty : Convert.ToString(sdr["EN_Subject"]),
                                EN_CcEmail = sdr["EN_CcEmail"] == DBNull.Value ? string.Empty : Convert.ToString(sdr["EN_CcEmail"]),
                                EN_BccEmail = sdr["EN_BccEmail"] == DBNull.Value ? string.Empty : Convert.ToString(sdr["EN_BccEmail"]),
                                EN_ENPID = sdr["EN_ENPID"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_ENPID"]),
                                EN_IsEnableEmail = sdr["EN_IsEnableEmail"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_IsEnableEmail"]),
                                EN_IsEnableNotification = sdr["EN_IsEnableNotification"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_IsEnableNotification"]),
                                EN_MasterCompanyId = sdr["EN_MasterCompanyId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_MasterCompanyId"]),
                                EN_CompanyUnitId = sdr["EN_CompanyUnitId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_CompanyUnitId"]),
                                EN_CreatedEmployeeId = sdr["EN_CreatedEmployeeId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_CreatedEmployeeId"]),
                                EN_CreatedDateTime = sdr["EN_CreatedDateTime"] == DBNull.Value ? string.Empty : Convert.ToString(sdr["EN_CreatedDateTime"]),
                                EN_UpdatedDateTime = sdr["EN_UpdatedDateTime"] == DBNull.Value ? string.Empty : Convert.ToString(sdr["EN_UpdatedDateTime"]),

                                EmailId = sdr["EmailId"].ToString(),
                                FirstName = sdr["FirstName"].ToString(),
                                LastName = sdr["LastName"].ToString(),
                                MobileNumber = sdr["MobileNumber"].ToString(),
                                Gender = sdr["Gender"].ToString(),
                                Address = sdr["Address"].ToString(),
                                City = sdr["City"].ToString(),
                                State = sdr["State"].ToString(),
                                Country = sdr["Country"].ToString(),
                                PINCode = sdr["PINCode"].ToString(),
                                ProfilePicture = sdr["ProfilePicture"].ToString(),
                            };

                            items.Add(item);
                        }
                    }
                }
            }

            return items;
        }

        public List<EmailNotificationData> EmailNotificationDataGet()
        {
            List<EmailNotificationData> items = new List<EmailNotificationData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM EmailNotification en
                                 INNER JOIN MasterEmployeeData med  ON med.EmployeeId = en.EN_EmployeeId
                                 INNER JOIN MasterEmployeeDetails med1 ON med1.EmployeeId = en.EN_EmployeeId
                                 WHERE EN_MasterCompanyId = @EN_MasterCompanyId AND EN_CompanyUnitId = @EN_CompanyUnitId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@EN_MasterCompanyId", Convert.ToInt32(HttpContext.Current.Session["MasterCompanyId"]));
                    cmd.Parameters.AddWithValue("@EN_CompanyUnitId", Convert.ToInt32(HttpContext.Current.Session["CompanyUnitId"]));
                    con.Open();

                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            EmailNotificationData item = new EmailNotificationData
                            {
                                EN_ID = sdr["EN_ID"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_ID"]),
                                EN_EmployeeId = sdr["EN_EmployeeId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_EmployeeId"]),
                                EN_CCEmployeeId = sdr["EN_CCEmployeeId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_CCEmployeeId"]),
                                EN_BCCEmployeeId = sdr["EN_BCCEmployeeId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_BCCEmployeeId"]),
                                EN_EmployeeRoleId = sdr["EN_EmployeeRoleId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_EmployeeRoleId"]),
                                EN_CCEmployeeRoleId = sdr["EN_CCEmployeeRoleId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_CCEmployeeRoleId"]),
                                EN_BCCEmployeeRoleId = sdr["EN_BCCEmployeeRoleId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_BCCEmployeeRoleId"]),
                                EN_ToEmail = sdr["EN_ToEmail"] == DBNull.Value ? string.Empty : Convert.ToString(sdr["EN_ToEmail"]),
                                EN_Subject = sdr["EN_Subject"] == DBNull.Value ? string.Empty : Convert.ToString(sdr["EN_Subject"]),
                                EN_CcEmail = sdr["EN_CcEmail"] == DBNull.Value ? string.Empty : Convert.ToString(sdr["EN_CcEmail"]),
                                EN_BccEmail = sdr["EN_BccEmail"] == DBNull.Value ? string.Empty : Convert.ToString(sdr["EN_BccEmail"]),
                                EN_ENPID = sdr["EN_ENPID"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_ENPID"]),
                                EN_IsEnableEmail = sdr["EN_IsEnableEmail"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_IsEnableEmail"]),
                                EN_IsEnableNotification = sdr["EN_IsEnableNotification"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_IsEnableNotification"]),
                                EN_MasterCompanyId = sdr["EN_MasterCompanyId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_MasterCompanyId"]),
                                EN_CompanyUnitId = sdr["EN_CompanyUnitId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_CompanyUnitId"]),
                                EN_CreatedEmployeeId = sdr["EN_CreatedEmployeeId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_CreatedEmployeeId"]),
                                EN_CreatedDateTime = sdr["EN_CreatedDateTime"] == DBNull.Value ? string.Empty : Convert.ToString(sdr["EN_CreatedDateTime"]),
                                EN_UpdatedDateTime = sdr["EN_UpdatedDateTime"] == DBNull.Value ? string.Empty : Convert.ToString(sdr["EN_UpdatedDateTime"]),


                                EmailId = sdr["EmailId"].ToString(),
                                FirstName = sdr["FirstName"].ToString(),
                                LastName = sdr["LastName"].ToString(),
                                MobileNumber = sdr["MobileNumber"].ToString(),
                                Gender = sdr["Gender"].ToString(),
                                Address = sdr["Address"].ToString(),
                                City = sdr["City"].ToString(),
                                State = sdr["State"].ToString(),
                                Country = sdr["Country"].ToString(),
                                PINCode = sdr["PINCode"].ToString(),
                                ProfilePicture = sdr["ProfilePicture"].ToString(),
                            };

                            items.Add(item);
                        }
                    }
                }
            }

            return items;
        }

        public List<EmailNotificationData> EmailNotificationWithUnitName()
        {
            List<EmailNotificationData> items = new List<EmailNotificationData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM EmailNotification en
                                 INNER JOIN MasterEmployeeData med  ON med.EmployeeId = en.EN_EmployeeId
                                 INNER JOIN MasterEmployeeDetails med1 ON med1.EmployeeId = en.EN_EmployeeId
                                 INNER JOIN CompanyUnitMaster cum ON cum.CompanyUnitId = en.EN_CompanyUnitId
                                 WHERE EN_MasterCompanyId = @EN_MasterCompanyId
                                 ORDER BY CONVERT(datetime, EN_CreatedDateTime, 120) DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@EN_MasterCompanyId", Convert.ToInt32(HttpContext.Current.Session["MasterCompanyId"]));
                    cmd.Parameters.AddWithValue("@EN_CompanyUnitId", Convert.ToInt32(HttpContext.Current.Session["CompanyUnitId"]));
                    con.Open();

                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            EmailNotificationData item = new EmailNotificationData
                            {
                                EN_ID = sdr["EN_ID"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_ID"]),
                                EN_EmployeeId = sdr["EN_EmployeeId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_EmployeeId"]),
                                EN_CCEmployeeId = sdr["EN_CCEmployeeId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_CCEmployeeId"]),
                                EN_BCCEmployeeId = sdr["EN_BCCEmployeeId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_BCCEmployeeId"]),

                                EN_EmployeeRoleId = sdr["EN_EmployeeRoleId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_EmployeeRoleId"]),
                                EN_CCEmployeeRoleId = sdr["EN_CCEmployeeRoleId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_CCEmployeeRoleId"]),
                                EN_BCCEmployeeRoleId = sdr["EN_BCCEmployeeRoleId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_BCCEmployeeRoleId"]),


                                EN_ToEmail = sdr["EN_ToEmail"] == DBNull.Value ? string.Empty : Convert.ToString(sdr["EN_ToEmail"]),
                                EN_Subject = sdr["EN_Subject"] == DBNull.Value ? string.Empty : Convert.ToString(sdr["EN_Subject"]),
                                EN_CcEmail = sdr["EN_CcEmail"] == DBNull.Value ? string.Empty : Convert.ToString(sdr["EN_CcEmail"]),
                                EN_BccEmail = sdr["EN_BccEmail"] == DBNull.Value ? string.Empty : Convert.ToString(sdr["EN_BccEmail"]),
                                EN_ENPID = sdr["EN_ENPID"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_ENPID"]),
                                EN_IsEnableEmail = sdr["EN_IsEnableEmail"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_IsEnableEmail"]),
                                EN_IsEnableNotification = sdr["EN_IsEnableNotification"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_IsEnableNotification"]),
                                EN_MasterCompanyId = sdr["EN_MasterCompanyId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_MasterCompanyId"]),
                                EN_CompanyUnitId = sdr["EN_CompanyUnitId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_CompanyUnitId"]),
                                EN_CreatedEmployeeId = sdr["EN_CreatedEmployeeId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EN_CreatedEmployeeId"]),
                                EN_CreatedDateTime = sdr["EN_CreatedDateTime"] == DBNull.Value ? string.Empty : Convert.ToString(sdr["EN_CreatedDateTime"]),
                                EN_UpdatedDateTime = sdr["EN_UpdatedDateTime"] == DBNull.Value ? string.Empty : Convert.ToString(sdr["EN_UpdatedDateTime"]),


                                EmailId = sdr["EmailId"].ToString(),
                                FirstName = sdr["FirstName"].ToString(),
                                LastName = sdr["LastName"].ToString(),
                                MobileNumber = sdr["MobileNumber"].ToString(),
                                Gender = sdr["Gender"].ToString(),
                                Address = sdr["Address"].ToString(),
                                City = sdr["City"].ToString(),
                                State = sdr["State"].ToString(),
                                Country = sdr["Country"].ToString(),
                                PINCode = sdr["PINCode"].ToString(),
                                ProfilePicture = sdr["ProfilePicture"].ToString(),

                                UnitName = sdr["UnitName"].ToString(),
                            };

                            items.Add(item);
                        }
                    }
                }
            }

            return items;
        }

        public List<NotificationAssignMasterModelData> NotificationAssignWithUnitName()
        {
            List<NotificationAssignMasterModelData> items = new List<NotificationAssignMasterModelData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT nam.*,    
                                        med_to.EmailId AS ToEmployeeEmail,
                                        med1_to.FirstName AS ToEmployeeFirstName,
                                        med1_to.LastName AS ToEmployeeLastName,
                                        med1_to.MobileNumber AS ToEmployeeMobile,    
                                        med_cc.EmailId AS CCEmployeeEmail,
                                        med1_cc.FirstName AS CCEmployeeFirstName,
                                        med1_cc.LastName AS CCEmployeeLastName,
                                        med1_cc.MobileNumber AS CCEmployeeMobile,    
                                        med_bcc.EmailId AS BCCEmployeeEmail,
                                        med1_bcc.FirstName AS BCCEmployeeFirstName,
                                        med1_bcc.LastName AS BCCEmployeeLastName,
                                        med1_bcc.MobileNumber AS BCCEmployeeMobile,
	                                    cum.UnitName AS UnitName
                                    FROM NotificationAssignMaster nam
                                    LEFT JOIN MasterEmployeeData med_to ON med_to.EmployeeId = nam.NA_ToEmployeeId
                                    LEFT JOIN MasterEmployeeDetails med1_to ON med1_to.EmployeeId = nam.NA_ToEmployeeId
                                    LEFT JOIN MasterEmployeeData med_cc ON med_cc.EmployeeId = nam.NA_CCEmployeeId
                                    LEFT JOIN MasterEmployeeDetails med1_cc ON med1_cc.EmployeeId = nam.NA_CCEmployeeId
                                    LEFT JOIN MasterEmployeeData med_bcc ON med_bcc.EmployeeId = nam.NA_BCCEmployeeId
                                    LEFT JOIN MasterEmployeeDetails med1_bcc ON med1_bcc.EmployeeId = nam.NA_BCCEmployeeId
                                    INNER JOIN CompanyUnitMaster cum ON cum.CompanyUnitId = nam.NA_CompanyUnitId
                                    WHERE NA_MasterCompanyId = @NA_MasterCompanyId
                                    ORDER BY CONVERT(datetime, NA_CreatedDateTime, 120) DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@NA_MasterCompanyId", Convert.ToInt32(HttpContext.Current.Session["MasterCompanyId"]));
                    //cmd.Parameters.AddWithValue("@EN_CompanyUnitId", Convert.ToInt32(HttpContext.Current.Session["CompanyUnitId"]));
                    con.Open();

                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new NotificationAssignMasterModelData
                            {
                                NA_ID = sdr["NA_ID"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NA_ID"]),
                                NA_NID = sdr["NA_NID"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NA_NID"]),
                                NA_SubjectName = sdr["NA_SubjectName"].ToString(),
                                NA_ToEmployeeRoleId = sdr["NA_ToEmployeeRoleId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NA_ToEmployeeRoleId"]),
                                NA_ToEmployeeId = sdr["NA_ToEmployeeId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NA_ToEmployeeId"]),
                                NA_CCEmployeeRoleId = sdr["NA_CCEmployeeRoleId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NA_CCEmployeeRoleId"]),
                                NA_CCEmployeeId = sdr["NA_CCEmployeeId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NA_CCEmployeeId"]),
                                NA_BCCEmployeeRoleId = sdr["NA_BCCEmployeeRoleId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NA_BCCEmployeeRoleId"]),
                                NA_BCCEmployeeId = sdr["NA_BCCEmployeeId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NA_BCCEmployeeId"]),
                                NA_MasterCompanyId = sdr["NA_MasterCompanyId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NA_MasterCompanyId"]),
                                NA_CompanyUnitId = sdr["NA_CompanyUnitId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NA_CompanyUnitId"]),
                                NA_CreatedEmployeeId = sdr["NA_CreatedEmployeeId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NA_CreatedEmployeeId"]),
                                NA_CreatedDateTime = sdr["NA_CreatedDateTime"].ToString(),
                                NA_UpdatedDateTime = sdr["NA_UpdatedDateTime"].ToString(),
                                ToEmployeeEmail = sdr["ToEmployeeEmail"].ToString(),
                                ToEmployeeFirstName = sdr["ToEmployeeFirstName"].ToString(),
                                ToEmployeeLastName = sdr["ToEmployeeLastName"].ToString(),
                                ToEmployeeMobile = sdr["ToEmployeeMobile"].ToString(),
                                CCEmployeeEmail = sdr["CCEmployeeEmail"].ToString(),
                                CCEmployeeFirstName = sdr["CCEmployeeFirstName"].ToString(),
                                CCEmployeeLastName = sdr["CCEmployeeLastName"].ToString(),
                                CCEmployeeMobile = sdr["CCEmployeeMobile"].ToString(),
                                BCCEmployeeEmail = sdr["BCCEmployeeEmail"].ToString(),
                                BCCEmployeeFirstName = sdr["BCCEmployeeFirstName"].ToString(),
                                BCCEmployeeLastName = sdr["BCCEmployeeLastName"].ToString(),
                                BCCEmployeeMobile = sdr["BCCEmployeeMobile"].ToString(),
                                UnitName = sdr["UnitName"].ToString()
                            });
                        }
                    }
                }
            }

            return items;
        }

        /* Notification type Settings */
        public List<NotificationSettingModelData> NotificationTypeSettingDataGet()
        {
            List<NotificationSettingModelData> items = new List<NotificationSettingModelData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM NotificationTypeSetting ORDER BY NT_ID ASC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@N_MasterCompanyId", Convert.ToInt32(HttpContext.Current.Session["MasterCompanyId"]));
                    con.Open();

                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new NotificationSettingModelData
                            {
                                NT_ID = sdr["NT_ID"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NT_ID"]),
                                NT_Name = sdr["NT_Name"] == DBNull.Value ? string.Empty : sdr["NT_Name"].ToString(),
                                NT_MasterCompanyId = sdr["NT_MasterCompanyId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NT_MasterCompanyId"]),
                                NT_CompanyUnitId = sdr["NT_CompanyUnitId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NT_CompanyUnitId"]),
                                NT_CreatedEmployeeId = sdr["NT_CreatedEmployeeId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NT_CreatedEmployeeId"]),
                                NT_IsEnabled = sdr["NT_IsEnabled"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NT_IsEnabled"]),
                                NT_IsVisibled = sdr["NT_IsVisibled"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NT_IsVisibled"]),
                                NT_CreatedDateTime = sdr["NT_CreatedDateTime"] == DBNull.Value ? string.Empty : sdr["NT_CreatedDateTime"].ToString(),
                                NT_UpdatedDateTime = sdr["NT_UpdatedDateTime"] == DBNull.Value ? string.Empty : sdr["NT_UpdatedDateTime"].ToString(),
                            });
                        }
                    }
                }
            }
            return items;
        }

        public List<NotificationSettingModelData> NotificationSettingDataGet()
        {
            List<NotificationSettingModelData> items = new List<NotificationSettingModelData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM NotificationSetting ORDER BY N_ID ASC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    //cmd.Parameters.AddWithValue("@N_MasterCompanyId", Convert.ToInt32(HttpContext.Current.Session["MasterCompanyId"]));
                    con.Open();

                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new NotificationSettingModelData
                            {
                                N_ID = sdr["N_ID"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["N_ID"]),
                                N_NTID = sdr["N_NTID"] == DBNull.Value ? string.Empty : sdr["N_NTID"].ToString(),
                                N_Subject = sdr["N_Subject"] == DBNull.Value ? string.Empty : sdr["N_Subject"].ToString(),
                                //N_MasterCompanyId = sdr["N_MasterCompanyId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["N_MasterCompanyId"]),
                                //NT_CompanyUnitId = sdr["NT_CompanyUnitId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NT_CompanyUnitId"]),
                                //NT_CreatedEmployeeId = sdr["NT_CreatedEmployeeId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NT_CreatedEmployeeId"]),
                                //NT_IsEnabled = sdr["NT_IsEnabled"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NT_IsEnabled"]),
                                //NT_IsVisibled = sdr["NT_IsVisibled"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NT_IsVisibled"]),
                                N_CreatedDateTime = sdr["N_CreatedDateTime"] == DBNull.Value ? string.Empty : sdr["N_CreatedDateTime"].ToString(),
                                //NT_UpdatedDateTime = sdr["NT_UpdatedDateTime"] == DBNull.Value ? string.Empty : sdr["NT_UpdatedDateTime"].ToString(),
                            });
                        }
                    }
                }
            }
            return items;
        }

        /* Notification type Settings */
        public List<NotificationSettingModelData> NotificationSubjectNameDisplay()
        {
            List<NotificationSettingModelData> items = new List<NotificationSettingModelData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM NotificationSetting NS
                                 INNER JOIN NotificationTypeSetting NTS ON CHARINDEX(',' + CAST(NTS.NT_ID AS VARCHAR) + ',', ',' + NS.N_NTID + ',') > 0
                                 WHERE N_MasterCompanyId = @N_MasterCompanyId ORDER BY N_Subject ASC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@N_MasterCompanyId", Convert.ToInt32(HttpContext.Current.Session["MasterCompanyId"]));
                    con.Open();

                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new NotificationSettingModelData
                            {
                                N_ID = sdr["N_ID"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["N_ID"]),
                                N_NTID = sdr["N_NTID"] == DBNull.Value ? string.Empty : sdr["N_NTID"].ToString(),
                                N_Subject = sdr["N_Subject"] == DBNull.Value ? string.Empty : sdr["N_Subject"].ToString(),
                                N_MasterCompanyId = sdr["N_MasterCompanyId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["N_MasterCompanyId"]),
                                N_CompanyUnitId = sdr["N_CompanyUnitId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["N_CompanyUnitId"]),
                                N_CreatedEmployeeId = sdr["N_CreatedEmployeeId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["N_CreatedEmployeeId"]),
                                N_CreatedDateTime = sdr["N_CreatedDateTime"] == DBNull.Value ? string.Empty : sdr["N_CreatedDateTime"].ToString(),
                                N_UpdatedDateTime = sdr["N_UpdatedDateTime"] == DBNull.Value ? string.Empty : sdr["N_UpdatedDateTime"].ToString(),

                                NT_ID = sdr["NT_ID"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NT_ID"]),
                                NT_Name = sdr["NT_Name"] == DBNull.Value ? string.Empty : sdr["NT_Name"].ToString(),
                                NT_MasterCompanyId = sdr["NT_MasterCompanyId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NT_MasterCompanyId"]),
                                NT_CompanyUnitId = sdr["NT_CompanyUnitId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NT_CompanyUnitId"]),
                                NT_CreatedEmployeeId = sdr["NT_CreatedEmployeeId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NT_CreatedEmployeeId"]),
                                NT_IsEnabled = sdr["NT_IsEnabled"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NT_IsEnabled"]),
                                NT_IsVisibled = sdr["NT_IsVisibled"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NT_IsVisibled"]),
                                NT_CreatedDateTime = sdr["NT_CreatedDateTime"] == DBNull.Value ? string.Empty : sdr["NT_CreatedDateTime"].ToString(),
                                NT_UpdatedDateTime = sdr["NT_UpdatedDateTime"] == DBNull.Value ? string.Empty : sdr["NT_UpdatedDateTime"].ToString(),
                            });
                        }
                    }
                }
            }
            return items;
        }

        public List<NotificationSettingModelData> GetNotificationItemsBySubject(string notificationSubject)
        {
            List<NotificationSettingModelData> items = new List<NotificationSettingModelData>();

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM NotificationSetting NS
                                 INNER JOIN NotificationTypeSetting NTS ON CHARINDEX(',' + CAST(NTS.NT_ID AS VARCHAR) + ',', ',' + NS.N_NTID + ',') > 0                                
                                 WHERE N_MasterCompanyId = @N_MasterCompanyId AND N_Subject = @N_Subject
                                 ORDER BY N_Subject ASC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@N_MasterCompanyId", Convert.ToInt32(HttpContext.Current.Session["MasterCompanyId"]));
                    cmd.Parameters.AddWithValue("@N_Subject", notificationSubject);

                    con.Open();

                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new NotificationSettingModelData
                            {
                                N_ID = sdr["N_ID"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["N_ID"]),
                                N_NTID = sdr["N_NTID"] == DBNull.Value ? string.Empty : sdr["N_NTID"].ToString(),
                                N_Subject = sdr["N_Subject"] == DBNull.Value ? string.Empty : sdr["N_Subject"].ToString(),
                                N_MasterCompanyId = sdr["N_MasterCompanyId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["N_MasterCompanyId"]),
                                N_CompanyUnitId = sdr["N_CompanyUnitId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["N_CompanyUnitId"]),
                                N_CreatedEmployeeId = sdr["N_CreatedEmployeeId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["N_CreatedEmployeeId"]),
                                N_CreatedDateTime = sdr["N_CreatedDateTime"] == DBNull.Value ? string.Empty : sdr["N_CreatedDateTime"].ToString(),
                                N_UpdatedDateTime = sdr["N_UpdatedDateTime"] == DBNull.Value ? string.Empty : sdr["N_UpdatedDateTime"].ToString(),

                                NT_ID = sdr["NT_ID"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NT_ID"]),
                                NT_Name = sdr["NT_Name"] == DBNull.Value ? string.Empty : sdr["NT_Name"].ToString(),
                                NT_MasterCompanyId = sdr["NT_MasterCompanyId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NT_MasterCompanyId"]),
                                NT_CompanyUnitId = sdr["NT_CompanyUnitId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NT_CompanyUnitId"]),
                                NT_CreatedEmployeeId = sdr["NT_CreatedEmployeeId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NT_CreatedEmployeeId"]),
                                NT_IsEnabled = sdr["NT_IsEnabled"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NT_IsEnabled"]),
                                NT_IsVisibled = sdr["NT_IsVisibled"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NT_IsVisibled"]),
                                NT_CreatedDateTime = sdr["NT_CreatedDateTime"] == DBNull.Value ? string.Empty : sdr["NT_CreatedDateTime"].ToString(),
                                NT_UpdatedDateTime = sdr["NT_UpdatedDateTime"] == DBNull.Value ? string.Empty : sdr["NT_UpdatedDateTime"].ToString(),

                                //ND_NTID = sdr["ND_NTID"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["ND_NTID"]),
                                //ND_Value = sdr["ND_Value"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["ND_Value"]),
                            });
                        }
                    }
                }
            }

            return items;
        }

        public List<NotificationSettingModelData> NotificationSubjectNameDisplayinDropDown()
        {
            List<NotificationSettingModelData> items = new List<NotificationSettingModelData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM NotificationSetting
                                 WHERE N_MasterCompanyId = @N_MasterCompanyId ORDER BY N_Subject ASC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@N_MasterCompanyId", Convert.ToInt32(HttpContext.Current.Session["MasterCompanyId"]));
                    con.Open();

                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new NotificationSettingModelData
                            {
                                N_ID = sdr["N_ID"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["N_ID"]),
                                N_NTID = sdr["N_NTID"] == DBNull.Value ? string.Empty : sdr["N_NTID"].ToString(),
                                N_Subject = sdr["N_Subject"] == DBNull.Value ? string.Empty : sdr["N_Subject"].ToString(),
                                N_MasterCompanyId = sdr["N_MasterCompanyId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["N_MasterCompanyId"]),
                                N_CompanyUnitId = sdr["N_CompanyUnitId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["N_CompanyUnitId"]),
                                N_CreatedEmployeeId = sdr["N_CreatedEmployeeId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["N_CreatedEmployeeId"]),
                                N_CreatedDateTime = sdr["N_CreatedDateTime"] == DBNull.Value ? string.Empty : sdr["N_CreatedDateTime"].ToString(),
                                N_UpdatedDateTime = sdr["N_UpdatedDateTime"] == DBNull.Value ? string.Empty : sdr["N_UpdatedDateTime"].ToString()
                            });
                        }
                    }
                }
            }
            return items;
        }

        /* store hod find for send notification */
        public List<NotificationSettingSaveModelData> StoreHODGet(int CompanyUnitId)
        {
            List<NotificationSettingSaveModelData> items = new List<NotificationSettingSaveModelData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"select EmployeeId from MasterEmployeeData where RoleId = 15 and UnitId = @UnitId and MasterCompanyId = @MasterCompanyId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@MasterCompanyId", Convert.ToInt32(HttpContext.Current.Session["MasterCompanyId"]));
                    cmd.Parameters.AddWithValue("@UnitId", CompanyUnitId);
                    con.Open();

                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new NotificationSettingSaveModelData
                            {
                                NS_EmployeeId = sdr["EmployeeId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EmployeeId"]),
                            });
                        }
                    }
                }
            }

            return items;
        }

        /* Notification Settings Save get Employee wise*/
        public List<NotificationSettingSaveModelData> NotificationSaveDataDisplay()
        {
            List<NotificationSettingSaveModelData> items = new List<NotificationSettingSaveModelData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM NotificationSettingSave nds
                                 INNER JOIN MasterEmployeeData med  ON med.EmployeeId = nds.NS_EmployeeId
                                 INNER JOIN MasterEmployeeDetails med1 ON med1.EmployeeId = nds.NS_EmployeeId
                                 WHERE NS_EmployeeId = @NS_EmployeeId
                                 ORDER BY NS_SubjectName ASC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@NS_EmployeeId", Convert.ToInt32(HttpContext.Current.Session["UserID"]));
                    cmd.Parameters.AddWithValue("@NS_MasterCompanyId", Convert.ToInt32(HttpContext.Current.Session["MasterCompanyId"]));
                    cmd.Parameters.AddWithValue("@NS_CompanyUnitId", Convert.ToInt32(HttpContext.Current.Session["CompanyUnitId"]));
                    con.Open();

                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new NotificationSettingSaveModelData
                            {
                                NS_ID = sdr["NS_ID"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NS_ID"]),
                                NS_EmployeeId = sdr["NS_EmployeeId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NS_EmployeeId"]),
                                NS_MasterCompanyId = sdr["NS_MasterCompanyId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NS_MasterCompanyId"]),
                                NS_CompanyUnitId = sdr["NS_CompanyUnitId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NS_CompanyUnitId"]),
                                NS_SubjectName = sdr["NS_SubjectName"] == DBNull.Value ? string.Empty : sdr["NS_SubjectName"].ToString(),
                                NS_TypeName = sdr["NS_TypeName"] == DBNull.Value ? string.Empty : sdr["NS_TypeName"].ToString(),
                                NS_IsChecke = sdr["NS_IsChecke"] == DBNull.Value ? string.Empty : sdr["NS_IsChecke"].ToString(),
                                NS_IsEnable = sdr["NS_IsEnable"] == DBNull.Value ? string.Empty : sdr["NS_IsEnable"].ToString(),
                                NS_IsVisible = sdr["NS_IsVisible"] == DBNull.Value ? string.Empty : sdr["NS_IsVisible"].ToString(),
                                NS_CreatedDateTime = sdr["NS_CreatedDateTime"] == DBNull.Value ? string.Empty : sdr["NS_CreatedDateTime"].ToString(),
                                NS_UpdatedDateTime = sdr["NS_UpdatedDateTime"] == DBNull.Value ? string.Empty : sdr["NS_UpdatedDateTime"].ToString(),

                                EmailId = sdr["EmailId"].ToString(),
                                FirstName = sdr["FirstName"].ToString(),
                                LastName = sdr["LastName"].ToString(),
                                MobileNumber = sdr["MobileNumber"].ToString(),
                                Gender = sdr["Gender"].ToString(),
                                Address = sdr["Address"].ToString(),
                                City = sdr["City"].ToString(),
                                State = sdr["State"].ToString(),
                                Country = sdr["Country"].ToString(),
                                PINCode = sdr["PINCode"].ToString(),
                                ProfilePicture = sdr["ProfilePicture"].ToString(),
                            });
                        }
                    }
                }
            }

            return items;
        }

        public List<NotificationSettingSaveModelData> NotificationSaveDataEmployeeWiseDisplay()
        {
            List<NotificationSettingSaveModelData> items = new List<NotificationSettingSaveModelData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                //string query = @"SELECT DISTINCT NS_EmployeeId FROM NotificationSettingSave";
                string query = @"select * from MasterEmployeeData";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new NotificationSettingSaveModelData
                            {
                                NS_EmployeeId = sdr["EmployeeId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EmployeeId"]),
                                //NS_EmployeeId = sdr["NS_EmployeeId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NS_EmployeeId"]),
                            });
                        }
                    }
                }
            }

            return items;
        }

        public List<NotificationSettingSaveModelData> NotificationSaveDataEmployeeAndSubjectNameWiseDisplay()
        {
            List<NotificationSettingSaveModelData> items = new List<NotificationSettingSaveModelData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT DISTINCT NS_EmployeeId, NS_SubjectName FROM NotificationSettingSave";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new NotificationSettingSaveModelData
                            {
                                //NS_ID = sdr["NS_ID"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NS_ID"]),
                                NS_EmployeeId = sdr["NS_EmployeeId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NS_EmployeeId"]),
                                //NS_MasterCompanyId = sdr["NS_MasterCompanyId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NS_MasterCompanyId"]),
                                //NS_CompanyUnitId = sdr["NS_CompanyUnitId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NS_CompanyUnitId"]),
                                NS_SubjectName = sdr["NS_SubjectName"] == DBNull.Value ? string.Empty : sdr["NS_SubjectName"].ToString(),
                                //NS_TypeName = sdr["NS_TypeName"] == DBNull.Value ? string.Empty : sdr["NS_TypeName"].ToString(),
                                //NS_IsChecke = sdr["NS_IsChecke"] == DBNull.Value ? string.Empty : sdr["NS_IsChecke"].ToString(),
                                //NS_IsEnable = sdr["NS_IsEnable"] == DBNull.Value ? string.Empty : sdr["NS_IsEnable"].ToString(),
                                //NS_IsVisible = sdr["NS_IsVisible"] == DBNull.Value ? string.Empty : sdr["NS_IsVisible"].ToString(),
                                //NS_CreatedDateTime = sdr["NS_CreatedDateTime"] == DBNull.Value ? string.Empty : sdr["NS_CreatedDateTime"].ToString(),
                                //NS_UpdatedDateTime = sdr["NS_UpdatedDateTime"] == DBNull.Value ? string.Empty : sdr["NS_UpdatedDateTime"].ToString(),                                
                            });
                        }
                    }
                }
            }

            return items;
        }

        public List<NotificationSettingSaveModelData> SaveNotificationTypeBySubject(string notificationSubject)
        {
            List<NotificationSettingSaveModelData> items = new List<NotificationSettingSaveModelData>();

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM NotificationSettingSave 
                                 WHERE NS_EmployeeId = @NS_EmployeeId AND NS_SubjectName = @NS_SubjectName
                                 ORDER BY NS_SubjectName ASC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@NS_EmployeeId", Convert.ToInt32(HttpContext.Current.Session["UserID"]));
                    cmd.Parameters.AddWithValue("@NS_SubjectName", notificationSubject);

                    con.Open();

                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new NotificationSettingSaveModelData
                            {
                                NS_ID = sdr["NS_ID"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NS_ID"]),
                                NS_EmployeeId = sdr["NS_EmployeeId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NS_EmployeeId"]),
                                NS_MasterCompanyId = sdr["NS_MasterCompanyId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NS_MasterCompanyId"]),
                                NS_CompanyUnitId = sdr["NS_CompanyUnitId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NS_CompanyUnitId"]),
                                NS_SubjectName = sdr["NS_SubjectName"] == DBNull.Value ? string.Empty : sdr["NS_SubjectName"].ToString(),
                                NS_TypeName = sdr["NS_TypeName"] == DBNull.Value ? string.Empty : sdr["NS_TypeName"].ToString(),
                                NS_IsChecke = sdr["NS_IsChecke"] == DBNull.Value ? string.Empty : sdr["NS_IsChecke"].ToString(),
                                NS_IsEnable = sdr["NS_IsEnable"] == DBNull.Value ? string.Empty : sdr["NS_IsEnable"].ToString(),
                                NS_IsVisible = sdr["NS_IsVisible"] == DBNull.Value ? string.Empty : sdr["NS_IsVisible"].ToString(),
                                NS_CreatedDateTime = sdr["NS_CreatedDateTime"] == DBNull.Value ? string.Empty : sdr["NS_CreatedDateTime"].ToString(),
                                NS_UpdatedDateTime = sdr["NS_UpdatedDateTime"] == DBNull.Value ? string.Empty : sdr["NS_UpdatedDateTime"].ToString(),
                            });
                        }
                    }
                }
            }

            return items;
        }

        public List<NotificationSettingSaveModelData> SaveNotificationSettingDataGet(int employeeId, string notificationSubject)
        {
            List<NotificationSettingSaveModelData> items = new List<NotificationSettingSaveModelData>();

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM NotificationSettingSave 
                                 WHERE NS_EmployeeId = @NS_EmployeeId AND NS_SubjectName = @NS_SubjectName";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@NS_EmployeeId", employeeId);
                    cmd.Parameters.AddWithValue("@NS_SubjectName", notificationSubject);

                    con.Open();

                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new NotificationSettingSaveModelData
                            {
                                NS_ID = sdr["NS_ID"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NS_ID"]),
                                NS_EmployeeId = sdr["NS_EmployeeId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NS_EmployeeId"]),
                                NS_MasterCompanyId = sdr["NS_MasterCompanyId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NS_MasterCompanyId"]),
                                NS_CompanyUnitId = sdr["NS_CompanyUnitId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["NS_CompanyUnitId"]),
                                NS_SubjectName = sdr["NS_SubjectName"] == DBNull.Value ? string.Empty : sdr["NS_SubjectName"].ToString(),
                                NS_TypeName = sdr["NS_TypeName"] == DBNull.Value ? string.Empty : sdr["NS_TypeName"].ToString(),
                                NS_IsChecke = sdr["NS_IsChecke"] == DBNull.Value ? string.Empty : sdr["NS_IsChecke"].ToString(),
                                NS_IsEnable = sdr["NS_IsEnable"] == DBNull.Value ? string.Empty : sdr["NS_IsEnable"].ToString(),
                                NS_IsVisible = sdr["NS_IsVisible"] == DBNull.Value ? string.Empty : sdr["NS_IsVisible"].ToString(),
                                NS_CreatedDateTime = sdr["NS_CreatedDateTime"] == DBNull.Value ? string.Empty : sdr["NS_CreatedDateTime"].ToString(),
                                NS_UpdatedDateTime = sdr["NS_UpdatedDateTime"] == DBNull.Value ? string.Empty : sdr["NS_UpdatedDateTime"].ToString(),
                            });
                        }
                    }
                }
            }

            return items;
        }

        public List<NotificationDataForEmpAndHOD> EmailDataforEmployeeAndItsHOD(int CompanyUnitId)
        {
            List<NotificationDataForEmpAndHOD> items = new List<NotificationDataForEmpAndHOD>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"select 
                                    med.EmployeeId as [EmployeeId], 
                                    med.RoleId as [RoleIdOfEmployee], 
                                    ed.FirstName as [FirstNameOfEmployee], 
                                    ed.LastName as [LastNameOfEmployee],
                                    rm.RoleName as [RoleNameOfEmployee], 
                                    med.EmailId as [EmailIdOfEmployee],
                                    med.HODEmployeeId as [EmployeeIdOfHOD], 
                                    med.HODEmployeeRoleId as [RoleIdOfHOD], 
                                    ed1.FirstName as [FirstNameOfHOD],
                                    ed1.LastName as [LastNameOfHOD],
                                    rm1.RoleName as [RoleNameOfHOD],
                                    med1.EmailId as [EmailIdOfHOD]
                                from MasterEmployeeData med
                                inner join MasterEmployeeDetails ed on ed.EmployeeId = med.EmployeeId
                                inner join RoleMasterTB rm on rm.RoleId = med.RoleId
                                left join MasterEmployeeData med1 on med1.EmployeeId = med.HODEmployeeId
                                left join MasterEmployeeDetails ed1 on ed1.EmployeeId = med.HODEmployeeId
                                left join RoleMasterTB rm1 on rm1.RoleId = med.HODEmployeeRoleId
                                where med.UnitId = @UnitId and med.MasterCompanyId = @MasterCompanyId and med.AccountStatus = 1";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@MasterCompanyId", Convert.ToInt32(HttpContext.Current.Session["MasterCompanyId"]));
                    cmd.Parameters.AddWithValue("@UnitId", CompanyUnitId);
                    con.Open();

                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new NotificationDataForEmpAndHOD
                            {
                                EmployeeId = sdr["EmployeeId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EmployeeId"]),
                                RoleIdOfEmployee = sdr["RoleIdOfEmployee"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["RoleIdOfEmployee"]),
                                FirstNameOfEmployee = sdr["FirstNameOfEmployee"] == DBNull.Value ? string.Empty : sdr["FirstNameOfEmployee"].ToString(),
                                LastNameOfEmployee = sdr["LastNameOfEmployee"] == DBNull.Value ? string.Empty : sdr["LastNameOfEmployee"].ToString(),
                                RoleNameOfEmployee = sdr["RoleNameOfEmployee"] == DBNull.Value ? string.Empty : sdr["RoleNameOfEmployee"].ToString(),
                                EmailIdOfEmployee = sdr["EmailIdOfEmployee"] == DBNull.Value ? string.Empty : sdr["EmailIdOfEmployee"].ToString(),
                                EmployeeIdOfHOD = sdr["EmployeeIdOfHOD"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["EmployeeIdOfHOD"]),
                                RoleIdOfHOD = sdr["RoleIdOfHOD"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["RoleIdOfHOD"]),
                                FirstNameOfHOD = sdr["FirstNameOfHOD"] == DBNull.Value ? string.Empty : sdr["FirstNameOfHOD"].ToString(),
                                LastNameOfHOD = sdr["LastNameOfHOD"] == DBNull.Value ? string.Empty : sdr["LastNameOfHOD"].ToString(),
                                RoleNameOfHOD = sdr["RoleNameOfHOD"] == DBNull.Value ? string.Empty : sdr["RoleNameOfHOD"].ToString(),
                                EmailIdOfHOD = sdr["EmailIdOfHOD"] == DBNull.Value ? string.Empty : sdr["EmailIdOfHOD"].ToString()
                            });
                        }
                    }
                }
            }

            return items;
        }

        public string GenerateEmailBodyForIGIRApproval(string PoNumber, string IGIRNumber, string ApproverName, string ProjectCode, string RequesterName)
        {
            DataAccessLayer objDB = new DataAccessLayer();
            var emailTemplateDataGet = objDB.EmailTemplateDataDisplay().FirstOrDefault(p => p.EmailType == "IGIRApprovalByQC");
            string emailBodyFromDatabase = emailTemplateDataGet.Body;
            string projectUrl = HttpContext.Current.Session["Project_URL"].ToString();
            string WebsiteURL = HttpContext.Current.Session["WebsiteURL"].ToString();
            string LogoURL = "/assets/img/logo.png";

            // Get the item details from Session
            string itemsTableRows = HttpContext.Current.Session["ItemsTableRows"]?.ToString() ?? "";

            emailBodyFromDatabase = emailBodyFromDatabase.Replace("{CompanyLogoURL}", WebsiteURL + LogoURL)
                .Replace("{PoNumber}", PoNumber)
                .Replace("{IGIRNumber}", IGIRNumber)
                .Replace("{ProjectCode}", ProjectCode)
                .Replace("{ApproverName}", ApproverName)
                .Replace("{RequesterName}", RequesterName)
                .Replace("{ApprovalLink}", WebsiteURL + "/Login.aspx");

            return emailBodyFromDatabase;
        }

        public string GenerateEmailBodyForMRApproval(string MRNumber, string ApproverName, string ProjectCode, string RequiredDate, string RequesterName)
        {
            DataAccessLayer objDB = new DataAccessLayer();
            var emailTemplateDataGet = objDB.EmailTemplateDataDisplay().FirstOrDefault(p => p.EmailType == "MR-ApprovalByHOD");
            string emailBodyFromDatabase = emailTemplateDataGet.Body;
            string projectUrl = HttpContext.Current.Session["Project_URL"].ToString();
            string WebsiteURL = HttpContext.Current.Session["WebsiteURL"].ToString();
            string LogoURL = "/assets/img/logo.png";

            // Get the item details from Session
            string itemsTableRows = HttpContext.Current.Session["ItemsTableRows"]?.ToString() ?? "";

            emailBodyFromDatabase = emailBodyFromDatabase.Replace("{CompanyLogoURL}", WebsiteURL + LogoURL)
                .Replace("{MRNumber}", MRNumber)
                .Replace("{ApproverName}", ApproverName)
                .Replace("{ProjectCode}", ProjectCode)
                .Replace("{RequesterName}", RequesterName)
                .Replace("{RequiredDate}", RequiredDate)
                .Replace("{ApprovalLink}", WebsiteURL + "/Login.aspx")
                .Replace("{ItemsTableRows}", itemsTableRows);

            return emailBodyFromDatabase;
        }

        public string GenerateEmailBodyForGRNApproval(string PoNumber, string ApproverName, string GRNNumber, string IGIRNumber, string ProjectCode, string RequesterName)
        {
            DataAccessLayer objDB = new DataAccessLayer();
            var emailTemplateDataGet = objDB.EmailTemplateDataDisplay().FirstOrDefault(p => p.EmailType == "GRN_ApprovalToHOD");
            string emailBodyFromDatabase = emailTemplateDataGet.Body;
            string projectUrl = HttpContext.Current.Session["Project_URL"].ToString();
            string WebsiteURL = HttpContext.Current.Session["WebsiteURL"].ToString();
            string LogoURL = "/assets/img/logo.png";

            emailBodyFromDatabase = emailBodyFromDatabase.Replace("{CompanyLogoURL}", WebsiteURL + LogoURL)
                .Replace("{ApproverName}", ApproverName)
                .Replace("{RequesterName}", RequesterName)
                .Replace("{GRNNumber}", GRNNumber)
                .Replace("{IGIRNumber}", IGIRNumber)
                .Replace("{ProjectCode}", ProjectCode)
                .Replace("{ApprovalLink}", WebsiteURL + "/Login.aspx")
                .Replace("{PoNumber}", PoNumber);

            return emailBodyFromDatabase;
        }

        public string GenerateEmailBodyForDeliveryDateOver(string PoNumber, string DeliveryDate)
        {
            DataAccessLayer objDB = new DataAccessLayer();
            var emailTemplateDataGet = objDB.EmailTemplateDataDisplay().FirstOrDefault(p => p.EmailType == "Delivery_Date_Overdue");
            string emailBodyFromDatabase = emailTemplateDataGet.Body;
            string projectUrl = HttpContext.Current.Session["Project_URL"].ToString();

            emailBodyFromDatabase = emailBodyFromDatabase.Replace("{CompanyName}", HttpContext.Current.Session["CompanyName"].ToString())
                .Replace("{EmployeeName}", "")
                .Replace("{PoNumber}", PoNumber)
                .Replace("{DeliveryDate}", DeliveryDate);

            return emailBodyFromDatabase;
        }

        public bool UpdateEmailNotificationData(string subject, int isEnable)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ToString()))
            {
                string updateQuery = @"UPDATE EmailNotification SET EN_IsEnableEmail = @EN_IsEnableEmail 
                                        WHERE EN_EmployeeId = @EN_EmployeeId AND EN_Subject = @EN_Subject";

                using (SqlCommand command = new SqlCommand(updateQuery, con))
                {
                    command.Parameters.AddWithValue("@EN_IsEnableEmail", isEnable);
                    command.Parameters.AddWithValue("@EN_Subject", subject);
                    command.Parameters.AddWithValue("@EN_EmployeeId", Convert.ToInt32(HttpContext.Current.Session["UserID"]));
                    con.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    con.Close();
                    return rowsAffected > 0;
                }
            }
        }

        public bool UpdateSimpleNotificationData(string subject, int isEnable)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ToString()))
            {
                string updateQuery = @"UPDATE EmailNotification SET EN_IsEnableNotification = @EN_IsEnableNotification 
                                        WHERE EN_EmployeeId = @EN_EmployeeId AND EN_Subject = @EN_Subject";

                using (SqlCommand command = new SqlCommand(updateQuery, con))
                {
                    command.Parameters.AddWithValue("@EN_IsEnableNotification", isEnable);
                    command.Parameters.AddWithValue("@EN_Subject", subject);
                    command.Parameters.AddWithValue("@EN_EmployeeId", Convert.ToInt32(HttpContext.Current.Session["UserID"]));
                    con.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    con.Close();
                    return rowsAffected > 0;
                }
            }
        }

        public List<EmailNotificationData> SaveMailBoxDisplay()
        {
            List<EmailNotificationData> items = new List<EmailNotificationData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM EmailBoxMaster WHERE M_MasterCompanyId = @M_MasterCompanyId AND M_CompanyUnitId = @M_CompanyUnitId ORDER BY M_TotalEmailSent DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@M_MasterCompanyId", Convert.ToInt32(HttpContext.Current.Session["MasterCompanyId"]));
                    cmd.Parameters.AddWithValue("@M_CompanyUnitId", Convert.ToInt32(HttpContext.Current.Session["CompanyUnitId"]));
                    con.Open();

                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            EmailNotificationData item = new EmailNotificationData
                            {
                                MailId = sdr["MailId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["MailId"]),
                                M_CreatedEmployeeId = sdr["M_CreatedEmployeeId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["M_CreatedEmployeeId"]),
                                M_FromAddress = sdr["M_FromAddress"] == DBNull.Value ? string.Empty : Convert.ToString(sdr["M_FromAddress"]),
                                M_ToAddress = sdr["M_ToAddress"] == DBNull.Value ? string.Empty : Convert.ToString(sdr["M_ToAddress"]),
                                M_CCAddress = sdr["M_CCAddress"] == DBNull.Value ? string.Empty : Convert.ToString(sdr["M_CCAddress"]),
                                M_BCCAddress = sdr["M_BCCAddress"] == DBNull.Value ? string.Empty : Convert.ToString(sdr["M_BCCAddress"]),
                                M_Subject = sdr["M_Subject"] == DBNull.Value ? string.Empty : Convert.ToString(sdr["M_Subject"]),
                                M_Body = sdr["M_Body"] == DBNull.Value ? string.Empty : Convert.ToString(sdr["M_Body"]),
                                M_Purpose = sdr["M_Purpose"] == DBNull.Value ? string.Empty : Convert.ToString(sdr["M_Purpose"]),
                                M_FileName = sdr["M_FileName"] == DBNull.Value ? string.Empty : Convert.ToString(sdr["M_FileName"]),
                                M_FileType = sdr["M_FileType"] == DBNull.Value ? string.Empty : Convert.ToString(sdr["M_FileType"]),
                                M_EmailStatus = sdr["M_EmailStatus"] == DBNull.Value ? string.Empty : Convert.ToString(sdr["M_EmailStatus"]),
                                M_MasterCompanyId = sdr["M_MasterCompanyId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["M_MasterCompanyId"]),
                                M_CompanyUnitId = sdr["M_CompanyUnitId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["M_CompanyUnitId"]),
                                M_CreatedEmployeeRoleId = sdr["M_CreatedEmployeeRoleId"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["M_CreatedEmployeeRoleId"]),
                                M_MailCounter = sdr["M_MailCounter"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["M_MailCounter"]),
                                M_TotalEmailSent = sdr["M_TotalEmailSent"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["M_TotalEmailSent"]),
                                M_IsSend = sdr["M_IsSend"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["M_IsSend"]),
                                M_IsRead = sdr["M_IsRead"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["M_IsRead"]),
                                M_CreatedDateTime = sdr["M_CreatedDateTime"] == DBNull.Value ? string.Empty : Convert.ToString(sdr["M_CreatedDateTime"]),
                                M_UpdatedDateTime = sdr["M_UpdatedDateTime"] == DBNull.Value ? string.Empty : Convert.ToString(sdr["M_UpdatedDateTime"])
                            };
                            items.Add(item);
                        }
                    }
                }
            }
            return items;
        }

        public EmailNotificationData GetLastEmailNotificationData()
        {
            List<EmailNotificationData> emailList = SaveMailBoxDisplay();
            if (emailList.Count > 0)
            {
                return emailList.First();
            }
            else
            {
                return null;
            }
        }

        public int EmailNotificationSave(string fromAddress, string toAddress, string ccAddress, string bccAddress, string subject, string body,
            string purpose, string fileName, string fileType, string emailStatus, int mailCounter, int TotalMailCounter)
        {
            try
            {
                con.Open();
                int prevIdentity = GetReseedIdentity("EmailBoxMaster", "MailId");
                ReseedIdentity("EmailBoxMaster", prevIdentity);
                con.Close();

                string query = @"Insert into EmailBoxMaster
                                   (M_FromAddress, M_ToAddress, M_CCAddress, M_BCCAddress, M_Subject, M_Body, M_Purpose, M_FileName, M_FileType, 
                                    M_EmailStatus, M_MasterCompanyId, M_CompanyUnitId, M_CreatedEmployeeId, M_CreatedEmployeeRoleId, 
                                    M_MailCounter, M_TotalEmailSent, M_IsSend, M_CreatedDateTime)
                                 Output INSERTED.MailId values 
                                   (@M_FromAddress, @M_ToAddress, @M_CCAddress, @M_BCCAddress, @M_Subject, @M_Body, @M_Purpose, @M_FileName, @M_FileType, 
                                    @M_EmailStatus, @M_MasterCompanyId, @M_CompanyUnitId, @M_CreatedEmployeeId, @M_CreatedEmployeeRoleId, 
                                    @M_MailCounter, @M_TotalEmailSent, @M_IsSend, @M_CreatedDateTime)";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Close();
                    cmd.Parameters.AddWithValue("@M_FromAddress", fromAddress);
                    cmd.Parameters.AddWithValue("@M_ToAddress", toAddress);
                    cmd.Parameters.AddWithValue("@M_CCAddress", ccAddress);
                    cmd.Parameters.AddWithValue("@M_BCCAddress", bccAddress);
                    cmd.Parameters.AddWithValue("@M_Subject", subject);
                    cmd.Parameters.AddWithValue("@M_Body", body);
                    cmd.Parameters.AddWithValue("@M_Purpose", purpose);
                    cmd.Parameters.AddWithValue("@M_FileName", fileName);
                    cmd.Parameters.AddWithValue("@M_FileType", fileType);
                    cmd.Parameters.AddWithValue("@M_EmailStatus", emailStatus);
                    cmd.Parameters.AddWithValue("@M_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    cmd.Parameters.AddWithValue("@M_CompanyUnitId", HttpContext.Current.Session["CompanyUnitId"].ToString());
                    cmd.Parameters.AddWithValue("@M_CreatedEmployeeId", HttpContext.Current.Session["UserID"].ToString());
                    cmd.Parameters.AddWithValue("@M_CreatedEmployeeRoleId", HttpContext.Current.Session["RoleId"].ToString());
                    cmd.Parameters.AddWithValue("@M_MailCounter", mailCounter);
                    cmd.Parameters.AddWithValue("@M_TotalEmailSent", TotalMailCounter);
                    cmd.Parameters.AddWithValue("@M_IsSend", 1);
                    cmd.Parameters.AddWithValue("@M_CreatedDateTime", date + " " + time);

                    con.Open();
                    int getID = (int)cmd.ExecuteScalar();
                    con.Close();

                    if (getID > 0)
                    {
                        return getID;
                    }
                    else
                    {
                        return 0;
                    }
                    ;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool NotificationDelete()
        {
            try
            {
                DateTime data1 = DateTime.Now.AddMinutes(-1);
                DateTime data2 = DateTime.Now.AddHours(-12);
                DateTime data3 = DateTime.Now.AddDays(-1);

                string formattedData1 = data1.ToString("yyyy-MM-dd HH:mm:ss");
                string formattedData2 = data2.ToString("yyyy-MM-dd HH:mm:ss");
                string formattedData3 = data3.ToString("yyyy-MM-dd HH:mm:ss");

                string query = "DELETE FROM NotificationMaster WHERE IsReadData = @IsReadData AND SenderUpdatedDateTime < @SenderUpdatedDateTime";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IsReadData", 1);
                    cmd.Parameters.AddWithValue("@SenderUpdatedDateTime", formattedData2);
                    con.Open();
                    int i = cmd.ExecuteNonQuery();
                    con.Close();

                    return (i > 0);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool NotificationSend(string senderDepartment, string subject, string description, string receiverEmployeeId)
        {
            try
            {
                con.Open();
                int prevIdentity = GetReseedIdentity("NotificationMaster", "NotificationId");
                ReseedIdentity("NotificationMaster", prevIdentity);
                con.Close();

                string query = @"Insert into NotificationMaster
                                 (SenderMasterCompanyId, SenderEmployeeId, SenderRoleId, SenderCompanyUnitId, SenderEmailId, SenderDepartment, 
                                  Subject, Description, IsReadData, ReceiverEmployeeId, SenderCreatedDateTime) 
                                  output INSERTED.NotificationId values 
                                 (@SenderMasterCompanyId, @SenderEmployeeId, @SenderRoleId, @SenderCompanyUnitId, @SenderEmailId, @SenderDepartment, 
                                  @Subject, @Description, @IsReadData, @ReceiverEmployeeId, @SenderCreatedDateTime)";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Close();
                    cmd.Parameters.AddWithValue("@SenderMasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    cmd.Parameters.AddWithValue("@SenderEmployeeId", HttpContext.Current.Session["UserID"].ToString());
                    cmd.Parameters.AddWithValue("@SenderRoleId", HttpContext.Current.Session["RoleId"].ToString());
                    cmd.Parameters.AddWithValue("@SenderCompanyUnitId", HttpContext.Current.Session["CompanyUnitId"].ToString());
                    cmd.Parameters.AddWithValue("@SenderEmailId", HttpContext.Current.Session["EmailId"].ToString());
                    cmd.Parameters.AddWithValue("@SenderDepartment", senderDepartment);
                    cmd.Parameters.AddWithValue("@Subject", subject);
                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.Parameters.AddWithValue("@IsReadData", 0);
                    cmd.Parameters.AddWithValue("@ReceiverEmployeeId", Convert.ToInt32(receiverEmployeeId));
                    cmd.Parameters.AddWithValue("@SenderCreatedDateTime", date + " " + time);

                    con.Open();
                    int getID = (int)cmd.ExecuteScalar();
                    con.Close();

                    return (getID > 0);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /* AuditLogData use for before Login */
        public bool AuditLogDataBeforeLogin(string employeeId, string emailId, string RoleId, string masterCompanyId, string companyUnitId, string LogEvent, string TableName, string TableRowId, string description)
        {
            try
            {
                con.Open();
                int prevIdentity = GetReseedIdentity("AuditLogMaster", "AuditLogId");
                ReseedIdentity("AuditLogMaster", prevIdentity);
                con.Close();

                string query = @"Insert into AuditLogMaster 
                                (EmployeeId,EmailId, RoleId, CompanyUnitId, MasterCompanyId, LogEvent, TableName, TableRowId, Description, CreatedDateTime) 
                                values 
                                (@EmployeeId,@EmailId, @RoleId, @CompanyUnitId, @MasterCompanyId, @LogEvent, @TableName, @TableRowId, @Description, @CreatedDateTime)";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                    cmd.Parameters.AddWithValue("@EmailId", emailId);
                    cmd.Parameters.AddWithValue("@RoleId", RoleId);
                    cmd.Parameters.AddWithValue("@MasterCompanyId", masterCompanyId);
                    cmd.Parameters.AddWithValue("@CompanyUnitId", companyUnitId);
                    cmd.Parameters.AddWithValue("@LogEvent", LogEvent);
                    cmd.Parameters.AddWithValue("@TableName", TableName);
                    cmd.Parameters.AddWithValue("@TableRowId", TableRowId);
                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.Parameters.AddWithValue("@CreatedDateTime", date + " " + time);

                    con.Open();
                    int i = cmd.ExecuteNonQuery();
                    con.Close();

                    return (i > 0);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool AuditLogData(string LogEvent, string TableName, string TableRowId, string description)
        {
            try
            {
                con.Close();
                con.Open();
                int prevIdentity = GetReseedIdentity("AuditLogMaster", "AuditLogId");
                ReseedIdentity("AuditLogMaster", prevIdentity);
                con.Close();

                string query = @"Insert into AuditLogMaster 
                                (EmployeeId,EmailId, RoleId, CompanyUnitId, MasterCompanyId, LogEvent, TableName, TableRowId, Description, CreatedDateTime) 
                                values 
                                (@EmployeeId,@EmailId, @RoleId, @CompanyUnitId, @MasterCompanyId, @LogEvent, @TableName, @TableRowId, @Description, @CreatedDateTime)";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", HttpContext.Current.Session["UserID"].ToString());
                    cmd.Parameters.AddWithValue("@EmailId", HttpContext.Current.Session["EmailId"].ToString());
                    cmd.Parameters.AddWithValue("@RoleId", HttpContext.Current.Session["RoleId"].ToString());
                    cmd.Parameters.AddWithValue("@MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    cmd.Parameters.AddWithValue("@CompanyUnitId", HttpContext.Current.Session["CompanyUnitId"].ToString());
                    cmd.Parameters.AddWithValue("@LogEvent", LogEvent);
                    cmd.Parameters.AddWithValue("@TableName", TableName);
                    cmd.Parameters.AddWithValue("@TableRowId", TableRowId);
                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.Parameters.AddWithValue("@CreatedDateTime", date + " " + time);

                    con.Open();
                    int i = cmd.ExecuteNonQuery();
                    con.Close();

                    return (i > 0);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ErrorLogData(string methodName, string errorMessage, string errorDescription, string errorLineNumber)
        {
            try
            {
                con.Close();
                con.Open();
                int prevIdentity = GetReseedIdentity("ErrorLogMaster", "ErrorLogId");
                ReseedIdentity("ErrorLogMaster", prevIdentity);
                con.Close();

                string query = @"Insert into ErrorLogMaster (MethodName, ErrorMessage, ErrorDescription,ErrorLineNumber, CreatedDateTime) 
                                 values (@MethodName, @ErrorMessage, @ErrorDescription, @ErrorLineNumber, @CreatedDateTime)";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@MethodName", methodName);
                    cmd.Parameters.AddWithValue("@ErrorMessage", errorMessage);
                    cmd.Parameters.AddWithValue("@ErrorDescription", errorDescription);
                    cmd.Parameters.AddWithValue("@ErrorLineNumber", errorLineNumber);
                    cmd.Parameters.AddWithValue("@CreatedDateTime", date + " " + time);

                    con.Open();
                    int i = cmd.ExecuteNonQuery();
                    con.Close();

                    return (i > 0);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /* Data Table use for Data Get/Bind */
        public DataTable MaterialRequestDataBind(string MRM_NumberGet)
        {
            DataTable dt = new DataTable();

            string query = @"SELECT * FROM MaterialRequestMaster mrm
                                INNER JOIN MaterialRequestDetails mrd ON mrd.MR_MRMID = mrm.MRM_ID
                                INNER JOIN MasterEmployeeDetails medr ON medr.EmployeeId = mrm.MRM_EmployeeID
                                INNER JOIN CompanyUnitMaster cum ON cum.CompanyUnitId = mrm.MRM_CompanyUnitId
                                INNER JOIN StatusMasterTB sm ON sm.StatusID = mrm.MRM_StatusId
                                WHERE MRM_MasterCompanyId = @MRM_MasterCompanyId AND MRM_IsActive = @MRM_IsActive AND MRM_NUMBER = @MRM_NUMBER
                                ORDER BY CONVERT(datetime, mrm.MRM_CreatedDateTime, 120) DESC
                                ";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                int masterCompanyID = Convert.ToInt32(HttpContext.Current.Session["MasterCompanyId"].ToString());
                cmd.Parameters.AddWithValue("@MRM_IsActive", 1);
                cmd.Parameters.AddWithValue("@MRM_NUMBER", MRM_NumberGet);
                cmd.Parameters.AddWithValue("@MRM_MasterCompanyId", masterCompanyID);
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                con.Close();
                return dt;
            }
        }

        public DataTable MaterialRequestDataBind(string MRM_NumberGet, string MRItemCode)
        {
            DataTable dt = new DataTable();

            string query = @"SELECT * FROM MaterialRequestMaster mrm
                                INNER JOIN MaterialRequestDetails mrd ON mrd.MR_MRMID = mrm.MRM_ID
                                INNER JOIN POStatus ps ON ps.POStatusID = mrm.MRM_StatusId
                                INNER JOIN Users u ON u.UserID = mrm.MRM_EmployeeID
                                INNER JOIN Employees e ON e.ID = u.employeeID
                                WHERE MRM_MasterCompanyId = @MRM_MasterCompanyId AND MRM_IsActive = @MRM_IsActive AND MRM_NUMBER = @MRM_NUMBER AND 
                                MR_ItemCode = @MR_ItemCode
                                ORDER BY CONVERT(datetime, mrm.MRM_CreatedDateTime, 120) DESC
                                ";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                int masterCompanyID = Convert.ToInt32(HttpContext.Current.Session["MasterCompanyId"].ToString());
                cmd.Parameters.AddWithValue("@MRM_IsActive", 1);
                cmd.Parameters.AddWithValue("@MRM_NUMBER", MRM_NumberGet);
                cmd.Parameters.AddWithValue("@MRM_MasterCompanyId", masterCompanyID);
                cmd.Parameters.AddWithValue("@MR_ItemCode", MRItemCode);
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                con.Close();
                return dt;
            }
        }

        public List<ItemTransactionData> PoNumberFind(int itemId, string poNumber)
        {
            List<ItemTransactionData> allData = new List<ItemTransactionData>();


            string query = @"SELECT * FROM ItemTransactionMaster WHERE IT_PoNumber = @IT_PoNumber AND IT_ItemId = @IT_ItemId AND IT_Id = (SELECT MIN(IT_Id) FROM ItemTransactionMaster WHERE IT_PoNumber = @IT_PoNumber AND it_itemId = @IT_ItemId)";

            using (SqlCommand command = new SqlCommand(query, con))
            {
                command.Parameters.AddWithValue("@IT_ItemId", itemId);
                command.Parameters.AddWithValue("@IT_PoNumber", poNumber);
                //command.Parameters.AddWithValue("@IT_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                //command.Parameters.AddWithValue("@IT_CreatedEmployeeId", HttpContext.Current.Session["UserID"].ToString());
                con.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ItemTransactionData item = new ItemTransactionData
                        {
                            IT_Id = Convert.ToInt32(reader["IT_Id"].ToString()),
                        };

                        allData.Add(item);
                    }
                }
            }
            con.Close();
            return allData;
        }

        /* PO Search */
        public List<ItemTransactionData> PoNumberSearch1(string prefixText)
        {
            List<ItemTransactionData> allData = new List<ItemTransactionData>();
            con.Open();

            string query = @"SELECT DISTINCT IT_PoNumber FROM ItemTransactionMaster WHERE IT_PoNumber LIKE @PrefixText + '%' 
                             AND IT_MasterCompanyId = @IT_MasterCompanyId AND IT_CompanyUnitId = @IT_CompanyUnitId AND IT_CreatedEmployeeId = @IT_CreatedEmployeeId";

            using (SqlCommand command = new SqlCommand(query, con))
            {
                command.Parameters.AddWithValue("@PrefixText", prefixText);
                command.Parameters.AddWithValue("@IT_CompanyUnitId", HttpContext.Current.Session["CompanyUnitId"].ToString());
                command.Parameters.AddWithValue("@IT_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                command.Parameters.AddWithValue("@IT_CreatedEmployeeId", HttpContext.Current.Session["UserID"].ToString());

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ItemTransactionData item = new ItemTransactionData
                        {
                            IT_PoNumber = reader["IT_PoNumber"].ToString(),
                        };

                        allData.Add(item);
                    }
                }
            }
            return allData;
        }

        public List<string> PoNumberSearch(string prefixText)
        {
            List<string> suggestions = new List<string>();

            string query = @"SELECT DISTINCT PO_Number FROM PoExcelUploadMaster WHERE PO_Number LIKE @PrefixText + '%' 
                              AND PO_MasterCompanyId = @PO_MasterCompanyId AND PO_CompanyUnitId = @PO_CompanyUnitId";

            using (SqlCommand command = new SqlCommand(query, con))
            {
                command.Parameters.AddWithValue("@PrefixText", prefixText);
                command.Parameters.AddWithValue("@PO_CompanyUnitId", HttpContext.Current.Session["CompanyUnitId"].ToString());
                command.Parameters.AddWithValue("@PO_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                command.Parameters.AddWithValue("@PO_CreatedEmployeeId", HttpContext.Current.Session["UserID"].ToString());
                con.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string poNumber = reader["Po_Number"].ToString();
                        suggestions.Add(poNumber);
                    }
                }
                con.Close();
            }

            return suggestions;
        }

        public List<string> PoNumberSearchNew(string prefixText)
        {
            List<string> suggestions = new List<string>();

            string query = @"SELECT DISTINCT PONumber FROM PurchaseOrder WHERE PONumber LIKE @PrefixText + '%'";

            using (SqlCommand command = new SqlCommand(query, con))
            {
                command.Parameters.AddWithValue("@PrefixText", prefixText);
                command.Parameters.AddWithValue("@PO_CompanyUnitId", HttpContext.Current.Session["CompanyUnitId"].ToString());
                command.Parameters.AddWithValue("@PO_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                command.Parameters.AddWithValue("@PO_CreatedEmployeeId", HttpContext.Current.Session["UserID"].ToString());
                con.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string poNumber = reader["PoNumber"].ToString();
                        suggestions.Add(poNumber);
                    }
                }
                con.Close();
            }

            return suggestions;
        }

        public List<string> GRNNumberSearch(string prefixText)
        {
            List<string> suggestions = new List<string>();

            string query = @"SELECT DISTINCT GRN_NUMBER FROM GRNMaster 
                             WHERE GRN_NUMBER LIKE @PrefixText + '%' 
                             AND GRN_MasterCompanyId = @GRN_MasterCompanyId 
                             AND GRN_CompanyUnitId = @GRN_CompanyUnitId AND GRN_Status = 7";

            using (SqlCommand command = new SqlCommand(query, con))
            {
                command.Parameters.AddWithValue("@PrefixText", prefixText);
                command.Parameters.AddWithValue("@GRN_CompanyUnitId", HttpContext.Current.Session["CompanyUnitId"].ToString());
                command.Parameters.AddWithValue("@GRN_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                con.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string GRNnumber = reader["GRN_NUMBER"].ToString();
                        suggestions.Add(GRNnumber);
                    }
                }
                con.Close();
            }

            return suggestions;
        }

        public List<MaterialRequestData> MaterialRequestGetData()
        {
            List<MaterialRequestData> items = new List<MaterialRequestData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM MaterialRequestMaster mrm
                                 INNER JOIN MaterialRequestDetails mrd ON mrd.MR_MRMID = mrm.MRM_ID
                                 INNER JOIN POStatus ps ON ps.POStatusID = mrm.MRM_StatusId
                                 INNER JOIN Users u ON u.UserID = mrm.MRM_EmployeeID
                                 INNER JOIN Employees e ON e.ID = u.employeeID
                                 Where MRM_MasterCompanyId = @MRM_MasterCompanyId
                                 ORDER BY CONVERT(datetime, mrm.MRM_CreatedDateTime, 120) DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@MRM_IsActive", 1);
                    cmd.Parameters.AddWithValue("@MRM_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new MaterialRequestData
                            {
                                MRM_ID = Convert.IsDBNull(sdr["MRM_ID"]) ? 0 : Convert.ToInt32(sdr["MRM_ID"]),
                                MRM_MasterCompanyId = Convert.IsDBNull(sdr["MRM_MasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["MRM_MasterCompanyId"]),
                                MRM_EmployeeID = Convert.IsDBNull(sdr["MRM_EmployeeID"]) ? 0 : Convert.ToInt32(sdr["MRM_EmployeeID"]),
                                MRM_CompanyUnitId = Convert.IsDBNull(sdr["MRM_CompanyUnitId"]) ? 0 : Convert.ToInt32(sdr["MRM_CompanyUnitId"]),
                                MRM_StatusId = Convert.IsDBNull(sdr["MRM_StatusId"]) ? 0 : Convert.ToInt32(sdr["MRM_StatusId"]),
                                MRM_IsActive = Convert.IsDBNull(sdr["MRM_IsActive"]) ? 0 : Convert.ToInt32(sdr["MRM_IsActive"]),
                                MRM_CounterYear = Convert.IsDBNull(sdr["MRM_CounterYear"]) ? 0 : Convert.ToInt32(sdr["MRM_CounterYear"]),
                                MRM_CounterNumber = Convert.IsDBNull(sdr["MRM_CounterNumber"]) ? 0 : Convert.ToInt32(sdr["MRM_CounterNumber"]),
                                MRM_NUMBER = Convert.IsDBNull(sdr["MRM_NUMBER"]) ? string.Empty : sdr["MRM_NUMBER"].ToString(),
                                MRM_ApprovedEmployeeID = Convert.IsDBNull(sdr["MRM_ApprovedEmployeeID"]) ? 0 : Convert.ToInt32(sdr["MRM_ApprovedEmployeeID"]),
                                MRM_ApprovedDateTime = Convert.IsDBNull(sdr["MRM_ApprovedDateTime"]) ? string.Empty : sdr["MRM_ApprovedDateTime"].ToString(),
                                MRM_CreatedDateTime = Convert.IsDBNull(sdr["MRM_CreatedDateTime"]) ? string.Empty : sdr["MRM_CreatedDateTime"].ToString(),
                                MRM_UpdatedDateTime = Convert.IsDBNull(sdr["MRM_UpdatedDateTime"]) ? string.Empty : sdr["MRM_UpdatedDateTime"].ToString(),
                                MRM_HODEmployeeID = Convert.IsDBNull(sdr["MRM_HODEmployeeID"]) ? 0 : Convert.ToInt32(sdr["MRM_HODEmployeeID"]),
                                MRM_ProjectCode = Convert.IsDBNull(sdr["MRM_ProjectCode"]) ? string.Empty : sdr["MRM_ProjectCode"].ToString(),

                                MR_ID = Convert.IsDBNull(sdr["MR_ID"]) ? 0 : Convert.ToInt32(sdr["MR_ID"]),
                                MR_StatusId = Convert.IsDBNull(sdr["MR_StatusId"]) ? 0 : Convert.ToInt32(sdr["MR_StatusId"]),
                                MR_MRMID = Convert.IsDBNull(sdr["MR_MRMID"]) ? 0 : Convert.ToInt32(sdr["MR_MRMID"]),
                                MR_NUMBER = Convert.IsDBNull(sdr["MR_NUMBER"]) ? string.Empty : sdr["MR_NUMBER"].ToString(),
                                MR_ItemCode = Convert.IsDBNull(sdr["MR_ItemCode"]) ? string.Empty : sdr["MR_ItemCode"].ToString(),
                                MR_ItemName = Convert.IsDBNull(sdr["MR_ItemName"]) ? string.Empty : sdr["MR_ItemName"].ToString(),
                                MR_UOM = Convert.IsDBNull(sdr["MR_UOM"]) ? string.Empty : sdr["MR_UOM"].ToString(),
                                MR_CurrentStock = Convert.IsDBNull(sdr["MR_CurrentStock"]) ? 0 : Convert.ToDecimal(sdr["MR_CurrentStock"]),
                                MR_MinStock = Convert.IsDBNull(sdr["MR_MinStock"]) ? 0 : Convert.ToDecimal(sdr["MR_MinStock"]),
                                MR_EmployeeRequireItemQty = Convert.IsDBNull(sdr["MR_EmployeeRequireItemQty"]) ? 0 : Convert.ToDecimal(sdr["MR_EmployeeRequireItemQty"]),
                                MR_ReceiveQty = Convert.IsDBNull(sdr["MR_ReceiveQty"]) ? 0 : Convert.ToDecimal(sdr["MR_ReceiveQty"]),
                                MR_BalanceQty = Convert.IsDBNull(sdr["MR_BalanceQty"]) ? 0 : Convert.ToDecimal(sdr["MR_BalanceQty"]),
                                MR_RequireDate = Convert.IsDBNull(sdr["MR_RequireDate"]) ? string.Empty : sdr["MR_RequireDate"].ToString(),

                                ApprovalFirstName = sdr["EmployeeName"].ToString(),
                                StatusTitle = sdr["POStatus"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<MaterialRequestData> MaterialRequestGetDataforApprovalBothShow()
        {
            List<MaterialRequestData> items = new List<MaterialRequestData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT *,e1.EmployeeName as ApprovalHOD FROM MaterialRequestMaster mrm
                                 INNER JOIN MaterialRequestDetails mrd ON mrd.MR_MRMID = mrm.MRM_ID
                                 INNER JOIN POStatus ps ON ps.POStatusID = mrm.MRM_StatusId
                                 INNER JOIN Users u ON u.UserID = mrm.MRM_EmployeeID
                                 INNER JOIN Employees e ON e.ID = u.employeeID
								 INNER JOIN Users u1 ON u1.UserID = mrm.MRM_ApprovedEmployeeID
                                 INNER JOIN Employees e1 ON e1.ID = u1.employeeID
                                 Where MRM_MasterCompanyId = @MRM_MasterCompanyId
                                 ORDER BY CONVERT(datetime, mrm.MRM_CreatedDateTime, 120) DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@MRM_IsActive", 1);
                    cmd.Parameters.AddWithValue("@MRM_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new MaterialRequestData
                            {
                                MRM_ID = Convert.IsDBNull(sdr["MRM_ID"]) ? 0 : Convert.ToInt32(sdr["MRM_ID"]),
                                MRM_MasterCompanyId = Convert.IsDBNull(sdr["MRM_MasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["MRM_MasterCompanyId"]),
                                MRM_EmployeeID = Convert.IsDBNull(sdr["MRM_EmployeeID"]) ? 0 : Convert.ToInt32(sdr["MRM_EmployeeID"]),
                                MRM_CompanyUnitId = Convert.IsDBNull(sdr["MRM_CompanyUnitId"]) ? 0 : Convert.ToInt32(sdr["MRM_CompanyUnitId"]),
                                MRM_StatusId = Convert.IsDBNull(sdr["MRM_StatusId"]) ? 0 : Convert.ToInt32(sdr["MRM_StatusId"]),
                                MRM_IsActive = Convert.IsDBNull(sdr["MRM_IsActive"]) ? 0 : Convert.ToInt32(sdr["MRM_IsActive"]),
                                MRM_CounterYear = Convert.IsDBNull(sdr["MRM_CounterYear"]) ? 0 : Convert.ToInt32(sdr["MRM_CounterYear"]),
                                MRM_CounterNumber = Convert.IsDBNull(sdr["MRM_CounterNumber"]) ? 0 : Convert.ToInt32(sdr["MRM_CounterNumber"]),
                                MRM_NUMBER = Convert.IsDBNull(sdr["MRM_NUMBER"]) ? string.Empty : sdr["MRM_NUMBER"].ToString(),
                                MRM_ApprovedEmployeeID = Convert.IsDBNull(sdr["MRM_ApprovedEmployeeID"]) ? 0 : Convert.ToInt32(sdr["MRM_ApprovedEmployeeID"]),
                                MRM_ApprovedDateTime = Convert.IsDBNull(sdr["MRM_ApprovedDateTime"]) ? string.Empty : sdr["MRM_ApprovedDateTime"].ToString(),
                                MRM_CreatedDateTime = Convert.IsDBNull(sdr["MRM_CreatedDateTime"]) ? string.Empty : sdr["MRM_CreatedDateTime"].ToString(),
                                MRM_UpdatedDateTime = Convert.IsDBNull(sdr["MRM_UpdatedDateTime"]) ? string.Empty : sdr["MRM_UpdatedDateTime"].ToString(),
                                MRM_HODEmployeeID = Convert.IsDBNull(sdr["MRM_HODEmployeeID"]) ? 0 : Convert.ToInt32(sdr["MRM_HODEmployeeID"]),
                                MRM_ProjectCode = Convert.IsDBNull(sdr["MRM_ProjectCode"]) ? string.Empty : sdr["MRM_ProjectCode"].ToString(),

                                MR_ID = Convert.IsDBNull(sdr["MR_ID"]) ? 0 : Convert.ToInt32(sdr["MR_ID"]),
                                MR_StatusId = Convert.IsDBNull(sdr["MR_StatusId"]) ? 0 : Convert.ToInt32(sdr["MR_StatusId"]),
                                MR_MRMID = Convert.IsDBNull(sdr["MR_MRMID"]) ? 0 : Convert.ToInt32(sdr["MR_MRMID"]),
                                MR_NUMBER = Convert.IsDBNull(sdr["MR_NUMBER"]) ? string.Empty : sdr["MR_NUMBER"].ToString(),
                                MR_ItemCode = Convert.IsDBNull(sdr["MR_ItemCode"]) ? string.Empty : sdr["MR_ItemCode"].ToString(),
                                MR_ItemName = Convert.IsDBNull(sdr["MR_ItemName"]) ? string.Empty : sdr["MR_ItemName"].ToString(),
                                MR_UOM = Convert.IsDBNull(sdr["MR_UOM"]) ? string.Empty : sdr["MR_UOM"].ToString(),
                                MR_CurrentStock = Convert.IsDBNull(sdr["MR_CurrentStock"]) ? 0 : Convert.ToDecimal(sdr["MR_CurrentStock"]),
                                MR_MinStock = Convert.IsDBNull(sdr["MR_MinStock"]) ? 0 : Convert.ToDecimal(sdr["MR_MinStock"]),
                                MR_EmployeeRequireItemQty = Convert.IsDBNull(sdr["MR_EmployeeRequireItemQty"]) ? 0 : Convert.ToDecimal(sdr["MR_EmployeeRequireItemQty"]),
                                MR_ReceiveQty = Convert.IsDBNull(sdr["MR_ReceiveQty"]) ? 0 : Convert.ToDecimal(sdr["MR_ReceiveQty"]),
                                MR_BalanceQty = Convert.IsDBNull(sdr["MR_BalanceQty"]) ? 0 : Convert.ToDecimal(sdr["MR_BalanceQty"]),
                                MR_RequireDate = Convert.IsDBNull(sdr["MR_RequireDate"]) ? string.Empty : sdr["MR_RequireDate"].ToString(),

                                FirstName = sdr["EmployeeName"].ToString(),
                                StatusTitle = sdr["POStatus"].ToString(),

                                ApprovalFirstName = sdr["ApprovalHOD"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }


        public List<MaterialRequestData> MaterialRequestGetDataforApprovalandReleased()
        {
            List<MaterialRequestData> items = new List<MaterialRequestData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT *,e1.EmployeeName as ApprovalHOD, e2.EmployeeName as RelesedHOD FROM MaterialRequestMaster mrm
                                 INNER JOIN MaterialRequestDetails mrd ON mrd.MR_MRMID = mrm.MRM_ID
								 INNER JOIN MaterialTransactionMaster mt ON mt.MT_MRMID = mrm.MRM_ID
								 INNER JOIN MaterialTransationSubData mts ON mts.MTS_MTID = mt.MT_ID
                                 INNER JOIN POStatus ps ON ps.POStatusID = mrm.MRM_StatusId
                                 INNER JOIN Users u ON u.UserID = mrm.MRM_EmployeeID
                                 INNER JOIN Employees e ON e.ID = u.employeeID
								 INNER JOIN Users u1 ON u1.UserID = mrm.MRM_ApprovedEmployeeID
                                 INNER JOIN Employees e1 ON e1.ID = u1.employeeID
                                 INNER JOIN Employees e2 ON e2.ID = mt.MT_EmployeeId
                                 Where MRM_MasterCompanyId = @MRM_MasterCompanyId
                                 ORDER BY CONVERT(datetime, mrm.MRM_CreatedDateTime, 120) DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@MRM_IsActive", 1);
                    cmd.Parameters.AddWithValue("@MRM_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new MaterialRequestData
                            {
                                MRM_ID = Convert.IsDBNull(sdr["MRM_ID"]) ? 0 : Convert.ToInt32(sdr["MRM_ID"]),
                                MRM_MasterCompanyId = Convert.IsDBNull(sdr["MRM_MasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["MRM_MasterCompanyId"]),
                                MRM_EmployeeID = Convert.IsDBNull(sdr["MRM_EmployeeID"]) ? 0 : Convert.ToInt32(sdr["MRM_EmployeeID"]),
                                MRM_CompanyUnitId = Convert.IsDBNull(sdr["MRM_CompanyUnitId"]) ? 0 : Convert.ToInt32(sdr["MRM_CompanyUnitId"]),
                                MRM_StatusId = Convert.IsDBNull(sdr["MRM_StatusId"]) ? 0 : Convert.ToInt32(sdr["MRM_StatusId"]),
                                MRM_IsActive = Convert.IsDBNull(sdr["MRM_IsActive"]) ? 0 : Convert.ToInt32(sdr["MRM_IsActive"]),
                                MRM_CounterYear = Convert.IsDBNull(sdr["MRM_CounterYear"]) ? 0 : Convert.ToInt32(sdr["MRM_CounterYear"]),
                                MRM_CounterNumber = Convert.IsDBNull(sdr["MRM_CounterNumber"]) ? 0 : Convert.ToInt32(sdr["MRM_CounterNumber"]),
                                MRM_NUMBER = Convert.IsDBNull(sdr["MRM_NUMBER"]) ? string.Empty : sdr["MRM_NUMBER"].ToString(),
                                MRM_ApprovedEmployeeID = Convert.IsDBNull(sdr["MRM_ApprovedEmployeeID"]) ? 0 : Convert.ToInt32(sdr["MRM_ApprovedEmployeeID"]),
                                MRM_ApprovedDateTime = Convert.IsDBNull(sdr["MRM_ApprovedDateTime"]) ? string.Empty : sdr["MRM_ApprovedDateTime"].ToString(),
                                MRM_CreatedDateTime = Convert.IsDBNull(sdr["MRM_CreatedDateTime"]) ? string.Empty : sdr["MRM_CreatedDateTime"].ToString(),
                                MRM_UpdatedDateTime = Convert.IsDBNull(sdr["MRM_UpdatedDateTime"]) ? string.Empty : sdr["MRM_UpdatedDateTime"].ToString(),
                                MRM_HODEmployeeID = Convert.IsDBNull(sdr["MRM_HODEmployeeID"]) ? 0 : Convert.ToInt32(sdr["MRM_HODEmployeeID"]),
                                MRM_ProjectCode = Convert.IsDBNull(sdr["MRM_ProjectCode"]) ? string.Empty : sdr["MRM_ProjectCode"].ToString(),

                                MR_ID = Convert.IsDBNull(sdr["MR_ID"]) ? 0 : Convert.ToInt32(sdr["MR_ID"]),
                                MR_StatusId = Convert.IsDBNull(sdr["MR_StatusId"]) ? 0 : Convert.ToInt32(sdr["MR_StatusId"]),
                                MR_MRMID = Convert.IsDBNull(sdr["MR_MRMID"]) ? 0 : Convert.ToInt32(sdr["MR_MRMID"]),
                                MR_NUMBER = Convert.IsDBNull(sdr["MR_NUMBER"]) ? string.Empty : sdr["MR_NUMBER"].ToString(),
                                MR_ItemCode = Convert.IsDBNull(sdr["MR_ItemCode"]) ? string.Empty : sdr["MR_ItemCode"].ToString(),
                                MR_ItemName = Convert.IsDBNull(sdr["MR_ItemName"]) ? string.Empty : sdr["MR_ItemName"].ToString(),
                                MR_UOM = Convert.IsDBNull(sdr["MR_UOM"]) ? string.Empty : sdr["MR_UOM"].ToString(),
                                MR_CurrentStock = Convert.IsDBNull(sdr["MR_CurrentStock"]) ? 0 : Convert.ToDecimal(sdr["MR_CurrentStock"]),
                                MR_MinStock = Convert.IsDBNull(sdr["MR_MinStock"]) ? 0 : Convert.ToDecimal(sdr["MR_MinStock"]),
                                MR_EmployeeRequireItemQty = Convert.IsDBNull(sdr["MR_EmployeeRequireItemQty"]) ? 0 : Convert.ToDecimal(sdr["MR_EmployeeRequireItemQty"]),
                                MR_ReceiveQty = Convert.IsDBNull(sdr["MR_ReceiveQty"]) ? 0 : Convert.ToDecimal(sdr["MR_ReceiveQty"]),
                                MR_BalanceQty = Convert.IsDBNull(sdr["MR_BalanceQty"]) ? 0 : Convert.ToDecimal(sdr["MR_BalanceQty"]),
                                MR_RequireDate = Convert.IsDBNull(sdr["MR_RequireDate"]) ? string.Empty : sdr["MR_RequireDate"].ToString(),

                                MT_ID = Convert.IsDBNull(sdr["MT_ID"]) ? 0 : Convert.ToInt32(sdr["MT_ID"]),
                                MT_MRID = Convert.IsDBNull(sdr["MT_MRID"]) ? 0 : Convert.ToInt32(sdr["MT_MRID"]),
                                MT_MRMID = Convert.IsDBNull(sdr["MT_MRMID"]) ? 0 : Convert.ToInt32(sdr["MT_MRMID"]),
                                MT_ItemCode = Convert.IsDBNull(sdr["MT_ItemCode"]) ? string.Empty : sdr["MT_ItemCode"].ToString(),
                                MT_RequiredQty = Convert.IsDBNull(sdr["MT_RequiredQty"]) ? 0 : Convert.ToDecimal(sdr["MT_RequiredQty"]),
                                MT_ReceivedQty = Convert.IsDBNull(sdr["MT_ReceivedQty"]) ? 0 : Convert.ToDecimal(sdr["MT_ReceivedQty"]),
                                MT_BalanceQty = Convert.IsDBNull(sdr["MT_BalanceQty"]) ? 0 : Convert.ToDecimal(sdr["MT_BalanceQty"]),
                                MT_PreviousReceivedQty = Convert.IsDBNull(sdr["MT_PreviousReceivedQty"]) ? 0 : Convert.ToDecimal(sdr["MT_PreviousReceivedQty"]),
                                MT_PreviousBalanceQty = Convert.IsDBNull(sdr["MT_PreviousBalanceQty"]) ? 0 : Convert.ToDecimal(sdr["MT_PreviousBalanceQty"]),
                                MT_CreatedDateTime = Convert.IsDBNull(sdr["MT_CreatedDateTime"]) ? string.Empty : sdr["MT_CreatedDateTime"].ToString(),
                                MT_UpdateDateTime = Convert.IsDBNull(sdr["MT_UpdateDateTime"]) ? string.Empty : sdr["MT_UpdateDateTime"].ToString(),
                                MT_MasterCompanyId = Convert.IsDBNull(sdr["MT_MasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["MT_MasterCompanyId"]),
                                MT_CompanyUnitId = Convert.IsDBNull(sdr["MT_CompanyUnitId"]) ? 0 : Convert.ToInt32(sdr["MT_CompanyUnitId"]),
                                MT_EmployeeId = Convert.IsDBNull(sdr["MT_EmployeeId"]) ? 0 : Convert.ToInt32(sdr["MT_EmployeeId"]),
                                MT_StatusId = Convert.IsDBNull(sdr["MT_StatusId"]) ? 0 : Convert.ToInt32(sdr["MT_StatusId"]),

                                MTS_ID = Convert.IsDBNull(sdr["MTS_ID"]) ? 0 : Convert.ToInt32(sdr["MTS_ID"]),
                                MTS_MTID = Convert.IsDBNull(sdr["MTS_MTID"]) ? 0 : Convert.ToInt32(sdr["MTS_MTID"]),
                                MTS_ItemStock = Convert.IsDBNull(sdr["MTS_ItemStock"]) ? 0 : Convert.ToDecimal(sdr["MTS_ItemStock"]),
                                MTS_BatchNo = Convert.IsDBNull(sdr["MTS_BatchNo"]) ? string.Empty : Convert.ToString(sdr["MTS_BatchNo"]),
                                MTS_ExpiryDate = Convert.IsDBNull(sdr["MTS_ExpiryDate"]) ? string.Empty : Convert.ToString(sdr["MTS_ExpiryDate"]),
                                MTS_CreatedDateTime = Convert.IsDBNull(sdr["MTS_CreatedDateTime"]) ? string.Empty : Convert.ToString(sdr["MTS_CreatedDateTime"]),
                                MTS_UpdatedDateTime = Convert.IsDBNull(sdr["MTS_UpdatedDateTime"]) ? string.Empty : Convert.ToString(sdr["MTS_UpdatedDateTime"]),

                                FirstName = sdr["EmployeeName"].ToString(),
                                StatusTitle = sdr["POStatus"].ToString(),

                                ApprovalFirstName = sdr["ApprovalHOD"].ToString(),

                                ApprovalLastName = sdr["RelesedHOD"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }



        public List<MaterialRequestData> MaterialRequestThrowItemCode()
        {
            List<MaterialRequestData> items = new List<MaterialRequestData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM MaterialRequestMaster mrm
								 INNER JOIN MaterialRequestDetails mrd ON mrd.MR_MRMID = mrm.MRM_ID
                                 INNER JOIN POStatus ps ON ps.POStatusID = mrm.MRM_StatusId
                                 INNER JOIN Users u ON u.UserID = mrm.MRM_EmployeeID
                                 INNER JOIN Employees e ON e.ID = u.employeeID
                                 Where MRM_MasterCompanyId = @MRM_MasterCompanyId
                                 ORDER BY CONVERT(datetime, mrm.MRM_CreatedDateTime, 120) DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@MRM_IsActive", 1);
                    cmd.Parameters.AddWithValue("@MRM_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new MaterialRequestData
                            {
                                MRM_ID = Convert.IsDBNull(sdr["MRM_ID"]) ? 0 : Convert.ToInt32(sdr["MRM_ID"]),
                                MRM_MasterCompanyId = Convert.IsDBNull(sdr["MRM_MasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["MRM_MasterCompanyId"]),
                                MRM_EmployeeID = Convert.IsDBNull(sdr["MRM_EmployeeID"]) ? 0 : Convert.ToInt32(sdr["MRM_EmployeeID"]),
                                MRM_CompanyUnitId = Convert.IsDBNull(sdr["MRM_CompanyUnitId"]) ? 0 : Convert.ToInt32(sdr["MRM_CompanyUnitId"]),
                                MRM_StatusId = Convert.IsDBNull(sdr["MRM_StatusId"]) ? 0 : Convert.ToInt32(sdr["MRM_StatusId"]),
                                MRM_IsActive = Convert.IsDBNull(sdr["MRM_IsActive"]) ? 0 : Convert.ToInt32(sdr["MRM_IsActive"]),
                                MRM_CounterYear = Convert.IsDBNull(sdr["MRM_CounterYear"]) ? 0 : Convert.ToInt32(sdr["MRM_CounterYear"]),
                                MRM_CounterNumber = Convert.IsDBNull(sdr["MRM_CounterNumber"]) ? 0 : Convert.ToInt32(sdr["MRM_CounterNumber"]),
                                MRM_NUMBER = Convert.IsDBNull(sdr["MRM_NUMBER"]) ? string.Empty : sdr["MRM_NUMBER"].ToString(),
                                MRM_ApprovedEmployeeID = Convert.IsDBNull(sdr["MRM_ApprovedEmployeeID"]) ? 0 : Convert.ToInt32(sdr["MRM_ApprovedEmployeeID"]),
                                MRM_ApprovedDateTime = Convert.IsDBNull(sdr["MRM_ApprovedDateTime"]) ? string.Empty : sdr["MRM_ApprovedDateTime"].ToString(),
                                MRM_CreatedDateTime = Convert.IsDBNull(sdr["MRM_CreatedDateTime"]) ? string.Empty : sdr["MRM_CreatedDateTime"].ToString(),
                                MRM_UpdatedDateTime = Convert.IsDBNull(sdr["MRM_UpdatedDateTime"]) ? string.Empty : sdr["MRM_UpdatedDateTime"].ToString(),
                                MRM_HODEmployeeID = Convert.IsDBNull(sdr["MRM_HODEmployeeID"]) ? 0 : Convert.ToInt32(sdr["MRM_HODEmployeeID"]),
                                MRM_ProjectCode = Convert.IsDBNull(sdr["MRM_ProjectCode"]) ? string.Empty : sdr["MRM_ProjectCode"].ToString(),

                                MR_ID = Convert.IsDBNull(sdr["MR_ID"]) ? 0 : Convert.ToInt32(sdr["MR_ID"]),
                                MR_StatusId = Convert.IsDBNull(sdr["MR_StatusId"]) ? 0 : Convert.ToInt32(sdr["MR_StatusId"]),
                                MR_MRMID = Convert.IsDBNull(sdr["MR_MRMID"]) ? 0 : Convert.ToInt32(sdr["MR_MRMID"]),
                                MR_NUMBER = Convert.IsDBNull(sdr["MR_NUMBER"]) ? string.Empty : sdr["MR_NUMBER"].ToString(),
                                MR_ItemCode = Convert.IsDBNull(sdr["MR_ItemCode"]) ? string.Empty : sdr["MR_ItemCode"].ToString(),
                                MR_ItemName = Convert.IsDBNull(sdr["MR_ItemName"]) ? string.Empty : sdr["MR_ItemName"].ToString(),
                                MR_UOM = Convert.IsDBNull(sdr["MR_UOM"]) ? string.Empty : sdr["MR_UOM"].ToString(),
                                MR_CurrentStock = Convert.IsDBNull(sdr["MR_CurrentStock"]) ? 0 : Convert.ToDecimal(sdr["MR_CurrentStock"]),
                                MR_MinStock = Convert.IsDBNull(sdr["MR_MinStock"]) ? 0 : Convert.ToDecimal(sdr["MR_MinStock"]),
                                MR_EmployeeRequireItemQty = Convert.IsDBNull(sdr["MR_EmployeeRequireItemQty"]) ? 0 : Convert.ToDecimal(sdr["MR_EmployeeRequireItemQty"]),
                                MR_ReceiveQty = Convert.IsDBNull(sdr["MR_ReceiveQty"]) ? 0 : Convert.ToDecimal(sdr["MR_ReceiveQty"]),
                                MR_BalanceQty = Convert.IsDBNull(sdr["MR_BalanceQty"]) ? 0 : Convert.ToDecimal(sdr["MR_BalanceQty"]),
                                MR_RequireDate = Convert.IsDBNull(sdr["MR_RequireDate"]) ? string.Empty : sdr["MR_RequireDate"].ToString(),


                                ApprovalFirstName = sdr["EmployeeName"].ToString(),
                                //ApprovalLastName = sdr["LastName"].ToString(),

                                StatusTitle = sdr["POStatus"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<MaterialRequestData> MaterialRequestShowHOD()
        {
            List<MaterialRequestData> items = new List<MaterialRequestData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM MaterialRequestMaster mrm
                                    INNER JOIN POStatus ps ON ps.POStatusID = mrm.MRM_StatusId
                                    INNER JOIN Users u ON u.UserID = mrm.MRM_EmployeeID
                                    INNER JOIN Employees e ON e.ID = u.employeeID
                                    Where MRM_MasterCompanyId = @MRM_MasterCompanyId
                                    ORDER BY CONVERT(datetime, mrm.MRM_CreatedDateTime, 120) DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@MRM_IsActive", 1);
                    cmd.Parameters.AddWithValue("@MRM_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new MaterialRequestData
                            {
                                MRM_ID = Convert.IsDBNull(sdr["MRM_ID"]) ? 0 : Convert.ToInt32(sdr["MRM_ID"]),
                                MRM_MasterCompanyId = Convert.IsDBNull(sdr["MRM_MasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["MRM_MasterCompanyId"]),
                                MRM_EmployeeID = Convert.IsDBNull(sdr["MRM_EmployeeID"]) ? 0 : Convert.ToInt32(sdr["MRM_EmployeeID"]),
                                MRM_CompanyUnitId = Convert.IsDBNull(sdr["MRM_CompanyUnitId"]) ? 0 : Convert.ToInt32(sdr["MRM_CompanyUnitId"]),
                                MRM_StatusId = Convert.IsDBNull(sdr["MRM_StatusId"]) ? 0 : Convert.ToInt32(sdr["MRM_StatusId"]),
                                MRM_IsActive = Convert.IsDBNull(sdr["MRM_IsActive"]) ? 0 : Convert.ToInt32(sdr["MRM_IsActive"]),
                                MRM_CounterYear = Convert.IsDBNull(sdr["MRM_CounterYear"]) ? 0 : Convert.ToInt32(sdr["MRM_CounterYear"]),
                                MRM_CounterNumber = Convert.IsDBNull(sdr["MRM_CounterNumber"]) ? 0 : Convert.ToInt32(sdr["MRM_CounterNumber"]),
                                MRM_NUMBER = Convert.IsDBNull(sdr["MRM_NUMBER"]) ? string.Empty : sdr["MRM_NUMBER"].ToString(),
                                MRM_ApprovedEmployeeID = Convert.IsDBNull(sdr["MRM_ApprovedEmployeeID"]) ? 0 : Convert.ToInt32(sdr["MRM_ApprovedEmployeeID"]),
                                MRM_ApprovedDateTime = Convert.IsDBNull(sdr["MRM_ApprovedDateTime"]) ? string.Empty : sdr["MRM_ApprovedDateTime"].ToString(),
                                MRM_CreatedDateTime = Convert.IsDBNull(sdr["MRM_CreatedDateTime"]) ? string.Empty : sdr["MRM_CreatedDateTime"].ToString(),
                                MRM_UpdatedDateTime = Convert.IsDBNull(sdr["MRM_UpdatedDateTime"]) ? string.Empty : sdr["MRM_UpdatedDateTime"].ToString(),
                                MRM_HODEmployeeID = Convert.IsDBNull(sdr["MRM_HODEmployeeID"]) ? 0 : Convert.ToInt32(sdr["MRM_HODEmployeeID"]),
                                MRM_ProjectCode = Convert.IsDBNull(sdr["MRM_ProjectCode"]) ? string.Empty : sdr["MRM_ProjectCode"].ToString(),


                                ApprovalFirstName = sdr["EmployeeName"].ToString(),

                                StatusTitle = sdr["POStatus"].ToString(),

                                FirstName = Convert.IsDBNull(sdr["EmployeeName"]) ? string.Empty : sdr["EmployeeName"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<MaterialRequestData> MaterialRequestShowHODPaged(int pageIndex, int pageSize, string searchTerm,
    DateTime? startDate, DateTime? endDate, out int totalRecords, int employeeId = 0, string roleId = "")
        {
            List<MaterialRequestData> items = new List<MaterialRequestData>();
            totalRecords = 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string baseWhere = @"WHERE MRM_MasterCompanyId = @MRM_MasterCompanyId 
                                    AND MRM_IsActive = 1
                                    AND (@SearchTerm = '' OR 
                                        mrm.MRM_NUMBER LIKE '%' + @SearchTerm + '%')";

                // Add role-based filtering
                if (roleId == "11") // Not admin or privileged role
                {
                    if (startDate.HasValue)
                    {
                        baseWhere += " AND CONVERT(date, mrm.MRM_CreatedDateTime) >= @StartDate";
                    }
                    if (endDate.HasValue)
                    {
                        baseWhere += " AND CONVERT(date, mrm.MRM_CreatedDateTime) <= @EndDate";
                    }
                }
                else
                {
                    baseWhere += " AND mrm.MRM_EmployeeID = @EmployeeId";
                }

                // Add date filters if provided
                //string countQuery = $@"SELECT COUNT(*) FROM MaterialRequestMaster mrm
                //                       INNER JOIN MasterEmployeeDetails medr ON medr.EmployeeId = mrm.MRM_EmployeeID
                //                       INNER JOIN StatusMasterTB sm ON sm.StatusID = mrm.MRM_StatusId
                //                       INNER JOIN CompanyUnitMaster cum ON cum.CompanyUnitId = mrm.MRM_CompanyUnitId
                //                       {baseWhere}";

                string countQuery = $@"SELECT COUNT(*) FROM MaterialRequestMaster mrm
                                        INNER JOIN POStatus ps ON ps.POStatusID = mrm.MRM_StatusId
                                        INNER JOIN Users u ON u.UserID = mrm.MRM_EmployeeID
                                        INNER JOIN Employees e ON e.ID = u.employeeID
                                        {baseWhere}";

                //string dataQuery = $@"SELECT 
                //                            mrm.*,
                //                            medr.FirstName, medr.LastName,
                //                            sm.StatusTitle,
                //                            cum.UnitName AS CompanyUnitName
                //                        FROM MaterialRequestMaster mrm
                //                        INNER JOIN MasterEmployeeDetails medr ON medr.EmployeeId = mrm.MRM_EmployeeID
                //                        INNER JOIN StatusMasterTB sm ON sm.StatusID = mrm.MRM_StatusId
                //                        INNER JOIN CompanyUnitMaster cum ON cum.CompanyUnitId = mrm.MRM_CompanyUnitId
                //                        {baseWhere}
                //                        ORDER BY mrm.MRM_CreatedDateTime DESC
                //                        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                string dataQuery = $@"SELECT * FROM MaterialRequestMaster mrm
                                      INNER JOIN POStatus ps ON ps.POStatusID = mrm.MRM_StatusId
                                      INNER JOIN Users u ON u.UserID = mrm.MRM_EmployeeID
                                      INNER JOIN Employees e ON e.ID = u.employeeID
                                      {baseWhere}
                                      ORDER BY mrm.MRM_CreatedDateTime DESC
                                      OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                using (SqlCommand countCmd = new SqlCommand(countQuery, con))
                using (SqlCommand dataCmd = new SqlCommand(dataQuery, con))
                {
                    // Common parameters
                    foreach (var cmd in new[] { countCmd, dataCmd })
                    {
                        cmd.Parameters.AddWithValue("@MRM_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                        cmd.Parameters.AddWithValue("@SearchTerm", searchTerm ?? "");

                        if (roleId == "11") // Not admin or privileged role
                        {
                            if (startDate.HasValue)
                                cmd.Parameters.AddWithValue("@StartDate", startDate.Value);
                            if (endDate.HasValue)
                                cmd.Parameters.AddWithValue("@EndDate", endDate.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                        }

                    }
                    // Paging parameters
                    int offset = (pageIndex - 1) * pageSize;
                    dataCmd.Parameters.AddWithValue("@Offset", offset);
                    dataCmd.Parameters.AddWithValue("@PageSize", pageSize);

                    con.Open();

                    // Get total records first
                    totalRecords = (int)countCmd.ExecuteScalar();

                    // Then get paged data
                    using (SqlDataReader sdr = dataCmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new MaterialRequestData
                            {
                                MRM_ID = Convert.ToInt32(sdr["MRM_ID"]),
                                MRM_NUMBER = sdr["MRM_NUMBER"].ToString(),
                                //MRM_ProjectCode = sdr["MRM_ProjectCode"].ToString(),
                                //CompanyUnitName = sdr["CompanyUnitName"].ToString(),
                                StatusTitle = sdr["POStatus"].ToString(),
                                FirstName = sdr["EmployeeName"].ToString(),
                                //LastName = sdr["LastName"].ToString(),
                                MRM_CreatedDateTime = sdr["MRM_CreatedDateTime"].ToString(),
                                MRM_StatusId = Convert.ToInt32(sdr["MRM_StatusId"])
                            });
                        }
                    }
                }
            }
            return items;
        }

        public List<MaterialRequestData> MaterialReleasedShowEmployee()
        {
            List<MaterialRequestData> items = new List<MaterialRequestData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"select * from MaterialRequestMaster mrm
                                    INNER JOIN MaterialRequestDetails mrd ON mrd.MR_MRMID = mrm.MRM_ID
                                    INNER JOIN MasterEmployeeDetails medr ON medr.EmployeeId = mrm.MRM_EmployeeID
                                    INNER JOIN CompanyUnitMaster cum ON cum.CompanyUnitId = mrm.MRM_CompanyUnitId
                                    INNER JOIN StatusMasterTB sm ON sm.StatusID = mrd.MR_StatusId
                                    WHERE MRM_MasterCompanyId = @MRM_MasterCompanyId
                                    ORDER BY MRM_ID DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@MRM_IsActive", 1);
                    cmd.Parameters.AddWithValue("@MRM_EmployeeID", HttpContext.Current.Session["UserID"].ToString());
                    cmd.Parameters.AddWithValue("@MRM_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new MaterialRequestData
                            {
                                MRM_ID = Convert.IsDBNull(sdr["MRM_ID"]) ? 0 : Convert.ToInt32(sdr["MRM_ID"]),
                                MRM_MasterCompanyId = Convert.IsDBNull(sdr["MRM_MasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["MRM_MasterCompanyId"]),
                                MRM_EmployeeID = Convert.IsDBNull(sdr["MRM_EmployeeID"]) ? 0 : Convert.ToInt32(sdr["MRM_EmployeeID"]),
                                MRM_CompanyUnitId = Convert.IsDBNull(sdr["MRM_CompanyUnitId"]) ? 0 : Convert.ToInt32(sdr["MRM_CompanyUnitId"]),
                                MRM_StatusId = Convert.IsDBNull(sdr["MRM_StatusId"]) ? 0 : Convert.ToInt32(sdr["MRM_StatusId"]),
                                MRM_IsActive = Convert.IsDBNull(sdr["MRM_IsActive"]) ? 0 : Convert.ToInt32(sdr["MRM_IsActive"]),
                                //MRM_CounterYear = Convert.IsDBNull(sdr["MRM_CounterYear"]) ? 0 : Convert.ToInt32(sdr["MRM_CounterYear"]),
                                //MRM_CounterNumber = Convert.IsDBNull(sdr["MRM_CounterNumber"]) ? 0 : Convert.ToInt32(sdr["MRM_CounterNumber"]),
                                MRM_NUMBER = Convert.IsDBNull(sdr["MRM_NUMBER"]) ? string.Empty : sdr["MRM_NUMBER"].ToString(),
                                //MRM_ApprovedEmployeeID = Convert.IsDBNull(sdr["MRM_ApprovedEmployeeID"]) ? 0 : Convert.ToInt32(sdr["MRM_ApprovedEmployeeID"]),
                                //MRM_ApprovedDateTime = Convert.IsDBNull(sdr["MRM_ApprovedDateTime"]) ? string.Empty : sdr["MRM_ApprovedDateTime"].ToString(),
                                MRM_CreatedDateTime = Convert.IsDBNull(sdr["MRM_CreatedDateTime"]) ? string.Empty : sdr["MRM_CreatedDateTime"].ToString(),
                                //MRM_UpdatedDateTime = Convert.IsDBNull(sdr["MRM_UpdatedDateTime"]) ? string.Empty : sdr["MRM_UpdatedDateTime"].ToString(),
                                //MRM_HODEmployeeID = Convert.IsDBNull(sdr["MRM_HODEmployeeID"]) ? 0 : Convert.ToInt32(sdr["MRM_HODEmployeeID"]),
                                MRM_ProjectCode = Convert.IsDBNull(sdr["MRM_ProjectCode"]) ? string.Empty : sdr["MRM_ProjectCode"].ToString(),
                                //MT_ID = Convert.IsDBNull(sdr["MT_ID"]) ? 0 : Convert.ToInt32(sdr["MT_ID"]),
                                //MT_MRID = Convert.IsDBNull(sdr["MT_MRID"]) ? 0 : Convert.ToInt32(sdr["MT_MRID"]),
                                //MT_MRMID = Convert.IsDBNull(sdr["MT_MRMID"]) ? 0 : Convert.ToInt32(sdr["MT_MRMID"]),
                                MT_ItemCode = Convert.IsDBNull(sdr["MR_ItemCode"]) ? string.Empty : sdr["MR_ItemCode"].ToString(),
                                MT_RequiredQty = Convert.IsDBNull(sdr["MR_EmployeeRequireItemQty"]) ? 0 : Convert.ToDecimal(sdr["MR_EmployeeRequireItemQty"]),
                                MT_ReceivedQty = Convert.IsDBNull(sdr["MR_ReceiveQty"]) ? 0 : Convert.ToDecimal(sdr["MR_ReceiveQty"]),
                                MT_BalanceQty = Convert.IsDBNull(sdr["MR_BalanceQty"]) ? 0 : Convert.ToDecimal(sdr["MR_BalanceQty"]),
                                //MT_PreviousReceivedQty = Convert.IsDBNull(sdr["MT_PreviousReceivedQty"]) ? 0 : Convert.ToDecimal(sdr["MT_PreviousReceivedQty"]),
                                //MT_PreviousBalanceQty = Convert.IsDBNull(sdr["MT_PreviousBalanceQty"]) ? 0 : Convert.ToDecimal(sdr["MT_PreviousBalanceQty"]),
                                //MT_CreatedDateTime = Convert.IsDBNull(sdr["MT_CreatedDateTime"]) ? string.Empty : sdr["MT_CreatedDateTime"].ToString(),
                                //MT_UpdateDateTime = Convert.IsDBNull(sdr["MT_UpdateDateTime"]) ? string.Empty : sdr["MT_UpdateDateTime"].ToString(),
                                //MT_MasterCompanyId = Convert.IsDBNull(sdr["MT_MasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["MT_MasterCompanyId"]),
                                //MT_CompanyUnitId = Convert.IsDBNull(sdr["MT_CompanyUnitId"]) ? 0 : Convert.ToInt32(sdr["MT_CompanyUnitId"]),
                                //MT_EmployeeId = Convert.IsDBNull(sdr["MT_EmployeeId"]) ? 0 : Convert.ToInt32(sdr["MT_EmployeeId"]),
                                MT_StatusId = Convert.IsDBNull(sdr["MR_StatusId"]) ? 0 : Convert.ToInt32(sdr["MR_StatusId"]),

                                CompanyUnitName = sdr["UnitName"].ToString(),
                                //ApprovalFirstName = sdr["FirstName"].ToString(),
                                //ApprovalLastName = sdr["LastName"].ToString(),

                                //StatusTitle = sdr["StatusTitle"].ToString(),

                                FirstName = Convert.IsDBNull(sdr["FirstName"]) ? string.Empty : sdr["FirstName"].ToString(),
                                LastName = Convert.IsDBNull(sdr["LastName"]) ? string.Empty : sdr["LastName"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<MaterialRequestData> MaterialReleasedShowEmployeePaged(int pageIndex, int pageSize, string searchTerm, DateTime? startDate, DateTime? endDate, out int totalRecords, int employeeId = 0, string roleId = "")
        {
            List<MaterialRequestData> items = new List<MaterialRequestData>();
            totalRecords = 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string baseWhere = @"WHERE MRM_MasterCompanyId = @MRM_MasterCompanyId 
                                     AND (@SearchTerm = '' OR 
                                     mrm.MRM_NUMBER LIKE '%' + @SearchTerm + '%' OR 
                                     mrd.MR_ItemCode LIKE '%' + @SearchTerm + '%')";

                // Add role-based filtering
                //if (roleId != "1" && roleId != "7") // Not admin or privileged role
                //{
                baseWhere += " AND mrm.MRM_EmployeeID = @EmployeeId AND mrd.MR_StatusId != 22";
                //}

                //// Add date filters if provided
                //if (startDate.HasValue)
                //{
                //    baseWhere += " AND CONVERT(date, mrm.MRM_CreatedDateTime) >= @StartDate";
                //}
                //if (endDate.HasValue)
                //{
                //    baseWhere += " AND CONVERT(date, mrm.MRM_CreatedDateTime) <= @EndDate";
                //}

                string countQuery = $@"SELECT COUNT(*)FROM MaterialRequestMaster mrm
                                        INNER JOIN MaterialRequestDetails mrd ON mrd.MR_MRMID = mrm.MRM_ID
                                        INNER JOIN POStatus ps ON ps.POStatusID = mrm.MRM_StatusId
                                        INNER JOIN Users u ON u.UserID = mrm.MRM_EmployeeID
                                        INNER JOIN Employees e ON e.ID = u.employeeID
                                        {baseWhere}";

                string dataQuery = $@"SELECT * FROM MaterialRequestMaster mrm
                                        INNER JOIN MaterialRequestDetails mrd ON mrd.MR_MRMID = mrm.MRM_ID
                                        INNER JOIN POStatus ps ON ps.POStatusID = mrm.MRM_StatusId
                                        INNER JOIN Users u ON u.UserID = mrm.MRM_EmployeeID
                                        INNER JOIN Employees e ON e.ID = u.employeeID
                                        {baseWhere}
                                        ORDER BY mrm.MRM_ID DESC
                                        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                using (SqlCommand countCmd = new SqlCommand(countQuery, con))
                using (SqlCommand dataCmd = new SqlCommand(dataQuery, con))
                {
                    // Common parameters
                    foreach (var cmd in new[] { countCmd, dataCmd })
                    {
                        cmd.Parameters.AddWithValue("@MRM_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                        cmd.Parameters.AddWithValue("@SearchTerm", searchTerm ?? "");
                        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                    }

                    // Paging parameters
                    int offset = Math.Max((pageIndex - 1) * pageSize, 0);
                    dataCmd.Parameters.AddWithValue("@Offset", offset);
                    dataCmd.Parameters.AddWithValue("@PageSize", pageSize);

                    con.Open();
                    totalRecords = (int)countCmd.ExecuteScalar();

                    using (SqlDataReader sdr = dataCmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new MaterialRequestData
                            {
                                MRM_ID = Convert.IsDBNull(sdr["MRM_ID"]) ? 0 : Convert.ToInt32(sdr["MRM_ID"]),
                                MRM_NUMBER = sdr["MRM_NUMBER"].ToString(),
                                MT_ItemCode = sdr["MR_ItemCode"].ToString(),
                                MT_RequiredQty = Convert.IsDBNull(sdr["MR_EmployeeRequireItemQty"]) ? 0 : Convert.ToDecimal(sdr["MR_EmployeeRequireItemQty"]),
                                MT_ReceivedQty = Convert.IsDBNull(sdr["MR_ReceiveQty"]) ? 0 : Convert.ToDecimal(sdr["MR_ReceiveQty"]),
                                MT_BalanceQty = Convert.IsDBNull(sdr["MR_BalanceQty"]) ? 0 : Convert.ToDecimal(sdr["MR_BalanceQty"]),
                                FirstName = sdr["EmployeeName"].ToString(),
                                //LastName = sdr["LastName"].ToString(),
                                MRM_CreatedDateTime = Convert.IsDBNull(sdr["MRM_CreatedDateTime"]) ? string.Empty : sdr["MRM_CreatedDateTime"].ToString(),
                                MT_StatusId = Convert.IsDBNull(sdr["MR_StatusId"]) ? 0 : Convert.ToInt32(sdr["MR_StatusId"])
                            });
                        }
                    }
                }
            }
            return items;
        }

        public List<MaterialRequestData> MaterialReleasedShowEmployeePagedforHOD(int pageIndex, int pageSize, string searchTerm, DateTime? startDate, DateTime? endDate, out int totalRecords, string roleId = "")
        {
            List<MaterialRequestData> items = new List<MaterialRequestData>();
            totalRecords = 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string baseWhere = @"WHERE MRM_MasterCompanyId = @MRM_MasterCompanyId AND 
                                     (@SearchTerm = '' OR mrm.MRM_NUMBER LIKE '%' + @SearchTerm + '%' OR mrd.MR_ItemCode LIKE '%' + @SearchTerm + '%' )";

                if (roleId == "11")
                {
                    if (startDate.HasValue)
                    {
                        baseWhere += " AND CONVERT(date, mrm.MRM_CreatedDateTime) >= @StartDate";
                    }
                    if (endDate.HasValue)
                    {
                        baseWhere += " AND CONVERT(date, mrm.MRM_CreatedDateTime) <= @EndDate";
                    }
                }

                string countQuery = $@"SELECT COUNT(*) FROM MaterialRequestMaster mrm
                                        INNER JOIN MaterialRequestDetails mrd ON mrd.MR_MRMID = mrm.MRM_ID
                                        INNER JOIN POStatus ps ON ps.POStatusID = mrm.MRM_StatusId
										INNER JOIN Users u ON u.UserID = mrm.MRM_EmployeeID
										INNER JOIN Employees e ON e.ID = u.employeeID
                                         {baseWhere}";

                string dataQuery = $@"SELECT * FROM MaterialRequestMaster mrm
                                        INNER JOIN MaterialRequestDetails mrd ON mrd.MR_MRMID = mrm.MRM_ID
                                        INNER JOIN POStatus ps ON ps.POStatusID = mrm.MRM_StatusId
										INNER JOIN Users u ON u.UserID = mrm.MRM_EmployeeID
										INNER JOIN Employees e ON e.ID = u.employeeID
                                        {baseWhere}
                                        ORDER BY mrm.MRM_ID DESC
                                        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                using (SqlCommand countCmd = new SqlCommand(countQuery, con))
                using (SqlCommand dataCmd = new SqlCommand(dataQuery, con))
                {
                    foreach (var cmd in new[] { countCmd, dataCmd })
                    {
                        cmd.Parameters.AddWithValue("@MRM_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                        cmd.Parameters.AddWithValue("@SearchTerm", searchTerm ?? "");

                        if (roleId == "11")
                        {
                            if (startDate.HasValue)
                                cmd.Parameters.AddWithValue("@StartDate", startDate.Value);
                            if (endDate.HasValue)
                                cmd.Parameters.AddWithValue("@EndDate", endDate.Value);
                        }
                    }

                    int offset = Math.Max((pageIndex - 1) * pageSize, 0);
                    dataCmd.Parameters.AddWithValue("@Offset", offset);
                    dataCmd.Parameters.AddWithValue("@PageSize", pageSize);

                    con.Open();
                    totalRecords = (int)countCmd.ExecuteScalar();

                    using (SqlDataReader sdr = dataCmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new MaterialRequestData
                            {
                                MRM_ID = Convert.IsDBNull(sdr["MRM_ID"]) ? 0 : Convert.ToInt32(sdr["MRM_ID"]),
                                MRM_NUMBER = sdr["MRM_NUMBER"].ToString(),
                                MT_ItemCode = sdr["MR_ItemCode"].ToString(),
                                MT_RequiredQty = Convert.IsDBNull(sdr["MR_EmployeeRequireItemQty"]) ? 0 : Convert.ToDecimal(sdr["MR_EmployeeRequireItemQty"]),
                                MT_ReceivedQty = Convert.IsDBNull(sdr["MR_ReceiveQty"]) ? 0 : Convert.ToDecimal(sdr["MR_ReceiveQty"]),
                                MT_BalanceQty = Convert.IsDBNull(sdr["MR_BalanceQty"]) ? 0 : Convert.ToDecimal(sdr["MR_BalanceQty"]),
                                //MRM_ProjectCode = sdr["MRM_ProjectCode"].ToString(),
                                FirstName = sdr["EmployeeName"].ToString(),
                                //LastName = sdr["LastName"].ToString(),
                                MRM_CreatedDateTime = Convert.IsDBNull(sdr["MRM_CreatedDateTime"]) ? string.Empty : sdr["MRM_CreatedDateTime"].ToString(),
                                MT_StatusId = Convert.IsDBNull(sdr["MR_StatusId"]) ? 0 : Convert.ToInt32(sdr["MR_StatusId"])
                            });
                        }
                    }
                }
            }
            return items;
        }

        public List<ItemTransactionData> ItemTransactionMasterDataGetforMR(string itemCodeGet)
        {
            List<ItemTransactionData> items = new List<ItemTransactionData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"WITH OrderedItems AS (
                                    SELECT 
                                        itm.IT_CompanyUnitId,
                                        um.UOMName,
                                        itm.IT_MasterCompanyId,
                                        itm.IT_Status,
                                        im.ItemCode,
                                        im.ItemName,
                                        ivm.InventoryCurrentStock,
                                        ivm.InventoryMinStock,
                                        rtd.RackType,
                                        rsd.RackSerialNumber,
                                        SUM(its.IS_StoreQty) AS TotalItemInRack,
                                        itm.IT_BatchNumber,
                                        CONVERT(VARCHAR(10), itm.IT_ExpiryDateTime, 120) AS IT_ExpiryDateTime,
                                        ROW_NUMBER() OVER (ORDER BY itm.IT_ExpiryDateTime) AS ExpiryRank
                                    FROM 
                                        ItemStore its
                                        INNER JOIN ItemTransactionMaster itm ON itm.IT_Id = its.IS_ITId
                                        INNER JOIN InventoryMaster ivm ON ivm.InventoryId = itm.IT_InventoryId
                                        INNER JOIN ItemMaster im ON im.ItemId = itm.IT_ItemId
                                        INNER JOIN UOM um ON um.UOMId = itm.IT_UOMId
                                        INNER JOIN RackTypeDetails rtd ON rtd.RackTypeId = itm.IT_RackTypeId
                                        INNER JOIN RackSerialDetails rsd ON rsd.RackSerialId = its.IS_RackSerialNo
                                    WHERE 
                                        ITM.IT_Id = @IT_Id
                                    GROUP BY
                                        itm.IT_CompanyUnitId,
                                        um.UOMName,
                                        itm.IT_MasterCompanyId,
                                        itm.IT_Status,
                                        im.ItemCode,
                                        im.ItemName,
                                        ivm.InventoryCurrentStock,
                                        ivm.InventoryMinStock,
                                        rtd.RackType,
                                        rsd.RackSerialNumber,
                                        itm.IT_BatchNumber,
                                        itm.IT_ExpiryDateTime
                                )
                                , CumulativeItems AS (
                                    SELECT
                                        *,
                                        SUM(TotalItemInRack) OVER (ORDER BY ExpiryRank) AS CumulativeTotalItemInRack
                                    FROM
                                        OrderedItems
                                )
                                SELECT
                                    IT_CompanyUnitId,
                                    UOMName,
                                    IT_MasterCompanyId,
                                    IT_Status,
                                    ItemCode,
                                    ItemName,
                                    InventoryCurrentStock,
                                    InventoryMinStock,
                                    RackType,
                                    RackSerialNumber,
                                    TotalItemInRack,
                                    IT_BatchNumber,
                                    IT_ExpiryDateTime,
                                    ExpiryRank,
                                    CumulativeTotalItemInRack
                                FROM
                                    CumulativeItems
                                ORDER BY
                                    IT_ExpiryDateTime

";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IT_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    cmd.Parameters.AddWithValue("@IT_CompanyUnitId", HttpContext.Current.Session["CompanyUnitId"].ToString());
                    cmd.Parameters.AddWithValue("@IT_CreatedEmployeeId", HttpContext.Current.Session["UserID"].ToString());
                    cmd.Parameters.AddWithValue("@ItemCode", itemCodeGet);
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new ItemTransactionData
                            {
                                IT_CompanyUnitId = Convert.ToInt32(sdr["IT_CompanyUnitId"]),
                                IT_MasterCompanyId = Convert.ToInt32(sdr["IT_MasterCompanyId"]),
                                IT_Status = Convert.ToInt32(sdr["IT_Status"]),
                                IT_BatchNumber = sdr["IT_BatchNumber"].ToString(),
                                IT_ExpiryDateTime = sdr["IT_ExpiryDateTime"] != DBNull.Value
                                                    ? DateTime.TryParse(sdr["IT_ExpiryDateTime"].ToString(), out DateTime expiryDateTime)
                                                        ? expiryDateTime.ToString("dd/MM/yyyy")
                                                        : string.Empty
                                                    : string.Empty,

                                TotalItemInRack = Convert.IsDBNull(sdr["TotalItemInRack"]) ? 0 : Convert.ToDecimal(sdr["TotalItemInRack"]),
                                InventoryCurrentStock = Convert.ToDecimal(sdr["InventoryCurrentStock"]),
                                InventoryMinStock = Convert.ToDecimal(sdr["InventoryMinStock"]),
                                ItemCode = sdr["ItemCode"].ToString(),
                                ItemName = sdr["ItemName"].ToString(),
                                UOMName = sdr["UOMName"].ToString(),
                                RackType = sdr["RackType"].ToString(),
                                RackSerialNumber = sdr["RackSerialNumber"].ToString(),
                                ExpiryRank = Convert.ToInt32(sdr["ExpiryRank"].ToString()),
                                CumulativeTotalItemInRack = Convert.ToDecimal(sdr["CumulativeTotalItemInRack"].ToString()),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<ItemTransactionData> ItemTransactionMasterDataGetforQRCode(string transactionID)
        {
            List<ItemTransactionData> items = new List<ItemTransactionData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"WITH OrderedItems AS (
                                    SELECT 
										itm.IT_Id,
                                        itm.IT_CompanyUnitId,
                                        um.UOMName,
                                        itm.IT_MasterCompanyId,
                                        itm.IT_Status,
                                        im.ItemCode,
                                        im.ItemName,
                                        ivm.InventoryCurrentStock,
                                        ivm.InventoryMinStock,
                                        rtd.RackType,
                                        rsd.RackSerialNumber,
                                        SUM(its.IS_StoreQty) AS TotalItemInRack,
                                        itm.IT_BatchNumber,
                                        CONVERT(VARCHAR(10), itm.IT_ExpiryDateTime, 120) AS IT_ExpiryDateTime,
                                        ROW_NUMBER() OVER (ORDER BY itm.IT_ExpiryDateTime) AS ExpiryRank
                                    FROM 
                                        ItemStore its
                                        INNER JOIN ItemTransactionMaster itm ON itm.IT_Id = its.IS_ITId
                                        INNER JOIN InventoryMaster ivm ON ivm.InventoryId = itm.IT_InventoryId
                                        INNER JOIN Items im ON im.ItemId = itm.IT_ItemId
                                        INNER JOIN UOM um ON um.UOMId = itm.IT_UOMId
                                        INNER JOIN RackTypeDetails rtd ON rtd.RackTypeId = itm.IT_RackTypeId
                                        INNER JOIN RackSerialDetails rsd ON rsd.RackSerialId = its.IS_RackSerialNo
                                    WHERE 
                                        itm.IT_Id = @IT_Id
                                    GROUP BY
										itm.IT_Id,
                                        itm.IT_CompanyUnitId,
                                        um.UOMName,
                                        itm.IT_MasterCompanyId,
                                        itm.IT_Status,
                                        im.ItemCode,
                                        im.ItemName,
                                        ivm.InventoryCurrentStock,
                                        ivm.InventoryMinStock,
                                        rtd.RackType,
                                        rsd.RackSerialNumber,
                                        itm.IT_BatchNumber,
                                        itm.IT_ExpiryDateTime
                                )
                                , CumulativeItems AS (
                                    SELECT
                                        *,
                                        SUM(TotalItemInRack) OVER (ORDER BY ExpiryRank) AS CumulativeTotalItemInRack
                                    FROM
                                        OrderedItems
                                )
                                SELECT
									IT_Id,
                                    IT_CompanyUnitId,
                                    UOMName,
                                    IT_MasterCompanyId,
                                    IT_Status,
                                    ItemCode,
                                    ItemName,
                                    InventoryCurrentStock,
                                    InventoryMinStock,
                                    RackType,
                                    RackSerialNumber,
                                    TotalItemInRack,
                                    IT_BatchNumber,
                                    IT_ExpiryDateTime,
                                    ExpiryRank,
                                    CumulativeTotalItemInRack
                                FROM
                                    CumulativeItems
                                WHERE
                                    TotalItemInRack > 0
                                ORDER BY
                                    IT_ExpiryDateTime;
                                ";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IT_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    cmd.Parameters.AddWithValue("@IT_CompanyUnitId", HttpContext.Current.Session["CompanyUnitId"].ToString());
                    cmd.Parameters.AddWithValue("@IT_CreatedEmployeeId", HttpContext.Current.Session["UserID"].ToString());
                    cmd.Parameters.AddWithValue("@IT_Id", transactionID);
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new ItemTransactionData
                            {
                                IT_Id = Convert.ToInt32(sdr["IT_Id"]),
                                IT_CompanyUnitId = Convert.ToInt32(sdr["IT_CompanyUnitId"]),
                                IT_MasterCompanyId = Convert.ToInt32(sdr["IT_MasterCompanyId"]),
                                IT_Status = Convert.ToInt32(sdr["IT_Status"]),
                                IT_BatchNumber = sdr["IT_BatchNumber"].ToString(),
                                IT_ExpiryDateTime = sdr["IT_ExpiryDateTime"] != DBNull.Value
                                                    ? DateTime.TryParse(sdr["IT_ExpiryDateTime"].ToString(), out DateTime expiryDateTime)
                                                        ? expiryDateTime.ToString("dd/MM/yyyy")
                                                        : string.Empty
                                                    : string.Empty,

                                TotalItemInRack = Convert.IsDBNull(sdr["TotalItemInRack"]) ? 0 : Convert.ToDecimal(sdr["TotalItemInRack"]),
                                InventoryCurrentStock = Convert.ToDecimal(sdr["InventoryCurrentStock"]),
                                InventoryMinStock = Convert.ToDecimal(sdr["InventoryMinStock"]),
                                ItemCode = sdr["ItemCode"].ToString(),
                                ItemName = sdr["ItemName"].ToString(),
                                UOMName = sdr["UOMName"].ToString(),
                                RackType = sdr["RackType"].ToString(),
                                RackSerialNumber = sdr["RackSerialNumber"].ToString(),
                                ExpiryRank = Convert.ToInt32(sdr["ExpiryRank"].ToString()),
                                CumulativeTotalItemInRack = Convert.ToDecimal(sdr["CumulativeTotalItemInRack"].ToString()),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<ItemTransactionData> ItemTransactionMasterDataGetforMRSendItem(string itemCodeGet, string desiredQuantity, string projectCode)
        {
            List<ItemTransactionData> items = new List<ItemTransactionData>();
            using (SqlConnection con = new SqlConnection(constr))
            {

                string query = @"WITH OrderedItems AS (
                                    SELECT 
                                        its.IS_ItemStoreId,
										itm.IT_Id,
                                        itm.IT_CompanyUnitId,
                                        um.UOMName,
                                        itm.IT_MasterCompanyId,
                                        itm.IT_Status,
                                        im.ItemCode,
                                        im.ItemName,itm.IT_ProjectID,
                                        ivm.InventoryCurrentStock,
                                        ivm.InventoryMinStock,
                                        rtd.RackType,
                                        rsd.RackSerialNumber,
                                        SUM(its.IS_StoreQty) AS TotalItemInRack,
                                        itm.IT_BatchNumber,
                                        CONVERT(VARCHAR(10), itm.IT_ExpiryDateTime, 120) AS IT_ExpiryDateTime,
                                        ROW_NUMBER() OVER (ORDER BY itm.IT_ExpiryDateTime) AS ExpiryRank
                                    FROM 
                                        ItemStore its
                                        INNER JOIN ItemTransactionMaster itm ON itm.IT_Id = its.IS_ITId
                                        INNER JOIN InventoryMaster ivm ON ivm.InventoryId = itm.IT_InventoryId
                                        INNER JOIN Items im ON im.ItemId = itm.IT_ItemId
                                        INNER JOIN UOM um ON um.UOMId = itm.IT_UOMId
                                        INNER JOIN RackTypeDetails rtd ON rtd.RackTypeId = itm.IT_RackTypeId
                                        INNER JOIN RackSerialDetails rsd ON rsd.RackSerialId = its.IS_RackSerialNo
                                    WHERE 
									    itm.IT_MasterCompanyId = @IT_MasterCompanyId
                                    AND im.ItemCode = @ItemCode

                                    GROUP BY
                                        its.IS_ItemStoreId,
										itm.IT_Id,
                                        itm.IT_CompanyUnitId,
                                        um.UOMName,
                                        itm.IT_MasterCompanyId,
                                        itm.IT_Status,
                                        im.ItemCode,
                                        im.ItemName,itm.IT_ProjectID,
                                        ivm.InventoryCurrentStock,
                                        ivm.InventoryMinStock,
                                        rtd.RackType,
                                        rsd.RackSerialNumber,
                                        itm.IT_BatchNumber,
                                        itm.IT_ExpiryDateTime
                                )
                                , CumulativeItems AS (
                                    SELECT
                                        *,
                                        SUM(TotalItemInRack) OVER (ORDER BY ExpiryRank) AS CumulativeTotalItemInRack
                                    FROM
                                        OrderedItems
                                )
                                , FirstSatisfiedRow AS (
                                    SELECT 
                                        ExpiryRank
                                    FROM
                                        CumulativeItems
                                    WHERE 
                                        CumulativeTotalItemInRack >= @DesiredQuantity
                                )
                                SELECT
                                    OrderedItems.IS_ItemStoreId,
									OrderedItems.IT_Id,
                                    OrderedItems.IT_CompanyUnitId,
                                    OrderedItems.UOMName,
                                    OrderedItems.IT_MasterCompanyId,
                                    OrderedItems.IT_Status,
                                    OrderedItems.ItemCode,
                                    OrderedItems.ItemName,
									OrderedItems.IT_ProjectID,
                                    OrderedItems.InventoryCurrentStock,
                                    OrderedItems.InventoryMinStock,
                                    OrderedItems.RackType,
                                    OrderedItems.RackSerialNumber,
                                    OrderedItems.TotalItemInRack,
                                    OrderedItems.IT_BatchNumber,
                                    OrderedItems.IT_ExpiryDateTime,
                                    CumulativeItems.ExpiryRank,
                                    CumulativeItems.CumulativeTotalItemInRack,
                                    CASE
                                        WHEN OrderedItems.ExpiryRank <= (SELECT MIN(ExpiryRank) FROM FirstSatisfiedRow) THEN 1
                                        ELSE 0
                                    END AS IsQtyGo
                                FROM
                                    OrderedItems
                                JOIN
                                    CumulativeItems ON OrderedItems.ExpiryRank = CumulativeItems.ExpiryRank
								WHERE
									OrderedItems.TotalItemInRack > 0
                                ORDER BY
                                    OrderedItems.IT_ExpiryDateTime";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IT_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    cmd.Parameters.AddWithValue("@IT_CompanyUnitId", HttpContext.Current.Session["CompanyUnitId"].ToString());
                    cmd.Parameters.AddWithValue("@IT_CreatedEmployeeId", HttpContext.Current.Session["UserID"].ToString());
                    cmd.Parameters.AddWithValue("@ItemCode", itemCodeGet);
                    cmd.Parameters.AddWithValue("@ProjectCode", projectCode);
                    cmd.Parameters.AddWithValue("@DesiredQuantity", Convert.ToDecimal(desiredQuantity));

                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new ItemTransactionData
                            {
                                IT_Id = Convert.ToInt32(sdr["IT_Id"]),
                                IS_ItemStoreId = Convert.ToInt32(sdr["IS_ItemStoreId"]),
                                IT_CompanyUnitId = Convert.ToInt32(sdr["IT_CompanyUnitId"]),
                                IT_MasterCompanyId = Convert.ToInt32(sdr["IT_MasterCompanyId"]),
                                IT_Status = Convert.ToInt32(sdr["IT_Status"]),
                                IT_BatchNumber = sdr["IT_BatchNumber"].ToString(),
                                IT_ExpiryDateTime = sdr["IT_ExpiryDateTime"] != DBNull.Value
                                                    ? DateTime.TryParse(sdr["IT_ExpiryDateTime"].ToString(), out DateTime expiryDateTime)
                                                    ? expiryDateTime.ToString("dd/MM/yyyy") : string.Empty : string.Empty,
                                TotalItemInRack = Convert.IsDBNull(sdr["TotalItemInRack"]) ? 0 : Convert.ToDecimal(sdr["TotalItemInRack"]),
                                InventoryCurrentStock = Convert.ToDecimal(sdr["InventoryCurrentStock"]),
                                InventoryMinStock = Convert.ToDecimal(sdr["InventoryMinStock"]),
                                ItemCode = sdr["ItemCode"].ToString(),
                                ItemName = sdr["ItemName"].ToString(),
                                UOMName = sdr["UOMName"].ToString(),
                                RackType = sdr["RackType"].ToString(),
                                RackSerialNumber = sdr["RackSerialNumber"].ToString(),

                                ExpiryRank = Convert.ToInt32(sdr["ExpiryRank"].ToString()),
                                CumulativeTotalItemInRack = Convert.ToDecimal(sdr["CumulativeTotalItemInRack"].ToString()),
                                IsQtyGo = Convert.ToInt32(sdr["IsQtyGo"].ToString()),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<ItemTransactionData> ItemTransactionMasterDataGetforReleased(string itemCodeGet, string desiredQuantity, string itId, string projectCode)
        {
            List<ItemTransactionData> items = new List<ItemTransactionData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"WITH OrderedItemsPart1 AS (
                                    SELECT 
                                        its.IS_ItemStoreId,
                                        itm.IT_Id,                                        
                                        um.UOMName,
                                        itm.IT_MasterCompanyId,
                                        itm.IT_Status,
                                        im.ItemCode,
                                        im.ItemName,
                                        ivm.InventoryCurrentStock,
                                        ivm.InventoryMinStock,
                                        rtd.RackType,
                                        rsd.RackSerialNumber,
                                        SUM(its.IS_StoreQty) AS TotalItemInRack,
                                        itm.IT_BatchNumber,
                                        CONVERT(VARCHAR(10), itm.IT_ExpiryDateTime, 120) AS IT_ExpiryDateTime,
                                        ROW_NUMBER() OVER (ORDER BY itm.IT_ExpiryDateTime) AS ExpiryRank
                                    FROM 
                                        ItemStore its
                                        INNER JOIN ItemTransactionMaster itm ON itm.IT_Id = its.IS_ITId
                                        INNER JOIN InventoryMaster ivm ON ivm.InventoryId = itm.IT_InventoryId
                                        INNER JOIN Items im ON im.ItemId = itm.IT_ItemId
                                        INNER JOIN UOM um ON um.UOMId = itm.IT_UOMId
                                        INNER JOIN RackTypeDetails rtd ON rtd.RackTypeId = itm.IT_RackTypeId
                                        INNER JOIN RackSerialDetails rsd ON rsd.RackSerialId = its.IS_RackSerialNo
                                    WHERE 
                                            itm.IT_MasterCompanyId = @IT_MasterCompanyId AND
                                            im.ItemCode =  @ItemCode
                                    GROUP BY
                                        its.IS_ItemStoreId,
                                        itm.IT_Id,
                                        um.UOMName,
                                        itm.IT_MasterCompanyId,
                                        itm.IT_Status,
                                        im.ItemCode,
                                        im.ItemName,
                                        ivm.InventoryCurrentStock,
                                        ivm.InventoryMinStock,
                                        rtd.RackType,
                                        rsd.RackSerialNumber,
                                        itm.IT_BatchNumber,
                                        itm.IT_ExpiryDateTime
                                ),
                                CumulativeItemsPart1 AS (
                                    SELECT
                                        *,
                                        SUM(TotalItemInRack) OVER (ORDER BY ExpiryRank) AS CumulativeTotalItemInRack
                                    FROM
                                        OrderedItemsPart1
                                ),
                                FirstSatisfiedRowPart1 AS (
                                    SELECT 
                                        ExpiryRank
                                    FROM
                                        CumulativeItemsPart1
                                    WHERE 
                                        CumulativeTotalItemInRack >= @DesiredQuantity
                                )
                                , OrderedItemsPart2 AS (
                                    SELECT 
                                        itm.IT_Id,
                                        um.UOMName,
                                        itm.IT_MasterCompanyId,
                                        itm.IT_Status,
                                        im.ItemCode,
                                        im.ItemName,
                                        ivm.InventoryCurrentStock,
                                        ivm.InventoryMinStock,
                                        rtd.RackType,
                                        rsd.RackSerialNumber,
                                        SUM(its.IS_StoreQty) AS TotalItemInRack,
                                        itm.IT_BatchNumber,
                                        CONVERT(VARCHAR(10), itm.IT_ExpiryDateTime, 120) AS IT_ExpiryDateTime,
                                        ROW_NUMBER() OVER (ORDER BY itm.IT_ExpiryDateTime) AS ExpiryRank
                                    FROM 
                                        ItemStore its
                                        INNER JOIN ItemTransactionMaster itm ON itm.IT_Id = its.IS_ITId
                                        INNER JOIN InventoryMaster ivm ON ivm.InventoryId = itm.IT_InventoryId
                                        INNER JOIN Items im ON im.ItemId = itm.IT_ItemId
                                        INNER JOIN UOM um ON um.UOMId = itm.IT_UOMId
                                        INNER JOIN RackTypeDetails rtd ON rtd.RackTypeId = itm.IT_RackTypeId
                                        INNER JOIN RackSerialDetails rsd ON rsd.RackSerialId = its.IS_RackSerialNo
                                    WHERE 
                                        itm.IT_Id =  @IT_Id
                                    GROUP BY
                                        itm.IT_Id,
                                        um.UOMName,
                                        itm.IT_MasterCompanyId,
                                        itm.IT_Status,
                                        im.ItemCode,
                                        im.ItemName,
                                        ivm.InventoryCurrentStock,
                                        ivm.InventoryMinStock,
                                        rtd.RackType,
                                        rsd.RackSerialNumber,
                                        itm.IT_BatchNumber,
                                        itm.IT_ExpiryDateTime
                                ),
                                CumulativeItemsPart2 AS (
                                    SELECT
                                        *,
                                        SUM(TotalItemInRack) OVER (ORDER BY ExpiryRank) AS CumulativeTotalItemInRack
                                    FROM
                                        OrderedItemsPart2
                                )
                                SELECT
                                    OrderedItemsPart1.IS_ItemStoreId,
                                    OrderedItemsPart1.IT_Id AS IT_Id,                                   
                                    OrderedItemsPart1.UOMName AS UOMName,
                                    OrderedItemsPart1.IT_MasterCompanyId AS IT_MasterCompanyId,
                                    OrderedItemsPart1.IT_Status AS IT_Status,
                                    OrderedItemsPart1.ItemCode AS ItemCode,
                                    OrderedItemsPart1.ItemName AS ItemName,
                                    OrderedItemsPart1.InventoryCurrentStock AS InventoryCurrentStock,
                                    OrderedItemsPart1.InventoryMinStock AS InventoryMinStock,
                                    OrderedItemsPart1.RackType AS RackType,
                                    OrderedItemsPart1.RackSerialNumber AS RackSerialNumber,
                                    OrderedItemsPart1.TotalItemInRack AS TotalItemInRack,
                                    OrderedItemsPart1.IT_BatchNumber AS IT_BatchNumber,
                                    OrderedItemsPart1.IT_ExpiryDateTime AS IT_ExpiryDateTime,
                                    CumulativeItemsPart1.ExpiryRank AS ExpiryRank,
                                    CumulativeItemsPart1.CumulativeTotalItemInRack AS CumulativeTotalItemInRack,
                                    CASE
                                        WHEN OrderedItemsPart1.ExpiryRank <= (SELECT MIN(ExpiryRank) FROM FirstSatisfiedRowPart1) THEN 1
                                        ELSE 0
                                    END AS IsQtyGo,    
                                    CASE
                                        WHEN EXISTS (SELECT 1 FROM OrderedItemsPart2 WHERE IT_Id = OrderedItemsPart1.IT_Id) THEN 1
                                        ELSE 0
                                    END AS BothMatch
                                FROM
                                    OrderedItemsPart1
                                JOIN
                                    CumulativeItemsPart1 ON OrderedItemsPart1.ExpiryRank = CumulativeItemsPart1.ExpiryRank
                                ORDER BY
                                    OrderedItemsPart1.IT_ExpiryDateTime;
                                ";



                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IT_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    cmd.Parameters.AddWithValue("@IT_CompanyUnitId", HttpContext.Current.Session["CompanyUnitId"].ToString());
                    cmd.Parameters.AddWithValue("@IT_CreatedEmployeeId", HttpContext.Current.Session["UserID"].ToString());
                    cmd.Parameters.AddWithValue("@ItemCode", itemCodeGet);
                    cmd.Parameters.AddWithValue("@IT_ProjectID", projectCode);
                    cmd.Parameters.AddWithValue("@DesiredQuantity", Convert.ToDecimal(desiredQuantity));
                    cmd.Parameters.AddWithValue("@IT_Id", itId);

                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new ItemTransactionData
                            {
                                IT_Id = Convert.ToInt32(sdr["IT_Id"]),
                                IS_ItemStoreId = Convert.ToInt32(sdr["IS_ItemStoreId"]),
                                //IT_CompanyUnitId = Convert.ToInt32(sdr["IT_CompanyUnitId"]),
                                IT_MasterCompanyId = Convert.ToInt32(sdr["IT_MasterCompanyId"]),
                                IT_Status = Convert.ToInt32(sdr["IT_Status"]),
                                IT_BatchNumber = sdr["IT_BatchNumber"].ToString(),
                                IT_ExpiryDateTime = sdr["IT_ExpiryDateTime"] != DBNull.Value
                                                    ? DateTime.TryParse(sdr["IT_ExpiryDateTime"].ToString(), out DateTime expiryDateTime)
                                                    ? expiryDateTime.ToString("dd/MM/yyyy") : string.Empty : string.Empty,
                                TotalItemInRack = Convert.IsDBNull(sdr["TotalItemInRack"]) ? 0 : Convert.ToDecimal(sdr["TotalItemInRack"]),
                                InventoryCurrentStock = Convert.ToDecimal(sdr["InventoryCurrentStock"]),
                                InventoryMinStock = Convert.ToDecimal(sdr["InventoryMinStock"]),
                                ItemCode = sdr["ItemCode"].ToString(),
                                ItemName = sdr["ItemName"].ToString(),
                                UOMName = sdr["UOMName"].ToString(),
                                RackType = sdr["RackType"].ToString(),
                                RackSerialNumber = sdr["RackSerialNumber"].ToString(),

                                ExpiryRank = Convert.ToInt32(sdr["ExpiryRank"].ToString()),
                                CumulativeTotalItemInRack = Convert.ToDecimal(sdr["CumulativeTotalItemInRack"].ToString()),
                                IsQtyGo = Convert.ToInt32(sdr["IsQtyGo"].ToString()),
                                BothMatch = Convert.ToInt32(sdr["BothMatch"].ToString()),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<MasterTransactionData> MasterTransactionDataGet()
        {
            List<MasterTransactionData> items = new List<MasterTransactionData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM MaterialTransactionMaster
                                 Where MT_MasterCompanyId = @MT_MasterCompanyId
                                 ORDER BY CONVERT(datetime, MT_CreatedDateTime, 120) DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@MT_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new MasterTransactionData
                            {
                                MT_ID = Convert.IsDBNull(sdr["MT_ID"]) ? 0 : Convert.ToInt32(sdr["MT_ID"]),
                                MT_MRID = Convert.IsDBNull(sdr["MT_MRID"]) ? 0 : Convert.ToInt32(sdr["MT_MRID"]),
                                MT_MRMID = Convert.IsDBNull(sdr["MT_MRMID"]) ? 0 : Convert.ToInt32(sdr["MT_MRMID"]),
                                MT_ItemCode = Convert.IsDBNull(sdr["MT_ItemCode"]) ? string.Empty : sdr["MT_ItemCode"].ToString(),
                                MT_RequiredQty = Convert.IsDBNull(sdr["MT_RequiredQty"]) ? 0 : Convert.ToDecimal(sdr["MT_RequiredQty"]),
                                MT_ReceivedQty = Convert.IsDBNull(sdr["MT_ReceivedQty"]) ? 0 : Convert.ToDecimal(sdr["MT_ReceivedQty"]),
                                MT_BalanceQty = Convert.IsDBNull(sdr["MT_BalanceQty"]) ? 0 : Convert.ToDecimal(sdr["MT_BalanceQty"]),
                                MT_PreviousReceivedQty = Convert.IsDBNull(sdr["MT_PreviousReceivedQty"]) ? 0 : Convert.ToDecimal(sdr["MT_PreviousReceivedQty"]),
                                MT_PreviousBalanceQty = Convert.IsDBNull(sdr["MT_PreviousBalanceQty"]) ? 0 : Convert.ToDecimal(sdr["MT_PreviousBalanceQty"]),
                                MT_CreatedDateTime = Convert.IsDBNull(sdr["MT_CreatedDateTime"]) ? string.Empty : sdr["MT_CreatedDateTime"].ToString(),
                                MT_UpdateDateTime = Convert.IsDBNull(sdr["MT_UpdateDateTime"]) ? string.Empty : sdr["MT_UpdateDateTime"].ToString(),
                                MT_MasterCompanyId = Convert.IsDBNull(sdr["MT_MasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["MT_MasterCompanyId"]),
                                MT_CompanyUnitId = Convert.IsDBNull(sdr["MT_CompanyUnitId"]) ? 0 : Convert.ToInt32(sdr["MT_CompanyUnitId"]),
                                MT_EmployeeId = Convert.IsDBNull(sdr["MT_EmployeeId"]) ? 0 : Convert.ToInt32(sdr["MT_EmployeeId"]),
                                MT_StatusId = Convert.IsDBNull(sdr["MT_StatusId"]) ? 0 : Convert.ToInt32(sdr["MT_StatusId"]),
                                //MT_BatchNo = Convert.IsDBNull(sdr["MT_BatchNo"]) ? string.Empty : sdr["MT_BatchNo"].ToString(),
                                //MT_ExpiryDate = Convert.IsDBNull(sdr["MT_ExpiryDate"]) ? string.Empty : sdr["MT_ExpiryDate"].ToString(),

                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<MaterialTransationSubData> MaterialTransationSubDataGet()
        {
            List<MaterialTransationSubData> items = new List<MaterialTransationSubData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM MaterialTransationSubData";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new MaterialTransationSubData
                            {
                                MTS_ID = Convert.IsDBNull(sdr["MTS_ID"]) ? 0 : Convert.ToInt32(sdr["MTS_ID"]),
                                MTS_MTID = Convert.IsDBNull(sdr["MTS_MTID"]) ? 0 : Convert.ToInt32(sdr["MTS_MTID"]),
                                MTS_ItemStock = Convert.IsDBNull(sdr["MTS_ItemStock"]) ? 0 : Convert.ToDecimal(sdr["MTS_ItemStock"]),
                                MTS_BatchNo = Convert.IsDBNull(sdr["MTS_BatchNo"]) ? string.Empty : Convert.ToString(sdr["MTS_BatchNo"]),
                                MTS_ExpiryDate = Convert.IsDBNull(sdr["MTS_ExpiryDate"]) ? string.Empty : Convert.ToString(sdr["MTS_ExpiryDate"]),
                                MTS_CreatedDateTime = Convert.IsDBNull(sdr["MTS_CreatedDateTime"]) ? string.Empty : Convert.ToString(sdr["MTS_CreatedDateTime"]),
                                MTS_UpdatedDateTime = Convert.IsDBNull(sdr["MTS_UpdatedDateTime"]) ? string.Empty : Convert.ToString(sdr["MTS_UpdatedDateTime"]),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }


        /* MT BATCH TAG */
        public List<MaterialRequestData> MaterialTransationTAGDataget()
        {
            List<MaterialRequestData> items = new List<MaterialRequestData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT MRM_NUMBER,MR_ItemCode, MR_ItemName, MR_EmployeeRequireItemQty, MR_ReceiveQty, MR_BalanceQty, MR_RequireDate,
                                 MTS_BatchNo, MTS_ExpiryDate, SUM(MTS_ItemStock) AS MTS_ItemStock, 
                                 MT_MasterCompanyId, MT_CompanyUnitId, MT_EmployeeId
                                 FROM MaterialTransationSubData mtsd
                                 INNER JOIN MaterialTransactionMaster mtm ON mtm.MT_ID = mtsd.MTS_MTID
                                 INNER JOIN MaterialRequestDetails mrd ON mrd.MR_ID = mtm.MT_MRID
                                 INNER JOIN MaterialRequestMaster mrm ON mrm.MRM_ID = mrd.MR_MRMID								 
                                 GROUP BY MRM_NUMBER, MR_ItemCode, MR_ItemName, MR_EmployeeRequireItemQty, MR_ReceiveQty, MR_BalanceQty, MR_RequireDate,
                                 MTS_BatchNo, MTS_ExpiryDate,MT_MasterCompanyId ,MT_CompanyUnitId,MT_EmployeeId;
                                ";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new MaterialRequestData
                            {
                                MRM_NUMBER = Convert.IsDBNull(sdr["MRM_NUMBER"]) ? string.Empty : Convert.ToString(sdr["MRM_NUMBER"]),
                                MR_ItemCode = Convert.IsDBNull(sdr["MR_ItemCode"]) ? string.Empty : Convert.ToString(sdr["MR_ItemCode"]),
                                MR_ItemName = Convert.IsDBNull(sdr["MR_ItemName"]) ? string.Empty : Convert.ToString(sdr["MR_ItemName"]),
                                MR_EmployeeRequireItemQty = Convert.IsDBNull(sdr["MR_EmployeeRequireItemQty"]) ? 0 : Convert.ToDecimal(sdr["MR_EmployeeRequireItemQty"]),
                                MR_ReceiveQty = Convert.IsDBNull(sdr["MR_ReceiveQty"]) ? 0 : Convert.ToDecimal(sdr["MR_ReceiveQty"]),
                                MR_BalanceQty = Convert.IsDBNull(sdr["MR_BalanceQty"]) ? 0 : Convert.ToDecimal(sdr["MR_BalanceQty"]),
                                MTS_ItemStock = Convert.IsDBNull(sdr["MTS_ItemStock"]) ? 0 : Convert.ToDecimal(sdr["MTS_ItemStock"]),
                                MTS_BatchNo = Convert.IsDBNull(sdr["MTS_BatchNo"]) ? string.Empty : Convert.ToString(sdr["MTS_BatchNo"]),
                                MTS_ExpiryDate = Convert.IsDBNull(sdr["MTS_ExpiryDate"]) ? string.Empty : Convert.ToString(sdr["MTS_ExpiryDate"]),
                                MT_MasterCompanyId = Convert.IsDBNull(sdr["MT_MasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["MT_MasterCompanyId"]),
                                MT_CompanyUnitId = Convert.IsDBNull(sdr["MT_CompanyUnitId"]) ? 0 : Convert.ToInt32(sdr["MT_CompanyUnitId"]),
                                MT_EmployeeId = Convert.IsDBNull(sdr["MT_EmployeeId"]) ? 0 : Convert.ToInt32(sdr["MT_EmployeeId"]),
                                MR_RequireDate = Convert.IsDBNull(sdr["MR_RequireDate"]) ? string.Empty : sdr["MR_RequireDate"].ToString(),
                                //MTS_ID = Convert.IsDBNull(sdr["MTS_ID"]) ? 0 : Convert.ToInt32(sdr["MTS_ID"]),
                                //MTS_MTID = Convert.IsDBNull(sdr["MTS_MTID"]) ? 0 : Convert.ToInt32(sdr["MTS_MTID"]),
                                //MTS_CreatedDateTime = Convert.IsDBNull(sdr["MTS_CreatedDateTime"]) ? string.Empty : Convert.ToString(sdr["MTS_CreatedDateTime"]),
                                //MTS_UpdatedDateTime = Convert.IsDBNull(sdr["MTS_UpdatedDateTime"]) ? string.Empty : Convert.ToString(sdr["MTS_UpdatedDateTime"]),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }


        /* Received Total Item from the Other-Company/Vendor */
        public List<ItemTransactionData> TotalMaterialReceivedData(string itemCodeGet, string companyUnitId, string mothsGet, string yearsGet)
        {
            List<ItemTransactionData> items = new List<ItemTransactionData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT 
	                                IT_ItemId, 
	                                ItemCode, ItemName,
	                                SUM(IT_ReceivedQty) AS TotalReceived, 
	                                ivm.InventoryCurrentStock AS InventoryCurrentStock, 
	                                IT_MasterCompanyId, 
	                                IT_CompanyUnitId
                                 FROM ItemMaster im
                                 INNER JOIN ItemTransactionMaster itm ON itm.IT_ItemId = im.ItemId
                                 INNER JOIN InventoryMaster ivm ON ivm.InventoryItemId = im.ItemId
                                 WHERE 
                                    ItemCode = @ItemCode AND 
                                    IT_MasterCompanyId = @IT_MasterCompanyId AND InventoryMasterCompanyId = @IT_MasterCompanyId AND 
                                    IT_CompanyUnitId = @IT_CompanyUnitId AND InventoryCompanyUnitId = @IT_CompanyUnitId AND
                                    MONTH(IT_CreatedDateTime) = @Months AND
                                    YEAR(IT_CreatedDateTime) = @Year
                                 Group By
	                                IT_ItemId,ItemCode,ItemName,InventoryCurrentStock,IT_MasterCompanyId,IT_CompanyUnitId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ItemCode", itemCodeGet);
                    cmd.Parameters.AddWithValue("@IT_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    cmd.Parameters.AddWithValue("@IT_CompanyUnitId", Convert.ToInt32(companyUnitId));
                    cmd.Parameters.AddWithValue("@Months", Convert.ToInt32(mothsGet));
                    cmd.Parameters.AddWithValue("@Year", Convert.ToInt32(yearsGet));

                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new ItemTransactionData
                            {
                                IT_ItemId = Convert.IsDBNull(sdr["IT_ItemId"]) ? 0 : Convert.ToInt32(sdr["IT_ItemId"]),
                                ItemCode = Convert.IsDBNull(sdr["ItemCode"]) ? string.Empty : Convert.ToString(sdr["ItemCode"]),
                                ItemName = Convert.IsDBNull(sdr["ItemName"]) ? string.Empty : Convert.ToString(sdr["ItemName"]),
                                IT_ReceivedQty = Convert.IsDBNull(sdr["TotalReceived"]) ? 0 : Convert.ToDecimal(sdr["TotalReceived"]),

                                //IT_ShortFall = Convert.IsDBNull(sdr["IT_ShortFall"]) ? 0 : Convert.ToDecimal(sdr["IT_ShortFall"]),
                                //IT_PreviousShortFall = Convert.IsDBNull(sdr["IT_PreviousShortFall"]) ? 0 : Convert.ToDecimal(sdr["IT_PreviousShortFall"]),
                                //IT_ReceivedQtyInvoice = Convert.IsDBNull(sdr["IT_ReceivedQtyInvoice"]) ? 0 : Convert.ToDecimal(sdr["IT_ReceivedQtyInvoice"]),
                                //IT_PreviousReceivedQtyInvoice = Convert.IsDBNull(sdr["IT_PreviousReceivedQtyInvoice"]) ? 0 : Convert.ToDecimal(sdr["IT_PreviousReceivedQtyInvoice"]),

                                InventoryCurrentStock = Convert.IsDBNull(sdr["InventoryCurrentStock"]) ? 0 : Convert.ToDecimal(sdr["InventoryCurrentStock"]),
                                IT_MasterCompanyId = Convert.IsDBNull(sdr["IT_MasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["IT_MasterCompanyId"]),
                                IT_CompanyUnitId = Convert.IsDBNull(sdr["IT_CompanyUnitId"]) ? 0 : Convert.ToInt32(sdr["IT_CompanyUnitId"]),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }


        /* Released Total Item from the Unit */
        public List<MaterialRequestData> TotalMaterialRelesedData(string itemCodeGet, string companyUnitId, string mothsGet, string yearsGet)
        {
            List<MaterialRequestData> items = new List<MaterialRequestData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT 
                                     MT_ItemCode, ItemName,   
                                     SUM(MT_ReceivedQty) AS TotalReleasedQty,   
                                     MT_MasterCompanyId,
                                     MT_CompanyUnitId 
                                 FROM 
                                    MaterialTransactionMaster mtm
                                 INNER JOIN ItemMaster im ON im.ItemCode = mtm.MT_ItemCode
                                 WHERE 
                                     MT_ItemCode  = @MT_ItemCode
                                    AND  MT_CompanyUnitId  = @MT_CompanyUnitId
                                    AND  MT_MasterCompanyId  = @MT_MasterCompanyId
                                    AND MONTH(MT_CreatedDateTime) = @Months
                                    AND YEAR(MT_CreatedDateTime) = @Year
                                 GROUP BY
                                     MT_ItemCode, ItemName,
                                     MT_MasterCompanyId ,
                                     MT_CompanyUnitId; ";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@MT_ItemCode", itemCodeGet);
                    cmd.Parameters.AddWithValue("@MT_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    cmd.Parameters.AddWithValue("@MT_CompanyUnitId", Convert.ToInt32(companyUnitId));
                    cmd.Parameters.AddWithValue("@Months", Convert.ToInt32(mothsGet));
                    cmd.Parameters.AddWithValue("@Year", Convert.ToInt32(yearsGet));

                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new MaterialRequestData
                            {
                                MT_ItemCode = Convert.IsDBNull(sdr["MT_ItemCode"]) ? string.Empty : Convert.ToString(sdr["MT_ItemCode"]),
                                ItemName = Convert.IsDBNull(sdr["ItemName"]) ? string.Empty : Convert.ToString(sdr["ItemName"]),
                                TotalReleasedQty = Convert.IsDBNull(sdr["TotalReleasedQty"]) ? 0 : Convert.ToDecimal(sdr["TotalReleasedQty"]),
                                MT_MasterCompanyId = Convert.IsDBNull(sdr["MT_MasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["MT_MasterCompanyId"]),
                                MT_CompanyUnitId = Convert.IsDBNull(sdr["MT_CompanyUnitId"]) ? 0 : Convert.ToInt32(sdr["MT_CompanyUnitId"]),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }


        /* Item received Transaction Month-Year wise get data */
        public List<ItemTransactionData> ItemReceivedStatementGetMonthAndYearWise(string itemIdGet, string companyUnitId, string mothsGet, string yearsGet)
        {
            List<ItemTransactionData> items = new List<ItemTransactionData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT IT_OrderedQty,IT_ReceivedQty,IT_AcceptedQty,IT_RejectedQty,IT_BalanceQty,IT_CreatedDateTime,UOMName
                                  from ItemTransactionMaster itr
                                 INNER JOIN InventoryMaster im ON im.InventoryId = itr.IT_InventoryId
                                 INNER JOIN ItemMaster itm ON itm.ItemId = itr.IT_ItemId
                                 INNER JOIN VendorDetailsMaster vds ON vds.VendorId = itr.IT_VendorId
                                 INNER JOIN UOM um ON um.UOMId = itr.IT_UOMId
                                 INNER JOIN VAT vt on vt.VATID = itr.IT_GSTId
                                 INNER JOIN RackTypeDetails rtd ON rtd.RackTypeId = itr.IT_RackTypeId
                                 INNER JOIN CompanyUnitMaster cum ON cum.CompanyUnitId = itr.IT_CompanyUnitId
                                 WHERE IT_MasterCompanyId = @IT_MasterCompanyId AND IT_CompanyUnitId = @IT_CompanyUnitId AND IT_ItemId = @IT_ItemId
                                 ORDER BY CONVERT(datetime, itr.IT_CreatedDateTime, 120) ASC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IT_ItemId", Convert.ToInt32(itemIdGet));
                    cmd.Parameters.AddWithValue("@IT_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    cmd.Parameters.AddWithValue("@IT_CompanyUnitId", Convert.ToInt32(companyUnitId));
                    cmd.Parameters.AddWithValue("@Months", Convert.ToInt32(mothsGet));
                    cmd.Parameters.AddWithValue("@Year", Convert.ToInt32(yearsGet));

                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new ItemTransactionData
                            {
                                IT_Id = Convert.ToInt32(sdr["IT_Id"]),
                                IT_InventoryId = Convert.ToInt32(sdr["IT_InventoryId"]),
                                IT_ItemId = Convert.ToInt32(sdr["IT_ItemId"]),
                                IT_VendorId = Convert.ToInt32(sdr["IT_VendorId"]),
                                IT_PoNumber = sdr["IT_PoNumber"].ToString(),
                                IT_UOMId = Convert.ToInt32(sdr["IT_UOMId"]),
                                IT_GSTId = Convert.ToInt32(sdr["IT_GSTId"]),
                                IT_CompanyUnitId = Convert.ToInt32(sdr["IT_CompanyUnitId"]),
                                IT_RackTypeId = Convert.ToInt32(sdr["IT_RackTypeId"]),
                                IT_BasicPrice = Convert.ToDecimal(sdr["IT_BasicPrice"]),
                                IT_ReceivedQty = Convert.ToDecimal(sdr["IT_ReceivedQty"]),
                                IT_BalanceQty = Convert.ToDecimal(sdr["IT_BalanceQty"]),
                                IT_TotalAmount = Convert.ToDecimal(sdr["IT_TotalAmount"]),
                                IT_BatchNumber = sdr["IT_BatchNumber"].ToString(),
                                IT_ExpiryDateTime = sdr["IT_ExpiryDateTime"].ToString(),
                                IT_DeliveryDateTime = sdr["IT_DeliveryDateTime"].ToString(),
                                IT_CreatedDateTime = sdr["IT_CreatedDateTime"].ToString(),
                                IT_UpdatedDateTime = sdr["IT_UpdatedDateTime"].ToString(),
                                IT_RackRowSeriesNo = sdr["IT_RackRowSeriesNo"].ToString(),
                                IT_CreatedEmployeeId = Convert.ToInt32(sdr["IT_CreatedEmployeeId"]),
                                IT_MasterCompanyId = Convert.ToInt32(sdr["IT_MasterCompanyId"]),
                                IT_Status = Convert.ToInt32(sdr["IT_Status"]),
                                IT_PreviousReceivedQty = Convert.ToDecimal(sdr["IT_PreviousReceivedQty"]),
                                IT_PreviousBalanceQty = Convert.ToDecimal(sdr["IT_PreviousBalanceQty"]),
                                IT_AcceptedQty = Convert.ToDecimal(sdr["IT_AcceptedQty"]),
                                IT_RejectedQty = Convert.ToDecimal(sdr["IT_RejectedQty"]),
                                IT_PreviousAcceptedQty = Convert.ToDecimal(sdr["IT_PreviousAcceptedQty"]),
                                IT_PreviousRejectedQty = Convert.ToDecimal(sdr["IT_PreviousRejectedQty"]),
                                IT_PoId = Convert.ToInt32(sdr["IT_PoId"]),
                                IT_OrderedQty = Convert.ToDecimal(sdr["IT_OrderedQty"]),
                                //IT_QRCodeNumber = sdr["IT_QRCodeNumber"].ToString(),
                                //IT_QRFileName = sdr["IT_QRFileName"].ToString(),
                                //IT_CompanyUnitId = Convert.ToInt32(sdr["IT_CompanyUnitId"]),
                                IT_IsDelete = Convert.IsDBNull(sdr["IT_IsDelete"]) ? 0 : Convert.ToInt32(sdr["IT_IsDelete"]),

                                UOMName = sdr["UOMName"].ToString(),

                                UnitName = sdr["UnitName"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }


        /* Item Released Transaction Month-Year wise get data */
        public List<MasterTransactionData> ItemRelesedStatementGetMonthAndYearWise(string itemCodeGet, string companyUnitId, string mothsGet, string yearsGet)
        {
            List<MasterTransactionData> items = new List<MasterTransactionData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT 
                                       MT_ItemCode,
                                       CONVERT(date, MT_CreatedDateTime) AS TransactionDate,    
                                       SUM(MT_ReceivedQty) AS TotalReceivedQty    
                                   FROM 
                                       MaterialTransactionMaster
                                   WHERE 
                                       MT_MasterCompanyId = @MT_MasterCompanyId
                                       AND MT_CompanyUnitId = @MT_CompanyUnitId
                                       AND MT_ItemCode = @MT_ItemCode 
                                       AND MONTH(MT_CreatedDateTime) = @Months 
                                       AND YEAR(MT_CreatedDateTime) = @Year
                                   GROUP BY 
	                                   MT_ItemCode,
                                       CONVERT(date, MT_CreatedDateTime)
                                   ORDER BY 
                                       TransactionDate ASC;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@MT_ItemCode", itemCodeGet);
                    cmd.Parameters.AddWithValue("@MT_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    cmd.Parameters.AddWithValue("@MT_CompanyUnitId", Convert.ToInt32(companyUnitId));
                    cmd.Parameters.AddWithValue("@Months", Convert.ToInt32(mothsGet));
                    cmd.Parameters.AddWithValue("@Year", Convert.ToInt32(yearsGet));
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new MasterTransactionData
                            {
                                MT_ID = Convert.IsDBNull(sdr["MT_ID"]) ? 0 : Convert.ToInt32(sdr["MT_ID"]),
                                MT_MRID = Convert.IsDBNull(sdr["MT_MRID"]) ? 0 : Convert.ToInt32(sdr["MT_MRID"]),
                                MT_MRMID = Convert.IsDBNull(sdr["MT_MRMID"]) ? 0 : Convert.ToInt32(sdr["MT_MRMID"]),
                                MT_ItemCode = Convert.IsDBNull(sdr["MT_ItemCode"]) ? string.Empty : sdr["MT_ItemCode"].ToString(),
                                MT_RequiredQty = Convert.IsDBNull(sdr["MT_RequiredQty"]) ? 0 : Convert.ToDecimal(sdr["MT_RequiredQty"]),
                                MT_ReceivedQty = Convert.IsDBNull(sdr["MT_ReceivedQty"]) ? 0 : Convert.ToDecimal(sdr["MT_ReceivedQty"]),
                                MT_BalanceQty = Convert.IsDBNull(sdr["MT_BalanceQty"]) ? 0 : Convert.ToDecimal(sdr["MT_BalanceQty"]),
                                MT_PreviousReceivedQty = Convert.IsDBNull(sdr["MT_PreviousReceivedQty"]) ? 0 : Convert.ToDecimal(sdr["MT_PreviousReceivedQty"]),
                                MT_PreviousBalanceQty = Convert.IsDBNull(sdr["MT_PreviousBalanceQty"]) ? 0 : Convert.ToDecimal(sdr["MT_PreviousBalanceQty"]),
                                MT_CreatedDateTime = Convert.IsDBNull(sdr["MT_CreatedDateTime"]) ? string.Empty : sdr["MT_CreatedDateTime"].ToString(),
                                MT_UpdateDateTime = Convert.IsDBNull(sdr["MT_UpdateDateTime"]) ? string.Empty : sdr["MT_UpdateDateTime"].ToString(),
                                MT_MasterCompanyId = Convert.IsDBNull(sdr["MT_MasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["MT_MasterCompanyId"]),
                                MT_CompanyUnitId = Convert.IsDBNull(sdr["MT_CompanyUnitId"]) ? 0 : Convert.ToInt32(sdr["MT_CompanyUnitId"]),
                                MT_EmployeeId = Convert.IsDBNull(sdr["MT_EmployeeId"]) ? 0 : Convert.ToInt32(sdr["MT_EmployeeId"]),
                                MT_StatusId = Convert.IsDBNull(sdr["MT_StatusId"]) ? 0 : Convert.ToInt32(sdr["MT_StatusId"]),
                                //MT_BatchNo = Convert.IsDBNull(sdr["MT_BatchNo"]) ? string.Empty : sdr["MT_BatchNo"].ToString(),
                                //MT_ExpiryDate = Convert.IsDBNull(sdr["MT_ExpiryDate"]) ? string.Empty : sdr["MT_ExpiryDate"].ToString(),

                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }


        /* WarehouseStockDetails show */
        public List<WarehouseStockDetailsData> WarehouseStockDetailsGet()
        {
            List<WarehouseStockDetailsData> items = new List<WarehouseStockDetailsData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM WarehouseStockDetails where SC_MasterCompanyId = @SC_MasterCompanyId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@SC_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new WarehouseStockDetailsData
                            {
                                SC_ID = Convert.IsDBNull(sdr["SC_ID"]) ? 0 : Convert.ToInt32(sdr["SC_ID"]),
                                SC_ItemCode = Convert.IsDBNull(sdr["SC_ItemCode"]) ? string.Empty : sdr["SC_ItemCode"].ToString(),
                                SC_TotalReceivedQty = Convert.IsDBNull(sdr["SC_TotalReceivedQty"]) ? 0 : Convert.ToDecimal(sdr["SC_TotalReceivedQty"]),
                                SC_TotalReleasedQty = Convert.IsDBNull(sdr["SC_TotalReleasedQty"]) ? 0 : Convert.ToDecimal(sdr["SC_TotalReleasedQty"]),
                                SC_BalanceQty = Convert.IsDBNull(sdr["SC_BalanceQty"]) ? 0 : Convert.ToDecimal(sdr["SC_BalanceQty"]),
                                //SC_PreTotalReceivedQty = Convert.IsDBNull(sdr["SC_PreTotalReceivedQty"]) ? 0 : Convert.ToDecimal(sdr["SC_PreTotalReceivedQty"]),
                                //SC_PreTotalReleasedQty = Convert.IsDBNull(sdr["SC_PreTotalReleasedQty"]) ? 0 : Convert.ToDecimal(sdr["SC_PreTotalReleasedQty"]),
                                //SC_PreBalanceQty = Convert.IsDBNull(sdr["SC_PreBalanceQty"]) ? 0 : Convert.ToDecimal(sdr["SC_PreBalanceQty"]),
                                //SC_ITID = Convert.IsDBNull(sdr["SC_ITID"]) ? 0 : Convert.ToInt32(sdr["SC_ITID"]),
                                //SC_MTID = Convert.IsDBNull(sdr["SC_MTID"]) ? 0 : Convert.ToInt32(sdr["SC_MTID"]),
                                SC_MasterCompanyId = Convert.IsDBNull(sdr["SC_MasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["SC_MasterCompanyId"]),
                                SC_CompanyUnitId = Convert.IsDBNull(sdr["SC_CompanyUnitId"]) ? 0 : Convert.ToInt32(sdr["SC_CompanyUnitId"]),
                                SC_EmployeeId = Convert.IsDBNull(sdr["SC_EmployeeId"]) ? 0 : Convert.ToInt32(sdr["SC_EmployeeId"]),
                                SC_EntryDate = Convert.IsDBNull(sdr["SC_EntryDate"]) ? string.Empty : sdr["SC_EntryDate"].ToString(),
                                SC_UOM = Convert.IsDBNull(sdr["SC_UOM"]) ? string.Empty : sdr["SC_UOM"].ToString(),
                                SC_CreatedDateTime = Convert.IsDBNull(sdr["SC_CreatedDateTime"]) ? string.Empty : sdr["SC_CreatedDateTime"].ToString(),
                                SC_UpdatedDateTime = Convert.IsDBNull(sdr["SC_UpdatedDateTime"]) ? string.Empty : sdr["SC_UpdatedDateTime"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }


        /* Month wise Stock show */
        public List<WarehouseStockDetailsData> WarehouseStockDetailsMonthWise(string itemCodeGet, string companyUnitId, string mothsGet, string yearsGet)
        {
            List<WarehouseStockDetailsData> items = new List<WarehouseStockDetailsData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM StockManage WHERE MONTH(SC_EntryDate) = @Month AND YEAR(SC_EntryDate) = @Year AND 
                                 SC_ItemCode = @SC_ItemCode AND SC_MasterCompanyId = @SC_MasterCompanyId AND SC_CompanyUnitId = @SC_CompanyUnitId
	                             ORDER BY SC_CreatedDateTime ASC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@SC_ItemCode", itemCodeGet);
                    cmd.Parameters.AddWithValue("@SC_CompanyUnitId", Convert.ToInt32(companyUnitId));
                    cmd.Parameters.AddWithValue("@Month", Convert.ToInt32(mothsGet));
                    cmd.Parameters.AddWithValue("@Year", Convert.ToInt32(yearsGet));
                    cmd.Parameters.AddWithValue("@SC_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());

                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new WarehouseStockDetailsData
                            {
                                SC_ID = Convert.IsDBNull(sdr["SC_ID"]) ? 0 : Convert.ToInt32(sdr["SC_ID"]),
                                SC_ItemCode = Convert.IsDBNull(sdr["SC_ItemCode"]) ? string.Empty : sdr["SC_ItemCode"].ToString(),
                                SC_TotalReceivedQty = Convert.IsDBNull(sdr["SC_TotalReceivedQty"]) ? 0 : Convert.ToDecimal(sdr["SC_TotalReceivedQty"]),
                                SC_TotalReleasedQty = Convert.IsDBNull(sdr["SC_TotalReleasedQty"]) ? 0 : Convert.ToDecimal(sdr["SC_TotalReleasedQty"]),
                                OpeningStock = Convert.IsDBNull(sdr["OpeningStock"]) ? 0 : Convert.ToDecimal(sdr["OpeningStock"]),
                                ClosingStock = Convert.IsDBNull(sdr["ClosingStock"]) ? 0 : Convert.ToDecimal(sdr["ClosingStock"]),
                                SC_MasterCompanyId = Convert.IsDBNull(sdr["SC_MasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["SC_MasterCompanyId"]),
                                SC_CompanyUnitId = Convert.IsDBNull(sdr["SC_CompanyUnitId"]) ? 0 : Convert.ToInt32(sdr["SC_CompanyUnitId"]),
                                SC_EmployeeId = Convert.IsDBNull(sdr["SC_EmployeeId"]) ? 0 : Convert.ToInt32(sdr["SC_EmployeeId"]),
                                SC_EntryDate = Convert.IsDBNull(sdr["SC_EntryDate"]) ? string.Empty : sdr["SC_EntryDate"].ToString(),
                                SC_UOM = Convert.IsDBNull(sdr["SC_UOM"]) ? string.Empty : sdr["SC_UOM"].ToString(),
                                SC_CreatedDateTime = Convert.IsDBNull(sdr["SC_CreatedDateTime"]) ? string.Empty : sdr["SC_CreatedDateTime"].ToString(),
                                SC_UpdatedDateTime = Convert.IsDBNull(sdr["SC_UpdatedDateTime"]) ? string.Empty : sdr["SC_UpdatedDateTime"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }


        public List<WarehouseStockDetailsData> MonthWiseStockShow(string companyUnitId, string mothsGet, string yearsGet)
        {
            List<WarehouseStockDetailsData> items = new List<WarehouseStockDetailsData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM StockManageForAllItem 
                                 WHERE MONTH(SC_EntryDate) = @Month AND YEAR(SC_EntryDate) = @Year AND 
                                 SC_MasterCompanyId = @SC_MasterCompanyId AND SC_CompanyUnitId = @SC_CompanyUnitId
                                 ORDER BY SC_ItemCode ASC, SC_EntryDate ASC;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {

                    cmd.Parameters.AddWithValue("@SC_CompanyUnitId", Convert.ToInt32(companyUnitId));
                    cmd.Parameters.AddWithValue("@Month", Convert.ToInt32(mothsGet));
                    cmd.Parameters.AddWithValue("@Year", Convert.ToInt32(yearsGet));
                    cmd.Parameters.AddWithValue("@SC_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());

                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new WarehouseStockDetailsData
                            {
                                SC_ID = Convert.IsDBNull(sdr["SC_ID"]) ? 0 : Convert.ToInt32(sdr["SC_ID"]),
                                SC_ItemCode = Convert.IsDBNull(sdr["SC_ItemCode"]) ? string.Empty : sdr["SC_ItemCode"].ToString(),
                                SC_TotalReceivedQty = Convert.IsDBNull(sdr["SC_TotalReceivedQty"]) ? 0 : Convert.ToDecimal(sdr["SC_TotalReceivedQty"]),
                                SC_TotalReleasedQty = Convert.IsDBNull(sdr["SC_TotalReleasedQty"]) ? 0 : Convert.ToDecimal(sdr["SC_TotalReleasedQty"]),
                                OpeningStock = Convert.IsDBNull(sdr["OpeningStock"]) ? 0 : Convert.ToDecimal(sdr["OpeningStock"]),
                                ClosingStock = Convert.IsDBNull(sdr["ClosingStock"]) ? 0 : Convert.ToDecimal(sdr["ClosingStock"]),
                                SC_MasterCompanyId = Convert.IsDBNull(sdr["SC_MasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["SC_MasterCompanyId"]),
                                SC_CompanyUnitId = Convert.IsDBNull(sdr["SC_CompanyUnitId"]) ? 0 : Convert.ToInt32(sdr["SC_CompanyUnitId"]),
                                SC_EmployeeId = Convert.IsDBNull(sdr["SC_EmployeeId"]) ? 0 : Convert.ToInt32(sdr["SC_EmployeeId"]),
                                SC_EntryDate = Convert.IsDBNull(sdr["SC_EntryDate"]) ? string.Empty : sdr["SC_EntryDate"].ToString(),
                                SC_UOM = Convert.IsDBNull(sdr["SC_UOM"]) ? string.Empty : sdr["SC_UOM"].ToString(),
                                SC_CreatedDateTime = Convert.IsDBNull(sdr["SC_CreatedDateTime"]) ? string.Empty : sdr["SC_CreatedDateTime"].ToString(),
                                SC_UpdatedDateTime = Convert.IsDBNull(sdr["SC_UpdatedDateTime"]) ? string.Empty : sdr["SC_UpdatedDateTime"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }


        public List<WarehouseStockDetailsData> MonthWiseStockShowPaged(string companyUnitId, string month, string year, string searchTerm, int pageIndex, int pageSize, out int totalRecords)
        {
            List<WarehouseStockDetailsData> items = new List<WarehouseStockDetailsData>();
            totalRecords = 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string baseWhere = @"WHERE MONTH(SC_EntryDate) = @Month 
                        AND YEAR(SC_EntryDate) = @Year 
                        AND SC_MasterCompanyId = @SC_MasterCompanyId 
                        AND SC_CompanyUnitId = @SC_CompanyUnitId
                        AND (@SearchTerm = '' OR 
                            SC_ItemCode LIKE '%' + @SearchTerm + '%')";

                string countQuery = $@"SELECT COUNT(*) FROM StockManageForAllItem {baseWhere}";

                string dataQuery = $@"SELECT * FROM StockManageForAllItem 
                         {baseWhere}
                         ORDER BY SC_ItemCode ASC, SC_EntryDate ASC
                         OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                using (SqlCommand countCmd = new SqlCommand(countQuery, con))
                using (SqlCommand dataCmd = new SqlCommand(dataQuery, con))
                {
                    // Common parameters
                    foreach (var cmd in new[] { countCmd, dataCmd })
                    {
                        cmd.Parameters.AddWithValue("@SC_CompanyUnitId", Convert.ToInt32(companyUnitId));
                        cmd.Parameters.AddWithValue("@Month", Convert.ToInt32(month));
                        cmd.Parameters.AddWithValue("@Year", Convert.ToInt32(year));
                        cmd.Parameters.AddWithValue("@SC_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                        cmd.Parameters.AddWithValue("@SearchTerm", searchTerm ?? "");
                    }

                    // Paging parameters
                    int offset = (pageIndex - 1) * pageSize;
                    dataCmd.Parameters.AddWithValue("@Offset", offset);
                    dataCmd.Parameters.AddWithValue("@PageSize", pageSize);

                    con.Open();

                    // Get total records first
                    totalRecords = (int)countCmd.ExecuteScalar();

                    // Then get paged data
                    using (SqlDataReader sdr = dataCmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new WarehouseStockDetailsData
                            {
                                SC_ID = Convert.IsDBNull(sdr["SC_ID"]) ? 0 : Convert.ToInt32(sdr["SC_ID"]),
                                SC_ItemCode = Convert.IsDBNull(sdr["SC_ItemCode"]) ? string.Empty : sdr["SC_ItemCode"].ToString(),
                                SC_TotalReceivedQty = Convert.IsDBNull(sdr["SC_TotalReceivedQty"]) ? 0 : Convert.ToDecimal(sdr["SC_TotalReceivedQty"]),
                                SC_TotalReleasedQty = Convert.IsDBNull(sdr["SC_TotalReleasedQty"]) ? 0 : Convert.ToDecimal(sdr["SC_TotalReleasedQty"]),
                                OpeningStock = Convert.IsDBNull(sdr["OpeningStock"]) ? 0 : Convert.ToDecimal(sdr["OpeningStock"]),
                                ClosingStock = Convert.IsDBNull(sdr["ClosingStock"]) ? 0 : Convert.ToDecimal(sdr["ClosingStock"]),
                                SC_MasterCompanyId = Convert.IsDBNull(sdr["SC_MasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["SC_MasterCompanyId"]),
                                SC_CompanyUnitId = Convert.IsDBNull(sdr["SC_CompanyUnitId"]) ? 0 : Convert.ToInt32(sdr["SC_CompanyUnitId"]),
                                SC_EmployeeId = Convert.IsDBNull(sdr["SC_EmployeeId"]) ? 0 : Convert.ToInt32(sdr["SC_EmployeeId"]),
                                SC_EntryDate = Convert.IsDBNull(sdr["SC_EntryDate"]) ? string.Empty : sdr["SC_EntryDate"].ToString(),
                                SC_UOM = Convert.IsDBNull(sdr["SC_UOM"]) ? string.Empty : sdr["SC_UOM"].ToString(),
                                SC_CreatedDateTime = Convert.IsDBNull(sdr["SC_CreatedDateTime"]) ? string.Empty : sdr["SC_CreatedDateTime"].ToString(),
                                SC_UpdatedDateTime = Convert.IsDBNull(sdr["SC_UpdatedDateTime"]) ? string.Empty : sdr["SC_UpdatedDateTime"].ToString(),
                            });
                        }
                    }
                }
            }
            return items;
        }


        /* Month wise TOTAL Stock show */
        public List<WarehouseStockDetailsData> WarehouseTotalStockMonthWise(string itemCodeGet, string companyUnitId, string mothsGet, string yearsGet)
        {
            List<WarehouseStockDetailsData> items = new List<WarehouseStockDetailsData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT 
                                    YEAR(SC_EntryDate) AS Years,
                                    MONTH(SC_EntryDate) AS Months,
                                    SUM(SC_TotalReceivedQty) AS SC_TotalReceivedQty,
                                    SUM(SC_TotalReleasedQty) AS SC_TotalReleasedQty,
                                    (SUM(SC_TotalReceivedQty) - SUM(SC_TotalReleasedQty)) AS ClosingStock
                                 FROM 
                                    WarehouseStockDetails
                                 WHERE 
                                    SC_ItemCode = @SC_ItemCode AND SC_MasterCompanyId = @SC_MasterCompanyId AND SC_CompanyUnitId = @SC_CompanyUnitId AND 
                                    YEAR(SC_EntryDate) = @Year AND MONTH(SC_EntryDate) = @Month
                                 GROUP BY 
                                    YEAR(SC_EntryDate), MONTH(SC_EntryDate)
                                 ORDER BY 
                                    Years, Months;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@SC_ItemCode", itemCodeGet);
                    cmd.Parameters.AddWithValue("@SC_CompanyUnitId", Convert.ToInt32(companyUnitId));
                    cmd.Parameters.AddWithValue("@Month", Convert.ToInt32(mothsGet));
                    cmd.Parameters.AddWithValue("@Year", Convert.ToInt32(yearsGet));
                    cmd.Parameters.AddWithValue("@SC_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());

                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new WarehouseStockDetailsData
                            {
                                Years = Convert.IsDBNull(sdr["Years"]) ? 0 : Convert.ToInt32(sdr["Years"]),
                                Months = Convert.IsDBNull(sdr["Months"]) ? 0 : Convert.ToInt32(sdr["Months"]),
                                SC_TotalReceivedQty = Convert.IsDBNull(sdr["SC_TotalReceivedQty"]) ? 0 : Convert.ToDecimal(sdr["SC_TotalReceivedQty"]),
                                SC_TotalReleasedQty = Convert.IsDBNull(sdr["SC_TotalReleasedQty"]) ? 0 : Convert.ToDecimal(sdr["SC_TotalReleasedQty"]),
                                ClosingStock = Convert.IsDBNull(sdr["ClosingStock"]) ? 0 : Convert.ToDecimal(sdr["ClosingStock"]),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }


        /* Year wise Stock show */
        public List<WarehouseStockDetailsData> WarehouseStockDetailsYearWise(string itemCodeGet, string companyUnitId, string yearsGet)
        {
            List<WarehouseStockDetailsData> items = new List<WarehouseStockDetailsData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT 
                                    SC_ItemCode,
                                    YEAR(SC_EntryDate) AS Years,
                                    MONTH(SC_EntryDate) AS Months,
                                    SUM(SC_TotalReceivedQty) AS SC_TotalReceivedQty,
                                    SUM(SC_TotalReleasedQty) AS SC_TotalReleasedQty,
                                    (SUM(SC_TotalReceivedQty) - SUM(SC_TotalReleasedQty)) AS ClosingStock
                                 FROM 
                                    WarehouseStockDetails
                                 WHERE 
                                    SC_ItemCode = @SC_ItemCode
                                    AND SC_MasterCompanyId = @SC_MasterCompanyId
                                    AND SC_CompanyUnitId = @SC_CompanyUnitId
                                    AND YEAR(SC_EntryDate) = @Year
                                 GROUP BY 
                                    SC_ItemCode, YEAR(SC_EntryDate), MONTH(SC_EntryDate)
                                 ORDER BY 
                                    SC_ItemCode, Years, Months;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@SC_ItemCode", itemCodeGet);
                    cmd.Parameters.AddWithValue("@SC_CompanyUnitId", Convert.ToInt32(companyUnitId));
                    cmd.Parameters.AddWithValue("@Year", Convert.ToInt32(yearsGet));
                    cmd.Parameters.AddWithValue("@SC_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());

                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new WarehouseStockDetailsData
                            {
                                Years = Convert.IsDBNull(sdr["Years"]) ? 0 : Convert.ToInt32(sdr["Years"]),
                                Months = Convert.IsDBNull(sdr["Months"]) ? 0 : Convert.ToInt32(sdr["Months"]),
                                SC_ItemCode = Convert.IsDBNull(sdr["SC_ItemCode"]) ? string.Empty : sdr["SC_ItemCode"].ToString(),
                                SC_TotalReceivedQty = Convert.IsDBNull(sdr["SC_TotalReceivedQty"]) ? 0 : Convert.ToDecimal(sdr["SC_TotalReceivedQty"]),
                                SC_TotalReleasedQty = Convert.IsDBNull(sdr["SC_TotalReleasedQty"]) ? 0 : Convert.ToDecimal(sdr["SC_TotalReleasedQty"]),
                                ClosingStock = Convert.IsDBNull(sdr["ClosingStock"]) ? 0 : Convert.ToDecimal(sdr["ClosingStock"]),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }


        public List<ProjectMasterData> ProjectMasterDetailsGet()
        {
            List<ProjectMasterData> items = new List<ProjectMasterData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * from ProjectMaster ORDER BY CONVERT(datetime, CreatedDateTime, 120) DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    //cmd.Parameters.AddWithValue("@ItemIsActive", 1);
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new ProjectMasterData
                            {
                                ProjectID = Convert.IsDBNull(sdr["ProjectID"]) ? 0 : Convert.ToInt32(sdr["ProjectID"]),
                                MasterCompanyId = Convert.IsDBNull(sdr["MasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["MasterCompanyId"]),
                                CreatedEmployeeId = Convert.IsDBNull(sdr["CreatedEmployeeId"]) ? 0 : Convert.ToInt32(sdr["CreatedEmployeeId"]),
                                ProjectName = Convert.IsDBNull(sdr["ProjectName"]) ? string.Empty : sdr["ProjectName"].ToString(),
                                Status = Convert.IsDBNull(sdr["Status"]) ? string.Empty : sdr["Status"].ToString(),
                                NDTPercent = Convert.IsDBNull(sdr["NDTPercent"]) ? 0 : Convert.ToDecimal(sdr["NDTPercent"]),
                                StartDate = Convert.IsDBNull(sdr["StartDate"]) ? DateTime.MinValue : Convert.ToDateTime(sdr["StartDate"]),
                                FormattedStartDate = Convert.IsDBNull(sdr["StartDate"]) ? string.Empty : Convert.ToDateTime(sdr["StartDate"]).ToString("dd/MM/yyyy"),
                                EndDate = Convert.IsDBNull(sdr["EndDate"]) ? DateTime.MinValue : Convert.ToDateTime(sdr["EndDate"]),
                                FormattedEndDate = Convert.IsDBNull(sdr["EndDate"]) ? string.Empty : Convert.ToDateTime(sdr["EndDate"]).ToString("dd/MM/yyyy"),
                                NewEndDate = Convert.IsDBNull(sdr["NewEndDate"]) ? DateTime.MinValue : Convert.ToDateTime(sdr["NewEndDate"]),
                                FormattedNewEndDate = Convert.IsDBNull(sdr["NewEndDate"]) ? string.Empty : Convert.ToDateTime(sdr["NewEndDate"]).ToString("dd/MM/yyyy"),
                                ProjectNumber = Convert.IsDBNull(sdr["ProjectNumber"]) ? string.Empty : sdr["ProjectNumber"].ToString(),
                                CreatedDateTime = Convert.IsDBNull(sdr["CreatedDateTime"]) ? string.Empty : sdr["CreatedDateTime"].ToString(),
                                UpdateDateTime = Convert.IsDBNull(sdr["UpdateDateTime"]) ? string.Empty : sdr["UpdateDateTime"].ToString()

                                //ProjectID = Convert.ToInt32(sdr["ProjectID"]),
                                //MasterCompanyId = Convert.ToInt32(sdr["MasterCompanyId"]),
                                //CreatedEmployeeId = Convert.ToInt32(sdr["CreatedEmployeeId"]),
                                //ProjectName = sdr["ProjectName"].ToString(),
                                //Status = sdr["Status"].ToString(),
                                //NDTPercent = Convert.ToDecimal(sdr["NDTPercent"]),
                                //StartDate = Convert.ToDateTime(sdr["StartDate"]),
                                //FormattedStartDate = Convert.ToDateTime(sdr["StartDate"]).ToString("dd/MM/yyyy"),
                                //EndDate = Convert.ToDateTime(sdr["EndDate"]),
                                //FormattedEndDate = Convert.ToDateTime(sdr["EndDate"]).ToString("dd/MM/yyyy"),
                                //NewEndDate = Convert.ToDateTime(sdr["NewEndDate"]),
                                //FormattedNewEndDate = Convert.ToDateTime(sdr["NewEndDate"]).ToString("dd/MM/yyyy"),
                                //ProjectNumber = sdr["ProjectNumber"].ToString(),
                                //CreatedDateTime = sdr["CreatedDateTime"].ToString(),
                                //UpdateDateTime = sdr["UpdateDateTime"].ToString()
                            });

                        }
                    }
                    con.Close();
                }
            }
            return items;
        }


        public List<ProjectMasterData> ProjectMasterDetailsGetPaged(int pageIndex, int pageSize, int companyId, string searchTerm, DateTime? startDate, DateTime? endDate, out int totalRecords)
        {
            List<ProjectMasterData> items = new List<ProjectMasterData>();
            totalRecords = 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string countQuery = @"SELECT COUNT(*) 
                           FROM ProjectMaster 
                           WHERE MasterCompanyId = @CompanyId
                           AND (ProjectName LIKE @Search OR ProjectNumber LIKE @Search)
                           AND (@StartDate IS NULL OR CONVERT(datetime, CreatedDateTime, 120) >= @StartDate)
                           AND (@EndDate IS NULL OR CONVERT(datetime, CreatedDateTime, 120) <= @EndDate)";

                string dataQuery = $@"SELECT * 
                           FROM ProjectMaster 
                           WHERE MasterCompanyId = @CompanyId
                           AND (ProjectName LIKE @Search OR ProjectNumber LIKE @Search)
                           AND (@StartDate IS NULL OR CONVERT(datetime, CreatedDateTime, 120) >= @StartDate)
                           AND (@EndDate IS NULL OR CONVERT(datetime, CreatedDateTime, 120) <= @EndDate)
                           ORDER BY CONVERT(datetime, CreatedDateTime, 120) DESC
                           OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                using (SqlCommand countCmd = new SqlCommand(countQuery, con))
                using (SqlCommand dataCmd = new SqlCommand(dataQuery, con))
                {
                    string likeSearch = "%" + searchTerm + "%";

                    // Common parameters
                    countCmd.Parameters.AddWithValue("@Search", likeSearch);
                    countCmd.Parameters.AddWithValue("@CompanyId", companyId);

                    dataCmd.Parameters.AddWithValue("@Search", likeSearch);
                    dataCmd.Parameters.AddWithValue("@CompanyId", companyId);
                    dataCmd.Parameters.AddWithValue("@Offset", (pageIndex - 1) * pageSize);
                    dataCmd.Parameters.AddWithValue("@PageSize", pageSize);

                    // Date parameters (handle nulls)
                    countCmd.Parameters.AddWithValue("@StartDate", startDate ?? (object)DBNull.Value);
                    countCmd.Parameters.AddWithValue("@EndDate", endDate ?? (object)DBNull.Value);

                    dataCmd.Parameters.AddWithValue("@StartDate", startDate ?? (object)DBNull.Value);
                    dataCmd.Parameters.AddWithValue("@EndDate", endDate ?? (object)DBNull.Value);

                    con.Open();
                    totalRecords = (int)countCmd.ExecuteScalar();

                    using (SqlDataReader sdr = dataCmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new ProjectMasterData
                            {
                                ProjectID = Convert.ToInt32(sdr["ProjectID"]),
                                ProjectNumber = sdr["ProjectNumber"].ToString(),
                                ProjectName = sdr["ProjectName"].ToString(),
                                Status = sdr["Status"].ToString(),
                                StartDate = Convert.ToDateTime(sdr["StartDate"]),
                                FormattedStartDate = Convert.ToDateTime(sdr["StartDate"]).ToString("dd/MM/yyyy"),
                                EndDate = Convert.ToDateTime(sdr["EndDate"]),
                                FormattedEndDate = Convert.ToDateTime(sdr["EndDate"]).ToString("dd/MM/yyyy"),
                                NewEndDate = sdr["NewEndDate"] != DBNull.Value ? Convert.ToDateTime(sdr["NewEndDate"]) : DateTime.MinValue,
                                FormattedNewEndDate = sdr["NewEndDate"] != DBNull.Value ? Convert.ToDateTime(sdr["NewEndDate"]).ToString("dd/MM/yyyy") : "",
                                CreatedDateTime = sdr["CreatedDateTime"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }


        public List<PoMasterData> POMasterData()
        {
            List<PoMasterData> items = new List<PoMasterData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM PoMaster pm INNER JOIN InspectionReportStatus sm ON sm.StatusID = pm.status ORDER BY POMasterID DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new PoMasterData
                            {
                                POMasterID = Convert.IsDBNull(sdr["POMasterID"]) ? 0 : Convert.ToInt32(sdr["POMasterID"]),
                                CreatedDate = Convert.IsDBNull(sdr["CreatedDate"]) ? "" : Convert.ToDateTime(sdr["CreatedDate"]).ToString("dd/MM/yyyy"),
                                ReportStatus = sdr["ReportStatus"]?.ToString() ?? "",
                                PurchaseOrderNo = sdr["PurchaseOrderNo"]?.ToString() ?? "",
                                ProjectID = sdr["ProjectID"]?.ToString() ?? "",
                                ReportFileName = sdr["ReportFileName"]?.ToString() ?? "",
                                Revision = Convert.IsDBNull(sdr["Revision"]) ? 0 : Convert.ToInt32(sdr["Revision"]),
                                Type = sdr["Type"]?.ToString() ?? "",
                                Status = sdr["Status"]?.ToString() ?? "",
                                UserID = Convert.IsDBNull(sdr["UserID"]) ? 0 : Convert.ToInt32(sdr["UserID"]),
                                StoreKeeperID = Convert.IsDBNull(sdr["StoreKeeperID"]) ? 0 : Convert.ToInt32(sdr["StoreKeeperID"]),
                                QualityID = Convert.IsDBNull(sdr["QualityID"]) ? 0 : Convert.ToInt32(sdr["QualityID"]),
                                StatusID = Convert.IsDBNull(sdr["StatusID"]) ? 0 : Convert.ToInt32(sdr["StatusID"]),
                                StatusTitle = sdr["StatusName"]?.ToString() ?? "",

                                IGIR_Number = sdr["IGIR_Number"]?.ToString() ?? "",
                                IGIR_Series = Convert.IsDBNull(sdr["IGIR_Series"]) ? 0 : Convert.ToInt32(sdr["IGIR_Series"]),
                            });

                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<PoMasterData> POMasterDataPaged(int pageIndex, int pageSize, string searchTerm, out int totalRecords, bool isRole5 = false)
        {
            List<PoMasterData> items = new List<PoMasterData>();
            totalRecords = 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string filterCondition = isRole5 ? "AND pm.Status = '6'" : "";

                string countQuery = $@"SELECT COUNT(*) 
                                        FROM PoMaster pm
                                        INNER JOIN InspectionReportStatus sm ON sm.StatusID = pm.Status
                                        WHERE 
                                            (pm.PurchaseOrderNo LIKE @SearchTerm OR 
                                             pm.ProjectID LIKE @SearchTerm OR 
                                             sm.StatusName LIKE @SearchTerm)
                                            {filterCondition}";

                string dataQuery = $@"SELECT * 
                                        FROM PoMaster pm
                                        INNER JOIN InspectionReportStatus sm ON sm.StatusID = pm.Status
                                        WHERE 
                                            (pm.PurchaseOrderNo LIKE @SearchTerm OR 
                                             pm.ProjectID LIKE @SearchTerm OR 
                                             sm.StatusName LIKE @SearchTerm)
                                            {filterCondition}
                                        ORDER BY pm.POMasterID DESC
                                        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                using (SqlCommand countCmd = new SqlCommand(countQuery, con))
                using (SqlCommand dataCmd = new SqlCommand(dataQuery, con))
                {
                    string likeSearch = "%" + searchTerm + "%";

                    foreach (var cmd in new[] { countCmd, dataCmd })
                    {
                        cmd.Parameters.AddWithValue("@SearchTerm", likeSearch);
                    }

                    // Ensure OFFSET is never negative
                    int offset = Math.Max((pageIndex - 1) * pageSize, 0);
                    dataCmd.Parameters.AddWithValue("@Offset", offset);
                    dataCmd.Parameters.AddWithValue("@PageSize", pageSize);

                    con.Open();
                    totalRecords = (int)countCmd.ExecuteScalar();

                    using (SqlDataReader sdr = dataCmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new PoMasterData
                            {
                                POMasterID = Convert.IsDBNull(sdr["POMasterID"]) ? 0 : Convert.ToInt32(sdr["POMasterID"]),
                                CreatedDate = Convert.IsDBNull(sdr["CreatedDate"]) ? "" : Convert.ToDateTime(sdr["CreatedDate"]).ToString("dd/MM/yyyy"),
                                ReportStatus = sdr["ReportStatus"]?.ToString() ?? "",
                                PurchaseOrderNo = sdr["PurchaseOrderNo"]?.ToString() ?? "",
                                ProjectID = sdr["ProjectID"]?.ToString() ?? "",
                                ReportFileName = sdr["ReportFileName"]?.ToString() ?? "",
                                Revision = Convert.IsDBNull(sdr["Revision"]) ? 0 : Convert.ToInt32(sdr["Revision"]),
                                Type = sdr["Type"]?.ToString() ?? "",
                                Status = sdr["Status"]?.ToString() ?? "",
                                UserID = Convert.IsDBNull(sdr["UserID"]) ? 0 : Convert.ToInt32(sdr["UserID"]),
                                StoreKeeperID = Convert.IsDBNull(sdr["StoreKeeperID"]) ? 0 : Convert.ToInt32(sdr["StoreKeeperID"]),
                                QualityID = Convert.IsDBNull(sdr["QualityID"]) ? 0 : Convert.ToInt32(sdr["QualityID"]),
                                StatusID = Convert.IsDBNull(sdr["StatusID"]) ? 0 : Convert.ToInt32(sdr["StatusID"]),
                                StatusTitle = sdr["StatusName"]?.ToString() ?? "",
                                IGIR_Number = sdr["IGIR_Number"]?.ToString() ?? "",
                                IGIR_Series = Convert.IsDBNull(sdr["IGIR_Series"]) ? 0 : Convert.ToInt32(sdr["IGIR_Series"])
                            });
                        }
                    }
                }
            }

            return items;
        }

        public List<PoMasterData> POMasterIGIRDataPaged(int pageIndex, int pageSize, string searchTerm, out int totalRecords, bool isRole5 = false)
        {
            List<PoMasterData> items = new List<PoMasterData>();
            totalRecords = 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string filterCondition = isRole5 ? "AND pm.Status = '6'" : "";

                string countQuery = $@"SELECT COUNT(*) 
                                        FROM PoMaster pm
                                        INNER JOIN InspectionReportStatus sm ON sm.StatusID = pm.Status
                                        WHERE 
                                            (pm.PurchaseOrderNo LIKE @SearchTerm OR 
                                             pm.ProjectID LIKE @SearchTerm OR 
                                             sm.StatusName LIKE @SearchTerm) AND pm.Status > 1 
                                            {filterCondition}";

                string dataQuery = $@"SELECT * 
                                        FROM PoMaster pm
                                        INNER JOIN InspectionReportStatus sm ON sm.StatusID = pm.Status
                                        WHERE 
                                            (pm.PurchaseOrderNo LIKE @SearchTerm OR 
                                             pm.ProjectID LIKE @SearchTerm OR 
                                             sm.StatusName LIKE @SearchTerm) AND pm.Status > 1 
                                            {filterCondition}
                                        ORDER BY pm.POMasterID DESC
                                        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                using (SqlCommand countCmd = new SqlCommand(countQuery, con))
                using (SqlCommand dataCmd = new SqlCommand(dataQuery, con))
                {
                    string likeSearch = "%" + searchTerm + "%";

                    foreach (var cmd in new[] { countCmd, dataCmd })
                    {
                        cmd.Parameters.AddWithValue("@SearchTerm", likeSearch);
                    }

                    // Ensure OFFSET is never negative
                    int offset = Math.Max((pageIndex - 1) * pageSize, 0);
                    dataCmd.Parameters.AddWithValue("@Offset", offset);
                    dataCmd.Parameters.AddWithValue("@PageSize", pageSize);

                    con.Open();
                    totalRecords = (int)countCmd.ExecuteScalar();

                    using (SqlDataReader sdr = dataCmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new PoMasterData
                            {
                                POMasterID = Convert.IsDBNull(sdr["POMasterID"]) ? 0 : Convert.ToInt32(sdr["POMasterID"]),
                                CreatedDate = Convert.IsDBNull(sdr["CreatedDate"]) ? "" : Convert.ToDateTime(sdr["CreatedDate"]).ToString("dd/MM/yyyy"),
                                ReportStatus = sdr["ReportStatus"]?.ToString() ?? "",
                                PurchaseOrderNo = sdr["PurchaseOrderNo"]?.ToString() ?? "",
                                ProjectID = sdr["ProjectID"]?.ToString() ?? "",
                                ReportFileName = sdr["ReportFileName"]?.ToString() ?? "",
                                Revision = Convert.IsDBNull(sdr["Revision"]) ? 0 : Convert.ToInt32(sdr["Revision"]),
                                Type = sdr["Type"]?.ToString() ?? "",
                                Status = sdr["Status"]?.ToString() ?? "",
                                UserID = Convert.IsDBNull(sdr["UserID"]) ? 0 : Convert.ToInt32(sdr["UserID"]),
                                StoreKeeperID = Convert.IsDBNull(sdr["StoreKeeperID"]) ? 0 : Convert.ToInt32(sdr["StoreKeeperID"]),
                                QualityID = Convert.IsDBNull(sdr["QualityID"]) ? 0 : Convert.ToInt32(sdr["QualityID"]),
                                StatusID = Convert.IsDBNull(sdr["StatusID"]) ? 0 : Convert.ToInt32(sdr["StatusID"]),
                                StatusTitle = sdr["StatusName"]?.ToString() ?? "",
                                IGIR_Number = sdr["IGIR_Number"]?.ToString() ?? "",
                                IGIR_Series = Convert.IsDBNull(sdr["IGIR_Series"]) ? 0 : Convert.ToInt32(sdr["IGIR_Series"])
                            });
                        }
                    }
                }
            }

            return items;
        }

        public List<PoMasterData> POMasterDataPaged(int pageIndex, int pageSize, string searchTerm, DateTime? startDate, DateTime? endDate, out int totalRecords, bool isRole5 = false)
        {
            List<PoMasterData> items = new List<PoMasterData>();
            totalRecords = 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string filterCondition = isRole5 ? "AND pm.Status = '6'" : "";

                string baseWhere = $@"WHERE (pm.PurchaseOrderNo LIKE @SearchTerm OR 
                               pm.ProjectID LIKE @SearchTerm OR 
                               sm.StatusName LIKE @SearchTerm)
                               {filterCondition}";

                // Add date filters if provided
                if (startDate.HasValue)
                {
                    baseWhere += " AND CONVERT(date, pm.CreatedDate) >= @StartDate";
                }
                if (endDate.HasValue)
                {
                    baseWhere += " AND CONVERT(date, pm.CreatedDate) <= @EndDate";
                }

                string countQuery = $@"SELECT COUNT(*) 
                            FROM PoMaster pm
                            INNER JOIN InspectionReportStatus sm ON sm.StatusID = pm.Status
                            {baseWhere}";

                string dataQuery = $@"SELECT * 
                            FROM PoMaster pm
                            INNER JOIN InspectionReportStatus sm ON sm.StatusID = pm.Status
                            {baseWhere}
                            ORDER BY pm.POMasterID DESC
                            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                using (SqlCommand countCmd = new SqlCommand(countQuery, con))
                using (SqlCommand dataCmd = new SqlCommand(dataQuery, con))
                {
                    string likeSearch = "%" + searchTerm + "%";

                    foreach (var cmd in new[] { countCmd, dataCmd })
                    {
                        cmd.Parameters.AddWithValue("@SearchTerm", likeSearch);
                        if (startDate.HasValue)
                            cmd.Parameters.AddWithValue("@StartDate", startDate.Value);
                        if (endDate.HasValue)
                            cmd.Parameters.AddWithValue("@EndDate", endDate.Value);
                    }

                    // Ensure OFFSET is never negative
                    int offset = Math.Max((pageIndex - 1) * pageSize, 0);
                    dataCmd.Parameters.AddWithValue("@Offset", offset);
                    dataCmd.Parameters.AddWithValue("@PageSize", pageSize);

                    con.Open();
                    totalRecords = (int)countCmd.ExecuteScalar();

                    using (SqlDataReader sdr = dataCmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new PoMasterData
                            {
                                POMasterID = Convert.IsDBNull(sdr["POMasterID"]) ? 0 : Convert.ToInt32(sdr["POMasterID"]),
                                CreatedDate = Convert.IsDBNull(sdr["CreatedDate"]) ? "" : Convert.ToDateTime(sdr["CreatedDate"]).ToString("dd/MM/yyyy"),
                                ReportStatus = sdr["ReportStatus"]?.ToString() ?? "",
                                PurchaseOrderNo = sdr["PurchaseOrderNo"]?.ToString() ?? "",
                                ProjectID = sdr["ProjectID"]?.ToString() ?? "",
                                ReportFileName = sdr["ReportFileName"]?.ToString() ?? "",
                                Revision = Convert.IsDBNull(sdr["Revision"]) ? 0 : Convert.ToInt32(sdr["Revision"]),
                                Type = sdr["Type"]?.ToString() ?? "",
                                Status = sdr["Status"]?.ToString() ?? "",
                                UserID = Convert.IsDBNull(sdr["UserID"]) ? 0 : Convert.ToInt32(sdr["UserID"]),
                                StoreKeeperID = Convert.IsDBNull(sdr["StoreKeeperID"]) ? 0 : Convert.ToInt32(sdr["StoreKeeperID"]),
                                QualityID = Convert.IsDBNull(sdr["QualityID"]) ? 0 : Convert.ToInt32(sdr["QualityID"]),
                                StatusID = Convert.IsDBNull(sdr["StatusID"]) ? 0 : Convert.ToInt32(sdr["StatusID"]),
                                StatusTitle = sdr["StatusName"]?.ToString() ?? "",
                                IGIR_Number = sdr["IGIR_Number"]?.ToString() ?? "",
                                IGIR_Series = Convert.IsDBNull(sdr["IGIR_Series"]) ? 0 : Convert.ToInt32(sdr["IGIR_Series"])
                            });
                        }
                    }
                }
            }

            return items;
        }

        public List<PoMasterData> POMasterIGIRDataPaged(int pageIndex, int pageSize, string searchTerm, DateTime? startDate, DateTime? endDate, out int totalRecords, bool isRole5 = false)
        {
            List<PoMasterData> items = new List<PoMasterData>();
            totalRecords = 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string filterCondition = isRole5 ? "AND pm.Status = '6'" : "";

                string baseWhere = $@"WHERE (pm.PurchaseOrderNo LIKE @SearchTerm OR 
                               pm.ProjectID LIKE @SearchTerm OR 
                               sm.StatusName LIKE @SearchTerm) 
                               AND pm.Status > 1 
                               {filterCondition}";

                // Add date filters if provided
                if (startDate.HasValue)
                {
                    baseWhere += " AND CONVERT(date, pm.CreatedDate) >= @StartDate";
                }
                if (endDate.HasValue)
                {
                    baseWhere += " AND CONVERT(date, pm.CreatedDate) <= @EndDate";
                }

                string countQuery = $@"SELECT COUNT(*) 
                            FROM PoMaster pm
                            INNER JOIN InspectionReportStatus sm ON sm.StatusID = pm.Status
                            {baseWhere}";

                string dataQuery = $@"SELECT * 
                            FROM PoMaster pm
                            INNER JOIN InspectionReportStatus sm ON sm.StatusID = pm.Status
                            {baseWhere}
                            ORDER BY pm.POMasterID DESC
                            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                using (SqlCommand countCmd = new SqlCommand(countQuery, con))
                using (SqlCommand dataCmd = new SqlCommand(dataQuery, con))
                {
                    string likeSearch = "%" + searchTerm + "%";

                    foreach (var cmd in new[] { countCmd, dataCmd })
                    {
                        cmd.Parameters.AddWithValue("@SearchTerm", likeSearch);
                        if (startDate.HasValue)
                            cmd.Parameters.AddWithValue("@StartDate", startDate.Value);
                        if (endDate.HasValue)
                            cmd.Parameters.AddWithValue("@EndDate", endDate.Value);
                    }

                    // Ensure OFFSET is never negative
                    int offset = Math.Max((pageIndex - 1) * pageSize, 0);
                    dataCmd.Parameters.AddWithValue("@Offset", offset);
                    dataCmd.Parameters.AddWithValue("@PageSize", pageSize);

                    con.Open();
                    totalRecords = (int)countCmd.ExecuteScalar();

                    using (SqlDataReader sdr = dataCmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new PoMasterData
                            {
                                POMasterID = Convert.IsDBNull(sdr["POMasterID"]) ? 0 : Convert.ToInt32(sdr["POMasterID"]),
                                CreatedDate = Convert.IsDBNull(sdr["CreatedDate"]) ? "" : Convert.ToDateTime(sdr["CreatedDate"]).ToString("dd/MM/yyyy"),
                                ReportStatus = sdr["ReportStatus"]?.ToString() ?? "",
                                PurchaseOrderNo = sdr["PurchaseOrderNo"]?.ToString() ?? "",
                                ProjectID = sdr["ProjectID"]?.ToString() ?? "",
                                ReportFileName = sdr["ReportFileName"]?.ToString() ?? "",
                                Revision = Convert.IsDBNull(sdr["Revision"]) ? 0 : Convert.ToInt32(sdr["Revision"]),
                                Type = sdr["Type"]?.ToString() ?? "",
                                Status = sdr["Status"]?.ToString() ?? "",
                                UserID = Convert.IsDBNull(sdr["UserID"]) ? 0 : Convert.ToInt32(sdr["UserID"]),
                                StoreKeeperID = Convert.IsDBNull(sdr["StoreKeeperID"]) ? 0 : Convert.ToInt32(sdr["StoreKeeperID"]),
                                QualityID = Convert.IsDBNull(sdr["QualityID"]) ? 0 : Convert.ToInt32(sdr["QualityID"]),
                                StatusID = Convert.IsDBNull(sdr["StatusID"]) ? 0 : Convert.ToInt32(sdr["StatusID"]),
                                StatusTitle = sdr["StatusName"]?.ToString() ?? "",
                                IGIR_Number = sdr["IGIR_Number"]?.ToString() ?? "",
                                IGIR_Series = Convert.IsDBNull(sdr["IGIR_Series"]) ? 0 : Convert.ToInt32(sdr["IGIR_Series"])
                            });
                        }
                    }
                }
            }

            return items;
        }

        public List<PoMasterData> IGIRData()
        {
            List<PoMasterData> items = new List<PoMasterData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM PoMaster pm INNER JOIN InspectionReportStatus sm ON sm.StatusID = pm.status WHERE IGIR_Number IS NOT NULL ORDER BY POMasterID DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new PoMasterData
                            {
                                POMasterID = Convert.IsDBNull(sdr["POMasterID"]) ? 0 : Convert.ToInt32(sdr["POMasterID"]),
                                CreatedDate = Convert.IsDBNull(sdr["CreatedDate"]) ? "" : Convert.ToDateTime(sdr["CreatedDate"]).ToString("dd/MM/yyyy"),
                                ReportStatus = sdr["ReportStatus"]?.ToString() ?? "",
                                PurchaseOrderNo = sdr["PurchaseOrderNo"]?.ToString() ?? "",
                                ProjectID = sdr["ProjectID"]?.ToString() ?? "",
                                ReportFileName = sdr["ReportFileName"]?.ToString() ?? "",
                                Revision = Convert.IsDBNull(sdr["Revision"]) ? 0 : Convert.ToInt32(sdr["Revision"]),
                                Type = sdr["Type"]?.ToString() ?? "",
                                Status = sdr["Status"]?.ToString() ?? "",
                                UserID = Convert.IsDBNull(sdr["UserID"]) ? 0 : Convert.ToInt32(sdr["UserID"]),
                                StoreKeeperID = Convert.IsDBNull(sdr["StoreKeeperID"]) ? 0 : Convert.ToInt32(sdr["StoreKeeperID"]),
                                QualityID = Convert.IsDBNull(sdr["QualityID"]) ? 0 : Convert.ToInt32(sdr["QualityID"]),
                                StatusID = Convert.IsDBNull(sdr["StatusID"]) ? 0 : Convert.ToInt32(sdr["StatusID"]),
                                StatusTitle = sdr["StatusName"]?.ToString() ?? "",

                                IGIR_Number = sdr["IGIR_Number"]?.ToString() ?? "",
                                IGIR_Series = Convert.IsDBNull(sdr["IGIR_Series"]) ? 0 : Convert.ToInt32(sdr["IGIR_Series"]),
                            });

                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<PoMasterData> IGIRDataDropDown()
        {
            List<PoMasterData> items = new List<PoMasterData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT DISTINCT pm.IGIR_Number,pm.Status FROM POMaster pm ORDER BY PM.IGIR_Number DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new PoMasterData
                            {
                                IGIR_Number = sdr["IGIR_Number"]?.ToString() ?? "",
                                Status = sdr["Status"]?.ToString() ?? "",
                            });

                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<PoMasterData> IGIRNumberShowInDropDown(string PONumber, string projectId)
        {
            List<PoMasterData> items = new List<PoMasterData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT *  FROM POMaster WHERE IGIR_Series = (SELECT MAX(IGIR_Series) FROM POMaster WHERE PurchaseOrderNo LIKE @PONumber) and ProjectID = @ProjectID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    //cmd.Parameters.AddWithValue("@PONumber", "%" + PONumber + "%");
                    cmd.Parameters.AddWithValue("@PONumber", PONumber);
                    cmd.Parameters.AddWithValue("@ProjectID", projectId);
                    con.Open();

                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new PoMasterData
                            {
                                POMasterID = sdr["POMasterID"] != DBNull.Value ? Convert.ToInt32(sdr["POMasterID"]) : 0,
                                CreatedDate = sdr["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(sdr["CreatedDate"]).ToString("dd/MM/yyyy") : "",
                                ReportStatus = sdr["ReportStatus"]?.ToString() ?? "",
                                PurchaseOrderNo = sdr["PurchaseOrderNo"]?.ToString() ?? "",
                                ProjectID = sdr["ProjectID"]?.ToString() ?? "",
                                ReportFileName = sdr["ReportFileName"]?.ToString() ?? "",
                                Revision = sdr["Revision"] != DBNull.Value ? Convert.ToInt32(sdr["Revision"]) : 0,
                                Type = sdr["Type"]?.ToString() ?? "",
                                Status = sdr["Status"]?.ToString() ?? "",
                                UserID = sdr["UserID"] != DBNull.Value ? Convert.ToInt32(sdr["UserID"]) : 0,
                                StoreKeeperID = sdr["StoreKeeperID"] != DBNull.Value ? Convert.ToInt32(sdr["StoreKeeperID"]) : 0,
                                QualityID = sdr["QualityID"] != DBNull.Value ? Convert.ToInt32(sdr["QualityID"]) : 0,
                                IGIR_Number = sdr["IGIR_Number"]?.ToString() ?? "",
                                IGIR_Series = sdr["IGIR_Series"] != DBNull.Value ? Convert.ToInt32(sdr["IGIR_Series"]) : 0,
                            });
                        }
                    }

                    con.Close();
                }
            }

            return items;
        }

        public List<PoMasterData> POMasterDataGet()
        {
            List<PoMasterData> items = new List<PoMasterData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM PoMaster pm INNER JOIN StatusMasterTB sm ON sm.StatusID = pm.status ORDER BY POMasterID DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new PoMasterData
                            {
                                POMasterID = Convert.IsDBNull(sdr["POMasterID"]) ? 0 : Convert.ToInt32(sdr["POMasterID"]),
                                CreatedDate = Convert.IsDBNull(sdr["CreatedDate"]) ? "" : Convert.ToDateTime(sdr["CreatedDate"]).ToString("dd/MM/yyyy"),
                                ReportStatus = sdr["ReportStatus"]?.ToString() ?? "",
                                PurchaseOrderNo = sdr["PurchaseOrderNo"]?.ToString() ?? "",
                                ProjectID = sdr["ProjectID"]?.ToString() ?? "",
                                ReportFileName = sdr["ReportFileName"]?.ToString() ?? "",
                                Revision = Convert.IsDBNull(sdr["Revision"]) ? 0 : Convert.ToInt32(sdr["Revision"]),
                                Type = sdr["Type"]?.ToString() ?? "",
                                Status = sdr["Status"]?.ToString() ?? "",
                                UserID = Convert.IsDBNull(sdr["UserID"]) ? 0 : Convert.ToInt32(sdr["UserID"]),
                                StoreKeeperID = Convert.IsDBNull(sdr["StoreKeeperID"]) ? 0 : Convert.ToInt32(sdr["StoreKeeperID"]),
                                QualityID = Convert.IsDBNull(sdr["QualityID"]) ? 0 : Convert.ToInt32(sdr["QualityID"]),
                                IGIR_Number = Convert.IsDBNull(sdr["IGIR_Number"]) ? "" : sdr["IGIR_Number"].ToString(),
                                IGIR_Series = Convert.IsDBNull(sdr["IGIR_Series"]) ? 0 : Convert.ToInt32(sdr["IGIR_Series"]),
                                StatusTitle = sdr["StatusTitle"]?.ToString() ?? "",
                            });

                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<IGIRData> onlyIGIRDetailsGet()
        {
            List<IGIRData> items = new List<IGIRData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM IGIR ORDER BY IGIRID DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new IGIRData
                            {
                                IGIRID = Convert.IsDBNull(sdr["IGIRID"]) ? 0 : Convert.ToInt32(sdr["IGIRID"]),
                                PurchaseOrderNo = Convert.IsDBNull(sdr["PurchaseOrderNo"]) ? "" : sdr["PurchaseOrderNo"].ToString(),
                                ProjectID = Convert.IsDBNull(sdr["ProjectID"]) ? "" : sdr["ProjectID"].ToString(),
                                VendorCode = Convert.IsDBNull(sdr["VendorCode"]) ? "" : sdr["VendorCode"].ToString(),
                                ApprovalStatus = Convert.IsDBNull(sdr["ApprovalStatus"]) ? "" : sdr["ApprovalStatus"].ToString(),
                                ItemNumber = Convert.IsDBNull(sdr["ItemNumber"]) ? "" : sdr["ItemNumber"].ToString(),
                                DeliveryDate = Convert.IsDBNull(sdr["DeliveryDate"]) ? "" : sdr["DeliveryDate"].ToString(),
                                VendorName = Convert.IsDBNull(sdr["VendorName"]) ? "" : sdr["VendorName"].ToString(),
                                ProductDescription = Convert.IsDBNull(sdr["ProductDescription"]) ? "" : sdr["ProductDescription"].ToString(),
                                Unit = Convert.IsDBNull(sdr["Unit"]) ? "" : sdr["Unit"].ToString(),
                                ProductType = Convert.IsDBNull(sdr["ProductType"]) ? "" : sdr["ProductType"].ToString(),
                                OrderedQuantity = Convert.IsDBNull(sdr["OrderedQuantity"]) ? 0 : Convert.ToDecimal(sdr["OrderedQuantity"]),
                                ReceivedQuantity = Convert.IsDBNull(sdr["ReceivedQuantity"]) ? 0 : Convert.ToDecimal(sdr["ReceivedQuantity"]),
                                DeliverRemainder = Convert.IsDBNull(sdr["DeliverRemainder"]) ? "" : sdr["DeliverRemainder"].ToString(),
                                UnitPrice = Convert.IsDBNull(sdr["UnitPrice"]) ? 0 : Convert.ToDecimal(sdr["UnitPrice"]),
                                Site = Convert.IsDBNull(sdr["Site"]) ? "" : sdr["Site"].ToString(),
                                Warehouse = Convert.IsDBNull(sdr["Warehouse"]) ? "" : sdr["Warehouse"].ToString(),
                                Location = Convert.IsDBNull(sdr["Location"]) ? "" : sdr["Location"].ToString(),
                                PurchaseRequisition = Convert.IsDBNull(sdr["PurchaseRequisition"]) ? "" : sdr["PurchaseRequisition"].ToString(),
                                PurchaseLineDateTime = Convert.IsDBNull(sdr["PurchaseLineDateTime"]) ? "" : sdr["PurchaseLineDateTime"].ToString(),
                                ProjectCategory = Convert.IsDBNull(sdr["ProjectCategory"]) ? "" : sdr["ProjectCategory"].ToString(),
                                Currency = Convert.IsDBNull(sdr["Currency"]) ? "" : sdr["Currency"].ToString(),
                                TermsOfPayment = Convert.IsDBNull(sdr["TermsOfPayment"]) ? "" : sdr["TermsOfPayment"].ToString(),
                                CMTR = Convert.IsDBNull(sdr["CMTR"]) ? "" : sdr["CMTR"].ToString(),
                                TDCNo = Convert.IsDBNull(sdr["TDCNo"]) ? "" : sdr["TDCNo"].ToString(),
                                MaterialReceived = Convert.IsDBNull(sdr["MaterialReceived"]) ? "" : sdr["MaterialReceived"].ToString(),
                                StockMaterial = Convert.IsDBNull(sdr["StockMaterial"]) ? "" : sdr["StockMaterial"].ToString(),
                                CertificateReceived = Convert.IsDBNull(sdr["CertificateReceived"]) ? "" : sdr["CertificateReceived"].ToString(),
                                ItemRemark = Convert.IsDBNull(sdr["ItemRemark"]) ? "" : sdr["ItemRemark"].ToString(),
                                IGIRRemark = Convert.IsDBNull(sdr["IGIRRemark"]) ? "" : sdr["IGIRRemark"].ToString(),
                                Marking = Convert.IsDBNull(sdr["Marking"]) ? "" : sdr["Marking"].ToString(),
                                NetAmount = Convert.IsDBNull(sdr["NetAmount"]) ? 0 : Convert.ToDecimal(sdr["NetAmount"]),
                                Created = Convert.IsDBNull(sdr["Created"]) ? "" : sdr["Created"].ToString(),
                                POMasterID = Convert.IsDBNull(sdr["POMasterID"]) ? (int?)null : Convert.ToInt32(sdr["POMasterID"]),
                                Project = Convert.IsDBNull(sdr["Project"]) ? "" : sdr["Project"].ToString(),
                                Status = Convert.IsDBNull(sdr["Status"]) ? "" : sdr["Status"].ToString(),
                                IGIRSeries = Convert.IsDBNull(sdr["IGIRSeries"]) ? "" : sdr["IGIRSeries"].ToString(),
                                VendorReferenceDO = Convert.IsDBNull(sdr["VendorReferenceDO"]) ? "" : sdr["VendorReferenceDO"].ToString(),
                                MaterialOrigin = Convert.IsDBNull(sdr["MaterialOrigin"]) ? "" : sdr["MaterialOrigin"].ToString(),
                                GeneralRemark = Convert.IsDBNull(sdr["GeneralRemark"]) ? "" : sdr["GeneralRemark"].ToString(),
                                IGIRRemarkQuality = Convert.IsDBNull(sdr["IGIRRemarkQuality"]) ? "" : sdr["IGIRRemarkQuality"].ToString(),
                                PastReceivedQuantity = Convert.IsDBNull(sdr["PastReceivedQuantity"]) ? 0 : Convert.ToDecimal(sdr["PastReceivedQuantity"]),

                            });

                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<IGIRData> POMasterIGIRDetailsGet()
        {
            List<IGIRData> items = new List<IGIRData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM POMaster pm INNER JOIN IGIR i ON i.POMasterID = pm.POMasterID ORDER BY PM.POMasterID DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new IGIRData
                            {
                                IGIRID = Convert.IsDBNull(sdr["IGIRID"]) ? 0 : Convert.ToInt32(sdr["IGIRID"]),
                                PurchaseOrderNo = Convert.IsDBNull(sdr["PurchaseOrderNo"]) ? "" : sdr["PurchaseOrderNo"].ToString(),
                                ProjectID = Convert.IsDBNull(sdr["ProjectID"]) ? "" : sdr["ProjectID"].ToString(),
                                VendorCode = Convert.IsDBNull(sdr["VendorCode"]) ? "" : sdr["VendorCode"].ToString(),
                                ApprovalStatus = Convert.IsDBNull(sdr["ApprovalStatus"]) ? "" : sdr["ApprovalStatus"].ToString(),
                                ItemNumber = Convert.IsDBNull(sdr["ItemNumber"]) ? "" : sdr["ItemNumber"].ToString(),
                                DeliveryDate = Convert.IsDBNull(sdr["DeliveryDate"]) ? "" : sdr["DeliveryDate"].ToString(),
                                VendorName = Convert.IsDBNull(sdr["VendorName"]) ? "" : sdr["VendorName"].ToString(),
                                ProductDescription = Convert.IsDBNull(sdr["ProductDescription"]) ? "" : sdr["ProductDescription"].ToString(),
                                Unit = Convert.IsDBNull(sdr["Unit"]) ? "" : sdr["Unit"].ToString(),
                                ProductType = Convert.IsDBNull(sdr["ProductType"]) ? "" : sdr["ProductType"].ToString(),
                                OrderedQuantity = Convert.IsDBNull(sdr["OrderedQuantity"]) ? 0 : Convert.ToDecimal(sdr["OrderedQuantity"]),
                                ReceivedQuantity = Convert.IsDBNull(sdr["ReceivedQuantity"]) ? 0 : Convert.ToDecimal(sdr["ReceivedQuantity"]),
                                DeliverRemainder = Convert.IsDBNull(sdr["DeliverRemainder"]) ? "" : sdr["DeliverRemainder"].ToString(),
                                UnitPrice = Convert.IsDBNull(sdr["UnitPrice"]) ? 0 : Convert.ToDecimal(sdr["UnitPrice"]),
                                Site = Convert.IsDBNull(sdr["Site"]) ? "" : sdr["Site"].ToString(),
                                Warehouse = Convert.IsDBNull(sdr["Warehouse"]) ? "" : sdr["Warehouse"].ToString(),
                                Location = Convert.IsDBNull(sdr["Location"]) ? "" : sdr["Location"].ToString(),
                                PurchaseRequisition = Convert.IsDBNull(sdr["PurchaseRequisition"]) ? "" : sdr["PurchaseRequisition"].ToString(),
                                PurchaseLineDateTime = Convert.IsDBNull(sdr["PurchaseLineDateTime"]) ? "" : sdr["PurchaseLineDateTime"].ToString(),
                                ProjectCategory = Convert.IsDBNull(sdr["ProjectCategory"]) ? "" : sdr["ProjectCategory"].ToString(),
                                Currency = Convert.IsDBNull(sdr["Currency"]) ? "" : sdr["Currency"].ToString(),
                                TermsOfPayment = Convert.IsDBNull(sdr["TermsOfPayment"]) ? "" : sdr["TermsOfPayment"].ToString(),
                                CMTR = Convert.IsDBNull(sdr["CMTR"]) ? "" : sdr["CMTR"].ToString(),
                                TDCNo = Convert.IsDBNull(sdr["TDCNo"]) ? "" : sdr["TDCNo"].ToString(),
                                MaterialReceived = Convert.IsDBNull(sdr["MaterialReceived"]) ? "" : sdr["MaterialReceived"].ToString(),
                                StockMaterial = Convert.IsDBNull(sdr["StockMaterial"]) ? "" : sdr["StockMaterial"].ToString(),
                                CertificateReceived = Convert.IsDBNull(sdr["CertificateReceived"]) ? "" : sdr["CertificateReceived"].ToString(),
                                ItemRemark = Convert.IsDBNull(sdr["ItemRemark"]) ? "" : sdr["ItemRemark"].ToString(),
                                IGIRRemark = Convert.IsDBNull(sdr["IGIRRemark"]) ? "" : sdr["IGIRRemark"].ToString(),
                                Marking = Convert.IsDBNull(sdr["Marking"]) ? "" : sdr["Marking"].ToString(),
                                NetAmount = Convert.IsDBNull(sdr["NetAmount"]) ? 0 : Convert.ToDecimal(sdr["NetAmount"]),
                                Created = Convert.IsDBNull(sdr["Created"]) ? "" : sdr["Created"].ToString(),
                                POMasterID = Convert.IsDBNull(sdr["POMasterID"]) ? (int?)null : Convert.ToInt32(sdr["POMasterID"]),
                                Project = Convert.IsDBNull(sdr["Project"]) ? "" : sdr["Project"].ToString(),
                                Status = Convert.IsDBNull(sdr["Status"]) ? "" : sdr["Status"].ToString(),
                                IGIRSeries = Convert.IsDBNull(sdr["IGIRSeries"]) ? "" : sdr["IGIRSeries"].ToString(),
                                VendorReferenceDO = Convert.IsDBNull(sdr["VendorReferenceDO"]) ? "" : sdr["VendorReferenceDO"].ToString(),
                                MaterialOrigin = Convert.IsDBNull(sdr["MaterialOrigin"]) ? "" : sdr["MaterialOrigin"].ToString(),
                                GeneralRemark = Convert.IsDBNull(sdr["GeneralRemark"]) ? "" : sdr["GeneralRemark"].ToString(),
                                IGIRRemarkQuality = Convert.IsDBNull(sdr["IGIRRemarkQuality"]) ? "" : sdr["IGIRRemarkQuality"].ToString(),
                                PastReceivedQuantity = Convert.IsDBNull(sdr["PastReceivedQuantity"]) ? 0 : Convert.ToDecimal(sdr["PastReceivedQuantity"]),

                                IGIR_Number = Convert.IsDBNull(sdr["IGIR_Number"]) ? "" : sdr["IGIR_Number"].ToString(),
                                IGIR_Series = Convert.IsDBNull(sdr["IGIR_Series"]) ? 0 : Convert.ToInt32(sdr["IGIR_Series"]),

                                CreatedDate = Convert.IsDBNull(sdr["CreatedDate"]) ? "" : sdr["CreatedDate"].ToString(),
                                ReportStatus = Convert.IsDBNull(sdr["ReportStatus"]) ? "" : sdr["ReportStatus"].ToString(),
                                ReportFileName = Convert.IsDBNull(sdr["ReportFileName"]) ? "" : sdr["ReportFileName"].ToString(),
                                Revision = Convert.IsDBNull(sdr["Revision"]) ? (int?)null : Convert.ToInt32(sdr["Revision"]),
                                Type = Convert.IsDBNull(sdr["Type"]) ? "" : sdr["Type"].ToString(),
                                UserID = Convert.IsDBNull(sdr["UserID"]) ? 0 : Convert.ToInt32(sdr["UserID"]),
                                StoreKeeperID = Convert.IsDBNull(sdr["StoreKeeperID"]) ? (int?)null : Convert.ToInt32(sdr["StoreKeeperID"]),
                                QualityID = Convert.IsDBNull(sdr["QualityID"]) ? (int?)null : Convert.ToInt32(sdr["QualityID"]),
                            });

                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<IGIRData> IGIRthrowDataShow()
        {
            List<IGIRData> items = new List<IGIRData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"select * from IGIR i
                                  inner join POMaster pm ON pm.POMasterID= i.POMasterID
                                  INNER JOIN ItemMaster im ON im.ItemCode = i.ItemNumber
                                  INNER JOIN InventoryMaster ivm ON ivm.InventoryItemId = im.ItemId
                                  INNER JOIN VendorDetailsMaster vds ON vds.VendorCode = i.VendorCode
                                  INNER JOIN UOM um ON um.UOMId = ivm.InventoryUOMId
                                  INNER JOIN VAT vt on vt.VATID = ivm.InventoryGSTId
                                  INNER JOIN RackTypeDetails rtd ON rtd.RackTypeId = ivm.InventoryRackTypeId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new IGIRData
                            {
                                IGIRID = Convert.IsDBNull(sdr["IGIRID"]) ? 0 : Convert.ToInt32(sdr["IGIRID"]),
                                PurchaseOrderNo = Convert.IsDBNull(sdr["PurchaseOrderNo"]) ? "" : sdr["PurchaseOrderNo"].ToString(),
                                ProjectID = Convert.IsDBNull(sdr["ProjectID"]) ? "" : sdr["ProjectID"].ToString(),
                                VendorCode = Convert.IsDBNull(sdr["VendorCode"]) ? "" : sdr["VendorCode"].ToString(),
                                ApprovalStatus = Convert.IsDBNull(sdr["ApprovalStatus"]) ? "" : sdr["ApprovalStatus"].ToString(),
                                ItemNumber = Convert.IsDBNull(sdr["ItemNumber"]) ? "" : sdr["ItemNumber"].ToString(),
                                DeliveryDate = Convert.IsDBNull(sdr["DeliveryDate"]) ? "" : sdr["DeliveryDate"].ToString(),
                                VendorName = Convert.IsDBNull(sdr["VendorName"]) ? "" : sdr["VendorName"].ToString(),
                                ProductDescription = Convert.IsDBNull(sdr["ProductDescription"]) ? "" : sdr["ProductDescription"].ToString(),
                                Unit = Convert.IsDBNull(sdr["Unit"]) ? "" : sdr["Unit"].ToString(),
                                ProductType = Convert.IsDBNull(sdr["ProductType"]) ? "" : sdr["ProductType"].ToString(),
                                OrderedQuantity = Convert.IsDBNull(sdr["OrderedQuantity"]) ? 0 : Convert.ToDecimal(sdr["OrderedQuantity"]),
                                ReceivedQuantity = Convert.IsDBNull(sdr["ReceivedQuantity"]) ? 0 : Convert.ToDecimal(sdr["ReceivedQuantity"]),
                                DeliverRemainder = Convert.IsDBNull(sdr["DeliverRemainder"]) ? "" : sdr["DeliverRemainder"].ToString(),
                                UnitPrice = Convert.IsDBNull(sdr["UnitPrice"]) ? 0 : Convert.ToDecimal(sdr["UnitPrice"]),
                                Site = Convert.IsDBNull(sdr["Site"]) ? "" : sdr["Site"].ToString(),
                                Warehouse = Convert.IsDBNull(sdr["Warehouse"]) ? "" : sdr["Warehouse"].ToString(),
                                Location = Convert.IsDBNull(sdr["Location"]) ? "" : sdr["Location"].ToString(),
                                PurchaseRequisition = Convert.IsDBNull(sdr["PurchaseRequisition"]) ? "" : sdr["PurchaseRequisition"].ToString(),
                                PurchaseLineDateTime = Convert.IsDBNull(sdr["PurchaseLineDateTime"]) ? "" : sdr["PurchaseLineDateTime"].ToString(),
                                ProjectCategory = Convert.IsDBNull(sdr["ProjectCategory"]) ? "" : sdr["ProjectCategory"].ToString(),
                                Currency = Convert.IsDBNull(sdr["Currency"]) ? "" : sdr["Currency"].ToString(),
                                TermsOfPayment = Convert.IsDBNull(sdr["TermsOfPayment"]) ? "" : sdr["TermsOfPayment"].ToString(),
                                CMTR = Convert.IsDBNull(sdr["CMTR"]) ? "" : sdr["CMTR"].ToString(),
                                TDCNo = Convert.IsDBNull(sdr["TDCNo"]) ? "" : sdr["TDCNo"].ToString(),
                                MaterialReceived = Convert.IsDBNull(sdr["MaterialReceived"]) ? "" : sdr["MaterialReceived"].ToString(),
                                StockMaterial = Convert.IsDBNull(sdr["StockMaterial"]) ? "" : sdr["StockMaterial"].ToString(),
                                CertificateReceived = Convert.IsDBNull(sdr["CertificateReceived"]) ? "" : sdr["CertificateReceived"].ToString(),
                                ItemRemark = Convert.IsDBNull(sdr["ItemRemark"]) ? "" : sdr["ItemRemark"].ToString(),
                                IGIRRemark = Convert.IsDBNull(sdr["IGIRRemark"]) ? "" : sdr["IGIRRemark"].ToString(),
                                Marking = Convert.IsDBNull(sdr["Marking"]) ? "" : sdr["Marking"].ToString(),
                                NetAmount = Convert.IsDBNull(sdr["NetAmount"]) ? 0 : Convert.ToDecimal(sdr["NetAmount"]),
                                Created = Convert.IsDBNull(sdr["Created"]) ? "" : sdr["Created"].ToString(),
                                POMasterID = Convert.IsDBNull(sdr["POMasterID"]) ? (int?)null : Convert.ToInt32(sdr["POMasterID"]),
                                Project = Convert.IsDBNull(sdr["Project"]) ? "" : sdr["Project"].ToString(),
                                Status = Convert.IsDBNull(sdr["Status"]) ? "" : sdr["Status"].ToString(),
                                IGIRSeries = Convert.IsDBNull(sdr["IGIRSeries"]) ? "" : sdr["IGIRSeries"].ToString(),
                                VendorReferenceDO = Convert.IsDBNull(sdr["VendorReferenceDO"]) ? "" : sdr["VendorReferenceDO"].ToString(),
                                MaterialOrigin = Convert.IsDBNull(sdr["MaterialOrigin"]) ? "" : sdr["MaterialOrigin"].ToString(),
                                GeneralRemark = Convert.IsDBNull(sdr["GeneralRemark"]) ? "" : sdr["GeneralRemark"].ToString(),
                                IGIRRemarkQuality = Convert.IsDBNull(sdr["IGIRRemarkQuality"]) ? "" : sdr["IGIRRemarkQuality"].ToString(),
                                PastReceivedQuantity = Convert.IsDBNull(sdr["PastReceivedQuantity"]) ? 0 : Convert.ToDecimal(sdr["PastReceivedQuantity"]),

                                IGIR_Number = Convert.IsDBNull(sdr["IGIR_Number"]) ? "" : sdr["IGIR_Number"].ToString(),
                                IGIR_Series = Convert.IsDBNull(sdr["IGIR_Series"]) ? 0 : Convert.ToInt32(sdr["IGIR_Series"]),

                                InventoryId = Convert.ToInt32(sdr["InventoryId"]),
                                InventoryCompanyUnitId = Convert.ToInt32(sdr["InventoryCompanyUnitId"]),
                                InventoryRackTypeId = Convert.ToInt32(sdr["InventoryRackTypeId"]),
                                InventoryItemCategoryId = Convert.ToInt32(sdr["InventoryItemCategoryId"]),
                                InventoryItemId = Convert.ToInt32(sdr["InventoryItemId"]),
                                InventoryUOMId = Convert.ToInt32(sdr["InventoryUOMId"]),
                                InventoryGSTId = Convert.ToInt32(sdr["InventoryGSTId"]),
                                InventoryCurrentStock = Convert.ToDecimal(sdr["InventoryCurrentStock"]),
                                InventoryMinStock = Convert.ToDecimal(sdr["InventoryMinStock"]),
                                InventoryCreatedEmployeeId = Convert.ToInt32(sdr["InventoryCreatedEmployeeId"]),
                                InventoryMasterCompanyId = Convert.ToInt32(sdr["InventoryMasterCompanyId"]),
                                InventoryItemLocationStore = sdr["InventoryItemLocationStore"].ToString(),
                                InventoryCreatedDateTime = sdr["InventoryCreatedDateTime"].ToString(),
                                InventoryUpdatedDateTime = sdr["InventoryUpdatedDateTime"].ToString(),

                                ItemId = Convert.ToInt32(sdr["ItemId"]),
                                ItemCategoryId = Convert.ToInt32(sdr["ItemCategoryId"]),
                                ItemUOMId = Convert.ToInt32(sdr["ItemUOMId"]),
                                ItemGSTId = Convert.ToInt32(sdr["ItemGSTId"]),
                                ItemCode = sdr["ItemCode"].ToString(),
                                ItemName = sdr["ItemName"].ToString(),
                                ItemDescription = sdr["ItemDescription"].ToString(),
                                ItemHSNCode = sdr["ItemHSNCode"].ToString(),
                                ItemCreatedEmployeeId = Convert.ToInt32(sdr["ItemCreatedEmployeeId"]),
                                ItemMasterCompanyId = Convert.ToInt32(sdr["ItemMasterCompanyId"]),
                                ItemCreatedDateTime = sdr["ItemCreatedDateTime"].ToString(),
                                ItemUpdatedDateTime = sdr["ItemUpdatedDateTime"].ToString(),
                                ItemImageName = sdr["ItemImageName"].ToString(),

                                UOMId = Convert.ToInt32(sdr["UOMId"]),
                                UOMCode = sdr["UOMCode"].ToString(),
                                UOMName = sdr["UOMName"].ToString(),

                                VendorId = Convert.ToInt32(sdr["VendorId"]),
                                VendorEmail = sdr["VendorEmail"].ToString(),
                                VendorMobileNumber = sdr["VendorMobileNumber"].ToString(),
                                VendorCompanyName = sdr["VendorCompanyName"].ToString(),
                                VendorCompanyAddress = sdr["VendorCompanyAddress"].ToString(),
                                VendorCompanyGSTNumber = sdr["VendorCompanyGSTNumber"].ToString(),
                                VendorCompanyPANNumber = sdr["VendorCompanyPANNumber"].ToString(),
                                VendorCountry = sdr["VendorCountry"].ToString(),
                                VendorState = sdr["VendorState"].ToString(),
                                VendorCity = sdr["VendorCity"].ToString(),
                                VendorPinCode = sdr["VendorPinCode"].ToString(),
                                VendorBusinessType = sdr["VendorBusinessType"].ToString(),
                                VendorCreatedDateTime = sdr["VendorCreatedDateTime"].ToString(),
                                VendorUpdatedDateTime = sdr["VendorUpdatedDateTime"].ToString(),
                                VendorIsActive = Convert.ToInt32(sdr["VendorIsActive"]),
                                VendorMasterCompanyId = Convert.ToInt32(sdr["VendorMasterCompanyId"]),
                                VendorCreatedEmployeeId = Convert.ToInt32(sdr["VendorCreatedEmployeeId"]),

                                VendorNameAndCompanyName = $"{sdr["VendorName"]} - {sdr["VendorCompanyName"]}",

                                VATID = Convert.ToInt32(sdr["VATID"]),
                                VATCode = sdr["VATCode"].ToString(),
                                Percentage = Convert.ToDecimal(sdr["Percentage"]),

                                RackTypeId = Convert.ToInt32(sdr["RackTypeId"]),
                                RackType = sdr["RackType"].ToString(),
                                RackMasterCompanyId = Convert.ToInt32(sdr["RackMasterCompanyId"]),
                                RackCreatedEmployeeId = Convert.ToInt32(sdr["RackCreatedEmployeeId"]),
                                RackCreatedDateTime = sdr["RackCreatedDateTime"].ToString(),
                                RackUpdatedDateTime = sdr["RackUpdatedDateTime"].ToString()
                            });

                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<ReturnMaterialMaster> GetReturnMaterialMaster()
        {
            List<ReturnMaterialMaster> items = new List<ReturnMaterialMaster>();

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT * FROM ReturnMaterialMaster where RM_MasterCompanyId = @RM_MasterCompanyId ORDER BY RM_ID DESC";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@RM_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());

                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new ReturnMaterialMaster
                            {
                                RM_ID = Convert.IsDBNull(sdr["RM_ID"]) ? 0 : Convert.ToInt32(sdr["RM_ID"]),
                                RM_ItemId = Convert.IsDBNull(sdr["RM_ItemId"]) ? 0 : Convert.ToInt32(sdr["RM_ItemId"]),
                                RM_ItemCode = Convert.IsDBNull(sdr["RM_ItemCode"]) ? string.Empty : sdr["RM_ItemCode"].ToString(),
                                RM_PoNumber = Convert.IsDBNull(sdr["RM_PoNumber"]) ? string.Empty : sdr["RM_PoNumber"].ToString(),
                                RM_ProjectCode = Convert.IsDBNull(sdr["RM_ProjectCode"]) ? string.Empty : sdr["RM_ProjectCode"].ToString(),
                                RM_PoOrderedQty = Convert.IsDBNull(sdr["RM_PoOrderedQty"]) ? 0 : Convert.ToDecimal(sdr["RM_PoOrderedQty"]),
                                RM_PoReceivedQty = Convert.IsDBNull(sdr["RM_PoReceivedQty"]) ? 0 : Convert.ToDecimal(sdr["RM_PoReceivedQty"]),
                                RM_PoGettingStock = Convert.IsDBNull(sdr["RM_PoGettingStock"]) ? 0 : Convert.ToDecimal(sdr["RM_PoGettingStock"]),
                                RM_PoAcceptQty = Convert.IsDBNull(sdr["RM_PoAcceptQty"]) ? 0 : Convert.ToDecimal(sdr["RM_PoAcceptQty"]),
                                RM_PoRejectQty = Convert.IsDBNull(sdr["RM_PoRejectQty"]) ? 0 : Convert.ToDecimal(sdr["RM_PoRejectQty"]),
                                RM_PoShortFall = Convert.IsDBNull(sdr["RM_PoShortFall"]) ? 0 : Convert.ToDecimal(sdr["RM_PoShortFall"]),
                                RM_PoBalanceQty = Convert.IsDBNull(sdr["RM_PoBalanceQty"]) ? 0 : Convert.ToDecimal(sdr["RM_PoBalanceQty"]),
                                RM_PoReturnQty = Convert.IsDBNull(sdr["RM_PoReturnQty"]) ? 0 : Convert.ToDecimal(sdr["RM_PoReturnQty"]),
                                RM_PreviousReturnQty = Convert.IsDBNull(sdr["RM_PreviousReturnQty"]) ? 0 : Convert.ToDecimal(sdr["RM_PreviousReturnQty"]),
                                RM_TotalReturnQty = Convert.IsDBNull(sdr["RM_TotalReturnQty"]) ? 0 : Convert.ToDecimal(sdr["RM_TotalReturnQty"]),
                                RM_PoBasicPrice = Convert.IsDBNull(sdr["RM_PoBasicPrice"]) ? 0 : Convert.ToDecimal(sdr["RM_PoBasicPrice"]),
                                RM_PoGSTPercentage = Convert.IsDBNull(sdr["RM_PoGSTPercentage"]) ? 0 : Convert.ToDecimal(sdr["RM_PoGSTPercentage"]),
                                RM_PoVendorCode = Convert.IsDBNull(sdr["RM_PoVendorCode"]) ? string.Empty : sdr["RM_PoVendorCode"].ToString(),
                                //RM_CreatedDateTime = Convert.IsDBNull(sdr["RM_CreatedDateTime"]) ? string.Empty : sdr["RM_CreatedDateTime"].ToString(dd/MM/yyy),
                                RM_CreatedDateTime = Convert.IsDBNull(sdr["RM_CreatedDateTime"]) ? string.Empty : Convert.ToDateTime(sdr["RM_CreatedDateTime"]).ToString("dd/MM/yyyy"),
                                RM_UpdatedDateTime = Convert.IsDBNull(sdr["RM_UpdatedDateTime"]) ? string.Empty : sdr["RM_UpdatedDateTime"].ToString(),
                                RM_CreatedEmployeeId = Convert.IsDBNull(sdr["RM_CreatedEmployeeId"]) ? 0 : Convert.ToInt32(sdr["RM_CreatedEmployeeId"]),
                                RM_MasterCompanyId = Convert.IsDBNull(sdr["RM_MasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["RM_MasterCompanyId"]),
                                RM_CompanyUnitId = Convert.IsDBNull(sdr["RM_CompanyUnitId"]) ? 0 : Convert.ToInt32(sdr["RM_CompanyUnitId"]),
                                RM_StatusId = Convert.IsDBNull(sdr["RM_StatusId"]) ? 0 : Convert.ToInt32(sdr["RM_StatusId"]),
                                RM_IsActive = Convert.IsDBNull(sdr["RM_IsActive"]) ? 0 : Convert.ToInt32(sdr["RM_IsActive"]),
                            });
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }

        public List<ReturnMaterialMaster> GetReturnMaterialMasterPaged(int pageIndex, int pageSize, int companyUnitId, string searchTerm, DateTime? startDate, DateTime? endDate, out int totalRecords)
        {
            List<ReturnMaterialMaster> items = new List<ReturnMaterialMaster>();
            totalRecords = 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string countQuery = @"SELECT COUNT(*) FROM ReturnMaterialMaster 
                                       WHERE RM_MasterCompanyId = @CompanyId 
                                       AND RM_CompanyUnitId = @CompanyUnitId
                                       AND (RM_PoNumber LIKE @Search OR RM_ItemCode LIKE @Search)
                                       AND (@StartDate IS NULL OR CONVERT(datetime, RM_CreatedDateTime, 120) >= @StartDate)
                                       AND (@EndDate IS NULL OR CONVERT(datetime, RM_CreatedDateTime, 120) <= @EndDate)";

                string dataQuery = $@"SELECT * FROM ReturnMaterialMaster 
                                      WHERE RM_MasterCompanyId = @CompanyId 
                                      AND RM_CompanyUnitId = @CompanyUnitId
                                      AND (RM_PoNumber LIKE @Search OR RM_ItemCode LIKE @Search)
                                      AND (@StartDate IS NULL OR CONVERT(datetime, RM_CreatedDateTime, 120) >= @StartDate)
                                      AND (@EndDate IS NULL OR CONVERT(datetime, RM_CreatedDateTime, 120) <= @EndDate)
                                      ORDER BY CONVERT(datetime, RM_CreatedDateTime, 120) DESC
                                      OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                using (SqlCommand countCmd = new SqlCommand(countQuery, con))
                using (SqlCommand dataCmd = new SqlCommand(dataQuery, con))
                {
                    string likeSearch = "%" + searchTerm + "%";
                    string companyId = HttpContext.Current.Session["MasterCompanyId"].ToString();

                    // Common parameters
                    countCmd.Parameters.AddWithValue("@Search", likeSearch);
                    countCmd.Parameters.AddWithValue("@CompanyId", companyId);
                    countCmd.Parameters.AddWithValue("@CompanyUnitId", companyUnitId);

                    dataCmd.Parameters.AddWithValue("@Search", likeSearch);
                    dataCmd.Parameters.AddWithValue("@CompanyId", companyId);
                    dataCmd.Parameters.AddWithValue("@CompanyUnitId", companyUnitId);
                    dataCmd.Parameters.AddWithValue("@Offset", (pageIndex - 1) * pageSize);
                    dataCmd.Parameters.AddWithValue("@PageSize", pageSize);

                    // Date parameters (handle nulls)
                    countCmd.Parameters.AddWithValue("@StartDate", startDate ?? (object)DBNull.Value);
                    countCmd.Parameters.AddWithValue("@EndDate", endDate ?? (object)DBNull.Value);

                    dataCmd.Parameters.AddWithValue("@StartDate", startDate ?? (object)DBNull.Value);
                    dataCmd.Parameters.AddWithValue("@EndDate", endDate ?? (object)DBNull.Value);

                    con.Open();
                    totalRecords = (int)countCmd.ExecuteScalar();

                    using (SqlDataReader sdr = dataCmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new ReturnMaterialMaster
                            {
                                RM_ID = Convert.IsDBNull(sdr["RM_ID"]) ? 0 : Convert.ToInt32(sdr["RM_ID"]),
                                RM_ItemId = Convert.IsDBNull(sdr["RM_ItemId"]) ? 0 : Convert.ToInt32(sdr["RM_ItemId"]),
                                RM_ItemCode = Convert.IsDBNull(sdr["RM_ItemCode"]) ? string.Empty : sdr["RM_ItemCode"].ToString(),
                                RM_PoNumber = Convert.IsDBNull(sdr["RM_PoNumber"]) ? string.Empty : sdr["RM_PoNumber"].ToString(),
                                RM_ProjectCode = Convert.IsDBNull(sdr["RM_ProjectCode"]) ? string.Empty : sdr["RM_ProjectCode"].ToString(),
                                RM_PoOrderedQty = Convert.IsDBNull(sdr["RM_PoOrderedQty"]) ? 0 : Convert.ToDecimal(sdr["RM_PoOrderedQty"]),
                                RM_PoReceivedQty = Convert.IsDBNull(sdr["RM_PoReceivedQty"]) ? 0 : Convert.ToDecimal(sdr["RM_PoReceivedQty"]),
                                RM_PoGettingStock = Convert.IsDBNull(sdr["RM_PoGettingStock"]) ? 0 : Convert.ToDecimal(sdr["RM_PoGettingStock"]),
                                RM_PoAcceptQty = Convert.IsDBNull(sdr["RM_PoAcceptQty"]) ? 0 : Convert.ToDecimal(sdr["RM_PoAcceptQty"]),
                                RM_PoRejectQty = Convert.IsDBNull(sdr["RM_PoRejectQty"]) ? 0 : Convert.ToDecimal(sdr["RM_PoRejectQty"]),
                                RM_PoShortFall = Convert.IsDBNull(sdr["RM_PoShortFall"]) ? 0 : Convert.ToDecimal(sdr["RM_PoShortFall"]),
                                RM_PoBalanceQty = Convert.IsDBNull(sdr["RM_PoBalanceQty"]) ? 0 : Convert.ToDecimal(sdr["RM_PoBalanceQty"]),
                                RM_PoReturnQty = Convert.IsDBNull(sdr["RM_PoReturnQty"]) ? 0 : Convert.ToDecimal(sdr["RM_PoReturnQty"]),
                                RM_PreviousReturnQty = Convert.IsDBNull(sdr["RM_PreviousReturnQty"]) ? 0 : Convert.ToDecimal(sdr["RM_PreviousReturnQty"]),
                                RM_TotalReturnQty = Convert.IsDBNull(sdr["RM_TotalReturnQty"]) ? 0 : Convert.ToDecimal(sdr["RM_TotalReturnQty"]),
                                RM_PoBasicPrice = Convert.IsDBNull(sdr["RM_PoBasicPrice"]) ? 0 : Convert.ToDecimal(sdr["RM_PoBasicPrice"]),
                                RM_PoGSTPercentage = Convert.IsDBNull(sdr["RM_PoGSTPercentage"]) ? 0 : Convert.ToDecimal(sdr["RM_PoGSTPercentage"]),
                                RM_PoVendorCode = Convert.IsDBNull(sdr["RM_PoVendorCode"]) ? string.Empty : sdr["RM_PoVendorCode"].ToString(),
                                RM_CreatedDateTime = Convert.IsDBNull(sdr["RM_CreatedDateTime"]) ? string.Empty : Convert.ToDateTime(sdr["RM_CreatedDateTime"]).ToString("dd/MM/yyyy"),
                                RM_UpdatedDateTime = Convert.IsDBNull(sdr["RM_UpdatedDateTime"]) ? string.Empty : sdr["RM_UpdatedDateTime"].ToString(),
                                RM_CreatedEmployeeId = Convert.IsDBNull(sdr["RM_CreatedEmployeeId"]) ? 0 : Convert.ToInt32(sdr["RM_CreatedEmployeeId"]),
                                RM_MasterCompanyId = Convert.IsDBNull(sdr["RM_MasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["RM_MasterCompanyId"]),
                                RM_CompanyUnitId = Convert.IsDBNull(sdr["RM_CompanyUnitId"]) ? 0 : Convert.ToInt32(sdr["RM_CompanyUnitId"]),
                                RM_StatusId = Convert.IsDBNull(sdr["RM_StatusId"]) ? 0 : Convert.ToInt32(sdr["RM_StatusId"]),
                                RM_IsActive = Convert.IsDBNull(sdr["RM_IsActive"]) ? 0 : Convert.ToInt32(sdr["RM_IsActive"]),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<ProjectMaterialReturnDetail> ProjectWiseMaterialReturnDetails()
        {
            List<ProjectMaterialReturnDetail> items = new List<ProjectMaterialReturnDetail>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT * FROM ProjectMaterialReturnDetails";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new ProjectMaterialReturnDetail
                            {
                                PMR_ID = Convert.IsDBNull(sdr["PMR_ID"]) ? 0 : Convert.ToInt32(sdr["PMR_ID"]),
                                PMR_ProjectId = Convert.IsDBNull(sdr["PMR_ProjectId"]) ? 0 : Convert.ToInt32(sdr["PMR_ProjectId"]),
                                PMR_RackSerialId = Convert.IsDBNull(sdr["PMR_RackSerialId"]) ? 0 : Convert.ToInt32(sdr["PMR_RackSerialId"]),
                                PMR_ProjectCode = sdr["PMR_ProjectCode"]?.ToString() ?? "",
                                PMR_ItemCode = sdr["PMR_ItemCode"]?.ToString() ?? "",
                                PMR_UOM = sdr["PMR_UOM"]?.ToString() ?? "",
                                PMR_RackType = sdr["PMR_RackType"]?.ToString() ?? "",
                                PMR_RackSerialNumber = sdr["PMR_RackSerialNumber"]?.ToString() ?? "",
                                PMR_ReturnStock = Convert.IsDBNull(sdr["PMR_ReturnStock"]) ? 0 : Convert.ToDecimal(sdr["PMR_ReturnStock"]),
                                PMR_BatchNumber = sdr["PMR_BatchNumber"]?.ToString() ?? "",

                                PMR_ExpiryDate = Convert.IsDBNull(sdr["PMR_ExpiryDate"]) || string.IsNullOrWhiteSpace(sdr["PMR_ExpiryDate"].ToString()) ? ""
                                : DateTime.TryParseExact(sdr["PMR_ExpiryDate"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate)
                                ? parsedDate.ToString("dd/MM/yyyy") : "Invalid Date",

                                PMR_CreatedDateTime = Convert.IsDBNull(sdr["PMR_CreatedDateTime"]) ? "" : Convert.ToDateTime(sdr["PMR_CreatedDateTime"]).ToString("dd/MM/yyyy HH:mm:ss"),
                                PMR_UpdatedDateTime = Convert.IsDBNull(sdr["PMR_UpdatedDateTime"]) ? "" : Convert.ToDateTime(sdr["PMR_UpdatedDateTime"]).ToString("dd/MM/yyyy"),
                                PMR_MasterCompanyId = Convert.IsDBNull(sdr["PMR_MasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["PMR_MasterCompanyId"]),
                                PMR_CompanyUnitId = Convert.IsDBNull(sdr["PMR_CompanyUnitId"]) ? 0 : Convert.ToInt32(sdr["PMR_CompanyUnitId"]),
                                PMR_CreatedEmployeeId = Convert.IsDBNull(sdr["PMR_CreatedEmployeeId"]) ? 0 : Convert.ToInt32(sdr["PMR_CreatedEmployeeId"]),
                                PMR_IsDelete = Convert.IsDBNull(sdr["PMR_IsDelete"]) ? 0 : Convert.ToInt32(sdr["PMR_IsDelete"]),

                                PMR_InventoryId = Convert.IsDBNull(sdr["PMR_InventoryId"]) ? 0 : Convert.ToInt32(sdr["PMR_InventoryId"]),
                                PMR_ItemStockId = Convert.IsDBNull(sdr["PMR_ItemStockId"]) ? 0 : Convert.ToInt32(sdr["PMR_ItemStockId"]),
                                PMR_wsdID = Convert.IsDBNull(sdr["PMR_wsdID"]) ? 0 : Convert.ToInt32(sdr["PMR_wsdID"]),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<MaterialTransactionData> ProjectWiseMaterialReceived()
        {
            List<MaterialTransactionData> items = new List<MaterialTransactionData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT MT_ProjectCode, MT_ItemCode, SUM(MT_ReceivedQty) AS [TotalReceivedQty],MT_EmployeeId, MT_CompanyUnitId, MT_MasterCompanyId  
                                 FROM MaterialTransactionMaster
                                 GROUP BY MT_ProjectCode, MT_ItemCode, MT_EmployeeId, MT_CompanyUnitId, MT_MasterCompanyId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new MaterialTransactionData
                            {
                                MT_ProjectCode = sdr["MT_ProjectCode"].ToString(),
                                MT_ItemCode = sdr["MT_ItemCode"].ToString(),
                                TotalReceivedQty = Convert.IsDBNull(sdr["TotalReceivedQty"]) ? 0 : Convert.ToDecimal(sdr["TotalReceivedQty"]),
                                MT_EmployeeId = Convert.IsDBNull(sdr["MT_EmployeeId"]) ? 0 : Convert.ToInt32(sdr["MT_EmployeeId"]),
                                MT_CompanyUnitId = Convert.IsDBNull(sdr["MT_CompanyUnitId"]) ? 0 : Convert.ToInt32(sdr["MT_CompanyUnitId"]),
                                MT_MasterCompanyId = Convert.IsDBNull(sdr["MT_MasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["MT_MasterCompanyId"]),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<ProjectMaterialReturnData> GetProjectMaterialReturnData()
        {
            List<ProjectMaterialReturnData> items = new List<ProjectMaterialReturnData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT 
                                    PMR_ProjectId, 
                                    PMR_ProjectCode, 
                                    PMR_ItemCode, 
                                    PMR_UOM, 
                                    PMR_RackType, 
                                    SUM(PMR_ReturnStock) AS TotalReturnStock,
                                    PMR_MasterCompanyId, 
                                    PMR_CompanyUnitId, 
                                    PMR_CreatedEmployeeId   
                                FROM ProjectMaterialReturnDetails
                                GROUP BY 
                                    PMR_ProjectId, 
                                    PMR_ProjectCode, 
                                    PMR_ItemCode, 
                                    PMR_UOM, 
                                    PMR_RackType, 
                                    PMR_MasterCompanyId, 
                                    PMR_CompanyUnitId, 
                                    PMR_CreatedEmployeeId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new ProjectMaterialReturnData
                            {
                                PMR_ProjectId = Convert.IsDBNull(sdr["PMR_ProjectId"]) ? 0 : Convert.ToInt32(sdr["PMR_ProjectId"]),
                                PMR_ProjectCode = sdr["PMR_ProjectCode"].ToString(),
                                PMR_ItemCode = sdr["PMR_ItemCode"].ToString(),
                                PMR_UOM = sdr["PMR_UOM"].ToString(),
                                PMR_RackType = sdr["PMR_RackType"].ToString(),
                                TotalReturnStock = Convert.IsDBNull(sdr["TotalReturnStock"]) ? 0 : Convert.ToDecimal(sdr["TotalReturnStock"]),
                                PMR_MasterCompanyId = Convert.IsDBNull(sdr["PMR_MasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["PMR_MasterCompanyId"]),
                                PMR_CompanyUnitId = Convert.IsDBNull(sdr["PMR_CompanyUnitId"]) ? 0 : Convert.ToInt32(sdr["PMR_CompanyUnitId"]),
                                PMR_CreatedEmployeeId = Convert.IsDBNull(sdr["PMR_CreatedEmployeeId"]) ? 0 : Convert.ToInt32(sdr["PMR_CreatedEmployeeId"])
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<ProjectItemSummaryData> GetProjectItemSummaryData()
        {
            List<ProjectItemSummaryData> items = new List<ProjectItemSummaryData>();
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT 
                                    t1.MT_ProjectCode AS ProjectCode,
                                    t1.MT_ItemCode AS ItemCode,
                                    t1.TotalReceivedQty AS ReceivedQty,
                                    COALESCE(t2.TotalReturnStock, 0) AS ReturnQty,
                                    (t1.TotalReceivedQty - COALESCE(t2.TotalReturnStock, 0)) AS UsedQty,
                                    t1.MT_CompanyUnitId,
                                    t1.MT_MasterCompanyId 
                                FROM 
                                    (SELECT 
                                        MT_ProjectCode, 
                                        MT_ItemCode, 
                                        SUM(MT_ReceivedQty) AS TotalReceivedQty,
                                        MT_EmployeeId, 
                                        MT_CompanyUnitId, 
                                        MT_MasterCompanyId
                                    FROM MaterialTransactionMaster
                                    GROUP BY MT_ProjectCode, MT_ItemCode, MT_EmployeeId, MT_CompanyUnitId, MT_MasterCompanyId) t1
                                LEFT JOIN 
                                    (SELECT 
                                        PMR_ProjectId, 
                                        PMR_ProjectCode, 
                                        PMR_ItemCode, 
                                        SUM(PMR_ReturnStock) AS TotalReturnStock,
                                        PMR_MasterCompanyId, 
                                        PMR_CompanyUnitId, 
                                        PMR_CreatedEmployeeId
                                    FROM ProjectMaterialReturnDetails
                                    GROUP BY 
                                        PMR_ProjectId, 
                                        PMR_ProjectCode, 
                                        PMR_ItemCode, 
                                        PMR_MasterCompanyId, 
                                        PMR_CompanyUnitId, 
                                        PMR_CreatedEmployeeId) t2
                                ON t1.MT_ProjectCode = t2.PMR_ProjectCode 
                                AND t1.MT_ItemCode = t2.PMR_ItemCode";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new ProjectItemSummaryData
                            {
                                ProjectCode = sdr["ProjectCode"].ToString(),
                                ItemCode = sdr["ItemCode"].ToString(),
                                ReceivedQty = Convert.IsDBNull(sdr["ReceivedQty"]) ? 0 : Convert.ToDecimal(sdr["ReceivedQty"]),
                                ReturnQty = Convert.IsDBNull(sdr["ReturnQty"]) ? 0 : Convert.ToDecimal(sdr["ReturnQty"]),
                                UsedQty = Convert.IsDBNull(sdr["UsedQty"]) ? 0 : Convert.ToDecimal(sdr["UsedQty"]),
                                CompanyUnitId = Convert.IsDBNull(sdr["MT_CompanyUnitId"]) ? 0 : Convert.ToInt32(sdr["MT_CompanyUnitId"]),
                                MasterCompanyId = Convert.IsDBNull(sdr["MT_MasterCompanyId"]) ? 0 : Convert.ToInt32(sdr["MT_MasterCompanyId"])
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }


















        public bool IsNotificationNameExist(string subjectName)
        {
            bool IsValid = false;
            string query = @"SELECT COUNT(*) FROM NotificationSetting WHERE N_Subject = @N_Subject";

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@N_Subject", subjectName);
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                con.Close();
                if (count > 0)
                {
                    IsValid = true;
                }
                return IsValid;
            }
        }

        public bool IsNotificationTypeNameExist(string typeName)
        {
            bool IsValid = false;
            string query = @"SELECT COUNT(*) FROM NotificationTypeSetting WHERE NT_Name = @NT_Name";

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@NT_Name", typeName);
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                con.Close();
                if (count > 0)
                {
                    IsValid = true;
                }
                return IsValid;
            }
        }

        public bool IsNotificationSubjectNameExist(string name)
        {
            bool IsValid = false;
            string query = @"SELECT COUNT(*) FROM NotificationSetting WHERE N_Subject = @N_Subject";

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@N_Subject", name);
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                con.Close();
                if (count > 0)
                {
                    IsValid = true;
                }
                return IsValid;
            }
        }

        public bool IsNotificationSave(string subjectName, string typeName)
        {
            bool IsValid = false;
            string query = @"SELECT COUNT(*) FROM NotificationSettingSave 
                              WHERE NS_EmployeeId = @NS_EmployeeId AND NS_SubjectName = @NS_SubjectName AND NS_TypeName = @NS_TypeName";

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@NS_TypeName", typeName);
                cmd.Parameters.AddWithValue("@NS_SubjectName", subjectName);
                cmd.Parameters.AddWithValue("@NS_EmployeeId", HttpContext.Current.Session["UserID"].ToString());
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                con.Close();
                if (count > 0)
                {
                    IsValid = true;
                }
                return IsValid;
            }
        }

        public bool IsNotificationSaveExist(string subjectName, string typeName, string employeeId)
        {
            bool IsValid = false;
            string query = @"SELECT COUNT(*) FROM NotificationSettingSave 
                             WHERE NS_EmployeeId = @NS_EmployeeId AND NS_SubjectName = @NS_SubjectName AND NS_TypeName = @NS_TypeName";

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@NS_TypeName", typeName);
                cmd.Parameters.AddWithValue("@NS_SubjectName", subjectName);
                cmd.Parameters.AddWithValue("@NS_EmployeeId", employeeId);
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                con.Close();
                if (count > 0)
                {
                    IsValid = true;
                }
                return IsValid;
            }
        }

        public bool IsNotificationAssign(string employeeId, string nID)
        {
            bool IsValid = false;
            string query = @"SELECT COUNT(*) FROM NotificationAssignMaster 
                             WHERE NA_ToEmployeeId = @NA_ToEmployeeId AND NA_NID = @NA_NID AND NA_MasterCompanyId = @NA_MasterCompanyId";

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@NA_ToEmployeeId", employeeId);
                cmd.Parameters.AddWithValue("@NA_NID", nID);
                cmd.Parameters.AddWithValue("@NA_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());

                con.Open();
                int count = (int)cmd.ExecuteScalar();
                con.Close();

                if (count > 0)
                {
                    IsValid = true;
                }
                return IsValid;
            }
        }
        /* Vendor Entry Check */
        public bool VendorIsExist(string vendorCode, string companyName)
        {
            bool IsValid = false;
            string query = @"SELECT COUNT(*) FROM VendorDetailsMaster 
                             WHERE (VendorCode = @VendorCode OR VendorCompanyName = @VendorCompanyName) 
                                  AND VendorMasterCompanyId = @VendorMasterCompanyId;";

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@VendorCode", vendorCode);
                cmd.Parameters.AddWithValue("@VendorCompanyName", companyName);
                cmd.Parameters.AddWithValue("@VendorIsActive", 1);
                cmd.Parameters.AddWithValue("@VendorMasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());

                con.Open();
                int count = (int)cmd.ExecuteScalar();
                con.Close();

                if (count > 0)
                {
                    IsValid = true;
                }
                return IsValid;
            }
        }

        /* WarehouseStockDetails Date wise entry count check */
        public bool IsDateExistForDataStoreOrRelease(string entryDate, string itemCode)
        {
            bool IsValid = false;
            string query = @"SELECT COUNT(*) FROM WarehouseStockDetails WHERE SC_EntryDate = @SC_EntryDate AND SC_ItemCode = @SC_ItemCode AND
                             SC_MasterCompanyId = @SC_MasterCompanyId ";

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@SC_MasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                cmd.Parameters.AddWithValue("@SC_ItemCode", itemCode);
                cmd.Parameters.AddWithValue("@SC_EntryDate", entryDate);

                con.Open();
                int count = (int)cmd.ExecuteScalar();
                con.Close();
                if (count > 0)
                {
                    IsValid = true;
                }
                return IsValid;
            }
        }

        public bool IsMaterialRequestDetailsExist(string MRNumber, string ItemCode)
        {
            bool IsValid = false;
            string query = "SELECT COUNT(*) FROM MaterialRequestDetails WHERE MR_NUMBER = @MR_NUMBER AND MR_ItemCode = @MR_ItemCode";
            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@MR_NUMBER", MRNumber);
                cmd.Parameters.AddWithValue("@MR_ItemCode", ItemCode);

                con.Open();
                int count = (int)cmd.ExecuteScalar();
                con.Close();
                if (count > 0)
                {
                    IsValid = true;
                }
                return IsValid;
            }
        }

        public bool IsStoreDataInRack(string grnNo, string rackSerialId)
        {
            bool IsValid = false;
            string query = "SELECT COUNT(*) FROM ItemStore WHERE IS_GRNNUMBER = @IS_GRNNUMBER AND IS_RackSerialNo = @IS_RackSerialNo";
            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@IS_GRNNUMBER", grnNo);
                cmd.Parameters.AddWithValue("@IS_RackSerialNo", Convert.ToInt32(rackSerialId));

                con.Open();
                int count = (int)cmd.ExecuteScalar();
                con.Close();
                if (count > 0)
                {
                    IsValid = true;
                }
                return IsValid;
            }
        }

        public bool PODetailsUploadIsExist(string poNumber, string itemCode, decimal orderedQty, string vendorCode)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    bool IsValid = false;
                    string query = @"SELECT COUNT(*) FROM PoExcelUploadMaster WHERE PO_Number = @PO_Number AND PO_ItemCode = @PO_ItemCode AND 
                                        PO_OrderedQty = @PO_OrderedQty AND PO_VendorCode = @PO_VendorCode AND PO_IsActive = @PO_IsActive";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@PO_Number", poNumber);
                        cmd.Parameters.AddWithValue("@PO_ItemCode", itemCode);
                        cmd.Parameters.AddWithValue("@PO_OrderedQty", orderedQty);
                        //cmd.Parameters.AddWithValue("@PO_BasicPrice", basicPrice);
                        cmd.Parameters.AddWithValue("@PO_VendorCode", vendorCode);
                        //cmd.Parameters.AddWithValue("@PO_OrderedDateTime", POorderedDateTime);
                        //cmd.Parameters.AddWithValue("@PO_GeneratedDateTime", POgeneratedDateTime);
                        cmd.Parameters.AddWithValue("@PO_IsActive", 1);


                        con.Open();
                        int count = (int)cmd.ExecuteScalar();
                        con.Close();
                        if (count > 0)
                        {
                            IsValid = true;
                        }
                        return IsValid;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool PoExcelUploadMasterCheckforNewIGIR(string poNumber)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    bool IsValid = false;
                    string query = @"select COUNT(*) from PoExcelUploadMaster where PO_Number = @PO_Number and (PO_StatusId = 3 or PO_StatusId = 7 or PO_StatusId = 17)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@PO_Number", poNumber);
                        con.Open();
                        int count = (int)cmd.ExecuteScalar();
                        con.Close();
                        if (count > 0)
                        {
                            IsValid = true;
                        }
                        return IsValid;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool PONumberExisted(string poNumber)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    bool IsValid = false;
                    string query = @"SELECT COUNT(*) FROM POMaster WHERE PurchaseOrderNo = @PO_Number";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@PO_Number", poNumber);
                        con.Open();
                        int count = (int)cmd.ExecuteScalar();
                        con.Close();
                        if (count > 0)
                        {
                            IsValid = true;
                        }
                        return IsValid;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool PONumberWiseIGIRExisted(string poNumber)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    bool IsValid = false;
                    string query = @"SELECT COUNT(*) FROM POMaster WHERE PurchaseOrderNo = @PO_Number";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@PO_Number", poNumber);
                        con.Open();
                        int count = (int)cmd.ExecuteScalar();
                        con.Close();
                        if (count > 0)
                        {
                            IsValid = true;
                        }
                        return IsValid;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool PODetailsUploadIsExist1(string poNumber, string itemCode, decimal orderedQty, decimal receivedQty, string deliveryDate, string vendorCode)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    bool IsValid = false;
                    string query = @"SELECT COUNT(*) FROM PoExcelUploadMaster WHERE PO_Number = @PO_Number AND PO_ItemCode = @PO_ItemCode AND 
                                        PO_OrderedQty = @PO_OrderedQty AND PO_VendorCode = @PO_VendorCode AND PO_IsActive = @PO_IsActive AND
                                        PO_ReceivedQty = @PO_ReceivedQty AND PO_DeliveryDateTime = @PO_DeliveryDateTime";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@PO_Number", poNumber);
                        cmd.Parameters.AddWithValue("@PO_ItemCode", itemCode);
                        cmd.Parameters.AddWithValue("@PO_OrderedQty", orderedQty);
                        //cmd.Parameters.AddWithValue("@PO_BasicPrice", basicPrice);
                        cmd.Parameters.AddWithValue("@PO_VendorCode", vendorCode);
                        cmd.Parameters.AddWithValue("@PO_ReceivedQty", receivedQty);
                        cmd.Parameters.AddWithValue("@PO_DeliveryDateTime", deliveryDate);
                        //cmd.Parameters.AddWithValue("@PO_OrderedDateTime", POorderedDateTime);
                        //cmd.Parameters.AddWithValue("@PO_GeneratedDateTime", POgeneratedDateTime);
                        cmd.Parameters.AddWithValue("@PO_IsActive", 1);


                        con.Open();
                        int count = (int)cmd.ExecuteScalar();
                        con.Close();
                        if (count > 0)
                        {
                            IsValid = true;
                        }
                        return IsValid;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool ItemInventoryExistUnitWise(int itemID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    bool IsValid = false;
                    string query = @"SELECT COUNT(*) FROM InventoryMaster 
                                     WHERE InventoryItemId = @InventoryItemId AND InventoryMasterCompanyId = @InventoryMasterCompanyId";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@InventoryItemId", itemID);
                        //cmd.Parameters.AddWithValue("@InventoryCompanyUnitId", UnitId);
                        cmd.Parameters.AddWithValue("@InventoryMasterCompanyId", HttpContext.Current.Session["MasterCompanyId"].ToString());
                        con.Open();
                        int count = (int)cmd.ExecuteScalar();
                        con.Close();
                        if (count > 0)
                        {
                            IsValid = true;
                        }
                        return IsValid;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /* data fetch 23,24 and store 22,23 get 23 exist...*/
        public bool ItemInventoryExist(string rackTypeId, string inventoryItemLocationStore)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    bool IsValid = false;
                    string query = @"SELECT COUNT(*) FROM InventoryMaster WHERE 
                                     EXISTS (SELECT 1 FROM STRING_SPLIT(@InventoryItemLocationStore, ',') AS StoreItems
                                             WHERE StoreItems.value IN (SELECT value FROM STRING_SPLIT(InventoryItemLocationStore, ',')))                                     
                                     AND InventoryRackTypeId = @InventoryRackTypeId";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@InventoryItemLocationStore", inventoryItemLocationStore);
                        cmd.Parameters.AddWithValue("@InventoryRackTypeId", rackTypeId);
                        con.Open();
                        int count = (int)cmd.ExecuteScalar();
                        con.Close();
                        if (count > 0)
                        {
                            IsValid = true;
                        }
                        return IsValid;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool IsRackNameExists(string rackType)
        {
            bool IsValid = false;
            string query = "SELECT COUNT(*) FROM RackTypeDetails WHERE RackType = @RackType";

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@RackType", rackType);
                //cmd.Parameters.AddWithValue("@RackCompanyUnitId", rackCompanyUnitId);
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                con.Close();
                if (count > 0)
                {
                    IsValid = true;
                }
                return IsValid;
            }
        }

        public bool RackNumberExists(int branchId, int branchUnitId, int rackTypeId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    bool IsValid = false;
                    string query = "SELECT COUNT(*) FROM RackNumberDetails WHERE BranchId = @BranchId AND CompanyUnitId = @CompanyUnitId AND RackTypeId = @RackTypeId";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@BranchId", branchId);
                        cmd.Parameters.AddWithValue("@CompanyUnitId", branchUnitId);
                        cmd.Parameters.AddWithValue("@RackTypeId", rackTypeId);
                        con.Open();
                        int count = (int)cmd.ExecuteScalar();
                        con.Close();

                        if (count > 0)
                        {
                            IsValid = true;
                        }
                        return IsValid;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool RackRowExists(int branchId, int branchUnitId, int rackTypeId, int rackNumberId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    bool IsValid = false;
                    string query = "SELECT COUNT(*) FROM RackRowDetails WHERE BranchId = @BranchId AND CompanyUnitId = @CompanyUnitId AND RackTypeId = @RackTypeId AND RackNumberId = @RackNumberId";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@BranchId", branchId);
                        cmd.Parameters.AddWithValue("@CompanyUnitId", branchUnitId);
                        cmd.Parameters.AddWithValue("@RackTypeId", rackTypeId);
                        cmd.Parameters.AddWithValue("@RackNumberId", rackNumberId);

                        con.Open();
                        int count = (int)cmd.ExecuteScalar();
                        con.Close();

                        if (count > 0)
                        {
                            IsValid = true;
                        }
                        return IsValid;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool RackRowSeriesExists(int branchId, int branchUnitId, int rackTypeId, int rackNumberId, int rackRowNumberId, int rackSerialNumberId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    bool IsValid = false;
                    string query = @"SELECT COUNT(*) FROM RackSerialDetails 
                                    WHERE BranchId = @BranchId AND CompanyUnitId = @CompanyUnitId AND RackTypeId = @RackTypeId AND 
                                    RackNumberId = @RackNumberId AND RackRowId = @RackRowId AND SerialNumberId = @SerialNumberId";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@BranchId", branchId);
                        cmd.Parameters.AddWithValue("@CompanyUnitId", branchUnitId);
                        cmd.Parameters.AddWithValue("@RackTypeId", rackTypeId);
                        cmd.Parameters.AddWithValue("@RackNumberId", rackNumberId);
                        cmd.Parameters.AddWithValue("@RackRowId", rackRowNumberId);
                        cmd.Parameters.AddWithValue("@SerialNumberId", rackSerialNumberId);

                        con.Open();
                        int count = (int)cmd.ExecuteScalar();
                        con.Close();

                        if (count > 0)
                        {
                            IsValid = true;
                        }
                        return IsValid;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool RackCheckRowSeriesExists(int branchId, int branchUnitId, int rackTypeId, int rackNumberId, int rackRowNumberId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    bool IsValid = false;
                    string query = @"SELECT COUNT(*) FROM RackSerialDetails 
                                    WHERE BranchId = @BranchId AND CompanyUnitId = @CompanyUnitId AND RackTypeId = @RackTypeId AND 
                                    RackNumberId = @RackNumberId AND RackRowId = @RackRowId";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@BranchId", branchId);
                        cmd.Parameters.AddWithValue("@CompanyUnitId", branchUnitId);
                        cmd.Parameters.AddWithValue("@RackTypeId", rackTypeId);
                        cmd.Parameters.AddWithValue("@RackNumberId", rackNumberId);
                        cmd.Parameters.AddWithValue("@RackRowId", rackRowNumberId);
                        //cmd.Parameters.AddWithValue("@SerialNumberId", rackSerialNumberId);

                        con.Open();
                        int count = (int)cmd.ExecuteScalar();
                        con.Close();

                        if (count > 0)
                        {
                            IsValid = true;
                        }
                        return IsValid;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool CheckRackRowSeriesExists(int branchId, int branchUnitId, int rackTypeId, int rackNumberId, int rackSerialNumberId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    bool IsValid = false;
                    string query = @"SELECT COUNT(*) FROM RackSerialDetails 
                                    WHERE BranchId = @BranchId AND CompanyUnitId = @CompanyUnitId AND RackTypeId = @RackTypeId AND 
                                    RackNumberId = @RackNumberId AND SerialNumberId = @SerialNumberId";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@BranchId", branchId);
                        cmd.Parameters.AddWithValue("@CompanyUnitId", branchUnitId);
                        cmd.Parameters.AddWithValue("@RackTypeId", rackTypeId);
                        cmd.Parameters.AddWithValue("@RackNumberId", rackNumberId);
                        //cmd.Parameters.AddWithValue("@RackRowId", rackRowNumberId);
                        cmd.Parameters.AddWithValue("@SerialNumberId", rackSerialNumberId);

                        con.Open();
                        int count = (int)cmd.ExecuteScalar();
                        con.Close();

                        if (count > 0)
                        {
                            IsValid = true;
                        }
                        return IsValid;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool CheckRackUnitAndSerialWiseExists(int rackTypeId, int rackSerialNumberId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    bool IsValid = false;
                    string query = @"SELECT COUNT(*) FROM RackSerialDetails WHERE
                                        RackTypeId = @RackTypeId AND 
                                        SerialNumberId = @SerialNumberId";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        //cmd.Parameters.AddWithValue("@CompanyUnitId", branchUnitId);
                        cmd.Parameters.AddWithValue("@RackTypeId", rackTypeId);
                        //cmd.Parameters.AddWithValue("@RackNumberId", rackNumberId);
                        //cmd.Parameters.AddWithValue("@RackRowId", rackRowNumberId);
                        cmd.Parameters.AddWithValue("@SerialNumberId", rackSerialNumberId);

                        con.Open();
                        int count = (int)cmd.ExecuteScalar();
                        con.Close();

                        if (count > 0)
                        {
                            IsValid = true;
                        }
                        return IsValid;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool IsCategoryNameExists(string categoryName)
        {
            bool IsValid = false;
            string query = "SELECT COUNT(*) FROM CategoryMaster WHERE CategoryName = @CategoryName";
            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@CategoryName", categoryName);
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                con.Close();
                if (count > 0)
                {
                    IsValid = true;
                }
                return IsValid;
            }
        }

        public bool ItemCodeExist(string itemCode)
        {
            bool IsValid = false;
            string query = "SELECT COUNT(*) FROM ItemMaster WHERE ItemCode = @ItemCode";
            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ItemCode", itemCode);
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                con.Close();
                if (count > 0)
                {
                    IsValid = true;
                }
                return IsValid;
            }
        }

        public bool ProjectNumberExist(string projectNumber)
        {
            bool IsValid = false;
            string query = "SELECT COUNT(*) FROM ProjectMaster WHERE ProjectNumber = @ProjectNumber";
            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ProjectNumber", projectNumber);
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                con.Close();
                if (count > 0)
                {
                    IsValid = true;
                }
                return IsValid;
            }
        }

        public bool IsSMTPUserNameExist(string username)
        {
            bool IsValid = false;
            string query = "SELECT COUNT(*) FROM SMTP_Master WHERE SMTP_USERNAME = @SMTP_USERNAME";
            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@SMTP_USERNAME", username);
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                con.Close();
                if (count > 0)
                {
                    IsValid = true;
                }
                return IsValid;
            }
        }

        public bool ProjectNumberWithItemCodeExist(string projectNumber, string itemCode)
        {
            bool IsValid = false;
            string query = "SELECT COUNT(*) FROM ProjectMaterialReturnDetails WHERE PMR_ProjectCode = @PMR_ProjectCode and PMR_ItemCode = @PMR_ItemCode ";
            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@PMR_ItemCode", itemCode);
                cmd.Parameters.AddWithValue("@PMR_ProjectCode", projectNumber);
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                con.Close();
                if (count > 0)
                {
                    IsValid = true;
                }
                return IsValid;
            }
        }

        public bool ProjectNameExist(string projectName)
        {
            bool IsValid = false;
            string query = "SELECT COUNT(*) FROM ProjectMaster WHERE ProjectName = @ProjectName";
            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ProjectName", projectName);
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                con.Close();
                if (count > 0)
                {
                    IsValid = true;
                }
                return IsValid;
            }
        }

        public bool CompanyBranchNameExists(string branchName)
        {
            bool IsValid = false;
            string query = "SELECT COUNT(*) FROM CompanyBranchMaster WHERE BranchName = @BranchName";
            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@BranchName", branchName);
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                con.Close();
                if (count > 0)
                {
                    IsValid = true;
                }
                return IsValid;
            }
        }

        public bool BranchUnitNameExists(string unitName)
        {
            bool IsValid = false;
            string query = "SELECT COUNT(*) FROM CompanyUnitMaster WHERE UnitName = @UnitName";
            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UnitName", unitName);
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                con.Close();
                if (count > 0)
                {
                    IsValid = true;
                }
                return IsValid;
            }
        }

        public int GetEmployeeCount(string masterCompanyID)
        {
            int userCount = 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT COUNT(*) FROM MasterEmployeeData WHERE MasterCompanyID = @MasterCompanyID AND AccountStatus = @AccountStatus";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@MasterCompanyID", masterCompanyID);
                    cmd.Parameters.AddWithValue("@AccountStatus", 1);
                    con.Open();
                    // Execute the query and get the result
                    object result = cmd.ExecuteScalar();
                    // Check if the result is not null and convert it to an integer
                    if (result != null && result != DBNull.Value)
                    {
                        userCount = Convert.ToInt32(result);
                    }
                    con.Close();
                }
            }

            return userCount;
        }

        public bool IsUserNumberCheckStatic(string masterCompanyID)
        {
            bool IsUserExist = false;
            string query = @"SELECT 
                                CASE 
                                    WHEN COUNT(*) > 9 THEN 'false' 
                                    ELSE 'true' 
                                END AS IsWithinLimit
                            FROM MasterEmployeeData";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@MasterCompanyID", masterCompanyID);
                cmd.Parameters.AddWithValue("@MaxEmp", 9);

                con.Open();
                object result = cmd.ExecuteScalar();
                con.Close();

                if (result != null && result != DBNull.Value)
                {
                    IsUserExist = bool.Parse(result.ToString());
                }
            }

            return IsUserExist;
        }

        public bool IsUserNumberCheck(string masterCompanyID)
        {
            bool IsUserExist = false;
            string query = @"SELECT CASE WHEN ((SELECT COUNT(*) AS Table1Count FROM MasterEmployeeData WHERE MasterCompanyID = 1 AND AccountStatus = 1) < 
                               (SELECT ul.LimitMaxNumber AS LimitMaxNumber FROM MasterCompanyData mcd
                              INNER JOIN AdminUserLimitSet ul ON ul.UserLimitSetId = mcd.NumberofUsersId
                              INNER JOIN LicenseValidityMaster lvm ON lvm.LicenseValidityId = mcd.LicenseValidityId
                              WHERE mcd.MasterCompanyID = 1 AND AccountStatus = 1))
                                THEN 'true'
                                ELSE 'false'
                                END AS Result;";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@MasterCompanyID", masterCompanyID);
                con.Open();
                object result = cmd.ExecuteScalar();
                con.Close();

                if (result != null && result != DBNull.Value)
                {
                    IsUserExist = bool.Parse(result.ToString());
                }
            }

            return IsUserExist;
        }

        public bool IsPageNameExist(string pageName)
        {
            bool IsPageExist = false;
            string query = "select * from AspxPageMasterTB where AspxPageName=@AspxPageName";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@AspxPageName", pageName);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (dt.Rows.Count > 0)
                {
                    IsPageExist = true;
                }
            }
            return IsPageExist;
        }

        public bool IsEmailExist(string email)
        {
            bool IsUserExist = false;
            string query = "select * from MasterCompanyData where EmailId=@Email";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (dt.Rows.Count > 0)
                {
                    IsUserExist = true;
                }
                else
                {
                    string query1 = "select * from MasterEmployeeData where EmailId=@Email";
                    using (SqlCommand cmd1 = new SqlCommand(query1, con))
                    {
                        cmd1.Parameters.AddWithValue("@Email", email);
                        SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
                        DataTable dt1 = new DataTable();
                        sda1.Fill(dt1);
                        con.Open();
                        int j = cmd1.ExecuteNonQuery();
                        con.Close();
                        if (dt1.Rows.Count > 0)
                        {
                            IsUserExist = true;
                        }
                    }

                }
            }
            return IsUserExist;
        }

        public bool IsMobileNumberExist(string mobileNo)
        {
            bool IsUserExist = false;
            string query = "select * from MasterEmployeeDetails where MobileNumber=@MobileNumber";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@MobileNumber", mobileNo);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (dt.Rows.Count > 0)
                {
                    IsUserExist = true;
                }
                else
                {
                    string query1 = "select * from MasterCompanyDetails where MobileNumber=@MobileNumber";
                    using (SqlCommand cmd1 = new SqlCommand(query1, con))
                    {
                        cmd1.Parameters.AddWithValue("@MobileNumber", mobileNo);
                        SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
                        DataTable dt1 = new DataTable();
                        sda1.Fill(dt1);
                        con.Open();
                        int j = cmd1.ExecuteNonQuery();
                        con.Close();
                        if (dt1.Rows.Count > 0)
                        {
                            IsUserExist = true;
                        }
                    }

                }
            }
            return IsUserExist;
        }

        public bool VerifyCompanyCurrentPassword(string currentPassword)
        {
            var encryptedPasswordCurrent = Base64Encode(currentPassword);
            con.Open();
            string selectPasswordQuery = "SELECT Password FROM MasterCompanyData WHERE EmailID = @EmailID";

            using (SqlCommand command = new SqlCommand(selectPasswordQuery, con))
            {
                var sessionEmail = HttpContext.Current.Session["EmailID"].ToString();
                command.Parameters.AddWithValue("@EmailID", sessionEmail);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string storedPasswordHash = reader.GetString(0);
                        return encryptedPasswordCurrent == storedPasswordHash;
                    }
                }
            }
            con.Close();
            return false;
        }

        public bool VerifyEmployeeCurrentPassword(string currentPassword)
        {
            var encryptedPasswordCurrent = Base64Encode(currentPassword);
            con.Open();
            string selectPasswordQuery = "SELECT Password FROM MasterEmployeeData WHERE EmailId = @EmailId";

            using (SqlCommand command = new SqlCommand(selectPasswordQuery, con))
            {
                var sessionEmail = HttpContext.Current.Session["EmailID"].ToString();
                command.Parameters.AddWithValue("@EmailId", sessionEmail);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string storedPasswordHash = reader.GetString(0);
                        return encryptedPasswordCurrent == storedPasswordHash;
                    }
                }
            }
            con.Close();
            return false;
        }

        public bool IsAccountActive(string emailID)
        {
            bool IsValid = false;
            string query = "select * from MasterEmployeeData where EmailId = @EmailID AND AccountStatus='" + 1 + "' ";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@EmailID", emailID);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (dt.Rows.Count > 0)
                {
                    IsValid = true;
                }
                else
                {
                    string query1 = "select * from MasterCompanyData where EmailId=@EmailID AND AccountStatus = '" + 1 + "' ";
                    using (SqlCommand cmd1 = new SqlCommand(query1, con))
                    {
                        cmd1.Parameters.AddWithValue("@EmailID", emailID);
                        SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
                        DataTable dt1 = new DataTable();
                        sda1.Fill(dt1);
                        con.Open();
                        int j = cmd1.ExecuteNonQuery();
                        con.Close();
                        if (dt1.Rows.Count > 0)
                        {
                            IsValid = true;
                        }
                    }
                }
            }
            return IsValid;
        }

        public bool IsLicenseRequestSent()
        {
            bool IsValid = false;
            string query = "SELECT * from MasterCompanyData WHERE MasterCompanyId=@MasterCompanyId  AND LicenseRenewRequest=@LicenseRenewRequest ";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                var sessionMasterCompanyId = HttpContext.Current.Session["MasterCompanyId"].ToString();
                cmd.Parameters.AddWithValue("@MasterCompanyId", sessionMasterCompanyId);
                cmd.Parameters.AddWithValue("@LicenseRenewRequest", 0);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (dt.Rows.Count > 0)
                {
                    IsValid = true;
                }
            }
            return IsValid;
        }

        public bool IsRoleNameExists(string roleName)
        {
            bool IsValid = false;
            string query = "SELECT COUNT(*) FROM RoleMasterTB WHERE RoleName = @RoleName";

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@RoleName", roleName);
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                con.Close();

                if (count > 0)
                {
                    IsValid = true;
                }
                return IsValid;

            }
        }







        public DataTable GetProjectDataforEmail(string projectId)
        {
            using (SqlConnection connection = new SqlConnection(constr))
            {
                string query = @"
                SELECT * FROM ProjectMaster pm
                INNER JOIN ProjectMasterEmailDetails pmed ON pmed.PD_ProjectId = pm.ProjectID
                INNER JOIN MasterEmployeeData med ON med.EmployeeId = pmed.PD_EmployeeId
                INNER JOIN MasterEmployeeDetails md ON md.EmployeeId = pmed.PD_EmployeeId
                INNER JOIN RoleMasterTB r ON r.RoleId = pmed.PD_RoleId
                where ProjectID = '" + projectId + "' ";

                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                return dataTable;
            }
        }

        public int GetMaxRMID(string poNumber, string itemCode)
        {
            int maxRMID = 0;

            string query = @"SELECT MAX(RM_ID) AS RMID FROM ReturnMaterialMaster WHERE RM_PoNumber = @RM_PoNumber AND RM_ItemCode = @RM_ItemCode AND RM_MasterCompanyId = @RM_MasterCompanyId";

            using (SqlConnection conn = new SqlConnection(constr))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        var sessionMasterCompanyId = HttpContext.Current.Session["MasterCompanyId"].ToString();
                        cmd.Parameters.AddWithValue("@RM_PoNumber", poNumber);
                        cmd.Parameters.AddWithValue("@RM_ItemCode", itemCode);
                        cmd.Parameters.AddWithValue("@RM_MasterCompanyId", sessionMasterCompanyId);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            if (dt.Rows.Count > 0 && dt.Rows[0]["RMID"] != DBNull.Value)
                            {
                                maxRMID = Convert.ToInt32(dt.Rows[0]["RMID"]);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error fetching RM_ID", ex);
                }
            }
            return maxRMID;
        }

        public string GetUniqueFileName(string fileName)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string extension = System.IO.Path.GetExtension(fileName);
            return $"{timestamp}_{Guid.NewGuid().ToString().Substring(0, 4)}{extension}";
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public List<PaymentRequisitionData> GetPaymentRequisitionFiltered(int pageIndex, int pageSize, string searchTerm, DateTime? startDate, DateTime? endDate, out int totalRecords, int choice )
        {
            List<PaymentRequisitionData> items = new List<PaymentRequisitionData>();
            totalRecords = 0;

            using (SqlConnection con = new SqlConnection(constr))
            {                
                string masterCompanyId = HttpContext.Current.Session["MasterCompanyId"].ToString();

                // Base query with optional search
                string baseQuery = @" from PaymentRequisitionmaster pm inner join PaymentRequisitionDetails pd on pm.PaymentRequisitionID = pd.PaymentRequisitionID inner join Employees e on e.ID = pm.RequisitionBy";

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    if (choice != 0)
                    {
                        baseQuery += @" WHERE pd.StatusID=" + choice + " and pm.RequisitionNumber LIKE @SearchTerm ";
                    }
                    else
                    {
                        baseQuery += @" WHERE pm.RequisitionNumber LIKE @SearchTerm ";
                    }
                }
                else
                {
                    if (choice != 0)
                    {
                        baseQuery += @" WHERE pd.StatusID=" + choice;
                    }
                }
                // Add date filters if provided
                if (startDate.HasValue)
                {
                    if (choice != 0)
                    {
                        baseQuery += " WHERE pd.StatusID=" + choice + " CONVERT(date, pm.RequisitionDate) >= @StartDate";
                    }
                    else
                    {
                        baseQuery += " WHERE CONVERT(date, pm.RequisitionDate) >= @StartDate";
                    }
                }
                if (endDate.HasValue)
                {
                    baseQuery += " AND CONVERT(date, pm.RequisitionDate) <= @EndDate";
                }

                // Query for total record count
                string countQuery = "SELECT COUNT(*) " + baseQuery;

                // Query for paginated data - NOTE THE ADDED SPACE BEFORE ORDER BY
                string dataQuery = @"SELECT pm.PaymentRequisitionID,RequisitionDate,e.EmployeeName as RequisitionBy,RequisitionNumber,RequisitionType,pd.StatusID,RequisitionSubType,TotalAmount " + baseQuery + @" group by pm.PaymentRequisitionID,RequisitionDate,EmployeeName,RequisitionNumber,RequisitionType,RequisitionSubType,StatusID,TotalAmount ORDER BY CONVERT(datetime, pm.RequisitionDate, 120) DESC
                          OFFSET @Offset ROWS
                          FETCH NEXT @PageSize ROWS ONLY";

                using (SqlCommand countCmd = new SqlCommand(countQuery, con))
                {
                    countCmd.Parameters.AddWithValue("@MasterCompanyId", masterCompanyId);
                    if (!string.IsNullOrEmpty(searchTerm))
                        countCmd.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");
                    if (startDate.HasValue)
                        countCmd.Parameters.AddWithValue("@StartDate", startDate.Value);
                    if (endDate.HasValue)
                        countCmd.Parameters.AddWithValue("@EndDate", endDate.Value);

                    con.Open();
                    totalRecords = Convert.ToInt32(countCmd.ExecuteScalar());
                    con.Close();
                }

                using (SqlCommand dataCmd = new SqlCommand(dataQuery, con))
                {
                    dataCmd.Parameters.AddWithValue("@Offset", (pageIndex - 1) * pageSize);
                    dataCmd.Parameters.AddWithValue("@PageSize", pageSize);
                    if (!string.IsNullOrEmpty(searchTerm))
                        dataCmd.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");
                    if (startDate.HasValue)
                        dataCmd.Parameters.AddWithValue("@StartDate", startDate.Value);
                    if (endDate.HasValue)
                        dataCmd.Parameters.AddWithValue("@EndDate", endDate.Value);

                    con.Open();
                    using (SqlDataReader sdr = dataCmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new PaymentRequisitionData
                            {
                                PaymentRequisitionID = Convert.ToInt32(sdr["PaymentRequisitionID"]),
                                RequisitionDate = Convert.ToDateTime(sdr["RequisitionDate"].ToString()),
                                RequisitionBy = sdr["RequisitionBy"].ToString(),
                                RequisitionNumber = sdr["RequisitionNumber"].ToString(),
                                RequisitionType = sdr["RequisitionType"].ToString(),
                                RequisitionSubType = sdr["RequisitionSubType"].ToString(),
                                RequisitionStatus = sdr["StatusID"] == DBNull.Value
                                    ? ""
                                    : sdr["StatusID"].ToString(),
                                TotalAmount = Convert.ToDecimal(sdr["TotalAmount"].ToString()),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }
        public List<PaymentRequisitionData> GetPaymentRequisitionFiltered(int pageIndex, int pageSize, string searchTerm, DateTime? startDate, DateTime? endDate, out int totalRecords, int RequestedBy, int choice)
        {
            List<PaymentRequisitionData> items = new List<PaymentRequisitionData>();
            totalRecords = 0;

            using (con)
            {
                string masterCompanyId = HttpContext.Current.Session["MasterCompanyId"].ToString();

                // Base query with optional search
                string baseQuery = "";
                if (choice != 0)
                {
                    baseQuery = @" from PaymentRequisitionmaster pm inner join PaymentRequisitionDetails pd on pm.PaymentRequisitionID = pd.PaymentRequisitionID inner join Employees e on e.ID = pm.RequisitionBy where pd.StatusID=" + choice + " and pm.RequisitionBy=" + RequestedBy;
                }
                else
                {
                    baseQuery = @" from PaymentRequisitionmaster pm inner join PaymentRequisitionDetails pd on pm.PaymentRequisitionID = pd.PaymentRequisitionID inner join Employees e on e.ID = pm.RequisitionBy where pm.RequisitionBy=" + RequestedBy;
                }

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    baseQuery += @" and pm.RequisitionNumber LIKE @SearchTerm ";
                }

                // Add date filters if provided
                if (startDate.HasValue)
                {
                    baseQuery += " and CONVERT(date, pm.RequisitionDate) >= @StartDate";
                }
                if (endDate.HasValue)
                {
                    baseQuery += " AND CONVERT(date, pm.RequisitionDate) <= @EndDate";
                }

                // Query for total record count
                string countQuery = "SELECT COUNT(*) " + baseQuery;

                // Query for paginated data - NOTE THE ADDED SPACE BEFORE ORDER BY
                string dataQuery = @"SELECT pm.PaymentRequisitionID,pd.statusID,RequisitionDate,e.EmployeeName as RequisitionBy,StatusID,RequisitionNumber,RequisitionType,RequisitionSubType,TotalAmount " + baseQuery + @" group by pm.PaymentRequisitionID,pm.RequisitionStatus,RequisitionDate,EmployeeName,StatusID,RequisitionNumber,RequisitionType,RequisitionSubType,TotalAmount ORDER BY CONVERT(datetime, pm.RequisitionDate, 120) DESC
                          OFFSET @Offset ROWS
                          FETCH NEXT @PageSize ROWS ONLY";

                using (SqlCommand countCmd = new SqlCommand(countQuery, con))
                {
                    countCmd.Parameters.AddWithValue("@MasterCompanyId", masterCompanyId);
                    if (!string.IsNullOrEmpty(searchTerm))
                        countCmd.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");
                    if (startDate.HasValue)
                        countCmd.Parameters.AddWithValue("@StartDate", startDate.Value);
                    if (endDate.HasValue)
                        countCmd.Parameters.AddWithValue("@EndDate", endDate.Value);

                    con.Open();
                    totalRecords = Convert.ToInt32(countCmd.ExecuteScalar());
                    con.Close();
                }

                using (SqlCommand dataCmd = new SqlCommand(dataQuery, con))
                {
                    dataCmd.Parameters.AddWithValue("@Offset", (pageIndex - 1) * pageSize);
                    dataCmd.Parameters.AddWithValue("@PageSize", pageSize);
                    if (!string.IsNullOrEmpty(searchTerm))
                        dataCmd.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");
                    if (startDate.HasValue)
                        dataCmd.Parameters.AddWithValue("@StartDate", startDate.Value);
                    if (endDate.HasValue)
                        dataCmd.Parameters.AddWithValue("@EndDate", endDate.Value);

                    con.Open();
                    using (SqlDataReader sdr = dataCmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new PaymentRequisitionData
                            {
                                PaymentRequisitionID = Convert.ToInt32(sdr["PaymentRequisitionID"]),
                                RequisitionDate = Convert.ToDateTime(sdr["RequisitionDate"].ToString()),
                                RequisitionBy = sdr["RequisitionBy"].ToString(),
                                RequisitionNumber = sdr["RequisitionNumber"].ToString(),
                                RequisitionType = sdr["RequisitionType"].ToString(),
                                RequisitionSubType = sdr["RequisitionSubType"].ToString(),
                                RequisitionStatus = sdr["StatusID"].ToString(),
                                TotalAmount = Convert.ToDecimal(sdr["TotalAmount"].ToString()),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        public List<PaymentRequisitionData> PaymentRequisitionGetPaged(int pageIndex, int pageSize, string searchTerm, DateTime? startDate, DateTime? endDate, out int totalRecords)
        {
            List<PaymentRequisitionData> items = new List<PaymentRequisitionData>();
            totalRecords = 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                string masterCompanyId = HttpContext.Current.Session["MasterCompanyId"].ToString();

                // Base query with optional search
                string baseQuery = @" FROM PaymentRequisitionmaster pm
                            inner join PaymentRequisitionDetails pd on pm.PaymentRequisitionID=pd.PaymentRequisitionID
							inner join Employees e on e.ID=tm.EmployeeID";

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    baseQuery += @" AND (tm.RequisitionNumber LIKE @SearchTerm)";
                }

                // Add date filters if provided
                if (startDate.HasValue)
                {
                    baseQuery += " AND CONVERT(date, tm.RequisitionDate) >= @StartDate";
                }
                if (endDate.HasValue)
                {
                    baseQuery += " AND CONVERT(date, tm.RequisitionDate) <= @EndDate";
                }

                // Query for total record count
                string countQuery = "SELECT COUNT(*) " + baseQuery;

                // Query for paginated data - NOTE THE ADDED SPACE BEFORE ORDER BY
                string dataQuery = @"SELECT PaymentRequisitionID,RequisitionDate,e.EmployeeName as RequisitionBy,StatusID,RequisitionNumber,RequisitionType,RequisitionSubType,TotalAmount  " + baseQuery + @" group by PaymentRequisitionID,RequisitionDate,EmployeeName,StatusID,RequisitionNumber,RequisitionType,RequisitionSubType,TotalAmount ORDER BY CONVERT(datetime, tm.RequisitionDate, 120) DESC
                  OFFSET @Offset ROWS
                  FETCH NEXT @PageSize ROWS ONLY";

                using (SqlCommand countCmd = new SqlCommand(countQuery, con))
                {
                    countCmd.Parameters.AddWithValue("@MasterCompanyId", masterCompanyId);
                    if (!string.IsNullOrEmpty(searchTerm))
                        countCmd.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");
                    if (startDate.HasValue)
                        countCmd.Parameters.AddWithValue("@StartDate", startDate.Value);
                    if (endDate.HasValue)
                        countCmd.Parameters.AddWithValue("@EndDate", endDate.Value);

                    con.Open();
                    totalRecords = Convert.ToInt32(countCmd.ExecuteScalar());
                    con.Close();
                }

                using (SqlCommand dataCmd = new SqlCommand(dataQuery, con))
                {
                    dataCmd.Parameters.AddWithValue("@Offset", (pageIndex - 1) * pageSize);
                    dataCmd.Parameters.AddWithValue("@PageSize", pageSize);
                    if (!string.IsNullOrEmpty(searchTerm))
                        dataCmd.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");
                    if (startDate.HasValue)
                        dataCmd.Parameters.AddWithValue("@StartDate", startDate.Value);
                    if (endDate.HasValue)
                        dataCmd.Parameters.AddWithValue("@EndDate", endDate.Value);

                    con.Open();
                    using (SqlDataReader sdr = dataCmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new PaymentRequisitionData
                            {
                                PaymentRequisitionID = Convert.ToInt32(sdr["PaymentRequisitionID"]),
                                RequisitionDate = Convert.ToDateTime(sdr["RequisitionDate"].ToString()),
                                RequisitionBy = sdr["RequisitionBy"].ToString(),
                                RequisitionNumber = sdr["RequisitionNumber"].ToString(),
                                RequisitionType = sdr["RequisitionType"].ToString(),
                                RequisitionSubType = sdr["RequisitionSubType"].ToString(),
                                RequisitionStatus = sdr["StatusID"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

    }
}