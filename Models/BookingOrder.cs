using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;

namespace movie_ticket_booking.Models
{
    public class BookingOrder
    {
        public int Id { get; set; }
        [Required]
        public string RazorpayOrderID { get; set; }
        public string? RazorPayPaymentId { get; set; }
        public string? Status { get; set; }
        [Required]
        public string? Receipt { get; set; }
        [Required]
        public DateOnly OrderDate { get; set; }
        [Required]
        public double TotalAmount { get; set; }
        public string UserId { get; set; }
        public virtual User? User { get; set; }
    }
}

