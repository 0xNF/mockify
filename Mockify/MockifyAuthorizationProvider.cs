using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using AspNet.Security.OpenIdConnect.Server;
using AspNet.Security.OpenIdConnect.Primitives;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Mockify.Data;
using Mockify.Models;
using Mockify.Services;
using Mockify.Models.Spotify;
using AspNet.Security.OpenIdConnect.Extensions;


namespace Mockify {
    public sealed class MockifyAuthorizationProvider : OpenIdConnectServerProvider {

        IValidateAuthorizationService authservice;


    public override Task MatchEndpoint(MatchEndpointContext context) {
            // Note: by default, the OIDC server middleware only handles authorization requests made to
            // AuthorizationEndpointPath. This handler uses a more relaxed policy that allows extracting
            // authorization requests received at /connect/authorize/accept and /connect/authorize/deny.
            if (context.Options.AuthorizationEndpointPath.HasValue &&
                context.Request.Path.Value.StartsWith(context.Options.AuthorizationEndpointPath)) {
                context.MatchAuthorizationEndpoint();
            }
            return Task.CompletedTask;
        }

        #region Validate
        private async Task _validateAuthorizationCodeFlow(ValidateAuthorizationRequestContext context) {
            authservice = context.HttpContext.RequestServices.GetRequiredService<IValidateAuthorizationService>();
            if (authservice == null) {
                context.Reject(OpenIdConnectConstants.Errors.ServerError, "Failed to validate this authorization request");
                return;
            }
            if (string.IsNullOrEmpty(context.RedirectUri)) {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidRequest,
                    description: "The required redirect_uri parameter was missing.");
                return;
            }


            if (!(await authservice.CheckClientIdExists(context.ClientId))) {
                context.Reject(error: OpenIdConnectConstants.Errors.InvalidClient, description: "Supplied Client Id was not a valid application Client Id.");
                return;
            }
            if (!(await authservice.CheckRedirectURIMatches(context.RedirectUri, context.ClientId))) {
                context.Reject(error: OpenIdConnectConstants.Errors.InvalidRequest, description: "Supplied Redirect URI was not valid for the supplied Client Id.");
                return;
            }
            if (!(await authservice.CheckScopesAreValid(context.Request.Scope))) {
                context.Reject(error: OpenIdConnectConstants.Errors.InvalidRequest, description: "Supplied scopes were was not valid Spotify Scopes.");
                return;
            }

