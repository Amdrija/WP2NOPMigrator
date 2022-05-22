using System;
using System.IO;
using System.Net;

namespace WP2NOPMigrator
{
    public class PictureBinary
    {
        public int Id { get; set; }

        public int PictureId { get; set; }

        public byte[] BinaryData { get; set; }

        public PictureBinary()
        {
            
        }

        public PictureBinary(Picture picture)
        {
            this.PictureId = picture.Id;

            using (var client = new WebClient())
            {
                var url = $"https://www.svezaimunitet.com{picture.Url}";
                this.BinaryData = client.DownloadData(url);
            }
        }
    }
}