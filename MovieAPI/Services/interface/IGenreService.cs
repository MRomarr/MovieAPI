
namespace MovieAPI.Services.@interface
{
    public interface IGenreService
    {
        IEnumerable<Genre> GetGenres();
        Genre GetGenreById(byte id);
        Genre AddGenre(GenreDTO dto);
        Genre UpdateGenre(byte id, GenreDTO dto);
        void DeleteGenre(byte id);
    }
}
