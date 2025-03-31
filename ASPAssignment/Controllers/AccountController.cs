using ASPAssignment.ViewModels;
using Business.Dtos;
using Business.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPAssignment.Controllers;

public class AccountController(IAccountService accountService) : Controller
{
    private readonly IAccountService _accountService = accountService;

    [HttpGet]
    public IActionResult Login(string returnUrl = "~/")
    {
        ViewBag.ReturnUrl = returnUrl;
        ViewBag.ErrorMessage = string.Empty;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginForm form, string returnUrl = "/Home/Index")
    {
        if (ModelState.IsValid)
        {
            var dto = new LoginDto
            {
                Email = form.Email,
                Password = form.Password
            };

            var result = await _accountService.LoginAsync(dto);
            if (result)
                return LocalRedirect(returnUrl);

            ViewBag.ErrorMessage = "Invalid login attempt.";
        }

        return View(form);
    }

    [HttpGet]
    public IActionResult Register()
    {
        ViewBag.ErrorMessage = string.Empty;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterForm form)
    {
        if (ModelState.IsValid)
        {
            var dto = new RegisterDto
            {
                FirstName = form.FirstName,
                LastName = form.LastName,
                Email = form.Email,
                Password = form.Password
            };

            var result = await _accountService.RegisterAsync(dto);
            if (result)
                return LocalRedirect("~/");

            ViewBag.ErrorMessage = "Registration failed. Please try again.";
        }
        else
        {
            ViewBag.ErrorMessage = "One or more fields are invalid.";
        }

        return View(form);
    }

    [Authorize]
    public async Task<IActionResult> LogoutAsync()
    {
        await _accountService.SignOutAsync();
        return LocalRedirect("~/");
    }
}
