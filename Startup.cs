using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;

[assembly: OwinStartup(typeof(Startup))]

public class Startup
{
    public void Configuration(IAppBuilder app)
    {
       

        // Map SignalR
        app.MapSignalR();
    }
}