using System.Collections.Generic;
using MeinPortfolio.Models;

namespace MeinPortfolio.Services
{
    public static class VirtualFileSystem
    {
        private static readonly Dictionary<NavigationSection, Dictionary<string, LocalizedFile>> FileSystem = new()
        {
            [NavigationSection.About] = new Dictionary<string, LocalizedFile>
            {
                ["bio.txt"] = new LocalizedFile(
                    $"Name: Moritz Nicola Kreis\nAlter: {PortfolioProfile.Age} Jahre alt\nAbschluss: Staatlich geprüfter Wirtschaftsinformatiker\nFokus: Backend-Entwicklung mit C#/.NET & Java/Spring Boot\nErfahrung: Software Developer bei Smart InsurTech AG (Jan 2026 – Jun 2026)\nMotivation: Innovative Lösungen entwickeln und mich kontinuierlich in neuen Technologien weiterentwickeln\nInteressen: Norwegisch lernen & skandinavische Kultur, Backpacking & Wandern, persönliche Nebenprojekte sowie American Football",
                    $"Name: Moritz Nicola Kreis\nAge: {PortfolioProfile.Age} years old\nDegree: State-certified Business Informatics Specialist\nFocus: Backend development with C#/.NET & Java/Spring Boot\nExperience: Software Developer at Smart InsurTech AG (Jan 2026 – Jun 2026)\nMotivation: Building innovative solutions and continuously expanding my skills in new technologies\nInterests: Learning Norwegian & Scandinavian culture, backpacking & hiking, personal side projects, and American football"
                ),
                ["skills.txt"] = new LocalizedFile(
                    "Programmiersprachen: C#, Java, SQL, HTML/CSS\n.NET-Ökosystem: Blazor, WPF, Entity Framework, ASP.NET Core\nJava-Ökosystem: Spring Boot, Apache Kafka, Maven, REST APIs, Quarkus\nTools & Datenbanken: Git, IntelliJ IDEA, Visual Studio, JetBrains Rider, MSSQL, PostgreSQL, Docker, Jira",
                    "Languages: C#, Java, SQL, HTML/CSS\n.NET Ecosystem: Blazor, WPF, Entity Framework, ASP.NET Core\nJava Ecosystem: Spring Boot, Apache Kafka, Maven, REST APIs, Quarkus\nTools & Databases: Git, IntelliJ IDEA, Visual Studio, JetBrains Rider, MSSQL, PostgreSQL, Docker, Jira"
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
                    "Projekt: Garbage Collection Tool\nBeschreibung: Tool zur automatisierten Löschung temporärer Dateien inklusive Scheduler, Login-System und Datenbank für Konfigurationsverwaltung.\nTechnologien: C#, WPF, Task Scheduling, MSSQL, Docker",
                    "Project: Garbage Collection Tool\nDescription: Tool for automated deletion of temporary files including scheduler, login system and database for configuration management.\nTechnologies: C#, WPF, Task Scheduling, MSSQL, Docker"
                ),

                ["code-commenter-tool.txt"] = new LocalizedFile(
                    "Projekt: Code-Kommentierungstool\nBeschreibung: Anwendung zur automatischen Kommentierung von Quellcode über die ChatGPT-API, konfigurierbar nach Detailtiefe und Programmiersprache.\nTechnologien: C#, WPF, OpenAI API, File Processing",
                    "Project: Code Commenting Tool\nDescription: Application for automatic commenting of source code via the ChatGPT API, configurable by level of detail and programming language.\nTechnologies: C#, WPF, OpenAI API, File Processing"
                ),

                ["pdf-report-generator.txt"] = new LocalizedFile(
                    "Projekt: PDF-Generator für Suchstaffel\nBeschreibung: Webanwendung zur automatischen Erstellung von PDF-Berichten für Einsätze einer Suchhundestaffel. Unterstützt aktiv die Bekämpfung der Afrikanischen Schweinepest in Hessen und Rheinland-Pfalz.\nTechnologien: C#, Blazor Server, PostgreSQL, PDF Generation",
                    "Project: PDF Generator for Search Squad\nDescription: Web application for automatic creation of PDF reports for search dog squad operations. Actively supports the fight against African Swine Fever in Hesse and Rhineland-Palatinate.\nTechnologies: C#, Blazor Server, PostgreSQL, PDF Generation"
                )
            },

            [NavigationSection.Experience] = new Dictionary<string, LocalizedFile>
            {
                ["smart-insurtech.txt"] = new LocalizedFile(
                    "Unternehmen: Smart InsurTech AG\nPosition: Software Developer\nZeitraum: Januar 2026 – Juni 2026\nBeschreibung: Entwicklung und Wartung von Microservices im Versicherungsumfeld. Schwerpunkte auf Event-Driven Architecture mit Apache Kafka, REST-APIs mit Spring Boot sowie Integration in bestehende Systemlandschaften.\nTechnologien: Java, Spring Boot, Apache Kafka, REST APIs, PostgreSQL, Docker, Git",
                    "Company: Smart InsurTech AG\nPosition: Software Developer\nPeriod: January 2026 – June 2026\nDescription: Development and maintenance of microservices in the insurance domain. Focus on event-driven architecture with Apache Kafka, REST APIs with Spring Boot, and integration into existing system landscapes.\nTechnologies: Java, Spring Boot, Apache Kafka, REST APIs, PostgreSQL, Docker, Git"
                ),
                ["tech-stack.txt"] = new LocalizedFile(
                    "Beruflich eingesetzte Technologien:\n\n  Backend: Java, Spring Boot\n  Messaging: Apache Kafka\n  APIs: REST APIs\n  Datenbanken: PostgreSQL\n  DevOps: Docker\n  Tools: Git, Jira, Confluence",
                    "Technologies used professionally:\n\n  Backend: Java, Spring Boot\n  Messaging: Apache Kafka\n  APIs: REST APIs\n  Databases: PostgreSQL\n  DevOps: Docker\n  Tools: Git, Jira, Confluence"
                ),
                ["education.txt"] = new LocalizedFile(
                    "Ausbildung: Staatlich geprüfter Wirtschaftsinformatiker\nAbschluss: 2025\nSchwerpunkt: Anwendungsentwicklung mit Fokus auf C#/.NET, Datenbanken und Software-Engineering. Abschlussprojekte in Desktop- und Webanwendungen.\nTechnologien: C#, .NET, WPF, SQL, Software Engineering",
                    "Education: State-certified Business Informatics Specialist\nGraduated: 2025\nSpecialization: Application development with a focus on C#/.NET, databases, and software engineering. Final projects in desktop and web applications.\nTechnologies: C#, .NET, WPF, SQL, Software Engineering"
                )
            },

            [NavigationSection.Contact] = new Dictionary<string, LocalizedFile>
            {
                ["email.txt"] = new LocalizedFile(
                    "E-Mail: mail@moritz-kreis.de",
                    "Email: mail@moritz-kreis.de"
                ),
                ["profiles.txt"] = new LocalizedFile(
                    "LinkedIn: https://www.linkedin.com/in/moritz-nicola-kreis\nGitHub: https://github.com/PilliPalli\nLebenslauf: cv/cv_de.pdf",
                    "LinkedIn: https://www.linkedin.com/in/moritz-nicola-kreis\nGitHub: https://github.com/PilliPalli\nCV: cv/cv_en.pdf"
                )
            }
        };

        public static IEnumerable<string> GetFileNames(NavigationSection section)
            => FileSystem.TryGetValue(section, out var files) ? files.Keys : [];

        public static IReadOnlyDictionary<string, LocalizedFile> GetFiles(NavigationSection section)
            => FileSystem.TryGetValue(section, out var files) ? files : new Dictionary<string, LocalizedFile>();

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
