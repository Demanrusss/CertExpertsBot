using ManageDb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManageDb.Pages.Views.TNVEDCode
{
    public class DeleteModel : PageModel
    {
        private readonly ITNVEDCodeService<Models.TNVEDCode> tNVEDCodeService;

        public DeleteModel(ITNVEDCodeService<Models.TNVEDCode> tNVEDCodeService)
        {
            this.tNVEDCodeService = tNVEDCodeService;
        }

        [BindProperty]
        public ManageDb.Models.TNVEDCode TNVEDCode { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            ViewData["Title"] = "Delete";

            if (id == null || tNVEDCodeService == null)
                return NotFound();

            TNVEDCode = await tNVEDCodeService.GetByIdAsync(id.Value);
            if (TNVEDCode.Id == 0)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || tNVEDCodeService == null)
                return NotFound();

            int result = await tNVEDCodeService.DeleteAsync(id.Value);
            if (result == 0)
                return NotFound();
            
            return RedirectToPage("./Index");
        }
    }
}
