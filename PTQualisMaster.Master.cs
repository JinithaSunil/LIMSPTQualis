using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LIMS.Models;

namespace LIMS
{
    public partial class PTQualisMaster : System.Web.UI.MasterPage
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ToString());
        string constr = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
        DataAccessLayer dal = new DataAccessLayer();
        DAL.CM _cm = new DAL.CM();
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddDays(-1));
            Response.Cache.SetNoStore();
            try
            {
                if (Session["Email"] == null)
                {
                    Response.Redirect("Login.aspx");
                    Context.ApplicationInstance.CompleteRequest();
                }
                else
                {
                    LoadUserRoles();
                    if (!IsPostBack)
                    {
                        int userID =Convert.ToInt32(Session["UserID"]);
                        string str1 = @"SELECT * FROM Employees e
                                        INNER JOIN Users u ON u.employeeID = e.ID
                                        Where u.UserID ='" + userID + "' ";

                        SqlDataAdapter sda = new SqlDataAdapter(str1, con);
                        con.Open();
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            var cem = Session["Email"].ToString();
                            var fem = dt.Rows[0]["Email"].ToString();

                            if (cem != fem)
                            {
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert(' Session Expired!! '); </script>");
                                Response.Redirect("Login.aspx");
                            }

                            if (cem == fem)
                            {
                                if (Session["RoleID"] == null)
                                {
                                    Response.Redirect("Login.aspx");
                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Role Not Getting'); setTimeout(function() { window.location = 'Login.aspx'; }, 100); </script>");
                                }
                                else
                                {
                                    DynamicMenuRepeater.DataSource = FetchMenuItemsFromDatabase(Session["RoleID"].ToString());
                                    DynamicMenuRepeater.DataBind();

                                    List<SMTPDataModel> SMTPGetDataList = dal.SMTPDataListDisplay();
                                    if (SMTPGetDataList == null)
                                    {
                                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No active SMTP records found.'); </script>");
                                    }
                                    else
                                    {
                                        foreach (var item in SMTPGetDataList)
                                        {
                                            var port = item.SMTP_PORT;
                                            Session["SMTP_PORT"] = item.SMTP_PORT;
                                            Session["SMTP_NAME"] = item.SMTP_NAME;
                                            Session["SMTP_EMAIL"] = item.SMTP_EMAIL;
                                            Session["SMTP_PASSWORD"] = item.SMTP_PASSWORD;
                                            Session["SMTP_URL"] = item.SMTP_URL;
                                            Session["FROM_EMAIL"] = item.SMTP_EMAIL;
                                            Session["Project_URL"] = item.SMTP_URL;
                                            Session["WebsiteURL"] = item.WebsiteURL;
                                        }
                                    }


                                    lblUserName.Text = Session["EmployeeName"].ToString();
                                    lblUserEmail.Text = Session["Email"].ToString();

                                    //if (dt.Rows[0]["CompanyLogo"].ToString().Length > 1)
                                    //{
                                    //    var IMG1 = dt.Rows[0]["CompanyLogo"].ToString();
                                    //    imagePreview.ImageUrl = Session["WebsiteURL"].ToString() + "/UploadImages/" + IMG1;
                                    //    imagePreview1.ImageUrl = Session["WebsiteURL"].ToString() + "/UploadImages/" + IMG1;
                                    //}
                                    //else
                                    //{
                                    //    imagePreview.ImageUrl = "assets/images/DefaultProfilePic.png";
                                    //    imagePreview1.ImageUrl = "assets/images/logo-icon.png";
                                    //}
                                }
                            }
                        }
                        con.Close();
                        


                    }
                }
                base.OnInit(e);
            }
            catch (Exception ex)
            {
                if (!(ex is System.Threading.ThreadAbortException))
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Error: " + ex.Message.Replace("'", "\\'") + "'); </script>");
                }
            }
        }
        public List<Models.MenuItem> FetchMenuItemsFromDatabase(string roleId)
        {
            Dictionary<int, Models.MenuItem> menuMap = new Dictionary<int, Models.MenuItem>();

            using (SqlConnection connection = new SqlConnection(constr))
            {
                string query = @"SELECT * 
                         FROM Menus ms 
                         INNER JOIN IconTB itb ON itb.IconId = ms.MenuIcon";

                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                // -------- PASS 1: Load menus ----------
                while (reader.Read())
                {
                    string[] roleIds = reader["RoleId"].ToString().Split(',');

                    if (!roleIds.Contains(roleId))
                        continue;

                    int menuId = Convert.ToInt32(reader["MenuId"]);

                    if (!menuMap.ContainsKey(menuId))
                    {
                        menuMap[menuId] = new Models.MenuItem
                        {
                            MenuId = menuId,
                            ParentMenuId = Convert.ToInt32(reader["ParentMenuId"]),
                            Title = reader["Title"].ToString(),
                            Description = reader["Description"].ToString(),
                            Url = reader["Url"].ToString(),
                            RoleId = reader["RoleId"].ToString(),
                            MenuIcon = reader["MenuIcon"].ToString(),
                            Position = Convert.ToInt32(reader["Position"]),
                            IconId = Convert.ToInt32(reader["IconId"]),
                            IconName = reader["IconName"].ToString(),
                            IconClassName = reader["IconClassName"].ToString(),
                            OpenInNewTab = Convert.ToBoolean(reader["OpenInNewTab"]),
                            SubItems = new List<Models.MenuItem>()
                        };
                    }
                }
                reader.Close();
            }

            // -------- PASS 2: Build hierarchy ----------
            List<Models.MenuItem> menuItems = new List<Models.MenuItem>();

            foreach (var item in menuMap.Values)
            {
                if (item.ParentMenuId == 0)
                {
                    menuItems.Add(item);
                }
                else if (menuMap.ContainsKey(item.ParentMenuId))
                {
                    menuMap[item.ParentMenuId].SubItems.Add(item);
                }
            }

            // -------- Sorting ----------
            menuItems = menuItems.OrderBy(x => x.Position).ToList();
            foreach (var menu in menuItems)
            {
                menu.SubItems = menu.SubItems.OrderBy(x => x.Position).ToList();
            }

            return menuItems;
        }
        //public List<Models.MenuItem> FetchMenuItemsFromDatabase(string roleId)
        //{
        //    List<Models.MenuItem> menuItems = new List<Models.MenuItem>();

        //    using (SqlConnection connection = new SqlConnection(constr))
        //    {
        //        /*string query = "SELECT [MenuId], [ParentMenuId], [Title], [Description], [Url], [RoleId], [MenuIcon], [Position] FROM [Menus]";*/
        //        string query = "SELECT * FROM Menus ms INNER JOIN IconTB itb ON itb.IconId = ms.MenuIcon";
        //        SqlCommand command = new SqlCommand(query, connection);
        //        connection.Open();
        //        SqlDataReader reader = command.ExecuteReader();
        //        Dictionary<int, Models.MenuItem> menuMap = new Dictionary<int, Models.MenuItem>();
        //        while (reader.Read())
        //        {
        //            string[] roleIds = reader["RoleId"].ToString().Split(',');

        //            /* Check if the desired roleId exists in the roleIds list*/
        //            if (roleIds.Contains(roleId))
        //            {
        //                Models.MenuItem menuItem = new Models.MenuItem
        //                {
        //                    MenuId = Convert.ToInt32(reader["MenuId"]),
        //                    ParentMenuId = Convert.ToInt32(reader["ParentMenuId"]),
        //                    Title = reader["Title"].ToString(),
        //                    Description = reader["Description"].ToString(),
        //                    Url = reader["Url"].ToString(),
        //                    RoleId = reader["RoleId"].ToString(),
        //                    MenuIcon = reader["MenuIcon"].ToString(),
        //                    Position = Convert.ToInt32(reader["Position"]),

        //                    IconId = Convert.ToInt32(reader["IconId"]),
        //                    IconName = reader["IconName"].ToString(),
        //                    IconClassName = reader["IconClassName"].ToString(),

        //                    OpenInNewTab = Convert.ToBoolean(reader["OpenInNewTab"].ToString()),

        //                    SubItems = new List<Models.MenuItem>()
        //                };

        //                int menuId = menuItem.MenuId;
        //                int parentMenuId = menuItem.ParentMenuId;

        //                if (!menuMap.ContainsKey(menuId))
        //                {
        //                    menuMap.Add(menuId, menuItem);
        //                }
        //                else
        //                {
        //                    menuMap[menuId] = menuItem;
        //                }

        //                if (parentMenuId == 0)
        //                {
        //                    menuItems.Add(menuItem);
        //                }
        //                else
        //                {
        //                    if (menuMap.ContainsKey(parentMenuId))
        //                    {
        //                        menuMap[parentMenuId].SubItems.Add(menuItem);
        //                    }
        //                }
        //            }
        //        }

        //        reader.Close();
        //    }

        //    // Sort the top-level menu items by position
        //    menuItems = menuItems.OrderBy(item => item.Position).ToList();

        //    // Sort the sub-items of each top-level menu item by position
        //    foreach (var menuItem in menuItems)
        //    {
        //        menuItem.SubItems = menuItem.SubItems.OrderBy(subItem => subItem.Position).ToList();
        //    }

        //    return menuItems;
        //}

        
            protected bool HasActiveChild(List<LIMS.Models.MenuItem> items)
        {
            if (items == null || !items.Any())
                return false;

            string currentPath = Request.Path.ToLower();

            foreach (var item in items)
            {
                if (IsCurrentPage(item.Url))
                    return true;

                if (item.SubItems != null && item.SubItems.Any())
                {
                    if (item.SubItems.Any(subItem => IsCurrentPage(subItem.Url)))
                        return true;
                }
            }
            return false;
        }

        protected bool IsCurrentPage(string url)
        {
            if (string.IsNullOrEmpty(url) || url == "javascript:void(0);")
                return false;

            string currentPath = Request.Path.ToLower();

            if (!string.IsNullOrEmpty(url))
            {
                url = url.ToLower();

                if (currentPath.Contains("://"))
                    currentPath = new Uri(currentPath).AbsolutePath;
                if (url.Contains("://"))
                    url = new Uri(url).AbsolutePath;

                string currentPage = System.IO.Path.GetFileName(currentPath);
                string urlPage = System.IO.Path.GetFileName(url);

                return currentPage.Equals(urlPage, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        private void LoadUserRoles()
        {
            phRoles.Controls.Clear();
            string EmployeeID = Session["EmployeeID"].ToString();
            string activeRole = Session["ActiveRole"].ToString();

            DataTable dt = _cm.GetUserRoles(EmployeeID);
            if (dt.Rows.Count == 1)
            {
                phSwitchRoleSection.Visible = false;
            }
            else
            {
                foreach (DataRow row in dt.Rows)
                {
                    string roleName = row["RoleName"].ToString();
                    string roleID = row["RoleID"].ToString();

                    LinkButton lb = new LinkButton();
                    lb.Text = roleName;
                    lb.CommandArgument = roleID + "|" + roleName;
                    lb.CssClass = "role-menu";
                    lb.Click += ChangeRole_Click;

                    if (roleName == activeRole)
                    {
                        lb.Text = "<i class='material-icons' style='font-size:10px;'>check</i> " + roleName;
                    }
                    else
                    {
                        lb.Text = "<i class='material-icons' style='font-size:10px;'>fiber_manual_record</i> " + roleName;
                    }

                    lb.Attributes["style"] = "display:block;padding:10px 15px;color:#333;text-decoration:none;";

                    phRoles.Controls.Add(lb);
                }
            }

        }
        protected void ChangeRole_Click(object sender, EventArgs e)
        {
            LinkButton lb = (LinkButton)sender;

            string[] roleData = lb.CommandArgument.Split('|');

            string roleID = roleData[0];
            string roleName = roleData[1];

            Session["RoleID"] = roleID;
            Session["ActiveRole"] = roleName;
            Response.Redirect(Request.RawUrl);
        }

    }

}
