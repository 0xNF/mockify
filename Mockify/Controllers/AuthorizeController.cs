using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AspNet.Security.OpenIdConnect.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mockify.Data;
using Mockify.Models;
using Mockify.Models.Spotify;
using Mockify.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Mockify.Controllers {

    [Authorize]
    [Area("account")]
    [Route("/authorize/")]
    public class AuthorizeController : Controller {

        private readonly ILogger _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly MockifyDbContext _mockifyContext;

        public AuthorizeController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AccountController> logger, MockifyDbContext mc) {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _mockifyContext = mc;
        }

        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CancellationToken cancellationToken) {
            OpenIdConnectRequest request = HttpContext.GetOpenIdConnectRequest();
            RegisteredApplication ra = await (from entity in _mockifyContext.Applications
                                     where entity.ClientId == request.ClientId
                                     select entity).SingleOrDefaultAsync(cancellationToken);
            if(ra == null) {
                return View("Error", new AuthorizeErrorViewModel {
                    Error = OpenIdConnectConstants.Errors.InvalidClient,
                    ErrorDescription = "Details concerning the calling client " +
                       "application cannot be found in the database"
                });
            }
            // Regular view
            List<SpotifyScope> sscopes = new List<SpotifyScope>() { SpotifyScope.Public };
            if (!string.IsNullOrWhiteSpace(request.Scope)) {
                foreach (string split in request.Scope.Split(',')) {
                    if (SpotifyScope.IdMap.ContainsKey(split)) {
                        var scope = SpotifyScope.IdMap[split];
                        if(!sscopes.Contains(scope)) {
                            sscopes.Add(SpotifyScope.IdMap[split]);
                            // no dupes
                        }
                    }
                }
            }
            return View(new AuthorizeViewModel() {
                AppName = ra.ApplicationName,
                ClientId = ra.ClientId,
                Description = ra.ApplicationDescription,
                RedirectUri = request.RedirectUri,
                TokenType = request.ResponseType,
                Scopes = sscopes,
                State = request.State
            });
        }

        [HttpPost("accept")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(CancellationToken cancellationToken) {

            ApplicationUser au = await _userManager.GetUserAsync(HttpContext.User);
            if (au == null) {
                return View("Error", new AuthorizeErrorViewModel() {
                    Error = "No such user",
                    ErrorDescription = "Failed to find specified user"
                });
            }


            OpenIdConnectRequest request = HttpContext.GetOpenIdConnectRequest();

            ClaimsIdentity identity = new ClaimsIdentity(OpenIdConnectServerDefaults.AuthenticationScheme, OpenIdConnectConstants.Claims.Name, OpenIdConnectConstants.Claims.Role);

            // We serialize the user_id so we can determine which user the caller of this token is
            string userid = _userManager.GetUserId(HttpContext.User);
            identity.AddClaim(
                    new Claim(OpenIdConnectConstants.Claims.Subject, userid)
                        .SetDestinations(OpenIdConnectConstants.Destinations.AccessToken, OpenIdConnectConstants.Destinations.IdentityToken));

            // We serialize the grant_type so we can user discriminate rate-limits. AuthorizationCode grants have the highest rate-limit allowance
            identity.AddClaim(
                    new Claim("grant_type", OpenIdConnectConstants.GrantTypes.AuthorizationCode)
                                            .SetDestinations(OpenIdConnectConstants.Destinations.AccessToken, OpenIdConnectConstants.Destinations.IdentityToken));

            // We serialize the client_id so we can monitor for usage patterns of a given app, and also to allow for app-based token revokes.
            identity.AddClaim(
                    new Claim("client_id", request.ClientId)
                    .SetDestinations(OpenIdConnectConstants.Destinations.AccessToken, OpenIdConnectConstants.Destinations.IdentityToken));



            RegisteredApplication ra = await (from entity in _mockifyContext.Applications
                                     where entity.ClientId == request.ClientId
                                     select entity).SingleOrDefaultAsync(cancellationToken);

            if (ra == null) {
                return View("Error", new AuthorizeErrorViewModel {
                    Error = OpenIdConnectConstants.Errors.InvalidClient,
                    ErrorDescription = "Details concerning the calling client " +
                                       "application cannot be found in the database"
                });
            }

            AuthenticationTicket ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), new AuthenticationProperties(), OpenIdConnectServerDefaults.AuthenticationScheme);

            List<string> scopesToAdd = new List<string>() {
                OpenIdConnectConstants.Scopes.OpenId,
                OpenIdConnectConstants.Scopes.OfflineAccess,
                SpotifyScope.Public.Name,
            };

            if (!String.IsNullOrWhiteSpace(request.Scope)) {
                foreach (string scope in request.Scope.Split(',')) {
                    if (SpotifyScope.IdMap.ContainsKey(scope) && !scopesToAdd.Contains(scope)) {
                        scopesToAdd.Add(scope);
                    }
                }
            }
            ticket.SetScopes(scopesToAdd);
            ticket.SetResources("resource_server"); // ?? what is this
            Microsoft.AspNetCore.Mvc.SignInResult sr = SignIn(ticket.Principal, ticket.Properties, ticket.AuthenticationScheme);
            return sr;
        }

        [HttpPost("deny")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deny(CancellationToken cancellationToken) {
            return Forbid(OpenIdConnectServerDefaults.AuthenticationScheme);
        }


        private IActionResult Error(string err, string message) {
            return View("Error", new AuthorizeErrorViewModel() { Error = err, ErrorDescription = message });
        }

        private void AddErrors(IdentityResult result) {
            foreach (var error in result.Errors) {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl) {
            if (Url.IsLocalUrl(returnUrl)) {
                return Redirect(returnUrl);
            }
            else {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }


    }
}
