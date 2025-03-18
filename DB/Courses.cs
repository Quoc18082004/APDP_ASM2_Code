using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ASM_SIMS.DB
{
    public class Courses
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("CategoryId")]
        public int CategoryId { get; set; }

        [Column("NameCourse", TypeName = "Varchar(60)"), Required]
        public string NameCourse { get; set; }

        [Column("Description", TypeName = "Varchar(255)"), AllowNull]
        public string? Description { get; set; }

        [Column("StartDate"), Required]
        public DateOnly StartDate { get; set; }

        [Column("EndDate"), Required]
        public DateOnly EndDate { get; set; }

        [Column("AvatarCourse", TypeName = "Varchar(150)"), AllowNull]
        public string? AvatarCourse { get; set; }

        [Column("Vote", TypeName = "Integer"), Required]
        public int Vote { get; set; }

        [Column("Status", TypeName = "Varchar(20)"), Required]
        public bool Status { get; set; }



        [AllowNull]
        public DateTime? CreatedAt { get; set; }

        [AllowNull]
        public DateTime? UpdatedAt { get; set; }

        [AllowNull]
        public DateTime? DeletedAt { get; set; }

    }
}
