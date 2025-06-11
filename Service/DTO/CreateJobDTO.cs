using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO
{
    public class CreateJobDTO
    {
        public string Title { get; set; }
        public string Position { get; set; }
        public string Requirement { get; set; }
        public string Description { get; set; }
        public string JobDetails { get; set; }

        public string Requirements { get; set; }
        public string Experience { get; set; }

        public string Benefits { get; set; }

        public decimal Salary { get; set; }
        public Guid? CategoryId { get; set; }
        public string ContactPhone { get; set; }
        public bool? IsUrgent { get; set; }
    }
}
