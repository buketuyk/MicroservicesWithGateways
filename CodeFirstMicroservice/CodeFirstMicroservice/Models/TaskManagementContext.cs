using Microsoft.EntityFrameworkCore;

namespace CodeFirstMicroservice.Models
{
    public class TaskManagementContext : DbContext
    {
        public TaskManagementContext(DbContextOptions<TaskManagementContext> contextOptions) : base(contextOptions)
        {
        }

        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<TaskStatus> TaskStatuses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TaskStatus>().HasData(
                new TaskStatus { Id = 1, Name = "Backlog" },
                new TaskStatus { Id = 2, Name = "Todo" },
                new TaskStatus { Id = 3, Name = "In Progress" },
                new TaskStatus { Id = 4, Name = "Test" },
                new TaskStatus { Id = 5, Name = "Done" }
            );

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Career" },
                new Category { Id = 2, Name = "Shopping" },
                new Category { Id = 3, Name = "Personal Development" },
                new Category { Id = 4, Name = "Health" },
                new Category { Id = 5, Name = "Finance" },
                new Category { Id = 6, Name = "Home" },
                new Category { Id = 7, Name = "Hobbies" },
                new Category { Id = 8, Name = "Education" },
                new Category { Id = 9, Name = "Family" },
                new Category { Id = 10, Name = "Other" }
            );
        }
    }
}
