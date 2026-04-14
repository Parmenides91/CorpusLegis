using CorpusLegis.Shared.Dtos.Lex;

namespace CorpusLegis.Web.Clients.Lex;

public interface ILexClient
{
    public Task<LexDetailsDto?> GetLexByIdAsync(Guid id); // Método para obtener una lex por su ID.

    public Task<List<LexSummaryDto>> GetLegesAsync(); // Método para obtener la lista de leges.

    public Task<List<LexSummaryDto>> GetLegesByCurrentCivisAsync(); // Método para obtener la lista de leges a las que pertenece el Civis actual.

    public Task<List<LexSummaryDto>> GetLegesByCivisIdAsync(Guid civisId); // Método para obtener las leges a las que pertenece un Civis.


}
