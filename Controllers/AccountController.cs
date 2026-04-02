using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OstaFeedbackApp.Models;
using System.Threading.Tasks;

namespace OstaFeedbackApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // ================= LOGIN =================
        [HttpGet]
        public IActionResult AdminLogin(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminLogin(string email, string password, bool rememberMe, string? returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Email and Password are required.");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(
                email,
                password,
                rememberMe,
                lockoutOnFailure: true
            );

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Dashboard", "Feedback");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View();
        }

        // ================= REGISTER USER =================
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult RegisterUser()
        {
            return View(new RegisterUserViewModel());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterUser(RegisterUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // 🔒 Prevent duplicate users
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "User already exists.");
                return View(model);
            }

            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(model);
            }

            // 🔐 Assign Role (SAFE)
            var roleAssigned = await _userManager.AddToRoleAsync(user, model.Role);

            if (!roleAssigned.Succeeded)
            {
                ModelState.AddModelError("", "User created but role assignment failed.");
                return View(model);
            }

            ViewBag.Success = "User created successfully!";
            return View(new RegisterUserViewModel());
        }

        // ================= LOGOUT =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            Response.Cookies.Delete("OstaAdminAuth");

            return RedirectToAction("AdminLogin");
        }
    }
}