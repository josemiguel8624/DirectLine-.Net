using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DirectLineTest.Startup))]
namespace DirectLineTest
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
