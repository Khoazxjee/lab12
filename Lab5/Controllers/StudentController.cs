using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyWebApp.Models;
using MyWebApp.Services;

namespace MyWebApp.Controllers;

public class StudentController : Controller
{
    private readonly StudentRepository students;
    private readonly IWebHostEnvironment environment;

    public StudentController(StudentRepository students, IWebHostEnvironment environment)
    {
        this.students = students;
        this.environment = environment;
    }

    public IActionResult Index(string? q, string mode = "action")
    {
        ViewBag.LinkMode = GetMode(mode);
        ViewBag.Search = q;
        var result = students.GetAll();
        if (!string.IsNullOrWhiteSpace(q))
            result = result.Where(s => s.Id.ToString().Contains(q)
                || (s.Name?.Contains(q, StringComparison.OrdinalIgnoreCase) ?? false)
                || (s.Email?.Contains(q, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();
        return View(result);
    }

    [HttpGet]
    public IActionResult Create(string mode = "action")
    {
        PrepareForm(mode);
        return View(new Student());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(3 * 1024 * 1024)]
    public async Task<IActionResult> Create(
        [Bind("Name,Email,Password,Branch,Gender,IsRegular,Address,DateOfBorth,Score")] Student student,
        IFormFile? avatar, string mode = "action")
    {
        byte[]? avatarBytes = null;
        string? extension = null;
        if (avatar != null)
        {
            extension = Path.GetExtension(avatar.FileName).ToLowerInvariant();
            if (avatar.Length == 0 || avatar.Length > 2 * 1024 * 1024
                || (extension != ".png" && extension != ".jpg" && extension != ".jpeg"))
                ModelState.AddModelError("avatar", "Chọn ảnh PNG hoặc JPG không quá 2 MB.");
            else
            {
                using var buffer = new MemoryStream();
                await avatar.CopyToAsync(buffer);
                avatarBytes = buffer.ToArray();
                bool png = avatarBytes.Length >= 8 && avatarBytes.Take(8).SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 });
                bool jpg = avatarBytes.Length >= 3 && avatarBytes[0] == 255 && avatarBytes[1] == 216 && avatarBytes[2] == 255;
                if (!(extension == ".png" ? png : jpg))
                    ModelState.AddModelError("avatar", "Tệp đã chọn không phải ảnh hợp lệ.");
            }
        }
        if (!ModelState.IsValid)
        {
            PrepareForm(mode);
            return View(student);
        }
        if (avatarBytes != null)
        {
            string folder = Path.Combine(environment.WebRootPath, "uploads");
            Directory.CreateDirectory(folder);
            string fileName = Guid.NewGuid().ToString("N") + extension;
            await System.IO.File.WriteAllBytesAsync(Path.Combine(folder, fileName), avatarBytes);
            student.AvatarPath = "/uploads/" + fileName;
        }
        students.Add(student);
        return RedirectToRoute("StudentList", new { mode = GetMode(mode) });
    }

    public IActionResult PartialLayout()
    {
        ViewData["UsePartialLayout"] = true;
        ViewBag.LinkMode = "action";
        return View("Index", students.GetAll());
    }

    private static string GetMode(string mode) => mode is "html" or "route" ? mode : "action";

    private void PrepareForm(string mode)
    {
        ViewBag.LinkMode = GetMode(mode);
        ViewBag.AllGenders = Enum.GetValues<Gender>().ToList();
        ViewBag.AllBranches = Enum.GetValues<Branch>()
            .Select(b => new SelectListItem
            {
                Text = b switch { Branch.IT => "Công nghệ thông tin", Branch.BE => "Kinh tế", Branch.CE => "Công trình", _ => "Điện - Điện tử" },
                Value = ((int)b).ToString()
            }).ToList();
    }
}
