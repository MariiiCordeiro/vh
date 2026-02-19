using VHBurger.Domains;

namespace VHBurger.Interfaces
{
    public interface IUsuarioRepository
    {
        List<Usuario> Listar();
        // Pode ser que não venha nenhuem usuário na busca então coloca-se "?", pois a busca asvezes pode ser nula.
        Usuario? ObterPorID(int id);

        Usuario? ObterPorEmail(string email);
        bool EmailExiste(string email);

        void Adicionar(Usuario usuario);

        void Atualizar(Usuario usuario);

        void Remover(int id);   
    }
}
