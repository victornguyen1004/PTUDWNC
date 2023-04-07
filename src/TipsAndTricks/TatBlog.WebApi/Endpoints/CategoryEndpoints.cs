using FluentValidation;
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
    public static class CategoryEndpoints
    {

        public static WebApplication MapCategoryEndpoints(
            this WebApplication app)
        {

            var routeGroupBuilder = app.MapGroup("/api/categories");

            routeGroupBuilder.MapGet("/", GetCategories)
                .WithName("GetCategories")
                .Produces<ApiResponse<PaginationResult<CategoryItem>>>();

            routeGroupBuilder.MapGet("/{id:int}", GetCategoryDetail)
                .WithName("GetCategoryById")
                .Produces<ApiResponse<CategoryItem>>()
                .Produces(404);

            routeGroupBuilder.MapGet("/{slug:regex(^[a-z0-9_-]+$)}/posts", GetPostsByCategorySlug)
                .WithName("GetPostsByCategorySlug")
                .Produces<ApiResponse<PaginationResult<PostDto>>>();

            routeGroupBuilder.MapPost("/", AddCategory)
                .WithName("AddNewCategory")
                .AddEndpointFilter<ValidatorFilter<CategoryEditModel>>()
                .Produces(201)
                .Produces(400)
                .Produces(409);

            routeGroupBuilder.MapPut("/{id:int}", UpdateCategory)
                .WithName("UpdateACategory")
                .AddEndpointFilter<ValidatorFilter<CategoryEditModel>>()
                .Produces(204)
                .Produces(400)
                .Produces(409);

            routeGroupBuilder.MapDelete("/{id:int}", DeleteCategory)
                .WithName("DeleteACategory")
                .Produces(204)
                .Produces(404);

            return app;
        }

        private static async Task<IResult> GetCategories(
            [AsParameters] CategoryFilterModel model,
            IBlogRepository blogRepo)
        {

            var categoriesList = await blogRepo.GetPagedCategoriesAsync(model, model.Name);

            var paginationResult = new PaginationResult<CategoryItem>(categoriesList);

            return Results.Ok(ApiResponse.Success(paginationResult));
        }

        private static async Task<IResult> GetCategoryDetail(
            int id,
            IBlogRepository blogRepo,
            IMapper mapper)
        {

            var category = await blogRepo.GetCategoryByIdAsync(id);
            return category == null ? Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Không tìm thấy chủ đề có mã số `{id}`"))
                                    : Results.Ok(ApiResponse.Success(mapper.Map<CategoryItem>(category)));

        }


        //private static async Task<IResult> GetPostsByCategoryId(
        //    int id,
        //    [AsParameters] PagingModel pagingModel,
        //    IBlogRepository blogRepo) {

        //    var postQuery = new PostQuery()
        //    {
        //        CategoryId = id,
        //    };

        //    var postsList = await blogRepo.GetPagedPostsAsync(
        //        postQuery, pagingModel,
        //        posts => posts.ProjectToType<PostDto>());

        //    var paginationResult = new PaginationResult<PostDto>(postsList);

        //    return Results.Ok(paginationResult);
        //}

        private static async Task<IResult> GetPostsByCategorySlug(
            [FromRoute] string slug,
            [AsParameters] PagingModel pagingModel,
            IBlogRepository blogRepo)
        {

            var postQuery = new PostQuery()
            {
                CategorySlug = slug
            };

            var postsList = await blogRepo.GetPagedPostsAsync(
                postQuery, pagingModel,
                posts => posts.ProjectToType<PostDto>());

            var paginationResult = new PaginationResult<PostDto>(postsList);

            return Results.Ok(ApiResponse.Success(paginationResult));
        }


        private static async Task<IResult> AddCategory(
            CategoryEditModel model,
            IBlogRepository blogRepo,
            IMapper mapper)
        {

            if (await blogRepo.IsCategorySlugExistedAsync(0, model.UrlSlug))
            {
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict,
                    $"Slug '{model.UrlSlug}' đã được sử dụng"));
            }

            var category = mapper.Map<Category>(model);
            await blogRepo.AddOrUpdateCategoryAsync(category);

            return Results.Ok(ApiResponse.Success(mapper.Map<CategoryItem>(category), HttpStatusCode.Created));
        }

        private static async Task<IResult> UpdateCategory(
            int id,
            CategoryEditModel model,
            IBlogRepository blogRepo,
            IMapper mapper)
        {

            if (await blogRepo.IsCategorySlugExistedAsync(id, model.UrlSlug))
            {
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict,
                    $"Slug '{model.UrlSlug}' đã được sử dụng"));
            }

            var category = mapper.Map<Category>(model);

            category.Id = id;

            return await blogRepo.AddOrUpdateCategoryAsync(category)
                    ? Results.Ok(ApiResponse.Success("Chủ đề đã được cập nhập", HttpStatusCode.NoContent))
                    : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Không tìm thấy chủ đề với id: `{id}`"));
        }

        private static async Task<IResult> DeleteCategory(int id,
            IBlogRepository blogRepo)
        {
            return await blogRepo.DeleteCategoryByIdAsync(id)
                    ? Results.Ok(ApiResponse.Success("Chủ đề đã được xóa", HttpStatusCode.NoContent))
                    : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Không tìm thấy chủ đề với id: `{id}`"));
        }

    }
}