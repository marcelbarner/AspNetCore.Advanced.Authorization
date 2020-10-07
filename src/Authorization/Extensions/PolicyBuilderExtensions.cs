using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace Microsoft.AspNetCore.Authorization
{
    public static class PolicyBuilderExtensions
    {
        public static AuthorizationPolicyBuilder RequirePermission(this AuthorizationPolicyBuilder builder, string permission)
        {
            builder.RequireClaim("permissions", permission);
            return builder;
        }
    }
}
