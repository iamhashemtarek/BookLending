using BookLending.Application.DTOs;
using BookLending.Application.Interfaces;
using BookLending.Application.Services;
using BookLending.Domain.Entities;
using BookLending.Infrastructure.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Tests.ApplicationTests.Services
{
    public class AccountServiceTests
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IOptions<JwtConfig> _jwtConfig;
        private readonly IAccountService _accountService;

        public AccountServiceTests()
        {
            var store = Substitute.For<IUserStore<AppUser>>();
            _userManager = Substitute.For<UserManager<AppUser>>(
                store, null, null, null, null, null, null, null, null);

            var roleStore = Substitute.For<IRoleStore<IdentityRole>>();
            _roleManager = Substitute.For<RoleManager<IdentityRole>>(
                roleStore, null, null, null, null);

            _jwtConfig = Substitute.For<IOptions<JwtConfig>>();
            _jwtConfig.Value.Returns(new JwtConfig
            {
                Key = "ThisIsASecretKey1234567890aaaaaaaaaaaaa",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                ExpirationMinutes = 60
            });
            _accountService = new AccountService(_userManager, _roleManager, _jwtConfig);

        }

        [Fact]
        public async Task RegisterAsync_ShouldCreateUserAndReturnToken_OnSuccess()
        {
            // Arrange
            var newUser = new CreateUserDto
            {
                Email = "test@example.com",
                Password = "Test@123"
            };

            _userManager.FindByEmailAsync(newUser.Email).Returns((AppUser)null); 
            _userManager.CreateAsync(Arg.Any<AppUser>(), newUser.Password).Returns(IdentityResult.Success);
            _roleManager.RoleExistsAsync("Member").Returns(true);
            _userManager.AddToRoleAsync(Arg.Any<AppUser>(), "Member").Returns(IdentityResult.Success);
            _userManager.GetRolesAsync(Arg.Any<AppUser>()).Returns(new List<string> { "Member" });

            // Act
            var token = await _accountService.RegisterAsync(newUser);

            // Assert
            Assert.NotNull(token);
            Assert.IsType<string>(token);
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowException_WhenUserExists()
        {
            // Arrange
            var newUser = new CreateUserDto
            {
                Email = "existing@example.com",
                Password = "Test@123"
            };

            _userManager.FindByEmailAsync(newUser.Email).Returns(new AppUser());

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _accountService.RegisterAsync(newUser));
            Assert.Equal("User already exists", ex.Message);
        }

        [Fact]
        public async Task Login_ShouldReturnToken_OnSuccess()
        {
            var loginDto = new LoginDto
            {
                Email = "test@email",
                Password = "test@123"
            };

            // arrange
            _userManager.FindByEmailAsync(Arg.Any<string>()).Returns(new AppUser()
            {
                Id = Guid.NewGuid().ToString(),
                Email = loginDto.Email,
                UserName = loginDto.Email.Split('@')[0]
            });
            _userManager.CheckPasswordAsync(Arg.Any<AppUser>(), Arg.Any<string>()).Returns(true);

            //act
            var token = await _accountService.LoginAsync(loginDto);

            // assert
            Assert.NotNull(token);
            Assert.IsType<string>(token);

        }

        [Fact]
        public async Task Login_ShouldThrowUnuthorizedException_OnFail()
        {
            var loginDto = new LoginDto
            {
                Email = "test@email",
                Password = "test@123"
            };
            // arrange
            _userManager.FindByEmailAsync(Arg.Any<string>()).Returns(new AppUser()
            {
                Id = Guid.NewGuid().ToString(),
                Email = loginDto.Email,
                UserName = loginDto.Email.Split('@')[0]
            });
            _userManager.CheckPasswordAsync(Arg.Any<AppUser>(), Arg.Any<string>()).Returns(false);
            //act
            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _accountService.LoginAsync(loginDto));
            // assert
            Assert.Equal("Invalid email or password", ex.Message);
        }

    }
}
