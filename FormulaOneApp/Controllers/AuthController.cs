using FormulaOneApp.Configurations;
using FormulaOneApp.Models;
using FormulaOneApp.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FormulaOneApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // api/auth
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;

        public AuthController(UserManager<IdentityUser> userManager, IOptions<JwtConfig> configOptions)
        {
            _userManager = userManager;
            _jwtConfig = configOptions.Value;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto request)
        {
            // validate the incoming request
            if(ModelState.IsValid)
            {
                // check if user already exists
                var user_exists = await _userManager.FindByEmailAsync(request.Email);

                if(user_exists != null)
                {
                    return BadRequest(new AuthResult()
                    {
                        Errors = new List<string>()
                        {
                            "Email already exists. please login"
                        },
                        Result = false
                    });
                }

                // create new user row in db
                var newUser = new IdentityUser()
                {
                    Email = request.Email,
                    UserName = request.Name
                };

                var userCreationResult = await _userManager.CreateAsync(newUser, request.Password);

                if(userCreationResult.Succeeded)
                {
                    // register success
                    return Ok("Registration success");
                }

                return StatusCode(500, new AuthResult()
                {
                    Errors = userCreationResult.Errors.Select(err => err.Description).ToList(),
                    Result = false
                });
            }

            return BadRequest();
        }

        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login(UserLoginRequestDto request)
        {
            // validate request payload
            if (ModelState.IsValid)
            {
                // validate if user exists
                var user = await _userManager.FindByEmailAsync(request.Email);

                if(user == null)
                {
                    return BadRequest(new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>()
                        {
                            "user does not exists"
                        }
                    });
                }

                // validate password

                var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

                if (isPasswordValid)
                {
                    var token = GenerateJwtToken(user);
                    return Ok(new AuthResult()
                    {
                        Result = true,
                        Token = token
                    });
                }

                return BadRequest(new AuthResult()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        "invalid credentials"
                    }
                });
            }

            return BadRequest(new AuthResult()
            {
                Result = false,
                Errors = new List<string>()
                {
                    "invalid payload"
                }
            });
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())
                }),

                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }
    }
}
