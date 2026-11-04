using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
namespace MyWebApp.Models;
public class Learner
{
    [Display(Name = "Mã người học")]
    public int LearnerID { get; set; }
    [Required(ErrorMessage = "Vui lòng nhập họ.")]
    [StringLength(50, ErrorMessage = "Họ không được vượt quá 50 ký tự.")]
    [Display(Name = "Họ")]
    public string LastName { get; set; } = "";
    [Required(ErrorMessage = "Vui lòng nhập tên và tên đệm.")]
    [StringLength(100, ErrorMessage = "Tên và tên đệm không được vượt quá 100 ký tự.")]
    [Display(Name = "Tên và tên đệm")]
    public string FirstMidName { get; set; } = "";
    [Required(ErrorMessage = "Vui lòng nhập ngày nhập học.")]
    [Range(typeof(DateTime), "1900-01-01", "2100-12-31", ParseLimitsInInvariantCulture = true,
        ErrorMessage = "Ngày nhập học phải từ 01/01/1900 đến 31/12/2100.")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày nhập học")]
    public DateTime EnrollmentDate { get; set; }
    [Required(ErrorMessage = "Vui lòng chọn ngành học.")]
    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn ngành học.")]
    [Display(Name = "Ngành học")]
    public int MajorID { get; set; }
    [ValidateNever]
    public virtual Major? Major { get; set; }
    [ValidateNever]
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new HashSet<Enrollment>();
}
