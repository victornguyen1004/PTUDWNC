using Microsoft.AspNetCore.Mvc;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Components
{
    public class FeaturedPostsWidget : ViewComponent
    {
        private readonly IBlogRepository _blogRepository;

        public FeaturedPostsWidget(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int postsNum = 3;
            var posts = await _blogRepository.GetPopularAriticlesAsync(postsNum);
            return View(posts);
        }
    }
}