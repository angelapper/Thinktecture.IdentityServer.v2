using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BrockAllen.MembershipReboot;
using Thinktecture.IdentityServer.Repositories;

namespace Neutm.Customize.Repository
{
    public class MrUserRepository : IUserRepository
    {
        #region IUserRepository Members

        public bool ValidateUser(string userName, string password)
        {
            return MrMembership.Instance.UserSvc.Authenticate(userName, password);
        }

        public bool ValidateUser(System.Security.Cryptography.X509Certificates.X509Certificate2 clientCertificate, out string userName)
        {
            UserAccount user;
            if (MrMembership.Instance.UserSvc.AuthenticateWithCertificate(clientCertificate, out user))
            {
                userName = user.Username;
                return true;
            }

            userName = null;
            return false;
        }

        public IEnumerable<string> GetRoles(string userName)
        {
            var user = MrMembership.Instance.UserSvc.GetByUsername(userName);
            if (user != null)
            {
                return user.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value);
            }
            return Enumerable.Empty<string>();
        }

        #endregion
    }
}
