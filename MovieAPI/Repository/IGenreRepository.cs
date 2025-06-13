
namespace MoviesAPI.Repository
{
    public interface IGenreRepository
    {
        IQueryable<Genre> GetGenres();
        Genre GetGenreById(byte id);
        Genre AddGenre(GenreDTO dto);
        Genre UpdateGenre(byte id, GenreDTO dto);
        void DeleteGenre(byte id);
    }
}