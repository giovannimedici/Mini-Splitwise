using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Minisplitwise.Infrastructure.Data;

namespace MiniSplitwise.Infrastructure.Data;

public class MinisplitwiseDbContextFactory
    : IDesignTimeDbContextFactory<MinisplitwiseDbContext>
{
    public MinisplitwiseDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder =
            new DbContextOptionsBuilder<MinisplitwiseDbContext>();

        var connStr =
            "Data Source=Minisplitwise.db";

        optionsBuilder
            .UseSqlite(connStr)
            .UseSnakeCaseNamingConvention();

        return new MinisplitwiseDbContext(optionsBuilder.Options);
    }
}