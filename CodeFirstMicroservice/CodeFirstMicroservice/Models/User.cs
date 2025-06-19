namespace CodeFirstMicroservice.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = "anonymous";
        public string Email { get; set; } = "a@a.com.tr";

        public ICollection<TaskItem>? Tasks { get; set; }
    }
}
