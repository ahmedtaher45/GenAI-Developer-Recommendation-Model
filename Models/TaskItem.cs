namespace GenAI_Recommendation_Model.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public int DeveloperId { get; set; }
        public Developer Developer { get; set; }

    }
}
