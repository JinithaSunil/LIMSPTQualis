using System;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web.UI;
using System.Configuration;
using LIMS.DAL;
using LIMS.Models;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class ForgotPassword1 : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["CM"].ConnectionString;
        DataAccessLayer dal = new DataAccessLayer();
        DAL.CM _cm = new DAL.CM();

        // Email configuration
        private string smtpHost;
        private int smtpPort;
        private string smtpUserName;
        private string smtpPassword;
        private bool enableSsl = true;
        private string fromEmail;
        private string appName = "PTQualis";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblMessage.CssClass = "message";
                lblMessage.Text = "";

                BindRoles();
            }
        }
        
        private void BindRoles()
        {
            try
            {
                DataTable dt = _cm.getAllRoles();
                if (dt.Rows.Count > 0)
                {
                    ddlRole.DataSource = dt;
                    ddlRole.DataTextField = "RoleName";
                    ddlRole.DataValueField = "RoleID";
                    ddlRole.DataBind();
                    ddlRole.Items.Insert(0, new ListItem("Select Role", ""));
                }
            }
            catch (Exception ex)
            {
                LogError("BindRoles", ex);
                ShowMessage($"Error loading roles: {ex.Message}", "error");
            }
        }

        protected void btnSendPassword_Click(object sender, EventArgs e)
        {
            string UserName = txtUserName.Text.Trim();
            string role = ddlRole.SelectedValue;

            // Validate input
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(role))
            {
                ShowMessage("Please fill all required fields.", "error");
                return;
            }

            // Get user details including password
            UserDetails user = GetUserDetails(UserName, role);

            if (user != null)
            {
                // Check if email exists
                if (string.IsNullOrEmpty(user.Email))
                {
                    ShowMessage("❌ Email Address not found. Please contact Data-Admin.", "error");
                    return;
                }

                // Send password via email
                if (SendPasswordEmail(user))
                {
                    ShowMessageWithIcon(
                        $"✅ Password has been sent to <strong>{user.Email}</strong><br/>" +
                        "Please check your inbox (and spam folder).",
                        "success"
                    );
                    // Clear the form
                    txtUserName.Text = "";
                    ddlRole.SelectedIndex = 0;
                }
                else
                {
                    ShowMessage("❌ Failed to send email. Please try again later or contact support.", "error");
                }
            }
            else
            {
                ShowMessage("❌ Invalid UserName or role combination. Please check your credentials or contact Data-Admin.", "error");
            }
        }

        private UserDetails GetUserDetails(string UserName, string role)
        {
            UserDetails user = null;

            string query = @"SELECT * FROM Employees e
                                INNER JOIN Users u ON u.employeeID = e.ID
                                INNER JOIN EmployeeDepartment ed ON ed.EmployeeDepartmentID = e.DepartmentID
                                INNER JOIN Roles r ON r.RoleID = u.RoleID
                                INNER JOIN designation d ON d.DesignationID = e.DesignationID
                                INNER JOIN Department dp ON dp.DepartmentID = ed.DepartmentID
                                WHERE u.UserName = @UserName AND u.RoleID = @Role AND deleted = 0 AND status = 1";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserName", UserName);
                        cmd.Parameters.AddWithValue("@Role", role);

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                user = new UserDetails
                                {
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    UserName = reader["UserName"].ToString(),
                                    Password = reader["Password"].ToString(),
                                    FullName = reader["EmployeeName"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    Role = reader["RoleName"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError("GetUserDetails", ex);
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts",
                    "<script> alert('Error: ' + '" + ex.Message.Replace("'", "\\'") + "'); </script>");
            }

            return user;
        }

        private bool SendPasswordEmail(UserDetails user)
        {
            try
            {
               
                List<SMTPDataModel> SMTPGetDataList = dal.SMTPDataListDisplay();
                if (SMTPGetDataList == null || SMTPGetDataList.Count == 0)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script> alert('No active SMTP records found.'); </script>");
                    return false;
                }
                else
                {
                    foreach (var item in SMTPGetDataList)
                    {
                        Session["SMTP_PORT"] = item.SMTP_PORT;
                        Session["SMTP_NAME"] = item.SMTP_NAME;
                        Session["SMTP_EMAIL"] = item.SMTP_EMAIL;
                        Session["SMTP_PASSWORD"] = item.SMTP_PASSWORD;
                        Session["SMTP_URL"] = item.SMTP_URL;
                        Session["FROM_EMAIL"] = item.SMTP_EMAIL;
                        Session["Project_URL"] = item.SMTP_URL;
                        Session["WebsiteURL"] = item.WebsiteURL;
                    }
                    smtpHost = Session["SMTP_NAME"].ToString() ?? "smtp.gmail.com"; 
                    smtpPort = Convert.ToInt32(Session["SMTP_PORT"] ?? 587);
                    smtpUserName = Session["SMTP_EMAIL"]?.ToString();
                    smtpPassword = Session["SMTP_PASSWORD"]?.ToString();
                    fromEmail = Session["SMTP_EMAIL"]?.ToString();
                }

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(fromEmail, appName);
                    mail.To.Add(new MailAddress(user.Email, user.FullName));
                    mail.Subject = $"Your Password - {appName}";
                    mail.IsBodyHtml = true;
                    mail.Body = GetEmailBody(user);
                    mail.Priority = MailPriority.High;

                    using (SmtpClient smtp = new SmtpClient(smtpHost, smtpPort))
                    {
                        smtp.Credentials = new NetworkCredential(smtpUserName, smtpPassword);
                        smtp.EnableSsl = enableSsl;
                        smtp.Timeout = 30000;
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                        // Send email
                        smtp.Send(mail);
                        LogInfo($"Email sent successfully to {user.Email}");
                        return true;
                    }
                }
            }
            catch (SmtpException smtpEx)
            {
                string errorMessage = GetSmtpErrorMessage(smtpEx);
                LogError("SendPasswordEmail - SMTP", smtpEx);
                ShowMessage($"SMTP Error: {errorMessage}", "error");
                return false;
            }
            catch (Exception ex)
            {
                LogError("SendPasswordEmail - General", ex);
                ShowMessage($"Email Error: {ex.Message}", "error");
                return false;
            }
        }

        private string GetSmtpErrorMessage(SmtpException ex)
        {
            // Using the correct SMTP status codes that exist in .NET Framework
            switch (ex.StatusCode)
            {
                case SmtpStatusCode.GeneralFailure:
                    return "General failure. Check SMTP server settings.";
                case SmtpStatusCode.ServiceNotAvailable:
                    return "SMTP service is not available. Check server status.";
                case SmtpStatusCode.MailboxBusy:
                    return "Mailbox is busy. Please try again later.";
                case SmtpStatusCode.InsufficientStorage:
                    return "Insufficient storage in mailbox.";
                case SmtpStatusCode.ExceededStorageAllocation:
                    return "Mailbox storage limit exceeded.";
                case SmtpStatusCode.ClientNotPermitted:
                    return "Client not permitted. Check authentication settings.";
                // These status codes don't exist in older .NET Framework
                // Use general messages instead
                case SmtpStatusCode.MailboxUnavailable:
                    return "Mailbox is unavailable. Check the recipient email address.";
                case SmtpStatusCode.UserNotLocalWillForward:
                    return "User not local but will forward.";
                case SmtpStatusCode.MustIssueStartTlsFirst:
                    return "Must issue STARTTLS command first. Enable SSL.";
                default:
                    return $"SMTP Error: {ex.StatusCode} - {ex.Message}";
            }
        }

        private string GetEmailBody(UserDetails user)
        {
            StringBuilder body = new StringBuilder();

            body.AppendLine(@"
            <html>
            <head>
                <style>
                    body { font-family: 'Segoe UI', Arial, sans-serif; line-height: 1.6; color: #333; }
                    .container { max-width: 600px; margin: 0 auto; padding: 20px; }
                    .header { 
                        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                        color: white; 
                        padding: 30px; 
                        text-align: center;
                        border-radius: 10px 10px 0 0;
                    }
                    .content { 
                        padding: 30px; 
                        background: #f9f9f9;
                        border: 1px solid #e0e0e0;
                        border-top: none;
                        border-radius: 0 0 10px 10px;
                    }
                    .password-box {
                        background: white;
                        padding: 15px;
                        border-radius: 8px;
                        border-left: 4px solid #667eea;
                        margin: 20px 0;
                        box-shadow: 0 2px 4px rgba(0,0,0,0.1);
                    }
                    .password-box .label {
                        font-size: 12px;
                        color: #999;
                        text-transform: uppercase;
                        letter-spacing: 1px;
                    }
                    .password-box .password {
                        font-size: 24px;
                        font-weight: bold;
                        color: #333;
                        font-family: 'Courier New', monospace;
                        letter-spacing: 2px;
                        margin-top: 5px;
                    }
                    .details-table {
                        width: 100%;
                        margin: 15px 0;
                    }
                    .details-table td {
                        padding: 8px 5px;
                        border-bottom: 1px solid #e0e0e0;
                    }
                    .details-table .label-cell {
                        font-weight: 600;
                        color: #555;
                        width: 30%;
                    }
                    .warning {
                        background: #fff3cd;
                        padding: 15px;
                        border-radius: 8px;
                        border-left: 4px solid #ffc107;
                        margin: 15px 0;
                    }
                    .footer {
                        margin-top: 25px;
                        padding-top: 20px;
                        border-top: 1px solid #e0e0e0;
                        text-align: center;
                        font-size: 12px;
                        color: #999;
                    }
                    .btn {
                        display: inline-block;
                        padding: 10px 20px;
                        background: #667eea;
                        color: white !important;
                        text-decoration: none;
                        border-radius: 5px;
                        margin: 10px 0;
                    }
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h2>🔐 Password Recovery</h2>
                        <p>Your account information</p>
                    </div>
                    <div class='content'>
                        <p>Dear <strong>" + user.FullName + @"</strong>,</p>
                        <p>You requested to recover your password. Below are your account details:</p>
                        
                        <div class='password-box'>
                            <div class='label'>Your Password</div>
                            <div class='password'>" + user.Password + @"</div>
                        </div>
                        
                        <table class='details-table'>
                            <tr>
                                <td class='label-cell'>UserName</td>
                                <td><strong>" + user.UserName + @"</strong></td>
                            </tr>
                            <tr>
                                <td class='label-cell'>Role</td>
                                <td><strong>" + user.Role + @"</strong></td>
                            </tr>
                            <tr>
                                <td class='label-cell'>Email</td>
                                <td><strong>" + user.Email + @"</strong></td>
                            </tr>
                        </table>
                        
                        <div class='warning'>
                            <strong>⚠️ Security Notice:</strong>
                            <ul style='margin: 10px 0 0 20px;'>
                                <li>For security reasons, please change your password after logging in.</li>
                                <li>Do not share this email with anyone.</li>
                                <li>If you didn't request this, please ignore this email.</li>
                            </ul>
                        </div>
                        
                        <p style='text-align: center; margin: 20px 0;'>
                            <a href='" + GetLoginUrl() + @"' class='btn'>Login to Your Account</a>
                        </p>
                    </div>
                    <div class='footer'>
                        <p>&copy; " + DateTime.Now.Year + @" " + appName + @". All rights reserved.</p>
                        <p>This is an automated message, please do not reply to this email.</p>
                    </div>
                </div>
            </body>
            </html>");

            return body.ToString();
        }

        private string GetLoginUrl()
        {
            // Get the login URL from session or use default
            string websiteUrl = Session["WebsiteURL"]?.ToString();
            if (!string.IsNullOrEmpty(websiteUrl))
            {
                return websiteUrl + "/Login.aspx";
            }
            return "https://yourdomain.com/Login.aspx";
        }

        private void ShowMessage(string message, string type)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = $"message {type} show";
        }

        private void ShowMessageWithIcon(string message, string type)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = $"message {type} show";

            // Add script to hide loader
            string script = "document.getElementById('loaderDiv').classList.remove('show');";
            ScriptManager.RegisterStartupScript(this, GetType(), "HideLoader", script, true);
        }

        private void LogError(string method, Exception ex)
        {
            try
            {
                string logPath = Server.MapPath("~/App_Data/ErrorLog.txt");
                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | ERROR | {method} | {ex.Message} | {ex.StackTrace}{Environment.NewLine}";

                string directory = System.IO.Path.GetDirectoryName(logPath);
                if (!System.IO.Directory.Exists(directory))
                    System.IO.Directory.CreateDirectory(directory);

                System.IO.File.AppendAllText(logPath, logMessage);
            }
            catch { /* Silent fail */ }
        }

        private void LogInfo(string message)
        {
            try
            {
                string logPath = Server.MapPath("~/App_Data/InfoLog.txt");
                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | INFO | {message}{Environment.NewLine}";

                string directory = System.IO.Path.GetDirectoryName(logPath);
                if (!System.IO.Directory.Exists(directory))
                    System.IO.Directory.CreateDirectory(directory);

                System.IO.File.AppendAllText(logPath, logMessage);
            }
            catch { /* Silent fail */ }
        }
    }

    // UserDetails class
    public class UserDetails
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}