using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
using movie_ticket_booking.Models;
using System.Linq;

namespace movie_ticket_booking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowTimeController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public ShowTimeController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShowTime>>> GetShowTimes()
        {
            if (_applicationDbContext.ShowTimes == null)
            {
                return NotFound();
            }
            List<ShowTime> showTimes = _applicationDbContext.ShowTimes.ToList();
            foreach(var showTime in showTimes)
            {
                showTime.Theater = _applicationDbContext.Theaters.Where(x => x.Id == showTime.TheaterId).FirstOrDefault();
                showTime.Movie = _applicationDbContext.Movies.Where(x => x.Id == showTime.MovieId).FirstOrDefault();
            }
            return showTimes;
        }
        [HttpGet]
        [Route("get-showtime-by-movie")]
        public async Task<IEnumerable<IGrouping<long, ShowTime>>> GetShowTimesByMovie(long id,DateOnly date)
        {
            /* if (_applicationDbContext.ShowTimes == null)
             {
                 return NotFound();
             }*/
            //I have movie id
            //id startTime endTime movieId 
           // var tmp = new DateTime(date);
           

            var showTimeByMovieId = _applicationDbContext.ShowTimes.Where(x => x.MovieId == id && x.ShowDate == date).ToList();

            foreach (ShowTime showTime in showTimeByMovieId)
            {
                showTime.Theater = _applicationDbContext.Theaters.Where(x => x.Id == showTime.TheaterId).FirstOrDefault();
            }

            var showTimeByTheater = showTimeByMovieId.GroupBy(x => x.TheaterId);

            return showTimeByTheater;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ShowTime>> GetShowTime(long id)
        {
            if (_applicationDbContext.ShowTimes == null)
            {
                return NotFound();
            }
            var show = _applicationDbContext.ShowTimes.Find(id);
            if (show == null)
            {
                return NotFound();
            }
            return show;
        }

        [HttpPost]
        public async Task<ActionResult<ShowTime>> PostShowTime(ShowTime show)
        {
            if (_applicationDbContext.ShowTimes == null)
            {
                return NotFound();
            }
            _applicationDbContext.Add(show);
            await _applicationDbContext.SaveChangesAsync();

            return CreatedAtAction("GetShowTime", new { id = show.Id }, show);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutShowTime(long id, ShowTime show)
        {
            if (id != show.Id)
            {
                return BadRequest();
            }

            _applicationDbContext.Entry(show).State = EntityState.Modified;

            try
            {
                await _applicationDbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShowTimeExists(id))
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShowTime(long id)
        {
            if (_applicationDbContext.ShowTimes == null)
            {
                return NotFound();
            }
            var show = await _applicationDbContext.ShowTimes.FindAsync(id);
            if (show == null)
            {
                return NotFound();
            }

            _applicationDbContext.ShowTimes.Remove(show);
            await _applicationDbContext.SaveChangesAsync();

            return NoContent();
        }
        private bool ShowTimeExists(long id)
        {
            return (_applicationDbContext.ShowTimes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
