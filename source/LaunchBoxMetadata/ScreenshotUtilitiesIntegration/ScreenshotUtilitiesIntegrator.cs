using KNARZhelper;
using KNARZhelper.ScreenshotsCommon;
using KNARZhelper.ScreenshotsCommon.Models;
using Playnite.SDK;
using Playnite.SDK.Data;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LaunchBoxMetadata.ScreenshotUtilitiesIntegration;

public class ScreenshotUtilitiesIntegrator(string name, Guid id)
{
    // TODO: change these methods to use the ones provided by the addon. Mainly look into
    // GetImageDetails in LaunchBoxMetadataProvider.

    public async Task<bool> FetchScreenshotsAsync(Game game, int daysSinceLastUpdate, bool forceUpdate, string gogId = default)
    {
        Game _game;
        ScreenshotGroup _screenshotGroup;

        try
        {
            // return when the main addon isn't installed.
            if (!ScreenshotHelper.IsScreenshotUtilitiesInstalled || game == null)
            {
                return false;
            }

            // set game and load the file.
            _game = game;

            var fileExists = false;

            (fileExists, _screenshotGroup) = ScreenshotHelper.LoadGroup(_game, name, id);

            // return if we don't want to force an update and the last update was inside the days configured.
            if (!forceUpdate
                && _screenshotGroup.LastUpdate != null
                && (_screenshotGroup.LastUpdate > DateTime.Now.AddDays(daysSinceLastUpdate * -1)))
            {
                return false;
            }

            // Return if a game was searched and it's the one we already have.
            if (gogId != default && gogId.Equals(_screenshotGroup.GameIdentifier))
            {
                return false;
            }

            // Get the right name to search for.
            var searchName = GetGogId(gogId);

            if (string.IsNullOrEmpty(searchName))
            {
                return false;
            }

            // We need to reset the file if we got a new gogId from the method call and it's not the
            // same we already got.
            if (!fileExists || (gogId != default && !searchName.Equals(_screenshotGroup.GameIdentifier)))
            {
                _screenshotGroup.GameIdentifier = searchName;

                _screenshotGroup.Screenshots.Clear();
            }

            var updated = await LoadScreenshotsFromSourceAsync();

            ScreenshotHelper.SaveScreenshotGroupJson(game, _screenshotGroup);

            return updated;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error fetching screenshots for {game.Name}");
            return false;
        }
    }

    public string GetScreenshotSearchResult(Game game, string searchTerm)
    {
        var gogSearchResult = ApiHelper.GetJsonFromApi<GogSearchResult>($"{_searchUrl}{searchTerm.RemoveDiacritics().UrlEncode()}", Name);

        var searchResults = new List<GenericItemOption>();

        if (!gogSearchResult?.Products?.Any() ?? true)
        {
            return null;
        }

        var result = new List<ScreenshotSearchResult>(gogSearchResult.Products.Select(product => new ScreenshotSearchResult
        {
            Name = product.Title,
            Description = $"{product.ReleaseDate} -  ID {product.Id}",
            Identifier = product.Id
        }));

        return Serialization.ToJson(result);
    }
}
