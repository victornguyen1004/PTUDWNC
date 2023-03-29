using FluentValidation;
using FluentValidation.AspNetCore;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using TatBlog.Core.Collections;
using TatBlog.Core.Contracts;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.WebApp.Areas.Admin.Models;
using TatBlog.WebApp.Validations;

namespace TatBlog.WebApp.Areas.Admin.Controllers
{
    public class TagsController : Controller
    {

        private readonly IBlogRepository _blogRepo;
        private readonly IMapper _mapper;
        private readonly IValidator<TagEditModel> _tagValidator;

        public TagsController(IBlogRepository blogRepo, IMapper mapper)
        {
            _blogRepo = blogRepo;
            _mapper = mapper;
            _tagValidator = new TagValidator(_blogRepo);
        }

        public async Task<IActionResult> Index(TagFilterModel model,
            PagingParams pageParam,
            [FromQuery(Name = "p")] int pageNumber = 1,
            [FromQuery(Name = "ps")] int pageSize = 5)
        {

            IPagingParams paging = new PagingParams()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortOrder = "DESC",
                SortColumn = "PostCount"
            };

            if (pageParam.PageSize != 0 || pageParam.PageNumber != 0)
            {
                paging.PageNumber = pageParam.PageNumber;
                paging.PageSize = pageParam.PageSize;
            }

            var tagQuery = _mapper.Map<TagQuery>(model);

            ViewBag.TagsList = await _blogRepo.GetPagedTagsAsync(tagQuery, paging); ;

            return View(model);

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var tag = id > 0
                ? await _blogRepo.GetTagByIdAsync(id)
                : null;

            var model = tag == null
                ? new TagEditModel()
                : _mapper.Map<TagEditModel>(tag);

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(TagEditModel model)
        {
            var validationResult = await this._tagValidator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var tag = model.Id > 0
                ? await _blogRepo.GetTagByIdAsync(model.Id)
                : null;

            if (tag == null)
            {
                tag = _mapper.Map<Tag>(model);

                tag.Id = 0;
            }
            else
            {
                _mapper.Map(model, tag);
            }

            await _blogRepo.AddOrUpdateTagAsync(tag);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteTag(int id)
        {
            await _blogRepo.DeleteTagByIdAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}