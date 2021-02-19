using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HaloMondays.Startup))]
namespace HaloMondays
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
