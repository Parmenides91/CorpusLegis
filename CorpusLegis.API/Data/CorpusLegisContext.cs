using Microsoft.EntityFrameworkCore;
using CorpusLegis.API.Domain;
using MassTransit;

namespace CorpusLegis.API.Data;

public class CorpusLegisContext(DbContextOptions<CorpusLegisContext> options)
    : DbContext(options)
{

    public DbSet<Civitas> Civitates { get; set; }

    public DbSet<Civis> Cives { get; set; }

    public DbSet<CivitasSodalis> CivitasSodales { get; set; } // La defino explícitamente, aunque la matice luego en el OnModelCreating.

    public DbSet<Rogatio> Rogationes { get; set; } = null!;

    public DbSet<Lex> Leges { get; set; }

    public DbSet<Sententia> Sententiae { get; set; }

    public DbSet<Suffragium> Suffragia { get; set; }

    public DbSet<Invitatio> Invitationes { get; set; }



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

        modelBuilder.Entity<Civitas>()
            .Property(c => c.Visibility)
            .HasConversion<string>();

        modelBuilder.Entity<Invitatio>()
            .Property(i => i.Status)
            .HasConversion<string>();



        // Un Civis sólo puede votar una vez por Rogatio.
        modelBuilder.Entity<Suffragium>()
            .HasIndex(s => new { s.RogatioId, s.CivisId })
            .IsUnique();



        // Al borrar una Civitas no se borrarán sus Cives (sólo las relaciones establecidas en la tabla intermedia).
        //modelBuilder.Entity<Civitas>()
        //    .HasMany(c => c.Cives)
        //    .WithMany(c => c.Civitates);
        // TODO: borrar esta definición anterior, porque con lo posterior ya es suficiente.
        // La relación entre Civitas y Civis se maneja a través de la tabla intermedia CivitasSodalis.
        // Al borrar una Civitas no se borrarán sus Cives
        modelBuilder.Entity<Civitas>()
            .HasMany(c => c.Cives)
            .WithMany(c => c.Civitates)
            .UsingEntity<CivitasSodalis>(
                j => j.HasOne(cs => cs.Civis)
                .WithMany(c => c.CivitasSodales)
                .HasForeignKey(cs => cs.CivisId)
                .OnDelete(DeleteBehavior.Restrict), // No se puede borrar un Civis que sea miembro de una Civitas.
                j => j.HasOne(cs => cs.Civitas)
                .WithMany(c => c.Sodales)
                .HasForeignKey(cs => cs.CivitasId)
                .OnDelete(DeleteBehavior.Cascade), // Si se borra una Civitas, se eliminan sus relaciones con los Cives, pero no los Cives en sí mismos.
                j =>
                {
                    j.HasKey(cs => new { cs.CivitasId, cs.CivisId }); // clave primaria compuesta por las dos claves foráneas.
                    j.Property(cs => cs.Role).HasConversion<string>(); // el rol del Civis en la Civitas se guarda como string.
                    j.ToTable("CivitasSodales"); // nombre explícito para la tabla intermedia.
                }
            );



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



        // Los parámetros que definen los resultados de las votaciones de las Rogationes necesitan tener más precisión.
        modelBuilder.Entity<Rogatio>()
            .Property(r => r.RequiredQuorum)
            .HasPrecision(5, 4); // Admite valores desde -9'9999 hasta 9'9999.

        modelBuilder.Entity<Rogatio>()
            .Property(r => r.RequiredMajority)
            .HasPrecision(5, 4);



        // Configuración necesaria para que funcione el Outbox de MassTransit con EF Core.
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();



        // Configuración necesaria para Invitatio.
        modelBuilder.Entity<Invitatio>()
            .HasOne(i => i.Civitas)
            .WithMany(c => c.Invitationes)
            .HasForeignKey(i => i.CivitasId)
            .OnDelete(DeleteBehavior.Cascade); // Si se borra una Civitas, se borran sus invitaciones.

        modelBuilder.Entity<Invitatio>()
            .HasOne(i => i.Inviter)
            .WithMany(c => c.InvitationesEmissae)
            .HasForeignKey(i => i.InviterId)
            .OnDelete(DeleteBehavior.Restrict); // No se puede borrar un Civis que haya emitido invitaciones.

        modelBuilder.Entity<Invitatio>()
            .HasOne(i => i.Invitee)
            .WithMany(c => c.InvitationesAcceptae)
            .HasForeignKey(i => i.InviteeId)
            .OnDelete(DeleteBehavior.Restrict); // No se puede borrar un Civis que haya sido invitado a una Civitas.
    }

}
