using Business.Dtos;
using Business.Interface;
using Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace Business.Services
{
    public class AccountService : IAccountService
    {
        private readonly SignInManager<MemberEntity> _signInManager;
        private readonly UserManager<MemberEntity> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AccountService(
            SignInManager<MemberEntity> signInManager,
            UserManager<MemberEntity> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> LoginAsync(LoginDto loginDto)
        {
            var result = await _signInManager
                .PasswordSignInAsync(loginDto.Email, loginDto.Password, loginDto.RememberMe, false);
            return result.Succeeded;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterDto registerDto)
        {
            //Hämta den standard-rollen "User"
            var userRole = await _roleManager.FindByNameAsync("User");
            if (userRole == null)
                throw new InvalidOperationException("Standard-rollen 'User' är inte konfigurerad!");

            //Bygg MemberEntity med Profile.RoleId redan ifyllt
            var memberEntity = new MemberEntity
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                Profile = new MemberProfileEntity
                {
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    RoleId = userRole.Id
                }
            };

            //Skapa användaren
            var result = await _userManager.CreateAsync(
                memberEntity,
                registerDto.Password ?? "ExternalLogin123!"
            );
            if (!result.Succeeded)
                return result;

            //Lägg till användaren i AspNetUserRoles
            await _userManager.AddToRoleAsync(memberEntity, userRole.Name!);

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
                await _userManager.AddLoginAsync(user, loginInfo);
        }
    }
}
