using ManageDb.Models;
using ManageDb.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManageDb.Pages.Views.TechReg;

public class IndexModel(ITechRegService<TechRegModel> techRegService) : PageModel
{
    public ICollection<TechRegModel> TechRegs { get;set; } = [];

    public async Task OnGetAsync()
    {
        ViewData["Title"] = "Тех. регламенты / Решения";
        TechRegs = await techRegService.GetAllAsync();
    }
}