

using Business.Interface;
using Data.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Business.Services
{
    public class AccountService(SignInManager<MemberEntity> signInManager, UserManager<MemberEntity> userManager) : IAccountService
    {
        private readonly SignInManager<MemberEntity> _signInManager = signInManager;
        private readonly UserManager<MemberEntity> _userManager = userManager;


        public async Task<bool> LoginAsync(LoginForm loginForm)
        {
            var result = await _signInManager.PasswordSignInAsync(loginForm.Email, loginForm.Password, false, false);
            return result.Succeeded;
        }

        public async Task<bool> RegisterAsync(RegisterForm regForm)
        {
            var memberEntity = new MemberEntity
            {
                UserName = regForm.Email,
                FirstName = regForm.FirstName,
                LastName = regForm.LastName,
                Email = regForm.Email,
            };

            var result = await _userManager.CreateAsync(memberEntity, regForm.Password);
            return result.Succeeded;
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

    }
}
