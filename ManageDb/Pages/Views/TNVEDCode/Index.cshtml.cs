using ManageDb.Models;
using ManageDb.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManageDb.Pages.Views.TNVEDCode;

public class IndexModel(ITNVEDCodeService<TNVEDCodeModel> tNvedCodeService) : PageModel
{
    public IEnumerable<TNVEDCodeModel> TNVEDCode { get;set; } = [];

    public async Task OnGetAsync()
    {
        ViewData["Title"] = "Коды ТН ВЭД";
        ViewData["PageSize"] = 10;
        TNVEDCode = await tNvedCodeService.GetAllWithTechRegsAsync(1, 10);
    }

    public async Task OnGetMoreOnPageAsync(int pageSize)
    {
        ViewData["Title"] = "Коды ТН ВЭД";
        ViewData["PageSize"] = pageSize;
        TNVEDCode = await tNvedCodeService.GetAllWithTechRegsAsync(1, pageSize);
    }

    public async Task OnGetSearchResultsAsync(string searchCode)
    {
        ViewData["Title"] = "Коды ТН ВЭД";
        ViewData["PageSize"] = 100;
        TNVEDCode = await tNvedCodeService.GetByCodeWithTechRegsAsync(searchCode);
    }
}