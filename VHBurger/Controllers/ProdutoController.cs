using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VHBurger.Aplications.Services;
using VHBurguer.DTOs.ProdutoDto;

namespace VHBurger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        private readonly ProdutoService _service;

        public ProdutoController(ProdutoService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<List<LerProdutoDto>> Listar()
        {
            List<LerProdutoDto> produtos = _service.Listar();

            //return StatusCode(200, produtos)

            return Ok(produtos);
        }

        //Autenticação do usuario

        [HttpGet("{id}")]
        public ActionResult<LerProdutoDto> ObterPorId(int id)
        {
            LerProdutoDto produto = _service.ObterPorId(id);

            if (produto == null)
            {
                // return StatusCode(404)
                return NotFound();
            }
            return Ok(produto);
        }
    }
}
