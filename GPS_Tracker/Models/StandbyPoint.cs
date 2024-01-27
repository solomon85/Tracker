using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS_Tracker.Models
{
    internal class StandbyPoint
    {
        [Key]
        public long StandbyId { get; set; }
        public Int16 StandbyDeviceId { get; set; }
        public DateTime StandbyStartTime { get; set; }
        public DateTime StandbyEndTime { get; set; }
        public int StandbyLatitude { get; set; }
        public int StandbyLongitude { get; set; }
    }
}
