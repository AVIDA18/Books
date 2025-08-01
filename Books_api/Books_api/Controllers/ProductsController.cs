using Books_api.AppLogics;
using Books_api.Data;
using Books_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Books_api.Controllers
{
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductsClass _productsClass;
        private readonly IConfiguration _configuration;

        public ProductsController(IConfiguration configuration, ProductsClass productsClass)
        {
            _configuration = configuration;
            _productsClass = productsClass;
        }


        /// <summary>
        /// It saves and edits the product. If ProductId IS NOT NULL then it edits the product of that product Id else it
        /// saves product as new.
        /// Product Category is also saved using this. If IsParentProduct is true then it saves the product as Product Category
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/SaveEditProduct")]
        public async Task<IActionResult> SaveProduct([FromForm] ProductParameters parameters)
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

            try
            {
                // Validate image format
                if (parameters.ImageFile != null && parameters.ImageFile.Length > 0)
                {
                    var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png" };
                    if (!allowedTypes.Contains(parameters.ImageFile.ContentType.ToLower()))
                    {
                        return BadRequest("Only JPG, JPEG, and PNG formats are allowed.");
                    }

                    using var ms = new MemoryStream();
                    await parameters.ImageFile.CopyToAsync(ms);
                    parameters.Image = ms.ToArray(); // Set byte[] image for DB
                }

                var result = await _productsClass.SaveEditProducts(parameters);

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
        /// It lists the product Categories on the basis of status if the category is active or not
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = "SelectAllProductCategories")]
        [Route("api/SelectAllProductCategories")]
        public async Task<IActionResult> SelectProductCategories(ListProductCategoriesParameters parameter)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest(Status.InvalidUser);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(Status.InvalidParameters);
            }

            var result = await _productsClass.SelectAllProductCategories(parameter);

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
        /// This lists the product on the basis of filters. It also sets the pagination and number of products on that page.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("api/SelectAllProducts")]
        public async Task<IActionResult> SelectProducts(ListProductParameters parameter)
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

            var result = await _productsClass.SelectAllProducts(parameter);

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
        /// This does both searching and sorting product with pagination in results.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("api/SearchAndSortProducts")]
        public async Task<IActionResult> SearchAndSortProducts( [FromBody] SearchAndSortProductParameters parameter)
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

            var result = await _productsClass.ListBySearchAndSort(parameter);

            if (result != null && result.Count > 0)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(Status.DataNotFound);
            }
        }
    }
}
