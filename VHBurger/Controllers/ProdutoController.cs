using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VHBurger.Aplications.Services;
using VHBurger.Exceptions;
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
        private int ObterUsuarioIdLogado()
        {
            //Busca no token/claims o valor armazenado como id do usuario
            //Claim.Types.NameIdentifier geralmente gurado o ID do usuario no JWT
            string? idTexto = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(idTexto))
            {
                throw new DomainException("Usuário não autenticado");
            } 
            //Converte o id que veio como texto para inteiro
            // nosso UsuarioID no sistema está como int
            // As claims (infomações do usuário dentro do token) sempre são armazenadas como texto.
            return int.Parse(idTexto);
        }

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

        // GET - api/produto/5/image
        [HttpGet("{id}/image")]
        public ActionResult ObterImage(int id)
        {
            try
            {
                var imagem = _service.ObterImagem(id);

                //Retorna o arquivo para o navegador
                //"image/jpeg" informa o tipo de imagem (MIME Type)
                // O navegador entende que deve redenrizar como imagem
                return File(imagem, "image/jpeg");
            }
            catch (DomainException ex)
            {
                return NotFound(ex.Message); // Not Found() -  não encontrado
            }
        }
    }
}
