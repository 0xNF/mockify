using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mockify.Models;
using Mockify.Models.Spotify;
using Mockify.Services;
using Mockify.ViewModels;

namespace Mockify.Controllers {

    [Route("/users")]
    public class UsersController : Controller {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private const int MAXUSERS = 100_000_000;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger) {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery]int offset = 0, [FromQuery]int limit = 20) {
            Paging<ApplicationUser> pt;
            UserPageViewModel upvm = new UserPageViewModel() {
                CreateSingleViewModel = new CreateUserViewModel(),
                CreateManyViewModel = new CreateManyUsersViewModel()
            };
            try {
                pt = await GetUserPage(offset, limit);
                upvm.UserListViewModel = pt;
            } catch (ArgumentOutOfRangeException ooe) {
                pt = await GetUserPage(0, 20);
                upvm.UserListViewModel = pt;
            } catch (Exception e) {
                pt = await GetUserPage(0, 20);
                upvm.UserListViewModel = pt;
            }
            return View(upvm);
        }

        private string validateDate(string input) {
            //string[] dateformats = { "d/M/yyyy", "d/MM/yyyy", "dd/MM/yyyy", "dd/M/yyyy", };
            string[] dateformats = { "M/d/yyyy", "MM/dd/yyyy", "M/dd/yyyy", "MM/d/yyyy" };
            DateTime bdate;
            try {
                bdate = DateTime.ParseExact(input, dateformats,
                                             new CultureInfo("en-US"),
                                             DateTimeStyles.None);
                return bdate.ToString("MM/dd/yyyy");
            }
            catch {
                return "";
            }
        }

        private string validateProduct(string input) {
            foreach (SpotifyAccountTypes st in SpotifyAccountTypes.AllValues) {
                if (st.Name.Equals(input)) {
                    return st.Name;
                }
            }
            return SpotifyAccountTypes.Free.Name;
        }

        private string validateEmail(string input) {
            if (input.Contains("@")) {
                return input;
            } else {
                return "";
            }
        }

        private string validateCountry(string input) {
            foreach (Country c in Country.SpotifyMarkets) {
                if (c.FormalName.Equals(input)) {
                    return c.FormalName;
                }
            }
            return Country.US.FormalName; // Default is US
        }

        private string validateDisplayName(string input) {
            return input; // ???
        }

        private async Task<Paging<ApplicationUser>> GetUserPage(int offset = 0, int limit = 20) {
            if(offset < 0 || limit < 0) {
                throw new ArgumentOutOfRangeException("limit or offset negative");
            }
            List<ApplicationUser> users = await _userManager.Users.OrderBy(x=>x.Id).Skip(offset).Take(limit).ToListAsync();
            int count = await _userManager.Users.CountAsync();
            return new Paging<ApplicationUser>() {
                Limit = limit,
                Offset = offset,
                Total = count,
                Items = users,
            };
        }

        [HttpPost("/createuser")]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model, string returnUrl = "/users") {
            ViewData["ReturnUrl"] = returnUrl;
            // All string length checks took place in the model creation. No need to replicate them 
            if ((await _userManager.Users.CountAsync()) > MAXUSERS) {
                return BadRequest("Too many users in the database, cannot create any more.");
            }
            if (ModelState.IsValid) {
                ApplicationUser au = new ApplicationUser() {
                    Birthdate = validateDate(model.BirthDate),
                    Product = validateProduct(model.Product),
                    Email = validateEmail(model.Email),
                    Country = validateCountry(model.Country),
                    UserName = Guid.NewGuid().ToString(),
                    DisplayName = validateDisplayName(model.DisplayName)
                };
                var result = await _userManager.CreateAsync(au, model.Password);
                if (result.Succeeded) {
                    _logger.LogInformation("User created a new account with password.");
                    return RedirectToLocal(returnUrl);
                }
            }
            return View();
        }

        [HttpPost("/createusers")]
        public async Task<IActionResult> CreateUsers(CreateManyUsersViewModel model, string returnUrl = "/users") {

            if (model.CreateThisManyUsers > 1_000_000) {
                return BadRequest("Too many users. Please submit a number less than 1,000,000");
            }
            else if ((await _userManager.Users.CountAsync()) > MAXUSERS) {
                return BadRequest("Too many users in the database, cannot create any more.");
            }
            for(int i = 0; i < model.CreateThisManyUsers; i++) {
                ApplicationUser au = ApplicationUser.Randomize();
                await _userManager.CreateAsync(au, ApplicationUser.RandomPass());
            }
            return RedirectToLocal(returnUrl);
        }
            
        [HttpPost("/restoreUsers")]
        public async Task<IActionResult> RestoreDefaultUsers(string returnUrl="/users") {
            if(ModelState.IsValid) {
                //TODO 
            }
            return RedirectToLocal(returnUrl);
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
