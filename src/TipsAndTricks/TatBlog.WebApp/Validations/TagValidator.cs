﻿using FluentValidation;
using TatBlog.Services.Blogs;
using TatBlog.WebApp.Areas.Admin.Models;

namespace TatBlog.WebApp.Validations
{
    public class TagValidator : AbstractValidator<TagEditModel>
    {

        private readonly IBlogRepository _blogRepo;

        public TagValidator(IBlogRepository blogRepository)
        {
            _blogRepo = blogRepository;

            RuleFor(s => s.Name)
          .NotEmpty().WithMessage("Tên chủ đề không được bỏ trống")
          .MaximumLength(500).WithMessage("Tên chủ đề không được nhiều hơn 500 ký tự");

            RuleFor(s => s.Description)
                .NotEmpty()
                .WithMessage("Giới thiệu không được bỏ trống");


            RuleFor(s => s.UrlSlug)
                .NotEmpty()
                .WithMessage("Slug không được bỏ trống")
                .MaximumLength(1000)
                .WithMessage("Slug không được nhiều hơn 512 ký tự");

            RuleFor(s => s.UrlSlug)
                .MustAsync(async (tagModel, slug, cancellationToken) =>
                    !await _blogRepo.IsTagSlugExistedAsync(tagModel.Id, slug, cancellationToken))
                .WithMessage("Slug '{PropertyValue}' đã được sử dụng");

        }



    }
}