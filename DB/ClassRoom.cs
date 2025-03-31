using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ASM_SIMS.DB
{
    public class ClassRoom
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("CourseId")]
        public int CourseId { get; set; }

        [ForeignKey("TeacherId")]
        public int TeacherId { get; set; }

        [Column("ClassName", TypeName = "Varchar(60)"), Required]
        public string ClassName { get; set; }

        [Column("Schedule", TypeName = "Varchar(100)")]
        public string Schedule { get; set; }

        [Column("Location", TypeName = "Varchar(100)")]
        public string Location { get; set; }

        [Column("Status", TypeName = "Varchar(20)"), Required]
        public string Status { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public Courses Course { get; set; }
        public Teacher Teacher { get; set; }
    }
}
