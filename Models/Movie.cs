using System.ComponentModel.DataAnnotations;

namespace movie_ticket_booking.Models
{
    public class Movie
    {
        public long Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public DateTime Release_Date { get; set; }
        [Required]
        public int Duration { get; set; }

    }
}
