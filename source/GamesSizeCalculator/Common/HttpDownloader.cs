using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GamesSizeCalculator.Common;

// Based on https://github.com/JosefNemec/Playnite
public static class HttpDownloader
{
    private static readonly ILogger Logger = LogManager.GetLogger();
    private static readonly HttpClient HttpClient = new();
    private static readonly HttpClient HttpClientJson = new();
    private static readonly Downloader Downloader = new();

    public static string DownloadString(IEnumerable<string> mirrors)
    {
        return Downloader.DownloadString(mirrors);
    }

    public static string DownloadString(string url)
    {
        return Downloader.DownloadString(url);
    }

    public static string DownloadString(string url, CancellationToken cancelToken)
    {
        return Downloader.DownloadString(url, cancelToken);
    }

    public static string DownloadString(string url, Encoding encoding)
    {
        return Downloader.DownloadString(url, encoding);
    }

    public static string DownloadString(string url, List<Cookie> cookies)
    {
        return Downloader.DownloadString(url, cookies);
    }

    public static string DownloadString(string url, List<Cookie> cookies, Encoding encoding)
    {
        return Downloader.DownloadString(url, cookies, encoding);
    }

    public static void DownloadString(string url, string path)
    {
        Downloader.DownloadString(url, path);
    }

    public static void DownloadString(string url, string path, Encoding encoding)
    {
        Downloader.DownloadString(url, path, encoding);
    }

    public static byte[] DownloadData(string url)
    {
        return Downloader.DownloadData(url);
    }

    public static void DownloadFile(string url, string path)
    {
        Downloader.DownloadFile(url, path);
    }

    public static void DownloadFile(string url, string path, CancellationToken cancelToken)
    {
        Downloader.DownloadFile(url, path, cancelToken);
    }

    public static bool DownloadFile(string url, string path, CancellationToken cancelToken, Action<DownloadProgressChangedEventArgs> progressHandler)
    {
        return Downloader.DownloadFile(url, path, cancelToken, progressHandler);
    }

    public static HttpStatusCode GetResponseCode(string url)
    {
        try
        {
            var response = HttpClient.GetAsync(url).GetAwaiter().GetResult();
            return response.StatusCode;
        }
        catch (Exception e)
        {
            Logger.Error(e, $"Failed to get HTTP response for {url}.");
            return HttpStatusCode.ServiceUnavailable;
        }
    }

    public static async Task<bool> DownloadFileAsync(string requestUri, string fileToWriteTo)
    {
        Logger.Debug($"DownloadFileAsync method with url {requestUri} and file to write {fileToWriteTo}");
        try
        {
            using HttpResponseMessage response = await HttpClient.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using Stream streamToReadFrom = await response.Content.ReadAsStreamAsync();
                using Stream streamToWriteTo = File.Open(fileToWriteTo, FileMode.Create);
                await streamToReadFrom.CopyToAsync(streamToWriteTo);
                Logger.Debug("Ran to completion");
                return true;
            }
            else
            {
                Logger.Debug($"Request Url {requestUri} didn't give OK status code and was not downloaded");
            }
        }
        catch (Exception e)
        {
            Logger.Error(e, $"Error during file download, url {requestUri}.");
        }

        return false;
    }

    public static async Task<bool> DownloadJsonFileAsync(string requestUri, string fileToWriteTo)
    {
        Logger.Debug($"DownloadJsonFileAsync method with url {requestUri} and file to write {fileToWriteTo}");
        if (!HttpClientJson.DefaultRequestHeaders.Any())
        {
            HttpClientJson.DefaultRequestHeaders.Add("Accept", "application/json");
            HttpClient.Timeout = TimeSpan.FromMilliseconds(2000);
        }

        try
        {
            using HttpResponseMessage response = await HttpClient.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using Stream streamToReadFrom = await response.Content.ReadAsStreamAsync();
                using Stream streamToWriteTo = File.Open(fileToWriteTo, FileMode.Create);
                await streamToReadFrom.CopyToAsync(streamToWriteTo);
                Logger.Debug("Ran to completion");
                return true;
            }
            else
            {
                Logger.Debug($"Request Url {requestUri} didn't give OK status code and was not downloaded");
            }
        }
        catch (Exception e)
        {
            Logger.Error(e, $"Error during file download, url {requestUri}.");
        }

        return false;
    }

    public static async Task<string> DownloadStringAsync(string requestUri)
    {
        Logger.Debug($"DownloadStringAsync method with url {requestUri}");
        try
        {
            using var response = await HttpClient.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            using var content = response.Content;
            Logger.Debug("Ran to completion");
            return await content.ReadAsStringAsync();
        }
        catch (Exception e)
        {
            Logger.Error(e, $"Error during file download, url {requestUri}.");
            return string.Empty;
        }
    }

    public static async Task<bool> DownloadFileWithHeadersAsync(string requestUri, string fileToWriteTo, Dictionary<string, string> headersDictionary)
    {
        Logger.Debug($"DownloadFileWithHeadersAsync method with url {requestUri} and file to write {fileToWriteTo}");
        using var request = new HttpRequestMessage(HttpMethod.Put, requestUri);
        foreach (var pair in headersDictionary)
        {
            request.Headers.Add(pair.Key, pair.Value);
        }

        try
        {
            using HttpResponseMessage response = await HttpClient.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using Stream streamToReadFrom = await response.Content.ReadAsStreamAsync();
                using Stream streamToWriteTo = File.Open(fileToWriteTo, FileMode.Create);
                await streamToReadFrom.CopyToAsync(streamToWriteTo);
                Logger.Debug("Ran to completion");
                return true;
            }
            else
            {
                Logger.Debug($"Request Url {requestUri} didn't give OK status code and was not downloaded");
            }
        }
        catch (Exception e)
        {
            Logger.Error(e, $"Error during file download, url {requestUri}.");
        }

        return false;
    }

    public static async Task<string> DownloadStringWithHeadersAsync(string requestUri, Dictionary<string, string> headersDictionary)
    {
        Logger.Debug($"DownloadStringWithHeadersAsync method with url {requestUri}");
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        foreach (var pair in headersDictionary)
        {
            request.Headers.Add(pair.Key, pair.Value);
        }

        try
        {
            using var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            using var content = response.Content;
            Logger.Debug("Ran to completion");
            return await content.ReadAsStringAsync();
        }
        catch (Exception e)
        {
            Logger.Error(e, $"Error during file download, url {requestUri}.");
            return string.Empty;
        }
    }
}
