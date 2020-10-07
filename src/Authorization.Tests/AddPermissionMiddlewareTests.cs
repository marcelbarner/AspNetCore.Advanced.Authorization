using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.Advanced.Authorization;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;

namespace Authorization.Tests
{
    public class AddPermissionMiddlewareTests
    {
        [Fact]
        public async Task Should_Handle_Null_User()
        {
            var requestDelegate = new RequestDelegate((innerContext) => Task.FromResult(0));
            var middleware = new AddPermissionMiddleware(requestDelegate, new List<IPermissionSource>(), null);

            var context = new Mock<HttpContext>();
            context.Setup(c => c.User).Returns((ClaimsPrincipal)null);
            using(new AssertionScope())
            {
                context.Object.User.Should().BeNull();

                Func<Task> action = async () => await middleware.InvokeAsync(context.Object);

                await action.Should().NotThrowAsync();
            }
        }

        [Fact]
        public async Task Should_Handle_Null_Sources()
        {
            var requestDelegate = new RequestDelegate((innerContext) => Task.FromResult(0));
            var middleware = new AddPermissionMiddleware(requestDelegate, null, null);

            var context = new DefaultHttpContext();
            var identity = new Mock<ClaimsIdentity>();
            identity.Setup(c => c.IsAuthenticated).Returns(true);
            var user = new ClaimsPrincipal(identity.Object);
            context.User = user;

            using (new AssertionScope())
            {
                Func<Task> action = async () => await middleware.InvokeAsync(context);

                await action.Should().NotThrowAsync();
            }
        }

        [Fact]
        public async Task Should_Handle_Empty_List_Of_Sources()
        {
            var requestDelegate = new RequestDelegate((innerContext) => Task.FromResult(0));
            var middleware = new AddPermissionMiddleware(requestDelegate, new List<IPermissionSource>(), null);

            var context = new DefaultHttpContext();
            var identity = new Mock<ClaimsIdentity>();
            identity.Setup(c => c.IsAuthenticated).Returns(true);
            var user = new ClaimsPrincipal(identity.Object);

            context.User = user;

            Func<Task> action = async () => await middleware.InvokeAsync(context);

            await action.Should().NotThrowAsync();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(10)]
        public async Task Should_Add_One_Or_Multiple_Permissions_From_One_PertmissionSource_If_User_Is_Authenticated(int numberOfPermissions)
        {
            var permissionSource = new Mock<IPermissionSource>();
            var permissions = new List<string>();

            for(var i = 0; i < numberOfPermissions; i++)
            {
                permissions.Add($"test-permission{i}");
            }

            permissionSource.Setup(c => c.GetPermissions(It.IsAny<HttpContext>())).Returns(permissions);

            var requestDelegate = new RequestDelegate((innerContext) => Task.FromResult(0));

            var middleware = new AddPermissionMiddleware(requestDelegate, new List<IPermissionSource>() { permissionSource.Object }, null);

            var context = new DefaultHttpContext();
            var identity = new Mock<ClaimsIdentity>();
            identity.Setup(c => c.IsAuthenticated).Returns(true);
            var user = new ClaimsPrincipal(identity.Object);

            context.User = user;

            await middleware.InvokeAsync(context);

            using (new AssertionScope())
            {
                for (var i = 0; i < numberOfPermissions; i++)
                {
                    context.User.Claims.Should().Contain(c => c.Type.Equals("permissions") && c.Value.Equals($"test-permission{i}"));
                }
            }
            
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(10)]
        public async Task Should_Add_One_Permission_From_Each_PertmissionSource_If_User_Is_Authenticated(int numberOfSources)
        {
            var permissionSources = new List<IPermissionSource>();
            for (var i = 0; i < numberOfSources; i++)
            {
                var permissionSource = new Mock<IPermissionSource>();
                permissionSource.Setup(c => c.GetPermissions(It.IsAny<HttpContext>())).Returns(new List<string>() { $"test-permission{i}" });
                permissionSources.Add(permissionSource.Object);
            }

            var requestDelegate = new RequestDelegate((innerContext) => Task.FromResult(0));

            var middleware = new AddPermissionMiddleware(requestDelegate, permissionSources, null);

            var context = new DefaultHttpContext();
            var identity = new Mock<ClaimsIdentity>();
            identity.Setup(c => c.IsAuthenticated).Returns(true);
            var user = new ClaimsPrincipal(identity.Object);

            context.User = user;

            await middleware.InvokeAsync(context);
            using (new AssertionScope())
            {
                for (var i = 0; i < numberOfSources; i++)
                {
                    context.User.Claims.Should().Contain(c => c.Type.Equals("permissions") && c.Value.Equals($"test-permission{i}"));
                }
            }
        }
    }
}
