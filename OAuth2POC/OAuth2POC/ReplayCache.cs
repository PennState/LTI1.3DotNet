using System;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
//using System.Web.Mvc;

namespace OAuth2POC
{
    public class ReplayCache : ITokenReplayCache
    {
        private Dictionary<string, DateTime> _cache = new Dictionary<string, DateTime>();

        public bool TryAdd(string securityToken, DateTime expiresOn)
        {
            // don't add if expiration date is in the past
            if (expiresOn <= DateTime.UtcNow)
                return false;

            // add if the token is not found
            if (!TryFind(securityToken))
            {
                // check that token is not in cache since it could be there with an expiration within the time window
                if (!_cache.ContainsKey(securityToken))
                    _cache.Add(securityToken, expiresOn);

                return true;
            }

            return false;
        }

        public bool TryFind(string securityToken)
        {
            if (!_cache.ContainsKey(securityToken))
                return false;

            // if existing token expiration is within time window, treat it as Not Found
            DateTime cachedTokenExpiration = _cache[securityToken];

            if (DateTime.UtcNow <= cachedTokenExpiration)
                return false;

            // existing token is in cache and is not within time window, treat it as Found
            return true;
        }
    }
}