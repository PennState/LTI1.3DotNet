using System.IdentityModel.Tokens.Jwt;
//using System.Web.Mvc;

namespace OAuth2POC
{
    public static class Extensions
    {
        public static string Val(this JwtPayload payload, string key)
        {
            if (payload.ContainsKey(key))
                return payload[key].ToString();

            return "";
        }
    }
}