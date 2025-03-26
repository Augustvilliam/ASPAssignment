using Domain.Models;

namespace Business.Interface
{
    public interface IAccountService
    {
        Task<bool> LoginAsync(LoginForm loginform);
        Task<bool> RegisterAsync(RegisterForm regForm);
        Task SignOutAsync();
    }
}