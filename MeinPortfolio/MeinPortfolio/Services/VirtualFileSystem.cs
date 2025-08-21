using MeinPortfolio.Services;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;


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
                    "Technologien: C#, WPF",
                        
                ["pdf-report-generator.txt"] = 
                    "Projekt: PDF-Generator für eine Suchstaffel\n" +
                    "Beschreibung: Webanwendung zur automatischen Erstellung von PDF-Berichten für Einsätze einer Suchhundestaffel. Nutzer erfassen Einsatzdaten über ein Formular, die in einer PostgreSQL-Datenbank gespeichert und anschließend als strukturierte PDF-Berichte generiert werden. Das Tool unterstützt aktiv die Bekämpfung der Afrikanischen Schweinepest in Hessen und Rheinland-Pfalz und wird bereits von mehreren Personen im Einsatz genutzt.\n" +
                    "Technologien: C#, Blazor Server, PostgreSQL"
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
    }
}
