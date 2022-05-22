using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace WP2NOPMigrator
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string Body { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaTitle { get; set; }

        public int LanguageId { get; set; }

        public bool IncludeInSitemap { get; set; }

        public string BodyOverview { get; set; }

        public bool AllowComments { get; set; }

        public string Tags { get; set; }

        public DateTime? StartDateUtc { get; set; }
        
        public DateTime? EndDateUtc { get; set; }

        public string MetaDescription { get; set; }

        public bool LimitedToStores { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public BlogPost()
        {
            this.MetaKeywords = "";
            this.LanguageId = 3;
            this.IncludeInSitemap = true;
            this.AllowComments = true;
            this.Tags = "";
            this.StartDateUtc = null;
            this.EndDateUtc = null;
            this.LimitedToStores = false;
        }
        public BlogPost(WPBlog wp)
        {
            this.Title = wp.Title;
            this.Body = wp.Content;
            this.MetaKeywords = "";
            this.ConstructBody(wp);
            this.MetaTitle = wp.Meta.MetaTitle;
            this.LanguageId = 3;
            this.IncludeInSitemap = true;
            this.BodyOverview = wp.Excerpt.Length > 0 ? wp.Excerpt : $"<p>{wp.Title}</p>";
            this.AllowComments = true;
            this.Tags = "";
            this.StartDateUtc = null;
            this.EndDateUtc = null;
            this.MetaDescription = wp.Meta.MetaDescription;
            this.LimitedToStores = false;
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
            this.Body = this.ConstructReferences(wp);
            this.Body = this.ConstructFeaturedText(wp);
            this.Body = this.ConstructLinks();
            this.Body = WPAutoP.WpAutoP(this.Body);
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
            newBody = newBody.Replace("[toc]", $"<div class=\"table-of-contents\">{tableOfContents}</div>");
            
            return newBody;
        }

        private string ConstructFeaturedText(WPBlog wp)
        {
            string newBody = this.Body;
            int i = 0;
            foreach (var featuredText in wp.Meta.FeaturedTexts)
            {
                newBody = newBody.Replace($"[featured_text position=\"{i}\"]", "<div class=\"featuredText\">" + featuredText + "</div>");
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
        
        public List<Picture> ConstructImages()
        {
            //<img class="alignnone wp-image-521052 size-full" src="https://www.svezaimunitet.com/wp-content/uploads/2021/06/crna-zova.jpg" alt="" width="1252" height="838" />
            var imageRegex = new Regex("<img.*src=\\\"(.*)\\\" alt=\\\"(.*)\\\" width=\\\"(\\d+)\\\" height=\\\"(\\d+)\\\"\\ />");
            int offset = 0;
            List<Picture> pictures = new();
            foreach (Match match in imageRegex.Matches(this.Body))
            {
                string imageName = match.Groups[1].Value.Split("/").Last();
                string alt = match.Groups[2].Value;
                int width = Int32.Parse(match.Groups[3].Value);
                var picture = new Picture()
                {
                    SeoFileName = imageName.Split(".")[0],
                    AltAttribute = alt,
                    Url = match.Groups[1].Value
                };
                pictures.Add(picture);
                if (width > 1000)
                {
                    this.Body = this.Body.Remove(match.Groups[3].Index + offset, $"width=\"{width}\"".Length)
                                    .Insert(match.Groups[3].Index + offset, "width=\"1000\"");
                    offset += "width=\"1000\"".Length - $"width=\"{width}\"".Length;
                }

                var newSrc = picture.VirtualPath.Replace("~", "") +
                             "/" +
                             picture.SeoFileName +
                             (width > 1000 ? "_1000" : "") +
                             ".jpeg";
                this.Body = this.Body.Replace(match.Groups[1].Value, 
                                    newSrc)
                                .Replace($"height=\"{match.Groups[4].Value}\"", "");
                offset += newSrc.Length - match.Groups[1].Length - $"height=\"{match.Groups[4].Value}\"".Length;

            }

            return pictures;
        }
    }
}