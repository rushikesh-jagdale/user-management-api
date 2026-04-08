using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace UserManagement.Infrastructure.Security;

public class TenantAccessRequirement : IAuthorizationRequirement
{
}
