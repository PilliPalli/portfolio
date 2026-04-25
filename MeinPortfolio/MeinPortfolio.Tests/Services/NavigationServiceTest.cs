using MeinPortfolio.Services;

namespace MeinPortfolio.Tests.Services;

public class NavigationServiceTest
{
    private readonly NavigationService _sut = new();

    // --- Initial

    [Fact]
    public void InitialSection_IsHome()
    {
        Assert.Equal(NavigationSection.Home, _sut.CurrentSection);
    }

    [Fact]
    public void InitialPath_IsTilde()
    {
        Assert.Equal("~", _sut.GetSectionPath());
    }

    // --- NavigateTo ---

    [Fact]
    public void NavigateTo_ChangeCurrentSection()
    {
        _sut.NavigateTo(NavigationSection.About);

        Assert.Equal(NavigationSection.About, _sut.CurrentSection);
    }

    [Fact]
    public void NavigateTo_SameSection_DoesNothing()
    {
        _sut.NavigateTo(NavigationSection.About);

        int invokeCount = 0;
        _sut.OnNavigate += _ => invokeCount++;

        _sut.NavigateTo(NavigationSection.About); // same section

        Assert.Equal(0, invokeCount);
        Assert.Equal(NavigationSection.About, _sut.CurrentSection);
    }

    [Fact]
    public void NavigateTo_FiresOnNavigateEvent()
    {
        NavigationSection? received = null;
        _sut.OnNavigate += s => received = s;

        _sut.NavigateTo(NavigationSection.Projects);
        Assert.Equal(NavigationSection.Projects, received);
    }

    [Fact]
    public void NavigateTo_DoesNotFireEvent_WhenSameSection()
    {
        _sut.NavigateTo(NavigationSection.Contact);

        bool fired = false;
        _sut.OnNavigate += _ => fired = true;

        _sut.NavigateTo(NavigationSection.Contact);

        Assert.False(fired);
    }

    // --- GoHome ---

    [Fact]
    public void GoHome_NavigatesBackToHome()
    {
        _sut.NavigateTo(NavigationSection.Projects);

        _sut.GoHome();

        Assert.Equal(NavigationSection.Home, _sut.CurrentSection);
    }

    [Fact]
    public void GoHome_WhenAlreadyHome_DoesNothing()
    {
        int invokeCount = 0;
        _sut.OnNavigate += _ => invokeCount++;

        _sut.GoHome(); // already home

        Assert.Equal(0, invokeCount);
    }

    [Fact]
    public void GoHome_FiresOnNavigateEvent()
    {
        _sut.NavigateTo(NavigationSection.About);

        NavigationSection? received = null;
        _sut.OnNavigate += s => received = s;

        _sut.GoHome();

        Assert.Equal(NavigationSection.Home, received);
    }

    // --- GetSectionPath ---
    [Theory]
    [InlineData(NavigationSection.Home, "~")]
    [InlineData(NavigationSection.About, "~/about")]
    [InlineData(NavigationSection.Projects, "~/projects")]
    [InlineData(NavigationSection.Contact, "~/contact")]
    public void GetSectionPath_ReturnsCorrectPathForSection(NavigationSection section, string expectedPath)
    {
        _sut.NavigateTo(section);

        Assert.Equal(expectedPath, _sut.GetSectionPath());
    }

    [Fact]
    public void MultipleNavigations_TrackCurrentSectionCorrectly()
    {
        _sut.NavigateTo(NavigationSection.About);
        Assert.Equal(NavigationSection.About, _sut.CurrentSection);

        _sut.NavigateTo(NavigationSection.Projects);
        Assert.Equal(NavigationSection.Projects, _sut.CurrentSection);

        _sut.NavigateTo(NavigationSection.Contact);
        Assert.Equal(NavigationSection.Contact, _sut.CurrentSection);

        _sut.GoHome();
        Assert.Equal(NavigationSection.Home, _sut.CurrentSection);
    }
}