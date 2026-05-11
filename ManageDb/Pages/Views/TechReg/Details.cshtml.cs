using ManageDb.Models;
using ManageDb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManageDb.Pages.Views.TechReg;

public class DetailsModel(ITechRegService<TechRegModel> techRegService) : PageModel
{
    public TechRegModel TechRegModel { get; set; } = default!; 

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        ViewData["Title"] = "Details";

        if (id == null)
            return NotFound();

        TechRegModel = await techRegService.GetByIdAsync(id.Value);
            
        if (TechRegModel.Id == 0)
            return NotFound();

        return Page();
    }
}