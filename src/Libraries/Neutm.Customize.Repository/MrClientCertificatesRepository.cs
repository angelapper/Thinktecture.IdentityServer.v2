using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrockAllen.MembershipReboot;
using Thinktecture.IdentityServer.Repositories;

namespace Neutm.Customize.Repository
{
    public class MrClientCertificatesRepository : IClientCertificatesRepository
    {
        #region IClientCertificatesRepository Members

        public bool TryGetUserNameFromThumbprint(System.Security.Cryptography.X509Certificates.X509Certificate2 certificate, out string userName)
        {
            UserAccount user;
            if (MrMembership.Instance.UserSvc
                .AuthenticateWithCertificate(certificate, out user))
            {
                userName = user.Username;
                return true;
            }
            userName = null;
            return false;
        }

        public bool SupportsWriteAccess
        {
            get { return true; }
        }

        public IEnumerable<string> List(int pageIndex, int pageSize)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 0) pageSize = 10;
            int skip = pageSize * (pageIndex - 1);
            int totalCount;
            return MrMembership.Instance.UserQuery.Query(
                MrMembership.Instance.UserSvc.Configuration.DefaultTenant, null, skip, pageSize, out totalCount).Select(x => x.Username);
        }

        public IEnumerable<Thinktecture.IdentityServer.Models.ClientCertificate> GetClientCertificatesForUser(string userName)
        {
            var user = MrMembership.Instance.UserSvc.GetByUsername(userName);
            if (user != null)
            {
                return user.Certificates.Select(x => new Thinktecture.IdentityServer.Models.ClientCertificate() { UserName = user.Username, Thumbprint = x.Thumbprint, Description = x.Subject });
            }
            return Enumerable.Empty<Thinktecture.IdentityServer.Models.ClientCertificate>();
        }

        public void Add(Thinktecture.IdentityServer.Models.ClientCertificate certificate)
        {
            var user = MrMembership.Instance.UserSvc.GetByUsername(certificate.UserName);
            if (user != null)
            {
                MrMembership.Instance.UserSvc.AddCertificate
                    (user.ID, certificate.Thumbprint, certificate.Description);
            }
        }

        public void Delete(Thinktecture.IdentityServer.Models.ClientCertificate certificate)
        {
            var user = MrMembership.Instance.UserSvc.GetByUsername(certificate.UserName);
            if (user != null)
            {
                MrMembership.Instance.UserSvc.RemoveCertificate(user.ID, certificate.Thumbprint);
            }
        }

        #endregion
    }
}
