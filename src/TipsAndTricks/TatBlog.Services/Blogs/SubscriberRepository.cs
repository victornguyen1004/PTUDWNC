using Microsoft.EntityFrameworkCore;
using System.Threading;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;
using TatBlog.Services.Extensions;

namespace TatBlog.Services.Blogs
{
    public class SubscriberRepository : ISubscriberRepository
    {

        private readonly BlogDbContext _context;

        public SubscriberRepository(BlogDbContext context)
        {
            _context = context;
        }
        public async Task<bool> SubscribeAsync(string email, CancellationToken cancellationToken = default)
        {
            var subEmail = _context.Set<Subscriber>().FirstOrDefault(s => s.Email == email);

            if (subEmail == null)
            {
                var newSub = new Subscriber()
                {
                    DateSubscribe = DateTime.Now,
                    Email = email,
                    subscribeStatus = SubscribeStatus.Subscribe,
                };

                _context.Set<Subscriber>().Add(newSub);
            }
            else
            {
                if (subEmail.subscribeStatus == SubscribeStatus.Block || subEmail.subscribeStatus == SubscribeStatus.Subscribe)
                    return false;
                subEmail.DateSubscribe = DateTime.Now;
                subEmail.DateUnsubscribe = null;
                subEmail.subscribeStatus = SubscribeStatus.Subscribe;
                subEmail.Reason = null;
                _context.Entry(subEmail).State = EntityState.Modified;
            }
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> UnsubscribeAsync(string email, string reason, CancellationToken cancellationToken = default)
        {
            var subEmail = _context.Set<Subscriber>().FirstOrDefault(s => s.Email == email);
            if (subEmail != null)
            {
                subEmail.DateUnsubscribe = DateTime.Now;
                subEmail.subscribeStatus = SubscribeStatus.Unsubscribe;
                subEmail.Reason = reason;
                _context.Entry(subEmail).State = EntityState.Modified;
                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
            return false;
        }

        public async Task<bool> BlockSubscriberAsync(int id, string reason, string notes, CancellationToken cancellationToken = default)
        {
            var subscriber = _context.Set<Subscriber>().FirstOrDefault(s => s.Id == id);
            if (subscriber != null)
            {
                subscriber.DateUnsubscribe = DateTime.Now;
                subscriber.subscribeStatus = SubscribeStatus.Block;
                subscriber.Reason = reason;
                subscriber.Note = notes;
                _context.Entry(subscriber).State = EntityState.Modified;
                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteSubscriberAsync(int id, CancellationToken cancellationToken = default)
        {
            var subscriber = _context.Set<Subscriber>().FirstOrDefault(s => s.Id == id);
            if (subscriber != null)
            {
                subscriber.DateUnsubscribe = DateTime.Now;
                subscriber.subscribeStatus = SubscribeStatus.Unsubscribe;
                _context.Entry(subscriber).State = EntityState.Modified;
                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
            return false;

        }

        public async Task<Subscriber> GetSubscriberByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Subscriber>().FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<Subscriber> GetSubscriberByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Subscriber>().FirstOrDefaultAsync(s => s.Email == email, cancellationToken);
        }

        public async Task<IPagedList<Subscriber>> SearchSubscribersAsync(IPagingParams pagingParams, string keyword, SubscribeStatus status, CancellationToken cancellationToken = default)
        {
            var subsQuery = _context.Set<Subscriber>()
                .Where(x => x.Email.Contains(keyword) ||
                x.Note.ToLower().Contains(keyword.ToLower()) ||
                x.Reason.ToLower().Contains(keyword.ToLower()) ||
                x.subscribeStatus == status);
            return await subsQuery.ToPagedListAsync(pagingParams, cancellationToken);

        }

    }
}