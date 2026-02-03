using FluentAssertions;
using Playnite.SDK.Models;
using PlayniteExtensions.Tests.Common;
using System;
using System.Linq;
using Xunit;

namespace PCGamingWikiMetadata.Tests;

public class PCGWGame_Test_CODMW_nomulti : IDisposable
{
    private readonly PcgwGame testGame;

    public PCGWGame_Test_CODMW_nomulti()
    {
        const string title = "Call of Duty: Modern Warfare";
        var settings = new PCGamingWikiMetadataSettings
        {
            ImportMultiplayerTypes = false,
            ImportFeatureHDR = false,
            ImportFeatureRayTracing = false,
            ImportFeatureFramerate60 = false,
            ImportFeatureFramerate120 = true,
            ImportFeatureUltrawide = false,
            ImportFeatureVR = true,
        };
        var downloader = new FakeWebDownloader(PCGWClient.GetGamePageUrl(title), "data/cod-mw.json");
        testGame = TestSetupHelper.GetGame(title, TestMetadataRequestOptions.BattleNet, settings, downloader);
    }

    [Fact]
    public void TestParseWindowsReleaseDate()
    {
        var date = this.testGame.WindowsReleaseDate();
        Assert.Equal(new ReleaseDate(2019, 10, 25), date);
    }

    [Fact]
    public void TestParseDevelopers()
    {
        var arr = this.testGame.Developers.Select(i => i.ToString()).ToArray();
        arr.Should().BeEquivalentTo("Infinity Ward", "High Moon Studios", "Sledgehammer Games", "Raven Software", "Beenox");
    }

    [Fact]
    public void TestParsePublishers()
    {
        var arr = this.testGame.Publishers.Select(i => i.ToString()).ToArray();
        arr.Should().BeEquivalentTo("Activision");
    }

    [Fact]
    public void TestParseSeries()
    {
        var arr = this.testGame.Series.Select(i => i.ToString()).ToArray();
        arr.Should().BeEquivalentTo("Call of Duty: Modern Warfare", "Call of Duty");
    }

    [Fact]
    public void TestParseGenres()
    {
        var arr = this.testGame.Genres.Select(i => i.ToString()).ToArray();
        arr.Should().BeEquivalentTo("Action", "Shooter", "Battle royale");
    }

    [Fact]
    public void TestParsePerspectives()
    {
        var arr = this.testGame.Tags.Select(i => i.ToString()).ToArray();
        arr.Should().Contain("First-person");
    }

    [Fact]
    public void TestParseControls()
    {
        var arr = this.testGame.Tags.Select(i => i.ToString()).ToArray();
        arr.Should().Contain("Direct control");
    }

    [Fact]
    public void TestParseArtStyles()
    {
        var arr = this.testGame.Tags.Select(i => i.ToString()).ToArray();
        arr.Should().Contain("Realistic");
    }

    [Fact]
    public void TestParsePacing()
    {
        var arr = this.testGame.Tags.Select(i => i.ToString()).ToArray();
        arr.Should().Contain("Real-time");
    }

    [Fact]
    public void TestParseThemes()
    {
        var arr = this.testGame.Tags.Select(i => i.ToString()).ToArray();
        arr.Should().Contain("Middle East");
    }

    [Fact]
    public void TestParseModes()
    {
        var arr = this.testGame.Features.Select(i => i.ToString()).ToArray();
        arr.Should().Contain("Singleplayer", "Multiplayer");
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
        features.Should().NotContain("Online Multiplayer: 8+", "Online Multiplayer: Co-op", "Online Multiplayer: Versus");
        features.Should().NotContain("LAN Multiplayer", "LAN Multiplayer: Co-op", "LAN Multiplayer: Versus");
        features.Should().NotContain("Local Multiplayer", "Local Multiplayer: Co-op", "Local Multiplayer: Versus");
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
        features.Should().NotContain("60 FPS");
        features.Should().Contain("120+ FPS");
    }

    [Fact]
    public void TestUltrawide()
    {
        var features = this.testGame.Features.Select(i => i.ToString()).ToArray();
        features.Should().NotContain("Ultra-widescreen");
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
