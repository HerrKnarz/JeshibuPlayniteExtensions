using MobyGamesMetadata.Api;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using PlayniteExtensions.Common;
using PlayniteExtensions.Metadata.Common;
using System;
using System.Collections.Generic;

namespace MobyGamesMetadata;

public class MobyGamesMetadataProvider(MetadataRequestOptions options, MobyGamesMetadata plugin, IGameSearchProvider<GameSearchResult> dataSource, IPlatformUtility platformUtility, MobyGamesMetadataSettings settings)
    : GenericMetadataProvider<GameSearchResult>(dataSource, options, plugin.PlayniteApi, platformUtility)
{
    public override List<MetadataField> AvailableFields => plugin.SupportedFields;

    protected override string ProviderName => "MobyGames";

    protected override bool FilterImage(GameField field, IImageData imageData)
    {
        var imgSettings = field switch
        {
            GameField.CoverImage => settings.Cover,
            GameField.BackgroundImage => settings.Background,
            _ => null,
        };

        if (imgSettings == null)
            return true;

        if (imageData.Width < imgSettings.MinWidth || imageData.Height < imgSettings.MinHeight)
            return false;

        return imgSettings.AspectRatio switch
        {
            AspectRatio.Vertical => imageData.Width < imageData.Height,
            AspectRatio.Horizontal => imageData.Width > imageData.Height,
            AspectRatio.Square => imageData.Width == imageData.Height,
            _ => true,
        };
    }

    public override void Dispose()
    {
        if (dataSource is IDisposable disposableDataSource)
            disposableDataSource.Dispose();

        base.Dispose();
    }
}
