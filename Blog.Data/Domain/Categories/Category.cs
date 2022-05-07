using Blog.Data.Domain.Articles;
using Blog.Data.Domain.Users;

namespace Blog.Data.Domain.Categories
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public bool Approved { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}
