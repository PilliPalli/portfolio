using MeinPortfolio.Services;
using Microsoft.JSInterop;
using NSubstitute;

namespace MeinPortfolio.Tests.Services;

public class VirtualFileSystemTests
{
    private readonly LanguageService _englishService;
    private readonly LanguageService _germanService;

    public VirtualFileSystemTests()
    {
        var jsRuntime = Substitute.For<IJSRuntime>();
        _germanService = new LanguageService(jsRuntime);

        var jsRuntime2 = Substitute.For<IJSRuntime>();
        _englishService = new LanguageService(jsRuntime2);
        _englishService.SetLanguageAsync(LanguageType.English).GetAwaiter().GetResult();
    }

    // --- GetFileNames ---

    [Fact]
    public void GetFileNames_AboutSection_ContainsBioFile()
    {
        var files = VirtualFileSystem.GetFileNames(NavigationSection.About).ToList();

        Assert.Contains("bio.txt", files);
    }

    [Fact]
    public void GetFileNames_ProjectsSection_ContainsProjectFiles()
    {
        var files = VirtualFileSystem.GetFileNames(NavigationSection.Projects).ToList();

        Assert.Contains("portfolio.txt", files);
        Assert.Contains("schwimmbad-management.txt", files);
        Assert.Contains("garbage-collection-tool.txt", files);
        Assert.Contains("code-commenter-tool.txt", files);
        Assert.Contains("pdf-report-generator.txt", files);
    }

    [Fact]
    public void GetFileNames_ContactSection_ContainsEmailFile()
    {
        var files = VirtualFileSystem.GetFileNames(NavigationSection.Contact).ToList();

        Assert.Contains("email.txt", files);
    }

    [Fact]
    public void GetFileNames_HomeSection_ReturnsEmpty()
    {
        var files = VirtualFileSystem.GetFileNames(NavigationSection.Home).ToList();

        Assert.Empty(files);
    }

    // --- GetFiles ---

    [Fact]
    public void GetFiles_KnownSection_ReturnsNonEmptyDictionary()
    {
        var files = VirtualFileSystem.GetFiles(NavigationSection.About);

        Assert.NotEmpty(files);
    }

    [Fact]
    public void GetFiles_HomeSection_ReturnsEmptyDictionary()
    {
        var files = VirtualFileSystem.GetFiles(NavigationSection.Home);

        Assert.Empty(files);
    }

    // --- GetFileContent ---

    [Fact]
    public void GetFileContent_ExistingFile_ReturnsContent()
    {
        var content = VirtualFileSystem.GetFileContent(NavigationSection.About, "bio.txt", _germanService);

        Assert.NotNull(content);
        Assert.Contains("Moritz Nicola Kreis", content);
    }

    [Fact]
    public void GetFileContent_NonexistentFile_ReturnsNull()
    {
        var content = VirtualFileSystem.GetFileContent(NavigationSection.About, "nonexistent.txt", _germanService);

        Assert.Null(content);
    }

    [Fact]
    public void GetFileContent_WrongSection_ReturnsNull()
    {
        // bio.txt exists in About, not in Projects
        var content = VirtualFileSystem.GetFileContent(NavigationSection.Projects, "bio.txt", _germanService);

        Assert.Null(content);
    }

    // --- Localization ---

    [Fact]
    public void GetFileContent_German_ReturnsGermanContent()
    {
        var content = VirtualFileSystem.GetFileContent(NavigationSection.About, "bio.txt", _germanService);

        Assert.NotNull(content);
        Assert.Contains("Alter:", content); // German field name
    }

    [Fact]
    public void GetFileContent_English_ReturnsEnglishContent()
    {
        var content = VirtualFileSystem.GetFileContent(NavigationSection.About, "bio.txt", _englishService);

        Assert.NotNull(content);
        Assert.Contains("Age:", content); // English field name
    }

    [Fact]
    public void GetFileContent_ContactEmail_SameInBothLanguages()
    {
        var german = VirtualFileSystem.GetFileContent(NavigationSection.Contact, "email.txt", _germanService);
        var english = VirtualFileSystem.GetFileContent(NavigationSection.Contact, "email.txt", _englishService);

        Assert.NotNull(german);
        Assert.NotNull(english);
        Assert.Contains("mail@moritz-kreis.de", german);
        Assert.Contains("mail@moritz-kreis.de", english);
    }
}