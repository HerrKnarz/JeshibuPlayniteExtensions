using PlayniteExtensions.Common;

namespace PCGamingWikiMetadata.Tests;

public static class TestSetupHelper
{
    public static PcgwGame GetGame(string pageTitle, TestMetadataRequestOptions options, PCGamingWikiMetadataSettings settings, IWebDownloader downloader)
    {
        var testGame = new PcgwGame(settings, pageTitle, -1);
        var controller = new PCGWGameController(testGame, settings);
        var client = new PCGWClient(options, controller, downloader);
        client.FetchGamePageContent(testGame);
        return testGame;
    }
}
