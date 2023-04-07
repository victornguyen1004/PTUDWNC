using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TatBlog.Core.Collections;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.WebApi.Filters;
using TatBlog.WebApi.Models;

namespace TatBlog.WebApi.Endpoints
{
    public static class TagEndPoints
    {
        public static WebApplication MapTagEndpoints(
       this WebApplication app)
        {
            var routeGroupBuilder = app.MapGroup("/api/tags");

            routeGroupBuilder.MapGet("/", GetTag)
                .WithName("GetTag")
                .Produces<ApiResponse<PaginationResult<TagItem>>>();

            routeGroupBuilder.MapGet("/{id:int}", GetTagById)
                .WithName("GetTagById")
                .Produces<ApiResponse<TagDto>>();

            routeGroupBuilder.MapGet("/{slug:regex(^[a-z0-9_-]+$)}/posts", GetPostsByTagSlug)
                .WithName("GetPostsByTagSlug")
                .Produces<ApiResponse<PaginationResult<PostDto>>>();

            routeGroupBuilder.MapPost("/", AddTag)
                .WithName("AddTag")
                .AddEndpointFilter<ValidatorFilter<TagEditModel>>()
                .Produces(201)
                .Produces(400)
                .Produces(409);

            routeGroupBuilder.MapPut("/{id:int}", UpdateTag)
                .WithName("UpdateTag")
                .AddEndpointFilter<ValidatorFilter<TagEditModel>>()
                .Produces(204)
                .Produces(400)
                .Produces(409);

            routeGroupBuilder.MapDelete("/{id:int}", DeleteTag)
                .WithName("DeleteTag")
                .Produces(204)
                .Produces(404);

            return app;
        }

        private static async Task<IResult> GetTag(
            [AsParameters] TagFilterModel model,
            IBlogRepository blogRepo)
        {

            var tagsList = await blogRepo.GetPagedTagsAsync(model, model.Name);
            var paginationResult = new PaginationResult<TagItem>(tagsList);

            return Results.Ok(ApiResponse.Success(paginationResult));
        }

        private static async Task<IResult> GetTagById(
            int id,
            IBlogRepository blogRepository,
            IMapper mapper)
        {
            var tag = await blogRepository.GetTagByIdAsync(id);
            var tagItem = mapper.Map<TagDto>(tag);

            return tag == null
                ? Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, "Tag is not found"))
                : Results.Ok(ApiResponse.Success(tagItem));
        }

        private static async Task<IResult> GetPostsByTagSlug(
            [FromRoute] string slug,
            [AsParameters] PagingModel pagingModel,
            IBlogRepository blogRepository)
        {
            var postsQuery = new PostQuery()
            {
                TagSlug = slug,
                Published = true
            };

            var postList = await blogRepository.GetPagedPostsAsync(
                postsQuery,
                pagingModel,
                p => p.ProjectToType<PostDto>());

            var paginationResult = new PaginationResult<PostDto>(postList);
            return Results.Ok(ApiResponse.Success(paginationResult));
        }

        private static async Task<IResult> AddTag(
            TagEditModel model,
            IBlogRepository blogRepository,
            IMapper mapper)
        {
            if (await blogRepository.IsTagSlugExistedAsync(0, model.UrlSlug))
            {
                return Results.Ok(ApiResponse.Fail(
                    HttpStatusCode.Conflict,
                    $"Slug {model.UrlSlug} đã được sử dụng"));
            }

            var tag = mapper.Map<Tag>(model);

            await blogRepository.AddOrUpdateTagAsync(tag);

            return Results.Ok(ApiResponse.Success(
                mapper.Map<TagItem>(tag), HttpStatusCode.Created));
        }

        private static async Task<IResult> UpdateTag(
            int id,
            TagEditModel model,
            IBlogRepository blogRepository,
            IMapper mapper)
        {
            if (await blogRepository.IsTagSlugExistedAsync(id, model.UrlSlug))
            {
                return Results.Ok(ApiResponse.Fail(
                    HttpStatusCode.Conflict,
                    $"Slug {model.UrlSlug} đã được sử dụng"));
            }

            var tag = mapper.Map<Tag>(model);
            tag.Id = id;

            return await blogRepository.AddOrUpdateTagAsync(tag) != null
                ? Results.Ok(ApiResponse.Success(HttpStatusCode.NoContent))
                : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, "Tag is not found"));
        }

        private static async Task<IResult> DeleteTag(
            int id,
            IBlogRepository blogRepository)
        {
            return await blogRepository.DeleteTagByIdAsync(id)
                ? Results.Ok(ApiResponse.Success("Tag is deleted", HttpStatusCode.NoContent))
                : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Không tìm thấy thẻ với id: `{id}`"));
        }
    }
}