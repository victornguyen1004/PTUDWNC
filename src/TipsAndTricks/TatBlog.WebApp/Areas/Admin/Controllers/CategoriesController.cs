using FluentValidation;
using FluentValidation.AspNetCore;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using TatBlog.Core.Collections;
using TatBlog.Core.Contracts;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.WebApp.Areas.Admin.Models;
using TatBlog.WebApp.Media;
using TatBlog.WebApp.Validations;

namespace TatBlog.WebApp.Areas.Admin.Controllers
{
    public class AuthorsController : Controller
    {

        private readonly IAuthorRepository _authorRepo;
        private readonly IMediaManager _mediaManager;
        private readonly IMapper _mapper;
        private readonly IValidator<AuthorEditModel> _validator;

        public AuthorsController(IAuthorRepository authorRepo, IMediaManager mediaManager, IMapper mapper)
        {
            _authorRepo = authorRepo;
            _mediaManager = mediaManager;
            _mapper = mapper;
            _validator = new AuthorValidator(_authorRepo);
        }

        public async Task<IActionResult> Index(AuthorFilterModel model,
            PagingParams pageParam,
            [FromQuery(Name = "p")] int pageNumber = 1,
            [FromQuery(Name = "ps")] int pageSize = 5)
        {

            IPagingParams paging = new PagingParams()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortColumn = "FullName"
            };

            var authorQuery = _mapper.Map<AuthorQuery>(model);

            var authorPage = await _authorRepo.GetPagedAuthorPosts(authorQuery, paging);

            ViewBag.AuthorFilter = model;

            return View(authorPage);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var author = id > 0
                ? await _authorRepo.GetAuthorById(id)
                : null;

            var model = author == null
                ? new AuthorEditModel()
                : _mapper.Map<AuthorEditModel>(author);

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(AuthorEditModel model)
        {

            var validationResult = await this._validator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var author = model.Id > 0
                ? await _authorRepo.GetAuthorById(model.Id)
                : null;

            if (author == null)
            {
                author = _mapper.Map<Author>(model);

                author.Id = 0;
                author.JoinedDate = DateTime.Now;
            }
            else
            {
                _mapper.Map(model, author);
            }

            if (model.ImageFile?.Length > 0)
            {
                var newImagePath = await _mediaManager.SaveFileAsync(
                    model.ImageFile.OpenReadStream(),
                    model.ImageFile.FileName,
                    model.ImageFile.ContentType);


                if (!string.IsNullOrWhiteSpace(newImagePath))
                {
                    await _mediaManager.DeleteFileAsync(author.ImageUrl);
                    author.ImageUrl = newImagePath;
                }
            }

            await _authorRepo.AddOrUpdateAuthor(author);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteAuthor(int id)
        {
            await _authorRepo.DeleteAuthorAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}