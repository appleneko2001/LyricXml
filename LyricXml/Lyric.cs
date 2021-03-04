using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using System.Xml;

namespace LyricXml
{
    public class Lyric
    {
        private static XmlReaderSettings _readerSettings = new XmlReaderSettings()
        {
            Async = true,
        };

        private static XmlWriterSettings _writerSettings = new XmlWriterSettings()
        {
            Indent = true,
            Encoding = Encoding.UTF8
        };

        private const int RootMaxAttributesCount = 64;
        private const string RootTag = "lyric";

        private const string Attribute_Title = "title";
        private const string Attribute_Artist = "artist";
        private const string Attribute_Album = "album";
        private const string Attribute_Provider = "provider";
        private const string Attribute_Languages = "languages";

        private Dictionary<string, string> _attributes;
        public Dictionary<string, string> Attributes => _attributes;

        private List<LyricTimeline> _timelines;
        public List<LyricTimeline> Timelines => _timelines;

        public string Title
        {
            get => AttributeGetter(Attribute_Title);
            set => AttributeSetter(Attribute_Title, value);
        }

        public string Artist
        {
            get => AttributeGetter(Attribute_Artist);
            set => AttributeSetter(Attribute_Artist, value);
        }

        public string Album
        {
            get => AttributeGetter(Attribute_Album);
            set => AttributeSetter(Attribute_Album, value);
        }

        public string Provider
        {
            get => AttributeGetter(Attribute_Provider);
            set => AttributeSetter(Attribute_Provider, value);
        }

        public Lyric()
        {
            _attributes = new Dictionary<string, string>();
            _timelines = new List<LyricTimeline>();
        }
        
        public Lyric(string xmlData)
        {
            using (var strReader = new StringReader(xmlData))
            {
                using (var reader = XmlReader.Create(strReader, _readerSettings))
                {
                    _attributes = new Dictionary<string, string>();
                    _timelines = new List<LyricTimeline>();
                    ParseProcedure(reader);
                }
            }
        }

        private string AttributeGetter(string key)
        {
            if (Attributes.ContainsKey(key.ToLower()))
                return Attributes[key.ToLower()];
            return null;
        }

        private void AttributeSetter(string key, string value)
        {
            if (Attributes.ContainsKey(key.ToLower()))
                Attributes[key.ToLower()] = value;
            else
                Attributes.Add(key.ToLower(), value);
        }

        public static async Task<Lyric> Parse(Stream stream)
        {
            using (var reader = XmlReader.Create(stream, _readerSettings))
            {
                var instance = new Lyric();
                await instance.ParseProcedure(reader);
                return instance;
            }
        }

        private async Task ParseProcedure(XmlReader reader)
        {
            var endRead = false;
            while (!endRead && await reader.ReadAsync())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name.ToLower() == RootTag)
                        {
                            while (reader.MoveToNextAttribute()) {
                                _attributes.Add(reader.Name.ToLower(), reader.Value);
                            }
                            break;
                        }
                        else
                        {
                            var timeline = await LyricTimeline.Parse(reader);
                            _timelines.Add(timeline);
                        }
                        break;
                    case XmlNodeType.EndElement:
                        if(reader.Name.ToLower() == RootTag) 
                            endRead = true;
                        break;
                }
            }
        }

        public string ToXml()
        {
            
            using (var stream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(stream, _writerSettings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement(Utils.FirstCharToUpper(RootTag));
                
                    foreach (var attribute in Attributes)
                    {
                        writer.WriteAttributeString(null, attribute.Key, null, attribute.Value);
                    }

                    foreach (var timeline in Timelines)
                    {
                        timeline.WriteXml(writer);
                    }
                
                    writer.WriteEndElement();
                }

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public string[] Languages
        {
            get
            {
                if (!Attributes.ContainsKey(Attribute_Languages))
                    return Array.Empty<string>();
                return Attributes[Attribute_Languages].Split(',');
            }
        }

        public void AddLanguage(string codeName)
        {
            var attr = AttributeGetter(Attribute_Languages);

            if (attr is null)
            {
                AttributeSetter(Attribute_Languages, codeName);
                return;
            }

            if(attr.Length > 0)
                attr += ",";
            attr += codeName;
            
            AttributeSetter(Attribute_Languages, attr);
        }
    }
}