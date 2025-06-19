using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BookShoppingCartMVC.Models;
using BookShoppingCartMVC.Repositories;
using BookShoppingCartMvcUI.Models;

namespace BookShoppingCartMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeRepository _homeRepository;

        public HomeController(ILogger<HomeController> logger, IHomeRepository homeRepository)
        {
            _logger = logger;
            _homeRepository = homeRepository;
        }

        public async Task<IActionResult> Index(string sterm = "", int genreId = 0)
        {
            // Get books with optional search term and genre filter
            IEnumerable<Book> books = await _homeRepository.GetBooks(sterm, genreId);

            // Make sure Quantity is populated (assuming Book has Stock relationship)
            foreach (var book in books)
            {
                book.Quantity = book.Stock?.Quantity ?? 0; // This assumes Book has Stock navigation property
            }

            // Get all genres
            IEnumerable<Genre> genres = await _homeRepository.Genres();

            // Prepare the model for the view
            BookDisplayModel bookModel = new BookDisplayModel
            {
                Books = books,
                Genres = genres,
                STerm = sterm,
                GenreId = genreId
            };

            return View(bookModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
