using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;
using movie_ticket_booking.Models;
using movie_ticket_booking.Models.DTO;
using Razorpay.Api;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace movie_ticket_booking.Controllers
{
   
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public BookingController(ApplicationDbContext applicationDbContext) 
        {
            _applicationDbContext = applicationDbContext;
        }

        [Authorize]
        [Route("api/get-all-booking")]
        [HttpGet]
        public async Task<ActionResult<List<Booking>>> GetAllBooking()
        {
            return await _applicationDbContext.Bookings.ToListAsync();
        }

        [Route("api/get-ticket")]
        [HttpGet]
        public async Task<ActionResult<Booking>> GetBookingTicket(int id)
        {
            return _applicationDbContext.Bookings.FirstOrDefault(x => x.Id == id);

        }

        /*[Authorize]
        [Route("api/getBookingByUser")]
        [HttpGet]
        public async Task<ActionResult<List<Booking>>> GetBookingByUser(string userId)
        {
            return await _applicationDbContext.Bookings.Where(x => x.UserId == userId).ToListAsync();
        }*/


        [Authorize]
        [Route("api/book-ticket")]
        [HttpPost]
        public async Task<ActionResult<Booking>> BookTicket(BookingDTO bookingDTO, string userId)
        {
            //Get All Seats based on current ShowTime
            var TotalAvailableSeats = _applicationDbContext.Seats.Where(x => x.ShowTimeId == bookingDTO.showTimeId);
            double totalPrice = 0;
            
            var selectedBooking = new List<Booking>();
       
            //Genrate recipt number, which will be act as a forigen key to Order table
            Random _random = new Random();
            string TransactionID = _random.Next(0, 10000).ToString();

            foreach (var seat in bookingDTO.seatNumber)
            {
                var selectedSeat = TotalAvailableSeats.Where(x => x.Row == seat.row && x.Number == seat.number).FirstOrDefault();

                if (selectedSeat.IsAvailable == true)
                {
                    var amount = _applicationDbContext.ShowTimes.Where(x => x.Id == selectedSeat.ShowTimeId).FirstOrDefault().Amount;
                    selectedSeat.IsAvailable = false;
                    _applicationDbContext.Seats.Update(selectedSeat);

                    var bookedTicket = new Booking
                    {
                        BookingDate = DateOnly.FromDateTime(DateTime.Now),
                        SeatId = TotalAvailableSeats.Where(x => x.Row == seat.row && x.Number == seat.number).FirstOrDefault().Id,
                        ShowTimeId = bookingDTO.showTimeId,
                        /*UserId = userId*/
                        Receipt = TransactionID
                    };
                    totalPrice += amount;
                    selectedBooking.Add(bookedTicket);
                   // _applicationDbContext.Bookings.Add(bookedTicket);
                    //await _applicationDbContext.SaveChangesAsync();
                }
                else
                {
                    return Ok(new BookingResponseDTO() { selectedSeats = selectedBooking, TotalAmount = totalPrice, TransactionID = TransactionID, Message = "Seats are not available", Status = 201});
                }

            }

            //create razorpay order
            //get razorpay order id
            //add into order table id{PK},razorpayOrderID,Date,totalAmount,userId)

            return Ok(new BookingResponseDTO() { selectedSeats = selectedBooking, TotalAmount = totalPrice, TransactionID = TransactionID, Message = "Seats are available, Do the Payment", Status = 200 });
        }
        //orderresponse,bookedTicket,bookingorder
        [HttpPost]
        [Route("/intitae-payment")]
        public async Task<IActionResult> InitatePayment(OrderRequestDTO orderReqDTO, string userId)
        {
            string key = "rzp_test_MVtbvvIPLifFCR";
            string secret = "uVUVh6FudJ2Mc6EKUoadsvxe";
            
            Dictionary<string, object> input = new Dictionary<string, object>
            {
                { "amount", Convert.ToDecimal(orderReqDTO.TotalAmount)*100 },
                { "currency", "INR" },
                { "receipt", orderReqDTO.TransactionID }
            };

            RazorpayClient client = new RazorpayClient(key, secret);
            Order order = client.Order.Create(input);

            //get razorpay order id
            //add into order table (id{PK},razorpayOrderID,Date,totalAmount,userId)
            BookingOrder bookingOrder = new BookingOrder()
            {
                RazorpayOrderID = order["id"],
                OrderDate = DateOnly.FromDateTime(DateTime.Now),
                TotalAmount = orderReqDTO.TotalAmount,
                Status = "Created",
                Receipt = orderReqDTO.TransactionID,
                RazorPayPaymentId = null,
                UserId = userId
            };
            _applicationDbContext.BookingOrders.Add(bookingOrder);
            await _applicationDbContext.SaveChangesAsync();

            //razorpay order id,total amount, user id, status
            /*string orderId = order["id"].ToString();*/
            OrderResponseDTO orderResponseDTO = new OrderResponseDTO()
            {
                id = order["id"],
                amount = order["amount"],
                entity = order["entity"],
                amount_paid = order["amount_paid"],
                amount_due = order["amount_due"],
                currency = order["currency"],
                receipt = order["receipt"],
                attempts = order["attempts"],
                status = order["status"],
                created_at = order["created_at"],
                bookingOrder = bookingOrder
            };
            return Ok(orderResponseDTO);
        }

        [HttpPost]
        [Route("/api/updateOnServer")]
        public async Task<IActionResult> UpdateOnServer(BookingDetailDTO bookingDetailDTO)
        {
            var existingBookingOrder = _applicationDbContext.BookingOrders.Where(x => x.RazorpayOrderID == bookingDetailDTO.razorpay_order_id).FirstOrDefault();

            if (existingBookingOrder == null)
            {
                return BadRequest();
                //Order is not generated
            }
            else
            {
                //Updated existing booking order with paymentID and status
                existingBookingOrder.RazorPayPaymentId = bookingDetailDTO.razorpay_payment_id;
                existingBookingOrder.Status = bookingDetailDTO.status;
                //existingBookingOrder.Receipt = bookingDetailDTO.selectedSeats.FirstOrDefault().Receipt;
                _applicationDbContext.BookingOrders.Update(existingBookingOrder);
                await _applicationDbContext.SaveChangesAsync();

                //Add selectedSetas into Booking table
                foreach(var selectedSeat in bookingDetailDTO.selectedSeats)
                {
                    var seatToMark = _applicationDbContext.Seats.Where(x => x.Id == selectedSeat.SeatId).FirstOrDefault();
                    seatToMark.IsAvailable = false;
                    _applicationDbContext.Seats.Update(seatToMark);
                    await _applicationDbContext.SaveChangesAsync();
                }
                _applicationDbContext.Bookings.AddRange(bookingDetailDTO.selectedSeats);
                await _applicationDbContext.SaveChangesAsync();

                return Ok("Everything is done");
            }
            return Ok();
        }

    }
}
