using Portfolio.Models;

namespace Portfolio.Tests.Models;

public class CommandResultTests
{
    [Fact]
    public void Constructor_SetupsCorrectly()
    {
        var result = new CommandResult("test output");

        Assert.Equal("test output", result.Output);
    }

    [Fact]
    public void Constructor_DefaultIsHtmlToFalse()
    {
        var result = new CommandResult("text");

        Assert.False(result.IsHtml);
    }

    [Fact]
    public void Constructor_DefaultsShouldClearToFalse()
    {
        var result = new CommandResult("text");

        Assert.False(result.ShouldClear);
    }

    [Fact]
    public void Constructor_WithIsHtml_SetsFlag()
    {
        var result = new CommandResult("<b>bold</b>", isHtml: true);

        Assert.True(result.IsHtml);
    }

    [Fact]
    public void Constructor_WithShouldClear_SetsFlag()
    {
        var result = new CommandResult("", shouldClear: true);

        Assert.True(result.ShouldClear);
    }

    [Fact]
    public void FromText_CreatesPlainTextResult()
    {
        var result = CommandResult.FromText("text");

        Assert.Equal("text", result.Output);
        Assert.False(result.IsHtml);
        Assert.False(result.ShouldClear);
    }

    [Fact]
    public void FromText_WithEmptyString_CreatesEmptyResult()
    {
        var result = CommandResult.FromText("");

        Assert.Equal("", result.Output);
    }

    [Fact]
    public void Constructor_DefaultsAllFlagsToFalse()
    {
        var line = new OutputLine("text");

        Assert.False(line.IsHtml);
        Assert.False(line.IsError);
        Assert.False(line.IsCommand);
        Assert.False(line.IsSuccess);
    }

    [Fact]
    public void Constructor_CommandLine_SetsIsCommandFlag()
    {
        var line = new OutputLine("$ pwd", isCommand: true);

        Assert.True(line.IsCommand);
        Assert.False(line.IsError);
    }

    [Fact]
    public void Constructor_CommandLine_SetsIsErrorFlag()
    {
        var line = new OutputLine("command not found", isError: true);

        Assert.True(line.IsError);
        Assert.False(line.IsSuccess);
    }

    [Fact]
    public void Constructor_SuccessLine_SetsIsSuccessFlag()
    {
        var line = new OutputLine("done", isSuccess: true);

        Assert.True(line.IsSuccess);
    }

    [Fact]
    public void Constructor_HtmlLine_SetsIsHtmlFlag()
    {
        var line = new CommandResult("<b>bold</b>", isHtml: true);

        Assert.True(line.IsHtml);
    }

    [Fact]
    public void Constructor_MultipleFlags_SetsAllCorrectly()
    {
        var line = new OutputLine("result", isCommand: true, isHtml: true);

        Assert.True(line.IsCommand);
        Assert.True(line.IsHtml);
        Assert.False(line.IsError);
        Assert.False(line.IsSuccess);
    }

    public class OutputLineTests
    {
        [Fact]
        public void Constructor_SetsContentCorrectly()
        {
            var line = new OutputLine("test content");

            Assert.Equal("test content", line.Content);
        }
    }
}