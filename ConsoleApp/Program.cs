using ClassLibrary;
using System;
using System.IO;

namespace ConsoleApp {
    class Program {
        static void Main(string[] args) {
            var dir = Directory.GetCurrentDirectory();
            var issuesDir = new FileInfo(dir).Directory.Parent.Parent.FullName;
            var resultDir = Directory.CreateDirectory(Path.Combine(dir, "result"));
            //Console.WriteLine(Path.Combine(issuesDir, "issues.xml"));
            OJSMigration.Migrate(Path.Combine(issuesDir, "issues.xml"), resultDir.FullName);
        }
    }
}
