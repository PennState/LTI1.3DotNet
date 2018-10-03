using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Text;

namespace Ims
{
    /// <summary>
    /// Learning Tools Interoperability® 1.3 Core Specification (https://www.imsglobal.org/spec/lti/v1p3)
    /// A.2 Role vocabularies
    /// </summary>
    public static class LISRoleValidator
    {
        #region public methods

        /// <summary>
        /// Indicates that the provided input role has the provided role type
        /// and the input role and role type are valid role names.
        /// </summary>
        /// <param name="longRoleName">The role being tested</param>
        /// <param name="simpleRoleNameToFind">The type of role</param>
        /// <returns></returns>
        public static bool CheckContextPrincipalRole(string longRoleName, string simpleRoleNameToFind)
        {
            string longRoleNameToFind = $"{contextRolePrefix}#{simpleRoleNameToFind}";

            return RolesValidAndEqual(longRoleName, longRoleNameToFind);
        }

        public static bool CheckContextPrincipalAndSubrole(string longRoleName, string simplePrincipalRoleNameToFind, string simpleSubroleNameToFind)
        {
            string longRoleNameToFind = $"{contextRolePrefix}/{simplePrincipalRoleNameToFind}#{simpleSubroleNameToFind}";

            return RolesValidAndEqual(longRoleName, longRoleNameToFind);
        }

        public static bool CheckSystemRole(string longRoleName, string simpleRoleNameToFind)
        {
            string longRoleNameToFind = $"{systemRolePrefix}{simpleRoleNameToFind}";

            return RolesValidAndEqual(longRoleName, longRoleNameToFind);
        }

        public static bool CheckInstitutionRole(string longRoleName, string simpleRoleNameToFind)
        {
            string longRoleNameToFind = $"{intitutionRolePrefix}{simpleRoleNameToFind}";

            return RolesValidAndEqual(longRoleName, longRoleNameToFind);
        }

        /// <summary>
        /// Checks that an array of roles contains the provided role.
        /// </summary>
        /// <param name="longRoleNames">Collection of roles</param>
        /// <param name="roleToFind">The roles to find</param>
        /// <returns></returns>
        public static bool ContainsSystemRole(string[] longRoleNames, string simpleRoleNameToFind)
        {
            foreach (string longRoleName in longRoleNames)
            {
                if (CheckSystemRole(longRoleName, simpleRoleNameToFind))
                    return true;
            }

            return false;
        }

        public static bool ContainsInstitutionRole(string[] longRoleNames, string simpleRoleNameToFind)
        {
            foreach (string longRoleName in longRoleNames)
            {
                if (CheckInstitutionRole(longRoleName, simpleRoleNameToFind))
                    return true;
            }

            return false;
        }

        public static bool ContainsContextPrincipalRole(string[] longRoleNames, string simpleRoleNameToFind)
        {
            foreach (string longRoleName in longRoleNames)
            {
                if (CheckContextPrincipalRole(longRoleName, simpleRoleNameToFind))
                    return true;
            }

            return false;
        }

        public static bool ContainsContextPrincipalAndSubrole(string[] longRoleNames, string simplePrincipalRoleNameToFind, string simpleSubroleToFind)
        {
            foreach (string longRoleName in longRoleNames)
            {
                if (CheckContextPrincipalAndSubrole(longRoleName, simplePrincipalRoleNameToFind, simpleSubroleToFind))
                    return true;
            }

            return false;
        }
        #endregion

        #region role collections

        private static List<string> _systemRoles = new List<string>
        {
            // core system roles
            LISRoleName.Administrator,
            LISRoleName.None,

            // non-core system roles
            LISRoleName.AccountAdmin,
            LISRoleName.Creator,
            LISRoleName.SysAdmin,
            LISRoleName.SysSupport,
            LISRoleName.User
        };
        
        private static List<string> _institutionRoles = new List<string>
        {
            // core institution roles
            LISRoleName.Administrator,
            LISRoleName.Guest,
            LISRoleName.None,
            LISRoleName.Other,
            LISRoleName.Staff,
            LISRoleName.Student,

            // non-core institution roles
            LISRoleName.Alumni,
            LISRoleName.Faculty,
            LISRoleName.Instructor,
            LISRoleName.Learner,
            LISRoleName.Member,
            LISRoleName.Mentor,
            LISRoleName.Observer,
            LISRoleName.ProspectiveStudent
        };

        private static List<string> _contextPrincipalRoles = new List<string>
        {
            // core context roles
            LISRoleName.Administrator,
            LISRoleName.ContentDeveloper,
            LISRoleName.Instructor,
            LISRoleName.Learner,
            LISRoleName.Mentor,

            // non-core context roles
            LISRoleName.Manager,
            LISRoleName.Member,
            LISRoleName.Officer
        };

        private static List<string> _contextAdministratorSubroles = new List<string>
        {
            // subroles are non-core
            LISRoleName.Administrator,
            LISRoleName.Developer,
            LISRoleName.ExternalDeveloper,
            LISRoleName.ExternalSupport,
            LISRoleName.ExternalSystemAdministrator,
            LISRoleName.Support,
            LISRoleName.SystemAdministrator
        };

