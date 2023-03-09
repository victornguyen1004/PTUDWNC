namespace TatBlog.Core.Contracts
{
    public interface IPagedList
    {
        // Tổng số trang
        int PageCount { get; }

        // Tổng số phần từ trả về từ câu truy vấn
        int TotalItemCount { get; }

        // Chỉ số trang hiện tại (bắt đầu từ 0)
        int PageIndex { get; }

        // Vị trí của trang hiện tại (bắt đầu từ 1)
        int PageNumber { get; }

        // Số lượng phần tử tối đa trên 1 trang
        int PageSize { get; }

        // Kiểm tra có trang trước hay không
        bool HasPreviousPage { get; }

        // Kiểm tra còn trang sau hay không
        bool HasNextPage { get; }

        // Kiểm tra xem có phải trang đầu tiên hay không
        bool IsFirstPage { get; }

        // Kiểm tra xem có phải trang cuối hay không
        bool IsLastPage { get; }

        // Thứ tự của phần tử đầu trang trong truy vấn (bắt đầu từ 1)
        int FirstItemIndex { get; }

        // Thứ tự của phần từ cuối trang trong truy vấn (bắt đầu từ 1)
        int LastItemIndex { get; }
    }

    public interface IPagedList<out T> : IPagedList, IEnumerable<T>
    {

        // Lấy phần từ tại vị trí index (bắt đầu từ 0)
        T this[int index] { get; }

        // Đếm số lượng phần từ chứa trong trang
        int Count { get; }
    }
}