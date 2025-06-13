
namespace MoviesAPI.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet]
        public IActionResult GetGenres()
        {
            var genres = _genreService.GetGenres();
            return Ok(genres);
        }

        [HttpGet("{id}")]
        public IActionResult GetGenreById(byte id)
        {
            var genre = _genreService.GetGenreById(id);
            if (genre is null)
            {
                return NotFound();
            }
            return Ok(genre);
        }

        [HttpPost]
        public IActionResult CreateGenre([FromBody] GenreDTO dto)
        {
            if (dto is null || string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("Genre name is required.");
            }

            var genre = _genreService.AddGenre(dto);
            return CreatedAtAction(nameof(GetGenreById), new { id = genre.Id }, genre);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateGenre(byte id, [FromBody] GenreDTO dto)
        {
            if (dto is null || string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("Genre name is required.");
            }

            var updatedGenre = _genreService.UpdateGenre(id, dto);
            if (updatedGenre is null)
            {
                return NotFound();
            }
            return Ok(updatedGenre);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteGenre(byte id)
        {
            var genre = _genreService.GetGenreById(id);
            if (genre is null)
            {
                return NotFound();
            }
            _genreService.DeleteGenre(id);
            return NoContent();
        }
    }
}
