using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ClassLibrary {
    public static class OJSMigration {
        public static void Migrate(string filePath, string directory) {
            var file = XDocument.Load(filePath);
            foreach(var issue in file.Descendants("issue")) {
                foreach(var article in issue.Element("section").Descendants("article")) {
                    var articleObj = new Article(article);
                    var document = new XDocument(
                        new XDeclaration("1.0", null, null), articleObj.ToXElement()
                    );
                    Console.WriteLine(Path.Combine(directory, articleObj.FileName + ".xml"));
                    document.Save(Path.Combine(directory, articleObj.FileName + ".xml"));

                }

            }
        }
    }
}
