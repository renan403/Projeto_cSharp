using ApiMvc.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ApiProjeto.Controllers
{
    [ApiController]
    [Route("Api/v1/Purchase")]
    public class ApiPurchaseController : ControllerBase
    {

        private readonly IPurchaseRepository _purch;
        public ApiPurchaseController(IPurchaseRepository purch)
        {
            _purch = purch;
        }
        [HttpPost("AddOneItem/{idUser}")]
        public async Task<IActionResult> AddItemUnico([FromRoute] string idUser,[FromForm] ModelProduto produto)
        {
            await _purch.AddItemUnico(idUser, produto);
            return Ok();
        }
        [HttpPatch("ChangeSingleItem/{idUser}")]
        public async Task<IActionResult> AlterarItemUnico([FromRoute]string idUser, [FromForm] string quantidade)
        {
            await _purch.AlterarItemUnico(idUser, int.Parse(quantidade));
            return Ok();
        }
        [HttpGet("PickUpProduct/{idUser}")]
        public async Task<IActionResult> PegarProduto([FromRoute] string idUser)
        {
            var prod = await _purch.PegarProduto(idUser);
            if (prod != null)
                return Ok(prod);
            return NoContent();
        }
    }
}
