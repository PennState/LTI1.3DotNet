using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Net.Security;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Web.Security;
//using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace OAuth2POC
{
    public partial class _Default : Page
    {
        private static string scopeClaimType = "http://schemas.microsoft.com/identity/claims/scope";

        private static ITokenReplayCache _replayCache = new ReplayCache();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Initial page load - redirect to the IMS reference platform (acting as the LMS) set up on the IMS website
            if (!Request.HttpMethod.Equals("POST", StringComparison.InvariantCultureIgnoreCase))
            {
                Response.Redirect("https://lti-ri.imsglobal.org/platforms/44/resource_links", true);
                return;
            }

            // OAuth 2 Specs for Platform Originating Messages
            // https://www.imsglobal.org/spec/security/v1p0/#platform-originating-messages

            // Get the JWT Token from the id_token form field.
            // We could detect the LTI version by checking which OAuth Form Fields are present.
            // Presence of id_token indicates OAuth 2. Once the token is unpacked, we can check the LTI Version Claim for the exact LTI version
            string ltiLaunchJwtToken = Request.Form["id_token"];
            if (ltiLaunchJwtToken == null)
            {
                CompleteRequest(HttpStatusCode.Forbidden, "JWT token was not provided in id_token form field");
                return;
            }

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                // OAuth 2.0 Required Validations (https://www.imsglobal.org/spec/security/v1p0/#message-security-and-message-signing)

                // 5.1.3 Authentication Response Validation

                // 5.1.3.1
                IssuerSigningKeys = OutOfBandData.GetPlatformSigningKeys(),
                ValidateIssuerSigningKey = true,
                RequireSignedTokens = true,

                // 5.1.3.2 
                ValidIssuer = OutOfBandData.PlatforIssuerId,
                ValidateIssuer = true,

                // 5.1.3.3
                ValidAudience = OutOfBandData.ToolClientId,
                ValidateAudience = true,

                // 5.1.3.4, 5.1.3.5 related to multiple audiences - skip for now

                // 5.1.3.7, 5.1.3.8
                ValidateLifetime = true,
                RequireExpirationTime = true,

                // 5.1.3.9
                TokenReplayCache = _replayCache,                
                ValidateTokenReplay = true,

                ClockSkew = new TimeSpan(0, 0, 15)
            };

            try
            {
                // Validate token.
                ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(ltiLaunchJwtToken, validationParameters, out SecurityToken validatedToken);

                // Set the ClaimsPrincipal on the current thread.
                Thread.CurrentPrincipal = claimsPrincipal;

                // Set the ClaimsPrincipal on HttpContext.Current if the app is running in web hosted environment.
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.User = claimsPrincipal;
                }

                // If the token is scoped, verify that required permission is set in the scope claim.
                // See https://www.imsglobal.org/spec/security/v1p0/#access-token-management
                // 7.1 Access Token Management
                if (ClaimsPrincipal.Current.FindFirst(scopeClaimType) != null && ClaimsPrincipal.Current.FindFirst(scopeClaimType).Value != "user_impersonation")
                {
                    CompleteRequest(HttpStatusCode.Forbidden, "Invalid scope");
                    return;
                }

                // now we have validated token and can use the claims containing usable LTI Launch parameters
                var launchToken = (JwtSecurityToken)validatedToken;

                // Could validate schema here...

                // LTI 1.3 Spec: https://www.imsglobal.org/spec/lti/v1p3
                // COULD USE EXTENSION POINT ON ValidationParameters PERHAPS

                // 2.1.3 lti_deployment_id must be present
                if (!launchToken.Payload.ContainsKey("https://purl.imsglobal.org/spec/lti/claim/deployment_id"))
                {
                    CompleteRequest(HttpStatusCode.Unauthorized, "Deployment id missing");
                    return;
                }

                // 4.3 Required Message Claims

                // 4.3.1 Message type claim
                string messageType = launchToken.Payload.ContainsKey("https://purl.imsglobal.org/spec/lti/claim/message_type") ?
                    launchToken.Payload["https://purl.imsglobal.org/spec/lti/claim/message_type"].ToString() : "";
                if (messageType != "LtiResourceLinkRequest")
                {
                    CompleteRequest(HttpStatusCode.BadRequest, "message_type claim missing or invalid");
                    return;
                }

                // there are additional validations...

                OutputTokenInfo(launchToken.Payload);
            }
            catch (SecurityTokenValidationException stve)
            {
                CompleteRequest(HttpStatusCode.Unauthorized, stve.Message);
                return;
            }
            catch (Exception ex2)
            {
                CompleteRequest(HttpStatusCode.InternalServerError, ex2.Message);
                return;
            }

            CompleteRequest(HttpStatusCode.OK, "");
        }


        private void CompleteRequest(HttpStatusCode statusCode, string statusDescription)
        {
            Response.StatusCode = (int)statusCode;
            Response.StatusDescription = statusDescription;

            if (Response.StatusCode != (int)HttpStatusCode.OK)
                lblError.Text = $"Error: {statusCode.ToString()} {statusDescription}";
            else
                lblError.Text = "";

            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        private void OutputTokenInfo(JwtPayload payload)
        {
            lblLaunchIssuer.Text = payload.Val("iss");
            lblLaunchAudience.Text = payload.Val("aud");
            lblSubjectId.Text = payload.Val("sub");
            lblNonce.Text = payload.Val("nonce");
            lblKeySetUrl.Text = OutOfBandData.PlatformKeySetUrl;

            lblEmail.Text = payload.Val("email");
            lblUserName.Text = payload.Val("name");

            var contextJson = payload.Val("https://purl.imsglobal.org/spec/lti/claim/context");
            LtiContext ctx = JsonConvert.DeserializeObject<LtiContext>(contextJson);
            lblContextCourse.Text = $"({ctx.type[0]} {ctx.id}) {ctx.label}";

            var resourceLinkJson = payload.Val("https://purl.imsglobal.org/spec/lti/claim/resource_link");
            LtiResourceLink rl = JsonConvert.DeserializeObject<LtiResourceLink>(resourceLinkJson);
            lblLaunchLink.Text = $"({rl.id}) {rl.description}";

            string imsRolesJson = payload.Val("https://purl.imsglobal.org/spec/lti/claim/roles");
            string[] imsRoles = JsonConvert.DeserializeObject<string[]>(imsRolesJson);
            foreach (string imsRole in imsRoles)
            {
                blRoles.Items.Add(new ListItem(imsRole));
            }

            string custom = payload.Val("https://purl.imsglobal.org/spec/lti/claim/custom");
            var customDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(custom);
            foreach (var item in customDict)
            {
                blCustom.Items.Add(new ListItem($"{item.Key} = {item.Value}", item.Value));
            }

            // endpoint scopes
            string endpointsJson = payload.Val("https://purl.imsglobal.org/spec/lti-ags/claim/endpoint");
            var endpoints = JsonConvert.DeserializeObject<LtiEndpoint>(endpointsJson);
            foreach (string scope in endpoints.scope)
            {
                blScope.Items.Add(scope);
            }

            // platform
            string platformJson = payload.Val("https://purl.imsglobal.org/spec/lti/claim/tool_platform");
            var platform = JsonConvert.DeserializeObject<LtiPlatform>(platformJson);
            lblPlatform.Text = $"{platform.name}, {platform.version}";

            // launch presentation
            string presentationJson = payload.Val("https://purl.imsglobal.org/spec/lti/claim/launch_presentation");
            var presentation = JsonConvert.DeserializeObject<LtiLaunchPresentation>(presentationJson);
            lblLaunchPresentation.Text = $"{presentation.document_target}, {presentation.width}x{presentation.height}, {presentation.return_url}";
        }
    }

    /// <summary>
    /// This is OAuth 2.0 related data established during out-of-band Tool (LTI) registration process 
    /// </summary>
    public class OutOfBandData
    {
        // Platform's Issuer Id. In the reference tool Platform Configuration this is called "Audience"
        public const string PlatforIssuerId = "https://psu.lti13dot.net/lms";

        // Tool's OAuth 2 client_id
        public const string ToolClientId = "094538B8-3E5F-4713-8EAC-B14B8FCDF9BE";        

        // Platform's public key set discovery URL
        public const string PlatformKeySetUrl = "https://lti-ri.imsglobal.org/platforms/44/platform_keys/39.json";

        // Platform's Public Keys used for signing
        public static IEnumerable<SecurityKey> GetPlatformSigningKeys()
        {
            List<SecurityKey> platformSigningKeys = new List<SecurityKey>();

            // The issuer and signingKeys are cached for 24 hours. They are updated if any of the conditions in the if condition is true.
            if (DateTime.UtcNow.Subtract(_stsMetadataRetrievalTime).TotalHours > 24 || !platformSigningKeys.Any())
            {
                // Get tenant information that's used to validate incoming jwt tokens

                HttpDocumentRetriever documentRetriver = new HttpDocumentRetriever { RequireHttps = true };
                OpenIdConnectConfigurationRetriever configRetriever = new OpenIdConnectConfigurationRetriever();
                ConfigurationManager<OpenIdConnectConfiguration> configManager = new ConfigurationManager<OpenIdConnectConfiguration>(PlatformKeySetUrl, configRetriever, documentRetriver);

                OpenIdConnectConfiguration openIdConfig = configManager.GetConfigurationAsync(CancellationToken.None).GetAwaiter().GetResult();

                JArray platformKeys = (JArray)openIdConfig.AdditionalData["keys"];
                string additionalData = JsonConvert.SerializeObject(openIdConfig.AdditionalData);

                JsonWebKeySet jsonWebKeySet = new JsonWebKeySet(additionalData);
                foreach (JsonWebKey key in jsonWebKeySet.Keys)
                {
                    // filter to only signing keys since there could be others
                    if (key.Use.ToLower() != "sig")
                        continue;

                    if (key.HasPrivateKey)
                        continue;

                    platformSigningKeys.Add(key);
                }

                _stsMetadataRetrievalTime = DateTime.UtcNow;
            }

            return platformSigningKeys;
        }

        private static DateTime _stsMetadataRetrievalTime = DateTime.MinValue;
    }

    public static class Extensions
    {
        public static string Val(this JwtPayload payload, string key)
        {
            if (payload.ContainsKey(key))
                return payload[key].ToString();

            return "";
        }
    }
    
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

    public class LtiContext
    {
        public string id { get; set; }
        public string label { get; set; }
        public string[] type { get; set; }
    }

    public class LtiResourceLink
    {
        public string id { get; set; }
        public string label { get; set; }
        public string description { get; set; }
    }

    public class LtiPlatform
    {
        public string name { get; set; }
        public string contact_email { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string product_family_code { get; set; }
        public string version { get; set; }
    }

    public class LtiEndpoint
    {
        public string[] scope { get; set; }
        public string lineitem { get; set; }
        public string lineitems { get; set; }
    }

    public class LtiLaunchPresentation
    {
        public string document_target { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public string return_url { get; set; }
    }
}