using Microsoft.EntityFrameworkCore;
using CorpusLegis.API.Domain;

namespace CorpusLegis.API.Data;

public class CorpusLegisContext(DbContextOptions<CorpusLegisContext> options)
    : DbContext(options)
{

    public DbSet<Civitas> Civitates { get; set; }

    public DbSet<Civis> Cives { get; set; }

    public DbSet<Rogatio> Rogationes { get; set; } = null!;

    public DbSet<Lex> Leges { get; set; }

    public DbSet<Sententia> Sententiae { get; set; }

    public DbSet<Suffragium> Suffragia { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);



        // Forzamos a que nuestros Enums se guarden como string en la BBDD en lugar de como enteros.
        modelBuilder.Entity<Rogatio>()
            .Property(r => r.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Suffragium>()
            .Property(s => s.Votum)
            .HasConversion<string>();



        // Un Civis sólo puede votar una vez por Rogatio.
        modelBuilder.Entity<Suffragium>()
            .HasIndex(s => new { s.RogatioId, s.CivisId })
            .IsUnique();



        // Al borrar una Civitas no se borrarán sus Cives (sólo las relaciones establecidas en la tabla intermedia).
        modelBuilder.Entity<Civitas>()
            .HasMany(c => c.Cives)
            .WithMany(c => c.Civitates);



        // Si se elimina a un Civis, sus votos y comentarios deberían mantenerse por registro histórico (se pone a null o se usa Soft Delete)
        // Por ahora, usamos Restrict para obligar a no borrar usuarios con comentarios o votos.
        modelBuilder.Entity<Sententia>()
            .HasOne(s => s.Civis)
            .WithMany()
            .HasForeignKey(s => s.CivisId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Suffragium>()
            .HasOne(s => s.Civis)
            .WithMany()
            .HasForeignKey(s => s.CivisId)
            .OnDelete(DeleteBehavior.Restrict);



        // Evitar bucles de borrado en cascada para Lex.
        modelBuilder.Entity<Lex>()
            .HasOne(l => l.OriginRogatio)
            .WithMany() // Una Rogatio podría teóricamente originar múltiples versiones de Lex, o ninguna.
            .HasForeignKey(l => l.OriginRogatioId)
            .OnDelete(DeleteBehavior.Restrict); // obligatorio o llevamos un bucle de borrado en cascada entre Rogatio y Lex.

        modelBuilder.Entity<Lex>()
            .HasOne(l => l.Civitas)
            .WithMany(c => c.Leges)
            .HasForeignKey(l => l.CivitasId)
            .OnDelete(DeleteBehavior.Restrict);
    }

}
