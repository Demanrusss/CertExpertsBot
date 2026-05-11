using ManageDb.Models;
using ManageDb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManageDb.Pages.Views.TechReg;

public class DeleteModel(ITechRegService<TechRegModel> techRegService) : PageModel
{
    [BindProperty]
    public TechRegModel TechRegModel { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        ViewData["Title"] = "Delete";

        if (id == null)
            return NotFound();

        TechRegModel = await techRegService.GetByIdAsync(id.Value);

        if (TechRegModel.Id == 0)
            return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null)
            return NotFound();
            
        await techRegService.DeleteAsync(id.Value);

        return RedirectToPage("./Index");
    }
}