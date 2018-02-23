using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mockify.Data;
using Mockify.Models;
using Mockify.ViewModels;

namespace Mockify.Controllers {
    [Route("/applications/{clientid}")]
    public class RegisteredApplicationController : Controller {

        private MockifyDbContext _context;

        public RegisteredApplicationController(MockifyDbContext context) {
            this._context = context;
        }

        public async Task<IActionResult> Index(string clientid) {
            RegisteredApplication ra = await _context.Applications.Include(r => r.RedirectURIs).FirstOrDefaultAsync(r2 => r2.ClientId.Equals(clientid));
            if (ra == null) {
                return Error();
            }
            else {
                RegisteredApplicationViewModel rvm = new RegisteredApplicationViewModel() {
                    ApplicationDescription = ra.ApplicationDescription,
                    ApplicationName = ra.ApplicationName,
                    RedirectURIs = ra.RedirectURIs.Select(x => x.URI),
                    ClientId = ra.ClientId,
                    ClientSecret = ra.ClientSecret
                };
                return View(rvm);
            }
        }


        [HttpGet("error")]
        public IActionResult Error() {
            return View("Error");
        }

        [AllowAnonymous]
        [HttpPost("/deregister")]
        public async Task<IActionResult> Deregister(string clientId, string returnUrl = "/applications") {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid) {
                bool success = await _context.DeleteRegisteredApplication(clientId);
                if (success) {
                    return RedirectToLocal(returnUrl);
                }
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost("/changesecret")]
        public async Task<IActionResult> ChangeSecret(string clientId, string returnUrl = "/applications") {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid) {
                RegisteredApplication success = await _context.UpdateClientSecret(clientId);
                if (success != null) {
                    return RedirectToLocal($"/applications/{clientId}");
                }
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost("/update")]
        public async Task<IActionResult> UpdateRegisteredApplication(RegisteredApplicationViewModel model, string returnUrl = "/applications") {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid) {
                RegisteredApplication registeredApp = await _context.Applications.Include(r => r.RedirectURIs).FirstOrDefaultAsync(r2 => r2.ClientId.Equals(model.ClientId));
                if (registeredApp == null) {
                    return View(); // We failed?
                }
                var RedirectUriForm = HttpContext.Request.Form;
                List<string> rdis = registeredApp.RedirectURIs.Select(x => x.URI).ToList();
                List<string> newRids = new List<string>();
                foreach (string key in RedirectUriForm.Keys) {
                    if (key.ToLowerInvariant().StartsWith("redirect_")) {
                        string val = RedirectUriForm[key];
                        if (!String.IsNullOrWhiteSpace(val)) {
                            bool success = Uri.TryCreate(val, UriKind.RelativeOrAbsolute, out Uri testUri);
                            if (success) {
                                string pathAndQuery = testUri.ToString();
                                // Check that this item hasn't already been added
                                if (!newRids.Contains(pathAndQuery)) {
                                    newRids.Add(pathAndQuery);
                                }
                            }
                        }
                    }
                }
                registeredApp.ApplicationDescription = String.IsNullOrEmpty(model.ApplicationDescription) ? registeredApp.ApplicationDescription : model.ApplicationDescription;
                registeredApp.ApplicationName = String.IsNullOrEmpty(model.ApplicationName) ? registeredApp.ApplicationName : model.ApplicationName;
                registeredApp.RedirectURIs = newRids.Any() ? newRids.Select((r) => {
                    return new RedirectURI(r) { RegisteredApplication = registeredApp, RegisteredApplicationId = registeredApp.ClientId };
                }).ToList() : registeredApp.RedirectURIs;
                RegisteredApplication updated = await _context.UpdateRegisteredApplication(registeredApp);
                if (updated != null) {
                    return RedirectToLocal(returnUrl);
                }
            }
            return View();
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
