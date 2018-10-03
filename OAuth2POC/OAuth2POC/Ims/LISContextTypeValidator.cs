using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ims
{
    public static class LISContextTypeValidator
    {
        #region public methods

        public static bool CheckContextType(string inputContextType, string contextType)
        {
            return _allContextTypes.Contains(inputContextType) && _allContextTypes.Contains(contextType) && inputContextType == contextType;
        }

        #endregion

        #region context type collection

        private static List<string> _contextTypes = new List<string>
        {
            LISContextType.CourseTemplate,
            LISContextType.CourseOffering,
            LISContextType.CourseSection,
            LISContextType.Group
        };

        #endregion

        #region implementation details

        private static List<string> _allContextTypes = GetAllContextTypes();

        private static List<string> GetAllContextTypes()
        {
            List<string> allContextTypes = new List<string>();

            const string uriPatternPrefix = "http://purl.imsglobal.org/vocab/lis/v2/course#";
            const string urnPatternPrefix = "urn:lti:context-type:ims/lis/";

            foreach (string contextType in _contextTypes)
            {
                // simple name (deprecated) but allowed for backwards compatibility
                allContextTypes.Add(contextType);

                // urn name (deprecated) but allowed for backwards compatibility
                allContextTypes.Add($"{urnPatternPrefix}{contextType}");

                // uri pattern
                allContextTypes.Add($"{uriPatternPrefix}{contextType}");
            }

            return allContextTypes;
        }

        #endregion
    }
}