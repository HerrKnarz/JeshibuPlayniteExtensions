using FluentAssertions;
using PlayniteExtensions.Tests.Common;
using System;
using System.Linq;
using Xunit;

namespace PCGamingWikiMetadata.Tests;

public class PCGWGame_Test_WH_40K_SPACE_MARINE : IDisposable
{
    private readonly PcgwGame testGame;

    public PCGWGame_Test_WH_40K_SPACE_MARINE()
    {
        const string title = "Warhammer 40,000: Space Marine";
        var downloader = new FakeWebDownloader(PCGWClient.GetGamePageUrl(title), "data/space-marine.json");
        testGame = TestSetupHelper.GetGame(title, TestMetadataRequestOptions.BattleNet, new(), downloader);
    }

    [Fact]
    public void TestParseSeries()
    {
        var arr = this.testGame.Series.Select(i => i.ToString()).ToArray();
        arr.Should().Contain("Warhammer 40,000: Space Marine");
    }

    public void Dispose()
    {

    }
}
