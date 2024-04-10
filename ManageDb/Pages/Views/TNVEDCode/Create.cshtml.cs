using ManageDb.Services;
using Microsoft.AspNetCore.Mvc;

namespace ManageDb.Pages.Views.TNVEDCode
{
    public class CreateModel : TechRegNamePageModel
    {
        private readonly ITNVEDCodeService<Models.TNVEDCode> tNVEDCodeService;
        private readonly ITechRegService<Models.TechReg> techRegService;

        public CreateModel(ITNVEDCodeService<Models.TNVEDCode> tNVEDCodeService, 
                           ITechRegService<Models.TechReg> techRegService)
        {
            this.tNVEDCodeService = tNVEDCodeService;
            this.techRegService = techRegService;
        }

        public IActionResult OnGet()
        {
            ViewData["Title"] = "Create";

            PopulateTechRegsDropDownList(techRegService, new List<Models.TechReg>());

            return Page();
        }

        public async Task<IActionResult> OnGetCopyFrom(int id)
        {
            ViewData["Title"] = $"Create from {id}";

            if (id == 0 || tNVEDCodeService == null)
                return NotFound();

            TNVEDCode = await tNVEDCodeService.GetByIdAsync(id);

            if (TNVEDCode.Id == 0)
                return NotFound();

            TNVEDCode.Id = 0;
            TNVEDCode.Name = String.Empty;
            TNVEDCode.Code = String.Empty;
            var selectedTechRegs = TNVEDCode.TechRegs;
            PopulateTechRegsDropDownList(techRegService, selectedTechRegs);

            return Page();
        }

        [BindProperty]
        public ManageDb.Models.TNVEDCode TNVEDCode { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync(int[] TNVEDCode_TechRegs)
        {
            if (!ModelState.IsValid || tNVEDCodeService == null || TNVEDCode == null)
                return Page();

            await AddTechRegs(TNVEDCode_TechRegs);
            await tNVEDCodeService.AddAsync(TNVEDCode);

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
