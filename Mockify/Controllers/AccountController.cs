using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mockify.Data;
using Mockify.Models;
using Mockify.Models.AccountViewModels;
using Mockify.Services;
using Mockify.ViewModels;

namespace Mockify.Controllers {

    [Authorize]
    [Route("/us/account")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly MockifyDbContext _mc;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            MockifyDbContext mockifyContext,
            IEmailSender emailSender,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _mc = mockifyContext;
        }

        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        public IActionResult Index() {
            return RedirectToLocal("/us/account/overview");
        }

        [HttpGet("overview/")]
        public async Task<IActionResult> Overview() {
            ApplicationUser au = await _userManager.GetUserAsync(HttpContext.User);
            return View("Overview", au);
        }

        [HttpGet("apps/")]
        public async Task<IActionResult> Apps() {
            string userid = _userManager.GetUserId(HttpContext.User);
            ApplicationUser au = await _mc.ApplicationUser.Include(x => x.UserApplicationTokens).Where(x => x.Id == userid).FirstOrDefaultAsync();
            IEnumerable<string> idsOfAllowedApps = au.UserApplicationTokens.Select(x => x.ClientId);
            List<RegisteredApplication> allowedApps = _mc.Applications
                .Where(x => idsOfAllowedApps.Contains(x.ClientId))
                .ToList();
            UsersAppsViewModel uavm = new UsersAppsViewModel() {
                User = au,
                Applications = allowedApps
            };
            if (au == null) {
                return RedirectToLocal("/");
            }
            return View("Apps", uavm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost("apps/revoke/{client_id}")]
        public async Task<IActionResult> Revoke(string client_id) {
            string userid = _userManager.GetUserId(HttpContext.User);
            ApplicationUser au = await _mc.ApplicationUser.Include(x => x.UserApplicationTokens).Where(x => x.Id == userid).FirstOrDefaultAsync();
            List<UserApplicationToken> uats = au.UserApplicationTokens.Where(x => x.ClientId == client_id).ToList();
            foreach(UserApplicationToken uat in uats) {
                au.UserApplicationTokens.Remove(uat);
            }
            await _mc.SaveChangesAsync();
            return RedirectToLocal("/us/account/apps");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    //await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("User created a new account with password.");
                    return RedirectToLocal(returnUrl);
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}
