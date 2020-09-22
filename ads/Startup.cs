using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ads.Startup))]
namespace ads
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
