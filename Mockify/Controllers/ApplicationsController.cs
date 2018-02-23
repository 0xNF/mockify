using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mockify.Data;
using Mockify.Models;
using Mockify.ViewModels;

namespace Mockify.Controllers {
    [Route("/applications")]
    public class ApplicationsController : Controller {

        private MockifyDbContext _context;

        public ApplicationsController(MockifyDbContext context) {
            this._context = context;
        }

        public IActionResult Index() {
            List<RegisteredApplication> apps = _context.Applications.ToList();
            ApplicationsViewModel apvm = new ApplicationsViewModel() {
                Applications = apps
            };
            return View(apvm);
        }

        [AllowAnonymous]
        [HttpPost("/register")]
        public async Task<IActionResult> Register(RegisteredApplicationViewModel model, string returnUrl = "/applications") {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid) {
                RegisteredApplication ra = await _context.CreateRegisteredApplication(model.ApplicationName, model.ApplicationDescription);
                if (ra != null) {
                    return RedirectToLocal(returnUrl);
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
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
