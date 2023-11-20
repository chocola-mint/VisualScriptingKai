using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CHM.VisualScriptingPlus.Editor
{
    public static class StringUtility
    {
        // https://stackoverflow.com/questions/32628511/fuzzy-match-by-regular-expression
        public static string FuzzyMatchPattern(string inputWithWhitespaces)
        {
            StringBuilder result = new();
            bool isFirst = true;
            foreach(var token in inputWithWhitespaces.Split(' ', System.StringSplitOptions.RemoveEmptyEntries))
            {
                if(!isFirst)
                    result.Append('|');
                result.Append(@"(?=.*\b");
                result.Append(token);
                result.Append(@"\b).*");
            }
            return result.ToString();
        }
        // TODO: Pick better name. This is just used to sanitize input to graph queries.
        public static string MakeAlphanumeric(this string str)
        {
            char[] arr = str.Where(c => char.IsLetterOrDigit(c) 
            || char.IsWhiteSpace(c)
            || c == ':').ToArray(); 
            return new string(arr);
        }
    }
}
