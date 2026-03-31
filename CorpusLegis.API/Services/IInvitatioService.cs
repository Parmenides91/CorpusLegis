using CorpusLegis.Shared.Dtos.Invitatio;

namespace CorpusLegis.API.Services;

public interface IInvitatioService
{
    // Emisores (Rector /  Magistratus)
    Task<InvitatioDetailsDto> SendAsync(CreateInvitatioDto dto);
    Task<bool> RevokeAsync(Guid invitatioId);
    Task<List<InvitatioDetailsDto>> GetPendingForCivitasAsync(Guid civitasId);


    // Receptores (Civis -> Plebeius)
    Task<List<InvitatioDetailsDto>> GetPendingInvitationesForCivisAsync(Guid civisId);
    Task<List<InvitatioDetailsDto>> GetPendingInvitationesForCurrentCivisAsync();
    Task<bool> AcceptAsync(Guid invitatioId);
    Task<bool> RejectAsync(Guid invitatioId);
}
