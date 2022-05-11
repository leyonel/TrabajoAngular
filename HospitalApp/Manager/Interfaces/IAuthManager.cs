using Models.DTOs;
using Models.Models;

namespace Manager.Interfaces
{
    public interface IAuthManager
    {
        string Register(UserDto user);
        AuthResult Login(UserDto user);
    }
}
