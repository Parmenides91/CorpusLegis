using CorpusLegis.Shared.Dtos.Invitatio;

namespace CorpusLegis.Web.Clients.Invitatio;

public interface IInvitatioClient
{
    // Emisores (Rector /  Magistratus)
    Task<InvitatioDetailsDto?> SendInvitatioAsync(CreateInvitatioDto dto);

    Task<bool> RevokeInvitatioAsync(Guid id);

    Task<List<InvitatioDetailsDto>> GetPendingInvitationesForCivitasAsync(Guid civitasId);



    // Receptores (Civis -> Plebeius)
    Task<List<InvitatioDetailsDto>> GetMyPendingInvitationesAsync();

    Task<bool> AcceptInvitatioAsync(Guid id);

    Task<bool> RejectInvitatioAsync(Guid id);
}
