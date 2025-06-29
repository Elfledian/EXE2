using Microsoft.Extensions.Options;
using Net.payOS.Types;
using Net.payOS;
using Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Repo.Entities;
using Microsoft.EntityFrameworkCore;
using Repo.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Service.Services
{
    public class PayOSService
    {
        private readonly PayOS _payOS;

        private readonly PaymentRepo _paymentRepo;
        private readonly SubscriptionRepo _subscriptionRepo;
        public PayOSService(IOptions<PayOSSettings> options, PaymentRepo paymentRepo)
        {
            var settings = options.Value;
            _payOS = new PayOS(settings.ClientId, settings.ApiKey, settings.ChecksumKey);
            _paymentRepo = paymentRepo;
        }

        // Existing methods
        public async Task<PaymentLinkInformation> GetPaymentByOrderCode(long orderCode)
        {
            return await _payOS.getPaymentLinkInformation(orderCode);
        }

        public async Task<string> CreatePaymentLinkAsync(CreatePaymentRequest request)
        {
            var items = request.Items.Select(i => new ItemData(i.Name, i.Quantity, i.Price)).ToList();

            var paymentData = new PaymentData(
                orderCode: request.OrderCode,
                amount: request.Amount,
                description: request.Description,
                items: items,
                cancelUrl: request.CancelUrl,
                returnUrl: request.ReturnUrl
            );

            var response = await _payOS.createPaymentLink(paymentData);

            var payment = new Payment
            {
                PaymentId = Guid.NewGuid(),
                Amount = request.Amount,
                TransactionId = response.orderCode.ToString(),
                Status = response.status, // Set initial status from PayOS response
                PaidAt = DateTime.UtcNow // Consider updating this based on actual payment time
            };

            await _paymentRepo.AddPaymentAsync(payment);

            return response.checkoutUrl;
        }

        // New method for filtered transactions
        public async Task<List<Payment>> GetFilteredPaymentsAsync(
            DateTime? startDate = null,
            DateTime? endDate = null,
            string status = null,
            bool syncWithPayOS = false)
        {
            // Get filtered payments from database  
            var payments = await _paymentRepo.GetAllPaymentsAsync(startDate, endDate, status);

            if (syncWithPayOS)
            {
                // Sync status with PayOS for each payment  
                foreach (var payment in payments)
                {
                    if (long.TryParse(payment.TransactionId, out long orderCode))
                    {
                        var paymentInfo = await _payOS.getPaymentLinkInformation(orderCode);
                        if (paymentInfo.status != payment.Status)
                        {
                            payment.Status = paymentInfo.status;

                            // Fix: Convert 'createdAt' string to DateTime before assignment  
                            if (DateTime.TryParse(paymentInfo.createdAt, out var createdAt))
                            {
                                payment.PaidAt = paymentInfo.status == "PAID" ? createdAt : payment.PaidAt;
                            }

                            await _paymentRepo.UpdatePaymentAsync(payment);
                        }
                    }
                }
            }

            return payments;
        }

        public async Task HandleWebhookAsync(WebhookType webhookBody)
        {
            // Verify webhook data
            var webhookData = _payOS.verifyPaymentWebhookData(webhookBody);

            // Map webhook code to payment status
            string status = webhookBody.code == "00" ? "PAID" : "FAILED";

            // Parse transactionDateTime to DateTime (assuming format like "2023-11-21 15:20:34")
            DateTime? transactionDate = null;
            if (!string.IsNullOrEmpty(webhookData.transactionDateTime))
            {
                if (DateTime.TryParse(webhookData.transactionDateTime, out var parsedDate))
                {
                    transactionDate = parsedDate.ToUniversalTime().AddHours(7); // Convert to GMT+7
                }
            }

            // Query payment by TransactionId
            var payment = await _paymentRepo.GetPaymentByTransactionIdAsync(webhookData.orderCode.ToString());

            if (payment != null)
            {
                // Update existing payment
                payment.Status = status;
                payment.PaidAt = status == "PAID" ? (transactionDate ?? payment.PaidAt) : payment.PaidAt;
                payment.Amount = webhookData.amount; // Update amount in case it changed
                await _paymentRepo.UpdatePaymentAsync(payment);

                if (status == "PAID")
                {
                    // Create subscription only for successful payments
                    var subscription = new Subscription
                    {
                        SubscriptionId = Guid.NewGuid(),
                        UserId = payment.UserId,
                        IsActivated = true,
                        Price = webhookData.amount,
                        Subtitle = webhookData.description,
                        OriginalPrice = payment.Amount,
                        StartDate = transactionDate ?? DateTime.UtcNow.AddHours(7),
                        EndDate = transactionDate.HasValue ? transactionDate.Value.AddDays(30) : DateTime.UtcNow.AddHours(7).AddDays(30),
                        Status = "Active"
                    };
                    // Assuming a subscription repository exists
                    await _subscriptionRepo.AddAsync(subscription);
                }
            }
            else
            {
                // Create new payment
                payment = new Payment
                {
                    PaymentId = Guid.NewGuid(),
                    Amount = webhookData.amount,
                    TransactionId = webhookData.orderCode.ToString(),
                    Status = status,
                    PaidAt = status == "PAID" ? (transactionDate ?? DateTime.UtcNow.AddHours(7)) : DateTime.UtcNow.AddHours(7)
                };
                await _paymentRepo.AddPaymentAsync(payment);

                if (status == "PAID")
                {
                    // Create subscription only for successful payments
                    var subscription = new Subscription
                    {
                        SubscriptionId = Guid.NewGuid(),
                        UserId = payment.UserId, // Assumes payment.UserId is available or needs to be set
                        IsActivated = true,
                        Price = webhookData.amount,
                        Subtitle = webhookData.description,
                        OriginalPrice = payment.Amount,
                        StartDate = transactionDate ?? DateTime.UtcNow.AddHours(7),
                        EndDate = transactionDate.HasValue ? transactionDate.Value.AddDays(30) : DateTime.UtcNow.AddHours(7).AddDays(30),
                        Status = "Active"
                    };
                    // Assuming a subscription repository exists
                    await _subscriptionRepo.AddAsync(subscription);
                }
            }
        }
        public async Task<List<PaymentSummaryDto>> GetDailyTotalsAsync(DateTime startDate, DateTime endDate)
        {
            var payments = await _paymentRepo.GetAllPaymentsAsync(startDate, endDate, "PAID");
            var paymentGroups = payments
                .Where(p => p.PaidAt.HasValue)
                .GroupBy(p => p.PaidAt.Value.Date)
                .ToDictionary(g => g.Key, g => g.Sum(p => p.Amount));

            var result = new List<PaymentSummaryDto>();
            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                paymentGroups.TryGetValue(date, out var total);
                result.Add(new PaymentSummaryDto
                {
                    Time = date.ToString("yyyy-MM-dd"),
                    TotalAmount = total
                });
            }
            return result;
        }


        public async Task<List<PaymentSummaryDto>> GetMonthlyTotalsAsync(int startMonth, int startYear, int endMonth, int endYear)
        {
            var startDate = new DateTime(startYear, startMonth, 1);
            var endDate = new DateTime(endYear, endMonth, DateTime.DaysInMonth(endYear, endMonth));
            var payments = await _paymentRepo.GetAllPaymentsAsync(startDate, endDate, "PAID");
            var paymentGroups = payments
                .Where(p => p.PaidAt.HasValue)
                .GroupBy(p => new { p.PaidAt.Value.Year, p.PaidAt.Value.Month })
                .ToDictionary(
                    g => (g.Key.Year, g.Key.Month),
                    g => g.Sum(p => p.Amount)
                );

            var result = new List<PaymentSummaryDto>();
            var current = new DateTime(startYear, startMonth, 1);
            var last = new DateTime(endYear, endMonth, 1);

            while (current <= last)
            {
                paymentGroups.TryGetValue((current.Year, current.Month), out var total);
                result.Add(new PaymentSummaryDto
                {
                    Time = $"{current.Year}-{current.Month:D2}",
                    TotalAmount = total
                });
                current = current.AddMonths(1);
            }
            return result;
        }
        public async Task<List<PaymentSummaryDto>> GetDataForChartAsync(PaymentSummaryRequest request)
        {
            var result = new List<PaymentSummaryDto>();
            if (request == null)
                return result;

            // Normalize input
            var dayOrMonth = request.dayOrMonth?.ToLower() ?? "day";

            DateTime startDate, endDate;

            if (dayOrMonth == "month")
            {
                // Use only year and month, day is always 1 for start, last day for end
                startDate = new DateTime(request.startYear, request.startMonth, 1);
                endDate = new DateTime(request.endYear, request.endMonth, DateTime.DaysInMonth(request.endYear, request.endMonth));
                if (startDate > endDate)
                {
                    var tempMonth = startDate;
                    startDate = endDate;
                    endDate = tempMonth;
                }
                result = await GetMonthlyTotalsAsync(startDate.Month, startDate.Year, endDate.Month, endDate.Year);
            }
            else // "day" or default
            {
                // Use full date
                startDate = new DateTime(request.startYear, request.startMonth, request.startDay);
                endDate = new DateTime(request.endYear, request.endMonth, request.endDay);
                if (startDate > endDate)
                {
                    var tempDate = startDate;
                    startDate = endDate;
                    endDate = tempDate;
                }
                result = await GetDailyTotalsAsync(startDate, endDate);
            }

            return result;
        }


        public async Task<List<PaymentSummaryDto>> GetDataForChartAsync(String DayOrMonth, DateTime startDate, DateTime endDate)
        {
            var result = new List<PaymentSummaryDto>();
            if (startDate > endDate)
            {
                var temp = startDate;
                startDate = endDate;
                endDate = temp;
            }
            if (DayOrMonth.ToLower() == "day")
            {
                result = await GetDailyTotalsAsync(startDate, endDate);
                return result;
            }
            else if (DayOrMonth.ToLower() == "month")
            {
                // Extract start and end month/year from the provided dates
                int startMonth = startDate.Month;
                int startYear = startDate.Year;
                int endMonth = endDate.Month;
                int endYear = endDate.Year;
                result = await GetMonthlyTotalsAsync(startMonth, startYear, endMonth, endYear);
                return result;
            }
            else
            {
                result = new List<PaymentSummaryDto>();
            }
            return result;
        }

    }
}