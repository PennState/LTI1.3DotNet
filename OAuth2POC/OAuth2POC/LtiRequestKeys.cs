// <copyright file="LTIRequestKeys.cs" company="The Pennsylvania State University">
//     Copyright (c) The Pennsylvania State University. All rights reserved.
// </copyright>
namespace OAuth2POC
{
    /// <summary>
    /// Keys for LTI Request form fields
    /// </summary>
    public static class LTIRequestKeys
    {
        /* please keep these alphabetized */

        public const string ContextId = "context_id";
        public const string ContextLabel = "context_label";
        public const string ContextTitle = "context_title";

        public const string CustomCanvasApiDomain = "custom_canvas_api_domain";
        public const string CustomCanvasCourseId = "custom_canvas_course_id";
        public const string CustomCanvasCss = "custom_canvas_css_common";
        public const string CustomCanvasEnrollmentState = "custom_canvas_enrollment_state";
        public const string CustomCanvasSisCourseId = "custom_canvas_sis_course_id";
        public const string CustomCanvasUserId = "custom_canvas_user_id";
        public const string CustomCanvasUserLoginId = "custom_canvas_user_login_id";
        public const string CustomPsuExternalToolId = "custom_psu_external_tool_id";

        public const string ExtRoles = "ext_roles";
        public const string ExtRoleAdministrator = "urn:lti:instrole:ims/lis/Administrator";
        public const string ExtRoleInstructor = "Instructor";

        public const string HelpDeskAdminRolesEnvVarKey = "HelpdeskAdminRoles";
        public const string LaunchPresentationReturnUrl = "launch_presentation_return_url";
        public const string OAuthSignature = "oauth_signature";

        public const string RoleInstructor = "Instructor";
        public const string RoleStudent = "Student";
        public const string Roles = "roles";

        public const string UserId = "user_id";
    }
}