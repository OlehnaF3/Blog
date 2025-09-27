using ProjBlog.Repository.Interfaces;

namespace ProjBlog.Repository
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        IUserRepository Users { get; }
        IArticleRepository Articles { get; }
        ITagRepository Tags { get; }
        ICommentRepository Comments { get; }
        IRoleUserRepository RoleUser { get; }
        IRoleRepository Role { get; }
        IArticleTagRepository ArticleTag { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
