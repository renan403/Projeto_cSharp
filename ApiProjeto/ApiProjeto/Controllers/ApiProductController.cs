using ApiMvc.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ApiMvc.Controllers
{
    [ApiController]
    [Route("Api/v1/Product")]

    public class ApiProductController : ControllerBase
    {
        private readonly IProductRepository _product;
        private readonly IWebHostEnvironment _iWeb;
        public ApiProductController(IProductRepository product, IWebHostEnvironment iweb)
        {
            _product = product;
            _iWeb = iweb;
        }
        [HttpPost("{idUser}")]
        public async Task<IActionResult> AddProduct(string idUser, [FromForm] ModelProduto p)
        {
            await _product.AddProdutoAsync(idUser, _iWeb, p);
            return Ok();
        }   
        [HttpGet("GetAll")]
        public async Task<IActionResult> RetornaArrayProdutos()
        {
            var retorno = await _product.RetornaArrayProdutosAsync();
            return retorno.Count > 0 ? Ok(retorno) : NoContent();
        }
        [HttpGet("GetProdById/{id}")]
        public async Task<IActionResult> GetRetornaArrayProdutosWithId(string id)
        {
            var rt = await _product.RetornarArrayProdutosVendedorAsync(id);
            return rt.Length < 1 ? NotFound(id) : Ok(rt);
        }
        [HttpGet("GetProdSeller/{idUser}")]
        public async Task<IActionResult> GetProdSeller(string idUser)
        {
            var produtos = await _product.RetornarObjProdutosVendedorAsync(idUser);
            return produtos == null ? NoContent() : Ok(produtos);
        }
        [HttpDelete("DeleteProd/{idProd}")]
        public async Task<IActionResult> DeleteProd(string idProd)
        {
            return await _product.DeleteProdutosAsync(idProd) ? Ok() : NoContent();
        }
        [HttpDelete("DeleteOneProd/{keyProd}")]
        public async Task<IActionResult> DeleteOneProd(string keyProd)
        {
            try
            {
                await _product.DeleteUmProdutoAsync(keyProd);
                return NoContent();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
        [HttpGet("ReturnProd/{idProd}")]
        public async Task<IActionResult> ReturnProdforID(string idProd)
        {
            try
            {
                var rt = await _product.ReturnProductForIDAsync(idProd);
                return rt == null ? NoContent() : Ok(rt);
            }
            catch (Exception)
            {

                return NoContent();
            }

        }
        [HttpPatch("AlterProd/{idUser}")]
        public async Task<IActionResult> AlterProd([FromRoute] string idUser, [FromForm] ModelProduto model)
        {
            var rt = await _product.AlterarProdutoAsync(idUser, model);
            if(rt)                
                return NoContent();
            return NotFound();
        }
    }
}
