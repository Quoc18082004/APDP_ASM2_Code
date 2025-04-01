using System.ComponentModel.DataAnnotations;

namespace ASM_SIMS.Models
{
    public class ClassRoomViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên lớp là bắt buộc")]
        [StringLength(60, ErrorMessage = "Tên lớp không được dài quá 60 ký tự")]
        public string ClassName { get; set; }

        [Required(ErrorMessage = "Khóa học là bắt buộc")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Giảng viên là bắt buộc")]
        public int TeacherId { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        public DateOnly StartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
        [CustomValidation(typeof(ClassRoomViewModel), nameof(ValidateEndDate))]
        public DateOnly EndDate { get; set; }

        [Required(ErrorMessage = "Lịch học là bắt buộc")]
        [StringLength(100, ErrorMessage = "Lịch học không được dài quá 100 ký tự")]
        public string Schedule { get; set; } 

        [StringLength(100, ErrorMessage = "Địa điểm không được dài quá 100 ký tự")]
        public string? Location { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        [StringLength(20, ErrorMessage = "Trạng thái không được dài quá 20 ký tự")]
        public string Status { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public static ValidationResult ValidateEndDate(DateOnly endDate, ValidationContext context)
        {
            var instance = (ClassRoomViewModel)context.ObjectInstance;
            if (endDate < instance.StartDate)
            {
                return new ValidationResult("Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu");
            }
            return ValidationResult.Success;
        }
    }
}