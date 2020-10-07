using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Advanced.Authorization
{
    public interface IPermissionSource
    {
        IEnumerable<string> GetPermissions(HttpContext context);
    }
}
