using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Amccloy.MusicBot.Net.Utils
{
    
    public static class ArgumentParseUtils
    {
        /// <summary>
        /// Returns the list of arguments with any args in quotes joined into 1 string
        /// </summary>
        public static string[] ParseArgsWithQuotes(string[] args) 
            => ParseArgsWithQuotes(String.Join(' ', args));

        /// <summary>
        /// Returns the list of arguments with any args in quotes joined into 1 string
        /// </summary>
        public static string[] ParseArgsWithQuotes(string args)
        {
            return Regex.Matches(args, @"[\""].+?[\""]|[^ ]+")
                        .Cast<Match>()
                        .Select(x => x.Value.Trim('"'))
                        .ToArray();
        }
    }
}