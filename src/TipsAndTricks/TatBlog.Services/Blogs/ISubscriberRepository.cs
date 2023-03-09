using TatBlog.Core.Contracts;
using TatBlog.Core.Entities;

namespace TatBlog.Services.Blogs
{
    public interface ISubscriberRepository
    {
        Task<bool> SubscribeAsync(
            string email,
            CancellationToken cancellationToken = default);

        Task<bool> UnsubscribeAsync(
            string email,
            string reason,
            CancellationToken cancellationToken = default);

        Task<bool> BlockSubscriberAsync(
            int id,
            string reason,
            string notes,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteSubscriberAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<Subscriber> GetSubscriberByIdAsync(
            int id,
            CancellationToken cancellationToken = default);


        Task<Subscriber> GetSubscriberByEmailAsync(
            string email,
            CancellationToken cancellationToken = default);

        Task<IPagedList<Subscriber>> SearchSubscribersAsync(
            IPagingParams pagingParams,
            string keyword,
            SubscribeStatus status,
            CancellationToken cancellationToken = default);
    }
}