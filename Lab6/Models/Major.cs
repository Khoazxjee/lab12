using System.ComponentModel.DataAnnotations;
namespace MyWebApp.Models;
public class Major
{
    public int MajorID { get; set; }
    [Required, StringLength(100)]
    [Display(Name = "Ngành học")]
    public string MajorName { get; set; } = "";
    public virtual ICollection<Learner> Learners { get; set; } = new HashSet<Learner>();
}
