using System.Security.Claims;
using Business.Dtos;
using Business.Interface;
using Business.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ASPAssignment.Controllers;

public class ExternalController(IAccountService accountService, SignInManager<MemberEntity> signInManager) : Controller 
{
    private readonly IAccountService _accountService = accountService;
    private readonly SignInManager<MemberEntity> _signInManager = signInManager;


    [HttpGet]
    public IActionResult ExternalSignIn()
    {
        return View();
    }


    [HttpPost]
    public IActionResult ExternalSignIn(string provider, string returnUrl = null!)
    {
        if (string.IsNullOrEmpty(provider))
        {
            ModelState.AddModelError("", "Invalid Provider");
            return View();
        }

        var redirectUrl = Url.Action("ExternalSignInCallback", "External", new { returnUrl })!;
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }

    public async Task <IActionResult> ExternalSignInCallback(string returnUrl = null!, string remoteError = null!)
    {
        returnUrl ??= Url.Content("~/");

        if (!string.IsNullOrEmpty(remoteError))
        {
            ModelState.AddModelError("", $"Error from external provider: {remoteError}");
            return View("ExternalSignIn");
        }

        var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
        if (externalLoginInfo == null)
        {
            return RedirectToAction("ExternalSignIn");
        }

        var signInResult = await _signInManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey, isPersistent: false, bypassTwoFactor: true);
        if (signInResult.Succeeded)
        {
            return LocalRedirect(returnUrl);
        }
        else
        {
            string email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email)!;
            string username = $"ext_{externalLoginInfo.LoginProvider.ToLower()}_{email}";

            var dto = new RegisterDto
            {
                Email = email,
            };

            var result = await _accountService.RegisterAsync(dto);
            if (result.Succeeded)
            {
                await _accountService.AddLoginAsync(dto.Email, externalLoginInfo);
                
                var user = await _signInManager.UserManager.FindByEmailAsync(dto.Email);
                if ( user != null)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }

            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View("ExternalSignIn");
        }

    }
}
