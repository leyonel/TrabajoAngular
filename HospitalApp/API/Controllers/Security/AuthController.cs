using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Manager.Interfaces;
using Models.DTOs;
using Models.Models;

namespace API.Controllers
{
    [AllowAnonymous]
    public class AuthController : BaseApiController
    {
        private readonly IAuthManager _authServiceManager;
        private readonly IRefreshTokenManager _refreshTokenManager;

        public AuthController(IAuthManager authServiceManager, IRefreshTokenManager refreshTokenManager)
        {
            _authServiceManager = authServiceManager;
            _refreshTokenManager = refreshTokenManager;
        }

        [HttpPost("register")]
        public ActionResult Register([FromBody] UserDto request)
        {
            string response = _authServiceManager.Register(request);

            if (response == "Registed Successfully") return Ok(response);
            if (response == "Email was taken") return Ok(response);

            return BadRequest(response);
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] UserDto request)
        {
            AuthResult response = _authServiceManager.Login(request);

            if (response == null) return Ok("Email or password incorrect");

            UserReturnDto tokenResponse = new () { Token = response.Token, RefreshToken = response.RefreshToken };

            return Ok(tokenResponse);
        }

        [HttpPost]
        [Route("RefreshToken")]
        public ActionResult RefreshToken([FromBody] TokenRequestDto tokenRequest)
        {
            if(ModelState.IsValid)
            {
                var result = _refreshTokenManager.VerifyAndGenerateToken(tokenRequest);

                if(result == null)
                {
                    return BadRequest(new AuthResult()
                    {
                        Errors = new List<string>()
                        {
                            "Invalid payload"
                        },
                        Success = false
                    });
                }

                UserReturnDto tokenResponse = new() { Token = result.Token, RefreshToken = result.RefreshToken };

                return Ok(tokenResponse);
            }

            return BadRequest(new AuthResult()
            {
                Errors = new List<string>()
                {
                    "Invalid payload"
                },
                Success = false
            });
        }
    }
}
