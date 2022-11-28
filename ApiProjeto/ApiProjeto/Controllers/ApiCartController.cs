using ApiMvc.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ApiProjeto.Controllers
{
    [ApiController]
    [Route("Api/v1/Cart")]
    public class ApiCartController : ControllerBase
    {
        ICartRepository _cart;
        public ApiCartController(ICartRepository cart)
        {
            _cart = cart;
        }

        [HttpGet("ReturnCart/{userId}")]
        public async Task<IActionResult> ReturnCart([FromRoute] string userId)
        {
            var rt = await _cart.RetornaCarrinho(userId);
            return Ok(rt);
        }
    }
}
