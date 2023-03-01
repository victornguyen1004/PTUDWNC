using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Contracts;

namespace TatBlog.Core.Entities
{
    public class Post : IEntity
    {
        // Mã bài viết
        public int Id { get; set; }

        // Tiêu đề bài viết
        public string Title { get; set; }

        // Mô tả hay giới thiệu ngắn về nội dung
        public string ShortDescription { get; set; }

        // Nội dung chi tiết của bài viết
        public string Description { get; set; }

        // Metadata
        public string Meta { get; set; }

        //Tên định danh để tạo URL
        public string UrlSlug { get; set; }

        // ĐƯờng dẫn đến tập tin hình ảnh
        public string ImageUrl { get; set; }

        // Số lượt xem, đọc bài viết
        public int ViewCount { get; set; }

        // Trạng thái của bài viết
        public bool Published { get; set; }

        // Ngày giờ đăng bài
        public DateTime PostDate { get; set; }

        // Ngày giờ cập nhạt lần cuối
        public DateTime? ModifedDate { get; set; }

        // Mã chuyên mục
        public int CategoryID { get; set; }

        // Mã tác giả của bài viết
        public int AuthorID { get; set; }

        // Chuyên mục của bài viết
        public Category Category { get; set; }

        // Tác giả của bài viết
        public Author Author { get; set; }

        // Danh sách ác từ khóa của bài viết
        public IList<Tag> Tags { get; set; }
    }
}
