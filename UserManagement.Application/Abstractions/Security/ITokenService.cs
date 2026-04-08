using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Application.Abstractions.Security;

public interface ITokenService
{
    string GenerateAccessToken(
     Guid userId,
     string email,
     Guid tenantId,
     IEnumerable<string> roles,
     IEnumerable<string> permissions);

    string GenerateRefreshToken();
}

