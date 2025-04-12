using Books_api.AppLogics;
using Books_api.Data;
using Books_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Books_api.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly LoginClass _loginClass;

        public LoginController(IConfiguration configuration, LoginClass loginClass)
        {
            _configuration = configuration;
            _loginClass = loginClass;
        }

        [Route("api/Login")]
        [HttpPost]
        public async Task<IActionResult> Login(LoginParameters parameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Status.InvalidParameters);
            }

            if (!string.IsNullOrWhiteSpace(parameters.UserName) && !string.IsNullOrWhiteSpace(parameters.Password))
            {
                var listUserDetails = await _loginClass.GetUserData(parameters.UserName);

                if (listUserDetails != null)
                {
                    bool response = SecurityClass.VerifyBcryptHash(parameters.Password, listUserDetails.Password);
                    if (response == false)
                    {
                        return NotFound(Status.DataNotFound);
                    }
                    else
                    {
                        /*If user is valid then generate token using JWT*/
                        var tokenOptions = _configuration.GetSection("JWT-TokenOptions").Get<JWTTokenOptions>();
                        var key = Encoding.ASCII.GetBytes(tokenOptions.IssuerSigningKey);

                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(new[]
                            {
                                new Claim("user_id", listUserDetails.UserId.ToString(), ClaimValueTypes.Integer32),
                                new Claim(ClaimTypes.Role, Convert.ToString(listUserDetails.RoleType)),
                            }),
                            Expires = DateTime.UtcNow.AddDays(tokenOptions.ExpiryDays),
                            Issuer = tokenOptions.ValidIssuer,
                            Audience = tokenOptions.ValidAudience,
                            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                        };

                        var tokenHandler = new JwtSecurityTokenHandler();
                        var token = tokenHandler.CreateToken(tokenDescriptor);

                        return Ok(new { token = tokenHandler.WriteToken(token) });
                    }

                }
                else
                {
                    return NotFound(Status.DataNotFound);
                }
            }
            else
            {
                return NotFound(Status.InvalidParameters);
            }
        }
    }
}
