using System.Security.Claims;

namespace CorpusLegis.API.Infrastructure.Security;

public interface IUserProvisioningService
{
    //Task ProvisionUserAsync(string email, string password, CancellationToken cancellationToken);

    public Task ProvisionUserAsync(ClaimsPrincipal user, CancellationToken cancellationToken);

}
