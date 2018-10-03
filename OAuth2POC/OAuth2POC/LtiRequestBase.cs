using System;
using System.Collections.Specialized;
using System.Web;

namespace OAuth2POC
{
    /// <summary>
    /// Base functionality for LTI Requests.
    /// 
    /// Subclasses must use the "_formData" member to access HTTP request form data.
    /// Subclasses must use the "_absoluteUri" member to access HTTP absolute URI.
    /// Subclasses must implement the "_Validate()" method, which must set the IsValid property to the result of the validation.
    /// Subclasses must implement the "_ExtractClaims" method, which sets the ILtiRequest properties from request payload
    /// 
    /// Clients must use LtiRequestBase.CreateInstance() factory method for instantiation.
    /// </summary>
    public abstract class LtiRequestBase : ILTIRequest
    {
        #region factory method

        /// <summary>
        /// Creates a concrete implementation of ILTIRequest for the LTI version detected by inspecting the provided parameters
        /// </summary>
        /// <param name="formData">HTTP Form data</param>
        /// <param name="absoluteUri">HTTP Request Absolute URI</param>
        /// <returns>Concrete ILTIRequest instance</returns>
        public static ILTIRequest CreateInstance(NameValueCollection formData, string absoluteUri)
        {
            if (IsLti11Launch(formData))
                return new LTI11Request(formData, absoluteUri);

            if (IsLti13Launch(formData))
                return new Lti13Request(formData, absoluteUri);

            if (IsCustomJavascriptLaunch(formData))
                return new LtiCustomJavascriptRequest(formData, absoluteUri);

            return null;
        }

        #endregion

        #region public properties

        public string CourseName { get; set; }

        public string ApiDomain { get; set; }

        public long CourseNumericId { get; set; }

        public string SisCourseId { get; set; }

        public long UserNumericId { get; set; }

        public string SisUserId { get; set; }

        public string UserLoginId { get; set; }

        public long ExternalToolId { get; set; }

        public string LaunchPresentationURL { get; set; }

        public string CommonCSSURL { get; set; }

        public bool IsCourseInstructor { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsInstructor { get; set; }

        public bool IsHelpDeskAdmin { get; set; }

        public bool IsGroupAdmin { get; set; }

        public bool IsDevTeamAdmin { get; set; }

        public bool IsCourseAdmin { get; set; }

        public bool IsValid { get; set; }

        #endregion

        #region subclass members

        protected NameValueCollection _formData { get; private set; }

        protected string _absoluteUri { get; private set; }

        protected abstract void _Validate();

        protected abstract void _ExtractClaims();

        #endregion

        #region protected constructors

        /// <summary>
        /// Constructor that validates the request based on the provided HTTP form data and sets all properties to non-null values.
        /// </summary>
        /// <param name="formData"></param>
        protected LtiRequestBase(NameValueCollection formData, string absoluteUri)
        {
            this._formData = formData;
            this._absoluteUri = absoluteUri;

            InitFields();

            _Validate();

            if (IsValid)
                _ExtractClaims();
        }

        #endregion

        #region implementation details

        private void InitFields()
        {
            this.IsValid = false;

            // provide non-null defaults for all properties
            this.ApiDomain = string.Empty;
            this.CommonCSSURL = string.Empty;
            this.CourseName = string.Empty;
            this.CourseNumericId = 0L;
            this.ExternalToolId = 0L;
            this.IsAdmin = false;
            this.IsCourseAdmin = false;
            this.IsCourseInstructor = false;
            this.IsDevTeamAdmin = false;
            this.IsGroupAdmin = false;
            this.IsHelpDeskAdmin = false;
            this.IsInstructor = false;
            this.LaunchPresentationURL = string.Empty;
            this.SisCourseId = string.Empty;
            this.SisUserId = string.Empty;
            this.UserLoginId = string.Empty;
            this.UserNumericId = 0L;
        }

        private static bool IsLti11Launch(NameValueCollection formData)
        {
            bool result = formData["oauth_signature"] != null;

            return result;
        }

        private static bool IsLti13Launch(NameValueCollection formData)
        {
            bool result = formData["id_token"] != null;

            return result;
        }

        private static bool IsCustomJavascriptLaunch(NameValueCollection formData)
        {
            bool result = formData["JWT"] != null;

            return result;
        }

        #endregion
    }
}