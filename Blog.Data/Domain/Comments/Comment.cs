namespace Blog.Data.Domain.Comments
{
    public class Comment : BaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Content { get; set; }
        public List<string> Article { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public bool Approved { get; set; }

    }
}
