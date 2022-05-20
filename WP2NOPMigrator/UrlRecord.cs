namespace WP2NOPMigrator
{
    public class UrlRecord
    {
        public int Id { get; set; }
        
        public string EntityName { get; set; }

        public string Slug { get; set; }

        public int EntityId { get; set; }

        public bool IsActive { get; set; }

        public int LanguageId { get; set; }

        public UrlRecord()
        {
            this.EntityName = "BlogPost";
            this.IsActive = true;
            this.LanguageId = 2;
        }
        
        public UrlRecord(BlogPost blogPost)
        {
            this.EntityName = "BlogPost";
            this.Slug = blogPost.createLinkFromTitle(blogPost.Title);
            this.EntityId = blogPost.Id;
            this.IsActive = true;
            this.LanguageId = 2;
        }
    }
}