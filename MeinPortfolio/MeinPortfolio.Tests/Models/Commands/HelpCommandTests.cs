using MeinPortfolio.Models.Commands;
using MeinPortfolio.Services;

namespace MeinPortfolio.Tests.Models.Commands;

public class HelpCommandTests
{
    private readonly CommandService _commandService = new();
    private readonly HelpCommand _sut;

    public HelpCommandTests()
    {
        _sut = new HelpCommand(_commandService);

        _commandService.RegisterCommand(_sut);
        _commandService.RegisterCommand(new ClearCommand());
        _commandService.RegisterCommand(new WhoamiCommand());
    }

    [Fact]
    public async Task Execute_NoArgs_ListsAllCommands()
    {
        var result = await _sut.ExecuteAsync([]);

        Assert.Contains("Available commands:", result);
        Assert.Contains("help", result);
        Assert.Contains("clear", result);
        Assert.Contains("whoami", result);
    }

    [Fact]
    public async Task Execute_NoArgs_ShowsDescription()
    {
        var result = await _sut.ExecuteAsync([]);

        Assert.Contains("Clear the terminal screen", result);
    }

    [Fact]
    public async Task Execute_WithValidCommand_ShowsCommandDetails()
    {
        var result = await _sut.ExecuteAsync(["clear"]);

        Assert.Contains("Command: clear", result);
        Assert.Contains("Description:", result);
        Assert.Contains("Usage:", result);
    }

    [Fact]
    public async Task Execute_WithUnknownCommand_ReturnsNotFoundMessage()
    {
        var result = await _sut.ExecuteAsync(["nonexistent"]);

        Assert.Contains("not found", result);
        Assert.Contains("help", result);
    }

    [Fact]
    public async Task Execute_CommandLookup_IsCaseSensitive()
    {
        var result = await _sut.ExecuteAsync(["CleAr"]);

        Assert.Contains("Command: clear", result);
    }

    [Fact]
    public async Task Execute_NoArgs_ContainsUsageHint()
    {
        var result = await _sut.ExecuteAsync([]);

        Assert.Contains("help [command]", result);
    }
}