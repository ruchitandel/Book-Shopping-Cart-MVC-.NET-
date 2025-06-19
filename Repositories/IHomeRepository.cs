using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookShoppingCartMvcUI.Models
{
    public interface IHomeRepository
    {
        Task<IEnumerable<Book>> GetBooks(string sTerm = "", int genreId = 0);
        Task<IEnumerable<Genre>> Genres();
    }
}


