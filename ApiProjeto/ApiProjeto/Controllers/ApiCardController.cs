using ApiMvc.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ApiProjeto.Controllers
{
    [ApiController]
    [Route("Api/v1/Card")]
    public class ApiCardController : ControllerBase
    {
        private readonly ICardRepository _card;
        public ApiCardController(ICardRepository icard)
        {
            _card = icard;
        }
        [HttpPost("SaveCard/{userId}")]
        public async Task<IActionResult> SalvarCartao([FromRoute] string userId, [FromForm] ModelCartao cartao)
        {
            var resp = await _card.SalvarCard(userId, cartao);
            return Ok(resp);
        }
        [HttpGet("ReturnCard/{userId}")]
        public async Task<IActionResult> RetornarCartao([FromRoute] string userId)
        {
            var resp = await _card.ReturnCard(userId);
            return Ok(resp);
        }
        [HttpDelete("DeleteCard/{userId}/{card}")]
        public async Task<IActionResult> DeletarCartao([FromRoute] string userId, [FromRoute] string card)
        {
            var rt = await _card.DeleteCard(userId, card);
            return Ok(rt);
        }
        [HttpGet("ReturnPatternCard/{userId}")]
        public async Task<IActionResult> RetornarCartaoPadrao([FromRoute] string userId)
        {
            var rt = await _card.RetornarCartaoPadrao(userId);
            return Ok(rt);
        }
        [HttpPatch("ChangePatternCard/{userId}")]
        public async Task<IActionResult> MudarCartaoPadrao([FromRoute] string userId, [FromForm] string key)
        {
            var rt = await _card.MudarPadraoCard(userId,key);
            return Ok(rt);
        }
        [HttpGet("Card/{userId}/{keyCard}")]
        public async Task<IActionResult> Cartao([FromRoute] string userId, [FromRoute] string keyCard)
        {
            var rt = await _card.Cartao(userId, keyCard);
            return Ok(rt);
        }
        [HttpPatch("AlterCard/{userId}")]
        public async Task<IActionResult> AlterarCartao([FromRoute] string userId, [FromForm] string cartao, [FromForm] string nome, [FromForm] string data)
        {
            var rt = await _card.AlterarCard(userId, cartao, nome, data);
            return Ok(rt);
        }
    }
}
