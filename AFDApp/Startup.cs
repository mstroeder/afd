using Microsoft.Owin;
using Owin;
using System.Data.Entity;

[assembly: OwinStartupAttribute(typeof(AFDApp.Startup))]
namespace AFDApp
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
            var db = new AFDDbInitialize();
            Database.SetInitializer(db);
        }
    }
}
