using ManageDb.Models;
using ManageDb.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManageDb.Pages;

public class IndexModel(ITNVEDCodeService<TNVEDCodeModel> tNvedCodeService) : PageModel
{
    public ICollection<TNVEDCodeModel> TNVEDCodes { get; set; }

    public async Task OnGet()
    {
        ViewData["Title"] = "Главная";
        ViewData["PageSize"] = 100;
        ViewData["RecordsQuantity"] = await tNvedCodeService.GetCountAsync();
        TNVEDCodes = await tNvedCodeService.GetAllAsync(1, 100);
    }

    public async Task OnGetMoreOnPageAsync(int pageSize)
    {
        ViewData["Title"] = "Главная";
        ViewData["PageSize"] = pageSize;
        ViewData["RecordsQuantity"] = await tNvedCodeService.GetCountAsync();
        TNVEDCodes = await  tNvedCodeService.GetAllAsync(1, pageSize);
    }

    public async Task OnGetSearchResultsAsync(string searchCode)
    {
        ViewData["Title"] = "Главная";
        ViewData["PageSize"] = 100;
        ViewData["RecordsQuantity"] = await tNvedCodeService.GetCountAsync();
        TNVEDCodes = await tNvedCodeService.GetByCodeAsync(searchCode);
    }
}