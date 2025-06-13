namespace MovieAPI.DTOs
{
    public class MovieDTO
    {
        [MaxLength(250)]
        public string Title { get; set; }

        public int Year { get; set; }

        [MaxLength(2500)]
        public string Storeline { get; set; }
        [AllowedExtensions(FileSettings.AllowedExtensions)]
        [MaxFileSizeAttibute(FileSettings.MaxFileSizeByte)]
        public IFormFile Poster { get; set; }

        public byte GenreId { get; set; }
    }
}
