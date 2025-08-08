public class Book
{
    public int Id { get; set; }
    public string Tittle { get; set; }
    public string Author { get; set; }
    public string ISBN { get; set; }
    public int AñoPublicacion { get; set; }
    public bool Disponible { get; set; }
    public int CategoriaId { get; set; }
    public Category Category { get; set; }
}