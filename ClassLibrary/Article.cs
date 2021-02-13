using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ClassLibrary {
    public struct Article {
        public string SCHEMA_LOCATION;
        public XAttribute SCHEMA_LOCATION_ATTRIBUTE;
        public string XMLNS_XSI;
        public XAttribute XMLNS_XSI_ATTRIBUTE;
        public string LOCALE;
        public string XMLNS;
        public XNamespace XMLNS_NAMESPACE;
        public string DatePublished { get; }
        public string DateSubmitted { get; }
        public string Stage { get; }
        public int AccessStatus { get; }
        public string SectionRef { get; }
        public string Id { get; }
        public string FileName { get; }
        public string Title { get; }
        public string Prefix { get; }
        public string Abstract { get; }
        public string LicenseURL { get; }
        public List<Author> Authors { get; }
        public int Seq { get; }
        public Galley Galley { get; }
        public Identifiaction Identifiaction { get; }
        public string Pages { get; }
        public string[] Keywords;
        public Article(XElement el) {
            SCHEMA_LOCATION = "http://pkp.sfu.ca native.xsd";
            XMLNS_XSI = "http://www.w3.org/2001/XMLSchema-instance";
            SCHEMA_LOCATION_ATTRIBUTE =
                new XAttribute(XNamespace.Get(XMLNS_XSI) + "schemaLocation", SCHEMA_LOCATION);
            XMLNS_XSI_ATTRIBUTE = new XAttribute(XNamespace.Xmlns + "xsi", XMLNS_XSI);

            // W plikach wynikowych nie ma metainformacji związanych z
            // językiem polskim (elementy z atrybutem locale="pl_PL") - rozumiem to jako, że locale jest zawsze en_US
            LOCALE = "en_US";
            XMLNS = "http://pkp.sfu.ca";
            XMLNS_NAMESPACE = XMLNS; 
            DatePublished = el.Element("date_published").Value;
            DateSubmitted = el.Parent.Parent.Element("date_published").Value;
            Stage = el.Parent.Parent.Attribute("published").Value == "true" ? "published" : "production";
            AccessStatus = el.Parent.Elements("open_access").Count() == 1 ? 1 : 0;
            SectionRef = el.Parent.Element("abbrev").Value;
            var fileNameParts = el.Descendants("remote").First().Attribute("src").Value.Split('-');
            FileName = string.Join("-", fileNameParts.TakeLast(2));
            Id = string.Join("", el.Descendants("remote").First().Attribute("src").Value.Split('-').TakeLast(2));
            var locale = LOCALE;
            var title = el.Elements("title").Where(e => e.Attribute("locale").Value == locale).First().Value;
            var titleParts = title.Split(' ');
            if(titleParts[0] == "The" || titleParts[0] == "A" || titleParts[0] == "An") {
                Prefix = titleParts[0];
                Title = string.Join(" ", titleParts.Skip(1));
            } else {
                Title = title;
                Prefix = "";
            }
            Abstract = el.Elements("abstract").Where(e => e.Attribute("locale").Value == locale).First().Value;
            LicenseURL = el.Element("permissions").Element("license_url").Value;
            Authors = new List<Author>();
            foreach(var author in el.Elements("author")) {
                Authors.Add(new Author(author, LOCALE));
            }
            Galley = new Galley(el.Element("galley"), Stage == "published");
            Seq = 1;
            Identifiaction = new Identifiaction(
                int.Parse(el.Parent.Parent.Element("year").Value),
                int.Parse(el.Parent.Parent.Element("volume").Value),
                el.Parent.Parent.Element("number").Value);
            Pages = el.Element("pages").Value;

            Keywords = el.Descendants("subject").Any() ? 
                el.Descendants("subject")
                    .Where(el => el.Attribute("locale").Value == locale)
                    .First().Value.Split("; ") : new string[0];
        }

        public XElement ToXElement() {
            var locale = LOCALE;
            var elements = new List<XElement>() {
                new XElement(
                    XMLNS_NAMESPACE + "id",
                    new XAttribute("advice", "ignore"),
                    new XAttribute("type", "internal"),
                    Id
                ),
                new XElement(
                    XMLNS_NAMESPACE + "title",
                    new XAttribute("locale", LOCALE),
                    Title
                ),
                
            };
            if(Prefix != "") {
                elements.Add(
                    new XElement(
                        XMLNS_NAMESPACE + "Prefix",
                        new XAttribute("locale", LOCALE),
                        Prefix
                    )
                );
            }
            if(Abstract != "") {
                elements.Add(
                    new XElement(
                        XMLNS_NAMESPACE + "abstract",
                        new XAttribute("locale", LOCALE),
                        Abstract
                    )
                );
            }
            if(LicenseURL != "") {
                elements.Add(
                    new XElement(
                        XMLNS_NAMESPACE + "licenseUrl",
                        LicenseURL
                    )
                );
            }
            var xmlnsnamespace = XMLNS_NAMESPACE;
            if(Keywords.Length != 0) {
                elements.Add(
                    new XElement(
                        XMLNS_NAMESPACE + "keywords",
                        new XAttribute("locale", LOCALE),
                        Keywords.Select(el => new XElement(
                            xmlnsnamespace + "keyword",
                            el
                        )).ToArray()
                    )
                );
            }
            var xNameSpace = XMLNS_NAMESPACE;
            if(Authors.Count() != 0) {
                elements.Add(
                    new XElement(
                        XMLNS_NAMESPACE + "authors",
                        SCHEMA_LOCATION_ATTRIBUTE,
                        XMLNS_XSI_ATTRIBUTE,
                        Authors.Select(el => el.ToXElement(locale, xNameSpace)).ToArray()
                    )
                );
            }
            elements.Add(
                Galley.ToXElement(locale, XMLNS_NAMESPACE, SCHEMA_LOCATION_ATTRIBUTE, XMLNS_XSI_ATTRIBUTE, Stage == "published")
            );
            
            elements.Add(
                Identifiaction.ToXElement(XMLNS_NAMESPACE)
            );
            elements.Add(
                new XElement(XMLNS_NAMESPACE + "pages", Pages)
            );
            return new XElement(
                XMLNS_NAMESPACE + "article",
                SCHEMA_LOCATION_ATTRIBUTE,
                new XAttribute("access_status", AccessStatus),
                new XAttribute("seq", Seq),
                new XAttribute("section_ref", SectionRef),
                new XAttribute("date_published", DatePublished),
                new XAttribute("stage", Stage),
                new XAttribute("date_submitted", DateSubmitted),
                new XAttribute("locale", locale),
                XMLNS_XSI_ATTRIBUTE,
                elements.ToArray()
            );
        }
    }
}
