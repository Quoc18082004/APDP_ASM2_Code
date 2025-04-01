using System.ComponentModel.DataAnnotations;

namespace ASM_SIMS.Models
{
    public class TeacherViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        public string Phone { get; set; }

        public string Address { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        public string Status { get; set; }

        public int? CourseId { get; set; } // Thêm trường CourseId

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Nếu cần hiển thị danh sách lớp học hoặc khóa học liên quan, có thể thêm:
        // public List<int> ClassRoomIds { get; set; } // Hoặc thông tin khác
    }
}