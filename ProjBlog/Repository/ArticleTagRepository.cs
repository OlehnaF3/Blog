using ProjBlog.DbContext;
using ProjBlog.Models;
using ProjBlog.Repository.Interfaces;

namespace ProjBlog.Repository
{
    public class ArticleTagRepository : Repository<ArticleTag>, IArticleTagRepository
    {
        public ArticleTagRepository(BlogDbContext context) : base(context)
        {
        }
    }
}
