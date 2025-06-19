using BookShoppingCartMVC.Models;
using BookShoppingCartMvcUI.Models;
using BookShoppingCartMvcUI.Models.DTOs;
using System.Threading.Tasks;

namespace BookShoppingCartMVC.Repositories
{
    public interface ICartRepository
    {
        Task<int> AddItem(int bookId, int qty);
        Task<int> RemoveItem(int bookId);

        Task<ShoppingCart> GetUserCart(); // ✅ Changed from IEnumerable<ShoppingCart> to ShoppingCart

        Task<int> GetCartItemCount(string userId = "");

        Task<bool> DoCheckout(CheckoutModel model);

        Task<ShoppingCart> GetCart(string userId);
    }
}
