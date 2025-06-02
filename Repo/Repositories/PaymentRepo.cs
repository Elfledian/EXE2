using Microsoft.EntityFrameworkCore;
using Repo.Data;
using Repo.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.Repositories
{
    public class PaymentRepo
    {
        private readonly TheShineDbContext _context;
        // Constructor to initialize the context
        public PaymentRepo()
        {
            _context = new TheShineDbContext();
        }
        public PaymentRepo(TheShineDbContext context)
        {
            _context = context;
        }
        public async Task<List<Payment>> GetAllPaymentsAsync()
        {
            return await _context.Payments.ToListAsync();
        }
        public async Task<Payment> GetPaymentByTransactionIdAsync(string transactionId)
        {
            return await _context.Payments.FirstOrDefaultAsync(p => p.TransactionId == transactionId);
        }
        
        public async Task<List<Payment>> GetAllPaymentsAsync(
            DateTime? startDate = null,
            DateTime? endDate = null,
            string status = null)
        {
            var query = _context.Payments.AsQueryable();

            // Filter by date range
            if (startDate.HasValue)
                query = query.Where(p => p.PaidAt >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(p => p.PaidAt <= endDate.Value);

            // Filter by status
            if (!string.IsNullOrEmpty(status))
                query = query.Where(p => p.Status == status);

            return await query.ToListAsync();
        }

        public async Task AddPaymentAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePaymentAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
        }
    }
}
