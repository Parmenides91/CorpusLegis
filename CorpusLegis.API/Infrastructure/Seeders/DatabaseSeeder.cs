using CorpusLegis.API.Data;
using CorpusLegis.API.Domain;

namespace CorpusLegis.API.Infrastructure.Seeders;

public static class DatabaseSeeder
{

    public static void Seed(CorpusLegisContext db)
    {
        if (db.Rogationes.Any() || db.Cives.Any() || db.Civitates.Any())
        {
            return;
        }

        var defaultCivisIds = new[] { Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e"), Guid.Parse("11111111-1111-1111-1111-111111111111"), Guid.Parse("22222222-1111-1111-1111-111111111111"), Guid.Parse("33333333-1111-1111-1111-111111111111"), Guid.Parse("44444444-1111-1111-1111-111111111111"), Guid.Parse("e5991ab1-2e75-4bac-884d-deb11ffadfb5") };
        var defaultCivitasIds = new[] { Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7"), Guid.Parse("11111111-2222-1111-1111-111111111111"), Guid.Parse("22222222-2222-1111-1111-111111111111"), Guid.Parse("33333333-2222-1111-1111-111111111111"), Guid.Parse("44444444-2222-1111-1111-111111111111"), Guid.Parse("55555555-2222-1111-1111-111111111111") };

        var defaultCives = new List<Civis>
        {
            new Civis { Id = defaultCivisIds[0], Name = "Sempronio" },
            new Civis { Id = defaultCivisIds[1], Name = "Tulio" },
            new Civis { Id = defaultCivisIds[2], Name = "Escipion" },
            new Civis { Id = defaultCivisIds[3], Name = "Sila" },
            new Civis { Id = defaultCivisIds[4], Name = "Pompeyo" },
            new Civis { Id = defaultCivisIds[5], Name = "Craso" }
        };

        var defaultCivitates = new List<Civitas>
        {
            new Civitas { Id = defaultCivitasIds[0], Name = "Roma" },
            new Civitas { Id = defaultCivitasIds[1], Name = "Cartago" },
            new Civitas { Id = defaultCivitasIds[2], Name = "Esparta" },
            new Civitas { Id = defaultCivitasIds[3], Name = "Corinto" },
            new Civitas { Id = defaultCivitasIds[4], Name = "Atenas" },
            new Civitas { Id = defaultCivitasIds[5], Name = "Alejandria" }
        };

        defaultCives[0].Civitates.Add(defaultCivitates[0]);
        defaultCives[1].Civitates.Add(defaultCivitates[1]);
        //defaultCivitates[2].Cives.Add(defaultCives[2]);
        //defaultCivitates[3].Cives.Add(defaultCives[2]);
        //defaultCivitates[4].Cives.Add(defaultCives[2]);
        //defaultCives[3].Civitates.Add(defaultCivitates[5]);

        //defaultCives[0].Civitates.Add(defaultCivitates[3]);
        //defaultCives[0].Civitates.Add(defaultCivitates[4]);
        //defaultCives[0].Civitates.Add(defaultCivitates[5]);
        //defaultCivitates[3].Cives.Add(defaultCives[0]);
        //defaultCivitates[4].Cives.Add(defaultCives[0]);
        //defaultCivitates[5].Cives.Add(defaultCives[0]);

        defaultCives[5].Civitates.Add(defaultCivitates[0]);
        defaultCives[5].Civitates.Add(defaultCivitates[1]);
        defaultCives[5].Civitates.Add(defaultCivitates[2]);
        defaultCives[5].Civitates.Add(defaultCivitates[3]);


        var rogatio = new Rogatio
        {
            Id = Guid.NewGuid(),
            Title = "Rogatio primigenia",
            Content = "Contenido fundacional de la Civitas.",
            CreatedAt = DateTime.UtcNow,
            CivisId = defaultCives[0].Id,
            CivitasId = defaultCivitates[0].Id
        };

        db.Cives.AddRange(defaultCives);
        db.Civitates.AddRange(defaultCivitates);
        db.Rogationes.Add(rogatio);

        db.SaveChanges();
    }

}
