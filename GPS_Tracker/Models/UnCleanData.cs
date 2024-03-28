using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS_Tracker.Models
{
    internal class UnCleanData
    {
        internal class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<UnCleanData>
        {
            public Configuration()
            {
                ToTable("UnCleanData");
                //HasRequired(u => u.ProjectItem)
                //    .WithMany(p => p.Assets)
                //    .HasForeignKey(u => u.ProjectID)
                //    .WillCascadeOnDelete(false)
                //    ;
            }
        }
        public long Id { get; set; }
        public Int16 DeviceId { get; set; }
        public DateTime DeviceTime { get; set; }
        public DateTime ServerTime { get; set; }
        public int Latitude { get; set; }
        public int Longitude { get; set; }
        public Int16 Altitude { get; set; }
        public Int16 AnalogIn { get; set; }
        public bool DigitalIn { get; set; }
        public Int16 Angel { get; set; }
        public byte Speed { get; set; }
        public Int16 AreaCode { get; set; }
        public double VehicleBatteryVoltage { get; set; }
        public double DeviceBatteryVoltage { get; set; }
        public byte DeviceBatteryPercent { get; set; }
        public Int16 CellId { get; set; }
        public Int16 DistanceTraveled { get; set; }

        public Int16 DriverId { get; set; }
        public byte Fuel { get; set; }
        public byte GPSState { get; set; }
        public byte Satellites { get; set; }
        public byte GSMState { get; set; }
        public Int16 GSMOperatorCode { get; set; }
        public byte OverSpeed { get; set; }
        public byte Tempture { get; set; }
        public Int16 DeviceDistanceTraveled { get; set; }
    }
}
