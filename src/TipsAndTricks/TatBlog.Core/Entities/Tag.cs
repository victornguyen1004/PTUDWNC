﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Contracts;

namespace TatBlog.Core.Entities
{
    public class Tag : IEntity
    {
        // Mã từ khóa 
            public int Id { get; set; }
            // Nội dung từ khóa 
            public string Name { get; set; }

            //Tên định danh để tạo URL
            public string UrlSlug { get; set; }

            // Mô tả thêm về từ khóa
            public string Description { get; set; }

            // Dánh ách bài viết có chứa từ khóa
            public IList<Post> Posts { get; set; }
        }
    }
