using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VHBurger.Aplications.Services;
using VHBurger.DTOs.AutenticacaoDto;
using VHBurger.Exceptions;

namespace VHBurger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticacaoController : ControllerBase
    {
        private readonly AutenticacaoService _service;

        public AutenticacaoController(AutenticacaoService service)
        {
            _service = service;
        }

        [HttpPost("login")]

        public ActionResult<TokenDto> Login(LoginDto loginDto)
        {
            try
            {
                var token = _service.Login(loginDto);

                return Ok(token);
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
