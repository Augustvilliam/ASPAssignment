﻿
using ASPAssignment.ViewModels;
using Business.Dtos;
using Business.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ASPAssignment.Services;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace ASPAssignment.Controllers;

public class AccountController : Controller
{
    private readonly IAccountService _accountService;
    private readonly IMemberService _memberService;
    private readonly INotificationService _notificationService;
    private readonly UserManager<MemberEntity> _userManager;


    public AccountController(
        IAccountService accountService,
        IMemberService memberService,
        INotificationService notificationService,
        UserManager<MemberEntity> userManager)
    {
        _accountService = accountService;
        _memberService = memberService;
        _notificationService = notificationService;
        _userManager = userManager;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string returnUrl = "/Home/Index")
    {
        ViewBag.ReturnUrl = returnUrl;
        ViewBag.ErrorMessage = string.Empty;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginForm form, string returnUrl = "/Home/Index")
    {
        // Se till att returnUrl alltid finns med i vyn vid redisplay
        ViewBag.ReturnUrl = returnUrl;

        // Om användaren redan är inloggad, skicka direkt vidare
        if (User.Identity?.IsAuthenticated == true)
            return LocalRedirect(returnUrl);

        // Validera client‐side‐errors
        if (!ModelState.IsValid)
        {
            ViewBag.ErrorMessage = string.Empty;
            return View(form);
        }

        var dto = new LoginDto
        {
            Email = form.Email,
            Password = form.Password,
            RememberMe = form.RememberMe
        };

        // Försök logga in
        if (!await _accountService.LoginAsync(dto))
        {
            var err = "Invalid login attempt.";
            ModelState.AddModelError(string.Empty, err);
            ViewBag.ErrorMessage = err;
            return View(form);
        }

        // Lyckad inloggning → redirecta till returnUrl
        return LocalRedirect(returnUrl);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        ViewBag.ErrorMessage = string.Empty;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterForm form)
    {
        if(!form.Terms) //se till att terms är ikryssad
        {
            ModelState.AddModelError(nameof(form.Terms), "You must accept the terms and conditions.");
            return View(form);
        }

        if (!ModelState.IsValid)
            return View(form);

        var dto = new RegisterDto
        {
            FirstName = form.FirstName,
            LastName = form.LastName,
            Email = form.Email,
            Password = form.Password
        }; //skicka till DTO

        var result = await _accountService.RegisterAsync(dto);
        if (!result.Succeeded)
        {
            // fel om e-post redan finns
            if (result.Errors.Any(e => e.Code == "DuplicateEmail"))
            {
                ViewBag.ErrorMessage = "This Email is already in use";
            }
            else
            {
                // Visa övriga valideringsfel normalt
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(form);
        }

        // Lyckad registrering – skicka global notis
        var displayName =
            !string.IsNullOrWhiteSpace(form.FirstName) && !string.IsNullOrWhiteSpace(form.LastName)
            ? $"{form.FirstName} {form.LastName}"
            : form.Email;

        var joinedNotification = new NotificationDto
        {
            ImageUrl = "/img/default-user.svg",
            Message = $"{displayName} Has joined, Say hi!",
            Timestamp = DateTime.UtcNow,
            NotificationType = "UserJoined"
            // NotificationId fylls av SendNotificationAsync per användare
        };

        //Hämta alla medlemmar och skicka notis var för sig:
        var allMembers = await _memberService.GetAllMembersAsync();
        foreach (var member in allMembers)
        {
            await _notificationService.SendNotificationAsync(member.Email, joinedNotification);
        }

        //Påminnelse-notis ENDAST till den nya användaren:
        var reminderNotification = new NotificationDto
        {
            ImageUrl = "/img/profile-reminder.svg",
            Message = "Hi, and welcome, Please go to 'Settings' and finish up your profile!.",
            Timestamp = DateTime.UtcNow,
            NotificationType = "ProfileReminder"
        };
        await _notificationService.SendNotificationAsync(form.Email, reminderNotification);

        return LocalRedirect("~/Account/Login");
    }
    [Authorize]
    public async Task<IActionResult> LogoutAsync()
    {
        await _accountService.SignOutAsync();
        return LocalRedirect("~/");
    }

    public IActionResult Terms()
    {
        return View();
    }

}
