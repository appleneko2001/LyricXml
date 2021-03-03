using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace LyricXml
{
    public class Utils
    {
        public static KeyValuePair<string, string> GetAttribute(XmlReader reader, int index)
        {
            var key = reader.GetAttribute(index);
            var value = reader[key];
            return new KeyValuePair<string, string>(key, value);
        }
        
        public static string FirstCharToUpper(string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => input.First().ToString().ToUpper() + input.Substring(1)
            };
    }
}