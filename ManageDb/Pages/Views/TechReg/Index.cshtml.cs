﻿using ManageDb.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManageDb.Pages.Views.TechReg
{
    public class IndexModel : PageModel
    {
        private readonly ITechRegService<Models.TechReg> techRegService;

        public IndexModel(ITechRegService<Models.TechReg> techRegService)
        {
            this.techRegService = techRegService;
        }

        public ICollection<ManageDb.Models.TechReg> TechRegs { get;set; } = default!;

        public async Task OnGetAsync()
        {
            ViewData["Title"] = "Тех. регламенты / Решения";

            if (techRegService != null)
                TechRegs = await techRegService.GetAllAsync();
        }
    }
}
