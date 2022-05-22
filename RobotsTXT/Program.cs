// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WP2NOPMigrator;

string nopCommerceConnectionString = Environment.GetCommandLineArgs()[1];
var contextOptions = new DbContextOptionsBuilder<NopDbContext>()
                     .UseSqlServer(nopCommerceConnectionString)
                     .Options;
var dbContext = new NopDbContext(contextOptions);

List<string> slugs = dbContext.UrlRecords
                              .Where(b => b.EntityName == "BlogPost")
                              .Select(b => b.Slug)
                              .ToList();
Console.WriteLine(slugs.Count);
File.AppendAllLines("/Users/andrija/Documents/Programiranje/C#/WP2NOPMigrator/RobotsTXT/robots.txt", slugs.Select(slug => $"Disallow: /{slug}"));