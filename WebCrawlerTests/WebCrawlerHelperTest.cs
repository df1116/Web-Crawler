using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebCrawlerTests
{
    [TestClass]
    public class WebCrawlerHelperTests
    {
        private const string BaseDomain = "www.example.com";

        [TestMethod]
        public void GetUrlsFromHtml_ValidHtml_ReturnsCorrectUrls()
        {
            const string htmlString = @"<html><body><a href='/page1'>Link 1</a><a href='https://www.example.com/page2'>Link 2</a></body></html>";
            var result = WebCrawlerHelper.GetUrlsFromHtml(htmlString, BaseDomain);

            CollectionAssert.AreEquivalent(new[] { "https://www.example.com/page1", "https://www.example.com/page2" }, result);
        }

        [TestMethod]
        public void BuildAbsoluteUrl_RelativeUrl_ReturnsAbsoluteUrl()
        {
            const string relativeUrl = "/page3";
            var result = WebCrawlerHelper.BuildAbsoluteUri(relativeUrl, BaseDomain);

            var uri = new Uri("https://www.example.com/page3");
            Assert.AreEqual(uri, result);
        }

        [TestMethod]
        public void IsSameDomain_SameDomain_ReturnsTrue()
        {
            var uri = new Uri("https://www.example.com/page1");
            var result = WebCrawlerHelper.IsSameDomain(uri, BaseDomain);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsSameDomain_DifferentDomain_ReturnsFalse()
        {
            var uri = new Uri("https://www.anotherdomain.com/page1");
            var result = WebCrawlerHelper.IsSameDomain(uri, BaseDomain);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsValidUrl_ValidHttpUrl_ReturnsTrue()
        {
            var result = WebCrawlerHelper.IsValidUri("https://www.monzo.com");

            Assert.IsTrue(result);
        }
        
        [DataTestMethod]
        [DataRow("http://not a valid url.com")]
        [DataRow("hts://example.com")]
        [DataRow("ftp://example.com")]
        [DataRow(null)]
        public void IsValidUrl_InvalidUrl_ReturnsFalse(string inputUrl)
        {
            var result = WebCrawlerHelper.IsValidUri(inputUrl);

            Assert.IsFalse(result);
        }
    }
}