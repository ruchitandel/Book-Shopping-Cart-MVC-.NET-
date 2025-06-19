using BookShoppingCartMvcUI.Models.DTOs;
using BookShoppingCartMvcUI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BookShoppingCartMVC.Models;
using BookShoppingCartMVC.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShoppingCartMVC.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartRepository(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor,
            UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> AddItem(int bookId, int qty)
        {
            string userId = GetUserId();
            using var transaction = _db.Database.BeginTransaction();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException("User is not logged-in");

                var cart = await GetCart(userId);
                if (cart == null)
                {
                    cart = new ShoppingCart { UserId = userId };
                    _db.ShoppingCarts.Add(cart);
                    await _db.SaveChangesAsync();
                }

                var cartItem = _db.CartDetails
                                  .FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.BookId == bookId);

                if (cartItem != null)
                {
                    cartItem.Quantity += qty;
                }
                else
                {
                    var book = _db.Books.Find(bookId);
                    cartItem = new CartDetail
                    {
                        BookId = bookId,
                        ShoppingCartId = cart.Id,
                        Quantity = qty,
                        UnitPrice = book.Price
                    };
                    _db.CartDetails.Add(cartItem);
                }

                await _db.SaveChangesAsync();
                transaction.Commit();
            }
            catch (Exception)
            {
                // Handle exception or log it
            }

            return await GetCartItemCount(userId);
        }

        public async Task<int> RemoveItem(int bookId)
        {
            string userId = GetUserId();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException("User is not logged-in");

                var cart = await GetCart(userId);
                if (cart == null)
                    throw new InvalidOperationException("Invalid cart");

                var cartItem = _db.CartDetails
                                  .FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.BookId == bookId);

                if (cartItem == null)
                    throw new InvalidOperationException("No items in cart");
                else if (cartItem.Quantity == 1)
                    _db.CartDetails.Remove(cartItem);
                else
                    cartItem.Quantity -= 1;

                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
                // Handle exception or log it
            }

            return await GetCartItemCount(userId);
        }

        // ✅ Updated to return single ShoppingCart
        public async Task<ShoppingCart> GetUserCart()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                throw new InvalidOperationException("Invalid user ID");

            var shoppingCart = await _db.ShoppingCarts
                .Include(a => a.CartDetails)
                    .ThenInclude(cd => cd.Book)
                        .ThenInclude(b => b.Stock)
                .Include(a => a.CartDetails)
                    .ThenInclude(cd => cd.Book)
                        .ThenInclude(b => b.Genre)
                .FirstOrDefaultAsync(a => a.UserId == userId);

            return shoppingCart;
        }

        public async Task<ShoppingCart> GetCart(string userId)
        {
            return await _db.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<int> GetCartItemCount(string userId = "")
        {
            if (string.IsNullOrEmpty(userId))
                userId = GetUserId();

            var data = await (from cart in _db.ShoppingCarts
                              join cartDetail in _db.CartDetails on cart.Id equals cartDetail.ShoppingCartId
                              where cart.UserId == userId
                              select cartDetail.Id).ToListAsync();

            return data.Count;
        }

        public async Task<bool> DoCheckout(CheckoutModel model)
        {
            using var transaction = _db.Database.BeginTransaction();
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException("User is not logged-in");

                var cart = await GetCart(userId);
                if (cart == null)
                    throw new InvalidOperationException("Invalid cart");

                var cartDetail = _db.CartDetails
                                    .Where(a => a.ShoppingCartId == cart.Id).ToList();

                if (cartDetail.Count == 0)
                    throw new InvalidOperationException("Cart is empty");

                var pendingRecord = _db.OrderStatuses.FirstOrDefault(s => s.StatusName == "Pending");
                if (pendingRecord == null)
                    throw new InvalidOperationException("Order status 'Pending' not found");

                var order = new Order
                {
                    UserId = userId,
                    CreateDate = DateTime.UtcNow,
                    Name = model.Name,
                    Email = model.Email,
                    MobileNumber = model.MobileNumber,
                    PaymentMethod = model.PaymentMethod,
                    Address = model.Address,
                    IsPaid = false,
                    OrderStatusId = pendingRecord.Id
                };

                _db.Orders.Add(order);
                await _db.SaveChangesAsync();

                foreach (var item in cartDetail)
                {
                    var orderDetail = new OrderDetail
                    {
                        BookId = item.BookId,
                        OrderId = order.Id,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    };
                    _db.OrderDetails.Add(orderDetail);

                    var stock = await _db.Stocks.FirstOrDefaultAsync(a => a.BookId == item.BookId);
                    if (stock == null)
                        throw new InvalidOperationException("Stock not found");

                    if (item.Quantity > stock.Quantity)
                        throw new InvalidOperationException($"Only {stock.Quantity} item(s) available in stock");

                    stock.Quantity -= item.Quantity;
                }

                _db.CartDetails.RemoveRange(cartDetail);
                await _db.SaveChangesAsync();
                transaction.Commit();

                return true;
            }   
            catch (Exception)
            {
                return false;
            }
        }

        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            return _userManager.GetUserId(principal);
        }
    }
}
