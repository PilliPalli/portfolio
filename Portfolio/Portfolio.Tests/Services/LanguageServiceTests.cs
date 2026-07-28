using Portfolio.Services;
using Microsoft.JSInterop;
using NSubstitute;

namespace Portfolio.Tests.Services;

public class LanguageServiceTests
{
    private readonly IJSRuntime _jsRuntime = Substitute.For<IJSRuntime>();
    private readonly LanguageService _sut;

    public LanguageServiceTests()
    {
        _sut = new LanguageService(_jsRuntime);
    }

    // --- Initial State ---

    [Fact]
    public void DefaultLanguage_IsGerman()
    {
        Assert.Equal(LanguageType.German, _sut.CurrentLanguage);
    }

    // --- GetText ---

    [Fact]
    public void GetText_WhenGerman_ReturnsGermanText()
    {
        //  Default is German
        var result = _sut.GetText("Hallo", "Hello");

        Assert.Equal("Hallo", result);
    }

    [Fact]
    public async Task GetText_WhenEnglish_ReturnsEnglishText()
    {
        await _sut.SetLanguageAsync(LanguageType.English);

        var result = _sut.GetText("Hallo", "Hello");

        Assert.Equal("Hello", result);
    }

    [Fact]
    public void GetText_WithEmptyStrings_ReturnsCorrectEmptyString()
    {
        var result = _sut.GetText("", "Hello");

        Assert.Equal("", result);
    }

    // --- GetCvPath ---

    [Fact]
    public void GetCvPath_WhenGerman_ReturnsGermanCvPath()
    {
        var result = _sut.GetCvPath();

        Assert.Equal("cv/cv_de.pdf", result);
    }

    [Fact]
    public async Task GetCvPath_WhenEnglish_ReturnsEnglishCvPath()
    {
        await _sut.SetLanguageAsync(LanguageType.English);

        var result = _sut.GetCvPath();

        Assert.Equal("cv/cv_en.pdf", result);
    }

    // --- SetLanguageServiceAsync ---

    [Fact]
    public async Task SetLanguageAsync_ChangeCurrentLanguage()
    {
        await _sut.SetLanguageAsync(LanguageType.English);

        Assert.Equal(LanguageType.English, _sut.CurrentLanguage);
    }

    [Fact]
    public async Task SetLanguageAsync_SameLanguage_DoesNotFireEvent()
    {
        // Default is German
        bool fired = false;
        _sut.OnLanguageChanged += _ => fired = true;

        await _sut.SetLanguageAsync(LanguageType.German);

        Assert.False(fired);
    }

    [Fact]
    public async Task SetLanguageAsync_DifferentLanguage_FiresEvent()
    {
        LanguageType? received = null;
        _sut.OnLanguageChanged += lang => received = lang;

        await _sut.SetLanguageAsync(LanguageType.English);

        Assert.Equal(LanguageType.English, received);
    }

    [Fact]
    public async Task SetLanguageAsync_PersistsToLocalStorage()
    {
        await _sut.SetLanguageAsync(LanguageType.English);

        await _jsRuntime.Received(1).InvokeVoidAsync(
            "localStorage.setItem",
            Arg.Is<object[]>(args =>
                args.Length == 2 &&
                (string)args[0] == "portfolio_language" &&
                (string)args[1] == "English"));
    }

    [Fact]
    public async Task SetLanguageAsync_SameLanguage_DoesNotPersist()
    {
        // Default is German, setting German again should not call localStorage
        await _sut.SetLanguageAsync(LanguageType.German);

        await _jsRuntime.DidNotReceive().InvokeVoidAsync(
            "localStorage.setItem",
            Arg.Any<object[]>());
    }

    // --- LanguageToggle round-trip ---
    [Fact]
    public async Task LanguageToggle_SwitchesBackAndForth()
    {
        Assert.Equal(LanguageType.German, _sut.CurrentLanguage);

        await _sut.SetLanguageAsync(LanguageType.English);
        Assert.Equal(LanguageType.English, _sut.CurrentLanguage);
        Assert.Equal("Hello", _sut.GetText("Hallo", "Hello"));

        await _sut.SetLanguageAsync(LanguageType.German);
        Assert.Equal(LanguageType.German, _sut.CurrentLanguage);
        Assert.Equal("Hallo", _sut.GetText("Hallo", "Hello"));
    }
}