using Microsoft.Owin;
using Owin;
using SocialNetwork.Web.SignalR;

[assembly: OwinStartup(typeof(Startup))]

namespace SocialNetwork.Web.SignalR
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
