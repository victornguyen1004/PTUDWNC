using TatBlog.Core.Collections;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;

namespace TatBlog.Services.Blogs
{
    public interface IBlogRepository
    {

        // Get all Posts
        Task<IList<Post>> GetAllPostsAsync(
            CancellationToken cancellationToken = default);

        // Tìm bài viết có tên định danh là 'slug' và được đăng vào tháng 'month' năm 'year'
        Task<Post> GetPostAsync(
            int year,
            int month,
            int day,
            string slug,
            CancellationToken cancellationToken = default);


        // Tìm Top N bài viết phổ biến nhất
        Task<IList<Post>> GetPopularAriticlesAsync(
            int numPosts,
            CancellationToken cancellationToken = default);

        // Kiểm tra xem tên định danh của bài viết đã có hay chưa
        Task<bool> IsPostSlugExistedAsync(
            int postId, string slug,
            CancellationToken cancellationToken = default);

        // Tăng số lượt xem của một bài viết
        Task IncreaseViewCountAsync(
            int postId,
            CancellationToken cancelslationToken = default);

        // Lấy danh sách chuyên mục và số lượng bài viết nằm thuộc từng chuyên mục/chủ đề
        Task<IList<CategoryItem>> GetCategoriesAsync(
            bool showOnMenu = false,
            CancellationToken cancellationToken = default);

        //Lấy danh sách từ khóa/thẻ và phân trang theo các tham số pagingParams
        Task<IPagedList<TagItem>> GetPagedTagsQueryAsync(
            ITagQuery tagQuery,
            IPagingParams pagingParams,
            CancellationToken cancellationToken = default);

        // Part C
        // Find tag by slug
        Task<Tag> GetTagFromSlugAsync(
            string slug,
            CancellationToken cancellationToken = default);

        // Get all Tags with number of posts belong to it.
        Task<IList<TagItem>> GetTagsAsync(
            CancellationToken cancellationToken = default);

        // Delete tag by Id.
        Task<bool> DeleteTagByIdAsync(int id,
            CancellationToken cancellationToken = default);

        // Find category by slug.
        Task<Category> GetCategoryBySlugAsync(string slug,
            CancellationToken cancellation = default);

        // Find category by Id.
        Task<Category> GetCategoryByIdAsync(int id,
            CancellationToken cancellationToken = default);
        Task<Tag> GetTagByIdAsync(int id,
            CancellationToken cancellationToken = default);

        // Add or Update a category.
        Task<bool> AddOrUpdateCategoryAsync(
            Category category,
            CancellationToken cancellationToken = default);
        Task<Tag> AddOrUpdateTagAsync(
            Tag tag,
            CancellationToken cancellationToken = default);

        // Delete category by Id
        Task<bool> DeleteCategoryByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<bool> DeletePostByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        // Check a category slug whether it exists or not
        Task<bool> IsCategorySlugExistedAsync(
            int id,
            string slug,
            CancellationToken cancellationToken = default);

        Task<bool> IsTagSlugExistedAsync(
            int id,
            string slug,
            CancellationToken cancellationToken = default);

        // Get and paging list of categories, return type IPageList<CategoryItem>
        Task<IPagedList<CategoryItem>> GetPagedCategoryAsync(
            ICategoryQuery categoryQuery,
            IPagingParams pagingParams,
            CancellationToken cancellationToken = default);

        // Count posts by N month. Input N. Output list objects with: Year, month, posts count. 
        Task<IList<MonthlyPostsCountItem>> CountTotalPostFromMonthsAsync(int month,
            CancellationToken cancellationToken = default);

        // Find post by Id
        Task<Post> GetPostByIdAsync(
            int id, bool includeDetail = false,
            CancellationToken cancellationToken = default);


        // Add/ Update a post
        Task<Post> AddOrUpdatePostAsync(
            Post post, IEnumerable<string> tags,
            CancellationToken cancellationToken = default);

        // Get all Categories
        Task<IList<Category>> GetAllCategoriesAsync(
            CancellationToken cancellationToken = default);

        // Toggle Published Status
        Task TogglePublishedStatusAsync(
            int id,
            CancellationToken cancellationToken = default);

        // Get random N posts
        Task<IList<Post>> GetRandomPostsAsync(
            int randomNumber,
            CancellationToken cancellationToken = default);

        // Find all Posts that match the PostQuery
        Task<IList<Post>> FindPostsFromPostQueryAsync(
            IPostQuery postQuery,
            CancellationToken cancellationToken = default);

        // Count posts match the PostQuery
        Task<int> CountPostsQueryAsync(
            IPostQuery postQuery,
            CancellationToken cancellationToken = default);

        // Paging Posts match PostQuery
        Task<IPagedList<Post>> PagingPostQueryAsync(
            IPagingParams pagingParams,
            IPostQuery postQuery,
            CancellationToken cancellationToken = default);

        Task<IPagedList<Post>> GetPagedPostsQueryAsync(
            IPostQuery postQuery,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<IPagedList<T>> GetPagedPostsAsync<T>(
            PostQuery postQuery,
            IPagingParams pagingParam,
            Func<IQueryable<Post>, IQueryable<T>> mapper);

        Task<IPagedList<CategoryItem>> GetPagedCategoriesAsync(
        IPagingParams pagingParams,
        string name = null,
        CancellationToken cancellationToken = default);

        Task<IPagedList<T>> GetPagedCategoriesAsync<T>(
            Func<IQueryable<Category>, IQueryable<T>> mapper,
            IPagingParams pagingParams,
            string keyword = null,
            CancellationToken cancellationToken = default);


        Task<IPagedList<TagItem>> GetPagedTagsAsync(
        IPagingParams pagingParams,
        string keyword,
        CancellationToken cancellationToken = default);

        Task<bool> SetImageUrlAsync(int postId, string imageUrl, CancellationToken cancellationToken = default);

    }
}