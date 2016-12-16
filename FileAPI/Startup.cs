using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MCS.Startup))]
namespace MCS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
