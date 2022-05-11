using Models.DTOs;
using Models.Models;

namespace DomainObject.Interfaces
{
    public interface IAuthDomainObject
    {
        string Register(User user);
        AuthResult Login(User user, UserDto request);
    }
}
