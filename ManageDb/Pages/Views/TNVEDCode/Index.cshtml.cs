using ManageDb.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManageDb.Pages.Views.TNVEDCode
{
    public class IndexModel : PageModel
    {
        private readonly ITNVEDCodeService<Models.TNVEDCode> tNVEDCodeService;

        public IndexModel(ITNVEDCodeService<Models.TNVEDCode> tNVEDCodeService)
        {
            this.tNVEDCodeService = tNVEDCodeService;
        }

        public IEnumerable<ManageDb.Models.TNVEDCode> TNVEDCode { get;set; } = default!;

        public async Task OnGetAsync()
        {
            ViewData["Title"] = "Коды ТН ВЭД";
            ViewData["PageSize"] = 10;

            if (tNVEDCodeService != null)
                TNVEDCode = await tNVEDCodeService.GetAllWithTechRegsAsync(1, 10);
        }

        public async Task OnGetMoreOnPageAsync(int pageSize)
        {
            ViewData["Title"] = "Коды ТН ВЭД";
            ViewData["PageSize"] = pageSize;

            if (tNVEDCodeService != null)
                TNVEDCode = await tNVEDCodeService.GetAllWithTechRegsAsync(1, pageSize);
        }
    }
}
