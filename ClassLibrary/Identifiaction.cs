using System.Collections.Generic;
using System.Xml.Linq;

namespace ClassLibrary {
    public struct Identifiaction {
        public int Volume { get; }
        public int Number { get; }
        public int Year { get; }
        public Identifiaction(int year, int volume, int number) {
            Volume = volume;
            Number = number;
            Year = year;
        }

        public Identifiaction(int year) : this(year, -1, -1) { }
        public Identifiaction(int year, int volume) : this(year, volume, -1) { }


        public XElement ToXElement(XNamespace xNameSpace) {
            var elements = new List<XElement>();
            
            if(Volume != -1) {
                elements.Add(
                    new XElement(xNameSpace + "volume", Volume)
                );
            }
            if(Number != -1) {
                elements.Add(
                    new XElement(xNameSpace + "number", Number)
                );
            }

            if(Year != -1) {
                elements.Add(
                    new XElement(xNameSpace + "year", Year)
                );
            }
            return new XElement(
                xNameSpace + "issue_identification",
                elements.ToArray()
            );
        }
    }

    
}
