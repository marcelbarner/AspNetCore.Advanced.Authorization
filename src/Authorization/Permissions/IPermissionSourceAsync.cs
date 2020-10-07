using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.Advanced.Authorization.Permissions
{
    public interface IPermissionSourceAsync
    {
        Task<IEnumerable<string>> GetPermissionsAsync(HttpContext context, CancellationToken cancellationToken);
    }
}
