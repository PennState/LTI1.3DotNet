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
    public class LtiCustomJavascriptRequest : LtiRequestBase
    {
        protected override void _Validate()
        {
            var token = _formData["JWT"];
            var courseid = (!string.IsNullOrEmpty(_formData["CourseId"])) ? _formData["CourseId"] : "";
            var userid = (!string.IsNullOrEmpty(_formData["UserId"])) ? _formData["UserId"] : "";

            // NEXT LINE need to know how to unpack token, then add in courseid and userid to claims collection before validating
            //            NameValueCollection nvc = addFormValuesToLTI(courseid, userid);
            //            form = new Jwt(Settings.LTISecret).DecodeToFormData(token, nvc);
            // USE MICROSOFT JWT LIBRARY HERE
            //            IsValid = ValidateSignature(_formData, "mysecret", _absoluteUri).valid;
            // secret above would come from configuration setting in our code
        }

        public LtiCustomJavascriptRequest(NameValueCollection formData, string absoluteUri) : base(formData, absoluteUri) { }

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
        /// Determines whether the given collection contains an administrator role.
        /// </summary>
        /// <param name="formVariables"></param>
        /// <returns></returns>
        private static bool HasAdminRole(NameValueCollection formVariables)
        {
            return (formVariables?[LTIRequestKeys.ExtRoles] ?? "").Contains(LTIRequestKeys.ExtRoleAdministrator);
        }

        #endregion

        #region Internal Logic

        /// <summary>
        /// Build a validated LTI request
        /// </summary>
        /// <param name="formVariables"></param>
        /// <param name="LTIReq"></param>
        private static void buildLtiRequest(NameValueCollection formVariables, LtiCustomJavascriptRequest LTIReq)
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