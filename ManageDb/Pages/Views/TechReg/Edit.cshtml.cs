using ManageDb.Models;
using ManageDb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManageDb.Pages.Views.TechReg;

public class EditModel(ITechRegService<TechRegModel> techRegService, ITNVEDCodeService<TNVEDCodeModel> tnvedCodeService) : PageModel
{
    [BindProperty]
    public TechRegModel TechRegModel { get; set; } = default!;

    [BindProperty]
    public int[] SelectedTNVEDCodeIds { get; set; } = [];

    public MultiSelectList TNVEDCodeMSL { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        ViewData["Title"] = "Edit";

        if (id == null)
            return NotFound();

        TechRegModel = await techRegService.GetByIdAsync(id.Value);
        if (TechRegModel.Id == 0)
            return NotFound();
                
        // Get current selected codes (needs a separate method or handled internally)
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

        var result = await techRegService.UpdateAsync(TechRegModel);

        // TechRegService update might not actually fail if 0 means "no scalar changes" or depending on implementation.
        // But if it's fine, we update TNVEDs.
        await techRegService.UpdateTNVEDCodesAsync(TechRegModel.Id, SelectedTNVEDCodeIds);

        return RedirectToPage("./Index");
    }
        
    private async Task LoadTNVEDCodesAsync()
    {
        // Instead of GetAllAsync (which returns top 10), we only load the CURRENTLY selected ones into the MultiSelectList
        // so they display in the multi-select. The rest will be loaded via AJAX/Select2.
            
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