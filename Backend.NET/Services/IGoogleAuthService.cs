using System.Collections.Generic;
using System.Threading.Tasks;

namespace FleetManagementSystem.Api.Services;

public interface IGoogleAuthService
{
    Task<Dictionary<string, object>> VerifyGoogleTokenAndGetJwtAsync(string accessToken);
}
