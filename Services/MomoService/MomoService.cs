using Ecommerce.DTO;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Crmf;
using RestSharp;
using System.Security.Cryptography;
using System.Text;
using static QRCoder.PayloadGenerator.ShadowSocksConfig;


namespace Ecommerce.Services.MomoService
{
    public class MomoService : IMomoService
    {
        private readonly IOptions<MomoOptionModel> _options;
        public MomoService(IOptions<MomoOptionModel> options)
        {
            _options = options;
        }

        public async Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(OrderInfo model)
        {
            string orderId = model.OrderId.ToString();
            string orderInfoText = $"Thanh toan don hang {orderId} - Khach hang Luu Quang Phu";
            string finalReturnUrl = string.IsNullOrEmpty(model.ReturnUrl) ? _options.Value.ReturnUrl : model.ReturnUrl;
            long amountLong = Convert.ToInt64(model.Amount);
            string requestId = orderId; // Có thể dùng Guid.NewGuid().ToString() nếu muốn
            string extraData = "";
            string requestType = "captureWallet";

            var rawData =
                $"accessKey={_options.Value.AccessKey}" +
                $"&amount={amountLong}" +
                $"&extraData={extraData}" +
                $"&ipnUrl={_options.Value.NotifyUrl}" +
                $"&orderId={orderId}" +
                $"&orderInfo={orderInfoText}" +
                $"&partnerCode={_options.Value.PartnerCode}" +
                $"&redirectUrl={finalReturnUrl}" +
                $"&requestId={requestId}" +
                $"&requestType={requestType}";

            var signature = ComputeHmacSha256(rawData, _options.Value.SecretKey);

            // 3. Cấu hình RestClient với UserAgent để qua mặt Nginx 403
            var options = new RestClientOptions(_options.Value.MomoApiUrl)
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko)",
                MaxTimeout = 30000
            };
            var client = new RestSharp.RestClient(options);
            var request = new RestSharp.RestRequest("", RestSharp.Method.Post);

            // 4. Body JSON
            var requestData = new
            {
                partnerCode = _options.Value.PartnerCode,
                partnerName = "Hutech Restaurant",
                storeId = _options.Value.PartnerCode,
                requestId = requestId,
                amount = amountLong,
                orderId = orderId,
                orderInfo = orderInfoText,
                redirectUrl = finalReturnUrl, 
                ipnUrl = _options.Value.NotifyUrl,
                lang = "vi",
                extraData = extraData,
                requestType = requestType,
                signature = signature
            };

            request.AddHeader("Content-Type", "application/json; charset=UTF-8");
            request.AddJsonBody(requestData);

            var response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful || response.Content.StartsWith("<"))
            {
                Console.WriteLine("--- MOMO DEBUG ---");
                Console.WriteLine("RawData: " + rawData);
                Console.WriteLine("Response Content: " + response.Content);
                throw new Exception($"Momo API Error: {response.StatusCode}. Vui lòng kiểm tra MomoApiUrl trong appsettings.");
            }

            return JsonConvert.DeserializeObject<MomoCreatePaymentResponseModel>(response.Content);
        }

        public MomoExecuteResponseModel PaymentExecute(IQueryCollection collection)
        {
            var amount = collection.First(s => s.Key == "amount").Value;
            var orderInfo = collection.First(s => s.Key == "orderInfo").Value;
            var orderId = collection.First(s => s.Key == "orderId").Value;

            return new MomoExecuteResponseModel()
            {
                Amount = amount,
                OrderId = orderId,
                OrderInfo = orderInfo

            };
        }
        private string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            byte[] hashBytes;

            using (var hmac = new HMACSHA256(keyBytes))
            {
                hashBytes = hmac.ComputeHash(messageBytes);
            }

            var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return hashString;
        }
    }
}
