using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
namespace MyWebApp.ViewComponents;

public class MajorViewComponent(SchoolContext db) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var majors = await db.Majors.AsNoTracking().OrderBy(m => m.MajorID).ToListAsync();
        return View("RenderMajor", majors);
    }
}
