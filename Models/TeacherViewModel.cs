﻿using System.ComponentModel.DataAnnotations;

namespace ASM_SIMS.Models
{
    public class TeacherViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        public string Phone { get; set; }

        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
