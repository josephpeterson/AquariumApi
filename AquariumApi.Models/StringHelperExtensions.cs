using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AquariumApi.Models
{
    public static class StringHelperExtensions
    {
        public static string AggregateParams(this string str, params string[] replacements)
        {
            var regex = new Regex(@"/ (?<=\{).+? (?=\})/ g");
            var paramsToReplace = regex.Matches(str);
            foreach(Match match in paramsToReplace)
            {
                str = str.Replace(match.Value, replacements[match.Index]);
            }
            return str;
        }
    }
}
