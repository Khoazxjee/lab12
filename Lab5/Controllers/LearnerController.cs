using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
namespace MyWebApp.Controllers;

public class LearnerController(SchoolContext db) : Controller
{
    private async Task<List<Learner>> FilterAsync(int? mid, string? q)
    {
        var query = db.Learners.AsNoTracking().AsQueryable();
        if (mid.HasValue) query = query.Where(l => l.MajorID == mid);
        if (!string.IsNullOrWhiteSpace(q))
        {
            var keyword = q.Trim();
            query = query.Where(l => l.FirstMidName.Contains(keyword) || l.LastName.Contains(keyword) || l.Major!.MajorName.Contains(keyword));
        }
        return await query.Include(l => l.Major).OrderBy(l => l.LearnerID).ToListAsync();
    }

    public async Task<IActionResult> Index(int? mid, string? q, string? mode)
    {
        ViewBag.Mid = mid;
        ViewBag.Search = q?.Trim();
        ViewBag.Mode = mode == "sync" ? "sync" : "ajax";
        return View(await FilterAsync(mid, q));
    }

    public async Task<IActionResult> LearnerByMajorID(int? mid, string? q)
    {
        return PartialView("LearnerTable", await FilterAsync(mid, q));
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();
        var learner = await db.Learners.Include(l => l.Major)
            .Include(l => l.Enrollments).ThenInclude(e => e.Course)
            .AsNoTracking().FirstOrDefaultAsync(l => l.LearnerID == id);
        return learner is null ? NotFound() : View(learner);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateMajorsAsync();
        return View(new Learner { EnrollmentDate = DateTime.Today });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("FirstMidName,LastName,MajorID,EnrollmentDate")] Learner learner)
    {
        await ValidateMajorAsync(learner.MajorID);
        if (ModelState.IsValid)
        {
            db.Learners.Add(learner);
            await db.SaveChangesAsync();
            TempData["Success"] = "Đã thêm người học và lưu vào cơ sở dữ liệu.";
            return RedirectToAction(nameof(Index));
        }
        await PopulateMajorsAsync(learner.MajorID);
        return View(learner);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();
        var learner = await db.Learners.FindAsync(id.Value);
        if (learner is null) return NotFound();
        await PopulateMajorsAsync(learner.MajorID);
        return View(learner);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("LearnerID,FirstMidName,LastName,MajorID,EnrollmentDate")] Learner learner)
    {
        if (id != learner.LearnerID) return NotFound();
        var existing = await db.Learners.FindAsync(id);
        if (existing is null) return NotFound();
        await ValidateMajorAsync(learner.MajorID);
        if (ModelState.IsValid)
        {
            existing.FirstMidName = learner.FirstMidName;
            existing.LastName = learner.LastName;
            existing.MajorID = learner.MajorID;
            existing.EnrollmentDate = learner.EnrollmentDate;
            try { await db.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!await db.Learners.AnyAsync(l => l.LearnerID == id)) return NotFound();
                throw;
            }
            TempData["Success"] = "Đã cập nhật thông tin người học.";
            return RedirectToAction(nameof(Index));
        }
        await PopulateMajorsAsync(learner.MajorID);
        return View(learner);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();
        var learner = await ForDeleteAsync(id.Value);
        if (learner is null) return NotFound();
        ViewBag.CanDelete = learner.Enrollments.Count == 0;
        return View(learner);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var learner = await ForDeleteAsync(id);
        if (learner is null) return NotFound();
        // Kiểm tra lại ở POST; không dựa riêng vào nút xác nhận của giao diện.
        if (learner.Enrollments.Count > 0)
        {
            ModelState.AddModelError("", "Không thể xóa người học đang có đăng ký học.");
            ViewBag.CanDelete = false;
            return View("Delete", learner);
        }
        db.Learners.Remove(learner);
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateException)
        {
            db.ChangeTracker.Clear();
            learner = await ForDeleteAsync(id);
            if (learner is null) return NotFound();
            ModelState.AddModelError("", "Không thể xóa. Dữ liệu có thể đang được tham chiếu; vui lòng tải lại trang.");
            ViewBag.CanDelete = false;
            return View("Delete", learner);
        }
        TempData["Success"] = "Đã xóa người học.";
        return RedirectToAction(nameof(Index));
    }

    private Task<Learner?> ForDeleteAsync(int id) => db.Learners.Include(l => l.Major)
        .Include(l => l.Enrollments).FirstOrDefaultAsync(l => l.LearnerID == id);

    private async Task PopulateMajorsAsync(int? selected = null) =>
        ViewBag.MajorID = new SelectList(await db.Majors.AsNoTracking().OrderBy(m => m.MajorID).ToListAsync(), "MajorID", "MajorName", selected);

    private async Task ValidateMajorAsync(int id)
    {
        if (id > 0 && !await db.Majors.AnyAsync(m => m.MajorID == id))
            ModelState.AddModelError(nameof(Learner.MajorID), "Ngành học không tồn tại.");
    }
}
