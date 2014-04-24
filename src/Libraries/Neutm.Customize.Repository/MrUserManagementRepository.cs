using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.IdentityServer.Repositories;

namespace Neutm.Customize.Repository
{
    public class MrUserManagementRepository : IUserManagementRepository
    {
        #region IUserManagementRepository Members

        public void CreateUser(string userName, string password, string email = null)
        {
            MrMembership.Instance.UserSvc.CreateAccount(userName, password, email);
        }

        public void DeleteUser(string userName)
        {
            var user = MrMembership.Instance.UserSvc.GetByUsername(userName);
            if (user != null)
            {
                MrMembership.Instance.UserSvc.DeleteAccount(user.ID);
            }
        }

        public IEnumerable<string> GetUsers(int start, int count, out int totalCount)
        {
            if (start < 0) start = 0;
            if (count < 0) count = 10;
            var skip = start * count;
            return MrMembership.Instance.UserQuery.Query(
                MrMembership.Instance.UserSvc.Configuration.DefaultTenant, null, skip, count, out totalCount).Select(x => x.Username);
        }

        public IEnumerable<string> GetUsers(string filter, int start, int count, out int totalCount)
        {
            // convert from pages to rows
            if (start < 0) start = 0;
            if (count < 0) count = 10;
            var skip = start * count;
            return MrMembership.Instance.UserQuery.Query(
                MrMembership.Instance.UserSvc.Configuration.DefaultTenant, 
                filter, skip, count, out totalCount).Select(x => x.Username);
        }

        public void SetPassword(string userName, string password)
        {
            var user = MrMembership.Instance.UserSvc.GetByUsername(userName);
            if (user != null)
            {
                MrMembership.Instance.UserSvc.SetPassword(user.ID, password);
            }
        }

        public void SetRolesForUser(string userName, IEnumerable<string> roles)
        {
            var user = MrMembership.Instance.UserSvc.GetByUsername(userName);
            if (user != null)
            {
                MrMembership.Instance.UserSvc.RemoveClaim(user.ID, ClaimTypes.Role);
                if (roles != null)
                {
                    foreach (var role in roles)
                    {
                        MrMembership.Instance.UserSvc.AddClaim(user.ID, ClaimTypes.Role, role);
                    }
                }
            }
        }

        public IEnumerable<string> GetRolesForUser(string userName)
        {
            var user = MrMembership.Instance.UserSvc.GetByUsername(userName);
            if (user != null)
            {
                return user.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value);
            }
            return Enumerable.Empty<string>();
        }

        public IEnumerable<string> GetRoles()
        {
            return MrMembership.Instance.GroupQuery.GetRoleNames(
                 MrMembership.Instance.UserSvc.Configuration.DefaultTenant);
        }

        public void CreateRole(string roleName)
        {
            MrMembership.Instance.GroupSvc.Create(roleName);
        }

        public void DeleteRole(string roleName)
        {
            var grp = MrMembership.Instance.GroupSvc.Get(roleName);
            if (grp != null)
            {
                MrMembership.Instance.GroupSvc.Delete(grp.ID);
            }
        }

        #endregion
    }
}
