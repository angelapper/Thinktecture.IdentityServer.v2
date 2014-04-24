using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.IdentityServer.Repositories;

namespace Neutm.Customize.Repository
{
    public class MrClaimsRepository : IClaimsRepository
    {

        #region IClaimsRepository Members

        public IEnumerable<System.Security.Claims.Claim> GetClaims(System.Security.Claims.ClaimsPrincipal principal, Thinktecture.IdentityServer.TokenService.RequestDetails requestDetails)
        {
            var user = MrMembership.Instance.UserSvc.GetByUsername(principal.Identity.Name);
            if (user == null) throw new ArgumentException("Invalid Username");

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.Username));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.ID.ToString("D")));
            if (!String.IsNullOrWhiteSpace(user.Email))
            {
                claims.Add(new Claim(ClaimTypes.Email, user.Email));
            }
            if (!String.IsNullOrWhiteSpace(user.MobilePhoneNumber))
            {
                claims.Add(new Claim(ClaimTypes.MobilePhone, user.MobilePhoneNumber));
            }
            //var x509 = from c in user.Certificates
            //           select new Claim(ClaimTypes.X500DistinguishedName, c.Subject);
            //claims.AddRange(x509);
            var otherClaims =
                (from uc in user.Claims
                 select new Claim(uc.Type, uc.Value)).ToList();
            claims.AddRange(otherClaims);

            return claims;
        }

        public IEnumerable<string> GetSupportedClaimTypes()
        {
            return
                new string[] { ClaimTypes.Name, ClaimTypes.Email, ClaimTypes.MobilePhone, ClaimTypes.Role };
        }

        #endregion
    }
}
