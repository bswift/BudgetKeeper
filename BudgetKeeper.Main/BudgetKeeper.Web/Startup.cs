using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BudgetKeeper.Web.Startup))]
namespace BudgetKeeper.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
