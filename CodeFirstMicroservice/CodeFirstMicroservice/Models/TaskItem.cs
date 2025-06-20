namespace CodeFirstMicroservice.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int AssignedUserId { get; set; }
        public User? AssignedUser { get; set; }
        public Status Status { get; set; } = new Status { Id = 1, Name = "Backlog" };
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
