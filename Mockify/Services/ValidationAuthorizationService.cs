using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mockify.Data;
using Mockify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Timers;

namespace Mockify.Services {

    public class ValidationAuthorizationService : IValidateAuthorizationService {

        MockifyDbContext _mc;
        UserManager<ApplicationUser> _um;
        ILogger<ValidationAuthorizationService> _logger;

        public ValidationAuthorizationService(MockifyDbContext mc, UserManager<ApplicationUser> um, ILogger<ValidationAuthorizationService> logger) {
            this._mc = mc;
            this._um = um;
            this._logger = logger;
        }

        public async Task<bool> CheckClientIdExists(string clientid) {
            if (String.IsNullOrWhiteSpace(clientid)) {
                _logger.LogError("no client id was supplied");
                return false;
            }
            // Check exists in DB...
            return _mc.Applications.Any(x => x.ClientId.Equals(clientid));
        }

        public async Task<bool> CheckRedirectURIMatches(string redirectUri, string clientid) {
            if (String.IsNullOrWhiteSpace(redirectUri)) {
                return false;
            }
            // Check is associated with the Client Id....
            RegisteredApplication ra = await _mc.Applications.Include(x => x.RedirectURIs).FirstAsync(x => x.ClientId.Equals(clientid));
            return ra.RedirectURIs.Any(x => x.URI.Equals(redirectUri));
        }

        public async Task<bool> CheckScopesAreValid(string scope) {
            if (String.IsNullOrWhiteSpace(scope)) {
                return true;
            }
            string[] scopes = scope.Split(',');
            foreach(string s in scopes) {
                if(!Models.Spotify.SpotifyScope.IdMap.ContainsKey(s)) {
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> CheckSecretMatchesId(string clientId, string clientSecret) {
            RegisteredApplication ra = await _mc.Applications.Where(x => x.ClientId.Equals(clientId, StringComparison.Ordinal)).FirstAsync();
            if (ra == null) {
                return false;
            }
            // Note: to mitigate brute force attacks, you SHOULD strongly consider applying
            // a key derivation function like PBKDF2 to slow down the secret validation process.
            // You SHOULD also consider using a time-constant comparer to prevent timing attacks.
            return ra.ClientSecret.Equals(clientSecret, StringComparison.Ordinal);
        }

        public async Task<bool> CheckTokenNotRevoked(string user_id, string refreshToken) {
            ApplicationUser user = await _um.Users.Where(x => x.Id.Equals(user_id)).Include(x => x.UserApplicationTokens).FirstOrDefaultAsync();
            if(user == null) {
                return false; //error, no user was found or database encountered an error
            }
            return user.UserApplicationTokens.Any(x => x.TokenValue.Equals(refreshToken));
        }
    }


}
