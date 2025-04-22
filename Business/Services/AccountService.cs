
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

        public async Task<IdentityResult> RegisterAsync(RegisterDto registerDto)
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

            var result = await _userManager.CreateAsync(memberEntity, registerDto.Password ?? "ExternalLogin123!");
            if (!result.Succeeded)
                return result;

            await _userManager.AddToRoleAsync(memberEntity, "User");
            return result;
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task AddLoginAsync(string email, ExternalLoginInfo loginInfo)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                await _userManager.AddLoginAsync(user, loginInfo);
            }
        }

    }
}
