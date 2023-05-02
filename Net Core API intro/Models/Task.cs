namespace Net_Core_API_intro.Models
{
    public class Task
    {
        public int pk_task_id { get; set; }
        public string task_detail { get; set; }
        public string fk_user_id { get; set; }
    }
}
