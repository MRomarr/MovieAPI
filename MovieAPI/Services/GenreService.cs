using MovieAPI.Services.@interface;

namespace MoviesAPI.Services
{

    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _genreRepository;
        public GenreService(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }
        public Genre AddGenre(GenreDTO dto)
        {
            return _genreRepository.AddGenre(dto);
        }
        public void DeleteGenre(byte id)
        {
            _genreRepository.DeleteGenre(id);
        }
        public Genre GetGenreById(byte id)
        {
            return _genreRepository.GetGenreById(id);
        }
        public IEnumerable<Genre> GetGenres()
        {
            return _genreRepository.GetGenres();
        }
        public Genre UpdateGenre(byte id, GenreDTO dto)
        {
            return _genreRepository.UpdateGenre(id, dto);
        }
    }
}
