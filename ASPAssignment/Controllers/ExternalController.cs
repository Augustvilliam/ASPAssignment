using System.Security.Claims;
using ASPAssignment.Services;
using Business.Dtos;
using Business.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ASPAssignment.Controllers;
//kontroller för att hantera inloggning med externa leverantörer (t.ex. Google, Facebook)
public class ExternalController : Controller
{
    private readonly IAccountService _accountService;
    private readonly SignInManager<MemberEntity> _signInManager;
    private readonly IMemberService _memberService;
    private readonly INotificationService _notificationService;

    public ExternalController(
        IAccountService accountService,
        SignInManager<MemberEntity> signInManager,
        IMemberService memberService,
        INotificationService notificationService)
    {
        _accountService = accountService;
        _signInManager = signInManager;
        _memberService = memberService;
        _notificationService = notificationService;
    }

    [HttpGet]
    public IActionResult ExternalSignIn(string returnUrl = null!)
    {
        ViewBag.ReturnUrl = returnUrl ?? Url.Action("Index", "Home");
        return View();
    }

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

    public async Task<IActionResult> ExternalSignInCallback(
        string returnUrl = null!,
        string remoteError = null!)
    {
        //Standard-redirect om ingen specifik returnUrl är angiven
        returnUrl ??= Url.Action("Index", "Home");

        if (!string.IsNullOrEmpty(remoteError))
        {
            ViewBag.ErrorMessage = "Something went wrong";
            ModelState.AddModelError("", $"Error from external provider: {remoteError}");
            ViewBag.ReturnUrl = returnUrl;
            return View("ExternalSignIn");
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            ViewBag.ErrorMessage = "Something went wrong";
            ViewBag.ReturnUrl = returnUrl;
            return View("ExternalSignIn");
        }

        //Försök logga in med redan kopplad extern-login
        var signInRes = await _signInManager.ExternalLoginSignInAsync(
            info.LoginProvider,
            info.ProviderKey,
            isPersistent: false,
            bypassTwoFactor: true
        );
        if (signInRes.Succeeded)
            return LocalRedirect(returnUrl);

        //Annars: hämta e-post och antingen länka eller skapa konto
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
            else //else gjort av chatgpt-mini-high
            {
                // Skapa nytt konto med automagic-lösen
                var dto = new RegisterDto { Email = email };
                var regResult = await _accountService.RegisterAsync(dto);
                if (regResult.Succeeded)
                {
                    // Länka login och logga in
                    await _accountService.AddLoginAsync(email, info);
                    var user = await _signInManager.UserManager.FindByEmailAsync(email);
                    await _signInManager.SignInAsync(user!, isPersistent: false);

                    // Bygg displayName från Claims (om tillgängligt)
                    var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
                    var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname);
                    var displayName =
                        !string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName)
                        ? $"{firstName} {lastName}"
                        : email;

                    var joinedNotification = new NotificationDto
                    {
                        ImageUrl = "/img/default-user.svg",
                        Message = $"{displayName} has joined, say hi!",
                        Timestamp = DateTime.UtcNow,
                        NotificationType = "UserJoined"
                        // NotificationId fylls per användare i SendNotificationAsync
                    };

                    var allMembers = await _memberService.GetAllMembersAsync();
                    foreach (var member in allMembers)
                    {
                        await _notificationService
                            .SendNotificationAsync(member.Email, joinedNotification);
                    }

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
