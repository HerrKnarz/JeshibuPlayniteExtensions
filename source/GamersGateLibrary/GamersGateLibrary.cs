using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using PlayniteExtensions.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;

namespace GamersGateLibrary;

public class GamersGateLibrary : LibraryPlugin
{
    private static readonly ILogger logger = LogManager.GetLogger();
    private static readonly string iconPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "icon.png");
    public override string LibraryIcon => iconPath;
    private GamersGateLibrarySettingsViewModel Settings { get; set; }

    public override Guid Id { get; } = Guid.Parse("b28970a8-37b0-4461-aa33-628024643e73");

    public override string Name => "GamersGate";

    private GamersGateScraper Scraper { get; }
    private IPlatformUtility PlatformUtility { get; }

    public GamersGateLibrary(IPlayniteAPI api) : base(api)
    {
        Settings = new GamersGateLibrarySettingsViewModel(this, api);
        Properties = new LibraryPluginProperties
        {
            HasSettings = true
        };
        Scraper = new GamersGateScraper();
        PlatformUtility = new PlatformUtility(api);
    }

    public override IEnumerable<GameMetadata> GetGames(LibraryGetGamesArgs args)
    {
        switch (Settings.Settings.ImportAction)
        {
            case OnImportAction.Prompt:
                var result = PlayniteApi.Dialogs.ShowMessage(
                    "Import GamersGate games? This will open a browser window because you might encounter CAPTCHAs that you need to solve within 60 seconds. This prompt (or the import as a whole) can be turned off in the add-on settings. Do not close the browser window during the import.",
                    "GamersGate import", System.Windows.MessageBoxButton.OKCancel);
                if (result == System.Windows.MessageBoxResult.Cancel)
                    return [];
                break;
            case OnImportAction.DoNothing:
                return [];
            case OnImportAction.ImportWithoutPrompt:
            case OnImportAction.ImportOffscreen:
            default:
                break;
        }

        bool offscreen = Settings.Settings.ImportAction == OnImportAction.ImportOffscreen;

        var webView = new WebViewWrapper(PlayniteApi, offscreen: offscreen, timeoutSeconds: offscreen ? 15 : 60);

        try
        {
            Scraper.SetWebRequestDelay(Settings.Settings.MinimumWebRequestDelay, Settings.Settings.MaximumWebRequestDelay);
            var data = Scraper.GetAllGames(webView, Settings.Settings.KnownOrderIds.ToArray());
            var output = new List<GameMetadata>(data.Games.Count);
            foreach (var g in data.Games)
            {
                if (!Settings.Settings.InstallData.TryGetValue(g.Id, out var installInfo))
                {
                    installInfo = new GameInstallInfo
                    {
                        Id = g.Id,
                        OrderId = g.OrderId,
                        Name = g.Title,
                    };
                    Settings.Settings.InstallData.Add(g.Id, installInfo);
                }

                installInfo.DownloadUrls = g.DownloadUrls;
                installInfo.UnrevealedKey = g.UnrevealedKey;
                installInfo.Key = g.Key;
                installInfo.DRM = g.DRM;

                var metadata = new GameMetadata
                {
                    GameId = g.Id,
                    Source = new MetadataNameProperty("GamersGate"),
                    Platforms = new(PlatformUtility.GetPlatformsFromName(g.Title, out string name)),
                    Name = name
                };

                if (metadata.Platforms.Count == 0)
                    metadata.Platforms.Add(new MetadataSpecProperty("pc_windows"));

                if (Settings.Settings.UseCoverImages && !string.IsNullOrWhiteSpace(g.CoverImageUrl))
                    metadata.CoverImage = new MetadataFile(g.CoverImageUrl);

                output.Add(metadata);
            }

            Settings.Settings.KnownOrderIds = data.OrderIds;
            SavePluginSettings(Settings.Settings);

            return output;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error fetching GamersGate games");
            PlayniteApi.Notifications.Add("gamersgate-error", "Error fetching GamersGate games: " + ex.Message, NotificationType.Error);
            return [];
        }
        finally
        {
            webView.Dispose();
        }
    }

    public override ISettings GetSettings(bool firstRunSettings) => Settings;

    public override UserControl GetSettingsView(bool firstRunSettings) => new GamersGateLibrarySettingsView();

    public override IEnumerable<InstallController> GetInstallActions(GetInstallActionsArgs args)
    {
        if (args.Game.PluginId == Id)
            yield return new GamersGateManualInstallController(args.Game, Settings.Settings, PlayniteApi, this);
    }

    public override IEnumerable<UninstallController> GetUninstallActions(GetUninstallActionsArgs args)
    {
        if (args.Game.PluginId == Id)
            yield return new GamersGateManualUninstallController(args.Game, Settings.Settings, PlayniteApi, this);
    }

    public override IEnumerable<PlayController> GetPlayActions(GetPlayActionsArgs args)
    {
        if (args.Game.PluginId != Id)
            yield break;

        if (!Settings.Settings.InstallData.TryGetValue(args.Game.GameId, out var installData))
        {
            logger.Debug($"No install data found for {args.Game.Name}, ID: {args.Game.GameId}");
            PlayniteApi.Dialogs.ShowErrorMessage("No install data found.", "GamersGate game launch error");
            yield break;
        }

        string path = Path.Combine(installData.InstallLocation, installData.RelativeExecutablePath);

        yield return new AutomaticPlayController(args.Game)
        {
            Path = path,
            WorkingDir = installData.InstallLocation,
            TrackingMode = TrackingMode.Default
        };
    }
}
