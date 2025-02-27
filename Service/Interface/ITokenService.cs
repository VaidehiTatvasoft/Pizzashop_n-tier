using System.Collections.Generic;
using System.Security.Claims;
namespace Service.Interface;

public interface ITokenService
{
        string GenerateToken(IEnumerable<Claim> claims);
}
