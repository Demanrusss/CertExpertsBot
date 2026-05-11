using ManageDb.Models;
using ManageDb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManageDb.Pages.Views.TechReg;

public class EditModel(ITechRegService<TechRegModel> techRegService) : PageModel
{
    [BindProperty] public TechRegModel TechRegModel { get; set; } = default!;
    [BindProperty] public int[] SelectedTNVEDCodeIds { get; set; } = [];
    public MultiSelectList TNVEDCodeMSL { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        ViewData["Title"] = "Изменить";

        if (id == null)
            return NotFound();

        TechRegModel = await techRegService.GetByIdAsync(id.Value);
        if (TechRegModel.Id == 0)
            return NotFound();
                
        SelectedTNVEDCodeIds = TechRegModel.TNVEDCodes?.Select(x => x.Id).ToArray() ?? [];
        await LoadTNVEDCodesAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadTNVEDCodesAsync();
            return Page();
        }

        _ = await techRegService.UpdateAsync(TechRegModel);
        _ = await techRegService.UpdateTNVEDCodesAsync(TechRegModel.Id, SelectedTNVEDCodeIds);

        return RedirectToPage("./Index");
    }
        
    private async Task LoadTNVEDCodesAsync()
    {
        var existingItems = (TechRegModel?.TNVEDCodes ?? Array.Empty<TNVEDCodeModel>()).ToList();
            
        var items = existingItems.OrderBy(x => x.Code).Select(x => new
        {
            x.Id,
            Text = x.Code + " - " + x.Name
        }).ToList();

        TNVEDCodeMSL = new MultiSelectList(
            items,
            "Id",
            "Text",
            SelectedTNVEDCodeIds);
    }
}