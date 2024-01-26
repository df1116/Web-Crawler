using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebCrawlerTests
{
    [TestClass]
    public class WebCrawlerTests
    {
        // Testing top level functionality of the whole application. Tests rely on external http call to google.com.
        // To comprehensively test functionality I would extend these tests and create mocks of
        // httpClient.GetStringAsync(url) response.

        private HttpClient _httpClient;

        [TestInitialize]
        public void TestInitialize()
        {
            _httpClient = new HttpClient();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _httpClient.Dispose();
        }
        
        [TestMethod]
        public async Task Crawl_ValidUrl_CrawlsSuccessfully()
        {
            var uri = new Uri("https://www.google.co.uk");
            var crawler = new WebCrawler(uri, _httpClient);
            await crawler.ParallelCrawl();

            Assert.IsTrue(crawler.VisitedUrls.Contains("https://www.google.co.uk/"));
            Assert.IsTrue(crawler.VisitedUrls.Count >= 1);
        }

        [TestMethod]
        public async Task Crawl_InvalidUrl_HandlesError()
        {
            var uri = new Uri($"https://www.invalid-url-{Guid.NewGuid()}.com");
            var crawler = new WebCrawler( uri,_httpClient);
            await crawler.ParallelCrawl();

            Assert.AreEqual(crawler.VisitedUrls.Count, 0);
        }
    }
}

