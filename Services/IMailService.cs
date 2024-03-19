using movie_ticket_booking.Models.EmailService;

namespace movie_ticket_booking.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
