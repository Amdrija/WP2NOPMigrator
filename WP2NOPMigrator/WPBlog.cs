using System;
using System.Collections.Generic;
using MySqlConnector;

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
        
        public static List<WPBlog> LoadBlogs(string wordpressConnectionString)
        {
                        
            List<WPBlog> blogs = new();
            using (var connection = new MySqlConnection(wordpressConnectionString))
            {
                connection.Open();
                var query = "Select * from wp_posts as p where p.post_type = 'post' AND NOT post_title LIKE '%Folna%' AND post_title LIKE '%Crna Zova%' limit 1;";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var blog = new WPBlog
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                Title = reader.GetString(reader.GetOrdinal("post_title")),
                                Content = reader.GetString(reader.GetOrdinal("post_content")),
                                Excerpt = reader.GetString(reader.GetOrdinal("post_excerpt")),
                                DateGMT = reader.GetDateTime(reader.GetOrdinal("post_date_gmt")),
                            };

                            using (var metaConnection = new MySqlConnection(wordpressConnectionString))
                            {
                                metaConnection.Open();
                                var meta_query = $"Select * from wp_postmeta where post_id={blog.Id};";
                                using (var metaCommand = new MySqlCommand(meta_query, metaConnection))
                                {
                                    using (var metaReader = metaCommand.ExecuteReader())
                                    {
                                        var meta = new WPPostMeta();
                                        while (metaReader.Read())
                                        {
                                            string meta_key = metaReader.GetString(metaReader.GetOrdinal("meta_key"));
                                            switch (meta_key)
                                            {
                                                case "_yoast_wpseo_title":
                                                    meta.MetaTitle = metaReader.GetString(metaReader.GetOrdinal("meta_value"));
                                                    break;
                                                case "_yoast_wpseo_metadescription":
                                                    meta.MetaDescription =
                                                        metaReader.GetString(metaReader.GetOrdinal("meta_value"));
                                                    break;
                                                case "reference":
                                                    meta.Reference =
                                                        metaReader.GetString(metaReader.GetOrdinal("meta_value"));
                                                    break;
                                                
                                                default:
                                                    if (meta_key.StartsWith("izdvojeni_tekstovi_"))
                                                    {
                                                        meta.FeaturedTexts.Add(
                                                            metaReader.GetString(metaReader.GetOrdinal("meta_value")));
                                                    }

                                                    break;
                                            }

                                            blog.Meta = meta;
                                        }
                                    }
                                }
                            }

                            blogs.Add(blog);
                        }
                    }
                }
            }

            return blogs;
        }
    }
}