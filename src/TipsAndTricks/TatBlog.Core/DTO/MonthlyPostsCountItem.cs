using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TatBlog.Core.DTO
{
    public class MonthlyPostsCountItem
    {

        public int Year { get; set; }
        public int Month { get; set; }
        public int PostsCount { get; set; }

    }
}