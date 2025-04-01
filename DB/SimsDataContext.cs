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
            // Cấu hình quan hệ một-một giữa Student và Account
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Account)
                .WithOne()
                .HasForeignKey<Student>(s => s.AccountId)
                .OnDelete(DeleteBehavior.NoAction); // Tránh cascade
                                                    // Cấu hình quan hệ một-một giữa Teacher và Account
            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.Account)
                .WithOne()
                .HasForeignKey<Teacher>(t => t.AccountId)
                .OnDelete(DeleteBehavior.NoAction); // Tránh cascade

            // Cấu hình quan hệ một-nhiều giữa Courses và ClassRoom
            modelBuilder.Entity<ClassRoom>()
                .HasOne(c => c.Course)
                .WithMany()
                .HasForeignKey(c => c.CourseId)
                .OnDelete(DeleteBehavior.NoAction); // Tránh cascade

            // Cấu hình quan hệ một-nhiều giữa Teacher và ClassRoom
            modelBuilder.Entity<ClassRoom>()
                .HasOne(c => c.Teacher)
                .WithMany()
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.NoAction); // Tránh cascade



            // Cấu hình quan hệ một-nhiều giữa Categories và Courses
            modelBuilder.Entity<Courses>()
                .HasOne(c => c.Category) // Sửa từ 'CategoryId' thành 'Category'
                .WithMany()
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.NoAction); // Tránh cascade

            // Cấu hình quan hệ một-nhiều giữa ClassRoom và Student (nếu có)
            modelBuilder.Entity<Student>()
                .HasOne(s => s.ClassRoom)
                .WithMany()
                .HasForeignKey(s => s.ClassRoomId)
                .OnDelete(DeleteBehavior.NoAction); // Tránh cascade

        }
    }
}
/* sử dụng xóa mềm (soft delete) với trường DeletedAt để giữ lịch sử dữ liệu,
 thay vì xóa trực tiếp.
Các thực thể được định nghĩa với các thuộc tính bắt buộc (Required) và không bắt buộc phù hợp với logic nghiệp vụ.*/