using Microsoft.EntityFrameworkCore;

namespace ASM_SIMS.DB
{
    public class SimsDataContext : DbContext
    {
        public SimsDataContext(DbContextOptions<SimsDataContext> options) : base(options)
        {
        }

        //Truyen vao DbSet de mapping voi bang trong database
        // gen ra cac bang trong database
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Courses> Courses { get; set; }
        public DbSet<Account> Accounts { get; set; } // Sửa "Account" thành "Accounts" để tuân theo quy ước đặt tên
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<ClassRoom> ClassRooms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Cấu hình quan hệ một-một giữa Student/Teacher và Account
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Account)
                .WithOne()
                .HasForeignKey<Student>(s => s.AccountId);

            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.Account)
                .WithOne()
                .HasForeignKey<Teacher>(t => t.AccountId);
        }
    }
}
/* sử dụng xóa mềm (soft delete) với trường DeletedAt để giữ lịch sử dữ liệu,
 thay vì xóa trực tiếp.
Các thực thể được định nghĩa với các thuộc tính bắt buộc (Required) và không bắt buộc phù hợp với logic nghiệp vụ.*/