using ManageDb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ManageDb.Pages.Views.TNVEDCode
{
    public class DetailsModel : PageModel
    {
        private readonly ITNVEDCodeService<Models.TNVEDCode> tNVEDCodeService;

        public DetailsModel(ITNVEDCodeService<Models.TNVEDCode> tNVEDCodeService)
        {
            this.tNVEDCodeService = tNVEDCodeService;
        }

        public ManageDb.Models.TNVEDCode TNVEDCode { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            ViewData["Title"] = "Details";

            if (id == null || tNVEDCodeService == null)
                return NotFound();

            TNVEDCode = await tNVEDCodeService.GetByIdAsync(id.Value);
            if (TNVEDCode.Id == 0)
                return NotFound();

            return Page();
        }
    }
}
