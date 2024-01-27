using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS_Tracker
{
    public class GPSdata
    {
        public int ID { get; set; }
        public string IMEI { get; set; }
        public long? Timestamp { get; set; }
        public DateTime? DeviceTime { get; set; }
        public byte? Priority { get; set; }
        public int? Long { get; set; }
        public int? Lat { get; set; }
        public short? Altitude { get; set; }
        public short? Direction { get; set; }
        public byte? Satellites { get; set; }
        public short? Speed { get; set; }
        public short? LocalAreaCode { get; set; }
        public short? CellID { get; set; }
        public byte? GsmSignalQuality { get; set; }
        public int? OperatorCode { get; set; }
        public byte Event_IO_element_ID { get; set; }
        public Dictionary<byte, byte> IO_Elements_1B { get; set; } = new Dictionary<byte, byte>();
        public Dictionary<byte, short> IO_Elements_2B { get; set; } = new Dictionary<byte, short>();
        public Dictionary<byte, int> IO_Elements_4B { get; set; } = new Dictionary<byte, int>();
        public Dictionary<byte, long> IO_Elements_8B { get; set; } = new Dictionary<byte, long>();
    }
}
