using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ClassLibrary {
    public struct Galley {
        public int SEQ;
        public int Id { get; }
        public string Name { get; }
        public string Remote { get; }
        public bool Approved { get; }

        public Galley(XElement el, bool approved) {
            SEQ = 0;
            Name = el.Elements("label").Any() ? el.Element("label").Value : "";
            Remote = el.Descendants("remote").Any() ? el.Descendants("remote").First().Attribute("src").Value : "";
            var parts = Remote.Split('-');
            Id = Remote != "" ? int.Parse(parts[^1]) : -1;
            Approved = approved;
        }

        public XElement ToXElement(string locale, XNamespace xNameSpace, XAttribute schemaLocation, XAttribute xmlnsxsi, bool approved) {
            var elements = new List<XElement>();
            if(Id != -1) {
                elements.Add(
                    new XElement(
                        xNameSpace + "id", 
                        new XAttribute("locale", "ignore"), 
                        new XAttribute("type", "internal"),
                        Id
                    )
                );
            }
            if(Name != "") {
                elements.Add(
                    new XElement(xNameSpace + "name", new XAttribute("locale", locale), Name)
                );
            }
            elements.Add(
                    new XElement(xNameSpace + "seq", SEQ)
            );
            if(Remote != "") {
                elements.Add(
                    new XElement(xNameSpace +"remote", new XAttribute("src", Remote))
                );
            }
            return new XElement(
                xNameSpace + "article_galley",
                schemaLocation,
                xmlnsxsi,
                new XAttribute("approved", approved),
                elements.ToArray()
            );
        }
    }
}
