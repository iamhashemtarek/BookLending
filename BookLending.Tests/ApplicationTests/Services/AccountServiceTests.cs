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
            _accountService = new AccountService(_userManager, _roleManager, _jwtConfig);

        }

        [Fact]
        public async Task RegisterAsync_ShouldCreateUserAndReturnToken_WhenValidData()
        {
            // Arrange
            var dto = new CreateUserDto
            {
                Email = "test@example.com",
                Password = "Test123!"
            };

            _userManager.FindByEmailAsync(dto.Email).Returns((AppUser)null);

            var createdUser = new AppUser { Email = dto.Email, UserName = "test" };
            _userManager.CreateAsync(Arg.Any<AppUser>(), dto.Password)
                        .Returns(IdentityResult.Success);

            _roleManager.RoleExistsAsync("Member").Returns(true);
            _userManager.AddToRoleAsync(Arg.Any<AppUser>(), "Member")
                        .Returns(IdentityResult.Success);

            _accountService.CreateTokenAsync(Arg.Any<AppUser>())
                           .Returns("fake-jwt-token");


            // Act
            var result = await _accountService.RegisterAsync(dto);

            // Assert
            Assert.Equal("fake-jwt-token", result);

            await _userManager.Received(1).CreateAsync(Arg.Any<AppUser>(), dto.Password);
            await _userManager.Received(1).AddToRoleAsync(Arg.Any<AppUser>(), "Member");
            await _accountService.Received(1).CreateTokenAsync(Arg.Any<AppUser>());
        }
    }
}
