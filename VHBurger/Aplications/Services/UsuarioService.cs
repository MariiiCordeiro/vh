using System.Security.Cryptography;
using System.Text;
using VHBurger.Domains;
using VHBurger.DTOs.UsuarioDto;
using VHBurger.Exceptions;
using VHBurger.Interfaces;

namespace VHBurger.Aplications.Services
{
    // Service concetra o "como fazer"
    // Aqui irá ficar as regras de negócio
    // Iremos utilizar o repositorio que ira conversar com o banco e a interface será oq sera utilizado
    // camada de proteção
    public class UsuarioService
    {
        //_repository é o canal para acessar os dados
        private readonly IUsuarioRepository _repository;

        // injeção de dependencias
        // implementação do repositorio e o service só depende da interface
        public UsuarioService(IUsuarioRepository repository)
        {
            _repository = repository;
        }

        // Por que private
        // porque o método não é regra de negócio e não faz sentido existir fora do UsuarioService
        // LerUsuarioDto é uma função para ler o usuario parecida com a viewModel
        private static LerUsuarioDto LerDto(Usuario usuario) // pega a entidade usuario e gera um dto somente  que eu estou desejando
        {
            LerUsuarioDto lerUsuario = new LerUsuarioDto
            {
                UsuarioID = usuario.UsuarioID,
                Nome = usuario.Nome,
                Email = usuario.Email,
                StatusUsuario = usuario.StatusUsuario ?? true // se não tiver usuario no banco deixa com true
            };
            return lerUsuario;
        }

        public List<LerUsuarioDto> Listar()
        {
            List<Usuario> usuarios = _repository.Listar();

            List<LerUsuarioDto> usuarioDto = usuarios.Select(usuarioBanco => LerDto(usuarioBanco)).ToList(); //SELECT que percorre cada usuario e LerDTO
            return usuarioDto;
        }

        private static void ValidarEmail(string email)
        {
            if(string.IsNullOrWhiteSpace(email) || !email.Contains("@")){
               throw new  DomainException("Email inválido!");
            }
        }

        private static byte[] HashSenha(string senha)
        {
            if(string.IsNullOrWhiteSpace(senha)) //garante que a senha não esta vazia
            {
                throw new DomainException("Senha é obrigatória");
            }

            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(senha));
        }

        public LerUsuarioDto ObterPorId(int id)
        {
            Usuario? usuario = _repository.ObterPorID(id);
            if(usuario == null)
            {
                throw new DomainException("Usuario não encontrado!");
            }
            return LerDto(usuario); // se existir usuario, converte para DTO e devolve o usuario.
        }

        public LerUsuarioDto ObterPorEmail(string email)
        {
            Usuario? usuario = _repository.ObterPorEmail(email);
            if(usuario == null)
            {
                throw new DomainException("Usuario não encontrado!");
            }
            return LerDto(usuario); ; // se existir usuario, converte para DTO e devolve o usuario.
        }

        public LerUsuarioDto Adicionar(CriarUsuarioDto usuarioDto)
        {
            ValidarEmail(usuarioDto.Email);

            if (_repository.EmailExiste(usuarioDto.Email))
            {
                throw new DomainException("Já existe um usuário com este email!");
            };

            Usuario usuario = new Usuario // Criando a entidade de usuario
            {
                Nome = usuarioDto.Nome,
                Email = usuarioDto.Email,
                Senha = HashSenha(usuarioDto.Senha),
                StatusUsuario = true      // Garantia de que será true de que o usuario está ativo
            };

            _repository.Adicionar(usuario);

            return LerDto(usuario); // REtorna ler dto para não retornar objeto com acento. Salva e cria o novo usuario e retorna sem mostrar a senha.
        }

        public LerUsuarioDto Atualizar(int id, CriarUsuarioDto usuarioDto) //Para atualizar deve-se procurar pelo id
        {
            ValidarEmail(usuarioDto.Email);
            Usuario usuarioBanco = _repository.ObterPorID(id);  

            if( usuarioBanco == null)
            {
                throw new DomainException("Usuário não encontrado!");
            }

            ValidarEmail(usuarioDto.Email);  // Usuario pode tentar trocar o email e não talvez não consiga pois pode ser que ele já exista no banco. Para isso criase um regra:

            Usuario usuarioComMesmoEmail = _repository.ObterPorEmail(usuarioDto.Email);

            if(usuarioComMesmoEmail != null && usuarioComMesmoEmail.UsuarioID != id) // Se o email esta preenchid e se o id do banco esta la, sendo outro não deixa cadastrar
            {
                throw new DomainException("Já existe um usuário com o este email!");
            }

            //Tudo certo com o email salva-se as informações
            // Substitui as infotmações do banco (usuarioBanco)
            // Inserindo as alterações que estão vindo do usuarioDto
            usuarioBanco.Nome = usuarioDto.Nome;
            usuarioBanco.Email = usuarioDto.Email;  
            usuarioBanco.Senha =  HashSenha(usuarioDto.Senha);

            _repository.Atualizar(usuarioBanco);

            return LerDto(usuarioBanco);
        }

        public void Remover(int id)
        {
            Usuario usuario = _repository.ObterPorID(id);   

            if(usuario == null)
            {
                throw new DomainException("Usuário não encontrado!");
            }
            _repository.Remover(id);
        }
    }
}
