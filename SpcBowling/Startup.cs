using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SpcBowling.Startup))]
namespace SpcBowling
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
