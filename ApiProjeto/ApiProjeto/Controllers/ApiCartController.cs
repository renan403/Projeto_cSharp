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
        [HttpPost("SaveCart/{userId}")]
        public async Task<IActionResult> SalvarCarrinho([FromRoute] string userId, [FromForm] string idProd, [FromForm] string quantidade)
        {
            var rt = await _cart.SalvarProdCarrinho(userId,idProd,Convert.ToInt32(quantidade));
            return Ok(rt);
        }
        [HttpDelete("DelProdCar/{userId}/{key}")]
        public async Task<IActionResult> DeleteProdCar([FromRoute] string userId, [FromRoute] string key)
        {
            var rt = await _cart.DeleteProdCarrinho( key, userId);
            return Ok(rt);
        }
        [HttpDelete("DelCar/{userId}")]
        public async Task<IActionResult> DeleteProdCar([FromRoute] string userId)
        {
            await _cart.DeletarCarrinho(userId);
            return Ok();
        }
        [HttpGet("HaveInTheCar/{userId}/{idprod}")]
        public async Task<IActionResult> TemNoCarrinho([FromRoute] string userId, [FromRoute] string idprod)
        {
            var rt = await _cart.PossuiNoCarrinho(userId,idprod);
            return Ok(rt);
        }
        [HttpPost("AddQtdCar/{userId}")]
        public async Task<IActionResult> AddQtdNoCar([FromRoute] string userId, [FromForm] string idProd, [FromForm] string quantidade)
        {
            var rt = await _cart.AddQtdCarrinho(userId, idProd, Convert.ToInt32(quantidade));
            return Ok(rt);
        }
        [HttpPatch("AltQtdCar/{userId}")]
        public async Task<IActionResult> AaltQtdNoCar([FromRoute] string userId, [FromForm] string idProd, [FromForm] string quantidade)
        {
            var rt = await _cart.AlterarQtdCarrinho(userId, idProd, Convert.ToInt32(quantidade));
            return Ok(rt);
        }

    }
}
