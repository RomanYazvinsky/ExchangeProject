using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;

namespace Exchange.Common.Utils
{
    public static class AuthUtils
    {
        public static Guid? GetUserId([NotNull] IEnumerable<Claim> claims)
        {
            var idClaim = claims.FirstOrDefault(claim => ClaimTypes.NameIdentifier.Equals(claim.Type));
            if (idClaim == null)
            {
                return null;
            }

            return Guid.Parse(idClaim.Value);
        }
    }
}
