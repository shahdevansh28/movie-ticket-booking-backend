namespace movie_ticket_booking.Models.DTO
{
    public class SeatDetailDTO
    {
        public int TotalRow { get; set; }
        public int TotalCol { get; set; }
        public long ShowTimeID { get; set; }
        public bool IsAvailable { get; set; }
    }
}
