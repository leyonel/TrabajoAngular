using DomainObject.Interfaces;
using System.Security.Cryptography;
using Models.DTOs;
using Models.Models;
using DataAccess.Interfaces;

namespace DomainObject.DomainObjects
{
    public class AuthDomainObject : IAuthDomainObject
    {
        private readonly IAuthRepository _authRepository;
        private readonly IAuthService _authService;
        private readonly IRefreshToken _refreshToken;

        public AuthDomainObject(IAuthRepository authRepository, IAuthService authService, 
            IRefreshToken refreshToken) 
        { 
            _authRepository = authRepository;
            _authService = authService;
            _refreshToken = refreshToken;
        }

        public AuthResult Login(User userCheck, UserDto request)
        {
            User dbUser = _authRepository.Login(userCheck);

            if (dbUser.Email != request.Email) return null;
       

            if (!VerifyPasswordHash(request.Password, dbUser.PasswordHash, dbUser.PasswordSalt)) return null;
         

            AuthResult response = _authService.GenerateToken(dbUser);

            if (response.Token.Length == 0) return null;

            _refreshToken.SaveLoginToken(response, dbUser);

            return response;
        }

        public string Register(User user)
        {
            CreatePasswordHash(user.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            return _authRepository.Register(user);
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            return computedHash.SequenceEqual(passwordHash);
        }
    }
}
