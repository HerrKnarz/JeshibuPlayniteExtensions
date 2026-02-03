using FluentAssertions;
using PlayniteExtensions.Tests.Common;
using System;
using System.Linq;
using Xunit;

namespace PCGamingWikiMetadata.Tests;

public class PCGWGame_Test_SKYRIMVR : IDisposable
{
    private readonly PcgwGame testGame;

    public PCGWGame_Test_SKYRIMVR()
    {
        const string title = "The Elder Scrolls V: Skyrim VR";
        var settings = new PCGamingWikiMetadataSettings { ImportFeatureVR =  true };
        var downloader = new FakeWebDownloader(PCGWClient.GetGamePageUrl(title), "data/skyrim-vr.json");
        testGame = TestSetupHelper.GetGame(title, TestMetadataRequestOptions.Steam, settings, downloader);
    }

    [Fact]
    public void TestVR()
    {
        var features = this.testGame.Features.Select(i => i.ToString()).ToArray();
        features.Should().Contain("VR");
    }

    public void Dispose()
    {

    }
}
