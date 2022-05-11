using Models.DTOs;
using Models.Models;

namespace DomainObject.Interfaces
{
    public interface IAuthService
    {
        AuthResult GenerateToken(User user);
        AuthResult VerifyAndGenerateToken(TokenRequestDto tokenRequest);
    }
}
