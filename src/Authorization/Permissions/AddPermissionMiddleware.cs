using AspNetCore.Advanced.Authorization.Permissions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Advanced.Authorization
{
    public class AddPermissionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IEnumerable<IPermissionSource> _permissionSources;
        private readonly IEnumerable<IPermissionSourceAsync> _permissionSourceAsyncs;

        public AddPermissionMiddleware(RequestDelegate next, IEnumerable<IPermissionSource> permissionSources, IEnumerable<IPermissionSourceAsync> permissionSourceAsyncs)
        {
            _next = next;
            _permissionSources = permissionSources;
            _permissionSourceAsyncs = permissionSourceAsyncs;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context?.User is null || !context.User.Identity.IsAuthenticated)
            {
                await _next(context);
                return;
            }

            if (_permissionSources is null)
            {
                await _next(context);
                return;
            }

            var permissionSets = _permissionSources.Select(x => x.GetPermissions(context));

            var identities = permissionSets.Select(x => new ClaimsIdentity(x.Select(y => new Claim("permissions", y))));
            context.User.AddIdentities(identities);
            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
