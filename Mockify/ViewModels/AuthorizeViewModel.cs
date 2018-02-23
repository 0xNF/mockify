using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using Mockify.Models.Spotify;

namespace Mockify.ViewModels {

    public class AuthorizeViewModel {

        public string AppName { get; set; }
        public string ClientId { get; set; }
        public string RedirectUri { get; set; }
        public string TokenType { get; set; }
        public string Description { get; set; }
        public IList<SpotifyScope> Scopes { get; set; } = new List<SpotifyScope>();
        public string State { get; set; }

        // For handling logins/redirects - this is not part of the Application requesting authorization.
        public string AuthUrl {
            get {
                string authurl = $"/authorize/";
                string client = $"?client_id={ClientId}";
                string redirect = String.IsNullOrWhiteSpace(RedirectUri) ? "" : $"&redirect_uri={RedirectUri}";
                string state = String.IsNullOrWhiteSpace(State) ? "" : $"&state={State}";
                string token = $"&response_type={TokenType}";
                string scopes = !Scopes.Any() ? "" : $"&scope={String.Join(',', Scopes.Select(x=>x.Name))}";
                return $"{authurl}{client}{redirect}{scopes}{token}{state}";
            }
        }
        public string LoginRedirect {
            get {
                string loginurl = $"/login?returnUrl={AuthUrl}";
                return loginurl;
            }
        }

        public AuthorizeViewModel() {
            // empty
        }
    }

    public class AuthorizeErrorViewModel {
        [Display(Name = "Error")]
        public string Error { get; set; }

        [Display(Name = "Error Description")]
        public string ErrorDescription { get; set; }
    }
}
