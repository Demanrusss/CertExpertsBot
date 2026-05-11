using ManageDb.Models;
using ManageDb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManageDb.Pages.Views.TechReg;

public class CreateModel(ITechRegService<TechRegModel> techRegService) : PageModel
{
    public IActionResult OnGet()
    {
        ViewData["Title"] = "Create";

        return Page();
    }

    [BindProperty]
    public TechRegModel TechRegModel { get; set; } = default!;
        
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        await techRegService.AddAsync(TechRegModel);

        return RedirectToPage("./Index");
    }
}