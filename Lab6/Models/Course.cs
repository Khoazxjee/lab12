using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MyWebApp.Models;
public class Course
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int CourseID { get; set; }
    [Required, StringLength(200)]
    public string Title { get; set; } = "";
    [Range(1, 30)]
    public int Credits { get; set; }
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new HashSet<Enrollment>();
}
