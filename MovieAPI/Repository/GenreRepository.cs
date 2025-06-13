namespace MoviesAPI.Repository
{
    public class GenreRepository : IGenreRepository
    {
        private readonly ApplicationDbContext _context;
        public GenreRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public IQueryable<Genre> GetGenres()
        {
            return _context.Genres.OrderBy(g => g.Name).AsQueryable();
        }
        public Genre GetGenreById(byte id)
        {
            return _context.Genres.FirstOrDefault(g => g.Id == id);
        }
        public Genre AddGenre(GenreDTO dto)
        {
            Genre genre = new Genre
            {
                Name = dto.Name
            };
            _context.Genres.Add(genre);
            _context.SaveChanges();
            return genre;
        }
        public Genre UpdateGenre(byte id, GenreDTO dto)
        {
            var genre = _context.Genres.FirstOrDefault(g => g.Id == id);
            if (genre != null)
            {
                genre.Name = dto.Name;
                _context.SaveChanges();
            }
            return genre;
        }
        public void DeleteGenre(byte id)
        {
            var genre = _context.Genres.FirstOrDefault(g => g.Id == id);
            if (genre != null)
            {
                _context.Genres.Remove(genre);
                _context.SaveChanges();
            }
        }
    }
}
