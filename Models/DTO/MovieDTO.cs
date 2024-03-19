using System.ComponentModel.DataAnnotations;

namespace movie_ticket_booking.Models.DTO
{
    public class MovieDTO
    {
        public int Id { get; set; } 
        public string Title { get; set; }
        public DateTime Release_Date { get; set; }
        public int Duration { get; set; }
    }
}
