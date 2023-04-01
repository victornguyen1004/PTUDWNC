using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using TatBlog.Core.Collections;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.WebApi.Extensions;
using TatBlog.WebApi.Filters;
using TatBlog.WebApi.Media;
using TatBlog.WebApi.Models;

namespace TatBlog.WebApi.Endpoints
{
    public static class AuthorEndpoints
    {
        public static WebApplication MapAuthorEndpoints(
            this WebApplication app)
        {

            var routeGroupBuilder = app.MapGroup("/api/authors");

            routeGroupBuilder.MapGet("/", GetAuthors)
                .WithName("GetAuthors")
                .Produces<PaginationResult<AuthorItem>>();

            routeGroupBuilder.MapGet("/{id:int}", GetAuthorDetails)
                .WithName("GetAuthorById")
                .Produces<AuthorItem>()
                .Produces(404);

            routeGroupBuilder.MapGet(
                "/{slug:regex(^[a-z0-9 -]+$)}/posts", GetPostsByAuthorSlug)
                .WithName("GetPostsByAuthorSlug")
                .Produces<PaginationResult<PostDto>>();

            routeGroupBuilder.MapGet("/best/{limit:int}", GetAuthorsWithMostPosts)
                .WithName("GetAuthorsWithMostPosts")
                .Produces<AuthorItem>();

            routeGroupBuilder.MapPost("/", AddAuthor)
                .WithName("AddNewAuthor")
                .AddEndpointFilter<ValidatorFilter<AuthorEditModel>>()
                .Produces(201)
                .Produces(400)
                .Produces(409);

            routeGroupBuilder.MapPost("/{id:int}/avatar", SetAuthorPicture)
                .WithName("SetAuthorPicture")
                .Accepts<IFormFile>("multipart/form-data")
                .Produces<string>()
                .Produces(400);

            routeGroupBuilder.MapPut("/{id:int}", UpdateAuthor)
                .WithName("UpdateAnAuthor")
                .AddEndpointFilter<ValidatorFilter<AuthorEditModel>>()
                .Produces(204)
                .Produces(400)
                .Produces(409);

            routeGroupBuilder.MapDelete("/{id:int}", DeleteAuthor)
                .WithName("DeleteAnAuthor")
                .Produces(204)
                .Produces(404);

            return app;
        }

        private static async Task<IResult> GetAuthors(
            [AsParameters] AuthorFilterModel model,
            IAuthorRepository authorRepo)
        {

            var authorsList = await authorRepo.GetPagedAuthorsAsync(model, model.Name);

            var paginationResult = new PaginationResult<AuthorItem>(authorsList);

            return Results.Ok(paginationResult);
        }

        private static async Task<IResult> GetAuthorDetails(
            int id,
            IAuthorRepository authorRepo,
            IMapper mapper)
        {

            var author = await authorRepo.GetCachedAuthorByIdAsync(id);
            return author == null ? Results.NotFound($"Không tìm thấy tác giả có mã số {id}")
                                  : Results.Ok(mapper.Map<AuthorItem>(author));
        }

        private static async Task<IResult> GetPostsByAuthorId(
            int id,
            [AsParameters] PagingModel pagingModel,
            IBlogRepository blogRepo)
        {

            var postQuery = new PostQuery()
            {
                AuthorId = id,
                Published = true
            };

            var postsList = await blogRepo.GetPagedPostsAsync(
                postQuery, pagingModel,
                posts => posts.ProjectToType<PostDto>());

            var paginationResult = new PaginationResult<PostDto>(postsList);

            return Results.Ok(paginationResult);
        }

        private static async Task<IResult> GetPostsByAuthorSlug(
            [FromRoute] string slug,
            [AsParameters] PagingModel pagingModel,
            IBlogRepository blogRepo)
        {

            var postQuery = new PostQuery()
            {
                AuthorSlug = slug,
                Published = true
            };

            var postsList = await blogRepo.GetPagedPostsAsync(
                postQuery, pagingModel,
                posts => posts.ProjectToType<PostDto>());

            var paginationResult = new PaginationResult<PostDto>(postsList);

            return Results.Ok(paginationResult);
        }

        private static async Task<IResult> AddAuthor(
            AuthorEditModel model,
            IValidator<AuthorEditModel> validator,
            IAuthorRepository authorRepo,
            IMapper mapper)
        {

            if (await authorRepo
                        .IsAuthorSlugExistedAsync(0, model.UrlSlug))
            {
                return Results.Conflict(
                    $"Slug '{model.UrlSlug}' đã được sử dụng");
            }

            var author = mapper.Map<Author>(model);
            await authorRepo.AddOrUpdateAsync(author);

            return Results.CreatedAtRoute(
                "GetAuthorById", new { author.Id },
                mapper.Map<AuthorItem>(author));
        }

        private static async Task<IResult> SetAuthorPicture(
            int id, IFormFile imageFile,
            IAuthorRepository authorRepo,
            IMediaManager mediaManager)
        {

            var imageUrl = await mediaManager.SaveFileAsync(
                imageFile.OpenReadStream(),
                imageFile.FileName, imageFile.ContentType);

            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return Results.BadRequest("Không lưu được tập tin");
            }

            await authorRepo.SetImageUrlAsync(id, imageUrl);
            return Results.Ok(imageUrl);
        }

        private static async Task<IResult> UpdateAuthor(
            int id, AuthorEditModel model,
            IValidator<AuthorEditModel> validator,
            IAuthorRepository authorRepo,
            IMapper mapper)
        {

            if (await authorRepo
                        .IsAuthorSlugExistedAsync(id, model.UrlSlug))
            {
                return Results.Conflict(
                    $"Slug '{model.UrlSlug}' đã được sử dụng");
            }

            var author = mapper.Map<Author>(model);

            author.Id = id;

            return await authorRepo.AddOrUpdateAsync(author)
                    ? Results.NoContent()
                    : Results.NotFound();
        }

        private static async Task<IResult> DeleteAuthor(
            int id,
            IAuthorRepository authorRepo)
        {
            return await authorRepo.DeleteAuthorAsync(id)
                    ? Results.NoContent()
                    : Results.NotFound($"Couldn't find author with id = {id}");
        }

        private static async Task<IResult> GetAuthorsWithMostPosts(
            int limit,
            IAuthorRepository authorRepo)
        {

            var authors = await authorRepo.GetAuthorsWithMostPost(limit);
            return Results.Ok(authors);
        }

    }
}