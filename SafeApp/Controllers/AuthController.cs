using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SafeApp.DataAccess.Services;
using SafeApp.Dtos;
using SafeApp.Models;
using SafeApp.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SafeApp.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILoginAttemptService _loginAttemptService;
        private readonly ITokenGenerator _tokenGenerator;
        public AuthController(IAccountService accountService, ITokenGenerator tokenGenerator, ILoginAttemptService loginAttemptService)
        {
            _accountService = accountService;
            _tokenGenerator = tokenGenerator;
            _loginAttemptService = loginAttemptService;
        }

        [HttpPost, Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto credentials)
        {
            if (await _loginAttemptService.CheckIfLoginAttemptShouldBeBlocked(credentials.UserName))
            {
                return BadRequest("Too many attempt to log in. Try again later.");
            }

            var user = await _accountService.GetUser(credentials.UserName);

            if (user is null)
            {
                await _loginAttemptService.AddLoginAttempt(credentials.UserName);
                return BadRequest("Invalid username or password.");
            }

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(credentials.Password));

            for(int i = 0; i < computedHash.Length; ++i)
            {
                if (computedHash[i] != user.PasswordHash[i])
                {
                    await _loginAttemptService.AddLoginAttempt(credentials.UserName);
                    return Unauthorized("Invalid username or password.");
                }
            }

            return Ok(new { Token = _tokenGenerator.GenerateToken(credentials.UserName) });
        }

        [HttpPost, Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto user)
        {
            if (await _accountService.GetUser(user.UserName) is not null) return BadRequest("Invalid username or password.");
            if (user.Password.Length < 8) return BadRequest("Invalid username or password.");

            using var hmac = new HMACSHA512();

            var newAccount = new UserModel
            {
                UserName = user.UserName,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(user.Password)),
                PasswordSalt = hmac.Key
            };

            await _accountService.AddAccount(newAccount);

            return Ok(new { Token = _tokenGenerator.GenerateToken(newAccount.UserName) });
        }
    }
}
