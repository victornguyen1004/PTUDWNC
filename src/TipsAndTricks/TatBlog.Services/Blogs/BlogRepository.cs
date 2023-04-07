using Microsoft.EntityFrameworkCore;
using SlugGenerator;
using System.Linq.Dynamic.Core;
using TatBlog.Core.Collections;
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

        #region Post
        public async Task<IList<Post>> GetAllPostsAsync(CancellationToken cancellationToken)
        {
            return await _context.Set<Post>()
                .Include(c => c.Category)
                .Include(a => a.Author)
                .Include(t => t.Tags)
                .Include(m => m.Comments)
                .OrderBy(n => n.Title)
                .ToListAsync(cancellationToken);
        }

        public async Task<Post> GetPostAsync(int year, int month, int day, string slug, CancellationToken cancellationToken = default)
        {
            IQueryable<Post> postsQuery = _context.Set<Post>()
               .Include(x => x.Category)
               .Include(x => x.Author)
               .Include(x => x.Tags)
               .Include(x => x.Comments);

            if (year > 0)
            {
                postsQuery = postsQuery.Where(x => x.PostedDate.Year == year);
            }

            if (month > 0)
            {
                postsQuery = postsQuery.Where(x => x.PostedDate.Month == month);
            }

            if (day > 0)
            {
                postsQuery = postsQuery.Where(x => x.PostedDate.Day == day);
            }

            if (!string.IsNullOrWhiteSpace(slug))
            {
                postsQuery = postsQuery.Where(x => x.UrlSlug == slug);
            }

            return await postsQuery.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IList<Post>> GetPopularAriticlesAsync(int numPosts, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Post>()
                .Include(x => x.Author)
                .Include(x => x.Category)
                .OrderByDescending(p => p.ViewCount)
                .Take(numPosts)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsPostSlugExistedAsync(int postId, string slug, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Post>()
                .AnyAsync(x => x.Id != postId && x.UrlSlug == slug, cancellationToken);
        }

        public async Task IncreaseViewCountAsync(int postId, CancellationToken cancellationToken = default)
        {
            await _context.Set<Post>()
                .Where(x => x.Id == postId)
                .ExecuteUpdateAsync(p =>
                    p.SetProperty(x => x.ViewCount, x => x.ViewCount + 1), cancellationToken);
        }

        public async Task<bool> DeletePostByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Post>()
                .Where(c => c.Id == id)
                .ExecuteDeleteAsync(cancellationToken) > 0;
        }


        public async Task<IList<MonthlyPostsCountItem>> CountTotalPostFromMonthsAsync(int month, CancellationToken cancellationToken = default)
        {
            var result = await _context.Set<Post>()
               .GroupBy(s => new { s.PostedDate.Month, s.PostedDate.Year })
               .Select(p => new MonthlyPostsCountItem()
               {
                   Month = p.Key.Month,
                   Year = p.Key.Year,
                   PostsCount = p.Count(x => x.Published)
               })
               .OrderByDescending(s => s.Year)
               .ThenByDescending(s => s.Month)
               .Take(month)
               .ToListAsync(cancellationToken);
            return result;
        }

        public async Task<Post> GetPostByIdAsync(int id, bool includeDetail = false, CancellationToken cancellationToken = default)
        {
            if (!includeDetail) return await _context.Set<Post>().FindAsync(id, cancellationToken);
            return await _context.Set<Post>()
                .Include(c => c.Category)
                .Include(a => a.Author)
                .Include(t => t.Tags)
                .Include(m => m.Comments)
                .Where(p => p.Id.Equals(id))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Post> AddOrUpdatePostAsync(Post post, IEnumerable<string> tags, CancellationToken cancellationToken)
        {

            var postExists = await _context.Set<Post>().AnyAsync(p => p.Id == post.Id, cancellationToken);

            if (!postExists || post.Tags == null)
            {
                post.Tags = new List<Tag>();
            }
            else if (post.Tags == null || post.Tags.Count == 0)
            {
                await _context.Entry(post)
                    .Collection(s => s.Tags)
                    .LoadAsync(cancellationToken);
            }

            var validTags = tags.Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => new {
                    Name = x,
                    Slug = x.GenerateSlug()
                })
                .GroupBy(x => x.Slug)
                .ToDictionary(g => g.Key, g => g.First().Name);

            if (postExists)
            {
                var oldPost = await GetPostByIdAsync(post.Id, true, cancellationToken);
                var oldTags = oldPost.Tags.ToList();
                foreach (var postTag in oldTags)
                {
                    if (!validTags.ContainsKey(postTag.UrlSlug))
                    {
                        postTag.Posts.Remove(post);
                        post.Tags.Remove(postTag);
                    }
                }
            }

            foreach (var tagItem in validTags)
            {
                if (post.Tags.Any(x => string.Compare(x.UrlSlug, tagItem.Key, StringComparison.InvariantCultureIgnoreCase) == 0)) continue;

                var tag = await GetTagFromSlugAsync(tagItem.Key, cancellationToken) ?? new Tag()
                {
                    Name = tagItem.Value,
                    Description = tagItem.Value,
                    UrlSlug = tagItem.Key,
                    Posts = new List<Post>()
                };

                if (tag.Posts.All(p => p.Id != post.Id))
                {
                    tag.Posts.Add(post);
                }

                post.Tags.Add(tag);
            }

            if (postExists)
            {
                _context.Entry(post).State = EntityState.Modified;
            }
            else
            {
                _context.Posts.Add(post);
            }

            await _context.SaveChangesAsync(cancellationToken);
            return post;

        }

        public async Task TogglePublishedStatusAsync(int id, CancellationToken cancellationToken = default)
        {
            await _context.Set<Post>()
                .Where(x => x.Id == id)
                .ExecuteUpdateAsync(p => p.SetProperty(x => x.Published, x => !x.Published), cancellationToken);
        }

        public async Task<IList<Post>> GetRandomPostsAsync(int randomNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Post>()
                .Include(a => a.Author)
                .Include(c => c.Category)
                .OrderBy(c => Guid.NewGuid())
                .Take(randomNumber).ToListAsync(cancellationToken);
        }

        public async Task<IList<Post>> FindPostsFromPostQueryAsync(IPostQuery postQuery, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Post>()
                .Include(a => a.Author)
                .Include(c => c.Category)
                .Where(s => s.Author.Id == postQuery.AuthorId ||
                    s.Category.Id == postQuery.CategoryId ||
                    s.Category.UrlSlug == postQuery.CategorySlug ||
                    s.PostedDate.Day == postQuery.Day ||
                    s.PostedDate.Month == postQuery.Month ||
                    s.Tags.Any(t => t.Name.Contains(postQuery.TagName)))
                .ToListAsync(cancellationToken);
        }

        public async Task<int> CountPostsQueryAsync(IPostQuery postQuery, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Post>()
                .Include(a => a.Author)
                .Include(c => c.Category)
                .CountAsync(s => s.Author.Id == postQuery.AuthorId ||
                    s.Category.Id == postQuery.CategoryId ||
                    s.Category.UrlSlug == postQuery.CategorySlug ||
                    s.PostedDate.Day == postQuery.Day ||
                    s.PostedDate.Month == postQuery.Month ||
                    s.Tags.Any(t => t.Name.Contains(postQuery.TagName)), cancellationToken);
        }

        public async Task<IPagedList<Post>> PagingPostQueryAsync(IPagingParams pagingParams, IPostQuery postQuery, CancellationToken cancellationToken = default)
        {
            var postsQuery = _context.Set<Post>()
                .Include(a => a.Author)
                .Include(c => c.Category)
                .Where(s => s.Author.Id == postQuery.AuthorId ||
                    s.Category.Id == postQuery.CategoryId ||
                    s.Category.UrlSlug == postQuery.CategorySlug ||
                    s.PostedDate.Day == postQuery.Day ||
                    s.PostedDate.Month == postQuery.Month ||
                    s.Tags.Any(t => t.Name.Contains(postQuery.TagName)));

            return await postsQuery.ToPagedListAsync(pagingParams, cancellationToken);
        }


        // Filter Post

        private IQueryable<Post> FilterPosts(IPostQuery postQuery)
        {

            int keyNumber = 0;
            var keyword = !string.IsNullOrWhiteSpace(postQuery.Keyword) ? postQuery.Keyword.ToLower() : "";
            int.TryParse(postQuery.Keyword, out keyNumber);

            IQueryable<Post> posts = _context.Set<Post>()
                .Include(a => a.Author)
                .Include(c => c.Category)
                .Include(t => t.Tags)
                .WhereIf(postQuery.Published, s => s.Published)
                .WhereIf(postQuery.NonPublished, s => !s.Published)
                .WhereIf(postQuery.CategoryId > 0, p => p.CategoryId == postQuery.CategoryId)
                .WhereIf(!string.IsNullOrWhiteSpace(postQuery.CategorySlug), p => p.Category.UrlSlug == postQuery.CategorySlug)
                .WhereIf(postQuery.AuthorId > 0, p => p.AuthorId == postQuery.AuthorId)
                .WhereIf(!string.IsNullOrWhiteSpace(postQuery.AuthorSlug), p => p.Author.UrlSlug == postQuery.AuthorSlug)
                .WhereIf(!string.IsNullOrWhiteSpace(postQuery.TagSlug), p => p.Tags.Any(t => t.UrlSlug == postQuery.TagSlug))
                .WhereIf(!string.IsNullOrWhiteSpace(postQuery.PostSlug), p => p.UrlSlug == postQuery.PostSlug)
                .WhereIf(postQuery.Year > 0, p => p.PostedDate.Year == postQuery.Year)
                .WhereIf(postQuery.Month > 0, p => p.PostedDate.Month == postQuery.Month)
                .WhereIf(postQuery.Day > 0, p => p.PostedDate.Day == postQuery.Day)
                .WhereIf(!string.IsNullOrWhiteSpace(postQuery.Keyword), q =>
                    //q.Category.UrlSlug.ToLower().Contains(keyword) ||
                    //q.Author.UrlSlug.ToLower().Contains(keyword) ||
                    //q.Author.FullName.ToLower().Contains(keyword) ||
                    q.Meta.ToLower().Contains(keyword) ||
                    q.Title.ToLower().Contains(keyword) ||
                    q.Description.ToLower().Contains(keyword) ||
                    q.ShortDescription.ToLower().Contains(keyword) ||
                    q.PostedDate.Day == keyNumber ||
                    q.PostedDate.Month == keyNumber ||
                    q.PostedDate.Year == keyNumber ||
                    q.Tags.Any(t => t.UrlSlug.ToLower().Contains(keyword) || t.Name.ToLower().Contains(keyword)));

            return posts;

        }

        public async Task<IPagedList<Post>> GetPagedPostsQueryAsync(
            IPostQuery postQuery,
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            return await FilterPosts(postQuery).ToPagedListAsync(
                pageNumber, pageSize,
                nameof(Post.PostedDate), "DESC",
                cancellationToken);
        }


        public async Task<IPagedList<T>> GetPagedPostsAsync<T>(PostQuery postQuery, IPagingParams pagingParam, Func<IQueryable<Post>, IQueryable<T>> mapper)
        {
            var posts = FilterPosts(postQuery);
            var projectedPosts = mapper(posts);

            return await projectedPosts.ToPagedListAsync(pagingParam);
        }

        public async Task<bool> SetImageUrlAsync(int postId, string imageUrl, CancellationToken cancellationToken = default)
        {
            return await _context.Posts
                .Where(p => p.Id == postId)
                .ExecuteUpdateAsync(p => p.SetProperty(i => i.ImageUrl, i => imageUrl),
                cancellationToken) > 0;
        }

        #endregion














        #region Category

        public async Task<IList<CategoryItem>> GetCategoriesAsync(bool showOnMenu = false, CancellationToken cancellationToken = default)
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

        public async Task<Category> GetCategoryBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Category>()
                .Where(c => c.UrlSlug.Equals(slug))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Category> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default)
        {

            return await _context.Set<Category>()
                .Where(c => c.Id.Equals(id))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> AddOrUpdateCategoryAsync(Category category, CancellationToken cancellationToken = default)
        {

            if (_context.Set<Category>().Any(c => c.Id == category.Id))
            {
                _context.Entry(category).State = EntityState.Modified;
            }
            else
            {
                _context.Categories.Add(category);
            }

            return await _context.SaveChangesAsync(cancellationToken) > 0;

        }

        public async Task<bool> DeleteCategoryByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Category>()
                .Where(c => c.Id == id)
                .ExecuteDeleteAsync(cancellationToken) > 0;
        }


        public async Task<bool> IsCategorySlugExistedAsync(int id, string slug, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Category>().AnyAsync(c => c.Id != id && c.UrlSlug.Equals(slug), cancellationToken);
        }
        public async Task<bool> IsTagSlugExistedAsync(int id, string slug, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Tag>().AnyAsync(t => t.Id != id && t.UrlSlug.Equals(slug), cancellationToken);
        }

        private IQueryable<CategoryItem> CategoriesFilter(ICategoryQuery cateQuery)
        {
            var categories = _context.Set<Category>()
                .WhereIf(!string.IsNullOrWhiteSpace(cateQuery.Keyword), s =>
                    s.Name.Contains(cateQuery.Keyword) ||
                    s.UrlSlug.Contains(cateQuery.Keyword) ||
                    s.Description.Contains(cateQuery.Keyword))
                .Select(s => new CategoryItem()
                {
                    Id = s.Id,
                    Name = s.Name,
                    UrlSlug = s.UrlSlug,
                    PostCount = s.Posts.Count(p => p.Published)
                });
            return categories;
        }

        public async Task<IPagedList<CategoryItem>> GetPagedCategoryAsync(ICategoryQuery categoryQuery, IPagingParams pagingParams, CancellationToken cancellationToken = default)
        {
            return await CategoriesFilter(categoryQuery).ToPagedListAsync(pagingParams, cancellationToken);
        }

        public async Task<IList<Category>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<Category>()
                .ToListAsync(cancellationToken);
        }

        public async Task<IPagedList<CategoryItem>> GetPagedCategoriesAsync(IPagingParams pagingParams, string name = null, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Category>()
            .AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(name),
                x => x.Name.Contains(name))
            .Select(a => new CategoryItem()
            {
                Id = a.Id,
                Name = a.Name,
                UrlSlug = a.UrlSlug,
                PostCount = a.Posts.Count(p => p.Published)
            })
            .ToPagedListAsync(pagingParams, cancellationToken);
        }

        public async Task<IPagedList<T>> GetPagedCategoriesAsync<T>(Func<IQueryable<Category>, IQueryable<T>> mapper, IPagingParams pagingParams, string name = null, CancellationToken cancellationToken = default)
        {
            var cateQuery = _context.Set<Category>().AsNoTracking();

            if (!string.IsNullOrEmpty(name))
            {
                cateQuery = cateQuery.Where(x => x.Name.Contains(name));
            }

            return await mapper(cateQuery)
                .ToPagedListAsync(pagingParams, cancellationToken);
        }


        #endregion


        #region Tag

        public async Task<IPagedList<TagItem>> GetPagedTagsQueryAsync(ITagQuery tagQuery, IPagingParams pagingParams, CancellationToken cancellationToken = default)
        {
            var tag = _context.Set<Tag>()
                .WhereIf(!string.IsNullOrWhiteSpace(tagQuery.Keyword), t => t.Name.ToLower().Contains(tagQuery.Keyword) ||
                                                                       t.UrlSlug.ToLower().Contains(tagQuery.Keyword) ||
                                                                       t.Description.ToLower().Contains(tagQuery.Keyword))
                .Select(x => new TagItem()
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlSlug = x.UrlSlug,
                    Description = x.Description,
                    PostCount = x.Posts.Count(p => p.Published)
                });

            return await tag
                .ToPagedListAsync(pagingParams, cancellationToken);
        }

        public async Task<Tag> GetTagFromSlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            IQueryable<Tag> tagQuery = _context.Set<Tag>()
                .Include(p => p.Posts);
            if (!string.IsNullOrWhiteSpace(slug))
            {
                tagQuery = tagQuery.Where(x => x.UrlSlug.Equals(slug));
            }
            return await tagQuery.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IList<TagItem>> GetTagsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<Tag>()
                .OrderBy(x => x.Name)
                .Select(x => new TagItem()
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlSlug = x.UrlSlug,
                    Description = x.Description,
                    PostCount = x.Posts.Count(p => p.Published),
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> DeleteTagByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Tag>()
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync(cancellationToken) > 0;
        }

        public async Task<Tag> AddOrUpdateTagAsync(Tag tag, CancellationToken cancellationToken = default)
        {
            if (_context.Set<Tag>().Any(c => c.Id == tag.Id))
            {
                _context.Entry(tag).State = EntityState.Modified;
            }
            else
            {
                _context.Tags.Add(tag);
            }

            await _context.SaveChangesAsync(cancellationToken);
            return tag;
        }

        public async Task<Tag> GetTagByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Tag>().Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IPagedList<TagItem>> GetPagedTagsAsync(IPagingParams pagingParams, string keyword, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Tag>()
            .WhereIf(!string.IsNullOrWhiteSpace(keyword), t =>
                t.Name.ToLower().Contains(keyword.ToLower()) ||
                t.UrlSlug.ToLower().Contains(keyword.ToLower()) ||
                t.Description.ToLower().Contains(keyword.ToLower()))
            .Select(x => new TagItem()
            {
                Id = x.Id,
                Name = x.Name,
                UrlSlug = x.UrlSlug,
                Description = x.Description,
                PostCount = x.Posts.Count(p => p.Published)
            })
            .ToPagedListAsync(pagingParams, cancellationToken);
        }

        #endregion

    }
}