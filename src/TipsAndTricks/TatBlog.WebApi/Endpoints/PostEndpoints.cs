using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TatBlog.Core.Collections;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.WebApi.Filters;
using TatBlog.WebApi.Media;
using TatBlog.WebApi.Models;

namespace TatBlog.WebApi.Endpoints
{
    public static class PostEndPoints
    {
        public static WebApplication MapPostEndpoints(
            this WebApplication app)
        {
            var routeGroupBuilder = app.MapGroup("/api/posts");

            routeGroupBuilder.MapGet("/", GetPost)
                .WithName("GetPost")
                .Produces<ApiResponse<PaginationResult<PostDto>>>();

            routeGroupBuilder.MapGet("/featured/{limit:int}", GetFeaturedPost)
                .WithName("GetFeaturedPost")
                .Produces<ApiResponse<PaginationResult<PostDto>>>();

            routeGroupBuilder.MapGet("/random/{limit:int}", GetRandomPost)
            .WithName("GetRandomPost")
                .Produces<ApiResponse<PaginationResult<PostDto>>>();

            routeGroupBuilder.MapGet("/archives/{month:int}", GetArchives)
                .WithName("GetArchives")
                .Produces<ApiResponse<PaginationResult<MonthlyPostsCountItem>>>();

            routeGroupBuilder.MapGet("/{id:int}", GetPostById)
                .WithName("GetPostById")
                .Produces<ApiResponse<PostDetail>>()
                .Produces(404);

            routeGroupBuilder.MapGet("/byslug/{slug:regex(^[a-z0-9_-]+$)}", GetPostBySlug)
                .WithName("GetPostBySlug")
                .Produces<ApiResponse<PaginationResult<PostDetail>>>()
                .Produces(404);

            routeGroupBuilder.MapPost("/", AddPost)
                .WithName("AddPost")
                .AddEndpointFilter<ValidatorFilter<PostEditModel>>()
                .Produces(201)
                .Produces(400)
                .Produces(409);

            routeGroupBuilder.MapPut("/{id:int}", UpdatePost)
                .WithName("UpdatePost")
                .AddEndpointFilter<ValidatorFilter<PostEditModel>>()
                .Produces(204)
                .Produces(400)
                .Produces(409);

            routeGroupBuilder.MapGet("/TogglePost/{id:int}", TogglePublicStatus)
                .WithName("TogglePublicStatus")
                .Produces(204)
                .Produces(404);

            routeGroupBuilder.MapPost("/{id:int}/picture", SetPostPicture)
                .WithName("SetPostPicture")
                .Accepts<IFormFile>("multipart/form-data")
                .Produces<ApiResponse<string>>()
                .Produces(400);

            routeGroupBuilder.MapDelete("/{id:int}", DeletePost)
                .WithName("DeletePost")
                .Produces(204)
                .Produces(404);

            return app;
        }

        private static async Task<IResult> GetPost(
            [AsParameters] PostFilterModel model,
            IBlogRepository blogRepo,
            IMapper mapper)
        {
            var postsQuery = mapper.Map<PostQuery>(model);

            var postList = await blogRepo.GetPagedPostsAsync(
                postsQuery,
                model,
                p => p.ProjectToType<PostDto>());

            var paginationResult = new PaginationResult<PostDto>(postList);
            return Results.Ok(ApiResponse.Success(paginationResult));
        }

        private static async Task<IResult> GetFeaturedPost(
            int limit,
            IBlogRepository blogRepo,
            IMapper mapper)
        {
            var postList = await blogRepo.GetPopularAriticlesAsync(limit);
            var postDto = mapper.Map<IList<PostDto>>(postList);


            return Results.Ok(ApiResponse.Success(postDto));
        }

        private static async Task<IResult> GetRandomPost(
            int limit,
            IBlogRepository blogRepo,
            IMapper mapper)
        {
            var postList = await blogRepo.GetRandomPostsAsync(limit);
            var postDto = mapper.Map<IList<PostDto>>(postList);


            return Results.Ok(ApiResponse.Success(postDto));
        }

        private static async Task<IResult> GetArchives(
            int month,
            IBlogRepository blogRepo)
        {
            var result = await blogRepo.CountTotalPostFromMonthsAsync(month);
            return Results.Ok(ApiResponse.Success(result));
        }

        private static async Task<IResult> GetPostById(
            int id,
            IBlogRepository blogRepo,
            IMapper mapper)
        {
            var post = await blogRepo.GetPostByIdAsync(id, true);

            var postDetail = mapper.Map<PostDetail>(post);



            return postDetail != null
                    ? Results.Ok(ApiResponse.Success(postDetail))
                    : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Không tìm thấy bài viết với id: `{id}`"));

        }

        private static async Task<IResult> GetPostBySlug(
            [FromRoute] string slug,
            IBlogRepository blogRepo,
            IMapper mapper)
        {
            var post = await blogRepo.GetPostAsync(0, 0, 0, slug);

            var postDetail = mapper.Map<PostDto>(post);
            return postDetail != null
                ? Results.Ok(ApiResponse.Success(postDetail))
                : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Không tìm thấy bài viết với mã định danh: `{slug}`"));
        }

