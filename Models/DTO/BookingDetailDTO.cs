namespace movie_ticket_booking.Models.DTO
{
    public class BookingDetailDTO
    {
        public string? razorpay_payment_id { get; set; }
        public string? razorpay_order_id { get; set; }
        public string? status { get; set; }
        public List<Booking>? selectedSeats { get; set; }
        public BookingOrder? bookingOrder { get; set; }
    }
}
