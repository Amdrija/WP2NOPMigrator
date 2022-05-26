using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace WP2NOPMigrator
{
    public class Picture
    {
        public int Id { get; set; }

        public string MimeType
        {
            get;
            set;
        }

        public string SeoFileName { get; set; }
        
        public string AltAttribute { get; set; }

        public string TitleAttribute { get; set; }

        public bool IsNew { get; set; }

        public string VirtualPath { get; set; }
        
        public string Url { get; set; }
        
        public string Extension { get; set; }

        private readonly Dictionary<string, string> mimeTypes = new Dictionary<string, string>()
        {
            { "jpg", "image/jpeg" },
            {"jpeg", "image/jpeg"},
            {"png", "image/png"}
        };

        public Picture()
        {
            this.IsNew = false;
            this.VirtualPath = "~/images/uploaded";
        }

        public Picture(string imageName, string altAttribute, string url)
        {
            this.SeoFileName = imageName.Split(".")[0];
            this.AltAttribute = altAttribute;
            this.IsNew = false;
            this.VirtualPath = "~/images/uploaded";
            this.Url = url;
            this.Extension = imageName.Split(".")[1];
            this.MimeType = this.mimeTypes[this.Extension];
        }
    }
}