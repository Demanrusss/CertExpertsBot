using ManageDb.Models;
using ManageDb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManageDb.Pages.Views.TNVEDCode;

public class DeleteModel(ITNVEDCodeService<TNVEDCodeModel> tNvedCodeService) : PageModel
{
    [BindProperty]
    public TNVEDCodeModel TnvedCodeModel { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        ViewData["Title"] = "Удалить";

        if (id == null)
            return NotFound();

        TnvedCodeModel = await tNvedCodeService.GetByIdAsync(id.Value);
        if (TnvedCodeModel.Id == 0)
            return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null)
            return NotFound();

        var result = await tNvedCodeService.DeleteAsync(id.Value);
        if (result == 0)
            return NotFound();
            
        return RedirectToPage("./Index");
    }
}