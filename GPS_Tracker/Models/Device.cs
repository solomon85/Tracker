using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS_Tracker.Models
{
    internal class Device
    {
        public Int16 DeviceId { get; set; }
        public long DeviceIMEI { get; set; }
        public DateTime? DeviceFirstReceivedDate { get; set; }
        public bool? DeviceIsActive { get; set; }
        public Int16 DeviceModelId { get; set; }
        public string DeviceSimCardNumber { get; set; }
        public byte DeviceSimCardOperatorId { get; set; }
        public int? DeviceSmsRemainCount { get; set; }
        public byte? DeviceGPSTypeId { get; set; }
        public DateTime? DeviceValidDateTo { get; set; }
        public int DeviceVehicleId { get; set; }
        public string DeviceName { get; set; } = string.Empty;
    }
}
