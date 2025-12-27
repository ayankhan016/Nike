using System;
using Razorpay.Api;

namespace P2WebMVC.Services
{
    public class RazorPayService
    {
        private readonly string key = "rzp_test_RgG9AwfnysizZu";     // Use your test key
        private readonly string secret = "1CaqfrrZdfeKKOIdPypjADfr"; // Use your test secret

        public Order? CreateOrder(int amount, string currency, Guid orderId)
        {
            try
            {
                // Amount validation
                if (amount <= 0)
                    throw new ArgumentException("Amount must be greater than 0");

                RazorpayClient client = new(key, secret);

                var options = new Dictionary<string, object>
                {
                    { "amount", amount * 100 },  // Razorpay expects amount in paisa
                    { "currency", currency },
                    { "receipt", orderId.ToString() },
                    { "payment_capture", 1 }
                };

                Order order = client.Order.Create(options);
                Console.WriteLine($"✅ Razorpay Order Created: {order["id"]}");
                return order;
            }
            catch (Razorpay.Api.Errors.BadRequestError ex)
            {
                Console.WriteLine($"❌ Razorpay BadRequestError: {ex.Message}");
                return null;
            }
            catch (Razorpay.Api.Errors.ServerError ex)
            {
                Console.WriteLine($"❌ Razorpay ServerError: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Razorpay Unknown Error: {ex.Message}");
                return null;
            }
        }
    }
}
