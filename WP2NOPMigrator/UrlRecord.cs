namespace WP2NOPMigrator
{
    public class UrlRecord
    {
        public string EntityName = "BlogPost";

        public string Slug { get; set; }

        public int EntityId { get; set; }

        public bool IsActive = true;

        public int LanguageId = 2;

        public UrlRecord(Blog blog)
        {
            this.Slug = blog.createLinkFromTitle(blog.Title);
            this.EntityId = blog.Id;
        }
    }
}