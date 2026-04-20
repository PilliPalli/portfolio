using MeinPortfolio.Models.Commands;

namespace MeinPortfolio.Tests.Models.Commands;

public class ClearCommandTests
{
    private readonly ClearCommand _sut = new();

    [Fact]
    public void Name_IsClear()
    {
        Assert.Equal("clear", _sut.Name);
    }

    [Fact]
    public async Task Execute_ReturnsClearSignal()
    {
        var result = await _sut.ExecuteAsync([]);

        Assert.Equal("clear", result);
    }
}

public class WhoamiCommandTests
{
    private readonly WhoamiCommand _sut = new();

    [Fact]
    public void Name_IsWhoami()
    {
        Assert.Equal("whoami", _sut.Name);
    }

    [Fact]
    public async Task Execute_ReturnsOwnerName()
    {
        var result = await _sut.ExecuteAsync([]);

        Assert.Contains("Moritz Nicola Kreis", result);
    }
}

public class DateCommandTests
{
    private readonly DateCommand _sut = new();

    [Fact]
    public void Name_IsDate()
    {
        Assert.Equal("date", _sut.Name);
    }

    [Fact]
    public async Task Execute_ReturnsNonEmptyDateString()
    {
        var result = await _sut.ExecuteAsync([]);

        Assert.False(string.IsNullOrWhiteSpace(result));
    }

    [Fact]
    public async Task Execute_ReturnsCurrentYear()
    {
        var result = await _sut.ExecuteAsync([]);

        Assert.Contains(DateTime.Now.Year.ToString(), result);
    }
}