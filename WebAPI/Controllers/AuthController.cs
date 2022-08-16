using Business.Abstract;
using Entities.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public IActionResult Register(UserForRegisterDto userForRegisterDto)
        {


            var userToRegister = _authService.Register(userForRegisterDto);
            if (!userToRegister.Success)
            {
                return BadRequest(userToRegister);
            }
            var accessToken = _authService.CreateAccessToken(userToRegister.Data);
            if (!accessToken.Success)
            {
                return BadRequest(accessToken);
            }
            return Ok(accessToken);
        }

        [HttpPost("login")]
        public IActionResult Login(UserForLoginDto userForLoginDto)
        {
            var userToLogin = _authService.Login(userForLoginDto);
            if (!userToLogin.Success)
            {
                return BadRequest(userToLogin);
            }
            var accessToken = _authService.CreateAccessToken(userToLogin.Data);
            if (!accessToken.Success)
            {
                return BadRequest(accessToken);
            }
            return Ok(accessToken);

        }


    }
}
