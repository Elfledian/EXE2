using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using Service.DTO;
using Service.Services;

namespace FastWork.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly PayOSService _payOSService;

        public PaymentController(PayOSService payOSService)
        {
            _payOSService = payOSService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            var checkoutUrl = await _payOSService.CreatePaymentLinkAsync(request);
            return Ok(new { checkoutUrl });
        }

        [HttpGet("{orderCode}")]
        public async Task<IActionResult> GetPayment(long orderCode)
        {
            var info = await _payOSService.GetPaymentByOrderCode(orderCode);
            return Ok(info);
        }
        [HttpGet]
        public IActionResult GetAllPayments()
        {
            var all = _payOSService.GetFilteredPaymentsAsync().Result;
            return Ok(all);
        }
        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook([FromBody] WebhookType webhookBody)
        {
            try
            {
                await _payOSService.HandleWebhookAsync(webhookBody);
                return Ok(new { Message = "Webhook processed successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpPost("paymentChartData")]
        public async Task<IActionResult> GetPaymentChartData([FromBody] PaymentSummaryRequest request)
        {
            try
            {
                var chartData = await _payOSService.GetDataForChartAsync(request);
                return Ok(chartData);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }

}
