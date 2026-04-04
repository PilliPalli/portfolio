# MeinPortfolio
Interaktives Terminal-Portfolio von **Moritz Kreis** — gebaut mit Blazor WebAssembly (.NET 8).
Besucher können das Portfolio klassisch durchscrollen oder über ein integriertes Terminal mit Unix-ähnlichen Befehlen navigieren.
## Live
Gehostet auf GitHub Pages via automatischem Deployment (`main` → `gh-pages`).
## Tech-Stack
| Kategorie | Technologie |
|---|---|
| Framework | Blazor WebAssembly (.NET 8) |
| Sprache | C# |
| Styling | CSS3 (Custom Properties, responsive) |
| Interop | JavaScript (jsPDF, Terminal-Input) |
| Hosting | GitHub Pages |
| CI/CD | GitHub Actions |
## Projektstruktur
```
MeinPortfolio/
├── Components/
│   ├── SectionIntro.razor(.cs)   # Hero-Bereich mit Terminal + optionalem CodeGen-Panel
│   ├── SectionAbout.razor        # Über mich
│   ├── SectionSkills.razor       # Fähigkeiten & Technologien
│   ├── SectionProjects.razor     # Projekte
│   ├── SectionContact.razor      # Kontakt
│   ├── TopNav.razor              # Navigation + Sprachwechsel
│   └── ProjectCard.razor         # Wiederverwendbare Projektkarte
├── Models/
│   └── Commands/                 # Terminal-Befehle (help, cd, ls, cat, cv, ...)
├── Services/
│   ├── CommandService.cs         # Befehlsregistrierung & -ausführung
│   ├── LanguageService.cs        # Deutsch/Englisch Umschaltung
│   ├── NavigationService.cs      # Sektions-Navigation
│   ├── VirtualFileSystem.cs      # Virtuelles Dateisystem für Terminal
│   └── FeatureFlags.cs           # Feature Toggle (z.B. AI ein/aus)
├── Pages/
│   └── Index.razor               # Hauptseite, komponiert alle Sections
├── wwwroot/
│   ├── css/app.css               # Globale Styles
│   ├── js/app.js                 # JS-Interop (Terminal, Downloads)
│   ├── cv/                       # CV-PDFs (DE/EN)
│   ├── appsettings.json          # Konfiguration inkl. Feature Flags
│   └── config.json               # API-Endpunkt-Konfiguration
├── Program.cs                    # Entry Point & DI-Setup
└── .github/workflows/deploy.yml  # CI/CD Pipeline
```
## Terminal-Befehle
| Befehl | Beschreibung |
|---|---|
| `help` | Verfügbare Befehle anzeigen |
| `cd <section>` | Zu einer Sektion navigieren (about, projects, contact, home) |
| `ls` | Dateien in aktueller Sektion auflisten |
| `cat <datei>` | Dateiinhalt anzeigen |
| `pwd` | Aktuelle Sektion anzeigen |
| `cv` | Lebenslauf herunterladen |
| `whoami` | Kurzinfo anzeigen |
| `date` | Aktuelles Datum anzeigen |
| `clear` | Terminal leeren |
| `funfact` | Zufälliger Fun Fact |
| `codegen` | Code generieren via API *(nur wenn AI aktiviert)* |
## Feature Toggle
Die AI-Funktionalität (Code-Generator) kann zentral ein-/ausgeschaltet werden:
**`wwwroot/appsettings.json`:**
```json
{
  "Features": {
    "AiEnabled": false
  }
}
```
- `false` (Standard): Klassisches Portfolio ohne AI-Features
- `true`: CodeGen-Tab, `codegen`-Befehl und API-Anbindung aktiv
## Lokal starten
**Voraussetzungen:** .NET 8 SDK
```bash
cd MeinPortfolio/MeinPortfolio
dotnet run
```
Die App startet unter `http://localhost:5143`.
## Build
```bash
cd MeinPortfolio/MeinPortfolio
dotnet build
```
## Deployment
Automatisch via GitHub Actions bei Push auf `main`:
1. `dotnet publish` erstellt die Release-Artefakte
2. `wwwroot/` wird auf den `gh-pages`-Branch deployed
3. GitHub Pages serviert die statische Blazor WASM App
Manuelles Deployment ist jederzeit über `workflow_dispatch` möglich.
## Sprachen
Die gesamte Website ist zweisprachig (Deutsch/Englisch). Umschaltung über den Button in der Navigation.
