﻿using System.ComponentModel;

namespace TatBlog.WebApp.Areas.Admin.Models
{
    public class CategoryEditModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? UrlSlug { get; set; }
        public string? Description { get; set; }
        public bool ShowOnMenu { get; set; }
    }
}