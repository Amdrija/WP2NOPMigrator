using System;

namespace WP2NOPMigrator
{
    public class WPBlog
    {
        public int Id { get; set; }
        public string Title { get; set; }
        
        public string Content { get; set; }

        public string Excerpt { get; set; }

        public DateTime DateGMT { get; set; }
        
        public WPPostMeta Meta { get; set; }
    }
}