
using Online_training.Server.Models;
using Stripe;

namespace Online_training.Server.Services
{
    public class PaymentService
    {

        private readonly IConfiguration _config;
        public PaymentService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<PaymentIntent> PaymentIntentAsync(Panier basket)
        {
            StripeConfiguration.ApiKey = _config["Stripe:private_key"];

            var service = new PaymentIntentService();

            var intent = new PaymentIntent();

            var total = basket.PanierItems.Sum(item => item.Formation!.Price);

            long updatedTotal = (long)(total * 100);

            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = updatedTotal,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" }
                };

                intent = await service.CreateAsync(options);

            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = updatedTotal
                };

                await service.UpdateAsync(basket.PaymentIntentId, options);
            }

            return intent;
        }
    }
}