using MovieAPI.Services.@interface;

namespace MoviesAPI.Repository
{
    public class MovieRepository:IMovieRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public MovieRepository(ApplicationDbContext context, IGenreService genreService,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public IQueryable<Movie> GetMovies()
        {
            return _context.Movies.OrderByDescending(m => m.Rate).Include(m => m.Genre).AsQueryable();
        }
        public Movie GetMovieById(int id)
        {
            return _context.Movies.FirstOrDefault(m => m.Id == id);
        }
        public Movie AddMovie(MovieDTO dto)
        {

            using var datastrem = new MemoryStream();
            dto.Poster.CopyTo(datastrem);
            var movie = _mapper.Map<Movie>(dto);
            movie.Poster = datastrem.ToArray();
            _context.Movies.Add(movie);
            _context.SaveChanges();
            return movie;
        }
        public bool DeleteMovie(int id)
        {
            var movie = _context.Movies.FirstOrDefault(m => m.Id == id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        public Movie UpdateMovie(int id, MovieDTO dto)
        {
            var movie = _context.Movies.FirstOrDefault(m => m.Id == id);
            if (movie != null)
            {
                using var datastrem = new MemoryStream();
                dto.Poster.CopyTo(datastrem);
                _mapper.Map(dto, movie);
                movie.Poster = datastrem.ToArray();
                _context.SaveChanges();
            }
            return movie;
        }
    }
}
