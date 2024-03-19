namespace movie_ticket_booking.Models.DTO
{
    public class OrderRequestDTO
    {
        public int TotalAmount { get; set; }
        public string TransactionID { get; set; }
    }
    public class OrderResponseDTO
    {
        public string id { get; set; }
        public int amount { get; set; }
        public string entity { get; set; }
        public int amount_paid { get; set; }
        public int amount_due { get; set; }
        public string currency { get; set; }
        public string receipt { get; set; }
        public string status { get; set; }
        public int attempts { get; set; }
        public int created_at { get; set; }
        public BookingOrder bookingOrder { get; set; }
    }
}
