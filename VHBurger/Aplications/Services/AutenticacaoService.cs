using VHBurger.Aplications.Autenticacao;
using VHBurger.Domains;
using VHBurger.DTOs.AutenticacaoDto;
using VHBurger.Exceptions;
using VHBurger.Interfaces;

namespace VHBurger.Aplications.Services
{
    public class AutenticacaoService
    {
        private readonly IUsuarioRepository  _repository;
        private readonly GeradorTokenJwt _tokenJwt;

        public AutenticacaoService(IUsuarioRepository repository, GeradorTokenJwt tokenJwt)
        {
            _repository = repository;
            _tokenJwt = tokenJwt;
        }

        // Compara a HASH SHA256
        private static bool VerificarSenha(string senhaDigitada, byte[] senhaHashBanco)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var hashDigitada = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(senhaDigitada));

            return hashDigitada.SequenceEqual(senhaHashBanco);
        }

        public TokenDto Login(LoginDto loginDto)
        {
            Usuario usuario = _repository.ObterPorEmail(loginDto.Email);

            if (usuario == null)
            {
                throw new DomainException("Usuário não existe!");
            }

            //Comparar com a senha armazenada
            if (!VerificarSenha(loginDto.Senha, usuario.Senha))
            {
                throw new DomainException("Email ou senha inválidos!");
            }

            //Gerando o token
            var token = _tokenJwt.GerarToken(usuario);
            TokenDto novoToken = new TokenDto { Token = token};

            return novoToken;
        }
    }
}
