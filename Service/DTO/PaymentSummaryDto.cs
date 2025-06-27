using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO
{
    public class PaymentSummaryDto
    {
        public string Time { get; set; } // Day (yyyy-MM-dd) or Month (yyyy-MM)
        public decimal TotalAmount { get; set; }

    }
    public class PaymentSummaryRequest
    {
        [Required]
        public string dayOrMonth { get; set; } = "day"; // "day" or "month"
        public int startMonth { get; set; }
        public int startYear { get; set; }
        public int startDay { get; set; }
        public int endMonth { get; set; }
        public int endYear { get; set; }
        public int endDay { get; set; }

        public PaymentSummaryRequest()
        {
            var now = DateTime.Now;
            if (dayOrMonth.ToLower() == "month")
            {
                startMonth = 1;
                startYear = now.Year;
                startDay = 1;
                endMonth = now.Month;
                endYear = now.Year;
                endDay = DateTime.DaysInMonth(now.Year, now.Month);
            }
            else
            {
                startMonth = now.Month;
                startYear = now.Year;
                startDay = 1;
                endMonth = now.Month;
                endYear = now.Year;
                endDay = DateTime.DaysInMonth(now.Year, now.Month);
            }
        }
    }
}