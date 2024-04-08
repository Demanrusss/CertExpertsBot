using ManageDb.Services;
using Microsoft.AspNetCore.Mvc;

namespace ManageDb.Pages.Views.TNVEDCode
{
    public class EditModel : TechRegNamePageModel
    {
        private readonly ITNVEDCodeService<Models.TNVEDCode> tNVEDCodeService;
        private readonly ITechRegService<Models.TechReg> techRegService;

        public EditModel(ITNVEDCodeService<Models.TNVEDCode> tNVEDCodeService,
                         ITechRegService<Models.TechReg> techRegService)
        {
            this.tNVEDCodeService = tNVEDCodeService;
            this.techRegService = techRegService;
        }

        [BindProperty]
        public ManageDb.Models.TNVEDCode TNVEDCode { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            ViewData["Title"] = "Edit";

            if (id == null || tNVEDCodeService == null)
                return NotFound();

            TNVEDCode = await tNVEDCodeService.GetByIdAsync(id.Value);

            if (TNVEDCode.Id == 0)
                return NotFound();

            var selectedTechRegs = TNVEDCode.TechRegs;
            PopulateTechRegsDropDownList(techRegService, selectedTechRegs);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int[] TNVEDCode_TechRegs)
        {
            if (!ModelState.IsValid)
                return Page();

            await AddTechRegs(TNVEDCode_TechRegs);

            var result = await tNVEDCodeService.UpdateAsync(TNVEDCode);
            if (result == 0)
                return NotFound();

            return RedirectToPage("./Index");
        }

        private async Task AddTechRegs(int[] TNVEDCode_TechRegs)
        {
            TNVEDCode.TechRegs = new List<Models.TechReg>();
            Models.TechReg techReg;
            for (int i = 0; i < TNVEDCode_TechRegs.Length; i++)
            {
                techReg = await techRegService.GetByIdAsync(TNVEDCode_TechRegs[i]);
                if (techReg.Id == 0)
                    continue;

                TNVEDCode.TechRegs.Add(techReg);
            }
        }
    }
}
