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
                    "Name: Moritz Nicola Kreis\nAlter: 22 Jahre alt\nAbschluss: Staatlich geprüfter Wirtschaftsinformatiker\nErfahrung: Berufseinsteiger\nKenntnisse: C#, Blazor, Git, SQL\nInteressen: Norwegisch lernen & skandinavische Kultur, Freunde treffen",
                    "Name: Moritz Nicola Kreis\nAge: 22 years old\nDegree: State-certified Business Informatics Specialist\nExperience: Entry level\nSkills: C#, Blazor, Git, SQL\nInterests: learning norwegian & scandinavian culture, meeting friends"
                )
            },

            [NavigationSection.Projects] = new Dictionary<string, LocalizedFile>
            {
                ["portfolio.txt"] = new LocalizedFile(
                    "Projekt: Interaktives Terminal-Portfolio\nBeschreibung: Eine Portfolio-Website im Terminal-Stil mit Informationen über meine Person\nTechnologien: C#, Blazor WebAssembly",
                    "Project: Interactive terminal portfolio\nDescription: A terminal-style portfolio website with information about me\nTechnologies: C#, Blazor WebAssembly"
                ),

                ["schwimmbad-management.txt"] = new LocalizedFile(
                    "Projekt: Schwimmbad-Verwaltung\nBeschreibung: Anwendung zur Verwaltung von Mitgliedern eines fiktiven Schwimmbads inkl. Datenbankanbindung\nTechnologien: C#, WPF, MSSQL",
                    "Project: Swimming pool management\nDescription: App to manage members of a fictional pool incl. database integration\nTechnologies: C#, WPF, MSSQL"
                ),

                ["garbage-collection-tool.txt"] = new LocalizedFile(
                    "Projekt: Garbage Collection Tool\nBeschreibung: Tool zur automatisierten Löschung temporärer Dateien inkl. Scheduler, Login-System und Datenbank\nTechnologien: C#, WPF",
                    "Project: Garbage Collection Tool\nDescription: Tool for automated deletion of temp files incl. scheduler, login system and database\nTechnologies: C#, WPF"
                ),

                ["code-commenter-tool.txt"] = new LocalizedFile(
                    "Projekt: Code-Kommentierungstool\nBeschreibung: Anwendung zur automatischen Kommentierung von Quellcode über die ChatGPT-API, konfigurierbar nach Detailtiefe\nTechnologien: C#, WPF",
                    "Project: Code commenting tool\nDescription: App that auto-comments source code via the ChatGPT API, configurable by detail level\nTechnologies: C#, WPF"
                ),

                ["pdf-report-generator.txt"] = new LocalizedFile(
                    "Projekt: PDF-Generator für eine Suchstaffel\nBeschreibung: Webanwendung zur automatischen Erstellung von PDF-Berichten für Einsätze einer Suchhundestaffel. Nutzer erfassen Einsatzdaten über ein Formular, die in einer PostgreSQL-Datenbank gespeichert und anschließend als strukturierte PDF-Berichte generiert werden. Das Tool unterstützt aktiv die Bekämpfung der Afrikanischen Schweinepest in Hessen und Rheinland-Pfalz und wird bereits von mehreren Personen im Einsatz genutzt.\nTechnologien: C#, Blazor Server, PostgreSQL",
                    "Project: PDF report generator for a search unit\nDescription: Web app that generates PDF reports for search dog missions. Users enter mission data via a form, stored in PostgreSQL and rendered as structured PDFs. The tool supports efforts against African swine fever in Hesse and Rhineland-Palatinate and is already in active use.\nTechnologies: C#, Blazor Server, PostgreSQL"
                )
            },

            [NavigationSection.Contact] = new Dictionary<string, LocalizedFile>
            {
                ["email.txt"] = new LocalizedFile(
                    "E-Mail: bewerbung@moritz-kreis.de",
                    "Email: bewerbung@moritz-kreis.de"
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
