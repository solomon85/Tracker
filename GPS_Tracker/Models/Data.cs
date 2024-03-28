using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS_Tracker.Models
{
    internal class Data
    {
        internal class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Data>
        {
            public Configuration()
            {
                ToTable("Data");
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
        public DateTime DataDeviceTime { get; set; }
        public DateTime DataServerTime { get; set; }
        public int DataLatitude { get; set; }
        public int DataLongitude { get; set; }
        public Int16 DataAltitude { get; set; }
        public Int16 DataAnalogIn { get; set; }
        public bool DataDigitalIn { get; set; }
        public Int16 DataAngel { get; set; }
        public byte DataSpeed { get; set; }
        public Int16 DataAreaCode { get; set; }
        public double DataVehicleBatteryVoltage { get; set; }
        public double DataDeviceBatteryVoltage { get; set; }
        public byte DataDeviceBatteryPercent { get; set; }
        public Int16 DataCellId { get; set; }
        public Int16 DataDistanceTraveled { get; set; }

        public Int16 DataDriverId { get; set; }
        public byte DataFuel { get; set; }
        public byte DataGPSState { get; set; }
        public byte DataSatellites { get; set; }
        public byte DataGSMState { get; set;}
        public Int16 DataGSMOperatorCode { get; set; }
        public byte DataOverSpeed { get;set; }
        public byte DataTempture { get;set; }
        public Int16 DataDeviceDistanceTraveled { get;set; }

    }
}
