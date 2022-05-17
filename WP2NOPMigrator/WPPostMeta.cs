using System.Collections.Generic;

namespace WP2NOPMigrator
{
    public class WPPostMeta
    {
        public string MetaTitle { get; set; }

        public string MetaDescription { get; set; }

        public string Reference { get; set; }

        public List<string> FeaturedTexts { get; } = new ();
    }
}