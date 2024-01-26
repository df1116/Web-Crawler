using System;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        Console.Write("Enter the starting URL: ");
        var startingUrl = Console.ReadLine();
        
        if (WebCrawlerHelper.IsValidUri(startingUrl))
        {
            var startingUri = new Uri(startingUrl);
            Console.WriteLine($"Crawling the following URL: {startingUrl}");

            using (var httpClient = new HttpClient())
            {
                var webCrawler = new WebCrawler(startingUri, httpClient);

                await webCrawler.ParallelCrawl();
            }
        }
        else
        {
            Console.WriteLine($"The following URL is invalid: {startingUrl}");
        }
    }
}