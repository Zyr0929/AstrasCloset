using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectAstra.Models;
using Resend; 
using System.Threading.Tasks;

namespace ProjectAstra.Pages
{
    public class PaymentSuccessModel : PageModel
    {
        private readonly AppDbContext _context;

        public Order? Order { get; set; }
        public string OrderId { get; set; } = "";

        public PaymentSuccessModel(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(string orderId)
        {
            OrderId = orderId;

            Order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ThenInclude(p => p.Variations)
                .FirstOrDefaultAsync(x => x.OrderReference == orderId);

            if (Order != null)
            {
                if (Order.PaymentStatus != "SETTLED")
                {
                    Order.PaymentStatus = "SETTLED";
                    await _context.SaveChangesAsync();

                    await SendInvoiceEmailAsync(Order);
                }
            }

            return Page();
        }

        private async Task SendInvoiceEmailAsync(Order order)
        {
            IResend resend = ResendClient.Create("re_Thz4XDDz_DBrCyv846BzL6cyTSnf89MeN");


            var htmlReceipt = $@"
    <div style='font-family: Arial, Helvetica, sans-serif; background-color: #f8f9fa; padding: 40px 10px;'>
        <div style='max-width: 500px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; border: 1px solid #e0e0e0; box-shadow: 0 2px 4px rgba(0,0,0,0.05); overflow: hidden;'>
            
            <div style='background-color: #0D4F4E; color: #ffffff; text-align: center; padding: 20px; font-weight: bold; text-transform: uppercase; letter-spacing: 2px; font-size: 18px;'>
                ASTRA'S Closet
            </div>

            <div style='padding: 30px;'>
                <div style='text-align: center; margin-bottom: 25px;'>
                    <div style='color: #198754; font-size: 45px; line-height: 1; margin-bottom: 10px;'>✓</div>
                    <h2 style='margin: 0 0 10px 0; color: #121212; font-size: 22px;'>Payment Confirmed</h2>
                    <p style='margin: 0; color: #6c757d; font-size: 14px;'>Your order has been successfully paid and recorded.</p>
                </div>

                <table width='100%' cellpadding='0' cellspacing='0' style='width: 100%; border-collapse: collapse;'>
                    <tr>
                        <td style='padding: 12px 0; border-bottom: 1px solid #f0f0f0; color: #6c757d; font-size: 11px; font-weight: bold; text-transform: uppercase;'>Order Reference</td>
                        <td style='padding: 12px 0; border-bottom: 1px solid #f0f0f0; color: #121212; font-weight: bold; text-align: right;'>{order.OrderReference}</td>
                    </tr>
                    <tr>
                        <td style='padding: 12px 0; border-bottom: 1px solid #f0f0f0; color: #6c757d; font-size: 11px; font-weight: bold; text-transform: uppercase;'>Customer</td>
                        <td style='padding: 12px 0; border-bottom: 1px solid #f0f0f0; color: #121212; text-align: right;'>{order.CustomerName}</td>
                    </tr>
                    <tr>
                        <td style='padding: 12px 0; border-bottom: 1px solid #f0f0f0; color: #6c757d; font-size: 11px; font-weight: bold; text-transform: uppercase;'>Year & Section</td>
                        <td style='padding: 12px 0; border-bottom: 1px solid #f0f0f0; color: #121212; text-align: right;'>{order.YearSection}</td>
                    </tr>
                    <tr>
                        <td style='padding: 12px 0; border-bottom: 1px solid #f0f0f0; color: #6c757d; font-size: 11px; font-weight: bold; text-transform: uppercase;'>Date Submitted</td>
                        <td style='padding: 12px 0; border-bottom: 1px solid #f0f0f0; color: #121212; text-align: right;'>{order.OrderDate.ToString("MMM dd, yyyy")}</td>
                    </tr>
                    <tr>
                        <td style='padding: 12px 0; border-bottom: 1px solid #f0f0f0; color: #6c757d; font-size: 11px; font-weight: bold; text-transform: uppercase;'>Time</td>
                        <td style='padding: 12px 0; border-bottom: 1px solid #f0f0f0; color: #121212; text-align: right;'>{order.OrderDate.ToString("hh:mm tt")}</td>
                    </tr>
                    <tr>
                        <td style='padding: 20px 0 0 0; color: #6c757d; font-size: 11px; font-weight: bold; text-transform: uppercase;'>Payment Status</td>
                        <td style='padding: 20px 0 0 0; text-align: right;'>
                            <span style='background-color: #198754; color: #ffffff; padding: 6px 12px; border-radius: 4px; font-size: 12px; font-weight: bold; letter-spacing: 1px;'>SETTLED</span>
                        </td>
                    </tr>
                </table>

            </div>
        </div>
        <div style='text-align: center; margin-top: 20px; color: #999; font-size: 12px;'>
            This is an automated receipt. Please do not reply to this email.
        </div>
    </div>";

            var message = new EmailMessage
            {
                From = "onboarding@resend.dev",
                Subject = $"Astra's Closet - Payment Receipt [{order.OrderReference}]",
                HtmlBody = htmlReceipt
            };

            message.To.Add("edcyruz54@gmail.com");

            await resend.EmailSendAsync(message);
        }
    }
}