using Microsoft.AspNetCore.Mvc;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Components
{
    public class RandomPostsWidget : ViewComponent
    {
        private readonly IBlogRepository _blogRepository;

        public RandomPostsWidget(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int postsNum = 5;
            var posts = await _blogRepository.GetRandomPostsAsync(postsNum);
            return View(posts);
        }

    }
}
