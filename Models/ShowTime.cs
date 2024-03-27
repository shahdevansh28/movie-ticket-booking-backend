using System.ComponentModel.DataAnnotations;

namespace movie_ticket_booking.Models
{
    public class ShowTime
    {
        public long Id { get; set; }
        [Required]
        public DateOnly ShowDate { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        [Required]
        public int Amount { get; set; }
        [Required]
        public int Capacity { get; set; }
        //Movie Id foregin key
        public long MovieId { get; set; }
        public Movie? Movie { get; set; }
        //Theater Id foregin key
        public long TheaterId { get; set; }
        public Theater? Theater { get; set; }
    }
}
