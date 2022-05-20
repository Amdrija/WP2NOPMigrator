using System;

namespace WP2NOPMigrator
{
    public class ActivityLog
    {
        public int Id { get; set; }

        public string Comment { get; set; }
        
        public string IpAddress { get; set; }

        public string EntityName { get; set; }

        public int ActivityLogTypeId { get; set; }

        public int CustomerId { get; set; }

        public int EntityId { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public ActivityLog(int entityId)
        {
            this.Comment = $"Added a new blog post (ID = {entityId})";
            this.IpAddress = "127.0.0.1";
            this.EntityName = "BlogPost";
            this.ActivityLogTypeId = 4;
            this.CustomerId = 1;
            this.EntityId = entityId;
            this.CreatedOnUtc = DateTime.UtcNow;
        }
    }
}