using ManageDb.Models;
using ManageDb.Services;
using Microsoft.AspNetCore.Mvc;

namespace ManageDb.Pages.Views.TNVEDCode;

public class EditModel(
    ITNVEDCodeService<TNVEDCodeModel> tNvedCodeService,
    ITechRegService<TechRegModel> techRegService)
    : TechRegNamePageModel
{
    [BindProperty]
    public TNVEDCodeModel TnvedCodeModel { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        ViewData["Title"] = "Edit";

        if (id == null)
            return NotFound();

        TnvedCodeModel = await tNvedCodeService.GetByIdAsync(id.Value);

        if (TnvedCodeModel.Id == 0)
            return NotFound();

        var selectedTechRegs = TnvedCodeModel.TechRegs ?? [];
        PopulateTechRegsDropDownList(techRegService, selectedTechRegs);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int[] TNVEDCode_TechRegs)
    {
        if (!ModelState.IsValid)
            return Page();

        await AddTechRegs(TNVEDCode_TechRegs);

        var result = await tNvedCodeService.UpdateAsync(TnvedCodeModel);
        if (result == 0)
            return NotFound();

        return RedirectToPage("./Index");
    }

    private async Task AddTechRegs(int[] TNVEDCode_TechRegs)
    {
        TnvedCodeModel.TechRegs = new List<TechRegModel>();
        TechRegModel techRegModel;
        foreach (var t in TNVEDCode_TechRegs)
        {
            techRegModel = await techRegService.GetByIdAsync(t);
            if (techRegModel.Id == 0)
                continue;

            TnvedCodeModel.TechRegs.Add(techRegModel);
        }
    }
}