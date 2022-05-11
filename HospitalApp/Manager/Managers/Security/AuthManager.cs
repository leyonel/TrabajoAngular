using DomainObject.Interfaces;
using Manager.Interfaces;
using Models.DTOs;
using Models.Models;

namespace Manager.Managers
{
    public class AuthManager : IAuthManager
    {
        private readonly IAuthDomainObject _authDomainObject;

        public AuthManager(IAuthDomainObject authDomainObject)
        {
            _authDomainObject = authDomainObject;
        }

        public AuthResult Login(UserDto request)
        {
            User userCheck = new();

            try
            {
                userCheck.Email = request.Email;
                userCheck.Password = request.Password;

                AuthResult response = new();
                response = _authDomainObject.Login(userCheck, request);

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string Register(UserDto user)
        {
            try
            {
                User newUser = new()
                {
                    Email = user.Email,
                    Password = user.Password,
                };

                return _authDomainObject.Register(newUser);
            }
            catch (Exception ex)
            {
                string message = ex.Message;

                return message;
            }
        }
    }
}
