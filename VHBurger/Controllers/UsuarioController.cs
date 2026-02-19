using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VHBurger.Aplications.Services;
using VHBurger.DTOs.UsuarioDto;
using VHBurger.Exceptions;

namespace VHBurger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioService _service;
        public UsuarioController(UsuarioService service)
        {
            _service = service;
        }

        //GET -> Lista informações aqui trata-se do controle do que iria aparecer na view
        [HttpGet]
        public ActionResult<List<LerUsuarioDto>> Listar()
        {
            List<LerUsuarioDto> usuarios = _service.Listar();
            // Retorna a lista de usuarios a partir da dto de services
            return Ok(usuarios); // OK = 200 deu certo
        }

        [HttpGet("{id}")]
        public ActionResult<LerUsuarioDto> ObterPorId(int id) 
        {
            LerUsuarioDto usuario = _service.ObterPorId(id);

            if (usuario == null)
            {
                return NotFound();
            }
            return Ok(usuario);
        }

        [HttpGet("email/{email}")]
        public ActionResult<LerUsuarioDto> ObterPorEmail(string email)
        {
            LerUsuarioDto usuario = _service.ObterPorEmail(email);
            if (usuario == null)
            {
                return NotFound();
            }
            return Ok(usuario);
        }

        [HttpPost]
        public ActionResult<LerUsuarioDto> Adicionar(CriarUsuarioDto usuarioDto)
        {
            try
            {
                LerUsuarioDto usuarioCriado = _service.Adicionar(usuarioDto);
                return StatusCode(201, usuarioCriado);
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("{id}")]
        public ActionResult<LerUsuarioDto> Atualizar(int id, CriarUsuarioDto usuarioDto)
        {
            try
            {
                LerUsuarioDto usuarioAtualizado = _service.Atualizar(id, usuarioDto);
                return StatusCode(200, usuarioAtualizado);
            }
            catch(DomainException ex) //nao deu certo as atualizações retorna as exceções existentes para atualização
            {
                return BadRequest(ex.Message);
            }
        }

        // Inativação do usuario por conta da trigger no banco
        [HttpDelete("{id}")]
        public ActionResult Remover(int id)
        {
            try
            {
                _service.Remover(id);
                return NoContent(); // ok funcional, ok sem retorno
            }
            catch(DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        
    }
}
