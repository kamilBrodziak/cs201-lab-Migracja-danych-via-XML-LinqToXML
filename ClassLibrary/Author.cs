using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ClassLibrary {
    public struct Author {
        public string GivenName { get; }
        public string FamilyName { get; }
        public string Affilation { get; }
        public string Country { get; }
        public string Email { get; }
        public bool IncludeInBrowse { get; }
        public bool PrimaryContact { get; }
        public Author(XElement el, string locale) {
            GivenName = el.Element("firstname").Value;
            FamilyName = el.Element("lastname").Value;
            var affiliation = el.Elements("affiliation").Where(e => e.Attribute("locale").Value == locale);
            Affilation = (affiliation.Count() == 1) ? affiliation.First().Value : "";
            Country = el.Elements("country").Any() ? el.Element("country").Value : "";
            Email = el.Elements("email").Any() ? el.Element("email").Value : "";
            IncludeInBrowse = true;
            PrimaryContact = el.Attributes("primary_contact").Any();
        }

        public XElement ToXElement(string locale, XNamespace xNameSpace) {
            var elements = new List<XElement>() {
                new XElement(xNameSpace + "givenname", new XAttribute("locale", locale), GivenName),
                new XElement(xNameSpace + "familyname", new XAttribute("locale", locale), FamilyName)
            };
            
            if(Affilation != "") {
                elements.Add(
                    new XElement(xNameSpace + "affiliation", new XAttribute("locale", locale), Affilation)
                );
            }
            if(Country != "") {
                elements.Add(
                    new XElement(xNameSpace + "country", new XAttribute("locale", locale), Country)
                );
            }
            if(Email != "") {
                elements.Add(
                    new XElement(xNameSpace + "email", new XAttribute("locale", locale), Email)
                );
            }
            var author = new XElement(
                xNameSpace + "author",
                new XAttribute("user_group_ref", "Author"),
                new XAttribute("include_in_browse", IncludeInBrowse),
                elements.ToArray()
            );
            if(PrimaryContact) {
                author.SetAttributeValue("primary_contact", true);
            }

            return author;
        }
    }
}
