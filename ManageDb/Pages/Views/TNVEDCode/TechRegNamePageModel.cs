using ManageDb.Models;
using ManageDb.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManageDb.Pages.Views.TNVEDCode;

public class TechRegNamePageModel : PageModel
{
    public MultiSelectList TechRegNameMSL { get; set; } = null!;

    public void PopulateTechRegsDropDownList(ITechRegService<TechRegModel> techRegService,
        ICollection<TechRegModel> selectedTechRegs)
    {
        var techRegsQuery = techRegService.GetAllAsync().Result
            .Where(tr => !selectedTechRegs.Contains(tr))
            .OrderBy(tr => tr.Name);

        TechRegNameMSL = new MultiSelectList(techRegsQuery,
            nameof(TechRegModel.Id),
            nameof(TechRegModel.Name),
            selectedTechRegs);
    }
}