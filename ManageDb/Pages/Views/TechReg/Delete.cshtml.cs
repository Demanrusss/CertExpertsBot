using ManageDb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ManageDb.Pages.Views.TechReg
{
    public class DeleteModel : PageModel
    {
        private readonly ITechRegService<Models.TechReg> techRegService;

        public DeleteModel(ITechRegService<Models.TechReg> techRegService)
        {
            this.techRegService = techRegService;
        }

        [BindProperty]
        public ManageDb.Models.TechReg TechReg { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            ViewData["Title"] = "Delete";

            if (id == null || techRegService == null)
                return NotFound();

            TechReg = await techRegService.GetByIdAsync(id.Value);

            if (TechReg.Id == 0)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || techRegService == null)
                return NotFound();
            
            await techRegService.DeleteAsync(id.Value);

            return RedirectToPage("./Index");
        }
    }
}
