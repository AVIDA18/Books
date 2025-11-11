using Books_api.AppLogics;
using Books_api.Data;
using Books_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Books_api.Controllers
{
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UsersClass _usersClass;
        private readonly IConfiguration _configuration;

        public UsersController(IConfiguration configuration, UsersClass usersClass)
        {
            _configuration = configuration;
            _usersClass = usersClass;
        }


        /// <summary>
        /// This api is used to create a new user and the username and password created here can be used for login.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("api/RegisterUser")]
        public async Task<IActionResult> RegisterUser(UsersParameters parameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Status.InvalidParameters);
            }
            parameters.Password = SecurityClass.GetBcryptHash(parameters.Password);

            try
            {
                var result = await _usersClass.RegisterUsers(parameters);

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

        /// <summary>
        /// This is for admin to see all the users. We can filter users on the basis of roles too.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = "SelectAllUsers")]
        [Route("api/SelectAllUsers")]
        public async Task<IActionResult> SelectUsers(ListUsersParameter parameter)
        {
            //var userIdClaim = User.FindFirst("nameidentifier");
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest(Status.InvalidUser);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(Status.InvalidParameters);
            }

            var result = await _usersClass.SelectAllUsers(parameter);

            if (result != null && result.Count > 0)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(Status.DataNotFound);
            }
        }


        /// <summary>
        /// This api can be used by admin to manage user Role and status can only be done by admin.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Route("api/EditUserRoleAndStatus")]
        //public async Task<IActionResult> EditUserRoleAndStatus(UsersRoleSetParameters parameters)
        //{
        //    var userIdClaim = User.FindFirst("user_id");
        //    if (userIdClaim == null ||
        //        !int.TryParse(userIdClaim.Value, out int userId) ||
        //        userId != 1)
        //    {
        //        return BadRequest(Status.InvalidUser);
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(Status.InvalidParameters);
        //    }

        //    try
        //    {
        //        var result = await _usersClass.SetUsersRole(parameters);

        //        if (string.IsNullOrEmpty(result) || result == "-1")
        //        {
        //            return Ok();
        //        }
        //        else
        //        {
        //            return Problem(result);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Problem(ex.Message);
        //    }
        //}
    }
}
