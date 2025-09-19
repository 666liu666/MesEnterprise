using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesEnterprise.Auth
{
    internal class IJwtService
    {
        string GenerateToken(ApplicationUser user, IEnumerable<string> roles, IEnumerable<string> permissions);
    }

}
