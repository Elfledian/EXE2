using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO
{
    public class ItemRequest
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
    }
}
