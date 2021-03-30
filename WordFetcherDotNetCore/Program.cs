using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WordFetcherDotNetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Run().Wait();
        }


        static async Task Run()
        {
            var allWords = new List<string>();

            using (var httpClient = new HttpClient())
            {
                var httpResponse = await httpClient.GetAsync("https://slovored.com/sitemap/grammar");
                var httpContent = await httpResponse.Content.ReadAsByteArrayAsync();
                string responseString = Encoding.GetEncoding(1251).GetString(httpContent, 0, httpContent.Length);

                //Console.OutputEncoding = Encoding.GetEncoding(1251);
                //Console.WriteLine(responseString);

                HtmlDocument page = new HtmlDocument();
                page.LoadHtml(responseString);
                
                var links = getTags(page.DocumentNode, "a");
                var filteredTags = links.Where(tag => {
                    var attribute = tag.GetAttributeValue("title", "randomString");
                    return attribute.StartsWith("Думи започващи с буквата");
                    }
                    );
                var startingLetters = filteredTags.Select(tag => tag.InnerText).ToList();


                //second letter----------------------------------------------------------------------------------------------------------------------------------------------------------------

                var allTwoLetterBegginings = new List<string>();

                foreach (string startingLetter in startingLetters)
                {
                    httpResponse = await httpClient.GetAsync($"https://slovored.com/sitemap/grammar/letter/{startingLetter}");
                    httpContent = await httpResponse.Content.ReadAsByteArrayAsync();

                    responseString = Encoding.GetEncoding(1251).GetString(httpContent, 0, httpContent.Length);
                    page.LoadHtml(responseString);
                    var innerLinks = getTags(page.DocumentNode, "a");

                    var innerFilteredTags = innerLinks.Where(innerTag =>
                             innerTag.GetAttributeValue("title", "randomString").StartsWith("Думи започващи с буквите")
                        );

                    var initialTwoLettersList = innerFilteredTags.Select(tag => tag.InnerText).ToList();
                    allTwoLetterBegginings.AddRange(initialTwoLettersList);
                }
                
                //all words----------------------------------------------------------------------------------------------------------------------------------------------------------------


                foreach (string twoLetters in allTwoLetterBegginings)
                {
                    httpResponse = await httpClient.GetAsync($"https://slovored.com/sitemap/grammar/letter/{twoLetters[0]}/{twoLetters}");
                    httpContent = await httpResponse.Content.ReadAsByteArrayAsync();

                    responseString = Encoding.GetEncoding(1251).GetString(httpContent, 0, httpContent.Length);
                    page.LoadHtml(responseString);
                    var innerLinks = getTags(page.DocumentNode, "a");

                    var innerFilteredLinks = innerLinks.Where(innerTag =>
                             innerTag.GetAttributeValue("href", "randomString").StartsWith(@"/search/grammar/")
                        );
                    var allWordsOnPage = innerFilteredLinks.Select(tag => tag.InnerText).ToList();
                    allWords.AddRange(allWordsOnPage);
                }
                
            }


            System.IO.File.WriteAllLines(@"words.txt", allWords);

        }

        static List<HtmlNode> getTags(HtmlNode node, string tagName)
        {
            List<HtmlNode> output = new List<HtmlNode>();
            if (node.Name == tagName)
            {
                output.Add(node);
            }

            if (!node.HasChildNodes)//bottom of reccursion
            {
                return output;
            }

            foreach (var child in node.ChildNodes)
            {
                if (child.NodeType == HtmlNodeType.Element)
                {
                    var childTags = getTags(child, tagName);
                    output.AddRange(childTags);
                }
            }

            return output;
        }

    }
}
