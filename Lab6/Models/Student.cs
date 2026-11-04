using System.ComponentModel.DataAnnotations;
namespace MyWebApp.Models;

public class Student
{
    [Display(Name = "Mã sinh viên")]
    public int Id { get; set; }

    [Display(Name = "Họ tên")]
    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    [StringLength(100, MinimumLength = 4, ErrorMessage = "Họ tên phải có từ 4 đến 100 ký tự.")]
    public string? Name { get; set; }

    [Display(Name = "Email")]
    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    [RegularExpression(@"^[^@\s]+@[gG][mM][aA][iI][lL]\.[cC][oO][mM]$", ErrorMessage = "Email phải có đuôi @gmail.com.")]
    public string? Email { get; set; }

    [Display(Name = "Mật khẩu")]
    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Mật khẩu phải có từ 8 đến 100 ký tự.")]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[^\w\s]).+$", ErrorMessage = "Mật khẩu cần có chữ hoa, chữ thường, chữ số và ký tự đặc biệt.")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Display(Name = "Ngành học")]
    [Required(ErrorMessage = "Vui lòng chọn ngành học.")]
    [EnumDataType(typeof(Branch), ErrorMessage = "Ngành học không hợp lệ.")]
    public Branch? Branch { get; set; }

    [Display(Name = "Giới tính")]
    [Required(ErrorMessage = "Vui lòng chọn giới tính.")]
    [EnumDataType(typeof(Gender), ErrorMessage = "Giới tính không hợp lệ.")]
    public Gender? Gender { get; set; }

    [Display(Name = "Hệ chính quy")]
    public bool IsRegular { get; set; }

    [Display(Name = "Địa chỉ")]
    [Required(ErrorMessage = "Vui lòng nhập địa chỉ.")]
    [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự.")]
    [DataType(DataType.MultilineText)]
    public string? Address { get; set; }

    // Giữ tên DateOfBorth để khớp tài liệu và các lab trước.
    [Display(Name = "Ngày sinh")]
    [Required(ErrorMessage = "Vui lòng nhập ngày sinh.")]
    [DataType(DataType.Date)]
    [Range(typeof(DateTime), "1963-01-01", "2005-12-31", ParseLimitsInInvariantCulture = true,
        ErrorMessage = "Ngày sinh phải từ 01/01/1963 đến 31/12/2005 theo đề bài.")]
    public DateTime? DateOfBorth { get; set; }

    [Display(Name = "Điểm")]
    [Required(ErrorMessage = "Vui lòng nhập điểm.")]
    [Range(0.0, 10.0, ErrorMessage = "Điểm phải nằm trong khoảng từ 0 đến 10.")]
    public double? Score { get; set; }

    public string? AvatarPath { get; set; }
}
