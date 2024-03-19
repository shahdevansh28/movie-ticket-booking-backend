using System.ComponentModel.DataAnnotations;

namespace movie_ticket_booking.Models
{
    public class Theater
    {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Location { get; set; }
        //[Required]
        //public int Capacity { get; set; }
    }
}
