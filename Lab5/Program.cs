using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews(options =>
{
    var messages = options.ModelBindingMessageProvider;
    messages.SetAttemptedValueIsInvalidAccessor((value, field) => $"Giá trị nhập cho {field} không hợp lệ.");
    messages.SetValueMustBeANumberAccessor(field => $"{field} phải là một số hợp lệ.");
    messages.SetValueMustNotBeNullAccessor(value => "Vui lòng nhập thông tin bắt buộc.");
    messages.SetUnknownValueIsInvalidAccessor(field => $"Giá trị nhập cho {field} không hợp lệ.");
});
builder.Services.AddSingleton<StudentRepository>();
builder.Services.AddDbContext<SchoolContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("SchoolContext")
    ?? throw new InvalidOperationException("Chưa cấu hình ConnectionStrings:SchoolContext.")));
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    await DbInitializer.InitializeAsync(scope.ServiceProvider.GetRequiredService<SchoolContext>());
}
if (args.Contains("--initialize-database")) return;
if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Home/Error");
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute("StudentList", "Admin/Student/List", new { controller = "Student", action = "Index" });
app.MapControllerRoute("StudentAdd", "Admin/Student/Add", new { controller = "Student", action = "Create" });
app.MapControllerRoute("Students", "Students/List", new { controller = "Student", action = "Index" });
app.MapControllerRoute("default", "{controller=Learner}/{action=Index}/{id?}");
app.Run();
