using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMAuth.Extensions
{
    /// <summary>
    /// Class reform all scope values to Facebook fields, to perform user info request.
    /// </summary>
    public class ReformScopeToFields
    {
        private static List<string> fields;

        /// <summary>
        /// Method checks if scope is empty or null, then return nothing;
        /// Else transform sopes to fields values
        /// </summary>
        /// <param name="scope">User Scope</param>
        /// <returns>
        /// Empty string if scope is emty
        ///     Or
        /// Valid fields values, 
        ///     as Http query
        /// </returns>
        public static string Transform(string scope)
        {
            if (string.IsNullOrEmpty(scope))
            {
                return string.Empty;
            }

            fields = new List<string>();
            var scopeArray = scope.Split(" ");

            foreach(string scopeUnit in scopeArray)
            {
                if (scopeUnit.StartsWith("user_"))
                {
                    fields.Add(scopeUnit.Substring(5));
                } else
                {
                    fields.Add(scopeUnit);
                }
            }

            return "&fields=" + string.Join(",", fields);
        }
    }
}
