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

        /// <summary>
        /// This fetches the password of the user from the databse using userName and checks the
        /// password here.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
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

        /// <summary>
        /// It lets user to change password it takes password as input and changes the password of logged user.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [Route("api/ChangePassword")]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePassword parameters)
        {
            var userIdClaim = User.FindFirst("user_id");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest(Status.InvalidUser);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(Status.InvalidParameters);
            }

            parameters.Password = SecurityClass.GetBcryptHash(parameters.Password);
            parameters.UserId = Convert.ToInt32(User.FindFirst("user_id")?.Value);

            try
            {
                var result = await _loginClass.ChangePassword(parameters);

                if (string.IsNullOrEmpty(result) || result == "-1")
                {
                    return Ok();
                }
                else
                {
                    return Problem(result);
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
