using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DoctorLight
{
    public static class StringExtensions
    {
        public static List<string> EverythingBetween(this string source, string start, string end)
        {
            var results = new List<string>();

            string pattern = $"{Regex.Escape(start)}(.+?){Regex.Escape(end)}";

            foreach (Match m in Regex.Matches(source, pattern))
            {
                results.Add(m.Groups[1].Value);
            }

            return results;
        }
    }
}