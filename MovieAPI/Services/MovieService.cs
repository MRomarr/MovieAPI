
namespace MovieAPI.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        public MovieService(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;

        }
        public IEnumerable<Movie> GetMovies()
        {
            return _movieRepository.GetMovies().ToList();
        }
        public Movie GetMovieById(int id)
        {
            return _movieRepository.GetMovieById(id);
        }
        public Movie AddMovie(MovieDTO dto)
        {
            return _movieRepository.AddMovie(dto);
        }
        public bool DeleteMovie(int id)
        {
            return _movieRepository.DeleteMovie(id);
        }
        public Movie UpdateMovie(int id, MovieDTO dto)
        {
            return _movieRepository.UpdateMovie(id, dto);
        }
    }
}
