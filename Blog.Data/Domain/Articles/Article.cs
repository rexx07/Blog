using Blog.Data.Domain.Categories;
using Blog.Data.Domain.Users;

namespace Blog.Data.Domain.Articles
{
    public class Article : BaseEntity
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Image { get; set; }
        public string ImageCredit { get; set; }
        public List<string> Tags { get; set; }
        public string Body { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public bool ArticleStatus { get; set; }
        public int Views { get; set; }
        public int CountWords { get; set; }
        public int ReadTime { get; set; }
        public bool IsDeleted { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
        public List<Category> Categories { get; set; }
    }
}
