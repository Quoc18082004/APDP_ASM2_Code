using System.ComponentModel.DataAnnotations;
using ASM_SIMS.Validations;

namespace ASM_SIMS.Models
{
    public class StudentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email invalid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        public string Phone { get; set; }

        public string Address { get; set; }

        public int? AccountId { get; set; }

        public int? ClassRoomId { get; set; }

        public int? CourseId { get; set; }

        [AllowedSizeFile(3 * 1024 * 1024)]
        [AllowedTypeFile(new string[] { ".jpg", ".png", ".jpeg", ".gif" })]
        public IFormFile? ViewAvatar { get; set; }

        public string? Avatar { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsSelected { get; set; }

        public bool IsEdit { get; set; }
    }
}
