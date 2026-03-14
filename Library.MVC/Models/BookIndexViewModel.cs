using Library.Domain;
using Library.Domain.Models;

namespace Library.MVC.Models
{
    public class BookIndexViewModel
    {
        public List<Book> Books { get; set; } = new();

        public string SearchTerm { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string Availability { get; set; } = string.Empty;

        public List<string> Categories { get; set; } = new();
    }
}
