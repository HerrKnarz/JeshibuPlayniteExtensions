using Playnite;
using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;

namespace PlayniteExtensions.Tests.Common;

public class FakePlayniteDatabase : IGameDatabaseAPI
{
    public Game ImportGame(GameMetadata game)
    {
        throw new NotImplementedException();
    }

    public Game ImportGame(GameMetadata game, LibraryPlugin sourcePlugin)
    {
        throw new NotImplementedException();
    }

    public bool GetGameMatchesFilter(Game game, FilterPresetSettings filterSettings)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Game> GetFilteredGames(FilterPresetSettings filterSettings)
    {
        throw new NotImplementedException();
    }

    public bool GetGameMatchesFilter(Game game, FilterPresetSettings filterSettings, bool useFuzzyNameMatch)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Game> GetFilteredGames(FilterPresetSettings filterSettings, bool useFuzzyNameMatch)
    {
        throw new NotImplementedException();
    }

    public IItemCollection<Game> Games { get; } = new FakeItemCollection<Game>();
    public IItemCollection<Platform> Platforms { get; } = new FakeItemCollection<Platform>();
    public IItemCollection<Emulator> Emulators { get; } = new FakeItemCollection<Emulator>();
    public IItemCollection<Genre> Genres { get; } = new FakeItemCollection<Genre>();
    public IItemCollection<Company> Companies { get; } = new FakeItemCollection<Company>();
    public IItemCollection<Tag> Tags { get; } = new FakeItemCollection<Tag>();
    public IItemCollection<Category> Categories { get; } = new FakeItemCollection<Category>();
    public IItemCollection<Series> Series { get; } = new FakeItemCollection<Series>();
    public IItemCollection<AgeRating> AgeRatings { get; } = new FakeItemCollection<AgeRating>();
    public IItemCollection<Region> Regions { get; } = new FakeItemCollection<Region>();
    public IItemCollection<GameSource> Sources { get; } = new FakeItemCollection<GameSource>();
    public IItemCollection<GameFeature> Features { get; } = new FakeItemCollection<GameFeature>();
    public IItemCollection<GameScannerConfig> GameScanners { get; } = new FakeItemCollection<GameScannerConfig>();
    public IItemCollection<CompletionStatus> CompletionStatuses { get; } = new FakeItemCollection<CompletionStatus>();
    public IItemCollection<ImportExclusionItem> ImportExclusions { get; } = new FakeItemCollection<ImportExclusionItem>();
    public IItemCollection<FilterPreset> FilterPresets { get; } = new FakeItemCollection<FilterPreset>();
    public bool IsOpen { get; }
    public event EventHandler DatabaseOpened;

    public string AddFile(string path, Guid parentId)
    {
        throw new NotImplementedException();
    }

    public void SaveFile(string id, string path)
    {
        throw new NotImplementedException();
    }

    public void RemoveFile(string id)
    {
        throw new NotImplementedException();
    }

    private class FakeBufferedUpdate : IDisposable
    {
        public void Dispose()
        {
        }
    }

    public IDisposable BufferedUpdate() => new FakeBufferedUpdate();

    public void BeginBufferUpdate()
    {
    }

    public void EndBufferUpdate()
    {
    }

    public string GetFileStoragePath(Guid parentId)
    {
        throw new NotImplementedException();
    }

    public string GetFullFilePath(string databasePath)
    {
        throw new NotImplementedException();
    }

    public string DatabasePath { get; }
}
