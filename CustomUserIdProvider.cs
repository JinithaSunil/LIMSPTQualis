using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using System.Web;

namespace LIMS
{
    public class CustomUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            return HttpContext.Current.Session["UserId"]?.ToString();
        }
    }
}