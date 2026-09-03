using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class PTQualisMasterFull : System.Web.UI.MasterPage
    {

        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CM"].ToString());
        string date = DateTime.Today.ToString("dd-MMM-yyyy");
        string time = DateTime.Now.ToString("h:mm:ss tt");
        string CreateDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string UpdateDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        string constr = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
        //DataAccessLayer dal = new DataAccessLayer();

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddDays(-1));
            Response.Cache.SetNoStore();
            string pageName = System.IO.Path.GetFileName(Request.Url.AbsolutePath);

            // Skip session check for VendorNew.aspx
            if (pageName.Equals("VendorNew.aspx", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            try
            {
                if (Session["Email"] == null)
                {
                    Response.Redirect("Login.aspx");
                    //Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('Session Timeout'); setTimeout(function() { window.location = 'Login.aspx'; }, 100); </script>");
                }
                else
                {
                    if (!IsPostBack)
                    {
                        string str1 = @"SELECT * FROM Employees e
                                        INNER JOIN Users u ON u.employeeID = e.ID
                                        Where u.UserID ='" + Session["UserID"] + "' ";
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

                         
                        }
                        con.Close();



                    }
                }
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
            List<Models.MenuItem> menuItems = new List<Models.MenuItem>();

            using (SqlConnection connection = new SqlConnection(constr))
            {
                /*string query = "SELECT [MenuId], [ParentMenuId], [Title], [Description], [Url], [RoleId], [MenuIcon], [Position] FROM [Menus]";*/
                string query = "SELECT * FROM Menus ms INNER JOIN IconTB itb ON itb.IconId = ms.MenuIcon";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                Dictionary<int, Models.MenuItem> menuMap = new Dictionary<int, Models.MenuItem>();
                while (reader.Read())
                {
                    string[] roleIds = reader["RoleId"].ToString().Split(',');

                    /* Check if the desired roleId exists in the roleIds list*/
                    if (roleIds.Contains(roleId))
                    {
                        Models.MenuItem menuItem = new Models.MenuItem
                        {
                            MenuId = Convert.ToInt32(reader["MenuId"]),
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

                            SubItems = new List<Models.MenuItem>()
                        };

                        int menuId = menuItem.MenuId;
                        int parentMenuId = menuItem.ParentMenuId;

                        if (!menuMap.ContainsKey(menuId))
                        {
                            menuMap.Add(menuId, menuItem);
                        }
                        else
                        {
                            menuMap[menuId] = menuItem;
                        }

                        if (parentMenuId == 0)
                        {
                            menuItems.Add(menuItem);
                        }
                        else
                        {
                            if (menuMap.ContainsKey(parentMenuId))
                            {
                                menuMap[parentMenuId].SubItems.Add(menuItem);
                            }
                        }
                    }
                }

                reader.Close();
            }

            // Sort the top-level menu items by position
            menuItems = menuItems.OrderBy(item => item.Position).ToList();

            // Sort the sub-items of each top-level menu item by position
            foreach (var menuItem in menuItems)
            {
                menuItem.SubItems = menuItem.SubItems.OrderBy(subItem => subItem.Position).ToList();
            }

            return menuItems;
        }

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

        

    }

}
