using TatBlog.WebApp.Extensions;
using Microsoft.EntityFrameworkCore;
using TatBlog.Data.Contexts;
using TatBlog.Data.Seeders;
using TatBlog.Services.Blogs;
using Microsoft.EntityFrameworkCore.Design;
using TatBlog.WebApp.Mapsters;
using TatBlog.WebApp.Validations;

var builder = WebApplication.CreateBuilder(args);
{
    builder
        .ConfigureMvc()
        .ConfigureServices()
        .ConfigureFluentValidation
        .ConfigureMapster();
}

var app = builder.Build();
{
    app.UseRequestPipeLine();
    app.UseBlogRoutes();
    app.UseDataSeeder();
}

app.Run();