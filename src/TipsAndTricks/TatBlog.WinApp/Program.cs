using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;
using TatBlog.Data.Seeders;
using TatBlog.Services.Blogs;
using TatBlog.WinApp;

public class Program
{
    public static async Task Main(string[] args)
    {
        //var context = new BlogDbContext();
        //IBlogRepository blogRepository = new BlogRepository(context);
        //Category newCategory = new Category();
        //var result = blogRepository.AddOrUpdateCategoryAsync(newCategory);
        //Console.WriteLine(result);


        //string slug = "Hihi";
        //var resultCategory = await blogRepository.GetCategoryBySlug(slug);
        //if (resultCategory != null) { 
        //Console.WriteLine(resultCategory.Name);
        //}
        //else
        //{
        //    Console.WriteLine("Khong tim thay Category voi slug la {0}", slug);
        //}


        //bool result = await blogRepository.RemoveTagById(6);
        //if (result)
        //{
        //    Console.WriteLine("OK");
        //}
        //else
        //{
        //    Console.WriteLine("KO");
        //}

        //var pagingParams = new PagingParams
        //{
        //    PageNumber = 1,
        //    PageSize = 3,
        //    SortColumn = "name",
        //    SortOrder = "DESC"
        //};
        //var tagsList = await blogRepository.GetPagedTagsAsync(pagingParams);
        //Console.WriteLine("{0,-5}{1,-50}{2,10}",
        //    "ID", "Name", "Count");
        //foreach (var item in tagsList)
        //{
        //    Console.WriteLine("{0,-5}{1,-50}{2,10}",
        //        item.Id, item.Name, item.PostCount);
        //}

        //string slug = "reactjs";
        //Tag tagBySlug = await blogRepository.GetTagBySlug(slug);
        //Console.WriteLine(tagBySlug.Name);
        //Console.ReadKey();

        //var context = new BlogDbContext();
        //IBlogRepository blogRepository = new BlogRepository(context);
        //var categories=await blogRepository.GetCategoryItemsAsync();
        //Console.WriteLine("{0,-5}{1,-50}{2,10}", "ID", "Name", "Count");
        //foreach(var item in categories)
        //{
        //    Console.WriteLine("{0,-5}{1,-50}{2,10}",item.Id,item.Name,item.PostCount);
        //}
        //var posts = await blogRepository.GetPopularArticleAsync(3);
        //var posts=context.Posts
        //    .Where(p=>p.Published)
        //    .OrderBy(p=>p.Title)
        //    .Select(p=>new
        //    {
        //        Id=p.Id,
        //        Title=p.Title,
        //        ViewCount=p.ViewCount,
        //        PostDate=p.PostDate,
        //        Author=p.Author.FullName,
        //        Category=p.Category.Name

        //    })
        //    .ToList();
        //foreach (var post in posts)
        //{
        //    Console.WriteLine("ID               :{0}", post.Id);
        //    Console.WriteLine("Title            :{0}", post.Title);
        //    Console.WriteLine("View             :{0}", post.ViewCount);
        //    Console.WriteLine("Date             :{0:MM/dd/yyyy}", post.PostDate);
        //    Console.WriteLine("Author           :{0}", post.Author);
        //    Console.WriteLine("Categor          :{0}", post.Category);
        //    Console.WriteLine("".PadRight(80, '-'));


        //var seeder = new DataSeeder(context);

        // GỌi hàm Initialize để nhập dữ liệu mẫu
        //seeder.Initialize();

        //var authors=context.Authors.ToList();

        //Console.WriteLine("{0,-4}{1,-30}{2,-30}{3,12}",
        //    "ID", "Full Name", "Email", "Joined Date");

        //foreach(var author in authors)
        //{
        //    Console.WriteLine("{0,-4}{1,-30}{2,-30}{3,12:MM/dd/yyyy}",
        //        author.Id, author.FullName, author.Email, author.JoinedDate);
        //}

    }
}