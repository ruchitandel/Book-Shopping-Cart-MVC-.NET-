using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using BookShoppingCartMvcUI.Models;

namespace BookShoppingCartMVC.Models
{
    [Table("Book")]
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string? BookName { get; set; }

        [Required]
        [MaxLength(40)]
        public string? AuthorName { get; set; }

        [Required]
        public double Price { get; set; }

        public string? Image { get; set; }

        [Required]
        public int GenreId { get; set; }

        public Genre? Genre { get; set; }

        public List<OrderDetail>? OrderDetail { get; set; }

        public List<CartDetail>? CartDetail { get; set; }

        // ✅ Add this to support 1-to-1 relation with Stock
        public Stock? Stock { get; set; }

        [NotMapped]
        public string? GenreName { get; set; }

        [NotMapped]
        public int Quantity { get; set; }
    }
}
