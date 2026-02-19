namespace VHBurger.DTOs.ProdutoDto
{
    public class AtualizarProdutoDto
    {
        public string Nome { get; set; } = null!;
        public decimal Preco { get; set; }
        public string Descricao { get; set; } = null!;

        public IFormFile Imagem { get; set; } = null!; // A imagem vem via multipart/form-data é idela para upload de arquivo

        public List<int> CategoriaIds { get; set; } = new();

        public bool? StatusProduto {  get; set; }

    }
}
