using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TatBlog.Core.Collections;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Controllers
{
    public class BlogController : Controller
    {
        private readonly IBlogRepository _blogRepository; 
        private readonly ICommentRepository _cmtRepository;
        private IConfiguration _configuration;

        public BlogController(IBlogRepository blogRepository, ICommentRepository commentRepository, IConfiguration configuration)
        {
            _blogRepository = blogRepository;
            _cmtRepository = commentRepository;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(
            [FromQuery(Name = "k")] string keyword = null,
            [FromQuery(Name = "p")] int pageNumber = 1,
            [FromQuery(Name = "ps")] int pageSize = 10)
        {
            var postQuery = new PostQuery()
            {
                Published = true,

                Keyword = keyword
            };

            var postLists = await _blogRepository
            .GetPagedPostsAsync(postQuery, pageNumber, pageSize);
           
            ViewBag.PostQuery = postQuery;
            
            return View(postLists);
        }
        public async Task<IActionResult> Category(
            string slug,
            [FromQuery(Name = "p")] int pageNumber = 1,
            [FromQuery(Name = "ps")] int pageSize = 3)
        {

            var postQuery = new PostQuery()
            {
                CategorySlug = slug,
                Published = true,
            };

            var postsList = await _blogRepository.GetPagedPostsAsync(postQuery, pageNumber, pageSize);

            ViewBag.PostQuery = postQuery;

            return View(postsList);
        }

        public async Task<IActionResult> Author(
            string slug,
            [FromQuery(Name = "p")] int pageNumber = 1,
            [FromQuery(Name = "ps")] int pageSize = 3)
        {

            var postQuery = new PostQuery()
            {
                AuthorSlug = slug,
                Published = true,
            };

            var postsList = await _blogRepository.GetPagedPostsAsync(postQuery, pageNumber, pageSize);

            ViewBag.PostQuery = postQuery;

            return View(postsList);
        }

        public async Task<IActionResult> Tag(
            string slug,
            [FromQuery(Name = "p")] int pageNumber = 1,
            [FromQuery(Name = "ps")] int pageSize = 3)
        {

            var postQuery = new PostQuery()
            {
                TagSlug = slug,
                Published = true,
            };

            var postsList = await _blogRepository.GetPagedPostsAsync(postQuery, pageNumber, pageSize);

            ViewBag.PostQuery = postQuery;

            return View(postsList);
        }

        public async Task<IActionResult> Post(
            int year,
            int month,
            int day,
            string slug)
        {

            var post = await _blogRepository.GetPostAsync(year, month, day, slug);
            await _blogRepository.IncreaseViewCountAsync(post.Id);
            var cmtsList = await _cmtRepository.GetCommentsByPostAsync(post.Id);


            ViewBag.Post = post;
            ViewData["Comments"] = cmtsList;

            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> Post(int postId, string userName, string content)
        {
            try
            {
                var newCmt = new Comment()
                {
                    PostId = postId,
                    Active = true,
                    commentStatus = CommentStatus.Violate,
                    Content = content,
                    UserName = userName,
                    CommentTime = DateTime.Now
                };

                var cmtSuccess = await _cmtRepository.AddOrUpdateCommentAsync(newCmt);
                var cmtList = await _cmtRepository.GetCommentsByPostAsync(postId);

                ViewData["Comments"] = cmtList;
                ViewBag.CmtSuccess = cmtSuccess;

                var post = await _blogRepository.GetPostByIdAsync(postId);
                return View(post);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Error", e.Message);
                return BadRequest(ModelState);
            }
        }


        public IActionResult About() => View();
        public IActionResult Contact() => View();
        public IActionResult RSS() => Content("Nội dung sẽ được cập nhật");
    }
}
