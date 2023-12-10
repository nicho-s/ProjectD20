namespace GameForum.Models
{
    public class Topic
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreatingTime { get; set; }
        public IEnumerable<Review> Reviews { get; set; }
        public ApplicationUser? User { get; set; }

        public Topic()
        {
            Reviews = new List<Review>();
        }
        public bool IsHidden { get; set; }
    }
}
