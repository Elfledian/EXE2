using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO
{
    public class RatingDto
    {
        public Guid RatingId { get; set; }
        public int? Rating1 { get; set; }
        public string Comment { get; set; }
        public string ContributedComment { get; set; }
    }
    public class RatingDtoCreate
    {
        public int? Rating1 { get; set; }
        public string Comment { get; set; }
        public string ContributedComment { get; set; }
    }

}
