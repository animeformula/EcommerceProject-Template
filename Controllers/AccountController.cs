using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EcommerceProject.Models;
using EcommerceProject.ViewModels;

// This is the AccountController class that handles the registration and login of users

namespace EcommerceProject.Controllers
{
    public class AccountController : Controller
    {
        // This is the constructor for the AccountController class
        // It takes a UserManager<ApplicationUser> and a SignInManager<ApplicationUser> as parameters
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // This is the Register action that displays the registration view
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // This is the Register action that handles the registration of a new user
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // This is the user object that will be created
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address = model.Address,
                    City = model.City,
                    State = model.State,
                    PostalCode = model.PostalCode
                };

                // This is the result of the user creation
                var result = await _userManager.CreateAsync(user, model.Password);

                // If the user creation was successful, sign in the user and redirect to the home page
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                // If the user creation was not successful, add the errors to the model state
                foreach (var error in result.Errors)    
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If the model state is not valid, return the registration view with the model
            return View(model);
        }

        // This is the Login action that displays the login view
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // This is the Login action that handles the login of a user
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // If the model state is valid, attempt to sign in the user
            if (ModelState.IsValid)
            {
                // This is the result of the sign in attempt
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false);

                // If the sign in was successful, redirect to the home page
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }

        // This is the Logout action that handles the logout of a user
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // This is the result of the logout attempt
            await _signInManager.SignOutAsync();

            // This is the redirect to the home page
            return RedirectToAction("Index", "Home");
        }
    }
} 