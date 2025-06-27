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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Validate the date range
            if (request.startYear < 1 || request.endYear < 1 ||
                request.startMonth < 1 || request.startMonth > 12 ||
                request.endMonth < 1 || request.endMonth > 12 ||
                request.startDay < 1 || request.startDay > 31 ||
                request.endDay < 1 || request.endDay > 31)
            {
                if(request.dayOrMonth.ToLower() != "day" && request.dayOrMonth.ToLower() != "month")
                request = new PaymentSummaryRequest();
                if(request.dayOrMonth.ToLower() == "month")
                request = new PaymentSummaryRequest() { 
                    dayOrMonth = "month",
                    startMonth = 1,
                    startYear = DateTime.Now.Year,
                    startDay = 1
                };
            }
            var startDate = new DateTime(request.startYear, request.startMonth, request.startDay);
            var endDate = new DateTime(request.endYear, request.endMonth, request.endDay);
            try
            {
                var chartData = await _payOSService.GetDataForChartAsync(request.dayOrMonth, startDate, endDate);
                return Ok(chartData);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }

}
