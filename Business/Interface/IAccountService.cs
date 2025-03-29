using Business.Dtos;

namespace Business.Interface;

public interface IAccountService
{
    Task<bool> LoginAsync(LoginDto loginDto);
    Task<bool> RegisterAsync(RegisterDto registerDto);
    Task SignOutAsync();
}