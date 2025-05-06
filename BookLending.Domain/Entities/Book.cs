using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Domain.Entities
{
    public class Book : BaseEntity
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public int PublishedYear { get; set; }
        public string Description { get; set; }
        public string CoverImageUrl { get; set; }
        public bool IsAvailable { get; set; }
        public ICollection<Borrow> Borrows { get; set; }
    }
}
