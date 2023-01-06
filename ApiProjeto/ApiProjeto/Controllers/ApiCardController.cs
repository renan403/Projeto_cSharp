using ApiMvc.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
        public async Task<IActionResult> SalvarCartao([FromRoute] string userId, [FromForm] string jsonCartao)
        {
            ModelCartao cartao = JsonConvert.DeserializeObject<ModelCartao>(jsonCartao) ?? new ModelCartao();
            var resp = await _card.SalvarCard(userId, cartao);
            return resp.Length > 0 ? Ok(resp) : BadRequest();
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
            return rt ? Ok(rt) : Forbid(rt.ToString());
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
            try
            {
                return Ok(await _card.MudarPadraoCard(userId, key));
            }
            catch (Exception)
            {
                return BadRequest(false);
            }
            
        }
        [HttpGet("Card/{userId}/{keyCard}")]
        public async Task<IActionResult> Cartao([FromRoute] string userId, [FromRoute] string keyCard)
        {
            try
            {
                return Ok(await _card.Cartao(userId, keyCard));
            }
            catch (Exception)
            {
                return BadRequest(new ModelCartao());
            }
            
        }
        [HttpPatch("AlterCard/{userId}")]
        public async Task<IActionResult> AlterarCartao([FromRoute] string userId, [FromForm] string cartao, [FromForm] string nome, [FromForm] string data)
        {
            try
            {
                var rt = await _card.AlterarCard(userId, cartao, nome, data);
                return Ok(rt);
            }
            catch (Exception)
            {
                return BadRequest();
            }
            
        }
    }
}
