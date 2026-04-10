using CorpusLegis.Shared.Dtos.Invitatio;

namespace CorpusLegis.API.Services;

public interface IInvitatioService
{
    // Emisores (Rector /  Magistratus)
    public Task<InvitatioDetailsDto> SendAsync(CreateInvitatioDto dto);
    public Task<bool> RevokeAsync(Guid invitatioId);
    public Task<List<InvitatioDetailsDto>> GetPendingForCivitasAsync(Guid civitasId);


    // Receptores (Civis -> Plebeius)
    public Task<List<InvitatioDetailsDto>> GetPendingInvitationesForCivisAsync(Guid civisId);
    public Task<List<InvitatioDetailsDto>> GetPendingInvitationesForCurrentCivisAsync();
    public Task<bool> AcceptAsync(Guid invitatioId);
    public Task<bool> RejectAsync(Guid invitatioId);
}
