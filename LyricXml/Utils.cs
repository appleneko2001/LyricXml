using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace LyricXml
{
    public class Utils
    {
        public static string FirstCharToUpper(string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => input.First().ToString().ToUpper() + input.Substring(1)
            };

        public static bool StartOrEndWith(string target, string find)
        {
            return target.StartsWith(find) && target.EndsWith(find);
        }
    }
}