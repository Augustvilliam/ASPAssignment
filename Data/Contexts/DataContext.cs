

using System.Collections.Generic;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Context;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<ProjectEntity> Projects { get; set; }
    public DbSet<UserProfileEntity> UserProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProjectEntity>()
            .Property(p => p.Budget)
            .HasPrecision(18, 2);

        base.OnModelCreating(modelBuilder);
    }
}
