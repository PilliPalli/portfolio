using MeinPortfolio.Services;
using System.Collections.Generic;

namespace MeinPortfolio.Services
{
    public static class VirtualFileSystem
    {
        private static readonly Dictionary<NavigationSection, Dictionary<string, string>> _fileSystem = new()
        {
            [NavigationSection.About] = new Dictionary<string, string>
            {
                ["bio.txt"] = "Name: Moritz Nicola Kreis\nAlter: 22 Jahre alt\nAbschluss: Staatlich geprüfter Wirtschaftsinformatiker\nErfahrung: Berufseinsteiger\nKenntnisse: C#, Blazor, Git, SQL\nHobbys: Fitness, Freunde treffen"
            },
            [NavigationSection.Projects] = new Dictionary<string, string>
            {
                ["portfolio.txt"] = 
                    "Projekt: Interaktives Terminal-Portfolio\n" +
                    "Beschreibung: Eine Portfolio-Website im Terminal-Stil mit Informationen über meine Person\n" +
                    "Technologien: C#, Blazor WebAssembly",

                ["schwimmbad-management.txt"] = 
                    "Projekt: Schwimmbad-Verwaltung\n" +
                    "Beschreibung: Anwendung zur Verwaltung von Mitgliedern eines fiktiven Schwimmbads inkl. Datenbankanbindung\n" +
                    "Technologien: C#, WPF, MSSQL",

                ["garbage-collection-tool.txt"] = 
                    "Projekt: Garbage Collection Tool\n" +
                    "Beschreibung: Tool zur automatisierten Löschung temporärer Dateien inkl. Scheduler, Login-System und Datenbank\n" +
                    "Technologien: C#, WPF",

                ["code-commenter-tool.txt"] = 
                    "Projekt: Code-Kommentierungstool\n" +
                    "Beschreibung: Anwendung zur automatischen Kommentierung von Quellcode über die ChatGPT-API, konfigurierbar nach Detailtiefe\n" +
                    "Technologien: C#, WPF"
            },
            [NavigationSection.Contact] = new Dictionary<string, string>
            {
                ["email.txt"] = "Email: moritz.nicola.kreis@gmail.com"
            }
        };

        public static Dictionary<string, string> GetFiles(NavigationSection section)
        {
            return _fileSystem.TryGetValue(section, out var files) ? files : new Dictionary<string, string>();
        }

        public static string GetFileContent(NavigationSection section, string filename)
        {
            var files = GetFiles(section);
            return files.TryGetValue(filename, out var content) ? content : null;
        }

        public static bool FileExists(NavigationSection section, string filename)
        {
            var files = GetFiles(section);
            return files.ContainsKey(filename);
        }
    }
}
