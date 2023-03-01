using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;
using TatBlog.Services.Extensions;

namespace TatBlog.Services.Blogs
{
    public class BlogRepository : IBlogRepository
    {
        private readonly BlogDbContext _context;
        public BlogRepository(BlogDbContext context)
        {
            _context = context;
        }
        // Tìm bài viết có tên định danh là 'slug'
        // Và được đăng vào tháng month năm year
        public async Task<Post> GetPostAsync(int year, int month, string slug, CancellationToken cancellationToken = default)
        {
            IQueryable<Post> postQuery = _context.Set<Post>()
                 .Include(x => x.Category).Include(x => x.Author);
            if (year > 0)
            {
                postQuery = postQuery.Where(x => x.PostDate.Year == year);
            }
            if (month > 0)
            {
                postQuery = postQuery.Where(x => x.UrlSlug == slug);
            }
            return await postQuery.FirstOrDefaultAsync(cancellationToken);
        }

        // Tìm top N bài viết phổ biến được nhiều người xem nhất
        public async Task<IList<Post>> GetPopularArticleAsync(int numPosts, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Post>()
                .Include(x => x.Author)
                .Include(x => x.Category)
                .OrderByDescending(x => x.ViewCount)
                .Take(numPosts)
                .ToListAsync(cancellationToken);
        }

        // Kiểm tra xem tên định danh của bìa viết đã có hay chưa
        public async Task<bool> IsPostSlugExistAsync(int postId, string slug, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Post>()
                .AnyAsync(x => x.Id != postId && x.UrlSlug == slug, cancellationToken);
        }

        // Tăng số lượt xem cả một bài viết 
        public async Task IncreaseViewCountAsync(int postID, CancellationToken cancellationToken = default)
        {
            await _context.Set<Post>()
                .Where(x => x.Id == postID)
                .ExecuteUpdateAsync(p =>
                    p.SetProperty(x => x.ViewCount, x => x.ViewCount + 1), cancellationToken);
        }

        public async Task<IList<CategoryItem>> GetCategoryItemsAsync(bool showOnMenu = false, CancellationToken cancellationToken = default)
        {
            IQueryable<Category> categories = _context.Set<Category>();
            if (showOnMenu)
            {
                categories = categories.Where(x => x.ShowOnMenu);
            }
            return await categories
                .OrderBy(x => x.Name)
                .Select(x => new CategoryItem()
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlSlug = x.UrlSlug,
                    Description = x.Description,
                    ShowOnMenu = x.ShowOnMenu,
                    PostCount = x.Posts.Count(p => p.Published)
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<IPagedList<TagItem>> GetPagedTagsAsync(IPagingParams pagingParams, CancellationToken cancellationToken = default)
        {
            var tagQuery = _context.Set<Tag>()
                .Select(x => new TagItem()
                {
                    Id = x.Id,
                    Name = x.Name,

                    UrlSlug = x.UrlSlug,
                    Description = x.Description,
                    PostCount = x.Posts.Count(p => p.Published)
                });
            return await tagQuery.ToPagedListAsync(pagingParams, cancellationToken);
        }
    }
}
