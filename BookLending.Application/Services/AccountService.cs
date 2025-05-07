using BookLending.Application.DTOs;
using BookLending.Application.Interfaces;
using BookLending.Domain.Entities;
using BookLending.Infrastructure.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IOptions<JwtConfig> _jwtConfig;

        public AccountService(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<JwtConfig> jwtConfig
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtConfig = jwtConfig;
        }

        public async Task<string> RegisterAsync(CreateUserDto newuser)
        {
            var existingUser = await _userManager.FindByEmailAsync(newuser.Email);
            if (existingUser != null)
                throw new Exception("User already exists");

            var user = new AppUser
            {
                UserName = newuser.Email.Split('@')[0],
                Email = newuser.Email,
            };

            var userCreationresult = await _userManager.CreateAsync(user, newuser.Password);
            if (!userCreationresult.Succeeded)
                throw new Exception("User registration failed");
           
            if(await _roleManager.RoleExistsAsync("Member"))
            {
                var roleCreationResult = await _userManager.AddToRoleAsync(user, "Member");
                if (!roleCreationResult.Succeeded)
                    throw new Exception("Failed to assign role to user");
            }

            return await CreateTokenAsync(user);
        }
        public async Task<string> LoginAsync(LoginDto user)
        {
            var existingUser = await _userManager.FindByEmailAsync(user.Email);
            var passwordCheck = await _userManager.CheckPasswordAsync(existingUser, user.Password);

            if (existingUser == null || !passwordCheck)
                throw new UnauthorizedAccessException("Invalid email or password");
    
            return await CreateTokenAsync(existingUser);
        }
        private async Task<string> CreateTokenAsync(AppUser user)
        {
            // private claims (user defined)
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
                authClaims.Add(new Claim(ClaimTypes.Role, role));

            // public claims (standard)
            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Value.Key));
            var token = new JwtSecurityToken(
                issuer: _jwtConfig.Value.Issuer,
                audience: _jwtConfig.Value.Audience,
                claims: authClaims,
                expires: DateTime.UtcNow.AddMinutes(_jwtConfig.Value.ExpirationMinutes),
                signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
