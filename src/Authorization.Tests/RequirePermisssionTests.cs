using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using FluentAssertions;

namespace Authorization.Tests
{
    public class RequirePermisssionTests
    {
        [Fact]
        public async Task Should_Allow_Access_If_User_Has_Permission()
        {
            // Arrange
            var authorizationService = BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("SomePolicyName", c => c.RequirePermission("test-permission"));
                });
            });
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] { new Claim("permissions", "test-permission") }));

            // Act
            var allowed = await authorizationService.AuthorizeAsync(user, "SomePolicyName");

            // Assert
            allowed.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Forbid_Access_If_User_Has_Permission()
        {
            // Arrange
            var authorizationService = BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("SomePolicyName", c => c.RequirePermission("test-permission"));
                });
            });
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] { new Claim("permissions", "test-permission2") }));

            // Act
            var allowed = await authorizationService.AuthorizeAsync(user, "SomePolicyName");

            // Assert
            allowed.Succeeded.Should().BeFalse();
        }

        private IAuthorizationService BuildAuthorizationService(
    Action<IServiceCollection> setupServices = null)
        {
            var services = new ServiceCollection();
            services.AddAuthorization();
            services.AddLogging();
            services.AddOptions();
            setupServices?.Invoke(services);
            return services.BuildServiceProvider().GetRequiredService<IAuthorizationService>();
        }
    }
}
