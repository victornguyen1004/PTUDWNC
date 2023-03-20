using TatBlog.WebApp.Extensions;
using Microsoft.EntityFrameworkCore;
using TatBlog.Data.Contexts;
using TatBlog.Data.Seeders;
using TatBlog.Services.Blogs;

var builder = WebApplication.CreateBuilder(args);
{
    builder
        .ConfigureMvc()
        .ConfigureServices();
}

var app = builder.Build();
{
    app.UseRequestPipeLine();
    app.UseBlogRoutes();
    app.UseDataSeeder();
}

app.Run();

app.MapControllerRoute(
    name: "posts-by-category",
    pattern: "blog/category/{slug}",
    defaults: new { controller = "Blog", action = "Category" });

app.MapControllerRoute(
    name: "posts-by-tag",
    pattern: "blog/tag/{slug}",
    defaults: new { controller = "Blog", action = "Tag" });

app.MapControllerRoute(
    name: "single-post",
    pattern: "blog/post/{year:int}/{month:int}/{day:int}/{slug}",
    defaults: new { controller = "Blog", action = "Post" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Blog}/{action=Index}/{id?}");