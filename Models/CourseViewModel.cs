using ASM_SIMS.Validations;
using System.ComponentModel.DataAnnotations;

namespace ASM_SIMS.Models
{
    public class CourseViewModel
    {
        public List<CourseDetail> courseList { get; set; }
    }

    public class CourseDetail
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Tên khóa học là bắt buộc")]
        public string NameCourse { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "Danh mục là bắt buộc")]
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        public DateOnly StartDate { get; set; }
        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
        [CustomValidation(typeof(CourseDetail), nameof(ValidateEndDate))] // Sửa thành CourseDetail
        public DateOnly EndDate { get; set; }

        [AllowedSizeFile(3 * 1024 * 1024)]
        [AllowedTypeFile(new string[] { ".jpg", ".png", ".jpeg", ".gif" })]
        public IFormFile? AvatarCourseFile { get; set; }
        public string? AvatarCourse { get; set; }
        [Range(0, 5, ErrorMessage = "Đánh giá phải từ 0 đến 5")]
        public int Vote { get; set; }
        public bool Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public static ValidationResult ValidateEndDate(DateOnly endDate, ValidationContext context)
        {
            var instance = (CourseDetail)context.ObjectInstance; // Ép kiểu thành CourseDetail
            if (endDate < instance.StartDate)
            {
                return new ValidationResult("Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu");
            }
            return ValidationResult.Success;
        }
    }
}