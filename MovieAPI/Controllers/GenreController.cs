using Microsoft.AspNetCore.Authorization;
using MovieAPI.Services.@interface;

namespace MoviesAPI.Controllers
{
    //[Authorize]
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
            var Genres = _genreService.GetGenres();
            return Ok(Genres);
        }
        [HttpGet("{id}")]
        public IActionResult GetGenreById(byte id)
        {
            var genre = _genreService.GetGenreById(id);
            return Ok(genre);
        }
        [HttpPost]
        public IActionResult CreateGenre([FromBody] GenreDTO dto)
        {
            var genre = _genreService.AddGenre(dto);
            return Ok(genre);
        }
        [HttpPost("{id}")]
        public IActionResult UpdateGenre(byte id,[FromBody] GenreDTO dto)
        {
            var genre = _genreService.UpdateGenre(id,dto);
            if (genre is null)
            {
                return NotFound();
            }
            return Ok(genre);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteGenre(byte id)
        {
            var genre = _genreService.GetGenreById(id);
            if (genre is null)
            {
                return NotFound();
            }
            _genreService.DeleteGenre(genre.Id);
            return NoContent();
        }
    }   
}
