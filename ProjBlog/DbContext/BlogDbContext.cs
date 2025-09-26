
namespace ProjBlog.DbContext
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    using ProjBlog.Models;
    using System.Reflection;
    public class BlogDbContext : DbContext
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ArticleTag> ArticleTag { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<RoleUser> RoleUser { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Конфигурация пользователя
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
                entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(u => u.IsActive).HasDefaultValue(true);

                entity.HasMany(u => u.Articles).WithOne(a => a.Author)
                .HasForeignKey(a => a.AuthorId).OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.Comments).WithOne(c => c.User)
                .HasForeignKey(c => c.UserId);

                entity.HasMany(u => u.RoleUser).WithOne(ru => ru.User)
                .HasForeignKey(ru => ru.UserId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(u => u.Username).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();
            });

            // Конфигурация статьи
            modelBuilder.Entity<Article>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Title).IsRequired().HasMaxLength(255);
                entity.Property(a => a.Content).IsRequired();
                entity.Property(a => a.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(a => a.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(a => a.IsPublished).HasDefaultValue(false);

                //entity.HasOne(a => a.Author)
                //      .WithMany(u => u.Articles)
                //      .HasForeignKey(a => a.AuthorId)
                //      .OnDelete(DeleteBehavior.Cascade);
            });

            // Конфигурация тега
            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Name).IsRequired().HasMaxLength(50);
                entity.Property(t => t.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(t => t.Name).IsUnique();

                entity.HasMany(t => t.ArticleTags)
                .WithMany(ar => ar.Tags);

            });

            // Конфигурация связи статья-тег
            modelBuilder.Entity<ArticleTag>(entity =>
            {
                entity.HasKey(at => at.Id);
                entity.Property(at => at.ArticleId);
                entity.Property(at => at.TagId);
                entity.Property(at => at.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(at => at.Article)
                .WithMany(a => a.ArticleTags)
                .HasForeignKey(at => at.ArticleId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            // Конфигурация комментария
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Content).IsRequired();
                entity.Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(c => c.IsApproved).HasDefaultValue(false);

                entity.HasOne(c => c.Article)
                      .WithMany(a => a.Comments)
                      .HasForeignKey(c => c.ArticleId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.User)
                      .WithMany(u => u.Comments)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.ParentComment)
                      .WithMany(c => c.ChildComments)
                      .HasForeignKey(c => c.ParentCommentId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Role>(entity =>
            {

                entity.HasKey(r => r.Id);
                entity.Property(r => r.Name);
                entity.Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<RoleUser>(entity =>
            {
                entity.HasKey(ru => ru.Id);
                entity.Property(ru => ru.RolesId);
                entity.Property(ru => ru.UserId);
                entity.Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(ur => ur.Role)
                .WithMany(r => r.RoleUser)
                .HasForeignKey(ur => ur.RolesId);

                entity.HasOne(ur => ur.User)
                .WithMany(u => u.RoleUser)
                .HasForeignKey(ur => ur.UserId);


            });

            // Применение конфигураций из сборки
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Автоматическое обновление UpdatedAt для статей
            var entries = ChangeTracker.Entries<Article>()
                .Where(e => e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
