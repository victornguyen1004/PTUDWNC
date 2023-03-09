using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;

namespace TatBlog.Services.Blogs
{
    public interface IAuthorRepository
    {

        Task<Author> GetAuthorById(int id, CancellationToken cancellationToken = default);

        Task<Author> GetAuthorBySlug(string slug, CancellationToken cancellationToken = default);

        Task<IPagedList<AuthorItem>> GetPagedAuthorNumPosts(
            IPagingParams pagingParams,
            CancellationToken cancellationToken = default);

        Task<Author> AddOrUpdateAuthor(Author author, CancellationToken cancellationToken = default);

        Task<IList<Author>> GetAuthorsWithMostPost(int authorsQuantities, CancellationToken cancellationToken = default);

    }
}