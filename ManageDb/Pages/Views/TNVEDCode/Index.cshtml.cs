using ManageDb.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

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
            ViewData["Title"] = "Index";

            if (tNVEDCodeService != null)
                TNVEDCode = (await tNVEDCodeService.GetAllAsync()).OrderByDescending(c => c.Code).Take(10);
        }
    }
}
