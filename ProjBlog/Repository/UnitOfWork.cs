using Microsoft.EntityFrameworkCore.Storage;
using ProjBlogDb.DbContext;
using ProjBlog.Repository.Interfaces;

namespace ProjBlog.Repository
{

    public class UnitOfWork : IUnitOfWork
    {
        private readonly BlogDbContext _context;
        private IDbContextTransaction? _transaction;

        public IUserRepository Users { get; }
        public IArticleRepository Articles { get; }
        public ITagRepository Tags { get; }
        public ICommentRepository Comments { get; }
        public IRoleUserRepository RoleUser { get; }
        public IRoleRepository Role { get; }
        public IArticleTagRepository ArticleTag { get; }

        public UnitOfWork(BlogDbContext context)
        {
            _context = context;

            Users = new UserRepository(context);
            Articles = new ArticleRepository(context);
            Tags = new TagRepository(context);
            Comments = new CommentRepository(context);
            RoleUser = new RoleUserRepository(context);
            Role = new RoleRepository(context);
            ArticleTag = new ArticleTagRepository(context);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
            }
            await _context.DisposeAsync();
        }
    }
}