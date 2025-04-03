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
        [Required(ErrorMessage = "Course name is required")]
        public string NameCourse { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Start date is required")]
        public DateOnly StartDate { get; set; }
        [Required(ErrorMessage = "End date is required")]
        [CustomValidation(typeof(CourseDetail), nameof(ValidateEndDate))] // Sửa thành CourseDetail
        public DateOnly EndDate { get; set; }

        [AllowedSizeFile(3 * 1024 * 1024)]
        [AllowedTypeFile(new string[] { ".jpg", ".png", ".jpeg", ".gif" })]
        public IFormFile? AvatarCourseFile { get; set; }
        public string? AvatarCourse { get; set; }
        [Range(0, 5, ErrorMessage = "Rating must be from 0 to 5")]
        public int Vote { get; set; }
        public bool Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public static ValidationResult ValidateEndDate(DateOnly endDate, ValidationContext context)
        {
            var instance = (CourseDetail)context.ObjectInstance; // Ép kiểu thành CourseDetail
            if (endDate < instance.StartDate)
            {
                return new ValidationResult("End date must be greater than or equal to start date");
            }
            return ValidationResult.Success;
        }
    }
}