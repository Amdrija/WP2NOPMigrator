// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using WP2NOPMigrator;

Console.WriteLine("Initializing Migrator");
if (Environment.GetCommandLineArgs().Length != 3)
{
    Console.WriteLine("You need to specify wordpress database and nopcommerce database connection strings as arguments.");
    return;
}


string wordpressConnectionString = Environment.GetCommandLineArgs()[1];
string nopCommerceConnectionString = Environment.GetCommandLineArgs()[2];

Console.WriteLine("Loading Wordpress blog posts");
List<WPBlog> wpBlogs = WPBlog.LoadBlogs(wordpressConnectionString);
Console.WriteLine("Finished loading Wordpress blog posts");
List<BlogPost> blogPosts = wpBlogs.Select((wp) => new BlogPost(wp)).ToList();

Console.WriteLine("Initializing DB context");

var contextOptions = new DbContextOptionsBuilder<NopDbContext>()
                     .UseSqlServer(nopCommerceConnectionString)
                     .Options;
var dbContext = new NopDbContext(contextOptions);

Console.WriteLine("Adding blog posts to NOP Commerce");
foreach (var blogPost in blogPosts)
{
    dbContext.BlogPosts.Add(blogPost);
}
dbContext.SaveChanges();
Console.WriteLine("Finished adding blog posts to NOP Commerce");
Console.WriteLine("Adding URLs and Logs to NOP Commerce");
foreach (var blogPost in blogPosts)
{
    var urlRecord = new UrlRecord(blogPost);
    var activityLog = new ActivityLog(blogPost.Id);
    dbContext.UrlRecords.Add(urlRecord);
    dbContext.ActivityLogs.Add(activityLog);
}
dbContext.SaveChanges();
Console.WriteLine("Finished adding URLs and Logs to NOP Commerce");