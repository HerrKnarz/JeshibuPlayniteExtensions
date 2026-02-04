using Playnite.SDK.Models;
using PlayniteExtensions.Metadata.Common;
using System;
using System.Collections.Generic;

namespace OpenCriticMetadata;

public class OpenCriticBaseModel
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class OpenCriticSearchResultItem : OpenCriticBaseModel, IGameSearchResult
{
    public string Relation { get; set; }

    string IGameSearchResult.Title => Name;

    IEnumerable<string> IGameSearchResult.AlternateNames => [];

    IEnumerable<string> IGameSearchResult.Platforms => [];

    ReleaseDate? IGameSearchResult.ReleaseDate => null;
}

public class OpenCriticGame : OpenCriticBaseModel
{
    public bool? HasLootboxes { get; set; }
    public bool IsMajorRelease { get; set; }
    public OpenCriticImageCollection Images { get; set; }
    public int NumReviews { get; set; }
    public int NumTopCriticReviews { get; set; }
    public double MedianScore { get; set; }
    public double TopCriticScore { get; set; }
    public double Percentile { get; set; }
    public double PercentRecommended { get; set; }
    public string Description { get; set; }
    public DateTimeOffset? FirstReleaseDate { get; set; }
    public List<OpenCriticCompany> Companies { get; set; } = [];
    public List<OpenCriticBaseModel> Genres { get; set; } = [];
    public List<OpenCriticPlatform> Platforms { get; set; } = [];
    public string Url { get; set; }
}

public class OpenCriticImageCollection
{
    public OpenCriticImage Box { get; set; }
    public OpenCriticImage Square { get; set; }
    public OpenCriticImage Masthead { get; set; }
    public OpenCriticImage Banner { get; set; }
    public List<OpenCriticImage> Screenshots { get; set; } = [];
}

public class OpenCriticImage : IImageData
{
    public string OG { get; set; }
    public string SM { get; set; }

    string IImageData.Url => AddDomain(OG);

    string IImageData.ThumbnailUrl => AddDomain(SM);

    int IImageData.Width => 0;

    int IImageData.Height => 0;

    string IImageData.Description => null;

    IEnumerable<string> IImageData.Platforms => [];

    private static string AddDomain(string path) => path == null ? null : $"https://img.opencritic.com/{path}";
}

public class OpenCriticCompany
{
    public string Name { get; set; }
    public string Type { get; set; }
}

public class OpenCriticPlatform : OpenCriticBaseModel
{
    public string ShortName { get; set; }
    public DateTimeOffset ReleaseDate { get; set; }
}

public class OpenCriticUserRatings
{
    public double? Median { get; set; }
    public int Count { get; set; }
}
