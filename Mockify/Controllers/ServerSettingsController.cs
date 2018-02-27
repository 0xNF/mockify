using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mockify.Data;
using Mockify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mockify.Controllers {
    [Route("/settings")]
    public class ServerSettingsController : Controller {
        private readonly MockifyDbContext _mc;
        private readonly ILogger _logger;

        public ServerSettingsController(
            UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, MockifyDbContext mockifyContext,  ILogger<AccountController> logger) {
            _logger = logger;
            _mc = mockifyContext;
            ensureServerSettings();
        }
        
        private void ensureServerSettings() {
            if (ServerSettings.Settings == null) {
                try {
                    IQueryable<ServerSettings> sec = _mc.ServerSettings.Include(x => x.RateLimits).Include(x => x.Endpoints);
                    if(!sec.Any()) {
                        ServerSettings.Settings = ServerSettings.DEFAULT;
                    }
                }
                catch (Exception e) {
                    //Make a default Server Settings
                    ServerSettings.Settings = ServerSettings.DEFAULT;
                }
            }
        }

        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        public IActionResult Index() {
            ensureServerSettings();
            return View(ServerSettings.Settings);
        }

        #region Helpers

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

        #endregion

    }
}
