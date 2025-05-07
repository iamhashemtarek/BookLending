using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Domain.Specifications
{
    public class BookParameters
    {
        private const int MAX_PAGE_SIZE = 10;
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? ISBN { get; set; }
        public int PublishedYear { get; set; }
        public string? SortBy { get; set; }
        public bool IsSortAscending { get; set; } = true;
        public int PageNumber { get; set; } = 1;
        private int pageSize { get; set; } = 10;
        public int PageSize
        {
            get => pageSize;
            set => pageSize = (value > MAX_PAGE_SIZE) ? MAX_PAGE_SIZE : value;
        }
    }
}
