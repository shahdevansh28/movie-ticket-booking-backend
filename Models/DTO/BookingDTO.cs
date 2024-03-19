namespace movie_ticket_booking.Models.DTO
{
    public class SeatNumber
    {
        public int row { get; set; }
        public int number { get; set; }
    }
    public class BookingDTO
    {
        public long showTimeId { get; set; }
        public List<SeatNumber> seatNumber { get; set; }
        public DateTime bookingDate { get; set; }
    }

    public class BookingResponseDTO
    {
        public List<Booking> selectedSeats { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
        public double TotalAmount { get; set; }
        public string TransactionID { get; set; }
    }
}
