
namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly IGenreService _genreService;

        public MoviesController(IMovieService movieService, IGenreService genreService)
        {
            _movieService = movieService;
            _genreService = genreService;
        }

        [HttpGet]
        public IActionResult GetMovies()
        {
            var movies = _movieService.GetMovies();
            return Ok(movies);
        }

        [HttpGet("{id}")]
        public IActionResult GetMovieById(int id)
        {
            var movie = _movieService.GetMovieById(id);
            if (movie is null)
                return NotFound();
            return Ok(movie);
        }

        [HttpPost]
        public IActionResult CreateMovie([FromForm] MovieDTO dto)
        {
            var genre = _genreService.GetGenreById(dto.GenreId);
            if (genre is null)
                return BadRequest("Genre not found");

            var movie = _movieService.AddMovie(dto);
            return CreatedAtAction(nameof(GetMovieById), new { id = movie.Id }, movie);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateMovie(int id, [FromForm] MovieDTO dto)
        {
            var movie = _movieService.UpdateMovie(id, dto);
            if (movie is null)
                return NotFound();
            return Ok(movie);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteMovie(int id)
        {
            var result = _movieService.DeleteMovie(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}
