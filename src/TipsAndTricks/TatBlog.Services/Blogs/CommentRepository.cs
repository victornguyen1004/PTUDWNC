using Microsoft.EntityFrameworkCore;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;

namespace TatBlog.Services.Blogs
{
    public class CommentRepository : ICommentRepository
    {

        private readonly BlogDbContext _context;

        public CommentRepository(BlogDbContext context)
        {
            _context = context;
        }

        public async Task<Comment> AddOrUpdateCommentAsync(Comment comment, CancellationToken cancellationToken = default)
        {
            if (_context.Set<Comment>().Any(c => c.Id == comment.Id))
            {
                _context.Entry(comment).State = EntityState.Modified;
            }
            else
            {
                _context.Set<Comment>().Add(comment);
            }

            await _context.SaveChangesAsync(cancellationToken);
            return comment;
        }

        public async Task<bool> DeleteCommentAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Comment>()
                .Where(c => c.Id == id)
                .ExecuteDeleteAsync(cancellationToken) > 0;
        }

        public async Task<Comment> VerifyCommentAsync(int id, CommentStatus status, CancellationToken cancellationToken = default)
        {
            var comment = _context.Set<Comment>().FirstOrDefault(c => c.Id == id);
            if (comment != null)
            {
                comment.commentStatus = status;
                if (status == CommentStatus.Violate)
                {
                    comment.Active = true;
                }

                _context.Entry(comment).State = EntityState.Modified;

                await _context.SaveChangesAsync(cancellationToken);
            }
            return comment;
        }
    }
}