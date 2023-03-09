using Microsoft.EntityFrameworkCore;
using System.Threading;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;
using TatBlog.Services.Extensions;

namespace TatBlog.Services.Blogs
{
    public class AuthorRepository : IAuthorRepository
    {

        private readonly BlogDbContext _context;

        public AuthorRepository(BlogDbContext context)
        {
            _context = context;
        }

        public async Task<Author> AddOrUpdateAuthor(Author author, CancellationToken cancellationToken = default)
        {
            if (_context.Set<Author>().Any(a => a.Id == author.Id))
            {
                _context.Entry(author).State = EntityState.Modified;
            }
            else
            {
                _context.Set<Author>().Add(author);
            }

            await _context.SaveChangesAsync(cancellationToken);
            return author;
        }

        public async Task<Author> GetAuthorById(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Author>()
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }

        public async Task<Author> GetAuthorBySlug(string slug, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Author>()
                .FirstOrDefaultAsync(a => a.UrlSlug == slug, cancellationToken);
        }

        public async Task<IList<Author>> GetAuthorsWithMostPost(int authorsQuantities, CancellationToken cancellationToken = default)
        {
            var authors = _context.Set<Author>()
                .Select(a => new AuthorItem()
                {
                    PostCount = a.Posts.Count(p => p.Published)
                })
                .ToList();

            var maxPostCount = authors.Max(a => a.PostCount);

            return await _context.Set<Author>()
                .Where(a => a.Posts.Count(p => p.Published) == maxPostCount)
                .Take(authorsQuantities)
                .ToListAsync(cancellationToken);
        }

        public async Task<IPagedList<AuthorItem>> GetPagedAuthorNumPosts(IPagingParams pagingParams, CancellationToken cancellationToken = default)
        {
            var authorsQuery = _context.Set<Author>()
                .Select(a => new AuthorItem()
                {
                    Id = a.Id,
                    Email = a.Email,
                    FullName = a.FullName,
                    ImageUrl = a.ImageUrl,
                    JoinedDate = a.JoinedDate,
                    Notes = a.Notes,
                    PostCount = a.Posts.Count(p => p.Published)
                });
            return await authorsQuery.ToPagedListAsync(pagingParams, cancellationToken);
        }
    }
}