

using System.Security.Claims;
using System.Text.Json;
using HorsesForCourses.Core;
using HorsesForCourses.MVC.Models.Accounts;
using HorsesForCourses.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace HorsesForCourses.MVC.Controllers;

public class AccountController : Controller
{
    private readonly IAccountService _service;
    private readonly ICoachService _coachservice;

    public AccountController(IAccountService service, ICoachService coachservice)
    {
        _service = service;
        _coachservice = coachservice;
    }


    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpGet]
    public IActionResult AccessDenied(string? returnUrl = null)
    {
        return View(model: returnUrl);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string password)
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, email) };

        var hasher = new Pbkdf2PasswordHasher();

        var user = await _service.GetUser(email);
        if (user is null) return NotFound();

        if (!hasher.Verify(password, user.PasswordHash))
        {
            return BadRequest("Invalid password.");
        }

        claims.Add(new Claim(ClaimTypes.Role, user.Role));
        var id = new ClaimsIdentity(claims, "Cookies");
        await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(id));
        return Redirect("../Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("Cookies");
        return Redirect("../Home");
    }


    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterAccountViewModel account, string choice)
    {
        var newuser = AppUser.From(account.Name, account.Email, account.Password, account.PassConfirm, choice);
        if (choice == "coach")
        {
            await _coachservice.AddCoach(account.Name, account.Email);
        }

        await _service.AddUser(newuser);
        return await Login(newuser.Email.Value, account.Password);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveUser(string email)
    {
        var user = await _service.GetUser(email);
        if (user is null) return NotFound();

        await _service.RemoveUser(user);
        await Logout();
        return Redirect("../Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DownloadUserData(string email)
    {
        var user = await _service.GetUser(email);
        if (user is null) return NotFound();

        var json = JsonSerializer.Serialize(user);
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);

        return File(bytes, "application/json", "userdata.json");
    }
}