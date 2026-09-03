using System;
using System.Web.Routing;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using LIMS.DAL;
using Syncfusion.Licensing;
using System.IO;

namespace LIMS
{
	public class Global : System.Web.HttpApplication
    {
        public static readonly string SyncfusionLicenseKey = "Ngo9BigBOggjHTQxAR8/V1JAaF5cX2pCd1p/TH5YfUNzdUVEY1ZUTXxaS1ZhSXxVdkJgWX1dcnJWRGFVUER9XEY=";
        public class LoggedUser
		{
			public string EmployeeName { get; set; }
			public string UserName { get; set; }
			public DateTime LoginTime { get; set; }
			public DateTime LastActivity { get; set; }
		}
		public static List<LoggedUser> LoggedUsers = new List<LoggedUser>();
		protected void Application_AcquireRequestState(object sender, EventArgs e)
		{
			if (HttpContext.Current.Session != null &&
				HttpContext.Current.Session["UserName"] != null)
			{
				string user = HttpContext.Current.Session["UserName"].ToString();

				lock (LoggedUsers)
				{
					var u = LoggedUsers.FirstOrDefault(x => x.UserName == user);
					if (u != null)
					{
						u.LastActivity = DateTime.Now;
					}
				}
			}
			try
			{
				string userId = "";
				string userName = "";

				if (Session != null)
				{
					userId = Convert.ToString(Session["UserID"]);
					userName = Convert.ToString(Session["UserName"]);
				}

				System.IO.File.AppendAllText(
					@"C:\inetpub\logs\requests.log",
					DateTime.Now +
					" | " + Request.RawUrl +
					" | " + userId +
					" : " + userName +
					Environment.NewLine);
			}
			catch { }
		}
		protected void Application_Start(object sender, EventArgs e) 		
		{
            SyncfusionLicenseProvider.RegisterLicense(SyncfusionLicenseKey);
        }
		
		protected void Session_Start(object sender, EventArgs e) 
		{

		}

		protected void Application_BeginRequest(object sender, EventArgs e) 
		{
			//System.IO.File.AppendAllText(
	  // @"C:\inetpub\logs\requests.log",
	  // DateTime.Now + " | " + Request.RawUrl + Environment.NewLine);
		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e) 
		{

		}
		//protected void Application_Error(object sender, EventArgs e)
		//{
		//    Exception ex = Server.GetLastError();

		//    while (ex != null)
		//    {
		//        Response.Write("<hr/>");
		//        Response.Write("<b>" + ex.GetType().FullName + "</b><br/>");
		//        Response.Write(ex.Message + "<br/><br/>");
		//        Response.Write("<pre>" + ex.StackTrace + "</pre>");

		//        ex = ex.InnerException;
		//    }

		//    Response.End();
		//}
		protected void Application_Error(object sender, EventArgs e)
		{
			Exception ex = Server.GetLastError();

			string message = ex.Message;
			string stackTrace = ex.StackTrace;

			// For inner exception (very important)
			if (ex.InnerException != null)
			{
				message += " | Inner: " + ex.InnerException.Message;
			}
			Guid errorId = Guid.NewGuid();

			ErrorLogger.LogError(errorId, ex);

			Server.ClearError();

			Session["ErrorMessage"] = ex.Message + "," + ex.Source + "," + ex.StackTrace;
            string logPath = Server.MapPath("~/ErrorLog/ApplicationErrors.txt");

            message =
                "Date: " + DateTime.Now + Environment.NewLine +
                "URL: " + Request.Url + Environment.NewLine +
                "User: " + (Session?["UserName"] ?? "Anonymous") + Environment.NewLine +
                "Error: " + ex.Message + Environment.NewLine +
                "StackTrace: " + ex.StackTrace + Environment.NewLine +
                "InnerException: " + ex.InnerException?.ToString() + Environment.NewLine +
                "--------------------------------------------------" +
                Environment.NewLine;

            File.AppendAllText(logPath, message);

            //Response.Redirect("~/Error.aspx?id=" + errorId);
        }

		void Session_End(object sender, EventArgs e)
		{
			string user = Session["UserName"] as string;
			lock (Global.LoggedUsers)
			{
				Global.LoggedUsers.RemoveAll(u => u.UserName == user);
			}
		}

		protected void Application_End(object sender, EventArgs e) 
		{

		}
	}
}