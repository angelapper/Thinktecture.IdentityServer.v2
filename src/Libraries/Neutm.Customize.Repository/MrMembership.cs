using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Ef;

namespace Neutm.Customize.Repository
{
    public class MrMembership
    {
        private static MrMembership instance;

        private UserAccountService userSvc;
        private GroupService groupSvc;
        private IUserAccountQuery userQuery;
        private IGroupQuery groupQuery;

        public static MrMembership Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MrMembership();
                }
                return instance;
            }
        }

        public UserAccountService UserSvc
        {
            get { return userSvc; }
        }

        public GroupService GroupSvc
        {
            get { return groupSvc; }
        }

        public IUserAccountQuery UserQuery
        {
            get { return userQuery; }
        }

        public IGroupQuery GroupQuery
        {
            get { return groupQuery; }
        }

        public MrMembership()
        {
            Database.SetInitializer<DefaultMembershipRebootDatabase>(
                new MigrateDatabaseToLatestVersion<DefaultMembershipRebootDatabase, 
                    BrockAllen.MembershipReboot.Ef.Migrations.Configuration>());

            var settings = SecuritySettings.FromConfiguration();
            settings.RequireAccountVerification = false;
            settings.PasswordHashingIterationCount = 50000;
            var config = new MembershipRebootConfiguration(settings);
            var uarepo = new BrockAllen.MembershipReboot.Ef.DefaultUserAccountRepository();
            this.userSvc = new UserAccountService(config, uarepo);
            this.userQuery = uarepo;

            var grpRepo = new BrockAllen.MembershipReboot.Ef.DefaultGroupRepository();
            this.groupSvc = new GroupService(config.DefaultTenant, grpRepo);
            this.groupQuery = grpRepo;
        }
    }
}
