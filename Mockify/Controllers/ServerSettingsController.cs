using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mockify.Data;
using Mockify.Models;
using Mockify.ViewModels;
using Mockify.Models.Spotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mockify.Controllers {
    [Route("/settings")]
    public class ServerSettingsController : Controller {

        private readonly MockifyDbContext _mc;
        private readonly ILogger _logger;

        public ServerSettingsController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, MockifyDbContext mockifyContext,  ILogger<AccountController> logger) {
            _logger = logger;
            _mc = mockifyContext;
        }
        

        private RateLimitViewModel makeRateVm(RateLimits rl) {
            RateLimitViewModel rlvm = new RateLimitViewModel() {
                CallsPerWindow = rl.CallsPerWindow,
                RateLimitId = rl.RateLimitsId,
                RateWindowInMinutes = (int)Math.Ceiling(rl.RateWindow.TotalMinutes)
            };
            return rlvm;
        }
        private ServerSettingsViewModel makeVm(ServerSettings settings) {
            ServerSettingsViewModel ssvm = new ServerSettingsViewModel() {
                DefaultMarket = settings.DefaultMarket,
                ResponseModeDescription = settings.ResponseMode.Description,
                ServerSettingsId = settings.ServerSettingsId,
                ResponseModeId = settings.ResponseMode.Name.ToString(),
                RateLimits = makeRateVm(settings.RateLimits)
            };
            return ssvm;
        }

        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        public async Task<IActionResult> Index() {
            ServerSettingsViewModel ssvm = makeVm(ServerSettings.Settings);
            return View(ssvm);
        }

        [HttpPost]
        public async Task<IActionResult> SaveSettings(ServerSettingsViewModel vm) {
            if(!ModelState.IsValid) {
                return RedirectToLocal("/settings");
            }
            if(!Country.SpotifyMarkets.Select(x=>x.FormalName).Contains(vm.DefaultMarket)) {
                ModelState.AddModelError(string.Empty, "Supplied default country not a valid country");
            }
            // TODO View is supplying the Name to us, but it should be supplying the Id
            if (!SpecialResponseMode.SpecialResponseModes.Select(x => x.Name).Contains(vm.ResponseModeId)) {
                ModelState.AddModelError(string.Empty, "Supplied Special Response Mode not a valid Special Response Mode");
            }
            if(vm.RateLimits.CallsPerWindow > RateLimits.MaxRateLimit) {
                ModelState.AddModelError(string.Empty, $"Supplied Calls per Window is greater than the maximum supported ({RateLimits.MaxRateLimit})");
            }
            if(vm.RateLimits.CallsPerWindow < RateLimits.MinRateLimit) {
                ModelState.AddModelError(string.Empty, $"Supplied Calls per Window is less than the minimum supported ({RateLimits.MinRateLimit})");
            }
            if (vm.RateLimits.RateWindowInMinutes > RateLimits.MaxTimeWindowMinutes) {
                ModelState.AddModelError(string.Empty, $"Supplied Time Window is greater than the maximum supported ({RateLimits.MaxTimeWindowMinutes})");
            }
            if (vm.RateLimits.RateWindowInMinutes < RateLimits.MinTimeWindowMinutes) {
                ModelState.AddModelError(string.Empty, $"Supplied Time Window is less than the minimum supported ({RateLimits.MinTimeWindowMinutes})");
            }
            if (ModelState.ErrorCount > 0) {
                return View();
            }
            else {
                // Save the new settings
                ServerSettings.Settings.DefaultMarket = vm.DefaultMarket;
                ServerSettings.Settings.ResponseMode = SpecialResponseMode.SpecialResponseModes.Where(x => x.Name == vm.ResponseModeId).First();
                ServerSettings.Settings.RateLimits.CallsPerWindow = vm.RateLimits.CallsPerWindow;
                ServerSettings.Settings.RateLimits.RateWindow = TimeSpan.FromMinutes(vm.RateLimits.RateWindowInMinutes);
                _mc.ServerSettings.Update(ServerSettings.Settings);
                await _mc.SaveChangesAsync();
            }
            return Ok();
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
