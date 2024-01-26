using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

public class WebCrawler
{
    public readonly HashSet<string> VisitedUrls;
    private readonly Uri _startingUri;
    private readonly HttpClient _httpClient;
    private readonly ConcurrentStack<string> _urlStack;
    private readonly object _lockObject = new();
    private int _httpErrors;
    private int _otherErrors;

    public WebCrawler(Uri startingUri, HttpClient httpClient)
    {
        VisitedUrls = new HashSet<string>();
        _startingUri = startingUri;
        _httpClient = httpClient;
        _urlStack = new ConcurrentStack<string>();
        _httpErrors = 0;
        _otherErrors = 0;
    }

    public async Task ParallelCrawl()
    {
        var url = _startingUri.ToString();
        _urlStack.Push(url);
        var tasks = new List<Task>();
      
        while (true)
        {
            lock (_lockObject)
            {
                while (_urlStack.TryPop(out var currentUrl) && VisitedUrls.Count < 20)
                {
                    if (!VisitedUrls.Contains(currentUrl))
                    {
                        var result = currentUrl;
                        tasks.Add(Task.Run(() => Crawl(result)));
                    }
                }
            }
          
            if (tasks.All(t => t.Status == TaskStatus.RanToCompletion || t.Status == TaskStatus.Faulted))
            {
                break;
            }

            await Task.Delay(500);
        }

        await Task.WhenAll(tasks);

        Console.WriteLine($"{_httpErrors} HTTP errors were encountered and {_otherErrors} other errors were encountered.");
    }

    public async Task Crawl(string url)
    {
        try
        {
            var htmlString = await _httpClient.GetStringAsync(url);

            lock (_lockObject)
            {
                if (!VisitedUrls.Contains(url))
                {
                    VisitedUrls.Add(url);
                    Console.WriteLine($"Visited: {url}");
                }
            }

            var urlsFromHtml = WebCrawlerHelper.GetUrlsFromHtml(htmlString, _startingUri.Host);

            foreach (var childUrl in urlsFromHtml)
            {
                _urlStack.Push(childUrl);
            }
        }
        catch (HttpRequestException _)
        {
            // Keep track of HTTP errors
            _httpErrors += 1;
        }
        catch (Exception _)
        {
            // Keep track of other errors
            _otherErrors += 1;
        }
    }
}