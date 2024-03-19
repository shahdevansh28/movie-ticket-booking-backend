namespace movie_ticket_booking.Models.DTO
{
    public class TheaterResponseDTO
    {
        public Theater theater { get; set; }
        public List<ShowTime> showTimes { get; set; }
    }
}
