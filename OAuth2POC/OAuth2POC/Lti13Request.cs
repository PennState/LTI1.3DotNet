using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Threading;
using Newtonsoft.Json;
using System.Collections.Specialized;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json.Linq;

using Ims;

namespace OAuth2POC
{

    public sealed class Lti13Request : LtiRequestBase
    {
        private static string scopeClaimType = "http://schemas.microsoft.com/identity/claims/scope";
        private static ITokenReplayCache _replayCache = new ReplayCache();
        private JwtSecurityToken _jwtToken;

        protected override void _Validate()
        {
            _jwtToken = ValidateOAuth20();
            var lti13Valid = ValidateLTI13();

            IsValid = _jwtToken != null && lti13Valid;
        }

        public Lti13Request(NameValueCollection formData, string absoluteUri) : base(formData, absoluteUri) { }

        /// <summary>
        /// OAuth 2.0 validation
        /// </summary>
        /// <returns></returns>
        private JwtSecurityToken ValidateOAuth20()
        {
            // OAuth 2 Specs for Platform Originating Messages
            // https://www.imsglobal.org/spec/security/v1p0/#platform-originating-messages

            // Get the JWT Token from the id_token form field.
            // We could detect the LTI version by checking which OAuth Form Fields are present.
            // Presence of id_token indicates OAuth 2. Once the token is unpacked, we can check the LTI Version Claim for the exact LTI version
            string ltiLaunchJwtToken = _formData["id_token"];
            if (ltiLaunchJwtToken == null)
                return null;

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
                return null;

            // now we have validated token and can use the claims containing usable LTI Launch parameters
            var launchToken = (JwtSecurityToken)validatedToken;

            return launchToken;
        }

        /// <summary>
        /// LTI 1.3 validation
        /// </summary>
        /// <param name="launchToken"></param>
        /// <returns></returns>
        private bool ValidateLTI13()
        {
            if (_jwtToken == null)
                return false;

            // 2.1.3 lti_deployment_id must be present
            if (!_jwtToken.Payload.ContainsKey("https://purl.imsglobal.org/spec/lti/claim/deployment_id"))
                return false;

            // 4.3 Required Message Claims

            // 4.3.1 Message type claim
            string messageType = _jwtToken.Payload.ContainsKey("https://purl.imsglobal.org/spec/lti/claim/message_type") ?
                _jwtToken.Payload["https://purl.imsglobal.org/spec/lti/claim/message_type"].ToString() : "";
            if (messageType != "LtiResourceLinkRequest")
                return false;

            // TODO: more validations required for LTI 1.3 specification conformance

            return true;
        }


        // how to test for roles????

        protected override void _ExtractClaims()
        {
            var payload = _jwtToken.Payload;

            this.UserLoginId = payload.Val("email");

            var contextJson = payload.Val("https://purl.imsglobal.org/spec/lti/claim/context");
            LtiContext ctx = JsonConvert.DeserializeObject<LtiContext>(contextJson);
            this.CourseName = $"({ctx.type[0]} {ctx.id}) {ctx.label}";

            string imsRolesJson = payload.Val("https://purl.imsglobal.org/spec/lti/claim/roles");
            string[] imsRoles = JsonConvert.DeserializeObject<string[]>(imsRolesJson);

            this.IsAdmin = LISRoleValidator.ContainsContextPrincipalRole(imsRoles, LISRoleName.Administrator);
            this.IsCourseAdmin = LISRoleValidator.ContainsContextPrincipalRole(imsRoles, LISRoleName.ContentDeveloper);
            this.IsCourseInstructor = LISRoleValidator.ContainsContextPrincipalRole(imsRoles, LISRoleName.Instructor);
            this.IsGroupAdmin = LISRoleValidator.ContainsContextPrincipalRole(imsRoles, LISRoleName.Mentor);

            //string custom = payload.Val("https://purl.imsglobal.org/spec/lti/claim/custom");
            //var customDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(custom);
            //foreach (var item in customDict)
            //{
            //    blCustom.Items.Add(new ListItem($"{item.Key} = {item.Value}", item.Value));
            //}

            //// endpoint scopes
            //string endpointsJson = payload.Val("https://purl.imsglobal.org/spec/lti-ags/claim/endpoint");
            //var endpoints = JsonConvert.DeserializeObject<LtiEndpoint>(endpointsJson);
            //foreach (string scope in endpoints.scope)
            //{
            //    blScope.Items.Add(scope);
            //}

            //// platform
            //string platformJson = payload.Val("https://purl.imsglobal.org/spec/lti/claim/tool_platform");
            //var platform = JsonConvert.DeserializeObject<LtiPlatform>(platformJson);
            //lblPlatform.Text = $"{platform.name}, {platform.version}";

            // launch presentation
            string presentationJson = payload.Val("https://purl.imsglobal.org/spec/lti/claim/launch_presentation");
            var presentation = JsonConvert.DeserializeObject<LtiLaunchPresentation>(presentationJson);
            this.LaunchPresentationURL = $"{presentation.document_target}, {presentation.width}x{presentation.height}, {presentation.return_url}";
        }
    }

    public class LtiContext
    {
        public string id { get; set; }
        public string label { get; set; }
        public string[] type { get; set; }
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

    public class LtiPlatform
    {
        public string name { get; set; }
        public string contact_email { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string product_family_code { get; set; }
        public string version { get; set; }
    }

    public class LtiResourceLink
    {
        public string id { get; set; }
        public string label { get; set; }
        public string description { get; set; }
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
}