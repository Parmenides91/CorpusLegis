using CorpusLegis.Shared.Enums;

namespace CorpusLegis.API.Domain;

public class Invitatio
{
    public Guid Id { get; set; }

    public Guid CivitasId { get; set; }
    public Civitas Civitas { get; set; } = null!;

    public Guid InviterId { get; set; } // quién invita
    public Civis Inviter { get; set; } = null!;

    public Guid InviteeId { get; set; } // quién es invitado
    public Civis Invitee { get; set; } = null!;


    public string Token { get; set; } = string.Empty;
    public InvitationisStatus Status { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RespondedAt { get; set; }
}
