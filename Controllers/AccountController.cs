using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AnnounceHub.Models;
using AnnounceHub.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

public class AccountController : Controller
{
    private readonly UserManager<AnnounceHub.Data.User> _userManager;
    private readonly SignInManager<AnnounceHub.Data.User> _signInManager;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
        //    var claims = new List<Claim>
        //{
        //    new Claim(ClaimTypes.Name, model.UserName),
        //    new Claim(ClaimTypes.GivenName, model.UserName),
        //    new Claim(ClaimTypes.NameIdentifier, model.UserName),
        //};

        //    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        //    var authenticationProperties = new AuthenticationProperties();

        //    var principal = new ClaimsPrincipal(claimsIdentity);

        //    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authenticationProperties);

         var result = await _signInManager.PasswordSignInAsync(model.UserName!, model.Password!, model.RememberMe, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(model.UserName!);
                var roles = await _userManager.GetRolesAsync(user!);

                foreach (var role in roles)
                {
                    Console.WriteLine($"User {model.UserName} has role: {role}");
                }

                if (model.UserName == "Intitech")
                {
                    return RedirectToAction("Index", "Home");
                }
                return RedirectToAction("Index", "Home");
            }
        }
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new AnnounceHub.Data.User { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password!);
            if (result.Succeeded) return RedirectToAction("Index", "Home");

            foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);
        }
        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
