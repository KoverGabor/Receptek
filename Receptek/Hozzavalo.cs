namespace ReceptekApi.Models
{
    public class Hozzavalo
    {
        public int Id { get; set; }
        public string IngredientName { get; set; } = string.Empty;
        public string Quantity { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime AddedTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public int SzakacsId { get; set; }
    }

    public class HozzavaloDto
    {
        public string IngredientName { get; set; } = string.Empty;
        public string Quantity { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int SzakacsId { get; set; }
    }
}