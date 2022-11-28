using ApiMvc.Repositories;
using Microsoft.AspNetCore.Mvc;

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
        [HttpPut("ChangeStandard/{userId}/{key}")]
        public async Task<IActionResult> ChangeStandard(string userId,string key)
        {          
            return await _address.MudarPadrao(userId, key) ? Ok() : NotFound();
        }
        [HttpDelete("DeleteUser/{userId}/{key}")]
        public async Task<IActionResult> DeleteUser(string userId, string key)
        {
            return await _address.DeleteEndereco(key, userId) ? NoContent() : NotFound();
        }
        [HttpGet("CheckPattern/{userId}/{key}")]
        public async Task<IActionResult> CheckPattern(string userId)
        {
            return await _address.VerificaPadrao(userId) ? Ok() : NotFound();
        }
        [HttpGet("PullAddress/{userId}")]
        public async Task<IActionResult> PullAddress(string idUser)
        {
            var rt = await _address.PuxarEndereco(idUser);
            if(rt is not null)
                return Ok(rt);
            return NotFound();
        }
        [HttpDelete("DeleteAddress/{userId}/{key}")]
        public async Task<IActionResult> DeleteAddress(string idUser, string key)
        {
            var rt = await _address.DeleteEndereco(idUser, key);
            if (rt)
                return NoContent();
            return NotFound();
        }
        [HttpPatch("AlterAddress/{userId}/{key}")]
        public async Task<IActionResult> AlterAddress(string idUser, string key, ModelEndereco objEnd)
        {
            var rt = await _address.AlterarEndereco(key,idUser,objEnd);
            if(rt)
                return NoContent();
            return NotFound();
        }
    }
}
