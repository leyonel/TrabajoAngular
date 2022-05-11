using Models.Models;

namespace DataAccess.Interfaces
{
    public interface IRefreshToken
    {
        void AddRefreshToken(RefreshToken refreshToken);
        RefreshToken FirstOrDefault(string request);
        void UpdateRefreshToken(RefreshToken request);
        void SaveNewToken(int id, string refreshToken);
        void SaveLoginToken(AuthResult response, User user);
        User FindById(int request);
    }
}
