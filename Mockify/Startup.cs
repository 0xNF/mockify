using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AspNet.Security.OpenIdConnect.Server;
using AspNet.Security.OpenIdConnect;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OAuth.Validation;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Bson;
using System.IO;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;
using Mockify.Data;
using Mockify.Models;
using Mockify.Services;
using Microsoft.AspNetCore.Identity;
using Mockify.Models.Spotify;

namespace Mockify {
    public partial class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MockifyDbContext>(options =>
                //options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"))
                options.UseSqlite("Data Source=Mockify.db")
            );

            services.AddIdentity<ApplicationUser, IdentityRole>((x) => {
                x.Password.RequireNonAlphanumeric = false;
                x.Password.RequiredLength = 6;
                x.Password.RequireDigit = false;
                x.Password.RequireLowercase = false;
                x.Password.RequireUppercase = false;
            })
                .AddEntityFrameworkStores<MockifyDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IValidateAuthorizationService, ValidationAuthorizationService>();


            services.AddAuthentication().AddOpenIdConnectServer(options => {
                options.Provider = new MockifyAuthorizationProvider();
                options.TokenEndpointPath = "/api/token";
                options.AuthorizationEndpointPath = "/authorize/";
                options.UseSlidingExpiration = false; // Spotify does not issue new refresh tokens
            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

    }

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


        private async Task _validateAuthorizationCodeFlow(ValidateAuthorizationRequestContext context) {
            if (string.IsNullOrEmpty(context.RedirectUri)) {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidRequest,
                    description: "The required redirect_uri parameter was missing.");
                return;
            }


            if (!(await authservice.CheckClientIdExists(context.ClientId))) {
                context.Reject(error: OpenIdConnectConstants.Errors.InvalidClient, description: "Supplied Client Id was not a valid application Client Id.");
            }
            if (!(await authservice.CheckRedirectURIMatches(context.RedirectUri, context.ClientId))) {
                context.Reject(error: OpenIdConnectConstants.Errors.InvalidRequest, description: "Supplied Redirect URI was not valid for the supplied Client Id.");
            }
            if (!(await authservice.CheckScopesAreValid(context.Request.Scope))) {
                context.Reject(error: OpenIdConnectConstants.Errors.InvalidRequest, description: "Supplied scopes were was not valid Spotify Scopes.");
            }

            context.Validate();
        }
        // Implement OnValidateAuthorizationRequest to support interactive flows (code/implicit/hybrid).
        public override async Task ValidateAuthorizationRequest(ValidateAuthorizationRequestContext context) {
            authservice = authservice ?? context.HttpContext.RequestServices.GetRequiredService<IValidateAuthorizationService>();
            if (authservice == null) {
                context.Reject(OpenIdConnectConstants.Errors.ServerError, "Failed to validate this authorization request");
                return;
            }
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


        // This checks that the user requesting an API call is within their api allowance as determined by the grant type.
        // True = Within Limits, False = Is Rate Limited
        // TODO NYI.
        private async Task<bool> RequestWithinRateLimits(string client_id, string user_id, string grant_type) {
            //generic rate limits are determined by the grant_type. Authentication_Codes have the highest allowance, Client_Credentials and Implicit have the lowest.
            await Task.Delay(0);
            return true;
        }

        // This increments the user's API call count for the specified client_id
        // True = database success, false = database failure
        private async Task<bool> IncrementUserRate(string client_id, string user_id) {
            return true;
        }

        // Checks that the supplied token, either access or refresh, is valid.
        // true = TOKEN REVOKED
        // false = fine, proceed.
        private async Task<bool> IsTokenRevoked(string client_id, string token) {
            return false;
        }


        #region Validate
        private async Task _validateAuthorizationTokenRequest(ValidateTokenRequestContext context) {
            // TODO increment user rate limit (nb this increment only happens in Validate)
            // TODO check user is within their rate limit for the requested client_id
            if (String.IsNullOrWhiteSpace(context.ClientId)) {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidClient,
                    description: "Missing credentials: ensure that you specified a client_id.");
            }
            else if (String.IsNullOrWhiteSpace(context.ClientSecret)) {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidClient,
                    description: "Missing credentials: ensure that you specified a client_secret.");
            }
            if (String.IsNullOrWhiteSpace(context.Request.Code)) {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidRequest,
                    description: "Missing credentials: ensure that you specified a value for code.");
            }
            context.Validate();
        }
        private async Task _validateRefreshTokenRequest(ValidateTokenRequestContext context) {
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
            } else if (await IsTokenRevoked(context.ClientId, context.Request.RefreshToken)) {
                context.Reject(OpenIdConnectConstants.Errors.AccessDenied, "The supplied token has been revoked. Please re-authenticate.");
            }
            else if (!await RequestWithinRateLimits(context.ClientId, userid.Value, gtype.Value)) {
                context.Reject(RateLimits.RateLimitExceededError, RateLimits.RateLimitExceededErrorDescription);
                return;
            }
            await IncrementUserRate(context.ClientId, userid.Value);
            context.Validate();
        }
        private async Task _validateImplicitTokenRequest(ValidateTokenRequestContext context) {
            // TODO increment user rate limit (nb this increment only happens in Validate)

        }
        private async Task _validateClientCredentialsTokenRequest(ValidateTokenRequestContext context) {
            // TODO increment user rate limit (nb this increment only happens in Validate)

        }
        public override async Task ValidateTokenRequest(ValidateTokenRequestContext context) {
            authservice = authservice ?? context.HttpContext.RequestServices.GetRequiredService<IValidateAuthorizationService>();
            if (authservice == null) {
                context.Reject(OpenIdConnectConstants.Errors.ServerError, "Failed to validate this authorization request");
                return;
            }
            if (context.Request.IsRefreshTokenGrantType()) {
                await _validateRefreshTokenRequest(context);
                return;
            }
            else if(context.Request.IsAuthorizationCodeGrantType()) {
                await _validateAuthorizationTokenRequest(context);
                return;
            }
            else if(context.Request.IsImplicitFlow()) {
                await _validateImplicitTokenRequest(context);
                return;
            }
            context.Reject(
               error: OpenIdConnectConstants.Errors.UnsupportedGrantType,
               description: "Only authorization code, client credentials, and implicit grants " +
                            "are accepted by this authorization server");
            return;
        }
        #endregion

        #region Handle
        public async Task _handleAuthorizationToken(HandleTokenRequestContext context) {
            AuthenticateResult ar = await context.HttpContext.AuthenticateAsync(OpenIdConnectServerDefaults.AuthenticationScheme);
            AuthenticationTicket at = ar.Ticket;
            context.Validate(at);

        }
        public async Task _handleRefreshToken(HandleTokenRequestContext context) {
            // TODO if successful,
            // Store the new refresh tokens in the database. 

        }
        public async Task _handleImplicitCredentialsToken(HandleTokenRequestContext context) {

        }
        public async Task _handleClientCredentialsToken(HandleTokenRequestContext context) {

        }
        public override async Task HandleTokenRequest(HandleTokenRequestContext context) {
            authservice = authservice ?? context.HttpContext.RequestServices.GetRequiredService<IValidateAuthorizationService>();
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
        private void _stripUnnecessaryResponseParameters(ApplyTokenResponseContext context) {
            // Here we remove any internal scopes we added for the sake creating a serialized access token.
            // For example, openid and offline access should not be returned to the user
            // Furthermore, the internal scope of Public, which is implied by all token requests, should not be displayed to the user.
            List<string> stripped_scopes = new List<string>();
            foreach (string scope in context.Response.Scope.Split(' ')) {
                if (SpotifyScope.IdMap.ContainsKey(scope) && scope != SpotifyScope.Public.Name) {
                    stripped_scopes.Add(scope);
                }
            }
            context.Response.Scope = String.Join(' ', stripped_scopes);

            context.Response.RemoveParameter("resource");
            context.Response.RemoveParameter("id_token");
        }
        private async Task _applyAuthorizationToken(ApplyTokenResponseContext context) {
            /* Only for successful Token grants */
            // TODO access & refresh tokens go into Database at this step.
            // TODO Any old tokens that existesd for this user for this application get replaced. 
            if (!String.IsNullOrWhiteSpace(context.Response.AccessToken)) {
                var at = context.Response.AccessToken;
                var rt = context.Response.RefreshToken;
                var ei = context.Response.ExpiresIn;


                _stripUnnecessaryResponseParameters(context);
            }
        }
        private async Task _applyRefreshToken(ApplyTokenResponseContext context) {
            _stripUnnecessaryResponseParameters(context);
        }
        private async Task _applyImplicitToken(ApplyTokenResponseContext context) {

        }
        private async Task _applyClientCredentialsToken(ApplyTokenResponseContext context) {

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
            return base.ApplyAuthorizationResponse(context);
        }
        #endregion
    }


}
