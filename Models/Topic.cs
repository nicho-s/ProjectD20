namespace Lab4_5.Models
{
    public class Topic
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreatingTime { get; set; }
        public List<Review>? Reviews { get; set; }
    }
}
