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
            var regex = new Regex(@"\{([^\}]+)\}");
            var paramsToReplace = regex.Matches(str);
            for (var i = 0; i < paramsToReplace.Count; i++)
            {
                str = str.Replace(paramsToReplace[i].Value, replacements[i]);
            }
            return str;
        }
    }
}
