using ManageDb.Models;
using ManageDb.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManageDb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ITNVEDCodeService<TNVEDCode> tNVEDCodeService;

        public ICollection<TNVEDCode> TNVEDCodes { get; set; }

        public IndexModel(ITNVEDCodeService<TNVEDCode> tNVEDCodeService)
        {
            this.tNVEDCodeService = tNVEDCodeService;
        }

        public async Task OnGet()
        {
            ViewData["Title"] = "Home page";
            ViewData["PageSize"] = 100;

            var countTask = tNVEDCodeService.GetCountAsync();
            var allRecordsTask = tNVEDCodeService.GetAllAsync(1, 100);

            ViewData["RecordsQuantity"] = await countTask;
            TNVEDCodes = await allRecordsTask;
        }

        public async Task OnGetMoreOnPageAsync(int pageSize)
        {
            ViewData["Title"] = "Home page";
            ViewData["PageSize"] = pageSize;

            var countTask = tNVEDCodeService.GetCountAsync();
            var allRecordsTask = tNVEDCodeService.GetAllAsync(1, pageSize);

            ViewData["RecordsQuantity"] = await countTask;
            TNVEDCodes = await allRecordsTask;
        }

        public async Task OnGetSearchResultsAsync(string searchCode)
        {
            ViewData["Title"] = "Home page";
            ViewData["PageSize"] = 100;

            var countTask = tNVEDCodeService.GetCountAsync();
            var foundRecordsTask = tNVEDCodeService.GetByCodeAsync(searchCode);

            ViewData["RecordsQuantity"] = await countTask;
            TNVEDCodes = await foundRecordsTask;
        }
    }
}