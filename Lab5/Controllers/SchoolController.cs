using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
namespace MyWebApp.Controllers;
public class SchoolController(SchoolContext db) : Controller
{
    public async Task<IActionResult> Majors() => View(await db.Majors.AsNoTracking().Include(m => m.Learners).OrderBy(m => m.MajorID).ToListAsync());
    public async Task<IActionResult> Courses() => View(await db.Courses.AsNoTracking().OrderBy(c => c.CourseID).ToListAsync());
    public async Task<IActionResult> Enrollments() => View(await db.Enrollments.AsNoTracking().Include(e => e.Learner).Include(e => e.Course).OrderBy(e => e.EnrollmentID).ToListAsync());
}
