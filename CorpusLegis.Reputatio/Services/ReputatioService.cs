using CorpusLegis.Reputatio.Domain;
using MongoDB.Driver;

namespace CorpusLegis.Reputatio.Services;

public class ReputatioService : IReputatioService
{

    private readonly IMongoCollection<CivisReputatio> _collection;

    public ReputatioService(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("mongo-db-corpuslegis");
        _collection = database.GetCollection<CivisReputatio>("civis_reputatio");
    }


    public async Task<CivisReputatio?> GetByCivisIdAsync(Guid civisId, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(cr => cr.CivisId == civisId)
            .FirstOrDefaultAsync(cancellationToken);
    }

}
