namespace GenAI_Recommendation_Model.Models
{
    public class Developer
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
        public int TaskCount { get; set; } = 0;

        public List<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
