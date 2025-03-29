
using Business.Dtos;
using Business.Interface;
using Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace Business.Services
{
    public class AccountService(SignInManager<MemberEntity> signInManager, UserManager<MemberEntity> userManager) : IAccountService
    {
        private readonly SignInManager<MemberEntity> _signInManager = signInManager;
        private readonly UserManager<MemberEntity> _userManager = userManager;


        public async Task<bool> LoginAsync(LoginDto loginDto)
        {
            var result = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, false, false);
            return result.Succeeded;
        }

        public async Task<bool> RegisterAsync(RegisterDto redgisterDto)
        {
            var memberEntity = new MemberEntity
            {
                UserName = redgisterDto.Email,
                FirstName = redgisterDto.FirstName,
                LastName = redgisterDto.LastName,
                Email = redgisterDto.Email,
            };

            var result = await _userManager.CreateAsync(memberEntity, redgisterDto.Password);
            return result.Succeeded;
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

    }
}
