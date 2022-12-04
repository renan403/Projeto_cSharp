using ApiMvc.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApiProjeto.Controllers
{
    [ApiController]
    [Route("Api/v1/Receipt")]
    public class ApiReceiptController : ControllerBase
    {
        private readonly IReceiptRepository _IReceipt;

        public ApiReceiptController(IReceiptRepository receipt)
        {
            _IReceipt = receipt;
        }

        [HttpGet("ReturnReceipt/{userId}")]
        public async Task<IActionResult> RetornarNotaFiscal([FromRoute] string userId)
        {
            var rt = await _IReceipt.RetornarNotaFiscal(userId);
            return Ok(rt);
        }
        [HttpGet("GetReceipt/{userId}/{nota}")]
        public async Task<IActionResult> PegarNotaFiscal([FromRoute]string userId, [FromRoute] string nota)
        {
            var rt = await _IReceipt.PegarNotaFiscal(userId,nota);
            return Ok(rt);
        }
        [HttpPost("ConfReceipt/{userId}")]
        public async Task<IActionResult> ConfirmarPedido([FromRoute] string userId, [FromForm] string nome, [FromForm] string nota)
        {
            try
            {
                await _IReceipt.ConfirmarPedido(userId, nota, nome);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
           
            
        }
        [HttpPost("GenerateReceipt/{userId}")]
        public async Task<IActionResult> GerarNotaFiscal([FromRoute] string userId, [FromForm] string notaJson)
        {
            try
            {
                ModelNotaFiscal nota = JsonConvert.DeserializeObject<ModelNotaFiscal>(notaJson);
                await _IReceipt.GerarNotaFiscal(userId,nota);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }


        }

    }
}
