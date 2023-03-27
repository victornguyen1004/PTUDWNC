using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Contracts;

namespace TatBlog.Core.Entities
{
    public enum SubscribeStatus
    {
        Subscribe,
        Unsubscribe,
        Block
    }
    public class Subscriber : IEntity
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime DateSubscribe { get; set; }
        public DateTime? DateUnsubscribe { get; set; }
        public string? Reason { get; set; }
        public SubscribeStatus subscribeStatus { get; set; }
        public string Note { get; set; }
    }
}