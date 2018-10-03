using System;
using System.Text;

namespace OAuth2POC
{
    public class Utility
    {
        protected readonly static string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
        /// <summary>
        /// Encode a URL 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string UrlEncode(string value)
        {
            StringBuilder result = new StringBuilder();

            foreach (char symbol in value)
            {
                if (unreservedChars.IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else
                {
                    result.Append(Uri.EscapeDataString(symbol.ToString()));
                }
            }

            return result.ToString();
        }
    }
}