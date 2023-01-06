using ApiMvc.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApiMvc.Controllers
{
    [ApiController]
    [Route("Api/v1/Address")]
    public class ApiAddressController : ControllerBase
    {
        private readonly IAddressRepository _address;
        public ApiAddressController(IAddressRepository address)
        {
            _address = address;
        }

        [HttpPut("ChangeDefault/{userId}")]
        public async Task<IActionResult> ChangeDefault(string userId,[FromForm] string key)
        {          
            return await _address.MudarPadrao(userId, key) ? Ok() : NotFound();
        }
       
        [HttpGet("CheckPattern/{idUser}")]
        public async Task<IActionResult> CheckPattern(string userId)
        {
            return await _address.VerificaPadrao(userId) ? Ok() : NotFound();
        }
        [HttpGet("PullAddress/{idUser}")]
        public async Task<IActionResult> PullAddress(string idUser)
        {
            var rt = await _address.PuxarEndereco(idUser);
            if(rt is not null)
                return Ok(rt);
            return NotFound();
        }
        [HttpDelete("DeleteAddress/{idUser}/{key}")]
        public async Task<IActionResult> DeleteAddress(string idUser, string key)
        {
            var rt = await _address.DeleteEndereco(key, idUser);
            if (rt)
                return NoContent();
            return NotFound();
        }
        [HttpPatch("AlterAddress/{idUser}/{key}")]
        public async Task<IActionResult> AlterAddress([FromRoute] string idUser,[FromRoute] string key, [FromForm] string jsonEndereco)
        {
            var endereco = JsonConvert.DeserializeObject<ModelEndereco>(jsonEndereco);
            var rt = await _address.AlterarEndereco(key,idUser, endereco ?? new ModelEndereco());
            if(rt)
                return NoContent();
            return NotFound();
        }
        [HttpGet("ReturnPattern/{idUser}")]
        public async Task<IActionResult> ReturnPattern(string idUser)
        {
            try
            {
                var rt = await _address.RetornaEnderecoPadrao(idUser);
                return Ok(rt);
            }
            catch (Exception)
            {
                return BadRequest(new ModelEndereco());
            }
            
        }
        [HttpPost("SaveAddress/{idUser}")]
        public async Task<IActionResult> SaveAddress([FromRoute]string idUser, [FromForm] ModelEndereco end)
        {
            try
            {
                await _address.SalvarEndereco(idUser, end);
                return Ok();
            }
            catch (Exception e)
            {
                return Forbid($"Erro: {e.Message.ToString()}");
            }
            
        }
    }
}
