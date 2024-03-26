using GPS_Tracker.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GPS_Tracker
{
    public class FMXXXX_Parser : ParserBase
    {
        ApplicationDataContext db = new ApplicationDataContext();
        public FMXXXX_Parser(bool showDiagnosticInfo)
        {
            _showDiagnosticInfo = showDiagnosticInfo;
        }
        public override int DecodeAVL(List<byte> receiveBytes, string IMEI)
        {
            string hexDataLength = string.Empty;
            receiveBytes.Skip(4).Take(4).ToList().ForEach(delegate (byte b) { hexDataLength += string.Format("{0:X2}", b); });
            int dataLength = Convert.ToInt32(hexDataLength, 16);


            //ShowDiagnosticInfo("Data Length: -----".PadRight(40, '-') + " " + dataLength);
            int codecId = Convert.ToInt32(receiveBytes.Skip(8).Take(1).ToList()[0]);
            //ShowDiagnosticInfo("Codec ID: -----".PadRight(40, '-') + " " + codecId);
            int numberOfData = Convert.ToInt32(receiveBytes.Skip(9).Take(1).ToList()[0]); ;
            //ShowDiagnosticInfo("Number of data: ----".PadRight(40, '-') + " " + numberOfData);
            var deviceId = Redis.GetCacheData<Int16>(IMEI);

            int tokenAddress = 10;
            for (int n = 0; n < numberOfData; n++)
            {
                GPSdata gpsData = new GPSdata();
                string hexTimeStamp = string.Empty;
                receiveBytes.Skip(tokenAddress).Take(8).ToList().ForEach(delegate (byte b) { hexTimeStamp += String.Format("{0:X2}", b); });
                long timeStamp = Convert.ToInt64(hexTimeStamp, 16);

                DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                DateTime deviceTime = origin.AddMilliseconds(Convert.ToDouble(timeStamp));


                int priority = Convert.ToInt32(receiveBytes.Skip(tokenAddress + 8).Take(1).ToList()[0]);
                //  ShowDiagnosticInfo("Priority: ------------".PadRight(40, '-') + " " + priority);

                string longt = string.Empty;
                receiveBytes.Skip(tokenAddress + 9).Take(4).ToList().ForEach(delegate (byte b) { longt += String.Format("{0:X2}", b); });
                int longtitude = Convert.ToInt32(longt, 16);
                //ShowDiagnosticInfo("Longtitude: -----".PadRight(40, '-') + " " + longtitude);

                string lat = string.Empty;
                receiveBytes.Skip(tokenAddress + 13).Take(4).ToList().ForEach(delegate (byte b) { lat += String.Format("{0:X2}", b); });
                int latitude = Convert.ToInt32(lat, 16);
                //ShowDiagnosticInfo("Latitude: -----".PadRight(40, '-') + " " + latitude);

                string alt = string.Empty;
                receiveBytes.Skip(tokenAddress + 17).Take(2).ToList().ForEach(delegate (byte b) { alt += String.Format("{0:X2}", b); });
                Int16 altitude = Convert.ToInt16(alt, 16);
                //ShowDiagnosticInfo("Altitude: -----".PadRight(40, '-') + " " + altitude);

                string ang = string.Empty;
                receiveBytes.Skip(tokenAddress + 19).Take(2).ToList().ForEach(delegate (byte b) { ang += String.Format("{0:X2}", b); });
                var angle = Convert.ToInt16(ang, 16);
                //ShowDiagnosticInfo("Angle: -----".PadRight(40, '-') + " " + angle);

                byte satellites = (receiveBytes.Skip(tokenAddress + 21).Take(1).ToList()[0]);
                //ShowDiagnosticInfo("Satellites: -----".PadRight(40, '-') + " " + satellites);

                string sp = string.Empty;
                receiveBytes.Skip(tokenAddress + 22).Take(2).ToList().ForEach(delegate (byte b) { sp += String.Format("{0:X2}", b); });
                var speed = Convert.ToInt16(sp, 16);
                //ShowDiagnosticInfo("Speed: -----".PadRight(40, '-') + " " + speed);

                byte event_IO_element_ID = (byte)Convert.ToInt32(receiveBytes.Skip(tokenAddress + 24).Take(1).ToList()[0]);
                gpsData.Event_IO_element_ID = event_IO_element_ID;
                //ShowDiagnosticInfo("IO element ID of Event generated: ------".PadRight(40, '-') + " " + event_IO_element_ID);

                int IO_element_in_record = Convert.ToInt32(receiveBytes.Skip(tokenAddress + 25).Take(1).ToList()[0]);
                //ShowDiagnosticInfo("IO_element_in_record: --------".PadRight(40, '-') + " " + IO_element_in_record);


                if (IO_element_in_record != 0)
                {
                    int currentCursor = tokenAddress + 26;

                    int IO_Elements_1B_Quantity = Convert.ToInt32(receiveBytes.Skip(currentCursor).Take(1).ToList()[0]);
                    //ShowDiagnosticInfo("1 byte IO element in record: --------".PadRight(40, '-') + " " + IO_Elements_1B_Quantity);


                    for (int IO_1 = 0; IO_1 < IO_Elements_1B_Quantity; IO_1++)
                    {
                        var parameterID = (byte)Convert.ToInt32(receiveBytes.Skip(currentCursor + 1 + IO_1 * 2).Take(1).ToList()[0]);
                        var IO_Element_1B = (byte)Convert.ToInt32(receiveBytes.Skip(currentCursor + 2 + IO_1 * 2).Take(1).ToList()[0]);
                        gpsData.IO_Elements_1B.Add(parameterID, IO_Element_1B);
                        //ShowDiagnosticInfo("IO element 1B ID: --------".PadRight(40, '-') + " " + parameterID);
                        //ShowDiagnosticInfo(IO_1 + "'st 1B IO element value: --------".PadRight(40 - IO_1.ToString().Length, '-') + " " + IO_Element_1B);
                    }
                    currentCursor += IO_Elements_1B_Quantity * 2 + 1;

                    int IO_Elements_2B_Quantity = Convert.ToInt32(receiveBytes.Skip(currentCursor).Take(1).ToList()[0]);
                    //ShowDiagnosticInfo("2 byte IO element in record: --------".PadRight(40, '-') + " " + IO_Elements_2B_Quantity);

                    for (int IO_2 = 0; IO_2 < IO_Elements_2B_Quantity; IO_2++)
                    {
                        var parameterID = (byte)Convert.ToInt32(receiveBytes.Skip(currentCursor + 1 + IO_2 * 3).Take(1).ToList()[0]);
                        string value = string.Empty;
                        receiveBytes.Skip(currentCursor + 2 + IO_2 * 3).Take(2).ToList().ForEach(delegate (byte b) { value += String.Format("{0:X2}", b); });
                        var IO_Element_2B = Convert.ToInt16(value, 16);
                        gpsData.IO_Elements_2B.Add(parameterID, IO_Element_2B);
                        //ShowDiagnosticInfo("IO element 2B ID: --------".PadRight(40, '-') + " " + parameterID);
                        //ShowDiagnosticInfo(IO_2 + "'st 2B IO element value: --------".PadRight(40 - IO_2.ToString().Length, '-') + " " + IO_Element_2B);
                    }
                    currentCursor += IO_Elements_2B_Quantity * 3 + 1;

                    int IO_Elements_4B_Quantity = Convert.ToInt32(receiveBytes.Skip(currentCursor).Take(1).ToList()[0]);
                    //ShowDiagnosticInfo("4 byte IO element in record: --------".PadRight(40, '-') + " " + IO_Elements_4B_Quantity);

                    for (int IO_4 = 0; IO_4 < IO_Elements_4B_Quantity; IO_4++)
                    {
                        var parameterID = (byte)Convert.ToInt32(receiveBytes.Skip(currentCursor + 1 + IO_4 * 5).Take(1).ToList()[0]);
                        string value = string.Empty;
                        receiveBytes.Skip(currentCursor + 2 + IO_4 * 5).Take(4).ToList().ForEach(delegate (byte b) { value += String.Format("{0:X2}", b); });
                        var IO_Element_4B = Convert.ToInt32(value, 16);
                        gpsData.IO_Elements_4B.Add(parameterID, IO_Element_4B);
                        //ShowDiagnosticInfo("IO element 4B ID: --------".PadRight(40, '-') + " " + parameterID);
                        //ShowDiagnosticInfo(IO_4 + "'st 4B IO element value: --------".PadRight(40 - IO_4.ToString().Length, '-') + " " + IO_Element_4B);
                    }
                    currentCursor += IO_Elements_4B_Quantity * 5 + 1;

                    int IO_Elements_8B_Quantity = Convert.ToInt32(receiveBytes.Skip(currentCursor).Take(1).ToList()[0]);
                    //ShowDiagnosticInfo("8 byte IO element in record: --------".PadRight(40, '-') + " " + IO_Elements_8B_Quantity);

                    for (int IO_8 = 0; IO_8 < IO_Elements_8B_Quantity; IO_8++)
                    {
                        var parameterID = (byte)Convert.ToInt32(receiveBytes.Skip(currentCursor + 1 + IO_8 * 9).Take(1).ToList()[0]);
                        string value = string.Empty;
                        receiveBytes.Skip(currentCursor + 2 + IO_8 * 9).Take(8).ToList().ForEach(delegate (byte b) { value += String.Format("{0:X2}", b); });
                        var IO_Element_8B = Convert.ToInt64(value, 16);
                        gpsData.IO_Elements_8B.Add(parameterID, IO_Element_8B);
                        //ShowDiagnosticInfo("IO element 8B ID: --------".PadRight(40, '-') + " " + parameterID);
                        //ShowDiagnosticInfo(IO_8 + "'st 8B IO element value: --------".PadRight(40 - IO_8.ToString().Length, '-') + " " + IO_Element_8B);
                    }

                    tokenAddress += 30 + IO_Elements_1B_Quantity * 2 +
                        IO_Elements_2B_Quantity * 3 + IO_Elements_4B_Quantity * 5
                        + IO_Elements_8B_Quantity * 9;
                }
                else
                {
                    tokenAddress += 30;
                }


                var redisTime = Redis.GetCacheData<long>(deviceId + "_DataTimeStamp");
                var redisLat = Redis.GetCacheData<int>(deviceId + "_DataLat");
                var redisLon = Redis.GetCacheData<int>(deviceId + "_DataLong");
                var minTime = DateTimeOffset.Now.AddDays(-365).ToUnixTimeSeconds();

                var powerOn = false;
                try
                {
                    if (/*gpsData.IO_Elements_1B[1] == 1 &&*/ (((double)gpsData.IO_Elements_2B[66]) / 1000) > 13.5)
                    {
                        Redis.SetCacheData(deviceId + "_VehiclePower", "1");
                        powerOn = true;
                    }
                    else
                        Redis.SetCacheData(deviceId + "_VehiclePower", "0");
                }
                catch (Exception ex)
                {
                    Redis.SetCacheData(deviceId + "_VehiclePower", "0");
                }


                Data newData = new Data()
                {
                    DeviceId = deviceId,
                    DataDeviceTime = deviceTime,
                    DataServerTime = DateTime.Now,
                    DataLongitude = longtitude,
                    DataLatitude = latitude,
                    DataAltitude = altitude,
                    DataAngel = angle,
                    DataSpeed = (byte)speed,
                    DataSatellites = satellites,


                    DataDigitalIn = Convert.ToBoolean(gpsData.IO_Elements_1B[1]),/////////
                    DataAnalogIn = gpsData.IO_Elements_2B[9],////////
                    DataDeviceBatteryVoltage = (((double)gpsData.IO_Elements_2B[67]) / 1000),
                    DataVehicleBatteryVoltage = (((double)gpsData.IO_Elements_2B[66]) / 1000),
                    DataGSMState = gpsData.IO_Elements_1B[21],
                    DataDeviceBatteryPercent = gpsData.IO_Elements_1B[113],
                    DataCellId = gpsData.IO_Elements_2B[205],
                    DataAreaCode = gpsData.IO_Elements_2B[206],
                    DataGSMOperatorCode = (short)gpsData.IO_Elements_4B[241],
                    DataDeviceDistanceTraveled = (short)gpsData.IO_Elements_4B[199],
                    DataDistanceTraveled = CalculateDistance(redisLat, redisLon, latitude, longtitude),
                };
                db.Data.Add(newData);

                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ShowDiagnosticInfo("Exception in CRUD : " + ex.Message);
                    Log l = new Log()
                    {
                        Date = DateTime.Now,
                        LogType = "CRUD Exception",
                        LogContent = ex.Message + (ex.InnerException != null && ex.InnerException.InnerException != null ? "\n" + ex.InnerException.InnerException.Message : "")
                    };
                    db.Logs.Add(l);
                    db.SaveChanges();
                }

                if (timeStamp > redisTime && minTime < timeStamp)
                {
                    Redis.SetCacheData(deviceId + "_DataTimeStamp", timeStamp.ToString());
                    Redis.SetCacheData(deviceId + "_DataLat", latitude.ToString());
                    Redis.SetCacheData(deviceId + "_DataLong", longtitude.ToString());
                    Redis.SetCacheData(deviceId + "_DataAlt", altitude.ToString());
                    Redis.SetCacheData(deviceId + "_DataAngle", angle.ToString());
                    Redis.SetCacheData(deviceId + "_DataSpeed", speed.ToString());
                    Redis.SetCacheData(deviceId + "_DataSatellites", satellites.ToString());
                    Redis.SetCacheData(deviceId + "_DataDeviceBatteryVoltage", (((double)gpsData.IO_Elements_2B[67]) / 1000).ToString());
                }

                //if (timeStamp > redisTime && (latitude != redisLat || longtitude != redisLon) && minTime < timeStamp)
                //{
                //    if (powerOn)
                //    {
                //        Data newData = new Data()
                //        {
                //            DeviceId = deviceId,
                //            DataDeviceTime = deviceTime,
                //            DataServerTime = DateTime.Now,
                //            DataLongitude = longtitude,
                //            DataLatitude = latitude,
                //            DataAltitude = altitude,
                //            DataAngel = angle,
                //            DataSpeed = (byte)speed,
                //            DataSatellites = satellites,


                //            DataDigitalIn = Convert.ToBoolean(gpsData.IO_Elements_1B[1]),/////////
                //            DataAnalogIn = gpsData.IO_Elements_2B[9],////////
                //            DataDeviceBatteryVoltage = (((double)gpsData.IO_Elements_2B[67]) / 1000),
                //            DataVehicleBatteryVoltage = (((double)gpsData.IO_Elements_2B[66]) / 1000),
                //            DataGSMState = gpsData.IO_Elements_1B[21],
                //            DataDeviceBatteryPercent = gpsData.IO_Elements_1B[113],
                //            DataCellId = gpsData.IO_Elements_2B[205],
                //            DataAreaCode = gpsData.IO_Elements_2B[206],
                //            DataGSMOperatorCode = (short)gpsData.IO_Elements_4B[241],
                //            DataDeviceDistanceTraveled = (short)gpsData.IO_Elements_4B[199],
                //            DataDistanceTraveled = CalculateDistance(redisLat, redisLon, latitude, longtitude),
                //        };
                //        db.Data.Add(newData);
                //        if (redisTime > 0)
                //        {
                //            SqlParameter[] param = new SqlParameter[]{
                //            new SqlParameter("@DeviceId", deviceId)
                //            ,new SqlParameter("@ReportDate", deviceTime)
                //            ,new SqlParameter("@TotalMovingTime", (timeStamp - redisTime) / 1000)
                //            ,new SqlParameter("@TotalMovingDistance", newData.DataDistanceTraveled)
                //            ,new SqlParameter("@TotalParkTime", DBNull.Value)
                //            ,new SqlParameter("@TotalStandByTime", DBNull.Value)
                //            ,new SqlParameter("@TotalTowingDistance", DBNull.Value)
                //            ,new SqlParameter("@TotalTowingTime", DBNull.Value)
                //            };
                //            db.Database.ExecuteSqlCommand(@"USP_Rpt_DailyPerformances_InsertOrUpdate 
                //                                            @DeviceId, 
                //                                            @ReportDate, 
                //                                            @TotalMovingTime,
                //                                            @TotalMovingDistance,
                //                                            @TotalParkTime,
                //                                            @TotalStandByTime,
                //                                            @TotalTowingTime, 
                //                                            @TotalTowingDistance", param);
                //        }
                //    }
                //    else
                //    {
                //        TowingData newData = new TowingData()
                //        {
                //            DeviceId = deviceId,
                //            TowingDataDeviceTime = deviceTime,
                //            TowingDataServerTime = DateTime.Now,
                //            TowingDataLongitude = longtitude,
                //            TowingDataLatitude = latitude,
                //            TowingDataAltitude = altitude,
                //            TowingDataAngel = angle,
                //            TowingDataSpeed = (byte)speed,
                //            TowingDataSatellites = satellites,


                //            TowingDataDigitalIn = Convert.ToBoolean(gpsData.IO_Elements_1B[1]),/////////
                //            TowingDataAnalogIn = gpsData.IO_Elements_2B[9],////////
                //            TowingDataDeviceBatteryVoltage = (((double)gpsData.IO_Elements_2B[67]) / 1000),
                //            TowingDataVehicleBatteryVoltage = (((double)gpsData.IO_Elements_2B[66]) / 1000),
                //            TowingDataGSMState = gpsData.IO_Elements_1B[21],
                //            TowingDataDeviceBatteryPercent = gpsData.IO_Elements_1B[113],
                //            TowingDataCellId = gpsData.IO_Elements_2B[205],
                //            TowingDataAreaCode = gpsData.IO_Elements_2B[206],
                //            TowingDataGSMOperatorCode = (short)gpsData.IO_Elements_4B[241],
                //            TowingDataDeviceDistanceTraveled = (short)gpsData.IO_Elements_4B[199],
                //            TowingDataDistanceTraveled = CalculateDistance(redisLat, redisLon, latitude, longtitude),
                //        };
                //        db.TowingData.Add(newData);
                //        if (redisTime > 0)
                //        {
                //            SqlParameter[] param = new SqlParameter[]{
                //            new SqlParameter("@DeviceId", deviceId)
                //            ,new SqlParameter("@ReportDate", deviceTime)
                //            ,new SqlParameter("@TotalMovingTime", DBNull.Value)
                //            ,new SqlParameter("@TotalMovingDistance", DBNull.Value)
                //            ,new SqlParameter("@TotalParkTime", DBNull.Value)
                //            ,new SqlParameter("@TotalStandByTime", DBNull.Value)
                //            ,new SqlParameter("@TotalTowingDistance", newData.TowingDataDistanceTraveled)
                //            ,new SqlParameter("@TotalTowingTime", (timeStamp - redisTime) / 1000)
                //            };
                //            db.Database.ExecuteSqlCommand(@"USP_Rpt_DailyPerformances_InsertOrUpdate 
                //                                            @DeviceId, 
                //                                            @ReportDate, 
                //                                            @TotalMovingTime,
                //                                            @TotalMovingDistance,
                //                                            @TotalParkTime,
                //                                            @TotalStandByTime,
                //                                            @TotalTowingTime, 
                //                                            @TotalTowingDistance", param);
                //        }
                //    }
                //    try
                //    {
                //        db.SaveChanges();
                //    }
                //    catch (Exception ex)
                //    {
                //        ShowDiagnosticInfo("Exception in CRUD : " + ex.Message);
                //        Log l = new Log()
                //        {
                //            Date = DateTime.Now,
                //            LogType = "CRUD Exception",
                //            LogContent = ex.Message + (ex.InnerException!= null && ex.InnerException.InnerException != null ? "\n" + ex.InnerException.InnerException.Message : "")
                //        };
                //        db.Logs.Add(l);
                //        db.SaveChanges();
                //    }

                //                Redis.SetCacheData(deviceId + "_DataTimeStamp", timeStamp.ToString());
                //                Redis.SetCacheData(deviceId + "_DataLat", latitude.ToString());
                //                Redis.SetCacheData(deviceId + "_DataLong", longtitude.ToString());
                //                Redis.SetCacheData(deviceId + "_DataAlt", altitude.ToString());
                //                Redis.SetCacheData(deviceId + "_DataAngle", angle.ToString());
                //                Redis.SetCacheData(deviceId + "_DataSpeed", speed.ToString());
                //                Redis.SetCacheData(deviceId + "_DataSatellites", satellites.ToString());
                //                Redis.SetCacheData(deviceId + "_DataDeviceBatteryVoltage", (((double)gpsData.IO_Elements_2B[67]) / 1000).ToString());



                //    var parkStartTime = Redis.GetCacheData(deviceId + "_ParkStartTime");
                //    var standbyStartTime = Redis.GetCacheData(deviceId + "_StandbyStartTime");
                //    if (!String.IsNullOrEmpty(parkStartTime))
                //        SaveStandByPoint(IMEI, deviceId, deviceTime);
                //    if (!String.IsNullOrEmpty(standbyStartTime))
                //        SaveStandByPoint(IMEI, deviceId, deviceTime);




                //    gpsData.Priority = (byte)priority;
                //}

                //else if (timeStamp > redisTime && minTime < timeStamp)
                //{
                //    Redis.SetCacheData(deviceId + "_LastDataTime", deviceTime.ToString());
                //    var parkStartTime = Redis.GetCacheData(deviceId + "_ParkStartTime");
                //    var standbyStartTime = Redis.GetCacheData(deviceId + "_StandbyStartTime");
                //    if (!powerOn && String.IsNullOrEmpty(parkStartTime))
                //    {
                //        Redis.SetCacheData(deviceId + "_ParkLat", latitude.ToString());
                //        Redis.SetCacheData(deviceId + "_ParkLong", longtitude.ToString());
                //        Redis.SetCacheData(deviceId + "_ParkStartTime", deviceTime.ToString());

                //        if (!String.IsNullOrEmpty(parkStartTime))
                //            SaveStandByPoint(IMEI, deviceId, deviceTime);
                //    }
                //    if (powerOn && String.IsNullOrEmpty(standbyStartTime))
                //    {
                //        Redis.SetCacheData(deviceId + "_StandbyLat", latitude.ToString());
                //        Redis.SetCacheData(deviceId + "_StandbyLong", longtitude.ToString());
                //        Redis.SetCacheData(deviceId + "_StandbyStartTime", deviceTime.ToString());

                //        if (!String.IsNullOrEmpty(parkStartTime))
                //            SaveParkPoint(IMEI, deviceId, deviceTime);
                //    }

                //}
                //ShowDiagnosticInfo("Timestamp: -----".PadRight(40, '-') + " " + deviceTime.ToLongDateString() + " " + deviceTime.ToLongTimeString());
            }
            //CRC for check of data correction and request again data from device if it not correct
            string crcString = string.Empty;
            receiveBytes.Skip(dataLength + 8).Take(4).ToList().ForEach(delegate (byte b) { crcString += String.Format("{0:X2}", b); });
            int CRC = Convert.ToInt32(crcString, 16);
            //ShowDiagnosticInfo("CRC: -----".PadRight(40, '-') + " " + CRC);
            //We must skeep first 8 bytes and last 4 bytes with CRC value.
            int calculatedCRC = GetCRC16(receiveBytes.Skip(8).Take(receiveBytes.Count - 12).ToArray());
            //ShowDiagnosticInfo("Calculated CRC: -------".PadRight(40, '-') + " " + calculatedCRC);
            //ShowDiagnosticInfo("||||||||||||||||||||||||||||||||||||||||||||||||");
            if (calculatedCRC == CRC)
                return numberOfData;
            else
            {
                //ShowDiagnosticInfo("Incorect CRC ");
                return 0;
            }
        }
        private void SaveParkPoint(string imei, short deviceId, DateTime endTime)
        {
            var startTime = Redis.GetCacheData<DateTime>(deviceId + "_ParkStartTime");
            ParkPoint point = new ParkPoint()
            {
                ParkDeviceId = deviceId,
                ParkStartTime = startTime,
                ParkEndTime = endTime,
                ParkLatitude = Redis.GetCacheData<int>(deviceId + "_ParkLat"),
                ParkLongitude = Redis.GetCacheData<int>(deviceId + "_ParkLong")
            };

            SqlParameter[] param = new SqlParameter[]{
                            new SqlParameter("@DeviceId", deviceId)
                            ,new SqlParameter("@ReportDate", endTime)
                            ,new SqlParameter("@TotalMovingTime", DBNull.Value)
                            ,new SqlParameter("@TotalMovingDistance", DBNull.Value)
                            ,new SqlParameter("@TotalParkTime", (endTime - startTime).TotalSeconds)
                            ,new SqlParameter("@TotalStandByTime", DBNull.Value)
                            ,new SqlParameter("@TotalTowingDistance", DBNull.Value)
                            ,new SqlParameter("@TotalTowingTime", DBNull.Value)
                            };
            db.Database.ExecuteSqlCommand(@"USP_Rpt_DailyPerformances_InsertOrUpdate 
                                                            @DeviceId, 
                                                            @ReportDate, 
                                                            @TotalMovingTime,
                                                            @TotalMovingDistance,
                                                            @TotalParkTime,
                                                            @TotalStandByTime,
                                                            @TotalTowingTime, 
                                                            @TotalTowingDistance", param);

            Redis.KeyDelete(deviceId + "_ParkLat");
            Redis.KeyDelete(deviceId + "_ParkLong");
            Redis.KeyDelete(deviceId + "_ParkStartTime");
            db.ParkPoints.Add(point);

            try { db.SaveChanges(); }
            catch (Exception ex)
            {
                ShowDiagnosticInfo("Exception in SaveParkPoint : " + ex.Message);
                Log l = new Log()
                {
                    Date = DateTime.Now,
                    LogType = "SaveParkPoint Exception",
                    LogContent = ex.Message + (ex.InnerException != null ? "\n" + ex.InnerException.Message : "")
                };
                db.Logs.Add(l);
                db.SaveChanges();
            }
        }
        private void SaveStandByPoint(string imei, short deviceId, DateTime endTime)
        {
            var startTime = Redis.GetCacheData<DateTime>(deviceId + "_StandbyStartTime");
            StandbyPoint point = new StandbyPoint()
            {
                StandbyDeviceId = deviceId,
                StandbyStartTime = startTime,
                StandbyEndTime = endTime,
                StandbyLatitude = Redis.GetCacheData<int>(deviceId + "_StandbyLat"),
                StandbyLongitude = Redis.GetCacheData<int>(deviceId + "_StandByLong")
            };


            SqlParameter[] param = new SqlParameter[]{
                            new SqlParameter("@DeviceId", deviceId)
                            ,new SqlParameter("@ReportDate", endTime)
                            ,new SqlParameter("@TotalMovingTime", DBNull.Value)
                            ,new SqlParameter("@TotalMovingDistance", DBNull.Value)
                            ,new SqlParameter("@TotalParkTime", DBNull.Value)
                            ,new SqlParameter("@TotalStandByTime", (endTime - startTime).TotalSeconds)
                            ,new SqlParameter("@TotalTowingDistance", DBNull.Value)
                            ,new SqlParameter("@TotalTowingTime", DBNull.Value)
                            };
            db.Database.ExecuteSqlCommand(@"USP_Rpt_DailyPerformances_InsertOrUpdate 
                                                            @DeviceId, 
                                                            @ReportDate, 
                                                            @TotalMovingTime,
                                                            @TotalMovingDistance,
                                                            @TotalParkTime,
                                                            @TotalStandByTime,
                                                            @TotalTowingTime, 
                                                            @TotalTowingDistance", param);

            Redis.KeyDelete(deviceId + "_StandbyLat");
            Redis.KeyDelete(deviceId + "_StandbyLong");
            Redis.KeyDelete(deviceId + "_StandbyStartTime");
            db.StandbyPoints.Add(point);


            try { db.SaveChanges(); }
            catch (Exception ex)
            {
                ShowDiagnosticInfo("Exception in SaveParkPoint : " + ex.Message);
                Log l = new Log()
                {
                    Date = DateTime.Now,
                    LogType = "SaveParkPoint Exception",
                    LogContent = ex.Message + (ex.InnerException != null ? "\n" + ex.InnerException.Message : "")
                };
                db.Logs.Add(l);
                db.SaveChanges();
            }
        }
        private int GetCRC16(byte[] buffer)
        {
            return GetCRC16(buffer, buffer.Length, 0xA001);
        }
        private int GetCRC16(byte[] buffer, int bufLen, int polynom)
        {
            polynom &= 0xFFFF;
            int crc = 0;
            for (int i = 0; i < bufLen; i++)
            {
                int data = buffer[i] & 0xFF;
                crc ^= data;
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x0001) != 0)
                    {
                        crc = (crc >> 1) ^ polynom;
                    }
                    else
                    {
                        crc = crc >> 1;
                    }
                }
            }
            return crc & 0xFFFF;
        }
        private short CalculateDistance(int lat1, int long1, int lat2, int long2)
        {
            short result = 0;
            if (lat1 == 0 || long1 == 0 || lat2 == 0 || long2 == 0)
                return 0;
            try
            {
                GeoCoordinate point1 = new GeoCoordinate(((double)lat1 / 10000000), ((double)long1 / 10000000));
                GeoCoordinate point2 = new GeoCoordinate(((double)lat2 / 10000000), ((double)long2 / 10000000));

                result = (short)point1.GetDistanceTo(point2);
            }
            catch (Exception ex)
            {
                Log l = new Log()
                {
                    Date = DateTime.Now,
                    LogType = "CalculateDistance Exception",
                    LogContent = ex.Message + (ex.InnerException != null ? "\n" + ex.InnerException.Message : "")
                };
                db.Logs.Add(l);
                db.SaveChanges();
            }
            return result;
        }
    }
}
