using VHBurger.Contexts;
using VHBurger.Domains;
using VHBurger.Interfaces;

// Repositorio serve só para fazer a conexão com o banco as ações de cadastrar, deletar e editar será em outras camadas.
// Não há regra de negócio aqui somente a conversa com o banco.
// Persisitir com o banco
namespace VHBurger.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly VH_BurguerContext _context;

        public UsuarioRepository(VH_BurguerContext context) 
        {
            _context = context;
        }   

        public List<Usuario>Listar()
        {
            return _context.Usuario.ToList();   
        }

        public Usuario? ObterPorID(int id)
        {
            return _context.Usuario.Find(id); //Funciona bem com chaves primárias;
        }
        public Usuario? ObterPorEmail(string email)
        { 
            return _context.Usuario.FirstOrDefault(usuario => usuario.Email == email); //Verificação se o email informado é o que está no banco FirstOrDeFault -> Retorna nosso usuário do banco
        }
        public bool EmailExiste(string email) //Caso exista será aplicado na service
        { 
            return _context.Usuario.Any(usuario =>usuario.Email == email); // Any -> Retorna um true or false para validar se existe um usuario com esse email.
        }

        public void Adicionar(Usuario usuario) //Por enquanto será passado por info do banco deois por DTO
        { 
            _context.Usuario.Add(usuario); //Adicionando o usuario
            _context.SaveChanges();
        }

        public void Atualizar(Usuario usuario) // Atualização de dados
        {
            Usuario? usuarioBanco = _context.Usuario.FirstOrDefault(usuarioAuxiliar => usuarioAuxiliar.UsuarioID == usuario.UsuarioID); // Verificação se o usuario existe dentro do banco para validação da atualização

            if(usuarioBanco == null) //* Caso nao exista, não aualiza nada pois não existe
            {
                return;
            }

            //* Esta aqui paa visualização de consumir -> sem aplicação de regras por enquanto somente a conexão com o banco e salvar 
            usuarioBanco.Nome = usuario.Nome; //* se existir irá atualizar o nome do usuario.
            usuarioBanco.Email = usuario.Email; //* se existir irá atualizar o email do usuario.
            usuarioBanco.Senha = usuario.Senha; //* se exitir irá atualizar a senha do usuario.

            _context.SaveChanges();
        }

        public void Remover(int id)
        {
            Usuario? usuario = _context.Usuario.FirstOrDefault(usuarioAuxiliar => usuarioAuxiliar.UsuarioID == id);

            if (usuario == null) //* Caso nao exista, não aualiza nada pois não existe
            {
                return;
            }
            
            _context.Usuario.Remove(usuario);
            _context.SaveChanges();

        }
    }
}
