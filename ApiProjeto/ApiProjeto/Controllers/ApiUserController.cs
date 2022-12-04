using Microsoft.AspNetCore.Mvc;
using ApiMvc.Repositories;

namespace ApiProjeto.Controllers
{
    [ApiController]
    [Route("Api/v1/Auth")]
    public class ApiUserController : ControllerBase 
    {
        private readonly IUserRepository _userRepository;

        public ApiUserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        [HttpPost("Register/{name}")]
        public async Task<IActionResult> Register([FromRoute] string name,[FromForm] string email, [FromForm] string pwd)
        {
            var rt = await _userRepository.RegisterUser(name, email, pwd);
            if (rt.Length > 0)
                return Ok(rt);
            return Forbid();
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm] string email, [FromForm] string pwd)
        {
            var rt = await _userRepository.LoginUser(email,pwd);
            if (rt.Length >0)
                return Ok(rt);
            return NotFound();
        }
        [HttpGet("ReturnName/{idUser}")]
        public async Task<IActionResult> RetornaNome([FromRoute] string idUser)
        {
            var rt = await _userRepository.RetornaNome(idUser);
            if (!string.IsNullOrEmpty(rt))
                return Ok(rt);
            return NotFound();           
        }
        [HttpGet("ReturnId/{email}")]
        public async Task<IActionResult> RetornaID( [FromRoute] string email)
        {
            var rt = await _userRepository.RetornaID(email);
            if (!string.IsNullOrEmpty(rt))
                return Ok(rt);
            return NotFound();
        }
        [HttpPatch("ChangeName")]
        public async Task<IActionResult> TrocarNome([FromForm] string idUser, [FromForm] string nome)
        {
            var rt = await _userRepository.TrocarNome(idUser, nome);
            if (rt)
                return Ok();
            return NotFound();
        }
        [HttpPost("DeleteAcount")]
        public async Task<IActionResult> DeletarConta([FromForm] string idUser, [FromForm] string email, [FromForm] string pwd)
        {
            var rt = await _userRepository.DeletarConta(idUser, email, pwd);
            if (rt)
                return Ok(rt);
            return Conflict();
        }
        [HttpPost("RecoverPassword")]
        public async Task<IActionResult> RecuperarSenha([FromForm] string email)
        {
            var rt = await _userRepository.ResetarSenha(email);
            if (rt.Length > 0)
                return Ok(rt);
            return Conflict();
        }
    }
}
