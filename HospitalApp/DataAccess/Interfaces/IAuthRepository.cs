using Models.Models;

namespace DataAccess.Interfaces
{
    public interface IAuthRepository
    {
        User Login(User user);
        string Register(User user);
    }
}
