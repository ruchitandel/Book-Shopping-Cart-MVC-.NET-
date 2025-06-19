using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BookShoppingCartMVC.Models;
using BookShoppingCartMvcUI.Models;

namespace BookShoppingCartMVC.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDbContext _db;

        public HomeRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Genre>> Genres()
        {
            return await _db.Genres.ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetBooks(string sTerm = "", int genreId = 0)
        {
            sTerm = sTerm.ToLower();

            var booksQuery = _db.Books
                .Include(b => b.Stock)      // ✅ Load Stock table (to access Quantity)
                .Include(b => b.Genre)      // Optional: needed if you want GenreName
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(sTerm))
            {
                booksQuery = booksQuery.Where(b => b.BookName.ToLower().StartsWith(sTerm));
            }

            if (genreId > 0)
            {
                booksQuery = booksQuery.Where(b => b.GenreId == genreId);
            }

            return await booksQuery.ToListAsync();
        }
    }
}
