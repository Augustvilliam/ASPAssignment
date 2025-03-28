using Business.Interface;
using Business.Services;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPAssignment.Controllers;

public class AccountController(IAccountService accountService) : Controller
{
    private readonly IAccountService _accountService = accountService;

    public IActionResult Login(string returnUrl = "~/")
    {
        ViewBag.ErrorMessage = "";
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(LoginForm form, string returnUrl = "/Home/Index")
    {
        ViewBag.ErrorMessage = "";

        if (ModelState.IsValid)
        {
            var result = await _accountService.LoginAsync(form);
           if (result)
                return LocalRedirect(returnUrl);
        }

        ViewBag.ErrorMessage = "Invalid login attempt";
        return View();
    }

    public IActionResult Register()
    {
        ViewBag.ErrorMessage = "";
        return View();
    }

    [HttpPost]
     public async Task<IActionResult> Register(RegisterForm form)
    {
        if (ModelState.IsValid)
        {
            var result = await _accountService.RegisterAsync(form);
            if (result)
                return LocalRedirect("~/");
        }

        ViewBag.ErrorMessage = "";
        return View(form);
    }

    [Authorize]

    public async Task<IActionResult> LogoutAsync()
    {
        await _accountService.SignOutAsync();
        return LocalRedirect("~/");
    }
}
