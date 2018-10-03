using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OAuth2POC
{
    public interface ILTIRequest
    {
        string CourseName { get; }

        string ApiDomain { get; }

        long CourseNumericId { get; }

        string SisCourseId { get; }

        long UserNumericId { get; }

        string SisUserId { get; } // no @ sign

        string UserLoginId { get; } // includes @ sign, might not need this at all

        long ExternalToolId { get; }

        string LaunchPresentationURL { get; }

        string CommonCSSURL { get; }

        bool IsCourseInstructor { get; }

        bool IsAdmin { get; }

        bool IsInstructor { get; }

        bool IsHelpDeskAdmin { get; }

        bool IsGroupAdmin { get; }

        bool IsDevTeamAdmin { get; }

        bool IsCourseAdmin { get; }

        bool IsValid { get; }
    }
}
