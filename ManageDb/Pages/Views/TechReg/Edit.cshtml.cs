using ManageDb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManageDb.Pages.Views.TechReg
{
    public class EditModel : PageModel
    {
        private readonly ITechRegService<Models.TechReg> techRegService;

        public EditModel(ITechRegService<Models.TechReg> techRegService)
        {
            this.techRegService = techRegService;
        }

        [BindProperty]
        public ManageDb.Models.TechReg TechReg { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            ViewData["Title"] = "Edit";

            if (id == null || techRegService == null)
                return NotFound();

            TechReg = await techRegService.GetByIdAsync(id.Value);
            if (TechReg.Id == 0)
                return NotFound();

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            int result = await techRegService.UpdateAsync(TechReg);

            if (result == 0)
                return NotFound();

            return RedirectToPage("./Index");
        }
    }
}
