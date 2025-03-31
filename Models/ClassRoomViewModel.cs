using System.ComponentModel.DataAnnotations;

namespace ASM_SIMS.Models
{
    public class ClassRoomViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên lớp là bắt buộc")]
        public string ClassName { get; set; }

        [Required(ErrorMessage = "Khóa học là bắt buộc")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Giảng viên là bắt buộc")]
        public int TeacherId { get; set; }

        public string Schedule { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
