
namespace Net_Core_API_intro.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Task> Tasks { get; set; }
    }
}
