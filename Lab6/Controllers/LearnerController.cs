using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
namespace MyWebApp.Controllers;

public class LearnerController(SchoolContext db) : Controller
{
    private const int PageSize = 3;

    private async Task<List<Learner>> ReadPageAsync(int? mid, string? keyword, int? pageIndex)
    {
        var query = db.Learners.AsNoTracking().AsQueryable();
        if (mid.HasValue) query = query.Where(l => l.MajorID == mid);
        keyword = keyword?.Trim();
        if (!string.IsNullOrEmpty(keyword))
        {
            var term = keyword.ToLower();
            query = query.Where(l => l.FirstMidName.ToLower().Contains(term));
        }
        int total = await query.CountAsync();
        int pages = (int)Math.Ceiling(total / (double)PageSize);
        int page = Math.Clamp(pageIndex ?? 1, 1, Math.Max(1, pages));
        ViewBag.Mid = mid;
        ViewBag.Search = keyword;
        ViewBag.PageNum = pages;
        ViewBag.PageIndex = page;
        ViewBag.Total = total;
        return await query.Include(l => l.Major).OrderBy(l => l.LearnerID)
            .Skip((page - 1) * PageSize).Take(PageSize).ToListAsync();
    }

    public async Task<IActionResult> Index(int? mid, string? keyword, int? pageIndex)
    {
        return View(await ReadPageAsync(mid, keyword, pageIndex));
    }

    public async Task<IActionResult> LearnerFilter(int? mid, string? keyword, int? pageIndex)
    {
        return PartialView("LearnerTable", await ReadPageAsync(mid, keyword, pageIndex));
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
