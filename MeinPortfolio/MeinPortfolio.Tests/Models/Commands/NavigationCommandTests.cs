using MeinPortfolio.Models.Commands;
using MeinPortfolio.Services;
using Microsoft.JSInterop;
using NSubstitute;

namespace MeinPortfolio.Tests.Models.Commands;

public class CdCommandTests
{
    private readonly NavigationService _navService = new();
    private readonly CdCommand _sut;

    public CdCommandTests()
    {
        _sut = new CdCommand(_navService);
    }

    [Fact]
    public async Task Execute_NoArgs_ReturnsCurrentLocactionHint()
    {
        var result = await _sut.ExecuteAsync([]);

        Assert.Contains("Current location:", result);
        Assert.Contains("cd [section]", result);
    }

    [Theory]
    [InlineData("about", NavigationSection.About)]
    [InlineData("projects", NavigationSection.Projects)]
    [InlineData("contact", NavigationSection.Contact)]
    public async Task Execute_ValidSection_NavigatesToSection(string input, NavigationSection expected)
    {
        await _sut.ExecuteAsync([input]);

        Assert.Equal(expected, _navService.CurrentSection);
    }

    [Fact]
    public async Task Execute_Home_NavigatesToHome()
    {
        _navService.NavigateTo(NavigationSection.Contact);

        await _sut.ExecuteAsync(["home"]);

        Assert.Equal(NavigationSection.Home, _navService.CurrentSection);
    }

    [Fact]
    public async Task Execute_Tilde_NavigatesToHome()
    {
        _navService.NavigateTo(NavigationSection.Projects);

        await _sut.ExecuteAsync(["~"]);

        Assert.Equal(NavigationSection.Home, _navService.CurrentSection);
    }

    [Fact]
    public async Task Execute_IsCaseInsensitive()
    {
        await _sut.ExecuteAsync(["AbOuT"]);

        Assert.Equal(NavigationSection.About, _navService.CurrentSection);
    }

    [Fact]
    public async Task Execute_ValidSection_ReturnsConfirmationMessage()
    {
        var result = await _sut.ExecuteAsync(["about"]);

        Assert.Contains("Navigated to", result);
    }
}

public class PwdCommandTests
{
    private readonly NavigationService _navService = new();
    private readonly PwdCommand _sut;

    public PwdCommandTests()
    {
        _sut = new PwdCommand(_navService);
    }

    [Fact]
    public async Task Execute_AtHome_ReturnsTilde()
    {
        var result = await _sut.ExecuteAsync([]);

        Assert.Equal("~", result);
    }

    [Fact]
    public async Task Execute_InAbout_ReturnsAboutPath()
    {
        _navService.NavigateTo(NavigationSection.About);

        var result = await _sut.ExecuteAsync([]);

        Assert.Equal("~/about", result);
    }

    [Fact]
    public async Task Execute_InProjects_ReturnsProjectsPath()
    {
        _navService.NavigateTo(NavigationSection.Projects);

        var result = await _sut.ExecuteAsync([]);

        Assert.Equal("~/projects", result);
    }
}

public class LsCommandTests
{
    private readonly NavigationService _navService = new();
    private readonly LsCommand _sut;

    public LsCommandTests()
    {
        _sut = new LsCommand(_navService);
    }

    [Fact]
    public async Task Execute_AtHome_ListsSections()
    {
        var result = await _sut.ExecuteAsync([]);

        Assert.Contains("about", result);
        Assert.Contains("projects", result);
        Assert.Contains("contact", result);
    }

    [Fact]
    public async Task Execute_InAbout_ListsFiles()
    {
        _navService.NavigateTo(NavigationSection.About);

        var result = await _sut.ExecuteAsync([]);

        Assert.Contains("bio.txt", result);
    }

    [Fact]
    public async Task Execute_InProjects_ListsProjectFiles()
    {
        _navService.NavigateTo(NavigationSection.Projects);

        var result = await _sut.ExecuteAsync([]);

        Assert.Contains("portfolio.txt", result);
        Assert.Contains("schwimmbad-management.txt", result);
    }

    [Fact]
    public async Task Execute_InContact_ListsContactFiles()
    {
        _navService.NavigateTo(NavigationSection.Contact);

        var result = await _sut.ExecuteAsync([]);

        Assert.Contains("email.txt", result);
    }
}

public class HomeCommandTests
{
    private readonly NavigationService _navService = new();
    private readonly HomeCommand _sut;

    public HomeCommandTests()
    {
        _sut = new HomeCommand(_navService);
    }

    [Fact]
    public async Task Execute_NavigatesToHome()
    {
        _navService.NavigateTo(NavigationSection.About);

        await _sut.ExecuteAsync([]);

        Assert.Equal(NavigationSection.Home, _navService.CurrentSection);
    }

    [Fact]
    public async Task Execute_ReturnsConfirmation()
    {
        _navService.NavigateTo(NavigationSection.About);

        var result = await _sut.ExecuteAsync([]);

        Assert.Equal("Navigated to Home section.", result);
    }
}

public class CatCommandTests
{
    private readonly LanguageService _langService;
    private readonly NavigationService _navService = new();
    private readonly CatCommand _sut;

    public CatCommandTests()
    {
        var jsRuntime = Substitute.For<IJSRuntime>();
        _langService = new LanguageService(jsRuntime);
        _sut = new CatCommand(_navService, _langService);
    }

    [Fact]
    public async Task Execute_NoArgs_ReturnsUsageHint()
    {
        var result = await _sut.ExecuteAsync([]);

        Assert.Contains("Usage:", result);
        Assert.Contains("cat", result);
    }

    [Fact]
    public async Task Execute_AtHome_ReturnsCannotDisplayError()
    {
        var result = await _sut.ExecuteAsync(["bio.txt"]);

        Assert.Contains("cannot display directory contents", result);
    }

    [Fact]
    public async Task Execute_ExistingFile_ReturnsContent()
    {
        _navService.NavigateTo(NavigationSection.About);

        var result = await _sut.ExecuteAsync(["bio.txt"]);

        Assert.Contains("Moritz Nicola Kreis", result);
    }

    [Fact]
    public async Task Execute_NonexistentFile_ReturnsNotFoundError()
    {
        _navService.NavigateTo(NavigationSection.About);

        var result = await _sut.ExecuteAsync(["nonexistent.txt"]);

        Assert.Contains("No such file or directory", result);
    }

    [Fact]
    public async Task Execute_RespectsCurrentLanguage()
    {
        _navService.NavigateTo(NavigationSection.About);

        // Default is German
        var germanResult = await _sut.ExecuteAsync(["bio.txt"]);
        Assert.Contains("Alter:", germanResult);

        // Switch to English
        await _langService.SetLanguageAsync(LanguageType.English);
        var englishResult = await _sut.ExecuteAsync(["bio.txt"]);
        Assert.Contains("Age:", englishResult);
    }

    [Fact]
    public async Task Execute_InProjects_CanReadProjectFiles()
    {
        _navService.NavigateTo(NavigationSection.Projects);

        var result = await _sut.ExecuteAsync(["portfolio.txt"]);

        Assert.Contains("Terminal", result);
    }
}