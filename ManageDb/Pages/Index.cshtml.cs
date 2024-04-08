using ManageDb.Models;
using ManageDb.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManageDb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> logger;
        private readonly ITNVEDCodeService<TNVEDCode> tNVEDCodeService;

        public ICollection<TNVEDCode> TNVEDCodes { get; set; }

        public IndexModel(ILogger<IndexModel> logger, ITNVEDCodeService<TNVEDCode> tNVEDCodeService)
        {
            this.logger = logger!;
            this.tNVEDCodeService = tNVEDCodeService!;
        }

        public async Task OnGet()
        {
            ViewData["Title"] = "Home page";

            TNVEDCodes = await tNVEDCodeService.GetAllAsync();
        }
    }
}