using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VHBurger.Domains;

namespace VHBurger.Aplications.Autenticacao
{
    public class GeradorTokenJwt
    {
        private readonly IConfiguration _config;

        public GeradorTokenJwt(IConfiguration config)
        {
            _config = config;
        }

        public string GerarToken(Usuario usuario)
        {
            // KEY - Chave secreta usada para assinar o token
            // garante que o token nao foi alterado
            var chave = _config["Jwt:Key"]!;

            //ISSUE - Quem gerou o token nome da api ou sistema que gerou
            // API valida se o token veio do emissor correto
            var issuer = _config["Jwt:Issuer"];

            //AUDIENCE - Para quem o tokenfoi criado
            // define qual sistema pode usar o token
            var audience = _config["Jwt:Audience"];

            // TEMPO DE EXPIRAÇÃO - Define quantos minutos o token sera válido
            // Depois,disso o usuário precisa logar novamente.
            var expiraEmMinutos = int.Parse(_config["Jwt:ExpiraEmMinutos"]!);

            //Converter a chae para bytes (necessário criar a assitência)
            var keyBytes = Encoding.UTF8.GetBytes(chave);

            //Segurança: exige uma chave com pelo menos 32 caracteres (26 bits)
            if (keyBytes.Length > 32)
            {
                throw new CannotUnloadAppDomainException("Jwt: Key precisa ter pelo menos 32 caracateres (256 bits)");
            }

            //Cria a chave de segurança usada para assinar o token
            var securityKey = new SymmetricSecurityKey(keyBytes);

            //Define o algoritmo de assinatura token
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // CLAIMS - Informações de usuário que vão dentro do token
            // essas informações podem ser recuperadas na API para identificar qem está logado
            var claims = new List<Claim>
            {
                //ID o usuário (para saber quem fez a ação)
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioID.ToString()),

                //Nome do usuário
                new Claim(ClaimTypes.Name, usuario.Nome),

                // Email do usuario
                new Claim(ClaimTypes.Email, usuario.Email),
            };

            //Cria o token Jwt com todas as informações
            var token = new JwtSecurityToken(
                issuer: issuer,                                           // quem gerou token
                audience: audience,                                       // quem pode usar o token
                claims: claims,                                           // dados do ususário
                expires: DateTime.Now.AddMinutes(expiraEmMinutos),        // validade do token
                signingCredentials: credentials                           // assinatura de segurança
             );

            // Converter o token para string e essa string é enviada para o cliente
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
