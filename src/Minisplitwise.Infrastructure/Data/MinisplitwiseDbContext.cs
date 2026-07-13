using Microsoft.EntityFrameworkCore;
using Minisplitwise.Domain.Entities;

namespace Minisplitwise.Infrastructure.Data;

public class MinisplitwiseDbContext(DbContextOptions<MinisplitwiseDbContext> options) : DbContext(options)
{
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<Expense> Expenses => Set<Expense>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MinisplitwiseDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}