using CorpusLegis.Shared.Dtos.Invitatio;

namespace CorpusLegis.Web.Clients.Invitatio;

public interface IInvitatioClient
{
    // Emisores (Rector /  Magistratus)
    public Task<InvitatioDetailsDto?> SendInvitatioAsync(CreateInvitatioDto dto);

    public Task<bool> RevokeInvitatioAsync(Guid id);

    public Task<List<InvitatioDetailsDto>> GetPendingInvitationesForCivitasAsync(Guid civitasId);



    // Receptores (Civis -> Plebeius)
    public Task<List<InvitatioDetailsDto>> GetMyPendingInvitationesAsync();

    public Task<bool> AcceptInvitatioAsync(Guid id);

    public Task<bool> RejectInvitatioAsync(Guid id);
}
