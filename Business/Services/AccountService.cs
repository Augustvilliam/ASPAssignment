
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

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            var memberEntity = new MemberEntity
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                Profile = new MemberProfileEntity
                {
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName
                }
            };

            var result = await _userManager.CreateAsync(memberEntity, registerDto.Password);
            if (!result.Succeeded)
                return false;

            await _userManager.AddToRoleAsync(memberEntity, "User");
            return true;
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

    }
}
