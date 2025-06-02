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
            string status = webhookBody.code == "00" ? "PAID" : "PENDING"; // Adjust based on PayOS documentation

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
            }
        }
    }

}
