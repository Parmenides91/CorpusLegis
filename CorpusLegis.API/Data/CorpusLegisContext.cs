using Microsoft.EntityFrameworkCore;

namespace CorpusLegis.API.Data;

public class CorpusLegisContext(DbContextOptions<CorpusLegisContext> options)
    : DbContext(options)
{
    public DbSet<Domain.Rogatio> Rogatios { get; set; } = null!; // TODO: he tenido que poner el null! o me da error.
}
