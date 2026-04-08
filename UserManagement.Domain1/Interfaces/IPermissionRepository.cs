using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IPermissionRepository
{
    Task<List<string>> GetPermissionsByUserIdAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken);
}
