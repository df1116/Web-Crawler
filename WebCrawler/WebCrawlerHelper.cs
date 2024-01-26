using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

public class WebCrawlerHelper
{
    // Gets all the URLs within a HTML string that belong to the same base domain
    public static List<string> GetUrlsFromHtml(string htmlString, string baseDomain)
    {
        // Convert the HTML string to an HTML document
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(htmlString);

        // Get all the links from the HTML document
        var links = htmlDocument.DocumentNode.SelectNodes("//a[@href]");

        var sameDomainUrls = new List<string>();

        if (links != null && links.Any())
        {
            foreach (var link in links)
            {
                // Get the URL of the link and convert it to an absolute URL (if relative)
                var url = link.GetAttributeValue("href", "");
                var absoluteUri = BuildAbsoluteUri(url, baseDomain);

                // Check the URI originates from the same subdomain and that it is valid
                if (IsValidUri(absoluteUri.ToString()) && IsSameDomain(absoluteUri, baseDomain))
                {
                    lock (sameDomainUrls)
                    {
                        sameDomainUrls.Add(absoluteUri.AbsoluteUri);
                    }
                }
            }
        }

        return sameDomainUrls;
    }

    // Makes a URL absolute
    public static Uri BuildAbsoluteUri(string url, string baseDomain)
    {
        // Combine the base domain with the relative URL to create an absolute URI
        return new Uri(new Uri($"https://{baseDomain}"), url);
    }

    // Checks whether a URI originatoriginateses from the same domain as the base domain
    public static bool IsSameDomain(Uri uri, string baseDomain)
    {
        return uri.Host == baseDomain;
    }

    // Checks if a URI has a valid scheme
    public static bool IsValidUri(string url)
    {
        return
            Uri.TryCreate(url, UriKind.Absolute, out Uri uri)
            && (uri.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase) ||
                uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase));
    }
}