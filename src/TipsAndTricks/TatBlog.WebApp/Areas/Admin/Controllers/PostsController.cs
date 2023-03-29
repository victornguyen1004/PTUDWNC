using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TatBlog.Core.Collections;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.WebApp.Media;
using TatBlog.WebApp.Areas.Admin.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using TatBlog.WebApp.Validations;
using TatBlog.Core.Contracts;

namespace TatBlog.WebApp.Areas.Admin.Controllers
{
    public class PostsController : Controller
    {

        private readonly ILogger<PostsController> _logger;
        private readonly IBlogRepository _blogRepo;
        private readonly IMediaManager _mediaManager;
        private readonly IMapper _mapper;
        private readonly IValidator<PostEditModel> _postValidator;

        public PostsController(ILogger<PostsController> logger, IBlogRepository blogRepository, IMediaManager mediaManager, IMapper mapper)
        {
            _logger = logger;
            _blogRepo = blogRepository;
            _mediaManager = mediaManager;
            _mapper = mapper;
            _postValidator = new PostValidator(_blogRepo);
        }


        public async Task<IActionResult> Index(PostFilterModel model,
            PagingParams pageParam,
            [FromQuery(Name = "p")] int pageNumber = 1,
            [FromQuery(Name = "ps")] int pageSize = 10)
        {
            _logger.LogInformation("Tạo điều kiện truy vấn");

            // Use Mapster to create object PostQuery from object PostFilterModel model

            var postQuery = _mapper.Map<PostQuery>(model);

            _logger.LogInformation("Lấy danh sách bài viết từ CSDL");

            if (pageParam.PageSize != 0 || pageParam.PageNumber != 0)
            {
                pageNumber = pageParam.PageNumber;
                pageSize = pageParam.PageSize;
            }

            ViewBag.PostsList = await _blogRepo
                .GetPagedPostsAsync(postQuery, pageNumber, pageSize);

            _logger.LogInformation("Chuẩn bị dữ liệu cho ViewModel");

            await PopulatePostFilterModelAsync(model);

            return View(model);
        }

        private async Task PopulatePostFilterModelAsync(PostFilterModel model)
        {


            var authors = await _blogRepo.GetAllAuthorsAsync();
            var categories = await _blogRepo.GetCategoriesAsync();

            model.AuthorList = authors.Select(a => new SelectListItem()
            {
                Text = a.FullName,
                Value = a.Id.ToString()
            });

            model.CategoryList = categories.Select(c => new SelectListItem()
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });
        }

        //public IActionResult Index() {
        //    return View();
        //}

        private async Task PopulatePostEditModelAsync(PostEditModel model)
        {
            var authors = await _blogRepo.GetAllAuthorsAsync();
            var categories = await _blogRepo.GetCategoriesAsync();

            model.AuthorList = authors.Select(a => new SelectListItem()
            {
                Text = a.FullName,
                Value = a.Id.ToString()
            });

            model.CategoryList = categories.Select(c => new SelectListItem()
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id = 0)
        {

            // Id = 0 -> Add post
            // Id > 0 -> Read post data from DB
            var post = id > 0 ? await _blogRepo.GetPostByIdAsync(id, true) : null;

            // Create View model from DB data
            var model = post == null ? new PostEditModel() : _mapper.Map<PostEditModel>(post);

            // Update data for View model
            await PopulatePostEditModelAsync(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PostEditModel model)
        {

            var validationResult = await this._postValidator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
            }

            if (!ModelState.IsValid)
            {
                await PopulatePostEditModelAsync(model);
                return View(model);
            }
            var post = model.Id > 0 ? await _blogRepo.GetPostByIdAsync(model.Id) : null;

            if (post == null)
            {
                post = _mapper.Map<Post>(model);

                post.Id = 0;
                post.PostedDate = DateTime.Now;
            }
            else
            {
                _mapper.Map(model, post);

                post.Category = null;
                post.ModifiedDate = DateTime.Now;
            }

            // Nếu người dùng có update hình ảnh minh họa cho bài viết
            if (model.ImageFile?.Length > 0)
            {
                // Thực hiện việc lưu tập tin vào thư mục uploads
                var newImagePath = await _mediaManager.SaveFileAsync(
                    model.ImageFile.OpenReadStream(),
                    model.ImageFile.FileName,
                    model.ImageFile.ContentType);


                // Nếu lưu thành công, xóa tập tin hình ảnh cũ (nếu có)
                if (!string.IsNullOrWhiteSpace(newImagePath))
                {
                    await _mediaManager.DeleteFileAsync(post.ImageUrl);
                    post.ImageUrl = newImagePath;
                }
            }

            await _blogRepo.AddOrUpdatePostAsync(post, model.GetSelectedTags());

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> VerifyPostSlug(
            int id,
            string urlSlug)
        {
            var slugExisted = await _blogRepo
                .IsPostSlugExistedAsync(id, urlSlug);

            return slugExisted ? Json($"Slug '{urlSlug}' đã được sử dụng") : Json(true);
        }


        public async Task<IActionResult> TogglePublished(int id)
        {
            await _blogRepo.TogglePublishedStatusAsync(id);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeletePost(int id)
        {
            await _blogRepo.DeletePostByIdAsync(id);
            return RedirectToAction(nameof(Index));
        }


    }
}