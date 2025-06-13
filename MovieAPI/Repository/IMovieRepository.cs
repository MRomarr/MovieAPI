namespace MoviesAPI.Repository
{
    public interface IMovieRepository
    {
        IQueryable<Movie> GetMovies();
        Movie GetMovieById(int id);
        Movie AddMovie(MovieDTO dto);
        Movie UpdateMovie(int id, MovieDTO dto);
        bool DeleteMovie(int id);
    }
}
