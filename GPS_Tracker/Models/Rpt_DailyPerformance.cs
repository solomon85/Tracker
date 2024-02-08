using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS_Tracker.Models
{
    internal class Rpt_DailyPerformance
    {
        [Key, Column(Order = 0)]
        public Int16 DeviceId { get; set; }
        [Key, Column(Order = 1)]
        public DateTime ReportDate { get; set; }
        public int TotalMovingTime { get; set; }
        public int TotalMovingDistance { get; set; }
        public int TotalParkTime { get; set; }
        public int TotalStandByTime { get; set; }
        public int TotalTowingTime { get; set; }
    }
}
