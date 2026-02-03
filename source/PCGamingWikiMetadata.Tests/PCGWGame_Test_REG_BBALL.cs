using FluentAssertions;
using Playnite.SDK.Models;
using PlayniteExtensions.Tests.Common;
using System;
using System.Linq;
using Xunit;

namespace PCGamingWikiMetadata.Tests;

public class PCGWGame_Test_REG_BBALL : IDisposable
{
    private readonly PcgwGame testGame;

    public PCGWGame_Test_REG_BBALL()
    {
        const string title = "Regular Human Basketball";
        var settings = new PCGamingWikiMetadataSettings
        {
            ImportMultiplayerTypes = true,
            ImportFeatureFramerate60 = true,
            ImportFeatureFramerate120 = true,
            ImportFeatureVR = true,
        };
        var downloader = new FakeWebDownloader(PCGWClient.GetGamePageUrl(title), "data/regular-human-basketball.json");
        testGame = TestSetupHelper.GetGame(title, TestMetadataRequestOptions.Epic, settings, downloader);
    }

    [Fact]
    public void TestParseWindowsReleaseDate()
    {
        var date = this.testGame.WindowsReleaseDate();
        Assert.Equal(new ReleaseDate(2018, 8, 1), date);
    }

    [Fact]
    public void TestParseDevelopers()
    {
        var arr = this.testGame.Developers.Select(i => i.ToString()).ToArray();
        arr.Should().BeEquivalentTo("Powerhoof");
    }

    [Fact]
    public void TestParsePublishers()
    {
        var arr = this.testGame.Publishers.Select(i => i.ToString()).ToArray();
        arr.Should().BeEmpty();
    }

    [Fact]
    public void TestParseSeries()
    {
        var arr = this.testGame.Series.Select(i => i.ToString()).ToArray();
        arr.Should().BeEmpty();
    }

    [Fact]
    public void TestParseGenres()
    {
        var arr = this.testGame.Genres.Select(i => i.ToString()).ToArray();
        arr.Should().BeEmpty();
    }

    [Fact]
    public void TestControllerSupport()
    {
        var features = this.testGame.Features.Select(i => i.ToString()).ToArray();
        features.Should().Contain("Full Controller Support");
    }

    [Fact]
    public void TestMultiplayer()
    {
        var features = this.testGame.Features.Select(i => i.ToString()).ToArray();
        features.Should().Contain("Online Multiplayer: 8+");
        features.Should().NotContain("LAN Multiplayer", "LAN Multiplayer: Co-op", "LAN Multiplayer: Versus");
        features.Should().Contain("Local Multiplayer: 8+");
    }

    [Fact]
    public void TestHDR()
    {
        var features = this.testGame.Features.Select(i => i.ToString()).ToArray();
        features.Should().NotContain("HDR");
    }

    [Fact]
    public void TestRayTracing()
    {
        var features = this.testGame.Features.Select(i => i.ToString()).ToArray();
        features.Should().NotContain("Ray Tracing");
    }

    [Fact]
    public void TestFPS()
    {
        var features = this.testGame.Features.Select(i => i.ToString()).ToArray();
        features.Should().Contain("60 FPS");
        features.Should().Contain("120+ FPS");
    }

    [Fact]
    public void TestVR()
    {
        var features = this.testGame.Features.Select(i => i.ToString()).ToArray();
        features.Should().NotContain("VR");
    }

    public void Dispose()
    {
    }
}
