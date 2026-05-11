using ManageDb.Models;
using ManageDb.Services;
using Microsoft.AspNetCore.Mvc;

namespace ManageDb.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TNVEDCodesController(ITNVEDCodeService<TNVEDCodeModel> tnvedCodeService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(string? q, int page = 1)
    {
        var pageSize = 30;
        var codes = string.IsNullOrWhiteSpace(q) 
            ? await tnvedCodeService.GetAllAsync(page, pageSize) 
            : await tnvedCodeService.GetByCodeAsync(q);

        var items = codes.Select(c => new
        {
            id = c.Id,
            text = c.Code
        }).ToList();

        return Ok(new { items, total_count = items.Count });
    }
}