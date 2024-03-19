using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movie_ticket_booking.Models;
using movie_ticket_booking.Models.DTO;

namespace movie_ticket_booking.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SeatController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public SeatController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Seat>>> GetSeats()
        {
            if (_applicationDbContext.Seats == null)
            {
                return NotFound();
            }
            return await _applicationDbContext.Seats.ToListAsync();
        }
        [HttpGet]
        [ActionName("seat-layout")]
        public async Task<ActionResult<IEnumerable<Seat>>> GetSeatByShowTimeId(long showTimeId)
        {
            if(_applicationDbContext.Seats == null)
            {
                return NotFound();
            }

            return await _applicationDbContext.Seats.Where(e => e.ShowTimeId == showTimeId).ToListAsync();
        }


        [HttpPost]
        [ActionName("add-seat")]
        public async Task<ActionResult<IEnumerable<Seat>>> AddSeats(SeatDetailDTO seatDetail)
        {
            for(int i = 1; i <= seatDetail.TotalRow;i++)
            {
                for(int j = 1; j <= seatDetail.TotalCol; j++)
                {
                    var seatObj = new Seat()
                    {
                        Row = i,
                        Number = j,
                        ShowTimeId = seatDetail.ShowTimeID,
                        IsAvailable = true
                    };
                    _applicationDbContext.Add(seatObj);
                }
            }
            await _applicationDbContext.SaveChangesAsync();

            return await _applicationDbContext.Seats.Where(e => e.ShowTimeId == seatDetail.ShowTimeID).ToListAsync();
            /*if (_applicationDbContext.Seats == null)
            {
                return NotFound();
            }

            return await _applicationDbContext.Seats.Where(e => e.ShowTimeId == showTimeId).ToListAsync();*/
        }

    }
}
