// <copyright file="LTIRequest.cs" company="The Pennsylvania State University">
//     Copyright (c) The Pennsylvania State University. All rights reserved.
// </copyright>
namespace OAuth2POC
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Data.SqlClient;
    using System.Web;
    using System.Net;

    /// <summary>
    /// LTI Launch data and API.
    /// http://oauth.net/core/1.0
    /// </summary>
    [Serializable]
    public class LTI11Request : LtiRequestBase
    {
        protected override void _Validate()
        {
            IsValid = ValidateSignature(_formData, "mysecret", _absoluteUri).valid;
            // secret above would come from configuration setting in our code
        }

        public LTI11Request(NameValueCollection formData, string absoluteUri) : base(formData, absoluteUri) { }

        protected override void _ExtractClaims()
        {
            CourseName = context_title;
            ApiDomain = custom_canvas_api_domain;

            long.TryParse(custom_canvas_course_id, out long courseId);
            CourseNumericId = courseId;

            SisCourseId = custom_canvas_sis_course_id;

            long.TryParse(custom_canvas_user_id, out long userId);
            UserNumericId = userId;

            if (custom_canvas_user_login_id != null)
            {
                int atPos = custom_canvas_user_login_id.IndexOf('@');
                int length = atPos > 0 ? atPos : custom_canvas_user_login_id.Length;
                SisUserId = custom_canvas_user_login_id.Substring(0, length);
            }

            UserLoginId = custom_canvas_user_login_id;

            long.TryParse(custom_psu_external_tool_id, out long externalToolId);
            ExternalToolId = externalToolId;

            LaunchPresentationURL = launch_presentation_return_url;

            CommonCSSURL = custom_canvas_css_common;

            IsCourseInstructor = isCourseInstructor;
            IsAdmin = isAdmin;
            IsInstructor = isInstructor;
            IsHelpDeskAdmin = isHelpDeskAdmin;
            IsGroupAdmin = isGroupAdmin;
            IsDevTeamAdmin = isDevTeamAdmin;
            IsCourseAdmin = isCourseAdmin;
        }

        #region properties

        private string oauth_consumer_secret { get; set; }
        private string oauth_consumer_key { get; set; }
        private string oauth_token { get; set; }
        private string oauth_signature_method { get; set; }
        private string oauth_timestamp { get; set; }
        private string oauth_nonce { get; set; }
        private string oauth_version { get; set; }
        private string user_id { get; set; }
        private string roles { get; set; }
        private string context_id { get; set; }
        private string context_title { get; set; }
        private string context_label { get; set; }
        private string custom_canvas_api_domain { get; set; }
        private string custom_canvas_course_id { get; set; }
        private string custom_canvas_sis_course_id { get; set; }
        private string custom_canvas_enrollment_state { get; set; }
        private string custom_canvas_user_id { get; set; }
        private string custom_canvas_user_login_id { get; set; }
        private string custom_psu_external_tool_id { get; set; }
        private string ext_roles { get; set; }
        private string launch_presentation_return_url { get; set; }
        private bool valid { get; set; }
        private bool isCourseInstructor { get; set; }
        private bool isAdmin { get; set; }
        private bool isStudent { get; set; }
        private bool isInstructor { get; set; }
        private string custom_canvas_css_common { get; set; }
        private bool isHelpDeskAdmin { get; set; }
        private bool isGroupAdmin { get; set; }
        private bool isDevTeamAdmin { get; set; }
        private bool isCourseAdmin { get; set; }

        #endregion properties

        #region public methods

        /// <summary>
        /// Generate oAuth standard timestamp
        /// Unless otherwise specified by the Service Provider, the timestamp is expressed in the number of seconds 
        /// since January 1, 1970 00:00:00 GMT. The timestamp value MUST be a positive integer and MUST be equal or 
        /// greater than the timestamp used in previous requests.
        /// </summary>
        /// <returns></returns>
        private static string GenerateTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /// <summary>
        /// Generate Nounce for oAuth signature, this could be changed to a GUID
        /// The term nonce means ‘number used once’ and is a unique and usually random string that is meant 
        /// to uniquely identify each signed request.
        /// The Consumer SHALL then generate a Nonce value that is unique for all requests with that timestamp. 
        /// A nonce is a random string, uniquely generated for each request. The nonce allows the Service Provider to 
        /// verify that a request has never been made before and helps prevent replay attacks when requests are made 
        /// over a non-secure channel (such as HTTP).
        /// </summary>
        /// <returns></returns>
        private static string GenerateNonce()
        {
            // Just a simple implementation of a random number between 123400 and 9999999
            Random random = new Random();
            return random.Next(123400, 9999999).ToString();
        }

        /// <summary>
        /// The Signature Base String is a consistent reproducible concatenation of the request elements into a single string. 
        /// The string is used as an input in hashing or signing algorithms. The HMAC-SHA1 signature method provides both a 
        /// standard and an example of using the Signature Base String with a signing algorithm to generate signatures. 
        /// All the request parameters MUST be encoded as described in Parameter Encoding prior to constructing the Signature 
        /// Base String.
        /// </summary>
        /// <param name="formVariables"></param>
        /// <param name="absoluteUri"></param>
        /// <returns></returns>
        private static string SignatureBase(NameValueCollection formVariables, string absoluteUri)
        {

            StringBuilder sBase = new StringBuilder();
            string start = "POST&" + Utility.UrlEncode(absoluteUri.Replace("http:", "https:")) + "&";

            SortedDictionary<string, string> sortedRequest = new SortedDictionary<string, string>();
            foreach (string key in formVariables.Keys)
            {
                if (key != LTIRequestKeys.OAuthSignature)
                {
                    sortedRequest.Add(key, formVariables[key]);
                }
            }

            foreach (string k in sortedRequest.Keys)
            {
                if (k != LTIRequestKeys.OAuthSignature)
                {
                    sBase.Append((k)).Append(("=")).Append(Utility.UrlEncode(sortedRequest[k] ?? ""));
                    sBase.Append(("&"));
                }
            }
            if (sBase.Length > 0)
            {
                sBase.Remove(sBase.Length - 1, 1);
            }

            return start + Utility.UrlEncode(sBase.ToString());
        }

        /// <summary>
        /// Generate the compelte signature by hashins the base with the secret
        /// </summary>
        /// <param name="SignatureBase"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        private static string GenerateSignature(string SignatureBase, string secret)
        {
            HMACSHA1 hmacsha1 = new HMACSHA1();
            hmacsha1.Key = Encoding.ASCII.GetBytes(string.Format("{0}&{1}", HttpUtility.UrlEncode(secret), ""));
            byte[] hashBytes = hmacsha1.ComputeHash(Encoding.ASCII.GetBytes(SignatureBase));
            hmacsha1.Dispose();
            return Convert.ToBase64String(hashBytes);
        }

        //private static LTI11Request ValidateSignature(HttpRequest request, string secret)
        //{
        //    return ValidateSignature(request.Form, secret, request.Url.AbsoluteUri);
        //}

        /// <summary>
        /// Validate a signature by comparing a generated value and the value passed by the launch
        /// </summary>
        /// <param name="formVariables"></param>
        /// <param name="secret"></param>
        /// <param name="absoluteUri"></param>
        /// <returns></returns>
        private LTI11Request ValidateSignature(NameValueCollection formVariables, string secret, string absoluteUri)
        {
            Exception exception = null;

            //LTI11Request LTIReq = new LTI11Request();
            this.valid = false;

            try
            {
                string signatureBase = SignatureBase(formVariables, absoluteUri);
                string genSig = GenerateSignature(signatureBase, secret);

                if (genSig == (formVariables[LTIRequestKeys.OAuthSignature] ?? ""))
                {
                    buildLtiRequest(formVariables, this);
                }
                else
                {
                    // generated signature did not match oauth_signature
                    throw new UnauthorizedAccessException("Unauthorized LTI Launch: " + JsonConvert.SerializeObject(formVariables));
                }
            }
            catch (Exception e)
            {
                exception = e;
            }

            return this;
        }

        /// <summary>
        /// Validate a JWT 
        /// </summary>
        /// <param name="formVariables"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        private LTI11Request ValidateJwt(NameValueCollection formVariables, string secret)
        {
            Exception exception = null;

            LTI11Request LTIReq = this;
            LTIReq.valid = false;

            try
            {
                buildLtiRequest(formVariables, LTIReq);
            }
            catch (Exception e)
            {
                exception = e;
            }

            return LTIReq;
        }

        /// <summary>
        /// Determines whether the given collection contains an administrator role.
        /// </summary>
        /// <param name="formVariables"></param>
        /// <returns></returns>
        private static bool HasAdminRole(NameValueCollection formVariables)
        {
            return (formVariables?[LTIRequestKeys.ExtRoles] ?? "").Contains(LTIRequestKeys.ExtRoleAdministrator);
        }

        /// <summary>
        /// Retrieves the custom Canvas API domain from the given collection.
        /// </summary>
        /// <param name="formVariables"></param>
        /// <returns></returns>
        private static string GetCustomCanvasApiDomain(NameValueCollection formVariables)
        {
            return formVariables?[LTIRequestKeys.CustomCanvasApiDomain] ?? "";
        }

        #endregion

        #region Internal Logic

        /// <summary>
        /// Build a validated LTI request
        /// </summary>
        /// <param name="formVariables"></param>
        /// <param name="LTIReq"></param>
        private static void buildLtiRequest(NameValueCollection formVariables, LTI11Request LTIReq)
        {

            //TODO:  validate timestamp: LTIReq.oauth_timestamp                    
            LTIReq.custom_canvas_css_common = formVariables[LTIRequestKeys.CustomCanvasCss] ?? "";
            LTIReq.user_id = formVariables[LTIRequestKeys.UserId] ?? "";
            LTIReq.roles = formVariables[LTIRequestKeys.Roles] ?? "";
            LTIReq.context_id = formVariables[LTIRequestKeys.ContextId] ?? "";
            LTIReq.context_title = formVariables[LTIRequestKeys.ContextTitle] ?? "";
            LTIReq.custom_canvas_api_domain = formVariables[LTIRequestKeys.CustomCanvasApiDomain] ?? "";
            LTIReq.custom_canvas_user_id = formVariables[LTIRequestKeys.CustomCanvasUserId] ?? "";
            LTIReq.custom_canvas_user_login_id = formVariables[LTIRequestKeys.CustomCanvasUserLoginId] ?? "";
            LTIReq.custom_canvas_sis_course_id = formVariables[LTIRequestKeys.CustomCanvasSisCourseId] ?? "";
            LTIReq.custom_psu_external_tool_id = formVariables[LTIRequestKeys.CustomPsuExternalToolId] ?? "";
            LTIReq.ext_roles = formVariables[LTIRequestKeys.ExtRoles] ?? "";
            LTIReq.launch_presentation_return_url = formVariables[LTIRequestKeys.LaunchPresentationReturnUrl] ?? "";

            var contextLabel = (formVariables[LTIRequestKeys.ContextLabel] ?? "").Trim();
            if (contextLabel.Length > 0)
            {
                LTIReq.context_label = contextLabel;
            }

            //srg25 7/6/2015 - Adding a check to see if there is a course id or not
            var customCanvasCourseId = (formVariables[LTIRequestKeys.CustomCanvasCourseId] ?? "").Trim();
            if (customCanvasCourseId.Length > 0)
            {
                LTIReq.custom_canvas_course_id = customCanvasCourseId;
            }

            //srg25 7/6/2015 - Adding a check to see if there is an enrollment state.  
            var customCanvasEncollmentState = (formVariables[LTIRequestKeys.CustomCanvasEnrollmentState] ?? "").Trim();
            if (customCanvasEncollmentState.Length > 0)
            {
                LTIReq.custom_canvas_enrollment_state = customCanvasEncollmentState;
            }

            // populate role flags
            LTIReq.isAdmin = HasAdminRole(formVariables);
            LTIReq.isInstructor = (formVariables[LTIRequestKeys.ExtRoles] ?? "").Contains(LTIRequestKeys.ExtRoleInstructor);

            LTIReq.isStudent = LTIReq.roles.Contains(LTIRequestKeys.RoleStudent);
            LTIReq.isCourseInstructor = LTIReq.roles.Contains(LTIRequestKeys.RoleInstructor);
            //sxn82-2016/11/29 - null check for user id. 
            //if ((formVariables[LTIRequestKeys.ExtRoles] ?? "").Contains(LTIRequestKeys.ExtRoleAdministrator) && !string.IsNullOrWhiteSpace(LTIReq.custom_canvas_user_id))
            //{
            //    //srg25-2016/4/11 -  need to check to see if this person is in an allowable admin position
            //    var admRoles = CanvasAdminApiBL.ListAccountAdmins(Settings.PsuAccountId, new[] { Convert.ToInt64(LTIReq.custom_canvas_user_id) });

            //    // srg25-2016/4/11 - Get the allowable admin roles to use tool from env var value
            //    var helpdeskAdminRoles = PsuDbDal.GetEnvVar("HelpdeskAdminRoles", "1").Split(',').ToList();

            //    LTIReq.isHelpDeskAdmin = admRoles.Any(r => helpdeskAdminRoles.Contains(r.Role));

            //    // Start Revision: 2017/4/10 srg25 -- Needed another property for dev team admins
            //    var devTeamAdminRoles = PsuDbDal.GetEnvVar("DevTeamAdminRoles", "1").Split(',').ToList();

            //    LTIReq.isDevTeamAdmin = admRoles.Any(r => devTeamAdminRoles.Contains(r.Role));
            //}
            //else
            //{
            //    LTIReq.isHelpDeskAdmin = false;
            //    LTIReq.isDevTeamAdmin = false;
            //    // End Revision: 2017/4/10 srg25
            //}

            //// Get all 'Pride Admins' for this course
            //LTIReq.isGroupAdmin = false;
            //long courseid;
            //long.TryParse(LTIReq.custom_canvas_course_id, out courseid);
            //List<CanvasEnrollmentApiStruct> TAEnrollments = CanvasEnrollmentApiBL.GetCourseEnrollments(courseid, type: new string[] { "TaEnrollment" });
            //var groupAdmins = TAEnrollments.Where(r => r.Role.Equals("Pride Admin")).ToList();
            //foreach (var thisEnrollment in groupAdmins)
            //{
            //    if (thisEnrollment.User.Id.ToString() == LTIReq.custom_canvas_user_id)
            //    {
            //        LTIReq.isGroupAdmin = true;
            //    }
            //}

            //LTIReq.isCourseAdmin = false;
            //List<CanvasEnrollmentApiStruct> TeacherEnrollments = CanvasEnrollmentApiBL.GetCourseEnrollments(courseid, type: new string[] { "TeacherEnrollment" });
            //var courseAdmins = TeacherEnrollments.Where(r => r.Role.Equals("Course Admin")).ToList();
            //foreach (var thisEnrollment in courseAdmins)
            //{
            //    if (thisEnrollment.User.Id.ToString() == LTIReq.custom_canvas_user_id)
            //    {
            //        LTIReq.isCourseAdmin = true;
            //    }
            //}

            LTIReq.valid = true;

        }

        #endregion Internal Logic
    }


}