            context.Validate();
        }
        private async Task _validateAuthorizationTokenRequest(ValidateTokenRequestContext context) {
            // TODO increment user rate limit (nb this increment only happens in Validate)
            // TODO check user is within their rate limit for the requested client_id
            if (String.IsNullOrWhiteSpace(context.ClientId)) {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidClient,
                    description: "Missing credentials: ensure that you specified a client_id.");
                return;
            }
            else if (String.IsNullOrWhiteSpace(context.ClientSecret)) {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidClient,
                    description: "Missing credentials: ensure that you specified a client_secret.");
                return;
            }
            if (String.IsNullOrWhiteSpace(context.Request.Code)) {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidRequest,
                    description: "Missing credentials: ensure that you specified a value for code.");
                return;
            }
            context.Validate();
        }
        private async Task _validateRefreshTokenRequest(ValidateTokenRequestContext context) {
            authservice = context.HttpContext.RequestServices.GetRequiredService<IValidateAuthorizationService>();
            if (authservice == null) {
                context.Reject(OpenIdConnectConstants.Errors.ServerError, "Failed to validate this authorization request");
                return;
            }
            IRateLimitService rls = context.HttpContext.RequestServices.GetRequiredService<IRateLimitService>();
            if(rls == null) {
                context.Reject(OpenIdConnectConstants.Errors.ServerError, "Failed to validate this authorization request");
                return;
            }

            // TODO increment user rate limit (nb this increment only happens in Validate)
            if (String.IsNullOrWhiteSpace(context.Request.RefreshToken)) {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidRequest,
                    description: "Missing parameter: ensure that you specified a refresh_token.");
            }

            AuthenticateResult ar = await context.HttpContext.AuthenticateAsync(OpenIdConnectServerDefaults.AuthenticationScheme);
            AuthenticationTicket at = ar.Ticket;
            if(ar == null) {
                context.Reject();
                return;
            } else if (ar.Principal == null) {
                context.Reject(error: OpenIdConnectConstants.Errors.InvalidRequest, description: "Supplied refresh token was not valid");
                return;
            } else if (ar.Principal.Identity == null) {
                context.Reject();
                return;
            } else if(!ar.Principal.Identity.IsAuthenticated) {
                context.Reject();
                return;
            }

            /** here we do the legwork of validating that: 
             * the client application still exists,
             * the supplied secret still matches, 
             * the user's refresh and access tokens haven't been revoked,
             * the user isn't rate limited
             */
            List<Claim> claims = ar.Principal.Claims.ToList();
            var gtype = claims.Find(x => x.Type == "grant_type");
            var userid = claims.Find(x => x.Type == "sub");
            var clientSecret = context.ClientSecret;
            if (gtype == null || string.IsNullOrWhiteSpace(gtype.Value)) {
                context.Reject();
            }
            if (userid == null || string.IsNullOrWhiteSpace(userid.Value)) {
                context.Reject();
            }
            if(!await authservice.CheckClientIdExists(context.ClientId)) {
                context.Reject(OpenIdConnectConstants.Errors.InvalidClient, "The supplied client id no longer exists.");
                return;
            } else if(!await authservice.CheckSecretMatchesId(context.ClientId, context.ClientSecret)) {
                context.Reject(OpenIdConnectConstants.Errors.InvalidClient, "The supplied client secret is no longer valid.");
                return;
            } else if (!await authservice.CheckTokenNotRevoked(gtype.Value, context.Request.RefreshToken)) {
                context.Reject(OpenIdConnectConstants.Errors.AccessDenied, "The supplied token has been revoked. Please re-authenticate.");
            }
            else if (!(await rls.UserWithinRateLimit(userid.Value)) && !(await rls.UserWithinAppRateLimit(context.ClientId, userid.Value, gtype.Value))) {
                context.Reject(RateLimits.RateLimitExceededError, RateLimits.RateLimitExceededErrorDescription);
                return;
            }
            await rls.IncrementUserAPICallCount(userid.Value, context.ClientId);
            context.Validate();
        }

        private async Task _validateClientCredentialsTokenRequest(ValidateTokenRequestContext context) {
            // TODO increment user rate limit (nb this increment only happens in Validate)
            authservice = context.HttpContext.RequestServices.GetRequiredService<IValidateAuthorizationService>();
            if (authservice == null) {
                context.Reject(OpenIdConnectConstants.Errors.ServerError, "Failed to validate this authorization request");
                return;
            }
            if (String.IsNullOrWhiteSpace(context.ClientId)) {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidClient,
                    description: "Missing credentials: ensure that you specified a client_id.");
                return;
            } else if (String.IsNullOrWhiteSpace(context.ClientSecret)) {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidClient,
                    description: "Missing credentials: ensure that you specified a client_secret.");
                return;
            } else if (!(await authservice.CheckClientIdExists(context.ClientId))) {
                context.Reject(error: OpenIdConnectConstants.Errors.InvalidClient, description: "Supplied Client Id was not a valid application Client Id.");
                return;
            } else  if (!(await authservice.CheckSecretMatchesId(context.ClientId, context.ClientSecret))) {
                context.Reject(error: OpenIdConnectConstants.Errors.InvalidRequest, description: "Supplied Secret was not correct for the Client Id.");
                return;
            }
            context.Validate();
        }

        public override async Task ValidateTokenRequest(ValidateTokenRequestContext context) {

            if (context.Request.IsRefreshTokenGrantType()) {
                await _validateRefreshTokenRequest(context);
                return;
            }
            else if(context.Request.IsAuthorizationCodeGrantType()) {
                await _validateAuthorizationTokenRequest(context);
                return;
            } else if (context.Request.IsClientCredentialsGrantType()) {
                await _validateClientCredentialsTokenRequest(context);
                return;
            }
            context.Reject(
               error: OpenIdConnectConstants.Errors.UnsupportedGrantType,
               description: "Only authorization code, client credentials, and implicit grants " +
                            "are accepted by this authorization server");
            return;
        }
        public override async Task ValidateAuthorizationRequest(ValidateAuthorizationRequestContext context) {
            if (!context.Request.IsAuthorizationCodeFlow()
                && !context.Request.IsClientCredentialsGrantType()
                && !context.Request.IsImplicitFlow()) {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.UnsupportedResponseType,
                    description: "Only the authorization code flow, client credentials code flow, and implicit grant code flows are supported by this server.");
                return;
            }

            await _validateAuthorizationCodeFlow(context);

            return;
        }
        #endregion

        #region Handle
        private async Task _handleAuthorizationToken(HandleTokenRequestContext context) {
            AuthenticateResult ar = await context.HttpContext.AuthenticateAsync(OpenIdConnectServerDefaults.AuthenticationScheme);
            AuthenticationTicket at = ar.Ticket;
            context.Validate(at);
        }
        private async Task _handleRefreshToken(HandleTokenRequestContext context) {
 
        }
        private async Task _handleImplicitCredentialsToken(HandleTokenRequestContext context) {

        }
        private async Task _handleClientCredentialsToken(HandleTokenRequestContext context) {
            ClaimsIdentity identity = new ClaimsIdentity(OpenIdConnectServerDefaults.AuthenticationScheme, OpenIdConnectConstants.Claims.Name, OpenIdConnectConstants.Claims.Role);
            // We serialize the user_id so we can determine which user the caller of this token is
            identity.AddClaim(
                    new Claim(OpenIdConnectConstants.Claims.Subject, context.Request.ClientId)
                        .SetDestinations(OpenIdConnectConstants.Destinations.AccessToken, OpenIdConnectConstants.Destinations.IdentityToken));

            // We serialize the grant_type so we can user discriminate rate-limits. AuthorizationCode grants have the highest rate-limit allowance
            identity.AddClaim(
                    new Claim("grant_type", OpenIdConnectConstants.GrantTypes.ClientCredentials)
                                            .SetDestinations(OpenIdConnectConstants.Destinations.AccessToken, OpenIdConnectConstants.Destinations.IdentityToken));

            // We serialize the client_id so we can monitor for usage patterns of a given app, and also to allow for app-based token revokes.
            identity.AddClaim(
                    new Claim("client_id", context.Request.ClientId)
                    .SetDestinations(OpenIdConnectConstants.Destinations.AccessToken, OpenIdConnectConstants.Destinations.IdentityToken));

            AuthenticationTicket ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), new AuthenticationProperties(), OpenIdConnectServerDefaults.AuthenticationScheme);
            context.Validate(ticket);
        }
        public override async Task HandleTokenRequest(HandleTokenRequestContext context) {
            authservice = context.HttpContext.RequestServices.GetRequiredService<IValidateAuthorizationService>();
            if (authservice == null) {
                context.Reject(OpenIdConnectConstants.Errors.ServerError, "Failed to validate this authorization request");
                return;
            }
            if (context.Request.IsRefreshTokenGrantType()) {
                await _handleRefreshToken(context);
                return;
            }
            else if (context.Request.IsAuthorizationCodeGrantType()) {
                await _handleAuthorizationToken(context);
                return;
            }
            else if (context.Request.IsImplicitFlow()) {
                await _handleImplicitCredentialsToken(context);
                return;
            }
            else if (context.Request.IsClientCredentialsGrantType()) {
                await _handleClientCredentialsToken(context);
                return;
            }
            context.Reject(
               error: OpenIdConnectConstants.Errors.UnsupportedGrantType,
               description: "Only authorization code, client credentials, and implicit grants " +
                            "are accepted by this authorization server");
            return;
        }
        #endregion

        #region Apply
        private void _stripC(OpenIdConnectResponse response) {
            // Here we remove any internal scopes we added for the sake creating a serialized access token.
            // For example, openid and offline access should not be returned to the user
            // Furthermore, the internal scope of Public, which is implied by all token requests, should not be displayed to the user.
            List<string> stripped_scopes = new List<string>();
            if (!String.IsNullOrWhiteSpace(response.Scope)) {
                foreach (string scope in response.Scope.Split(' ')) {
                    if (SpotifyScope.IdMap.ContainsKey(scope) && scope != SpotifyScope.Public.Name) {
                        stripped_scopes.Add(scope);
                    }
                }
                response.Scope = String.Join(' ', stripped_scopes);
            }
            response.RemoveParameter("resource");
            response.RemoveParameter("id_token");
        }
        private void _stripUnnecessaryResponseParameters(ApplyAuthorizationResponseContext context) {
            _stripC(context.Response);
            if (context.Request.IsImplicitFlow()) {
                // Implicit Grant does not supply the Scopes in the response.
                context.Response.RemoveParameter("scope");
            }
        }
        private void _stripUnnecessaryResponseParameters(ApplyTokenResponseContext context) {
            _stripC(context.Response);
        }

        private async Task _applyAuthorizationToken(ApplyTokenResponseContext context) {
            /* Only for successful Token grants */
            if (!String.IsNullOrWhiteSpace(context.Response.AccessToken)) {
                var at = context.Response.AccessToken;
                var rt = context.Response.RefreshToken;
                var ei = context.Response.ExpiresIn ?? 0;

                var authResult = await context.HttpContext.AuthenticateAsync(OpenIdConnectServerDefaults.AuthenticationScheme);
                string clientid = authResult.Principal.Claims.Where(x => x.Type == "client_id").First().Value;
                string userid = authResult.Principal.Claims.Where(x => x.Type == "sub").First().Value;
                

                // Replace the old Tokens with the new ones
                MockifyDbContext DatabaseContext = context.HttpContext.RequestServices.GetRequiredService<MockifyDbContext>();
                ApplicationUser au = await DatabaseContext.ApplicationUser.Include(x => x.UserApplicationTokens).Where(x => x.Id == userid).FirstOrDefaultAsync();
                foreach (UserApplicationToken old in au.UserApplicationTokens.Where(x => x.ClientId == clientid)) {
                    au.UserApplicationTokens.Remove(old);
                }
                au.UserApplicationTokens.Add(new UserApplicationToken() { ClientId = clientid, TokenType = "access_token", TokenValue = at, ExpiresAt = DateTime.UtcNow.AddSeconds(ei) });
                au.UserApplicationTokens.Add(new UserApplicationToken() { ClientId = clientid, TokenType = "refresh_token", TokenValue = rt });

                await DatabaseContext.SaveChangesAsync();
                _stripUnnecessaryResponseParameters(context);
            }
        }
        private async Task _applyRefreshToken(ApplyTokenResponseContext context) {
            if (!String.IsNullOrWhiteSpace(context.Response.AccessToken)) {
                var at = context.Response.AccessToken;

                var authResult = await context.HttpContext.AuthenticateAsync(OpenIdConnectServerDefaults.AuthenticationScheme);
                string clientid = authResult.Principal.Claims.Where(x => x.Type == "client_id").First().Value;
                string userid = authResult.Principal.Claims.Where(x => x.Type == "sub").First().Value;


                // Replace the old Tokens with the new ones
                MockifyDbContext DatabaseContext = context.HttpContext.RequestServices.GetRequiredService<MockifyDbContext>();
                ApplicationUser au = await DatabaseContext.ApplicationUser.Include(x=>x.UserApplicationTokens).Where(x=>x.Id == userid).FirstOrDefaultAsync();
                foreach (UserApplicationToken old in au.UserApplicationTokens.Where(x => x.ClientId == clientid && x.TokenType == "access_token").ToList()) {
                    au.UserApplicationTokens.Remove(old);
                }
                au.UserApplicationTokens.Add(new UserApplicationToken() { ClientId = clientid, TokenType = "access_token", TokenValue = at });

                await DatabaseContext.SaveChangesAsync();
                _stripUnnecessaryResponseParameters(context);
            }
        }
        private async Task _applyImplicitToken(ApplyTokenResponseContext context) {
            if (!String.IsNullOrWhiteSpace(context.Response.AccessToken)) {
                var at = context.Response.AccessToken;
                var ei = context.Response.ExpiresIn ?? 0;

                var authResult = await context.HttpContext.AuthenticateAsync(OpenIdConnectServerDefaults.AuthenticationScheme);
                string clientid = authResult.Principal.Claims.Where(x => x.Type == "client_id").First().Value;
                string userid = authResult.Principal.Claims.Where(x => x.Type == "sub").First().Value;


                // Replace the old Tokens with the new ones
                // If the user has an existing refresh or access token from the same application, they are both erased in favor of the one new Access token.
                MockifyDbContext DatabaseContext = context.HttpContext.RequestServices.GetRequiredService<MockifyDbContext>();
                ApplicationUser au = await DatabaseContext.ApplicationUser.Include(x => x.UserApplicationTokens).Where(x => x.Id == userid).FirstOrDefaultAsync();
                foreach (UserApplicationToken old in au.UserApplicationTokens.Where(x => x.ClientId == clientid)) {
                    au.UserApplicationTokens.Remove(old);
                }
                au.UserApplicationTokens.Add(new UserApplicationToken() { ClientId = clientid, TokenType = "access_token", TokenValue = at, ExpiresAt = DateTime.UtcNow.AddSeconds(ei)});

                await DatabaseContext.SaveChangesAsync();
                _stripUnnecessaryResponseParameters(context);
            }
        }
        private async Task _applyClientCredentialsToken(ApplyTokenResponseContext context) {
            if (!String.IsNullOrWhiteSpace(context.Response.AccessToken)) {
                var at = context.Response.AccessToken;
                string clientid = context.Request.ClientId;
                var ei = context.Response.ExpiresIn ?? 0;

                // Write this Client Access Token to the database, replacing any old one that may be in use.
                MockifyDbContext DatabaseContext = context.HttpContext.RequestServices.GetRequiredService<MockifyDbContext>();
                RegisteredApplication ra = await DatabaseContext.Applications.Include(x => x.ClientCredentialToken).FirstOrDefaultAsync(x => x.ClientId == clientid);
                if(ra == null) {
                    // ??
                    return;
                }
                else {
                    ra.ClientCredentialToken = new UserApplicationToken() { ClientId = clientid, TokenType = "client_credential", TokenValue = at, ExpiresAt = DateTime.UtcNow.AddSeconds(ei) };
                    await DatabaseContext.SaveChangesAsync();
                    _stripUnnecessaryResponseParameters(context);
                }

            }
        }
        public override async Task ApplyTokenResponse(ApplyTokenResponseContext context) {
            if (context.Request.IsRefreshTokenGrantType()) {
                await _applyRefreshToken(context);
            } else if (context.Request.IsAuthorizationCodeGrantType()) {
                await _applyAuthorizationToken(context);
            } else if (context.Request.IsImplicitFlow()) {
                await _applyImplicitToken(context);
            } else if (context.Request.IsClientCredentialsGrantType()) {
                await _applyClientCredentialsToken(context);
            }
            await base.ApplyTokenResponse(context);
        }
        public override Task ApplyAuthorizationResponse(ApplyAuthorizationResponseContext context) {
            context.Response.State = context.Request.State;
            _stripUnnecessaryResponseParameters(context);
            return base.ApplyAuthorizationResponse(context);
        }
        #endregion
    }


}
