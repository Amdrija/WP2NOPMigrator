using System;
using System.Text.RegularExpressions;

namespace WP2NOPMigrator
{
    public class Blog
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string Body { get; set; }

        public string MetaKeywords = "";

        public string MetaTitle { get; set; }

        public int LanguageId = 2;

        public bool IncludeInSitemap = true;

        public string BodyOverview { get; set; }

        public bool AllowComments = true;

        public string Tags = "";

        public DateTime? StartDateUtc = null;
        
        public DateTime? EndDateUtc = null;

        public string MetaDescription { get; set; }

        public bool LimitedToStores = false;

        public DateTime CreatedOnUtc { get; set; }

        public Blog(WPBlog wp)
        {
            this.Title = wp.Title;
            this.Body = wp.Content;
            this.ConstructBody(wp);
            this.MetaTitle = wp.Meta.MetaTitle;
            this.BodyOverview = wp.Excerpt.Length > 0 ? wp.Excerpt : wp.Meta.MetaDescription;
            this.MetaDescription = wp.Meta.MetaDescription;
            this.CreatedOnUtc = wp.DateGMT;
        }
        
        public string createLinkFromTitle(string title)
        {
            return title.ToLower()
                        .Replace(" ", "-")
                        .Replace("đ", "dj")
                        .Replace("ž", "z")
                        .Replace("š", "s")
                        .Replace("ć", "c")
                        .Replace("č", "c");
        }

        private void ConstructBody(WPBlog wp)
        {
            this.Body = this.ConstructTableOfContents();
            this.Body = this.ConstructFeaturedText(wp);
            this.Body = this.ConstructReferences(wp);
            this.Body = this.ConstructLinks();
        }
        
        private string ConstructTableOfContents()
        {
            string newBody = this.Body;
            string tableOfContents = "<h2><span style=\"font-size: 24pt;\"><strong>Sadržaj</strong></span></h2>";
            string h2Regex = "<h2><span style=\"font-size: 24pt;\"><strong>(.*)</strong></span></h2>";
            var regex = new Regex(h2Regex);
            tableOfContents += "<ul>";
            int lengthOffset = 0;
            foreach (Match match in regex.Matches(this.Body))
            {
                string id = this.createLinkFromTitle(match.Groups[1].Value);
                string idAttribute = $" id=\"{id}\"";
                newBody = newBody.Insert(match.Index + 3 + lengthOffset, idAttribute);
                lengthOffset += idAttribute.Length;
                tableOfContents += $"<li><a href=\"#{id}\">{match.Groups[1].Value}</a></li>";
            }

            tableOfContents += "</ul>";
            newBody = newBody.Replace("[toc]", tableOfContents);
            
            return newBody;
        }

        private string ConstructFeaturedText(WPBlog wp)
        {
            string newBody = this.Body;
            int i = 0;
            foreach (var featuredText in wp.Meta.FeaturedTexts)
            {
                newBody = newBody.Replace($"[featured_text position=\"{i}\"]", featuredText);
                i++;
            }

            return newBody;
        }

        private string ConstructReferences(WPBlog wp)
        {
            string newBody = this.Body;
            var referenceRegex = new Regex("\\[reference number=(\\d+)\\]");
            foreach (Match match in referenceRegex.Matches(this.Body))
            {
                string referenceNumber = match.Groups[1].Value;
                newBody = newBody.Replace(match.Groups[0].Value, $"<a href=\"#reference-{referenceNumber}\">[{referenceNumber}]</a>");
            }
            
            string references = wp.Meta.Reference;
            var liRegex = new Regex("<li>");

            int lengthOffset = 0;
            int currentReference = 1;
            foreach (Match match in liRegex.Matches(wp.Meta.Reference))
            {
                string id = $" id=\"reference-{currentReference}\"";
                references = references.Insert(match.Index + 3 + lengthOffset, id);
                lengthOffset += id.Length;
                currentReference++;
            }

            newBody += "\n";
            newBody += "<h2>Izvori članka</h2>\n";
            newBody += references;

            return newBody;
        }

        private string ConstructLinks()
        {
            string newBody = this.Body;
            return newBody.Replace("https://www.svezaimunitet.com", "")
                          .Replace("/zdravlje-a-z", "");
        }
    }
}