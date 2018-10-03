using System;
using System.Web;
using System.Web.UI;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace OAuth2POC
{
    public partial class Lti1113 : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!Request.HttpMethod.Equals("POST", StringComparison.InvariantCultureIgnoreCase))
            //{
            //    if (Request.QueryString["a"] != null)
            //        Response.Redirect("https://lti-ri.imsglobal.org/platforms/44/resource_links", true);
            //    else
            //        Response.Redirect("https://worldcampus.instructure.com/courses/233", true);

            //        return;
            //}

            ILTIRequest request = LtiRequestBase.CreateInstance(Request.Form, Request.Url.AbsoluteUri);
            if (request == null)
            {
                lblError.Text = "LTI Version could not be determined";
                return;
            }

            if (request.IsValid)
            {
                OutputTokenInfo(request);
                CompleteRequest(HttpStatusCode.OK, "Successful Launch!");
            }
            else
            {
                CompleteRequest(HttpStatusCode.Unauthorized, "Launch failed!");
            }
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

        private void OutputTokenInfo(ILTIRequest request)
        {
            lblApiDomain.Text = request.ApiDomain;
            lblCommonCssUrl.Text = request.CommonCSSURL;
            lblCourseId.Text = request.CourseNumericId.ToString();
            lblCourseName.Text = request.CourseName;
            lblCourseSisId.Text = request.SisCourseId;
            lblExtToolId.Text = request.ExternalToolId.ToString();

            lblIsAdmin.Text = request.IsAdmin.ToString();
            lblIsCourseAdmin.Text = request.IsCourseAdmin.ToString();
            lblIsCourseInstructor.Text = request.IsCourseInstructor.ToString();
            lblIsGroupAdmin.Text = request.IsGroupAdmin.ToString();

            lblLaunchPresentationUrl.Text = request.LaunchPresentationURL;
            lblLoginId.Text = request.UserLoginId;
            lblUserId.Text = request.UserNumericId.ToString();
            lblUserSisId.Text = request.SisUserId;            
        }
    }
}