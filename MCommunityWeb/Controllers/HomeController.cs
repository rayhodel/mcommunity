using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MCommunityWeb.Models;
using MCommunityWeb.Services;
using System.Text.RegularExpressions;

namespace MCommunityWeb.Controllers;

public class HomeController : Controller
{
    private readonly MCommunityService _mCommunityService;

    public HomeController(MCommunityService mCommunityService)
    {
        _mCommunityService = mCommunityService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Search([FromBody] SearchRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Query) || string.IsNullOrWhiteSpace(request.AppId) || string.IsNullOrWhiteSpace(request.Secret))
        {
            return BadRequest("Missing query or credentials.");
        }

        try
        {
            var token = await _mCommunityService.GetTokenAsync(request.AppId, request.Secret);
            
            object? result = null;

            // Uniqname: 3-8 chars
            // Group: 9-62 chars
            if (request.Query.Length <= 8)
            {
                 result = await _mCommunityService.GetPersonAsync(token, request.Query);
            }
            else
            {
                 result = await _mCommunityService.GetGroupAsync(token, request.Query);
            }

            if (result == null)
            {
                return NotFound("Entry not found.");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