        private static List<string> _contextContentDeveloperSubroles = new List<string>
        {
            // subroles are non-core
            LISRoleName.ContentDeveloper,
            LISRoleName.ContentExpert,
            LISRoleName.ExternalContentExpert,
            LISRoleName.Librarian
        };

        private static List<string> _contextInstructorSubroles = new List<string>
        {
            // subroles are non-core
            LISRoleName.ExternalInstructor,
            LISRoleName.Grader,
            LISRoleName.GuestInstructor,
            LISRoleName.Instructor,
            LISRoleName.Lecturer,
            LISRoleName.PrimaryInstructor,
            LISRoleName.SecondaryInstructor,
            LISRoleName.TeachingAssistant,
            LISRoleName.TeachingAssistantGroup,
            LISRoleName.TeachingAssistantOffering,
            LISRoleName.TeachingAssistantSection,
            LISRoleName.TeachingAssistantTemplate
        };

        private static List<string> _contextLearnerSubroles = new List<string>
        {
            // subroles are non-core
            LISRoleName.ExternalLearner,
            LISRoleName.GuestLearner,
            LISRoleName.Learner,
            LISRoleName.NonCreditLearner
        };

        private static List<string> _contextManagerSubroles = new List<string>
        {
            // subroles are non-core
            LISRoleName.AreaManager,
            LISRoleName.CourseCoordinator,
            LISRoleName.ExternalObserver,
            LISRoleName.Manager,
            LISRoleName.Observer
        };

        private static List<string> _contextMemberSubroles = new List<string>
        {
            // subroles are non-core
            LISRoleName.Member
        };

        private static List<string> _contextMentorSubroles = new List<string>
        {
            // subroles are non-core
            LISRoleName.Advisor,
            LISRoleName.Auditor,
            LISRoleName.ExternalAdvisor,
            LISRoleName.ExternalAuditor,
            LISRoleName.ExternalLearningFacilitator,
            LISRoleName.ExternalMentor,
            LISRoleName.ExternalReviewer,
            LISRoleName.LearningFacilitator,
            LISRoleName.Mentor,
            LISRoleName.Reviewer,
            LISRoleName.Tutor
        };

        private static List<string> _contextOfficerSubroles = new List<string>
        {
            // subroles are non-core
            LISRoleName.Chair,
            LISRoleName.Secretary,
            LISRoleName.Treasurer,
            LISRoleName.ViceChair
        };

        private static Dictionary<string, List<string>> _contextPrincipalToSubroleMap = new Dictionary<string, List<string>>
        {
            { LISRoleName.Administrator, _contextAdministratorSubroles },
            { LISRoleName.ContentDeveloper, _contextContentDeveloperSubroles },
            { LISRoleName.Instructor, _contextInstructorSubroles },
            { LISRoleName.Learner, _contextLearnerSubroles },
            { LISRoleName.Manager, _contextManagerSubroles },
            { LISRoleName.Member, _contextMemberSubroles },
            { LISRoleName.Mentor, _contextMentorSubroles },
            { LISRoleName.Officer, _contextOfficerSubroles }
        };

        private static List<string> _allRoles = GetAllRoles();

        #endregion

        #region implementation details

        const string systemRolePrefix = "http://purl.imsglobal.org/vocab/lis/v2/system/person#";
        const string intitutionRolePrefix = "http://purl.imsglobal.org/vocab/lis/v2/institution/person#";
        const string contextRolePrefix = "http://purl.imsglobal.org/vocab/lis/v2/membership";

        private static bool RolesValidAndEqual(string longRoleName1, string longRoleName2)
        {
            bool namesAreValid = _allRoles.Contains(longRoleName1) && _allRoles.Contains(longRoleName2);

            return namesAreValid && longRoleName1 == longRoleName2;
        }

        private static List<string> GetAllRoles()
        {
            List<string> allRoleNames = new List<string>();

            // system roles
            foreach (string systemRole in _systemRoles)
                allRoleNames.Add($"{systemRolePrefix}#{systemRole}");

            // institution roles
            foreach (string institutionRole in _institutionRoles)
                allRoleNames.Add($"{intitutionRolePrefix}#{institutionRole}");

            // principal context roles
            foreach (string principalRole in _contextPrincipalRoles)
            {
                // uri based (preferred)
                allRoleNames.Add($"{contextRolePrefix}#{principalRole}");

                // simple name (deprecated)
                allRoleNames.Add($"{principalRole}");
            }

            // context subroles
            foreach (string principalRoleName in _contextPrincipalToSubroleMap.Keys)
            {
                var subroles = _contextPrincipalToSubroleMap[principalRoleName];

                foreach (string subrole in subroles)
                    allRoleNames.Add($"{contextRolePrefix}/{principalRoleName}#{subrole}");
            }

            return allRoleNames;
        }

        #endregion
    }
}
