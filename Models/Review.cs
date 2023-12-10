namespace GameForum.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime CreatingTime { get; set; }
        public int TopicId { get; set; }
        public Topic Topic { get; set; }
        public  ApplicationUser User { get; set; }
        public bool IsHidden { get; set; }
    }
}