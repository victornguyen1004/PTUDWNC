﻿namespace TatBlog.Core.Contracts
{
    public interface IPostQuery
    {
        public int AuthorId { get; set; }
        public int CategoryId { get; set; }
        public string CategorySlug { get; set; }
        public string AuthorSlug { get; set; }
        public string PostSlug { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public bool Published { get; set; }
        public string TagSlug { get; set; }
        public string TagName { get; set; }
        public string Keyword { get; set; }
    }
}