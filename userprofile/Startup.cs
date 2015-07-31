using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(userprofile.Startup))]
namespace userprofile
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
