#! "net8.0"
#r "nuget: Metalsharp, 0.9.0-rc.5"
#r "nuget: Metalsharp.LiquidTemplates, 0.9.0-rc-3"
#r "nuget: System.ServiceModel.Syndication, 8.0.0"
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using Metalsharp;
using Metalsharp.LiquidTemplates;

var config = JsonSerializer.Deserialize<Config>(await File.ReadAllTextAsync("config.json"));
var feeds = new List<SyndicationFeed>();

using (var client = new HttpClient())
{
    var downloadTasks = config.Feeds.Select(async url =>
    {
        try
        {
            Console.WriteLine($"Downloading {url}");

            using var stream = await client.GetStreamAsync(url);

            using var xmlReader = XmlReader.Create(stream);
            var feed = SyndicationFeed.Load(xmlReader);

            if (feed != null)
            {
                lock (feeds)
                {
                    feeds.Add(feed);
                }
            }
            else
            {
                Console.WriteLine($"Failed to parse feed from {url}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to download {url}: {ex.Message}");
        }
    });

    await Task.WhenAll(downloadTasks);
}

Console.WriteLine("Finished downloading rss feeds");

var blogs =
    feeds
    .Select(f =>
    {
        var url = f.Links.First(l => !l.Uri.ToString().Contains("feed") && !l.Uri.ToString().Contains("xml") && !l.Uri.ToString().Contains("rss")).Uri.ToString();
        return new Blog(
            url,
            f.Title.Text,
            f.Description?.Text ?? string.Empty,
            f.Items
                .Where(i => i.Links.Count != 0)
                .Select(i => new Post(i.Links.First().Uri.ToString(), i.Title.Text, i.PublishDate.UtcDateTime.ToString("dd MMMM yyyy"), i.PublishDate.UtcDateTime, f.Title.Text, url))
        );
    })
    .OrderBy(b => b.Name);

var posts =
    blogs
    .SelectMany(b => b.Posts)
    .OrderByDescending(p => p.RawDate)
    .Take(config.PostsToShow);

new MetalsharpProject()
    .AddOutput(new MetalsharpFile(string.Empty, "index.html", new Dictionary<string, object>()
    {
        ["template"] = "blogroll",
        ["blogs"] = blogs,
        ["title"] = config.Pages.Blogroll.Title,
        ["subtitle"] = config.Pages.Blogroll.Subtitle,
        ["heading"] = config.Title,
        ["cssUrl"] = config.CssUrl
    }))
    .AddOutput(new MetalsharpFile(string.Empty, "latest.html", new Dictionary<string, object>()
    {
        ["template"] = "feed",
        ["posts"] = posts,
        ["title"] = config.Pages.Latest.Title,
        ["subtitle"] = config.Pages.Latest.Subtitle,
        ["heading"] = config.Title,
        ["cssUrl"] = config.CssUrl
    }))
    .UseLiquidTemplates("Templates")
    .AddOutput("Static", @".\")
    .Build(new BuildOptions()
    {
        OutputDirectory = "output",
        ClearOutputDirectory = true
    });

record PageConfig(string Title, string Subtitle);
record PagesConfig(PageConfig Blogroll, PageConfig Latest);
record Config(string Title, int PostsToShow, string CssUrl, PagesConfig Pages, string[] Feeds);
record Post(string Url, string Title, string Date, DateTime RawDate, string Author, string AuthorUrl);
record Blog(string Url, string Name, string Description, IEnumerable<Post> Posts);
