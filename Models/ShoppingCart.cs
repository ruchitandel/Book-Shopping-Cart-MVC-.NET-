using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookShoppingCartMvcUI.Models
{
    [Table("ShoppingCart")]
    public class ShoppingCart
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Navigation property to cart details (e.g., items in cart)
        public ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();
    }
}
