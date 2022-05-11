using Models.DTOs;
using Models.Models;

namespace Manager.Interfaces
{
    public interface IRefreshTokenManager
    {
        AuthResult VerifyAndGenerateToken(TokenRequestDto tokenRequest);
    }
}
