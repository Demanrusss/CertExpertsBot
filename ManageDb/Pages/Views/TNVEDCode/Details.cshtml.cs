using ManageDb.Models;
using ManageDb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManageDb.Pages.Views.TNVEDCode;

public class DetailsModel(ITNVEDCodeService<TNVEDCodeModel> tNvedCodeService) : PageModel
{
    public TNVEDCodeModel TnvedCodeModel { get; set; } = default!; 

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        ViewData["Title"] = "Details";

        if (id == null)
            return NotFound();

        TnvedCodeModel = await tNvedCodeService.GetByIdAsync(id.Value);
        if (TnvedCodeModel.Id == 0)
            return NotFound();

        return Page();
    }
}