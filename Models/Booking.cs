using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace movie_ticket_booking.Models
{
    public class Booking
    {
        public long Id { get; set; }
        //public double TotalPrice { get; set; }
        [Required]
        public DateOnly BookingDate { get; set; }
        [Required]
        public string Receipt { get; set; }
        public long? SeatId { get; set; }
        public Seat? Seat { get; set; }
        public long ShowTimeId { get; set; }
        public ShowTime? ShowTime { get; set; }
        /*public string UserId { get; set; }
        public virtual User? User { get; set; }*/
    }
}
