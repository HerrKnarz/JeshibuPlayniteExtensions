using Playnite.SDK;
using System;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GamersGateLibrary;

public interface IWebViewWrapper : IDisposable
{
    string DownloadPageSource(string targetUrl);
}

/// <summary>
/// Intended for use for only one request
/// </summary>
public class WebViewWrapper : IWebViewWrapper
{
    public WebViewWrapper(IPlayniteAPI playniteApi, int width = 675, int height = 600, bool offscreen = false, int timeoutSeconds = 60)
    {
        Offscreen = offscreen;
        TimeoutSeconds = timeoutSeconds;
        _view = System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            IWebView v = offscreen
                ? playniteApi.WebViews.CreateOffscreenView()
                : playniteApi.WebViews.CreateView(width, height, Colors.Black);

            if (!offscreen)
                v.Open();

            return v;
        });
        _view.LoadingChanged += View_LoadingChanged;
    }

    private readonly IWebView _view;
    private readonly ILogger _logger = LogManager.GetLogger();
    private readonly object _requestLifespanLock = new();
    public bool Offscreen { get; }
    public int TimeoutSeconds { get; }
    public string TargetUrl { get; private set; }
    private TaskCompletionSource<string> DownloadCompletionSource { get; set; }

    public string DownloadPageSource(string targetUrl)
    {
        lock (_requestLifespanLock)
        {
            _logger.Debug($"Getting {targetUrl}, timeout {TimeoutSeconds} seconds");
            TargetUrl = targetUrl;

            DownloadCompletionSource = new TaskCompletionSource<string>();

            _view.Navigate(targetUrl);

            DownloadCompletionSource.Task.Wait(TimeoutSeconds * 1000);
            if (DownloadCompletionSource.Task.IsCompleted)
            {
                string source = DownloadCompletionSource.Task.Result;
                DownloadCompletionSource = null;

                return source;
            }

            return null;
        }
    }

    public static bool IsAuthenticated(string pageSource)
    {
        bool authenticated = pageSource.Contains("navigation-link--icon-user");
        return authenticated;
    }

    private bool IsTargetUrl()
    {
        var currentUri = new Uri(_view.GetCurrentAddress());
        return currentUri.GetLeftPart(UriPartial.Query) == TargetUrl;
    }

    private async void View_LoadingChanged(object sender, Playnite.SDK.Events.WebViewLoadingChangedEventArgs e)
    {
        if (e.IsLoading)
            return;

        try
        {
            if (!IsTargetUrl())
            {
                _logger.Debug($"Waiting for {TargetUrl}, got {_view.GetCurrentAddress()}");
                return;
            }

            var source = await _view.GetPageSourceAsync();
            if (IsAuthenticated(source))
            {
                DownloadCompletionSource?.TrySetResult(source);
                _logger.Debug($"Completed request for {TargetUrl}");
            }
            else
            {
                _logger.Debug($"Source for {TargetUrl} is not authenticated");
                return;
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error trying to navigate to " + TargetUrl);
        }
    }

    public void Dispose()
    {
        try
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                _view.Close();
                _view.Dispose();
            });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error disposing WebViewWrapper");
        }
    }
}
