using System.ComponentModel.DataAnnotations;

namespace movie_ticket_booking.Models
{
    public class Seat
    {
        public long Id { get; set; }
        public int Row { get; set; }//row no of seat
        public int Number { get; set; } // seat no within the row
        [Required]
        public bool IsAvailable { get; set; }
        //Theater id foregin key
        public long ShowTimeId { get; set; }
        public ShowTime? ShowTime { get; set; }
    }
}
