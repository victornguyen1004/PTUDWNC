﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;

namespace TatBlog.Data.Seeders
{
    public class DataSeeder : IDataSeeder
    {
        public readonly BlogDbContext _dbContext;

        public DataSeeder(BlogDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Initialize()
        {
            //tại vì có dữ liệu rồi nên nó sẽ ko thêm nữa
            _dbContext.Database.EnsureCreated();
            if (_dbContext.Posts.Any()) return;
            var authors = AddAuthor();
            var categories = AddCategory();
            var tags = AddTag();
            var posts = AddPost(authors, categories, tags);
        }

        private IList<Author> AddAuthor()
        {
            var authors = new List<Author>()
            {
                new()
                {
                    FullName = "Jason Mouth",
                    UrlSlug = "Jason-mouth",
                    Email="json@gmail.com",
                    JoinedDate=new DateTime(2022,10,21)

                },
                new()
                {
                    FullName = "Jessica Wonder",
                    UrlSlug = "Jessica Wonder",
                    Email="jessica665@motip.com",
                    JoinedDate=new DateTime(2020,4,19)
                },
                new()
                {
                    FullName = "Kathy Smith",
                    UrlSlug = "kathy Smith",
                    Email="kathy.smith@iworld.com",
                    JoinedDate=new DateTime(2016,9,6)
                }
            };
            _dbContext.Authors.AddRange(authors);
            _dbContext.SaveChanges();

            return authors;
        }
        private IList<Category> AddCategory()
        {
            var categories = new List<Category>()
            {
                new() { Name = ".NET Core", Description = ".NET Core", UrlSlug = "NET Core", ShowOnMenu = true },
                new() { Name = "Architecture", Description = "Architecture", UrlSlug = "Architecture", ShowOnMenu = true },
                new() { Name = "Messaging", Description = "Architecture", UrlSlug = "Architecture", ShowOnMenu = true },
                new() { Name = "OOP", Description = "Architecture", UrlSlug = "Architecture", ShowOnMenu = true },
                new() { Name = "Design Pattern", Description = "Design Pattern", UrlSlug = "Design Pattern", ShowOnMenu = true },
                new() { Name = "ASP.Net", Description = "ASP.Net", UrlSlug = "ASP.Net", ShowOnMenu = true },
                new() { Name = "Web Design", Description = "Web Design", UrlSlug = "Web Design", ShowOnMenu = true },
                new() { Name = "TailwindCss", Description = "TailwindCss", UrlSlug = "TailwindCss", ShowOnMenu = true },
                new() { Name = "Database", Description = "Database", UrlSlug = "Database", ShowOnMenu = true },
                new() { Name = "Design", Description = "Design", UrlSlug = "Design", ShowOnMenu = true }
            };

            _dbContext.Categories.AddRange(categories);
            _dbContext.SaveChanges();

            return categories;
        }

        private IList<Tag> AddTag()
        {
            var tags = new List<Tag>()
            {
                new() {Name="Google",Description="Google",UrlSlug="google"},
                new(){Name="ASP.net",Description="ASP.net",UrlSlug="asp.net"},
                new(){Name="TailwindCSS",Description="TailwindCSS",UrlSlug="tailwindcss"},
                new(){Name="ReactJS",Description="ReactJS",UrlSlug="reactjs"},
                new(){Name="API",Description="API",UrlSlug="api"},
                new() {Name="Ohio Final Boss",Description="Ohio Final Boss",UrlSlug="ohiofinalboss"},
                new(){Name="ADO.net",Description="ado.net",UrlSlug="ado.net"},
                new(){Name="Twitter",Description="Twitter",UrlSlug="twitter"},
                new(){Name="Apple",Description="Apple",UrlSlug="apple"},
                new(){Name="Machine Learning",Description="Machine Learning",UrlSlug="machinelearning"},
                new() {Name="NextJS",Description="NextJS",UrlSlug="nextjs"}
            };

            _dbContext.Tags.AddRange(tags);
            _dbContext.SaveChanges();

            return tags;
        }
        private IList<Post> AddPost(IList<Author> authors, IList<Category> categories, IList<Tag> tags)
        {


            var posts = new List<Post>()
            {
                new()
                {
                    Title="ASP.NET Core Diagnostic Scenarious",
                    ShortDescription="This is a short description",
                    Description="Here's a few greate DON'T and Do examples",
                    Meta="David and friends has a greate repository filled",
                    UrlSlug="ASPNET core diagnoretic-scenarious",
                    Published=true,
                    PostedDate=new DateTime(2021,9,30,10,20,0),
                    ModifiedDate=null,
                    ViewCount=10,
                    Author=authors[0],
                    Category=categories[0],
                    Tags=new List<Tag>()
                    {
                        tags[0]
                    }
                },
                new()
                {
                    Title="TailwindCSS",
                    ShortDescription="David and has a great responsive",
                    Description="Here's a few greate DON'T and Do examples",
                    Meta="David and friends has a greate repository filled",
                    UrlSlug="TailwindCSS is new css",
                    Published=true,
                    PostedDate=new DateTime(2021,9,30,10,20,0),
                    ModifiedDate=null,
                    ViewCount=10,
                    Author=authors[1],
                    Category=categories[1],
                    Tags=new List<Tag>()
                    {
                        tags[1]
                    }
                },
                new()
                {
                    Title="OKlalala",
                    ShortDescription="David and Friend has a greate reposive",
                    Description="Here's a few greate DON'T and Do examples",
                    Meta="David and friends has a greate repository filled",
                    UrlSlug="Ok lalala",
                    Published=true,
                    PostedDate = new DateTime(2021, 9, 30, 10, 20, 0), ModifiedDate = null,
                    ViewCount=10,
                    Author=authors[2],
                    Category=categories[2],
                    Tags=new List<Tag>()
                    {
                        tags[2]
                    }
                },
                new()
                {
                    Title="ReactJS is the new framework",
                    ShortDescription="David and Friend has a greate reposive",
                    Description="Here's a few greate DON'T and Do examples",
                    Meta="David and friends has a greate repository filled",
                    UrlSlug="reactjs is the new framework",
                    Published=true,
                    PostedDate = new DateTime(2021, 9, 30, 10, 20, 0), ModifiedDate = null,
                    ViewCount=10,
                    Author=authors[2],
                    Category=categories[3],
                    Tags=new List<Tag>()
                    {
                        tags[3]
                    }
                },
                new()
                {
                    Title="ReactJS is the new framework",
                    ShortDescription="David and Friend has a greate reposive",
                    Description="Here's a few greate DON'T and Do examples",
                    Meta="David and friends has a greate repository filled",
                    UrlSlug="reactjs is the new framework",
                    Published=true,
                    PostedDate = new DateTime(2021, 9, 30, 10, 20, 0), ModifiedDate = null,
                    ViewCount=10,
                    Author=authors[2],
                    Category=categories[3],
                    Tags=new List<Tag>()
                    {
                        tags[3]
                    }
                },
                new()
                {
                    Title="ReactJS is the new framework",
                    ShortDescription="David and Friend has a greate reposive",
                    Description="Here's a few greate DON'T and Do examples",
                    Meta="David and friends has a greate repository filled",
                    UrlSlug="reactjs is the new framework",
                    Published=true,
                    PostedDate = new DateTime(2021, 9, 30, 10, 20, 0), ModifiedDate = null,
                    ViewCount=10,
                    Author=authors[0],
                    Category=categories[4],
                    Tags=new List<Tag>()
                    {
                        tags[4]
                    }
                },
                new()
                {
                    Title="ReactJS is the new framework",
                    ShortDescription="David and Friend has a greate reposive",
                    Description="Here's a few greate DON'T and Do examples",
                    Meta="David and friends has a greate repository filled",
                    UrlSlug="reactjs is the new framework",
                    Published=true,
                    PostedDate = new DateTime(2021, 9, 30, 10, 20, 0), ModifiedDate = null,
                    ViewCount=10,
                    Author=authors[1],
                    Category=categories[5],
                    Tags=new List<Tag>()
                    {
                        tags[2]
                    }
                },
                new()
                {
                    Title="RESTful API new tech",
                    ShortDescription="David and Friend has a greate reposive",
                    Description="Here's a few greate DON'T and Do examples",
                    Meta="David and friends has a greate repository filled",
                    UrlSlug="RESTful API new tech",
                    Published=true,
                    PostedDate = new DateTime(2021, 9, 30, 10, 20, 0), ModifiedDate = null,
                    ViewCount=10,
                    Author=authors[1],
                    Category=categories[0],
                    Tags=new List<Tag>()
                    {
                        tags[0]
                    }
                }
            };

            _dbContext.Posts.AddRange(posts);
            _dbContext.SaveChanges();

            return posts;

        }
    }
}
