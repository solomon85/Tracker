using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS_Tracker.Models
{
    internal class TowingData
    {
        internal class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<TowingData>
        {
            public Configuration()
            {
                ToTable("TowingData");
                //HasRequired(u => u.ProjectItem)
                //    .WithMany(p => p.Assets)
                //    .HasForeignKey(u => u.ProjectID)
                //    .WillCascadeOnDelete(false)
                //    ;
            }
        }
        [Key, Column(Order = 0)]
        public Int16 DeviceId { get; set; }
        [Key, Column(Order = 1)]
        public DateTime TowingDataDeviceTime { get; set; }
        public DateTime TowingDataServerTime { get; set; }
        public int TowingDataLatitude { get; set; }
        public int TowingDataLongitude { get; set; }
        public Int16 TowingDataAltitude { get; set; }
        public Int16 TowingDataAnalogIn { get; set; }
        public bool TowingDataDigitalIn { get; set; }
        public Int16 TowingDataAngel { get; set; }
        public byte TowingDataSpeed { get; set; }
        public Int16 TowingDataAreaCode { get; set; }
        public double TowingDataVehicleBatteryVoltage { get; set; }
        public double TowingDataDeviceBatteryVoltage { get; set; }
        public byte TowingDataDeviceBatteryPercent { get; set; }
        public Int16 TowingDataCellId { get; set; }
        public Int16 TowingDataDistanceTraveled { get; set; }

        public Int16 TowingDataDriverId { get; set; }
        public byte TowingDataFuel { get; set; }
        public byte TowingDataGPSState { get; set; }
        public byte TowingDataSatellites { get; set; }
        public byte TowingDataGSMState { get; set; }
        public Int16 TowingDataGSMOperatorCode { get; set; }
        public byte TowingDataOverSpeed { get; set; }
        public byte TowingDataTempture { get; set; }
        public Int16 TowingDataDeviceDistanceTraveled { get; set; }

    }
}
