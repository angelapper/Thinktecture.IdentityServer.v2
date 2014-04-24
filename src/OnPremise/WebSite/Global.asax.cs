using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Data.Entity;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using BrockAllen.MembershipReboot.Ef;
using RedisSessionProvider.Config;
using StackExchange.Redis;
using Thinktecture.IdentityServer.Repositories;
using Thinktecture.IdentityServer.Repositories.Sql;

namespace Thinktecture.IdentityServer.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        [Import]
        public IConfigurationRepository ConfigurationRepository { get; set; }

        [Import]
        public IUserRepository UserRepository { get; set; }

        [Import]
        public IRelyingPartyRepository RelyingPartyRepository { get; set; }

        private static ConfigurationOptions redisConfigOpts;

        protected void Application_Start()
        {
            // create empty config database if it not exists
            Database.SetInitializer(new ConfigurationDatabaseInitializer());

            Database.SetInitializer<DefaultMembershipRebootDatabase>(
                 new MigrateDatabaseToLatestVersion<DefaultMembershipRebootDatabase,
                 BrockAllen.MembershipReboot.Ef.Migrations.Configuration>());


            // set the anti CSRF for name (that's a unqiue claim in our system)
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Name;

            MvcApplication.redisConfigOpts = ConfigurationOptions.Parse("localhost:6379");

            // pass it to RedisSessionProvider configuration class
            RedisConnectionConfig.GetSERedisServerConfig = (HttpContextBase context) =>
            {
                return new KeyValuePair<string, ConfigurationOptions>(
                    "DefaultConnection",                // if you use multiple configuration objects, please make the keys unique
                    MvcApplication.redisConfigOpts);
            };

            // setup MEF
            SetupCompositionContainer();
            Container.Current.SatisfyImportsOnce(this);

            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters, ConfigurationRepository);
            RouteConfig.RegisterRoutes(RouteTable.Routes, ConfigurationRepository, UserRepository);
            ProtocolConfig.RegisterProtocols(GlobalConfiguration.Configuration, RouteTable.Routes, ConfigurationRepository, UserRepository, RelyingPartyRepository);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        private void SetupCompositionContainer()
        {
            Container.Current = new CompositionContainer(CompositionOptions.IsThreadSafe, new RepositoryExportProvider());
        }
    }
}