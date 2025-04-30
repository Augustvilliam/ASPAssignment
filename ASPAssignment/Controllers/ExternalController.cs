using System.Security.Claims;
using Business.Dtos;
using Business.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ASPAssignment.Controllers;

public class ExternalController : Controller
{
    private readonly IAccountService _accountService;
    private readonly SignInManager<MemberEntity> _signInManager;

    public ExternalController(
        IAccountService accountService,
        SignInManager<MemberEntity> signInManager)
    {
        _accountService = accountService;
        _signInManager = signInManager;
    }

    // GET: /External/ExternalSignIn?returnUrl=/Home/Index
    [HttpGet]
    public IActionResult ExternalSignIn(string returnUrl = null!)
    {
        ViewBag.ReturnUrl = returnUrl ?? Url.Action("Index", "Home");
        return View();
    }

    // POST: /External/ExternalSignIn
    [HttpPost]
    public IActionResult ExternalSignIn(string provider, string returnUrl = null!)
    {
        if (string.IsNullOrEmpty(provider))
        {
            ModelState.AddModelError("", "Invalid provider");
            ViewBag.ReturnUrl = returnUrl ?? Url.Action("Index", "Home");
            return View();
        }

        // Säkerställ att returnUrl skickas vidare till callback
        var redirectUrl = Url.Action(
            nameof(ExternalSignInCallback),
            "External",
            new { returnUrl = returnUrl ?? Url.Action("Index", "Home") }
        )!;
        var props = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(props, provider);
    }

    // GET or POST från externa leverantören
    public async Task<IActionResult> ExternalSignInCallback(
        string returnUrl = null!,
        string remoteError = null!)
    {
        // Standard-redirect om ingen specifik returnUrl är angiven
        returnUrl ??= Url.Action("Index", "Home");

        if (!string.IsNullOrEmpty(remoteError))
        {
            ModelState.AddModelError("", $"Error from external provider: {remoteError}");
            ViewBag.ReturnUrl = returnUrl;
            return View("ExternalSignIn");
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
            return RedirectToAction(nameof(ExternalSignIn), new { returnUrl });

        // 1) Försök logga in med redan kopplad extern-login
        var signInRes = await _signInManager.ExternalLoginSignInAsync(
            info.LoginProvider,
            info.ProviderKey,
            isPersistent: false,
            bypassTwoFactor: true
        );
        if (signInRes.Succeeded)
            return LocalRedirect(returnUrl);

        // 2) Annars: hämta e-post och antingen länka eller skapa konto
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        if (email != null)
        {
            var existing = await _signInManager.UserManager.FindByEmailAsync(email);
            if (existing != null)
            {
                // Länka nya provider till samma konto
                await _accountService.AddLoginAsync(email, info);
                await _signInManager.SignInAsync(existing, isPersistent: false);
                return LocalRedirect(returnUrl);
            }
            else
            {
                // Skapa nytt konto med automagic-lösen
                var dto = new RegisterDto { Email = email };
                var regResult = await _accountService.RegisterAsync(dto);
                if (regResult.Succeeded)
                {
                    await _accountService.AddLoginAsync(email, info);
                    var user = await _signInManager.UserManager.FindByEmailAsync(email);
                    await _signInManager.SignInAsync(user!, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                // Visa eventuella fel från registreringen
                foreach (var e in regResult.Errors)
                    ModelState.AddModelError("", e.Description);
            }
        }

        // Om vi hamnar här: något gick fel, visa login-sidan igen
        ViewBag.ReturnUrl = returnUrl;
        return View("ExternalSignIn");
    }
}
