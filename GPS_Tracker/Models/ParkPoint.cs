using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS_Tracker.Models
{
    internal class ParkPoint
    {
        [Key]
        public long ParkId { get; set; }
        public Int16 ParkDeviceId { get; set; }
        public DateTime ParkStartTime { get; set; }
        public DateTime ParkEndTime { get; set; }
        public int ParkLatitude { get; set; }
        public int ParkLongitude { get; set; }
    }
}
