using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movie_ticket_booking.Models;
using movie_ticket_booking.Models.DTO;

namespace movie_ticket_booking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public MovieController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            if (_applicationDbContext.Movies == null)
            {
                return NotFound();
            }
            return await _applicationDbContext.Movies.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovie(long id)
        {
            if (_applicationDbContext.Movies == null)
            {
                return NotFound();
            }
            var movie = _applicationDbContext.Movies.Find(id);
            if (movie == null)
            {
                return NotFound();
            }
            return movie;
        }
        [HttpGet]
        [Route("api/get-movieByshow")]
        public async Task<ActionResult<Movie>> GetMovieByShowTime(long showTimeId)
        {
            if (_applicationDbContext.ShowTimes == null)
            {
                return NotFound();
            }
            var showTime = _applicationDbContext.ShowTimes.Where(x => x.Id == showTimeId).FirstOrDefault();
            var movie = await _applicationDbContext.Movies.Where(x => x.Id == showTime.MovieId).FirstOrDefaultAsync();
            return movie;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie(long id, Movie movie)
        {
            if (id != movie.Id)
            {
                return BadRequest();
            }

            _applicationDbContext.Entry(movie).State = EntityState.Modified;

            try
            {
                await _applicationDbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MoveExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        [HttpPost]
        public async Task<ActionResult<Movie>> PostMovie(Movie movie)
        {
            if (_applicationDbContext.Movies == null)
            {
                return NotFound();
            }
            _applicationDbContext.Add(movie);
            await _applicationDbContext.SaveChangesAsync();

            return CreatedAtAction("GetMovie", new { id = movie.Id }, movie);
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(long id)
        {
            if (_applicationDbContext.Movies == null)
            {
                return NotFound();
            }
            var movie = await _applicationDbContext.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            _applicationDbContext.Movies.Remove(movie);
            await _applicationDbContext.SaveChangesAsync();

            return NoContent();
        }
        private bool MoveExists(long id)
        {
            return (_applicationDbContext.Movies?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
