using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Security.Claims;
using System.Threading;
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
}
