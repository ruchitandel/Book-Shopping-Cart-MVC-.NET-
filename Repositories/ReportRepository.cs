using BookShoppingCartMVC.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MySqlConnector; // Use this for MySQL parameters
using BookShoppingCartMVC.Models; // Make sure this contains TopNSoldBookModel
using BookShoppingCartMvcUI.Models.DTOs; // or wherever TopNSoldBookModel is defined




namespace BookShoppingCartMvcUI.Repositories;

[Authorize(Roles = nameof(Roles.Admin))]
public class ReportRepository : IReportRepository
{
    private readonly ApplicationDbContext _context;

    public ReportRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TopNSoldBookModel>> GetTopNSellingBooksByDate(DateTime startDate, DateTime endDate)
    {
        var startDateParam = new MySqlParameter("@startDate", startDate);
        var endDateParam = new MySqlParameter("@endDate", endDate);

        var topSoldBooks = await _context.Set<TopNSoldBookModel>()
            .FromSqlRaw("CALL Usp_GetTopNSellingBooksByDate(@startDate,@endDate)", startDateParam, endDateParam)
            .ToListAsync();

        return topSoldBooks;
    }
}

public interface IReportRepository
{
    Task<IEnumerable<TopNSoldBookModel>> GetTopNSellingBooksByDate(DateTime startDate, DateTime endDate);
}
