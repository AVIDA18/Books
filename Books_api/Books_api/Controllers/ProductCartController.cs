using Books_api.Data;
using Books_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Books_api.Controllers
{
    [ApiController]
    public class ProductCartController : ControllerBase
    {
        private readonly ProductCartClass _productCartClass;
        private readonly IConfiguration _configuration;

        public ProductCartController(IConfiguration configuration, ProductCartClass productCartClass)
        {
            _configuration = configuration;
            _productCartClass = productCartClass;
        }

        /// <summary>
        /// This api adds product to cart of the user. Max cart items is 10.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("api/AddProductToCart")]
        public async Task<IActionResult> AddProductToCart([FromBody] AddToCartParams parameter)
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

            var result = await _productCartClass.AddProductsToCart(parameter, userId);

            if (string.IsNullOrEmpty(result) || result == "-1")
            {
                return Ok();
            }
            else
            {
                return Problem(result);
            }
        }

        /// <summary>
        /// This loads the total cart items.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("api/SelectAllCartItems")]
        public async Task<IActionResult> SelectCartItems()
        {
            //var userIdClaim = User.FindFirst("nameidentifier");
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest(Status.InvalidUser);
            }

            var result = await _productCartClass.SelectAllCartProducts(userId);

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
        /// It lets us increase or decrease the total count of a product in the cart.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("api/UpdateProductCountCart")]
        public async Task<IActionResult> UpdateProductCountCart([FromBody] UpdateCartProductCount parameter)
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

            var result = await _productCartClass.UpdateCartProductCount(parameter);

            if (string.IsNullOrEmpty(result) || result == "-1")
            {
                return Ok();
            }
            else
            {
                return Problem(result);
            }
        }

        /// <summary>
        /// To remove single item from the cart
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("api/RemoveSingleProductFromCart")]
        public async Task<IActionResult> RemoveSingleProductFromCart([FromBody] UpdateAndDeleteCartProductStatus parameter)
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

            var result = await _productCartClass.RemoveSingleProductFromCart(parameter);

            if (string.IsNullOrEmpty(result) || result == "-1")
            {
                return Ok();
            }
            else
            {
                return Problem(result);
            }
        }

        /// <summary>
        /// Re-Add or renew the product on cart. The product on cart automatically expires in 30 days.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("api/UpdateProductOnCartStatus")]
        public async Task<IActionResult> UpdateProductOnCartStatus([FromBody] UpdateAndDeleteCartProductStatus parameter)
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

            var result = await _productCartClass.UpdateCartProductStatus(parameter);

            if (string.IsNullOrEmpty(result) || result == "-1")
            {
                return Ok();
            }
            else
            {
                return Problem(result);
            }
        }

        /// <summary>
        /// This api removes all the items added to cart by logged in user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("api/RemoveAllProductFromCart")]
        public async Task<IActionResult> RemoveAllProductFromCart()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest(Status.InvalidUser);
            }

            var result = await _productCartClass.RemoveAllProductFromCart(userId);

            if (string.IsNullOrEmpty(result) || result == "-1")
            {
                return Ok();
            }
            else
            {
                return Problem(result);
            }
        }
    }
}