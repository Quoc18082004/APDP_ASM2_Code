using System.ComponentModel.DataAnnotations;

namespace ASM_SIMS.Models
{
    public class StudentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email invalid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Pls, Enter your Phone Number")]
        public string Phone { get; set; }

        public string Address { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
