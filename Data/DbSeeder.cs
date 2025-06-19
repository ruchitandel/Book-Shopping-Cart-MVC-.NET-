using BookShoppingCartMVC.Models;
using BookShoppingCartMvcUI.Models; // ✅ Needed for Book, Stock
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShoppingCartMVC.Data
{
    public class DbSeeder
    {
        public static async Task SeedDefaultData(IServiceProvider service)
        {
            // ===== SEED ROLES & ADMIN =====
            var userMgr = service.GetRequiredService<UserManager<IdentityUser>>();
            var roleMgr = service.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleMgr.RoleExistsAsync(Constants.Roles.Admin.ToString()))
                await roleMgr.CreateAsync(new IdentityRole(Constants.Roles.Admin.ToString()));

            if (!await roleMgr.RoleExistsAsync(Constants.Roles.User.ToString()))
                await roleMgr.CreateAsync(new IdentityRole(Constants.Roles.User.ToString()));

            var adminEmail = "admin@gmail.com";
            var admin = await userMgr.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userMgr.CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                    await userMgr.AddToRoleAsync(admin, Constants.Roles.Admin.ToString());
                else
                    throw new Exception("Failed to create admin user: " + string.Join(", ", result.Errors));
            }

            // ===== SEED BOOKS & STOCKS =====
            var db = service.GetRequiredService<ApplicationDbContext>();
            db.Database.Migrate(); // Make sure DB is up to date

            if (!db.Genres.Any())
            {
                db.Genres.AddRange(new List<Genre>
                {
                    new Genre { GenreName = "Fantasy" },
                    new Genre { GenreName = "Adventure" },
                    new Genre { GenreName = "Science Fiction" }
                });
                await db.SaveChangesAsync();
            }

            if (!db.Books.Any())
            {
                var books = new List<Book>
                {
                    new Book { BookName = "The Hobbit", AuthorName = "J.R.R. Tolkien", GenreId = 1, Price = 399 },
                    new Book { BookName = "Treasure Island", AuthorName = "R.L. Stevenson", GenreId = 2, Price = 249 },
                    new Book { BookName = "Dune", AuthorName = "Frank Herbert", GenreId = 3, Price = 499 }
                };
                db.Books.AddRange(books);
                await db.SaveChangesAsync();
            }

            if (!db.Stocks.Any())
            {
                var books = db.Books.ToList();
                var stockList = books.Select(b => new Stock
                {
                    BookId = b.Id,
                    Quantity = 10 // ✅ Set stock to a non-zero number
                }).ToList();

                db.Stocks.AddRange(stockList);
                await db.SaveChangesAsync();
            }
        }
    }
}
