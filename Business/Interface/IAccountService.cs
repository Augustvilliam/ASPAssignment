using Business.Dtos;
using Microsoft.AspNetCore.Identity;

namespace Business.Interface;

public interface IAccountService
{
    Task AddLoginAsync(string email, ExternalLoginInfo loginInfo);
    Task<bool> LoginAsync(LoginDto loginDto);
    Task<IdentityResult> RegisterAsync(RegisterDto registerDto);
    Task SignOutAsync();
}