using Microsoft.AspNetCore.Mvc;

namespace MyWebApp.Controllers;

public class CatalogController : Controller
{
    [HttpGet("Branches/List")]
    public IActionResult Branches()
    {
        ViewData["Title"] = "Branches";
        return View("List", new[] { "IT - CNTT", "BE - Kinh tế", "CE - Công trình", "EE - Điện - Điện tử" });
    }

    [HttpGet("Subjects/List")]
    public IActionResult Subjects()
    {
        ViewData["Title"] = "Subjects";
        return View("List", new[] { "Lập trình Web", "Cơ sở dữ liệu" });
    }

    [HttpGet("Courses/List")]
    public IActionResult Courses()
    {
        ViewData["Title"] = "Courses";
        return View("List", new[] { "K65" });
    }
}
