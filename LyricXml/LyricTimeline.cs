using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;

namespace LyricXml
{
    public class LyricTimeline
    {
        private const string LyricTimelineTag = "timeline";
        private const string LyricTimelineTimestampAttr = "timestamp";
        private const string LyricTimelineItem = "item";
        private const string LyricTimelineItemLanguageAttr = "language";

        private Dictionary<string, string> _elements;
        
        public Dictionary<string, string> Elements => _elements;

        public TimeSpan Timestamp = TimeSpan.Zero;

        public LyricTimeline()
        {
            _elements = new Dictionary<string, string>();
        }
        
        public static async Task<LyricTimeline> Parse(XmlReader reader)
        {
            if (reader.Name.ToLower() == LyricTimelineTag)
            {
                var timeline = new LyricTimeline();

                while (reader.MoveToNextAttribute()) {
                    switch (reader.Name.ToLower())
                    {
                        case LyricTimelineTimestampAttr:
                            timeline.Timestamp = TimeSpan.Parse(reader.Value);
                            break;
                        default:
                            break;
                    }
                }

                var lang = "origin";
                var awaitingText = false;
                string? text = null;

                void Flush()
                {
                    lang = "origin";
                    awaitingText = false;
                    text = null;
                }
                Flush();

                var endRead = false;
                
                while (!endRead && await reader.ReadAsync())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name.ToLower() == LyricTimelineItem)
                            {
                                while (reader.MoveToNextAttribute()) {
                                    switch (reader.Name.ToLower())
                                    {
                                        case LyricTimelineItemLanguageAttr:
                                            lang = reader.Value;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                awaitingText = true;
                            }
                            break;
                        case XmlNodeType.Text:
                            if (awaitingText)
                            {
                                text = reader.Value;
                            }
                            break;
                        case XmlNodeType.EndElement:
                            if (reader.Name.ToLower() == LyricTimelineTag)
                                endRead = true;
                            else if (reader.Name.ToLower() == LyricTimelineItem)
                            {
                                timeline._elements.Add(lang, text);
                                Flush();
                            }
                            break;
                        default:
                            break;
                    }
                }
                
                return timeline;
            }

            throw new NotSupportedException($"Cannot resolve element: {reader.Name}.");
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(Utils.FirstCharToUpper(LyricTimelineTag));
            writer.WriteAttributeString(Utils.FirstCharToUpper(LyricTimelineTimestampAttr), Timestamp.ToString());

            foreach (var element in Elements)
            {
                writer.WriteStartElement(Utils.FirstCharToUpper(LyricTimelineItem));
                writer.WriteAttributeString(LyricTimelineItemLanguageAttr, element.Key.ToLower());
                writer.WriteString(element.Value);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }
}