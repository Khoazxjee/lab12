using Microsoft.AspNetCore.Mvc;
using MyWebApp.Models;
namespace MyWebApp.ViewComponents;
public class RenderViewComponent : ViewComponent
{
    public IViewComponentResult Invoke() => View("RenderLeftMenu", new List<MenuItem>
    {
        new() { Id = 1, Name = "Người học", Link = "Learner" },
        new() { Id = 2, Name = "Ngành học", Link = "School/Majors" },
        new() { Id = 3, Name = "Môn học", Link = "School/Courses" },
        new() { Id = 4, Name = "Đăng ký học", Link = "School/Enrollments" },
        new() { Id = 5, Name = "Sinh viên · Lab 3", Link = "Student" }
    });
}
