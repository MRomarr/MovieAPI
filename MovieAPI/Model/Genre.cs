namespace MoviesAPI.Model
{
    public class Genre
    {
        public byte Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }

    }
}
