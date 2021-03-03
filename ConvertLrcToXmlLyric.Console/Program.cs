using System;
using System.Collections.Generic;

namespace ConvertLrcToXmlLyric.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var lyrics = new Dictionary<string, string>();
            
            foreach (var arg in args)
            {
                var param = arg.Split(':');
                lyrics.Add(param[0], System.IO.File.ReadAllText(param[1]));
            }

            if (lyrics.Count > 0)
            {
                var output = ConvertLrcToXmlLyric.LrcToXmlLyric.Convert(lyrics);
                System.IO.File.WriteAllText("output.xlrc", output);
            }
        }
    }
}