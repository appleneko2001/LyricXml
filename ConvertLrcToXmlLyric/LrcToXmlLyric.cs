using System;
using System.Collections.Generic;
using System.Linq;
using LyricXml;
using Opportunity.LrcParser;

namespace ConvertLrcToXmlLyric
{
    public class LrcToXmlLyric
    {
        public static string Convert(Dictionary<string, string> lrcs)
        {
            var lyrics = new Dictionary<string, IParseResult<Line>>();
            foreach (var lrc in lrcs)
            {
                lyrics.Add(lrc.Key, Lyrics.Parse(lrc.Value));
            }

            var xmlLyric = new Lyric();
            foreach (var lyric in lyrics)
            {
                foreach (var line in lyric.Value.Lyrics.Lines)
                {
                    LyricTimeline item;

                    var existsItem = xmlLyric.Timelines.Where(e => e.Timestamp == line.Timestamp);
                    if (existsItem.Any())
                    {
                        item = existsItem.First();
                    }
                    else
                    {
                        item = new LyricTimeline()
                        {
                            Timestamp = line.Timestamp
                        };
                    }

                    item.Elements.Add(lyric.Key, line.Content);

                    if (!xmlLyric.Timelines.Exists(e => e == item))
                        xmlLyric.Timelines.Add(item);
                }
            }

            return xmlLyric.ToXml();
        }
    }
}