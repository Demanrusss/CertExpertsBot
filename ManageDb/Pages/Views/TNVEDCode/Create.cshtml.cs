using ManageDb.Models;
using ManageDb.Services;
using Microsoft.AspNetCore.Mvc;

namespace ManageDb.Pages.Views.TNVEDCode;

public class CreateModel(
    ITNVEDCodeService<TNVEDCodeModel> tNvedCodeService,
    ITechRegService<TechRegModel> techRegService)
    : TechRegNamePageModel
{
    public IActionResult OnGet()
    {
        ViewData["Title"] = "Create";

        PopulateTechRegsDropDownList(techRegService, new List<TechRegModel>());

        return Page();
    }

    public async Task<IActionResult> OnGetCopyFrom(int id)
    {
        ViewData["Title"] = $"Create from {id}";

        if (id == 0)
            return NotFound();

        TnvedCodeModel = await tNvedCodeService.GetByIdAsync(id);

        if (TnvedCodeModel.Id == 0)
            return NotFound();

        TnvedCodeModel.Id = 0;
        TnvedCodeModel.Name = String.Empty;
        TnvedCodeModel.Code = String.Empty;
        var selectedTechRegs = TnvedCodeModel.TechRegs ?? [];
        PopulateTechRegsDropDownList(techRegService, selectedTechRegs);

        return Page();
    }

    [BindProperty]
    public TNVEDCodeModel TnvedCodeModel { get; set; } = default!;

    public async Task<IActionResult> OnPostAsync(int[] TNVEDCode_TechRegs)
    {
        if (!ModelState.IsValid)
            return Page();

        await AddTechRegs(TNVEDCode_TechRegs);
        await tNvedCodeService.AddAsync(TnvedCodeModel);

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