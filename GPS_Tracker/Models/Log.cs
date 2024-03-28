using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS_Tracker.Models
{
    public class Log
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public String? LogType { get; set; }
        public String? LogContent { get; set; }
    }
}
