using FluentAssertions;
using PlayniteExtensions.Tests.Common;
using System;
using System.Linq;
using Xunit;

namespace PCGamingWikiMetadata.Tests;

public class PCGWGame_Test_CATLADY : IDisposable
{
    private readonly PcgwGame testGame;

    public PCGWGame_Test_CATLADY()
    {
        const string title = "Cat Lady - The Card Game";
        var downloader = new FakeWebDownloader(PCGWClient.GetGamePageUrl(title), "data/cat-lady.json");
        testGame = TestSetupHelper.GetGame(title, TestMetadataRequestOptions.Steam, new(), downloader);
    }

    [Fact]
    public void TestTouchscreenSupport()
    {
        var features = this.testGame.Features.Select(i => i.ToString()).ToArray();
        features.Should().Contain("Touchscreen optimised");
    }

    public void Dispose()
    {

    }
}
