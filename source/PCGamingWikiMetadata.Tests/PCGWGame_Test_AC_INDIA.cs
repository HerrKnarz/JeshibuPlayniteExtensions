using FluentAssertions;
using PlayniteExtensions.Tests.Common;
using System;
using System.Linq;
using Xunit;

namespace PCGamingWikiMetadata.Tests;

public class PCGWGame_Test_AC_INDIA : IDisposable
{
    private readonly PcgwGame testGame;

    public PCGWGame_Test_AC_INDIA()
    {
        const string title = "Assassin's Creed Chronicles: India";
        var settings = new PCGamingWikiMetadataSettings
        {
            ImportTagMiddleware =  true,
            TagPrefixMiddleware = "[Middleware]",
        };
        var downloader = new FakeWebDownloader(PCGWClient.GetGamePageUrl(title), "data/ac-india.json");
        testGame = TestSetupHelper.GetGame(title, TestMetadataRequestOptions.BattleNet, settings, downloader);
    }

    [Fact]
    public void TestParseSeries()
    {
        var arr = testGame.Series.Select(i => i.ToString()).ToArray();
        arr.Should().Contain("Assassin's Creed Chronicles");
        arr.Should().Contain("Assassin's Creed");
    }

    [Fact]
    public void TestParseMiddleware()
    {
        var tags = testGame.Tags.Select(tag => tag.ToString()).ToArray();
        tags.Should().Contain("[Middleware] Physics: PhysX");
        tags.Should().Contain("[Middleware] Interface: Scaleform GFx");
        tags.Should().Contain("[Middleware] Cutscenes: Bink Video");
        tags.Where(t => t.Contains("[Middleware]")).Should().HaveCount(3);
    }

    public void Dispose()
    {

    }
}
