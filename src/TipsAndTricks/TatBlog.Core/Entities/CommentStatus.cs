using System.ComponentModel.DataAnnotations.Schema;
using TatBlog.Core.Contracts;

namespace TatBlog.Core.Entities
{
    public enum CommentStatus
    {
        Violate,
        Valid,
    }

    public class Comment : IEntity
    {

        public int Id { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }
        public DateTime CommentTime { get; set; }
        public bool Active { get; set; }
        public CommentStatus commentStatus { get; set; }

        [ForeignKey("Post")]
        public int PostId { get; set; }
        public virtual Post post { get; set; }
    }
}