        private static async Task<IResult> AddPost(
            PostEditModel model,
            IBlogRepository blogRepo,
            IAuthorRepository authorRepo,
            IMapper mapper)
        {
            if (await blogRepo.IsPostSlugExistedAsync(0, model.UrlSlug))
            {
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict,
                    $"Slug '{model.UrlSlug}' đã được sử dụng"));
            }

            var isExitsCategory = await blogRepo.GetCategoryByIdAsync(model.CategoryId);
            var isExitsAuthor = await authorRepo.GetAuthorByIdAsync(model.AuthorId);

            if (isExitsAuthor == null || isExitsCategory == null)
            {
                return Results.Ok(ApiResponse.Fail(
                    HttpStatusCode.NotFound,
                    $"Mã tác giả hoặc chủ đề không tồn tại!"));
            }

            var post = mapper.Map<Post>(model);
            post.PostedDate = DateTime.Now;

            await blogRepo.AddOrUpdatePostAsync(post, model.SelectedTags);

            return Results.Ok(ApiResponse.Success(mapper.Map<PostDetail>(post), HttpStatusCode.Created));
        }

        private static async Task<IResult> UpdatePost(
            int id,
            PostEditModel model,
            IBlogRepository blogRepo,
            IAuthorRepository authorRepo,
            IMapper mapper)
        {
            var post = await blogRepo.GetPostByIdAsync(id);

            if (post == null)
            {
                return Results.Ok(ApiResponse.Fail(
                    HttpStatusCode.NotFound,
                    $"Không tìm thấy bài viết với id: `{id}`"));
            }

            if (await blogRepo.IsPostSlugExistedAsync(id, model.UrlSlug))
            {
                return Results.Ok(ApiResponse.Fail(
                    HttpStatusCode.Conflict,
                    $"Slug {model.UrlSlug} đã được sử dụng"));
            }

            var isExitsCategory = await blogRepo.GetCategoryByIdAsync(model.CategoryId);
            var isExitsAuthor = await authorRepo.GetAuthorByIdAsync(model.AuthorId);

            if (isExitsAuthor == null || isExitsCategory == null)
            {
                return Results.Ok(ApiResponse.Fail(
                    HttpStatusCode.NotFound,
                    $"Mã tác giả hoặc chủ đề không tồn tại!"));
            }

            mapper.Map(model, post);
            post.Id = id;
            post.Category = null;
            post.Author = null;

            return await blogRepo.AddOrUpdatePostAsync(post, model.SelectedTags) != null
                ? Results.Ok(ApiResponse.Success("Post is updated", HttpStatusCode.NoContent))
                : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, "Không tìm thấy bài viết"));
        }

        private static async Task<IResult> TogglePublicStatus(
            int id,
            IBlogRepository blogRepo)
        {
            var oldPost = await blogRepo.GetPostByIdAsync(id);

            if (oldPost == null)
            {
                return Results.Ok(ApiResponse.Fail(
                    HttpStatusCode.NotFound,
                    $"Không tìm thấy bài viết với id: `{id}`"));
            }

            await blogRepo.TogglePublishedStatusAsync(id);
            return Results.Ok(ApiResponse.Success("Toggle post success"));
        }


        private static async Task<IResult> SetPostPicture(
            int id, IFormFile imageFile,
            IBlogRepository blogRepo,
            IMediaManager mediaManager)
        {
            var oldPost = await blogRepo.GetPostByIdAsync(id);
            if (oldPost == null)
            {
                return Results.Ok(ApiResponse.Fail(
                    HttpStatusCode.NotFound,
                    $"Không tìm thấy bài viết với id: `{id}`"));
            }

            await mediaManager.DeleteFileAsync(oldPost.ImageUrl);

            var imageUrl = await mediaManager.SaveFileAsync(
                imageFile.OpenReadStream(),
                imageFile.FileName, imageFile.ContentType);

            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return Results.Ok(ApiResponse.Fail(
                    HttpStatusCode.BadRequest,
                    "Không lưu được tệp"));
            }

            await blogRepo.SetImageUrlAsync(id, imageUrl);
            return Results.Ok(ApiResponse.Success(imageUrl));
        }

        private static async Task<IResult> DeletePost(
            int id,
            IBlogRepository blogRepo,
            IMediaManager _media)
        {
            var oldPost = await blogRepo.GetPostByIdAsync(id);

            await _media.DeleteFileAsync(oldPost.ImageUrl);

            return await blogRepo.DeletePostByIdAsync(id)
                ? Results.Ok(ApiResponse.Success(HttpStatusCode.NoContent))
                : Results.Ok(ApiResponse.Fail(
                    HttpStatusCode.NotFound,
                    $"Không tìm thấy bài viết với id: `{id}`"));

        }
    }
}