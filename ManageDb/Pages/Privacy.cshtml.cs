using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManageDb.Pages;

public class PrivacyModel : PageModel
{
    public void OnGet()
    {
        ViewData["Title"] = "Конфиденциальность";
    }
}