using System.Collections.Generic;

namespace MeinPortfolio.Services
{
    public static class VirtualFileSystem
    {
        private static readonly Dictionary<NavigationSection, Dictionary<string, LocalizedFile>> _fileSystem = new()
        {
            [NavigationSection.About] = new Dictionary<string, LocalizedFile>
            {
                ["bio.txt"] = new LocalizedFile(
                    "Name: Moritz Nicola Kreis\nAlter: 23 Jahre alt\nAbschluss: Staatlich geprüfter Wirtschaftsinformatiker\nErfahrung: Berufseinsteiger\nKenntnisse: C#, Blazor, Git, SQL\nInteressen: Norwegisch lernen & skandinavische Kultur, Freunde treffen",
                    "Name: Moritz Nicola Kreis\nAge: 23 years old\nDegree: State-certified Business Informatics Specialist\nExperience: Entry level\nSkills: C#, Blazor, Git, SQL\nInterests: learning norwegian & scandinavian culture, meeting friends"
                )
            },

            [NavigationSection.Projects] = new Dictionary<string, LocalizedFile>
            {
                ["portfolio.txt"] = new LocalizedFile(
                    "Projekt: Interaktives Terminal-Portfolio\nBeschreibung: Eine Portfolio-Website mit integriertem Terminal und Informationen über meine Person. Nutzer können durch Unix-ähnliche Befehle navigieren und Inhalte erkunden.\nTechnologien: C#, Blazor WebAssembly, HTML/CSS, JavaScript",
                    "Project: Interactive Terminal Portfolio\nDescription: A portfolio website with an integrated terminal providing information about myself. Users can navigate and explore content through Unix-like commands.\nTechnologies: C#, Blazor WebAssembly, HTML/CSS, JavaScript"
                ),
                
                ["schwimmbad-management.txt"] = new LocalizedFile(
                    "Projekt: Schwimmbad-Verwaltung\nBeschreibung: Anwendung zur Verwaltung von Mitgliedern eines fiktiven Schwimmbads inklusive Datenbankanbindung und Mitgliederverwaltung.\nTechnologien: C#, WPF, MSSQL, Entity Framework",
                    "Project: Swimming Pool Management\nDescription: Application for managing members of a fictional swimming pool including database connection and member management.\nTechnologies: C#, WPF, MSSQL, Entity Framework"
                ),

                ["garbage-collection-tool.txt"] = new LocalizedFile(
                    "Projekt: Garbage Collection Tool\nBeschreibung: Tool zur automatisierten Löschung temporärer Dateien inklusive Scheduler, Login-System und Datenbank für Konfigurationsverwaltung.\nTechnologien: C#, WPF, Task Scheduling, Database",
                    "Project: Garbage Collection Tool\nDescription: Tool for automated deletion of temporary files including scheduler, login system and a database for configuration management.\nTechnologies: C#, WPF, Task Scheduling, Database"
                ),

                ["code-commenter-tool.txt"] = new LocalizedFile(
                    "Projekt: Code-Kommentierungstool\nBeschreibung: Anwendung zur automatischen Kommentierung von Quellcode über die ChatGPT-API, konfigurierbar nach Detailtiefe und Programmiersprache.\nTechnologien: C#, WPF, OpenAI API, File Processing",
                    "Project: Code Commenting Tool\nDescription: Application for automatic commenting of source code via the ChatGPT API, configurable by level of detail and programming language.\nTechnologies: C#, WPF, OpenAI API, File Processing"
                ),

                ["pdf-report-generator.txt"] = new LocalizedFile(
                    "Projekt: PDF-Generator für eine Suchstaffel\nBeschreibung: Webanwendung zur automatischen Erstellung von PDF-Berichten für Einsätze einer Suchhundestaffel. Nutzer erfassen Einsatzdaten über ein Formular, die in einer PostgreSQL-Datenbank gespeichert und anschließend als strukturierte PDF-Berichte generiert werden. Unterstützt aktiv die Bekämpfung der Afrikanischen Schweinepest in Hessen und Rheinland-Pfalz.\nTechnologien: C#, Blazor Server, PostgreSQL, PDF Generation",
                    "Project: PDF Generator for a Search Squad\nDescription: Web application for the automatic creation of PDF reports for search dog squad operations. Users enter mission data via a form, which is stored in PostgreSQL and rendered as structured PDFs. Actively supports the fight against African Swine Fever in Hesse and Rhineland-Palatinate.\nTechnologies: C#, Blazor Server, PostgreSQL, PDF Generation"
                )
            },

            [NavigationSection.Contact] = new Dictionary<string, LocalizedFile>
            {
                ["email.txt"] = new LocalizedFile(
                    "E-Mail: mail@moritz-kreis.de",
                    "Email: mail@moritz-kreis.de"
                )
            }
        };

        public static IEnumerable<string> GetFileNames(NavigationSection section)
            => _fileSystem.TryGetValue(section, out var files) ? files.Keys : [];

        public static IReadOnlyDictionary<string, LocalizedFile> GetFiles(NavigationSection section)
            => _fileSystem.TryGetValue(section, out var files) ? files : new Dictionary<string, LocalizedFile>();

        public static string? GetFileContent(NavigationSection section, string filename, LanguageService languageService)
        {
            var files = GetFiles(section);
            if (files.TryGetValue(filename, out var localizedFile))
            {
                return languageService.GetText(localizedFile.German, localizedFile.English);
            }
            return null;
        }
    }
}
