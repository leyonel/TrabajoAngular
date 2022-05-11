using DomainObject.Interfaces;
using Manager.Interfaces;
using Models.DTOs;
using Models.Models;

namespace Manager.Managers
{
    public class RefreshTokenManager : IRefreshTokenManager
    {
        private readonly IAuthService _authService;

        public RefreshTokenManager(IAuthService authService)
        {
            _authService = authService;
        }

        public AuthResult VerifyAndGenerateToken(TokenRequestDto tokenRequest)
        {
            try
            {
                return _authService.VerifyAndGenerateToken(tokenRequest);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
