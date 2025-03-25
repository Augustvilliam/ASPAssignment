using Business.Interface;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASPAssignment.Controllers;

public class AccountController(IAccountService accountService) : Controller
{
    private readonly IAccountService _accountService = accountService;

    public IActionResult Login()
    {
        ViewBag.ErrorMessage = string.Empty;

        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(LoginForm form, string returnUrl ="")
    {
        ViewBag.ErrorMessage = string.Empty;

        if (!ModelState.IsValid)
        {
            ViewBag.ErrorMessage = "Invalid Email or password. Please try again.";
            return View(form);
        }

        var result = await _accountService.LoginAsync(form);
        if (result)
        {
            return Redirect(returnUrl);
        }

        ViewBag.ErrorMessage = "Something went wrong. Please try again later";
        return View();

    }
    public IActionResult Register()
    {
        return View();
    }
}
