using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomersMicroservice.Models
{
    public class DateRange
    {
        public DateOnly MinDate { get; set; }
        public DateOnly MaxDate { get; set;}
    }
}
