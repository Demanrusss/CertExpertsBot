using ManageDb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ManageDb.Pages.Views.TechReg
{
    public class DetailsModel : PageModel
    {
        private readonly ITechRegService<Models.TechReg> techRegService;

        public DetailsModel(ITechRegService<Models.TechReg> techRegService)
        {
            this.techRegService = techRegService;
        }

        public ManageDb.Models.TechReg TechReg { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            ViewData["Title"] = "Details";

            if (id == null || techRegService == null)
                return NotFound();

            TechReg = await techRegService.GetByIdAsync(id.Value);
            
            if (TechReg.Id == 0)
                return NotFound();

            return Page();
        }
    }
}
