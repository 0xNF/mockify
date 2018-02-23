using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mockify.Data;
using Mockify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Timers;

namespace Mockify.Services {

    public class AuthRequestClientInfo {
        public string UserId;
        public string ClientId;

        public AuthRequestClientInfo(string userid, string clientid) {
            this.UserId = userid;
            this.ClientId = clientid;
        }

        public override bool Equals(object obj) {
            if (obj is AuthRequestClientInfo that) {
                return (that.UserId.Equals(this.UserId) && that.ClientId.Equals(this.ClientId));
            }
            return false;
        }

        public override int GetHashCode() {
            var hashCode = -96061894;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(UserId);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ClientId);
            return hashCode;
        }
    }
    public class AuthRequestValidationInfo {
        public string Code;
        public string RedirectURI;

        public AuthRequestValidationInfo(string code, string redirect) {
            this.Code = code;
            this.RedirectURI = redirect;
        }


        public override bool Equals(object obj) {
            if (obj is AuthRequestValidationInfo that) {
                return (that.Code.Equals(this.Code) && that.RedirectURI.Equals(this.RedirectURI));
            }
            return false;
        }

        public override int GetHashCode() {
            var hashCode = 1171506621;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Code);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(RedirectURI);
            return hashCode;
        }
    }

    public class ValidationAuthorizationService : IValidateAuthorizationService {

        MockifyDbContext _mc;
        UserManager<ApplicationUser> _um;

        public ValidationAuthorizationService(MockifyDbContext mc, UserManager<ApplicationUser> um) {
            this._mc = mc;
            this._um = um;
        }

        public async Task<bool> CheckClientIdExists(string clientid) {
            if (String.IsNullOrWhiteSpace(clientid)) {
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


    }


}
