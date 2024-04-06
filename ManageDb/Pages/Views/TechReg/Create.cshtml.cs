using ManageDb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManageDb.Pages.Views.TechReg
{
    public class CreateModel : PageModel
    {
        private readonly ITechRegService<Models.TechReg> techRegService;

        public CreateModel(ITechRegService<Models.TechReg> techRegService)
        {
            this.techRegService = techRegService;
        }
        
        public IActionResult OnGet()
        {
            ViewData["Title"] = "Create";

            return Page();
        }

        [BindProperty]
        public ManageDb.Models.TechReg TechReg { get; set; } = default!;
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || TechReg == null)
                return Page();

            await techRegService.AddAsync(TechReg);

            return RedirectToPage("./Index");
        }
    }
}
