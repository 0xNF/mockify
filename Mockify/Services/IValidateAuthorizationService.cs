using System.Security.Claims;
using System.Threading.Tasks;

namespace Mockify.Services {
    public interface IValidateAuthorizationService {

        Task<bool> CheckClientIdExists(string clientid);
        Task<bool> CheckRedirectURIMatches(string redirectUri, string clientid);
        Task<bool> CheckScopesAreValid(string scope);
        Task<bool> CheckSecretMatchesId(string clientId, string clientSecret);
    }
}