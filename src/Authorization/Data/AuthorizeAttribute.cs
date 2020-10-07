using System;
using Microsoft.AspNetCore.Authorization;

namespace AspNetCore.Advanced.Authorization
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class DataAuthorizeAttribute: AuthorizeAttribute
    {
    }
}
