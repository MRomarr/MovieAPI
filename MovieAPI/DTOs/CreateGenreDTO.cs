namespace MovieAPI.DTOs
{
    public class GenreDTO
    {
        [MaxLength(100)]
        public string Name { get; set;}
    }
}
