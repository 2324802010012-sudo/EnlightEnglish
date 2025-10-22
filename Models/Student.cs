using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EnlightEnglishCenter.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [Required, Display(Name = "Họ và tên")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Điểm trung bình")]
        [Range(0, 10)]
        public double? Grade { get; set; }

    }
}
