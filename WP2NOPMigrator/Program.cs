// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using MySqlConnector;
using WP2NOPMigrator;

string wordpressConnectionString = "Server=localhost;Database=sandzako;User=root;Password=admin;";

List<WPBlog> blogs = new();
using (var connection = new MySqlConnection(wordpressConnectionString))
{
    connection.Open();
    var query = "Select * from wp_posts as p left join wp_postmeta m on p.id = m.post_id where p.post_type = 'post' limit 1";
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
                    var meta_query = $"Select * from wp_postmeta where post_id={blog.Id}";
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

var nblog = new BlogPost(blogs.First());

var dbContext = new NopDbContext();
dbContext.BlogPosts.Add(nblog);
dbContext.SaveChanges();
var urlRecord = new UrlRecord(nblog);
dbContext.UrlRecords.Add(urlRecord);
dbContext.SaveChanges();

Console.WriteLine(blogs.First().Meta.FeaturedTexts.Count);