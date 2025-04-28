

using Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data.Contexts;

public class DataContext : IdentityDbContext<MemberEntity, ApplicationRole, string>
{
    public DataContext(DbContextOptions<DataContext> options)
    : base(options)
    {
    }
    public virtual DbSet<MemberProfileEntity> MemberProfile { get; set; }
    public DbSet<ProjectEntity> Projects { get; set; } = null!;
    public DbSet<ChatMessageEntity> ChatMessages { get; set; } = null!;
    public DbSet<NotificationEntity> Notifications { get; set; }

}

