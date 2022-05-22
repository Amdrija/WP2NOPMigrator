using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace WP2NOPMigrator
{
    public class Picture
    {
        public int Id { get; set; }

        public string MimeType { get; set; }

        public string SeoFileName { get; set; }
        
        public string AltAttribute { get; set; }

        public string TitleAttribute { get; set; }

        public bool IsNew { get; set; }

        public string VirtualPath { get; set; }
        
        public string Url { get; set; }

        public Picture()
        {
            this.MimeType = "image/jpeg";
            this.IsNew = false;
            this.VirtualPath = "~/images/uploaded";
        }

        
    }
}