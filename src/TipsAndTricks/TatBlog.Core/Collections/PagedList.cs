using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Contracts;

namespace TatBlog.Core.Collections
{
    public class PagedList<T> : IPagedList<T>
    {
        private readonly List<T> _subset = new();
        public PagedList(IList<T> list, int pageNumber, int pageSize, int totalCount)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalItemCount = totalCount;
            _subset.AddRange(list);
        }


        public int PageIndex { get; set; }
        public int TotalItemCount { get; set; }
        public int PageSize { get; set; }
        public int PageNumber
        {
            get => PageIndex + 1;
            set => PageIndex = value - 1;
        }
        public int PageCount
        {
            get
            {
                if (PageSize == 0) return 0;
                var total = TotalItemCount / PageSize;
                if (TotalItemCount % PageSize > 0) total++;
                return total;
            }
        }

        public int HasPreviousPage => throw new NotImplementedException();

        public int HasNextPage => throw new NotImplementedException();

        public bool IsFirstPage => throw new NotImplementedException();

        public bool IsLastPage => throw new NotImplementedException();

        public int FirstItemIndex => throw new NotImplementedException();

        public int LastItemIndex => throw new NotImplementedException();

        #region
        public IEnumerator<T> GetEnumerator()
        {
            return _subset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T this[int index] => _subset[index];
        public virtual int Count => _subset.Count;
        #endregion
    }
}
