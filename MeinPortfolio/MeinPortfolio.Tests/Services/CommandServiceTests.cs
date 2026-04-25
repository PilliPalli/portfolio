using MeinPortfolio.Models;
using MeinPortfolio.Services;

namespace MeinPortfolio.Tests.Services;

public class CommandServiceTests
{
    private readonly CommandService _sut = new();

    // --- RegisterCommand / GetCommand ---

    [Fact]
    public void RegisterCommand_StoresCommandByLowercaseName()
    {
        var cmd = new FakeCommand();
        _sut.RegisterCommand(cmd);

        var result = _sut.GetCommand("test");

        Assert.NotNull(result);
        Assert.Equal("test", result.Name);
    }

    [Fact]
    public void GetCommand_IsCaseSensitive()
    {
        _sut.RegisterCommand(new FakeCommand());

        Assert.NotNull(_sut.GetCommand("TEST"));
        Assert.NotNull(_sut.GetCommand("test"));
    }

    [Fact]
    public void GetCommand_ReturnsNullForUnknownCommand()
    {
        Assert.Null(_sut.GetCommand("nonexistent"));
    }

    [Fact]
    public void GetCommandNames_ReturnsReisteredCommandsSorted()
    {
        _sut.RegisterCommand(new FakeCommand()); // "test"
        _sut.RegisterCommand(new ThrowingCommand()); // "crash"

        var names = _sut.GetCommandNames().ToList();

        Assert.Equal(new[] { "crash", "test" }, names);
    }

    // --- ExecuteCommandAsync

    [Fact]
    public async Task ExecuteCommandAsync_EasyInput_ReturnsEmptyResult()
    {
        var result = await _sut.ExecuteCommandAsync("");

        Assert.Equal(string.Empty, result.Output);
    }

    [Fact]
    public async Task ExecuteCommandAsnyc_WhiteSpaceInput_ReturnsEmptyResult()
    {
        var result = await _sut.ExecuteCommandAsync("  ");

        Assert.Equal(string.Empty, result.Output);
    }

    [Fact]
    public async Task ExecuteCommandAsync_UnknownCommand_ReturnsNotFoundMessage()
    {
        var result = await _sut.ExecuteCommandAsync("unknown");

        Assert.Contains("Command not found: unknown", result.Output);
        Assert.Contains("help", result.Output);
    }

    [Fact]
    public async Task ExecuteCommandAsync_ParseArgumentsCorrectly()
    {
        var cmd = new FakeCommand();
        _sut.RegisterCommand(cmd);

        await _sut.ExecuteCommandAsync("test arg1 arg2");

        Assert.Equal(new[] { "arg1", "arg2" }, cmd.LastArgs);
    }

    [Fact]
    public async Task ExecuteCommandAsync_IsCaseInsensitive()
    {
        _sut.RegisterCommand(new FakeCommand());

        var result = await _sut.ExecuteCommandAsync("TEST");

        Assert.Equal("test output", result.Output);
    }

    [Fact]
    public async Task ExecuteCommandAsync_KnownCommand_DelegatesToCommand()
    {
        var cmd = new FakeCommand();
        _sut.RegisterCommand(cmd);

        var result = await _sut.ExecuteCommandAsync("test");

        Assert.Equal("test output", result.Output);
    }

    [Fact]
    public async Task ExecuteCommandAsync_CommandThrows_ReturnsErrorMessage()
    {
        var cmd = new ThrowingCommand();
        _sut.RegisterCommand(cmd);

        var result = await _sut.ExecuteCommandAsync("crash");

        Assert.Contains("Error executing command", result.Output);
        Assert.Contains("Something went wrong", result.Output);
    }

    [Fact]
    public async Task ExecuteCommandAsync_TrimsLeadingAndTrailingWhitespace()
    {
        _sut.RegisterCommand(new FakeCommand());

        var result = await _sut.ExecuteCommandAsync("  test  ");

        Assert.Equal("test output", result.Output);
    }

    // --- Command History ---

    [Fact]
    public void GetPreviousCommand_NoHistory_ReturnsEmpty()
    {
        Assert.Equal(string.Empty, _sut.GetPreviousCommand());
    }

    [Fact]
    public async Task GetPreviousCommand_AfterExecution_ReturnsPreviousInput()
    {
        _sut.RegisterCommand(new FakeCommand());
        await _sut.ExecuteCommandAsync("test arg1");

        var prev = _sut.GetPreviousCommand();

        Assert.Equal("test arg1", prev);
    }

    [Fact]
    public async Task GetPreviousCommand_MultipleCalls_NavigatesBackward()
    {
        _sut.RegisterCommand(new FakeCommand());
        await _sut.ExecuteCommandAsync("test first");
        await _sut.ExecuteCommandAsync("test second");
        await _sut.ExecuteCommandAsync("test third");

        Assert.Equal("test third", _sut.GetPreviousCommand());
        Assert.Equal("test second", _sut.GetPreviousCommand());
        Assert.Equal("test first", _sut.GetPreviousCommand());
    }

    [Fact]
    public async Task GetPreviousCommand_AtBeginning_ReturnsEmpty()
    {
        _sut.RegisterCommand(new FakeCommand());
        await _sut.ExecuteCommandAsync("test only");

        _sut.GetPreviousCommand(); // "test only", index now at 0
        var result = _sut.GetPreviousCommand(); // index <= 0, returns empty

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public async Task GetNextCommand_AtEnd_ReturnsEmpty()
    {
        _sut.RegisterCommand(new FakeCommand());
        await _sut.ExecuteCommandAsync("test one");

        // historyIndex is at Count, calling GetNext should return empty
        var result = _sut.GetNextCommand();

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public async Task GetNextCommand_AfterNavigatingBack_MovesForward()
    {
        _sut.RegisterCommand(new FakeCommand());
        await _sut.ExecuteCommandAsync("test first");
        await _sut.ExecuteCommandAsync("test second");

        _sut.GetPreviousCommand(); // "test second"
        _sut.GetPreviousCommand(); // "test first"

        Assert.Equal("test second", _sut.GetNextCommand());
    }

    [Fact]
    public async Task History_AlsoRecordsUnknownCommands()
    {
        await _sut.ExecuteCommandAsync("nonexistent");

        var prev = _sut.GetPreviousCommand();

        Assert.Equal("nonexistent", prev);
    }

    private class FakeCommand : BaseCommand
    {
        public override string Name => "test";
        public override string Description => "A test command";
        public override string Usage => "test [args]";

        public string[] LastArgs { get; private set; } = [];

        public override Task<string> ExecuteAsync(string[] args)
        {
            LastArgs = args;
            return Task.FromResult("test output");
        }
    }

    private class ThrowingCommand : BaseCommand
    {
        public override string Name => "crash";
        public override string Description => "A crashing command";

        public override Task<string> ExecuteAsync(string[] args)
        {
            throw new InvalidOperationException("Something went wrong");
        }
    }
}