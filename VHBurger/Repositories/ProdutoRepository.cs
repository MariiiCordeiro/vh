using Microsoft.EntityFrameworkCore;
using VHBurger.Contexts;
using VHBurger.Domains;
using VHBurger.Interfaces;
using VHBurguer.Interfaces;

namespace VHBurger.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly VH_BurguerContext _context;

        public List<Categoria> Categoria { get; private set; }

        public ProdutoRepository(VH_BurguerContext context)
        {
            _context = context;
        }

        public List<Produto> Listar()
        {
            List<Produto> produtos = _context.Produto.Include(produto => produto.Categoria).Include(produto => produto.Usuario).ToList();
            return produtos;
        }

        public Produto ObterPorId(int id)
        {
            // Procura no banco (aux produtodb) e verifica se o ID do porduto o banco é igual ao id passado com o parâmetro no método ObterPorId 
            Produto? produto = _context.Produto.Include(produto => produto.Categoria).Include(produto => produto.Usuario).FirstOrDefault(produtoDb => produtoDb.ProdutoID == id);

            return produto;
        }

        public bool NomeExiste(string nome, int? produtoIdAtual = null)
        {
            //AsQueryable() -> Monta a consulta para executar passo a passo
            //monta a consulta na tabela produto
            // não executa nada no banco ainda
            var produtoConsultado = _context.Produto.AsQueryable();

            if (produtoIdAtual.HasValue)
            {
                produtoConsultado = produtoConsultado.Where(produtoConsultado => produtoConsultado.ProdutoID != produtoIdAtual.Value);
            }

            return produtoConsultado.Any(produtoConsultado => produtoConsultado.Nome == nome);

        }

        public byte[] ObterImagem(int id)
        {
            var produto = _context.Produto.Where(produto => produto.ProdutoID == id).Select(produto => produto.Imagem).FirstOrDefault();
            return produto;
        }

        public void Adicionar(Produto produto, List<int> categoriasIds)
        {
            List<Categoria> categorias = _context.Categoria.Where(categoria => categoriasIds.Contains(categoria.CategoriaID)).ToList();

            produto.Categoria = categorias;

            _context.Produto.Add(produto);
            _context.SaveChanges();
        }

        public void Atualizar(Produto produto, List<int> categoriaIds)
        {
            Produto? produtoBanco = _context.Produto.Include(produto => produto.Categoria).FirstOrDefault(produtoAuxiliar => produtoAuxiliar.ProdutoID == produto.ProdutoID);

            if(produtoBanco == null)
            {
                return;
            }

            produtoBanco.Nome = produto.Nome;
            produtoBanco.Preco = produto.Preco;
            produtoBanco.Descricao = produto.Descricao;

            if(produto.Imagem != null && produto.Imagem.Length > 0)
            {
                produtoBanco.Imagem = produto.Imagem;
            }

            if (produto.StatusProduto.HasValue)
            {
                produtoBanco.StatusProduto = produto.StatusProduto;
            }

            var categorias = _context.Categoria.Where(categoria => categoriaIds.Contains(categoria.CategoriaID)).ToList();

            //Clear() -> Remove as ligações atuais entre o produto e as caregorias ele não apaga a categoria do banco só remove o vinculo com a tabela ProdutoCategoria
            produtoBanco.Categoria.Clear();

            foreach(var categoria in categorias)
            {
                produtoBanco.Categoria.Add(categoria);
            }
            _context.SaveChanges();

        }

        public void Remover(int id)
        {
            Produto? produto = _context.Produto.FirstOrDefault(produto => produto.ProdutoID == id);
            if (produto != null)
            {
                return;
            }
            _context.Produto.Remove(produto);
            _context.SaveChanges();
        }
    }
}



