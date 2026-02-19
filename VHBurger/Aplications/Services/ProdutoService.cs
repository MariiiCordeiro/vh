using VHBurger.DTOs.ProdutoDto;
using VHBurger.Interfaces;
using VHBurger.Domains;
using VHBurger.Aplications.Conversoes;
using VHBurger.Aplications.Regras;
using VHBurger.Exceptions;
using VHBurguer.Interfaces;
using VHBurguer.DTOs.ProdutoDto;

namespace VHBurger.Aplications.Services
{
    public class ProdutoService
    {
        private readonly IProdutoRepository _repository;

        public object HorarioAlteraçãoProduto { get; private set; }

        public ProdutoService(IProdutoRepository repository)
        {
            _repository = repository;
        }


        public List<LerProdutoDto> Listar()
        {
            List<Produto> produtos = _repository.Listar();

            List<LerProdutoDto> produtosDtos = produtos.Select(ProdutoParaDto.ConverterParaDto).ToList();

            return produtosDtos;
        }

        public LerProdutoDto ObterPorId(int id)
        {
            Produto produto = _repository.ObterPorId(id);

            if (produto == null)
            {
                throw new DomainException("Produto não encontrado!");
            }

            //Converte o produto encontrado para DTO e devolve
            return ProdutoParaDto.ConverterParaDto(produto);

        }

        public static void ValidarCadastro(CriarProdutoDto produtoDto)
        {
            if (string.IsNullOrWhiteSpace(produtoDto.Nome))
            {
                throw new DomainException("Nome é obrigatório!");
            }

            if (produtoDto.Preco < 0)
            {
                throw new DomainException("Preço deve ser maior que 0!");
            }

            if (string.IsNullOrWhiteSpace(produtoDto.Descricao))
            {
                throw new DomainException("Descrição é obrigatório!");
            }

            if (produtoDto.Imagem == null || produtoDto.Imagem.Length == 0)
            {
                throw new DomainException("Imagem é obrigatório!");
            }

            if (produtoDto.CategoriaIds == null || produtoDto.CategoriaIds.Count == 0)
            {
                throw new DomainException("Produto deve ter ao menos uma categoria!");
            }
        }

        public byte[] ObterImagem(int id)
        {
            byte[] imagem = _repository.ObterImagem(id);

            if (imagem == null || imagem.Length == 0)
            {
                throw new DomainException("Imagem não encontrada!");
            }
            return imagem;
        }

        public LerProdutoDto Adicionar(CriarProdutoDto produtoDto, int usuarioId)
        {
            ValidarCadastro(produtoDto);

            if (_repository.NomeExiste(produtoDto.Nome))
            {
                throw new DomainException("Produto existente!");
            }

            Produto produto = new Produto
            {
                Nome = produtoDto.Nome,
                Preco = produtoDto.Preco,
                Descricao = produtoDto.Descricao,
                Imagem = ImagemParaBytes.ConverterImagem(produtoDto.Imagem),
                StatusProduto = true,
                UsuarioID = usuarioId
            };

            _repository.Adicionar(produto, produtoDto.CategoriaIds);

            return ProdutoParaDto.ConverterParaDto(produto);
        }

        public LerProdutoDto Atualizar(int id, AtualizarProdutoDto produtoDto)
        {
            Produto produtoBanco = _repository.ObterPorId(id);

            HorarioAlteracaoProduto.ValidarHorario();

            if (produtoBanco == null)
            {
                throw new DomainException("Produto não encontrado");
            }

            //produtoIdAtual tem : este serve para passar o valor do parâmetro

            if(_repository.NomeExiste(produtoDto.Nome, produtoIdAtual: id))
            {
                throw new DomainException("Já existe outro porduto com esse nome!");
            }

            if (produtoDto.CategoriaIds == null || produtoDto.CategoriaIds.Count == 0)
            {
                throw new DomainException("Produto deve ter ao menos uma categoria!");
            }

            if (produtoDto.Preco < 0)
            {
                throw new DomainException("Preço deve ser maior que 0!");
            }

            produtoBanco.Nome = produtoDto.Nome;
            produtoBanco.Preco = produtoDto.Preco;
            produtoBanco.Descricao = produtoDto.Descricao;

            if (produtoDto.Imagem != null && produtoDto.Imagem.Length > 0)
            {
                produtoBanco.Imagem = ImagemParaBytes.ConverterImagem(produtoDto.Imagem);
            }

            if (produtoDto.StatusProduto.HasValue)
            {
                produtoBanco.StatusProduto = produtoDto.StatusProduto.Value;
            }

            _repository.Atualizar(produtoBanco, produtoDto.CategoriaIds);

            return ProdutoParaDto.ConverterParaDto(produtoBanco);
        }

        public void Remover(int id)
        {
            HorarioAlteracaoProduto.ValidarHorario();

            Produto produto = _repository.ObterPorId(id);

            if (produto == null)
            {
                throw new DomainException("Produto não encontrado!");
            }
            _repository.Remover(id);
        }
    }
}
